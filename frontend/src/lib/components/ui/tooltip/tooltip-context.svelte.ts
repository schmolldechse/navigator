import { getContext, setContext } from "svelte";

const TOOLTIP_CONTEXT_KEY = Symbol("tooltip-context");

class TooltipContext {
	contentVisible: boolean = $state(false);

	delayTimer: ReturnType<typeof setTimeout> | undefined = undefined;
	delay: number;

	constructor(delay: number = 150) {
		this.delay = delay;
	}

	show = () => {
		if (this.delayTimer) clearTimeout(this.delayTimer);
		this.delayTimer = setTimeout(() => (this.contentVisible = true), this.delay);
	};

	hide = () => {
		if (this.delayTimer) clearTimeout(this.delayTimer);

		this.delayTimer = undefined;
		this.contentVisible = false;
	};
}

const setTooltipContext = (context: TooltipContext) => setContext(TOOLTIP_CONTEXT_KEY, context);

const getTooltipContext = () => getContext<TooltipContext>(TOOLTIP_CONTEXT_KEY);

export { TooltipContext, setTooltipContext, getTooltipContext };
