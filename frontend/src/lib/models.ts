import type { Component } from "svelte";

interface ValidMessage {
	type: string;
	component?: Component;
}

export type { ValidMessage };
