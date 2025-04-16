<script lang="ts">
	let {
		type = $bindable("departures"),
		class: classNames = ""
	}: {
		type: "departures" | "arrivals";
		class?: string;
	} = $props();
</script>

<div class={["flex flex-row items-center justify-end gap-x-2", classNames]}>
	<button
		type="button"
		class="cursor-pointer font-bold"
		class:underline={type === "departures"}
		class:underline-offset-2={type === "departures"}
		class:decoration-2={type === "departures"}
		class:text-accent={type === "departures"}
		class:text-text={type !== "departures"}
		onclick={() => (type = "departures")}
	>
		Departures
	</button>
	<span class="nd-fg-white text-xl">|</span>
	<button
		type="button"
		class="cursor-pointer font-bold"
		class:underline={type === "arrivals"}
		class:underline-offset-2={type === "arrivals"}
		class:decoration-2={type === "arrivals"}
		class:text-accent={type === "arrivals"}
		class:text-text={type !== "arrivals"}
		onclick={() => (type = "arrivals")}
	>
		Arrivals
	</button>
</div>
