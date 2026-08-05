Imports AgronicaCoreDataProvider
Imports AgronicaCoreRegVinoDAL

Public MustInherit Class TeleregistriManager
    Implements ITeleregistriManager

    'Utility per LOG
    Protected objLog As AgronicaCoreDataProvider.LogProvider
    Protected LogFileName As String
    Protected LogDirectory As String
    Protected LogDescrizioneUtente As String
    Protected DirectoryFileImportazioni As String
    Protected DirectoryFileEsportazioni As String
    'Utility per LOG

    Protected caller As RegVino
    Protected username As String
    Protected password As String
    Protected urlSync As String
    Protected urlASync As String
    Protected CertificateFile As String

    Protected ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Protected ObjParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Protected utils As DBUtilityComuni

    Protected customLOGParams As CustomLOGParams

    Public Sub New(user As String,
                   pwd As String,
                   urs As String,
                   ura As String,
                   server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                   _Configurazione_Servizio As AgronicaCoreVarieDAL.Configurazione_Servizio,
                   CertificateFile As String)
        username = user
        password = pwd
        urlSync = urs
        urlASync = ura
        ObjParametri_Server = server
        ObjParametri_Utenti = utenti
        Me.CertificateFile = CertificateFile

        InizializzoOggettiCore()

        ImpostoGliAltriParametri(_Configurazione_Servizio)

        caller = New RegVino()
        utils = New DBUtilityComuni()
        inizializzaReaderWriter()
    End Sub

    Public Function cicloEsportazioneInput() As Boolean Implements ITeleregistriManager.cicloEsportazioneInput
        Dim insert = inserisciAggiornaTuttoInput(TipoRichiesta.I)
        Dim refresh = inserisciAggiornaTuttoInput(TipoRichiesta.A)
        Dim delete = eliminaTuttoInput()
        If (insert Or refresh Or delete) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function cicloEsportazioneOutput() As Boolean Implements ITeleregistriManager.cicloEsportazioneOutput
        Dim insertRefresh = inserisciAggiornaTuttoOutput()
        Dim delete = eliminaTuttoOutput()
        If (insertRefresh Or delete) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Sub cicloEsportazione() Implements ITeleregistriManager.cicloEsportazione
        Try
            Dim input = cicloEsportazioneInput()
            Dim output = cicloEsportazioneOutput()
            'If input Or output Then
            '    checkResult()
            'End If
            checkResult()
        Catch ex As Exception
            logga(1, "AgronicaCoreRegVinoBIZ.Modelli.TeleregistriManager.cicloEsportazione", $"EXCEPTION: {ex.Message}")
            Throw New Exception("[" & "cicloEsportazione" & "] : " & ex.Message)
        End Try

    End Sub

    Public MustOverride Sub inizializzaReaderWriter() Implements ITeleregistriManager.inizializzaReaderWriter

    Public MustOverride Function eliminaTuttoInput() As Boolean Implements ITeleregistriManager.eliminaTuttoInput

    Public MustOverride Function eliminaTuttoOutput() As Boolean Implements ITeleregistriManager.eliminaTuttoOutput

    Public MustOverride Function inserisciAggiornaTuttoInput(tipoRichiesta As Integer) As Boolean Implements ITeleregistriManager.inserisciAggiornaTuttoInput

    Public MustOverride Function inserisciAggiornaTuttoOutput() As Boolean Implements ITeleregistriManager.inserisciAggiornaTuttoOutput

    Public MustOverride Sub checkResult() Implements ITeleregistriManager.checkResult

    Public Sub logga(level As Integer, nomeRoutine As String, ByVal msg As String)
        Dim tabs As String = ""
        For value As Integer = 0 To level - 1
            tabs &= vbTab
        Next
        'objLog.Scrivi_LOG(LogDirectory, LogFileName, LogDescrizioneUtente, "", tabs & msg)

        objLog.Scrivi_LOG(ObjParametri_Server, nomeRoutine, $"{tabs}{msg}", CustomLOGParams:=customLOGParams)

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
        customLOGParams = New CustomLOGParams With {
                .LogDescrizioneUtente = LogDescrizioneUtente,
                .LogDirectory = LogDirectory,
                .LogFileName = LogFileName
            }
    End Sub

End Class
