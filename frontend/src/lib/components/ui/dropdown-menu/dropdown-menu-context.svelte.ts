import { getContext, setContext } from "svelte";

const DROPDOWN_MENU_CONTEXT_KEY = Symbol("dropdown-menu");

type DropdownMenuState = "open" | "closed";
type DropdownMenuAlign = "start" | "center" | "end";

type DropdownMenuItemRecord = {
	id: string;
	element: HTMLElement;
	disabled: () => boolean;
	textValue: () => string;
	select: (event: Event) => void;
};

interface DropdownMenuContextProps {
	get open(): boolean;
	set open(value: boolean);
	get disabled(): boolean;
	get loop(): boolean;
	get closeOnEscape(): boolean;
	get closeOnInteractOutside(): boolean;
	get triggerId(): string;
	get contentId(): string;
	get rootElement(): HTMLElement | undefined;
	get triggerElement(): HTMLElement | undefined;
	set triggerElement(value: HTMLElement | undefined);
	get contentElement(): HTMLElement | undefined;
	set contentElement(value: HTMLElement | undefined);
	onopenchange?: (open: boolean) => void;
}

class DropdownMenuContext {
	items: DropdownMenuItemRecord[] = $state([]);
	activeItemId: string | undefined = $state(undefined);
	triggerWidth = $state(0);

	constructor(private props: DropdownMenuContextProps) {}

	get open(): boolean {
		return this.props.open;
	}

	get disabled(): boolean {
		return this.props.disabled;
	}

	get loop(): boolean {
		return this.props.loop;
	}

	get closeOnEscape(): boolean {
		return this.props.closeOnEscape;
	}

	get closeOnInteractOutside(): boolean {
		return this.props.closeOnInteractOutside;
	}

	get state(): DropdownMenuState {
		return this.open ? "open" : "closed";
	}

	get triggerId(): string {
		return this.props.triggerId;
	}

	get contentId(): string {
		return this.props.contentId;
	}

	get rootElement(): HTMLElement | undefined {
		return this.props.rootElement;
	}

	get triggerElement(): HTMLElement | undefined {
		return this.props.triggerElement;
	}

	set triggerElement(value: HTMLElement | undefined) {
		this.props.triggerElement = value;
		this.updateTriggerWidth();
	}

	get contentElement(): HTMLElement | undefined {
		return this.props.contentElement;
	}

	set contentElement(value: HTMLElement | undefined) {
		this.props.contentElement = value;
	}

	get enabledItems(): DropdownMenuItemRecord[] {
		return this.items.filter((item) => item && !item.disabled());
	}

	get activeItem(): DropdownMenuItemRecord | undefined {
		return this.items.find((item) => item && item.id === this.activeItemId && !item.disabled());
	}

	updateTriggerWidth = () => {
		this.triggerWidth = this.triggerElement?.getBoundingClientRect().width ?? 0;
	};

	setOpen = (open: boolean) => {
		if (this.disabled && open) return;
		if (this.open === open) return;

		this.props.open = open;
		this.props.onopenchange?.(open);

		if (open) {
			this.updateTriggerWidth();
			queueMicrotask(this.updateTriggerWidth);
			return;
		}

		this.activeItemId = undefined;
	};

	openMenu = () => this.setOpen(true);
	closeMenu = () => this.setOpen(false);
	toggleMenu = () => this.setOpen(!this.open);

	closeAndFocusTrigger = () => {
		this.closeMenu();
		queueMicrotask(() => this.triggerElement?.focus());
	};

	registerItem = (item: DropdownMenuItemRecord) => {
		const index = this.items.findIndex((registeredItem) => registeredItem?.id === item.id);
		if (index >= 0) {
			this.items[index] = item;
			return;
		}

		this.items.push(item);
		if (!this.activeItemId && !item.disabled()) this.activeItemId = item.id;
	};

	unregisterItem = (id: string) => {
		this.items = this.items.filter((item) => item && item.id !== id);
		if (this.activeItemId !== id) return;

		this.activeItemId = this.enabledItems[0]?.id;
	};

	setActiveItem = (id: string | undefined) => {
		if (id && !this.items.some((item) => item && item.id === id && !item.disabled())) return;
		this.activeItemId = id;
	};

	focusItem = (item: DropdownMenuItemRecord | undefined) => {
		if (!item || item.disabled()) return;

		this.activeItemId = item.id;
		item.element.focus();
	};

	focusFirstItem = () => this.focusItem(this.enabledItems[0]);
	focusLastItem = () => this.focusItem(this.enabledItems.at(-1));

	moveFocus = (delta: number) => {
		const items = this.enabledItems;
		if (items.length === 0) return;

		const currentIndex = Math.max(
			0,
			items.findIndex((item) => item.id === this.activeItemId)
		);
		const nextIndex = currentIndex + delta;

		if (nextIndex < 0) {
			this.focusItem(this.loop ? items.at(-1) : items[0]);
			return;
		}

		if (nextIndex >= items.length) {
			this.focusItem(this.loop ? items[0] : items.at(-1));
			return;
		}

		this.focusItem(items[nextIndex]);
	};

	selectActiveItem = (event: Event) => {
		this.activeItem?.select(event);
	};

	handleTriggerKeydown = (event: KeyboardEvent) => {
		if (this.disabled) return;

		if (event.key === "ArrowDown") {
			event.preventDefault();
			this.openMenu();
			queueMicrotask(this.focusFirstItem);
			return;
		}

		if (event.key === "ArrowUp") {
			event.preventDefault();
			this.openMenu();
			queueMicrotask(this.focusLastItem);
			return;
		}

		if (event.key === "Enter" && this.open && this.activeItem) {
			event.preventDefault();
			this.selectActiveItem(event);
			return;
		}

		if (event.key === "Escape" && this.open && this.closeOnEscape) {
			event.preventDefault();
			this.closeMenu();
		}
	};

	handleContentKeydown = (event: KeyboardEvent) => {
		if (this.disabled) return;

		if (event.key === "Escape" && this.closeOnEscape) {
			event.preventDefault();
			this.closeAndFocusTrigger();
			return;
		}

		if (event.key === "ArrowDown") {
			event.preventDefault();
			this.moveFocus(1);
			return;
		}

		if (event.key === "ArrowUp") {
			event.preventDefault();
			this.moveFocus(-1);
			return;
		}

		if (event.key === "Home") {
			event.preventDefault();
			this.focusFirstItem();
			return;
		}

		if (event.key === "End") {
			event.preventDefault();
			this.focusLastItem();
			return;
		}

		if ((event.key === "Enter" || event.key === " ") && this.activeItem) {
			event.preventDefault();
			this.selectActiveItem(event);
		}
	};

	handleDocumentPointerDown = (event: PointerEvent) => {
		if (!this.open || !this.closeOnInteractOutside) return;
		if (!(event.target instanceof Node)) return;
		if (this.rootElement?.contains(event.target)) return;

		this.closeMenu();
	};
}

const setDropdownMenuContext = (context: DropdownMenuContext) => setContext(DROPDOWN_MENU_CONTEXT_KEY, context);

const getDropdownMenuContext = () => {
	const context = getContext<DropdownMenuContext | undefined>(DROPDOWN_MENU_CONTEXT_KEY);
	if (!context) throw new Error("Dropdown menu components must be used inside DropdownMenu.Root.");
	return context;
};

export { DropdownMenuContext, getDropdownMenuContext, setDropdownMenuContext, type DropdownMenuAlign, type DropdownMenuState };
