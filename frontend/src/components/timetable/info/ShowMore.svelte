<script lang="ts">
	let { onclick }: { onclick: () => void } = $props();
</script>

<button
	class="group ml-2 inline-flex cursor-pointer items-center justify-center rounded-2xl bg-primary-dark px-2 py-1"
	style="vertical-align: middle"
	{onclick}
	aria-label="Show more"
>
	<svg height="3" width="16px" xmlns="http://www.w3.org/2000/svg" class="transition-all duration-300 group-hover:w-[22px]">
		<circle cx="2" cy="1.5" r="1.5" fill="white" />
		<circle cx="8" cy="1.5" r="1.5" fill="white" />
		<circle cx="14" cy="1.5" r="1.5" fill="white" />
		<circle
			cx="20"
			cy="1.5"
			r="1.5"
			fill="white"
			class="origin-left translate-x-[-6px] scale-0 opacity-0 transition-all duration-300 group-hover:translate-x-0 group-hover:scale-100 group-hover:opacity-100"
		/>
	</svg>
</button>
