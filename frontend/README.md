# Navigator Frontend

The frontend is the SvelteKit dashboard for Navigator. It presents project-level counters, global network statistics, station-specific statistics, rankings, and map-based quality views from the Navigator API.

## Responsibilities

- Render the main Navigator dashboard and statistics experience.
- Query the backend API through SvelteKit server-side remote functions.
- Use generated OpenAPI types and Valibot schemas from `src/lib/api`.
- Provide station search and station-detail navigation.
- Visualize time series, rankings, transport breakdowns, and station map metrics.

The `/timetable` route currently exists as a preview placeholder. The active product focus is the statistics dashboard.

## Stack

- [SvelteKit](https://svelte.dev/docs/kit) with Svelte 5
- [Bun](https://bun.sh/) for package management and scripts
- [Tailwind CSS](https://tailwindcss.com/) through the Vite plugin
- [LayerChart](https://www.layerchart.com/) for chart components
- [MapLibre GL JS](https://maplibre.org/maplibre-gl-js/docs/) for map rendering
- [@hey-api/openapi-ts](https://heyapi.dev/openapi-ts/) for generated API types

## Configuration

Create `frontend/.env` from `frontend/.env.example`:

```sh
cp .env.example .env
```

The required public API URL is:

```text
PUBLIC_API_URL=http://localhost:5019/
```

This value is read by the server-side remote modules in `src/lib/remote`.

## Development

Install dependencies:

```sh
bun install
```

Start the Vite development server:

```sh
bun run dev
```

Run checks:

```sh
bun run lint
bun run check
bun run build
```

Regenerate API types while `Navigator.Api` is running:

```sh
bun run generate:api
```

`openapi-ts.config.ts` reads the backend OpenAPI document from:

```text
http://localhost:5019/swagger.json
```

## References

- [Navigator API README](../backend/Navigator.Api/README.md)
- [SvelteKit documentation](https://svelte.dev/docs/kit)
- [LayerChart documentation](https://www.layerchart.com/)
- [MapLibre GL JS documentation](https://maplibre.org/maplibre-gl-js/docs/)
- [@hey-api/openapi-ts documentation](https://heyapi.dev/openapi-ts/)
