import type { PageServerLoad } from "./$types";
import { loadHeatmap } from "@lib/components/statistics/heatmap/heatmap.remote";
import { createDefaultHeatmapSettings, type HeatmapSettings } from "@lib/components/statistics/heatmap/heatmap";

export const load: PageServerLoad = async ({
	getClientAddress
}): Promise<{
	heatmap: { promise: Promise<Awaited<ReturnType<typeof loadHeatmap>>>; settings: HeatmapSettings };
}> => {
	const heatmapSettings: HeatmapSettings = createDefaultHeatmapSettings();

	return {
		heatmap: {
			promise: loadHeatmap({
				request: {
					seriesType: heatmapSettings.seriesType,
					scheduleType: heatmapSettings.scheduleType,
					start: heatmapSettings.dates.start.toISO()!,
					end: heatmapSettings.dates.end.toISO()!,
					transportTypes: heatmapSettings.transportTypes
				},
				userIp: getClientAddress()
			}),
			settings: heatmapSettings
		}
	};
};
