
function _aggiungiParametro(id, tipo) {

    let value = ""

    if (tipo === "text") {
        value = $("#" + id).val();
    } else if (tipo === "ddl") {
        value = $("#" + id).getKendoDropDownList().value();
    } else if (tipo === "multisel") {
        value = $("#" + id).getKendoMultiSelect().value().join("|");
    } else if (tipo === "radio") {
        value = "" + $("#" + id).prop("checked");
    } else if (tipo === "switch") {
        value = $("#" + id).getKendoSwitch().check();
    } else if (tipo === "date") {
        let dt = $("#" + id).getKendoDatePicker().value();
        if (dt === null) {
            value = "";
        } else {
            value = kendo.toString(dt, "yyyy-MM-dd");
        }
    } else if (tipo === 'tabstrip') {
        value = $("#" + id).getKendoTabStrip().select().index();
    }

    return {
        id: id,
        tipo: tipo,
        value: value
    }
}

function Salva_Filtri() {

    if (!flgIncludiVisite) {
        return;
    }

    let parametri = {
        _versione: versioneJsonFiltri,
        _parametri: [
            _aggiungiParametro("CBLCooperativa", "multisel")
        ]
    };

    _salvaParametri(parametri);
}

function _salvaParametri(parametri) {

    let strParametri = JSON.stringify(parametri);

    ajaxAgronica(location.pathname + '/SalvaFiltri',
        kendo.stringify({ "parametri": strParametri }),
        function (risposta) {

            if (risposta.RispostaOK) {

                var risp = risposta.RispostaStringa;
            } else {

                console.log("Errore nel salvataggio parametri filtro: " + risposta.Errore);
            }
        },
        function () {
            console.log("Errore nel salvataggio parametri filtro");
        }
    );

}

function radioButtonCambio(str) {
    switch (str) {
        case "#annoO":
            var div = document.getElementById("fine");
            div.style.visibility = 'hidden';
            div = document.getElementById("lblInizioIntervallo");
            div.innerHTML = 'data';
            break;
        case "#intervalloO":
            var div = document.getElementById("fine");
            div.style.visibility = 'visible';
            div = document.getElementById("lblInizioIntervallo");
            div.innerHTML = 'data inizio';
            break;
    }
}

function popolaDropDownCooperative() {
    $("#CBLCooperativa").kendoMultiSelect({
        autoclose: false,
        autoBind: true,
        filter: "contains",
        dataTextField: "des",
        dataValueField: "val",
        dataSource: { transport: { read: RiempiDdlCooperative } },
        change: function (e) {

        }
    });
}

function RiempiDdlCooperative(options) {
    $.ajax({
        type: 'POST',
        url: "../SchedePersonalizzate/x_Esporta_GiasToSap.asmx/GetCooperative",
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json', async: true,
        success: function (r) {
            var p = JSON.parse(r.d);
            options.success(p);
        }
    });
}

function popolaDropDownPianiSemina() {
    $("#CBLPianoSemina").kendoMultiSelect({
        autoclose: false,
        autoBind: true,
        filter: "contains",
        dataTextField: "des",
        dataValueField: "val",
        dataSource: { transport: { read: RiempiDdlPianiSemina } },
        change: function (e) {

        }
    });
}

function RiempiDdlPianiSemina(options) {
    $.ajax({
        type: 'POST',
        url: "../SchedePersonalizzate/x_Esporta_GiasToSap.asmx/GetPianiSemina",
        data: "{}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json', async: true,
        success: function (r) {
            var p = JSON.parse(r.d);
            options.success(p);
        }
    });
}

function popolaDropDownSpecieVegetali() {
    $("#ddlSpecieVegetali").kendoDropDownList({
        autoClose: false,
        autoBind: false,
        filter: "contains",
        dataTextField: "descrizione",
        dataValueField: "codice",
        dataSource: { transport: { read: RiempiDdlSpecieVegetali } },
        value: [],
        change: function (e) {
            if ($("#ddlVarieta").data("kendoMultiSelect").dataSource !== undefined)
                $("#ddlVarieta").data("kendoMultiSelect").dataSource.read();
        }
    });
}

function RiempiDdlSpecieVegetali(options) {

    var Gru_Cod = "0";
    $.ajax({
        type: 'POST',
        url: '../SchedePersonalizzate/x_Esporta_GiasToSap.asmx/GetSpecieVegetali',
        data: "{Gru_Cod: '" + Gru_Cod + "'}",
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        success: function (r) {
            var p = JSON.parse(r.d);
            options.success(p);
        }
    });

}

function popolaDropDownVarieta() {
    $("#ddlVarieta").kendoMultiSelect({
        autoClose: false,
        autoBind: true,
        filter: "contains",
        dataTextField: "descrizione",
        dataValueField: "codice",
        dataSource: { transport: { read: RiempiDdlVarieta }, group: { field: "veg_des" }, sort: { field: "descrizione", dir: "asc" } }
    });
}

function RiempiDdlVarieta(options) {

    var Veg_Cod = $("#ddlSpecieVegetali").data("kendoDropDownList").value();

    //if (Veg_Cod !== null && Veg_Cod !== "" && !(Veg_Cod.length === 1 && Veg_Cod[0] === "")) {
    //    Veg_Cod = Veg_Cod.clean("");
    if (Veg_Cod.length > 0) {

        $.ajax({
            type: 'POST',
            url: '../SchedePersonalizzate/x_Esporta_GiasToSap.asmx/GetCultivar',
            data: "{Veg_Cod: '" + Veg_Cod + "'}",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: false,
            success: function (r) {
                var p = JSON.parse(r.d);
                options.success(p);
            }
        });

    } else {
        options.success([]);
    }

}

function prendiValori() {
    //Cooperative
    var objC = $("#CBLCooperativa").data("kendoMultiSelect").value();

    //Piani Semina
    var objPs = $("#CBLPianoSemina").data("kendoMultiSelect").value();
    var PsStr = JSON.stringify(objPs);
    if (PsStr.length === 2 || !PsStr) {
        objPs = [""];
    };

    //Specie Vegetali
    var objSv = {
        "Valore": $("#ddlSpecieVegetali").data("kendoDropDownList").value(),
        "Testo": $("#ddlSpecieVegetali").data("kendoDropDownList").text()
    };

    //Varieta
    var objV = $("#ddlVarieta").data("kendoMultiSelect").value();
    var PsStr = JSON.stringify(objV);
    if (PsStr.length === 2 || !PsStr) {
        objV = [""];
    };

    //Formato e anno/intervallo
    var objR = document.querySelector('input[name="formato"]:checked').value;
    var anno = document.querySelector('input[name="annointervallo"]:checked').value;
    if (anno == "0") {
        var objDi = kendo.toString($("#txtInizioIntervallo").data("kendoDatePicker").value(), 'yyyy');
        var objDf = "";

        //if (objDi === null) {
        //    objDi = "2000";
        //    alert(objDi);
        //};
    }
    else {
        var objDi = kendo.toString($("#txtInizioIntervallo").data("kendoDatePicker").value(), 'dd/MM/yyyy');
        var objDf = kendo.toString($("#txtFineIntervallo").data("kendoDatePicker").value(), 'dd/MM/yyyy');
    }

    var indata = {
        "Cooperativa": objC,
        "PianoSemina": objPs,
        "SpecieVegetali": objSv,
        "Varieta": objV,
        "DataInizio": objDi,
        "DataFine": objDf,
        "Intervallo": objR
    };

    $.ajax({
        type: 'POST',
        url: '../SchedePersonalizzate/x_Esporta_GiasToSap.asmx/stamp',
        data: JSON.stringify(indata),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: false,
        success: function (r) {
            console.log(r["d"]);
            window.location.href= r["d"];
        }
    });
}

function ImpostaOggi() {
    var oggi = kendo.toString(new Date(), 'dd/MM/yyyy');
    $("#txtInizioIntervallo").val(oggi);
    $("#txtInizioIntervallo").data("kendoDatePicker").value(oggi);
    $("#txtFineIntervallo").val(oggi);
    $("#txtFineIntervallo").data("kendoDatePicker").value(oggi);
}