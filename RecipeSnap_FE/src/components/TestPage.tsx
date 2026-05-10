import { useState } from 'react';
import { scanImage } from '../services/scanServices';

export default function TestPage() {
    const [imageBase64, setImageBase64] = useState<string | null>(null);
    const [isMetric, setIsMetric] = useState(true);

    console.log(isMetric);
    const handleUnitType = (e: React.ChangeEvent<HTMLInputElement, HTMLInputElement>) => {
        setIsMetric(e.target.checked);
    };

    const handleImageSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = () => {
            const base64 = (reader.result as string).split(",")[1];
            setImageBase64(base64);
        };
        reader.readAsDataURL(file);
    };

    const handleSubmit = async () => {
        if (!imageBase64) return;
        const data = await scanImage(imageBase64, isMetric);
        console.log(data); // { conversions: [...], unrecognised: [...] }
    };

    return (
        <>
            <label className="swap border-2 p-2 border-primary">
                <input
                    type="checkbox"
                    checked={isMetric}
                    onChange={handleUnitType} />
                <div className="swap-on">Metric</div>
                <div className="swap-off">Imperial</div>
            </label>

            <div>
                <input
                    type="file"
                    accept="image/*"
                    {...{ capture: "environment" }} // cast to avoid TS type error
                    onChange={handleImageSelect}
                />
            </div>

            <button className="btn btn-wide" onClick={handleSubmit}>
                Submit
            </button>
            <span className="loading loading-ring loading-lg"></span>
        </>
    );
}
