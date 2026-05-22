import type { PageServerLoad } from "./$types";
import { loadHeatmap } from "@lib/components/statistics/global/heatmap/heatmap.remote";
import {
	createDefaultHeatmapSettings,
	createHeatmapRequest,
	type HeatmapSettings
} from "@lib/components/statistics/global/heatmap/heatmap";
import { loadMetric } from "@lib/remote/metrics.remote";
import {
	createAdministrationRankingRequest,
	createDefaultAdministrationRankingSettings,
	type AdministrationRankingSettings
} from "@lib/components/statistics/global/administration-ranking/administration-ranking";

export const load: PageServerLoad = async ({
	getClientAddress
}): Promise<{
	heatmap: { promise: Promise<Awaited<ReturnType<typeof loadHeatmap>>>; settings: HeatmapSettings };
	administrationRanking: {
		promise: Promise<Awaited<ReturnType<typeof loadMetric>>>;
		settings: AdministrationRankingSettings;
	};
}> => {
	const heatmapSettings: HeatmapSettings = createDefaultHeatmapSettings();
	const administrationRankingSettings: AdministrationRankingSettings = createDefaultAdministrationRankingSettings();

	return {
		heatmap: {
			promise: loadHeatmap({
				request: createHeatmapRequest(heatmapSettings),
				userIp: getClientAddress()
			}),
			settings: heatmapSettings
		},
		administrationRanking: {
			promise: loadMetric({
				request: createAdministrationRankingRequest(administrationRankingSettings),
				userIp: getClientAddress()
			}),
			settings: administrationRankingSettings
		}
	};
};
