import {Stop} from "@/app/lib/objects";

const browserLanguage = (): string => {
    const supported = ['en', 'de'];
    const language = navigator.language.split("-")[0];
    return supported.includes(language) ? language : 'en';
}

const writeName = (stop: Stop): string => {
    if (!stop.nameParts) return stop.name;
    if (stop.nameParts.length === 0) return stop.name;

    return stop.nameParts.map(part => part.value).join('').trim();
}

const calculateDuration = (startDate: Date, endDate: Date): number => {
    if (!startDate || !endDate) return 60;
    const diffInMs = Math.abs(endDate.getTime() - startDate.getTime());
    return Math.round(diffInMs / 60_000);
}

export { browserLanguage, writeName, calculateDuration };
