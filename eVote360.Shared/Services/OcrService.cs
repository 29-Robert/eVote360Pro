using System;
using System.IO;
using System.Threading.Tasks;
using Tesseract;
using eVote360.Core.Application.Interfaces.Services;

namespace eVote360.Infrastructure.Shared.Services
{
    public class OcrService : IOcrService
    {
        public async Task<string> ExtraerTextoDeImagenAsync(Stream imagenCedula)
        {
            if (imagenCedula == null || imagenCedula.Length == 0)
                return string.Empty;

            // Guardar temporalmente la imagen cargada para que Tesseract la pueda leer
            var rutaTemporal = Path.GetTempFileName();
            using (var stream = new FileStream(rutaTemporal, FileMode.Create))
            {
                await imagenCedula.CopyToAsync(stream);
            }

            try
            {
                // Uso de AppDomain para garantizar compatibilidad total en capas de infraestructura
                string rutaTessdata = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");

                using var engine = new TesseractEngine(rutaTessdata, "spa", EngineMode.Default);
                using var img = Pix.LoadFromFile(rutaTemporal);
                using var page = engine.Process(img);

                string textoExtraido = page.GetText();
                return textoExtraido;
            }
            catch (Exception)
            {
                throw new Exception("No fue posible leer correctamente el número de documento en la imagen cargada."); //[cite: 1]
            }
            finally
            {
                // Limpieza del archivo temporal
                if (File.Exists(rutaTemporal))
                    File.Delete(rutaTemporal);
            }
        }
    }
}