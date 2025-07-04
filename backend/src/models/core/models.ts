import { StationSchema } from "../elysia/station.model";
import { TimeSchema } from "../elysia/time.model";
import { LineColorSchema } from "../elysia/connection.model";
import { GroupedTimetableEntrySchema, TimetableMessageSchema, TimetableStopSchema } from "../elysia/timetable.model";
import { JourneyStatisticsSchema } from "../elysia/analytics.model";
import { RouteDetailsSchema } from "../elysia/route.model";

type Station = typeof StationSchema.static;

type Time = typeof TimeSchema.static;
type LineColor = typeof LineColorSchema.static;

type JourneyStatistics = typeof JourneyStatisticsSchema.static;

type TimetableEntry = typeof GroupedTimetableEntrySchema.static;
type TimetableStop = typeof TimetableStopSchema.static;
type TimetableMessage = typeof TimetableMessageSchema.static;

type RouteDetails = typeof RouteDetailsSchema.static;

export { Station, Time, LineColor, JourneyStatistics, TimetableEntry, TimetableStop, TimetableMessage, RouteDetails };
