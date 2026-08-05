import { Injectable } from '@angular/core';
import { GoogleMapService } from '../google-map/google-map.service';
import { enum_zIndex } from '../GIS-enum/GIS-zIndex';

@Injectable()
export class MeasureDistanceService {

    private firstMarker: google.maps.Marker;
    private secondMarker: google.maps.Marker;
    private polyline: google.maps.Polyline;
    private rulerLabel: any;

    private get GoogleMapWrapper() {
        return this.googleMapService.googleMapWrapper
    }

    constructor(private googleMapService: GoogleMapService) {}

    public startStopMeasure() {
        if(this.firstMarker != undefined || this.secondMarker != undefined) {
            this.destroyMarkers()
        } else {
            let markerOptions: google.maps.MarkerOptions = {};

            markerOptions.clickable = true;
            markerOptions.draggable = true;
            markerOptions.visible = true;
            markerOptions.position = this.GoogleMapWrapper.getCenter();
            markerOptions.map = this.GoogleMapWrapper.googleMap;
            markerOptions.icon = {
                url: 'https://maps.google.com/mapfiles/kml/pushpin/ylw-pushpin.png',
                anchor: new google.maps.Point(11,35),
                scaledSize: new google.maps.Size(36, 36)
            };

            let polylineOptions:  google.maps.PolylineOptions = {};

            polylineOptions.map = this.GoogleMapWrapper.googleMap;
            polylineOptions.clickable = false;
            polylineOptions.draggable = false;
            polylineOptions.editable = false;
            polylineOptions.strokeColor = 'yellow';
            polylineOptions.path = [markerOptions.position];
            polylineOptions.zIndex = enum_zIndex.strumentoMisurazione;

            this.firstMarker = new google.maps.Marker(markerOptions);
            this.secondMarker = new google.maps.Marker(markerOptions);

            this.polyline = new google.maps.Polyline(polylineOptions);

            this.firstMarker.addListener('drag', this.handleDragEvent.bind(this));
            this.secondMarker.addListener('drag', this.handleDragEvent.bind(this));
            const labelsruler = require('../../GiasJSLibraries/GIS-js-libraries/labelsruler');

            this.rulerLabel = new labelsruler.Label({map: this.GoogleMapWrapper.googleMap},10,10);
            this.rulerLabel.bindTo('position', this.firstMarker, 'position');
            this.rulerLabel.set('text', Math.floor(this.measureDistance()));
        }
    }

    private handleDragEvent(): void {
        this.polyline.setPath([this.firstMarker.getPosition(), this.secondMarker.getPosition()]);
        this.rulerLabel.set('text', Math.floor(this.measureDistance()));
    }

    private measureDistance(): number {
        return google.maps.geometry.spherical.computeDistanceBetween(this.firstMarker.getPosition(), this.secondMarker.getPosition());
    }

    private destroyMarkers() {
        this.firstMarker.setMap(null);
        this.secondMarker.setMap(null);
        this.polyline.setMap(null);
        this.rulerLabel.setMap(null);

        this.firstMarker = null;
        this.secondMarker = null;
        this.polyline = null;
        this.rulerLabel = null;
    }
}
