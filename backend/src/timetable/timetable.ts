import { GroupedTimetableEntrySchema } from "../models/elysia/timetable.model";

abstract class Timetable {
    abstract fetchTimetable(): Promise<typeof GroupedTimetableEntrySchema.static[]>;
}

export { Timetable };