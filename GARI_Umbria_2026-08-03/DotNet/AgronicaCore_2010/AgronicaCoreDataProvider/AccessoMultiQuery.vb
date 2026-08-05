Imports System.Data
Imports System.Data.Common
Imports System.Data.SqlClient



Public Class AccessoMultiQuery : Inherits DataProvider



    ''################################################################################################
    'Private Function MLT_ConnectionString_from_ConnessioneGias( _
    '                            ByVal PathFil... As String, _
    '                            ByVal ConnessioneGias As String) _
    '                            As String

    '    Dim Fs As New System.IO.StreamReader(PathFil---)
    '    Dim strConString, strParUsr, strUsr, strParPwd, strPwd As String
    '    strConString = ""
    '    strParUsr = ""
    '    strUsr = ""
    '    strParPwd = ""
    '    strPwd = ""

    '    Dim StringaConnessione As String = ""

    '    Try

    '        Dim strLine As String = Fs.ReadLine
    '        While Not IsNothing(strLine) 'cioè finchè c'è qualcosa nel file cicla

    '            If strLine = "[" & ConnessioneGias.Trim & "]" Then 'cicla fino a quando non trova la chiave
    '                'leggo la riga successiva..
    '                strLine = Fs.ReadLine 'leggo fino a quando non trovo il ; o la stringa vuota..
    '                While Not strLine.StartsWith(";") Or strLine = ""

    '                    If strLine.StartsWith("ProviderParam") Or strLine.StartsWith("ServerParam") Or _
    '                        strLine.StartsWith("DbParam") Then
    '                        'prendo la parte seguente il primo = ..
    '                        strConString += strLine.Substring(strLine.IndexOf("=") + 1).Replace("""", "") + ";"
    '                    ElseIf strLine.StartsWith("UserIdParam") Then
    '                        strParUsr = strLine.Substring(strLine.IndexOf("=") + 1) + "="
    '                    ElseIf strLine.StartsWith("PasswordParam") Then
    '                        strParPwd = strLine.Substring(strLine.IndexOf("=") + 1) + "="
    '                    ElseIf strLine.StartsWith("UserId") Then
    '                        strUsr = strLine.Substring(strLine.IndexOf("=") + 1)
    '                    ElseIf strLine.StartsWith("Password") Then
    '                        strPwd = strLine.Substring(strLine.IndexOf("=") + 1)

    '                    End If

    '                    strLine = Fs.ReadLine

    '                End While

    '                Exit While

    '            End If

    '            strLine = Fs.ReadLine

    '        End While

    '        Fs.Close()

    '        strConString = strConString.Replace("Provider=SQLOLEDB;Server", "Data Source")
    '        StringaConnessione = strConString + strParUsr + strUsr + ";" + strParPwd + strPwd + ";"
    '        Return StringaConnessione

    '    Catch exc As Exception

    '        Fs.Close()
    '        Throw New Exception(exc.Message.ToString)
    '        Return ""

    '    End Try

    'End Function




    ''################################################################################################
    'Public Function MLT_SelectFiltrataConTabellaTemporanea_NON_USARE_USARE_2013(ByVal Query1_TempTableCreazione As String, _
    '                                                        ByVal Query2_TempTableIndice As String, _
    '                                                        ByVal Query3_TempTableFill As String, _
    '                                                        ByVal Query4_TempTableJoin As String, _
    '                                                        ByVal Flag_1_ConnGias_2_ConnAltro As String, _
    '                                                        ByVal Connessione_Server As String, _
    '                                                        ByVal PathFil... As String, _
    '                                                        ByVal Stringa_Connessione_Altro As String, _
    '                                                        ByRef strErr As String) _
    '                                                        As DataTable

    '    Dim xConnectionString As String

    '    '-------------------------------------------------------------------
    '    '--- Preparazione della connessione
    '    '-------------------------------------------------------------------


    '    If Flag_1_ConnGias_2_ConnAltro = 1 Then
    '        xConnectionString = MLT_ConnectionString_from_ConnessioneGias( _
    '                             PathFil..., _
    '                             Connessione_Server)
    '    Else
    '        xConnectionString = Stringa_Connessione_Altro
    '    End If

    '    'Connessione
    '    Dim xConnessione As New SqlConnection(xConnectionString)
    '    xConnessione.Open()

    '    '-------------------------------------------------------------------
    '    '--- Query 1 : Creazione della tabella temporanea
    '    '-------------------------------------------------------------------

    '    'Comando
    '    Dim xComando1 As SqlCommand = xConnessione.CreateCommand()
    '    xComando1.Connection = xConnessione
    '    xComando1.CommandText = Query1_TempTableCreazione

    '    's.Append(" SELECT   Piva, Sa_Cod, Appezza, Id_Reg ")
    '    's.Append(" INTO     #TempImpianti ")
    '    's.Append(" FROM     Reg_Impianti ")
    '    's.Append(" WHERE    1 = 0   ")
    '    's.Append(vbCrLf)

    '    Try
    '        xComando1.ExecuteNonQuery()

    '    Catch ex As Exception
    '        strErr = ex.Message
    '        Return Nothing
    '        Exit Function
    '    End Try

    '    '-------------------------------------------------------------------
    '    '--- Query 2 : Creazione dell'indice per la tabella temporanea
    '    '-------------------------------------------------------------------

    '    'Comando
    '    Dim xComando2 As SqlCommand = xConnessione.CreateCommand()
    '    xComando2.Connection = xConnessione
    '    xComando2.CommandText = Query2_TempTableIndice

    '    's.Append(" CREATE UNIQUE INDEX [#AgroIndexTempImpianti] ON [dbo].[#TempImpianti]([Piva], [Sa_Cod], [Appezza], [Id_Reg]) ")
    '    's.Append(vbCrLf)

    '    Try
    '        xComando2.ExecuteNonQuery()

    '    Catch ex As Exception
    '        strErr = ex.Message
    '        Return Nothing
    '        Exit Function
    '    End Try


    '    '-------------------------------------------------------------------
    '    '--- Query 3 : Inserimento dei record nella tabella
    '    '-------------------------------------------------------------------

    '    'Comando
    '    Dim xComando3 As SqlCommand = xConnessione.CreateCommand()
    '    xComando3.Connection = xConnessione
    '    xComando3.CommandText = Query3_TempTableFill

    '    's.Append(" INSERT INTO #TempImpianti (Piva, Sa_Cod, Appezza, Id_Reg)  " & vbCrLf)
    '    's.Append(" VALUES     ('" + SQL_SaveText(Piva) + "'," + SQL_SaveNum(Sa_Cod) + "," + SQL_SaveNum(Appezza) + "," + SQL_SaveNum(Id_Reg) + ")  " & vbCrLf)

    '    Try
    '        xComando3.ExecuteNonQuery()

    '    Catch ex As Exception
    '        strErr = ex.Message
    '        Return Nothing
    '        Exit Function
    '    End Try

    '    '-------------------------------------------------------------------
    '    '--- Query 4 : Recupero delle informazioni filtrate
    '    '-------------------------------------------------------------------

    '    Dim DT As New DataTable

    '    'Comando
    '    Dim xComando4 As SqlCommand = xConnessione.CreateCommand()
    '    xComando4.Connection = xConnessione
    '    xComando4.CommandText = Query4_TempTableJoin
    '    xComando4.CommandTimeout = 600

    '    's.Append(" SELECT * FROM #TempImpianti   ")

    '    Dim xDataAdapter As New SqlDataAdapter

    '    Try
    '        xDataAdapter.SelectCommand = xComando4
    '        xDataAdapter.Fill(DT)

    '        Return DT

    '    Catch ex As Exception
    '        strErr = ex.Message
    '        Return Nothing
    '        Exit Function
    '    End Try

    '    xComando1.Dispose()
    '    xComando2.Dispose()
    '    xComando3.Dispose()
    '    xComando4.Dispose()

    '    xConnessione.Close()
    '    xDataAdapter.Dispose()
    '    xConnessione.Dispose()

    'End Function



    '################################################################################################
    Public Function MLT_SelectFiltrataConTabellaTemporanea_2013(ByVal Query1_TempTableCreazione As String,
                                                            ByVal Query2_TempTableIndice As String,
                                                            ByVal Query3_TempTableFill As String,
                                                            ByVal Query4_TempTableJoin As String,
                                                            ByVal OleDB_Stringa_Connessione As String,
                                                            ByRef strErr As String) _
                                                            As DataTable


        Dim parameters As List(Of SqlParameter) = New List(Of SqlParameter)
        Dim StringaSql1 As String = String.Empty
        Dim StringaSql2 As String = String.Empty
        Dim StringaSql3 As String = String.Empty
        Dim StringaSql4 As String = String.Empty
        Dim queryParametrizzata1 As Boolean = False
        Dim queryParametrizzata2 As Boolean = False
        Dim queryParametrizzata3 As Boolean = False
        Dim queryParametrizzata4 As Boolean = False

        StringaSql1 = Query1_TempTableCreazione
        queryParametrizzata1 = Parametrizza(StringaSql1, parameters)

        StringaSql2 = Query2_TempTableIndice
        queryParametrizzata2 = Parametrizza(StringaSql2, parameters)

        StringaSql3 = Query3_TempTableFill
        queryParametrizzata3 = Parametrizza(StringaSql3, parameters)

        StringaSql4 = Query4_TempTableJoin
        queryParametrizzata4 = Parametrizza(StringaSql4, parameters)

        'Connessione
        Dim xConnessione As DbConnection = DataProviderFactory.Instance.CreaNuovaConnessione(OleDB_Stringa_Connessione)
        xConnessione.Open()

        '-------------------------------------------------------------------
        '--- Query 1 : Creazione della tabella temporanea
        '-------------------------------------------------------------------
        Dim xComando1 As DbCommand = xConnessione.CreateCommand()
        xComando1.Connection = xConnessione

        If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.SqlDataProvider Then

            xComando1.CommandText = StringaSql1
            If queryParametrizzata1 AndAlso StringaSql1 <> Query1_TempTableCreazione Then
                For Each p As SqlParameter In parameters
                    Dim copy As New SqlParameter(p.ParameterName, p.SqlDbType)
                    copy.Value = p.Value
                    xComando1.Parameters.Add(copy)
                Next
            End If
        Else
            xComando1.CommandText = Query1_TempTableCreazione
        End If

        Try
            xComando1.ExecuteNonQuery()
        Catch ex As Exception
            strErr = ex.Message
            Return Nothing
            Exit Function
        End Try

        '-------------------------------------------------------------------
        '--- Query 2 : Creazione dell'indice per la tabella temporanea
        '-------------------------------------------------------------------

        'Comando
        Dim xComando2 As DbCommand = xConnessione.CreateCommand()
        xComando2.Connection = xConnessione

        If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.SqlDataProvider Then

            xComando2.CommandText = StringaSql2
            If queryParametrizzata2 AndAlso StringaSql2 <> Query2_TempTableIndice Then
                For Each p As SqlParameter In parameters
                    Dim copy As New SqlParameter(p.ParameterName, p.SqlDbType)
                    copy.Value = p.Value
                    xComando2.Parameters.Add(copy)
                Next
            End If

        Else
            xComando2.CommandText = Query2_TempTableIndice
        End If

        Try
            xComando2.ExecuteNonQuery()
        Catch ex As Exception
            strErr = ex.Message
            Return Nothing
            Exit Function
        End Try


        '-------------------------------------------------------------------
        '--- Query 3 : Inserimento dei record nella tabella
        '-------------------------------------------------------------------

        'Comando
        Dim xComando3 As DbCommand = xConnessione.CreateCommand()
        xComando3.Connection = xConnessione

        If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.SqlDataProvider Then

            xComando3.CommandText = StringaSql3
            If queryParametrizzata3 AndAlso StringaSql3 <> Query3_TempTableFill Then
                For Each p As SqlParameter In parameters
                    Dim copy As New SqlParameter(p.ParameterName, p.SqlDbType)
                    copy.Value = p.Value
                    xComando3.Parameters.Add(copy)
                Next
            End If

        Else
            xComando3.CommandText = Query3_TempTableFill
        End If

        Try
            xComando3.ExecuteNonQuery()

        Catch ex As Exception
            strErr = ex.Message
            Return Nothing
            Exit Function
        End Try

        '-------------------------------------------------------------------
        '--- Query 4 : Recupero delle informazioni filtrate
        '-------------------------------------------------------------------

        Dim DT As New DataTable

        'Comando
        Dim xComando4 As DbCommand = xConnessione.CreateCommand()
        xComando4.Connection = xConnessione

        If DataProviderFactory.Instance.TipoProvider = TipiEnumerativi.enum_DataProvidersType.SqlDataProvider Then

            xComando4.CommandText = StringaSql4
            If queryParametrizzata4 AndAlso StringaSql4 <> Query4_TempTableJoin Then
                For Each p As SqlParameter In parameters
                    Dim copy As New SqlParameter(p.ParameterName, p.SqlDbType)
                    copy.Value = p.Value
                    xComando4.Parameters.Add(copy)
                Next
            End If
        Else
            xComando4.CommandText = Query4_TempTableJoin
        End If
        xComando4.CommandTimeout = 600
        Dim xDataAdapter As DbDataAdapter = DataProviderFactory.Instance.CreaDataAdapter()

        Try
            xDataAdapter.SelectCommand = xComando4
            xDataAdapter.Fill(DT)

            Return DT

        Catch ex As Exception
            strErr = ex.Message
            Return Nothing
            Exit Function
        End Try

        xComando1.Dispose()
        xComando2.Dispose()
        xComando3.Dispose()
        xComando4.Dispose()

        xConnessione.Close()
        xDataAdapter.Dispose()
        xConnessione.Dispose()

    End Function

End Class
