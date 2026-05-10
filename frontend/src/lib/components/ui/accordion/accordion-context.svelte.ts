import { getContext, setContext } from "svelte";

const ACCORDION_ROOT_CONTEXT_KEY = Symbol("accordion-root");
const ACCORDION_ITEM_CONTEXT_KEY = Symbol("accordion-item");

type AccordionType = "single" | "multiple";
type AccordionValue = string | string[] | undefined;

interface AccordionRootContextProps {
	get type(): AccordionType;
	get value(): AccordionValue;
	set value(value: AccordionValue);
	get disabled(): boolean;
}

interface AccordionItemContextProps {
	get value(): string;
	get disabled(): boolean;
	get itemId(): string;
	get triggerId(): string;
	get contentId(): string;
}

class AccordionRootContext {
	constructor(private props: AccordionRootContextProps) {}

	get type(): AccordionType {
		return this.props.type;
	}

	get disabled(): boolean {
		return this.props.disabled;
	}

	get value(): AccordionValue {
		if (this.type === "multiple") return Array.isArray(this.props.value) ? this.props.value : [];
		return typeof this.props.value === "string" ? this.props.value : undefined;
	}

	set value(value: AccordionValue) {
		this.props.value = value;
	}

	isOpen = (value: string): boolean => {
		const currentValue = this.value;
		return Array.isArray(currentValue) ? currentValue.includes(value) : currentValue === value;
	};

	open = (value: string) => {
		if (this.disabled || this.isOpen(value)) return;

		if (this.type === "multiple") {
			const currentValue = Array.isArray(this.value) ? this.value : [];
			this.value = [...currentValue, value];
			return;
		}

		this.value = value;
	};

	toggle = (value: string) => {
		if (this.disabled) return;

		if (this.type === "single") {
			this.value = this.isOpen(value) ? undefined : value;
			return;
		}

		const currentValue = Array.isArray(this.value) ? this.value : [];
		this.value = currentValue.includes(value)
			? currentValue.filter((selectedValue) => selectedValue !== value)
			: [...currentValue, value];
	};
}

class AccordionItemContext {
	constructor(
		private root: AccordionRootContext,
		private props: AccordionItemContextProps
	) {}

	get value(): string {
		return this.props.value;
	}

	get disabled(): boolean {
		return this.root.disabled || this.props.disabled;
	}

	get open(): boolean {
		return this.root.isOpen(this.value);
	}

	get collapsed(): boolean {
		return !this.open;
	}

	get itemId(): string {
		return this.props.itemId;
	}

	get triggerId(): string {
		return this.props.triggerId;
	}

	get contentId(): string {
		return this.props.contentId;
	}

	openItem = () => {
		if (this.disabled) return;
		this.root.open(this.value);
	};

	toggle = () => {
		if (this.disabled) return;
		this.root.toggle(this.value);
	};
}

const setAccordionRootContext = (context: AccordionRootContext) => setContext(ACCORDION_ROOT_CONTEXT_KEY, context);

const getAccordionRootContext = () => getContext<AccordionRootContext | undefined>(ACCORDION_ROOT_CONTEXT_KEY);

const setAccordionItemContext = (context: AccordionItemContext) => setContext(ACCORDION_ITEM_CONTEXT_KEY, context);

const getAccordionItemContext = () => getContext<AccordionItemContext | undefined>(ACCORDION_ITEM_CONTEXT_KEY);

export {
	type AccordionType,
	type AccordionValue,
	AccordionRootContext,
	AccordionItemContext,
	setAccordionRootContext,
	getAccordionRootContext,
	setAccordionItemContext,
	getAccordionItemContext
};
