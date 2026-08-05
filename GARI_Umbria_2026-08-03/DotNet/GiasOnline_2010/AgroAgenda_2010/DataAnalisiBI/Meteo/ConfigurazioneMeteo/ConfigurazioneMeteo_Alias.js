

function _ridimensiona_AliasTabstrip(h) {

    $("#aliasArea").height(h);

    $("#aliasArea > div").each(function (i, e) {
        $(e).height(h);
    });
}


function _alias_jQueryDocReady() {

    let parentElem = document.getElementById("aliasArea");

    let style = document.createElement("style");
    style.type = "text/css";
    let css = ".alias-map-label { text-shadow: -1px -1px 0 #000, 0 -1px 0 #000, 1px -1px 0 #000, 1px 0 0 #000, 1px 1px 0 #000, 0 1px 0 #000, -1px 1px 0 #000, -1px 0 0 #000; } ";
    css += ".alias-action { font-size: 21px; } ";
    css += ".alias-action.k-i-check { color: #008000; } ";
    css += ".alias-action.k-i-cancel { color: #d45b1a; } ";
    css += ".alias-action.k-i-trash { color: #777777; } ";
    style.innerHTML = css;

    parentElem.appendChild(style);

    let container = document.createElement("div");
    container.id = "alias-map-container";
    container.style.position = "relative";
    container.style.border = "1px solid #ccc";
    container.style.backgroundColor = "#fff";

    let map = document.createElement("div");
    map.id = "alias-map-wrapper";
    map.style.width = "calc(100% - 10px)";
    map.style.left = "5px";
    map.style.height = "calc(100% - 10px)";
    map.style.top = "5px";

    container.appendChild(map);

    parentElem.appendChild(container);

    let gmap = CreateMap(map);

    google.maps.event.addListener(gmap, "click", (event) => {
        let ll = event.latLng;
        AddAlias(gmap, ll.lat(), ll.lng());
    });

    // Leggo gli alias presenti in DB.
    ajaxAgronicaSync(url_meteows + "/LeggiElencoAlias",
        JSON.stringify({}),
        false,
        function (risposta) {

            let arrAlias = risposta.RispostaStringa;

            $.each(arrAlias, function (i, e) {

                AddMarker(gmap, e.Lat, e.Lng, { id: e.Id, label: e.Name, isNew: false });
            });
        },
        function (risposta) {
        },
        null,
        false
    );
}


function AddMarker(gmap, lat, lng, info) {

    let ll = new google.maps.LatLng(lat, lng);
    let marker = StaticMarker(ll)

    marker.metadata = info;

    let etichetta = {
        text: info.label,
        fontSize: "13px",
        fontWeight: "bold",
        color: "#FFD700",
        className: "alias-map-label"
    };

    marker.setLabel(etichetta);

    marker.addListener("click", function () {
        EditAlias(marker)
    });

    marker.setMap(gmap);

    return marker;
}


function EditAlias(marker) {

    let win_el = document.createElement("div");
    win_el.id = "edit-alias";
    document.body.appendChild(win_el);
    let $win_el = $("#" + win_el.id);

    let actions = [
        {
            text: "<span class='alias-action k-icon k-i-check'></span>",
            action: function (e) {
                return UpdateAlias(marker);
            }
        },
        {
            text: "<span class='alias-action k-icon k-i-cancel'></span>",
            action: function (e) {
                if (marker.metadata.isNew) {
                    marker.setMap(null);
                    marker = null;
                }
                return true;
            }
        }
    ];
    if (!marker.metadata.isNew) {
        actions.push(
            {
                text: "<span class='alias-action k-icon k-i-trash'></span>",
                action: function (e) {
                    return DeleteAlias(marker);
                }
            });
    }

    $win_el.kendoDialog({
        title: TraduzioneMultiResx(datiMeteoResx, "...", "Alias stazione"),
        closable: false,
        modal: true,
        visible: false,
        minWidth: "50%",
        content: "<input id='__alias__' type='text' />",
        open: function () {
            $("#__alias__").kendoTextBox({
                value: marker.metadata.label
            });
        //    let textbox = $("#__alias__").getKendoTextBox();
        //    textbox.focus();
        },
        close: function (e) {
            this.destroy();
        },
        actions: actions
    });

    $win_el.data("kendoDialog").open();
}


function AddAlias(gmap, lat, lng) {

    //let loaderContainer = document.createElement("div");
    //loaderContainer.id = "alias-loader";
    //loaderContainer.style.position = "absolute";
    //loaderContainer.style.left = 0;
    //loaderContainer.style.top = 0;
    //loaderContainer.style.width = "100%";
    //loaderContainer.style.height = "100%";
    //loaderContainer.style.backgroundColor = "rgb(255 255 255 / 75%)";

    //document.getElementById("aliasArea").appendChild(loaderContainer);

    //let loader = document.createElement("div");
    //loader.style.cssText = "position:absolute; left:50%; top:50%; transform:translate(-50%, -50%);";
    //loaderContainer.appendChild(loader);
    //$(loader).StyleLoader();

//    setTimeout(function () {

        let param = {
            Lat: lat,
            Lng: lng
        };

        ajaxAgronica(url_meteows + "/LeggiStazioneXAlias",
            JSON.stringify(param),
            function (risposta) {

                //$("#aliasArea").find("#alias-loader").remove();

                let alias = risposta.RispostaStringa;

                if (alias.FlagNew)
                    EditAlias(AddMarker(gmap, alias.Lat, alias.Lng, { id: alias.Id, label: alias.Fornitore + " (" + alias.Name + ")", isNew: true }));
            },
            function (risposta) {

                //$("#aliasArea").find("#alias-loader").remove();
            }//,
        //    null,
        //    false
        );
//    }, 500);
}



function UpdateAlias(marker) {

    let alias = $("#__alias__").getKendoTextBox().value();

    if (alias.length == 0) {

        return false;
    }

    if (!marker.metadata.isNew && alias === marker.metadata.label) {

        return true;
    }

    let result = true;

    // Inserisco o aggiorno il DB
    ajaxAgronicaSync(url_meteows + "/UpsertAlias",
        JSON.stringify({ id: marker.metadata.id, value: alias}),
        false,
        function (risposta) {

            let alias = risposta.RispostaStringa;

            marker.metadata.isNew = false;
            marker.metadata.label = alias.Name;
            marker.getLabel().text = alias.Name;

            let gmap = marker.getMap();
            marker.setMap(null);
            marker.setMap(gmap);

            $("#aliasArea").attr("data-reload", "1");
        },
        function (risposta) {
        },
        null,
        false
    );

    return result;
}


function DeleteAlias(marker) {
    // Elimino dal DB...
    ajaxAgronicaSync(url_meteows + "/DeleteAlias",
        JSON.stringify({ id: marker.metadata.id}),
        false,
        function (risposta) {

            marker.setMap(null);
            marker = null;

            $("#aliasArea").attr("data-reload", "1");
        },
        function (risposta) {
        },
        null,
        false
    );

    return true;
}