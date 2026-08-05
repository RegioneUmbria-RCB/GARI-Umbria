Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Filtrone
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Sub New()
        MyBase.New()
    End Sub
    Public Function CreaStringaQueryPerDTFiltrone(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByVal Filtro As String,
                                   ByVal TipoSelect As Integer,
                                   ByVal Ordinamento As String,
                                   ByVal Join As JoinFiltrone,
                                   Optional ByVal noLock As Boolean = False,
                                   Optional ByVal FiltroVisibilita As Boolean = True) As String

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

        Dim stb As New System.Text.StringBuilder
        stb.Length = 0

        Select Case TipoSelect

            Case enum_TipoSelect_FiltroneSuperNova.Base '0

                'Nota: NON RIMUOVERE NULLA DALLA SELECT LIST!!!

                stb.Append(" SELECT Distinct " + vbCrLf)
                stb.AppendLine("        dbo.Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale ,")
                stb.AppendLine("        dbo.Imprese.rag_soc , dbo.Imprese.Sup_Totale, Imprese.Validita_Inizio as Impresa_Validita_Inizio, Imprese.Validita_Fine as Impresa_Validita_Fine,")
                stb.AppendLine("        dbo.Centri_Aziendali.sa_cod, dbo.Centri_Aziendali.sa_nome, dbo.Centri_Aziendali.TitoloPossesso, Centri_Aziendali.Validita_Inizio as Centri_Aziendali_Validita_Inizio, Centri_Aziendali.Validita_Fine as Centri_Aziendali_Validita_Fine,")
                stb.AppendLine("        Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine("        dbo.Appezzamento.APPEZZA, dbo.Appezzamento.SUP_APP, dbo.Appezzamento.APP_NOME, dbo.Appezzamento.CAMPO_COD, Appezzamento.Validita_Inizio as Appezzamento_Validita_Inizio, Appezzamento.Validita_Fine as Appezzamento_Validita_Fine,")
                'elimino p_ha xkè ora passa dall'impianto alla distinta (FEDE 13/05/2010)
                stb.AppendLine("        dbo.Reg_Impianti.CUL_COD, dbo.Reg_Impianti.GRVA_Cod_VEG, GruppoVarietale.GRVA_DES, dbo.Reg_Impianti.ID_REG, dbo.Reg_Impianti.Port_Cod, Reg_Impianti.Validita_Inizio as Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine as Reg_Impianti_Validita_Fine, dbo.Reg_Impianti.Sup_Imp, Reg_Impianti.Resa_Prevista, ")
                'StrSQL.append( "        dbo.Reg_Impianti.CUL_COD, dbo.Reg_Impianti.GRVA_Cod_VEG, GruppoVarietale.GRVA_DES, dbo.Reg_Impianti.ID_REG, dbo.Reg_Impianti.P_HA, dbo.Reg_Impianti.Port_Cod, Reg_Impianti.Validita_Inizio as Reg_Impianti_Validita_Inizio, Reg_Impianti.Validita_Fine as Reg_Impianti_Validita_Fine, dbo.Reg_Impianti.Sup_Imp, Reg_Impianti.Resa_Prevista, "
                stb.AppendLine("        dbo.Lista_Province.PROVINCIA,")
                stb.AppendLine("        dbo.GruppoVegetale.GRU_DES, dbo.Istat.Cap, dbo.Istat.Localita,")
                stb.AppendLine("        dbo.SpecieVegetali.VEG_COD, dbo.SpecieVegetali.VEG_DES, dbo.Cultivar.CUL_DES, Imprese.TipoImpresaGerarchia ,")
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


            Case enum_TipoSelect_FiltroneSuperNova.ImpreseAlbero '1 'Solo Imprese x Albero

                stb.AppendLine(" SELECT dbo.Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale ,dbo.Imprese.rag_soc, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia")
                Join.bGerarchiaImprese = True

            Case enum_TipoSelect_FiltroneSuperNova.Imprese  '2 'Imprese

                'StrSQL.append( " SELECT DISTINCT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine "
                stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA as chiave, dbo.Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine, IC.val_cod AS CodiceSocio, ISNULL(IC_Cuaa.val_cod, '') AS CodiceCuaa ")

                Join.bListaProvince = True
                Join.bIndirizzi = True
                Join.bIstat = True
                Join.bImpreseCodici_CodiceSocio = True
                Join.bImpreseCodici_CodiceCuaa = True

            Case enum_TipoSelect_FiltroneSuperNova.Imprese_APP

                stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA as chiave, dbo.Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Indirizzi.Stato, Istat.Localita, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine, IC_Cuaa.val_cod AS CodiceCuaa, ")
                stb.AppendLine(" Lista_Province.Sigla AS PROV, Lista_Regioni.REG, Lista_Regioni.Regione_Des AS REGIONE, GerarchiaImprese.Padre, GerarchiaImprese.Livello, GerarchiaImprese.Foglia ")

                Join.bGerarchiaImprese = True
                Join.bListaProvince = True
                Join.bListaRegioni = True
                Join.bIndirizzi = True
                Join.bIstat = True
                Join.bImpreseCodici_CodiceCuaa = True

            Case enum_TipoSelect_FiltroneSuperNova.Imprese_Visibilita_Appoggio

                'StrSQL.append( " SELECT DISTINCT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine "
                stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA as chiave, dbo.Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, dbo.Imprese.rag_soc ")

                Join.bListaProvince = False
                Join.bIndirizzi = False
                Join.bIstat = False
                Join.bImpreseCodici_CodiceSocio = False
                Join.bImpreseCodici_CodiceCuaa = False
                Join.bCentriAziendali = True

            Case enum_TipoSelect_FiltroneSuperNova.Imprese_Codici  '9 'Imprese_Codici

                'StrSQL.append( " SELECT DISTINCT dbo.Imprese.PIVA, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine "
                stb.AppendLine(" SELECT DISTINCT dbo.Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, dbo.Imprese.rag_soc, Lista_Province.PROVINCIA, Indirizzi.CAP, Istat.Localita, Imprese.TipoImpresaGerarchia, Imprese.Validita_Inizio, Imprese.Validita_Fine , Imprese_Codici.id_cod, Imprese_Codici.val_cod  ")

                Join.bListaProvince = True
                Join.bIndirizzi = True
                Join.bIstat = True
                Join.bImpreseCodici = True


            Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali  '3 'Centri Aziendali

                stb.AppendLine(" SELECT DISTINCT  (Imprese.PIVA + '_' + cast(Centri_Aziendali.sa_cod  as varchar(20))) as chiave, Imprese.PIVA, ")
                stb.AppendLine(" CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, ")
                stb.AppendLine(" Imprese.rag_soc, ")
                stb.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine ")

                If Join.bGis_CentriAziendali Then
                    stb.Append(" , isnull( geo.Poligono_GeoEntity.STAsText(), case when Centri_Aziendali.lat = 0 then '' else 'POINT (' + replace( cast(Centri_Aziendali.long as varchar(50)), ',','.') + ' ' + replace( cast(Centri_Aziendali.lat as varchar(50)), ',','.') +  ')' end) as Gis_wkt")
                End If

                Join.bListaProvince = True
                Join.bCentriAziendali = True

            Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali_Visibilita_Appoggio   '3 'Centri Aziendali

                stb.AppendLine(" SELECT DISTINCT  (Imprese.PIVA + '_' + cast(Centri_Aziendali.sa_cod  as varchar(20))) as chiave, Imprese.PIVA, ")
                stb.AppendLine(" CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, Imprese.rag_soc, ")
                stb.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, Centri_Aziendali.Validita_Inizio, Centri_Aziendali.Validita_Fine ")

                If Join.bGis_CentriAziendali Then
                    stb.Append(" , isnull( geo.Poligono_GeoEntity.STAsText(), case when Centri_Aziendali.lat = 0 then '' else 'POINT (' + replace( cast(Centri_Aziendali.long as varchar(50)), ',','.') + ' ' + replace( cast(Centri_Aziendali.lat as varchar(50)), ',','.') +  ')' end) as Gis_wkt")
                End If

                Join.bListaProvince = False
                Join.bCentriAziendali = True

            Case enum_TipoSelect_FiltroneSuperNova.Appezzamenti  '4 'Appezzamenti

                '02/09/2019: eliminata dalla select Campi.Campo_Cod, perchè c'era un doppione su campo_cod
                'tra l'altro la tabella campi è in left join e lì poteva esserci null

                stb.AppendLine(" SELECT DISTINCT  (Imprese.PIVA + '_' + cast(Centri_Aziendali.sa_cod  as varchar(20))+ '_' + cast(Appezzamento.Appezza  as varchar(20))) as chiave, ")
                stb.AppendLine(" Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, Imprese.rag_soc, ")
                stb.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
                stb.AppendLine(" Campi.Campo_Des,")
                stb.AppendLine(" Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.APP_NOME, Appezzamento.Campo_Cod, Appezzamento.Validita_Inizio, Appezzamento.Validita_Fine ")

                If Join.bGis_Appezzamento Then
                    stb.Append(" , isnull( geo.Poligono_GeoEntity.STAsText(), '') as Gis_wkt")
                End If

                Join.bListaProvince = True
                Join.bCentriAziendali = True
                Join.bCampi = True
                Join.bAppezzamento = True

            Case enum_TipoSelect_FiltroneSuperNova.Impianti  '5 'Impianti

                stb.AppendLine(" SELECT DISTINCT  (Imprese.PIVA + '_' + cast(Centri_Aziendali.sa_cod  as varchar(20))+ '_' + cast(Appezzamento.Appezza  as varchar(20)) + '_' + cast(Reg_Impianti.id_reg  as varchar(20)) + '_' + cast(isnull(SpecieVegetali.veg_cod ,'0')  as varchar(20)) + '_' + cast(isnull(Imprese_Progetti.Progetto_Cod  ,'0')  as varchar(20)) ) as chiave, ")
                stb.AppendLine(" Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, Imprese.rag_soc, Lista_Province.PROVINCIA, ")
                stb.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome, ")
                stb.AppendLine(" SpecieVegetali.Veg_Cod, Reg_Impianti.Cul_Cod,  dbo.Reg_Impianti.GRVA_Cod_VEG, ")
                stb.AppendLine(" SpecieVegetali.Veg_Des, Cultivar.Cul_Des, GruppoVarietale.GRVA_DES, ")
                stb.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine(" Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.APP_NOME, ")
                stb.AppendLine(" Reg_Impianti.ID_REG, Reg_Impianti.Validita_Inizio, Reg_Impianti.Validita_Fine, ")
                stb.AppendLine(" Reg_Impianti.Sup_Imp, Imprese_Progetti.Regolamento_Cod AS Regolamento, Imprese_Progetti.Disciplinare_Cod AS Finanziamento, Reg_Impianti.Grfi_Cod,  Reg_Impianti.Grva_Cod_Veg, Reg_Impianti.Cop_Cod, ")
                stb.AppendLine(" Imprese_Progetti.Progetto_Cod, Imprese_Progetti.Progetto_Nome, ")
                stb.AppendLine(" Imprese_Progetti.Validita_Inizio AS Progetto_Validita_Inizio, Imprese_Progetti.Validita_Fine AS Progetto_Validita_Fine, IC.val_cod AS CodiceSocio, IC_Cuaa.val_cod AS CodiceCuaa ")

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

            Case enum_TipoSelect_FiltroneSuperNova.Movimenti  '6 'Movimenti


                stb.AppendLine(" SELECT DISTINCT  (Imprese.PIVA + '_' + cast(Centri_Aziendali.sa_cod  as varchar(20))+ '_' + cast(Appezzamento.Appezza  as varchar(20)) + '_' + cast(Reg_Impianti.id_reg  as varchar(20)) + '_' + cast(isnull(SpecieVegetali.veg_cod ,'0')  as varchar(20)) + '_' + cast(Agenda.Id_Agenda  as varchar(20)) ) as chiave, ")
                stb.AppendLine(" Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, Imprese.rag_soc, ")
                stb.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
                stb.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine(" Appezzamento.APPEZZA, Appezzamento.SUP_APP, Appezzamento.APP_NOME, ")
                stb.AppendLine(" Reg_Impianti.ID_REG, Reg_Impianti.Sup_Imp, ")
                stb.AppendLine(" Agenda.Id_Agenda, Agenda.Lav_Cod, (select lav_des from operazioni where lav_cod = Agenda.Lav_Cod) as Lav_des, Agenda.Des_Lib, Agenda.Username_Creazione, Agenda.Username_Modifica, Agenda.Data_Creazione, Agenda.Data_Modifica, Agenda.Validita_Inizio, ")
                stb.AppendLine(" Movimenti.Data_Movimento, Movimenti.Cau_Mov, Movimenti.Num_Protocollo, ")
                stb.AppendLine(" SpecieVegetali.VEG_COD, SpecieVegetali.Veg_Des, Reg_Impianti.Cul_Cod, Cultivar.Cul_Des  ")

                Join.bCentriAziendali = True
                Join.bCampi = True
                Join.bAppezzamento = True
                Join.bRegImpianti = True
                Join.bAgenda = True
                Join.bMovimenti = True
                Join.bSpecieVegetali = True
                Join.bCultivar = True

                Join.bListaProvince = True

            Case enum_TipoSelect_FiltroneSuperNova.Contatti  '7 'Contatti

                stb.AppendLine(" SELECT DISTINCT Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, Imprese.rag_soc, ")
                stb.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine(" Contatti.Cod_Contatto, Contatti.Rag_Soc as Contatto_Rag_Soc,")
                stb.AppendLine(" Risorse_Umane.Cod_Rapporto, Risorse_Umane.Cod_RisUm, Risorse_Umane.Piva, Risorse_Umane.Settore_Des, Risorse_Umane.Attivita_Des, Risorse_Umane.Validita_Fine as Cessazione,")
                stb.AppendLine(" Rapporti_Contabili.Rapporto_Des")

                Join.bCampi = True
                Join.bContatti = True
                Join.bRisorseUmane = True
                Join.bRapportiContabili = True

            Case enum_TipoSelect_FiltroneSuperNova.Campi  '8 'Campi

                stb.AppendLine(" SELECT DISTINCT Imprese.PIVA, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Imprese.PIVA ELSE Imprese.partitaIvaReale END PivaReale, Imprese.rag_soc, ")
                stb.AppendLine(" Centri_Aziendali.sa_cod, Centri_Aziendali.sa_nome,")
                stb.AppendLine(" Campi.Campo_Cod, Campi.Campo_Des,")
                stb.AppendLine(" Campi.Validita_Inizio, Campi.Validita_Fine ")

                Join.bCentriAziendali = True
                Join.bCampi = True



        End Select

        '----------------------------------------------------
        '--- Gestione delle dipendenze dei JOIN -------------
        '----------------------------------------------------

        ImpostaVariabiliJOIN(Join)

        Dim StrJOIN As New System.Text.StringBuilder
        Dim nParentesiTondeAperte As Integer = 0

        StrJOIN.Length = 0

        If Join.bGerarchiaImprese Then
            'StrJOIN.Append(" LEFT JOIN GerarchiaImprese  (NOLOCK) ON Imprese.Piva = GerarchiaImprese.Figlio) " + vbCrLf)
            StrJOIN.AppendLine(" LEFT JOIN GerarchiaImprese " & IIf(noLock, " (NOLOCK) ", "") & " ON Imprese.Piva = GerarchiaImprese.Figlio) ")
            nParentesiTondeAperte += 1
        End If

        Join.bImpreseXIndirizzi = True
        If TipoSelect = enum_TipoSelect_FiltroneSuperNova.Imprese_Visibilita_Appoggio Or TipoSelect = enum_TipoSelect_FiltroneSuperNova.CentriAziendali_Visibilita_Appoggio Then
            Join.bImpreseXIndirizzi = False
        End If

        If Join.bImpreseXIndirizzi Then
            StrJOIN.AppendLine(" INNER join ImpresexIndirizzi " & IIf(noLock, " (NOLOCK) ", "") & " ON Imprese.PIVA = ImpresexIndirizzi.PIVA) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bIndirizzi Then
            StrJOIN.AppendLine(" INNER JOIN Indirizzi " & IIf(noLock, " (NOLOCK) ", "") & " ON ImpresexIndirizzi.cod_indirizzo = Indirizzi.cod_indirizzo) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bIstat Then
            StrJOIN.AppendLine(" INNER JOIN ISTAT " & IIf(noLock, " (NOLOCK) ", "") & " ON Indirizzi.pro_cod_istat = ISTAT.PROV AND Indirizzi.com_cod_istat = ISTAT.COM) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bListaProvince Then
            StrJOIN.AppendLine(" INNER JOIN Lista_Province " & IIf(noLock, " (NOLOCK) ", "") & " ON Lista_Province.Sigla = ISTAT.COMUNI_PROV) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bListaRegioni Then
            StrJOIN.AppendLine(" LEFT JOIN Lista_Regioni " & IIf(noLock, " (NOLOCK) ", "") & " ON Lista_Regioni.REG = Lista_Province.REG) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bCentriAziendali Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Centri_Aziendali " & IIf(noLock, " (NOLOCK) ", "") & " ON Imprese.Piva=Centri_Aziendali.Piva) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bCentrixIndirizzi Then
            StrJOIN.AppendLine(" LEFT OUTER join CentrixIndirizzi " & IIf(noLock, " (NOLOCK) ", "") & " ON (Centri_Aziendali.PIVA = CentrixIndirizzi.PIVA And Centri_Aziendali.Sa_Cod = CentrixIndirizzi.Sa_Cod) ")
        End If

        If Join.bIndirizziCentro Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Indirizzi " & IIf(noLock, " (NOLOCK) ", "") & " As Indirizzi_Centro ON CentrixIndirizzi.cod_indirizzo = Indirizzi_Centro.cod_indirizzo) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bIstatCentro Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN ISTAT As Istat_Centro " & IIf(noLock, " (NOLOCK) ", "") & " ON Indirizzi_Centro.pro_cod_istat = Istat_Centro.PROV AND Indirizzi_Centro.com_cod_istat = Istat_Centro.COM) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bListaProvinceCentro Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Lista_Province as Lista_Province_Centro " & IIf(noLock, " (NOLOCK) ", "") & " ON Lista_Province_Centro.Sigla = Istat_Centro.COMUNI_PROV) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bAppezzamento Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Appezzamento " & IIf(noLock, " (NOLOCK) ", "") & " on (Centri_Aziendali.Piva = Appezzamento.Piva AND Centri_Aziendali.Sa_Cod = Appezzamento.Sa_Cod ) ")
        End If

        If Join.bRegImpianti Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Reg_Impianti " & IIf(noLock, " (NOLOCK) ", "") & " on (Appezzamento.Piva = Reg_Impianti.Piva AND Appezzamento.SA_Cod = Reg_Impianti.Sa_Cod AND Appezzamento.Appezza = Reg_Impianti.Appezza) ")
        End If

        If Join.bImpreseProgetti Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Progetti " & IIf(noLock, " (NOLOCK) ", "") & " on (Imprese_Progetti.Piva = Reg_Impianti.Piva AND Imprese_Progetti.SA_Cod = Reg_Impianti.Sa_Cod AND Imprese_Progetti.Appezza = Reg_Impianti.Appezza AND Imprese_Progetti.Id_Reg = Reg_Impianti.Id_Reg) ")
        End If

        If Join.bImpreseCodici Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici  " & IIf(noLock, " (NOLOCK) ", "") & " on (Imprese_Codici.Piva = Imprese.Piva) ")
        End If

        If Join.bImpreseCodici_CodiceSocio Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC " & IIf(noLock, " (NOLOCK) ", "") & " on (IC.Piva = Imprese.Piva) AND IC.id_cod = " & enum_CodiciAnagrafe.Codice_Socio & " ")
        End If

        If Join.bImpreseCodici_CodiceCuaa Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Imprese_Codici IC_cuaa " & IIf(noLock, " (NOLOCK) ", "") & " on (IC_cuaa.Piva = Imprese.Piva) AND IC_cuaa.id_cod = " & enum_CodiciAnagrafe.CodiceCUAA & " ")
        End If

        If Join.bCentriAziendaliCodici Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Centri_Aziendali_Codici " & IIf(noLock, " (NOLOCK) ", "") & " on (Centri_Aziendali_Codici.Piva = Centri_Aziendali.Piva AND Centri_Aziendali_Codici.SA_Cod = Centri_Aziendali.Sa_Cod ) ")
        End If

        If Join.bAppezzamentoCodici Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Appezzamento_Codici " & IIf(noLock, " (NOLOCK) ", "") & " on (Appezzamento_Codici.Piva = Appezzamento.Piva AND Appezzamento_Codici.SA_Cod = Appezzamento.Sa_Cod AND Appezzamento_Codici.Appezza = Appezzamento.Appezza ) ")
        End If

        If Join.bRegImpiantiCodici Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Reg_Impianti_Codici " & IIf(noLock, " (NOLOCK) ", "") & " on (Reg_Impianti_Codici.Piva = Reg_Impianti.Piva AND Reg_Impianti_Codici.SA_Cod = Reg_Impianti.Sa_Cod AND Reg_Impianti_Codici.Appezza = Reg_Impianti.Appezza AND Reg_Impianti_Codici.Id_Reg = Reg_Impianti.Id_Reg) ")
        End If

        If Join.bCampi Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Campi " & IIf(noLock, " (NOLOCK) ", "") & " on (Campi.Piva = Appezzamento.Piva AND Campi.Sa_Cod = Appezzamento.Sa_Cod AND Campi.Campo_Cod = Appezzamento.Campo_Cod ) ")
        End If

        If Join.bAppezzamentixParticelle Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN AppezzamentixParticelle " & IIf(noLock, " (NOLOCK) ", "") & " on (AppezzamentixParticelle.Piva = Appezzamento.Piva AND AppezzamentixParticelle.Sa_Cod = Appezzamento.Sa_Cod AND AppezzamentixParticelle.Appezza = Appezzamento.Appezza ) ")
        End If

        If Join.bGruppoVarietale Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN GruppoVarietale " & IIf(noLock, " (NOLOCK) ", "") & " ON abs(Reg_Impianti.GRVA_Cod_VEG) = GruppoVarietale.GRVA_COD) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bCultivar Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Cultivar " & IIf(noLock, " (NOLOCK) ", "") & " ON Reg_Impianti.CUL_COD = Cultivar.CUL_COD) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bSpecieVegetali Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN SpecieVegetali " & IIf(noLock, " (NOLOCK) ", "") & " ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bGruppoVegetale Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN GruppoVegetale " & IIf(noLock, " (NOLOCK) ", "") & " ON SpecieVegetali.GRU_COD = GruppoVegetale.GRU_COD) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bMovDestinazioni Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Mov_Destinazioni " & IIf(noLock, " (NOLOCK) ", "") & " ON Mov_Destinazioni.Piva = Reg_Impianti.Piva and Mov_Destinazioni.Sa_Cod = Reg_Impianti.Sa_Cod and Mov_Destinazioni.Appezza = Reg_Impianti.Appezza and Mov_Destinazioni.Id_Destinazione = Reg_Impianti.Id_Reg  And Mov_Destinazioni.Tipo_Destinazione = 0) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bAgenda Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Agenda " & IIf(noLock, " (NOLOCK) ", "") & " ON Agenda.Piva = Reg_Impianti.Piva and Agenda.Sa_Cod = Reg_Impianti.Sa_Cod and Agenda.Id_Agenda = Mov_Destinazioni.Id_Agenda) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bMovimenti Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Movimenti " & IIf(noLock, " (NOLOCK) ", "") & " ON Agenda.Piva = Movimenti.Piva and Agenda.Sa_Cod = Movimenti.Sa_Cod and Agenda.Id_Agenda = Movimenti.Id_Agenda) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bMovDettaglioTecnico Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Mov_Dettaglio_Tecnico " & IIf(noLock, " (NOLOCK) ", "") & " ON Mov_Dettaglio_Tecnico.Piva = Movimenti.Piva and Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti.Sa_Cod and Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti.Id_Agenda and Mov_Dettaglio_Tecnico.Id_Mov = Movimenti.Id_Mov) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bMovimentiDettagli Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Movimenti_Dettagli " & IIf(noLock, " (NOLOCK) ", "") & " ON Movimenti_Dettagli.Piva = Movimenti.Piva and Movimenti_Dettagli.Sa_Cod = Movimenti.Sa_Cod and Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda and Movimenti_Dettagli.Id_Mov = Movimenti.Id_Mov) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bOperazioni Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Operazioni " & IIf(noLock, " (NOLOCK) ", "") & " ON  Agenda.Lav_Cod = Operazioni.Lav_Cod) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bGruppoOperazioni Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN GruppoOperazioni " & IIf(noLock, " (NOLOCK) ", "") & " ON  Operazioni.Gru_Op = GruppoOperazioni.Gru_Cod) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bRisorseUmane Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Risorse_Umane " & IIf(noLock, " (NOLOCK) ", "") & " ON ((Risorse_Umane.Cod_RisUm = Movimenti.Cod_RisUm) or (Risorse_Umane.Cod_RisUm = Movimenti_Dettagli.Mat_Cod and Movimenti_Dettagli.Elem_Cod = 0) ) )")
            nParentesiTondeAperte += 1
        End If

        If Join.bContatti Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Contatti " & IIf(noLock, " (NOLOCK) ", "") & " ON Contatti.Cod_Contatto = Risorse_Umane.Cod_Contatto) ")
            nParentesiTondeAperte += 1
        End If

        If Join.bRapportiContabili Then
            StrJOIN.AppendLine(" LEFT OUTER JOIN Rapporti_Contabili " & IIf(noLock, " (NOLOCK) ", "") & " ON Rapporti_Contabili.Cod_Rapporto = Risorse_Umane.Cod_Rapporto ) ")
            nParentesiTondeAperte += 1
        End If


        If Join.bGis_CentriAziendali AndAlso TipoSelect = enum_TipoSelect_FiltroneSuperNova.CentriAziendali Then

            StrJOIN.AppendLine(" LEFT OUTER JOIN GIS_Entita ent " & IIf(noLock, " (NOLOCK) ", "") & " ")
            StrJOIN.AppendLine(" ON ent.piva = Centri_Aziendali.piva  ")
            StrJOIN.AppendLine(" and ent.sa_Cod = Centri_Aziendali.sa_cod  ")
            StrJOIN.AppendLine(" and ent.tipoEntita_Cod = 67 )")

            nParentesiTondeAperte += 1

            StrJOIN.AppendLine(" LEFT OUTER JOIN GIS_ElementiGrafici geo " & IIf(noLock, " (NOLOCK) ", "") & " ")
            StrJOIN.AppendLine(" on ent.entita_cod = geo.entita_Cod ) ")

            nParentesiTondeAperte += 1
        End If


        If Join.bGis_Appezzamento AndAlso TipoSelect = enum_TipoSelect_FiltroneSuperNova.Appezzamenti Then

            Select Case Join.bGis_Appezzamento_TipoEntitaCheSurrogaDAti
                Case enum_GIS2012_TipoEntita.CAMPI

                    '' VAnni: 26/9/2019: nota: la select che sceglie una sola entità (vedi "case") fa sì che a fronte di più righe una sola poi va in inner join sulla seconda tabella Gis_ElementiGrafici,
                    '      quindi non verranno selezionati righe ripetute.
                    StrJOIN.AppendLine("  LEFT OUTER JOIN ( ")
                    StrJOIN.AppendLine("  Select ent1.* ")
                    StrJOIN.AppendLine("  From ( ")
                    StrJOIN.AppendLine("      Select ")
                    StrJOIN.AppendLine("          Case when eApp.Entita_Cod Is null then  ")
                    StrJOIN.AppendLine("              eCmp.Entita_Cod ")
                    StrJOIN.AppendLine("          Else ")
                    StrJOIN.AppendLine("              Case when eCmp.Entita_Cod Is null then   ")
                    StrJOIN.AppendLine("                  eImp.Entita_Cod ")
                    StrJOIN.AppendLine("              Else ")
                    StrJOIN.AppendLine("                  eApp.Entita_Cod ")
                    StrJOIN.AppendLine("              End ")
                    StrJOIN.AppendLine("         End As Entita_cod  ")
                    StrJOIN.AppendLine("       , app.PIVA   ")
                    StrJOIN.AppendLine("       , app.SA_COD  ")
                    StrJOIN.AppendLine("       , app.APPEZZA         ")
                    StrJOIN.AppendLine("    From Appezzamento app  ")
                    StrJOIN.AppendLine("    Left Join GIS_Entita eCmp  ")
                    StrJOIN.AppendLine("          On eCmp.Piva = app.PIVA   ")
                    StrJOIN.AppendLine("        And eCmp.Sa_Cod = app.SA_COD          ")
                    StrJOIN.AppendLine("        And eCmp.Campo_Cod = app.Campo_Cod    ")
                    StrJOIN.AppendLine("        And eCmp.TipoEntita_Cod = 18  ")
                    StrJOIN.AppendLine("    Left Join GIS_Entita eApp  ")
                    StrJOIN.AppendLine("           On eApp.Piva = app.PIVA   ")
                    StrJOIN.AppendLine("        And eApp.Sa_Cod = app.SA_COD  ")
                    StrJOIN.AppendLine("        And eApp.Appezza = app.APPEZZA        ")
                    StrJOIN.AppendLine("        And eApp.TipoEntita_Cod = 1  ")
                    StrJOIN.AppendLine("    Left Join GIS_Entita eImp  ")
                    StrJOIN.AppendLine("          On eImp.Piva = app.PIVA   ")
                    StrJOIN.AppendLine("       And eImp.Sa_Cod = app.SA_COD  ")
                    StrJOIN.AppendLine("       And eImp.Appezza = app.APPEZZA        ")
                    StrJOIN.AppendLine("       And eImp.TipoEntita_Cod in (19,20,21,22,23)")
                    StrJOIN.AppendLine(" ) ent1   ")

                    '' VAnni: 26/9/2019: Nota: questo inner join permette di selezionare una sola riga
                    StrJOIN.AppendLine("  INNER JOIN GIS_ElementiGrafici g ")
                    StrJOIN.AppendLine("  on ent1.entita_cod = g.entita_Cod  ")
                    StrJOIN.AppendLine(" ) ent   ")
                    ' VAnni: 26/9/2019: Nota: il tutto in left join con appezzamento
                    StrJOIN.AppendLine(" ON ent.piva = Appezzamento.piva  ")
                    StrJOIN.AppendLine(" and ent.sa_Cod = Appezzamento.sa_cod  ")
                    StrJOIN.AppendLine(" and ent.appezza = appezzamento.appezza ) ")

                    nParentesiTondeAperte += 1

                    StrJOIN.AppendLine(" LEFT OUTER JOIN GIS_ElementiGrafici geo ")
                    StrJOIN.AppendLine(" on ent.entita_cod = geo.entita_Cod ) ")

                    nParentesiTondeAperte += 1



                Case Else


                    StrJOIN.AppendLine(" LEFT OUTER JOIN GIS_Entita ent  ")
                    StrJOIN.AppendLine(" ON ent.piva = appezzamento.piva  ")
                    StrJOIN.AppendLine(" and ent.sa_Cod = appezzamento.sa_cod  ")
                    StrJOIN.AppendLine(" and ent.appezza = appezzamento.appezza ")
                    StrJOIN.AppendLine(" and ent.tipoEntita_Cod = 1 )")

                    nParentesiTondeAperte += 1

                    StrJOIN.AppendLine(" LEFT OUTER JOIN GIS_ElementiGrafici geo ")
                    StrJOIN.AppendLine(" On ent.entita_cod = geo.entita_Cod ) ")

                    nParentesiTondeAperte += 1
            End Select


        End If


        Dim strParentesi As String = ""
        strParentesi = strParentesi.PadRight(nParentesiTondeAperte, "(")

        stb.AppendLine(" FROM ( " & strParentesi)
        stb.AppendLine(" Imprese " & IIf(noLock, " (NOLOCK) ", "") & " INNER JOIN UtentiXImprese " & IIf(noLock, " (NOLOCK) ", "") & " On Imprese.Piva = UtentixImprese.Piva) ")

        '----------------------------------------------------------------
        '--- Filtro associato all'utente 
        '----------------------------------------------------------------
        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
        Dim i As Integer

        Dim DtImpreseVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri)
        Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, "", "", objParametri)
        'i due filtri sql vengono aggiunti alla query ma mai valorizzati...?
        Dim UtenteProfiloImpreseSql As String = ""
        Dim UtenteProfiloCentriSql As String = ""

        'If Not DtImpreseVisibili Is Nothing Then
        '    For i = 0 To DtImpreseVisibili.Rows.Count - 1
        '        UtenteProfiloImpreseSql &= "'" & DtImpreseVisibili.Rows(i).Item("Piva") & "',"
        '    Next
        '    If UtenteProfiloImpreseSql <> "" Then
        '        UtenteProfiloImpreseSql = " AND Imprese.piva IN (" & Left(UtenteProfiloImpreseSql, UtenteProfiloImpreseSql.Length - 1) & ") "
        '    End If
        'End If
        If FiltroVisibilita AndAlso DtImpreseVisibili IsNot Nothing AndAlso DtImpreseVisibili.Rows.Count > 0 Then
            Select Case TipoSelect
                Case enum_TipoSelect_FiltroneSuperNova.Appezzamenti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & IIf(noLock, " (NOLOCK) ", "") & " ON Appezzamento.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1 AND Utenti_Visibilita_Appoggio_I.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                Case enum_TipoSelect_FiltroneSuperNova.Impianti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & IIf(noLock, " (NOLOCK) ", "") & " ON Reg_Impianti.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1 AND Utenti_Visibilita_Appoggio_I.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                Case enum_TipoSelect_FiltroneSuperNova.Esercizi
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & IIf(noLock, " (NOLOCK) ", "") & " ON Imprese_Progetti.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1 AND Utenti_Visibilita_Appoggio_I.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                Case enum_TipoSelect_FiltroneSuperNova.Imprese, enum_TipoSelect_FiltroneSuperNova.Imprese_APP, enum_TipoSelect_FiltroneSuperNova.ImpreseAlbero
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & IIf(noLock, " (NOLOCK) ", "") & " ON Imprese.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1 AND Utenti_Visibilita_Appoggio_I.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                Case enum_TipoSelect_FiltroneSuperNova.Movimenti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_I " & IIf(noLock, " (NOLOCK) ", "") & " ON Agenda.Piva = Utenti_Visibilita_Appoggio_I.Piva AND Utenti_Visibilita_Appoggio_I.Entita_Cod = 1 AND Utenti_Visibilita_Appoggio_I.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            End Select
        End If

        If FiltroVisibilita AndAlso DtCentriVisibili IsNot Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
            Select Case TipoSelect
                Case enum_TipoSelect_FiltroneSuperNova.Appezzamenti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & IIf(noLock, " (NOLOCK) ", "") & " ON Appezzamento.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Appezzamento.Sa_Cod = Utenti_Visibilita_Appoggio_C.Sa_Cod AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 2 AND Utenti_Visibilita_Appoggio_C.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                Case enum_TipoSelect_FiltroneSuperNova.Impianti
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & IIf(noLock, " (NOLOCK) ", "") & " ON Reg_Impianti.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Reg_Impianti.Sa_Cod = Utenti_Visibilita_Appoggio_C.Sa_Cod AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 2 AND Utenti_Visibilita_Appoggio_C.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                Case enum_TipoSelect_FiltroneSuperNova.Esercizi
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & IIf(noLock, " (NOLOCK) ", "") & " ON Imprese_Progetti.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Imprese_Progetti.Sa_Cod = Utenti_Visibilita_Appoggio_C.Sa_Cod AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 2 AND Utenti_Visibilita_Appoggio_C.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali
                    StrJOIN.AppendLine(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & IIf(noLock, " (NOLOCK) ", "") & " ON Centri_Aziendali.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Centri_Aziendali.Sa_Cod = Utenti_Visibilita_Appoggio_C.Sa_Cod AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 2 AND Utenti_Visibilita_Appoggio_C.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
                    'Case enum_TipoSelect_FiltroneSuperNova.Movimenti
                    '    StrJOIN.Append(" INNER JOIN Utenti_Visibilita_Appoggio Utenti_Visibilita_Appoggio_C " & IIf(noLock, " (NOLOCK) ", "") & " ON Agenda.Piva = Utenti_Visibilita_Appoggio_C.Piva AND Utenti_Visibilita_Appoggio_C.Entita_Cod = 1  AND Utenti_Visibilita_Appoggio.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'   " + vbCrLf)
            End Select
        End If

        stb.AppendLine(StrJOIN.ToString)
        stb.AppendLine(" Where 1 = 1 ")
        If Join.bImpreseXIndirizzi Then
            stb.AppendLine(" AND (ImpresexIndirizzi.Tipo_Indirizzo = 1)")
        End If
        stb.AppendLine(" And   UtentixImprese.[User] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
        stb.AppendLine(" AND   Imprese.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
        stb.AppendLine(" AND   Imprese.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

        If Trim(Filtro) <> "" Then

            ' se nel filtro non c'è AND lo aggiungo
            If Not Trim(Filtro).StartsWith("AND") Then
                stb.Append(" AND " + vbCrLf)
            End If
            'StrSQL.Append(Agro_SQL_SaveText(Filtro) + vbCrLf)
            'non faccio l'Agro_SQL_SaveText perchè se arriva, ad esempio,
            'come Filtro  (Imprese.Rag_Soc like '%blablabla%')   
            'poi diventa  (Imprese.Rag_Soc like ''%blablabla%'') 
            'e da errore
            stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(Filtro,, objParametri))
        End If

        Select Case TipoSelect

            Case enum_TipoSelect_FiltroneSuperNova.Base '0

                stb.AppendLine(UtenteProfiloImpreseSql)
                stb.AppendLine(UtenteProfiloCentriSql)

            Case enum_TipoSelect_FiltroneSuperNova.ImpreseAlbero '1 'Solo Imprese x Albero

                stb.AppendLine(UtenteProfiloImpreseSql)

            Case enum_TipoSelect_FiltroneSuperNova.Imprese, enum_TipoSelect_FiltroneSuperNova.Imprese_APP  '2 'Imprese

                If FiltroVisibilita AndAlso Not DtImpreseVisibili Is Nothing AndAlso DtImpreseVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                stb.AppendLine(UtenteProfiloImpreseSql)

            Case enum_TipoSelect_FiltroneSuperNova.Imprese_Visibilita_Appoggio   '2 'Imprese

                stb.AppendLine(UtenteProfiloImpreseSql)

            Case enum_TipoSelect_FiltroneSuperNova.Imprese_Codici  '9 'Imprese_Codici

                stb.AppendLine(UtenteProfiloImpreseSql)

            Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali  '3 'Centri Aziendali

                If FiltroVisibilita AndAlso Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    'stb.Append(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " " + vbCrLf)
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_C.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                stb.AppendLine(UtenteProfiloImpreseSql)
                stb.AppendLine(UtenteProfiloCentriSql)

            Case enum_TipoSelect_FiltroneSuperNova.CentriAziendali_Visibilita_Appoggio   '3 'Centri Aziendali

                stb.AppendLine(UtenteProfiloImpreseSql)
                stb.AppendLine(UtenteProfiloCentriSql)

            Case enum_TipoSelect_FiltroneSuperNova.Appezzamenti  '4 'Appezzamenti

                If FiltroVisibilita AndAlso Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_C.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                stb.AppendLine(UtenteProfiloImpreseSql)
                stb.AppendLine(UtenteProfiloCentriSql)

                stb.AppendLine(" AND   Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                stb.AppendLine(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            Case enum_TipoSelect_FiltroneSuperNova.Impianti  '5 'Impianti

                If FiltroVisibilita AndAlso Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_C.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                stb.AppendLine(UtenteProfiloImpreseSql)
                stb.AppendLine(UtenteProfiloCentriSql)

                stb.AppendLine(" AND   Reg_Impianti.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                stb.AppendLine(" AND   Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            Case enum_TipoSelect_FiltroneSuperNova.Movimenti  '6 'Movimenti

                If FiltroVisibilita AndAlso Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    stb.AppendLine(" AND   Utenti_Visibilita_Appoggio_I.Username = " & Agro_SQL_SaveText_NULL(objParametri.UtenteUsername) & " ")
                End If

                stb.AppendLine(UtenteProfiloImpreseSql)
                stb.AppendLine(UtenteProfiloCentriSql)

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

                    stb.Append(" ORDER BY Imprese.Rag_Soc, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg, Agenda.Id_Agenda ASC" + vbCrLf) 'Importante

                Case 1

                    stb.Append(" GROUP BY dbo.Imprese.PIVA, Imprese.partitaIvaReale, dbo.Imprese.rag_soc, GerarchiaImprese.Foglia, GerarchiaImprese.Livello, GerarchiaImprese.Padre, GerarchiaImprese.Figlio, Imprese.TipoImpresaGerarchia    " + vbCrLf)

                Case 4 'Appezzamenti

                    stb.Append(" ORDER BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc  " + vbCrLf)

                Case 5 'Impianti (Ordinamento per chiave in modo tale da facilitare l'esclusione dei duplicati)

                    'MODIFICA IN DATA 15/06/2012: richiesta di Terremerse: ordinare le aziende in ordine alfabetico (rag_soc)
                    'StrSQL.Append(" ORDER BY Imprese.Piva, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg  " + vbCrLf)
                    stb.Append(" ORDER BY Imprese.rag_soc, Centri_Aziendali.Sa_Cod, Appezzamento.Appezza, Reg_Impianti.Id_Reg  " + vbCrLf)

                Case 8 'Campi

                    stb.Append(" ORDER BY Imprese.Piva, Centri_Aziendali.Sa_Cod, Campi.Campo_Cod " + vbCrLf)

                Case Else

                    stb.Append(" ORDER BY dbo.Imprese.PIVA, dbo.Imprese.rag_soc  " + vbCrLf)

            End Select

        Else

            stb.Append(" " & Ordinamento + vbCrLf)

        End If


        Return stb.ToString()

    End Function

    '###############################################################################
    'E' l'equivalente del COM di Agro_Anagrafe_AD - UtentixImprese_Read - FiltroneSuperNova
    Public Function CreaDTFiltrone(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                   ByVal Filtro As String, _
                                   ByVal TipoSelect As Integer, _
                                   ByVal Ordinamento As String, _
                                   ByVal Join As JoinFiltrone) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtility.Filtrone.CreaDTFiltrone()"

        '----- Descrizione
        Dim DescrizioneFunzione As String = "CreaDTFiltrone"
        Dim MessaggioErrore As String = ""
        Dim DT As DataTable


        Try

            Dim StrSQL As String = CreaStringaQueryPerDTFiltrone(objParametri, Filtro, TipoSelect, Ordinamento, Join, True)

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



    Public Function ImpostaVariabiliJOIN(ByRef Join As JoinFiltrone)

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

    End Function





End Class


Public Class JoinFiltrone

    Public bGerarchiaImprese As Boolean
    Public bImpreseXIndirizzi As Boolean
    Public bIstat As Boolean
    Public bListaProvince As Boolean
    Public bListaRegioni As Boolean
    Public bIndirizzi As Boolean
    Public bCentriAziendali As Boolean
    Public bCentrixIndirizzi As Boolean
    Public bIndirizziCentro As Boolean
    Public bIstatCentro As Boolean
    Public bListaProvinceCentro As Boolean
    Public bAppezzamento As Boolean
    Public bRegImpianti As Boolean
    Public bImpreseProgetti As Boolean
    Public bImpreseCodici As Boolean
    Public bImpreseCodici_CodiceSocio As Boolean
    Public bImpreseCodici_CodiceCuaa As Boolean
    Public bCentriAziendaliCodici As Boolean
    Public bAppezzamentoCodici As Boolean
    Public bRegImpiantiCodici As Boolean
    Public bCampi As Boolean
    Public bAppezzamentixParticelle As Boolean
    Public bGruppoVarietale As Boolean
    Public bCultivar As Boolean
    Public bSpecieVegetali As Boolean
    Public bGruppoVegetale As Boolean

    Public bMovDestinazioni As Boolean
    Public bAgenda As Boolean
    Public bMovimenti As Boolean
    Public bMovDettaglioTecnico As Boolean
    Public bMovimentiDettagli As Boolean
    Public bOperazioni As Boolean
    Public bGruppoOperazioni As Boolean
    Public bRisorseUmane As Boolean
    Public bContatti As Boolean
    Public bRapportiContabili As Boolean
    Public bPraticheStati As Boolean

    Public bGis_CentriAziendali As Boolean
    Public bGis_Appezzamento As Boolean
    Public bGis_Appezzamento_TipoEntitaCheSurrogaDAti As enum_GIS2012_TipoEntita

    Public Sub New()

        bGerarchiaImprese = False
        bImpreseXIndirizzi = False
        bIstat = False
        bListaProvince = False
        bListaRegioni = False
        bIndirizzi = False
        bCentriAziendali = False
        bCentrixIndirizzi = False
        bIndirizziCentro = False
        bIstatCentro = False
        bListaProvinceCentro = False
        bAppezzamento = False
        bRegImpianti = False
        bImpreseProgetti = False
        bImpreseCodici = False
        bImpreseCodici_CodiceSocio = False
        bImpreseCodici_CodiceCuaa = False
        bCentriAziendaliCodici = False
        bAppezzamentoCodici = False
        bRegImpiantiCodici = False
        bCampi = False
        bAppezzamentixParticelle = False
        bGruppoVarietale = False
        bCultivar = False
        bSpecieVegetali = False
        bGruppoVegetale = False

        bMovDestinazioni = False
        bAgenda = False
        bMovimenti = False
        bMovDettaglioTecnico = False
        bMovimentiDettagli = False
        bOperazioni = False
        bGruppoOperazioni = False
        bRisorseUmane = False
        bContatti = False
        bRapportiContabili = False
        bPraticheStati = False

        bGis_CentriAziendali = False
        bGis_Appezzamento = False

        bGis_Appezzamento_TipoEntitaCheSurrogaDAti = enum_GIS2012_TipoEntita.NON_DEFINITO
    End Sub




End Class
