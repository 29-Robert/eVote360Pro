using eVote360.Core.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace eVote360.Core.Application.Services
{
    public class OcrService : IOcrService
    {
        public async Task<string> ExtraerTextoDeImagenAsync(Stream imagenStream)
        {
            if (imagenStream == null || imagenStream.Length == 0)
            {
                throw new ArgumentException("El archivo de imagen no es válido.");
            }

            var rutaTemporal = Path.GetTempFileName();
            using (var stream = new FileStream(rutaTemporal, FileMode.Create))
            {
                await imagenStream.CopyToAsync(stream);
            }

            try
            {
                string rutaTessData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

                using var engine = new TesseractEngine(rutaTessData, "spa", EngineMode.Default);
                using var img = Pix.LoadFromFile(rutaTemporal);
                using var page = engine.Process(img);

                // 4. Extraer el texto detectado
                string textoExtraido = page.GetText();
                return textoExtraido ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error interno en el motor de reconocimiento OCR: {ex.Message}", ex);
            }
            finally
            {
                if (File.Exists(rutaTemporal))
                {
                    File.Delete(rutaTemporal);
                }
            }
        }
    }
}
