<script lang="ts">
	let { checked = false, onchecked }: { checked?: boolean; onchecked?: (checked: boolean) => void } = $props();
</script>

<div class="flex items-center gap-3">
	<button
		role="switch"
		aria-label="switch"
		aria-checked={checked}
		class="switch"
		onclick={() => {
			checked = !checked;
			if (onchecked) onchecked(checked);
		}}
	>
		<span class="switch-track"></span>
		<span class="switch-thumb"></span>
	</button>
</div>

<style>
	.switch {
		position: relative;
		width: 2.75rem;
		height: 1.5rem;
		border: none;
		background: transparent;
		padding: 0;
		cursor: pointer;
	}

	.switch-track {
		position: absolute;
		left: 0;
		top: 0.25rem;
		width: 100%;
		height: 1rem;
		border-radius: 9999px;
		background-color: rgba(255, 255, 255, 0.1);
		transition: background-color 0.2s ease;
	}

	.switch-thumb {
		position: absolute;
		left: 0;
		top: 0;
		width: 1.5rem;
		height: 1.5rem;
		border-radius: 9999px;
		background-color: #fff;
		box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
		transition:
			transform 0.2s ease,
			background-color 0.2s ease;
	}

	.switch[aria-checked="true"] .switch-track {
		background-color: rgba(255, 218, 0, 0.4);
	}

	.switch[aria-checked="true"] .switch-thumb {
		transform: translateX(1.25rem);
		background-color: var(--color-accent);
	}
</style>
