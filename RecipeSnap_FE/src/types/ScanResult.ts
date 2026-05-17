interface ConversionResult {
  original: string;
  converted: string;
}

interface ScanResult {
  conversions: ConversionResult[];
  unrecognised: string[];
}

export type { ScanResult, ConversionResult };