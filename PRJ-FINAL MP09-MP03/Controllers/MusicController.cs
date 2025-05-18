using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using PRJ_FINAL_MP09_MP03.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using PRJ_FINAL_MP09_MP03.Models;
using System.Web; // para HttpUtility.UrlEncode
using System.Text.Json;
using System.Security.Claims;

namespace PRJ_FINAL_MP09_MP03.Controllers
{
    public class MusicController : Controller
    {
        private readonly TodoContext _context;
        private readonly IHttpClientFactory _httpClientFactory;


        public MusicController(TodoContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        private List<string?> GetValidApiKey() //  Obtiene una clave API válida
        {
            List<string?> apiKeys = new List<string?>();
            var apiKey = _context.ApiKeys
                .Where(a => a.EsValida && a.Funcion == "Trending" && !string.IsNullOrEmpty(a.ApiKeyValue))
                .AsEnumerable() // pasa el resto del LINQ al lado cliente
                .OrderBy(a => Guid.NewGuid()) // ahora sí puedes usar Guid.NewGuid()
                .ToList();


            return apiKeys;
        }

        private async Task<string?> HacerPeticionConApiKey(Func<string, Task<HttpResponseMessage>> generarPeticion)
        {
            var clavesDisponibles = _context.ApiKeys
                .Where(a => a.EsValida && !string.IsNullOrEmpty(a.ApiKeyValue))
                .AsEnumerable()
                .OrderBy(a => Guid.NewGuid())
                .ToList();

            foreach (var clave in clavesDisponibles)
            {
                var response = await generarPeticion(clave.ApiKeyValue);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                if ((int)response.StatusCode == 404 || (int)response.StatusCode == 429)
                {
                    clave.EsValida = false;
                    _context.Update(clave);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    break;
                }
            }

            return null;
        }


        public async Task<string?> ObtenerTrackId(string nombreCancion)
        {
            var encodedName = HttpUtility.UrlEncode(nombreCancion);
            var client = _httpClientFactory.CreateClient();

            // Lista de API Keys disponibles
            var apiKeys = new List<string>
            {
                "48da421e5cmshede3a32b145be6cp11c965jsn8ada1c95370a",
                "ca15f4ff05msh47e7259b07555e0p1cc0abjsnf5cb9726aaa1",
                "ef818b1aecmsh79827fc8797dd58p1dd2eajsn8f080457a53a",
                "68487643c5mshe3db2baf52846c4p115085jsn4eb5204b61b9",
                "dd6fe17c30msh67fbaec763cfb74p11e836jsn92e674d92f50",
                "478218197amshd30b0d9d273cc0ap18b41ejsnff2ef884be54",
                "d09e880361msh8e4af3eca60f00bp123ceejsn2b2c95b0e10e",
                "aaf7330c73mshf9c8c253d9d98c4p168bc3jsn71b3c8e5b784",
                "00d4898e62msh780845403496f42p18f327jsn788acc6c8e53",
                "6401f7ec16mshbb3b159f0e3395ep1e4da4jsnbb14774b241b",
                "80f9461861mshc0e582b1dce56c7p17a9eajsn2e510d81e363",
                "a2891d6444mshf051420047419b7p134cf0jsn4da4efb44af8",
                "e337bdc97amsh96aa026340eb6c1p1b74ffjsna328a0f57664",
                "e6b3325a7amsh57f1d3a942fa6bcp1f0bb3jsnc16c8f8e55ad",
                "f467220ce9mshb87f4affe0fe75bp1d92d3jsnef8a059801eb",
                "accd065368msh44d9e1f58da3ac3p17cbcbjsn7ed51cc65cf5",
                "48a9c81c4dmsh7435c81d3b1920ep146a5ajsn9569988c8e30",
                "1763bd5976msh4ad294cb1e2f587p127454jsn3472d75f51d4",
                "72f831b276msh3ee9e228b95ea01p1a3806jsn31794d029dd3",
                "3df3367ademshb68e9e12217693ep11a616jsn2df850ca8efd",
                "2975ff274bmshf0826a5b1ceef3cp104080jsn007b63bbf7f0",
                "13315001f0msha48fd7135e48136p1c577cjsn506389eae63d",
                "0dec62608amshb6ee4084c66f28fp1b2e10jsn39c2560b7ffd",
                "13a133ec69msh7fbd77349184af0p1039c4jsne33a1ed775a3",
                "a5e561ef21msh0cf0c061b088bedp1f2985jsn7660201d8e24",
                "58b3fd3b1emshc5f8f60360b4df1p18a51cjsn5ccdc9955863",
                "fa3762a3cbmshbb63e47b4100eb6p1f7e3cjsn7fc71bde127a",
                "812c7ba73amsh19ad22d322219d3p149bccjsn4f3e9e3dd722",
                "7a6fd4e639msh872c557c7765546p102b83jsnd8af2ac6c197",
                "2d736bac49msh78466f9f6b34473p1a1152jsnfb098f83d9ef",
                "4ce159012amshda496facba725d6p1e2e7ajsn7acce8567b61",
                "c2046a1b30msh1e10a33ae7f5c3bp16d764jsncb49744540e6",
                "72b05f3751msh50b29dcb758fca4p1ca526jsnca1c22750ddd",
                "ca57b469famshc3e624c2eb25885p1ee0f9jsn994fda06b4fe",
                "7e490513fbmshc9d116ee3b1d612p17745ajsn1afbf569e260",
                "7726c87e31mshd079c503b7786edp187b97jsn1c851944c6f0",
                "b937143747msh58382a92f2f85bcp1f6c29jsn3d3fc6c2dbda",
                "8a1f3ca396msh2c30f6ac2d3e29dp12db09jsn7ebbed223f56",
                "d7d760bd05mshbfb025add276a23p152593jsnc1f0d01f1327"
            };

            var random = new Random();
            var randomApiKey = apiKeys[random.Next(apiKeys.Count)];

            // Hacer la petición con la API Key aleatoria
            var json = await HacerPeticionConApiKey(_ =>
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://spotify-scraper.p.rapidapi.com/v1/track/search?name={encodedName}"),
                };
                request.Headers.Add("x-rapidapi-key", randomApiKey);
                request.Headers.Add("x-rapidapi-host", "spotify-scraper.p.rapidapi.com");

                return client.SendAsync(request);
            });

            if (string.IsNullOrEmpty(json))
                return null;

            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resultado = JsonSerializer.Deserialize<RootScraper>(json, opts);

            return resultado?.id;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTrackId(string nombreCancion)
        {
            if (string.IsNullOrWhiteSpace(nombreCancion))
                return RedirectToAction("Dashboard");

            var trackId = await ObtenerTrackId(nombreCancion);

            if (trackId == null)
            {
                TempData["Error"] = $"No se encontró el ID para la canción '{nombreCancion}'.";
                return RedirectToAction("Dashboard");
            }

            TempData["TrackId"] = trackId;
            return RedirectToAction("Dashboard", new { nombreCancion });
        }


        [HttpGet]
        public async Task<IActionResult> BuscarTrackIdLyrics(string nombreCancion)
        {
            if (string.IsNullOrWhiteSpace(nombreCancion))
                return RedirectToAction("Lyrics");

            var trackId = await ObtenerTrackId(nombreCancion);

            if (trackId == null)
            {
                TempData["Error"] = $"No se encontró el ID para la canción '{nombreCancion}'.";
                return RedirectToAction("Lyrics");
            }

            TempData["TrackId"] = trackId;
            return RedirectToAction("Lyrics", new { nombreCancion });
        }



        [HttpPost]
        public IActionResult GuardarPlaylist([FromBody] Playlist datos)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            Console.WriteLine("Datos recibidos:");
            Console.WriteLine($"Nombre Canción: {datos.NombreCancion}");
            Console.WriteLine($"Artista: {datos.Artista}");
            Console.WriteLine($"Descripción: {datos.Descripcion}");
            Console.WriteLine($"URL Música: {datos.UrlMusica}");
            Console.WriteLine($"URL Descarga: {datos.UrlDescarga}");
            Console.WriteLine($"URL Imagen: {datos.UrlImg}");
            Console.WriteLine($"ID Track: {datos.IdTrack}");

            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Usuario no autenticado." });
            }

            if (string.IsNullOrWhiteSpace(datos.Descripcion))
            {
                datos.Descripcion = "";  // O un texto por defecto, por ejemplo "Sin descripción"
            }

            datos.UserId = userId.Value;
            datos.DateCreated = DateTime.Now;

            try
            {
                _context.Playlists.Add(datos);
                _context.SaveChanges();

                return Ok(new { success = true, message = "Track guardado en Playlist." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error al guardar en la base de datos: {ex.Message}", detalle = ex.InnerException?.Message });
            }
        }




        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient();
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };


            var SongsIds = new List<string>
            {
                "811",
                "207",
                "9103956",
                "9152241"
            };

            var random = new Random();

            var randomIndex = random.Next(SongsIds.Count);
            var randomSongId = SongsIds[randomIndex];
            // Obtener recomendaciones  
            var recommendations = new List<Recommendation>();
            try
            {
                var apiKeyGenius = GetGeniusApiKey();
                var recJson = await HacerPeticionConApiKey(apiKey =>
                {
                    Console.WriteLine($"Usando API Key: {apiKey}");
                    var request = new HttpRequestMessage
                    {

                        Method = HttpMethod.Get,
                        RequestUri = new Uri("https://genius-song-lyrics1.p.rapidapi.com/song/recommendations/?id=" + randomSongId),
                        Headers =
                        {
                            { "x-rapidapi-key", apiKeyGenius },
                            { "x-rapidapi-host", "genius-song-lyrics1.p.rapidapi.com" }
                        }
                    };
                    return client.SendAsync(request);
                });


                if (recJson != null)
                {
                    var recRoot = JsonSerializer.Deserialize<Root>(recJson, options);
                    recommendations = recRoot?.song_recommendations?.recommendations ?? new List<Recommendation>();
                }
            }
            catch { }

            ViewBag.Username = username;

            return View(new MusicViewModel
            {
                Recommendations = recommendations
            });
        }
        public string GetGeniusApiKey()
        {
           var apiKey = new List<string>
            {
                "f1775ba3acmshc4255e3f0ac61c8p10daaajsn14b8532df961",
                "1a0556d5b6mshf21144e664b96e3p118a27jsnb9e85f6e39ad",
                "e9d2fc2944mshb679d56b563b68bp135573jsn162d0e71160a",
                "e6b090354dmsh3776f6ee9e236c9p1fe861jsn7768b2e28fc8",
                "253b32a34amsh7ca4a967b877983p1826f1jsnf4333c84239d",
                "ed284722f7msh0c8e05f959454a4p19c336jsnfdf595b053df",
                "29b285b8f7mshd6a0ec198c37247p1245cajsnd84917ad2619",
                "77e6ad39b8msh4772d84cf77d9d7p147f4ajsna2a7c04c7a00",
                "fbf3d57ee7mshcceda0a9d9f4a17p1fd33bjsn35d22b04a612",
                "be40f932a4mshae5a277fe604d6ap1f4909jsn7d09170cad05",
                "d143785ac4msh1270cf6fbd0c380p1e72a9jsn3ebd14713625",
                "fd2239c396msh7e2bca773c54705p1a52dfjsn0f9b72f2b3ea",
                "508a39c6bcmshf21fb878b10f795p11d05fjsncb79932eef6e",
                "963fd5d570mshe85a0fbd272bc36p1d5918jsn8193d7f164d8",
                "37a723e09fmshb62d4b6fafe8b66p123627jsn803704317a17",
                "eeb798c60cmsha283ffdc4a5b1d8p19c987jsn5ee14bcae4ac",
                "1adb642fe3msh7916e5af2328261p10b6a1jsn71095e805359",
                "31844968cemsh0633c03e05771b4p1c3b43jsne6ad23c5690b",
                "16df48b991msh1179a3797475ae1p1c5a0ajsnebe301a8996e",
                "0507ad16eamsh50e5df016446442p17de8djsn6543762188e7",
                "59681c8f7dmsh69d9e5cee2a84ccp14710bjsn5d0a7353e746",
                "f626628beemshc6f5bb796ac0352p10bc51jsncbc69049b6a0",
                "c404ee5a80msh151f3af68fb4478p1e0d8cjsn4c42a8a9f4ed",
                "b8ac268209msh8ca780c2688a6f7p1c646fjsn65fc9a5d6c99",
                "ca7b7d6df6msh257be952f0485f6p1ed37ejsnbc50eff7b41c",
                "210325ebdfmsh51cdf2ce5b545fep1fa28cjsn65780e1d4d3e",
                "88aa39b68emshcd9a6f358e68090p12340ejsn18a5b1e387ea",
                "008594cd9bmsh08486414e0a9413p10f1edjsn9a45f55ca9f1",
                "408eb6dc88mshddd7b2b76c542a0p10901ejsn9e5bc9411b76",
                "c25199fedcmshd8347a58c1d09b6p13e356jsn73f826e737f5",
                "2ccc2d135cmsh333a85b2299fc6cp17964cjsn18423ad237a1",
                "5c8c5619aamsh183f1a69e7e3fa9p100b02jsn11f502dc9f50",
                "2fcb06153amsh002557b195a948bp116ab8jsn4ca97dafb2a2",
                "948a666c48mshd71a9fef848e27fp1774bdjsn50abff0b89bd",
                "0920d39a7amshf664be97bd86404p1329ddjsn0f9b3e961055",
                "4bfe06c964msh3be38bac5bdcbcfp1e4b3cjsn1d354cec6b2e",
                "fac36bb05emsh61ab8e8f510a609p16f3ccjsn699a022c6115",
                "633581b4d5msha4a53f5f297e8fcp175235jsn6043d4b946e6",
                "691e7a06b1msh2144f95ff4995dep1ba513jsnf1b8904e189a",
                "bb7126e3fcmshb9df1b56f5cdbe6p13a870jsnca91a26e389b",
                "5cce9ea618msh7ed85a67addab44p1bc2c3jsn626fb4a275c8",
                "3e6f553e82msh7dbcd3f46d9dbffp1110f9jsn9e81ae24ef4a",
                "5955fdcb96msh3b8d2362c86267ep1e24fajsn15286abc29a2",
                "dc97d7db7dmsh1a64c5cff94614ap12d8f3jsn8b6a8db6236d",
                "fcfdb63589msh8d54ea42435fd56p119501jsn1af63ff4ffdf",
                "ced8d96c34msh1a437851b6f821fp1813d4jsn30f0e13688a4",
                "5062e5b0b3msh04159ed525ace61p1f27a7jsn4efaf0c2f030",
                "c89b0b7669msh88c17db60413668p18dfeajsn0e0d663a0609"
            };

            var random = new Random();
            return apiKey[random.Next(apiKey.Count)];
        }
        [HttpGet]
        public async Task<IActionResult> Trending(string timePeriod = "week")
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            var client = _httpClientFactory.CreateClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var trending = new List<ChartItem>();
            var trendingArtists = new List<ChartItemArtis>();
            var trendingAlbums = new List<ChartItemAlbum>();

            try 
            {
                var apiKeyGenius = GetGeniusApiKey();
                var jsonSongs = await HacerPeticionConApiKey(apiKey =>
                {
                    var request = new HttpRequestMessage
                    {
                        
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"https://genius-song-lyrics1.p.rapidapi.com/chart/songs/?time_period={timePeriod}&chart_genre=all&per_page=10&page=1"),
                        Headers = {
                            { "x-rapidapi-key", apiKeyGenius },
                            { "x-rapidapi-host", "genius-song-lyrics1.p.rapidapi.com" }
                        }
                    };
                    return client.SendAsync(request);
                });
                if (jsonSongs != null)
                {
                    var result = JsonSerializer.Deserialize<RootTrending>(jsonSongs, options);
                    trending = result?.chart_items ?? new List<ChartItem>();
                }

                var jsonArtists = await HacerPeticionConApiKey(apiKey =>
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"https://genius-song-lyrics1.p.rapidapi.com/chart/artists/?time_period={timePeriod}&per_page=13&page=1"),
                        Headers = {
                            { "x-rapidapi-key", apiKeyGenius },
                            { "x-rapidapi-host", "genius-song-lyrics1.p.rapidapi.com" }
                        }
                    };
                    return client.SendAsync(request);
                });
                if (jsonArtists != null)
                {
                    var result = JsonSerializer.Deserialize<RootArtist>(jsonArtists, options);
                    trendingArtists = result?.chart_items ?? new List<ChartItemArtis>();
                }

                var jsonAlbums = await HacerPeticionConApiKey(apiKey =>
                {
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"https://genius-song-lyrics1.p.rapidapi.com/chart/albums/?time_period={timePeriod}&per_page=10&page=1"),
                        Headers = {
                            { "x-rapidapi-key", apiKeyGenius },
                            { "x-rapidapi-host", "genius-song-lyrics1.p.rapidapi.com" }
                        }
                    };
                    return client.SendAsync(request);
                });
                if (jsonAlbums != null)
                {
                    var result = JsonSerializer.Deserialize<RootAlbum>(jsonAlbums, options);
                    trendingAlbums = result?.chart_items ?? new List<ChartItemAlbum>();
                }
            }
            catch { }

            ViewBag.Username = username;
            return View(new MusicViewModel
            {
                Trending = trending,
                TrendingArtists = trendingArtists,
                TrendingAlbums = trendingAlbums
            });
        }



        [HttpGet]
        public IActionResult Recomendaciones()
        {
            return View();
        }

        public IActionResult Playlist()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Usuario no autenticado, redirigir a login o mostrar error
                return RedirectToAction("Login");
            }

            // Traer sólo los playlists del usuario con ese Id
            var playlists = _context.Playlists
                                    .Where(p => p.UserId == userId.Value)
                                    .ToList();

            return View(playlists);
        }



        [HttpGet]
        public IActionResult Lyrics()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public IActionResult TopTracksArtista()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public string GetSpotifyApiKey()
        {
            var apiKey = new List<string>
            {
                "3985f53dc8msh9c15680f95d2864p1b408ajsnc61d1713b36a",
                "86a7bf4e34mshaf5f880211c2826p15185djsnd747916cda85",
                "d5839bbb1emshe750ab49434039cp1ceeddjsn691b5ec6ee90",
                "6c78710d80mshaab1639b4a2f43bp17a136jsn7506fbaaa118",
                "da5cb8f3admsh95ce57d749cec13p1967c6jsn85ba5ece5eba",
                "f6e6825361msheb1cfd8c3ac6b9cp19fe4ajsn300fd9c474ff",
                "0f084d9bfbmshfe369dc74c8e150p145b04jsn0c7b3b89574a",
                "0a8d08f0eemsh2b231cd38fc4804p119d8fjsn482b439163d1",
                "178fe5bafdmsh4f85734d804947bp17546bjsna676959250ae",
                "172e9bba04msh4e8835e94e71c83p17c541jsnae1dea977ac6",
                "d9141f2b07msh62d6719333fa2f6p163487jsn6e027dbc1436",
                "a648e802f7mshfab664717d5a822p124fb3jsn953b8750acee",
                "8fa7739482msh9a806517ef26c2ep1790d6jsnc0cf125f2ee9",
                "4c245b7f76msh3abf65f10935a86p1bb712jsn6cbce75f9b8a",
                "3a5c4273demsh13db258a9838e8dp13dc5fjsne59b4ab152aa",
                "386c5911fdmsh3538e647a8ca6c5p1d4041jsn0f4db79dbae5",
                "2265c4857emsh66cab1212c65ca5p1f6eeejsn24bc722c142c",
                "cd971d59dbmsh4aebfd1426fd7e4p10c405jsndee9ed130e16",
                "9b49a31b74msh27330860396329bp173f08jsn42c685e3bfe4",
                "f6832d3e7dmsh552f916c64d7be2p1d4e35jsn8986f4fc9242",
                "4a7dbcfbd0mshee5571db02742f2p1e2e27jsn9df0290cd663",
                "23819bd9famshb3b845a5bd90b3fp14ac61jsn3908c31c182c",
                "776be17c33msh3509756a9fb0c7cp1ac74cjsn9befdaa2f1e2",
                "e7c569125amshc79b1169a431e80p1530e3jsn915be63c3afa",
                "b708211c0dmsh54485bbbdfc99c9p1fa74ejsn1c730d45d8c6",
                "3b6165888cmsh5ef9d241fc3b30ep1a6ef8jsn8a0c0a71adac",
                "380413a58bmsh9dfd36b85af74e3p190776jsn3a6151d3425b",
                "4e6ad826aamsh3fe7f2ebf920f4cp15ace9jsn7edd16a0009e",
                "80e3fcb8b3msh5faaaa4501303e0p15bf6cjsn82bb0d20bc4a",
                "fe30ed2a86msh580427eb1643843p138486jsn8449b075e52e",
                "0509c9014fmsh998e84d2df3e3cdp150c9ejsn67f0ca3fa95f",
                "6ebb8cf621msh5963ccaa79ce4d6p1ed0bfjsne49a08752d7b",
                "9435a3b059mshbb9f993efc44a0ap14e9b1jsnf1e115f46ade",
                "a53a0826c0msh6ee0e614738a767p1df215jsn34c6a4f325a0",
                "90a3f20f7fmshbdefaac6480f498p1422ffjsn35772f7a8bc2",
                "2ecd6aa386msh3cc8a3977034efbp112dbfjsn6763bb291e79",
                "79e29fb911mshef0ff5492592e5dp18ba17jsnb2b47d3c0ec7",
                "c8afeec66dmshc46f77d388840c0p1c3742jsnd2ab3a9f69ef",
                "b772056fe4msh7baa4c5118a400cp162cc0jsn5f231edda761",
                "1de95a09e4mshc29cbd9f900af08p1e1ccejsn96981c2adb32",
                "95861a1f65mshcaa98cc7cd70719p153a54jsn0c41702940b9",
                "bad9b6beb3mshe1c5510a0db1250p155f63jsnd0e3f8ae19a5",
                "cba629d11dmsh7319bcdc0af54f6p1c431ajsn4fa353e998a9",
                "86348578cdmsh02f3f297e205ff3p14ae70jsn31c3c3dc421d",
                "a6adacfa49msh7a066f7e91aef59p1a7556jsn9ead181b8d34",
                "d41dab6610msh1d18999974bc920p1f82a6jsn7293e2062e07",
                "1d9e13826dmsh828871cd95f73d7p13a5a7jsnbc8ea252b537",
                "f1dd78fa7dmshcc55560e47dcf51p19b21djsndb76cf5d7ca1",
                "fa800e5859msh66b67e0832f2025p128296jsndfa6749ee392",
                "47cb1df0f2msh9fc3c4a4fce747bp1bcb19jsne73b233d38ca",
                "eb60ee14f7mshab43243db9a15c9p1908cajsn58c961f0e54c",
                "07090faf6amsh4a48572412b7065p10e284jsn9d5a270c6c05",
                "9fbf6ee9a6mshdea4a9ea315681dp1b3fffjsnbaf6481e0970",
                "df36816d13msh10e65ff4dc86d87p12b1f3jsne406d7897194",
                "e21ddbaf95msh15cd17271c71c42p15183djsnae72967f2d06",
                "42a7f3f148msh694d01ba2038ab4p199778jsnf33aa9229ff9",
                "d11a283d82mshc4617f3b0132c20p10b291jsnb200f0e4991f",
                "53b24b08c8mshf61178d79471fa7p1400f4jsn6fecd54140a5",
                "ab0e40b482msh18871d35122464fp1edaedjsn206af4cfd05e",
                "94d426ad4amsh14c8f8432fa7671p146158jsne07000bd0510",
                "ec55ad43f4msha9cfcd80ea94580p149b9fjsn892bcfeef149"
            };

            var random = new Random();
            return apiKey[random.Next(apiKey.Count)];
        }
        [HttpPost]
        public async Task<IActionResult> TopTracksArtista(string nombreArtista)
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(nombreArtista))
            {
                TempData["Error"] = "Debes introducir el nombre de un artista.";
                return View();
            }

            var client = _httpClientFactory.CreateClient();

            // API Keys disponibles
            var spotifyDataKeys = new List<string>
            {
                "c2790e14a8msh6dcd708484e07fdp127ddejsn0d093079485c",
                "9afd7e374amsh00a9dfff402b214p10749djsn93d3676d4b5e",
                "ad0fbde44fmshd54e28ad35d243ap15a66djsn48194130d1a6",
                "40de99330fmshb0f43bc4a60568dp1e2a13jsn0d04e07e9e57",
                "f50cea3032msh4935bfd9af5d90ap10772ajsn71ad1a132980"
            };

            var spotifyDownloaderKeys = new List<string>
            {
                "f8ae66a71cmsh49ce049de8abc19p1d3bfbjsn14952a7179cd",
                "f85b2031a8msh46e01e2150cd46cp186027jsne553139a06ed",
                "fa19469f67msh3d6b4ca2cfee502p1e1f44jsn02da718c5a45",
                "32528e58camsh619cff676f16cdfp16445ejsn4e8640400af3",
                "1027cc1a88msh5bf573078f0992bp1a1587jsnf578b096ec38"
            };

            
            var spotifyDataKey = GetSpotifyApiKey();
            var spotifyDownloaderKey = GetSpotifyApiKey();

            try
            {
                // 1. Buscar artista y obtener su ID
                var searchUri = $"https://spotify-data.p.rapidapi.com/search/?q={Uri.EscapeDataString(nombreArtista)}&type=artists&offset=0&limit=10&numberOfTopResults=5";
                var artistRequest = new HttpRequestMessage(HttpMethod.Get, searchUri);
                artistRequest.Headers.Add("x-rapidapi-key", spotifyDataKey);
                artistRequest.Headers.Add("x-rapidapi-host", "spotify-data.p.rapidapi.com");

                var artistResponse = await client.SendAsync(artistRequest);
                artistResponse.EnsureSuccessStatusCode();
                var artistJson = await artistResponse.Content.ReadAsStringAsync();
                var artistData = JsonSerializer.Deserialize<RootArtistInfo>(artistJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var artistUri = artistData?.artists?.items?.FirstOrDefault()?.data?.uri;
                if (string.IsNullOrEmpty(artistUri))
                {
                    TempData["Error"] = "Artista no encontrado.";
                    return View();
                }

                var artistId = artistUri.Split(':').Last();

                // 2. Obtener top tracks
                var topTracksUri = $"https://spotify-downloader9.p.rapidapi.com/artistTopTracks?id={artistId}&country=US";
                var tracksRequest = new HttpRequestMessage(HttpMethod.Get, topTracksUri);
                tracksRequest.Headers.Add("x-rapidapi-key", spotifyDownloaderKey);
                tracksRequest.Headers.Add("x-rapidapi-host", "spotify-downloader9.p.rapidapi.com");

                var tracksResponse = await client.SendAsync(tracksRequest);
                tracksResponse.EnsureSuccessStatusCode();
                var tracksJson = await tracksResponse.Content.ReadAsStringAsync();
                var topTracks = JsonSerializer.Deserialize<PRJ_FINAL_MP09_MP03.Models.TopTracks.Root>(tracksJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(topTracks.data.tracks);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Ocurrió un error: {ex.Message}";
                return View();
            }
        }

    



        [HttpGet]
        public async Task<IActionResult> ArtistDetails(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var ApiKey = GetGeniusApiKey();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://genius-song-lyrics1.p.rapidapi.com/artist/details/?id={id}"),
                Headers =
                {
                    { "x-rapidapi-key", ApiKey },
                    { "x-rapidapi-host", "genius-song-lyrics1.p.rapidapi.com" },
                },
            };

            try
            {
                using var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var root = JsonSerializer.Deserialize<ArtistDetaille.Root>(json, options);

                return View("ArtistDetail", root.artist); // Crea una vista `ArtistDetail.cshtml`
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> AlbumDetails(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var ApiKey = GetGeniusApiKey();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://genius-song-lyrics1.p.rapidapi.com/album/details/?id={id}"),
                Headers =
                {
                    { "x-rapidapi-key", ApiKey },
                    { "x-rapidapi-host", "genius-song-lyrics1.p.rapidapi.com" },
                },
            };

            try
            {
                using var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var root = JsonSerializer.Deserialize<AlbumsDetaille.Root>(json, options);

                return View("AlbumDetail", root.album); // Asegúrate de crear `AlbumDetail.cshtml`
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }


        [HttpGet]
        public async Task<IActionResult> SongDetails(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var ApiKey = GetGeniusApiKey();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://genius-song-lyrics1.p.rapidapi.com/song/details/?id={id}"),
                Headers =
                {
                    { "x-rapidapi-key", ApiKey },
                    { "x-rapidapi-host", "genius-song-lyrics1.p.rapidapi.com" },
                },
            };

            try
            {
                using var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var root = JsonSerializer.Deserialize<SongDetaille.Root>(json, options);

                return View("SongDetail", root.song); // Asegúrate de crear `AlbumDetail.cshtml`
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}");
            }
        }


     [HttpGet]
public async Task<IActionResult> GetTrackAudioUrl(string idTrack)
{
    if (string.IsNullOrEmpty(idTrack))
        return BadRequest(new { url = "", error = "idTrack vacío" });

    // Aquí tu lógica para obtener la URL válida del MP3
    // Por ejemplo, igual que haces en el dashboard:
    var client = _httpClientFactory.CreateClient();
    var encodedTrackUrl = Uri.EscapeDataString($"https://open.spotify.com/track/{idTrack}");
    var downloaderApiUrl = $"https://spotify-downloader9.p.rapidapi.com/downloadSong?songId={encodedTrackUrl}";
    var apiKey = GetSpotifyApiKey();
    var request = new HttpRequestMessage(HttpMethod.Get, downloaderApiUrl);
    request.Headers.Add("x-rapidapi-key", apiKey); // Usa tu API Key válida
    request.Headers.Add("x-rapidapi-host", "spotify-downloader9.p.rapidapi.com");

    try
    {
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            return NotFound(new { url = "", error = "No se pudo obtener el MP3" });

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.TryGetProperty("data", out var data) && data.TryGetProperty("downloadLink", out var urlProp))
        {
            var url = urlProp.GetString();
            return Json(new { url });
        }
        else
        {
            return NotFound(new { url = "", error = "No se encontró el enlace de descarga" });
        }
    }
    catch
    {
        return StatusCode(500, new { url = "", error = "Error interno" });
    }
}







    }
}
