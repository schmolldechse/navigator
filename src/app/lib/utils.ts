import {Stop} from "@/app/lib/objects";

const browserLanguage = (): string => {
    const supported = ['en', 'de'];
    const language = navigator.language.split("-")[0];
    return supported.includes(language) ? language : 'en';
}

const mapStops = (rawData: any): Stop[] => {
    if (!Array.isArray(rawData)) rawData = [rawData];

    return rawData.map((rawStop: any) => {
        const {evaNumber, name, canceled, additional, separation, nameParts} = rawStop;
        return {
            id: evaNumber,
            name: name,
            cancelled: canceled,
            additional: additional || false,
            separation: separation || false,
            nameParts: nameParts ? nameParts.map((part: any) => ({
                type: part.type,
                value: part.value
            })) : [{type: "default", value: name}]
        };
    });
}

export {browserLanguage, mapStops};
