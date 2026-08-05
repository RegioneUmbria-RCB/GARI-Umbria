Imports System.Security.Cryptography
Imports System.Text
Imports System.IO
Imports Sincro_Agrea2Gias.MyWsAgriRER
Imports Sincro_Agrea2Gias
Imports System.Web.Services

Imports System.Xml
Imports System.Xml.Serialization
Imports System.Runtime.Serialization
Imports System.Configuration
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgroFascicoloBA_BIZ.localhost
Imports System.Web
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider

Public Class GestoreFascicoli

    Private ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri


    Private objLog As AgronicaCoreDataProvider.LogProvider
    Private Configurazione_Servizi_R As AgronicaCoreVarieDAL.Configurazione_Servizi_R

    Private LogFileName As String
    Private LogDirectory As String
    Private LogDescrizioneUtente As String
    Private DirectoryFileImportazioni As String
    Private DirectoryFileEsportazioni As String
    Private ParametriExtra As String

    Private username As String
    Private password As String

    Private linkWS_Anagrafe As String
    Private username_Anagrafe As String
    Private password_Anagrafe As String
    Private linkWS_Agrea As String
    Private username_Agrea As String
    Private password_Agrea As String
    Private limiteGiornaliero As Integer
    Private AnniDaImportare As List(Of Integer)

    Sub New(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio, ByVal _ObjParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal _ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal _ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Me.ObjParametri_SuperServer = _ObjParametri_SuperServer
        Me.ObjParametri_Server = _ObjParametri_Server
        Me.ObjParametri_Utenti = _ObjParametri_Utenti

        InizializzoOggettiCore()

        ImpostoGliAltriParametri(_Configurazione_Servizio)

        impostaParametriExtra()

    End Sub

    Public Function avviaImportazione() As Boolean
        logga("START Servizio")
        Dim returnBool = True
        'If False Then
        If linkWS_Anagrafe <> "" And username_Anagrafe <> "" And password_Anagrafe <> "" Then
            Dim importatoreAnagrafe As New ImportatoreGIAS_FascicoliAnagrafe(ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti, objLog, LogFileName, LogDirectory, LogDescrizioneUtente,
                                                                     DirectoryFileImportazioni, DirectoryFileEsportazioni, ParametriExtra, username, password, linkWS_Anagrafe, username_Anagrafe, password_Anagrafe, limiteGiornaliero, AnniDaImportare)
            logga("importatoreAnagrafe.AziendeModificate()")
            importatoreAnagrafe.AziendeModificate()
            logga("importatoreAnagrafe.importaAnagrafe()")
            returnBool = importatoreAnagrafe.importaAnagrafe()
        End If

        Dim importatoreAgrea As New ImportatoreGIAS_FascicoliAgrea(ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti, objLog, LogFileName, LogDirectory, LogDescrizioneUtente,
                                                                     DirectoryFileImportazioni, DirectoryFileEsportazioni, ParametriExtra, username, password, linkWS_Agrea, username_Agrea, password_Agrea, AnniDaImportare)
        logga("importatoreAgrea.importaAgrea()")
        importatoreAgrea.importaAgrea2()
        'importatoreAgrea.AggiornaFascicoliDaImportare()

        logga("FINE importatoreAgrea.importaAgrea()")

        'Dim importatoreArtea As New ImportatoreGIAS_FascicoliArtea(ObjParametri_SuperServer, ObjParametri_Server, ObjParametri_Utenti, objLog, LogFileName, LogDirectory, LogDescrizioneUtente,
        '                                                                 DirectoryFileImportazioni, DirectoryFileEsportazioni, ParametriExtra, username, password, AnniDaImportare)
        'logga("importatoreAgrea.importaArtea()")
        'importatoreArtea.importaArtea()

        'logga("END Servizio")
        Return returnBool
    End Function

    Private Sub aziendeModificate(ByRef EnteValidatore_Cod As Integer)
        Dim writer As New Fascicolo_W
        Dim ErrCOD As Integer
        Dim ErrMsg As String = ""
        Dim result As String
        Dim dataModifica As DateTime? = getDataModifica(EnteValidatore_Cod)
        If dataModifica IsNot Nothing Then
            logga("Richiesta Aziende Modificate " + CStr(dataModifica))

            Dim ws_FascicoloA As New SincroAnagrafeBA.FascicoloSiar2Response
            Dim ws_FascicoloColdi As New SincroAnagrafeBA.FascicoloWS_FascicoloHttpService("http://192.168.1.245/WS_AgroFascicoloBA/FascicoloMM_FascicoloWS.asmx")
            Dim gF As New SincroAnagrafeBA.getFascicoloNew
            gF.Data_Riferimento = dataModifica
            Select Case EnteValidatore_Cod
                Case 1
                    gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.anagrafe
                Case 2
                    gF.filtro = SincroAnagrafeBA.FiltroGetFascicolo.PianoColturale
            End Select
            ws_FascicoloColdi.SoapVersion = Protocols.SoapProtocolVersion.Soap12
            Dim FascicoloSiar As New FascicoloSiar2Response
            result = ws_FascicoloColdi.getFascicoloNew(gF)

            If result IsNot Nothing AndAlso result <> "" Then
                Dim listaCuaa = result.Split("|").ToList
                For Each cuaa In listaCuaa
                    If cuaa IsNot Nothing AndAlso cuaa <> "" Then
                        writer.ScriviAggiornaFascicoli(ObjParametri_Server, EnteValidatore_Cod, dataModifica, Now.Date, cuaa, 0)
                    End If
                Next
            End If
        End If
    End Sub

    Private Function getDataModifica(ByRef enteValidatore_cod As Integer) As DateTime?
        Dim reader As New Fascicolo_R
        Dim dataModifica As Date
        Dim dt As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod)
        If dt.Rows.Count > 0 Then
            Dim dt1 As DataTable = reader.LeggiAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod, , , , 0)
            If dt1.Rows.Count = 0 Then
                dataModifica = CDate(reader.maxDataAggiornaFascicoli(ObjParametri_Server, enteValidatore_cod).Rows(0)(0))
                If dataModifica = Now.Date Then
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Else
            dataModifica = New Date(2016, 12, 1)
        End If
        Return dataModifica
    End Function
#Region "Util"

    Private Sub logga(ByVal msg As String)
        Dim customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = LogDescrizioneUtente,
            .LogDirectory = LogDirectory,
            .LogFileName = LogFileName
        }

        objLog.Scrivi_LOG(ObjParametri_Server, "", msg, CustomLOGParams:=customLOGParams)
    End Sub

    Private Sub InizializzoOggettiCore()

        objLog = New AgronicaCoreDataProvider.LogProvider

    End Sub

    Private Sub ImpostoGliAltriParametri(ByVal _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio)

        LogFileName = _Configurazione_Servizio.Tipo_Sincro.ToString & "_log.txt"
        LogDirectory = _Configurazione_Servizio.DirectoryLOG
        LogDescrizioneUtente = _Configurazione_Servizio.Tipo_Sincro.ToString
        DirectoryFileImportazioni = _Configurazione_Servizio.DirectoryFileImportazioni
        DirectoryFileEsportazioni = _Configurazione_Servizio.DirectoryFileEsportazioni
        ParametriExtra = _Configurazione_Servizio.Parametri_Extra

        username = _Configurazione_Servizio.Username
        password = _Configurazione_Servizio.Password

    End Sub

    Private Sub impostaParametriExtra()
        Dim listaPar As List(Of String) = ParametriExtra.Split("|").ToList
        Dim ht As New Hashtable
        For Each par In listaPar
            Dim key = par.Split("=")(0)
            Dim value = par.Split("=")(1)
            ht.Add(key, value)
        Next
        'WS
        linkWS_Anagrafe = ht.Item("WS_Anagrafe")
        linkWS_Agrea = ht.Item("Ws_Agrea")
        username_Anagrafe = ht.Item("usernameWSAnagrafe_Coldi")
        username_Agrea = ht.Item("usernameWSAgrea_Coldi")
        password_Anagrafe = ht.Item("pwdWSAnagrafe_Coldi")
        password_Agrea = ht.Item("pwdWSAgrea_Coldi")
        limiteGiornaliero = CInt(ht.Item("limite_Giornaliero"))
        Dim stringaAnni As String = ht.Item("Anni_Da_Importare")
        If stringaAnni <> "" Then
            AnniDaImportare = New List(Of Integer)
            For Each annoStr In stringaAnni.Split(",")
                If annoStr <> "" Then
                    Try
                        Dim anno As Integer = CInt(annoStr)
                        AnniDaImportare.Add(anno)
                    Catch ex As Exception

                    End Try
                End If
            Next
        End If
    End Sub

#End Region

End Class