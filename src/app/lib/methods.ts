const browserLanguage = (): string => {
    const supported = ['en', 'de'];
    const language = navigator.language.split("-")[0];
    return supported.includes(language) ? language : 'en';
}

export { browserLanguage };
