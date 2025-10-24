import type { NavigationItem } from "$lib/components/Header.svelte";
import type { LayoutServerLoad } from "./$types";

export const load: LayoutServerLoad = async ({}) => {
	const pages: NavigationItem[] = [
		{ href: "/maps", pageName: "Station Map", icon: "Map" },
		{ href: "/statistics", pageName: "Statistics", icon: "ChartLine" },
		{ href: "/timetable", pageName: "Timetable", icon: "Clock_4" }
	];

	return { pages };
};
