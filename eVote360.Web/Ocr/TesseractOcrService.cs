using eVote360.Core.Application.Interfaces.Services;
using Org.BouncyCastle.Asn1.Pkcs;
using System;
using System.IO;
using System.Threading.Tasks;
using Tesseract;

namespace eVote360.WebApp.Ocr 
{
    public class TesseractOcrService : IOcrService
    {
        private readonly string _tessdataPath;

        public TesseractOcrService()
        {
            string baseDir = AppContext.BaseDirectory;
            _tessdataPath = Path.Combine(baseDir, "tessdata");
        }

        public async Task<string> ExtraerTextoDeImagenAsync(Stream imagenStream)
        {
            string archivoIdioma = Path.Combine(_tessdataPath, "spa.traineddata");
            if (!File.Exists(archivoIdioma))
            {
                throw new FileNotFoundException($"Falta el archivo de idioma en la ruta: {archivoIdioma}. Verifica que sus propiedades estén en 'Copiar si es posterior'.");
            }

            if (imagenStream.CanSeek)
            {
                imagenStream.Position = 0;
            }

            return await Task.Run(() =>
            {
                try
                {
                    using var memoryStream = new MemoryStream();
                    imagenStream.CopyTo(memoryStream);
                    byte[] bytesImagen = memoryStream.ToArray();

                    if (bytesImagen.Length == 0) return string.Empty;

                    using var engine = new TesseractEngine(_tessdataPath, "spa", EngineMode.Default);
                    using var img = Pix.LoadFromMemory(bytesImagen);
                    using var page = engine.Process(img);

                    string resultado = page.GetText();
                    return resultado ?? string.Empty;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Tesseract Exception]: {ex.Message}");
                    return string.Empty;
                }
            });
        }
    }
}