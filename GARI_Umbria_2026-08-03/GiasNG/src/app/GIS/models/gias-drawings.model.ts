import {DrawingService} from '../services/drawing.service';
import {TipologiaLayer} from '../../Service/api.service';

export class GiasBaseDraw{
    id: any;
    drawingService: DrawingService;
    tipologiaLayer: TipologiaLayer;

    constructor(
        drawingService: DrawingService,
        id: number | string
    ) {
        this.drawingService = drawingService;
        this.id = id;
    }
}

export class GiasPolygon extends GiasBaseDraw{
    polygon: google.maps.Polygon;

    constructor(
        drawingService: DrawingService,
        id: number | string,
        polygon: google.maps.Polygon,
        tipo: TipologiaLayer
    ) {
        super(drawingService, id);
        this.polygon = polygon;
        this.tipologiaLayer = tipo;
        this.setListeners();
    }

    private setListeners() {
        //this.polygon.addListener('click', () => this.drawingService.removeEvent.next(this));
        this.polygon.addListener('dragend', () => this.drawingService.updatedDrawEvent.next(this));
        this.polygon.addListener('mouseup', () => this.drawingService.updatedDrawEvent.next(this));
    }
}

export class GiasCircle extends GiasBaseDraw{
    circle: google.maps.Circle;

    constructor(
        drawingService: DrawingService,
        id: number,
        circle: google.maps.Circle
    ) {
        super(drawingService, id);
        this.circle = circle;
        this.setListeners();
    }

    private setListeners() {
        this.circle.addListener('click', () => this.drawingService.removeEvent.next(this));
    }
}

export class GiasRectangle extends GiasBaseDraw{
    rectangle: google.maps.Rectangle;

    constructor(
        drawingService: DrawingService,
        id: number,
        rectangle: google.maps.Rectangle
    ) {
        super(drawingService, id);
        this.rectangle = rectangle;
        this.setListeners();
    }

    private setListeners() {
        this.rectangle.addListener('click', () => this.drawingService.removeEvent.next(this));
    }
}

export class GiasMarker extends GiasBaseDraw{
    marker: google.maps.Marker;

    constructor(
        drawingService: DrawingService,
        id: number,
        marker: google.maps.Marker,
        tipo: TipologiaLayer,
        isMultipoint: boolean = false
    ) {
        super(drawingService, id);
        this.marker = marker;
        if(isMultipoint)
            this.marker.setLabel({text: '' + id, color: 'black'});
        this.tipologiaLayer = tipo;
        this.setListeners();
    }

    private setListeners() {
        // TODO Salvo 03-02-2023: ho commentato la riga sotto perché va in conflitto con l'annulla inserimento nella tabella attributi,
        //  capire come fare e se ha seno reinserire la funzionalità
        // this.marker.addListener('click', () => this.drawingService.removeEvent.next(this));
    }
}

export class GiasPolyline extends GiasBaseDraw{
    polyline: google.maps.Polyline;
    tipologiaLayer: TipologiaLayer;

    constructor(
        drawingService: DrawingService,
        id: number | string,
        polyline: google.maps.Polyline,
        tipo: TipologiaLayer
    ) {
        super(drawingService, id);
        this.polyline = polyline;
        this.tipologiaLayer = tipo;
        this.setListeners();
    }

    private setListeners() {
        //this.polygon.addListener('click', () => this.drawingService.removeEvent.next(this));
        this.polyline.addListener('dragend', () => this.drawingService.updatedDrawEvent.next(this));
        this.polyline.addListener('mouseup', () => this.drawingService.updatedDrawEvent.next(this));
    }
}
