<script lang="ts">
    import NationalExpress from "$components/timetable/filter/icons/NationalExpress.svelte";
    import Regional from "$components/timetable/filter/icons/Regional.svelte";
    import Suburban from "$components/timetable/filter/icons/Suburban.svelte";
    import Subway from "$components/timetable/filter/icons/Subway.svelte";
    import Tram from "$components/timetable/filter/icons/Tram.svelte";
    import Bus from "$components/timetable/filter/icons/Bus.svelte";
    import Ferry from "$components/timetable/filter/icons/Ferry.svelte";
	import type {ProductType} from "$src/models/product";

    const types: ProductType[] = [
        {
            key: "*",
            name: "Every connection",
            component: undefined,
            values: ["*"]
        },
        {
            key: "Longdistance",
            name: "Long distance",
            component: NationalExpress,
            values: ['HOCHGESCHWINDIGKEITSZUEGE', 'INTERCITYUNDEUROCITYZUEGE']
        },
        {
            key: "Regional",
            name: "Regional",
            component: Regional,
            values: ['INTERREGIOUNDSCHNELLZUEGE', 'NAHVERKEHRSONSTIGEZUEGE']
        },
        {
            key: "Suburban",
            name: "Suburban",
            component: Suburban,
            values: ['SBAHNEN']
        },
        {
            key: "Bus",
            name: "Busses",
            component: Bus,
            values: ['BUSSE']
        },
        {
            key: "Ferry",
            name: "Ferry",
            component: Ferry,
            values: ['FERRY']
        },
        {
            key: "Subway",
            name: "Subway",
            component: Subway,
            values: ['UBAHN']
        },
        {
            key: "Tram",
            name: "Tram",
            component: Tram,
            values: ['STRASSENBAHN']
        }
    ]
</script>

<div class="flex items-center gap-x-4">
    <!-- Everything -->
    <span>Every connection</span>

    <div class="flex items-center">
        <NationalExpress/>
        <span>Long distance</span>
    </div>

    <div class="flex items-center">
        <Regional/>
        <span>Regional</span>
    </div>

    <div class="flex items-center">
        <Suburban/>
        <span>Suburban</span>
    </div>

    <div class="flex items-center">
        <Subway/>
        <span>Subway</span>
    </div>

    <div class="flex items-center">
        <Tram/>
        <span>Tram</span>
    </div>

    <div class="flex items-center">
        <Ferry/>
        <span>Ferry</span>
    </div>

    <div class="flex items-center">
        <Bus/>
        <span>Bus</span>
    </div>
</div>