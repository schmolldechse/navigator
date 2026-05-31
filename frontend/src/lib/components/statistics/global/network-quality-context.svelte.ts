import { createContext } from "svelte";
import type { MetricSeries } from "@lib/api";
import { createNetworkTimeSeriesPromises, type NetworkTimeSeriesPromises } from "./network-time-series/network-time-series";
import type { StatisticsScopeSettings } from "./statistics-scope";

class NetworkQualityContext {
	promises: NetworkTimeSeriesPromises;
	evaNumber?: number;

	constructor(initial: NetworkTimeSeriesPromises, evaNumber?: number) {
		this.promises = $state(initial);
		this.evaNumber = evaNumber;
	}

	update = (scope: StatisticsScopeSettings) => {
		this.promises = createNetworkTimeSeriesPromises(scope, undefined, this.evaNumber);
	};

	isLoading = () =>
		Object.values(this.promises).some((promise: Promise<MetricSeries>) => "loading" in promise && Boolean(promise.loading));
}

const [getNetworkQualityContext, setNetworkQualityContext] = createContext<NetworkQualityContext>();

export { NetworkQualityContext, getNetworkQualityContext, setNetworkQualityContext };
