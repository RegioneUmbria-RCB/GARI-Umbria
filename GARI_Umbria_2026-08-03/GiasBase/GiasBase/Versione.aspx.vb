Imports System.IO

Namespace GiasBaseVersione

Public Class Versione
    Inherits System.Web.UI.Page

    Private Sub Carica_Pannello()

            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
            '                     CHI COMPILA
            '           SI PREOCCUPI DI VERIFICARE COSA SERVE:
            '                       NEI COM,
            '                       NEL MIGRA,
            '                       NEL CORE WS,
            '                       NEL WEB.CONFIG
            '§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

            Riga_Data("02 Marzo 2026")

            Riga_Text("Nuova versione Kendo",
                      "- Aggiunta versione Kendo 2025.4.1321")

            '==================================

            Riga_Data("20 Febbraio 2026")

            Riga_Text("Modulo Zootecnia",
                     "- Rimozione di tooltip centralizzato 'k-grid-Bdn' per permetterne la localizzazione e fix CSS per pagina di operazioni di scarico capi")

            '==================================

            Riga_Data("01 Dicembre 2025")

            Riga_Text("Nuovi loghi",
                     "- Aggiunti nuovi loghi per il sistema FoodMetaVerse")

            '==================================

            Riga_Data("07 Novembre 2025")

            Riga_Text("KendoGrid - Export Excel",
                     "- fix: rese le chiamate di export excel chunk (griglie con più di 10.000 righe) asincrone così non si blocca più la pagina, 
                             Esportate in excel solo le colonne visibili dell'utente, formattate le date")

            '==================================

            Riga_Data("03 Novembre 2025")

            Riga_Text("KendoWindow - grafica nuova",
                     "- fix: eliminato overflow:hidden che faceva sparire la barra di scorrimento laterale [#196111]")

            '==================================

            Riga_Data("26 Settembre 2025")

            Riga_Text("KendoGrid",
                     "- fix: salvataggio viste personalizzate #191588")

            '==================================

            Riga_Data("12 Settembre 2025")

            Riga_Text("Header",
                     "- fix: more specific form css selector for umbria custom header per #175333")

            '==================================

            Riga_Data("08 Settembre 2025")

            Riga_Text("Multiselect",
                     "- Fix in 'styleGiasComponents.css' per ripristinare height dinamica nelle multiselect dentro agli '.input-group'")

            '==================================

            Riga_Data("16 Luglio 2025")

            Riga_Text("Header",
                     "- fix: various bugs on umbria specific UI after header restyling per #175333, #176987")

            '==================================

            Riga_Data("07 Luglio 2025")

            Riga_Text("Sidemenu",
                     "Il menu laterale mantiene la tab selezionata ('Tutti i servizi' vs 'Prefetiti')")

            '==================================

            Riga_Data("17 Giugno 2025")

            Riga_Bug("Security",
                      "- Eliminate librerie di terze parti mai usate (jquery.jstree, jquery.multiple.select e jquery.treeselect)")

            Riga_Bug("Filtri KendoGrid",
                     "Modificata funzione onColumnMenuInit per consentire l'ordinamento degli elementi checkbox del filtro
                     (note: task 180080 Imostare un filtro e verificare che le checkbox siano ordinate)")

            '==================================

            Riga_Data("13 Giugno 2025")

            Riga_Bug("Security",
                      "Fix per evitare errore sicurezza nel salvataggio personalizzazioni gliglia kendo con sottocolonne (task 180184)")

            '==================================

            Riga_Data("30 Maggio 2025")

            Riga_Text("Loghi",
                     "- Inserito logo ""\Logo1005\Fileni.png"" per mostrare logo di Fileni in Homepage" &
                     "- Inserito logo ""\Logo1006\DeMatteis.png"" per mostrare logo di De Matteis in Homepage")

            '==================================

            Riga_Data("16 Maggio 2025")

            Riga_Bug("Filtri KendoGrid",
                     "Modificata funzione onColumnMenuInit per consentire che il pulsante seleziona tutti selezioni solo le checkbox precedentemente filtrate e non tutte le righe
                     (note: task 173869 Imostare un filtro testuale su una colonna, provare a premere seleziona tutto e vedere se seleziona solo le caselle precedentemetne filtrate e non tutti i record)")

            '==================================

            Riga_Data("14 Maggio 2025")

            Riga_Text("VetInfo",
                      "Aggiunta icona e classe css per bottone sincro VetInfo")

            '==================================

            Riga_Data("12 Maggio 2025")

            Riga_Bug("Filtri KendoGrid",
                     "Modificata funzione onColumnMenuInit per permettere di visualizzare le checkbox selezionate quando si preimposta un modello salvato nei report,
                     inoltre mostrate tutte le checkbox anche quelle non selezionate nelle griglie in cui c'è un'impostazione salvata, prima non era più possibile aggiungere altri checkbox a quelli del salvataggio
                     (note: task 173869 Provare a salvare un filtro di tipo checkbox, caricare il report con il modello salvato, verificare che i filtri vengano applicati correttamente e che 
                     selezionando il filtro per modificarlo sia possibile vedere i checkbox spuntati; testare anche che se in una griglia si è salvato l'impostazione della griglia con un filtro,
                     aprendo il filtro devono essere visibili sia le checkbox selezionate che quelle non selezionate in modo da poterne aggiungere altre altrimenti non è possibile modificare il filtro impostato di defualt;
                     testare in generale il funzionamento dei filtri nelle griglie in generale poiché questa modifica impatta su tutte le grigie)")

            Riga_Text("Header",
                      "Aggiunti css personalizzati per header Regione Umbria (Site1004.css e Site1004_NG.css) per renderlo più simile a New Agri")

            '==================================

            Riga_Data("06 Maggio 2025")

            Riga_Text("Security",
                      "Fix per evitare errore script injection nell'export excel server della kendo grid")

            '==================================

            Riga_Data("30 Aprile 2025")

            Riga_Text("Security funzioniComuniKendoGrid",
                      "Fix parametri griglia per evitare errore script injection nell'export excel lato server")

            '==================================

            Riga_Data("14 Aprile 2025")

            Riga_Text("Header Gias", "Aggiornata gestione valore chiave in configurazione siti 'AgronicaSpecialNavigationCFG' per la gestione dell'apertura del Gias in IFrame, in una nuova tab, normalmente")

            '==================================

            Riga_Data("07 Aprile 2025")

            Riga_Text("Login",
                     "Modificato css pagine index e recupera credenziali per aggiornamento a Bootstrap 5")

            '==================================

            Riga_Data("17 Marzo 2025")

            Riga_Text("Security",
                     "Rimosse librerie vecchie non richiamate: html5shiv.js, json.js, linq.js")

            '==================================

            Riga_Data("21 Febbraio 2025")

            Riga_Bug("Reportistica zoo",
                     "Aggiunto padding destro per ddl e commentato blocco che imponeva label sulla stessa riga nei selettori nella maschera reportistica zoo")

            '==================================

            Riga_Data("17 Febbraio 2025")

            Riga_Bug("Piani campionamento",
                     "Modifiche al foglio di stile per correggere l'aspetto grafico del menu kendo nei PDC")

            '==================================

            Riga_Data("31 Gennaio 2025")

            Riga_Bug("Analisi zootecniche",
                     "Modificato CSS, corregge bug che sostituiva l'icona di modifica classica con l'icona dei prelievi")

            '==================================

            Riga_Data("24 Gennaio 2025")

            Riga_Text("Schermata Login",
                     "Aggiunto tra le risorse il logo di Credit Agricole")

            '==================================

            Riga_Data("18 Dicembre 2024")

            Riga_Text("Security - Nomi Report",
                     "Aggiunta funzione centralizzata per rimuovere caratteri a rischio SQL, JS e HTMl injection quando si assegna un nome ai report")

            '==================================

            Riga_Data("11 Dicembre 2024")

            Riga_Text("Anagrafiche zoo",
                     "incluse icone nella soluzione per importazione modelli 4 e sincronizzazione vetinfo")

            '==================================

            Riga_Data("06 Dicembre 2024")

            Riga_Text("Anagrafiche zoo",
                     "Modificate icone per importazione modelli 4 e sincronizzazione vetinfo")

            '==================================

            Riga_Data("15 Novembre 2024")

            Riga_Bug("Anagrafiche zoo",
                     "Modificato CSS, corregge bug che nascondeva il pulsante del calendario nel campo VALIDI ALLA DATA")

            '==================================

            Riga_Data("21 Ottobre 2024")

            Riga_Text("CSS styleGiasPages",
                      "- Generalizzati i selettori per la classe 'gias-btn-square-filter' per aggiunta di pulsanti alla pagina Doc Contabili di Agenda che la utilizzano")

            '==================================

            Riga_Data("11 Ottobre 2024")

            Riga_Text("Anagrafiche zoo - stalle",
                      "Aggiunta icona per importazione singola modelli 4")

            Riga_Text("Kendo DDL Server Filtering",
                      "Aggiunto parametro opzionale per personalizzare messaggio in assenza di dati (noDataTemplate)")

            Riga_Text("Loghi",
                      "Aggiunti loghi personalizzati per Regione Umbria")

            '==================================

            Riga_Data("03 Ottobre 2024")

            Riga_Text("Kendo Input Zoo - Operazioni Scarico",
                      "- Cambiato padding tra i controlli di input nell'operazione di scarico.")

            '==================================

            Riga_Data("20 Settembre 2024")

            Riga_Text("Kendo Input Vari",
                      "- Introdotta nuova funzione ServerFiltering completa")

            '==================================

            Riga_Data("19 Settembre 2024")

            Riga_Text("Kendo salva personalizzazioni griglia",
                      "- Fix su salvataggio parametri griglia per evitare script injection")

            '==================================

            Riga_Data("30 Agosto 2024")

            Riga_Text("Kendo drop down list",
                     " - Aggiunto paramentro virtual al creadropdownlist 
                            - Aggiunta funzione per settare il valore delle ddl virtual
                            - Aggiunto optionLabel per l'elemento di default")

            Riga_Text("Kendo multiselect",
                      "Aggiunto paramentro virtual creaKendoMultiselect")

            '==================================

            Riga_Data("27 Agosto 2024")

            Riga_Text("Header/menu nuovi",
                      "- Eliminato il waitframe.hide nelle chiamate di header e menu")

            '==================================

            Riga_Data("23 Agosto 2024")

            Riga_Text("Restyle grafico",
                      "- Stile per bottone elenco trattamenti")

            '==================================

            Riga_Data("08 Agosto 2024")

            Riga_Bug("Restyle grafico",
                      "- Fix su Restyle pagine Zootecnia")

            '==================================

            Riga_Data("15 Luglio 2024")

            Riga_Text("Restyle grafico",
                      "- Restyle pagine Zootecnia")

            '==================================

            Riga_Data("14 Giugno 2024")

            Riga_Text("Segnalazioni SonarQube",
                      "- SecurityHotSpot: Weak Cryptography <code>jquery.innerfade.js</code> (File rimosso perché vecchio e non usato da nessuno)" &
                      "- Fix su CSS Restyle grafico")

            Riga_Text("Restyle grafico",
                      "- Fix pagine DSS")

            Riga_Text("MaskedTextbox Coordinate",
                      "Aggiunta MaskedTextbox per coordinate con due formati (DEC e GMS)")

            '==================================

            Riga_Data("17 Maggio 2024")

            Riga_Text("Segnalazioni SonarQube",
                      "- Web:S5256: Fix a pagina Versione.aspx" &
                      "- javascript:S3923: All branches in a conditional structure should not have exactly the same implementation su funzioniComuniKendoGrid.js" &
                      "- javascript:S930: Function calls should not pass extra arguments su funzioniComuniKendoChart.js" &
                      "- vbnet:S6145: Option Strict On su progetto" &
                      "- css:S4656: Properties should not be duplicated su legend-style.css" &
                      "- css:S4670: Selectors should be known su legend-style.css" &
                      "- css:S4656: Unexpected duplicate ""border-radius"" su css Kendo 2023.2.606" &
                      "- SecurityHotSpot: Encryption of Sensitive Data su js leaflet-openweathermap" &
                      "- javascript:S1534: Duplicate key 'maximumAge' su AgroMeteo/weather.js" &
                      "- javascript:S3923: All branches in a conditional structure should not have exactly the same implementation su headerMenuUC.js" &
                      "- javascript:S930: Function calls should not pass extra arguments su headerMenuUC.js" &
                      "- javascript:S905: Non-empty statements should change control flow or have at least one side-effect su Header/script.js" &
                      "- css:S4670: Selectors should be known su headers.css")

            '==================================

            Riga_Data("09 Maggio 2024")

            Riga_Text("Segnalazioni SonarQube",
                      "- SecurityHotSpot: link Dejavu font in http in funzioniComuniKendoEditor.js" &
                      "- css:S4670: Unexpected unknown type selector ""buttont""" &
                      "- javascript:S3403: Strict equality operators should not be used with dissimilar types in funzioniComuniKendoInputVari.js")

            '==================================

            Riga_Data("06 Maggio 2024")

            Riga_Text("Restyle Grafico",
                     "Pagina menù stampe bootstrap")

            '==================================

            Riga_Data("26 Aprile 2024")

            Riga_Text("Aggiunta machinekey.config",
                     "Aggiunta machinekey.config al GiasBase")

            '==================================

            Riga_Data("17 Aprile 2024")

            Riga_Bug("Predisposizione nuovo menù Manualistica",
                     "Modificato CSS + aggiunta icona per nuovo menù Manualistica di primo livello")

            '==================================

            Riga_Data("15 Aprile 2024")

            Riga_Bug("Restyle grafico",
                     "- Update dark grey table color")

            '==================================

            Riga_Data("10 Aprile 2024")

            Riga_Text("Icone",
                     "Aggiunta icona upload")

            '==================================

            Riga_Data("26 Marzo 2024")

            Riga_Text("Mattoncini",
                     "Riabilitato i mattoncini su grafica Coldiretti")

            '==================================

            Riga_Data("12 Marzo 2024")

            Riga_Text("Colori",
                     " - aggiunto colore verdescuro")

            Riga_Text("Restyle Grafico",
                     "- Rifacimento pagine DSS")

            Riga_Bug("Restyle Grafico",
                     "- Fix a stili personalizzati Demetra")

            '==================================

            Riga_Data("29 Febbraio 2024")

            Riga_Bug("Modifiche Griglia k-selected",
                     "Fix colorazione righe alternate selezionate (quando combo grafica vecchia + nuovo header)")

            '==================================

            Riga_Data("23 Febbraio 2024")

            Riga_Bug("Restyle Grafico",
                     "- Update Print icon in GestioneMagazzini/GestioneMagazziniBS.aspx")

            '==================================

            Riga_Data("15 Febbraio 2024 BIS")

            Riga_Bug("Modifiche kendo dll",
                     "- FIX aggiunto groupField e groupTemplate come parametri per creazione di una kendo dll, modifiche fatte dalla versione 2022.3.1109 in poi")

            Riga_Bug("Personalizzazione Demetra",
                     "- Aggiornamento righe tabella colorazione in quaderno di campagna Angular")

            '==================================

            Riga_Data("14 Febbraio 2024")

            Riga_Text("Modifiche kendo dll",
                     " - aggiunto groupField e groupTemplate come parametri per creazione di una kendo dll, modifiche fatte dalla versione 2022.3.1109 in poi")

            Riga_Text("Regione Umbria",
                      "Modifica logo Gari Umbria")

            '==================================

            Riga_Data("01 Febbraio 2024")

            Riga_Bug("Restyle Grafico",
                     "Fix a pulsante su pagina filtrone per confermare selezione elementi")

            '==================================

            Riga_Data("23 Gennaio 2024")

            Riga_Text("Kendo Grid",
                      "Ordinamento alfabetico dei valori nei filtri Multicheck ")

            Riga_Text("Regione Umbria",
                      "Aggiunti loghi personalizzati per regione Umbria")

            '==================================

            Riga_Data("11 Gennaio 2024")

            Riga_Text("Kendo 2023",
                      "- Fix stili post-migrazione")

            '==================================

            Riga_Data("20 Dicembre 2023")

            Riga_Text("Stile personalizzato Coldiretti/Demetra",
                      "- Aggiornamento fogli di stile personalizzati per DotNet e Angular")

            '==================================

            Riga_Data("06 Dicembre 2023")

            Riga_Bug("Kendo 2023",
                      "- Fix Checkbox invisibile con grafica vecchia (no test assistenza, solo problema in sviluppo)")

            Riga_Text("Stile personalizzato Coldiretti/Demetra",
                      "- Aggiornamento fogli di stile personalizzati per DotNet")

            '==================================

            Riga_Data("29 Novembre 2023")

            Riga_Text("Restyle Grafico e NG",
                      "- Aggiunti file per sovrascrittura stili personalizzati ambiente Coldiretti (Site1000)")

            Riga_Text("Restyle Grafico",
                      "- Stilizzazione notifiche" &
                      "- Update icon for import content from app")

            '==================================

            Riga_Data("22 Novembre 2023")

            Riga_Bug("Header nuova",
                      "- Fix pulsante di ricerca gigante in pagine con grafica vecchia ma header nuovo")

            Riga_Text("Restyle Grafico",
                      "- Adattamenti per Kendo 2023")

            '==================================

            Riga_Data("06 Novembre 2023")

            Riga_Text("Restyle Grafico",
                      "- Aggiunta icone file type e modifica icone per salva ed esci e salva + nuovo documento + controllo di gestione" &
                      "- Stilizzazione Audit BIO + pagina checklist per global gap e ottimizzazioni minori stili pagine audit" &
                      "- Nuovi file stile per Kendo 2023 (WIP)")

            '==================================

            Riga_Data("26 Ottobre 2023")

            Riga_Text("Dashboard Header",
                      "- Aggiunto avviso per licenze e/o permessi scaduti, username e mail nel riquadro utente della header" &
                      "- Nascosti controlli di navigazione quando viene aperto da demetra")

            '==================================

            Riga_Data("19 Ottobre 2023")

            Riga_Text("Restyle Grafico",
                     "- Aggiornamento del filtro sul cedente nei conferimenti")

            '==================================

            Riga_Data("10 Ottobre 2023")

            Riga_Text("Restyle Grafico",
                     "- Aggiunta icona domanda irrigua al menu laterale" &
                     "- Sviluppo Header Fixed in ricerca DDT, tab bar in Anagrafiche CdC e fix modale nuova squadra" &
                     "- Aggiunta icona print bianca, sistemati gli input date, ottimizzati i pulsanti primary ed allineamento elementi dei form")

            '==================================

            Riga_Data("29 Settembre 2023")

            Riga_Text("Restyle Grafico",
                     "- Ri-organizzazione file css in multipli file separati <code>styleGiasComponents.css, styleGiasIcons.css, styleGiasPages.css, styleGiasUtils.css</code> anzichè l'unico file <code>styleXonneTables.css</code>")

            '==================================

            Riga_Data("05 Settembre 2023")

            Riga_Text("Restyle Grafico",
                     "- Stilizzazione pagina Configurazione Servizi")

            '==================================

            Riga_Data("29 Agosto 2023")

            Riga_Bug("Restyle Grafico",
                     "- Fix visualizzazione bottoni toolbar griglie annidate" &
                     "- Ottimizzazione filtri menu laterale")

            '==================================

            Riga_Data("24 Agosto 2023")

            Riga_Bug("Raggruppamento colonne kendoGrid",
                     "- Fix raggruppamento delle colonne nelle griglie, 
                              se si aggiungeva un footer o header al raggruppamento veniva contanto come dataItem 
                              e andava in errore essendo undefined ")

            Riga_Bug("Restyle Grafico",
                     "- Fix anagrafica zoo - sezione stalle, bottone per sincronizzazione VetInfo" &
                     "- Fix bug grafico puntatore nel menu" &
                     "- Minor fix icone refresh filtrone albero aziende")

            Riga_Text("Restyle Grafico",
                     "- Miglioramenti menu di navigazione laterale" &
                     "- Ottimizzazione menu laterale con visualizzazione submenu in apertura")

            '==================================

            Riga_Data("31 Luglio 2023")

            Riga_Requisiti("AGENDA",
                           "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Bug("Fix cambio azienda",
                     " - Fixxato bug che impediva il cambio dell'azienda in determinate casistiche")

            '==================================

            Riga_Data("19 Luglio 2023")

            Riga_Requisiti("AGENDA",
                           "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Bug("Fix ddl_editor in kendoGrid",
                     " - Fix ddl editor per kendoGrid: passando false al parametro required,
                               la ddl potrà essere chiusa senza bloccare tutto finchè non si inserisce un valore; 
                               se si passa true o non si passa nulla, il campo diventerà required in automatico")

            Riga_Text("Nuova versione Kendo + Nuovo stile",
                      "- Aggiunto file specifico styleXonneTables2023.css da caricare in presenza di nuovo stile grafico + Kendo con versione >= 2023")

            '==================================

            Riga_Data("15 Giugno 2023")

            Riga_Requisiti("AGENDA",
                           "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Bug("Restyle grafico:",
                     "- Fix stili pagina report vendite-acquisti, fix bottone squadra in controllo gestione")

            Riga_Text("Nuova versione Kendo",
                      "- Aggiunta versione Kendo 2023.2.606 e cancellata versione 2021.3.1109")

            '==================================

            Riga_Data("09 Giugno 2023")

            Riga_Requisiti("AGENDA",
                           "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Bug("Restyle Grafico",
                     "- Fix anagrafica zoo - sezione stalle, bottoni per sincronizzazione BDN"&
                     "- Modifiche a MenuBS Piano Concimazione" &
                     "- Modifiche a PCB Inserimento per restyle ASP Checkbox in Switch")

            '==================================

            Riga_Data("31 Maggio 2023")

            Riga_Requisiti("AGENDA",
                           "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Bug("Restyle Grafico DotNet",
                      "- Fix documenti contabili sezione dati economici e box peso")

            '==================================

            Riga_Data("23 Maggio 2023")

            Riga_Requisiti("AGENDA",
                           "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyle Grafico DotNet",
                      "- Miglioramento stili pagine cdg e filtri menu laterale cdg" &
                      "- Stilizzazione pagina configurazione lavorazioni")

            '==================================

            Riga_Data("12 Maggio 2023")

            Riga_Requisiti("AGENDA",
                           "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Loghi",
                      "- Aggiunto nuovo logo personalizzato per Coprob")

            Riga_Text("Restyle Grafico DotNet",
                      "- Filtri tramite sidebar in magazzini e scadenziario e ricerca doc contabili" &
                      "- Fix conferimento box 'Peso a pagamento kg'" &
                      "- Refactory classi stili utility")

            Riga_Text("GiasBase", "Eliminati tutti i riferimenti fissi al GiasBase (/GiasBase/... etc nel codice - lettura parametrizzata tramite chiave in configurazione siti")

            '==================================

            Riga_Data("21 Aprile 2023")

            Riga_Requisiti("AGENDA",
                           "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyle Grafico DotNet",
                      "- Adattamento menu laterale e header al nuovo logo verde" &
                      "- Fix stile righe selezione in list, fix colori righe stato verifica conformità e fix padding container principale" &
                      "- Stilizzati campi obbligatori" &
                      "- Aggiunta di diverse icone aggiornate per i bottoni" &
                      "- Stilizzazione pagina Inserimento costi" &
                      "- Stilizzazione pagina Nuova Visita")

            '==================================

            Riga_Data("31 Marzo 2023")

            Riga_Requisiti("AGENDA",
                           "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyle Grafico DotNet",
                      "- Completata stilizzazione irrigazione e verifica conformità" &
                      "- Completata stilizzazione ricerca documenti conferimento" &
                      "- Allineati colori tooltip con versione angular" &
                      "- Aggiunto nuovo loader")

            '==================================

            Riga_Data("17 Marzo 2023")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Header + Dashboard",
                      "- Gestione Spegnimento header su menu aperti in popup" &
                      "- Gestione Spegnimento sidebar su nuova finestra" &
                      "- Gestione Scroll su ricerca aziende con molti risultati" &
                      "- Localizzazione testi non localizzati")

            Riga_Text("Integrazione BDN", " Icone per ingressi e uscite BDN in anagrafica stalle ")

            Riga_Text("Restyle Grafico DotNet",
                      "- Aumento del contrasto nei bordi degli input" &
                      "- Cambio logo visibile in pagina login (sia grafica vecchia che nuova)")

            '==================================

            Riga_Data("10 Marzo 2023")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Bug("Restyle Grafico ",
                      "- Fix icone aggiunta e modifica contatto dentro al Doc Contabile" &
                      "- Allineata icona annulla modifiche in modifica riga table con icone angular")

            '==================================

            Riga_Data("06 Marzo 2023")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text(" Restyle Grafico ",
                      "- Aggiunto file vuoto agronica/styles/styleXonneLogin.css ")

            Riga_Text(" Dahsboard (Header + Sidebar) ",
                      "- Gestione Apertura menu di tipo 1 (nuova scheda) ")

            '==================================

            Riga_Data("21 Febbraio 2023")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyle Grafico",
                      "- Modifiche a stile grafico .net")

            Riga_Bug("Header",
                      "- fix colore icona se non presente (ora è nero come la scritta così è visibile)")

            '==================================

            Riga_Data("10 Febbraio 2023")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyle Grafico",
                      "- Aggiunte icone")

            Riga_Text("Kendo Grid",
                      "- kendo_AggiustaDimensioneColonne aggiusta correttamente anche le dimensioni delle colonne gerarchiche")

            Riga_Bug("Restyle Grafico",
                      "- Fix icona conferma edit in riga di griglia kendo")

            Riga_Text("Dashboard",
                      "- Cambio icone x Icone colorate")

            Riga_Text("Dashboard",
                      "- Restyling breadcrumbs")

            '==================================

            Riga_Data("27 Gennaio 2023")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyle Grafico",
                      "- Modifiche a stile grafico .net  header + sidebar + breadcrumbs")

            '==================================

            Riga_Data("16 Gennaio 2023")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyle Grafico",
                      "- Modifiche a stile grafico .net e header")

            '==================================

            Riga_Data("22 Dicembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyle Grafico",
                      "- Modifiche a stile grafico .net")

            Riga_Text("Breadcrumbs",
                      "- Fix breadcrumbs tra passaggio NG e Dot.NET")

            Riga_Text("Kendo",
                      "- Impostata a default la versione 2022.3.1109 per tutti")

            Riga_Text("Vecchie versioni Kendo",
                      "- Cancellata versione 2022.3.913")

            '==================================

            Riga_Data("13 Dicembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Bug(" Header",
                     "- Reso non clickabile e tolta la freccia overlay utente. Fatto in modo che la sidebar vada sopra il resto quando aperta. ")

            '==================================

            Riga_Data("06 Dicembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyle Grafico",
                     "- Modifiche a foglio di stile 2022")

            Riga_Bug(" Dashboard ",
                     "- Fix Breadcrumbs / Fix spazio disabilitato su ricerca menu ")

            Riga_Bug(" Dashboard ",
                     "- Fix perdita figli in albero menu dopo ricerca ")

            '==================================

            Riga_Data("02 Dicembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Bug("Kendo 2022",
                     "- fix kendoFastRedrawRow in presenza di griglia gerarchica per cui venivano traslati i contenuti delle varie colonne")

            Riga_Text("Restyle Grafico",
                     "- Nuovo stile grafico .net + header")

            '==================================

            Riga_Data("28 Novembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Vecchie versioni Kendo",
                      "- Cancellata versione 2021.1.119")

            Riga_Text("Modifiche propedeutiche a css per nuova dashboard",
                      "- cambiato id contenitore_principale in master_contenitore_principale")

            '==================================

            Riga_Data("16 Novembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Restyling UX/UI",
                      "- Aggiornamento stylesheet")

            Riga_Bug("Kendo (2022.3.913 e 2022.3.1109)",
                      "- Fix opacity per pulsanti disabilitati in header griglie dentro a tabstrip per cui non era più visibile il pulsante 'Elimina personalizzazioni griglia'")

            '==================================

            Riga_Data("11 Novembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Nuova versione Kendo",
                      "- Aggiunta versione Kendo 2022.3.1109 e cancellata versione 2022.2.802")

            Riga_Text("Kendo (solo 2022.3.1109)",
                      "- Modifiche ai file kendo.common-bootstrap.min.css e kendo.bootstrap.min.css per rimuovere i riferimenti alla vecchia classe k-state-active nella tabstrip " &
                      "per evitare che rimanga visivamente selezionata la tab pre-settata come attiva prima dell'inizializzazione")

            Riga_Text("Restyling UX/UI",
                      "- Aggiunti file per nuova header e dashboard")

            '==================================

            Riga_Data("04 Novembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Kendo Grid (solo 2022.3.913)",
                     "- Modifiche in previsione del nuovo stile per organizzare diversamente i pulsanti in testata e togliere le scritte da export in pdf e Excel")

            Riga_Text("Kendo (solo 2022.3.913)",
                     "- Modifiche al file kendo.common-bootstrap.min.css per rimuovere i riferimenti alla vecchia classe k-state-active nella tabstrip " &
                     "per evitare che rimanga visivamente selezionata la tab pre-settata come attiva prima dell'inizializzazione")

            '==================================

            Riga_Data("28 Settembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Nuova versione Kendo",
                      "- Aggiunta versione Kendo 2022.3.913 e cancellata versione 2022.2.510")

            Riga_Bug("Kendo Grid",
                     "- Corretto il funzionamento della griglia a scorrimento infinito")

            '==================================

            Riga_Data("01 Settembre 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Text("Nuova versione Kendo",
                      "Aggiunta versione Kendo 2022.2.802")

            Riga_Text("Kendo versioni 2021.3.1109 | 2022.2.510 | 2022.2.802",
                      "Introdotte costanti generali per gli stati")

            Riga_Text("Kendo Grid",
                      "Aggiunta funzione centralizzata rowKendoGridSelected")

            Riga_Text("Modifiche funzionalità funzioni condivise Kendo 2022.2.802",
                      "La funzione che restituisce il valore di una ddl è stata aggiornata")

            Riga_Text("Kendo versioni 2022.2.510 | 2022.2.802",
                      "Vari fix grafici su css/js per breaking changes")

            '==================================

            Riga_Data("12 Agosto 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Text("Corretti alcuni problemi rilevati dopo l'aggiornamento di Kendo",
                      "- Aggiornamento Kendo 2021.3.1109 -> 2022.2.510")

            Riga_Text("Kendo Slider",
                      "- Aggiunte funzioni per gestire il controllo slider in funzioniComuniKendoInputVari.js delle versioni 2021.3.1109 e 2022.2.510")

            '==================================

            Riga_Data("22 Luglio 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Text("Kendo CSS",
                      "Aggiunto stile per colorare la riga di verde")

            '==================================

            Riga_Data("08 Luglio 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Text("funzioniComuniKendoInputVari.js",
                      "Nuova funzione KendoMultiColumnComboBox per versioni 2021.3.1109 e 2022.2.510")

            '==================================

            Riga_Data("01 Luglio 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Text("Gias_Kendo.css",
                      "Corretto CSS kendo che con la versione 2021.3.1109 rendeva il testo blu dentro i panel")

            '==================================

            Riga_Data("3 Giugno 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Text("Nuova versione Kendo",
                      "Aggiunta versione Kendo 2022.2.510 e cancellata versione 2020.3.1118")

            Riga_Text("Versione kendo effettiva:",
                      "Resa effettiva la 2021.3.1109 al posto della 2021.1.119")

            Riga_Text("Aggiunto fileSaver.js ",
                      "Aggiunto fileSaver.js per scaricamento file zip (create con il kendoUpload nel documentale)")

            '==================================

            Riga_Data("13 Maggio 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Text("L'aspetto del menu che gestisce i filtri della griglia è stato aggiornato",
                      "Ora il testo si avvolge a una nuova riga, togliendo il bisogno di scorrere a destra.")

            Riga_Bug("Ridimensionamento delle colonne nella griglia:",
                     "- Modifiche per evitare ri-lettura griglia")

            '==================================

            Riga_Data("04 Aprile 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Text("L'aspetto del menu che gestisce i filtri della griglia è stato aggiornato",
                      "Ora il testo si avvolge a una nuova riga, togliendo il bisogno di scorrere a destra.")

            Riga_Bug("Ridimensionamento delle colonne nella griglia.",
                     "Ora le colonne dinamiche vanno considerate quando queste vengono ripristinate.")

            '==================================

            Riga_Data("24 Marzo 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Text("L'aspetto del menu che gestisce i filtri della griglia è stato aggiornato",
                      "Ora il testo si avvolge a una nuova riga, togliendo il bisogno di scorrere a destra.")

            '==================================

            Riga_Data("11 Marzo 2022")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Text("Larghezza fissa per i pulsanti dei filtri della griglia:",
                  "Ora i pulsanti sono visibili sempre, nonostante la larghezza dei risultati nei filtri")

            Riga_Text("Salvataggio della larghezza di ogni colonna di una griglia",
                  "Al salvataggio di una vista (personalizzazione griglia) la larghezza di ogni colonna è conservata")

            '==================================

            Riga_Data("6 Dicembre 2021")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Bug("funzioniComuniKendoGrid:",
                  "Bugfix sulle modifiche del 3/12/2021 per gestire dataItem.id contenenti i chrs: / ^ ~ ` & = ' , ; { } |")

            '==================================

            Riga_Data("3 Dicembre 2021")

            Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

            Riga_Bug("funzioniComuniKendoGrid:",
                  "Correzione per gestire dataItem.id contenenti i chrs: / ^ ~ ` & = ' , ; { } |")

            '==================================

            Riga_Data("29 Novembre 2021")

            Riga_Requisiti("AGENDA",
                    "Gestione pagina GIS", "05 Maggio 2020")

            Riga_Requisiti("SMART",
                    "Per GIS", "01 Marzo 2018")

            Riga_Bug("funzioniComuniKendoGrid:",
                """Correzione per gestire dataItem.id contenenti la /"" va in errore se la griglia è senza chiave (frequente sulle griglie in sola lettura)")

            '==================================

            Riga_Data("19 Novembre 2021")

            Riga_Requisiti("AgronicaCore:",
                       "")

        Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

        Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoInputVari:",
                  "Aggiunto parametro opzionale autoWidth in creazione ddl")

        Riga_Text("Versione Kendo:",
                  "Aggiunta versione 2021.3.1109")

        Riga_Text("Versione Kendo:",
                  "Cancellata la versione 2020.2.617")

        Riga_Text("Logo:",
                  "Aggiunta favicon.svg")

        '==================================

        Riga_Data("3 Novembre 2021")

        Riga_Requisiti("AgronicaCore:",
                       "")

        Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

        Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Bug("funzioniComuniKendoGrid:",
                  "Correzione per gestire dataItem.id contenenti la /")

        '==================================

        Riga_Data("25 Giugno 2021")

        Riga_Requisiti("AgronicaCore:",
                       "")

        Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

        Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoGrid:",
                  "Aggiunta funzione dateTimeEditor per generare un editor con kendoDateTimePicker")

        '==================================

        Riga_Data("26 Maggio 2021")

        Riga_Requisiti("AgronicaCore:",
                       "")

        Riga_Requisiti("AGENDA",
                       "Gestione pagina GIS", "05 Maggio 2020")

        Riga_Requisiti("SMART",
                       "Per GIS", "01 Marzo 2018")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Bug("funzioniComuniKendoGrid:",
                  "Fix recupero fieldName in onEditKendoGrid ")


        '==================================

        Riga_Data("30 Marzo 2021")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Controlli_form:",
                  "corretto baco gestione sessionStorage")

        '==================================
        '==================================

        Riga_Data("26 Marzo 2021")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Controlli_form:",
                  "gestione sessionStorage")

        '==================================
        '==================================

        Riga_Data("22 Febbraio 2021")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Versione Kendo:",
                  "Aggiunta versione Kendo 2021.1.119")

        '==================================

        Riga_Data("17 Febbraio 2021")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Export griglia Kendo su Excel:",
                  "Correzione per test su colonne numeriche")

        '==================================

        Riga_Data("5 Febbraio 2021")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Export griglia Kendo su Excel:",
                  "Modifica per inserire un apice all'inizio della cella se si trovano i caratteri = + - @ e la cella non contiene un numero")

        '==================================

        Riga_Data("28 Gennaio 2021")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Export griglia Kendo su Excel:",
                  "ROLLBACK Modifica per sostituire i caratteri = + - @ con '' ma permettere comunque l'export")

        '==================================

        Riga_Data("27 Gennaio 2021")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Export griglia Kendo su Excel:",
                  "Modifica per sostituire i caratteri = + - @ con '' ma permettere comunque l'export")

        '==================================

        Riga_Data("25 Gennaio 2021")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Export griglia Kendo su Excel:",
                  "Introdotta non possibilità di esportare su Excel campi che o controllo che i campi non inizino con = + - @ ")

        Riga_Text("kendoNumericTextBox in Griglia e campi singoli: ",
                  "Abilitato selectOnFocus con default a true")

        '==================================

        Riga_Data("26 Novembre 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Versione Kendo:",
                  "Aggiunta versione Kendo 2020.3.1118 e cancellata versione 2020.1.114 ")

        '==================================

        Riga_Data("07 Settembre 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoGrid:",
                  "Aggiunto templateIdControllo a colonneKendoGrid per poter fare riferimento ad un id di tipo template anche quando si crea la grid lato server ")

        '==================================

        Riga_Data("10 Agosto 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoGrid:",
                  "Aggiunto flag menuColonneAdattivo alle funzioniCRUD per autoadattamento contextMenu di scelta colonne  ")

        '==================================

        Riga_Data("3 Luglio 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Rilascio nuova versione:",
                  " 2020.2.617 ")

        '==================================

        Riga_Data("22 Maggio 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoGrid:",
                  " Filtro 'tipo excel' in griglia kendo con la presenza di seleziona tutti ")

        '==================================


        Riga_Data("05 Maggio 2020 BIS")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA 05 Maggio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS:",
                  " Mappatura Smart")

        '==================================

        Riga_Data("05 Maggio 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '21 Febbraio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("KendoGrid:",
                  " implementato filtro tipo excel con seleziona tutti gli elementi filtrati ")

        Riga_Text("Loghi:",
                  " Aggiunto nuovo logo Gias per login ")

        '==================================

        Riga_Data("21 Aprile 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '21 Febbraio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS:",
                  " Gestione flag modalità mobile ")

        '==================================

        Riga_Data("16 Aprile 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '21 Febbraio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoGrid:",
                  "fix template su colonna con checkbox di selezione anche su versione 2019.3.917")

        '==================================

        Riga_Data("08 Aprile 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '21 Febbraio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoInputVari:",
                  "nuova funzione KendoSwitch che restituisce il controllo Kendo Switch")

        Riga_Text("funzioniComuniKendoGrid:",
                  "nuova funzione KendoGrid che restituisce il controllo Kendo Grid")

        '==================================

        Riga_Data("31 Marzo 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '21 Febbraio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoInputVari:",
                  "passare un boolean che indichi che se il kendoNumericTextBox è null venga passato indietro 0")

        Riga_Text("js_demo_tiles.js:",
                  " In modalità Smartphone ora viene associato un Punto da GPS (richiesta REI Progetti) ")

        '==================================

        Riga_Data("23 Marzo 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '21 Febbraio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS:",
                  " In modalità Smartphone ora viene associato un Punto da GPS (richiesta REI Progetti) ")

        '==================================

        Riga_Data("21 Febbraio 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '21 Febbraio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoGrid:",
                  " fix template su colonna con checkbox di selezione ")

        Riga_Text("GIS:",
                  " Internazionalizzazione su js ")

        '==================================

        Riga_Data("06 Febbraio 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '14 Gennaio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("funzioniComuniKendoInputVari:",
                  " Aggiunti parametri template e valueTemplate su creaKendoDropDownList ")

        '==================================

        Riga_Data("14 Gennaio 2020")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '14 Gennaio 2020' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS:",
                  " Rilascio funzionalità nuovo impianto da appezzamento selezionato e nuova toolbar che raggruppa gli strumenti di disegno. ")

        '==================================

        Riga_Data("13 Dicembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '09 Ottobre 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" GIS:",
                  " Nuova Gestione Agenda con linguetta in pagina gis che collega in iFrame il menu agenda ")

        '==================================

        Riga_Data("30 Novembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '09 Ottobre 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" KendoGrid Input Vari:",
                  " Introdotta setKendoSwitchVisible ")

        '==================================

        Riga_Data("25 Novembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '09 Ottobre 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" Gis - Precision Farming:",
                  " Integrazione su lettura dei dati Sentinel in fase di generazione di una mappa a rateo Variabile")

        Riga_Text(" Gis:",
                  " Pulizia su variabili non + utlizzate.")

        Riga_Text(" Gis:",
                  " Sentile2 - Parto dal livello 14 sulle mappe dettagliate a seguito di rimozione di tale livello per via dello spazio a disposizione ")

        Riga_Text(" Gis:",
                  " Sentinel2, grafico con ndvi et al: mese evidenziato in dettagli e fondoscala ricalcolato in base a numero di date lette ")
        '==================================

        Riga_Data("09 Ottobre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '09 Ottobre 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")
        Riga_Text("GIS: ",
                  " in fase di modifica di un impianto esistente, dividere il salvataggio del poligono dal salvataggio delle informazioni di anagrafica")


        '==================================

        Riga_Data("07 Ottobre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '07 Ottobre 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS: ",
                  "In modifica di un impianto occorre effettuare una verifica nel caso di vada a cambiare la specie e questo deve essere impedito se ci sono operazioni di agenda (vedi anche modifica in anagrafica nuova)")

        '==================================

        Riga_Data("27 Settembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '20 Settembre 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS: ",
                  "Avanzamenti su catasto")

        '==================================

        Riga_Data("23 Settembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '20 Settembre 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS: ",
                  "Varie modifiche Sementieri per interferenze")

        '==================================

        Riga_Data("20 Settembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '20 Settembre 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")
        Riga_Text("Gis",
                  "Avanzamento su varie e riparto catasto. ")
        '==================================

        Riga_Data("18 Settembre 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '18 Settembre 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo Date Picker",
                  "Aggiunte funzioni KendoDate e KendoDateTime per selezionare il contenuto dei controlli")

        Riga_Text("Gis",
                  "Modifica dato analitico su particelle catastali partendo da GIS")

        '==================================

        Riga_Data("21 Agosto 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '19 Agosto 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Gestione GIS",
                  "Fix in finestra nuovo/modifica impianto per caricamento specie")

        '==================================

        Riga_Data("19 Agosto 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '19 Agosto 2019 bis' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo Grid: ",
                  "Fix su salvataggio personalizzazione griglia in caso di filtro su più colonne")

        Riga_Text("Kendo Date Picker",
                  "Aggiunto set_dataTime per il kendoDateTimePicker")

        Riga_Text("Kendo Input vari",
                  "Aggiunto possibilità di specificare proprietà autoBind in creaKendoDropDownList")

        Riga_Text("Versioni Kendo",
                  "Cancellate la 2018.3.911 e la 2019.1.220")

        Riga_Text("Gestione GIS",
                  "Ottimizzazioni e aggiornamento interfaccia")

        '==================================

        Riga_Data("05 Agosto 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '05 Agosto 2019' :",
                       "Gestione pagina GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        '==================================

        Riga_Data("31 Luglio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '12 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo Input Vari: ",
                  "Aggiunta creaKendoDropDownListServerFiltering utilizzata in nuova FormProdotto")

        Riga_Text("Gis: ",
                  "Prima versione gestione WMS")


        '==================================

        Riga_Data("26 Giugno 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '12 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo Grid: ",
                  "migliorata gestione evento su click colonna di selezione")

        '==================================

        Riga_Data("21 Giugno 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '12 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo: ",
                  "Aggiunta versione 2019.2.619")

        '==================================

        Riga_Data("07 Giugno 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '12 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo Grid Drop Down Editor: ",
                  "Creata nuova funzione creaDropDownEditorId che permette di specificare l'id del controllo risultante " &
                  "e la possibilità di forzare il binding sul value con data-bind=value: ...")

        '==================================

        Riga_Data("31 Maggio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '12 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo Multiselect: ",
                  "Impostato AutoClose = False di default per le multiselect")

        Riga_Text("Kendo Pivot: ",
                  "eliminato il messaggio 'Vuoi espandere i primi due livelli di righe e colonne' e la relativa elaborazione")

        Riga_Text("Kendo Grid: ",
                  "gestito parametro gestisciSalvataggioFinaleAParte per evitare che con l'edit inline o popup si sia costretti a salvare su db e ricaricare la griglia")

        '==================================

        Riga_Data("23 aprile 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '12 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS: ",
                  "Ripristino di alcune funzionalità a seguito di ultime modifiche")

        '==================================

        Riga_Data("17 aprile 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '12 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS: ",
                  "Gestione Sementieri primo rilascio")
        '==================================

        Riga_Data("12 aprile 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '12 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS: ",
                  "Gestito Passaggio della Superficie anche in fase di modifica di una pianificazione")

        '==================================

        Riga_Data("11 aprile 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '9 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS: ",
                  "Primo rilascio per gestione Sementieri")

        '==================================

        Riga_Data("10 aprile 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '9 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo:",
                  "Modifica DDL server filtering")

        Riga_Text("GIS: ",
                  "Modifiche di contorno")

        '==================================

        Riga_Data("9 aprile 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '9 Aprile 2019' :",
                       "Gestione pagina GIS per gestione enumerativo GISPurpose")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo:",
                  "Modifica DDL server filtering")

        Riga_Text("GIS: ",
                  "Gestione edit del planning (nuovo e modifica) in collaborazione con pagina sincro")

        '==================================

        Riga_Data("4 aprile 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo:",
                  "Modifica DDL server filtering")

        Riga_Text("GIS: ",
                  "Gestione sementieri (sportello, mappatura libera impianti in osservazione), Gestione modifica da planning, vari upgrade grafici")

        '==================================

        Riga_Data("3 aprile 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo:",
                  "Modifica DDL server filtering")

        '==================================

        Riga_Data("18 Marzo 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS:",
                  "Gestione Sementieri in Cartografia")

        '==================================

        Riga_Data("14 Marzo 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("bootstrap_AGRONICA.css:",
                  "portata in linea con quella più aggiornata in Agenda (commentato .nav.navbar-nav li a span)")

        '==================================

        Riga_Data("6 marzo 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Sementi:",
                       "GIS -> aggiunto controllo callback in AggiornaLayer")

        '==================================

        Riga_Data("5 marzo 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Aggiunta versione:",
                       "aggiunta versione 2019.1.220")

        '==================================


        Riga_Data("26 Febbraio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS:",
                       "Gias Base - GIS SAT - Aggiustato Grafico per visualizzare degnamente altri indici")

        Riga_Text("GIS:",
                       "Gias Base - Zoom sul bounding box dei persorcorsi selezionati in funzione ""tecnici in campo"" ")

        '==================================


        Riga_Data("14 Febbraio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("AgroMeteo:",
                       "Corretta visualizzazione dashboard + gestione errore")

        Riga_Text("GIS:",
                       "Gias Base - GIS SAT - Impostato un'aggiornamento automatico delle mappe satellitari in calendario dopo ""n"" secondi che la mappa è IDLE e per una distanza > ""x"" dopo ultimo evento idle")


        '==================================
        Riga_Data("13 Febbraio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")


        Riga_Text("Sementi:",
                       "Cartografia in sola lettura con visione solo delle interferenze")

        '==================================

        Riga_Data("07 Febbraio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")


        Riga_Text("GIS:",
                       "Affinamento feedback analisi dati satellitari")

        '==================================

        Riga_Data("05 Febbraio 2019 B")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")


        Riga_Text("GIS:",
                       "Verifiche su situazione analisi dati satellitari")
        '==================================

        Riga_Data("05 Febbraio 2019")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")
        Riga_Text("KendoPivotGrid:",
                       "Aggiunto parametro per passare l'impostazione predefinita dei filtri")

        Riga_Text("KendoInputVari:",
                       "Aggiunte funzioni retrocompatibili per gestire nuovo componente kendo Switch")

        '==================================

        Riga_Data("01 Febbraio 2019 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("KendoGrid:",
                       "- Aggiunto pulsante per cancellare le preferenze della griglia" &
                       "- Sostituito l'alert con il kendoAlert nei messaggi di conferma" &
                       "- Modifiche grafiche sui pulsanti della toolbar")

        Riga_Text("GIS:",
                       "Algoritmo per la gestione dei tiles in elaborazione dettagliata mappa satellitare")

        '==================================

        Riga_Data("30 Gennaio 2019 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo:",
                       "Modifica funzioniComuniKendoEditor e funzioniComuniKendoChart")

        '==================================

        Riga_Data("25 Gennaio 2019 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo:",
                       "Cancellazione vecchie versioni")

        Riga_Text("Gias_Kendo.css:",
                       "versionato in base alle release di Kendo")

        '==================================

        Riga_Data("23 Gennaio 2019 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Kendo:",
                       "Aggiunta versione 2019.1.115")

        Riga_Text("funzioniComuniKendoInputVari:",
                       "Aggiunto creaKendoMultiselectServerFiltering")

        '==================================

        Riga_Data("12 Dicembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '11 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Pagina GIS:",
                       "Mappa ed albero anagrafica - nuova versione (in progress)")

        '==================================

        Riga_Data("5 Dicembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '5 Dicembre 2018' :",
                       "Gestione pagina GIS per albero anagrafica")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Pagina GIS:",
                       "Mappa ed albero anagrafica")

        '==================================

        Riga_Data("29 Novembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Funzioni Comuni KendoGrid:",
                       "field name in onEditKendoGrid")

        '==================================
        '==================================

        Riga_Data("27 Novembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("AgronicaControlliGIS:",
                       "Aggiornata gestione voci menù da mappa")

        '==================================

        Riga_Data("23 Novembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Ultime 3 versioni Kendo' :",
                       "migliorato test campo corrente in onEditKendoGrid")

        Riga_Text("AgroMasterPage :",
                       "aggiunto js e css per gestione centralizzata nuovo menu GIAS")

        Riga_Text("AgroMasterPage :",
                       "aggiunto js e css per gestione centralizzata nuovo menu GIAS")

        Riga_Text("Kendo 2018.3.911 - funzioniComuniKendoInputVari:",
                       "aggiunto creaMultiColumnComboBoxEditor")


        '==================================

        Riga_Data("2 novembre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")


        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("AGENDA '6 settembre 2018' :",
                       "KendoGrid: aggiunta gestione di un parametriKendoGrid.headerAttributes per definire lo stile delle colonne aggiunte (selected e cancella); utilizzato al momento solo nella griglie detail inserimento ore")

        Riga_Text("Kendo Grid ultime 3 versioni' :",
                       "Aggiunta parametriKendoGrid.lockCancella per gestire il fatto che la colonna di cancellazione aggiunta in automatico sia Locked")

        Riga_Text("Kendo Grid ultime 3 versioni' :",
                         "Gestione pulsante Elimina tutti i filtri")

        Riga_Text("Kendo Grid ultime 3 versioni' :",
                         "modificata kendoFastRedrawRow per gestire colonne con doppia intestazione. Aggiunto parametro value in creaDropDownEditorServerFiltering")

        Riga_Text("Kendo Grid ultime 3 versioni' :",
                         "Modifica onEditKendoGrid per testare il nome del campo cambiato per ogni tipo di colonna")

        Riga_Text("Kendo Input vari ultime 3 versioni' :",
                         "Modifica DDL ServerFiltering per gestire evento close e per rimuovere le righe dal dataSource all'open della DDL")

        '==================================
        Riga_Data("23 Ottobre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")


        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("AGENDA '6 settembre 2018' :",
                       "KendoGrid: aggiunta gestione di un parametriKendoGrid.headerAttributes per definire lo stile delle colonne aggiunte (selected e cancella); utilizzato al momento solo nella griglie detail inserimento ore")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")


        Riga_Text("Gis:",
                  "Correzioni per API Google 3.34")

        '==================================
        Riga_Data("17 Ottobre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")


        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("AGENDA '6 settembre 2018' :",
                       "KendoGrid: aggiunta gestione di un parametriKendoGrid.headerAttributes per definire lo stile delle colonne aggiunte (selected e cancella); utilizzato al momento solo nella griglie detail inserimento ore")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("Gis:",
                  "sat,  Realizzata chiamata via Proxy locale per evitare problema CORS")

        Riga_Text("Gis:",
                  "sat,  Chart")


        '==================================

        Riga_Data("12 Ottobre 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")


        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("AGENDA '6 settembre 2018' :",
                       "KendoGrid: aggiunta gestione di un parametriKendoGrid.headerAttributes per definire lo stile delle colonne aggiunte (selected e cancella); utilizzato al momento solo nella griglie detail inserimento ore")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")


        Riga_Text("Gis:",
                  "sat, gestione trasparenza")
        '==================================

        Riga_Data("10 Ottobre 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")


        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("AGENDA '6 settembre 2018' :",
                       "KendoGrid: aggiunta gestione di un parametriKendoGrid.headerAttributes per definire lo stile delle colonne aggiunte (selected e cancella); utilizzato al momento solo nella griglie detail inserimento ore")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                   "Migliorie interfaccia e retinatura raccolta in layer cultivar")

        Riga_Text("GIS :",
                   "Gestione pannello controllo Analisi Dati satellitari")


        Riga_Text("Kendo :",
                   "Aggiunta versione 2018.3.911")

        '==================================


        Riga_Data("11 Settembre 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")


        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("AGENDA '6 settembre 2018' :",
                       "KendoGrid: aggiunta gestione di un parametriKendoGrid.headerAttributes per definire lo stile delle colonne aggiunte (selected e cancella); utilizzato al momento solo nella griglie detail inserimento ore")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")


        '==================================

        Riga_Data("27 Agosto 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")


        Riga_Requisiti("AGENDA '24 Agosto 2018' :",
                       "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                   "Cambiando colori sulla barra dei temi confermando con OK resetta tutte le selezioni precedenti e filtri precedenti")

        Riga_Text("GIS :",
                   "Attraverso un cookie viene ri-selezionata la tipologia layer della sessione precedente.")

        Riga_Text("KendoGrid :",
                   "aggiunta funzione timeEditor per avere un campo di ora (senza data) in griglia")

        '==================================

        Riga_Data("09 Agosto 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                   "gestita inizializzazione della sezione tematizzazione, che veniva sempre caricata e mai ripulita")

        Riga_Text("Gias_Kendo :",
                   "Adeguamento css per utilizzo datetimepicker")


        '==================================

        Riga_Data("01 Agosto 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                   "Kendo modale per op. agenda in sostituzione di BS modale, varie migliorie GUI")

        '==================================

        Riga_Data("27 Luglio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("MENU GIAS :",
                   "Aggiunti loghi e icone per nuovo menu GIAS")

        Riga_Text("GIS :",
                  "Ora si possono creare nuove operazioni di agenda anche da tipologia layer diverse da standard ")

        '==================================
        Riga_Data("25 Luglio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                  "miglioramenti in gestione Tiles ")

        '==================================
        Riga_Data("24 Luglio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")


        Riga_Text("GIS :",
                  "Prima versione layer 'Analisi Cronologia Agenda' ")

        '==================================

        Riga_Data("16 Luglio 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '16 luglio 2018' :",
                       "per GIS gestione pulsanti nuova visita e nuova op agenda")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                  "varie migliorie interfaccia ")

        '==================================

        Riga_Data("12 Luglio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '12 luglio 2018' :",
                       "per GIS invio coordinate ad operazioni agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                  "varie migliorie interfaccia più  pulsante op agenda e punti GPS ")

        '==================================

        Riga_Data("11 Luglio 2018 (2) ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                  "- Migliore Gestione di immagini satellite Sentinel2")
        '==================================

        Riga_Data("11 Luglio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                  "- Migliore Gestione punti rilievi indici ")
        '==================================

        Riga_Data("10 Luglio 2018 (2) ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                  "primo rilascio berlucchi")

        '==================================

        Riga_Data("10 Luglio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text("GIS :",
                  "prima versione con aggancio filtro impianti")

        Riga_Text("GIS :",
                  " - Ripristinata descrizione con lettura da albero Kendo")

        Riga_Text("GIS :",
                  " - a seguito di modifica di un elemento adesso non viene esguito l'autoFit sulla schermata GIS")

        '==================================

        Riga_Data("04 Luglio 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" Kendo Input Vari : ",
                  " Aggiunto valore di default da impostare se in dropdown non è stato selezionato nulla in Get_KendoDDLValue ")

        Riga_Text(" Gias_Kendo.css : ",
                  " fix grafici per griglia kendo, dropdown, numerictextbox e datepicker ")

        Riga_Text("  GIS: ",
                  "  agganciato da gis versione pagina meteo rilievi per analisi schede rilievi")

        '==================================

        Riga_Data("04 Giungo 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" Kendo Input Vari : ",
                  " Aggiunto valore di default da impostare se in dropdown non è stato selezionato nulla in Get_KendoDDLValue ")

        Riga_Text(" Gias_Kendo.css : ",
                  " fix grafici per griglia kendo, dropdown, numerictextbox e datepicker ")

        Riga_Text("  : ",
                  "  ")

        '==================================
        Riga_Data("21 Maggio 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" 2018.2.515 : ",
                  " introdotta nuova versione 2018.2.515 ")

        Riga_Text(" Kendo Input Vari : ",
                  " introdotte nuove funzioni per pulizia DDL e Remote Filternig ")
        '==================================

        Riga_Data("30 Aprile 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" Gis : ",
                  " Gestita kendo window generica (per visualizzazione analisi) ")

        Riga_Text(" Gis : ",
                  " Resize della mappa su tile load ")
        '==================================

        Riga_Data("27 Aprile 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" funzioniComuniKendoGrid : ",
                  "Aggiunto parametro selectable")

        Riga_Text(" agronicacontrolli_gis : ",
                  " Apertura delle analisi a seguito di selezione")

        '==================================

        Riga_Data("13 Aprile 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '13 Aprile 2018 bis' :",
                       "per preferiti menu agenda")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" GIS : ",
                  " GIS - maps API - reso compatibile il caricamento di tiles (sfondi personalizzati) con funzione CoordMapType.prototype.getTile in js_demo_tiles.js " &
                  "<br> ")

        Riga_Text(" site.css : ",
                  "modificato css spostando gli elementi del menu agenda direttamente sulla pagina in questione")

        '==================================

        Riga_Data("10 Aprile 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '05 Marzo 2018' :",
                       "")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")
        Riga_Text(" GIS : ",
                  " GIS - Gestita mancata apertura Dialog Multipoint in chiusura inserimento punti su interfaccia bootstrap XS " &
                  "<br> ")

        Riga_Text(" GIS : ",
            " La selezione adesso funziona anche se si fa click sul CheckBox (viene selezionato l'ultimo elemento checkato) " &
            "<br> ")

        '==================================

        Riga_Data(" 05 Aprile 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '05 Marzo 2018' :",
                       "")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" Loghi : ",
                  " Aggiunto Logo Agronica Sementi  " &
                  "<br> ")

        Riga_Text(" GIS : ",
                  " compatibilità su Agronica Sementi  " &
                  "<br> ")

        '==================================

        Riga_Data("26 Marzo 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '05 Marzo 2018' :",
                       "")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" GIS : ",
                  " Gestita mancata selezione di terreni nudi quando questi sono associati a campi" &
                  "<br> ")

        '==================================

        Riga_Data("21 Marzo 2018")

        Riga_Requisiti("AgronicaCore '' :",
                       "")

        Riga_Requisiti("Componenti '' :",
                       "")

        Riga_Requisiti("AGENDA '05 Marzo 2018' :",
                       "")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                       "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                       "Modifiche Sì/NO. ")

        Riga_Text(" funzioniComuniKendoInputVari : ",
                  " introdotte/modificate molte funzioni per DropDownList, MultiSelect e NumericTextBox" &
                  "<br> ")
        '==================================

        Riga_Data("19 Marzo 2018 (2)")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '05 Marzo 2018' :",
                 "")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")

        Riga_Text(" funzioniComuniKendoInputVari : ",
                  " creaKendoDropDownList: Introdotto un parametro array per filtrare su più campi in contemporanea" &
                  "<br> ")

        '==================================

        Riga_Data("19 Marzo 2018")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '05 Marzo 2018' :",
                 "")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")

        Riga_Text(" VisualizzazioneGIS : ",
                  " Nella visualizzazione delle operazioni di agende impostata legenda con plugin InfoBox" &
                  "<br> ")

        Riga_Text(" funzioniComuniKendoGrid : ",
                  " Introdotto un campo stringa che contrassegna il gruppo di colonne in modo da poter mostrare / nascondere con un unico checkbox o pulsante tutto un gruppo di colonne" &
                  "<br> ")

        '==================================

        Riga_Data("12 Marzo 2018")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '01 Marzo 2018' :",
                 "Per GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")

        Riga_Text(" VisualizzazioneGIS : ",
                  " Nella visualizzazione dei Linestring aggiunto punto ""A"" --> ""B"" " &
                  "<br> ")

        '==================================

        Riga_Data("01 Marzo 2018")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '01 Marzo 2018' :",
                 "Per GIS")

        Riga_Requisiti("SMART '01 Marzo 2018' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")

        Riga_Text(" VisualizzazioneGIS : ",
                  " Lettura da layer per decidere se nel poligono occorre impostare un'etichetta che riporti il nome degli impianti / appezzamenti" &
                  "<br> ")

        '==================================

        Riga_Data("27 Febbraio 2018 ")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '19 Febbraio 2018' :",
                 "Per GIS")

        Riga_Requisiti("SMART '27 Febbraio 2018' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")


        Riga_Text(" GIS : ",
                  " Nel poligono impostare un'etichetta che riporti il nome degli impianti / appezzamenti, così come letto dall'albero anagrafico di sinistra" &
                  "<br> ")
        '==================================

        Riga_Data("19 Febbraio 2018")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '19 Febbraio 2018' :",
                 "Per GIS")

        Riga_Requisiti("SMART '07 Dicembre 2017' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")

        Riga_Text("Gis: ",
                  " Pre-release con inserimento nuovi impianti in fase di modifica appezzamento")

        '==================================

        Riga_Data("06 Febbraio 2018")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '06 Febbraio 2018' :",
                 "Per GIS")

        Riga_Requisiti("SMART '07 Dicembre 2017' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")

        Riga_Text("Gis: ",
                  " Affinamenti interfaccia")


        '==================================

        Riga_Data("02 Febbraio 2018")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '12 Dicembre 2017' :",
                 "Per GIS")

        Riga_Requisiti("SMART '07 Dicembre 2017' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")

        Riga_Text("CSS: ",
                  " punta del tooltip in griglia spostata a sx per indicare la cella correta anche con colonne strette ")

        Riga_Text("funzioniComuniKendoInputVari e funzioniComuniKendoGrid: ",
                  " modifiche varie ")

        Riga_Text("Versione 2018.1.117: ",
                  " aggiunta versione ")

        Riga_Text("Gis: ",
                  " Nuova gestione layer")

        '==================================

        Riga_Data("12 Dicembre 2017")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '12 Dicembre 2017' :",
                 "Per GIS")

        Riga_Requisiti("SMART '07 Dicembre 2017' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")


        Riga_Text("GIS+visite:",
               "Wip")


        '==================================

        Riga_Data("07 Dicembre 2017")

        Riga_Requisiti("AgronicaCore '' :",
                  "")

        Riga_Requisiti("Componenti '' :",
                  "")

        Riga_Requisiti("AGENDA '07 Dicembre 2017' :",
                 "Per GIS")

        Riga_Requisiti("SMART '07 Dicembre 2017' :",
                 "Per GIS")

        Riga_Requisiti("WEB.CONFIG :",
                  "Modifiche Sì/NO. ")

        Riga_Text(" GIS : ",
                 " Gestione Layers ")

        '==================================

    End Sub




    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.TabellaVersione.Rows.Clear()
        Call Carica_Pannello()
        Me.Pannello_Versione.Visible = False
    End Sub


    '#################################################################################################
    Private Sub Riga_Data(ByVal dataAggiornamento As String)

        Dim riga As New HtmlTableRow

        riga.Cells.Add(New HtmlTableCell)

        riga.Cells(0).BgColor = "#00bfff"
        riga.Cells(0).Height = "25px"
        riga.Cells(0).Attributes.Add("class", "Testo_08_Nero_Bold")
        riga.Cells(0).InnerText = dataAggiornamento

        Me.TabellaVersione.Rows.Add(riga)

    End Sub

    '#################################################################################################
    Private Sub Riga_Requisiti(ByVal titolo As String, ByVal testo As String, Optional ByVal ver As String = "")

        Dim riga As New HtmlTableRow

        riga.Cells.Add(New HtmlTableCell)

        riga.Cells(0).BgColor = "#FFFFC0"
        riga.Cells(0).Height = "20px"
        riga.Cells(0).Attributes.Add("class", "Testo_08_Rosso")

        riga.Cells(0).InnerHtml = "<b>" & titolo & If(Not String.IsNullOrEmpty(ver), " [" & ver & "]", "") & "</b><br>" & testo

        Me.TabellaVersione.Rows.Add(riga)

    End Sub


    '#################################################################################################
    Private Sub Riga_Text(ByVal titolo As String, ByVal testo As String)

        Dim riga As New HtmlTableRow

        riga.Cells.Add(New HtmlTableCell)

        riga.Cells(0).BgColor = "#c0ffc0"
        riga.Cells(0).Height = "20px"
        riga.Cells(0).Attributes.Add("class", "Testo_08_Nero")

        riga.Cells(0).InnerHtml = "<b>" & titolo & "</b><br>" & testo

        Me.TabellaVersione.Rows.Add(riga)

    End Sub


    '#################################################################################################
    Private Sub Riga_Bug(ByVal titolo As String, ByVal testo As String)

        Dim riga As New HtmlTableRow

        riga.Cells.Add(New HtmlTableCell)

        riga.Cells(0).BgColor = "#c0ffc0"
        riga.Cells(0).Height = "20px"
        riga.Cells(0).Attributes.Add("class", "Testo_08_Nero")

        riga.Cells(0).InnerHtml = "<b>" & titolo & "</b><br>" & testo

        Me.TabellaVersione.Rows.Add(riga)

    End Sub

    '#################################################################################################
    Private Sub Riga_Fine()

        Dim riga As New HtmlTableRow

        riga.Cells.Add(New HtmlTableCell)

        riga.Cells(0).BgColor = "whitesmoke"
        riga.Cells(0).Height = "20px"
        riga.Cells(0).Attributes.Add("class", "Testo_08_Nero")

        riga.Cells(0).InnerHtml = "&nbsp;"

        Me.TabellaVersione.Rows.Add(riga)

    End Sub

    '#####################################################################
    'Protected Sub Btn_Codice_Ins_Click(sender As Object, e As EventArgs) Handles Btn_Codice_Ins.Click
    '    Call Carica_Pannello()
    '    Me.Pannello_Versione.Visible = True
    'End Sub

End Class

End Namespace
