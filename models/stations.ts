import {Products} from "./products.ts";

export interface Station {
    name: string;
    locationId: string;
    evaNr: string;
    coordinates?: {
        latitude: number,
        longitude: number
    };
    products: Products[];
}