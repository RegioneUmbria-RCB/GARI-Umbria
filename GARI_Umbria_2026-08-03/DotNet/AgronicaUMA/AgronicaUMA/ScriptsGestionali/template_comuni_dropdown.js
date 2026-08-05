// ------------------------
// Template Ricerca su DDL
// ------------------------

filterable_celle_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function(element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoCelle,
            dataTextField: "Ubic_Des",
            dataValueField: "Ubic_Des",
            optionLabel: " "
        });
    }
};

filterable_materie_primeTemplate = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function(element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoTuttiProdotti,
            dataTextField: "Mat_Des",
            dataValueField: "Mat_Des",
            optionLabel: " "
        });
    }
};

filterable_unita_misuraTemplate = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function(element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoCategorieXUnitaMisura,
            dataTextField: "Udm_Des",
            dataValueField: "Udm_Des",
            optionLabel: " "
        });
    }
};

filterable_tipoParamQualTemplate = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoParametriQualitativi,
            dataTextField: "Tabella_Des",
            dataValueField: "Tabella_Des",
            optionLabel: " "
        });
    }
};

function getfilterable_paramQualTemplate(piva, Tabella_ID, mostraSigla) {

    var dTF = "val_des";
    var dVF = "val_des";

    if (mostraSigla) {
        dTF = "val_sigla";
        dVF = "val_sigla";
    }

    var valoriParametriQualitativiFF = RicercaValoriParametriQualitativi(Tabella_ID, piva, true);

    filterable_paramQualTemplate = {
        extra: false,
        operators: {
            string: {
                eq: "Uguale a"
            }
        },
        ui: function (element) {
            element.kendoDropDownList({
                autoWidth: true,
                filter: "contains",
                dataSource: valoriParametriQualitativiFF,
                //optionLabel: {
                //    val_des: "",
                //    val_cod: 0
                //},
                dataTextField: dTF,
                dataValueField: dVF
            });
        }
    };

    return filterable_paramQualTemplate;
}

filterable_anniApertiConti_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoAnniApertiConti,
            dataTextField: "Anno",
            dataValueField: "Anno_Cod",
            optionLabel: " "
        });
    }
};

filterable_conto_economico_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoContoEconomico,
            dataTextField: "Descr_Conto",
            dataValueField: "Descr_Conto",
            optionLabel: " "
        });
    }
};

filterable_conto_patrimoniale_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoContoPatrimoniale,
            dataTextField: "Descr_Conto_Pat",
            dataValueField: "Descr_Conto_Pat",
            optionLabel: " "
        });
    }
};

filterable_sconto_Modalita_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoScontoModalita,
            dataTextField: "Sconto_Modalita_Descr",
            dataValueField: "Sconto_Modalita",
            optionLabel: " "
        });
    }
};

filterable_IVA_Aliquote_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoIVA_Aliquote,
            dataTextField: "Sigla_IVA",
            dataValueField: "Cod_IVA",
            optionLabel: " "
        });
    }
};

filterable_prezzo_Livello_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoPrezzoLivello,
            dataTextField: "Prezzo_Livello_Descr",
            dataValueField: "Prezzo_Livello",
            optionLabel: " "
        });
    }
};

filterable_scontomaggiorazione_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoScontoMaggiorazione,
            dataTextField: "ScontoMaggiorazione_Descr",
            dataValueField: "ScontoMaggiorazione",
            optionLabel: " "
        });
    }
};

filterable_tempocarenza_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "contains",
            dataSource: elencoTempoCarenza,
            dataTextField: "TempoCarenza_Descr",
            dataValueField: "TempoCarenza",
            optionLabel: " "
        });
    }
};

filterable_fornitori_Template = {
    extra: false,
    operators: {
        string: {
            eq: "Uguale a"
        }
    },
    ui: function (element) {
        element.kendoDropDownList({
            autoWidth: true,
            filter: "equals",
            dataSource: elencoFornitori,
            dataTextField: "Rag_Soc",
            dataValueField: "Rag_Soc",
            optionLabel: " "
        });
    }
};

// ------------------
// Template DDL
// ------------------

function paramQual_Template(container, options) {

    paramQualGestiti = RicercaParametriQualitativi(true, $(cIdPiva).val()); //TODO: GIULIA - ELIMINA ANCHE DA QUI!!!

    var tabella_ID = 0;
    var campoCodice = "";
    for (var i = 0; i < paramQualGestiti.length; i++) {
        if (paramQualGestiti[i].Tabella_ID !== 0) {
            if (options.field === "FF_" + paramQualGestiti[i].Tabella_Cod_Des + "_Sigla" ||
                options.field === "FF_" + paramQualGestiti[i].Tabella_Cod_Des + "_Descrizione") {

                tabella_ID = kendo.parseInt(paramQualGestiti[i].Tabella_ID);
                campoCodice = "FF_" + paramQualGestiti[i].Tabella_Cod_Des + "_Tipo_Cod";
            }
        }
    }

    var valoriParametriQualitativiFF = RicercaValoriParametriQualitativi(tabella_ID, $(cIdPiva).val(), true); //TODO: GIULIA - ELIMINA ANCHE DA QUI!!!

    //$('<input name="val_cod" ' + setValidation(container, options) + '" data-bind="value:' + campoCodice + '"/>')
    $('<input name="' + campoCodice + '" ' + setValidation(container, options) + '" data-bind="value:' + campoCodice + '"/>')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            nomeCampoEffettivo: options.field,
            dataTextField: "val_des",
            dataValueField: "val_cod",
            //optionLabel: {
            //    val_des: "",
            //    val_cod: 0
            //},
            dataSource: valoriParametriQualitativiFF,
            change: function (e) {
                changedParamQual(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="val_cod"></span>').appendTo(container);
}

function materie_prime_Template(container, options) {

    $('<input name="Mat_Cod" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "Mat_Des",
            dataValueField: "Mat_Cod",
            optionLabel: " ",
            dataSource: elencoTuttiProdotti,
            change: function (e) {
                changedMateriePrime(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="Mat_Cod"></span>').appendTo(container);
}

function unita_misura_Template(container, options) {

    $('<input name="Udm_Cod" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "Udm_Des",
            dataValueField: "Udm_Cod",
            optionLabel: " ",
            dataSource: elencoCategorieXUnitaMisura,
            change: function (e) {
                changedUnitaMisura(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="Udm_Cod"></span>').appendTo(container);
}

function celle_Template(container, options) {
    
    $('<input name="key_Dest" data-text-field="Ubic_Des" ' + setValidation(container, options) + '"  data-value-field="key_Dest" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            optionLabel: " ",
            dataSource: elencoCelle,
            change: function (e) {
                changedCelle(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="key_Dest"></span>').appendTo(container);
}

function magazzini_Template(container, options) {

    $('<input name="key_Dest" data-text-field="Ubic_Des" ' + setValidation(container, options) + '" data-value-field="key_Dest"  />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            optionLabel: " ",
            dataSource: elencoMagazzini,
            change: function (e) {
                changedMagazzini(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="key_Dest"></span>').appendTo(container);
}

function celleMagazzini_Template(container, options) {

    $('<input name="key_Dest" data-text-field="Ubic_Des" required="' + setValidation(container, options) + '" data-value-field="key_Dest"  />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            optionLabel: " ",
            dataSource: elencoCelleMagazzini,
            change: function (e) {
                changedCelleMagazzini(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="key_Dest"></span>').appendTo(container);
}

function anniApertiConti_Template(container, options) {

    $('<input name="Anno_Cod" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "Anno",
            dataValueField: "Anno_Cod",
            optionLabel: " ",
            dataSource: elencoAnniApertiConti,
            change: function (e) {
                changedAnniApertiConti(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="Anno_Cod"></span>').appendTo(container);
}

function conto_economico_Template(container, options) {

    $('<input name="Cod_Conto" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "Descr_Conto",
            dataValueField: "Cod_Conto",
            optionLabel: " ",
            dataSource: elencoContoEconomico,
            //change: function (e) {
            //    changedMateriePrime(e); // Da gestire in ogni specifico js
            //},
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="Cod_Conto"></span>').appendTo(container);
}

function conto_patrimoniale_Template(container, options) {

    $('<input name="Cod_Conto_Pat" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "Descr_Conto_Pat",
            dataValueField: "Cod_Conto_Pat",
            optionLabel: " ",
            dataSource: elencoContoPatrimoniale,
            //change: function (e) {
            //    changedMateriePrime(e); // Da gestire in ogni specifico js
            //},
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="Cod_Conto_Pat"></span>').appendTo(container);
}

function sconto_Modalita_Template(container, options) {

    $('<input name="Sconto_Modalita" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "Sconto_Modalita_Descr",
            dataValueField: "Sconto_Modalita",
            optionLabel: " ",
            dataSource: elencoScontoModalita,
            change: function (e) {
                changedSconto_Modalita(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="Sconto_Modalita"></span>').appendTo(container);
}

function IVA_Aliquote_Template(container, options) {

    $('<input name="Cod_IVA" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "Sigla_IVA",
            dataValueField: "Cod_IVA",
            optionLabel: " ",
            dataSource: elencoIVA_Aliquote,
            change: function (e) {
                changedIVA_Aliquote(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="Sconto_Modalita"></span>').appendTo(container);
}

function prezzo_Livello_Template(container, options) {

    $('<input name="Prezzo_Livello" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "Prezzo_Livello_Descr",
            dataValueField: "Prezzo_Livello",
            optionLabel: " ",
            dataSource: elencoPrezzoLivello,
            change: function (e) {
                changed_prezzo_Livello(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="Prezzo_Livello"></span>').appendTo(container);
}

function scontomaggiorazione_Template(container, options) {

    $('<input name="ScontoMaggiorazione" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "ScontoMaggiorazione_Descr",
            dataValueField: "ScontoMaggiorazione",
            optionLabel: " ",
            dataSource: elencoScontoMaggiorazione,
            //change: function (e) {
            //    scontomaggiorazione_Livello(e); // Da gestire in ogni specifico js
            //},
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="ScontoMaggiorazione"></span>').appendTo(container);
}

function tempocarenza_Template(container, options) {

    $('<input name="TempoCarenza" ' + setValidation(container, options) + '" />')
        .appendTo(container)
        .kendoDropDownList({
            autoWidth: true,
            autoBind: true,
            filter: "contains",
            dataTextField: "TempoCarenza_Descr",
            dataValueField: "TempoCarenza",
            optionLabel: " ",
            dataSource: elencoTempoCarenza,
            change: function (e) {
                tempocarenza_Livello(e); // Da gestire in ogni specifico js
            },
            open: kendoDropDownAdjustWidth,
            dataBound: kendoDropDownAdjustWidth
        });
    $('<span class="k-invalid-msg" data-for="TempoCarenza"></span>').appendTo(container);
}