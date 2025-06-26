import { t } from "elysia";

const BasicStationSchema = t.Object({
	name: t.String({ description: "Name of the station" }),
	evaNumber: t.Number({ description: "Unique evaNumber of the station" })
});

const StationSchema = t.Intersect([
	BasicStationSchema,
	t.Object({
		coordinates: t.Object(
			{
				latitude: t.Number({ description: "Latitude of the station" }),
				longitude: t.Number({ description: "Longitude of the station" })
			},
			{ description: "Coordinates of the station" }
		),
		ril100: t.Optional(t.Array(t.String(), { description: "List of RIL100 numbers of the station" })),
		products: t.Array(t.String(), { description: "List of products available at the station" })
	})
]);

const StationDatabaseSchema = t.Intersect([
	StationSchema,
	t.Object({
		weight: t.Optional(t.Number())
	})
]);

export { BasicStationSchema, StationSchema, StationDatabaseSchema };
