<script lang="ts">
	let { type = $bindable() } = $props();
</script>

<span class="montserrat-regular text-base md:text-xl flex items-center space-x-4">
  <button
	  onclick={() => type = "timetable"}
	  class="cursor-pointer decoration-2 underline-offset-4"
	  class:underline={type === 'timetable'}
	  class:no-underline={type !== 'timetable'}
  >
    Timetable
  </button>
  <button
	  onclick={() => type = "route_planner"}
	  class="cursor-pointer decoration-2 underline-offset-4"
	  class:underline={type === 'route_planner'}
	  class:no-underline={type !== 'route_planner'}
  >
    Route Planner
  </button>
</span>