# LayerChart Next v2 Reference

## Official Docs

Start from the official LLM index when current API details matter. If it has not already been read in the current turn, fetch `https://next.layerchart.com/docs/llms.txt` for broader LayerChart Next v2 context before making API-sensitive changes:

- Root LLM documentation: https://next.layerchart.com/docs/llms.txt
- Getting started: https://next.layerchart.com/docs/getting-started/llms.txt
- Structure: https://next.layerchart.com/docs/guides/structure/llms.txt
- Data: https://next.layerchart.com/docs/guides/data/llms.txt
- Scales: https://next.layerchart.com/docs/guides/scales/llms.txt
- Styling: https://next.layerchart.com/docs/guides/styles/llms.txt
- Tooltip: https://next.layerchart.com/docs/guides/tooltip/llms.txt
- Series: https://next.layerchart.com/docs/guides/series/llms.txt
- Brush: https://next.layerchart.com/docs/guides/brush/llms.txt
- Transform: https://next.layerchart.com/docs/guides/transform/llms.txt
- v1 to v2 migration: https://next.layerchart.com/docs/guides/migrations/v1-to-v2/llms.txt

Use component-specific LLM pages by appending `/llms.txt`, for example:

- `https://next.layerchart.com/docs/components/LineChart/llms.txt`
- `https://next.layerchart.com/docs/components/BarChart/llms.txt`
- `https://next.layerchart.com/docs/components/AreaChart/llms.txt`
- `https://next.layerchart.com/docs/components/ScatterChart/llms.txt`
- `https://next.layerchart.com/docs/components/Chart/llms.txt`
- `https://next.layerchart.com/docs/components/Sankey/llms.txt`
- `https://next.layerchart.com/docs/components/ForceSimulation/llms.txt`

## Navigator Baseline

- Frontend lives in `frontend`.
- Package manager is Bun; prefer commands from `frontend/package.json`.
- Current chart dependency is `layerchart` `2.0.0-next.62`.
- Svelte is v5 and Tailwind is v4.
- Existing LayerChart-specific code starts under `frontend/src/lib/components/layerchart`, including `tooltips/DateTooltip.svelte`.

## Choosing Components

- Use `LineChart` for trends and time series.
- Use `AreaChart` for magnitude over intervals, cumulative views, ranges, and stacked flows over time.
- Use `BarChart` for category comparisons, grouped bars, stacks, and horizontal rankings.
- Use `ScatterChart` for point clouds, correlations, punchcards, and radius or color encoded points.
- Use `PieChart` or `ArcChart` only when part-to-whole comparison is small and labels remain readable.
- Use `Sankey` for flow magnitude between categories.
- Use `ForceSimulation` for node-link networks where physical layout helps reveal relationships.
- Use `<Chart>` with marks/primitives when simplified charts hide needed composition details.

## Data And Scales

- LayerChart infers scale types from domain/data values: `Date` maps to time, numbers to linear, strings to band.
- Use explicit domains such as `yDomain={[0, null]}` when a metric should start at zero.
- Keep date parsing outside the chart component when possible; pass `Date` instances into LayerChart.
- For time-series tooltips with `bisect-x`, sort by x. Use `quadtree-x` when data may be unsorted.
- Use stable keyed each-blocks for custom nodes, bars, links, and labels.

## Tooltips And Interaction

- Use `tooltipContext` on `<Chart>` or simplified charts.
- Use `Tooltip.Root`, `Tooltip.Header`, `Tooltip.List`, and `Tooltip.Item` for styled tooltip content.
- Use `Highlight` for crosshairs or hovered points when it improves scanning.
- Pick tooltip modes by shape:
  - `bisect-x` or `quadtree-x` for line and area charts.
  - `band` or `bisect-band` for bars.
  - `voronoi` or `quadtree` for scatter plots.
  - `manual` for geo, radial, or custom shapes.
- For charts with date hover labels, consider reusing or extending `frontend/src/lib/components/layerchart/tooltips/DateTooltip.svelte`.

## LayerChart v2 Reminders

- Use Svelte 5 runes and snippets.
- Use `tooltipContext`, not the old `tooltip` prop, on `<Chart>`.
- Use `layer`, not `renderContext`, on simplified charts.
- Use `Axis x="$left"`, `Axis x="$right"`, `Axis y="$top"`, and `Axis y="$bottom"` for fixed axis positions.
- Use `Bar data={...}`, not `Bar bar={...}`.
- Use `bind:pathRef`, not `bind:splineRef`.
- Use `getChartContext()` for chart state access; avoid removed standalone context getters.
