import { Products } from "./products.ts";

export interface Station {
	name: string;
	evaNumber: string;
	locationId?: string;
	coordinates?: {
		latitude: number;
		longitude: number;
	};
	products?: Products[];
}

export interface Stop extends Station {
	cancelled: boolean;
	additional?: boolean;
	separation?: boolean;
	nameParts?: NamePart[];
}

export interface NamePart {
	type: string;
	value: string;
}
