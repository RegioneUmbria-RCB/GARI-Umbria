

var GlobalAgroDrawing;


AgroDrawing.prototype = {

    init: function () {
        let that = this;
        google.maps.event.addListener(this.mappa.drawingManager, 'overlaycomplete', function (e) { that.overlayComplete(that, e); });

        this.mappa.drawingManager.polygonOptions.fillColor = "#F8F8FF";
        this.mappa.drawingManager.polygonOptions.fillOpacity = 0.7;
    },

    overlayComplete: function (that, e) {
        if (e.type != google.maps.drawing.OverlayType.POLYGON) {
            //CIRCLE, MARKER, POLYLINE, RECTANGLE
            return;
        }

        that.creaControllo();

        that.mappa.drawingManager.setDrawingMode(null);
        e.overlay.editable = true;

        that.shape = e.overlay;

        google.maps.event.addListener(that.shape, "click", (e) => {
            // Check if click was on a vertex control point
            if (e.vertex == undefined) {
                return;
            }
            if (that.shape.getPath() === null) {
                return;
            }
            if (!that.checkPath4Remove(that.shape.getPath(), e.vertex)) {
                return;
            }

            let vertex = e.vertex;
            that.deleteMenu.open(that.mappa.elemenotMappa, that.shape.getPath().getAt(vertex), function () {

                that.shape.getPath().removeAt(vertex);
                that.updateInfo();
            });
        });

        google.maps.event.addListener(that.shape.getPath(), 'set_at', function (index) {
            that.updateInfo();
        });

        google.maps.event.addListener(that.shape.getPath(), 'insert_at', function (index) {
            that.updateInfo();
        });

        that.updateInfo();
    },

    checkPath4Remove: function (path, vertex) {
        //controllo se togliendo il vertice rimane un poligono valido...
        let cnt = path.getLength();

        if (cnt < 4) {
            return false;
        }
        if (cnt === 4) {
            return true;
        }

        let tmpPath = new google.maps.MVCArray;
        let idx = 0;
        while (idx < cnt) {

            if (idx !== vertex) {

                tmpPath.push(path.getAt(idx));
            }
            idx++;
        }

        let validator = new PolygonValidator(tmpPath);

        return validator.isValid;
    },

    creaControllo: function () {
        if (this.controlDiv !== null) {
            return;
        }

        this.controlDiv = document.createElement("div");

        let controlUI = document.createElement("div");
        controlUI.style.backgroundColor = "#fff";
        controlUI.style.border = "2px solid #fff";
        controlUI.style.borderRadius = "3px";
        controlUI.style.boxShadow = "0 2px 6px rgba(0,0,0,.3)";
        controlUI.style.marginBottom = "25px";
        this.controlDiv.appendChild(controlUI);

        let controlInfo = document.createElement("div");
        controlInfo.style.padding = "5px";
        controlInfo.style.marginBottom = "5px";
        controlInfo.style.borderRadius = "4px";
        controlInfo.style.backgroundColor = "#0059B3";
        controlInfo.style.color = "#fff";
        controlInfo.style.textAlign = "center";
        controlInfo.style.fontWeight = "bold";
        controlInfo.style.fontSize = "14px";
        let infoText = document.createElement("div");
        infoText.id = "control-info-text";
        controlInfo.appendChild(infoText);
        controlUI.appendChild(controlInfo);

        let controlDDL = document.createElement("div");
        controlDDL.id = "control-ddl-layer";
        controlDDL.style.marginBottom = "5px";
        controlDDL.style.width = "-webkit-fill-available";
        controlDDL.style.fontSize = "13px";
        controlUI.appendChild(controlDDL);

        let controlBtns = document.createElement("div");
        controlBtns.style.cssText = "display: grid; grid-template-columns: 1fr 1fr; grid-gap: 5px;";
        controlUI.appendChild(controlBtns);

        let controlSave = document.createElement("div");
        controlSave.className = "k-button";
        let saveIcon = document.createElement("span");
        saveIcon.className = "fa fa-check fa-lg";
        saveIcon.style.color = "#008000";
        let saveText = document.createElement("span");
        saveText.innerHTML = "Salva";
        controlSave.appendChild(saveIcon);
        controlSave.appendChild(saveText);
        controlBtns.appendChild(controlSave);

        let controlCancel = document.createElement("div");
        controlCancel.className = "k-button";
        let cancelIcon = document.createElement("span");
        cancelIcon.className = "fa fa-close fa-lg";
        cancelIcon.style.color = "#B30000";
        let cancelText = document.createElement("span");
        cancelText.innerHTML = "Annulla";
        controlCancel.appendChild(cancelIcon);
        controlCancel.appendChild(cancelText);
        controlBtns.appendChild(controlCancel);

        let that = this;

        controlSave.addEventListener("click", () => {

            if (!that.validatePolygon()) {

                return;
            }

            let layer = $("#control-ddl-layer").getKendoDropDownList().value();

            that.controlDiv.remove();
            that.controlDiv = null;

            that.shape.setEditable(false);

            if (that.saveCallback !== undefined) {

                that.saveCallback(that.shape, layer);

            } else {

                that.shape.setMap(null);
            }

        });

        controlCancel.addEventListener("click", () => {
            that.controlDiv.remove();
            that.controlDiv = null;

            that.shape.setEditable(false);
            that.shape.setMap(null);
        });

        this.mappa.elemenotMappa.controls[google.maps.ControlPosition.BOTTOM_CENTER].push(this.controlDiv);

        $("#control-ddl-layer").kendoDropDownList({
            dataTextField: "text",
            dataValueField: "value",
            dataSource: that.layers,
            index: 0
        });
    },

    validatePolygon: function () {

        let path = this.shape.getPath();
        let area = google.maps.geometry.spherical.computeSignedArea(path);
        if (area < 0) {
            let idx = 0;
            let cnt = Math.floor(path.getLength() / 2);
            while (idx < cnt) {
                let tmp = path.getAt(idx);
                path.setAt(idx, path.getAt(path.length - 1 - idx));
                path.setAt(path.length - 1 - idx, tmp);
                idx++;
            }
        }

        let validator = new PolygonValidator(path);

        if (!validator.isValid) {

            if (validator.intersection) {

                let edge1Path = new google.maps.Polyline({
                    path: validator.intersection.edge1,
                    map: this.mappa.elemenotMappa,
                    strokeColor: "#DC143C",
                    strokeOpacity: 0,
                    strokeWeight: 3,
                });

                let edge2Path = new google.maps.Polyline({
                    path: validator.intersection.edge2,
                    map: this.mappa.elemenotMappa,
                    strokeColor: "#DC143C",
                    strokeOpacity: 0,
                    strokeWeight: 3,
                });

                let deg = 0;

                let blink = window.setInterval(() => {

                    let opacity = 0.5 + Math.cos((180 + deg) * Math.PI / 180.0) * 0.5;

                    deg += 10;

                    edge1Path.setOptions({ strokeOpacity: opacity });
                    edge2Path.setOptions({ strokeOpacity: opacity });

                    if (deg >= 720) {

                        clearInterval(blink);

                        edge1Path.setMap(null);
                        edge2Path.setMap(null);

                        edge1Path = null;
                        edge2Path = null;
                    }

                }, 15);

            } else {

                let shape = this.shape;

                let clrs = [shape.fillColor, "#B22222"];
                let clr = 0;

                let blink = window.setInterval(() => {

                    clr++;
                    shape.setOptions({ fillColor: clrs[clr % 2] });

                    if (clr >= 4) {
                        clearInterval(blink);
                    }

                }, 150);

            }

            return false;
        }

        return true;
    },

    updateInfo: function () {

        let area_mq = Math.abs(google.maps.geometry.spherical.computeSignedArea(this.shape.getPath()));

        $(this.controlDiv).find("#control-info-text").text("Area: " + kendo.toString(area_mq * 0.0001, "0.0000") + " ha");
    },

};

function AgroDrawing(mappa, layers, saveCallback) {
    if (!(this instanceof AgroDrawing)) return new AgroDrawing();

    this.mappa = mappa;
    this.layers = layers;
    this.saveCallback = saveCallback;
    this.shape = null;
    this.deleteMenu = new DeleteMenu();
    this.controlDiv = null;
    this.init();
}

class DeleteMenu extends google.maps.OverlayView {
    constructor() {
        super();

        this.div_ = document.createElement("div");
        this.div_.style.cssText = "position:absolute; margin-top:-0.7em; margin-left:9px; font-size:20px; height:1.4em;";

        let container = document.createElement("div");
        container.style.cssText = "position:relative; height:100%;";

        this.clickable_ = document.createElement("div");
        this.clickable_.id = "gis-vertex-delete-menu-content";
        this.clickable_.style.cssText = "margin-left: 6px; background-color:#f5f5f5; color:#b40000; padding:0em 0.6em; border-radius:2px; box-shadow:rgb(0 0 0 / 30%) 1px 3px 3px; cursor:pointer; height:100%; display:flex; align-items:center;";

        let icon = document.createElement("span");
        icon.style.cssText = "pointer-events:none;"
        icon.className = "fa fa-times";

        let callout = document.createElement("div");
        callout.id = "gis-vertex-delete-menu-callout";
        callout.style.cssText = "position:absolute; border-width:6px; border-style:solid; border-color:transparent; border-right-color:#f5f5f5; margin-left:-6px; top:50%; transform:translateY(-50%);";

        this.div_.appendChild(container);
        container.appendChild(this.clickable_);
        this.clickable_.appendChild(icon);
        container.appendChild(callout);

        let style = document.createElement("style");
        style.type = "text/css";
        let css = "#gis-vertex-delete-menu-content:hover { background-color: #e1e1e1 !important; }";
        css += "#gis-vertex-delete-menu-content:hover + #gis-vertex-delete-menu-callout { border-right-color: #e1e1e1 !important; }";
        style.innerHTML = css;
        this.div_.appendChild(style);

        const deleteMenu = this;

        google.maps.event.addDomListener(this.clickable_, "click", () => {
            let callback = deleteMenu.get("callback");
            if (callback !== null) {
                callback();
            }
            deleteMenu.close();
        });
    }

    onAdd() {
        const deleteMenu = this;
        const map = this.getMap();

        this.getPanes().floatPane.appendChild(this.div_);

        // mousedown anywhere on the map except on the menu div will close the menu.
        this.divListener_ = google.maps.event.addDomListener(map.getDiv(),
            "mousedown",
            (e) => {
                if (e.target != deleteMenu.clickable_) {
                    deleteMenu.close();
                }
            },
            true
        );
    }
    onRemove() {
        if (this.divListener_) {
            google.maps.event.removeListener(this.divListener_);
        }
        this.div_.parentNode.removeChild(this.div_);
        // clean up
        this.set("position", null);
        this.set("callback", null);
    }
    close() {
        this.setMap(null);
    }
    draw() {
        const position = this.get("position");
        const projection = this.getProjection();

        if (!position || !projection) {
            return;
        }
        const point = projection.fromLatLngToDivPixel(position);
        this.div_.style.top = point.y + "px";
        this.div_.style.left = point.x + "px";
    }
    open(map, position, callback) {
        this.set("position", position);
        this.set("callback", callback);
        this.setMap(map);
        this.draw();
    }
}

class PolygonValidator {

    constructor(polygon) {

        this.polygon = polygon;
        this.intersection = null;
    }

    get isValid() {
        //Controllo se esiste una intersezione tra due lati del poligono

        let nVert = this.polygon.getLength();

        if (nVert === 3)
            return true;

        let _intersect = false;
        let v = 0;
        let lastw = nVert - 1;
        //Due lati adiacenti del poligono non possono intersecarsi

        while (!_intersect && v < nVert - 2) {

            // v-esimo lato (p1, p2)
            let p1 = this.polygon.getAt(v);
            let p2 = this.polygon.getAt(v + 1)

            let w = v + 2;
            while (!_intersect && w < lastw) {

                //w-esimo lato (q1, q2)
                let q1 = this.polygon.getAt(w);
                let q2 = this.polygon.getAt((w + 1) % nVert);

                _intersect = this.lineSegmentsIntersect(p1, p2, q1, q2);

                if (_intersect) {

                    this.intersection = {
                        edge1: [p1, p2],
                        edge2: [q1, q2]
                    }
                }

                w++;
            }

            v++;
            lastw = nVert;
        }

        return !_intersect;
    }

    //See if two line segments intersect. This uses the vector cross product approach described below:
    //http://stackoverflow.com/a/565282/78639

    lineSegmentsIntersect(p1, p2, q1, q2) {

        let r = new google.maps.LatLng(p2.lat() - p1.lat(), p2.lng() - p1.lng());
        let s = new google.maps.LatLng(q2.lat() - q1.lat(), q2.lng() - q1.lng());
        let qp = new google.maps.LatLng(q1.lat() - p1.lat(), q1.lng() - p1.lng());

        let uNumerator = qp.lng() * r.lat() - qp.lat() * r.lng(); //cross product
        let denominator = r.lng() * s.lat() - r.lat() * s.lng(); //cross product

        if (denominator == 0) {

            if (uNumerator == 0) {

                // Collinears, do they overlap?

                let min_lng = Math.min(p1.lng(), p2.lng());
                let max_lng = Math.max(p1.lng(), p2.lng());
                let min_lat = Math.min(p1.lat(), p2.lat());
                let max_lat = Math.max(p1.lat(), p2.lat());

                //q1 in (p1, p2) or q2 in (p1, p2)
                return ((min_lng <= q1.lng() && q1.lng() <= max_lng && min_lat <= q1.lat() && q1.lat() <= max_lat) ||
                    (min_lng <= q2.lng() && q2.lng() <= max_lng && min_lat <= q2.lat() && q2.lat() <= max_lat));
            }

            // Parallels
            return false;
        }

        let u = uNumerator / denominator;
        let t = (qp.lng() * s.lat() - qp.lat() * s.lng()) / denominator;

        return (0 <= t) && (t <= 1) && (0 <= u) && (u <= 1);
    }
}

function DisplayErrorPolyLines(intersection, mappa) {

    let edge1Path = new google.maps.Polyline({
        path: intersection.edge1,
        map: mappa,
        strokeColor: "#DC143C",
        strokeOpacity: 0,
        strokeWeight: 3,
    });

    let edge2Path = new google.maps.Polyline({
        path: intersection.edge2,
        map: mappa,
        strokeColor: "#DC143C",
        strokeOpacity: 0,
        strokeWeight: 3,
    });

    let deg = 0;

    let blink = window.setInterval(() => {

        let opacity = 0.5 + Math.cos((180 + deg) * Math.PI / 180.0) * 0.5;

        deg += 10;

        edge1Path.setOptions({ strokeOpacity: opacity });
        edge2Path.setOptions({ strokeOpacity: opacity });

        if (deg >= 720) {

            clearInterval(blink);

            edge1Path.setMap(null);
            edge2Path.setMap(null);

            edge1Path = null;
            edge2Path = null;
        }

    }, 15);
}

function checkPath4Remove(path, vertex) {
    //controllo se togliendo il vertice rimane un poligono valido...
    let cnt = path.getLength();

    if (cnt < 4) {
        return false;
    }
    if (cnt === 4) {
        return true;
    }

    let tmpPath = new google.maps.MVCArray;
    let idx = 0;
    while (idx < cnt) {
        if (idx !== vertex) {
            tmpPath.push(path.getAt(idx));
        }
        idx++;
    }

    let validator = new PolygonValidator(tmpPath);

    return validator.isValid;
}

function checkForRemove (shape, e, mappa) {
    // Check if click was on a vertex control point
    if (e.vertex == undefined) {
        return;
    }
    if (shape.getPath() === null) {
        return;
    }
    if (!checkPath4Remove(shape.getPath(), e.vertex)) {
        return;
    }

    let vertex = e.vertex;

    let deleteMenu = new DeleteMenu();
    //mappa.elementoMappa;
    deleteMenu.open(mappa, shape.getPath().getAt(vertex), function () {

        shape.getPath().removeAt(vertex);
        //that.updateInfo();
    });
}

export  { PolygonValidator, DisplayErrorPolyLines, checkForRemove };
