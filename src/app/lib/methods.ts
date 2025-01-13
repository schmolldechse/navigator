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

export { browserLanguage, writeName };
