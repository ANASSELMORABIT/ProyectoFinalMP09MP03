# Proyecto Final DAW: App Musical Version 2.0

> **⚠️ NOTA IMPORTANTE:**
> **Marc, si el funcionamiento de la API no devuelve los datos al hacer la búsqueda o petición, es debido a que la API ha colapsado su servidor.**
> **Esto es un problema persistente en las APIs que estamos usando de Spotify y servicios relacionados.**

**Autores:** Rafael Valerio y Anass El Morabit  
**Módulos:** M9 y M3  
**Curso:** Segundo de DAW  

---
***Link Hosting WebApp**
> ** http://raffaele26etp-001-site1.ntempurl.com/ **




***Prueba Wave**
> **Marc, la prueba del wave no me muestra los estilos aplicados y no deja avanzar del inicio de sesion esto solo sucede en el validator del wave.**
>
> ![prueba wave](https://github.com/RVDveloper/M9M3Final/blob/main/PRJ-FINAL%20MP09-MP03/SCR-20250518-mjoy.png)


## Justificación y motivación del proyecto

Este proyecto nace de la pasión por la música y la tecnología, con el objetivo de crear una plataforma web moderna donde los usuarios puedan descubrir, buscar y gestionar música de manera intuitiva y visualmente atractiva. La motivación principal es aplicar los conocimientos adquiridos en los módulos de DAW, especialmente en el desarrollo de aplicaciones web con arquitectura MVC, y ofrecer una experiencia de usuario innovadora, accesible y adaptada a los estándares actuales.

---

## Esquema de arquitectura

El proyecto sigue la arquitectura **Modelo-Vista-Controlador (MVC)**, separando claramente la lógica de negocio, la presentación y el control de flujo de la aplicación.

```
[Usuario]
   |
   v
[Vista (Views)] <----> [Controlador (Controllers)] <----> [Modelo (Models)]
```

- **Vista (Views):** Interfaz de usuario, páginas Razor con HTML, CSS y JS.
- **Controlador (Controllers):** Gestiona las peticiones, procesa la lógica y comunica modelos y vistas.
- **Modelo (Models):** Representa los datos y la lógica de negocio, incluyendo acceso a la base de datos.

---

## Explicación detallada del código y la arquitectura MVC

- **Modelos:**
  - Definen las entidades principales: usuarios, canciones, artistas, álbumes, playlists, etc.
  - Gestionan la lógica de negocio y la persistencia de datos (Entity Framework, SQLite).

- **Controladores:**
  - Reciben las peticiones del usuario (por ejemplo, buscar una canción, ver detalles de un artista, gestionar playlists).
  - Procesan la lógica necesaria y seleccionan la vista adecuada para mostrar la respuesta.
  - Ejemplo: `MusicController` gestiona la lógica de recomendaciones, tendencias, detalles de canciones, etc.

- **Vistas:**
  - Páginas Razor que muestran la información al usuario con un diseño moderno, animaciones y efectos glass.
  - Incluyen formularios de búsqueda, listados de canciones, detalles, reproductor musical, gestión de playlists, etc.
  - Se ha puesto especial atención a la accesibilidad (WAVE), el responsive y la experiencia de usuario.

- **Estilo y scripts:**
  - CSS avanzado para efectos glass, sidebar, topbar, cards animadas y reproductor fijo.
  - JavaScript para la gestión del reproductor, notificaciones (SweetAlert, Toast), y la interacción dinámica.

---

## Detalles de las partes más esenciales del código y funcionalidad

### 1. **Búsqueda de canciones**

- Permite al usuario buscar canciones por nombre desde la barra superior.
- El controlador recibe la petición, consulta la base de datos o la API y muestra los resultados en la vista.
- Mejora la experiencia permitiendo encontrar música rápidamente.

### 2. **Recomendaciones musicales**

- El Dashboard muestra recomendaciones personalizadas según el historial o preferencias del usuario.
- Cada recomendación se presenta en una card con imagen, título, artista y botón de detalles.
- El controlador gestiona la lógica de recomendación y la vista las muestra de forma atractiva.

### 3. **Tendencias (Trending)**

- Sección que muestra las canciones, artistas y álbumes más populares del momento.
- Permite filtrar por día, semana o mes.
- Cada elemento trending tiene su propia card con imagen, nombre y acceso a detalles.

### 4. **Gestión de playlists**

- Los usuarios pueden crear, ver y gestionar sus propias playlists.
- Las playlists se muestran con carátulas y listado de canciones.
- Incluye funcionalidad para dar like, eliminar o reproducir canciones directamente desde la playlist.

### 5. **Reproductor musical avanzado**

- Reproductor fijo en la parte inferior de la pantalla.
- Permite reproducir, pausar, avanzar, retroceder, ajustar volumen y ver el progreso de la canción.
- Sincronizado con las cards y la playlist para reproducir cualquier canción seleccionada.

### 6. **Notificaciones y alertas (SweetAlert, Toast)**

- Uso de SweetAlert para confirmaciones importantes (logout, acciones críticas).
- Toasts personalizados para feedback rápido (canción añadida, error, etc.).
- Mejora la interacción y la accesibilidad.

---

## Propuestas de mejora y nuevas funcionalidades

- **Mejorar la gestión de usuarios:** Añadir roles, perfiles avanzados y personalización.
- **Integración con APIs externas:** Permitir importar playlists de Spotify, Apple Music, etc.
- **Sistema de comentarios y valoraciones:** Para canciones, álbumes y artistas.
- **Reproductor avanzado:** Soporte para listas de reproducción en cola, letras sincronizadas y visualizaciones.
- **Modo oscuro/tema personalizado:** Para mayor personalización del usuario.
- **PWA (Progressive Web App):** Para acceso offline y notificaciones push.
- **Internacionalización:** Soporte multilingüe para llegar a más usuarios.

---

## Diagrama de arquitectura

![Diagrama de arquitectura MVC](https://github.com/RVDveloper/M9M3Final/blob/main/PRJ-FINAL%20MP09-MP03/76fe85cb-9804-4b91-948b-7f9d3989f97e.JPG)

---

## Ejemplos de secciones de código relevantes

### Búsqueda de canciones (Vista y Controlador)

```csharp
// MusicController.cs
public IActionResult BuscarTrackId(string nombreCancion)
{
    var resultados = _musicService.BuscarCanciones(nombreCancion);
    return View("ResultadosBusqueda", resultados);
}
```

```html
<!-- Dashboard.cshtml -->
<form method="get" asp-controller="Music" asp-action="BuscarTrackId">
    <label for="nombreCancion" class="visually-hidden">Buscar canción</label>
    <input type="text" id="nombreCancion" name="nombreCancion" placeholder="Buscar canción..." required />
</form>
```

### Recomendaciones musicales (Vista)

```html
@foreach (var rec in Model.Recommendations)
{
    var song = rec.recommended_song;
    <div class="card h-100 shadow-sm">
        <img src="@song.song_art_image_url" class="card-img-top" alt="Imagen de @song.title">
        <div class="card-body">
            <h5 class="card-title">@song.title</h5>
            <p class="card-text"><strong>Artista:</strong> @song.artist_names</p>
            <a asp-controller="Music" asp-action="SongDetails" asp-route-id="@song.id" class="btn btn-primary mt-2">
                <i class="fas fa-info-circle"></i> Ver Detalles
            </a>
        </div>
    </div>
}
```

### Trending (Vista)

```html
@foreach (var item in Model.Trending)
{
    var song = item.item;
    <div class="trending-card">
        <div class="trending-card__image">
            <img src="@song.song_art_image_url" alt="Imagen de @song.title">
        </div>
        <div class="trending-card__content">
            <h5 class="trending-card__title">@song.title</h5>
            <p class="trending-card__artist">@song.artist_names</p>
            <a asp-controller="Music" asp-action="SongDetails" asp-route-id="@song.id" class="trending-card__button">
                <i class="fas fa-info-circle"></i> Ver Detalles
            </a>
        </div>
    </div>
}
```

### Gestión de playlists (Vista)

```html
@for (int i = 0; i < uniqueModel.Count; i++)
{
    <div class="playlist-item" data-index="@i">
        <img src="@uniqueModel[i].UrlImg" alt="@uniqueModel[i].NombreCancion" />
        <div class="song">
            <p>@uniqueModel[i].Artista</p>
            <p>@uniqueModel[i].NombreCancion</p>
        </div>
        <span class="duration">--:--</span>
        <i class="fa-regular fa-heart like-btn"></i>
    </div>
}
```

### Reproductor musical (JS)

```javascript
// Dashboard.cshtml
let currentAudio = null;
function updatePlayer(track) {
    const albumCover = document.getElementById('playerAlbumCover');
    albumCover.src = track.imageUrl;
    document.getElementById('playerTitle').textContent = track.title;
    document.getElementById('playerArtist').textContent = track.artists;
    if (currentAudio) currentAudio.pause();
    currentAudio = new Audio(track.mp3Url);
    currentAudio.addEventListener('timeupdate', updateProgress);
}
```

### Notificaciones y alertas (SweetAlert)

```javascript
// Dashboard.cshtml y Trending.cshtml

document.querySelectorAll('.logout-link').forEach(link => {
  link.addEventListener('click', function (e) {
    e.preventDefault();
    Swal.fire({
      title: '¿Seguro que quieres cerrar sesión?',
      text: "Se cerrará tu sesión en la aplicación.",
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sí, cerrar sesión',
      background: '#1a1a1a',
      color: '#f1f1f1',
      customClass: {
        popup: 'rounded-4 shadow-lg',
        confirmButton: 'btn btn-danger',
        cancelButton: 'btn btn-secondary me-3'
      }
    }).then((result) => {
      if (result.isConfirmed) {
        window.location.href = '/Account/Logout';
      }
    });
  });
});
```

### Búsqueda dinámica y renderizado de la card (AJAX + API)

```javascript
<code_block_to_apply_changes_from>
async function getTrackUrlFromApi(trackId) {
    // ... (peticiones a las APIs)
    // Mostrar card con los datos
    document.getElementById("downloadResult").innerHTML = `
        <div class="music-card">
            <h2>${title} - ${artists}</h2>
            <img src="${imageUrl}" alt="Portada del álbum">
            <div class="player">
                <audio id="audio-${trackId}" src="${mp3Url}" preload="auto"></audio>
                <div class="controls">
                    <button onclick="document.getElementById('audio-${trackId}').play()"><i class="bi bi-play-fill"></i></button>
                    <button onclick="document.getElementById('audio-${trackId}').pause()"><i class="bi bi-pause-fill"></i></button>
                    <!-- ...otros controles... -->
                </div>
                <input type="range" id="progress-${trackId}" min="0" step="0.1" value="0">
            </div>
            <div class="card-details mt-3">
                <p><strong>Álbum:</strong> ${albumName}</p>
                <p><strong>Fecha de lanzamiento:</strong> ${releaseDate}</p>
                <p><strong>Popularidad:</strong> ${popularity}</p>
                <a href="${mp3UrlMp3}" download class="start-button">⬇️ Descargar MP3</a>
            </div>
        </div>`;
    // ... (gestión de eventos del reproductor)
}
```

**Explicación:**  
Cuando el usuario busca una canción, se realiza una petición a varias APIs externas para obtener todos los datos y el MP3. El resultado se muestra dinámicamente en el dashboard, sin recargar la página, y se genera una card con toda la información y controles de reproducción.

---

**Proyecto realizado por Rafael Valerio y Anass El Morabit**  
**Segundo curso de DAW – Proyecto final M9 y M3**
