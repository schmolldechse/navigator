import { LargeStopSchema, StationSchema } from "../elysia/station.model";
import { TimeSchema } from "../elysia/time.model";
import { MessageSchema } from "../elysia/message.model";
import { ConnectionSchema, LineColorSchema } from "../elysia/connection.model";
import { RouteDataSchema } from "../elysia/route.model";
import { StopAnalyticsSchema } from "../elysia/analytics.model";
import { GroupedTimetableEntrySchema, TimetableStopSchema } from "../elysia/timetable.model";

type Station = typeof StationSchema.static;
type LargeStop = typeof LargeStopSchema.static;

type Time = typeof TimeSchema.static;
type Message = typeof MessageSchema.static;
type Connection = typeof ConnectionSchema.static;
type LineColor = typeof LineColorSchema.static;
type RouteData = typeof RouteDataSchema.static;
type StopAnalytics = typeof StopAnalyticsSchema.static;

type TimetableEntry = typeof GroupedTimetableEntrySchema.static;
type TimetableStop = typeof TimetableStopSchema.static;

export { Station, LargeStop, Time, Message, Connection, LineColor, RouteData, StopAnalytics, TimetableEntry, TimetableStop };
