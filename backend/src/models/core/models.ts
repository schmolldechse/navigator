import { StationSchema, StopSchema } from "../elysia/station.model";
import { TimeSchema } from "../elysia/time.model";
import { ProductSchema } from "../elysia/product.model";
import { MessageSchema } from "../elysia/message.model";
import { ConnectionSchema, LineColorSchema } from "../elysia/connection.model";
import { RouteDataSchema } from "../elysia/route.model";
import { StopAnalyticsSchema } from "../elysia/analytics.model";

type Station = typeof StationSchema.static;
type Stop = typeof StopSchema.static;
type Time = typeof TimeSchema.static;
type Product = typeof ProductSchema.static;
type Message = typeof MessageSchema.static;
type Connection = typeof ConnectionSchema.static;
type LineColor = typeof LineColorSchema.static;
type RouteData = typeof RouteDataSchema.static;
type StopAnalytics = typeof StopAnalyticsSchema.static;

export { Station, Stop, Time, Product, Message, Connection, LineColor, RouteData, StopAnalytics };
