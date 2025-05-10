import Elysia, { t } from "elysia";

const stationController = new Elysia({ prefix: "/station", tags: ["Stations"] })
	.get("/", ({ query }) => query, {
		query: t.Object({
			query: t.String({ required: true })
		})
	})
	.get("/:id", ({ params: { id } }) => id);

export default stationController;
