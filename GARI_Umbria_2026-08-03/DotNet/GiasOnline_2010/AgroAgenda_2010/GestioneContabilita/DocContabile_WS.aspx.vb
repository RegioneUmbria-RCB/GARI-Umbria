Imports System.Web.Services
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreContabBIZ.FF_ConferimentoPomodoro
Imports AgronicaCoreContabHLP.Contabilita
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreXML.XML_Stampe
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgroAgenda_2010.Resources
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModelsSTD.exceptions

Public Class DocContabile_WS
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function Url_Indietro_DocContabile(codPagRitorno As Integer, piva As String, ricercaType As String, ricercaDoc As String) As RispostaStandard

        Dim r As New RispostaStandard With {.RispostaOK = True}

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametriServer)

        Dim flag_MenuBS_2017 = Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso DTConfigSiti.Rows(0)("valore").ToString.ToLower = "true"

        Dim objParametriAgenda As New ParametriAgenda()

        r.RispostaStringa = Calcola_Url_Indietro_DocContabile(codPagRitorno, piva, ricercaType, ricercaDoc, flag_MenuBS_2017, objParametriServer, objParametriAgenda)

        Return r

    End Function

    Public Shared Function Calcola_Url_Indietro_DocContabile(codPagRitorno As Integer, piva As String, ricercaType As String, ricercaDoc As String, flag_MenuBS_2017 As Boolean, objParametriServer As AgronicaCoreParametri, objParametriAgenda As ParametriAgenda) As String

        Dim url As String = ""

        Select Case CInt(codPagRitorno)
            Case enum_PagineAgenda_2010.Pagina_RicercaDocContabili
                url = VirtualPathUtility.ToAbsolute("~/GestioneContabilita/RicercaDocContabili.aspx" &
                                    "?p=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder) &
                                    "&type=" & ricercaType &
                                    "&doc=" & ricercaDoc)

            Case enum_PagineAgenda_2010.Menu

                If flag_MenuBS_2017 Then
                    url = VirtualPathUtility.ToAbsolute("~/Menu/MenuBS_2017.aspx")
                Else
                    url = VirtualPathUtility.ToAbsolute("~/Menu/Menu.aspx")
                End If

            Case enum_PagineAgenda_2010.Menu_BS
                url = VirtualPathUtility.ToAbsolute("~/Menu/MenuBS_Agenda_Nuovo.aspx")

            Case enum_PagineAgenda_2010.Pagina_GestioneMagazziniBS
                url = VirtualPathUtility.ToAbsolute("~/GestioneMagazzini/GestioneMagazziniBS.aspx")

            Case Else

                If objParametriAgenda.SitoOrigine = 300 Then

                    MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                  objParametriAgenda.SitoOrigine,
                                                                  objParametriAgenda.PaginaSitoOrigine,
                                                                  url,
                                                                  objParametriServer)
                End If

        End Select

        Return url

    End Function

#Region "script services Carica Giacenze"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Giacenze(ByVal w_piva As String,
                                           ByVal w_Elem_Cod As Integer,
                                           ByVal w_Udm As Integer,
                                           ByVal w_Udm_Desc As String,
                                           ByVal w_ChkLottoImpianto As Boolean,
                                           ByVal w_Lotto As String,
                                           ByVal w_Prodotto_Cod As Integer,
                                           ByVal w_CauMov As String,
                                           ByVal w_Lotto_Accettazione As String,
                                           ByVal w_Cal_Cod As Integer,
                                           ByVal w_Ubic_Provenienza As String,
                                           ByVal w_Ubic_Destinazione As String,
                                           ByVal w_Chk_ParametroQualitativo As Boolean,
                                           ByVal Operazione As Integer,
                                           ByVal DataMovimento As String,
                                           ByVal Qta_Mask As Decimal,
                                           ByVal KgLordi_Mask As Decimal,
                                           ByVal KgNetti_Mask As Decimal,
                                           ByVal Imballaggi_Mask As Integer,
                                           ByVal Contenitori_Mask As Integer,
                                           ByVal Confezioni_Mask As Integer,
                                           ByVal modulo_anagrafe_log As Integer,
                                           ByVal Flag_QtaNoZero As Boolean,
                                           ByVal Flag_QtaMaggioreZero As Boolean,
                                           ByVal w_key_idMovDet As Integer,
                                           ByVal FF_gest_materiale_vivaistico As Boolean
                                           ) As RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            Return New RispostaStandard With {.Sessione = False}
        End If

        Return DocContabileDettagliUC.Carica_Giacenze(w_piva, w_Elem_Cod, w_Udm, w_Udm_Desc, w_ChkLottoImpianto, w_Lotto,
                                                      w_Prodotto_Cod, w_CauMov, w_Lotto_Accettazione, w_Cal_Cod,
                                                      w_Ubic_Provenienza, w_Ubic_Destinazione, w_Chk_ParametroQualitativo,
                                                      Operazione,
                                                      DataMovimento,
                                                      Qta_Mask,
                                                      KgLordi_Mask, KgNetti_Mask, Imballaggi_Mask, Contenitori_Mask, Confezioni_Mask,
                                                      modulo_anagrafe_log,
                                                      objParametriServer,
                                                      objParametriUtenti,
                                                      Flag_QtaNoZero, Flag_QtaMaggioreZero, w_key_idMovDet, FF_gest_materiale_vivaistico)

    End Function
#End Region

#Region "script services Carica Dati"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaDati(ByVal lav_cod As Integer, ByVal cIdTipoOp As Integer) As RispostaStandard

        Return DocContabileDettagliUC.CaricaDati(lav_cod, cIdTipoOp)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RecuperaValiditaSportello(ByVal Piva As String,
                                                     ByVal ServizioCod As Integer,
                                                     ByVal objP_server As String,
                                                     ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        ElseIf objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim dataMin As Date = AGRODATAINIZIO
            Dim dataMax As Date = AGRODATAFINE
            Dim SportelloAperto As Boolean = True
            Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R
            objPratiche.Data_Sportello_Da_Servizio(Piva,
                                                    enum_Servizi.Quaderno_Campagna_Caa,',ServizioCod
                                                    DateTime.Now,
                                                    SportelloAperto,
                                                    dataMin,
                                                    dataMax,
                                                    objParametriServer,
                                                    objParametriUtenti)

            objPratiche.Sportello_ChiamataSecondaria_SeNessunCambiamento(Piva,
                                                                        enum_Servizi.QuadernoCampagnaBio,
                                                                        DateTime.Now,
                                                                        True,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        SportelloAperto,
                                                                        dataMin,
                                                                        dataMax,
                                                                        objParametriServer,
                                                                        objParametriUtenti)

            r.RispostaStringa = JsonConvert.SerializeObject(New Pair(dataMin, dataMax), Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "script services Carica Causali Riga"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Causale_Riga(ByVal lav_cod As Integer, ByVal qs_tipo As String, ByVal chkAccompagnatoria As Integer) As RispostaStandard

        Return DocContabileDettagliUC.Carica_Causale_Riga(lav_cod, qs_tipo, chkAccompagnatoria)

    End Function

#End Region
#Region "script services ricerca Pua Regolamento"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_PUA_Regolamenti(ByVal Regolamento_Cod As Integer,
                                                 ByVal DataMovimento As Date,
                                                 ByVal Elem_Cod As Integer
                                                 ) As RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            Return New RispostaStandard With {.Sessione = False}
        End If

        Return DocContabileDettagliUC.Leggi_PUA_Regolamenti(Regolamento_Cod,
                                                            DataMovimento,
                                                            Elem_Cod,
                                                            objParametriServer,
                                                            objParametriUtenti)

    End Function
#End Region

#Region "script services ricerca UdM per Fertilizzante"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Recupera_UdmFertilizzante(ByVal Fer_Cod As Integer, ByVal PUA_RegolamentoCod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        '(27/09/2018 fede)
        Dim objAgroWebConfig As AgroWebConfig
        If Not HttpContext.Current.Session Is Nothing Then
            objAgroWebConfig = New AgroWebConfig
            'Else
            '    objAgroWebConfig = New AgroWebConfig(objParametri_Super_Server, objParametri_server, True)
        End If

        Dim msgErrorWS As String = ""
        Dim udmCodDefault As Integer = DocContabileDettagliUC.Recupera_UdmFertilizzante(Fer_Cod,
                                                                                        PUA_RegolamentoCod,
                                                                                        objParametriServer, objParametriUtenti,
                                                                                        objAgroWebConfig,
                                                                                        msgErrorWS)

        If udmCodDefault <> 0 Then
            r.RispostaStringa = udmCodDefault
            r.RispostaOK = True
        Else
            r.Errore = msgErrorWS
            r.RispostaOK = False
        End If

        Return r

    End Function

#End Region

#Region "script services Carica Griglia Imballaggi"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Ricerca_ImballiFormProdottoUC(ByVal piva As String,
                                                         ByVal idAgenda As Integer,
                                                         ByVal lavCod As Integer,
                                                         ByVal idMovDet As Integer
                                                         ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ
            r.RispostaStringa = objMagazzinoBIZ.Leggi_Imballaggi_Riga_Documento(piva, idAgenda, lavCod, idMovDet,
                                                                                False, Nothing, objParametriServer, objParametriUtenti)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "script services Carica UnM per Regolamento"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Udm_Optimize_Regolamento(ByVal piva As String,
                                                    ByVal DataMovimento As Date,
                                                    ByVal xSa_Cod As Integer,
                                                    ByVal xFabbricato_Cod As Integer,
                                                    ByVal Cau_Mov As String,
                                                    ByVal Elem_Cod As Integer,
                                                    ByVal Flag_ProCod_Negativo As Boolean,
                                                    ByVal Prodotto_Cod As Integer,
                                                    ByVal Mat_Cod As Integer,
                                                    ByVal Flag_Prima_Riga As Boolean,
                                                    ByVal Testo_PrimaRiga As String,
                                                    ByVal Cod_Prima_Riga As String,
                                                    ByVal Testo_Da_Ricercare As String,
                                                    ByVal RegolamentoCod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                        ByVal isFreshAndFood As Boolean,
                                        ByVal Flag_QtaNoZero As Boolean,
                                        ByVal Flag_QtaMaggioreZero As Boolean
                                                    ) As RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            Return New RispostaStandard With {.Sessione = False}
        End If

        Return DocContabileDettagliUC.Udm_Optimize_Regolamento(piva,
                                                               DataMovimento,
                                                               xSa_Cod,
                                                               xFabbricato_Cod,
                                                               Cau_Mov,
                                                               Elem_Cod,
                                                               Flag_ProCod_Negativo,
                                                               Prodotto_Cod,
                                                               Mat_Cod,
                                                               Flag_Prima_Riga,
                                                               Testo_PrimaRiga,
                                                               Cod_Prima_Riga,
                                                               Testo_Da_Ricercare,
                                                               RegolamentoCod,
                                                               xFiltroAggiuntivo,
                                                               xOrderBy,
                                                   isFreshAndFood,
                                                    Flag_QtaNoZero,
                                                    Flag_QtaMaggioreZero,
                                                               objParametriServer, objParametriUtenti)

    End Function

#End Region

#Region "script services Ottieni Numeratori e Defaults"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Numeratori_E_Defaults(ByVal piva As String,
                                                 ByVal data_Documento As Date,
                                                 ByVal sa_Cod As Integer,
                                                 ByVal lav_Cod As Integer,
                                                 ByVal fattura_Accompagnatoria As Boolean
                                                 ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objDefault As New Documento_Default_BIZ
            Dim numEDefaults As NumeratoriDefaults = objDefault.NumeratoriEDefaults(data_Documento, piva, sa_Cod, lav_Cod, fattura_Accompagnatoria, objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(numEDefaults, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


#End Region

#Region "script services Carica UnM in Giacenza"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Udm_Optimize(ByVal piva As String,
                                        ByVal xSa_Cod As Integer,
                                        ByVal xFabbricato_Cod As Integer,
                                        ByVal Cau_Mov As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Flag_ProCod_Negativo As Boolean,
                                        ByVal Pro_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Flag_Prima_Riga As Boolean,
                                        ByVal Testo_PrimaRiga As String,
                                        ByVal Cod_Prima_Riga As String,
                                        ByVal Testo_Da_Ricercare As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByVal isFreshAndFood As Boolean,
                                        ByVal Flag_QtaNoZero As Boolean,
                                        ByVal Flag_QtaMaggioreZero As Boolean
                                        ) As RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            Return New RispostaStandard With {.Sessione = False}
        End If

        Return DocContabileDettagliUC.Udm_Optimize(piva,
                                                   xSa_Cod,
                                                   xFabbricato_Cod,
                                                   Cau_Mov,
                                                   Elem_Cod,
                                                   Flag_ProCod_Negativo,
                                                   Pro_Cod,
                                                   Mat_Cod,
                                                   Flag_Prima_Riga,
                                                   Testo_PrimaRiga,
                                                   Cod_Prima_Riga,
                                                   Testo_Da_Ricercare,
                                                   xFiltroAggiuntivo,
                                                   xOrderBy,
                                                   isFreshAndFood,
                                                    Flag_QtaNoZero,
                                                    Flag_QtaMaggioreZero,
                                                   objParametriServer, objParametriUtenti)

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Udm_Optimize_Lotto(ByVal piva As String,
                                              ByVal xSa_Cod As Integer,
                                              ByVal xFabbricato_Cod As Integer,
                                              ByVal Cau_Mov As String,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Flag_ProCod_Negativo As Boolean,
                                              ByVal Pro_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Lotto As String,
                                              ByVal Flag_Prima_Riga As Boolean,
                                              ByVal Testo_PrimaRiga As String,
                                              ByVal Cod_Prima_Riga As String,
                                              ByVal Testo_Da_Ricercare As String,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                        ByVal isFreshAndFood As Boolean,
                                        ByVal Flag_QtaNoZero As Boolean,
                                        ByVal Flag_QtaMaggioreZero As Boolean
                                              ) As RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            Return New RispostaStandard With {.Sessione = False}
        End If

        Return DocContabileDettagliUC.Udm_Optimize_Lotto(piva,
                                                         xSa_Cod,
                                                         xFabbricato_Cod,
                                                         Cau_Mov,
                                                         Elem_Cod,
                                                         Flag_ProCod_Negativo,
                                                         Pro_Cod,
                                                         Mat_Cod,
                                                         Lotto,
                                                         Flag_Prima_Riga,
                                                         Testo_PrimaRiga,
                                                         Cod_Prima_Riga,
                                                         Testo_Da_Ricercare,
                                                         xFiltroAggiuntivo,
                                                         xOrderBy,
                                                   isFreshAndFood,
                                                    Flag_QtaNoZero,
                                                    Flag_QtaMaggioreZero,
                                                         objParametriServer, objParametriUtenti)

    End Function

#End Region

#Region "Ricerca Udm Farmaci"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Recupera_UdmsFarmaco(ByVal Farm_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objP_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objP_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objP_Server) OrElse IsNothing(objP_Utenti) Then
            r.Sessione = False
            Return r
        End If

        Const UDM_GRAMMI As Integer = 3
        Const UDM_MILLILITRI As Integer = 101

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim jsonArr As New JArray
            Dim farmxUdm_R As New AgronicaCoreMetaSchemaDAL.FarmacixUnitaMisura
            Dim dtfarmxUdm As DataTable = farmxUdm_R.Leggi(Farm_Cod, -1, objP_Server, enumSelezioneVariabile.Selezione_JoinDescrizioni)

            Dim hasDefault As Boolean = False
            Dim hasGrammi As Boolean = False
            Dim hasMillilitri As Boolean = False
            If Not IsNothing(dtfarmxUdm) AndAlso dtfarmxUdm.Rows.Count > 0 Then
                dtfarmxUdm.ToExpandoObject.
                    ForEach(Sub(row)
                                Dim jsonObj = New JObject
                                Dim udmCod As Integer = row("Udm_Cod")
                                Dim udmDes As String = row("UDM_DES")
                                Dim isDefault As Integer = row("Default")

                                If isDefault.Equals(1) Then hasDefault = True
                                If udmCod.Equals(UDM_GRAMMI) Then hasGrammi = True
                                If udmCod.Equals(UDM_MILLILITRI) Then hasMillilitri = True

                                jsonObj.Add("isDefault", isDefault)
                                jsonObj.Add("Udm_Cod", udmCod)
                                jsonObj.Add("Udm_Des", udmDes)
                                jsonArr.Add(jsonObj)
                            End Sub)

                ' Aggiunge i ml se non presenti
                If Not hasMillilitri Then
                    Dim jsonObj = New JObject
                    If hasDefault Then
                        jsonObj.Add("isDefault", 0)
                    Else
                        hasDefault = True
                        jsonObj.Add("isDefault", 1)
                    End If
                    jsonObj.Add("Udm_Cod", UDM_MILLILITRI)
                    jsonObj.Add("Udm_Des", AgronicaAgenda_2010.Millilitri)
                    jsonArr.Add(jsonObj)
                End If

                ' Aggiunge i g se non presenti
                If Not hasGrammi Then
                    Dim jsonObj = New JObject
                    jsonObj.Add("isDefault", If(hasDefault, 0, 1))
                    jsonObj.Add("Udm_Cod", UDM_GRAMMI)
                    jsonObj.Add("Udm_Des", AgronicaAgenda_2010.Grammi)
                    jsonArr.Add(jsonObj)
                End If
            End If

            r.RispostaStringa = jsonArr.ToString
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True)
        End Try

        Return r

    End Function

#End Region

    '####################################################################################################################

#Region "script services VerificaPErmessoClasseToxPatentino"

    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaClasseToxPatentinoERicercaUdm(ByVal piva As String,
                                                                 ByVal Data_Validita As Date,
                                                                 ByVal Prodotto_Cod As Integer,
                                                                 ByVal Udm_Dose As Integer,
                                                                 ByVal linkWsFitofarmaci As String
                                                                 ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim messStop As String = ""
        Dim messWarning As String = ""
        Dim newUdm_Cod As Integer = 0
        Dim msg As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim allOk As Boolean = DocContabileDettagliUC.VerificaPermessoClasseToxPatentino(Data_Validita,
                                                                                         Prodotto_Cod,
                                                                                         piva,
                                                                                         objParametriServer,
                                                                                         objParametriUtenti,
                                                                                         messWarning,
                                                                                         msg,
                                                                                         linkWsFitofarmaci,
                                                                                         HttpContext.Current.Session)

        If Not allOk Then
            messStop = String.Format(AgronicaAgenda_2010.Cambio_Prodotto_RichiestoContattoConPatentinoValido, msg)
        Else

            'Cerco Udm_Cod di default

            If Udm_Dose = 0 Then
                Dim strErr As String = ""
                Dim Udm_Cod As Integer = 0
                Dim Udm_Des As String = ""
                Dim Udm_Sim As String = ""

                Dim objDPILeggi As New AgronicaCoreDpiBIZ.Fitofarmaci_Leggi
                objDPILeggi.Recupera_UdM_da_FrCod(strErr,
                                                  Prodotto_Cod,
                                                  Udm_Cod,
                                                  Udm_Sim,
                                                  Udm_Des,
                                                  HttpContext.Current.Session)

                If strErr = "" Then
                    If Udm_Des <> "" Then
                        newUdm_Cod = Udm_Cod
                    End If
                End If

            Else
                newUdm_Cod = Udm_Dose
            End If
        End If

        Dim jObj As New JObject(New JProperty("newUdm_Cod", CInt(newUdm_Cod)),
                                New JProperty("messWarning", messWarning),
                                New JProperty("messStop", messStop)
                                )

        r.RispostaStringa = JsonConvert.SerializeObject(jObj, Formatting.None)
        r.RispostaOK = True

        Return r

    End Function
#End Region


#Region "script services FineScorta_e_altreInfo"

    <WebMethod(EnableSession:=True)>
    Public Shared Function FineScorta_e_altreInfo(ByVal Prodotto_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim infoText As String = ""

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        infoText = DocContabileDettagliUC.FineScorta_e_altreInfo(Prodotto_Cod,
                                                                 objParametriServer,
                                                                 objParametriUtenti)

        If infoText = "<span ></span>" Then
            infoText = ""
        End If
        r.RispostaStringa = infoText
        r.RispostaOK = True

        Return r

    End Function

#End Region

#Region "script services Costruisci Link InfProdotto"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CostruisciLinkInfoProdotto(ByVal p As String, ByVal e As Integer, ByVal l As Integer,
                                                                                ByVal c As String, ByVal d As String, ByVal a As Integer,
                                                                                ByVal orig As String, ByVal mode As String) As RispostaStandard

        Return DocContabileDettagliUC.CostruisciLinkInfoProdotto(p, e, l, c, d, a, orig, mode)

    End Function

#End Region

#Region "script services Costruisci Link Edit Prodotto"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CostruisciLinkNuovoProdotto(ByVal elem_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        r.RispostaStringa = MenuBS_Anagrafica.NuovoProdotto(elem_cod)
        r.RispostaOK = True
        Return r

    End Function

#End Region

#Region "script services Costruisci Info Fertilizzante"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CostruisciInfoFertilizzante(ByVal fer_cod As Integer) As RispostaStandard

        Return DocContabileDettagliUC.CostruisciInfoFertilizzante(fer_cod)

    End Function

#End Region

#Region "script services Carica Griglia Movimenti"

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaGiacenze(ByVal piva As String, ByVal _prodotto As Integer,
            ByVal _specie As Integer, ByVal _varieta As String, ByVal _sa_cod As Integer,
            ByVal _dataRif As String, ByVal _lotto As String,
            ByVal _calibro As Integer, ByVal _qualita As Integer, ByVal _certificazione As Integer, ByVal _rugginosita As Integer,
            ByVal _imballaggio As Integer, ByVal _contenitore As Integer, ByVal _confezione As Integer, ByVal _chkGiacenzePositive As Boolean, _mostraCampiInput As Boolean) As RispostaStandard

        Return Giacenze_MagazzinoUC.CaricaGrigliaGiacenze(piva, _prodotto,
             _specie, _varieta, _sa_cod, _dataRif, _lotto, _calibro, _qualita, _certificazione, _rugginosita,
               _imballaggio, _contenitore, _confezione, _chkGiacenzePositive, _mostraCampiInput)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaRigheDoc(ByVal piva As String,
                                           ByVal id_agenda As Integer,
                                           ByVal lav_cod As Integer
                                           ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            Dim statoOrdineDoc As New Contabilita_Evasione_Ordine

            Dim impreseImpostazioniR As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
            Dim defaultCategorie As String = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser(piva, Nothing,
                                                                                      enum_Impostazioni_Utenti.Default_GruppoMerce_CategoriaProdotto,
                                                                                      "",
                                                                                      objParametriUtenti,
                                                                                      objParametriServer)
            Dim gestioneGruppiMerce = Not String.IsNullOrEmpty(defaultCategorie)

            'Leggo le righe
            Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ
            r.RispostaStringa = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(piva, id_agenda, "", 0, lav_cod,
                                                                               False, Nothing, objParametriServer, objParametriUtenti,
                                                                               "ordine_det <> 1000", "",
                                                                               statoOrdineDoc:=statoOrdineDoc.Stato_Cod, gruppiMerce:=gestioneGruppiMerce, contestoDocContabile:=True)

            statoOrdineDoc.Stato_Des = GetStatoOrdine_Des(statoOrdineDoc.Stato_Cod)
            r.ParametroDue_stringa = JsonConvert.SerializeObject(statoOrdineDoc)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiParametriIndiciSalvati(ByVal piva As String,
                                                       ByVal id_agenda As Integer,
                                                       ByVal id_mov_det As Integer
                                                       ) As RispostaStandard

        Dim DTParametri As New DataTable
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim ObjGHG_Registrazioni As New AgronicaCoreContabDAL.GHG_Registrazioni_R
            DTParametri = ObjGHG_Registrazioni.Leggi_From_Id_Mov_Det(piva, id_agenda, id_mov_det, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)


            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(DTParametri, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r


    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaImputazioneImpianti(piva As String, id_agenda As Integer, id_mov_det As Integer,
                                                            piva_rif As String, elem_cod As Integer, mat_cod As Integer,
                                                            filtro_impianti As Integer, data_movimento As String,
                                                            chksmart As Integer) As RispostaStandard

        Dim Dt As DataTable = Nothing
        Dim Dt_Agenda As DataTable
        Dim Dt_Destinazioni As DataTable = Nothing
        Dim Dt_Particelle As DataTable
        Dim Dr_Search() As DataRow
        Dim Dt_Raccolta As DataTable
        Dim Dt_Trapianto As DataTable
        Dim Dt_Movimento As DataTable
        Dim Dt_Materie_Prime As DataTable

        Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
        Dim objMov_Destinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_R
        Dim objMovimenti As New AgronicaCoreContabDAL.Movimenti_R
        Dim objMaterie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        Dim strAgenda_Rif As String = "" 'Elenco raccolte collegato (possono essere + di 1 una poiché suddivise per centro aziendale)
        Dim Numero_Piante As Integer
        Dim Deno As Single
        Dim strFiltro As String = ""
        Dim Note_Raccolta As String = ""
        Dim bok As Boolean = False

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            If Not IsDate(data_movimento) Then
                data_movimento = CDate(Now)
            End If


            If id_mov_det <> 0 Then

                chksmart = 0 'Nessun filtro intelligente per la migliore impostazione possibile

                'Lettura dell'agenda di riferimento
                Dim ObjRiferimenti As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                Dt_Agenda = ObjRiferimenti.Leggi_Specifica(piva, 0, id_agenda, 0, id_mov_det, 0, "", piva_rif, 0, 0, 0, 0, 125, "", "", "", objParametriServer)

                If Dt_Agenda.Rows.Count > 0 Then

                    piva_rif = Dt_Agenda(0).Item("Piva_Rif")

                    'Lettura dei Dati Raccolta
                    If Dt_Agenda(0).Item("Id_Agenda_Rif") <> 0 Then
                        Dt_Movimento = objMovimenti.Leggi(piva_rif, 0, Dt_Agenda(0).Item("Id_Agenda_Rif"), 0, 0, CostantiPersonalizzate.CAU_RILIEVO_RACCOLTA, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)

                        If Dt_Movimento.Rows.Count > 0 Then

                            Note_Raccolta = Dt_Movimento(0).Item("Mov_Desc")

                        End If
                    End If


                    For Each dr As DataRow In Dt_Agenda.Rows

                        strAgenda_Rif = strAgenda_Rif & If(Trim(strAgenda_Rif) = "", "", ",") & dr.Item("Id_Agenda_Rif")

                    Next


                End If

            End If


            'Lettura Particelle Catastali associate alla piva_rif
            Dt_Particelle = objParticelle.AppezzamentixParticelle_Leggi(piva_rif, 0, 0, "", "", "0", 0, 0, "0", enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametriServer, False)

            'Lettura delle Raccolte associate all'impresa
            If Trim(strAgenda_Rif) <> "" Then
                strFiltro = "Agenda.Id_Agenda Not In ( " & strAgenda_Rif & ")"
            End If

            Dt_Raccolta = objMov_Destinazioni.Leggi_Raccolte(piva_rif, 0, 0, 0, 0, 0, 0, 0, AGRODATAINIZIO, data_movimento, enumSelezioneVariabile.Selezione_JoinCompleta, strFiltro, "Data_Movimento Desc", objParametriServer)

            'Lettura delle Raccolte associate all'impresa
            Dt_Trapianto = objMov_Destinazioni.Leggi_Semine_Trapianti(piva_rif, 0, 0, 0, 0, 0, 0, 0, AGRODATAINIZIO, data_movimento, enumSelezioneVariabile.Selezione_JoinCompleta, "", "Data_Movimento Desc", objParametriServer)

            'Lettura Prodotto x Vedere se è referenza
            Dt_Materie_Prime = objMaterie_Prime.Leggi2(piva, 0, mat_cod, "", 0, "", True, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)

            If Dt_Materie_Prime.Rows.Count > 0 Then

                If Not IsDBNull(Dt_Materie_Prime(0).Item("Mat_Cod_Referenza")) AndAlso Dt_Materie_Prime(0).Item("Mat_Cod_Referenza") <> 0 Then

                    'Impostazione della referenza sul log omni
                    mat_cod = Dt_Materie_Prime(0).Item("Mat_Cod_Referenza")

                End If

            End If


            Select Case chksmart

                Case 0

                    'Lettura degli impianti validi per mat_cod
                    Dt = objReg_Impianti.LeggixImputazione(objParametriServer.PivaSuperUser, piva_rif, strAgenda_Rif, data_movimento, elem_cod, mat_cod, filtro_impianti, "", "", objParametriServer)

                Case 1

                    'Lettura degli impianti validi per mat_cod SMART 

                    filtro_impianti = 0
                    Do While filtro_impianti < 3 And Not bok

                        Dt = objReg_Impianti.LeggixImputazione(objParametriServer.PivaSuperUser, piva_rif, 0, data_movimento, elem_cod, mat_cod, filtro_impianti, "", "", objParametriServer)
                        bok = CBool(Dt.Rows.Count)

                        If Not bok Then
                            filtro_impianti += 1
                        End If

                    Loop


            End Select



            If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

                Dt.Columns.Add(New DataColumn("Key", GetType(String)))
                Dt.Columns.Add(New DataColumn("Id_Mov_Det", GetType(Integer)))

                Dt.Columns.Add(New DataColumn("Catasto", GetType(String)))
                Dt.Columns.Add(New DataColumn("Numero_Raccolte", GetType(Integer)))
                Dt.Columns.Add(New DataColumn("Data_Ultima_Raccolta", GetType(String)))
                Dt.Columns.Add(New DataColumn("Data_Trapianto", GetType(String)))
                Dt.Columns.Add(New DataColumn("Piante", GetType(Integer)))
                Dt.Columns.Add(New DataColumn("Note_Raccolta", GetType(String)))
                Dt.Columns.Add(New DataColumn("Impianti_Indefiniti", GetType(Integer)))
                Dt.Columns.Add(New DataColumn("Filtro_Impianti", GetType(Integer)))


                For Each dr As DataRow In Dt.Rows

                    dr.Item("Key") = id_mov_det & "|" & dr.Item("Sa_Cod") & "|" & dr.Item("Appezza") & "|" & dr.Item("Id_Reg") & "|" & dr.Item("Progetto_Cod")
                    dr.Item("Id_Mov_Det") = 0
                    dr.Item("Catasto") = ""
                    dr.Item("Numero_Raccolte") = 0
                    dr.Item("Data_Ultima_Raccolta") = ""
                    dr.Item("Data_Trapianto") = ""
                    dr.Item("Piante") = 0
                    dr.Item("Note_Raccolta") = Note_Raccolta
                    dr.Item("Filtro_Impianti") = filtro_impianti

                    'Al momento se trovo degli impianti li mostro
                    If chksmart = 1 And Not bok Then
                        dr.Item("Impianti_Indefiniti") = 1
                    Else
                        dr.Item("Impianti_Indefiniti") = 0
                    End If


                    'Impostazione Catasto
                    If Dt_Particelle.Rows.Count > 0 Then

                        Dr_Search = Dt_Particelle.Select("Sa_Cod = " & dr.Item("Sa_Cod") & " And Appezza = " & dr.Item("Appezza"))

                        For Each Dr_Particelle As DataRow In Dr_Search

                            dr.Item("Catasto") = dr.Item("Catasto") & IIf(dr.Item("Catasto") = "", "", ", ") & Dr_Particelle.Item("Localita") & " F:" & Dr_Particelle.Item("Foglio") & " N:" & Dr_Particelle.Item("Numero") & " S:" & Dr_Particelle.Item("Sezione") & IIf(Dr_Particelle.Item("Subalterno") = "0", "", " Sub: " & Dr_Particelle.Item("Subalterno"))

                        Next

                    End If

                    'Impostazione Raccolte
                    If Dt_Raccolta.Rows.Count > 0 Then

                        Dr_Search = Dt_Raccolta.Select("Sa_Cod = " & dr.Item("Sa_Cod") & " And Appezza = " & dr.Item("Appezza") & " And Id_Destinazione = " & dr.Item("Id_Reg"))

                        If Dr_Search.Count > 0 Then

                            dr.Item("Numero_Raccolte") = Dr_Search.Count
                            dr.Item("Data_Ultima_Raccolta") = Format(Dr_Search(0).Item("Data_Movimento"), "dd/MM/yyyy")

                        End If

                    End If

                    'Impostazione Data Semina Trapianto
                    If Dt_Trapianto.Rows.Count > 0 Then

                        Dr_Search = Dt_Trapianto.Select("Sa_Cod = " & dr.Item("Sa_Cod") & " And Appezza = " & dr.Item("Appezza") & " And Id_Destinazione = " & dr.Item("Id_Reg"))

                        If Dr_Search.Count > 0 Then

                            dr.Item("Data_Trapianto") = Format(Dr_Search(0).Item("Data_Movimento"), "dd/MM/yyyy")

                        End If

                    End If

                    'Impostazione Pianto/Impianto
                    If IsNumeric(dr.Item("Tra_Fila")) And IsNumeric(dr.Item("Su_Fila")) Then
                        'Inserimento ok
                        Deno = (CSng(Format(dr.Item("Tra_Fila"), "0.0")) * CSng(Format(dr.Item("Su_Fila"), "0.0")))
                        'La formattazione sopra serve per evitare l'overflow
                        If Deno = CSng(0) Then
                            'Evito la Division By zero
                            Numero_Piante = 0

                        Else
                            'Piante per Ettaro
                            Numero_Piante = If(CDbl(10000 / Deno) < 1, 0, Format(CDbl(10000 / Deno), "0.0")) * dr.Item("Sup_Imp")
                        End If

                        dr.Item("Piante") = Numero_Piante

                    End If


                Next


            End If


            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiImprese_Impostazioni(piva As String, ByVal sa_cod As Integer, ByVal impostazione_cod As Integer, ByVal valore_iniziale As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objLeggi As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
            Dim imputazioneValore As String = objLeggi.LeggiScalareMulticentroAziendaSuperUser(piva, New List(Of Integer)({sa_cod}),
                                                                                   impostazione_cod,
                                                                                   "",
                                                                                   objParametriUtenti,
                                                                                   objParametriServer)

            If Not String.IsNullOrEmpty(imputazioneValore) Then
                r.RispostaStringa = imputazioneValore
            Else
                r.RispostaStringa = valore_iniziale
            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Beni_Confezionamento(ByVal piva As String, ByVal tipo As Integer, ByVal modulo_generazione As Integer) As RispostaStandard

        Dim Dt As DataTable
        Dim filtro_mp As String = ""
        Dim objMaterie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Select Case modulo_generazione

                Case 2 'Gias FF

                    Select Case tipo
                        Case 0
                            filtro_mp = "" 'Tutte
                        Case 1
                            filtro_mp = " Tabella_Cod = 5 " 'Confezioni
                        Case 2
                            filtro_mp = " Tabella_Cod = 8 " 'Contenitori
                        Case 3
                            filtro_mp = " Tabella_Cod = 4 " 'Imballaggi

                    End Select

                Case Else

                    Select Case tipo
                        Case 0
                            filtro_mp = " And Udm_Cod_Extra <> 0 "
                        Case 1
                            filtro_mp = " And Udm_Cod_Extra <> 0 And ChkContenitore = 0 And ChkImballaggio = 0 "
                        Case 2
                            filtro_mp = " And Udm_Cod_Extra <> 0 And ChkContenitore = 1 And ChkImballaggio = 0 "
                        Case 3
                            filtro_mp = " And Udm_Cod_Extra <> 0 And ChkContenitore = 0 And ChkImballaggio = 1 "

                    End Select

            End Select


            Dt = objMaterie_Prime.Leggi_Beni_Confezionamento_Modulo(piva, modulo_generazione, True, filtro_mp, "Mat_Des", objParametriServer)

            If Dt.Rows.Count > 0 Then


                Dt.Columns.Add(New DataColumn("Tipo_BC", GetType(Integer)))
                Dt.Columns.Add(New DataColumn("Tipo_Des_BC", GetType(String)))

                Dt.Columns.Add(New DataColumn("Mat_Cod_Scarico", GetType(Integer)))
                Dt.Columns.Add(New DataColumn("Mat_Des_Scarico", GetType(String)))

                For Each dr As DataRow In Dt.Rows


                    dr.Item("Mat_Cod_Scarico") = dr.Item("Mat_Cod")
                    dr.Item("Mat_Des_Scarico") = dr.Item("Mat_Des")
                    If Not String.IsNullOrWhiteSpace(dr.Item("Cod_Articolo")) Then
                        dr.Item("Mat_Des_Scarico") &= " (" & dr.Item("Cod_Articolo") & ")"
                    End If

                    Select Case modulo_generazione

                        Case 2

                            Select Case dr.Item("Tabella_Cod")

                                Case 5

                                    dr.Item("Tipo_BC") = 1
                                    dr.Item("Tipo_Des_BC") = "Confezione"

                                Case 8

                                    dr.Item("Tipo_BC") = 2
                                    dr.Item("Tipo_Des_BC") = "Contenitore"

                                Case 4

                                    dr.Item("Tipo_BC") = 3
                                    dr.Item("Tipo_Des_BC") = "Imballaggio"

                                Case Else

                                    dr.Item("Tipo_BC") = 0
                                    dr.Item("Tipo_Des_BC") = ""

                            End Select

                        Case Else

                            dr.Item("Tipo_BC") = 1
                            dr.Item("Tipo_Des_BC") = "Confezione"

                            If dr.Item("ChkImballaggio") = 1 Then
                                dr.Item("Tipo_BC") = 3
                                dr.Item("Tipo_Des_BC") = "Imballaggio"
                            ElseIf dr.Item("ChkContenitore") = 1 Then
                                dr.Item("Tipo_BC") = 2
                                dr.Item("Tipo_Des_BC") = "Contenitore"

                            End If


                    End Select


                    'Correzione Mat_Des
                    dr.Item("Mat_Des") = dr.Item("Mat_Des") & " (" & dr.Item("Cod_Articolo") & ")"

                Next

            End If

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Codici_Articolo(ByVal piva As String, ByVal tipo As Integer) As RispostaStandard

        Dim dt As DataTable
        Dim filtro_mp As String = ""
        Dim objMaterie_Prime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Select Case tipo
                Case 0
                    filtro_mp = " And Udm_Cod_Extra <> 0 "
                Case 1
                    filtro_mp = " And Udm_Cod_Extra <> 0 And ChkContenitore = 0 And ChkImballaggio = 0 "
                Case 2
                    filtro_mp = " And Udm_Cod_Extra <> 0 And ChkContenitore = 1 And ChkImballaggio = 0 "
                Case 3
                    filtro_mp = " And Udm_Cod_Extra <> 0 And ChkContenitore = 0 And ChkImballaggio = 1 "

            End Select

            dt = objMaterie_Prime.Leggi(piva, 0, BENI_CONFEZ_VEGETALE, 0, "", 0, 0, 0, 0, 0, 0, 0,
                                        "", 0, "", False, False, filtro_mp,
                                        enumSelezioneVariabile.Selezione_JoinCompleta, "", "Cod_Articolo", objParametriServer)

            If dt.Rows.Count > 0 Then

                dt.Columns.Add(New DataColumn("Tipo_BC", GetType(Integer)))
                dt.Columns.Add(New DataColumn("Tipo_Des_BC", GetType(String)))

                For Each dr As DataRow In dt.Rows

                    dr.Item("Tipo_BC") = 1
                    dr.Item("Tipo_Des_BC") = "Confezione"

                    If dr.Item("ChkImballaggio") = 1 Then
                        dr.Item("Tipo_BC") = 3
                        dr.Item("Tipo_Des_BC") = "Imballaggio"
                    ElseIf dr.Item("ChkContenitore") = 1 Then
                        dr.Item("Tipo_BC") = 2
                        dr.Item("Tipo_Des_BC") = "Contenitore"

                    End If

                Next

            End If

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    '<WebMethod(EnableSession:=True)>
    'Public Shared Function Leggi_Giacenza(ByVal piva As String,
    '                                      ByVal sa_cod As Integer,
    '                                      ByVal fabbricato_cod As Integer,
    '                                      ByVal elem_cod As Integer,
    '                                      ByVal mat_cod As Integer,
    '                                      ByVal udm_cod As Integer,
    '                                      ByVal data_movimento As String,
    '                                      ) As RispostaStandard

    '    Dim r As New RispostaStandard

    '    Dim dt As DataTable
    '    Dim objProdotti As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ
    '    Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
    '    Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
    '    Dim lotto As String
    '    Dim Flag_QtaNoZero = False
    '    Dim Flag_QtaMaggioreZero = False
    '    Dim Giacenza As Integer = 0

    '    Try

    '        'Impostazioni utente per giacenza
    '        Dim Impostazione_UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE As String = Nothing

    '        Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read



    '        Dim Impo_UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE = objImp.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE,
    '                                                                                      objParametriUtenti, 2)
    '        Dim impostazioneGiacenze() As String = Nothing
    '        Dim elem_cod_impostazione() As String = Nothing
    '        If Not String.IsNullOrEmpty(Impo_UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE) Then
    '            impostazioneGiacenze = Impo_UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE.Split("|")
    '        End If

    '        If Not impostazioneGiacenze Is Nothing Then
    '            For Each s As String In impostazioneGiacenze
    '                elem_cod_impostazione = s.Split("_")

    '                If CInt(elem_cod_impostazione(0)) = elem_cod Then
    '                    '0=soloMovimentati; 1=soloPresenti (default); 2=tutti
    '                    If elem_cod_impostazione(1) = enum_Gestione_Giacenze.TuttiProdotti Then
    '                        Flag_QtaNoZero = False
    '                        Flag_QtaMaggioreZero = False
    '                    Else
    '                        If elem_cod_impostazione(1) = enum_Gestione_Giacenze.SoloPresenti Then
    '                            Flag_QtaNoZero = True
    '                            Flag_QtaMaggioreZero = True
    '                        Else
    '                            If elem_cod_impostazione(1) = enum_Gestione_Giacenze.SoloMovimentati Then
    '                                Flag_QtaNoZero = False
    '                                Flag_QtaMaggioreZero = False
    '                            End If
    '                        End If
    '                    End If

    '                    Exit For
    '                End If
    '            Next
    '        End If



    '        lotto = LOTTO_NONDEFINITO

    '        dt = objProdotti.Leggi_Giacenze_Globale(0, False, piva, sa_cod, 20, fabbricato_cod, elem_cod, 0, mat_cod, lotto, 0, udm_cod, data_movimento, False, Flag_QtaNoZero, "Prodotto_Des", objParametriServer, objParametriUtenti, True, Flag_QtaMaggioreZero)

    '        If dt.Rows.Count > 0 Then

    '            For Each dr As DataRow In dt.Rows
    '                Giacenza = Giacenza + dr.Item("Giacenza")
    '            Next

    '        End If

    '        r.RispostaStringa = Giacenza.ToString()

    '        'Dim serializerSettings As New JsonSerializerSettings()
    '        'serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
    '        'r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)

    '        r.RispostaOK = True

    '    Catch ex As Exception
    '        r.RispostaOK = False
    '        r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
    '    End Try

    '    Return r

    'End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Agenda_BC(piva As String, id_agenda As Integer) As DataTable

        Dim dt As DataTable = Nothing
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
                Throw New Exception("Sessione Scaduta")
            End If

            Dim leggi As New AgronicaCoreContabDAL.Agenda_R
            dt = leggi.Leggi_Agenda_BC(piva, id_agenda, objParametriServer)

        Catch ex As Exception
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return dt

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Agenda_Riferimento_BC(piva As String, id_agenda As Integer) As DataTable

        Dim dt As DataTable = Nothing
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
                Throw New Exception("Sessione Scaduta")
            End If

            Dim leggi As New AgronicaCoreContabDAL.Agenda_R
            dt = leggi.Leggi_Agenda_Riferimento_BC(piva, id_agenda, objParametriServer)

        Catch ex As Exception
            Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return dt

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaBeniConfezionamento(ByVal piva As String, ByVal cau_mov As String,
                                                            ByVal id_agenda As Integer, ByVal id_mov As Integer,
                                                            ByVal filtro_aggiuntivo As String, ByVal modulo_generazione As Integer
                                                            ) As RispostaStandard

        Dim dt As DataTable
        Dim r As New RispostaStandard
        Dim ObjProdotti As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggi As New AgronicaCoreContabDAL.Agenda_R

            dt = leggi.LeggiMovimentoBeniConfezionamento(piva, id_agenda, id_mov, cau_mov, filtro_aggiuntivo, objParametriServer)

            If dt.Rows.Count > 0 Then

                dt.Columns.Add(New DataColumn("Tipo_BC", GetType(Integer)))
                dt.Columns.Add(New DataColumn("Tipo_Des_BC", GetType(String)))
                dt.Columns.Add(New DataColumn("Giacenza", GetType(Integer)))

                dt.Columns.Add(New DataColumn("Mat_Cod_Scarico", GetType(Integer)))
                dt.Columns.Add(New DataColumn("Mat_Des_Scarico", GetType(String)))

                For Each dr As DataRow In dt.Rows

                    dr.Item("Mat_Cod_Scarico") = dr.Item("Mat_Cod")
                    dr.Item("Mat_Des_Scarico") = dr.Item("Mat_Des")
                    If Not String.IsNullOrWhiteSpace(dr.Item("Cod_Articolo")) Then
                        dr.Item("Mat_Des_Scarico") &= " (" & dr.Item("Cod_Articolo") & ")"
                    End If

                    Select Case modulo_generazione

                        Case 2

                            Select Case dr.Item("Tabella_Cod")

                                Case 5

                                    dr.Item("Tipo_BC") = 1
                                    dr.Item("Tipo_Des_BC") = "Confezione"

                                Case 8

                                    dr.Item("Tipo_BC") = 2
                                    dr.Item("Tipo_Des_BC") = "Contenitore"

                                Case 4

                                    dr.Item("Tipo_BC") = 3
                                    dr.Item("Tipo_Des_BC") = "Imballaggio"

                                Case Else

                                    dr.Item("Tipo_BC") = 0
                                    dr.Item("Tipo_Des_BC") = ""

                            End Select

                        Case Else

                            dr.Item("Tipo_BC") = 1
                            dr.Item("Tipo_Des_BC") = "Confezione"

                            If dr.Item("ChkImballaggio") = 1 Then
                                dr.Item("Tipo_BC") = 3
                                dr.Item("Tipo_Des_BC") = "Imballaggio"
                            ElseIf dr.Item("ChkContenitore") = 1 Then
                                dr.Item("Tipo_BC") = 2
                                dr.Item("Tipo_Des_BC") = "Contenitore"

                            End If


                    End Select


                    'Lettura Giacenze
                    Dim DtGiacenza = ObjProdotti.Leggi_Giacenze_Globale(1, False, piva, dr.Item("Sa_Cod"), 20, dr.Item("Id_Destinazione"), dr.Item("Elem_Cod"), 0, dr.Item("Mat_Cod"), LOTTO_NONDEFINITO, 0, 0, dr.Item("Data_Movimento"), False, False, "Prodotto_Des", objParametriServer, objParametriUtenti)

                    If DtGiacenza.Rows.Count > 0 Then
                        dr.Item("Giacenza") = DtGiacenza(0)("Giacenza")
                    End If


                Next

            End If

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaRifCatastali(ByVal piva As String,
                                                     ByVal idAgenda As Integer
                                                     ) As RispostaStandard

        Dim dt As DataTable
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggiContrattiImpreseParticelle As New AgronicaCoreContabDAL.ContrattiXImpreseXParticelle_R

            Dim strOrdinamento As New StringBuilder
            strOrdinamento.Length = 0
            strOrdinamento.AppendLine("   ISTAT.LOCALITA")
            strOrdinamento.AppendLine(" , ISTAT.COMUNI_PROV ")
            strOrdinamento.AppendLine(" , ContrattiXImpreseXParticelle.SEZIONE ")
            strOrdinamento.AppendLine(" , ContrattiXImpreseXParticelle.FOGLIO ")
            strOrdinamento.AppendLine(" , ContrattiXImpreseXParticelle.NUMERO ")
            strOrdinamento.AppendLine(" , ContrattiXImpreseXParticelle.SUBALTERNO ")

            dt = leggiContrattiImpreseParticelle.Leggi_conClassamentoCodice(piva,
                                                                            0,
                                                                            idAgenda,
                                                                            0,
                                                                            Nothing,
                                                                            "",
                                                                            strOrdinamento.ToString,
                                                                            objParametriServer,
                                                                            TitoloPossesso:=enum_TitoloPossesso.AffittoContratto)

            If dt.Rows.Count > 0 Then

                CaricaColonneAggiuntiveParticelle(dt)

            End If

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Particelle_Imprese(ByVal piva As String,
                                                    ByVal saCod As Integer,
                                                    ByVal dataInizioValidita As Date,
                                                    ByVal dataFineValidita As Date
                                                    ) As RispostaStandard

        Dim dt As DataTable

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objParticelle As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_R

            Dim strOrdinamento As New StringBuilder
            strOrdinamento.Length = 0
            strOrdinamento.AppendLine("   ISTAT.LOCALITA")
            strOrdinamento.AppendLine(" , ISTAT.COMUNI_PROV ")
            strOrdinamento.AppendLine(" , ParticelleCatastali.SEZIONE ")
            strOrdinamento.AppendLine(" , ParticelleCatastali.FOGLIO ")
            strOrdinamento.AppendLine(" , ParticelleCatastali.NUMERO ")
            strOrdinamento.AppendLine(" , ParticelleCatastali.SUBALTERNO ")

            dt = objParticelle.Leggi_conClassamentoCodice(piva,
                                                          saCod,
                                                          "",
                                                          "",
                                                          "",
                                                          0,
                                                          0,
                                                          "",
                                                          "",
                                                          strOrdinamento.ToString,
                                                          objParametriServer,
                                                          dataInizioValidita:=dataInizioValidita,
                                                          dataFineValidita:=dataFineValidita,
                                                          TitoloPossesso:=enum_TitoloPossesso.AffittoContratto)

            If dt.Rows.Count > 0 Then

                CaricaColonneAggiuntiveParticelle(dt)

            End If

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Shared Sub CaricaColonneAggiuntiveParticelle(ByRef dt As DataTable)

        Dim nomeDescParticella = "Desc_Particella"
        Dim nomeIdenParticella = "Iden_Particella"
        Dim nomeSuperficieCatastale = "Superficie_Catastale"

        dt.Columns.Add(New DataColumn(nomeDescParticella, GetType(String)))
        dt.Columns.Add(New DataColumn(nomeIdenParticella, GetType(String)))
        dt.Columns.Add(New DataColumn(nomeSuperficieCatastale, GetType(Decimal)))

        For Each dr As DataRow In dt.Rows

            dr.Item(nomeDescParticella) = ComponiDescrizioneParticella(dr)
            dr.Item(nomeIdenParticella) = ComponiIdentificativoParticella(dr)

            If Not IsDBNull(dr.Item("ETTARI")) AndAlso
               Not IsDBNull(dr.Item("ARE")) AndAlso
               Not IsDBNull(dr.Item("CENTIARE")) Then

                dr.Item(nomeSuperficieCatastale) = dr.Item("ETTARI") + dr.Item("ARE") / 100 + dr.Item("CENTIARE") / 10000

            Else

                dr.Item(nomeSuperficieCatastale) = 0

            End If

        Next

    End Sub

    Private Shared Function ComponiDescrizioneParticella(dr As DataRow) As String

        Return String.Format("{0} ({1}) Sez.{2} Fgl.{3} Num.{4} S.{5}",
                             dr.Item("LOCALITA"),
                             dr.Item("COMUNI_PROV"),
                             dr.Item("SEZIONE"),
                             dr.Item("FOGLIO"),
                             dr.Item("NUMERO"),
                             dr.Item("SUBALTERNO"))

    End Function

    Private Shared Function ComponiIdentificativoParticella(dr As DataRow) As String

        Return String.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                             dr.Item("PROV"),
                             dr.Item("COM"),
                             dr.Item("SEZIONE"),
                             dr.Item("FOGLIO"),
                             dr.Item("NUMERO"),
                             dr.Item("SUBALTERNO"))

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaContrattiImpreseParticelle(ByVal piva As String,
                                                              ByVal saCod As Integer,
                                                              ByVal idAgenda As Integer,
                                                              ByVal rigaInserita As String,
                                                              ByVal rigaModificata As String,
                                                              ByVal rigaCancellata As String,
                                                              ByVal forzaCancellazioneLegamiAppezzamentiCampi As Integer
                                                              ) As RispostaStandard

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objRigaInserita As JObject = JsonConvert.DeserializeObject(rigaInserita, settingLoc)

        Dim objRigaModificata As JObject = JsonConvert.DeserializeObject(rigaModificata, settingLoc)

        Dim objRigaCancellata As JObject = JsonConvert.DeserializeObject(rigaCancellata, settingLoc)

        Dim objRigaDaAggiornare As New JObject
        Dim tipoOperazioneRiga As ContrattiXImpreseXParticelle.enum_TipoOperazioneRiga

        Select Case True

            Case objRigaInserita.Count > 0

                tipoOperazioneRiga = ContrattiXImpreseXParticelle.enum_TipoOperazioneRiga.Inserimento
                objRigaDaAggiornare = objRigaInserita

            Case objRigaModificata.Count > 0

                tipoOperazioneRiga = ContrattiXImpreseXParticelle.enum_TipoOperazioneRiga.Modifica
                objRigaDaAggiornare = objRigaModificata

            Case objRigaCancellata.Count > 0

                tipoOperazioneRiga = ContrattiXImpreseXParticelle.enum_TipoOperazioneRiga.Cancellazione
                objRigaDaAggiornare = objRigaCancellata

        End Select

        Dim objDatiRigaContrattoAffitto = CreaDatiRigaContrattoAffitto(objRigaDaAggiornare)

        Dim objContrattiImpreseParticelleBiz As New AgronicaCoreContabBIZ.ContrattiXImpreseXParticelle(objParametriServer)

        Try

            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)

            Dim Particella As New AgronicaCoreContabDAL.ContrattiXImpreseXParticelle_Particella With {
                .PROV = objDatiRigaContrattoAffitto.PROV,
                .COM = objDatiRigaContrattoAffitto.COM,
                .SEZIONE = objDatiRigaContrattoAffitto.SEZIONE,
                .FOGLIO = objDatiRigaContrattoAffitto.FOGLIO,
                .NUMERO = objDatiRigaContrattoAffitto.NUMERO,
                .SUBALTERNO = objDatiRigaContrattoAffitto.SUBALTERNO
            }

            Dim inserisciNuovoCodParticella = SeCodiceParticellaDaInserire(tipoOperazioneRiga, objDatiRigaContrattoAffitto)

            Dim IdImprPart As Integer = 0

            If Not inserisciNuovoCodParticella Then

                IdImprPart = DeterminaIdImpreseParticelle(objDatiRigaContrattoAffitto.Cod_Particella_Id)

            End If

            objContrattiImpreseParticelleBiz.AggiornaContrattiImpreseParticelle(tipoOperazioneRiga,
                                                                                piva,
                                                                                saCod,
                                                                                idAgenda,
                                                                                Particella,
                                                                                objDatiRigaContrattoAffitto.Superficie_Catastale,
                                                                                IdImprPart,
                                                                                objDatiRigaContrattoAffitto.Cod_Particella,
                                                                                inserisciNuovoCodParticella,
                                                                                objDatiRigaContrattoAffitto.Superficie,
                                                                                objDatiRigaContrattoAffitto.Validita_Inizio,
                                                                                objDatiRigaContrattoAffitto.Validita_Fine,
                                                                                objDatiRigaContrattoAffitto.BioVincolo,
                                                                                forzaCancellazioneLegamiAppezzamentiCampi)

            Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)

            r.RispostaStringa = ""
            r.RispostaOK = True

        Catch ex As Exception

            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        Finally

            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)

        End Try

        Return r

    End Function

    Private Shared Function CreaDatiRigaContrattoAffitto(ByVal objRigaDaAggiornare As JObject) As DatiRigaContrattoAffitto

        Dim objDatiRigaContratto As New DatiRigaContrattoAffitto

        If Not IsNothing(objRigaDaAggiornare.Property("Id")) Then
            objDatiRigaContratto.Id = CInt(objRigaDaAggiornare("Id"))
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("Cod_Particella")) Then
            objDatiRigaContratto.Cod_Particella = objRigaDaAggiornare("Cod_Particella")
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("Cod_Particella_Id")) Then
            objDatiRigaContratto.Cod_Particella_Id = objRigaDaAggiornare("Cod_Particella_Id")
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("PROV")) Then
            objDatiRigaContratto.PROV = objRigaDaAggiornare("PROV")
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("COM")) Then
            objDatiRigaContratto.COM = objRigaDaAggiornare("COM")
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("SEZIONE")) Then
            objDatiRigaContratto.SEZIONE = objRigaDaAggiornare("SEZIONE")
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("FOGLIO")) Then
            objDatiRigaContratto.FOGLIO = CInt(objRigaDaAggiornare("FOGLIO"))
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("NUMERO")) Then
            objDatiRigaContratto.NUMERO = CInt(objRigaDaAggiornare("NUMERO"))
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("SUBALTERNO")) Then
            objDatiRigaContratto.SUBALTERNO = objRigaDaAggiornare("SUBALTERNO")
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("Superficie")) Then
            objDatiRigaContratto.Superficie = CDec(objRigaDaAggiornare("Superficie"))
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("Validita_Inizio")) Then
            objDatiRigaContratto.Validita_Inizio = CDate(objRigaDaAggiornare("Validita_Inizio"))
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("Validita_Fine")) Then
            objDatiRigaContratto.Validita_Fine = CDate(objRigaDaAggiornare("Validita_Fine"))
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("BioVincolo")) Then
            objDatiRigaContratto.BioVincolo = CInt(objRigaDaAggiornare("BioVincolo"))
        End If

        If Not IsNothing(objRigaDaAggiornare.Property("Superficie_Catastale")) Then
            objDatiRigaContratto.Superficie_Catastale = CDec(objRigaDaAggiornare("Superficie_Catastale"))
        End If

        Return objDatiRigaContratto

    End Function

    Private Shared Function SeCodiceParticellaDaInserire(tipoOperazioneRiga As ContrattiXImpreseXParticelle.enum_TipoOperazioneRiga,
                                                         objDatiRigaContrattoAffitto As DatiRigaContrattoAffitto
                                                         ) As Boolean

        Return tipoOperazioneRiga = ContrattiXImpreseXParticelle.enum_TipoOperazioneRiga.Inserimento AndAlso
               String.IsNullOrEmpty(objDatiRigaContrattoAffitto.Cod_Particella_Id) AndAlso
               Not String.IsNullOrEmpty(objDatiRigaContrattoAffitto.Cod_Particella)

    End Function

    Private Shared Function DeterminaIdImpreseParticelle(ByVal codParticellaId As String) As Integer

        Dim IdImprPart As Integer = 0

        Dim particellaId() = Split(codParticellaId, "|")

        If particellaId.Count = 2 AndAlso IsNumeric(particellaId(1)) Then

            IdImprPart = CInt(particellaId(1))

        End If

        Return IdImprPart

    End Function

    Private Class DatiRigaContrattoAffitto

        Property Id As Integer

        Property Cod_Particella As String

        Property Cod_Particella_Id As String

        Property PROV As String

        Property COM As String

        Property SEZIONE As String

        Property FOGLIO As Integer

        Property NUMERO As Integer

        Property SUBALTERNO As String

        Property Superficie As Decimal

        Property Validita_Inizio As Date

        Property Validita_Fine As Date

        Property BioVincolo As Integer

        Property Superficie_Catastale As Decimal

    End Class

    <WebMethod(EnableSession:=True)>
    Public Shared Function Link_Pagina_Catasto_Edit(piva As String,
                                                    titoloPossesso As Integer,
                                                    validitaInizio As Date,
                                                    validitaFine As Date
                                                    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objAgenda As New Parametri_ObjParametriAgenda_NG

            objAgenda.Pagina_Richiesta = enum_PagineGiasNG.Pagina_Edit_Catasto
            objAgenda.Pagina_Provenienza = 0
            objAgenda.Piva = piva
            objAgenda.TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura
            objAgenda.QueryStringFiltrino = String.Format("?seFrame=1" &
                                                          "&IxP_TitoloPossesso={0}" &
                                                          "&IxP_ValiditaInizio={1}" &
                                                          "&IxP_ValiditaFine={2}",
                                                          titoloPossesso,
                                                          formattaDataInStringaAnnoMeseGiorno(validitaInizio),
                                                          formattaDataInStringaAnnoMeseGiorno(validitaFine))

            objAgenda.Salva()

            r.RispostaStringa = RedirectGestione.IndirizzoCompleto_SitoGiasNG_PassandoDirettamente_ParametriAgenda_NG(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgenda)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    Private Shared Function formattaDataInStringaAnnoMeseGiorno(ByVal data As Date) As String

        Dim dataOra As DateTime = data

        Return dataOra.ToString("yyyyMMdd")

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaCastelletto(ByVal piva As String,
                                                    ByVal id_agenda As Integer,
                                                    ByVal lav_cod As Integer) As RispostaStandard

        Dim dt As New DataTable
        Dim dr As DataRow
        Dim r As New RispostaStandard
        Dim ObjProdotti As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ

        Dim Variazioni As Decimal = 0
        Dim Imposta As Decimal = 0
        Dim Importo As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Imponibile_Lordo As Decimal = 0
        Dim listCastelletto As List(Of Contabilita_Castelletto_Iva) = Nothing
        Dim objMovimentiHelper As New Agenda_Movimenti_Helper
        Dim objMovimenti_DettagliHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim objMovimenti_DestinazioniHelper As New Agenda_Movimenti_Destinazioni_Helper
        Dim utilityHelper As New AgronicaCoreModello.UtilityHelper
        Dim objMovDets As List(Of Movimento_Dettaglio)

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            dt.Columns.Add(New DataColumn("Cod_Iva", GetType(Integer)))
            dt.Columns.Add(New DataColumn("Imponibile", GetType(Decimal)))
            dt.Columns.Add(New DataColumn("Aliquota", GetType(String)))
            dt.Columns.Add(New DataColumn("Imposta", GetType(Decimal)))

            'Lettura Dettagli
            objMovDets = objMovimenti_DettagliHelper.Leggi(piva, 0, id_agenda, 0, 0, objParametriServer)

            objMovDets = objMovDets.Where(Function(det) Not {RIGA_IMBALLI_CONTENTI_PRODOTTI, RIGA_IMBALLI_VUOTI_IN_ENTRATA}.Contains(det.Ordine_Det)).ToList()

            Importo = utilityHelper.Calcola_Importo_Documento(Imponibile_Netto,
                                                              Imponibile_Lordo,
                                                              Variazioni,
                                                              Imposta,
                                                              listCastelletto,
                                                              lav_cod,
                                                              0,
                                                              objMovDets,
                                                              objParametriServer)


            If listCastelletto.Count > 0 Then

                For Each Riga As Contabilita_Castelletto_Iva In listCastelletto

                    dr = dt.NewRow

                    dr("Cod_Iva") = Riga.Cod_Iva
                    dr("Aliquota") = Riga.Iva_Des
                    dr("Imposta") = Riga.Iva
                    dr("Imponibile") = Riga.Imponibile_Netto

                    dt.Rows.Add(dr)

                Next

            End If

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

            Dim RiepilogoImporti As New Contabilita_Riepilogo_Importi With {
                .ImponibileLordo = Imponibile_Lordo,
                .Variazioni = Variazioni,
                .ImponibileNetto = Imponibile_Netto,
                .Imposta = Imposta,
                .TotaleDocumento = Importo
            }
            r.ParametroDue_stringa = JsonConvert.SerializeObject(RiepilogoImporti, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function Agenda_Consistente(ByVal piva As String, ByVal id_agenda As Integer) As Boolean

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim bConsistente As Boolean = True

        Try

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
                Throw New Exception("Sessione Scaduta")
            End If

            Dim objMovDett As New Agenda_Movimenti_Dettagli_Helper
            Dim listaDettagli As IList(Of Movimento_Dettaglio)
            listaDettagli = objMovDett.Leggi(piva, 0, id_agenda, 0, 0, objParametriServer)
            objMovDett = Nothing

            If Not IsNothing(listaDettagli) Then

                If listaDettagli.Count = 0 Then

                    'Cancellazione Agenda
                    Dim objAgendaHelper As New Agenda_Operazione_Helper
                    bConsistente = Not objAgendaHelper.Cancella(piva, 0, id_agenda, False, objParametriServer)

                End If

            End If

        Catch ex As Exception


        End Try

        Return bConsistente

    End Function




#End Region


#Region "Calcolo lotto da assegnare"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Impostazione_LottoProdotto(ByVal piva As String,
                                                      ByVal sa_cod As Integer,
                                                      ByVal modulo_generazione As Integer,
                                                      ByVal mat_cod As Integer,
                                                      ByVal cod_contatto As String,
                                                      ByVal cod_risum As Integer,
                                                      ByVal doc_numero_sin As String,
                                                      ByVal doc_numero As Integer,
                                                      ByVal doc_numero_des As String,
                                                      ByVal doc_numero_sin_accettazione As String,
                                                      ByVal doc_numero_accettazione As Integer,
                                                      ByVal doc_numero_des_accettazione As String,
                                                      ByVal data_ingresso As DateTime,
                                                      ByVal qualita_cod As Integer,
                                                      ByVal destinazioni_cod As String,
                                                      ByVal appezzamenti As String,
                                                      ByVal lotti_impianti As String,
                                                      ByVal certificato_cod As Integer,
                                                      ByVal contatore_univoco_parametri As String,
                                                      ByVal sigla_certificazione As String) As RispostaStandard

        Dim errCode As Integer = 0
        Dim risultato As String
        Dim r As New RispostaStandard


        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objMagazzinoBiz As New FF_MagazzinoBIZ
            risultato = objMagazzinoBiz.Impostazione_LottoProdotto(piva,
                                                                   sa_cod,
                                                                   modulo_generazione,
                                                                   mat_cod,
                                                                   cod_contatto,
                                                                   cod_risum,
                                                                   doc_numero_sin,
                                                                   doc_numero,
                                                                   doc_numero_des,
                                                                   doc_numero_sin_accettazione,
                                                                   doc_numero_accettazione,
                                                                   doc_numero_des_accettazione,
                                                                   data_ingresso,
                                                                   qualita_cod,
                                                                   destinazioni_cod,
                                                                   appezzamenti,
                                                                   lotti_impianti,
                                                                   certificato_cod,
                                                                   contatore_univoco_parametri,
                                                                   sigla_certificazione,
                                                                   objParametriServer,
                                                                   errCode)

            '============================================================================================================
            'Controllo Errore
            '------------------------------------------------------------------------------------------------------------
            If Trim(errCode) <> 0 Then

                r.RispostaStringa = risultato
                r.RispostaOK = False

            Else

                r.RispostaStringa = risultato
                r.RispostaOK = True

            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
#End Region

#Region "Scripts Nuovo Contatto"

    <WebMethod(EnableSession:=True)>
    Public Shared Function UrlNuovoContatto(ByVal piva As String, ByVal lavCod As Integer) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            r.RispostaStringa = Redirect_Nuovo_Contatto(piva, lavCod)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Private Shared Function Redirect_Nuovo_Contatto(ByVal piva As String, ByVal lavCod As Integer) As String

        Dim tipoRappCont As Integer = 0

        Select Case lavCod
            Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA,
                 LAVCOD_ORDINE_VENDITA
                tipoRappCont = COD_CLIENTE
            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA,
                 LAVCOD_ORDINE_ACQUISTO
                tipoRappCont = COD_FORNITORE
        End Select

        Dim url As String = "../Anagrafica/New_Contatto_Edit.aspx"

        Dim queryString As String = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Nothing) &
                                    "&tipo_rapporto=" & Stringa_Codifica(tipoRappCont, AgroKey_EncoderDecoder, Nothing) &
                                    "&lav_cod=" & Stringa_Codifica(lavCod, AgroKey_EncoderDecoder, Nothing) &
                                    "&piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing) &
                                    "&orig=" & Stringa_Codifica(enum_PagineGiasOnline.Fattura, AgroKey_EncoderDecoder, Nothing) &
                                    "&codcont=" & Stringa_Codifica("0", AgroKey_EncoderDecoder, Nothing)

        Return url & queryString

    End Function

#End Region

#Region "Scripts Gestione Contatto"

    <WebMethod(EnableSession:=True)>
    Public Shared Function UrlGestioneContatto(
                        ByVal piva As String,
                        ByVal lavCod As Integer,
                        ByVal cod_Contatto As String,
                        ByVal tipologia_Contatto As Integer,
                        ByVal solo_Lettura As Boolean
                        ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim permessi = New PermessiUtente

            Dim utenteAbilitato_Modifica = permessi.getPermesso(enum_Security_Attivita.Anagrafica_Contatto).Scrittura

            If Not utenteAbilitato_Modifica Then
                r.RispostaOK = False
                r.Errore = AgronicaAgenda_2010.MancanzaPermessiModificaContatti
            Else
                r.RispostaStringa = Redirect_Gestione_Contatto(piva, lavCod, cod_Contatto, tipologia_Contatto, solo_Lettura)
                r.RispostaOK = True
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    Private Shared Function Redirect_Gestione_Contatto(
                            ByVal piva As String,
                            ByVal lavCod As Integer,
                            ByVal cod_Contatto As String,
                            ByVal tipologia_Contatto As Integer,
                            ByVal solo_Lettura As Boolean) As String

        Dim tipoRappCont As Integer = 0
        Dim idCf As Integer = PERSONA_FISICA

        Select Case tipologia_Contatto
            Case 1, 2
                ' Cedente / Cessionario
                Select Case lavCod
                    Case LAVCOD_FATTURA_EMESSA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA,
                        LAVCOD_ORDINE_VENDITA
                        tipoRappCont = COD_CLIENTE
                        idCf = PERSONA_GIURIDICA
                    Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA,
                        LAVCOD_ORDINE_ACQUISTO
                        idCf = PERSONA_GIURIDICA
                        tipoRappCont = COD_FORNITORE
                    Case LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                        idCf = PERSONA_GIURIDICA
                        tipoRappCont = COD_CONFERENTE
                End Select
            Case 3
                ' Agente
                tipoRappCont = COD_AGENTE
            Case 4
                ' vettore
                tipoRappCont = COD_TRASPORTATORE
                idCf = PERSONA_GIURIDICA
            Case 5
                ' capo area
                tipoRappCont = COD_CAPO_AREA
        End Select

        Dim url As String = "../Anagrafica/New_Contatto_Edit.aspx"
        Dim operazione As String = If(cod_Contatto = "0", "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Scrittura, AgroKey_EncoderDecoder, Nothing),
                                                          "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Modifica, AgroKey_EncoderDecoder, Nothing))

        If (solo_Lettura) Then
            operazione = "?o=" & Stringa_Codifica(enum_TipoOperazioneDB.Lettura, AgroKey_EncoderDecoder, Nothing)
        End If

        Dim queryString As String = operazione &
                                    "&tipo_rapporto=" & Stringa_Codifica(tipoRappCont, AgroKey_EncoderDecoder, Nothing) &
                                    "&lav_cod=" & Stringa_Codifica(lavCod, AgroKey_EncoderDecoder, Nothing) &
                                    "&piva=" & Stringa_Codifica(piva, AgroKey_EncoderDecoder, Nothing) &
                                    "&codcont=" & Stringa_Codifica(cod_Contatto, AgroKey_EncoderDecoder, Nothing) &
                                    "&orig=" & Stringa_Codifica(enum_PagineAgenda_2010.Pagina_DocumentoContabileGenerico, AgroKey_EncoderDecoder, Nothing) &
                                    "&id_Cf=" & Stringa_Codifica(idCf, AgroKey_EncoderDecoder, Nothing)


        Return url & queryString

    End Function

#End Region

#Region "Salvataggio impostazioni PanelBars"

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function SalvaStatiPanelBar(ByVal parametri As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.UTENTE_STATI_PANEL_BAR, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)

            Dim trovato As Boolean = False

            Dim jArrayListaDaSalvare As New JArray()
            Dim daSalvare = JsonConvert.DeserializeObject(parametri)(0)
            Dim daModificare = Nothing

            If dtImpostazioni.Rows.Count > 0 Then
                Dim salvati = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                For Each s In salvati
                    If s("LavCod") = daSalvare("LavCod") Then
                        trovato = True
                        daModificare = s
                    Else
                        jArrayListaDaSalvare.Add(s)
                    End If
                Next
            End If

            If Not trovato Then
                jArrayListaDaSalvare.Add(daSalvare)
            Else
                trovato = False
                Dim jArrayPannelli As New JArray

                Dim pannelliSalvati = daModificare("Pannelli")
                Dim idPannelloPadreDaSalvare = daSalvare("Pannelli")(0)("idPannelloPadre").ToString()
                Dim idPannelloFiglioDaSalvare = daSalvare("Pannelli")(0)("idPannelloFiglio").ToString()

                For Each p In pannelliSalvati
                    If p("idPannelloPadre") = idPannelloPadreDaSalvare AndAlso p("idPannelloFiglio") = idPannelloFiglioDaSalvare Then
                        p("Stato") = daSalvare("Pannelli")(0)("Stato")
                        trovato = True
                        Exit For
                    End If
                Next
                If Not trovato Then
                    DirectCast(daModificare("Pannelli"), JArray).Add(daSalvare("Pannelli")(0))
                End If
                jArrayListaDaSalvare.Add(daModificare)
            End If


            Dim scriviImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            r.RispostaOK = scriviImpostazioni.Cancella(enum_Impostazioni_Utenti.UTENTE_STATI_PANEL_BAR, "", objParametriUtenti)
            r.RispostaOK = scriviImpostazioni.Scrivi(enum_Impostazioni_Utenti.UTENTE_STATI_PANEL_BAR, JsonConvert.SerializeObject(jArrayListaDaSalvare, Formatting.None), "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametriUtenti)

            r.RispostaStringa = ""

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function CaricaStatiPanelBar(ByVal Lav_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim leggiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dtImpostazioni = leggiImpostazioni.Leggi(enum_Impostazioni_Utenti.UTENTE_STATI_PANEL_BAR, 1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)

            Dim trovato As Boolean = False

            Dim impostazioni = Nothing
            If dtImpostazioni.Rows.Count > 0 Then
                Dim salvati = JsonConvert.DeserializeObject(dtImpostazioni.Rows(0).Item("Impostazione_Valore_1"))
                For Each s In salvati
                    If s("LavCod") = Lav_Cod Then
                        trovato = True
                        impostazioni = s
                    End If
                Next
            End If

            r.RispostaOK = True
            If trovato Then
                Dim pannelli = impostazioni("Pannelli")
                r.RispostaStringa = JsonConvert.SerializeObject(pannelli, Formatting.None)
            Else
                r.RispostaStringa = ""
            End If
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Salvataggio"

    Private Enum BF
        Cau_Mov = 0
        Piva = 1
        Id_Agenda = 2
        Id_Mov_Testata = 3
        Id_Mov_Magazzino = 4
        Lav_Cod = 5
        Des_Lib = 6
        Sa_Cod = 7
        Fabbricato_Cod = 8
        Righe_Inserite = 9
        Righe_Modificate = 10
        Righe_Cancellate = 11
        Mov_Det_Des = 12
        Cod_Risum = 13
        Numero_Colli = 14
        Cod_Indirizzo = 15
        Doc_Numero_Sin = 16
        Doc_Numero = 17
        Doc_Numero_Des = 18
        Doc_Numero_Visualizzato = 19
        Doc_Numero_Lock = 20
        Doc_Numero_Lunghezza = 21
        Doc_Numero_Carattere_Formattazione = 22
        Ordine_Det = 23
        Conteggi_Totali = 24
    End Enum


    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaBeniConfezionamento(ByVal piva_carico As String,
                                                       ByVal id_agenda_carico As Integer,
                                                       ByVal id_mov_testata_carico As Integer,
                                                       ByVal id_mov_carico As Integer,
                                                       ByVal lav_cod_carico As Integer,
                                                       ByVal rag_soc_carico As String,
                                                       ByVal docnumerosin_carico As String,
                                                       ByVal docnumero_carico As Integer,
                                                       ByVal docnumero_lunghezza_carico As Integer,
                                                       ByVal docnumero_carattere_formattazione_carico As String,
                                                       ByVal docnumerodes_carico As String,
                                                       ByVal docnumero_visualizzato_carico As String,
                                                       ByVal docnumero_lock_carico As Boolean,
                                                       ByVal sa_cod_carico As Integer,
                                                       ByVal fabbricato_cod_carico As Integer,
                                                       ByVal numero_colli_carico As Integer,
                                                       ByVal cod_risum_carico As Integer,
                                                       ByVal piva_scarico As String,
                                                       ByVal id_agenda_scarico As Integer,
                                                       ByVal id_mov_testata_scarico As Integer,
                                                       ByVal id_mov_scarico As Integer,
                                                       ByVal lav_cod_scarico As Integer,
                                                       ByVal rag_soc_scarico As String,
                                                       ByVal docnumerosin_scarico As String,
                                                       ByVal docnumero_scarico As Integer,
                                                       ByVal docnumero_lunghezza_scarico As Integer,
                                                       ByVal docnumero_carattere_formattazione_scarico As String,
                                                       ByVal docnumerodes_scarico As String,
                                                       ByVal docnumero_visualizzato_scarico As String,
                                                       ByVal docnumero_lock_scarico As Boolean,
                                                       ByVal sa_cod_scarico As Integer,
                                                       ByVal fabbricato_cod_scarico As Integer,
                                                       ByVal numero_colli_scarico As Integer,
                                                       ByVal cod_risum_scarico As Integer,
                                                       ByVal data_movimento As String,
                                                       ByVal data_registrazione As String,
                                                       ByVal cod_indirizzo_scarico As Integer,
                                                       ByVal righeInseriteGrid_Carico As String, ByVal righeModificateGrid_Carico As String, ByVal righeCancellateGrid_Carico As String,
                                                       ByVal righeInseriteGrid_Scarico As String, ByVal righeModificateGrid_Scarico As String, ByVal righeCancellateGrid_Scarico As String
                                                       ) As RispostaStandard

        Dim objAgenda As Operazione_Agenda

        Dim objAgendaHelper As New Agenda_Operazione_Helper
        Dim objMovimentiHelper As New Agenda_Movimenti_Helper
        Dim objMovimenti_DettagliHelper As New Agenda_Movimenti_Dettagli_Helper
        Dim objMovimenti_DestinazioniHelper As New Agenda_Movimenti_Destinazioni_Helper

        Dim bOk As Boolean
        Dim bSalva As Boolean
        Dim bModifica As Boolean
        Dim bCancella As Boolean
        Dim bDelete_Agenda As Boolean
        Dim righeArray As JArray = Nothing

        Dim Chiavi As String

        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False
        Dim Dati(0 To 26, 0 To 2) As String

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If


        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            'TODO: compito di questa funzione NON DEVE ESSERE quello di scrivere/aggiornare Agenda e movimenti di testata PER CAU_CARICO,
            'perché non avrei tutti i dati e rischierei cmq di cambiare informazioni che non mi competono

            'In caso di scarico invece devo poter scrivere e modificare testata (in ogni caso modifico solo determinati campi, non tutto)

            'Inserimento in struttura di appoggio binaria

            'Carico
            Dati(BF.Cau_Mov, 0) = CAU_CARICO
            Dati(BF.Piva, 0) = piva_carico
            Dati(BF.Id_Agenda, 0) = id_agenda_carico
            Dati(BF.Id_Mov_Testata, 0) = id_mov_testata_carico
            Dati(BF.Id_Mov_Magazzino, 0) = id_mov_carico
            Dati(BF.Lav_Cod, 0) = lav_cod_carico
            Dati(BF.Des_Lib, 0) = AgronicaCoreContabHLP.Contabilita.CreaDesLib(lav_cod_carico, docnumerosin_carico, docnumero_carico, docnumerodes_carico, rag_soc_carico)
            Dati(BF.Sa_Cod, 0) = sa_cod_carico
            Dati(BF.Fabbricato_Cod, 0) = fabbricato_cod_carico
            Dati(BF.Righe_Inserite, 0) = righeInseriteGrid_Carico
            Dati(BF.Righe_Modificate, 0) = righeModificateGrid_Carico
            Dati(BF.Righe_Cancellate, 0) = righeCancellateGrid_Carico
            Dati(BF.Mov_Det_Des, 0) = "Carico"
            Dati(BF.Cod_Risum, 0) = cod_risum_carico
            Dati(BF.Numero_Colli, 0) = numero_colli_carico
            Dati(BF.Cod_Indirizzo, 0) = 0
            Dati(BF.Doc_Numero_Sin, 0) = docnumerosin_carico
            Dati(BF.Doc_Numero, 0) = docnumero_carico
            Dati(BF.Doc_Numero_Des, 0) = docnumerodes_carico
            Dati(BF.Doc_Numero_Visualizzato, 0) = docnumero_visualizzato_carico
            Dati(BF.Doc_Numero_Lock, 0) = docnumero_lock_carico
            Dati(BF.Doc_Numero_Lunghezza, 0) = docnumero_lunghezza_carico
            Dati(BF.Doc_Numero_Carattere_Formattazione, 0) = docnumero_carattere_formattazione_carico
            Dati(BF.Ordine_Det, 0) = 30000
            Dati(BF.Conteggi_Totali, 0) = JsonConvert.SerializeObject(Nothing)

            'Scarico
            Dati(BF.Cau_Mov, 1) = CAU_SCARICO
            Dati(BF.Piva, 1) = piva_scarico
            Dati(BF.Id_Agenda, 1) = id_agenda_scarico
            Dati(BF.Id_Mov_Testata, 1) = id_mov_testata_scarico
            Dati(BF.Id_Mov_Magazzino, 1) = id_mov_scarico
            Dati(BF.Lav_Cod, 1) = lav_cod_scarico
            Dati(BF.Des_Lib, 1) = "" ' lo costruisco dopo, perché potrebbe cambiare il numero del documento a causa dell'algoritmo
            Dati(BF.Sa_Cod, 1) = sa_cod_scarico
            Dati(BF.Fabbricato_Cod, 1) = fabbricato_cod_scarico
            Dati(BF.Righe_Inserite, 1) = righeInseriteGrid_Scarico
            Dati(BF.Righe_Modificate, 1) = righeModificateGrid_Scarico
            Dati(BF.Righe_Cancellate, 1) = righeCancellateGrid_Scarico
            Dati(BF.Mov_Det_Des, 1) = "Scarico"
            Dati(BF.Cod_Risum, 1) = cod_risum_scarico
            Dati(BF.Numero_Colli, 1) = numero_colli_scarico
            Dati(BF.Cod_Indirizzo, 1) = cod_indirizzo_scarico
            Dati(BF.Doc_Numero_Sin, 1) = docnumerosin_scarico
            Dati(BF.Doc_Numero, 1) = docnumero_scarico
            Dati(BF.Doc_Numero_Des, 1) = docnumerodes_scarico
            Dati(BF.Doc_Numero_Visualizzato, 1) = docnumero_visualizzato_scarico
            Dati(BF.Doc_Numero_Lock, 1) = docnumero_lock_scarico
            Dati(BF.Doc_Numero_Lunghezza, 1) = docnumero_lunghezza_scarico
            Dati(BF.Doc_Numero_Carattere_Formattazione, 1) = docnumero_carattere_formattazione_scarico
            Dati(BF.Ordine_Det, 1) = 0
            Dati(BF.Conteggi_Totali, 1) = JsonConvert.SerializeObject(Nothing)


            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)


            For indice = 0 To 1

                bSalva = False
                bModifica = False
                bCancella = False

                'Controllo presenza dati
                If (Dati(BF.Righe_Inserite, indice) <> "" AndAlso Dati(BF.Righe_Inserite, indice) <> "[]") OrElse
                   (Dati(BF.Righe_Modificate, indice) <> "" AndAlso Dati(BF.Righe_Modificate, indice) <> "[]") OrElse
                   (Dati(BF.Righe_Cancellate, indice) <> "" AndAlso Dati(BF.Righe_Cancellate, indice) <> "[]") Then


                    'TODO: algoritmo assegnazione numero doc (da fare solo se SCARICO)

                    If indice = 1 Then

                        Dim msgErrorCoerenzaDate As String = ""
                        Dim newNumDoc As Integer = 0
                        Dim newNumDocVisualizzato As String = ""
                        msgErrorCoerenzaDate = UtilityHelper.AlgoritmoAssegnazioneNumDoc(newNumDoc, newNumDocVisualizzato,
                                                                                         CInt(Dati(BF.Lav_Cod, indice)),
                                                                                         Dati(BF.Piva, indice),
                                                                                         CDate(data_movimento),
                                                                                         CStr(Dati(BF.Doc_Numero_Sin, indice)),
                                                                                         CInt(Dati(BF.Doc_Numero, indice)),
                                                                                         CStr(Dati(BF.Doc_Numero_Des, indice)),
                                                                                         CBool(Dati(BF.Doc_Numero_Lock, indice)),
                                                                                         CStr(Dati(BF.Doc_Numero_Visualizzato, indice)),
                                                                                         objParametriServer,
                                                                                         CInt(Dati(BF.Doc_Numero_Lunghezza, indice)),
                                                                                         CStr(Dati(BF.Doc_Numero_Carattere_Formattazione, indice)))

                        If msgErrorCoerenzaDate <> "" Then
                            Throw New Exception(msgErrorCoerenzaDate)
                        End If

                        Dati(BF.Des_Lib, indice) = CreaDesLib(CInt(Dati(BF.Lav_Cod, indice)),
                                                              Dati(BF.Doc_Numero_Sin, indice), newNumDoc, Dati(BF.Doc_Numero_Des, indice),
                                                              rag_soc_scarico, numeroDocFormattato:=newNumDocVisualizzato)

                        Dati(BF.Doc_Numero, indice) = newNumDoc
                        Dati(BF.Doc_Numero_Visualizzato, indice) = newNumDocVisualizzato

                    End If



                    '------------------------------------------------
                    '----- AGENDA
                    '------------------------------------------------
                    objAgenda = New Operazione_Agenda With {
                        .Tipo_Operazione = enum_TipoOperazioneDB.Scrittura,
                        .Id_Agenda = CInt(Dati(BF.Id_Agenda, indice)),
                        .Data = CDate(data_movimento),
                        .Piva = Dati(BF.Piva, indice),
                        .Sa_Cod = 0,
                        .Lav_Cod = CInt(Dati(BF.Lav_Cod, indice)),
                        .Linea_Cod = 0,
                        .Preparazione_Cod = 0,
                        .Id_Trasformazione = 0,
                        .Des_Lib = Dati(BF.Des_Lib, indice)
                    }

                    '------------------------------------------------
                    '----- MOVIMENTI
                    '------------------------------------------------

                    'MOVIMENTO DI TESTATA
                    Dim objMovimentoT = New Movimento With {
                        .Piva = objAgenda.Piva,
                        .Id_Agenda = objAgenda.Id_Agenda,
                        .Id_Mov = CInt(Dati(BF.Id_Mov_Testata, indice)),
                        .Sa_Cod = 0,
                        .Cod_Risum = CInt(Dati(BF.Cod_Risum, indice)),
                        .Cod_IndirizzoRisUm = CInt(Dati(BF.Cod_Indirizzo, indice)),
                        .Doc_Numero_Sin = CStr(Dati(BF.Doc_Numero_Sin, indice)),
                        .Doc_Numero = CInt(Dati(BF.Doc_Numero, indice)),
                        .Doc_Numero_Des = CStr(Dati(BF.Doc_Numero_Des, indice)),
                        .Doc_Numero_Visualizzato = CStr(Dati(BF.Doc_Numero_Visualizzato, indice)),
                        .Data = objAgenda.Data,
                        .Data_Registrazione = CDate(data_registrazione),
                        .Lav_Cod = objAgenda.Lav_Cod,
                        .Cau_Mov = CAU_REGISTRAZIONI,
                        .Mov_Desc = "",
                        .Colli = Dati(BF.Numero_Colli, indice),
                        .Ora = CDate(data_movimento),
                        .Extra_Date = #12/30/1899#
                    }

                    'MOVIMENTO DI CARICO/SCARICO
                    Dim objMovimentoC = New Movimento With {
                        .Piva = objAgenda.Piva,
                        .Id_Agenda = objAgenda.Id_Agenda,
                        .Id_Mov = CInt(Dati(BF.Id_Mov_Magazzino, indice)),
                        .Sa_Cod = 0,
                        .Cod_Risum = CInt(Dati(BF.Cod_Risum, indice)),
                        .Cod_IndirizzoRisUm = CInt(Dati(BF.Cod_Indirizzo, indice)),
                        .Doc_Numero_Sin = CStr(Dati(BF.Doc_Numero_Sin, indice)),
                        .Doc_Numero = CInt(Dati(BF.Doc_Numero, indice)),
                        .Doc_Numero_Des = CStr(Dati(BF.Doc_Numero_Des, indice)),
                        .Doc_Numero_Visualizzato = CStr(Dati(BF.Doc_Numero_Visualizzato, indice)),
                        .Data = objAgenda.Data,
                        .Data_Registrazione = CDate(data_registrazione),
                        .Lav_Cod = objAgenda.Lav_Cod,
                        .Cau_Mov = Dati(BF.Cau_Mov, indice),
                        .Mov_Desc = Dati(BF.Mov_Det_Des, indice) & " Beni Confezionamento",
                        .Colli = Dati(BF.Numero_Colli, indice),
                        .Scadenza_Extra = AGRODATAINIZIO,
                        .Scadenza = AGRODATAFINE,
                        .Ora = CDate(data_movimento),
                        .Extra_Date = #12/30/1899#
                    }


                    For TipoOperazioneDB = 1 To 3

                        bOk = False

                        Select Case TipoOperazioneDB

                            Case enum_TipoOperazioneDB.Scrittura
                                'Righe Inserite
                                If Dati(BF.Righe_Inserite, indice) <> "" And Dati(BF.Righe_Inserite, indice) <> "[]" Then
                                    righeArray = JArray.Parse(Dati(BF.Righe_Inserite, indice))
                                    bOk = True
                                End If

                            Case enum_TipoOperazioneDB.Modifica
                                'Righe Modificate
                                If Dati(BF.Righe_Modificate, indice) <> "" And Dati(BF.Righe_Modificate, indice) <> "[]" Then
                                    righeArray = JArray.Parse(Dati(BF.Righe_Modificate, indice))
                                    bOk = True
                                End If

                            Case enum_TipoOperazioneDB.Cancellazione
                                'Righe Cancellate
                                If Dati(BF.Righe_Cancellate, indice) <> "" And Dati(BF.Righe_Cancellate, indice) <> "[]" Then
                                    righeArray = JArray.Parse(Dati(BF.Righe_Cancellate, indice))
                                    bOk = True
                                End If

                        End Select



                        If bOk Then

                            For Each obj As JObject In righeArray

                                '------------------------------------------------
                                '----- MOVIMENTI DETTAGLI
                                '------------------------------------------------
                                Dim objMovDettagli = New Movimento_Dettaglio With {
                                    .Piva = objAgenda.Piva,
                                    .Sa_Cod = Dati(BF.Sa_Cod, indice),
                                    .Id_Agenda = objMovimentoC.Id_Agenda,
                                    .Id_Mov = objMovimentoC.Id_Mov,
                                    .Id_Mov_Det = Val(obj("Id_Mov_Det")),
                                    .Lav_Cod = objAgenda.Lav_Cod,
                                    .Elem_Cod = Val(obj("Elem_Cod")),
                                    .Pro_Cod = 0,
                                    .Mat_Cod = Val(obj("Mat_Cod")),
                                    .Mov_Det_Des = Dati(BF.Mov_Det_Des, indice) & " " & Agro_SQL_SaveText(obj("Mat_Des")),
                                    .Qta = Val(obj("Qta")),
                                    .Udm_Cod = enum_UnitaMisura.Numero,
                                    .Jolly_Int = 0,
                                    .Contabilizzato = CONTABILE,
                                    .Pendente = enum_Pendenza.MovGiustificato,
                                    .Lotto = Agro_SQL_SaveText(obj("Lotto")),
                                    .Udm_Cod_Extra = 0,
                                    .Qta_Extra = Val(obj("Tara")),
                                    .Qta_Extra_Totale = Val(obj("Qta")) * Val(obj("Tara")),
                                    .Ordine_Det = Dati(BF.Ordine_Det, indice),
                                    .Validita_Inizio = CDate(data_movimento),
                                    .Validita_Fine = AGRODATAFINE
                                }


                                '------------------------------------------------
                                '----- MOVIMENTI DESTINAZIONI
                                '------------------------------------------------
                                Dim objMovDestinazioni = New Movimento_Destinazione With {
                                    .Piva = objAgenda.Piva,
                                    .Sa_Cod = objMovDettagli.Sa_Cod,
                                    .Id_Agenda = objMovimentoC.Id_Agenda,
                                    .Id_Mov = objMovimentoC.Id_Mov,
                                    .Id_Mov_Det = objMovDettagli.Id_Mov_Det,
                                    .Tipo = MAGAZZINO,
                                    .Id_Destinazione = Dati(BF.Fabbricato_Cod, indice),
                                    .Qta = Val(obj("Qta"))
                                }


                                Select Case TipoOperazioneDB

                                    Case enum_TipoOperazioneDB.Scrittura 'Salvataggio

                                        'Aggancio la destinazione al dettaglio
                                        objMovDettagli.Movimenti_Destinazioni = New List(Of Movimento_Destinazione) From {objMovDestinazioni}

                                        bSalva = True

                                        'Controllo Id_Agenda
                                        If CInt(Dati(BF.Id_Agenda, indice)) <> 0 Then

                                            'Inserimento Nuovi Dettagli su Agenda Esistente
                                            If Not IsNothing(objMovDettagli) Then
                                                bOk = objMovimenti_DettagliHelper.Scrivi(objMovDettagli, objParametriServer)
                                                objMovDettagli = Nothing
                                            End If

                                        Else

                                            'Aggancio i movimenti dettagli al movimento
                                            objMovimentoC.Movimenti_Dettagli = New List(Of Movimento_Dettaglio) From {objMovDettagli}
                                            objMovDettagli = Nothing

                                        End If


                                    Case enum_TipoOperazioneDB.Modifica 'Modifica

                                        'In modifica non ho bisogno del ByPass per il lotto, perché non viene mai ritrasformato in Indefinito

                                        If Not IsNothing(objMovDettagli) Then
                                            'TODO: dovrebbe passare per l'altra modifica puntuale, perché questa sovrascrive in realtà tutto!
                                            bOk = objMovimenti_DettagliHelper.ModificaPuntuale(objMovDettagli.Piva, objMovDettagli.Sa_Cod,
                                                                                               objMovDettagli.Id_Agenda, objMovDettagli.Id_Mov,
                                                                                               objMovDettagli.Id_Mov_Det, objParametriServer,
                                                                                               objMovDettagli)
                                            objMovDettagli = Nothing
                                        End If

                                        If Not IsNothing(objMovDestinazioni) Then
                                            'TODO: dovrebbe passare per l'altra modifica puntuale, perché questa sovrascrive in realtà tutto!
                                            bOk = objMovimenti_DestinazioniHelper.ModificaPuntuale(objMovDestinazioni.Piva, objMovDestinazioni.Sa_Cod,
                                                                                                   objMovDestinazioni.Id_Agenda, objMovDestinazioni.Id_Mov,
                                                                                                   objMovDestinazioni.Id_Mov_Det, 0,
                                                                                                   objMovDestinazioni.Id_Destinazione, objParametriServer,
                                                                                                   objMovDestinazioni)
                                            objMovDestinazioni = Nothing
                                        End If

                                        bModifica = True


                                    Case enum_TipoOperazioneDB.Cancellazione 'Cancellazione

                                        If Not IsNothing(objMovDettagli) Then
                                            bOk = objMovimenti_DettagliHelper.Cancella(objMovDettagli.Piva, 0,
                                                                                       objMovDettagli.Id_Agenda, objMovDettagli.Id_Mov,
                                                                                       objMovDettagli.Id_Mov_Det, objParametriServer)
                                            objMovDettagli = Nothing
                                        End If

                                        bCancella = True

                                End Select

                            Next

                        End If

                    Next

                    'Controllo 'Id_Agenda
                    If CInt(Dati(BF.Id_Agenda, indice)) = 0 Then

                        'Aggancio i movimenti sull'agenda in scrittura
                        objAgenda.Movimenti = New List(Of Movimento) From {
                            objMovimentoT,
                            objMovimentoC
                        }


                        If Not IsNothing(objAgenda) Then
                            Dati(BF.Id_Agenda, indice) = objAgendaHelper.Scrivi(objAgenda, objParametriServer, flagUsaOraReale:=True)

                            'Lettura Chiavi Tabella Movimenti
                            Dim objMov As New Agenda_Movimenti_Helper
                            Dim listaMovimenti As IList(Of Movimento)
                            listaMovimenti = objMov.Leggi(CStr(Dati(BF.Piva, indice)), 0, CInt(Dati(BF.Id_Agenda, indice)), objParametriServer)
                            objMov = Nothing

                            If Not IsNothing(listaMovimenti) Then

                                For Each movimento In listaMovimenti

                                    Select Case movimento.Cau_Mov

                                        Case CAU_REGISTRAZIONI

                                            Dati(BF.Id_Mov_Testata, indice) = movimento.Id_Mov

                                        Case CAU_CARICO, CAU_SCARICO

                                            Dati(BF.Id_Mov_Magazzino, indice) = movimento.Id_Mov

                                    End Select

                                Next

                            End If

                            objAgenda = Nothing
                            objMovimentoT = Nothing
                            objMovimentoC = Nothing

                        End If


                    ElseIf bModifica Or bCancella Then

                        bDelete_Agenda = False

                        If bCancella Then

                            'Controllo Esistenza Dettagli per Agenda
                            If Not Agenda_Consistente(objAgenda.Piva, objAgenda.Id_Agenda) Then

                                'Azzeramento Chiavi
                                Dati(BF.Id_Agenda, indice) = 0
                                Dati(BF.Id_Mov_Testata, indice) = 0
                                Dati(BF.Id_Mov_Magazzino, indice) = 0

                                bDelete_Agenda = True

                            End If


                        End If


                        'Se indice = 0 (CARICO) non lo devo fare perché mi potrebbe cambiare i dati del documento in cui già sono!
                        If Not bDelete_Agenda AndAlso indice = 1 Then

                            'Modifica Puntuale Agenda e Movimenti

                            'Agenda
                            If Not IsNothing(objAgenda) Then
                                bOk = objAgendaHelper.ModificaPuntuale(objAgenda.Piva, objAgenda.Sa_Cod,
                                                                       objAgenda.Id_Agenda, objParametriServer,
                                                                       validitaInizio:=objAgenda.Data,
                                                                       lavCod:=objAgenda.Lav_Cod,
                                                                       desLib:=objAgenda.Des_Lib)
                                objAgenda = Nothing
                            End If

                            'Movimento Testata
                            If Not IsNothing(objMovimentoT) Then
                                bOk = objMovimentiHelper.ModificaPuntuale(objMovimentoT.Piva, objMovimentoT.Sa_Cod,
                                                                          objMovimentoT.Id_Agenda, objMovimentoT.Id_Mov,
                                                                          objParametriServer,
                                                                          codRisUm:=objMovimentoT.Cod_Risum,
                                                                          Cod_IndirizzoRisUm:=objMovimentoT.Cod_IndirizzoRisUm,
                                                                          docNumeroSin:=objMovimentoT.Doc_Numero_Sin,
                                                                          Doc_Numero:=objMovimentoT.Doc_Numero,
                                                                          docNumeroDes:=objMovimentoT.Doc_Numero_Des,
                                                                          docNumeroVisualizzato:=objMovimentoT.Doc_Numero_Visualizzato,
                                                                          dataMovimento:=objMovimentoT.Data,
                                                                          validitaInizio:=objMovimentoT.Data,
                                                                          dataRegistrazione:=objMovimentoT.Data_Registrazione,
                                                                          cauMov:=objMovimentoT.Cau_Mov,
                                                                          movDesc:=objMovimentoT.Mov_Desc,
                                                                          colli:=objMovimentoT.Colli,
                                                                          ora:=objMovimentoT.Ora,
                                                                          extraDate:=objMovimentoT.Extra_Date)
                                objMovimentoT = Nothing
                            End If

                            'Movimento Carico/Scarico
                            If Not IsNothing(objMovimentoC) Then
                                bOk = objMovimentiHelper.ModificaPuntuale(objMovimentoC.Piva, objMovimentoC.Sa_Cod,
                                                                          objMovimentoC.Id_Agenda, objMovimentoC.Id_Mov,
                                                                          objParametriServer,
                                                                          codRisUm:=objMovimentoC.Cod_Risum,
                                                                          Cod_IndirizzoRisUm:=objMovimentoC.Cod_IndirizzoRisUm,
                                                                          docNumeroSin:=objMovimentoC.Doc_Numero_Sin,
                                                                          Doc_Numero:=objMovimentoC.Doc_Numero,
                                                                          docNumeroDes:=objMovimentoC.Doc_Numero_Des,
                                                                          docNumeroVisualizzato:=objMovimentoC.Doc_Numero_Visualizzato,
                                                                          dataMovimento:=objMovimentoC.Data,
                                                                          validitaInizio:=objMovimentoC.Data,
                                                                          dataRegistrazione:=objMovimentoC.Data_Registrazione,
                                                                          cauMov:=objMovimentoC.Cau_Mov,
                                                                          movDesc:=objMovimentoC.Mov_Desc,
                                                                          colli:=objMovimentoC.Colli,
                                                                          Scadenza_Extra:=objMovimentoC.Scadenza_Extra,
                                                                          Scadenza:=objMovimentoC.Scadenza,
                                                                          ora:=objMovimentoC.Ora,
                                                                          extraDate:=objMovimentoC.Extra_Date)
                                objMovimentoC = Nothing
                            End If

                        End If


                    End If


                    'TODO: devo scrivere anche Agronica_Log_Agenda!!!
                    '(se scrive anche l'objAgenda, viene già scritta, ma se sto toccando solo le righe va scritta a mano)


                    'Prima di uscire devo aggiornare i conteggi totali e devo anche passarmi indietro i conteggi
                    If CInt(Dati(BF.Id_Agenda, indice)) > 0 Then
                        Dim conteggiTotali As Contabilita_Totali_Testata = Nothing
                        conteggiTotali = UtilityHelper.AggiornaConteggiTotali(Dati(BF.Piva, indice),
                                                                              Dati(BF.Lav_Cod, indice),
                                                                              Dati(BF.Id_Agenda, indice),
                                                                              objParametriServer,
                                                                              Dati(BF.Id_Mov_Testata, indice))

                        Dati(BF.Conteggi_Totali, indice) = JsonConvert.SerializeObject(conteggiTotali)
                    End If

                End If

            Next

            '------------------------------------------------
            '----- RIFERIMENTO OPERAZIONI DI AGENDA
            '------------------------------------------------
            If CInt(Dati(BF.Id_Agenda, 0)) <> 0 And CInt(Dati(BF.Id_Agenda, 1)) <> 0 And (id_agenda_carico = 0 Or id_agenda_scarico = 0) Then

                'Agende Create per la prima volta --> inserimento riferimento
                Dim objRiferimento = New Movimento_Dettaglio_Riferimento With {
                    .Piva = CStr(Dati(BF.Piva, 1)),
                    .Id_Agenda = CInt(Dati(BF.Id_Agenda, 1)),
                    .Lav_Cod = CInt(Dati(BF.Lav_Cod, 1)),
                    .Cau_Mov = CStr(Dati(BF.Cau_Mov, 1)),
                    .Piva_Rif = CStr(Dati(BF.Piva, 0)),
                    .Id_Agenda_Rif = CInt(Dati(BF.Id_Agenda, 0)),
                    .Lav_Cod_Rif = CInt(Dati(BF.Lav_Cod, 0)),
                    .Cau_Mov_Rif = CStr(Dati(BF.Cau_Mov, 0))
                }

                Dim objRiferimentiHelper As New Agenda_Movimenti_Dettagli_Riferimenti_Helper
                bOk = objRiferimentiHelper.Scrivi(objRiferimento, objParametriServer)

            End If




            'TODO: l'update della testata su db è da fare sia in cario che scarico, ma passarmi indietro i conteggi mi interessa solo per i carichi


            Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)


            'Impostazione Risultato
            Chiavi = CInt(Dati(BF.Id_Agenda, 0)) & "§" & CInt(Dati(BF.Id_Mov_Testata, 0)) & "§" & CInt(Dati(BF.Id_Mov_Magazzino, 0)) & "§" & Dati(BF.Conteggi_Totali, 0) & "§" &
                     CInt(Dati(BF.Id_Agenda, 1)) & "§" & CInt(Dati(BF.Id_Mov_Testata, 1)) & "§" & CInt(Dati(BF.Id_Mov_Magazzino, 1)) & "§" & Dati(BF.Conteggi_Totali, 1)


            r.RispostaStringa = Chiavi
            r.RispostaOK = True

        Catch ex As GiasException
            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            r.RispostaOK = False
            r.RispostaStringa = ex.Message
        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=False)
        Finally
            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)
        End Try

        Return r

    End Function

#End Region

#Region "Lettura e Salvataggio Testata documento"

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function LeggiTestataDocumento(ByVal objP_server As String,
                                                 ByVal objP_utenti As String,
                                                 ByVal piva As String,
                                                 ByVal saCod As Integer,
                                                 ByVal idAgenda As Integer,
                                                 ByVal lavCod As Integer
                                                 ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objContabTestata As Contabilita_Testata
        Dim msgError As String = ""

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        ElseIf objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objContabHelper As New ContabilitaHelper_Testata(objParametriServer, objParametriUtenti)
            objContabTestata = objContabHelper.LeggiTestataDocumento(piva, saCod, idAgenda, lavCod, msgError)

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

            r.RispostaStringa = JsonConvert.SerializeObject(objContabTestata, settingLoc)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function ModificaTestataDocumento(ByVal objP_server As String,
                                                    ByVal objP_utenti As String,
                                                    ByVal piva As String,
                                                    ByVal contabTestata As String,
                                                    ByVal messaggioDettagliato As Boolean
                                                    ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New Contabilita_Output

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        ElseIf objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            'La data che mi arriva da json è 2017-10-29T23:00:00.000Z che è equivalente a 
            '   Mon Oct 30 2017 00:00:00 GMT+0100 (ora solare Europa occidentale)
            '       e
            '   #10/30/2017 12:00:00 AM#

            Lingua.Gias_InizializzaCultura_DaSession()

            Dim objContabTestata As Contabilita_Testata = JsonConvert.DeserializeObject(contabTestata, (New Contabilita_Testata).GetType(), settingLoc)

            objContabTestata.Piva = piva

            Dim objContabHelper As New ContabilitaHelper_Testata(objParametriServer, objParametriUtenti)
            objOutput = objContabHelper.AggiornaTestataDocumento(objContabTestata, messaggioDettagliato)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc) 'If(xRisp = True, "OK", msgError)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = "Errore Modifica"
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function ScriviTestataDocumento(ByVal objP_server As String,
                                                  ByVal objP_utenti As String,
                                                  ByVal piva As String,
                                                  ByVal contabTestata As String,
                                                  ByVal messaggioDettagliato As Boolean
                                                  ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New Contabilita_Output

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        ElseIf objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Lingua.Gias_InizializzaCultura_DaSession()

            'La data che mi arriva da json è 2017-10-29T23:00:00.000Z che è equivalente a 
            '   Mon Oct 30 2017 00:00:00 GMT+0100 (ora solare Europa occidentale)
            '       e
            '   #10/30/2017 12:00:00 AM#

            Dim objContabTestata As Contabilita_Testata = JsonConvert.DeserializeObject(contabTestata, (New Contabilita_Testata).GetType(), settingLoc)

            objContabTestata.Piva = piva

            Dim objContabHelper As New ContabilitaHelper_Testata(objParametriServer, objParametriUtenti)
            objOutput = objContabHelper.ScriviTestataDocumento(objContabTestata, messaggioDettagliato)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = "Errore Scrittura"
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

#End Region

#Region "Salvataggio riga di documento"

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function SalvaSingolaRigaDocumento(ByVal objP_server As String,
                                                     ByVal objP_utenti As String,
                                                     ByVal contabDettaglio As String,
                                                     ByVal lavCod As Integer,
                                                     ByVal cauMov As String,
                                                     ByVal idMovT As Integer,
                                                     ByVal idMovCS As Integer,
                                                     ByVal dataOp As Date,
                                                     ByVal usernameOperatore As String,
                                                     ByVal messaggioDettagliato As Boolean,
                                                     ByVal contabTestata As String
                                                     ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New Contabilita_Output

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        ElseIf objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Lingua.Gias_InizializzaCultura_DaSession()

            'La data che mi arriva da json è 2017-10-29T23:00:00.000Z che è equivalente a 
            '   Mon Oct 30 2017 00:00:00 GMT+0100 (ora solare Europa occidentale)
            '       e
            '   #10/30/2017 12:00:00 AM#

            Dim objContabDettaglio As Contabilita_Riga = JsonConvert.DeserializeObject(Of Contabilita_Riga)(contabDettaglio, settingLoc)

            Dim objContabHelper As New ContabilitaHelper_Dettaglio(objParametriServer, objParametriUtenti)

            If objContabDettaglio.IdMovDet > 0 Then
                objOutput = objContabHelper.AggiornaSingolaRigaDocumento(objContabDettaglio,
                                                                         lavCod, cauMov,
                                                                         idMovT, idMovCS,
                                                                         dataOp, usernameOperatore,
                                                                         messaggioDettagliato)
            Else

                Dim objContabTestata As Contabilita_Testata = Nothing
                If Not contabTestata Is Nothing AndAlso contabTestata <> "" Then
                    objContabTestata = JsonConvert.DeserializeObject(Of Contabilita_Testata)(contabTestata, settingLoc)
                End If

                objOutput = objContabHelper.ScriviSingolaRigaDocumento(objContabDettaglio,
                                                                       lavCod, cauMov,
                                                                       idMovT, idMovCS,
                                                                       dataOp, usernameOperatore,
                                                                       messaggioDettagliato,
                                                                       objContabTestata)
            End If

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = "Errore Modifica"
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function SalvaTestataPiuRigaDocumento(ByVal objP_server As String,
                                                        ByVal objP_utenti As String,
                                                        ByVal contabTestata As String,
                                                        ByVal contabDettaglio As String,
                                                        ByVal cauMov As String,
                                                        ByVal idMovCS As Integer,
                                                        ByVal idMovDet As Integer,
                                                        ByVal messaggioDettagliato As Boolean
                                                        ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New Contabilita_Output

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        ElseIf objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Lingua.Gias_InizializzaCultura_DaSession()

            'La data che mi arriva da json è 2017-10-29T23:00:00.000Z che è equivalente a 
            '   Mon Oct 30 2017 00:00:00 GMT+0100 (ora solare Europa occidentale)
            '       e
            '   #10/30/2017 12:00:00 AM#

            Dim objContabTestata As Contabilita_Testata = JsonConvert.DeserializeObject(Of Contabilita_Testata)(contabTestata, settingLoc)

            Dim objContabDettaglio As Contabilita_Riga = Nothing
            If Not contabDettaglio Is Nothing AndAlso contabDettaglio <> "" Then
                objContabDettaglio = JsonConvert.DeserializeObject(Of Contabilita_Riga)(contabDettaglio, settingLoc)
            End If

            Dim objContabHelper As New ContabilitaHelper_Testata(objParametriServer, objParametriUtenti)

            objOutput = objContabHelper.SalvaTestataPiuRiga(objContabTestata,
                                                            cauMov,
                                                            idMovCS,
                                                            idMovDet,
                                                            messaggioDettagliato,
                                                            objContabDettaglio)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = AgronicaAgenda_2010.ErroreAggiornamento
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function CancellaSingolaRigaDocumento(ByVal objP_server As String,
                                                        ByVal objP_utenti As String,
                                                        ByVal contabDettaglio As String,
                                                        ByVal lavCod As Integer
                                                        ) As RispostaStandard
        '   ByVal cauMov As String,
        'ByVal idMovT As Integer,
        'ByVal idMovCS As Integer,
        'ByVal dataOp As Date,
        'ByVal usernameOperatore As String,
        'ByVal messaggioDettagliato As Boolean
        ') As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New Contabilita_Output

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        ElseIf objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            'La data che mi arriva da json è 2017-10-29T23:00:00.000Z che è equivalente a 
            '   Mon Oct 30 2017 00:00:00 GMT+0100 (ora solare Europa occidentale)
            '       e
            '   #10/30/2017 12:00:00 AM#

            Dim objContabDettaglio As Contabilita_Riga = JsonConvert.DeserializeObject(contabDettaglio, (New Contabilita_Riga).GetType(), settingLoc)

            Dim objContabHelper As New ContabilitaHelper_Dettaglio(objParametriServer, objParametriUtenti)

            objOutput = objContabHelper.CancellaSingolaRigaDocumento(objContabDettaglio,
                                                                     lavCod, "",
                                                                     0, 0,
                                                                     Now, objParametriServer.UsernameOperazione,
                                                                     True)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = "Errore Cancellazione"
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function AggiornaDettagliEconomici(ByVal contabDettaglio As String,
                                                     ByVal lavCod As Integer
                                                     ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

            Dim objContabDettaglio As Contabilita_Riga = JsonConvert.DeserializeObject(contabDettaglio, (New Contabilita_Riga).GetType(), settingLoc)

            UtilityHelper.AggiornaDettagliEconomici(objContabDettaglio, lavCod)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(objContabDettaglio, settingLoc)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Cancellazione Documento / Riga"

    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaEliminaModificaDocumento(ByVal piva As String,
                                                            ByVal Id_Agenda As Integer,
                                                            ByVal Id_Mov As Integer,
                                                            ByVal Id_Mov_Det As Integer,
                                                            ByVal Lav_Cod As Integer,
                                                            ByVal Tipo_Operazione As Integer,
                                                            ByVal IgnoraAvvisoWarning As Boolean,
                                                            ByVal ModuloGiasLicenziato As Integer,
                                                            ByVal flagAggiornaConteggi As Boolean,
                                                            ByVal dataMov As Date
                                                            ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim messaggio As String = ""
        Dim Modalita_Protetta As Integer = 0

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try

            Lingua.Gias_InizializzaCultura_DaSession()

            r = UtilityHelper.CancellaDocumento(objParametriServer,
                                                piva,
                                                Id_Agenda,
                                                Id_Mov,
                                                Id_Mov_Det,
                                                Lav_Cod,
                                                Tipo_Operazione,
                                                IgnoraAvvisoWarning,
                                                False,
                                                ModuloGiasLicenziato,
                                                flagAggiornaConteggi,
                                                messaggio,
                                                Modalita_Protetta,
                                                objParametriUtenti:=objParametriUtenti,
                                                DataMovimento:=dataMov)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
            r.RispostaStringa = ""
            r.RispostaConferma = False
        End Try

        Return r

    End Function

#End Region

#Region "Blocca / Sblocca Documento"

    <WebMethod(EnableSession:=True)>
    Public Shared Function BloccaDocumento(ByVal piva As String, ByVal saCod As Integer, ByVal idAgenda As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False
        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            xRisp = objAgenda.Agenda_Blocca(piva, saCod, idAgenda,
                                            objParametriServer.UsernameOperazione,
                                            Now, "", objParametriServer)

            r.RispostaOK = xRisp

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SbloccaDocumento(ByVal piva As String, ByVal saCod As Integer, ByVal idAgenda As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False
        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_W

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            xRisp = objAgenda.Agenda_Sblocca(piva, saCod, idAgenda,
                                             objParametriServer.UsernameOperazione,
                                             Now, "", objParametriServer)

            r.RispostaOK = xRisp

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Modifica Data Documento"

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function ModificaDataDocumento(ByVal objP_server As String,
                                                 ByVal objP_utenti As String,
                                                 ByVal piva As String,
                                                 ByVal idAgenda As Integer,
                                                 ByVal lavCod As Integer,
                                                 ByVal cauMov As String,
                                                 ByVal attualeDataDoc As Date,
                                                 ByVal newDataDoc As Date,
                                                 ByVal contabTestata As String
                                                 ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objOutput As New Contabilita_Output

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        ElseIf objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            'La data che mi arriva da json è 2017-10-29T23:00:00.000Z che è equivalente a 
            '   Mon Oct 30 2017 00:00:00 GMT+0100 (ora solare Europa occidentale)
            '       e
            '   #10/30/2017 12:00:00 AM#

            Dim objContabTestata As Contabilita_Testata = JsonConvert.DeserializeObject(Of Contabilita_Testata)(contabTestata, settingLoc)

            Dim objContabHelper As New ContabilitaHelper_Testata(objParametriServer, objParametriUtenti)
            objOutput = objContabHelper.ModificaDataDocPossibile(objContabTestata, piva, idAgenda, lavCod, cauMov, attualeDataDoc, newDataDoc)

            r.RispostaOK = objOutput.Risultato
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)

        Catch ex As Exception

            If objOutput.MsgError = "" Then
                objOutput.MsgError = "Errore Scrittura"
            End If

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(objOutput, settingLoc)
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function ModificaDistanzaDocumento(ByVal objP_server As String,
                                                     ByVal objP_utenti As String,
                                                     ByVal piva As String,
                                                     ByVal id_agenda As Integer,
                                                     ByVal udm_cod As Integer,
                                                     ByVal distanza As String,
                                                     ByVal valore As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim bOk As Boolean = False

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        ElseIf objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            If IsNumeric(udm_cod) And IsNumeric(distanza) And IsNumeric(valore) Then

                Dim objGHG As New AgronicaCoreContabDAL.GHG_Registrazioni_W
                bOk = objGHG.ModificaStandardFactor(piva, id_agenda, udm_cod, distanza, valore, objParametriServer)

            End If

            r.RispostaOK = bOk
            r.RispostaStringa = bOk

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function



#End Region

#Region "Ordini"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Ricerca_OrdiniFormProdottoUC(ByVal piva As String, ByVal cod_risum As Integer, ByVal lav_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Leggo i moduli installati
            Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim listModuli As List(Of Integer) = objO.Recupera_Moduli_Cliente(piva, objParametriServer)


            Dim lav_cod_ordine As Integer

            Dim lavCodDDTVendita As Integer() = {LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA}
            Dim lavCodDDTAcquisto As Integer() = {LAVCOD_BOLLA_RICEVUTA}
            Dim lavCodFattAcquisto As Integer() = {LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA}
            Dim lavCodFattVendita As Integer() = {LAVCOD_FATTURA_EMESSA}

            Dim filtro As New StringBuilder


            If lavCodDDTAcquisto.Contains(lav_cod) OrElse lavCodFattAcquisto.Contains(lav_cod) Then
                lav_cod_ordine = LAVCOD_ORDINE_ACQUISTO

                filtro.AppendLine("Movimenti_Dettagli.ordine_det <> 1000 AND Movimenti.cod_risum = " & cod_risum & " AND Agenda.lav_cod = " & lav_cod_ordine)

                If listModuli IsNot Nothing AndAlso listModuli.Count > 0 AndAlso
                               (listModuli.Contains(enum_Omni_Modulo_Generazione.FreshFood) OrElse
                               listModuli.Contains(enum_Omni_Modulo_Generazione.Tabacco) OrElse
                               listModuli.Contains(enum_Omni_Modulo_Generazione.Zoo)) Then

                    filtro.AppendLine("AND Movimenti_Dettagli.Elem_Cod NOT IN (" & TRASFORMATI_VEGETALI & "," & TRASFORMATI_ANIMALI & ")")

                End If

            ElseIf UtilityHelper.IsAccettazione(lav_cod) Then
                lav_cod_ordine = LAVCOD_ORDINE_ACQUISTO

                filtro.AppendLine("Movimenti_Dettagli.ordine_det <> 1000 AND Movimenti.cod_risum = " & cod_risum & " AND Agenda.lav_cod = " & lav_cod_ordine)
                filtro.AppendLine("AND Movimenti_Dettagli.Elem_Cod IN (" & TRASFORMATI_VEGETALI & "," & TRASFORMATI_ANIMALI & ")")

            ElseIf lavCodDDTVendita.Contains(lav_cod) OrElse lavCodFattVendita.Contains(lav_cod) Then
                lav_cod_ordine = LAVCOD_ORDINE_VENDITA

                'Nessun filtro particolare sugli elem_cod in vendita
                filtro.AppendLine("Movimenti_Dettagli.ordine_det <> 1000 AND Movimenti.cod_risum = " & cod_risum & " AND Agenda.lav_cod = " & lav_cod_ordine)

            End If


            ' Filtro contabilizzato
            'filtro.AppendLine(" AND Movimenti_Dettagli.ContabilizzatoNOT IN (3,-3) ")

            ' Filtro ordini da evadere
            'filtro.AppendLine(" AND (Movimenti_Dettagli.Id_Mov_Det NOT IN (")
            'filtro.AppendLine(" SELECT Id_Mov_Det_Rif From Mov_Dettagli_Riferimenti")
            'filtro.AppendLine(" WHERE (Lav_Cod_Rif = 2002 AND Lav_Cod = 1001) Or (Lav_Cod_Rif = 2002 AND Lav_Cod = 1031) Or (Lav_Cod_Rif = 2002 AND Lav_Cod = 1071))")
            'filtro.AppendLine(" OR Movimenti_Dettagli.Id_Mov_Det IN (")
            'filtro.AppendLine(" SELECT a.id_mov_det_rif FROM mov_dettagli_riferimenti a INNER JOIN movimenti_dettagli b ON a.id_mov_det_rif = b.id_mov_det")
            'filtro.AppendLine(" WHERE (Lav_Cod_Rif = 2002 AND Lav_Cod = 1001) Or (Lav_Cod_Rif = 2002 AND Lav_Cod = 1031) Or (Lav_Cod_Rif = 2002 AND Lav_Cod = 1071)")
            'filtro.AppendLine(" GROUP BY a.id_mov_det_rif HAVING sum(a.qta) < avg(b.qta) ))")

            'Questa impostazione, che ha significato quando la gestione del workflow con le pratiche è abilitata, significa che l'utente può collegare al ddt 
            'solo gli ordini già inviati al sistema informativo esterno, quindi se attiva leggo la pratica collegata all'ordine per avere l'informazione sullo stato
            Dim impreseImpostazioniR As New Imprese_Impostazioni_R
            Dim valImp = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser(piva, Nothing,
                                                                                              enum_Impostazioni_Utenti.Collega_Solo_Ordini_Inviati,
                                                                                              "0",
                                                                                              objParametriUtenti,
                                                                                              objParametriServer)
            Dim flagLeggiPrat As Boolean = valImp = "1"

            Dim objMagazzinoBIZ As New FF_MagazzinoBIZ
            r.RispostaStringa = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(piva, 0, "", 0, lav_cod_ordine, False, Nothing, objParametriServer, objParametriUtenti, filtro.ToString, "", ordiniDaEvadere:=True, leggiPratica:=flagLeggiPrat, contestoDocContabile:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Ricerca_DDTFormProdottoUC(ByVal piva As String, ByVal cod_risum As Integer, ByVal lav_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse
           IsNothing(objParametriServer) OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Lingua.Gias_InizializzaCultura_DaSession()

            'Leggo i moduli installati
            Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim listModuli As List(Of Integer) = objO.Recupera_Moduli_Cliente(piva, objParametriServer)


            Dim lav_cod_ddt As Integer

            'Dim lavCodDDTVendita As Integer() = {LAVCOD_DDT_CONTABILIZZATO_EMESSO, LAVCOD_BOLLA_EMESSA}
            'Dim lavCodDDTAcquisto As Integer() = {LAVCOD_BOLLA_RICEVUTA}
            Dim lavCodFattAcquisto As Integer() = {LAVCOD_FATTURA_RICEVUTA, LAVCOD_NOTA_ACCREDITO_RICEVUTA}
            Dim lavCodFattVendita As Integer() = {LAVCOD_FATTURA_EMESSA}

            Dim filtro As New StringBuilder

            If lavCodFattVendita.Contains(lav_cod) Then
                lav_cod_ddt = LAVCOD_BOLLA_EMESSA

                filtro.AppendLine("Movimenti_Dettagli.ordine_det <> 1000 AND Movimenti.cod_risum = " & cod_risum & " AND Agenda.lav_cod = " & lav_cod_ddt)

            ElseIf lavCodFattAcquisto.Contains(lav_cod) Then
                lav_cod_ddt = LAVCOD_BOLLA_RICEVUTA

                filtro.AppendLine("Movimenti_Dettagli.ordine_det <> 1000 AND Movimenti.cod_risum = " & cod_risum & " AND Agenda.lav_cod = " & lav_cod_ddt)

                If listModuli IsNot Nothing AndAlso listModuli.Count > 0 AndAlso
                               (listModuli.Contains(enum_Omni_Modulo_Generazione.FreshFood) OrElse
                               listModuli.Contains(enum_Omni_Modulo_Generazione.Tabacco) OrElse
                               listModuli.Contains(enum_Omni_Modulo_Generazione.Zoo)) Then

                    filtro.AppendLine("AND Movimenti_Dettagli.Elem_Cod NOT IN (" & TRASFORMATI_VEGETALI & "," & TRASFORMATI_ANIMALI & ")")

                End If
            End If

            Dim objMagazzinoBIZ As New AgronicaCoreContabBIZ.FF_MagazzinoBIZ
            r.RispostaStringa = objMagazzinoBIZ.Leggi_Movimenti_Carico_Scarico(
                piva,
                0,
                "",
                0,
                lav_cod_ddt,
                False,
                Nothing,
                objParametriServer,
                objParametriUtenti,
                filtro.ToString,
                "",
                ddtDaEvadere:=True, contestoDocContabile:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r


    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function ForzaEvasioneRigheOrdine(ByVal objP_server As String,
                                                    ByVal objP_utenti As String,
                                                    ByVal piva As String,
                                                    ByVal idAgenda As Integer,
                                                    ByVal listDettagli As List(Of Integer),
                                                    ByVal forzaEvasione As Boolean
                                                    ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objContabHelper As New ContabilitaHelper_Dettaglio(objParametriServer, objParametriUtenti)

            xRisp = objContabHelper.ForzaEvasioneRigheOrdine(piva, idAgenda, listDettagli, forzaEvasione, objParametriServer)

            r.RispostaOK = xRisp

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Script service Stampa Bar Code"

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaBarCode(ByVal piva As String,
                                         ByVal Id_Agenda As Integer,
                                         ByVal Lav_Cod As Integer,
                                         ByVal Mat_Cod As String,
                                         ByVal Cal_Cod As String,
                                         ByVal Lotto As String,
                                         ByVal Modulo As Integer,
                                         ByVal Tipo_Accettazione As Integer,
                                         ByVal Dettaglio As Boolean
                                         ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim permessiGruppiMerce As Boolean = ControllaPermessiVisibilitaGruppiMerce(objParametriServer, objParametriUtenti, piva, Id_Agenda, Lav_Cod)
        If permessiGruppiMerce = False Then
            r.RispostaOK = False
            r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabile.aspx", "DocNonStampabileGruppiMerce"), String)
            Return r
        End If

        Dim vVarStampe = New List(Of ElementoStampe) From {
            New ElementoStampe With {.Nome = "piva", .Valore = piva},
            New ElementoStampe With {.Nome = "id_agenda", .Valore = Id_Agenda},
            New ElementoStampe With {.Nome = "printcode", .Valore = Lav_Cod},
            New ElementoStampe With {.Nome = "printtoprinter", .Valore = 0},
            New ElementoStampe With {.Nome = "printname", .Valore = ""}
        }
        If Dettaglio Then
            Dim filtroStampa = String.Format("{0},{1},{2}", Mat_Cod, Lotto, Cal_Cod)
            vVarStampe.Add(New ElementoStampe With {.Nome = "attributo_jolly", .Valore = filtroStampa})
        End If

        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe.ToArray)
        Dim StrNodiVariabili As String = StrNodo
        Dim Report As Integer = enum_CodificaStampe.FF_Etichette

        If Report <> 0 Then

            Dim objAgronicaStampe As New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe With {
                .report = Report,
                .username = CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                .user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
            }
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

            r.RispostaStringa = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
            r.RispostaOK = True

        Else
            r.RispostaOK = False
            r.Errore = "Tipo di stampa non disponibile"
        End If

        Return r

    End Function


#End Region


#Region "Script Service per Stampa Documento"

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaDocumento(ByVal piva As String,
                                           ByVal Id_Agenda As Integer,
                                           ByVal Lav_Cod As Integer,
                                           ByVal Modulo As Integer,
                                           ByVal Tipo_Accettazione As Integer
                                           ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim permessiGruppiMerce As Boolean = ControllaPermessiVisibilitaGruppiMerce(objParametriServer, objParametriUtenti, piva, Id_Agenda, Lav_Cod)
        If permessiGruppiMerce = False Then
            r.RispostaOK = False
            r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabile.aspx", "DocNonStampabileGruppiMerce"), String)
            Return r
        End If

        Dim vVarStampe(4) As ElementoStampe
        vVarStampe(0).Nome = "piva"
        vVarStampe(0).Valore = piva
        vVarStampe(1).Nome = "id_agenda"
        vVarStampe(1).Valore = Id_Agenda
        vVarStampe(2).Nome = "printcode"
        vVarStampe(2).Valore = Lav_Cod
        vVarStampe(3).Nome = "printtoprinter"
        vVarStampe(3).Valore = 0
        vVarStampe(4).Nome = "printname"
        vVarStampe(4).Valore = ""

        Dim objVS As New AgronicaCoreXML.XML_Stampe
        Dim StrNodo As String = objVS.XML_VariabiliStampe(vVarStampe)
        Dim StrNodiVariabili As String = StrNodo
        Dim Report As Integer = 0

        Select Case Lav_Cod

            Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                Report = enum_CodificaStampe.DDT_Contabilizzato_Emesso

            Case LAVCOD_BOLLA_EMESSA, LAVCOD_BOLLA_RICEVUTA
                Report = enum_CodificaStampe.Bolle

            Case LAVCOD_FATTURA_EMESSA
                Report = enum_CodificaStampe.Fatture

            Case LAVCOD_NOTA_ACCREDITO_EMESSA
                Report = enum_CodificaStampe.Nota_Accredito

            Case LAVCOD_RICEVUTA_EMESSA
                Report = enum_CodificaStampe.RicevuteFiscali

            Case LAVCOD_ORDINE_VENDITA
                Report = enum_CodificaStampe.Ordine

            Case LAVCOD_ORDINE_ACQUISTO
                Report = enum_CodificaStampe.Ordine_Acquisto

            Case LAVCOD_ACCETTAZIONE_DIVERSI
                ' Accettazione DDT ricevuto
                If Tipo_Accettazione = -1 Then
                    'Report = enum_CodificaStampe.Certificato_Pomodoro 'Vecchia stampa (136)
                    Report = enum_CodificaStampe.Conf_Certificato_Pomodoro 'Nuova stampa (222)
                Else
                    Select Case Modulo
                        Case enum_Omni_Modulo_Generazione.Cantine
                            Report = enum_CodificaStampe.ConferimentoUva_DDTRicevuto
                        Case enum_Omni_Modulo_Generazione.FreshFood,
                            enum_Omni_Modulo_Generazione.Tabacco
                            Report = enum_CodificaStampe.FreshFood_BollaAccettazione

                        Case Else

                            'Visto che ora non viene più scritto sull'Agenda il modulo, vado a vedere quello che è installato
                            'TODO: da riguardare quando si gestiranno anche le Cantine se sta ancora in piedi

                            'Leggo i moduli installati
                            Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                            Dim listModuli As List(Of Integer) = objO.Recupera_Moduli_Cliente(piva, objParametriServer)

                            If listModuli IsNot Nothing AndAlso listModuli.Count > 0 AndAlso
                               (listModuli.Contains(enum_Omni_Modulo_Generazione.FreshFood) OrElse
                               listModuli.Contains(enum_Omni_Modulo_Generazione.Tabacco) OrElse
                               listModuli.Contains(enum_Omni_Modulo_Generazione.Zoo)) Then
                                Report = enum_CodificaStampe.FreshFood_BollaAccettazione
                            Else
                                Report = enum_CodificaStampe.Buono_Accettazione_Diversi
                            End If

                    End Select
                End If

            Case LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                Select Case Modulo
                    Case enum_Omni_Modulo_Generazione.Cantine
                        Report = enum_CodificaStampe.ConferimentoUva_AutoDDT
                    Case enum_Omni_Modulo_Generazione.FreshFood,
                        enum_Omni_Modulo_Generazione.Tabacco
                        Report = enum_CodificaStampe.FreshFood_AutoDDT_Accettazione
                    Case Else
                        ' Errore
                        Report = 0
                End Select

            Case LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
                Report = enum_CodificaStampe.FreshFood_DistintaCarico_Accettazione
        End Select

        If Report <> 0 Then

            Dim objAgronicaStampe As New ParametriAgronicaStampe With {
                .report = Report,
                .username = CStr(HttpContext.Current.Session("ASG_Utente_Username")),
                .user_profilo = CStr(HttpContext.Current.Session("ASG_ProgressivoGIAS"))
            }
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(StrNodiVariabili)

            r.RispostaStringa = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(Enum_SiteRedirector.Sito_AgronicaStampe_2010, objAgronicaStampe)
            r.RispostaOK = True

        Else
            r.RispostaOK = False
            r.Errore = "Tipo di stampa non disponibile"
        End If

        Return r

    End Function



#End Region

#Region "Verifica utilizzo Cal_Cod"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Verifica_Utilizzo_CalCod(ByVal piva As String,
                                                    ByVal id_mov_det As Integer,
                                                    ByVal cal_cod As Integer
                                                    ) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            r.RispostaStringa = ""
            Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim dt = objMovDet.Leggi(piva, 0,
                          0,
                          0,
                          0,
                          0,
                          0,
                          0,
                          "",
                          cal_cod,
                          0,
                          0,
                          0,
                          0,
                          0,
                          enumSelezioneVariabile.Selezione_JoinCompleta,
                          "Id_Mov_Det != " & CStr(id_mov_det) & " AND Lav_Cod != " & CostantiPersonalizzate.LAVCOD_RACCOLTA,
                          "", objParametriServer)

            If dt.Rows.Count > 0 Then
                Dim mov = ""
                For Each row In dt.Rows
                    If Not String.IsNullOrEmpty(mov) Then
                        mov += "|"
                    End If
                    mov += CStr(row.Item("Lav_cod")) & "_" & CStr(row.Item("Des_Lib")) & " - Data " & CDate(row.Item("Data_Movimento"))
                Next
                r.RispostaStringa = mov
            End If
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
#End Region

#Region "Conferimento_Pomodoro"
    <WebMethod(EnableSession:=True)>
    Public Shared Function Verifica_Contratto_Pomodoro(ByVal piva As String, ByVal cod_risum As Integer, ByVal elem_cod As Integer, ByVal mat_cod As Integer, ByVal data As Date) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            r.RispostaOK = True
            r.RispostaStringa = ""

            Dim leggi As New AgronicaCoreContabDAL.Imprese_Contratto_Fasi_R
            objParametriServer.ImpostaFinestre_con_SalvataggioTemporale(data, data)
            Dim dt = leggi.Leggi(piva, -1, 0, cod_risum, "", elem_cod, 0, mat_cod, 0, 0, "", 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametriServer)
            objParametriServer.ResettaFinestra()

            If dt.Rows.Count > 0 Then

                Dim Errore As String = ""
                Dim Prezzo As Decimal = 0
                Dim Listino_Cod As Integer = dt.Rows(0).Item("Listino_Cod")

                If mat_cod <> 0 Then
                    Dim campConf_R As New AgronicaCoreContabDAL.FF_CampionamentoConferimento_R
                    Errore = campConf_R.Leggi_Prezzi_Conferimento_Pomodoro(piva, Listino_Cod, elem_cod, mat_cod, 0, 0, data, objParametriServer, Prezzo)
                End If

                If Errore = "" Then

                    Dim contratto As New Contratto_Conferimento_Pomodoro With {
                        .Contratto_Cod = dt.Rows(0).Item("Contratto_Cod"),
                        .Fase_Cod = dt.Rows(0).Item("Fase_Cod"),
                        .ChkPremio = dt.Rows(0).Item("Valore1") = 1,
                        .Premio = CDec(dt.Rows(0).Item("Valore2")),
                        .Tetto = CDec(dt.Rows(0).Item("Valore3")),
                        .DataInizioPremio = dt.Rows(0).Item("Data_Inizio_Prevista"),
                        .DataFinePremio = dt.Rows(0).Item("Data_Fine_Prevista"),
                        .ChkPremio2 = If(dt.Rows(0).Item("Valore1_2") IsNot DBNull.Value AndAlso dt.Rows(0).Item("Valore1_2") = 1, True, False),
                        .Premio2 = CDec(If(dt.Rows(0).Item("Valore2_2") IsNot DBNull.Value, dt.Rows(0).Item("Valore2_2"), 0)),
                        .Tetto2 = CDec(If(dt.Rows(0).Item("Valore3_2") IsNot DBNull.Value, dt.Rows(0).Item("Valore3_2"), 0)),
                        .DataInizioPremio2 = If(dt.Rows(0).Item("Data_Inizio_Prevista2") IsNot DBNull.Value, dt.Rows(0).Item("Data_Inizio_Prevista2"), AGRODATAINIZIO),
                        .DataFinePremio2 = If(dt.Rows(0).Item("Data_Fine_Prevista2") IsNot DBNull.Value, dt.Rows(0).Item("Data_Fine_Prevista2"), AGRODATAFINE),
                        .TotaleDMA_MAX = CDec(dt.Rows(0).Item("Valore4")),
                        .Franchigia = CDec(dt.Rows(0).Item("Valore5")),
                        .TotaleDMI_MAX = CDec(dt.Rows(0).Item("Valore6")),
                        .Coefficiente = CDec(dt.Rows(0).Item("Valore7")),
                        .Abbattimento = CDec(dt.Rows(0).Item("Valore8")),
                        .Maggiorazione = CDec(dt.Rows(0).Item("Valore9")),
                        .Listino_Cod = dt.Rows(0).Item("Listino_Cod"),
                        .Prezzo = Prezzo
                    }


                    r.RispostaStringa = JsonConvert.SerializeObject(contratto, Formatting.None)
                    'r.RispostaStringa = JsonConvert.SerializeObject(New JObject(dt.Columns.Cast(Of DataColumn)().[Select](Function(c) New JProperty(c.ColumnName, JToken.FromObject(dt.Rows(0)(c))))).ToString(Formatting.None))

                Else

                    'Contratto o prezzo non trovato
                    r.RispostaOK = False
                    r.Errore = Errore

                End If

            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Calcola_Prezzo_Pomodoro(ByVal peso As Decimal, ByVal prezzo As Decimal, ByVal data As Date, ByVal parametri As String, ByVal contratto As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objConferimentoPomodoro As New FF_ConferimentoPomodoro
            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim parametriPomodoro = JsonConvert.DeserializeObject(parametri, (New Parametri_Conferimento_Pomodoro).GetType(), settingLoc)
            Dim contrattoPomodoro = JsonConvert.DeserializeObject(contratto, (New Contratto_Conferimento_Pomodoro).GetType(), settingLoc)
            Dim riepilogoPomodoro = objConferimentoPomodoro.CalcolaPrezzoPomodoro(peso, prezzo, data, parametriPomodoro, contrattoPomodoro)

            r.RispostaStringa = JsonConvert.SerializeObject(riepilogoPomodoro, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Riepilogo_Dati_Pomodoro(ByVal parametri As String, ByVal contratto As String, ByVal riepilogo As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objConferimentoPomodoro As New FF_ConferimentoPomodoro
            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim parametriPomodoro = JsonConvert.DeserializeObject(parametri, (New Parametri_Conferimento_Pomodoro).GetType(), settingLoc)
            Dim contrattoPomodoro = JsonConvert.DeserializeObject(contratto, (New Contratto_Conferimento_Pomodoro).GetType(), settingLoc)
            Dim riepilogoPomodoro = JsonConvert.DeserializeObject(riepilogo, (New Riepilogo_Conferimento_Pomodoro).GetType(), settingLoc)

            r.RispostaStringa = objConferimentoPomodoro.RiepilogoDatiPomodoro(parametriPomodoro, contrattoPomodoro, riepilogoPomodoro)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CodificaVarietaPomodoro() As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCodifica As New AgronicaCoreAnagrafeDAL.CAC_Codifica_Cultivar_R
            Dim dt = objCodifica.LeggiCodificaPomodoro("", 0, 52, 0, "", "", objParametriServer)

            Dim arrayVarieta As New JArray
            For Each row In dt.Rows
                Dim objVarieta As New JObject(
                    New JProperty("Cod_Varieta", CInt(row.Item("Cod_Varieta"))),
                    New JProperty("Desc_Varieta", row.Item("Desc_Varieta")))
                arrayVarieta.Add(objVarieta)
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(arrayVarieta)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "Aggiungi Macchina"

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiungiMacchinaDaTarga(ByVal piva As String,
                                                   ByVal saCod As Integer,
                                                   ByVal codContatto As String,
                                                   ByVal targa As String,
                                                   ByVal macDes As String
                                                   ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False
        Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_W
        Dim flagConnessione As Boolean = False
        Dim flagTransazione As Boolean = False

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Utility.VerificaApriTransazione(objParametriServer, flagConnessione, flagTransazione)

            Dim seq As New Agro_Sequenze()
            Dim macCodNew As Integer = seq.NuovoId_Tabella("Parco_Macchine", 0, 2000000000, objParametriServer)

            'Tipo = 2 (Commerciale)

            xRisp = objMacchine.Scrivi(piva, saCod, macCodNew, codContatto,
                                       Class_Code:="",
                                       Tipo:=2,
                                       Mac_Des:=macDes,
                                       Costo_Acquisto:=0,
                                       Targa:=targa,
                                       Telaio:="",
                                       Ditta_Cod:=0,
                                       Modello:="",
                                       Potenza:="",
                                       Ammortamento:=0,
                                       Data_Immatricolazione:=AGRODATAINIZIO,
                                       Ultima_Manutenzione:=AGRODATAINIZIO,
                                       Ultima_Revisione:=AGRODATAINIZIO,
                                       Stato_Utilizzo:="",
                                       Note:="",
                                       N_Immatricolazione:="",
                                       N_Immatricolazione_Rimorchio:="",
                                       N_Autorizzazione_Trasporto:="",
                                       Data_Rilascio_Autorizzazione:=AGRODATAINIZIO,
                                       Peso:=0,
                                       Portata_Max:=0,
                                       ChkDefault:=0,
                                       Alimentazione_Cod:=0,
                                       Potenza_Udm_Cod:=0,
                                       Mac_Cod_Origine:=0,
                                       Piva_SuperUser_Origine:="",
                                       CUAA_Proprietario:="",
                                       Denominazione_Proprietario:="",
                                       Tipo_Targa_Cod:=0,
                                       Tipo_Trazione_Cod:=0,
                                       N_Omologazione:="",
                                       Ditta_Cod_Motore:=0,
                                       Tipo_Motore:="",
                                       Matricola_Motore:="",
                                       Data_Reimmatricolazione:=AGRODATAFINE,
                                       Data_Carico:=AGRODATAINIZIO,
                                       Data_Scarico:=AGRODATAFINE,
                                       TitoloPossesso:=0,
                                       Flag_Attrezzatura_Macchina:="",
                                       Validita_Inizio:=AGRODATAINIZIO,
                                       Validita_Fine:=AGRODATAFINE,
                                       objParametri:=objParametriServer,
                                       Taratura_Ugello:=0,
                                       Data_Taratura_Inizio:=AGRODATAINIZIO,
                                       Data_Taratura_Fine:=AGRODATAFINE,
                                       Visibile_ctrl_gestione:=1,
                                       Codice:="")

            If xRisp = True Then
                Utility.VerificaChiudiTransazione(objParametriServer, flagTransazione)
                r.RispostaStringa = macCodNew
            Else
                Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            End If

            r.RispostaOK = xRisp

        Catch ex As Exception
            Utility.VerificaAnnullaTransazione(objParametriServer, flagTransazione)
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            Utility.VerificaChiudiConnessione(objParametriServer, flagConnessione)
        End Try

        Return r

    End Function

#End Region

#Region "Riepilogo Prezzi Listini"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Riepilogo_Prezzi_Listini(ByVal piva As String, ByVal tipo As Integer, ByVal data As Date, ByVal sa_cod As Integer, ByVal cod_risum As Integer, ByVal elem_cod As Integer, ByVal udm_cod As Integer, ByVal pro_cod As Integer, ByVal mat_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            ' Livello applicabilità listino
            Dim objImpostazioni As New Utenti_Impostazioni_Read
            Dim applica_listini = objImpostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_Livello_Applicazione_Listini, objParametriUtenti, 2)
            Dim livello As Integer = If(applica_listini = "", 0, CInt(applica_listini))

            Dim cod_rapporto As Integer = 0
            Dim cod_contatto As String = ""
            If cod_risum <> 0 Then
                Dim ru_R As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                Dim risorsa = ru_R.Leggi_RisorseUmane(cod_risum, objParametriServer)
                If risorsa IsNot Nothing Then
                    cod_rapporto = risorsa.Cod_Rapporto
                    cod_contatto = risorsa.Cod_Contatto
                End If
            End If

            Dim objListini As New AgronicaCoreContabDAL.Listini_Prezzi_Dettagli_R
            Dim strFiltro As String = "Listini_Prezzi.ChkApplicabilita > 0"
            Dim dtListini = objListini.LeggiPrezziListini(piva, tipo, 0, data, sa_cod, cod_rapporto, cod_contatto, 0, elem_cod, udm_cod, pro_cod, mat_cod, strFiltro, objParametriServer)

            If livello > 0 AndAlso dtListini.Rows.Count > 0 Then
                Dim dvListini = New DataView(dtListini)
                dvListini.RowFilter = "Livello >=" & livello
                dtListini = dvListini.ToTable
            End If

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dtListini, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

#Region "IVA"

    ''' <summary>
    ''' La funzione restituisce un dictionary contenente il codice IVA, il codice IVA Compensazione, 
    ''' il codice del conto economico di default, e il codice del conto patrimoniale di default
    ''' associati a queste etichette "Cod_Iva", "Cod_Iva_Compensazione", "Cod_Conto_Economico", "Cod_Conto_Patrimoniale"
    ''' </summary>
    <WebMethod(EnableSession:=True)>
    Public Shared Function IvaConti(ByVal piva As String,
                                    ByVal data_movimento As Date,
                                    ByVal cod_risum As Integer,
                                    ByVal elem_cod As Integer,
                                    ByVal pro_cod As Integer,
                                    ByVal mat_cod As Integer,
                                    ByVal cau_mov As String
                                    ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objContabImpo As New AgronicaCoreModello.Contabilita_Impostazioni
            objContabImpo.ValorizzaCodIvaDefault(objParametriUtenti)
            Dim defIva As String = objContabImpo.SUPERUSER_COD_IVA_DEFAULT

            Dim di As New Dictionary(Of String, Integer)
            Dim iva As New AgronicaCoreContabDAL.Contabilita_R
            di = iva.IvaConti(di, piva, data_movimento, cod_risum, elem_cod, pro_cod, mat_cod, cau_mov, defIva, objParametriServer)

            r.RispostaStringa = JsonConvert.SerializeObject(di)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

#End Region

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Alias_Prodotto(ByVal piva As String, ByVal mat_cod As Integer, ByVal cod_risum As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim xfiltroAggiuntivo As String = String.Format(
                "(Materie_Prime_Alias.Filtro_Contatti = '' OR " & vbCrLf &
                "Materie_Prime_Alias.Filtro_Contatti = '{0}' OR " & vbCrLf &
                "Materie_Prime_Alias.Filtro_Contatti LIKE '{0}|%' OR " & vbCrLf &
                "Materie_Prime_Alias.Filtro_Contatti LIKE '%|{0}|%' OR" & vbCrLf &
                "Materie_Prime_Alias.Filtro_Contatti LIKE '%|{0}')",
                cod_risum)

            Dim handleProd As New AgronicaCoreAnagrafeDAL.Materie_Prime_Alias_R
            Dim dt = handleProd.LeggiAliasxGrigliaAnagraficaProdotti(piva,
                                                                     mat_cod,
                                                                     0,
                                                                     Nothing,
                                                                     0,
                                                                     xfiltroAggiuntivo,
                                                                     "",
                                                                     objParametriServer,
                                                                     True)

            r.RispostaStringa = JsonConvert.SerializeObject(dt)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiImpostazioneImputazioneImpianti(ByVal piva As String, ByVal saCod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objImpreseImpostazioni As New Imprese_Impostazioni_R

            Dim creaRaccoltaAutomatica As String = objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(piva, New List(Of Integer)({saCod}), enum_Impostazioni_Utenti.Imputazione_Impianti_Raccolta_Conf, "", objParametriUtenti, objParametriServer)

            r.RispostaStringa = creaRaccoltaAutomatica
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiImpostazioneRaccolteConferimenti(ByVal piva As String, ByVal saCod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objImpreseImpostazioni As New Imprese_Impostazioni_R

            Dim collegaRaccolteConferimento As String = objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(piva, New List(Of Integer)({saCod}), enum_Impostazioni_Utenti.Conferimento_Da_Raccolta, "", objParametriUtenti, objParametriServer)

            r.RispostaStringa = collegaRaccolteConferimento
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaRaccolte(ByVal piva As String, ByVal dataMovimento As Date, ByVal idMovDet As Integer) As RispostaStandard
        Return RaccolteXConferimentiUC.CercaRaccolte(piva, dataMovimento, idMovDet)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiImpostazioneProponiPesoRaccolta(ByVal piva As String, ByVal saCod As Integer, ByVal vegCod As Integer) As RispostaStandard
        Return RaccolteXConferimentiUC.LeggiImpostazioneProponiPesoRaccolta(piva, saCod, vegCod)
    End Function


#Region "Scheduling_Documenti_Contabili"

    <WebMethod(EnableSession:=True)>
    Public Shared Function Scheduling_Documenti_Contabili(ByVal ID As Integer) As RispostaStandard

        Const nomeRoutine = "DocContabile_WS.Scheduling_Documenti_Contabili()"

        Dim r As New RispostaStandard

        Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        Dim Piva_SuperUser = objParametri_Server.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim strFiltroDocumenti As String = ""
        Dim strFiltroGruppi As String = ""

        Dim j As Integer = 0
        Dim i_Dettaglio As Integer = 0
        Dim i_Dettaglio_Gruppo As Integer = 0

        Dim Piva As String = ""
        Dim Rag_Soc_Contatto As String = ""
        Dim moduloGias As Integer = enum_Omni_Modulo_Generazione.Nessuno

        Dim Data_Emissione As DateTime
        Dim Data_Scadenza As DateTime
        Dim Id_Agenda_Base As Integer = 0
        Dim Id_Mov_Base As Integer = 0
        Dim Id_Mov_Det_Base As Integer = 0
        Dim Lav_Cod_Base As Integer = 0
        Dim Cau_Mov As String = ""
        Dim Id_Agenda_New As Integer
        Dim Qta_Residua As Decimal = 0
        Dim Cod_Contatto As String = ""
        Dim Cod_Risum As Integer = 0

        Dim Doc_Numero As Integer = 0
        Dim Doc_Numero_Sin As String = ""
        Dim Doc_Numero_Des As String = ""
        Dim Doc_Numero_Visualizzato As String = ""
        Dim Doc_Numero_Visualizzato_New As String = ""
        Dim Tipo_Enum_Documento As enum_FatturaTipo

        Dim Numero_Documenti_Creati As Integer = 0
        Dim Numero_Documenti_Creati_Totale As Integer = 0
        Dim Numero_Dettagli_Creati As Integer = 0

        Dim Progr_Protocollo As Integer = 0
        Dim Num_Registrazione As Integer = 0
        Dim Sezionale_Cod As Integer = 0
        Dim Anno As Integer = 0

        Dim Variazioni As Decimal = 0
        Dim Imposta As Decimal = 0
        Dim Importo As Decimal = 0
        Dim Imponibile_Netto As Decimal = 0
        Dim Imponibile_Lordo As Decimal = 0

        Dim Data_Inizio_Elaborazione As DateTime = AGRODATAINIZIO

        Dim Dummy As String = ""

        Dim Parametri_Rottura As String = ""
        Dim bSoggetto_Privato As Boolean = False
        Dim bSezionale As Boolean = False
        Dim bCessionario_Diverso As Boolean = False
        Dim Lav_Cod_Destinazione_Valido = True
        Dim strFiltro As String = ""
        Dim Num_Colli As Integer = 0
        Dim Peso As Decimal = 0
        Dim Lav_Cod_Destinazione As Integer = 0

        Dim dataModifica = Date.Now

        Dim ArrayContatti(0 To 5, 0 To 0) As String
        Dim ArrayGruppi(0 To 8, 0 To 0) As String

        Dim bDuplicato As Boolean = False

        Dim IndiceMov As Integer = 0
        Dim IndiceMov4000 As Integer = 0
        Dim Last_Id_Mov_Det As Integer = 0

        'Colonne Parametri Rottura
        Dim Col_Cod_Risum As Integer = 0
        Dim Col_Rag_Soc As Integer = 1
        Dim Col_Cau_Pagamento As Integer = 2
        Dim Col_Cod_IndirizzoRisUm As Integer = 3
        Dim Col_Cod_Destinazione As Integer = 4
        Dim Col_Id_Agenda As Integer = 5
        Dim Col_Sezionale As Integer = 6
        Dim Col_Elenco_Dettagli As Integer = 7
        Dim Col_Elenco_Agenda As Integer = 8

        Try
            'Creazione Oggetti
            Dim DT_Scheduling As New DataTable
            Dim DT_Contatti As New DataTable
            Dim DT_Contatto As New DataTable
            Dim DT_Riferimenti As New DataTable
            Dim DT_Dettagli As New DataTable
            Dim DTPagamenti As New DataTable
            Dim dr_search() As DataRow
            Dim Leggi_Scheduling As New AgronicaCoreContabDAL.Scheduling_Documenti_Contabili_R
            Dim Modifica_Scheduling As New AgronicaCoreContabDAL.Scheduling_Documenti_Contabili_W
            Dim Leggi_Contatti As New AgronicaCoreContabDAL.Movimenti_R
            Dim Leggi_Contatto As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim Leggi_Dettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
            Dim Leggi_Riferimenti As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
            Dim ObjContabilita As New AgronicaCoreContabDAL.Contabilita_R
            Dim objProgressivi As New AgronicaCoreDataProvider.Sequenza_Progressivi_R
            Dim objPagamenti As New AgronicaCoreContabDAL.Pagamenti_R


            Dim objAgendaHelper As New Agenda_Operazione_Helper
            Dim objMovimentiHelper As New Agenda_Movimenti_Helper
            Dim objMovimenti_DettagliHelper As New Agenda_Movimenti_Dettagli_Helper
            Dim objMovimenti_DestinazioniHelper As New Agenda_Movimenti_Destinazioni_Helper
            Dim utilityHelper As New AgronicaCoreModello.UtilityHelper
            Dim objPagamentiHelper As New Agenda_Pagamenti_Helper
            Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R

            Dim objAgenda As Operazione_Agenda
            Dim objMov As Movimento
            Dim objMovDets As List(Of Movimento_Dettaglio)
            Dim objMovDet As Movimento_Dettaglio
            Dim objMovDest As Movimento_Destinazione
            Dim objMovDetTec As Movimento_Dettaglio_Tecnico
            Dim objMovDetTecExtra As Movimento_Dettaglio_Tecnico_Extra


            objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO
            objParametri_Server.FinestraTemporaleFine = AGRODATAFINE

            Lingua.Gias_InizializzaCultura_DaSession()

            'Lettura DT
            DT_Scheduling = Leggi_Scheduling.Leggi(ID, "Data_Inizio_Elaborazione Is Null", "", objParametri_Server)

            'Controllo Presenza Processi
            If DT_Scheduling.Rows.Count > 0 Then

                'Ciclo sui Processi
                For Each dr_scheduling As DataRow In DT_Scheduling.Rows

                    Utility.VerificaApriConnessione(objParametri_Server, False)

                    Numero_Documenti_Creati = 0

                    'Impostazione Parametri
                    ID = dr_scheduling.Item("ID")
                    Lav_Cod_Destinazione = dr_scheduling.Item("Lav_Cod_Destinazione")
                    Data_Emissione = dr_scheduling.Item("Data")
                    Parametri_Rottura = dr_scheduling.Item("Parametri_Rottura")

                    bSoggetto_Privato = False
                    bSezionale = False
                    bCessionario_Diverso = False


                    'Costruzione di Filtro Impostato
                    If dr_scheduling.Item("Elenco_Id_Agenda") <> "" Then
                        strFiltroDocumenti = strFiltroDocumenti & IIf(Trim(strFiltroDocumenti = ""), "", " Or ") & " Movimenti.ID_Agenda In ( " & Replace(dr_scheduling.Item("Elenco_Id_Agenda"), "|", ",") & ")"
                    End If
                    If dr_scheduling.Item("Elenco_Id_Mov_Det") <> "" Then
                        strFiltroDocumenti = strFiltroDocumenti & IIf(Trim(strFiltroDocumenti = ""), "", " Or ") & " Movimenti_Dettagli.ID_Mov_Det In ( " & Replace(dr_scheduling.Item("Elenco_Id_Mov_Det"), "|", ",") & ")"
                    End If

                    If Trim(strFiltroDocumenti) = "" Then

                        'Nessun documento specificato --> lettura di tutti i documenti per lav_cod

                        Select Case Lav_Cod_Destinazione

                            Case LAVCOD_FATTURA_EMESSA ' 1001 'Fatture Emesse

                                strFiltroDocumenti = strFiltroDocumenti & IIf(Trim(strFiltroDocumenti = ""), "", " And ") & " ( Agenda.Lav_Cod In (1031, 1069) ) "
                                Lav_Cod_Destinazione_Valido = True

                            Case LAVCOD_FATTURA_RICEVUTA '1000 'Fatture Ricevute

                                strFiltroDocumenti = strFiltroDocumenti & IIf(Trim(strFiltroDocumenti = ""), "", " And ") & " ( Agenda.Lav_Cod In (1025) ) "
                                Lav_Cod_Destinazione_Valido = True

                            Case Else 'Lav_cod Non Valido

                                Lav_Cod_Destinazione_Valido = False

                        End Select

                    End If


                    If Lav_Cod_Destinazione_Valido Then

                        'Impostazione Parametri Punti di Rottura
                        Dim ParametriRotturaJS As JArray

                        If Parametri_Rottura <> "" And Parametri_Rottura <> "[]" Then

                            ParametriRotturaJS = JArray.Parse("[" & Parametri_Rottura & "]")

                            For Each obj As JObject In ParametriRotturaJS

                                'Impostazione Parametri
                                bSoggetto_Privato = CBool(obj("soggetti_privati"))
                                bSezionale = CBool(obj("sezionali"))
                                bCessionario_Diverso = CBool(obj("cessionari_diversi"))

                            Next

                        End If

                        'Lettura dei Contatti (Punto di Rottura Implicito)
                        DT_Contatti = Leggi_Contatti.LeggixScheduling_Documenti_Contabili("", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, strFiltroDocumenti, "", objParametri_Server)

                        'Ciclo sui Processi
                        For Each dr_contatti As DataRow In DT_Contatti.Rows

                            'Lettura Rag_Soc Contatto                            
                            DT_Contatto = Leggi_Contatto.Dati_Fatturazione_from_Cod_Risum(dr_contatti("Cod_Risum"), objParametri_Server)

                            ReDim Preserve ArrayContatti(0 To UBound(ArrayContatti, 1), 0 To UBound(ArrayContatti, 2) + 1)
                            ArrayContatti(0, UBound(ArrayContatti, 2) - 1) = dr_contatti("Cod_Risum")
                            ArrayContatti(1, UBound(ArrayContatti, 2) - 1) = DT_Contatto(0)("Modalita_Fatturazione")


                            If Trim(DT_Contatto(0)("Rag_Soc")) <> "" Then
                                ArrayContatti(2, UBound(ArrayContatti, 2) - 1) = DT_Contatto(0)("Rag_Soc")
                            Else
                                ArrayContatti(2, UBound(ArrayContatti, 2) - 1) = DT_Contatto(0)("Cognome") & " " & DT_Contatto(0)("Nome")
                            End If


                            'Controllo Soggetto Privato
                            If DT_Contatto(0)("Id_CF") <> 0 Or bSoggetto_Privato Then
                                ArrayContatti(3, UBound(ArrayContatti, 2) - 1) = 1
                            Else
                                ArrayContatti(3, UBound(ArrayContatti, 2) - 1) = 0 'Escluso poichè privato
                            End If

                            'Impostazione Cod_Contatto
                            Cod_Contatto = DT_Contatto(0)("Cod_Contatto")
                            Cod_Risum = dr_contatti("Cod_Risum")

                        Next
                        ArrayContatti(3, 0) = 1

                        For i = 0 To UBound(ArrayContatti, 2) - 1

                            'Controllo che il contatto non sia escluso
                            If ArrayContatti(3, i) = 1 Then

                                'Costruzione Filtro Gruppo
                                strFiltroGruppi = "(" & strFiltroDocumenti & ") And Cod_Risum = " & ArrayContatti(0, i)

                                'Lettura dei dettagli del gruppo 
                                DT_Dettagli = Leggi_Dettagli.Elenco_Dettagli_Scheduling_Documenti_Contabili("", "", strFiltroGruppi, "", objParametri_Server)

                                Last_Id_Mov_Det = 0
                                ReDim ArrayGruppi(0 To 9, 0 To 0)

                                'Controllo Presenza Processi
                                If DT_Dettagli.Rows.Count > 0 Then

                                    'Ciclo sui Dettagli 
                                    For Each dr_dettagli As DataRow In DT_Dettagli.Rows

                                        'Controllo Duplicati su Id_Mov_Det (se ci sono più tranche di pagamento il record è raddoppiato
                                        If Last_Id_Mov_Det = 0 Or Last_Id_Mov_Det <> dr_dettagli("Id_Mov_Det") Then

                                            Last_Id_Mov_Det = dr_dettagli("Id_Mov_Det")

                                            'Controllo Duplicato su Punti Rottura
                                            bDuplicato = False
                                            For j = 0 To UBound(ArrayGruppi, 2) - 1

                                                If CStr(ArrayGruppi(Col_Cau_Pagamento, j)) = CStr(dr_dettagli("Cau_Pagamento")) And
                                               CInt(ArrayGruppi(Col_Cod_IndirizzoRisUm, j)) = CInt(dr_dettagli("Cod_IndirizzoRisUm")) And
                                               (CInt(ArrayGruppi(Col_Cod_Destinazione, j)) = CInt(dr_dettagli("Cod_Destinazione")) Or (CInt(ArrayGruppi(Col_Cod_Destinazione, j)) <> CInt(dr_dettagli("Cod_Destinazione")) And Not bCessionario_Diverso)) And
                                               (CInt(ArrayGruppi(Col_Sezionale, j)) = CInt(dr_dettagli("Sezionale_Cod")) Or (CInt(ArrayGruppi(Col_Sezionale, j)) <> CInt(dr_dettagli("Sezionale_Cod")) And Not bSezionale)) And
                                               (CInt(ArrayGruppi(Col_Id_Agenda, j)) = CInt(dr_dettagli("Id_Agenda")) Or ArrayContatti(1, i) = 1) Then

                                                    bDuplicato = True
                                                    Exit For

                                                End If

                                            Next

                                            If Not bDuplicato Then

                                                'Inserimento in Gruppo Processi
                                                ReDim Preserve ArrayGruppi(0 To UBound(ArrayGruppi, 1), 0 To UBound(ArrayGruppi, 2) + 1)
                                                ArrayGruppi(Col_Cod_Risum, UBound(ArrayGruppi, 2) - 1) = ArrayContatti(0, i)
                                                ArrayGruppi(Col_Rag_Soc, UBound(ArrayGruppi, 2) - 1) = ArrayContatti(2, i)
                                                ArrayGruppi(Col_Cau_Pagamento, UBound(ArrayGruppi, 2) - 1) = dr_dettagli("Cau_Pagamento")
                                                ArrayGruppi(Col_Cod_IndirizzoRisUm, UBound(ArrayGruppi, 2) - 1) = dr_dettagli("Cod_IndirizzoRisUm")
                                                ArrayGruppi(Col_Cod_Destinazione, UBound(ArrayGruppi, 2) - 1) = dr_dettagli("Cod_Destinazione")
                                                ArrayGruppi(Col_Id_Agenda, UBound(ArrayGruppi, 2) - 1) = dr_dettagli("Id_Agenda")
                                                ArrayGruppi(Col_Sezionale, UBound(ArrayGruppi, 2) - 1) = dr_dettagli("Sezionale_Cod")

                                                'Costruzione Dettagli per Documento
                                                ArrayGruppi(Col_Elenco_Dettagli, UBound(ArrayGruppi, 2) - 1) = dr_dettagli("Id_Mov_Det")

                                                'Costruzione Agenda per Documento
                                                ArrayGruppi(Col_Elenco_Agenda, UBound(ArrayGruppi, 2) - 1) = dr_dettagli("Id_Agenda")


                                            Else

                                                'Costruzione Dettagli per Documento
                                                ArrayGruppi(Col_Elenco_Dettagli, j) = ArrayGruppi(Col_Elenco_Dettagli, j) & ", " & dr_dettagli("Id_Mov_Det")

                                                'Costruzione Agenda per Documento 
                                                ArrayGruppi(Col_Elenco_Agenda, j) = ArrayGruppi(Col_Elenco_Agenda, j) & ", " & dr_dettagli("Id_Agenda")


                                            End If



                                        End If

                                    Next

                                End If

                            End If


                            'Scorro i gruppi
                            If DT_Dettagli.Rows.Count > 0 And UBound(ArrayGruppi, 2) > 0 Then

                                For j = 0 To UBound(ArrayGruppi, 2) - 1

                                    Rag_Soc_Contatto = ArrayGruppi(Col_Rag_Soc, j)

                                    'Nota: è importante filtrare per cuasale perché in presenza di più tranche i dettagli sono duplicati
                                    strFiltroGruppi = "Id_Mov_Det In (" & ArrayGruppi(Col_Elenco_Dettagli, j) & ") And Cau_Pagamento = '" & ArrayGruppi(Col_Cau_Pagamento, j) & "'"

                                    dr_search = DT_Dettagli.Select(strFiltroGruppi)

                                    'Prendo la prima agenda/movimenti come riferimento
                                    Piva = dr_search(0).Item("Piva")
                                    Doc_Numero = 0
                                    Data_Inizio_Elaborazione = Date.Now
                                    Numero_Dettagli_Creati = 0
                                    Importo = 0
                                    Progr_Protocollo = 0
                                    Num_Registrazione = 0
                                    Sezionale_Cod = 0
                                    Anno = 0
                                    Num_Colli = 0
                                    Peso = 0

                                    'Imposto l'id_agenda base                                
                                    Id_Agenda_Base = dr_search(0).Item("Id_Agenda")
                                    Lav_Cod_Base = dr_search(0).Item("Lav_Cod")

                                    'Lettura Id_Agenda
                                    objAgenda = objAgendaHelper.Leggi(Piva, 0, Id_Agenda_Base, 0, objParametri_Server)

                                    'Lettura modulo cliente
                                    Dim listModuli As List(Of Integer) = objO.Recupera_Moduli_Cliente(Piva, objParametri_Server)
                                    moduloGias = If(listModuli.Count > 0, listModuli(0), enum_Omni_Modulo_Generazione.Nessuno)

                                    'AGENDA
                                    objAgenda.Id_Agenda = 0
                                    objAgenda.Lav_Cod = Lav_Cod_Destinazione
                                    objAgenda.Data = Data_Emissione
                                    objAgenda.Data_Creazione = Date.Now

                                    IndiceMov = 0

                                    'MOVIMENTI

                                    For Each objMov In objAgenda.Movimenti

                                        'Mi serve per capire pe riconoscere il 4000 per aggancio pagamenti
                                        IndiceMov = IndiceMov + 1

                                        objMov.Id_Agenda = 0
                                        objMov.Id_Mov = 0

                                        objMov.Data = Data_Emissione
                                        objMov.Ora = Data_Emissione
                                        objMov.Data_Registrazione = Data_Emissione

                                        objMov.TipoDocumento = 24 'Differita

                                        objMov.Lav_Cod = Lav_Cod_Destinazione

                                        objMov.Doc_Numero_Sin = ""
                                        objMov.Doc_Numero_Des = ""
                                        Doc_Numero_Visualizzato = objMov.Doc_Numero_Visualizzato

                                        'Determinazione Prefisso/Suffisso
                                        Dim ObjNumeratori As New AgronicaCoreContabBIZ.Documento_Default_BIZ
                                        Dim Numeratori As New NumeratoriDefaults

                                        'Ricavo il Sa_Cod
                                        Dim Sa_Cod_Dettaglio As Integer = 0
                                        For Each objMov2 In objAgenda.Movimenti
                                            If objMov2.Movimenti_Dettagli.Count > 0 Then
                                                For Each objMovDet2 In objMov2.Movimenti_Dettagli

                                                    Sa_Cod_Dettaglio = objMovDet2.Sa_Cod

                                                Next
                                            End If
                                        Next



                                        Numeratori = ObjNumeratori.NumeratoriEDefaults(Data_Emissione, Piva, Sa_Cod_Dettaglio, Lav_Cod_Destinazione, False, objParametri_Server)

                                        If Numeratori.Numeratori.Count > 0 Then

                                            Doc_Numero_Sin = Numeratori.Numeratori(0).Doc_Numero_Sin ' objMov.Doc_Numero_Sin
                                            Doc_Numero_Des = Numeratori.Numeratori(0).Doc_Numero_Des 'objMov.Doc_Numero_Des

                                            objMov.Doc_Numero_Sin = Doc_Numero_Sin
                                            objMov.Doc_Numero_Des = Doc_Numero_Des

                                        End If

                                        'Movimenti Tecnici
                                        For Each objMovDetTec In objMov.Movimenti_Dettagli_Tecnici

                                            objMovDetTec.Id_Agenda = 0
                                            objMovDetTec.Id_Mov = 0
                                            objMovDetTec.Id_Mov_Det = 0
                                            objMovDetTec.Id_Reg_Dettaglio = 0

                                        Next

                                        'Movimenti Tecnici Extra
                                        For Each objMovDetTecExtra In objMov.Movimenti_Dettagli_Tecnici_Extra

                                            objMovDetTecExtra.Id_Agenda = 0
                                            objMovDetTecExtra.Id_Mov = 0
                                            objMovDetTecExtra.Id_Mov_Det = 0
                                            objMovDetTecExtra.Id_Reg_Dettaglio = 0

                                        Next

                                        'Per ora il pagamento viene ricreato da zero
                                        objMov.Pagamenti.Clear()

                                        'Pulizia Dettagli
                                        objMov.Movimenti_Dettagli.Clear()

                                        Anno = Year(objMov.Data)
                                        Sezionale_Cod = objMov.Sezionale_Cod

                                        'Numero Protocollo
                                        If Progr_Protocollo = 0 Then
                                            Progr_Protocollo = ObjContabilita.LastProgressivoVendite(Piva, Anno, Sezionale_Cod, 0, objParametri_Server) + 1
                                        End If

                                        objMov.Progr_Protocollo = Progr_Protocollo

                                        'Progressivo Registrazione
                                        If Num_Registrazione = 0 Then
                                            Num_Registrazione = objProgressivi.Nuovo_ProgressivoValBase(Piva, Anno, enum_SequenzaProgressiviTipi.NumeroRegistrazioneOperazioniContabili, Doc_Numero_Sin, Doc_Numero_Des, Sezionale_Cod, 1, objParametri_Server)
                                        End If
                                        objMov.Progr_Registrazione = Num_Registrazione

                                        Select Case objMov.Cau_Mov

                                            Case CAU_REGISTRAZIONI

                                                'Aggiornamento Scadenza
                                                objMov.Extra_Date = AGRODATAFINE
                                                objMov.ChkLayOut_Prezzo = 1
                                                objMov.Extra_Int = CInt(enum_FatturaTipo.Differita)

                                                'Salvo l'indice 4000 in modo da collegare il pagamento
                                                IndiceMov4000 = IndiceMov - 1


                                            Case CAU_CARICO, CAU_SCARICO

                                                'Aggancio Dettagli
                                                For i_Dettaglio_Gruppo = 0 To dr_search.Count - 1

                                                    'Lettura Dettaglio
                                                    objMovDets = objMovimenti_DettagliHelper.Leggi(Piva, 0, 0, 0, CInt(dr_search(i_Dettaglio_Gruppo)("ID_Mov_Det")), objParametri_Server)

                                                    Cau_Mov = objMov.Cau_Mov

                                                    For i_Dettaglio = 0 To objMovDets.Count - 1

                                                        objMovDet = objMovDets(i_Dettaglio)

                                                        Id_Agenda_Base = objMovDet.Id_Agenda
                                                        Id_Mov_Base = objMovDet.Id_Mov
                                                        Id_Mov_Det_Base = CInt(dr_search(i_Dettaglio_Gruppo)("ID_Mov_Det"))


                                                        'MOVIMENTI_DETTAGLI

                                                        objMovDet.Id_Agenda = 0
                                                        objMovDet.Id_Mov = 0
                                                        objMovDet.Id_Mov_Det = 0

                                                        objMovDet.Lav_Cod = Lav_Cod_Destinazione

                                                        'Sistemazioni Dedicae per Lav_Cod_Destinazione
                                                        Select Case Lav_Cod_Destinazione

                                                            Case LAVCOD_FATTURA_EMESSA

                                                                'Pendenza
                                                                Select Case Lav_Cod_Base

                                                                    Case LAVCOD_BOLLA_EMESSA

                                                                        objMovDet.Pendente = enum_Pendenza.DocBolla

                                                                        objMovDet.Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato
                                                                        Cau_Mov = CAU_SCARICO
                                                                        Tipo_Enum_Documento = enum_FatturaTipo.Differita


                                                                    Case LAVCOD_ORDINE_VENDITA

                                                                        objMovDet.Pendente = enum_Pendenza.DocOrdine

                                                                        objMovDet.Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato
                                                                        Cau_Mov = CAU_SCARICO
                                                                        Tipo_Enum_Documento = enum_FatturaTipo.Differita

                                                                    Case Else

                                                                        objMovDet.Pendente = enum_Pendenza.DocBolla

                                                                        objMovDet.Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato
                                                                        Cau_Mov = CAU_SCARICO
                                                                        Tipo_Enum_Documento = enum_FatturaTipo.Differita

                                                                        'TODO

                                                                End Select


                                                            Case LAVCOD_FATTURA_RICEVUTA

                                                                Select Case Lav_Cod_Base

                                                                    Case LAVCOD_BOLLA_RICEVUTA

                                                                        objMovDet.Pendente = enum_Pendenza.DocBolla

                                                                        objMovDet.Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato
                                                                        Cau_Mov = CAU_CARICO
                                                                        Tipo_Enum_Documento = enum_FatturaTipo.Differita

                                                                    Case LAVCOD_ORDINE_ACQUISTO

                                                                        objMovDet.Pendente = enum_Pendenza.DocOrdine

                                                                        objMovDet.Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato
                                                                        Cau_Mov = CAU_SCARICO
                                                                        Tipo_Enum_Documento = enum_FatturaTipo.Differita

                                                                    Case Else

                                                                        objMovDet.Pendente = enum_Pendenza.DocBolla

                                                                        objMovDet.Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato
                                                                        Cau_Mov = CAU_CARICO
                                                                        Tipo_Enum_Documento = enum_FatturaTipo.Differita

                                                                        'TODO

                                                                End Select


                                                            Case Else

                                                                'Todo
                                                                objMovDet.Jolly_Int = enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato
                                                                Cau_Mov = CAU_SCARICO
                                                                Tipo_Enum_Documento = enum_FatturaTipo.Differita

                                                        End Select


                                                        If objMovDet.Elem_Cod = 501 Then
                                                            Dummy = ""
                                                        End If

                                                        'Determinazione Qta
                                                        Qta_Residua = objMovDet.Qta

                                                        '==============================================================================================================================================================================
                                                        'Lettura dei riferimenti di agenda
                                                        '------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                                        DT_Riferimenti = Leggi_Riferimenti.MovimentiRiferimenti_LeggiBilaterale_Completa(Piva, 0, Id_Agenda_Base, Id_Mov_Base, Id_Mov_Det_Base, Lav_Cod_Base, CAU_REGISTRAZIONI, "", Piva, 0, 0, 0, 0, Lav_Cod_Destinazione, "", "", "", objParametri_Server)

                                                        If DT_Riferimenti.Rows.Count > 0 Then

                                                            'Ciclo sui Riferimenti
                                                            For Each dr_riferimenti As DataRow In DT_Riferimenti.Rows

                                                                Qta_Residua = Qta_Residua - dr_riferimenti("Qta")

                                                            Next

                                                        End If

                                                        'Controllo Saturazione della Qta
                                                        If Qta_Residua > 0 Then

                                                            'Aggiornamento Qta
                                                            objMovDet.Qta = Qta_Residua

                                                            Dim objContabHlp As New AgronicaCoreContabHLP.Contabilita()

                                                            'Movimenti Tecnici Extra
                                                            For Each objMovDetTecExtra In objMovDet.Movimenti_Dettagli_Tecnici_Extra

                                                                'È sufficiente che una delle seguenti colonne sia diversa dal suo default per considerare abilitate le quantità riscontrate
                                                                If objMovDetTecExtra.Peso_Lordo_Riscontrato <> 0 OrElse
                                                                    objMovDetTecExtra.Peso_Netto_Riscontrato <> 0 OrElse
                                                                    objMovDetTecExtra.Num_Imballi_Riscontrati <> -1 OrElse
                                                                    objMovDetTecExtra.Tara_Unit_Imballo_Riscontrata <> -1 OrElse
                                                                    objMovDetTecExtra.Num_Colli_Riscontrati <> -1 OrElse
                                                                    objMovDetTecExtra.Tara_Unit_Collo_Riscontrata <> -1 OrElse
                                                                    objMovDetTecExtra.Num_Conf_Riscontrate <> -1 OrElse
                                                                    objMovDetTecExtra.Tara_Unit_Conf_Riscontrata <> -1 Then

                                                                    'Devo prendere i valori riscontrati e trattarli come reali dal punto di vista della fattura
                                                                    Dim w_imponibile As Decimal = objContabHlp.Leggi_Imponibile_PositivoNegativo(Lav_Cod_Destinazione, objMovDet.Imponibile)
                                                                    Dim w_imponibileNetto As Decimal = objContabHlp.Leggi_Imponibile_PositivoNegativo(Lav_Cod_Destinazione, objMovDet.Imponibile_Netto)
                                                                    Dim w_iva As Decimal = objContabHlp.Leggi_IVA_PositivaNegativa(Lav_Cod_Destinazione, objMovDet.Iva)

                                                                    Dim scontoAddiz1 As Decimal = 0
                                                                    Dim scontoAddiz2 As Decimal = 0
                                                                    Dim scontoAddiz3 As Decimal = 0
                                                                    If objMovDet.Sconto_Testo IsNot Nothing AndAlso objMovDet.Sconto_Testo <> "" Then
                                                                        Dim sconti As String() = objMovDet.Sconto_Testo.Split("-")
                                                                        If sconti.Length = 3 Then
                                                                            scontoAddiz3 = sconti(2)
                                                                            scontoAddiz2 = sconti(1)
                                                                            scontoAddiz1 = sconti(0)
                                                                        End If
                                                                        If sconti.Length = 2 Then
                                                                            scontoAddiz2 = sconti(1)
                                                                            scontoAddiz1 = sconti(0)
                                                                        End If
                                                                        If sconti.Length = 1 Then
                                                                            scontoAddiz1 = sconti(0)
                                                                        End If
                                                                    End If

                                                                    Dim scontoCalcolato As Decimal = Agro_Math.ArrotondaVal_6(AgronicaCoreContabHLP.Contabilita.CalcolaScontoComplessivo(
                                                                    Math.Abs(objMovDet.Sconto),
                                                                    scontoAddiz1,
                                                                    scontoAddiz2,
                                                                    scontoAddiz3))

                                                                    Dim objIvaAliquote As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R()
                                                                    Dim aliquotaIva As Decimal = objIvaAliquote.AliquotaFloat_from_CodIVA(objMovDet.Cod_Iva, "", objParametri_Server)

                                                                    'Preparo l'oggetto per richiamare la funzione UtilityHelper.AggiornaDettagliEconomici
                                                                    Dim contabRiga As New Contabilita_Riga With {
                                                                    .ModuloGias = moduloGias,
                                                                    .ElemCod = objMovDet.Elem_Cod,
                                                                    .UdM = objMovDet.Udm_Cod,
                                                                    .Udm_Cod_Extra = objMovDet.Udm_Cod_Extra,
                                                                    .Qta_Extra = objMovDet.Qta_Extra,
                                                                    .Tara = objMovDetTecExtra.Peso_Lordo_Riscontrato - objMovDetTecExtra.Peso_Netto_Riscontrato,
                                                                    .ValoreRiferimentoPrezzo = objMovDet.TempoCarenza,
                                                                    .PrezzoRiferitoA = objMovDet.Prezzo_Livello,
                                                                    .Quantita = objMovDetTecExtra.Peso_Netto_Riscontrato, 'Sia kg F&F che altre udm
                                                                    .NumImballi = objMovDetTecExtra.Num_Imballi_Riscontrati,
                                                                    .NumContenitori = objMovDetTecExtra.Num_Colli_Riscontrati,
                                                                    .NumConfezioni = objMovDetTecExtra.Num_Conf_Riscontrate,
                                                                    .KgNetti = objMovDetTecExtra.Peso_Netto_Riscontrato, 'Sia kg F&F che altre udm
                                                                    .Degrado = objMovDet.Variazione,
                                                                    .Prezzo = objMovDet.Prezzo_Unitario,
                                                                    .PrezzoNetto = objMovDet.Prezzo_Unitario_Netto,
                                                                    .PrezzoEffettivoKgL = objMovDet.Prezzo_Effettivo,
                                                                    .ScontoModalita = objMovDet.Sconto_Modalita,
                                                                    .ScontoBase = Math.Abs(objMovDet.Sconto),
                                                                    .MaggiorazioneBase = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .ScontoMaggiorazioneBase = Math.Abs(objMovDet.Sconto),
                                                                    .ScontoBaseEuro = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .ScontoAddiz1 = scontoAddiz1,
                                                                    .ScontoAddiz2 = scontoAddiz2,
                                                                    .ScontoAddiz3 = scontoAddiz3,
                                                                    .ScontoCalcolato = scontoCalcolato,
                                                                    .ScontoAddizTotalePerc = objMovDet.Sconto_Listino,
                                                                    .ScontoAddizTotaleEuro = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .ForzaIva = False,
                                                                    .CodIva = objMovDet.Cod_Iva,
                                                                    .Iva = w_iva,
                                                                    .AliquotaIva = aliquotaIva,
                                                                    .IvaModalita = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .IvaIndetraibilePerc = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .IvaIndetraibile = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .IvaCreditoDebito = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .ImponibileTotale = w_imponibile,
                                                                    .ImponibileTotaleNetto = w_imponibileNetto,
                                                                    .ImportoUnitario = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .ImportoTotale = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .ProvvigionePercAgente = objMovDetTecExtra.Provvigione,
                                                                    .ProvvigioneAgente = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .ProvvigionePercCapoArea = objMovDetTecExtra.Provvigione_CapoArea,
                                                                    .ProvvigioneCapoArea = 0, 'Inizializzo a vuota perché non usata in scrittura
                                                                    .EsigibilitaIva = 0 'Inizializzo a vuota perché non usata in scrittura
                                                                }

                                                                    UtilityHelper.AggiornaDettagliEconomici(contabRiga, Lav_Cod_Destinazione)
                                                                    ContabilitaHelper_Dettaglio.SistemaValoriRigaPerScrittura(contabRiga, Lav_Cod_Destinazione)

                                                                    'Una volta ricalcolati i dettagli economici, imposto i nuovi valori sugli oggetti che verranno scritti
                                                                    objMovDet.Udm_Cod = contabRiga.DB_Udm_Cod
                                                                    objMovDet.Udm_Cod_Extra = contabRiga.DB_Udm_Cod_Extra
                                                                    objMovDet.Qta = contabRiga.DB_Qta
                                                                    objMovDet.Qta_Extra = contabRiga.DB_Qta_Extra
                                                                    objMovDet.Qta_Extra_Totale = contabRiga.KgNetti
                                                                    objMovDet.Tara = contabRiga.Tara
                                                                    'objMovDet.Jolly_Int = contabRiga.jolly_int
                                                                    'objMovDet.Cod_Iva = contabRiga.CodIva
                                                                    objMovDet.Iva = contabRiga.Iva
                                                                    'objMovDet.ChkIva_Manuale ?
                                                                    'objMovDet.Variazione = contabRiga.Degrado
                                                                    objMovDet.Prezzo_Unitario = contabRiga.DB_Prezzo_Unitario
                                                                    objMovDet.Prezzo_Unitario_Netto = contabRiga.DB_Prezzo_Unitario_Netto
                                                                    objMovDet.Prezzo_Effettivo = contabRiga.PrezzoEffettivoKgL
                                                                    objMovDet.Prezzo_Livello = contabRiga.PrezzoRiferitoA
                                                                    objMovDet.TempoCarenza = contabRiga.ValoreRiferimentoPrezzo
                                                                    'objMovDet.Sconto_Modalita = contabRiga.ScontoModalita
                                                                    'objMovDet.Sconto = contabRiga.ScontoMaggiorazioneBase
                                                                    'objMovDet.Sconto_Listino = contabRiga.ScontoAddizTotalePerc
                                                                    'objMovDet.Sconto_Testo = contabRiga.ScontoAddizTesto
                                                                    objMovDet.Imponibile = contabRiga.ImponibileTotale
                                                                    objMovDet.Imponibile_Netto = contabRiga.ImponibileTotaleNetto
                                                                    objMovDet.Qta_Dettaglio1 = contabRiga.NumContenitori
                                                                    objMovDet.Qta_Dettaglio2 = contabRiga.NumImballi

                                                                    objMovDetTecExtra.Peso_Lordo_Riscontrato = 0
                                                                    objMovDetTecExtra.Peso_Netto_Riscontrato = 0
                                                                    objMovDetTecExtra.Num_Imballi_Riscontrati = -1
                                                                    'objMovDetTecExtra.Tara_Unit_Imballo_Riscontrata = -1
                                                                    objMovDetTecExtra.Num_Colli_Riscontrati = -1
                                                                    'objMovDetTecExtra.Tara_Unit_Collo_Riscontrata = -1
                                                                    objMovDetTecExtra.Num_Conf_Riscontrate = -1
                                                                    'objMovDetTecExtra.Tara_Unit_Conf_Riscontrata = -1

                                                                End If

                                                                objMovDetTecExtra.Id_Agenda = 0
                                                                objMovDetTecExtra.Id_Mov = 0
                                                                objMovDetTecExtra.Id_Mov_Det = 0
                                                                objMovDetTecExtra.Id_Reg_Dettaglio = 0

                                                            Next

                                                            objMovDet.Movimenti_Dettagli_Riferiti.Clear()
                                                            objMovDet.Movimenti_Dettagli_Conferimento.Clear()



                                                            'Reset Riferimenti
                                                            objMovDet.Movimenti_Dettagli_Riferimenti.Clear()

                                                            'Agende Create per la prima volta --> inserimento riferimento
                                                            Dim objMovimento_Dettaglio_Riferimento = New Movimento_Dettaglio_Riferimento With {
                                                                .Piva = Piva,
                                                                .Sa_Cod = 0,
                                                                .Id_Agenda = 0,
                                                                .Id_Mov = 0,
                                                                .Id_Mov_Det = 0,
                                                                .Lav_Cod = Lav_Cod_Destinazione,
                                                                .Cau_Mov = Cau_Mov,
                                                                .Piva_Rif = Piva,
                                                                .Id_Agenda_Rif = Id_Agenda_Base,
                                                                .Id_Mov_Rif = Id_Mov_Base,
                                                                .Id_Mov_Det_Rif = Id_Mov_Det_Base,
                                                                .Lav_Cod_Rif = Lav_Cod_Base,
                                                                .Cau_Mov_Rif = CAU_REGISTRAZIONI,
                                                                .Qta = Qta_Residua
                                                        }

                                                            objMovDet.Movimenti_Dettagli_Riferimenti.Add(objMovimento_Dettaglio_Riferimento)



                                                            'MOVIMENTI DESTINAZIONI
                                                            For Each objMovDest In objMovDet.Movimenti_Destinazioni


                                                                objMovDest.Id_Agenda = 0
                                                                objMovDest.Id_Mov = 0
                                                                objMovDest.Id_Mov_Det = 0
                                                                objMovDest.Qta = Qta_Residua


                                                                Select Case objMovDet.Elem_Cod

                                                                    Case ALTRI_BENI_AMMORTIZZABILI, ALTRI_BENI, RIGA_DESCRIZIONE_LIBERA

                                                                    Case Else

                                                                        'Aggiornamento Pesi e Colli
                                                                        Peso = Peso + Qta_Residua * If(objMovDet.Qta_Extra = 0, 1, objMovDet.Qta_Extra)
                                                                        Num_Colli = Num_Colli + Qta_Residua


                                                                End Select

                                                            Next

                                                            Numero_Dettagli_Creati = Numero_Dettagli_Creati + 1

                                                            'Prendendo i dettagli dei ddt come base per quelli della fattura,
                                                            'occorre ricreare gli ordinamenti eccetto quelli speciali degli imballi
                                                            If Not {RIGA_IMBALLI_CONTENTI_PRODOTTI, RIGA_IMBALLI_VUOTI_IN_ENTRATA}.Contains(objMovDet.Ordine_Det) Then
                                                                objMovDet.Ordine_Det = Numero_Dettagli_Creati
                                                            End If

                                                            objMov.Movimenti_Dettagli.Add(objMovDet)

                                                        End If


                                                    Next

                                                Next

                                                If Numero_Dettagli_Creati > 0 Then

                                                    'Calcolo Totale Documento
                                                    Variazioni = 0
                                                    Imposta = 0
                                                    Importo = 0
                                                    Imponibile_Netto = 0
                                                    Imponibile_Lordo = 0
                                                    Dim listCastelletto As List(Of Contabilita_Castelletto_Iva) = Nothing

                                                    Importo = utilityHelper.Calcola_Importo_Documento(Imponibile_Netto,
                                                                                              Imponibile_Lordo,
                                                                                              Variazioni,
                                                                                              Imposta,
                                                                                              listCastelletto,
                                                                                              Lav_Cod_Destinazione,
                                                                                              0,
                                                                                              objMov.Movimenti_Dettagli,
                                                                                              objParametri_Server)



                                                    '========================================================================================================
                                                    'Impostazione Pagamento
                                                    '--------------------------------------------------------------------------------------------------------

                                                    Select Case Lav_Cod_Destinazione

                                                        Case 1001 'Fattura Emessa

                                                            Dim ObjPagamento As New Pagamento

                                                            ObjPagamento = utilityHelper.Impostazione_Modalita_Pagamento(Piva,
                                                                                                                         Cod_Contatto,
                                                                                                                         Cod_Risum,
                                                                                                                         Lav_Cod_Destinazione,
                                                                                                                         ArrayGruppi(Col_Cau_Pagamento, j),
                                                                                                                         Importo,
                                                                                                                         0,
                                                                                                                         0,
                                                                                                                         Data_Emissione,
                                                                                                                         Data_Scadenza,
                                                                                                                         objParametri_Server)


                                                            'Imposto la data di scadenza
                                                            objAgenda.Movimenti(IndiceMov4000).Scadenza = Data_Scadenza


                                                            'Aggancio il pagamento al movimento con cau_mov 4000
                                                            objAgenda.Movimenti(IndiceMov4000).Pagamenti.Add(ObjPagamento)

                                                        Case Else

                                                            'Nessuna Gestione Pagamento

                                                    End Select

                                                End If

                                        End Select

                                        If Doc_Numero = 0 Then

                                            'Calcolo Numero Documento

                                            If Lav_Cod_Destinazione = LAVCOD_FATTURA_EMESSA Then

                                                Dummy = AgronicaCoreModello.UtilityHelper.AlgoritmoAssegnazioneNumDoc(Doc_Numero, Doc_Numero_Visualizzato_New, Lav_Cod_Destinazione, Piva, Data_Emissione, Doc_Numero_Sin, 0, Doc_Numero_Des, False, Doc_Numero_Visualizzato, objParametri_Server)

                                            Else

                                                Doc_Numero = 0 'Todo non sono in grado di determinarlo
                                                Doc_Numero_Visualizzato_New = 0

                                            End If

                                        End If


                                        'Sostituzione Numero Documento
                                        objMov.Doc_Numero = Doc_Numero
                                        objMov.Doc_Numero_Visualizzato = Doc_Numero_Visualizzato_New

                                    Next


                                    'Scrittura Documento

                                    'Controllo numero dettagli creati
                                    If Numero_Dettagli_Creati > 0 Then

                                        'Costruzione Des_lib
                                        objAgenda.Des_Lib = AgronicaCoreContabHLP.Contabilita.CreaDesLib(Lav_Cod_Destinazione, Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des, Rag_Soc_Contatto, Tipo_Enum_Documento)
                                        objAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura

                                        'Aggiornamento Totale Documento e Pesi per Entrambi i Movimenti
                                        For Each objMov In objAgenda.Movimenti
                                            objMov.Num_Protocollo = 0 'Deve funzionare così
                                            objMov.Num_Protocollo_Decimal = Importo
                                            objMov.Peso = Peso
                                            objMov.Colli = Num_Colli
                                        Next

                                        Id_Agenda_New = objAgendaHelper.Scrivi(objAgenda, objParametri_Server)

                                        'Aggiornamento Num_Registrazione
                                        objProgressivi.Nuovo_Progressivo_UpdateImmediato(Piva, Anno, enum_SequenzaProgressiviTipi.NumeroRegistrazioneOperazioniContabili,
                                                                                 Doc_Numero_Sin, Doc_Numero_Des, Sezionale_Cod, objParametri_Server)

                                        Numero_Documenti_Creati += 1

                                    End If


                                Next


                            End If


                        Next 'Fine Contatto




                        If Numero_Documenti_Creati > 0 Then

                            'Aggiornamento Risultato
                            Modifica_Scheduling.Modifica(ID, Numero_Documenti_Creati, Data_Inizio_Elaborazione, Date.Now, objParametri_Server)

                            'Aggiornamento Numero Totale Documenti Creati
                            Numero_Documenti_Creati_Totale = Numero_Documenti_Creati_Totale + Numero_Documenti_Creati


                        Else

                            'Valutare se impostare la chiusura dell'ID (Numero_Dicumenti_Creati = 0 e Data_inizio_Elaborazione = Null

                            'Cancellazione ID poichè inconsistente
                            Modifica_Scheduling.Cancella(ID, "", objParametri_Server)

                        End If

                        Utility.VerificaChiudiConnessione(objParametri_Server, False)

                    End If

                Next


            End If

            r.RispostaStringa = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabile.aspx", "NumeroDocumentiCreati"), String) & " = " & Numero_Documenti_Creati_Totale
            r.RispostaOK = True





        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function





#End Region


#Region "Collega Imputazioni"

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiListaCdgProgetti(ByVal piva As String, ByVal analisi As String) As RispostaStandard

        'Dichiarazione per recuperare lista delle imputazione
        Dim Dt As DataTable
        Dim objProgetti As New AgronicaCoreContabDAL.Imputazioni_R
        Dim r As New RispostaStandard
        Dim filtroAggiuntivo As String

        'Dichiarazione per recuperare tipo_imputazione
        Dim objImpreseImpostazioni As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
        Dim imputazioneValoreInt As Integer = 0

        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            'Recupero tipo_imputazione
            Dim imputazioneValore As String = objImpreseImpostazioni.LeggiScalareMulticentroAziendaSuperUser(piva, Nothing,
                                                                                                 enum_Impostazioni_Utenti.IMPRESA_DocContabili_Cod_Tipo_Imputazione,
                                                                                                 "",
                                                                                                 objParametri_Utenti,
                                                                                                 objParametri_Server)

            If Not String.IsNullOrEmpty(imputazioneValore) Then
                imputazioneValoreInt = CInt(imputazioneValore)
            End If


            filtroAggiuntivo = " it.tipo_imputazione = " & Agro_SQL_SaveNum(imputazioneValoreInt) & " AND i.analisi in ('" & Agro_SQL_SaveText(analisi) & "','|0|') AND i.validita_inizio <= " & Agro_SQL_SaveDateTime(DateTime.Now) & " AND i.validita_fine >= " & Agro_SQL_SaveDateTime(DateTime.Now)

            Dt = objProgetti.Leggi(piva, 0, filtroAggiuntivo, "", objParametri_Server)

            For Each row As DataRow In Dt.Rows
                row.Item("Imputazione_Nome") = row.Item("imputazione_Cod_Des") & " - " & row.Item("Imputazione_Nome") & " ( " + row.Item("imputazione_classe_des") & " )"
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaCDGImputazioniDocContabile(ByVal piva As String,
                                                              ByVal saCod As Integer,
                                                              ByVal idAgenda As Integer,
                                                              ByVal idMov As Integer,
                                                              ByVal idMovDet As Integer,
                                                              ByVal lavCod As Integer,
                                                              ByVal dataDoc As Date,
                                                              ByVal descLib As String,
                                                              ByVal imputazione_Cod As String,
                                                              ByVal odaOddt As Integer
                                                              ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xRisp As Boolean = False
        Dim cdgR As New AgronicaCoreContabDAL.CDG_DAL_R

        Dim objtestataEredita
        Dim objdettagliImputazioni

        Dim datoAggiuntivoDesLib

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try


            Dim Id_Agenda_CDG As Integer = 0

            ' Ricavo l'Id agenda CDG
            Dim dt_agenda_rif = cdgR.Leggi_Agenda_Riferimento(piva, idAgenda, idMovDet, True, objParametriServer)
            If dt_agenda_rif.Rows.Count > 0 Then
                Id_Agenda_CDG = dt_agenda_rif.Rows(0).Item("Id_Agenda_Rif")
            End If

            Dim dati_CDG As String = "{}"

            If Id_Agenda_CDG <> 0 Then
                ' se esiste agenda cdg recupero i dati relativi
                dati_CDG = cdgR.Leggi_CDG(piva, idAgenda, idMovDet, Id_Agenda_CDG,
                                          dataDoc, 0, 0, "", 0,
                                          objParametriServer)

                'Ottengo Json con { "kendo_Eredita": [{"Id_CDG": 201,...}], "kendo_Progetti_Dettagli": [{},{}] }
            Else
                'se non esiste, devo crearmi cmq la testata
                dati_CDG = cdgR.Leggi_CDG(piva, idAgenda, idMovDet, Id_Agenda_CDG,
                                          dataDoc, 1, 0, "", 0,
                                          objParametriServer)

                'Ottengo Json con { "kendo_Eredita": [{"Id_CDG": 0,...}] }

            End If


            ' ***** VALORIZZAZIONE INFORMAZIONI *****

            If Not String.IsNullOrEmpty(dati_CDG) AndAlso dati_CDG <> "{ }" AndAlso dati_CDG <> "{}" Then

                Dim objCdg As JObject = JObject.Parse(dati_CDG)

                Dim righeArray_Testata As JArray = objCdg("kendo_Eredita")

                'TODO: valorizzo Id_Attivita in kendoEredita (ed anche Desc?!?)

                For Each obj As JObject In righeArray_Testata


                    'Recupero Id_Attivita e Desc_Attivita'
                    Dim Dt_AttivitaProgetto As DataTable
                    Dim obj_AttivitaProgetto As New AgronicaCoreContabDAL.Imputazioni_Fasi_R
                    Dt_AttivitaProgetto = obj_AttivitaProgetto.Leggi(piva, 0, imputazione_Cod, 0, "", "", objParametriServer, enumSelezioneVariabile.Selezione_TabellaCompleta)

                    If Dt_AttivitaProgetto.Rows.Count > 0 Then
                        Dim row = Dt_AttivitaProgetto.Rows(0)
                        obj("ID_Attivita") = row.Item("Imputazione_Fase_Cod").ToString()
                        'obj("Desc") = row.Item("Attivita_Des").ToString()
                        obj("Desc") = "IMPORT"
                    End If

                    obj("Modalita_Imputazione") = 4 'TODO: ???
                    datoAggiuntivoDesLib = " (" + obj("Descrizione").ToString() + " " + obj("Lotto").ToString() + ")"

                Next

                objtestataEredita = JsonConvert.SerializeObject(righeArray_Testata, Formatting.None)



                If objCdg("kendo_Progetti_Dettagli") IsNot Nothing Then

                    'Avevo già scritto la CDG

                    'TODO: se modifica, update in kendo_Progetti_Dettagli di Imputazione_Cod, Imputazione_Nome,
                    'Tipo_Imputazione, Tipo_Imputazione_Des, Imputazione_Classe_Cod, Imputazione_Classe_Des

                    Dim righeArray_Progetti_Dettagli As JArray = objCdg("kendo_Progetti_Dettagli")

                    For Each objDettagli As JObject In righeArray_Progetti_Dettagli
                        objDettagli("Imputazione_Cod") = imputazione_Cod     'TODO: nuovo codiceImputazione
                        'objDettagli("Imputazione_Nome") = ""   'TODO: nuova descrizione imputazione

                        'objDettagli("Tipo_Imputazione") = 0         'TODO: nuovo codice tipo imputazione
                        'objDettagli("Tipo_Imputazione_Des") = ""    'TODO: nuova descrizione tipo imputazione

                        'objDettagli("Imputazione_Classe_Cod") = 0     'TODO: nuovo codice classe imputazione
                        'objDettagli("Imputazione_Classe_Des") = ""    'TODO: nuova descrizione classe imputazione
                    Next

                    'Devo serializzare l'oggetto
                    objdettagliImputazioni = JsonConvert.SerializeObject(righeArray_Progetti_Dettagli, Formatting.None)

                Else

                    'Non avevo ancora scritto la CDG, quindi devo creare l'oggetto

                    'TODO: se creazione devo creato tutto l'oggetto kendo_Progetti_Dettagli

                    Dim objDettagli As New JObject
                    objDettagli("Id_CDG_Dettagli") = 0
                    objDettagli("Valore") = 100             '100%
                    objDettagli("Imputazione_Cod") = imputazione_Cod     'TODO: nuovo codiceImputazione

                    objdettagliImputazioni = "[" + JsonConvert.SerializeObject(objDettagli, Formatting.None) + "]"

                    'TODO: creare l'oggetto kendo_Progetti_Dettagli

                End If

            End If


            'TODO: Dopo aver creato/modificato la parte dei dettagli e/o la testata, vanno ri-serializzate come stringhe



            Dim righeInseriteGrid_Testata_Eredita As String = ""
            Dim righeModificateGrid_Testata_Eredita As String = ""
            Dim righeInseriteGrid_Dettagli_Progetti As String = ""
            Dim righeModificateGrid_Dettagli_Progetti As String = ""


            If Id_Agenda_CDG <> 0 Then
                'Avevo già segnato assegnato un'imputazione
                righeModificateGrid_Testata_Eredita = objtestataEredita
                righeModificateGrid_Dettagli_Progetti = objdettagliImputazioni
            Else
                'lo devo inserire
                righeInseriteGrid_Testata_Eredita = objtestataEredita
                righeInseriteGrid_Dettagli_Progetti = objdettagliImputazioni
            End If




            ' ***** CREATE / UPDATE  records *****
            Dim costoRicavo As Integer = 0 '0 per ODA e (1 per ddt emesso?)
            costoRicavo = odaOddt

            Dim idAgendaCdgScritta As Integer = 0
            Dim cgdW As New AgronicaCoreContabBIZ.CDG_BIZ_W
            idAgendaCdgScritta = cgdW.AggiornaCDGImputazioniDocContabile(piva, idAgenda, Id_Agenda_CDG,
                                                                         lavCod, Des_Lib:=descLib + datoAggiuntivoDesLib,
                                                                         Data_Inserimento:=dataDoc,
                                                                         Costi_Ricavi:=costoRicavo,
                                                                         Id_Attivita_Batch:=0, ModifInAutom:=0,
                                                                         righeInseriteGrid_Testata_Eredita:=righeInseriteGrid_Testata_Eredita,
                                                                         righeModificateGrid_Testata_Eredita:=righeModificateGrid_Testata_Eredita,
                                                                         righeInseriteGrid_Dettagli_Progetti:=righeInseriteGrid_Dettagli_Progetti,
                                                                         righeModificateGrid_Dettagli_Progetti:=righeModificateGrid_Dettagli_Progetti,
                                                                         objParametri:=objParametriServer,
                                                                         Id_Mov:=idMov, Id_Mov_Det:=idMovDet)

            If idAgendaCdgScritta > 0 Then
                xRisp = True
            End If

            r.RispostaOK = xRisp
            r.RispostaStringa = idAgendaCdgScritta

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
#End Region


    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaPermessoVisibilitaGruppiMerce(ByVal piva As String, ByVal Id_Agenda As Integer, ByVal Lav_Cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim permessiGruppiMerce As Boolean = ControllaPermessiVisibilitaGruppiMerce(objParametriServer, objParametriUtenti, piva, Id_Agenda, Lav_Cod)
        If permessiGruppiMerce = False Then
            r.RispostaOK = False
            r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabile.aspx", "DocNonVisualizzabileGruppiMerceOperazioneNonEseguibile"), String)
        Else
            r.RispostaOK = True
        End If

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaPermessoVisibilitaGruppiMerceMultiDocumento(ByVal listaDocumenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim listaDoc As JArray = JsonConvert.DeserializeObject(listaDocumenti, (New JArray).GetType(), settingLoc)

        Dim permessiGruppiMerce As Boolean

        For Each objDoc As JToken In listaDoc.Children

            permessiGruppiMerce = ControllaPermessiVisibilitaGruppiMerce(objParametriServer, objParametriUtenti,
                                                                         objDoc.Value(Of String)("Piva"),
                                                                         objDoc.Value(Of String)("Id_Agenda"),
                                                                         objDoc.Value(Of String)("Lav_Cod"))

            If permessiGruppiMerce = False Then
                Exit For
            End If

        Next


        If permessiGruppiMerce = False Then
            r.RispostaOK = False
            r.Errore = DirectCast(HttpContext.GetLocalResourceObject("~/GestioneContabilita/DocContabile.aspx", "DocumentiNonVisualizzabiliGruppiMerceOperazioneNonEseguibile"), String)
        Else
            r.RispostaOK = True
        End If

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaConfezionamentoLotto(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objMateriePrime As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

            Dim dtConfezionamentoLotto = objMateriePrime.MateriePrime_DescrizioniOP(piva,
                                                                                    0,
                                                                                    BENI_CONFEZ_VEGETALE,
                                                                                    "",
                                                                                    " Materie_Prime.Extra_Int = 1 ",
                                                                                    "",
                                                                                    objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }

            r.RispostaStringa = JsonConvert.SerializeObject(dtConfezionamentoLotto, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiLogInvioDettaglio(ByVal piva As String, ByVal idAgende As String()) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim impImpresa As New Dictionary(Of String, String)

            Dim impreseImpostazioniR As New AgronicaCoreAnagrafeDAL.Imprese_Impostazioni_R
            Dim valImp As String

            Dim dictImp As New Dictionary(Of enum_Impostazioni_Utenti, String)
            dictImp.Add(enum_Impostazioni_Utenti.IB_RisorseUmane_SettoreDes_Controllo, "0")
            dictImp.Add(enum_Impostazioni_Utenti.GruppoMerce_Controllo, "0")
            dictImp.Add(enum_Impostazioni_Utenti.CdC_Wbs_Controllo, "0")
            dictImp.Add(enum_Impostazioni_Utenti.Collega_Solo_Ordini_Inviati, "0")

            For Each elemDictImp In dictImp
                valImp = impreseImpostazioniR.LeggiScalareMulticentroAziendaSuperUser(piva, Nothing,
                                                                                              elemDictImp.Key,
                                                                                              elemDictImp.Value,
                                                                                              objParametriUtenti,
                                                                                              objParametriServer)

                impImpresa.Add(elemDictImp.Key, valImp)
            Next

            Dim dataProviderGenerico As New DataProvider

            Dim strSql As New StringBuilder
            strSql.AppendLine("SELECT *")
            strSql.AppendLine("FROM __Log_Invio_BC_Dettaglio")
            strSql.AppendLine("WHERE 1 = 1")

            Dim listaCondImpresa As New List(Of String)

            If impImpresa(enum_Impostazioni_Utenti.IB_RisorseUmane_SettoreDes_Controllo) Then
                listaCondImpresa.Add("Validita_Codice_Contatto <> 'OK'")
            End If

            If impImpresa(enum_Impostazioni_Utenti.GruppoMerce_Controllo) Then
                listaCondImpresa.Add("Validita_Gruppo_Merce <> 'OK'")
            End If

            If impImpresa(enum_Impostazioni_Utenti.CdC_Wbs_Controllo) Then
                listaCondImpresa.Add("Validita_CDC_WBS = 'NON PRESENTE'")
            End If

            If impImpresa(enum_Impostazioni_Utenti.Collega_Solo_Ordini_Inviati) Then
                listaCondImpresa.Add("Validita_Collegamento_Ordine = 'NON PRESENTE'")
            End If

            If listaCondImpresa.Count > 0 Then
                strSql.AppendLine("AND (" & String.Join(" OR ", listaCondImpresa) & ")")
            End If

            If idAgende.Length = 0 Then
                idAgende = {0}
            End If
            strSql.AppendLine("AND id_agenda IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", idAgende), False) & ")")

            Dim dt = dataProviderGenerico.EseguiQuery_Lettura(objParametriServer, strSql.ToString, "DocContabile_WS.LeggiLogInvioDettaglio")

            If dt.Rows.Count > 0 Then

                dt.Columns.Add("Messaggio", GetType(String))

                For Each dr As DataRow In dt.Rows

                    Dim listaMessaggi As New List(Of String)

                    If impImpresa(enum_Impostazioni_Utenti.IB_RisorseUmane_SettoreDes_Controllo) AndAlso dr("Validita_Codice_Contatto") = "NON PRESENTE" Then
                        listaMessaggi.Add("Codice Contatto non valido")
                    End If

                    If impImpresa(enum_Impostazioni_Utenti.GruppoMerce_Controllo) AndAlso dr("Validita_Gruppo_Merce") = "NON PRESENTE" Then
                        listaMessaggi.Add("Gruppo Merce non specificato")
                    End If

                    If impImpresa(enum_Impostazioni_Utenti.CdC_Wbs_Controllo) AndAlso dr("Validita_CDC_WBS") = "NON PRESENTE" Then
                        listaMessaggi.Add("CDC/WBS non specificato")
                    End If

                    If impImpresa(enum_Impostazioni_Utenti.Collega_Solo_Ordini_Inviati) AndAlso dr("Validita_Collegamento_Ordine") = "NON PRESENTE" Then
                        listaMessaggi.Add("Documento non collegato ad un ordine")
                    End If

                    dr("Messaggio") = String.Join("<br/>", listaMessaggi)

                Next

            End If

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r


    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Udm_Trasporto() As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim handleUm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R()

            Dim listaCodTrasporto As New List(Of Integer) From {enum_UnitaMisura.Chilometri, enum_UnitaMisura.Miglia}

            Dim dt = handleUm.LeggiDaElencoCod(objParametriServer, listaCodTrasporto, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "")

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }

            r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False, source:=True)
        End Try

        Return r

    End Function

End Class