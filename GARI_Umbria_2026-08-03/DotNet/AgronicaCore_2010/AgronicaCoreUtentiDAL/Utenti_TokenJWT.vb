Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.Identity

Public Class Utenti_TokenJWT_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Token_ID As String,
                           ByVal objP_SuperServer As String,
                           ByVal objP_Server As String,
                           ByVal objP_Utenti As String,
                           ByVal Codice_Fiscale As String,
                           ByVal CoreWSBaseURL As String,
                           ByVal Utente_Username As String,
                           ByVal PivaSuperUser As String,
                           ByVal VersioneApp As String,
                           ByVal refreshToken As String,
                           ByVal dataCreazione As DateTime,
                           ByVal dataFineValidita As DateTime,
                           ByVal idDb As Integer,
                           ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_RefreshToken_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Utenti_Token_JWT (idToken, ")
            StrSQL.AppendLine("                           objP_SuperServer, ")
            StrSQL.AppendLine("                           objP_Server, ")
            StrSQL.AppendLine("                           objP_Utenti, ")
            StrSQL.AppendLine("                           Codice_Fiscale, ")
            StrSQL.AppendLine("                           CoreWSBaseURL, ")
            StrSQL.AppendLine("                           Username, ")
            StrSQL.AppendLine("                           PivaSuperUser, ")
            StrSQL.AppendLine("                           VersioneApp, ")
            StrSQL.AppendLine("                           refreshToken, ")
            StrSQL.AppendLine("                           Inviato, ")
            StrSQL.AppendLine("                           Data_Creazione, ")
            StrSQL.AppendLine("                           Data_Modifica, ")
            StrSQL.AppendLine("                           Validita_Inizio, ")
            StrSQL.AppendLine("                           Validita_Fine, ")
            StrSQL.AppendLine("                           IdDB )")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(Token_ID) & "', ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(objP_SuperServer) & "', ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(objP_Server) & "', ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(objP_Utenti) & "', ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(Codice_Fiscale) & "', ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(CoreWSBaseURL) & "', ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(Utente_Username) & "', ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(PivaSuperUser) & "', ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(VersioneApp) & "', ")
            StrSQL.AppendLine("    '" & Agro_SQL_SaveText(refreshToken) & "', ")

            StrSQL.AppendLine("    0, ")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDateTime(dataCreazione) & ", ")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDateTime(dataCreazione) & ", ")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDateTime(dataCreazione) & ", ")
            StrSQL.AppendLine("    " & Agro_SQL_SaveDateTime(dataFineValidita) & ", ")
            StrSQL.AppendLine("    " & Agro_SQL_SaveNum(idDb) & " ")
            StrSQL.AppendLine(")")
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

    Public Function Modifica(ByVal Token_ID As String,
                             ByVal Codice_Fiscale As String,
                             ByVal CoreWSBaseURL As String,
                             ByVal Utente_Username As String,
                             ByVal PivaSuperUser As String,
                             ByVal refreshToken As String,
                             ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_RefreshToken_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Utenti_Token_JWT SET ")
            StrSQL.AppendLine("       Codice_Fiscale = '" & Agro_SQL_SaveText(Codice_Fiscale) & "', ")
            StrSQL.AppendLine("       CoreWSBaseURL = '" & Agro_SQL_SaveText(CoreWSBaseURL) & "', ")
            StrSQL.AppendLine("       refreshToken = '" & Agro_SQL_SaveText(refreshToken) & "', ")
            StrSQL.AppendLine("       Data_Modifica  = " & Agro_SQL_SaveDateTime(Date.Now))

            StrSQL.AppendLine("WHERE idToken = '" & Agro_SQL_SaveText(Token_ID) & "' ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine("    AND PivaSuperUser ='" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            End If

            If Utente_Username <> "" Then
                StrSQL.AppendLine("    AND Username = '" & Agro_SQL_SaveText(Utente_Username) & "' ")
            End If

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

    Public Function EliminaTokenScaduti(ByRef objParametri As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = NameOf(EliminaTokenScaduti)

        Dim messaggioErrore As String = ""
        Dim stbSql As New System.Text.StringBuilder
        Dim risposta As Boolean = False

        Try

            stbSql.Length = 0

            Dim dateTimeNow As DateTime = DateTime.Now
            Dim todaysDate As DateTime = New DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day)

            stbSql.AppendLine("IF EXISTS (SELECT 1 FROM Utenti_Token_JWT ")
            stbSql.AppendLine("           WHERE          (Validita_Fine <= " & Agro_SQL_SaveDateTime(todaysDate))
            stbSql.AppendLine("           OR             IdDB IS NULL)) ")
            stbSql.AppendLine("BEGIN")
            stbSql.AppendLine("     DELETE FROM    Utenti_Token_JWT ")
            stbSql.AppendLine("     WHERE          (Validita_Fine <= " & Agro_SQL_SaveDateTime(todaysDate))
            stbSql.AppendLine("     OR             IdDB IS NULL) ")
            stbSql.AppendLine("END")

            risposta = EseguiQuery_Scrittura(objParametri, stbSql.ToString, nomeRoutine)

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            risposta = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return risposta

    End Function

    Public Function Elimina(ByVal Token_ID As String,
                            ByVal Utente_Username As String,
                            ByVal PivaSuperUser As String,
                            ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_RefreshToken_W.Elimina()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("DELETE FROM Utenti_Token_JWT ")
            StrSQL.AppendLine("WHERE idToken = '" & Agro_SQL_SaveText(Token_ID) & "' ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine("    AND PivaSuperUser ='" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            End If

            If Utente_Username <> "" Then
                StrSQL.AppendLine("    AND Username = '" & Agro_SQL_SaveText(Utente_Username) & "' ")
            End If

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

    Public Function AggiornaValiditaToken(ByVal Token_ID As String,
                                          ByVal PivaSuperUser As String,
                                          ByVal Utente_Username As String,
                                          ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_RefreshToken_W.AggiornaValiditaToken()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE Utenti_Token_JWT SET ")
            StrSQL.AppendLine("    Validita_Fine = " & Agro_SQL_SaveDateTime(Date.Now.AddDays(7)))
            StrSQL.AppendLine("WHERE Token_ID = '" & Agro_SQL_SaveText(Token_ID) & "'")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine("    AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            End If

            If Utente_Username <> "" Then
                StrSQL.AppendLine("    AND Utente_Username = '" & Agro_SQL_SaveText(Utente_Username) & "'")
            End If

            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp

    End Function

End Class

<CachedDataProvider(NameOf(Utenti_TokenJWT_R))>
Public Class Utenti_TokenJWT_R
    Inherits AgronicaCoreDataProvider.CachedDataProvider


    <Cacheable(True)>
    Public Function LeggiConBearerToken(ByVal bearerToken As String,
                                        ByRef objParametri_Super_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = NameOf(LeggiConBearerToken)

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try

            Dim query As String = CreateQuery("", "", "", "", "", "", bearerToken, "", "", objParametri_Super_Server)
            DT = EseguiQuery_Lettura(objParametri_Super_Server, query, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Super_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    <Cacheable(True)>
    Public Function LeggiConAuthCookie(ByVal authCookie As String,
                                       ByRef objParametri_Super_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = NameOf(LeggiConAuthCookie)

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try

            Dim query As String = CreateQuery("", "", "", "", "", authCookie, "", "", "", objParametri_Super_Server)
            DT = EseguiQuery_Lettura(objParametri_Super_Server, query, NomeRoutine)

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Super_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Function Leggi(ByVal Token_ID As String,
                          ByVal Refresh_Token As String,
                          ByVal PivaSuperUser As String,
                          ByVal Username As String,
                          ByVal objP_Server As String,
                          ByVal authCookie As String,
                          ByVal bearerToken As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_RefreshToken_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable

        Try

            Dim query As String = CreateQuery(Token_ID, Refresh_Token, PivaSuperUser, Username, objP_Server, authCookie, bearerToken, xFiltroAggiuntivo, xOrderBy, objParametri)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, query, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Private Function CreateQuery(ByVal Token_ID As String,
                          ByVal Refresh_Token As String,
                          ByVal PivaSuperUser As String,
                          ByVal Username As String,
                          ByVal objP_Server As String,
                          ByVal authCookie As String,
                          ByVal bearerToken As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri) As String

        Dim StrSQL As New System.Text.StringBuilder

        StrSQL.Length = 0
        StrSQL.AppendLine("SELECT * ")
        StrSQL.AppendLine("FROM Utenti_Token_JWT ")
        StrSQL.AppendLine("WHERE 1=1 ")

        If Token_ID <> "" Then
            StrSQL.AppendLine("    AND idToken = '" & Agro_SQL_SaveText(Token_ID) & "' ")
        End If

        If Refresh_Token <> "" Then
            StrSQL.AppendLine("    AND refreshToken = '" & Agro_SQL_SaveText(Refresh_Token) & "' ")
        End If

        If PivaSuperUser <> "" Then
            StrSQL.AppendLine("    AND PivaSuperUser ='" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
        End If

        If Username <> "" Then
            StrSQL.AppendLine("    AND Username ='" & Agro_SQL_SaveText(Username) & "' ")
        End If

        If objP_Server <> "" Then
            StrSQL.AppendLine("    AND objP_Server ='" & Agro_SQL_SaveText(objP_Server) & "' ")
        End If

        If authCookie <> "" Then
            StrSQL.AppendLine("    AND AuthCookie ='" & Agro_SQL_SaveText(authCookie) & "' ")
        End If

        If bearerToken <> "" Then
            StrSQL.AppendLine("    AND BearerToken='" & Agro_SQL_SaveText(bearerToken) & "' ")
        End If

        '--------------------------------------------------------------------------
        If xFiltroAggiuntivo <> "" Then
            StrSQL.AppendLine("    AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If

        '--------------------------------------------------------------------------
        If xOrderBy <> "" Then
            StrSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
        End If

        Return StrSQL.ToString()

    End Function

End Class
