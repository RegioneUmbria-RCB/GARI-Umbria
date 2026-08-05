import { enum_OrigineChiamataFilterService } from "app/GIS/GIS-enum/GIS-origine-chiamata";

export class GeoJsonFilterServiceParam {
    public OrigineChiamata: enum_OrigineChiamataFilterService;
    public ImpostaCentroMappa: boolean;
    public FlagLoadGeoJson: boolean;

    constructor(
        origineChiamata: enum_OrigineChiamataFilterService,
        impostaCentroMappa: boolean,
        flagLoadGeoJson: boolean
    ) {
        this.OrigineChiamata = origineChiamata;
        this.ImpostaCentroMappa = impostaCentroMappa;
        this.FlagLoadGeoJson = flagLoadGeoJson;
    }

}
