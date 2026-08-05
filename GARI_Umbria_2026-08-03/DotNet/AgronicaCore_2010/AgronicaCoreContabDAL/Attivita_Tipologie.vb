Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text
Imports AgronicaCoreDataProvider.My.Resources

Public Class Attivita_Tipologie_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Private Function Tabella_Da_Lingua(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Select Case objParametri.Lingua_Cod
            Case 1 : Return "Attivita_Gruppi_Tipologie"
            Case 2 : Return "Attivita_Gruppi_Tipologie_XLingue_en"
            Case 3 : Return "Attivita_Gruppi_Tipologie_XLingue_fr"
            Case Else : Return ""
        End Select
    End Function

    Public Function Leggi(ByVal Piva As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Tipologie_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT i.rag_soc, agt.*, CASE WHEN agt.Sa_Cod = 0 THEN '" & Gias.No & "' ELSE '" & Gias.Sì & "' END AS Sa_Cod_Desc, ")
            StrSQL.Append(" CASE WHEN agt.ID_Tipologia_Attivita = 1 THEN '" & Gias.Prima & "'  ")
            StrSQL.Append(" WHEN agt.ID_Tipologia_Attivita = 2 THEN '" & Gias.Seconda & "' ELSE '" & Gias.Terza & "' END As ID_Tipologia_Desc ")
            StrSQL.Append(" FROM  Attivita_Gruppi_Tipologie agt ")
            StrSQL.Append(" LEFT JOIN Imprese i on i.piva = agt.piva ")
            StrSQL.Append(" WHERE agt.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   agt.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   agt.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND (agt.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR agt.Sa_Cod = -1 )")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   agt.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   agt.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY agt.Piva ASC ")
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

    '''<remarks>
    ''' Il metodo restituisce le descrizione delle 3 tipologie dei gruppi attivita. Se Piva è presente trova solo quelli relativi a quella Piva, 
    ''' altrimenti trova solo quelli pubblici
    '''</remarks>
    Public Function Trova_Tipologie(ByVal Piva As String,
                                    ByVal primoRichiesto As Boolean,
                                    ByVal secondoRichiesto As Boolean,
                                    ByVal terzoRichiesto As Boolean,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Tipologie_R.Trova_Tipologie()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT ID_Tipologia_Attivita As Tipologia_Attivita, Desc_Tipologia_Attivita As Tipologia_Attivita_Desc")
            StrSQL.Append(" FROM  " + Tabella_Da_Lingua(objParametri) + " ")
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' AND Sa_Cod = 0")
            Else
                StrSQL.AppendLine(" AND Sa_Cod = -1 ")
            End If

            If primoRichiesto OrElse secondoRichiesto OrElse terzoRichiesto Then
                StrSQL.AppendLine(" AND (")
                If primoRichiesto Then
                    StrSQL.AppendLine(" ID_Tipologia_Attivita = 1 ")
                    If secondoRichiesto Then
                        StrSQL.AppendLine(" OR ID_Tipologia_Attivita = 2 ")
                    End If
                    If terzoRichiesto Then
                        StrSQL.AppendLine(" OR ID_Tipologia_Attivita = 3 ")
                    End If
                ElseIf secondoRichiesto Then
                    StrSQL.AppendLine(" ID_Tipologia_Attivita = 2 ")
                    If terzoRichiesto Then
                        StrSQL.AppendLine(" OR ID_Tipologia_Attivita = 3 ")
                    End If
                ElseIf terzoRichiesto Then
                    StrSQL.AppendLine(" ID_Tipologia_Attivita = 3 ")
                End If

                StrSQL.AppendLine(" ) ")

            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY ID_Tipologia_Attivita ASC, piva ASC ")
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

    Public Function Trova_TipologieXLingua(ByVal piva As String,
                                           ByVal Id_Tipologia As Integer,
                                           ByVal lingua_Cod As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Attivita_Tipologie_R.Trova_TipologieXLingua()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" SELECT Attivita_Gruppi_Tipologie_XLingue.*,Lingue.Lingua_Cod , Lingue.Nome ")
            strSql.AppendLine(" FROM   Attivita_Gruppi_Tipologie_XLingue ")
            strSql.AppendLine(" LEFT JOIN Lingue ")
            strSql.AppendLine(" ON  Lingue.Lingua_Cod = Attivita_Gruppi_Tipologie_XLingue.Lingua_Cod")
            strSql.AppendLine(" WHERE 1 = 1")
            'If piva <> "" Then
            '    strSql.AppendLine(" AND Attivita_Gruppi_Tipologie_XLingue.Piva  = '" & Agro_SQL_SaveText(piva) & "'   ")
            'End If

            If Id_Tipologia <> 0 Then
                strSql.AppendLine(" AND Attivita_Gruppi_Tipologie_XLingue.Id_Tipologia = " & Agro_SQL_SaveNum(Id_Tipologia) & "   ")
            End If

            If lingua_Cod <> 0 Then
                strSql.AppendLine(" AND Attivita_Gruppi_Tipologie_XLingue.Lingua_Cod = " & Agro_SQL_SaveNum(lingua_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
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


Public Class Attivita_Tipologie_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Id_Tipologia As Integer,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Tipologia_Attivita As Integer,
                           ByVal Desc_Tipologia_Attivita As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Tipologia_W.Scrivi()"

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

            StrSQL.AppendLine("INSERT INTO Attivita_Gruppi_Tipologie ")
            StrSQL.AppendLine("            (Piva_SuperUser,     Piva,  Sa_Cod, ID_Tipologia, ")
            StrSQL.AppendLine("             Desc_Tipologia_Attivita,  ID_Tipologia_Attivita, ")

            StrSQL.AppendLine("             Inviato,            DataInvio, ")
            StrSQL.AppendLine("             Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("             UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("             Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("             ) ")

            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Tipologia) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Desc_Tipologia_Attivita) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Id_Tipologia_Attivita) & "' ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
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

    ''#########################################################################
    Public Function Modifica(ByVal ID_Tipologia As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Piva As String = Nothing,
                                     Optional ByVal Sa_Cod As Integer? = Nothing,
                                     Optional ByVal Id_Tipologia_Attivita As Integer = Nothing,
                                     Optional ByVal Desc_Tipologia_Attivita As String = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Tipologie_W.ModificaPuntuale()"

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

            If ID_Tipologia = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Tipologia obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Attivita_Gruppi_Tipologie ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Not IsNothing(Sa_Cod) Then
                strSql.AppendLine("   , Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Not IsNothing(Desc_Tipologia_Attivita) Then
                strSql.AppendLine("   , Desc_Tipologia_Attivita = '" & Agro_SQL_SaveText(Desc_Tipologia_Attivita) & "' ")
            End If

            If Not IsNothing(Id_Tipologia_Attivita) Then
                strSql.AppendLine("   , Id_Tipologia_Attivita = '" & Agro_SQL_SaveText(Id_Tipologia_Attivita) & "' ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")

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


    ''#########################################################################
    Public Function Cancella(ByVal Id_Tipologia As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Tipologie_W.Cancella()"

        Cancella_XLingue(Id_Tipologia, 0, "", objParametri)

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Attivita_Gruppi_Tipologie ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Attivita_Gruppi_Tipologie ")
                StrSQL.Append(" WHERE Inviato = 0")

            End If

            StrSQL.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Id_Tipologia = " & Agro_SQL_SaveNum(Id_Tipologia) & " ")

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


    ''#########################################################################
    Public Function Scrivi_XLingue(ByVal Id_Tipologia As Integer,
                                   ByVal Lingua_Cod As Integer,
                                   ByVal Desc_Tipologia_Attivita As String,
                                   ByVal Validita_Inizio As Date,
                                   ByVal Validita_Fine As Date,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                   Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                   Optional ByVal username_creazione As String = "",
                                   Optional ByVal username_modifica As String = ""
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Tipologia_W.Scrivi_XLingue()"

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

            StrSQL.AppendLine("INSERT INTO Attivita_Gruppi_Tipologie_XLingue ")
            StrSQL.AppendLine("            (Lingua_Cod, Id_Tipologia, ")
            StrSQL.AppendLine("             Desc_Tipologia_Attivita, ")

            StrSQL.AppendLine("             Inviato,            DataInvio, ")
            StrSQL.AppendLine("             Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("             UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("             Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("             ) ")

            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("          " & Agro_SQL_SaveNum(Lingua_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Tipologia) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Desc_Tipologia_Attivita) & "' ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
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

    ''#########################################################################
    Public Function Modifica_XLingue(ByVal Id_Tipologia As Integer,
                                     ByVal Lingua_Cod As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Desc_Tipologia_Attivita As String = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Tipologie_W.Modifica_XLingue()"

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

            If Id_Tipologia = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Tipologia obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Attivita_Gruppi_Tipologie_XLingue ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Desc_Tipologia_Attivita) Then
                strSql.AppendLine("   , Desc_Tipologia_Attivita = '" & Agro_SQL_SaveText(Desc_Tipologia_Attivita) & "' ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE ID_Tipologia = " & Agro_SQL_SaveNum(Id_Tipologia) & " ")
            strSql.AppendLine(" AND Lingua_Cod = " & Agro_SQL_SaveNum(Lingua_Cod) & " ")

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


    ''#########################################################################
    Public Function Cancella_XLingue(ByVal Id_Tipologia As Integer,
                                     ByVal Lingua_Cod As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Attivita_Tipologie_W.Cancella_XLingue()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Attivita_Gruppi_Tipologie_XLingue ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Attivita_Gruppi_Tipologie_XLingue ")
                StrSQL.Append(" WHERE Inviato = 0")

            End If

            If Lingua_Cod > 0 Then
                StrSQL.Append(" AND Lingua_Cod = '" & Lingua_Cod.ToString & "' ")
            End If

            StrSQL.Append(" AND Id_Tipologia = " & Id_Tipologia.ToString & " ")

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
