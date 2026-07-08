<script lang="ts">
	import ArrowRight from "@lucide/svelte/icons/arrow-right";
	import Database from "@lucide/svelte/icons/database";
	import Info from "@lucide/svelte/icons/info";
	import Network from "@lucide/svelte/icons/network";
	import Radar from "@lucide/svelte/icons/radar";
	import Search from "@lucide/svelte/icons/search";
	import Button from "@lib/components/ui/Button.svelte";
	import Card from "@lib/components/ui/card/Card.svelte";
	import * as Accordion from "@lib/components/ui/accordion";
	import type { LucideIcon } from "@lucide/svelte";

	let faqValues: string[] = $state([]);

	type DataFlowStep = {
		title: string;
		description: string;
		icon: LucideIcon;
	};

	const dataFlowSteps: DataFlowStep[] = [
		{
			title: "Discover services",
			description: "Station boards reveal active RIS IDs and transport coverage.",
			icon: Search
		},
		{
			title: "Store journeys",
			description: "Recurring services are captured with stops, delays, cancellations, and operator data.",
			icon: Database
		},
		{
			title: "Compare quality",
			description: "Global metrics turn the raw history into punctuality, delay, and ranking views.",
			icon: Network
		}
	];

	const projectFocusCards: DataFlowStep[] = [
		{
			title: "Raw journey archive",
			description: "Collected journeys keep stops, transport, delay and cancellation facts inspectable.",
			icon: Database
		},
		{
			title: "API-grained statistics",
			description: "Network and station views read specific CAGGs and detail views for each question.",
			icon: Network
		},
		{
			title: "Dashboard drilldown",
			description: "Maps, rankings, time series and station details share one consistent filter model.",
			icon: Radar
		}
	];
</script>

<svelte:head>
	<title>Navigator</title>
</svelte:head>

<main class="container mx-auto flex flex-col gap-y-10 p-4 sm:py-8">
	<!-- Hero Section -->
	<section class="relative">
		<div class="bg-accent absolute top-1 bottom-1 w-0.5 sm:-left-4 sm:w-1"></div>

		<div class="flex flex-col gap-y-5 pl-4 sm:pl-8">
			<h1 class="max-w-4xl text-3xl font-bold text-balance sm:text-4xl md:text-6xl">
				Visualizing public transport performance
			</h1>

			<p class="text-foreground/65 max-w-3xl text-base leading-relaxed text-pretty sm:text-lg lg:text-xl">
				Navigator collects German rail journey data and turns it into clear, inspectable statistics for punctuality, delays,
				cancellations, stations, operators, and lines.
			</p>

			<div class="flex flex-col gap-2 sm:flex-row">
				<Button href="/statistics" mode="primary" class="inline-flex items-center justify-center gap-x-2">
					Open statistics
					<ArrowRight size={18} />
				</Button>
				<Button href="/timetable" mode="secondary" class="inline-flex items-center justify-center gap-x-2">
					Timetable preview
				</Button>
			</div>
		</div>
	</section>

	<!-- Project Dimensions -->
	<section class="flex flex-col gap-y-6">
		<div class="flex flex-col gap-y-1">
			<h2 class="text-2xl font-medium">Project Dimensions</h2>
			<p class="text-foreground/60 max-w-3xl text-sm sm:text-base">
				The current statistics surface is organized around stored raw journeys, fact tables, and API-specific aggregates.
			</p>
		</div>

		<div class="grid grid-cols-1 items-start gap-4 sm:grid-cols-2 lg:grid-cols-3">
			{#each projectFocusCards as card (card.title)}
				{@const Icon = card.icon}
				<Card class="bg-secondary/10 gap-y-3">
					<div class="border-border bg-background flex size-10 items-center justify-center rounded-lg border">
						<Icon size={19} class="text-accent" />
					</div>
					<div>
						<p class="text-foreground font-semibold">{card.title}</p>
						<p class="text-foreground/60 mt-1 text-sm leading-relaxed">{card.description}</p>
					</div>
				</Card>
			{/each}
		</div>
	</section>

	<!-- Data Flow -->
	<section class="grid gap-4 lg:grid-cols-[minmax(0,0.8fr)_minmax(0,1.2fr)] lg:items-start">
		<div class="flex flex-col gap-y-2">
			<h2 class="text-2xl font-medium">How the data becomes insight</h2>
			<p class="text-foreground/60 text-sm leading-relaxed sm:text-base">
				Navigator separates collection, storage, and evaluation so statistics can be inspected by scope instead of being locked
				into annual summaries.
			</p>
		</div>

		<div class="grid gap-3">
			{#each dataFlowSteps as step, index (step.title)}
				{@const Icon = step.icon}
				<Card class="bg-secondary/15 grid grid-cols-[auto_minmax(0,1fr)] gap-x-3 gap-y-1">
					<div class="border-border bg-background flex size-9 items-center justify-center rounded-lg border">
						<Icon size={18} class="text-accent" />
					</div>
					<div class="min-w-0">
						<p class="text-foreground font-semibold">{index + 1}. {step.title}</p>
						<p class="text-foreground/60 text-sm leading-relaxed">{step.description}</p>
					</div>
				</Card>
			{/each}
		</div>
	</section>

	<section class="flex flex-col gap-y-4">
		<div class="flex flex-col gap-y-1">
			<h2 class="text-2xl font-medium">FAQ</h2>
			<p class="text-foreground/60 max-w-3xl text-sm sm:text-base">
				Answers to the most important questions about Navigator's data collection.
			</p>
		</div>

		<Accordion.Root type="multiple" bind:value={faqValues}>
			<Accordion.Item value="data-source">
				<Accordion.Trigger>Where do the data come from?</Accordion.Trigger>
				<Accordion.Content>
					<div class="flex flex-col gap-y-3 leading-relaxed text-pretty">
						<p>
							Navigator uses railway information from the
							<a
								href="https://developers.deutschebahn.com/db-api-marketplace/apis/"
								target="_blank"
								rel="noreferrer noopener"
								class="text-accent underline underline-offset-2"
							>
								Deutsche Bahn API Marketplace
							</a>
							. The main sources are <code>RIS::Boards</code>, which lists arrivals and departures for selected stations and
							time windows, and <code>RIS::Journeys</code>, which supplies the detailed journey record for an individual
							service.
						</p>
						<p>
							Navigator regularly checks station boards to find active services and their RIS IDs. Those IDs are used to
							retrieve journey details such as stops, planned and forecast times, platforms, delays, cancellations, operators,
							and route information. The statistics are built from the records collected over time.
						</p>
					</div>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="ris-id-matching">
				<Accordion.Trigger>How are journeys discovered?</Accordion.Trigger>
				<Accordion.Content>
					<div class="flex flex-col gap-y-3 leading-relaxed text-pretty">
						<p>
							Navigator does not begin with a complete timetable archive. It builds its catalogue from live boards: monitored
							stations are checked for arrivals and departures, and each service found there contributes a Deutsche Bahn journey
							ID.
						</p>
						<p>
							Because that ID contains the operating date, Navigator stores the stable part as a RIS ID. For each operating day,
							that stable ID is combined with the date again to request the detailed journey record from
							<code>RIS::Journeys</code>.
						</p>
					</div>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="dataset-coverage">
				<Accordion.Trigger>Is the dataset complete?</Accordion.Trigger>
				<Accordion.Content>
					<div class="flex flex-col gap-y-3 leading-relaxed text-pretty">
						<p>
							Navigator is a growing collection, not an official complete Deutsche Bahn punctuality archive. It covers services
							that appear on the monitored station boards and match the transport types enabled for those stations.
						</p>
						<p>
							Coverage grows as more stations are added and as RIS IDs are discovered over time. The statistics are therefore
							best read as transparent measurements of Navigator's collected dataset, not as a claim that every service in
							Germany has already been recorded.
						</p>
					</div>
				</Accordion.Content>
			</Accordion.Item>

			<Accordion.Item value="local-transit">
				<Accordion.Trigger>What about trams and local transit?</Accordion.Trigger>
				<Accordion.Content>
					<div class="flex flex-col gap-y-3 leading-relaxed text-pretty">
						<p>
							Trams, buses, and other local transit would be a great addition, especially for comparing rail journeys with the
							wider public transport network. The Deutsche Bahn RIS APIs can include public transport information in some
							contexts, but they are not necessarily the best primary source for local operators.
						</p>
						<p>
							For broader local transit coverage, Navigator would likely need an additional
							<a
								href="https://gtfs.org/realtime/feed-entities/"
								target="_blank"
								rel="noreferrer noopener"
								class="text-accent underline underline-offset-2"
							>
								GTFS Realtime
							</a>
							integration. GTFS Realtime is a common format used by transit agencies for trip updates, vehicle positions, service
							alerts, and related real-time information.
						</p>
					</div>
				</Accordion.Content>
			</Accordion.Item>
		</Accordion.Root>
	</section>
</main>
