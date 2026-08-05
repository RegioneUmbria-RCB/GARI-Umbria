Imports System.IO
Imports Microsoft.VisualBasic.FileIO.FileSystem
Imports AgronicaCoreVarieDAL
Imports System.Text

Public Class FileSystemHelper
    Private PercorsoImport As String
    Private PercorsoEsport As String
    Private PercorsoLog As String
    Private PercorsoLavoro As String
    Private PercorsoImportArchivioOK As String
    Private PercorsoImportArchivioERR As String
    Private PercorsoEsportArchivioTMP As String
    Private PercorsoEsportArchivioStorico As String

    Private StoricoAbilitato As Boolean = False

    ' Sottocartelle importazioni
    Private Const sottocartellaArchivioOK = "\ArchivioOK"
    Private Const sottocartellaArchivioERR = "\ArchivioERR"

    ' Sottocartelle esportazioni
    Private Const sottocartellaArchivioTMP = "\ArchivioTMP"
    Private Const sottocartellaArchivioStorico = "\ArchivioStorico"

    Public Sub New(configurazione As Configurazione_Servizio)
        PercorsoImport = configurazione.DirectoryFileImportazioni
        PercorsoEsport = configurazione.DirectoryFileEsportazioni
        PercorsoLog = configurazione.DirectoryLOG
        PercorsoLavoro = String.Empty
        StoricoAbilitato = False
        ImpostaPercorsiSottocartelle()
    End Sub

    Public Sub New(configurazione As Configurazione_Servizio,
                   configurazioneImportatore As ConfigurazioneImportatore)
        Me.New(configurazione)
        PercorsoImport = configurazioneImportatore.PercorsoInput
        PercorsoEsport = configurazioneImportatore.PercorsoOutput
        PercorsoLog = configurazioneImportatore.PercorsoLog
        PercorsoLavoro = configurazioneImportatore.PercorsoLavoro
        StoricoAbilitato = configurazioneImportatore.AbilitaStorico
        ImpostaPercorsiSottocartelle()
    End Sub

    Private Sub ImpostaPercorsiSottocartelle()
        If String.IsNullOrEmpty(PercorsoLavoro) Then
            If Not String.IsNullOrEmpty(PercorsoImport) Then
                PercorsoLavoro = PercorsoImport
            Else
                PercorsoLavoro = PercorsoEsport
            End If
        End If
        PercorsoImportArchivioOK = PercorsoLavoro & sottocartellaArchivioOK
        PercorsoImportArchivioERR = PercorsoLavoro & sottocartellaArchivioERR
        PercorsoEsportArchivioTMP = PercorsoLavoro & sottocartellaArchivioTMP
        If StoricoAbilitato Then
            PercorsoEsportArchivioStorico = PercorsoLavoro & sottocartellaArchivioStorico
        End If
    End Sub

    Public Sub CrearePercorsiImport()
        If Not String.IsNullOrEmpty(PercorsoImport) AndAlso Not Directory.Exists(PercorsoImport) Then
            Directory.CreateDirectory(PercorsoImport)
        End If
        If Not String.IsNullOrEmpty(PercorsoLavoro) AndAlso Not Directory.Exists(PercorsoLavoro) Then
            Directory.CreateDirectory(PercorsoLavoro)
        End If
        If Not String.IsNullOrEmpty(PercorsoImportArchivioOK) AndAlso Not Directory.Exists(PercorsoImportArchivioOK) Then
            Directory.CreateDirectory(PercorsoImportArchivioOK)
        End If
        If Not String.IsNullOrEmpty(PercorsoImportArchivioERR) AndAlso Not Directory.Exists(PercorsoImportArchivioERR) Then
            Directory.CreateDirectory(PercorsoImportArchivioERR)
        End If
    End Sub

    Public Sub CrearePercorsiEsport()
        If Not String.IsNullOrEmpty(PercorsoEsport) AndAlso Not Directory.Exists(PercorsoEsport) Then
            Directory.CreateDirectory(PercorsoEsport)
        End If
        If Not String.IsNullOrEmpty(PercorsoLavoro) AndAlso Not Directory.Exists(PercorsoLavoro) Then
            Directory.CreateDirectory(PercorsoLavoro)
        End If
        If Not String.IsNullOrEmpty(PercorsoEsportArchivioTMP) AndAlso Not Directory.Exists(PercorsoEsportArchivioTMP) Then
            Directory.CreateDirectory(PercorsoEsportArchivioTMP)
        End If
        If Not String.IsNullOrEmpty(PercorsoEsportArchivioStorico) AndAlso Not Directory.Exists(PercorsoEsportArchivioStorico) Then
            Directory.CreateDirectory(PercorsoEsportArchivioStorico)
        End If
    End Sub

    Friend Function OttieniElencoFileDaImportare(tipoFile As String) As String()
        Return Directory.GetFiles(PercorsoImport, tipoFile)
    End Function

    Friend Function GetPercorsoLog() As String
        Return PercorsoLog
    End Function

    Friend Sub SpostareFileInArchivioOK(fileImport As String, nomeFile As String)
        SpostareFileInArchivio(fileImport, nomeFile, PercorsoImportArchivioOK)
    End Sub

    Friend Sub SpostareFileInArchivioERR(fileImport As String, nomeFile As String)
        SpostareFileInArchivio(fileImport, nomeFile, PercorsoImportArchivioERR)
    End Sub

    Friend Sub SpostareFileInArchivio(fileImport As String, nomeFile As String, percorsoArchivio As String)
        Dim estensione = Path.GetExtension(nomeFile)
        Dim nomeFileSenzaEstensione = Path.GetFileNameWithoutExtension(nomeFile)
        Dim suffissoDataOra = DateTime.Now.ToString("_yyyyMMdd_HHmmss")
        Dim nomeFileAssoluto As String = Path.Combine(percorsoArchivio, nomeFileSenzaEstensione & suffissoDataOra & estensione)
        If File.Exists(nomeFileAssoluto) Then
            File.Delete(nomeFileAssoluto)
        End If
        File.Move(fileImport, nomeFileAssoluto)
    End Sub

    Friend Sub SpostareFileDaTmpInArchivio(nomeFileAssolutoEsportTMP As String, nomeFile As String)
        Dim nomeFileAssoluto As String = Path.Combine(PercorsoEsport, nomeFile)
        If File.Exists(nomeFileAssoluto) Then
            If StoricoAbilitato Then
                Dim estensione = Path.GetExtension(nomeFileAssoluto)
                Dim nomeFileSenzaEstensione = Path.GetFileNameWithoutExtension(nomeFileAssoluto)
                Dim suffissoDataOra = DateTime.Now.ToString("_yyyyMMdd_HHmmss")
                Dim nomeFileAssolutoStorico As String = Path.Combine(PercorsoEsportArchivioStorico, nomeFileSenzaEstensione & suffissoDataOra & estensione)
                File.Move(nomeFileAssoluto, nomeFileAssolutoStorico)
            Else
                File.Delete(nomeFileAssoluto)
            End If
        End If
        File.Move(nomeFileAssolutoEsportTMP, nomeFileAssoluto)
    End Sub

    Friend Function OttieneNomeFile(fileImport As String) As Object
        Return Path.GetFileName(fileImport)
    End Function

    Friend Function OttieneNomeFileAssolutoEsport(fileEsport As String) As String
        Return Path.Combine(PercorsoEsport, fileEsport)
    End Function

    Friend Function OttieneNomeFileAssolutoEsportTMP(fileEsport As String) As String
        Return Path.Combine(PercorsoEsportArchivioTMP, fileEsport)
    End Function

    Friend Function ScriviRigaFile(file As String, riga As String, accoda As Boolean, Optional encoding As Encoding = Nothing) As String
        Return InnerScriviRigaFile(file, riga, accoda, encoding)
    End Function

    Private Function InnerScriviRigaFile(file As String, riga As String, accoda As Boolean, Optional encoding As Encoding = Nothing) As String

        Dim errore As String = String.Empty
        Dim fileStreamWriter As StreamWriter = Nothing
        Try

            If Not IsNothing(encoding) Then
                Dim enc As Encoding = Nothing
                If encoding.GetType Is GetType(UTF8Encoding) Then
                    enc = New UTF8Encoding(False)
                ElseIf encoding.GetType Is GetType(UnicodeEncoding) Then
                    enc = New UnicodeEncoding(True, True)
                Else
                    enc = encoding
                End If
                fileStreamWriter = New StreamWriter(file, accoda, enc)
            Else
                fileStreamWriter = OpenTextFileWriter(file, accoda)
            End If
            fileStreamWriter.WriteLine(riga)
            fileStreamWriter.Close()
        Catch ex As Exception
            errore = ex.Message
        End Try
        Return errore

    End Function


    Friend Function GetPercorsoEsport() As String
        Return PercorsoEsport
    End Function

End Class
