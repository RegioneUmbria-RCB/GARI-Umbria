Imports System.Data.Common
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class System_Database_R
    Inherits AgronicaCoreDataProvider.DataProvider




    'Funzione per reperire i nomi di tutte le tabelle del DB
    '====================================================================================
    'Public Function Leggi_ElencoNomiTabelle(
    '                                ByRef objConnessione As DbConnection,
    '                                ByVal StringaConnessione As String,
    '                                ByVal DirectoryLOG As String,
    '                                ByVal FileLOG As String,
    '                                ByVal IdentificatoreUtente As String) _
    '                                    As DataTable

    '    Const nomeRoutine = "AgronicaCoreVarieDAL.System_Database_R.Leggi_ElencoNomiTabelle"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim dt As DataTable

    '    Try

    '        '------------------------------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append("  sp_tables @table_type = " & Chr(34) & " 'TABLE'" & Chr(34))
    '        '------------------------------------------------------------------

    '        dt = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

    '    Catch ex As Exception

    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, messaggioErrore)
    '        dt = Nothing
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

    '    End Try

    '    Return dt

    'End Function

    '#############################################################################
    'Rispetto all'altra Usa l'objparametri
    'Funzione per reperire i nomi di tutte le tabelle del DB
    '====================================================================================
    Public Function Leggi_ElencoNomiTabelle_2(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              Optional ByVal xFiltroAggiuntivo As String = "") _
                                                As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.System_Database_R.Leggi_ElencoNomiTabelle_2"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("  sp_tables @table_type = " & Chr(34) & " 'TABLE'" & Chr(34))
            '------------------------------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" ,  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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





    'Funzione per reperire tutte le colonne di una tabella
    '====================================================================================
    'Public Function Leggi_ElencoNomiColonneTabella(
    '                            ByVal NomeTabella As String,
    '                                ByRef objConnessione As DbConnection,
    '                                ByVal StringaConnessione As String,
    '                                ByVal DirectoryLOG As String,
    '                                ByVal FileLOG As String,
    '                                ByVal IdentificatoreUtente As String) _
    '                                    As DataTable

    '    Const nomeRoutine = "AgronicaCoreVarieDAL.System_Database_R.Leggi_ElencoNomiColonneTabella"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim dt As DataTable

    '    Try

    '        If NomeTabella = "" Then
    '            Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
    '        End If

    '        '------------------------------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append("   sp_columns @table_name = '" & NomeTabella & "'")
    '        '------------------------------------------------------------------

    '        dt = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

    '    Catch ex As Exception

    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, messaggioErrore)
    '        dt = Nothing
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

    '    End Try

    '    Return dt

    'End Function






    'Funzione per reperire tutte le colonne di una tabella
    '====================================================================================
    'Public Function Leggi_ElencoNomiColonneTabella_2(
    '                            ByVal NomeTabella As String,
    '                                ByRef objConnessione As DbConnection,
    '                                ByVal StringaConnessione As String,
    '                                ByVal DirectoryLOG As String,
    '                                ByVal FileLOG As String,
    '                                ByVal IdentificatoreUtente As String) _
    '                                    As DataTable

    '    Const nomeRoutine = "AgronicaCoreVarieDAL.System_Database_R.Leggi_ElencoNomiColonneTabella_2"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim dt As DataTable

    '    Try

    '        If NomeTabella = "" Then
    '            Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
    '        End If

    '        '------------------------------------------------------------------
    '        StrSQL.Length = 0

    '        'strCampi = " sp_columns @table_name = '" & TableName & "'"

    '        ''c'è un BUG nella system stored procedure "sp_columns" ?!?
    '        ''se si esegue: sp_columns @table_name = 'Linee_Preparazioni_Report'
    '        ''la stored ritorna tutti i campi della tabella Linee_Preparazioni_Report
    '        ''ma anche tutti i campi della tabella Linee_PreparazionixReport !!!

    '        'TableName = TableName.Replace("_", "[_]")
    '        'strCampi = " sp_columns @table_name = '" & TableName & "'"

    '        ''dal sito: http://www.forumtopics.com/busobj/viewtopic.php?p=198230
    '        ''si pensava di ovviare al problema sostituendo gli "_" con "[_]"
    '        ''eseguendo quindi: sp_columns @table_name = 'Linee[_]Preparazioni[_]Report'
    '        ''che restituisce il risultato corretto, peccato però che
    '        ''sp_columns @table_name = 'ACC[_]Configurazione' dia errore!!!

    '        'Vanni ha risolto così:

    '        StrSQL.Append(" ")
    '        StrSQL.Append(" SELECT * ")
    '        StrSQL.Append(" FROM syscolumns ")
    '        StrSQL.Append(" WHERE id IN ( ")
    '        StrSQL.Append("               SELECT id ")
    '        StrSQL.Append("               FROM sysobjects ")
    '        StrSQL.Append("               WHERE name = '" & NomeTabella & "' ")
    '        StrSQL.Append("             )")

    '        '------------------------------------------------------------------

    '        dt = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

    '    Catch ex As Exception

    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, messaggioErrore)
    '        dt = Nothing
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)

    '    End Try

    '    Return dt

    'End Function


    '#############################################################################
    'Rispetto alla Leggi_ElencoNomiColonneTabella_2 Usa l'objparametri
    'Funzione per reperire tutte le colonne di una tabella
    '====================================================================================
    Public Function Leggi_ElencoNomiColonneTabella_3( _
                                ByVal NomeTabella As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                        As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.System_Database_R.Leggi_ElencoNomiColonneTabella_3"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            If NomeTabella = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
            End If

            '------------------------------------------------------------------
            StrSQL.Length = 0

            'strCampi = " sp_columns @table_name = '" & TableName & "'"

            ''c'è un BUG nella system stored procedure "sp_columns" ?!?
            ''se si esegue: sp_columns @table_name = 'Linee_Preparazioni_Report'
            ''la stored ritorna tutti i campi della tabella Linee_Preparazioni_Report
            ''ma anche tutti i campi della tabella Linee_PreparazionixReport !!!

            'TableName = TableName.Replace("_", "[_]")
            'strCampi = " sp_columns @table_name = '" & TableName & "'"

            ''dal sito: http://www.forumtopics.com/busobj/viewtopic.php?p=198230
            ''si pensava di ovviare al problema sostituendo gli "_" con "[_]"
            ''eseguendo quindi: sp_columns @table_name = 'Linee[_]Preparazioni[_]Report'
            ''che restituisce il risultato corretto, peccato però che
            ''sp_columns @table_name = 'ACC[_]Configurazione' dia errore!!!

            'Vanni ha risolto così:

            StrSQL.Append(" ")
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM syscolumns ")
            StrSQL.Append(" WHERE id IN ( ")
            StrSQL.Append("               SELECT id ")
            StrSQL.Append("               FROM sysobjects ")
            StrSQL.Append("               WHERE name = '" & Agro_SQL_SaveText(NomeTabella) & "' ")
            StrSQL.Append("             )")


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


    '#############################################################################

    Public Function TrovaValoreStringa_DaTabellaGenerica(ByVal NomeTabella As String,
                                                            ByVal NomeColonna As String,
                                                            ByVal ValoreStringa As String,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                                            As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.System_Database_R.Leggi_DaTabellaGenerica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            If NomeTabella = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeTabella obbligatorio)")
            End If

            If NomeColonna = "" Then
                Throw New Exception("Parametro non corretto nella query (NomeColonna obbligatorio)")
            End If

            If ValoreStringa = "" Then
                Throw New Exception("Parametro non corretto nella query (ValoreStringa obbligatorio)")
            End If

            '------------------------------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT 1")
            StrSQL.AppendLine(" FROM " & NomeTabella)
            StrSQL.AppendLine(" WHERE " & NomeColonna & " = '" & Agro_SQL_SaveText(ValoreStringa) & "'")

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

    '#############################################################################

    Public Function TrovaValoreStringa_FiltroNomeColonna(ByVal NomeColonna As String,
                                                          ByVal ValoreStringa As String,
                                                          ByRef NomeTabellaPresenteValoreStringa As String,
                                                          ByRef List_objParametri As List(Of AgronicaCoreDataProvider.AgronicaCoreParametri)) _
                                                          As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.System_Database_R.Leggi_ElencoTabelle_FiltroNomeColonne()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim objParametri_Exception As New AgronicaCoreDataProvider.AgronicaCoreParametri

        Dim DT_TabellaGenerica As DataTable

        Dim Valore_Trovato As Boolean = False

        Try

            For Each objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri In List_objParametri

                objParametri_Exception = objParametri

                If NomeColonna = "" Then
                    Throw New Exception("Parametro non corretto nella query (NomeColonna obbligatorio)")
                End If

                If ValoreStringa = "" Then
                    Throw New Exception("Parametro non corretto nella query (ValoreStringa obbligatorio)")
                End If

                '------------------------------------------------------------------
                StrSQL.Length = 0
                StrSQL.AppendLine(" SELECT tt.name AS table_name , cc.name AS column_name ")
                StrSQL.AppendLine(" FROM sys.tables tt  ")
                StrSQL.AppendLine(" inner join sys.columns cc  ")
                StrSQL.AppendLine(" on tt.object_id = cc.object_id  ")
                StrSQL.AppendLine(" where cc.name like ('%" & Agro_SQL_SaveText(NomeColonna) & "%')  ")
                StrSQL.AppendLine("  and system_type_id in (167, 175, 231, 239)  ")

                '--------------------------------------------------------------------------
                dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
                '--------------------------------------------------------------------------


                If Not IsNothing(dt) Then

                    For Each row In dt.Rows

                        DT_TabellaGenerica = TrovaValoreStringa_DaTabellaGenerica(row("table_name").ToString(),
                                                                                  row("column_name").ToString(),
                                                                                  ValoreStringa,
                                                                                  objParametri)

                        If Not IsNothing(DT_TabellaGenerica) AndAlso DT_TabellaGenerica.Rows.Count > 0 Then
                            Valore_Trovato = True
                            NomeTabellaPresenteValoreStringa = row("table_name").ToString()
                            Exit For
                        End If

                    Next

                End If

                If Valore_Trovato Then
                    Exit For
                End If

            Next



        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Exception, nomeRoutine, messaggioErrore)
            Valore_Trovato = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Valore_Trovato

    End Function

    'Legge tutte le tabelle e le colonne di un db e ti dice se ha la colonna Data_Modifica
    Public Function Leggi_ElencoNomiColonneTabelle_Data_Modifica(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreVarieDAL.System_Database_R.Leggi_ElencoNomiColonneTabelle_Data_Modifica()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("select t1.name AS Nome_Tabella , c1.name AS Nome_Colonna ,")
            StrSQL.AppendLine("CASE")
            StrSQL.AppendLine(" WHEN (SELECT c2.name from sys.tables t2")
            StrSQL.AppendLine("      inner Join sys.columns c2 on t2.object_id = c2.object_id")
            StrSQL.AppendLine("      where t1.name = t2.name")
            StrSQL.AppendLine("      And c2.name = 'Data_Modifica') IS NOT NULL ")
            StrSQL.AppendLine(" THEN ")
            StrSQL.AppendLine(" 'True'")
            StrSQL.AppendLine(" ELSE")
            StrSQL.AppendLine(" 'False'")
            StrSQL.AppendLine("END As Flag_DataModifica")
            StrSQL.AppendLine("FROM sys.tables t1")
            StrSQL.AppendLine("inner Join sys.columns c1 on t1.object_id = c1.object_id")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------


        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] :  " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class

Public Class System_Database_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Modifica_Valore_Campo_Tabella( _
                              ByVal Nome_Tabella As String, _
                              ByVal Nome_Campo As String, _
                              ByVal Valore_OLD_Stringa As String, _
                              ByVal Valore_NEW_Stringa As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreVarieDAL.System_Database_W.Modifica_Valore_Campo_Tabella"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE " & Nome_Tabella & " SET ")
            StrSQL.Append("        " & Nome_Campo & "             =  '" & Agro_SQL_SaveText(Valore_NEW_Stringa) & "'  ")
            'StrSQL.Append("       ,Data_Modifica      =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            StrSQL.Append(" WHERE    " & Nome_Campo & "  = '" & Agro_SQL_SaveText(Valore_OLD_Stringa) & "' ")

            '---------------------------------------------
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


