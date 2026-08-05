

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreVarieBIZ

Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports Newtonsoft.Json

Public Class Indici_Documentale
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

#Region "script services Carica Griglia Ricerca"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Ricerca_Dettagli(ByVal piva As String, ByVal id_indice As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_Dettagli_R
            DT = leggi.Leggi(piva, id_indice, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region


#Region "Caricamento"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Indice(ByVal piva As String, ByVal id_indice As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            DT = leggi.Leggi("", id_indice, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Aree(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Area_R
            DT = leggi.Leggi(piva, 0, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Tipologie(ByVal piva As String, ByVal id_area As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Tipologia_R
            DT = leggi.Leggi(id_area, 0, piva, False, objParametri_Server, True)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#Region "Non Usata"
    'Anna 29/04/22: aggiunti campi al filtro di ricerca
    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_UtentiUpload(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Elenco_R
            DT = leggi.Leggi_UtentiUpload(piva, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
#End Region

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Tipologie_Bloccate(ByVal piva As String, ByVal id_indice As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            DT = leggi.LeggiTipologieBloccate(piva, id_indice, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Elenco_Tipo(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DTRapportiContabili As New DataTable
        Dim ListTipoMacchina As New DropDownList
        Dim DT_Elenco As New DataTable
        Dim DR_Elenco As DataRow


        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            DT_Elenco.Columns.Add(New DataColumn("Elenco_Tipo", GetType(Integer)))
            DT_Elenco.Columns.Add(New DataColumn("Elenco_Cod", GetType(Integer)))
            DT_Elenco.Columns.Add(New DataColumn("Elenco_Cod_String", GetType(String)))
            DT_Elenco.Columns.Add(New DataColumn("Elenco_Des", GetType(String)))
            DT_Elenco.Columns.Add(New DataColumn("Elenco_Key", GetType(String)))

            DR_Elenco = DT_Elenco.NewRow
            DR_Elenco.Item("Elenco_Tipo") = Enum_ElencoTipoEntita.Impresa
            DR_Elenco.Item("Elenco_Cod") = 0
            DR_Elenco.Item("Elenco_Des") = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Scadenzario/Indici_Documentale.aspx", "ImpresaGias"), String)
            DR_Elenco.Item("Elenco_Key") = DR_Elenco.Item("Elenco_Tipo") & "_" & DR_Elenco.Item("Elenco_Cod")
            DT_Elenco.Rows.Add(DR_Elenco)

            DR_Elenco = DT_Elenco.NewRow
            DR_Elenco.Item("Elenco_Tipo") = Enum_ElencoTipoEntita.ContattoGenerico
            DR_Elenco.Item("Elenco_Cod") = 0
            DR_Elenco.Item("Elenco_Des") = DirectCast(System.Web.HttpContext.GetLocalResourceObject("~/Scadenzario/Indici_Documentale.aspx", "ContattoGias"), String)
            DR_Elenco.Item("Elenco_Key") = DR_Elenco.Item("Elenco_Tipo") & "_" & DR_Elenco.Item("Elenco_Cod")
            DT_Elenco.Rows.Add(DR_Elenco)

            'Ora includo anche l'Organismo di Controllo tra i Rapporti Contabili da visualizzare
            Dim leggiRapportiContabili As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
            DTRapportiContabili = leggiRapportiContabili.Contatti_RapportiContabili_Leggi(0, 0, False, False, False, False, False, False, False, enumSelezioneVariabile.Selezione_TabellaCompleta, "Rapporti_Contabili.Cod_rapporto < 0 OR Rapporti_Contabili.Cod_rapporto = -28", "", objParametri_Server)

            If DTRapportiContabili.Rows.Count > 0 Then

                For Each dr As DataRow In DTRapportiContabili.Rows

                    DR_Elenco = DT_Elenco.NewRow
                    DR_Elenco.Item("Elenco_Tipo") = Enum_ElencoTipoEntita.ContattoSpecifico
                    DR_Elenco.Item("Elenco_Cod") = dr.Item("Cod_Rapporto")
                    DR_Elenco.Item("Elenco_Des") = AgronicaAgenda_2010.Contatti & " " & dr.Item("Rapporto_Des")
                    DR_Elenco.Item("Elenco_Key") = DR_Elenco.Item("Elenco_Tipo") & "_" & DR_Elenco.Item("Elenco_Cod")
                    DT_Elenco.Rows.Add(DR_Elenco)

                Next

            End If


            DR_Elenco = DT_Elenco.NewRow
            DR_Elenco.Item("Elenco_Tipo") = Enum_ElencoTipoEntita.SpecieVegetale
            DR_Elenco.Item("Elenco_Cod") = 0
            DR_Elenco.Item("Elenco_Des") = AgronicaAgenda_2010.SpecieVegetale
            DR_Elenco.Item("Elenco_Key") = DR_Elenco.Item("Elenco_Tipo") & "_" & DR_Elenco.Item("Elenco_Cod")
            DT_Elenco.Rows.Add(DR_Elenco)

            DR_Elenco = DT_Elenco.NewRow
            DR_Elenco.Item("Elenco_Tipo") = Enum_ElencoTipoEntita.MacchinaGenerica
            DR_Elenco.Item("Elenco_Cod") = 0
            DR_Elenco.Item("Elenco_Des") = AgronicaAgenda_2010.MacchinaAttrezzaturaGias
            DR_Elenco.Item("Elenco_Key") = DR_Elenco.Item("Elenco_Tipo") & "_" & DR_Elenco.Item("Elenco_Cod")
            DT_Elenco.Rows.Add(DR_Elenco)

            'Ora includo anche i tipi specifici di Macchina da visualizzare
            CaricaListControl.TipoMacchine(ListTipoMacchina, 1, "", "", objParametri_Server, primaRigaVuota:=False)
            If ListTipoMacchina IsNot Nothing AndAlso ListTipoMacchina.Items IsNot Nothing Then
                For Each macchina In ListTipoMacchina.Items
                    DR_Elenco = DT_Elenco.NewRow
                    DR_Elenco.Item("Elenco_Tipo") = Enum_ElencoTipoEntita.MacchinaSpecifica
                    DR_Elenco.Item("Elenco_Cod") = 0
                    DR_Elenco.Item("Elenco_Cod_String") = macchina.Value
                    DR_Elenco.Item("Elenco_Des") = macchina.Text
                    DR_Elenco.Item("Elenco_Key") = DR_Elenco.Item("Elenco_Tipo") & "_" & DR_Elenco.Item("Elenco_Cod_String")
                    DT_Elenco.Rows.Add(DR_Elenco)
                Next
            End If

            DR_Elenco = DT_Elenco.NewRow
            DR_Elenco.Item("Elenco_Tipo") = Enum_ElencoTipoEntita.CentroAziendale
            DR_Elenco.Item("Elenco_Cod") = 0
            DR_Elenco.Item("Elenco_Des") = AgronicaAgenda_2010.CentroAziendale
            DR_Elenco.Item("Elenco_Key") = DR_Elenco.Item("Elenco_Tipo") & "_" & DR_Elenco.Item("Elenco_Cod")
            DT_Elenco.Rows.Add(DR_Elenco)

            DR_Elenco = DT_Elenco.NewRow
            DR_Elenco.Item("Elenco_Tipo") = Enum_ElencoTipoEntita.Campo
            DR_Elenco.Item("Elenco_Cod") = 0
            DR_Elenco.Item("Elenco_Des") = AgronicaAgenda_2010.Campo
            DR_Elenco.Item("Elenco_Key") = DR_Elenco.Item("Elenco_Tipo") & "_" & DR_Elenco.Item("Elenco_Cod")
            DT_Elenco.Rows.Add(DR_Elenco)

            DR_Elenco = DT_Elenco.NewRow
            DR_Elenco.Item("Elenco_Tipo") = Enum_ElencoTipoEntita.Appezzamento
            DR_Elenco.Item("Elenco_Cod") = 0
            DR_Elenco.Item("Elenco_Des") = AgronicaAgenda_2010.Appezzamento
            DR_Elenco.Item("Elenco_Key") = DR_Elenco.Item("Elenco_Tipo") & "_" & DR_Elenco.Item("Elenco_Cod")
            DT_Elenco.Rows.Add(DR_Elenco)


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT_Elenco, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Udm() As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim leggi As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
            DT = leggi.Leggi(0, 0, "", "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "Udm_Des", objParametri_Server)

            'Lettura degli Indici Documentali


            If DT.Rows.Count > 0 Then

                DT.Columns.Add(New DataColumn("Elenco_Valore_Cod", GetType(Integer)))
                DT.Columns.Add(New DataColumn("Elenco_Valore_Des", GetType(String)))

                For Each dr As DataRow In DT.Rows

                    dr.Item("Elenco_Valore_Cod") = dr.Item("Udm_Cod")
                    dr.Item("Elenco_Valore_Des") = dr.Item("Udm_Des")

                Next

            End If


            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region



#Region "Scrittura"

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaIndice(ByVal piva As String,
                                          ByVal id_indice As Integer,
                                          ByVal titoloindice As String,
                                          ByVal chkriservato As Boolean,
                                          ByVal chkobbligatorio As Boolean,
                                          ByVal chkindice_speciale As Boolean,
                                          ByVal tipocampo As Integer,
                                          ByVal tipodato As String,
                                          ByVal elenco_tipo As Integer,
                                          ByVal elenco_cod As Integer,
                                          ByVal elenco_val As String,
                                          ByVal elenco_cod_string As String,
                                          ByVal validita_inizio As String,
                                          ByVal validita_fine As String,
                                          ByVal righeInseriteGrid_Dettagli As String,
                                          ByVal righeModificateGrid_Dettagli As String,
                                          ByVal righeCancellateGrid_Dettagli As String
                                          ) As RispostaStandard
        '      

        Dim tipooperazione As Integer = 0

        Dim Validita_Inizio_Date As Date = "01/01/1900"
        Dim Validita_Fine_Date As Date = "31/12/2100"

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            If IsDate(validita_inizio) Then
                Validita_Inizio_Date = CDate(validita_inizio)
            End If

            If IsDate(validita_fine) Then
                Validita_Fine_Date = CDate(validita_fine)
            End If

            Select Case id_indice
                Case 0
                    tipooperazione = 1
                Case Else
                    tipooperazione = 2
            End Select

            'Pulizia Campi
            Select Case tipocampo

                Case 0 'Testo Libero

                    elenco_tipo = 0
                    elenco_cod = 0
                    elenco_val = 0

                Case 1 'Dettagli

                    elenco_tipo = 0
                    elenco_cod = 0
                    elenco_val = 0
                    tipodato = ""

                Case 2 'Elenco

                    tipodato = ""

            End Select


            Dim aggiorna As New AgronicaCoreScadenziario_BIZ.Alert_W


            id_indice = aggiorna.AggiornaIndice(tipooperazione,
                                                piva,
                                               id_indice,
                                               titoloindice,
                                               IIf(chkriservato, 1, 0),
                                               IIf(chkobbligatorio, 1, 0),
                                               IIf(chkindice_speciale, 1, 0),
                                               tipocampo,
                                               tipodato,
                                               elenco_tipo,
                                               elenco_cod,
                                               elenco_val,
                                               Validita_Inizio_Date,
                                               Validita_Fine_Date,
                                               righeInseriteGrid_Dettagli,
                                               righeModificateGrid_Dettagli,
                                               righeCancellateGrid_Dettagli,
                                               objParametri_Server,
                                               elenco_cod_string:=elenco_cod_string)


            r.RispostaStringa = CStr(id_indice)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        End Try

        Return r

    End Function

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim url As String = hdPaginaRedirect.Value &
                            "?p=" & Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder) &
                            "&tab_default=tab_indici"

        Response.Redirect(url)

    End Sub




#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("IndiceDocumentale"), String)

        'Master.SetTitoloPagina(121)

        Master.flag_MostraBtnIndietro = True
        AddHandler Master.ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto

        inizializzoObjParametri()
        inizializzoParametriPagina()

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Scadenzario_IndiciRicerca,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Scadenzario_IndiciRicerca,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If UtenteAbilitatoLettura = False Then
            Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
        End If

        hdPiva.Value = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                          AgroKey_EncoderDecoder,
                                          Server)


        hdPiva_Codificata.Value = Stringa_Codifica(hdPiva.Value, AgroKey_EncoderDecoder, HttpContext.Current.Session)

        Dim PaginaRedirect As String = ""

        hdPaginaRedirect.Value = ""
        If Not Request.QueryString("origine") Is Nothing Then
            hdPaginaRedirect.Value = CStr(Request.QueryString("origine"))
        Else
            hdPaginaRedirect.Value = "../Menu/MenuBS_Agenda_Nuovo.aspx"
        End If

        hdId_Area.Value = 0
        hdId_Indice.Value = 0
        If Not Request.QueryString("id_indice") Is Nothing Then

            hdId_Indice.Value = CInt(Request.QueryString("id_indice"))

        End If

        'Impostazione Possibilità di modificare l'indice (solo se Utente = SuperUser)
        If objParametri_Utenti.UtenteCodFiscale = objParametri_Utenti.PivaSuperUser Then
            hdRiservato.Value = 1
        Else
            hdRiservato.Value = 0
        End If

        If Not Page.IsPostBack Then
            'caricaControlli()
        End If

        If Not IsNothing(Session("ParametriAgenda_2010")) Then

            Dim objParametriAgenda_2010 As New ParametriAgenda_2010
            objParametriAgenda_2010.Leggi()

        End If



    End Sub


    Private Sub inizializzoParametriPagina()


    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

End Class