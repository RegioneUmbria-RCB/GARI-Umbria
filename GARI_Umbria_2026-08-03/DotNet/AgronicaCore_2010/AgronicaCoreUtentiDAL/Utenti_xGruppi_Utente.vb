Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Utenti_xGruppi_Utente_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiJoinGruppi(
                         ByVal UserName As String,
                         ByVal Gruppi_Utente_cod As Integer,
                         ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        Optional dataValidita As DateTime = AGRODATAINIZIO
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0
            strSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSQL.Append(" SELECT ug.*, g.Gruppi_Utente_Identificativo, g.Gruppi_Utente_des, g.ConfigurazioniAggiuntive " + vbCrLf)
            strSQL.Append(" FROM Utenti_xGruppi_Utente ug " + vbCrLf)
            strSQL.Append(" inner join  Gruppi_Utente g " + vbCrLf)
            strSQL.Append("    on ug.gruppi_Utente_Cod = g.gruppi_Utente_Cod " + vbCrLf)
            If dataValidita <> AGRODATAINIZIO Then
                strSQL.AppendLine($" and g.Validita_Inizio <= {Agro_SQL_SaveDate(dataValidita)} and g.Validita_Fine >= {Agro_SQL_SaveDate(dataValidita)} ")
            End If
            strSQL.Append(" WHERE 1=1 " + vbCrLf)

            If UserName <> "" Then
                strSQL.Append(" AND ug.[Username] = '" & Agro_SQL_SaveText(UserName) & "'" & vbCrLf)
            End If

            If Gruppi_Utente_cod <> 0 Then
                strSQL.Append(" AND ug.Gruppi_Utente_Cod = " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & vbCrLf)
            End If

            If dataValidita <> AGRODATAINIZIO Then
                strSQL.AppendLine($" and ug.Validita_Inizio <= {Agro_SQL_SaveDate(dataValidita)} and ug.Validita_Fine >= {Agro_SQL_SaveDate(dataValidita)} ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   ug.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   ug.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    '##############################################################################################
    Public Function Leggi(
                         ByVal UserName As String,
                         ByVal Gruppi_Utente_cod As String,
                         ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0
            strSQL.Append(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSQL.Append(" SELECT * " + vbCrLf)
            strSQL.Append(" FROM Utenti_xGruppi_Utente " + vbCrLf)
            strSQL.Append(" WHERE 1=1 " + vbCrLf)

            If UserName <> "" Then
                strSQL.Append(" AND [Username] = '" & Agro_SQL_SaveText(UserName) & "'" & vbCrLf)
            End If

            If Gruppi_Utente_cod <> 0 Then
                strSQL.Append(" AND Gruppi_Utente_Cod = " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT





    End Function

    '##############################################################################################
    Public Function LeggiUtenti( _
                                ByVal UserName As String, _
                                ByVal Gruppi_Utente_cod As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.LeggiUtenti()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            strSQL.Length = 0

            strSQL.Append(" SELECT Utenti_xGruppi_Utente.Gruppi_Utente_cod, Gruppi_Utente_des, Utenti_Dettagli.UserNameCommerciale AS Qualifica, " + vbCrLf)
            strSQL.Append(" Utenti_Dettagli.[UserName], CASE RAG_SOC WHEN '' THEN Cognome + ' ' + Nome ELSE RAG_SOC END AS Dettagli" + vbCrLf)


            strSQL.Append(" FROM Utenti_Dettagli " + vbCrLf)
            strSQL.Append(" LEFT JOIN Utenti_xGruppi_Utente ON Utenti_Dettagli.Username = Utenti_xGruppi_Utente.Username " + vbCrLf)
            strSQL.Append(" LEFT JOIN Gruppi_Utente ON Gruppi_Utente.Gruppi_Utente_cod = Utenti_xGruppi_Utente.Gruppi_Utente_cod " + vbCrLf)
            strSQL.Append(" WHERE 1=1 " + vbCrLf)

            If UserName <> "" Then
                strSQL.Append(" AND Utenti_xGruppi_Utente.[Username] = '" & Agro_SQL_SaveText(UserName) & "'" & vbCrLf)
            End If

            If Gruppi_Utente_cod <> 0 Then
                strSQL.Append(" AND Utenti_xGruppi_Utente.Gruppi_Utente_Cod = " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & vbCrLf)
            End If

            If xFiltroAggiuntivo <> "" Then
                strSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    ' strSQL.Append(" AND   Utenti_xGruppi_Utente.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    'strSQL.Append(" AND   Utenti_xGruppi_Utente.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    '##############################################################################################
    Public Function Leggi_IdentificativoGruppoUtenti_Singolo( _
                                ByVal UserName As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As String

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Risultato As String = ""

        Try
            strSQL.Length = 0

            strSQL.Append(" SELECT  Gruppi_Utente.Gruppi_Utente_cod, " + vbCrLf)
            strSQL.Append("         Gruppi_Utente.Gruppi_Utente_des, " + vbCrLf)
            strSQL.Append("         Gruppi_Utente.Gruppi_Utente_Identificativo, " + vbCrLf)
            strSQL.Append("         Utenti_xGruppi_Utente.UserName" + vbCrLf)

            strSQL.Append(" FROM    Gruppi_Utente INNER JOIN" + vbCrLf)
            strSQL.Append("         Utenti_xGruppi_Utente ON Gruppi_Utente.Gruppi_Utente_cod = Utenti_xGruppi_Utente.Gruppi_Utente_cod " + vbCrLf)

            strSQL.Append(" WHERE   Utenti_xGruppi_Utente.[Username] = '" & Agro_SQL_SaveText(UserName) & "' " + vbCrLf)
         
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso _
               DT.Rows.Count > 0 AndAlso _
               Not IsDBNull(DT.Rows(0).Item("Gruppi_Utente_Identificativo")) Then

                Risultato = DT.Rows(0).Item("Gruppi_Utente_Identificativo").ToString

            Else
                Risultato = ""
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Risultato = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Risultato

    End Function

    '##############################################################################################
    Public Function Leggi_IdentificativoGruppoUtenti(
                                ByVal UserName As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Leggi_IdentificativoGruppoUtenti()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim strSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Risultato As String = ""

        Try
            strSQL.Length = 0

            strSQL.Append(" SELECT  Gruppi_Utente.Gruppi_Utente_cod, " + vbCrLf)
            strSQL.Append("         Gruppi_Utente.Gruppi_Utente_des, " + vbCrLf)
            strSQL.Append("         Gruppi_Utente.Gruppi_Utente_Identificativo, " + vbCrLf)
            strSQL.Append("         Utenti_xGruppi_Utente.UserName" + vbCrLf)

            strSQL.Append(" FROM    Gruppi_Utente INNER JOIN" + vbCrLf)
            strSQL.Append("         Utenti_xGruppi_Utente ON Gruppi_Utente.Gruppi_Utente_cod = Utenti_xGruppi_Utente.Gruppi_Utente_cod " + vbCrLf)

            strSQL.Append(" WHERE   Utenti_xGruppi_Utente.[Username] = '" & Agro_SQL_SaveText(UserName) & "' " + vbCrLf)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, strSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Risultato = ""
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiUtentiDaGruppo(
        groups As IEnumerable(Of Integer),
        selezione As enumSelezioneVariabile,
        objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.LeggiUtentiDaGruppo()"
        Dim strSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim xIn = groups.Select(Function(cod) cod.ToString).
            Aggregate(Function(c1, c2) c1 & "," & c2)
        Try

            Select Case selezione
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    strSQL.AppendLine(" SELECT UserName, Gruppi_Utente_cod ")
                    strSQL.AppendLine(" FROM Utenti_xGruppi_Utente ")

                Case Else
                    strSQL.AppendLine(" SELECT * ")
                    strSQL.AppendLine(" FROM Utenti_xGruppi_Utente ")
                    strSQL.AppendLine(" LEFT JOIN Utenti_Dettagli ON Utenti_xGruppi_Utente.UserName = Utenti_Dettagli.UserName ")

            End Select
            strSQL.AppendLine(" WHERE Utenti_xGruppi_Utente.Gruppi_Utente_cod IN (" & Agro_SQL_Save_Clausola_IN(xIn) & ")")
            '--------------------------------------------------------------------------
            Return EseguiQuery_Lettura(objParametri_Utenti, strSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

End Class








Public Class Utenti_xGruppi_Utente_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Gruppi_Utente_cod As Int32, _
                           ByVal Username As String, _
                           ByVal Validita_Inizio As Date, _
                           ByVal Validita_Fine As Date, _
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                           ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_xGruppi_Utente_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Utenti_xGruppi_Utente       ")
            StrSQL.Append("                  ( Gruppi_Utente_cod,   Username,  ")
            StrSQL.Append("                    Inviato,             DataInvio, ")
            StrSQL.Append("                    Data_Creazione,      Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione,  UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,     Validita_Fine ")
            StrSQL.Append("                    ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("           " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & " ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Username) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
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

    ''##############################################################################################
    'Public Function Modifica(ByVal Gruppi_Utente_cod As Int32, _
    '                         ByVal Username As String, _
    '                         ByVal xFiltroAggiuntivo As String, _
    '                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                         ) As Boolean

    '    Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Gruppi_Utente_W.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append("UPDATE Gruppi_Utente SET ")
    '        StrSQL.Append("       Gruppi_Utente_des = '" & Agro_SQL_SaveText(Gruppi_Utente_des) & "'")
    '        StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
    '        StrSQL.Append(" WHERE Piva_SuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
    '        StrSQL.Append(" AND   Gruppi_Utente_cod = " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & " ")
    '        '---------------------------------------------

    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function


    '##############################################################################################
    Public Function Cancella(ByVal Gruppi_Utente_cod As Int32, _
                             ByVal Username As String, _
                             ByVal xFiltroAggiuntivo As String, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_xGruppi_Utente_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Utenti_xGruppi_Utente ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Inviato >= 0")

                If Gruppi_Utente_cod <> 0 Then
                    StrSQL.Append(" AND  Gruppi_Utente_cod =  " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & " ")
                End If

                If Username <> "" Then
                    StrSQL.Append(" AND  Username = '" & Agro_SQL_SaveText(Username) & "' ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Utenti_xGruppi_Utente ")
                StrSQL.Append(" WHERE    1=1 ")

                If Gruppi_Utente_cod <> 0 Then
                    StrSQL.Append(" AND  Gruppi_Utente_cod =  " & Agro_SQL_SaveNum(Gruppi_Utente_cod) & " ")
                End If

                If Username <> "" Then
                    StrSQL.Append(" AND  Username = '" & Agro_SQL_SaveText(Username) & "' ")
                End If

            End If
            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

