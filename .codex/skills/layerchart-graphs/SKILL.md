---
name: layerchart-graphs
description: Create, revise, and maintain charts, graphs, network diagrams, and data visualizations in Navigator's Svelte frontend using LayerChart Next v2. Use when Codex works on frontend chart components or routes, LayerChart imports, time series, bar/area/line/scatter/pie/arc/sankey/force/geo/heatmap visualizations, tooltip/legend/scale/brush/zoom behavior, or migrating chart code to LayerChart v2.
---

# LayerChart Graphs

## Scope

Use this skill for Navigator frontend work involving charts, graphs, diagrams, or data visualization. Treat LayerChart Next v2 as the charting library for this repo. Do not introduce another chart library, a parallel SVG chart framework, or imperative D3 DOM rendering unless the user explicitly asks for it. D3 utilities are fine for data shaping, scales, curves, layouts, or force configuration when LayerChart expects them.

Read `references/layerchart-next-v2.md` when creating a new chart, changing LayerChart API usage, adding interactivity, or resolving uncertainty about LayerChart v2 props. If more LayerChart context is needed and the official LLM documentation has not already been fetched in the current turn, fetch `https://next.layerchart.com/docs/llms.txt` before relying on memory.

## Workflow

1. Inspect local context first: `frontend/package.json`, existing files under `frontend/src/lib/components/layerchart`, domain components under `frontend/src/lib/components/statistics`, and the route or API data shape.
2. Choose the smallest LayerChart abstraction that fits:
   - Use simplified charts (`LineChart`, `AreaChart`, `BarChart`, `ScatterChart`, `PieChart`, `ArcChart`) for conventional visualizations.
   - Use `<Chart>` plus `<Layer>` and marks/primitives for custom composition, annotations, nonstandard overlays, or mixed marks.
   - Use LayerChart layout packages for graph structures such as `Sankey` from `layerchart/graph` and `ForceSimulation` from `layerchart/force`.
3. Normalize data before rendering. Prefer typed view-model arrays, stable keys, real `Date` objects for time scales, numbers for continuous scales, and strings for categorical band scales.
4. Model multi-series data with LayerChart `series` where possible instead of duplicating whole chart components. Use per-mark data only when the series truly have different shapes or lengths.
5. Add interaction through LayerChart state and components: `tooltipContext`, `Tooltip`, `Highlight`, `BrushContext`, `TransformContext`, and `Legend`. Pick tooltip modes by geometry, not habit.
6. Keep Navigator UI conventions: Svelte 5 runes, TypeScript props near the top, Tailwind utility classes with semantic tokens, `@lucide/svelte` icons for controls, and stable responsive dimensions.
7. Validate with the relevant frontend command from `frontend`, usually `bun run check`; add `bun run lint` or browser verification when layout, visual interaction, or responsiveness changed.

## Placement

- Put reusable LayerChart-specific helpers/components in `frontend/src/lib/components/layerchart`.
- Put domain-specific chart widgets near their feature area, for example `frontend/src/lib/components/statistics/...`.
- Keep generic controls, buttons, dialogs, and layout primitives in `frontend/src/lib/components/ui`; combine this with `$reusable-ui-components` when building reusable non-chart UI.
- Avoid placing route-specific data fetching or API DTO assumptions inside generic chart components.

## Implementation Notes

- Prefer LayerChart's built-in axes, grid, rule, legends, highlights, and tooltips before custom recreations.
- Use the `marks`, `belowMarks`, `aboveMarks`, `belowContext`, and `aboveContext` snippets before replacing the entire chart with a `children` snippet.
- Use `tooltipContext` on `<Chart>` and simplified charts. In v2, the old `<Chart tooltip={...}>` prop is `tooltipContext`.
- For LayerChart v2 migrations, use `layer` instead of `renderContext`, `$left`/`$right`/`$top`/`$bottom` for `Axis` positions, `data` instead of the old `bar` prop on `Bar`, and `pathRef` instead of `splineRef`.
- Style with current color, LayerChart CSS variables, and targetable `lc-*` classes where useful. Keep colors aligned with Navigator tokens such as `bg-background`, `text-foreground`, `text-muted-foreground`, `border-border`, `bg-secondary`, and `bg-accent`.
- Ensure charts have explicit responsive height or aspect constraints so axes, legends, loading states, and empty states do not shift the surrounding layout.
- Include empty, loading, and error states for charts backed by remote data.
