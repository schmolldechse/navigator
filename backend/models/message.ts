export interface Message {
	type: string; // e.g. "bicycle-transport", "canceled-stops", "additional-stops", ...
	text?: string;
	change?: boolean;
	important?: boolean;
	open?: boolean;
	links: any[];
}

/**
 * Examples:
 *
 * Label: [{"type":"link","label":"+43 517 17","href":"tel:+4351717","openInNewTab":false}]
 * Station: [{"type":"station","name":"Norddeich Mole","evaNumber":"8007768","slug":"norddeich-mole","nameParts":[{"type":"city","value":"Norddeich"},{"type":"city-separator","value":" "},{"type":"default","value":"Mole"}]}]
 * Replacement Service: [{"type":"replacement-service-map","label":"Wo fährt der Bus?"},{"type":"line","lineName":"ICE 519","transportType":"HIGH_SPEED_TRAIN","isCanceled":false,"isReplacementBusService":false}]
 *
 * or may hold a simple "name" message object
 */
