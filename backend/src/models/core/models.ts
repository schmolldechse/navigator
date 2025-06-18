import { StationSchema } from "../elysia/station.model";
import { TimeSchema } from "../elysia/time.model";
import { ConnectionSchema, LineColorSchema } from "../elysia/connection.model";
import { StopAnalyticsSchema } from "../elysia/analytics.model";
import { GroupedTimetableEntrySchema, TimetableMessageSchema, TimetableStopSchema } from "../elysia/timetable.model";

type Station = typeof StationSchema.static;

type Time = typeof TimeSchema.static;
type Connection = typeof ConnectionSchema.static;
type LineColor = typeof LineColorSchema.static;
type StopAnalytics = typeof StopAnalyticsSchema.static;

type TimetableEntry = typeof GroupedTimetableEntrySchema.static;
type TimetableStop = typeof TimetableStopSchema.static;
type TimetableMessage = typeof TimetableMessageSchema.static;

export { Station, Time, Connection, LineColor, StopAnalytics, TimetableEntry, TimetableStop, TimetableMessage };
