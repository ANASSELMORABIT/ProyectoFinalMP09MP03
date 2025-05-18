// Este archivo controla el slider, la lista y el reproductor de la playlist
// playlistData debe ser definido en la vista Razor antes de este script

let currentSongIndex = 0;
let isSongLoaded = false;
const audioPlayer = document.getElementById("audioPlayer");
const playPauseBtn = document.getElementById("playPauseBtn");
const playPauseIcon = document.getElementById("playPauseIcon");
const prevBtn = document.getElementById("prevBtn");
const nextBtn = document.getElementById("nextBtn");
const shuffleBtn = document.getElementById("shuffleBtn");
const volumeRange = document.getElementById("volume-range");
const progressBar = document.getElementById("progress-bar");
const playlistList = document.querySelector(".playlist");
const polaroidStack = document.querySelector(".polaroid-stack");
let currentBlobUrl = null;

const uniqueTracks = [];
const seenNames = new Set();
playlistData.forEach(track => {
  if (!seenNames.has(track.nombreCancion)) {
    uniqueTracks.push(track);
    seenNames.add(track.nombreCancion);
  }
});

// --- Cargar Duraciones ---
function formatTime(seconds) {
  if (isNaN(seconds)) return "--:--";
  const min = Math.floor(seconds / 60);
  const sec = Math.floor(seconds % 60).toString().padStart(2, "0");
  return `${min}:${sec}`;
}

function setDurations() {
  playlistData.forEach((track, i) => {
    const audio = new Audio(track.urlMusica);
    audio.addEventListener("loadedmetadata", () => {
      const dur = formatTime(audio.duration);
      const durElem = document.querySelector(`.playlist-item[data-index='${i}'] .duration`);
      if (durElem) durElem.textContent = dur;
    });
  });
}

// --- Render Polaroid Stack ---
function renderPolaroidStack() {
  polaroidStack.innerHTML = "";
  playlistData.forEach((track, i) => {
    const card = document.createElement("div");
    card.className = "polaroid-card" + (i === currentSongIndex ? " active" : "");
    card.style.zIndex = playlistData.length - i;
    card.style.transform = `translateX(${(i - currentSongIndex) * 40}px) rotate(${(i - currentSongIndex) * 3}deg)`;
    card.innerHTML = `
      <img src="${track.urlImg}" alt="${track.nombreCancion}" />
      <div class="polaroid-title">${track.nombreCancion}</div>
    `;
    card.addEventListener("click", () => {
      selectSong(i);
    });
    polaroidStack.appendChild(card);
  });
}

// --- Render Playlist List ---
function renderPlaylistList() {
  playlistList.innerHTML = "";
  playlistData.forEach((track, i) => {
    const item = document.createElement("div");
    item.className = "playlist-item" + (i === currentSongIndex ? " active-playlist-item" : "");
    item.dataset.index = i;
    item.innerHTML = `
      <img src="${track.urlImg}" alt="${track.nombreCancion}" />
      <div class="song">
        <p>${track.artista}</p>
        <p>${track.nombreCancion}</p>
      </div>
      <span class="duration">--:--</span>
      <i class="fa-regular fa-heart like-btn"></i>
    `;
    item.addEventListener("click", () => {
      selectSong(i);
    });
    playlistList.appendChild(item);
  });
}

// --- Selección y reproducción ---
function selectSong(index) {
  currentSongIndex = index;
  loadAndPlaySong(index);
  renderPolaroidStack();
  renderPlaylistList();
  setDurations();
}

// --- SWIPER Y REPRODUCTOR CON INDEXEDDB ---

// Inicializa Swiper
var swiper = new Swiper('.swiper', {
  effect: 'cards',
  cardsEffect: {
    perSlideOffset: 9,
    perSlideRotate: 3,
  },
  grabCursor: true,
  speed: 700,
  initialSlide: 0,
});

// IndexedDB helpers
const DB_NAME = 'playlist-audio-db';
const STORE_NAME = 'audios';
function openDB() {
  return new Promise((resolve, reject) => {
    const req = indexedDB.open(DB_NAME, 1);
    req.onupgradeneeded = () => {
      req.result.createObjectStore(STORE_NAME);
    };
    req.onsuccess = () => resolve(req.result);
    req.onerror = () => reject(req.error);
  });
}
function saveAudioToDB(key, blob) {
  return openDB().then(db => {
    return new Promise((resolve, reject) => {
      const tx = db.transaction(STORE_NAME, 'readwrite');
      tx.objectStore(STORE_NAME).put(blob, key);
      tx.oncomplete = () => resolve();
      tx.onerror = () => reject(tx.error);
    });
  });
}
function getAudioFromDB(key) {
  return openDB().then(db => {
    return new Promise((resolve, reject) => {
      const tx = db.transaction(STORE_NAME, 'readonly');
      const req = tx.objectStore(STORE_NAME).get(key);
      req.onsuccess = () => resolve(req.result);
      req.onerror = () => reject(req.error);
    });
  });
}
function deleteAudioFromDB(key) {
  return openDB().then(db => {
    return new Promise((resolve, reject) => {
      const tx = db.transaction(STORE_NAME, 'readwrite');
      tx.objectStore(STORE_NAME).delete(key);
      tx.oncomplete = () => resolve();
      tx.onerror = () => reject(tx.error);
    });
  });
}

// --- Lógica de reproducción ---
async function loadAndPlaySong(index) {
  // Limpia el blob anterior
  if (currentBlobUrl) {
    URL.revokeObjectURL(currentBlobUrl);
    currentBlobUrl = null;
  }
  await deleteAudioFromDB('current-audio').catch(()=>{});

  // 1. Pide el audio al backend
  const id = playlistData[index].id;
  const res = await fetch(`/Music/GetTrackAudioUrl?idTrack=${encodeURIComponent(id)}`);
  const data = await res.json();
  const mp3Url = data.url;
  try {
    const audioRes = await fetch(mp3Url);
    const blob = await audioRes.blob();
    await saveAudioToDB('current-audio', blob);
    currentBlobUrl = URL.createObjectURL(blob);
    audioPlayer.src = currentBlobUrl;
    audioPlayer.load();
    playSong();
    isSongLoaded = true;
    updatePlayPauseIcon(true);
  } catch (e) {
    alert('No se pudo descargar el audio');
  }
  updatePlaylistHighlight(index);
  updateSwiperToMatchSong(index);
}

function playSong() {
  audioPlayer.play();
  updatePlayPauseIcon(true);
}
function pauseSong() {
  audioPlayer.pause();
  updatePlayPauseIcon(false);
}
function togglePlayPause() {
  if (!isSongLoaded) {
    loadAndPlaySong(currentSongIndex);
    isSongLoaded = true;
  } else if (audioPlayer.paused) {
    playSong();
  } else {
    pauseSong();
  }
}
function updatePlayPauseIcon(isPlaying) {
  if (isPlaying) {
    playPauseIcon.classList.add('fa-pause');
    playPauseIcon.classList.remove('fa-play');
  } else {
    playPauseIcon.classList.add('fa-play');
    playPauseIcon.classList.remove('fa-pause');
  }
}
function nextSong() {
  currentSongIndex = (currentSongIndex + 1) % playlistData.length;
  loadAndPlaySong(currentSongIndex);
  swiper.slideTo(currentSongIndex);
}
function prevSong() {
  currentSongIndex = (currentSongIndex - 1 + playlistData.length) % playlistData.length;
  loadAndPlaySong(currentSongIndex);
  swiper.slideTo(currentSongIndex);
}
function updatePlaylistHighlight(index) {
  document.querySelectorAll('.playlist-item').forEach((item, i) => {
    if (i === index) {
      item.classList.add('active-playlist-item');
    } else {
      item.classList.remove('active-playlist-item');
    }
  });
}
function updateSwiperToMatchSong(index) {
  if (swiper.activeIndex !== index) {
    swiper.slideTo(index);
  }
}

// --- Player Info ---
function updatePlayerInfo() {
  const info = document.querySelector(".player-info");
  if (info) {
    info.innerHTML = `<strong>${playlistData[currentSongIndex].artista}</strong> - ${playlistData[currentSongIndex].nombreCancion}`;
  }
}

// --- Eventos ---
playPauseBtn.addEventListener('click', togglePlayPause);
nextBtn.addEventListener('click', nextSong);
prevBtn.addEventListener('click', prevSong);
shuffleBtn.addEventListener('click', () => {
  let randomIndex = Math.floor(Math.random() * playlistData.length);
  if (randomIndex === currentSongIndex) randomIndex = (randomIndex + 1) % playlistData.length;
  currentSongIndex = randomIndex;
  loadAndPlaySong(currentSongIndex);
  swiper.slideTo(currentSongIndex);
});
volumeRange.addEventListener('input', () => {
  audioPlayer.volume = volumeRange.value / 100;
});
audioPlayer.addEventListener('loadedmetadata', () => {
  progressBar.max = audioPlayer.duration;
  progressBar.value = audioPlayer.currentTime;
});
audioPlayer.addEventListener('timeupdate', () => {
  if (!audioPlayer.paused) {
    progressBar.value = audioPlayer.currentTime;
  }
});
progressBar.addEventListener('input', () => {
  audioPlayer.currentTime = progressBar.value;
});
audioPlayer.addEventListener('ended', nextSong);

// --- Sincronización Swiper <-> Playlist ---
swiper.on('slideChange', () => {
  const newIndex = swiper.realIndex;
  if (newIndex !== currentSongIndex) {
    currentSongIndex = newIndex;
    loadAndPlaySong(currentSongIndex);
    updatePlayPauseIcon(true);
  }
});

document.querySelectorAll('.playlist-item').forEach((item, index) => {
  item.addEventListener('click', () => {
    currentSongIndex = index;
    loadAndPlaySong(index);
    swiper.slideTo(index);
  });
});

// --- Limpieza al salir de la playlist ---
window.addEventListener('beforeunload', () => {
  if (currentBlobUrl) URL.revokeObjectURL(currentBlobUrl);
  deleteAudioFromDB('current-audio');
});

document.addEventListener("DOMContentLoaded", () => {
  renderPolaroidStack();
  renderPlaylistList();
  setDurations();
  updatePlayerInfo();
});

// Cargar duración de cada canción
document.querySelectorAll('.playlist-item').forEach((item, i) => {
  const audio = new Audio(songs[i]);
  audio.addEventListener('loadedmetadata', () => {
    const dur = formatTime(audio.duration);
    item.querySelector('.duration').textContent = dur;
  });
}); 
