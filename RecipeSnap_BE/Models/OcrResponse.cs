namespace RecipeSnap_BE.Models
{
    public class OcrResponse
    {
        public List<ParsedResult> ParsedResults { get; set; } = new();
        public int OCRExitCode { get; set; }
        public bool IsErroredOnProcessing { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ParsedResult
    {
        public string ParsedText { get; set; } = string.Empty;
        public int FileParseExitCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
