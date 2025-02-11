import type { Component } from "svelte";

export type ProductType = {
	key: string;
	name: string;
	component?: Component;
	values: string[];
};
