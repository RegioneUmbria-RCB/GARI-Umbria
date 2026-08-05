Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class UtentixImprese_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixImprese_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '---------------------------------------------
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("         UtentiXImprese.[USER] AS PivaSuperUser,  ")
                    strSql.AppendLine("         Imprese.* ")

                    strSql.AppendLine(" FROM    UtentiXImprese INNER JOIN ")
                    strSql.AppendLine("         Imprese ON UtentiXImprese.PIVA = Imprese.PIVA ")

                    strSql.AppendLine(" WHERE   (UtentiXImprese.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (UtentiXImprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    strSql.AppendLine(" AND     (Imprese.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    strSql.AppendLine(" AND     (UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser).Trim & "')    ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND (Imprese.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            strSql.AppendLine(" AND not exists ( ")
                            strSql.AppendLine(" select 1 from g2g_Recode_Imprese rr where rr.From_Piva = Imprese.Piva and rr.FROM_SaCod = 0 ")
                            strSql.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            strSql.AppendLine("and exists ( ")
                            strSql.AppendLine(" select 1 from g2g_Recode_Imprese rr where rr.DataInvio < Imprese.Data_Modifica and rr.From_Piva = Imprese.Piva and rr.FROM_SaCod = 0 ")
                            strSql.AppendLine(" ) ")

                    End Select

                    '---------------------------------------------

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato >= 0)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato = -1)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese.Rag_Soc ASC ")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '---------------------------------------------
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("         UtentiXImprese.[USER] AS PivaSuperUser,  ")
                    strSql.AppendLine("         Imprese.* ")
                    strSql.AppendLine("         , UtentiXImprese.Validita_Inizio as UtentiXImprese_Validita_Inizio ")
                    strSql.AppendLine("         , UtentiXImprese.Validita_fine as UtentiXImprese_Validita_fine ")
                    strSql.AppendLine("         , UtentiXImprese.Data_Creazione as UtentiXImprese_Data_Creazione ")
                    strSql.AppendLine("         , UtentiXImprese.Data_Modifica as UtentiXImprese_Data_Modifica ")
                    strSql.AppendLine("         , UtentiXImprese.Username_Modifica as UtentiXImprese_Username_Modifica ")
                    strSql.AppendLine("         , UtentiXImprese.Username_Creazione as UtentiXImprese_Username_creazione ")

                    strSql.AppendLine("         , UtentiXImprese.Validazione as UtentiXImprese_Validazione ")
                    strSql.AppendLine("         , UtentiXImprese.Data_Validazione as UtentiXImprese_Data_Validazione ")
                    strSql.AppendLine("         , UtentiXImprese.UserName_Validazione as UtentiXImprese_UserName_Validazione ")

                    strSql.AppendLine(" FROM    UtentiXImprese INNER JOIN ")
                    strSql.AppendLine("         Imprese ON UtentiXImprese.PIVA = Imprese.PIVA ")

                    strSql.AppendLine(" WHERE   (UtentiXImprese.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (UtentiXImprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    strSql.AppendLine(" AND     (Imprese.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    strSql.AppendLine(" AND     (UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser).Trim & "')    ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND (Imprese.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            strSql.AppendLine(" AND not exists ( ")
                            strSql.AppendLine(" select 1 from g2g_Recode_Imprese rr where rr.From_Piva = Imprese.Piva and rr.FROM_SaCod = 0 ")
                            strSql.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            strSql.AppendLine("and exists ( ")
                            strSql.AppendLine(" select 1 from g2g_Recode_Imprese rr where rr.DataInvio < Imprese.Data_Modifica and rr.From_Piva = Imprese.Piva and rr.FROM_SaCod = 0 ")
                            strSql.AppendLine(" ) ")

                    End Select

                    '---------------------------------------------

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato >= 0)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato = -1)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese.Rag_Soc ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case enumSelezioneVariabile.Selezione_JoinCompleta



            End Select

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



    'Public Function FiltroneSuperNova( _
    '                            ByVal UtenteProfilo As String, _
    '                            ByVal Select_Code As Integer, _
    '                            ByVal xSelezioneVariabile As enumSelezioneVariabile, _
    '                            ByVal xFiltroAggiuntivo As String, _
    '                            ByVal xOrderBy As String, _
    '                            ByRef objParametri As AgronicaCoreParametri _
    '                            ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.UtentixImprese_Read.FiltroneSuperNova()"

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
    '        Select Case xSelezioneVariabile

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi



    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta



    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
    '                '
    '                '
    '                '
    '                '


    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

    '                StrSQL.Length = 0

    '                Select Case Select_Code

    '                    Case 0

    '                        'Nota: NON RIMUOVERE NULLA DALLA SELECT LIST!!!

    '                        StrSQL.Append(" SELECT Distinct ")
    '                        StrSQL.Append("        dbo.Imprese.PIVA, dbo.Imprese.rag_soc, dbo.Imprese.Sup_Totale, Imprese.Validita_Inizio as Impresa_Validita_Inizio, Imprese.Validita_Fine as Impresa_Validita_Fine,")
    '                        StrSQL.Append("        dbo.Centri_Aziendali.sa_cod, dbo.Centri_Aziendali.sa_nome, dbo.Centri_Aziendali.TitoloPossesso, Centri_Aziendali.Validita_Inizio as Centri_Aziendali_Validita_Inizio, Centri_Aziendali.Validita_Fine as Centri_Aziendali_Validita_Fine,")
    '                        StrSQL.Append("        Campi.Campo_Cod, Campi.Campo_Des,")
    '                        StrSQL.Append("        dbo.Appezzamento.APPEZZA, dbo.Appezzamento.SUP_APP, dbo.Appezzamento.APP_NOME, dbo.Appezzamento.CAMPO_COD, Appezzamento.Validita_Inizio as Appezzamento_Validita_Inizio, Appezzamento.Validita_Fine as Appezzamento_Validita_Fine,")
    '                        StrSQL.Append("        dbo.Reg_Impianti.CUL_COD, dbo.Reg_Impianti.GRVA_Cod_VEG, GruppoVarietale.GRVA_DES, dbo.Reg_Impianti.ID_REG, dbo.Reg_Impianti.P_HA, dbo.Reg_Impianti.Port_Cod, Reg_Impianti.Validita_Inizio as Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine as Reg_Impianti_Validita_Fine, dbo.Reg_Impianti.Sup_Imp, Reg_Impianti.Resa_Prevista, ")
    '                        StrSQL.Append("        dbo.Lista_Province.PROVINCIA,")
    '                        StrSQL.Append("        dbo.GruppoVegetale.GRU_DES, dbo.Istat.Cap, dbo.Istat.Localita,")
    '                        StrSQL.Append("        dbo.SpecieVegetali.VEG_COD, dbo.SpecieVegetali.VEG_DES, dbo.Cultivar.CUL_DES, Imprese.TipoImpresaGerarchia ,")
    '                        StrSQL.Append("        Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Des_Lib, Operazioni.Lav_Des, ")
    '                        StrSQL.Append("        Agenda.Validita_Inizio, Movimenti.Data_Movimento ")


    '                    Case 1 'Solo Imprese x Albero

    '                        StrSQL.Append(" SELECT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia ")

    '                    Case 2 'Imprese

    '                        'StrSQL.Append(" SELECT DISTINCT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine "
    '                        StrSQL.Append(" SELECT DISTINCT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine ")


    '                    Case 3 'Centri Aziendali

    '                        StrSQL.Append(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
    '                        StrSQL.Append(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine ")

    '                    Case 4 'Appezzamenti

    '                        StrSQL.Append(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
    '                        StrSQL.Append(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
    '                        StrSQL.Append(" Campi.Campo_Cod, ISNULL(Campi.Campo_Des,''),")
    '                        StrSQL.Append(" Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.APP_NOME, Appezzamento.Campo_Cod, Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine ")

    '                    Case 5 'Impianti

    '                        StrSQL.Append(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, Lista_Province.PROVINCIA, ")
    '                        StrSQL.Append(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, ")
    '                        StrSQL.Append(" SpecieVegetali.Veg_Cod, Reg_Impianti.Cul_Cod,  dbo.Reg_Impianti.GRVA_Cod_VEG, ")
    '                        StrSQL.Append(" SpecieVegetali.Veg_Des, Cultivar.Cul_Des, GruppoVarietale.GRVA_DES, ")
    '                        StrSQL.Append(" Campi.Campo_Cod, ISNULL(Campi.Campo_Des,''),")
    '                        StrSQL.Append(" Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.APP_NOME, ")
    '                        StrSQL.Append(" Reg_Impianti.ID_REG, Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine, ")
    '                        StrSQL.Append(" Reg_Impianti.Sup_Imp, Reg_Impianti.ID_Consociazione, Imprese_Progetti.Regolamento_Cod AS Regolamento, Imprese_Progetti.Disciplinare_Cod AS Finanziamento, Reg_Impianti.Grfi_Cod,  Reg_Impianti.Grva_Cod_Veg, Reg_Impianti.Cop_Cod, ")
    '                        StrSQL.Append(" Imprese_Progetti.Progetto_Cod, Imprese_Progetti.Progetto_Nome, ")
    '                        StrSQL.Append(" Imprese_Progetti.Validita_Inizio AS Progetto_Validita_Inizio, Imprese_Progetti.Validita_Fine AS Progetto_Validita_Fine ")

    '                    Case 6 'Movimenti

    '                        StrSQL.Append(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
    '                        StrSQL.Append(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
    '                        StrSQL.Append(" Campi.Campo_Cod, ISNULL(Campi.Campo_Des,''),")
    '                        StrSQL.Append(" Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.APP_NOME, ")
    '                        StrSQL.Append(" Reg_Impianti.ID_REG, Reg_Impianti.Sup_Imp, ")
    '                        StrSQL.Append(" Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Des_Lib, Agenda.Username_Creazione, Agenda.Username_Modifica, Agenda.Data_Creazione, Agenda.Data_Modifica, Agenda.Validita_Inizio, ")
    '                        StrSQL.Append(" Movimenti.Data_Movimento, Movimenti.Cau_Mov, Movimenti.Num_Protocollo, ")
    '                        StrSQL.Append(" SpecieVegetali.VEG_COD, SpecieVegetali.Veg_Des, Reg_Impianti.Cul_Cod, Cultivar.Cul_Des  ")

    '                    Case 7 'Contatti

    '                        StrSQL.Append(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
    '                        StrSQL.Append(" Campi.Campo_Cod, ISNULL(Campi.Campo_Des,''),")
    '                        StrSQL.Append(" Contatti.Cod_Contatto, Contatti.Rag_Soc as Contatto_Rag_Soc,")
    '                        StrSQL.Append(" Risorse_Umane.Cod_Rapporto, Risorse_Umane.Cod_RisUm, Risorse_Umane.Piva, Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, Risorse_Umane.Validita_Fine as Cessazione,")
    '                        StrSQL.Append(" Rapporti_Contabili.Rapporto_Des")

    '                    Case 8 'Campi

    '                        StrSQL.Append(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
    '                        StrSQL.Append(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
    '                        StrSQL.Append(" Campi.Campo_Cod, ISNULL(Campi.Campo_Des,''),")
    '                        StrSQL.Append(" Campi.Validita_Inizio, Campi.Validita_Fine ")

    '                End Select


    '                'Completo la query SQL
    '                StrSQL.Append(" From (((((((((((((((((((((((( ")
    '                StrSQL.Append(" Imprese INNER JOIN UtentiXImprese On Imprese.Piva = UtentixImprese.Piva)")
    '                StrSQL.Append("         INNER JOIN GerarchiaImprese ON Imprese.Piva = GerarchiaImprese.Figlio)")
    '                StrSQL.Append("         INNER JOIN ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA)")
    '                StrSQL.Append("         INNER JOIN Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo)")
    '                StrSQL.Append("         INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM)")
    '                StrSQL.Append("         INNER JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV)")
    '                StrSQL.Append("    LEFT OUTER JOIN Centri_Aziendali ON Imprese.Piva=Centri_Aziendali.Piva)")
    '                StrSQL.Append("    LEFT OUTER join CentrixIndirizzi ON (Centri_Aziendali.PIVA = CentrixIndirizzi.PIVA And Centri_Aziendali.Sa_Cod = CentrixIndirizzi.Sa_Cod)")
    '                StrSQL.Append("    LEFT OUTER JOIN Indirizzi As Indirizzi_Centro ON CentrixIndirizzi.cod_indirizzo = Indirizzi_Centro.cod_indirizzo)")
    '                StrSQL.Append("    LEFT OUTER JOIN ISTAT As Istat_Centro ON Indirizzi_Centro.pro_cod_istat = Istat_Centro.PROV AND Indirizzi_Centro.com_cod_istat = Istat_Centro.COM)")
    '                StrSQL.Append("    LEFT OUTER JOIN Lista_Province as Lista_Province_Centro ON Lista_Province_Centro.Sigla = Istat_Centro.COMUNI_PROV)")
    '                StrSQL.Append("    LEFT OUTER JOIN Appezzamento on (Centri_Aziendali.Piva = Appezzamento.Piva AND Centri_Aziendali.Sa_Cod = Appezzamento.Sa_Cod )")
    '                StrSQL.Append("    LEFT OUTER JOIN Reg_Impianti on (Appezzamento.Piva = Reg_Impianti.Piva AND Appezzamento.SA_Cod = Reg_Impianti.Sa_Cod AND Appezzamento.Appezza = Reg_Impianti.Appezza)")
    '                StrSQL.Append("    LEFT OUTER JOIN Imprese_Progetti on (Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)")
    '                StrSQL.Append("    LEFT OUTER JOIN Imprese_Codici on (Imprese_Codici.Piva = Imprese.Piva)")
    '                StrSQL.Append("    LEFT OUTER JOIN Centri_Aziendali_Codici on (Centri_Aziendali_Codici.Piva = Centri_Aziendali.Piva AND Centri_Aziendali_Codici.SA_Cod = Centri_Aziendali.Sa_Cod )")
    '                StrSQL.Append("    LEFT OUTER JOIN Appezzamento_Codici on (Appezzamento_Codici.Piva = Appezzamento.Piva AND Appezzamento_Codici.SA_Cod = Appezzamento.Sa_Cod AND Appezzamento_Codici.Appezza = Appezzamento.Appezza )")
    '                StrSQL.Append("    LEFT OUTER JOIN Reg_Impianti_Codici on (Reg_Impianti_Codici.Piva = Reg_Impianti.Piva AND Reg_Impianti_Codici.SA_Cod = Reg_Impianti.Sa_Cod AND Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)")
    '                StrSQL.Append("    LEFT OUTER JOIN Campi on (Campi.Piva = Appezzamento.Piva AND Campi.Sa_Cod = Appezzamento.Sa_Cod AND Campi.Campo_Cod = Appezzamento.Campo_Cod )")
    '                StrSQL.Append("    LEFT OUTER JOIN AppezzamentixParticelle on (AppezzamentixParticelle.Piva = Appezzamento.Piva AND AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod AND AppezzamentixParticelle.Appezza = Appezzamento.Appezza )")

    '                StrSQL.Append("    LEFT OUTER JOIN GruppoVarietale ON abs(Reg_Impianti.GRVA_Cod_VEG) = GruppoVarietale.GRVA_COD)")
    '                StrSQL.Append("    LEFT OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.CUL_COD)")
    '                StrSQL.Append("    LEFT OUTER JOIN SpecieVegetali ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD)")
    '                StrSQL.Append("    LEFT OUTER JOIN GruppoVegetale ON SpecieVegetali.GRU_COD = GruppoVegetale.GRU_COD)")
    '                StrSQL.Append("    LEFT OUTER JOIN Mov_Destinazioni ON Mov_Destinazioni.Piva = Reg_Impianti.Piva and Mov_Destinazioni.Sa_Cod = Reg_Impianti.Sa_Cod and Mov_Destinazioni.Appezza = Reg_Impianti.Appezza and Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg  And Mov_Destinazioni.Tipo_Destinazione = 0)")
    '                StrSQL.Append("    LEFT OUTER JOIN Agenda ON Agenda.Piva = Reg_Impianti.Piva and Agenda.Sa_Cod = Reg_Impianti.Sa_Cod and Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda)")
    '                StrSQL.Append("    LEFT OUTER JOIN Movimenti ON Agenda.Piva = Movimenti.Piva and Agenda.Sa_Cod = Movimenti.Sa_Cod and Agenda.Id_Agenda = Movimenti.Id_Agenda)")
    '                StrSQL.Append("    LEFT OUTER JOIN Mov_Dettaglio_Tecnico ON Mov_Dettaglio_Tecnico.Piva = Movimenti.Piva and Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti.Sa_Cod and Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti.Id_Agenda and Mov_Dettaglio_Tecnico.Id_Mov = Movimenti.Id_Mov)")
    '                StrSQL.Append("    LEFT OUTER JOIN Movimenti_Dettagli ON Movimenti_Dettagli.Piva = Movimenti.Piva and Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod and Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda and Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov)")
    '                StrSQL.Append("    LEFT OUTER JOIN Operazioni ON  Agenda.Lav_Cod = Operazioni.Lav_Cod) ")
    '                StrSQL.Append("    LEFT OUTER JOIN GruppoOperazioni ON  Operazioni.Gru_Op = GruppoOperazioni.Gru_Cod) ")
    '                StrSQL.Append("    LEFT OUTER JOIN Risorse_Umane ON ((Risorse_Umane.Cod_RisUm = Movimenti.Cod_RisUm) or (Risorse_Umane.Cod_RisUm = Movimenti_Dettagli.Mat_Cod and Movimenti_Dettagli.Elem_Cod = 0) ) )")
    '                StrSQL.Append("    LEFT OUTER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto)")
    '                StrSQL.Append("    LEFT OUTER JOIN Rapporti_Contabili ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto)")


    '                StrSQL.Append(" Where (ImpresexIndirizzi.Tipo_Indirizzo = 1)")
    '                StrSQL.Append(" AND   UtentixImprese.[User] = '" & Agro_SQL_SaveText(UtenteProfilo) & "' ")
    '                StrSQL.Append(" AND   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                StrSQL.Append(" AND   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                '---------------------------------------------
    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                Select Case objParametri.FlagVisibilita
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND     (UtentiXImprese.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Imprese.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (GerarchiaImprese.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (ImpresexIndirizzi.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Indirizzi.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (ISTAT.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Lista_Province.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Centri_Aziendali.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (CentrixIndirizzi.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Appezzamento.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Reg_Impianti.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Imprese_Progetti.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Imprese_Codici.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Centri_Aziendali_Codici.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Appezzamento_Codici.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Reg_Impianti_Codici.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Campi.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (AppezzamentixParticelle.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (GruppoVarietale.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Cultivar.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (SpecieVegetali.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (GruppoVegetale.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Mov_Destinazioni.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Agenda.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Movimenti.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Mov_Dettaglio_Tecnico.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Movimenti_Dettagli.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Operazioni.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (GruppoOperazioni.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Risorse_Umane.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Contatti.inviato >= 0)  ")
    '                        StrSQL.Append(" AND     (Rapporti_Contabili.inviato >= 0)  ")

    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
    '                        StrSQL.Append(" AND     (UtentiXImprese.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Imprese.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (GerarchiaImprese.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (ImpresexIndirizzi.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Indirizzi.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (ISTAT.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Lista_Province.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Centri_Aziendali.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (CentrixIndirizzi.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Appezzamento.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Reg_Impianti.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Imprese_Progetti.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Imprese_Codici.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Centri_Aziendali_Codici.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Appezzamento_Codici.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Reg_Impianti_Codici.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Campi.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (AppezzamentixParticelle.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (GruppoVarietale.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Cultivar.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (SpecieVegetali.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (GruppoVegetale.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Mov_Destinazioni.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Agenda.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Movimenti.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Mov_Dettaglio_Tecnico.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Movimenti_Dettagli.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Operazioni.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (GruppoOperazioni.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Risorse_Umane.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Contatti.inviato = -1)  ")
    '                        StrSQL.Append(" AND     (Rapporti_Contabili.inviato = -1)  ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                        '...................................
    '                    Case Else
    '                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '                End Select


    '                '------------------------------------------------------

    '                If xOrderBy = "" Then

    '                    Select Case Select_Code

    '                        Case 0

    '                            StrSQL.Append(" ORDER BY Imprese.Rag_Soc, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg, Agenda.Id_Agenda ASC") 'Importante

    '                        Case 1

    '                            StrSQL.Append(" GROUP BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia    ")

    '                        Case 4 'Appezzamenti

    '                            StrSQL.Append(" AND   Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                            StrSQL.Append(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                            StrSQL.Append(" ORDER BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc  ")

    '                        Case 5 'Impianti (Ordinamento per chiave in modo tale da facilitare l'esclusione dei duplicati)

    '                            StrSQL.Append(" AND   Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                            StrSQL.Append(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                            StrSQL.Append(" ORDER BY Imprese.Piva, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg  ")

    '                        Case 8 'Campi

    '                            StrSQL.Append(" AND   Campi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                            StrSQL.Append(" AND   Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                            StrSQL.Append(" ORDER BY Imprese.Piva, Centri_Aziendali.Sa_Cod, Campi.Campo_Cod ")

    '                        Case Else

    '                            StrSQL.Append(" ORDER BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc  ")

    '                    End Select

    '                Else

    '                    Select Case Select_Code

    '                        Case 4 'Appezzamenti

    '                            StrSQL.Append(" AND   Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                            StrSQL.Append(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                        Case 5 'Impianti (Ordinamento per chiave in modo tale da facilitare l'esclusione dei duplicati)

    '                            StrSQL.Append(" AND   Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                            StrSQL.Append(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                        Case 8 'Campi

    '                            StrSQL.Append(" AND   Campi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '                            StrSQL.Append(" AND   Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

    '                    End Select

    '                    StrSQL.Append(" " & xOrderBy)

    '                End If

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
    Public Function Leggi2(ByVal PivaSuperUser As String,
                           ByVal Piva As String,
                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixImprese_Read.Leggi2()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '---------------------------------------------
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("         UtentiXImprese.[USER] AS PivaSuperUser,  ")
                    strSql.AppendLine("         Imprese.* ")

                    strSql.AppendLine(" FROM    UtentiXImprese INNER JOIN ")
                    strSql.AppendLine("         Imprese ON UtentiXImprese.PIVA = Imprese.PIVA ")

                    strSql.AppendLine(" WHERE   (UtentiXImprese.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (UtentiXImprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    strSql.AppendLine(" AND     (Imprese.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    strSql.AppendLine(" AND     (UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(PivaSuperUser).Trim & "')    ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND (Imprese.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If


                    '---------------------------------------------

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato >= 0)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato = -1)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese.Rag_Soc ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '---------------------------------------------
                    strSql.Length = 0

                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("         UtentiXImprese.[USER] AS PivaSuperUser,  ")
                    strSql.AppendLine("         Imprese.* ")

                    strSql.AppendLine(" FROM    UtentiXImprese INNER JOIN ")
                    strSql.AppendLine("         Imprese ON UtentiXImprese.PIVA = Imprese.PIVA ")

                    strSql.AppendLine(" WHERE   (UtentiXImprese.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (UtentiXImprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")
                    strSql.AppendLine(" AND     (Imprese.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ") ")
                    strSql.AppendLine(" AND     (Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    strSql.AppendLine(" AND     (UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(PivaSuperUser).Trim & "')    ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND (Imprese.PIVA = '" & Agro_SQL_SaveText(Piva).Trim & "') ")
                    End If


                    '---------------------------------------------

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato >= 0)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato = -1)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese.Rag_Soc ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni



                Case enumSelezioneVariabile.Selezione_JoinCompleta



            End Select

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

    ''' <summary>
    ''' Da utilizzare con Selezione_JoinCompleta
    ''' </summary>
    ''' <param name="Piva">Per non essere considerato ""</param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo">Per non essere considerato ""</param>
    ''' <param name="xOrderBy">Per non essere considerato ""</param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Leggi3(ByVal Piva As String,
                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixImprese_Read.Leggi3()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT  *   ")
                    strSql.AppendLine(" FROM    Imprese ")

                    strSql.AppendLine(" INNER JOIN UtentiXImprese ON Imprese.PIVA = UtentiXImprese.PIVA ")

                    strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(CStr(objParametri.PivaSuperUser)) & "' ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND     (Imprese.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
                    End If

                    '---------------------------------------------

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato >= 0)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato >= 0)  ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     (UtentiXImprese.inviato = -1)  ")
                            strSql.AppendLine(" AND     (Imprese.inviato = -1)  ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Imprese.Rag_Soc ASC ")
                    End If

            End Select

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


    'Filtrone modificato per filtrare i dati delle tabelle RapportiniStrube_Testata e RapportiniStrube_Dettagli
    Public Function FiltroneSuperNova_Strube(ByRef objParametri As AgronicaCoreParametri,
                                             ByVal Filtro As String,
                                             ByVal TipoSelect As Integer,
                                             ByVal Ordinamento As String
                                             ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixImprese_Read.FiltroneSuperNova_Strube()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            Select Case TipoSelect

                Case enum_TipoSelect_FiltroneSuperNova.Base '0

                    'Nota: NON RIMUOVERE NULLA DALLA SELECT LIST!!!

                    strSql.AppendLine(" SELECT Distinct ")
                    strSql.AppendLine("        dbo.Imprese.PIVA, dbo.Imprese.rag_soc, dbo.Imprese.Sup_Totale, Imprese.Validita_Inizio as Impresa_Validita_Inizio, Imprese.Validita_Fine as Impresa_Validita_Fine,")
                    strSql.AppendLine("        dbo.Centri_Aziendali.sa_cod, dbo.Centri_Aziendali.sa_nome, dbo.Centri_Aziendali.TitoloPossesso, Centri_Aziendali.Validita_Inizio as Centri_Aziendali_Validita_Inizio, Centri_Aziendali.Validita_Fine as Centri_Aziendali_Validita_Fine,")
                    strSql.AppendLine("        Campi.Campo_Cod, Campi.Campo_Des,")
                    strSql.AppendLine("        dbo.Appezzamento.APPEZZA, dbo.Appezzamento.SUP_APP, dbo.Appezzamento.APP_NOME, dbo.Appezzamento.CAMPO_COD, Appezzamento.Validita_Inizio as Appezzamento_Validita_Inizio, Appezzamento.Validita_Fine as Appezzamento_Validita_Fine,")
                    strSql.AppendLine("        dbo.Reg_Impianti.CUL_COD, dbo.Reg_Impianti.GRVA_Cod_VEG, GruppoVarietale.GRVA_DES, dbo.Reg_Impianti.ID_REG, dbo.Reg_Impianti.P_HA, dbo.Reg_Impianti.Port_Cod, Reg_Impianti.Validita_Inizio as Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine as Reg_Impianti_Validita_Fine, dbo.Reg_Impianti.Sup_Imp, Reg_Impianti.Resa_Prevista, ")
                    strSql.AppendLine("        dbo.Lista_Province.PROVINCIA,")
                    strSql.AppendLine("        dbo.GruppoVegetale.GRU_DES, dbo.Istat.Cap, dbo.Istat.Localita,")
                    strSql.AppendLine("        dbo.SpecieVegetali.VEG_COD, dbo.SpecieVegetali.VEG_DES, dbo.Cultivar.CUL_DES, Imprese.TipoImpresaGerarchia ,")
                    strSql.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Des_Lib, Operazioni.Lav_Des, ")
                    strSql.AppendLine("        Agenda.Validita_Inizio, Movimenti.Data_Movimento ")


                Case enum_TipoSelect_FiltroneSuperNova.ImpreseAlbero '1 'Solo Imprese x Albero

                    strSql.AppendLine(" SELECT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia ")

                Case enum_TipoSelect_FiltroneSuperNova.Imprese  '2 'Imprese

                    'strSql.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine "
                    strSql.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine ")


                Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali  '3 'Centri Aziendali

                    strSql.AppendLine(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
                    strSql.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine ")

                Case enum_TipoSelect_FiltroneSuperNova.Appezzamenti  '4 'Appezzamenti

                    strSql.AppendLine(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
                    strSql.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
                    strSql.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                    strSql.AppendLine(" Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.APP_NOME, Appezzamento.Campo_Cod, Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine ")

                Case enum_TipoSelect_FiltroneSuperNova.Impianti  '5 'Impianti

                    strSql.AppendLine(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, Lista_Province.PROVINCIA, ")
                    strSql.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, ")
                    strSql.AppendLine(" SpecieVegetali.Veg_Cod, Reg_Impianti.Cul_Cod,  dbo.Reg_Impianti.GRVA_Cod_VEG, ")
                    strSql.AppendLine(" SpecieVegetali.Veg_Des, Cultivar.Cul_Des, GruppoVarietale.GRVA_DES, ")
                    strSql.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                    strSql.AppendLine(" Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.APP_NOME, ")
                    strSql.AppendLine(" Reg_Impianti.ID_REG, Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine, ")
                    strSql.AppendLine(" Reg_Impianti.Sup_Imp, Imprese_Progetti.Regolamento_Cod AS Regolamento, Imprese_Progetti.Disciplinare_Cod AS Finanziamento, Reg_Impianti.Grfi_Cod,  Reg_Impianti.Grva_Cod_Veg, Reg_Impianti.Cop_Cod, ")
                    strSql.AppendLine(" Imprese_Progetti.Progetto_Cod, Imprese_Progetti.Progetto_Nome ")
                    '         strSql.AppendLine("        ,RapportiniStrube_Testata.Genetica_m, RapportiniStrube_Testata.Genetica_f, RapportiniStrube_Testata.OType_m, RapportiniStrube_Testata.OType_f, RapportiniStrube_Testata.OffType_m, RapportiniStrube_Testata.OffType_f, RapportiniStrube_Testata.Data_Trap, RapportiniStrube_Testata.Scopazzi, RapportiniStrube_Testata.Grover_Code, ")
                    '         strSql.AppendLine("        RapportiniStrube_Dettagli.stadiofisio_m, RapportiniStrube_Dettagli.stadiofisio_f, RapportiniStrube_Dettagli.vigore_m, RapportiniStrube_Dettagli.vigore_f, RapportiniStrube_Dettagli.uniformita_m, RapportiniStrube_Dettagli.uniformita_f, RapportiniStrube_Dettagli.produttivita_m, RapportiniStrube_Dettagli.produttivita_f, RapportiniStrube_Dettagli.malattie_m, RapportiniStrube_Dettagli.malattie_f, RapportiniStrube_Dettagli.parassiti, RapportiniStrube_Dettagli.emergenti, RapportiniStrube_Dettagli.infestanti, RapportiniStrube_Dettagli.note ")

                Case enum_TipoSelect_FiltroneSuperNova.Movimenti  '6 'Movimenti

                    strSql.AppendLine(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
                    strSql.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
                    strSql.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                    strSql.AppendLine(" Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.APP_NOME, ")
                    strSql.AppendLine(" Reg_Impianti.ID_REG, Reg_Impianti.Sup_Imp, ")
                    strSql.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Des_Lib, Agenda.Username_Creazione, Agenda.Username_Modifica, Agenda.Data_Creazione, Agenda.Data_Modifica, Agenda.Validita_Inizio, ")
                    strSql.AppendLine(" Movimenti.Data_Movimento, Movimenti.Cau_Mov, Movimenti.Num_Protocollo, ")
                    strSql.AppendLine(" SpecieVegetali.VEG_COD, SpecieVegetali.Veg_Des, Reg_Impianti.Cul_Cod, Cultivar.Cul_Des  ")

                Case enum_TipoSelect_FiltroneSuperNova.Contatti  '7 'Contatti

                    strSql.AppendLine(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
                    strSql.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                    strSql.AppendLine(" Contatti.Cod_Contatto, Contatti.Rag_Soc as Contatto_Rag_Soc,")
                    strSql.AppendLine(" Risorse_Umane.Cod_Rapporto, Risorse_Umane.Cod_RisUm, Risorse_Umane.Piva, Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, Risorse_Umane.Validita_Fine as Cessazione,")
                    strSql.AppendLine(" Rapporti_Contabili.Rapporto_Des")

                Case enum_TipoSelect_FiltroneSuperNova.Campi  '8 'Campi

                    strSql.AppendLine(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
                    strSql.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
                    strSql.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                    strSql.AppendLine(" Campi.Validita_Inizio, Campi.Validita_Fine ")

            End Select


            'Completo la query SQL
            strSql.AppendLine(" From (((((((((((((((((((((((( " &
                              " Imprese INNER JOIN UtentiXImprese On Imprese.Piva = UtentixImprese.Piva)" &
                              "         INNER JOIN GerarchiaImprese ON Imprese.Piva = GerarchiaImprese.Figlio)" &
                              "         INNER join ImpresexIndirizzi ON Imprese.PIVA = ImpresexIndirizzi.PIVA)" &
                              "         INNER JOIN Indirizzi ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo)" &
                              "         INNER JOIN ISTAT ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM)" &
                              "         INNER JOIN Lista_Province ON Lista_Province.Sigla = ISTAT.COMUNI_PROV)" &
                              "    LEFT OUTER JOIN Centri_Aziendali ON Imprese.Piva=Centri_Aziendali.Piva)" &
                              "    LEFT OUTER join CentrixIndirizzi ON (Centri_Aziendali.PIVA = CentrixIndirizzi.PIVA And Centri_Aziendali.Sa_Cod = CentrixIndirizzi.Sa_Cod)" &
                              "    LEFT OUTER JOIN Indirizzi As Indirizzi_Centro ON CentrixIndirizzi.cod_indirizzo = Indirizzi_Centro.cod_indirizzo)" &
                              "    LEFT OUTER JOIN ISTAT As Istat_Centro ON Indirizzi_Centro.pro_cod_istat = Istat_Centro.PROV AND Indirizzi_Centro.com_cod_istat = Istat_Centro.COM)" &
                              "    LEFT OUTER JOIN Lista_Province as Lista_Province_Centro ON Lista_Province_Centro.Sigla = Istat_Centro.COMUNI_PROV)" &
                              "    LEFT OUTER JOIN Appezzamento on (Centri_Aziendali.Piva = Appezzamento.Piva AND Centri_Aziendali.Sa_Cod = Appezzamento.Sa_Cod )" &
                              "    LEFT OUTER JOIN Reg_Impianti on (Appezzamento.Piva = Reg_Impianti.Piva AND Appezzamento.SA_Cod = Reg_Impianti.Sa_Cod AND Appezzamento.Appezza = Reg_Impianti.Appezza)" &
                              "    LEFT OUTER JOIN Imprese_Progetti on (Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg)" &
                              "    LEFT OUTER JOIN Imprese_Codici on (Imprese_Codici.Piva = Imprese.Piva)" &
                              "    LEFT OUTER JOIN Centri_Aziendali_Codici on (Centri_Aziendali_Codici.Piva = Centri_Aziendali.Piva AND Centri_Aziendali_Codici.SA_Cod = Centri_Aziendali.Sa_Cod )" &
                              "    LEFT OUTER JOIN Appezzamento_Codici on (Appezzamento_Codici.Piva = Appezzamento.Piva AND Appezzamento_Codici.SA_Cod = Appezzamento.Sa_Cod AND Appezzamento_Codici.Appezza = Appezzamento.Appezza )" &
                              "    LEFT OUTER JOIN Reg_Impianti_Codici on (Reg_Impianti_Codici.Piva = Reg_Impianti.Piva AND Reg_Impianti_Codici.SA_Cod = Reg_Impianti.Sa_Cod AND Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)" &
                              "    LEFT OUTER JOIN Campi on (Campi.Piva = Appezzamento.Piva AND Campi.Sa_Cod = Appezzamento.Sa_Cod AND Campi.Campo_Cod = Appezzamento.Campo_Cod )" &
                              "    LEFT OUTER JOIN AppezzamentixParticelle on (AppezzamentixParticelle.Piva = Appezzamento.Piva AND AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod AND AppezzamentixParticelle.Appezza = Appezzamento.Appezza )")

            strSql.AppendLine("    LEFT OUTER JOIN GruppoVarietale ON abs(Reg_Impianti.GRVA_Cod_VEG) = GruppoVarietale.GRVA_COD)" &
                              "    LEFT OUTER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.CUL_COD)" &
                              "    LEFT OUTER JOIN SpecieVegetali ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD)" &
                              "    LEFT OUTER JOIN GruppoVegetale ON SpecieVegetali.GRU_COD = GruppoVegetale.GRU_COD)" &
                              "    LEFT OUTER JOIN Mov_Destinazioni ON Mov_Destinazioni.Piva = Reg_Impianti.Piva and Mov_Destinazioni.Sa_Cod = Reg_Impianti.Sa_Cod and Mov_Destinazioni.Appezza = Reg_Impianti.Appezza and Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg  And Mov_Destinazioni.Tipo_Destinazione = 0)" &
                              "    LEFT OUTER JOIN Agenda ON Agenda.Piva = Reg_Impianti.Piva and Agenda.Sa_Cod = Reg_Impianti.Sa_Cod and Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda)" &
                              "    LEFT OUTER JOIN Movimenti ON Agenda.Piva = Movimenti.Piva and Agenda.Sa_Cod = Movimenti.Sa_Cod and Agenda.Id_Agenda = Movimenti.Id_Agenda)" &
                              "    LEFT OUTER JOIN Mov_Dettaglio_Tecnico ON Mov_Dettaglio_Tecnico.Piva = Movimenti.Piva and Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti.Sa_Cod and Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti.Id_Agenda and Mov_Dettaglio_Tecnico.Id_Mov = Movimenti.Id_Mov)" &
                              "    LEFT OUTER JOIN Movimenti_Dettagli ON Movimenti_Dettagli.Piva = Movimenti.Piva and Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod and Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda and Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov)" &
                              "    LEFT OUTER JOIN Operazioni ON  Agenda.Lav_Cod = Operazioni.Lav_Cod) " &
                              "    LEFT OUTER JOIN GruppoOperazioni ON  Operazioni.Gru_Op = GruppoOperazioni.Gru_Cod) " &
                              "    LEFT OUTER JOIN Risorse_Umane ON ((Risorse_Umane.Cod_RisUm = Movimenti.Cod_RisUm) or (Risorse_Umane.Cod_RisUm = Movimenti_Dettagli.Mat_Cod and Movimenti_Dettagli.Elem_Cod = 0) ) )" &
                              "    LEFT OUTER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto)" &
                              "    LEFT OUTER JOIN Rapporti_Contabili ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto)" &
                              "    LEFT OUTER JOIN RapportiniStrube_Testata ON (RapportiniStrube_Testata.Piva = Reg_Impianti.Piva AND RapportiniStrube_Testata.SA_Cod = Reg_Impianti.Sa_Cod AND RapportiniStrube_Testata.Appezza = Reg_Impianti.Appezza AND RapportiniStrube_Testata.Id_Reg = Reg_Impianti.Id_Reg)" &
                              "    LEFT OUTER JOIN RapportiniStrube_Dettagli ON (RapportiniStrube_Testata.Piva = RapportiniStrube_Dettagli.Piva AND RapportiniStrube_Testata.SA_Cod = RapportiniStrube_Dettagli.Sa_Cod AND RapportiniStrube_Testata.Appezza = RapportiniStrube_Dettagli.Appezza AND RapportiniStrube_Testata.Id_Reg = RapportiniStrube_Dettagli.Id_Reg)")


            strSql.AppendLine(" Where (ImpresexIndirizzi.Tipo_Indirizzo = 1)")
            strSql.AppendLine(" AND   UtentixImprese.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine(" AND   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Trim(Filtro) <> "" Then

                ' se nel filtro non c'è AND lo aggiungo
                If Not Trim(Filtro).StartsWith("AND") Then
                    strSql.AppendLine(" AND ")
                End If

                strSql.AppendLine(Filtro)
            End If


            Select Case TipoSelect

                Case 0

                    strSql.AppendLine(" ORDER BY Imprese.Rag_Soc, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg, Agenda.Id_Agenda ASC") 'Importante

                Case 1

                    strSql.AppendLine(" GROUP BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia    ")

                Case 4 'Appezzamenti

                    strSql.AppendLine(" AND   Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    strSql.AppendLine(" ORDER BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc  ")

                Case 5 'Impianti (Ordinamento per chiave in modo tale da facilitare l'esclusione dei duplicati)

                    strSql.AppendLine(" AND   Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    strSql.AppendLine(" ORDER BY Imprese.Piva, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg  ")

                Case 8 'Campi

                    strSql.AppendLine(" AND   Campi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    strSql.AppendLine(" ORDER BY Imprese.Piva, Centri_Aziendali.Sa_Cod, Campi.Campo_Cod ")

                Case Else

                    strSql.AppendLine(" ORDER BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc  ")

            End Select

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


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class UtentixImprese_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Validazione As Integer = 0,
                           Optional ByVal Data_Validazione As DateTime = #2/1/1900#,
                           Optional ByVal UserName_Validazione As String = ""
                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.UtentixImprese_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        If Data_Validazione = #2/1/1900# Then
            Data_Validazione = Now
        End If

        Try

            strSql.Length = 0
            strSql.AppendLine("INSERT INTO UtentixImprese(       ")
            strSql.AppendLine("                    [User],         ")
            strSql.AppendLine("                    PIVA,         ")
            strSql.AppendLine("                    Validazione,   Data_Validazione,  UserName_Validazione, ")

            strSql.AppendLine("                    Inviato,            DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("                    ) ")
            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Validazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Validazione) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(UserName_Validazione) & "' ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine(")")

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

    '##############################################################################################
    Public Function Modifica(ByVal PivaSuperUser As String,
                             ByVal Piva As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.UtentixImprese_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("UPDATE UtentixImprese SET ")
            strSql.AppendLine("        Inviato           =  0 ")
            strSql.AppendLine("       ,DataInvio         =  Null ")
            strSql.AppendLine("       ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.AppendLine("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine(" WHERE Piva='" & Agro_SQL_SaveText(Trim(Piva)) & "'")
            strSql.AppendLine(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    '##############################################################################################
    Public Function Cancella(ByVal PivaSuperUser As String,
                             ByVal Piva As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.UtentixImprese_Write.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE UtentixImprese ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.AppendLine(" AND Inviato >= 0")

                If PivaSuperUser <> "" Then
                    strSql.AppendLine(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
                End If

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     UtentixImprese ")
                strSql.AppendLine(" WHERE    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

                If PivaSuperUser <> "" Then
                    strSql.AppendLine(" AND   [User] = '" & Replace(PivaSuperUser, "'", "''") & "' ")
                End If

            End If
            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    '##############################################################################################
    Public Function AggiornaValiditaInizio(ByVal PivaSuperUser As String,
                                           ByVal Piva As String,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.UtentixImprese_Write.AggiornaValiditaInizio()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("UPDATE UtentixImprese SET ")
            strSql.AppendLine("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND  [User]= '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            strSql.AppendLine(" AND   Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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

    '##############################################################################################
    Public Function AggiornaValiditaFine(ByVal PivaSuperUser As String,
                                         ByVal Piva As String,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.UtentixImprese_Write.AggiornaValiditaFine()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("UPDATE UtentixImprese SET ")
            strSql.AppendLine("   UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Fine   =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND  [User]= '" & Agro_SQL_SaveText(PivaSuperUser) & "'")
            strSql.AppendLine(" AND   Validita_Fine > " & Agro_SQL_SaveDate(Validita_Fine))

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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
