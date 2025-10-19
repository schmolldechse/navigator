<script lang="ts">
	import { type NavigationItem } from "$lib/models/navigation";

    let { items }: { items: NavigationItem[] } = $props();
    let isOpen: boolean = $state(true);
</script>

<nav class="">

</nav>