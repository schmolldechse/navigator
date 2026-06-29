import type { LayoutServerLoad } from "./$types";

export type NavigationItem = {
	href: string;
	pageName: string;
	icon?: string;
};

export const load: LayoutServerLoad = async () => ({
	pages: [
		{ href: "/statistics", pageName: "Statistics", icon: "ChartLine" },
		{ href: "/timetable", pageName: "Timetable", icon: "Clock_4" }
	]
});
