function StampaRiepilogoUtilizzoSuperficiMono(ev) {
    let pivaImpresa = Get_KendoDDLValue("ddlImprese", "");

    if (pivaImpresa === "") {
        MessaggioErrore_Bootstrap("Selezionare una impresa", "DIV_Messaggi");
        return;
    }

    let saCod = Get_KendoDDLValue("ddlCentriAziendali", 0);
    let dataRif = KendoDate("dpDataRif").value();
    if (dataRif === null) {
        dataRif = new Date();
    }

    GeneraRiepilogoUtilizzoSuperficiMono(pivaImpresa, saCod, dataRif);
}



function GenericOption(value, desc) {
    this.Value = value;
    this.Desc = desc;
}

/**
 * Aggiunge un elemento vuoto all'inizio dell'array di oggetti passato valorizzando le due proprietà specificate e impostando le altre a null.
 * Utile per aggiungere una riga all'inizio di una drop down list e renderla opzionale
 * @param {any} dataSource
 * @param {string} textProperty Nome della proprietà il cui testo è visibile all'utente
 * @param {string} valueProperty Nome della proprietà il cui valore deve essere passato lato server
 * @param {string} text Testo per l'utente
 * @param {any} value Valore per il server
 */
function AggiungiRigaVuota(dataSource, textProperty, valueProperty, text, value) {
    if (dataSource.length > 0) {
        var itemDataSource = Object.create(dataSource[0]); // La funzione crea sostanzialmente un clone dell'oggetto passato
        for (var prop in itemDataSource) { //restituisce il nome delle proprietà dell'oggetto

            // Sfrutto la bracket notation per valorizzare le proprietà dell'oggetto:
            // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Operators/Property_Accessors
            if (prop == textProperty) {
                itemDataSource[textProperty] = text;
            }
            else {
                if (prop == valueProperty) {
                    itemDataSource[valueProperty] = value;
                }
                else {
                    itemDataSource[prop] = null;
                }
            }

        }

        dataSource.unshift(itemDataSource);
    }
}


function ddlImpreseRead(options) {
    // Tramite la funzione slice ottengo una copia dell'array globale che la funzione RicercaImprese restituisce per riferimento
    var companiesList = RicercaImprese(false).slice();
    AggiungiRigaVuota(companiesList, "rag_soc", "piva", "", "");
    options.success(companiesList);
}

function ddlImpreseDataBound(e) {
    // Utilizzo il metodo select al posto di value perché in questo modo non causo una ri-esecuzione dell'evento dataBound stesso,
    // al netto del caso specifico per il quale ho collegato l'evento al controllo con la funzione "one"
    e.sender.select(function (dataItem) {
        return dataItem.piva === $(cIdPiva).val();
    });
    e.sender.trigger("change");
}

function ddlImpreseChange(e) {
    if (KendoDDL("ddlMagazzini") !== undefined) {
        KendoDDL("ddlMagazzini").dataSource.data([]); // Prima azzero i magazzini
    }
    if (KendoDDL("ddlCentriAziendali") !== undefined) {
        var kddlCentriAz = KendoDDL("ddlCentriAziendali");
        kddlCentriAz.dataSource.read();
        
        kddlCentriAz.select(0);
    }
        
}

function ddlCentriAziendaliRead(options) {
    var businessList = [];
    var pivaSel = Get_KendoDDLValue("ddlImprese", "");
    if (pivaSel !== "") {
        elencoCentriAziendali = null; // Forzo la rilettura dei centri.
        businessList = RicercaCentriAziendali(
            pivaSel,
            true,
            2,
            false
        );
        var businessCount = businessList.length;
        if (businessCount === 0) {
            AggiungiRigaVuota(businessList, "sa_nome", "sa_cod", "Nessun centro aziendale presente", 0)
        }
        else {
            if (businessCount > 1) {
                AggiungiRigaVuota(businessList, "sa_nome", "sa_cod", "", 0)
            }
        }

    }
    options.success(businessList);
}
