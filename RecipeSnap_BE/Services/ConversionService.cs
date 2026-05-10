using UnitsNet;

// This class provides methods for converting between different units of measurement.
//weight: gram, kilogram, pound, uns/ounce
//volume: liter, milliliter, fluid ounce (fl oz), cup, pint
//temperature: celsius, fahrenheit

public class ConversionService
{
    //Temperature conversion: 
    public string ConvertTemperature(double value, string fromUnit, string toUnit)
    {
        string result = $"{value} {toUnit}";
        return result;
    }
}