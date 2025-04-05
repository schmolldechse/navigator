interface Time {
	// mostly as ISO string
	plannedTime: string;
	actualTime: string;

	// in sec
	delay: number;
	plannedPlatform: string;
	actualPlatform: string;
}

export type { Time };
