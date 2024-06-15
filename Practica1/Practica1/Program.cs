using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string directorio = "C:\\Users\\isaac\\OneDrive\\CUATRIMESTRE 8\\Tecnologias de internet 1\\Practica1\\Practica1";
        string pathPaginaPrincipal = Path.Combine(directorio, "PaginaPrincipal.html");
        string pathPasatiempos = Path.Combine(directorio, "Pasatiempos.html");
        string pathYoutubers = Path.Combine(directorio, "youtubers.html");
        string pathAnimes = Path.Combine(directorio, "animes.html");
        string pathFormularios = Path.Combine(directorio, "formularios.html");
        string connectionString = "Server=ISAC\\SQLEXPRESS;Database=Arbol genealogico;Integrated Security=True;TrustServerCertificate=True";

        // Generar contenido HTML de la página principal
        string queryDatosPersona = @"
            SELECT g.Nombre AS NombreGenealogia, g.Imagenes AS ImagenesG, g.Parentesco, p.PersonaId, p.Nombre, p.Apellido, p.FechaNacimiento, p.Imagenes AS ImagenPersona 
            FROM Genealogia g 
            INNER JOIN Persona p ON g.PersonaId = p.PersonaId;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(queryDatosPersona, connection))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                string contenidoHtml = @"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Menú de Navegación</title>
                    <link rel=""stylesheet"" href=""main.css"">
                </head>
                <body>
                    <header>
                        <nav>
                            <ul class=""menu"">
                                <li><a href=""PaginaPrincipal.html"" class=""menu-link"" data-page=""sobre-mi"">Sobre mi</a></li>
                                <li><a href=""Pasatiempos.html"" class=""menu-link"" data-page=""pasatiempos"">Pasatiempos</a></li>
                                <li><a href=""youtubers.html"" class=""menu-link"" data-page=""youtubers-favoritos"">Youtubers favoritos</a></li>
                                <li><a href=""animes.html"" class=""menu-link"" data-page=""anime-series-favoritos"">Anime o series favoritos</a></li>
                                <li><a href=""formularios.html"" class=""menu-link"" data-page=""formulario-contactos"">Formulario de contactos</a></li>
                            </ul>
                        </nav>
                    </header>
                    <div class=""center"">";

                HashSet<int> processedPersonIds = new HashSet<int>();

                // Procesar las filas
                foreach (DataRow row in dataTable.Rows)
                {
                    int personaId = row["PersonaId"] != DBNull.Value ? Convert.ToInt32(row["PersonaId"]) : -1;

                    if (personaId != -1 && !processedPersonIds.Contains(personaId))
                    {
                        processedPersonIds.Add(personaId);

                        string nombrePersona = row["Nombre"].ToString();
                        string apellido = row["Apellido"].ToString();
                        DateTime fechaNacimiento = row["FechaNacimiento"] != DBNull.Value ? Convert.ToDateTime(row["FechaNacimiento"]) : DateTime.MinValue;
                        string rutaImagenPersona = row["ImagenPersona"].ToString();

                        // Añadir imagen de la persona asociada si existe
                        if (!string.IsNullOrEmpty(rutaImagenPersona) && File.Exists(rutaImagenPersona))
                        {
                            byte[] bytesImagenPersona = File.ReadAllBytes(rutaImagenPersona);
                            string base64ImagenPersona = Convert.ToBase64String(bytesImagenPersona);

                            contenidoHtml += $@"
                                        <div class=""image-container"">
                                            <img class=""profile-image"" src=""data:image/png;base64,{base64ImagenPersona}"" alt=""Imagen de {nombrePersona} {apellido}"">
                                            <div>
                                                <b><p>{nombrePersona} {apellido}</p></b>
                                                <b><p>Fecha de nacimiento: {fechaNacimiento.ToShortDateString()}</p></b>
                                            </div>
                                        </div>";

                        }
                        else
                        {
                            contenidoHtml += $@"
                            <div>
                                <p>No hay imagen disponible para {nombrePersona} {apellido}</p>
                                <div>
                                    <b><p>{nombrePersona} {apellido}</p></b>
                                </div>
                            </div>";
                        }
                    }
                }

                // Añadir las imágenes de genealogía después de procesar las personas
                foreach (DataRow row in dataTable.Rows)
                {
                    int personaId = row["PersonaId"] != DBNull.Value ? Convert.ToInt32(row["PersonaId"]) : -1;

                    if (personaId != -1 && processedPersonIds.Contains(personaId))
                    {
                        string rutaImagenG = row["ImagenesG"].ToString();
                        string nombreGenealogia = row["NombreGenealogia"].ToString();
                        string parentesco = row["Parentesco"].ToString();

                        // Añadir imagen de la genealogía si existe
                        if (!string.IsNullOrEmpty(rutaImagenG) && File.Exists(rutaImagenG))
                        {
                            byte[] bytesImagenG = File.ReadAllBytes(rutaImagenG);
                            string base64ImagenG = Convert.ToBase64String(bytesImagenG);

                            contenidoHtml += $@"
                            <div>
                                <img src=""data:image/png;base64,{base64ImagenG}"" alt=""Imagen de genealogía {nombreGenealogia}"">
                                <div>
                                    <b><p style=""text-align: center;"">{parentesco}: {nombreGenealogia}</p></b>
                                </div>
                            </div>";
                        }
                    }
                }

                contenidoHtml += @"
                    </div>
                    <script src=""script.js""></script>
                </body>
                </html>";

                File.WriteAllText(pathPaginaPrincipal, contenidoHtml);
            }
        }

        // Generar contenido HTML de la página de animes
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string contenidoHtmlAnimes = @"
    <!DOCTYPE html>
    <html lang=""en"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Anime Favoritos</title>
        <link rel=""stylesheet"" href=""main.css"">
    </head>
    <body>
        <header>
            <nav>
                <ul class=""menu"">
                    <li><a href=""PaginaPrincipal.html"" class=""menu-link"" data-page=""sobre-mi"">Sobre mi</a></li>
                    <li><a href=""Pasatiempos.html"" class=""menu-link"" data-page=""pasatiempos"">Pasatiempos</a></li>
                    <li><a href=""youtubers.html"" class=""menu-link"" data-page=""youtubers-favoritos"">Youtubers favoritos</a></li>
                    <li><a href=""animes.html"" class=""menu-link"" data-page=""anime-series-favoritos"">Anime o series favoritos</a></li>
                    <li><a href=""formularios.html"" class=""menu-link"" data-page=""formulario-contactos"">Formulario de contactos</a></li>
                </ul>
            </nav>
        </header>
        <div class=""center"">";

            string queryAnimes = "SELECT NombreAnime, EnlaceTrailer FROM Animes;";
            using (SqlCommand command = new SqlCommand(queryAnimes, connection))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                foreach (DataRow row in dataTable.Rows)
                {
                    string nombre = row["NombreAnime"].ToString();
                    string enlaceTrailer = row["EnlaceTrailer"].ToString();
                    string videoId = GetYouTubeVideoId(enlaceTrailer); // Extraer el ID del video de YouTube

                    contenidoHtmlAnimes += $@"
                    <div>
                        <h2>{nombre}</h2>
                        <iframe width=""560"" height=""315"" src=""https://www.youtube.com/embed/{videoId}"" frameborder=""0"" allowfullscreen></iframe>
                    </div>";
                }

                contenidoHtmlAnimes += @"
            </div>
            <script src=""script.js""></script>
        </body>
        </html>";

                File.WriteAllText(pathAnimes, contenidoHtmlAnimes);
            }
        }
    

    static string GetYouTubeVideoId(string url)
    {
        var uri = new Uri(url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query["v"];
    }




// Generar el contenido HTML de las otras páginas (Pasatiempos, Youtubers, Formularios)
string contenidoHtmlPasatiempos = @"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Pasatiempos</title>
            <link rel=""stylesheet"" href=""main.css"">
        </head>
        <body>
            <header>
                <nav>
                    <ul class=""menu"">
                        <li><a href=""PaginaPrincipal.html"" class=""menu-link"" data-page=""sobre-mi"">Sobre mi</a></li>
                       
<li><a href=""Pasatiempos.html"" class=""menu-link"" data-page=""pasatiempos"">Pasatiempos</a></li>
                        <li><a href=""youtubers.html"" class=""menu-link"" data-page=""youtubers-favoritos"">Youtubers favoritos</a></li>
                        <li><a href=""animes.html"" class=""menu-link"" data-page=""anime-series-favoritos"">Anime o series favoritos</a></li>
                        <li><a href=""formularios.html"" class=""menu-link"" data-page=""formulario-contactos"">Formulario de contactos</a></li>
                    </ul>
                </nav>
            </header>
            <div class=""center""> Entre mis pasatiempos se encuentran dedicarme a la palabra de Dios, ir a la Iglesia, ver series de television o peliculas en netflix, jugar videojuegos, etc.</div>
            <script src=""script.js""></script>
        </body>
        </html>";

        string contenidoHtmlYoutubers = @"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Youtubers Favoritos</title>
            <link rel=""stylesheet"" href=""main.css"">
        </head>
        <body>
            <header>
                <nav>
                    <ul class=""menu"">
                        <li><a href=""PaginaPrincipal.html"" class=""menu-link"" data-page=""sobre-mi"">Sobre mi</a></li>
                        <li><a href=""Pasatiempos.html"" class=""menu-link"" data-page=""pasatiempos"">Pasatiempos</a></li>
                        <li><a href=""youtubers.html"" class=""menu-link"" data-page=""youtubers-favoritos"">Youtubers favoritos</a></li>
                        <li><a href=""animes.html"" class=""menu-link"" data-page=""anime-series-favoritos"">Anime o series favoritos</a></li>
                        <li><a href=""formularios.html"" class=""menu-link"" data-page=""formulario-contactos"">Formulario de contactos</a></li>
                    </ul>
                </nav>
            </header>
            <div class=""center"">
                Entre mis youtubers favoritos se encuentran:<br><br>";

        // Consulta para obtener los datos de los Youtubers favoritos
        string queryYoutubers = "SELECT youtuber, enlace, rutaImagen FROM Youtubers";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(queryYoutubers, connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Leer los datos y construir el contenido HTML
                while (reader.Read())
                {
                    string youtuber = reader["youtuber"].ToString();
                    string enlace = reader["enlace"].ToString();
                    string rutaImagen = reader["rutaImagen"].ToString();

                    contenidoHtmlYoutubers += $@"
                    <div>
                        <img src=""{rutaImagen}"" alt=""Imagen del canal de YouTube de {youtuber}"">
                        <p><a href=""{enlace}"">{youtuber}</a></p>
                    </div>";
                }

                reader.Close();
            }
        }

        contenidoHtmlYoutubers += @"
            </div>
            <script src=""script.js""></script>
        </body>
        </html>";

        File.WriteAllText(pathYoutubers, contenidoHtmlYoutubers);

        Console.WriteLine("Página de Youtubers favoritos generada y abierta en el navegador.");
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = pathYoutubers,
            UseShellExecute = true
        });

        // Generar contenido HTML de la página de Pasatiempos
        File.WriteAllText(pathPasatiempos, contenidoHtmlPasatiempos);

        Console.WriteLine("Página de Pasatiempos generada.");

        // Generar contenido HTML de la página de Formulario de Contacto
        string contenidoHtmlContactos = @"
        <!DOCTYPE html>
        <html lang=""en"">
        <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>Contacto</title>
            <link rel=""stylesheet"" href=""main.css"">
        </head>
        <body>
            <header>
                <nav>
                    <ul class=""menu"">
                        <li><a href=""PaginaPrincipal.html"" class=""menu-link"" data-page=""sobre-mi"">Sobre mi</a></li>
                        <li><a href=""Pasatiempos.html"" class=""menu-link"" data-page=""pasatiempos"">Pasatiempos</a></li>
                        <li><a href=""youtubers.html"" class=""menu-link"" data-page=""youtubers-favoritos"">Youtubers favoritos</a></li>
                        <li><a href=""animes.html"" class=""menu-link"" data-page=""anime-series-favoritos"">Anime o series favoritos</a></li>
                        <li><a href=""formularios.html"" class=""menu-link"" data-page=""formulario-contactos"">Formulario de contactos</a></li>
                    </ul>
                </nav>
            </header>
            <div class=""center"">
                <h2>Formulario de Contacto</h2>
                <form action=""#"" method=""post"">
                    <label for=""nombre"">Nombre:</label><br>
                    <input type=""text"" id=""nombre"" name=""nombre"" required><br><br>
                    <label for=""email"">Correo Electrónico:</label><br>
                    <input type=""email"" id=""email"" name=""email"" required><br><br>
                    <label for=""mensaje"">Mensaje:</label><br>
                    <textarea id=""mensaje"" name=""mensaje"" rows=""4"" required></textarea><br><br>
                    <input type=""submit"" value=""Enviar"">
                </form>

                <h2>Sígueme en Redes Sociales</h2>
                <div class=""social-media"">
                    <a href=""https://www.facebook.com/tuperfil"" target=""_blank"">
                        <img src=""imagenes/facebook.png"" alt=""Facebook"" style=""width: 50px; height: 50px;"">
                    </a>
                    <a href=""https://www.twitter.com/tuperfil"" target=""_blank"">
                        <img src=""imagenes/twitter.png"" alt=""Twitter"" style=""width: 50px; height: 50px;"">
                    </a>
                    <a href=""https://www.instagram.com/tuperfil"" target=""_blank"">
                        <img src=""imagenes/instagram.png"" alt=""Instagram"" style=""width: 50px; height: 50px;"">
                    </a>
                </div>
            </div>
            <script src=""script.js""></script>
        </body>
        </html>";

        File.WriteAllText(pathFormularios, contenidoHtmlContactos);

        Console.WriteLine("Página de Formulario de Contacto generada y abierta en el navegador.");

        // Mostrar mensaje final de finalización
        Console.WriteLine("Páginas adicionales generadas.");
    }
}
