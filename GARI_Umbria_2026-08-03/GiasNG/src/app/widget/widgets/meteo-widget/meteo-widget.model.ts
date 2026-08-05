export interface Main {
    temp: number;
    feels_like: number;
    temp_min: number;
    temp_max: number;
    pressure: number;
    sea_level: number;
    grnd_level: number;
    humidity: number;
    temp_kf: number;
}

export interface Weather {
    id: number;
    main: string;
    description: string;
    icon: string;
}

export interface Clouds {
    all: number;
}

export interface Wind {
    speed: number;
    deg: number;
    gust: number;
}

export interface Sys {
    pod: string;
}

// export interface Rain {
//     3h: number;
// }

export interface List {
    dt: number;
    main: Main;
    weather: Weather[];
    clouds: Clouds;
    wind: Wind;
    visibility: number;
    pop: number;
    sys: Sys;
    dt_txt: string;
    rain: any;
}

export interface MeteoObservation {
    dt: number;
    main: Main;
    weather: Weather[];
    clouds: Clouds;
    wind: Wind;
    visibility: number;
    pop: number;
    dt_txt: string;
    rain: string;
}

export interface HubMeteoObservation {
    dataOra: string;
    dataOraLocale?: string;
    valore: number;
    descr: string;
    icon: string;
}

export interface Coord {
    lat: number;
    lon: number;
}

export interface City {
    id: number;
    name: string;
    coord: Coord;
    country: string;
    population: number;
    timezone: number;
    sunrise: number;
    sunset: number;
}

export interface MeteoData {
    cod: string;
    message: number;
    cnt: number;
    list: MeteoObservation[];
    city: City;
}

export interface HubMeteoData {
    icons: HubMeteoObservation[];
}
