<script lang="ts">
    let { type = $bindable() } = $props();
</script>

<span class="montserrat-regular text-base md:text-xl flex items-center space-x-4">
  <button
      onclick={() => type = "timetable"}
      class="cursor-pointer"
      class:border-b-2={type === "timetable"}
      class:border-white={type === "timetable"}
  >
    Timetable
  </button>
  <button
      onclick={() => type = "route_planner"}
      class="cursor-pointer"
      class:border-b-2={type === "route_planner"}
      class:border-white={type === "route_planner"}
  >
    Route Planner
  </button>
</span>