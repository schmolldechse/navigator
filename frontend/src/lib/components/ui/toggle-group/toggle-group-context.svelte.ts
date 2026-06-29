import { getContext, setContext } from "svelte";

const TOGGLE_GROUP_CONTEXT_KEY = Symbol("toggle-group");

type ToggleItem = { [key: string]: any };
type ToggleGroupMode = "single" | "multiple";
type ToggleGroupValue<T extends ToggleItem> = T | T[] | undefined;

interface ToggleGroupProps<T extends ToggleItem> {
	get selected(): ToggleGroupValue<T>;
	set selected(item: ToggleGroupValue<T>);
	get mode(): ToggleGroupMode;
	get keyFn(): (item: T) => unknown;
	get allowEmpty(): boolean;
}

class ToggleGroupContext<T extends ToggleItem> {
	items: T[] = $state([]);

	constructor(private props: ToggleGroupProps<T>) {}

	get selected(): ToggleGroupValue<T> {
		return this.props.selected;
	}

	set selected(option: ToggleGroupValue<T>) {
		this.props.selected = option;
	}

	get selectedItems(): T[] {
		if (!this.selected) return [];
		return Array.isArray(this.selected) ? this.selected : [this.selected];
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

	toggle = (option: T): boolean => {
		const key = this.props.keyFn(option);
		const isAlreadySelected = this.selectedItems.some((selectedOption: T) => this.props.keyFn(selectedOption) === key);

		if (this.props.mode === "single") {
			if (isAlreadySelected && !this.props.allowEmpty) return false;

			this.selected = isAlreadySelected ? undefined : option;
		} else {
			this.selected = isAlreadySelected
				? this.selectedItems.filter((selectedOption: T) => this.props.keyFn(selectedOption) !== key)
				: [...this.selectedItems, option];
		}

		return true;
	};

	isSelected = (option: T): boolean => {
		const key = this.props.keyFn(option);
		return this.selectedItems.some((selectedOption: T) => this.props.keyFn(selectedOption) === key);
	};
}

const setToggleGroupContext = <T extends ToggleItem>(context: ToggleGroupContext<T>) =>
	setContext(TOGGLE_GROUP_CONTEXT_KEY, context);

const getToggleGroupContext = <T extends ToggleItem>() => getContext<ToggleGroupContext<T>>(TOGGLE_GROUP_CONTEXT_KEY);

export {
	type ToggleItem,
	type ToggleGroupMode,
	type ToggleGroupValue,
	ToggleGroupContext,
	setToggleGroupContext,
	getToggleGroupContext
};
