import { enum_zoom } from "app/GIS/GIS-enum/GIS-zoom";

export class CentraMappa {
    punto: LatLng;
    livelloZoom: number;

    constructor() {
        this.punto = new LatLng();
        this.livelloZoom = enum_zoom.defaultCentraMappaSenzaPoligoni;
    }

}

export class LatLng {
    lat?: number;
    lng?: number;
    isValid: boolean;

    constructor() {
        this.lat = null;
        this.lng = null;
        this.isValid = false;
    }

}
