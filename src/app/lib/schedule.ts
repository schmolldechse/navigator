export type ScheduledLine = {
    tripId: string,
    plannedWhen: string,
    actualWhen: string,
    delay: number, // in sec
    plannedPlatform: string,
    actualPlatform: string,
    directionName: string, // name of the destination
    directionId: string, // id of the destination
    line: Line
}

type Line = {
    id: string,
    name: string,
    operator: string
}
