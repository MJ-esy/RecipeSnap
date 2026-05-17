using RecipeSnap_BE.Models;
using System.Text.Json;

namespace RecipeSnap_BE.Services
{
    public class OcrService
    {

        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        public OcrService(HttpClient client, IConfiguration config)
        {
            _client = client;
            _config = config;
            _client.DefaultRequestHeaders.Add("apikey", _config["OcrSpace:ApiKey"]);
        }

        public async Task<OcrResponse> ExtractTextAsync(IFormFile file)
        {
            // Reformat image/file to be able to read and write
            using var stream = file.OpenReadStream();
            var bytes = new byte[stream.Length];
            await stream.ReadAsync(bytes);
            var base64 = $"data:{file.ContentType};base64," + Convert.ToBase64String(bytes);


            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(base64), "base64Image");
            form.Add(new StringContent("2"), "OCREngine");
            form.Add(new StringContent("true"), "scale");
            form.Add(new StringContent("true"), "detectOrientation");
            form.Add(new StringContent("true"), "isTable");
            form.Add(new StringContent("true"), "isOverlayRequired");

            try
            {
                var response = await _client.PostAsync("https://api.ocr.space/parse/image", form);
                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<OcrResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });

                return result;

            }
            catch (Exception ex)
            {
                throw new Exception($"OCR request failed: {ex.Message}", ex);
            }
        }
    }
}
