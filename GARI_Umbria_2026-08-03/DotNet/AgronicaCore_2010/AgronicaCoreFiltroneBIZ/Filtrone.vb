Imports System.Text
Imports System.Web.ModelBinding
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

Public Class Filtrone
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Accoda al datatable tutte le chiavi presenti nella lista dell'oggetto passato come primo parametro (accoda piva <> '' ed altre chiavi = 0 )
    ''' </summary>
    ''' <param name="richiesta"></param>
    ''' <param name="DataTableToAdd"></param>
    Public Sub oggettoFiltroAzienda(richiesta As jsonProseguiSelezionati, ByRef DataTableToAdd As DataTable)


        For Each chiave As String In richiesta.chiavi

            Dim piva As String = chiave.Split("_")(0)


            Dim dr As DataRow = DataTableToAdd.NewRow()

            dr("piva") = piva

            DataTableToAdd.Rows.Add(dr)
        Next

    End Sub

    ''' <summary>
    ''' Accoda al datatable tutte le chiavi presenti nella lista dell'oggetto passato come primo parametro (accoda piva <> '' ed altre chiavi = 0 )
    ''' </summary>
    ''' <param name="richiesta"></param>
    ''' <param name="DataTableToAdd"></param>
    Public Sub DataTableFiltroAzienda(ByVal DataTableAzienda As DataTable, ByRef DataTableToAdd As DataTable)


        For Each chiave As DataRow In DataTableAzienda.Rows

            Dim piva As String = chiave("piva")

            Dim sa_cod As String = 0
            Dim appezza As String = 0
            Dim id_reg As String = 0
            Dim progetto_cod As String = 0


            Dim dr As DataRow = DataTableToAdd.NewRow()

            dr("piva") = piva
            dr("sa_cod") = sa_cod
            dr("appezza") = appezza
            dr("id_reg") = id_reg
            dr("progetto_cod") = progetto_cod

            DataTableToAdd.Rows.Add(dr)

        Next

    End Sub

    ''' <summary>
    ''' Accoda al datatable tutte le chiavi presenti nella lista dell'oggetto passato come primo parametro (accoda piva <> '' ed altre chiavi = 0 )
    ''' </summary>
    ''' <param name="richiesta"></param>
    ''' <param name="DataTableToAdd"></param>
    Public Sub oggettoFiltroImpiantiConChiaveAzienda(richiesta As jsonProseguiSelezionati, ByRef DataTableToAdd As DataTable)


        For Each chiave As String In richiesta.chiavi

            Dim piva As String = chiave.Split("_")(0)
            Dim sa_cod As String = 0
            Dim appezza As String = 0
            Dim id_reg As String = 0
            Dim progetto_cod As String = 0


            Dim dr As DataRow = DataTableToAdd.NewRow()

            dr("piva") = piva
            dr("sa_cod") = sa_cod
            dr("appezza") = appezza
            dr("id_reg") = id_reg
            dr("progetto_cod") = progetto_cod

            DataTableToAdd.Rows.Add(dr)
        Next

    End Sub

    ''' <summary>
    ''' Accoda al datatable tutte le chiavi presenti nella lista dell'oggetto passato come primo parametro
    ''' </summary>
    ''' <param name="richiesta"></param>
    ''' <param name="DataTableToAdd"></param>
    Public Sub oggettoFiltroImpianti(richiesta As jsonProseguiSelezionati, ByRef DataTableToAdd As DataTable)


        For Each chiave As String In richiesta.chiavi

            Dim piva As String = chiave.Split("_")(0)
            Dim sa_cod As String = chiave.Split("_")(1)
            Dim appezza As String = chiave.Split("_")(2)
            Dim id_reg As String = chiave.Split("_")(3)
            Dim veg_cod As String = chiave.Split("_")(4)
            Dim progetto_cod As String = chiave.Split("_")(5)


            Dim dr As DataRow = DataTableToAdd.NewRow()

            dr("piva") = piva
            dr("sa_cod") = sa_cod
            dr("appezza") = appezza
            dr("id_reg") = id_reg
            dr("progetto_cod") = progetto_cod

            DataTableToAdd.Rows.Add(dr)
        Next


    End Sub

    ''' <summary>
    ''' Accoda al datatable tutte le chiavi presenti nella lista dell'oggetto passato come primo parametro
    ''' </summary>
    ''' <param name="richiesta"></param>
    ''' <param name="DataTableToAdd"></param>
    Public Sub oggettoFiltroFabbricati(richiesta As jsonProseguiSelezionati, ByRef DataTableToAdd As DataTable)


        For Each chiave As String In richiesta.chiavi

            Dim piva As String = chiave.Split("_")(0)
            Dim sa_cod As String = chiave.Split("_")(1)
            Dim fabbricato_cod As String = chiave.Split("_")(2)


            Dim dr As DataRow = DataTableToAdd.NewRow()

            dr("piva") = piva
            dr("sa_cod") = sa_cod
            dr("fabbricato_cod") = fabbricato_cod

            DataTableToAdd.Rows.Add(dr)
        Next


    End Sub

    ''' <summary>
    ''' Genera un nuovo datatable ed accoda tutte le chiavi presenti nella lista
    ''' </summary>
    ''' <param name="richiesta"></param>
    ''' <returns></returns>
    Public Function oggettoFiltroImpianti(richiesta As jsonProseguiSelezionati) As DataTable

        Dim rvalDataTable As DataTable = oggettoFiltroImpianti_CreaDT()

        For Each chiave As String In richiesta.chiavi

            Dim piva As String = chiave.Split("_")(0)
            Dim sa_cod As String = chiave.Split("_")(1)
            Dim appezza As String = chiave.Split("_")(2)
            Dim id_reg As String = chiave.Split("_")(3)
            Dim veg_cod As String = chiave.Split("_")(4)
            Dim progetto_cod As String = chiave.Split("_")(5)


            Dim dr As DataRow = rvalDataTable.NewRow()

            dr("piva") = piva
            dr("sa_cod") = sa_cod
            dr("appezza") = appezza
            dr("id_reg") = id_reg
            dr("progetto_cod") = progetto_cod

            rvalDataTable.Rows.Add(dr)
        Next

        Return rvalDataTable
    End Function

    Public Function oggettoFiltroImpianti_CreaDT() As DataTable
        Dim Dt As New DataTable

        Dt.Columns.Add("piva", System.Type.GetType("System.String"))
        Dt.Columns.Add("sa_cod", System.Type.GetType("System.Int32"))
        Dt.Columns.Add("appezza", System.Type.GetType("System.Int32"))
        Dt.Columns.Add("id_reg", System.Type.GetType("System.Int32"))
        Dt.Columns.Add("progetto_cod", System.Type.GetType("System.Int32"))

        Return Dt
    End Function

    Public Function oggettoFiltroFabbricati_CreaDT() As DataTable
        Dim Dt As New DataTable

        Dt.Columns.Add("piva", System.Type.GetType("System.String"))
        Dt.Columns.Add("sa_cod", System.Type.GetType("System.Int32"))
        Dt.Columns.Add("fabbricato_cod", System.Type.GetType("System.Int32"))

        Return Dt
    End Function

    Public Function oggettoFiltroAzienda_CreaDT() As DataTable
        Dim Dt As New DataTable

        Dt.Columns.Add("piva", System.Type.GetType("System.String"))

        Return Dt
    End Function

    Public Function CreaStringaQueryPerDTFiltrone(
        ByVal jsonRichiesta As filtrone_richiesta_parametriQuery,
        ByRef objParametri As AgronicaCoreParametri,
        ByVal Filtro As String,
        ByVal TipoSelect As Integer,
        ByVal Ordinamento As String,
        ByVal Join As JoinFiltrone,
        Optional IDTestataTemp As Integer = 0,
        Optional ByVal noLock As Boolean = False,
        Optional ByRef objParametriUtenti As AgronicaCoreParametri = Nothing
    ) As String

        '----------------------------------------------------
        '--- Preparo la Query SQL ---------------------------
        '----------------------------------------------------

        'enum_TipoSelect_FiltroneSuperNova
        'Base = 0
        'ImpreseAlbero = 1
        'Imprese = 2
        'CentriAziendali = 3
        'Appezzamenti = 4
        'Impianti = 5
        'Movimenti = 6
        'Contatti = 7
        'Campi = 8
        'Esercizi / Distinta = 10
        'Fabbricati = 12


        ' parte 1 riportare la chiamata di fede
        Dim objUtentiImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DtUtentiImp As DataTable = Nothing
        Dim udm_cod As Integer = 0
        Dim fattConv As Decimal = 1

        If objParametriUtenti IsNot Nothing Then
            DtUtentiImp = objUtentiImp.Leggi(enum_Impostazioni_Utenti.Visualizza_UdmAggiuntiva_xSup, 2, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriUtenti)

            If DtUtentiImp.Rows.Count > 0 Then
                udm_cod = If(IsDBNull(DtUtentiImp.Rows(0)("Impostazione_Valore_1")), 0, CInt(DtUtentiImp.Rows(0)("Impostazione_Valore_1")))
            End If

            If udm_cod <> 0 Then

                Dim objUnitaMisuraConv As New AgronicaCoreMetaSchemaDAL.UnitaMisura_Conversione_R
                Dim dtFattConv As DataTable = Nothing

                dtFattConv = objUnitaMisuraConv.Leggi(enum_UnitaMisura.Ettaro, udm_cod, "", "", objParametri)
                If dtFattConv.Rows.Count > 0 Then
                    fattConv = If(IsDBNull(dtFattConv.Rows(0)("fattoreconversione")), 0, CDec(dtFattConv.Rows(0)("fattoreconversione")))
                End If
            End If
        End If


        Dim sql2017 As Boolean = True
        Dim majorSql As Integer = DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri)
        Dim utenti_DB_name As String = String.Empty

        If Not IsNothing(objParametriUtenti) Then
            utenti_DB_name = String.Format("{0}.dbo.", NomeDataBase_FromStringaConnessione(objParametriUtenti.StringaConnessione))
        End If

        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017
            sql2017 = True
        Else
            sql2017 = False
        End If

        Dim stb As New System.Text.StringBuilder
        stb.Length = 0

        Select Case TipoSelect

            Case enum_TipoSelect_FiltroneSuperNova.Base '0
#Region "Base"
                'Nota: NON RIMUOVERE NULLA DALLA SELECT LIST!!!

                stb.AppendLine(" SELECT DISTINCT ")
                stb.AppendLine("        dbo.Imprese.PIVA, dbo.Imprese.rag_soc, dbo.Imprese.Sup_Totale, Imprese.Validita_Inizio AS Impresa_Validita_Inizio, Imprese.Validita_Fine AS Impresa_Validita_Fine,")
                stb.AppendLine("        dbo.Centri_Aziendali.sa_cod, dbo.Centri_Aziendali.sa_nome, dbo.Centri_Aziendali.TitoloPossesso, Centri_Aziendali.Validita_Inizio AS Centri_Aziendali_Validita_Inizio, Centri_Aziendali.Validita_Fine AS Centri_Aziendali_Validita_Fine,")
                stb.AppendLine("        Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine("        dbo.Appezzamento.APPEZZA ")
                stb.AppendLine("  , Appezzamento.SUP_APP ")
                If (fattConv > 1) Then
                    stb.AppendLine("  , ROUND(ISNULL( Appezzamento.SUP_APP,0) * " & fattConv.ToString().Replace(",", ".") & ", 4) as acro")
                End If
                stb.AppendLine("        , dbo.Appezzamento.APP_NOME, dbo.Appezzamento.CAMPO_COD, Appezzamento.Validita_Inizio AS Appezzamento_Validita_Inizio, Appezzamento.Validita_Fine AS Appezzamento_Validita_Fine,")
                'elimino p_ha xkè ora passa dall'impianto alla distinta (FEDE 13/05/2010)
                stb.AppendLine("        dbo.Reg_Impianti.CUL_COD, dbo.Reg_Impianti.GRVA_Cod_VEG, GruppoVarietale.GRVA_DES, dbo.Reg_Impianti.ID_REG, dbo.Reg_Impianti.Port_Cod, Reg_Impianti.Validita_Inizio AS Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine AS Reg_Impianti_Validita_Fine, dbo.Reg_Impianti.Sup_Imp, Reg_Impianti.Resa_Prevista, ")
                'StrSQL.append( "        dbo.Reg_Impianti.CUL_COD, dbo.Reg_Impianti.GRVA_Cod_VEG, GruppoVarietale.GRVA_DES, dbo.Reg_Impianti.ID_REG, dbo.Reg_Impianti.P_HA, dbo.Reg_Impianti.Port_Cod, Reg_Impianti.Validita_Inizio AS Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine AS Reg_Impianti_Validita_Fine, dbo.Reg_Impianti.Sup_Imp, Reg_Impianti.Resa_Prevista, "
                stb.AppendLine("        dbo.Lista_Province.PROVINCIA,")
                stb.AppendLine("        dbo.GruppoVegetale.GRU_DES, dbo.Istat.Cap, dbo.Istat.Localita,")
                stb.AppendLine("        dbo.SpecieVegetali.VEG_COD, dbo.SpecieVegetali.VEG_DES, dbo.Cultivar.CUL_DES, Imprese.TipoImpresaGerarchia,")
                stb.AppendLine("        Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.Des_Lib, Operazioni.Lav_Des, ")
                stb.AppendLine("        Agenda.Validita_Inizio, Movimenti.Data_Movimento ")

                Join.bCentriAziendali = True
                Join.bCampi = True
                Join.bAppezzamento = True
                Join.bRegImpianti = True
                Join.bListaProvince = True
                Join.bIstat = True
                Join.bGruppoVegetale = True
                Join.bGruppoVarietale = True
                Join.bSpecieVegetali = True
                Join.bCultivar = True
                Join.bAgenda = True
                Join.bOperazioni = True
                Join.bMovimenti = True
#End Region

            Case enum_TipoSelect_FiltroneSuperNova.ImpreseAlbero '1 'Solo Imprese x Albero

                stb.AppendLine(" SELECT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia")
                stb.AppendLine("        , Imprese.Username_Creazione, Imprese.Username_Modifica, Imprese.Data_Creazione, Imprese.Data_Modifica ")
                Join.bGerarchiaImprese = True

            Case enum_TipoSelect_FiltroneSuperNova.Imprese  '2 'Imprese

                stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA AS chiave, dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine, IC.val_cod AS CodiceSocio, IC_Cuaa.val_cod AS CodiceCuaa ")
                stb.AppendLine("        , Imprese.Username_Creazione, Imprese.Username_Modifica, CONVERT(DATE, Imprese.Data_Creazione) AS Data_Creazione, CONVERT(DATE, Imprese.Data_Modifica) AS Data_Modifica ")

                If Not IsNothing(objParametriUtenti) Then
                    If sql2017 Then
                        stb.AppendLine("        , TRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, '')) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , TRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, '')) AS Username_Modifica_Completo ")
                    Else
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, ''))) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, ''))) AS Username_Modifica_Completo")
                    End If
                Else
                    stb.AppendLine("        , Imprese.Username_Creazione AS Username_Creazione_Completo ")
                    stb.AppendLine("        , Imprese.Username_Modifica AS Username_Modifica_Completo ")
                End If

                stb.AppendLine("        , ISNULL((SELECT TOP 1 Cognome + ' ' + Nome + ' ' + Rag_Soc FROM Contatti WHERE Cod_Contatto = IC_Tecnico.val_cod), '') AS TecnicoRifImpresa")
                stb.AppendLine("        , ISNULL(STUFF ((SELECT ', ' + CA_Des FROM CertificazioniAziendali AS ca WHERE ca.CA_Cod in (SELECT strName FROM dbo.fSplit(IC_Certificazione.val_cod, ','))")
                stb.AppendLine("                         For XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 2, ''), '') AS Certificazione")
                stb.AppendLine("        , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS Gruppi_Raccolta")

                If Join.bGerarchiaImprese AndAlso sql2017 Then
                    stb.AppendLine("  , g.Padri, g.rag_soc_s AS Referenti ")
                End If

                Join.bListaProvince = True
                Join.bIndirizzi = True
                Join.bIstat = True
                Join.bImpreseCodici_CodiceSocio = True
                Join.bImpreseCodici_CodiceCuaa = True

                Join.bImpreseCodici_CodiceTecnico = True
                Join.bImpreseCodici_CodiceCertificazione = True
                Join.bGruppiRaccolta = True
                Join.bGerarchiaImprese = True

            Case enum_TipoSelect_FiltroneSuperNova.Imprese_Codici  '9 'Imprese_Codici

                stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine, Imprese_Codici.id_cod, Imprese_Codici.val_cod  ")

                Join.bListaProvince = True
                Join.bIndirizzi = True
                Join.bIstat = True
                Join.bImpreseCodici = True


            Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali  '3 'Centri Aziendali
#Region "Centri Aziendali"

                stb.AppendLine(" ;WITH cte_LegaleRappresentante AS ( ")
                stb.AppendLine("    SELECT")
                stb.AppendLine("          Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS LegaleRappresentante ")
                stb.AppendLine("        , Data_Nascita ")
                stb.AppendLine("        , Codice_Fiscale ")
                stb.AppendLine("        , Risorse_Umane.Piva ")
                stb.AppendLine("        , ind_des AS Indirizzo ")
                stb.AppendLine("    FROM Risorse_Umane ")
                stb.AppendLine("    INNER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
                stb.AppendLine("    INNER JOIN ContattiXIndirizzi ON ContattiXIndirizzi.Cod_Contatto = Contatti.Cod_Contatto and Tipo_Indirizzo = " & enum_IndirizzoTipo.LuogoNascita & " ")
                stb.AppendLine("    INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = ContattiXIndirizzi.Cod_Indirizzo ")
                stb.AppendLine("    WHERE Risorse_Umane.Cod_Rapporto = " & enum_Rapporti_Contabili_Standard.Legale_Rappresentante & " ")
                stb.AppendLine(" ) ")
                stb.AppendLine(" SELECT DISTINCT (Imprese.PIVA + '_' + CAST(Centri_Aziendali.sa_cod AS VARCHAR(20))) AS chiave, Imprese.PIVA, Imprese.rag_soc, ")
                stb.AppendLine("        Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine, ")
                stb.AppendLine("        Indirizzi_Centro.ind_Des AS Indirizzo, Indirizzi_Centro.CAP AS CAP, ISTAT_Centro.Localita AS Comune, ISTAT_CENTRO.COMUNI_PROV AS Provincia ")

                'Istat_Particelle
                stb.AppendLine("        , Istat_Particelle.Localita AS Comune_Particella, Istat_Particelle.COMUNI_Prov AS Provincia_Particella, CASE WHEN ImpresexParticelle.Sezione = '0' THEN '' ELSE ImpresexParticelle.Sezione END AS Sezione, ImpresexParticelle.Foglio, ImpresexParticelle.Numero, ImpresexParticelle.Subalterno ")
                stb.AppendLine("        , ImpresexParticelle.Validita_Inizio AS Validita_Inizio_Possesso, ImpresexParticelle.Validita_Fine AS Validita_Fine_Possesso")
                stb.AppendLine("        , Centri_Aziendali.Username_Creazione, Centri_Aziendali.Username_Modifica, CONVERT(DATE, Centri_Aziendali.Data_Creazione) AS Data_Creazione, CONVERT(DATE, Centri_Aziendali.Data_Modifica) AS Data_Modifica")
                stb.AppendLine("        , ImpresexParticelle.Sup_Condotta, ParticelleCatastali.Ettari + (CAST(ParticelleCatastali.ARE AS decimal) / 100) + (CAST(ParticelleCatastali.CENTIARE AS decimal) / 10000) AS Sup_Catastale ")
                If fattConv > 1 Then
                    stb.Append(" , ROUND(ISNULL(ImpresexParticelle.Sup_Condotta,0) * " & fattConv.ToString().Replace(",", ".") & ", 4) as Sup_condottaAcro")
                    stb.Append(" , ROUND(ISNULL(ParticelleCatastali.Ettari + (CAST(ParticelleCatastali.ARE as decimal) / 100) + (CAST(ParticelleCatastali.CENTIARE as decimal) / 10000),0)  * " & fattConv.ToString().Replace(",", ".") & ", 4) as Sup_Catastale_Acro " & vbCrLf)
                End If

                If Not IsNothing(objParametriUtenti) Then
                    If sql2017 Then
                        stb.AppendLine("        , TRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, '')) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , TRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, '')) AS Username_Modifica_Completo ")
                    Else
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, ''))) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, ''))) AS Username_Modifica_Completo")
                    End If
                Else
                    stb.AppendLine("        , Centri_Aziendali.Username_Creazione AS Username_Creazione_Completo ")
                    stb.AppendLine("        , Centri_Aziendali.Username_Modifica AS Username_Modifica_Completo ")
                End If

                stb.AppendLine("        , IC_Cuaa.val_cod AS CodiceCuaa ")

                stb.AppendLine("        , ISNULL((SELECT TOP 1 Cognome + ' ' + Nome + ' ' + Rag_Soc FROM Contatti WHERE Cod_Contatto = IC_Tecnico.val_cod), '') AS TecnicoRifImpresa")
                stb.AppendLine("        , ISNULL(STUFF ((SELECT ', ' + CA_Des FROM CertificazioniAziendali AS ca WHERE ca.CA_Cod in (SELECT strName FROM dbo.fSplit(IC_Certificazione.val_cod, ','))")
                stb.AppendLine("                         For XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 2, ''), '') AS Certificazione")
                stb.AppendLine("        , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS Gruppi_Raccolta")

                If Join.bGerarchiaImprese AndAlso sql2017 Then
                    stb.AppendLine("  , g.Padri, g.rag_soc_s AS Referenti ")
                End If

                stb.AppendLine("      , ISNULL(IC_UltimaVerificaIspettiva.Val_Cod, '') AS Ultima_Verifica_Ispettiva")
                stb.AppendLine("      , ISNULL(IC_SAUTotAziendale.Val_Cod, '') AS SAU_Tot_Aziendale")
                stb.AppendLine("      , ISNULL(RegioneImpresa.Regione_Des, '') AS RegioneImpresa")

                stb.AppendLine("      , Centri_Aziendali.lat AS Latitudine, Centri_Aziendali.long AS Longitudine ")

                stb.AppendLine("      , ISNULL((SELECT TOP 1 LegaleRappresentante FROM cte_LegaleRappresentante WHERE cte_LegaleRappresentante.Piva = Imprese.Piva), '') AS LegaleRappresentante ")
                stb.AppendLine("      , (SELECT TOP 1 Data_Nascita FROM cte_LegaleRappresentante WHERE cte_LegaleRappresentante.Piva = Imprese.Piva) AS LegaleRappresentante_DataNascita ")
                stb.AppendLine("      , ISNULL((SELECT TOP 1 Codice_Fiscale FROM cte_LegaleRappresentante WHERE cte_LegaleRappresentante.Piva = Imprese.Piva), '') AS LegaleRappresentante_CodiceFiscale ")
                stb.AppendLine("      , ISNULL((SELECT TOP 1 Indirizzo FROM cte_LegaleRappresentante WHERE cte_LegaleRappresentante.Piva = Imprese.Piva), '') AS LegaleRappresentante_LuogoNascita ")

                Join.bListaProvince = True
                Join.bCentriAziendali = True
                Join.bCentrixIndirizzi = True
                Join.bIndirizziCentro = True
                Join.bIstatCentro = True
                Join.bImpresexParticelle = True
                Join.bImpreseCodici_CodiceCuaa = True
                Join.bImpreseCodici_CodiceTecnico = True
                Join.bImpreseCodici_CodiceCertificazione = True
                Join.bGruppiRaccolta = True
                Join.bGerarchiaImprese = True

                Join.bImpreseCodici_UltimaVerificaIspettiva = True
                Join.bImpreseCodici_SAUTotAziendale = True
                Join.bListaRegioni = True

#End Region
            Case enum_TipoSelect_FiltroneSuperNova.Appezzamenti,  '4 'Appezzamenti
                 enum_TipoSelect_FiltroneSuperNova.AppezzamentiRipartoCatasto
#Region "Appezzamenti"
                stb.AppendLine(" SELECT DISTINCT (Imprese.PIVA + '_' + CAST(Centri_Aziendali.sa_cod AS VARCHAR(20))+ '_' + CAST(Appezzamento.Appezza AS VARCHAR(20))) AS chiave, ")

                stb.AppendLine("        Imprese.PIVA, Imprese.rag_soc, ")
                stb.AppendLine("        Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
                stb.AppendLine("        Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine("        Appezzamento.APPEZZA ")
                stb.AppendLine("  , Appezzamento.SUP_APP ")
                If (fattConv > 1) Then
                    stb.AppendLine("  , ROUND(ISNULL(Appezzamento.SUP_APP,0) * " & fattConv.ToString().Replace(",", ".") & " , 4) as acro")
                End If
                stb.AppendLine("        , Appezzamento.APP_NOME, Appezzamento.Campo_Cod, Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine, ")
                stb.AppendLine("        CASE WHEN ZonexParticelle.Zona_Cod IS NULL THEN '' ELSE 'ZVN' END AS ZVN ")

                stb.AppendLine("        , Appezzamento.Username_Creazione, Appezzamento.Username_Modifica, CONVERT(DATE, Appezzamento.Data_Creazione) AS Data_Creazione, CONVERT(DATE, Appezzamento.Data_Modifica) AS Data_Modifica ")

                If Not IsNothing(objParametriUtenti) Then
                    If sql2017 Then
                        stb.AppendLine("        , TRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, '')) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , TRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, '')) AS Username_Modifica_Completo ")
                    Else
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, ''))) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, ''))) AS Username_Modifica_Completo")
                    End If
                Else
                    stb.AppendLine("        , Appezzamento.Username_Creazione AS Username_Creazione_Completo ")
                    stb.AppendLine("        , Appezzamento.Username_Modifica AS Username_Modifica_Completo ")
                End If
                stb.AppendLine("        , IC_Cuaa.val_cod AS CodiceCuaa ")
                stb.AppendLine("        , ISNULL((SELECT TOP 1 Cognome + ' ' + Nome + ' ' + Rag_Soc FROM Contatti WHERE Cod_Contatto = IC_Tecnico.val_cod), '') AS TecnicoRifImpresa")
                stb.AppendLine("        , Appezzamento.X AS Latitudine, Appezzamento.Y AS Longitudine ")
                stb.AppendLine("        , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS Gruppi_Raccolta")

                If Join.bGerarchiaImprese AndAlso sql2017 Then
                    stb.AppendLine("  , g.Padri, g.rag_soc_s AS Referenti ")
                End If

                Join.bListaProvince = True
                Join.bCentriAziendali = True
                Join.bCampi = True
                Join.bAppezzamento = True
                Join.bImpreseCodici_CodiceCuaa = True
                Join.bImpreseCodici_CodiceTecnico = True
                Join.bGruppiRaccolta = True
                Join.bGerarchiaImprese = True

#End Region
            Case enum_TipoSelect_FiltroneSuperNova.Impianti  '5 'Impianti
#Region "Impianti"
                stb.AppendLine("        SELECT DISTINCT (Imprese.PIVA + '_' + CAST(Centri_Aziendali.sa_cod AS VARCHAR(20))+ '_' + CAST(Appezzamento.Appezza  AS VARCHAR(20)) + '_' + CAST(Reg_Impianti.id_reg  AS VARCHAR(20)) + '_' + CAST(isnull(SpecieVegetali.veg_cod, '0') AS VARCHAR(20)) + '_' + CAST(isnull(Imprese_Progetti.Progetto_Cod  ,'0')  AS VARCHAR(20)) ) AS chiave, ")

                If Join.bGerarchiaImprese AndAlso sql2017 Then
                    stb.AppendLine("        COALESCE(g.Padri, '') AS Padri , COALESCE(g.rag_soc_s, '') AS Referenti, ")
                End If

                stb.AppendLine("        Imprese.PIVA, Imprese.rag_soc, Lista_Province.PROVINCIA, ")
                stb.AppendLine("        Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, ")
                stb.AppendLine("        SpecieVegetali.Veg_Cod, Reg_Impianti.Cul_Cod, dbo.Reg_Impianti.GRVA_Cod_VEG, ")
                stb.AppendLine("        SpecieVegetali.Veg_Des, Cultivar.Cul_Des, GruppoVarietale.GRVA_DES, ")
                stb.AppendLine("        Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine("        Appezzamento.APPEZZA ")

                stb.AppendLine("  , Appezzamento.SUP_APP ")
                If fattConv > 1 Then
                    stb.AppendLine("  ,  ROUND(ISNULL(Appezzamento.SUP_APP,0) * " & fattConv.ToString().Replace(",", ".") & ", 4) as acro")
                End If

                stb.AppendLine("        , Appezzamento.APP_NOME, ")
                stb.AppendLine("        Reg_Impianti.ID_REG, Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine, ")
                stb.AppendLine("        Reg_Impianti.Sup_Imp, Imprese_Progetti.Regolamento_Cod AS Regolamento, Imprese_Progetti.Disciplinare_Cod AS Finanziamento, Reg_Impianti.Grfi_Cod, Reg_Impianti.Grva_Cod_Veg, Reg_Impianti.Cop_Cod, ")
                stb.AppendLine("        Imprese_Progetti.Progetto_Cod, Imprese_Progetti.Progetto_Nome, CASE WHEN Imprese_Progetti.data_fine_prevista = " & Agro_SQL_SaveDate(AGRODATAFINE) & " THEN NULL ELSE Imprese_Progetti.data_fine_prevista END AS Data_Fine_Prevista, ")
                stb.AppendLine("        Imprese_Progetti.Validita_Inizio AS Progetto_Validita_Inizio, Imprese_Progetti.Validita_Fine AS Progetto_Validita_Fine, IC.val_cod AS CodiceSocio, IC_Cuaa.val_cod AS CodiceCuaa ")

                stb.AppendLine("        , Reg_Impianti.Username_Creazione, Reg_Impianti.Username_Modifica, CONVERT(DATE, Reg_Impianti.Data_Creazione) AS Data_Creazione, CONVERT(DATE, Reg_Impianti.Data_Modifica) AS Data_Modifica")
                ' VAnni: 11/2/2020: ora è esteso attraverso permesso utente, non più personalizzato su Riunite.
                ''Grilli: Modifica volante per Valerio che probabilmente resterà qui per sempre... (RiuniteCiv)
                ''If objParametri.PivaSuperUser = "00127310357" AndAlso jsonRichiesta.IncludiVisite THEN
                If jsonRichiesta.IncludiVisite Then
                    stb.AppendLine("        , riuniteCiv.dataVisita, riuniteCiv.punteggioVisita ")
                End If

                'Centro_Conferimento
                stb.AppendLine("        , ISNULL(f_magConf.Fabbricato_Des,'') AS Fabbricato_Des ")
                stb.AppendLine("        , Centro_Conferimento.val_cod AS Centro_Conferimento ")
                stb.AppendLine("        , CASE Reg_Impianti.cul_cod WHEN 0 ")
                stb.AppendLine("            THEN ISNULL(Codici_Anagrafe.descrizione, 'Terreno Nudo') ")
                stb.AppendLine("             ELSE SpecieVegetali.Veg_Des END AS [Utilizzo] ")

                If Not IsNothing(objParametriUtenti) Then
                    If sql2017 Then
                        stb.AppendLine("        , TRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, '')) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , TRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, '')) AS Username_Modifica_Completo ")
                    Else
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, ''))) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, ''))) AS Username_Modifica_Completo")
                    End If
                Else
                    stb.AppendLine("        , Reg_Impianti.Username_Creazione AS Username_Creazione_Completo ")
                    stb.AppendLine("        , Reg_Impianti.Username_Modifica AS Username_Modifica_Completo ")
                End If

                stb.AppendLine("        , IC_Cuaa.val_cod AS CodiceCuaa ")
                stb.AppendLine("        , ISNULL((SELECT TOP 1 Cognome + ' ' + Nome + ' ' + Rag_Soc FROM Contatti WHERE Cod_Contatto = IC_Tecnico.val_cod), '') AS TecnicoRifImpresa")
                stb.AppendLine("        , Appezzamento.X AS Latitudine, Appezzamento.Y AS Longitudine ")
                stb.AppendLine("        , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS Gruppi_Raccolta")

                Join.bListaProvince = True
                Join.bCentriAziendali = True
                Join.bSpecieVegetali = True
                Join.bRegImpianti = True
                Join.bCultivar = True
                Join.bGruppoVarietale = True
                Join.bCampi = True
                Join.bAppezzamento = True
                Join.bImpreseProgetti = True
                Join.bImpreseCodici_CodiceSocio = True
                Join.bImpreseCodici_CodiceCuaa = True
                Join.bImpreseCodici_CodiceTecnico = True
                Join.bGruppiRaccolta = True

#End Region
            Case enum_TipoSelect_FiltroneSuperNova.Movimenti  '6 'Movimenti
#Region "Movimenti"

                stb.AppendLine(" SELECT DISTINCT (Imprese.PIVA + '_' + CAST(Centri_Aziendali.sa_cod AS VARCHAR(20))+ '_' + CAST(Appezzamento.Appezza AS VARCHAR(20)) + '_' + CAST(Reg_Impianti.id_reg AS VARCHAR(20)) + '_' + CAST(isnull(SpecieVegetali.veg_cod, '0') AS VARCHAR(20)) + '_' + CAST(Agenda.Id_Agenda AS VARCHAR(20)) ) AS chiave, ")
                stb.AppendLine(" Imprese.PIVA, Imprese.rag_soc, ")
                stb.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
                stb.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine(" Appezzamento.APPEZZA ")

                If (fattConv < 2) Then
                    stb.AppendLine("  , Appezzamento.SUP_APP ")
                Else
                    stb.AppendLine("  , Appezzamento.SUP_APP * " & fattConv.ToString().Replace(",", ".") & " AS acro")
                End If

                stb.AppendLine(" , Appezzamento.APP_NOME, ")
                stb.AppendLine(" Reg_Impianti.ID_REG, Reg_Impianti.Sup_Imp, ")
                stb.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, (select lav_des from operazioni where lav_cod = Agenda.Lav_Cod) AS Lav_des, Agenda.Des_Lib, Agenda.Username_Creazione, Agenda.Username_Modifica, Agenda.Data_Creazione, Agenda.Data_Modifica, Agenda.Validita_Inizio, ")
                stb.AppendLine(" Movimenti.Data_Movimento, Movimenti.Cau_Mov, Movimenti.Num_Protocollo, ")
                stb.AppendLine(" SpecieVegetali.VEG_COD, SpecieVegetali.Veg_Des, Reg_Impianti.Cul_Cod, Cultivar.Cul_Des  ")

                stb.AppendLine(" ,Movimenti.Username_Creazione, Movimenti.Username_Modifica, Movimenti.Data_Creazione, Movimenti.Data_Modifica ")

                If Not IsNothing(objParametriUtenti) Then
                    If sql2017 Then
                        stb.AppendLine(" , TRIM(COALESCE(udca.nome, '') + ' ' + COALESCE(udca.Cognome, '') + ' ' + COALESCE(udca.Rag_Soc, '')) AS Username_Creazione_Agenda_Completo ")
                        stb.AppendLine("  , TRIM(COALESCE(udma.nome, '') + ' ' + COALESCE(udma.Cognome, '') + ' ' + COALESCE(udma.Rag_Soc, '')) AS Username_Modifica_Agenda_Completo ")
                        stb.AppendLine(" , TRIM(COALESCE(udcm.nome, '') + ' ' + COALESCE(udcm.Cognome, '') + ' ' + COALESCE(udcm.Rag_Soc, '')) AS Username_Creazione_Movimenti_Completo ")
                        stb.AppendLine("  , TRIM(COALESCE(udmm.nome, '') + ' ' + COALESCE(udmm.Cognome, '') + ' ' + COALESCE(udmm.Rag_Soc, '')) AS Username_Modifica_Movimenti_Completo ")
                    Else
                        stb.AppendLine(" , LTRIM(RTRIM(COALESCE(udca.nome, '') + ' ' + COALESCE(udca.Cognome, '') + ' ' + COALESCE(udca.Rag_Soc, ''))) AS Username_Creazione_Agenda_Completo ")
                        stb.AppendLine("  , LTRIM(RTRIM(COALESCE(udma.nome, '') + ' ' + COALESCE(udma.Cognome, '') + ' ' + COALESCE(udma.Rag_Soc, ''))) AS Username_Modifica_Agenda_Completo ")
                        stb.AppendLine(" , LTRIM(RTRIM(COALESCE(udcm.nome, '') + ' ' + COALESCE(udcm.Cognome, '') + ' ' + COALESCE(udcm.Rag_Soc, ''))) AS Username_Creazione_Movimenti_Completo ")
                        stb.AppendLine("  , LTRIM(RTRIM(COALESCE(udmm.nome, '') + ' ' + COALESCE(udmm.Cognome, '') + ' ' + COALESCE(udmm.Rag_Soc, ''))) AS Username_Modifica_Movimenti_Completo ")
                    End If
                Else
                    stb.AppendLine(" , Agenda.Username_Creazione AS Username_Creazione_Agenda_Completo ")
                    stb.AppendLine(" , Agenda.Username_Modifica AS Username_Modifica_Agenda_Completo ")
                    stb.AppendLine(" , Movimenti.Username_Creazione AS Username_Creazione_Movimenti_Completo ")
                    stb.AppendLine(" , Movimenti.Username_Modifica AS Username_Modifica_Movimenti_Completo ")
                End If

                stb.AppendLine(" , IC_Cuaa.val_cod AS CodiceCuaa ")
                stb.AppendLine(" , ISNULL((SELECT TOP 1 Cognome + ' ' + Nome + ' ' + Rag_Soc FROM Contatti WHERE Cod_Contatto = IC_Tecnico.val_cod), '') AS TecnicoRifImpresa")
                stb.AppendLine(" , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS Gruppi_Raccolta")

                If Join.bGerarchiaImprese AndAlso sql2017 Then
                    stb.AppendLine("  , g.Padri, g.rag_soc_s AS Referenti ")
                End If

                Join.bCentriAziendali = True
                Join.bCampi = True
                Join.bAppezzamento = True
                Join.bRegImpianti = True
                Join.bAgenda = True
                Join.bMovimenti = True
                Join.bSpecieVegetali = True
                Join.bCultivar = True
                Join.bImpreseCodici_CodiceCuaa = True
                Join.bListaProvince = True
                Join.bImpreseCodici_CodiceTecnico = True
                Join.bGruppiRaccolta = True
                Join.bGerarchiaImprese = True

#End Region
            Case enum_TipoSelect_FiltroneSuperNova.Contatti  '7 'Contatti
#Region "Contatti"
                stb.AppendLine(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, ")
                stb.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine(" Contatti.Cod_Contatto, Contatti.Rag_Soc AS Contatto_Rag_Soc,")
                stb.AppendLine(" Risorse_Umane.Cod_Rapporto, Risorse_Umane.Cod_RisUm, Risorse_Umane.Piva, Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, Risorse_Umane.Validita_Fine AS Cessazione,")
                stb.AppendLine(" Rapporti_Contabili.Rapporto_Des")

                Join.bCampi = True
                Join.bContatti = True
                Join.bRisorseUmane = True
                Join.bRapportiContabili = True
#End Region
            Case enum_TipoSelect_FiltroneSuperNova.Campi  '8 'Campi
#Region "Campi"
                'stb.AppendLine(" SELECT DISTINCT Imprese.PIVA, Imprese.rag_soc, " )
                'stb.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome," )
                'stb.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des," )
                'stb.AppendLine(" Campi.Validita_Inizio, Campi.Validita_Fine " )

                'Join.bCentriAziendali = True
                'Join.bCampi = True

                stb.AppendLine(" SELECT DISTINCT (Imprese.PIVA + '_' + CAST(Centri_Aziendali.sa_cod AS VARCHAR(20))+ '_' + CAST(Campi.Campo_Cod AS VARCHAR(20))) AS chiave, ")
                stb.AppendLine(" Imprese.PIVA, Imprese.rag_soc, ")
                stb.AppendLine(" IC.val_cod AS CodiceSocio, IC_Cuaa.val_cod AS CodiceCuaa, ")
                stb.AppendLine(" Indirizzi.ind_des + ' ' + Indirizzi.Cap + ' ' + ISNULL(Istat.LOCALITA, '') + ' ' + ISNULL(Istat.COMUNI_PROV, '') + ' ' + Indirizzi.Stato AS Indirizzo_Azienda, ")
                'stb.AppendLine(" ISNULL ((SELECT TOP 1 Contatti.Cognome + ' ' + Contatti.Nome FROM Imprese_Codici INNER JOIN Contatti ON Imprese_Codici.val_cod = Contatti.Cod_Contatto WHERE Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.Tecnico & " AND Imprese_Codici.piva=Imprese.piva), ' ') AS Tecnico_Referente, ")
                stb.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, ")
                stb.AppendLine(" Indirizzi_Centro.ind_des AS Indirizzo_Centro, ISNULL(Istat_Centro.LOCALITA, '') AS Comune_Centro, ISNULL(Istat_Centro.COMUNI_PROV, '') AS Provincia_Centro, Indirizzi_Centro.Cap AS CAP_Centro, Indirizzi_Centro.Stato AS Stato_Centro, ")
                stb.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine(" Campi.Validita_Inizio, Campi.Validita_Fine, ")
                stb.AppendLine(" CC1279.val_Cod AS Codice_Campo, ")
                stb.AppendLine(" IC1324.val_Cod AS Contratto_Produzione, ")
                stb.AppendLine(" CC1325.val_Cod AS Sup_Contratto, ")
                stb.AppendLine(" CC1326.val_Cod AS Filiera, ")
                stb.AppendLine(" SpecieVegetali.VEG_COD, SpecieVegetali.Veg_Des, GruppoVegetale.Gru_Cod, GruppoVegetale.Gru_Des,  ")
                stb.AppendLine(" Campi.Username_Creazione, Campi.Username_Modifica, CONVERT(DATE, Campi.Data_Creazione) AS Data_Creazione, CONVERT(DATE, Campi.Data_Modifica) AS Data_Modifica ")

                If Not IsNothing(objParametriUtenti) Then
                    If sql2017 Then
                        stb.AppendLine(" , TRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, '')) AS Username_Creazione_Completo ")
                        stb.AppendLine(" , TRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, '')) AS Username_Modifica_Completo ")
                    Else
                        stb.AppendLine(" , LTRIM(RTRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, ''))) AS Username_Creazione_Completo ")
                        stb.AppendLine(" , LTRIM(RTRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, ''))) AS Username_Modifica_Completo")
                    End If
                Else
                    stb.AppendLine(" , Campi.Username_Creazione AS Username_Creazione_Completo ")
                    stb.AppendLine(" , Campi.Username_Modifica AS Username_Modifica_Completo ")
                End If

                stb.AppendLine(" , ISNULL((SELECT TOP 1 Cognome + ' ' + Nome + ' ' + Rag_Soc FROM Contatti WHERE Cod_Contatto = IC_Tecnico.val_cod), '') AS TecnicoRifImpresa")
                stb.AppendLine(" , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS Gruppi_Raccolta")

                If Join.bGerarchiaImprese AndAlso sql2017 Then
                    stb.AppendLine("  , g.Padri, g.rag_soc_s AS Referenti ")
                End If

                Join.bCampi = True
                Join.bCampiCodici = True
                Join.bIndirizzi = True
                Join.bIstat = True
                Join.bIndirizziCentro = True
                Join.bIstatCentro = True
                Join.bImpreseCodici_CodiceSocio = True
                Join.bImpreseCodici_CodiceCuaa = True
                Join.bImpreseCodici_CodiceTecnico = True
                Join.bGruppiRaccolta = True
                Join.bGerarchiaImprese = True

#End Region
            Case enum_TipoSelect_FiltroneSuperNova.Esercizi  '10 'Esercizi
#Region "Esercizi"
                'Anna 06/06/21 Aggiunte nuove colonne per TTI - STRINGHE PER COMPATTARE IL CODICE
                Dim data_nascita As String = " ISNULL((SELECT TOP 1 data_nascita 
			                                           FROM Imprese_Codici  
			                                           INNER JOIN Contatti ON Contatti.Cod_Contatto = Imprese_Codici.val_cod   
                                                       INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva 
                                                       WHERE (Imprese_Codici.Piva = Reg_Impianti.Piva) 
                                                       AND (Imprese_Codici.Id_Cod = " & enum_CodiciAnagrafe.Tecnico & ")),'') "

                Dim metodo_produzione As String = " ISNULL((SELECT TOP 1 val_cod 
                                                            FROM [Appezzamento_Codici] 
						                                    WHERE Piva = Appezzamento.Piva 
						                                    AND Sa_Cod = Appezzamento.Sa_Cod 
						                                    AND Appezza = Appezzamento.Appezza 
						                                    AND appezzamento_codici.id_cod = " & enum_CodiciAnagrafe.MetodoDiProduzione & "), '')"

                stb.AppendLine(" ;WITH cte_LegaleRappresentante AS ( ")
                stb.AppendLine("    SELECT")
                stb.AppendLine("          Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS LegaleRappresentante ")
                stb.AppendLine("        , Data_Nascita ")
                stb.AppendLine("        , Codice_Fiscale ")
                stb.AppendLine("        , Risorse_Umane.Piva ")
                stb.AppendLine("        , ind_des AS Indirizzo ")
                stb.AppendLine("    FROM Risorse_Umane ")
                stb.AppendLine("    INNER JOIN Contatti ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto ")
                stb.AppendLine("    INNER JOIN ContattiXIndirizzi ON ContattiXIndirizzi.Cod_Contatto = Contatti.Cod_Contatto and Tipo_Indirizzo = " & enum_IndirizzoTipo.LuogoNascita & " ")
                stb.AppendLine("    INNER JOIN Indirizzi ON Indirizzi.cod_indirizzo = ContattiXIndirizzi.Cod_Indirizzo ")
                stb.AppendLine("    WHERE Risorse_Umane.Cod_Rapporto = " & enum_Rapporti_Contabili_Standard.Legale_Rappresentante & " ")
                stb.AppendLine(" ) ")
                stb.AppendLine(" SELECT DISTINCT  ( ")

                stb.AppendLine("    Imprese.PIVA + '_' + CAST(Centri_Aziendali.sa_cod AS VARCHAR(20)) + '_' + CAST(Appezzamento.Appezza AS VARCHAR(20)) + '_' + CAST(Reg_Impianti.id_reg AS VARCHAR(20)) + '_' + CAST(isnull(SpecieVegetali.veg_cod, '0') AS VARCHAR(20)) + '_' + CAST(isnull(Imprese_Progetti.Progetto_Cod, '0') AS VARCHAR(20)) ) AS chiave ")
                stb.AppendLine("  , Imprese.PIVA ")
                stb.AppendLine("  , Imprese.rag_soc ")

                If Join.bGerarchiaImprese AndAlso sql2017 Then
                    stb.AppendLine("  , g.Padri, g.rag_soc_s AS Referenti ")
                End If

                stb.AppendLine("  , Lista_Province.PROVINCIA ")
                stb.AppendLine("  , Centri_Aziendali.sa_cod ")
                stb.AppendLine("  , Centri_Aziendali.sa_nome ")
                stb.AppendLine("  , SpecieVegetali.Veg_Cod ")
                stb.AppendLine("  , Reg_Impianti.Cul_Cod ")
                stb.AppendLine("  , dbo.Reg_Impianti.GRVA_Cod_VEG ")
                stb.AppendLine("  , SpecieVegetali.Veg_Des ")
                stb.AppendLine("  , Cultivar.Cul_Des ")
                stb.AppendLine("  , GruppoVarietale.GRVA_DES ")
                stb.AppendLine("  , Campi.Campo_Cod ")
                stb.AppendLine("  , Campi.Campo_Des ")
                stb.AppendLine("  , Appezzamento.APPEZZA ")
                stb.AppendLine("  , Appezzamento.SUP_APP ")
                If (fattConv > 1) Then
                    stb.AppendLine("  , ROUND(ISNULL(Appezzamento.SUP_APP,0) * " & fattConv.ToString().Replace(",", ".") & ", 4) as acro")
                End If
                stb.AppendLine("  , Appezzamento.APP_NOME ")
                stb.AppendLine("  , Reg_Impianti.ID_REG ")
                stb.AppendLine("  , Imprese_Progetti.Validita_Inizio ")
                stb.AppendLine("  , Imprese_Progetti.Validita_Fine ")
                stb.AppendLine("  , Reg_Impianti.Sup_Imp ")
                If (fattConv > 1) Then
                    stb.AppendLine("  , ROUND(ISNULL(Reg_Impianti.Sup_Imp ,0) * " & fattConv.ToString().Replace(",", ".") & ", 4) as Sup_Imp_acro")
                End If
                stb.AppendLine("  , Imprese_Progetti.Regolamento_Cod AS Regolamento ")
                stb.AppendLine("  , Imprese_Progetti.Disciplinare_Cod AS Finanziamento ")
                stb.AppendLine("  , Imprese_Progetti.Produzione_Prevista AS Resa ")
                stb.AppendLine("  , Reg_Impianti.Grfi_Cod ")
                stb.AppendLine("  , Reg_Impianti.Grva_Cod_Veg ")
                stb.AppendLine("  , Reg_Impianti.Cop_Cod ")
                stb.AppendLine("  , Imprese_Progetti.Progetto_Cod ")
                stb.AppendLine("  , Imprese_Progetti.Progetto_Nome AS Lotto ")
                stb.AppendLine("  , Imprese_Progetti.Progetto_Des AS Descrizione ")
                stb.AppendLine("  , Imprese_Progetti.Validita_Inizio AS Progetto_Validita_Inizio ")
                stb.AppendLine("  , Imprese_Progetti.Validita_Fine AS Progetto_Validita_Fine ")
                stb.AppendLine("  , IC.val_cod AS CodiceSocio ")
                stb.AppendLine("  , IC_Cuaa.val_cod AS CodiceCuaa ")
                stb.AppendLine("  , kpin.val_Cod AS kpin ")
                stb.AppendLine("  , blockName.val_Cod AS blockName")
                stb.AppendLine("  , growernumber.val_Cod AS GrowerNumber")

                ' Anna 27/07/21 - Aggiunte nuove colonne alla griglia degli esercizi
                stb.AppendLine("  , impreseOrgRef.rag_soc AS organismoReferente")

                stb.AppendLine("  , CASE WHEN AppIndirizzi.stato IN ('ITALIA','Italy','It','Italia','italiano') ")
                stb.AppendLine("         THEN 'ITALIA' ")
                stb.AppendLine("     ELSE AppIndirizzi.stato ")
                stb.AppendLine("     END AS nazioneKPIN ")
                stb.AppendLine("  , UPPER(lista_regioni.Regione_Des) AS regioneKPIN ")
                stb.AppendLine("  , UPPER(ProvinciaKPIN2.sigla) AS provinciaKPIN ")
                stb.AppendLine("  , UPPER(AppIndirizzi.com_des) AS comuneKPIN ")
                stb.AppendLine("  , UPPER(AppIndirizzi.frz_des) AS frazioneKPIN ")
                stb.AppendLine("  , UPPER(AppIndirizzi.ind_des) AS indirizzoKPIN ")
                stb.AppendLine("  , Imprese_progetti.P_HA AS nmPiante ")
                stb.AppendLine("  , Imprese_progetti.P_HA_Femmine AS nmPianteF ")
                stb.AppendLine("  , CASE WHEN Reg_Impianti.Piante_Maschi_InSesto = 0 ")
                stb.AppendLine("        THEN 'NO' ")
                stb.AppendLine("    ELSE CASE WHEN Reg_Impianti.Piante_Maschi_InSesto = 1 ")
                stb.AppendLine("        THEN 'SI' ")
                stb.AppendLine("        END END AS maschiInSesto ")

                stb.AppendLine("  , CASE WHEN Reg_Impianti.Data_Inizio_Innesto = CAST('1900-01-01' AS date) ")
                stb.AppendLine("        THEN NULL ")
                stb.AppendLine("    ELSE Reg_Impianti.Data_Inizio_Innesto ")
                stb.AppendLine("    END AS annoInnestoVarieta ")

                stb.AppendLine("  , CASE WHEN Reg_Impianti.Data_Inizio_Portinnesto = CAST('1900-01-01' AS date) ")
                stb.AppendLine("        THEN NULL ")
                stb.AppendLine("    ELSE Reg_Impianti.Data_Inizio_Portinnesto ")
                stb.AppendLine("    END AS annoInizioPortinnesto ")

                stb.AppendLine("  , CASE WHEN Reg_Impianti.Data_Inizio_Produzione = CAST('2100-12-31' AS date) ")
                stb.AppendLine("        THEN NULL ")
                stb.AppendLine("    ELSE Reg_Impianti.Data_Inizio_Produzione ")
                stb.AppendLine("    END AS annoPrimaProduzione ")

                stb.AppendLine("  , portinnesti.port_des AS Portinnesto ")
                stb.AppendLine("  , formeallevamento.foral_des AS formaAllevamento ")
                stb.AppendLine("  , copertura.cop_des AS Copertura ")
                stb.AppendLine("  , impiantiirrigazioni.imp_des AS impiantoIrriguo ")
                stb.AppendLine("  , OTabelle_Parametri.descrizione AS Licenza ")

                'Anna 06/06/21 Aggiunte nuove colonne per TTI
                stb.AppendLine("  , ESPOSIZ AS Esposizione")
                stb.AppendLine("  , PENDE AS Pendenza")

                stb.AppendLine("  , ISNULL((SELECT TOP 1 numero ")
                stb.AppendLine("            FROM Imprese_Codici ")
                stb.AppendLine("                INNER JOIN Contatti ON Contatti.Cod_Contatto = Imprese_Codici.val_cod")
                stb.AppendLine("                INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva ")
                stb.AppendLine("                INNER JOIN ContattiXRubrica ON CONTATTI.Cod_Contatto = ContattiXRubrica.Cod_Contatto")
                stb.AppendLine("                INNER JOIN rubrica ON ContattiXRubrica.cod_rubrica = rubrica.cod_rubrica")
                stb.AppendLine("            WHERE (Imprese_Codici.Id_Cod = " & enum_CodiciAnagrafe.Tecnico & ")")
                stb.AppendLine("            AND descr in ('Email:', 'mail', 'Email', 'E-Mail:')), '') AS IndirizzoEmailTecnico")

                stb.AppendLine("  , ISNULL((SELECT TOP 1 numero ")
                stb.AppendLine("                FROM Imprese_Codici ")
                stb.AppendLine("                    INNER JOIN Contatti ON Contatti.Cod_Contatto = Imprese_Codici.val_cod")
                stb.AppendLine("                    INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva ")
                stb.AppendLine("                    INNER JOIN ContattiXRubrica ON CONTATTI.Cod_Contatto = ContattiXRubrica.Cod_Contatto")
                stb.AppendLine("                    INNER JOIN rubrica ON ContattiXRubrica.cod_rubrica = rubrica.cod_rubrica")
                stb.AppendLine("                WHERE (Imprese_Codici.Id_Cod = " & enum_CodiciAnagrafe.Tecnico & ")")
                stb.AppendLine("                AND descr IN ('Cellulare', 'personale', 'Telefono','elefono', 'ellulare', 'ax', 'Fax:', 'N. Telefono:')), '') AS CellulareTecnico")

                'SOSTITUITO CON TecnicoRifImpresa
                'stb.AppendLine("  , ISNULL((SELECT TOP 1 Contatti.Rag_Soc + ' ' + Contatti.Nome + ' ' + Contatti.Cognome AS TecnicoReferente ")
                'stb.AppendLine("                FROM Imprese_Codici ")
                'stb.AppendLine("                    INNER JOIN Contatti ON Contatti.Cod_Contatto = Imprese_Codici.val_cod")
                'stb.AppendLine("                    INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva ")
                'stb.AppendLine("                WHERE (Imprese_Codici.Id_Cod = " & enum_CodiciAnagrafe.Tecnico & ")")
                'stb.AppendLine("                AND (Imprese_Codici.Piva = Reg_Impianti.Piva)), '') AS TecnicoReferente")

                stb.AppendLine("  , ISNULL((SELECT TOP 1 sesso ")
                stb.AppendLine("                FROM Imprese_Codici ")
                stb.AppendLine("                    INNER JOIN Contatti ON Contatti.Cod_Contatto = Imprese_Codici.val_cod")
                stb.AppendLine("                    INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva ")
                stb.AppendLine("                WHERE (Imprese_Codici.Id_Cod = " & enum_CodiciAnagrafe.Tecnico & ")")
                stb.AppendLine("                AND (Imprese_Codici.Piva = Reg_Impianti.Piva)), '') AS GenereTecnico")

                stb.AppendLine("  , CASE WHEN " & data_nascita & " = CAST('1900-01-01' AS DATE) ")
                stb.AppendLine("      THEN NULL ")
                stb.AppendLine("    ELSE" & data_nascita & " ")
                stb.AppendLine("    END AS DataNascitaTecnico ")

                '16/03/2022 - Aggiunta nuova colonna: DISCIPLINARE
                stb.AppendLine("  , DPI_Regolamenti.Des AS Disciplinare")

                'stb.AppendLine("  , ISNULL(Imprese_1.Piva, '') AS PIVA_padre")

                'Anagrafica Imprese
                stb.AppendLine("  , ISNULL(IndImp.ind_des, '') AS imp_ind_des")
                stb.AppendLine("  , ISNULL(IndImp.frz_des, '') AS imp_frz_des")
                stb.AppendLine("  , ISNULL(IndImp.CAP, '') AS imp_cap")
                stb.AppendLine("  , ISNULL(IstatImp.LOCALITA, '') AS imp_com_des")
                stb.AppendLine("  , ISNULL(IstatImp.COMUNI_PROV, '') AS imp_pro_cod ")

                'Anagrafica Centro
                stb.AppendLine("  , ISNULL(Indirizzi_Centro.ind_des, '') AS cen_ind_des")
                stb.AppendLine("  , ISNULL(Indirizzi_Centro.frz_des, '') AS cen_frz_des")
                stb.AppendLine("  , ISNULL(Indirizzi_Centro.CAP, '') AS cen_cap")
                stb.AppendLine("  , ISNULL(Istat_Centro.LOCALITA, '') AS cen_com_des")
                stb.AppendLine("  , ISNULL(Istat_Centro.COMUNI_PROV, '') AS cen_pro_cod ")
                stb.AppendLine("  , ISNULL(Indirizzi_Centro.pro_cod_istat, '') AS pro_cod_istat")
                stb.AppendLine("  , ISNULL(Indirizzi_Centro.com_cod_istat, '') AS com_cod_istat")
                stb.AppendLine("  , ISNULL(Centri_Aziendali.sa_cod, 0) AS codice_centro")

                stb.AppendLine("  , (CONVERT(NVARCHAR, ISNULL(Appezzamento.Validita_Inizio, '01/01/1900'), 103) + ' - ' + CONVERT(NVARCHAR, ISNULL(Appezzamento.Validita_Fine, '31/12/2100'), 103)) AS DurataAppezzamento")
                stb.AppendLine("  , CONVERT(NVARCHAR, ISNULL(Appezzamento.Validita_Inizio, '01/01/1900'), 103) AS InizioDurataAppezzamento")
                stb.AppendLine("  , CONVERT(NVARCHAR, ISNULL(Appezzamento.Validita_Fine, '31/12/2100'), 103) AS FineDurataAppezzamento")

                stb.AppendLine("  , CASE WHEN " & metodo_produzione & " = 1 THEN 'Integrato'")
                stb.AppendLine("    ELSE CASE WHEN " & metodo_produzione & " = 2 THEN 'In Conversione'")
                stb.AppendLine("    ELSE CASE WHEN " & metodo_produzione & " = 3 THEN 'Biologico'")
                stb.AppendLine("        ELSE '' ")
                stb.AppendLine("    END END END AS Metodo_Produzione")

                stb.AppendLine("  , ISNULL(GruppoVegetale.Gru_Des, '') AS GruppoVegetale")
                stb.AppendLine("  , ISNULL(GruppoFinalita.Grfi_Des, '') AS Finalita")
                stb.AppendLine("  , ISNULL((SELECT TOP 1 Codici_Anagrafe.Descrizione ")
                stb.AppendLine("  				 FROM Reg_Impianti_Codici ")
                stb.AppendLine("  				 INNER JOIN Codici_Anagrafe ON Reg_Impianti_Codici.id_cod = Codici_Anagrafe.codice ")
                stb.AppendLine("  				 WHERE (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva)")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza)")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Progetto_Cod = 0) ")
                stb.AppendLine("  				 AND (Codici_Anagrafe.gruppo = 'TERRENO')) , '') AS DestinazioneUso ")

                stb.AppendLine("  	, (CONVERT(NVARCHAR, ISNULL(Reg_Impianti.Validita_Inizio, '01/01/1900'), 103) + ' - ' + CONVERT(NVARCHAR, ISNULL(Reg_Impianti.Validita_Fine, '31/12/2100'), 103)) AS DurataImpianto")
                stb.AppendLine("  	, CONVERT(NVARCHAR, ISNULL(Reg_Impianti.Validita_Inizio, '01/01/1900'), 103) AS InizioDurataImpianto")
                stb.AppendLine("  	, CONVERT(NVARCHAR, ISNULL(Reg_Impianti.Validita_Fine, '31/12/2100'), 103) AS FineDurataImpianto")
                stb.AppendLine("  	, ISNULL((SELECT TOP 1 CONVERT(VARCHAR(10), (Agenda.Validita_Inizio), 103)")
                stb.AppendLine("                FROM Agenda ")
                stb.AppendLine("  				INNER JOIN Movimenti ")
                stb.AppendLine("					 ON Agenda.Piva = Movimenti.Piva  ")
                stb.AppendLine(" 					 AND Agenda.Sa_Cod = Movimenti.Sa_Cod ")
                stb.AppendLine(" 					 AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                stb.AppendLine(" 				INNER JOIN Movimenti_dettagli ")
                stb.AppendLine(" 					 ON Movimenti.Piva = Movimenti_dettagli.Piva ")
                stb.AppendLine(" 					 AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
                stb.AppendLine(" 					 AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda ")
                stb.AppendLine(" 					 AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
                stb.AppendLine("  				INNER JOIN Mov_Destinazioni ")
                stb.AppendLine(" 					 ON Movimenti_dettagli.Piva = Mov_Destinazioni.Piva")
                stb.AppendLine(" 					 AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
                stb.AppendLine(" 					 AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
                stb.AppendLine(" 					 AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov")
                stb.AppendLine(" 					 AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
                stb.AppendLine("  				 WHERE (Agenda.Lav_Cod = 2 OR Agenda.Lav_Cod = 71) ")
                stb.AppendLine(" 				 AND Movimenti.Cau_Mov = '" & enum_Agenda_Causali.LAVORAZIONE & "' ")
                stb.AppendLine("                 AND Mov_Destinazioni.Piva = Reg_Impianti.Piva ")
                stb.AppendLine(" 				 AND Mov_Destinazioni.sa_cod = Reg_Impianti.sa_cod ")
                stb.AppendLine(" 				 AND Mov_Destinazioni.appezza = Reg_Impianti.Appezza")
                stb.AppendLine(" 				 AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg")
                stb.AppendLine(" 				 ORDER BY Agenda.Validita_Inizio), '01/01/1900') AS DataSeminaTrapianto")

                'Anna
                stb.AppendLine(", CASE WHEN Imprese_Progetti.Data_Fioritura_Prevista IS NULL THEN CAST('2100-12-31' AS date) ELSE Imprese_Progetti.Data_Fioritura_Prevista END AS Data_Fioritura_Prevista")
                stb.AppendLine(", CASE WHEN Imprese_Progetti.Data_Inizio_Prevista IS NULL THEN CAST('1900-01-01' AS date) ELSE Imprese_Progetti.Data_Inizio_Prevista END AS Data_Semina_Prevista")
                stb.AppendLine(", CASE WHEN Imprese_Progetti.Data_Fine_Prevista IS NULL THEN CAST('2100-12-31' AS date) ELSE Imprese_Progetti.Data_Fine_Prevista END AS Data_Raccolta_Prevista")


                stb.AppendLine("  	, ISNULL(( SELECT TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des")
                stb.AppendLine("  				 FROM Reg_Impianti_Codici ")
                stb.AppendLine("  				 INNER JOIN CAC_Codifica_InfoAggiuntive")
                stb.AppendLine("  					ON Reg_Impianti_Codici.val_cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod")
                stb.AppendLine("  				 WHERE (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva)")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod)")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza)")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg)")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Progetto_Cod = 0) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato & ")")
                stb.AppendLine("  				 AND (CAC_Codifica_InfoAggiuntive.Argomento_Cod = 2)), '') AS DettaglioSpecie")

                stb.AppendLine("  	, ISNULL(( SELECT TOP 1 Reg_Impianti_Codici.val_cod ")
                stb.AppendLine("  				 FROM Reg_Impianti_Codici ")
                stb.AppendLine("  				 WHERE (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Progetto_Cod = 0) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Impianto_TraFila_Maschio & ")), 0) AS distanza_tra_fila")

                stb.AppendLine("  	, ISNULL(( SELECT TOP 1 Reg_Impianti_Codici.val_cod ")
                stb.AppendLine("  				 FROM Reg_Impianti_Codici ")
                stb.AppendLine("  				 WHERE (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Progetto_Cod = 0) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Impianto_SuFila_Maschio & ")), 0) AS distanza_su_fila ")

                stb.AppendLine("  	, (Imprese_progetti.P_HA * sup_imp) AS nmPianteTot")

                stb.AppendLine("  	, (CONVERT(NVARCHAR, ISNULL((SELECT TOP 1 CONVERT(VARCHAR(10), Imprese_Progetti.Validita_Inizio, 103)")
                stb.AppendLine("  									 FROM Imprese_Progetti ")
                stb.AppendLine("  									 WHERE (Imprese_Progetti.Piva = Reg_Impianti.Piva) ")
                stb.AppendLine("  									 AND (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  									 AND (Imprese_Progetti.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  									 AND (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  									 AND (Imprese_Progetti.Validita_Inizio <= CONVERT(DateTime,'2021/08/09', 103))), '01/01/1900'), 103) ")
                stb.AppendLine("  	+ ' - ' ")
                stb.AppendLine("  	+ CONVERT(NVARCHAR, ISNULL((SELECT TOP 1 CONVERT(VARCHAR(10), Imprese_Progetti.Validita_Fine, 103) ")
                stb.AppendLine("  								 FROM Imprese_Progetti ")
                stb.AppendLine("  								 WHERE (Imprese_Progetti.Piva = Reg_Impianti.Piva) ")
                stb.AppendLine("  								 AND (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  								 AND (Imprese_Progetti.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  								 AND (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  								 AND (Imprese_Progetti.Validita_Fine >= CONVERT(DateTime,'2021/08/09',103))), '31/12/2100'), 103)) AS DurataDistinta")


                stb.AppendLine("  	, ISNULL(( SELECT TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des ")
                stb.AppendLine("  				 FROM Imprese_Progetti ")
                stb.AppendLine("  				 INNER JOIN Reg_Impianti_Codici ")
                stb.AppendLine("  					ON Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva ")
                stb.AppendLine("  					AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod ")
                stb.AppendLine("  					AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza ")
                stb.AppendLine("  					AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg ")
                stb.AppendLine("  					AND Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod ")
                stb.AppendLine("  				 INNER JOIN CAC_Codifica_InfoAggiuntive ")
                stb.AppendLine("  				    ON Reg_Impianti_Codici.val_cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod")
                stb.AppendLine("  				 WHERE (Imprese_Progetti.Piva = Reg_Impianti.Piva) ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Capitolato_Privato & ") ")
                stb.AppendLine("  				 AND (CAC_Codifica_InfoAggiuntive.Argomento_Cod = 1) ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Validita_Inizio <= CONVERT(DateTime,'2021/08/09',120)) ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Validita_Fine >= CONVERT(DateTime,'2021/08/09',120))), '') AS capitolato_privato ")

                stb.AppendLine("  	, ISNULL((SELECT TOP 1 Fabbricati.Fabbricato_des ")
                stb.AppendLine("  				 FROM Imprese_Progetti ")
                stb.AppendLine("  				 INNER JOIN Reg_Impianti_Codici ")
                stb.AppendLine("  					 ON Imprese_Progetti.Piva = Reg_Impianti_Codici.Piva ")
                stb.AppendLine("  					 AND Imprese_Progetti.Sa_Cod = Reg_Impianti_Codici.sa_cod ")
                stb.AppendLine("  					 AND Imprese_Progetti.Appezza = Reg_Impianti_Codici.appezza ")
                stb.AppendLine("  					 AND Imprese_Progetti.Id_Reg = Reg_Impianti_Codici.Id_Reg   ")
                stb.AppendLine("  					 AND Imprese_Progetti.Progetto_Cod = Reg_Impianti_Codici.Progetto_Cod    ")
                stb.AppendLine("  				 INNER JOIN Fabbricati ")
                stb.AppendLine("  					ON CONVERT(VARCHAR(50),Fabbricati.Fabbricato_Cod) + ' | ' + CONVERT(VARCHAR(50),FABBRICATI.sa_COD) + ' | ' + Fabbricati.piva = Reg_Impianti_Codici.val_cod      ")
                stb.AppendLine("  				 WHERE (Imprese_Progetti.Piva = Reg_Impianti.Piva) ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_cod = " & enum_CodiciAnagrafe.Magazzino_Conferimento & ") ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Validita_Inizio <= CONVERT(DateTime,'2021/08/09',120)) ")
                stb.AppendLine("  				 AND (Imprese_Progetti.Validita_Fine >= CONVERT(DateTime,'2021/08/09',120))), '') AS magazzino_conf ")

                stb.AppendLine(" , Imprese_Progetti.Username_Creazione, Imprese_Progetti.Username_Modifica, CONVERT(DATE, Imprese_Progetti.Data_Creazione) AS Data_Creazione, CONVERT(DATE, Imprese_Progetti.Data_Modifica) AS Data_Modifica ")
                stb.AppendLine(", CASE Reg_Impianti.Cul_Cod WHEN 0")
                stb.AppendLine("    THEN ISNULL(Codici_Anagrafe.descrizione, 'Terreno Nudo')")
                stb.AppendLine("    ELSE SpecieVegetali.Veg_Des END AS [Utilizzo]")

                stb.AppendLine(", CASE WHEN Imprese_Progetti.Data_Fioritura_Prevista IS NULL THEN CAST('2100-12-31' AS date) ELSE Imprese_Progetti.Data_Fioritura_Prevista END AS Data_Fioritura")
                stb.AppendLine(", CASE WHEN Imprese_Progetti.Data_Inizio_Prevista IS NULL THEN CAST('1900-01-01' AS date) ELSE Imprese_Progetti.Data_Inizio_Prevista END AS Data_Semina")
                stb.AppendLine(", CASE WHEN Imprese_Progetti.Data_Fine_Prevista IS NULL THEN CAST('2100-12-31' AS date) ELSE Imprese_Progetti.Data_Fine_Prevista END AS Data_Raccolta")
                stb.AppendLine("    , CASE WHEN Imprese_Progetti.Stato_Impianto = 0 THEN '' ELSE COALESCE(FasiCicloColturale_Anagrafiche.fase_des, CASE WHEN Imprese_Progetti.Stato_Impianto = 102 THEN 'In produzione' ELSE '' END) END AS DESSTATOIMPIANTO")

                stb.AppendLine("  	, ISNULL(( SELECT TOP 1 Reg_Impianti_Codici.val_cod ")
                stb.AppendLine("  				 FROM Reg_Impianti_Codici ")
                stb.AppendLine("  				 WHERE (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Progetto_Cod = 0) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Impianto_Interbina & ")), 0) AS Interbina ")

                stb.AppendLine("  	, ISNULL(( SELECT TOP 1 Reg_Impianti_Codici.val_cod ")
                stb.AppendLine("  				 FROM Reg_Impianti_Codici ")
                stb.AppendLine("  				 WHERE (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Progetto_Cod = 0) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Impianto_Germinabilita & ")), 0) AS Germinabilita ")

                If Not IsNothing(objParametriUtenti) Then
                    If sql2017 Then
                        stb.AppendLine(" , TRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, '')) AS Username_Creazione_Completo ")
                        stb.AppendLine(" , TRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, '')) AS Username_Modifica_Completo ")
                    Else
                        stb.AppendLine(" , LTRIM(RTRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, ''))) AS Username_Creazione_Completo ")
                        stb.AppendLine(" , LTRIM(RTRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, ''))) AS Username_Modifica_Completo")
                    End If
                Else
                    stb.AppendLine(" , Imprese_Progetti.Username_Creazione AS Username_Creazione_Completo ")
                    stb.AppendLine(" , Imprese_Progetti.Username_Modifica AS Username_Modifica_Completo ")
                End If

                stb.AppendLine("  		, ISNULL(STUFF ((select ', ' + CA_Des FROM CertificazioniAziendali AS ca where ca.CA_Cod in (select strName from dbo.fSplit(TcodiCont.val_cod, '|'))  ")
                stb.AppendLine("  	For XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 2, ''), '') AS CertificazioneAziendale ")

                stb.AppendLine("  		 	, ISNULL(STUFF ((select ', ' + Contributo_Des FROM ContributiColtivazioni cc ")
                stb.AppendLine("  		 where cc.Contributo_Cod in (select strName from dbo.fSplit(TcodiCont.val_cod, '|'))  ")
                stb.AppendLine("  		 FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 2, '') , '') AS Contributi ")

                stb.AppendLine(", ISNULL(( SELECT TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des  ")
                stb.AppendLine("  				 FROM Reg_Impianti_Codici ")
                stb.AppendLine("  				 inner JOIN CAC_Codifica_InfoAggiuntive ")
                stb.AppendLine("  				 ON CAC_Codifica_InfoAggiuntive.InfoAgg_Cod = Reg_Impianti_Codici.val_cod ")
                stb.AppendLine("  				 AND CAC_Codifica_InfoAggiuntive.Argomento_Cod=8 ")
                stb.AppendLine("  				 WHERE (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Codice_Certificazione_Prodotto & ")) , '') AS CertificazioneProdotto ")

                stb.AppendLine("  	, ISNULL(( SELECT TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des  ")
                stb.AppendLine("  				 FROM Reg_Impianti_Codici ")
                stb.AppendLine("  				 inner JOIN CAC_Codifica_InfoAggiuntive ")
                stb.AppendLine("  				 ON CAC_Codifica_InfoAggiuntive.InfoAgg_Cod = Reg_Impianti_Codici.val_cod ")
                stb.AppendLine("  				 and CAC_Codifica_InfoAggiuntive.Argomento_Cod=1 ")
                stb.AppendLine("  				 WHERE (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Capitolato_Privato & ")), '') AS CapitolatoPrivato ")

                stb.AppendLine("  	, ISNULL(( SELECT TOP 1 CAC_Codifica_InfoAggiuntive.InfoAgg_Des  ")
                stb.AppendLine("  				 FROM Reg_Impianti_Codici ")
                stb.AppendLine("  				 inner JOIN CAC_Codifica_InfoAggiuntive ")
                stb.AppendLine("  				 ON CAC_Codifica_InfoAggiuntive.InfoAgg_Cod = Reg_Impianti_Codici.val_cod ")
                stb.AppendLine("  				 and CAC_Codifica_InfoAggiuntive.Argomento_Cod=7 ")
                stb.AppendLine("  				 WHERE (Reg_Impianti_Codici.Piva= Reg_Impianti.Piva) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Sa_Cod = Reg_Impianti.sa_cod) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
                stb.AppendLine("  				 AND (Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Codice_Residuo & "  )), '') AS Residuo ")

                stb.AppendLine("  		, ISNULL( STUFF ((select ', ' + Contatti.Cognome + ' ' + Contatti.nome FROM Contatti where Contatti.Cod_Contatto collate Latin1_General_CI_AS in (select strName from dbo.fSplit(Tcodi.val_cod, '|')) ")
                stb.AppendLine("  		                  FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 2, ''), '') AS TecniciRifImpianto ")
                stb.AppendLine("  		, ISNULL((SELECT TOP 1 Cognome + ' ' + Nome + ' ' + Rag_Soc FROM Contatti WHERE Cod_Contatto = IC_Tecnico.val_cod), '') AS TecnicoRifImpresa")
                stb.AppendLine("  		, ISNULL(Materie_Prime.Mat_Des, '') AS Prodotto")
                stb.AppendLine("  		, Appezzamento.X AS Latitudine, Appezzamento.Y AS Longitudine ")
                stb.AppendLine("      , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS Gruppi_Raccolta")
                stb.AppendLine("      , (Imprese_Progetti.Produzione_Prevista * Reg_Impianti.Sup_Imp) AS ResaTotalePrevista")

                stb.AppendLine("      , ISNULL(IC_UltimaVerificaIspettiva.Val_Cod, '') AS Ultima_Verifica_Ispettiva")
                stb.AppendLine("      , ISNULL(IC_SAUTotAziendale.Val_Cod, '') AS SAU_Tot_Aziendale")
                stb.AppendLine("      , ISNULL(RegioneImpresa.Regione_Des, '') AS RegioneImpresa")

                stb.AppendLine("      , Centri_Aziendali.lat AS LatitudineCentro, Centri_Aziendali.long AS LongitudineCentro ")

                stb.AppendLine("      , ISNULL((SELECT TOP 1 LegaleRappresentante FROM cte_LegaleRappresentante WHERE cte_LegaleRappresentante.Piva = Imprese.Piva), '') AS LegaleRappresentante ")
                stb.AppendLine("      , (SELECT TOP 1 Data_Nascita FROM cte_LegaleRappresentante WHERE cte_LegaleRappresentante.Piva = Imprese.Piva) AS LegaleRappresentante_DataNascita ")
                stb.AppendLine("      , ISNULL((SELECT TOP 1 Codice_Fiscale FROM cte_LegaleRappresentante WHERE cte_LegaleRappresentante.Piva = Imprese.Piva), '') AS LegaleRappresentante_CodiceFiscale ")
                stb.AppendLine("      , ISNULL((SELECT TOP 1 Indirizzo FROM cte_LegaleRappresentante WHERE cte_LegaleRappresentante.Piva = Imprese.Piva), '') AS LegaleRappresentante_LuogoNascita ")

                Join.bListaProvince = True
                Join.bCentriAziendali = True
                Join.bSpecieVegetali = True
                Join.bRegImpianti = True
                Join.bCultivar = True
                Join.bGruppoVarietale = True
                Join.bCampi = True
                Join.bAppezzamento = True
                Join.bImpreseProgetti = True
                Join.bImpreseCodici_CodiceSocio = True
                Join.bImpreseCodici_CodiceCuaa = True

                'Anna 10/08/21: Attivate JOINs per colonne TTI
                Join.bGruppoVegetale = True
                Join.bGruppoFinalita = True

                Join.bCentrixIndirizzi = True
                Join.bIndirizziCentro = True
                Join.bIstatCentro = True
                Join.bIndirizzi = True
                Join.bIstat = True
                Join.bGerarchiaImprese = True

                Join.bImpreseCodici_CodiceTecnico = True
                Join.bGruppiRaccolta = True

                Join.bImpreseCodici_UltimaVerificaIspettiva = True
                Join.bImpreseCodici_SAUTotAziendale = True
                Join.bListaRegioni = True

#End Region

            'Anna 16/08/21 - Aggiunta scheda fabbricati nel filtrone, per TTI
            Case enum_TipoSelect_FiltroneSuperNova.Fabbricati  '12 Fabbricati
#Region "Fabbricati"
                stb.AppendLine(" SELECT DISTINCT (")

                stb.AppendLine("     fabbricati.piva + '_' + CAST(fabbricati.sa_cod AS VARCHAR(20)) + '_' + CAST(fabbricati.Fabbricato_Cod  AS VARCHAR(20))) AS chiave")
                stb.AppendLine("   , fabbricati.sa_cod")
                stb.AppendLine("   , fabbricati.Fabbricato_Cod")
                stb.AppendLine("   , fabbricati.piva AS PIVA")
                stb.AppendLine("   , imprese.rag_soc AS Rag_Soc")
                stb.AppendLine("   , IC_Cuaa.val_cod AS CodiceCuaa")
                'stb.AppendLine("   , ISNULL(Imprese_1.rag_soc, '') AS ImpresaReferente")
                stb.AppendLine("   , ISNULL((SELECT TOP 1 val_cod ")
                stb.AppendLine("             FROM Imprese_Codici AS Imprese_Codici_CodSocio ")
                stb.AppendLine("             WHERE Imprese_Codici_CodSocio.piva = fabbricati.piva ")
                stb.AppendLine("             AND Imprese_Codici_CodSocio.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & "), '') AS CodiceSocio")

                'SOSTITUTITO CON TecnicoRifImpresa<
                'stb.AppendLine("   , ISNULL((SELECT TOP 1 Contatti.Rag_Soc + Contatti.Nome + ' ' + Contatti.Cognome AS TecnicoReferente  ")
                'stb.AppendLine("             FROM Imprese_Codici")
                'stb.AppendLine("                    INNER JOIN Contatti ON Contatti.Cod_Contatto = Imprese_Codici.val_cod  ")
                'stb.AppendLine("                    INNER JOIN UtentiXImprese ON Contatti.Piva = UtentiXImprese.Piva")
                'stb.AppendLine("             WHERE (Imprese_Codici.Piva = fabbricati.Piva) ")
                'stb.AppendLine("             AND (Imprese_Codici.Id_Cod = " & enum_CodiciAnagrafe.Tecnico & ")) , '') AS TecnicoReferente ")

                stb.AppendLine("   , fabbricato_des AS Denominazione")

                stb.AppendLine("   ,  CASE WHEN CombustibileForno.val_cod IN ('Metano ','METANO') THEN 'Metano' ")
                stb.AppendLine("      ELSE CASE WHEN CombustibileForno.val_cod IN ('GPL ','gpl') THEN 'GPL'")
                stb.AppendLine("      ELSE CASE WHEN CombustibileForno.val_cod IN ('Gasolio ','GASOLIO') THEN 'Gasolio'")
                stb.AppendLine("      ELSE CASE WHEN CombustibileForno.val_cod IN ('Cippato ','cippato') THEN 'Cippato'")
                stb.AppendLine("      ELSE CombustibileForno.val_cod END END END END AS CombustibileForno")

                stb.AppendLine("   , CASE WHEN FiammaForno.val_cod IN ('indir','INDIRE','Indiretta','Indiretto') THEN 'Indiretta' ")
                stb.AppendLine("     ELSE FiammaForno.val_cod END AS FiammaForno ")

                stb.AppendLine("   , CASE WHEN CantiereForno.val_cod IN ('cassoni ','Cassoni','CASSONI') THEN 'Cassoni' ")
                stb.AppendLine("     ELSE CantiereForno.val_cod END AS CantiereForno")

                stb.AppendLine("   , CASE WHEN UmidificazioneForno.val_cod IN ('Acqua ','acqua','ACQUA') THEN 'Acqua' ")
                stb.AppendLine("     ELSE UmidificazioneForno.val_cod END AS UmidificazioneForno ")

                stb.AppendLine("   , CASE WHEN TipoForno.val_cod IN ('DE CLOE','DE CLOET') THEN 'De Cloet'")
                stb.AppendLine("     ELSE CASE WHEN TipoForno.val_cod IN ('EuropeTob','Europetop') THEN 'EuropeTob'")
                stb.AppendLine("     ELSE CASE WHEN TipoForno.val_cod IN ('TAB','Tab') THEN 'Tab'")
                stb.AppendLine("     ELSE CASE WHEN TipoForno.val_cod IN ('MENCAGLI','Mencagli') THEN 'Mencagli'")
                stb.AppendLine("     ELSE CASE WHEN TipoForno.val_cod IN ('GOME','Gome') THEN 'Gome'")
                stb.AppendLine("     ELSE CASE WHEN TipoForno.val_cod IN ('GODIOLI','Godioli') THEN 'Godioli'")
                stb.AppendLine("     ELSE CASE WHEN TipoForno.val_cod IN ('CALDERINI','Calderini') THEN 'Calderini'")
                stb.AppendLine("     ELSE TipoForno.val_cod END END END END END END END AS TipoForno")

                stb.AppendLine("   , fabbricati.Username_Creazione, fabbricati.Username_Modifica, CONVERT(DATE, fabbricati.Data_Creazione) AS Data_Creazione, CONVERT(DATE, fabbricati.Data_Modifica) AS Data_Modifica")

                If Not IsNothing(objParametriUtenti) Then
                    If sql2017 Then
                        stb.AppendLine("        , TRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, '')) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , TRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, '')) AS Username_Modifica_Completo ")
                    Else
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udc.nome, '') + ' ' + COALESCE(udc.Cognome, '') + ' ' + COALESCE(udc.Rag_Soc, ''))) AS Username_Creazione_Completo ")
                        stb.AppendLine("        , LTRIM(RTRIM(COALESCE(udm.nome, '') + ' ' + COALESCE(udm.Cognome, '') + ' ' + COALESCE(udm.Rag_Soc, ''))) AS Username_Modifica_Completo")
                    End If
                Else
                    stb.AppendLine("        , fabbricati.Username_Creazione AS Username_Creazione_Completo ")
                    stb.AppendLine("        , fabbricati.Username_Modifica AS Username_Modifica_Completo ")
                End If

                stb.AppendLine("        , ISNULL((SELECT TOP 1 Cognome + ' ' + Nome + ' ' + Rag_Soc FROM Contatti WHERE Cod_Contatto = IC_Tecnico.val_cod), '') AS TecnicoRifImpresa")
                stb.AppendLine("        , ISNULL(Gruppi_Raccolta.GruppoRaccolta_Des, '') AS Gruppi_Raccolta")

                If Join.bGerarchiaImprese AndAlso sql2017 Then
                    stb.AppendLine("  , g.Padri, g.rag_soc_s AS Referenti ")
                End If

                Join.bImpreseCodici_CodiceSocio = True
                Join.bImpreseCodici_CodiceCuaa = True
                Join.bGerarchiaImprese = True
                Join.bFabbricatiCodici = True
                Join.bFabbricati = True
                Join.bRegImpianti = True

                Join.bImpreseXIndirizzi = False
                Join.bCentriAziendali = False
                Join.bAppezzamento = False
                Join.bImpreseXIndirizzi = False

                Join.bImpreseCodici_CodiceTecnico = True
                Join.bGruppiRaccolta = True

#End Region
        End Select

        '----------------------------------------------------
        '--- Gestione delle dipendenze dei JOIN -------------
        '----------------------------------------------------
#Region "BLOCCO JOIN"
        ImpostaVariabiliJOIN(Join)

        Dim StrJOIN As New System.Text.StringBuilder
        Dim nParentesiTondeAperte As Integer = 0

        StrJOIN.Length = 0

        If Join.bGerarchiaImprese Then
            If sql2017 Then
                StrJOIN.AppendLine(" LEFT JOIN ( SELECT STRING_AGG(Padre, ', ') AS padri, STRING_AGG(Imprese.rag_soc, ', ') AS rag_soc_s, GerarchiaImprese.Figlio ")
                StrJOIN.AppendLine(" 			FROM GerarchiaImprese " & If(noLock, " (NOLOCK) ", "") & " ")
                StrJOIN.AppendLine(" 			JOIN Imprese " & If(noLock, " (NOLOCK) ", "") & " ON GerarchiaImprese.Padre = Imprese.PIVA ")
                StrJOIN.AppendLine(" 			GROUP BY GerarchiaImprese.Figlio) g ON Imprese.Piva = g.Figlio ) ")
            Else
                StrJOIN.AppendLine(" INNER JOIN GerarchiaImprese " & If(noLock, " (NOLOCK) ", "") & " ON Imprese.Piva = GerarchiaImprese.Figlio) ")
            End If

            'Anna 10/08/21: Aggiunte JOINs per colonne TTI
            'If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Fabbricati Then
            '    StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese Imprese_1 " & If(noLock, " (NOLOCK) ", "") & " ")
            '    StrJOIN.AppendLine("  	INNER JOIN GerarchiaImprese GerarchiaImprese_Padre " & If(noLock, " (NOLOCK) ", "") & " ON Imprese_1.Piva = GerarchiaImprese_Padre.Padre ")
            '    StrJOIN.AppendLine(" ON Imprese.Piva = GerarchiaImprese_Padre.Figlio ")
            'End If

            nParentesiTondeAperte += 1
        End If

        Join.bImpreseXIndirizzi = True
        If Join.bImpreseXIndirizzi Then
            StrJOIN.AppendLine(" INNER JOIN ImpresexIndirizzi " & If(noLock, " (NOLOCK) ", "") & " ON Imprese.PIVA = ImpresexIndirizzi.PIVA) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bIndirizzi Then
            StrJOIN.AppendLine(" INNER JOIN Indirizzi " & If(noLock, " (NOLOCK) ", "") & " ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo) ")
            'Anna 10/08/21: Aggiunte JOINs per colonne TTI
            StrJOIN.AppendLine(" INNER JOIN Indirizzi IndImp " & If(noLock, " (NOLOCK) ", "") & " ON ImpresexIndirizzi.cod_indirizzo = IndImp.cod_indirizzo ")
            nParentesiTondeAperte += 1
        End If

        If Join.bIstat Then
            StrJOIN.AppendLine(" INNER JOIN ISTAT " & If(noLock, " (NOLOCK) ", "") & " ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM) ")
            'Anna 10/08/21: Aggiunte JOINs per colonne TTI
            StrJOIN.AppendLine("  INNER JOIN ISTAT IstatImp " & If(noLock, " (NOLOCK) ", "") & " ON IndImp.pro_cod_istat = IstatImp.PROV AND IndImp.com_cod_istat = IstatImp.COM  ")
            nParentesiTondeAperte += 1
        End If

        If Join.bListaProvince Then
            StrJOIN.AppendLine(" INNER JOIN Lista_Province " & If(noLock, " (NOLOCK) ", "") & " ON Lista_Province.Sigla = ISTAT.COMUNI_PROV) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bListaRegioni Then
            StrJOIN.AppendLine(" INNER JOIN Lista_Regioni RegioneImpresa" & If(noLock, " (NOLOCK) ", "") & " ON RegioneImpresa.REG = Lista_Province.REG) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bCentriAziendali Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Centri_Aziendali " & If(noLock, " (NOLOCK) ", "") & " ON Imprese.Piva=Centri_Aziendali.Piva) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bImpresexParticelle Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN ImpresexParticelle " & If(noLock, " (NOLOCK) ", "") & " ON (Centri_Aziendali.PIVA = ImpresexParticelle.PIVA AND Centri_Aziendali.Sa_Cod = ImpresexParticelle.Sa_Cod) ")
            StrJOIN.AppendLine(" LEFT OUTER JOIN ParticelleCatastali " & If(noLock, " (NOLOCK) ", "") & " ON (ImpresexParticelle.PROV = ParticelleCatastali.PROV AND ImpresexParticelle.COM = ParticelleCatastali.COM AND ImpresexParticelle.SEZIONE = ParticelleCatastali.SEZIONE AND ImpresexParticelle.FOGLIO = ParticelleCatastali.FOGLIO AND ImpresexParticelle.NUMERO = ParticelleCatastali.NUMERO AND ImpresexParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO) ")
            StrJOIN.AppendLine(" LEFT OUTER JOIN ISTAT Istat_Particelle " & If(noLock, " (NOLOCK) ", "") & " ON ImpresexParticelle.PROV = Istat_Particelle.PROV AND ImpresexParticelle.COM = Istat_Particelle.COM ")
        End If

        If Join.bCentrixIndirizzi Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN CentrixIndirizzi " & If(noLock, " (NOLOCK) ", "") & " ON (Centri_Aziendali.PIVA = CentrixIndirizzi.PIVA AND Centri_Aziendali.Sa_Cod = CentrixIndirizzi.Sa_Cod AND CentrixIndirizzi.Tipo_Indirizzo = " & enum_IndirizzoTipo.SedeOperativa & ") ")
        End If

        If Join.bIndirizziCentro Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Indirizzi AS Indirizzi_Centro " & If(noLock, " (NOLOCK) ", "") & " ON CentrixIndirizzi.cod_indirizzo = Indirizzi_Centro.cod_indirizzo) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bIstatCentro Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN ISTAT AS Istat_Centro " & If(noLock, " (NOLOCK) ", "") & " ON Indirizzi_Centro.pro_cod_istat = Istat_Centro.PROV AND Indirizzi_Centro.com_cod_istat = Istat_Centro.COM) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bListaProvinceCentro Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Lista_Province AS Lista_Province_Centro " & If(noLock, " (NOLOCK) ", "") & " ON Lista_Province_Centro.Sigla = Istat_Centro.COMUNI_PROV) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bAppezzamento Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Appezzamento " & If(noLock, " (NOLOCK) ", "") & " ON (Centri_Aziendali.Piva = Appezzamento.Piva AND Centri_Aziendali.Sa_Cod = Appezzamento.Sa_Cod ) ")
            StrJOIN.AppendLine(" LEFT JOIN AppezzamentiXParticelle " & If(noLock, " (NOLOCK) ", "") & " ON Appezzamento.Piva = AppezzamentiXParticelle.Piva AND Appezzamento.SA_COD = AppezzamentiXParticelle.sa_cod AND Appezzamento.APPEZZA = AppezzamentiXParticelle.APPEZZA ")
            StrJOIN.AppendLine(" LEFT JOIN ZonexParticelle " & If(noLock, " (NOLOCK) ", "") & " ON AppezzamentiXParticelle.PROV = ZonexParticelle.PROV AND AppezzamentiXParticelle.COM = ZonexParticelle.COM AND AppezzamentiXParticelle.SEZIONE = ZonexParticelle.SEZIONE AND AppezzamentiXParticelle.FOGLIO = ZonexParticelle.FOGLIO AND AppezzamentiXParticelle.NUMERO = ZonexParticelle.NUMERO AND AppezzamentiXParticelle.SUBALTERNO = ZonexParticelle.SUBALTERNO AND ZonexParticelle.Zona_Cod = -17 ")

            'Anna 27 / 7 / 21 - Aggiunte nuove colonne: Nazione, Regione, Città
            StrJOIN.AppendLine(" LEFT JOIN AppezzamentixIndirizzi " & If(noLock, " (NOLOCK) ", "") & " ")
            StrJOIN.AppendLine(" ON Appezzamento.PIVA = AppezzamentixIndirizzi.PIVA")
            StrJOIN.AppendLine(" AND Appezzamento.SA_COD = AppezzamentixIndirizzi.SA_COD")
            StrJOIN.AppendLine(" AND Appezzamento.APPEZZA = AppezzamentixIndirizzi.APPEZZA")

            StrJOIN.AppendLine(" LEFT JOIN Indirizzi AppIndirizzi " & If(noLock, " (NOLOCK) ", "") & "")
            StrJOIN.AppendLine(" ON AppezzamentixIndirizzi.cod_indirizzo = AppIndirizzi.cod_indirizzo")

            StrJOIN.AppendLine(" LEFT JOIN Lista_Province ProvinciaKPIN2 " & If(noLock, " (NOLOCK) ", "") & "")
            StrJOIN.AppendLine(" ON AppIndirizzi.pro_cod_istat = ProvinciaKPIN2.PROV")

            StrJOIN.AppendLine(" LEFT JOIN Lista_Regioni " & If(noLock, " (NOLOCK) ", "") & "")
            StrJOIN.AppendLine(" ON ProvinciaKPIN2.REG = Lista_Regioni.REG")
            StrJOIN.AppendLine(" AND ProvinciaKPIN2.REG <> '000'")

        End If

        If Join.bRegImpianti Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Reg_Impianti " & If(noLock, " (NOLOCK) ", "") & " ON (Appezzamento.Piva = Reg_Impianti.Piva AND Appezzamento.SA_Cod = Reg_Impianti.Sa_Cod AND Appezzamento.Appezza = Reg_Impianti.Appezza) ")

            ' Anna 27/07/21 - Aggiunte nuove colonne: port_des, foral_des, cop_des, imp_des
            StrJOIN.AppendLine(" LEFT JOIN portinnesti " & If(noLock, " (NOLOCK) ", "") & " ON (Reg_Impianti.Port_Cod = portinnesti.Port_Cod)")
            StrJOIN.AppendLine(" LEFT JOIN formeallevamento " & If(noLock, " (NOLOCK) ", "") & " ON (Reg_Impianti.Foral_Cod = formeallevamento.Foral_Cod)")
            StrJOIN.AppendLine(" LEFT JOIN copertura " & If(noLock, " (NOLOCK) ", "") & " ON (Reg_Impianti.Cop_Cod = copertura.Cop_Cod)")
            StrJOIN.AppendLine(" LEFT JOIN impiantiirrigazioni " & If(noLock, " (NOLOCK) ", "") & " ON (Reg_Impianti.Imp_Cod = impiantiirrigazioni.Imp_Cod) ")

            ''' Anna 27/07/21 - Aggiunte nuove colonne: organismoReferente, licenza
            ''StrJOIN.AppendLine(" LEFT JOIN Reg_Impianti_Codici RegxOrganismoRef ON (Imprese.PIVA = RegxOrganismoRef.PIVA AND id_cod IN ( " & enum_CodiciAnagrafe.Impianto_Cooperativa & " ) ) " )

            ''StrJOIN.AppendLine(" LEFT JOIN OTabelle ON (RegxOrganismoRef.id_cod = OTabelle.Tabella_Cod AND id_cod IN ( " & enum_CodiciAnagrafe.Zespri_Fasi_Fase & " ) ) " )
            ''StrJOIN.AppendLine(" LEFT JOIN OTabelle_Parametri  ON (OTabelle.Tabella_Cod = OTabelle_Parametri.Tabella_Cod ) " )

        End If

        If Join.bImpreseProgetti Then

            If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Esercizi Then

                StrJOIN.AppendLine(" INNER JOIN Imprese_Progetti " & If(noLock, " (NOLOCK) ", "") & " ON (Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) ")
                StrJOIN.AppendLine(" LEFT JOIN Reg_Impianti_Codici kpin " & If(noLock, " (NOLOCK) ", "") & " ON (Imprese_Progetti.Piva = kpin.Piva AND Imprese_Progetti.SA_Cod = kpin.Sa_Cod AND Imprese_Progetti.Appezza = kpin.Appezza AND Imprese_Progetti.Id_Reg = kpin.Id_Reg AND Imprese_Progetti.Progetto_Cod = kpin.Progetto_Cod AND kpin.id_Cod in ( " & enum_CodiciAnagrafe.Zespri_Codice_kPIN & " )  ) ")
                StrJOIN.AppendLine(" LEFT JOIN Reg_Impianti_Codici blockName " & If(noLock, " (NOLOCK) ", "") & " ON (Imprese_Progetti.Piva = blockName.Piva AND Imprese_Progetti.SA_Cod = blockName.Sa_Cod AND Imprese_Progetti.Appezza = blockName.Appezza AND Imprese_Progetti.Id_Reg = blockName.Id_Reg AND Imprese_Progetti.Progetto_Cod = blockName.Progetto_Cod AND blockName.id_Cod in ( " & enum_CodiciAnagrafe.Zespri_Block_Name & " )  ) ")
                StrJOIN.AppendLine(" LEFT JOIN Reg_Impianti_Codici growerNumber " & If(noLock, " (NOLOCK) ", "") & " ON (Imprese_Progetti.Piva = growerNumber.Piva AND Imprese_Progetti.SA_Cod = growerNumber.Sa_Cod AND Imprese_Progetti.Appezza = growerNumber.Appezza AND Imprese_Progetti.Id_Reg = growerNumber.Id_Reg AND Imprese_Progetti.Progetto_Cod = growerNumber.Progetto_Cod AND growerNumber.id_Cod in ( " & enum_CodiciAnagrafe.Zepri_Grower_Number & " )  ) ")

                '27/07/21 - Aggiunte nuove colonne: organismoReferente, licenza
                StrJOIN.AppendLine(" LEFT JOIN Reg_Impianti_Codici OrganismoRef " & If(noLock, " (NOLOCK) ", "") & " ON (Imprese_Progetti.Piva = OrganismoRef.Piva AND Imprese_Progetti.SA_Cod = OrganismoRef.Sa_Cod AND Imprese_Progetti.Appezza = OrganismoRef.Appezza AND Imprese_Progetti.Id_Reg = OrganismoRef.Id_Reg AND Imprese_Progetti.Progetto_Cod = OrganismoRef.Progetto_Cod AND OrganismoRef.id_Cod in ( " & enum_CodiciAnagrafe.Organismo_Referente & " ) ) left JOIN imprese impreseOrgRef ON impreseOrgRef.piva = OrganismoRef.val_cod ")
                StrJOIN.AppendLine(" LEFT JOIN Reg_Impianti_Codici Licenza " & If(noLock, " (NOLOCK) ", "") & " ON (Imprese_Progetti.Piva = Licenza.Piva AND Imprese_Progetti.SA_Cod = Licenza.Sa_Cod AND Imprese_Progetti.Appezza = Licenza.Appezza AND Imprese_Progetti.Id_Reg = Licenza.Id_Reg AND Imprese_Progetti.Progetto_Cod = Licenza.Progetto_Cod AND Licenza.id_Cod in ( " & enum_CodiciAnagrafe.Zespri_Fasi_Fase & " ) ) left JOIN OTabelle_Parametri ON OTabelle_Parametri.tabella_cod = Licenza.id_cod and OTabelle_Parametri.tabella_par_cod = Licenza.val_cod   ")

                '16/03/2022 - Aggiunta nuova colonna: DISCIPLINARE
                StrJOIN.AppendLine(" LEFT OUTER JOIN DPI_Regolamenti " & If(noLock, " (NOLOCK) ", "") & " ON Imprese_Progetti.Disciplinare_Cod = DPI_Regolamenti.COD_REGOLAMENTO AND Imprese_Progetti.Disciplinare_PubblicoPrivato = DPI_Regolamenti.Flag_Privato_Pubblico ")

                StrJOIN.AppendLine(" LEFT OUTER JOIN Materie_Prime " & If(noLock, " (NOLOCK) ", "") & " ON (Materie_Prime.Mat_Cod = Imprese_Progetti.Mat_Cod) AND Materie_Prime.Elem_Cod = " & TRASFORMATI_VEGETALI & " ")

            Else
                StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Progetti " & If(noLock, " (NOLOCK) ", "") & " ON (Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) ")
            End If
        End If

        If Join.bImpreseCodici Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici " & If(noLock, " (NOLOCK) ", "") & " ON (Imprese_Codici.Piva = Imprese.Piva) ")
        End If

        If Join.bImpreseCodici_CodiceSocio Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC " & If(noLock, " (NOLOCK) ", "") & " ON (IC.Piva = Imprese.Piva) AND IC.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & " ")
        End If

        If Join.bImpreseCodici_CodiceCuaa Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC_cuaa " & If(noLock, " (NOLOCK) ", "") & " ON (IC_cuaa.Piva = Imprese.Piva) AND IC_cuaa.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA & " ")
        End If

        If Join.bImpreseCodici_CodiceTecnico Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC_Tecnico " & If(noLock, " (NOLOCK) ", "") & " ON (IC_Tecnico.Piva = Imprese.Piva) AND IC_Tecnico.id_cod = " & enum_CodiciAnagrafe.Tecnico & " ")
        End If

        If Join.bGruppiRaccolta Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Gruppi_Raccolta " & If(noLock, " (NOLOCK) ", "") & " ON (Gruppi_Raccolta.GruppoRaccolta_Cod = Imprese.GruppoRaccolta_Cod) ")
        End If

        If Join.bImpreseCodici_CodiceCertificazione Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC_Certificazione " & If(noLock, " (NOLOCK) ", "") & " ON (IC_Certificazione.Piva = Imprese.Piva) AND IC_Certificazione.id_cod = " & enum_CodiciAnagrafe.Codice_Certificazione & " ")
        End If

        If Join.bImpreseCodici_UltimaVerificaIspettiva Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC_UltimaVerificaIspettiva " & If(noLock, " (NOLOCK) ", "") & " ON (IC_UltimaVerificaIspettiva.Piva = Imprese.Piva) AND IC_UltimaVerificaIspettiva.id_cod = " & enum_CodiciAnagrafe.Ultima_Verifica_Ispettiva & " ")
        End If

        If Join.bImpreseCodici_SAUTotAziendale Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC_SAUTotAziendale " & If(noLock, " (NOLOCK) ", "") & " ON (IC_SAUTotAziendale.Piva = Imprese.Piva) AND IC_SAUTotAziendale.id_cod = " & enum_CodiciAnagrafe.Sau_Tot_Azienda_ha & " ")
        End If

        If Join.bCentriAziendaliCodici Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Centri_Aziendali_Codici " & If(noLock, " (NOLOCK) ", "") & " ON (Centri_Aziendali_Codici.Piva = Centri_Aziendali.Piva AND Centri_Aziendali_Codici.SA_Cod = Centri_Aziendali.Sa_Cod ) ")
        End If

        If Join.bAppezzamentoCodici Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Appezzamento_Codici " & If(noLock, " (NOLOCK) ", "") & " ON (Appezzamento_Codici.Piva = Appezzamento.Piva AND Appezzamento_Codici.SA_Cod = Appezzamento.Sa_Cod AND Appezzamento_Codici.Appezza = Appezzamento.Appezza ) ")
        End If

        If Join.bRegImpiantiCodici Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Reg_Impianti_Codici " & If(noLock, " (NOLOCK) ", "") & " ON (Reg_Impianti_Codici.Piva = Reg_Impianti.Piva AND Reg_Impianti_Codici.SA_Cod = Reg_Impianti.Sa_Cod AND Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
        End If

        If Join.bCampi Then
            If Join.bCampiCodici Then
                StrJOIN.AppendLine(" INNER JOIN Campi " & If(noLock, " (NOLOCK) ", "") & " ON Campi.Piva = Centri_Aziendali.Piva AND Campi.Sa_Cod = Centri_Aziendali.Sa_Cod ")
                StrJOIN.AppendLine(" LEFT OUTER JOIN SpecieVegetali " & If(noLock, " (NOLOCK) ", "") & " ON Campi.VEG_COD = SpecieVegetali.VEG_COD ")
                StrJOIN.AppendLine(" LEFT OUTER JOIN GruppoVegetale " & If(noLock, " (NOLOCK) ", "") & " ON Campi.GRU_COD = GruppoVegetale.GRU_COD ")
                StrJOIN.AppendLine(" LEFT JOIN Campi_Codici CC1279 " & If(noLock, " (NOLOCK) ", "") & " ON CC1279.Piva = Campi.Piva AND CC1279.SA_Cod = Campi.Sa_Cod AND CC1279.Campo_Cod = Campi.Campo_Cod AND CC1279.id_Cod = " & enum_CodiciAnagrafe.Riferimento_Alfanumerico_Campo & " ")
                StrJOIN.AppendLine(" LEFT JOIN Campi_Codici CC1325 " & If(noLock, " (NOLOCK) ", "") & " ON CC1325.Piva = Campi.Piva AND CC1325.SA_Cod = Campi.Sa_Cod AND CC1325.Campo_Cod = Campi.Campo_Cod AND CC1325.id_Cod = " & enum_CodiciAnagrafe.Sup_Contratto & " ")
                StrJOIN.AppendLine(" LEFT JOIN Campi_Codici CC1326 " & If(noLock, " (NOLOCK) ", "") & " ON CC1326.Piva = Campi.Piva AND CC1326.SA_Cod = Campi.Sa_Cod AND CC1326.Campo_Cod = Campi.Campo_Cod AND CC1326.id_Cod = " & enum_CodiciAnagrafe.Filiera & " ")
                StrJOIN.AppendLine(" LEFT JOIN Imprese_Codici IC1324 " & If(noLock, " (NOLOCK) ", "") & " ON IC1324.Piva = Imprese.Piva AND IC1324.id_Cod = " & enum_CodiciAnagrafe.Contratto_Produzione & " ")
            Else
                StrJOIN.AppendLine(" LEFT OUTER JOIN Campi " & If(noLock, " (NOLOCK) ", "") & " ON (Campi.Piva = Appezzamento.Piva AND Campi.Sa_Cod = Appezzamento.Sa_Cod AND Campi.Campo_Cod = Appezzamento.Campo_Cod ) ")
            End If
        End If

        If Join.bAppezzamentixParticelle Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN AppezzamentixParticelle " & If(noLock, " (NOLOCK) ", "") & " ON (AppezzamentixParticelle.Piva = Appezzamento.Piva AND AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod AND AppezzamentixParticelle.Appezza = Appezzamento.Appezza ) ")
        End If

        If Join.bGruppoVarietale Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN GruppoVarietale " & If(noLock, " (NOLOCK) ", "") & " ON abs(Reg_Impianti.GRVA_Cod_VEG) = GruppoVarietale.GRVA_COD) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bCultivar Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Cultivar " & If(noLock, " (NOLOCK) ", "") & " ON Reg_Impianti.CUL_COD = Cultivar.CUL_COD) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bSpecieVegetali Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN SpecieVegetali " & If(noLock, " (NOLOCK) ", "") & " ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bGruppoVegetale Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN GruppoVegetale " & If(noLock, " (NOLOCK) ", "") & " ON SpecieVegetali.GRU_COD = GruppoVegetale.GRU_COD) ")
            nParentesiTondeAperte += 1
        End If

        'Anna 10/08/21: Aggiunta JOINs per colonne TTI
        If Join.bGruppoFinalita Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN GruppoFinalita " & If(noLock, " (NOLOCK) ", "") & " ON Reg_Impianti.GRFI_COD = GruppoFinalita.Grfi_Cod) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bMovDestinazioni Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Mov_Destinazioni " & If(noLock, " (NOLOCK) ", "") & " ON Mov_Destinazioni.Piva = Reg_Impianti.Piva AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.Sa_Cod AND Mov_Destinazioni.Appezza = Reg_Impianti.Appezza AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg  AND Mov_Destinazioni.Tipo_Destinazione = 0) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bAgenda Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Agenda " & If(noLock, " (NOLOCK) ", "") & " ON Agenda.Piva = Reg_Impianti.Piva AND Agenda.Sa_Cod = Reg_Impianti.Sa_Cod AND Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bMovimenti Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Movimenti " & If(noLock, " (NOLOCK) ", "") & " ON Agenda.Piva = Movimenti.Piva AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bMovDettaglioTecnico Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico " & If(noLock, " (NOLOCK) ", "") & " ON Mov_Dettaglio_Tecnico.Piva = Movimenti.Piva AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti.Id_Agenda AND Mov_Dettaglio_Tecnico.Id_Mov = Movimenti.Id_Mov) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bMovimentiDettagli Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Movimenti_Dettagli " & If(noLock, " (NOLOCK) ", "") & " ON Movimenti_Dettagli.Piva = Movimenti.Piva AND Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod AND Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bOperazioni Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Operazioni " & If(noLock, " (NOLOCK) ", "") & " ON  Agenda.Lav_Cod = Operazioni.Lav_Cod) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bGruppoOperazioni Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN GruppoOperazioni " & If(noLock, " (NOLOCK) ", "") & " ON  Operazioni.Gru_Op = GruppoOperazioni.Gru_Cod) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bRisorseUmane Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Risorse_Umane " & If(noLock, " (NOLOCK) ", "") & " ON ((Risorse_Umane.Cod_RisUm = Movimenti.Cod_RisUm) Or (Risorse_Umane.Cod_RisUm = Movimenti_Dettagli.Mat_Cod AND Movimenti_Dettagli.Elem_Cod = 0) ) )")
            nParentesiTondeAperte += 1
        End If

        If Join.bContatti Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Contatti " & If(noLock, " (NOLOCK) ", "") & " ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bRapportiContabili Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Rapporti_Contabili " & If(noLock, " (NOLOCK) ", "") & " ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto ) ")
            nParentesiTondeAperte += 1
        End If

        'Anna 17/08/21: Aggiunte JOINs per colonne TTI
        If Join.bFabbricati Then
            StrJOIN.AppendLine(" LEFT JOIN fabbricati " & If(noLock, " (NOLOCK) ", "") & " ON imprese.piva = fabbricati.piva ")
        End If

        If Join.bFabbricatiCodici Then
            StrJOIN.AppendLine(" LEFT JOIN fabbricati_Codici CombustibileForno " & If(noLock, " (NOLOCK) ", "") & " ")
            StrJOIN.AppendLine(" ON CombustibileForno.piva = imprese.piva AND CombustibileForno.fabbricato_cod = Fabbricati.fabbricato_cod AND CombustibileForno.sa_cod = fabbricati.sa_cod AND CombustibileForno.id_cod = " & enum_CodiciAnagrafe.Fabbricato_Forno_Combustibile & " ")

            StrJOIN.AppendLine(" LEFT JOIN fabbricati_Codici FiammaForno " & If(noLock, " (NOLOCK) ", "") & "")
            StrJOIN.AppendLine(" ON FiammaForno.piva = imprese.piva AND FiammaForno.fabbricato_cod = Fabbricati.fabbricato_cod AND FiammaForno.sa_cod = fabbricati.sa_cod AND FiammaForno.id_cod = " & enum_CodiciAnagrafe.Fabbricato_Forno_Fiamma & " ")

            StrJOIN.AppendLine(" LEFT JOIN fabbricati_Codici CantiereForno " & If(noLock, " (NOLOCK) ", "") & "")
            StrJOIN.AppendLine(" ON CantiereForno.piva = imprese.piva AND CantiereForno.fabbricato_cod = Fabbricati.fabbricato_cod AND CantiereForno.sa_cod = fabbricati.sa_cod AND CantiereForno.id_cod = " & enum_CodiciAnagrafe.Fabbricato_Forno_Cantiere & " ")

            StrJOIN.AppendLine(" LEFT JOIN fabbricati_Codici UmidificazioneForno " & If(noLock, " (NOLOCK) ", "") & "")
            StrJOIN.AppendLine(" ON UmidificazioneForno.piva = imprese.piva AND UmidificazioneForno.fabbricato_cod = Fabbricati.fabbricato_cod AND UmidificazioneForno.sa_cod = fabbricati.sa_cod AND UmidificazioneForno.id_cod = " & enum_CodiciAnagrafe.Fabbricato_Forno_Umidificazione & "  ")

            StrJOIN.AppendLine(" LEFT JOIN fabbricati_Codici TipoForno " & If(noLock, " (NOLOCK) ", "") & "")
            StrJOIN.AppendLine(" ON TipoForno.piva = imprese.piva AND TipoForno.fabbricato_cod = Fabbricati.fabbricato_cod AND TipoForno.sa_cod = fabbricati.sa_cod AND TipoForno.id_cod =" & enum_CodiciAnagrafe.Fabbricato_Forno_Tipo & "  ")
        End If


        ' VAnni: 11/2/2020: ora è esteso attraverso permesso utente, non più personalizzato su Riunite.
        'Grilli: Modifica volante per Valerio che probabilmente resterà qui per sempre... (RiuniteCiv)
        'If objParametri.PivaSuperUser = "00127310357" AndAlso JsonRichiesta.IncludiVisite AndAlso TipoSelect = enum_TipoSelect_FiltroneSuperNova.Impianti THEN
        If jsonRichiesta.IncludiVisite AndAlso TipoSelect = enum_TipoSelect_FiltroneSuperNova.Impianti Then
            StrJOIN.AppendLine(" LEFT JOIN ( ")
            StrJOIN.AppendLine(" SELECT piva, SA_COD, APPEZZA, ID_REG, validita_inizio AS dataVisita, qta AS punteggioVisita ")
            StrJOIN.AppendLine(" from  ")
            StrJOIN.AppendLine(" ( ")
            StrJOIN.AppendLine(" SELECT ri.piva,ri.SA_COD,ri.APPEZZA,ri.ID_REG,ri.cul_cod, ")
            StrJOIN.AppendLine(" ri2.piva AS piva2,ri2.SA_COD AS sa_cod2,ri2.APPEZZA AS appezza2,ri2.ID_REG AS id_reg2,ri2.cul_cod AS cul_cod2, ")
            StrJOIN.AppendLine(" a.id_agenda,a.validita_inizio,mdt.ff_classe,mdt.dett_cod,mdest.qta, ")
            StrJOIN.AppendLine(" row_number() over(partition by ri.piva,ri.SA_COD,ri.APPEZZA,ri.ID_REG order by a.id_agenda desc) AS rn ")
            StrJOIN.AppendLine(" from Reg_Impianti ri ")
            StrJOIN.AppendLine(" inner JOIN Reg_Impianti ri2 " & If(noLock, " (NOLOCK) ", "") & " ON ri.PIVA=ri2.PIVA AND ri.CUL_COD=ri2.CUL_COD  --AND ri.sa_cod=ri2.sa_cod ")
            StrJOIN.AppendLine(" INNER JOIN Imprese_Progetti ip " & If(noLock, " (NOLOCK) ", "") & " ON ri.Piva = ip.Piva AND ri.SA_COD = ip.Sa_Cod AND ip.Appezza = ri.APPEZZA AND ip.Id_Reg = ri.Id_Reg ")
            StrJOIN.AppendLine(" left JOIN Mov_Destinazioni mdest " & If(noLock, " (NOLOCK) ", "") & " ON ri2.piva=mdest.Piva AND ri2.SA_COD=mdest.Sa_Cod AND ri2.APPEZZA=mdest.Appezza AND ri2.ID_REG=mdest.Id_Destinazione ")
            StrJOIN.AppendLine(" left JOIN Mov_Dettaglio_Tecnico mdt " & If(noLock, " (NOLOCK) ", "") & " ON mdt.Id_Agenda=mdest.Id_Agenda AND mdt.Id_Mov=mdest.Id_Mov AND mdt.Id_Mov_Det=mdest.Id_Mov_Det ")
            StrJOIN.AppendLine(" left JOIN agenda a " & If(noLock, " (NOLOCK) ", "") & " ON a.Id_Agenda=mdt.Id_Agenda ")
            StrJOIN.AppendLine(" where --ri.piva='00000000000' and  ")
            StrJOIN.AppendLine(" (a.Validita_Inizio IS NULL OR (a.Validita_Inizio > ip.validita_inizio AND a.Validita_Inizio < ip.validita_fine)) AND ")
            StrJOIN.AppendLine(" (lav_cod is NULL or Lav_Cod=109) and (FF_Classe is NULL or FF_Classe=43) ")
            StrJOIN.AppendLine(" ) xxx ")
            StrJOIN.AppendLine(" where rn=1 ) riuniteCiv ")
            StrJOIN.AppendLine(" ON riuniteCiv.piva=reg_impianti.piva and riuniteCiv.sa_cod=reg_impianti.sa_cod and riuniteCiv.appezza=reg_impianti.appezza and riuniteCiv.id_reg=reg_impianti.id_reg )")
            nParentesiTondeAperte += 1

        End If

        If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Impianti Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN reg_impianti_Codici ric_magConf " & If(noLock, " (NOLOCK) ", "") & " ON ric_magConf.id_cod = " & enum_CodiciAnagrafe.Magazzino_Conferimento & " AND ric_magConf.Piva = Reg_Impianti.Piva AND ric_magConf.sa_cod = Reg_Impianti.sa_cod AND ric_magConf.appezza = Reg_Impianti.APPEZZA AND ric_magConf.Id_Reg = Reg_Impianti.ID_REG ) ")
            StrJOIN.AppendLine(" LEFT OUTER JOIN Fabbricati f_magConf " & If(noLock, " (NOLOCK) ", "") & " ON (CAST(f_magConf.Fabbricato_Cod AS VARCHAR(20)) + '|' + CAST(f_magConf.SA_COD  AS VARCHAR(20)) + '|' + CAST(f_magConf.piva AS VARCHAR(20))) = ric_magConf.val_cod)  ")
            nParentesiTondeAperte += 2
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici Centro_Conferimento " & If(noLock, " (NOLOCK) ", "") & " ON Imprese.Piva = Centro_Conferimento.Piva AND Centro_Conferimento.id_Cod = " & enum_CodiciAnagrafe.V01CON00_VCCOIS & " ")
            StrJOIN.AppendLine("LEFT JOIN Reg_Impianti_Codici Destinazione " & If(noLock, " (NOLOCK) ", "") & " ON Reg_Impianti.PIVA = Destinazione.PIVA ")
            StrJOIN.AppendLine("AND Reg_Impianti.Sa_Cod = Destinazione.Sa_Cod AND Reg_Impianti.Appezza = Destinazione.Appezza ")
            StrJOIN.AppendLine("AND Reg_Impianti.ID_Reg = Destinazione.ID_Reg AND Destinazione.id_Cod >= 3000 ")
            StrJOIN.AppendLine("AND Destinazione.id_cod < 4000 ")
            StrJOIN.AppendLine("LEFT JOIN Codici_Anagrafe " & If(noLock, " (NOLOCK) ", "") & " ON Destinazione.id_cod = Codici_Anagrafe.codice ")
            If Not IsNothing(objParametriUtenti) Then
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udc ON udc.CodFisc = Reg_Impianti.Username_Creazione ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udm ON udm.CodFisc = Reg_Impianti.Username_Modifica ")
            End If

            ' Da aggiungere l'inner JOIN con la tabella __tmp_FiltroImpianti qui.
        End If

        If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Esercizi Then
            StrJOIN.AppendLine("LEFT JOIN Reg_Impianti_Codici Destinazione " & If(noLock, " (NOLOCK) ", "") & " ON Reg_Impianti.PIVA = Destinazione.PIVA ")
            StrJOIN.AppendLine("AND Reg_Impianti.Sa_Cod = Destinazione.Sa_Cod AND Reg_Impianti.Appezza = Destinazione.Appezza ")
            StrJOIN.AppendLine("AND Reg_Impianti.ID_Reg = Destinazione.ID_Reg AND Destinazione.id_Cod >= 3000 ")
            StrJOIN.AppendLine("AND Destinazione.id_cod < 4000 ")
            StrJOIN.AppendLine("LEFT JOIN Codici_Anagrafe " & If(noLock, " (NOLOCK) ", "") & " ON Destinazione.id_cod = Codici_Anagrafe.codice ")

            If Not IsNothing(objParametriUtenti) Then
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udc ON udc.CodFisc = Imprese_Progetti.Username_Creazione ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udm ON udm.CodFisc = Imprese_Progetti.Username_Modifica ")
            End If


            StrJOIN.AppendLine(" LEFT JOIN Reg_Impianti_Codici Tcodi ON Tcodi.Piva= Reg_Impianti.Piva ")
            StrJOIN.AppendLine(" 						 AND Tcodi.Sa_Cod = Reg_Impianti.sa_cod ")
            StrJOIN.AppendLine(" 						 AND Tcodi.Appezza  = Reg_Impianti.Appezza ")
            StrJOIN.AppendLine(" 						 AND Tcodi.Id_Reg = Reg_Impianti.Id_Reg ")
            StrJOIN.AppendLine(" 						 AND Tcodi.id_cod = " & enum_CodiciAnagrafe.Tecnico & "  ")
            StrJOIN.AppendLine(" LEFT JOIN Reg_Impianti_Codici TcodiCont ON TcodiCont.Piva= Reg_Impianti.Piva ")
            StrJOIN.AppendLine(" 						 AND TcodiCont.Sa_Cod = Reg_Impianti.sa_cod ")
            StrJOIN.AppendLine(" 						 AND TcodiCont.Appezza  = Reg_Impianti.Appezza ")
            StrJOIN.AppendLine(" 						 AND TcodiCont.Id_Reg = Reg_Impianti.Id_Reg ")
            StrJOIN.AppendLine(" 						 AND TcodiCont.id_cod = " & enum_CodiciAnagrafe.Codice_Certificazione & " ")
            StrJOIN.AppendLine(" LEFT JOIN FasiCicloColturale_Anagrafiche (NOLOCK) ON Imprese_Progetti.Stato_Impianto = FasiCicloColturale_Anagrafiche.Fase_Cod")


        End If

        If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Movimenti Then
            If Not IsNothing(objParametriUtenti) Then
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udca ON udca.CodFisc = Agenda.Username_Creazione ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udcm ON udcm.CodFisc = Movimenti.Username_Creazione ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udma ON udma.CodFisc = Agenda.Username_Modifica ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udmm ON udmm.CodFisc = Movimenti.Username_Modifica ")
            End If
        End If

        If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Appezzamenti Then
            If Not IsNothing(objParametriUtenti) Then
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udc ON udc.CodFisc = Appezzamento.Username_Creazione ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udm ON udm.CodFisc = Appezzamento.Username_Modifica ")
            End If
        End If

        If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Campi Then
            If Not IsNothing(objParametriUtenti) Then
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udc ON udc.CodFisc = Campi.Username_Creazione ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udm ON udm.CodFisc = Campi.Username_Modifica ")
            End If
        End If

        If TipoSelect = enum_TipoSelect_FiltroneSuperNova.CentriAziendali Then
            If Not IsNothing(objParametriUtenti) Then
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udc ON udc.CodFisc = Centri_Aziendali.Username_Creazione ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udm ON udm.CodFisc = Centri_Aziendali.Username_Modifica ")
            End If
        End If

        If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Imprese Then
            If Not IsNothing(objParametriUtenti) Then
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udc ON udc.CodFisc = Imprese.Username_Creazione ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udm ON udm.CodFisc = Imprese.Username_Modifica ")
            End If
        End If

        If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Fabbricati Then
            If Not IsNothing(objParametriUtenti) Then
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udc ON udc.CodFisc = fabbricati.Username_Creazione ")
                StrJOIN.AppendLine("LEFT JOIN " & utenti_DB_name & "Utenti_Dettagli udm ON udm.CodFisc = fabbricati.Username_Modifica ")
            End If
        End If

        '----------------------------------------------------------------
        '--- Filtro associato all'utente 
        '----------------------------------------------------------------
        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim UtenteProfiloImpreseSql As String = ""
        Dim UtenteProfiloCentriSql As String = ""
        Dim DtImpreseVisibili, DtCentriVisibili As DataTable
        Dim i As Integer

        DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
        If DtImpreseVisibili IsNot Nothing AndAlso DtImpreseVisibili.Rows.Count > 0 Then
            Select Case TipoSelect
                Case enum_TipoSelect_FiltroneSuperNova.Appezzamenti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & If(noLock, " (NOLOCK) ", "") & " ON Appezzamento.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1 ")
                Case enum_TipoSelect_FiltroneSuperNova.Impianti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & If(noLock, " (NOLOCK) ", "") & " ON Reg_Impianti.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1  ")
                Case enum_TipoSelect_FiltroneSuperNova.Esercizi
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & If(noLock, " (NOLOCK) ", "") & " ON Imprese_Progetti.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1  ")
                Case enum_TipoSelect_FiltroneSuperNova.Imprese
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & If(noLock, " (NOLOCK) ", "") & " ON Imprese.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1  ")
                Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & If(noLock, " (NOLOCK) ", "") & " ON Imprese.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1  ")
                Case enum_TipoSelect_FiltroneSuperNova.Campi
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & If(noLock, " (NOLOCK) ", "") & " ON Imprese.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1  ")
                Case enum_TipoSelect_FiltroneSuperNova.Movimenti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & If(noLock, " (NOLOCK) ", "") & " ON Agenda.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1 ")
            End Select
        End If

        DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, "", "", objParametri)
        If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
            Select Case TipoSelect
                Case enum_TipoSelect_FiltroneSuperNova.Appezzamenti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & If(noLock, " (NOLOCK) ", "") & " ON Appezzamento.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Appezzamento.Sa_Cod = Utenti_Visibilita_Appoggio_C.Sa_Cod AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 2 ")
                Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & If(noLock, " (NOLOCK) ", "") & " ON Centri_Aziendali.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Centri_Aziendali.Sa_Cod = Utenti_Visibilita_Appoggio_C.Sa_Cod AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 2 ")
                Case enum_TipoSelect_FiltroneSuperNova.Campi
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & If(noLock, " (NOLOCK) ", "") & " ON Centri_Aziendali.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Centri_Aziendali.Sa_Cod = Utenti_Visibilita_Appoggio_C.Sa_Cod AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 2 ")
                Case enum_TipoSelect_FiltroneSuperNova.Impianti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & If(noLock, " (NOLOCK) ", "") & " ON Reg_Impianti.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Reg_Impianti.Sa_Cod = Utenti_Visibilita_Appoggio_C.Sa_Cod AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 2 ")
                Case enum_TipoSelect_FiltroneSuperNova.Esercizi
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & If(noLock, " (NOLOCK) ", "") & " ON Imprese_Progetti.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Imprese_Progetti.Sa_Cod = Utenti_Visibilita_Appoggio_C.Sa_Cod AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 2 ")
                    'CASE enum_TipoSelect_FiltroneSuperNova.Movimenti
                    '    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & If(noLock, " (NOLOCK) ", "") & " ON Agenda.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 1  AND Utenti_Visibilita_Appoggio.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'   " )
            End Select
        End If
#End Region

        Dim strParentesi As String = ""
        strParentesi = strParentesi.PadRight(nParentesiTondeAperte, "(")

        stb.AppendLine(" FROM " & strParentesi)
        'stb.AppendLine(" Imprese INNER JOIN UtentiXImprese ON Imprese.Piva = UtentixImprese.Piva) " )
        stb.AppendLine(" Imprese " & If(noLock, " (NOLOCK) ", "") & " ")
        stb.AppendLine(StrJOIN.ToString)

        stb.AppendLine(" Where (ImpresexIndirizzi.Tipo_Indirizzo = 1)")
        'stb.AppendLine(" AND   UtentixImprese.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' " )
        If objParametri.FinestraTemporaleFine <> AGRODATAFINE Then
            stb.AppendLine(" AND   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
        End If

        If objParametri.FinestraTemporaleInizio <> AGRODATAINIZIO Then
            stb.AppendLine(" AND   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
        End If

        If Trim(Filtro) <> "" Then

            ' se nel filtro non c'è AND lo aggiungo
            If Not Trim(Filtro).StartsWith("AND", True, Nothing) Then
                stb.AppendLine(" AND ")
            End If
            'StrSQL.AppendLine(Agro_SQL_SaveText(Filtro) )
            'non faccio l'Agro_SQL_SaveText perchè se arriva, ad esempio,
            'come Filtro  (Imprese.Rag_Soc like '%blablabla%')   
            'poi diventa  (Imprese.Rag_Soc like ''%blablabla%'') 
            'e da errore
            stb.AppendLine(Filtro)
        End If


        '----------------------------------------------------------------
        '--- Filtro associato all'utente 
        '----------------------------------------------------------------
        'Dim objUtentiVisibilita AS New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        'Dim UtenteProfiloImpreseSql AS String = ""
        'Dim UtenteProfiloCentriSql AS String = ""
        'Dim DtImpreseVisibili, DtCentriVisibili AS DataTable
        'Dim i AS Integer

        Select Case TipoSelect
            Case enum_TipoSelect_FiltroneSuperNova.Base,
                 enum_TipoSelect_FiltroneSuperNova.Contatti,
                 enum_TipoSelect_FiltroneSuperNova.ImpreseAlbero,
                 enum_TipoSelect_FiltroneSuperNova.Imprese_Codici
                DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
                If DtImpreseVisibili IsNot Nothing Then
                    For i = 0 To DtImpreseVisibili.Rows.Count - 1
                        UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
                    Next
                    If UtenteProfiloImpreseSql <> "" Then
                        UtenteProfiloImpreseSql = " AND Imprese.piva IN (" & Agro_SQL_Save_Clausola_IN(Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1), True) & ") "
                    End If
                End If
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, "", "", objParametri)
                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        UtenteProfiloCentriSql &= " (Centri_Aziendali.Piva='" & DtCentriVisibili.Rows(i).Item("piva") & "' AND Centri_Aziendali.sa_cod=" & DtCentriVisibili.Rows(i).Item("sa_cod") & ") OR "
                    Next
                    If UtenteProfiloCentriSql <> "" Then
                        UtenteProfiloCentriSql = " AND (" & Left(UtenteProfiloCentriSql, UtenteProfiloCentriSql.Length - 3) & ") "
                    End If
                End If
        End Select

        Select Case TipoSelect

            Case enum_TipoSelect_FiltroneSuperNova.Base '0

                stb.AppendLine(UtenteProfiloImpreseSql)
                stb.AppendLine(UtenteProfiloCentriSql)

            Case enum_TipoSelect_FiltroneSuperNova.ImpreseAlbero '1 'Solo Imprese x Albero

                stb.AppendLine(UtenteProfiloImpreseSql)

            Case enum_TipoSelect_FiltroneSuperNova.Imprese  '2 'Imprese

                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                stb.AppendLine(UtenteProfiloImpreseSql)

            Case enum_TipoSelect_FiltroneSuperNova.Movimenti

                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                stb.AppendLine(UtenteProfiloImpreseSql)

            Case enum_TipoSelect_FiltroneSuperNova.Imprese_Codici  '9 'Imprese_Codici

                stb.AppendLine(UtenteProfiloImpreseSql)

            Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali  '3 'Centri Aziendali

                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_C.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                stb.AppendLine(UtenteProfiloImpreseSql)
                stb.AppendLine(UtenteProfiloCentriSql)

            Case enum_TipoSelect_FiltroneSuperNova.Appezzamenti  '4 'Appezzamenti

                'stb.AppendLine(UtenteProfiloImpreseSql )
                'stb.AppendLine(UtenteProfiloCentriSql )

                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_C.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                If objParametri.FinestraTemporaleFine <> AGRODATAFINE Then
                    stb.AppendLine(" AND   Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                End If

                If objParametri.FinestraTemporaleInizio <> AGRODATAINIZIO Then
                    stb.AppendLine(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                End If


            Case enum_TipoSelect_FiltroneSuperNova.Impianti  '5 'Impianti

                'stb.AppendLine(UtenteProfiloImpreseSql )
                'stb.AppendLine(UtenteProfiloCentriSql )

                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_C.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                If objParametri.FinestraTemporaleFine <> AGRODATAFINE Then
                    stb.AppendLine(" AND   Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                End If

                If objParametri.FinestraTemporaleInizio <> AGRODATAINIZIO Then
                    stb.AppendLine(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                End If

            Case enum_TipoSelect_FiltroneSuperNova.Esercizi  '10 'Impianti

                'stb.AppendLine(UtenteProfiloImpreseSql )
                'stb.AppendLine(UtenteProfiloCentriSql )

                If DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_C.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                If objParametri.FinestraTemporaleFine <> AGRODATAFINE Then
                    stb.AppendLine(" AND   Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                End If

                If objParametri.FinestraTemporaleInizio <> AGRODATAINIZIO Then
                    stb.AppendLine(" AND   Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                End If

            'CASE enum_TipoSelect_FiltroneSuperNova.Movimenti  '6 'Movimenti

            '    stb.AppendLine(UtenteProfiloImpreseSql )
            '    stb.AppendLine(UtenteProfiloCentriSql )

            Case enum_TipoSelect_FiltroneSuperNova.Contatti  '7 'Contatti

                stb.AppendLine(UtenteProfiloImpreseSql)

            Case enum_TipoSelect_FiltroneSuperNova.Campi  '8 'Campi

                stb.AppendLine(UtenteProfiloImpreseSql)
                stb.AppendLine(UtenteProfiloCentriSql)

                stb.AppendLine(" AND   Campi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                stb.AppendLine(" AND   Campi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

        End Select

        If Ordinamento = "" Then

            Select Case TipoSelect

                Case 0

                    stb.AppendLine(" ORDER BY Imprese.Rag_Soc, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg, Agenda.Id_Agenda ASC") 'Importante

                Case 1

                    stb.AppendLine(" GROUP BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia    ")

                Case 4 'Appezzamenti

                    stb.AppendLine(" ORDER BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc  ")

                Case 5 'Impianti (Ordinamento per chiave in modo tale da facilitare l'esclusione dei duplicati)

                    'MODIFICA IN DATA 15/06/2012: richiesta di Terremerse: ordinare le aziende in ordine alfabetico (rag_soc)
                    'StrSQL.AppendLine(" ORDER BY Imprese.Piva, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg  " )
                    stb.AppendLine(" ORDER BY Imprese.rag_soc, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg  ")

                Case 8 'Campi

                    stb.AppendLine(" ORDER BY Imprese.Piva, Centri_Aziendali.Sa_Cod, Campi.Campo_Cod ")

                Case Else

                    stb.AppendLine(" ORDER BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc  ")

            End Select

        Else

            stb.AppendLine(" " & Ordinamento)

        End If

        Return stb.ToString()

    End Function

    '###############################################################################
    'E' l'equivalente del COM di Agro_Anagrafe_AD - UtentixImprese_Read - FiltroneSuperNova
    Public Function CreaDTFiltrone(ByVal jsonRichiesta As filtrone_richiesta_parametriQuery,
                                   ByRef objParametri As AgronicaCoreParametri,
                                   ByVal Filtro As String,
                                   ByVal TipoSelect As Integer,
                                   ByVal Ordinamento As String,
                                   ByVal Join As JoinFiltrone,
                                   Optional ByRef objParametriUtenti As AgronicaCoreParametri = Nothing
                                   ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtility.Filtrone.CreaDTFiltrone()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable


        Try

            Dim StrSQL As String = CreaStringaQueryPerDTFiltrone(jsonRichiesta, objParametri, Filtro, TipoSelect, Ordinamento, Join, 0, True, objParametriUtenti)

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL, NomeRoutine)
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
    Public Sub ImpostaVariabiliJOIN_xFiltroUtente(ByVal SQL_FiltroUtente As String, ByVal ClassJoin As JoinFiltrone)

        If SQL_FiltroUtente <> "" Then

            If InStr(SQL_FiltroUtente, "GerarchiaImprese") Then
                ClassJoin.bGerarchiaImprese = True
            End If

            If InStr(SQL_FiltroUtente, "Imprese_Codici") Then
                ClassJoin.bImpreseCodici = True
            End If

            If InStr(SQL_FiltroUtente, "ImpresexIndirizzi") Then
                ClassJoin.bListaProvince = True
            End If

            If InStr(SQL_FiltroUtente, "CentrixIndirizzi") Then
                ClassJoin.bListaProvinceCentro = True
            End If

            If InStr(SQL_FiltroUtente, "Centri_Aziendali_Codici") Then
                ClassJoin.bCentriAziendaliCodici = True
            End If

            If InStr(SQL_FiltroUtente, "Appezzamento") Then
                ClassJoin.bAppezzamento = True
            End If

            If InStr(SQL_FiltroUtente, "Appezzamento_Codici") Then
                ClassJoin.bAppezzamentoCodici = True
            End If

            If InStr(SQL_FiltroUtente, "AppezzamentixParticelle") Then
                ClassJoin.bAppezzamentixParticelle = True
            End If

            If InStr(SQL_FiltroUtente, "Reg_Impianti") Then
                ClassJoin.bRegImpianti = True
            End If

            If InStr(SQL_FiltroUtente, "Reg_Impianti_Codici") Then
                ClassJoin.bRegImpiantiCodici = True
            End If

            If InStr(SQL_FiltroUtente, "Imprese_Progetti") Then
                ClassJoin.bImpreseProgetti = True
            End If

            If InStr(SQL_FiltroUtente, "Campi") Then
                ClassJoin.bCampi = True
            End If

            If InStr(SQL_FiltroUtente, "GruppoVarietale") Then
                ClassJoin.bGruppoVarietale = True
            End If

            If InStr(SQL_FiltroUtente, "Cultivar") Then
                ClassJoin.bCultivar = True
            End If

            If InStr(SQL_FiltroUtente, "SpecieVegetali") Then
                ClassJoin.bSpecieVegetali = True
            End If

            If InStr(SQL_FiltroUtente, "GruppoVegetale") Then
                ClassJoin.bGruppoVegetale = True
            End If


            If InStr(SQL_FiltroUtente, "Agenda") Then
                ClassJoin.bAgenda = True
            End If

            If InStr(SQL_FiltroUtente, "Movimenti") Then
                ClassJoin.bMovimenti = True
            End If

            If InStr(SQL_FiltroUtente, "Movimenti_dettagli") Then
                ClassJoin.bMovimentiDettagli = True
            End If

            If InStr(SQL_FiltroUtente, "Mov_Destinazioni") Then
                ClassJoin.bMovDestinazioni = True
            End If

            If InStr(SQL_FiltroUtente, "Mov_Dettaglio_Tecnico") Then
                ClassJoin.bMovDettaglioTecnico = True
            End If


            If InStr(SQL_FiltroUtente, "Operazioni") Then
                ClassJoin.bOperazioni = True
            End If

            If InStr(SQL_FiltroUtente, "GruppoOperazioni") Then
                ClassJoin.bGruppoOperazioni = True
            End If


            If InStr(SQL_FiltroUtente, "Contatti") Then
                ClassJoin.bContatti = True
            End If

            If InStr(SQL_FiltroUtente, "Risorse_Umane") Then
                ClassJoin.bRisorseUmane = True
            End If

            If InStr(SQL_FiltroUtente, "RapportiContabili") Then
                ClassJoin.bRapportiContabili = True
            End If

        End If


    End Sub



    Public Sub ImpostaVariabiliJOIN(ByRef Join As JoinFiltrone)

        If Join.bIndirizzi Then
            Join.bImpreseXIndirizzi = True
        End If

        If Join.bIstat Then
            Join.bIndirizzi = True
            Join.bImpreseXIndirizzi = True
        End If

        If Join.bListaProvince Then
            Join.bIndirizzi = True
            Join.bImpreseXIndirizzi = True
            Join.bIstat = True
        End If

        If Join.bCentrixIndirizzi Then
            Join.bCentriAziendali = True
        End If

        If Join.bIndirizziCentro Then
            Join.bCentriAziendali = True
            Join.bCentrixIndirizzi = True
        End If

        If Join.bIstatCentro Then
            Join.bCentriAziendali = True
            Join.bCentrixIndirizzi = True
            Join.bIndirizziCentro = True
        End If

        If Join.bListaProvinceCentro Then
            Join.bCentriAziendali = True
            Join.bCentrixIndirizzi = True
            Join.bIndirizziCentro = True
            Join.bIstatCentro = True
        End If

        If Join.bAppezzamento Then
            Join.bCentriAziendali = True
        End If

        If Join.bRegImpianti Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
        End If

        If Join.bImpreseProgetti Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
        End If

        If Join.bCentriAziendaliCodici Then
            Join.bCentriAziendali = True
        End If

        If Join.bAppezzamentoCodici Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
        End If

        If Join.bRegImpiantiCodici Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
        End If

        If Join.bCampi Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
        End If

        If Join.bAppezzamentixParticelle Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
        End If

        If Join.bGruppoVarietale Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
        End If

        If Join.bCultivar Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
        End If

        If Join.bSpecieVegetali Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
            Join.bCultivar = True
        End If

        If Join.bGruppoVegetale Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
            Join.bCultivar = True
            Join.bSpecieVegetali = True
        End If

        If Join.bMovDestinazioni Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
        End If

        If Join.bAgenda Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
            Join.bMovDestinazioni = True
        End If

        If Join.bMovimenti Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
            Join.bMovDestinazioni = True
            Join.bAgenda = True
        End If


        If Join.bMovDettaglioTecnico Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
            Join.bMovDestinazioni = True
            Join.bAgenda = True
            Join.bMovimenti = True
        End If

        If Join.bMovimentiDettagli Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bMovDestinazioni = True
            Join.bRegImpianti = True
            Join.bAgenda = True
            Join.bMovimenti = True
        End If

        If Join.bOperazioni Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
            Join.bMovDestinazioni = True
            Join.bAgenda = True
        End If

        If Join.bGruppoOperazioni Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
            Join.bMovDestinazioni = True
            Join.bAgenda = True
            Join.bOperazioni = True
        End If

        If Join.bRisorseUmane Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bMovDestinazioni = True
            Join.bRegImpianti = True
            Join.bAgenda = True
            Join.bMovimenti = True
        End If

        If Join.bContatti Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
            Join.bMovDestinazioni = True
            Join.bAgenda = True
            Join.bMovimenti = True
            Join.bRisorseUmane = True
        End If

        If Join.bRapportiContabili Then
            Join.bCentriAziendali = True
            Join.bAppezzamento = True
            Join.bRegImpianti = True
            Join.bMovDestinazioni = True
            Join.bAgenda = True
            Join.bMovimenti = True
            Join.bRisorseUmane = True
        End If

    End Sub


    Public Function CreaFiltroDati(
        ByVal Qs_Funzione As String,
        ByVal JsonRichiesta As filtrone_richiesta_nuovo,
        ByVal ASG_SuperUser_CodFiscale As String,
        ByVal ASG_Utente_Username_Crypt As String,
        ByVal ASG_Utente_Password_Crypt As String,
        ByVal Param_Sql_Permessi As String,
        ByVal Param_Xml_Permessi As String,
        ByVal Sql_Permessi_Amministratore As String,
        ByVal DgrVuoti As String,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        ByRef OUTPUT_strFiltro As String,
        ByRef OUTPUT_dt_x_export_azienda As DataTable,
        ByRef OUTPUT_dt_x_export_centro As DataTable,
        ByRef OUTPUT_dt_x_export_appezza As DataTable,
        ByRef OUTPUT_dt_x_export_impianto As DataTable,
        ByRef OUTPUT_dt_x_export_distinta As DataTable,
        ByRef OUTPUT_dt_x_export_movimento As DataTable,
        ByRef OUTPUT_dt_x_export_fabbricato As DataTable, 'Anna 17/08/21:Aggiunta scheda fabbricati nel filtrone, per TTI
        ByRef OUTPUT_Xml_permessi As String,
        ByRef OUTPUT_Sql_Permessi As String,
        ByRef OUTPUT_Xml_Filtro As String,
        ByRef OUTPUT_Join As JoinFiltrone,
        Optional ByRef OUTPUT_dt_x_export_campo As DataTable = Nothing
    ) As String

        Dim parametriXQuery As New filtrone_richiesta_parametriQuery
        parametriXQuery.IncludiVisite = JsonRichiesta.filtroazioni.IncludiVisite

        Dim str_RISPOSTA As String = ""



        Dim Xml_Permessi As String


        Dim strFiltro As String
        Dim strFiltro_SenzaPadri As String
        'Dim objSql As New Codex_Utility_Sql_DistinctOnDT
        Dim i As Integer
        Dim classJoin As New JoinFiltrone
        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_DatiFiltri As System.Xml.XmlElement
        Dim XML_Filtro As System.Xml.XmlElement
        Dim XML_Impresa As System.Xml.XmlElement
        Dim XML_Struttura As System.Xml.XmlElement
        Dim XML_Appezzamento As System.Xml.XmlElement
        Dim XML_Impianto As System.Xml.XmlElement
        Dim XML_Agenda As System.Xml.XmlElement
        Dim XML_Contatto As System.Xml.XmlElement
        Dim XML_DatiGerarchiaImprese As System.Xml.XmlElement
        Dim XML_GerarchiaImprese As System.Xml.XmlElement
        Dim XML_CentroAziendale As System.Xml.XmlElement


        strFiltro = "" 'imposto stringa vuota

        'creo i nodi per la stringa xml

        XML_DatiFiltri = XmlDoc.CreateElement("DatiFiltri")

        XML_Filtro = XmlDoc.CreateElement("Filtro")
        XML_DatiFiltri.AppendChild(XML_Filtro)

        XML_Impresa = XmlDoc.CreateElement("Impresa")
        XML_Filtro.AppendChild(XML_Impresa)

        XML_Struttura = XmlDoc.CreateElement("Struttura")
        XML_Impresa.AppendChild(XML_Struttura)

        XML_Appezzamento = XmlDoc.CreateElement("Appezzamento")
        XML_Struttura.AppendChild(XML_Appezzamento)

        XML_Impianto = XmlDoc.CreateElement("Impianto")
        XML_Appezzamento.AppendChild(XML_Impianto)

        XML_Agenda = XmlDoc.CreateElement("Agenda")
        XML_Impianto.AppendChild(XML_Agenda)

        XML_Contatto = XmlDoc.CreateElement("Contatto")
        XML_Agenda.AppendChild(XML_Contatto)

        XML_DatiGerarchiaImprese = XmlDoc.CreateElement("DatiGerarchiaImprese")
        XML_Filtro.AppendChild(XML_DatiGerarchiaImprese)

        XML_CentroAziendale = XmlDoc.CreateElement("CentroAziendale")
        XML_DatiFiltri.AppendChild(XML_CentroAziendale)


        '================================
        '       IMPRESA
        '--------------------------------
#Region "IMPRESA"
        '-- PIVA
        If JsonRichiesta.filtrone_azienda.txPiva.Trim <> "" Then
            Dim piva As String = JsonRichiesta.filtrone_azienda.txPiva.Trim
            strFiltro &= "AND Imprese.Piva like '%" & Agro_SQL_SaveText(piva) & "%' "
            XML_Impresa.SetAttribute(LCase("piva"), piva)
        End If


        '-- RAGIONE SOCIALE
        If JsonRichiesta.filtrone_azienda.txtRagSoc.Trim <> "" Then
            strFiltro &= "AND Imprese.Rag_Soc like '%" & Agro_SQL_SaveText(JsonRichiesta.filtrone_azienda.txtRagSoc.Trim) & "%' "
            XML_Impresa.SetAttribute(LCase("ragione_sociale"), JsonRichiesta.filtrone_azienda.txtRagSoc.Trim)
        End If


        '--    CUAA
        If JsonRichiesta.filtrone_azienda.txtCuaa.Trim <> "" Then
            Dim cuaa As String = JsonRichiesta.filtrone_azienda.txtCuaa.Trim
            strFiltro &= " AND IC_cuaa.val_cod like '%" & Agro_SQL_SaveText(cuaa) & "%' "
            XML_Impresa.SetAttribute(LCase("cuaa"), cuaa)
        End If


        '*********************************************************************************************************
        'Modifica Mauro 5/12/2008
        'Ciclo su gli oggetti della lista regione
        Dim Num_Sel As Integer = 0
        Dim strZone As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_azienda.cbl_zone) Then
            For i = 0 To JsonRichiesta.filtrone_azienda.cbl_zone.Count - 1
                'If JsonRichiesta.filtrone_azienda.cbl_zone(i) Then
                strZone &= JsonRichiesta.filtrone_azienda.cbl_zone(i) & ","
                Num_Sel += 1
                'End If
            Next
        End If
        If strZone <> "" Then

            strZone = strZone.Substring(0, strZone.Length - 1)

            strFiltro &= " AND Imprese.Piva IN (SELECT DISTINCT IP.PIVA " &
                         "FROM  ImpreseXParticelle IP " &
                         "WHERE EXISTS " &
                            "(SELECT PC.PROV, PC.COM, PC.SEZIONE, PC.FOGLIO, PC.NUMERO, PC.SUBALTERNO " &
                            "FROM ZonexParticelle INNER JOIN " &
                            "ParticelleCatastali PC ON ZonexParticelle.PROV = PC.PROV AND ZonexParticelle.COM = PC.COM " &
                            "AND ZonexParticelle.SEZIONE = PC.SEZIONE AND ZonexParticelle.FOGLIO = PC.FOGLIO AND ZonexParticelle.NUMERO = PC.NUMERO " &
                            "AND ZonexParticelle.SUBALTERNO = PC.SUBALTERNO " &
                            "WHERE ZonexParticelle.Zona_Cod IN (" & Agro_SQL_Save_Clausola_IN(strZone) & ") AND (IP.PROV = PC.PROV AND IP.COM = PC.COM AND IP.SEZIONE = PC.SEZIONE " &
                            "AND IP.FOGLIO = PC.FOGLIO AND IP.NUMERO = PC.NUMERO AND IP.SUBALTERNO = PC.SUBALTERNO) " &
                            "GROUP BY PC.PROV, PC.COM, PC.SEZIONE, PC.FOGLIO, PC.NUMERO, PC.SUBALTERNO "
            If JsonRichiesta.filtrone_azienda.Rbl_Zone_And_Or = "AND" Then
                strFiltro &= " HAVING COUNT(*) = " & Num_Sel.ToString & "))"
            Else
                strFiltro &= "))"
            End If
        End If



        '-- MACROUSO
        Dim Num_Sel_macro As Integer = 0
        Dim strMacro As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_azienda.CBL_Macrouso) Then
            For i = 0 To JsonRichiesta.filtrone_azienda.CBL_Macrouso.Count - 1
                'If JsonRichiesta.filtrone_azienda.CBL_Macrouso(i).Selected Then
                strMacro &= "'" & JsonRichiesta.filtrone_azienda.CBL_Macrouso(i) & "',"
                Num_Sel_macro += 1
                'End If
            Next
        End If
        If strMacro <> "" Then
            'tolgo la virgola finale 
            strMacro = strMacro.Substring(0, strMacro.Length - 1)

            strFiltro &= " AND Imprese.Piva IN ("
            strFiltro &= " Select PIVA from "
            strFiltro &= " (SELECT TOP 100 percent IP.PIVA , PM.Macrouso_Cod, sum(PM.Superficie)as Somma_Sup "
            strFiltro &= " FROM          ImpreseXParticelle IP"
            strFiltro &= " inner join ParticelleCatastalixMacrousi PM on PM.PROV = IP.PROV "
            strFiltro &= " AND PM.COM = IP.COM "
            strFiltro &= " AND PM.SEZIONE = IP.SEZIONE "
            strFiltro &= " And PM.FOGLIO = IP.FOGLIO "
            strFiltro &= " And PM.NUMERO = IP.NUMERO "
            strFiltro &= " And PM.SUBALTERNO = IP.SUBALTERNO "

            strFiltro &= "     AND ( PM.macrouso_cod in ( " & Agro_SQL_Save_Clausola_IN(strMacro, True) & "))"

            strFiltro &= "     Group by IP.PIVA , PM.Macrouso_Cod"
            strFiltro &= " order by PIVA , PM.Macrouso_Cod) "
            strFiltro &= "    as TABELLA1"
            strFiltro &= "    group by PIVA "

            JsonRichiesta.filtrone_azienda.Txt_SupMacrousi = JsonRichiesta.filtrone_azienda.Txt_SupMacrousi.Replace(".", ",")
            If JsonRichiesta.filtrone_azienda.Rbl_Macrousi_And_Or = "AND" Then
                strFiltro &= " having "
                strFiltro &= " count(*) = " & Num_Sel_macro
                If IsNumeric(JsonRichiesta.filtrone_azienda.Txt_SupMacrousi.Trim) Then
                    'If chk_SupMacro.Checked = True Then
                    strFiltro &= " AND SUM(somma_sup) " & JsonRichiesta.filtrone_azienda.Rbl_min_mag_Superficie_Macrousi & JsonRichiesta.filtrone_azienda.Txt_SupMacrousi.Trim
                    'End If
                End If
            Else
                If IsNumeric(JsonRichiesta.filtrone_azienda.Txt_SupMacrousi.Trim) Then
                    'If chk_SupMacro.Checked = True Then
                    strFiltro &= " having SUM(somma_sup) " & JsonRichiesta.filtrone_azienda.Rbl_min_mag_Superficie_Macrousi & JsonRichiesta.filtrone_azienda.Txt_SupMacrousi.Trim
                    'End if 
                End If
            End If

            strFiltro &= " )"

        End If


        '-- SUPERFICIE CATASTALE
        If IsNumeric(JsonRichiesta.filtrone_azienda.txt_SupCat) Then
            JsonRichiesta.filtrone_azienda.txt_SupCat = JsonRichiesta.filtrone_azienda.txt_SupCat.Replace(".", ",")

            strFiltro &= " AND Imprese.Piva IN (SELECT   I.piva " &
                         " FROM     UtentixImprese , Imprese AS I ,IMPRESEXPARTICELLE AS IP, PARTICELLECATASTALI AS PC" &
                         " WHERE    UtentixImprese.PIVA = I.PIVA " &
                         " AND UtentixImprese.[User] = '" & Agro_SQL_SaveText(ASG_SuperUser_CodFiscale) & "' " &
                         " AND IP.Piva = I.Piva AND PC.Prov = IP.Prov AND PC.Com = IP.Com AND PC.Sezione = IP.Sezione AND PC.Foglio = IP.Foglio " &
                         " AND PC.Numero = IP.Numero AND PC.Subalterno = IP.Subalterno " &
                         " GROUP BY I.PIVA " &
                         " HAVING  SUM(convert(float,PC.ETTARI) + (((convert(float,PC.ARE) * 100) + (convert(float,PC.CENTIARE))) / 10000))" &
                         JsonRichiesta.filtrone_azienda.Rbl_min_mag_Superficie_Catastale & Agro_SQL_SaveText(JsonRichiesta.filtrone_azienda.txt_SupCat) & ") "
        End If

        Dim StrParoleChiave As String
        Dim iPC As Int32

        'Parole Chiave Impresa
        If Not IsNothing(JsonRichiesta.filtrone_azienda.CBL_PChiaveImpresa) Then
            If JsonRichiesta.filtrone_azienda.CBL_PChiaveImpresa.Count > 0 Then

                StrParoleChiave = ""
                For iPC = 0 To JsonRichiesta.filtrone_azienda.CBL_PChiaveImpresa.Count - 1
                    'If JsonRichiesta.filtrone_azienda.CBL_PChiaveImpresa(iPC).Selected = True Then
                    'Prima parola
                    If StrParoleChiave = "" Then
                        StrParoleChiave = "'" & JsonRichiesta.filtrone_azienda.CBL_PChiaveImpresa(iPC) & "'"
                    Else
                        StrParoleChiave = StrParoleChiave & ", '" & JsonRichiesta.filtrone_azienda.CBL_PChiaveImpresa(iPC) & "'"
                    End If
                    'End If
                Next iPC

                If StrParoleChiave <> "" Then

                    strFiltro &= " AND Imprese.Piva IN (SELECT   I.piva " &
                                                             " FROM     ParoleChiavexImprese , Imprese AS I " &
                                                             " WHERE    ParoleChiavexImprese.PIVA = I.PIVA " &
                                                             " AND ParoleChiavexImprese.pivasuperuser = '" & CStr(objParametri_Server.PivaSuperUser) & "' " &
                                                             " AND ParoleChiavexImprese.sa_cod = 0 " &
                                                             " AND ParoleChiavexImprese.ParolaChiave IN(" & Agro_SQL_Save_Clausola_IN(StrParoleChiave, True) & ")" &
                                                             " GROUP BY I.PIVA )"

                End If

            End If
        End If


        '*********************************************************************************************************
        '------------------------------------
        'BY MAGA - 04/03/2009
        'Imposto i join per il filtro impostato sull'utente
        'che sono salvati in HttpContext.Current.Session("Sql_Permessi")
        ImpostaVariabiliJOIN_xFiltroUtente(Param_Sql_Permessi, classJoin)
        '------------------------------------

        '-- REGIONE
        Dim Num_Reg As Integer = 1
        Dim strRegione As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_azienda.CBL_Regioni) Then
            For i = 0 To JsonRichiesta.filtrone_azienda.CBL_Regioni.Count - 1

                strRegione = strRegione & " '" & JsonRichiesta.filtrone_azienda.CBL_Regioni(i) & "',"
                XML_Impresa.SetAttribute(LCase("reg") & Num_Reg, JsonRichiesta.filtrone_azienda.CBL_Regioni(i))
                Num_Reg += 1

            Next
        End If
        'Se la stringa non è vuota l'aggiungo al filtro....
        If strRegione <> "" Then
            'tolgo l'ultima virgola.....
            strRegione = strRegione.Substring(0, strRegione.LastIndexOf(","))
            strFiltro &= "AND Lista_Province.Reg IN (" & Agro_SQL_Save_Clausola_IN(strRegione, True) & ") "

            classJoin.bListaProvince = True

        End If


        '-- PROVINCIA
        Dim Num_Prov As Integer = 1
        Dim strProvincia As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_azienda.CBL_Province) Then
            For i = 0 To JsonRichiesta.filtrone_azienda.CBL_Province.Count - 1
                If JsonRichiesta.filtrone_azienda.CBL_Province(i) Then
                    strProvincia = strProvincia & " '" & JsonRichiesta.filtrone_azienda.CBL_Province(i) & "',"
                    XML_Impresa.SetAttribute(LCase("pro_cod_istat") & Num_Prov, JsonRichiesta.filtrone_azienda.CBL_Province(i))
                    Num_Prov += 1
                End If
            Next
        End If
        'Se la stringa non è vuota l'aggiungo al filtro....
        If strProvincia <> "" Then
            'tolgo l'ultima virgola.....
            strProvincia = strProvincia.Substring(0, strProvincia.LastIndexOf(","))
            strFiltro &= "AND Indirizzi.Pro_Cod_Istat IN (" & Agro_SQL_Save_Clausola_IN(strProvincia, True) & ") "

            classJoin.bIndirizzi = True

        End If


        '-- COMUNE
        Dim com_cod_istat As String = ""
        If JsonRichiesta.filtrone_azienda.Cmb_Comuni <> "" Then
            com_cod_istat = Mid(CStr(JsonRichiesta.filtrone_azienda.Cmb_Comuni), 4, 3)
            strFiltro &= "AND Indirizzi.Com_Cod_Istat = '" & com_cod_istat & "' "
            XML_Impresa.SetAttribute(LCase("com_cod_istat"), com_cod_istat)

            classJoin.bIndirizzi = True
        End If


        '-- DATA COSTITUZIONE
        If IsDate(JsonRichiesta.filtrone_azienda.txtDataCost) Then
            Dim strSegno As String = ""
            'successiva a...
            If JsonRichiesta.filtrone_azienda.rblDataCost = "0" Then
                strSegno = " >= "
                XML_Impresa.SetAttribute(LCase("validita_inizio_superiore_az"), JsonRichiesta.filtrone_azienda.txtDataCost.Trim)
                'precedente a...
            ElseIf JsonRichiesta.filtrone_azienda.rblDataCost = "1" Then
                strSegno = " <="
                XML_Impresa.SetAttribute(LCase(" validita_inizio_inferiore_az"), JsonRichiesta.filtrone_azienda.txtDataCost.Trim)
            End If
            strFiltro &= " AND Imprese.Validita_Inizio" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_azienda.txtDataCost) & " ")
        End If


        '-- DATA CESSAZIONE
        If IsDate(JsonRichiesta.filtrone_azienda.txtDataCess) Then
            Dim strSegno As String = ""
            'successiva a...
            If JsonRichiesta.filtrone_azienda.rblDataCess = " 0" Then
                strSegno = ">= "
                XML_Impresa.SetAttribute(LCase("validita_fine_superiore_az"), JsonRichiesta.filtrone_azienda.txtDataCess.Trim)
                'precedente a...
            ElseIf JsonRichiesta.filtrone_azienda.rblDataCess = "1" Then
                strSegno = " <="
                XML_Impresa.SetAttribute(LCase(" validita_fine_inferiore_az"), JsonRichiesta.filtrone_azienda.txtDataCess.Trim)
            End If
            strFiltro &= " AND Imprese.Validita_Fine" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_azienda.txtDataCess) & " ")
        End If


        '-- CODICE AZIENDA
        If JsonRichiesta.filtrone_azienda.cmb_codiceazienda <> "0" AndAlso JsonRichiesta.filtrone_azienda.Txt_CodiceAzienda <> "" Then
            strFiltro &= "AND Imprese_Codici.id_cod = " & JsonRichiesta.filtrone_azienda.cmb_codiceazienda & " "
            strFiltro &= "AND Imprese_Codici.val_cod like '%" & JsonRichiesta.filtrone_azienda.Txt_CodiceAzienda & "%' "
            XML_Impresa.SetAttribute(LCase("codice"), JsonRichiesta.filtrone_azienda.cmb_codiceazienda)
            XML_Impresa.SetAttribute(LCase("codice_valore"), JsonRichiesta.filtrone_azienda.Txt_CodiceAzienda.Trim)

            classJoin.bImpreseCodici = True
        End If


        '-- TECNICI
        Dim strTecnici As String = ""
        Dim Num_Tecnici As Integer = 1
        If Not IsNothing(JsonRichiesta.filtrone_azienda.CBL_Tecnici) Then
            For i = 0 To JsonRichiesta.filtrone_azienda.CBL_Tecnici.Count - 1
                'If JsonRichiesta.filtrone_azienda.CBL_Tecnici.Items(i).Selected Then
                strTecnici = strTecnici & " '" & JsonRichiesta.filtrone_azienda.CBL_Tecnici(i) & "',"
                XML_Impresa.SetAttribute(LCase("tecnico") & Num_Tecnici, JsonRichiesta.filtrone_azienda.CBL_Tecnici(i))
                Num_Tecnici += 1
                'End If
            Next
        End If
        'Se la stringa non è vuota l'aggiungo al filtro....
        If strTecnici <> "" Then
            strTecnici = strTecnici.Substring(0, strTecnici.LastIndexOf(","))
            strFiltro &= "AND (Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.Tecnico & " "
            strFiltro &= "AND Imprese_Codici.val_cod IN (" & Agro_SQL_Save_Clausola_IN(strTecnici, True) & ")) "

            classJoin.bImpreseCodici = True
        End If


        '-- TITOLO POSSESSO
        If Not IsNothing(JsonRichiesta.filtrone_azienda.cmbTitoloPossessoAzienda) AndAlso JsonRichiesta.filtrone_azienda.cmbTitoloPossessoAzienda <> "0" Then
            strFiltro &= "AND Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.TitoloPossesso & " "
            strFiltro &= "AND Imprese_Codici.val_cod = '" & JsonRichiesta.filtrone_azienda.cmbTitoloPossessoAzienda & "' "
            XML_Impresa.SetAttribute(LCase("TitoloPossesso"), enum_CodiciAnagrafe.TitoloPossesso)
            XML_Impresa.SetAttribute(LCase("TitoloPossesso_valore"), JsonRichiesta.filtrone_azienda.cmbTitoloPossessoAzienda)

            classJoin.bImpreseCodici = True
        End If


        '-- UTENTE
        Dim strUtenti As String = ""
        Dim Num_Utenti As Integer = 1
        If Not IsNothing(JsonRichiesta.filtrone_azienda.CBL_Utenti) Then
            For i = 0 To JsonRichiesta.filtrone_azienda.CBL_Utenti.Count - 1
                If JsonRichiesta.filtrone_azienda.CBL_Utenti(i) Then
                    strUtenti = strUtenti & " '" & JsonRichiesta.filtrone_azienda.CBL_Utenti(i) & "',"
                    XML_Impresa.SetAttribute(LCase("utente") & Num_Utenti, JsonRichiesta.filtrone_azienda.CBL_Utenti(i))
                    Num_Utenti += 1
                End If
            Next
        End If
        'se la stringa non è vuota l'aggiungo al filtro....
        If strUtenti <> "" Then
            strUtenti = strUtenti.Substring(0, strUtenti.LastIndexOf(","))
            strFiltro &= "AND (Imprese.Username_Creazione IN (" & Agro_SQL_Save_Clausola_IN(strUtenti, True) & ") OR Imprese.Username_Modifica IN (" & Agro_SQL_Save_Clausola_IN(strUtenti, True) & "))"
        End If
#End Region


        '================================
        '       CENTRO
        '--------------------------------
#Region "CENTRO"
        '-- NOME CENTRO 
        If JsonRichiesta.filtrone_centro.txtCentroAziendale.Trim <> "" Then
            strFiltro &= "AND Centri_Aziendali.Sa_Nome like '%" & Agro_SQL_SaveText(JsonRichiesta.filtrone_centro.txtCentroAziendale.Trim) & "%' "
            XML_Struttura.SetAttribute(LCase("sa_nome"), JsonRichiesta.filtrone_centro.txtCentroAziendale.Trim)

            classJoin.bCentriAziendali = True
            classJoin.bCentriAziendaliCodici = True
        End If


        '-- TIPO CENTRO
        If JsonRichiesta.filtrone_centro.CmbTipoCentro <> "0" Then
            strFiltro &= "AND Centri_Aziendali_Codici.id_cod = " & JsonRichiesta.filtrone_centro.CmbTipoCentro & " "
            strFiltro &= "AND Centri_Aziendali_Codici.val_cod ='" & JsonRichiesta.filtrone_centro.CmbTipoCentro & "' "
            XML_Struttura.SetAttribute(LCase("TipoCentro"), JsonRichiesta.filtrone_centro.CmbTipoCentro)
            XML_Struttura.SetAttribute(LCase("TipoCentro_valore"), JsonRichiesta.filtrone_centro.CmbTipoCentro)

            classJoin.bCentriAziendali = True
            classJoin.bCentriAziendaliCodici = True
        End If


        '-- TIPO ATTIVITA
        If JsonRichiesta.filtrone_centro.CmbTipoAttivita <> " " Then
            strFiltro &= "AND Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.TipoAttivita & " "
            strFiltro &= "AND Centri_Aziendali_Codici.val_cod ='" & JsonRichiesta.filtrone_centro.CmbTipoAttivita & "' "
            XML_Struttura.SetAttribute(LCase("TipoAttivita"), enum_CodiciAnagrafe.TipoAttivita)
            XML_Struttura.SetAttribute(LCase("TipoAttivita_valore"), JsonRichiesta.filtrone_centro.CmbTipoAttivita)

            classJoin.bCentriAziendali = True
            classJoin.bCentriAziendaliCodici = True
        End If


        '-- OTE
        If JsonRichiesta.filtrone_centro.CmbOTE <> " " Then
            strFiltro &= "AND Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.OTE & " "
            strFiltro &= "AND Centri_Aziendali_Codici.val_cod LIKE '%" & JsonRichiesta.filtrone_centro.CmbOTE & "%' "
            XML_Struttura.SetAttribute(LCase("OTE"), enum_CodiciAnagrafe.OTE)
            XML_Struttura.SetAttribute(LCase("OTE_valore"), JsonRichiesta.filtrone_centro.CmbOTE)

            classJoin.bCentriAziendali = True
            classJoin.bCentriAziendaliCodici = True
        End If


        '--  TITOLO POSSESSO
        If JsonRichiesta.filtrone_centro.cmbTitoloPossessoStruttura <> "0" Then
            strFiltro &= "AND Centri_Aziendali_Codici.id_cod = " & enum_CodiciAnagrafe.TitoloPossesso & " "
            strFiltro &= "AND Centri_Aziendali_Codici.val_cod = '" & JsonRichiesta.filtrone_centro.cmbTitoloPossessoStruttura & "' "
            XML_Struttura.SetAttribute(LCase("TitoloPossesso"), enum_CodiciAnagrafe.TitoloPossesso)
            XML_Struttura.SetAttribute(LCase("TitoloPossesso_valore"), JsonRichiesta.filtrone_centro.cmbTitoloPossessoStruttura)

            classJoin.bCentriAziendali = True
            classJoin.bCentriAziendaliCodici = True
        End If


        '-- CODICE STRUTTURA
        If JsonRichiesta.filtrone_centro.Cmb_CodiceStruttura <> "0" Then
            strFiltro &= "AND Centri_Aziendali_Codici.id_cod = " & JsonRichiesta.filtrone_centro.Cmb_CodiceStruttura & " "
            strFiltro &= "AND Centri_Aziendali_Codici.val_cod like '%" & JsonRichiesta.filtrone_centro.Txt_CodiceStruttura & "%' "
            XML_Struttura.SetAttribute(LCase("codice"), JsonRichiesta.filtrone_centro.Cmb_CodiceStruttura)
            XML_Struttura.SetAttribute(LCase("codice_valore"), JsonRichiesta.filtrone_centro.Txt_CodiceStruttura)

            classJoin.bCentriAziendali = True
            classJoin.bCentriAziendaliCodici = True
        End If


        '-- ORGANISMO DI CONTROLLO
        If JsonRichiesta.filtrone_centro.CmbOrganismoControllo <> "0" Then
            strFiltro &= "AND Centri_Aziendali_Codici.id_cod = " & JsonRichiesta.filtrone_centro.CmbOrganismoControllo & " "
            strFiltro &= "AND Centri_Aziendali_Codici.val_cod ='0' "
            XML_Struttura.SetAttribute(LCase("Organismicontrollo"), JsonRichiesta.filtrone_centro.CmbOrganismoControllo)
            XML_Struttura.SetAttribute(LCase("Organismicontrollo_valore"), "0")

            classJoin.bCentriAziendali = True
            classJoin.bCentriAziendaliCodici = True
        End If


        '-- CARTOGRAFICA
        Select Case JsonRichiesta.filtrone_centro.Cmb_Cartografia
            Case 1 'Cartografia Presente
                strFiltro &= "AND Centri_Aziendali.Sa_Cod IN (select centrixsfondi.sa_cod from centrixsfondi where centrixsfondi.sa_cod = centri_aziendali.sa_cod and centrixsfondi.Piva = centri_aziendali.piva) "
                classJoin.bCentriAziendali = True
            Case 2 'Cartografia Assente
                strFiltro &= "AND Centri_Aziendali.Sa_Cod NOT IN (select centrixsfondi.sa_cod from centrixsfondi where centrixsfondi.sa_cod = centri_aziendali.sa_cod and centrixsfondi.Piva = centri_aziendali.piva) "
                classJoin.bCentriAziendali = True
        End Select


        'Parole Chiave Centro
        If Not IsNothing(JsonRichiesta.filtrone_centro.CBL_PChiaveCentro) Then
            StrParoleChiave = ""
            For iPC = 0 To JsonRichiesta.filtrone_centro.CBL_PChiaveCentro.Count - 1
                'If JsonRichiesta.filtrone_centro.CBL_PChiaveCentro(iPC).Selected = True Then
                'Prima parola
                If StrParoleChiave = "" Then
                    StrParoleChiave = "'" & JsonRichiesta.filtrone_centro.CBL_PChiaveCentro(iPC) & "'"
                Else
                    StrParoleChiave = StrParoleChiave & ", '" & JsonRichiesta.filtrone_centro.CBL_PChiaveCentro(iPC) & "'"
                End If
                'End If
            Next iPC

            If StrParoleChiave <> "" Then
                strFiltro &= " AND Centri_Aziendali.Sa_Cod IN (SELECT   ca.sa_cod " &
                                         " FROM     centri_aziendali as CA, ParoleChiavexImprese " &
                                         " WHERE ParoleChiavexImprese.PIVA = ca.PIVA" &
                                         " AND ParoleChiavexImprese.sa_cod = ca.sa_cod " &
                                         " AND ParoleChiavexImprese.ParolaChiave IN(" & Agro_SQL_Save_Clausola_IN(StrParoleChiave, True) & ")" &
                                         " GROUP BY ca.sa_cod)"
            End If

        End If


        '-- STATO
        Dim Num_Stati_2 As Integer = 1
        Dim strStato_2 As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_centro.CBL_Stati_2) Then
            For i = 0 To JsonRichiesta.filtrone_centro.CBL_Stati_2.Count - 1
                Dim stato = JsonRichiesta.filtrone_centro.CBL_Stati_2(i)
                strStato_2 = strStato_2 & " '" & stato & "',"
                If stato = "IT" Then
                    strStato_2 = strStato_2 & " 'ITALIA',"
                End If
                XML_Struttura.SetAttribute(LCase("stato") & Num_Stati_2, stato)
                Num_Stati_2 += 1
            Next
        End If
        'Se la stringa non è vuota l'aggiungo al filtro....
        If strStato_2 <> "" Then
            'tolgo l'ultima virgola.....
            strStato_2 = strStato_2.Substring(0, strStato_2.LastIndexOf(","))
            strFiltro &= "AND Indirizzi_Centro.stato IN (" & Agro_SQL_Save_Clausola_IN(strStato_2, True) & ") "
            classJoin.bIndirizziCentro = True
        End If


        '-- REGIONI
        Dim Num_Reg_2 As Integer = 1
        Dim strRegione_2 As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_centro.CBL_Regioni_2) Then
            For i = 0 To JsonRichiesta.filtrone_centro.CBL_Regioni_2.Count - 1
                'If JsonRichiesta.filtrone_centro.CBL_Regioni_2(i).Selected Then
                strRegione_2 = strRegione_2 & " '" & JsonRichiesta.filtrone_centro.CBL_Regioni_2(i) & "',"
                XML_Struttura.SetAttribute(LCase("reg") & Num_Reg_2, JsonRichiesta.filtrone_centro.CBL_Regioni_2(i))
                Num_Reg_2 += 1
                'End If
            Next
        End If
        'Se la stringa non è vuota l'aggiungo al filtro....
        If strRegione_2 <> "" Then
            'tolgo l'ultima virgola.....
            strRegione_2 = strRegione_2.Substring(0, strRegione_2.LastIndexOf(","))
            strFiltro &= "AND Lista_Province_Centro.Reg IN (" & Agro_SQL_Save_Clausola_IN(strRegione_2, True) & ") "

            classJoin.bListaProvinceCentro = True

        End If


        '-- PROVINCIA
        Dim Num_Prov_2 As Integer = 1
        Dim strProvincia_2 As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_centro.CBL_Province_2) Then
            For i = 0 To JsonRichiesta.filtrone_centro.CBL_Province_2.Count - 1
                'If JsonRichiesta.filtrone_centro.CBL_Province_2(i) Then
                strProvincia_2 = strProvincia_2 & " '" & JsonRichiesta.filtrone_centro.CBL_Province_2(i) & "',"
                XML_Struttura.SetAttribute(LCase("pro_cod_istat") & Num_Prov_2, JsonRichiesta.filtrone_centro.CBL_Province_2(i))
                Num_Prov_2 += 1
                'End If
            Next
        End If
        'Se la stringa non è vuota l'aggiungo al filtro....
        If strProvincia_2 <> "" Then
            'tolgo l'ultima virgola.....
            strProvincia_2 = strProvincia_2.Substring(0, strProvincia_2.LastIndexOf(","))
            strFiltro &= "AND Indirizzi_Centro.Pro_Cod_Istat IN (" & Agro_SQL_Save_Clausola_IN(strProvincia_2, True) & ") "

            classJoin.bIndirizziCentro = True
        End If


        '-- COMUNE
        'ciclo su gli oggetti della lista comuni
        Dim Num_Com_2 As Integer = 1
        Dim strComuni_2 As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_centro.Cmb_Comuni_2) Then
            For i = 0 To JsonRichiesta.filtrone_centro.Cmb_Comuni_2.Count - 1
                Dim comune = JsonRichiesta.filtrone_centro.Cmb_Comuni_2(i).Split("_")
                'If JsonRichiesta.filtrone_centro.Cmb_Comuni_2(i) Then
                strComuni_2 = strComuni_2 & " '" & If(comune.Length > 1, comune(1), comune(0)) & "',"
                XML_Struttura.SetAttribute(LCase("com_cod_istat") & Num_Com_2, If(comune.Length > 1, comune(1), comune(0)))
                Num_Com_2 += 1
                'End If
            Next
        End If
        'Se la stringa non è vuota l'aggiungo al filtro....
        If strComuni_2 <> "" Then
            'tolgo l'ultima virgola.....
            strComuni_2 = strComuni_2.Substring(0, strComuni_2.LastIndexOf(","))
            strFiltro &= "AND Indirizzi_Centro.Com_Cod_Istat IN (" & Agro_SQL_Save_Clausola_IN(strComuni_2, True) & ") "

            classJoin.bIndirizziCentro = True
        End If


        '-- CODICI
        Dim Array_CodiciCentro As String()
        Array_CodiciCentro = Split((JsonRichiesta.filtrone_centro.Txt_CodiceCentro), ";")
        Dim n_Centro As Integer
        If JsonRichiesta.filtrone_centro.Cmb_CodiceCentro <> "0" AndAlso JsonRichiesta.filtrone_centro.Txt_CodiceCentro.Trim <> "" Then

            strFiltro &= " AND EXISTS ("
            strFiltro &= "      SELECT * FROM Centri_Aziendali_Codici ccod " & vbCrLf
            strFiltro &= "      WHERE ccod.id_cod = " & JsonRichiesta.filtrone_centro.Cmb_CodiceCentro & " " & vbCrLf
            strFiltro &= "      AND Centri_Aziendali.PIVA = ccod.piva and Centri_Aziendali.SA_COD = ccod.sa_cod " & vbCrLf

            If Array_CodiciCentro.Length = 1 Then
                strFiltro &= "      AND (ccod.val_cod like '%" & JsonRichiesta.filtrone_centro.Txt_CodiceCentro & "%'  " & vbCrLf
            Else
                strFiltro &= "      AND ("
                For n_Centro = 0 To UBound(Array_CodiciCentro)
                    strFiltro &= " ccod.val_cod like '%" & Array_CodiciCentro(n_Centro) & "%' OR"
                Next
                strFiltro = Left(strFiltro, strFiltro.Length - 2)
                strFiltro &= ")"
            End If
            strFiltro &= "))"

            XML_CentroAziendale.SetAttribute(LCase("codice"), JsonRichiesta.filtrone_centro.Cmb_CodiceCentro)
            XML_CentroAziendale.SetAttribute(LCase("codice_valore"), JsonRichiesta.filtrone_centro.Txt_CodiceCentro.Trim)

            classJoin.bCentriAziendaliCodici = True
        End If

#End Region


        '================================
        '       APPEZZAMENTO
        '--------------------------------
#Region "APPEZZAMENTO"
        '-- NOME APPEZZAMENTO
        If JsonRichiesta.filtrone_appezzamento.TxtAppNome <> "" Then
            strFiltro &= "AND Appezzamento.APP_NOME like '%" & Agro_SQL_SaveText(JsonRichiesta.filtrone_appezzamento.TxtAppNome) & "%' "
            XML_Appezzamento.SetAttribute(LCase("app_nome"), JsonRichiesta.filtrone_appezzamento.TxtAppNome.Trim)

            classJoin.bAppezzamento = True
        End If


        '-- NOME CAMPO
        If JsonRichiesta.filtrone_appezzamento.TxtCampoDes <> "" Then
            strFiltro &= "AND Campi.Campo_Des like '%" & Agro_SQL_SaveText(JsonRichiesta.filtrone_appezzamento.TxtCampoDes) & "%' "
            XML_Appezzamento.SetAttribute(LCase("Campo_Des"), JsonRichiesta.filtrone_appezzamento.TxtCampoDes)

            classJoin.bCampi = True
        End If


        '-- SUPERFICIE
        If IsNumeric(JsonRichiesta.filtrone_appezzamento.txtSupApp) Then
            Dim strSegno As String = ""
            'superficie maggiore
            If JsonRichiesta.filtrone_appezzamento.rblSupApp = "0" Then
                strSegno = " > "
                XML_Appezzamento.SetAttribute(LCase("superficie_superiore_ap"), JsonRichiesta.filtrone_appezzamento.txtSupApp.Trim)
                'superficie minore
            ElseIf JsonRichiesta.filtrone_appezzamento.rblSupApp = "1" Then
                strSegno = " < "
                XML_Appezzamento.SetAttribute(LCase(" superficie_inferiore_ap"), JsonRichiesta.filtrone_appezzamento.txtSupApp.Trim)
            End If
            strFiltro &= " AND Appezzamento.Sup_App " & strSegno & Agro_SQL_SaveText(JsonRichiesta.filtrone_appezzamento.txtSupApp) & " "

            classJoin.bAppezzamento = True
        End If


        '-- TITOLO POSSESSO
        If Not IsNothing(JsonRichiesta.filtrone_appezzamento.cmbTitoloPossessoAppezzamento) AndAlso JsonRichiesta.filtrone_appezzamento.cmbTitoloPossessoAppezzamento <> "0" Then
            strFiltro &= "AND Appezzamento_Codici.id_cod = " & enum_CodiciAnagrafe.TitoloPossesso & " "
            strFiltro &= "AND Appezzamento_Codici.val_cod = '" & JsonRichiesta.filtrone_appezzamento.cmbTitoloPossessoAppezzamento & "' "
            XML_Appezzamento.SetAttribute(LCase("TitoloPossesso"), enum_CodiciAnagrafe.TitoloPossesso)
            XML_Appezzamento.SetAttribute(LCase("TitoloPossesso_valore"), JsonRichiesta.filtrone_appezzamento.cmbTitoloPossessoAppezzamento)


            classJoin.bAppezzamentoCodici = True
        End If


        '-- METODO PRODUZIONE
        Select Case JsonRichiesta.filtrone_appezzamento.OptionList_MetodoProduzioneAppezzamento
            Case 1, 2, 3
                strFiltro &= "AND Appezzamento_Codici.id_cod = " & enum_CodiciAnagrafe.MetodoDiProduzione & " "
                strFiltro &= "AND Appezzamento_Codici.val_cod = '" & JsonRichiesta.filtrone_appezzamento.OptionList_MetodoProduzioneAppezzamento & "' "
                XML_Appezzamento.SetAttribute(LCase("MetodoProduzione"), enum_CodiciAnagrafe.MetodoDiProduzione)
                XML_Appezzamento.SetAttribute(LCase("MetodoProduzione_valore"), JsonRichiesta.filtrone_appezzamento.OptionList_MetodoProduzioneAppezzamento)

                classJoin.bAppezzamentoCodici = True
        End Select


        '-- FINE IMPIEGO
        If JsonRichiesta.filtrone_appezzamento.Txt_DataFineImpiego.Trim <> "" Then
            strFiltro &= "AND Appezzamento_Codici.id_cod = " & enum_CodiciAnagrafe.DataFineImpiegoPNC & " "
            strFiltro &= "AND Appezzamento_Codici.val_cod = '" & JsonRichiesta.filtrone_appezzamento.Txt_DataFineImpiego & "' "
            XML_Appezzamento.SetAttribute(LCase("FineImpiego"), enum_CodiciAnagrafe.DataFineImpiegoPNC)
            XML_Appezzamento.SetAttribute(LCase("FineImpiego_valore"), JsonRichiesta.filtrone_appezzamento.Txt_DataFineImpiego)

            classJoin.bAppezzamentoCodici = True
        End If


        '-- CODICE APPEZZAMENTO
        If Not IsNothing(JsonRichiesta.filtrone_appezzamento.Cmb_CodiceAppezzamento) AndAlso
            JsonRichiesta.filtrone_appezzamento.Cmb_CodiceAppezzamento <> "0" AndAlso JsonRichiesta.filtrone_appezzamento.Txt_CodiceAppezzamento <> "" Then
            strFiltro &= "AND Appezzamento_Codici.id_cod = " & JsonRichiesta.filtrone_appezzamento.Cmb_CodiceAppezzamento & " "
            strFiltro &= "AND Appezzamento_Codici.val_cod like '%" & JsonRichiesta.filtrone_appezzamento.Txt_CodiceAppezzamento & "%' "
            XML_Appezzamento.SetAttribute(LCase("codice"), JsonRichiesta.filtrone_appezzamento.Cmb_CodiceAppezzamento)
            XML_Appezzamento.SetAttribute(LCase("codice_valore"), JsonRichiesta.filtrone_appezzamento.Txt_CodiceAppezzamento)

            classJoin.bAppezzamentoCodici = True
        End If
#End Region


        '================================
        '       IMPIANTO
        '--------------------------------
#Region "IMPIANTO"
        '-- TERRENO NUDO
        If JsonRichiesta.filtrone_impianto.chkTerrNudo = True Then

            'imposto il cul cod a 0 
            strFiltro &= "AND Reg_Impianti.Cul_Cod = 0 "
            XML_Impianto.SetAttribute(LCase("cul_cod1"), "0")

            classJoin.bRegImpianti = True

        Else 'controllo terreno nudo -> prendo gli altri controlli

            'controllo finalità...
            If JsonRichiesta.filtrone_impianto.ddlFinProdComm <> "" Then
                strFiltro &= "AND Reg_Impianti.Grfi_Cod = " & JsonRichiesta.filtrone_impianto.ddlFinProdComm & " "
                XML_Impianto.SetAttribute(LCase("Grfi_Cod"), JsonRichiesta.filtrone_impianto.ddlFinProdComm)

                classJoin.bRegImpianti = True
            End If

            'regolamento...
            If JsonRichiesta.filtrone_impianto.ddlRegolamento <> "" Then
                strFiltro &= "AND Imprese_Progetti.Regolamento_Cod = " & JsonRichiesta.filtrone_impianto.ddlRegolamento & " "
                XML_Impianto.SetAttribute(LCase("Regolamento"), JsonRichiesta.filtrone_impianto.ddlRegolamento)

                classJoin.bImpreseProgetti = True
            End If

            'disciplinare...
            If JsonRichiesta.filtrone_impianto.ddlDisciplinare <> "" Then
                strFiltro &= "AND Imprese_Progetti.Disciplinare_Cod = " & JsonRichiesta.filtrone_impianto.ddlDisciplinare & " "
                XML_Impianto.SetAttribute(LCase("Finanziamento"), JsonRichiesta.filtrone_impianto.ddlDisciplinare)

                classJoin.bImpreseProgetti = True
            End If





            'stato impianto...
            If JsonRichiesta.filtrone_impianto.ddlStatoImpianto <> "" Then

                strFiltro &= "AND Imprese_Progetti.Stato_Impianto = " & JsonRichiesta.filtrone_impianto.ddlStatoImpianto & " "
                'strFiltro &= "AND Imprese_Progetti.Validita_Inizio <=" & ddlStatoImpianto.SelectedItem.Value & " "
                'strFiltro &= " AND Imprese_Progetti.Validita_Fine >= " & ddlStatoImpianto.SelectedItem.Value & " "
                XML_Impianto.SetAttribute(LCase("stato_impianto"), JsonRichiesta.filtrone_impianto.ddlStatoImpianto)

                classJoin.bImpreseProgetti = True
            End If


            'creo la stringa sul filtro colturale...
            Dim strFiltroColturale As String = ""

            'controllo selezione gruppi vegetali
            Dim Num_Gruppi As Integer = 1
            Dim strGruVeg As String = ""
            If JsonRichiesta.filtrone_impianto.cblGruVeg_1 = True Then
                strGruVeg &= "1,"
                XML_Impianto.SetAttribute(LCase("gru_cod") & Num_Gruppi, "1")
                Num_Gruppi += 1
            End If
            If JsonRichiesta.filtrone_impianto.cblGruVeg_2 = True Then
                strGruVeg &= "2,"
                XML_Impianto.SetAttribute(LCase("gru_cod") & Num_Gruppi, "2")
                Num_Gruppi += 1
            End If
            If JsonRichiesta.filtrone_impianto.cblGruVeg_3 = True Then
                strGruVeg &= "3,"
                XML_Impianto.SetAttribute(LCase("gru_cod") & Num_Gruppi, "3")
                Num_Gruppi += 1
            End If

            'se la stringa non è vuota l'aggiungo al filtro...
            If strGruVeg <> "" Then
                'tolgo l'ultima virgola.....
                strGruVeg = strGruVeg.Substring(0, strGruVeg.LastIndexOf(","))
                strFiltroColturale &= " SpecieVegetali.Gru_Cod IN (" & Agro_SQL_Save_Clausola_IN(strGruVeg) & ") AND "

                classJoin.bSpecieVegetali = True

            End If

            'ciclo sugli oggetti della lista
            Dim Num_Specie As Integer = 1
            Dim strSpecie As String = ""
            If Not IsNothing(JsonRichiesta.filtrone_impianto.cblSpecie) Then
                For i = 0 To JsonRichiesta.filtrone_impianto.cblSpecie.Count - 1
                    'If Me.cblSpecie.Items(i).Selected Then
                    strSpecie &= JsonRichiesta.filtrone_impianto.cblSpecie(i) & ","
                    XML_Impianto.SetAttribute(LCase("veg_cod") & Num_Specie, JsonRichiesta.filtrone_impianto.cblSpecie(i))
                    Num_Specie += 1
                    'End If
                Next
            End If
            'se la stringa non è vuota l'aggiungo al filtro....
            If strSpecie <> "" Then
                'tolgo l'ultima virgola.....
                strSpecie = strSpecie.Substring(0, strSpecie.LastIndexOf(","))
                strFiltroColturale &= " SpecieVegetali.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(strSpecie) & ") AND "

                classJoin.bSpecieVegetali = True
            End If


            'varietà...
            Dim Num_Cultivar As Integer = 1
            Dim strCultivar As String = ""
            If Not IsNothing(JsonRichiesta.filtrone_impianto.CBL_Cultivar) Then
                For i = 0 To JsonRichiesta.filtrone_impianto.CBL_Cultivar.Count - 1
                    strCultivar &= JsonRichiesta.filtrone_impianto.CBL_Cultivar(i) & ","
                    XML_Impianto.SetAttribute(LCase("cul_cod") & Num_Cultivar, JsonRichiesta.filtrone_impianto.CBL_Cultivar(i))
                    Num_Cultivar += 1
                Next
            End If

            If strCultivar <> "" Then
                strCultivar = strCultivar.Substring(0, strCultivar.LastIndexOf(","))
                strFiltroColturale &= " Reg_Impianti.cul_cod IN (" & Agro_SQL_Save_Clausola_IN(strCultivar) & ") AND "

                classJoin.bRegImpianti = True
            End If

            'tipologia varietale...
            Dim Num_TipologieVarietali As Integer = 1
            Dim strTipologieVarietali As String = ""
            If Not IsNothing(JsonRichiesta.filtrone_impianto.CBL_TipologiaVarietale) Then
                For i = 0 To JsonRichiesta.filtrone_impianto.CBL_TipologiaVarietale.Count - 1
                    strTipologieVarietali &= JsonRichiesta.filtrone_impianto.CBL_TipologiaVarietale(i) & ","
                    XML_Impianto.SetAttribute(LCase("GRVA_Cod_VEG") & Num_TipologieVarietali, JsonRichiesta.filtrone_impianto.CBL_TipologiaVarietale(i))
                    Num_TipologieVarietali += 1
                Next
            End If

            If strTipologieVarietali <> "" Then
                strTipologieVarietali = strTipologieVarietali.Substring(0, strTipologieVarietali.LastIndexOf(","))
                strFiltroColturale &= " Reg_Impianti.GRVA_Cod_VEG IN (" & Agro_SQL_Save_Clausola_IN(strTipologieVarietali) & ") AND "

                classJoin.bRegImpianti = True
            End If

            'tolgo l'ultimo and
            If strFiltroColturale <> "" Then
                strFiltroColturale = Left(strFiltroColturale, strFiltroColturale.Length - 4)
            End If


            'se si è scelto di includere i terreni nudi... 
            'aggiungo alla stringa sql 'AND ((Reg_Impianti.Cul_Cod = 0) OR (strFiltroColturale))
            'e alla stringa xml il nodo <terreninudi>
            Dim strTerrenoNudo As String = ""

            If JsonRichiesta.filtrone_impianto.chkIncludiTerrNudo = True Then
                If strFiltroColturale <> "" Then
                    strFiltroColturale = " ((Reg_Impianti.Cul_Cod = 0) OR (" & strFiltroColturale & ")) "
                    XML_Impianto.SetAttribute(LCase("terreninudi"), "includi terreni nudi")

                    classJoin.bRegImpianti = True
                Else
                    'strFiltroColturale = " ((Reg_Impianti.Cul_Cod = 0) OR (GruppoVegetale.Gru_Cod IN (1,2,3)) "
                    XML_Impianto.SetAttribute(LCase("terreninudi"), "includi terreni nudi")
                    'strFiltroColturale = " (Reg_Impianti.Cul_Cod = 0) "
                End If
            Else
                If strFiltroColturale = "" Then
                    strFiltroColturale = " ((Reg_Impianti.Cul_Cod <> 0) AND (GruppoVegetale.Gru_Cod IN (1,2,3))) "

                    classJoin.bRegImpianti = True
                    classJoin.bGruppoVegetale = True
                Else
                    strFiltroColturale = " ((Reg_Impianti.Cul_Cod <> 0) AND (" & strFiltroColturale & ")) "

                    classJoin.bRegImpianti = True
                End If
            End If

            If strFiltroColturale <> "" Then
                strFiltro &= " AND " & strFiltroColturale
            End If

        End If ' fine controllo terreno nudo 


        '-- COPERTURA
        Select Case JsonRichiesta.filtrone_impianto.CkbCopertura
            Case True 'Coltura Protetta
                'Escludo la Protezione "Nessuna"
                strFiltro &= "AND Reg_Impianti.Cop_Cod NOT IN (3, 4, 5, 6)"
                XML_Impianto.SetAttribute(LCase("coltura_protetta"), "1")

                classJoin.bRegImpianti = True

            Case False

                If JsonRichiesta.filtrone_impianto.ddlCopertura <> "" Then
                    'E' stata selezionata una copertura specifico

                    Select Case CInt(JsonRichiesta.filtrone_impianto.ddlCopertura)
                        Case 3, 4, 5, 6 'Nessuna Copertura
                            'Includo anche il Cop_Cod = 0 (utilizzato x indicare indefinito)
                            strFiltro &= "AND Reg_Impianti.Cop_Cod IN (0," & Agro_SQL_SaveNum(CInt(JsonRichiesta.filtrone_impianto.ddlCopertura)) & " )"
                            XML_Impianto.SetAttribute(LCase("cop_cod"), JsonRichiesta.filtrone_impianto.ddlCopertura)

                        Case Else 'Copertura Specifica

                            strFiltro &= "AND Reg_Impianti.Cop_Cod = " & CInt(JsonRichiesta.filtrone_impianto.ddlCopertura) & " "
                            XML_Impianto.SetAttribute(LCase("cop_cod"), JsonRichiesta.filtrone_impianto.ddlCopertura)

                    End Select

                    classJoin.bRegImpianti = True

                End If

        End Select


        '-- DATE DISTINTA
        Dim STR_APP_DISTINTA As String = ""
        JsonRichiesta.filtrone_impianto.txtDataIniDist = JsonRichiesta.filtrone_distinta.txtDataIniDist
        JsonRichiesta.filtrone_impianto.txtDataFinDist = JsonRichiesta.filtrone_distinta.txtDataFinDist
        JsonRichiesta.filtrone_impianto.RadiobuttonlistIniDist = JsonRichiesta.filtrone_distinta.RadiobuttonlistIniDist
        JsonRichiesta.filtrone_impianto.RadiobuttonlistFineDist = JsonRichiesta.filtrone_distinta.RadiobuttonlistFineDist
        If IsDate(JsonRichiesta.filtrone_distinta.txtDataIniDist) Then
            Dim strSegno As String = " >= "
            If JsonRichiesta.filtrone_distinta.RadiobuttonlistIniDist = "0" Then
                strSegno = " >= "
                'XML_Impianto.SetAttribute(LCase("validita_inizio_superiore_im"), txtDataIniDist.Text.Trim)
            ElseIf JsonRichiesta.filtrone_distinta.RadiobuttonlistIniDist = "1" Then
                strSegno = " <="
                'XML_Impianto.SetAttribute(LCase(" validita_inizio_inferiore_im"), txtDataIniDist.Text.Trim)
            End If
            strFiltro &= " AND imprese_progetti.Validita_Inizio" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_distinta.txtDataIniDist) & " ")
            STR_APP_DISTINTA &= " AND imprese_progetti.Validita_Inizio" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_distinta.txtDataIniDist) & " ")

            classJoin.bImpreseProgetti = True
        End If

        If IsDate(JsonRichiesta.filtrone_distinta.txtDataFinDist) Then
            Dim strSegno As String = " <="
            If JsonRichiesta.filtrone_distinta.RadiobuttonlistFineDist = "0" Then
                strSegno = ">= "
                'XML_Impianto.SetAttribute(LCase("validita_inizio_superiore_im"), txtDataFinDist.Text.Trim)
            ElseIf JsonRichiesta.filtrone_distinta.RadiobuttonlistFineDist = "1" Then
                strSegno = " <="
                'XML_Impianto.SetAttribute(LCase(" validita_inizio_inferiore_im"), txtDataFinDist.Text.Trim)
            End If
            strFiltro &= " AND imprese_progetti.Validita_Fine" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_distinta.txtDataFinDist) & " ")
            STR_APP_DISTINTA &= " AND imprese_progetti.Validita_Fine" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_distinta.txtDataFinDist) & " ")

            classJoin.bImpreseProgetti = True
        End If


        '-- LOTTO
        If JsonRichiesta.filtrone_distinta.txtLotto <> "" Then
            strFiltro &= " AND imprese_progetti.Progetto_Nome like '%" & Agro_SQL_SaveText(JsonRichiesta.filtrone_distinta.txtLotto) & "%' "
        End If


        '-- PROGETTO_DES
        If JsonRichiesta.filtrone_distinta.txtProgetto_Des <> "" Then
            strFiltro &= " AND imprese_progetti.Progetto_Des like '%" & Agro_SQL_SaveText(JsonRichiesta.filtrone_distinta.txtProgetto_Des) & "%' "
        End If


        ''impianti
        'If IsDate(JsonRichiesta.filtrone_impianto.txtDataIniDist) Then
        '    Dim strSegno As String = " >= "
        '    If JsonRichiesta.filtrone_impianto.RadiobuttonlistIniDist = "0" Then
        '        strSegno = " >= "
        '        'XML_Impianto.SetAttribute(LCase("validita_inizio_superiore_im"), txtDataIniDist.Text.Trim)
        '    ElseIf JsonRichiesta.filtrone_impianto.RadiobuttonlistIniDist = "1" Then
        '        strSegno = " <="
        '        'XML_Impianto.SetAttribute(LCase(" validita_inizio_inferiore_im"), txtDataIniDist.Text.Trim)
        '    End If
        '    strFiltro &= " AND imprese_progetti.Validita_Inizio" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_impianto.txtDataIniDist) & " ")
        '    STR_APP_DISTINTA &= " AND imprese_progetti.Validita_Inizio" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_impianto.txtDataIniDist) & " ")

        '    classJoin.bImpreseProgetti = True
        'End If


        'If IsDate(JsonRichiesta.filtrone_impianto.txtDataFinDist) Then
        '    Dim strSegno As String = " <="
        '    If JsonRichiesta.filtrone_impianto.RadiobuttonlistFineDist = "0" Then
        '        strSegno = ">= "
        '        'XML_Impianto.SetAttribute(LCase("validita_inizio_superiore_im"), txtDataFinDist.Text.Trim)
        '    ElseIf JsonRichiesta.filtrone_impianto.RadiobuttonlistFineDist = "1" Then
        '        strSegno = " <="
        '        'XML_Impianto.SetAttribute(LCase(" validita_inizio_inferiore_im"), txtDataFinDist.Text.Trim)
        '    End If
        '    strFiltro &= " AND imprese_progetti.Validita_Fine" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_impianto.txtDataFinDist) & " ")
        '    STR_APP_DISTINTA &= " AND imprese_progetti.Validita_Fine" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_impianto.txtDataFinDist) & " ")

        '    classJoin.bImpreseProgetti = True
        'End If


        '-- CAPITOLATO
        Dim lk As Integer
        Dim str_app_capitolati As String = ""
        Dim str_app_capitolato_op As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_impianto.chk_Capitolato) Then
            For lk = 0 To JsonRichiesta.filtrone_impianto.chk_Capitolato.Count - 1
                'If JsonRichiesta.filtrone_impianto.chk_Capitolato.Items(lk).Selected = True Then
                If str_app_capitolati = "" Then
                    str_app_capitolati &= " AND EXISTS( SELECT * FROM Reg_Impianti_Codici r1 where r1.id_cod = " & enum_CodiciAnagrafe.Capitolato_Privato & " "
                    str_app_capitolati &= " AND (Reg_Impianti.PIVA = r1.piva and Reg_Impianti.SA_COD = r1.sa_cod and Reg_Impianti.APPEZZA = r1.appezza and Reg_Impianti.ID_REG = r1.Id_Reg  ) "
                    str_app_capitolati &= " AND ( (r1.val_cod like '%" & JsonRichiesta.filtrone_impianto.chk_Capitolato(lk) & "%' ) "
                    str_app_capitolato_op = JsonRichiesta.filtrone_impianto.chk_Capitolato(lk)

                    classJoin.bRegImpiantiCodici = True
                Else
                    str_app_capitolati &= "OR "
                    str_app_capitolati &= "( r1.val_cod like '%" & JsonRichiesta.filtrone_impianto.chk_Capitolato(lk) & "%' ) "
                    str_app_capitolato_op = str_app_capitolato_op & "|" & JsonRichiesta.filtrone_impianto.chk_Capitolato(lk)
                End If
                'End If
            Next
        End If
        If classJoin.bRegImpiantiCodici Then
            'aggiungo clausola su distinta
            str_app_capitolati &= ") "

            If STR_APP_DISTINTA <> "" Then
                str_app_capitolati &= " and r1.Progetto_Cod in ( " &
                                " select Progetto_Cod from Imprese_Progetti where 1=1 " & STR_APP_DISTINTA & " ) "
            End If



            str_app_capitolati &= ") "

            XML_Impianto.SetAttribute(LCase("codice"), enum_CodiciAnagrafe.Capitolato_Privato)
            XML_Impianto.SetAttribute(LCase("codice_valore"), str_app_capitolato_op)

            strFiltro &= str_app_capitolati
        End If


        '-- DATE IMPIANTO
        If IsDate(JsonRichiesta.filtrone_impianto.txtDataIniImp) Then
            Dim strSegno As String = ""
            If JsonRichiesta.filtrone_impianto.rblInizioImp = "0" Then
                strSegno = " >= "
                'XML_Impianto.SetAttribute(LCase("validita_inizio_superiore_im"), txtDataIniImp.Text.Trim)
            ElseIf JsonRichiesta.filtrone_impianto.rblInizioImp = "1" Then
                strSegno = " <= "
                'XML_Impianto.SetAttribute(LCase("validita_inizio_inferiore_im"), txtDataIniImp.Text.Trim)
            End If
            strFiltro &= "AND Reg_Impianti.Validita_Inizio" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_impianto.txtDataIniImp) & " ")

            classJoin.bRegImpianti = True
        End If
        If IsDate(JsonRichiesta.filtrone_impianto.txtDataFinImp) Then
            Dim strSegno As String = ""
            If JsonRichiesta.filtrone_impianto.rblFineImp = "0" Then
                strSegno = " >= "
                XML_Impianto.SetAttribute(LCase("validita_fine_superiore_im"), JsonRichiesta.filtrone_impianto.txtDataFinImp.Trim)
            ElseIf JsonRichiesta.filtrone_impianto.rblFineImp = "1" Then
                strSegno = " <= "
                XML_Impianto.SetAttribute(LCase("validita_fine_inferiore_im"), JsonRichiesta.filtrone_impianto.txtDataFinImp.Trim)
            End If
            strFiltro &= "AND Reg_Impianti.Validita_Fine" & strSegno & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_impianto.txtDataFinImp) & " ")

            classJoin.bRegImpianti = True
        End If

        '-- DATA CREAZIONE IMPIANTO
        If IsDate(JsonRichiesta.filtrone_impianto.txtDataCreazioneImpDAL) Then
            Dim strSegno As String = ""
            If JsonRichiesta.filtrone_impianto.rblDataCreazioneImpDAL = "0" Then
                strSegno = " >= "
            ElseIf JsonRichiesta.filtrone_impianto.rblDataCreazioneImpDAL = "1" Then
                strSegno = " <= "
            End If
            strFiltro &= "AND CONVERT(Date,Reg_Impianti.Data_Creazione, 120)" & strSegno & "CONVERT(DATE," & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_impianto.txtDataCreazioneImpDAL) & " ") & " , 120) "

            classJoin.bRegImpianti = True
        End If
        If IsDate(JsonRichiesta.filtrone_impianto.txtDataCreazioneImpAL) Then
            Dim strSegno As String = ""
            If JsonRichiesta.filtrone_impianto.rblDataCreazioneImpAL = "0" Then
                strSegno = " >= "
            ElseIf JsonRichiesta.filtrone_impianto.rblDataCreazioneImpAL = "1" Then
                strSegno = " <= "
            End If
            strFiltro &= "AND CONVERT(DATE,Reg_Impianti.Data_Creazione, 120)" & strSegno & "CONVERT(DATE," & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_impianto.txtDataCreazioneImpAL) & " ") & " , 120) "

            classJoin.bRegImpianti = True
        End If

        ''Metodo Produzione
        'Select Case Me.OptionList_MetodoProduzioneImpianto.SelectedValue
        '    Case 1, 2, 3
        '        strFiltro &= "AND Reg_Impianti_Codici.id_cod = " &  enum_CodiciAnagrafe.MetodoDiProduzione & " "
        '        strFiltro &= "AND Reg_Impianti_Codici.val_cod = '" & OptionList_MetodoProduzioneImpianto.SelectedValue & "' "
        '        XML_Impianto.SetAttribute(LCase("MetodoProduzione"),  enum_CodiciAnagrafe.MetodoDiProduzione)
        '        XML_Impianto.SetAttribute(LCase("MetodoProduzione_valore"), OptionList_MetodoProduzioneImpianto.SelectedValue)
        'End Select

        '-- CODICI
        Dim Array_Codici As String()
        Array_Codici = Split((JsonRichiesta.filtrone_impianto.Txt_CodiceImpianto), ";")
        Dim n As Integer
        If JsonRichiesta.filtrone_impianto.Cmb_CodiceImpianto <> "0" AndAlso JsonRichiesta.filtrone_impianto.Txt_CodiceImpianto.Trim <> "" Then

            strFiltro &= " AND EXISTS ("
            strFiltro &= " select * from Reg_Impianti_Codici r2 where r2.id_cod = " & JsonRichiesta.filtrone_impianto.Cmb_CodiceImpianto & " "
            strFiltro &= "  AND Reg_Impianti.PIVA = r2.piva and Reg_Impianti.SA_COD=r2.sa_cod and Reg_Impianti.APPEZZA = r2.appezza and Reg_Impianti.ID_REG = r2.Id_Reg "

            If Array_Codici.Length = 1 Then
                strFiltro &= " AND (r2.val_cod like '%" & JsonRichiesta.filtrone_impianto.Txt_CodiceImpianto & "%'  "
                'strFiltro &= " AND Reg_Impianti_Codici.val_cod like '%" & Me.Txt_CodiceImpianto.Text & "%'  "
            Else
                strFiltro &= "AND ("
                For n = 0 To UBound(Array_Codici)
                    strFiltro &= " r2.val_cod like '%" & Array_Codici(n) & "%' OR"
                    'strFiltro &= " Reg_Impianti_Codici.val_cod like '%" & Array_Codici(n) & "%' OR"
                Next
                strFiltro = Left(strFiltro, strFiltro.Length - 2)
                strFiltro &= ")"
            End If
            strFiltro &= ")"
            strFiltro &= ")"
            XML_Impianto.SetAttribute(LCase("codice"), JsonRichiesta.filtrone_impianto.Cmb_CodiceImpianto)
            XML_Impianto.SetAttribute(LCase("codice_valore"), JsonRichiesta.filtrone_impianto.Txt_CodiceImpianto.Trim)

            classJoin.bRegImpiantiCodici = True
        End If

        '-- DETTAGLIO VARIETA PERSONALIZZATO
        If Not IsNothing(JsonRichiesta.filtrone_impianto.Cmb_DettaglioVarietaPersonalizzato) AndAlso JsonRichiesta.filtrone_impianto.Cmb_DettaglioVarietaPersonalizzato <> "0" Then
            strFiltro &= "AND Reg_Impianti_Codici.id_cod = " & enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato & " "
            strFiltro &= "AND Reg_Impianti_Codici.val_cod like '%" & JsonRichiesta.filtrone_impianto.Cmb_DettaglioVarietaPersonalizzato & "%' "
            XML_Impianto.SetAttribute(LCase("codice"), enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato)
            XML_Impianto.SetAttribute(LCase("codice_valore"), JsonRichiesta.filtrone_impianto.Cmb_DettaglioVarietaPersonalizzato)

            classJoin.bRegImpiantiCodici = True
        End If
#End Region


        '================================
        '       MOVIMENTO
        '--------------------------------
#Region "MOVIMENTO"
        'controllo date inizio....
        If IsDate(JsonRichiesta.filtrone_movimento.txtDataMovimentoDa) Then
            strFiltro &= "AND Movimenti.Data_Movimento >=" & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_movimento.txtDataMovimentoDa) & " ")
            XML_Agenda.SetAttribute(LCase("data_movimento_inizio"), JsonRichiesta.filtrone_movimento.txtDataMovimentoDa)

            classJoin.bMovimenti = True
        End If

        'fine...
        If IsDate(JsonRichiesta.filtrone_movimento.txtDataMovimentoA) Then
            strFiltro &= "AND Movimenti.Data_Movimento <=" & Agro_SQL_SaveDate(CDate(JsonRichiesta.filtrone_movimento.txtDataMovimentoA) & " ")
            XML_Agenda.SetAttribute(LCase("data_movimento_fine"), JsonRichiesta.filtrone_movimento.txtDataMovimentoA.Trim)

            classJoin.bMovimenti = True
        End If

        'gruppi operazioni
        Dim Num_GruOpe As Integer = 1
        Dim strGruOpe As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_movimento.CBL_GruppoOperazioni) Then
            For i = 0 To JsonRichiesta.filtrone_movimento.CBL_GruppoOperazioni.Count - 1
                strGruOpe = strGruOpe & JsonRichiesta.filtrone_movimento.CBL_GruppoOperazioni(i) & ","
                XML_Agenda.SetAttribute(LCase("gru_op") & Num_GruOpe, JsonRichiesta.filtrone_movimento.CBL_GruppoOperazioni(i))
                Num_GruOpe += 1
            Next
        End If

        'se la stringa non è vuota l'aggiungo al filtro....
        If strGruOpe <> "" Then
            'tolgo l'ultima virgola.....
            strGruOpe = strGruOpe.Substring(0, strGruOpe.LastIndexOf(","))
            strFiltro &= "AND Operazioni.gru_op IN (" & Agro_SQL_Save_Clausola_IN(strGruOpe) & ") "

            classJoin.bOperazioni = True
        End If

        'If (Me.ddlGruOpeCol.Items.Count > 0) And (ddlGruOpeCol.SelectedIndex <> 0) And (ddlOperaCol.SelectedIndex = 0) Then
        '    AgroMsgBox("Selezionare l'Operazione Colturale!!!", Page)
        '    Exit Function
        'End If


        'operazioni
        Dim Num_Ope As Integer = 1
        Dim strOpe As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_movimento.CBL_Operazioni) Then
            For i = 0 To JsonRichiesta.filtrone_movimento.CBL_Operazioni.Count - 1
                If JsonRichiesta.filtrone_movimento.CBL_Operazioni(i) Then
                    strOpe = strOpe & JsonRichiesta.filtrone_movimento.CBL_Operazioni(i) & ","
                    XML_Agenda.SetAttribute(LCase("lav_cod") & Num_Ope, JsonRichiesta.filtrone_movimento.CBL_Operazioni(i))
                    Num_Ope += 1
                End If
            Next
        End If
        'se la stringa non è vuota l'aggiungo al filtro....
        If strOpe <> "" Then
            'tolgo l'ultima virgola.....
            strOpe = strOpe.Substring(0, strOpe.LastIndexOf(","))
            strFiltro &= "AND Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(strOpe) & ") "

            classJoin.bAgenda = True
        End If


        'prodotti.... per ora solo gias
        'If (Me.ddlCatProd.Items.Count > 0) And (ddlCatProd.SelectedIndex <> 0) Then
        '    AgroMsgBox("Selezionare il Prodotto!!!", Page)
        '    Exit Function

        'End If

        If JsonRichiesta.filtrone_movimento.ddlCatProd <> "" Then
            strFiltro &= " AND Movimenti_dettagli.Elem_Cod = " & JsonRichiesta.filtrone_movimento.ddlCatProd
            XML_Agenda.SetAttribute(LCase("elem_cod"), JsonRichiesta.filtrone_movimento.ddlCatProd)

            classJoin.bMovimentiDettagli = True
        End If

        'ciclo su gli oggetti della lista prodotti
        Dim Num_Prodotti As Integer = 1
        Dim strProdotti As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_movimento.CBL_Prodotto) Then

            Dim prodotto As Integer = CInt(JsonRichiesta.filtrone_movimento.CBL_Prodotto)

            strProdotti = strProdotti & prodotto & ","
            XML_Agenda.SetAttribute(LCase("pro_cod") & Num_Prodotti, prodotto)

            'For i = 0 To JsonRichiesta.filtrone_movimento.CBL_Prodotto.Count - 1
            'strProdotti = strProdotti & JsonRichiesta.filtrone_movimento.CBL_Prodotto(i) & ","
            'XML_Agenda.SetAttribute(LCase("pro_cod") & Num_Prodotti, JsonRichiesta.filtrone_movimento.CBL_Prodotto(i))
            'Num_Prodotti += 1
            'Next


        End If

        'se la stringa non è vuota l'aggiungo al filtro....
        If strProdotti <> "" Then
            'tolgo l'ultima virgola.....
            strProdotti = strProdotti.Substring(0, strProdotti.LastIndexOf(","))
            strFiltro &= " AND Movimenti_dettagli.Pro_Cod IN (" & Agro_SQL_Save_Clausola_IN(strProdotti) & ") "

            classJoin.bMovimentiDettagli = True
        End If


        'avversità.....
        Dim Num_Avversita As Integer = 1
        Dim strAvversita As String = ""
        If Not IsNothing(JsonRichiesta.filtrone_movimento.CBL_Avversita) Then
            For i = 0 To JsonRichiesta.filtrone_movimento.CBL_Avversita.Count - 1
                'If Me.CBL_Avversita.Items(i).Selected Then
                If JsonRichiesta.filtrone_movimento.rblAvversita = 0 Then
                    strAvversita = strAvversita & JsonRichiesta.filtrone_movimento.CBL_Avversita(i) & ","
                    XML_Agenda.SetAttribute(LCase("Av_Cod") & Num_Avversita, JsonRichiesta.filtrone_movimento.CBL_Avversita(i))
                Else
                    strAvversita = strAvversita & JsonRichiesta.filtrone_movimento.CBL_Avversita(i) & ","
                    XML_Agenda.SetAttribute(LCase("av_gru") & Num_Avversita, JsonRichiesta.filtrone_movimento.CBL_Avversita(i))
                End If
                'Else
                'strAvversita = strAvversita & JsonRichiesta.filtrone_movimento.CBL_Avversita(i) & ","
                'XML_Agenda.SetAttribute(LCase("Av_Cod") & Num_Avversita, JsonRichiesta.filtrone_movimentoCBL_Avversita(i))
                'End If
                Num_Avversita += 1
                'End If
            Next
        End If
        'se la stringa non è vuota l'aggiungo al filtro....
        If strAvversita <> "" Then
            'tolgo l'ultima virgola.....
            strAvversita = strAvversita.Substring(0, strAvversita.LastIndexOf(","))

            If JsonRichiesta.filtrone_movimento.rblAvversita = 0 Then 'infestanti
                strFiltro &= " AND Mov_Dettaglio_Tecnico.Av_Cod IN (" & Agro_SQL_Save_Clausola_IN(strAvversita) & ") "
            Else 'gruppi infestanti
                strFiltro &= " AND Mov_Dettaglio_Tecnico.Av_Gru IN (" & Agro_SQL_Save_Clausola_IN(strAvversita) & ") "
            End If

            classJoin.bMovDettaglioTecnico = True

        End If

        'utente....
        Dim strUtentiMovimenti As String = ""
        Dim Num_UtentiMovimenti As Integer = 1
        If Not IsNothing(JsonRichiesta.filtrone_movimento.CBL_UtentiMovimenti) Then
            For i = 0 To JsonRichiesta.filtrone_movimento.CBL_UtentiMovimenti.Count - 1
                strUtentiMovimenti = strUtentiMovimenti & " '" & JsonRichiesta.filtrone_movimento.CBL_UtentiMovimenti(i) & "',"
                XML_Agenda.SetAttribute(LCase("utente") & Num_UtentiMovimenti, JsonRichiesta.filtrone_movimento.CBL_UtentiMovimenti(i))
                Num_UtentiMovimenti += 1
            Next
        End If
        'se la stringa non è vuota l'aggiungo al filtro....
        If strUtentiMovimenti <> "" Then
            strUtentiMovimenti = strUtentiMovimenti.Substring(0, strUtentiMovimenti.LastIndexOf(","))
            strFiltro &= "AND (Agenda.Username_Creazione IN (" & Agro_SQL_Save_Clausola_IN(strUtentiMovimenti, True) & ") OR Agenda.Username_Modifica IN (" & Agro_SQL_Save_Clausola_IN(strUtentiMovimenti, True) & "))"

            classJoin.bAgenda = True
        End If
#End Region

        strFiltro_SenzaPadri = strFiltro

        '------------------------------------------------------------------------------------------------------------

        'ciclo su gli oggetti della lista cooperativa
        Dim strCoop As String = ""
        Dim PivaPadre As String = ""


        '        JsonRichiesta.filtrone_azienda.albero.Length

        'TO DO 
        'utente
        For i = 0 To JsonRichiesta.filtrone_azienda.albero.Length - 1
            'For i = 0 To Me.TreeViewImprese.Nodes(0).Nodes.Count - 1
            If JsonRichiesta.filtrone_azienda.albero(i) = "" Then
                Continue For
            End If
            Albero.ChiaveAlbero_Decodifica_ImpiantiVegetali_x_json(JsonRichiesta.filtrone_azienda.albero(i),
                                                                   enum_TipoNodo.Impresa, PivaPadre,
                                                                   Nothing, Nothing, Nothing,
                                                                   Nothing, Nothing, Nothing)

            XML_GerarchiaImprese = XmlDoc.CreateElement("GerarchiaImprese")
            XML_DatiGerarchiaImprese.AppendChild(XML_GerarchiaImprese)

            XML_GerarchiaImprese.SetAttribute(LCase("padre"), PivaPadre)
            strCoop &= " ((GerarchiaImprese.Padre = '" & PivaPadre & "' AND GerarchiaImprese.Foglia=1 ) OR Imprese.piva = '" & PivaPadre & "') OR "

        Next

        'se la stringa non è vuota l'aggiungo al filtro....
        If strCoop <> "" Then
            'tolgo Or
            strCoop = Left(strCoop, strCoop.Length - 3)
            'strCoop &= ")"
            strFiltro &= "AND (" & strCoop & ") "
        End If

        classJoin.bGerarchiaImprese = True

        '------------------------------------------------------------------------------------------------------------

        Select Case Qs_Funzione

            Case enum_TipoFiltrone.Utenti

                If Not String.IsNullOrEmpty(Sql_Permessi_Amministratore) Then

                    ImpostaVariabiliJOIN_xFiltroUtente(Sql_Permessi_Amministratore, classJoin)

                    strFiltro = strFiltro & " AND " & Sql_Permessi_Amministratore

                End If

                'tolgo l'and x il filtrone galattico
                If strFiltro <> "" Then
                    strFiltro = Right(strFiltro, strFiltro.Length - 4)
                End If

            Case Else

                'in modalità stampa se non si è selezionato nulla..
                'faccio vedere ciò che l'utente può vedere poichè settato nella tabella Utenti_Profili
                If strFiltro = "" Then
                    'If strFiltro_SenzaPadri = "" Then
                    If Param_Sql_Permessi <> "" Then
                        strFiltro = Param_Sql_Permessi
                        Xml_Permessi = Param_Xml_Permessi
                    Else
                        If strFiltro <> "" Then
                            'tolgo l'and x il filtrone galattico
                            strFiltro = Right(strFiltro, strFiltro.Length - 4)
                        End If
                    End If
                Else
                    'altrimenti accodo a ciò che ha scelto nel filtro 
                    'la stringa della tabella Utenti_Profili in cui gli sono assegnati i permessi
                    'cosicchè possa vedere cmq solo i dati su cui ha il permesso
                    If Param_Sql_Permessi <> "" Then
                        If InStr(LCase(Mid(Param_Sql_Permessi.Trim, 1, 3)), "and") = 0 Then
                            strFiltro = strFiltro & " AND (" & Param_Sql_Permessi & ")"
                        Else
                            strFiltro = strFiltro & Param_Sql_Permessi
                        End If
                    End If
                    'tolgo l'and x il filtrone galattico
                    strFiltro = Right(strFiltro, strFiltro.Length - 4)
                End If

        End Select

        'HttpContext.Current.Session("strFiltro") = strFiltro
        OUTPUT_strFiltro = strFiltro.ToOrigin(objParametri_Server)


        'vanni, fine preparazione filtro


        '-----------------------------------------------------------------------
        'Carico il Datagrid associato al pannello richiesto 
        'e svuoto gli altri x non sovraccaricare la pagina
        '
        'N.B. Nel caso della gestione utenti carico sempre il datagrid Aziende
        '-----------------------------------------------------------------------

        Dim dt As DataTable = Nothing

        Select Case JsonRichiesta.tab_selezionata
            Case "azienda"
                '_____________________________________________________DATAGRID TAB STRUTTURA______________________________________________________________________________________

                Select Case Qs_Funzione

                    Case enum_TipoFiltrone.Stampa,
                         enum_TipoFiltrone.Agenda,
                         enum_TipoFiltrone.MultiModificaAgenda,
                         enum_TipoFiltrone.AnalisiCosti,
                         enum_TipoFiltrone.ModificaImpianti,
                         enum_TipoFiltrone.MultiModificaImpianti,
                         enum_TipoFiltrone.CancellaAppezzamenti,
                         enum_TipoFiltrone.PianificazioneInterventi,
                         enum_TipoFiltrone.EliminaInterventi,
                         enum_TipoFiltrone.Blocco_OperazioniAgenda,
                         enum_TipoFiltrone.Esportazione_OP_Inv,
                         enum_TipoFiltrone.Esportazione_OP_Gest,
                         enum_TipoFiltrone.Esportazione_OP_Gest_Coop,
                         enum_TipoFiltrone.Esportatore_Universale_Impianti,
                         enum_TipoFiltrone.Esportatore_Contatti,
                         enum_TipoFiltrone.Esportatore_Universale_Imprese,
                         enum_TipoFiltrone.Esportatore_Universale_Centri,
                         enum_TipoFiltrone.Esportatore_Universale_Appezza,
                         enum_TipoFiltrone.Esportatore_Universale_Agenda,
                         enum_TipoFiltrone.Richiesta_Verifica_Conformita,
                         enum_TipoFiltrone.Esportatore_Universale_Rintraccio,
                         enum_TipoFiltrone.Esportazione_CellulariTecnici,
                         enum_TipoFiltrone.Bilancio_Fertilizzazioni,
                         enum_TipoFiltrone.PianiCampionamento_AggiungiImpianti,
                         enum_TipoFiltrone.Associa_Ricetta_Impianti,
                         enum_TipoFiltrone.Associa_Ricetta_Interventi,
                         enum_TipoFiltrone.PianoConcimazione

                        If DgrVuoti = "false" Then
                            dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.Imprese, "", classJoin, objParametri_Utenti)
                            'CaricaDgrAziende(strFiltro, classJoin)
                            str_RISPOSTA = DT_to_Json_Azienda(dt, JsonRichiesta, objParametri_Server)
                        End If
                        'HttpContext.Current.Session("dt_x_export_azienda") = dt 'vanni
                        OUTPUT_dt_x_export_azienda = dt

                    Case enum_TipoFiltrone.Utenti,
                                    enum_TipoFiltrone.Gestione_Servizi,
                                    enum_TipoFiltrone.ExportSigpa

                        'nel caso di gestione utenti carico sempre questo datagrid
                        dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.Imprese, "", classJoin, objParametri_Utenti)
                        str_RISPOSTA = DT_to_Json_Azienda(dt, JsonRichiesta, objParametri_Server)
                        'Me.CaricaDgrAziende(strFiltro, classJoin)
                        'HttpContext.Current.Session("dt_x_export_azienda") = dt 'vanni
                        OUTPUT_dt_x_export_azienda = dt

                End Select

            Case "centro"

                If (Qs_Funzione = enum_TipoFiltrone.Stampa OrElse
                    Qs_Funzione = enum_TipoFiltrone.Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.AnalisiCosti OrElse
                    Qs_Funzione = enum_TipoFiltrone.ModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.CancellaAppezzamenti OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianificazioneInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.EliminaInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.Blocco_OperazioniAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Inv OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest_Coop OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Imprese OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Contatti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Centri OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Appezza OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Richiesta_Verifica_Conformita OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Rintraccio OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_CellulariTecnici OrElse
                    Qs_Funzione = enum_TipoFiltrone.Bilancio_Fertilizzazioni OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianiCampionamento_AggiungiImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Interventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianoConcimazione) AndAlso
                    DgrVuoti = "false" Then

                    '_____________________________________________________DATAGRID TAB STRUTTURA______________________________________________________________________________________
                    'Me.CaricaDgrStruttura(strFiltro, classJoin)
                    Dim strOrdinamento As String = "ORDER BY Imprese.rag_soc, Centri_Aziendali.Sa_Nome "
                    dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.CentriAziendali, strOrdinamento, classJoin, objParametri_Utenti)
                    'CaricaDgrAziende(strFiltro, classJoin)
                    str_RISPOSTA = DT_to_Json_Centro(dt, JsonRichiesta, objParametri_Server)
                    'HttpContext.Current.Session("dt_x_export_centro") = dt ' vanni
                    OUTPUT_dt_x_export_centro = dt
                End If

            Case "campo"

                If (Qs_Funzione = enum_TipoFiltrone.Stampa OrElse
                    Qs_Funzione = enum_TipoFiltrone.Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.AnalisiCosti OrElse
                    Qs_Funzione = enum_TipoFiltrone.ModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.CancellaAppezzamenti OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianificazioneInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.EliminaInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.Blocco_OperazioniAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Inv OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest_Coop OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Imprese OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Contatti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Centri OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Appezza OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Richiesta_Verifica_Conformita OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Rintraccio OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_CellulariTecnici OrElse
                    Qs_Funzione = enum_TipoFiltrone.Bilancio_Fertilizzazioni OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianiCampionamento_AggiungiImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Interventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianoConcimazione) AndAlso
                    DgrVuoti = "false" Then

                    '_____________________________________________________DATAGRID TAB STRUTTURA______________________________________________________________________________________
                    Dim strOrdinamento As String = "ORDER BY Imprese.rag_soc, Centri_Aziendali.Sa_Nome, Campi.Campo_Des "
                    dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.Campi, strOrdinamento, classJoin, objParametri_Utenti)
                    dt = CampoPredisponiDatiMancanti(dt, JsonRichiesta, objParametri_Server, objParametri_Utenti, ASG_SuperUser_CodFiscale, ASG_Utente_Username_Crypt, ASG_Utente_Password_Crypt)
                    str_RISPOSTA = DT_to_Json_Campo(dt, JsonRichiesta, objParametri_Server)
                    OUTPUT_dt_x_export_campo = dt
                End If

            Case "appezzamento"

                If (Qs_Funzione = enum_TipoFiltrone.Stampa OrElse
                    Qs_Funzione = enum_TipoFiltrone.Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.AnalisiCosti OrElse
                    Qs_Funzione = enum_TipoFiltrone.ModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.CancellaAppezzamenti OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianificazioneInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.EliminaInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.Blocco_OperazioniAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Inv OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest_Coop OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Imprese OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Contatti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Centri OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Appezza OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Richiesta_Verifica_Conformita OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Rintraccio OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_CellulariTecnici OrElse
                    Qs_Funzione = enum_TipoFiltrone.Bilancio_Fertilizzazioni OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianiCampionamento_AggiungiImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Interventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianoConcimazione) AndAlso
                    DgrVuoti = "false" Then

                    '______________________________________________________DATAGRID TAB APPEZZAMENTO_________________________________________________________________________________________________
                    Dim strOrdinamento As String = "ORDER BY Imprese.rag_soc, Centri_Aziendali.Sa_Nome, Appezzamento.Appezza "
                    dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.Appezzamenti, strOrdinamento, classJoin, objParametri_Utenti)
                    str_RISPOSTA = DT_to_Json_Appezzamento(dt, objParametri_Server)
                    'Me.CaricaDgrAppezza(strFiltro, classJoin)
                    'HttpContext.Current.Session("dt_x_export_appezza") = dt 'vanni
                    OUTPUT_dt_x_export_appezza = dt

                End If

            Case "appezzamento_riparto_catasto"

                If Qs_Funzione = enum_TipoFiltrone.Agenda AndAlso
                    DgrVuoti = "false" Then


                    Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                    Dim UtenteAbilitatoCatasto As Boolean
                    UtenteAbilitatoCatasto = objPermessi.Controlla_Permessi_Utente(
                                            objParametri_Server.UtenteUsername,
                                            Web.HttpContext.Current.Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Gest_Stampe,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

                    If Not UtenteAbilitatoCatasto Then
                        Throw New Exception("Non abilitato")
                    End If

                    '______________________________________________________DATAGRID TAB APPEZZAMENTO RIPARTO CATASTO _________________________________________________________________________________________________
                    Dim strOrdinamento As String = "ORDER BY Imprese.rag_soc, Centri_Aziendali.Sa_Nome, Appezzamento.Appezza "
                    dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.AppezzamentiRipartoCatasto, strOrdinamento, classJoin, objParametri_Utenti)

                    'utilizzo il datatable come filtro per poi impostare la query di lettura del riparto.

                    Dim dtRiparto As DataTable =
                        DT_to_AppezzamentoRipartoCatasto("", mostraRipartoAppezzamenti:=True, mostraAppezzamentoSenzaRiparto:=False, mostraParticelleSenzaRiparto:=False, DataDa:=JsonRichiesta.filtrone_distinta.txtDataIniDist, DataA:=JsonRichiesta.filtrone_distinta.txtDataFinDist, IDTestataTemp:=0, dtFiltroAppezza:=dt, objParametri_Server:=objParametri_Server, objParametri_utenti:=objParametri_Utenti)

                    str_RISPOSTA = DT_to_Json_AppezzamentoRipartoCatasto(dtRiparto)

                End If


            Case "impianto"

                If (Qs_Funzione = enum_TipoFiltrone.Stampa OrElse
                    Qs_Funzione = enum_TipoFiltrone.Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.AnalisiCosti OrElse
                    Qs_Funzione = enum_TipoFiltrone.ModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.CancellaAppezzamenti OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianificazioneInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.EliminaInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.Blocco_OperazioniAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Inv OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest_Coop OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Imprese OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Contatti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Centri OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Appezza OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Richiesta_Verifica_Conformita OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Rintraccio OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_CellulariTecnici OrElse
                    Qs_Funzione = enum_TipoFiltrone.Bilancio_Fertilizzazioni OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianiCampionamento_AggiungiImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Interventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianoConcimazione) AndAlso
                    DgrVuoti = "false" Then
                    '____________________________________________________DATAGRID TAB IMPIANTO________________________________________
                    'Me.CaricaDgrImpianti(strFiltro, classJoin)
                    Dim strOrdinamento As String
                    If classJoin.bImpreseProgetti Then
                        strOrdinamento = "ORDER BY Imprese.piva, Centri_Aziendali.sa_cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg DESC, Imprese_Progetti.Validita_Inizio DESC "

                    Else
                        strOrdinamento = "ORDER BY Imprese.piva, Centri_Aziendali.sa_cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg DESC "
                    End If
                    'Dim strOrdinamento As String = "ORDER BY Imprese.rag_soc, Centri_Aziendali.Sa_Nome, Appezzamento.Appezza, Reg_Impianti.Id_Reg, Imprese_Progetti.Validita_Inizio DESC "
                    dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.Impianti, strOrdinamento, classJoin, objParametri_Utenti)



                    str_RISPOSTA = DT_to_Json_Impianto(dt, JsonRichiesta, objParametri_Server, objParametri_Utenti, ASG_SuperUser_CodFiscale, ASG_Utente_Username_Crypt, ASG_Utente_Password_Crypt)
                    'HttpContext.Current.Session("dt_x_export_impianto") = dt 'vanni
                    OUTPUT_dt_x_export_impianto = dt
                End If

            Case "esercizio"
                'Me.CaricaDgrImpianti(strFiltro, classJoin)
                Dim strOrdinamento As String
                If classJoin.bImpreseProgetti Then
                    strOrdinamento = "ORDER BY Imprese.piva, Centri_Aziendali.sa_cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg DESC, Imprese_Progetti.Validita_Inizio DESC "

                Else
                    strOrdinamento = "ORDER BY Imprese.piva, Centri_Aziendali.sa_cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg DESC "
                End If
                'Dim strOrdinamento As String = "ORDER BY Imprese.rag_soc, Centri_Aziendali.Sa_Nome, Appezzamento.Appezza, Reg_Impianti.Id_Reg, Imprese_Progetti.Validita_Inizio DESC "
                dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.Esercizi, strOrdinamento, classJoin, objParametri_Utenti)
                str_RISPOSTA = DT_to_Json_Distinta(dt, JsonRichiesta, objParametri_Server, objParametri_Utenti, ASG_SuperUser_CodFiscale, ASG_Utente_Username_Crypt, ASG_Utente_Password_Crypt)
                'HttpContext.Current.Session("dt_x_export_impianto") = dt 'vanni
                OUTPUT_dt_x_export_distinta = dt

            Case "movimento"

                If (Qs_Funzione = enum_TipoFiltrone.Stampa OrElse
                    Qs_Funzione = enum_TipoFiltrone.Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.AnalisiCosti OrElse
                    Qs_Funzione = enum_TipoFiltrone.ModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.MultiModificaImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.CancellaAppezzamenti OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianificazioneInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.EliminaInterventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.Blocco_OperazioniAgenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Inv OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest_Coop OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Imprese OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Contatti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Centri OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Appezza OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Agenda OrElse
                    Qs_Funzione = enum_TipoFiltrone.Richiesta_Verifica_Conformita OrElse
                    Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Rintraccio OrElse
                    Qs_Funzione = enum_TipoFiltrone.Bilancio_Fertilizzazioni OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianiCampionamento_AggiungiImpianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Impianti OrElse
                    Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Interventi OrElse
                    Qs_Funzione = enum_TipoFiltrone.PianoConcimazione) AndAlso
                    DgrVuoti = "false" Then
                    '____________________________________________________DATAGRID MOVIMENTI___________________________________________
                    ' Me.CaricaDgrMovimento(strFiltro, classJoin)
                    Dim strOrdinamento As String = " ORDER BY Imprese.rag_soc, Centri_Aziendali.Sa_Nome, Movimenti.Data_Movimento, SpecieVegetali.Veg_Des, Cultivar.Cul_Des  "
                    Dim str_mov As String = " cau_mov='2050' or cau_mov='2100' or cau_mov='2200' or cau_mov='2300' "
                    strFiltro = strFiltro & " and (" & str_mov & " ) "
                    dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.Movimenti, strOrdinamento, classJoin, objParametri_Utenti)
                    str_RISPOSTA = DT_to_Json_Movimento(dt, JsonRichiesta, objParametri_Server)
                    'HttpContext.Current.Session("dt_x_export_movimento") = dt 'vanni
                    OUTPUT_dt_x_export_movimento = dt
                End If

            'Anna 17/08/21:Aggiunta scheda fabbricati nel filtrone, per TTI
            Case "fabbricato"

                If (Qs_Funzione = enum_TipoFiltrone.Stampa OrElse
                   Qs_Funzione = enum_TipoFiltrone.Agenda OrElse
                   Qs_Funzione = enum_TipoFiltrone.MultiModificaAgenda OrElse
                   Qs_Funzione = enum_TipoFiltrone.AnalisiCosti OrElse
                   Qs_Funzione = enum_TipoFiltrone.ModificaImpianti OrElse
                   Qs_Funzione = enum_TipoFiltrone.MultiModificaImpianti OrElse
                   Qs_Funzione = enum_TipoFiltrone.CancellaAppezzamenti OrElse
                   Qs_Funzione = enum_TipoFiltrone.PianificazioneInterventi OrElse
                   Qs_Funzione = enum_TipoFiltrone.EliminaInterventi OrElse
                   Qs_Funzione = enum_TipoFiltrone.Blocco_OperazioniAgenda OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Inv OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportazione_OP_Gest_Coop OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Impianti OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Imprese OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportatore_Contatti OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Centri OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Appezza OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Agenda OrElse
                   Qs_Funzione = enum_TipoFiltrone.Richiesta_Verifica_Conformita OrElse
                   Qs_Funzione = enum_TipoFiltrone.Esportatore_Universale_Rintraccio OrElse
                   Qs_Funzione = enum_TipoFiltrone.Bilancio_Fertilizzazioni OrElse
                   Qs_Funzione = enum_TipoFiltrone.PianiCampionamento_AggiungiImpianti OrElse
                   Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Impianti OrElse
                   Qs_Funzione = enum_TipoFiltrone.Associa_Ricetta_Interventi OrElse
                   Qs_Funzione = enum_TipoFiltrone.PianoConcimazione) AndAlso
                   DgrVuoti = "false" Then
                    '____________________________________________________DATAGRID FABBRICATI___________________________________________
                    Dim strOrdinamento As String = " ORDER BY Rag_Soc, fabbricati.PIVA, fabbricati.Sa_Cod, fabbricati.fabbricato_Cod, denominazione  DESC  "

                    dt = CreaDTFiltrone(parametriXQuery, objParametri_Server, strFiltro, enum_TipoSelect_FiltroneSuperNova.Fabbricati, strOrdinamento, classJoin, objParametri_Utenti)

                    str_RISPOSTA = DT_to_Json_Fabbricato(dt, JsonRichiesta, objParametri_Server)

                    OUTPUT_dt_x_export_fabbricato = dt
                End If

            Case Else

        End Select



        '----- Assemblo la struttura

        XmlDoc.AppendChild(XML_DatiFiltri)

        ' Nicoletta
        'HttpContext.Current.Session("Filtro" & CStr(JsonRichiesta.tab_selezionata)) = strFiltro

        '----- Restituisco il risultato  (se sono nella gestione permessi)

        If Qs_Funzione = enum_TipoFiltrone.Utenti Then

            OUTPUT_Xml_permessi = XmlDoc.OuterXml
            OUTPUT_Sql_Permessi = strFiltro

        Else
            OUTPUT_Xml_Filtro = XmlDoc.OuterXml
        End If

        OUTPUT_Join = classJoin
        classJoin = Nothing


        Return str_RISPOSTA

    End Function

    Public Function DT_to_AppezzamentoRipartoCatasto(
        piva As String,
        mostraRipartoAppezzamenti As Boolean,
        mostraAppezzamentoSenzaRiparto As Boolean,
        mostraParticelleSenzaRiparto As Boolean,
        DataDa As Date,
        DataA As Date,
        IDTestataTemp As Integer,
        dtFiltroAppezza As DataTable,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_utenti As AgronicaCoreParametri
    ) As DataTable

        'imposta e scrive su db il filtro, restituisce IDTestataTemp (e filtro se necessario)
        If dtFiltroAppezza IsNot Nothing Then

            Dim qry As String = ""
            DT_to_AppezzamentoRipartoCatasto_ScriviFiltro(
                NumeroElementi:=3,
                ScriviFiltroImpiantiSuDB:=True,
                dtFiltroAppezza:=dtFiltroAppezza,
                objParametri_Server:=objParametri_Server,
                cliIDTestataTemp:=IDTestataTemp,
                OperazioneCorrente_FiltroImpianti:=qry,
                nomeTab:=""
            )

        End If

        Dim stbAgea2015 As New StringBuilder
        stbAgea2015.AppendLine("--")


        Dim CodificaAgeaDaGias_livelloDettaglio As New List(Of enum_CodificaAgeaDaGias_livelloDettaglio)
        CodificaAgeaDaGias_livelloDettaglio.Add(enum_CodificaAgeaDaGias_livelloDettaglio.SpecieVegetale)
        CodificaAgeaDaGias_livelloDettaglio.Add(enum_CodificaAgeaDaGias_livelloDettaglio.CodiciAnagrafe)

        Dim SostiuisciSpecieSpecificaConSpecieGenerica As Boolean = True

        Dim leggiCodificheAgea As New AgronicaCoreMetaSchemaDAL.Agea_Codifiche_R
        leggiCodificheAgea.LeggiCodificheSuImpiantiOppurePlanning(
                stb:=stbAgea2015,
                piva:="",
                FiltroImpiantiConAND:="",
                Veg_Cod:=0,
                Cul_Cod:=0,
                Grfi_Cod:=0,
                Grva_Cod:=0,
                Metodo_Produzione_Cod:=0,
                Reg_Cod:=0,
                Id_Cod:=0,
                Grsp_Cod:=0,
                Cul_Cod_Agea:="",
                Uso_Cod_Agea:="",
                Occupazione_Cod_Agea:="",
                Destinazione_Cod_Agea:="",
                CodificaAgeaDaGias_livelloDettaglio:=CodificaAgeaDaGias_livelloDettaglio,
                SostiuisciSpecieSpecificaConSpecieGenerica:=SostiuisciSpecieSpecificaConSpecieGenerica,
                Qualita_Cod_Agea:="",
                xFiltroAggiuntivo:=" Veg_Cod_Agea <> '000' ",
                xOrderBy:="",
                objParametri:=objParametri_Server
            )

        Dim letturaRiparti As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R
        Dim dtRiparti As DataTable =
            letturaRiparti.LeggiAppezzamenti_Da_ParticellaGis(
            mostraRipartoAppezzamenti,
            mostraAppezzamentoSenzaRiparto,
            mostraParticelleSenzaRiparto,
            0,
            piva,
            0,
            DataDa,
            DataA,
            IDTestataTemp,
            True,
            stbAgea2015,
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            "",
            "",
            objParametri_Server)

        If dtRiparti.Rows.Count > 1 Then
            DT_to_AppezzamentoRipartoCatasto_Log(objParametri_Server, objParametri_utenti, dtRiparti)
            DT_to_AppezzamentoRipartoCatasto_OperazioniInfo(objParametri_Server, objParametri_utenti, dtRiparti)
            'appezzamento
        End If

        Dim xcancellaFiltroTemp As New AgronicaCoreVarieDAL.__tmp_FiltroImpianti_W
        xcancellaFiltroTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri_Server)
        Return dtRiparti

    End Function

    '########################################################################################################
    Public Function DT_to_CampoRipartoCatasto(
        piva As String,
        campiConRiparto As Boolean,
        campiSenzaRiparto As Boolean,
        particelleInConduzioneSenzaRipartoSuiCampi As Boolean,
        DataDa As Date,
        DataA As Date,
        IDTestataTemp As Integer,
        dtFiltroCampi As DataTable,
        tuttoIlCatastoInArchivio As Boolean,
        sintesiCUAAEstremiCatastali As Boolean,
        objParametri_Server As AgronicaCoreParametri,
        objParametri_utenti As AgronicaCoreParametri
    ) As DataTable

        'imposta e scrive su db il filtro, restituisce IDTestataTemp (e filtro se necessario)
        If dtFiltroCampi IsNot Nothing Then

            Dim qry As String = ""
            DT_to_CampoRipartoCatasto_ScriviFiltro(NumeroElementi:=3,
                                                   ScriviFiltroCampiSuDB:=True,
                                                   dtFiltroCampo:=dtFiltroCampi,
                                                   objParametri_Server:=objParametri_Server,
                                                   cliIDTestataTemp:=IDTestataTemp,
                                                   OperazioneCorrente_FiltroCampi:=qry
                                                   )
        End If

        Dim stbAgea2015 As New StringBuilder
        stbAgea2015.AppendLine("--")


        Dim CodificaAgeaDaGias_livelloDettaglio As New List(Of enum_CodificaAgeaDaGias_livelloDettaglio)
        CodificaAgeaDaGias_livelloDettaglio.Add(enum_CodificaAgeaDaGias_livelloDettaglio.SpecieVegetale)
        CodificaAgeaDaGias_livelloDettaglio.Add(enum_CodificaAgeaDaGias_livelloDettaglio.CodiciAnagrafe)

        Dim SostiuisciSpecieSpecificaConSpecieGenerica As Boolean = True

        Dim letturaRiparti As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
        Dim dtRiparti As DataTable =
            letturaRiparti.LeggiCampi_Da_Particelle(
            campiConRiparto,
            campiSenzaRiparto,
            particelleInConduzioneSenzaRipartoSuiCampi,
            tuttoIlCatastoInArchivio,
            0,
            piva,
            0,
            DataDa,
            DataA,
            IDTestataTemp,
            False,
            sintesiCUAAEstremiCatastali,
            objParametri_Server)

        Dim xcancellaFiltroTemp As New AgronicaCoreVarieDAL.__tmp_FiltroCampi_W
        xcancellaFiltroTemp.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri_Server)
        Return dtRiparti

    End Function

    '########################################################################################################

    Private Shared Sub DT_to_AppezzamentoRipartoCatasto_Log(
            objParametri_Server As AgronicaCoreParametri,
            objParametri_utenti As AgronicaCoreParametri,
            dtRiparti As DataTable)

        Dim LeggiLog As New AgronicaCoreAnagrafeDAL.CatastoModificheParticelleXAppezzamentiLog_R

        For Each rigaDettaglioRiparto As DataRow In dtRiparti.Rows


            If rigaDettaglioRiparto("piva") <> "" And rigaDettaglioRiparto("prov") <> "" Then
                Dim dtLeggiLog As DataTable =
                LeggiLog.Leggi(
                    0,
                    rigaDettaglioRiparto("piva"),
                    rigaDettaglioRiparto("sa_cod"),
                    0,
                    rigaDettaglioRiparto("appezza"),
                    rigaDettaglioRiparto("prov"),
                    rigaDettaglioRiparto("com"),
                    rigaDettaglioRiparto("sezione"),
                    rigaDettaglioRiparto("foglio"),
                    rigaDettaglioRiparto("numero"),
                    rigaDettaglioRiparto("subalterno"),
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "",
                    "",
                    objParametri_Server)

                Dim infoGeneraliRiparto As String =
                    GetInfoGeneraliRipartoLog(True, dtLeggiLog, objParametri_utenti)

                rigaDettaglioRiparto("infoGeneraliRiparto") = infoGeneraliRiparto

            End If

        Next

    End Sub

    Private Sub DT_to_AppezzamentoRipartoCatasto_OperazioniInfo(
            objParametri_Server As AgronicaCoreParametri,
            objParametri_utenti As AgronicaCoreParametri,
            dtRiparti As DataTable)

        Dim Impianti_Query1_TempTableCreazione As String = ""
        Dim Impianti_Query2_TempTableIndice As String = ""
        Dim Fornitori_Query1_TempTableCreazione As String = ""
        Dim Fornitori_Query2_TempTableIndice As String = ""

        DT_to_AppezzamentoRipartoCatasto_OpTmpTable(
            Impianti_Query1_TempTableCreazione,
            Impianti_Query2_TempTableIndice,
            Fornitori_Query1_TempTableCreazione,
            Fornitori_Query2_TempTableIndice
        )


        Dim QueryLetturaFornitoriEseguita As Boolean = False
        Dim DT_Fornitori As DataTable = Nothing

        Dim Impianti_Query3_TempTableFill As String = ""
        Dim Fornitori_Query3_TempTableFill As String = ""
        Impianti_QueryInsert_TFill(objParametri_Server,
                                   dtRiparti,
                                   Impianti_Query3_TempTableFill,
                                   Fornitori_Query3_TempTableFill)

        Dim objCoreStampa As New AgronicaCoreStampeDAL.AnagraficaAziendale
        Dim DtOperazioni As DataTable = objCoreStampa.PianoColturale_Excel_ImpiantiConOperazioni(
            Impianti_Query1_TempTableCreazione,
            Impianti_Query2_TempTableIndice,
            Impianti_Query3_TempTableFill,
            objParametri_Server)

        For Each rigaDettaglioRiparto As DataRow In dtRiparti.Rows


            Dim pivaDet As String = rigaDettaglioRiparto("piva").ToString()
            Dim sa_codDet As String = rigaDettaglioRiparto("sa_cod").ToString()
            Dim AppezzaDet As String = rigaDettaglioRiparto("Appezza").ToString()
            Dim id_regDet As String = rigaDettaglioRiparto("ID_Reg").ToString()
            Dim sSupImp As String = rigaDettaglioRiparto("Sup_Imp").ToString()

            If pivaDet <> "" AndAlso sa_codDet <> "" AndAlso AppezzaDet <> "" AndAlso id_regDet <> "" Then

                Dim drSemine As DataRow() = DtOperazioni.Select("Piva='" & pivaDet & "'" &
                                             " AND Sa_Cod=" & sa_codDet.ToString &
                                             " AND Appezza=" & AppezzaDet.ToString &
                                             " AND Id_Reg=" & id_regDet.ToString &
                                             " AND Lav_Cod IN (2,71) ")


                Dim strLotto_Seme As String = ""
                Dim strDataSemina As String = ""
                Dim strFornitore As String = ""
                Dim Udm_Seme As String = ""


                Dim QtaTotSeme As Integer = 0

                Dim Num_Semine As Integer = drSemine.Count

                For j = 0 To Num_Semine - 1

                    Dim qta_seme As Double

                    Dim Id_Semina As Integer = drSemine(j).Item("id_agenda")
                    Dim Data_Semina As Date = drSemine(j).Item("data_movimento")
                    Dim Lotto_Seme As String = "Cod." & drSemine(j).Item("Cod_Articolo")

                    If CStr(drSemine(j).Item("Lotto")).ToLower <> "indefinito" AndAlso drSemine(j).Item("Lotto") <> "" Then
                        Lotto_Seme &= " - Lotto: " & drSemine(j).Item("Lotto")
                    End If

                    Udm_Seme = drSemine(j).Item("udm_sim")

                    qta_seme = drSemine(j).Item("Qta_Impianto")
                    QtaTotSeme += qta_seme

                    '-------------------------------
                    'Ricavo il Fornitore delle sementi/piantine
                    'Fornitore = Recupera_Fornitore(Piva, drSemine(j).Item("mat_cod"))
                    'Fornitore = drSemine(j).Item("Fornitore")

                    Dim Fornitore As String = Recupera_Fornitore(
                        DT_Fornitori,
                        QueryLetturaFornitoriEseguita,
                        drSemine(j).Item("piva"),
                        drSemine(j).Item("mat_cod"),
                        Fornitori_Query1_TempTableCreazione,
                        Fornitori_Query2_TempTableIndice,
                        Fornitori_Query3_TempTableFill,
                        objParametri_Server
                    )

                    If strDataSemina.LastIndexOf(Data_Semina) < 0 Then
                        strDataSemina &= If(strDataSemina = "", "", " - ") & Data_Semina
                    End If

                    If strLotto_Seme.LastIndexOf(Lotto_Seme) < 0 Then
                        strLotto_Seme &= If(strLotto_Seme = "", "", " - ") & Lotto_Seme
                    End If

                    If strFornitore.LastIndexOf(Fornitore) < 0 Then
                        strFornitore &= If(strFornitore = "", "", " - ") & Fornitore
                    End If


                Next
                'semina

                ' dopo che ho finito il ciclo delle semine dell'impianto
                Dim sQtaTot_Seme = Format(QtaTotSeme, "0.00")

                'totale del seme dell'impianto
                Dim Qta As Double = CDbl(QtaTotSeme)

                Dim Sup_Imp As Double = 0
                If sSupImp <> "" Then
                    Sup_Imp = CDbl(sSupImp)
                End If

                Dim Qta_Seme_HA As Double
                If Sup_Imp <> 0 AndAlso Qta <> 0 Then
                    Qta_Seme_HA = Format(Qta / Sup_Imp, "0.00")
                Else
                    Qta_Seme_HA = 0
                End If

                Dim SemTrap_Superficie As Decimal = 0.0
                If drSemine.Length > 0 Then
                    SemTrap_Superficie = drSemine(0).Item("SemTrap_Superficie")
                End If
                Dim SemTrap_Ha = 0
                Dim SemTrap_Are = 0
                Dim SemTrap_Centiare = 0
                AgronicaCoreDataProvider.Conversioni.EttariAreCentiare_from_Ettari(SemTrap_Superficie, SemTrap_Ha, SemTrap_Are, SemTrap_Centiare)

                rigaDettaglioRiparto("SemTrap_Superficie") = SemTrap_Superficie
                rigaDettaglioRiparto("SemTrap_Ha") = SemTrap_Ha
                rigaDettaglioRiparto("SemTrap_Are") = SemTrap_Are
                rigaDettaglioRiparto("SemTrap_Centiare") = SemTrap_Centiare

                rigaDettaglioRiparto("SeminaTrapianto_data") = strDataSemina
                rigaDettaglioRiparto("SeminaTrapianto_fornitoreSeme") = strFornitore
                rigaDettaglioRiparto("SeminaTrapianto_lottoSeme") = strLotto_Seme
                rigaDettaglioRiparto("SeminaTrapianto_udm") = Udm_Seme
                rigaDettaglioRiparto("SeminaTrapianto_qta") = sQtaTot_Seme
                rigaDettaglioRiparto("SeminaTrapianto_qta_ha") = Qta_Seme_HA


            End If
            'se esiste impianto

        Next
        'Dettaglio

    End Sub

    Private Sub Impianti_QueryInsert_TFill(objParametri_Server As AgronicaCoreParametri,
                                           dtRiparti As DataTable,
                                           ByRef Impianti_Query3_TempTableFill As String,
                                           ByRef Fornitori_Query3_TempTableFill As String)

        Impianti_Query3_TempTableFill = " INSERT INTO #tempimpianti (Piva, Sa_Cod, Appezza, Id_Reg) VALUES  "
        Fornitori_Query3_TempTableFill = ""
        Dim DT_Impianti_Query3_TempTableFill As DataTable =
                DatatableUtility.SelectDistinct_To_DT(dtRiparti, True, {"piva", "sa_Cod", "appezza", "id_reg"})

        Dim HT_Imprese As New Hashtable

        DT_Impianti_Query3_TempTableFill.Columns.Add(New DataColumn("Chiave", GetType(System.String)))
        For Each cc In DT_Impianti_Query3_TempTableFill.Rows
            cc("chiave") =
                cc("piva") & "_" &
                cc("sa_Cod") & "_" &
                cc("appezza") & "_" &
                cc("id_reg")

            If Not HT_Imprese.Contains(cc("Piva")) Then
                HT_Imprese.Add(cc("Piva"), "")
                Fornitori_Query3_TempTableFill &= " INSERT INTO #tempimprese (Piva)  " & vbCrLf
                Fornitori_Query3_TempTableFill &= " VALUES     ('" & cc("Piva") & "' )  " & vbCrLf
            End If

        Next


        DT_to_AppezzamentoRipartoCatasto_ScriviFiltro(
                NumeroElementi:=4,
                ScriviFiltroImpiantiSuDB:=False,
                dtFiltroAppezza:=DT_Impianti_Query3_TempTableFill,
                objParametri_Server:=objParametri_Server,
                cliIDTestataTemp:=0,
                OperazioneCorrente_FiltroImpianti:=Impianti_Query3_TempTableFill,
                nomeTab:="#tempimpianti"
            )

    End Sub

    Private Shared Sub DT_to_AppezzamentoRipartoCatasto_OpTmpTable(ByRef Impianti_Query1_TempTableCreazione As String, ByRef Impianti_Query2_TempTableIndice As String, ByRef Fornitori_Query1_TempTableCreazione As String, ByRef Fornitori_Query2_TempTableIndice As String)
        Impianti_Query1_TempTableCreazione &= " SELECT Piva, Sa_Cod, Appezza, Id_Reg "
        Impianti_Query1_TempTableCreazione &= "   INTO #tempimpianti   "
        Impianti_Query1_TempTableCreazione &= "       FROM Reg_Impianti "
        Impianti_Query1_TempTableCreazione &= "           WHERE 1 = 0   "
        Impianti_Query1_TempTableCreazione &= vbCrLf
        Impianti_Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempimpianti] ON [dbo].[#tempimpianti]([Piva], [Sa_Cod], [Appezza], [Id_Reg]) "

        Fornitori_Query1_TempTableCreazione += " SELECT Piva "
        Fornitori_Query1_TempTableCreazione += "   INTO #tempimprese   "
        Fornitori_Query1_TempTableCreazione += "       FROM Imprese "
        Fornitori_Query1_TempTableCreazione += "           WHERE 1 = 0   "
        Fornitori_Query1_TempTableCreazione += vbCrLf

        Fornitori_Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempimprese] ON [dbo].[#tempimprese]([Piva]) "
    End Sub

    '##############################################################
    Private Function Recupera_Fornitore(ByRef DT_Fornitori As DataTable,
                                        ByRef QueryLetturaEseguita As Boolean,
                                        ByVal Piva As String,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Fornitori_Query1_TempTableCreazione As String,
                                        ByVal Fornitori_Query2_TempTableIndice As String,
                                        ByVal Fornitori_Query3_TempTableFill As String,
                                        objParametri_Server As AgronicaCoreParametri) As String

        Dim Des As String = ""
        Dim msg As String
        Dim i As Integer

        Try

            If Mat_Cod <> 0 Then

                If Not QueryLetturaEseguita Then

                    Dim objforn As New AgronicaCoreStampeDAL.AnagraficaAziendale

                    DT_Fornitori = objforn.PianoColturale_Excel_FornitoriSementi(Fornitori_Query1_TempTableCreazione,
                                                                                Fornitori_Query2_TempTableIndice,
                                                                                Fornitori_Query3_TempTableFill,
                                                                                objParametri_Server)


                    QueryLetturaEseguita = True

                End If

                If Not IsNothing(DT_Fornitori) AndAlso DT_Fornitori.Rows.Count > 0 Then

                    Dim dr As DataRow()

                    dr = DT_Fornitori.Select("Piva='" & Piva & "'" &
                                            " AND mat_cod = " & Mat_Cod.ToString & " ")

                    If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                        For i = 0 To dr.Length - 1
                            Des &= dr(i).Item("Fornitore") & " - "
                        Next
                    End If

                End If

            End If


        Catch ex As Exception
            msg = ex.Message
        End Try

        Return Des

    End Function

    Private Shared Function GetInfoGeneraliRipartoLog(ByVal MostraProposte As Boolean, ByVal dtDetLog As DataTable, ByVal objParametri_Utenti As AgronicaCoreParametri) As String

        Dim rval As New StringBuilder

        Dim letturaUtente As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim dtUtente As DataTable

        Dim flgRigaLog As Boolean = False

        For Each riga As DataRow In dtDetLog.Rows

            Dim descrizioneTipoOperazione As String = ""
            Dim ParametriSalvataggio As String = ""

            If MostraProposte Then
                flgRigaLog = True
            End If


            If riga("CfgSalvataggio").ToString <> "" Then
                Dim CfgSalvataggio1 As RipartoCatastoCfgSalvataggio =
                    JsonConvert.DeserializeObject(Of RipartoCatastoCfgSalvataggio)(riga("CfgSalvataggio").ToString)

                ParametriSalvataggio = AgronicaCoreUtility.jSon.Escape("Proposta")
                If CfgSalvataggio1.GlobalSalvaSelezione Then
                    flgRigaLog = True
                    ParametriSalvataggio = AgronicaCoreUtility.jSon.Escape("Confermata in effettivo")
                End If

            End If

            If flgRigaLog Then


                Select Case riga("Operazione_DB")
                    Case enum_TipoOperazioneDB.Scrittura
                        descrizioneTipoOperazione = AgronicaCoreUtility.jSon.Escape("Nuovo catasto investito")
                    Case enum_TipoOperazioneDB.Modifica
                        descrizioneTipoOperazione = AgronicaCoreUtility.jSon.Escape("Modifica superficie investita")
                    Case enum_TipoOperazioneDB.Cancellazione
                        descrizioneTipoOperazione = AgronicaCoreUtility.jSon.Escape("Cancellata associazione con particella")
                End Select

                Dim DettaglioSuperficie As String = ""
                If riga("Operazione_DB") = enum_TipoOperazioneDB.Modifica Then
                    DettaglioSuperficie =
                    AgronicaCoreUtility.jSon.Escape(" - Sup. Prec.") & " (Ha): {4} "
                End If

                Dim rigaStr As String = "[{7}]: {6} ({1} {2}, {3})" & DettaglioSuperficie


                dtUtente = letturaUtente.Utenti_Dettagli_from_CF(riga("Username_Modifica"), enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

                rval.Append(
                String.Format(
                    rigaStr,
                    "",
                    dtUtente(0)("Nome"),
                    dtUtente(0)("cognome"),
                    CDate(riga("Data_Modifica")).ToShortDateString,
                    Riparto_HaAreCentiAre_Ha(riga("vecchioInter_Ettari_Are_Centiare")),
                    Riparto_HaAreCentiAre_Ha(riga("inter_Ettari_Are_Centiare")),
                    descrizioneTipoOperazione,
                    ParametriSalvataggio
                )
            )
            End If
            'se devo loggare procedo

        Next
        'riga log

        If MostraProposte Then
            If dtDetLog.Rows.Count = 0 Then
                rval.Append(AgronicaCoreUtility.jSon.Escape("Riparto non generato da GIS"))
            End If
        Else
            If Not flgRigaLog Then
                rval.Append(AgronicaCoreUtility.jSon.Escape("Riparto non generato da GIS"))
            End If
        End If

        Return rval.ToString()

    End Function

    Private Shared Function Riparto_HaAreCentiAre_Ha(txtHaAre As String) As String

        Dim v1 As String() = txtHaAre.Split(".")
        If v1.Length <> 3 Then
            Return txtHaAre
        End If

        Riparto_HaAreCentiAre_Ha_impostaZero(v1(0))
        Riparto_HaAreCentiAre_Ha_impostaZero(v1(1))
        Riparto_HaAreCentiAre_Ha_impostaZero(v1(2))

        Try

            Dim rval As Decimal = AgronicaCoreDataProvider.UtilityProvider.Ettari_from_EttariAreCentiare(CDec(v1(0)), CDec(v1(1)), CDec(v1(2)))
            Return rval.ToString.Replace(".", ",")
        Catch ex As Exception

        End Try
        Return txtHaAre
    End Function

    Private Shared Sub Riparto_HaAreCentiAre_Ha_impostaZero(ByRef v1 As String)
        If v1 = "" Then
            v1 = "0"
        End If
    End Sub

    Private Sub DT_to_AppezzamentoRipartoCatasto_ScriviFiltro(
        NumeroElementi As Integer,
        ScriviFiltroImpiantiSuDB As Boolean,
        dtFiltroAppezza As DataTable,
        objParametri_Server As AgronicaCoreParametri,
        ByRef cliIDTestataTemp As Integer,
        ByRef OperazioneCorrente_FiltroImpianti As String,
        ByVal nomeTab As String
    )

        Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        If ScriviFiltroImpiantiSuDB Then
            'cliIDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            cliIDTestataTemp =
                xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
        End If

        If OperazioneCorrente_FiltroImpianti = "" Then
            OperazioneCorrente_FiltroImpianti =
                "insert into __tmp_FiltroImpianti (piva, sa_cod, appezza, id_reg, IDTestataTemp) values "
            nomeTab = "__tmp_FiltroImpianti"
        End If

        Dim listaFiltroImpianti As New List(Of String)
        For Each rr As DataRow In dtFiltroAppezza.Rows

            Dim ci = rr("Chiave").Split("_")

            Dim daAggiungere1 As New List(Of String)
            daAggiungere1.Add("'" & ci(0) & "'")
            daAggiungere1.Add(ci(1))
            daAggiungere1.Add(ci(2))
            If NumeroElementi = 4 Then
                daAggiungere1.Add(ci(3))
            Else
                daAggiungere1.Add("0")
            End If

            If cliIDTestataTemp > 0 Then
                daAggiungere1.Add(cliIDTestataTemp)
            End If

            Dim daAggiungere As String = "(" & String.Join(",", daAggiungere1) & ")"

            If Not listaFiltroImpianti.Contains(daAggiungere) Then
                listaFiltroImpianti.Add(daAggiungere)
            End If

        Next

        OperazioneCorrente_FiltroImpianti = AgronicaCoreUtility.QueryBuilderUtility.GeneraStringaInsertDaList(nomeTab, listaFiltroImpianti)

        If dtFiltroAppezza.Rows.Count > 0 AndAlso ScriviFiltroImpiantiSuDB Then
            PopolaTabellaFiltro(OperazioneCorrente_FiltroImpianti, objParametri_Server)
        End If

    End Sub

    Private Sub DT_to_CampoRipartoCatasto_ScriviFiltro(
        NumeroElementi As Integer,
        ScriviFiltroCampiSuDB As Boolean,
        dtFiltroCampo As DataTable,
        objParametri_Server As AgronicaCoreParametri,
        ByRef cliIDTestataTemp As Integer,
        ByRef OperazioneCorrente_FiltroCampi As String
    )

        Dim xAgrosequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        If ScriviFiltroCampiSuDB Then
            'cliIDTestataTemp = xAgrosequenze.Agronica_SequenzaTabelle_NuovoID("IDTestataTemp", objParametri_Server)
            'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
            cliIDTestataTemp =
                xAgrosequenze.NuovoId_Tabella("IDTestataTemp", 0, UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri_Server)
        End If

        If OperazioneCorrente_FiltroCampi = "" Then
            OperazioneCorrente_FiltroCampi =
                "insert into __tmp_FiltroCampi (piva, sa_cod, campo_cod, IDTestataTemp) values "
        End If

        Dim listaFiltroCampi As New List(Of String)
        For Each rr As DataRow In dtFiltroCampo.Rows

            Dim ci = rr("Chiave").Split("_")

            Dim daAggiungere1 As New List(Of String)
            daAggiungere1.Add("'" & ci(0) & "'")
            daAggiungere1.Add(ci(1))
            daAggiungere1.Add(ci(2))
            daAggiungere1.Add("0")

            If cliIDTestataTemp > 0 Then
                daAggiungere1.Add(cliIDTestataTemp)
            End If

            Dim daAggiungere As String = "(" & String.Join(",", daAggiungere1) & ")"

            If Not listaFiltroCampi.Contains(daAggiungere) Then
                listaFiltroCampi.Add(daAggiungere)
            End If

        Next

        OperazioneCorrente_FiltroCampi &= String.Join(",", listaFiltroCampi)

        If dtFiltroCampo.Rows.Count > 0 AndAlso ScriviFiltroCampiSuDB Then
            PopolaTabellaFiltro(OperazioneCorrente_FiltroCampi, objParametri_Server)
        End If

    End Sub

    Public Shared Sub PopolaTabellaFiltro(operazioneCorrente_Filtro As String,
                                          ByVal objParametri As AgronicaCoreParametri)

        Dim vQry As String() = operazioneCorrente_Filtro.Split("|")

        Dim scrivi As New AgronicaCoreDataProvider.DataProvider

        For Each stmt In vQry

            Dim rVal As Boolean = scrivi.EseguiQuery_Scrittura(objParametri, stmt, "PopolaTabellaFiltro")

        Next

    End Sub

    Public Shared Function DT_to_Json_Azienda(ByVal dt As DataTable,
                                              ByVal JsonRichiesta As filtrone_richiesta_nuovo,
                                              ByVal objParametri_Server As AgronicaCoreParametri
                                              ) As String

        Dim qs_FiltroAzioni As FiltroAzioni_nuovo = JsonRichiesta.filtroazioni

        Dim check As Boolean = False
        Select Case qs_FiltroAzioni.TipoFiltrone
            Case enum_TipoFiltrone.Gestione_Servizi,
                enum_TipoFiltrone.ExportSigpa
                check = True
            Case Else
                'solo se sto assegnando i permessi agli utenti visualizzo la colonna CheckBox nelle aziende
                If qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Quadro_P AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Esportatore_Universale_Imprese AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Esportazione_AnagraficaContatti AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Esportatore_Universale_Rintraccio AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Esportazione_OP_Gest AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Esportazione_OP_Produttori AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Esportazione_CellulariContatti AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Registro_Trattamenti_Massivo AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Registro_Fertilizzazioni_Massivo AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Adesione_Etico_Ambientale AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Tenuta_Scheda_Campagna AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Codice_Condotta AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Adesione_DPI AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Impegnativa_Eurep AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Impegnativa_QC AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Impegnativa_Confusione_Sessuale AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Adesione_Conad AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Adesione_Despar AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.ImpegnoProduzioneSociDivisoxCentri AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Accordo_Responsabilita_di_Filiera AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.ObiettivoDiProduzioneAsipo AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.QuestionarioValutazioneAzienda_Aggiornamento AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.ImpegnativaColtivazioneConferimento AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.SchedaAziendale AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.ImpegnoProduzioneSociDivisoxCentri AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.ImpegnativaColtivazioneConferimento AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Fitoregolatori_Kiwi AndAlso
                    qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Dichiarazione_di_Responsabilita Then

                    check = False
                Else
                    check = True

                End If
        End Select

        Dim majorSql As Integer = DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri_Server)
        Dim sql2017OrHigher = False
        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017OrHigher = True
        End If

        Dim mapCoopReferente As New Dictionary(Of String, String)
        For Each row In dt.Rows
            GetImpreseReferenti(dt, row, mapCoopReferente, dt.Rows.IndexOf(row), sql2017OrHigher, objParametri_Server)
        Next

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        If (Not String.IsNullOrEmpty(JsonRichiesta.filtroazioni.G2G_Cod) AndAlso Integer.Parse(JsonRichiesta.filtroazioni.G2G_Cod) > 0) OrElse
            qs_FiltroAzioni.Filtrino = "1" Then
            check = True
        End If

        If qs_FiltroAzioni.Filtrino = "10" Then ' usato per disattivare il check in alcune stampe
            check = False
        End If

        If check Then
            l.Add(New ColonneNome("chiave", "seleziona", "string") With {._hidden = True})
        End If

        l.Add(New ColonneNome("Referente", My.Resources.AgronicaCoreFiltroneBIZ.ImpresaReferente, "string") With {._Display = True, ._width = "300px"})
        l.Add(New ColonneNome("PIVA_padre", Gias.PIVAImpresaReferente, "string") With {._Display = False, ._width = "300px"})

        l.Add(New ColonneNome("Piva", Gias.Piva, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("CodiceCuaa", Gias.CodiceUnicoAziendaAgricolaSigla, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("TecnicoRifImpresa", Gias.TecnicoRifImpresa, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Gruppi_Raccolta", Gias.GruppoRaccolta, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Rag_Soc", Gias.RagioneSociale, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Provincia", Gias.Provincia, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Cap", Gias.CAP, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Localita", Gias.Localita, "string") With {._Display = True, ._width = "200px"})

        l.Add(New ColonneNome("Certificazione", Gias.Certificazione, "string") With {._Display = True, ._width = "200px"})

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Username_Creazione_Completo", Gias.UsernameCreazione, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Modifica_Completo", Gias.UsernameModifica, "string") With {._Display = True, ._width = "150px"})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp

    End Function

    Public Shared Function DT_to_Json_Centro(ByVal dt As DataTable,
                                             ByVal JsonRichiesta As filtrone_richiesta_nuovo,
                                             ByVal objParametri_Server As AgronicaCoreParametri
                                             ) As String

        Dim qs_FiltroAzioni As FiltroAzioni_nuovo = JsonRichiesta.filtroazioni

        Dim check As Boolean = True
        If qs_FiltroAzioni.Pagina_Origine.ToLower = "../Stampe/Menu_Stampe.aspx".ToLower Then
            'tutte le stampe  si
            If qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Quadro_P AndAlso
               qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.EstrattoreDatiGrafici AndAlso
               qs_FiltroAzioni.CodificaStampe <> enum_CodificaStampe.Esportatore_Universale_Centri Then
                'Se ho selezionato una stampa diversa dal quadro p non visualizzo la colonna CheckBox nei centri aziendali
                check = False
            End If
        Else
            check = False
        End If

        Dim majorSql As Integer = DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri_Server)
        Dim sql2017OrHigher = False
        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017OrHigher = True
        End If

        Dim mapCoopReferente As New Dictionary(Of String, String)
        For Each row In dt.Rows
            GetImpreseReferenti(dt, row, mapCoopReferente, dt.Rows.IndexOf(row), sql2017OrHigher, objParametri_Server)
        Next
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        If check Then
            l.Add(New ColonneNome("chiave", Gias.Seleziona, "string") With {._FormatoParticolare = " <input type='checkbox' value='{0}' class='seleziona_piva_sacod chk_tabella'/> "})
        End If

        l.Add(New ColonneNome("Referente", My.Resources.AgronicaCoreFiltroneBIZ.ImpresaReferente, "string") With {._Display = True, ._width = "300px"})
        l.Add(New ColonneNome("PIVA_padre", Gias.PIVAImpresaReferente, "string") With {._Display = False, ._width = "300px"})

        l.Add(New ColonneNome("Piva", Gias.Piva, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Rag_Soc", Gias.RagioneSociale, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("CodiceCuaa", Gias.CodiceUnicoAziendaAgricolaSigla, "string") With {._width = "200px"})
        l.Add(New ColonneNome("TecnicoRifImpresa", Gias.TecnicoRifImpresa, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Gruppi_Raccolta", Gias.GruppoRaccolta & " " & Gias.Azienda, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("RegioneImpresa", Gias.RegioneImpresa, "string") With {._Display = False, ._width = "150px"})

        l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string") With {._Display = True, ._width = "200px"})

        l.Add(New ColonneNome("Indirizzo", Gias.Indirizzo, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("CAP", Gias.CAP, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Comune", Gias.Comune, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Provincia", Gias.Provincia, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Comune_Particella", My.Resources.AgronicaCoreFiltroneBIZ.ComuneParticella, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Provincia_Particella", My.Resources.AgronicaCoreFiltroneBIZ.ProvinciaParticella, "string") With {._Display = True, ._width = "150px"})

        If dt.Columns.Contains(Gias.Sezione) Then
            l.Add(New ColonneNome(Gias.Sezione, Gias.Sezione, "string") With {._Display = True, ._width = "150px"})
        End If

        l.Add(New ColonneNome("Foglio", Gias.Foglio, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Numero", Gias.Numero, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Subalterno", Gias.Subalterno, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Validita_Inizio_Possesso", My.Resources.AgronicaCoreFiltroneBIZ.DataInizioPossesso, "date") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Validita_Fine_Possesso", My.Resources.AgronicaCoreFiltroneBIZ.DataFinePossesso, "date") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Sup_Catastale", Gias.SuperficieCatastaleAbbr & " [ha]", "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Sup_Condotta", Gias.SuperficieCondottaAbbr & " [ha]", "string") With {._Display = True, ._width = "150px"})
        If dt.Columns.Contains("Sup_condottaAcro") Then
            l.Add(New ColonneNome("Sup_condottaAcro", Gias.SuperficieCatastaleAbbr & " [ac]", "number") With {._width = "150px"})
        End If
        If dt.Columns.Contains("Sup_Catastale_Acro") Then
            l.Add(New ColonneNome("Sup_Catastale_Acro", Gias.SuperficieCondottaAbbr & " [ac]", "number") With {._width = "150px"})
        End If

        l.Add(New ColonneNome("Certificazione", Gias.Certificazione, "string") With {._Display = True, ._width = "200px"})

        l.Add(New ColonneNome("Ultima_Verifica_Ispettiva", Gias.UltimaVerificaIspettiva, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("SAU_Tot_Aziendale", Gias.SAUTotAziendale & " [ha]", "string") With {._Display = False, ._width = "150px"})

        Dim _latCentro As New ColonneNome("Latitudine", Gias.Latitudine, "number") With {._Display = True, ._width = "150px"}
        _latCentro._formatNr = "n7"
        l.Add(_latCentro)
        Dim _longCentro As New ColonneNome("Longitudine", Gias.Longitudine, "number") With {._Display = True, ._width = "150px"}
        _longCentro._formatNr = "n7"
        l.Add(_longCentro)

        ' ########## ANAGRAFICA RAPPRESENTANTE LEGALE ##########
        l.Add(New ColonneNome("LegaleRappresentante", Gias.LegaleRappresentante, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("LegaleRappresentante_LuogoNascita", Gias.LuogoNascitaLegaleRappresentante, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("LegaleRappresentante_DataNascita", Gias.DataNascitaLegaleRappresentante, "date") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("LegaleRappresentante_CodiceFiscale", Gias.CodiceFiscaleLegaleRappresentante, "string") With {._Display = False, ._width = "200px"})


        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._Display = True, ._width = "150px"})

        'mancano le particelle
        l.Add(New ColonneNome("Username_Creazione_Completo", Gias.UtenteCreazione, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Modifica_Completo", Gias.UtenteModifica, "string") With {._Display = True, ._width = "150px"})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp

    End Function

    Public Shared Function DT_to_Json_Campo(ByVal dt As DataTable, ByVal JsonRichiesta As filtrone_richiesta_nuovo, ByVal objParametri_Server As AgronicaCoreParametri) As String
        Dim qs_FiltroAzioni As FiltroAzioni_nuovo = JsonRichiesta.filtroazioni
        Dim check As Boolean = False

        If qs_FiltroAzioni.TipoFiltrone = enum_TipoFiltrone.OperazioniMultiAziendali Then
            check = True
        End If

        Dim majorSql As Integer = DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri_Server)
        Dim sql2017OrHigher = False
        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017OrHigher = True
        End If

        Dim mapCoopReferente As New Dictionary(Of String, String)
        For Each row In dt.Rows
            GetImpreseReferenti(dt, row, mapCoopReferente, dt.Rows.IndexOf(row), sql2017OrHigher, objParametri_Server)
        Next
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        If check Then
            l.Add(New ColonneNome("chiave", "seleziona", "string") With {._hidden = True})
        End If

        l.Add(New ColonneNome("Referente", My.Resources.AgronicaCoreFiltroneBIZ.ImpresaReferente, "string") With {._Display = True, ._width = "300px"})
        l.Add(New ColonneNome("PIVA_padre", Gias.PIVAImpresaReferente, "string") With {._Display = False, ._width = "300px"})

        l.Add(New ColonneNome("Rag_Soc", Gias.RagioneSociale, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Piva", Gias.Piva, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("CodiceCuaa", Gias.CodiceUnicoAziendaAgricolaSigla, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("CodiceSocio", Gias.CodiceSocio, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("TecnicoRifImpresa", Gias.TecnicoRifImpresa, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Gruppi_Raccolta", Gias.GruppoRaccolta, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Contratto_Produzione", My.Resources.AgronicaCoreFiltroneBIZ.ContrattoProduzione, "string") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("Indirizzo_Azienda", Gias.IndirizzoAzienda, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string") With {._Display = True, ._width = "200px"})

        l.Add(New ColonneNome("Indirizzo_Centro", Gias.IndirizzoCentro, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Comune_Centro", Gias.ComuneCentro, "string") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("Provincia_Centro", Gias.ProvinciaCentro, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("CAP_Centro", Gias.CAPCentro, "string") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("Stato_Centro", Gias.StatoCentro, "string") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("Codice_Campo", My.Resources.AgronicaCoreFiltroneBIZ.CodiceCampo, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Campo_Des", Gias.Campo, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Superficie_Totale", Gias.SuperficieTotale, "number") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Superficie_Biologico", My.Resources.AgronicaCoreFiltroneBIZ.SuperficieBiologico, "number") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Superficie_Convenzionale", My.Resources.AgronicaCoreFiltroneBIZ.SuperficieConvenzionale, "number") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Superficie_Conversione", My.Resources.AgronicaCoreFiltroneBIZ.SuperficieConversione, "number") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Superficie_Catastale", Gias.SuperficieCatastale, "number") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Gru_Des", Gias.GruppoVegetale, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Veg_Des", Gias.Specie, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Sup_Contratto", My.Resources.AgronicaCoreFiltroneBIZ.SuperficieContrattoAbbr, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Filiera", My.Resources.AgronicaCoreFiltroneBIZ.Filiera, "string") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Username_Creazione_Completo", Gias.UtenteCreazione, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Modifica_Completo", Gias.UtenteModifica, "string") With {._Display = True, ._width = "150px"})
        'mancano le particelle

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp

    End Function

    Public Shared Function DT_to_Json_AppezzamentoRipartoCatasto(ByVal dt As DataTable,
                                                                 Optional ByVal SintesiCUAAEstremiCatastali As Boolean = False) As String


        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("chiave", "seleziona", "string") With {._hidden = True})

        l.Add(New ColonneNome("TipoDatoEstratto", "Tipo Di Dato Estratto", "string"))
        l.Add(New ColonneNome("PadreInGerarchia", "Padre In Gerarchia", "string"))
        l.Add(New ColonneNome("OrganismoReferente", Gias.OrganismoReferente, "string"))
        l.Add(New ColonneNome("Piva", Gias.PartitaIva, "string"))
        l.Add(New ColonneNome("CUAA", "CUAA", "string"))
        l.Add(New ColonneNome("Rag_Soc", Gias.RagioneSociale, "string"))
        l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string"))
        l.Add(New ColonneNome("Campo_Des", Gias.Campo, "string"))
        l.Add(New ColonneNome("RiferimentoAppezzamento", "Rif. Appezzamento", "string"))

        Dim c As New ColonneNome("", "", "")

        If Not SintesiCUAAEstremiCatastali Then
            l.Add(New ColonneNome("APP_NOME", Gias.Appezzamento, "string"))
            l.Add(New ColonneNome("IndirizzoAppezzamento", Gias.Indirizzo, "string"))
            l.Add(New ColonneNome("N_AppezzamentoBiologico", Gias.NumeroAbbr & " " & Gias.Appezzamento & " " & Gias.Biologico, "string"))

            c = New ColonneNome("sup_app", Gias.Superficie & " " & Gias.Appezzamento & " [ha]", "number")
            c._sum = True
            c._formatNr = "n4"
            c._width = "150px"
            l.Add(c)

            If dt.Columns.Contains("acro") Then
                Dim Ac As New ColonneNome("acro", Gias.Superficie & " [ac]", "number")
                Ac._sum = True
                Ac._formatNr = "n4"
                Ac._width = "150px"
                l.Add(Ac)
            End If

            c = New ColonneNome("AreaAppezzamentoGIS", Gias.Superficie & " GIS Appezzamento [ha]", "number")
            c._formatNr = "n4"
            l.Add(c)

            l.Add(New ColonneNome("ePart", "Gis - Poligono Particella Presente", "string"))
            l.Add(New ColonneNome("eApp", "Gis - Poligono Appezzamento Presente", "string"))
            l.Add(New ColonneNome("eImp", "Gis - Poligono Impianto Presente", "string"))

            c = New ColonneNome("Appezzamento_Latitudine", "Gis - Appezzamento Latitudine", "number")
            c._formatNr = "n5"
            l.Add(c)
            c = New ColonneNome("Appezzamento_Longitudine", "Gis - Appezzamento Longitudine", "number")
            c._formatNr = "n5"
            l.Add(c)

            c = New ColonneNome("InfoGeneraliRiparto", "Informazioni Sul Riparto", "string")
            c._RemoveHtmlEncode = True
            l.Add(c)
        End If

        l.Add(New ColonneNome("RiferimentoParticella", "Cod. Particella", "string"))

        l.Add(New ColonneNome("Comuni_Prov", "Prov", "string"))
        l.Add(New ColonneNome("Localita", "Comune", "string"))
        l.Add(New ColonneNome("CAP", "CAP", "string"))
        l.Add(New ColonneNome("Prov", "ISTAT - Provincia", "string"))
        l.Add(New ColonneNome("Com", "ISTAT - Comune", "string"))

        l.Add(New ColonneNome("Sezione", "Sezione", "string"))
        l.Add(New ColonneNome("Foglio", "Foglio", "string"))
        l.Add(New ColonneNome("Numero", "Particella", "string"))
        l.Add(New ColonneNome("Subalterno", "Subalterno", "string"))
        l.Add(New ColonneNome("Macrouso_DES", "Macrouso", "string"))

        If Not SintesiCUAAEstremiCatastali Then
            c = New ColonneNome("Area", "Sup. Inters. [Ha]", "number")
            c._formatNr = "n4"
            c._sum = True
            l.Add(c)
        End If

        c = New ColonneNome("ParticellaSuperficieHa", "Particella Superficie [Ha]", "number")
        c._formatNr = "n4"
        c._sum = True
        l.Add(c)
        l.Add(New ColonneNome("ETTARI", "Particella Ha ", "number"))
        l.Add(New ColonneNome("ARE", "Particella Are ", "number"))
        l.Add(New ColonneNome("CENTIARE", "Particella Centiare ", "number"))

        If SintesiCUAAEstremiCatastali Then
            c = New ColonneNome("sup_app", Gias.Superficie & " " & Gias.Appezzamento & " [ha]", "number")
            c._sum = True
            c._formatNr = "n4"
            c._width = "150px"
            l.Add(c)

            If dt.Columns.Contains("acro") Then
                Dim Ac As New ColonneNome("acro", Gias.Superficie & " [ac]", "number")
                Ac._sum = True
                Ac._formatNr = "n4"
                Ac._width = "150px"
                l.Add(Ac)
            End If

            l.Add(New ColonneNome("sup_app_Ha", "Superficie Appezzamento Ha ", "number"))
            l.Add(New ColonneNome("sup_app_Are", "Superficie Appezzamento Are ", "number"))
            l.Add(New ColonneNome("sup_app_Centiare", "Superficie Appezzamento Centiare ", "number"))

        End If

        If Not SintesiCUAAEstremiCatastali Then
            l.Add(New ColonneNome("RiferimentoImpianto", "Codice Impianto", "string"))
            l.Add(New ColonneNome("LottoEsercizio", "Lotto Esercizio", "string"))
            l.Add(New ColonneNome("RiferimentoEsercizio", "Riferimento Esercizio", "string"))

            'Salvatore Zammataro: 06/07/2022
            l.Add(New ColonneNome("CapitolatoPrivato", "Capitolato Privato", "string"))
            l.Add(New ColonneNome("DettaglioSpeciePersonalizzato", "Dettaglio Specie Personalizzato", "string"))
            l.Add(New ColonneNome("Regolamento", "Regolamento", "string"))
            l.Add(New ColonneNome("FormaAllevamento", "Forma Allevamento", "string"))
            l.Add(New ColonneNome("GruppoVegetaleDes", "Gruppo Vegetale", "string"))
            l.Add(New ColonneNome("Portinnesto", "Portinnesto", "string"))
            l.Add(New ColonneNome("ImpiantoIrrigazione", "Impianto Irrigazione", "string"))
            l.Add(New ColonneNome("Copertura", "Copertura", "string"))
            '-------------------------------

            l.Add(New ColonneNome("Specie", Gias.Specie, "string"))
            l.Add(New ColonneNome("Varieta", Gias.Varieta, "string"))
            l.Add(New ColonneNome("TipologiaVarietale", "Tipologia Varietale", "string"))
            l.Add(New ColonneNome("Finalita", Gias.Finalita, "string"))

            l.Add(New ColonneNome("Precessione1", "COLTURA PRECEDENTE 1° ANNO", "string"))
            l.Add(New ColonneNome("Precessione2", "COLTURA PRECEDENTE 2° ANNO", "string"))
            l.Add(New ColonneNome("Precessione3", "COLTURA PRECEDENTE 3° ANNO", "string"))
            l.Add(New ColonneNome("Precessione4", "COLTURA PRECEDENTE 4° ANNO", "string"))

            l.Add(New ColonneNome("Appezzamento_Validita_Inizio", Gias.Appezzamento & ": " & Gias.ValiditaInizio, "date"))
            l.Add(New ColonneNome("Appezzamento_Validita_Fine", Gias.Appezzamento & ": " & Gias.ValiditaFine, "date"))

            l.Add(New ColonneNome("Impianto_Validita_Inizio", Gias.Impianto & ": " & Gias.ValiditaInizio, "date"))
            l.Add(New ColonneNome("Impianto_Validita_Fine", Gias.Impianto & ": " & Gias.ValiditaFine, "date"))

            l.Add(New ColonneNome("Esercizio_Validita_Inizio", Gias.Esercizio & ": " & Gias.ValiditaInizio, "date"))
            l.Add(New ColonneNome("Esercizio_Validita_Fine", Gias.Esercizio & ": " & Gias.ValiditaFine, "date"))

            l.Add(New ColonneNome("MesiValiditaEsercizio", "Durata di Validita Esercizio in mesi", "number"))

            c = New ColonneNome("PianteHa", " [Piante/Ha]", "number")
            c._formatNr = "n4"
            l.Add(c)

            c = New ColonneNome("PianteHa_Impianto", " Piante Per Impianto", "number")
            c._formatNr = "n4"
            l.Add(c)

            c = New ColonneNome("ResaKgHa", Gias.Resa & " [kg/Ha]", "number")
            c._formatNr = "n4"
            l.Add(c)

            c = New ColonneNome("ResaKgHa_Impianto", Gias.Resa & " Impianto ", "number")
            c._formatNr = "n4"
            l.Add(c)
        End If

        l.Add(New ColonneNome("SeminaTrapianto_data", "Semina/Trapianto: Date ", "string"))
        l.Add(New ColonneNome("SeminaTrapianto_lottoSeme", "Semina/Trapianto: Lotti ", "string"))

        If Not SintesiCUAAEstremiCatastali Then
            c = New ColonneNome("SemTrap_Superficie", "Semina/Trapianto Superficie [Ha] ", "number")
            c._formatNr = "n4"
            c._sum = True
            l.Add(c)
            l.Add(New ColonneNome("SemTrap_Ha", "Semina/Trapianto Ha ", "number"))
            l.Add(New ColonneNome("SemTrap_Are", "Semina/Trapianto Are ", "number"))
            l.Add(New ColonneNome("SemTrap_Centiare", "Semina/Trapianto Centiare ", "number"))

            l.Add(New ColonneNome("SeminaTrapianto_udm", "Semina/Trapianto: Unità di misura", "string"))
            l.Add(New ColonneNome("SeminaTrapianto_qta", "Semina/Trapianto: Quantità Seminata/Trapiantata", "number"))
            l.Add(New ColonneNome("SeminaTrapianto_qta_ha", "Semina/Trapianto: Quantità Seminata/Trapiantata/HA", "number"))
            l.Add(New ColonneNome("SeminaTrapianto_fornitoreSeme", "Semina/Trapianto: Fornitore Seme ", "string"))
        End If

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp

    End Function

    Public Shared Function DT_to_Json_CampoRipartoCatasto(ByVal dt As DataTable,
                                                          ByVal tuttoIlCatastoInArchivio As Boolean,
                                                          Optional ByVal SintesiCUAAEstremiCatastali As Boolean = False) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As New ColonneNome("", "", "")

        l.Add(New ColonneNome("TipoDatoEstratto", "Tipo Di Dato Estratto", "string"))
        l.Add(New ColonneNome("OrganismoReferente", Gias.OrganismoReferente, "string"))
        l.Add(New ColonneNome("PIVA", Gias.PartitaIva, "string"))
        l.Add(New ColonneNome("CUAA", "CUAA", "string"))
        l.Add(New ColonneNome("rag_soc", Gias.RagioneSociale, "string"))

        If Not SintesiCUAAEstremiCatastali Then
            l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string"))
            l.Add(New ColonneNome("Campo_Des", Gias.Campo, "string"))
        End If

        l.Add(New ColonneNome("COMUNI_PROV", "Prov", "string"))
        l.Add(New ColonneNome("LOCALITA", "Comune", "string"))
        l.Add(New ColonneNome("CAP", "CAP", "string"))
        l.Add(New ColonneNome("PROV", "ISTAT - Provincia", "string"))
        l.Add(New ColonneNome("COM", "ISTAT - Comune", "string"))

        l.Add(New ColonneNome("SEZIONE", "Sezione", "string"))
        l.Add(New ColonneNome("FOGLIO", "Foglio", "string"))
        l.Add(New ColonneNome("NUMERO", "Particella", "string"))
        l.Add(New ColonneNome("SUBALTERNO", "Subalterno", "string"))

        c = New ColonneNome("ParticellaSuperficieHa", "Particella Superficie [Ha]", "number")
        c._formatNr = "n4"
        If Not tuttoIlCatastoInArchivio Then
            c._sum = True
        End If

        l.Add(c)
        l.Add(New ColonneNome("ETTARI", "Particella Ha ", "number"))
        l.Add(New ColonneNome("ARE", "Particella Are ", "number"))
        l.Add(New ColonneNome("CENTIARE", "Particella Centiare ", "number"))

        c = New ColonneNome("SemTrap_Superficie", "Semina/Trapianto Superficie [Ha] ", "number")
        c._formatNr = "n4"
        If Not tuttoIlCatastoInArchivio Then
            c._sum = True
        End If
        l.Add(c)
        l.Add(New ColonneNome("SemTrap_Ha", "Semina/Trapianto Ha ", "number"))
        l.Add(New ColonneNome("SemTrap_Are", "Semina/Trapianto Are ", "number"))
        l.Add(New ColonneNome("SemTrap_Centiare", "Semina/Trapianto Centiare ", "number"))

        If Not SintesiCUAAEstremiCatastali Then
            l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date"))
            l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date"))
        End If

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp

    End Function

    Public Shared Function DT_to_Json_Appezzamento(ByVal dt As DataTable, ByVal objParametri_Server As AgronicaCoreParametri) As String

        Dim majorSql As Integer = DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri_Server)
        Dim sql2017OrHigher = False
        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017OrHigher = True
        End If

        Dim mapCoopReferente As New Dictionary(Of String, String)
        For Each row In dt.Rows
            GetImpreseReferenti(dt, row, mapCoopReferente, dt.Rows.IndexOf(row), sql2017OrHigher, objParametri_Server)
        Next

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("Referente", My.Resources.AgronicaCoreFiltroneBIZ.ImpresaReferente, "string") With {._Display = True, ._width = "300px"})
        l.Add(New ColonneNome("PIVA_padre", Gias.PIVAImpresaReferente, "string") With {._Display = False, ._width = "300px"})

        l.Add(New ColonneNome("Piva", Gias.Piva, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Rag_Soc", Gias.RagioneSociale, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("CodiceCuaa", Gias.CodiceUnicoAziendaAgricolaSigla, "string") With {._width = "200px"})
        l.Add(New ColonneNome("TecnicoRifImpresa", Gias.TecnicoRifImpresa, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Gruppi_Raccolta", Gias.GruppoRaccolta, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Campo_Des", Gias.Campo, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("APP_NOME", Gias.Appezzamento, "string") With {._Display = True, ._width = "150px"})

        Dim c As New ColonneNome("SUP_APP", Gias.Superficie & " [ha]", "number")
        c._sum = True
        c._formatNr = "n4"
        c._width = "150px"
        l.Add(c)

        If dt.Columns.Contains("acro") Then
            Dim Ac As New ColonneNome("acro", Gias.Superficie & " [ac]", "number")
            Ac._sum = True
            Ac._formatNr = "n4"
            Ac._width = "150px"
            l.Add(Ac)
        End If

        l.Add(New ColonneNome("ZVN", "ZVN", "string") With {._Display = True, ._width = "150px"})

        Dim _lat As New ColonneNome("Latitudine", Gias.Latitudine, "number") With {._Display = True, ._width = "150px"}
        _lat._formatNr = "n7"
        l.Add(_lat)
        Dim _long As New ColonneNome("Longitudine", Gias.Longitudine, "number") With {._Display = True, ._width = "150px"}
        _long._formatNr = "n7"
        l.Add(_long)

        l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Username_Creazione_Completo", Gias.UtenteCreazione, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Modifica_Completo", Gias.UtenteModifica, "string") With {._Display = True, ._width = "150px"})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp

    End Function

    Public Shared Function DT_to_Json_Distinta(
        ByVal dt As DataTable, ByVal JsonRichiesta As filtrone_richiesta_nuovo,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        ByVal ASG_SuperUser_CodFiscale As String,
        ByVal ASG_Utente_Username_Crypt As String,
        ByVal ASG_Utente_Password_Crypt As String
        ) As String
        Dim qs_FiltroAzioni As FiltroAzioni_nuovo = JsonRichiesta.filtroazioni

        dt = DistintaPredisponiDatiMancanti(dt, JsonRichiesta, objParametri_Server, objParametri_Utenti, ASG_SuperUser_CodFiscale, ASG_Utente_Username_Crypt, ASG_Utente_Password_Crypt)

        Dim check As Boolean = True
        Select Case qs_FiltroAzioni.TipoFiltrone
            Case enum_TipoFiltrone.Utenti,
                 enum_TipoFiltrone.Agenda,
                 enum_TipoFiltrone.MultiModificaAgenda,
                 enum_TipoFiltrone.EliminaInterventi,
                 enum_TipoFiltrone.Blocco_OperazioniAgenda
                check = False

            Case enum_TipoFiltrone.Stampa
                If qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Quadro_P OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.EstrattoreDatiGrafici OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.ReportRisultatoFilrone OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.ReportConserveItalia OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportatore_Universale_Imprese OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportazione_AnagraficaContatti OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportatore_Universale_Rintraccio OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportazione_OP_Gest OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportatore_Universale_Agenda OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportazione_OP_Produttori OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportazione_CellulariContatti OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Adesione_Etico_Ambientale OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Tenuta_Scheda_Campagna OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Codice_Condotta OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Adesione_DPI OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Impegnativa_Eurep OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Impegnativa_QC OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Impegnativa_Confusione_Sessuale OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati Then

                    check = False
                End If

            Case enum_TipoFiltrone.Esportatore_Universale_Imprese,
                 enum_TipoFiltrone.Esportatore_Universale_Agenda,
                 enum_TipoFiltrone.Esportatore_Contatti,
                 enum_TipoFiltrone.Esportatore_Universale_Centri,
                 enum_TipoFiltrone.Esportatore_Universale_Appezza,
                 enum_TipoFiltrone.Esportatore_Universale_Agenda,
                 enum_TipoFiltrone.Esportatore_Universale_Rintraccio,
                 enum_TipoFiltrone.Esportazione_OP_Gest,
                 enum_TipoFiltrone.Esportazione_CellulariTecnici
                'enum_TipoFiltrone.PianiCampionamento_AggiungiImpianti
                check = False

            Case enum_TipoFiltrone.PianoConcimazione

        End Select

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        If check Then
            l.Add(New ColonneNome("chiave", "seleziona", "string") With {._hidden = True})
        End If


        ' ########## ANAGRAFICA IMPRESA ##########
        l.Add(New ColonneNome("Referente", My.Resources.AgronicaCoreFiltroneBIZ.ImpresaReferente, "string") With {._Display = True, ._width = "300px"})
        l.Add(New ColonneNome("PIVA_padre", Gias.PIVAImpresaReferente, "string") With {._Display = False, ._width = "300px"})

        l.Add(New ColonneNome("PIVA", (Gias.Piva).ToUpper, "string") With {._Display = False, ._width = "300px"})
        l.Add(New ColonneNome("Rag_Soc", Gias.RagioneSociale, "string") With {._Display = False, ._width = "300px"})
        l.Add(New ColonneNome("CodiceCuaa", Gias.CodiceUnicoAziendaAgricolaSigla, "string") With {._width = "200px"})
        l.Add(New ColonneNome("CodiceSocio", Gias.CodiceSocio, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("TecnicoRifImpresa", Gias.TecnicoRifImpresa, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Gruppi_Raccolta", Gias.GruppoRaccolta & " " & Gias.Azienda, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("imp_ind_des", Gias.IndirizzoImpresa, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("imp_frz_des", Gias.FrazioneImpresa, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("imp_cap", Gias.CAPImpresa, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("imp_com_des", Gias.ComuneImpresa, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("imp_pro_cod", Gias.ProvinciaImpresa, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("RegioneImpresa", Gias.RegioneImpresa, "string") With {._Display = False, ._width = "150px"})

        l.Add(New ColonneNome("Ultima_Verifica_Ispettiva", Gias.UltimaVerificaIspettiva, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("SAU_Tot_Aziendale", Gias.SAUTotAziendale & " [ha]", "string") With {._Display = False, ._width = "150px"})


        ' ########## ANAGRAFICA TECNICO ##########
        l.Add(New ColonneNome("IndirizzoEmailTecnico", Gias.IndirizzoEmailTecnico, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("CellulareTecnico", Gias.CellulareTecnico, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("GenereTecnico", Gias.GenereTecnico, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("DataNascitaTecnico", Gias.DataNascitaTecnico, "date") With {._Display = False, ._width = "200px"})


        ' ########## ANAGRAFICA RAPPRESENTANTE LEGALE ##########
        l.Add(New ColonneNome("LegaleRappresentante", Gias.LegaleRappresentante, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("LegaleRappresentante_LuogoNascita", Gias.LuogoNascitaLegaleRappresentante, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("LegaleRappresentante_DataNascita", Gias.DataNascitaLegaleRappresentante, "date") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("LegaleRappresentante_CodiceFiscale", Gias.CodiceFiscaleLegaleRappresentante, "string") With {._Display = False, ._width = "200px"})


        ' ########## ANAGRAFICA CENTRO ##########
        l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("cen_ind_des", Gias.IndirizzoCentro, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("cen_frz_des", Gias.FrazioneCentro, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("cen_cap", Gias.CAPCentro, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("cen_com_des", Gias.ComuneCentro, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("cen_pro_cod", Gias.ProvinciaCentro, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("pro_cod_istat", Gias.ISTATProvCentro, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("com_cod_istat", Gias.ISTATComuneCentro, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("codice_centro", Gias.CodiceCentro, "string") With {._Display = False, ._width = "150px"})

        Dim _latCentro As New ColonneNome("LatitudineCentro", Gias.Latitudine & " " & Gias.CentroAziendale, "number") With {._Display = True, ._width = "150px"}
        _latCentro._formatNr = "n7"
        l.Add(_latCentro)
        Dim _longCentro As New ColonneNome("LongitudineCentro", Gias.Longitudine & " " & Gias.CentroAziendale, "number") With {._Display = True, ._width = "150px"}
        _longCentro._formatNr = "n7"
        l.Add(_longCentro)

        ' ########## ANAGRAFICA CAMPO ##########
        l.Add(New ColonneNome("Campo_Des", Gias.Campo, "string") With {._width = "150px"})


        ' ########## ANAGRAFICA APPEZZAMENTO ##########
        l.Add(New ColonneNome("APP_NOME", Gias.AppezzamentoAbbr, "string") With {._width = "100px"})
        l.Add(New ColonneNome("SUP_APP", Gias.SuperficieAbbr & " " & Gias.AppezzamentoAbbr & " [ha]", "number") With {._width = "150px"})
        If dt.Columns.Contains("acro") Then
            l.Add(New ColonneNome("acro", Gias.SuperficieAbbr & " " & Gias.AppezzamentoAbbr & " [ac]", "number") With {._width = "150px"})
        End If

        l.Add(New ColonneNome("nazioneKPIN", My.Resources.AgronicaCoreFiltroneBIZ.NazioneApp, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("regioneKPIN", My.Resources.AgronicaCoreFiltroneBIZ.RegioneApp, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("provinciaKPIN", My.Resources.AgronicaCoreFiltroneBIZ.ProvinciaApp, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("comuneKPIN", My.Resources.AgronicaCoreFiltroneBIZ.ComuneApp, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("frazioneKPIN", My.Resources.AgronicaCoreFiltroneBIZ.CittaApp, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("indirizzoKPIN", My.Resources.AgronicaCoreFiltroneBIZ.IndirizzoApp, "string") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("kpin", "kpin", "string") With {._Display = False, ._width = "100px"})
        l.Add(New ColonneNome("blockName", "blockName", "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("growernumber", "GrowerNumber", "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("chiuso", My.Resources.AgronicaCoreFiltroneBIZ.Chiuso, "string") With {._FormatoParticolare = "<span class='chiuso chiuso_{0}'></span>", ._width = "150px"})

        l.Add(New ColonneNome("Esposizione", Gias.EsposizioneApp, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Pendenza", Gias.PendenzaApp, "string") With {._Display = False, ._width = "150px"})

        l.Add(New ColonneNome("DurataAppezzamento", Gias.DurataAppezzamento, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("InizioDurataAppezzamento", Gias.IniziovaliditaAppezzamento, "date") With {._Display = False, ._width = "300px"})
        l.Add(New ColonneNome("FineDurataAppezzamento", Gias.FineValiditaAppezzamento, "date") With {._Display = False, ._width = "300px"})

        Dim _lat As New ColonneNome("Latitudine", Gias.Latitudine & " " & Gias.AppezzamentoAbbr, "number") With {._Display = True, ._width = "150px"}
        _lat._formatNr = "n7"
        l.Add(_lat)
        Dim _long As New ColonneNome("Longitudine", Gias.Longitudine & " " & Gias.AppezzamentoAbbr, "number") With {._Display = True, ._width = "150px"}
        _long._formatNr = "n7"
        l.Add(_long)

        ' ########## ANAGRAFICA IMPIANTO ##########
        l.Add(New ColonneNome("nmPianteTot", Gias.NmPianteTot, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("nmPiante", My.Resources.AgronicaCoreFiltroneBIZ.NumPianteHA, "number") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("nmPianteF", My.Resources.AgronicaCoreFiltroneBIZ.NumPianteF, "number") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("maschiInSesto", My.Resources.AgronicaCoreFiltroneBIZ.MaschiInSesto, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("distanza_tra_fila", Gias.DistanzaTraFila, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("distanza_su_fila", Gias.DistanzaSuFila, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Interbina", Gias.Interbina, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("Germinabilita", Gias.Germinabilita, "string") With {._Display = False, ._width = "150px"})

        l.Add(New ColonneNome("annoInnestoVarieta", My.Resources.AgronicaCoreFiltroneBIZ.DataInnestoVarieta, "date") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("annoInizioPortinnesto", My.Resources.AgronicaCoreFiltroneBIZ.DataInizioPortinnesto, "date") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("annoPrimaProduzione", My.Resources.AgronicaCoreFiltroneBIZ.DataPrimaProduzione, "date") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("Portinnesto", My.Resources.AgronicaCoreFiltroneBIZ.Portinnesto, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("formaAllevamento", My.Resources.AgronicaCoreFiltroneBIZ.FormaAllevamento, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Copertura", My.Resources.AgronicaCoreFiltroneBIZ.Copertura, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("impiantoIrriguo", My.Resources.AgronicaCoreFiltroneBIZ.ImpiantoIrriguo, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("Licenza", "Licenza", "string") With {._Display = False, ._width = "150px"})

        l.Add(New ColonneNome("Metodo_Produzione", Gias.MetodoProduzione, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("GruppoVegetale", Gias.GruppoVegetale, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Finalita", Gias.Finalita, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("DestinazioneUso", Gias.DestinazioneUso, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("DettaglioSpecie", Gias.DettaglioSpeciePersonalizzato, "string") With {._Display = False, ._width = "300px"})
        l.Add(New ColonneNome("GRVA_DES", Gias.GruppoVarietale, "string") With {._width = "200px"})
        l.Add(New ColonneNome("capitolato_privato", Gias.CapitolatoPrivato, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("regolamento_str", Gias.Regolamento, "string") With {._width = "300px"})
        l.Add(New ColonneNome("organismoReferente", Gias.OrganismoReferente, "string") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("Descrizione", Gias.Descrizione, "string") With {._width = "200px"})
        l.Add(New ColonneNome("Lotto", Gias.Lotto, "string") With {._width = "150px"})
        l.Add(New ColonneNome("Utilizzo", My.Resources.AgronicaCoreFiltroneBIZ.UtilizzoTerreno, "string") With {._width = "150px"})
        l.Add(New ColonneNome("Cul_Des", Gias.Varieta, "string") With {._width = "150px"})
        l.Add(New ColonneNome("Sup_Imp", My.Resources.AgronicaCoreFiltroneBIZ.SuperficieImpiantoAbbr & " [ha]", "number") With {._width = "200px"})
        If dt.Columns.Contains("Sup_Imp_acro") Then
            l.Add(New ColonneNome("Sup_Imp_acro", My.Resources.AgronicaCoreFiltroneBIZ.SuperficieImpiantoAbbr & " [ac]", "number") With {._width = "150px"})
        End If
        l.Add(New ColonneNome("Resa", Gias.Resa & " [Kg/ha]", "number") With {._width = "150px", ._formatNr = "n2"})
        l.Add(New ColonneNome("ResaTotalePrevista", Gias.ResaTotalePrevista & " [kg]", "number") With {._width = "150px", ._formatNr = "n2"})

        l.Add(New ColonneNome("DurataImpianto", Gias.DurataImpianto, "string") With {._Display = False, ._width = "250px"})
        l.Add(New ColonneNome("InizioDurataImpianto", Gias.DataInizioImpianto, "date") With {._Display = False, ._width = "300px"})
        l.Add(New ColonneNome("FineDurataImpianto", Gias.DataFineImpianto, "date") With {._Display = False, ._width = "300px"})

        l.Add(New ColonneNome("DataSeminaTrapianto", Gias.DataSeminaTrapianto, "date") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Data_Raccolta_Prevista", Gias.DataRaccoltaPrevista, "date") With {._Display = False, ._width = "300px"})
        l.Add(New ColonneNome("Data_Semina_Prevista", Gias.DataSeminaPrevista, "date") With {._Display = False, ._width = "250px"})
        l.Add(New ColonneNome("Data_Fioritura_Prevista", Gias.DataFiorituraPrevista, "date") With {._Display = False, ._width = "200px"})


        ' ########## ANAGRAFICA ESERCIZIO ##########
        l.Add(New ColonneNome("datastr", My.Resources.AgronicaCoreFiltroneBIZ.DurataEsercizio, "string") With {._width = "150px"})
        l.Add(New ColonneNome("InizioValiditadatastr", Gias.DataInizioEsercizio, "date") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("FineValiditadatastr", Gias.DataFineEsercizio, "date") With {._Display = False, ._width = "150px"})

        l.Add(New ColonneNome("magazzino_conf", Gias.MagazzinoConferimento, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Disciplinare", Gias.Disciplinare, "string") With {._Display = False, ._width = "200px"})

        l.Add(New ColonneNome("DESSTATOIMPIANTO", Gias.StatoImpianto, "string") With {._Display = False, ._width = "150px"})

        l.Add(New ColonneNome("CertificazioneAziendale", Gias.CertificazioneAziendale, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("Contributi", Gias.Contributi, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("CertificazioneProdotto", Gias.CertificazioneProdotto, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("CapitolatoPrivato", Gias.CapitolatoPrivato, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("Residuo", Gias.Residuo, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("TecniciRifImpianto", Gias.TecnicoRifImpianto, "string") With {._Display = False, ._width = "150px"})
        l.Add(New ColonneNome("Gruppi_Raccolta", Gias.GruppoRaccolta & " " & Gias.Esercizio, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Prodotto", Gias.Prodotto, "string") With {._Display = True, ._width = "150px"})


        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = True})
        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._Display = True})

        l.Add(New ColonneNome("Username_Creazione_Completo", Gias.UtenteCreazione, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Modifica_Completo", Gias.UtenteModifica, "string") With {._Display = True, ._width = "150px"})

        If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
            l.Add(New ColonneNome("chiave", My.Resources.AgronicaCoreFiltroneBIZ.DettagliConformita, "string") With {._FormatoParticolare = "<span class='fa fa-info-circle non_conformita' chiave='{0}'></span> ", ._width = "200px"})
        End If

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp

    End Function

    Public Shared Function DT_to_Json_Impianto(
        ByVal dt As DataTable, ByVal JsonRichiesta As filtrone_richiesta_nuovo,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        ByVal ASG_SuperUser_CodFiscale As String,
        ByVal ASG_Utente_Username_Crypt As String,
        ByVal ASG_Utente_Password_Crypt As String
        ) As String
        Dim qs_FiltroAzioni As FiltroAzioni_nuovo = JsonRichiesta.filtroazioni

        dt = RimuoviDoppioniImpianti(dt, JsonRichiesta, objParametri_Server, objParametri_Utenti, ASG_SuperUser_CodFiscale, ASG_Utente_Username_Crypt, ASG_Utente_Password_Crypt)

        Dim check As Boolean = True
        Select Case qs_FiltroAzioni.TipoFiltrone
            Case enum_TipoFiltrone.Utenti,
                 enum_TipoFiltrone.Agenda,
                 enum_TipoFiltrone.MultiModificaAgenda,
                 enum_TipoFiltrone.EliminaInterventi,
                 enum_TipoFiltrone.Blocco_OperazioniAgenda,
                 enum_TipoFiltrone.StimeProduzione
                check = False

            Case enum_TipoFiltrone.Stampa

                'qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.ReportConserveItalia OrElse
                If qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Quadro_P OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.EstrattoreDatiGrafici OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.ReportRisultatoFilrone OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportatore_Universale_Imprese OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportazione_AnagraficaContatti OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportatore_Universale_Rintraccio OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportazione_OP_Gest OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportatore_Universale_Agenda OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportazione_OP_Produttori OrElse
                   qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Esportazione_CellulariContatti OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Adesione_Etico_Ambientale OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Tenuta_Scheda_Campagna OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Codice_Condotta OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Adesione_DPI OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Impegnativa_Eurep OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Impegnativa_QC OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Impegnativa_Confusione_Sessuale OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Mandato_Trasmissione_Telematica_Dati OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Adesione_Conad OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Adesione_Despar OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Accordo_Responsabilita_di_Filiera OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Fitoregolatori_Kiwi OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.Dichiarazione_di_Responsabilita OrElse
                    qs_FiltroAzioni.CodificaStampe = enum_CodificaStampe.QuestionarioValutazioneAzienda_Aggiornamento Then

                    check = False
                End If

            Case enum_TipoFiltrone.Esportatore_Universale_Imprese,
                 enum_TipoFiltrone.Esportatore_Universale_Agenda,
                 enum_TipoFiltrone.Esportatore_Contatti,
                 enum_TipoFiltrone.Esportatore_Universale_Centri,
                 enum_TipoFiltrone.Esportatore_Universale_Appezza,
                 enum_TipoFiltrone.Esportatore_Universale_Agenda,
                 enum_TipoFiltrone.Esportatore_Universale_Rintraccio,
                 enum_TipoFiltrone.Esportazione_OP_Gest,
                 enum_TipoFiltrone.Esportazione_CellulariTecnici
                check = False

            Case enum_TipoFiltrone.PianoConcimazione

        End Select

        If Not String.IsNullOrEmpty(JsonRichiesta.filtroazioni.G2G_Cod) AndAlso Integer.Parse(JsonRichiesta.filtroazioni.G2G_Cod) > 0 Then
            check = True
        End If

        If qs_FiltroAzioni.Filtrino = "1" Then
            check = False
        End If

        If qs_FiltroAzioni.Filtrino = "10" Then ' usato per disattivare il check in alcune stampe
            check = False
        End If

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        If check Then
            l.Add(New ColonneNome("chiave", "seleziona", "string") With {._hidden = True})
        End If

        l.Add(New ColonneNome("Referente", My.Resources.AgronicaCoreFiltroneBIZ.ImpresaReferente, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("PIVA_padre", Gias.PIVAImpresaReferente, "string") With {._Display = False, ._width = "300px"})

        l.Add(New ColonneNome("Rag_Soc", Gias.RagioneSociale, "string") With {._FiltrabileConCheck = True, ._width = "200px"})
        l.Add(New ColonneNome("CodiceCuaa", Gias.CodiceUnicoAziendaAgricolaSigla, "string") With {._width = "200px"})
        l.Add(New ColonneNome("TecnicoRifImpresa", Gias.TecnicoRifImpresa, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Gruppi_Raccolta", Gias.GruppoRaccolta, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("app_nome", Gias.AppezzamentoAbbr, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Sup_Imp", Gias.SuperficieAbbr & " [ha]", "number") With {._Display = True, ._width = "150px"})
        If dt.Columns.Contains("acro") Then
            l.Add(New ColonneNome("acro", Gias.SuperficieAbbr & " [ac]", "number") With {._width = "150px"})
        End If
        l.Add(New ColonneNome("Utilizzo", My.Resources.AgronicaCoreFiltroneBIZ.UtilizzoTerreno, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Cul_Des", Gias.Varieta, "string") With {._Display = True, ._width = "200px"})

        l.Add(New ColonneNome("datastr", My.Resources.AgronicaCoreFiltroneBIZ.DurataImpianto, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("GRVA_DES", Gias.GruppoVarietale, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Campo_Des", Gias.Campo, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("regolamento_str", My.Resources.AgronicaCoreFiltroneBIZ.RegolamentoAbbr, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Data_Fine_Prevista", Gias.DataRaccoltaPrevista, "date") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("chiuso", My.Resources.AgronicaCoreFiltroneBIZ.Chiuso, "string") With {._FormatoParticolare = "<span class='chiuso chiuso_{0}'></span>", ._width = "150px"})

        l.Add(New ColonneNome("piva", Gias.Piva, "string") With {._Display = False, ._FiltrabileConCheck = True, ._width = "150px"})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True, ._width = "150px"})
        l.Add(New ColonneNome("campo_cod", "campo_cod", "string") With {._hidden = True, ._width = "150px"})
        l.Add(New ColonneNome("appezza", "appezza", "string") With {._hidden = True, ._width = "150px"})
        l.Add(New ColonneNome("id_reg", "id_reg", "string") With {._hidden = True, ._width = "150px"})
        l.Add(New ColonneNome("veg_cod", "veg_cod", "string") With {._hidden = True, ._width = "150px"})
        l.Add(New ColonneNome("cul_cod", "cul_cod", "string") With {._hidden = True, ._width = "150px"})

        ' VAnni: 11/2/2020: ora è esteso attraverso permesso utente, non più personalizzato su Riunite.
        'Grilli: Modifica volante per Valerio che probabilmente resterà qui per sempre... (RiuniteCiv)
        'If objParametri_Server.PivaSuperUser = "00127310357" And JsonRichiesta.filtroazioni.IncludiVisite Then
        If JsonRichiesta.filtroazioni.IncludiVisite Then
            l.Add(New ColonneNome("dataVisita", My.Resources.AgronicaCoreFiltroneBIZ.DataVisita, "date") With {._hidden = True, ._width = "150px"})
            l.Add(New ColonneNome("punteggioVisita", My.Resources.AgronicaCoreFiltroneBIZ.Punteggio, "integer") With {._hidden = True, ._width = "150px"})
        End If
        l.Add(New ColonneNome("fabbricato_Des", My.Resources.AgronicaCoreFiltroneBIZ.MagazzinoConferimentoAbbr, "string") With {._hidden = True, ._width = "150px"})
        Dim col = New ColonneNome("Centro_Conferimento", My.Resources.AgronicaCoreFiltroneBIZ.CentroConferimento, "string")
        col._Display = False
        col._width = "200px"
        l.Add(col)

        Dim _lat As New ColonneNome("Latitudine", Gias.Latitudine, "number") With {._Display = True, ._width = "150px"}
        _lat._formatNr = "n7"
        l.Add(_lat)
        Dim _long As New ColonneNome("Longitudine", Gias.Longitudine, "number") With {._Display = True, ._width = "150px"}
        _long._formatNr = "n7"
        l.Add(_long)

        If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
            l.Add(New ColonneNome("chiave", My.Resources.AgronicaCoreFiltroneBIZ.DettagliConformita, "string") With {._FormatoParticolare = "<span class='fa fa-info-circle non_conformita' chiave='{0}'></span> ", ._width = "150px"})
        End If

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Username_Creazione_Completo", Gias.UtenteCreazione, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Modifica_Completo", Gias.UtenteModifica, "string") With {._Display = True, ._width = "150px"})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp

    End Function

    Public Shared Function DT_to_Json_Movimento(ByVal dt As DataTable, ByVal JsonRichiesta As filtrone_richiesta_nuovo, ByVal objParametri_Server As AgronicaCoreParametri) As String
        Dim qs_FiltroAzioni As FiltroAzioni_nuovo = JsonRichiesta.filtroazioni

        Dim check As Boolean = False
        Select Case qs_FiltroAzioni.TipoFiltrone
            Case enum_TipoFiltrone.Esportatore_Universale_Agenda,
                 enum_TipoFiltrone.Agenda,
                 enum_TipoFiltrone.MultiModificaAgenda,
                 enum_TipoFiltrone.EliminaInterventi,
                 enum_TipoFiltrone.Blocco_OperazioniAgenda
                check = True
            Case enum_TipoFiltrone.Stampa
                Select Case qs_FiltroAzioni.CodificaStampe
                    Case enum_CodificaStampe.Esportatore_Universale_Agenda,
                         enum_CodificaStampe.Bilancio_Fertilizzazioni,
                         enum_CodificaStampe.Bilancio_Fertilizzazioni_Dettagliato
                        check = True
                End Select
            Case enum_TipoFiltrone.OperazioniMultiAziendali
                check = True
            Case Else
                check = False
        End Select

        Dim majorSql As Integer = DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri_Server)
        Dim sql2017OrHigher = False
        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017OrHigher = True
        End If

        Dim mapCoopReferente As New Dictionary(Of String, String)
        For Each row In dt.Rows
            GetImpreseReferenti(dt, row, mapCoopReferente, dt.Rows.IndexOf(row), sql2017OrHigher, objParametri_Server)
        Next
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        If check Then
            l.Add(New ColonneNome("chiave", "seleziona", "string") With {._hidden = True})
        End If

        l.Add(New ColonneNome("Referente", My.Resources.AgronicaCoreFiltroneBIZ.ImpresaReferente, "string") With {._Display = True, ._width = "300px"})
        l.Add(New ColonneNome("PIVA_padre", Gias.PIVAImpresaReferente, "string") With {._Display = False, ._width = "300px"})

        l.Add(New ColonneNome("Rag_Soc", Gias.RagioneSociale, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("CodiceCuaa", Gias.CodiceUnicoAziendaAgricolaSigla, "string") With {._width = "150px"})
        l.Add(New ColonneNome("TecnicoRifImpresa", Gias.TecnicoRifImpresa, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Gruppi_Raccolta", Gias.GruppoRaccolta, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("PIVA", (Gias.Piva).ToUpper, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Data_Movimento", Gias.DataMovimentoAbbr, "date") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Des_Lib", Gias.Descrizione, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("APP_NOME", Gias.Appezzamento, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Lav_des", My.Resources.AgronicaCoreFiltroneBIZ.TipoOperazione, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Veg_Des", Gias.Specie, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Cul_Des", Gias.Varieta, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._Display = True, ._width = "200px"})

        l.Add(New ColonneNome("Username_Creazione_Agenda_Completo", Gias.UtenteCreazione & " (" & Gias.OperazioniCampagna & ")", "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Modifica_Agenda_Completo", Gias.UtenteModifica & " (" & Gias.OperazioniCampagna & ")", "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Creazione_Movimenti_Completo", Gias.UtenteCreazione & " (" & Gias.MovimentoMagazzino & ")", "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Modifica_Movimenti_Completo", Gias.UtenteModifica & " (" & Gias.MovimentoMagazzino & ")", "string") With {._Display = True, ._width = "150px"})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp

    End Function

    'Anna 16/08/21 - Aggiunta scheda fabbricati nel filtrone, per TTI
    Public Shared Function DT_to_Json_Fabbricato(ByVal dt As DataTable,
                                                 ByVal JsonRichiesta As filtrone_richiesta_nuovo,
                                                 ByVal objParametri_Server As AgronicaCoreParametri
                                                 ) As String

        Dim check As Boolean = False

        Dim majorSql As Integer = DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri_Server)
        Dim sql2017OrHigher = False
        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017OrHigher = True
        End If

        Dim mapCoopReferente As New Dictionary(Of String, String)
        For Each row In dt.Rows
            GetImpreseReferenti(dt, row, mapCoopReferente, dt.Rows.IndexOf(row), sql2017OrHigher, objParametri_Server)
        Next

        Dim l As New List(Of ColonneNome)

        If check Then
            l.Add(New ColonneNome("chiave", "seleziona", "string") With {._hidden = True})
        End If

        l.Add(New ColonneNome("PIVA", (Gias.Piva).ToUpper, "string") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Rag_Soc", Gias.RagioneSociale, "string") With {._FiltrabileConCheck = True, ._width = "200px"})

        l.Add(New ColonneNome("CodiceCuaa", Gias.CodiceUnicoAziendaAgricolaSigla, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("Gruppi_Raccolta", Gias.GruppoRaccolta, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("Referente", My.Resources.AgronicaCoreFiltroneBIZ.ImpresaReferente, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("PIVA_padre", Gias.PIVAImpresaReferente, "string") With {._Display = False, ._width = "300px"})
        l.Add(New ColonneNome("TecnicoRifImpresa", Gias.TecnicoRifImpresa, "string") With {._Display = True, ._width = "150px"})

        l.Add(New ColonneNome("CodiceSocio", Gias.CodiceSocio, "string") With {._Display = True, ._width = "200px"})

        l.Add(New ColonneNome("Denominazione", Gias.Denominazione, "string") With {._Display = True, ._width = "200px"})
        ' TIPOLOGIA QUI
        l.Add(New ColonneNome("CombustibileForno", Gias.CombustibileForno, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("FiammaForno", Gias.FiammaForno, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("CantiereForno", Gias.CantiereForno, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("UmidificazioneForno", Gias.UmidificazioneForno, "string") With {._Display = True, ._width = "200px"})
        l.Add(New ColonneNome("TipoForno", Gias.TipoForno, "string") With {._Display = True, ._width = "200px"})

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Data_Modifica", Gias.DataModifica, "date") With {._Display = False, ._width = "200px"})
        l.Add(New ColonneNome("Username_Creazione_Completo", Gias.UsernameCreazione, "string") With {._Display = True, ._width = "150px"})
        l.Add(New ColonneNome("Username_Modifica_Completo", Gias.UsernameModifica, "string") With {._Display = True, ._width = "150px"})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, ImpostaFiltroColonneStdGIAS:=True)
        Return risp
    End Function

    Private Shared Function RimuoviDoppioniImpianti(ByVal dt As DataTable,
                                                    ByVal JsonRichiesta As filtrone_richiesta_nuovo,
                                                    ByVal objParametri_Server As AgronicaCoreParametri,
                                                    ByVal objParametri_Utenti As AgronicaCoreParametri,
                                                    ByVal ASG_SuperUser_CodFiscale As String,
                                                    ByVal ASG_Utente_Username_Crypt As String,
                                                    ByVal ASG_Utente_Password_Crypt As String
                                                    ) As DataTable

        'rimuovo i doppioni
        Dim Validita As String = ""
        Dim Num_Impianti As Integer
        Dim Sup_Totale As Double
        Dim i As Integer
        Dim Coltura As String = ""

        Dim Dummy_Conformita As String
        Dim bOkConformita As Boolean
        Dim nNonConformita As Integer
        Dim bOk As Boolean
        Dim strRisultato_Conformita As String
        Dim strRisultato As String
        Dim Disciplinare_Cod As Integer
        Dim Disciplinare_PubblicoPrivato As Integer

        Dim strIntestazione As String = ""

        Dim LastPiva As String = ""
        Dim LastSa_Cod As Integer = 0
        Dim LastAppezza As Integer = 0
        Dim LastId_Reg As Integer = 0

        Dim Impianto_Duplicato As Boolean = False


        Dim objRegolamentoR As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim dt_regolamenti As DataTable = objRegolamentoR.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        dt.Columns.Add("piano_semina", GetType(String))
        dt.Columns.Add("selezione", GetType(Integer))
        dt.Columns.Add("disciplinare", GetType(String))
        dt.Columns.Add("datastr", GetType(String))
        dt.Columns.Add("regolamento_str", GetType(String))
        dt.Columns.Add("chiuso", GetType(String))

        Num_Impianti = dt.Rows.Count

        Dim mapCoopReferente As New Dictionary(Of String, String)

        Dim sql2017OrHigher As Boolean = True
        Dim majorSql As Integer = DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri_Server)

        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017OrHigher = True
        Else
            sql2017OrHigher = False
        End If

        Dim isFirst As Boolean = True

        For i = Num_Impianti - 1 To 0 Step -1

            Dim eliminato As Boolean
            eliminato = False


            If Not IsDBNull(dt.Rows(i).Item("appezza")) AndAlso Not IsDBNull(dt.Rows(i).Item("id_reg")) Then

                '==================================================================
                'Controllo Duplicati di Impianti in griglia
                '(dovuti alle diverse distinte)
                '------------------------------------------------------------------
                Impianto_Duplicato = False
                If LastPiva = dt.Rows(i).Item("piva") AndAlso
                    LastSa_Cod = dt.Rows(i).Item("sa_cod") AndAlso
                    LastAppezza = dt.Rows(i).Item("appezza") AndAlso
                    LastId_Reg = dt.Rows(i).Item("id_reg") Then

                    Impianto_Duplicato = True
                End If

                If Not Impianto_Duplicato Then

                    strRisultato = ""
                    strIntestazione = ""
                    strRisultato_Conformita = ""

                    '-------------------------------
                    'CONTROLLO CONFORMITA
                    '-------------------------------

                    If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then

                        If Not IsDBNull(dt.Rows(i).Item("Regolamento")) AndAlso
                            Not IsDBNull(dt.Rows(i).Item("Finanziamento")) AndAlso
                            Not IsDBNull(dt.Rows(i).Item("Veg_Cod")) Then

                            If dt.Rows(i).Item("Finanziamento") <> 0 Then

                                Dim objDPILeggi As New AgronicaCoreDpiBIZ.DPI_Leggi
                                dt.Rows(i).Item("Disciplinare") = objDPILeggi.Nome_Disciplinare(
                                    CInt(dt.Rows(i).Item("Regolamento")),
                                    CInt(dt.Rows(i).Item("Finanziamento")),
                                    CInt(dt.Rows(i).Item("Veg_Cod")),
                                    0,
                                    0,
                                    objParametri_Utenti,
                                    ASG_Utente_Username_Crypt,
                                    ASG_Utente_Password_Crypt)
                            Else
                                dt.Rows(i).Item("Disciplinare") = "Nessuno"
                            End If

                        End If

                        Validita = If(CDate(dt.Rows(i).Item("validita_inizio")).ToShortDateString <> "01/01/1900", CDate(dt.Rows(i).Item("validita_inizio")).ToShortDateString, "...") & "-" & If(CDate(dt.Rows(i).Item("validita_fine")).ToShortDateString <> "31/12/2100", CDate(dt.Rows(i).Item("validita_fine")).ToShortDateString, "...")
                        Coltura = dt.Rows(i).Item("veg_des") & " - " & dt.Rows(i).Item("cul_des")

                        bOkConformita = True

                        nNonConformita = 0

                        '##################################################################################################
                        '############## Controllo Conformita Piano Concimazione ###########################################
                        '##################################################################################################

                        If CInt(JsonRichiesta.filtrone_impianto.Cmb_PianoConcimazione) > 0 Then
                            Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica
                            Dummy_Conformita = objDpiVerifica.DPI_Verifica_Piano_Concimazione(objParametri_Server, objParametri_Utenti,
                                                                               ASG_SuperUser_CodFiscale,
                                                                               CStr(dt.Rows(i).Item("piva")),
                                                                               CInt(dt.Rows(i).Item("sa_cod")),
                                                                               CInt(dt.Rows(i).Item("appezza")),
                                                                               CInt(dt.Rows(i).Item("id_reg")),
                                                                               False)

                            If CInt(JsonRichiesta.filtrone_impianto.Cmb_PianoConcimazione) = 1 Then
                                'Solo Impianti Conformi
                                bOkConformita = If(CInt(Dummy_Conformita) = 0, True, False)
                            Else
                                'Solo Impianti Non Conformi
                                bOkConformita = If(CInt(Dummy_Conformita) = 1, True, False)
                            End If

                        End If


                        '##################################################################################################
                        '#################### Controllo Conformita Diserbo     ############################################
                        '##################################################################################################

                        If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
                            'bOk = If(nNonConformita = 0, True, False)
                            bOk = True
                        Else
                            bOk = If(CInt(JsonRichiesta.filtrone_impianto.Cmb_Diserbo) > 0, True, False)
                        End If

                        If bOk AndAlso bOkConformita Then
                            Dim objDPIVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica

                            If JsonRichiesta.filtrone_impianto.Cmb_Disciplinare <> "" Then

                                Disciplinare_Cod = Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/")(0)
                                Disciplinare_PubblicoPrivato = 0
                                If Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/").Length > 0 Then
                                    Disciplinare_PubblicoPrivato = Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/")(1)
                                End If

                                Dim r As New rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Impianto)
                                r = objDPIVerifica.Verifica_Conformita_Impianto_New(objParametri_Server,
                                                            objParametri_Utenti,
                                                            CStr(dt.Rows(i).Item("piva")),
                                                            CInt(dt.Rows(i).Item("sa_cod")),
                                                            CInt(dt.Rows(i).Item("appezza")),
                                                            CInt(dt.Rows(i).Item("id_reg")),
                                                            Disciplinare_Cod, Disciplinare_PubblicoPrivato, 1,
                                                            True,
                                                            CDate(dt.Rows(i).Item("Progetto_Validita_Inizio")),
                                                            CDate(dt.Rows(i).Item("Progetto_Validita_Fine")))

                                If r.RispostaOK Then
                                    Select Case r.RispostaStringa.Conforme
                                        Case True
                                            Dummy_Conformita = "0"
                                        Case False
                                            Dummy_Conformita = "1"
                                    End Select
                                End If

                            End If

                            If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
                                nNonConformita = nNonConformita + CInt(Dummy_Conformita)
                            Else
                                If CInt(JsonRichiesta.filtrone_impianto.Cmb_Diserbo) = 1 Then
                                    'Solo Impianti Conformi
                                    bOkConformita = If(CInt(Dummy_Conformita) = 0, True, False)
                                Else
                                    'Solo Impianti Non Conformi
                                    bOkConformita = If(CInt(Dummy_Conformita) = 1, True, False)
                                End If
                            End If

                        End If


                        '##################################################################################################
                        '#################### Controllo Conformita Difesa     #############################################
                        '##################################################################################################

                        If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
                            bOk = True
                        Else
                            bOk = If(CInt(JsonRichiesta.filtrone_impianto.Cmb_Difesa) > 0, True, False)
                        End If

                        If bOk AndAlso bOkConformita Then

                            Dim objDPIVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica

                            If JsonRichiesta.filtrone_impianto.Cmb_Disciplinare <> "" Then

                                Disciplinare_Cod = Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/")(0)
                                Disciplinare_PubblicoPrivato = 0
                                If Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/").Length > 0 Then
                                    Disciplinare_PubblicoPrivato = Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/")(1)
                                End If

                                Dim r As New rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Impianto)
                                r = objDPIVerifica.Verifica_Conformita_Impianto_New(objParametri_Server,
                                                            objParametri_Utenti,
                                                            CStr(dt.Rows(i).Item("piva")),
                                                            CInt(dt.Rows(i).Item("sa_cod")),
                                                            CInt(dt.Rows(i).Item("appezza")),
                                                            CInt(dt.Rows(i).Item("id_reg")),
                                                            Disciplinare_Cod, Disciplinare_PubblicoPrivato, 0,
                                                            True,
                                                            CDate(dt.Rows(i).Item("Progetto_Validita_Inizio")),
                                                            CDate(dt.Rows(i).Item("Progetto_Validita_Fine")))

                                If r.RispostaOK Then
                                    Select Case r.RispostaStringa.Conforme
                                        Case True
                                            Dummy_Conformita = "0"
                                        Case False
                                            Dummy_Conformita = "1"
                                    End Select
                                End If

                            End If


                            If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
                                nNonConformita = nNonConformita + CInt(Dummy_Conformita)
                                bOkConformita = If(nNonConformita > 0, True, False)
                            Else
                                If CInt(JsonRichiesta.filtrone_impianto.Cmb_Difesa) = 1 Then
                                    'Solo Impianti Conformi
                                    bOkConformita = If(CInt(Dummy_Conformita) = 0, True, False)
                                Else
                                    'Solo Impianti Non Conformi
                                    bOkConformita = If(CInt(Dummy_Conformita) = 1, True, False)
                                End If
                            End If

                        End If


                        If bOkConformita Then

                            Sup_Totale += CDbl(dt.Rows(i).Item("sup_imp"))

                            dt.Rows(i).Item("selezione") = 0
                            dt.Rows(i).Item("piano_semina") = ""

                            If IsDBNull(dt.Rows(i).Item("id_reg")) Then
                                dt.Rows(i).Delete()
                                eliminato = True
                            Else
                                LastPiva = dt.Rows(i).Item("piva")
                                LastSa_Cod = dt.Rows(i).Item("sa_cod")
                                LastAppezza = dt.Rows(i).Item("appezza")
                                LastId_Reg = dt.Rows(i).Item("id_reg")
                            End If

                        Else

                            LastPiva = dt.Rows(i).Item("piva")
                            LastSa_Cod = dt.Rows(i).Item("sa_cod")
                            LastAppezza = dt.Rows(i).Item("appezza")
                            LastId_Reg = dt.Rows(i).Item("id_reg")

                            dt.Rows(i).Delete()
                            eliminato = True
                        End If

                    Else

                        Sup_Totale += CDbl(dt.Rows(i).Item("sup_imp"))

                        dt.Rows(i).Item("selezione") = 0
                        dt.Rows(i).Item("piano_semina") = ""

                        If IsDBNull(dt.Rows(i).Item("id_reg")) Then
                            dt.Rows(i).Delete()
                            eliminato = True
                        Else
                            LastPiva = dt.Rows(i).Item("piva")
                            LastSa_Cod = dt.Rows(i).Item("sa_cod")
                            LastAppezza = dt.Rows(i).Item("appezza")
                            LastId_Reg = dt.Rows(i).Item("id_reg")
                        End If

                    End If

                    GetImpreseReferenti(dt, dt.Rows(i), mapCoopReferente, If(isFirst, 0, 1), sql2017OrHigher, objParametri_Server)
                    isFirst = False

                    If Not eliminato Then

                        Dim datastr As String =
                                If(CDate(dt.Rows(i).Item("Validita_Inizio")).ToShortDateString <> "01/01/1900", CDate(dt.Rows(i).Item("Validita_Inizio")).ToShortDateString, "...") &
                                 "-" & If(CDate(dt.Rows(i).Item("Validita_Fine")).ToShortDateString <> "31/12/2100", CDate(dt.Rows(i).Item("Validita_Fine")).ToShortDateString, "...")

                        dt.Rows(i).Item("datastr") = datastr

                        Dim dr_reg As DataRow()
                        If Not IsDBNull(dt.Rows(i).Item("regolamento")) Then
                            dr_reg = dt_regolamenti.Select("Reg_Cod= " & dt.Rows(i).Item("regolamento"))
                            If dr_reg IsNot Nothing AndAlso dr_reg.Count > 0 Then
                                dt.Rows(i).Item("regolamento_str") = dr_reg(0).Item("reg_des")
                            Else
                                dt.Rows(i).Item("regolamento_str") = ""
                            End If
                        Else
                            dt.Rows(i).Item("regolamento_str") = ""
                        End If

                        dt.Rows(i).Item("chiuso") = ""
                        If CDate(dt.Rows(i).Item("Validita_Fine")) <= Date.Today Then
                            dt.Rows(i).Item("chiuso") = "true"
                        End If
                    End If
                Else

                    'se l'impianto è duplicato 
                    '(potrebbe x esempio essere presente 2volte 1impianto con 2progetti associati)
                    'lo elimino
                    dt.Rows(i).Delete()
                    eliminato = True
                End If 'controllo duplicato

            Else

                'se appezza o id_reg sono = NULL elimino il record
                dt.Rows(i).Delete()
                eliminato = True
            End If  'controllo id_reg appezza = NULL


        Next
        dt.AcceptChanges()

        Return dt
    End Function

    Private Shared Function DistintaPredisponiDatiMancanti(
        ByVal dt As DataTable,
        ByVal JsonRichiesta As filtrone_richiesta_nuovo,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        ByVal ASG_SuperUser_CodFiscale As String,
        ByVal ASG_Utente_Username_Crypt As String,
        ByVal ASG_Utente_Password_Crypt As String
    ) As DataTable

        'rimuovo i doppioni
        Dim Validita As String = ""
        Dim Num_Impianti As Integer
        Dim Sup_Totale As Double
        Dim i As Integer
        Dim Coltura As String = ""

        Dim Dummy_Conformita As String
        Dim bOkConformita As Boolean
        Dim nNonConformita As Integer
        Dim bOk As Boolean
        Dim strRisultato_Conformita As String
        Dim strRisultato As String
        Dim Disciplinare_Cod As Integer
        Dim Disciplinare_PubblicoPrivato As Integer

        Dim strIntestazione As String = ""

        Dim LastPiva As String = ""
        Dim LastSa_Cod As Integer = 0
        Dim LastAppezza As Integer = 0
        Dim LastId_Reg As Integer = 0

        Dim objRegolamentoR As New AgronicaCoreMetaSchemaDAL.Regolamenti_R
        Dim dt_regolamenti As DataTable = objRegolamentoR.Leggi(0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        dt.Columns.Add("piano_semina", GetType(String))
        dt.Columns.Add("selezione", GetType(Integer))
        dt.Columns.Add("disciplinare", GetType(String))
        dt.Columns.Add("datastr", GetType(String))
        dt.Columns.Add("regolamento_str", GetType(String))
        dt.Columns.Add("chiuso", GetType(String))
        dt.Columns.Add("InizioValiditadatastr", GetType(String))
        dt.Columns.Add("FineValiditadatastr", GetType(String))

        Num_Impianti = dt.Rows.Count

        Dim mapCoopReferente As New Dictionary(Of String, String)

        Dim sql2017OrHigher As Boolean = True
        Dim majorSql As Integer = DataProviderFactory.Instance.Provider().VersioneSqlServer_Major(objParametri_Server)

        If majorSql >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017OrHigher = True
        Else
            sql2017OrHigher = False
        End If

        Dim isFirst As Boolean = True

        For i = Num_Impianti - 1 To 0 Step -1

            Dim eliminato As Boolean
            eliminato = False


            If Not IsDBNull(dt.Rows(i).Item("appezza")) AndAlso Not IsDBNull(dt.Rows(i).Item("id_reg")) Then


                strRisultato = ""
                strIntestazione = ""
                strRisultato_Conformita = ""


                '-------------------------------
                'CONTROLLO CONFORMITA
                '-------------------------------

                If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then

                    If Not IsDBNull(dt.Rows(i).Item("Regolamento")) AndAlso
                        Not IsDBNull(dt.Rows(i).Item("Finanziamento")) AndAlso
                        Not IsDBNull(dt.Rows(i).Item("Veg_Cod")) Then

                        If dt.Rows(i).Item("Finanziamento") <> 0 Then

                            Dim objDPILeggi As New AgronicaCoreDpiBIZ.DPI_Leggi
                            dt.Rows(i).Item("Disciplinare") = objDPILeggi.Nome_Disciplinare(
                                CInt(dt.Rows(i).Item("Regolamento")),
                                CInt(dt.Rows(i).Item("Finanziamento")),
                                CInt(dt.Rows(i).Item("Veg_Cod")),
                                0,
                                0,
                                objParametri_Utenti,
                                ASG_Utente_Username_Crypt,
                                ASG_Utente_Password_Crypt)
                        Else
                            dt.Rows(i).Item("Disciplinare") = "Nessuno"
                        End If

                    End If

                    Validita = If(CDate(dt.Rows(i).Item("validita_inizio")).ToShortDateString <> "01/01/1900", CDate(dt.Rows(i).Item("validita_inizio")).ToShortDateString, "...") & "-" & If(CDate(dt.Rows(i).Item("validita_fine")).ToShortDateString <> "31/12/2100", CDate(dt.Rows(i).Item("validita_fine")).ToShortDateString, "...")
                    Coltura = dt.Rows(i).Item("veg_des") & " - " & dt.Rows(i).Item("cul_des")

                    bOkConformita = True

                    nNonConformita = 0

                    '##################################################################################################
                    '############## Controllo Conformita Piano Concimazione ###########################################
                    '##################################################################################################

                    If CInt(JsonRichiesta.filtrone_impianto.Cmb_PianoConcimazione) > 0 Then
                        Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica
                        Dummy_Conformita = objDpiVerifica.DPI_Verifica_Piano_Concimazione(objParametri_Server, objParametri_Utenti,
                                                                           ASG_SuperUser_CodFiscale,
                                                                           CStr(dt.Rows(i).Item("piva")),
                                                                           CInt(dt.Rows(i).Item("sa_cod")),
                                                                           CInt(dt.Rows(i).Item("appezza")),
                                                                           CInt(dt.Rows(i).Item("id_reg")),
                                                                           False)

                        If CInt(JsonRichiesta.filtrone_impianto.Cmb_PianoConcimazione) = 1 Then
                            'Solo Impianti Conformi
                            bOkConformita = If(CInt(Dummy_Conformita) = 0, True, False)
                        Else
                            'Solo Impianti Non Conformi
                            bOkConformita = If(CInt(Dummy_Conformita) = 1, True, False)
                        End If

                    End If


                    '##################################################################################################
                    '#################### Controllo Conformita Diserbo     ############################################
                    '##################################################################################################

                    If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
                        bOk = True
                    Else
                        bOk = If(CInt(JsonRichiesta.filtrone_impianto.Cmb_Diserbo) > 0, True, False)
                    End If

                    If bOk AndAlso bOkConformita Then
                        Dim objDPIVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica

                        If JsonRichiesta.filtrone_impianto.Cmb_Disciplinare <> "" Then

                            Disciplinare_Cod = Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/")(0)
                            Disciplinare_PubblicoPrivato = 0
                            If Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/").Length > 0 Then
                                Disciplinare_PubblicoPrivato = Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/")(1)
                            End If

                            Dim r As New rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Impianto)
                            r = objDPIVerifica.Verifica_Conformita_Impianto_New(objParametri_Server,
                                                        objParametri_Utenti,
                                                        CStr(dt.Rows(i).Item("piva")),
                                                        CInt(dt.Rows(i).Item("sa_cod")),
                                                        CInt(dt.Rows(i).Item("appezza")),
                                                        CInt(dt.Rows(i).Item("id_reg")),
                                                        Disciplinare_Cod, Disciplinare_PubblicoPrivato, 1,
                                                        True,
                                                        CDate(dt.Rows(i).Item("Progetto_Validita_Inizio")),
                                                        CDate(dt.Rows(i).Item("Progetto_Validita_Fine")))

                            If r.RispostaOK Then
                                Select Case r.RispostaStringa.Conforme
                                    Case True
                                        Dummy_Conformita = "0"
                                    Case False
                                        Dummy_Conformita = "1"
                                End Select
                            End If

                        End If

                        If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
                            nNonConformita = nNonConformita + CInt(Dummy_Conformita)
                        Else
                            If CInt(JsonRichiesta.filtrone_impianto.Cmb_Diserbo) = 1 Then
                                'Solo Impianti Conformi
                                bOkConformita = If(CInt(Dummy_Conformita) = 0, True, False)
                            Else
                                'Solo Impianti Non Conformi
                                bOkConformita = If(CInt(Dummy_Conformita) = 1, True, False)
                            End If
                        End If

                    End If


                    '##################################################################################################
                    '#################### Controllo Conformita Difesa     #############################################
                    '##################################################################################################

                    If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
                        'bOk = If(nNonConformita = 0, True, False)
                        bOk = True
                    Else
                        bOk = If(CInt(JsonRichiesta.filtrone_impianto.Cmb_Difesa) > 0, True, False)
                    End If

                    If bOk AndAlso bOkConformita Then

                        Dim objDPIVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica

                        If JsonRichiesta.filtrone_impianto.Cmb_Disciplinare <> "" Then

                            Disciplinare_Cod = Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/")(0)
                            Disciplinare_PubblicoPrivato = 0
                            If Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/").Length > 0 Then
                                Disciplinare_PubblicoPrivato = Split(JsonRichiesta.filtrone_impianto.Cmb_Disciplinare, "/")(1)
                            End If

                            Dim r As New rispostaStandard(Of AgronicaCoreDpiBIZ.Verifica_Disciplinare_Impianto)
                            r = objDPIVerifica.Verifica_Conformita_Impianto_New(objParametri_Server,
                                                        objParametri_Utenti,
                                                        CStr(dt.Rows(i).Item("piva")),
                                                        CInt(dt.Rows(i).Item("sa_cod")),
                                                        CInt(dt.Rows(i).Item("appezza")),
                                                        CInt(dt.Rows(i).Item("id_reg")),
                                                        Disciplinare_Cod, Disciplinare_PubblicoPrivato, 0,
                                                        True,
                                                        CDate(dt.Rows(i).Item("Progetto_Validita_Inizio")),
                                                        CDate(dt.Rows(i).Item("Progetto_Validita_Fine")))

                            If r.RispostaOK Then
                                Select Case r.RispostaStringa.Conforme
                                    Case True
                                        Dummy_Conformita = "0"
                                    Case False
                                        Dummy_Conformita = "1"
                                End Select
                            End If

                        End If

                        If JsonRichiesta.filtrone_impianto.CkB_AttivaConformita Then
                            nNonConformita = nNonConformita + CInt(Dummy_Conformita)
                            bOkConformita = If(nNonConformita > 0, True, False)
                        Else
                            If CInt(JsonRichiesta.filtrone_impianto.Cmb_Difesa) = 1 Then
                                'Solo Impianti Conformi
                                bOkConformita = If(CInt(Dummy_Conformita) = 0, True, False)
                            Else
                                'Solo Impianti Non Conformi
                                bOkConformita = If(CInt(Dummy_Conformita) = 1, True, False)
                            End If
                        End If

                    End If


                    If bOkConformita Then

                        Sup_Totale += CDbl(dt.Rows(i).Item("sup_imp"))

                        dt.Rows(i).Item("selezione") = 0
                        dt.Rows(i).Item("piano_semina") = ""

                        If IsDBNull(dt.Rows(i).Item("id_reg")) Then
                            dt.Rows(i).Delete()
                            eliminato = True
                        Else
                            LastPiva = dt.Rows(i).Item("piva")
                            LastSa_Cod = dt.Rows(i).Item("sa_cod")
                            LastAppezza = dt.Rows(i).Item("appezza")
                            LastId_Reg = dt.Rows(i).Item("id_reg")
                        End If

                    Else

                        LastPiva = dt.Rows(i).Item("piva")
                        LastSa_Cod = dt.Rows(i).Item("sa_cod")
                        LastAppezza = dt.Rows(i).Item("appezza")
                        LastId_Reg = dt.Rows(i).Item("id_reg")

                        dt.Rows(i).Delete()
                        eliminato = True
                    End If

                Else

                    Sup_Totale += CDbl(dt.Rows(i).Item("sup_imp"))

                    dt.Rows(i).Item("selezione") = 0
                    dt.Rows(i).Item("piano_semina") = ""

                    If IsDBNull(dt.Rows(i).Item("id_reg")) Then
                        dt.Rows(i).Delete()
                        eliminato = True
                    Else
                        LastPiva = dt.Rows(i).Item("piva")
                        LastSa_Cod = dt.Rows(i).Item("sa_cod")
                        LastAppezza = dt.Rows(i).Item("appezza")
                        LastId_Reg = dt.Rows(i).Item("id_reg")
                    End If

                End If

                GetImpreseReferenti(dt, dt.Rows(i), mapCoopReferente, If(isFirst, 0, 1), sql2017OrHigher, objParametri_Server)
                isFirst = False

                If Not eliminato Then

                    Dim datastr As String =
                            If(CDate(dt.Rows(i).Item("Validita_Inizio")).ToShortDateString <> "01/01/1900", CDate(dt.Rows(i).Item("Validita_Inizio")).ToShortDateString, "...") &
                                "-" & If(CDate(dt.Rows(i).Item("Validita_Fine")).ToShortDateString <> "31/12/2100", CDate(dt.Rows(i).Item("Validita_Fine")).ToShortDateString, "...")
                    Dim InizioValiditadatastr As String = If(CDate(dt.Rows(i).Item("Validita_Inizio")).ToShortDateString <> "01/01/1900", CDate(dt.Rows(i).Item("Validita_Inizio")).ToShortDateString, "")

                    Dim FineValiditadatastr As String = If(CDate(dt.Rows(i).Item("Validita_Fine")).ToShortDateString <> "31/12/2100", CDate(dt.Rows(i).Item("Validita_Fine")).ToShortDateString, "")

                    dt.Rows(i).Item("InizioValiditadatastr") = InizioValiditadatastr
                    dt.Rows(i).Item("FineValiditadatastr") = FineValiditadatastr
                    dt.Rows(i).Item("datastr") = datastr


                    Dim dr_reg As DataRow()
                    If Not IsDBNull(dt.Rows(i).Item("regolamento")) Then
                        dr_reg = dt_regolamenti.Select("Reg_Cod= " & dt.Rows(i).Item("regolamento"))
                        If dr_reg IsNot Nothing AndAlso dr_reg.Count > 0 Then
                            dt.Rows(i).Item("regolamento_str") = dr_reg(0).Item("reg_des")
                        Else
                            dt.Rows(i).Item("regolamento_str") = ""
                        End If
                    Else
                        dt.Rows(i).Item("regolamento_str") = ""
                    End If


                    dt.Rows(i).Item("chiuso") = ""
                    If CDate(dt.Rows(i).Item("Validita_Fine")) <= Date.Today Then
                        dt.Rows(i).Item("chiuso") = "true"
                    End If
                End If

            Else

                'se appezza o id_reg sono = NULL elimino il record
                dt.Rows(i).Delete()
                eliminato = True
            End If  'controllo id_reg appezza = NULL

        Next

        dt.AcceptChanges()

        Return dt
    End Function

    Private Shared Function CampoPredisponiDatiMancanti(
        ByVal dt As DataTable, ByVal JsonRichiesta As filtrone_richiesta_nuovo,
        ByVal objParametri_Server As AgronicaCoreParametri,
        ByVal objParametri_Utenti As AgronicaCoreParametri,
        ByVal ASG_SuperUser_CodFiscale As String,
        ByVal ASG_Utente_Username_Crypt As String,
        ByVal ASG_Utente_Password_Crypt As String
    ) As DataTable

        Dim Num_Campi As Integer
        Dim i As Integer

        Dim objAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

        dt.Columns.AddRange({
            New DataColumn("Superficie_Totale", Type.GetType("System.Decimal")),
            New DataColumn("Superficie_Convenzionale", Type.GetType("System.Decimal")),
            New DataColumn("Superficie_Biologico", Type.GetType("System.Decimal")),
            New DataColumn("Superficie_Conversione", Type.GetType("System.String")),
            New DataColumn("Superficie_Catastale", Type.GetType("System.String"))
        })

        Num_Campi = dt.Rows.Count

        For i = Num_Campi - 1 To 0 Step -1

            Dim Sup_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale, SAU_Catastale As Double
            objAppezzamento.Recupera_Superfici_Campo(dt.Rows(i).Item("Piva"), dt.Rows(i).Item("sa_cod"), dt.Rows(i).Item("Campo_Cod"),
                Sup_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale, SAU_Catastale, "", "", objParametri_Server)

            dt.Rows(i).Item("Superficie_Totale") = Format(Sup_Totale, "0.0000")
            dt.Rows(i).Item("Superficie_Biologico") = Format(SAU_Biologico, "0.0000")
            dt.Rows(i).Item("Superficie_Convenzionale") = Format(SAU_Convenzionale, "0.0000")
            dt.Rows(i).Item("Superficie_Conversione") = Format(SAU_Conversione, "0.0000")
            dt.Rows(i).Item("Superficie_Catastale") = Format(SAU_Catastale, "0.0000")

        Next

        dt.AcceptChanges()

        Return dt

    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Shared Sub GetImpreseReferenti(ByRef dt As DataTable, ByRef row As DataRow, ByRef mapCoopReferente As Dictionary(Of String, String), currentRowIndex As Integer, sql2017OrHigher As Boolean, ByVal objParametri_Server As AgronicaCoreParametri)

        If currentRowIndex = 0 Then
            'Entro in questa funzione n volte quante sono le righe estratte, l'aggiunta delle colonne la faccio solo la prima volta
            dt.Columns.Add("Referente", GetType(String))
            dt.Columns.Add("Piva_Padre", GetType(String))
        End If

        Dim leggiCoopReferente As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R

        If Not sql2017OrHigher Then
            Dim Referente As String = ""
            Dim PivaReferente As String = ""
            If mapCoopReferente.ContainsKey(row("piva")) Then
                Referente = mapCoopReferente(row("piva"))
                Dim Tokens As String() = Referente.Split("|")
                If Tokens.Length > 1 Then
                    Referente = Tokens(0)
                    PivaReferente = Tokens(1)
                End If
            Else
                Referente = leggiCoopReferente.LeggiImpreseReferenti(row("piva"), objParametri_Server, PivaReferente)
                mapCoopReferente.Add(row("piva"), Referente & "|" & PivaReferente)
            End If
            row("Referente") = Referente
            row("Piva_Padre") = PivaReferente
        Else
            row("Referente") = row("Referenti")
            row("Piva_Padre") = row("Padri")
        End If

        dt.AcceptChanges()

    End Sub

End Class

Public Class jsonProseguiSelezionati
    Public tipo As String
    Public chiavi As String()
End Class

Public Class filtrone_richiesta_parametriQuery
    Public Property IncludiVisite As Boolean
End Class

Public Class filtrone_richiesta_nuovo
    Public filtroazioni As FiltroAzioni_nuovo
    Public tab_selezionata As String
    Public filtrone_azienda As filtrone_azienda_nuovo
    Public filtrone_centro As filtrone_centro_nuovo
    Public filtrone_appezzamento As filtrone_appezzamento_nuovo
    Public filtrone_impianto As filtrone_impianto_nuovo
    Public filtrone_distinta As filtrone_distinta_nuovo
    Public filtrone_movimento As filtrone_movimento_nuovo
End Class

Public Class filtrone_richiesta
    Public filtroazioni As FiltroAzioni_nuovo
    Public tab_selezionata As String
    Public filtrone_azienda As filtrone_azienda_nuovo
    Public filtrone_centro As filtrone_centro
    Public filtrone_appezzamento As filtrone_appezzamento_nuovo
    Public filtrone_impianto As filtrone_impianto_nuovo
    Public filtrone_distinta As filtrone_distinta_nuovo
    Public filtrone_movimento As filtrone_movimento_nuovo
End Class

Public Class filtrone_azienda_nuovo
    Public txPiva As String
    Public txtRagSoc As String
    Public txtCuaa As String
    Public albero As String()
    Public rblDataCost As String
    Public txtDataCost As String
    Public rblDataCess As String
    Public txtDataCess As String
    Public CBL_Regioni As String()
    Public CBL_Province As String()
    Public Cmb_Comuni As String
    Public cmb_codiceazienda As String
    Public Txt_CodiceAzienda As String
    Public Rbl_min_mag_Superficie_Catastale As String
    Public txt_SupCat As String
    Public Rbl_Zone_And_Or As String
    Public cbl_zone As String()
    Public Rbl_Macrousi_And_Or As String
    Public CBL_Macrouso As String()
    Public Rbl_min_mag_Superficie_Macrousi As String
    Public Txt_SupMacrousi As String
    Public CBL_Tecnici As String()
    Public CBL_Utenti As String()
    Public CBL_PChiaveImpresa As String()
    Public cmbTitoloPossessoAzienda As String
End Class

Public Class filtrone_centro_nuovo
    Public txtCentroAziendale As String
    Public Cmb_Cartografia As String
    Public CBL_Stati_2 As String()
    Public CBL_Regioni_2 As String()
    Public CBL_Province_2 As String()
    Public Cmb_Comuni_2 As String()
    Public CmbTipoCentro As String
    Public CmbTipoAttivita As String
    Public CmbOTE As String
    Public cmbTitoloPossessoStruttura As String
    Public Cmb_CodiceStruttura As String
    Public Txt_CodiceStruttura As String
    Public CmbOrganismoControllo As String
    Public CBL_PChiaveCentro As String()
    Public Cmb_CodiceCentro As String
    Public Txt_CodiceCentro As String
End Class

Public Class filtrone_centro
    Public txtCentroAziendale As String
    Public Cmb_Cartografia As String
    Public CBL_Regioni_2 As String()
    Public CBL_Province_2 As String()
    Public Cmb_Comuni_2 As String
    Public CmbTipoCentro As String
    Public CmbTipoAttivita As String
    Public CmbOTE As String
    Public cmbTitoloPossessoStruttura As String
    Public Cmb_CodiceStruttura As String
    Public Txt_CodiceStruttura As String
    Public CmbOrganismoControllo As String
    Public CBL_PChiaveCentro As String()
End Class

Public Class filtrone_appezzamento_nuovo
    Public TxtAppNome As String
    Public TxtCampoDes As String
    Public rblSupApp As String
    Public txtSupApp As String
    Public cmbTitoloPossessoAppezzamento As String
    Public OptionList_MetodoProduzioneAppezzamento As String
    Public Txt_DataFineImpiego As String
    Public Cmb_CodiceAppezzamento As String
    Public Txt_CodiceAppezzamento As String
End Class

Public Class filtrone_impianto_nuovo
    Public chkTerrNudo As String
    Public chkIncludiTerrNudo As String
    Public ckbTuttiGruVeg As String
    Public cblGruVeg_1 As String
    Public cblGruVeg_2 As String
    Public cblGruVeg_3 As String
    Public cblSpecie As String()
    Public CBL_Cultivar As String()
    Public CBL_TipologiaVarietale As String()

    Public rblInizioImp As String
    Public txtDataIniImp As String
    Public rblFineImp As String
    Public txtDataFinImp As String

    Public ddlFinProdComm As String

    Public ddlRegolamento As String
    Public ddlDisciplinare As String

    Public chk_Capitolato As String()

    Public ddlStatoImpianto As String

    Public CkbCopertura As String
    Public ddlCopertura As String

    Public Cmb_DettaglioVarietaPersonalizzato As String

    Public Cmb_CodiceImpianto As String
    Public Txt_CodiceImpianto As String

    Public CkB_AttivaConformita As Boolean

    Public Cmb_Disciplinare As String
    Public Cmb_Difesa As String
    Public Cmb_Diserbo As String
    Public Cmb_Irrigazione As String
    Public Cmb_PianoConcimazione As String

    Public RadiobuttonlistIniDist As String
    Public txtDataIniDist As String
    Public RadiobuttonlistFineDist As String
    Public txtDataFinDist As String

    Public rblDataCreazioneImpDAL As String
    Public txtDataCreazioneImpDAL As String
    Public rblDataCreazioneImpAL As String
    Public txtDataCreazioneImpAL As String
End Class

Public Class filtrone_distinta_nuovo
    Public RadiobuttonlistIniDist As String
    Public txtDataIniDist As String
    Public RadiobuttonlistFineDist As String
    Public txtDataFinDist As String
    Public txtLotto As String
    Public txtProgetto_Des As String
End Class

Public Class filtrone_movimento_nuovo
    Public txtDataMovimentoDa As String
    Public txtDataMovimentoA As String
    Public CBL_GruppoOperazioni As String()
    Public CBL_Operazioni As String()
    Public rblAvversita As String
    Public CBL_Avversita As String()
    Public ddlCatProd As String
    Public Txt_CercaProdotto As String
    Public CBL_Prodotto As String
    Public CBL_UtentiMovimenti As String()
End Class

Public Class FiltroAzioni_nuovo
    Public Testo_Descrizione As String

    Public Pagina_Origine As String
    Public Sito_Origine As String

    Public Pagina_Destinazione As String
    Public Sito_Destinazione As String

    Public TipoFiltrone As String
    Public CodificaStampe As String

    Public Veg_Cod As String
    Public Cul_Cod As String
    Public Data_Inizio As String
    Public Data_Fine As String

    Public Annata_Inizio As Date
    Public Annata_Fine As Date

    Public Testo_Bottone_Prosegui As String
    Public Icona_Prosegui As String

    Public Singola_Piva As String
    Public Singola_Sa_Cod As String
    Public Singola_Veg_Cod As String

    Public RedirUrl As String = ""


    Public G2G_Cod As String = ""
    Public Tipo_Utilizzo_Filtrone_G2G As Boolean = False

    Public IncludiVisite As Boolean = False

    Public Filtrino As String = "0"

    Property js() As String
        Get
            Return "{Testo_Descrizione:'" & Testo_Descrizione &
                "', Pagina_Origine:'" & Pagina_Origine &
                "', Sito_Origine:'" & Sito_Origine &
                "', Pagina_Destinazione:'" & Pagina_Destinazione &
                "', Sito_Destinazione:'" & Sito_Destinazione &
                "', TipoFiltrone:'" & TipoFiltrone &
                "', CodificaStampe:'" & CodificaStampe &
                "', Veg_Cod:'" & Veg_Cod &
                "', Cul_Cod:'" & Cul_Cod &
                "', Testo_Bottone_Prosegui:'" & Testo_Bottone_Prosegui &
                "', Icona_Prosegui:'" & Icona_Prosegui &
                "', Singola_Piva:'" & Singola_Piva &
                "', Singola_Sa_Cod:'" & Singola_Sa_Cod &
                "', Singola_Veg_Cod:'" & Singola_Veg_Cod &
                "', Annata_Inizio:'" & Annata_Inizio.ToShortDateString() &
                "', Annata_Fine:'" & Annata_Fine.ToShortDateString() &
                "', Data_Inizio:'" & Data_Inizio &
                "', Data_Fine:'" & Data_Fine &
                "', RedirUrl:'" & RedirUrl &
                "', G2G_Cod:'" & G2G_Cod &
                "', Tipo_Utilizzo_Filtrone_G2G:'" & Tipo_Utilizzo_Filtrone_G2G.ToString() &
                "', IncludiVisita: '" & IncludiVisite.ToString() &
                "', Filtrino: '" & Filtrino & "' " &
             "}"
        End Get
        Set(ByVal Value As String)

        End Set
    End Property

End Class

Public Class JoinFiltrone

    '--------------------
    ' IMPRESA
    '--------------------
    Public bGerarchiaImprese As Boolean

    Public bImpreseXIndirizzi As Boolean
    Public bIndirizzi As Boolean
    Public bIstat As Boolean
    Public bListaProvince As Boolean
    Public bListaRegioni As Boolean

    Public bImpresexParticelle As Boolean

    Public bImpreseCodici As Boolean
    Public bImpreseCodici_CodiceSocio As Boolean
    Public bImpreseCodici_CodiceCuaa As Boolean
    Public bImpreseCodici_CodiceTecnico As Boolean
    Public bImpreseCodici_CodiceCertificazione As Boolean

    Public bGruppiRaccolta As Boolean

    Public bImpreseCodici_UltimaVerificaIspettiva As Boolean
    Public bImpreseCodici_SAUTotAziendale As Boolean

    '--------------------
    ' CENTRO
    '--------------------
    Public bCentriAziendali As Boolean

    Public bCentrixIndirizzi As Boolean
    Public bIndirizziCentro As Boolean
    Public bIstatCentro As Boolean
    Public bListaProvinceCentro As Boolean

    Public bCentriAziendaliCodici As Boolean

    '--------------------
    ' CAMPO
    '--------------------
    Public bCampi As Boolean

    Public bCampiCodici As Boolean

    '--------------------
    ' APPEZZAMENTO
    '--------------------
    Public bAppezzamento As Boolean

    Public bAppezzamentixParticelle As Boolean

    Public bAppezzamentoCodici As Boolean

    '--------------------
    ' IMPIANTO / ESERCIZIO
    '--------------------
    Public bRegImpianti As Boolean
    Public bImpreseProgetti As Boolean

    Public bRegImpiantiCodici As Boolean

    '--------------------
    ' UTILIZZO / SPECIE
    '--------------------
    Public bGruppoVarietale As Boolean
    Public bCultivar As Boolean
    Public bSpecieVegetali As Boolean
    Public bGruppoVegetale As Boolean
    'Anna 10/08/21: Aggiunta JOINs per colonne TTI
    Public bGruppoFinalita As Boolean

    '--------------------
    ' FABBRICATI
    '--------------------
    Public bFabbricati As Boolean
    Public bFabbricatiCodici As Boolean

    '--------------------
    ' MOVIMENTI
    '--------------------
    Public bAgenda As Boolean
    Public bMovimenti As Boolean
    Public bMovimentiDettagli As Boolean
    Public bMovDettaglioTecnico As Boolean
    Public bMovDestinazioni As Boolean

    Public bOperazioni As Boolean
    Public bGruppoOperazioni As Boolean

    '--------------------
    ' CONTATTI
    '--------------------
    Public bContatti As Boolean
    Public bRisorseUmane As Boolean
    Public bRapportiContabili As Boolean

    Public Sub New()

        '--------------------
        ' IMPRESA
        '--------------------
        bGerarchiaImprese = False

        bImpreseXIndirizzi = False
        bIndirizzi = False
        bIstat = False
        bListaProvince = False

        bImpresexParticelle = False

        bImpreseCodici = False
        bImpreseCodici_CodiceSocio = False
        bImpreseCodici_CodiceCuaa = False
        bImpreseCodici_CodiceTecnico = False
        bImpreseCodici_CodiceCertificazione = False

        bGruppiRaccolta = True

        '--------------------
        ' CENTRO
        '--------------------
        bCentriAziendali = False

        bCentrixIndirizzi = False
        bIndirizziCentro = False
        bIstatCentro = False
        bListaProvinceCentro = False

        bCentriAziendaliCodici = False

        '--------------------
        ' CAMPO
        '--------------------
        bCampi = False

        bCampiCodici = False

        '--------------------
        ' APPEZZAMENTO
        '--------------------
        bAppezzamento = False

        bAppezzamentixParticelle = False

        bAppezzamentoCodici = False

        '--------------------
        ' IMPIANTO / ESERCIZIO
        '--------------------
        bRegImpianti = False
        bImpreseProgetti = False

        bRegImpiantiCodici = False

        '--------------------
        ' UTILIZZO / SPECIE
        '--------------------
        bGruppoVarietale = False
        bCultivar = False
        bSpecieVegetali = False
        bGruppoVegetale = False
        'Anna 10/08/21: Aggiunta JOINs per colonne TTI
        bGruppoFinalita = False

        '--------------------
        ' FABBRICATI
        '--------------------
        bFabbricati = False
        bFabbricatiCodici = False

        '--------------------
        ' MOVIMENTI
        '--------------------
        bAgenda = False
        bMovimenti = False
        bMovimentiDettagli = False
        bMovDettaglioTecnico = False
        bMovDestinazioni = False

        bOperazioni = False
        bGruppoOperazioni = False

        '--------------------
        ' CONTATTI
        '--------------------
        bContatti = False
        bRisorseUmane = False
        bRapportiContabili = False
    End Sub

End Class
