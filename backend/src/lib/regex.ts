const extractLeadingLetters = (input: string): string => {
	const match = input.match(/^([A-Za-z]+)/);
	return match ? match[1] : "";
}

const extractJourneyNumber = (input: string): number => {
	const match = input.match(/(\d+)$/);
	return match ? parseInt(match[1]) : 0;
}

export { extractLeadingLetters, extractJourneyNumber };