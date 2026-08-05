Imports System.Data.OleDb
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.exceptions

Public Class Utenti_Token_Spid_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id As Integer,
                          ByVal username As String,
                          ByVal Sistema_Cod As Integer,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti_Token_Spid ")
            StrSQL.Append(" WHERE 1 = 1 ")

            If Id <> 0 Then
                StrSQL.Append(" AND  ID = " & Agro_SQL_SaveNum(Id) & " ")
            End If

            If Id <> 0 Then
                StrSQL.Append(" AND  ID = " & Agro_SQL_SaveNum(Id) & " ")
            End If

            If username <> "" Then
                StrSQL.Append(" AND  username = '" & Agro_SQL_SaveText(username) & "' ")
            End If

            If Sistema_Cod <> 0 Then
                StrSQL.Append(" AND  Sistema_Cod = " & Agro_SQL_SaveNum(Sistema_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

End Class

Public Class Utenti_Token_Spid_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal ID As String,
                     ByVal Username As String,
                     ByVal Sistema_Cod As Integer,
                     ByVal Access_Token As String,
                     ByVal Access_Token_ExpireDate As DateTime,
                     ByVal Refresh_Token As String,
                     ByVal Refresh_Token_ExpireDate As DateTime,
                     ByVal Validita_Inizio As DateTime,
                     ByVal Validita_Fine As DateTime,
                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID = 0 Then
                Throw New GiasException("ID = 0 in scrivi Utenti_Token_Spid ")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Utenti_Token_Spid (ID, ")
            StrSQL.Append("                    Username, ")
            StrSQL.Append("                    Sistema_Cod, ")
            StrSQL.Append("                    Access_Token, ")
            StrSQL.Append("                    Access_Token_ExpireDate, ")
            StrSQL.Append("                    Refresh_Token, ")
            StrSQL.Append("                    Refresh_Token_ExpireDate, ")
            StrSQL.Append("                    inviato, ")
            StrSQL.Append("                    datainvio, ")
            StrSQL.Append("                    Data_Creazione, ")
            StrSQL.Append("                    Data_Modifica, ")
            StrSQL.Append("                    Username_Creazione, ")
            StrSQL.Append("                    Username_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,  ")
            StrSQL.Append("                    Validita_Fine ")
            StrSQL.Append("                    ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("           " & Agro_SQL_SaveNum(ID) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Username) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sistema_Cod) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Access_Token) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Access_Token_ExpireDate) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Refresh_Token) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Refresh_Token_ExpireDate) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(0) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(DateTime.Now) & "  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'  ")
            StrSQL.Append("         , '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Validita_Fine) & "  ")
            StrSQL.Append(")")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Elimina(ByVal ID As String,
                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID = 0 Then
                Throw New GiasException("ID = 0 in Elimina Utenti_Token_Spid ")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" DELETE FROM Utenti_Token_Spid ")
            StrSQL.Append(" WHERE ID = " & Agro_SQL_SaveNum(ID) & "  ")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

    Public Function Modifica_Parametrizzata(ByVal ID As String,
                     ByVal Campo As String,
                     ByVal Valore As String,
                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_W.Modifica_Parametrizzata()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID = 0 Then
                Throw New GiasException("ID = 0 in scrivi Utenti_Token_Spid ")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Utenti_Token_Spid ")
            StrSQL.Append(" SET  " & Campo & " = '" & Agro_SQL_SaveText(Valore) & "', Data_Modifica =  CONVERT(datetime, GETDATE(), 120) ")
            StrSQL.Append(" WHERE ID = " & Agro_SQL_SaveNum(ID) & " ")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function
End Class
