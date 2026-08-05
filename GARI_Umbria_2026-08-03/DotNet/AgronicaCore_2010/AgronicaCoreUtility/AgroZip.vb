Imports System.IO
Imports System.IO.Compression
Imports System.IO.Packaging

Public Class AgroZip

#Region "Gestione ZIP (con System.IO.Compression)"

    ''' <summary>
    ''' Comprime una cartella in un file zip.
    ''' Se il file zip non esiste, viene creato ex-novo con tutti i file e sottocartelle della cartella di origine.
    ''' Se il file zip esiste già, aggiunge (merge) i file e le sottocartelle della cartella di origine allo zip esistente.
    ''' I file con lo stesso percorso relativo vengono sovrascritti nello zip.
    ''' La struttura delle sottocartelle viene mantenuta.
    ''' Utilizza System.IO.Compression; non vengono salvati attributi avanzati dei file (solo data modifica).
    ''' </summary>
    ''' <param name="zipFilePath">Percorso completo del file zip da creare o aggiornare.</param>
    ''' <param name="cartellaDaComprimere">Percorso della cartella da comprimere.</param>
    Public Shared Sub ZipAFolder(ByVal zipFilePath As String, ByVal cartellaDaComprimere As String)

        If Not Directory.Exists(cartellaDaComprimere) Then 
            Exit Sub
        End If

        If Not File.Exists(zipFilePath)
            ' Se lo zip non esiste, crea da zero
            ZipFile.CreateFromDirectory(cartellaDaComprimere, zipFilePath, CompressionLevel.Optimal, False)
        Else
            ' Se lo zip esiste, aggiungi i file della cartella
            Using archive As ZipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Update)
                Dim basePathLength As Integer = cartellaDaComprimere.Length
                If Not cartellaDaComprimere.EndsWith(Path.DirectorySeparatorChar) Then
                    basePathLength += 1
                End If

                For Each filePath In Directory.GetFiles(cartellaDaComprimere, "*", SearchOption.AllDirectories)
                    Dim relativePath = filePath.Substring(basePathLength).Replace("\", "/")

                    ' Rimuovi eventuale file già presente con lo stesso nome
                    Dim entry = archive.GetEntry(relativePath)
                    If entry IsNot Nothing Then 
                        entry.Delete()
                    End If

                    archive.CreateEntryFromFile(filePath, relativePath, CompressionLevel.Optimal)
                Next
            End Using

        End If

    End Sub

    ''' <summary>
    ''' Aggiunge un singolo file a un archivio zip.
    ''' Se lo zip non esiste, viene creato. Se esiste, il file viene aggiunto o sovrascritto in base al parametro 'sovrascriviZip'.
    ''' È possibile specificare una directory interna nello zip.
    ''' Utilizza System.IO.Compression; non vengono gestiti attributi avanzati dei file.
    ''' </summary>
    ''' <param name="zipFileName">Percorso completo del file zip da creare o aggiornare.</param>
    ''' <param name="filePath">Percorso del file da aggiungere.</param>
    ''' <param name="directoryNelZip">Directory interna nello zip (opzionale).</param>
    ''' <param name="sovrascriviZip">Se True, lo zip viene ricreato da zero; se False, il file viene aggiunto/aggiornato (merge).</param>
    Public Shared Sub AggiungiFileAZip(ByRef zipFileName As String, filePath As String, Optional directoryNelZip As String = "", Optional sovrascriviZip As Boolean = False)
        AggiungiFileAZipInterno(zipFileName, New List(Of String) From {filePath}, directoryNelZip, sovrascriviZip)
    End Sub

    ''' <summary>
    ''' Aggiunge una lista di file a un archivio zip.
    ''' Se lo zip non esiste, viene creato. Se esiste, i file vengono aggiunti o sovrascritti in base al parametro 'sovrascriviZip'.
    ''' È possibile specificare una directory interna nello zip.
    ''' Utilizza System.IO.Compression; non vengono gestiti attributi avanzati dei file.
    ''' </summary>
    ''' <param name="zipFileName">Percorso completo del file zip da creare o aggiornare.</param>
    ''' <param name="filePaths">Lista dei percorsi dei file da aggiungere.</param>
    ''' <param name="directoryNelZip">Directory interna nello zip (opzionale).</param>
    ''' <param name="sovrascriviZip">Se True, lo zip viene ricreato da zero; se False, i file vengono aggiunti/aggiornati (merge).</param>
    Public Shared Sub AggiungiPiuFileAZip(ByRef zipFileName As String, filePaths As List(Of String), Optional directoryNelZip As String = "", Optional sovrascriviZip As Boolean = False)
        AggiungiFileAZipInterno(zipFileName, filePaths, directoryNelZip, sovrascriviZip)
    End Sub

    ''' <summary>
    ''' Estrae il contenuto di un file zip nella cartella specificata.
    ''' Per ogni file, epura il nome da eventuali caratteri speciali (ChrW(15), "Shift In") prima di estrarlo.
    ''' Viene mantenuta la struttura delle sottocartelle.
    ''' I file estratti sovrascrivono quelli eventualmente già presenti nella destinazione.
    ''' Utilizza System.IO.Compression; non vengono ripristinati attributi avanzati dei file (solo data modifica).
    ''' </summary>
    ''' <param name="zipFilePath">Percorso completo del file zip da estrarre.</param>
    ''' <param name="unZipFolderLocation__1">Percorso della cartella di destinazione per l'estrazione.</param>
    Public Shared Sub UnZip(ByVal zipFilePath As String, ByVal unZipFolderLocation__1 As String)

        ' Definisco una soglia di ratio di compressione per evitare zip bomb
        ' (deve essere abbastanza alto perché alcuni shapefile hanno una compression ratio di 70)
        Const ThresholdRatio As Double = 100

        If String.IsNullOrEmpty(zipFilePath) OrElse String.IsNullOrEmpty(unZipFolderLocation__1) Then 
            Exit Sub
        End If

        If Not File.Exists(zipFilePath) Then 
            Exit Sub
        End If

        ' Assicura che la cartella di destinazione termini con il separatore corretto
        If Not unZipFolderLocation__1.EndsWith(Path.DirectorySeparatorChar) Then
            unZipFolderLocation__1 &= Path.DirectorySeparatorChar
        End If

        Using archive As ZipArchive = ZipFile.OpenRead(zipFilePath)

            ' Calcola la dimensione totale non compressa
            Dim totalUncompressedSize As Long = 0
            For Each entry As ZipArchiveEntry In archive.Entries
                totalUncompressedSize += entry.Length
            Next

            ' Calcola la ratio di compressione
            Dim zipFileSize As Long = New FileInfo(zipFilePath).Length
            Dim compressionRatio As Double = If(zipFileSize = 0, Double.MaxValue, totalUncompressedSize / zipFileSize)

            ' SECURITY - Controllo Zip Bomb
            If compressionRatio > ThresholdRatio Then
                Throw New InvalidOperationException($"SECURITY - Estrazione bloccata: la ratio di compressione ({compressionRatio:F2}) supera il limite consentito imposto per evitare Zip Bombing ({ThresholdRatio}).")
            End If


            For Each entry As ZipArchiveEntry In archive.Entries
                ' Epura il nome del file da caratteri speciali (Shift IN)
                Dim cleanName As String = entry.FullName.Replace(ChrW(15), "")

                ' Costruisci il percorso di destinazione
                Dim destinationPath As String = Path.Combine(unZipFolderLocation__1, cleanName)

                ' SECURITY - Verifica che il percorso di destinazione sia all'interno della cartella di destinazione specificata
                Dim canonicalDestinationPath As String = Path.GetFullPath(destinationPath)
                If Not canonicalDestinationPath.StartsWith(unZipFolderLocation__1, StringComparison.Ordinal) Then
                    Throw New InvalidOperationException($"SECURITY - Estrazione bloccata: il percorso di destinazione tenta di uscire dalla cartella di destinazione [{unZipFolderLocation__1}] specificata.")
                End If

                ' Crea la directory se necessario
                Dim destinationDir As String = Path.GetDirectoryName(destinationPath)
                If Not Directory.Exists(destinationDir) Then
                    Directory.CreateDirectory(destinationDir)
                End If

                ' Se l'entry non è una directory, estrai il file
                If Not String.IsNullOrEmpty(entry.Name) Then
                    entry.ExtractToFile(destinationPath, True)
                End If
            Next
        End Using

    End Sub

    ''' <summary>
    ''' Logica centralizzata per aggiungere uno o più file a un archivio zip.
    ''' Se 'sovrascriviZip' è True, lo zip viene ricreato da zero. Se False, i file vengono aggiunti o sovrascritti (merge).
    ''' La directory interna nello zip può essere specificata.
    ''' Utilizza System.IO.Compression; non vengono gestiti attributi avanzati dei file.
    ''' </summary>
    ''' <param name="zipFileName">Percorso completo del file zip da creare o aggiornare.</param>
    ''' <param name="filePaths">Collezione dei percorsi dei file da aggiungere.</param>
    ''' <param name="directoryNelZip">Directory interna nello zip (opzionale).</param>
    ''' <param name="sovrascriviZip">Se True, lo zip viene ricreato da zero; se False, i file vengono aggiunti/aggiornati (merge).</param>
    Private Shared Sub AggiungiFileAZipInterno(zipFileName As String,
                                               filePaths As List(Of String),
                                               Optional directoryNelZip As String = "",
                                               Optional sovrascriviZip As Boolean = False)

        Dim destFolder = Path.GetDirectoryName(zipFileName)
        If Not Directory.Exists(destFolder) Then
            Directory.CreateDirectory(destFolder)
        End If

        ' Se richiesto, sovrascrivi lo zip esistente
        If sovrascriviZip AndAlso File.Exists(zipFileName) Then
            File.Delete(zipFileName)
        End If

        Dim zipMode As ZipArchiveMode = If(File.Exists(zipFileName), ZipArchiveMode.Update, ZipArchiveMode.Create)

        'zipMode = Update : aggiungi/sovrascrivi il file nello zip esistente

        Using archive As ZipArchive = ZipFile.Open(zipFileName, zipMode)
            For Each filePath In filePaths
                If File.Exists(filePath) Then
                    Dim entryName As String
                    If String.IsNullOrEmpty(directoryNelZip) Then
                        entryName = Path.GetFileName(filePath)
                    Else
                        entryName = Path.Combine(directoryNelZip, Path.GetFileName(filePath)).Replace("\", "/")
                    End If

                    '(Solo in modalità Update) Nel caso esista già lo stesso file all'interno dello zip sovrascrivilo
                    If zipMode = ZipArchiveMode.Update Then
                        Dim entry = archive.GetEntry(entryName)
                        If entry IsNot Nothing Then 
                            entry.Delete()
                        End If
                    End If

                    archive.CreateEntryFromFile(filePath, entryName, CompressionLevel.Optimal)
                End If
            Next
        End Using

    End Sub


#End Region

    ''' <summary>
    ''' Method to create file at the temp folder
    ''' </summary>
    ''' <param name="rootFolder"></param>
    ''' <param name="contentFileURI"></param>
    ''' <returns></returns>
    Protected Shared Sub createFile(rootFolder As String, contentFile As System.IO.Packaging.ZipPackagePart)
        ' Initially create file under the folder specified
        Dim contentFilePath As String = String.Empty
        contentFilePath = contentFile.Uri.OriginalString.Replace("/"c, Path.DirectorySeparatorChar)

        If contentFilePath.StartsWith(Path.DirectorySeparatorChar.ToString()) Then
            contentFilePath = contentFilePath.TrimStart(Path.DirectorySeparatorChar)
        Else
            'do nothing
        End If

        contentFilePath = Path.Combine(rootFolder, contentFilePath)

        'Check for the folder already exists. If not then create that folder

        If Not Directory.Exists(Path.GetDirectoryName(contentFilePath)) Then
            Directory.CreateDirectory(Path.GetDirectoryName(contentFilePath))
        Else
            'do nothing
        End If

        Dim newFileStream As FileStream = File.Create(contentFilePath)
        newFileStream.Close()
        Dim content As Byte() = New Byte(contentFile.GetStream().Length - 1) {}
        contentFile.GetStream().Read(content, 0, content.Length)
        File.WriteAllBytes(contentFilePath, content)

    End Sub


    Private Const BUFFER_SIZE As Long = 4096


    Public Shared Sub AddFileToZip(zipFilename As String, fileToAdd As String, Optional ByVal AbsolutePath As String = "")
        Using zip As Package = System.IO.Packaging.Package.Open(zipFilename, FileMode.OpenOrCreate)

            Dim destFilename As String
            If AbsolutePath = "" Then
                destFilename = ".\" & Path.GetFileName(fileToAdd)
            Else
                destFilename = "." & fileToAdd.Replace(AbsolutePath, "")
            End If


            Dim uri As Uri = PackUriHelper.CreatePartUri(New Uri(destFilename, UriKind.Relative))
            If zip.PartExists(uri) Then
                zip.DeletePart(uri)
            End If

            Dim part As PackagePart = zip.CreatePart(uri, "", CompressionOption.Normal)
            Using fileStream As New FileStream(fileToAdd, FileMode.Open, FileAccess.Read)
                Using dest As Stream = part.GetStream()
                    CopyStream(fileStream, dest)
                End Using
            End Using
        End Using


    End Sub

    Private Shared Sub CopyStream(inputStream As FileStream, outputStream As Stream)
        Dim bufferSize As Long = If(inputStream.Length < BUFFER_SIZE, inputStream.Length, BUFFER_SIZE)
        Dim buffer As Byte() = New Byte(bufferSize - 1) {}
        Dim bytesRead As Integer = 0
        Dim bytesWritten As Long = 0


        bytesRead = inputStream.Read(buffer, 0, buffer.Length)
        While (bytesRead) <> 0
            outputStream.Write(buffer, 0, bytesRead)
            bytesWritten += bufferSize
            bytesRead = inputStream.Read(buffer, 0, buffer.Length)
        End While

    End Sub

    '  Marco Grilli, 17/06/2014 12:48:51: converte un byte array in una stringa base64
    Private Shared Function ByteArrayToStrBASE64(ByVal byteArray As Byte()) As String
        Return Convert.ToBase64String(byteArray)
    End Function

    '  Marco Grilli, 17/06/2014 12:48:51: converte una stringa base64 in un byte array
    Private Shared Function StrBASE64ToByteArray(ByVal str As String) As Byte()
        Return Convert.FromBase64String(str)
    End Function

#Region "StrToByteArray"

    ' VB.NET to convert a string to a byte array
    Public Shared Function StrToByteArray(ByVal str As String) As Byte()
        'Dim encoding As New System.Text.ASCIIEncoding()
        Dim encoding As New System.Text.UnicodeEncoding()
        Return encoding.GetBytes(str)
    End Function

    ' VB.NET to convert a string to a byte array
    Public Shared Function StrToByteArrayASCII(ByVal str As String) As Byte()
        Dim encoding As New System.Text.ASCIIEncoding()        
        Return encoding.GetBytes(str)
    End Function

    ' VB.NET to convert a string to a byte array
    Public Shared Function StrToByteArrayUTF8(ByVal str As String) As Byte()
        Dim encoding As New System.Text.UTF8Encoding()        
        Return encoding.GetBytes(str)
    End Function
    
    ' VB.NET to convert a string to a byte array
    Public Shared Function StrToByteArrayUTF32(ByVal str As String) As Byte()
        Dim encoding As New System.Text.UTF32Encoding()        
        Return encoding.GetBytes(str)
    End Function

#End Region

#Region "ByteArrayToStr"

    Public Shared Function ByteArrayToStr(ByVal byteArray As Byte(), Optional ByVal removeBOM As Boolean = False) As String        
        Dim myEncoding As New System.Text.UnicodeEncoding()
        Dim stringDati As String = myEncoding.GetString(byteArray)

        If removeBOM Then
            Dim byteOrderMarkUnicode As String = Text.Encoding.Unicode.GetString(Text.Encoding.Unicode.GetPreamble())
            If (stringDati.StartsWith(byteOrderMarkUnicode, StringComparison.Ordinal)) Then
                stringDati = stringDati.Remove(0, byteOrderMarkUnicode.Length)
            End If
        End If

        Return stringDati
    End Function

    Public Shared Function ByteArrayToStrASCII(ByVal byteArray As Byte()) As String
        Dim myEncoding As New System.Text.ASCIIEncoding()        
        Return myEncoding.GetString(byteArray)
    End Function

    Public Shared Function ByteArrayToStrUTF32(ByVal byteArray As Byte(), Optional ByVal removeBOM As Boolean = False) As String
        Dim myEncoding As New System.Text.UTF32Encoding()
        Dim stringDati As String = myEncoding.GetString(byteArray)

        If removeBOM Then
            Dim byteOrderMarkUtf32 As String = Text.Encoding.UTF32.GetString(Text.Encoding.UTF32.GetPreamble())
            If (stringDati.StartsWith(byteOrderMarkUtf32, StringComparison.Ordinal)) Then
                stringDati = stringDati.Remove(0, byteOrderMarkUtf32.Length)
            End If
        End If

        Return stringDati
    End Function

    Public Shared Function ByteArrayToStrUTF8(ByVal byteArray As Byte(), Optional ByVal removeBOM As Boolean = False) As String
        Dim myEncoding As New System.Text.UTF8Encoding()
        Dim stringDati As String = myEncoding.GetString(byteArray)

        If removeBOM Then
            Dim byteOrderMarkUtf8 As String = Text.Encoding.UTF8.GetString(Text.Encoding.UTF8.GetPreamble())
            If (stringDati.StartsWith(byteOrderMarkUtf8, StringComparison.Ordinal)) Then
                stringDati = stringDati.Remove(0, byteOrderMarkUtf8.Length)
            End If
        End If

        Return stringDati
    End Function

#End Region
    
    '################################################################################################
    Public Shared Function Compressione(ByVal ZipMode As Byte, ByVal VettoreByteIn() As Byte) As Byte()

        Dim MemStream As New MemoryStream
        Dim ZipStream As Stream = Nothing

        'Verifico l'algoritmo di compressione richiesto
        Select Case ZipMode

            Case 0  '----- Nessuno --------------------------------
                Return VettoreByteIn

            Case 1  '----- GZip -----------------------------------
                ZipStream = New GZipStream(MemStream, CompressionMode.Compress, True)
                ZipStream.Write(VettoreByteIn, 0, VettoreByteIn.Length)
                ZipStream.Close()
                MemStream.Position = 0
                Dim VettoreByteOut(MemStream.Length - 1) As Byte
                MemStream.Read(VettoreByteOut, 0, MemStream.Length)
                Return VettoreByteOut

            Case 2  '----- Deflate --------------------------------
                ZipStream = New DeflateStream(MemStream, CompressionMode.Compress, True)
                ZipStream.Write(VettoreByteIn, 0, VettoreByteIn.Length)
                ZipStream.Close()
                MemStream.Position = 0
                Dim VettoreByteOut(MemStream.Length - 1) As Byte
                MemStream.Read(VettoreByteOut, 0, MemStream.Length)
                Return VettoreByteOut

            Case Else '--- Non definito ---------------------------
                Return VettoreByteIn

        End Select

    End Function

    ''' <summary>
    ''' Ritorna una stringa codificata in Base64 ma ne prende una tradizionale
    ''' </summary>
    ''' <param name="ZipMode">Modalità compressione: 0 = Nessuna, 1 = GZIP, 2 = Deflate</param>
    ''' <param name="StringIn">Stringa in input tradizionale</param>
    ''' <param name="enc">(Opzionale) Permette di specificare il tipo di stringa arrivata (Unicode, UTF8, ...). Default = Unicode</param>
    Public Shared Function CompressioneBase64(ByVal ZipMode As Byte,
                                              ByVal StringIn As String,
                                              Optional ByVal enc As Text.Encoding = Nothing
                                              ) As String

        '  Marco Grilli, 17/06/2014 12:37:56: converto la stringa in un array di byte
        Dim byteArray As Byte()

        If enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.UTF8.GetType() Then
            byteArray = StrToByteArrayUTF8(StringIn)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.Unicode.GetType() Then
            byteArray = StrToByteArray(StringIn)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.UTF32.GetType() Then
            byteArray = StrToByteArrayUTF32(StringIn)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.ASCII.GetType() Then
            byteArray = StrToByteArrayASCII(StringIn)
        Else
            byteArray = StrToByteArray(StringIn)
        End If

        '  Marco Grilli, 17/06/2014 12:37:56: comprimo come al solito
        Dim byteOut As Byte() = Compressione(ZipMode, byteArray)

        '  Marco Grilli, 17/06/2014 12:40:31: ritorno l'array convertito in stringa Base64
        Return ByteArrayToStrBASE64(byteOut)

    End Function

    Public Shared Function CompressioneBase64PerJS(ByVal ZipMode As Byte,
                                              ByVal StringIn As String,
                                              Optional ByVal enc As Text.Encoding = Nothing
                                              ) As Byte()

        '  Marco Grilli, 17/06/2014 12:37:56: converto la stringa in un array di byte
        Dim byteArray As Byte()

        If enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.UTF8.GetType() Then
            byteArray = StrToByteArrayUTF8(StringIn)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.Unicode.GetType() Then
            byteArray = StrToByteArray(StringIn)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.UTF32.GetType() Then
            byteArray = StrToByteArrayUTF32(StringIn)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.ASCII.GetType() Then
            byteArray = StrToByteArrayASCII(StringIn)
        Else
            byteArray = StrToByteArray(StringIn)
        End If

        '  Marco Grilli, 17/06/2014 12:37:56: comprimo come al solito
        Dim byteOut As Byte() = Compressione(ZipMode, byteArray)

        '  Marco Grilli, 17/06/2014 12:40:31: ritorno l'array convertito in stringa Base64
        Return byteOut

    End Function

    ''' <summary>
    ''' Ritorna una stringa tradizionale, ma ne prende una codificata in Base64
    ''' </summary>
    ''' <param name="ZipMode">Modalità compressione: 0 = Nessuna, 1 = GZIP, 2 = Deflate</param>
    ''' <param name="arrayByte">arrayByte con contenuto compresso
    ''' <param name="enc">(Opzionale) Permette di specificare il tipo di stringa arrivata (Unicode, UTF8, ...). Default = Unicode</param>
    ''' <param name="removeBOM">(Opzionale) Flag per forzare rimozione di Byte Order Mark se presente, per avere in output stringa pulita di questi byte iniziali</param>
    Public Shared Function DeCompressioneBase64PerJs(ByVal ZipMode As Byte,
                                                ByVal arrayByte As Byte(),
                                                Optional ByVal enc As Text.Encoding = Nothing,
                                                Optional ByVal removeBOM As Boolean = False
                                                ) As String

        '  Marco Grilli, 17/06/2014 12:37:56: decomprimo come al solito
        Dim byteOut As Byte() = DeCompressione(ZipMode, arrayByte)

        If enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.UTF8.GetType() Then
            Return ByteArrayToStrUTF8(byteOut, removeBOM)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.Unicode.GetType() Then
            Return ByteArrayToStr(byteOut, removeBOM)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.UTF32.GetType() Then
            Return ByteArrayToStrUTF32(byteOut, removeBOM)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.ASCII.GetType() Then
            Return ByteArrayToStrASCII(byteOut)
        Else
            '  Marco Grilli, 17/06/2014 12:40:31: ritorno l'array convertito in stringa tradizionale
            Return ByteArrayToStr(byteOut)
        End If

    End Function


    '################################################################################################
    Public Shared Function DeCompressione(ByVal ZipMode As Byte, ByVal VettoreByteIn() As Byte) As Byte()

        Dim MemStream As New MemoryStream(VettoreByteIn)
        Dim ZipStream As Stream = Nothing

        'Verifico l'algoritmo di compressione richiesto
        Select Case ZipMode

            Case 0  '----- Nessuno --------------------------------
                Return VettoreByteIn

            Case 1  '----- GZip -----------------------------------
                ZipStream = New GZipStream(MemStream, CompressionMode.Decompress, True)
                Dim VettoreByteOut() As Byte
                VettoreByteOut = RetrieveBytesFromStream(ZipStream, VettoreByteIn.Length)
                Return VettoreByteOut

            Case 2  '----- Deflate --------------------------------
                ZipStream = New DeflateStream(MemStream, CompressionMode.Decompress, True)
                Dim VettoreByteOut() As Byte
                VettoreByteOut = RetrieveBytesFromStream(ZipStream, VettoreByteIn.Length)
                Return VettoreByteOut

            Case Else '--- Non definito ---------------------------
                Return VettoreByteIn

        End Select

    End Function

    ''' <summary>
    ''' Ritorna una stringa tradizionale, ma ne prende una codificata in Base64
    ''' </summary>
    ''' <param name="ZipMode">Modalità compressione: 0 = Nessuna, 1 = GZIP, 2 = Deflate</param>
    ''' <param name="StringIn">Stringa in input in Base64</param>
    ''' <param name="enc">(Opzionale) Permette di specificare il tipo di stringa arrivata (Unicode, UTF8, ...). Default = Unicode</param>
    ''' <param name="removeBOM">(Opzionale) Flag per forzare rimozione di Byte Order Mark se presente, per avere in output stringa pulita di questi byte iniziali</param>
    Public Shared Function DeCompressioneBase64(ByVal ZipMode As Byte,
                                                ByVal StringIn As String,
                                                Optional ByVal enc As Text.Encoding = Nothing,
                                                Optional ByVal removeBOM As Boolean = False
                                                ) As String

        '  Marco Grilli, 17/06/2014 12:37:56: converto la stringa base64 in un array di byte
        Dim byteArray As Byte() = StrBASE64ToByteArray(StringIn)
        '  Marco Grilli, 17/06/2014 12:37:56: decomprimo come al solito
        Dim byteOut As Byte() = DeCompressione(ZipMode, byteArray)

        If enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.UTF8.GetType() Then
            Return ByteArrayToStrUTF8(byteOut, removeBOM)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.Unicode.GetType() Then
            Return ByteArrayToStr(byteOut, removeBOM)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.UTF32.GetType() Then
            Return ByteArrayToStrUTF32(byteOut, removeBOM)
        ElseIf enc IsNot Nothing AndAlso enc.GetType() = Text.Encoding.ASCII.GetType() Then
            Return ByteArrayToStrASCII(byteOut)
        Else
            '  Marco Grilli, 17/06/2014 12:40:31: ritorno l'array convertito in stringa tradizionale
            Return ByteArrayToStr(byteOut)
        End If

    End Function

    Public Shared Function DeCompressioneBase64Byte(ByVal ZipMode As Byte, ByVal StringIn As String) As Byte()

        '  Marco Grilli, 17/06/2014 12:37:56: converto la stringa base64 in un array di byte
        Dim byteArray As Byte() = StrBASE64ToByteArray(StringIn)
        '  Marco Grilli, 17/06/2014 12:37:56: decomprimo come al solito
        Dim byteOut As Byte() = DeCompressione(ZipMode, byteArray)

        '  Marco Grilli, 17/06/2014 12:40:31: ritorno l'array di byte
        Return byteOut

    End Function

    '################################################################################################
    ''' <summary>
    ''' ---retrieve the bytes from a stream object---
    ''' </summary>
    Public Shared Function RetrieveBytesFromStream(ByVal stream As Stream, ByVal bytesblock As Integer) As Byte()

        Const nomeRoutine = "AgroZip.RetrieveBytesFromStream"
        Dim data() As Byte
        Dim totalCount As Integer = 0
        Try
            While True
                '---progressively increase the size of the data byte array---
                ReDim Preserve data(totalCount + bytesblock)
                Dim bytesRead As Integer = stream.Read(data, totalCount, bytesblock)
                If bytesRead = 0 Then
                    Exit While
                End If
                totalCount += bytesRead
            End While
            '---make sure the byte array contains exactly the number 
            ' of bytes extracted---
            ReDim Preserve data(totalCount - 1)
            Return data
        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Function

    '###############################################################################################
    '###############################################################################################
    '###############################################################################################
    '###############################################################################################
    '###############################################################################################


End Class
