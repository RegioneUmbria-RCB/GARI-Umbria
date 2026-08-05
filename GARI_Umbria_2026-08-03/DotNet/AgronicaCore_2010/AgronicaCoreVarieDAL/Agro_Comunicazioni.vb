Imports System.Text
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class Agro_Comunicazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Cod_RisUm As Integer,
                                             ByVal Elem_Cod As Integer,
                                             ByVal Pro_Cod As Integer,
                                             ByVal Mat_Cod As Integer,
                                             ByRef Id As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Agro_Comunicazioni_R.Leggi()"

        Dim risposta As String = ""

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.Append(" SELECT * ")
            strSql.Append("  from Agro_Comunicazioni ")

            strSql.Append(" Where Piva_SuperUser = '" & Piva_SuperUser & "'")
            strSql.Append(" And Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.Append(" And Sa_Cod = " & Sa_Cod & " ")
            strSql.Append(" And Cod_RisUm = " & Cod_RisUm & " ")
            strSql.Append(" And Elem_Cod = " & Elem_Cod & " ")
            strSql.Append(" And Pro_Cod = " & Pro_Cod & " ")
            strSql.Append(" And Mat_Cod = " & Mat_Cod & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        If dt.Rows.Count > 0 Then
            risposta = dt.Rows(0)("Testo")
            Id = dt.Rows(0)("Id")
        Else
            Id = 0
        End If
        Return risposta

    End Function

End Class

Public Class Agro_Comunicazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '###############################################################################
    Public Function Scrivi(ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Modulo As Integer,
                               ByVal Testo As String,
                               ByVal Soluzione As String,
                                ByVal Stato As Integer,
                               ByVal Testo_Parametri As String,
                               ByVal Soluzione_Parametri As String,
                               ByVal Cod_RisUm As Integer,
                                ByVal Colore As Integer,
                               ByVal Username As String,
                               ByVal Modalita As Integer,
                               ByVal Elem_Cod As Integer,
                               ByVal Pro_Cod As Integer,
                               ByVal Mat_Cod As Integer,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Agro_Comunicazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" INSERT INTO Agro_Comunicazioni ")
            strSql.AppendLine("             ( ")
            strSql.AppendLine("              Piva_SuperUser, Piva, Sa_Cod, Modulo, Stato,  ")
            strSql.AppendLine("              Testo, Soluzione, Testo_Parametri, Soluzione_Parametri, Username, ")
            strSql.AppendLine("              Cod_RisUm, Colore, Modalita, Elem_Cod, Pro_Cod, Mat_Cod, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica ")
            strSql.AppendLine("              ) ")
            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Modulo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Stato) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Testo) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Soluzione) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Testo_Parametri) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Soluzione_Parametri) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Username) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Colore) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Modalita) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

    '###############################################################################
    Public Function Aggiorna(ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Modulo As Integer,
                               ByVal Testo As String,
                               ByVal Soluzione As String,
                               ByVal Stato As Integer,
                               ByVal Testo_Parametri As String,
                               ByVal Soluzione_Parametri As String,
                               ByVal Cod_RisUm As Integer,
                               ByVal Colore As Integer,
                               ByVal Username As String,
                               ByVal Modalita As Integer,
                               ByVal Elem_Cod As Integer,
                               ByVal Pro_Cod As Integer,
                               ByVal Mat_Cod As Integer,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Agro_Comunicazioni_W.Aggiorna()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try
            Dim risposta As String = ""
            Dim Id As Integer = 0
            Dim leggi As New AgronicaCoreVarieDAL.Agro_Comunicazioni_R
            risposta =
                leggi.Leggi(Piva, Sa_Cod, Cod_RisUm, Elem_Cod, Pro_Cod, Mat_Cod, Id, objParametri)

            Dim scrivi As New AgronicaCoreVarieDAL.Agro_Comunicazioni_W

            If Id <> 0 Then
                xRisp = scrivi.Modifica(Id, Piva, Sa_Cod, Modulo, Testo, Soluzione, Stato, Testo_Parametri, Soluzione_Parametri, Cod_RisUm, Colore,
                                         objParametri.UtenteUsername, Modalita, Elem_Cod, Pro_Cod, Mat_Cod, objParametri)
            Else
                xRisp = scrivi.Scrivi(Piva, Sa_Cod, Modulo, Testo, Soluzione, Stato, Testo_Parametri, Soluzione_Parametri, Cod_RisUm, Colore,
                                         objParametri.UtenteUsername, Modalita, Elem_Cod, Pro_Cod, Mat_Cod, objParametri)
            End If

        Catch ex As Exception
        End Try

        Return xRisp

    End Function


    '###############################################################################
    Public Function Modifica(ByVal Id As Integer,
                             ByVal Piva As String,
                               ByVal Sa_Cod As Integer,
                               ByVal Modulo As Integer,
                               ByVal Testo As String,
                               ByVal Soluzione As String,
                                ByVal Stato As Integer,
                               ByVal Testo_Parametri As String,
                               ByVal Soluzione_Parametri As String,
                               ByVal Cod_RisUm As Integer,
                                ByVal Colore As Integer,
                               ByVal Username As String,
                               ByVal Modalita As Integer,
                               ByVal Elem_Cod As Integer,
                               ByVal Pro_Cod As Integer,
                               ByVal Mat_Cod As Integer,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreVarieDAL.Agro_Comunicazioni_W.Aggiorna()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" UPDATE  Agro_Comunicazioni ")
            strSql.AppendLine(" SET  ")
            strSql.AppendLine(" Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine(" , Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" , Sa_Cod = '" & Agro_SQL_SaveText(Sa_Cod) & "'  ")
            strSql.AppendLine(" , Modulo =  " & Agro_SQL_SaveNum(Modulo) & "  ")
            strSql.AppendLine(" , Stato =  " & Agro_SQL_SaveNum(Stato) & "  ")
            strSql.AppendLine(" , Testo = '" & Agro_SQL_SaveText(Testo) & "'  ")
            strSql.AppendLine(" , Soluzione  = '" & Agro_SQL_SaveText(Soluzione) & "'  ")
            strSql.AppendLine(" , Testo_Parametri = '" & Agro_SQL_SaveText(Testo_Parametri) & "'  ")
            strSql.AppendLine(" , Soluzione_Parametri = '" & Agro_SQL_SaveText(Soluzione_Parametri) & "'  ")
            strSql.AppendLine(" , Cod_RisUm =  " & Agro_SQL_SaveNum(Cod_RisUm) & "  ")
            strSql.AppendLine(" , Colore = " & Agro_SQL_SaveNum(Colore) & "  ")
            strSql.AppendLine(" , UserName = '" & Agro_SQL_SaveText(Username) & "'  ")
            strSql.AppendLine(" , Modalita = " & Agro_SQL_SaveNum(Modalita) & "  ")
            strSql.AppendLine(" , Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.AppendLine(" , Pro_Cod =  " & Agro_SQL_SaveNum(Pro_Cod) & "  ")
            strSql.AppendLine(" , Mat_Cod =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.AppendLine(" , Data_Modifica   = " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine(" , UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            strSql.AppendLine(" WHERE ID = " & Id & " ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

        End Try

        Return xRisp

    End Function

End Class
