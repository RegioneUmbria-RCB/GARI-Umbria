import { Injectable } from "@angular/core";
import { ChiaveAlbero, GeoJson_Feature_New_1OfGeoJSONAgroGisProp } from 'app/Service/api.service';
import { FunzioniComuniService } from "app/Service/FunzioniComuni.service";
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { enum_OrigineChiamata } from "../GIS-enum/GIS-origine-chiamata";
import { SharedDataService } from "./shared-data.service";
import { GisFeaturesUtils } from '../utils/gis-features.utils';

@Injectable()
export class FeatureService {

    private featureSelezionate: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] = [];
    private featureSelezionateSource = new BehaviorSubject(this.featureSelezionate);
    origineChiamata: enum_OrigineChiamata = enum_OrigineChiamata.Mappa;
    origineChiamataSource = new BehaviorSubject(this.origineChiamata);
    featureDaCopiare: GeoJson_Feature_New_1OfGeoJSONAgroGisProp = null;
    featureDaCopiareSource = new BehaviorSubject(this.featureDaCopiare);
    public featureDeleted: Subject<string> = new Subject<string>();
    private featureDoubleClickedSource = new Subject<GeoJson_Feature_New_1OfGeoJSONAgroGisProp>();

    constructor(
        private funzioniComuniService: FunzioniComuniService,
        private sharedDataService: SharedDataService
    ) { }

    inizializzaFeatureSelezionate() {
        this.setOrigineChiamata(enum_OrigineChiamata.Mappa);
        this.setFeatureSelezionate([]);
    }

    inizializzaFeatureDaCopiare() {
        this.setFeatureDaCopiare(null);
    }

    public setFeatureSelezionate(featureSelezionate: GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]) {
        this.featureSelezionateSource.next(featureSelezionate);
    }

    public addFeatureSelezionata(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp) {
        let featureSelezionate = this.getFeatureSelezionate();
        featureSelezionate.push(feature);
        this.featureSelezionateSource.next(featureSelezionate);
    }

    public removeFeatureSelezionata(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp) {
        const idFeature = feature.properties.id;
        const featureSelezionate = this.getFeatureSelezionate();
        const indexFeature = featureSelezionate.findIndex(element => element.properties.id === idFeature);
        if (indexFeature >= 0) {
            featureSelezionate.splice(indexFeature, 1);
            this.featureSelezionateSource.next(featureSelezionate);
        }
    }

    public getFeatureSelezionate(): GeoJson_Feature_New_1OfGeoJSONAgroGisProp[] {
        return this.featureSelezionateSource.getValue();
    }

    public getFeatureSelezionate$(): Observable<GeoJson_Feature_New_1OfGeoJSONAgroGisProp[]> {
        return this.featureSelezionateSource.asObservable();
    }

    public getFeatureSelezionataById(featureId: string): GeoJson_Feature_New_1OfGeoJSONAgroGisProp {
        return this.featureSelezionateSource.getValue().find(feature => feature.properties.id === featureId);
    }

    public getElencoChiaviAlberoFeatureCompleteSelezionate(): string[] {
        const featureSelezionate = this.getFeatureSelezionate();

        let elencoChiaviAlberoFeatureCompleteSelezionate: string[] = [];

        featureSelezionate.forEach(feature => {
            const chiaveAlberoCompleta = this.getChiaveAlberoCompletaByFeature(feature);
            elencoChiaviAlberoFeatureCompleteSelezionate.push(chiaveAlberoCompleta);
        });

        return elencoChiaviAlberoFeatureCompleteSelezionate;
    }

    public getChiaveAlberoCompletaByFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp) {
        const chiaveAlbero = feature.properties.chiavealbero;
        return FunzioniComuniService.chiaveAlberoRidottaToBig(chiaveAlbero);
    }

    public getObjChiaveAlberoByFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): ChiaveAlbero {
        const chiaveAlbero = this.getChiaveAlberoCompletaByFeature(feature);
        return FunzioniComuniService.scomponiChiaveAlbero(chiaveAlbero);
    }

    public getPrimaFeatureSelezionata() {
        const featureSelezionate = this.getFeatureSelezionate();
        return featureSelezionate.slice(0)[0]
    }

    public getUltimaFeatureSelezionata() {
        const featureSelezionate = this.getFeatureSelezionate();
        return featureSelezionate.slice(-1)[0]
    }

    public esistonoFeatureSelezionate() {
        return this.getFeatureSelezionate().length > 0;
    }

    // Origine chiamata
    public setOrigineChiamata(origineChiamata: enum_OrigineChiamata) {
        this.origineChiamataSource.next(origineChiamata);
    }

    public getOrigineChiamata(): enum_OrigineChiamata {
        return this.origineChiamataSource.getValue();
    }

    // Feature da copiare
    public setFeatureDaCopiare(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp) {
        this.featureDaCopiareSource.next(feature);
    }

    public getFeatureDaCopiare(): GeoJson_Feature_New_1OfGeoJSONAgroGisProp {
        return this.featureDaCopiareSource.getValue();
    }

    // Feature cancellate
    public get featureDeleted$(): Observable<string> {
        return this.featureDeleted.asObservable();
    }

    public deleteFeature(featureId: string): void {
        this.featureDeleted.next(featureId);
    }

    // Permessi
    public getPermessiFeature(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): PermessiFeature {
        let permessiFeature = new PermessiFeature();
        // Permessi layer
        let layerId = feature.properties.layer;
        let layer = this.sharedDataService.getTipologiaLayerById(layerId);
        let permessiLayer = this.sharedDataService.getPermessiLayer(layer);
        //--------------------------------------------------------------------------------
        // Inserimento
        //--------------------------------------------------------------------------------
        if (permessiLayer.inserimento) {
            const inserimento = feature.properties.inserimento;
            permessiFeature.inserimento = this.stringToBoolean(inserimento);
        } else {
            permessiFeature.inserimento = false;
        }
        //--------------------------------------------------------------------------------
        // Modifica,
        // ammessi solo i poligoni senza innerboud. tutto il resto non è al momento modificabile:
        // . Poligoni con innerBound
        // . MultiPlugons
        //--------------------------------------------------------------------------------
        if (permessiLayer.modifica && !GisFeaturesUtils.isGeoJsonFeaturePolygonWithInnerBounds(feature) && !GisFeaturesUtils.isGeoJsonFeatureMultipolygon(feature)) {
            const modifica = feature.properties.modifica;
            permessiFeature.modifica = this.stringToBoolean(modifica);
        } else {
            permessiFeature.modifica = false;
        }
        //--------------------------------------------------------------------------------
        // Cancellazione
        //--------------------------------------------------------------------------------
        if (permessiLayer.cancellazione) {
            const cancellazione = feature.properties.cancellazione;
            permessiFeature.cancellazione = this.stringToBoolean(cancellazione);

        } else {
            permessiFeature.cancellazione = false;
        }
        //--------------------------------------------------------------------------------
        // Informazioni
        //--------------------------------------------------------------------------------
        if (permessiLayer.informazioni) {
            const informazioni = feature.properties.informazioni;
            permessiFeature.informazioni = this.stringToBoolean(informazioni);
        } else {
            permessiFeature.informazioni = false;
        }
        //--------------------------------------------------------------------------------
        return permessiFeature;
    }

    stringToBoolean(stringValue: string): boolean {
        return stringValue.toLowerCase() === 'true';
    }

    public get featureDoubleClicked$(): Observable<GeoJson_Feature_New_1OfGeoJSONAgroGisProp> {
        return this.featureDoubleClickedSource.asObservable();
    }

    public nextFeatureDoubleClicked(feature: GeoJson_Feature_New_1OfGeoJSONAgroGisProp): void {
        this.featureDoubleClickedSource.next(feature);
    }
}

export class PermessiFeature {
    inserimento: boolean;
    modifica: boolean;
    cancellazione: boolean;
    informazioni: boolean
}

