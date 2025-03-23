import Papa from "papaparse";
import * as fs from "node:fs";
import path from "node:path";
import type { LineColor } from "../models/connection.ts";

const parse = async (path: string): Promise<any[]> => {
	const text = fs.readFileSync(path, "utf-8");
	const parsed = Papa.parse(text, {
		header: false,
		skipEmptyLines: true
	});
	return parsed.data;
};

const normalize = (value: string): string => value.toLowerCase().replace("/[\s-]/g", "");

const fromLineName = async (inputLine: string, inputOperator?: string): Promise<LineColor[]> => {
	const data = await parse(path.join(__dirname, "..", "..", "assets", "line-colors.csv"));
	const normalizedInput = normalize(inputLine);

	return data
		.filter((line) => {
			const lineName = normalize(line[1] || "");

			/**
			 * line[2] 		hafasLineId
			 * line[3] 		hafasOperatorCode
			 */
			if (inputOperator) return (lineName === normalizedInput || line[3] === inputLine) && line[2] === inputOperator;
			return normalize(lineName).includes(normalizedInput);
		})
		.map(
			(line) =>
				({
					lineName: line[1],
					hafasLineId: line[3],
					hafasOperatorCode: line[2],
					backgroundColor: line[4],
					textColor: line[5],
					borderColor: line[6],
					shape: line[7]
				}) as LineColor
		);
};

export { fromLineName };
