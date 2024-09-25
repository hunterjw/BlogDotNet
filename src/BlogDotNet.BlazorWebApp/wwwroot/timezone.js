export function getBrowserTimeZone() {
    const options = Intl.DateTimeFormat().resolvedOptions();
    return options.timeZone;
}

export function getBrowserLocale() {
    const options = Intl.DateTimeFormat().resolvedOptions();
    return options.locale;
}