

//////////////////////////////////////////////////////////
// Griglia Imputazione
//////////////////////////////////////////////////////////

function EmptyRead(options) { }
function EmptySubmit(options) { }

function CaricaGrigliaImpianti(options, parametriPerLettura) {

    let bok = false;
    let elem_cod = 0;
    let matCod = 0;
    let codProdotto = Get_KendoDDLValue("ddlProdottoDes");
    if (Get_KendoDDLValue("ddlCategorieMagazzino") !== undefined)
        elem_cod = parseInt(Get_KendoDDLValue("ddlCategorieMagazzino"));
    if (codProdotto !== null && codProdotto !== undefined && codProdotto !== "" && codProdotto < 0) {
        matCod = Math.abs(codProdotto);
    }


    let keyDet = $('input[name$="hf_key_mov_dett"]').val();
    if (keyDet !== "") {
        idAgenda = parseInt(keyDet.split("_")[2]);
        idMov = parseInt(keyDet.split("_")[3]);
        idMovDet = parseInt(keyDet.split("_")[4]);

    } else {
        idAgenda = 0;
        idMov = 0;
        idMovDet = 0;
    }

    if (elem_cod !== 0 && matCod !== 0) {
        var param = kendo.stringify({
            piva: $(cIdPiva).val(),
            id_agenda: idAgenda,
            id_mov_det: idMovDet,
            piva_rif: GetPivaConferente(),
            elem_cod: elem_cod,
            mat_cod: matCod,
            filtro_impianti: KendoDDL("cmbFiltroImpianti").dataItem().Tipo_Filtro,
            data_movimento: get_data("inDataEmissione"),
            chksmart: parseInt(parametriPerLettura[0])
        });

        var risp;
        ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CaricaGrigliaImputazioneImpianti",
            param, 
            false,
            function (risposta) {
                risp = JSON.parse(risposta.RispostaStringa);
                bok = true;
            }, null);

        //Impostazione Impianti
        var lanciaChangeCmbRipartizione = false;
        if (risp.length > 0) {
            
            $('input[name$="txt_note_raccolta"]').val(risp[0].Note_Raccolta);

            if (risp[0].Impianti_Indefiniti === 1) {

                setKendoSwitch("ChkImpiantiIndefiniti", true);
                $("#tab_griglia_impianti").hide();
                $("#id_ripartizione").hide();
                $("#id_filtroimpianti").hide();
                $("#id_note_raccolta").hide();
                if (lavCodAccettazionePomodoro === false) {
                    $("#warningImpiantiIndefiniti").show();
                }

            } else {

               
                filtro_impianti = parseInt(risp[0].Filtro_Impianti); //da migliore setup in base ai dati inseriti
                              
                setKendoSwitch("ChkImpiantiIndefiniti", false);
                $("#tab_griglia_impianti").show();
                $("#id_ripartizione").show();
                $("#id_filtroimpianti").show();
                $("#id_note_raccolta").show();
                $("#warningImpiantiIndefiniti").hide();

                //In caso di ricerca filtro smart imposto il risultato senza far scattare il change
                if (parseInt(parametriPerLettura[0]) == 1 && idMovDet === 0) {

                    KendoDDL("cmbFiltroImpianti").select(filtro_impianti);

                    if (parseInt(Modalita_Ripartizione) !== -1) {

                        if (parseInt(Modalita_Ripartizione) == 0 && risp.length == 1) {

                            KendoDDL("cmbRipartizione").select(1) //1 solo impianto --> automatico

                        }
                        else {

                            KendoDDL("cmbRipartizione").select(parseInt(Modalita_Ripartizione)); //da tabella imprese_impostazioni
                        }
                    }

                    else {
                        KendoDDL("cmbRipartizione").select(1); //default automatico sulla sup
                    }

                    
                    //devo spostare questa esecuzione dopo, perché in questo punto in realtà non ho ancora messo i dati in griglia, 
                    // quindi è vuota e non setterebbe la qta
                    lanciaChangeCmbRipartizione = true;
                    //cmbRipartizione_change();


                }
                else {

                    if (idMovDet !== 0) {
                        //Impostazione Manuale
                        KendoDDL("cmbRipartizione").select(0); //Manuale
                        lanciaChangeCmbRipartizione = true;
                        //cmbRipartizione_change();

                    }
                }


            }

        } else {

            //Nessun Impianto Collegato                
            setKendoSwitch("ChkImpiantiIndefiniti", true);
            $("#tab_griglia_impianti").hide();
            $("#id_ripartizione").hide();
            //$("#id_filtroimpianti").hide();
            $("#id_note_raccolta").hide();
            if (lavCodAccettazionePomodoro === false) {
                $("#warningImpiantiIndefiniti").show();
            }
        }


        if (bok) {

            options.success(risp);

            if (lanciaChangeCmbRipartizione === true) {
                cmbRipartizione_change();
            }
        }

    }
    
}




function LeggiImprese_Impostazioni(impostazione_cod, valore_iniziale) {

    var impostazione_valore = "0"
    var param = kendo.stringify({});


    var param = kendo.stringify({
        piva: $(cIdPiva).val(), sa_cod: Qs_SaCod, impostazione_cod: impostazione_cod, valore_iniziale: valore_iniziale
    });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/LeggiImprese_Impostazioni",
        param, false,
        function (risposta) {            

            impostazione_valore = risposta.RispostaStringa;

        }, null);    

    return impostazione_valore;

}


function RiempiCodiceVarietaPomodoro(options) {

    var param = kendo.stringify({ });

    ajaxAgronicaSync(indirizzoHttp_DocContabile_WS + "/CodificaVarietaPomodoro",
        param,
        false,
        function (risposta) {
            let risp = JSON.parse(risposta.RispostaStringa);
            risp.unshift({ "Cod_Varieta": "", "Desc_Varieta": "" });
            options.success(risp);
        },
        null);

}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////////////// FUNZIONI CONTROLLO E RESTORE //////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Controllo campi obbligatori
function controllaRigheValidePerSubmitGrid(righe) {

    var nrErr = 0;

    for (let x = 0; x < righe.length; x++) {

        let item = righe[x];

        if (item.Id_Reg === 0 || item.Superficie === 0)  {
            nrErr++;
        }
    }

    return nrErr;
}
