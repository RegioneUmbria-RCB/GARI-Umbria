import { Injectable } from "@angular/core";
import { BehaviorSubject, Subject, Subscription } from 'rxjs';
import { GoogleMapService } from "../google-map/google-map.service";
import { GisToolbarService } from '../GIS-toolbar/gis-toolbar.service';
import { getServiceIdAndLog } from "app/Service/utils";
import { GiasBaseDraw, GiasMarker, GiasPolygon, GiasPolyline } from '../models/gias-drawings.model';
import { LayerService } from './layer.service';
import { enum_LayerElementiGraficiStd } from '../GIS-enum/GIS-layer-elementi-grafici';
import { DrawingOptionsService } from "./drawing-options.service";
import { KendoWindowsService, WindowTypes } from "app/Service";
import {GoogleMapDataService} from './google.maps-services/google-map-data.service';
import { FeatureInformationService } from "./feature-information.service";
import { GeoJsonUtils } from "../utils/geo-json.utils";
import { GoogleMapGeoJsonLazyService } from "./google.maps-services/google-map-geojson-lazy.service";

@Injectable()
export class DrawingService {
    private giasPolygonDrawings: BehaviorSubject<Array<GiasPolygon>> = new BehaviorSubject<Array<GiasPolygon>>([]);
    private giasPolylineDrawings: BehaviorSubject<Array<GiasPolyline>> = new BehaviorSubject<Array<GiasPolyline>>([]);
    private giasMarkerDrawings: BehaviorSubject<Array<GiasMarker>> = new BehaviorSubject<Array<GiasMarker>>([]);

    removeEvent: Subject<GiasBaseDraw> = new Subject<GiasBaseDraw>();
    updatedDrawEvent: Subject<GiasBaseDraw> = new Subject<GiasBaseDraw>();
    addEvent: Subject<GiasBaseDraw> = new Subject<GiasBaseDraw>();

    private subs: Subscription = new Subscription();

    private get googleMapWrapper() {
        return this.googleMapService.googleMapWrapper;
    }

    public get polygons() {
        return (<Array<GiasPolygon>>this.giasPolygonDrawings.getValue().filter(d => d instanceof GiasPolygon));
    }

    public get markers() {
        return (<Array<GiasMarker>>this.giasMarkerDrawings.getValue().filter(d => d instanceof GiasMarker));
    }

    public get polylines() {
        return (<Array<GiasPolyline>>this.giasPolylineDrawings.getValue().filter(d => d instanceof GiasPolyline));
    }

    public get markers$() {
        return this.giasMarkerDrawings.asObservable();
    }

    private serviceId = null;

    constructor(
        private googleMapService: GoogleMapService,
        private googleMapDataService: GoogleMapDataService,
        private gisToolbarService: GisToolbarService,
        private layerService: LayerService,
        private drawingOptionsService: DrawingOptionsService,
        private kendoWindowsService: KendoWindowsService,
        private featureInformationService: FeatureInformationService,
        private googleMapGeoJsonLazyService: GoogleMapGeoJsonLazyService
    ) {
        this.serviceId = getServiceIdAndLog('DrawingService', 'constructor');

        this.subs.add(this.removeEvent.subscribe((drawing) => {
            this.removeDrawing(drawing?.id);
        }));

        this.subs.add(this.updatedDrawEvent.subscribe((drawing) => {
            this.updateGeometry(drawing);
        }));

        this.subs.add(this.layerService.layerItemSelected$.subscribe(l => {
            // in modo che al cambio del layer non rimangano elementi non salvati sulla mappa
            this.removeAllPolygons();
            this.removeAllMarkers();
            this.removeAllPolylines();
        }));
    }

    private newDrawId(array: GiasBaseDraw[]): number {
        let max_id = 0;
        array.forEach(d => {
            if (d.id > max_id){
              max_id = d.id
            }
        });
        return ++max_id;
    }

    private newPolygonID(): number {
        return this.newDrawId(this.giasPolygonDrawings.getValue());
    }

    private newPolylineID(): number {
        return this.newDrawId(this.giasPolylineDrawings.getValue());
    }

    private newMarkerID(): number {
        return this.newDrawId(this.giasMarkerDrawings.getValue());
    }

    public addDrawing(draw: google.maps.Polygon | google.maps.Marker | google.maps.Polyline, isMultiplePointAllowed: boolean = false, validatePolygon: boolean = false): void {
        if (draw instanceof google.maps.Polygon) {
            this.addPolygon(draw, isMultiplePointAllowed);
        } else if (draw instanceof google.maps.Marker) {
            this.addMarker(draw, isMultiplePointAllowed, validatePolygon);
        } else if (draw instanceof google.maps.Polyline) {
            this.addPolyline(draw, isMultiplePointAllowed);
        }
    }

    public fromPointToMarker(coordinates: google.maps.LatLng, isMultiplePointAllowed: boolean, validatePolygon: boolean): void {
        const markerOptions: google.maps.MarkerOptions = this.drawingOptionsService.getMarkerDrawingOptions(enum_LayerElementiGraficiStd.DEEFAULT);
        markerOptions.position = coordinates;
        markerOptions.map = this.googleMapWrapper.googleMap;
        this.addMarker(new google.maps.Marker(markerOptions), isMultiplePointAllowed, validatePolygon);
    }

    // types accepted as PolygonOption path: MVCArray<MVCArray<LatLng>> | MVCArray<LatLng> | Array<Array<LatLng|LatLngLiteral>> | Array<LatLng|LatLngLiteral>
    public fromPointsToPolygon(coordinates: google.maps.LatLng[] | google.maps.LatLngLiteral[], isMultiplePointAllowed: boolean): void {
        const polygon = this.getPolygonFromCoordinates(coordinates);
        polygon.setMap(this.googleMapWrapper.googleMap);
        this.addPolygon(polygon, isMultiplePointAllowed);
    }

    public fromPointsToPolyline(coordinates: google.maps.LatLng[] | google.maps.LatLngLiteral[], isMultiplePointAllowed: boolean): void {
        const polyline = this.getPolylineFromCoordinates(coordinates);
        polyline.setMap(this.googleMapWrapper.googleMap);
        this.addPolyline(polyline, isMultiplePointAllowed);
    }

    public getPolygonFromCoordinates(coordinates: google.maps.LatLng[] | google.maps.LatLngLiteral[]): google.maps.Polygon {
        const polygonOptions = this.drawingOptionsService.getPolygonDrawingOptions(enum_LayerElementiGraficiStd.DEEFAULT);
        polygonOptions.paths = coordinates;
        return new google.maps.Polygon(polygonOptions);
    }

    private getPolylineFromCoordinates(coordinates: google.maps.LatLng[] | google.maps.LatLngLiteral[]): google.maps.Polyline {
        const polylineOptions = this.drawingOptionsService.getPolylineDrawingOptions(enum_LayerElementiGraficiStd.DEEFAULT);
        polylineOptions.path = coordinates;
        return new google.maps.Polyline(polylineOptions);
    }

    private addPolygon(draw: google.maps.Polygon, isMultiplePointAllowed: boolean): void {
        let giasPolygon = new GiasPolygon(
            this,
            this.newPolygonID(),
            <google.maps.Polygon>draw,
            this.layerService.layerItemSelected[0]
        );
        // TODO Salvo: cercare soluzione migliore di quella sottostante appena se ne ha il tempo
        // per evitare che rimangano dei poligoni disegnati sulla mappa
        if (isMultiplePointAllowed) {
            this.giasPolygonDrawings.next([...this.giasPolygonDrawings.value, giasPolygon])
        } else {
            this.removeAllPolygons();
            this.giasPolygonDrawings.next([giasPolygon]);
        }

        draw = null;
        const fo: google.maps.Data.FeatureOptions = {
            id: giasPolygon.id,
            geometry: new google.maps.Data.Polygon(
                giasPolygon.polygon.getPaths().getArray().map((p, i) => [p.getAt(i), p.getAt(i)])
            )
        };

        const f = new google.maps.Data.Feature(fo);
        const geoJsonFeature = GeoJsonUtils.featureToGeoJson(f);
        this.featureInformationService.addFeatures(this.googleMapWrapper.data.getMap().getProjection(), true, geoJsonFeature);

        giasPolygon.polygon.addListener('click', this.checkForRemove.bind(this));

        let firstPointCoordinates: google.maps.LatLng = new google.maps.LatLng(giasPolygon.polygon.getPaths().getArray()[0].getArray()[0]);
        this.gisToolbarService.setLatLng(firstPointCoordinates.lat(), firstPointCoordinates.lng());

        this.addEvent.next(giasPolygon);
    }

    private addMarker(draw: google.maps.Marker, isMultiplePointAllowed: boolean, validatePolygon: boolean): void {
        let fo: google.maps.Data.FeatureOptions;
        let f: google.maps.Data.Feature;

        let giasMarker = new GiasMarker(
            this,
            this.newMarkerID(),
            <google.maps.Marker>draw,
            this.layerService.layerItemSelected[0],
            isMultiplePointAllowed
        );

        if (validatePolygon) {
            const drawingManager = require('../../GiasJSLibraries/GIS-js-libraries/DrawingManager');
            const poligoValidator = this.getPolygonValidator([...this.giasMarkerDrawings.value, giasMarker], drawingManager);
            if (!poligoValidator.isValid) {
                drawingManager.DisplayErrorPolyLines(poligoValidator.intersection, this.googleMapWrapper.googleMap);
                return;
            }
        }

        giasMarker.marker.setVisible(true);

        // TODO Salvo: gestire disegno multipoint, quindi la riga sottostante va ripensata
        if (isMultiplePointAllowed) {
            this.giasMarkerDrawings.next([...this.giasMarkerDrawings.value, giasMarker])
        } else {
            this.removeAllMarkers();
            this.giasMarkerDrawings.next([giasMarker]);
        }

        draw = null;
        fo = {
            id: giasMarker.id,
            geometry: new google.maps.Data.Point(
                giasMarker.marker.getPosition()
            )
        };

        this.gisToolbarService.setLatLng(giasMarker.marker.getPosition().lat(), giasMarker.marker.getPosition().lng());

        if (!this.kendoWindowsService.getOpenState(WindowTypes.MarkerWindow)) {
            this.addEvent.next(giasMarker);
        }
    }

    private getPolygonValidator(giasMarkers: GiasMarker[], drawingManager: any): any {
        const polygon = this.getPolygonFromCoordinates(giasMarkers.map(x => x.marker.getPosition()));
        const polygonValidator = new drawingManager.PolygonValidator(polygon.getPath());
        return polygonValidator;
    }

    private addPolyline(draw: google.maps.Polyline, isMultiplePointAllowed: boolean): void {
        let fo: google.maps.Data.FeatureOptions;
        let f: google.maps.Data.Feature;

        let giasPolyline = new GiasPolyline(
            this,
            this.newPolylineID(),
            <google.maps.Polyline>draw,
            this.layerService.layerItemSelected[0]
        );

        if (isMultiplePointAllowed) {
            this.giasPolylineDrawings.next([...this.giasPolylineDrawings.value, giasPolyline])
        } else {
            this.giasPolylineDrawings.next([giasPolyline]);
        }

        draw = null;
        fo = {
            id: giasPolyline.id,
            geometry: new google.maps.Data.LineString(
                giasPolyline.polyline.getPath().getArray()
            )
        };

        f = new google.maps.Data.Feature(fo);
        const geoJsonFeature = GeoJsonUtils.featureToGeoJson(f);
        this.featureInformationService.addFeatures(this.googleMapWrapper.data.getMap().getProjection(), true, geoJsonFeature);

        let firstPointCoordinates: google.maps.LatLng = new google.maps.LatLng(giasPolyline.polyline.getPath()[0]);
        this.gisToolbarService.setLatLng(firstPointCoordinates.lat(), firstPointCoordinates.lng());

        this.addEvent.next(giasPolyline);
    }

    public removeDrawing(id: any, validatePolygon: boolean = false): void {
        let p: GiasPolygon | GiasMarker | GiasPolyline = this.giasPolygonDrawings.getValue().find(d => d.id == id);
        if (p == undefined) p = this.giasMarkerDrawings.getValue().find(d => d.id == id);
        if (p == undefined) p = this.giasPolylineDrawings.getValue().find(d => d.id == id);

        let idx = this.giasPolygonDrawings.getValue().findIndex(d => d.id == id);
        if (idx == -1) idx = this.giasMarkerDrawings.getValue().findIndex(d => d.id == id);
        if (idx == -1) idx = this.giasPolylineDrawings.getValue().findIndex(d => d.id == id);

        let f = this.googleMapWrapper.data.getFeatureById(id);

        if (p instanceof GiasPolygon) {
            (<GiasPolygon>p).polygon.setMap(null);
            let newValue = this.giasPolygonDrawings.getValue();
            newValue.splice(idx, 1);
            this.giasPolygonDrawings.next(newValue);
        } else if (p instanceof GiasMarker) {
            if (validatePolygon && this.giasMarkerDrawings.getValue().length > 2) {
                const drawingManager = require('../../GiasJSLibraries/GIS-js-libraries/DrawingManager');
                const markers = [...this.giasMarkerDrawings.getValue()];
                markers.splice(idx, 1);
                const poligoValidator = this.getPolygonValidator(markers, drawingManager);
                if (!poligoValidator.isValid) {
                    drawingManager.DisplayErrorPolyLines(poligoValidator.intersection, this.googleMapWrapper.googleMap);
                    return;
                }
            }

            (<GiasMarker>p).marker.setMap(null);
            let newValue = this.giasMarkerDrawings.getValue();
            newValue.splice(idx, 1);
            this.giasMarkerDrawings.next(newValue);
        } else if (p instanceof GiasPolyline) {
            (<GiasPolyline>p).polyline.setMap(null);
            let newValue = this.giasPolylineDrawings.getValue();
            newValue.splice(idx, 1);
            this.giasPolylineDrawings.next(newValue);
        }
        // if (f) {
        //     const feature = this.featureInformationService.getById(f.getId().toString());
        //     this.googleMapDataService.removeFeature(feature);
        // }
    }

    public removeAllPolygons(): void {
        this.giasPolygonDrawings.getValue().forEach(p => {
            p.polygon.setMap(null);
            p = null;
        });
        this.giasPolygonDrawings.next([]);
    }

    public removeAllMarkers(): void {
        this.giasMarkerDrawings.getValue().forEach(m => {
            m.marker.setMap(null);
            m = null;
        });
        this.giasMarkerDrawings.next([]);
    }

    private removeAllPolylines(): void {
        this.giasPolylineDrawings.getValue().forEach(p => {
            p.polyline.setMap(null);
            p = null;
        });
        this.giasPolylineDrawings.next([]);
    }

    private updateGeometry(drawing: GiasBaseDraw): void {
        let f = this.googleMapWrapper.data.getFeatureById(drawing.id);

        if (drawing instanceof GiasPolygon) {
            let geometry = new google.maps.Data.Polygon(
                drawing.polygon.getPaths().getArray().map((p, i) => [p.getAt(i), p.getAt(i)])
            )
            f.setGeometry(geometry);
        } else if (drawing instanceof GiasPolyline) {
            let geometry = new google.maps.Data.LineString(
                drawing.polyline.getPath().getArray()
            )
            f.setGeometry(geometry);
        }
    }

    private checkForRemove(e) {
        const DrawingManager = require('../../GiasJSLibraries/GIS-js-libraries/DrawingManager');

        DrawingManager.checkForRemove(this.giasPolygonDrawings[0].polygon, e, this.googleMapWrapper.googleMap);
    }

    public deleteSubscriptions(): void {
        this.subs.unsubscribe();
    }
}
