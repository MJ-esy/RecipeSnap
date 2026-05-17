// This class provides methods for converting between different units of measurement.
//weight: gram, kilogram, pound, uns/ounce
//volume: liter, milliliter, fluid ounce (fl oz), cup, pint
//temperature: celsius, fahrenheit

using RecipeSnap_BE.Models;
using System.Text.RegularExpressions; //For Regex formating

//Packages for conversions
using UnitsNet;
using UnitsNet.Units;

namespace RecipeSnap_BE.Services
{
    public class UnitParserService
    {
        //Handles patterns, mixed/simple fractions and/or multiword units (i.e. "200g", "350°F", "fl oz", "1 1/2 cups")
        //falls back to single-word units
        private static readonly Regex _pattern = new(
            @"(\d+\s+\d+/\d+|\d+/\d+|\d+\.?\d*)\s*" +
            @"(fl\.?\s*oz|imperial\s*pints?|us\s*pints?|" +
            @"cups?|tbsp|tsp|oz|lbs?|kg|g\b|ml|l\b|°[cfCF]|[cfCF](?=\b))",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        //Fraction parser to ensure extracted value can be read
        private static bool TryParseNumber(string input, out double value)
        {
            value = 0;

            input = input.Trim();

            // Mixed fraction: "1 1/2"
            if (input.Contains(' ') && input.Contains('/'))
            {
                //Split on space
                var parts = input.Split(' ');

                if (parts.Length == 2 &&
                    double.TryParse(parts[0], out double whole))
                {
                    //split on fraction '/'
                    var fractionParts = parts[1].Split('/');

                    if (fractionParts.Length == 2 &&
                        double.TryParse(fractionParts[0], out double numerator) &&
                        double.TryParse(fractionParts[1], out double denominator) &&
                        denominator != 0)
                    {
                        value = whole + (numerator / denominator);
                        return true;
                    }
                }
            }

            // Simple fraction: "1/2"
            if (input.Contains('/'))
            {
                //split on fraction '/'
                var fractionParts = input.Split('/');

                if (fractionParts.Length == 2 &&
                    double.TryParse(fractionParts[0], out double numerator) &&
                    double.TryParse(fractionParts[1], out double denominator) &&
                    denominator != 0) //prevents zero crash
                {
                    value = numerator / denominator;
                    return true;
                }
            }

            // Normal decimal/integer
            return double.TryParse(
                input,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, //accounts ',' as decimal
                out value);
        }

        //Parsing OCR-rawText to respective conversion
        public ParseResult Parse(string rawText, bool isMetric)
        { //parameter: Ocr text, metric/imperial
          //Lists to return to user (Model: ParseResult)
          //ParseResult: ConversionResult + Unrecognised
          // ConversionResult: Original + Converted
            var conversions = new List<ConversionResult>();
            var unrecognised = new List<String>();

            //loop through regex pattern
            foreach (Match match in _pattern.Matches(rawText))
            {
                //Extract Value
                if (!TryParseNumber(match.Groups[1].Value, out double value))
                {
                    unrecognised.Add(match.Value);
                    continue;
                }
                //Extract Unit
                var unit = match.Groups[2].Value.Trim().ToLower(); //trims + toLowercase for filtering
                var original = match.Value; //preserve original raw text

                //CONVERSION ATTEMPT (Call TryConvert() Method)
                var result = TryConvert(unit, value, original, isMetric);

                //if succeeded add to conversion, else to unrecognised
                if (result != null)
                    conversions.Add(result);
                else
                    unrecognised.Add(match.Value);
            }

            return new ParseResult
            {
                Conversions = conversions,
                Unrecognised = unrecognised
            };
        }

        //Mapping string unit to correct converter (weight/volume/temperature)
        //returns null if unit not supported, add it to List<Unrecognised>
        private static ConversionResult? TryConvert(string unit, double value, string original, bool isMetric)
        {
            //Calls Convert["UnitType"] methods
            return unit switch
            {
                //Weight
                "g" or "gram" or "grams" => ConvertWeight(value, original, MassUnit.Gram, isMetric),
                "kg" or "kilo" or "kilos" or "kilogram" or "kilograms" => ConvertWeight(value, original, MassUnit.Kilogram, isMetric),
                "oz" or "ounce" or "ounces" => ConvertWeight(value, original, MassUnit.Ounce, isMetric),
                "lb" or "lbs" or "pound" or "pounds" => ConvertWeight(value, original, MassUnit.Pound, isMetric),

                //Volume
                "ml" or "milliliter" or "millilitre" or "milliliters" or "millilitres" => ConvertVolume(value, original, VolumeUnit.Milliliter, isMetric),
                "l" or "liter" or "litre" or "liters" or "litres" => ConvertVolume(value, original, VolumeUnit.Liter, isMetric),
                "fl oz" or "fluid ounce" or "fluid ounces" => ConvertVolume(value, original, VolumeUnit.UsOunce, isMetric),
                "cup" or "cups" => ConvertVolume(value, original, VolumeUnit.UsCustomaryCup, isMetric),
                "pint" or "pints" or "pt" => ConvertVolume(value, original, VolumeUnit.UsPint, isMetric),
                "imperial pint" or "imperial pints" => ConvertVolume(value, original, VolumeUnit.ImperialPint, isMetric),


                // Temperature
                "°c" or "celsius" => ConvertTemp(value, original, TemperatureUnit.DegreeCelsius, isMetric),
                "°f" or "fahrenheit" => ConvertTemp(value, original, TemperatureUnit.DegreeFahrenheit, isMetric),

                // Default: Not a supported unit -- goes to unrecognised[]
                _ => null
            };
        }

        //Converting WEIGHTs
        private static ConversionResult ConvertWeight(double value, string original, MassUnit fromUnit, bool isMetric)
        {
            var mass = Mass.From(value, fromUnit);

            bool isMetricUnit = fromUnit == MassUnit.Gram || fromUnit == MassUnit.Kilogram;
            bool isImperialUnit = fromUnit == MassUnit.Ounce || fromUnit == MassUnit.Pound;

            //Check input to the Unit types requested, if same type then no conversion
            if (isMetric && isMetricUnit)
                return new ConversionResult { Original = original, Converted = $"{original} (no change)" };
            if (!isMetric && isImperialUnit)
                return new ConversionResult { Original = original, Converted = $"{original} (no change)" };

            return new ConversionResult
            {
                Original = original,
                Converted = isMetric ? $"{mass.Grams:0.##} g" : $"{mass.Ounces:0.##} oz"
            };
        }

        //Converting VOLUME
        private static ConversionResult ConvertVolume(double value, string original, VolumeUnit fromUnit, bool isMetric)
        {
            var vol = Volume.From(value, fromUnit);

            bool isMetricUnit = fromUnit == VolumeUnit.Milliliter || fromUnit == VolumeUnit.Liter;
            bool isImperialUnit = fromUnit == VolumeUnit.UsCustomaryCup ||
                                          fromUnit == VolumeUnit.UsOunce ||
                                          fromUnit == VolumeUnit.UsPint ||
                                          fromUnit == VolumeUnit.ImperialPint;

            if (isMetric && isMetricUnit)
                return new ConversionResult { Original = original, Converted = $"{original} (no change)" };

            if (!isMetric && isImperialUnit)
                return new ConversionResult { Original = original, Converted = $"{original} (no change)" };

            return new ConversionResult
            {
                Original = original,
                Converted = isMetric ? $"{vol.Milliliters:0.##} ml" : $"{vol.UsOunces:0.##} fl oz"
            };
        }

        //Converting Temperature
        private static ConversionResult ConvertTemp(double value, string original, TemperatureUnit fromUnit, bool isMetric)
        {
            var temp = Temperature.From(value, fromUnit);

            if (isMetric && fromUnit == TemperatureUnit.DegreeCelsius)
                return new ConversionResult { Original = original, Converted = $"{original} (no change)" };

            if (!isMetric && fromUnit == TemperatureUnit.DegreeFahrenheit)
                return new ConversionResult { Original = original, Converted = $"{original} (no change)" };

            return new ConversionResult
            {
                Original = original,
                Converted = isMetric
                   ? $"{temp.DegreesCelsius:0.##} °C"
                     : $"{temp.DegreesFahrenheit:0.##} °F"
            };
        }
    }
}
