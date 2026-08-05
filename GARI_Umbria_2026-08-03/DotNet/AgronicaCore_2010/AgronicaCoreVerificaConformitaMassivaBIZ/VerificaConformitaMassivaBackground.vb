Imports System.Globalization
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.Compliance
Imports AgronicaCoreEsitoVerificaConformitaBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports InData.Agenda
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class VerificaConformitaMassivaBackground
    Public Class ParametriExtra
        Public disciplinare As String
        Public piva As List(Of String)
        Public datada As String
        Public dataA As String
        Public vegCod As List(Of Integer)
        Public disciplinareVerifica As String
        Public verificaSoloControlliImpostazioniUtente As Boolean = False
        Public stampaInfoSuFile As Boolean = False
        Public modalitaVerifica As Integer
        Public flagMagazzino As Boolean = False
        Public accessToken As String
    End Class

    Public Enum enum_ModalitaVerifica
        Entrambi = 1
        Verifica_Conformita_Engine = 2
        Verifica_Conformita_Gias = 3
    End Enum

    Public Function EseguiVerifica(data_da_str As String,
                                   data_a_str As String,
                                   filtro_piva As List(Of String),
                                   disciplinareVerifica As String,
                                   vegCod As List(Of Integer),
                                   VerificaSoloControlliImpostazioniUtente As Boolean,
                                   stampaInfoSuFile As Boolean,
                                   configServizio As Configurazione_Servizio,
                                   objParametri_Server As AgronicaCoreParametri,
                                   objParametri_Utenti As AgronicaCoreParametri,
                                   objParametri_SuperServer As AgronicaCoreParametri,
                                   modalitaVerifica As Integer,
                                   flagMagazzino As Boolean,
                                   accessToken As String,
                                   ByRef Messaggio_di_Ritorno_Opzionale As String) As Boolean

        Dim result As Boolean = False

        Dim objScriviInteventiEVerifiche As New AgronicaCoreEsitoVerificaConformitaBIZ.Verifiche_Esiti_W

        Try
            Dim objLog As New AgronicaCoreDataProvider.LogProvider

            'Forzo la cultura italiana per evitare problemi di formattazione date con il web service
            CultureInfo.CurrentCulture = New CultureInfo("it-IT")


            Dim InfoVerifiche As New Info_Verifiche_Massive_R
            Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim objDpi As New AgronicaCoreDpiBIZ.DPI_Verifica
            Dim obj_Op As New AgronicaCoreContabDAL.Mov_Destinazioni_R

            Dim objWs As New AgronicaCoreWebService.AgroWs

            Dim data_a As Date = Date.ParseExact(data_a_str, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            Dim data_da As Date = Date.ParseExact(data_da_str, "dd/MM/yyyy", CultureInfo.InvariantCulture)

            Dim dataTableResult As DataTable = InfoVerifiche.LeggiImpresexSpeciexOperazioniVerificaConformita(data_da, data_a, filtro_piva, vegCod, objParametri_Server)

            Dim sw As New Stopwatch()
            sw.Start()

            Dim configurazioniSitiRead As New Configurazione_Siti_R
            Dim qdcaComplianceUri = configurazioniSitiRead.Leggi_Valore(0, "GiasOnline_QdCACompliance_API", "", "", objParametri_Server)

            Dim uri = qdcaComplianceUri & "/RichiesteVerifica/InviaRichiestaAnalisi"
            Dim utility As New AgronicaCoreUtility.CallNetCore()
            utility.AggiustaUrl(uri, objParametri_Server)

            Dim flagNuovoControlloRiduzioneDiserbo = False
            Dim dtConfigSiti = configurazioniSitiRead.Leggi_Valore(0, "Flag_Nuovo_Controllo_Riduzione_Diserbo", "", "", objParametri_Server)

            If (dtConfigSiti <> "") Then
                flagNuovoControlloRiduzioneDiserbo = CBool(dtConfigSiti)
            End If

            For Each row As DataRow In dataTableResult.Rows
                Dim idTestataEngine As Integer = -1
                Dim idTestataWebService As Integer = -1

                Dim piva As String = row!piva
                Dim sa_cod As Integer = 0

                Dim veg_cod As Integer = row!veg_cod

                Dim Disciplinare_Cod As String = "0"
                Dim Disciplinare_PubblicoPrivato As String = "0"
                Dim Regolamento_Concimazione_Cod As String = "0"

                If disciplinareVerifica <> "" Then
                    Dim Array() As String
                    Array = Split(disciplinareVerifica, "/")

                    Disciplinare_Cod = Array(0)
                    If Array.Length > 1 AndAlso Not Array(1) Is Nothing Then
                        Disciplinare_PubblicoPrivato = Array(1)
                    End If
                    If Array.Length > 1 AndAlso Not Array(2) Is Nothing Then
                        Regolamento_Concimazione_Cod = Array(2)
                    End If
                End If

                objLog.Scrivi_LOG(objParametri_Server, "ExportVerificaConformitaMassiva.EseguiVerifica()", String.Format("Inizio verifica per piva {0}, sa_cod {1} e veg_cod {2}", piva, sa_cod, veg_cod), verificaInviaElasticSearch:=True)

                If modalitaVerifica = enum_ModalitaVerifica.Entrambi OrElse modalitaVerifica = enum_ModalitaVerifica.Verifica_Conformita_Engine Then
                    'parte nuova
                    Dim attivitaPerVerifica As New LeggiAgendaHubQDCNew
                    'attivitaPerVerifica.idTestata = Nothing
                    attivitaPerVerifica.piva = piva
                    attivitaPerVerifica.saCod.Add(CInt(sa_cod))
                    attivitaPerVerifica.specieVegetale.Add(veg_cod)
                    Dim intervallo As New IntervalloTemporale
                    intervallo.inizio = data_da
                    intervallo.fine = data_a
                    attivitaPerVerifica.intervallo = intervallo

                    attivitaPerVerifica.disciplinare = New AgronicaCoreModelsSTD.metaschema.Disciplinare With {
                        .codice = disciplinareVerifica,
                        .disciplinarePubblicoPrivato = Disciplinare_PubblicoPrivato,
                        .regolamentoConcimazione = New AgronicaCoreModelsSTD.metaschema.RegolamentoConcimazione With {
                            .codice = Regolamento_Concimazione_Cod
                        }
                    }

                    attivitaPerVerifica.verificaSoloControlliUtente = VerificaSoloControlliImpostazioniUtente
                    attivitaPerVerifica.verificaIAF = False
                    attivitaPerVerifica.verificaMagazzino = flagMagazzino
                    attivitaPerVerifica.origin = Enum_OrigineRichiestaVerificaConformita.gsb_massivo_engine
                    attivitaPerVerifica.controlloRiduzioneDiserbo = flagNuovoControlloRiduzioneDiserbo

                    Dim payloadAsString = JsonConvert.SerializeObject(attivitaPerVerifica)
                    Dim response = utility.CallNetCorePost(uri, accessToken, payloadAsString, objParametri_Server)
                    Dim res = JsonConvert.DeserializeObject(Of RispostaStandard)(response)

                    ' Deserializza RispostaStringa come JArray e ottieni idTestata
                    Dim jArray As JArray = JArray.Parse(res.RispostaStringa)
                    If jArray.Count = 1 Then
                        Dim firstItem As JObject = CType(jArray(0), JObject)
                        idTestataEngine = CInt(firstItem("IdTestata"))
                    Else
                        objLog.Scrivi_LOG(objParametri_Server, "ExportVerificaConformitaMassiva.EseguiVerifica()", "Fallito recupero idTestataEngine", verificaInviaElasticSearch:=True)
                    End If
                End If


                If modalitaVerifica = enum_ModalitaVerifica.Entrambi OrElse modalitaVerifica = enum_ModalitaVerifica.Verifica_Conformita_Gias Then
                    ' parte vecchia
                    Dim DtCentri As DataTable

                    Dim FiltroCentri As String = ""
                    Dim FiltroCentrixPart As String = ""
                    Dim FiltroCentrixFasi As String = ""

                    If sa_cod <> "0" Then
                        FiltroCentri = " Centri_Aziendali.sa_cod in (" & sa_cod & ")"
                        FiltroCentrixFasi = " Mov_Destinazioni.sa_cod in (" & sa_cod & ")"
                        FiltroCentrixPart = " AppezzamentixParticelle.sa_cod in (" & sa_cod & ")"
                    End If

                    DtCentri = objCentro.Leggi(piva, 0, enumSelezioneVariabile.Selezione_JoinDescrizioni, FiltroCentri, "", objParametri_Server)

                    Dim DT_InterventoVerificato As New DataTable
                    Dim DT_InterventoVerificatoMagazzino As New DataTable
                    Dim DT_RisultatiVerifiche As New DataTable
                    Dim DT_RisultatiVerificheMagazzino As New DataTable
                    Dim DT_DettagliGiacenzeMagazzinoInizio As New DataTable
                    Dim DT_DettagliGiacenzeMagazzinoFine As New DataTable

                    objDpi.Crea_DT_InterventoVerificato(DT_InterventoVerificato)
                    objDpi.Crea_DT_RisultatiVerifiche(DT_RisultatiVerifiche)

                    objDpi.Crea_DT_InterventoVerificatoMagazzino(DT_InterventoVerificatoMagazzino)
                    objDpi.Crea_DT_RisultatiVerificheMagazzino(DT_RisultatiVerificheMagazzino, True)

                    objDpi.Crea_DT_DettagliGiacenzeMagazzino(DT_DettagliGiacenzeMagazzinoInizio)
                    objDpi.Crea_DT_DettagliGiacenzeMagazzino(DT_DettagliGiacenzeMagazzinoFine)

                    ' ----------------------------------------------------
                    ' RACCOLTE
                    ' ----------------------------------------------------

                    Dim Dt_Raccolte As DataTable
                    Dim Dt_Raccolte_Impianti As DataTable
                    Dim Dt_Trattamenti_x_Raccolte As New DataTable
                    Dim Dt_Trattamenti_Impianti_x_Raccolte As DataTable
                    Dim DTCarenze As DataTable
                    Dim DtPrincipi As DataTable

                    'estraggo i formulati distinti per ricavare le carenze necessarie al controllo della carenza ed i principi attivi
                    'Dim DTFrCod As DataTable = obj_Op.Leggi_Prodotti_Utilizzati_Su_VegCod_IntervalloTemporale(piva, sa_cod, veg_cod, CDate(AGRODATAINIZIO), CDate(data_a), FORMULATI, "", "", objParametri_Server)
                    Dim DTFrCod As DataTable = obj_Op.Leggi_Prodotti_Utilizzati_Su_VegCod_IntervalloTemporale(piva, sa_cod, veg_cod, AGRODATAINIZIO, AGRODATAFINE, FORMULATI, "", "", objParametri_Server)

                    Dim strFrCod As String = ""

                    Dim IDTestataTemp__tmp_FormulatiXPrincipiAttivi As Integer = -1

                    If Not DTFrCod Is Nothing AndAlso DTFrCod.Rows.Count > 0 Then


                        For f = 0 To DTFrCod.Rows.Count - 1
                            strFrCod &= DTFrCod.Rows(f).Item("pro_cod") & ","
                        Next

                        If strFrCod <> "" Then

                            DTCarenze = objWs.TempoCarenza_from_Multiple_FrCod_VegCod_Intervallo(Left(strFrCod, strFrCod.Length - 1),
                                                                                       veg_cod, CDate(AGRODATAINIZIO), CDate(data_a),
                                                                                       objParametri_Server, objParametri_Utenti, objParametri_SuperServer, True)

                            DtPrincipi = objWs.ComposizioneFormulatiRecupera(Left(strFrCod, strFrCod.Length - 1), objParametri_Server, objParametri_Utenti)

                            If Not DtPrincipi Is Nothing Then

                                Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                                'IDTestataTemp__tmp_FormulatiXPrincipiAttivi = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
                                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                                IDTestataTemp__tmp_FormulatiXPrincipiAttivi = xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)

                                For p = 0 To DtPrincipi.Rows.Count - 1
                                    PopolaTabellaFormulatiPA("", IDTestataTemp__tmp_FormulatiXPrincipiAttivi, objParametri_Server, DtPrincipi.Rows(p).Item("elenco_PrincipiAttivi"), DtPrincipi.Rows(p).Item("elenco_PrincipiAttiviPesi"), DtPrincipi.Rows(p).Item("fr_cod"))
                                Next

                            End If


                        End If
                    End If

                    Dt_Trattamenti_x_Raccolte = obj_Op.Leggi_Operazioni_VegCod_IntervalloTemporale_x_Analisi_Conformita(piva, sa_cod, veg_cod, AGRODATAINIZIO, CDate(data_a), enum_TipoOperazione_xAnalisiConformita.Difesa_Diserbo, "", " movimenti.Data_Movimento desc", objParametri_Server)

                    If Not Dt_Trattamenti_x_Raccolte Is Nothing AndAlso Dt_Trattamenti_x_Raccolte.Rows.Count > 0 Then

                        Dim strIdAgendaTrattamenti_x_Raccolte As String = ""
                        For i = 0 To Dt_Trattamenti_x_Raccolte.Rows.Count - 1
                            strIdAgendaTrattamenti_x_Raccolte &= Dt_Trattamenti_x_Raccolte.Rows(i).Item("id_agenda") & ","
                        Next

                        If strIdAgendaTrattamenti_x_Raccolte <> "" Then
                            Dt_Trattamenti_Impianti_x_Raccolte = obj_Op.Leggi_Dettagli_Impianti_x_Analisi_Conformita(piva, Left(strIdAgendaTrattamenti_x_Raccolte, strIdAgendaTrattamenti_x_Raccolte.Length - 1), enum_TipoOperazione_xAnalisiConformita.Difesa_Diserbo, "", "", objParametri_Server)
                        End If
                    End If


                    Dt_Raccolte = obj_Op.Leggi_Operazioni_VegCod_IntervalloTemporale_x_Analisi_Conformita(piva, sa_cod, veg_cod, CDate(data_da), CDate(data_a), enum_TipoOperazione_xAnalisiConformita.Raccolta, "", " movimenti.Data_Movimento desc", objParametri_Server)

                    If Not Dt_Raccolte Is Nothing AndAlso Dt_Raccolte.Rows.Count > 0 Then

                        Dim strIdAgendaRaccolte As String = ""
                        For i = 0 To Dt_Raccolte.Rows.Count - 1
                            strIdAgendaRaccolte &= Dt_Raccolte.Rows(i).Item("id_agenda") & ","
                        Next

                        If strIdAgendaRaccolte <> "" Then
                            Dt_Raccolte_Impianti = obj_Op.Leggi_Dettagli_Impianti_x_Analisi_Conformita(piva, Left(strIdAgendaRaccolte, strIdAgendaRaccolte.Length - 1), enum_TipoOperazione_xAnalisiConformita.Raccolta, "", "", objParametri_Server)
                        End If


                        'analisi
                        Dim Lrval_Raccolte As New List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

                        Lrval_Raccolte = objDpi.Verifica_Raccolte(Dt_Raccolte, Dt_Raccolte_Impianti, Dt_Trattamenti_Impianti_x_Raccolte, DTCarenze, VerificaSoloControlliImpostazioniUtente, objParametri_Server, objParametri_Utenti)

                        If Not Lrval_Raccolte Is Nothing Then
                            For Each rval In Lrval_Raccolte
                                If rval.RispostaOK = True Then
                                    If rval.RispostaStringa.Risultato <> "" Then
                                        objDpi.Analizza_RisultatoVerifica(rval.RispostaStringa.Risultato, VerificaSoloControlliImpostazioniUtente, DtCentri, DT_InterventoVerificato, DT_RisultatiVerifiche, 0, False, objParametri_Utenti)
                                    End If
                                Else
                                    objLog.Scrivi_LOG(objParametri_Server, "ExportVerificaConformitaMassiva.EseguiVerifica()", "Errore Verifica_Raccolte()", verificaInviaElasticSearch:=True)
                                End If
                            Next
                        End If
                    End If


                    ' -----------------------------------------------------
                    ' TRATTAMENTI
                    ' -----------------------------------------------------
                    Dim Dt_Trattamenti As New DataTable
                    Dim Dt_Trattamenti_Impianti As DataTable

                    Dim Dt_Raccolte_x_Trattamenti As DataTable
                    Dim Dt_Raccolte_Impianti_x_Trattamenti As DataTable

                    Dt_Raccolte_x_Trattamenti = obj_Op.Leggi_Operazioni_VegCod_IntervalloTemporale_x_Analisi_Conformita(piva, sa_cod, veg_cod, CDate(data_da), CDate(AGRODATAFINE), enum_TipoOperazione_xAnalisiConformita.Raccolta, "", " movimenti.Data_Movimento desc", objParametri_Server)

                    If Not Dt_Raccolte_x_Trattamenti Is Nothing AndAlso Dt_Raccolte_x_Trattamenti.Rows.Count > 0 Then

                        Dim strIdAgendaRaccolte_x_Trattamenti As String = ""
                        For i = 0 To Dt_Raccolte_x_Trattamenti.Rows.Count - 1
                            strIdAgendaRaccolte_x_Trattamenti &= Dt_Raccolte_x_Trattamenti.Rows(i).Item("id_agenda") & ","
                        Next

                        If strIdAgendaRaccolte_x_Trattamenti <> "" Then
                            Dt_Raccolte_Impianti_x_Trattamenti = obj_Op.Leggi_Dettagli_Impianti_x_Analisi_Conformita(piva, Left(strIdAgendaRaccolte_x_Trattamenti, strIdAgendaRaccolte_x_Trattamenti.Length - 1), enum_TipoOperazione_xAnalisiConformita.Raccolta, "", "", objParametri_Server)
                        End If

                    End If

                    Dt_Trattamenti = obj_Op.Leggi_Operazioni_VegCod_IntervalloTemporale_x_Analisi_Conformita(piva, sa_cod, veg_cod, CDate(data_da), CDate(data_a), enum_TipoOperazione_xAnalisiConformita.Difesa_Diserbo, "", " movimenti.Data_Movimento desc", objParametri_Server)

                    If Not Dt_Trattamenti Is Nothing AndAlso Dt_Trattamenti.Rows.Count > 0 Then

                        Dim strIdAgendaTrattamenti As String = ""
                        For i = 0 To Dt_Trattamenti.Rows.Count - 1
                            strIdAgendaTrattamenti &= Dt_Trattamenti.Rows(i).Item("id_agenda") & ","
                        Next

                        If strIdAgendaTrattamenti <> "" Then
                            Dt_Trattamenti_Impianti = obj_Op.Leggi_Dettagli_Impianti_x_Analisi_Conformita(piva, Left(strIdAgendaTrattamenti, strIdAgendaTrattamenti.Length - 1), enum_TipoOperazione_xAnalisiConformita.Difesa_Diserbo, "", "", objParametri_Server)
                        End If


                        '------------------------------------
                        ' FASI FENOLOGICHE
                        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
                        objParametriIngresso.Veg_Cod = veg_cod
                        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
                        Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

                        Dim FF_Cod_Fioritura_Old As Integer = 0
                        Dim FF_Cod_Fioritura_New As Integer = 0

                        'fasi bbch x fioritura
                        objParametriUscita = objFasi_WS.FasiFenologiche(objParametriIngresso, objParametri_Server, objParametri_SuperServer, True)
                        For f = 0 To objParametriUscita.ListaFasiFenologiche.Count - 1
                            If objParametriUscita.ListaFasiFenologiche(f).Fioritura Then
                                FF_Cod_Fioritura_New = objParametriUscita.ListaFasiFenologiche(f).Cod_SS
                                FF_Cod_Fioritura_Old = objParametriUscita.ListaFasiFenologiche(f).FF_Cod
                                Exit For
                            End If
                        Next

                        Dim dtFasiFeno As DataTable
                        Dim ObjFasiFeno As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R
                        dtFasiFeno = ObjFasiFeno.Leggi_FasiFenologiche(piva, 0, 0, 0, AGRODATAINIZIO, AGRODATAFINE,
                                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                            FiltroCentrixFasi, "Data_Ril Desc",
                                                                            objParametri_Server)

                        '------------------------------------
                        'CATASTO x deroghe territoriali
                        Dim DtAppxPart As DataTable
                        Dim objAppxPart As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

                        DtAppxPart = objAppxPart.LeggiParticelle_Da_Appezzamento(piva, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, FiltroCentrixPart, "", objParametri_Server)


                        ' -------------------------------------------------------------------------------------------------------------------------                       

                        'analisi
                        Dim Lrval_Trattamenti As New List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

                        Lrval_Trattamenti = objDpi.Verifica_Trattamenti(Dt_Trattamenti, Dt_Trattamenti_Impianti,
                                                                                         Disciplinare_Cod, Disciplinare_PubblicoPrivato, IDTestataTemp__tmp_FormulatiXPrincipiAttivi,
                                                                                         DtAppxPart, DtCentri,
                                                                                         Dt_Raccolte_Impianti_x_Trattamenti, dtFasiFeno, FF_Cod_Fioritura_Old, FF_Cod_Fioritura_New,
                                                                                         VerificaSoloControlliImpostazioniUtente, objParametri_Server, objParametri_Utenti, objParametri_SuperServer, True)

                        If Not Lrval_Trattamenti Is Nothing Then
                            For Each rval In Lrval_Trattamenti
                                If rval.RispostaOK = True Then
                                    If rval.RispostaStringa.Risultato <> "" Then
                                        objDpi.Analizza_RisultatoVerifica(rval.RispostaStringa.Risultato, VerificaSoloControlliImpostazioniUtente, DtCentri, DT_InterventoVerificato, DT_RisultatiVerifiche, 0, False, objParametri_Utenti)
                                    End If
                                Else
                                    objLog.Scrivi_LOG(objParametri_Server, "ExportVerificaConformitaMassiva.EseguiVerifica()", "Errore Verifica_Trattamenti()", verificaInviaElasticSearch:=True)
                                End If
                            Next

                            For Each drt As DataRow In DT_InterventoVerificato.Rows
                                Dim HashP As New Hashtable
                                Dim Prodotti As String = ""
                                Dim Prodotto As String = ""
                                Dim Principi As String = ""
                                Select Case drt("lav_cod")
                                    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_CONCIA_SEME, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO, LAVCOD_TRATTAMENTO_FITOREGOLATORE
                                        Dim DrProdOp() As DataRow = Dt_Trattamenti_Impianti.Select("id_agenda=" & drt("id_agenda"))
                                        If Not DrProdOp Is Nothing AndAlso Not DtPrincipi Is Nothing Then
                                            For Each Drtp As DataRow In DrProdOp
                                                If Not HashP.ContainsKey(Drtp("pro_cod")) Then
                                                    HashP.Add(Drtp("pro_cod"), "")
                                                    Dim DrPrin() As DataRow = DtPrincipi.Select("fr_cod=" & Drtp("pro_cod"))
                                                    If Not DrPrin Is Nothing Then
                                                        Dim testo As String = DrPrin(0).Item("Elenco_PrincipiAttivi")
                                                        Prodotto = DrPrin(0).Item("fr_des")
                                                        Principi = ""
                                                        If testo <> "" Then
                                                            Dim elenco As String() = Split(testo, "|")
                                                            For Each elem As String In elenco
                                                                Dim datiElem As String() = Split(elem, "§")
                                                                If datiElem.Count > 1 Then
                                                                    Principi &= datiElem(1) & ","
                                                                End If
                                                            Next
                                                        End If
                                                        Prodotti &= Prodotto & " (" & Left(Principi, Principi.Length - 1) & ") - "
                                                    End If
                                                End If
                                            Next
                                        End If
                                End Select

                                If Prodotti <> "" Then
                                    drt("des_lib") &= " - " & Left(Prodotti, Prodotti.Length - 3)
                                End If
                            Next
                        End If

                    End If

                    ' -----------------------------------------------------
                    ' FERTILIZZAZIONI
                    ' -----------------------------------------------------


                    Dim Dt_Fertilizzazioni As DataTable


                    Dt_Fertilizzazioni = obj_Op.Leggi_Operazioni_VegCod_IntervalloTemporale_x_Analisi_Conformita(piva, sa_cod, veg_cod, CDate(data_da), CDate(data_a), enum_TipoOperazione_xAnalisiConformita.Fertilizzazioni, "", " movimenti.Data_Movimento desc", objParametri_Server)

                    If Not Dt_Fertilizzazioni Is Nothing AndAlso Dt_Fertilizzazioni.Rows.Count > 0 Then

                        'estraggo i fertilizzanti distinti per ricavare le informazioni
                        'Dim DTFerCod As DataTable = obj_Op.Leggi_Prodotti_Utilizzati_Su_VegCod_IntervalloTemporale(piva, sa_cod, veg_cod, CDate(data_da), CDate(data_a), FERTILIZZANTI, "", "", objParametri_Server)
                        Dim DTFerCod As DataTable = obj_Op.Leggi_Prodotti_Utilizzati_Su_VegCod_IntervalloTemporale(piva, sa_cod, veg_cod, AGRODATAINIZIO, AGRODATAFINE, FERTILIZZANTI, "", "", objParametri_Server)

                        Dim strFerCod As String = ""
                        If Not DTFerCod Is Nothing AndAlso DTFerCod.Rows.Count > 0 Then
                            For f = 0 To DTFerCod.Rows.Count - 1
                                strFerCod &= DTFerCod.Rows(f).Item("pro_cod") & ","
                            Next
                        End If

                        'lista concimi utilizzati
                        Dim objParametriUscitaFertilizzanti As AgronicaCoreMetaSchemaBIZ.Fertilizzanti_output

                        If strFerCod <> "" Then

                            Dim objParametriIngressoFertilizzanti As New AgronicaCoreMetaSchemaBIZ.Fertilizzanti_input
                            objParametriIngressoFertilizzanti.Codice = 0
                            objParametriIngressoFertilizzanti.Descrizione = ""
                            objParametriIngressoFertilizzanti.DataInizio = AGRODATAINIZIO
                            objParametriIngressoFertilizzanti.DataFine = AGRODATAFINE
                            objParametriIngressoFertilizzanti.IncludiApporti = True
                            objParametriIngressoFertilizzanti.Regolamento = 0
                            objParametriIngressoFertilizzanti.strFiltro = " Fertilizzanti.Fer_Cod IN (" & Left(strFerCod, strFerCod.Length - 1) & ")"

                            Dim objFert_WS As New AgronicaCoreWebService.Fertilizzanti_WS
                            objParametriUscitaFertilizzanti = objFert_WS.Fertilizzanti(objParametriIngressoFertilizzanti, objParametri_Server, objParametri_SuperServer, True)

                        End If

                        If disciplinareVerifica <> "" Then

                            Dim Array() As String
                            Array = Split(disciplinareVerifica, "/")

                            If Array.Length > 0 AndAlso Not Array(0) Is Nothing Then
                                Disciplinare_Cod = Array(0)
                            End If
                            If Array.Length > 2 AndAlso Not Array(2) Is Nothing Then
                                Disciplinare_Cod = Array(2)
                            End If

                        End If

                        'lista n_max_intervento
                        Dim objParametriUscitaFattoriCorrettivi As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
                        Dim objParametriIngressoFattoriCorrettivi As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
                        objParametriIngressoFattoriCorrettivi.Regolamento_Cod = 0
                        objParametriIngressoFattoriCorrettivi.Veg_Cod = veg_cod
                        objParametriIngressoFattoriCorrettivi.Grfi_Cod = 0
                        objParametriIngressoFattoriCorrettivi.SoloValorizzati = True
                        objParametriIngressoFattoriCorrettivi.SoloVisibili = False
                        objParametriIngressoFattoriCorrettivi.Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_Max_Intervento

                        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                        objParametriUscitaFattoriCorrettivi = objPC_WS.FattoriCorrettivi_ConFinalitaGias_Leggi(objParametriIngressoFattoriCorrettivi, objParametri_Server, objParametri_SuperServer, True)

                        'analisi
                        Dim Lrval_Fertilizzazioni As New List(Of rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Intervento))

                        Lrval_Fertilizzazioni = objDpi.Verifica_Concimazioni(Dt_Fertilizzazioni,
                                                                                         Disciplinare_Cod, IDTestataTemp__tmp_FormulatiXPrincipiAttivi, objParametriUscitaFertilizzanti, objParametriUscitaFattoriCorrettivi,
                                                                                         VerificaSoloControlliImpostazioniUtente, objParametri_Server, objParametri_Utenti, objParametri_SuperServer)


                        If Not Lrval_Fertilizzazioni Is Nothing Then
                            For Each rval In Lrval_Fertilizzazioni
                                If rval.RispostaOK = True Then
                                    If rval.RispostaStringa.Risultato <> "" Then
                                        objDpi.Analizza_RisultatoVerifica(rval.RispostaStringa.Risultato, VerificaSoloControlliImpostazioniUtente, DtCentri, DT_InterventoVerificato, DT_RisultatiVerifiche, 0, False, objParametri_Utenti)
                                    End If
                                Else
                                    objLog.Scrivi_LOG(objParametri_Server, "ExportVerificaConformitaMassiva.EseguiVerifica()", "Errore Verifica_Concimazioni()", verificaInviaElasticSearch:=True)
                                End If
                            Next
                        End If
                    End If

                    If flagMagazzino = "true" Then

                        Dim DataOp As Date
                        Dim SaCodOp As Integer
                        Dim LavCodOp As Integer

                        Dim Id_Agenda As Integer
                        Dim Fabbricato_Cod As String
                        Dim Fabbricato_Des As String
                        Dim ProdottiSuff As Boolean = True
                        Dim HashMagazzini As New Hashtable
                        Dim HashProdottiMovimentati As New Hashtable

                        Dim objCatMag As New AgronicaCoreMetaSchemaDAL.Categorie_Magazzino_R
                        Dim objMat As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                        Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

                        Dim Dt_Op As New DataTable
                        Dim Dt_Op_Magazzino As DataTable
                        Dt_Op = obj_Op.Leggi_Operazioni_VegCod_IntervalloTemporale_x_Analisi_Conformita(piva, sa_cod, veg_cod, CDate(data_da), CDate(data_a), enum_TipoOperazione_xAnalisiConformita.Magazzino, "", " movimenti.Data_Movimento desc", objParametri_Server)


                        If Not Dt_Op Is Nothing AndAlso Dt_Op.Rows.Count > 0 Then

                            Dim strIdAgendaMagazzino As String = ""
                            For i = 0 To Dt_Op.Rows.Count - 1
                                strIdAgendaMagazzino &= Dt_Op.Rows(i).Item("id_agenda") & ","
                            Next

                            If strIdAgendaMagazzino <> "" Then
                                Dt_Op_Magazzino = obj_Op.Leggi_Dettagli_Magazzino_x_Analisi_Conformita(piva, Left(strIdAgendaMagazzino, strIdAgendaMagazzino.Length - 1), "", "", objParametri_Server)
                            End If

                        End If

                        For i = 0 To Dt_Op.Rows.Count - 1

                            Id_Agenda = Dt_Op.Rows(i).Item("id_agenda")
                            DataOp = Dt_Op.Rows(i).Item("Data_Movimento")
                            SaCodOp = Dt_Op.Rows(i).Item("sa_cod")
                            LavCodOp = Dt_Op.Rows(i).Item("lav_cod")

                            Select Case LavCodOp

                                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                                     LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                                     LAVCOD_CONCIA_SEME,
                                     LAVCOD_GEODISINFESTAZIONE,
                                     LAVCOD_DISERBO, LAVCOD_DISSECCAMENTO,
                                     LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE, LAVCOD_DISTRIBUZIONE_INSETTI,
                                     LAVCOD_CATTURE_MASSA, LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_REINNESCO_TRAPPOLE,
                                     LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                                     LAVCOD_CONCIMAZIONE_FOGLIARE,
                                     LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                     LAVCOD_DISTRIBUZIONE_CONCIME,
                                     LAVCOD_FERTIRRIGAZIONE,
                                     LAVCOD_SARCHIATURA_CONCIMAZIONE,
                                     LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE

                                    ProdottiSuff = True

                                    Fabbricato_Cod = ""
                                    Fabbricato_Des = "Magazzino non movimentato"

                                    If Not Dt_Op_Magazzino Is Nothing Then

                                        Dim DrProdottiOp() As DataRow = Dt_Op_Magazzino.Select("id_agenda=" & Id_Agenda)

                                        If Not DrProdottiOp Is Nothing AndAlso DrProdottiOp.Length > 0 Then


                                            For p = 0 To DrProdottiOp.Length - 1

                                                Fabbricato_Cod = DrProdottiOp(p).Item("piva") & "|" & DrProdottiOp(p).Item("sa_cod") & "|" & DrProdottiOp(p).Item("id_destinazione")
                                                Fabbricato_Des = DrProdottiOp(p).Item("Fabbricato_Des")

                                                Dim Descrizione As String = ""

                                                'ricavo i magazzini coinvolti negli interventi x verificare poi giacenze a data inizio e data fine analisi
                                                If Not HashMagazzini.ContainsKey(Fabbricato_Cod) Then
                                                    HashMagazzini.Add(Fabbricato_Cod, Fabbricato_Des)
                                                End If

                                                'ricavo i prodotti movimentati negli interventi x verificare poi giacenze a data inizio e data fine analisi
                                                If Not HashProdottiMovimentati.ContainsKey(DrProdottiOp(p).Item("elem_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("pro_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("mat_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("cod_progetto") & "|" &
                                                                                        DrProdottiOp(p).Item("lotto") & "|" &
                                                                                        DrProdottiOp(p).Item("cal_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("udm_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("sa_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("id_destinazione") & "|" &
                                                                                        DrProdottiOp(p).Item("piva")
                                                                                        ) Then


                                                    Select Case DrProdottiOp(p).Item("mat_cod")
                                                        Case 0
                                                            Descrizione = objCatMag.ProDes_from_ProCod(DrProdottiOp(p).Item("elem_cod"), DrProdottiOp(p).Item("pro_cod"), objParametri_Server)
                                                        Case Else
                                                            Descrizione = objMat.MatDes_from_MatCod(piva, DrProdottiOp(p).Item("elem_cod"), DrProdottiOp(p).Item("mat_cod"), "", "", "", objParametri_Server)
                                                    End Select

                                                    HashProdottiMovimentati.Add(DrProdottiOp(p).Item("elem_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("pro_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("mat_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("cod_progetto") & "|" &
                                                                                        DrProdottiOp(p).Item("lotto") & "|" &
                                                                                        DrProdottiOp(p).Item("cal_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("udm_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("sa_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("id_destinazione") & "|" &
                                                                                        DrProdottiOp(p).Item("piva"), Descrizione)
                                                End If

                                                'guardo se la quantità è conforme per la data di intervento

                                                Dim qta_in_data As Decimal = objMovDet.Verifica_Giacenze_Con_Magazzino_Esterno(DrProdottiOp(p).Item("piva"),
                                                               DrProdottiOp(p).Item("sa_cod"),
                                                               DrProdottiOp(p).Item("id_destinazione"),
                                                                DrProdottiOp(p).Item("elem_cod"),
                                                                DrProdottiOp(p).Item("pro_cod"),
                                                                DrProdottiOp(p).Item("mat_cod"),
                                                                DrProdottiOp(p).Item("cod_progetto"),
                                                                0,
                                                                DrProdottiOp(p).Item("lotto"),
                                                                DrProdottiOp(p).Item("cal_cod"),
                                                                DrProdottiOp(p).Item("udm_cod"),
                                                                AGRODATAINIZIO,
                                                                DataOp,
                                                                piva,
                                                                SaCodOp,
                                                                Id_Agenda,
                                                                objParametri_Server)

                                                Dim Qta_scaricata As Decimal = Math.Round(DrProdottiOp(p).Item("qta"), 3)

                                                'If (qta_in_data <> 0 And Not (qta_in_data < 0.00009 And qta_in_data > -0.00009)) Then
                                                '    qta_in_data = Math.Round(qta_in_data, 3)

                                                qta_in_data = Math.Round(qta_in_data, 3)
                                                Descrizione = HashProdottiMovimentati.Item(DrProdottiOp(p).Item("elem_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("pro_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("mat_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("cod_progetto") & "|" &
                                                                                        DrProdottiOp(p).Item("lotto") & "|" &
                                                                                        DrProdottiOp(p).Item("cal_cod") & "|" &
                                                                                        DrProdottiOp(p).Item("udm_cod"))

                                                objDpi.InserisciRiga_DT_RisultatiVerificheMagazzino(DT_RisultatiVerificheMagazzino, Id_Agenda,
                                                         DrProdottiOp(p).Item("elem_cod"),
                                                         DrProdottiOp(p).Item("pro_cod"),
                                                         DrProdottiOp(p).Item("mat_cod"),
                                                         DrProdottiOp(p).Item("cod_progetto"),
                                                         DrProdottiOp(p).Item("cal_cod"),
                                                         DrProdottiOp(p).Item("lotto"),
                                                         Descrizione,
                                                         DrProdottiOp(p).Item("udm_sim"),
                                                         DrProdottiOp(p).Item("udm_cod"),
                                                         Qta_scaricata, qta_in_data,
                                                         IIf(Qta_scaricata > qta_in_data, False, True),
                                                         IIf(Qta_scaricata > qta_in_data, "Quantita Insufficiente di prodotto", "Quantita Sufficiente di prodotto"),
                                                         True,
                                                         DrProdottiOp(p).Item("piva"),
                                                         DrProdottiOp(p).Item("sa_cod"),
                                                         DrProdottiOp(p).Item("fabbricato_cod"))

                                                If Qta_scaricata > qta_in_data Then
                                                    ProdottiSuff = False
                                                End If
                                            Next
                                        End If

                                    End If

                                    Dim Sa_Nome As String = ""
                                    If Not DtCentri Is Nothing Then
                                        Dim DrCentro() As DataRow = DtCentri.Select("piva='" & Dt_Op.Rows(i).Item("piva") & "' and sa_cod=" & SaCodOp)
                                        If Not DrCentro Is Nothing AndAlso DrCentro.Length > 0 Then
                                            Sa_Nome = DrCentro(0).Item("sa_nome")
                                        End If
                                    End If

                                    objDpi.InserisciRiga_DT_InterventoVerificatoMagazzino(DT_InterventoVerificatoMagazzino, DataOp,
                                                       Dt_Op.Rows(i).Item("des_lib"), Fabbricato_Cod, Fabbricato_Des,
                                                       Dt_Op.Rows(i).Item("piva"), SaCodOp, Sa_Nome,
                                                        Dt_Op.Rows(i).Item("lav_cod"), Id_Agenda,
                                                        "", 0, ProdottiSuff, IIf(ProdottiSuff = True, "La Giacenza di tutti i prodotti è positiva", "Esistono prodotti con Giacenza negativa"))

                            End Select

                        Next

                        Dim DtGiacenze As DataTable
                        Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R
                        Dim StringaFilter = " AND Movimenti_Dettagli.Elem_Cod <> -1"

                        Dim FiltroProdottiMovimentati As String = ""
                        For Each key In HashProdottiMovimentati
                            FiltroProdottiMovimentati &= " ( Movimenti_Dettagli.Elem_Cod=" & Split(key.key, "|")(0) &
                                                            " AND Movimenti_Dettagli.Pro_Cod=" & Split(key.key, "|")(1) &
                                                            " AND Movimenti_Dettagli.Mat_Cod=" & Split(key.key, "|")(2) &
                                                            " AND Movimenti_Dettagli.cod_progetto=" & Split(key.key, "|")(3) &
                                                            " AND Movimenti_Dettagli.lotto='" & Split(key.key, "|")(4) & "'" &
                                                            " AND Movimenti_Dettagli.cal_cod=" & Split(key.key, "|")(5) &
                                                            " AND Movimenti_Dettagli.udm_cod=" & Split(key.key, "|")(6) & ") OR "
                        Next
                        If FiltroProdottiMovimentati <> "" Then
                            StringaFilter &= " AND (" & Left(FiltroProdottiMovimentati, FiltroProdottiMovimentati.Length - 3) & ")"
                        End If

                        'per ogni magazzino verifica giacenze alla data inizio e data fine analisi 
                        For Each keyMagazzino In HashMagazzini

                            ' DATA INIZIO
                            DtGiacenze = objGiacenze.SchedaGiacenzeMagazzino(data_da,
                                                                             Split(keyMagazzino.key, "|")(0),
                                                                             Split(keyMagazzino.key, "|")(1),
                                                                             Split(keyMagazzino.key, "|")(2),
                                                                             0,
                                                                             0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                                             False,
                                                                             StringaFilter, "", "", "", "", "", "", "", "",
                                                                            "", "",
                                                                            "NO",
                                                                            objParametri_Server, objParametri_Utenti,
                                                                            creaParametriSql:=False)

                            For Each keyProdotti In HashProdottiMovimentati
                                If Split(keyProdotti.key, "|")(7) = Split(keyMagazzino.key, "|")(1) AndAlso Split(keyProdotti.key, "|")(8) = Split(keyMagazzino.key, "|")(2) Then
                                    If DtGiacenze IsNot Nothing AndAlso DtGiacenze.Rows.Count > 0 Then
                                        Dim selectedRows = DtGiacenze.Select("Elem_Cod=" & Split(keyProdotti.key, "|")(0) &
                                                                                " AND Pro_Cod=" & Split(keyProdotti.key, "|")(1) &
                                                                                " AND Mat_Cod=" & Split(keyProdotti.key, "|")(2) &
                                                                                " AND cod_progetto=" & Split(keyProdotti.key, "|")(3) &
                                                                                " AND lotto='" & Split(keyProdotti.key, "|")(4) & "'" &
                                                                                " AND cal_cod=" & Split(keyProdotti.key, "|")(5) &
                                                                                " AND udm_cod=" & Split(keyProdotti.key, "|")(6))

                                        If selectedRows IsNot Nothing AndAlso selectedRows.Length > 0 Then
                                            Dim dtGiacenzeProdotto = selectedRows.CopyToDataTable()

                                            If dtGiacenzeProdotto IsNot Nothing AndAlso dtGiacenzeProdotto.Rows.Count > 0 Then
                                                For g = 0 To dtGiacenzeProdotto.Rows.Count - 1

                                                    Dim Giacenza As Decimal = 0
                                                    If Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Giacenza")) AndAlso IsNumeric(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Giacenza"))) Then
                                                        Giacenza = Math.Round(CDec(dtGiacenzeProdotto.Rows(g).Item("Giacenza")), 3)
                                                    End If

                                                    'oltre al filtro nella query, devo fare anche il filtro su codice
                                                    'perchè molti Decimal vengono salvati come valori infinatamente piccoli
                                                    'ad esempio 0.00003680000000017003
                                                    'If (Giacenza <> 0 And Not (Giacenza < 0.00009 And Giacenza > -0.00009)) Then

                                                    objDpi.InserisciRiga_DT_RisultatiVerificheMagazzino(DT_RisultatiVerificheMagazzino, -1,
                                                                     dtGiacenzeProdotto.Rows(g).Item("Elem_Cod"),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Pro_Cod")), dtGiacenzeProdotto.Rows(g).Item("Pro_Cod"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Mat_Cod")), dtGiacenzeProdotto.Rows(g).Item("Mat_Cod"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Cod_Progetto")), dtGiacenzeProdotto.Rows(g).Item("Cod_Progetto"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Cal_Cod")), dtGiacenzeProdotto.Rows(g).Item("Cal_Cod"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Lotto")), dtGiacenzeProdotto.Rows(g).Item("Lotto"), ""),
                                                                          dtGiacenzeProdotto.Rows(g).Item("Descrizione_Prodotto"),
                                                                          dtGiacenzeProdotto.Rows(g).Item("Udm_Sim"),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Udm_Cod")), dtGiacenzeProdotto.Rows(g).Item("Udm_Cod"), 0),
                                                                          0, Giacenza,
                                                                          IIf(Giacenza >= 0, True, False),
                                                                          IIf(Giacenza >= 0, "Giacenza prodotto Positiva", "Giacenza prodotto Negativa"),
                                                                          True,
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("piva")), dtGiacenzeProdotto.Rows(g).Item("piva"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("sa_Cod")), dtGiacenzeProdotto.Rows(g).Item("sa_cod"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Cal_Cod")), dtGiacenzeProdotto.Rows(g).Item("fabbricato_cod"), 0))

                                                    If Giacenza < 0 Then
                                                        ProdottiSuff = False
                                                    End If

                                                    objDpi.InserisciRiga_DT_DettagliGiacenzeMagazzino(DT_DettagliGiacenzeMagazzinoInizio, -1,
                                                            idTestataWebService,
                                                            IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("piva")), dtGiacenzeProdotto.Rows(g).Item("piva"), 0),
                                                            IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("sa_Cod")), dtGiacenzeProdotto.Rows(g).Item("sa_cod"), 0),
                                                            dtGiacenzeProdotto.Rows(g).Item("fabbricato_cod"),
                                                            IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Lotto")), dtGiacenzeProdotto.Rows(g).Item("Lotto"), ""),
                                                            IIf(Giacenza >= 0, True, False),
                                                            dtGiacenzeProdotto.Rows(g).Item("Pro_Cod"),
                                                            dtGiacenzeProdotto.Rows(g).Item("elem_cod"),
                                                            Giacenza,
                                                            IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Udm_Cod")), dtGiacenzeProdotto.Rows(g).Item("Udm_Cod"), 0),
                                                            IIf(Giacenza >= 0, "Giacenza Positiva", "Giacenza Negativa"),
                                                            CDate(data_da),
                                                            1)

                                                    'End If 'Filtro arrotondamento giacenza

                                                Next 'ciclo giacenze
                                            End If
                                        Else
                                            'se entriamo qua il prodotto non è mai stato caricato
                                            Dim Giacenza As Decimal = 0

                                            objDpi.InserisciRiga_DT_DettagliGiacenzeMagazzino(
                                                DT_DettagliGiacenzeMagazzinoInizio,
                                                -1,
                                                idTestataWebService,
                                                Split(keyMagazzino.key, "|")(0),
                                                Split(keyMagazzino.key, "|")(1),
                                                Split(keyMagazzino.key, "|")(2),
                                                Split(keyProdotti.key, "|")(4),
                                                True,
                                                Split(keyProdotti.key, "|")(1),
                                                Split(keyProdotti.key, "|")(0),
                                                Giacenza,
                                                Split(keyProdotti.key, "|")(6),
                                                IIf(Giacenza >= 0, "Giacenza Positiva", "Giacenza Negativa"),
                                                CDate(data_da),
                                                1)
                                        End If
                                    Else
                                        Dim Giacenza As Decimal = 0

                                        objDpi.InserisciRiga_DT_DettagliGiacenzeMagazzino(
                                            DT_DettagliGiacenzeMagazzinoInizio,
                                            -1,
                                            idTestataWebService,
                                            Split(keyMagazzino.key, "|")(0),
                                            Split(keyMagazzino.key, "|")(1),
                                            Split(keyMagazzino.key, "|")(2),
                                            Split(keyProdotti.key, "|")(4),
                                            True,
                                            Split(keyProdotti.key, "|")(1),
                                            Split(keyProdotti.key, "|")(0),
                                            Giacenza,
                                            Split(keyProdotti.key, "|")(6),
                                            IIf(Giacenza >= 0, "Giacenza Positiva", "Giacenza Negativa"),
                                            CDate(data_da),
                                            1)
                                    End If
                                End If
                            Next

                            ProdottiSuff = True

                            Dim Sa_Nome As String = ""
                            If Not DtCentri Is Nothing Then
                                Dim DrCentro() As DataRow = DtCentri.Select("piva='" & Split(keyMagazzino.key, "|")(0) & "' and sa_cod=" & Split(keyMagazzino.key, "|")(1))
                                If Not DrCentro Is Nothing AndAlso DrCentro.Length > 0 Then
                                    Sa_Nome = DrCentro(0).Item("sa_nome")
                                End If
                            End If

                            objDpi.InserisciRiga_DT_InterventoVerificatoMagazzino(DT_InterventoVerificatoMagazzino, CDate(data_da),
                                                       "Giacenza inizio periodo", keyMagazzino.key, keyMagazzino.value,
                                                           Split(keyMagazzino.key, "|")(0), Split(keyMagazzino.key, "|")(1), Sa_Nome, Split(keyMagazzino.key, "|")(2),
                                                           -1, "", 0, ProdottiSuff, IIf(ProdottiSuff = True, "La Giacenza di tutti i prodotti è positiva", "Esistono prodotti con Giacenza negativa"))

                            ' DATA FINE
                            DtGiacenze = objGiacenze.SchedaGiacenzeMagazzino(data_a,
                                                                                     Split(keyMagazzino.key, "|")(0),
                                                                                     Split(keyMagazzino.key, "|")(1),
                                                                                     Split(keyMagazzino.key, "|")(2),
                                                                                     0,
                                                                                     0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                                                     False,
                                                                                     StringaFilter, "", "", "", "", "", "", "", "",
                                                                                    "", "",
                                                                                    "NO",
                                                                                    objParametri_Server, objParametri_Utenti)

                            ProdottiSuff = True

                            For Each keyProdotti In HashProdottiMovimentati
                                If Split(keyProdotti.key, "|")(7) = Split(keyMagazzino.key, "|")(1) AndAlso Split(keyProdotti.key, "|")(8) = Split(keyMagazzino.key, "|")(2) Then
                                    If DtGiacenze IsNot Nothing AndAlso DtGiacenze.Rows.Count > 0 Then
                                        Dim selectedRows = DtGiacenze.Select("Elem_Cod=" & Split(keyProdotti.key, "|")(0) &
                                                                                " AND Pro_Cod=" & Split(keyProdotti.key, "|")(1) &
                                                                                " AND Mat_Cod=" & Split(keyProdotti.key, "|")(2) &
                                                                                " AND cod_progetto=" & Split(keyProdotti.key, "|")(3) &
                                                                                " AND lotto='" & Split(keyProdotti.key, "|")(4) & "'" &
                                                                                " AND cal_cod=" & Split(keyProdotti.key, "|")(5) &
                                                                                " AND udm_cod=" & Split(keyProdotti.key, "|")(6))

                                        If selectedRows IsNot Nothing AndAlso selectedRows.Length > 0 Then
                                            Dim dtGiacenzeProdotto = selectedRows.CopyToDataTable()

                                            If dtGiacenzeProdotto IsNot Nothing AndAlso dtGiacenzeProdotto.Rows.Count > 0 Then
                                                For g = 0 To dtGiacenzeProdotto.Rows.Count - 1

                                                    Dim Giacenza As Decimal = 0
                                                    If Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Giacenza")) AndAlso IsNumeric(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Giacenza"))) Then
                                                        Giacenza = Math.Round(CDec(dtGiacenzeProdotto.Rows(g).Item("Giacenza")), 3)
                                                    End If

                                                    'oltre al filtro nella query, devo fare anche il filtro su codice
                                                    'perchè molti Decimal vengono salvati come valori infinatamente piccoli
                                                    'ad esempio 0.00003680000000017003
                                                    'If (Giacenza <> 0 And Not (Giacenza < 0.00009 And Giacenza > -0.00009)) Then

                                                    objDpi.InserisciRiga_DT_RisultatiVerificheMagazzino(DT_RisultatiVerificheMagazzino, -11,
                                                                     dtGiacenzeProdotto.Rows(g).Item("Elem_Cod"),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Pro_Cod")), dtGiacenzeProdotto.Rows(g).Item("Pro_Cod"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Mat_Cod")), dtGiacenzeProdotto.Rows(g).Item("Mat_Cod"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Cod_Progetto")), dtGiacenzeProdotto.Rows(g).Item("Cod_Progetto"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Cal_Cod")), dtGiacenzeProdotto.Rows(g).Item("fabbricato_cod"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Lotto")), dtGiacenzeProdotto.Rows(g).Item("Lotto"), ""),
                                                                          dtGiacenzeProdotto.Rows(g).Item("Descrizione_Prodotto"),
                                                                          dtGiacenzeProdotto.Rows(g).Item("Udm_Sim"),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Udm_Cod")), dtGiacenzeProdotto.Rows(g).Item("Udm_Cod"), 0),
                                                                          0, Giacenza,
                                                                          IIf(Giacenza >= 0, True, False),
                                                                          IIf(Giacenza >= 0, "Giacenza prodotto Positiva", "Giacenza prodotto Negativa"),
                                                                          True,
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("piva")), dtGiacenzeProdotto.Rows(g).Item("piva"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("sa_Cod")), dtGiacenzeProdotto.Rows(g).Item("sa_cod"), 0),
                                                                          IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Cal_Cod")), dtGiacenzeProdotto.Rows(g).Item("Cal_Cod"), 0))


                                                    If Giacenza < 0 Then
                                                        ProdottiSuff = False
                                                    End If

                                                    objDpi.InserisciRiga_DT_DettagliGiacenzeMagazzino(DT_DettagliGiacenzeMagazzinoFine, -11,
                                                            idTestataWebService,
                                                            IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("piva")), dtGiacenzeProdotto.Rows(g).Item("piva"), 0),
                                                            IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("sa_Cod")), dtGiacenzeProdotto.Rows(g).Item("sa_cod"), 0),
                                                            dtGiacenzeProdotto.Rows(g).Item("fabbricato_cod"),
                                                            IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Lotto")), dtGiacenzeProdotto.Rows(g).Item("Lotto"), ""),
                                                            IIf(Giacenza >= 0, True, False),
                                                            dtGiacenzeProdotto.Rows(g).Item("Pro_Cod"),
                                                            dtGiacenzeProdotto.Rows(g).Item("elem_cod"),
                                                            Giacenza,
                                                            IIf(Not IsDBNull(dtGiacenzeProdotto.Rows(g).Item("Udm_Cod")), dtGiacenzeProdotto.Rows(g).Item("Udm_Cod"), 0),
                                                            IIf(Giacenza >= 0, "Giacenza Positiva", "Giacenza Negativa"),
                                                            CDate(data_a),
                                                            2)

                                                    'End If 'Filtro arrotondamento giacenza

                                                Next 'ciclo giacenze
                                            End If
                                        Else
                                            Dim Giacenza As Decimal = 0

                                            objDpi.InserisciRiga_DT_DettagliGiacenzeMagazzino(
                                                    DT_DettagliGiacenzeMagazzinoInizio,
                                                    -1,
                                                    idTestataWebService,
                                                    Split(keyMagazzino.key, "|")(0),
                                                    Split(keyMagazzino.key, "|")(1),
                                                    Split(keyMagazzino.key, "|")(2),
                                                    Split(keyProdotti.key, "|")(4),
                                                    IIf(Giacenza >= 0, True, False),
                                                    Split(keyProdotti.key, "|")(1),
                                                    Split(keyProdotti.key, "|")(0),
                                                    Giacenza,
                                                    Split(keyProdotti.key, "|")(6),
                                                    IIf(Giacenza >= 0, "Giacenza Positiva", "Giacenza Negativa"),
                                                    CDate(data_a),
                                                    2)

                                        End If
                                    Else
                                        Dim Giacenza As Decimal = 0

                                        objDpi.InserisciRiga_DT_DettagliGiacenzeMagazzino(
                                                DT_DettagliGiacenzeMagazzinoInizio,
                                                -1,
                                                idTestataWebService,
                                                Split(keyMagazzino.key, "|")(0),
                                                Split(keyMagazzino.key, "|")(1),
                                                Split(keyMagazzino.key, "|")(2),
                                                Split(keyProdotti.key, "|")(4),
                                                IIf(Giacenza >= 0, True, False),
                                                Split(keyProdotti.key, "|")(1),
                                                Split(keyProdotti.key, "|")(0),
                                                Giacenza,
                                                Split(keyProdotti.key, "|")(6),
                                                IIf(Giacenza >= 0, "Giacenza Positiva", "Giacenza Negativa"),
                                                CDate(data_a),
                                                2)
                                    End If
                                End If
                            Next

                            objDpi.InserisciRiga_DT_InterventoVerificatoMagazzino(DT_InterventoVerificatoMagazzino, CDate(data_a),
                                                       "Giacenza fine periodo", keyMagazzino.key, keyMagazzino.value,
                                                           Split(keyMagazzino.key, "|")(0), Split(keyMagazzino.key, "|")(1), Sa_Nome, Split(keyMagazzino.key, "|")(2),
                                                           -11, "", 0, ProdottiSuff, IIf(ProdottiSuff = True, "La Giacenza di tutti i prodotti è positiva", "Esistono prodotti con Giacenza negativa"))

                        Next

                    End If

                    objScriviInteventiEVerifiche.Scrivi___InterventiEVerifiche(piva, sa_cod,
                                                                               veg_cod,
                                                                               disciplinareVerifica,
                                                                               CDate(data_da),
                                                                               CDate(data_a),
                                                                               False,
                                                                               flagMagazzino,
                                                                               False,
                                                                               DT_InterventoVerificato,
                                                                               DT_RisultatiVerifiche,
                                                                               DT_InterventoVerificatoMagazzino,
                                                                               DT_RisultatiVerificheMagazzino,
                                                                               DT_DettagliGiacenzeMagazzinoInizio,
                                                                               DT_DettagliGiacenzeMagazzinoFine,
                                                                               sw.Elapsed.TotalSeconds,
                                                                               objParametri_Server,
                                                                               Enum_OrigineRichiestaVerificaConformita.gsb_massivo_webservice,
                                                                               idTestataWebService)


                    If IDTestataTemp__tmp_FormulatiXPrincipiAttivi > 0 Then
                        PulisciTabellaFormulatiPA(IDTestataTemp__tmp_FormulatiXPrincipiAttivi, objParametri_Server)
                    End If

                End If

                objLog.Scrivi_LOG(objParametri_Server, "ExportVerificaConformitaMassiva.EseguiVerifica()", String.Format("fine verifica per piva {0}, sa_cod {1} e veg_cod {2}. idTestataEngine = {3};  idTestataWebService = {4}", piva, sa_cod, veg_cod, idTestataEngine, idTestataWebService), verificaInviaElasticSearch:=True)

                ' Scrivi report verifica conformità
                If stampaInfoSuFile AndAlso modalitaVerifica = enum_ModalitaVerifica.Entrambi Then
                    ScriviReportVerificaConformita(piva, sa_cod, veg_cod, data_da, data_a, idTestataEngine, idTestataWebService, configServizio, objParametri_Server, flagMagazzino)
                End If

            Next

            result = True

        Catch ex As Exception

            result = False

            Messaggio_di_Ritorno_Opzionale &= " Errore il controllo di verifica conformità massivo: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            Dim objLog As New AgronicaCoreDataProvider.LogProvider
            objLog.Scrivi_LOG(objParametri_Server, "ExportVerificaConformitaMassiva.EseguiVerifica()", Messaggio_di_Ritorno_Opzionale, verificaInviaElasticSearch:=True)

        End Try

        Return result

    End Function

    ' Query e stampa per Verifica_Conformita_Testata, Verifica_Conformita_Risultati e Verifica_Conformita_Esiti
    Public Shared Sub ScriviReportVerificaConformita(piva As String, saCod As Integer, vegCod As Integer, dataDa As String, dataA As String, idTestataNuova As Integer, idTestataVecchia As Integer, configServizio As Configurazione_Servizio, objParametri_Server As AgronicaCoreParametri, flagMagazzino As Boolean)
        Try
            Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd_HHmmss")
            Dim verificheEsitiRead As New VerificheEsitiReaderWithPolling(objParametri_Server)
            Dim dtCompletato = verificheEsitiRead.LeggiRisultatiVerificaConformita(idTestataNuova)

            If dtCompletato IsNot Nothing AndAlso dtCompletato.Rows.Count > 0 Then
                ' Costruisce il percorso della directory dell'utente
                Dim userLogDirectory As String = System.IO.Path.Combine(configServizio.DirectoryLOG, objParametri_Server.LogDescrizioneUtente)

                ' Crea la directory se non esiste
                If Not System.IO.Directory.Exists(userLogDirectory) Then
                    System.IO.Directory.CreateDirectory(userLogDirectory)
                End If

                ' Costruisce il percorso completo del file
                Dim path As String = System.IO.Path.Combine(userLogDirectory, $"ReportVerificaConformita_{piva}_{saCod}_{vegCod}_{timestamp}.txt")

                Using sw As New System.IO.StreamWriter(path, False, Encoding.UTF8)
                    sw.WriteLine("Report Verifica Conformità")
                    sw.WriteLine($"PIVA: {piva}")
                    sw.WriteLine($"SA_COD: {saCod}")
                    sw.WriteLine($"VEG_COD: {vegCod}")
                    sw.WriteLine($"Intervallo: {dataDa} - {dataA}")
                    sw.WriteLine($"IdTestata Engine: {idTestataNuova}")
                    sw.WriteLine($"IdTestata WebService: {idTestataVecchia}")
                    sw.WriteLine($"Timestamp: {timestamp}")
                    sw.WriteLine("----------------------------------------")

                    Dim infoVerifiche As New AgronicaCoreAnagrafeDAL.Info_Verifiche_Massive_R()

                    ' Query per Verifica_Conformita_Testata
                    Dim dtTestataNuova As DataTable = Nothing
                    Dim dtTestataVecchia As DataTable = Nothing
                    If idTestataNuova > 0 Then
                        dtTestataNuova = infoVerifiche.GetTestataById(objParametri_Server, idTestataNuova)
                    End If
                    If idTestataVecchia > 0 Then
                        dtTestataVecchia = infoVerifiche.GetTestataById(objParametri_Server, idTestataVecchia)
                    End If
                    sw.WriteLine("Tabella: Verifica_Conformita_Testata")
                    StampaDataTable(sw, dtTestataNuova, True)
                    StampaDataTable(sw, dtTestataVecchia, False)
                    sw.WriteLine("----------------------------------------")

                    ' Query per Verifica_Conformita_Risultati
                    Dim dtRisultatiNuova As DataTable = Nothing
                    Dim dtRisultatiVecchia As DataTable = Nothing
                    If idTestataNuova > 0 Then
                        dtRisultatiNuova = infoVerifiche.GetRisultatiById(objParametri_Server, idTestataNuova)
                    End If
                    If idTestataVecchia > 0 Then
                        dtRisultatiVecchia = infoVerifiche.GetRisultatiById(objParametri_Server, idTestataVecchia)
                    End If
                    If (dtRisultatiNuova Is Nothing OrElse dtRisultatiNuova.Rows.Count = 0) OrElse (dtRisultatiVecchia Is Nothing OrElse dtRisultatiVecchia.Rows.Count = 0) Then
                        sw.WriteLine("Nessuna operazione trovata con i parametri: ")
                        sw.WriteLine($"PIVA: {piva}")
                        sw.WriteLine($"SA_COD: {saCod}")
                        sw.WriteLine($"VEG_COD: {vegCod}")
                        sw.WriteLine("----------------------------------------")
                    Else
                        sw.WriteLine("Tabella: Verifica_Conformita_Risultati")
                        StampaDataTable(sw, dtRisultatiNuova, True)
                        StampaDataTable(sw, dtRisultatiVecchia, False)
                        sw.WriteLine("----------------------------------------")

                        ' Query per Verifica_Conformita_Esiti (confronto raggruppato per Id_Agenda)
                        Dim dtEsitiConfronto As DataTable = Nothing
                        If idTestataNuova > 0 AndAlso idTestataVecchia > 0 Then
                            dtEsitiConfronto = infoVerifiche.GetEsitiConfrontoRaggruppati(objParametri_Server, idTestataNuova, idTestataVecchia)
                        End If

                        ' Stampa degli esiti raggruppati per Id_Agenda
                        StampaEsitiRaggruppatiPerAgenda(sw, dtEsitiConfronto)
                        sw.WriteLine("----------------------------------------")

                        ' Scrivi Verifica_Conformita_Esiti_Magazzino
                        If flagMagazzino Then
                            Dim dtEsitiMagazzinoNuova As DataTable = Nothing
                            Dim dtEsitiMagazzinoVecchia As DataTable = Nothing
                            If idTestataNuova > 0 Then
                                dtEsitiMagazzinoNuova = infoVerifiche.GetEsitiMagazzinoById(objParametri_Server, idTestataNuova)
                            End If
                            If idTestataVecchia > 0 Then
                                dtEsitiMagazzinoVecchia = infoVerifiche.GetEsitiMagazzinoById(objParametri_Server, idTestataVecchia)
                            End If

                            sw.WriteLine("Tabella: Verifica_Conformita_Esiti_Magazzino")
                            If (dtEsitiMagazzinoNuova Is Nothing OrElse dtEsitiMagazzinoNuova.Rows.Count = 0) AndAlso (dtEsitiMagazzinoVecchia Is Nothing OrElse dtEsitiMagazzinoVecchia.Rows.Count = 0) Then
                                sw.WriteLine("(Nessun dato magazzino trovato)")
                            Else
                                ' Unisci i due DataTable e stampali insieme ordinati per Id_Agenda
                                Dim dtEsitiMagazzinoUniti As DataTable = UnisciDataTableMagazzino(dtEsitiMagazzinoNuova, dtEsitiMagazzinoVecchia)
                                StampaEsitiMagazzinoRaggruppatiPerAgenda(sw, dtEsitiMagazzinoUniti)
                            End If
                            sw.WriteLine("----------------------------------------")

                            Dim dtGiacenzeMagazzinoNuova As DataTable = Nothing
                            Dim dtGiacenzeMagazzinoVecchia As DataTable = Nothing
                            If idTestataNuova > 0 Then
                                dtGiacenzeMagazzinoNuova = infoVerifiche.GetGiacenzeMagazzinoById(objParametri_Server, idTestataNuova)
                            End If
                            If idTestataVecchia > 0 Then
                                dtGiacenzeMagazzinoVecchia = infoVerifiche.GetGiacenzeMagazzinoById(objParametri_Server, idTestataVecchia)
                            End If

                            sw.WriteLine("Tabella: Verifica_Conformita_Esiti_Giacenze")
                            'aggiungi le giacenze
                            If (dtEsitiMagazzinoNuova Is Nothing OrElse dtEsitiMagazzinoNuova.Rows.Count = 0) AndAlso (dtEsitiMagazzinoVecchia Is Nothing OrElse dtEsitiMagazzinoVecchia.Rows.Count = 0) Then
                                sw.WriteLine("(Nessun dato in giacenza trovato)")
                            Else
                                Dim giacenzeUnite As DataTable = UnisciDataTableGiacenze(dtGiacenzeMagazzinoNuova, dtGiacenzeMagazzinoVecchia)
                                StampaGiacenzeMagazzinoRaggruppate(sw, giacenzeUnite)
                            End If
                        Else
                            sw.WriteLine("Tabella: Verifica_Conformita_Esiti_Magazzino")
                            sw.WriteLine("(Flag magazzino disattivato - nessun controllo magazzino eseguito)")
                            sw.WriteLine("----------------------------------------")
                        End If
                    End If
                End Using
            End If
        Catch ex As Exception
            Dim objLog As New AgronicaCoreDataProvider.LogProvider

            objLog.Scrivi_LOG(objParametri_Server, "Errore creazione file :" & ex.Message, "")

        End Try
    End Sub

    ' Stampa un DataTable su uno StreamWriter
    Private Shared Sub StampaDataTable(sw As System.IO.StreamWriter, dt As DataTable, stampaColonne As Boolean)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            sw.WriteLine("(Nessun dato)")
            Return
        End If

        ' Ottieni la larghezza massima per ogni colonna
        Dim colWidths As New List(Of Integer)
        For Each col As DataColumn In dt.Columns
            colWidths.Add(col.ColumnName.Length + 5) ' Iniziamo con la lunghezza del nome della colonna
        Next

        ' Calcola la larghezza massima di ogni colonna
        For Each row As DataRow In dt.Rows
            For i As Integer = 0 To dt.Columns.Count - 1
                colWidths(i) = Math.Max(colWidths(i), row(i).ToString().Length)
            Next
        Next

        ' Intestazioni (opzionali)
        If stampaColonne Then
            For i As Integer = 0 To dt.Columns.Count - 1
                sw.Write(dt.Columns(i).ColumnName.PadRight(colWidths(i)) & vbTab)
            Next
            sw.WriteLine()
        End If

        ' Dati
        For Each row As DataRow In dt.Rows
            For i As Integer = 0 To dt.Columns.Count - 1
                sw.Write(row(i).ToString().PadRight(colWidths(i)) & vbTab)
            Next
            sw.WriteLine()
        Next
    End Sub

    ' Stampa esiti raggruppati per Id_Agenda con header delle sezioni e controllo di uguaglianza per singole righe
    Private Shared Sub StampaEsitiRaggruppatiPerAgenda(sw As System.IO.StreamWriter, dt As DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            sw.WriteLine("(Nessun dato)")
            Return
        End If

        ' Raggruppa i dati per Id_Agenda
        Dim gruppi = dt.AsEnumerable().GroupBy(Function(row) CInt(row("Id_Agenda")))

        For Each gruppo In gruppi
            Dim agendaId As Integer = gruppo.Key
            Dim righeGruppo = gruppo.ToArray()

            ' Ottieni l'header della sezione dal primo record del gruppo
            Dim sectionHeader As String = righeGruppo(0)("SectionHeader").ToString()

            ' Filtra solo le righe che hanno Engine diverso da WebService
            Dim righeDiverse As New List(Of DataRow)
            For Each row As DataRow In righeGruppo
                Dim engine = If(IsDBNull(row("Engine")), "", row("Engine").ToString())
                Dim webService = If(IsDBNull(row("WebService")), "", row("WebService").ToString())

                ' Aggiungi alla lista solo se Engine è diverso da WebService
                If engine <> webService Then
                    righeDiverse.Add(row)
                End If
            Next

            ' Stampa il gruppo solo se ci sono righe con differenze
            If righeDiverse.Count > 0 Then
                sw.WriteLine()
                sw.WriteLine(sectionHeader)

                ' Calcola le larghezze massime per ogni colonna in base alle righe diverse
                Dim maxWidthErrCode As Integer = "Err_Code".Length
                Dim maxWidthDescrizione As Integer = "Descrizione".Length
                Dim maxWidthEngine As Integer = "Engine".Length
                Dim maxWidthWebService As Integer = "WebService".Length

                ' Scorre solo le righe con differenze per calcolare la larghezza massima necessaria
                For Each row As DataRow In righeDiverse
                    If Not IsDBNull(row("Err_Code")) Then
                        maxWidthErrCode = Math.Max(maxWidthErrCode, row("Err_Code").ToString().Length)
                    End If
                    If Not IsDBNull(row("Descrizione")) Then
                        maxWidthDescrizione = Math.Max(maxWidthDescrizione, row("Descrizione").ToString().Length)
                    End If
                    If Not IsDBNull(row("Engine")) Then
                        maxWidthEngine = Math.Max(maxWidthEngine, row("Engine").ToString().Length)
                    End If
                    If Not IsDBNull(row("WebService")) Then
                        maxWidthWebService = Math.Max(maxWidthWebService, row("WebService").ToString().Length)
                    End If
                Next

                ' Aggiunge padding per rendere più leggibile
                maxWidthErrCode += 2
                maxWidthDescrizione += 2
                maxWidthEngine += 2
                maxWidthWebService += 2

                ' Stampa intestazioni colonne per gli esiti con larghezze dinamiche
                sw.WriteLine("Err_Code".PadRight(maxWidthErrCode) & vbTab &
                           "Descrizione".PadRight(maxWidthDescrizione) & vbTab &
                           "Engine".PadRight(maxWidthEngine) & vbTab &
                           "WebService".PadRight(maxWidthWebService))

                ' Stampa linea separatrice sotto le intestazioni
                sw.WriteLine(New String("-"c, maxWidthErrCode) & vbTab &
                           New String("-"c, maxWidthDescrizione) & vbTab &
                           New String("-"c, maxWidthEngine) & vbTab &
                           New String("-"c, maxWidthWebService))

                ' Stampa solo le righe con differenze
                For Each row As DataRow In righeDiverse
                    If Not IsDBNull(row("Err_Code")) Then
                        sw.WriteLine(row("Err_Code").ToString().PadRight(maxWidthErrCode) & vbTab &
                                   row("Descrizione").ToString().PadRight(maxWidthDescrizione) & vbTab &
                                   row("Engine").ToString().PadRight(maxWidthEngine) & vbTab &
                                   row("WebService").ToString().PadRight(maxWidthWebService))
                    End If
                Next

                sw.WriteLine("----------------------------------------")
            End If
        Next
    End Sub

    ' Stampa esiti magazzino raggruppati per Id_Agenda con header delle sezioni
    Private Shared Sub StampaEsitiMagazzinoRaggruppatiPerAgenda(sw As System.IO.StreamWriter, dt As DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            sw.WriteLine("(Nessun dato)")
            Return
        End If

        ' Raggruppa i dati per Id_Agenda
        Dim gruppi = dt.AsEnumerable().GroupBy(Function(row) CInt(row("Id_Agenda")))

        For Each gruppo In gruppi
            Dim agendaId As Integer = gruppo.Key
            Dim righeGruppo = gruppo.ToArray()

            ' Ottieni l'header della sezione dal primo record del gruppo
            Dim sectionHeader As String = righeGruppo(0)("SectionHeader").ToString()

            sw.WriteLine()
            sw.WriteLine(sectionHeader)

            ' Calcola le larghezze massime per le colonne principali
            Dim maxWidthOrigine As Integer = "Origine".Length
            Dim maxWidthIdProdotto As Integer = "Id_Prodotto".Length
            Dim maxWidthCategoria As Integer = "Categoria".Length
            Dim maxWidthLotto As Integer = "Lotto".Length
            Dim maxWidthQtaScaricata As Integer = "Qta_Scaricata".Length
            Dim maxWidthGiacenza As Integer = "Giacenza".Length
            Dim maxWidthConforme As Integer = "Conforme".Length
            Dim maxWidthDettagli As Integer = "Dettagli".Length

            ' Calcola la larghezza massima necessaria per ogni colonna
            For Each row As DataRow In righeGruppo
                If Not IsDBNull(row("Origine")) Then
                    maxWidthOrigine = Math.Max(maxWidthOrigine, row("Origine").ToString().Length)
                End If
                If Not IsDBNull(row("Id_Prodotto")) Then
                    maxWidthIdProdotto = Math.Max(maxWidthIdProdotto, row("Id_Prodotto").ToString().Length)
                End If
                If Not IsDBNull(row("Categoria_Prodotto")) Then
                    maxWidthCategoria = Math.Max(maxWidthCategoria, row("Categoria_Prodotto").ToString().Length)
                End If
                If Not IsDBNull(row("Lotto")) Then
                    maxWidthLotto = Math.Max(maxWidthLotto, row("Lotto").ToString().Length)
                End If
                If Not IsDBNull(row("Qta_Scaricata")) Then
                    maxWidthQtaScaricata = Math.Max(maxWidthQtaScaricata, row("Qta_Scaricata").ToString().Length)
                End If
                If Not IsDBNull(row("Giacenza_Magazzino")) Then
                    maxWidthGiacenza = Math.Max(maxWidthGiacenza, row("Giacenza_Magazzino").ToString().Length)
                End If
                If Not IsDBNull(row("Conforme")) Then
                    maxWidthConforme = Math.Max(maxWidthConforme, row("Conforme").ToString().Length)
                End If
                If Not IsDBNull(row("Dettagli")) Then
                    maxWidthDettagli = Math.Max(maxWidthDettagli, row("Dettagli").ToString().Length)
                End If
            Next

            ' Aggiunge padding per rendere più leggibile
            maxWidthOrigine += 2
            maxWidthIdProdotto += 2
            maxWidthCategoria += 2
            maxWidthLotto += 2
            maxWidthQtaScaricata += 2
            maxWidthGiacenza += 2
            maxWidthConforme += 2
            maxWidthDettagli += 2

            ' Stampa intestazioni colonne con larghezze dinamiche
            sw.WriteLine("Origine".PadRight(maxWidthOrigine) & vbTab &
                       "Id_Prodotto".PadRight(maxWidthIdProdotto) & vbTab &
                       "Categoria".PadRight(maxWidthCategoria) & vbTab &
                       "Lotto".PadRight(maxWidthLotto) & vbTab &
                       "Qta_Scaricata".PadRight(maxWidthQtaScaricata) & vbTab &
                       "Giacenza".PadRight(maxWidthGiacenza) & vbTab &
                       "Conforme".PadRight(maxWidthConforme) & vbTab &
                       "Dettagli".PadRight(maxWidthDettagli))

            ' Stampa linea separatrice sotto le intestazioni
            sw.WriteLine(New String("-"c, maxWidthOrigine) & vbTab &
                       New String("-"c, maxWidthIdProdotto) & vbTab &
                       New String("-"c, maxWidthCategoria) & vbTab &
                       New String("-"c, maxWidthLotto) & vbTab &
                       New String("-"c, maxWidthQtaScaricata) & vbTab &
                       New String("-"c, maxWidthGiacenza) & vbTab &
                       New String("-"c, maxWidthConforme) & vbTab &
                       New String("-"c, maxWidthDettagli))

            ' Stampa i dati del gruppo
            For Each row As DataRow In righeGruppo
                If Not IsDBNull(row("Id_Prodotto")) Then
                    sw.WriteLine(row("Origine").ToString().PadRight(maxWidthOrigine) & vbTab &
                               row("Id_Prodotto").ToString().PadRight(maxWidthIdProdotto) & vbTab &
                               row("Categoria_Prodotto").ToString().PadRight(maxWidthCategoria) & vbTab &
                               row("Lotto").ToString().PadRight(maxWidthLotto) & vbTab &
                               row("Qta_Scaricata").ToString().PadRight(maxWidthQtaScaricata) & vbTab &
                               row("Giacenza_Magazzino").ToString().PadRight(maxWidthGiacenza) & vbTab &
                               row("Conforme").ToString().PadRight(maxWidthConforme) & vbTab &
                               row("Dettagli").ToString().PadRight(maxWidthDettagli))
                End If
            Next

            sw.WriteLine("----------------------------------------")
        Next
    End Sub

    Private Shared Sub StampaGiacenzeMagazzinoRaggruppate(sw As System.IO.StreamWriter, dt As DataTable)
        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            sw.WriteLine("(Nessun dato)")
            Return
        End If

        sw.WriteLine()

        ' Calcola le larghezze massime per le colonne principali basate sulla struttura reale di Verifica_Conformita_Esiti_Giacenze
        Dim maxWidthOrigine As Integer = "Origine".Length
        Dim maxWidthIdTestata As Integer = "Id_Testata".Length
        Dim maxWidthIdProdotto As Integer = "Id_Prodotto".Length
        Dim maxWidthCategoriaProdotto As Integer = "Categoria_Prodotto".Length
        Dim maxWidthLotto As Integer = "Lotto".Length
        Dim maxWidthGiacenzaMagazzino As Integer = "Giacenza_Magazzino".Length
        Dim maxWidthUnitaMisura As Integer = "Unita_Misura".Length
        Dim maxWidthConforme As Integer = "Conforme".Length
        Dim maxWidthDettagli As Integer = "Dettagli".Length
        Dim maxWidthDataGiacenza As Integer = "Data_Giacenza".Length
        Dim maxWidthTipoGiacenza As Integer = "Tipo_Giacenza".Length

        ' Calcola la larghezza massima necessaria per ogni colonna
        For Each row As DataRow In dt.Rows
            If Not IsDBNull(row("Origine")) Then
                maxWidthOrigine = Math.Max(maxWidthOrigine, row("Origine").ToString().Length)
            End If
            If Not IsDBNull(row("Id_Testata")) Then
                maxWidthIdTestata = Math.Max(maxWidthIdTestata, row("Id_Testata").ToString().Length)
            End If
            If Not IsDBNull(row("Id_Prodotto")) Then
                maxWidthIdProdotto = Math.Max(maxWidthIdProdotto, row("Id_Prodotto").ToString().Length)
            End If
            If Not IsDBNull(row("Categoria_Prodotto")) Then
                maxWidthCategoriaProdotto = Math.Max(maxWidthCategoriaProdotto, row("Categoria_Prodotto").ToString().Length)
            End If
            If Not IsDBNull(row("Lotto")) Then
                maxWidthLotto = Math.Max(maxWidthLotto, row("Lotto").ToString().Length)
            End If
            If Not IsDBNull(row("Giacenza_Magazzino")) Then
                maxWidthGiacenzaMagazzino = Math.Max(maxWidthGiacenzaMagazzino, row("Giacenza_Magazzino").ToString().Length)
            End If
            If Not IsDBNull(row("Unita_Misura")) Then
                maxWidthUnitaMisura = Math.Max(maxWidthUnitaMisura, row("Unita_Misura").ToString().Length)
            End If
            If Not IsDBNull(row("Conforme")) Then
                maxWidthConforme = Math.Max(maxWidthConforme, row("Conforme").ToString().Length)
            End If
            If Not IsDBNull(row("Dettagli")) Then
                maxWidthDettagli = Math.Max(maxWidthDettagli, row("Dettagli").ToString().Length)
            End If
            If Not IsDBNull(row("DataGiacenza")) Then
                maxWidthDataGiacenza = Math.Max(maxWidthDataGiacenza, row("DataGiacenza").ToString().Length)
            End If
            If Not IsDBNull(row("TipoGiacenza")) Then
                maxWidthTipoGiacenza = Math.Max(maxWidthTipoGiacenza, row("TipoGiacenza").ToString().Length)
            End If
        Next

        ' Aggiunge padding per rendere più leggibile
        maxWidthOrigine += 2
        maxWidthIdTestata += 2
        maxWidthIdProdotto += 2
        maxWidthCategoriaProdotto += 2
        maxWidthLotto += 2
        maxWidthGiacenzaMagazzino += 2
        maxWidthUnitaMisura += 2
        maxWidthConforme += 2
        maxWidthDettagli += 2
        maxWidthDataGiacenza += 2
        maxWidthTipoGiacenza += 2

        ' Stampa intestazioni colonne con larghezze dinamiche
        sw.WriteLine("Origine".PadRight(maxWidthOrigine) & vbTab &
                   "Id_Testata".PadRight(maxWidthIdTestata) & vbTab &
                   "Id_Prodotto".PadRight(maxWidthIdProdotto) & vbTab &
                   "Categoria_Prodotto".PadRight(maxWidthCategoriaProdotto) & vbTab &
                   "Lotto".PadRight(maxWidthLotto) & vbTab &
                   "Giacenza_Magazzino".PadRight(maxWidthGiacenzaMagazzino) & vbTab &
                   "Unita_Misura".PadRight(maxWidthUnitaMisura) & vbTab &
                   "Conforme".PadRight(maxWidthConforme) & vbTab &
                   "Data_Giacenza".PadRight(maxWidthDataGiacenza) & vbTab &
                   "Tipo_Giacenza".PadRight(maxWidthTipoGiacenza) & vbTab &
                   "Dettagli".PadRight(maxWidthDettagli))

        ' Stampa linea separatrice sotto le intestazioni
        sw.WriteLine(New String("-"c, maxWidthOrigine) & vbTab &
                   New String("-"c, maxWidthIdTestata) & vbTab &
                   New String("-"c, maxWidthIdProdotto) & vbTab &
                   New String("-"c, maxWidthCategoriaProdotto) & vbTab &
                   New String("-"c, maxWidthLotto) & vbTab &
                   New String("-"c, maxWidthGiacenzaMagazzino) & vbTab &
                   New String("-"c, maxWidthUnitaMisura) & vbTab &
                   New String("-"c, maxWidthConforme) & vbTab &
                   New String("-"c, maxWidthDataGiacenza) & vbTab &
                   New String("-"c, maxWidthTipoGiacenza) & vbTab &
                   New String("-"c, maxWidthDettagli))

        ' Stampa i dati
        For Each row As DataRow In dt.Rows
            sw.WriteLine(If(IsDBNull(row("Origine")), "", row("Origine").ToString()).PadRight(maxWidthOrigine) & vbTab &
                       If(IsDBNull(row("Id_Testata")), "", row("Id_Testata").ToString()).PadRight(maxWidthIdTestata) & vbTab &
                       If(IsDBNull(row("Id_Prodotto")), "", row("Id_Prodotto").ToString()).PadRight(maxWidthIdProdotto) & vbTab &
                       If(IsDBNull(row("Categoria_Prodotto")), "", row("Categoria_Prodotto").ToString()).PadRight(maxWidthCategoriaProdotto) & vbTab &
                       If(IsDBNull(row("Lotto")), "", row("Lotto").ToString()).PadRight(maxWidthLotto) & vbTab &
                       If(IsDBNull(row("Giacenza_Magazzino")), "", row("Giacenza_Magazzino").ToString()).PadRight(maxWidthGiacenzaMagazzino) & vbTab &
                       If(IsDBNull(row("Unita_Misura")), "", row("Unita_Misura").ToString()).PadRight(maxWidthUnitaMisura) & vbTab &
                       If(IsDBNull(row("Conforme")), "", row("Conforme").ToString()).PadRight(maxWidthConforme) & vbTab &
                       If(IsDBNull(row("DataGiacenza")), "", row("DataGiacenza").ToString()).PadRight(maxWidthDataGiacenza) & vbTab &
                       If(IsDBNull(row("TipoGiacenza")), "", row("TipoGiacenza").ToString()).PadRight(maxWidthTipoGiacenza) & vbTab &
                       If(IsDBNull(row("Dettagli")), "", row("Dettagli").ToString()).PadRight(maxWidthDettagli))
        Next

        sw.WriteLine("----------------------------------------")
    End Sub

    Private Shared Sub PopolaTabellaFormulatiPA(ByVal principiAttivi As String, ByVal IDTestataTemp As Integer, ByRef objParametri As AgronicaCoreParametri, ByVal sPrincipiAttivi As String, ByVal sPrincipiAttiviPesi As String, ByVal fr_cod As Integer)

        If sPrincipiAttivi = "" Then
            Exit Sub
        End If

        Dim vSplitAP As String() = sPrincipiAttivi.Split("|")
        Dim vSplitAPP As String() = sPrincipiAttiviPesi.Split("|")

        Dim vPrincipiAttivi As String() = principiAttivi.Split(",")

        Dim p As Integer = 0

        For Each sSplitAP In vSplitAP

            Dim vPaTitolo As String() = sSplitAP.Split("§")

            Dim vPaPeso As String() = vSplitAPP(p).Split("§")

            If principiAttivi = "" OrElse vPrincipiAttivi.Contains(vPaTitolo(0)) Then

                Dim stb As New StringBuilder
                stb.Length = 0
                stb.AppendLine(" if not exists ( select  1 from  __tmp_FormulatiXPrincipiAttivi where IDTestataTemp = " & IDTestataTemp & " and  pa_cod = " & vPaTitolo(0) & " and  fr_cod = " & fr_cod & " )")
                stb.AppendLine(" insert __tmp_FormulatiXPrincipiAttivi(IDTestataTemp, pa_cod, fr_cod, titolo, peso) ")
                stb.AppendLine(" values (" & IDTestataTemp & ", " & vPaTitolo(0) & "," & fr_cod & "," & vPaTitolo(2).Replace(",", ".") & "," & vPaPeso(2).Replace(",", ".") & ")")

                '--------------------------------------------------------------------------
                Dim scrivi As New AgronicaCoreDataProvider.DataProvider
                Dim rVal As Boolean = scrivi.EseguiQuery_Scrittura(objParametri, stb.ToString, "PopolaTabellaFormulatiPA")
                '--------------------------------------------------------------------------

            End If

            p += 1

        Next

    End Sub

    Private Shared Sub PulisciTabellaFormulatiPA(idTestataTemp As Integer, ByRef objParametri As AgronicaCoreParametri)

        Dim stb As New StringBuilder
        stb.AppendLine(" delete from __tmp_FormulatiXPrincipiAttivi where IDTestatatemp = " & idTestataTemp & " ")

        Dim scrivi As New AgronicaCoreDataProvider.DataProvider
        Dim rVal As Boolean = scrivi.EseguiQuery_Scrittura(objParametri, stb.ToString, "PulisciTabellaFormulatiPA")


    End Sub

    ' Unisce due DataTable di esiti magazzino aggiungendo una colonna "Origine" per distinguerli
    Private Shared Function UnisciDataTableMagazzino(dtEngine As DataTable, dtWebService As DataTable) As DataTable
        Dim dtUnito As New DataTable()

        ' Se entrambi sono vuoti, restituisci vuoto
        If (dtEngine Is Nothing OrElse dtEngine.Rows.Count = 0) AndAlso (dtWebService Is Nothing OrElse dtWebService.Rows.Count = 0) Then
            Return dtUnito
        End If

        ' Usa il primo DataTable non vuoto come template per la struttura
        Dim dtTemplate As DataTable = If(dtEngine IsNot Nothing AndAlso dtEngine.Rows.Count > 0, dtEngine, dtWebService)

        ' Copia la struttura del DataTable template
        For Each column As DataColumn In dtTemplate.Columns
            dtUnito.Columns.Add(column.ColumnName, column.DataType)
        Next

        ' Aggiungi una colonna "Origine" per distinguere i dati
        dtUnito.Columns.Add("Origine", GetType(String))

        ' Aggiungi i dati da dtEngine (Engine)
        If dtEngine IsNot Nothing AndAlso dtEngine.Rows.Count > 0 Then
            For Each row As DataRow In dtEngine.Rows
                Dim newRow As DataRow = dtUnito.NewRow()
                For Each column As DataColumn In dtEngine.Columns
                    newRow(column.ColumnName) = row(column.ColumnName)
                Next
                newRow("Origine") = "Engine"
                dtUnito.Rows.Add(newRow)
            Next
        End If

        ' Aggiungi i dati da dtWebService (WebService)
        If dtWebService IsNot Nothing AndAlso dtWebService.Rows.Count > 0 Then
            For Each row As DataRow In dtWebService.Rows
                Dim newRow As DataRow = dtUnito.NewRow()
                For Each column As DataColumn In dtWebService.Columns
                    newRow(column.ColumnName) = row(column.ColumnName)
                Next
                newRow("Origine") = "WebService"
                dtUnito.Rows.Add(newRow)
            Next
        End If

        ' Ordina per Id_Agenda
        Dim dv As New DataView(dtUnito)
        dv.Sort = "Id_Agenda ASC, Origine ASC"
        Return dv.ToTable()
    End Function

    Private Shared Function UnisciDataTableGiacenze(dtEngine As DataTable, dtWebService As DataTable) As DataTable
        Dim dtUnito As New DataTable()

        ' Se entrambi sono vuoti, restituisci vuoto
        If (dtEngine Is Nothing OrElse dtEngine.Rows.Count = 0) AndAlso (dtWebService Is Nothing OrElse dtWebService.Rows.Count = 0) Then
            Return dtUnito
        End If

        ' Usa il primo DataTable non vuoto come template per la struttura
        Dim dtTemplate As DataTable = If(dtEngine IsNot Nothing AndAlso dtEngine.Rows.Count > 0, dtEngine, dtWebService)

        ' Copia la struttura del DataTable template
        For Each column As DataColumn In dtTemplate.Columns
            dtUnito.Columns.Add(column.ColumnName, column.DataType)
        Next

        ' Aggiungi una colonna "Origine" per distinguere i dati
        dtUnito.Columns.Add("Origine", GetType(String))

        ' Aggiungi i dati da dtEngine (Engine)
        If dtEngine IsNot Nothing AndAlso dtEngine.Rows.Count > 0 Then
            For Each row As DataRow In dtEngine.Rows
                Dim newRow As DataRow = dtUnito.NewRow()
                For Each column As DataColumn In dtEngine.Columns
                    newRow(column.ColumnName) = row(column.ColumnName)
                Next
                newRow("Origine") = "Engine"
                dtUnito.Rows.Add(newRow)
            Next
        End If

        ' Aggiungi i dati da dtWebService (WebService)
        If dtWebService IsNot Nothing AndAlso dtWebService.Rows.Count > 0 Then
            For Each row As DataRow In dtWebService.Rows
                Dim newRow As DataRow = dtUnito.NewRow()
                For Each column As DataColumn In dtWebService.Columns
                    newRow(column.ColumnName) = row(column.ColumnName)
                Next
                newRow("Origine") = "WebService"
                dtUnito.Rows.Add(newRow)
            Next
        End If

        ' Ordina utilizzando i nomi di campo corretti della tabella Verifica_Conformita_Esiti_Giacenze
        Dim dv As New DataView(dtUnito)
        dv.Sort = "Id_Prodotto ASC, Lotto ASC, Unita_Misura ASC, id_testata ASC, DataGiacenza ASC"
        Return dv.ToTable()
    End Function
End Class
