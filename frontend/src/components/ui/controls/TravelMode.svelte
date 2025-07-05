<script lang="ts">
	let {
		type = $bindable("departure"),
		class: classNames = ""
	}: {
		type: "departure" | "arrival";
		class?: string;
	} = $props();
</script>

<div class={["flex flex-row items-center justify-end gap-x-2", classNames]}>
	<button
		type="button"
		class="cursor-pointer font-bold"
		class:underline={type === "departure"}
		class:underline-offset-2={type === "departure"}
		class:decoration-2={type === "departure"}
		class:text-accent={type === "departure"}
		class:text-text={type !== "departure"}
		onclick={() => (type = "departure")}
	>
		Departure
	</button>
	<span class="nd-fg-white text-xl">|</span>
	<button
		type="button"
		class="cursor-pointer font-bold"
		class:underline={type === "arrival"}
		class:underline-offset-2={type === "arrival"}
		class:decoration-2={type === "arrival"}
		class:text-accent={type === "arrival"}
		class:text-text={type !== "arrival"}
		onclick={() => (type = "arrival")}
	>
		Arrival
	</button>
</div>
