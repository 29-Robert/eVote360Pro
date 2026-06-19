using Microsoft.AspNetCore.Http;
using System.IO;

namespace eVote360.WebApp.Helpers
{
    public static class UploadHelper
    {
        public static string UploadImage(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0) return null;

            // Generar un nombre único para evitar colisiones
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderPath);

            // Crear carpeta si no existe
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            string filePath = Path.Combine(path, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"/{folderPath}/{fileName}";
        }
    }
}