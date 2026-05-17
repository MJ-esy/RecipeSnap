import { type ScanResult } from "../types/ScanResult";
const BASE_URL = process.env.NEXT_PUBLIC_API_URL || "https://localhost:7166";
interface RequestBody {
  file: File;
  isMetric: boolean;
}

export async function scanImage({ file, isMetric }: RequestBody): Promise<ScanResult> {
  const formData = new FormData();
  formData.append("file", file);

  const response = await fetch(`${BASE_URL}/api/Scan/upload?isMetric=${isMetric}`, {
    method: "POST",
    body: formData,
  });

  if (!response.ok) throw new Error(`Scan failed: ${response.statusText}`);

  return response.json();
}