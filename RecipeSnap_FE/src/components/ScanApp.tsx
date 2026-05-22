import { useState } from 'react';
import { scanImage } from '../services/scanServices';
import type { ScanResult } from '../types/ScanResult';

export default function ScanApp() {
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [isMetric, setIsMetric] = useState(true);
    const [previewUrl, setPreviewUrl] = useState<string | null>(null);
    const [result, setResult] = useState<ScanResult | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    console.log(isMetric);
    const handleUnitType = (e: React.ChangeEvent<HTMLInputElement>) => {
        setIsMetric(e.target.checked);
    };

    const handleImageSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;
        setSelectedFile(file);
        setPreviewUrl(URL.createObjectURL(file));
    };

    const handleSubmit = async () => {
        if (!selectedFile) return;
        setIsLoading(true);
        setError(null);
        try {
            const data = await scanImage({ file: selectedFile, isMetric });
            setResult(data);
            console.log("API URL:", process.env.NEXT_PUBLIC_API_URL);
            console.log(data); // { conversions: [...], unrecognised: [...] }
        } catch (error) {
            console.error('Error scanning image:', error);
            setError(error instanceof Error ? error.message : 'Failed to scan image');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <>
            <section className='flex justify-center items-center flex-col'>
                <div className='mb-3'>
                    Pick the unit type:
                    <label className="swap border-2 p-1 text-center m-1 border-primary">

                        <input
                            type="checkbox"
                            checked={isMetric}
                            onChange={handleUnitType} />
                        <div className="swap-on">Metric</div>
                        <div className="swap-off">Imperial</div>
                    </label>
                </div>


                <div className="items-center gap-2 mb-3 ">
                    <label htmlFor="image-upload" className="btn btn-outline cursor-pointer mx-2">
                        Choose Photo
                    </label>
                    <input
                        id="image-upload"
                        type="file"
                        accept="image/*"
                        onChange={handleImageSelect}
                        className="hidden"
                    />
                    <span className="text-sm text-gray-500 mx-2">
                        {selectedFile ? selectedFile.name : 'No file chosen'}
                    </span>
                </div>

                <button className="btn btn-wide " onClick={handleSubmit}>
                    Submit
                </button>

                <div className='m-3'>
                    {previewUrl && (
                        <img
                            src={previewUrl}
                            alt="Scanned recipe"
                            className="rounded-lg mb-4 max-h-96 object-contain w-full"
                        />
                    )}
                    {isLoading && <span className="loading loading-ring loading-lg"></span>}
                    {error && <p className="text-error">Error: {error}</p>}

                    {result && (
                        <div className="mt-3">
                            <h2 className="text-lg font-bold mb-2">Conversions</h2>
                            <div className="space-y-2 mb-3">
                                {result.conversions.map((c, i) => (
                                    <p key={i}>{c.original} → {c.converted}</p>
                                ))}
                            </div>

                            {result.unrecognised.length > 0 && (
                                <>
                                    <h2 className="text-lg font-bold mt-6 mb-2">Unrecognised</h2>
                                    <div className="space-y-2 mb-3">
                                        {result.unrecognised.map((u, i) => (
                                            <p key={i}>{u}</p>
                                        ))}
                                    </div>
                                </>
                            )}
                        </div>
                    )}
                </div>
            </section>
        </>
    );
}
