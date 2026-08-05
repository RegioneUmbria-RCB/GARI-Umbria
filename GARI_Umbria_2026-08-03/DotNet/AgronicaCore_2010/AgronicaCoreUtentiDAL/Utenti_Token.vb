Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Utenti_Token_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Token_ID As String, _
                     ByVal PivaSuperUser As String, _
                     ByVal SuperUser_Username As String, _
                     ByVal SuperUser_Password As String, _
                     ByVal Utente_Username As String, _
                     ByVal Utente_Password As String, _
                     ByVal Applicazione_Richiedente As Int32, _
                     ByVal IP_Rilascio As String, _
                     ByVal Fr_Cod As Int32, _
                     ByVal Parametri As String, _
                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Utenti_Token (Token_ID, ")
            StrSQL.Append("                    PivaSuperUser, ")
            StrSQL.Append("                    SuperUser_Username, ")
            StrSQL.Append("                    SuperUser_Password, ")
            StrSQL.Append("                    Utente_Username, ")
            StrSQL.Append("                    Utente_Password, ")
            StrSQL.Append("                    Applicazione_Richiedente, ")
            StrSQL.Append("                    IP_Rilascio, ")
            StrSQL.Append("                    Data_Rilascio, ")
            StrSQL.Append("                    Fr_Cod, ")
            StrSQL.Append("                    Parametri, ")

            StrSQL.Append("                    inviato,  ")
            StrSQL.Append("                    datainvio ")
            StrSQL.Append("                    ) ")

            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Token_ID) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(SuperUser_Username) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(SuperUser_Password) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Utente_Username) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Utente_Password) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Applicazione_Richiedente) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(IP_Rilascio) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Fr_Cod) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Parametri) & "' ")

            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
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

    Public Function Modifica_Token(ByVal Token_ID As String, _
                     ByVal SuperUser_Username As String, _
                     ByVal Utente_Username As String, _
                     ByVal Fr_Cod As Int32, _
                     ByVal Parametri As String, _
                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti_Token with (ROWLOCK) SET ")
            StrSQL.Append("       Token_ID = '" & Agro_SQL_SaveText(Token_ID) & "'")
            StrSQL.Append("       ,Data_Rilascio     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("       ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("       ,parametri = '" & Agro_SQL_SaveText(Parametri) & "'")
            StrSQL.Append(" WHERE SuperUser_Username='" & Agro_SQL_SaveText(SuperUser_Username) & "'")
            StrSQL.Append(" AND   Utente_Username = '" & Agro_SQL_SaveText(Utente_Username) & "' ")
            StrSQL.Append(" AND   Fr_Cod = " & Agro_SQL_SaveNum(Fr_Cod) & " ")

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

Public Class Utenti_Token_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti_Token ")
            StrSQL.Append(" WHERE SuperUser_Username ='" & objParametri.SuperUserUsername & "'")
            StrSQL.Append(" AND Utente_Username ='" & objParametri.UtenteUsername & "'")

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

    Public Function LeggiSuperUser(
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
            StrSQL.Append(" FROM    Utenti_Token ")
            StrSQL.Append(" WHERE SuperUser_Username ='" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "'")
            StrSQL.Append(" AND Utente_Username ='" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "'")

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
    Public Function Leggi_Token(ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_R.Leggi_Token()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Token As String = ""

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti_Token ")
            StrSQL.Append(" WHERE SuperUser_Username ='" & objParametri.SuperUserUsername & "'")
            StrSQL.Append(" AND Utente_Username ='" & objParametri.UtenteUsername & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Token = DT.Rows(0).Item("Token_ID")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Token

    End Function

    Public Function Leggi_Token(
            ByVal token As String,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_R.Leggi_Token()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti_Token ")
            StrSQL.Append(" WHERE token_id ='" & Agro_SQL_SaveText(token) & "'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


    Public Function Esiste_Utente_Formulato(ByVal Fr_Cod As Int32, _
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                 ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_R.Esiste_Utente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Esiste As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti_Token ")
            StrSQL.Append(" WHERE SuperUser_Username ='" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "'")
            StrSQL.Append(" AND Utente_Username ='" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'")
            StrSQL.Append(" AND Fr_Cod =" & Agro_SQL_SaveNum(Fr_Cod) & " ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Esiste = True
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Esiste

    End Function

    Public Function Leggi_idDB(ByVal tipoDB As Integer,
                               ByVal nomeProvider As String,
                               ByVal nomeServer As String,
                               ByVal nomeDB As String,
                               ByVal userId As String,
                               ByVal pivaSuperUser As String,
                               ByRef objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Integer
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Token_R.Leggi_idDB()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim idDB As Integer = 0

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT * ")
            StrSQL.AppendLine("FROM Connessioni ")
            StrSQL.AppendLine("WHERE TipoDB = " & Agro_SQL_SaveNum(tipoDB) & "")
            StrSQL.AppendLine("    AND Provider ='" & Agro_SQL_SaveText(nomeProvider) & "'")
            StrSQL.AppendLine("    AND Server ='" & Agro_SQL_SaveText(nomeServer) & "'")
            StrSQL.AppendLine("    AND DB ='" & Agro_SQL_SaveText(nomeDB) & "' ")
            StrSQL.AppendLine("    AND UserID ='" & Agro_SQL_SaveText(userId) & "' ")
            StrSQL.AppendLine("    AND PivaSuperUser ='" & Agro_SQL_SaveText(pivaSuperUser) & "' ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Super_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count = 1 Then
                idDB = DT.Rows(0)("ID_DB")
            ElseIf Not DT Is Nothing AndAlso DT.Rows.Count > 1 Then
                Throw New Exception("Sono presenti più righe di connessioni al DB!")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Super_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return idDB

    End Function

End Class
