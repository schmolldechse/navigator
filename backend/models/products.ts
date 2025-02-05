enum Products {
    HOCHGESCHWINDIGKEITSZUEGE = "HOCHGESCHWINDIGKEITSZUEGE",
    INTERCITYUNDEUROCITYZUEGE = "INTERCITYUNDEUROCITYZUEGE",
    INTERREGIOUNDSCHNELLZUEGE = "INTERREGIOUNDSCHNELLZUEGE",
    NAHVERKEHRSONSTIGEZUEGE = "NAHVERKEHRSONSTIGEZUEGE",
    SBAHNEN = "SBAHNEN",
    BUSSE = "BUSSE",
    SCHIFFE = "SCHIFFE",
    UBAHN = "UBAHN",
    STRASSENBAHN = "STRASSENBAHN",
    ANRUFPFLICHTIGEVERKEHRE = "ANRUFPFLICHTIGEVERKEHRE"
}

const mapToEnum = (value: string): Products | undefined => {
    return Object.keys(Products).some(key => Products[key as keyof typeof Products] === value) ? value as Products : undefined;
}

export { Products, mapToEnum };