Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
Public Class PDC_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function LeggiDocumenti(ByVal Piva As String,
                                    ByVal TipiDocumento As String,
                                    ByVal DataRiferimento As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.LeggiAuditDocumenti()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.AppendLine(" SELECT Alert_Entita.ID_Alert_Entita, Alert_Entita.PivaSuperUser, Alert_Entita.Piva, Alert_Entita.Cod_Contatto, Alert_Entita.TipoEntita_Cod, ")
            StrSQL.AppendLine("   Allegati_Documenti.Allegati_Documenti_Cod, Allegati_Documenti.Allegati_Documenti_Numero, Allegati_Documenti.Allegati_Documenti_Des, ")
            StrSQL.AppendLine("   Allegati_Documenti.Sottocartella, Allegati_Documenti.Allegati_Documenti_NomeFile, Allegati_Documenti.Allegati_Documenti_CatCod, ")
            StrSQL.AppendLine("   Allegati_Documenti.Allegati_Documenti_Ente_Cod, Allegati_Documenti.Allegati_Documenti_Ente_Des, Allegati_Documenti.Validazione_Data, Allegati_Documenti.Validazione_Flag, ")
            StrSQL.AppendLine("   Alert_Elenco.ID_Elenco, Alert_Elenco.Data_Scadenza, Alert_Elenco.Descrizione_Scadenza, Alert_Elenco.ID_Tipologia, Alert_Tipologia.Nome AS Tipologia, ")
            StrSQL.AppendLine("   Alert_Tipologia.ID_Area, Alert_Area.Nome AS Area ")
            StrSQL.AppendLine(" FROM Alert_Entita ")
            StrSQL.AppendLine(" INNER JOIN Allegati_Documenti ON Alert_Entita.PivaSuperUser = Allegati_Documenti.Allegati_Documenti_SuperUser AND Alert_Entita.Allegati_Documenti_Cod = Allegati_Documenti.Allegati_Documenti_Cod ")
            StrSQL.AppendLine(" INNER JOIN Alert_Elenco ON Alert_Entita.PivaSuperUser = Alert_Elenco.PivaSuperUser AND Alert_Entita.ID_Alert_Entita = Alert_Elenco.ID_Alert_Entita ")
            StrSQL.AppendLine(" LEFT JOIN Alert_Tipologia ON Alert_Elenco.ID_Tipologia = Alert_Tipologia.ID_Tipologia ")
            StrSQL.AppendLine(" LEFT JOIN Alert_Area ON Alert_Tipologia.ID_Area = Alert_Area.ID_Area ")
            StrSQL.AppendLine(" WHERE Alert_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("   AND Alert_Entita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine("   AND Alert_Entita.PivaSuperUser = '" & Trim(objParametri.PivaSuperUser) & "'")

            If Not String.IsNullOrEmpty(Piva) Then
                StrSQL.AppendLine(" AND Alert_Entita.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Not String.IsNullOrEmpty(TipiDocumento) Then
                StrSQL.AppendLine(" AND Alert_Elenco.ID_Tipologia IN (" & Agro_SQL_Save_Clausola_IN(TipiDocumento) & ") ")
            End If

            If DataRiferimento <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND Alert_Elenco.Data_Scadenza >= " & Agro_SQL_SaveDate(DataRiferimento) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Alert_Entita.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Alert_Entita.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select


            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Function Leggi_Tipo_Campione_join(ByVal ID_PDC_Testata As Integer,
                                             ByVal ID_PDC_Dettagli As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi_Tipo_Campione_join()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" select  PDC_Tipo_Campione_Apofruit.codice from cac_codifica_infoaggiuntive ")
            StrSQL.AppendLine(" inner join PDC_Tipo_Campione_Apofruit on PDC_Tipo_Campione_Apofruit.capitolato  = cac_codifica_infoaggiuntive.infoAgg_Cod ")
            StrSQL.AppendLine(" inner join pdc_dettagli on pdc_dettagli.capitolatoprivato = cac_codifica_infoaggiuntive.infoagg_des ")

            StrSQL.AppendLine(" WHERE pdc_dettagli.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata) & " ")

            StrSQL.AppendLine(" AND PDC_Dettagli.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))

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


    Public Function FiltroMagico(ByVal flag_nero As Boolean,
                                 ByVal ID_PDC_Testata As Integer,
                                ByVal DataInizio_PDC As Date,
                                ByVal DataFine_PDC As Date,
                                ByVal DataInizio_Campione As Date,
                                ByVal DataFine_Campione As Date,
                                ByVal DataInizio_Richiesta As Date,
                                ByVal DataFine_Richiesta As Date,
                                ByVal DataInizio_Analisi As Date,
                                ByVal DataFine_Analisi As Date,
                                ByVal Nome_del_PDC As String,
                                ByVal ID_provenienza As Integer,
                                ByVal Veg_Cod_filtro As Integer,
                                ByVal id_tipo_lotta As String,
                                ByVal desc_articolo As String,
                                ByVal flag_campioni As Boolean,
                                ByVal flag_richiesta As Boolean,
                                ByVal flag_analisi As Boolean,
                                ByVal Piva As String,
                                ByVal rag_sociale As String,
                                ByVal SA As Integer,
                                ByVal CapitolatoCliente As Integer,
                                ByVal objParametri As AgronicaCoreParametri
                                ) As List(Of RispostaFiltrone)

        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.FiltroMagico()"

        If SA <> 0 Then
            flag_analisi = True
        End If

        'per velocizzare
        If CapitolatoCliente > -1 Then
            flag_analisi = True
        End If

        If flag_analisi Then
            flag_richiesta = True
            flag_campioni = True
        End If

        If flag_richiesta Then
            flag_campioni = True
        End If

        Dim Risposta As List(Of RispostaFiltrone)

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using db As New Gias_DeveloperServer_Entities(EFConnString)

            Dim impresePadri As New Dictionary(Of String, String)
            Dim gerarchiaImprese = (From g In db.GerarchiaImprese
                                    Join Imprese In db.Imprese On Imprese.PIVA Equals g.Padre
                                    Where g.Padre <> objParametri.PivaSuperUser
                                    Select g.Figlio, g.Padre, Imprese.rag_soc).ToList

            ' ricava le imprese padri dalla gerarchia
            For Each g In gerarchiaImprese
                If impresePadri.ContainsKey(g.Figlio) Then
                    impresePadri(g.Figlio) &= ", " & g.rag_soc
                Else
                    impresePadri(g.Figlio) = g.rag_soc
                End If
            Next

            Risposta = (
                From pdc_testata In db.PDC_Testata
                Join pdc_dettagli In db.PDC_Dettagli
                    On New With {pdc_testata.PivaSuperUser, pdc_testata.Id_PDC_Testata} _
                    Equals New With {pdc_dettagli.PivaSuperUser, .Id_PDC_Testata = pdc_dettagli.ID_PDC_Testata}
                Group Join Imprese In db.Imprese
                    On pdc_dettagli.Piva Equals Imprese.PIVA
                    Into Imprese_join = Group From Imprese In Imprese_join.DefaultIfEmpty()
                Group Join cultivar In db.Cultivar
                        On New With {.Cul_Cod = CInt(pdc_dettagli.Cul_Cod), .Veg_Cod = CInt(pdc_dettagli.Veg_Cod)} _
                        Equals New With {.Cul_Cod = cultivar.Cul_Cod, .Veg_Cod = cultivar.Veg_Cod}
                        Into cultivar_join = Group From cultivar In cultivar_join.DefaultIfEmpty()
                Group Join specievegetali In db.SpecieVegetali
                        On New With {.Veg_Cod = CInt(pdc_dettagli.Veg_Cod)} _
                        Equals New With {.Veg_Cod = specievegetali.Veg_Cod}
                        Into specievegetali_join = Group From specievegetali In specievegetali_join.DefaultIfEmpty()
                Group Join gruppovarietale In db.GruppoVarietale
                        On New With {.Grva_Cod = CInt(pdc_dettagli.Grva_Cod)} _
                        Equals New With {.Grva_Cod = CInt(gruppovarietale.Grva_Cod)}
                    Into gruppovarietale_join = Group From gruppovarietale In gruppovarietale_join.DefaultIfEmpty()
                Group Join PDC_DisciplinareAcquisti In db.PDC_DisciplinareAcquisti
                        On New With {.id_disciplinareacquisti = CStr(pdc_dettagli.Tipo_Lotta_Acquisti)} _
                        Equals New With {.id_disciplinareacquisti = CStr(PDC_DisciplinareAcquisti.ID_DisciplinareAcquisti)}
                        Into PDC_DisciplinareAcquisti_join = Group From PDC_DisciplinareAcquisti In PDC_DisciplinareAcquisti_join.DefaultIfEmpty()
                Group Join PDC_PuntoDiPrelievo In db.PDC_PuntoDiPrelievo
                        On New With {.id_puntodiprelievo = CInt(pdc_dettagli.PuntoPrelievo)} _
                        Equals New With {.id_puntodiprelievo = CInt(PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo)}
                        Into PDC_PuntoDiPrelievo_join = Group From PDC_PuntoDiPrelievo In PDC_PuntoDiPrelievo_join.DefaultIfEmpty()
                Group Join pdc_campioni In db.PDC_Campioni
                        On New With {.ID_PDC_Testata = CInt(pdc_dettagli.ID_PDC_Testata), .ID_PDC_Dettagli = CInt(pdc_dettagli.ID_PDC_Dettagli)} _
                        Equals New With {.ID_PDC_Testata = CInt(pdc_campioni.ID_PDC_Testata), .ID_PDC_Dettagli = CInt(pdc_campioni.ID_PDC_Dettagli)}
                        Into PDC_Campioni_join = Group From pdc_campioni In PDC_Campioni_join.DefaultIfEmpty()
                Group Join pdc_analisi In db.PDC_Analisi
                        On New With {.ID_PDC_Testata = CInt(pdc_campioni.ID_PDC_Testata), .ID_PDC_Dettagli = CInt(pdc_campioni.ID_PDC_Dettagli), .ID_PDC_Campione = CInt(pdc_campioni.ID_PDC_Campione)} _
                        Equals New With {.ID_PDC_Testata = CInt(pdc_analisi.ID_PDC_Testata), .ID_PDC_Dettagli = CInt(pdc_analisi.ID_PDC_Dettagli), .ID_PDC_Campione = CInt(pdc_analisi.ID_PDC_Campione)}
                        Into PDC_Analisi_join = Group From pdc_analisi In PDC_Analisi_join.DefaultIfEmpty()
                Group Join risorse_umane In db.Risorse_Umane
                        On New With {.cod_risum = CInt(pdc_analisi.Cod_Risum)} _
                        Equals New With {.cod_risum = CInt(risorse_umane.Cod_RisUm)}
                        Into risorse_umane_join = Group From risorse_umane In risorse_umane_join.DefaultIfEmpty()
                Group Join contatti In db.Contatti
                        On New With {.cod_contatto = CStr(risorse_umane.Cod_Contatto)} _
                        Equals New With {.cod_contatto = CStr(contatti.Cod_Contatto)}
                        Into contatti_join = Group From contatti In contatti_join.DefaultIfEmpty()
                Group Join analisi_tipologia In db.Analisi_Tipologia
                        On New With {.Analisi_Tipologia_Cod = CInt(pdc_analisi.Analisi_Tipologia_Cod)} _
                        Equals New With {.Analisi_Tipologia_Cod = CInt(analisi_tipologia.Analisi_Tipologia_Cod)}
                        Into analisi_tipologia_join = Group From analisi_tipologia In analisi_tipologia_join.DefaultIfEmpty()
                Group Join analisi_testata In db.Analisi_Testata
                        On New With {.Analisi_Testata_Cod = CInt(pdc_analisi.Analisi_Testata_Cod)} _
                        Equals New With {.Analisi_Testata_Cod = CInt(analisi_testata.Analisi_Testata_Cod)}
                        Into analisi_testata_join = Group From analisi_testata In analisi_testata_join.DefaultIfEmpty()
                Group Join analisi_dettagli In db.Analisi_Dettagli
                        On New With {.Analisi_Testata_Cod = CInt(pdc_analisi.Analisi_Testata_Cod)} _
                        Equals New With {.Analisi_Testata_Cod = CInt(analisi_dettagli.Analisi_Testata_Cod)}
                        Into analisi_dettagli_join = Group From analisi_dettagli In analisi_dettagli_join.DefaultIfEmpty()
                Where
                    (ID_PDC_Testata = 0 OrElse (ID_PDC_Testata <> 0 AndAlso pdc_testata.Id_PDC_Testata = ID_PDC_Testata)) _
                AndAlso
                    (pdc_testata.PDC_Testata_Des.Contains(Nome_del_PDC)) _
                AndAlso
                    (id_tipo_lotta = -1 OrElse (id_tipo_lotta = pdc_dettagli.Tipo_Lotta_Acquisti)) _
                AndAlso
                    (pdc_testata.PDC_Data_Istantanea <= DataFine_PDC AndAlso pdc_testata.PDC_Data_Istantanea >= DataInizio_PDC) _
                AndAlso
                    ((pdc_campioni.Data_Campionamento <= DataFine_Campione AndAlso pdc_campioni.Data_Campionamento >= DataInizio_Campione) OrElse pdc_campioni.Data_Campionamento Is Nothing) _
                AndAlso
                    ((pdc_analisi.Data_Richiesta_Analisi <= DataFine_Richiesta AndAlso pdc_analisi.Data_Richiesta_Analisi >= DataInizio_Richiesta) OrElse pdc_analisi.Data_Richiesta_Analisi Is Nothing) _
                AndAlso
                    ((analisi_testata.Analisi_Testata_Data_Fine <= DataFine_Analisi AndAlso analisi_testata.Analisi_Testata_Data_Fine >= DataInizio_Analisi) OrElse analisi_testata.Analisi_Testata_Data_Fine Is Nothing) _
                AndAlso
                    (ID_provenienza = 0 OrElse (ID_provenienza = 1 AndAlso (pdc_testata.Da_Campagna = 0 OrElse pdc_testata.Da_Campagna Is Nothing)) OrElse (ID_provenienza = 2 AndAlso pdc_testata.Da_Campagna = 1) OrElse (ID_provenienza = 3 AndAlso pdc_testata.Da_Campagna = 2)) _
                AndAlso
                    (desc_articolo = "" OrElse (pdc_dettagli.NomeArticolo.Contains(desc_articolo))) _
                AndAlso
                    (Veg_Cod_filtro = 0 OrElse (Veg_Cod_filtro <> 0 And pdc_dettagli.Veg_Cod = Veg_Cod_filtro)) _
                AndAlso
                    (Not flag_campioni OrElse (flag_campioni AndAlso Not flag_richiesta AndAlso pdc_dettagli.Flag_Pdc = -1 AndAlso pdc_analisi.PDC_Stato_Analisi Is Nothing) OrElse flag_richiesta) _
                AndAlso
                    (Not flag_richiesta OrElse (flag_richiesta AndAlso Not flag_analisi AndAlso pdc_analisi.PDC_Stato_Analisi = 1) OrElse
                                               (flag_richiesta AndAlso flag_analisi AndAlso pdc_analisi.PDC_Stato_Analisi = 2)) _
                AndAlso
                    (Piva.Length = 0 OrElse (Piva.Length <> 0 AndAlso pdc_dettagli.Piva.Contains(Piva))) _
                AndAlso
                    (rag_sociale.Length = 0 OrElse (rag_sociale.Length <> 0 AndAlso pdc_dettagli.Rag_Soc.Contains(rag_sociale))) _
                AndAlso
                    (SA = 0 OrElse (SA <> 0 AndAlso analisi_dettagli.Analisi_Parametro_Cod = SA)) _
                AndAlso
                   (Not flag_nero OrElse (flag_nero AndAlso pdc_analisi.Mostra_in_Stampe = -1))
                Select New RispostaFiltrone With {
                  .Nome_Campionamento = pdc_testata.PDC_Testata_Des,
                  .Data_Approssimativa = pdc_testata.PDC_Data_Istantanea,
                  .Provenienza = If(CLng(pdc_testata.Da_Campagna) = 1, "Acquisto", If(pdc_testata.Da_Campagna Is Nothing, "Campagna", Nothing)),
                  .Descrizione_Acquisto = pdc_dettagli.PDC_Dettagli_Des,
                  .Codice_Fornitore = pdc_dettagli.CodiceFornitore,
                  .Piva = pdc_dettagli.Piva,
                  .PivaReale = If(String.IsNullOrEmpty(Imprese.partitaIvaReale), pdc_dettagli.Piva, Imprese.partitaIvaReale),
                  .Ragione_Sociale = pdc_dettagli.Rag_Soc,
                  .Rag_Soc_Padre = "",
                  .Centro = pdc_dettagli.Sa_Nome,
                  .Appezzamento = pdc_dettagli.App_Nome,
                  .Superficie = pdc_dettagli.Sup_Imp,
                  .Note_Impianto = If(pdc_dettagli.note_impianto Is Nothing, "", pdc_dettagli.note_impianto),
                  .Specie_Vegetale = specievegetali.Veg_Des,
                  .Varietà = cultivar.Cul_Des,
                  .Tipologia_Varietale = If(gruppovarietale.Grva_Des Is Nothing, "", gruppovarietale.Grva_Des),
                  .Campionato = If(pdc_dettagli.Flag_Pdc = (-1), "Si", If(CLng(pdc_dettagli.Flag_Pdc) = 0, "No", If(pdc_dettagli.Flag_Pdc Is Nothing, "No", Nothing))),
                  .Capitolato_Privato = pdc_dettagli.CapitolatoPrivato,
                  .Certificato = pdc_dettagli.Certificato,
                  .Data_Semina = pdc_dettagli.Data_Semina,
                  .Data_Raccolta = pdc_dettagli.Data_Raccolta,
                  .Data_Fornitura = pdc_dettagli.Data_Fornitura,
                  .Codice_Articolo = pdc_dettagli.CodiceArticolo,
                  .Lotto_Fornitore = pdc_dettagli.LottoFornitore,
                  .Tipologia_Lotta = CStr(PDC_DisciplinareAcquisti.Descrizione_Disciplinare),
                  .Nome_Articolo = pdc_dettagli.NomeArticolo,
                  .Punto_di_Prelievo = PDC_PuntoDiPrelievo.Descrizione_PuntoDiPrelievo,
                  .Descrizione = pdc_dettagli.Descrizione,
                  .Documentazione = pdc_dettagli.Documentazione,
                  .Note_Documentazione = pdc_dettagli.Note_Documentazione,
                  .Disciplinare = pdc_dettagli.Disciplinare_Cod,
                  .Regolamento = pdc_dettagli.Regolamento,
                  .Data_campionamento = pdc_campioni.Data_Campionamento,
                  .Codice_Campione = pdc_campioni.Codice_Campione,
                  .Note_Campione = pdc_campioni.Note_Campione,
                  .Laboratorio = contatti.Rag_Soc,
                  .CodiceGriglia = analisi_tipologia.Analisi_Tipologia_Des,
                  .Data_Richiesta = pdc_analisi.Data_Richiesta_Analisi,
                  .Data_Fine_Analisi = analisi_testata.Analisi_Testata_Data_Fine,
                  .altreMolecole = pdc_analisi.Altre_Molecole,
                  .ID_PDC_Testata = pdc_testata.Id_PDC_Testata,
                  .ID_PDC_Dettagli = pdc_dettagli.ID_PDC_Dettagli,
                  .ID_PDC_Campione = pdc_campioni.ID_PDC_Campione,
                  .PDC_Stato_Analisi = pdc_analisi.PDC_Stato_Analisi,
                  .Analisi_Tipologia_Tipo = pdc_analisi.Analisi_Tipologia_Tipo,
                  .Analisi_Testata_Cod = pdc_analisi.Analisi_Testata_Cod,
                  .Analisi_Testata_Des = analisi_testata.Analisi_Testata_Des}).Distinct.ToList

            ' recupera le imprese padri associate
            For Each r As RispostaFiltrone In Risposta
                r.Rag_Soc_Padre = If(impresePadri.ContainsKey(r.Piva), impresePadri(r.Piva), "")
            Next

            'Grilli: non sono stato capace di farlo direttamente in select... pardon...
            'For Each r As RispostaFiltrone In Risposta
            '    r.Rag_Soc_Padre = String.Join(", ", (From gerarchia_imprese In db.GerarchiaImprese
            '                                         Join imprese In db.Imprese
            '                                               On imprese.PIVA Equals gerarchia_imprese.Padre
            '                                         Where r.Piva = gerarchia_imprese.Figlio AndAlso gerarchia_imprese.Padre <> objParametri.PivaSuperUser
            '                                         Select imprese.rag_soc))
            'Next

        End Using

        Return Risposta

    End Function

    Public Function FiltroPDC(ByVal flag_nero As Boolean,
                                ByVal ID_PDC_Testata As Integer,
                                ByVal DataInizio_PDC As Date,
                                ByVal DataFine_PDC As Date,
                                ByVal DataInizio_Campione As Date,
                                ByVal DataFine_Campione As Date,
                                ByVal DataInizio_Richiesta As Date,
                                ByVal DataFine_Richiesta As Date,
                                ByVal DataInizio_Analisi As Date,
                                ByVal DataFine_Analisi As Date,
                                ByVal Nome_del_PDC As String,
                                ByVal ID_provenienza As Integer,
                                ByVal Veg_Cod_filtro As Integer,
                                ByVal id_tipo_lotta As String,
                                ByVal desc_articolo As String,
                                ByVal flag_campioni As Boolean,
                                ByVal flag_richiesta As Boolean,
                                ByVal flag_analisi As Boolean,
                                ByVal Piva As String,
                                ByVal rag_sociale As String,
                                ByVal SA As Integer,
                                ByVal CapitolatoCliente As Integer,
                                ByVal objParametri As AgronicaCoreParametri
                                ) As List(Of RispostaFiltrone)

        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.FiltroPDC()"
        Dim MessaggioErrore As String = ""

        If SA <> 0 Then
            flag_analisi = True
        End If

        'per velocizzare
        If CapitolatoCliente > -1 Then
            flag_analisi = True
        End If

        If flag_analisi Then
            flag_richiesta = True
            flag_campioni = True
        End If

        If flag_richiesta Then
            flag_campioni = True
        End If

        Dim Risposta As New List(Of RispostaFiltrone)

        Try

            Dim impresePadri = LeggiImpresePadri(objParametri)

            Dim StrSQL As New Text.StringBuilder("
                SELECT DISTINCT
                    PDC_Testata.Da_Campagna,
                    PDC_Testata.Id_PDC_Testata,
                    PDC_Testata.PDC_Testata_Des,
                    PDC_Testata.PDC_Data_Istantanea,
                    PDC_Dettagli.PDC_Dettagli_Des,
                    PDC_Dettagli.Piva,
                    CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN PDC_Dettagli.PIVA ELSE Imprese.partitaIvaReale END PivaReale,
                    PDC_Dettagli.CodiceFornitore,
                    PDC_Dettagli.Rag_Soc,
                    PDC_Dettagli.Sa_Nome,
                    PDC_Dettagli.App_Nome,
                    PDC_Dettagli.Sup_Imp,
                    PDC_Dettagli.CapitolatoPrivato,
                    PDC_Dettagli.Certificato,
                    PDC_Dettagli.Data_Semina,
                    PDC_Dettagli.Data_Raccolta,
                    PDC_Dettagli.Data_Fornitura,
                    PDC_Dettagli.CodiceArticolo,
                    PDC_Dettagli.LottoFornitore,
                    PDC_Dettagli.NomeArticolo,
                    PDC_Dettagli.Descrizione,
                    PDC_Dettagli.Disciplinare_Cod,
                    PDC_Dettagli.Documentazione,
                    PDC_Dettagli.Note_Documentazione,
                    PDC_Dettagli.Regolamento,
                    PDC_Dettagli.note_impianto,
                    PDC_Dettagli.Flag_Pdc,
                    PDC_Dettagli.ID_PDC_Dettagli,
                    Cultivar.Cul_Des,
                    SpecieVegetali.Veg_Des,
                    GruppoVarietale.Grva_Des,
                    PDC_DisciplinareAcquisti.Descrizione_Disciplinare,
                    PDC_PuntoDiPrelievo.Descrizione_PuntoDiPrelievo,
                    PDC_Campioni.Data_Campionamento,
                    PDC_Campioni.ID_PDC_Campione,
                    PDC_Campioni.Codice_Campione,
                    PDC_Campioni.Note_Campione,
                    PDC_Analisi.Analisi_Testata_Cod,
                    PDC_Analisi.PDC_Stato_Analisi,
                    PDC_Analisi.Data_Richiesta_Analisi,
                    PDC_Analisi.Altre_Molecole,
                    PDC_Analisi.Analisi_Tipologia_Tipo,
                    Contatti.Rag_Soc AS Rag_Soc_Lab,
                    Analisi_Tipologia.Analisi_Tipologia_Des,
                    Analisi_Testata.Analisi_Testata_Des,
                    Analisi_Testata.Analisi_Testata_Data_Fine
                FROM 
                    PDC_Testata
                    INNER JOIN PDC_Dettagli
                        ON PDC_Testata.PivaSuperUser = PDC_Dettagli.PivaSuperUser 
                        AND PDC_Testata.Id_PDC_Testata = PDC_Dettagli.ID_PDC_Testata
                    LEFT OUTER JOIN Imprese
                        ON PDC_Dettagli.Piva = Imprese.Piva
                    LEFT OUTER JOIN Cultivar
                        ON PDC_Dettagli.Cul_Cod = Cultivar.Cul_Cod 
                        AND PDC_Dettagli.Veg_Cod = Cultivar.Veg_Cod
                    LEFT OUTER JOIN SpecieVegetali
                        ON PDC_Dettagli.Veg_Cod = SpecieVegetali.Veg_Cod
                    LEFT OUTER JOIN GruppoVarietale
                        ON PDC_Dettagli.Grva_Cod = GruppoVarietale.Grva_Cod
                    LEFT OUTER JOIN PDC_DisciplinareAcquisti
                        ON PDC_Dettagli.Tipo_Lotta_Acquisti = CAST(PDC_DisciplinareAcquisti.ID_DisciplinareAcquisti AS NVARCHAR)
                    LEFT OUTER JOIN PDC_PuntoDiPrelievo
                        ON PDC_Dettagli.PuntoPrelievo = CAST(PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo AS NVARCHAR)
                    LEFT OUTER JOIN PDC_Campioni
                        ON PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata 
                        AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli
                    LEFT OUTER JOIN PDC_Analisi
                        ON PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata 
                        AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli 
                        AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione
                    LEFT OUTER JOIN Risorse_Umane 
                        ON PDC_Analisi.Cod_Risum = Risorse_Umane.Cod_RisUm
                    LEFT OUTER JOIN Contatti
                        ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto
                    LEFT OUTER JOIN Analisi_Tipologia
                        ON PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod
                    LEFT OUTER JOIN Analisi_Testata
                        ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod

            ")

            If SA <> 0 Then
                StrSQL.AppendLine(" LEFT OUTER JOIN Analisi_Dettagli ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Dettagli.Analisi_Testata_Cod ")
            End If

            StrSQL.AppendLine(" WHERE pdc_testata.Da_Zoo = 0 ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND pdc_testata.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If Not String.IsNullOrEmpty(Nome_del_PDC) Then
                StrSQL.AppendLine(" AND pdc_testata.PDC_Testata_Des LIKE '%" & Agro_SQL_SaveText(Nome_del_PDC) & "%' ")
            End If

            If id_tipo_lotta <> "-1" Then
                StrSQL.AppendLine(" AND pdc_dettagli.Tipo_Lotta_Acquisti = '" & Agro_SQL_SaveText(id_tipo_lotta) & "' ")
            End If

            StrSQL.AppendLine(" AND (pdc_testata.PDC_Data_Istantanea <= " & Agro_SQL_SaveDate(DataFine_PDC) & " AND pdc_testata.PDC_Data_Istantanea >= " & Agro_SQL_SaveDate(DataInizio_PDC) & ") ")
            StrSQL.AppendLine(" AND (pdc_campioni.Data_Campionamento IS NULL OR (pdc_campioni.Data_Campionamento <= " & Agro_SQL_SaveDate(DataFine_Campione) & " AND pdc_campioni.Data_Campionamento >= " & Agro_SQL_SaveDate(DataInizio_Campione) & ")) ")
            StrSQL.AppendLine(" AND (pdc_analisi.Data_Richiesta_Analisi IS NULL OR (pdc_analisi.Data_Richiesta_Analisi <= " & Agro_SQL_SaveDate(DataFine_Richiesta) & " AND pdc_analisi.Data_Richiesta_Analisi >= " & Agro_SQL_SaveDate(DataInizio_Richiesta) & ")) ")
            StrSQL.AppendLine(" AND (analisi_testata.Analisi_Testata_Data_Fine IS NULL OR (analisi_testata.Analisi_Testata_Data_Fine <= " & Agro_SQL_SaveDate(DataFine_Analisi) & " AND analisi_testata.Analisi_Testata_Data_Fine >= " & Agro_SQL_SaveDate(DataInizio_Analisi) & ")) ")

            If ID_provenienza <> 0 Then
                If ID_provenienza = 1 Then
                    StrSQL.AppendLine(" AND (pdc_testata.Da_Campagna IS NULL OR pdc_testata.Da_Campagna = 0) ")
                ElseIf ID_provenienza = 2 Then
                    StrSQL.AppendLine(" AND pdc_testata.Da_Campagna = 1 ")
                ElseIf ID_provenienza = 3 Then
                    StrSQL.AppendLine(" AND pdc_testata.Da_Campagna = 2 ")
                End If
            End If

            If Not String.IsNullOrEmpty(desc_articolo) Then
                StrSQL.AppendLine(" AND pdc_dettagli.NomeArticolo LIKE '%" & Agro_SQL_SaveText(desc_articolo) & "%' ")
            End If

            If Veg_Cod_filtro <> 0 Then
                StrSQL.AppendLine(" AND pdc_dettagli.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod_filtro))
            End If

            If flag_campioni AndAlso Not flag_richiesta Then
                StrSQL.AppendLine(" AND pdc_dettagli.Flag_Pdc = -1 AND pdc_analisi.PDC_Stato_Analisi IS NULL ")
            End If

            If flag_richiesta AndAlso Not flag_analisi Then
                StrSQL.AppendLine(" AND pdc_analisi.PDC_Stato_Analisi = 1 ")
            End If

            If flag_richiesta AndAlso flag_analisi Then
                StrSQL.AppendLine(" AND pdc_analisi.PDC_Stato_Analisi = 2 ")
            End If

            If Not String.IsNullOrEmpty(Piva) Then
                StrSQL.AppendLine(" AND pdc_dettagli.Piva LIKE '%" & Agro_SQL_SaveText(Piva) & "%' ")
            End If

            If Not String.IsNullOrEmpty(rag_sociale) Then
                StrSQL.AppendLine(" AND pdc_dettagli.Rag_Soc LIKE '%" & Agro_SQL_SaveText(rag_sociale) & "%' ")
            End If

            If SA <> 0 Then
                StrSQL.AppendLine(" AND analisi_dettagli.Analisi_Parametro_Cod = " & Agro_SQL_SaveNum(SA))
            End If

            If flag_nero Then
                StrSQL.AppendLine(" AND pdc_analisi.Mostra_in_Stampe = -1 ")
            End If

            '--------------------------------------------------------------------------
            Dim DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            Risposta = DT.Rows.Cast(Of DataRow)().Select(Function(row) DataRowToRispostaFiltrone(row, impresePadri)).ToList()

        Catch ex As Exception

            MessaggioErrore &= ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return Risposta

    End Function

    Public Function DataRowToRispostaFiltrone(r As DataRow, impresepadri As Dictionary(Of String, String)) As RispostaFiltrone
        Return New RispostaFiltrone With {
            .Nome_Campionamento = r("PDC_Testata_Des"),
            .Data_Approssimativa = r("PDC_Data_Istantanea"),
            .Provenienza = If(IsDBNull(r("Da_Campagna")) OrElse r("Da_Campagna") = 0, "Campagna", If(r("Da_Campagna") = 1, "Acquisto", "Altro")),
            .Descrizione_Acquisto = r("PDC_Dettagli_Des"),
            .Codice_Fornitore = If(IsDBNull(r("CodiceFornitore")), "", r("CodiceFornitore")),
            .Piva = If(IsDBNull(r("Piva")), "", r("Piva")),
            .PivaReale = If(IsDBNull(r("PivaReale")), "", r("PivaReale")),
            .Ragione_Sociale = If(IsDBNull(r("Rag_Soc")), "", r("Rag_Soc")),
            .Rag_Soc_Padre = If(impresepadri.ContainsKey(r("Piva")), impresepadri(r("Piva")), ""),
            .Centro = If(IsDBNull(r("Sa_Nome")), "", r("Sa_Nome")),
            .Appezzamento = If(IsDBNull(r("App_Nome")), "", r("App_Nome")),
            .Superficie = If(IsDBNull(r("Sup_Imp")), Nothing, r("Sup_Imp")),
            .Note_Impianto = If(IsDBNull(r("note_impianto")), "", r("note_impianto")),
            .Specie_Vegetale = If(IsDBNull(r("Veg_Des")), "", r("Veg_Des")),
            .Varietà = If(IsDBNull(r("Cul_Des")), "", r("Cul_Des")),
            .Tipologia_Varietale = If(IsDBNull(r("Grva_Des")), "", r("Grva_Des")),
            .Campionato = If(r("Flag_Pdc") = -1, "Si", "No"),
            .Capitolato_Privato = If(IsDBNull(r("CapitolatoPrivato")), "", r("CapitolatoPrivato")),
            .Certificato = If(IsDBNull(r("Certificato")), "", r("Certificato")),
            .Data_Semina = If(IsDBNull(r("Data_Semina")), Nothing, r("Data_Semina")),
            .Data_Raccolta = If(IsDBNull(r("Data_Raccolta")), Nothing, r("Data_Raccolta")),
            .Data_Fornitura = If(IsDBNull(r("Data_Fornitura")), Nothing, r("Data_Fornitura")),
            .Codice_Articolo = If(IsDBNull(r("CodiceArticolo")), "", r("CodiceArticolo")),
            .Lotto_Fornitore = If(IsDBNull(r("LottoFornitore")), "", r("LottoFornitore")),
            .Tipologia_Lotta = If(IsDBNull(r("Descrizione_Disciplinare")), "", r("Descrizione_Disciplinare")),
            .Nome_Articolo = If(IsDBNull(r("NomeArticolo")), "", r("NomeArticolo")),
            .Punto_di_Prelievo = If(IsDBNull(r("Descrizione_PuntoDiPrelievo")), "", r("Descrizione_PuntoDiPrelievo")),
            .Descrizione = If(IsDBNull(r("Descrizione")), "", r("Descrizione")),
            .Documentazione = If(IsDBNull(r("Documentazione")), "", r("Documentazione")),
            .Note_Documentazione = If(IsDBNull(r("Note_Documentazione")), "", r("Note_Documentazione")),
            .Disciplinare = If(IsDBNull(r("Disciplinare_Cod")), "", r("Disciplinare_Cod")),
            .Regolamento = If(IsDBNull(r("Regolamento")), "", r("Regolamento")),
            .Data_campionamento = If(IsDBNull(r("Data_Campionamento")), Nothing, r("Data_Campionamento")),
            .Codice_Campione = If(IsDBNull(r("Codice_Campione")), "", r("Codice_Campione")),
            .Note_Campione = If(IsDBNull(r("Note_Campione")), "", r("Note_Campione")),
            .Laboratorio = If(IsDBNull(r("Rag_Soc_Lab")), "", r("Rag_Soc_Lab")),
            .CodiceGriglia = If(IsDBNull(r("Analisi_Tipologia_Des")), "", r("Analisi_Tipologia_Des")),
            .Data_Richiesta = If(IsDBNull(r("Data_Richiesta_Analisi")), Nothing, r("Data_Richiesta_Analisi")),
            .Data_Fine_Analisi = If(IsDBNull(r("Analisi_Testata_Data_Fine")), Nothing, r("Analisi_Testata_Data_Fine")),
            .altreMolecole = If(IsDBNull(r("Altre_Molecole")), "", r("Altre_Molecole")),
            .ID_PDC_Testata = r("Id_PDC_Testata"),
            .ID_PDC_Dettagli = r("ID_PDC_Dettagli"),
            .ID_PDC_Campione = If(IsDBNull(r("ID_PDC_Campione")), "", r("ID_PDC_Campione")),
            .PDC_Stato_Analisi = If(IsDBNull(r("PDC_Stato_Analisi")), Nothing, r("PDC_Stato_Analisi")),
            .Analisi_Tipologia_Tipo = If(IsDBNull(r("Analisi_Tipologia_Tipo")), Nothing, r("Analisi_Tipologia_Tipo")),
            .Analisi_Testata_Cod = If(IsDBNull(r("Analisi_Testata_Cod")), Nothing, r("Analisi_Testata_Cod")),
            .Analisi_Testata_Des = If(IsDBNull(r("Analisi_Testata_Des")), "", r("Analisi_Testata_Des"))
        }
    End Function


    Public Function LeggiImpresePadri(ByRef objParametri As AgronicaCoreParametri) As Dictionary(Of String, String)

        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.LeggiImpresePadri()"

        Dim impresePadri As New Dictionary(Of String, String)

        Try

            Dim StrSQL As New System.Text.StringBuilder
            StrSQL.AppendLine(" SELECT GerarchiaImprese.Figlio, GerarchiaImprese.Padre, Imprese.Rag_Soc AS RagSoc_Padre ")
            StrSQL.AppendLine(" FROM  GerarchiaImprese with(nolock) ")
            StrSQL.AppendLine(" INNER JOIN Imprese with(nolock) ON GerarchiaImprese.Padre = Imprese.PIVA ")
            StrSQL.AppendLine(" WHERE GerarchiaImprese.Padre <> '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            '--------------------------------------------------------------------------
            Dim DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            ' ricava le imprese padri dalla gerarchia
            For Each r In DT.Rows
                If impresePadri.ContainsKey(r("Figlio")) Then
                    impresePadri(r("Figlio")) &= ", " & r("RagSoc_Padre")
                Else
                    impresePadri(r("Figlio")) = r("RagSoc_Padre")
                End If
            Next

        Catch ex As Exception
            Dim MessaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return impresePadri

    End Function

    Public Function Leggi_solo_dettagli(ByVal ID_PDC_Testata As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT *  ")

            StrSQL.AppendLine(" FROM PDC_Dettagli ")

            StrSQL.AppendLine(" WHERE PDC_Dettagli.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else

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

    Public Function Leggi_dettagli_Campioni_Analisi(ByVal ID_PDC_Testata As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT  ")
            StrSQL.AppendLine(" 	PDC_Dettagli.* ")
            StrSQL.AppendLine(" 	,PDC_Campioni.ID_PDC_Campione, PDC_Campioni.Codice_Campione, PDC_Campioni.ID_PDC_Stato_Campione ")
            StrSQL.AppendLine(" 	, PDC_Analisi.Analisi_Testata_Cod, PDC_Analisi.PDC_Stato_Analisi ")
            StrSQL.AppendLine(" FROM PDC_Dettagli ")
            StrSQL.AppendLine(" LEFT JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser  ")
            StrSQL.AppendLine(" 					AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata ")
            StrSQL.AppendLine(" 					AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli ")
            StrSQL.AppendLine(" LEFT JOIN PDC_Analisi ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser  ")
            StrSQL.AppendLine(" 					AND PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata ")
            StrSQL.AppendLine(" 					AND PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli ")
            StrSQL.AppendLine(" 					AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione ")

            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else

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

    Public Function Distinct_Fornitore_2(ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Distinct_Fornitore_2()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine("select  distinct Fornitore_2  ")

            StrSQL.AppendLine(" FROM PDC_Dettagli ")

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


    Public Function Leggi_CodiceArticolo(ByVal CodiceArticolo As Integer,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi_CodiceArticolo()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT *  ")

            StrSQL.AppendLine(" FROM PDC_Dettagli ")

            StrSQL.AppendLine(" WHERE CodiceArticolo = '" & Agro_SQL_SaveText(CodiceArticolo) & "' ")

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



    Public Function Leggi(ByVal ID_PDC_Testata As Integer,
                          ByVal ID_PDC_Dettagli As Integer,
                          ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal Id_Reg As Integer,
                          ByVal Veg_Cod As Integer,
                          ByVal Cul_Cod As Integer,
                          ByVal Id_LFO As Integer,
                          ByVal Flag_PDC As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT PDC_Dettagli.ID_PDC_Testata , PDC_Dettagli.ID_PDC_Dettagli, PDC_Dettagli.PDC_Dettagli_Des , ")
            StrSQL.AppendLine("        PDC_Dettagli.Piva,  ISNULL(PDC_Dettagli.Sa_Cod,0) AS Sa_Cod ,  ISNULL(PDC_Dettagli.Appezza, 0) AS Appezza ,  ISNULL(PDC_Dettagli.Id_Reg,0) AS ID_Reg ,PDC_Dettagli.Veg_Cod, ")
            StrSQL.AppendLine("        PDC_Dettagli.Cul_Cod,PDC_Dettagli.ID_LFO,PDC_Dettagli.Flag_Pdc,ISNULL (PDC_Dettagli.note_impianto, '') AS note_impianto, ")
            StrSQL.AppendLine("        ISNULL(LFO_Des,'') AS LFO_Des ,  ISNULL(PDC_Dettagli.rag_soc,'') AS Rag_Soc,  ISNULL(PDC_Dettagli.Sa_Nome,'') AS Sa_Nome, ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.App_Nome,'') AS App_Nome, ISNULL(PDC_Dettagli.Sup_Imp,'') AS Sup_Imp, ISNULL(Cul_Des,'') AS Cul_Des, ISNULL(Veg_Des,'') AS Veg_Des,  ")

            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.note2,'') AS note2, ")

            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Disciplinare_cod,'') AS Disciplinare_cod, ")
            'data_Raccolta
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Data_Raccolta,'') AS Data_Raccolta, ")



            'data Semina
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Data_Semina, '') AS Data_Semina, ")
            'Codice
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.CodiceFornitore, '') AS CodiceFornitore ,  ")

            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.CapitolatoPrivato, '') AS CapitolatoPrivato,    ")


            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Grva_Cod,'') AS Grva_Cod,    ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Data_Fornitura,'') AS Data_Fornitura,    ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.CodiceArticolo,'') AS CodiceArticolo,    ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Descrizione,'') AS Descrizione,    ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.LottoFornitore,'') AS LottoFornitore,    ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Mat_Cod,'') AS Mat_Cod,    ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.NomeArticolo,'') AS NomeArticolo,    ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Regolamento,'') AS Regolamento,    ")


            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.tipolotta,'') AS tipolotta,    ")

            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Certificato,'') AS Certificato,   ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Fornitore_2,'') AS Fornitore_2,   ")


            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Codice_Cliente_2,'') AS Codice_Cliente_2,ISNULL(PDC_Dettagli.Sigla,'') AS Sigla,ISNULL(PDC_Dettagli.Data_Inizio_Impianto,'') AS Data_Inizio_Impianto,ISNULL(PDC_Dettagli.Magazzino_di_conferimento,'') AS Magazzino_di_conferimento, ")
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Tecnico_di_riferimento,'') AS Tecnico_di_riferimento, ")
            ''Dettaglio_Specie_Personalizzato
            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Dettaglio_Specie_Personalizzato,'') AS Dettaglio_Specie_Personalizzato, ")

            StrSQL.AppendLine("        ISNULL(PDC_Dettagli.Tipo_Lotta_Acquisti, '') AS Tipo_Lotta_Acquisti, ")

            StrSQL.AppendLine("        CASE WHEN PDC_DisciplinareAcquisti.ID_DisciplinareAcquisti IS NULL THEN  ISNULL(CAST(PDC_Dettagli.Tipo_Lotta_Acquisti AS nvarchar(500)) , '') ELSE ISNULL(CAST(PDC_DisciplinareAcquisti.Descrizione_Disciplinare AS nvarchar(500)) , '') END AS Tipo_Lotta_Acquisti_des, ")

            StrSQL.AppendLine("        ISNULL(PDC_PuntoDiPrelievo.Descrizione_PuntoDiPrelievo,'') AS PuntoPrelievo,  ")
            StrSQL.AppendLine("        ISNULL(PDC_DisciplinareAcquisti.ID_DisciplinareAcquisti,'')  AS TipoLotta_VAL , ISNULL(PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo,'') AS PuntoPrelievo_VAL ")
            StrSQL.AppendLine("        , ISNULL(GruppoVarietale.Grva_Des,ISNULL((select GruppoVarietale.Grva_Des + ' -- Ibrido ' from GruppoVarietale where GruppoVarietale.Grva_Cod = (0-PDC_Dettagli.Grva_Cod)),'')) AS Tipologia_Varietale  ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Documentazione,'') AS Documentazione ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Note_Documentazione,'') AS Note_Documentazione ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Fornitore_Azienda,'') AS Fornitore_Azienda ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.CapitolatiEsclusi,'') AS CapitolatiEsclusi ")

            ' nuovi campi PDC_Dettagli
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Piva_OP,'') AS Piva_OP ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Rag_Soc_OP,'') AS Rag_Soc_OP ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Grower_Number,'') AS Grower_Number ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Kpin,'') AS Kpin ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Block,'') AS Block ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Ind_Des,'') AS Ind_Des ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Frz_Des,'') AS Frz_Des ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.CAP,'') AS CAP ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Com_Des,'') AS Com_Des ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Pro_Cod,'') AS Pro_Cod ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Reg,'') AS Reg ")
            StrSQL.AppendLine("        , ISNULL(Lista_Regioni.Regione_Des,'') AS Regione ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Stato,'') AS Stato ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Pro_Cod_Istat,'') AS Pro_Cod_Istat ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Com_Cod_Istat,'') AS Com_Cod_Istat ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Tecnico_Campionamento,'') AS Tecnico_Campionamento ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Tecnico_Campionamento_Tel,'') AS Tecnico_Campionamento_Tel ")

            StrSQL.AppendLine("        , PDC_Dettagli.data_creazione")
            StrSQL.AppendLine("        , PDC_Dettagli.data_modifica")
            StrSQL.AppendLine("        , PDC_Dettagli.username_creazione")
            StrSQL.AppendLine("        , PDC_Dettagli.username_modifica")

            'Colonna Progetto_Nome per Lotto Impianto
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Progetto_Nome, '') AS Progetto_Nome ")

            'Colonne X e Y per Lat e Long Appezzamento
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.App_Lat, 0) AS App_Lat ")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.App_Long, 0) AS App_Long ")

            'COLONNE ZOO
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Cod_Animale, 0) AS Cod_Animale, ISNULL(PDC_Dettagli.Cod_Progetto, 0) AS Cod_Progetto")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.GEN_COD, 0) AS GEN_COD, ISNULL(Lista_Generi_Animali.GEN_DES, '') AS Genere")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.SPE_COD, 0) AS SPE_COD, ISNULL(Lista_Specie_Animali.SPE_DES, '') AS Specie")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.RAZ_COD, 0) AS RAZ_COD, ISNULL(Lista_Razze_Animali.RAZ_DES, '') AS Razza")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Data_Nascita, '') AS Data_Nascita")
            StrSQL.AppendLine("        , CASE WHEN ISNULL(PDC_Dettagli.Data_Nascita, '') <> '' THEN DATEDIFF(day, PDC_Dettagli.Data_Nascita, getdate()) ELSE 0 END AS Giorni_Vita")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Sesso, '') AS Sesso")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Matricola, '') AS Matricola")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Lotto, '') AS Lotto")
            StrSQL.AppendLine("        , ISNULL(PDC_Dettagli.Raggruppamento_Cod, 0) AS Raggruppamento_Cod, ISNULL(Stalla_Raggruppamenti.Raggruppamento_Des, '') AS Raggruppamento")
            StrSQL.AppendLine("        , COALESCE(Stalla.STA_NUM, 0) as Sta_Num, COALESCE(Stalla.Sta_Des, '') as Sta_Des ")

            StrSQL.AppendLine(" FROM PDC_Dettagli (NOLOCK) ")
            StrSQL.AppendLine(" LEFT OUTER JOIN (SELECT    PDC_Dettagli.* FROM          
                                                                PDC_Dettagli (NOLOCK) 
                                                                WHERE      (IsNumeric(Tipo_Lotta_Acquisti) <> 0)) AS PDC_Dettagli_numeri 
                                        ON PDC_Dettagli_numeri.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND PDC_Dettagli_numeri.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli  ")


            StrSQL.AppendLine(" LEFT JOIN Cultivar (NOLOCK) ON PDC_Dettagli.Cul_Cod = Cultivar.Cul_Cod AND PDC_Dettagli.Veg_Cod = Cultivar.Veg_Cod  ")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali (NOLOCK) ON SpecieVegetali.Veg_Cod = PDC_Dettagli.Veg_Cod ")
            StrSQL.AppendLine(" LEFT OUTER JOIN GruppoVarietale (NOLOCK) ON PDC_Dettagli.Grva_Cod = GruppoVarietale.Grva_Cod ")
            StrSQL.AppendLine(" LEFT OUTER JOIN PDC_DisciplinareAcquisti (NOLOCK) ON PDC_Dettagli_numeri.Tipo_Lotta_Acquisti = PDC_DisciplinareAcquisti.ID_DisciplinareAcquisti  ")
            StrSQL.AppendLine(" LEFT OUTER JOIN PDC_PuntoDiPrelievo (NOLOCK) ON PDC_Dettagli.PuntoPrelievo = PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo  ")
            StrSQL.AppendLine(" LEFT JOIN PDC_LFO (NOLOCK) ON PDC_LFO.ID_LFO  = PDC_Dettagli.ID_LFO ")
            StrSQL.AppendLine(" LEFT JOIN Lista_Regioni (NOLOCK) ON PDC_Dettagli.Reg  = Lista_Regioni.REG ")

            'JOIN ZOO
            StrSQL.AppendLine(" LEFT JOIN Lista_Generi_Animali (NOLOCK) ON ")
            StrSQL.AppendLine(" 	Lista_Generi_Animali.GEN_COD = PDC_Dettagli.GEN_COD")
            StrSQL.AppendLine(" LEFT JOIN Lista_Specie_Animali (NOLOCK) ON ")
            StrSQL.AppendLine(" 	Lista_Specie_Animali.GEN_COD = PDC_Dettagli.GEN_COD")
            StrSQL.AppendLine(" 	AND Lista_Specie_Animali.SPE_COD = PDC_Dettagli.SPE_COD")
            StrSQL.AppendLine(" LEFT JOIN Lista_Razze_Animali (NOLOCK) ON  ")
            StrSQL.AppendLine(" 	Lista_Razze_Animali.GEN_COD = PDC_Dettagli.GEN_COD")
            StrSQL.AppendLine(" 	AND Lista_Razze_Animali.SPE_COD = PDC_Dettagli.SPE_COD")
            StrSQL.AppendLine(" 	AND Lista_Razze_Animali.RAZ_COD = PDC_Dettagli.RAZ_COD")
            StrSQL.AppendLine(" LEFT JOIN Stalla_Raggruppamenti (NOLOCK) ON ")
            StrSQL.AppendLine(" 	Stalla_Raggruppamenti.Raggruppamento_Cod = PDC_Dettagli.Raggruppamento_Cod")

            StrSQL.AppendLine(" LEFT JOIN Stalla (NOLOCK) ON Stalla_Raggruppamenti.Piva = Stalla.Piva ")
            StrSQL.AppendLine("     AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod ")
            StrSQL.AppendLine("     AND Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM")

            StrSQL.AppendLine(" WHERE PDC_Dettagli.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If

            If Cul_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod))
            End If

            If Id_LFO <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.ID_LFO = " & Agro_SQL_SaveNum(Id_LFO))
            End If

            If Flag_PDC <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Flag_PDC = " & Agro_SQL_SaveNum(Flag_PDC))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY  PDC_Dettagli.Rag_Soc, PDC_Dettagli.Sa_Nome, Veg_Des,  Cul_Des ASC")
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

    Public Function LeggiDettagli_CampionieAnalisi(ByVal ID_PDC_Testata As Integer,
                          ByVal ID_PDC_Dettagli As Integer,
                          ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal Id_Reg As Integer,
                          ByVal Veg_Cod As Integer,
                          ByVal Cul_Cod As Integer,
                          ByVal Id_LFO As Integer,
                          ByVal Data_Distinta_Attiva_Inizio As Date,
                          ByVal Data_Distinta_Attiva_Fine As Date,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.LeggiDettagli_CampionieAnalisi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT PDC_Dettagli.*, LFO_Des + ' ' as LFO_Des , ")
            StrSQL.AppendLine(" Imprese.rag_soc + ' 'as rag_soc, Sa_Nome+ ' 'as Sa_Nome,  ")
            StrSQL.AppendLine(" App_Nome+ ' ' as App_Nome, Sup_Imp, Cul_Des+ ' ' as Cul_Des, ")
            StrSQL.AppendLine(" Veg_Des+ ' ' as Veg_Des , isnull(PDC_Campione_Des,'')as PDC_Campione_Des ,  ")
            StrSQL.AppendLine(" isnull(ID_PDC_Stato_Campione ,0) as ID_PDC_Stato_Campione,  ")
            StrSQL.AppendLine(" isnull(Convert(varchar(50), Data_Campionamento, 103),'') as Data_Campionamento, ")
            StrSQL.AppendLine(" isnull(Codice_Campione ,'')as Codice_Campione,  ")
            StrSQL.AppendLine(" isnull(PDC_Campione_Des ,'') as PDC_Campione_Des,  ")
            StrSQL.AppendLine(" CONVERT(VARCHAR(10),Imprese_Progetti.Data_Fine_Prevista, 103) as Data_Fine_Prevista, Imprese_Progetti.Data_Fine_Prevista as Data_Fine_Prevista_Data ")


            StrSQL.AppendLine(" FROM PDC_Dettagli ")
            StrSQL.AppendLine(" INNER JOIN Imprese ")
            StrSQL.AppendLine(" ON Imprese.PIVA = PDC_Dettagli.Piva ")

            StrSQL.AppendLine(" INNER JOIN Centri_Aziendali  ")
            StrSQL.AppendLine(" ON Centri_Aziendali.PIVA = PDC_Dettagli.Piva  ")
            StrSQL.AppendLine(" AND Centri_Aziendali.Sa_Cod = PDC_Dettagli.Sa_Cod  ")

            StrSQL.AppendLine(" INNER JOIN Appezzamento ON Appezzamento.PIVA = PDC_Dettagli.Piva      ")
            StrSQL.AppendLine(" AND Appezzamento.Appezza = PDC_Dettagli.Appezza      ")
            StrSQL.AppendLine(" AND Appezzamento.Sa_Cod = PDC_Dettagli.Sa_Cod ")

            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = PDC_Dettagli.Piva ")
            StrSQL.AppendLine(" AND Reg_Impianti.Appezza = PDC_Dettagli.Appezza      ")
            StrSQL.AppendLine(" AND Reg_Impianti.Sa_Cod = PDC_Dettagli.Sa_Cod      ")
            StrSQL.AppendLine(" AND Reg_Impianti.Id_Reg = PDC_Dettagli.Id_Reg ")

            StrSQL.AppendLine(" INNER JOIN Cultivar ON PDC_Dettagli.CUL_COD = Cultivar.Cul_Cod  ")
            StrSQL.AppendLine(" INNER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = PDC_Dettagli.Veg_Cod ")
            StrSQL.AppendLine(" LEFT JOIN PDC_LFO ON PDC_LFO.ID_LFO  = PDC_Dettagli.ID_LFO ")


            StrSQL.AppendLine(" LEFT JOIN PDC_Campioni  ON PDC_Dettagli.PivaSuperUser  = PDC_Campioni.PivaSuperUser ")
            StrSQL.AppendLine(" AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata  ")
            StrSQL.AppendLine(" AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli ")

            StrSQL.AppendLine(" LEFT JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser  = PDC_Analisi.PivaSuperUser ")
            StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata  ")
            StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli  ")
            StrSQL.AppendLine(" AND PDC_Campioni.ID_PDC_Campione  = PDC_Analisi.ID_PDC_Campione  ")


            StrSQL.AppendLine(" LEFT JOIN  ")
            StrSQL.AppendLine("     Imprese_Progetti ON PDC_Dettagli.Piva = Imprese_Progetti.Piva AND PDC_Dettagli.Sa_Cod = Imprese_Progetti.Sa_Cod AND  ")
            StrSQL.AppendLine("     PDC_Dettagli.Appezza = Imprese_Progetti.Appezza And PDC_Dettagli.Id_Reg = Imprese_Progetti.Id_Reg ")

            StrSQL.AppendLine(" WHERE PDC_Dettagli.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If


            If Piva <> "" Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            If Veg_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod))
            End If

            If Cul_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Cul_Cod = " & Agro_SQL_SaveNum(Cul_Cod))
            End If

            If Id_LFO <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.ID_LFO = " & Agro_SQL_SaveNum(Id_LFO))
            End If
            StrSQL.AppendLine(" AND PDC_Dettagli.FLAG_PDC = -1")


            'data distinta
            StrSQL.AppendLine(" AND Imprese_Progetti.Validita_inizio <= " & Agro_SQL_SaveDate(Data_Distinta_Attiva_Fine))
            StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Fine <= " & Agro_SQL_SaveDate(Data_Distinta_Attiva_Inizio))


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY  Rag_Soc, Sa_Nome, Veg_Des,  Cul_Des ASC")
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



    Public Function Leggi_x_TabellaBloccoSblocco_Campioni_e_Analisi(ByVal ID_PDC_Testata As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal xOrderBy As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi_x_TabellaBloccoSblocco()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------


            StrSQL.AppendLine("  SELECT     Analisi_Testata.Analisi_Testata_Cod, Analisi_Testata.Analisi_Testata_Des, PDC_Campioni.Codice_Campione, PDC_Dettagli.Piva, PDC_Dettagli.Sa_Cod, analisi_testata_data_fine, ")
            StrSQL.AppendLine("  PDC_Dettagli.Appezza, PDC_Dettagli.Id_Reg , PDC_Stato_Analisi , Mostra_in_Stampe ")
            StrSQL.AppendLine("  FROM         PDC_Campioni left JOIN  ")
            StrSQL.AppendLine("  PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND   ")
            StrSQL.AppendLine("  PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione left JOIN  ")
            StrSQL.AppendLine("  Analisi_Testata ON PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod AND   ")
            StrSQL.AppendLine("  PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser left JOIN  ")
            StrSQL.AppendLine("  PDC_Dettagli ON PDC_Campioni.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND   ")
            StrSQL.AppendLine("  PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli")


            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" WHERE PDC_Dettagli.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


    Public Function Leggi_x_Conformita_BloccoSblocco(ByVal ID_PDC_Testata As Integer,
                                                     ByVal tipologiaAnalisi As Integer,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi_x_TabellaBloccoSblocco()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("--TODO: Parto da PDC_Dettagli")

            StrSQL.AppendLine("SELECT ")

            'StrSQL.AppendLine("      PDC_LFO.LFO_Des, PDC_D.Rag_Soc, PDC_D.Sa_Nome, PDC_D.App_Nome")
            'StrSQL.AppendLine("      , Cultivar.Cul_Des, SpecieVegetali.Veg_Des, PDC_D.CapitolatoPrivato, PDC_D.Flag_Pdc")
            'StrSQL.AppendLine("      , ISNULL(PDC_C.Codice_Campione,'') AS Codice_Campione")
            'StrSQL.AppendLine("      , PDC_A.Analisi_Testata_Cod, AnaT.Analisi_Testata_Des, AnaT.Analisi_Testata_Data_Fine")

            StrSQL.AppendLine("   'PDC_D', PDC_D.PivaSuperUser, PDC_D.ID_PDC_Testata, PDC_D.ID_PDC_Dettagli, PDC_D.Flag_Pdc")
            StrSQL.AppendLine("   , PDC_D.Piva, PDC_D.Sa_Cod, PDC_D.Appezza, PDC_D.Id_Reg")
            StrSQL.AppendLine("   , PDC_D.Cul_Cod, PDC_D.Veg_Cod, PDC_D.ID_LFO")
            StrSQL.AppendLine("   , PDC_D.Rag_Soc, PDC_D.App_Nome, PDC_D.Sa_Nome")
            StrSQL.AppendLine("   , PDC_D.CapitolatoPrivato, PDC_D.Certificato, PDC_D.Regolamento, PDC_D.Regolamento_Cod")
            StrSQL.AppendLine("   , ISNULL(CONVERT(VARCHAR(10), PDC_D.Data_Fornitura, 103), '') AS Data_Fornitura_Str ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.NomeArticolo, '') AS NomeArticolo ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.CodiceFornitore, '') AS CodiceFornitore ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.Grower_Number, '') AS Grower_Number ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.Kpin, '') AS Kpin, ISNULL(PDC_D.[Block], '') AS [Block] ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.Piva_OP, '') AS Piva_OP, ISNULL(PDC_D.Rag_Soc_OP, '') AS Rag_Soc_OP ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.Regolamento,'') AS Regolamento, ISNULL(PDC_D.TipoLotta,'') AS Disciplinare ")

            StrSQL.AppendLine("   , PDC_LFO.LFO_Des, SpecieVegetali.Veg_Des, Cultivar.Cul_Des")

            'Colonne Progetto_Nome per Lotto Impianto
            StrSQL.AppendLine("        , ISNULL(PDC_D.Progetto_Nome, '') AS Progetto_Nome ")

            'Colonne X e Y per Lat e Long
            StrSQL.AppendLine("        , ISNULL(PDC_D.App_Lat, 0) AS App_Lat ")
            StrSQL.AppendLine("        , ISNULL(PDC_D.App_Long, 0) AS App_Long ")

            StrSQL.AppendLine("   , 'PDC_C'")
            StrSQL.AppendLine("   , PDC_C.ID_PDC_Campione, PDC_C.ID_PDC_Stato_Campione")
            StrSQL.AppendLine("   , ISNULL(PDC_C.Codice_Campione,'') AS Codice_Campione")
            StrSQL.AppendLine("   , ISNULL(PDC_C.PDC_Campione_Des, '') AS PDC_Campione_Des")
            StrSQL.AppendLine("   , ISNULL(CONVERT(VARCHAR(10), PDC_C.Data_Campionamento, 103), '') AS Data_Campionamento")
            StrSQL.AppendLine("   , ISNULL(PDC_C.X, 0) AS Lat_Campione ")
            StrSQL.AppendLine("   , ISNULL(PDC_C.Y, 0) AS Long_Campione ")


            StrSQL.AppendLine("   , 'PDC_A'")
            StrSQL.AppendLine("   , PDC_A.ID_PDC_Testata, PDC_A.ID_PDC_Dettagli, PDC_A.ID_PDC_Campione, PDC_A.Analisi_Testata_Cod")
            StrSQL.AppendLine("   , PDC_A.PDC_Stato_Analisi")
            StrSQL.AppendLine("   , PDC_A.PDC_Stato_Pubblicazione")
            StrSQL.AppendLine("   , CASE PDC_A.PDC_Stato_Pubblicazione")
            StrSQL.AppendLine("     WHEN 3 THEN 'Da Rimuovere'")
            StrSQL.AppendLine("     WHEN 2 THEN 'Pubblicata'")
            StrSQL.AppendLine("     WHEN 1 THEN 'Da Pubblicare'")
            StrSQL.AppendLine("     WHEN 0 THEN 'Non Pubblicata'")
            StrSQL.AppendLine("     ELSE ''")
            StrSQL.AppendLine("     END AS PDC_Stato_Pubblicazione_Des")
            StrSQL.AppendLine("   , ISNULL(PDC_Stato_Analisi.PDC_Stato_Analisi_Des, '') AS PDC_Stato_Analisi_Des")
            StrSQL.AppendLine("   , PDC_A.Cod_Risum")
            StrSQL.AppendLine("   , PDC_A.Analisi_Tipologia_Cod, Analisi_Tipologia.Analisi_Tipologia_Des")
            StrSQL.AppendLine("   , Analisi_Tipologia.Analisi_Tipologia_Tipo")
            'TODO: Completa con elenco analisi completo!!!
            StrSQL.AppendLine("   , CASE Analisi_Tipologia.Analisi_Tipologia_Tipo")
            StrSQL.AppendLine("     WHEN 8 THEN 'Analisi dei Fitofarmaci'")
            StrSQL.AppendLine("     WHEN 11 THEN 'Analisi Merceologiche'")
            StrSQL.AppendLine("     ELSE ''")
            StrSQL.AppendLine("     END AS Analisi_Tipologia_Tipo_Des")
            StrSQL.AppendLine("   , ISNULL(PDC_A.Altre_Molecole,'') AS Altre_Molecole, ISNULL(PDC_A.Altre_Molecole_Cod,'') AS Altre_Molecole_Cod")
            StrSQL.AppendLine("   , ISNULL(PDC_A.Mostra_in_Stampe,'') AS Mostra_In_Stampe,  ISNULL(PDC_A.Note_Richiesta_Analisi,'') AS Note_Richiesta_Analisi")
            StrSQL.AppendLine("   , CASE PDC_A.Mostra_in_Stampe WHEN -1 THEN 'true' ELSE 'false' END AS Analisi_Riferimento")

            StrSQL.AppendLine("   --, PDC_A.*")

            StrSQL.AppendLine("   , Contatti.Rag_Soc AS Cod_Risum_Des")

            StrSQL.AppendLine("   , 'AnaT', AnaT.Analisi_Testata_Cod, AnaT.Analisi_Testata_Des, AnaT.Analisi_Testata_Data_Inizio, AnaT.Analisi_Testata_Data_Fine")
            StrSQL.AppendLine("   , ISNULL(CONVERT(VARCHAR(10), AnaT.Analisi_Testata_Data_Fine, 103),'') AS Data_Richiesta_Analisi ")
            StrSQL.AppendLine("   --, 'AnaCCC', AnaCCC.CapitolatoCliente_Cod, AnaCCC.Esito, AnaCCC.Descrizione_Esito, AnaCCC.Capitolato_Des")
            StrSQL.AppendLine("   --, 'PDC_S', PDC_S.CapitolatoCliente_Cod, PDC_S.Esito, PDC_S.note, PDC_S.data_sblocco")

            StrSQL.AppendLine("FROM PDC_Dettagli PDC_D")
            StrSQL.AppendLine("INNER JOIN PDC_LFO ON PDC_D.PivaSuperUser = PDC_LFO.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_LFO.ID_PDC_Testata AND PDC_D.ID_LFO = PDC_LFO.ID_LFO")
            StrSQL.AppendLine("LEFT JOIN SpecieVegetali ON PDC_D.Veg_Cod = SpecieVegetali.Veg_Cod")
            StrSQL.AppendLine("LEFT JOIN Cultivar ON PDC_D.Cul_Cod = Cultivar.Cul_Cod")

            'Inverto l'ordine dei join per escludere campioni realizzati perché collegati ad analisi di altro tipo

            'StrSQL.AppendLine("LEFT JOIN PDC_Campioni PDC_C ON PDC_D.PivaSuperUser = PDC_C.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_C.ID_PDC_Testata AND PDC_D.ID_PDC_Dettagli = PDC_C.ID_PDC_Dettagli")
            'StrSQL.AppendLine("LEFT JOIN PDC_Analisi PDC_A ON PDC_C.PivaSuperUser = PDC_A.PivaSuperUser AND PDC_C.ID_PDC_Testata = PDC_A.ID_PDC_Testata AND PDC_C.ID_PDC_Dettagli = PDC_A.ID_PDC_Dettagli")
            'StrSQL.AppendLine("                            AND PDC_C.ID_PDC_Campione = PDC_A.ID_PDC_Campione")
            'If tipologiaAnalisi <> 0 Then
            '    StrSQL.AppendLine("                            AND PDC_A.Analisi_Tipologia_Tipo = " & Agro_SQL_SaveNum(tipologiaAnalisi))
            'End If

            StrSQL.AppendLine("LEFT JOIN PDC_Analisi PDC_A ON PDC_D.PivaSuperUser = PDC_A.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_A.ID_PDC_Testata AND PDC_D.ID_PDC_Dettagli = PDC_A.ID_PDC_Dettagli")
            If tipologiaAnalisi <> 0 Then
                StrSQL.AppendLine("                            AND PDC_A.Analisi_Tipologia_Tipo = " & Agro_SQL_SaveNum(tipologiaAnalisi))
            End If
            StrSQL.AppendLine("LEFT JOIN PDC_Campioni PDC_C ON PDC_A.PivaSuperUser = PDC_C.PivaSuperUser AND PDC_A.ID_PDC_Testata = PDC_C.ID_PDC_Testata AND PDC_A.ID_PDC_Dettagli = PDC_C.ID_PDC_Dettagli")
            StrSQL.AppendLine("                            AND PDC_A.ID_PDC_Campione = PDC_C.ID_PDC_Campione")

            StrSQL.AppendLine("LEFT JOIN PDC_Stato_Analisi ON PDC_A.PDC_Stato_Analisi = PDC_Stato_Analisi.ID_PDC_Stato_Analisi")

            StrSQL.AppendLine("LEFT JOIN Risorse_Umane ON PDC_A.Cod_Risum = Risorse_Umane.Cod_RisUm  ")
            StrSQL.AppendLine("LEFT JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  ")

            StrSQL.AppendLine("--LEFT JOIN PDC_FornitoreFatturazione ON PDC_A.Piva_FornitoreFatturazione = PDC_FornitoreFatturazione.Piva")
            StrSQL.AppendLine("LEFT JOIN Analisi_Testata AnaT ON PDC_A.PivaSuperUser = AnaT.Analisi_SuperUser AND PDC_A.Analisi_Testata_Cod = AnaT.Analisi_Testata_Cod")
            StrSQL.AppendLine("LEFT JOIN Analisi_Tipologia ON PDC_A.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod")

            StrSQL.AppendLine("--LEFT JOIN Analisi_Conformita_Capitolato_Cliente AnaCCC ON PDC_A.PivaSuperUser = AnaCCC.PivaSuperUser AND PDC_A.Analisi_Testata_Cod = AnaCCC.Analisi_Testata_Cod")

            StrSQL.AppendLine("--LEFT JOIN PDC_Sblocca PDC_S ON PDC_D.PivaSuperUser = PDC_S.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_S.ID_PDC_Testata ")
            StrSQL.AppendLine("--                           AND PDC_D.Piva = PDC_S.Piva AND PDC_D.Sa_Cod = PDC_S.Sa_Cod AND PDC_D.Appezza = PDC_S.Appezza AND PDC_D.Id_Reg = PDC_S.Id_Reg")
            StrSQL.AppendLine("----                         AND AnaCCC.CapitolatoCliente_Cod = PDC_S.CapitolatoCliente_Cod")


            StrSQL.AppendLine("WHERE 1 = 1")
            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_D.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            'StrSQL.AppendLine("GROUP BY PDC_LFO.LFO_Des, PDC_D.Rag_Soc, PDC_D.Sa_Nome, PDC_D.App_Nome")
            'StrSQL.AppendLine("         , Cultivar.Cul_Des, SpecieVegetali.Veg_Des, PDC_D.CapitolatoPrivato, PDC_D.Flag_Pdc")
            'StrSQL.AppendLine("         , PDC_C.Codice_Campione")
            'StrSQL.AppendLine("         , PDC_A.Analisi_Testata_Cod, AnaT.Analisi_Testata_Des, AnaT.Analisi_Testata_Data_Fine")
            'StrSQL.AppendLine("         -- qui risultati di conformità analisi")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PDC_LFO.LFO_Des")
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

    Public Function Leggi_x_MarketAccess(ByVal ID_PDC_Testata As Integer,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal F2BAttivo As Boolean = False
                                         ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi_x_MarketAccess()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT ")

            StrSQL.AppendLine("   PDC_T.PDC_Testata_Des, PDC_T.PivaOwner ")
            StrSQL.AppendLine("   , ISNULL(icFLEX.val_cod,'') AS Facility, ISNULL(icGGN.val_cod,'') AS GGN ")
            StrSQL.AppendLine("   , PDC_D.PivaSuperUser, PDC_D.ID_PDC_Testata, PDC_D.ID_PDC_Dettagli, PDC_D.Flag_Pdc")
            StrSQL.AppendLine("   , PDC_D.Piva, PDC_D.Sa_Cod, PDC_D.Appezza, PDC_D.Id_Reg")
            StrSQL.AppendLine("   , PDC_D.Cul_Cod, PDC_D.Veg_Cod, PDC_D.ID_LFO")
            StrSQL.AppendLine("   , PDC_D.Rag_Soc, PDC_D.App_Nome, PDC_D.Sa_Nome")
            StrSQL.AppendLine("   , UPPER(ISNULL(PDC_D.Stato, '')) AS Nazione, ISNULL(r.Regione_Des, '') AS Regione, ISNULL(PDC_D.Frz_Des, '') AS Frz_Des")
            StrSQL.AppendLine("   , PDC_D.CapitolatoPrivato, PDC_D.Certificato, PDC_D.Regolamento, PDC_D.Regolamento_Cod")
            StrSQL.AppendLine("   , ISNULL(CONVERT(VARCHAR(10), PDC_D.Data_Fornitura, 103), '') AS Data_Fornitura_Str ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.NomeArticolo, '') AS NomeArticolo ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.CodiceFornitore, '') AS CodiceFornitore ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.Grower_Number, '') AS Grower_Number ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.Kpin, '') AS Kpin, ISNULL(PDC_D.[Block], '') AS [Block] ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.Piva_OP, '') AS Piva_OP, ISNULL(PDC_D.Rag_Soc_OP, '') AS Rag_Soc_OP ")
            StrSQL.AppendLine("   , ISNULL(PDC_D.Regolamento,'') AS Regolamento, ISNULL(PDC_D.TipoLotta,'') AS Disciplinare ")

            StrSQL.AppendLine("   , PDC_LFO.LFO_Des, SpecieVegetali.Veg_Des, Cultivar.Cul_Des")

            If F2BAttivo Then
                StrSQL.AppendLine("   , ISNULL(PDC_F.Stato, -1) AS Flex_Stato, ISNULL(PDC_F.PayLoad, '') AS Flex_Payload, PDC_F.Data_Operazione AS Flex_Data ")
            End If

            StrSQL.AppendLine(" FROM PDC_Dettagli PDC_D")
            StrSQL.AppendLine(" INNER JOIN PDC_LFO ON PDC_D.PivaSuperUser = PDC_LFO.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_LFO.ID_PDC_Testata AND PDC_D.ID_LFO = PDC_LFO.ID_LFO")
            StrSQL.AppendLine(" INNER JOIN PDC_Testata PDC_T ON PDC_D.PivaSuperUser = PDC_T.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_T.Id_PDC_Testata")
            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali ON PDC_D.Veg_Cod = SpecieVegetali.Veg_Cod")
            StrSQL.AppendLine(" LEFT JOIN Cultivar ON PDC_D.Cul_Cod = Cultivar.Cul_Cod")
            StrSQL.AppendLine(" LEFT JOIN Lista_Regioni r ON PDC_D.Reg = r.REG AND PDC_D.Reg <> '000'")
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici AS icGGN ON icGGN.piva = PDC_D.Piva AND icGGN.id_cod = 1261 ")
            StrSQL.AppendLine(" LEFT JOIN Imprese_Codici AS icFLEX ON icFLEX.piva = PDC_T.PivaOwner AND icFLEX.id_cod = 1329 ")

            If F2BAttivo Then
                StrSQL.AppendLine(" LEFT JOIN PDC_Flex2B PDC_F ON PDC_D.PivaSuperUser = PDC_F.PivaSuperUser AND PDC_D.ID_PDC_Testata = PDC_F.Id_PDC_Testata AND PDC_D.ID_PDC_Dettagli = PDC_F.ID_PDC_Dettagli ")
            End If

            StrSQL.AppendLine(" WHERE 1 = 1")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND PDC_D.ID_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY PDC_T.PDC_Testata_Des, PDC_LFO.LFO_Des")
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

    Public Function Leggi_x_TabellaBloccoSblocco_EXPORT(ByVal ID_PDC_Testata As Integer,
                                                        ByVal xFiltroAggiuntivo As String,
                                                        ByVal xOrderBy As String,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi_x_TabellaBloccoSblocco()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT        PDC_LFO.LFO_Des AS 'LFO', PDC_Dettagli.Rag_Soc AS 'Ragione Sociale', PDC_Dettagli.Sa_Nome AS 'Centro Aziendale', PDC_Dettagli.App_Nome AS 'Appezzamento', SpecieVegetali.Veg_Des AS 'Specie Vegetali', Cultivar.Cul_Des AS 'Varietà', PDC_Dettagli.CapitolatoPrivato AS 'Capitolato Privato', PDC_Dettagli.Certificato, ISNULL(PDC_Campioni.Codice_Campione, '') AS 'Codice Campione', ISNULL(CONVERT(varchar(24),PDC_Campioni.Data_Campionamento,103 ), '') AS 'Data Campione',  ISNULL(PDC_Campioni.Note_Campione, '') AS 'Note Campione', ISNULL(PDC_PuntoDiPrelievo.Descrizione_PuntoDiPrelievo, '') AS 'Punto di Prelievo' , ISNULL(Contatti.Rag_Soc, '') AS 'Laboratorio', isnull(Analisi_Tipologia.Analisi_Tipologia_Des,'')as 'Tipo Analisi', ISNULL(CONVERT(varchar(24),PDC_Analisi.Data_Richiesta_Analisi,103 ), '') AS 'Data Richiesta',  ISNULL(PDC_Analisi.Altre_Molecole, '') AS 'Altre Molecole', ISNULL(PDC_Analisi.Note_Richiesta_Analisi, '') AS 'Note Richiesta Analisi',  isnull(PDC_Analisi.Piva_FornitoreFatturazione,'') as 'Fatturazione' " & vbNewLine)
            StrSQL.AppendLine(" , pdc_dettagli.piva, pdc_dettagli.sa_cod, pdc_dettagli.appezza, pdc_dettagli.id_reg , pdc_analisi.Analisi_Testata_Cod" & vbNewLine)
            StrSQL.AppendLine(" FROM PDC_Analisi " & vbNewLine)
            StrSQL.AppendLine(" 	LEFT OUTER JOIN Analisi_Tipologia ON PDC_Analisi.PivaSuperUser = Analisi_Tipologia.PivaSuperUser AND  PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod  " & vbNewLine)
            StrSQL.AppendLine(" RIGHT OUTER JOIN PDC_Campioni  " & vbNewLine)
            StrSQL.AppendLine(" LEFT OUTER JOIN PDC_PuntoDiPrelievo ON PDC_Campioni.PuntoPrelievo = PDC_PuntoDiPrelievo.ID_PuntoDiPrelievo ON PDC_Analisi.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Analisi.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Analisi.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli AND PDC_Analisi.ID_PDC_Campione = PDC_Campioni.ID_PDC_Campione  " & vbNewLine)
            StrSQL.AppendLine(" RIGHT OUTER JOIN SpecieVegetali  " & vbNewLine)
            StrSQL.AppendLine(" INNER JOIN PDC_Dettagli ON SpecieVegetali.Veg_Cod = PDC_Dettagli.Veg_Cod  " & vbNewLine)
            StrSQL.AppendLine(" INNER JOIN Cultivar ON PDC_Dettagli.Cul_Cod = Cultivar.Cul_Cod ON PDC_Campioni.ID_PDC_Dettagli = PDC_Dettagli.ID_PDC_Dettagli AND  PDC_Campioni.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND PDC_Campioni.PivaSuperUser = PDC_Dettagli.PivaSuperUser  " & vbNewLine)
            StrSQL.AppendLine(" LEFT OUTER JOIN PDC_LFO ON PDC_Dettagli.PivaSuperUser = PDC_LFO.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_LFO.ID_PDC_Testata AND PDC_Dettagli.ID_LFO = PDC_LFO.ID_LFO  " & vbNewLine)
            StrSQL.AppendLine(" FULL OUTER JOIN Risorse_Umane  " & vbNewLine)
            StrSQL.AppendLine(" INNER JOIN Contatti ON Risorse_Umane.Piva = Contatti.Piva AND Risorse_Umane.Sa_Cod = Contatti.Sa_Cod AND Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto ON  " & vbNewLine)
            StrSQL.AppendLine(" PDC_Analisi.Cod_Risum = Risorse_Umane.Cod_RisUm " & vbNewLine)


            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" WHERE PDC_Dettagli.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY  LFO_Des")
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




    Public Function Leggi_soloAnalisiEffettuate(ByVal ID_PDC_Testata As Integer,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT     PDC_LFO.LFO_Des, PDC_Dettagli.Rag_Soc, ISNULL(PDC_Dettagli.Sa_Nome,'') as SA_NOME, ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.App_Nome, '') as App_Nome, SpecieVegetali.Veg_Des, Cultivar.Cul_Des,   ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.CapitolatoPrivato, '') as CapitolatoPrivato, ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.Certificato,'' ) as Certificato, ")
            StrSQL.AppendLine("       PDC_Analisi.Analisi_Testata_Cod, Analisi_Tipologia.Analisi_Tipologia_Des,  ")
            StrSQL.AppendLine("       Contatti.Rag_Soc AS Cod_Risum_Des, PDC_Campioni.Codice_Campione, PDC_Campioni.ID_PDC_Stato_Campione, ")
            StrSQL.AppendLine("       ISNULL(PDC_Campioni.PDC_Campione_Des, '') as PDC_Campione_Des,  ")

            StrSQL.AppendLine("       ISNULL(CONVERT(VARCHAR(10), PDC_Dettagli.Data_Fornitura, 103), '') as Data_Fornitura_Str, ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.NomeArticolo, '') as NomeArticolo, ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.CodiceFornitore, '') as CodiceFornitore, ")

            StrSQL.AppendLine("       PDC_Dettagli.Id_PDC_Dettagli, ")
            StrSQL.AppendLine("       ISNULL(PDC_Campioni.ID_PDC_Campione, '') as ID_PDC_Campione, ")

            StrSQL.AppendLine("       CONVERT(VARCHAR(10), PDC_Campioni.Data_Campionamento, 103) AS Data_Campionamento, ")
            StrSQL.AppendLine("       CONVERT(VARCHAR(10), analisi_testata.analisi_testata_data_fine, 103) AS Data_Richiesta_Analisi, ")
            StrSQL.AppendLine("       ISNULL(Analisi_Testata.Analisi_Testata_Des,'') AS Analisi_Testata_Des ")

            StrSQL.AppendLine("FROM Cultivar  ")
            StrSQL.AppendLine("       INNER JOIN SpecieVegetali  ")
            StrSQL.AppendLine("       INNER JOIN PDC_Testata  ")
            StrSQL.AppendLine("       INNER JOIN PDC_Dettagli ON PDC_Testata.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Testata.Id_PDC_Testata = PDC_Dettagli.ID_PDC_Testata  ")
            StrSQL.AppendLine("       INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli  ")
            StrSQL.AppendLine("       INNER JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione  ")
            StrSQL.AppendLine("       INNER JOIN PDC_LFO ON PDC_Dettagli.ID_LFO = PDC_LFO.ID_LFO AND PDC_Dettagli.PivaSuperUser = PDC_LFO.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_LFO.ID_PDC_Testata ON SpecieVegetali.Veg_Cod = PDC_Dettagli.Veg_Cod ON Cultivar.Cul_Cod = PDC_Dettagli.Cul_Cod  ")
            StrSQL.AppendLine("       INNER JOIN Analisi_Tipologia ON PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod  ")
            StrSQL.AppendLine("       INNER JOIN Risorse_Umane ON PDC_Analisi.Cod_Risum = Risorse_Umane.Cod_RisUm  ")
            StrSQL.AppendLine("       INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  ")
            StrSQL.AppendLine("       LEFT OUTER JOIN Analisi_Testata ON PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser AND PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod  ")

            StrSQL.AppendLine(" WHERE PDC_Analisi.PDC_Stato_Analisi = " & enum_PDC_Stato_Analisi.Analizzata)

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND (PDC_Testata.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
                StrSQL.AppendLine(" AND (PDC_Dettagli.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY  PDC_LFO.LFO_Des, PDC_Dettagli.rag_soc, PDC_Dettagli.sa_nome, PDC_Dettagli.APP_NOME, Cultivar.Cul_Des ASC")
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

    Public Function Leggi_soloAnalisiEffettuate_NEW(ByVal ID_PDC_Testata As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT     PDC_LFO.LFO_Des, PDC_Dettagli.Rag_Soc, ISNULL(PDC_Dettagli.Sa_Nome,'') as SA_NOME, ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.App_Nome, '') as App_Nome, SpecieVegetali.Veg_Des, Cultivar.Cul_Des,   ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.CapitolatoPrivato, '') as CapitolatoPrivato, ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.Certificato,'' ) as Certificato, ")
            StrSQL.AppendLine("       PDC_Analisi.Analisi_Testata_Cod, Analisi_Tipologia.Analisi_Tipologia_Des,  ")
            StrSQL.AppendLine("       Contatti.Rag_Soc AS Cod_Risum_Des, PDC_Campioni.Codice_Campione, PDC_Campioni.ID_PDC_Stato_Campione, ")
            StrSQL.AppendLine("       ISNULL(PDC_Campioni.PDC_Campione_Des, '') as PDC_Campione_Des,  ")

            StrSQL.AppendLine("       ISNULL(CONVERT(VARCHAR(10), PDC_Dettagli.Data_Fornitura, 103), '') as Data_Fornitura_Str, ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.NomeArticolo, '') as NomeArticolo, ")
            StrSQL.AppendLine("       ISNULL(PDC_Dettagli.CodiceFornitore, '') as CodiceFornitore, ")

            StrSQL.AppendLine("       PDC_Dettagli.Id_PDC_Dettagli, ")
            StrSQL.AppendLine("       ISNULL(PDC_Campioni.ID_PDC_Campione, '') as ID_PDC_Campione, ")

            StrSQL.AppendLine("       CONVERT(VARCHAR(10), PDC_Campioni.Data_Campionamento, 103) AS Data_Campionamento, ")
            StrSQL.AppendLine("       CONVERT(VARCHAR(10), analisi_testata.analisi_testata_data_fine, 103) AS Data_Richiesta_Analisi, ")
            StrSQL.AppendLine("       ISNULL(Analisi_Testata.Analisi_Testata_Des,'') AS Analisi_Testata_Des, ")

            StrSQL.AppendLine("       PDC_Dettagli.PivaSuperUser, ")
            StrSQL.AppendLine("       PDC_Dettagli.Piva, PDC_Dettagli.Sa_Cod, PDC_Dettagli.Appezza, PDC_Dettagli.Id_Reg, ")
            StrSQL.AppendLine("       PDC_Dettagli.ID_LFO ")

            StrSQL.AppendLine("FROM Cultivar  ")
            StrSQL.AppendLine("       INNER JOIN SpecieVegetali  ")
            StrSQL.AppendLine("       INNER JOIN PDC_Testata  ")
            StrSQL.AppendLine("       INNER JOIN PDC_Dettagli ON PDC_Testata.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_Testata.Id_PDC_Testata = PDC_Dettagli.ID_PDC_Testata  ")
            StrSQL.AppendLine("       INNER JOIN PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli  ")
            StrSQL.AppendLine("       INNER JOIN PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione  ")
            StrSQL.AppendLine("       INNER JOIN PDC_LFO ON PDC_Dettagli.ID_LFO = PDC_LFO.ID_LFO AND PDC_Dettagli.PivaSuperUser = PDC_LFO.PivaSuperUser AND PDC_Dettagli.ID_PDC_Testata = PDC_LFO.ID_PDC_Testata ON SpecieVegetali.Veg_Cod = PDC_Dettagli.Veg_Cod ON Cultivar.Cul_Cod = PDC_Dettagli.Cul_Cod  ")
            StrSQL.AppendLine("       INNER JOIN Analisi_Tipologia ON PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod  ")
            StrSQL.AppendLine("       INNER JOIN Risorse_Umane ON PDC_Analisi.Cod_Risum = Risorse_Umane.Cod_RisUm  ")
            StrSQL.AppendLine("       INNER JOIN Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto  ")
            StrSQL.AppendLine("       LEFT OUTER JOIN Analisi_Testata ON PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser AND PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod  ")

            StrSQL.AppendLine(" WHERE PDC_Analisi.PDC_Stato_Analisi = " & enum_PDC_Stato_Analisi.Analizzata)

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND (PDC_Testata.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
                StrSQL.AppendLine(" AND (PDC_Dettagli.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY  PDC_LFO.LFO_Des, PDC_Dettagli.rag_soc, PDC_Dettagli.sa_nome, PDC_Dettagli.APP_NOME, Cultivar.Cul_Des ASC")
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

    Public Function Leggi_x_TabellaBloccoSblocco(ByVal ID_PDC_Testata As Integer,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi_x_TabellaBloccoSblocco()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------


            StrSQL.AppendLine(" SELECT     PDC_Dettagli.ID_PDC_Testata, pdc_dettagli.flag_pdc,  PDC_Dettagli.ID_PDC_Dettagli,PDC_Dettagli.Piva, PDC_Dettagli.Sa_Cod, PDC_Dettagli.Appezza, PDC_Dettagli.Id_Reg, PDC_Dettagli.Cul_Cod, PDC_Dettagli.Veg_Cod, ")
            StrSQL.AppendLine("             PDC_Dettagli.ID_LFO, PDC_LFO.LFO_Des, pdc_dettagli.capitolatoprivato, PDC_Dettagli.Rag_Soc, PDC_Dettagli.App_Nome,pdc_dettagli.Sa_Nome , SpecieVegetali.Veg_Des, Cultivar.Cul_Des, PDC_Dettagli.Certificato ")
            StrSQL.AppendLine(" FROM PDC_LFO ")
            StrSQL.AppendLine("         RIGHT OUTER JOIN SpecieVegetali ")
            StrSQL.AppendLine("         INNER JOIN PDC_Dettagli ON SpecieVegetali.Veg_Cod = PDC_Dettagli.Veg_Cod ")
            StrSQL.AppendLine("         INNER JOIN Cultivar ON PDC_Dettagli.Cul_Cod = Cultivar.Cul_Cod ON PDC_LFO.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND PDC_LFO.ID_PDC_Testata = PDC_Dettagli.ID_PDC_Testata AND PDC_LFO.ID_LFO = PDC_Dettagli.ID_LFO")



            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" WHERE PDC_Dettagli.Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY  LFO_Des")
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



    Public Function Leggi_soloAnalisiEffettuatex_ModalitaNero(ByVal ID_PDC_Testata As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT PDC_LFO.LFO_Des, Imprese.rag_soc, Centri_Aziendali.sa_nome, Appezzamento.APP_NOME, Cultivar.Cul_Des, SpecieVegetali.Veg_Des, ")
            StrSQL.AppendLine("        PDC_Analisi.Analisi_Testata_Cod, Analisi_Tipologia.Analisi_Tipologia_Des, Contatti.Rag_Soc as Cod_Risum_Des, ")
            StrSQL.AppendLine("        PDC_Campioni.Codice_Campione, PDC_Campioni.ID_PDC_Stato_Campione, PDC_Campioni.PDC_Campione_Des, ")
            StrSQL.AppendLine("        CONVERT(VARCHAR(10),PDC_Campioni.Data_Campionamento , 103) as Data_Campionamento , ")
            StrSQL.AppendLine("        CONVERT(VARCHAR(10),PDC_Analisi.Data_Richiesta_Analisi , 103) as Data_Richiesta_Analisi ")
            StrSQL.AppendLine("        , Analisi_Testata.Analisi_Testata_Des ")


            StrSQL.AppendLine(" FROM         Centri_Aziendali INNER JOIN " &
                      " Cultivar INNER JOIN" &
                      " SpecieVegetali INNER JOIN " &
                      " PDC_Testata INNER JOIN" &
                      " PDC_Dettagli ON PDC_Testata.PivaSuperUser = PDC_Dettagli.PivaSuperUser AND " &
                      " PDC_Testata.Id_PDC_Testata = PDC_Dettagli.ID_PDC_Testata INNER JOIN" &
                      " PDC_Campioni ON PDC_Dettagli.PivaSuperUser = PDC_Campioni.PivaSuperUser AND " &
                      " PDC_Dettagli.ID_PDC_Testata = PDC_Campioni.ID_PDC_Testata AND PDC_Dettagli.ID_PDC_Dettagli = PDC_Campioni.ID_PDC_Dettagli INNER JOIN" &
                      " PDC_Analisi ON PDC_Campioni.PivaSuperUser = PDC_Analisi.PivaSuperUser AND PDC_Campioni.ID_PDC_Testata = PDC_Analisi.ID_PDC_Testata AND" &
                      " PDC_Campioni.ID_PDC_Dettagli = PDC_Analisi.ID_PDC_Dettagli AND PDC_Campioni.ID_PDC_Campione = PDC_Analisi.ID_PDC_Campione INNER JOIN" &
                      " PDC_LFO ON PDC_Dettagli.ID_LFO = PDC_LFO.ID_LFO AND PDC_Dettagli.PivaSuperUser = PDC_LFO.PivaSuperUser AND " &
                      " PDC_Dettagli.ID_PDC_Testata = PDC_LFO.ID_PDC_Testata INNER JOIN" &
                      " Reg_Impianti ON PDC_Dettagli.Piva = Reg_Impianti.PIVA AND PDC_Dettagli.Sa_Cod = Reg_Impianti.SA_COD AND " &
                      " PDC_Dettagli.Appezza = Reg_Impianti.APPEZZA AND PDC_Dettagli.Id_Reg = Reg_Impianti.ID_REG INNER JOIN" &
                      " Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND " &
                      " Reg_Impianti.APPEZZA = Appezzamento.APPEZZA ON SpecieVegetali.Veg_Cod = PDC_Dettagli.Veg_Cod ON " &
                      " Cultivar.Cul_Cod = PDC_Dettagli.Cul_Cod ON Centri_Aziendali.PIVA = Appezzamento.PIVA AND " &
                      " Centri_Aziendali.sa_cod = Appezzamento.SA_COD INNER JOIN" &
                      " Imprese ON Centri_Aziendali.PIVA = Imprese.PIVA INNER JOIN" &
                      " Analisi_Tipologia ON PDC_Analisi.Analisi_Tipologia_Cod = Analisi_Tipologia.Analisi_Tipologia_Cod INNER JOIN" &
                      " Risorse_Umane ON PDC_Analisi.Cod_Risum = Risorse_Umane.Cod_RisUm " &
                      " INNER Join Contatti ON Risorse_Umane.Cod_Contatto = Contatti.Cod_Contatto " &
                      " LEFT OUTER JOIN " &
                      " Analisi_Testata ON PDC_Analisi.PivaSuperUser = Analisi_Testata.Analisi_SuperUser AND  " &
                      " PDC_Analisi.Analisi_Testata_Cod = Analisi_Testata.Analisi_Testata_Cod")


            StrSQL.AppendLine(" WHERE PDC_Analisi.PDC_Stato_Analisi = 2 ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND (PDC_Analisi.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
                StrSQL.AppendLine(" AND (PDC_Dettagli.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
                StrSQL.AppendLine(" AND (PDC_Campioni.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
                StrSQL.AppendLine(" AND (PDC_Testata.Id_PDC_Testata = " & ID_PDC_Testata & ") ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY  PDC_LFO.LFO_Des, Imprese.rag_soc, Centri_Aziendali.sa_nome, Appezzamento.APP_NOME, Cultivar.Cul_Des ASC")
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



    Public Function Exist_Dettaglio(ByVal ID_PDC_Testata As Integer,
                              ByVal ID_PDC_Dettagli As Integer,
                              ByVal Piva As String,
                              ByVal Sa_Cod As Integer,
                              ByVal Appezza As Integer,
                              ByVal Id_Reg As Integer,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As Boolean
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_R.Exist_Dettaglio()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------
            StrSQL.AppendLine(" SELECT ID_PDC_Dettagli ")
            StrSQL.AppendLine(" FROM PDC_Dettagli ")

            StrSQL.AppendLine(" WHERE 1=1")
            StrSQL.AppendLine(" AND   PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND Id_PDC_Testata = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If

            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            End If


            If Piva <> "" Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND PDC_Dettagli.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
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



        If DT.Rows.Count > 0 Then
            Return True
        End If
        Return False

    End Function


    Public Function LeggiProdottiUtilizzati(
                        ByVal Piva As String,
                        ByVal Sa_Cod As Integer,
                        ByVal Appezza As Integer,
                        ByVal Id_Reg As Integer,
                        ByVal Data_Distinta_Attiva As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli.LeggiProdottiUtilizzati()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT     Movimenti_dettagli.Pro_Cod, Mov_Destinazioni.Qta, Movimenti.Data_Movimento, Formulati.Fr_Des ")

            StrSQL.AppendLine(" FROM         Mov_Destinazioni INNER JOIN ")
            StrSQL.AppendLine("              Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND   ")
            StrSQL.AppendLine("              Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND   ")
            StrSQL.AppendLine("              Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN ")
            StrSQL.AppendLine("              Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND  ")
            StrSQL.AppendLine("              Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN ")
            StrSQL.AppendLine("              Imprese_Progetti ON Mov_Destinazioni.Piva = Imprese_Progetti.Piva AND Mov_Destinazioni.Sa_Cod = Imprese_Progetti.Sa_Cod AND ")
            StrSQL.AppendLine("              Mov_Destinazioni.Appezza = Imprese_Progetti.Appezza AND Mov_Destinazioni.Id_Destinazione = Imprese_Progetti.Id_Reg AND ")
            StrSQL.AppendLine("              Movimenti.Data_Movimento > Imprese_Progetti.Validita_Inizio AND Movimenti.Data_Movimento < Imprese_Progetti.Validita_Fine INNER JOIN ")
            StrSQL.AppendLine("              Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")

            '            StrSQL.AppendLine(" WHERE PDC_Dettagli.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" WHERE  ")
            StrSQL.AppendLine("  (Movimenti_dettagli.Elem_Cod = 191) ")
            StrSQL.AppendLine(" AND (Mov_Destinazioni.Tipo_Destinazione = 0)  ")

            StrSQL.AppendLine(" AND Movimenti.Data_Movimento >Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine(" AND Movimenti.Data_Movimento <Imprese_Progetti.Validita_Fine ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            End If

            If Appezza <> 0 Then
                StrSQL.AppendLine(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza))
            End If

            If Id_Reg <> 0 Then
                StrSQL.AppendLine(" AND Mov_Destinazioni.Id_Destinazione  = " & Agro_SQL_SaveNum(Id_Reg))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY  Data_Movimento")
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


    Public Function LeggiProdottiUtilizzatiFromID_PDC_Testata(
                        ByVal Flag_Distinct As Boolean,
                        ByVal ID_PDC_Testata As Integer,
                        ByVal Data_Distinta_Attiva As Date,
                        ByVal Data_MinimaIntervento As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByRef objParametri As AgronicaCoreParametri
                            ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli.LeggiProdottiUtilizzatiFromID_PDC_Testata()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            If Flag_Distinct Then
                StrSQL.AppendLine(" SELECT   distinct  Movimenti_dettagli.Pro_Cod ")
            Else
                StrSQL.AppendLine(" SELECT     Movimenti_dettagli.Pro_Cod, Mov_Destinazioni.Qta, Movimenti.Data_Movimento, Formulati.Fr_Des, PDC_Dettagli.ID_PDC_Testata, ")
                StrSQL.AppendLine(" Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione as ID_Reg ")
            End If


            StrSQL.AppendLine(" FROM         Mov_Destinazioni INNER JOIN ")
            StrSQL.AppendLine("              Movimenti_dettagli ON Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND  ")
            StrSQL.AppendLine("              Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND  ")
            StrSQL.AppendLine("              Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det INNER JOIN ")
            StrSQL.AppendLine("              Movimenti ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod AND  ")
            StrSQL.AppendLine("              Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov INNER JOIN ")
            StrSQL.AppendLine("              Imprese_Progetti ON Mov_Destinazioni.Piva = Imprese_Progetti.Piva AND Mov_Destinazioni.Sa_Cod = Imprese_Progetti.Sa_Cod AND  ")
            StrSQL.AppendLine("              Mov_Destinazioni.Appezza = Imprese_Progetti.Appezza AND Mov_Destinazioni.Id_Destinazione = Imprese_Progetti.Id_Reg AND  ")
            StrSQL.AppendLine("              Movimenti.Data_Movimento > Imprese_Progetti.Validita_Inizio AND Movimenti.Data_Movimento < Imprese_Progetti.Validita_Fine INNER JOIN ")
            StrSQL.AppendLine("              Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod INNER JOIN ")
            StrSQL.AppendLine("              PDC_Dettagli ON Mov_Destinazioni.Piva = PDC_Dettagli.Piva AND Mov_Destinazioni.Sa_Cod = PDC_Dettagli.Sa_Cod AND  ")
            StrSQL.AppendLine("              Mov_Destinazioni.Appezza = PDC_Dettagli.Appezza And Mov_Destinazioni.Id_Destinazione = PDC_Dettagli.Id_Reg ")


            StrSQL.AppendLine(" WHERE  ")
            StrSQL.AppendLine("  (Movimenti_dettagli.Elem_Cod = 191) ")
            StrSQL.AppendLine(" AND (Mov_Destinazioni.Tipo_Destinazione = 0)  ")

            StrSQL.AppendLine(" AND Movimenti.Data_Movimento >Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine(" AND Movimenti.Data_Movimento <Imprese_Progetti.Validita_Fine ")

            StrSQL.AppendLine(" AND  Imprese_Progetti.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Distinta_Attiva) & "")
            StrSQL.AppendLine(" AND  Imprese_Progetti.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Distinta_Attiva) & "")

            StrSQL.AppendLine(" AND  Movimenti_dettagli.Validita_Inizio >= " & Agro_SQL_SaveDate(Data_MinimaIntervento) & "")



            If ID_PDC_Testata <> 0 Then
                StrSQL.AppendLine(" AND ID_PDC_Testata  = " & Agro_SQL_SaveNum(ID_PDC_Testata))
            End If



            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            If Flag_Distinct <> True Then
                If xOrderBy <> "" Then
                    StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                Else
                    StrSQL.AppendLine(" ORDER BY  Data_Movimento")
                End If
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




    Public Function Leggi_x_mail_risposta_laboratori(ByVal Analisi_testata_cod As Integer,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As DataTable
        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_Dettagli_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Padre_Piva 
        '   Padre_Sa_Cod 
        '   Padre_Appezza
        '   Figlio_Piva 
        '   Figlio_Sa_Cod
        '   Figlio_Appezza 
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            '---------------------------------------------

            StrSQL.AppendLine(" SELECT a.Analisi_Testata_Cod, a.Analisi_Testata_Des, pt.PDC_Testata_Des, pc.PDC_Campione_Des, pc.Codice_Campione, pc.Note_Campione, pd.PDC_Dettagli_Des, isnull( pt.codicestabilimento, '') as codicestabilimento, ")
            StrSQL.AppendLine(" pd.rag_soc, ISNULL(i_padre.rag_soc,'') as rag_soc_padre, pt.id_pdc_testata, ")

            'Grilli: 30/09/2019 Aggiunto pèer Jingold il codice degli Appezzamenti
            StrSQL.AppendLine(" CASE ")
            StrSQL.AppendLine(" WHEN Tipo_Campione = 1 THEN ")
            StrSQL.AppendLine(" (Select SUBSTRING(  ")
            StrSQL.AppendLine(" 	(SELECT ', ' + ISNULL(val_cod,'') FROM PDC_Dettagli detImp ")
            StrSQL.AppendLine(" 	INNER JOIN PDC_Dettagli detLFO ON detImp.ID_LFO=detLFO.ID_LFO  ")
            StrSQL.AppendLine(" 	INNER JOIN Appezzamento_Codici ac ON ac.piva=detLFO.Piva AND ac.sa_cod=detLFO.Sa_Cod AND ac.appezza=detLFO.Appezza ")
            StrSQL.AppendLine(" 	WHERE ac.id_cod=1104 AND detImp.ID_PDC_Testata=pd.ID_PDC_Testata AND detImp.ID_PDC_Dettagli=pd.ID_PDC_Dettagli ")
            StrSQL.AppendLine(" 	ORDER BY val_cod ")
            StrSQL.AppendLine(" 	FOR XML PATH('') ) ")
            StrSQL.AppendLine(" 	, 3 , 9999) ) ")
            StrSQL.AppendLine(" WHEN Tipo_Campione = 0 THEN ")
            StrSQL.AppendLine(" 	(SELECT TOP 1 ISNULL(val_cod,'') FROM Appezzamento_Codici ac ")
            StrSQL.AppendLine(" 	INNER JOIN PDC_Dettagli det ON ac.piva=det.Piva AND ac.sa_cod=det.Sa_Cod AND ac.appezza=det.Appezza  ")
            StrSQL.AppendLine(" 	WHERE ac.id_cod=1104 AND det.ID_PDC_Testata=pd.ID_PDC_Testata AND det.ID_PDC_Dettagli=pd.ID_PDC_Dettagli) ")
            StrSQL.AppendLine(" ELSE ")
            StrSQL.AppendLine(" 	'Errore: Tipo Campione non supportato' ")
            StrSQL.AppendLine(" END AS ListaCodiciApp ")

            StrSQL.AppendLine(" FROM analisi_testata a ")
            StrSQL.AppendLine(" INNER JOIN pdc_analisi pa on pa.Analisi_Testata_Cod = a.Analisi_Testata_Cod ")
            StrSQL.AppendLine(" INNER JOIN PDC_Campioni pc on pc.ID_PDC_Campione = pa.ID_PDC_Campione and pc.ID_PDC_Dettagli = pa.ID_PDC_Dettagli and pc.ID_PDC_Testata = pa.ID_PDC_Testata")
            StrSQL.AppendLine(" INNER JOIN PDC_dettagli pd on pc.ID_PDC_Dettagli = pd.ID_PDC_Dettagli and pc.ID_PDC_Testata = pd.ID_PDC_Testata")
            StrSQL.AppendLine(" INNER JOIN PDC_testata pt on pc.ID_PDC_Testata = pt.ID_PDC_Testata ")
            StrSQL.AppendLine(" LEFT JOIN GerarchiaImprese as ger ON ger.Figlio = pd.Piva and ger.Padre <> '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" LEFT JOIN Imprese as i_padre ON i_padre.Piva = ger.Padre ")



            StrSQL.AppendLine(" WHERE pd.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Analisi_testata_cod <> 0 Then
                StrSQL.AppendLine(" AND a.Analisi_testata_cod= " & Agro_SQL_SaveNum(Analisi_testata_cod))
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



End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class PDC_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider


    Public Function Scrivi(ByVal ID_PDC_Testata As Integer,
                           ByVal ID_PDC_Dettagli As Integer,
                           ByVal PDC_Dettagli_Des As String,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal Id_Reg As Integer,
                           ByVal Veg_Cod As Integer,
                           ByVal Cul_Cod As Integer,
                           ByVal ID_LFO As Integer,
                           ByVal Flag_PDC As Integer,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Cod_Animale As Integer = 0,
                           Optional ByVal Cod_Progetto As Integer = 0,
                           Optional ByVal GEN_COD As Integer = 0,
                            Optional ByVal SPE_COD As Integer = 0,
                            Optional ByVal RAZ_COD As Integer = 0,
                            Optional ByVal Data_Nascita As Date = AGRODATAINIZIO,
                            Optional ByVal Sesso As String = "",
                            Optional ByVal Matricola As String = "",
                            Optional ByVal Lotto As String = "",
                            Optional ByVal Raggruppamento_Cod As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_W.Scrivi_Dettagli()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_Dettagli(PivaSuperUser , ID_PDC_Testata, ID_PDC_Dettagli, ")
            StrSQL.AppendLine("         PDC_Dettagli_Des, Piva, Sa_Cod, ")
            StrSQL.AppendLine("         Appezza, Id_Reg, Veg_Cod, Cul_Cod, ID_LFO, Flag_PDC,  ")
            StrSQL.AppendLine("         Data_Creazione, Data_Modifica, ")
            StrSQL.AppendLine("         Username_Creazione, Username_Modifica ")
            StrSQL.AppendLine(", Cod_Animale, Cod_Progetto, GEN_COD, SPE_COD, RAZ_COD, Data_Nascita, Sesso, Matricola, Lotto, Raggruppamento_Cod ")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PDC_Dettagli_Des) & "'")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Appezza))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Reg))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cul_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_LFO))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_PDC))
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")

            StrSQL.AppendLine("   , " & Agro_SQL_SaveNum(Cod_Animale) & "   ")
            StrSQL.AppendLine("   , " & Agro_SQL_SaveNum(Cod_Progetto) & "   ")
            StrSQL.AppendLine("   , " & Agro_SQL_SaveNum(GEN_COD) & "   ")
            StrSQL.AppendLine("   , " & Agro_SQL_SaveNum(SPE_COD) & "   ")
            StrSQL.AppendLine("   , " & Agro_SQL_SaveNum(RAZ_COD) & "  ")
            StrSQL.AppendLine("   , " & Agro_SQL_SaveDate(Data_Nascita) & "  ")
            StrSQL.AppendLine("   , '" & Agro_SQL_SaveText(Sesso) & "'   ")
            StrSQL.AppendLine("   , '" & Agro_SQL_SaveText(Matricola) & "'   ")
            StrSQL.AppendLine("   , '" & Agro_SQL_SaveText(Lotto) & "'   ")
            StrSQL.AppendLine("   , " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "   ")

            StrSQL.AppendLine(")")

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

    Public Function Scrivi_x_Acquisti(ByVal ID_PDC_Testata As Integer,
                                      ByVal ID_PDC_Dettagli As Integer,
                                      ByVal PDC_Dettagli_Des As String,
                                      ByVal Data_Fornitura As String,
                                      ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Appezza As Integer,
                                      ByVal ID_Reg As Integer,
                                      ByVal Rag_Soc As String,
                                      ByVal CodiceFornitore As String,
                                      ByVal Veg_Cod As Integer,
                                      ByVal Cul_Cod As Integer,
                                      ByVal Grva_Cod As Integer,
                                      ByVal ID_LFO As Integer,
                                      ByVal Mat_Cod As Integer,
                                      ByVal CodiceArticolo As String,
                                      ByVal NomeArticolo As String,
                                      ByVal LottoFornitore As String,
                                      ByVal TipoLotta As String,
                                      ByVal PuntoPrelievo As String,
                                      ByVal Disciplinare_cod As String,
                                      ByVal Note_Impianto As String,
                                      ByVal Documentazione As String,
                                      ByVal Note_Documentazione As String,
                                      ByVal Flag_PDC As Integer,
                                      ByVal Fornitore_Azienda As String,
                                      ByVal Tipo_Lotta_Acquisti As String,
                                      ByVal fornitore_2 As String,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal sa_nome As String = "",
                                      Optional ByVal App_Nome As String = "",
                                      Optional ByVal Sup_Imp As Decimal = Nothing,
                                      Optional ByVal CapitolatoPrivato As String = "",
                                      Optional ByVal Certificato As String = "",
                                      Optional ByVal Data_semina As DateTime = AGRODATAINIZIO,
                                      Optional ByVal Data_Raccolta As DateTime = AGRODATAFINE,
                                      Optional ByVal Descrizione As String = "",
                                      Optional ByVal Regolamento As String = "",
                                      Optional ByVal Note2 As String = "",
                                      Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                                      Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                                      Optional ByVal username_creazione As String = "",
                                      Optional ByVal username_modifica As String = ""
                                      ) As Boolean

        Dim NomeRoutine As String = "PianiCampionamentoDAL.PDC_W.Scrivi_Dettagli()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False


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

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO PDC_Dettagli(PivaSuperUser , ID_PDC_Testata, ID_PDC_Dettagli, ")
            StrSQL.AppendLine("         PDC_Dettagli_Des, ")

            If Data_Fornitura <> "" Then
                StrSQL.AppendLine("         Data_Fornitura, ")
            End If

            StrSQL.AppendLine("         Piva, Rag_Soc, ")

            StrSQL.AppendLine("         Sa_Cod, ")
            StrSQL.AppendLine("         Appezza, ")
            StrSQL.AppendLine("         Id_Reg, ")

            If CodiceFornitore <> "" Then
                StrSQL.AppendLine("         CodiceFornitore, ")
            End If
            StrSQL.AppendLine("         Veg_Cod, Cul_Cod, ID_LFO, Flag_PDC,  ")

            If Grva_Cod <> 0 Then
                StrSQL.AppendLine("         Grva_Cod,   ")
            End If


            If Mat_Cod <> 0 Then
                StrSQL.AppendLine("         Mat_Cod, ")
            End If
            If CodiceArticolo <> "" Then
                StrSQL.AppendLine("         CodiceArticolo, ")
            End If
            If NomeArticolo <> "" Then
                StrSQL.AppendLine("         NomeArticolo, ")
            End If


            If LottoFornitore <> "" Then
                StrSQL.AppendLine("         LottoFornitore, ")
            End If
            If TipoLotta <> "" Then
                StrSQL.AppendLine("         TipoLotta, ")
            End If
            If PuntoPrelievo <> "" Then
                StrSQL.AppendLine("         PuntoPrelievo, ")
            End If

            If Disciplinare_cod <> "" Then
                StrSQL.AppendLine("         Disciplinare_cod, ")
            End If




            If Note_Impianto <> "" Then
                StrSQL.AppendLine("         Note_Impianto, ")
            End If

            If fornitore_2 <> "" Then
                StrSQL.AppendLine("         fornitore_2, ")
            End If



            StrSQL.AppendLine("         Documentazione, ")
            StrSQL.AppendLine("         Note_Documentazione, ")

            If Fornitore_Azienda <> "" Then
                StrSQL.AppendLine("         Fornitore_Azienda, ")
            End If

            'opzonali
            If sa_nome <> "" Then
                StrSQL.AppendLine("         sa_nome, ")
            End If

            If App_Nome <> "" Then
                StrSQL.AppendLine("         app_nome, ")
            End If

            If Sup_Imp <> 0 Then
                StrSQL.AppendLine("         Sup_Imp, ")
            End If

            If CapitolatoPrivato <> "" Then
                StrSQL.AppendLine("         CapitolatoPrivato, ")
            End If

            If Certificato <> "" Then
                StrSQL.AppendLine("      Certificato   , ")
            End If

            If Data_semina <> AGRODATAINIZIO Then
                StrSQL.AppendLine("      Data_semina   , ")
            End If

            If Data_Raccolta <> AGRODATAFINE Then
                StrSQL.AppendLine("      Data_Raccolta   , ")
            End If

            If Descrizione <> "" Then
                StrSQL.AppendLine("      Descrizione   , ")
            End If

            If Regolamento <> "" Then
                StrSQL.AppendLine("      Regolamento   , ")
            End If

            If Note2 <> "" Then
                StrSQL.AppendLine("      Note2   , ")
            End If

            If Tipo_Lotta_Acquisti <> "" Then
                StrSQL.AppendLine("      Tipo_Lotta_Acquisti   , ")
            End If

            'fine opzionali

            StrSQL.AppendLine("         Data_Creazione, Data_Modifica, ")
            StrSQL.AppendLine("         Username_Creazione, Username_Modifica ")
            StrSQL.AppendLine("  ) ")

            StrSQL.AppendLine("  VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Testata))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PDC_Dettagli_Des) & "'")
            If Data_Fornitura <> "" Then
                StrSQL.AppendLine("         ," & Agro_SQL_SaveDate(Data_Fornitura) & "")
            End If
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva.Trim) & "'")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Rag_Soc) & "'")

            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(Appezza))
            StrSQL.AppendLine("         ," & Agro_SQL_SaveNum(ID_Reg))

            If CodiceFornitore <> "" Then
                StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(CodiceFornitore) & "'")
            End If

            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Veg_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Cul_Cod))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_LFO))
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Flag_PDC))

            If Grva_Cod <> 0 Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Grva_Cod))
            End If



            If Mat_Cod <> 0 Then
                StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Mat_Cod))
            End If
            If CodiceArticolo <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(CodiceArticolo) & "'")
            End If
            If NomeArticolo <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(NomeArticolo) & "'")
            End If




            If LottoFornitore <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(LottoFornitore) & "'")
            End If
            If TipoLotta <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(TipoLotta) & "'")
            End If
            If PuntoPrelievo <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(PuntoPrelievo) & "'")
            End If

            If Disciplinare_cod <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Disciplinare_cod) & "'")
            End If

            If Note_Impianto <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Note_Impianto) & "'")
            End If

            If fornitore_2 <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(fornitore_2) & "'")
            End If

            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Documentazione) & "'")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Note_Documentazione) & "'")

            If Fornitore_Azienda <> "" Then
                StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Fornitore_Azienda) & "'")
            End If



            'opzonali
            If sa_nome <> "" Then
                StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(sa_nome) & "'")
            End If

            If App_Nome <> "" Then
                StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(App_Nome) & "'")
            End If

            If Sup_Imp <> 0 Then
                StrSQL.AppendLine(" , " & Agro_SQL_SaveNum(Sup_Imp))
            End If

            If CapitolatoPrivato <> "" Then
                StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(CapitolatoPrivato) & "'")
            End If

            If Certificato <> "" Then
                StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(Certificato) & "'")
            End If

            If Data_semina <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" , " & Agro_SQL_SaveDateTime(Data_semina) & " ")
            End If

            If Data_Raccolta <> AGRODATAFINE Then
                StrSQL.AppendLine(" , " & Agro_SQL_SaveDateTime(Data_Raccolta) & " ")
            End If

            If Descrizione <> "" Then
                StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(Descrizione) & "' ")
            End If

            If Regolamento <> "" Then
                StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(Regolamento) & "' ")
            End If

            If Note2 <> "" Then
                StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(Note2) & "' ")
            End If

            If Tipo_Lotta_Acquisti <> "" Then
                StrSQL.AppendLine(" , '" & Agro_SQL_SaveText(Tipo_Lotta_Acquisti) & "' ")
            End If
            'fine opzionali

            StrSQL.AppendLine("         ," & Agro_SQL_SaveDate(Data_creazione) & " ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDate(Data_modifica) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine(")")

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


#Region "Modifica"

    Public Function Update_da_Anagrafica(ByVal ID_PDC_Testata As Integer,
                                         ByRef objParametri As AgronicaCoreParametri,
                                         Optional ByVal ID_PDC_Dettagli As String = Nothing
                                         ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_W.Update_da_Anagrafica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" update    pdc_d ")
            StrSQL.AppendLine("     set pdc_d.rag_soc =  Imprese.rag_soc, ")
            StrSQL.AppendLine("     pdc_d.cul_cod = Reg_Impianti.cul_cod, ")
            StrSQL.AppendLine("     pdc_d.sa_nome = Centri_Aziendali.sa_nome,  ")
            StrSQL.AppendLine("     pdc_d.app_nome = Appezzamento.APP_NOME,  ")
            StrSQL.AppendLine("     pdc_d.sup_imp = Reg_Impianti.Sup_Imp, ")

            StrSQL.AppendLine("     --data raccolta ")
            StrSQL.AppendLine("     pdc_d.data_raccolta =  ISNULL ( (SELECT top 1 CONVERT(VARCHAR,Movimenti.Data_Movimento, 103) FROM Agenda INNER JOIN  Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda  ")
            StrSQL.AppendLine("         INNER JOIN  Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND  Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            StrSQL.AppendLine("         INNER JOIN  Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND  Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod And Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda And Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov And Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            StrSQL.AppendLine("     WHERE(Agenda.Lav_Cod = 125) AND Movimenti.Cau_Mov = '2200'  AND Mov_Destinazioni.Piva = Reg_Impianti.PIVA  AND  Mov_Destinazioni.sa_cod = Reg_Impianti.sa_cod  And Mov_Destinazioni.appezza = Reg_Impianti.APPEZZA  And Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG  AND Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio   AND Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine  order by Movimenti.Data_Movimento asc)")
            StrSQL.AppendLine("         ,   		ISNULL (Imprese_Progetti.Data_Fine_Prevista, '31/12/2100')  )  , ")


            StrSQL.AppendLine("     --data Semina ")
            StrSQL.AppendLine("    pdc_d.data_semina = ISNULL ( ")
            StrSQL.AppendLine("     (SELECT  top 1  CONVERT(VARCHAR, Movimenti.Data_Movimento, 103)  FROM Agenda INNER JOIN      Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod  AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            StrSQL.AppendLine("         INNER JOIN      Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            StrSQL.AppendLine("         INNER JOIN     Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod And Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda And Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov And Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det  ")
            StrSQL.AppendLine("     WHERE(Agenda.Lav_Cod = 2) AND ( cau_mov = '2300') AND Mov_Destinazioni.Piva = Reg_Impianti.PIVA  AND Mov_Destinazioni.sa_cod = Reg_Impianti.sa_cod And Mov_Destinazioni.appezza = Reg_Impianti.APPEZZA  And Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG AND Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio  AND Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine order by Movimenti.Data_Movimento desc) ")
            StrSQL.AppendLine("         , ISNULL (Imprese_Progetti.Data_Inizio_Prevista, '01/01/1900'))  , ")

            StrSQL.AppendLine("     --Codice Fornitore ")
            StrSQL.AppendLine("     pdc_d.CodiceFornitore =      ISNULL((select Imprese_Codici.val_cod from Imprese_Codici where pdc_d.Piva = Imprese_Codici.PIVA AND Imprese_Codici.id_cod = 1033), '')  ,  ")

            StrSQL.AppendLine("     --Dettaglio_Specie_Personalizzato ")
            StrSQL.AppendLine("     pdc_d.Dettaglio_Specie_Personalizzato = isnull((select infoAgg_Des from Reg_Impianti_Codici  inner join CAC_Codifica_InfoAggiuntive on Reg_Impianti_Codici.val_cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod where piva = pdc_d.piva and sa_cod = pdc_d.sa_cod and appezza = pdc_d.appezza and id_reg = pdc_d.id_reg and id_cod = 1108 ),'') ,        ")


            StrSQL.AppendLine("     --x_terremerse ")
            StrSQL.AppendLine("     pdc_d.Codice_Cliente_2  =    ISNULL((select Imprese_Codici.val_cod from Imprese_Codici where pdc_d.Piva = Imprese_Codici.PIVA AND Imprese_Codici.id_cod = 1091), '')  ,  ")
            StrSQL.AppendLine("     pdc_d.Data_Inizio_Impianto   =   CONVERT(VARCHAR,Reg_Impianti.Validita_Inizio , 103) , ")
            StrSQL.AppendLine("     pdc_d.Sigla  =    isnull((select top 1 pro_cod  from CentrixIndirizzi inner join Indirizzi on Indirizzi.cod_indirizzo = CentrixIndirizzi.cod_indirizzo where CentrixIndirizzi.piva = pdc_d.piva and  CentrixIndirizzi.sa_cod  = pdc_d.sa_cod) , '' ) , ")
            StrSQL.AppendLine("     pdc_d.Tecnico_di_riferimento  = ISNULL(( select top 1 Contatti.Rag_Soc + ' ' + Contatti.Nome + ' '+  Contatti.Cognome from Imprese_Codici inner join Contatti on Contatti.Cod_Contatto =Imprese_Codici.val_cod where pdc_d.Piva = Imprese_Codici.PIVA AND Imprese_Codici.id_cod = 1010), '')  ,  ")



            StrSQL.AppendLine("     ---Capitolato Privato ")
            StrSQL.AppendLine("     pdc_d.CapitolatoPrivato =       ISNULL((SELECT  top 1   CAC_Codifica_InfoAggiuntive.InfoAgg_Des  ")
            StrSQL.AppendLine("             FROM Reg_Impianti ")
            StrSQL.AppendLine("             INNER JOIN Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza AND Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg ")
            StrSQL.AppendLine("             INNER JOIN CAC_Codifica_InfoAggiuntive ON Reg_Impianti_Codici.val_cod = CAC_Codifica_InfoAggiuntive.InfoAgg_Cod ")
            StrSQL.AppendLine("             INNER JOIN Imprese_Progetti ON Reg_Impianti_Codici.PIVA = Imprese_Progetti.Piva AND Reg_Impianti_Codici.sa_cod = Imprese_Progetti.Sa_Cod AND  Reg_Impianti_Codici.appezza = Imprese_Progetti.Appezza And Reg_Impianti_Codici.Id_Reg = Imprese_Progetti.Id_Reg And Reg_Impianti_Codici.Progetto_Cod = Imprese_Progetti.Progetto_Cod ")
            StrSQL.AppendLine("             where pdc_d.Piva = Reg_Impianti.PIVA And pdc_d.Sa_Cod = Reg_Impianti.Sa_Cod And pdc_d.Appezza = Reg_Impianti.Appezza And pdc_d.id_reg = Reg_Impianti.id_reg AND Reg_Impianti_Codici.id_cod = 1093   ")
            StrSQL.AppendLine("             AND Imprese_Progetti.Validita_Inizio <= PDC_Testata.PDC_Data_Istantanea  AND Imprese_Progetti.Validita_Fine >= PDC_Testata.PDC_Data_Istantanea ")
            StrSQL.AppendLine("             ), '')   ,  ")


            StrSQL.AppendLine("     --Regolamento ")
            StrSQL.AppendLine("     pdc_d.Regolamento = CASE WHEN Regolamenti.Reg_Des IS NULL THEN ''   ")
            StrSQL.AppendLine("              WHEN Regolamenti.Reg_Des= 'Nessuno' THEN ''   ")
            StrSQL.AppendLine("              WHEN Regolamenti.Reg_Des ='Reg. CE 834/07 (Ex.Reg. CE 2092/91)' THEN 'BIO'   ")
            StrSQL.AppendLine("              ELSE Regolamenti.Reg_Des   ")
            StrSQL.AppendLine("              End , ")

            StrSQL.AppendLine("     --Regolamento_cod ")
            StrSQL.AppendLine("     pdc_d.Regolamento_Cod = ISNULL(Regolamenti.Reg_Cod , 1 ) , ")


            StrSQL.AppendLine("     -- Magazzino_di_conferimento   ")
            StrSQL.AppendLine("      pdc_d.Magazzino_di_conferimento =     ")

            '06/11/2017: modificata by maga x adeguamento a nuovo salvataggio del magazzino conferimento su impianto
            'StrSQL.AppendLine("      ISNULL ((select Fabbricato_Des  from Fabbricati inner join ( ")
            'StrSQL.AppendLine("      select right(val_cod, CHARINDEX('|', val_cod) - 1) as sa_cod, LEFT(val_cod, CHARINDEX('|', val_cod) - 1) as fabbricato_cod , val_cod from Reg_Impianti_Codici ")
            'StrSQL.AppendLine("      INNER JOIN Imprese_Progetti ON Reg_Impianti_Codici.PIVA = Imprese_Progetti.Piva AND Reg_Impianti_Codici.sa_cod = Imprese_Progetti.Sa_Cod ")
            'StrSQL.AppendLine("      AND  Reg_Impianti_Codici.appezza = Imprese_Progetti.Appezza And Reg_Impianti_Codici.Id_Reg = Imprese_Progetti.Id_Reg And Reg_Impianti_Codici.Progetto_Cod = Imprese_Progetti.Progetto_Cod AND Reg_Impianti_Codici.id_cod = 1122 ")
            'StrSQL.AppendLine("      where(pdc_d.Piva = Reg_Impianti_Codici.PIVA And pdc_d.Sa_Cod = Reg_Impianti_Codici.Sa_Cod And pdc_d.Appezza = Reg_Impianti_Codici.Appezza And pdc_d.id_reg = Reg_Impianti_Codici.id_reg And Imprese_Progetti.Validita_Inizio <= PDC_Testata.PDC_Data_Istantanea And Imprese_Progetti.Validita_Fine >= PDC_Testata.PDC_Data_Istantanea And Reg_Impianti_Codici.progetto_cod = imprese_progetti.Progetto_Cod) ")
            'StrSQL.AppendLine("      ) a on fabbricati.Fabbricato_Cod = a.fabbricato_cod and Fabbricati.sa_cod = a.sa_cod  ")
            'StrSQL.AppendLine("      where  piva = pdc_d.pivasuperuser),'' ) , ")
            StrSQL.AppendLine("      ISNULL ( ")
            StrSQL.AppendLine("             (SELECT Fabbricato_Des ")
            StrSQL.AppendLine("             FROM Fabbricati  ")
            StrSQL.AppendLine("             INNER JOIN Reg_Impianti_Codici ")
            StrSQL.AppendLine("             ON CONVERT(varchar(50),Fabbricati.Fabbricato_Cod) +'|'+ CONVERT(varchar(50),FABBRICATI.sa_COD) +'|'+ Fabbricati.piva = Reg_Impianti_Codici.val_cod ")
            StrSQL.AppendLine("             INNER JOIN Imprese_Progetti ON Reg_Impianti_Codici.PIVA = Imprese_Progetti.Piva ")
            StrSQL.AppendLine("             AND Reg_Impianti_Codici.sa_cod = Imprese_Progetti.Sa_Cod ")
            StrSQL.AppendLine("             AND  Reg_Impianti_Codici.appezza = Imprese_Progetti.Appezza ")
            StrSQL.AppendLine("             And Reg_Impianti_Codici.Id_Reg = Imprese_Progetti.Id_Reg  ")
            StrSQL.AppendLine("             And Reg_Impianti_Codici.Progetto_Cod = Imprese_Progetti.Progetto_Cod  ")
            StrSQL.AppendLine("             AND Reg_Impianti_Codici.id_cod = 1122 ")
            StrSQL.AppendLine("             where(pdc_d.Piva = Reg_Impianti_Codici.PIVA ")
            StrSQL.AppendLine("             And pdc_d.Sa_Cod = Reg_Impianti_Codici.Sa_Cod ")
            StrSQL.AppendLine("             And pdc_d.Appezza = Reg_Impianti_Codici.Appezza ")
            StrSQL.AppendLine("             And pdc_d.id_reg = Reg_Impianti_Codici.id_reg ")
            StrSQL.AppendLine("             And Imprese_Progetti.Validita_Inizio <= PDC_Testata.PDC_Data_Istantanea ")
            StrSQL.AppendLine("             And Imprese_Progetti.Validita_Fine >= PDC_Testata.PDC_Data_Istantanea ")
            StrSQL.AppendLine("             And Reg_Impianti_Codici.progetto_cod = imprese_progetti.Progetto_Cod) ")
            StrSQL.AppendLine("             )	,'' ) , ")

            StrSQL.AppendLine("     --Certificato ")
            StrSQL.AppendLine("     pdc_d.Certificato =       ISNULL((SELECT Reg_Impianti_Codici.val_cod FROM         Reg_Impianti  INNER JOIN Reg_Impianti_Codici ON Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA AND Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod AND  Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza And Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg where pdc_d.Piva = Reg_Impianti.PIVA  And pdc_d.Sa_Cod = Reg_Impianti.Sa_Cod  And      pdc_d.Appezza = Reg_Impianti.Appezza And pdc_d.id_reg = Reg_Impianti.id_reg AND Reg_Impianti_Codici.id_cod = 1130    ), ''),  ")

            StrSQL.AppendLine("     --Disciplinare cod ")
            StrSQL.AppendLine("     pdc_d.Disciplinare_Cod =      CONVERT(VARCHAR,  CONVERT(VARCHAR,Imprese_Progetti.Disciplinare_Cod) + '/' + CONVERT(VARCHAR,Imprese_Progetti.disciplinare_pubblicoPrivato )) ,     ")

            StrSQL.AppendLine("     --tipologia varietale  ")
            StrSQL.AppendLine("     pdc_d.grva_cod =  ISNULL(Reg_Impianti.grva_cod_veg ,'') ,    ")


            StrSQL.AppendLine("     --Tipo lotta")
            StrSQL.AppendLine("     pdc_d.tipolotta =      ISNULL((select DPI_Regolamenti.nomeesteso from DPI_Regolamenti where Imprese_Progetti.Disciplinare_Cod =DPI_Regolamenti.cod_regolamento and DPI_Regolamenti.flag_privato_pubblico = Imprese_Progetti.disciplinare_pubblicoPrivato),'')      ")

            If Not String.IsNullOrEmpty(ID_PDC_Dettagli) Then
                StrSQL.AppendLine("     --Nuovi campi")
                StrSQL.AppendLine("     ,pdc_d.Piva_OP = i.piva, pdc_d.Rag_Soc_OP = i.Rag_Soc, pdc_d.kpin = ric.val_cod, pdc_d.block = ric2.val_cod, pdc_d.grower_number = ric3.val_cod, ")
                'StrSQL.AppendLine("     pdc_d.grower_number = ISNULL((select Imprese_Codici.val_cod from Imprese_Codici where pdc_d.Piva = Imprese_Codici.PIVA AND Imprese_Codici.id_cod = 1317), ''),")
                StrSQL.AppendLine("     pdc_d.Ind_Des = ai.Ind_Des, pdc_d.Frz_Des = ai.Frz_Des, pdc_d.CAP = ai.CAP, pdc_d.Com_Des = ai.Com_Des,  ")
                StrSQL.AppendLine("     pdc_d.Pro_Cod = lp.Sigla, pdc_d.Reg = lp.REG, pdc_d.Stato = ai.Stato,  ")
                StrSQL.AppendLine("     pdc_d.Pro_Cod_Istat = ai.Pro_Cod_Istat, pdc_d.Com_Cod_Istat = ai.Com_Cod_Istat,  ")
                StrSQL.AppendLine("     pdc_d.Tecnico_Campionamento = LTRIM(RTRIM(c.Nome+' '+c.cognome)), pdc_d.Tecnico_Campionamento_Tel = r.numero  ")
            End If

            StrSQL.AppendLine("     --Lat e Long Impianto")
            StrSQL.AppendLine("     , pdc_d.App_Lat = ISNULL(Appezzamento.X, 0), ")
            StrSQL.AppendLine("       pdc_d.App_Long = ISNULL(Appezzamento.Y, 0) ")

            StrSQL.AppendLine("     --Progetto Nome")
            StrSQL.AppendLine("     , pdc_d.Progetto_Nome = ISNULL(Imprese_Progetti.Progetto_Nome, '') ")

            StrSQL.AppendLine("   FROM        PDC_Dettagli AS pdc_d ")
            StrSQL.AppendLine("             INNER JOIN PDC_Testata on PDC_Testata.Id_PDC_Testata  = pdc_d.Id_PDC_Testata ")
            StrSQL.AppendLine("             INNER JOIN  Imprese ON Imprese.PIVA = pdc_d.Piva  ")
            StrSQL.AppendLine("             INNER JOIN Centri_Aziendali ON Centri_Aziendali.PIVA = pdc_d.Piva AND Centri_Aziendali.Sa_Cod = pdc_d.Sa_Cod   ")
            StrSQL.AppendLine("             INNER JOIN Appezzamento ON Appezzamento.PIVA = pdc_d.Piva AND Appezzamento.Appezza = pdc_d.Appezza AND Appezzamento.Sa_Cod = pdc_d.Sa_Cod ")
            StrSQL.AppendLine("             INNER JOIN Reg_Impianti ON Reg_Impianti.PIVA = pdc_d.Piva AND Reg_Impianti.Appezza = pdc_d.Appezza AND Reg_Impianti.Sa_Cod = pdc_d.Sa_Cod AND Reg_Impianti.Id_Reg = pdc_d.Id_Reg  ")
            StrSQL.AppendLine("             INNER JOIN Cultivar ON pdc_d.CUL_COD = Cultivar.Cul_Cod  ")
            StrSQL.AppendLine("             INNER JOIN SpecieVegetali ON SpecieVegetali.Veg_Cod = pdc_d.Veg_Cod ")
            StrSQL.AppendLine("             LEFT JOIN PDC_LFO ON PDC_LFO.ID_LFO  = pdc_d.ID_LFO ")
            StrSQL.AppendLine("             LEFT JOIN Imprese_Progetti ON pdc_d.Piva = Imprese_Progetti.Piva AND pdc_d.Sa_Cod = Imprese_Progetti.Sa_Cod AND pdc_d.Appezza = Imprese_Progetti.Appezza And pdc_d.Id_Reg = Imprese_Progetti.Id_Reg ")
            StrSQL.AppendLine("             LEFT JOIN Regolamenti ON Imprese_Progetti.Regolamento_Cod = Regolamenti.Reg_Cod ")

            If Not String.IsNullOrEmpty(ID_PDC_Dettagli) Then
                StrSQL.AppendLine("             LEFT JOIN GerarchiaImprese gi on gi.Figlio=pdc_d.Piva LEFT JOIN Imprese i on i.piva=gi.Padre LEFT JOIN Imprese_Codici ic on ic.PIVA=pdc_d.Piva and ic.id_cod=1088 ")
                StrSQL.AppendLine("             LEFT JOIN Reg_Impianti_Codici ric on ric.piva=pdc_d.Piva and ric.sa_cod = pdc_d.Sa_Cod and ric.appezza = pdc_d.Appezza and ric.Id_Reg=pdc_d.Id_Reg and ric.id_cod = 1287 ")
                StrSQL.AppendLine("             LEFT JOIN Reg_Impianti_Codici ric2 on ric2.piva=pdc_d.Piva and ric2.sa_cod = pdc_d.Sa_Cod and ric2.appezza = pdc_d.Appezza and ric2.Id_Reg=pdc_d.Id_Reg and ric2.id_cod = 1288 ")
                StrSQL.AppendLine("             LEFT JOIN Reg_Impianti_Codici ric3 on ric3.piva=pdc_d.Piva and ric3.sa_cod = pdc_d.Sa_Cod and ric3.appezza = pdc_d.Appezza and ric3.Id_Reg=pdc_d.Id_Reg and ric3.id_cod = 1317 ")
                StrSQL.AppendLine("             LEFT JOIN Contatti c on c.Cod_Contatto=ic.val_cod and c.sa_cod=-1 LEFT JOIN ContattiXRubrica cr on cr.Piva=c.Piva and cr.Cod_Contatto=c.Cod_Contatto LEFT JOIN Rubrica r on r.cod_rubrica = cr.Cod_Rubrica ")
                StrSQL.AppendLine("             LEFT JOIN AppezzamentixIndirizzi axi on axi.PIVA=pdc_d.Piva and axi.sa_cod=pdc_d.Sa_Cod and axi.appezza = pdc_d.Appezza LEFT JOIN Indirizzi ai on axi.cod_indirizzo=ai.cod_indirizzo ")
                StrSQL.AppendLine("             LEFT JOIN Lista_Province lp on lp.PROV=ai.pro_cod_istat LEFT JOIN Lista_Regioni lr on lr.REG=lp.REG ")
            End If

            StrSQL.AppendLine(" WHERE pdc_d.PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   pdc_d.ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND Imprese_Progetti.Validita_Inizio <= PDC_Testata.PDC_Data_Istantanea  AND Imprese_Progetti.Validita_Fine >= PDC_Testata.PDC_Data_Istantanea  ")

            ' filtro solo i dettagli passati
            If Not String.IsNullOrEmpty(ID_PDC_Dettagli) Then
                StrSQL.AppendLine(" AND pdc_d.ID_PDC_Dettagli IN (" & Agro_SQL_Save_Clausola_IN(ID_PDC_Dettagli) & ") ")
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

    Public Function Update_da_Acquisti(ByVal Id_PDC_Testata As Integer,
                                       ByVal ID_PDC_Dettagli As Integer,
                                       ByVal Cod_Articolo As String,
                                       ByVal NomeArticolo As String,
                                       ByVal Rag_Soc As String,
                                       ByVal CodiceFornitore As String,
                                       ByVal TipoLotta As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_W.Update_da_Acquisti()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------
        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" update    pdc_dettagli ")
            StrSQL.AppendLine("     set ")

            If Cod_Articolo <> "" Then
                StrSQL.AppendLine("     CodiceArticolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "',")
            End If

            If NomeArticolo <> "" Then
                StrSQL.AppendLine("     NomeArticolo = '" & Agro_SQL_SaveText(NomeArticolo) & "',")
            End If

            If Rag_Soc <> "" Then
                StrSQL.AppendLine("     Rag_Soc = '" & Agro_SQL_SaveText(Rag_Soc) & "',")
            End If

            If CodiceFornitore <> "" Then
                StrSQL.AppendLine("     CodiceFornitore = '" & Agro_SQL_SaveText(CodiceFornitore) & "',")
            End If

            If TipoLotta <> "" Then
                StrSQL.AppendLine("     TipoLotta = '" & Agro_SQL_SaveText(TipoLotta) & "',")
            End If

            Dim strs As String = StrSQL.ToString
            strs = strs.Trim
            StrSQL.Length = 0
            StrSQL.AppendLine(strs.Substring(0, strs.Length - 1))


            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(Id_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")


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

    Public Function LFO_Unico(ByVal ID_PDC_Testata As Integer,
                              ByVal ID_LFO As Integer,
                              ByRef objParametri As AgronicaCoreParametri
                              ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_W.LFO_Unico()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   ID_LFO   =  " & Agro_SQL_SaveNum(ID_LFO) & "   ")

            StrSQL.AppendLine("   ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now) & "   ")
            StrSQL.AppendLine("   ,Username_Modifica   =  '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_LFO    =  0 ")

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

    Public Function Modifica(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_PDC_Dettagli As Integer,
                             ByVal PDC_Dettagli_Des As String,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Reg As Integer,
                             ByVal Veg_Cod As Integer,
                             ByVal Cul_Cod As Integer,
                             ByVal ID_LFO As Integer,
                             ByVal Flag_PDC As Integer,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   PDC_Dettagli_Des   =  '" & Agro_SQL_SaveText(PDC_Dettagli_Des) & "'   ")
            StrSQL.AppendLine("   ,Piva   =  '" & Agro_SQL_SaveText(Piva) & "'   ")
            StrSQL.AppendLine("   ,Sa_Cod   =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            StrSQL.AppendLine("   ,Appezza   =  " & Agro_SQL_SaveNum(Appezza) & "   ")
            StrSQL.AppendLine("   ,Id_Reg   =  " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            StrSQL.AppendLine("   ,Veg_Cod   =  " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            StrSQL.AppendLine("   ,Cul_Cod   =  " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
            StrSQL.AppendLine("   ,ID_LFO   =  " & Agro_SQL_SaveNum(ID_LFO) & "   ")

            StrSQL.AppendLine("   ,Flag_PDC   =  " & Agro_SQL_SaveNum(Flag_PDC) & "   ")
            StrSQL.AppendLine("   ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now) & "   ")
            StrSQL.AppendLine("   ,Username_Modifica   =  '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")

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

    Public Function Modifica_x_Acquisti(ByVal ID_PDC_Testata As Integer,
                                        ByVal ID_PDC_Dettagli As Integer,
                                        ByVal PDC_Dettagli_Des As String,
                                        ByVal Data_Fornitura As String,
                                        ByVal Piva As String,
                                        ByVal Rag_Soc As String,
                                        ByVal CodiceFornitore As String,
                                        ByVal Veg_Cod As Integer,
                                        ByVal Cul_Cod As Integer,
                                        ByVal Grva_Cod As Integer,
                                        ByVal Mat_Cod As Integer,
                                        ByVal CodiceArticolo As String,
                                        ByVal NomeArticolo As String,
                                        ByVal LottoFornitore As String,
                                        ByVal TipoLotta As String,
                                        ByVal PuntoPrelievo As String,
                                        ByVal Disciplinare_cod As String,
                                        ByVal Note_Impianto As String,
                                        ByVal Documentazione As String,
                                        ByVal Note_Documentazione As String,
                                        ByVal Fornitore_Azienda As String,
                                        ByVal Tipo_Lotta_Acquisti As String,
                                        ByVal fornitore_2 As String,
                                        ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_W.Modifica_x_Acquisti()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   PDC_Dettagli_Des   =  '" & Agro_SQL_SaveText(PDC_Dettagli_Des) & "'   ")
            StrSQL.AppendLine("   ,Piva   =  '" & Agro_SQL_SaveText(Piva) & "'   ")
            StrSQL.AppendLine("   ,Rag_Soc   =  '" & Agro_SQL_SaveText(Rag_Soc) & "'   ")
            StrSQL.AppendLine("   ,CodiceFornitore   =  '" & Agro_SQL_SaveText(CodiceFornitore) & "'   ")
            StrSQL.AppendLine("   ,Veg_Cod   =  " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
            StrSQL.AppendLine("   ,Cul_Cod   =  " & Agro_SQL_SaveNum(Cul_Cod) & "   ")
            StrSQL.AppendLine("   ,Grva_Cod   =  " & Agro_SQL_SaveNum(Grva_Cod) & "   ")
            'If Data_Fornitura <> "" Then
            StrSQL.AppendLine("   ,Data_Fornitura   =  " & Agro_SQL_SaveDate(Data_Fornitura) & "   ")
            'End If
            'If CodiceArticolo <> "" Then
            StrSQL.AppendLine("   ,CodiceArticolo   =  '" & Agro_SQL_SaveText(CodiceArticolo) & "'   ")
            'End If
            'If Mat_Cod <> 0 Then
            StrSQL.AppendLine("   ,Mat_Cod   =  " & Agro_SQL_SaveNum(Mat_Cod))
            'End If
            'If NomeArticolo <> "" Then
            StrSQL.AppendLine("   ,NomeArticolo   =  '" & Agro_SQL_SaveText(NomeArticolo) & "'   ")
            'End If
            'If LottoFornitore <> "" Then
            StrSQL.AppendLine("   ,LottoFornitore   =  '" & Agro_SQL_SaveText(LottoFornitore) & "'   ")
            'End If
            'If TipoLotta <> "" Then
            StrSQL.AppendLine("   ,TipoLotta   =  '" & Agro_SQL_SaveText(TipoLotta) & "'   ")
            'End If
            'If PuntoPrelievo <> "" Then
            StrSQL.AppendLine("   ,PuntoPrelievo   =  '" & Agro_SQL_SaveText(PuntoPrelievo) & "'   ")
            'End If

            'If Disciplinare_cod <> "" Then
            StrSQL.AppendLine("   ,Disciplinare_cod   =  '" & Agro_SQL_SaveText(Disciplinare_cod) & "'   ")
            'End If

            'If Note_Impianto <> "" Then
            StrSQL.AppendLine("   ,Note_Impianto   =  '" & Agro_SQL_SaveText(Note_Impianto) & "'   ")
            'End If
            'If Fornitore_Azienda <> "" Then
            StrSQL.AppendLine("   ,Fornitore_Azienda   =  '" & Agro_SQL_SaveText(Fornitore_Azienda) & "'   ")
            'End If

            'If Tipo_Lotta_Acquisti <> "" Then
            StrSQL.AppendLine("   ,Tipo_Lotta_Acquisti   =  '" & Agro_SQL_SaveText(Tipo_Lotta_Acquisti) & "'   ")
            'End If
            StrSQL.AppendLine("   ,fornitore_2   =  '" & Agro_SQL_SaveText(fornitore_2) & "'   ")



            StrSQL.AppendLine("   ,Documentazione   =  '" & Agro_SQL_SaveText(Documentazione) & "'   ")
            StrSQL.AppendLine("   ,Note_Documentazione   =  '" & Agro_SQL_SaveText(Note_Documentazione) & "'   ")


            StrSQL.AppendLine("   ,Data_Modifica   =  " & Agro_SQL_SaveDateTime(Now) & "   ")
            StrSQL.AppendLine("   ,Username_Modifica   =  '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")

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

    Public Function Modifica_Flag_PDC(ByVal ID_PDC_Testata As Integer,
                                      ByVal ID_PDC_Dettagli As Integer,
                                      ByVal Flag_PDC As Integer,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_W.Modifica_Flag_PDC()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   Flag_PDC       =  " & Agro_SQL_SaveNum(Flag_PDC) & "   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")

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

    Public Function Modifica_Flag_LFO(ByVal ID_PDC_Testata As Integer,
                                      ByVal ID_PDC_Dettagli As Integer,
                                      ByVal ID_LFO As Integer,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_W.Modifica_Flag_LFO()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   ID_LFO       =  " & Agro_SQL_SaveNum(ID_LFO) & "   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")

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

    Public Function Modifica_Flag_OldLFO_NewLFO(ByVal ID_PDC_Testata As Integer,
                                                ByVal Old_ID_LFO As Integer,
                                                ByVal Nuovo_ID_LFO As Integer,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaAnalisiDAL.PDC_W.Modifica_Flag_LFO()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   ID_LFO       =  " & Agro_SQL_SaveNum(Nuovo_ID_LFO) & "   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_LFO    =  " & Agro_SQL_SaveNum(Old_ID_LFO) & "   ")

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




    Public Function Modifica_NoteDettaglio(ByVal ID_PDC_Testata As Integer, _
                          ByVal ID_PDC_Dettagli As Integer, _
                             ByVal note_impianto As String, _
                             ByRef objParametri As AgronicaCoreParametri _
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaAnalisiDAL.PDC_W.Modifica_NoteDettaglio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   note_impianto       =  '" & Agro_SQL_SaveText(note_impianto) & "'   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")

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

    Public Function Modifica_Note2(ByVal ID_PDC_Testata As Integer, _
                          ByVal ID_PDC_Dettagli As Integer, _
                             ByVal note2 As String, _
                             ByRef objParametri As AgronicaCoreParametri _
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaAnalisiDAL.PDC_W.Modifica_NoteDettaglio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   note2       =  '" & Agro_SQL_SaveText(note2) & "'   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")

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

    Public Function Modifica_CapitolatiEsclusi(ByVal ID_PDC_Testata As Integer, _
                      ByVal ID_PDC_Dettagli As Integer, _
                         ByVal CapitolatiEsclusi As String, _
                         ByRef objParametri As AgronicaCoreParametri _
                         ) As Boolean

        Dim NomeRoutine As String = "AgronicaAnalisiDAL.PDC_W.Modifica_CapitolatiEsclusi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   CapitolatiEsclusi       =  '" & Agro_SQL_SaveText(CapitolatiEsclusi) & "'   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")

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

    Public Function Modifica_DatiImpianto(ByVal ID_PDC_Testata As Integer, ByVal ID_PDC_Dettagli As Integer,
                             ByVal Note_Impianto As String,
                             ByVal Kpin As String,
                             ByVal Block As String,
                             ByVal Ind_Des As String,
                             ByVal Frz_Des As String,
                             ByVal CAP As String,
                             ByVal Com_Des As String,
                             ByVal Pro_Cod As String,
                             ByVal Stato As String,
                             ByVal Reg As String,
                             ByVal Pro_Cod_Istat As String,
                             ByVal Com_Cod_Istat As String,
                             ByVal Tecnico_Campionamento As String,
                             ByVal Tecnico_Campionamento_Tel As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaAnalisiDAL.PDC_W.Modifica_DatiDettaglio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   Note_Impianto       =  '" & Agro_SQL_SaveText(Note_Impianto) & "'   ")
            StrSQL.AppendLine("   ,Kpin      =  '" & Agro_SQL_SaveText(Kpin) & "'   ")
            StrSQL.AppendLine("   ,Block     =  '" & Agro_SQL_SaveText(Block) & "'   ")
            StrSQL.AppendLine("   ,Ind_Des   =  '" & Agro_SQL_SaveText(Ind_Des) & "'   ")
            StrSQL.AppendLine("   ,Frz_Des   =  '" & Agro_SQL_SaveText(Frz_Des) & "'   ")
            StrSQL.AppendLine("   ,CAP       =  '" & Agro_SQL_SaveText(CAP) & "'   ")
            StrSQL.AppendLine("   ,Com_Des   =  '" & Agro_SQL_SaveText(Com_Des) & "'   ")
            StrSQL.AppendLine("   ,Pro_Cod   =  '" & Agro_SQL_SaveText(Pro_Cod) & "'   ")
            StrSQL.AppendLine("   ,Stato     =  '" & Agro_SQL_SaveText(Stato) & "'   ")
            StrSQL.AppendLine("   ,Reg       =  '" & Agro_SQL_SaveText(Reg) & "'   ")
            StrSQL.AppendLine("   ,Pro_Cod_Istat       =  '" & Agro_SQL_SaveText(Pro_Cod_Istat) & "'   ")
            StrSQL.AppendLine("   ,Com_Cod_Istat       =  '" & Agro_SQL_SaveText(Com_Cod_Istat) & "'   ")
            StrSQL.AppendLine("   ,Tecnico_Campionamento       =  '" & Agro_SQL_SaveText(Tecnico_Campionamento) & "'   ")
            StrSQL.AppendLine("   ,Tecnico_Campionamento_Tel   =  '" & Agro_SQL_SaveText(Tecnico_Campionamento_Tel) & "'   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")

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

    Public Function Modifica_DatiCapoAnimale(ByVal ID_PDC_Testata As Integer, ByVal ID_PDC_Dettagli As Integer,
                                             ByVal GEN_COD As Integer,
                                             ByVal SPE_COD As Integer,
                                             ByVal RAZ_COD As Integer,
                                             ByVal Data_Nascita As Date,
                                             ByVal Sesso As String,
                                             ByVal Matricola As String,
                                             ByVal Lotto As String,
                                             ByVal Raggruppamento_Cod As Integer,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaAnalisiDAL.PDC_W.Modifica_DatiCapoAnimale()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")

            StrSQL.AppendLine("     GEN_COD       =  " & Agro_SQL_SaveNum(GEN_COD) & "   ")
            StrSQL.AppendLine("   , SPE_COD       =  " & Agro_SQL_SaveNum(SPE_COD) & "   ")
            StrSQL.AppendLine("   , RAZ_COD       =  " & Agro_SQL_SaveNum(RAZ_COD) & "  ")
            StrSQL.AppendLine("   , Data_Nascita  =  " & Agro_SQL_SaveDate(Data_Nascita) & "  ")
            StrSQL.AppendLine("   , Sesso         =  '" & Agro_SQL_SaveText(Sesso) & "'   ")
            StrSQL.AppendLine("   , Matricola     =  '" & Agro_SQL_SaveText(Matricola) & "'   ")
            StrSQL.AppendLine("   , Lotto         =  '" & Agro_SQL_SaveText(Lotto) & "'   ")
            StrSQL.AppendLine("   , Raggruppamento_Cod   =  " & Agro_SQL_SaveNum(Raggruppamento_Cod) & "   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser      = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND   ID_PDC_Testata    =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND   ID_PDC_Dettagli    =  " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & "   ")

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

    Public Function UpdateCapoAnimale_da_Anagrafica(ByVal ID_PDC_Testata As Integer,
                                                    ByVal ID_PDC_Dettagli As Integer,
                                                    ByVal RAZ_COD As Integer,
                                                    ByVal Data_Nascita As Date,
                                                    ByVal Sesso As String,
                                                    ByVal Lotto As String,
                                                    ByVal Raggruppamento_Cod As Integer,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_W.UpdateCapoAnimale_da_Anagrafica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("UPDATE PDC_D")
            StrSQL.AppendLine(" SET PDC_D.RAZ_COD =  " & Agro_SQL_SaveNum(RAZ_COD) & " ")
            StrSQL.AppendLine(" , PDC_D.Data_Nascita = " & Agro_SQL_SaveDate(Data_Nascita) & " ")
            StrSQL.AppendLine(" , PDC_D.Sesso = '" & Agro_SQL_SaveText(Sesso) & "' ")
            StrSQL.AppendLine(" , PDC_D.Lotto = '" & Agro_SQL_SaveText(Lotto) & "' ")
            StrSQL.AppendLine(" , PDC_D.Raggruppamento_Cod = " & Agro_SQL_SaveNum(Raggruppamento_Cod) & " ")

            StrSQL.AppendLine(" FROM PDC_Dettagli AS PDC_D ")
            StrSQL.AppendLine(" INNER JOIN PDC_Testata on PDC_Testata.Id_PDC_Testata  = PDC_D.Id_PDC_Testata ")

            StrSQL.AppendLine(" WHERE PDC_D.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND PDC_D.ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND PDC_D.ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli) & " ")

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

    Public Function Modifica_Flag_PDC_Massivo(ByVal ID_PDC_Testata As Integer,
                                              ByVal ID_PDC_Dettagli As String,
                                              ByVal Flag_PDC As Integer,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_Dettagli_W.Modifica_Flag_PDC()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE PDC_Dettagli SET ")
            StrSQL.AppendLine("   Flag_PDC =  " & Agro_SQL_SaveNum(Flag_PDC) & "   ")

            StrSQL.AppendLine(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.AppendLine(" AND ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata) & "   ")
            StrSQL.AppendLine(" AND ID_PDC_Dettagli IN (" & Agro_SQL_Save_Clausola_IN(ID_PDC_Dettagli) & ")   ")

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
#End Region


#Region "Cancellazione"

    Public Function Cancella(ByVal ID_PDC_Testata As Integer,
                             ByVal ID_PDC_Dettagli As Integer,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PianiCampionamentoDAL.PDC_W.Cancella_Dettagli()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE FROM PDC_Dettagli ")

            StrSQL.AppendLine("  WHERE ")
            StrSQL.AppendLine("         PivaSuperUser =  '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         AND ID_PDC_Testata =  " & Agro_SQL_SaveNum(ID_PDC_Testata))
            If ID_PDC_Dettagli <> 0 Then
                StrSQL.AppendLine("     AND ID_PDC_Dettagli = " & Agro_SQL_SaveNum(ID_PDC_Dettagli))
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

#End Region

End Class

