'CLASSE COPIATA DAL V.S.2003 DA DRUDI
'in realtà non andrebbe utilizzata perchè bisogna utilizzare le funzioni che fanno le query dentro al DataProvider

Imports System.Data.Common
Imports System.Reflection
Imports ADODB
Imports AgronicaCoreDataProvider

Namespace Codex_Utility
    'questa classe si occupa di eseguire delle query su un database
    Public Class Sql
        Implements IDisposable

        Private ADOConnection As ADODB.Connection
        Private OleDbCon As DbConnection
        Private OleDataRdr As DbDataReader

        'costruttore x istanzia gli oggetti...
        Public Sub New()
            ADOConnection = New ADODB.Connection()
            OleDbCon = DataProviderFactory.Instance.CreaNuovaConnessione()
        End Sub



        'architettura disconnessa....gestisce le stringhe vuote delle txtbox e ritorna un tipo dbnull
        Public Function ConvertiStringaVuotaDbNull(ByVal strValore As String) As Object
            If strValore = "" Then
                'ritorno il tipo dbnull
                Return DBNull.Value
            Else 'se non è vuoto ritorno il valore che c'è nella stringa
                Return strValore
            End If
        End Function







        'ritorno l'oggetto connessione corrente
        Public Function OttieniConnessione() As DbConnection
            'controllo che la connessione sia chiusa..
            If Me.OleDbCon.State = ConnectionState.Closed Then
                Return Me.OleDbCon
            Else
                Throw New Exception("Impossibile ottenere la connessione in quanto risulta attualmente aperta")
            End If
        End Function


        'effettua l'inserimento....
        Public Function SqlInsertUpdate_New(ByRef Connessione As DbConnection,
                                            ByRef Transazione As DbTransaction,
                                            ByVal strQuery As String,
                                            ByRef strErr As String) As Integer

            Dim Num_Record As Integer

            Try
                Dim Comando As DbCommand = DataProviderFactory.Instance.CreaCommand()

                Comando.Connection = Connessione
                Comando.Transaction = Transazione
                Comando.CommandText = strQuery

                ' eseguo la transazione
                Num_Record = Comando.ExecuteNonQuery()

                Comando.Dispose()

                strErr = Nothing

                Return Num_Record

            Catch exc As Exception 'ritorno l'eccezione...

                strErr = exc.Message.ToString

                Return 0

            End Try

        End Function

        'formattazione delle stringhe x gli insert
        Public Function FormatString4Db(ByVal strInputVal As String) As String
            'tolgo eventuali caratteri di ritorno a capo e apici singoli
            Dim strRet As String = strInputVal.Replace(vbNewLine, "")
            Return strRet.Replace("'", "''").Trim
        End Function

        'formattazione numeri reali, anche la valuta
        Public Function FormatNumeri4Db(ByVal strNumero As String, Optional ByVal blnIsCurrency As Boolean = False) As String
            'controllo se è numerico
            If IsNumeric(strNumero) Then
                If blnIsCurrency Then 'ritono il formato in valuta..
                    Return FormatNumber(strNumero, 2, , , TriState.False).ToString.Replace(",", ".")
                Else 'ritorno il numero...
                    Return strNumero.Replace(",", ".")
                End If
            Else 'ritono 0
                Return "0"
            End If

        End Function

        'formattazione delle date...
        Public Function FormatData(ByVal strData As String, Optional ByVal blnVisualizzaDateDefault As Boolean = False) As String

            If IsDate(strData) Then
                'formatto la data lunghezza 10
                strData = CDate(strData).ToString("dd/MM/yyyy")
                'controllo se devo gestire le date di default come stringa vuota..
                If blnVisualizzaDateDefault Then

                    If strData.Length > 10 Then 'tolgo le ore..
                        strData = strData.Substring(0, 10)
                    End If
                    Return strData
                Else 'ritono stringa vuota
                    'controllo che la data non corrisponda a quella di default...
                    If strData.Substring(0, 10) = "01/01/1900" Or strData.Substring(0, 10) = "31/12/2100" Then
                        Return ""
                    Else
                        If strData.Length > 10 Then 'tolgo le ore..
                            strData = strData.Substring(0, 10)
                        End If
                        Return strData
                    End If
                End If

            Else
                Return ""
            End If

        End Function

        'questa funzione imposta un campo a null se mi arriva vuoto
        Public Function GestisciCampoVuoto(ByVal objValore As Object, ByVal chrDdata_Sstring_Nnum As Char) As String

            If IsNothing(objValore) Then 'se il campo è vuoto lo gestisco..
                Return "null" 'ritono nullo
            Else 'ritorno il valore
                'controllo se devo appendere gli apici o meno..
                If objValore.ToString = "" Then
                    Return "null"
                Else

                    Select Case chrDdata_Sstring_Nnum
                        Case "D" 'lo formatto come data e poi ritorno il valore tra apici
                            Return "'" & Me.FormatData(objValore.ToString) & "'"
                        Case "S"
                            Return "'" & Me.FormatString4Db(objValore.ToString) & "'"
                        Case "N"
                            Return Me.FormatNumeri4Db(objValore.ToString)
                    End Select

                End If
            End If

        End Function



        'funzione che crea un datatable da un recordset...
        'come parametri ovviamente il recordset e opzionali un array di stringhe nell'eventualità
        'voglia solo che certi campi siano compresi nel dt e opzionali anche un array di stringhe con i
        'nomi dei campi chaive..
        Public Function CreateDT_from_RS(ByVal RS As ADODB.Recordset, ByRef strErrore As String,
                                            Optional ByVal strCampi() As String = Nothing,
                                            Optional ByVal strChiavi() As String = Nothing) As DataTable

            'controllo se il recordset è valido..
            If Not IsNothing(RS) AndAlso RS.State <> 0 AndAlso Not RS.EOF Then

                'creo la tabella x mappare i campi.......
                Dim dt As New DataTable

                'gestione errori..
                Try
                    Dim i As Integer
                    Dim Dap As New OleDb.OleDbDataAdapter

                    'controllo se è stato richiesto un mapping dei campi...
                    If Not IsNothing(strCampi) AndAlso strCampi.Length > 0 Then
                        'aggiungo i campi..
                        For i = 0 To strCampi.Length - 1
                            dt.Columns.Add(strCampi(i))
                        Next
                        'riempio il datatable passando dal rs solo le colonne che mi interessano...
                        Dap.MissingSchemaAction = MissingSchemaAction.Ignore
                    End If

                    Dap.Fill(dt, RS)

                    'controllo l'array delle chiavi......
                    '1 non deve contenre + oggetti dell'array dei campi..
                    '2 deve esserci qualcosa......
                    If Not IsNothing(strChiavi) And Not IsNothing(strCampi) AndAlso strChiavi.Length > 0 _
                                                AndAlso Not strChiavi.Length > strCampi.Length Then
                        Dim dcK() As DataColumn
                        'ciclo sull'array
                        For i = 0 To strChiavi.Length - 1
                            ReDim Preserve dcK(i)
                            'aggiungo le colonne chiavi
                            dcK(i) = dt.Columns(strChiavi(i))
                        Next
                        'aggiungo le chiavi.....
                        dt.PrimaryKey = dcK
                    End If

                    strErrore = Nothing
                    Return dt


                Catch ex As Exception
                    strErrore = "Si è verificato un errore: " & ex.Message

                Finally
                    'controllo se il rs è ancora aperto....
                    If Not RS.State = 0 AndAlso Not RS.ActiveConnection Is Nothing Then
                        RS.Close()
                    End If

                End Try

            Else
                Return Nothing
            End If

        End Function


        'implemento il metodo dispose dell'interfaccia x chiudere eventuali risorse rimaste aperte
        'in particolre in riferimento al metodo x il lancio della stored procedure che lasci aperta connenessione
        'e data reader..
        Public Sub SqlDispose() Implements IDisposable.Dispose
            'controllo il datareader
            If Not IsNothing(OleDataRdr) AndAlso Not OleDataRdr.IsClosed() Then
                'chiudo il datareader
                OleDataRdr.Close()
            End If
            'chiudo la connessione aperta
            If Not IsNothing(OleDbCon) AndAlso OleDbCon.State = ConnectionState.Open Then
                OleDbCon.Close() 'chiudo la connessione
            End If
        End Sub





        '---------------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------
        '----------------------------------------------------------------------------------------------------


        'Funzione modificata per aumentare il timeout di escuzione delle query che di default è 30 secondi
        'ritorna un recordset o un datatable...
        Public Function SqlSelect(ByVal strConnessione As String, ByVal primaIniOraVuoto As String, ByVal strQuery As String,
                                        ByVal intObjOut_RS0_DT1 As Integer, ByRef strErr As String) As Object


            Dim sqlCommand As DbCommand
            Dim OleDap As DbDataAdapter

            'imposto la stessa stringa di connessione..
            Me.ADOConnection.ConnectionString = strConnessione
            Me.OleDbCon.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(strConnessione)


            Select Case intObjOut_RS0_DT1

                Case 0 'ritorno un rs la connessione deve essere chiusa usando la sub della classe

                    Try

                        Dim dt As DataTable = DataProviderFactory.Instance.Provider.EseguiQuery_Lettura(strConnessione, strQuery, "")
                        Dim rs As ADODB.Recordset = ConvertToRecordSet(dt)
                        strErr = Nothing

                        Return rs

                        'Dim RS As New ADODB.Recordset
                        'ADOConnection.CommandTimeout = 600
                        'ADOConnection.Open()

                        'RS.ActiveConnection = ADOConnection
                        'RS.LockType = ADODB.LockTypeEnum.adLockReadOnly
                        ''questa istruzione mi xmette di usare il rs in maniera disconessa.....
                        'RS.CursorLocation = ADODB.CursorLocationEnum.adUseClient
                        'RS.Open(strQuery)

                        'RS.ActiveConnection = Nothing
                        'Me.ADOConnection.Close()

                        'strErr = Nothing

                        'Return RS

                    Catch exc As Exception 'incaso di errore...
                        strErr = exc.Message.ToString 'salvo il msg di errore nella stringa
                        If Me.ADOConnection.State = "1" Then '...se la connessione è aperta la chiudo
                            Me.ADOConnection.Close()
                        End If
                    End Try

                Case 1 'ritorna un oledbdatatable

                    Try

                        Dim DT As New DataTable

                        sqlCommand = DataProviderFactory.Instance.CreaCommand()
                        sqlCommand.Connection = OleDbCon
                        'imposto il timeout a 10 minuti
                        sqlCommand.CommandTimeout = 600

                        sqlCommand.CommandText = strQuery
                        OleDap = DataProviderFactory.Instance.CreaDataAdapter(sqlCommand)
                        'OleDap.SelectCommand = sqlCommand

                        '------------------------------------------
                        'COMMENTO!!!!!
                        'Sql Server 2008 da problemi!!!
                        '''imposto anche le chiavi x la tabella
                        ''OleDap.MissingSchemaAction = MissingSchemaAction.AddWithKey
                        '------------------------------------------
                        OleDap.Fill(DT)

                        strErr = Nothing

                        Return DT

                    Catch exc As Exception 'incaso di errore...
                        strErr = exc.Message.ToString 'salvo il msg di errore nella stringa
                    End Try


                Case Else 'non ritorno un bel niente...

                    Return Nothing

            End Select

        End Function




        'Funzione modificata per aumentare il timeout di escuzione delle query che di default è 30 secondi
        'passo x riferimento un ds da riempire coi dati della select, utile nel caso di ds tipizzati...
        Public Sub SqlSelect(ByVal strConnessione As String, ByVal primaIniOraVuoto As String, ByVal strQuery As String,
                                        ByRef ds2Fill As DataSet, ByVal strNomeDtNelDS As String,
                                        ByRef strErr As String)


            Dim sqlCommand As DbCommand

            Try

                'imposto la stessa stringa di connessione..
                Me.ADOConnection.ConnectionString = strConnessione
                Me.OleDbCon.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(strConnessione)

                sqlCommand = DataProviderFactory.Instance.CreaCommand()
                sqlCommand.Connection = OleDbCon

                'imposto il timeout a 10 minuti
                sqlCommand.CommandTimeout = 600

                sqlCommand.CommandText = strQuery


                Dim OleDap As DbDataAdapter = DataProviderFactory.Instance.CreaDataAdapter(sqlCommand)
                If strNomeDtNelDS <> "" Then
                    OleDap.Fill(ds2Fill, strNomeDtNelDS)
                Else
                    OleDap.Fill(ds2Fill)
                End If

                strErr = Nothing

            Catch exc As Exception 'incaso di errore...
                strErr = exc.Message.ToString 'salvo il msg di errore nella stringa
            End Try


        End Sub




        'Funzione modificata per aumentare il timeout di escuzione delle query che di default è 30 secondi
        'ritorna un datatable con il corrispondente oggetto dataadapter x la gestione dell'architettura discon-
        'nessa
        Public Function SqlSelect(ByVal strConnessione As String, ByVal primaIniOraVuoto As String, ByVal strQuery As String,
                                    ByRef strErr As String, ByRef OleDataAdapter As DbDataAdapter) _
                                                                                                    As DataTable

            Dim sqlCommand As DbCommand

            Try

                'imposto la stessa stringa di connessione..
                Me.ADOConnection.ConnectionString = strConnessione
                Me.OleDbCon.ConnectionString = DataProviderFactory.Instance.AggiustaStringaDiConnessione(strConnessione)

                sqlCommand = OleDbCon.CreateCommand
                sqlCommand.Connection = OleDbCon

                'imposto il timeout a 10 minuti
                sqlCommand.CommandTimeout = 600

                sqlCommand.CommandText = strQuery

                Dim DT As New DataTable
                OleDataAdapter = DataProviderFactory.Instance.CreaDataAdapter(sqlCommand)
                'imposto anche le chiavi x la tabella
                OleDataAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey
                OleDataAdapter.Fill(DT)

                strErr = Nothing

                Return DT

            Catch exc As Exception 'incaso di errore...
                strErr = exc.Message.ToString 'salvo il msg di errore nella stringa
            End Try


        End Function


        '---------------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------
        '-----------------------------------------------------------------------------------------------------


        Function ConvertToRecordSet(ByVal inTable As DataTable) As ADODB.Recordset

            Dim recordSet = New ADODB.Recordset With {.CursorLocation = CursorLocationEnum.adUseClient}
            Dim recordSetFields = recordSet.Fields
            Dim inColumns = inTable.Columns

            For Each column As DataColumn In inColumns
                recordSetFields.Append(column.ColumnName, TranslateType(column.DataType), column.MaxLength, If(column.AllowDBNull, FieldAttributeEnum.adFldIsNullable, FieldAttributeEnum.adFldUnspecified), Nothing)
            Next

            recordSet.Open(Missing.Value, Missing.Value, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockOptimistic, 0)

            For Each row As DataRow In inTable.Rows
                recordSet.AddNew(Missing.Value, Missing.Value)
                Dim columnIndex = 0

                For columnIndex = 0 To inColumns.Count - 1
                    recordSetFields(columnIndex).Value = row(columnIndex)
                Next

            Next

            Return recordSet

        End Function

        Private Function TranslateType(ByVal columnDataType As IReflect) As DataTypeEnum
            Select Case columnDataType.UnderlyingSystemType.ToString()
                Case "System.Boolean"
                    Return DataTypeEnum.adBoolean
                Case "System.Byte"
                    Return DataTypeEnum.adUnsignedTinyInt
                Case "System.Char"
                    Return DataTypeEnum.adChar
                Case "System.DateTime"
                    Return DataTypeEnum.adDate
                Case "System.Decimal"
                    Return DataTypeEnum.adCurrency
                Case "System.Double"
                    Return DataTypeEnum.adDouble
                Case "System.Int16"
                    Return DataTypeEnum.adSmallInt
                Case "System.Int32"
                    Return DataTypeEnum.adInteger
                Case "System.Int64"
                    Return DataTypeEnum.adBigInt
                Case "System.SByte"
                    Return DataTypeEnum.adTinyInt
                Case "System.Single"
                    Return DataTypeEnum.adSingle
                Case "System.UInt16"
                    Return DataTypeEnum.adUnsignedSmallInt
                Case "System.UInt32"
                    Return DataTypeEnum.adUnsignedInt
                Case "System.UInt64"
                    Return DataTypeEnum.adUnsignedBigInt
                Case Else
                    Return DataTypeEnum.adVarChar
            End Select
        End Function

    End Class


End Namespace
