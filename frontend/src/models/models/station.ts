import type {Products} from "./products";
import type {NamePart} from "./namepart";

export interface Station {
    name: string;
    evaNr: string;
    locationId?: string;
    coordinates?: {
        latitude: number,
        longitude: number
    };
    products?: Products[];
}

export interface Stop extends Station {
    cancelled: boolean;
    additional?: boolean;
    separation?: boolean;
    nameParts?: NamePart[];
}