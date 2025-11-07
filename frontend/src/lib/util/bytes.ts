const isUnitBytes = (unit: string) => unit.toLowerCase().includes("bytes");

const getUnit = (bytes: number, useDecimal: boolean = false): string => {
	if (bytes === 0) return "Bytes";

	const k = useDecimal ? 1000 : 1024;
	const sizes = useDecimal
		? ["Bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"]
		: ["Bytes", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB"];

	const i = Math.floor(Math.log(bytes) / Math.log(k));
	return sizes[i];
};

const formatBytes = (bytes: number, useDecimal: boolean = false, decimals = 2): number => {
	if (bytes === 0) return 0;

	const k = useDecimal ? 1000 : 1024;
	const dm = decimals < 0 ? 0 : decimals;
	const i = Math.floor(Math.log(bytes) / Math.log(k));
	return parseFloat((bytes / Math.pow(k, i)).toFixed(dm));
};

export { isUnitBytes, getUnit, formatBytes };
