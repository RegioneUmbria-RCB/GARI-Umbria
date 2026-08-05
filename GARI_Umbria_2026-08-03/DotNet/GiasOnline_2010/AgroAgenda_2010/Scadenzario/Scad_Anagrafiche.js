$('#tabs a[data-toggle="tab"]').on("shown.bs.tab", function (e) {
    let target = $(e.target).attr("id"); // activated tab

    if (target === "a_tabCategorieTipologie") {
        LsbAree_Load();

        setKendoSwitch("switch_utilizzabile_da_app", false);

        $("#Tipologie_utilizzabili_da_app").hide();
        $("#accessori_tipologia").hide();
    }

    if (target === "a_tabIndici") {
        ConfiguraGrigliaIndici("tab_griglia_Indici");
    }

    if (target === "a_tabIndiciXTipologie") {
        //Leggo l'anagrafica delle aziende nella tab associazione Tipologie-Indici, filtrate in base alla visibilità utente
        //InXTip_UC_ddlAzienda_Load();

        //Carico la Listbox con le aree associate alla Piva
        InXTip_UC_LsbAree_Load();

        //Creo lo Switch Obbligatorio nella tab tab associazione Tipologie-Indici e lo imposto disattivato
        creaKendoSwitch($("#cb_obbligatorio_liv").data("kendoSwitch"), undefined, undefined, false, undefined, undefined, undefined);

        $("#cb_obbligatorio_liv").data("kendoSwitch").bind("change", cb_obbligatorio_liv_change);

        $("#cb_obbligatorio_liv").data("kendoSwitch").enable(false);

        $("#Switch_Campo_Obbligatorio").hide();
    }

    if (target === "a_tabCategTipolDocumentoXUtenti") {

        //Panelbar con le Griglie
        $("#panelbarCategTipolDocumentoXUtenti").kendoPanelBar({
            expandMode: "multiple",
            select: function (e) {
                $("#panelbarCategTipolDocumentoXUtenti > li > span").addClass(GIAS_K_STATE_SELECTED);
            }
        });

        $("#panelbarCategTipolDocumentoXUtenti > li > span").addClass(GIAS_K_STATE_SELECTED);

        //Carico la griglia Utenti
        popolaGridcategtipodocxut_UC_utenti("categtipodocxut_UC_griglia_utenti");

        // Creazione Kendo Grid Categoria/Tipologia
        popolaGridcategtipodocxut_UC_griglia_CategTip("categtipodocxut_UC_griglia_CategTip");

        // Creazione Kendo Grid Categoria/TipologiaxUtenti
        popolaGridcategtipodocxut_UC_griglia_CategTipxUtenti("categtipodocxut_UC_griglia_CategTipxUtenti");
    }

    if (target === "a_tabSchemaDocumenti") {

        popolaGrigliaSchemaDocumenti("grdSchemaDocumenti");

    }
});