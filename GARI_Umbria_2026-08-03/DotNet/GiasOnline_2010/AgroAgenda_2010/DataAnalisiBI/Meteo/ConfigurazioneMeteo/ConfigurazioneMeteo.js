
(function ($) {
    var ResizableTabStrip = kendo.ui.TabStrip.extend({
        options: {
            name: "ResizableTabStrip",
            outerH: 0
        },
        init: function (element, options) {
            // The base call to the widget initialization.
            kendo.ui.TabStrip.fn.init.call(this, element, options);

            $(this.element).css({ "background-color": "transparent", "box-shadow": "none" });

            let endWhile = false;
            let iElem = 0;
            while (!endWhile) {
                let elem = this.contentElement(iElem);
                iElem++;
                if (undefined === elem) {
                    endWhile = true;
                } else {
                    elem.style.outline = "none";
                }
            }
        },
        fitHeight: function (h) {

            if (this.options.outerH === 0) {
                let this_h = $(this.element).height();
                let cont_h = $(this.element.find(".k-content")[0]).height();
                this.options.outerH = $(this.element).position().top + Math.max(0, this_h - cont_h);
            }

            let contH = Math.round(h - this.options.outerH);
            this.element.find(".k-content").each(function (i, e) {
                $(e).height(contH);
            });

            return contH;
        }
    });
    kendo.ui.plugin(ResizableTabStrip);
})(jQuery);



function yesno_dlg(title, content, fun_yes, fun_no) {

    let win_el = document.createElement("div");
    win_el.id = "tmp-kendo-dlg";
    document.body.appendChild(win_el);
    let $win_el = $("#" + win_el.id);

    $win_el.kendoDialog({
        title: title,
        closable: false,
        modal: true,
        visible: false,
        content: content,
        actions: [
            {
                text: TraduzioneMultiResx(datiMeteoResx, "Si", "Si"),
                action: function () {
                    if (typeof fun_yes === 'function') {
                        fun_yes();
                    }
                }
            },
            {
                text: TraduzioneMultiResx(datiMeteoResx, "No", "No"),
                action: function () {
                    if (typeof fun_no === 'function') {
                        fun_no();
                    }
                }
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}


function info_dlg(title, content) {

    let win_el = document.createElement("div");
    win_el.id = "tmp-kendo-dlg";
    document.body.appendChild(win_el);
    let $win_el = $("#" + win_el.id);

    $win_el.kendoDialog({
        title: title,
        closable: false,
        modal: true,
        visible: false,
        content: content,
        actions: [
            {
                text: TraduzioneMultiResx(datiMeteoResx, "Chiudi", "Chiudi")
            }
        ],
        close: function (e) {
            this.destroy();
        }
    });

    $win_el.data("kendoDialog").open();
}


function map_wnd(arrGeo) {

    let smap = $("#g-map-wrapper").data("simpleMap");

    let showLabel = (arrGeo.length <= 20);

    $.each(arrGeo, function (i, m) {

        smap.addMarker(m.geo.lat, m.geo.lng, m.label, showLabel);
    });

    smap.open();
    smap.showMarkers(true);
    smap.fitBounds();
}



(function ($) {
    $.filtroStazioni = function (elem, opts) {

        var plugin = this;

        var $listView = null;
        if (typeof opts.listView === "string") {
            if (!opts.listView.startsWith("#")) {
                opts.listView = "#" + opts.listView;
            }
            $listView = $(opts.listView);
        }

        var _filter = null;
        if (typeof opts.filter === "object") {
            _filter = opts.filter;
        }

        var filterInput = document.createElement("input");
        elem.appendChild(filterInput);

        let placeholder = "";
        if (typeof opts.placeholder === "string") {
            placeholder = opts.placeholder;
        }

        $(filterInput).kendoTextBox({
            placeholder: placeholder
        });

        let clearButton = document.createElement("span");
        clearButton.className = "k-clear-value";
        clearButton.setAttribute("unselectable", "on");
        clearButton.setAttribute("role", "button");
        clearButton.setAttribute("tabindex", "-1");

        $(filterInput).getKendoTextBox().wrapper.append(clearButton);

        let clearIcon = document.createElement("span");
        clearIcon.className = "k-icon k-i-x";

        clearButton.appendChild(clearIcon);

        clearButton.onclick = function () {
            plugin.clearFilter();
        };

        plugin.clearFilter = function () {
            if (filterInput.value === "") {
                return;
            }
            filterInput.value = "";
            if ($listView !== null) {
                $listView.getKendoListView().dataSource.filter({});
            }
        };

        filterInput.onkeyup = function (e) {

            if ($listView === null || _filter === null) {
                return;
            }

            let filter = {};
            let text = this.value;

            if (text !== "") {

                $.each(_filter.filters, function (i, f) {
                    f.value = text;
                });

                filter = _filter;
            }

            $listView.getKendoListView().dataSource.filter(filter);
        }
    }; 

    //Add the plugin to the jQuery.fn object
    $.fn.filtroStazioni = function (opts) {
        return this.each(function () {

            if (undefined == $(this).data('filtroStazioni')) {

                var plugin = new $.filtroStazioni(this, opts);

                $(this).data('filtroStazioni', plugin);
            }
        });
    };
})(jQuery);



(function ($) {

    $.stazioniDragDrop = function (elem, opts) {

        var plugin = this;

        plugin.$element = $(elem); 
        plugin.element = elem; 

        var _sourceListView = opts.sourceListView;
        var _stazioneDropCallback = opts.stazioneDropCallback;
        var _dragging = null;
        var _dropTarget = null;

        plugin.addDropTarget = function (e) {

            $(e).kendoDropTarget({
                dragenter: function (e) {
                    $(e.dropTarget).addClass("drag-enter");
                    _dropTarget = e.dropTarget;
                },
                dragleave: function (e) {
                    $(e.dropTarget).removeClass("drag-enter");
                    _dropTarget = null;
                },
                drop: function (e) {

                    $(e.dropTarget).removeClass("drag-enter");
                    _dropTarget = null;

                    if (_dragging === null) {

                        e.preventDefault();
                        return;
                    }
                    if (typeof _stazioneDropCallback !== "function") {
                        e.preventDefault();
                        return;
                    }

                    let stazDataItem = _sourceListView.dataSource.getByUid(_dragging.attr("data-uid"));

                    if (!_stazioneDropCallback(e.dropTarget, stazDataItem)) {
                        e.preventDefault();
                        return;
                    }
                }
            });
        };

        plugin.addDraggable = function (e) {

            $(e).kendoDraggable({
                filter: ".handler",
                hint: function (element) {
                    let item = element.closest(".list-view-item.stazione");
                    let hint = item.clone().css("pointer-events", "none");
                    hint.find(".drag-handle").addClass("dragging");
                    hint.find(".fa-btn").remove();

                    return hint;
                },
                dragstart: function (e) {
                    let item = e.currentTarget.closest(".list-view-item.stazione");
                    _enableHandler(false);
                    _dragging = item;
                },
                dragend: function (e) {
                    _enableHandler(true);
                    _dragging = null;
                },
                dragcancel: function (e) {
                    _enableHandler(true);
                    _dragging = null;
                    if (_dropTarget !== null) {
                        $(_dropTarget).removeClass("drag-enter");
                    }
                }
            });
        };


        var _enableHandler = function (bEnable) {
            if (bEnable) {

                _sourceListView.element.find(".stoppable-pointer-events").removeClass("pointer-events-disabled");

            } else {

                _sourceListView.element.find(".stoppable-pointer-events").addClass("pointer-events-disabled");
            }
        };  

    }; // stazioniDragDrop

    //Add the plugin to the jQuery.fn object
    $.fn.stazioniDragDrop = function (opts) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('stazioniDragDrop')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.stazioniDragDrop(this, opts);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('stazioniDragDrop', plugin);
            }
        });
    };

}) (jQuery);



(function ($) {

    $.listViewDropTarget = function (elem, opts) {

        // to avoid confusions, use "plugin" to reference the current instance of the object
        var plugin = this;
        // this will hold the merged default, and user-provided options plugin's properties will be available through this object like:
        // plugin.settings.propertyName from inside the plugin or element.data('pluginName').settings.propertyName from outside the plugin,
        // where "element" is the element the plugin is attached to;

        plugin.$element = $(elem); // reference to the jQuery version of DOM element
        plugin.element = elem; // reference to the actual DOM element

        var _$sourceElement = opts.sourceElement;
        var _placeholder = {
            dom: null,
            new_item: null,
            index: -1,
            hide: function () {
                if (this.dom === null) {
                    return;
                }
                if (this.new_item !== null) {
                    this.dom.remove();
                }
            },
            drag: function (e, listview_element) {

                if (this.dom === null) {
                    return;
                }

                let itemUnderMouse = e.target.closest(".list-view-item.sensore");

                if (this.new_item === null) {

                    if (itemUnderMouse === null) {
                        return;
                    }
                    if ($(itemUnderMouse).hasClass("placeholder")) {
                        return;
                    }
                    let parentLV = itemUnderMouse.closest(".k-listview");
                    if (parentLV === null) {
                        return;
                    }
                    if (parentLV.id !== listview_element.id) {
                        return;
                    }
                    if (e.offsetY > itemUnderMouse.clientHeight / 2) {

                        this.dom.insertAfter(itemUnderMouse);

                    } else {

                        this.dom.insertBefore(itemUnderMouse);
                    }

                } else {

                    let $listview = $(listview_element).getKendoListView().content;

                    if ($listview.children(":not(.placeholder)").length === 0) {

                        this.dom.appendTo($listview);

                    } else {

                        if (itemUnderMouse === null) {

                            this.dom.appendTo($listview);

                        } else {

                            if (!$(itemUnderMouse).hasClass("placeholder")) {

                                if (e.offsetY > itemUnderMouse.clientHeight / 2) {

                                    this.dom.insertAfter(itemUnderMouse);

                                } else {

                                    this.dom.insertBefore(itemUnderMouse);
                                }
                            }
                        }
                    }
                }
            },
            dragend: function () {

                if (this.dom === null) {
                    return;
                }

                if (this.new_item === null) {

                    //se non ho eseguito il drop riposiziono l'elemento nella sua posizione originale...

                    let newIndex = this.dom.index();
                    let oldIndex = this.index;

                    if (newIndex !== oldIndex) {

                        if (oldIndex === 0) {

                            this.dom.parent().prepend(this.dom);

                        } else {

                            let siblings = this.dom.parent().children();

                            if (oldIndex === siblings.length - 1) {

                                this.dom.parent().append(this.dom);

                            } else {

                                if (newIndex > oldIndex) {

                                    this.dom.insertBefore(siblings[oldIndex]);

                                } else {

                                    this.dom.insertAfter(siblings[oldIndex]);
                                }
                            }
                        }
                    }

                    this.dom.removeClass("placeholder");

                } else {

                    this.new_item = null;
                    this.dom.remove();
                }

                this.dom = null;
            },
            drop: function (e, datasource, allowChange) {

                if (this.dom === null) {
                    return;
                }

                let dom_index = this.dom.index();

                if (this.new_item === null) {

                    if (dom_index !== this.index) {

                        let uid = this.dom.attr("data-uid");
                        let drag_item = datasource.getByUid(uid);

                        if (drag_item !== undefined) {

                            datasource.remove(drag_item);
                            datasource.insert(dom_index, drag_item);
                        }
                    }

                    this.dom.removeClass("placeholder");

                } else {

                    this.dom.remove();

                    let dst_item = this.new_item;
                    this.new_item = null;

                    if (!allowChange) {

                        e.preventDefault();

                    } else {

                        let idata = 0;
                        let found = false;
                        while (!found && idata < datasource.data().length) {
                            found = (datasource.data()[idata].Id_Sensore === dst_item.Id_Sensore);
                            idata++;
                        }

                        if (found) {

                            e.preventDefault();
                        } else {

                            datasource.insert(dom_index, dst_item);
                        }
                    }

                }

                this.dom = null;
            }
        };
        var _allowChange = true;


        plugin.allowChange = function (bAllow) {
            _allowChange = bAllow;
        };


        plugin.addDraggable = function (e, fcallback) {

            $(e).kendoDraggable({
                filter: ".handler",
                hint: function (element) {
                    let item = element.closest(".draggable-container");
                    
                    let hint = item.clone().addClass("k-block sensore-background drag-hint").css("pointer-events", "none");
                    hint.find(".drag-handle").addClass("dragging");

                    return hint;
                },
                dragstart: function (e) {
                    let draggable = e.currentTarget.closest(".draggable-container");
                    let index = draggable.closest(".list-view-item.sensore").index();

                    let dst_item = fcallback(index);
                    if (dst_item === null) {
                        e.preventDefault();
                        return;
                    }

                    let placeholder = $("<div></div>").addClass("list-view-item").addClass("sensore").addClass("placeholder");
                    placeholder.append(draggable.clone().addClass("k-block"));

                    _placeholder.dom = placeholder;
                    _placeholder.new_item = dst_item;

                    _enableHandler(false);
                },
                dragend: function (e) {

                    _placeholder.dragend();
                    _enableHandler(true);
                },
                dragcancel: function (e) {

                    _placeholder.dragend();
                    _enableHandler(true);
                }
            });
        };
  
        plugin.$element.kendoDraggable({
            filter: ".handler",
            hint: function (element) {

                let hint = element.closest(".list-view-item.sensore").clone().addClass("drag-hint");
                hint.find(".drag-handle").addClass("dragging");

                return hint;
            },
            dragstart: function (e) {

                _placeholder.dom = e.currentTarget.closest(".list-view-item.sensore").addClass("placeholder");
                _placeholder.new_item = null;
                _placeholder.index = _placeholder.dom.index();

                _enableHandler(false);
            },
            drag: function (e) {

                _placeholder.drag(e, plugin.element);
            },
            dragend: function (e) {

                _placeholder.dragend();
                _enableHandler(true);
            },
            dragcancel: function (e) {

                _placeholder.dragend();
                _enableHandler(true);
            }
        });


        plugin.$element.kendoDropTarget({
            dragenter: function (e) {
                _placeholder.drag(e, plugin.element);
            },
            dragleave: function (e) {
                _placeholder.hide();
            },
            drop: function (e) {
                _placeholder.drop(e, plugin.$element.getKendoListView().dataSource, _allowChange);
            }
        });


        plugin.$element.mousemove(function (e) {
            _placeholder.drag(e, plugin.element);
        });


        var _enableHandler = function (bEnable) {
            if (bEnable) {

                plugin.$element.find(".stoppable-pointer-events").removeClass("pointer-events-disabled");
                _$sourceElement.find(".stoppable-pointer-events").removeClass("pointer-events-disabled");

            } else {

                plugin.$element.find(".stoppable-pointer-events").addClass("pointer-events-disabled");
                _$sourceElement.find(".stoppable-pointer-events").addClass("pointer-events-disabled");
            }
        };


    }; // listViewDropTarget

    //Add the plugin to the jQuery.fn object
    $.fn.listViewDropTarget = function (opts) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('listViewDropTarget')) {
                // create a new instance of the plugin
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.listViewDropTarget(this, opts);

                // in the jQuery version of the element store a reference to the plugin object
                // you can later access the plugin and its methods and properties like
                // element.data('pluginName').publicMethod(arg1, arg2, ... argn) or
                // element.data('pluginName').settings.propertyName
                $(this).data('listViewDropTarget', plugin);
            }
        });
    };

})(jQuery);



function openMapCallback(posEdit, places) {

    let smap = $("#g-map-wrapper").data("simpleMap");

    var _mapControl = {
        coord: null,
        latElem: null,
        lngElem: null
    };

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
        iconClass: "fa fa-check fa-fw'",
        click: function () {

            if (_mapControl.coord === null) {
                return;
            }
            posEdit.setLatLngDec(_mapControl.coord.lat, _mapControl.coord.lng);
            smap.close();
        }
    });

    _mapControl.coord = posEdit.getLatLngDec();
    _mapControl.latElem = _coordElem(divCoord, "Lat");
    _mapControl.lngElem = _coordElem(divCoord, "Lng");

    if (places && places.length > 0) {

        $.each(places, function (i, p) {
            smap.addMarker(p.lat, p.lng, p.label, true);
        });

        let css = ".gmap-toggle + label { ";
        css += "cursor: pointer; ";
        css += "position: relative; ";
        css += "box-shadow: inset 0 0 0px 1px #CCC; ";
        css += "height: 100%; ";
        css += "width: 100%; ";
        css += "border-radius: 4px; ";
        css += "margin: 0px; ";
        css += "font-size: 20px; ";
        css += "background: #F44836; ";
        css += "} ";
        css += ".gmap-toggle + label:before, .gmap-toggle + label:after { ";
        css += "position: absolute; ";
        css += "left: 0; ";
        css += "height: 100%; ";
        css += "width: 60%; ";
        css += "border-radius: 4px; ";
        css += "transition: .15s ease-in-out; ";
        css += "} ";
        css += ".gmap-toggle + label:before { content: ''; } ";
        css += ".gmap-toggle + label:after { ";
        css += "content: '\\f041'; ";
        css += "font-family: 'FontAwesome'; ";
        css += "text-align: center; ";
        css += "line-height: 175%; ";
        css += "color: #CCC; ";
        css += "background: #FFF; ";
        css += "box-shadow: inset 0 0 0px 1px #CCC; ";
        css += "} ";
        css += ".gmap-toggle:checked + label:before { width: 100%; background: #4CAF50; } ";
        css += ".gmap-toggle:checked + label:after { left: 40%; } ";

        let divToggle = document.createElement("div");
        divToggle.style.width = "60px";

        let toggle_style = document.createElement("style");
        toggle_style.type = "text/css";
        toggle_style.innerHTML = css;
        divToggle.appendChild(toggle_style);

        let toggle = document.createElement("input");
        toggle.type = "checkbox";
        toggle.id = "___toggle";
        toggle.className = "gmap-toggle";
        toggle.style.display = "none";
        let label = document.createElement("label");
        label.htmlFor = "___toggle";
        divToggle.appendChild(toggle);
        divToggle.appendChild(label);
        controlDiv.insertBefore(divToggle, controlDiv.childNodes[0]);

        toggle.onclick = function (ev) {
            smap.showMarkers(ev.currentTarget.checked);
        };
    }

    smap.open([{
        position: google.maps.ControlPosition.TOP_CENTER,
        control: controlDiv
    }]);

    var _setMapCoord = function (latlng) {

        _mapControl.coord = { lat: latlng.lat(), lng: latlng.lng() };

        _mapControl.latElem.innerText = kendo.toString(_mapControl.coord.lat, "0.0000000°");
        _mapControl.lngElem.innerText = kendo.toString(_mapControl.coord.lng, "0.0000000°");
    };

    if (_mapControl.coord === null) {

        smap.listenForMarker(_setMapCoord);

    } else {

        smap.addDraggableMarker(_mapControl.coord.lat, _mapControl.coord.lng, _setMapCoord);
    }

    smap.fitBounds();
}



(function ($) {

    $.simpleMap = function (elem, options) {

        var plugin = this;

        var _openCallback = function () { };
        if (typeof options.openCallback === "function") {
            _openCallback = options.openCallback;
        }

        var _closeCallback = function () { };
        if (typeof options.closeCallback === "function") {
            _closeCallback = options.closeCallback;
        }

        var _mapElem = elem;
        var _gmap = null;
        var _mapObjects = [];
        var _bounds = new google.maps.LatLngBounds();
        var _markers = [];
        var _makeMarker = function (lat, lng, draggable) {

            let ll = new google.maps.LatLng(lat, lng);

            _bounds.extend(ll);

            if (draggable) {

                return DraggableMarker(ll);
            }

            return StaticMarker(ll)
        }

        plugin.listenForMarker = function (funCallback) {
            google.maps.event.addListenerOnce(_gmap, "click", (event) => {
                let ll = event.latLng;
                this.addDraggableMarker(ll.lat(), ll.lng(), funCallback);
            });
        };

        var _draggableMarker = null;

        plugin.addDraggableMarker = function (lat, lng, dragCallback) {

            let marker = _makeMarker(lat, lng, true);
            marker.setMap(_gmap);
            marker.setDraggable(true);
            marker.setZIndex(1000);

            marker.addListener("drag", (event) => {
                dragCallback(event.latLng);
            });

            marker.addListener("dragend", (event) => {
                dragCallback(event.latLng);
            });

            dragCallback(marker.getPosition());

            _mapObjects.push(marker);

            _draggableMarker = marker;

            google.maps.event.addListener(_gmap, "click", (event) => {

                _draggableMarker.setPosition(event.latLng);
                dragCallback(_draggableMarker.getPosition());
            });

        };

        plugin.moveDraggableMarker = function (lat, lng, callback) {

            if (_draggableMarker == null) {

                plugin.addDraggableMarker(lat, lng, callback);

            } else {

                _draggableMarker.setPosition(new google.maps.LatLng(lat, lng));

                callback(_draggableMarker.getPosition());
            }

            _gmap.setCenter(_draggableMarker.getPosition());

            let zoom = _gmap.getZoom();
            if (zoom < 16) {
                _gmap.setZoom(16);
            }
        }

        plugin.addMarker = function (lat, lng, label, showLabel) {

            let marker = _makeMarker(lat, lng, false);
            _markers.push(marker);
            _mapObjects.push(marker);

            let etichetta = {
                text: label,
                fontSize: "12px",
                fontWeight: "bold",
                //color: "#F5F5F5",
                //color: "#FAFAD2",
                color: "#FFD700",
                className: "gmap-label"
            };

            if (showLabel) {

                marker.setLabel(etichetta);

            } else {

                marker.etichetta = etichetta;

                marker.addListener("click", function () {

                    if (!this.getLabel()) {

                        this.setLabel(this.etichetta);
                        this.setZIndex(1);

                    } else {

                        this.setLabel(null);
                        this.setZIndex();
                    }
                });
            }
        };

        plugin.showMarkers = function (show) {
            let map = (show ? _gmap : null);
            $.each(_markers, function (i, m) {
                m.setMap(map);
            });
        };

        plugin.open = function (arrCtrls) {

            if (_gmap === null) {

                _gmap = CreateMap(_mapElem);

                let style = document.createElement("style");
                style.type = "text/css";
                let css = ".gmap-label { text-shadow: -1px -1px 0 #000, 0 -1px 0 #000, 1px -1px 0 #000, 1px 0 0 #000, 1px 1px 0 #000, 0 1px 0 #000, -1px 1px 0 #000, -1px 0 0 #000; } ";
                css += ".gmap-close:hover { color: #333 !important; } ";
                css += "#" + _mapElem.id + " div:focus { outline: none !important; } ";
                style.innerHTML = css;

                _mapElem.appendChild(style);

                let closeDiv = document.createElement("div");
                closeDiv.className = "gmap-close";
                closeDiv.style.backgroundColor = "#FFF";
                closeDiv.style.paddingLeft = "5px";
                closeDiv.style.paddingBottom = "5px";
                closeDiv.style.borderBottomLeftRadius = "4px";
                closeDiv.style.cursor = "pointer";
                closeDiv.style.color = "#ccc";
                let closeSpan = document.createElement("span");
                closeSpan.className = "k-icon k-i-close";
                closeSpan.style.fontSize = "24px";
                closeDiv.appendChild(closeSpan);

                closeDiv.onclick = function () {
                    plugin.close();
                };

                _gmap.controls[google.maps.ControlPosition.TOP_RIGHT].push(closeDiv);
            }

            $.each(arrCtrls, function (i, ctrl) {
                _gmap.controls[ctrl.position].push(ctrl.control);
            });

            _openCallback();
        };

        plugin.fitBounds = function () {

            if (_bounds.isEmpty()) {
                return;
            }

            _gmap.setCenter(_bounds.getCenter());
            _gmap.fitBounds(_bounds);

            google.maps.event.addListenerOnce(_gmap, "tilesloaded", function () {
                let zoom = _gmap.getZoom();
                if (zoom > 16) {
                    _gmap.setZoom(16);
                }
            });
        };

        plugin.close = function () {

            _bounds = new google.maps.LatLngBounds();
            _markers = [];
            $.each(_mapObjects, function (i, o) {
                o.setMap(null);
            });
            _mapObjects = [];

            if (_gmap !== null) {

                google.maps.event.clearInstanceListeners(_gmap);
            }

            _gmap = null;

            _closeCallback();
        };

    }; //simpleMap

    $.fn.simpleMap = function (options) {
        return this.each(function () {
            // if plugin has not already been attached to the element
            if (undefined == $(this).data('simpleMap')) {

                var plugin = new $.simpleMap(this, options);
                $(this).data('simpleMap', plugin);
            }
        });
    };

})(jQuery);

