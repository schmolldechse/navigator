import { createContext } from "svelte";
import { cloneScopeSettings, type StatisticsScopeSettings } from "./statistics-scope";

class StatisticsScopeContext {
	current: StatisticsScopeSettings;

	constructor(initial: StatisticsScopeSettings) {
		this.current = $state(cloneScopeSettings(initial));
	}

	update = (next: StatisticsScopeSettings) => {
		this.current = cloneScopeSettings(next);
	};
}

const [getStatisticsScopeContext, setStatisticsScopeContext] = createContext<StatisticsScopeContext>();

export { StatisticsScopeContext, getStatisticsScopeContext, setStatisticsScopeContext };
