using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecipeSnap_BE.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RecipeSnap_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ScanController : ControllerBase
    {

        private readonly OcrService _ocrService;
        private readonly UnitParserService _unitParserService;
        public ScanController(OcrService ocrService, UnitParserService unitParserService)
        {
            _ocrService = ocrService;
            _unitParserService = unitParserService;
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadScannedImage([FromForm] IFormFile file, [FromQuery] bool isMetric)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            //Call the OCR service
            var ocrResult = await _ocrService.ExtractTextAsync(file);
            if (ocrResult.IsErroredOnProcessing) 
                return StatusCode(500, ocrResult.ErrorMessage ?? "OCR failed");
            if (ocrResult.ParsedResults == null || !ocrResult.ParsedResults.Any())
                return StatusCode(500, "OCR returned no results.");

            //Merging the result to one readable string (if multiple pages exists)
            var rawText = string.Join("\n", ocrResult.ParsedResults.Select(r => r.ParsedText));

            //Unit parsing
            var parsed = _unitParserService.Parse(rawText, isMetric);

            return Ok(parsed);
        }
    }
}
