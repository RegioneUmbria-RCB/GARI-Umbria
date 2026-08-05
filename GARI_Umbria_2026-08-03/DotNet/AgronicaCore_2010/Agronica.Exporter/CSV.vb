Imports System.Data.OleDb
Imports System.Text

Public Class CSV

    'primo utilizzo: mini frutta, esportazione conferimenti verso harvard
    'varcharLen: max 255
    Public Function ExportToCSV(ByVal PathFile As String,
                                ByVal NomeFile As String,
                                ByVal Flag_CancellaFiles As Boolean,
                                ByVal DT As DataTable,
                                ByVal varcharLen As String,
                                ByVal escape As String
                                ) As String

        Dim query As String
        Dim cmd As OleDbCommand = Nothing
        Dim cnn As OleDbConnection = Nothing

        Dim commandInsert As OleDbCommand = Nothing

        Dim Rval As String = ""

        Try

            If Not PathFile.EndsWith("\") Then
                PathFile &= "\"
            End If

            If Not NomeFile.EndsWith(".csv") Then
                NomeFile &= ".csv"
            End If

            ''DEBUG
            'NomeFile = "PROVA.csv"

            Dim PathCompleto As String = ""
            PathCompleto = PathFile & NomeFile

            'cancella i file
            If Flag_CancellaFiles = True Then

                'Dim FileCancellato As Boolean = CancellaFile(PathFile, NomeFile)

                'If Not FileCancellato Then
                AgronicaCoreDataProvider.GestioneFile.CancellaFiles(PathFile, "*.csv", Rval)
                'Rval = "Non riesco ad eliminare il file precedente."
                If Rval <> "" Then
                    Return Rval
                End If
                'End If

            End If

            Dim provider As String
            If Environment.Is64BitProcess Then
                provider = "PROVIDER=Microsoft.ACE.OLEDB.12.0"
            Else
                provider = "PROVIDER=Microsoft.Jet.OLEDB.4.0"
            End If
            Dim connectionString As String = provider
            connectionString &= ";Extended Properties=""text;HDR=Yes;FMT=Delimited"";data source="
            'Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\Dati\File_Esportazioni\;Extended Properties="text;HDR=Yes;FMT=Delimited"
            connectionString &= PathFile

            CreaSchemaCSV(PathFile & "Schema.ini", NomeFile)
            'My.Computer.FileSystem.WriteAllText(PathCompleto, "", False, Encoding.ASCII)

            cnn = New OleDbConnection(connectionString)
            cnn.Open()


            Try
                Dim esiste As Boolean
                esiste = IO.File.Exists(PathCompleto)
                If esiste = True Then
                    'Drop the existing sheet(first Sheet)
                    query = "DELETE FROM [" & NomeFile & "]"
                    cmd = New OleDbCommand(query, cnn)
                    cmd.ExecuteNonQuery()
                End If
            Catch ex As Exception
                Rval &= "Errore In fase di pulizia foglio excel: " & ex.Message & vbCrLf
            End Try

            'Create new sheet with our requirements
            query = "CREATE TABLE [" & NomeFile & "] ("

            For Each cc As DataColumn In DT.Columns
                query &= "[" & cc.ColumnName & "]"
                query &= " " & getColumnDataType(cc, varcharLen) & ","
            Next

            query = query.TrimEnd(",") & ")"

            Try
                cmd = New OleDbCommand(query, cnn)
                cmd.ExecuteNonQuery()

            Catch ex As Exception
                Rval &= "Errore in fase di creazione foglio excel: " & ex.Message & vbCrLf
            End Try

            Try

                '***** Insert Data con Parametri *****

                commandInsert = New OleDbCommand()
                commandInsert.Connection = cnn

                Dim stringaNomiColonne As String = ""
                Dim segnapostoParametri As String = ""
                For Each cc As DataColumn In DT.Columns
                    stringaNomiColonne &= cc.ColumnName & ","
                    segnapostoParametri &= "?,"
                    commandInsert.Parameters.Add(cc.ColumnName, getColumnDataTypeOleDB(cc), varcharLen)
                Next
                stringaNomiColonne = stringaNomiColonne.TrimEnd(",")
                segnapostoParametri = segnapostoParametri.TrimEnd(",")

                commandInsert.CommandText = String.Format("INSERT INTO [{0}] ({1}) VALUES ({2});",
                                                          NomeFile, stringaNomiColonne, segnapostoParametri)
                commandInsert.Prepare()

                For Each row As DataRow In DT.Rows

                    For i As Integer = 0 To DT.Columns.Count - 1
                        commandInsert.Parameters(i).Value = FormatDataByTypeObject(row(i), escape, DT.Columns(i), False, False)
                    Next

                    commandInsert.ExecuteNonQuery()
                Next

                ''***** Insert Data con query (MOLTO più LENTA) *****
                'For Each row As DataRow In DT.Rows
                '    Dim values As String = "("

                '    For i As Integer = 0 To DT.Columns.Count - 1
                '        values &= FormatDataByType(row(i), escape, DT.Columns(i))
                '    Next

                '    values = values.TrimEnd(",") & ")"

                '    query = String.Format("Insert into [" & NomeFile & "] VALUES {0}", values)

                '    commandInsert = New OleDbCommand(query, cnn)
                '    commandInsert.ExecuteNonQuery()
                'Next

            Catch ex As Exception
                Rval &= "Errore in fase di esportazione su foglio excel (Query = " & query & " ): " & ex.Message & vbCrLf

                Try
                    Dim str_err As String = ""
                    AgronicaCoreDataProvider.GestioneFile.CancellaFile(PathCompleto, str_err)
                    If str_err <> "" Then
                        Rval &= "<br/>Non è stato possibile cancellare il file appena creato """ & NomeFile & """ al percorso " & PathFile & " occorre cancellarlo manualmente [" & str_err & "]"
                    End If
                Catch ex2 As Exception
                    Rval &= "<br/>Non è stato possibile cancellare il file appena creato """ & NomeFile & """ al percorso " & PathFile & " occorre cancellarlo manualmente [" & ex2.Message & "]"
                End Try

            End Try

        Catch ex As Exception
            Rval &= "<br/>" & ex.Message
        Finally
            If cmd IsNot Nothing Then
                cmd.Dispose()
            End If
            cmd = Nothing
            If commandInsert IsNot Nothing Then
                commandInsert.Dispose()
            End If
            commandInsert = Nothing
            If cnn IsNot Nothing Then
                cnn.Close()
            End If
        End Try


        Return Rval


    End Function



    Private Sub CreaSchemaCSV(ByVal PathFileIni As String, ByVal NomeFileCSV As String)
        Dim schemaInistb As New StringBuilder
        If NomeFileCSV.Contains("#csv") > 0 Then
            NomeFileCSV = NomeFileCSV.Replace("#csv", "") & ".csv"
        End If
        'schemaInistb.Append("[" & NomeFileCSV.Replace("#csv", "") & ".csv]" & vbCrLf)
        schemaInistb.Append("[" & NomeFileCSV & "]" & vbCrLf)
        schemaInistb.Append("Format=Delimited(;)" & vbCrLf)
        schemaInistb.Append("ColNameHeader=True")

        My.Computer.FileSystem.WriteAllText(PathFileIni, schemaInistb.ToString, False, Encoding.ASCII)
    End Sub


    'Private Function getFolderFromIni(ByVal PathFile As String) As String
    '    Dim app As String() = PathFile.Split("\")
    '    app(app.Length - 1) = ""
    '    Return String.Join("\", app)
    'End Function

    'Private Function CancellaFile(ByVal PathFile As String, ByVal lNomeFile As String) As Boolean
    '    Dim rval As Boolean = True

    '    Try

    '        Dim fD As String = getFolderFromIni(PathFile) & lNomeFile.Replace("#csv", "") & ".csv"

    '        If My.Computer.FileSystem.FileExists(fD) Then
    '            My.Computer.FileSystem.DeleteFile(fD, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
    '        End If

    '    Catch ex As Exception
    '        rval = False
    '    End Try


    '    Return rval

    'End Function

    Private Function getColumnDataType(ByVal c As System.Data.DataColumn, ByVal lVarcharLen As String) As String

        Select Case c.DataType.ToString
            Case "System.Integer", "System.Int32", "System.Int16", "System.Int64"
                Return "int"
            Case "System.Double", "System.Decimal"
                Return "Double"
            Case "System.DateTime", "System.Date"
                Return "Date"
            Case "System.String"
                Return "Varchar(" & lVarcharLen & ")"
            Case Else
                Return ""
        End Select

    End Function

    Private Function getColumnDataTypeOleDB(ByVal c As System.Data.DataColumn) As OleDbType

        Select Case c.DataType.ToString
            Case "System.Integer", "System.Int32", "System.Int16", "System.Int64"
                Return OleDbType.Integer
            Case "System.Double", "System.Decimal"
                Return OleDbType.Double
            Case "System.DateTime", "System.Date"
                'Visto che i valori dei parametri sono passati come stringa, anche qui devo mettere varchar
                Return OleDbType.VarChar
                'Return OleDbType.DBDate
            Case "System.String"
                Return OleDbType.VarChar
            Case Else
                Return OleDbType.Empty
        End Select

    End Function

    Private Function FormatDataByType(ByVal D As Object,
                                      ByVal escape As String,
                                      ByVal c As System.Data.DataColumn,
                                      Optional ByVal addApici As Boolean = True,
                                      Optional ByVal addComma As Boolean = True
                                      ) As String

        Dim rval As String = ""

        Select Case c.DataType.ToString

            Case "System.DateTime","System.Date"

                If [String].IsNullOrEmpty(D.ToString()) Then
                    D = CType("#01/01/1900 00:00:00#", DateTime)
                End If

                If DateDiff(DateInterval.Hour, CType(D, DateTime), CType(D, Date).Date) = 0 Then
                    rval = CType(D, DateTime).ToString("dd/MM/yyyy")
                Else
                    rval = CType(D, DateTime).ToString("dd/MM/yyyy HH.mm.ss")
                End If

                If addApici Then
                    rval = String.Format("'{0}'", rval)
                End If

            Case "System.Int32", "System.Double", "System.Decimal"

                rval = If([String].IsNullOrEmpty(D.ToString()), "0", D.ToString.Replace(",", "."))

            Case Else

                Dim lDToString As String = D.ToString()
                Dim lenEscape As Integer = escape.Length
                For i = 0 To lenEscape - 1 Step 2
                    lDToString = lDToString.Replace(escape(i), escape(i + 1))
                Next

                lDToString = lDToString.Replace(vbCr, "").Replace(vbLf, "")
                
                If addApici Then
                    'L'escape dell'apostrofo lo devo fare solo se ci sto aggiungendo cmq gli apici
                    lDToString = lDToString.Replace("'", "''")

                    rval = String.Format("'{0}'", lDToString)
                Else
                    rval = lDToString
                End If
        End Select

        If addComma Then
            rval &= ","
        End If

        Return rval

    End Function

    Private Function FormatDataByTypeObject(ByVal D As Object,
                                            ByVal escape As String,
                                            ByVal c As System.Data.DataColumn,
                                            Optional ByVal addApici As Boolean = True,
                                            Optional ByVal addComma As Boolean = True
                                            ) As Object

        Dim rval As Object = Nothing

        Select Case c.DataType.ToString

            Case "System.DateTime","System.Date"

                If [String].IsNullOrEmpty(D.ToString()) Then
                    D = CType("#01/01/1900 00:00:00#", DateTime)
                End If

                If DateDiff(DateInterval.Hour, CType(D, DateTime), CType(D, Date).Date) = 0 Then
                    rval = CType(D, DateTime).ToString("dd/MM/yyyy")
                Else
                    rval = CType(D, DateTime).ToString("dd/MM/yyyy HH.mm.ss")
                End If

                If addApici Then
                    rval = String.Format("'{0}'", rval)
                End If

            Case "System.Int32"

                rval = CType(If(String.IsNullOrEmpty(D.ToString()), 0, D), Integer)

            Case "System.Double", "System.Decimal"

                rval = CType(If(String.IsNullOrEmpty(D.ToString()), 0, D), Double)

            Case Else

                Dim lDToString As String = D.ToString()
                Dim lenEscape As Integer = escape.Length
                For i = 0 To lenEscape - 1 Step 2
                    lDToString = lDToString.Replace(escape(i), escape(i + 1))
                Next

                lDToString = lDToString.Replace(vbCr, "").Replace(vbLf, "")
                
                If addApici Then
                    'L'escape dell'apostrofo lo devo fare solo se ci sto aggiungendo cmq gli apici
                    lDToString = lDToString.Replace("'", "''")

                    rval = String.Format("'{0}'", lDToString)
                Else
                    rval = lDToString
                End If
        End Select

        If addComma Then
            rval = CStr(rval) & ","
        End If

        Return rval

    End Function



End Class
