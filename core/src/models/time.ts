interface Time {
	// mostly as ISO string
	plannedTime: string;
	actualTime: string;

	// in sec
	delay: number;

	// platform may be null, for example for buses
	plannedPlatform?: string;
	actualPlatform?: string;
}

export type { Time };
