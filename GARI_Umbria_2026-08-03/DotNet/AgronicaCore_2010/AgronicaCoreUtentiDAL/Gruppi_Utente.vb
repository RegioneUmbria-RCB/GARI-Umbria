Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class Gruppi_Utente_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Gruppi_Utente_cod As Int32,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Gruppi_Utente_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Tipologia_Cod
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT  * ")
            StrSQL.AppendLine(" FROM    Gruppi_Utente ")
            StrSQL.AppendLine(" WHERE   1=1 ")

            If Gruppi_Utente_cod <> 0 Then
                StrSQL.AppendLine(" AND (Gruppi_Utente_cod = " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Gruppi_Utente_des ")
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

    Public Function LeggiGruppiDaUtente(ByRef objParametri As AgronicaCoreParametri, ByVal Optional Utente As String = "") As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Gruppi_Utente_R.LeggiGruppiDaUtente()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim UserName = objParametri.UtenteUsername
        If Not Utente.Equals("") Then
            UserName = Utente
        End If

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT distinct Gruppi_Utente_cod ")
            StrSQL.AppendLine(" FROM Utenti_xGruppi_Utente ")
            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" UserName = '{0}' ", Agro_SQL_SaveText(UserName)))

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

Public Class Gruppi_Utente_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(ByVal Gruppi_Utente_cod As Int32, _
                           ByVal Gruppi_Utente_des As String, _
                           ByVal Gruppi_Utente_Identificativo As String, _
                           ByVal Validita_Inizio As Date, _
                           ByVal Validita_Fine As Date, _
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                           ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Gruppi_Utente_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Gruppi_Utente       ")
            StrSQL.AppendLine("                  ( Gruppi_Utente_cod,   Gruppi_Utente_des,  Gruppi_Utente_Identificativo,")
            StrSQL.AppendLine("                    Inviato,             DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione,      Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione,  UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,     Validita_Fine ")
            StrSQL.AppendLine("                    ) ")

            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("           " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Gruppi_Utente_des) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Gruppi_Utente_Identificativo) & "' ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
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

    '##############################################################################################
    Public Function Modifica(ByVal Gruppi_Utente_cod As Int32, _
                             ByVal Gruppi_Utente_des As String, _
                             ByVal Gruppi_Utente_Identificativo As String, _
                             ByVal xFiltroAggiuntivo As String, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Gruppi_Utente_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE Gruppi_Utente SET ")
            StrSQL.AppendLine("       Gruppi_Utente_des = '" & Agro_SQL_SaveText(Gruppi_Utente_des) & "'")
            StrSQL.AppendLine("     , Gruppi_Utente_Identificativo = '" & Agro_SQL_SaveText(Gruppi_Utente_Identificativo) & "'")
            StrSQL.AppendLine("     , Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("     , UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("WHERE Gruppi_Utente_cod = " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & " ")
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


    '##############################################################################################
    Public Function Cancella(ByVal Gruppi_Utente_cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Gruppi_Utente_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.AppendLine(" UPDATE Gruppi_Utente ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE Inviato >= 0")

                If Gruppi_Utente_cod <> 0 Then
                    StrSQL.Append(" AND  Gruppi_Utente_cod =  " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & " ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Gruppi_Utente ")
                StrSQL.AppendLine(" WHERE    1=1 ")

                If Gruppi_Utente_cod <> 0 Then
                    StrSQL.AppendLine(" AND  Gruppi_Utente_cod =  " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & " ")
                End If

            End If
            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

End Class


