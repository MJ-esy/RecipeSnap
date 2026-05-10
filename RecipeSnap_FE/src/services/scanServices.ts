export async function scanImage(imageBase64: string, isMetric: boolean) {
  const response = await fetch("/api/scan", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ image: imageBase64, isMetric }),
  });

  if (!response.ok) throw new Error("Scan failed");

  return response.json();
}