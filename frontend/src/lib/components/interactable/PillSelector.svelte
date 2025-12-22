<script lang="ts" generics="T">
    interface Props {
        items: PillItem<T>[];
        selected: T;
        onselect: (id: T) => void;
    }

    let { items, selected = $bindable(), onselect }: Props = $props();

    let hoveredIndex: number = $state(-1);
    
    let pillDimensions = $state({ width: 0, left: 0 });
    let buttonRefs: (HTMLButtonElement | null)[] = $state([]);

    $effect(() => {
        const index = items.findIndex(item => item.id === selected);

        const activeNode = buttonRefs[index];
        if (!activeNode) return;
        
        pillDimensions = {
            width: activeNode.offsetWidth,
            left: activeNode.offsetLeft
        };
    });
</script>

<script module>
    export interface PillItem<T> {
        id: T;
        label: string;
    }
</script>

<div class="pill-wrapper w-full overflow-x-auto">
    <div
        class="pill-container bg-muted/80 relative flex  w-max min-w-full items-center gap-x-2 rounded-md p-1"
        style="--pill-width: {pillDimensions.width}px; --pill-left: {pillDimensions.left}px;"    
    >
        {#each items as item, index (item.id)}
            <button
                bind:this={buttonRefs[index]}
                onclick={() => {
                    selected = item.id;
                    onselect(item.id);
                }}
                onmouseenter={() => (hoveredIndex = index)}
                onmouseleave={() => (hoveredIndex = -1)}
                class="pill-button relative z-10 flex items-center justify-center whitespace-nowrap px-4 py-1 text-sm transition-all duration-300 cursor-pointer outline-none"
                class:selected={item.id === selected}
                class:hovered={index === hoveredIndex && item.id !== selected}
            >
                {item.label}
            </button>
        {/each}
    </div>
</div>

<style>
    .pill-wrapper {
        -ms-overflow-style: none;  /* IE and Edge */
        scrollbar-width: none;  /* Firefox */
    }

    .pill-wrapper::-webkit-scrollbar {
        display: none;  /* Chrome, Safari, Opera*/
    }

    .pill-container::before {
        content: "";
        position: absolute;
        /* matches the padding of the container to align vertically */
        top: 4px;
        bottom: 4px;
        left: 0;
        /* Dynamic width and position based on the selected element */
        width: var(--pill-width);
        background-color: color-mix(in srgb, var(--color-accent) 80%, transparent);
        border-radius: 0.375rem;
        transition: transform 0.4s cubic-bezier(0.4, 0, 0.2, 1), width 0.4s cubic-bezier(0.4, 0, 0.2, 1);
        transform: translateX(var(--pill-left));
        pointer-events: none;
    }

    .pill-button.selected {
        font-weight: 700;
        color: var(--color-background);
    }

    .pill-button.hovered {
        background-color: color-mix(in srgb, var(--color-accent) 15%, transparent);
        font-weight: 600;
        border-radius: 0.375rem;
    }

    .pill-button:focus-visible {
        outline: 2px solid var(--color-accent);
        outline-offset: 2px;
    }
</style>