
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Text

Public Class AlberoAnagraficaFaster_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Private _arr As String()
    Private _suffissoAlias As String

    Private QImpresa As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QCentroAziendale As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QCampo As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QCatastoAziendale As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QParticella As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QPlannnigEtichetta As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QPlannnigTestata As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QPlannnigEntita As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QAnagrafica As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QAppezzamento As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)
    Private QImpianto As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave)

    Public Property SuffissoAlias As String
        Get
            Return _suffissoAlias
        End Get
        Set(value As String)
            _suffissoAlias = value
        End Set
    End Property

    Public Sub New()
        _suffissoAlias = "_al__"

        QImpresa = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_Impresa("i.")
        QCentroAziendale = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_CentroAziendale("sa.")

        QCatastoAziendale = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_Catasto("sa.")
        QParticella = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_Particella("ipp.", "p.")

        QPlannnigEtichetta = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_PlanningEtichetta()
        QPlannnigTestata = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_PlanningTestata("e.")
        QPlannnigEntita = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_PlanningEntita("e1.")

        QAnagrafica = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_Anagrafica()
        QCampo = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_Campo("cc.")
        QAppezzamento = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_Appezzamento(enum_TipoNodo.Appezzamento, "a.", "")
        QImpianto = AgronicaCoreDataProvider.Albero.Albero_Posizioni_Query.PQ_Impianto("app.", "imp.", "Anag1.")

    End Sub

    Private Shared Function LeggiDescrizioneNodoUtente(
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String

        '************************
        '***** NODO UTENTE ******
        '************************

        Dim testo As String

        Dim objUtente As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim DTUtente As DataTable

        DTUtente = objUtente.Leggi(objParametri_Server.UtenteUsername,
                                   5,
                                   enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                   "", "", objParametri_Utenti)
        Dim TipoUtente As Integer

        'Prelevo la Ragione Sociale oppure Nome e Cognome
        If DTUtente.Rows.Count > 0 Then
            'Verifico il tipo di utente ... Azienda/Persona
            TipoUtente = DTUtente.Rows(0).Item("Flag_Azienda_Persona")
            If TipoUtente = 1 Then
                Testo = DTUtente.Rows(0).Item("Rag_Soc")
            Else
                Testo = DTUtente.Rows(0).Item("Cognome") & " " &
                        DTUtente.Rows(0).Item("Nome")
            End If
        Else
            testo = objParametri_Server.UtenteUsername
        End If

        Return testo

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="ApplicaFiltroUtentiVisibilitaAppoggio"></param>
    ''' <param name="letturaCatasto"></param>
    ''' <param name="letturaCatasto_Appezzamenti"></param>
    ''' <param name="letturaPlanning"></param>
    ''' <param name="letturaRicette"></param>
    ''' <param name="letturaAnagrafica"></param>
    ''' <param name="letturaAnalisi"></param>
    ''' <param name="CodiceFiscaleTecnico"></param>
    ''' <param name="NumeroDiAliasImpostati">il numero di alias impostati per i nodi di pari livello, serve al chiamate per identificarli ed effettuare opportune sostituzioni</param>
    ''' <param name="objParametri_Server"></param>
    ''' <param name="objParametri_Utenti"></param>
    ''' <returns></returns>
    Public Function LeggiViaJsonSQL(
        ByVal cfg As AlberoAnagraficaFasterModel.AlberoAnagraficaFasterModelCfg,
        ByRef NumeroDiAliasImpostati As Integer,
        ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As String



        '----- Descrizione
        Dim NomeRoutine As String = "AgronicaCore_DAL.Leggi()"

        '----- Variabili
        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim jSon As String = ""

        NumeroDiAliasImpostati = 0

        Try

            Dim NomeDB_Utenti As String = objParametri_Utenti.Recupera_NomeDB()

            cfg.dataInizio = objParametri_Server.FinestraTemporaleInizio
            cfg.dataFine = objParametri_Server.FinestraTemporaleFine

            Stb.Length = 0
            Stb.AppendLine(" declare @Username varchar(500) ")
            Stb.AppendLine(" set @Username = '" & objParametri_Server.UtenteUsername & "' ")


            ' VAnni: 31/3/2021: non necessaria se non in sql management studio
            'Stb.AppendLine(" Declare @JSON nvarchar(max) ")
            'Stb.AppendLine("Set @JSON = (   ")

            Stb.AppendLine(" select ")

            Dim testoUtente As String =
                LeggiDescrizioneNodoUtente(objParametri_Server, objParametri_Utenti)

            JsonTestata(My.Resources.AgronicaCoreAnagrafeDAL.Utente, testoUtente, AgronicaCoreDataProvider.Albero.Albero_Icone.IconaUtente, Stb)

            Stb.AppendLine("  , ( ")
            Stb.AppendLine("  select  ")

            JsonDettaglio(-1, "'{" & My.Resources.AgronicaCoreAnagrafeDAL.Impresa & "} : ' + i.rag_soc", "''", "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconaImpresa & "'", QImpresa, Stb)

            Stb.AppendLine("      , ( ")
            Stb.AppendLine("          select  ")
            JsonDettaglio(-1, "sa.sa_nome", "''", "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconaCentroAziendale & "'", QCentroAziendale, Stb)

            If cfg.Flag_CatastoAziendale Then

                Stb.AppendLine("              , ( ")
                LetturaNodoCatastoXCentriAziendali(NumeroDiAliasImpostati, Stb)
                Stb.AppendLine("              ) as items --nodo padre catasto ")

            End If
            'lettura catasto

            If cfg.Flag_Planning Then

                Stb.AppendLine("              , ( ")
                LetturaNodoPlanning(
                    NumeroDiAliasImpostati,
                    cfg,
                    Stb)
                Stb.AppendLine("              ) as items" & _suffissoAlias & NumeroDiAliasImpostati & " --nodo padre planning ") 'fine nodo padre planning

            End If
            'letturaPlanning 

            If cfg.Flag_Anagrafica Then

                Stb.AppendLine("              , ( ")
                LetturaNodoAnagraficaReale(
                    NumeroDiAliasImpostati,
                    cfg,
                    Stb
                )
                Stb.AppendLine("              ) as items" & _suffissoAlias & NumeroDiAliasImpostati & " --nodo padre anagrafica ") 'fine nodo padre anagrafica

            End If
            'Anagrafica reale


            Stb.AppendLine("  ")
            Stb.AppendLine("          from centri_Aziendali sa             ")

            If cfg.ApplicaFiltroUtentiVisibilitaAppoggio Then
                Stb.AppendLine("          inner join Utenti_Visibilita_Appoggio t (NOLOCK) ")
                Stb.AppendLine("              on sa.piva = t.piva  ")
                Stb.AppendLine("              and sa.sa_Cod = t.sa_cod ")
                Stb.AppendLine("              and t.Entita_Cod = 2 ")
                Stb.AppendLine("              and t.username = @username ")
            End If

            Stb.AppendLine("          where sa.piva = i.piva ")
            If cfg.Sa_Cod <> "" AndAlso cfg.Sa_Cod <> "0" Then
                Stb.AppendLine("        and sa.sa_Cod =  " & Agro_SQL_SaveNum(cfg.Sa_Cod))
            End If
            Stb.AppendLine("          for json path ")
            Stb.AppendLine("      ) as items --centri Aziendali ") ' fine centri Aziendali
            Stb.AppendLine("  from imprese i ")

            If cfg.ApplicaFiltroUtentiVisibilitaAppoggio Then
                Stb.AppendLine("          inner join Utenti_Visibilita_Appoggio t (NOLOCK) ")
                Stb.AppendLine("              on i.piva = t.piva  ")
                Stb.AppendLine("              and t.Entita_Cod = 1 ")
                Stb.AppendLine("              and t.username = @username ")
            End If

            Stb.AppendLine("  where i.piva = '" & Agro_SQL_SaveText(cfg.Piva) & "' ")
            Stb.AppendLine("  for json path ")
            Stb.AppendLine(" ) as items -- utente ")
            Stb.AppendLine(" from " & NomeDB_Utenti & " .dbo.utenti_dettagli ")
            Stb.AppendLine(" where username = @Username ")
            Stb.AppendLine(" for json path ")


            ' VAnni: 31/3/2021: non necessaria se non in sql management studio
            'Stb.AppendLine(")  ")
            'Stb.AppendLine(" select @JSON  ")

            '--------------------------------------------------------------------------
            jSon = EseguiQuery_Lettura_jSon(objParametri_Server, Stb.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return jSon


    End Function

    Private Sub QryDaElencoIcone_OLD(ElencoIconeSpecieVegetali As String, ByRef stb As StringBuilder)

        stb.AppendLine("  ( ")
        stb.AppendLine("     Select Right('00000000' + strName, 7) as vegCodIco ")
        stb.AppendLine("     From dbo.fSplit('1:2:3:9:11:13:15:16:17:21:22:23:25:28:30:31:32:33:34:35:38:39:40:41:43:44:45:46:47:48:49:50:51:52:53:54:56:57:58:59:60:62:63:64:65:66:69:70:71:72:73:74:75:76:77:78:79:80:81:83:84:85:86:87:91:101:102:108:110:116:117:118:120:125:126:127:209:210:235:294:311:347:350:5000123:5000309:5000310:9999999', ':') ")
        stb.AppendLine(" ) VegIco")

    End Sub


    Private Sub QryDaElencoIcone(ElencoIconeSpecieVegetali As String, ByRef stb As StringBuilder)

        ' VAnni: 7/4/2021: occorre gestire diversamente le icone, lo split della funzione "QryDaElencoIcone_OLD" manda in timeout la query

        stb.AppendLine("  ( ")
        stb.AppendLine("     Select '0000001' as vegCodIco ")
        'stb.AppendLine("     From dbo.fSplit('1:2:3:9:11:13:15:16:17:21:22:23:25:28:30:31:32:33:34:35:38:39:40:41:43:44:45:46:47:48:49:50:51:52:53:54:56:57:58:59:60:62:63:64:65:66:69:70:71:72:73:74:75:76:77:78:79:80:81:83:84:85:86:87:91:101:102:108:110:116:117:118:120:125:126:127:209:210:235:294:311:347:350:5000123:5000309:5000310:9999999', ':') ")
        stb.AppendLine(" ) VegIco")

    End Sub

    Private Sub LetturaNodoAnagraficaReale(
        ByRef NumeroDiAliasImpostati As Integer,
        ByVal cfg As AlberoAnagraficaFasterModel.AlberoAnagraficaFasterModelCfg,
        ByRef Stb As StringBuilder)


        Stb.AppendLine("                  select  ")
        JsonDettaglio(NumeroDiAliasImpostati,
                      "  'Anagrafica'",
                      "''",
                      "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconaAnagrafica & "'",
                      QAnagrafica, Stb)


        'campi, se esistenti
        Stb.AppendLine("      , ( ")
        Stb.AppendLine("          select  ")
        JsonDettaglio(NumeroDiAliasImpostati,
                      AgronicaCoreDataProvider.Albero.Albero_Descrizioni_Query.DQ_CampoDet("cc", "Campo_Des", ""),
                      "''",
                      "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconaCampo & "'",
                      QCampo, Stb)

        'Appezzementi ed impianti, legati a campi
        AppezzamentiEdImpianti(NumeroDiAliasImpostati, True, cfg, Stb)

        Stb.AppendLine("         from campi cc")
        Stb.AppendLine("         where cc.piva = sa.piva ")
        Stb.AppendLine("         And cc.sa_cod = sa.sa_Cod")

        Stb.AppendLine("              for json path ")
        Stb.AppendLine("              ) as items" & _suffissoAlias & NumeroDiAliasImpostati & " --campi ") ' fine campi

        'Appezzementi ed impianti, non legati a campi
        AppezzamentiEdImpianti(NumeroDiAliasImpostati, False, cfg, Stb)

        TabellaIdlePerArray(Stb)

        Stb.AppendLine("              for json path ")
    End Sub

    Private Sub AppezzamentiEdImpianti(
        ByRef NumeroDiAliasImpostati As Integer,
        ByVal Campo As Boolean,
        ByVal cfg As AlberoAnagraficaFasterModel.AlberoAnagraficaFasterModelCfg,
        ByRef Stb As StringBuilder
    )


        Stb.AppendLine("              , ( ")
        Stb.AppendLine("                  select  ")
        JsonDettaglio(NumeroDiAliasImpostati,
                      AgronicaCoreDataProvider.Albero.Albero_Descrizioni_Query.DQ_Appezzamento("a"),
                      AgronicaCoreDataProvider.Albero.Albero_Descrizioni_Query.DQ_Style_Appezzamento("a"),
                      "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconaAppezzamento & "'",
                      QAppezzamento, Stb)


        'impianti sotto appezzamento
        Stb.AppendLine("                , ( ")
        Stb.AppendLine("                    select  ")
        JsonDettaglio(-1,
                      AgronicaCoreDataProvider.Albero.Albero_Descrizioni_Query.DQ_Impianto("imp", "Anag1", "DescrizioneSpecieCultivar", "app.App_Nome", "Sup_Imp"),
                      AgronicaCoreDataProvider.Albero.Albero_Descrizioni_Query.DQ_Style_Impianto("app", "imp"),
                      "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconeVegetaliBasePath & "' + case when  VegIco.vegCodIco is null then '9999999.ico' else VegIco.vegCodIco + '.ico' end ",
                      QImpianto, Stb)

        Stb.AppendLine("                    From Reg_Impianti imp ")
        Stb.AppendLine("                    inner join Appezzamento app ")
        Stb.AppendLine("                       on imp.piva = app.piva ")
        Stb.AppendLine("                       And imp.sa_cod = app.sa_cod")
        Stb.AppendLine("                       And imp.appezza = app.appezza")

        Stb.AppendLine("                    left join Reg_Impianti_Codici e1 ")
        Stb.AppendLine("                       on imp.piva = e1.piva ")
        Stb.AppendLine("                       And imp.sa_cod = e1.sa_cod")
        Stb.AppendLine("                       And imp.appezza = e1.appezza")
        Stb.AppendLine("                       And imp.id_Reg = e1.id_reg")

        Stb.AppendLine("                       And e1.Progetto_cod = 0")
        Stb.AppendLine("                       And e1.id_cod between 3000 and 3999")

        LetturaAnagrafiche("imp", "e1", cfg.DatiSportelloSementieri, Stb)

        Stb.Append("Left join ")

        QryDaElencoIcone(cfg.Elenco_Icone_SpecieVegetali, Stb)
        Stb.Append(" on cast(VegIco.vegCodIco as int) = Anag1.veg_cod ")


        Stb.AppendLine("                    Where imp.piva = a.piva ")
        Stb.AppendLine("                    And imp.sa_cod = a.sa_cod")
        Stb.AppendLine("                    And imp.appezza = a.appezza")

        'validità impianti
        Stb.AppendLine(" and imp.Validita_inizio < " & Agro_SQL_SaveDate(cfg.dataFine) & " ")
        Stb.AppendLine(" AND imp.Validita_Fine > " & Agro_SQL_SaveDate(cfg.dataInizio) & " ")

        'If Not String.IsNullOrEmpty(CodiceFiscaleTecnico) Then
        '    Stb.AppendLine(" AND imp.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(CodiceFiscaleTecnico) & "' ")
        'End If

        Stb.AppendLine("                 for json path, INCLUDE_NULL_VALUES --fine impiati") 'fine impianti
        Stb.AppendLine("                 ) as items ")



        'catasto associato ad appezzamento
        If cfg.Flag_CatastoAppezzamento Then
            LetturaNodoParticelleCatastali(NumeroDiAliasImpostati, "Area", "", enum_TipoLetturaParticelle.appezzamentiXparticelle, Stb)
        End If
        'catasto sotto appezzamento

        Stb.AppendLine("                 From appezzamento a ")
        Stb.AppendLine("                 inner join  REg_Impianti ii ")
        Stb.AppendLine("                       on ii.piva = a.piva ")
        Stb.AppendLine("                       And ii.sa_cod = a.sa_cod")
        Stb.AppendLine("                       And ii.appezza = a.appezza")

        Stb.AppendLine("                    left join Reg_Impianti_Codici e2 ")
        Stb.AppendLine("                       on ii.piva = e2.piva ")
        Stb.AppendLine("                       And ii.sa_cod = e2.sa_cod")
        Stb.AppendLine("                       And ii.appezza = e2.appezza")
        Stb.AppendLine("                       And ii.id_Reg = e2.id_reg")

        Stb.AppendLine("                       And e2.Progetto_cod = 0")
        Stb.AppendLine("                       And e2.id_cod between 3000 and 3999")

        LetturaAnagrafiche("ii", "e2", cfg.DatiSportelloSementieri, Stb)

        Stb.AppendLine("                 Where a.piva = sa.piva ")
        Stb.AppendLine("                 And a.sa_cod = sa.sa_cod")

        Dim filtroAggiuntivo As String
        Dim xSementiero As String = ""

        If Not String.IsNullOrEmpty(xSementiero) OrElse cfg.Flag_Appezzamenti_Filtra_Tecnico Then
            xSementiero = " and ii.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(cfg.Codice_Fiscale_Tecnico) & "' "
        End If

        If (cfg.Veg_Cod <> 0) Then
            filtroAggiuntivo = "and Anag1.Veg_Cod =" + CStr(cfg.Veg_Cod) & IIf(xSementiero = "", "", " AND " & xSementiero)
        Else
            filtroAggiuntivo = xSementiero
        End If

        Stb.AppendLine(Agro_SQL_Save_xFiltroAggiuntivo(filtroAggiuntivo, , ))

        If Campo Then
            Stb.AppendLine("                 And a.campo_cod = cc.campo_cod")
        Else
            Stb.AppendLine("                 And a.campo_cod = 0")
        End If

        'validità appezzamento
        Stb.AppendLine(" and a.Validita_inizio < " & Agro_SQL_SaveDate(cfg.dataFine) & " ")
        Stb.AppendLine(" AND a.Validita_Fine > " & Agro_SQL_SaveDate(cfg.dataInizio) & " ")

        Stb.AppendLine("                 for json path, INCLUDE_NULL_VALUES --  ")
        Stb.AppendLine("              ) as items" & _suffissoAlias & NumeroDiAliasImpostati & " -- Appezzamento") 'fine appezzamento


    End Sub

    Private Sub LetturaNodoPlanning(
        ByRef NumeroDiAliasImpostati As Integer,
        ByVal cfg As AlberoAnagraficaFasterModel.AlberoAnagraficaFasterModelCfg,
        ByRef Stb As StringBuilder
     )
        Stb.AppendLine("                  select  ")
        JsonDettaglio(NumeroDiAliasImpostati,
                      "  'Planning'",
                      "''",
                      "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconaPlanning & "'",
                      QPlannnigEtichetta, Stb)
        Stb.AppendLine("              , ( ")
        Stb.AppendLine("                  select  ")
        JsonDettaglio(-1,
                      " '{Plan} : ' + t.programmazione_Des ",
                      "''",
                      "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconaPlanning & "'",
                      QPlannnigTestata, Stb)
        Stb.AppendLine("                      , ( ")
        Stb.AppendLine("                          select  ")
        JsonDettaglio(-1,
                      AgronicaCoreDataProvider.Albero.Albero_Descrizioni_Query.DQ_PlanningEntita("e1", "Anag1", "DescrizioneSpecieCultivar", "e1.Entita_Des", "Superficie"),
                      "''",
                      "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconeVegetaliBasePath & "' + case when  VegIco.vegCodIco is null then '9999999.ico' else VegIco.vegCodIco + '.ico' end ",
                      QPlannnigEntita, Stb)
        Stb.AppendLine("                              , null as items ")
        Stb.AppendLine("                          from programmazione_entita e1 ")

        LetturaAnagrafiche("e1", "e1", cfg.DatiSportelloSementieri, Stb)

        Stb.Append("Left join ")

        QryDaElencoIcone(cfg.Elenco_Icone_SpecieVegetali, Stb)
        Stb.Append(" on cast(VegIco.vegCodIco as int) = Anag1.veg_cod ")

        Stb.AppendLine("                          where e1.piva = sa.piva ") ' filtro sulla piva selezionata.
        Stb.AppendLine("                          and e1.sa_cod = sa.sa_cod ")
        Stb.AppendLine("                          and e1.programmazione_cod = t.programmazione_Cod ")

        'validità righe di planning
        Stb.AppendLine(" and e1.Validita_inizio < " & Agro_SQL_SaveDate(cfg.dataFine) & " ")
        Stb.AppendLine(" AND e1.Validita_Fine > " & Agro_SQL_SaveDate(cfg.dataInizio) & " ")

        'If Not String.IsNullOrEmpty(CodiceFiscaleTecnico) Then
        '    Stb.AppendLine(" AND e1.Codice_Fiscale_Tecnico = '" & Agro_SQL_SaveText(CodiceFiscaleTecnico) & "' ")
        'End If

        Stb.AppendLine("                          for json path, INCLUDE_NULL_VALUES ")
        Stb.AppendLine("                      ) as items -- planning entita ")
        Stb.AppendLine("  ")
        Stb.AppendLine("                  from ( select distinct programmazione_cod, piva, sa_Cod from programmazione_entita ) e  ")
        Stb.AppendLine("                      inner join programmazione_testata t ")
        Stb.AppendLine("                          on e.programmazione_Cod = t.programmazione_cod                           ")
        Stb.AppendLine("                  where t.piva = sa.piva ") ' filtro sulla piva selezionata.
        Stb.AppendLine("                  and e.sa_cod = sa.sa_Cod ")
        Stb.AppendLine("                  for json path ")
        Stb.AppendLine("                  ) as items --Planning Testata  ") 'fine Planning Testata

        TabellaIdlePerArray(Stb)

        Stb.AppendLine("              for json path ")
    End Sub

    Private Shared Sub TabellaIdlePerArray(Stb As StringBuilder)
        'Stb.AppendLine("              from (select top 1 piva from imprese ) a ") 'tabella "farlocca", tanto per avere una riga
        'Stb.AppendLine("              from (select '' as piva ) a ") 'tabella "farlocca", tanto per avere una riga
    End Sub

    Private Sub LetturaNodoCatastoXCentriAziendali(ByRef NumeroDiAliasImpostati As Integer, ByRef Stb As StringBuilder)
        Stb.AppendLine("                  select  ")
        JsonDettaglio(NumeroDiAliasImpostati,
                      "'Catasto Aziendale'",
                      "''",
                      "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconaCatasto & "'",
                      QCatastoAziendale, Stb)

        LetturaNodoParticelleCatastali(-1, "Sup_condotta", "tp", enum_TipoLetturaParticelle.impreseXparticelle, Stb)

        TabellaIdlePerArray(Stb)

        Stb.AppendLine("              for json path ")
    End Sub

    Friend Enum enum_TipoLetturaParticelle
        impreseXparticelle = 1
        appezzamentiXparticelle = 2
    End Enum


    Private Sub LetturaNodoParticelleCatastali(ByRef NumeroAliasImpostato As Integer, aliasColonnaSuperficie As String, aliasConduzione As String, tipoLettura As enum_TipoLetturaParticelle, Stb As StringBuilder)

        Stb.AppendLine("                      ,  ( ")
        Stb.AppendLine("                      select  ")
        JsonDettaglio(NumeroAliasImpostato,
                      AgronicaCoreDataProvider.Albero.Albero_Descrizioni_Query.DQ_Particella("ipp", "it", aliasConduzione, aliasColonnaSuperficie),
                      "''",
                      "'" & AgronicaCoreDataProvider.Albero.Albero_Icone.IconaParticella & "'",
                      QParticella, Stb)

        Dim tba As String = "impreseXparticelle"
        If tipoLettura = enum_TipoLetturaParticelle.appezzamentiXparticelle Then
            tba = "appezzamentiXparticelle"
        End If

        Stb.AppendLine("                        , null as items ")
        Stb.AppendLine("                      from " & tba & " ipp ")
        Stb.AppendLine("                        inner Join ParticelleCatastali p  ")
        Stb.AppendLine("                          On  p.PROV       = ipp.PROV ")
        Stb.AppendLine("                          And p.COM        = ipp.com ")
        Stb.AppendLine("                          And p.SEZIONE    = ipp.SEZIONE ")
        Stb.AppendLine("                          And p.FOGLIO     = ipp.FOGLIO ")
        Stb.AppendLine("                          And p.NUMERO     = ipp.NUMERO ")
        Stb.AppendLine("                          And p.SUBALTERNO = ipp.SUBALTERNO")

        Stb.AppendLine("                      inner Join ISTAT it")
        Stb.AppendLine("                          On  it.PROV = ipp.PROV")
        Stb.AppendLine("                      	  And it.COM = ipp.COM")
        If Not String.IsNullOrEmpty(aliasConduzione) Then

            Stb.AppendLine("                      inner Join(")
            Stb.AppendLine("                      Select  0 as TitoloPossesso, 'altro' as titoloPossessoDES")
            Stb.AppendLine("                      union ")
            Stb.AppendLine("                      Select  1, 'Proprietà'")
            Stb.AppendLine("                      union")
            Stb.AppendLine("                      Select  2, 'Comodato'")
            Stb.AppendLine("                      union")
            Stb.AppendLine("                      Select  3, 'Affitto Con Contratto'")
            Stb.AppendLine("                      union")
            Stb.AppendLine("                      Select  4, 'Affitto Senza Contratto'")
            Stb.AppendLine("                      union")
            Stb.AppendLine("                      Select  5, 'In Conto Terzi'")
            Stb.AppendLine("                      union")
            Stb.AppendLine("                      Select  6, 'In Convenzione'")
            Stb.AppendLine("                      union")
            Stb.AppendLine("                      Select  7, 'In Compartecipazione'")
            Stb.AppendLine("                      ) tp")
            Stb.AppendLine("                      On tp.TitoloPossesso = ipp.TitoloPossesso")

        End If
        'esiste conduzione

        Stb.AppendLine("                      where ipp.piva = sa.piva ") 'filtro sulla piva selezionata
        Stb.AppendLine("                      and ipp.sa_Cod = sa.sa_cod ")

        If tipoLettura = enum_TipoLetturaParticelle.appezzamentiXparticelle Then
            Stb.AppendLine("                      and ipp.appezza = a.appezza ")
        End If

        Stb.AppendLine("                      and ipp.piva = i.piva ")
        Stb.AppendLine("                      for json path, INCLUDE_NULL_VALUES ")
        If NumeroAliasImpostato >= 0 Then
            Stb.AppendLine("                  ) as items" & _suffissoAlias & NumeroAliasImpostato & " --particelle ")
        Else
            Stb.AppendLine("                  ) as items --particelle ")
        End If


    End Sub

    Private Shared Sub LetturaAnagrafiche(tabellaAnagrafica As String, tabellaCodiciAnagrfe As String, DatiSportelloSementieri As String, Stb As StringBuilder)
        Stb.AppendLine("                inner Join ( ")
        Stb.AppendLine("                             select  ")
        Stb.AppendLine("                                gru_Cod ")
        Stb.AppendLine("                              , veg_Cod ")
        Stb.AppendLine("                              , cul_cod ")
        Stb.AppendLine("                              , CodiceAnagrafe ")
        Stb.AppendLine("                              , case when veg_des = '' then DestinazioneUso else veg_des + ' - ' + cul_des end as DescrizioneSpecieCultivar ")
        Stb.AppendLine("                          from ( ")
        Stb.AppendLine("                              select gru.gru_cod, veg.veg_cod, veg_des, cul_cod, cul_des, 0 As CodiceAnagrafe, '' as DestinazioneUso ")
        Stb.AppendLine("                              From SpecieVegetali veg ")

        Dim SementieriSportelloConfigurazione_Cod As Integer = -1
        If Not String.IsNullOrEmpty(DatiSportelloSementieri) Then
            SementieriSportelloConfigurazione_Cod = DatiSportelloSementieri.Split("|")(4)
        End If


        If SementieriSportelloConfigurazione_Cod >= 0 Then
            Stb.AppendLine("            inner Join ( ")
            Stb.AppendLine("                         SELECT distinct    ")
            Stb.AppendLine("                          ms.Veg_Cod ")
            Stb.AppendLine("                          From Sementieri_Sportello_ConfigurazioneXmappatura_specie sscXms  ")
            Stb.AppendLine("                             INNER Join Mappatura_Specie ms ")
            Stb.AppendLine("                                 On sscXms.ID_Specie = ms.ID_Specie  ")
            Stb.AppendLine("                              And  sscXms.ID_SottoSpecie = ms.ID_SottoSpecie  ")
            Stb.AppendLine("                              And sscXms.ID_Gruppo = ms.ID_Gruppo  ")
            Stb.AppendLine("                              And sscXms.ID_Genotipo = ms.ID_Genotipo ")
            Stb.AppendLine("                          WHERE sscXms.Sementieri_Sportello_Configurazione_cod = " & SementieriSportelloConfigurazione_Cod)
            Stb.AppendLine("                          )  jMappaSportello")
            Stb.AppendLine("                          on jMappaSportello.veg_cod = veg.veg_Cod")

        End If

        Stb.AppendLine("                              inner Join Cultivar c ")
        Stb.AppendLine("                                     On c.Veg_Cod = veg.Veg_Cod ")
        Stb.AppendLine("                              Left Join gruppovegetale gru ")
        Stb.AppendLine("                                  On gru.gru_cod = veg.gru_cod ")

        'se specificato uno sportello non devo leggere i codici anagrafe
        If SementieriSportelloConfigurazione_Cod < 0 Then
            Stb.AppendLine("                              union all ")
            Stb.AppendLine("                              Select 0, 0, '', 0, '', codice, descrizione ")
            Stb.AppendLine("                              From Codici_Anagrafe ")
            Stb.AppendLine("                              Where codice between 3000 And 3999 ")
        End If

        Stb.AppendLine("                          ) d1 ")
        Stb.AppendLine("                      ) Anag1 ")
        Stb.AppendLine("                      On (  ")
        Stb.AppendLine("                              ( Anag1.Cul_Cod = " & tabellaAnagrafica & ".Cul_Cod And Anag1.CodiceAnagrafe = 0 ) Or ")
        Stb.AppendLine("                              ( Anag1.cul_cod = 0 And " & tabellaCodiciAnagrfe & ".id_cod = Anag1.CodiceAnagrafe) ")
        Stb.AppendLine("                      ) --condizione su cultivar oppure codice Anagrafe")
    End Sub

    ''' <summary>
    ''' genera una testata
    ''' </summary>
    ''' <param name="nomeParametro"></param>
    ''' <param name="DescrizioneParametro"></param>
    ''' <param name="icona"></param>
    ''' <param name="stb"></param>
    Private Sub JsonTestata(nomeParametro As String, DescrizioneParametro As String, icona As String, ByRef stb As Text.StringBuilder)

        stb.AppendLine(" '{" & nomeParametro & "} : " & DescrizioneParametro & "' as [text]  ")
        stb.AppendLine(" , cast (1 as bit) as expanded ")
        stb.AppendLine(" , '" & icona & "' as imageUrl ")
        stb.AppendLine(" , '' as style ")
        stb.AppendLine(" , '1§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0' as value")
        stb.AppendLine(" , '1§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§0§§0§0' as id")
    End Sub

    ''' <summary>
    ''' genera un dettaglio
    ''' </summary>
    ''' <param name="NumeroAliasImpostato">se si passa una variabile con valore >= 0 questa verrà incrementata di valore. se si passa costante "-1" non accadrà nulla.</param>
    ''' <param name="nomeParametro"></param>
    ''' <param name="DescrizioneParametro"></param>
    ''' <param name="icona"></param>
    ''' <param name="stb"></param>
    Private Sub JsonDettaglio(ByRef NumeroAliasImpostato As Integer, sTextQry As String, sStyleQuery As String, icona As String, sPosizioniChiavi As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave), ByRef stb As Text.StringBuilder)

        Dim aliasCampo As String = ""
        If NumeroAliasImpostato >= 0 Then
            aliasCampo = _suffissoAlias & NumeroAliasImpostato
        End If

        stb.AppendLine(" " & sTextQry & " as [text" & aliasCampo & "]  ")
        stb.AppendLine(" , cast (1 as bit) as expanded" & aliasCampo)
        stb.AppendLine(" , " & icona & " as imageUrl" & aliasCampo)
        stb.Append(" , ")
        stb.AppendLine(sStyleQuery)
        stb.Append(" as style" & aliasCampo)


        stb.Append(" , ")
        If sPosizioniChiavi.Count > 0 Then
            GeneratoreChiave(sPosizioniChiavi, stb)
        Else
            stb.Append("''") 'nessuna chiave richiesta
        End If
        stb.AppendLine(" as value" & aliasCampo)
        '------------

        stb.Append(" , ")
        If sPosizioniChiavi.Count > 0 Then
            GeneratoreChiave(sPosizioniChiavi, stb)
        Else
            stb.AppendLine("''")
        End If
        stb.AppendLine(" as id" & aliasCampo)
        '------------

        If NumeroAliasImpostato >= 0 Then
            NumeroAliasImpostato += 1
        End If

    End Sub

    Private Sub GeneratoreChiave(sQuery As List(Of AgronicaCoreDataProvider.Albero.posizioneChiave), stb As Text.StringBuilder)

        _arr = AgronicaCoreDataProvider.Albero.Albero_Posizioni.EsempioMaschera.Split("§")

        For Each cp In sQuery
            _arr(cp.Posizione) = cp.Query
        Next

        stb.Append("'" & String.Join("§", _arr) & "'")

    End Sub


End Class


'#################################################################
'#################################################################
'#################################################################
