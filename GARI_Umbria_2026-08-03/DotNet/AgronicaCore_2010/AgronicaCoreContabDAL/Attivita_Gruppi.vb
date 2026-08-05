Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Attivita_Gruppi_R
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Public Function Leggi(ByVal Piva As String,
                          ByVal Id_Attivita_Gruppo As Integer,
                          ByVal Tipologia_Attivita As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Gruppi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT i.rag_soc, ag.*, CASE WHEN Sa_Cod = 0 THEN 'No' ELSE 'Sì' END AS Sa_Cod_Desc, '' As Tipologia_Attivita_Desc ")
            StrSQL.Append(" FROM  Attivita_Gruppi ag ")
            StrSQL.Append(" LEFT JOIN imprese i on i.piva = ag.piva ")
            StrSQL.Append(" WHERE ag.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   ag.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   ag.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND (ag.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR ag.Sa_Cod = -1 )")
            End If

            If ID_Attivita_Gruppo <> 0 Then
                StrSQL.Append(" AND ag.ID_Attivita_Gruppo = " & Agro_SQL_SaveNum(Id_Attivita_Gruppo) & " ")
            End If

            If Tipologia_Attivita <> 0 Then
                StrSQL.Append(" AND ag.Tipologia_Attivita = " & Agro_SQL_SaveNum(Tipologia_Attivita) & " ")
            End If



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   ag.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   ag.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY ag.Tipologia_Attivita ASC, ag.Ordine ASC, ag.Desc_Gruppo ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class


Public Class Attivita_Gruppi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Attivita_Gruppo As Integer,
                           ByVal Desc_Gruppo As String,
                           ByVal Sigla As String,
                           ByVal Ordine As Integer,
                           ByVal Tipologia_Attivita As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Gruppi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Attivita_Gruppi ")
            StrSQL.AppendLine("            (Piva_SuperUser,     Piva,  Sa_Cod, ID_Attivita_Gruppo, ")
            StrSQL.AppendLine("             Desc_Gruppo,        Sigla, Ordine, Tipologia_Attivita, ")

            StrSQL.AppendLine("             Inviato,            DataInvio, ")
            StrSQL.AppendLine("             Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("             UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("             Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("             ) ")
            
            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_Attivita_Gruppo) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Desc_Gruppo) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Sigla) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Ordine) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipologia_Attivita) & " ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function ModificaPuntuale(ByVal ID_Attivita_Gruppo As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Piva As String = Nothing,
                                     Optional ByVal Sa_Cod As Integer? = Nothing,
                                     Optional ByVal Desc_Gruppo As String = Nothing,
                                     Optional ByVal Sigla As String = Nothing,
                                     Optional ByVal Ordine As Integer? = Nothing,
                                     Optional ByVal Tipologia_Attivita As Integer? = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Gruppi_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If ID_Attivita_Gruppo = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_Attivita_Gruppo obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Attivita_Gruppi ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Not IsNothing(Sa_Cod) Then
                strSql.AppendLine("   , Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Not IsNothing(Desc_Gruppo) Then
                strSql.AppendLine("   , Desc_Gruppo = '" & Agro_SQL_SaveText(Desc_Gruppo) & "' ")
            End If

            If Not IsNothing(Sigla) Then
                strSql.AppendLine("   , Sigla = '" & Agro_SQL_SaveText(Sigla) & "' ")
            End If

            If Not IsNothing(Ordine) Then
                strSql.AppendLine("   , Ordine = " & Agro_SQL_SaveNum(Ordine) & " ")
            End If

            If Not IsNothing(Tipologia_Attivita) Then
                strSql.AppendLine("   , Tipologia_Attivita = " & Agro_SQL_SaveNum(Tipologia_Attivita) & " ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND ID_Attivita_Gruppo = " & Agro_SQL_SaveNum(ID_Attivita_Gruppo) & " ")

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


    '#########################################################################
    Public Function Cancella(ByVal Id_Attivita_Gruppo As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Gruppi_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Attivita_Gruppi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Attivita_Gruppi ")
                StrSQL.Append(" WHERE Inviato = 0")

            End If

            StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Id_Attivita_Gruppo = " & Agro_SQL_SaveNum(Id_Attivita_Gruppo) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
