Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class GerarchiaImprese_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function OrganismiReferenti_Leggi(
                                    ByVal Piva As String,
                                    ByVal Piva_Padre As String,
                                            ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.OrganismiReferenti_Leggi()"

        '######################################################
        '#### Piva ----> è la partita iva dell'azienda della
        '               quale si devono elencare le possibili 
        '               imprese/cooperative referenti
        '######################################################
        '#### Piva_Padre ----> è la partita iva dell'impresa/
        '               organismo referente selezionato
        '(non c'entra niente con la partita iva dell'impresa che 
        ' ha creato il contatto, anzi è lei il contatto!)
        '######################################################

        Dim MessaggioErrore As String = ""
        Dim stb As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim i As Integer
        i = 0
        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    stb.Length = 0
                    stb.AppendLine(" (   SELECT  GerarchiaImprese.Padre AS piva_padre, Imprese.rag_soc AS ragsoc_padre ")
                    stb.AppendLine("     FROM    GerarchiaImprese INNER JOIN ")
                    stb.AppendLine("             Imprese ON GerarchiaImprese.Padre = Imprese.PIVA ")
                    stb.AppendLine(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
                    stb.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If Piva <> "" Then
                        stb.AppendLine(" AND     GerarchiaImprese.Figlio  = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If
                    If Piva_Padre <> "" Then
                        stb.AppendLine(" AND     GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Piva_Padre) & "'   ")
                    End If



                    stb.AppendLine(" ) ")


                    stb.AppendLine("     UNION ")

                    stb.AppendLine(" (   SELECT  Contatti.cod_contatto AS piva_padre, Contatti.rag_soc AS ragsoc_padre ")
                    stb.AppendLine("     FROM    Contatti INNER JOIN ")
                    stb.AppendLine("             Risorse_Umane ON Contatti.piva = Risorse_Umane.piva AND Contatti.cod_contatto = Risorse_Umane.cod_contatto ")
                    stb.AppendLine("             INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.PIVA ")
                    stb.AppendLine("     WHERE   Risorse_Umane.cod_rapporto = - 2 ")
                    stb.AppendLine("     AND     UtentiXImprese.[USER] = '" & CStr(objParametri.PivaSuperUser) & "' ")
                    stb.AppendLine("     AND     Contatti.id_cf = 1 ")
                    If Piva <> "" Then
                        'cerco le possibili cooperative referenti, create dall'azienda o pubbliche (create da altre aziende)
                        stb.AppendLine(" AND    (Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Contatti.Sa_Cod = -1 )  ")
                    End If
                    If Piva_Padre <> "" Then
                        'cerco un particolare organismo referente
                        stb.AppendLine(" AND     Contatti.cod_contatto = '" & Agro_SQL_SaveText(Piva_Padre) & "'   ")
                    End If
                    stb.AppendLine("     AND   Risorse_Umane.Cod_RisUm_Origine = 0   ")
                    stb.AppendLine(" ) ")
                    stb.AppendLine("")



                    ''--------------------------------------------------------------------------
                    'Select Case objParametri.FlagVisibilita
                    '    Case enumVisibilita.Visibilita_SoloNonCancellati
                    '        StrSQL.Append(" AND   Inviato >=0 ")
                    '    Case enumVisibilita.Visibilita_SoloNonCancellati
                    '        StrSQL.Append(" AND   Inviato =-1 ")
                    '    Case enumVisibilita.Visibilita_Tutti
                    '        '...................................
                    '    Case Else
                    '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    'End Select
                    ''--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        stb.AppendLine(" ORDER BY ragsoc_padre ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



    ''################################################################################
    'Public Function OrganismoReferente_from_Piva(ByVal Piva As String, _
    '                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                                ) As String

    '    Dim DT_Coop As DataTable

    '    'ATTENZIONE! passare la piva come piva_padre, non piva!!!!
    '    Dim obj As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
    '    DT_Coop = obj.OrganismiReferenti_Leggi("", Piva, _
    '                         enumSelezioneVariabile.Selezione_TabellaCompleta, _
    '                         "", _
    '                         objParametri)

    '    If DT_Coop.Rows.Count <> 0 Then
    '        Return DT_Coop.Rows(0).Item("ragsoc_padre")
    '    Else
    '        Return ""
    '    End If


    'End Function



    '##############################################################################################
    'Public Function Leggi( _
    '                        ByVal Padre As String, _
    '                        ByVal FiltroFoglia As Int32, _
    '                        ByVal FinestraTemp_Inizio As Date, _
    '                        ByVal FinestraTemp_Fine As Date, _
    '                        ByRef objConnessione As DbConnection, _
    '                        ByVal PivaSuperUser As String, _
    '                        ByVal StringaConnessione As String, _
    '                        ByVal FlagVisibilita As Int32, _
    '                        ByVal DirectoryLOG As String, _
    '                        ByVal FileLOG As String, _
    '                        ByVal IdentificatoreUtente As String _
    '                        ) As DataTable

    '    Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Piva = ""                   =>  si leggono tutte le imprese
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
    '        StrSQL.Append(" SELECT GerarchiaImprese.* ")
    '        StrSQL.Append(" FROM  GerarchiaImprese")
    '        StrSQL.Append(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
    '        'StrSQL.Append(" WHERE GerarchiaImprese.Foglia = 0 ")
    '        StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")

    '        StrSQL.Append(" AND GerarchiaImprese.Validita_inizio < " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
    '        StrSQL.Append(" AND GerarchiaImprese.Validita_Fine > " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

    '        Select Case FiltroFoglia
    '            Case 1
    '                StrSQL.Append(" AND GerarchiaImprese.Foglia = 1")
    '            Case 2
    '                StrSQL.Append(" AND GerarchiaImprese.Foglia = 0")
    '        End Select

    '        If Padre <> "" Then
    '            StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Padre) & "'")
    '        End If


    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND GerarchiaImprese.Inviato >= 0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND (GerarchiaImprese.Inviato = -1) ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select

    '        StrSQL.Append(" ORDER BY GerarchiaImprese.Padre ASC")
    '        '---------------------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return DT

    'End Function

    Public Function Leggi(ByVal Padre As String,
                          ByVal FiltroFoglia As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                     enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT GerarchiaImprese.* ")
                    StrSQL.Append(" FROM  GerarchiaImprese")
                    StrSQL.Append(" WHERE GerarchiaImprese.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   GerarchiaImprese.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    Select Case FiltroFoglia
                        Case 1
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 1")
                        Case 2
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 0")
                    End Select

                    If Padre <> "" Then
                        StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Padre) & "'")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GerarchiaImprese.Padre ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT GerarchiaImprese.* ")
                    StrSQL.Append(" FROM  GerarchiaImprese")
                    StrSQL.Append(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
                    'StrSQL.Append(" WHERE GerarchiaImprese.Foglia = 0 ")
                    StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND GerarchiaImprese.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   GerarchiaImprese.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    Select Case FiltroFoglia
                        Case 1
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 1")
                        Case 2
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 0")
                    End Select

                    If Padre <> "" Then
                        StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Padre) & "'")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GerarchiaImprese.Padre ASC")
                    End If



                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT GerarchiaImprese.*,Imprese.PIVA, imprese.rag_soc,imprese.TipoImpresaGerarchia, ")
                    StrSQL.Append(" Lista_Province.PROVINCIA, IC.val_cod AS CodiceSocio, IC_Cuaa.val_cod AS CodiceCuaa ")

                    StrSQL.Append(" FROM  GerarchiaImprese")
                    StrSQL.Append(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
                    StrSQL.Append(" INNER JOIN Imprese ON GerarchiaImprese.Figlio = Imprese.PIVA ")

                    StrSQL.Append(" INNER join ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA " & vbCrLf)
                    StrSQL.Append(" INNER JOIN Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
                    StrSQL.Append(" INNER JOIN Lista_Province ON Lista_Province.PROV = Indirizzi.pro_cod_istat " & vbCrLf)

                    StrSQL.Append(" LEFT OUTER JOIN Imprese_Codici IC on (IC.Piva = Imprese.Piva) AND IC.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & " " & vbCrLf)
                    StrSQL.Append(" LEFT OUTER JOIN Imprese_Codici IC_cuaa on (IC_cuaa.Piva = Imprese.Piva) AND IC_cuaa.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA & " " & vbCrLf)

                    StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND GerarchiaImprese.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   GerarchiaImprese.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    Select Case FiltroFoglia
                        Case 1
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 1")
                        Case 2
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 0")
                    End Select

                    If Padre <> "" Then
                        StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Padre) & "'")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GerarchiaImprese.Padre ASC")
                    End If


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

    'filtr anche il figlio
    Public Function Leggi2(ByVal Padre As String,
                            ByVal Figlio As String,
                            ByVal FiltroFoglia As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi2()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT GerarchiaImprese.* ")
            StrSQL.Append(" FROM  GerarchiaImprese")
            StrSQL.Append(" WHERE 1  =  1 ")

            Select Case FiltroFoglia
                Case 1
                    StrSQL.Append(" AND GerarchiaImprese.Foglia = 1")
                Case 2
                    StrSQL.Append(" AND GerarchiaImprese.Foglia = 0")
            End Select

            If Padre <> "" Then
                StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Padre) & "'")
            End If

            If Figlio <> "" Then
                StrSQL.Append(" AND GerarchiaImprese.Figlio = '" & Agro_SQL_SaveText(Figlio) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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


    Public Function LeggixAlberoImpresexSementi(
                                       ByVal ListaPiva As String,
                                       ByVal Codice_Sementi As Integer,
                                       ByVal Data_Inizio As String,
                                       ByVal Data_Fine As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" select DISTINCT a.Padre, a.rag_soc, a.PIVA, a.Foglia, a.Validazione, a.TipoImpresaGerarchia, a.Blk_Flag, a.validita_fine FROM ")

            StrSQL.Append(" ( ")
            StrSQL.Append(" SELECT  GerarchiaImprese.Padre, Imprese.rag_soc, Imprese.PIVA, GerarchiaImprese.Foglia, Imprese.Validazione, Imprese.TipoImpresaGerarchia , Imprese.Blk_Flag, Imprese.validita_fine ")
            StrSQL.Append(" FROM GerarchiaImprese (NOLOCK)  ")
            StrSQL.Append("     INNER JOIN Imprese (NOLOCK)  ON GerarchiaImprese.Figlio = Imprese.PIVA  ")
            StrSQL.Append("     INNER JOIN Reg_Impianti (NOLOCK)  ON Imprese.PIVA = Reg_Impianti.PIVA  ")
            StrSQL.Append("     INNER JOIN Cultivar (NOLOCK)  on cultivar.cul_cod = Reg_Impianti.CUL_COD ")
            StrSQL.Append("     INNER JOIN Mappatura_Specie (NOLOCK)  on Mappatura_Specie.Veg_Cod = Cultivar.Veg_Cod ")

            StrSQL.Append(" where Id_Specie = " & Codice_Sementi)
            StrSQL.Append(" AND Reg_Impianti.Validita_Inizio >= " & Agro_SQL_SaveDate(Data_Inizio))
            StrSQL.Append(" AND Reg_Impianti.Validita_Fine <= " & Agro_SQL_SaveDate(Data_Fine))
            StrSQL.Append(" AND (Padre in (" & Agro_SQL_Save_Clausola_IN(ListaPiva, True) & ")")
            StrSQL.Append(" OR Imprese.PIVA in (" & Agro_SQL_Save_Clausola_IN(ListaPiva, True) & "))")

            StrSQL.Append(" UNION ")

            StrSQL.Append(" SELECT  GerarchiaImprese.Padre, Imprese.rag_soc, Imprese.PIVA, GerarchiaImprese.Foglia, Imprese.Validazione, Imprese.TipoImpresaGerarchia , Imprese.Blk_Flag, Imprese.validita_fine ")
            StrSQL.Append(" FROM GerarchiaImprese (NOLOCK)  ")
            StrSQL.Append("     INNER JOIN Imprese (NOLOCK)  ON GerarchiaImprese.Figlio = Imprese.PIVA  ")
            StrSQL.Append("     INNER JOIN Programmazione_Entita (NOLOCK)   ON Imprese.PIVA = Programmazione_Entita.PIVA  ")
            StrSQL.Append("     INNER JOIN Cultivar (NOLOCK)  on cultivar.cul_cod = Programmazione_Entita.CUL_COD ")
            StrSQL.Append("     INNER JOIN Mappatura_Specie (NOLOCK)  on Mappatura_Specie.Veg_Cod = Cultivar.Veg_Cod ")

            StrSQL.Append(" where Id_Specie = " & Codice_Sementi)
            StrSQL.Append(" AND Programmazione_Entita.Validita_Inizio >= " & Agro_SQL_SaveDate(Data_Inizio))
            StrSQL.Append(" AND Programmazione_Entita.Validita_Fine <= " & Agro_SQL_SaveDate(Data_Fine))
            StrSQL.Append(" AND (Padre in (" & Agro_SQL_Save_Clausola_IN(ListaPiva, True) & ")")
            StrSQL.Append(" OR Imprese.PIVA in (" & Agro_SQL_Save_Clausola_IN(ListaPiva, True) & "))")

            StrSQL.Append(" UNION ")

            StrSQL.Append(" SELECT  GerarchiaImprese.Padre, Imprese.rag_soc, Imprese.PIVA, GerarchiaImprese.Foglia, Imprese.Validazione, Imprese.TipoImpresaGerarchia , Imprese.Blk_Flag, Imprese.validita_fine ")
            StrSQL.Append("    FROM GerarchiaImprese (NOLOCK)  INNER JOIN Imprese (NOLOCK)  ON GerarchiaImprese.Figlio = Imprese.PIVA ")
            StrSQL.Append(" where(Foglia = 0) ")
            StrSQL.Append(" AND (Padre in (" & Agro_SQL_Save_Clausola_IN(ListaPiva, True) & ")")
            StrSQL.Append(" OR Imprese.PIVA in (" & Agro_SQL_Save_Clausola_IN(ListaPiva, True) & ")")
            'StrSQL.Append(" OR GerarchiaImprese.livello = 1 OR GerarchiaImprese.livello = 2 )")
            StrSQL.Append("   )")


            StrSQL.Append(" ) a ")

            StrSQL.Append("  order by a.rag_soc ")



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



    Public Function LeggixGerarchiaAlberoImprese(
                                       ByVal ListaPiva As String,
                                       ByVal Data_Inizio As String,
                                       ByVal Data_Fine As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" select DISTINCT a.Padre, a.rag_soc, a.PIVA, a.Foglia, a.Validazione, a.TipoImpresaGerarchia, a.Blk_Flag, a.validita_fine FROM  ")

            StrSQL.Append(" ( ")
            StrSQL.Append(" SELECT  GerarchiaImprese.Padre, Imprese.rag_soc, Imprese.PIVA, GerarchiaImprese.Foglia, Imprese.Validazione, Imprese.TipoImpresaGerarchia , Imprese.Blk_Flag, Imprese.validita_fine  ")
            StrSQL.Append(" FROM GerarchiaImprese  (NOLOCK)  ")
            StrSQL.Append("     INNER JOIN Imprese  (NOLOCK)  ON GerarchiaImprese.Figlio = Imprese.PIVA  ")

            StrSQL.Append(" where 1=1 ")
            If ListaPiva <> "" Then
                StrSQL.Append(" AND (Padre in (" & Agro_SQL_Save_Clausola_IN(ListaPiva, True) & ")")
                StrSQL.Append(" OR Imprese.PIVA in (" & Agro_SQL_Save_Clausola_IN(ListaPiva, True) & "))")
            End If

            StrSQL.Append("     ) as a ")

            StrSQL.Append("  order by a.rag_soc ")



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


    Public Function LeggixGerarchiaAlberoImprese_Visibilita(
                                       ByVal ApplicaVisibilita As Boolean,
                                       ByVal Data_Inizio As String,
                                       ByVal Data_Fine As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" select DISTINCT a.Padre, a.rag_soc, a.PIVA, a.Foglia, a.Validazione, a.TipoImpresaGerarchia, a.Blk_Flag, a.validita_fine FROM  ")

            StrSQL.Append(" ( ")
            StrSQL.Append(" SELECT  GerarchiaImprese.Padre, Imprese.rag_soc, Imprese.PIVA, GerarchiaImprese.Foglia, Imprese.Validazione, Imprese.TipoImpresaGerarchia , Imprese.Blk_Flag, Imprese.validita_fine  ")
            StrSQL.Append(" FROM GerarchiaImprese (NOLOCK) ")
            StrSQL.Append("     INNER JOIN Imprese (NOLOCK) ON GerarchiaImprese.Figlio = Imprese.PIVA  ")
            If ApplicaVisibilita Then
                StrSQL.Append(" LEFT JOIN Utenti_Visibilita_Appoggio uvap (NOLOCK) ON GerarchiaImprese.Padre = uvap.Piva AND uvap.Entita_Cod = 1 AND uvap.sa_cod = 0 AND uvap.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                StrSQL.Append(" LEFT JOIN Utenti_Visibilita_Appoggio uvaf (NOLOCK) ON GerarchiaImprese.Figlio = uvaf.Piva AND uvaf.Entita_Cod = 1 AND uvaf.sa_cod = 0 AND uvaf.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            End If
            StrSQL.Append(" where 1=1 ")
            If ApplicaVisibilita Then
                StrSQL.Append(" AND NOT (uvap.Piva IS NULL AND uvaf.Piva is NULL) ")
            End If

            StrSQL.Append("     ) as a ")

            StrSQL.Append("  order by a.rag_soc ")



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


    Public Function LeggiFigli(ByVal Padre As String,
                               ByVal Figlio As String,
                               ByVal Foglia As Integer,
                               ByVal Livello As Integer,
                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                               ByVal xFiltroAggiuntivo As String,
                               ByVal xOrderBy As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                               ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    'StrSQL.Append(" SELECT distinct GerarchiaImprese.Figlio, Imprese.Rag_Soc AS RagSoc_Figlio ")

                    StrSQL.Append(" SELECT * ")

                    StrSQL.Append(" FROM  GerarchiaImprese ")
                    StrSQL.Append(" INNER JOIN Imprese ON GerarchiaImprese.Figlio = Imprese.PIVA ")
                    StrSQL.Append(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
                    'StrSQL.Append(" WHERE GerarchiaImprese.Foglia = 0 ")
                    StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND   (GerarchiaImprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    StrSQL.Append(" AND     (GerarchiaImprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    '----- Condizioni

                    If Padre <> "" Then
                        StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Trim(Padre)) & "' ")
                    End If

                    If Figlio <> "" Then
                        StrSQL.Append(" AND GerarchiaImprese.Figlio = '" & Agro_SQL_SaveText(Trim(Figlio)) & "' ")
                    End If

                    If Foglia <> 0 Then
                        StrSQL.Append(" AND GerarchiaImprese.Foglia = " & Agro_SQL_SaveNum(Foglia) & " ")
                    End If

                    If Livello <> 0 Then
                        StrSQL.Append(" AND GerarchiaImprese.Livello = " & Agro_SQL_SaveNum(Livello) & " ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GerarchiaImprese.Padre, GerarchiaImprese.Figlio ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


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

    '##############################################################################################
    Public Function LeggixFiglio(
                            ByVal Figlio As String,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.LeggixFiglio()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT GerarchiaImprese.* ")
                    StrSQL.AppendLine(" FROM  GerarchiaImprese ")
                    StrSQL.AppendLine(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
                    'StrSQL.Append(" WHERE GerarchiaImprese.Foglia = 0 ")
                    StrSQL.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" and GerarchiaImprese.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   GerarchiaImprese.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Figlio <> "" Then
                        StrSQL.AppendLine(" AND GerarchiaImprese.Figlio = '" & Agro_SQL_SaveText(Figlio) & "'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   GerarchiaImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   GerarchiaImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY GerarchiaImprese.Padre ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT GerarchiaImprese.* ")
                    StrSQL.AppendLine(" FROM  GerarchiaImprese ")
                    StrSQL.AppendLine(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
                    'StrSQL.Append(" WHERE GerarchiaImprese.Foglia = 0 ")
                    StrSQL.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" and GerarchiaImprese.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine(" AND   GerarchiaImprese.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Figlio <> "" Then
                        StrSQL.AppendLine(" AND GerarchiaImprese.Figlio = '" & Agro_SQL_SaveText(Figlio) & "'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   GerarchiaImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   GerarchiaImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY GerarchiaImprese.Padre ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


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



    '####################################################################################
    Public Function Ricava_Stringa_PivePadre(ByVal Figlio As String,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As String

        COnst NomeRoutine = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Ricava_Stringa_PivePadre()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim Stringa_Pive As String = ""
        Dim Piva As String
        Dim i As Integer

        Try

            DT = LeggixFiglio(Figlio,
                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                              xFiltroAggiuntivo, "",
                              objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For i = 0 To DT.Rows.Count - 1
                    Piva = DT.Rows(i).Item("Padre")
                    Stringa_Pive &= "'" & Agro_SQL_SaveText(Piva) & "',"
                Next

                If Stringa_Pive.Length > 0 Then
                    Stringa_Pive = "(" & Left(Stringa_Pive, Stringa_Pive.Length - 1) & ")"
                End If

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Stringa_Pive

    End Function

    Public Function Ricava_Stringa_PivePadre_Ricorsivo(ByVal Figlio As String,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As String

        Const NomeRoutine = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Ricava_Stringa_PivePadre_Ricorsivo()"

        Dim MessaggioErrore As String = ""
        Dim Stringa_Pive As String = ""
        Dim strRicorsiva As String = ""

        Try

            Dim DT As DataTable = LeggixFiglio(Figlio,
                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                               xFiltroAggiuntivo, "",
                                               objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For Each dr As DataRow In DT.Rows
                    If dr.Item("Padre") <> "" Then

                        Stringa_Pive &= "'" & dr.Item("Padre") & "',"

                        strRicorsiva = Ricava_Stringa_PivePadre_Ricorsivo(dr.Item("Padre"), "", objParametri)
                        If Trim(strRicorsiva) <> "" Then
                            Stringa_Pive &= strRicorsiva & ","
                        End If

                    End If


                Next

                Stringa_Pive = Stringa_Pive.TrimEnd(",")
                'If Stringa_Pive.Length > 0 Then
                '    Stringa_Pive = Left(Stringa_Pive, Stringa_Pive.Length - 1)
                'End If

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Stringa_Pive

    End Function

    '####################################################################################
    Public Function Ricava_HT_PivePadre(ByVal Figlio As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Hashtable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Ricava_HT_PivePadre()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim Piva As String
        Dim i As Integer
        Dim HT_Padri As New Hashtable

        Try

            DT = LeggixFiglio(Figlio,
                              enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                              xFiltroAggiuntivo, "",
                              objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For i = 0 To DT.Rows.Count - 1
                    Piva = DT.Rows(i).Item("Padre")
                    If Not HT_Padri.ContainsKey(Piva) Then
                        HT_Padri.Add(Piva, "")
                    End If
                Next

            End If

        Catch ex As Exception
            HT_Padri = Nothing
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return HT_Padri

    End Function





    '####################################################################################
    Public Function LeggixPermessi(
                                ByVal Padre As String,
                                ByVal FlagFoglia_1_SiFOGLIA_2_NoFOGLIA As Long,
                                    ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.LeggixPermessi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT GerarchiaImprese.*, Imprese.Rag_Soc ")
                    StrSQL.Append(" FROM  GerarchiaImprese , Imprese ")
                    StrSQL.Append(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
                    'StrSQL.Append(" WHERE GerarchiaImprese.Foglia = 0 ")
                    StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    StrSQL.Append(" AND GerarchiaImprese.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   GerarchiaImprese.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Imprese.Piva = GerarchiaImprese.Figlio ")

                    Select Case FlagFoglia_1_SiFOGLIA_2_NoFOGLIA
                        Case 1
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 1 ")
                        Case 2
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 0 ")
                    End Select

                    If Padre <> "" Then
                        StrSQL.Append(" AND Padre = '" & Agro_SQL_SaveText(Padre) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GerarchiaImprese.Padre ASC")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT GerarchiaImprese.*, Imprese.Rag_Soc ")
                    StrSQL.Append(" FROM  GerarchiaImprese , Imprese ")
                    StrSQL.Append(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
                    'StrSQL.Append(" WHERE GerarchiaImprese.Foglia = 0 ")
                    StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    StrSQL.Append(" AND GerarchiaImprese.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   GerarchiaImprese.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Imprese.Piva = GerarchiaImprese.Figlio ")

                    Select Case FlagFoglia_1_SiFOGLIA_2_NoFOGLIA
                        Case 1
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 1 ")
                        Case 2
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 0 ")
                    End Select

                    If Padre <> "" Then
                        StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Padre) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GerarchiaImprese.Padre ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT GerarchiaImprese.*, Imprese.rag_soc ")
                    StrSQL.Append(" FROM GerarchiaImprese ")
                    StrSQL.Append(" INNER JOIN Imprese ON GerarchiaImprese.Figlio = Imprese.PIVA ")
                    StrSQL.Append(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
                    'StrSQL.Append(" WHERE GerarchiaImprese.Foglia = 0 ")
                    StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


                    '''''''''''''''''''''''''''''''''''''''''''''''''
                    Select Case FlagFoglia_1_SiFOGLIA_2_NoFOGLIA
                        Case 1
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 1 ")
                        Case 2
                            StrSQL.Append(" AND GerarchiaImprese.Foglia = 0 ")
                    End Select


                    If Padre <> "" Then
                        StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Padre) & "'")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY GerarchiaImprese.Padre ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta


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


    '###############################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Numero_Cooperative(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim Num As Integer = 0
        Dim Dt As DataTable

        Dt = Leggi("", 2, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If Dt IsNot Nothing AndAlso Dt.Rows.Count > 0 Then
            Num = Dt.Rows.Count
        End If

        Return Num

    End Function

    '#########################################################################################
    Public Function CaricaBoxList_Cooperativa(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.CaricaBoxList_Cooperativa()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT GerarchiaImprese.*, Imprese.rag_soc ")
            StrSQL.AppendLine(" FROM GerarchiaImprese ")
            StrSQL.AppendLine(" INNER JOIN Imprese ON GerarchiaImprese.Figlio = Imprese.PIVA ")
            StrSQL.AppendLine(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA ")
            StrSQL.AppendLine(" WHERE GerarchiaImprese.Foglia = 0 ")
            StrSQL.AppendLine(" AND UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" ORDER BY GerarchiaImprese.Padre ")



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

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Nodo"></param>
    ''' <param name="Elenco_Imprese"></param>
    ''' <param name="objParametri"></param>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Sub LeggiFigliNodoGerarchiaImprese(ByVal Nodo As String,
                                              ByRef Elenco_Imprese As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'Dato un nodo legge tutti i suoi figli e ricorsivamente i figli dei figli etc.

        Dim DT As DataTable

        DT = Leggi(CStr(Nodo), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            Dim i As Integer
            For i = 0 To DT.Rows.Count - 1

                'Aggiungo all'elenco il nodo corrente
                If Elenco_Imprese = "" Then
                    Elenco_Imprese = "'" & DT.Rows(i).Item("Figlio") & "'"
                Else
                    Elenco_Imprese = Elenco_Imprese & " , '" & DT.Rows(i).Item("Figlio") & "'"
                End If

                If DT.Rows(i).Item("Foglia") = 0 Then
                    'Non è una foglia -> Nuova Ricorsione
                    LeggiFigliNodoGerarchiaImprese(DT.Rows(i).Item("Figlio"), Elenco_Imprese, objParametri)
                End If

            Next

        End If

    End Sub

    Public Sub LeggiFigliNodoGerarchiaImprese_conDettagli(ByVal Piva As String,
                                                               ByVal InserisciAziende_0tutte_1soloCoop As String,
                                                               ByRef DtFigli As DataTable,
                                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'Dato un nodo legge tutti i suoi figli e ricorsivamente i figli dei figli etc.
        Dim DT As DataTable

        If DtFigli Is Nothing Then
            DtFigli = New DataTable
            DtFigli.Columns.Add(New DataColumn("piva", GetType(String)))
            DtFigli.Columns.Add(New DataColumn("CodiceCuaa", GetType(String)))
            DtFigli.Columns.Add(New DataColumn("Rag_Soc", GetType(String)))
            DtFigli.Columns.Add(New DataColumn("CodiceSocio", GetType(String)))
            DtFigli.Columns.Add(New DataColumn("provincia", GetType(String)))
            DtFigli.Columns.Add(New DataColumn("TipoImpresaGerarchia", GetType(Integer)))
        End If

        DT = Leggi(CStr(Piva), 0, enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            For i = 0 To DT.Rows.Count - 1

                If InserisciAziende_0tutte_1soloCoop = 0 Or (InserisciAziende_0tutte_1soloCoop = 1 And DT.Rows(i).Item("TipoImpresaGerarchia") <> 1) Then
                    Dim newRow = DtFigli.NewRow
                    newRow.Item("piva") = DT.Rows(i).Item("piva")
                    newRow.Item("CodiceCuaa") = DT.Rows(i).Item("CodiceCuaa")
                    newRow.Item("Rag_Soc") = DT.Rows(i).Item("Rag_Soc")
                    newRow.Item("CodiceSocio") = DT.Rows(i).Item("CodiceSocio")
                    newRow.Item("provincia") = DT.Rows(i).Item("PROVINCIA")
                    newRow.Item("TipoImpresaGerarchia") = DT.Rows(i).Item("TipoImpresaGerarchia")
                    DtFigli.Rows.Add(newRow)
                End If

                If DT.Rows(i).Item("Foglia") = 0 Then
                    'Non è una foglia -> Nuova Ricorsione
                    LeggiFigliNodoGerarchiaImprese_conDettagli(DT.Rows(i).Item("Figlio"), InserisciAziende_0tutte_1soloCoop, DtFigli, objParametri)
                End If

            Next

        End If

    End Sub

    Function LeggiPadre(piva As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef Rag_Soc_Padre As String) As String
        Dim dt As DataTable = Leggi("", 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    " Figlio = '" & Agro_SQL_SaveText(piva) & "' ", "", objParametri)
        If dt.Rows.Count > 0 Then
            Dim padre As String = dt.Rows(0).Item("Padre")
            Rag_Soc_Padre = New AgronicaCoreAnagrafeDAL.Imprese_Read().RagSoc_from_Piva(padre, objParametri)
            Return padre
        End If
        Return ""

    End Function

    Function LeggiImpreseReferenti(piva As String, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, Optional ByRef PivaReferente As String = "") As String

        Dim Referente As String = ""
        Dim dtleggiCoopReferente As DataTable = LeggiPadriGerarchia("", piva, 0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri)
        Dim txtCoopRef As New List(Of String)
        Dim txtPivaRef As New List(Of String)

        For Each drCoop As DataRow In dtleggiCoopReferente.Rows
            txtCoopRef.Add(drCoop("RagSoc_Padre"))
            txtPivaRef.Add(drCoop("Padre"))
        Next

        If txtCoopRef.Count > 0 Then
            Referente = String.Join(",", txtCoopRef)
            PivaReferente = String.Join(",", txtPivaRef)
        End If

        Return Referente

    End Function

    Function LeggiPrimaPivaPadre(piva_figlio As String,
                                 objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim piva_padre As String = ""
        Dim dt As DataTable

        dt = Leggi2("",
                    piva_figlio,
                   0,
                   "", "", objParametri)

        If dt.Rows.Count > 0 Then
            piva_padre = dt.Rows(0).Item("Padre")
        End If

        Return piva_padre

    End Function



    Function HaQuestaPivaPadre(ByVal piva_figlio As String,
                                ByVal piva_padre As String,
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim dt As DataTable
        Dim HaQuestoPadre As Boolean = False

        dt = Leggi2(piva_padre,
                    piva_figlio,
                   0,
                   "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            HaQuestoPadre = True
        End If

        Return HaQuestoPadre

    End Function

    Public Function GerarchiaImprese_Leggi(ByVal Padre As String,
                                           ByVal Figlio As String,
                                           ByVal Foglia As Integer,
                                           ByVal Livello As Integer,
                                           ByVal FinestraTemp_Inizio As Date,
                                           ByVal FinestraTemp_Fine As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As DataTable




        'ByVal FinestraTemp_Inizio As Date = #1/1/1900#, _
        'ByVal FinestraTemp_Fine As Date = #12/31/2100#, _


        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT GerarchiaImprese.*, Imprese.Rag_Soc AS RagSoc_Figlio ")
            StrSQL.Append(" FROM  GerarchiaImprese ")
            StrSQL.Append(" INNER JOIN Imprese ON GerarchiaImprese.Figlio = Imprese.PIVA ")

            StrSQL.Append(" WHERE   (GerarchiaImprese.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & ") ")
            StrSQL.Append(" AND     (GerarchiaImprese.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & ") ")

            '----- Condizioni

            If Padre <> "" Then
                StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Trim(Padre)) & "' ")
            End If

            If Figlio <> "" Then
                StrSQL.Append(" AND GerarchiaImprese.Figlio = '" & Agro_SQL_SaveText(Trim(Figlio)) & "' ")
            End If

            If Foglia <> 0 Then
                StrSQL.Append(" AND GerarchiaImprese.Foglia = " & Agro_SQL_SaveNum(Foglia) & " ")
            End If

            If Livello <> 0 Then
                StrSQL.Append(" AND GerarchiaImprese.Livello = " & Agro_SQL_SaveNum(Livello) & " ")
            End If




            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Padre, Figlio ")
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

    Public Function LeggiPadriGerarchia(ByVal Padre As String,
                                       ByVal Figlio As String,
                                       ByVal Foglia As Integer,
                                       ByVal Livello As Integer,
                                       ByVal FinestraTemp_Inizio As Date,
                                       ByVal FinestraTemp_Fine As Date,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT  GerarchiaImprese.padre, Imprese_Padri.Rag_Soc AS RagSoc_Padre ")
            StrSQL.AppendLine(" FROM  GerarchiaImprese ")
            StrSQL.AppendLine(" LEFT OUTER JOIN Imprese Imprese_Padri ON GerarchiaImprese.Padre = Imprese_Padri.PIVA ")
            StrSQL.AppendLine(" INNER JOIN UtentiXImprese ON Imprese_Padri.PIVA = UtentiXImprese.PIVA  ")

            StrSQL.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.AppendLine(" AND   GerarchiaImprese.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.AppendLine(" AND   GerarchiaImprese.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            '----- Condizioni

            If Padre <> "" Then
                StrSQL.AppendLine(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Trim(Padre)) & "' ")
            End If

            If Figlio <> "" Then
                StrSQL.AppendLine(" AND GerarchiaImprese.Figlio = '" & Agro_SQL_SaveText(Trim(Figlio)) & "' ")
            End If

            If Foglia <> 0 Then
                StrSQL.AppendLine(" AND GerarchiaImprese.Foglia = " & Agro_SQL_SaveNum(Foglia) & " ")
            End If

            If Livello <> 0 Then
                StrSQL.AppendLine(" AND GerarchiaImprese.Livello = " & Agro_SQL_SaveNum(Livello) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   GerarchiaImprese.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   GerarchiaImprese.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY RagSoc_Padre ")
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

    Public Function LeggiPadriGerarchia_AncheVuoti(ByVal Padre As String,
                                       ByVal Figlio As String,
                                       ByVal Foglia As Integer,
                                       ByVal Livello As Integer,
                                       ByVal FinestraTemp_Inizio As Date,
                                       ByVal FinestraTemp_Fine As Date,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" SELECT DISTINCT  GerarchiaImprese.padre, ISNULL(Imprese_Padri.Rag_Soc, '') AS RagSoc_Padre, ")
            StrSQL.Append(" ISNULL(LibroSoci_Codice, '') AS LibroSoci_Codice, ISNULL(LibroSoci_DataIscrizione, '') AS LibroSoci_DataIscrizione ")
            StrSQL.Append(" FROM  GerarchiaImprese ")
            StrSQL.Append(" LEFT OUTER JOIN Imprese Imprese_Padri ON GerarchiaImprese.Padre = Imprese_Padri.PIVA ")
            StrSQL.Append(" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA  ")

            StrSQL.Append(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            StrSQL.Append(" AND   GerarchiaImprese.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.Append(" AND   GerarchiaImprese.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            '----- Condizioni

            If Padre <> "" Then
                StrSQL.Append(" AND GerarchiaImprese.Padre = '" & Agro_SQL_SaveText(Trim(Padre)) & "' ")
            End If

            If Figlio <> "" Then
                StrSQL.Append(" AND GerarchiaImprese.Figlio = '" & Agro_SQL_SaveText(Trim(Figlio)) & "' ")
            End If

            If Foglia <> 0 Then
                StrSQL.Append(" AND GerarchiaImprese.Foglia = " & Agro_SQL_SaveNum(Foglia) & " ")
            End If

            If Livello <> 0 Then
                StrSQL.Append(" AND GerarchiaImprese.Livello = " & Agro_SQL_SaveNum(Livello) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY RagSoc_Padre ")
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

    Public Function LeggiPadriRicorsiva(
        ByVal Figlio As String,
        ByVal Foglia As Integer,
        ByVal Livello As Integer,
        ByVal FinestraTemp_Inizio As Date,
        ByVal FinestraTemp_Fine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional ByVal mostraVuoti As Boolean = False,
        Optional ByVal livelloStop As Integer = 0
    ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append($" WITH Antenati AS ( {vbCrLf}")
            StrSQL.Append($" SELECT Figlio, Padre, Livello FROM GerarchiaImprese ")
            StrSQL.Append($" INNER JOIN UtentiXImprese ON GerarchiaImprese.Figlio = UtentiXImprese.PIVA  ")

            '----- Condizioni
            StrSQL.Append($" WHERE UtentiXImprese.[USER] = '{Agro_SQL_SaveText(objParametri.PivaSuperUser)}' ")
            If Figlio <> "" Then
                StrSQL.Append($" AND Figlio = '{Agro_SQL_SaveText(Figlio)}' ")
            End If
            If Livello <> 0 Then
                StrSQL.Append($" AND Livello = {Agro_SQL_SaveNum(Livello)} ")
            End If
            If Foglia <> 0 Then
                StrSQL.Append($" AND GerarchiaImprese.Foglia = {Agro_SQL_SaveNum(Foglia)} ")
            End If
            StrSQL.Append($" AND   GerarchiaImprese.Validita_inizio <= {Agro_SQL_SaveDate(FinestraTemp_Fine)} ")
            StrSQL.Append($" AND   GerarchiaImprese.Validita_Fine >= {Agro_SQL_SaveDate(FinestraTemp_Inizio)} ")

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   GerarchiaImprese.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   GerarchiaImprese.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append($" AND {Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri)}")
            End If
            StrSQL.Append($"{vbCrLf} UNION ALL {vbCrLf}")
            StrSQL.Append($" SELECT g.Figlio, g.Padre, g.Livello FROM GerarchiaImprese g JOIN Antenati a ON a.Padre = g.Figlio ")
            If Not mostraVuoti Then
                StrSQL.Append(" AND g.Padre != '' ")
            End If
            If livelloStop <> 0 Then
                StrSQL.Append($" AND Livello >= {Agro_SQL_SaveNum(livelloStop)} ")
            End If
            StrSQL.Append($"{vbCrLf} ) {vbCrLf}")
            StrSQL.Append($" SELECT Padre, Figlio, Livello  FROM Antenati ")


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

    Public Function LeggiDifferenzaGerarchiaImprese(
        ByVal filtroInsieme1 As String,
        ByVal filtroInsieme2 As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.ComparaInsiemiVisibilita()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" (SELECT DISTINCT a.Padre, a.rag_soc, a.PIVA, a.Foglia, a.Validazione, a.TipoImpresaGerarchia, a.Blk_Flag, a.validita_fine FROM (")
            StrSQL.AppendLine("   SELECT  GerarchiaImprese.Padre, Imprese.rag_soc, Imprese.PIVA, GerarchiaImprese.Foglia, Imprese.Validazione, Imprese.TipoImpresaGerarchia , Imprese.Blk_Flag, Imprese.validita_fine")
            StrSQL.AppendLine("   FROM GerarchiaImprese  (NOLOCK)")
            StrSQL.AppendLine("   INNER JOIN Imprese  (NOLOCK)  ON GerarchiaImprese.Figlio = Imprese.PIVA")
            StrSQL.AppendLine("   WHERE 1=1")
            If filtroInsieme1 <> "" Then
                StrSQL.AppendLine(filtroInsieme1)
            End If
            StrSQL.AppendLine(" ) as a)")
            StrSQL.AppendLine(" EXCEPT")
            StrSQL.AppendLine(" (SELECT DISTINCT b.Padre, b.rag_soc, b.PIVA, b.Foglia, b.Validazione, b.TipoImpresaGerarchia, b.Blk_Flag, b.validita_fine FROM (")
            StrSQL.AppendLine("   SELECT  GerarchiaImprese.Padre, Imprese.rag_soc, Imprese.PIVA, GerarchiaImprese.Foglia, Imprese.Validazione, Imprese.TipoImpresaGerarchia , Imprese.Blk_Flag, Imprese.validita_fine")
            StrSQL.AppendLine("   FROM GerarchiaImprese  (NOLOCK)")
            StrSQL.AppendLine("   INNER JOIN Imprese  (NOLOCK)  ON GerarchiaImprese.Figlio = Imprese.PIVA")
            StrSQL.AppendLine("   WHERE 1=1")
            If filtroInsieme2 <> "" Then
                StrSQL.AppendLine(filtroInsieme2)
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function LeggiTipologieGerarchiaImprese(ByVal Id_Servizio As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.LeggiTipologieGerarchiaImprese()"
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT a.Piva, b.TipologiaGerarchie_Cod, b.TipologiaGerarchie_Des, ")
            StrSQL.AppendLine("   c.TipologiaGerarchie_Dettagli_Livello_Cod, c.TipologiaGerarchie_Dettagli_Livello_Des, c.TipologiaGerarchie_Dettagli_Livello, ")
            StrSQL.AppendLine("   d.Gruppi_Utente_Cod, d.Id_Servizio, d.Flag_Amministrazione, d.Flag_Inserimento, d.Flag_Modifica, d.Flag_Cancellazione, d.Flag_Informazioni ")
            StrSQL.AppendLine(" FROM GerarchiaImprese_Tipologia_NodiApplicati a ")
            StrSQL.AppendLine("   INNER JOIN GerarchiaImprese_Tipologia b ON a.TipologiaGerarchie_Cod = b.TipologiaGerarchie_Cod ")
            StrSQL.AppendLine("   INNER JOIN GerarchiaImprese_Tipologia_Dettagli c ON a.TipologiaGerarchie_Cod = c.TipologiaGerarchie_Cod ")
            StrSQL.AppendLine("   INNER JOIN GerarchiaImprese_Tipologia_Dettagli_XGruppi_Utente d ON c.TipologiaGerarchie_Dettagli_Livello_Cod = d.TipologiaGerarchie_Dettagli_Livello_Cod  ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            If Id_Servizio <> 0 Then
                StrSQL.Append(" AND Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiPadriGerarchiaImprese(ByVal Gruppo_Utente_Cod As Integer, ByVal Tipologia_Cod As Integer, ByVal Id_Servizio As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.LeggiTipologieGerarchiaImprese()"
        Dim StrSQL As New Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT DISTINCT a.Piva, a.TipologiaGerarchie_Cod, d.Gruppi_Utente_Cod ")
            StrSQL.AppendLine(" FROM GerarchiaImprese_Tipologia_NodiApplicati a ")
            StrSQL.AppendLine("   INNER JOIN GerarchiaImprese_Tipologia b ON a.TipologiaGerarchie_Cod = b.TipologiaGerarchie_Cod ")
            StrSQL.AppendLine("   INNER JOIN GerarchiaImprese_Tipologia_Dettagli c ON a.TipologiaGerarchie_Cod = c.TipologiaGerarchie_Cod ")
            StrSQL.AppendLine("   INNER JOIN GerarchiaImprese_Tipologia_Dettagli_XGruppi_Utente d ON c.TipologiaGerarchie_Dettagli_Livello_Cod = d.TipologiaGerarchie_Dettagli_Livello_Cod  ")
            StrSQL.AppendLine(" WHERE 1=1 ")

            If Gruppo_Utente_Cod <> 0 Then
                StrSQL.Append(" AND Gruppi_Utente_Cod = " & Agro_SQL_SaveNum(Gruppo_Utente_Cod) & " ")
            End If

            If Tipologia_Cod <> 0 Then
                StrSQL.Append(" AND a.TipologiaGerarchie_Cod = " & Agro_SQL_SaveNum(Tipologia_Cod) & " ")
            End If

            If Id_Servizio <> 0 Then
                StrSQL.Append(" AND Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiGerarchiaDaCapostipiti(
        ByVal capostipiti As IEnumerable(Of String),
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional selezioneVariabile As enumSelezioneVariabile = enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
        Optional leggiSoloFigli As Boolean = False
    ) As DataTable
        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.LeggiGerarchiaDaCapostipiti()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable
        Dim xin = capostipiti.Select(Function(piva) "'" & piva & "'").
            DefaultIfEmpty().
            Aggregate(Function(p1, p2) p1 & "," & p2)
        Try
            StrSQL.AppendLine(" WITH imprese_figlie AS (  ")
            StrSQL.AppendLine("   SELECT padre, figlio, foglia, livello, cast(livello as int) as depth FROM GerarchiaImprese ")
            StrSQL.AppendLine("   WHERE 1=1 ")
            If Not String.IsNullOrEmpty(xin) Then
                StrSQL.AppendLine("     AND figlio in ( " & xin & " ) ")
            End If
            StrSQL.AppendLine("   UNION ALL ")
            StrSQL.AppendLine("   SELECT f.padre, f.figlio, f.foglia, f.livello, p.depth +1 as depth FROM GerarchiaImprese f ")
            StrSQL.AppendLine("   INNER JOIN imprese_figlie p ON f.Padre = p.Figlio ")
            StrSQL.AppendLine("   WHERE f.livello = p.depth + 1 ")
            StrSQL.AppendLine(" ) ")

            Select Case selezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    StrSQL.AppendLine(" SELECT gerarchia.* ")
                    StrSQL.AppendLine(" FROM (SELECT DISTINCT * FROM imprese_figlie) gerarchia ")

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.AppendLine(" SELECT gerarchia.*, imprese.rag_soc ")
                    StrSQL.AppendLine(" FROM (SELECT DISTINCT * FROM imprese_figlie) gerarchia ")
                    StrSQL.AppendLine(" LEFT JOIN imprese ON imprese.piva = gerarchia.Figlio ")

                Case Else
                    StrSQL.AppendLine(" SELECT gerarchia.*, imprese.*, imprese_codici.val_cod as cuaa ")
                    StrSQL.AppendLine(" FROM (SELECT DISTINCT * FROM imprese_figlie) gerarchia ")
                    StrSQL.AppendLine(" LEFT JOIN imprese ON imprese.piva = gerarchia.Figlio ")
                    StrSQL.AppendLine(" LEFT JOIN imprese_codici ON imprese_codici.piva = gerarchia.Figlio AND imprese_codici.id_cod = 1010 ")

            End Select

            If leggiSoloFigli Then
                StrSQL.AppendLine(" WHERE gerarchia.figlio NOT IN ( " & xin & " ) ")
            End If
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    Public Function FilterBusinessWithSons(
        business As IEnumerable(Of String),
        objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As IList(Of String)
        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_R.FilterBusinessWithSons()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim xin = business.Select(Function(piva) "'" & piva & "'").
            DefaultIfEmpty().
            Aggregate(Function(p1, p2) p1 & "," & p2)
        Try
            StrSQL.AppendLine(" SELECT DISTINCT Padre  ")
            StrSQL.AppendLine(" FROM GerarchiaImprese ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            If Not String.IsNullOrEmpty(xin) Then
                StrSQL.AppendLine("     AND Padre in ( " & xin & " ) ")
            End If
            '--------------------------------------------------------------------------
            Dim table = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            Return table.Select.AsParallel.
                Select(Function(row) CStr(row("Padre"))).
                ToList
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return New List(Of String)()
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class GerarchiaImprese_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(
                            ByVal Padre As String,
                            ByVal Figlio As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    , ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            , Optional ByVal Data_creazione As Date = #2/1/1900# _
                            , Optional ByVal Data_modifica As Date = #2/1/1900# _
                            , Optional ByVal username_creazione As String = "" _
                            , Optional ByVal username_modifica As String = ""
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim DT As DataTable
        Dim Foglia As Int32 = 1
        Dim Livello As Int32 = 1

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri_server.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri_server.UsernameOperazione
            End If

            '------------------------------
            'Verifica Livello

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  ISNULL(Max(Livello), 0) as LivelloPrecedente ")
            StrSQL.Append(" FROM    GerarchiaImprese")
            StrSQL.Append(" WHERE   Figlio = '" & Agro_SQL_SaveText(Padre) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Livello = 1 + CInt(DT.Rows(0).Item("LivelloPrecedente"))
            End If

            DT = Nothing

            '------------------------------
            'Verifica Foglia

            StrSQL.Length = 0
            StrSQL.Append(" SELECT  COUNT(*) AS NR ")
            StrSQL.Append(" FROM    GerarchiaImprese")
            StrSQL.Append(" WHERE   Padre = '" & Agro_SQL_SaveText(Figlio) & "'")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                If CInt(DT.Rows(0).Item(0)) > 0 Then
                    Foglia = 0
                End If
            End If

            DT = Nothing

            '---------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO GerarchiaImprese(Padre,Figlio,Foglia,Livello, ")
            StrSQL.Append("                             Inviato, DataInvio, ")
            StrSQL.Append("                             Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                             UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                             Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                             ) ")
            StrSQL.Append(" VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Padre) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Figlio) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Foglia) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Livello))
            StrSQL.Append("         , 0 ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------


            If xRisp Then

                'Segnalo che il nodo padre non è una foglia perché da questo momento ha un figlio

                '---------------------------------------------
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE  GerarchiaImprese SET  ")
                StrSQL.Append("          Foglia            =  0  ")
                StrSQL.Append("         ,Inviato           =  0 ")
                StrSQL.Append("         ,DataInvio         =  Null ")
                StrSQL.Append("         ,Data_Modifica     =  " & Agro_SQL_SaveDate(Now))
                StrSQL.Append("         ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri_server.UsernameOperazione) & "'")
                StrSQL.Append("         ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
                StrSQL.Append("         ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
                StrSQL.Append(" WHERE   Figlio = '" & Agro_SQL_SaveText(Padre) & "' ")
                '---------------------------------------------

                '--------------------------------------------------------------------------
                xRisp = EseguiQuery_Scrittura(objParametri_server, StrSQL.ToString, NomeRoutine)
                '--------------------------------------------------------------------------

                AggiornaUtentiProfili(objParametri_server, objParametri_utenti, Padre, Figlio)

            Else

                Throw New Exception("[" & NomeRoutine & "] : " &
                "Fallito inserimento in GerarchiaImprese " &
                "(Padre=" & Padre & ")(Figlio=" & Figlio & ") ")

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function



    Public Function Modifica(
                            ByVal Padre As String,
                            ByVal Figlio As String,
                            ByVal Foglia As Long,
                              ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_W.Modifica()"

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
            StrSQL.Append("UPDATE GerarchiaImprese SET ")
            StrSQL.Append("    Foglia            =  " & Agro_SQL_SaveNum(Foglia))
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri_server.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE Inviato >=0 ")
            StrSQL.Append(" AND Padre='" & Agro_SQL_SaveText(Padre) & "'")
            StrSQL.Append(" AND Figlio='" & Agro_SQL_SaveText(Figlio) & "'")



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If xRisp Then
                AggiornaUtentiProfili(objParametri_server, objParametri_utenti, Padre, Figlio)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    ' modifica il padre di un'impresa, ma si devono passare padre vecchio, figlio e padre nuovo
    Public Function ModificaPadre(
                            ByVal PadreNuovo As String,
                            ByVal Figlio As String,
                            ByVal PadreVecchio As String,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        If PadreNuovo = "" Then
            Throw New Exception("[" & NomeRoutine & "] : Il padre nuovo è obbligatorio")
        End If

        If PadreVecchio = "" Then
            Throw New Exception("[" & NomeRoutine & "] : Il padre vecchio è obbligatorio")
        End If

        If Figlio = "" Then
            Throw New Exception("[" & NomeRoutine & "] : Il figlio è obbligatorio")
        End If

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" UPDATE GerarchiaImprese SET ")
            StrSQL.Append(" Padre = '" & Agro_SQL_SaveText(PadreNuovo) & "' ")
            StrSQL.Append(" WHERE 1=1 ")
            StrSQL.Append(" AND Padre='" & Agro_SQL_SaveText(PadreVecchio) & "'")
            StrSQL.Append(" AND Figlio='" & Agro_SQL_SaveText(Figlio) & "'")


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If xRisp Then
                AggiornaUtentiProfili(objParametri_server, objParametri_utenti, PadreNuovo, Figlio)
                AggiornaUtentiProfili(objParametri_server, objParametri_utenti, PadreVecchio, Figlio)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    '#########################################################################################
    Public Function Cancella(
                                ByVal Padre As String,
                                ByVal Figlio As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri_server.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE GerarchiaImprese ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & objParametri_server.UsernameOperazione & "' ")
                StrSQL.Append("        ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato > 0 ")

                If Padre <> "" Then
                    StrSQL.Append(" AND Padre = '" & Agro_SQL_SaveText(Padre) & "'")
                End If

                If Figlio <> "" Then
                    StrSQL.Append(" AND Figlio = '" & Agro_SQL_SaveText(Figlio) & "'")
                End If

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM GerarchiaImprese ")
                StrSQL.Append(" WHERE 1=1 ")

                If Padre <> "" Then
                    StrSQL.Append(" AND Padre = '" & Agro_SQL_SaveText(Padre) & "'")
                End If

                If Figlio <> "" Then
                    StrSQL.Append(" AND Figlio = '" & Agro_SQL_SaveText(Figlio) & "'")
                End If

            End If
            '---------------------------------------------


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_server))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If xRisp Then
                AggiornaUtentiProfili(objParametri_server, objParametri_utenti, Padre, Figlio)
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    ''' <summary>
    ''' Notifico la utenti_profili del cambiamento di modo che possano essere aggiornati gli utenti
    ''' </summary>
    ''' <param name="objParametri_server"></param>
    ''' <param name="objParametri_utenti"></param>
    ''' <returns></returns>
    Public Function AggiornaUtentiProfili(ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal PivaPadre As String,
                                          ByVal Piva As String) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.GerarchiaImprese_W.AggiornaUtentiProfili()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            Dim strFiltro As String = " (Descrizione_2 like '" & Agro_SQL_SaveText($"%GerarchiaImprese.Padre = '{PivaPadre}'%") & "' OR " &
                "Descrizione_2 like '" & Agro_SQL_SaveText($"%Imprese.Piva = '{Piva}'%") & "')"

            '---------------------------------------------
            StrSQL.AppendLine("UPDATE Utenti_Profili WITH (ROWLOCK) SET")
            StrSQL.AppendLine(" DataUltimoRiportoUtentiVisibilitaAppoggio = NULL")
            StrSQL.AppendLine("WHERE " & strFiltro & " ")
            StrSQL.AppendLine("AND DataUltimoRiportoUtentiVisibilitaAppoggio IS NOT NULL ")
            xRisp = EseguiQuery_Scrittura(objParametri_utenti, StrSQL.ToString, NomeRoutine)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            'Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
