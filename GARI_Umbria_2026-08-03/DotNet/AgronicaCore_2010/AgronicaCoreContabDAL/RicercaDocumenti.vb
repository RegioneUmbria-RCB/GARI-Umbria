Imports System.Data.Entity.Core
Imports System.Data.Entity.Migrations
Imports System.Text
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json

Public Class RicercaDocumenti_R : Inherits AgronicaCoreDataProvider.DataProvider
    Private Class CausaleDocumento
        Public TYPE As String
        Public DOC_TYPE As String
        Public LAV_COD As String
        Public LAV_DES As String
    End Class

    Private causaliDocumenti As List(Of CausaleDocumento) = New List(Of CausaleDocumento)

    Private Sub ValorizzaCausaliDocumenti(tutteLeCausali As String)
        Dim causaliDaJson As List(Of CausaleDocumento)

        Try
            causaliDaJson = JsonConvert.DeserializeObject(Of List(Of CausaleDocumento))(tutteLeCausali)
        Catch
            causaliDaJson = New List(Of CausaleDocumento)
        End Try

        If causaliDaJson.Count > 0 Then
            causaliDocumenti = causaliDaJson
        Else
            causaliDocumenti = New List(Of CausaleDocumento) From {
                New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "F", .LAV_COD = LAVCOD_FATTURA_RICEVUTA, .LAV_DES = Gias.FatturaRicevuta},
                New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "F", .LAV_COD = LAVCOD_NOTA_ACCREDITO_RICEVUTA, .LAV_DES = Gias.NotaAccreditoRicevuta},
                New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "F", .LAV_COD = LAVCOD_FATTURA_EMESSA, .LAV_DES = Gias.FatturaEmessa},
                New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "F", .LAV_COD = LAVCOD_NOTA_ACCREDITO_EMESSA, .LAV_DES = Gias.NotaAccreditoEmessa},
                New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = LAVCOD_BOLLA_RICEVUTA, .LAV_DES = Gias.DDTRicevuto},
                New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "C", .LAV_COD = LAVCOD_BOLLA_EMESSA, .LAV_DES = Gias.DDTEmesso},
                New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "C", .LAV_COD = LAVCOD_DDT_CONTABILIZZATO_EMESSO, .LAV_DES = Gias.DDTContabilizzatoEmesso},
                New CausaleDocumento With {.TYPE = "C", .DOC_TYPE = "C", .LAV_COD = LAVCOD_ACCETTAZIONE_DIVERSI, .LAV_DES = Gias.AccettazioneDDTRicevuto},
                New CausaleDocumento With {.TYPE = "C", .DOC_TYPE = "P", .LAV_COD = LAVCOD_ACCETTAZIONE_DIVERSI, .LAV_DES = Gias.AccettazioneDDTRicevuto},
                New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = LAVCOD_DISTINTA_CARICO, .LAV_DES = Gias.DistintaDiCarico},
                New CausaleDocumento With {.TYPE = "C", .DOC_TYPE = "C", .LAV_COD = LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, .LAV_DES = Gias.AccettazioneDistintaDiCarico},
                New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = LAVCOD_AUTO_DDT_EMESSO, .LAV_DES = Gias.AutoDDTEmesso},
                New CausaleDocumento With {.TYPE = "C", .DOC_TYPE = "C", .LAV_COD = LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE, .LAV_DES = Gias.AccettazioneAutoDDTEmesso},
                New CausaleDocumento With {.TYPE = "V", .DOC_TYPE = "O", .LAV_COD = LAVCOD_ORDINE_VENDITA, .LAV_DES = Gias.OrdineDiVendita},
                New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "O", .LAV_COD = LAVCOD_ORDINE_ACQUISTO, .LAV_DES = Gias.OrdineDiAcquisto},
                New CausaleDocumento With {.TYPE = "CO", .DOC_TYPE = "AF", .LAV_COD = LAVCOD_CONTRATTO_AFFITTO, .LAV_DES = Gias.ContrattoDiAffitto}
            }
        End If

    End Sub

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Legge righe movimenti di vendita per report statistico
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Cerca_Documenti(ByVal type As String,
                                    ByVal doc_type As String,
                                    ByVal tutteLeCausali As String,
                                    ByVal tuttiiCentri As IEnumerable(Of Object),
                                    ByVal dettaglio As Boolean,
                                    ByVal piva As String,
                                    ByVal report As String,
                                    ByVal cubo As Integer,
                                    ByVal _descrizione As String,
                                    ByVal _docNumeroSin As String,
                                    ByVal _docNumero As Integer,
                                    ByVal _docNumeroDes As String,
                                    ByVal _nrRiga As String,
                                    ByVal _dataMovDal As String,
                                    ByVal _dataMovAl As String,
                                    ByVal _centriAziendali As String,
                                    ByVal _clienti As String,
                                    ByVal _agenti As String,
                                    ByVal _causali As String,
                                    ByVal _specie As String,
                                    ByVal _varieta As String,
                                    ByVal _prodotti As String,
                                    ByVal _categorie As String,
                                    ByVal _categcommerciali As String,
                                    ByVal _causali_trasp As String,
                                    ByVal _soloDDTNonFatturati As Boolean,
                                    ByVal _soloOrdiniNonSpediti As Boolean,
                                    ByVal _modalitaFatturazione As Boolean,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreParametri,
                                    ByRef objParametriUtenti As AgronicaCoreParametri,
                                    Optional ByRef GestioneWorkflowDocContabili As Boolean = False,
                                    Optional ByVal top_N_rows As Integer? = Nothing
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicercaDocumenti_R.Cerca_Documenti()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim sortedDT As DataTable
        Dim flagConnessione As Boolean = False

        Try

            ValorizzaCausaliDocumenti(tutteLeCausali)

            Utility.VerificaApriConnessione(objParametri, flagConnessione)

            ' Creazione di eventuali tabelle temporanee che servono per la query principale
            If dettaglio Then
                'Tabella temporanea che contiene i segni per l'imponibile e l'iva
                Dim strSqlTempSegniCreate As String = CreaTabellaTemp_Segni_ImpIva()
                Dim rVal As Boolean = EseguiQuery_Scrittura(objParametri, strSqlTempSegniCreate, "PopolaTabellaTempSegniImpIva")
            End If

            ' Tabella temporanea per conteggio documenti x Agenda
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitatoGestioneVisualizaAllegato = objPermessi.Controlla_Permessi_Utente(objParametriUtenti.UtenteUsername,
                                                                              5,
                                                                              enum_Security_Attivita.Documentale_Lista,
                                                                              enum_Security_Operazione.Lettura,
                                                                              Now, "",
                                                                              objParametriUtenti)
            Crea_Tabelle_Temp_Agende_Documenti(piva, _descrizione, _causali, type, doc_type, _docNumero, _docNumeroSin, _docNumeroDes, _dataMovDal, _dataMovAl, _causali_trasp, UtenteAbilitatoGestioneVisualizaAllegato, objParametri)



            ' Gestione Gruppi Merce: indica se vengono assegnati i gruppi alle categorie prodotto ed ai singoli prodotti
            Dim GestioneGruppiMerce As Boolean = False
            Dim objGruppiMerce As New Gruppi_Merce_R
            Dim ListaGruppiMercePerCategoria = objGruppiMerce.GetListGruppiMerceDefault(piva, SACOD_NOFILTRO, objParametriUtenti, objParametri)

            If Not IsNothing(ListaGruppiMercePerCategoria) AndAlso ListaGruppiMercePerCategoria.Count > 0 Then
                GestioneGruppiMerce = True

                Dim strSql = objGruppiMerce.ComponiSql_CreaTempDefaultGruppiMerce(New List(Of String) From {piva}, objParametri, ListaGruppiMercePerCategoria)
                Dim rVal As Boolean = EseguiQuery_Scrittura(objParametri, strSql.ToString(), "PopolaTabellaTempDefaultGruppiMerce")
            End If


            Dim QuerySQL As String = Componi_Query_Ricerca(type,
                                                           doc_type,
                                                           tutteLeCausali,
                                                           tuttiiCentri,
                                                           dettaglio,
                                                           piva,
                                                           report,
                                                           Nothing,
                                                           _descrizione,
                                                           _docNumeroSin,
                                                           _docNumero,
                                                           _docNumeroDes,
                                                           _nrRiga,
                                                           _dataMovDal,
                                                           _dataMovAl,
                                                           _centriAziendali,
                                                           _clienti,
                                                           _agenti,
                                                           _causali,
                                                           _specie,
                                                           _varieta,
                                                           _prodotti,
                                                           _categorie,
                                                           _categcommerciali,
                                                           _causali_trasp,
                                                           _soloDDTNonFatturati,
                                                           _soloOrdiniNonSpediti,
                                                           _modalitaFatturazione,
                                                           UtenteAbilitatoGestioneVisualizaAllegato,
                                                           xOrderBy,
                                                           objParametri,
                                                           objParametriUtenti,
                                                           GestioneWorkflowDocContabili,
                                                           GestioneGruppiMerce,
                                                           top_N_rows:=top_N_rows
                                                            )

            Dim parametriCollezionati = MyBase.DammiParametriCollezionati()
            MyBase.SettaParametriPrecedenti(parametriCollezionati)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, QuerySQL, nomeRoutine)
            '--------------------------------------------------------------------------

            Dim dv As DataView = dt.DefaultView
            Try
                If dettaglio Then
                    dv.Sort = If(String.IsNullOrEmpty(xOrderBy), "Data_Movimento DESC, Numero_Movimento DESC, Riga ASC", xOrderBy)
                Else
                    dv.Sort = If(String.IsNullOrEmpty(xOrderBy), "Data_Movimento DESC, Numero_Movimento DESC", xOrderBy)
                End If
                sortedDT = dv.ToTable()

            Catch ex As Exception
                sortedDT = dt
            End Try

            If dettaglio Then
                'Droppo la tabella temporanea per i segni dell'imponibile/iva
                Dim strSqlTempSegniDrop As String = EliminaTabellaTemp_Segni_ImpIva()
                Dim rVal As Boolean = EseguiQuery_Scrittura(objParametri, strSqlTempSegniDrop, "DropTabellaTempSegniImpIva")
            End If

            Dim strAgendaDocumenti As String = EliminaTabellaTemp_Agenda_Documenti()
            EseguiQuery_Scrittura(objParametri, strAgendaDocumenti, "DropTabellaTempAgendaDocumenti")

            If GestioneGruppiMerce Then
                EseguiQuery_Scrittura(objParametri, objGruppiMerce.ComponiSql_CancellaTempDefaultGruppiMerce().ToString(), "DropTabellaTempDefaultGruppiMerce")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        Finally
            Utility.VerificaChiudiConnessione(objParametri, flagConnessione)
        End Try

        Return sortedDT

    End Function

    Public Function CercaDocumentiPerPopolamentoWidget(ByVal type As String,
                                                        ByVal doc_Type As String,
                                                        ByVal tutteLeCausali As String,
                                                        ByVal tuttiICentri As IEnumerable(Of Object),
                                                        ByVal piva As String,
                                                        ByVal _causali As String,
                                                        ByVal _centriAziendali As String,
                                                        ByRef objParametriServer As AgronicaCoreParametri,
                                                        Optional ByVal top_N_rows As Integer? = Nothing
                                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicercaDocumenti_R.Cerca_Documenti()"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try
            ValorizzaCausaliDocumenti(tutteLeCausali)
            Dim QuerySQL As String = ComponiQueryRicercaPerPopolamentoWidget(type, doc_Type, tuttiICentri, piva, _causali,
                                                                         _centriAziendali, top_N_rows)

            Dim parametriCollezionati = MyBase.DammiParametriCollezionati()
            MyBase.SettaParametriPrecedenti(parametriCollezionati)

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, QuerySQL, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Parametri_Qualitativi(ByVal piva As String, ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim objConfigDettagli As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        Dim DTParamQual = objConfigDettagli.Leggi(piva, 0, False, " (Tipo IN ( 1,3,4,5) ) AND Tabella_Key NOT IN ('cliente','fornitore')", "", objParametri)
        Return DTParamQual

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Crea query movimenti per ricerca documenti
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Function Componi_Query_Ricerca(ByVal type As String,
                                          ByVal doc_type As String,
                                          ByVal tutteLeCausali As String,
                                          ByVal tuttiiCentri As IEnumerable(Of Object),
                                          ByVal dettaglio As Boolean,
                                          ByVal piva As String,
                                          ByVal report As String,
                                          ByVal tipoValore As String,
                                          ByVal _descrizione As String,
                                          ByVal _docNumeroSin As String,
                                          ByVal _docNumero As Integer,
                                          ByVal _docNumeroDes As String,
                                          ByVal _nrRiga As String,
                                          ByVal _dataMovDal As String,
                                          ByVal _dataMovAl As String,
                                          ByVal _centriAziendali As String,
                                          ByVal _clienti As String,
                                          ByVal _agenti As String,
                                          ByVal _causali As String,
                                          ByVal _specie As String,
                                          ByVal _varieta As String,
                                          ByVal _prodotti As String,
                                          ByVal _categorie As String,
                                          ByVal _categcommerciali As String,
                                          ByVal _causali_trasp As String,
                                          ByVal _soloDDTNonFatturati As Boolean,
                                          ByVal _soloOrdiniNonSpediti As Boolean,
                                          ByVal _modalitaFatturazione As Boolean,
                                          ByVal UtenteAbilitatoGestioneVisualizaAllegato As Boolean,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri,
                                          Optional ByRef objParametriUtenti As AgronicaCoreParametri = Nothing,
                                          Optional ByRef GestioneWorkflowDocContabili As Boolean = False,
                                          Optional ByVal GestioneGruppiMerce As Boolean = False,
                                          Optional ByVal top_N_rows As Integer? = Nothing
                                          ) As String

        Dim DTParamQual = Leggi_Parametri_Qualitativi(piva, objParametri)

        Dim StrSQL As New StringBuilder

        Dim clausaInCentri As String = "0"

        Dim Elenco_LavCod_In = Componi_Elenco_LavCod_In(_causali, type, doc_type)

        ' Gestione Info Creazione/Modifica
        Dim NomeDB_Utenti As String = ""
        If Not IsNothing(objParametriUtenti) Then
            NomeDB_Utenti = objParametriUtenti.Recupera_NomeDB()
        End If

        ' Gestione Workflow
        GestioneWorkflowDocContabili = LeggiGestioneWorkflowDocContabili(piva, objParametri, Elenco_LavCod_In)


        ' Gestione Gruppi Utenti X Gruppi Merce: indica se i gruppi degli utenti vengono limitati a utilizzare solo determinati gruppi merce
        Dim GestioneGruppiUtenteMerce As Boolean = False
        Dim objGruppiUtenteMerce As New Gruppi_UtenteXGruppi_Merce_R(objParametri, objParametriUtenti)
        Dim dtGruppiUtenteMerce As New DataTable()
        If GestioneGruppiMerce Then
            dtGruppiUtenteMerce = objGruppiUtenteMerce.Leggi("Gruppi_UtenteXGruppi_Merce.Piva IN('" & piva & "')", "")
            GestioneGruppiUtenteMerce = dtGruppiUtenteMerce.Rows.Count > 0
        End If

        'Giulia 25/01/2021: gli ordine potrebbero non avere anche associato il centro ed il magazzino, in questo caso il sa_cod è -1, non 0
        If String.IsNullOrEmpty(_centriAziendali) Then
            If (tuttiiCentri.Any) Then
                clausaInCentri = "0,-1," & String.Join(",", tuttiiCentri.Select(Function(s) s.sa_cod).ToList())
            End If
        Else
            clausaInCentri = "0,-1," & String.Join(",", _centriAziendali.Split("|"))
        End If

        'Andrea Novaga 30/09/2024: miglioramento delle prestazioni per la query: si utilizzano solo le join necessarie  
        Dim presentiDestinazioni As Boolean
        Dim presentiVettori As Boolean
        Dim presentiCessionari1 As Boolean
        Dim presentiCessionari2 As Boolean
        Dim presentiAgenti As Boolean
        Dim presentiCapiArea As Boolean
        VerificaPresenzaRiferimentiPerMovimenti(piva, _dataMovDal, _dataMovAl, clausaInCentri, Elenco_LavCod_In, objParametri,
                                                presentiDestinazioni, presentiVettori, presentiCessionari1, presentiCessionari2, presentiAgenti, presentiCapiArea)

        ' Per ricerca testate se sono in modalità ricerca x fatturazione devo comporre un cappello diverso
        If _modalitaFatturazione AndAlso Not dettaglio Then
            StrSQL.AppendLine(" With agenda_CTE ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" piva, id_agenda, lav_cod, sa_cod, ")
            StrSQL.AppendLine(" Des_Lib, Blocco_Flag, Modulo, Tipo_Accettazione, ")
            ' Colonne Aggiuntive Gestione Workflow
            If GestioneWorkflowDocContabili Then
                StrSQL.AppendLine(" Pratica_Cod, ")
            End If
            ' Gestione Info Creazione/Modifica
            StrSQL.AppendLine(" Data_Creazione, Username_Creazione, ")
            StrSQL.AppendLine(" Data_Modifica, Username_Modifica ")
            ' ---
            StrSQL.AppendLine(" )")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" (")
            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine(" ag.piva, ag.id_agenda, ag.Lav_Cod, ag.sa_cod, ")
            StrSQL.AppendLine(" ag.Des_Lib, ag.Blocco_Flag, ag.Modulo, ag.Tipo_Accettazione, ")
            ' Colonne Aggiuntive Gestione Workflow
            If GestioneWorkflowDocContabili Then
                StrSQL.AppendLine(" ISNULL(ag.Pratica_Cod, 0) AS Pratica_Cod, ")
            End If
            ' Gestione Info Creazione/Modifica
            StrSQL.AppendLine(" ag.Data_Creazione, ag.Username_Creazione, ")
            StrSQL.AppendLine(" ag.Data_Modifica, ag.Username_Modifica ")
            ' ---
            StrSQL.AppendLine(" From agenda As ag WITH (nolock)  ")
            StrSQL.AppendLine(" INNER Join Movimenti AS movNrDDT WITH (nolock) ")
            StrSQL.AppendLine(" On ag.PIVA = movNrDDT.PIVA And ag.Id_Agenda = movNrDDT.Id_Agenda And movNrDDT.Cau_Mov = '4000' ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")
            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine(" And (ag.PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
            End If
            If Not String.IsNullOrEmpty(_descrizione) Then
                StrSQL.AppendLine(" AND (ag.des_lib LIKE '%" & Agro_SQL_SaveText(_descrizione) & "%') ")
            End If
            StrSQL.AppendLine(" AND ( ag.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(Elenco_LavCod_In) & ") ) ")
            If _docNumero <> 0 Then
                StrSQL.AppendLine(" AND (Doc_Numero = " & Agro_SQL_SaveNum(_docNumero) & " ) ")
            End If
            If Not String.IsNullOrEmpty(_docNumeroSin) Then
                StrSQL.AppendLine(" AND (Doc_Numero_Sin = '" & Agro_SQL_SaveText(_docNumeroSin) & "' ) ")
            End If
            If Not String.IsNullOrEmpty(_docNumeroDes) Then
                StrSQL.AppendLine(" AND (Doc_Numero_Des = '" & Agro_SQL_SaveText(_docNumeroDes) & "' ) ")
            End If
            If Not String.IsNullOrEmpty(_dataMovDal) Then
                StrSQL.AppendLine(" AND ( movNrDDT.Data_Movimento >= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataMovDal)) & " ) ")
            End If
            If Not String.IsNullOrEmpty(_dataMovAl) Then
                StrSQL.AppendLine(" AND ( movNrDDT.Data_Movimento <= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataMovAl)) & " ) ")
            End If
            If Not String.IsNullOrEmpty(_causali_trasp) Then
                Dim listaCausali = "0, " & Replace(_causali_trasp, "|", ",")
                StrSQL.Append(" AND (movNrDDT.Causale_Trasporto_Cod IN (" & Agro_SQL_Save_Clausola_IN(listaCausali, False) & ")) ")
            End If
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" movimenti_dettagli_CTE ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" piva, id_agenda, sa_cod, id_mov, id_mov_det, ordine_det, elem_cod, qta, rn ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select md.piva, md.id_agenda, md.sa_cod, id_mov, Id_Mov_Det, ordine_det, Elem_cod, qta, ")
            StrSQL.AppendLine(" ROW_NUMBER() OVER(PARTITION BY md.piva, md.id_agenda ORDER BY md.sa_cod desc) As rn ")
            StrSQL.AppendLine(" From movimenti_dettagli md ")
            StrSQL.AppendLine(" INNER Join agenda_CTE ag ON md.piva = ag.piva And ag.Id_Agenda = md.Id_Agenda ")
            StrSQL.AppendLine(" where ")
            StrSQL.AppendLine(" ordine_det <> 1000 and elem_cod <> 502 ")
            StrSQL.AppendLine(" And md.sa_cod In (" & Agro_SQL_Save_Clausola_IN(clausaInCentri) & ") ")
            StrSQL.AppendLine(" ), ")
            StrSQL.AppendLine(" mov_dettagli_riferimenti_CTE ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" piva, id_agenda, id_agenda_rif, id_mov, id_mov_det, id_mov_det_rif,  lav_cod, lav_cod_rif, qta ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" as ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select mrif.piva, mrif.id_agenda, mrif.id_agenda_rif, mrif.id_mov, mrif.id_mov_det, mrif.id_mov_det_rif, mrif.lav_cod, mrif.lav_cod_rif, qta ")
            StrSQL.AppendLine(" From Mov_Dettagli_Riferimenti as mrif with (nolock) ")
            StrSQL.AppendLine(" inner join agenda_CTE ag on mrif.Id_Agenda_Rif = ag.id_agenda ")
            StrSQL.AppendLine(" WHERE (mrif.Lav_Cod_Rif = 1031 AND mrif.Lav_Cod = 1001)  ")
            StrSQL.AppendLine(" Or (mrif.Lav_Cod_Rif = 1071 AND mrif.Lav_Cod = 1001)  ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine(" Case ")
            StrSQL.AppendLine(" WHEN Mov_non_Fatturati <> '' and Mov_Fatturati_Parzialmente = '' THEN 'NON FATTURATI' ") 'i18n
            StrSQL.AppendLine(" WHEN Mov_non_Fatturati <> '' and Mov_Fatturati_Parzialmente <> '' THEN 'NON FATTURATI / FATT. PARZIALMENTE'  ")
            StrSQL.AppendLine(" WHEN Mov_non_Fatturati = '' and Mov_Fatturati_Parzialmente <> '' THEN 'FATT. PARZIALMENTE'  ")
            StrSQL.AppendLine(" Else '' END AS Stato_Fatturazione, * ")
            StrSQL.AppendLine(" FROM ")
            StrSQL.AppendLine(" ( ")
            StrSQL.AppendLine(" Select ")
            StrSQL.AppendLine(" -- movimenti non fatturati ")
            StrSQL.AppendLine(" coalesce(STUFF(( ")
            StrSQL.AppendLine(" Select ', ' + cast(mov_det.id_mov_det as varchar) ")
            StrSQL.AppendLine(" From movimenti_dettagli_CTE mov_det ")
            StrSQL.AppendLine(" Where mov_det.id_mov_det Not In ")
            StrSQL.AppendLine(" (")
            StrSQL.AppendLine(" Select  Id_Mov_Det_Rif ")
            StrSQL.AppendLine(" From mov_dettagli_riferimenti_CTE mrif ")
            StrSQL.AppendLine(" inner Join movimenti_dettagli_Cte md on mrif.id_mov_det_rif = md.id_mov_det ")
            StrSQL.AppendLine(" ) And mov_det.id_agenda = ag.id_agenda ")
            StrSQL.AppendLine(" For Xml PATH ('')),1,2,''), '') AS Mov_Non_Fatturati ")
            StrSQL.AppendLine(" -- movimenti fatturati parzialmente ")
            StrSQL.AppendLine(" , coalesce(STUFF (( ")
            StrSQL.AppendLine(" Select ', ' + cast(mov_det.id_mov_det as varchar) ")
            StrSQL.AppendLine(" From movimenti_dettagli_CTE mov_det ")
            StrSQL.AppendLine(" Where mov_det.id_mov_det In ")
            StrSQL.AppendLine(" (")
            StrSQL.AppendLine(" Select  Id_Mov_Det_Rif ")
            StrSQL.AppendLine(" From mov_dettagli_riferimenti_CTE mrif ")
            StrSQL.AppendLine(" inner Join movimenti_dettagli_Cte md on mrif.id_mov_det_rif = md.id_mov_det ")
            StrSQL.AppendLine(" And mrif.id_agenda_rif = ag.id_agenda ")
            StrSQL.AppendLine(" group by mrif.id_mov_det_rif ")
            StrSQL.AppendLine(" having sum(mrif.qta) < avg(md.qta) ")
            StrSQL.AppendLine(" ) And mov_det.id_agenda = ag.id_agenda ")
            StrSQL.AppendLine(" For Xml PATH ('')),1,2,''), '') AS Mov_Fatturati_Parzialmente ")
            StrSQL.AppendLine(", ag.PIVA ")
        Else
            StrSQL.AppendLine("SELECT ")
            If Not IsNothing(top_N_rows) AndAlso top_N_rows.HasValue Then
                StrSQL.AppendLine(" TOP " + top_N_rows.ToString)
            End If
            StrSQL.AppendLine(" '' AS Stato_Fatturazione ")
            StrSQL.AppendLine(" , '' AS Mov_Non_Fatturati ")
            StrSQL.AppendLine(" , '' AS Mov_Fatturati_Parzialmente ")
            StrSQL.AppendLine(" ,ag.PIVA ")
        End If

        StrSQL.AppendLine(" ,ag.Id_Agenda ")
        StrSQL.AppendLine(" ,ag.Lav_Cod ")
        StrSQL.AppendLine(" ,ag.Sa_Cod AS Sa_Cod_Agenda ")
        StrSQL.AppendLine(" ,mov_det.Sa_Cod ")
        StrSQL.AppendLine(" , CASE WHEN centri.sa_nome IS NULL THEN 'Non Specificato' ELSE centri.sa_nome END AS Sa_Nome ") 'i18n
        StrSQL.AppendLine(" ,ag.Des_Lib ")
        StrSQL.AppendLine(" ,ag.Blocco_Flag ")
        StrSQL.AppendLine(" ,ag.Modulo ")
        StrSQL.AppendLine(" ,ag.Tipo_Accettazione ")
        StrSQL.AppendLine(" ,movNrDDT.Causale_Trasporto ")
        StrSQL.AppendLine(" ,movNrDDT.Causale_Trasporto_Cod ")
        StrSQL.AppendLine(" ,movNrDDT.Scadenza ")
        StrSQL.AppendLine(" ,movNrDDT.Extra_Int ")
        StrSQL.AppendLine(" ,movNrDDT.peso as Peso ")
        StrSQL.AppendLine(" ,movNrDDT.tara_veicolo as Tara_Veicolo")

        If dettaglio Then
            StrSQL.AppendLine(" , ISNULL(mov_det.Variazione,0) as Degrado_Perc ")
            StrSQL.AppendLine(" ,CASE WHEN mov_det.Variazione IS NULL THEN 0 ")
            StrSQL.AppendLine("       WHEN mov_det.Variazione = 0 THEN 0 ")
            StrSQL.AppendLine("       ELSE Convert(Int, ROUND(ISNULL(mov_det.Qta_Extra_Totale, 0) / 100 * mov_det.Variazione, 0)) ")
            StrSQL.AppendLine("  END AS Degrado ")
            StrSQL.AppendLine(" ,CASE WHEN mov_det.Variazione IS NULL THEN Convert(Int, round(ISNULL(mov_det.Qta_Extra_Totale, 0), 0)) ")
            StrSQL.AppendLine("       WHEN mov_det.Variazione = 0 THEN Convert(Int, round(ISNULL(mov_det.Qta_Extra_Totale, 0), 0)) ")
            StrSQL.AppendLine("       ELSE Convert(Int, round(ISNULL(mov_det.Qta_Extra_Totale, 0), 0)) ")
            StrSQL.AppendLine("       - convert(int, round((isnull(mov_det.Qta_Extra_Totale,0) / 100 * mov_det.Variazione),0)) ")
            StrSQL.AppendLine("  END AS Netto_Pagamento ")
        End If

        If Not dettaglio Then
            StrSQL.AppendLine(" ,movNrDDT.Num_Protocollo as Importo ")
        End If

        StrSQL.AppendLine(" , CASE WHEN ag.Lav_Cod = " & LAVCOD_FATTURA_EMESSA & " THEN CASE WHEN movNrDDT.Extra_Int = 1 THEN 'Fattura accompagnatoria' ELSE 'Fattura' END ") 'i18n
        Dim lavCodAmmessi = Lista_LavCod_Ammessi(type, doc_type)

        For Each lc As CausaleDocumento In lavCodAmmessi
            StrSQL.AppendLine(String.Format("WHEN ag.Lav_Cod = {0} THEN '{1}'", lc.LAV_COD, lc.LAV_DES))
        Next

        StrSQL.AppendLine(" ELSE '' END AS Tipo_Documento ")

        StrSQL.AppendLine(" ,ISNULL(r_u.Settore_Des, '') AS Soggetto_Codice ")
        StrSQL.AppendLine(" ,ISNULL(cont.Cod_Contatto, '') AS Soggetto_Piva ")
        StrSQL.AppendLine(" ,ISNULL(cont.Rag_Soc, '') + ' ' + ISNULL(cont.Cognome, '') + ' ' + ISNULL(cont.Nome, '')  AS Soggetto_RagioneSociale ")

        If presentiCessionari1 Then
            StrSQL.AppendLine(" ,ISNULL(cont_cess1.Cod_Contatto, '') AS Soggetto_Cess1_Piva ")
            StrSQL.AppendLine(" ,ISNULL(cont_cess1.Rag_Soc, '') + ' ' + ISNULL(cont_cess1.Cognome, '') + ' ' + ISNULL(cont_cess1.Nome, '')  AS Soggetto_Cess1_RagioneSociale ")
        Else
            StrSQL.AppendLine(" ,'' AS Soggetto_Cess1_Piva ")
            StrSQL.AppendLine(" ,'' AS Soggetto_Cess1_RagioneSociale ")
        End If
        If presentiCessionari2 Then
            StrSQL.AppendLine(" ,ISNULL(cont_cess2.Cod_Contatto, '') AS Soggetto_Cess2_Piva ")
            StrSQL.AppendLine(" ,ISNULL(cont_cess2.Rag_Soc, '') + ' ' + ISNULL(cont_cess2.Cognome, '') + ' ' + ISNULL(cont_cess2.Nome, '')  AS Soggetto_Cess2_RagioneSociale ")
        Else
            StrSQL.AppendLine(" ,'' AS Soggetto_Cess2_Piva ")
            StrSQL.AppendLine(" ,'' AS Soggetto_Cess2_RagioneSociale ")
        End If

        If presentiDestinazioni Then
            StrSQL.AppendLine(" ,ISNULL(r_u_destinazione.Settore_Des, '') AS Destinazione_Codice ")
        Else
            StrSQL.AppendLine(" ,'' AS Destinazione_Codice ")
        End If
        StrSQL.AppendLine(" , movNrDDT.Data_Movimento ")

        StrSQL.AppendLine(" , CASE WHEN movNrDDT.ora IS NULL THEN '00:00' ")
        StrSQL.AppendLine("        ELSE ")
        StrSQL.AppendLine("         replicate('0', 2- len(ltrim(cast(DATEPART(HOUR, movNrDDT.ora) As varchar (2))))) + ltrim(cast(DATEPART(HOUR, movNrDDT.ora) As varchar (2))) + ':' ")
        StrSQL.AppendLine("         + replicate('0', 2- len(ltrim(cast(DATEPART(MINUTE, movNrDDT.ora) As varchar (2))))) + ltrim(cast(DATEPART(MINUTE, movNrDDT.ora) As varchar (2))) END AS Ora_Movimento ")

        StrSQL.AppendLine(" , movNrDDT.Doc_Numero_Sin + ")
        StrSQL.AppendLine("     CASE WHEN LEN(LTRIM(STR(movNrDDT.doc_numero,10))) > 5 THEN LTRIM(STR(movNrDDT.doc_numero,10)) ")
        StrSQL.AppendLine("     ELSE REPLICATE('0', 5 - LEN(LTRIM(STR(movNrDDT.doc_numero,10)))) + LTRIM(STR(movNrDDT.doc_numero,10))  END ")
        StrSQL.AppendLine("     + movNrDDT.Doc_Numero_Des AS Numero_Movimento ")
        StrSQL.AppendLine(" , CONVERT(varchar, movNrDDT.Data_Movimento, 103) AS Data_Documento ")
        StrSQL.AppendLine(" , movNrDDT.Doc_Numero_Sin ")
        StrSQL.AppendLine(" , movNrDDT.Doc_Numero ")
        StrSQL.AppendLine(" , movNrDDT.Doc_Numero_Des ")
        StrSQL.AppendLine(" , YEAR(movNrDDT.Data_Movimento) AS Anno_Movimento ")
        StrSQL.AppendLine(" , SUBSTRING(CONVERT(varchar,movNrDDT.Data_Movimento,120),1,7) AS Mese_Movimento ")
        StrSQL.AppendLine(" , REPLACE(REPLACE(SUBSTRING(CONVERT(varchar,movNrDDT.Data_Movimento,120),1,7), YEAR(movNrDDT.Data_Movimento) , ''), '-', '') As MeseNoAnno ")
        StrSQL.AppendLine(" , movNrBolla.Data_Movimento as Data_Movimento_DDT ")
        StrSQL.AppendLine(" , movNrBolla.Doc_Numero_Sin + ")
        StrSQL.AppendLine("     CASE WHEN LEN(LTRIM(STR(movNrBolla.doc_numero,10))) > 5 THEN LTRIM(STR(movNrBolla.doc_numero,10)) ")
        StrSQL.AppendLine("     ELSE REPLICATE('0', 5 - LEN(LTRIM(STR(movNrBolla.doc_numero,10)))) + LTRIM(STR(movNrBolla.doc_numero,10))  END  ")
        StrSQL.AppendLine("     + movNrBolla.Doc_Numero_Des As Numero_Movimento_DDT ")
        If type = DocContab_TipoRicerca_Contratti AndAlso doc_type = DocContab_TipoDoc_ContrattoAffitto Then
            StrSQL.AppendLine(" , movNrBolla.Data_Registrazione as Data_Iniz_Val_Contratto ")
            StrSQL.AppendLine(" , movNrBolla.Scadenza as Data_Fine_Val_Contratto ")
            StrSQL.AppendLine(" , movNrBolla.Mov_Desc as Altri_Locatori_Contratto ")
            StrSQL.AppendLine(" , movNrBolla.Extra_Str as Riferimento_Ordini_Contratto ")
        End If
        If presentiVettori Then
            StrSQL.AppendLine(" , ISNULL(r_u_vettore.Settore_Des, '') AS Vettore_Codice ")
            StrSQL.AppendLine(" , ISNULL(cont_vettore.Rag_Soc, '') + ' ' + ISNULL(cont_vettore.Cognome, '') + ' ' + ISNULL(cont_vettore.Nome, '') AS Vettore_RagioneSociale ")
            StrSQL.AppendLine(" , ISNULL(Movimento_Extra_0.Targa, '') AS TargaMezzoVettore ")
        Else
            StrSQL.AppendLine(" , '' AS Vettore_Codice ")
            StrSQL.AppendLine(" , '' AS Vettore_RagioneSociale ")
            StrSQL.AppendLine(" , '' AS TargaMezzoVettore ")
        End If
        If presentiAgenti Then
            StrSQL.AppendLine(" , ISNULL(r_u_agente.Settore_Des, '') AS Agente_Codice ")
            StrSQL.AppendLine(" , ISNULL(cont_agente.Cod_Contatto, '') AS Agente_Piva ")
            StrSQL.AppendLine(" , ISNULL(cont_agente.Rag_Soc, '') + ' ' + ISNULL(cont_agente.Cognome, '') + ' ' + ISNULL(cont_agente.Nome, '')  AS Agente_RagioneSociale ")
        Else
            StrSQL.AppendLine(" , '' AS Agente_Codice ")
            StrSQL.AppendLine(" , '' AS Agente_Piva ")
            StrSQL.AppendLine(" , '' AS Agente_RagioneSociale ")
        End If
        If presentiCapiArea Then
            StrSQL.AppendLine(" , ISNULL(r_u_capoarea.Settore_Des, '') AS Capoarea_Codice ")
            StrSQL.AppendLine(" , ISNULL(cont_capoarea.Rag_Soc, '') + ' ' + ISNULL(cont_capoarea.Cognome, '') + ' ' + ISNULL(cont_capoarea.Nome, '')  AS Capoarea_RagioneSociale ")
        Else
            StrSQL.AppendLine(" , '' AS Capoarea_Codice ")
            StrSQL.AppendLine(" , ''  AS Capoarea_RagioneSociale ")
        End If
        StrSQL.AppendLine(" , REPLACE( ")
        StrSQL.AppendLine("     CASE WHEN movNrDDT.Extra_Str IS NOT NULL AND movNrDDT.Extra_Str <> '' THEN")
        StrSQL.AppendLine("     movNrDDT.Extra_Str ELSE")
        StrSQL.AppendLine("     movNrDDT.Mov_Desc END,")
        StrSQL.AppendLine("   '§', ' ') AS Note ")
        If presentiDestinazioni Then
            StrSQL.AppendLine(" , CASE WHEN movNrDDT.Cod_RisUm = movNrDDT.Cod_destinazione THEN ")
            StrSQL.AppendLine("     CASE WHEN ISNULL( IndirizzoTipo_dest.Descrizione, '') = '' Then ")
            StrSQL.AppendLine("         CASE WHEN ISNULL(indirizzi_dest.ind_des, '') = '' THEN  ISNULL(cont_destinazione.Rag_Soc, '') ELSE indirizzi_dest.ind_des END ")
            StrSQL.AppendLine("     ELSE IndirizzoTipo_dest.Descrizione END ")
            StrSQL.AppendLine("  ELSE COALESCE(ISNULL(cont_destinazione.Rag_Soc , '') + ' ' + isnull(cont_destinazione.Cognome, '') + ' ' + ISNULL(cont_destinazione.nome, ''), '') end ")
            StrSQL.AppendLine("  AS Destinazione ")
        Else
            StrSQL.AppendLine(" , '' AS Destinazione ")
        End If
        If UtenteAbilitatoGestioneVisualizaAllegato = True Then
            StrSQL.AppendLine(" , Case when doc.conteggio Is null then 0 ")
            StrSQL.AppendLine(" Else ")
            StrSQL.AppendLine(" Case when doc.conteggio > 0 then 1 else 0 end ")
            StrSQL.AppendLine(" End ")
        Else
            StrSQL.AppendLine(" , 0 ")
        End If
        StrSQL.AppendLine(" As DocumentiPresenti ")

        If dettaglio Then

            StrSQL.AppendLine(" ,ISNULL(r_c.Rapporto_Des, '') as Soggetto_Rapporto ")
            StrSQL.AppendLine(" ,mov.id_mov")
            StrSQL.AppendLine(" ,mov.cau_mov")
            StrSQL.AppendLine(" ,mov_det.Id_Mov_Det ")
            StrSQL.AppendLine(" ,mov_det.Contabilizzato ")
            StrSQL.AppendLine(" , mov_det.Ordine_Det AS Riga ")
            StrSQL.AppendLine(" , mat_prima.Mat_Cod AS Referenza_Codice ")
            StrSQL.AppendLine(" , mat_prima.Mat_Des ")
            StrSQL.AppendLine(" , mat_prima.Cod_Articolo")
            StrSQL.AppendLine(" , ISNULL(mat_prima.Codice_Esterno, '') As Codice_Esterno")
            StrSQL.AppendLine(" , mov_det.Mov_Det_Des")
            StrSQL.AppendLine(" ,(CASE WHEN ISNULL(mat_prima.Cod_articolo,'') = '' THEN ISNULL(mat_prima.Mat_Des,mov_det.Mov_Det_Des) ELSE ")
            StrSQL.AppendLine(" ISNULL(mat_prima.Mat_Des,mov_det.Mov_Det_Des) + ' (' + mat_prima.Cod_articolo + ')' END)     AS Referenza_Descr ")
            StrSQL.AppendLine(" , REPLACE(mov_det.Extra_Str,'§',' ') AS Note_Prodotto ")
            StrSQL.AppendLine(" ,mov_det.Elem_Cod ")
            StrSQL.AppendLine(" , mov_det.Pro_Cod ")
            StrSQL.AppendLine(" , mov_det.Mat_Cod ")
            StrSQL.AppendLine(" , mat_prima.Veg_Cod ")
            StrSQL.AppendLine(" , sv.Veg_Des ")
            StrSQL.AppendLine(" , mat_prima.Cul_Cod ")
            StrSQL.AppendLine(" , cul.Cul_Des ")
            StrSQL.AppendLine(" , mat_prima.Cat_Cod ")
            StrSQL.AppendLine(" , CASE mov_det.Elem_Cod ")
            StrSQL.AppendLine("  WHEN " & ALTRI_BENI & " THEN '" & Agro_SQL_SaveText(Gias.AltriBeniStrumentali) & "'")
            StrSQL.AppendLine("  WHEN " & RIGA_DESCRIZIONE_LIBERA & " THEN '" & Agro_SQL_SaveText(Gias.RigaDescrizioneLibera) & "'")
            StrSQL.AppendLine("  WHEN " & SERVIZI & " THEN '" & Agro_SQL_SaveText(Gias.Servizi) & "'")
            StrSQL.AppendLine("  ELSE ISNULL(cat_mag.NomeComune,'') END as Categoria_Prodotto ")
            StrSQL.AppendLine(" , ISNULL(cat_com.Linea_Classe_Des,'') AS Categoria_Commerciale ")
            StrSQL.AppendLine(" , mov_det.Lotto ")
            StrSQL.AppendLine(" , mov_det.Cal_Cod ")
            StrSQL.AppendLine(" , mov_det.UDM_COD AS Unita_Misura ")
            StrSQL.AppendLine(" , unitamisura.udm_sim AS Unita_Misura_Sigla ")
            StrSQL.AppendLine(" , unitamisura.udm_des AS Unita_Misura_Des ")
            StrSQL.AppendLine(" , mov_det.UDM_COD_EXTRA AS Unita_Misura_Secondaria ")
            StrSQL.AppendLine(" , udm_sec.udm_sim AS Unita_Misura_Secondaria_Sigla ")
            StrSQL.AppendLine(" , udm_sec.udm_des AS Unita_Misura_Secondaria_Des, ")
            StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " THEN -1 ELSE 1 END) * CASE WHEN mov_det.Udm_Cod Not IN (0,2) THEN mov_det.Qta ELSE 0 END AS Nr_Confezioni, ")
            StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " THEN -1 ELSE 1 END) * mov_det.Qta_Dettaglio1 AS Nr_Contenitori, ")
            StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " THEN -1 ELSE 1 END) * mov_det.Qta_Dettaglio2 AS Nr_Imballi, ")

            'Dati Quantità / Peso
            If tipoValore <> "4" AndAlso tipoValore <> "5" Then
                StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " THEN -1 ELSE 1 END) * ISNULL(mov_det.Qta,mov_det.Qta) AS Qta,")
            Else
                ' Kg / Lt
                If tipoValore = "4" Then
                    ' Se sono già in Kg / Lt sono a posto
                    StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " THEN -1 ELSE 1 END) * (CASE WHEN mov_det.udm_cod = 2 Or mov_det.udm_cod = 29 THEN mov_det.Qta ELSE  ")
                    ' altrimenti Se sono in confezioni e sulla riga c'è già la conversione in U.M. secondaria ... 
                    StrSQL.AppendLine("    (CASE WHEN mov_det.udm_cod = 38 And mov_det.Qta_Extra_Totale != 0 And mov_det.udm_cod != mov_det.udm_cod_extra  THEN   ")
                    ' ... prendo la secondaria totale
                    StrSQL.AppendLine("         mov_det.Qta_Extra_Totale ELSE ")
                    ' altrimenti Se sono in confezioni e sull'anagrafica c'è un fattore di conversione moltiplico le confezioni per il fattore di conversione
                    StrSQL.AppendLine("             (CASE WHEN ISNULL(m_p.Qta_Extra,0) != 0 THEN mov_det.Qta * ISNULL(m_p.Qta_Extra,0) ")
                    ' altrimenti 0 in tutti gli altri casi
                    StrSQL.AppendLine("                                                     Else 0 End) ")
                    StrSQL.AppendLine("         End) ")
                    StrSQL.AppendLine("  End) As Qta,")
                Else
                    ' Pezzi / Nr
                    If tipoValore = "5" Then
                        ' Se sono già in Confezioni sono a posto
                        StrSQL.AppendLine(" (Case When ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " Then -1 Else 1 End) * (Case When mov_det.udm_cod = 38 Then mov_det.Qta Else  ")
                        ' altrimenti Se sono in kg / lt e sull'anagrafica c'è un fattore di conversione divido kg / lt per il fattore di conversione trovando le confezioni
                        StrSQL.AppendLine("   (Case When (mov_det.udm_cod = 2 Or mov_det.udm_cod = 29) And ISNULL(m_p.Qta_Extra,0) != 0 Then   ")
                        StrSQL.AppendLine("       mov_det.Qta / ISNULL(m_p.Qta_Extra,0) ")
                        ' altrimenti 0 in tutti gli altri casi
                        StrSQL.AppendLine("         Else 0 End) ")
                        StrSQL.AppendLine("  End) As Qta,")
                    End If
                End If
            End If

            StrSQL.AppendLine(" mov_det.Qta_Extra,")
            StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " Then -1 Else 1 End) * ISNULL(mov_det_rif.Qta_Extra_Totale, mov_det.Qta_Extra_Totale) As Kg_Netti, ")
            StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " Then -1 Else 1 End) * ISNULL(mov_det_rif.Tara, mov_det.Tara) As Tara_Totale, ")
            StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " Then -1 Else 1 End) * (ISNULL(mov_det_rif.Qta_Extra_Totale, mov_det.Qta_Extra_Totale) + ISNULL(mov_det_rif.Tara, mov_det.Tara)) As Kg_Lordi, ")
            StrSQL.AppendLine(" ISNULL(mov_det_rif.Qta_Evasa, 0) AS Qta_Evasa, ")
            StrSQL.AppendLine(" CASE WHEN mov_det_rif.Qta_Residua IS NOT NULL AND mov_det.Contabilizzato In (3,-3) THEN 0 ")
            StrSQL.AppendLine("      ELSE ISNULL(mov_det_rif.Qta_Residua, mov_det.Qta) ")
            StrSQL.AppendLine(" END As Qta_Residua, ")
            StrSQL.AppendLine(" CASE WHEN mat_prima.Elem_Cod = " & RIGA_DESCRIZIONE_LIBERA & " THEN '' ")
            StrSQL.AppendLine("   WHEN mov_det.Contabilizzato In (3,-3) THEN 'Evaso Forzatamente' ") 'i18n
            StrSQL.AppendLine("   WHEN ISNULL(mov_det_rif.Qta_Evasa, 0) = 0 THEN 'Non Evaso' ")
            StrSQL.AppendLine("   WHEN ISNULL(mov_det_rif.Qta_Evasa, 0) < mov_det.Qta THEN 'Evaso Parzialmente' ")
            StrSQL.AppendLine("   WHEN ISNULL(mov_det_rif.Qta_Evasa, 0) >= mov_det.Qta THEN 'Evaso' ")
            StrSQL.AppendLine(" END AS StatoEvasione_Des, ")
            'StrSQL.AppendLine(" mov_det_rif.Qta_Netta_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else mov_det_rif.Qta_Netta_Residua End As Qta_Netta_Residua, ")
            'StrSQL.AppendLine(" mov_det_rif.Qta_Tara_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else mov_det_rif.Qta_Tara_Residua End As Qta_Tara_Residua, ")
            'StrSQL.AppendLine(" mov_det_rif.Qta_Lorda_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else mov_det_rif.Qta_Lorda_Residua End As Qta_Lorda_Residua, ")
            'StrSQL.AppendLine(" mov_det_rif.Qta_Contenitori_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else mov_det_rif.Qta_Contenitori_Residua End As Qta_Contenitori_Residua, ")
            'StrSQL.AppendLine(" mov_det_rif.Qta_Imballi_Evasa, Case When mov_det_rif.Qta_Residua Is Not NULL And mov_det.Contabilizzato In (3,-3) Then 0 Else mov_det_rif.Qta_Imballi_Residua End As Qta_Imballi_Residua, ")
            StrSQL.AppendLine(" mov_det.Prezzo_Unitario_Netto As Prezzo_Netto, ")
            StrSQL.AppendLine(" CASE mov_det.prezzo_livello WHEN 4 THEN 'Imballo' WHEN 8 THEN 'Contenitore' WHEN 5 THEN 'Confezione' ELSE 'KG' END AS Prezzo_Riferito_A, ")

            StrSQL.AppendLine(" (ISNULL(#Segni_ImpIva.Segno_Imponibile,1) * mov_det.Imponibile) AS Imponibile, ")
            StrSQL.AppendLine(" mov_det.sconto * -1 as Sconto_Perc, ")
            StrSQL.AppendLine(" ISNULL(#Segni_ImpIva.Segno_Imponibile,1) * (mov_det.Imponibile - mov_det.Imponibile_Netto) AS Sconto, ")
            StrSQL.AppendLine(" (ISNULL(#Segni_ImpIva.Segno_Imponibile,1) * mov_det.Imponibile_Netto) AS Imponibile_Netto, ")
            StrSQL.AppendLine(" (ISNULL(#Segni_ImpIva.Segno_Iva, 1) * mov_det.Iva) AS Iva, ")
            StrSQL.AppendLine(" mov_det.Cod_Iva, IVA_Aliquote.Descrizione AS Aliquota_Iva_Des, ")
            StrSQL.AppendLine(" (ISNULL(#Segni_ImpIva.Segno_Imponibile,1) * mov_det.Imponibile_Netto) + (ISNULL(#Segni_ImpIva.Segno_Iva, 1) * mov_det.Iva) as Importo, ")
            StrSQL.AppendLine(" ISNULL(Movimento_Extra_Dettagli.Provvigione, 0) AS Provvigione, ")
            StrSQL.AppendLine(" (ISNULL(#Segni_ImpIva.Segno_Imponibile,1) * mov_det.Imponibile_Netto) / 100 * ISNULL(Movimento_Extra_Dettagli.Provvigione, 0) as Provvigione_Calcolata, ")

            StrSQL.AppendLine(" ISNULL(Indirizzi.CAP, '') AS CAP, ISNULL(Indirizzi.com_des, '') AS Comune, ISNULL(Indirizzi.pro_cod, '') AS Sigla, ")
            StrSQL.AppendLine(" ISNULL(Lista_Province.PROVINCIA, '') AS Provincia, ISNULL(Lista_Regioni.Regione_Des, '') AS Regione, ISNULL(Lista_Nazioni.Descrizione,ISNULL(Indirizzi.stato, '')) AS Stato, ")
            If presentiDestinazioni Then
                StrSQL.AppendLine(" ISNULL(indirizzi_dest.CAP, '') AS CAP_Dest, ")
                StrSQL.AppendLine(" CASE WHEN ISNULL(indirizzi_dest.com_des, '') = '' THEN ISNULL(indirizzi_dest.frz_des, '') ELSE ISNULL(indirizzi_dest.com_des, '') END AS Comune_Dest, ")
                StrSQL.AppendLine(" ISNULL(indirizzi_dest.pro_cod, '') AS Sigla_Dest, ")
                StrSQL.AppendLine(" ISNULL(province_dest.PROVINCIA, '') AS Provincia_Dest, ISNULL(regioni_dest.Regione_Des, '') AS Regione_Dest, ISNULL(nazioni_dest.Descrizione,ISNULL(indirizzi_dest.stato, '')) AS Stato_Dest ")
            Else
                StrSQL.AppendLine(" '' AS CAP_Dest, ")
                StrSQL.AppendLine(" '' AS Comune_Dest, ")
                StrSQL.AppendLine(" '' AS Sigla_Dest, ")
                StrSQL.AppendLine(" '' AS Stato_Dest ")
            End If

            ' Parametri Qualitativi
            If DTParamQual IsNot Nothing Then
                For Each paramQual In DTParamQual.Rows
                    Select Case paramQual("Tipo")
                        Case 1
                            StrSQL.AppendLine(" , ISNULL(OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Sigla, '') AS " + paramQual("Tabella_Key") + "_Sigla  ")
                            StrSQL.AppendLine(" , ISNULL(OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Descrizione, '') AS " + paramQual("Tabella_Key") + "_Descrizione ")
                        Case 3
                            StrSQL.AppendLine(" , ISNULL(Convert(float,Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Val_Cod), '') AS " + paramQual("Tabella_Key") + "_Val_Cod  ")
                        Case 4
                            StrSQL.AppendLine(" , ISNULL(Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Val_Cod, '') AS " + paramQual("Tabella_Key") + "_Val_Cod  ")
                        Case 5
                            StrSQL.AppendLine(" , Case  when ISNULL(Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Val_Cod, '') = '' THEN null ")
                            StrSQL.AppendLine("         when ISNULL(Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Val_Cod, '') = '0' THEN null ")
                            StrSQL.AppendLine("        Else Convert(DateTime, Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Val_Cod , 120)   ")
                            'StrSQL.AppendLine("        Else Convert(varchar,Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Val_Cod, 103)  ")
                            StrSQL.AppendLine(" End  ")
                            StrSQL.AppendLine(" AS " + paramQual("Tabella_Key") + "_Val_Cod")
                    End Select
                Next
            End If

            If {DocContab_TipoRicerca_Acquisti, DocContab_TipoRicerca_Conferimenti, DocContab_TipoRicerca_Vendite}.Contains(type) AndAlso
                    {DocContab_TipoDoc_Consegna, DocContab_TipoDoc_Fattura}.Contains(doc_type) Then

                StrSQL.AppendLine(" , CASE WHEN Movimenti_Ordini.Id_Mov IS NOT NULL THEN ")
                StrSQL.AppendLine("     Movimenti_Ordini.Doc_Numero_Sin + ")
                StrSQL.AppendLine("     CASE WHEN LEN(LTRIM(STR(Movimenti_Ordini.doc_numero,10))) > 5 THEN LTRIM(STR(Movimenti_Ordini.doc_numero,10)) ")
                StrSQL.AppendLine("     ELSE REPLICATE('0', 5 - LEN(LTRIM(STR(Movimenti_Ordini.doc_numero,10)))) + LTRIM(STR(Movimenti_Ordini.doc_numero,10))  END ")
                StrSQL.AppendLine("     + Movimenti_Ordini.Doc_Numero_Des ")
                StrSQL.AppendLine("   ELSE '' END AS Numero_Ordine ")
                StrSQL.AppendLine(" , CASE WHEN Movimenti_Ordini.Id_Mov IS NOT NULL THEN ")
                StrSQL.AppendLine("     CONVERT(varchar, Movimenti_Ordini.Data_Movimento, 103) ")
                StrSQL.AppendLine("   ELSE NULL END AS Data_Ordine ")
                StrSQL.AppendLine(" , ISNULL(Movimenti_Ordini.Doc_Numero_Sin, '') AS Doc_Numero_Sin_Ordine ")
                StrSQL.AppendLine(" , ISNULL(Movimenti_Ordini.Doc_Numero, '') AS Doc_Numero_Ordine ")
                StrSQL.AppendLine(" , ISNULL(Movimenti_Ordini.Doc_Numero_Des, '') AS Doc_Numero_Des_Ordine ")
            End If

        End If

        ' Colonne Aggiuntive Info Creazione/Modifica
        StrSQL.AppendLine(" ,ag.Data_Creazione ")
        StrSQL.AppendLine(" ,ag.Data_Modifica ")
        If String.IsNullOrEmpty(NomeDB_Utenti) Then
            StrSQL.AppendLine(" ,ag.Username_Creazione Utente_Creazione ")
            StrSQL.AppendLine(" ,ag.Username_Modifica Utente_Modifica ")
        Else
            StrSQL.AppendLine(" ,ISNULL((UtentiDetIns.Rag_Soc + UtentiDetIns.Cognome + ' ' + UtentiDetIns.Nome), 'N.D.') AS Utente_Creazione ")
            StrSQL.AppendLine(" ,ISNULL((UtentiDetMod.Rag_Soc + UtentiDetMod.Cognome + ' ' + UtentiDetMod.Nome), 'N.D.') AS Utente_Modifica ")
        End If

        ' Colonne Aggiuntive Gestione Workflow
        If GestioneWorkflowDocContabili Then
            StrSQL.AppendLine(" ,ISNULL(ag.Pratica_Cod, 0) AS Pratica_Cod ")
            StrSQL.AppendLine(" ,ISNULL(Pratiche.Servizio_Cod, 0) AS Pratica_Servizio_Cod")
            StrSQL.AppendLine(" ,ISNULL(praticheStatiAtt.Stato_Cod, 0) AS Pratica_Stato_Cod")
            StrSQL.AppendLine(" ,ISNULL(wAnagrStati.WAnagraficaStati_Des, '') AS Pratica_Stato_Des")
            StrSQL.AppendLine(" ,ISNULL(praticheStatiAtt.Note, '') AS Pratica_Stato_Note")
        End If

        ' Colonne Aggiuntive Gruppi Merce
        If GestioneGruppiMerce And dettaglio Then

            StrSQL.AppendLine(" ,CASE ")
            StrSQL.AppendLine(" WHEN grpMerce.Codice IS NOT NULL THEN grpMerce.Codice + ' ' + grpMerce.Descrizione ")
            StrSQL.AppendLine(" WHEN #DefaultGruppiMerce.Elem_Cod IS NOT NULL THEN #DefaultGruppiMerce.Codice + ' ' + #DefaultGruppiMerce.Descrizione COLLATE DATABASE_DEFAULT ")
            StrSQL.AppendLine(" ELSE '' END AS Gruppi_Merce ")

        End If

        '--- FROM ---

        If _modalitaFatturazione AndAlso Not dettaglio Then
            StrSQL.AppendLine(" From Agenda_Cte As ag WITH (nolock) ")
        Else
            StrSQL.AppendLine(" FROM Agenda AS ag WITH (nolock) ")
        End If

        StrSQL.AppendLine(" INNER JOIN Movimenti AS movNrDDT WITH (nolock) ON ag.PIVA = movNrDDT.PIVA AND ag.Id_Agenda = movNrDDT.Id_Agenda AND movNrDDT.Cau_Mov = '4000' ")
        StrSQL.AppendLine(" LEFT JOIN Movimenti as movNrBolla WITH (nolock) ON ag.PIVA = movNrBolla.PIVA AND ag.Id_Agenda = movNrBolla.Id_Agenda AND movNrBolla.Cau_Mov = '4050' ")
        StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u WITH (nolock) ON movNrDDT.Cod_RisUm = r_u.Cod_RisUm ")
        StrSQL.AppendLine(" LEFT JOIN Contatti AS cont WITH (nolock) ON r_u.Piva = cont.Piva AND r_u.Cod_Contatto = cont.Cod_Contatto ")

        If presentiDestinazioni Then
            StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_destinazione WITH (nolock) ON movNrDDT.Cod_Destinazione = r_u_destinazione.Cod_RisUm ")
            StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_destinazione WITH (nolock) ON r_u_destinazione.Piva = cont_destinazione.Piva AND r_u_destinazione.Cod_Contatto = cont_destinazione.Cod_Contatto ")

            StrSQL.AppendLine(" LEFT JOIN Indirizzi as indirizzi_dest WITH (nolock) ON movNrDDT.Cod_IndirizzoDestinazione = indirizzi_dest.cod_indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN ContattiXIndirizzi as cont_indirizzi_dest WITH (nolock) ON cont_destinazione.Piva = cont_indirizzi_dest.Piva And cont_destinazione.Cod_Contatto = cont_indirizzi_dest.Cod_Contatto  And indirizzi_dest.cod_indirizzo = cont_indirizzi_dest.Cod_Indirizzo  ")
            StrSQL.AppendLine(" LEFT JOIN IndirizzoTipo as IndirizzoTipo_dest WITH (nolock) ON IndirizzoTipo_dest.Piva = cont_indirizzi_dest.Piva And IndirizzoTipo_dest.IndirizzoTipo_Cod = cont_indirizzi_dest.Tipo_Indirizzo And IndirizzoTipo_dest.Cod_Contatto = cont_indirizzi_dest.Cod_Contatto  ")
        End If

        If presentiVettori Then
            StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_vettore WITH (nolock) ON movNrDDT.Cod_Vettore = r_u_vettore.Cod_RisUm ")
            StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_vettore WITH (nolock) ON r_u_vettore.Piva = cont_vettore.Piva AND r_u_vettore.Cod_Contatto = cont_vettore.Cod_Contatto ")
        End If

        If presentiVettori Or presentiAgenti Or presentiCapiArea Then
            StrSQL.AppendLine(" LEFT JOIN Mov_Dettaglio_Tecnico_Extra AS Movimento_Extra_0 WITH (nolock) ON Movimento_Extra_0.Piva = ag.PIVA AND Movimento_Extra_0.Id_Agenda = ag.Id_Agenda AND Movimento_Extra_0.Id_Mov_Det = 0 ")

            If presentiAgenti Then
                StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_agente WITH (nolock) ON Movimento_Extra_0.Agente_Cod = r_u_agente.Cod_RisUm ")
                StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_agente WITH (nolock) ON r_u_agente.Piva = cont_agente.Piva AND r_u_agente.Cod_Contatto = cont_agente.Cod_Contatto ")
            End If

            If presentiCapiArea Then
                StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_capoarea WITH (nolock) ON Movimento_Extra_0.CapoArea_Cod = r_u_capoarea.Cod_RisUm ")
                StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_capoarea WITH (nolock) ON r_u_capoarea.Piva = cont_capoarea.Piva AND r_u_capoarea.Cod_Contatto = cont_capoarea.Cod_Contatto ")
            End If
        End If

        If presentiCessionari1 Then
            StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_cess1 WITH (nolock) ON movNrDDT.Cod_RisUm_Altro = r_u_cess1.Cod_RisUm ")
            StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_cess1 WITH (nolock) ON r_u_cess1.Piva = cont_cess1.Piva And r_u_cess1.Cod_Contatto = cont_cess1.Cod_Contatto ")
        End If
        If presentiCessionari2 Then
            StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u_cess2 WITH (nolock) ON movNrDDT.Extra_Int= r_u_cess2.Cod_RisUm ")
            StrSQL.AppendLine(" LEFT JOIN Contatti AS cont_cess2 WITH (nolock) ON r_u_cess2.Piva = cont_cess2.Piva And r_u_cess2.Cod_Contatto = cont_cess2.Cod_Contatto ")
        End If

        ' Join Aggiuntive Info Creazione/Modifica
        If Not String.IsNullOrEmpty(NomeDB_Utenti) Then
            StrSQL.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli UtentiDetIns ON UtentiDetIns.CodFisc = ag.Username_Creazione ")
            StrSQL.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli UtentiDetMod ON UtentiDetMod.CodFisc = ag.Username_Modifica ")
        End If

        ' Join Aggiuntive Gestione Workflow
        If GestioneWorkflowDocContabili Then
            StrSQL.AppendLine(" LEFT JOIN Pratiche WITH (nolock) ON Pratiche.Pratica_Cod = ag.Pratica_Cod AND Pratiche.Piva = ag.Piva")
            StrSQL.AppendLine(" LEFT JOIN Pratiche_Stati_Attuali as praticheStatiAtt WITH (nolock) ON praticheStatiAtt.Pratica_Cod = ag.Pratica_Cod AND praticheStatiAtt.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine(" LEFT JOIN WAnagraficaStati as wAnagrStati WITH (nolock) ON wAnagrStati.WAnagraficaStati_Cod = praticheStatiAtt.Stato_Cod ")
        End If

        If dettaglio Then
            StrSQL.AppendLine(" LEFT JOIN Rapporti_Contabili AS r_c WITH (nolock) on r_u.Cod_Rapporto = r_c.Cod_Rapporto ")
            StrSQL.AppendLine(" INNER JOIN Movimenti AS mov WITH (nolock) ON ag.PIVA = mov.PIVA AND ag.Id_Agenda = mov.Id_Agenda ")
            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli AS mov_det WITH (nolock) ")
            StrSQL.AppendLine("     ON ag.PIVA = mov_det.PIVA And mov.Id_Agenda = mov_det.Id_Agenda And mov.Id_Mov = mov_det.Id_Mov And mov.Cau_Mov IN ('7300','7350', '7920') ")
            StrSQL.AppendLine("     AND mov_det.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(clausaInCentri) & ") ")

            'Join per ricavare il valore (-1 o 1) per cui moltiplicare imponibile ed iva
            StrSQL.AppendLine(" LEFT JOIN #Segni_ImpIva ON Cau_Mov_S = mov.Cau_Mov COLLATE DATABASE_DEFAULT AND (Lav_Cod_S = ag.Lav_Cod OR Lav_Cod_S IS NULL)")
            StrSQL.AppendLine(" LEFT JOIN IVA_Aliquote ON IVA_Aliquote.Codice = mov_det.Cod_Iva")

            Dim listTupleOrdiniMovDetRif As New List(Of Tuple(Of Integer, Integer))() From {
                New Tuple(Of Integer, Integer)(LAVCOD_BOLLA_EMESSA, LAVCOD_FATTURA_EMESSA),
                New Tuple(Of Integer, Integer)(LAVCOD_MVV_EMESSO, LAVCOD_FATTURA_EMESSA),
                New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_FATTURA_EMESSA),
                New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_BOLLA_EMESSA),
                New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_VENDITA, LAVCOD_MVV_EMESSO),
                New Tuple(Of Integer, Integer)(LAVCOD_BOLLA_RICEVUTA, LAVCOD_FATTURA_RICEVUTA),
                New Tuple(Of Integer, Integer)(LAVCOD_MVV_RICEVUTO, LAVCOD_FATTURA_RICEVUTA),
                New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_FATTURA_RICEVUTA),
                New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_BOLLA_RICEVUTA),
                New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_MVV_RICEVUTO),
                New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_ACCETTAZIONE_DIVERSI),
                New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE),
                New Tuple(Of Integer, Integer)(LAVCOD_ORDINE_ACQUISTO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE)
            }

            Dim listClausoleOrdiniMovDetRif As New List(Of String)
            For Each tupleOrdiniMovDetRif In listTupleOrdiniMovDetRif
                listClausoleOrdiniMovDetRif.Add(String.Format("Lav_Cod_Rif = {0} AND Lav_Cod = {1}", tupleOrdiniMovDetRif.Item1, tupleOrdiniMovDetRif.Item2))
            Next
            Dim strClausoleOrdiniMovDetRif = "(" & String.Join(")" & vbCrLf & "               OR (", listClausoleOrdiniMovDetRif) & ")"

            StrSQL.AppendLine(" LEFT JOIN (SELECT a.id_mov_det_rif, sum(a.qta) as Qta_Evasa, avg(b.qta) as Qta_Originale, avg(b.Qta)-sum(a.Qta) as Qta_Residua, ")
            StrSQL.AppendLine("                   avg(b.Qta)-sum(a.Qta) as Qta, avg(b.Qta_Extra_Totale)-sum(c.Qta_Extra_Totale) AS Qta_Extra_Totale, avg(b.Tara)-sum(c.Tara) as Tara, ")
            StrSQL.AppendLine("                   avg(b.Qta_Dettaglio1)-sum(c.Qta_Dettaglio1) AS Qta_Dettaglio1, avg(b.Qta_Dettaglio2)-sum(c.Qta_Dettaglio2) AS Qta_Dettaglio2, ")
            StrSQL.AppendLine("                   avg(b.imponibile)-sum(c.imponibile) as imponibile, ")
            StrSQL.AppendLine("                   avg(b.imponibile_netto)-sum(c.imponibile_netto) as imponibile_netto, ")
            StrSQL.AppendLine("                   avg(b.iva)-sum(c.iva) as iva ")

            StrSQL.AppendLine("            FROM mov_dettagli_riferimenti a ")
            StrSQL.AppendLine("            INNER JOIN movimenti_dettagli b ON a.id_mov_det_rif = b.id_mov_det ")
            StrSQL.AppendLine("            INNER JOIN movimenti_dettagli c ON a.id_mov_det = c.id_mov_det ")
            StrSQL.AppendLine("            WHERE Lav_Cod < 1000  ")
            StrSQL.AppendLine("              OR " & strClausoleOrdiniMovDetRif)
            StrSQL.AppendLine("            GROUP BY a.id_mov_det_rif ")
            'StrSQL.AppendLine("            HAVING sum(a.qta) < avg(b.qta) ")
            StrSQL.AppendLine("            ) AS mov_det_rif ON mov_det_rif.Id_Mov_Det_Rif = mov_det.Id_Mov_Det ")
            StrSQL.AppendLine(" LEFT JOIN (SELECT Piva, Sa_Cod, Elem_Cod, Mat_Cod, 0 AS Pro_Cod, Mat_Des, Cod_Articolo, Codice_Esterno, Veg_Cod, Cul_Cod, Cat_Cod FROM Materie_Prime WITH (nolock) ")
            StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 3 AS Elem_Cod, 0 AS Mat_Cod, Fer_Cod AS Pro_Cod, Fer_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Fertilizzanti WITH (nolock) ")
            StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 10 AS Elem_Cod, 0 AS Mat_Cod, Sem_Cod AS Pro_Cod, Sem_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM TipologieSementi WITH (nolock) ")
            StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 191 AS Elem_Cod, 0 AS Mat_Cod, Fr_Cod AS Pro_Cod, Fr_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Formulati WITH (nolock) ")
            StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 195 AS Elem_Cod, 0 AS Mat_Cod, Coad_Cod AS Pro_Cod, Coad_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Coadiuvante WITH (nolock) ")
            StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 196 AS Elem_Cod, 0 AS Mat_Cod, Ins_Cod AS Pro_Cod, Ins_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM InsettiUtili WITH (nolock) ")
            StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 197 AS Elem_Cod, 0 AS Mat_Cod, Trap_Cod AS Pro_Cod, Trap_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Trappole WITH (nolock) ")
            StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 198 AS Elem_Cod, 0 AS Mat_Cod, Av_Cod AS Pro_Cod, Av_Des_Vol AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Avversita WITH (nolock) ")
            StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 555 AS Elem_Cod, 0 AS Mat_Cod, cast(replace(COD,'S','') as int) AS Pro_Cod, DESCR AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Categorie WITH (nolock) WHERE COD LIKE 'S%' ")
            StrSQL.AppendLine("            ) AS mat_prima ON (mov_det.PIVA = ISNULL(mat_prima.Piva,mov_det.PIVA) OR mat_prima.Sa_Cod = -1) AND mov_det.Elem_Cod = mat_prima.Elem_Cod AND mov_det.Mat_Cod = mat_prima.Mat_Cod AND mov_det.Pro_Cod = mat_prima.Pro_Cod")
            StrSQL.AppendLine(" LEFT JOIN CategorieMagazzino AS cat_mag WITH (nolock) ON cat_mag.Elem_Cod = mov_det.Elem_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Linee_Classi_Produzioni AS cat_com WITH (nolock) ON cat_com.Linea_Classe_Cod = mat_prima.Cat_Cod ")
            StrSQL.AppendLine(" LEFT JOIN Mov_Dettaglio_Tecnico_Extra AS Movimento_Extra_Dettagli WITH (nolock) ON Movimento_Extra_Dettagli.Piva = ag.PIVA AND Movimento_Extra_Dettagli.Id_Agenda = ag.Id_Agenda AND Movimento_Extra_Dettagli.Id_Mov_Det = mov_det.Id_Mov_Det ")
            StrSQL.AppendLine(" LEFT JOIN unitamisura WITH (nolock) ON mov_det.udm_cod = unitamisura.udm_cod ")
            StrSQL.AppendLine(" LEFT JOIN unitamisura AS udm_sec WITH (nolock) ON mov_det.udm_cod_extra = udm_sec.udm_cod ")
            StrSQL.AppendLine(" LEFT JOIN Indirizzi WITH (nolock) ON movNrDDT.Cod_IndirizzoRisUm = Indirizzi.cod_indirizzo ")
            StrSQL.AppendLine(" LEFT JOIN Lista_Province WITH (nolock) ON Indirizzi.pro_cod_istat = Lista_Province.PROV ")
            StrSQL.AppendLine(" LEFT JOIN Lista_Regioni WITH (nolock) ON Lista_Province.REG = Lista_Regioni.REG ")
            StrSQL.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 AS Lista_Nazioni WITH (nolock) ON Indirizzi.stato = Lista_Nazioni.Codice ")

            If presentiDestinazioni Then
                StrSQL.AppendLine(" LEFT JOIN Lista_Province as province_dest WITH (nolock) ON indirizzi_dest.pro_cod_istat = province_dest.PROV ")
                StrSQL.AppendLine(" LEFT JOIN Lista_Regioni as regioni_dest WITH (nolock) ON province_dest.REG = regioni_dest.REG ")
                StrSQL.AppendLine(" LEFT JOIN ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 AS nazioni_dest WITH (nolock) ON indirizzi_dest.stato = nazioni_dest.Codice ")
            End If

            If tipoValore = "4" OrElse tipoValore = "5" Then
                StrSQL.AppendLine(" LEFT JOIN Materie_Prime AS m_p WITH (nolock) ON m_p.piva = mov_det.piva And  m_p.Elem_Cod = mov_det.Elem_Cod And  m_p.Mat_Cod = mov_det.Mat_Cod ")
            End If

            ' Join x Parametri Qualitativi
            If DTParamQual IsNot Nothing Then
                For Each paramQual In DTParamQual.Rows
                    StrSQL.AppendLine(" LEFT JOIN Materie_Prime_Campionature AS Materie_Prime_Campionature_" + paramQual("Tabella_Key") +
                        " WITH (nolock) ON mov_det.Cal_Cod = Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Progressivo " +
                        " And Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Tipo = 'o" + paramQual("Tabella_Key") + "'")
                    StrSQL.AppendLine(" LEFT JOIN OTabelle_Parametri AS OTabelle_Parametri_" + paramQual("Tabella_Key") +
                        " WITH (nolock) ON Materie_Prime_Campionature_" + paramQual("Tabella_Key") + ".Tipo_Cod =  OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Tabella_Par_Cod " +
                        " AND OTabelle_Parametri_" + paramQual("Tabella_Key") + ".Tabella_Cod = '" + paramQual("Tabella_ID") + "'")
                Next
            End If

            StrSQL.AppendLine(" LEFT JOIN SpecieVegetali AS sv WITH (nolock) ON sv.veg_cod = mat_prima.veg_cod ")
            StrSQL.AppendLine(" LEFT JOIN Cultivar AS cul WITH (nolock) ON cul.Cul_Cod = mat_prima.Cul_Cod ")

            ' Join Aggiuntive Gruppi Merce
            If GestioneGruppiMerce Then
                StrSQL.AppendLine(" LEFT JOIN Prodotti_Extra_Privata as prodExtraPriv WITH (nolock)")
                StrSQL.AppendLine("              ON prodExtraPriv.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
                StrSQL.AppendLine("       AND prodExtraPriv.Elem_Cod = mov_det.Elem_Cod")
                StrSQL.AppendLine("       AND prodExtraPriv.Mat_Cod = mov_det.Mat_Cod")
                StrSQL.AppendLine("       AND prodExtraPriv.Pro_Cod = mov_det.Pro_Cod")
                StrSQL.AppendLine("             AND (prodExtraPriv.Piva = mov_det.Piva OR mov_det.Pro_Cod = 0)")
                StrSQL.AppendLine(" LEFT JOIN Gruppi_Merce as grpMerce WITH (nolock)")
                StrSQL.AppendLine("              ON grpMerce.Id_Gruppo_Merce = prodExtraPriv.Id_Gruppo_Merce")
                StrSQL.AppendLine(" LEFT JOIN #DefaultGruppiMerce")
                StrSQL.AppendLine("              ON #DefaultGruppiMerce.Elem_Cod = mov_det.Elem_Cod")
            End If

            If {DocContab_TipoRicerca_Acquisti, DocContab_TipoRicerca_Conferimenti, DocContab_TipoRicerca_Vendite}.Contains(type) AndAlso
                    {DocContab_TipoDoc_Consegna, DocContab_TipoDoc_Fattura}.Contains(doc_type) Then

                StrSQL.AppendLine(" left join Mov_Dettagli_Riferimenti Riferimenti_Ordini ")
                StrSQL.AppendLine(" on Riferimenti_Ordini.Piva = mov_det.PIVA")
                StrSQL.AppendLine(" and Riferimenti_Ordini.Id_Agenda = mov_det.Id_Agenda")
                StrSQL.AppendLine(" and Riferimenti_Ordini.Id_Mov = mov_det.Id_Mov")
                StrSQL.AppendLine(" and Riferimenti_Ordini.Id_Mov_Det = mov_det.Id_Mov_Det")
                StrSQL.AppendLine(" and Riferimenti_Ordini.Lav_Cod_Rif IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", {LAVCOD_ORDINE_ACQUISTO, LAVCOD_ORDINE_VENDITA}.ToArray())) & ")")
                StrSQL.AppendLine(" left join Movimenti Movimenti_Ordini")
                StrSQL.AppendLine(" on Movimenti_Ordini.PIVA = Riferimenti_Ordini.PIVA_Rif")
                StrSQL.AppendLine(" and Movimenti_Ordini.Id_Agenda = Riferimenti_Ordini.Id_Agenda_Rif")
                StrSQL.AppendLine(" and Movimenti_Ordini.Id_Mov = Riferimenti_Ordini.Id_Mov_Rif")
            End If

        Else
            'Se Testata

            If _modalitaFatturazione Then
                StrSQL.AppendLine(" Left Join movimenti_dettagli_CTE as mov_det WITH (nolock) on mov_det.Id_Agenda = ag.Id_Agenda ")
            Else
                StrSQL.AppendLine(" LEFT JOIN( ")
                StrSQL.AppendLine("     SELECT md.piva, md.id_agenda, md.sa_cod, ")
                StrSQL.AppendLine("     ROW_NUMBER() OVER(PARTITION BY md.piva, md.id_agenda ORDER BY md.sa_cod desc) AS rn ")
                StrSQL.AppendLine("     FROM movimenti_dettagli md ")
                StrSQL.AppendLine("     INNER JOIN agenda ag ON md.piva = ag.piva ")
                StrSQL.AppendLine("     WHERE ag.PIVA = '" & Agro_SQL_SaveText(piva) & "' AND ag.id_agenda = md.id_agenda ")
                StrSQL.AppendLine(" )mov_det on mov_det.Id_Agenda = ag.Id_Agenda ")
                StrSQL.AppendLine(" And ag.PIVA = mov_det.piva ")
                StrSQL.AppendLine(" And mov_det.sa_cod In (" & Agro_SQL_Save_Clausola_IN(clausaInCentri) & ") ")
            End If

        End If

        StrSQL.AppendLine(" LEFT JOIN centri_aziendali As centri ON mov_det.PIVA = centri.PIVA AND mov_det.Sa_Cod = centri.sa_cod ")

        If UtenteAbilitatoGestioneVisualizaAllegato = True Then
            StrSQL.AppendLine(" Left Join #documenti_Cte doc on ag.PIVA = doc.piva collate Latin1_General_CI_AS  And ag.Id_Agenda = doc.Id_agenda ")
        End If

        '--- WHERE ---

        ' Filtri Principali

        StrSQL.AppendLine(" WHERE 1 = 1 ")

        ' **********************************************************************************************
        ' Alcuni Filtri, in modalità fatturazione, vengono applicati nelle CTE all'inizio
        ' **********************************************************************************************
        Dim filtriNormali As Boolean = False
        If dettaglio Then
            filtriNormali = True
        Else
            If Not _modalitaFatturazione Then
                filtriNormali = True
            End If
        End If

        If filtriNormali Then

            If Not String.IsNullOrEmpty(piva) Then
                StrSQL.AppendLine(" And (ag.PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
            End If

            If type.ToUpper = "C" AndAlso doc_type.ToUpper = "C" Then
                'Campo riciclato per scartare i conferimenti fatti col LAN
                StrSQL.AppendLine(" AND ag.split <> " & Agro_SQL_SaveNum(1) & " ")
            End If

            If Not String.IsNullOrEmpty(_descrizione) Then
                StrSQL.AppendLine(" AND (ag.des_lib LIKE '%" & Agro_SQL_SaveText(_descrizione) & "%') ")
            End If

            StrSQL.AppendLine(" AND ( ag.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(Elenco_LavCod_In) & ") ) ")

            If _docNumero <> 0 Then
                StrSQL.AppendLine(" AND (Doc_Numero = " & Agro_SQL_SaveNum(_docNumero) & " ) ")
            End If

            If Not String.IsNullOrEmpty(_docNumeroSin) Then
                StrSQL.AppendLine(" AND (Doc_Numero_Sin = '" & Agro_SQL_SaveText(_docNumeroSin) & "' ) ")
            End If

            If Not String.IsNullOrEmpty(_docNumeroDes) Then
                StrSQL.AppendLine(" AND (Doc_Numero_Des = '" & Agro_SQL_SaveText(_docNumeroDes) & "' ) ")
            End If

            If Not String.IsNullOrEmpty(_dataMovDal) Then
                StrSQL.AppendLine(" AND ( movNrDDT.Data_Movimento >= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataMovDal)) & " ) ")
            End If

            If Not String.IsNullOrEmpty(_dataMovAl) Then
                StrSQL.AppendLine(" AND ( movNrDDT.Data_Movimento <= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataMovAl)) & " ) ")
            End If

        End If

        If Not String.IsNullOrEmpty(_clienti) Then
            If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                StrSQL.AppendLine(" AND (cont.Cod_Contatto IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_clienti, "|", "','"), True) & "') ) ")
            Else
                If DataProviderFactory.Instance.ParametrizzaQuery Then
                    StrSQL.AppendLine(" AND (cont.Cod_Contatto IN (" & Agro_SQL_Save_Clausola_IN(Replace(_clienti, "|", "','"), True) & ") ) ")
                Else
                    StrSQL.AppendLine(" AND (cont.Cod_Contatto IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_clienti, "|", "','"), True) & "') ) ")
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(_agenti) Then
            If presentiAgenti Then
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    StrSQL.AppendLine(" AND (cont_agente.Cod_Contatto IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_agenti, "|", "','"), True) & "') ) ")
                Else
                    If DataProviderFactory.Instance.ParametrizzaQuery Then
                        StrSQL.AppendLine(" AND (cont_agente.Cod_Contatto IN (" & Agro_SQL_Save_Clausola_IN(Replace(_agenti, "|", "','"), True) & ") ) ")
                    Else
                        StrSQL.AppendLine(" AND (cont_agente.Cod_Contatto IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_agenti, "|", "','"), True) & "') ) ")
                    End If
                End If
            Else
                ' La condizione torna comunque False, in ogni caso non esistono agenti da riportare 
                If DataProviderFactory.Instance.TipoProvider = enum_DataProvidersType.OleDbProvider Then
                    StrSQL.AppendLine(" AND ('0' IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_agenti, "|", "','"), True) & "') ) ")
                Else
                    If DataProviderFactory.Instance.ParametrizzaQuery Then
                        StrSQL.AppendLine(" AND ('0' IN (" & Agro_SQL_Save_Clausola_IN(Replace(_agenti, "|", "','"), True) & ") ) ")
                    Else
                        StrSQL.AppendLine(" AND ('0' IN ('" & Agro_SQL_Save_Clausola_IN(Replace(_agenti, "|", "','"), True) & "') ) ")
                    End If
                End If
            End If
        End If

        If filtriNormali Then
            If Not String.IsNullOrEmpty(_causali_trasp) Then
                Dim listaCausali = "0, " & Replace(_causali_trasp, "|", ",")
                StrSQL.Append(" AND (movNrDDT.Causale_Trasporto_Cod IN (" & Agro_SQL_Save_Clausola_IN(listaCausali, False) & ")) ")
            End If
        End If

        If (dettaglio) Then

            If Not String.IsNullOrEmpty(_nrRiga) Then
                StrSQL.AppendLine(" AND (Ordine_Det = " & Agro_SQL_SaveNum(_nrRiga) & " ) ")
            End If

            If Not String.IsNullOrEmpty(_specie) Then
                StrSQL.AppendLine(" AND (mat_prima.Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_specie, "|", ",")) & ") ) ")
            End If

            If Not String.IsNullOrEmpty(_varieta) Then
                StrSQL.AppendLine(" AND (mat_prima.Cul_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_varieta, "|", ",")) & ") ) ")
            End If

            If Not String.IsNullOrEmpty(_categorie) Then
                StrSQL.AppendLine(" AND (mov_det.Elem_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_categorie, "|", ",")) & ") ) ")
            End If

            ' Filtro Categorie Commerciali  TODO
            If Not String.IsNullOrEmpty(_categcommerciali) Then
                StrSQL.AppendLine(" AND (Cat_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_categcommerciali, "|", ",")) & ") ) ")
            End If

            If Not String.IsNullOrEmpty(_prodotti) Then
                StrSQL.Append(" AND ( ")
                Dim elenco As New System.Text.StringBuilder("")
                Dim prodArray As String() = _prodotti.Split("|")
                For Each p In prodArray
                    If Not elenco.ToString() = "" Then
                        elenco.Append(" OR ")
                    End If
                    elenco.Append(" ( ")
                    Dim dueParti As String() = p.Split("_")
                    elenco.Append(" mov_det.Elem_Cod =  " & CInt(dueParti(0)) & " AND ")

                    If CInt(dueParti(1)) > 0 Then
                        elenco.Append(" mov_det.Pro_Cod =  " & CInt(dueParti(1)))
                    Else
                        elenco.Append(" mov_det.Mat_Cod =  " & CInt(dueParti(1)) * -1)
                    End If

                    elenco.Append(" ) ")
                Next
                StrSQL.Append(elenco.ToString())
                StrSQL.Append(" ) ")
            End If

            ' scarto gli imballi dalle righe DDT (da verificare)
            ' TODO ?????
            StrSQL.AppendLine("  AND mov_det.Ordine_Det <> 1000 ")
            'StrSQL.AppendLine(" AND (mov_det.Mat_Cod <> 0 OR mov_det.Pro_Cod <> 0 OR mov_det.Elem_Cod = 501) ")
            'StrSQL.AppendLine(" AND (mat_prima.Elem_Cod <> 205 OR ag.Lav_Cod NOT IN (" & LAVCOD_BOLLA_EMESSA & ", " & LAVCOD_MVV_EMESSO & ")) ")

            'StrSQL.AppendLine(" AND (mov_det.Contabilizzato NOT IN (3,-3) OR ag.Lav_Cod NOT IN (" & LAVCOD_ORDINE_VENDITA & ")) ")

            If _soloOrdiniNonSpediti Then
                ' escludo le righe ordine contabilizzate
                StrSQL.AppendLine(" AND (mov_det.Contabilizzato NOT IN (3,-3) AND ag.Lav_Cod IN (" & LAVCOD_ORDINE_VENDITA & "," & LAVCOD_ORDINE_ACQUISTO & ")) ")

                ' Filtro per escludere righe dei documenti collegato ad eccezione di quelle che hanno dei residui
                StrSQL.AppendLine(" AND (mov_det.Id_Mov_Det NOT IN ( ")
                StrSQL.AppendLine("         SELECT Id_Mov_Det_Rif ")
                StrSQL.AppendLine("         FROM Mov_Dettagli_Riferimenti ")
                StrSQL.AppendLine("         WHERE (Lav_Cod_Rif = " & LAVCOD_ORDINE_VENDITA & " AND Lav_Cod = " & LAVCOD_FATTURA_EMESSA & ") ")
                StrSQL.AppendLine("            OR (Lav_Cod_Rif = " & LAVCOD_ORDINE_VENDITA & " AND Lav_Cod = " & LAVCOD_BOLLA_EMESSA & ") ")
                StrSQL.AppendLine("            OR (Lav_Cod_Rif = " & LAVCOD_ORDINE_VENDITA & " AND Lav_Cod = " & LAVCOD_MVV_EMESSO & ") ")
                StrSQL.AppendLine("     ) ")
                StrSQL.AppendLine("     OR mov_det.Id_Mov_Det IN ( ")
                StrSQL.AppendLine("         SELECT a.id_mov_det_rif ")
                StrSQL.AppendLine("         FROM mov_dettagli_riferimenti a ")
                StrSQL.AppendLine("         INNER JOIN movimenti_dettagli b ON a.id_mov_det_rif = b.id_mov_det ")
                StrSQL.AppendLine("         WHERE (Lav_Cod_Rif = " & LAVCOD_ORDINE_VENDITA & " AND Lav_Cod = " & LAVCOD_FATTURA_EMESSA & ") ")
                StrSQL.AppendLine("            OR (Lav_Cod_Rif = " & LAVCOD_ORDINE_VENDITA & " AND Lav_Cod = " & LAVCOD_BOLLA_EMESSA & ") ")
                StrSQL.AppendLine("            OR (Lav_Cod_Rif = " & LAVCOD_ORDINE_VENDITA & " AND Lav_Cod = " & LAVCOD_MVV_EMESSO & ") ")
                StrSQL.AppendLine("         GROUP BY a.id_mov_det_rif HAVING sum(a.qta) < avg(b.qta) ")
                StrSQL.AppendLine("     ) ")
                StrSQL.AppendLine(" ) ")
            End If

            If _soloDDTNonFatturati Then
                StrSQL.AppendLine(" AND (mov_det.Id_Mov_Det NOT IN ( ")
                StrSQL.AppendLine("         SELECT Id_Mov_Det_Rif ")
                StrSQL.AppendLine("         FROM Mov_Dettagli_Riferimenti ")
                StrSQL.AppendLine("         WHERE (Lav_Cod_Rif = " & LAVCOD_BOLLA_EMESSA & " AND Lav_Cod = " & LAVCOD_FATTURA_EMESSA & ") ")
                StrSQL.AppendLine("            OR (Lav_Cod_Rif = " & LAVCOD_MVV_EMESSO & " AND Lav_Cod = " & LAVCOD_FATTURA_EMESSA & ") ")
                StrSQL.AppendLine("      ) ")
                StrSQL.AppendLine("      OR mov_det.Id_Mov_Det IN ( ")
                StrSQL.AppendLine("         SELECT a.id_mov_det_rif ")
                StrSQL.AppendLine("         FROM mov_dettagli_riferimenti a ")
                StrSQL.AppendLine("         INNER JOIN movimenti_dettagli b ON a.id_mov_det_rif = b.id_mov_det ")
                StrSQL.AppendLine("         WHERE (Lav_Cod_Rif = " & LAVCOD_BOLLA_EMESSA & " AND Lav_Cod = " & LAVCOD_FATTURA_EMESSA & ") ")
                StrSQL.AppendLine("            OR (Lav_Cod_Rif = " & LAVCOD_MVV_EMESSO & " AND Lav_Cod = " & LAVCOD_FATTURA_EMESSA & ") ")
                StrSQL.AppendLine("         GROUP BY a.id_mov_det_rif HAVING sum(a.qta) < avg(b.qta) ")
                StrSQL.AppendLine("      ) ")
                StrSQL.AppendLine(" ) ")
            End If

        Else
            If _modalitaFatturazione Then
                StrSQL.AppendLine(" And mov_det.rn = 1 ) qp ")
                StrSQL.AppendLine(" where (Mov_Non_Fatturati <> '' or Mov_Fatturati_Parzialmente <> '') ")
            Else
                StrSQL.AppendLine(" AND mov_det.rn = 1 ")
            End If
        End If

        If GestioneGruppiUtenteMerce = True AndAlso objParametri.UtenteUsername <> objParametri.SuperUserUsername Then

            If dettaglio = False Then
                'Nella visualizzazione di testata, mostro solo i documenti che hanno almeno un dettaglio visibile al gruppo utente

                Dim aliasTabAgenda = If(_modalitaFatturazione = False, "ag", "qp")

                StrSQL.AppendLine("and 0 < (")
                StrSQL.AppendLine(objGruppiUtenteMerce.ComponiSql_ContaDettagli_X_GruppoUtente(aliasTabAgenda, piva, 0, True, "mov_det.ordine_det <> 1000 AND mov_det.Elem_Cod <> " & RIGA_DESCRIZIONE))
                StrSQL.AppendLine(")")
            Else
                'Nella visualizzazione di dettaglio, mostro tutti i dettagli visibili al gruppo utente, se un documento ha più dettagli, di cui solo un sottoinsieme visibile,
                'verranno mostrati solo questi

                StrSQL.AppendLine(" AND COALESCE(prodExtraPriv.Id_Gruppo_Merce, #DefaultGruppiMerce.Id_Gruppo_Merce, 0) IN (")
                StrSQL.AppendLine(objGruppiUtenteMerce.ComponiSql_DistinctGruppiMerce_X_GruppiUtente("", piva))
                StrSQL.AppendLine(" )")
            End If

        End If

        If Not IsNothing(top_N_rows) AndAlso top_N_rows.HasValue Then
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
        End If

        If LivelloCompatibilita(objParametri) >= 150 Then

            StrSQL.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")

        End If


        Return StrSQL.ToString

    End Function
    Private Function ComponiQueryRicercaPerPopolamentoWidget(ByVal type As String,
                                                        ByVal doc_Type As String,
                                                        ByVal tuttiICentri As IEnumerable(Of Object),
                                                        ByVal piva As String,
                                                        ByVal _causali As String,
                                                        ByVal _centriAziendali As String,
                                                        Optional ByVal top_N_rows As Integer? = Nothing
                                                        ) As String

        Dim StrSQL As New StringBuilder
        Dim clausaInCentri As String = "0"
        Dim Elenco_LavCod_In = Componi_Elenco_LavCod_In(_causali, type, doc_Type)

        'Giulia 25/01/2021: gli ordine potrebbero non avere anche associato il centro ed il magazzino, in questo caso il sa_cod è -1, non 0
        If String.IsNullOrEmpty(_centriAziendali) Then
            If (tuttiICentri.Any) Then
                clausaInCentri = "0,-1," & String.Join(",", tuttiICentri.Select(Function(s) s.sa_cod).ToList())
            End If
        Else
            clausaInCentri = "0,-1," & String.Join(",", _centriAziendali.Split("|"))
        End If

        StrSQL.AppendLine("SELECT ")
        If Not IsNothing(top_N_rows) AndAlso top_N_rows.HasValue Then
            StrSQL.AppendLine(" TOP " + top_N_rows.ToString)
        End If
        StrSQL.AppendLine(" ag.PIVA ")
        StrSQL.AppendLine(" ,ag.Lav_Cod ")
        StrSQL.AppendLine(" ,ag.Id_Agenda ")
        StrSQL.AppendLine(" ,mov_det.Sa_Cod ")
        StrSQL.AppendLine(" ,ag.Des_Lib ")
        StrSQL.AppendLine(" , CONVERT(varchar, movNrDDT.Data_Movimento, 103) AS Data_Documento ")
        StrSQL.AppendLine(" , movNrDDT.Doc_Numero_Sin ")
        StrSQL.AppendLine(" , movNrDDT.Doc_Numero ")
        StrSQL.AppendLine(" , movNrDDT.Doc_Numero_Des ")
        StrSQL.AppendLine(" , mov_det.Ordine_Det AS Riga ")
        StrSQL.AppendLine(" , mat_prima.Mat_Des ")
        StrSQL.AppendLine(" , unitamisura.udm_sim AS Unita_Misura_Sigla ")
        StrSQL.AppendLine(" , movNrDDT.Data_Movimento ")
        StrSQL.AppendLine(" , movNrDDT.Doc_Numero_Sin + ")
        StrSQL.AppendLine("     CASE WHEN LEN(LTRIM(STR(movNrDDT.doc_numero,10))) > 5 THEN LTRIM(STR(movNrDDT.doc_numero,10)) ")
        StrSQL.AppendLine("     ELSE REPLICATE('0', 5 - LEN(LTRIM(STR(movNrDDT.doc_numero,10)))) + LTRIM(STR(movNrDDT.doc_numero,10))  END ")
        StrSQL.AppendLine("     + movNrDDT.Doc_Numero_Des AS Numero_Movimento ")
        StrSQL.AppendLine(" ,ISNULL(cont.Rag_Soc, '') + ' ' + ISNULL(cont.Cognome, '') + ' ' + ISNULL(cont.Nome, '')  AS Soggetto_RagioneSociale, ")
        StrSQL.AppendLine(" (CASE WHEN ag.Lav_Cod = " & LAVCOD_NOTA_ACCREDITO_EMESSA & " THEN -1 ELSE 1 END) * ISNULL(mov_det.Qta,mov_det.Qta) AS Qta,")
        StrSQL.AppendLine(" mov_det.Prezzo_Unitario_Netto As Prezzo_Netto ")

        StrSQL.AppendLine(" FROM Agenda AS ag WITH (nolock) ")

        StrSQL.AppendLine(" INNER JOIN Movimenti AS movNrDDT WITH (nolock) ON ag.PIVA = movNrDDT.PIVA AND ag.Id_Agenda = movNrDDT.Id_Agenda AND movNrDDT.Cau_Mov = '4000' ")
        StrSQL.AppendLine(" LEFT JOIN Movimenti as movNrBolla WITH (nolock) ON ag.PIVA = movNrBolla.PIVA AND ag.Id_Agenda = movNrBolla.Id_Agenda AND movNrBolla.Cau_Mov = '4050' ")
        StrSQL.AppendLine(" LEFT JOIN Risorse_Umane AS r_u WITH (nolock) ON movNrDDT.Cod_RisUm = r_u.Cod_RisUm ")
        StrSQL.AppendLine(" LEFT JOIN Contatti AS cont WITH (nolock) ON r_u.Piva = cont.Piva AND r_u.Cod_Contatto = cont.Cod_Contatto ")
        StrSQL.AppendLine(" INNER JOIN Movimenti AS mov WITH (nolock) ON ag.PIVA = mov.PIVA AND ag.Id_Agenda = mov.Id_Agenda ")
        StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli AS mov_det WITH (nolock) ")
        StrSQL.AppendLine("     ON ag.PIVA = mov_det.PIVA And mov.Id_Agenda = mov_det.Id_Agenda And mov.Id_Mov = mov_det.Id_Mov And mov.Cau_Mov IN ('7300','7350', '7920') ")
        StrSQL.AppendLine("     AND mov_det.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(clausaInCentri) & ") ")
        StrSQL.AppendLine(" LEFT JOIN (SELECT Piva, Sa_Cod, Elem_Cod, Mat_Cod, 0 AS Pro_Cod, Mat_Des, Cod_Articolo, Codice_Esterno, Veg_Cod, Cul_Cod, Cat_Cod FROM Materie_Prime WITH (nolock) ")
        StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 3 AS Elem_Cod, 0 AS Mat_Cod, Fer_Cod AS Pro_Cod, Fer_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Fertilizzanti WITH (nolock) ")
        StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 10 AS Elem_Cod, 0 AS Mat_Cod, Sem_Cod AS Pro_Cod, Sem_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM TipologieSementi WITH (nolock) ")
        StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 191 AS Elem_Cod, 0 AS Mat_Cod, Fr_Cod AS Pro_Cod, Fr_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Formulati WITH (nolock) ")
        StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 195 AS Elem_Cod, 0 AS Mat_Cod, Coad_Cod AS Pro_Cod, Coad_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Coadiuvante WITH (nolock) ")
        StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 196 AS Elem_Cod, 0 AS Mat_Cod, Ins_Cod AS Pro_Cod, Ins_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM InsettiUtili WITH (nolock) ")
        StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 197 AS Elem_Cod, 0 AS Mat_Cod, Trap_Cod AS Pro_Cod, Trap_Des AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Trappole WITH (nolock) ")
        StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 198 AS Elem_Cod, 0 AS Mat_Cod, Av_Cod AS Pro_Cod, Av_Des_Vol AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Avversita WITH (nolock) ")
        StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 307 AS Elem_Cod, 0 AS Mat_Cod, Farm_Cod AS Pro_Cod, Denominazione AS AIC, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Farmaci WITH (nolock)  ")
        StrSQL.AppendLine("            UNION ALL SELECT null AS Piva, 0 AS Sa_Cod, 555 AS Elem_Cod, 0 AS Mat_Cod, cast(replace(COD,'S','') as int) AS Pro_Cod, DESCR AS Mat_Des, '' As Cod_Articolo, '' As Codice_Esterno, 0 As Veg_Cod, 0 AS Cul_Cod, 0 AS Cat_Cod FROM Categorie WITH (nolock) WHERE COD LIKE 'S%' ")
        StrSQL.AppendLine("            ) AS mat_prima ON (mov_det.PIVA = ISNULL(mat_prima.Piva,mov_det.PIVA) OR mat_prima.Sa_Cod = -1) AND mov_det.Elem_Cod = mat_prima.Elem_Cod AND mov_det.Mat_Cod = mat_prima.Mat_Cod AND mov_det.Pro_Cod = mat_prima.Pro_Cod")
        StrSQL.AppendLine(" LEFT JOIN unitamisura WITH (nolock) ON mov_det.udm_cod = unitamisura.udm_cod ")

        StrSQL.AppendLine(" WHERE 1 = 1 ")

        If Not String.IsNullOrEmpty(piva) Then
            StrSQL.AppendLine(" And (ag.PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
        End If

        If type.ToUpper = "C" AndAlso doc_Type.ToUpper = "C" Then
            'Campo riciclato per scartare i conferimenti fatti col LAN
            StrSQL.AppendLine(" AND ag.split <> " & Agro_SQL_SaveNum(1) & " ")
        End If

        StrSQL.AppendLine(" AND ( ag.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(Elenco_LavCod_In) & ") ) ")
        StrSQL.AppendLine(" AND mov_det.Ordine_Det <> 1000 ")
        StrSQL.AppendLine(" ORDER BY  Data_Movimento DESC, Numero_Movimento DESC, Riga ASC ")

        Return StrSQL.ToString()
    End Function


    Private Function Componi_Elenco_LavCod_In(ByVal _causali As String,
                                              ByVal type As String,
                                              ByVal doc_type As String) As String

        'Dim Elenco_LavCod_Pipe As String = ""
        Dim Elenco_LavCod_Virgola As String = ""

        If Not String.IsNullOrEmpty(_causali) Then
            Elenco_LavCod_Virgola = Replace(_causali, "|", ",")
        Else
            Elenco_LavCod_Virgola = String.Join(",", Lista_LavCod_Ammessi(type, doc_type).Select(Function(l) l.LAV_COD))
        End If

        Return Elenco_LavCod_Virgola

    End Function

    Private Function LeggiGestioneWorkflowDocContabili(ByVal piva As String,
                                                       ByRef objParametriServer As AgronicaCoreParametri,
                                                       ByVal Elenco_LavCod_In As String) As Boolean

        Dim GestioneWorkflowDocContabili As Boolean = False

        If Not IsNothing(objParametriServer) AndAlso Not String.IsNullOrEmpty(Elenco_LavCod_In) Then

            Dim objImpImp As New Imprese_Impostazioni_R

            Dim dizionarioWorkflowDoc = objImpImp.LeggiDizionario_WorkflowDocContabili(piva, objParametriServer)

            Dim ElencoLavCod() = Elenco_LavCod_In.Split(",")

            For Each LavCod In ElencoLavCod

                If objImpImp.LavCod_Gestisce_Workflow(LavCod, dizionarioWorkflowDoc) Then

                    GestioneWorkflowDocContabili = True

                    Exit For

                End If

            Next

        End If

        Return GestioneWorkflowDocContabili

    End Function

    Private Sub VerificaPresenzaRiferimentiPerMovimenti(ByVal piva As String,
                                                        ByVal dataMovDal As String,
                                                        ByVal dataMovAl As String,
                                                        ByVal clausaInCentri As String,
                                                        ByVal Elenco_LavCod_In As String,
                                                        ByRef objParametriServer As AgronicaCoreParametri,
                                                        ByRef PresentiRiferimentiDestinazione As Boolean,
                                                        ByRef PresentiRiferimentiVettore As Boolean,
                                                        ByRef PresentiRiferimentiCessionario1 As Boolean,
                                                        ByRef PresentiRiferimentiCessionario2 As Boolean,
                                                        ByRef PresentiRiferimentiAgente As Boolean,
                                                        ByRef PresentiRiferimentiCapoArea As Boolean)

        PresentiRiferimentiDestinazione = VerificaPresenzaRiferimentiTipoSpecifico("Cod_Destinazione", piva, dataMovDal, dataMovAl, clausaInCentri, Elenco_LavCod_In, False, objParametriServer)
        PresentiRiferimentiVettore = VerificaPresenzaRiferimentiTipoSpecifico("Cod_Vettore", piva, dataMovDal, dataMovAl, clausaInCentri, Elenco_LavCod_In, False, objParametriServer)
        PresentiRiferimentiCessionario1 = VerificaPresenzaRiferimentiTipoSpecifico("Cod_RisUm_Altro", piva, dataMovDal, dataMovAl, clausaInCentri, Elenco_LavCod_In, False, objParametriServer)
        PresentiRiferimentiCessionario2 = VerificaPresenzaRiferimentiTipoSpecifico("Extra_Int", piva, dataMovDal, dataMovAl, clausaInCentri, Elenco_LavCod_In, False, objParametriServer)
        PresentiRiferimentiAgente = VerificaPresenzaRiferimentiTipoSpecifico("Agente_Cod", piva, dataMovDal, dataMovAl, clausaInCentri, Elenco_LavCod_In, True, objParametriServer)
        PresentiRiferimentiCapoArea = VerificaPresenzaRiferimentiTipoSpecifico("CapoArea_Cod", piva, dataMovDal, dataMovAl, clausaInCentri, Elenco_LavCod_In, True, objParametriServer)

    End Sub

    Private Function VerificaPresenzaRiferimentiTipoSpecifico(ByVal nomeCampoReferenziante As String,
                                                              ByVal piva As String,
                                                              ByVal dataMovDal As String,
                                                              ByVal dataMovAl As String,
                                                              ByVal clausaInCentri As String,
                                                              ByVal Elenco_LavCod_In As String,
                                                              ByVal joinDettTecnico As Boolean,
                                                              ByRef objParametriServer As AgronicaCoreParametri) As Boolean
        Dim strsql As New StringBuilder

        strsql.AppendLine(" select top(1) " & nomeCampoReferenziante & " ")
        strsql.AppendLine(" FROM Agenda AS ag WITH (nolock) ")
        strsql.AppendLine(" INNER JOIN Movimenti AS movNrDDT WITH (nolock) ON ag.PIVA = movNrDDT.PIVA AND ag.Id_Agenda = movNrDDT.Id_Agenda AND movNrDDT.Cau_Mov = '4000' ")
        If joinDettTecnico Then
            strsql.AppendLine(" LEFT JOIN Mov_Dettaglio_Tecnico_Extra AS Movimento_Extra_0 WITH (nolock) ON Movimento_Extra_0.Piva = ag.PIVA AND Movimento_Extra_0.Id_Agenda = ag.Id_Agenda AND Movimento_Extra_0.Id_Mov_Det = 0 ")
        End If
        strsql.AppendLine(" LEFT JOIN( ")
        strsql.AppendLine("     SELECT md.piva, md.id_agenda, md.sa_cod, ")
        strsql.AppendLine("     ROW_NUMBER() OVER(PARTITION BY md.piva, md.id_agenda ORDER BY md.sa_cod desc) AS rn ")
        strsql.AppendLine("     FROM movimenti_dettagli md ")
        strsql.AppendLine("     INNER JOIN agenda ag ON md.piva = ag.piva ")
        strsql.AppendLine("     WHERE 1 = 1 ")
        If Not String.IsNullOrEmpty(piva) Then
            strsql.AppendLine("     AND ag.PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
        End If
        strsql.AppendLine("     AND ag.id_agenda = md.id_agenda ")
        strsql.AppendLine(" ) mov_det on mov_det.Id_Agenda = ag.Id_Agenda ")
        strsql.AppendLine("     And ag.PIVA = mov_det.piva ")
        strsql.AppendLine("     And mov_det.sa_cod In (" & Agro_SQL_Save_Clausola_IN(clausaInCentri) & ") ")
        strsql.AppendLine(" WHERE 1 = 1 ")
        If Not String.IsNullOrEmpty(piva) Then
            strsql.AppendLine(" And ag.PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
        End If
        strsql.AppendLine(" AND ag.split <> 1 ")
        strsql.AppendLine(" AND ag.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(Elenco_LavCod_In) & ") ")
        If Not String.IsNullOrEmpty(dataMovDal) Then
            strsql.AppendLine(" AND movNrDDT.Data_Movimento >= " & Agro_SQL_SaveDate(Convert.ToDateTime(dataMovDal)) & " ")
        End If
        If Not String.IsNullOrEmpty(dataMovAl) Then
            strsql.AppendLine(" AND movNrDDT.Data_Movimento <= " & Agro_SQL_SaveDate(Convert.ToDateTime(dataMovAl)) & " ")
        End If
        strsql.AppendLine(" AND mov_det.rn = 1 ")
        strsql.AppendLine(" AND " & nomeCampoReferenziante & " is not null ")
        strsql.AppendLine(" AND " & nomeCampoReferenziante & " != 0 ")

        Dim dt = EseguiQuery_Lettura(objParametriServer, strsql.ToString(), "VerificaPresenzaRiferimentiTipoSpecifico")

        Return dt.Rows.Count > 0
    End Function

    Private Function CreaTabellaTemp_Segni_ImpIva() As String
        Dim stb As New StringBuilder

        stb.AppendLine(" IF OBJECT_ID('tempdb.dbo.#Segni_ImpIva') IS NULL BEGIN ")
        stb.AppendLine("    CREATE TABLE #Segni_ImpIva ( ")
        stb.AppendLine("        Cau_Mov_S varchar(50) null,")
        stb.AppendLine("        Lav_Cod_S int null,")
        stb.AppendLine("        Segno_Imponibile int,")
        stb.AppendLine("        Segno_Iva int")
        stb.AppendLine("    )")
        stb.AppendLine()
        stb.AppendLine("    INSERT INTO #Segni_ImpIva(Cau_Mov_S, Lav_Cod_S, Segno_Imponibile, Segno_Iva) VALUES('7300', NULL, -1, 1) ")
        stb.AppendLine("    INSERT INTO #Segni_ImpIva(Cau_Mov_S, Lav_Cod_S, Segno_Imponibile, Segno_Iva) VALUES('4100', NULL, -1, 1) ")
        stb.AppendLine("    INSERT INTO #Segni_ImpIva(Cau_Mov_S, Lav_Cod_S, Segno_Imponibile, Segno_Iva) VALUES('7900', NULL, -1, 1) ")
        stb.AppendLine("    INSERT INTO #Segni_ImpIva(Cau_Mov_S, Lav_Cod_S, Segno_Imponibile, Segno_Iva) VALUES('4070', NULL, -1, 1) ")
        stb.AppendLine("    INSERT INTO #Segni_ImpIva(Cau_Mov_S, Lav_Cod_S, Segno_Imponibile, Segno_Iva) VALUES('4500', 1003, -1, 1) ")
        stb.AppendLine()
        stb.AppendLine("    INSERT INTO #Segni_ImpIva(Cau_Mov_S, Lav_Cod_S, Segno_Imponibile, Segno_Iva) VALUES('7350', NULL, 1, -1) ")
        stb.AppendLine("    INSERT INTO #Segni_ImpIva(Cau_Mov_S, Lav_Cod_S, Segno_Imponibile, Segno_Iva) VALUES('4200', NULL, 1, -1) ")
        stb.AppendLine("    INSERT INTO #Segni_ImpIva(Cau_Mov_S, Lav_Cod_S, Segno_Imponibile, Segno_Iva) VALUES('7380', NULL, 1, -1) ")
        stb.AppendLine("    INSERT INTO #Segni_ImpIva(Cau_Mov_S, Lav_Cod_S, Segno_Imponibile, Segno_Iva) VALUES('4500', 1002, 1, -1) ")
        stb.AppendLine()
        stb.AppendLine(" END ")

        Return stb.ToString()
    End Function

    Private Function EliminaTabellaTemp_Segni_ImpIva() As String

        Dim stb As New StringBuilder
        stb.AppendLine(" DROP TABLE #Segni_ImpIva ")
        Return stb.ToString()

    End Function

    Private Function EliminaTabellaTemp_Agenda_Documenti() As String

        Dim stb As New StringBuilder
        stb.AppendLine(" DROP TABLE #documenti_Cte ")
        Return stb.ToString()

    End Function

    Private Function Lista_LavCod_Ammessi(ByVal type As String,
                                          ByVal doc_type As String) As List(Of CausaleDocumento)

        Return causaliDocumenti.Where(Function(s) s.TYPE = type AndAlso s.DOC_TYPE = doc_type).ToList()

    End Function

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Crea query movimenti di vendita per report statistico
    ''' </summary>
    ''' -----------------------------------------------------------------------------
    Public Sub Filtra_Query_Report_Vendite(ByRef StrSQL As StringBuilder, ByVal piva As String,
                                            ByVal _docNumeroSin As String, ByVal _docNumero As Integer,
                                            ByVal _docNumeroDes As String, ByVal _nrRiga As String,
                                            ByVal _dataMovDal As String, ByVal _dataMovAl As String,
                                            ByVal _clienti As String, ByVal _agenti As String, ByVal _causali As String,
                                            ByVal _specie As String, ByVal _varieta As String,
                                            ByVal _prodotti As String, ByVal _categorie As String, ByVal _categcommerciali As String,
                                            ByVal _causali_trasp As String)


        If Not String.IsNullOrEmpty(_nrRiga) Then
            StrSQL.AppendLine(" AND (Ordine_Det = " & Agro_SQL_SaveNum(_nrRiga) & " ) ")
        End If

        ' Filtro Specie
        If Not String.IsNullOrEmpty(_specie) Then
            StrSQL.Append(" AND (Veg_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_specie, "|", ",")) & ") ) ")
        End If

        ' Filtro Varietà
        If Not String.IsNullOrEmpty(_varieta) Then
            StrSQL.Append(" AND (Cul_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_varieta, "|", ",")) & ") ) ")
        End If

        ' Filtro Categorie
        If Not String.IsNullOrEmpty(_categorie) Then
            StrSQL.Append(" AND (Elem_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_categorie, "|", ",")) & ") ) ")
        End If

        ' Filtro Categorie Commerciali  TODO
        If Not String.IsNullOrEmpty(_categcommerciali) Then
            StrSQL.Append(" AND (Cat_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_categcommerciali, "|", ",")) & ") ) ")
        End If

        ' Filtro Prodotti
        If Not String.IsNullOrEmpty(_prodotti) Then
            StrSQL.Append(" AND ( ")
            Dim elenco As New System.Text.StringBuilder("")
            Dim prodArray As String() = _prodotti.Split("|")
            For Each p In prodArray
                If Not elenco.ToString() = "" Then
                    elenco.Append(" OR ")
                End If
                elenco.Append(" ( ")
                Dim dueParti As String() = p.Split("_")
                elenco.Append(" Elem_Cod =  " & CInt(dueParti(0)) & " AND ")
                If CInt(dueParti(1)) > 0 Then
                    elenco.Append(" Pro_Cod =  " & CInt(dueParti(1)))
                Else
                    elenco.Append(" Mat_Cod =  " & CInt(dueParti(1)) * -1)
                End If
                elenco.Append(" ) ")
            Next
            StrSQL.Append(elenco.ToString())
            StrSQL.Append(" ) ")
        End If

    End Sub

    Private Sub Crea_Tabelle_Temp_Agende_Documenti(
            ByVal piva As String,
            ByVal _descrizione As String,
            ByVal _causali As String,
            ByVal type As String,
            ByVal doc_type As String,
            ByVal _docNumero As String,
            ByVal _docNumeroSin As String,
            ByVal _docNumeroDes As String,
            ByVal _dataMovDal As String,
            ByVal _dataMovAl As String,
            ByVal _causali_trasp As String,
            ByVal UtenteAbilitatoGestioneVisualizaAllegato As Boolean,
            ByRef objParametri As AgronicaCoreParametri
        )

        Dim strsql As New StringBuilder

        strsql.AppendLine(" IF OBJECT_ID('tempdb.dbo.#documenti_Cte') IS NULL BEGIN")
        strsql.AppendLine("     create table #documenti_Cte ")
        strsql.AppendLine("     (	id_agenda int not null,	piva nvarchar(25) not null,	conteggio int null )  ")
        strsql.AppendLine(" END ")
        EseguiQuery_Lettura(objParametri, strsql.ToString, "")

        strsql.Clear()
        strsql.AppendLine(" ;With agenda_CTE ")
        strsql.AppendLine(" ( ")
        strsql.AppendLine(" piva, id_agenda, lav_cod, sa_cod, ")
        strsql.AppendLine(" Des_Lib, Blocco_Flag, Modulo, Tipo_Accettazione ")
        strsql.AppendLine(" )")
        strsql.AppendLine(" as ")
        strsql.AppendLine(" (")
        strsql.AppendLine(" Select ")
        strsql.AppendLine(" ag.piva, ag.id_agenda, ag.Lav_Cod, ag.sa_cod, ")
        strsql.AppendLine(" ag.Des_Lib, ag.Blocco_Flag, ag.Modulo, ag.Tipo_Accettazione ")
        strsql.AppendLine(" From agenda As ag WITH (nolock)  ")
        strsql.AppendLine(" INNER Join Movimenti AS movNrDDT WITH (nolock) ")
        strsql.AppendLine(" On ag.PIVA = movNrDDT.PIVA And ag.Id_Agenda = movNrDDT.Id_Agenda And movNrDDT.Cau_Mov = '4000' ")
        strsql.AppendLine(" WHERE 1 = 1 ")
        If Not String.IsNullOrEmpty(piva) Then
            strsql.AppendLine(" And (ag.PIVA = '" & Agro_SQL_SaveText(piva) & "') ")
        End If
        If Not String.IsNullOrEmpty(_descrizione) Then
            strsql.AppendLine(" AND (ag.des_lib LIKE '%" & Agro_SQL_SaveText(_descrizione) & "%') ")
        End If
        If Not String.IsNullOrEmpty(_causali) Then
            strsql.AppendLine(" AND ( ag.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(_causali, "|", ",")) & ") ) ")
        Else
            Dim causaliAmmesse = String.Join("|", Lista_LavCod_Ammessi(type, doc_type).Select(Function(l) l.LAV_COD))
            strsql.AppendLine(" AND ( ag.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(Replace(causaliAmmesse, "|", ",")) & ") ) ")
        End If
        If _docNumero <> 0 Then
            strsql.AppendLine(" AND (Doc_Numero = " & Agro_SQL_SaveNum(_docNumero) & " ) ")
        End If
        If Not String.IsNullOrEmpty(_docNumeroSin) Then
            strsql.AppendLine(" AND (Doc_Numero_Sin = '" & Agro_SQL_SaveText(_docNumeroSin) & "' ) ")
        End If
        If Not String.IsNullOrEmpty(_docNumeroDes) Then
            strsql.AppendLine(" AND (Doc_Numero_Des = '" & Agro_SQL_SaveText(_docNumeroDes) & "' ) ")
        End If
        If Not String.IsNullOrEmpty(_dataMovDal) Then
            strsql.AppendLine(" AND ( movNrDDT.Data_Movimento >= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataMovDal)) & " ) ")
        End If
        If Not String.IsNullOrEmpty(_dataMovAl) Then
            strsql.AppendLine(" AND ( movNrDDT.Data_Movimento <= " & Agro_SQL_SaveDate(Convert.ToDateTime(_dataMovAl)) & " ) ")
        End If
        If Not String.IsNullOrEmpty(_causali_trasp) Then
            Dim listaCausali = "0, " & Replace(_causali_trasp, "|", ",")
            strsql.Append(" AND (movNrDDT.Causale_Trasporto_Cod IN (" & Agro_SQL_Save_Clausola_IN(listaCausali, False) & ")) ")
        End If
        strsql.AppendLine(" ) ")
        strsql.AppendLine(", ")
        strsql.AppendLine(" documenti_Cte as ( ")

        If UtenteAbilitatoGestioneVisualizaAllegato = True Then
            strsql.AppendLine("SELECT  id_agenda, piva, ISNULL(conteggio,0) AS conteggio FROM ( ")
            strsql.AppendLine("     SELECT ag.id_agenda, ag.piva, COUNT(ae.id_agenda) AS conteggio FROM  agenda_CTE ag ")
            strsql.AppendLine("     LEFT JOIN Alert_Entita ae ON ag.piva = ae.Piva AND ag.id_agenda = ae.ID_Agenda ")
            strsql.AppendLine("     INNER JOIN Alert_Elenco aele ON ae.ID_Alert_Entita = aele.ID_Alert_Entita ")
            strsql.AppendLine("     WHERE ae.allegati_documenti_cod <> 0")
            strsql.AppendLine("GROUP BY ag.ID_Agenda, ag.Piva ) qp  ")
        Else
            strsql.AppendLine(" select top 1 id_agenda, piva, 0 as conteggio from agenda_CTE ")
        End If
        strsql.AppendLine("  )  ")
        strsql.AppendLine("  insert into #documenti_Cte select * from documenti_Cte ")

        EseguiQuery_Lettura(objParametri, strsql.ToString, "")

    End Sub

End Class
