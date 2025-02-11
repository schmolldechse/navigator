import type {SvelteComponent} from "svelte";

export type ProductType = {
	key: string;
	name: string;
	component?: typeof SvelteComponent;
	values: string[];
}