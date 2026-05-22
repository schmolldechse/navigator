<script lang="ts">
	import type { PageProps } from "./$types";
	import StationMetricMapSection from "@lib/components/statistics/global/StationMetricMapSection.svelte";
	import AdministrationRankingSection from "@lib/components/statistics/global/AdministrationRankingSection.svelte";
	import NetworkQualityTimeSeriesSection from "@lib/components/statistics/global/NetworkQualityTimeSeriesSection.svelte";
	import LineRankingSection from "@lib/components/statistics/global/LineRankingSection.svelte";
	import * as Accordion from "@lib/components/ui/accordion";

	let { data }: PageProps = $props();
	let faqValues: string[] = $state([]);
</script>

<svelte:head>
	<title>Statistics - Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-8 p-4 sm:py-8">
	<StationMetricMapSection promise={data.stationMetricMap.promise} settings={data.stationMetricMap.settings} />

	<NetworkQualityTimeSeriesSection promise={data.networkTimeSeries.promise} settings={data.networkTimeSeries.settings} />

	<AdministrationRankingSection promise={data.administrationRanking.promise} settings={data.administrationRanking.settings} />

	<LineRankingSection promise={data.lineRanking.promise} settings={data.lineRanking.settings} />

	<section class="space-y-4">
		<div class="flex flex-col gap-y-1">
			<h2 class="text-2xl font-medium">Statistics FAQ</h2>
			<p class="text-foreground/60 max-w-3xl text-sm sm:text-base">
				Answers to the most important questions about the map, rankings, time series, and metric interpretation.
			</p>
		</div>

		<Accordion.Root type="multiple" bind:value={faqValues}>
			<Accordion.Item value="map-modes">
				<Accordion.Trigger>Why are some metrics shown as density and others as points?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Station event counts are volume metrics, so a density layer is useful for showing where many events cluster. Quality
						metrics such as cancellation rate, average delay, and punctuality are station values. They are shown as colored
						points so the color represents the metric value instead of a mixture of value and nearby station density.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="polarity">
				<Accordion.Trigger>What does metric polarity mean?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Polarity describes whether a higher value is better, worse, or mostly neutral. Punctuality has positive polarity,
						cancellation rate and delay have negative polarity, and raw event counts are neutral. The statistics page uses this
						metadata for color scales, ranking icons, and descriptions, so a high bad value is not accidentally framed as a top
						performer.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="samples">
				<Accordion.Trigger>Why do tooltips show sample sizes?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Rates and averages need context. A 100 percent cancellation rate from one recorded event means something very
						different than the same rate from hundreds of events. The sample details show the numerator and denominator behind a
						value and flag small samples so comparisons stay cautious.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="station-events-vs-journeys">
				<Accordion.Trigger>What is the difference between station events and journeys?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						The station metric map is built from stop-place events: one planned arrival or departure at one station. Line and
						operator rankings are built from journey-route summaries: one row per journey, line, operator, route, and hour. This
						is why a station can show a cancelled stop event while a line ranking still does not count the whole journey as
						cancelled.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="cancellations">
				<Accordion.Trigger>What counts as a cancellation?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						For line and operator rankings, cancellation rate means fully cancelled journeys based on the journey-level
						<code class="bg-secondary rounded px-1 py-0.5 text-xs">cancelled</code>
						flag. If a planned terminal station is skipped and the route ends earlier, that is not counted as a fully cancelled journey
						there. On the station metric map, cancellation count and rate refer to cancelled station events at the selected arrival
						or departure stop.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="delay-reference">
				<Accordion.Trigger>What does delay refer to?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Station delay is calculated from the recorded delay of the planned station event, so arrivals are measured against
						the planned arrival time and departures against the planned departure time. Route delay uses the terminal delay from
						the last non-cancelled arrival event of the journey where possible. Cancelled events are excluded from delay
						averages and punctuality rates.
					</p>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="rankings">
				<Accordion.Trigger>How should the rankings be read?</Accordion.Trigger>
				<Accordion.Content>
					<p class="leading-relaxed text-pretty">
						Rankings are sorted by the selected metric, but the interpretation depends on polarity. For punctuality, high ranks
						highlight stronger performance. For cancellation rate or delay, high ranks identify the highest problem values
						rather than winners.
					</p>
				</Accordion.Content>
			</Accordion.Item>
		</Accordion.Root>
	</section>
</main>
