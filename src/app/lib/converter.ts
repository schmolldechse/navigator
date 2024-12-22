import Papa from "papaparse";
import * as fs from "node:fs";

const path = "./public/line-colors.csv";

const parse = async (path: string): Promise<any[]> => {
    const text = fs.readFileSync(path, 'utf-8');
    const parsed = Papa.parse(text, {
        header: false,
        skipEmptyLines: true
    });
    return parsed.data;
}

const keys = [
    "shortOperatorName",
    "lineName",
    "hafasOperatorCode",
    "hafasLineId",
    "backgroundColor",
    "textColor",
    "borderColor"
]

const normalize = (value: string): string => value.toLowerCase().replace("/[\s-]/g", "");

export const fromHafasLineId = async (input: string): Promise<any | null> => {
    const data = await parse(path);

    const matching = data.find((line) => {
        const lineName = normalize(line[1] || "");
        const hafasLineId = normalize(line[3] || "");
        const normalizedInput = normalize(input);

        return (hafasLineId === normalizedInput || lineName === normalizedInput);
    });
    if (!matching) return null;

    return keys.reduce((obj, key, index) => {
        obj[key] = matching[index] || "";
        return obj;
    }, {} as Record<string, string>);
}
