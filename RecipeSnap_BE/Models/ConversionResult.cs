namespace RecipeSnap_BE.Models
{
    public class ConversionResult
    {
        public string Original { get; set; } = string.Empty;
        public string Converted { get; set; } = string.Empty;
    }

    public class ParseResult
    {
        public List<ConversionResult> Conversions { get; set; } = new();
        public List<string> Unrecognised { get; set; } = new();
    }
}
