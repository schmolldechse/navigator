const extractLeadingLetters = (input: string): string => {
	const match = input.match(/^([A-Za-z]+)/);
	return match ? match[1] : "";
}

export { extractLeadingLetters };