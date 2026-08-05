Imports System.IO
Imports System.IO.Compression
Imports System.Security.Cryptography
Imports Microsoft.Build.Framework
Imports Microsoft.Build.Utilities
Imports NUglify
Imports NUglify.Css
Imports NUglify.JavaScript

Public Class MinifyWebAssets
    Inherits Task

    <Required>
    Public Property WebDir As ITaskItem

    Public Overrides Function Execute() As Boolean

        'TODO: DEBUG: Decommentare questa riga per debuggare
        'System.Diagnostics.Debugger.Launch()

        Dim success As Boolean = True

        If String.IsNullOrEmpty(WebDir.ItemSpec) Then
            Log.LogError("[{0}] La Web Directory non è stata indicata",
                         Me.GetType().FullName)
            success = False
        Else If Not Directory.Exists(WebDir.ItemSpec) Then
            Log.LogError("[{0}] La Web Directory {1} non esiste",
                         Me.GetType().FullName, WebDir.ItemSpec)
            success = False
        Else
            success = success AndAlso MinifyJavaScripts(False, True, False)
            'success = success AndAlso MinifyCascadingStyleSheets(False, True, False)
        End If

        Return success

    End Function

    Private Function MinifyJavaScripts(ByVal addMD5 As Boolean, ByVal deletePreventivamente As Boolean, ByVal gzip As Boolean) As Boolean

        Const nomeRoutine = "MinifyJavaScripts"
        Dim success As Boolean = True
        Dim conta As New ContatoriFile

        'Escludo i file già minificati
        'Escludo la cartella Scripts (e sottocartelle) dove ci sono librerie di terze parti
        'Escludo la cartella obj (e sottocartelle) -- questo succede solo in locale, perché la cartella dei sorgenti e di destinazione sono la stessa
        Dim files = (From chkFile As FileInfo In New DirectoryInfo(WebDir.ItemSpec).EnumerateFiles("*.js", SearchOption.AllDirectories)
                    Where Not chkFile.Name.EndsWith(".min.js") AndAlso
                          Not chkFile.DirectoryName = (WebDir.ItemSpec & "\Scripts") AndAlso
                          Not chkFile.DirectoryName.StartsWith(WebDir.ItemSpec & "\Scripts\") AndAlso
                          Not chkFile.DirectoryName = (WebDir.ItemSpec & "\obj") AndAlso
                          Not chkFile.DirectoryName.StartsWith(WebDir.ItemSpec & "\obj\")
                    Select chkFile.FullName).ToList()

        Dim settings As New CodeSettings With {
            .LocalRenaming = LocalRenaming.CrunchAll,
            .AlwaysEscapeNonAscii = False,
            .PreserveFunctionNames = True,
            .PreserveImportantComments = False,
            .EvalTreatment = EvalTreatment.MakeAllSafe,
            .RemoveUnneededCode = False
        }

        '.ScriptVersion = ScriptVersion.EcmaScript6,
        '.AlwaysEscapeNonAscii = True,
        '.ReorderScopeDeclarations = False,

        conta.FileTotali = files.Count()

        If deletePreventivamente Then
            conta.FileCancellati = CleanAllExistingCompressedFiles(WebDir.ItemSpec, ".js")
        End If

        For Each file As String In files

            Try
                Dim source As String = IO.File.ReadAllText(file)

                If source.Length > 0 Then

                    If Not deletePreventivamente Then
                        CleanExistingCompressedFiles(file, addMD5)
                        conta.FileCancellati += 1
                    End If

                    Dim uglifyResult As UglifyResult = Uglify.Js(source, settings)
                    Dim compressedFileContents As String = Trim(If(uglifyResult.Code, ""))

                    If uglifyResult.HasErrors Then
                        conta.FileErroreSintassi += 1
                        success = False 'Faccio fallire la compilazione
                        Dim numErrori As Integer = uglifyResult.Errors.Count
                        Log.LogError("[{0}] File [{1}] has {2} errors", nomeRoutine, file, numErrori)

                        For Each erMes In uglifyResult.Errors
                            Log.LogError("[{0}] File [{1}] error minify: {2}", nomeRoutine, file, erMes.ToString())
                        Next

                    ElseIf String.IsNullOrEmpty(compressedFileContents) Then
                        conta.FileVuotiPostMin += 1
                        Log.LogWarning("[{0}] File [{1}] is empty post minify", nomeRoutine, file)
                    Else
                        Dim md5 As String = If(addMD5, "." & GetMD5Hash(compressedFileContents), "")
                        Dim compressedFilePath As String = Path.Combine(Path.GetDirectoryName(file),
                                                                        Path.GetFileNameWithoutExtension(file) & md5 & ".build.min.js")
                        IO.File.WriteAllText(compressedFilePath, compressedFileContents, New UTF8Encoding(True))
                        conta.FileMin += 1

                        If gzip Then
                            GzipFile(compressedFilePath)
                            conta.FileMinZip += 1
                        End If

                    End If
                Else
                    conta.FileVuoti += 1
                    Log.LogWarning("[{0}] File [{1}] is empty", nomeRoutine, file)
                End If

            Catch ex As Exception
                conta.FileErrore += 1
                Log.LogError("[{0}] File = [{1}]", nomeRoutine, file)
                Log.LogErrorFromException(ex, True)
                success = False
            End Try
        Next

        Log.LogMessage(MessageImportance.High, "[{0}] ---> File JS Deleted         = {1}", nomeRoutine, conta.FileCancellati)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File JS                 = {1}", nomeRoutine, conta.FileTotali)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File JS EMPTY           = {1}", nomeRoutine, conta.FileVuoti)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File JS ERRORE          = {1}", nomeRoutine, conta.FileErrore)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File JS ERRORE SINTASSI = {1}", nomeRoutine, conta.FileErroreSintassi)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File JS EMPTY POST      = {1}", nomeRoutine, conta.FileVuotiPostMin)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File JS MIN             = {1}", nomeRoutine, conta.FileMin)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File JS MIN GZIP        = {1}", nomeRoutine, conta.FileMinZip)

        Return success

    End Function

    Private Function MinifyCascadingStyleSheets(ByVal addMD5 As Boolean, ByVal deletePreventivamente As Boolean, ByVal gzip As Boolean) As Boolean

        Const nomeRoutine = "MinifyCascadingStyleSheets"
        Dim success As Boolean = True
        Dim conta As New ContatoriFile

        'Escludo i file già minificati
        'Escludo la cartella obj (e sottocartelle) -- questo succede solo in locale, perché la cartella dei sorgenti e di destinazione sono la stessa
        Dim files = (From chkFile As FileInfo In New DirectoryInfo(WebDir.ItemSpec).EnumerateFiles("*.css", SearchOption.AllDirectories)
                    Where Not chkFile.Name.EndsWith(".min.css") AndAlso
                          Not chkFile.DirectoryName = (WebDir.ItemSpec & "\obj") AndAlso
                          Not chkFile.DirectoryName.StartsWith(WebDir.ItemSpec & "\obj\")
                    Select chkFile.FullName).ToList()

        Dim settings As New CssSettings With {
            .CommentMode = CssComment.Hacks,
            .OutputMode = OutputMode.SingleLine,
            .ColorNames = CssColor.Hex
        }

        conta.FileTotali = files.Count()

        If deletePreventivamente Then
            conta.FileCancellati = CleanAllExistingCompressedFiles(WebDir.ItemSpec, ".css")
        End If

        For Each file As String In files

            Try
                Dim source As String = IO.File.ReadAllText(file)

                If source.Length > 0 Then

                    If Not deletePreventivamente Then
                        CleanExistingCompressedFiles(file, addMD5)
                        conta.FileCancellati += 1
                    End If

                    Dim uglifyResult = Uglify.Css(source, settings)
                    Dim compressedFileContents As String = Trim(If(uglifyResult.Code, ""))

                    If uglifyResult.HasErrors Then
                        conta.FileErroreSintassi += 1
                        success = False 'Faccio fallire la compilazione
                        Dim numErrori As Integer = uglifyResult.Errors.Count
                        Log.LogError("[{0}] File [{1}] has {2} errors", nomeRoutine, file, numErrori)

                        For Each erMes In uglifyResult.Errors
                            Log.LogError("[{0}] File [{1}] error minify: {2}", nomeRoutine, file, erMes.ToString())
                        Next

                    ElseIf String.IsNullOrEmpty(compressedFileContents) Then
                        conta.FileVuotiPostMin += 1
                        Log.LogWarning("[{0}] File [{1}] is empty post minify", nomeRoutine, file)
                    Else
                        Dim md5 As String = If(addMD5, "." & GetMD5Hash(compressedFileContents), "")
                        Dim compressedFilePath As String = Path.Combine(Path.GetDirectoryName(file),
                                                                        Path.GetFileNameWithoutExtension(file) & md5 & ".build.min.css")
                        IO.File.WriteAllText(compressedFilePath, compressedFileContents, New UTF8Encoding(True))
                        conta.FileMin += 1

                        If gzip Then
                            GzipFile(compressedFilePath)
                            conta.FileMinZip += 1
                        End If

                    End If
                Else
                    conta.FileVuoti += 1
                    Log.LogWarning("[{0}] File [{1}] is empty", nomeRoutine, file)
                End If

            Catch ex As Exception
                conta.FileErrore += 1
                Log.LogError("[{0}] File = [{1}]", nomeRoutine, file)
                Log.LogErrorFromException(ex, True)
                success = False
            End Try
        Next

        Log.LogMessage(MessageImportance.High, "[{0}] ---> File CSS Deleted         = {1}", nomeRoutine, conta.FileCancellati)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File CSS                 = {1}", nomeRoutine, conta.FileTotali)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File CSS EMPTY           = {1}", nomeRoutine, conta.FileVuoti)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File CSS ERRORE          = {1}", nomeRoutine, conta.FileErrore)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File CSS ERRORE SINTASSI = {1}", nomeRoutine, conta.FileErroreSintassi)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File CSS EMPTY POST      = {1}", nomeRoutine, conta.FileVuotiPostMin)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File CSS MIN             = {1}", nomeRoutine, conta.FileMin)
        Log.LogMessage(MessageImportance.High, "[{0}] ---> File CSS MIN GZIP        = {1}", nomeRoutine, conta.FileMinZip)

        Return success

    End Function

    Private Shared Sub CleanExistingCompressedFiles(ByVal filePath As String, ByVal usedMD5 As Boolean)
        Dim directory As String = Path.GetDirectoryName(filePath)
        Dim noExtension As String = Path.GetFileNameWithoutExtension(filePath)
        Dim extension As String = Path.GetExtension(filePath)

        Dim filesToDelete = IO.Directory.EnumerateFiles(directory,
                                                        noExtension & If(usedMD5, ".*", "") & ".build.min" & extension & "*",
                                                        SearchOption.TopDirectoryOnly)

        For Each fileToDelete In filesToDelete
            File.Delete(fileToDelete)
        Next
    End Sub

    Private Shared Function CleanAllExistingCompressedFiles(ByVal directory As String, ByVal extension As String) As Integer

        Dim numFileDeleted As Integer = 0
        Dim filesToDelete = IO.Directory.EnumerateFiles(directory,
                                                        "*.build.min" & extension & "*",
                                                        SearchOption.AllDirectories)

        For Each fileToDelete In filesToDelete
            File.Delete(fileToDelete)
            numFileDeleted += 1
        Next

        Return numFileDeleted
    End Function

    Private Shared Function GetMD5Hash(ByVal contents As String) As String
        Dim md5Hash As String

        Using md5 As MD5 = MD5.Create()
            md5Hash = BitConverter.ToString(md5.ComputeHash(Encoding.[Default].GetBytes(contents))).Replace("-", "").ToLower()
        End Using

        Return md5Hash
    End Function

    Private Shared Sub GzipFile(ByVal filePath As String)
        Dim gzipFilePath = filePath & ".gz"

        Using inputStream = File.OpenRead(filePath)

            Using outputStream = File.OpenWrite(gzipFilePath)

                Using gzipStream = New GZipStream(outputStream, CompressionMode.Compress)
                    'Using gzipStream = New GZipStream(outputStream, CompressionLevel.Optimal)
                    inputStream.CopyTo(gzipStream)
                End Using
            End Using
        End Using
    End Sub

    Private Class ContatoriFile
        Public Property FileCancellati As Integer
        Public Property FileTotali As Integer
        Public Property FileVuoti As Integer
        Public Property FileVuotiPostMin As Integer
        Public Property FileMin As Integer
        Public Property FileMinZip As Integer
        Public Property FileErrore As Integer
        Public Property FileErroreSintassi As Integer

        Public Sub New()
            FileCancellati = 0
            FileTotali = 0
            FileVuoti = 0
            FileVuotiPostMin = 0
            FileMin = 0
            FileMinZip = 0
            FileErrore = 0
            FileErroreSintassi = 0
       End Sub
    End Class
End Class
