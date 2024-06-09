using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            SELECT DISTINCT g.Nombre AS NombreGenealogia, g.Imagenes AS ImagenesG, p.Nombre, p.Apellido, p.FechaNacimiento, p.Imagenes AS ImagenPersona 
            FROM Genealogia g 
            INNER JOIN Persona p ON g.PersonaId = p.PersonaId;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            using (SqlCommand command = new SqlCommand(queryDatosPersona, connection))
            {
                SqlDataReader reader = command.ExecuteReader();

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

                while (reader.Read())
                {
                    string nombrePersona = reader["Nombre"].ToString();
                    string apellido = reader["Apellido"].ToString();
                    DateTime fechaNacimiento = Convert.ToDateTime(reader["FechaNacimiento"]);
                    string rutaImagenPersona = reader["ImagenPersona"].ToString();
                    string rutaImagenG = reader["ImagenesG"].ToString();
                    string nombreGenealogia = reader["NombreGenealogia"].ToString();

                    // Añadir imagen de la persona
                    if (!string.IsNullOrEmpty(rutaImagenPersona) && File.Exists(rutaImagenPersona))
                    {
                        byte[] bytesImagenPersona = File.ReadAllBytes(rutaImagenPersona);
                        string base64ImagenPersona = Convert.ToBase64String(bytesImagenPersona);

                        contenidoHtml += $@"
                        <div>
                            <img src=""data:image/png;base64,{base64ImagenPersona}"" alt=""Imagen de {nombrePersona} {apellido}"">
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

                    // Añadir imagen de genealogía
                    if (!string.IsNullOrEmpty(rutaImagenG) && File.Exists(rutaImagenG))
                    {
                        byte[] bytesImagenG = File.ReadAllBytes(rutaImagenG);
                        string base64ImagenG = Convert.ToBase64String(bytesImagenG);

                        contenidoHtml += $@"
                        <div>
                            <img src=""data:image/png;base64,{base64ImagenG}"" alt=""Imagen de genealogía {nombreGenealogia}"">
                            <div>
                                <b><p>Genealogía: {nombreGenealogia}</p></b>
                            </div>
                        </div>";
                    }
                }

                contenidoHtml += @"
                    </div>
                    <script src=""script.js""></script>
                </body>
                </html>";

                Directory.CreateDirectory(directorio);
                File.WriteAllText(pathPaginaPrincipal, contenidoHtml);


                reader.Close();
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

            string queryAnimes = "SELECT NombreAnime, EnlaceTrailer FROM animes;";
            using (SqlCommand command = new SqlCommand(queryAnimes, connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string nombreAnime = reader["NombreAnime"].ToString();
                    string enlaceTrailer = reader["EnlaceTrailer"].ToString();

                    // Verificar si el enlace es de YouTube
                    if (!string.IsNullOrEmpty(enlaceTrailer) && enlaceTrailer.Contains("youtube.com"))
                    {
                        string embedLink = enlaceTrailer.Replace("watch?v=", "embed/");
                        contenidoHtmlAnimes += $@"
                <div class='video-container'>
                    <b><p>{nombreAnime}</p></b>
                    <iframe src=""{embedLink}"" frameborder=""0"" allowfullscreen></iframe>
                </div>";
                    }
                }

                reader.Close();
            }

            contenidoHtmlAnimes += @"
        </div>
        <script src=""script.js""></script>
    </body>
    </html>";

            File.WriteAllText(pathAnimes, contenidoHtmlAnimes);

            Console.WriteLine("Página de animes generada y abierta en el navegador.");
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = pathAnimes,
                UseShellExecute = true
            });
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

        // Establece la conexión con la base de datos y ejecuta la consulta
        string queryYoutubers = "SELECT youtuber, enlace, rutaImagen FROM Youtubers";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            using (SqlCommand command = new SqlCommand(queryYoutubers, connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Recorre los resultados de la consulta y agrega cada youtuber, su enlace y su imagen al contenido HTML
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
            }
        }

        contenidoHtmlYoutubers += @"
    </div>
    <script src=""script.js""></script>
</body>
</html>";



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

        // Escribir el contenido en el archivo HTML
        File.WriteAllText(pathFormularios, contenidoHtmlContactos);

        // Abrir el archivo HTML en el navegador web predeterminado


        // Escribir el contenido en los archivos HTML correspondientes
        File.WriteAllText(pathPasatiempos, contenidoHtmlPasatiempos);
        File.WriteAllText(pathYoutubers, contenidoHtmlYoutubers);
        File.WriteAllText(pathFormularios, contenidoHtmlContactos);

        Console.WriteLine("Páginas adicionales generadas.");
    }
}