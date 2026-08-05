Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Text
Imports AgronicaCoreDataProvider.DataProviderExtensions


Public Class Materie_Prime_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '############################################################################
    Public Function MateriePrime_DescrizioniOP(ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Elem_Cod As String,
                                                ByVal RicercaTesto As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.MateriePrime_DescrizioniOP()"
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.AppendLine("SELECT DISTINCT piva, sa_cod, cod_articolo, mat_des ")
            stb.AppendLine(" FROM Materie_Prime ")
            stb.AppendLine(" WHERE Materie_Prime.Elem_Cod IN (" & Agro_SQL_Save_Clausola_IN(Elem_Cod) & ") ")

            If Piva <> "" Then
                stb.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
            Else
                stb.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
            End If

            If RicercaTesto <> "" Then
                stb.AppendLine(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                stb.AppendLine(" ORDER BY Materie_Prime.Mat_Des Asc ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    ''''############################################################################
    Public Function MateriePrime_ProdottiConferiti(ByVal Piva As String,
                                                   ByVal Sa_Cod As Integer,
                                                   ByVal Elem_Cod As Integer,
                                                   ByVal Mat_Cod As Integer,
                                                   ByVal Cod_Articolo As String,
                                                   ByVal Veg_Cod As Integer,
                                                   ByVal Cul_Cod As Integer,
                                                   ByVal RicercaTesto As String,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                   ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                   ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.MateriePrime_ProdottiConferiti()"
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine("SELECT   Materie_Prime.*, ")
            stb.AppendLine("        CategorieMagazzino.NomeComune AS CategoriaMagazzino ")
            stb.AppendLine("  , SpecieVegetali.Veg_Des, Cultivar.Cul_Des ")

            stb.AppendLine(" FROM Materie_Prime ")

            'Join con CategorieMagazzino
            stb.AppendLine(" INNER JOIN  CategorieMagazzino ON Materie_Prime.Elem_Cod = CategorieMagazzino.Elem_Cod  ")

            'join con UtentiXImprese
            stb.AppendLine(" INNER  JOIN UtentiXImprese ON Materie_Prime.Piva = UtentiXImprese.PIVA ")

            'Join con SpecieVegetali
            stb.AppendLine("  LEFT OUTER JOIN SpecieVegetali ON Materie_Prime.Veg_Cod = SpecieVegetali.Veg_Cod ")

            'Join con Cultivar
            stb.AppendLine("  LEFT OUTER JOIN Cultivar ON Materie_Prime.Cul_Cod = Cultivar.Cul_Cod ")

            'WHERE
            stb.AppendLine(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri_Server.PivaSuperUser) & "'")

            'stb.AppendLine(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            'stb.AppendLine(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            stb.AppendLine(Recupera_FiltroMateriePrime(enum_Impostazioni_Utenti.SUPERUSER_COD_FILTRO_MATERIE_PRIME, Piva, objParametri_Utenti))

            If Piva <> "" Then
                stb.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
            Else
                stb.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
            End If

            If RicercaTesto <> "" Then
                stb.AppendLine(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%' ")
            End If

            If Cod_Articolo <> "" Then
                stb.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
            End If

            If Elem_Cod <> 0 Then
                stb.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            End If

            If Mat_Cod <> 0 Then
                stb.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            End If


            If Veg_Cod <> 0 Then
                stb.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Cul_Cod <> 0 Then
                stb.AppendLine(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                stb.AppendLine(" ORDER BY CategoriaMagazzino, Materie_Prime.Mat_Des Asc ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    '############################################################################
    'default: 
    'Flag_AncheImportatati As Boolean = False
    Public Function MateriePrime_Anagrafica(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Elem_Cod As Integer,
                                            ByVal Mat_Cod As Integer,
                                            ByVal Cod_Articolo As String,
                                            ByVal Veg_Cod As Integer,
                                            ByVal Cul_Cod As Integer,
                                            ByVal Gen_Cod As Integer,
                                            ByVal Spe_Cod As Integer,
                                            ByVal Raz_Cod As Integer,
                                            ByVal Ipro_Cod As Integer,
                                            ByVal Cat_Cod As Integer,
                                            ByVal RicercaTesto As String,
                                            ByVal Mat_Cod_Origine As Integer,
                                            ByVal Piva_SuperUser_Origine As String,
                                            ByVal Flag_AncheImportatati As Boolean,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal gruppiMerceDefaultPerCategoria As List(Of ImpostazioneDefault_GruppiMerce) = Nothing,
                                            Optional ByVal inibisciVisibilitaGruppiMerce As Boolean = False) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.MateriePrime_Anagrafica()"
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable
        Dim Flag_Vegetale As Boolean = False
        Dim Flag_Animale As Boolean = False

        Dim NomeDB_Utenti As String
        NomeDB_Utenti = objParametri_Utenti.StringaConnessione.Split(";")(2)
        NomeDB_Utenti = NomeDB_Utenti.Split("=")(1)

        Try

            stb.Length = 0

            'gias2gias

            'utenti x imprese

            Dim objGruppiMerce As New Gruppi_Merce_R
            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                Dim strSql = objGruppiMerce.ComponiSql_CreaTempDefaultGruppiMerce(New List(Of String) From {Piva}, objParametri_Server, gruppiMerceDefaultPerCategoria)

                stb.Append(strSql.ToString())
                stb.AppendLine("")
            End If

            stb.AppendLine("SELECT   Materie_Prime.*, ")
            stb.AppendLine("        CategorieMagazzino.NomeComune AS CategoriaMagazzino ")
            stb.AppendLine("        ,Imprese.Rag_Soc AS Rag_Soc_Proprietaria ")
            stb.AppendLine("        ,LTRIM(ISNULL(user_creazione.cognome, '') +  ' ' + ISNULL(user_creazione.nome, '')) AS Utente_Creazione ")
            stb.AppendLine("        ,LTRIM(ISNULL(user_modifica.cognome, '') +  ' ' + ISNULL(user_modifica.nome, '')) AS Utente_Modifica ")
            stb.AppendLine(" ,CASE WHEN (Materie_Prime.ChkReferenza = 1 Or Materie_Prime.Mat_Cod_Referenza <> 0) THEN 1 ELSE 0 END  AS LegatoALinea")
            stb.AppendLine(" ,CASE WHEN Materie_Prime.Elem_Cod != " & CStr(TRASFORMATI_VEGETALI) & "")
            stb.AppendLine("  THEN 0 ELSE CASE WHEN Materie_Prime.Mat_Cod_Referenza != 0 ")
            stb.AppendLine("     THEN Materie_Prime.Mat_Cod_Referenza ELSE Materie_Prime.Mat_Cod  END  END AS Mat_Cod_OMNI")
            stb.AppendLine(" ,CASE WHEN Materie_Prime.Elem_Cod != " & CStr(BENI_CONFEZ_VEGETALE) & "")
            stb.AppendLine("  THEN 0 ELSE   Materie_Prime.Qta_Extra END AS Qta_Extra ")
            stb.AppendLine(" ,CASE WHEN Materie_Prime.Elem_Cod != " & CStr(BENI_CONFEZ_VEGETALE) & "")
            stb.AppendLine("  THEN 0 ELSE   Materie_Prime.Udm_Cod_Extra END AS Udm_Cod_Extra ")

            'stb.AppendLine("        , Materie_PrimexLotto_Configurazione.Lotto_Cod, Materie_PrimexLotto_Configurazione.ChkListini, Materie_PrimexLotto_Configurazione.ChkReport, ")
            'stb.AppendLine("         Lotto_Configurazione.Lotto_Des, Materie_PrimexLotto_Configurazione.Cifra_Start, Materie_PrimexLotto_Configurazione.Cifra_End, ")
            'stb.AppendLine("        Lotto_Configurazione_Alias.Lotto_Val, Lotto_Configurazione_Alias.Lotto_Alias ")

            Select Case Elem_Cod

                Case 0
                    Flag_Vegetale = True
                    Flag_Animale = True
                    stb.AppendLine(" , SpecieVegetali.Veg_Des, Cultivar.Cul_Des,  ")
                    stb.AppendLine("  Lista_Generi_Animali.GEN_DES, Lista_Specie_Animali.SPE_DES, Lista_Razze_Animali.RAZ_DES, Lista_IndirizziProd_Animali.IPRO_DES,  ")
                    stb.AppendLine(" Lista_Categorie_Animali.CAT_DES, Lista_Categorie_Animali.SESSO, Lista_Categorie_Animali.ETA_GG_DA, Lista_Categorie_Animali.ETA_GG_A, ")
                    stb.AppendLine("  Lista_Categorie_Animali.PESO_KG_DA, Lista_Categorie_Animali.PESO_KG_A, Lista_Categorie_Animali.COEFF_UBA ")

                Case SEMENTI, SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI, BENI_CONFEZ_VEGETALE, MATERIE_VEGETALI, ALTRE_MATERIE
                    Flag_Vegetale = True
                    stb.AppendLine("  , SpecieVegetali.Veg_Des, Cultivar.Cul_Des  ")

                Case ZOO_CONSISTENZA, SEMILAVORATI_ANIMALI, TRASFORMATI_ANIMALI, BENI_CONFEZ_ANIMALE, MATERIE_ANIMALI
                    Flag_Animale = True
                    stb.AppendLine(" , '' as Veg_Des")
                    stb.AppendLine(" , '' as Cul_Des")
                    stb.AppendLine("  , Lista_Generi_Animali.GEN_DES, Lista_Specie_Animali.SPE_DES, Lista_Razze_Animali.RAZ_DES, Lista_IndirizziProd_Animali.IPRO_DES,  ")
                    stb.AppendLine(" Lista_Categorie_Animali.CAT_DES, Lista_Categorie_Animali.SESSO, Lista_Categorie_Animali.ETA_GG_DA, Lista_Categorie_Animali.ETA_GG_A, ")
                    stb.AppendLine("  Lista_Categorie_Animali.PESO_KG_DA, Lista_Categorie_Animali.PESO_KG_A, Lista_Categorie_Animali.COEFF_UBA ")

                Case CAT_MAG_SERVIZI_PROFESSIONALI, CARBURANTI, MANGIMI, FARMACI, CONFEZIONI_PRODOTTI, RICAMBI, ALTRI_BENI_AMMORTIZZABILI
                    stb.AppendLine(" , '' as Veg_Des")
                    stb.AppendLine(" , '' as Cul_Des")

            End Select

            '"    UnitaMisuraExtra.UDM_DES AS Udm_Des_Extra, UnitaMisuraExtra.UDM_SIM AS Udm_Sim_Extra, UnitaMisuraStandard.UDM_DES AS Udm_Des_Standard, UnitaMisuraStandard.UDM_SIM AS Udm_Sim_Standard, " &
            'Imprese_Progetti.Progetto_Nome, Imprese_Progetti.Progetto_Des,
            '"    Materie_Prime_Campionature.Tipo, Materie_Prime_Campionature.Tipo_Cod,  " &
            'Materie_Prime_Campionature.Udm_Cod AS Udm_Cod_Camp, Materie_Prime_Campionature.Val_Cod, Materie_Prime_Campionature.Descrizione,
            'CalibriFrutti.CAL_DES,
            '  "  ISNULL(ACC_VetrinaProdotti.Id_Prodotto, 0) AS Id_Prodotto "

            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then
                stb.AppendLine(" ,COALESCE(grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) AS Id_Gruppo_Merce ")
                stb.AppendLine(" ,CASE ")
                stb.AppendLine(" WHEN grpMerce.Codice IS NOT NULL THEN grpMerce.Codice + ' ' + grpMerce.Descrizione ")
                stb.AppendLine(" WHEN #DefaultGruppiMerce.Elem_Cod IS NOT NULL THEN #DefaultGruppiMerce.Codice + ' ' + #DefaultGruppiMerce.Descrizione COLLATE DATABASE_DEFAULT ")
                stb.AppendLine(" ELSE '' END AS Des_Gruppo_Merce ")
            End If

            stb.AppendLine(" FROM Materie_Prime    ")

            'Join con CategorieMagazzino
            stb.AppendLine(" INNER JOIN  CategorieMagazzino ON Materie_Prime.Elem_Cod = CategorieMagazzino.Elem_Cod  ")

            'join con UtentiXImprese
            stb.AppendLine(" INNER  JOIN UtentiXImprese On Materie_Prime.Piva = UtentiXImprese.PIVA ")

            'stb.AppendLine(" LEFT OUTER JOIN Materie_PrimexLotto_Configurazione On Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod And Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod ")

            'stb.AppendLine(" LEFT OUTER JOIN Lotto_Configurazione On Materie_PrimexLotto_Configurazione.Piva_SuperUser = Lotto_Configurazione.Piva_SuperUser And ")
            'stb.AppendLine("                Materie_PrimexLotto_Configurazione.Piva = Lotto_Configurazione.Piva And ")
            'stb.AppendLine("                Materie_PrimexLotto_Configurazione.Elem_Cod = Lotto_Configurazione.Elem_Cod And ")
            'stb.AppendLine("                 Materie_PrimexLotto_Configurazione.Lotto_Cod = Lotto_Configurazione.Lotto_Cod ")

            'stb.AppendLine(" LEFT OUTER JOIN Lotto_Configurazione_Alias On Lotto_Configurazione_Alias.Piva_SuperUser = Lotto_Configurazione.Piva_SuperUser And ")
            'stb.AppendLine("                Lotto_Configurazione_Alias.Piva = Lotto_Configurazione.Piva And ")
            'stb.AppendLine("                Lotto_Configurazione_Alias.Elem_Cod = Lotto_Configurazione.Elem_Cod And ")
            'stb.AppendLine("                 Lotto_Configurazione_Alias.Lotto_Cod = Lotto_Configurazione.Lotto_Cod ")


            If Flag_Animale = True Then

                'Join con Lista_IndirizziProd_Animali 
                stb.AppendLine(" LEFT OUTER JOIN  Lista_IndirizziProd_Animali On Materie_Prime.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD And  ")
                stb.AppendLine("  Materie_Prime.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD And  ")
                stb.AppendLine(" Materie_Prime.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")

                'Join con Lista_Categorie_Animali
                stb.AppendLine(" LEFT OUTER JOIN Lista_Categorie_Animali On Materie_Prime.GEN_COD = Lista_Categorie_Animali.GEN_COD And  ")
                stb.AppendLine("   Materie_Prime.SPE_COD = Lista_Categorie_Animali.SPE_COD And Materie_Prime.IPRO_COD = Lista_Categorie_Animali.IPRO_COD And ")
                stb.AppendLine("  Materie_Prime.CAT_COD = Lista_Categorie_Animali.CAT_COD ")

                'Join con Lista_Generi_Animali 
                stb.AppendLine("  LEFT OUTER JOIN  Lista_Generi_Animali On Materie_Prime.GEN_COD = Lista_Generi_Animali.GEN_COD ")

                'Join con Lista_Razze_Animali
                stb.AppendLine("  LEFT OUTER JOIN  Lista_Razze_Animali On Materie_Prime.GEN_COD = Lista_Razze_Animali.GEN_COD And  ")
                stb.AppendLine("  Materie_Prime.SPE_COD = Lista_Razze_Animali.SPE_COD And Materie_Prime.RAZ_COD = Lista_Razze_Animali.RAZ_COD ")

                'Join con Lista_Specie_Animali 
                stb.AppendLine("  LEFT OUTER JOIN Lista_Specie_Animali On Materie_Prime.GEN_COD = Lista_Specie_Animali.GEN_COD And  ")
                stb.AppendLine("  Materie_Prime.SPE_COD = Lista_Specie_Animali.SPE_COD ")

            End If

            If Flag_Vegetale = True Then

                'Join con SpecieVegetali
                stb.AppendLine("  LEFT OUTER JOIN SpecieVegetali On Materie_Prime.Veg_Cod = SpecieVegetali.Veg_Cod ")

                'Join con Cultivar
                stb.AppendLine("  LEFT OUTER JOIN Cultivar On Materie_Prime.Cul_Cod = Cultivar.Cul_Cod ")

            End If


            'Join con Imprese per avere azienda proprietaria 
            stb.AppendLine("JOIN  Imprese On Materie_Prime.Piva = Imprese.PIVA ")

            'Left Join con Utenti per avere utente creazione 
            stb.AppendLine("LEFT JOIN  " & NomeDB_Utenti & ".dbo.Utenti_Dettagli As user_creazione On Materie_Prime.UserName_creazione = user_creazione.UserName ")

            'Left Join con Utenti per avere utente modifica 
            stb.AppendLine("LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli As user_modifica On Materie_Prime.UserName_modifica = user_modifica.UserName ")

            ''Join con Materie_Prime_Campionature 
            'StrSQL &= "LEFT OUTER JOIN  Materie_Prime_Campionature On Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature.Progressivo "

            ''Join con Imprese_Progetti 
            'StrSQL &= "  LEFT OUTER JOIN Imprese_Progetti On Movimenti_dettagli.Cod_Progetto = Imprese_Progetti.Progetto_Cod  "

            ''Join con CalibriFrutti
            'StrSQL &= "  LEFT OUTER JOIN CalibriFrutti On Movimenti_dettagli.Cal_Cod = CalibriFrutti.CAL_COD "

            ''Join con UnitaMisura
            'StrSQL &= "  INNER JOIN   UnitaMisura UnitaMisuraStandard On Movimenti_dettagli.Udm_Cod =  UnitaMisuraStandard.UDM_COD "

            ''Join con UnitaMisuraExtra
            'StrSQL &= "   LEFT OUTER JOIN  UnitaMisura UnitaMisuraExtra On Movimenti_dettagli.UDM_COD_EXTRA = UnitaMisuraExtra.UDM_COD "

            ''Join con Fabbricati
            'StrSQL &= " INNER JOIN   Fabbricati On Mov_Destinazioni.Piva = Fabbricati.PIVA And Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD And  " &
            '          " Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod "

            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                stb.AppendLine(" LEFT JOIN Prodotti_Extra_Privata as prodExtraPriv WITH (nolock) ")
                stb.AppendLine("              ON prodExtraPriv.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri_Server.PivaSuperUser) & "'")
                stb.AppendLine("       AND prodExtraPriv.Elem_Cod = CategorieMagazzino.Elem_Cod ")
                stb.AppendLine("       AND prodExtraPriv.Mat_Cod = Materie_Prime.Mat_Cod")
                stb.AppendLine(" LEFT JOIN Gruppi_Merce as grpMerce WITH (nolock)")
                stb.AppendLine("              ON grpMerce.Id_Gruppo_Merce = prodExtraPriv.Id_Gruppo_Merce")
                stb.AppendLine(" LEFT JOIN #DefaultGruppiMerce")
                stb.AppendLine("              ON #DefaultGruppiMerce.Elem_Cod = CategorieMagazzino.Elem_Cod")

            End If

            'WHERE
            stb.AppendLine(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri_Server.PivaSuperUser) & "'")

            stb.AppendLine(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            stb.AppendLine(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")

            stb.AppendLine(Recupera_FiltroMateriePrime(enum_Impostazioni_Utenti.SUPERUSER_COD_FILTRO_MATERIE_PRIME, Piva, objParametri_Utenti))

            If Piva <> "" Then
                stb.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
            Else
                stb.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
            End If

            'stb.AppendLine(" AND ( Materie_PrimexLotto_Configurazione.Piva_SuperUser = UtentiXImprese.[USER] )")

            If RicercaTesto <> "" Then
                stb.AppendLine(" AND (Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%'   ")
                '2/4/2020 aggiunta ricerca anche sul codice articolo
                stb.AppendLine(" OR Materie_Prime.Cod_Articolo like '%" & Agro_SQL_SaveText(RicercaTesto) & "%' )  ")
            End If

            If Cod_Articolo <> "" Then
                stb.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
            End If

            If Elem_Cod <> 0 Then
                stb.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                stb.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Flag_Vegetale = True Then

                If Veg_Cod <> 0 Then
                    stb.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                End If

                If Cul_Cod <> 0 Then
                    stb.AppendLine(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                End If

            End If

            If Flag_Animale = True Then

                If Gen_Cod <> 0 Then
                    stb.AppendLine(" AND Materie_Prime.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
                End If

                If Spe_Cod <> 0 Then
                    stb.AppendLine(" AND Materie_Prime.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
                End If

                If Raz_Cod <> 0 Then
                    stb.AppendLine(" AND Materie_Prime.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
                End If

                If Ipro_Cod <> 0 Then
                    stb.AppendLine(" AND Materie_Prime.Ipro_Cod = " & Agro_SQL_SaveNum(Ipro_Cod) & "   ")
                End If

                If Cat_Cod <> 0 Then
                    stb.AppendLine(" AND Materie_Prime.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & "   ")
                End If

            End If

            'GIAS 2 GIAS 
            If Mat_Cod_Origine <> 0 Then
                stb.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
            End If

            If Piva_SuperUser_Origine <> "" Then
                stb.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If

            If Flag_AncheImportatati = False Then
                stb.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
            End If
            'FINE GIAS 2 GIAS

            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 AndAlso
                objParametri_Server.UtenteUsername <> objParametri_Server.SuperUserUsername AndAlso
                inibisciVisibilitaGruppiMerce = False Then

                'Calcolo gestione visibilità gruppi merce
                Dim objGruppiUtenteMerce As New Gruppi_UtenteXGruppi_Merce_R(objParametri_Server, objParametri_Utenti)
                Dim dtGruppiUtenteMerce As DataTable = objGruppiUtenteMerce.Leggi("Gruppi_UtenteXGruppi_Merce.Piva IN('" & Piva & "')", "")

                If dtGruppiUtenteMerce.Rows.Count > 0 Then
                    stb.AppendLine(" AND COALESCE(grpMerce.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) IN (")
                    stb.AppendLine(objGruppiUtenteMerce.ComponiSql_DistinctGruppiMerce_X_GruppiUtente("", Piva))
                    stb.AppendLine(" )")
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                stb.AppendLine(" ORDER BY CategoriaMagazzino, Materie_Prime.Mat_Des Asc ")
            End If

            If Not IsNothing(gruppiMerceDefaultPerCategoria) AndAlso gruppiMerceDefaultPerCategoria.Count > 0 Then

                Dim strSql = objGruppiMerce.ComponiSql_CancellaTempDefaultGruppiMerce()

                stb.AppendLine("")
                stb.Append(strSql.ToString())

            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    ''###############################################################################
    ''Default:
    ''Cod_Progetto As Integer = CODPROGETTO_NONDEFINITO,
    ''ByVal Lotto As String = LOTTO_NONDEFINITO,
    '' ByVal Flag_AncheImportatati As Boolean = False
    ''------------------------------------------------
    ''Viene usata nell'E-Commerce e FormProdotto
    ''quindi devo filtrare sempre le materie prime importate con il g2g
    'Public Function MateriePrime_Giacenze(ByVal Piva As String,
    '                                      ByVal Sa_Cod As Integer,
    '                                      ByVal Elem_Cod As Integer,
    '                                      ByVal Pro_Cod As Integer,
    '                                      ByVal Mat_Cod As Integer,
    '                                      ByVal Udm_Cod As Integer,
    '                                      ByVal Id_Destinazione As Integer,
    '                                      ByVal Cal_Cod As Integer,
    '                                      ByVal Cod_Progetto As Integer,
    '                                      ByVal Fase_Cod As Integer,
    '                                      ByVal Lotto As String,
    '                                      ByVal Cod_Articolo As String,
    '                                      ByVal Veg_Cod As Integer,
    '                                      ByVal Cul_Cod As Integer,
    '                                      ByVal Gen_Cod As Integer,
    '                                      ByVal Spe_Cod As Integer,
    '                                      ByVal Raz_Cod As Integer,
    '                                      ByVal Ipro_Cod As Integer,
    '                                      ByVal Cat_Cod As Integer,
    '                                      ByVal RicercaNomeProdotto As String,
    '                                      ByVal RicercaLottoAccettazione As String,
    '                                      ByVal RicercaCodArticolo As String,
    '                                      ByVal Mat_Cod_Origine As Integer,
    '                                      ByVal Piva_SuperUser_Origine As String,
    '                                      ByVal Flag_AncheImportatati As Boolean,
    '                                      ByVal xFiltroAggiuntivo As String,
    '                                      ByVal xOrderBy As String,
    '                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                                      ) As DataTable


    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.MateriePrime_Giacenze()"
    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        '----------------------------------------------------
    '        '--- Preparo la Query SQL ---------------------------
    '        '----------------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.AppendLine("SELECT Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Prezzo_Unitario, Movimenti_dettagli.Cod_Progetto, Movimenti_dettagli.Cal_Cod,  ")
    '        StrSQL.AppendLine(" Movimenti_dettagli.Fase_Cod, Movimenti_dettagli.QTA_EXTRA, Movimenti_dettagli.UDM_COD_EXTRA, UnitaMisuraExtra.UDM_DES AS Udm_Des_Extra, UnitaMisuraExtra.UDM_SIM AS Udm_Sim_Extra, Movimenti_dettagli.Prezzo_Unitario_Netto, ")
    '        StrSQL.AppendLine(" Movimenti_dettagli.Imponibile_Netto, Movimenti_dettagli.Jolly_Int, Movimenti_dettagli.Lotto, Mov_Destinazioni.Qta AS Dest_Qta, ")
    '        StrSQL.AppendLine("  Movimenti_dettagli.Qta AS Dett_Qta, Materie_Prime.*, CategorieMagazzino.NomeComune AS CategoriaMagazzino, UnitaMisuraStandard.UDM_DES AS Udm_Des_Standard, UnitaMisuraStandard.UDM_SIM AS Udm_Sim_Standard, Fabbricati.Fabbricato_Des, Fabbricati.Tipo_Fabbricato_Cod, Appezzamento.APP_NOME,")

    '        StrSQL.AppendLine("   Imprese_Progetti.Progetto_Nome, Imprese_Progetti.Progetto_Des, Materie_Prime_Campionature.Tipo, Materie_Prime_Campionature.Tipo_Cod,  ")
    '        StrSQL.AppendLine(" Materie_Prime_Campionature.Udm_Cod AS Udm_Cod_Camp, Materie_Prime_Campionature.Val_Cod, Materie_Prime_Campionature.Descrizione, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, CalibriFrutti.CAL_DES, ")
    '        StrSQL.AppendLine("  Lista_Generi_Animali.GEN_DES, Lista_Specie_Animali.SPE_DES, Lista_Razze_Animali.RAZ_DES, Lista_IndirizziProd_Animali.IPRO_DES,  ")
    '        StrSQL.AppendLine(" Lista_Categorie_Animali.CAT_DES, Lista_Categorie_Animali.SESSO, Lista_Categorie_Animali.ETA_GG_DA, Lista_Categorie_Animali.ETA_GG_A, ")
    '        StrSQL.AppendLine("  Lista_Categorie_Animali.PESO_KG_DA, Lista_Categorie_Animali.PESO_KG_A, Lista_Categorie_Animali.COEFF_UBA, ")
    '        StrSQL.AppendLine("  ISNULL(ACC_VetrinaProdotti.Id_Prodotto, 0) AS Id_Prodotto ")

    '        'Join con Movimenti
    '        StrSQL.AppendLine(" FROM Agenda INNER JOIN ")
    '        StrSQL.AppendLine("  Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

    '        'Join con Movimenti_dettagli
    '        StrSQL.AppendLine(" INNER JOIN   Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND  ")
    '        StrSQL.AppendLine("  Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")

    '        'Join con Mov_Destinazioni
    '        StrSQL.AppendLine("  INNER JOIN   Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND  ")
    '        StrSQL.AppendLine(" Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND  ")
    '        StrSQL.AppendLine("  Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")

    '        'Join con Materie_Prime 'Movimenti_dettagli.PIVA = Materie_Prime.Piva AND
    '        StrSQL.AppendLine(" INNER JOIN  Materie_Prime ON   ")
    '        StrSQL.AppendLine("      Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod  ")

    '        'Join con Materie_Prime_Campionature 
    '        StrSQL.AppendLine("LEFT OUTER JOIN  Materie_Prime_Campionature ON Movimenti_dettagli.Cal_Cod = Materie_Prime_Campionature.Progressivo ")

    '        'Join con Imprese_Progetti 
    '        StrSQL.AppendLine("  LEFT OUTER JOIN Imprese_Progetti ON Movimenti_dettagli.Cod_Progetto = Imprese_Progetti.Progetto_Cod  ")

    '        'Join con Lista_IndirizziProd_Animali 
    '        StrSQL.AppendLine(" LEFT OUTER JOIN  Lista_IndirizziProd_Animali ON Materie_Prime.GEN_COD = Lista_IndirizziProd_Animali.GEN_COD AND  ")
    '        StrSQL.AppendLine("  Materie_Prime.SPE_COD = Lista_IndirizziProd_Animali.SPE_COD AND  ")
    '        StrSQL.AppendLine(" Materie_Prime.IPRO_COD = Lista_IndirizziProd_Animali.IPRO_COD  ")

    '        'Join con Lista_Categorie_Animali
    '        StrSQL.AppendLine(" LEFT OUTER JOIN Lista_Categorie_Animali ON Materie_Prime.GEN_COD = Lista_Categorie_Animali.GEN_COD AND  ")
    '        StrSQL.AppendLine("   Materie_Prime.SPE_COD = Lista_Categorie_Animali.SPE_COD AND Materie_Prime.IPRO_COD = Lista_Categorie_Animali.IPRO_COD AND ")
    '        StrSQL.AppendLine("  Materie_Prime.CAT_COD = Lista_Categorie_Animali.CAT_COD ")

    '        'Join con Lista_Generi_Animali 
    '        StrSQL.AppendLine("  LEFT OUTER JOIN  Lista_Generi_Animali ON Materie_Prime.GEN_COD = Lista_Generi_Animali.GEN_COD ")

    '        'Join con Lista_Razze_Animali
    '        StrSQL.AppendLine("  LEFT OUTER JOIN  Lista_Razze_Animali ON Materie_Prime.GEN_COD = Lista_Razze_Animali.GEN_COD AND  ")
    '        StrSQL.AppendLine("  Materie_Prime.SPE_COD = Lista_Razze_Animali.SPE_COD AND Materie_Prime.RAZ_COD = Lista_Razze_Animali.RAZ_COD ")

    '        'Join con Lista_Specie_Animali 
    '        StrSQL.AppendLine("  LEFT OUTER JOIN Lista_Specie_Animali ON Materie_Prime.GEN_COD = Lista_Specie_Animali.GEN_COD AND  ")
    '        StrSQL.AppendLine("  Materie_Prime.SPE_COD = Lista_Specie_Animali.SPE_COD ")

    '        'Join con CalibriFrutti
    '        StrSQL.AppendLine("  LEFT OUTER JOIN CalibriFrutti ON Movimenti_dettagli.Cal_Cod = CalibriFrutti.CAL_COD ")

    '        'Join con SpecieVegetali
    '        StrSQL.AppendLine("  LEFT OUTER JOIN SpecieVegetali ON Materie_Prime.Veg_Cod = SpecieVegetali.Veg_Cod ")

    '        'Join con Cultivar
    '        StrSQL.AppendLine("  LEFT OUTER JOIN Cultivar ON Materie_Prime.Cul_Cod = Cultivar.Cul_Cod ")

    '        'Join con UnitaMisura
    '        StrSQL.AppendLine("  INNER JOIN   UnitaMisura UnitaMisuraStandard ON Movimenti_dettagli.Udm_Cod =  UnitaMisuraStandard.UDM_COD ")

    '        'Join con UnitaMisuraExtra
    '        StrSQL.AppendLine("   LEFT OUTER JOIN  UnitaMisura UnitaMisuraExtra ON Movimenti_dettagli.UDM_COD_EXTRA = UnitaMisuraExtra.UDM_COD ")

    '        'Join con CategorieMagazzino
    '        StrSQL.AppendLine(" INNER JOIN  CategorieMagazzino ON Materie_Prime.Elem_Cod = CategorieMagazzino.Elem_Cod  ")

    '        'Join con Fabbricati
    '        StrSQL.AppendLine(" INNER JOIN   Fabbricati ON Mov_Destinazioni.Piva = Fabbricati.PIVA AND Mov_Destinazioni.Sa_Cod = Fabbricati.SA_COD AND  ")
    '        StrSQL.AppendLine(" Mov_Destinazioni.Id_Destinazione = Fabbricati.Fabbricato_Cod ")

    '        'Join con Fabbricati
    '        StrSQL.AppendLine("  LEFT OUTER JOIN   Appezzamento ON Imprese_Progetti.Piva = Appezzamento.PIVA AND Imprese_Progetti.Sa_Cod = Appezzamento.SA_COD AND   ")
    '        StrSQL.AppendLine("  Imprese_Progetti.Appezza = Appezzamento.APPEZZA ")

    '        'Join con ACC_VetrinaProdotti
    '        StrSQL.AppendLine("   LEFT OUTER JOIN   ACC_VetrinaProdotti ON Movimenti_dettagli.PIVA = ACC_VetrinaProdotti.Piva AND Movimenti_dettagli.Elem_Cod = ACC_VetrinaProdotti.Elem_Cod AND    ")
    '        StrSQL.AppendLine("    Movimenti_dettagli.Mat_Cod = ACC_VetrinaProdotti.Mat_Cod AND Movimenti_dettagli.Cod_Progetto = ACC_VetrinaProdotti.Cod_Progetto AND    ")
    '        StrSQL.AppendLine("     Movimenti_dettagli.Lotto = ACC_VetrinaProdotti.Lotto AND Movimenti_dettagli.Cal_Cod = ACC_VetrinaProdotti.Cal_Cod AND    ")
    '        StrSQL.AppendLine("   Movimenti_dettagli.Udm_Cod = ACC_VetrinaProdotti.Udm_Cod AND    ")
    '        StrSQL.AppendLine(" Movimenti_dettagli.UDM_COD_EXTRA = ACC_VetrinaProdotti.Udm_Cod_Extra  ")

    '        'WHERE
    '        StrSQL.AppendLine(" WHERE Movimenti_Dettagli.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
    '        StrSQL.AppendLine(" AND   Movimenti_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
    '        StrSQL.AppendLine(" AND   Agenda.Lav_Cod = -1    ")
    '        StrSQL.AppendLine(" AND   Movimenti.Cau_Mov = 'GIACENZE'    ")
    '        StrSQL.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione = 20   ")
    '        StrSQL.AppendLine(" AND   Materie_Prime.Mat_Cod_Origine = 0   ")

    '        If RicercaNomeProdotto <> "" Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaNomeProdotto) & "%'   ")
    '        End If

    '        If RicercaLottoAccettazione <> "" Then
    '            StrSQL.AppendLine(" AND Movimenti_Dettagli.Lotto like '%" & Agro_SQL_SaveText(RicercaLottoAccettazione) & "%'   ")
    '        End If

    '        If RicercaCodArticolo <> "" Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Cod_Articolo like '%" & Agro_SQL_SaveText(RicercaCodArticolo) & "%'   ")
    '        End If

    '        If Lotto <> LOTTO_NONDEFINITO Then
    '            StrSQL.AppendLine(" AND Movimenti_Dettagli.Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
    '        End If

    '        If Cod_Articolo <> "" Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "'   ")
    '        End If

    '        If Piva <> "" Then
    '            StrSQL.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
    '        End If

    '        If Sa_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
    '        End If

    '        If Elem_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Movimenti_Dettagli.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
    '        End If

    '        'If Pro_Cod <> 0 Then
    '        StrSQL.AppendLine(" AND Movimenti_Dettagli.Pro_Cod = " & Agro_SQL_SaveNum(0) & "   ")
    '        'End If

    '        If Mat_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Movimenti_Dettagli.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
    '        End If

    '        If Udm_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Movimenti_Dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
    '        End If

    '        If Id_Destinazione <> 0 Then
    '            StrSQL.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Destinazione) & "   ")
    '        End If

    '        If Cal_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Movimenti_Dettagli.Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & "   ")
    '        End If

    '        'If Flag_FiltraCodProgetto0 = False Then
    '        '    If Cod_Progetto <> 0 Then
    '        '         StrSQL.AppendLine( " AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
    '        '    End If
    '        'Else
    '        '     StrSQL.AppendLine( " AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
    '        'End If
    '        If Cod_Progetto <> CODPROGETTO_NONDEFINITO Then
    '            StrSQL.AppendLine(" AND Movimenti_Dettagli.Cod_Progetto = " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
    '        End If

    '        If Fase_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Movimenti_Dettagli.Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
    '        End If

    '        If Veg_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
    '        End If

    '        If Cul_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
    '        End If

    '        If Gen_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
    '        End If

    '        If Spe_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
    '        End If

    '        If Raz_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
    '        End If

    '        If Ipro_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Ipro_Cod = " & Agro_SQL_SaveNum(Ipro_Cod) & "   ")
    '        End If

    '        If Cat_Cod <> 0 Then
    '            StrSQL.AppendLine(" AND Materie_Prime.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & "   " )
    '        End If

    '        If Mat_Cod_Origine <> 0 Then
    '            StrSQL.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
    '        End If

    '        If Piva_SuperUser_Origine <> "" Then
    '            StrSQL.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
    '        End If

    '        If Flag_AncheImportatati = False Then
    '            StrSQL.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
    '        End If

    '        If xFiltroAggiuntivo <> "" Then
    '            strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If


    '        '--------------------------------------------------------------------------
    '        Select Case objParametri.FlagVisibilita
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                StrSQL.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
    '                StrSQL.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                '...................................
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select
    '        '--------------------------------------------------------------------------
    '        If xOrderBy <> "" Then
    '            strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '        Else
    '            StrSQL.AppendLine(" ORDER BY CategoriaMagazzino, Materie_Prime.Mat_Des Asc ")
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



    Public Function LeggiMateriePrimeDaOrigine(ByVal Piva As String,
                                               ByVal Elem_Cod As Integer,
                                               ByVal Mat_Cod As Integer,
                                               ByVal Mat_Cod_Origine As Integer,
                                               ByVal Piva_SuperUser_Origine As String,
                                               ByVal Flag_AncheImportati As Boolean,
                                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.LeggiMateriePrimeDaOrigine()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Elem_Cod = 0            
        '   Mat_Cod = 0           
        '   Mat_Cod_Origine = 0  =
        '   Piva_SuperUser_Origine = ""  
        '   Flag_AncheImportati = True 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0

                    'Nota: E' necessario l'outer join per ricavare il proprietario del prodotto aziendale poiché
                    'in versione standalone l'impresa referente potrebbe non essere stata importata.

                    strSql.AppendLine(" SELECT Materie_Prime.*, Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Materie_Prime LEFT OUTER JOIN Imprese ON Materie_Prime.Piva = Imprese.Piva ")
                    strSql.AppendLine(" WHERE  Materie_Prime.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    strSql.AppendLine(" AND    Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Mat_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
                    End If


                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportati <> True Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Materie_Prime.Mat_Cod ASC, Materie_Prime.Elem_Cod ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0

                    'Nota: E' necessario l'outer join per ricavare il proprietario del prodotto aziendale poiché
                    'in versione standalone l'impresa referente potrebbe non essere stata importata.

                    strSql.AppendLine(" SELECT Materie_Prime.*, Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Materie_Prime LEFT OUTER JOIN Imprese ON Materie_Prime.Piva = Imprese.Piva ")
                    strSql.AppendLine(" WHERE  Materie_Prime.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    strSql.AppendLine(" AND    Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If Piva <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Mat_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
                    End If


                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportati <> True Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Materie_Prime.Mat_Cod ASC, Materie_Prime.Elem_Cod ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '
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


    '##############################################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Elem_Cod As Integer,
                          ByVal Mat_Cod As Integer,
                          ByVal Cod_Articolo As String,
                          ByVal Veg_Cod As Integer,
                          ByVal Cul_Cod As Integer,
                          ByVal Gen_Cod As Integer,
                          ByVal Spe_Cod As Integer,
                          ByVal Raz_Cod As Integer,
                          ByVal Ipro_Cod As Integer,
                          ByVal Cat_Cod As Integer,
                          ByVal RicercaTesto As String,
                          ByVal Mat_Cod_Origine As Integer,
                          ByVal Piva_SuperUser_Origine As String,
                          ByVal Flag_AncheImportatati As Boolean,
                          ByVal Flag_MateriePrimeSoloPrivate As Boolean,
                          ByVal FiltroMateriePrime As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              Optional ByVal TipoG2G As Integer = 0,
                              Optional ByVal PivaDestinazione As String = ""
                          ) As DataTable

        'default Flag_MateriePrimeSoloPrivate = false

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                'Selezione_JoinDescrizioni riga 1346

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    strSql.AppendLine(" SELECT   Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Materie_Prime.Cod_Articolo, Materie_Prime.Mat_des, Materie_Prime.Regolamento  ")
                    strSql.AppendLine(" FROM Materie_Prime WITH(NOLOCK)")
                    strSql.AppendLine(" INNER  JOIN UtentiXImprese WITH(NOLOCK) ON Materie_Prime.Piva = UtentiXImprese.PIVA ")
                    strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri.PivaSuperUser) & "'")
                    strSql.AppendLine(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Flag_MateriePrimeSoloPrivate = False Then
                        If Piva <> "" Then
                            strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
                        Else
                            strSql.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
                        End If
                    Else
                        strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' )")
                    End If

                    If RicercaTesto <> "" Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%'   ")
                    End If

                    If Cod_Articolo <> "" Then
                        strSql.AppendLine(" AND (Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ) ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cul_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                    End If

                    If Gen_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
                    End If

                    If Spe_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
                    End If

                    If Raz_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
                    End If

                    If Ipro_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Ipro_Cod = " & Agro_SQL_SaveNum(Ipro_Cod) & "   ")
                    End If

                    If Cat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & "   ")
                    End If

                    If Mat_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportatati = False Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            strSql.AppendLine(" AND not exists ( ")
                            strSql.AppendLine(" select 1 from g2g_Recode_MateriePrime rr WITH(NOLOCK) where rr.From_Mat_Cod = Materie_Prime.Mat_Cod and rr.To_Piva = '" & Agro_SQL_SaveText(PivaDestinazione) & "'")
                            strSql.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            strSql.AppendLine("and exists ( ")
                            strSql.AppendLine("    select 1 ")
                            strSql.AppendLine("    from g2g_Recode_MateriePrime rr WITH(NOLOCK)")
                            strSql.AppendLine("    where rr.DataInvio < Materie_Prime.Data_Modifica ")
                            strSql.AppendLine("    and rr.To_Piva = '" & Agro_SQL_SaveText(PivaDestinazione) & "'")
                            strSql.AppendLine("    and rr.From_Mat_Cod = Materie_Prime.Mat_Cod  ")
                            strSql.AppendLine(" ) ")

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine("  ORDER BY Materie_Prime.Mat_Des Asc ")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    strSql.AppendLine(" SELECT   Materie_Prime.* ")
                    strSql.AppendLine(" FROM Materie_Prime WITH(NOLOCK)")
                    strSql.AppendLine(" INNER  JOIN UtentiXImprese WITH(NOLOCK) ON Materie_Prime.Piva = UtentiXImprese.PIVA ")
                    strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri.PivaSuperUser) & "'")
                    strSql.AppendLine(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If (FiltroMateriePrime <> "") Then
                        strSql.AppendLine(FiltroMateriePrime)
                    End If

                    If Flag_MateriePrimeSoloPrivate = False Then
                        If Piva <> "" Then
                            strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
                        Else
                            strSql.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
                        End If
                    Else
                        strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' )")
                    End If

                    If RicercaTesto <> "" Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%'   ")
                    End If

                    If Cod_Articolo <> "" Then
                        strSql.AppendLine(" AND (Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "') ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cul_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                    End If

                    If Gen_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
                    End If

                    If Spe_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
                    End If

                    If Raz_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
                    End If

                    If Ipro_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Ipro_Cod = " & Agro_SQL_SaveNum(Ipro_Cod) & "   ")
                    End If

                    If Cat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & "   ")
                    End If

                    If Mat_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportatati = False Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            strSql.AppendLine(" AND not exists ( ")
                            strSql.AppendLine(" select 1 from g2g_Recode_MateriePrime rr WITH(NOLOCK) where rr.From_Mat_Cod = Materie_Prime.Mat_Cod and rr.To_Piva = '" & Agro_SQL_SaveText(PivaDestinazione) & "'")
                            strSql.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            strSql.AppendLine("and exists ( ")
                            strSql.AppendLine("    select 1 ")
                            strSql.AppendLine("    from g2g_Recode_MateriePrime rr WITH(NOLOCK) ")
                            strSql.AppendLine("    where rr.DataInvio < Materie_Prime.Data_Modifica ")
                            strSql.AppendLine("    and rr.To_Piva = '" & Agro_SQL_SaveText(PivaDestinazione) & "'")
                            strSql.AppendLine("    and rr.From_Mat_Cod = Materie_Prime.Mat_Cod  ")
                            strSql.AppendLine(" ) ")

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine("  ORDER BY Materie_Prime.Mat_Des Asc ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    strSql.AppendLine(" SELECT   Materie_Prime.* ")
                    strSql.AppendLine(" FROM Materie_Prime WITH(NOLOCK) ")
                    strSql.AppendLine(" INNER  JOIN UtentiXImprese WITH(NOLOCK) ON Materie_Prime.Piva = UtentiXImprese.PIVA ")
                    strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri.PivaSuperUser) & "'")
                    strSql.AppendLine(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If (FiltroMateriePrime <> "") Then
                        strSql.AppendLine(FiltroMateriePrime)
                    End If

                    If Flag_MateriePrimeSoloPrivate = False Then
                        If Piva <> "" Then
                            strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
                        Else
                            strSql.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
                        End If
                    Else
                        strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' )")
                    End If

                    If RicercaTesto <> "" Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%'   ")
                    End If

                    If Cod_Articolo <> "" Then
                        strSql.AppendLine(" AND (Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cul_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                    End If

                    If Gen_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
                    End If

                    If Spe_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
                    End If

                    If Raz_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
                    End If

                    If Ipro_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Ipro_Cod = " & Agro_SQL_SaveNum(Ipro_Cod) & "   ")
                    End If

                    If Cat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & "   ")
                    End If

                    If Mat_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportatati = False Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            strSql.AppendLine(" AND not exists ( ")
                            strSql.AppendLine(" select 1 from g2g_Recode_MateriePrime rr WITH(NOLOCK) where rr.From_Mat_Cod = Materie_Prime.Mat_Cod and rr.To_Piva = '" & Agro_SQL_SaveText(PivaDestinazione) & "'")
                            strSql.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            strSql.AppendLine("and exists ( ")
                            strSql.AppendLine("    select 1 ")
                            strSql.AppendLine("    from g2g_Recode_MateriePrime rr WITH(NOLOCK) ")
                            strSql.AppendLine("    where rr.DataInvio < Materie_Prime.Data_Modifica ")
                            strSql.AppendLine("    and rr.To_Piva = '" & Agro_SQL_SaveText(PivaDestinazione) & "'")
                            strSql.AppendLine("    and rr.From_Mat_Cod = Materie_Prime.Mat_Cod  ")
                            strSql.AppendLine(" ) ")

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine("  ORDER BY Materie_Prime.Mat_Des Asc ")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    'utilizzata ad esempio dall'importazione articoli di Fruttagel (Importazione_Articoli_Fruttagel_JDE sul sincro)
                    'viene chiamata anche dai PDC
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")
                    strSql.AppendLine(" SELECT Materie_Prime.Piva, Materie_Prime.Elem_Cod, Materie_Prime.Cat_Cod, Materie_Prime.Mat_Cod, Materie_Prime.Cod_Articolo, Materie_Prime.Mat_Des, Materie_Prime.Sem_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Veg_Cod, Materie_Prime.Grva_Cod_Veg, ")
                    strSql.AppendLine(" SpecieVegetali.Veg_Des, Cultivar.Cul_Des, Materie_Prime.Regolamento, Materie_Prime.Sa_Cod, Materie_Prime.Note, Materie_Prime.Extra_Int, Materie_Prime.ID_DisciplinareAcquisti, ")
                    strSql.AppendLine(" Materie_Prime.codice_esterno, Materie_Prime.Flag_Importato, Materie_Prime.Validita_Inizio, Materie_Prime.Validita_Fine,  ")
                    strSql.AppendLine(" Materie_Prime.Grfi_Cod, COALESCE(GruppoFinalita.Grfi_Des, '') AS Grfi_Des ")
                    strSql.AppendLine(" FROM         Materie_Prime WITH(NOLOCK) ")
                    strSql.AppendLine(" INNER JOIN   UtentiXImprese WITH(NOLOCK) ON Materie_Prime.Piva = UtentiXImprese.PIVA  ")
                    strSql.AppendLine(" LEFT OUTER JOIN  SpecieVegetali WITH(NOLOCK) ON Materie_Prime.Veg_Cod = SpecieVegetali.Veg_Cod ")
                    strSql.AppendLine(" LEFT OUTER JOIN  Cultivar WITH(NOLOCK) ON Materie_Prime.Cul_Cod = Cultivar.Cul_Cod And Materie_Prime.Veg_Cod = Cultivar.Veg_Cod ")
                    strSql.AppendLine(" LEFT OUTER JOIN  GruppoFinalita WITH(NOLOCK) ON Materie_Prime.Grfi_Cod = GruppoFinalita.Grfi_Cod ")
                    strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri.PivaSuperUser) & "'")
                    strSql.AppendLine(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If (FiltroMateriePrime <> "") Then
                        strSql.AppendLine(FiltroMateriePrime)
                    End If

                    If Flag_MateriePrimeSoloPrivate = False Then
                        If Piva <> "" Then
                            strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
                        Else
                            strSql.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
                        End If
                    Else
                        strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' )")
                    End If

                    If RicercaTesto <> "" Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%'   ")
                    End If

                    If Cod_Articolo <> "" Then
                        strSql.AppendLine(" AND (Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cul_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                    End If

                    If Gen_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
                    End If

                    If Spe_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
                    End If

                    If Raz_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
                    End If

                    If Ipro_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Ipro_Cod = " & Agro_SQL_SaveNum(Ipro_Cod) & "   ")
                    End If

                    If Cat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & "   ")
                    End If

                    If Mat_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportatati = False Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            strSql.AppendLine(" AND not exists ( ")
                            strSql.AppendLine(" select 1 from g2g_Recode_MateriePrime rr WITH(NOLOCK) where rr.From_Mat_Cod = Materie_Prime.Mat_Cod and rr.To_Piva = '" & Agro_SQL_SaveText(PivaDestinazione) & "'")
                            strSql.AppendLine(" ) ")

                        Case 2 'seleziona i dati modificati
                            strSql.AppendLine("and exists ( ")
                            strSql.AppendLine("    select 1 ")
                            strSql.AppendLine("    from g2g_Recode_MateriePrime rr WITH(NOLOCK) ")
                            strSql.AppendLine("    where rr.DataInvio < Materie_Prime.Data_Modifica ")
                            strSql.AppendLine("    and rr.To_Piva = '" & Agro_SQL_SaveText(PivaDestinazione) & "'")
                            strSql.AppendLine("    and rr.From_Mat_Cod = Materie_Prime.Mat_Cod  ")
                            strSql.AppendLine(" ) ")

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine("  ORDER BY Materie_Prime.Mat_Des Asc ")
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



    Public Function Leggi_Beni_Confezionamento_Modulo(ByVal Piva As String,
                          ByVal Modulo_Generazione As Integer,
                          ByVal Flag_MateriePrimeSoloPrivate As Boolean,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0,
                          Optional ByVal PivaDestinazione As String = ""
                          ) As DataTable

        'default Flag_MateriePrimeSoloPrivate = false

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi_Beni_Confezionamento_Modulo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0
            strSql.AppendLine(" SELECT   Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Materie_Prime.Cod_Articolo, Materie_Prime.Tara, Materie_Prime.Mat_des as Mat_Des, IsNull(Tabella_Cod, 0) as Tabella_Cod ")
            strSql.AppendLine(" FROM Materie_Prime ")
            strSql.AppendLine(" INNER  JOIN UtentiXImprese ON Materie_Prime.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" Left Outer JOIN oTabelle_Parametri ON Materie_Prime.Mat_Cod = oTabelle_Parametri.Mat_Cod_Generazione_Link ")

            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(" AND Materie_Prime.Elem_Cod In (205, 305) ")
            strSql.AppendLine(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Flag_MateriePrimeSoloPrivate = False Then
                If Piva <> "" Then
                    strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
                Else
                    strSql.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
                End If
            Else
                strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' )")
            End If


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine("  ORDER BY Materie_Prime.Mat_Des Asc ")
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

    '##############################################################################################
    Public Function Mat_Cod_Leggi(ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal Elem_Cod As Integer,
                                  ByVal Cod_Articolo As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Mat_Cod_Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" SELECT   Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Materie_Prime.Cod_Articolo, Materie_Prime.Mat_des, Materie_Prime.Regolamento  ")
            strSql.AppendLine(" FROM Materie_Prime ")
            strSql.AppendLine(" where  1=1 ")

            strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' )")

            strSql.AppendLine(" AND (Materie_Prime.Sa_Cod = '" & Agro_SQL_SaveText(Sa_Cod) & "' ) ")

            strSql.AppendLine(" AND (Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ) ")

            strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If dt.Rows.Count <> 1 Then
            Throw New Exception("DT.Rows.Count <> 1")
        End If

        Return dt.Rows(0).Item("Mat_Cod")

    End Function

    Public Function Leggi_da_MatCod_senzaFiltroVisibilita(ByVal Elem_Cod As Integer, ByVal Mat_Cod As Integer,
                                  ByVal Cod_Articolo As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi_da_MatCod_senzaFiltroVisibilita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" SELECT   Elem_Cod, Mat_Cod, Cod_Articolo, Mat_des, Regolamento, Veg_Cod, Cul_Cod, Sa_Cod, Piva  ")
            strSql.AppendLine(" FROM Materie_Prime ")
            strSql.AppendLine(" where  1=1 ")
            strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

            If Cod_Articolo <> "" Then
                strSql.AppendLine(" AND (Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ) ")
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

    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_JoinCompleta
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="Mat_Cod"></param>
    ''' <param name="Cod_Articolo"></param>
    ''' <param name="Mat_Cod_Origine"></param>
    ''' <param name="Piva_SuperUser_Origine"></param>
    ''' <param name="Flag_AncheImportatati"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	20/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi2(ByVal Piva As String,
                           ByVal Elem_Cod As Integer,
                           ByVal Mat_Cod As Integer,
                           ByVal Cod_Articolo As String,
                           ByVal Mat_Cod_Origine As Integer,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal Flag_AncheImportatati As Boolean,
                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        'default Flag_MateriePrimeSoloPrivate = false

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi




                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                    strSql.Length = 0

                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM   Materie_Prime ")
                    strSql.AppendLine(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    strSql.AppendLine(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Trim(Cod_Articolo) <> "" Then
                        strSql.AppendLine(" AND Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "'   ")
                    End If

                    Select Case Mat_Cod

                        Case 0 'Lettura delle Materie Prime Visibili dall'Impresa

                            strSql.AppendLine(" AND ( Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  OR Sa_Cod = -1 )   ")

                        Case Else 'Lettura Mirata

                            strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

                    End Select

                    If Mat_Cod_Origine <> 0 Then

                        strSql.AppendLine(" AND  Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")

                    End If

                    If Piva_SuperUser_Origine <> "" Then

                        strSql.AppendLine(" AND  Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")

                    End If

                    If Flag_AncheImportatati <> True Then

                        strSql.AppendLine(" AND  Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")

                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Mat_Cod ASC, Elem_Cod ASC")
                    End If



                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Materie_Prime.*, Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Materie_Prime LEFT OUTER JOIN Imprese ON Materie_Prime.Piva = Imprese.Piva ")
                    strSql.AppendLine(" WHERE  Materie_Prime.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    strSql.AppendLine(" AND    Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Trim(Cod_Articolo) <> "" Then
                        strSql.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "'   ")
                    End If

                    Select Case Mat_Cod

                        Case 0 'Lettura delle Materie Prime Visibili dall'Impresa

                            strSql.AppendLine(" AND ( Materie_Prime.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  OR Materie_Prime.Sa_Cod = -1 )   ")

                        Case Else 'Lettura Mirata

                            strSql.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

                    End Select

                    If Mat_Cod_Origine <> 0 Then

                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")

                    End If

                    If Piva_SuperUser_Origine <> "" Then

                        strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")

                    End If

                    If Flag_AncheImportatati <> True Then

                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")

                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                            strSql.AppendLine(" AND   Imprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Materie_Prime.Mat_Cod ASC, Materie_Prime.Elem_Cod ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni




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

    '##############################################################################################
    Public Function Leggi_Costi(ByVal Piva As String,
                                ByVal Sa_Cod As Integer,
                                ByVal Elem_Cod As Integer,
                                ByVal Mat_Cod As Integer,
                                ByVal Cod_Articolo As String,
                                ByVal Veg_Cod As Integer,
                                ByVal Cul_Cod As Integer,
                                ByVal Gen_Cod As Integer,
                                ByVal Spe_Cod As Integer,
                                ByVal Raz_Cod As Integer,
                                ByVal Ipro_Cod As Integer,
                                ByVal Cat_Cod As Integer,
                                ByVal RicercaTesto As String,
                                ByVal Mat_Cod_Origine As Integer,
                                ByVal Piva_SuperUser_Origine As String,
                                ByVal Flag_AncheImportatati As Boolean,
                                ByVal Flag_Costi As Boolean,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                     enumSelezioneVariabile.Selezione_JoinDescrizioni,
                     enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Materie_Prime.Piva, Materie_Prime.Sa_Cod, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Materie_Prime.Cod_Articolo, Materie_Prime.Mat_Des,  ")
                    strSql.AppendLine(" Materie_Prime.Sem_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Veg_Cod, Materie_Prime.Trap_Dur, Materie_Prime.Uso, Materie_Prime.ClToss_Cod,  ")
                    strSql.AppendLine(" Materie_Prime.NewClToss_Cod, Materie_Prime.Ditta_Cod, Materie_Prime.N, Materie_Prime.P2O5, Materie_Prime.K2O, Materie_Prime.MgO,  ")
                    strSql.AppendLine(" Materie_Prime.Note, Materie_Prime.inviato, Materie_Prime.datainvio, Materie_Prime.data_creazione, Materie_Prime.data_modifica,  ")
                    strSql.AppendLine(" Materie_Prime.username_creazione, Materie_Prime.username_modifica, Materie_Prime.validita_inizio, Materie_Prime.validita_fine,  ")
                    strSql.AppendLine(" Materie_Prime.Regolamento, Materie_Prime.Extra_Str, Materie_Prime.Extra_Int, Materie_Prime.Extra_Date, Materie_Prime.GRVA_COD_VEG,  ")
                    strSql.AppendLine(" Materie_Prime.GEN_COD, Materie_Prime.SPE_COD, Materie_Prime.RAZ_COD, Materie_Prime.IPRO_COD, Materie_Prime.CAT_COD,  ")
                    strSql.AppendLine(" Materie_Prime.Flag_Biologico, Materie_Prime.Flag_Convenzionale, Materie_Prime.Flag_NonAgricolo, Materie_Prime.Flag_AusiliareFabbricazione,  ")
                    strSql.AppendLine(" Materie_Prime.Udm_Cod_Extra, Materie_Prime.Flag_Extra, Materie_Prime.ChkImballaggio, Materie_Prime.Qta_Extra, Materie_Prime.Taglio,  ")
                    strSql.AppendLine(" Materie_Prime.Mat_Cod_Origine, Materie_Prime.Piva_SuperUser_Origine, Materie_Prime.ChkListino, Materie_Prime.Grfi_Cod,  ")
                    strSql.AppendLine(" Materie_Prime.Codice_Prodotto, Materie_Prime.Colore, Materie_Prime.Codice_NC, Materie_Prime.Manipolazioni, Materie_Prime.Titolo_Alcol,  ")
                    strSql.AppendLine(" Materie_Prime.ChkContenitore, Materie_Prime.Qta_Contenitore, Materie_Prime.Tipo_Peso, Materie_Prime.Tara, Materie_Prime.Peso_Set  ")

                    If Flag_Costi = True Then
                        strSql.AppendLine(" , Prodotti_Costi.Prezzo_Unitario, Prodotti_Costi.Validita_Inizio AS Validita_Inizio_Prezzo, Prodotti_Costi.Validita_Fine AS Validita_Fine_Prezzo, Prodotti_Costi.Udm_Cod, Prodotti_Costi.Mezzo ")
                    End If

                    strSql.AppendLine(" FROM Materie_Prime ")
                    strSql.AppendLine(" INNER  JOIN UtentiXImprese ON Materie_Prime.Piva = UtentiXImprese.PIVA ")

                    If Flag_Costi = True Then
                        strSql.AppendLine(" LEFT OUTER JOIN  Prodotti_Costi ON Materie_Prime.Elem_Cod = Prodotti_Costi.Elem_Cod And Prodotti_Costi.Id_Budget = 0 ")
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod = Prodotti_Costi.Mat_Cod ")
                    End If

                    strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri.PivaSuperUser) & "'")
                    strSql.AppendLine(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    strSql.AppendLine(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Flag_Costi = True Then
                        strSql.AppendLine(" AND Prodotti_Costi.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                        strSql.AppendLine(" AND Prodotti_Costi.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
                    End If

                    If Piva <> "" Then
                        strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
                    Else
                        strSql.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
                    End If

                    If RicercaTesto <> "" Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%'   ")
                    End If

                    If Cod_Articolo <> "" Then
                        strSql.AppendLine(" AND (Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
                    End If

                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If

                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If Cul_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
                    End If

                    If Gen_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
                    End If

                    If Spe_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
                    End If

                    If Raz_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
                    End If

                    If Ipro_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Ipro_Cod = " & Agro_SQL_SaveNum(Ipro_Cod) & "   ")
                    End If

                    If Cat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & "   ")
                    End If

                    If Mat_Cod_Origine <> 0 Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
                    End If

                    If Piva_SuperUser_Origine <> "" Then
                        strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                    End If

                    If Flag_AncheImportatati = False Then
                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine("  ORDER BY Materie_Prime.Mat_Des Asc ")
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

    Public Function Leggi_conCosti(ByVal Elem_Cod As Integer,
                                   ByVal Mat_Cod As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi_conCosti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try


            '---------------------------------------------
            strSql.Length = 0

            'Materie_Prime SENZA COSTI

            strSql.AppendLine(" (SELECT Materie_Prime.Piva, Sa_Cod, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Cod_Articolo, Mat_Des, Sem_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Veg_Cod, Trap_Dur, Uso, ClToss_Cod  ")
            strSql.AppendLine(" ,NewClToss_Cod,Ditta_Cod,N,P2O5,K2O,MgO,Note,Regolamento,Cal_Cod,Extra_Str,Extra_Int ")
            strSql.AppendLine(" ,Extra_Date,GRVA_COD_VEG,GEN_COD,SPE_COD,RAZ_COD,IPRO_COD,CAT_COD,Flag_Biologico,Flag_Convenzionale ")
            strSql.AppendLine(" ,Flag_NonAgricolo,Flag_AusiliareFabbricazione,Udm_Cod_Extra,Flag_Extra,ChkImballaggio,Qta_Extra ")
            strSql.AppendLine(" ,Taglio,Mat_Cod_Origine,Piva_SuperUser_Origine,ChkListino,Grfi_Cod,Codice_Prodotto,Colore ")
            strSql.AppendLine(" ,Codice_NC,Manipolazioni,Titolo_Alcol,ChkContenitore,Qta_Contenitore,Tipo_Peso,Tara,Materie_Prime.Udm_Cod,Peso_Set ")
            strSql.AppendLine(" ,ChkEscludi_Magazzino,ChkEscludi_Preparazione,Id_Accisa_Cod,Confezione_Cod,Categoria_Vino_Cod ")
            strSql.AppendLine(" ,Tipo_Reg_Alcoli,ChkAlias,Linea_Cod,Tipo_Default,ChkStampa_Dettagli, ")

            strSql.AppendLine(" 0 AS Prezzo_Unitario, ")
            strSql.AppendLine(" '01/01/1900' as Validita_Inizio_Prezzo, '31/12/2100' as Validita_Fine_Prezzo, ")
            strSql.AppendLine(" 0 as Udm_Cod_Prezzo, '' as Udm_Sim_Prezzo ")
            strSql.AppendLine(" FROM  Materie_Prime ")
            strSql.AppendLine(" WHERE Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" AND   Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            'StrSQL.AppendLine(" (SELECT Materie_Prime.*, 0 AS Prezzo_Unitario, " &
            '              "  '01/01/1900' as Validita_Inizio_Prezzo, '31/12/2100' as Validita_Fine_Prezzo, " &
            '              "  0 as Udm_Cod_Prezzo, '' as Udm_Sim_Prezzo " &
            '              " FROM  Materie_Prime " &
            '              " WHERE Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
            '              " AND   Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            strSql.AppendLine(" AND NOT EXISTS ( ")
            strSql.AppendLine("                 SELECT * ")
            strSql.AppendLine("                 FROM Prodotti_Costi ")
            strSql.AppendLine("                 WHERE Prodotti_Costi.Elem_Cod=" & Agro_SQL_SaveNum(Elem_Cod))
            strSql.AppendLine("                 AND Materie_Prime.Mat_Cod = Prodotti_Costi.Mat_Cod And Id_Budget = 0 )")

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.elem_cod =  " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.Mat_Cod =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine(") UNION ALL (")


            'Materie_Prime CON COSTI

            strSql.AppendLine(" SELECT Materie_Prime.Piva, Sa_Cod, Materie_Prime.Elem_Cod, Materie_Prime.Mat_Cod, Cod_Articolo, Mat_Des, Sem_Cod, Materie_Prime.Cul_Cod, Materie_Prime.Veg_Cod, Trap_Dur, Uso, ClToss_Cod  ")
            strSql.AppendLine(" ,NewClToss_Cod,Ditta_Cod,N,P2O5,K2O,MgO,Note,Regolamento,Cal_Cod,Extra_Str,Extra_Int ")
            strSql.AppendLine(" ,Extra_Date,GRVA_COD_VEG,GEN_COD,SPE_COD,RAZ_COD,IPRO_COD,CAT_COD,Flag_Biologico,Flag_Convenzionale ")
            strSql.AppendLine(" ,Flag_NonAgricolo,Flag_AusiliareFabbricazione,Udm_Cod_Extra,Flag_Extra,ChkImballaggio,Qta_Extra ")
            strSql.AppendLine(" ,Taglio,Mat_Cod_Origine,Piva_SuperUser_Origine,ChkListino,Grfi_Cod,Codice_Prodotto,Colore ")
            strSql.AppendLine(" ,Codice_NC,Manipolazioni,Titolo_Alcol,ChkContenitore,Qta_Contenitore,Tipo_Peso,Tara,Materie_Prime.Udm_Cod,Peso_Set ")
            strSql.AppendLine(" ,ChkEscludi_Magazzino,ChkEscludi_Preparazione,Id_Accisa_Cod,Confezione_Cod,Categoria_Vino_Cod ")
            strSql.AppendLine(" ,Tipo_Reg_Alcoli,ChkAlias,Linea_Cod,Tipo_Default,ChkStampa_Dettagli, ")

            strSql.AppendLine(" Prodotti_Costi.Prezzo_Unitario, ")
            strSql.AppendLine(" Prodotti_Costi.validita_inizio as Validita_Inizio_Prezzo, Prodotti_Costi.validita_fine as Validita_Fine_Prezzo, ")
            strSql.AppendLine(" Prodotti_Costi.Udm_Cod as Udm_Cod_Prezzo, UnitaMisura.UDM_SIM as Udm_Sim_Prezzo ")
            strSql.AppendLine(" FROM Materie_Prime INNER JOIN Prodotti_Costi ON Materie_Prime.mat_Cod = Prodotti_Costi.mat_Cod And Prodotti_Costi.Id_Budget = 0 ")
            strSql.AppendLine(" INNER JOIN UnitaMisura ON Prodotti_Costi.Udm_Cod = UnitaMisura.UDM_COD And Prodotti_Costi.Id_Budget = 0 ")

            strSql.AppendLine("  WHERE Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine("  AND   Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))



            'StrSQL.AppendLine(" SELECT Materie_Prime.*, Prodotti_Costi.Prezzo_Unitario, " &
            '              "  Prodotti_Costi.validita_inizio as Validita_Inizio_Prezzo, Prodotti_Costi.validita_fine as Validita_Fine_Prezzo, " &
            '              "  Prodotti_Costi.Udm_Cod as Udm_Cod_Prezzo, UnitaMisura.UDM_SIM as Udm_Sim_Prezzo " &
            '              "  FROM Materie_Prime INNER JOIN Prodotti_Costi ON Materie_Prime.mat_Cod = Prodotti_Costi.mat_Cod " &
            '              "  INNER JOIN UnitaMisura ON Prodotti_Costi.Udm_Cod = UnitaMisura.UDM_COD " &
            '              "  WHERE Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
            '              "  AND   Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.elem_cod =  " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.Mat_Cod =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.AppendLine(" ) ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Materie_Prime.Mat_DES ASC ")
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

    '################################################################################
    Public Sub Recupera_Extra(ByVal Elem_Cod As Integer,
                              ByVal Mat_Cod As Integer,
                              ByVal Peso_Set As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef Udm_Cod_Extra As Integer,
                              ByRef Udm_Sim_Extra As String,
                              ByRef Udm_Des_Extra As String,
                              ByRef Qta_Extra As Decimal,
                              ByRef Peso_Set_R As Integer,
                              ByRef Flag_Extra As Integer,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Udm_Cod_Extra = 0
        Qta_Extra = 0
        Peso_Set = 0
        Flag_Extra = 0

        Dim dt As DataTable

        dt = LeggiJoinUdmExtra(Mat_Cod,
                               Elem_Cod,
                               Peso_Set,
                               xFiltroAggiuntivo,
                               "",
                               objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Udm_Cod_Extra = dt.Rows(0).Item("Udm_Cod_Extra")
            Udm_Sim_Extra = dt.Rows(0).Item("Udm_Sim")
            Udm_Des_Extra = dt.Rows(0).Item("Udm_Des")
            Qta_Extra = dt.Rows(0).Item("Qta_Extra")
            Peso_Set = dt.Rows(0).Item("Peso_Set")
            Flag_Extra = dt.Rows(0).Item("Flag_Extra")
        End If

    End Sub

    '################################################################################
    Public Function Esiste_Semente(ByVal Piva As String,
                                   ByVal Veg_Cod As Integer,
                                   ByVal Cul_Cod As Integer,
                                   ByVal Sem_Cod As Integer,
                                   ByVal Regolamento As Integer,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) As Boolean

        Dim dt As DataTable
        Dim esiste As Boolean = False

        If Sem_Cod <> 0 Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo &= " AND "
            End If
            xFiltroAggiuntivo &= " Sem_cod = " & Agro_SQL_SaveNum(Sem_Cod) & "   "
        End If

        If Regolamento <> 0 Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo &= " AND "
            End If
            xFiltroAggiuntivo &= " Regolamento = " & Agro_SQL_SaveNum(Regolamento) & "   "
        End If

        dt = Leggi(Piva, 0, SEMENTI, 0, "", Veg_Cod, Cul_Cod, 0, 0, 0, 0, 0, "", 0, "", True, True, "",
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   xFiltroAggiuntivo,
                   "",
                   objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            esiste = True
        End If

        Return esiste

    End Function


    '################################################################################
    Public Function Esiste_SemilavoratoVegetale(ByVal Piva As String,
                                                ByVal Veg_Cod As Integer,
                                                ByVal Cul_Cod As Integer,
                                                ByVal Regolamento As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim dt As DataTable
        Dim esiste As Boolean = False

        If Regolamento <> 0 Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo &= " AND "
            End If
            xFiltroAggiuntivo &= " Regolamento = " & Agro_SQL_SaveNum(Regolamento) & "   "
        End If


        dt = Leggi(Piva, 0, SEMILAVORATI_VEGETALI, 0, "", Veg_Cod, Cul_Cod, 0, 0, 0, 0, 0, "", 0, "", True, True, "",
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   xFiltroAggiuntivo,
                   "",
                   objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            esiste = True
        End If

        Return esiste

    End Function

    '################################################################################
    Public Function Esiste_TrasformatoVegetale(ByVal Piva As String,
                                               ByVal Veg_Cod As Integer,
                                               ByVal Cul_Cod As Integer,
                                               ByVal Regolamento As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                               ) As Boolean

        Dim dt As DataTable
        Dim esiste As Boolean = False

        If Regolamento <> 0 Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo &= " AND "
            End If
            xFiltroAggiuntivo &= " Regolamento = " & Agro_SQL_SaveNum(Regolamento) & "   "
        End If

        dt = Leggi(Piva, 0, TRASFORMATI_VEGETALI, 0, "", Veg_Cod, Cul_Cod, 0, 0, 0, 0, 0, "", 0, "", True, True, "",
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   xFiltroAggiuntivo,
                   "",
                   objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            esiste = True
        End If

        Return esiste

    End Function


    '################################################################################
    Public Function MatDes_from_MatCod(ByVal Piva As String,
                                       ByVal Elem_Cod As Integer,
                                       ByVal Mat_Cod As Integer,
                                       ByVal FiltroMateriePrime As String,
                                       ByRef Cod_Articolo As String,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As String


        Dim dt As DataTable
        dt = Leggi(Piva, 0, Elem_Cod, Mat_Cod, "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, FiltroMateriePrime,
                   enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                   xFiltroAggiuntivo,
                   "",
                   objParametri)

        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then

                Cod_Articolo = dt.Rows(0).Item("Cod_Articolo")
                Return dt.Rows(0).Item("Mat_Des")
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function

    Public Function MatDes_from_MatCod_SenzaFiltroVisibilita(ByVal Elem_Cod As Integer,
                                       ByVal Mat_Cod As Integer,
                                       ByRef Cod_Articolo As String, ByRef Regolamento As Integer,
                                           ByRef Veg_Cod As Integer, ByRef Cul_Cod As Integer,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As String

        Dim dt As DataTable
        dt = Leggi_da_MatCod_senzaFiltroVisibilita(Elem_Cod, Mat_Cod, "", objParametri)

        Dim Mat_Des As String = ""
        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Mat_Des = dt.Rows(0).Item("Mat_Des")
            Cod_Articolo = dt.Rows(0).Item("Cod_Articolo")
            Regolamento = dt.Rows(0).Item("Regolamento")
            Veg_Cod = dt.Rows(0).Item("Veg_Cod")
            Cul_Cod = dt.Rows(0).Item("Cul_Cod")
        End If

        Return Mat_Des

    End Function


    '################################################################################
    Public Sub Recupera_Chk_TipoConfezionamento(ByVal piva As String,
                                                ByVal elemCod As Integer,
                                                ByVal matCod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef chkConfezione As Integer,
                                                ByRef chkContenitore As Integer,
                                                ByRef chkImballaggio As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Recupera_Chk_TipoConfezionamento()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable = Nothing

        chkConfezione = 0
        chkContenitore = 0
        chkImballaggio = 0

        Try

            dt = Get_Livello_Confezionamento(piva, elemCod, matCod, -1, "", "", objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                Select Case dt.Rows(0).Item("Tabella_Cod")
                    Case enum_OTabelle.Imballaggio
                        chkImballaggio = 1
                    Case enum_OTabelle.Contenitore
                        chkContenitore = 1
                    Case enum_OTabelle.Confezione
                        chkConfezione = 1
                    Case enum_OTabelle.Nessuno
                        'non sono in FF, quindi non ho Omni, perciò mi baso sui campi già stabiliti in Materie_prime
                        chkContenitore = dt.Rows(0).Item("ChkContenitore")
                        chkImballaggio = dt.Rows(0).Item("ChkImballaggio")
                End Select
            End If

            'dt = Leggi(piva, 0, elemCod, matCod, "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, "",
            '           enumSelezioneVariabile.Selezione_TabellaCompleta,
            '           xFiltroAggiuntivo,
            '           "",
            '           objParametri)

            'If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            '    chkContenitore = dt.Rows(0).Item("ChkContenitore")
            '    chkImballaggio = dt.Rows(0).Item("ChkImballaggio")
            'End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


    '################################################################################
    Public Function Recupera_FiltroMateriePrime(ByVal Impostazione_Cod As Integer,
                                                ByVal Piva As String,
                                                ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As String

        Dim x_Cod As Integer
        Dim Str_Filtro As String = ""
        Dim objUteImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        x_Cod = objUteImp.FiltroMateriePrime_from_ImpostazioneCod(Impostazione_Cod, objParametri_Utenti)

        Select Case x_Cod

            Case 0
                Str_Filtro = ""

            Case 1
                Str_Filtro = "AND (" &
                            "( Materie_Prime.Piva IN (SELECT Figlio" &
                            "                           FROM GerarchiaImprese " &
                            "			                WHERE Padre IN (SELECT Padre" &
                            "                                          FROM GerarchiaImprese " &
                            "                                          WHERE Figlio = '" & Agro_SQL_SaveText(Piva) & "')" &
                            "		                    )" &
                            ")" &
                            "OR " &
                            "( Materie_Prime.Piva IN (SELECT Figlio" &
                            "                           FROM GerarchiaImprese " &
                            "		                    WHERE Padre = '" & Agro_SQL_SaveText(objParametri_Utenti.PivaSuperUser) & "')" &
                            "                           )" &
                            ")"


        End Select


        Return Str_Filtro


    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_JoinCompleta.
    ''' MULTIHOST: Legge le materie prime create sotto un determinato super user.
    ''' </summary>
    ''' <param name="Piva_SuperUser"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="Mat_Cod"></param>
    ''' <param name="Mat_Cod_Origine"></param>
    ''' <param name="Piva_SuperUser_Origine"></param>
    ''' <param name="Flag_AncheImportatati"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	04/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiMateriePrimexSuperUser(ByVal Piva_SuperUser As String,
                                                ByVal Piva As String,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Mat_Cod_Origine As Integer,
                                                ByVal Piva_SuperUser_Origine As String,
                                                ByVal Flag_AncheImportatati As Boolean,
                                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.LeggiMateriePrimexSuperUser()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    strSql.Length = 0

                    strSql.AppendLine(" SELECT Materie_Prime.*, Imprese.Rag_Soc as Referente ")
                    strSql.AppendLine(" FROM   Materie_Prime LEFT OUTER JOIN Imprese ON Materie_Prime.Piva = Imprese.Piva ")
                    strSql.AppendLine(" INNER  JOIN UtentiXImprese ON Materie_Prime.Piva = UtentiXImprese.PIVA ")
                    strSql.AppendLine(" WHERE  Materie_Prime.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    strSql.AppendLine(" AND    Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    strSql.AppendLine(" AND    UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(Trim(Piva_SuperUser)) & "'")


                    If Elem_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
                    End If


                    If Mat_Cod <> 0 Then
                        strSql.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
                    End If


                    If Mat_Cod_Origine <> 0 Then

                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")

                    End If

                    If Piva_SuperUser_Origine <> "" Then

                        strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")

                    End If

                    If Flag_AncheImportatati <> True Then

                        strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")

                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   dbo.Materie_Prime.Inviato >=0 ")
                            strSql.AppendLine(" AND   dbo.UtentiXImprese.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   dbo.Materie_Prime.Inviato =-1 ")
                            strSql.AppendLine(" AND   dbo.UtentiXImprese.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Materie_Prime.Mat_Cod ASC, Materie_Prime.Elem_Cod ASC ")
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


    '###############################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="ElemCod"></param>
    ''' <param name="MatCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Regolamento_from_MatCod(ByRef Piva As String,
                                            ByRef ElemCod As Integer,
                                            ByRef MatCod As Integer,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                            ) As Integer

        Dim dt As DataTable

        dt = Leggi2(Piva, ElemCod, MatCod, "", 0, "", False, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then

            Return dt.Rows(0).Item("Regolamento")

        Else

            Return 0

        End If

    End Function



    '#############################################################################################
    'Reg_cod opzionale (0 oppure valore)
    'Flag_AggiungiMatCod se = true aggiunge all'hashtable tutti i mat_cod della lista che non sono stati trovati dalla query
    '(dipende se ho bisogno di sapere l'info per ognuno o se voglio solo quelli bio in risposta)
    Public Function BIO_1_0_from_ElencoMateriePrime(ByVal Lista_MP As String,
                                                    ByVal Reg_Cod As enum_Cod_Regolamento,
                                                    ByVal Flag_AggiungiMatCod As Boolean,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                    ) As Hashtable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.BIO_1_0_from_ElencoMateriePrime()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim i As Integer
        Dim filtro As String
        Dim HT_MP As New Hashtable
        Dim Mat_Cod As Integer

        Try

            filtro = " Materie_Prime.Mat_Cod IN (" & Lista_MP & ")"

            'reg_cod = 0 voglio leggerli tutti
            'reg_cod = 4 voglio in risultato solo quelli bio
            dt = Distinct_MatCod_Regolamento(0,
                                            Reg_Cod,
                                            filtro,
                                            "",
                                            objParametri)

            If Not IsNothing(dt) Then

                If dt.Rows.Count > 0 Then

                    For i = 0 To dt.Rows.Count - 1

                        Mat_Cod = dt.Rows(i).Item("Mat_cod")
                        Reg_Cod = dt.Rows(i).Item("Regolamento")

                        Select Case Reg_Cod

                            Case enum_Cod_Regolamento.Regolamento_bio
                                If Not HT_MP.Contains(Mat_Cod) Then
                                    HT_MP.Add(Mat_Cod, 1)
                                End If

                            Case Else
                                If Not HT_MP.Contains(Mat_Cod) Then
                                    HT_MP.Add(Mat_Cod, 0)
                                End If

                        End Select

                    Next

                End If

            End If


            'voglio avere la info 0 opp 1 per tutti i mat_cod della lista 
            '(anche per quelli che non sono presenti nel risultato della query)
            If Flag_AggiungiMatCod = True Then

                'aggiungo i mat_cod che non sono stati trovati nella tabella e li metto NON bio
                Dim vet_MP() As String
                vet_MP = Lista_MP.Split(",")

                For i = 0 To vet_MP.Length - 1

                    Mat_Cod = CInt(vet_MP(i))

                    If Not HT_MP.Contains(Mat_Cod) Then
                        HT_MP.Add(Mat_Cod, 0)
                    End If

                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return HT_MP

    End Function


    '###########################################################
    'Elem_cod opzionale 
    'Regolamento opzionale 
    Public Function Distinct_MatCod_Regolamento(ByVal Elem_Cod As Integer,
                                                ByVal Regolamento As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Distinct_MatCod_Regolamento()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT DISTINCT Mat_Cod, Regolamento ")
            strSql.AppendLine(" FROM   Materie_Prime ")
            strSql.AppendLine(" WHERE  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            strSql.AppendLine(" AND    Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Regolamento <> 0 Then
                strSql.AppendLine(" AND Regolamento = " & Agro_SQL_SaveNum(Regolamento) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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

    '########################################################################
    'legge solo la tabella Materie_Prime 
    'per non filtrare il sa_cod, passare il valore SACOD_NOFILTRO
    'perché 0 è significativo
    Public Function Leggi3(ByVal Piva As String,
                           ByVal Mat_Cod As Integer,
                           ByVal Sa_Cod As Integer,
                           ByVal Mat_Cod_Origine As Long,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal Flag_AncheImportati As Boolean,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                           ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi3()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT  Materie_Prime.* ")

            strSql.AppendLine(" FROM Materie_Prime   ")
            strSql.AppendLine(" INNER JOIN  UtentiXImprese ON Materie_Prime.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                strSql.AppendLine(" AND    (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "')   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND    (Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & ")   ")
            End If

            If Sa_Cod <> SACOD_NOFILTRO Then
                strSql.AppendLine(" AND    Materie_Prime.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            'gias2gias
            If Flag_AncheImportati = False Then
                strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = 0 ")
            Else
                If Mat_Cod_Origine <> 0 Then
                    strSql.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
                End If
                If Piva_SuperUser_Origine <> "" Then
                    strSql.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
                End If
            End If
            'fine gias2gias

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Materie_Prime.Mat_Des  ")

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

    '########################################################################
    'per non filtrare Peso_Set -> -1
    Public Function LeggiJoinUdmExtra(ByVal Mat_Cod As Integer,
                                      ByVal elem_Cod As Integer,
                                      ByVal Peso_Set As Integer,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.LeggiJoinUdmExtra()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            '----------------------------------------------------
            '--- Preparo la Query SQL ---------------------------
            '----------------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" SELECT  Materie_Prime.*, udm_sim, udm_des ")

            strSql.AppendLine(" FROM Materie_Prime   ")
            strSql.AppendLine(" INNER JOIN  UnitaMisura ON UnitaMisura.udm_cod = Materie_Prime.udm_cod_extra ")

            strSql.AppendLine(" INNER JOIN  UtentiXImprese ON Materie_Prime.Piva = UtentiXImprese.PIVA ")
            strSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND    Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If elem_Cod <> 0 Then
                strSql.AppendLine(" AND    Materie_Prime.elem_Cod = " & Agro_SQL_SaveNum(elem_Cod) & "  ")
            End If

            If Peso_Set <> -1 Then
                strSql.AppendLine(" AND    Materie_Prime.Peso_Set = " & Agro_SQL_SaveNum(Peso_Set) & "  ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Materie_Prime.Mat_Des  ")

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


    '################################################################################
    Public Sub VegCod_CulCod_from_MatCod(ByVal Piva As String,
                                         ByVal Elem_Cod As Integer,
                                         ByVal Mat_Cod As Integer,
                                         ByRef Veg_Cod As Integer,
                                         ByRef Cul_Cod As Integer,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          Optional verbose As Boolean = False,
                                          Optional ByRef Veg_Des As String = "",
                                          Optional ByRef Cul_Des As String = "")
        Dim dt As DataTable

        Dim _enumSelezioneVariabie As Integer = enumSelezioneVariabile.Selezione_TabellaCompleta
        If verbose Then
            _enumSelezioneVariabie = enumSelezioneVariabile.Selezione_JoinDescrizioni
        End If

        dt = Leggi(Piva,
                   0,
                   Elem_Cod,
                   Mat_Cod,
                   "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, "",
                   _enumSelezioneVariabie,
                   "", "", objParametri)


        If Not IsNothing(dt) Then
            If dt.Rows.Count <> 0 Then
                Cul_Cod = dt.Rows(0).Item("Cul_Cod")
                Veg_Cod = dt.Rows(0).Item("veg_Cod")

                If verbose Then
                    Cul_Des = dt.Rows(0).Item("Cul_Des")
                    Veg_Des = dt.Rows(0).Item("Veg_Des")
                End If
            End If
        End If


    End Sub

    '################################################################################
    Public Function RicavaUdmDefaultxTipoSemente_from_MatCod(ByVal Piva As String,
                                                         ByVal Elem_Cod As Integer,
                                                         ByVal Mat_Cod As Integer,
                                                         ByRef Sem_Cod As Integer,
                                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim dt As DataTable
        Dim Udm_Cod As Integer = 0

        dt = Leggi(Piva,
                   0,
                   Elem_Cod,
                   Mat_Cod,
                   "", 0, 0, 0, 0, 0, 0, 0, "", 0, "", True, False, "",
                   enumSelezioneVariabile.Selezione_TabellaCompleta,
                   "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Sem_Cod = dt.Rows(0).Item("Sem_Cod")

            Select Case Sem_Cod
                Case enum_TipologieSementi.Semente
                    Udm_Cod = enum_UnitaMisura.Unita_Seme
                Case enum_TipologieSementi.altre, enum_TipologieSementi.Selvatico, enum_TipologieSementi.GD
                    Udm_Cod = 0
                Case Else
                    Udm_Cod = enum_UnitaMisura.Num_Piante
            End Select

        End If

        Return Udm_Cod

    End Function


    Public Function Cod_Articolo_Leggi(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Elem_Cod As Integer,
                                       ByVal Mat_Cod As Integer,
                                       ByVal Cod_Articolo As String,
                                       ByVal Veg_Cod As Integer,
                                       ByVal Cul_Cod As Integer,
                                       ByVal Gen_Cod As Integer,
                                       ByVal Spe_Cod As Integer,
                                       ByVal Raz_Cod As Integer,
                                       ByVal Ipro_Cod As Integer,
                                       ByVal Cat_Cod As Integer,
                                       ByVal FinestraTemp_Inizio As String,
                                       ByVal FinestraTemp_Fine As String,
                                       ByVal RicercaTesto As String,
                                       ByVal Mat_Cod_Origine As Integer,
                                       ByVal Piva_SuperUser_Origine As String,
                                       ByVal Flag_AncheImportatati As Boolean,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable

        'ByVal FinestraTemp_Inizio As String = "01/01/1900",
        'ByVal FinestraTemp_Fine As String = "31/12/2100",

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.NewCom_Cod_Articolo_Leggi()"
        Dim messaggioErrore As String = ""
        Dim stbQuery As New StringBuilder
        Dim dt As DataTable


        Try

            stbQuery.Length = 0

            '----- Genero la query SQL 
            stbQuery.AppendLine(" SELECT DISTINCT Cod_Articolo ")
            stbQuery.AppendLine(" FROM Materie_Prime ")
            stbQuery.AppendLine(" INNER  JOIN UtentiXImprese ON Materie_Prime.Piva = UtentiXImprese.PIVA ")
            stbQuery.AppendLine(" WHERE UtentiXImprese.[USER] = '" & CStr(objParametri_Server.PivaSuperUser) & "'")
            stbQuery.AppendLine(" AND Materie_Prime.Validita_inizio <= " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            stbQuery.AppendLine(" AND Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")

            stbQuery.AppendLine(Recupera_FiltroMateriePrime(enum_Impostazioni_Utenti.SUPERUSER_COD_FILTRO_MATERIE_PRIME, Piva, objParametri_Utenti))


            If Piva <> "" Then
                stbQuery.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR Materie_Prime.Sa_Cod = -1  )")
            Else
                stbQuery.AppendLine(" AND (Materie_Prime.Sa_Cod = -1)")
            End If

            If RicercaTesto <> "" Then
                stbQuery.AppendLine(" AND Materie_Prime.Mat_Des like '%" & Agro_SQL_SaveText(RicercaTesto) & "%'   ")
            End If

            If Cod_Articolo <> "" Then
                stbQuery.AppendLine(" AND Materie_Prime.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
            End If

            If Elem_Cod <> 0 Then
                stbQuery.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                stbQuery.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Veg_Cod <> 0 Then
                stbQuery.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            End If

            If Cul_Cod <> 0 Then
                stbQuery.AppendLine(" AND Materie_Prime.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
            End If

            If Gen_Cod <> 0 Then
                stbQuery.AppendLine(" AND Materie_Prime.Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & "   ")
            End If

            If Spe_Cod <> 0 Then
                stbQuery.AppendLine(" AND Materie_Prime.Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & "   ")
            End If

            If Raz_Cod <> 0 Then
                stbQuery.AppendLine(" AND Materie_Prime.Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & "   ")
            End If

            If Ipro_Cod <> 0 Then
                stbQuery.AppendLine(" AND Materie_Prime.Ipro_Cod = " & Agro_SQL_SaveNum(Ipro_Cod) & "   ")
            End If

            If Cat_Cod <> 0 Then
                stbQuery.AppendLine(" AND Materie_Prime.Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & "   ")
            End If

            If Mat_Cod_Origine <> 0 Then
                stbQuery.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "   ")
            End If

            If Piva_SuperUser_Origine <> "" Then
                stbQuery.AppendLine(" AND  Materie_Prime.Piva_SuperUser_Origine = '" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "'")
            End If

            If Flag_AncheImportatati = False Then
                stbQuery.AppendLine(" AND  Materie_Prime.Mat_Cod_Origine = " & Agro_SQL_SaveNum(0) & " ")
            End If



            If xFiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If


            '--------------------------------------------------------------------------
            Select Case objParametri_Server.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stbQuery.AppendLine(" AND   Materie_Prime.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stbQuery.AppendLine(" AND   Materie_Prime.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stbQuery.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
            Else
                stbQuery.AppendLine(" ORDER BY Materie_Prime.Mat_Des ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '########################################################################

    Public Function ElemCod_from_MatCod(ByVal Mat_Cod As Integer,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Integer

        Dim dt As DataTable

        dt = Leggi3("",
                   Mat_Cod,
                   SACOD_NOFILTRO,
                   0,
                   "",
                   True,
                   "",
                   "",
                   objParametri)


        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            Return CInt(dt.Rows(0).Item("Elem_Cod"))
        Else
            Return 0
        End If

    End Function

    '########################################################################

    Public Function Tara_from_MatCod(ByVal Mat_Cod As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                     ) As Decimal

        Dim dt As DataTable

        dt = Leggi3("",
                   Mat_Cod,
                   SACOD_NOFILTRO,
                   0,
                   "",
                   True,
                   "",
                   "",
                   objParametri)


        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            Return CDec(dt.Rows(0).Item("Tara"))
        Else
            Return 0
        End If

    End Function

    Public Function Get_Livello_Confezionamento(ByVal piva As String,
                                                ByVal elemCod As Integer,
                                                ByVal matCod As Integer,
                                                ByVal tabellaCod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable

        'tabellaCod = -1 ==> nessun filtro, altrimenti (0[nessuno, scollegati da omni], 4[imballo], 5[confezione], 8[contenitore])

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Get_Livello_Confezionamento()"
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT *  ")
            stb.AppendLine(" FROM ")
            stb.AppendLine(" ( ")
            stb.AppendLine("     -- Confezionamenti creati a mano ma da Omni ")
            stb.AppendLine("     ( ")
            stb.AppendLine("         SELECT DISTINCT oPar.Tabella_Cod, mp.*  ")
            stb.AppendLine("         FROM Materie_Prime mp ")
            stb.AppendLine("         INNER JOIN OTabelle_Parametri oPar ON mp.mat_cod = oPar.mat_cod_generazione_link ")
            stb.AppendLine("         WHERE oPar.Tabella_Cod IN (" & CInt(enum_OTabelle.Imballaggio) & ", " &
                                                                    CInt(enum_OTabelle.Confezione) & ", " &
                                                                    CInt(enum_OTabelle.Contenitore) & ") ")
            stb.AppendLine("     ) ")
            stb.AppendLine("  ")
            stb.AppendLine("     UNION ALL ")
            stb.AppendLine("  ")
            stb.AppendLine("     -- Confezionamenti creati automaticamente da Omni ")
            stb.AppendLine("     ( ")
            stb.AppendLine("         SELECT DISTINCT oPar.Tabella_Cod, mp.*  ")
            stb.AppendLine("         FROM Materie_Prime mp ")
            stb.AppendLine("         INNER JOIN OGenerazioni_Anagrafe_Log oLog ON mp.Mat_Cod = oLog.Mat_Cod ")
            stb.AppendLine("         INNER JOIN OTabelle_Parametri oPar ON oLog.Codice_Generazione = oPar.Codice_Generazione_Link ")
            stb.AppendLine("         WHERE oPar.Tabella_Cod IN (" & CInt(enum_OTabelle.Imballaggio) & ", " &
                                                                    CInt(enum_OTabelle.Confezione) & ", " &
                                                                    CInt(enum_OTabelle.Contenitore) & ") ")
            stb.AppendLine("     ) ")
            stb.AppendLine("  ")
            stb.AppendLine("     UNION ALL ")
            stb.AppendLine("  ")
            stb.AppendLine("     -- Confezionamenti creati su Materie_Prime ma slegati da Omni ")
            stb.AppendLine("     ( ")
            stb.AppendLine("         SELECT DISTINCT  ")
            stb.AppendLine("         --CASE  ")
            stb.AppendLine("         --   WHEN mp.ChkImballaggio = 1 THEN 4 ")
            stb.AppendLine("         --   WHEN mp.ChkContenitore = 1 THEN 8 ")
            stb.AppendLine("         --   ELSE 5 ")
            stb.AppendLine("         --END ")
            stb.AppendLine("         " & CInt(enum_OTabelle.Nessuno) & " AS Tabella_Cod,  ")
            stb.AppendLine("         mp.* ")
            stb.AppendLine("         FROM Materie_Prime mp ")
            stb.AppendLine("         WHERE Elem_Cod IN (" & BENI_CONFEZ_VEGETALE & ", " & BENI_CONFEZ_ANIMALE & ") ")
            stb.AppendLine("         AND NOT EXISTS(SELECT 1 FROM OTabelle_Parametri oPar WHERE mat_cod_generazione_link = mp.Mat_Cod) ")
            stb.AppendLine("         AND NOT EXISTS(SELECT 1 FROM OGenerazioni_Anagrafe_Log oLog INNER JOIN OTabelle_Parametri oPar ON oLog.Codice_Generazione = oPar.Codice_Generazione_Link WHERE oLog.Mat_Cod = mp.Mat_Cod) ")
            stb.AppendLine("     ) ")
            stb.AppendLine("  ")
            stb.AppendLine(" ) AS MAT ")
            stb.AppendLine(" ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If piva <> "" Then
                stb.AppendLine(" AND (Piva = '" & Agro_SQL_SaveText(piva) & "' OR Sa_Cod = -1  )")
            Else
                stb.AppendLine(" AND (Sa_Cod = -1)")
            End If

            If elemCod <> 0 Then
                stb.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(elemCod) & " ")
            End If

            If matCod <> 0 Then
                stb.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(matCod) & "   ")
            End If

            If tabellaCod <> -1 Then
                stb.AppendLine(" AND Tabella_Cod = " & Agro_SQL_SaveNum(tabellaCod) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametriServer))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametriServer))
            Else
                stb.AppendLine(" ORDER BY Tabella_Cod, ChkImballaggio, ChkContenitore, Mat_Des ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_MateriePrime_Conferimento_ConControlloMovimentato(ByVal piva As String,
                                                               ByVal Elem_Cod As Integer,
                                                               ByVal Mat_Cod As Integer,
                                                               ByVal soloMovimentato As Boolean,
                                                               ByVal filters As String,
                                                               ByVal soloLegatiALinea As Boolean,
                                                               ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                               ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                               ) As String

        Dim risposta As String

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi_MateriePrime_ConTestMovimentato()"
        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Dim objFilters As JArray = Nothing
        If Not String.IsNullOrEmpty(filters) Then
            objFilters = JArray.Parse(filters)
        End If
        Try

            Dim objImp As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R
            Dim filtro_materie_prime = objImp.RecuperaFiltroSQLMatPrime_from_AreaGIAS(enum_AreaGIAS.Conferimento, objParametri_Utenti)
            Dim filtroNoOmniConReferenza = " Not EXISTS (Select TOP(1)* from materie_prime mpColl where mpColl.Elem_Cod = materie_prime.Elem_cod And mpColl.Mat_Cod_Referenza = materie_prime.Mat_Cod And mpColl.Mat_Cod_Referenza <> 0) "
            Dim filtroMovimentati = " EXISTS (Select TOP(1)* from Movimenti_dettagli md where md.Elem_Cod = materie_prime.Elem_cod And md.Mat_Cod = materie_prime.Mat_Cod) "

            Dim filtroDescr As String = ""
            If Not objFilters Is Nothing Then
                For Each obj As JObject In objFilters
                    Dim v = obj("value").ToString()
                    If obj("field") = "Mat_Des" Then
                        If obj("operator") = "startswith" Then
                            filtroDescr = " AND materie_prime.Mat_Des like '" & Agro_SQL_SaveText(v) & "%' "
                        End If
                        If obj("operator") = "contains" Then
                            filtroDescr = " AND materie_prime.Mat_Des like '%" & Agro_SQL_SaveText(v) & "%' "
                        End If
                        If obj("operator") = "equals" Then
                            filtroDescr = " AND materie_prime.Mat_Des = '" & Agro_SQL_SaveText(v) & "' "
                        End If
                        Exit For
                    End If
                Next
            End If

            stb.Length = 0

            'Nei prodotti legati a linea considero solo quelli con partita Iva = quella dell'azienda
            If soloLegatiALinea Then
                stb.AppendLine(" SELECT ")
                stb.AppendLine("  materie_prime.Piva, materie_prime.Mat_Cod, materie_prime.Mat_Des, materie_prime.Elem_Cod, _m_p_referenza.Linea_Cod, ")
                stb.AppendLine("  materie_prime.Veg_Cod, materie_prime.Cul_Cod, _specievegetali.Veg_Des,  _cultivar.Cul_Des, materie_prime.Cal_Cod, materie_prime.Mat_Cod_Referenza ")
                stb.AppendLine(" FROM ")
                stb.AppendLine(" Materie_Prime ")
                stb.AppendLine("    Join OGenerazioni_Anagrafe_Log _m_p_referenza ")
                stb.AppendLine("               On materie_prime.Elem_Cod = _m_p_referenza.Elem_Cod And ")
                stb.AppendLine("                 materie_prime.Mat_Cod_Referenza = _m_p_referenza.Mat_Cod And ")
                ' stb.AppendLine("                 materie_prime.Sa_Cod = _m_p_referenza.Sa_Cod And ")
                stb.AppendLine("                 materie_prime.Piva = _m_p_referenza.Piva ")
                stb.AppendLine("     Join  Linee_Produzioni _linee_produzioni ")
                stb.AppendLine("               On _linee_produzioni.Piva = _m_p_referenza.Piva And ")
                stb.AppendLine("                 _linee_produzioni.Linea_Cod = _m_p_referenza.Linea_Cod ")
                stb.AppendLine("     Join  Linee_Classi_Produzioni _linee_classi_produzioni ")
                stb.AppendLine("               On _linee_classi_produzioni.Piva = _linee_produzioni.Piva And ")
                stb.AppendLine("                 _linee_classi_produzioni.Linea_Classe_Cod = _linee_produzioni.Linea_Classe_Cod ")
                stb.AppendLine("     Join  SpecieVegetali _specievegetali ")
                stb.AppendLine("               On materie_prime.Veg_Cod = _specievegetali.Veg_Cod ")
                stb.AppendLine("       Join  Cultivar _cultivar ")
                stb.AppendLine("               On materie_prime.Veg_Cod = _cultivar.Veg_Cod And ")
                stb.AppendLine("                 materie_prime.Cul_Cod = _cultivar.Cul_Cod ")
                stb.AppendLine(" WHERE ")
                stb.AppendLine(" _m_p_referenza.ChkScollegamento = 0 ")
                stb.AppendLine("  AND (materie_prime.Piva = '" & Agro_SQL_SaveText(piva) & "' ) ")
                If Mat_Cod <> 0 Then
                    stb.AppendLine(" AND materie_prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod))
                End If
                If Elem_Cod <> 0 Then
                    stb.AppendLine(" AND materie_prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod))
                End If
                If filtro_materie_prime <> "" Then
                    stb.AppendLine(" AND  " & filtro_materie_prime)
                End If
                stb.AppendLine(" AND materie_prime.Elem_Cod In ( " & CStr(TRASFORMATI_VEGETALI) & ", " & CStr(TRASFORMATI_ANIMALI) & " ) ")
                stb.AppendLine(" AND materie_prime.Mat_Cod_Referenza <> 0  ")
                If filtroDescr <> "" Then
                    stb.AppendLine(filtroDescr)
                End If
                ' Filtro per i soli prodotti movimentati
                If soloMovimentato Then
                    stb.AppendLine(" AND " & filtroMovimentati)
                End If

                stb.AppendLine(" UNION ")

                stb.AppendLine(" Select ")
                stb.AppendLine("  materie_prime.Piva, materie_prime.Mat_Cod, materie_prime.Mat_Des, materie_prime.Elem_Cod, materie_prime.Linea_Cod, ")
                stb.AppendLine("  materie_prime.Veg_Cod, materie_prime.Cul_Cod, _specievegetali.Veg_Des,  _cultivar.Cul_Des, materie_prime.Cal_Cod, materie_prime.Mat_Cod_Referenza ")
                stb.AppendLine(" FROM ")
                stb.AppendLine(" Materie_Prime ")
                stb.AppendLine("     Join  SpecieVegetali _specievegetali ")
                stb.AppendLine("               On materie_prime.Veg_Cod = _specievegetali.Veg_Cod ")
                stb.AppendLine("       Join  Cultivar _cultivar ")
                stb.AppendLine("               On materie_prime.Veg_Cod = _cultivar.Veg_Cod And ")
                stb.AppendLine("                 materie_prime.Cul_Cod = _cultivar.Cul_Cod ")
                stb.AppendLine(" WHERE ")
                stb.AppendLine("   (materie_prime.Piva = '" & Agro_SQL_SaveText(piva) & "' ) ")
                If Mat_Cod <> 0 Then
                    stb.AppendLine(" AND materie_prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod))
                End If
                If Elem_Cod <> 0 Then
                    stb.AppendLine(" AND materie_prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod))
                End If
                If filtro_materie_prime <> "" Then
                    stb.AppendLine(" AND  " & filtro_materie_prime)
                End If
                stb.AppendLine(" AND materie_prime.Elem_Cod In ( " & CStr(TRASFORMATI_VEGETALI) & ", " & CStr(TRASFORMATI_ANIMALI) & " ) ")
                stb.AppendLine(" AND materie_prime.Mat_Cod_Referenza = 0 And materie_prime.ChkReferenza = 1 ")
                ' Scarto i prodotti OMNI se esiste già una referenza
                stb.AppendLine(" AND " & filtroNoOmniConReferenza)
                If filtroDescr <> "" Then
                    stb.AppendLine(filtroDescr)
                End If
                ' Filtro per i soli prodotti movimentati
                If soloMovimentato Then
                    stb.AppendLine(" AND " & filtroMovimentati)
                End If
            End If

            If Not soloLegatiALinea Then
                stb.AppendLine(" Select ")
                stb.AppendLine("  Materie_Prime.Piva, Materie_Prime.Mat_Cod, Materie_Prime.Mat_Des, Materie_Prime.Elem_Cod, Materie_Prime.Linea_Cod, ")
                stb.AppendLine("  Materie_Prime.Veg_Cod, Materie_Prime.Cul_Cod, _specievegetali.Veg_Des,  _cultivar.Cul_Des, Materie_Prime.Cal_Cod, Materie_Prime.Mat_Cod_Referenza ")
                stb.AppendLine(" FROM ")
                stb.AppendLine(" Materie_Prime  ")
                stb.AppendLine("     Join  SpecieVegetali _specievegetali ")
                stb.AppendLine("               On Materie_Prime.Veg_Cod = _specievegetali.Veg_Cod ")
                stb.AppendLine("       Join  Cultivar _cultivar ")
                stb.AppendLine("               On Materie_Prime.Veg_Cod = _cultivar.Veg_Cod And ")
                stb.AppendLine("                 Materie_Prime.Cul_Cod = _cultivar.Cul_Cod ")
                stb.AppendLine(" WHERE ")
                stb.AppendLine("   (Materie_Prime.Sa_Cod = -1 Or Materie_Prime.Piva = '" & Agro_SQL_SaveText(piva) & "' ) ")
                If Mat_Cod <> 0 Then
                    stb.AppendLine(" AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod))
                End If
                If Elem_Cod <> 0 Then
                    stb.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod))
                End If
                If filtro_materie_prime <> "" Then
                    stb.AppendLine(" AND  " & filtro_materie_prime)
                End If
                stb.AppendLine(" AND Materie_Prime.Elem_Cod In ( " & CStr(TRASFORMATI_VEGETALI) & ", " & CStr(TRASFORMATI_ANIMALI) & " ) ")
                ' Scarto comunque i prodotti OMNI se esiste già una referenza
                stb.AppendLine(" AND " & filtroNoOmniConReferenza)
                If filtroDescr <> "" Then
                    stb.AppendLine(filtroDescr)
                End If
                ' Filtro per i soli prodotti movimentati
                If soloMovimentato Then
                    stb.AppendLine(" AND " & filtroMovimentati)
                End If
            End If

            stb.AppendLine(" ORDER BY Materie_Prime.Mat_Des ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            risposta = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risposta

    End Function


    '########################################################################
    Public Function SpecieVarieta_NON_su_MateriePrime(ByVal Cul_Cod As Integer,
                                                        ByVal Veg_Cod As Integer,
                                                        ByVal Cerca_CulDes As String,
                                                      ByVal Elem_Cod As Integer,
                                                      ByVal Regolamento_Cod As Integer,
                                                         ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As DataTable

        '----- Descrizione
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.SpecieVarieta_NON_su_MateriePrime()"

        Dim dt As New DataTable
        Dim strSql As New StringBuilder
        Dim messaggioErrore As String

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT  SpecieVegetali.Veg_des, CUL_DES,  Cultivar.data_modifica,Cultivar.veg_cod,Cultivar.cul_cod ")
            strSql.AppendLine(" FROM        Cultivar ")
            strSql.AppendLine(" INNER JOIN    SpecieVegetali ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
            strSql.AppendLine(" WHERE   Cultivar.Validita_Inizio <=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.AppendLine(" And     Cultivar.Validita_Fine >=" & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            'se c'è un regolamento selezionato
            If Regolamento_Cod <> 0 Then
                strSql.AppendLine("  And Not EXISTS (  ")
                strSql.AppendLine("                  SELECT 1 ")
                strSql.AppendLine("                 FROM Materie_Prime ")
                strSql.AppendLine("                  WHERE Materie_Prime.veg_cod = Cultivar.veg_cod ")
                strSql.AppendLine("                  And Materie_Prime.cul_cod = Cultivar.cul_cod ")
                strSql.AppendLine("                  And Materie_Prime.regolamento = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
                If Elem_Cod <> 0 Then
                    strSql.AppendLine("             And Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
                End If
                strSql.AppendLine("                 )  ")
            Else
                'se devo cercare su regolamento conv e bio
                strSql.AppendLine("  And (  ")
                strSql.AppendLine("         Not EXISTS (  ")
                strSql.AppendLine("                  SELECT 1 ")
                strSql.AppendLine("                 FROM Materie_Prime ")
                strSql.AppendLine("                  WHERE Materie_Prime.veg_cod = Cultivar.veg_cod ")
                strSql.AppendLine("                  And Materie_Prime.cul_cod = Cultivar.cul_cod ")
                strSql.AppendLine("                  And Materie_Prime.regolamento = " & Agro_SQL_SaveNum(enum_Cod_Regolamento.Regolamento_Nessuno) & " ")
                If Elem_Cod <> 0 Then
                    strSql.AppendLine("             And Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
                End If
                strSql.AppendLine("                 )  ")
                strSql.AppendLine("         Or   ")
                strSql.AppendLine("         Not EXISTS (  ")
                strSql.AppendLine("                  SELECT 1 ")
                strSql.AppendLine("                 FROM Materie_Prime ")
                strSql.AppendLine("                  WHERE Materie_Prime.veg_cod = Cultivar.veg_cod ")
                strSql.AppendLine("                  And Materie_Prime.cul_cod = Cultivar.cul_cod ")
                strSql.AppendLine("                  And Materie_Prime.regolamento = " & Agro_SQL_SaveNum(enum_Cod_Regolamento.Regolamento_bio) & " ")
                If Elem_Cod <> 0 Then
                    strSql.AppendLine("             And Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
                End If
                strSql.AppendLine("                 )  ")
                strSql.AppendLine("  )  ")
            End If

            If Cul_Cod <> 0 Then
                strSql.AppendLine(" And Cultivar.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" And Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If Cerca_CulDes <> "" Then
                strSql.AppendLine(" And Cultivar.Cul_Des Like '%" & Agro_SQL_SaveText(Cerca_CulDes) & "%' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Cultivar.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Cultivar.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Veg_Des, Cul_Des ")
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

    '##############################################################################################
    Public Function TipSementiSpecieVarieta_NON_su_MateriePrime(ByVal CUL_COD As Integer,
                                                               ByVal VEG_COD As Integer,
                                                                ByVal SEM_COD As Integer,
                                                                ByVal regolamento_COD As Integer,
                                                                ByVal xFiltroAggiuntivo As String,
                                                                ByVal xOrderBy As String,
                                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Materie_Prime_R.TipSementiSpecieVarieta_NON_su_MateriePrime()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT TipologieSementi.Sem_Cod, TipologieSementi.Sem_Des, SpecieVegetali.Veg_cod, SpecieVegetali.Veg_Des, Cultivar.cul_cod, Cultivar.cul_des  ")
            StrSQL.AppendLine(" FROM  TipologieSementixSpecieVegetali ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON TipologieSementixSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN Cultivar ON Cultivar.VEG_COD = SpecieVegetali.VEG_COD ")
            StrSQL.AppendLine(" INNER JOIN TipologieSementi ON TipologieSementixSpecieVegetali.SEM_COD = TipologieSementi.SEM_COD  ")

            StrSQL.AppendLine(" WHERE TipologieSementixSpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementixSpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   SpecieVegetali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   TipologieSementi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine(" AND   Cultivar.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine(" AND   Cultivar.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            'se c'è un regolamento selezionato
            If regolamento_COD <> 0 Then
                StrSQL.AppendLine("  AND NOT EXISTS (  ")
                StrSQL.AppendLine("                  SELECT 1 ")
                StrSQL.AppendLine("                 FROM Materie_Prime ")
                StrSQL.AppendLine("                  WHERE Materie_Prime.veg_cod = Cultivar.veg_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.cul_cod = Cultivar.cul_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.SEM_cod = TipologieSementi.SEM_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.regolamento = " & Agro_SQL_SaveNum(regolamento_COD) & " ")
                StrSQL.AppendLine("                 AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(SEMENTI) & " ")
                StrSQL.AppendLine("  )  ")
            Else
                'se devo cercare su regolamento conv e bio
                StrSQL.AppendLine("  AND (  ")
                StrSQL.AppendLine("         NOT EXISTS (  ")
                StrSQL.AppendLine("                  SELECT 1 ")
                StrSQL.AppendLine("                 FROM Materie_Prime ")
                StrSQL.AppendLine("                  WHERE Materie_Prime.veg_cod = Cultivar.veg_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.cul_cod = Cultivar.cul_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.SEM_cod = TipologieSementi.SEM_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.regolamento = " & Agro_SQL_SaveNum(enum_Cod_Regolamento.Regolamento_Nessuno) & " ")
                StrSQL.AppendLine("                 AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(SEMENTI) & " ")
                StrSQL.AppendLine("                 )  ")
                StrSQL.AppendLine("         OR   ")
                StrSQL.AppendLine("         NOT EXISTS (  ")
                StrSQL.AppendLine("                  SELECT 1 ")
                StrSQL.AppendLine("                 FROM Materie_Prime ")
                StrSQL.AppendLine("                  WHERE Materie_Prime.veg_cod = Cultivar.veg_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.cul_cod = Cultivar.cul_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.SEM_cod = TipologieSementi.SEM_cod ")
                StrSQL.AppendLine("                  AND Materie_Prime.regolamento = " & Agro_SQL_SaveNum(enum_Cod_Regolamento.Regolamento_bio) & " ")
                StrSQL.AppendLine("                 AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(SEMENTI) & " ")
                StrSQL.AppendLine("                 )  ")
                StrSQL.AppendLine("  )  ")
            End If

            If CUL_COD <> 0 Then
                StrSQL.AppendLine(" AND Cultivar.CUL_COD =  " & Agro_SQL_SaveNum(CUL_COD) & "  ")
            End If

            If VEG_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
            End If

            If SEM_COD <> 0 Then
                StrSQL.AppendLine(" AND TipologieSementixSpecieVegetali.SEM_COD =  " & Agro_SQL_SaveNum(SEM_COD) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY SpecieVegetali.VEG_DES, Cultivar.Cul_Des, TipologieSementi.sem_des ")
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


    Public Function Max_DataModifica(ByVal Piva As String,
                                          ByVal Elem_Cod As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Date

        Dim dt As DataTable
        Dim MaxData As Date

        dt = Leggi_MaxDataModifica(Piva, Elem_Cod, xFiltroAggiuntivo, xOrderBy, objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            MaxData = dt.Rows(0).Item("data_modifica")
        End If

        Return MaxData

    End Function

    Public Function Leggi_MaxDataModifica(ByVal Piva As String,
                                          ByVal Elem_Cod As Integer,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                          ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi_MaxDataModifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" SELECT isnull(max([data_modifica]), '01/01/1900') as Data_Modifica ")
            strSql.AppendLine(" FROM   Materie_Prime ")
            strSql.AppendLine(" WHERE  1 = 1 ")

            If Piva <> "" Then
                strSql.AppendLine(" AND Piva  = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
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

    Public Function Leggi_Materie_Prime_XLingue(ByVal Piva As String,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Sa_Cod As Integer?,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Lingua_Cod As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As DataTable


        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi_Materie_Prime_XLingue()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" SELECT Materie_Prime_XLingue.*,Lingue.Lingua_Cod , Lingue.Nome ")
            strSql.AppendLine(" FROM   Materie_Prime_XLingue ")
            strSql.AppendLine(" LEFT JOIN Lingue ")
            strSql.AppendLine(" ON  Lingue.Lingua_Cod = Materie_Prime_XLingue.Lingua_Cod")
            strSql.AppendLine(" WHERE 1 = 1")
            If Piva <> "" Then
                strSql.AppendLine(" AND Materie_Prime_XLingue.Piva  = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime_XLingue.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Not IsNothing(Sa_Cod) Then
                strSql.AppendLine(" AND Materie_Prime_XLingue.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime_XLingue.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Lingua_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime_XLingue.Lingua_Cod = " & Agro_SQL_SaveNum(Lingua_Cod) & "   ")
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

    Function Check_Cod_Articolo_Esistente(ByVal piva As String,
                                          ByVal mat_Cod As Integer?,
                                          ByVal cod_Articolo As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Check_Cod_Articolo_Esistente()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim risultato As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Mat_Des from Materie_Prime ")
            strSql.AppendLine(" WHERE ")
            strSql.AppendLine(" Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            strSql.AppendLine(" AND Cod_Articolo = '" & Agro_SQL_SaveText(cod_Articolo) & "' ")

            If Not mat_Cod Is Nothing Then
                strSql.AppendLine(" AND Mat_cod <> " & Agro_SQL_SaveNum(mat_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                risultato = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return risultato

    End Function


    Function Leggi_LastCod_Articolo(ByVal piva As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.Leggi_LastCod_Articolo()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT DISTINCT  MAX(CAST(Cod_Articolo AS bigint))  as Codice ")
            strSql.AppendLine(" FROM  Materie_Prime ")
            strSql.AppendLine(" WHERE IsNumeric(Cod_Articolo) = 1 ")
            strSql.AppendLine(" AND (Piva = '" & Agro_SQL_SaveText(piva) & "' Or Sa_Cod = -1) AND Cod_Articolo NOT LIKE '%[a-z]%'")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.AppendLine(" ORDER BY Codice DESC")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function




    Function LeggiMateriePrimeContratto(ByVal Piva As String,
                                        ByVal Elem_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Veg_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.LeggiMateriePrimeContratto()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Distinct Materie_Prime.*, (Cod_Articolo + ' ' + Mat_Des) as Mat_Des_Esteso, ")
            strSql.AppendLine(" isnull(SpecieVegetali.veg_des, '') as Veg_Des  ")
            strSql.AppendLine(" FROM  Materie_Prime ")
            strSql.AppendLine(" Left Outer Join SpecieVegetali On (SpecieVegetali.Veg_Cod = Materie_Prime.Veg_Cod) ")
            strSql.AppendLine(" WHERE Materie_Prime.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            strSql.AppendLine(" AND   Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            strSql.AppendLine(" AND   Materie_Prime.ChkAlias = 0 ")
            strSql.AppendLine(" AND   Materie_Prime.Elem_Cod = 210 ")


            If Piva <> "" Then
                strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' or Materie_Prime.Sa_Cod = -1 )   ")
            End If

            If Mat_Cod <> 0 AndAlso Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " AND Materie_Prime.Mat_Cod_Referenza = " & Agro_SQL_SaveNum(Mat_Cod))
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Materie_Prime.Mat_Des ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function


    Function LeggiGenerazioniReferenze(ByVal Piva As String,
                                       ByVal Elem_Cod As Integer,
                                       ByVal Mat_Cod As Integer,
                                       ByVal Modulo_Generazione As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal Veg_Cod As Integer = 0) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.LeggiGenerazioniReferenze()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Distinct Materie_Prime.*, (Cod_Articolo + ' ' + Mat_Des) as Mat_Des_Esteso, OGenerazioni_Anagrafe_Log.Modulo_Generazione, OGenerazioni_Anagrafe_Log.Linea_Cod, ")
            strSql.AppendLine(" OGenerazioni_Anagrafe_Log.Codice_Generazione, Linee_Classi_Produzioni.Sa_Cod as Sa_Cod_Linea, isnull(SpecieVegetali.veg_des, '') as Veg_Des  ")
            strSql.AppendLine(" FROM  OGenerazioni_Anagrafe_Log, Linee_Classi_Produzioni, Linee_Produzioni,Materie_Prime ")
            strSql.AppendLine(" Left Outer Join SpecieVegetali On (SpecieVegetali.Veg_Cod = Materie_Prime.Veg_Cod) ")
            strSql.AppendLine(" WHERE Materie_Prime.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            strSql.AppendLine(" AND   Materie_Prime.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            strSql.AppendLine(" AND   Materie_Prime.Mat_Cod_Referenza = OGenerazioni_Anagrafe_Log.Mat_Cod ")
            strSql.AppendLine(" AND   Materie_Prime.ChkAlias = 0 ")
            strSql.AppendLine(" AND   Linee_Produzioni.Linea_Cod = OGenerazioni_Anagrafe_Log.Linea_Cod ")
            strSql.AppendLine(" AND   Linee_Classi_Produzioni.Piva = Linee_Produzioni.Piva ")
            strSql.AppendLine(" AND   Linee_Classi_Produzioni.Linea_Classe_Cod = Linee_Produzioni.Linea_Classe_Cod ")

            If Modulo_Generazione <> 0 Then
                strSql.AppendLine(" AND OGenerazioni_Anagrafe_Log.Modulo_Generazione = " & Agro_SQL_SaveNum(Modulo_Generazione) & "   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' or Materie_Prime.Sa_Cod = -1 )   ")
            End If

            If Mat_Cod <> 0 AndAlso Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " AND Materie_Prime.Mat_Cod_Referenza = " & Agro_SQL_SaveNum(Mat_Cod))
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            Else
                'Filtro Referenze
                strSql.AppendLine(" AND  Mat_Cod_Referenza <> 0 And Cal_Cod <> 0 And ChkReferenza = 0")
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY Materie_Prime.Mat_Des ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

    Function LeggiFinalitaMateriePrime(ByVal Piva As String,
                                       ByVal Elem_Cod As Integer,
                                       ByVal Mat_Cod As Integer,
                                       ByVal Veg_Cod As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_R.LeggiFinalitaProdotti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     Materie_Prime.Mat_Cod, ISNULL(Materie_Prime.Grfi_Cod, 0) AS Grfi_Cod, ISNULL(GruppoFinalita.Grfi_Des, '') AS Grfi_Des ")
            strSql.AppendLine(" FROM Materie_Prime ")
            strSql.AppendLine(" LEFT JOIN GruppoFinalita ON GruppoFinalita.Grfi_Cod = Materie_Prime.Grfi_Cod ")

            strSql.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                strSql.AppendLine(" AND (Materie_Prime.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' or Materie_Prime.Sa_Cod = -1 ) ")
            End If

            If Mat_Cod <> 0 AndAlso Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " AND Materie_Prime.Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod))
            End If

            If Veg_Cod <> 0 Then
                strSql.AppendLine(" AND Materie_Prime.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
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



Public Class Materie_Prime_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Elem_Cod As Integer,
                           ByVal Mat_Cod As Integer,
                           ByVal Mat_Des As String,
                           ByVal Sem_Cod As Integer,
                           ByVal Veg_Cod As Integer,
                           ByVal Cul_Cod As Integer,
                           ByVal Gen_Cod As Integer,
                           ByVal Spe_Cod As Integer,
                           ByVal IPro_Cod As Integer,
                           ByVal Raz_Cod As Integer,
                           ByVal Cat_Cod As Integer,
                           ByVal Flag_Biologico As Int16,
                           ByVal Flag_Convenzionale As Int16,
                           ByVal Flag_NonAgricolo As Int16,
                           ByVal Flag_AusiliareFabbricazione As Int16,
                           ByVal Trap_Dur As Integer,
                           ByVal Uso As Integer,
                           ByVal ClToss_Cod As String,
                           ByVal NewClToss_Cod As String,
                           ByVal Ditta_Cod As Integer,
                           ByVal N As Decimal,
                           ByVal P2O5 As Decimal,
                           ByVal K2O As Decimal,
                           ByVal MgO As Decimal,
                           ByVal Note As String,
                           ByVal Regolamento As Integer,
                           ByVal Cal_Cod As Integer,
                           ByVal Cod_Articolo As String,
                           ByVal Prezzo_Unitario As Decimal,
                           ByVal Extra_Int As Integer,
                           ByVal Extra_Str As String,
                           ByVal Extra_Date As Date,
                           ByVal Grva_Cod_Veg As Integer,
                           ByVal Flag_Extra As Int16,
                           ByVal ChkImballaggio As Int16,
                           ByVal Udm_Cod_Extra As Integer,
                           ByVal Qta_Extra As Decimal,
                           ByVal Taglio As Int16,
                           ByVal Grfi_Cod As Integer,
                           ByVal ChkListino As Int16,
                           ByVal Mat_Cod_Origine As Integer,
                           ByVal Piva_SuperUser_Origine As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal Codice_Esterno As String,
                            ByVal Flag_Importato As Int32,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           ByVal ID_DisciplinareAcquisti As Integer
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Materie_Prime ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("          Piva, Sa_Cod, Mat_Cod, Elem_Cod, Mat_Des, Veg_Cod, Cul_Cod, ")
            strSql.AppendLine("          Gen_Cod, Spe_Cod, IPro_Cod, Raz_Cod, Flag_Biologico, Flag_Convenzionale, Flag_NonAgricolo, Flag_AusiliareFabbricazione,  ")
            strSql.AppendLine("          Trap_Dur, Uso, CLTOSS_COD, NewCLTOSS_COD, Ditta_Cod, N, P2O5, K2O, MgO, Sem_Cod, Note,  ")
            strSql.AppendLine("          Regolamento, Grva_Cod_Veg, Cal_Cod, Cod_Articolo, Prezzo_Unitario, ")
            strSql.AppendLine("          Extra_Int, Extra_Str, Extra_Date, Flag_Extra, ChkImballaggio, Udm_Cod_Extra, Qta_Extra, Taglio, Grfi_Cod, ChkListino,")
            strSql.AppendLine("          Mat_Cod_Origine,    Piva_SuperUser_Origine,   ")
            strSql.AppendLine("          Cat_Cod, Codice_Esterno, Flag_Importato, ID_DisciplinareAcquisti,    ")

            strSql.AppendLine("          Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("         ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "'   ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Elem_Cod))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Mat_Des) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(IPro_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Biologico) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Convenzionale) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_NonAgricolo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_AusiliareFabbricazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Trap_Dur) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Uso) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(ClToss_Cod) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(NewClToss_Cod) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(P2O5) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(K2O) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(MgO) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sem_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Grva_Cod_Veg) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cal_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cod_Articolo) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Extra_Date) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Extra) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkImballaggio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Udm_Cod_Extra) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta_Extra) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Taglio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Grfi_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkListino) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "' ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Cat_Cod))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Codice_Esterno) & "' ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Flag_Importato))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(ID_DisciplinareAcquisti))

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine("        ) ")


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

    Public Function Scrivi_Completa(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Elem_Cod As Integer,
                                    ByVal Mat_Cod As Integer,
                                    ByVal Cod_Articolo As String,
                                    ByVal Mat_Des As String,
                                    ByVal Sem_Cod As Integer,
                                    ByVal Cul_Cod As Integer,
                                    ByVal Veg_Cod As Integer,
                                    ByVal Trap_Dur As Integer,
                                    ByVal Uso As Integer,
                                    ByVal ClToss_Cod As String,
                                    ByVal NewClToss_Cod As String,
                                    ByVal Ditta_Cod As Integer,
                                    ByVal N As Decimal,
                                    ByVal P2O5 As Decimal,
                                    ByVal K2O As Decimal,
                                    ByVal MgO As Decimal,
                                    ByVal Note As String,
                                    ByVal Regolamento As Integer,
                                    ByVal Cal_Cod As Integer,
                                     ByVal Prezzo_Unitario As Decimal,
                                    ByVal Extra_Int As Integer,
                                    ByVal Extra_Str As String,
                                    ByVal Extra_Date As Date,
                                    ByVal Grva_Cod_Veg As Integer,
                                    ByVal Gen_Cod As Integer,
                                    ByVal Spe_Cod As Integer,
                                    ByVal Raz_Cod As Integer,
                                    ByVal IPro_Cod As Integer,
                                    ByVal Cat_Cod As Integer,
                                    ByVal Flag_Biologico As Int16,
                                    ByVal Flag_Convenzionale As Int16,
                                    ByVal Flag_NonAgricolo As Int16,
                                    ByVal Flag_AusiliareFabbricazione As Int16,
                                    ByVal Udm_Cod_Extra As Integer,
                                    ByVal Flag_Extra As Int16,
                                    ByVal ChkImballaggio As Int16,
                                    ByVal Qta_Extra As Decimal,
                                    ByVal Taglio As Int16,
                                    ByVal Mat_Cod_Origine As Integer,
                                    ByVal Piva_SuperUser_Origine As String,
                                    ByVal ChkListino As Int16,
                                    ByVal Grfi_Cod As Integer,
                                    ByVal Codice_Prodotto As String,
                                    ByVal Colore As Integer,
                                    ByVal Codice_NC As String,
                                    ByVal Manipolazioni As Integer,
                                    ByVal Titolo_Alcol As Decimal,
                                    ByVal ChkContenitore As Int16,
                                    ByVal Qta_Contenitore As Decimal,
                                    ByVal Tipo_Peso As Int16,
                                    ByVal Tara As Decimal,
                                    ByVal Udm_Cod As Integer,
                                    ByVal Peso_Set As Int16,
                                    ByVal ChkEscludi_Magazzino As Int16,
                                    ByVal ChkEscludi_Preparazione As Int16,
                                    ByVal Id_Accisa_Cod As Integer,
                                    ByVal Confezione_Cod As String,
                                    ByVal Categoria_Vino_Cod As Int16,
                                    ByVal Tipo_Reg_Alcoli As String,
                                    ByVal ChkAlias As Int16,
                                    ByVal Linea_Cod As Integer,
                                    ByVal Tipo_Default As Integer,
                                    ByVal ChkStampa_Dettagli As Int16,
                                    ByVal ChkReferenza As Int16,
                                    ByVal Mat_Cod_Referenza As Integer,
                                    ByVal ID_DisciplinareAcquisti As Integer,
                                    ByVal Lotto_Default As String,
                                    ByVal OTabella_Cod_Base As Integer,
                                    ByVal Provenienza_TR As String,
                                    ByVal eBacchus As String,
                                    ByVal Flag_Variazione As Int16,
                                    ByVal Codice_Esterno As String,
                                    ByVal Flag_Importato As Int32,
                                    ByVal PrioritaCdG As Short,
                                    ByVal Cod_TecnologiaSementi As Integer,
                                    ByVal Germinabilita As Double,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Scrivi_Completa()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Materie_Prime ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("          Piva, Sa_Cod, Mat_Cod, Elem_Cod, Mat_Des, Veg_Cod, Cul_Cod, ")
            strSql.AppendLine("          Gen_Cod, Spe_Cod, IPro_Cod, Raz_Cod, Flag_Biologico, Flag_Convenzionale, Flag_NonAgricolo, Flag_AusiliareFabbricazione,  ")
            strSql.AppendLine("          Trap_Dur, Uso, CLTOSS_COD, NewCLTOSS_COD, Ditta_Cod, N, P2O5, K2O, MgO, Sem_Cod, Note,  ")
            strSql.AppendLine("          Regolamento, Grva_Cod_Veg, Cal_Cod, Cod_Articolo, Prezzo_Unitario, ")
            strSql.AppendLine("          Extra_Int, Extra_Str, Extra_Date, Flag_Extra, ChkImballaggio, Udm_Cod_Extra, Qta_Extra, Taglio, Grfi_Cod, ChkListino,")
            strSql.AppendLine("          Mat_Cod_Origine,    Piva_SuperUser_Origine,   ")
            strSql.AppendLine("          Cat_Cod, Codice_Esterno, Flag_Importato,    ")
            strSql.AppendLine("          ChkReferenza, Mat_Cod_Referenza,    ")
            strSql.AppendLine("          Udm_Cod, Linea_Cod, Tara ,   ")
            strSql.AppendLine("          ChkContenitore, ChkEscludi_Magazzino , ChkEscludi_Preparazione,  ")
            strSql.AppendLine("          Flag_Variazione, OTabella_Cod_Base , Peso_Set,  ")
            strSql.AppendLine("          Tipo_Peso, Confezione_Cod, Qta_Contenitore, ChkAlias, Priorita, Cod_TecnologiaSementi, Germinabilita, ")

            strSql.AppendLine("          Inviato,            DataInvio, ")
            strSql.AppendLine("          Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("          UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("         ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "'   ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod))
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Elem_Cod))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Mat_Des) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(IPro_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Biologico) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Convenzionale) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_NonAgricolo) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_AusiliareFabbricazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Trap_Dur) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Uso) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(ClToss_Cod) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(NewClToss_Cod) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(P2O5) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(K2O) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(MgO) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sem_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Grva_Cod_Veg) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Cal_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cod_Articolo) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Extra_Date) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Extra) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkImballaggio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Udm_Cod_Extra) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta_Extra) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Taglio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Grfi_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkListino) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod_Origine) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva_SuperUser_Origine) & "' ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Cat_Cod))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Codice_Esterno) & "' ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Flag_Importato))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(ChkReferenza))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Mat_Cod_Referenza))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Udm_Cod))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Linea_Cod))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Tara))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(ChkContenitore))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(ChkEscludi_Magazzino))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(ChkEscludi_Preparazione))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Flag_Variazione))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(OTabella_Cod_Base))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Peso_Set))
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Tipo_Peso))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Confezione_Cod) & "' ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Qta_Contenitore) & " ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(ChkAlias) & " ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(PrioritaCdG) & " ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Cod_TecnologiaSementi) & " ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(Germinabilita) & " ")


            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine("        ) ")


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


    Public Function Modifica_Completa(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Elem_Cod As Integer,
                                    ByVal Mat_Cod As Integer,
                                    ByVal Nuovo_Cod_Articolo As String,
                                    ByVal Cod_Articolo As String,
                                    ByVal Mat_Des As String,
                                    ByVal Sem_Cod As Integer?,
                                    ByVal Cul_Cod As Integer?,
                                    ByVal Veg_Cod As Integer?,
                                    ByVal Trap_Dur As Integer?,
                                    ByVal Uso As Integer?,
                                    ByVal ClToss_Cod As String,
                                    ByVal NewClToss_Cod As String,
                                    ByVal Ditta_Cod As Integer?,
                                    ByVal N As Decimal?,
                                    ByVal P2O5 As Decimal?,
                                    ByVal K2O As Decimal?,
                                    ByVal MgO As Decimal?,
                                    ByVal Note As String,
                                    ByVal Regolamento As Integer?,
                                    ByVal Cal_Cod As Integer?,
                                    ByVal Prezzo_Unitario As Decimal?,
                                    ByVal Extra_Int As Integer?,
                                    ByVal Extra_Str As String,
                                    ByVal Extra_Date As Date?,
                                    ByVal Grva_Cod_Veg As Integer?,
                                    ByVal Gen_Cod As Integer?,
                                    ByVal Spe_Cod As Integer?,
                                    ByVal Raz_Cod As Integer?,
                                    ByVal IPro_Cod As Integer?,
                                    ByVal Cat_Cod As Integer?,
                                    ByVal Flag_Biologico As Int16?,
                                    ByVal Flag_Convenzionale As Int16?,
                                    ByVal Flag_NonAgricolo As Int16?,
                                    ByVal Flag_AusiliareFabbricazione As Int16?,
                                    ByVal Udm_Cod_Extra As Integer?,
                                    ByVal Flag_Extra As Int16?,
                                    ByVal ChkImballaggio As Int16?,
                                    ByVal Qta_Extra As Decimal?,
                                    ByVal Taglio As Int16?,
                                    ByVal Mat_Cod_Origine As Integer?,
                                    ByVal Piva_SuperUser_Origine As String,
                                    ByVal ChkListino As Int16?,
                                    ByVal Grfi_Cod As Integer?,
                                    ByVal Codice_Prodotto As String,
                                    ByVal Colore As Integer?,
                                    ByVal Codice_NC As String,
                                    ByVal Manipolazioni As Integer?,
                                    ByVal Titolo_Alcol As Decimal?,
                                    ByVal ChkContenitore As Int16?,
                                    ByVal Qta_Contenitore As Decimal?,
                                    ByVal Tipo_Peso As Int16?,
                                    ByVal Tara As Decimal?,
                                    ByVal Udm_Cod As Integer?,
                                    ByVal Peso_Set As Int16?,
                                    ByVal ChkEscludi_Magazzino As Int16?,
                                    ByVal ChkEscludi_Preparazione As Int16?,
                                    ByVal Id_Accisa_Cod As Integer?,
                                    ByVal Confezione_Cod As String,
                                    ByVal Categoria_Vino_Cod As Int16?,
                                    ByVal Tipo_Reg_Alcoli As String,
                                    ByVal ChkAlias As Int16?,
                                    ByVal Linea_Cod As Integer?,
                                    ByVal Tipo_Default As Integer?,
                                    ByVal ChkStampa_Dettagli As Int16?,
                                    ByVal ChkReferenza As Int16?,
                                    ByVal Mat_Cod_Referenza As Integer?,
                                    ByVal ID_DisciplinareAcquisti As Integer?,
                                    ByVal Lotto_Default As String,
                                    ByVal OTabella_Cod_Base As Integer?,
                                    ByVal Provenienza_TR As String,
                                    ByVal eBacchus As String,
                                    ByVal Flag_Variazione As Int16?,
                                    ByVal Codice_Esterno As String,
                                    ByVal Flag_Importato As Int32?,
                                    ByVal PrioritaCdG As Short?,
                                    ByVal Cod_TecnologiaSementi As Integer?,
                                    ByVal Germinabilita As Double?,
                                    ByVal Validita_Inizio As Date?,
                                    ByVal Validita_Fine As Date?,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Modifica_Completa()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Materie_Prime Set ")
            strSql.AppendLine(" Mat_Des = '" & Agro_SQL_SaveText(Mat_Des) & "' ")
            strSql.AppendLine(" ,Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

            If Not Veg_Cod Is Nothing Then
                strSql.AppendLine(" ,Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
            End If
            If Not Cul_Cod Is Nothing Then
                strSql.AppendLine(" ,Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
            End If
            If Not Gen_Cod Is Nothing Then
                strSql.AppendLine(" ,Gen_Cod = " & Agro_SQL_SaveNum(Gen_Cod) & " ")
            End If
            If Not Spe_Cod Is Nothing Then
                strSql.AppendLine(" ,Spe_Cod = " & Agro_SQL_SaveNum(Spe_Cod) & " ")
            End If
            If Not IPro_Cod Is Nothing Then
                strSql.AppendLine(" ,IPro_Cod = " & Agro_SQL_SaveNum(IPro_Cod) & " ")
            End If
            If Not Raz_Cod Is Nothing Then
                strSql.AppendLine(" ,Raz_Cod = " & Agro_SQL_SaveNum(Raz_Cod) & " ")
            End If
            If Not Flag_Biologico Is Nothing Then
                strSql.AppendLine(" ,Flag_Biologico = " & Agro_SQL_SaveNum(Flag_Biologico) & " ")
            End If
            If Not Flag_Convenzionale Is Nothing Then
                strSql.AppendLine(" ,Flag_Convenzionale = " & Agro_SQL_SaveNum(Flag_Convenzionale) & " ")
            End If
            If Not Flag_NonAgricolo Is Nothing Then
                strSql.AppendLine(" ,Flag_NonAgricolo = " & Agro_SQL_SaveNum(Flag_NonAgricolo) & " ")
            End If
            If Not Flag_AusiliareFabbricazione Is Nothing Then
                strSql.AppendLine(" ,Flag_AusiliareFabbricazione = " & Agro_SQL_SaveNum(Flag_AusiliareFabbricazione) & " ")
            End If
            If Not Trap_Dur Is Nothing Then
                strSql.AppendLine(" ,Trap_Dur = " & Agro_SQL_SaveNum(Trap_Dur) & " ")
            End If
            If Not Uso Is Nothing Then
                strSql.AppendLine(" ,Uso = " & Agro_SQL_SaveNum(Uso) & " ")
            End If
            If Not String.IsNullOrEmpty(ClToss_Cod) Then
                strSql.AppendLine(" ,ClToss_Cod = '" & Agro_SQL_SaveText(ClToss_Cod) & "' ")
            End If
            If Not String.IsNullOrEmpty(NewClToss_Cod) Then
                strSql.AppendLine(" ,NewCLTOSS_COD = '" & Agro_SQL_SaveText(NewClToss_Cod) & "' ")
            End If
            If Not Ditta_Cod Is Nothing Then
                strSql.AppendLine(" ,Ditta_Cod = " & Agro_SQL_SaveNum(Ditta_Cod) & " ")
            End If
            If Not N Is Nothing Then
                strSql.AppendLine(" ,N = " & Agro_SQL_SaveNum(N) & " ")
            End If
            If Not P2O5 Is Nothing Then
                strSql.AppendLine(" ,P2O5 = " & Agro_SQL_SaveNum(P2O5) & " ")
            End If
            If Not K2O Is Nothing Then
                strSql.AppendLine(" ,K2O = " & Agro_SQL_SaveNum(K2O) & " ")
            End If
            If Not MgO Is Nothing Then
                strSql.AppendLine(" ,MgO = " & Agro_SQL_SaveNum(MgO) & " ")
            End If
            If Not Sem_Cod Is Nothing Then
                strSql.AppendLine(" ,Sem_Cod = " & Agro_SQL_SaveNum(Sem_Cod) & " ")
            End If
            If Not String.IsNullOrEmpty(Note) Then
                strSql.AppendLine(" ,Note = '" & Agro_SQL_SaveText(Note) & "' ")
            End If
            If Not Regolamento Is Nothing Then
                strSql.AppendLine(" ,Regolamento = " & Agro_SQL_SaveNum(Regolamento) & " ")
            End If
            If Not Grva_Cod_Veg Is Nothing Then
                strSql.AppendLine(" ,Grva_Cod_Veg = " & Agro_SQL_SaveNum(Grva_Cod_Veg) & " ")
            End If
            If Not Cal_Cod Is Nothing Then
                strSql.AppendLine(" ,Cal_Cod = " & Agro_SQL_SaveNum(Cal_Cod) & " ")
            End If
            If Not String.IsNullOrEmpty(Nuovo_Cod_Articolo) Then
                strSql.AppendLine(" ,Cod_Articolo = '" & Agro_SQL_SaveText(Nuovo_Cod_Articolo) & "' ")
            End If
            If Not Prezzo_Unitario Is Nothing Then
                strSql.AppendLine(" ,Prezzo_Unitario = " & Agro_SQL_SaveNum(Prezzo_Unitario) & " ")
            End If
            If Not Extra_Int Is Nothing Then
                strSql.AppendLine(" ,Extra_Int = " & Agro_SQL_SaveNum(Extra_Int) & " ")
            End If
            If Not String.IsNullOrEmpty(Extra_Str) Then
                strSql.AppendLine(" ,Extra_Str = '" & Agro_SQL_SaveText(Extra_Str) & "' ")
            End If
            If Not Extra_Date Is Nothing Then
                strSql.AppendLine(" ,Extra_Date = " & Agro_SQL_SaveDate(Extra_Date) & " ")
            End If
            If Not Flag_Extra Is Nothing Then
                strSql.AppendLine(" ,Flag_Extra = " & Agro_SQL_SaveNum(Flag_Extra) & " ")
            End If
            If Not ChkImballaggio Is Nothing Then
                strSql.AppendLine(" ,ChkImballaggio = " & Agro_SQL_SaveNum(ChkImballaggio) & " ")
            End If
            If Not Udm_Cod_Extra Is Nothing Then
                strSql.AppendLine(" ,Udm_Cod_Extra = " & Agro_SQL_SaveNum(Udm_Cod_Extra) & " ")
            End If
            If Not Qta_Extra Is Nothing Then
                strSql.AppendLine(" ,Qta_Extra = " & Agro_SQL_SaveNum(Qta_Extra) & " ")
            End If
            If Not Taglio Is Nothing Then
                strSql.AppendLine(" ,Taglio = " & Agro_SQL_SaveNum(Taglio) & " ")
            End If
            If Not Grfi_Cod Is Nothing Then
                strSql.AppendLine(" ,Grfi_Cod = " & Agro_SQL_SaveNum(Grfi_Cod) & " ")
            End If
            If Not ChkListino Is Nothing Then
                strSql.AppendLine(" ,ChkListino = " & Agro_SQL_SaveNum(ChkListino) & " ")
            End If
            If Not Mat_Cod_Origine Is Nothing Then
                strSql.AppendLine(" ,Mat_Cod_Origine = " & Agro_SQL_SaveNum(Mat_Cod_Origine) & " ")
            End If
            If Not Cat_Cod Is Nothing Then
                strSql.AppendLine(" ,Cat_Cod = " & Agro_SQL_SaveNum(Cat_Cod) & " ")
            End If
            If Not String.IsNullOrEmpty(Codice_Esterno) Then
                strSql.AppendLine(" ,Codice_Esterno = '" & Agro_SQL_SaveText(Codice_Esterno) & "' ")
            End If
            If Not Flag_Importato Is Nothing Then
                strSql.AppendLine(" ,Flag_Importato = " & Agro_SQL_SaveNum(Flag_Importato) & " ")
            End If
            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            If Not Validita_Inizio Is Nothing Then
                strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            End If
            If Not Validita_Fine Is Nothing Then
                strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            End If
            If Not ChkReferenza Is Nothing Then
                strSql.AppendLine(" ,ChkReferenza = " & Agro_SQL_SaveNum(ChkReferenza) & " ")
            End If
            If Not Mat_Cod_Referenza Is Nothing Then
                strSql.AppendLine(" ,Mat_Cod_Referenza = " & Agro_SQL_SaveNum(Mat_Cod_Referenza) & " ")
            End If
            If Not Udm_Cod Is Nothing Then
                strSql.AppendLine(" ,Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & " ")
            End If
            If Not ChkAlias Is Nothing Then
                strSql.AppendLine(" ,ChkAlias = " & Agro_SQL_SaveNum(ChkAlias) & " ")
            End If
            If Not Linea_Cod Is Nothing Then
                strSql.AppendLine(" ,Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " ")
            End If
            If Not Tara Is Nothing Then
                strSql.AppendLine(" ,Tara = " & Agro_SQL_SaveNum(Tara) & " ")
            End If
            If Not ChkContenitore Is Nothing Then
                strSql.AppendLine(" ,ChkContenitore = " & Agro_SQL_SaveNum(ChkContenitore) & " ")
            End If
            If Not ChkEscludi_Magazzino Is Nothing Then
                strSql.AppendLine(" ,ChkEscludi_Magazzino = " & Agro_SQL_SaveNum(ChkEscludi_Magazzino) & " ")
            End If
            If Not ChkEscludi_Preparazione Is Nothing Then
                strSql.AppendLine(" ,ChkEscludi_Preparazione = " & Agro_SQL_SaveNum(ChkEscludi_Preparazione) & " ")
            End If
            If Not Flag_Variazione Is Nothing Then
                strSql.AppendLine(" ,Flag_Variazione = " & Agro_SQL_SaveNum(Flag_Variazione) & " ")
            End If
            If Not OTabella_Cod_Base Is Nothing Then
                strSql.AppendLine(" ,OTabella_Cod_Base = " & Agro_SQL_SaveNum(OTabella_Cod_Base) & " ")
            End If
            If Not Peso_Set Is Nothing Then
                strSql.AppendLine(" ,Peso_Set = " & Agro_SQL_SaveNum(Peso_Set) & " ")
            End If
            If Not Tipo_Peso Is Nothing Then
                strSql.AppendLine(" ,Tipo_Peso = " & Agro_SQL_SaveNum(Tipo_Peso) & " ")
            End If
            If Not Confezione_Cod Is Nothing Then
                strSql.AppendLine(" ,Confezione_Cod = '" & Agro_SQL_SaveText(Confezione_Cod) & "' ")
            End If
            If Not Qta_Contenitore Is Nothing Then
                strSql.AppendLine(" ,Qta_Contenitore = " & Agro_SQL_SaveNum(Qta_Contenitore) & " ")
            End If
            If Not PrioritaCdG Is Nothing Then
                strSql.AppendLine(" ,Priorita = " & Agro_SQL_SaveNum(PrioritaCdG) & " ")
            End If
            If Not Cod_TecnologiaSementi Is Nothing Then
                strSql.AppendLine(" ,Cod_TecnologiaSementi = " & Agro_SQL_SaveNum(Cod_TecnologiaSementi) & " ")
            End If
            If Not Germinabilita Is Nothing Then
                strSql.AppendLine(" ,Germinabilita = " & Agro_SQL_SaveNum(Germinabilita) & " ")
            End If

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")
            strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & " ")
            If Not String.IsNullOrEmpty(Cod_Articolo) Then
                strSql.AppendLine(" AND Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' ")
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

    'Introduzione in chiave del campo "Cod_Articolo"
    '============================================================================
    Public Function Modifica2(ByVal New_Piva As String,
                              ByVal New_Sa_Cod As Integer,
                              ByVal New_Cod_Articolo As String,
                              ByVal Elem_Cod As Integer,
                              ByVal Mat_Cod As Integer,
                              ByVal Mat_Des As String,
                              ByVal Sem_Cod As Integer,
                              ByVal Veg_Cod As Integer,
                              ByVal Cul_Cod As Integer,
                              ByVal Gen_Cod As Integer,
                              ByVal Spe_Cod As Integer,
                              ByVal IPro_Cod As Integer,
                              ByVal Raz_Cod As Integer,
                              ByVal Cat_Cod As Integer,
                              ByVal Flag_Biologico As Int16,
                              ByVal Flag_Convenzionale As Int16,
                              ByVal Flag_NonAgricolo As Int16,
                              ByVal Flag_AusiliareFabbricazione As Int16,
                              ByVal Trap_Dur As Integer,
                              ByVal Uso As Integer,
                              ByVal ClToss_Cod As String,
                              ByVal NewClToss_Cod As String,
                              ByVal Ditta_Cod As Integer,
                              ByVal N As Decimal,
                              ByVal P2O5 As Decimal,
                              ByVal K2O As Decimal,
                              ByVal MgO As Decimal,
                              ByVal Note As String,
                              ByVal Regolamento As Integer,
                              ByVal Cal_Cod As Integer,
                              ByVal Cod_Articolo As String,
                              ByVal Prezzo_Unitario As Decimal,
                              ByVal Extra_Int As Integer,
                              ByVal Extra_Str As String,
                              ByVal Extra_Date As Date,
                              ByVal Flag_Extra As Int16,
                              ByVal ChkImballaggio As Int16,
                              ByVal Udm_Cod_Extra As Integer,
                              ByVal Qta_Extra As Decimal,
                              ByVal Grva_Cod_Veg As Integer,
                              ByVal Taglio As Int16,
                              ByVal Grfi_Cod As Integer,
                              ByVal ChkListino As Int16,
                              ByVal Validita_Inizio As Date,
                              ByVal Validita_Fine As Date,
                               ByVal Codice_Esterno As String,
                                ByVal Flag_Importato As Int32,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               ByVal ID_DisciplinareAcquisti As Integer
                              ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Modifica2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Mat_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mat_Cod obbligatorio)")
            End If

            If Elem_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Elem_Cod obbligatorio)")
            End If

            If Cod_Articolo = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Articolo obbligatorio)")
            End If


            '---------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" UPDATE Materie_Prime SET ")
            strSql.AppendLine("    Piva              = '" & Agro_SQL_SaveText(New_Piva) & "'  ")
            strSql.AppendLine("   ,Sa_Cod            =  " & Agro_SQL_SaveNum(New_Sa_Cod) & "  ")
            strSql.AppendLine("   ,Mat_Des           = '" & Agro_SQL_SaveText(Mat_Des) & "'  ")
            strSql.AppendLine("   ,Veg_Cod           =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            strSql.AppendLine("   ,Cul_Cod           =  " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            strSql.AppendLine("   ,Gen_Cod           =  " & Agro_SQL_SaveNum(Gen_Cod) & "  ")
            strSql.AppendLine("   ,Spe_Cod           =  " & Agro_SQL_SaveNum(Spe_Cod) & "  ")
            strSql.AppendLine("   ,IPro_Cod          =  " & Agro_SQL_SaveNum(IPro_Cod) & "  ")
            strSql.AppendLine("   ,Raz_Cod           =  " & Agro_SQL_SaveNum(Raz_Cod) & "  ")
            strSql.AppendLine("   ,Flag_Biologico               =  " & Agro_SQL_SaveNum(Flag_Biologico) & "  ")
            strSql.AppendLine("   ,Flag_Convenzionale           =  " & Agro_SQL_SaveNum(Flag_Convenzionale) & "  ")
            strSql.AppendLine("   ,Flag_NonAgricolo             =  " & Agro_SQL_SaveNum(Flag_NonAgricolo) & "  ")
            strSql.AppendLine("   ,Flag_AusiliareFabbricazione  =  " & Agro_SQL_SaveNum(Flag_AusiliareFabbricazione) & "  ")
            strSql.AppendLine("   ,Trap_Dur          =  " & Agro_SQL_SaveNum(Trap_Dur) & "  ")
            strSql.AppendLine("   ,Uso               =  " & Agro_SQL_SaveNum(Uso) & "  ")
            strSql.AppendLine("   ,CLTOSS_COD        = '" & Agro_SQL_SaveText(ClToss_Cod) & "'  ")
            strSql.AppendLine("   ,NewCLTOSS_COD     = '" & Agro_SQL_SaveText(NewClToss_Cod) & "'  ")
            strSql.AppendLine("   ,Ditta_Cod         =  " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.AppendLine("   ,N                 =  " & Agro_SQL_SaveNum(N) & "  ")
            strSql.AppendLine("   ,P2O5              =  " & Agro_SQL_SaveNum(P2O5) & "  ")
            strSql.AppendLine("   ,K2O               =  " & Agro_SQL_SaveNum(K2O) & "  ")
            strSql.AppendLine("   ,MgO               =  " & Agro_SQL_SaveNum(MgO) & "  ")
            strSql.AppendLine("   ,Sem_Cod           =  " & Agro_SQL_SaveNum(Sem_Cod) & "  ")
            strSql.AppendLine("   ,Regolamento       =  " & Agro_SQL_SaveNum(Regolamento) & "  ")
            strSql.AppendLine("   ,Grva_Cod_Veg      =  " & Agro_SQL_SaveNum(Grva_Cod_Veg) & "  ")
            strSql.AppendLine("   ,Cal_Cod           =  " & Agro_SQL_SaveNum(Cal_Cod) & "  ")
            strSql.AppendLine("   ,Cod_Articolo      =  '" & Agro_SQL_SaveText(New_Cod_Articolo) & "'  ")
            strSql.AppendLine("   ,Prezzo_Unitario   =  " & Agro_SQL_SaveNum(Prezzo_Unitario) & "  ")
            strSql.AppendLine("   ,Flag_Extra        =  " & Agro_SQL_SaveNum(Flag_Extra) & "  ")
            strSql.AppendLine("   ,ChkImballaggio    =  " & Agro_SQL_SaveNum(ChkImballaggio) & "  ")
            strSql.AppendLine("   ,Udm_Cod_Extra     =  " & Agro_SQL_SaveNum(Udm_Cod_Extra) & "  ")
            strSql.AppendLine("   ,Qta_Extra         =  " & Agro_SQL_SaveNum(Qta_Extra) & "  ")
            strSql.AppendLine("   ,Extra_Int         =  " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.AppendLine("   ,Extra_Str         =  '" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.AppendLine("   ,Extra_Date        =  " & Agro_SQL_SaveDate(Extra_Date))
            strSql.AppendLine("   ,Note              = '" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.AppendLine("   ,Taglio            =  " & Agro_SQL_SaveNum(Taglio) & "  ")
            strSql.AppendLine("   ,Grfi_Cod          =  " & Agro_SQL_SaveNum(Grfi_Cod) & "  ")
            strSql.AppendLine("   ,ChkListino        =  " & Agro_SQL_SaveNum(ChkListino) & "  ")
            strSql.AppendLine("   ,Cat_Cod        =  " & Agro_SQL_SaveNum(Cat_Cod) & "  ")
            strSql.AppendLine("   ,Codice_Esterno   =  '" & Agro_SQL_SaveText(Codice_Esterno) & "'  ")
            strSql.AppendLine("   ,Flag_Importato   =  " & Agro_SQL_SaveNum(Flag_Importato) & "  ")
            strSql.AppendLine("   ,ID_DisciplinareAcquisti   =  " & Agro_SQL_SaveNum(ID_DisciplinareAcquisti) & "  ")

            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.AppendLine(" WHERE Mat_Cod      =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.AppendLine(" AND   Elem_Cod     =  " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.AppendLine(" AND   Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "'  ")

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


    Public Function Modifica_solo_Veg_Cul_cod_ID_DisciplinareAcquisti(ByVal Mat_Cod As Integer,
                                                                      ByVal Veg_Cod As Integer,
                                                                      ByVal Cul_Cod As Integer,
                                                                      ByVal ID_DisciplinareAcquisti As Integer,
                                                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                                      ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Modifica_solo_Veg_Cul_cod_ID_DisciplinareAcquisti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Mat_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mat_Cod obbligatorio)")
            End If


            '---------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" UPDATE Materie_Prime SET ")
            strSql.AppendLine("   Veg_Cod           =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            strSql.AppendLine("   ,Cul_Cod          =  " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            strSql.AppendLine("   ,ID_DisciplinareAcquisti             =  " & Agro_SQL_SaveNum(ID_DisciplinareAcquisti) & " ")


            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            '29/06/2020: dopo consulto, deciso di non aggiornare la data_modifica dell'articolo per non causare problemi sulle
            'anagrafiche di Fruttagel che sono importate da JDE:
            'vengono importate con veg_cod e cul_cod = 0 e, durante l'inserimento dei PDC sulle pagine AltreAnalisi.aspx e AltriCampionamenti.aspx,
            'viene fatto update di veg_cod e cul_cod;
            'l'aggiornamento della data_modifica su GIAS, che viene controllata con la data modifica dell'articolo JDE, rischia di non far aggiornare l'articolo
            'perchè risulta "più nuovo".
            'rif mail "FRUTTAGEL assistenza PDC & anagrafiche articoli "aggiornate" dai PDC"
            'strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            '15/09/2020: commentato anche l'update di username_modifica altrimenti si generava disallineamento tra chi aveva modificato e quando
            'strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE Mat_Cod      =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")

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




    Public Function Modifica_solo_Veg_Cul_cod_Note(ByVal Mat_Cod As Integer,
                                                   ByVal Veg_Cod As Integer,
                                                   ByVal Cul_Cod As Integer,
                                                   ByVal Note As String,
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                   ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Modifica_solo_Veg_Cul_cod()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Mat_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mat_Cod obbligatorio)")
            End If


            '---------------------------------------------

            strSql.Length = 0

            strSql.AppendLine(" UPDATE Materie_Prime SET ")
            strSql.AppendLine("   Veg_Cod           =  " & Agro_SQL_SaveNum(Veg_Cod) & "  ")
            strSql.AppendLine("   ,Cul_Cod          =  " & Agro_SQL_SaveNum(Cul_Cod) & "  ")
            strSql.AppendLine("   ,Note             =  '" & Agro_SQL_SaveText(Note) & "'  ")


            strSql.AppendLine("   ,Inviato           =  0 ")
            strSql.AppendLine("   ,DataInvio         =  Null ")
            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.AppendLine(" WHERE Mat_Cod      =  " & Agro_SQL_SaveNum(Mat_Cod) & "  ")

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





    'Routine per la Cancellazione della Materia Prima dalle tabelle:
    '- Materie_Prime
    '- Materie_Prme_ParametriQualitativi
    '- Materie_PrimexLotto_Configurazione
    '- Materie_Prime_Dettagli
    '============================================================================
    Public Function Cancella2(ByVal Elem_Cod As Integer,
                              ByVal Mat_Cod As Integer,
                              ByVal Piva As String,
                              ByVal Cod_Articolo As String,
                              ByVal xFiltroAggiuntivo As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Cancella2()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Cod_Articolo = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Mat_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mat_Cod obbligatorio)")
            End If


            '-------------------------------------------------------------------------
            '---------- Materie_Prime ------------------------------------------------
            '-------------------------------------------------------------------------

            strSql.Length = 0

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.AppendLine(" UPDATE Materie_Prime ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      , Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else

                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Materie_Prime ")
                strSql.AppendLine(" WHERE  1=1 ")

            End If

            strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            '-------------------------------------------------------------------------
            '---------- Materie_Prime_ParametriQualitativi ---------------------------
            '-------------------------------------------------------------------------

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.AppendLine(" UPDATE Materie_Prime_ParametriQualitativi ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      , Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else

                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Materie_Prime_ParametriQualitativi ")
                strSql.AppendLine(" WHERE  1=1 ")

            End If

            strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

            '-------------------------------------------------------------------------
            '---------- Materie_PrimexLotto_Configurazione ---------------------------
            '-------------------------------------------------------------------------

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.AppendLine(" UPDATE Materie_PrimexLotto_Configurazione ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      , Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else

                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Materie_PrimexLotto_Configurazione ")
                strSql.AppendLine(" WHERE  1=1 ")

            End If

            strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            '-------------------------------------------------------------------------
            '---------- Materie_Prime_Dettagli ---------------------------------------
            '-------------------------------------------------------------------------

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.AppendLine(" UPDATE Materie_Prime_Dettagli ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      , Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0 ")

            Else

                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM Materie_Prime_Dettagli ")
                strSql.AppendLine(" WHERE  1=1 ")

            End If

            strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")

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


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Elem_Cod"></param>
    ''' <param name="Mat_Cod"></param>
    ''' <param name="Piva"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	04/02/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Cancella(ByVal Elem_Cod As Integer,
                             ByVal Mat_Cod As Integer,
                             ByVal Piva As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Cancella()"

        '============================================================================
        'Se tento di cancellare fisicamente un record con INVIATO=1
        'allora pongo INVIATO=-1
        'questo per consentire la risincronizzazione col server
        '============================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Mat_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mat_Cod obbligatorio)")
            End If


            '-------------------------------------------------------------------------
            '---------- Materie_Prime ------------------------------------------------
            '-------------------------------------------------------------------------

            strSql.Length = 0

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.AppendLine(" UPDATE  Materie_Prime ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine(" Username_Modifica = '" & objParametri.UsernameOperazione & "'  ")
                strSql.AppendLine(" , Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine(" ,Inviato = -1 ")
                strSql.AppendLine(" WHERE Inviato > 0 ")


            Else

                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM  Materie_Prime ")
                strSql.AppendLine(" WHERE Inviato = 0 ")

            End If

            If Mat_Cod <> 0 Then
                strSql.AppendLine(" AND Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

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



    Public Function CancellaListaMatCod(ByVal Elem_Cod As Integer,
                                        ByVal Mat_Cod As String,
                                        ByVal Piva As String,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.CancellaListaMatCod()"

        '============================================================================
        'Se tento di cancellare fisicamente un record con INVIATO=1
        'allora pongo INVIATO=-1
        'questo per consentire la risincronizzazione col server
        '============================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Mat_Cod = "" Then
                Throw New Exception("Parametro non corretto nella query (Mat_Cod obbligatorio)")
            End If


            '-------------------------------------------------------------------------
            '---------- Materie_Prime ------------------------------------------------
            '-------------------------------------------------------------------------

            strSql.Length = 0

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.AppendLine(" UPDATE  Materie_Prime ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine(" Username_Modifica = '" & objParametri.UsernameOperazione & "'  ")
                strSql.AppendLine(" , Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine(" ,Inviato = -1 ")
                strSql.AppendLine(" WHERE Inviato > 0 ")


            Else

                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM  Materie_Prime ")
                strSql.AppendLine(" WHERE Inviato = 0 ")

            End If

            If Mat_Cod <> "" Then
                strSql.AppendLine(" AND Mat_Cod in  (" & Mat_Cod & ")   ")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

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

    '============================================================================
    Public Function MateriePrime_MarcaComeInviato(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Mat_Cod As Integer,
                                                  ByVal Data_invio As DateTime,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                  ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime.MateriePrime_MarcaComeInviato()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'Non va bene, perché nelle operazioni contabili il sa_cod deve essere =0
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Mat_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Mat_Cod_ORIGINE obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Materie_Prime SET ")
            strSql.AppendLine("     inviato         =  -2 ")
            strSql.AppendLine("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            strSql.AppendLine(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.AppendLine(" AND     Mat_Cod   = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            'strSql.AppendLine(" AND     PivaSuperUser   = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")


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

    Public Function Scrivi_Materie_Prime_XLingue(ByVal Piva As String,
                                              ByVal Elem_Cod As Integer,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Mat_Cod As Integer,
                                              ByVal Lingua_Cod As Integer,
                                              ByVal Mat_Des As String,
                                              ByVal Validita_Inizio As Date?,
                                              ByVal Validita_Fine As Date?,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                              ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Scrivi_Materie_Prime_XLingue()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Materie_Prime_XLingue ")
            strSql.AppendLine("         ( ")
            strSql.AppendLine("          Lingua_Cod, Piva,  Elem_Cod, Sa_Cod, Mat_Cod, Mat_Des, ")
            strSql.AppendLine("          DATA_AGG, inviato, dataInvio, ")
            strSql.AppendLine("          Data_Creazione, Data_Modifica, ")
            strSql.AppendLine("          Username_Creazione, Username_Modifica, ")
            strSql.AppendLine("          Validita_Inizio,Validita_Fine ")
            strSql.AppendLine("         ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          " & Agro_SQL_SaveNum(Lingua_Cod))
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "'   ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Mat_Des) & "'   ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            If Not Validita_Inizio Is Nothing AndAlso Validita_Inizio.HasValue Then
                strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            End If

            If Not Validita_Fine Is Nothing AndAlso Validita_Fine.HasValue Then
                strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            End If

            strSql.AppendLine("        ) ")

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

    Public Function Modifica_Materie_Prime_XLingue(ByVal Piva As String,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Sa_Cod_Modificato As Integer?,
                                                ByVal Sa_Cod As Integer?,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Lingua_Cod_Modificata As Integer,
                                                ByVal Lingua_Cod As Integer,
                                                ByVal Mat_Des As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Modifica_Materie_Prime_XLingue()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Materie_Prime_XLingue SET ")
            strSql.AppendLine("   Elem_Cod          =  " & Agro_SQL_SaveNum(Elem_Cod) & "  ")

            If Not IsNothing(Sa_Cod_Modificato) Then
                strSql.AppendLine("    ,Sa_Cod        = " & Agro_SQL_SaveNum(Sa_Cod_Modificato) & "  ")
            End If

            If Lingua_Cod_Modificata <> 0 Then
                strSql.AppendLine("    ,Lingua_Cod        = " & Agro_SQL_SaveNum(Lingua_Cod_Modificata) & "  ")
            End If

            strSql.AppendLine("   ,Mat_Cod           = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.AppendLine("   ,Mat_Des           = '" & Agro_SQL_SaveText(Mat_Des) & "'  ")

            strSql.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            strSql.AppendLine(" WHERE Piva      =  '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine(" AND   Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            strSql.AppendLine(" AND   Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            strSql.AppendLine(" AND   Lingua_Cod     =  " & Agro_SQL_SaveNum(Lingua_Cod) & "  ")

            If Not IsNothing(Sa_Cod) Then
                strSql.AppendLine(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If


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


    Public Function Cancella_Materie_Prime_XLingue(ByVal Piva As String,
                                                ByVal Elem_Cod As Integer,
                                                ByVal Sa_Cod As Integer?,
                                                ByVal Mat_Cod As Integer,
                                                ByVal Lingua_Cod As Integer,
                                                ByVal Mat_Des As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Materie_Prime_W.Cancella_Materie_Prime_XLingue()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.AppendLine(" DELETE ")
            strSql.AppendLine(" FROM  Materie_Prime_XLingue ")
            strSql.AppendLine(" WHERE Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod) & " ")

            If Lingua_Cod <> 0 Then
                strSql.AppendLine(" AND Lingua_Cod = " & Agro_SQL_SaveNum(Lingua_Cod) & "")
            End If

            If Elem_Cod <> 0 Then
                strSql.AppendLine(" AND Elem_Cod = " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            End If

            If Not IsNothing(Sa_Cod) Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND  Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

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

End Class
