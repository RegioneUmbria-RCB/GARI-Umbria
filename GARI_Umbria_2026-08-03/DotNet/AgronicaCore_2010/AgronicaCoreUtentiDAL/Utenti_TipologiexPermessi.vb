Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Utenti_TipologiexPermessi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Tipologia_Cod As Int32, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Tipologia_Cod
        '   FiltroAggiuntivo
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Tipologia_Cod, Id_Servizio, Id_Attivita, Id_Operazione ")
                    StrSQL.Append(" FROM    Utenti_TipologiexPermessi ")
                    StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

                    If Tipologia_Cod <> 0 Then
                        StrSQL.Append(" AND (Tipologia_Cod = " & Agro_SQL_SaveNum(Tipologia_Cod) & ") ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM   Utenti_TipologiexPermessi ")
                    StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

                    If Tipologia_Cod <> 0 Then
                        StrSQL.Append(" AND (Tipologia_Cod = " & Agro_SQL_SaveNum(Tipologia_Cod) & ") ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Utenti_TipologiexPermessi.Tipologia_Cod, Utenti_Tipologie.Tipologia_Des, Utenti_TipologiexPermessi.Id_Servizio,  ")
                    StrSQL.Append("        Utenti_TipologiexPermessi.Id_Attivita, TB_Attivita.Attivita_Des, Utenti_TipologiexPermessi.Id_Operazione,    ")
                    StrSQL.Append("        Utenti_TipologiexPermessi.Validita_Inizio, Utenti_TipologiexPermessi.Validita_Fine    ")

                    StrSQL.Append(" FROM   Utenti_TipologiexPermessi INNER JOIN ")
                    StrSQL.Append("        Utenti_Tipologie ON Utenti_TipologiexPermessi.Piva_SuperUser = Utenti_Tipologie.Piva_SuperUser AND ")
                    StrSQL.Append("        Utenti_TipologiexPermessi.Tipologia_Cod = Utenti_Tipologie.Tipologia_Cod INNER JOIN ")
                    StrSQL.Append("        TB_Attivita ON Utenti_TipologiexPermessi.Id_Attivita = TB_Attivita.Id_Attivita AND Utenti_TipologiexPermessi.Id_Servizio = TB_Attivita.Id_Servizio ")

                    StrSQL.Append(" WHERE Utenti_TipologiexPermessi.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")

                    If Tipologia_Cod <> 0 Then
                        StrSQL.Append(" AND (Utenti_TipologiexPermessi.Tipologia_Cod = " & Agro_SQL_SaveNum(Tipologia_Cod) & ") ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Utenti_TipologiexPermessi.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Utenti_TipologiexPermessi.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If
                  

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select



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

    ''' <summary>
    ''' Legge i permessi appartenneti a tutte le tipologie.
    ''' I permessi di lettura e scrittura sono raggruppati in un'unica riga.
    ''' La tipologia del permesso può essere dedotta dalla colonna Tipo,
    ''' dove si considera 1 sta an indicare l'abilitazione in sola lettura del
    ''' permesso, 2 l'abilitazione in sola scrittura e 3 l'abilitazione in
    ''' lettura e scrittura.
    ''' </summary>
    ''' <param name="objParametri">ObjParametri_Utenti</param>
    Public Function LeggiTutteTipologie(xFiltroAggiuntivo As String, xOrderBy As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R.LeggiTutteTipologie()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable
        Try
            StrSQL.AppendLine(" ;WITH txp AS ( ")
            StrSQL.AppendLine("     SELECT ")
            StrSQL.AppendLine("         Utenti_Tipologie.Tipologia_Cod, Utenti_Tipologie.Tipologia_Des, ")
            StrSQL.AppendLine("         Utenti_TipologiexPermessi.Id_Servizio, Utenti_TipologiexPermessi.Id_Attivita, ")
            StrSQL.AppendLine("         Utenti_TipologiexPermessi.Validita_Inizio, Utenti_TipologiexPermessi.Validita_Fine, ")
            StrSQL.AppendLine("         (CASE Utenti_TipologiexPermessi.Id_Operazione WHEN 0 THEN 1 ELSE 2 END) AS pVal ")
            StrSQL.AppendLine("     FROM Utenti_Tipologie ")
            StrSQL.AppendLine("     LEFT JOIN Utenti_TipologiexPermessi ON Utenti_Tipologie.Tipologia_Cod = Utenti_TipologiexPermessi.Tipologia_Cod ")
            StrSQL.AppendLine("     WHERE Utenti_TipologiexPermessi.Id_Attivita > 0 ")
            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine(" SELECT Tipologia_Cod, Tipologia_Des, Id_Servizio, Id_Attivita, Validita_Inizio, Validita_Fine, SUM(pVal) AS Tipo ")
            StrSQL.AppendLine(" FROM txp ")
            If Not String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(" WHERE " & Agro_SQL_SaveText(xFiltroAggiuntivo))
            End If
            StrSQL.AppendLine(" GROUP BY Tipologia_Cod, Tipologia_Des, Id_Servizio, Id_Attivita, Validita_Inizio, Validita_Fine ")
            If Not String.IsNullOrWhiteSpace(xOrderBy) Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_SaveText(xOrderBy))
            End If

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    ''' <summary>
    ''' Carica i permessi relativi alle sezioni del menu.
    ''' </summary>
    Public Function Leggi_Gerarchia_Menu(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R.Leggi_Gerarchia_Menu()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Append("SELECT mn_padre.idsezione id_attivita_padre, mn.idsezione id_attivita_figlio, 5 as livello, ")
            StrSQL.Append("          convert(varchar(10), mn.ordinamento) + '_' + convert(varchar(10), gerarchia_attivita.ordine) as ordine, ")
            StrSQL.Append("       mn_padre.testo attivita_des_padre, mn.testo attivita_des_figlio, ")
            StrSQL.Append("       abs(mn.id_attivita) as id_attivita, a.Attivita_Des ")

            StrSQL.Append("FROM [" & objParametri_Server.Recupera_NomeDB() & "].[dbo].[MenuBS_2017_Sezioni] mn ")
            StrSQL.Append("       JOIN TB_Attivita a ON a.id_attivita = abs(mn.id_attivita) ")
            StrSQL.Append("       JOIN [" & objParametri_Server.Recupera_NomeDB() & "].[dbo].[MenuBS_2017_Sezioni] AS mn_padre ")
            StrSQL.Append("             ON mn_padre.idsezione = mn.idsezionepadre ")
            StrSQL.Append("       JOIN gerarchia_attivita on id_attivita_figlio = a.Id_Attivita ")

            StrSQL.Append("WHERE  mn.idsezionepadre IS NOT NULL ")
            StrSQL.Append("       AND (abs(mn.id_attivita) IN (  select id_attivita from tb_attivita ) )")
            StrSQL.Append("       AND gerarchia_attivita.id_servizio = 5 ")
            'StrSQL.Append("       AND mn_padre.idsezione = 111")

            StrSQL.Append("ORDER BY mn_padre.testo, mn.ordinamento, Gerarchia_Attivita.ordine")

            DT = EseguiQuery_Lettura(objParametri_Utenti, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    ''' <summary>
    ''' Carica i permessi non relativi alle sezioni del menu.
    ''' </summary>
    Public Function Leggi_Gerarchia_Liberi(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R.Leggi_Gerarchia_Liberi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Append("SELECT gerarchia_attivita.id_attivita_padre, padre.Attivita_Des AS Attivita_Des_Padre, tb_attivita.Attivita_Des ")
            StrSQL.Append("FROM tb_attivita ")
            StrSQL.Append("       JOIN gerarchia_attivita ON id_attivita_figlio = tb_attivita.Id_Attivita ")
            StrSQL.Append("       JOIN tb_attivita padre ON gerarchia_attivita.id_attivita_padre = padre.Id_Attivita ")

            StrSQL.Append("WHERE  gerarchia_attivita.id_servizio = 5 ")
            StrSQL.Append("       AND tb_attivita.id_attivita NOT IN (")
            StrSQL.Append("             SELECT id_attivita FROM [" & objParametri_Server.Recupera_NomeDB() & "].[dbo].[MenuBS_2017_Sezioni]")
            StrSQL.Append("             WHERE idsezionepadre IS NOT NULL AND id_attivita IS NOT NULL ) ")
            StrSQL.Append("       AND tb_attivita.id_attivita  NOT IN ( ")
            StrSQL.Append("             SELECT id_attivita_padre FROM gerarchia_attivita ) ")

            StrSQL.Append("ORDER BY padre.Attivita_Des, gerarchia_attivita.ordine")

            DT = EseguiQuery_Lettura(objParametri_Utenti, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    ''' <summary>
    ''' Ultima versione query di caricamento permessi
    ''' </summary>
    Public Function Leggi_Gerarchia_Permessi_Old(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R.Leggi_Gerarchia_Permessi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.AppendLine("WITH #Area as (  ")
            StrSQL.AppendLine("SELECT * FROM MenuBS_2017_Sezioni WHERE IDSezionePadre IS NULL)  ")

            StrSQL.AppendLine(" , #PermessiLettura as (  ")
            StrSQL.AppendLine("SELECT #Area.IDSezione as IDSezione_Area,  ")
            StrSQL.AppendLine("        #Area.Testo as Testo_Area, ")
            StrSQL.AppendLine("        padre.IDSezione as sezionepadre, ")
            StrSQL.AppendLine("        padre.Testo as Padre, ")
            StrSQL.AppendLine("        STRING_AGG(figlio.Testo, ',') as figlio, ")
            StrSQL.AppendLine("        figlio.ID_Attivita as attivita, ")
            StrSQL.AppendLine("     STRING_AGG(figlio.note, ',') as note, ")
            StrSQL.AppendLine("        MIN(figlio.[OrdinamentoComplessivo]) as ordinamento ")
            StrSQL.AppendLine("FROM MenuBS_2017_Sezioni figlio ")
            StrSQL.AppendLine("JOIN MenuBS_2017_Sezioni padre ON figlio.IDSezionePadre = padre.IDSezione ")
            StrSQL.AppendLine("JOIN #Area ON figlio.IDSezioneArea = #Area.IDSezione ")
            StrSQL.AppendLine("WHERE figlio.ID_Attivita > 0 ")
            StrSQL.AppendLine("GROUP BY #Area.IDSezione, #Area.Testo, padre.IDSezione, padre.Testo, figlio.ID_Attivita) ")

            StrSQL.AppendLine(", #PermessiScrittura as ( ")
            StrSQL.AppendLine("SELECT #Area.IDSezione as IDSezione_Area, ")
            StrSQL.AppendLine("        #Area.Testo as Testo_Area, ")
            StrSQL.AppendLine("        padre.IDSezione as sezionepadre, ")
            StrSQL.AppendLine("        padre.Testo as Padre, ")
            StrSQL.AppendLine("        STRING_AGG(figlio.Testo, ',') as figlio, ")
            StrSQL.AppendLine("        abs(figlio.ID_Attivita) as attivita, ")
            StrSQL.AppendLine("        STRING_AGG(figlio.note, ',') as note, ")
            StrSQL.AppendLine("        MIN(figlio.[OrdinamentoComplessivo]) as ordinamento  ")
            StrSQL.AppendLine("FROM MenuBS_2017_Sezioni figlio ")
            StrSQL.AppendLine("JOIN MenuBS_2017_Sezioni padre ON figlio.IDSezionePadre = padre.IDSezione ")
            StrSQL.AppendLine("JOIN #Area ON figlio.IDSezioneArea = #Area.IDSezione ")
            StrSQL.AppendLine("WHERE figlio.ID_Attivita < 0 ")
            StrSQL.AppendLine("GROUP BY #Area.IDSezione, #Area.Testo, padre.IDSezione, padre.Testo, figlio.ID_Attivita) ")

            StrSQL.AppendLine("SELECT ")
            StrSQL.AppendLine("        ISNULL(#PermessiLettura.IDSezione_Area, #PermessiScrittura.IDSezione_Area) as IDSezione_Area, ")
            StrSQL.AppendLine("        ISNULL(#PermessiLettura.Testo_Area, #PermessiScrittura.Testo_Area) as Testo_Area, ")
            StrSQL.AppendLine("        ISNULL(#PermessiLettura.sezionepadre, #PermessiScrittura.sezionepadre) as sezionepadre,  ")
            StrSQL.AppendLine("        ISNULL(#PermessiLettura.Padre, #PermessiScrittura.Padre) as Padre, ")
            StrSQL.AppendLine("        ISNULL(#PermessiLettura.attivita, #PermessiScrittura.attivita) as attivita,")
            StrSQL.AppendLine("        #PermessiLettura.figlio as Funzioni_Lettura, ")
            StrSQL.AppendLine("        #PermessiLettura.note as Note_Lettura, ")
            StrSQL.AppendLine("        #PermessiScrittura.figlio as Funzioni_Scrittura, ")
            StrSQL.AppendLine("        #PermessiScrittura.note as Note_Scrittura, ")
            StrSQL.AppendLine("        ISNULL(#PermessiLettura.ordinamento, #PermessiScrittura.ordinamento) as ordinamento ")
            StrSQL.AppendLine("FROM #PermessiLettura ")
            StrSQL.AppendLine("FULL JOIN #PermessiScrittura ON #PermessiLettura.attivita = #PermessiScrittura.attivita ")
            StrSQL.AppendLine("        AND #PermessiLettura.sezionepadre = #PermessiScrittura.sezionepadre ")
            StrSQL.AppendLine("ORDER BY ordinamento")

            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    ''' <remarks>Alcune attività potrebbero essere ripetute con entry di menù diverse!</remarks>
    Public Function Leggi_Gerarchia_Permessi(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_TipologiexPermessi_R.Leggi_Gerarchia_Permessi()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try
            StrSQL.AppendLine(" WITH ")
            StrSQL.AppendLine(" #Area as (SELECT * FROM MenuBS_2017_Sezioni WHERE IDSezionePadre IS NULL), ")
            StrSQL.AppendLine(" #Permessi as ( ")
            StrSQL.AppendLine("     SELECT #Area.IDSezione as IDSezione_Area, ")
            StrSQL.AppendLine("         #Area.Testo as Testo_Area, ")
            StrSQL.AppendLine("         padre.IDSezione as sezionepadre, ")
            StrSQL.AppendLine("         padre.Testo as Padre, ")
            StrSQL.AppendLine("         STRING_AGG(figlio.Testo, ',') as figlio, ")
            StrSQL.AppendLine("         ABS(figlio.ID_Attivita) as attivita, ")
            StrSQL.AppendLine("         STRING_AGG(CASE WHEN figlio.note <> '' THEN Trim(figlio.note) END, ',') as note, ")
            StrSQL.AppendLine("         MIN(figlio.[OrdinamentoComplessivo]) as ordinamento, ")
            StrSQL.AppendLine("         MIN(figlio.[Validita_Fine]) as Validita_Fine ")
            StrSQL.AppendLine("     FROM MenuBS_2017_Sezioni figlio ")
            StrSQL.AppendLine("     JOIN MenuBS_2017_Sezioni padre ON figlio.IDSezionePadre = padre.IDSezione ")
            StrSQL.AppendLine("     JOIN #Area ON ABS(figlio.IDSezioneArea) = #Area.IDSezione ")
            StrSQL.AppendLine("     WHERE figlio.ID_Attivita IS Not NULL ")
            StrSQL.AppendLine("     GROUP BY #Area.IDSezione, #Area.Testo, padre.IDSezione, padre.Testo, ABS(figlio.ID_Attivita) ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" #NonAssegnatiConPadri as ( ")
            StrSQL.AppendLine("   SELECT uat.id_attivita, uat.Attivita_Des, uatp.Id_Attivita as ID_Attivita_Padre, uatp.Attivita_Des as Attivita_Padre, uat.Validita_Fine ")
            StrSQL.AppendLine("   FROM " & objParametri_Utenti.Recupera_NomeDB & ".dbo.TB_Attivita uat ")
            StrSQL.AppendLine("   JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.Gerarchia_Attivita gat ON uat.Id_Attivita = gat.Id_Attivita_Figlio ")
            StrSQL.AppendLine("   JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.TB_Attivita uatp ON gat.Id_Attivita_Padre = uatp.Id_Attivita ")
            StrSQL.AppendLine("   WHERE uat.Id_Attivita NOT IN ( SELECT DISTINCT attivita FROM #Permessi ) ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine("")

            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine("     SELECT ")
            StrSQL.AppendLine("         COALESCE(#Permessi.IDSezione_Area, '') as IDSezione_Area, ")
            StrSQL.AppendLine("         COALESCE(#Permessi.Testo_Area, '') as Testo_Area, ")
            StrSQL.AppendLine("         COALESCE(#Permessi.sezionepadre, '') as sezionepadre, ")
            StrSQL.AppendLine("         COALESCE(#Permessi.Padre, '') as Padre, ")
            StrSQL.AppendLine("         COALESCE(#Permessi.attivita, '') as attivita, ")
            StrSQL.AppendLine("         COALESCE(#Permessi.figlio, '') as Funzioni, ")
            StrSQL.AppendLine("         COALESCE(#Permessi.note, '') as Note, ")
            StrSQL.AppendLine("         COALESCE(tbALettura.Attivita_Des, '') as NoteAgg, ")
            StrSQL.AppendLine("         COALESCE(#Permessi.ordinamento, '0') as ordinamento ")
            StrSQL.AppendLine("     FROM #Permessi ")
            StrSQL.AppendLine("     LEFT JOIN " & objParametri_Utenti.Recupera_NomeDB & ".dbo.TB_Attivita tbALettura ON #Permessi.attivita = tbALettura.Id_Attivita ")
            StrSQL.AppendLine("     WHERE #Permessi.attivita != 0 AND tbALettura.Validita_Fine > GETDATE()")
            StrSQL.AppendLine("         AND tbALettura.Validita_Fine > GETDATE()")
            StrSQL.AppendLine("         AND #Permessi.Validita_Fine > GETDATE()")
            StrSQL.AppendLine(" ) UNION ( ")
            StrSQL.AppendLine("     SELECT ")
            StrSQL.AppendLine("         0 as IDSezione_Area, ")
            StrSQL.AppendLine("         'No Area' as Testo_Area, ")
            StrSQL.AppendLine("         #NonAssegnatiConPadri.ID_Attivita_Padre as sezionePadre, ")
            StrSQL.AppendLine("         #NonAssegnatiConPadri.Attivita_Padre as Padre, ")
            StrSQL.AppendLine("         #NonAssegnatiConPadri.Id_Attivita as attivita, ")
            StrSQL.AppendLine("         #NonAssegnatiConPadri.Attivita_Des as Funzioni, ")
            StrSQL.AppendLine("         '' as Note, ")
            StrSQL.AppendLine("         #NonAssegnatiConPadri.Attivita_Des as NoteAgg, ")
            StrSQL.AppendLine("         '' as Ordinamento ")
            StrSQL.AppendLine("     FROM #NonAssegnatiConPadri ")
            StrSQL.AppendLine("     WHERE #NonAssegnatiConPadri.Id_Attivita != 0  AND #NonAssegnatiConPadri.Validita_Fine > GETDATE()")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" ORDER BY padre ")


            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return DT
    End Function

End Class

Public Class Utenti_TipologiexPermessi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Tipologia_Cod As Int32, _
                           ByVal Id_Servizio As Int32, _
                            ByVal Id_Attivita As Int32, _
                            ByVal Id_Operazione As Int32, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_TipologiexPermessi_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Utenti_TipologiexPermessi       ")
            StrSQL.Append("            ( Piva_SuperUser, Tipologia_Cod, Id_Servizio, Id_Attivita, Id_Operazione,  ")
            StrSQL.Append("                    Inviato,             DataInvio, ")
            StrSQL.Append("                    Data_Creazione,      Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione,  UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,     Validita_Fine ")
            StrSQL.Append("                    ) ")


            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipologia_Cod) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Operazione) & " ")
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

    Public Function CopiaDaAltraTipologia(original As integer, other as integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_TipologiexPermessi_W.CopiaDaAltraTipologia()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean = False
        Try
            StrSQL.Appendline("INSERT INTO Utenti_TipologiexPermessi (")
            StrSQL.Appendline("  Piva_SuperUser, Tipologia_Cod, Id_Servizio, Id_Attivita, Id_Operazione,  ")
            StrSQL.Appendline("  Inviato,             DataInvio, ")
            StrSQL.Appendline("  Data_Creazione,      Data_Modifica, ")
            StrSQL.Appendline("  UserName_Creazione,  UserName_Modifica, ")
            StrSQL.Appendline("  Validita_Inizio,     Validita_Fine ")
            StrSQL.Appendline(") ")

            StrSQL.AppendLine("SELECT")
            StrSQL.AppendLine("  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("  , " & Agro_SQL_SaveNum(other) & " ")
            StrSQL.AppendLine("  , Id_Servizio ")
            StrSQL.AppendLine("  , Id_Attivita ")
            StrSQL.AppendLine("  , Id_Operazione ")
            StrSQL.AppendLine("  , 0  ")
            StrSQL.AppendLine("  , Null  ")
            StrSQL.AppendLine("  , GETDATE()  ")
            StrSQL.AppendLine("  , GETDATE()  ")
            StrSQL.AppendLine("  ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("  ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("  , Validita_Inizio ")
            StrSQL.AppendLine("  , Validita_Fine ")
            StrSQL.AppendLine("FROM Utenti_TipologiexPermessi")
            StrSQL.AppendLine("WHERE Tipologia_Cod = " & Agro_SQL_SaveNum(original))
            StrSQL.AppendLine("  AND Validita_Fine > GETDATE() ")

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
    Public Function Cancella(ByVal Tipologia_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_TipologiexPermessi_W.Cancella()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean
        Try
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE Utenti_TipologiexPermessi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0")
            Else
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Utenti_TipologiexPermessi ")
                StrSQL.Append(" WHERE    Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If
            If Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND  Tipologia_Cod =  " & Agro_SQL_SaveNum(Tipologia_Cod) & " ")
            End If
            '------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function

    Public Function CancellaMultiplo(
        ByVal Id_Servizio As Integer, ByVal Id_Attivita As IEnumerable(Of Integer),
        ByVal Id_Operazione As Integer, ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_TipologiexPermessi_W.Cancella()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean
        If Id_Servizio = 0 AndAlso Id_Attivita.Count = 0 AndAlso Id_Operazione = 1 AndAlso String.IsNullOrWhiteSpace(xFiltroAggiuntivo) Then
            Throw New UnauthorizedAccessException("Operation aborted: risk of deleting everything on the table.")
        End If
        Dim xIn = Id_Attivita.Select(Function(cod) cod.ToString).
            Aggregate(Function(acc, cod) acc & ", " & cod)
        Try
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Append(" UPDATE Utenti_TipologiexPermessi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0")
            Else
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Utenti_TipologiexPermessi ")
                StrSQL.Append(" WHERE    Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If
            If Id_Servizio <> 0 Then
                StrSQL.Append(" AND  Id_Servizio =  " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            End If
            If Id_Operazione <> 0 Then
                StrSQL.Append(" AND  Id_Operazione =  " & Agro_SQL_SaveNum(Id_Operazione) & " ")
            End If
            If Id_Attivita.Any Then
                StrSQL.Append(" AND  Id_Attivita IN  (" & Agro_SQL_Save_Clausola_IN(xIn) & ") ")
            End If
            '------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function

    '##############################################################################################
    Public Function GeneraImpostazioniUtenteFiltroMonoDaTipologia(
            ByVal Tipologia_Cod As Int32,
            ByVal Username As String,
            ByVal Validita_Inizio As Date,
            ByVal Validita_Fine As Date,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_TipologiexPermessi_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        'pre-requisiti: esiste l'utente, esiste la tipologia, NON ci sono già permessi sull'utente

        Try
            '---------------------------------------------
            stb.Length = 0


            stb.AppendLine("  insert Utenti_Impostazioni_FiltroMono ( ")
            stb.AppendLine("    Piva_SuperUser ")
            stb.AppendLine("  , UserName ")
            stb.AppendLine("  , Impostazione_Cod ")
            stb.AppendLine("  , ID_0 ")
            stb.AppendLine("  , inviato ")
            stb.AppendLine("  , datainvio ")
            stb.AppendLine("  , Data_Creazione ")
            stb.AppendLine("  , Data_Modifica ")
            stb.AppendLine("  , Username_Creazione ")
            stb.AppendLine("  , Username_Modifica ")
            stb.AppendLine("  , Validita_Inizio ")
            stb.AppendLine("  , Validita_Fine ")
            stb.AppendLine("  , Str_0) ")
            stb.AppendLine(" Select ")
            stb.AppendLine("    Piva_SuperUser ")
            stb.AppendLine("  , '" & Agro_SQL_SaveText(Username) & "' as UserName ")
            stb.AppendLine("  , Impostazione_Cod ")
            stb.AppendLine("  , ID_0 ")
            stb.AppendLine("  , inviato ")
            stb.AppendLine("  , datainvio ")
            stb.AppendLine("  , Data_Creazione ")
            stb.AppendLine("  , Data_Modifica ")
            stb.AppendLine("  , Username_Creazione ")
            stb.AppendLine("  , Username_Modifica ")
            stb.AppendLine("  , Validita_Inizio ")
            stb.AppendLine("  , Validita_Fine ")
            stb.AppendLine("  , Str_0 ")
            stb.AppendLine(" From Utenti_Impostazioni_FiltroMono i ")
            stb.AppendLine(" Where Username = '" & Agro_SQL_SaveText(Tipologia_Cod) & "' ")
            stb.AppendLine(" and Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            stb.AppendLine(" And Not exists( ")
            stb.AppendLine("  select 1  ")
            stb.AppendLine("     From Utenti_Impostazioni_FiltroMono i2 ")
            stb.AppendLine("  where i2.Impostazione_Cod = i.Impostazione_Cod ")
            stb.AppendLine("  and i2.Username = '" & Agro_SQL_SaveText(Username) & "' ")
            stb.AppendLine("  and i2.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            stb.AppendLine("  And i2.ID_0 = i.ID_0 ")
            stb.AppendLine(" )")


            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
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
    Public Function GeneraImpostazioniUtenteDaTipologia(
            ByVal Tipologia_Cod As Int32,
            ByVal Username As String,
            ByVal Validita_Inizio As Date,
            ByVal Validita_Fine As Date,
            ByVal xFiltroAggiuntivo As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_TipologiexPermessi_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        'pre-requisiti: esiste l'utente, esiste la tipologia, NON ci sono già permessi sull'utente

        Try
            '---------------------------------------------
            stb.Length = 0
            stb.AppendLine("  insert Utenti_Impostazioni( ")
            stb.AppendLine("    Username ")
            stb.AppendLine("  , Impostazione_Cod ")
            stb.AppendLine("  , Impostazione_Valore_1 ")
            stb.AppendLine("  , Impostazione_Valore_2 ")
            stb.AppendLine("  , Impostazione_Valore_3 ")
            stb.AppendLine("  , Impostazione_Valore_4 ")
            stb.AppendLine("  , inviato ")
            stb.AppendLine("  , datainvio ")
            stb.AppendLine("  , Data_Creazione ")
            stb.AppendLine("  , Data_Modifica ")
            stb.AppendLine("  , Username_Creazione ")
            stb.AppendLine("  , Username_Modifica ")
            stb.AppendLine("  , Validita_Inizio ")
            stb.AppendLine("  , Validita_Fine ")
            stb.AppendLine("  , Piva_SuperUser ")
            stb.AppendLine(" ) ")
            stb.AppendLine(" Select  ")
            stb.AppendLine("    '" & Agro_SQL_SaveText(Username) & "' as UserName ")
            stb.AppendLine("  , Impostazione_Cod ")
            stb.AppendLine("  , Impostazione_Valore_1 ")
            stb.AppendLine("  , Impostazione_Valore_2 ")
            stb.AppendLine("  , Impostazione_Valore_3 ")
            stb.AppendLine("  , Impostazione_Valore_4 ")
            stb.AppendLine("  , inviato ")
            stb.AppendLine("  , datainvio ")
            stb.AppendLine("  , Data_Creazione ")
            stb.AppendLine("  , Data_Modifica ")
            stb.AppendLine("  , Username_Creazione ")
            stb.AppendLine("  , Username_Modifica ")
            stb.AppendLine("  , Validita_Inizio ")
            stb.AppendLine("  , Validita_Fine ")
            stb.AppendLine("  , Piva_SuperUser ")
            stb.AppendLine(" From Utenti_Impostazioni i ")
            stb.AppendLine(" Where Username = '" & Agro_SQL_SaveText(Tipologia_Cod) & "' ")
            stb.AppendLine(" and Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            stb.AppendLine(" And Not exists( ")
            stb.AppendLine("  select 1  ")
            stb.AppendLine("     From Utenti_Impostazioni i2 ")
            stb.AppendLine("  where i2.Impostazione_Cod = i.Impostazione_Cod ")
            stb.AppendLine("  and i2.Username = '" & Agro_SQL_SaveText(Username) & "' ")
            stb.AppendLine("  and i2.Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")
            stb.AppendLine(" )")

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
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
    Public Function GeneraPermessiUtenteDaTipologia(
            ByVal Tipologia_Cod As Int32,
            ByVal Username As String,
            ByVal Validita_Inizio As Date,
            ByVal Validita_Fine As Date,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_TipologiexPermessi_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        'pre-requisiti: esiste l'utente, esiste la tipologia, NON ci sono già permessi sull'utente

        Try
            '---------------------------------------------
            stb.Length = 0

            stb.AppendLine("insert Utenti_Permessi( ")
            stb.AppendLine("     UserName,  ")
            stb.AppendLine("     Id_Servizio,  ")
            stb.AppendLine("     Id_Attivita,  ")
            stb.AppendLine("     Id_Operazione,  ")
            stb.AppendLine("     ID,  ")
            stb.AppendLine("     Inizio_Ore,  ")
            stb.AppendLine("     Inizio_Minuti,  ")
            stb.AppendLine("     Fine_Ore,  ")
            stb.AppendLine("     Fine_Minuti,  ")
            stb.AppendLine("     inviato,  ")
            stb.AppendLine("     datainvio,  ")
            stb.AppendLine("     Data_Creazione,  ")
            stb.AppendLine("     Data_Modifica,  ")
            stb.AppendLine("     Username_Creazione,  ")
            stb.AppendLine("     Username_Modifica,  ")
            stb.AppendLine("     Validita_Inizio,  ")
            stb.AppendLine("     Validita_Fine,  ")
            stb.AppendLine("    Tipologia_Cod ")
            stb.AppendLine(" ) ")

            stb.AppendLine(" select  ")
            stb.AppendLine("     '" & Agro_SQL_SaveText(Username) & "' as UserName,  ")
            stb.AppendLine("     Id_Servizio,  ")
            stb.AppendLine("     Id_Attivita,  ")
            stb.AppendLine("     Id_Operazione,  ")
            stb.AppendLine("     1 as ID,  ")
            stb.AppendLine("     0 as Inizio_Ore,  ")
            stb.AppendLine("     0 as Inizio_Minuti,  ")
            stb.AppendLine("     23 as Fine_Ore,  ")
            stb.AppendLine("     59 as Fine_Minuti,  ")
            stb.AppendLine("     0 as inviato,  ")
            stb.AppendLine("     null as datainvio,  ")
            stb.AppendLine("     Data_Creazione,  ")
            stb.AppendLine("     Data_Modifica,  ")
            stb.AppendLine("     Username_Creazione,  ")
            stb.AppendLine("     Username_Modifica,  ")
            stb.AppendLine("     " & Agro_SQL_SaveDateTime(Validita_Inizio) & " as Validita_Inizio,  ")
            stb.AppendLine("     " & Agro_SQL_SaveDateTime(Validita_Fine) & " as Validita_Fine,  ")
            stb.AppendLine("     " & Tipologia_Cod & " as Tipologia_Cod ")
            stb.AppendLine("      ")
            stb.AppendLine(" from Utenti_TipologiexPermessi t1 ")
            stb.AppendLine(" where Tipologia_Cod = " & Tipologia_Cod)
            stb.AppendLine(" and not exists ( ")
            stb.AppendLine("     select 1  ")
            stb.AppendLine("     from Utenti_Permessi tEx ")
            stb.AppendLine("     where  ")
            stb.AppendLine("         tEx.UserName = '" & Agro_SQL_SaveText(Username) & "' ")
            stb.AppendLine("         and tEx.Id_Servizio = t1.id_servizio ")
            stb.AppendLine("         and tEx.id_attivita = t1.id_attivita ")
            stb.AppendLine("         and tEx.id_operazione = t1.id_operazione ")
            stb.AppendLine("          ")
            stb.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function AggiornaValiditaPermessiUtente(
            ByVal id_Servizio As Int32,
            ByVal Username As String,
            ByVal Validita_Fine As Date,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_TipologiexPermessi_W.Scrivi()"

        Dim stb As New Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        'pre-requisiti: esiste l'utente, esiste la tipologia, NON ci sono già permessi sull'utente

        Try
            stb.AppendLine(" update Utenti_Permessi  ")
            stb.AppendLine("     set Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & ",  ")
            stb.AppendLine("         Data_Modifica = " & Agro_SQL_SaveDate(Now) & ",  ")
            stb.AppendLine("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            stb.AppendLine("     where Id_Servizio =   " & id_Servizio)
            stb.AppendLine("     and Username = '" & Agro_SQL_SaveText(Username) & "'  ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function

End Class