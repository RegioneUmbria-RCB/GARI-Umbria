Imports System.IO
Imports System.Xml
Imports AgronicaCoreDataProvider
Imports GiasDuplicateResx

Public Class FileResxManager
    Implements IFileResxManager
    Private ReadOnly _path As String
    Private ReadOnly _logService As ILogService
    Private blockSize As Integer = 1000
    Private objParametriDBAppoggio As AgronicaCoreParametri

    Public Sub New(ByVal path As String, logService As ILogService, objParametriDBAppoggio As AgronicaCoreParametri)
        Me._path = path
        Me._logService = logService
        Me.objParametriDBAppoggio = objParametriDBAppoggio
    End Sub

    Public Function GetAllFilesResx() As List(Of FileResx) Implements IFileResxManager.GetAllFilesResx
        Dim resxFiles As New List(Of FileResx)()
        GetAllResxFiles(_path, resxFiles)
        Return resxFiles
    End Function

    Private Sub GetAllResxFiles(ByVal directoryPath As String, ByRef resxFiles As List(Of FileResx))
        ' Ottiene i file .resx nella directory corrente e le sottodirectory
        Try
            For Each file As String In Directory.GetFiles(directoryPath, "*.resx")
                Dim fileName As String = Path.GetFileNameWithoutExtension(file)
                Dim directoryName As String = Path.GetDirectoryName(file)
                Dim languages As String() = {"pt", "fr", "en"}

                If languages.All(Function(lang) System.IO.File.Exists(Path.Combine(directoryName, $"{fileName}.{lang}.resx"))) Then
                    resxFiles.Add(New FileResx(Path.GetFileName(file), file))
                End If
            Next

            For Each subDirectory As String In Directory.GetDirectories(directoryPath)
                GetAllResxFiles(subDirectory, resxFiles) ' Chiamata ricorsiva per le sottodirectory
            Next
        Catch ex As Exception
            Console.WriteLine($"Errore durante la scansione della directory {directoryPath}: {ex.Message}")
        End Try
    End Sub

    Public Function GetKeyValueFromResx(files As List(Of FileResx)) As List(Of ResxData) Implements IFileResxManager.GetKeyValueFromResx
        Dim resxDataList As New List(Of ResxData)
        Dim now As String = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff") ' Data/ora attuale

        If files Is Nothing OrElse files.Count = 0 Then
            Console.WriteLine("La lista dei file è vuota o nulla.")
            Return resxDataList
        End If

        Try
            For Each file As FileResx In files
                If Not System.IO.File.Exists(file.Path) Then
                    Console.WriteLine($"Il file {file.Path} non esiste.")
                    Continue For
                End If

                Dim doc As New XmlDocument()
                doc.Load(file.Path)


                Dim path As String = file.Path ' Percorso completo del file scansito

                For Each node As XmlNode In doc.SelectNodes("//data")
                    Dim key As String = node.Attributes("name").Value ' Chiave
                    Dim value As String = node.SelectSingleNode("value").InnerText ' Valore
                    resxDataList.Add(New ResxData(now, path, key, value))
                Next
                Threading.Thread.Sleep(100)
            Next
        Catch ex As Exception
            Console.WriteLine($"Errore durante l'analisi del file: {ex.Message}")
        End Try

        Return resxDataList
    End Function
End Class
