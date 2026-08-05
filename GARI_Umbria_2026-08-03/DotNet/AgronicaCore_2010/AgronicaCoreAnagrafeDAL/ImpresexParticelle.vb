Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


'===================================================================
'===================================================================
'==========        USARE ImpresexParticelle2    ====================
'==========        USARE ImpresexParticelle2    ====================
'==========        USARE ImpresexParticelle2    ====================
'==========        USARE ImpresexParticelle2    ====================
'==========        USARE ImpresexParticelle2    ====================
'==========        USARE ImpresexParticelle2    ====================
'==========        USARE ImpresexParticelle2    ====================
'===================================================================
'===================================================================

<Obsolete("Usare ImpresexParticelle2_R")>
Public Class ImpresexParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    '<Obsolete("Usare ImpresexParticelle2_R.Leggi (primo parametro ID=0)")>
    'Public Function Leggi(ByVal PIVA As String,
    '                      ByVal Sa_Cod As Integer,
    '                      ByVal Part_Cod As Integer,
    '                      ByVal PROV As String,
    '                      ByVal COM As String,
    '                      ByVal SEZIONE As String,
    '                      ByVal FOGLIO As Integer,
    '                      ByVal NUMERO As Integer,
    '                      ByVal SUBALTERNO As String,
    '                      ByVal xSelezioneVariabile As enumSelezioneVariabile,
    '                      ByVal xFiltroAggiuntivo As String,
    '                      ByVal xOrderBy As String,
    '                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
    '                      Optional ByVal TipoG2G As Integer = 0
    '                      ) As DataTable


    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Piva = ""                   =>  
    '    '   Sa_Cod = 0                  =>
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------
    '        Select Case xSelezioneVariabile

    '            Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


    '            Case enumSelezioneVariabile.Selezione_TabellaCompleta





    '            Case enumSelezioneVariabile.Selezione_JoinDescrizioni
    '                StrSQL.Length = 0
    '                StrSQL.Append(" SELECT  ParticelleCatastali.*, ")
    '                StrSQL.Append(" ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
    '                StrSQL.Append(" ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
    '                StrSQL.Append(" ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ImpresexParticelle.Sup_Condotta, ")
    '                StrSQL.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
    '                StrSQL.Append(" ImpresexParticelle.Piva, Imprese.rag_soc, ImpresexParticelle.Sa_Cod, Centri_Aziendali.sa_nome ")

    '                StrSQL.Append(" FROM  ImpreseXParticelle ")
    '                StrSQL.Append(" INNER JOIN ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND ")
    '                StrSQL.Append("               ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ")
    '                StrSQL.Append("               ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
    '                StrSQL.Append(" INNER JOIN ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM ")
    '                StrSQL.Append(" INNER JOIN Lista_Province ON ISTAT.PROV = Lista_Province.PROV ")
    '                StrSQL.Append(" INNER JOIN Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ")
    '                StrSQL.Append(" INNER JOIN Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA ")

    '                StrSQL.Append(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
    '                StrSQL.Append(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
    '                StrSQL.Append(" AND     (ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
    '                StrSQL.Append(" AND     (ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")



    '                If (PIVA <> "") Then
    '                    StrSQL.Append(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
    '                End If

    '                If Sa_Cod <> 0 Then
    '                    StrSQL.Append(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '                End If

    '                If Part_Cod <> 0 Then
    '                    StrSQL.Append(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
    '                End If

    '                If PROV <> "" Then
    '                    StrSQL.Append(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '                End If

    '                If COM <> "" Then
    '                    StrSQL.Append(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '                End If

    '                If SEZIONE <> "" Then
    '                    StrSQL.Append(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
    '                End If

    '                If FOGLIO <> 0 Then
    '                    StrSQL.Append(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
    '                End If

    '                If NUMERO <> 0 Then
    '                    StrSQL.Append(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
    '                End If

    '                If SUBALTERNO <> "" Then
    '                    StrSQL.Append(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
    '                End If


    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                Select Case objParametri.FlagVisibilita
    '                    Case enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND     (ImpresexParticelle.inviato >= 0)  ")
    '                    Case enumVisibilita.Visibilita_SoloCancellati
    '                        StrSQL.Append(" AND     (ImpresexParticelle.inviato = -1)  ")
    '                    Case enumVisibilita.Visibilita_Tutti
    '                        '...................................
    '                    Case Else
    '                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '                End Select
    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                End If





    '            Case enumSelezioneVariabile.Selezione_JoinCompleta
    '                '
    '                '
    '                '
    '                '


    '        End Select



    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try


    '    Return DT

    'End Function


    ''##############################################################################################
    '<Obsolete("Usare ImpresexParticelle2_R.Leggi_conMacrousi (eliminare il parametro Part_Cod)")>
    'Public Function Leggi_conMacrousi(ByVal PIVA As String, _
    '                            ByVal Sa_Cod As Int32, _
    '                            ByVal Part_Cod As Int32, _
    '                            ByVal PROV As String, _
    '                            ByVal COM As String, _
    '                            ByVal SEZIONE As String, _
    '                            ByVal FOGLIO As Int32, _
    '                            ByVal NUMERO As Int32, _
    '                            ByVal SUBALTERNO As String, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByVal xOrderBy As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As DataTable


    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.Leggi_conMacrousi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Piva = ""                   =>  
    '    '   Sa_Cod = 0                  =>
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT  ParticelleCatastali.*, ")
    '        StrSQL.Append(" ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
    '        StrSQL.Append(" ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
    '        StrSQL.Append(" ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ImpresexParticelle.Sup_Condotta, ")
    '        StrSQL.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
    '        StrSQL.Append(" ImpresexParticelle.Piva, Imprese.rag_soc, ImpresexParticelle.Sa_Cod, Centri_Aziendali.sa_nome, ")

    '        StrSQL.Append(" ISNULL(ParticelleCatastalixMacrousi.Macrouso_Cod,'') as Macrouso_Cod, ParticelleCatastalixMacrousi.Superficie AS Sup_Macrouso, ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des ")

    '        StrSQL.Append(" FROM   Macrousi INNER JOIN ")
    '        StrSQL.Append("        ParticelleCatastalixMacrousi ON Macrousi.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod RIGHT OUTER JOIN ")
    '        StrSQL.Append("        ImpreseXParticelle INNER JOIN ")
    '        StrSQL.Append("        ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND  ")
    '        StrSQL.Append("        ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
    '        StrSQL.Append("        ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
    '        StrSQL.Append("        ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM INNER JOIN ")
    '        StrSQL.Append("        Lista_Province ON ISTAT.PROV = Lista_Province.PROV INNER JOIN ")
    '        StrSQL.Append("        Imprese ON ImpreseXParticelle.PIVA = Imprese.PIVA INNER JOIN ")
    '        StrSQL.Append("        Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ON  ")
    '        StrSQL.Append("        ParticelleCatastalixMacrousi.PROV = ParticelleCatastali.PROV AND ParticelleCatastalixMacrousi.COM = ParticelleCatastali.COM AND  ")
    '        StrSQL.Append("        ParticelleCatastalixMacrousi.SEZIONE = ParticelleCatastali.SEZIONE AND ParticelleCatastalixMacrousi.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
    '        StrSQL.Append("        ParticelleCatastalixMacrousi.NUMERO = ParticelleCatastali.NUMERO AND  ")
    '        StrSQL.Append("        ParticelleCatastalixMacrousi.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")


    '        StrSQL.Append(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
    '        StrSQL.Append(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
    '        StrSQL.Append(" AND     (ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
    '        StrSQL.Append(" AND     (ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

    '        If (PIVA <> "") Then
    '            StrSQL.Append(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
    '            StrSQL.Append(" AND ParticelleCatastalixMacrousi.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        End If

    '        If Part_Cod <> 0 Then
    '            StrSQL.Append(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
    '        End If

    '        If PROV <> "" Then
    '            StrSQL.Append(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '        End If

    '        If COM <> "" Then
    '            StrSQL.Append(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '        End If

    '        If SEZIONE <> "" Then
    '            StrSQL.Append(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
    '        End If

    '        If FOGLIO <> 0 Then
    '            StrSQL.Append(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
    '        End If

    '        If NUMERO <> 0 Then
    '            StrSQL.Append(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
    '        End If

    '        If SUBALTERNO <> "" Then
    '            StrSQL.Append(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
    '        End If



    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If
    '        '--------------------------------------------------------------------------
    '        Select Case objParametri.FlagVisibilita
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                StrSQL.Append(" AND     (ImpresexParticelle.inviato >= 0)  ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
    '                StrSQL.Append(" AND     (ImpresexParticelle.inviato = -1)  ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                '...................................
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select
    '        '--------------------------------------------------------------------------
    '        If xOrderBy <> "" Then
    '            strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        End If



    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try


    '    Return DT

    'End Function

    ''##############################################################################################
    '<Obsolete("Usare ImpresexParticelle2_R.Leggi_conMacrousiUtilizzi (eliminare il parametro Part_Cod)")>
    'Public Function Leggi_conMacrousiUtilizzi( _
    '                            ByVal PIVA As String, _
    '                            ByVal Sa_Cod As Int32, _
    '                            ByVal Part_Cod As Int32, _
    '                            ByVal PROV As String, _
    '                            ByVal COM As String, _
    '                            ByVal SEZIONE As String, _
    '                            ByVal FOGLIO As Int32, _
    '                            ByVal NUMERO As Int32, _
    '                            ByVal SUBALTERNO As String, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByVal xOrderBy As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As DataTable


    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.Leggi_conMacrousiUtilizzi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Piva = ""                   =>  
    '    '   Sa_Cod = 0                  =>
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT  ParticelleCatastali.*, ")
    '        StrSQL.Append(" ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
    '        StrSQL.Append(" ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
    '        StrSQL.Append(" ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ImpresexParticelle.Sup_Condotta, ")
    '        StrSQL.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
    '        StrSQL.Append(" ImpresexParticelle.Piva, Imprese.rag_soc, ImpresexParticelle.Sa_Cod, Centri_Aziendali.sa_nome, ")

    '        StrSQL.Append(" ISNULL(ParticelleCatastalixMacrousi.Macrouso_Cod,'') as Macrouso_Cod, ParticelleCatastalixMacrousi.Superficie AS Sup_Macrouso, ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des, ")

    '        StrSQL.Append(" ISNULL(ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea,'') as Veg_Cod_Agea, ISNULL(Codifica_SpecieVegetali_AGEA.Veg_Des_Agea,'') as Veg_Des_Agea, ")
    '        StrSQL.Append(" ISNULL(ParticelleCatastalixMacrousixUtilizzo.Cul_Cod_Agea,'') as Cul_Cod_Agea, ISNULL(Codifica_SpecieVegetali_AGEA.Cul_Des_Agea,'') as Cul_Des_Agea, ")
    '        StrSQL.Append(" ISNULL(ParticelleCatastalixMacrousixUtilizzo.Superficie,0) AS Sup_Utilizzo ")

    '        'StrSQL.Append(" FROM   Codici_Anagrafe RIGHT OUTER JOIN ")
    '        'StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo ON Codici_Anagrafe.codice = ParticelleCatastalixMacrousixUtilizzo.Id_Cod LEFT OUTER JOIN ")
    '        'StrSQL.Append(" GruppoVarietale ON ParticelleCatastalixMacrousixUtilizzo.Grva_Cod = GruppoVarietale.Grva_Cod LEFT OUTER JOIN ")
    '        'StrSQL.Append(" GruppoFinalita ON ParticelleCatastalixMacrousixUtilizzo.Grfi_Cod = GruppoFinalita.Grfi_Cod LEFT OUTER JOIN ")
    '        'StrSQL.Append(" Cultivar ON ParticelleCatastalixMacrousixUtilizzo.Cul_Cod = Cultivar.Cul_Cod LEFT OUTER JOIN ")
    '        'StrSQL.Append(" SpecieVegetali ON ParticelleCatastalixMacrousixUtilizzo.Veg_Cod = SpecieVegetali.Veg_Cod RIGHT OUTER JOIN ")

    '        StrSQL.Append(" FROM   Codifica_SpecieVegetali_AGEA RIGHT OUTER JOIN ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo ON Codifica_SpecieVegetali_AGEA.Veg_Cod_Agea = ParticelleCatastalixMacrousixUtilizzo.Veg_Cod_Agea AND ")
    '        StrSQL.Append(" Codifica_SpecieVegetali_AGEA.Cul_Cod_Agea = ParticelleCatastalixMacrousixUtilizzo.Cul_Cod_Agea RIGHT OUTER JOIN ")
    '        StrSQL.Append(" Macrousi INNER JOIN ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousi ON Macrousi.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod ON  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo.PROV = ParticelleCatastalixMacrousi.PROV AND  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo.COM = ParticelleCatastalixMacrousi.COM AND  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo.SEZIONE = ParticelleCatastalixMacrousi.SEZIONE AND  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo.FOGLIO = ParticelleCatastalixMacrousi.FOGLIO AND  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo.NUMERO = ParticelleCatastalixMacrousi.NUMERO AND  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo.SUBALTERNO = ParticelleCatastalixMacrousi.SUBALTERNO AND  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousixUtilizzo.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod RIGHT OUTER JOIN ")
    '        StrSQL.Append(" ImpreseXParticelle INNER JOIN ")
    '        StrSQL.Append(" ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND  ")
    '        StrSQL.Append(" ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
    '        StrSQL.Append(" ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
    '        StrSQL.Append(" ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM INNER JOIN ")
    '        StrSQL.Append(" Lista_Province ON ISTAT.PROV = Lista_Province.PROV INNER JOIN ")
    '        StrSQL.Append(" Imprese ON ImpreseXParticelle.PIVA = Imprese.PIVA INNER JOIN ")
    '        StrSQL.Append(" Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ON  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousi.PROV = ParticelleCatastali.PROV AND ParticelleCatastalixMacrousi.COM = ParticelleCatastali.COM AND  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousi.SEZIONE = ParticelleCatastali.SEZIONE AND ParticelleCatastalixMacrousi.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
    '        StrSQL.Append(" ParticelleCatastalixMacrousi.NUMERO = ParticelleCatastali.NUMERO And ParticelleCatastalixMacrousi.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")


    '        StrSQL.Append(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
    '        StrSQL.Append(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
    '        StrSQL.Append(" AND     (ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
    '        StrSQL.Append(" AND     (ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

    '        If (PIVA <> "") Then
    '            StrSQL.Append(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.Append(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '        End If

    '        If Part_Cod <> 0 Then
    '            StrSQL.Append(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
    '        End If

    '        If PROV <> "" Then
    '            StrSQL.Append(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '        End If

    '        If COM <> "" Then
    '            StrSQL.Append(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '        End If

    '        If SEZIONE <> "" Then
    '            StrSQL.Append(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
    '        End If

    '        If FOGLIO <> 0 Then
    '            StrSQL.Append(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
    '        End If

    '        If NUMERO <> 0 Then
    '            StrSQL.Append(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
    '        End If

    '        If SUBALTERNO <> "" Then
    '            StrSQL.Append(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
    '        End If



    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If
    '        '--------------------------------------------------------------------------
    '        Select Case objParametri.FlagVisibilita
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                StrSQL.Append(" AND     (ImpresexParticelle.inviato >= 0)  ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
    '                StrSQL.Append(" AND     (ImpresexParticelle.inviato = -1)  ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                '...................................
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select
    '        '--------------------------------------------------------------------------
    '        If xOrderBy <> "" Then
    '            strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        End If



    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try


    '    Return DT

    'End Function
    ''###################################################################################

    '<Obsolete("Usare ImpresexParticelle2_R.Esiste_CentroxParticella")>
    'Public Function Esiste_CentroxParticella(ByVal Piva As String, _
    '                                            ByVal Sa_Cod As Integer, _
    '                                            ByVal Prov As String, _
    '                                            ByVal Com As String, _
    '                                            ByVal Sezione As String, _
    '                                            ByVal Foglio As Integer, _
    '                                            ByVal Numero As Integer, _
    '                                            ByVal Subalterno As String, _
    '                                            ByVal xFiltroAggiuntivo As String, _
    '                                            ByVal xOrderBy As String, _
    '                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle.Esiste_CentroxParticella()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable
    '    Dim Flag_Esiste As Boolean = False

    '    If Sezione = "" Then
    '        Sezione = "0"
    '    End If
    '    If Subalterno = "" Then
    '        Subalterno = "0"
    '    End If

    '    Try

    '        DT = LeggixChiave(Piva, _
    '                        Sa_Cod, _
    '                        Prov, _
    '                        Com, _
    '                        Sezione, _
    '                        Foglio, _
    '                        Numero, _
    '                        Subalterno, _
    '                         enumSelezioneVariabile.Selezione_TabellaDatiMinimi, _
    '                         xFiltroAggiuntivo, xOrderBy, _
    '                          objParametri)

    '        If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
    '            Flag_Esiste = True
    '        End If

    '        DT = Nothing

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return Flag_Esiste

    'End Function


    'Legge le informazioni di una determinata particella catastale senza vincoli temporali.
    '(Gias Lan: non esistono filtri temporali poichè mi interessa sapere prima della cancellazione
    'se un'altro utente utilizza la stessa particella).
    '##############################################################################################
    '<Obsolete("Usare ImpresexParticelle2_R.LeggixChiave")>
    'Public Function LeggixChiave(ByVal PIVA As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal PROV As String, _
    '                        ByVal COM As String, _
    '                        ByVal SEZIONE As String, _
    '                        ByVal FOGLIO As Int32, _
    '                        ByVal NUMERO As Int32, _
    '                        ByVal SUBALTERNO As String, _
    '                            ByVal xSelezioneVariabile As enumSelezioneVariabile, _
    '                            ByVal xFiltroAggiuntivo As String, _
    '                            ByVal xOrderBy As String, _
    '                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.LeggixChiave()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------
    '        Select Case xSelezioneVariabile

    '            Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

    '                StrSQL.Length = 0
    '                StrSQL.Append(" SELECT  PIVA, sa_cod, TitoloPossesso, Sup_Condotta, Validita_Inizio, Validita_Fine ")
    '                StrSQL.Append(" FROM  ImpresexParticelle  ")
    '                StrSQL.Append(" WHERE   ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '                StrSQL.Append(" AND     ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '                StrSQL.Append(" AND     ImpresexParticelle.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE,false) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
    '                StrSQL.Append(" AND     ImpresexParticelle.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO,false) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
    '                StrSQL.Append(" AND     ImpresexParticelle.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO,false) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
    '                StrSQL.Append(" AND     ImpresexParticelle.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO,false) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")

    '                If PIVA <> "" Then
    '                    StrSQL.Append(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
    '                End If

    '                If Sa_Cod <> 0 Then
    '                    StrSQL.Append(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '                End If

    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                Select Case objParametri.FlagVisibilita
    '                    Case enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND     (ImpresexParticelle.inviato >= 0)  ")
    '                    Case enumVisibilita.Visibilita_SoloCancellati
    '                        StrSQL.Append(" AND     (ImpresexParticelle.inviato = -1)  ")
    '                    Case enumVisibilita.Visibilita_Tutti
    '                        '...................................
    '                    Case Else
    '                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '                End Select
    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                End If


    '            Case enumSelezioneVariabile.Selezione_TabellaCompleta

    '                StrSQL.Length = 0
    '                StrSQL.Append(" SELECT * ")
    '                StrSQL.Append(" FROM  ImpresexParticelle , ParticelleCatastali ")
    '                StrSQL.Append(" WHERE ImpresexParticelle.PROV = ParticelleCatastali.PROV ")
    '                StrSQL.Append(" AND   ImpresexParticelle.COM = ParticelleCatastali.COM ")
    '                StrSQL.Append(" AND   ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
    '                StrSQL.Append(" AND   ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
    '                StrSQL.Append(" AND   ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO ")
    '                StrSQL.Append(" AND   ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
    '                StrSQL.Append(" AND   ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '                StrSQL.Append(" AND   ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '                StrSQL.Append(" AND   ImpresexParticelle.Sezione = '" & IIf(Agro_SQL_SaveText(SEZIONE,false) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), "0") & "'  ")
    '                StrSQL.Append(" AND   ImpresexParticelle.FOGLIO = " & IIf(Agro_SQL_SaveNum(FOGLIO,false) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
    '                StrSQL.Append(" AND   ImpresexParticelle.Numero = " & IIf(Agro_SQL_SaveNum(NUMERO,false) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
    '                StrSQL.Append(" AND   ImpresexParticelle.SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO,false) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), "0") & "' ")

    '                If PIVA <> "" Then
    '                    StrSQL.Append(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
    '                End If

    '                If Sa_Cod <> 0 Then
    '                    StrSQL.Append(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '                End If


    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                Select Case objParametri.FlagVisibilita
    '                    Case enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND     (ParticelleCatastali.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (ImpresexParticelle.inviato >= 0)  ")
    '                    Case enumVisibilita.Visibilita_SoloCancellati
    '                        StrSQL.Append(" AND     (ParticelleCatastali.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (ImpresexParticelle.inviato = -1)  ")
    '                    Case enumVisibilita.Visibilita_Tutti
    '                        '...................................
    '                    Case Else
    '                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '                End Select
    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                End If


    '            Case enumSelezioneVariabile.Selezione_JoinDescrizioni
    '                '
    '                '
    '                '
    '                '


    '            Case enumSelezioneVariabile.Selezione_JoinCompleta
    '                '
    '                '
    '                '
    '                '


    '        End Select



    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try


    '    Return DT

    'End Function

    '##############################################################################################
    <Obsolete("Usare ImpresexParticelle2_R.Esistono_Macrousi (eliminare parametro Part_Cod)")>
    Public Function Esistono_Macrousi(ByVal PIVA As String, _
                                ByVal Sa_Cod As Int32, _
                                ByVal Part_Cod As Int32, _
                                ByVal PROV As String, _
                                ByVal COM As String, _
                                ByVal SEZIONE As String, _
                                ByVal FOGLIO As Int32, _
                                ByVal NUMERO As Int32, _
                                ByVal SUBALTERNO As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_R.Esistono_Macrousi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  
        '   Sa_Cod = 0                  =>
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Esistono As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT  ParticelleCatastali.*, ")
            StrSQL.Append(" ImpresexParticelle.Validita_Inizio as xValidita_Inizio, ")
            StrSQL.Append(" ImpresexParticelle.Validita_Fine as xValidita_Fine, ")
            StrSQL.Append(" ImpresexParticelle.TitoloPossesso as xTitoloPossesso, ImpresexParticelle.Sup_Condotta, ")
            StrSQL.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, Lista_Province.PROVINCIA, ")
            StrSQL.Append(" ImpresexParticelle.Piva, Imprese.rag_soc, ImpresexParticelle.Sa_Cod, Centri_Aziendali.sa_nome, ")

            StrSQL.Append(" ISNULL(ParticelleCatastalixMacrousi.Macrouso_Cod,'') as Macrouso_Cod, ParticelleCatastalixMacrousi.Superficie AS Sup_Macrouso, ISNULL(Macrousi.Macrouso_Des,'') AS Macrouso_Des ")

            StrSQL.Append(" FROM   Macrousi INNER JOIN ")
            StrSQL.Append("        ParticelleCatastalixMacrousi ON Macrousi.Macrouso_Cod = ParticelleCatastalixMacrousi.Macrouso_Cod INNER JOIN ")
            StrSQL.Append("        ImpreseXParticelle INNER JOIN ")
            StrSQL.Append("        ParticelleCatastali ON ImpreseXParticelle.PROV = ParticelleCatastali.PROV AND ImpreseXParticelle.COM = ParticelleCatastali.COM AND  ")
            StrSQL.Append("        ImpreseXParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.Append("        ImpreseXParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpreseXParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO INNER JOIN ")
            StrSQL.Append("        ISTAT ON ImpreseXParticelle.PROV = ISTAT.PROV AND ImpreseXParticelle.COM = ISTAT.COM INNER JOIN ")
            StrSQL.Append("        Lista_Province ON ISTAT.PROV = Lista_Province.PROV INNER JOIN ")
            StrSQL.Append("        Imprese ON ImpreseXParticelle.PIVA = Imprese.PIVA INNER JOIN ")
            StrSQL.Append("        Centri_Aziendali ON ImpreseXParticelle.PIVA = Centri_Aziendali.PIVA AND ImpreseXParticelle.sa_cod = Centri_Aziendali.sa_cod ON  ")
            StrSQL.Append("        ParticelleCatastalixMacrousi.PROV = ParticelleCatastali.PROV AND ParticelleCatastalixMacrousi.COM = ParticelleCatastali.COM AND  ")
            StrSQL.Append("        ParticelleCatastalixMacrousi.SEZIONE = ParticelleCatastali.SEZIONE AND ParticelleCatastalixMacrousi.FOGLIO = ParticelleCatastali.FOGLIO AND  ")
            StrSQL.Append("        ParticelleCatastalixMacrousi.NUMERO = ParticelleCatastali.NUMERO AND  ")
            StrSQL.Append("        ParticelleCatastalixMacrousi.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")


            StrSQL.Append(" WHERE   (ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND     (ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
            StrSQL.Append(" AND     (ImpresexParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
            StrSQL.Append(" AND     (ImpresexParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

            If (PIVA <> "") Then
                StrSQL.Append(" AND ImpresexParticelle.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                StrSQL.Append(" AND ParticelleCatastalixMacrousi.PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND ImpresexParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If Part_Cod <> 0 Then
                StrSQL.Append(" AND ParticelleCatastali.Part_Cod = " & Agro_SQL_SaveNum(Part_Cod) & "  ")
            End If

            If PROV <> "" Then
                StrSQL.Append(" AND ImpresexParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                StrSQL.Append(" AND ImpresexParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                StrSQL.Append(" AND ImpresexParticelle.SEZIONE = '" & Agro_SQL_SaveText(Trim(LCase(SEZIONE))) & "' ")
            End If

            If FOGLIO <> 0 Then
                StrSQL.Append(" AND ImpresexParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                StrSQL.Append(" AND ImpresexParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                StrSQL.Append(" AND ImpresexParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     (ImpresexParticelle.inviato >= 0)  ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     (ImpresexParticelle.inviato = -1)  ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Esistono = True
            Else
                Esistono = False
            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Esistono = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Esistono

    End Function


End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

<Obsolete("Usare ImpresexParticelle2_W")>
Public Class ImpresexParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    <Obsolete("Usare ImpresexParticelle2_W.Scrivi_2")>
    Public Function Scrivi(ByVal Piva As String, _
                               ByVal Sa_Cod As Int32, _
                               ByVal PROV As String, _
                               ByVal COM As String, _
                               ByVal SEZIONE As String, _
                               ByVal FOGLIO As Int32, _
                               ByVal NUMERO As Int32, _
                               ByVal SUBALTERNO As String, _
                               ByVal Partita_Catastale As String, _
                               ByVal Validita_Inizio As Date, _
                               ByVal Validita_Fine As Date, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO ImpresexParticelle(       ")
            StrSQL.Append("                    PIVA, Sa_Cod, PROV, COM, SEZIONE, ")
            StrSQL.Append("                    FOGLIO, NUMERO, SUBALTERNO,  Partita_Catastale,     ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Partita_Catastale) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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

    <Obsolete("Usare ImpresexParticelle2_W.Scrivi_2")>
    Public Function Scrivi(ByVal Piva As String, _
                               ByVal Sa_Cod As Int32, _
                               ByVal PROV As String, _
                               ByVal COM As String, _
                               ByVal SEZIONE As String, _
                               ByVal FOGLIO As Int32, _
                               ByVal NUMERO As Int32, _
                               ByVal SUBALTERNO As String, _
                               ByVal Partita_Catastale As String, _
                               ByVal TitoloPossesso As Integer, _
                               ByVal Sup_Condotta As Decimal, _
                               ByVal Validita_Inizio As Date, _
                               ByVal Validita_Fine As Date, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                               ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO ImpresexParticelle(       ")
            StrSQL.Append("                    PIVA, Sa_Cod, PROV, COM, SEZIONE, ")
            StrSQL.Append("                    FOGLIO, NUMERO, SUBALTERNO,  Partita_Catastale,     ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("                    Sup_Condotta, TitoloPossesso) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            StrSQL.Append("         , " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            StrSQL.Append("         ,'" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Partita_Catastale) & "' ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sup_Condotta) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(TitoloPossesso) & "  ")
            StrSQL.Append(")")

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
    <Obsolete("Usare ImpresexParticelle2_W.Cancella")>
    Public Function Cancella(ByVal PIVA As String, _
                            ByVal Sa_Cod As Int32, _
                            ByVal PROV As String, _
                            ByVal COM As String, _
                            ByVal SEZIONE As String, _
                            ByVal FOGLIO As Int32, _
                            ByVal NUMERO As Int32, _
                            ByVal SUBALTERNO As String, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Prov = ""
        '   Com = ""
        '   Sezione = ""
        '   Foglio = 0
        '   Numero = 0
        '   Subalterno = ""
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try



            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE ImpresexParticelle ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  PIVA <> '0' ")
                StrSQL.Append(" AND Inviato >= 0")

                If PIVA <> "" Then
                    StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                End If

                If PROV <> "" Then
                    StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
                End If
                If COM <> "" Then
                    StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
                End If
                If SEZIONE <> "" Then
                    StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                End If
                If FOGLIO <> 0 Then
                    StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
                End If
                If NUMERO <> 0 Then
                    StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
                End If
                If SUBALTERNO <> "" Then
                    StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     ImpresexParticelle ")
                StrSQL.Append(" WHERE  PIVA <> '0' ")

                If PIVA <> "" Then
                    StrSQL.Append(" AND  Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                End If

                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
                End If

                If PROV <> "" Then
                    StrSQL.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
                End If
                If COM <> "" Then
                    StrSQL.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
                End If
                If SEZIONE <> "" Then
                    StrSQL.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                End If
                If FOGLIO <> 0 Then
                    StrSQL.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
                End If
                If NUMERO <> 0 Then
                    StrSQL.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
                End If
                If SUBALTERNO <> "" Then
                    StrSQL.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
                End If
            End If


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

    ''Cancellazione di una determinata associazione.
    ''(Gias Lan: utilizzata nel caso di eliminazione di UNA particella catastale)
    ''##############################################################################################
    '<Obsolete("Usare ImpresexParticelle2_W.Cancella_Associazione")>
    'Public Function Cancella_Associazione(ByVal PIVA As String, _
    '                        ByVal Sa_Cod As Int32, _
    '                        ByVal PROV As String, _
    '                        ByVal COM As String, _
    '                        ByVal SEZIONE As String, _
    '                        ByVal FOGLIO As Int32, _
    '                        ByVal NUMERO As Int32, _
    '                        ByVal SUBALTERNO As String, _
    '                            ByVal xFiltroAggiuntivo As String, _
    '                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                            ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Cancella_Associazione()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

    '            StrSQL.Length = 0
    '            StrSQL.Append(" UPDATE ImpresexParticelle ")
    '            StrSQL.Append(" SET ")
    '            StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
    '            StrSQL.Append("      ,Validita_Fine = " & Agro_SQL_SaveDate(CDate("01/01/1900")) & " ")
    '            StrSQL.Append("      ,Inviato = -1 ")
    '            StrSQL.Append(" WHERE  PIVA <> '0' ")
    '            StrSQL.Append(" AND Inviato >= 0")

    '            StrSQL.Append(" AND      PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
    '            StrSQL.Append(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '            StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '            StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE,false) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
    '            StrSQL.Append(" AND      FOGLIO      =  " & IIf(Agro_SQL_SaveNum(FOGLIO,false) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
    '            StrSQL.Append(" AND      Numero      =  " & IIf(Agro_SQL_SaveNum(NUMERO,false) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
    '            StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO,false) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

    '        Else

    '            StrSQL.Length = 0
    '            StrSQL.Append(" DELETE ")
    '            StrSQL.Append(" FROM     ImpresexParticelle ")
    '            StrSQL.Append(" WHERE    PIVA <> '0' ")

    '            StrSQL.Append(" AND      PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
    '            StrSQL.Append(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
    '            StrSQL.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
    '            StrSQL.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
    '            StrSQL.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE,false) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
    '            StrSQL.Append(" AND      FOGLIO      =  " & IIf(Agro_SQL_SaveNum(FOGLIO,false) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
    '            StrSQL.Append(" AND      Numero      =  " & IIf(Agro_SQL_SaveNum(NUMERO,false) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
    '            StrSQL.Append(" AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO,false) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

    '        End If
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

    <Obsolete("Usare ImpresexParticelle2_W.Modifica")>
    Public Sub Modifica(ByVal Piva As String, _
                    ByVal Sa_Cod As Int32, _
                    ByVal PROV As String, _
                    ByVal COM As String, _
                    ByVal SEZIONE As String, _
                    ByVal FOGLIO As Int32, _
                    ByVal NUMERO As Int32, _
                    ByVal SUBALTERNO As String, _
                    ByVal Partita_Catastale As String, _
                          ByVal Validita_Inizio As Date, _
                          ByVal Validita_Fine As Date, _
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                          )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE ImpresexParticelle SET " &
                        "        Partita_Catastale  = '" & Agro_SQL_SaveText(Partita_Catastale) & "'" &
                        "       ,Inviato            =  0 " &
                        "       ,DataInvio          =  Null " &
                        "       ,Data_Modifica      =  " & Agro_SQL_SaveDate(Date.Now) &
                        "       ,UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'" &
                        "       ,Validita_Inizio    =  " & Agro_SQL_SaveDate(Validita_Inizio) &
                        "       ,Validita_Fine      =  " & Agro_SQL_SaveDate(Validita_Fine) &
                        " WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                        " AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  " &
                        " AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' " &
                        " AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' " &
                        " AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  " &
                        " AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  " &
                        " AND      Numero      = " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  " &
                        " AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    Public Sub Modifica_Superficie(ByVal Old_Piva As String, _
                        ByVal Old_Sa_Cod As Int32, _
                        ByVal Old_PROV As String, _
                        ByVal Old_COM As String, _
                        ByVal Old_SEZIONE As String, _
                        ByVal Old_FOGLIO As Int32, _
                        ByVal Old_NUMERO As Int32, _
                        ByVal Old_SUBALTERNO As String, _
                        ByVal New_Sup_Condotta As Decimal, _
                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                        )

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE ImpresexParticelle SET " &
                        "        Sup_Condotta  = " & Agro_SQL_SaveNum(New_Sup_Condotta) &
                        "       ,Data_Modifica      =  " & Agro_SQL_SaveDateTime(DateTime.Now) &
                        "       ,UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'" &
                        " WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(Old_Piva)) & "' " &
                        " AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Old_Sa_Cod) & "  " &
                        " AND      PROV        = '" & Agro_SQL_SaveText(Trim(Old_PROV)) & "' " &
                        " AND      COM         = '" & Agro_SQL_SaveText(Trim(Old_COM)) & "' " &
                        " AND      Sezione     = '" & IIf(Agro_SQL_SaveText(Old_SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(Old_SEZIONE)), 0) & "'  " &
                        " AND      FOGLIO      = " & IIf(Agro_SQL_SaveNum(Old_FOGLIO, False) <> 0, Agro_SQL_SaveNum(Old_FOGLIO), 0) & "  " &
                        " AND      Numero      = " & IIf(Agro_SQL_SaveNum(Old_NUMERO, False) <> 0, Agro_SQL_SaveNum(Old_NUMERO), 0) & "  " &
                        " AND      SUBALTERNO =  '" & IIf(Agro_SQL_SaveText(Old_SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(Old_SUBALTERNO)), 0) & "' ")



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub

    <Obsolete("Usare ImpresexParticelle2_W.Modifica_Centro")>
    Public Function Modifica_Centro(ByVal Piva As String,
                                    ByVal Sa_Cod_OLD As Integer,
                                    ByVal Sa_Cod_NEW As Integer,
                                    ByVal PROV As String,
                                    ByVal COM As String,
                                    ByVal SEZIONE As String,
                                    ByVal FOGLIO As Integer,
                                    ByVal NUMERO As Integer,
                                    ByVal SUBALTERNO As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ImpresexParticelle_W.Modifica_Centro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE ImpresexParticelle SET " &
                        "        sa_cod  = " & Agro_SQL_SaveNum(Sa_Cod_NEW) &
                        "       ,Data_Modifica      =  " & Agro_SQL_SaveDateTime(Now) &
                        "       ,UserName_Modifica  = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'" &
                        " WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                        " AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod_OLD))

            If PROV <> "" Then
                StrSQL.AppendLine(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If
            If COM <> "" Then
                StrSQL.AppendLine(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If
            If SEZIONE <> "" Then
                StrSQL.AppendLine(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If
            If FOGLIO <> 0 Then
                StrSQL.AppendLine(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If
            If NUMERO <> 0 Then
                StrSQL.AppendLine(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If
            If SUBALTERNO <> "" Then
                StrSQL.AppendLine(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
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
