
function duplica_Effettivo_Righe_KendoGrid(e, rigaDuplicataKendoGrid, rigaDaCopiareKendoGrid)
{
    // Gestione della duplicazione di una riga
    //      Vengono copiati solo i valori delle colonne marcate come "Da duplicare""
    //      Questa funzione viaggia in coppia con la funzione Duplica
    if (rigaDuplicataKendoGrid && rigaDaCopiareKendoGrid != null && e.model.isNew() && !e.model.dirty) {

        var gridId = e.sender.element[0].id;
        var grid = $("#" + gridId).data("kendoGrid");

        rigaDaCopiareKendoGrid.forEach(function (valore, campo) {
            //console.log('[' + campo + '] ' + valore);
            var duplicazioneEffettuata = false;
            for (var r = 0; r < grid.columns.length; r++) {
                var col = grid.columns[r];
                if (col.daDuplicare && col.field !== undefined && col.field === campo) {
                    if (e.model.get(campo) !== valore) {
                        if (campo === "Validita_Inizio" || campo === "Validita_Fine")
                            e.container.find("input[name=" + campo + "]").val(formattedDate(valore, "/")).change();
                        else
                            e.container.find("input[name=" + campo + "]").val(valore).change();
                        e.model.set(campo, valore);
                    }
                    duplicazioneEffettuata = true;
                }
            }

            if (!duplicazioneEffettuata) {
                // Se non ho trovato il campo fra le colonne della griglia cerco se è una DropDownList (i campi 
                // codice in questo caso non fanno parte delle colonne quindi non li trova)
                if (e.container.find("input[name=" + campo + "]").data("kendoDropDownList") !== undefined) {
                    var keyCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.dataValueField;
                    var textCampo = null;

                    // nomeCampoEffettivo è un campo valorizzato solo per i parametri qualitativi
                    if (e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.nomeCampoEffettivo !== undefined)
                        textCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.nomeCampoEffettivo;
                    else
                        textCampo = e.container.find("input[name=" + campo + "]").data("kendoDropDownList").options.dataTextField;

                    if (textCampo != null) {
                        for (var r = 0; r < grid.columns.length; r++) {
                            var col = grid.columns[r];
                            if (col.daDuplicare && col.field !== undefined && col.field === textCampo) {
                                if (e.model.get(campo) !== valore) {
                                    e.container.find("input[name=" + keyCampo + "]").val(valore).change();
                                    e.model.set(campo, valore);
                                }
                                duplicazioneEffettuata = true;
                            }
                        }
                    }
                }
            }

            if (!duplicazioneEffettuata) {
                //Gestione dei campi che non voglio duplicare per evitare che vengano impostati con i default
                for (var f in grid.dataSource.options.schema.model.fields) {
                    if (grid.dataSource.options.schema.model.fields.hasOwnProperty(f) &&
                        f === campo &&
                        grid.dataSource.options.schema.model.fields[f].defaultValue !== undefined) {
                        //console.log(f + " -> " + grid.dataSource.options.schema.model.fields[f]);
                        if (grid.dataSource.options.schema.model.fields[f].type == "string") {
                            e.container.find("input[name=" + f + "]").val("").change();
                            e.model.set(f, "");
                        }
                        if (grid.dataSource.options.schema.model.fields[f].type == "date") {
                            e.container.find("input[name=" + f + "]").val("").change();
                            e.model.set(f, "");
                        }
                        if (grid.dataSource.options.schema.model.fields[f].type == "number") {
                            e.container.find("input[name=" + f + "]").val(0).change();
                            e.model.set(f, 0);
                        }
                        if (grid.dataSource.options.schema.model.fields[f].type == "boolean") {
                            e.container.find("input[name=" + f + "]").val(false).change();
                            e.model.set(f, false);
                        }
                    }
                }
            }
        });

        rigaDuplicataKendoGrid = false;
        rigaDaCopiareKendoGrid = null;

    }

}

function OnTabShow(tabId) {

    if (tabStripAperti.includes(tabId))
        return;

    tabStripAperti.push(tabId);
    switch (tabId)
    {
        case "a_tabNumeratoriPS":
            CaricaDropDownNumeratoriTipo();
            Popola_Numeratori_Prefisso_Suffisso("tab_numeratore_prefisso_suffisso");
            break;
        case "a_tabNumeratoriDefault":
            CaricaDropDownNumeratoriPS_Default();
            Popola_Numeratori_Default("tab_numeratore_defaults");
            break;
    }
    
}