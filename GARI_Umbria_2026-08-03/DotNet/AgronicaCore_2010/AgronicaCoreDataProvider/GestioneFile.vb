Public Class GestioneFile

    '################################################################################
    Public Shared Sub CancellaFiles(ByVal Percorso As String, _
                                            ByVal Pattern As String, _
                                            ByRef Str_Errore As String)

        Dim ElencoFile As String()
        Dim i As Integer

        ElencoFile = System.IO.Directory.GetFiles(Percorso, Pattern)

        'Se ci sono files che corrispondono al pattern di ricerca

        For i = 0 To ElencoFile.Length - 1

            Try

                System.IO.File.Delete(ElencoFile(i))

            Catch ex As Exception
                Str_Errore &= ex.Message
            End Try

        Next

    End Sub

    ' ################################################################################
    Public Shared Sub CancellaFilesWithoutPattern(ByVal Percorso As String)

        Dim ElencoFile As String()
        Dim i As Integer

        'Ricavo l'elenco dei file presenti in PERCORSO 

        ElencoFile = System.IO.Directory.GetFiles(Percorso)

        'Se ci sono files che corrispondono al pattern di ricerca

        For i = 0 To ElencoFile.Length - 1

            Try

                System.IO.File.Delete(ElencoFile(i))

            Catch ex As Exception

                'Se c'è un errore .....

            End Try

        Next

    End Sub

    '################################################################################
    Public Shared Sub CancellaFile(ByVal PathFileDaCancellare As String, _
                                   ByRef StrErrore As String)

        Try

            System.IO.File.Delete(PathFileDaCancellare)

        Catch ex As Exception
            StrErrore += "CancellaFile: " & ex.Message
        End Try

    End Sub


    '################################################################################
    ' usata in Esportatore_Universale_2 e Esportazione_OP_Gest
    'Crea una cartella al path specificato,
    'se la cartella esiste già, non viene fatto nulla
    <Obsolete("Usare If Not Directory.Exists(path) Then Directory.CreateDirectory(path)")>
    Public Shared Sub CreaCartellaNelPath(ByVal Path_Partenza As String,
                                            ByVal nome_cartella As String,
                                            ByRef Path_Finale As String,
                                            ByRef StrErrore As String)

        'crea la cartella nel percorso specificato
        'se la cartella è già presente si limita a entrarci.

        Dim esiste As Boolean

        If Not Path_Partenza.EndsWith("\") Then
            Path_Partenza += "\"
        End If

        'controllo se la cartella esiste già
        esiste = System.IO.Directory.Exists(Path_Partenza & nome_cartella)

        'se esiste
        If esiste = True Then

            Path_Finale = Path_Partenza & nome_cartella

            'ci vado dentro
            FileSystem.ChDir(Path_Finale)

        Else

            'la cartella non esiste, devo crearla
            If Not System.IO.Directory.Exists(Path_Partenza) Then
                FileSystem.MkDir(Path_Partenza)
            End If

            FileSystem.ChDir(Path_Partenza)

            Try

                FileSystem.MkDir(nome_cartella)

            Catch car_exc As Exception

                StrErrore = car_exc.Message.ToString
                Exit Sub

            End Try

            Path_Finale = Path_Partenza & nome_cartella

            'ci vado dentro
            FileSystem.ChDir(Path_Finale)


        End If 'esiste cartella

    End Sub

    '##############################################################################################
    'creata a partire dalla scrivilog del data provider
    Public Shared Sub CreaScriviFile(ByVal Directory As String,
                                    ByVal File As String,
                                    ByVal Testo As String)

        Dim NomeCompletoFile As String = ""
        Dim xStreamWriter As System.IO.StreamWriter = Nothing

        Try

            'Verifico se esiste la DIRECTORY  indicata ... altrimenti la creo
            If System.IO.Directory.Exists(Directory) = False Then
                System.IO.Directory.CreateDirectory(Directory)
            End If

            If Not Directory.EndsWith("\") Then
                Directory &= "\"
            End If

            'Costruisco il nome completo del file 
            NomeCompletoFile = (Directory & File).Replace("\\", "\")

            'Verifico se esiste il FILE  indicato ... altrimenti lo creo
            If System.IO.File.Exists(NomeCompletoFile) = False Then

                '  Giulia, 04/11/2016 12.00.43: Migliore gestione errore di creazione file di log,
                '                   perché il nome file (spesso contenente la ragione sociale) contiene caratteri invalidi

                Dim InvalidName As Char() = System.IO.Path.GetInvalidFileNameChars
                'Dim InvalidPath As Char() = System.IO.Path.GetInvalidPathChars
                Dim InvalidCharFound As New Hashtable

                For Each CharInvalid In InvalidName
                    If File.Contains(CharInvalid) Then
                        InvalidCharFound.Add(File.IndexOf(CharInvalid), CharInvalid)
                    End If
                Next

                If InvalidCharFound.Count <> 0 Then

                    Dim StringInvaliChar As String = ""

                    For Each pair In InvalidCharFound.Keys
                        StringInvaliChar = StringInvaliChar & "Carattere: " & InvalidCharFound.Item(pair).ToString &
                            " [ASCII code " & Asc(CChar(InvalidCharFound.Item(pair))) & "] - in posizione [" & CInt(pair) + 1 & "]" & vbCrLf

                        'Tenta la sostituzione
                        NomeCompletoFile.Replace(CStr(InvalidCharFound.Item(pair)), "-")
                    Next

                    'Throw New Exception("Trovati Caratteri non validi nel nome file [" & FileLOG & "] : " & vbCrLf & _
                    '                   StringInvaliChar)
                End If

                'Creo il file  nuovo
                xStreamWriter = System.IO.File.CreateText(NomeCompletoFile)
                xStreamWriter.Flush()
                xStreamWriter.Close()

            End If

            'Apro il file di LOG
            xStreamWriter = System.IO.File.AppendText(NomeCompletoFile)

            'Scrivo la stringa
            xStreamWriter.WriteLine(Testo)
            xStreamWriter.Flush()
            'xStreamWriter.Close()

        Catch ex As Exception
            Throw New Exception("LOG : " & ex.Message)
        Finally

            If Not IsNothing(xStreamWriter) Then
                xStreamWriter.Close()
            End If

        End Try

    End Sub

    '############################################################################################################
    '---------- Crea un file facendo scegliere l'estensione, se esiste già lo sovrascrive --------------------
    '---------- in caso di eccezione ritorna l'errore ---------------------------
    '------------------------------------------------------------------
    'usata in Esportazione_OP_Inv, Esportazione_OP_Gest, Esportatore_Universale_2
    Public Shared Sub CreaScriviFileSovrascrivi(ByVal Contenuto As String, ByVal PathBase As String, ByVal nome_file As String, ByRef StrErrore As String, Optional ByVal Ext As String = "log")

        Try

            Dim PathFile As String = PathBase & "\" & nome_file

            FileSystem.ChDir(PathBase)

            Dim FileObject = CreateObject("Scripting.FileSystemObject")
            Dim OutStream = FileObject.CreateTextFile(nome_file & "." & Ext, True, 0)

            OutStream.WriteLine(Contenuto)
            OutStream.Close()

            OutStream = Nothing
            FileObject = Nothing

            FileSystem.ChDir("C:\")

        Catch ex As Exception

            StrErrore = ex.Message

        End Try


    End Sub

    '################################################################################
    Public Shared Function EsistePercorsoCartella(ByVal PathCartella As String) As Boolean

        'cerca se esiste una determinata cartella nel percorso assegnato nel web config

        Dim esiste As Boolean

        esiste = System.IO.Directory.Exists(PathCartella)

        Return esiste

    End Function

    '################################################################################
    Public Shared Function EsistePercorsoCartella2(ByVal PathIniziale As String, _
                                                        ByVal nome_cartella As String, _
                                                        ByRef PathCompleto As String) As Boolean


        Dim esiste As Boolean

        If PathIniziale <> "" Then

            If Right(PathIniziale, 1) = "\" OrElse Right(PathIniziale, 1) = "/" Then
                PathCompleto = PathIniziale & nome_cartella
            Else
                PathCompleto = PathIniziale & "\" & nome_cartella
            End If

        Else

            '----------------------------------------------------------
            'SE NEL WEB CONFIG NON C'E' IL PERCORSO, USO AGROTEMPORANEA
            '----------------------------------------------------------

            PathCompleto = "C:\AgroTemporanea" & "\" & nome_cartella

        End If


        'controllo se la cartella esiste già
        esiste = System.IO.Directory.Exists(PathCompleto)

        Return esiste


    End Function

    '############################################################################################################
    '---------- Crea un file facendo scegliere l'estensione, se esiste già lo sovrascrive --------------------
    '---------- in caso di eccezione ritorna l'errore ---------------------------
    '------------------------------------------------------------------
    '---- di diverso da CreaFileLogSovrascrivi c'è che ritorna il patchcompleto
    '--------------------------------------------------------------------------------------------
    Public Shared Sub CreaScriviFileSovrascrivi_RitornaPath(ByVal Log As String, ByVal PathBase As String, ByVal nome_file As String, ByRef PathCompleto As String, ByRef StrErrore As String, Optional ByVal Ext As String = "log")

        Try

            PathCompleto = PathBase & "\" & nome_file & "." & Ext

            FileSystem.ChDir(PathBase)

            Dim FileObject = CreateObject("Scripting.FileSystemObject")

            Dim OutStream = FileObject.CreateTextFile(nome_file & "." & Ext, True, 0)

            OutStream.WriteLine(Log)
            OutStream.Close()

            OutStream = Nothing
            FileObject = Nothing

            FileSystem.ChDir("C:\")


        Catch ex As Exception

            StrErrore = ex.Message

        End Try


    End Sub


    '################################################################################
    Public Shared Sub CopiaFile(ByVal PathSource As String, _
                                                ByVal PathDestination As String, _
                                                ByRef StrErrore As String, _
                                                ByVal Flag_Sovrascrivi As Boolean)

        Try

            System.IO.File.Copy(PathSource, PathDestination, Flag_Sovrascrivi)

        Catch ex As Exception

            StrErrore = ex.Message

        End Try

    End Sub

    Shared Function FileIsLocked(ByVal fileFullPathName As String) As Boolean
        Dim isLocked As Boolean = False
        Dim fileObj As IO.FileStream = Nothing

        Try
            fileObj = New IO.FileStream(fileFullPathName,
                                        IO.FileMode.Open,
                                        IO.FileAccess.ReadWrite,
                                        IO.FileShare.None)
        Catch
            isLocked = True
        Finally
            If Not IsNothing(fileObj) Then
                fileObj.Close()
            End If
        End Try
        Return isLocked
    End Function

End Class
