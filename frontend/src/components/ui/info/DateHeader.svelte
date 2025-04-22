<script lang="ts">
    import type {DateTime} from "luxon";

    let { date }: { date: DateTime } = $props();
</script>

<div class="px-8 mb-4 mt-8">
    <span class="block w-full text-lg md:text-xl border-b border-text/50 text-text">{date.toLocaleString({ weekday: "short", day: "numeric", month: "short", year: "numeric" })}</span>
</div>