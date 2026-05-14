import { getContext, setContext } from "svelte";

const TOGGLE_GROUP_CONTEXT_KEY = Symbol("toggle-group");

type ToggleItem = { [key: string]: any };
type ToggleGroupMode = "single" | "multiple";

interface ToggleGroupProps<T extends ToggleItem> {
	get selected(): T[];
	set selected(item: T[]);
	get mode(): ToggleGroupMode;
	get keyFn(): (item: T) => unknown;
}

class ToggleGroupContext<T extends ToggleItem> {
	items: T[] = $state([]);

	constructor(private props: ToggleGroupProps<T>) {}

	get selected(): T[] {
		return this.props.selected;
	}

	set selected(option: T[]) {
		this.props.selected = option;
	}

	register = (option: T) => {
		const key = this.props.keyFn(option);
		if (this.items.some((availableOption: T) => this.props.keyFn(availableOption) === key)) return;

		this.items.push(option);
	};

	unregister = (option: T) => {
		const key = this.props.keyFn(option);
		this.items = this.items.filter((availableOption: T) => this.props.keyFn(availableOption) !== key);
	};

	toggle = (option: T) => {
		const key = this.props.keyFn(option);
		const isAlreadySelected = this.selected.some((selectedOption: T) => this.props.keyFn(selectedOption) === key);

		if (this.props.mode === "single") this.selected = isAlreadySelected ? [] : [option];
		else
			this.selected = isAlreadySelected
				? this.selected.filter((selectedOption: T) => this.props.keyFn(selectedOption) !== key)
				: [...this.selected, option];
	};

	isSelected = (option: T): boolean => {
		const key = this.props.keyFn(option);
		return this.selected.some((selectedOption: T) => this.props.keyFn(selectedOption) === key);
	};
}

const setToggleGroupContext = <T extends ToggleItem>(context: ToggleGroupContext<T>) =>
	setContext(TOGGLE_GROUP_CONTEXT_KEY, context);

const getToggleGroupContext = <T extends ToggleItem>() => getContext<ToggleGroupContext<T>>(TOGGLE_GROUP_CONTEXT_KEY);

export { type ToggleItem, type ToggleGroupMode, ToggleGroupContext, setToggleGroupContext, getToggleGroupContext };
