Imports System.Web.Services
Imports AgronicaCoreUmaDal
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports System.Web
Imports AgronicaCoreUmaBiz

Public Class VenditeCarburantiUMA
    Inherits System.Web.UI.Page
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            inizializzoObjParametri()

            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.UMA_Vendite_Carburanti,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

            Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.UMA_Vendite_Carburanti,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

            hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
            hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura
            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim DTConfigSiti As DataTable
            DTConfigSiti = objConfigSiti.Leggi(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametri_Server)
            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                hfPathCoreWS.Value = DTConfigSiti.Rows(0).Item("Valore")
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Public Shared Function EseguireOperazione(Of TResult)(f As Func(Of TResult)) As RispostaStandard
        Dim r As New RispostaStandard()

        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim risultato As TResult = f()
            r.RispostaStringa = JsonConvert.SerializeObject(risultato)
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
        End Try
        Return r
    End Function
    Public Shared Function EseguireOperazione(Of T, TResult)(f As Func(Of T, TResult), param As T) As RispostaStandard
        Dim r As New RispostaStandard()

        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim risultato As TResult = f(param)
            r.RispostaStringa = JsonConvert.SerializeObject(risultato)
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCarburantiVenditaDaDB(anno As Integer, piva As String, ByVal nuovaVisibilita As Boolean) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.CaricaCarburantiVendita, New FiltriCaricaRigheVenditaCarburanti(anno, piva, nuovaVisibilita))

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaValiditaCuaa(cuaa As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.VerificaValiditaCuaa, cuaa)

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaPraticaRichiestaCarburanteAperta(filtri As FiltriPraticaRichiestaCarburante) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.VerificarePraticaDiRichiestaCarburanteSiaAperta, filtri)

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaRagioneSociale(cuaa As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.CaricaRagioneSociale, cuaa)

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaLtAssegnati(parametri As FiltriAggiornaLtAssegnati) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.AggiornaLtAssegnati, parametri)

        Return r
    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function AggiornaTotaleLtAcquistati(parametri As FiltriAggiornaTotaleLtAcquistati) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.AggiornaTotaleLtAcquistati, parametri)

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function DettaglioLtAcquistabili(parametri As FiltriDettaglioLtAcquistabili) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.DettaglioLtAcquistabili, parametri)

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaTipoCarburantiDDL() As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.CaricaTipoCarburantiDDL)

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaTipoDocumentiDDL() As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.CaricaTipoDocumentiDDL)

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function OttieneContiPossibiliDellAcquisto() As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.CaricaContiAcquistoDDL)

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function OttieneAnniValidi() As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.CaricaAnniValidiDDL)

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaRigheModificate(righeDaAggiornare As RigheVenditaCarburantiDaAggiornare) As RispostaStandard
        Dim r As New RispostaStandard()

        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim risultato As RispostaStandard = servizio.AggiornaGrigliaVenditaCarburanti(righeDaAggiornare)
            r.RispostaStringa = JsonConvert.SerializeObject(risultato)
            r.RispostaOK = True
            r.RispostaConferma = True
        Catch ex As Exception
            r.RispostaStringa = JsonConvert.SerializeObject(ex.Message)
            r.RispostaOK = True
            r.RispostaConferma = False
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function OttienePivaClienteERagioneSociale(filtri As OttienePivaCliente) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriUtenti) Then
            r.Sessione = False
            Return r
        End If
        Dim servizio As New Vendita_Carburanti(objParametriServer, objParametriUtenti)
        r = EseguireOperazione(AddressOf servizio.OttienePivaClienteERagioneSociale, filtri)
        Return r
    End Function


End Class

