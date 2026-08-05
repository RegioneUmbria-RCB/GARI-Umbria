Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports InData.Operazione
Imports AgronicaCoreUtentiDAL

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Rilievi
    Inherits System.Web.Services.WebService

#Region "Web Service"

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PopolaRilievo_APP(
            ByVal objP_super_server As String,
            ByVal objP_server As String,
            ByVal objP_utenti As String,
            lavCod As String,
            vegCod As String,
            dpiCod As String,
            idRcdpi As String,
            dpiPubblicoPrivato As String,
            personalizzate As Boolean,
            filtraSpecie As Boolean
    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)

        Try

            Select Case lavCod

                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO
                    r.RispostaStringa = LetturaMisureXAvversita_APP(vegCod, dpiCod, idRcdpi, dpiPubblicoPrivato, 0, personalizzate, objParametri_Server, objParametri_Super_Server)

                Case LAVCOD_RILIEVO_INDICI_MATURITA
                    r.RispostaStringa = LetturaIndiciMaturita_APP(vegCod, personalizzate, 0, filtraSpecie, objParametri_Server, objParametri_Super_Server)

                Case LAVCOD_DANNI_RACCOLTA
                    r.RispostaStringa = LetturaDanniRaccolta_APP(vegCod, personalizzate, filtraSpecie, objParametri_Server, objParametri_Super_Server)

                Case LAVCOD_FASI_FENOLOGICHE
                    r.RispostaStringa = LetturaFasiFenologiche_APP(objParametri_Super_Server, objParametri_Server, objParametri_Utenti, vegCod, False, personalizzate)

                    'Case LAVCOD_RILIEVO_ERBE_INFESTANTI
                    '    r.RispostaStringa = LetturaMisureXAvversita(vegCod, dpiCod, idRcdpi, dpiPubblicoPrivato, 1, personalizzate, objParametri_Server, objParametri_Super_Server)

                    'Case LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
                    '    r.RispostaStringa = LetturaIndiciMaturita(vegCod, personalizzate, 1, objParametri_Server, objParametri_Super_Server)

            End Select

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PopolaRilievo(lavCod As String, vegCod As String,
                                  dpiCod As String, idRcdpi As String, dpiPubblicoPrivato As String, personalizzate As Boolean,
                                  ByVal objP_Super_Server As String, ByVal objP_Server As String, ByVal objP_Utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Server)
        Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Super_Server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Utenti)

        Dim objAgronicaControlli As New AgronicaControlli_2010.Rilievi

        Try

            r.RispostaStringa = objAgronicaControlli.Popola_Rilievo_In_base_Al_Lav_Cod(lavCod, vegCod, dpiCod, idRcdpi,
                                                                        dpiPubblicoPrivato, New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC),
                                                                        Nothing, personalizzate, False, False,
                                                                        objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function PopolaRilievo_NG(InData As CoreWS_Generic(Of PopolaRilievo)) As rispostaStandard(Of Object)
        Dim r As New rispostaStandard(Of Object)

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Input As PopolaRilievo = InData.InData
            Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

            Dim objAgronicaControlli As New AgronicaControlli_2010.Rilievi


            r.RispostaStringa = objAgronicaControlli.Popola_Rilievo_In_base_Al_Lav_Cod(objParametri_Input.lavCod, objParametri_Input.vegCod, objParametri_Input.dpiCod,
                                                                   objParametri_Input.idRcdpi, objParametri_Input.dpiPubblicoPrivato, objParametri_Input.eserciziCDC,
                                                                   objParametri_Input.avversitaGruppo, True, True, True,
                                                                    objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_ListaRilievi_ToKendoGrid_new(InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Try
            Dim datiRequest As String = JsonConvert.SerializeObject(InData)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local
            serializerSettings.NullValueHandling = NullValueHandling.Ignore
            serializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore
            Dim objRequest = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiRilievi))(datiRequest, serializerSettings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objRequest.objP.objP_utenti)
            Dim leggiRilievi As LeggiRilievi = objRequest.InData

            HttpContext.Current.Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
            HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
            HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

            Dim rilievi_R As New AgronicaCoreContabBIZ.Rilievi_R

            Dim dt As DataTable = rilievi_R.LeggiRilieviNew(leggiRilievi, objParametri_Server, objParametri_Utenti)

            'aggiungo la chiave composita
            dt.Columns.Add("chiave_composita", GetType(String))

            For i = 0 To dt.Rows.Count - 1
                dt.Rows(i).Item("chiave_composita") = dt.Rows(i).Item("Data2") & "_" &
                                                        dt.Rows(i).Item("id_agenda") & "_" &
                                                        dt.Rows(i).Item("Lav_cod") & "_" &
                                                        dt.Rows(i).Item("Piva") & "_" &
                                                        dt.Rows(i).Item("Sa_Cod") & "_" &
                                                        dt.Rows(i).Item("Blocco_Flag") & "_" &
                                                        dt.Rows(i).Item("Veg_Cod")

                'aggiorno la voce sulla Specie Vegetale, se è nulla, devo mettere 'Nessuna Specie'
                If IsDBNull(dt.Rows(i).Item("Veg_Des")) AndAlso IsDBNull(dt.Rows(i).Item("Des_DestUso")) Then
                    dt.Rows(i).Item("Veg_Des") = AgronicaCoreDataProvider.My.Resources.Gias.NessunaSpecie
                End If
            Next

            ''Carico la lingua per i file resx delle traduzioni
            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Piva_Riferimento", AgronicaCoreDataProvider.My.Resources.Gias.PIVAImpresaReferente, "string"))
            l.Add(New ColonneNome("Impresa_Riferimento", AgronicaCoreDataProvider.My.Resources.Gias.OrganismoReferente, "string"))
            l.Add(New ColonneNome("Piva", AgronicaCoreDataProvider.My.Resources.Gias.Piva, "string"))
            l.Add(New ColonneNome("rag_soc", AgronicaCoreDataProvider.My.Resources.Gias.RagioneSociale, "string"))
            l.Add(New ColonneNome("CUAA", AgronicaCoreDataProvider.My.Resources.Gias.CodiceUnicoAziendaAgricolaSigla, "string") With {._Display = False})
            l.Add(New ColonneNome("Sa_Nome_Rilievo", AgronicaCoreDataProvider.My.Resources.Gias.CentroAziendale, "string"))
            l.Add(New ColonneNome("Validita_Inizio_Rilievo", AgronicaCoreDataProvider.My.Resources.Gias.Data, "date"))
            l.Add(New ColonneNome("Regione", AgronicaCoreDataProvider.My.Resources.Gias.Regione, "string"))
            l.Add(New ColonneNome("Provincia", AgronicaCoreDataProvider.My.Resources.Gias.Provincia, "string"))
            l.Add(New ColonneNome("Localita", AgronicaCoreDataProvider.My.Resources.Gias.Localita, "string"))
            l.Add(New ColonneNome("Indirizzo", AgronicaCoreDataProvider.My.Resources.Gias.Indirizzo, "string"))
            l.Add(New ColonneNome("Veg_Des", AgronicaCoreDataProvider.My.Resources.Gias.SpecieVegetale, "string"))
            l.Add(New ColonneNome("Cul_Des", AgronicaCoreDataProvider.My.Resources.Gias.Varieta, "string"))
            l.Add(New ColonneNome("Des_DestUso", AgronicaCoreDataProvider.My.Resources.Gias.DestinazioneUso, "string"))
            l.Add(New ColonneNome("Blocco_Flag", "Blocco_Flag", "number") With {._hidden = True})
            l.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Des", AgronicaCoreDataProvider.My.Resources.Gias.Operazione, "string"))
            l.Add(New ColonneNome("APP_NOME", AgronicaCoreDataProvider.My.Resources.Gias.Appezzamento, "string"))
            l.Add(New ColonneNome("AppezzamentoID", AgronicaCoreDataProvider.My.Resources.Gias.CodiciAppezzamento, "string"))
            l.Add(New ColonneNome("Latitude", AgronicaCoreDataProvider.My.Resources.Gias.Latitudine, "string"))
            l.Add(New ColonneNome("Longitude", AgronicaCoreDataProvider.My.Resources.Gias.Longitudine, "string"))
            l.Add(New ColonneNome("Allegati", AgronicaCoreDataProvider.My.Resources.Gias.Documento, "string"))
            l.Add(New ColonneNome("Note", AgronicaCoreDataProvider.My.Resources.Gias.Note, "string"))
            l.Add(New ColonneNome("SUP_APP", AgronicaCoreDataProvider.My.Resources.Gias.SuperficieTotale, "string"))
            l.Add(New ColonneNome("Superficie_Trattata", AgronicaCoreDataProvider.My.Resources.Gias.SuperficieUtilizzata, "string"))
            l.Add(New ColonneNome("Rilievo", AgronicaCoreDataProvider.My.Resources.Gias.IndiceRilievo, "string"))
            l.Add(New ColonneNome("Valore_Rilievo", AgronicaCoreDataProvider.My.Resources.Gias.ValoreRilievo, "string"))
            l.Add(New ColonneNome("Causali_Rilievo", AgronicaCoreDataProvider.My.Resources.Gias.CausaliRilievo, "string"))
            l.Add(New ColonneNome("id_agenda", "ID", "string"))
            l.Add(New ColonneNome("UserCreazione", AgronicaCoreDataProvider.My.Resources.Gias.UtenteCreazione, "string"))
            l.Add(New ColonneNome("UserModifica", AgronicaCoreDataProvider.My.Resources.Gias.UtenteModifica, "string"))
            l.Add(New ColonneNome("Data2", AgronicaCoreDataProvider.My.Resources.Gias.DataMovimentoAbbr, "date") With {._Display = False})
            l.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

#End Region

#Region "Chiamate APP"

    Private Shared Function LetturaMisureXAvversita_APP(
            vegCod As Integer, dpiCod As Integer, idRcdpi As Integer, pubblicoPrivato As Integer,
            tipoTestata As Integer, personalizzate As Boolean,
            ByRef objParametri_Server As AgronicaCoreParametri,
            ByRef objParametri_Super_Server As AgronicaCoreParametri) As String

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_input
        objParametriIngresso.Veg_Cod = vegCod
        objParametriIngresso.Dpi_Cod = dpiCod
        objParametriIngresso.Id_Rcdpi = idRcdpi
        objParametriIngresso.Dpi_Pubblico_Privato = pubblicoPrivato
        objParametriIngresso.TipoTestata = tipoTestata

        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)


        objParametriIngresso.Personalizzate = personalizzate
        objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        objParametriIngresso.Url = url & "/MisureXAvversita"

        Dim objWS As New AgronicaCoreWebService.MisureXAvversita_WS
        Dim objParametriUscitaAgronicaWebService As AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_output = objWS.MisureXAvversita(objParametriIngresso)

        If objParametriUscitaAgronicaWebService.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscitaAgronicaWebService.MessaggioErrore)
        End If

        Dim objParametriUscitaCoreWS As New AgronicaCoreMetaSchemaBIZ.MisuraxAvversita_output_APP
        objParametriUscitaCoreWS.MessaggioErrore = objParametriUscitaAgronicaWebService.MessaggioErrore
        objParametriUscitaCoreWS.ListaAPP_MisuraxAvversita = (
            From m In objParametriUscitaAgronicaWebService.ListaMisureXAvversita
            Select New AgronicaCoreMetaSchemaBIZ.APP_MisuraxAvversita With {
                .COD = m.Cod,
                .UDM_COD = m.Udm_Cod,
                .AV_COD = m.Av_Cod,
                .VEG_COD = m.Veg_Cod,
                .Fondamentale = 1,
                .ff_Cod = m.FF_Cod,
                .ordine = m.Ordine,
                .AV_GRU = m.Av_Gru
            }
        ).ToList()

        Dim mxaAnag As New AgronicaCoreMetaSchemaDAL.MisuraxAvversita_Anagrafiche_R
        Dim dtMxAnag As DataTable =
            mxaAnag.Leggi(0, 0, 0, 0, "", "", objParametri_Server)

        objParametriUscitaCoreWS.ListaAPP_MisuraXAvversita_Anagrafiche = (
            From d In dtMxAnag.AsEnumerable()
            Select New AgronicaCoreMetaSchemaBIZ.APP_MisuraXAvversita_Anagrafiche With {
                .MxAV_Cod = d("MxAV_Cod"),
                .Anag_des = d("Anag_des"),
                .Anag_valore = d("Anag_valore")
                }).ToList()


        Dim rval As String =
            JsonConvert.SerializeObject(objParametriUscitaCoreWS)

        Return rval

    End Function

    Private Shared Function LetturaIndiciMaturita_APP(vegCod As Integer, personalizzate As Boolean, tipoTestata As Integer, filtraSpecie As Boolean,
                                                  ByRef objParametri_Server As AgronicaCoreParametri,
                                                  ByRef objParametri_Super_Server As AgronicaCoreParametri) As String

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.IndiciMaturita_input
        objParametriIngresso.veg_cod = vegCod
        objParametriIngresso.lav_cod = 0
        objParametriIngresso.FiltraSpecie = filtraSpecie

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        objParametriIngresso.Personalizzate = personalizzate
        objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        objParametriIngresso.Url = url & "/IndiciMaturita"
        objParametriIngresso.tipoTestata = tipoTestata
        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod

        Dim objWS As New AgronicaCoreWebService.IndiciMaturita_WS
        Dim objParametriUscitaAgronicaWebService As AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output = objWS.IndiciMaturita(objParametriIngresso)

        If objParametriUscitaAgronicaWebService.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscitaAgronicaWebService.MessaggioErrore)
        End If

        Dim objParametriUscitaCoreWS As New AgronicaCoreMetaSchemaBIZ.IndiciMaturita_output_APP
        objParametriUscitaCoreWS.MessaggioErrore = objParametriUscitaAgronicaWebService.MessaggioErrore

        For Each m In objParametriUscitaAgronicaWebService.ListaIndiciMaturita

            If (From aa In objParametriUscitaCoreWS.ListaAPP_IndiciMaturita
                Where aa.IND_MAT_COD = m.ind_mat_cod).ToList.Count = 0 Then

                objParametriUscitaCoreWS.ListaAPP_IndiciMaturita.Add(
                    New AgronicaCoreMetaSchemaBIZ.APP_IndiciMaturita With {
                        .IND_MAT_COD = m.ind_mat_cod,
                        .IND_MAT_DES = m.ind_mat_des,
                        .DATA_AGG = AGRODATAINIZIO,
                        .LAV_COD = m.lav_cod
                    })
            End If


            If (From aa In objParametriUscitaCoreWS.ListaAPP_MisuraXIndiciMaturita
                Where aa.IND_MAT_COD = m.ind_mat_cod _
                    And aa.UDM_COD = m.udm_cod).ToList.Count = 0 Then

                objParametriUscitaCoreWS.ListaAPP_MisuraXIndiciMaturita.Add(
                    New AgronicaCoreMetaSchemaBIZ.APP_MisuraXIndiciMaturita With {
                        .IND_MAT_COD = m.ind_mat_cod,
                        .UDM_COD = m.udm_cod,
                        .DATA_AGG = AGRODATAINIZIO
                    })

            End If

            If (From aa In objParametriUscitaCoreWS.ListaAPP_IndiciMaturitaxSpecieVegetali
                Where aa.IND_MAT_COD = m.ind_mat_cod _
                    And aa.VEG_COD = m.veg_cod).ToList.Count = 0 Then

                objParametriUscitaCoreWS.ListaAPP_IndiciMaturitaxSpecieVegetali.Add(
                    New AgronicaCoreMetaSchemaBIZ.APP_IndiciMaturitaxSpecieVegetali With {
                        .IND_MAT_COD = m.ind_mat_cod,
                        .VEG_COD = m.veg_cod,
                        .REG_COD = m.reg_cod,
                        .CLASSE = m.classe,
                        .DATA_AGG = AGRODATAINIZIO,
                        .Flag_Raccolta = m.flag_raccolta
                    })

            End If

        Next

        Dim reader As New AgronicaCoreMetaSchemaBIZ.MisurexIndiciMaturita_Anagrafiche
        Dim dtMxInAnag = reader.GetValuesFor(0, 0, 0, objParametri_Server)

        objParametriUscitaCoreWS.ListaAPP_MisuraXIndiciMaturita_Anagrafiche = (
            From d In dtMxInAnag.AsEnumerable()
            Select New AgronicaCoreMetaSchemaBIZ.APP_MisuraXIndiciMaturita_Anagrafiche With {
                .MxIn_Cod = d("Ind_Mat_Cod"),
                .UDM_COD = d("UDM_COD"),
                .Anag_des = d("Anag_des"),
                .Anag_valore = d("Anag_valore")
            }).ToList()

        Dim rval As String =
            JsonConvert.SerializeObject(objParametriUscitaCoreWS)

        Return rval

    End Function

    Private Shared Function LetturaDanniRaccolta_APP(vegCod As Integer,
                                                 personalizzate As Boolean,
                                                 filtraSpecie As Boolean,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByRef objParametri_Super_Server As AgronicaCoreParametri) As String

        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_input
        objParametriIngresso.Veg_Cod = vegCod

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        objParametriIngresso.Personalizzate = personalizzate
        objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser
        objParametriIngresso.Url = url & "/MisureXDanniRaccolta"
        objParametriIngresso.FiltraSpecie = filtraSpecie

        Dim objWS As New AgronicaCoreWebService.MisureXDanniRaccolta_WS
        Dim objParametriUscita As AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output = objWS.MisureXDanniRaccolta(objParametriIngresso)

        If objParametriUscita.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscita.MessaggioErrore)
        End If

        Dim objParametriUscitaCoreWS As New AgronicaCoreMetaSchemaBIZ.MisuraXDanniRaccolta_output_APP
        objParametriUscitaCoreWS.MessaggioErrore = objParametriUscita.MessaggioErrore

        objParametriUscitaCoreWS.ListaAPP_MisuraXDanniRaccolta = (
            From m In objParametriUscita.ListaMisureXDanniRaccolta
            Select New AgronicaCoreMetaSchemaBIZ.APP_MisuraXDanniRaccolta With {
                .Dr_Cod = m.Dr_Cod,
                .Dr_Des = m.Dr_Des,
                .Udm_Cod = m.Udm_Cod,
                .Udm_Des = m.Udm_Des,
                .Veg_Cod = m.Veg_Cod,
                .Udm_Sim = m.Udm_Sim,
                .Flag_Visibile = m.Visibile
            }
        ).ToList()

        Dim rval As String =
            JsonConvert.SerializeObject(objParametriUscitaCoreWS)

        Return rval

    End Function

    Private Shared Function LetturaFasiFenologiche_APP(
        ByRef objParametri_Super_Server As AgronicaCoreParametri, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri,
        vegCod As Integer,
        fasiOld As Boolean,
        personalizzate As Boolean
    ) As String

        'lettura fasi fenologiche (da web service sia nuove fasi bbch sia vecchie fasi)
        Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
        objParametriIngresso.Veg_Cod = CInt(vegCod)

        Dim leggiPath As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim url As String = leggiPath.Leggi_Valore(0, "GiasOnline_WS_SpecieVegetali_IAgroAPI_SpecieVegetali", "", "", objParametri_Super_Server)

        Dim objParametriUscitaCoreWS As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output_APP
        Dim objParametriUscitaAgronicaWebService As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
        Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

        objParametriIngresso.Personalizzate = personalizzate
        objParametriIngresso.Piva_Superuser = objParametri_Server.PivaSuperUser

        objParametriIngresso.Lingua_Cod = objParametri_Server.Lingua_Cod


        objParametriIngresso.Url = url & "/FasiFenologiche"
        objParametriUscitaAgronicaWebService = objFasi_WS.FasiFenologiche(objParametriIngresso)

        If objParametriUscitaAgronicaWebService.MessaggioErrore <> "" Then
            Throw New Exception(objParametriUscitaAgronicaWebService.MessaggioErrore)
        End If

        objParametriUscitaCoreWS.ListaFasiFenologiche = (
            From m In objParametriUscitaAgronicaWebService.ListaFasiFenologiche
            Select New AgronicaCoreMetaSchemaBIZ.APP_SpecieVegetaliXStadiCrescita With {
                .Cod_SS = m.Cod_SS,
                .Veg_Cod = m.Veg_Cod,
                .ID_BBCH = m.ID_BBCH,
                .Cod_MS = 0,
                .Progressivo = 0,
                .Descrizione = m.Descrizione & " ( BBCH " & m.Stadio & " )",
                .FF_Cod = m.FF_Cod,
                .Flag_Fioritura = m.Fioritura,
                .Flag_Visibile = m.Visibile
            }
        ).ToList()

        Dim rval As String =
            JsonConvert.SerializeObject(objParametriUscitaCoreWS.ListaFasiFenologiche)

        Return rval

    End Function

#End Region

End Class