


(function ($) {

    $.geoPosEdit = function (elem, opts) {

        var plugin = this;

        plugin.$element = $(elem);
        plugin.element = elem;

        var _changeCallback = function () { };
        if (typeof opts.change === "function") {
            _changeCallback = opts.change;
        }

        var _mapAction = opts.mapAction;

        let css = ".__geoPosWrapper { position: relative; border-radius: 4px; border: 1px solid #ccc; overflow: hidden; display: grid; grid-template-columns: auto 1fr auto; } ";
        css += ".__geoPosToggle { margin: 0; color: #333; } ";
        css += ".__geoPosToggle > input { display: none; } ";
        css += ".__geoPosToggleLbl { transition: .1s linear; } ";
        css += ".__geoPosToggleArrow { font-size: 18px; margin: 0; transition: .1s linear; opacity: 0.5; } ";
        css += ".__geoPosToggle > input:not(:checked) ~ .__geoPosToggleLbl.on, .__geoPosToggle > input:checked ~ .__geoPosToggleLbl.off { font-size: 8px; opacity: 0.3; } ";
        css += ".__geoPosToggle > input:not(:checked) ~ .__geoPosToggleArrow { transform: rotate(-180deg); } ";
        css += ".__geoPosInputWrapper { border-left: 1px solid #ccc; border-right: 1px solid #ccc; display: flex; } ";
        css += ".__geoPosInputWrapper > *:not(:first-child) { border-left: 1px solid #ccc; }";
        css += ".__geoPosInput { border: none; box-shadow: none !important; flex-grow: 1; } ";
        css += ".__geoPosInput.k-textbox { border: none; text-align: center; } ";
        css += ".__geoPosBtn { font-size: 14px; width: 5.5em; cursor: pointer; user-select: none; display: flex; align-items: center; justify-content: center; } ";
        css += ".__geoPosBtn:hover { background-color: #ebebeb; } ";
        css += ".__geoPosBtn:active { background-color: #ebebeb; box-shadow: inset 0 3px 5px rgb(0 0 0 / 13%); } ";
        css += "::placeholder { color: #dcdcdc; opacity: 1; } ";
        css += ":-ms-input-placeholder { color: #dcdcdc; } ";
        css += "::-ms-input-placeholder { color: #dcdcdc; } ";
        css += ".__geoPosHidden { display: none !important; } ";
        css += ".__geoPosLoader { ";
        css += "position: absolute; bottom: 0px; left: 0; width: 100%; height: 2px; background-color: #1274ac; transform: translateX(0%); z-index: 1000; ";
        css += "animation: geopos-loader 1500ms infinite; -moz-animation: geopos-loader 1500ms infinite; -webkit-animation: geopos-loader 1500ms infinite; ";
        css += "} ";
        css += "@keyframes geopos-loader { 0% { transform: translateX(-100%); } 100% { transform: translateX(100%); } } ";
        css += "@-moz-keyframes geopos-loader { 0% { transform: translateX(-100%); } 100% { transform: translateX(100%); } } ";
        css += "@-webkit-keyframes geopos-loader { 0% { transform: translateX(-100%); } 100% { transform: translateX(100%); } } ";

        let toggle_style = document.createElement("style");
        toggle_style.type = "text/css";
        toggle_style.innerHTML = css;
        plugin.element.appendChild(toggle_style);

        let wrapper = document.createElement("div");
        wrapper.className = "__geoPosWrapper";
        plugin.element.appendChild(wrapper);

        let label = document.createElement("label");
        label.className = "__geoPosToggle __geoPosBtn";
        wrapper.appendChild(label);

        let toggle = document.createElement("input");
        toggle.type = "checkbox";
        label.appendChild(toggle);

        let spanDD = document.createElement("span");
        spanDD.className = "__geoPosToggleLbl on";
        spanDD.innerText = "DD";
        label.appendChild(spanDD);

        let spanArrow = document.createElement("span");
        spanArrow.className = "__geoPosToggleArrow k-icon k-i-arrow-right";
        label.appendChild(spanArrow);

        let spanDMS = document.createElement("span");
        spanDMS.className = "__geoPosToggleLbl off";
        spanDMS.innerText = "DMS";
        label.appendChild(spanDMS);

        let inputWrapper = document.createElement("div");
        inputWrapper.className = "__geoPosInputWrapper";
        wrapper.appendChild(inputWrapper);

        let inputLat = document.createElement("input");
        inputLat.className = "__geoPosInput";
        inputLat.type = "text";

        let inputLng = document.createElement("input");
        inputLng.className = "__geoPosInput";
        inputLng.type = "text";

        inputWrapper.appendChild(inputLat);
        inputWrapper.appendChild(inputLng);

        let divMap = document.createElement("div");
        divMap.className = "__geoPosBtn";
        divMap.innerHTML = "<span class='fa fa-globe fa-lg fa-fw'></span>";
        wrapper.appendChild(divMap);

        let divLoader = document.createElement("div");
        divLoader.className = "__geoPosLoader __geoPosHidden";
        wrapper.appendChild(divLoader);

        var _change = function () {

            let pos = plugin.getLatLngDec();

            if (pos === null) {

                return;
            }

            $(".__geoPosLoader").removeClass("__geoPosHidden");

            setTimeout(function () {

                _changeCallback(pos);

                $(".__geoPosLoader").addClass("__geoPosHidden");

            }, 100);
            
        };

        $(inputLat).kendoCoordMaskedTextBox({ coord: "Lat", change: _change });
        $(inputLng).kendoCoordMaskedTextBox({ coord: "Lng", change: _change });

        plugin.inputLat = $(inputLat).data("kendoCoordMaskedTextBox");
        plugin.inputLng = $(inputLng).data("kendoCoordMaskedTextBox");

        toggle.checked = !plugin.inputLat.formatoDMS();

        toggle.onclick = function (ev) {
            let dms = !ev.currentTarget.checked;
            plugin.inputLat.formatoDMS(dms);
            plugin.inputLng.formatoDMS(dms);
        };

        plugin.setLatLngDec = function (lat, lng) {
            plugin.inputLat.decimalValue(lat);
            plugin.inputLng.decimalValue(lng);
        };

        plugin.getLatLngDec = function () {
            let lat = plugin.inputLat.decimalValue();
            let lng = plugin.inputLng.decimalValue();
            if (isNaN(lat) || isNaN(lng)) {
                return null;
            }
            return { lat: lat, lng: lng };
        };

        plugin.clear = function () {
            plugin.inputLat.value("");
            plugin.inputLng.value("");
        };

        divMap.onclick = function () {

            if (typeof _mapAction === "function") {

                _mapAction(plugin);
                return;
            }

            openMapWnd(plugin.getLatLngDec(), function (pos) {
                plugin.setLatLngDec(pos.lat(), pos.lng());
                _change();
            });
        };


        $(inputLat).bind("paste", function (e) {
            e.preventDefault();
            e.stopPropagation();
            _paste("lat", e.originalEvent.clipboardData);
            return false;
        });

        $(inputLng).bind("paste", function (e) {
            e.preventDefault();
            e.stopPropagation();
            _paste("lng", e.originalEvent.clipboardData);
            return false;
        });


        var _paste = function (who, clipboardData) {

            if (!clipboardData.types.includes("text/plain")) {

                return;
            }

            let pastedData = clipboardData.getData('text/plain').trim();

            let regEx = /^[+-]?(\d*\.)?\d+((\s*)?[\,\s](\s*)?[+-]?(\d*\.)?\d+)?$/;

            if (pastedData.match(regEx)) {

                // 1 o 2 valori separati da virgola o spazi...

                let arrValues;

                if (pastedData.includes(",")){

                    arrValues = pastedData.split(",").map(x => x.trim());

                } else {

                    arrValues = pastedData.split(" ").filter(x => x);
                }

                if (arrValues.length === 1) {

                    let val = parseFloat(arrValues[0]);

                    if (!isNaN(val)) {

                        switch (who) {

                            case "lat":
                                plugin.inputLat.decimalValue(val);
                                _change();
                                break;

                            case "lng":
                                plugin.inputLng.decimalValue(val);
                                _change();
                                break;
                        }
                    }

                } else {

                    let lat = parseFloat(arrValues[0]);
                    let lng = parseFloat(arrValues[1]);

                    if (!isNaN(lat) && !isNaN(lng)) {

                        plugin.inputLat.decimalValue(lat);
                        plugin.inputLng.decimalValue(lng);
                        _change();
                    }
                }
            }
        }

    }; // geoPosEdit

    //Add the plugin to the jQuery.fn object
    $.fn.geoPosEdit = function (opts) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('geoPosEdit')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.geoPosEdit(this, opts);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('geoPosEdit', plugin);
            }
        });
    };

})(jQuery);



(function ($) {
    var CoordMaskedTextBox = kendo.ui.MaskedTextBox.extend({
        options: {
            name: "CoordMaskedTextBox",
            coord: "",
            formatoDMS: false
        },
        init: function (element, options) {

            options.mask = "~";
            options.rules = {
                "~": /[+-]/
            };

            // The base call to the widget initialization.
            kendo.ui.MaskedTextBox.fn.init.call(this, element, options);

            this._gestisciFormato(this.options.formatoDMS);

            $(element).keypress(function (e) {
                let tb = $(this).data("kendoCoordMaskedTextBox");
                let raw = tb.raw();
                if (raw === "" || this.selectionStart === 0) {

                    if ('0' <= e.key && e.key <= '9') {

                        tb.value("+" + e.key);

                        e.preventDefault();
                    }
                }
            });
        },
        _gestisciFormato: function (formatoDMS) {

            let mask = "~00°00'00\"";
            let placeholder = "±DD°MM'SS\"";
            if (!formatoDMS) {
                mask = "~00.0000000°";
                placeholder = "±DD.DDDDDDD°";
            }

            this.element.attr("placeholder", this.options.coord + " " + placeholder);
            this.setOptions({ mask: mask, formatoDMS: formatoDMS });
        },
        _gestisciValue: function (decVal, formatoDMS) {

            if (isNaN(decVal)) {
                return "";
            }

            let val = ""
            if (!formatoDMS) {

                val = kendo.toString(decVal, "00.0000000000");

            } else {

                let max_deg = 90;
                if (this.options.coord === "Lng") {
                    max_deg = 180;
                }

                let sign = decVal < 0 ? -1 : 1;
                let absVal = Math.abs(Math.round(decVal * 1000000));
                if (absVal <= (max_deg * 1000000)) {

                    let dec = absVal % 1000000 / 1000000;
                    let deg = Math.floor(absVal / 1000000) * sign;
                    let min = Math.floor(dec * 60);
                    let sec = (dec - min / 60) * 3600;

                    val = kendo.toString(deg, "00") + "" + kendo.toString(min, "00") + "" + kendo.toString(sec, "00");
                }
            }

            if (val.length > 0 && val.charAt(0) !== "-") {
                val = "+" + val;
            }

            return val;
        },
        formatoDMS: function (dms) {

            if (dms === undefined) {

                return this.options.formatoDMS;
            }

            dms = !!dms;

            if (this.options.formatoDMS !== dms) {

                let val = this._gestisciValue(this.decimalValue(), dms);
                this._gestisciFormato(dms);
                this.value(val);
            }

            return this.options.formatoDMS;
        },
        decimalValue: function (val) {

            if (val === undefined) {

                val = Number.NaN;

                if (!this.options.formatoDMS) {

                    let v = this.value();
                    if (v.length > 0) {
                        val = parseFloat(v.replace("°", "").replace(this.options.promptChar, "").replace(",", "."));
                    }

                } else {

                    let raw = this.raw();
                    if (raw.length === 7) { //+DDMMSS

                        let deg = parseFloat(raw.slice(0, 3));
                        let min = parseFloat(raw.slice(3, 5));
                        let sec = parseFloat(raw.slice(5, 7));

                        if (!isNaN(deg) && !isNaN(min) && !isNaN(sec)) {

                            let sign = deg < 0 ? -1 : 1;
                            let abs = Math.abs(deg);
                            val = sign * (abs + (min / 60.0) + (sec / 3600));
                        }
                    }
                }

                return val;
            }

            this.value(this._gestisciValue(val, this.options.formatoDMS));
        }
    });
    kendo.ui.plugin(CoordMaskedTextBox);
})(jQuery);



function openMapWnd(geopos, callbackAccept) {

    let win_el = document.createElement("div");
    document.body.appendChild(win_el);
    let $win_el = $(win_el);

    let mapH = Math.round(window.outerHeight * 0.8);

    let map_el = document.createElement("div");
    map_el.style.cssText = "width:100%; height: " + mapH + "px;";
    map_el.id = "___mapContainer";
    win_el.appendChild(map_el);

    $win_el.kendoDialog({
        width: "90%",
        title: "",
        closable: true,
        modal: true,
        visible: false,
        show: function () {

            let that = this;
            $("#___mapContainer").geoPosMap({
                geopos: geopos,
                callbackAccept: function (pos) {
                    callbackAccept(pos);
                    that.close();
                }
            });
        },
        close: function () {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}



(function ($) {

    $.geoPosMap = function (elem, opts) {

        var _callbackAccept = function () { };
        if (typeof opts.callbackAccept === "function") {
            _callbackAccept = opts.callbackAccept;
        }

        var _gmap = CreateMap(elem);

        var _marker = null;
        var _mapElemLat = null;
        var _mapElemLng = null;

        var _dragCallback = function () {

            if (_marker === null || _mapElemLat === null || _mapElemLng === null) {
                return;
            }

            let ll = _marker.getPosition();

            _mapElemLat.innerText = kendo.toString(ll.lat(), "0.0000000°");
            _mapElemLng.innerText = kendo.toString(ll.lng(), "0.0000000°");
        };

        var _addMarker = function (ll) {

            if (_marker !== null) {
                return;
            }

            _marker = DraggableMarker(ll);
            _marker.setMap(_gmap);
            _marker.setDraggable(true);
            _marker.setZIndex(1000);

            _marker.addListener("drag", (event) => {
                _dragCallback();
            });

            _marker.addListener("dragend", (event) => {
                _dragCallback();
            });

            _dragCallback();

            google.maps.event.addListener(_gmap, "click", (event) => {
                _marker.setPosition(event.latLng);
                _dragCallback();
            });
        };

        const controlDiv = document.createElement("div");
        controlDiv.style.padding = "0px 5px 5px";
        controlDiv.style.borderBottomLeftRadius = "4px";
        controlDiv.style.borderBottomRightRadius = "4px";
        controlDiv.style.backgroundColor = "#fff";
        controlDiv.style.display = "flex";
        controlDiv.style.columnGap = "5px";
        controlDiv.style.userSelect = "none";
        controlDiv.style.fontSize = "16px";

        let style = document.createElement("style");
        style.type = "text/css";
        style.innerHTML = ".k-button-solid-base { box-shadow: none !important; }";
        controlDiv.appendChild(style);

        const divCoord = document.createElement("div");
        divCoord.className = "k-block";
        divCoord.style.height = "2.1em";
        divCoord.style.textAlign = "right";
        divCoord.style.display = "grid";
        divCoord.style.gridTemplateColumns = "8em 8em";
        divCoord.style.gridGap = "10px";
        divCoord.style.padding = "6px";
        controlDiv.appendChild(divCoord);

        const acceptBtn = document.createElement("div");
        acceptBtn.style.fontSize = "20px";
        acceptBtn.style.color = "#008000";
        acceptBtn.style.padding = "0px";
        acceptBtn.style.height = "auto";
        controlDiv.appendChild(acceptBtn);

        $(acceptBtn).kendoButton({
            iconClass: "fa fa-check fa-fw",
            click: function () {
                if (_marker === null) {
                    return;
                }

                this.enable(false);

                this.element.find(".k-loader").toggleClass("meteo-hidden");
                this.element.find(".k-button-icon").toggleClass("meteo-hidden");

                let that = this;
                setTimeout(function () {

                    _callbackAccept(_marker.getPosition());

                    that.element.find(".k-loader").toggleClass("meteo-hidden");
                    that.element.find(".k-button-icon").toggleClass("meteo-hidden");

                    that.enable(true);
                }, 100);
            }
        });

        const divLoader = document.createElement("div");
        divLoader.className = "meteo-hidden";
        $(acceptBtn).getKendoButton().element[0].appendChild(divLoader);
        $(divLoader).kendoLoader({ size: "small" });

        var _coordElem = function (parent, lbl) {

            let div = document.createElement("div");
            div.style.display = "flex";
            parent.appendChild(div);

            let outElem = document.createElement("span");
            outElem.style.flexGrow = "1";
            div.appendChild(outElem);

            let lbl_span = document.createElement("span");
            lbl_span.style.fontSize = "10px";
            lbl_span.style.marginLeft = "3px";
            lbl_span.style.alignSelf = "flex-end";
            lbl_span.style.color = "#aeaeae";
            lbl_span.innerText = lbl;
            div.appendChild(lbl_span);

            return outElem;
        };

        _mapElemLat = _coordElem(divCoord, "Lat");
        _mapElemLng = _coordElem(divCoord, "Lng");

        _gmap.controls[google.maps.ControlPosition.TOP_CENTER].push(controlDiv);

        let ll = null;

        if (typeof opts.geopos === "object" && opts.geopos !== null) {

            if (opts.geopos.lat && opts.geopos.lng) {

                ll = new google.maps.LatLng(opts.geopos.lat, opts.geopos.lng);
            }
        }

        if (ll === null) {

            google.maps.event.addListenerOnce(_gmap, "click", (event) => {
                _addMarker(event.latLng);
            });

        } else {

            let mapBounds = new google.maps.LatLngBounds();
            mapBounds.extend(ll);

            _gmap.setCenter(mapBounds.getCenter());
            _gmap.fitBounds(mapBounds);

            google.maps.event.addListenerOnce(_gmap, "tilesloaded", function () {
                let zoom = _gmap.getZoom();
                if (zoom > 16) {
                    _gmap.setZoom(16);
                }
            });

            _addMarker(ll);
        }

    }; // geoPosMap

    //Add the plugin to the jQuery.fn object
    $.fn.geoPosMap = function (opts) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('geoPosMap')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.geoPosMap(this, opts);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('geoPosMap', plugin);
            }
        });
    };

})(jQuery);



function CreateMap(elem)
{
    let gmap = new google.maps.Map(elem, {
        zoomControl: true,
        zoomControlOptions: {
            position: google.maps.ControlPosition.RIGHT_CENTER,
        },
        streetViewControl: false,
        fullscreenControl: false,
        mapTypeControl: false,
        rotateControl: false,
        mapTypeId: google.maps.MapTypeId.HYBRID,
        tilt: 0,
        zoom: 6,
        center: new google.maps.LatLng(41.90, 12.49),
        styles: [
            {
                featureType: "poi",
                elementType: "labels",
                stylers: [
                    {
                        visibility: "off"
                    }
                ]
            },
            {
                featureType: "transit",
                elementType: "labels",
                stylers: [
                    {
                        visibility: "off"
                    }
                ]
            }
        ]
    });

    return gmap;
}



function DraggableMarker(lat_lng)
{
    let w = 28;
    let h = 40;
    let center = w / 2;
    let outerRadius = (w - 2) / 2;

    let canvas = document.createElement("canvas");
    canvas.width = w;
    canvas.height = h;

    let ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, w, h);

    //ctx.fillStyle = "#4682B4"; //SteelBlue
    //ctx.fillStyle = "#FF8C00"; //DarkOrange
    //ctx.fillStyle = "#708090"; //SlateGray
    ctx.fillStyle = "#FFD700"; //Gold

    //ctx.strokeStyle = "#F5F5F5";
    ctx.strokeStyle = "#696969";

    ctx.beginPath();
    ctx.arc(center, center, outerRadius, 0, Math.PI, true);
    let cp1_y = center + 7;
    let cp2_y = center + 12;
    ctx.bezierCurveTo(1, cp1_y, center - 2, cp2_y, w / 2 - 1, h - 1)
    ctx.lineTo(w / 2 + 1, h - 1);
    ctx.bezierCurveTo(center + 2, cp2_y, w - 1, cp1_y, w - 1, center)
    ctx.closePath();
    ctx.fill();
    ctx.stroke();

    ctx.beginPath();
    ctx.fillStyle = "#FFFFE0";
    //ctx.arc(center, center, innerRadius + 2.5, 0, Math.PI * 2, true);

    let oR = outerRadius - 4;
    let iR = outerRadius - 8;
    let rot = Math.PI / 5;

    ctx.save();
    ctx.beginPath();
    ctx.translate(center, center);
    ctx.moveTo(0, -oR);
    for (let s = 0; s < 5; s++) {
        ctx.rotate(rot);
        ctx.lineTo(0, -iR);
        ctx.rotate(rot);
        ctx.lineTo(0, -oR);
    }
    ctx.closePath();
    ctx.fill();
    ctx.restore();

    return new google.maps.Marker({
        position: lat_lng,
        icon: {
            url: canvas.toDataURL(),
            labelOrigin: new google.maps.Point(w / 2, h + 7)
        }
    });
}



function StaticMarker(lat_lng)
{
    let w = 22;
    let h = 30;
    let center = w / 2;
    let outerRadius = (w - 2) / 2;
    let innerRadius = 4;

    let canvas = document.createElement("canvas");
    canvas.width = w;
    canvas.height = h;

    let ctx = canvas.getContext("2d");
    ctx.clearRect(0, 0, w, h);

    //ctx.fillStyle = "#4682B4"; //SteelBlue
    //ctx.fillStyle = "#FF8C00"; //DarkOrange
    //ctx.fillStyle = "#708090"; //SlateGray
    ctx.fillStyle = "#FFD700"; //Gold

    //ctx.strokeStyle = "#F5F5F5";
    ctx.strokeStyle = "#696969";

    ctx.beginPath();
    ctx.arc(center, center, outerRadius, 0, Math.PI, true);
    let cp1_y = center + 7;
    let cp2_y = center + 12;
    ctx.bezierCurveTo(1, cp1_y, center - 2, cp2_y, w / 2 - 1, h - 1)
    ctx.lineTo(w / 2 + 1, h - 1);
    ctx.bezierCurveTo(center + 2, cp2_y, w - 1, cp1_y, w - 1, center)
    ctx.closePath();
    ctx.fill();
    ctx.stroke();

    ctx.globalCompositeOperation = 'destination-out';

    ctx.beginPath();
    ctx.arc(center, center, innerRadius, 0, Math.PI * 2, true);
    ctx.closePath();
    ctx.fill();

    ctx.globalCompositeOperation = 'source-over';

    ctx.beginPath();
    ctx.arc(center, center, innerRadius, 0, Math.PI * 2, true);
    ctx.closePath();
    ctx.stroke();

    //ctx.fillStyle = "#696969";
    //ctx.font = '17px FontAwesome';
    //ctx.textAlign = "center";
    //ctx.textBaseline = "middle";
    ////ctx.fillText("\uF005", center, center);
    //ctx.fillText("\uF006", center, center);

    //return {
    //    url: canvas.toDataURL(),
    //    labelOrigin: { x: w / 2, y: h + 7 }
    //}

    return new google.maps.Marker({
        position: lat_lng,
        icon: {
            url: canvas.toDataURL(),
            labelOrigin: new google.maps.Point(w / 2, h + 7)
        }
    });
}

