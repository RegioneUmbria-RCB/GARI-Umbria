import { Injectable } from "@angular/core";
import { BehaviorSubject, Subject } from 'rxjs';
import { GeoJsonFilterServiceParam } from "app/Model/GIS/GeoJsonFilterServiceParam";
import { enum_OrigineChiamataFilterService } from "../GIS-enum/GIS-origine-chiamata";
import { CentraMappa, LatLng } from "app/Model/GIS/Utility";
import { getServiceIdAndLog } from "app/Service/utils";
import { GisDataReadParam } from "app/Service/api.service";

@Injectable({
    providedIn: 'root'
})
export class GeoJsonFilterService {

    gisDataReadParam: GisDataReadParam = null;

    gisDataReadParamSource = new BehaviorSubject(this.gisDataReadParam);

    geoJsonFilterServiceParam = new GeoJsonFilterServiceParam(
        enum_OrigineChiamataFilterService.Indefinito,
        true,
        false
    );

    geoJsonFilterServiceParamSource = new BehaviorSubject(this.geoJsonFilterServiceParam);

    centraMappa: CentraMappa = null;

    centraMappaSource = new BehaviorSubject(this.centraMappa);

    qdcConPoligoni: boolean = null;

    qdcConPoligoniSource = new BehaviorSubject(this.qdcConPoligoni);

    private serviceId = null;

    constructor () {
        this.serviceId = getServiceIdAndLog('GeoJsonFilterService','constructor');
    }

    public setGisDataReadParam(gisDataReadParam: GisDataReadParam) {
        this.gisDataReadParamSource.next(gisDataReadParam);
    }

    public getGisDataReadParam(): GisDataReadParam {
        return this.gisDataReadParamSource.getValue();
    }

    public setGeoJsonFilterServiceParam(geoJsonFilterServiceParam: GeoJsonFilterServiceParam) {
        this.geoJsonFilterServiceParamSource.next(geoJsonFilterServiceParam);
    }

    public getGeoJsonFilterServiceParam(): GeoJsonFilterServiceParam {
        return this.geoJsonFilterServiceParamSource.getValue();
    }

    public setCentraMappa(centraMappa: CentraMappa) {
        this.centraMappaSource.next(centraMappa);
    }

    public getCentraMappa(): CentraMappa {
        return this.centraMappaSource.getValue();
    }

    public setQdcConPoligoni(qdcConPoligoni: boolean) {
        this.qdcConPoligoniSource.next(qdcConPoligoni);
    }

    public getQdcConPoligoni(): boolean {
        return this.qdcConPoligoniSource.getValue();
    }

}
