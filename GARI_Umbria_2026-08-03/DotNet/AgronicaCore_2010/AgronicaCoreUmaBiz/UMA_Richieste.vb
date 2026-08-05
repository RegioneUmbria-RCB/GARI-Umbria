Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework_POCO
Imports System.Transactions
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreEntityFramework
Imports System.Data.Entity
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUmaDal
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModelsSTD.exceptions

Public Class UMA_Richieste

    Const DescrCaricoTrasferimento = "Carico da trasferimento"
    Const DescrScaricoTrasferimento = "Scarico da trasferimento"
    Const DescrScaricoRestituzione = "Scarico da restituzione"
    Const DescrScaricoRecAccise = "Scarico da recupero accise"

    Const OrigineMovTrasferimento = 1
    Const OrigineMovRestituzione = 2
    Const OrigineMovRecAccise = 3

    Const StatoInCompilazione = "In compilazione"

    'Inserire una nuova testata nella relativa tabella
    Public Function Nuova_Richiesta(ByVal piva As String,
                                    ByVal pratica_cod As Integer,
                                    ByVal richiesta_cod As Integer,
                                    ByVal isTerzista As Boolean,
                                    ByVal Avanzamento_Richiesta As Integer,
                                    ByVal Anno As Integer,
                                    ByVal Rimanenza_Gasolio As Double,
                                    ByVal Rimanenza_Benzina As Double,
                                    ByVal Rimanenza_Gasolio_Serra As Double,
                                    ByVal Integrativa As Integer,
                                    ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByRef objParametri_user As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                    ByVal anticipo As Boolean,
                                    Optional ByRef Carburante_Richiesto As Double = 0,
                                    Optional ByRef Carburante_Richiesto_Benzina As Double? = 0,
                                    Optional ByRef Carburante_Richiesto_Gasolio As Double? = 0,
                                    Optional ByRef Carburante_Richiesto_Gasolio_Serra As Double? = 0,
                                    Optional ByRef Percentuale_Anticipo_Carb As Double? = 0,
                                    Optional Tipo_Azienda As Integer = enum_TipoAzienda_UMA.Azienda_Agricola_Privata,
                                    Optional permesso_Acqua As Integer = 0,
                                    Optional note_permesso_acqua As String = "",
                                    Optional cessata_Attivita As Boolean = False) As Integer

        Dim res As Integer
        Dim testata As New AgronicaCoreUmaDal.UMA_Richieste_Testata_W
        Dim letturaTestata As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.Nuova_Richiesta()"



        Dim Data_Inizio = AGRODATAINIZIO
        Dim Data_Fine = AGRODATAFINE
        If Anno <> 0 And Anno > 1900 And Anno < 2100 Then
            Data_Inizio = New Date(Anno, 1, 1)
            Data_Fine = New Date(Anno, 12, 31)
        End If
        Dim AnnoPrecedemte = Anno - 1


        Dim carburante As New UMACarburanteAnno
        If anticipo Then
            Dim conto = 0
            If isTerzista Then
                conto = -1
            End If
            Dim dtRichieste = letturaTestata.Leggi_Elenco_Con_Indirizzi(piva, Anno, "", "", objParametri_server, objParametri_user, False, "", False, 0, "", "-1", conto, False, False, False, 0, True)
            If dtRichieste.Rows.Count > 0 Then
                Throw New GiasException("Esiste già una richiesta per l'anno corrente")
            End If
            'qui devo far passare il nuovo parametro user
            carburante = CaricaCarburantiScorsoAnno(piva, conto, objParametri_server, objParametri_user, Percentuale_Anticipo_Carb, AnnoPrecedemte)

            If Carburante_Richiesto = 0 AndAlso Carburante_Richiesto_Benzina = 0 AndAlso Carburante_Richiesto_Gasolio = 0 AndAlso Carburante_Richiesto_Gasolio_Serra = 0 Then
                Carburante_Richiesto = carburante.CarburanteRichiesto
                Carburante_Richiesto_Benzina = carburante.CarburantePerTipo(enum_TipoCarburante_UMA.Benzina)
                Carburante_Richiesto_Gasolio = carburante.CarburantePerTipo(enum_TipoCarburante_UMA.Gasolio)
                Carburante_Richiesto_Gasolio_Serra = carburante.CarburantePerTipo(enum_TipoCarburante_UMA.Gasolio_Serra)
            Else
                carburante.CarburanteRichiesto = Carburante_Richiesto
                carburante.CarburantePerTipo(enum_TipoCarburante_UMA.Benzina) = Carburante_Richiesto_Benzina
                carburante.CarburantePerTipo(enum_TipoCarburante_UMA.Gasolio) = Carburante_Richiesto_Gasolio
                carburante.CarburantePerTipo(enum_TipoCarburante_UMA.Gasolio_Serra) = Carburante_Richiesto_Gasolio_Serra
            End If

            If Carburante_Richiesto = 0 AndAlso Carburante_Richiesto_Benzina = 0 AndAlso Carburante_Richiesto_Gasolio = 0 AndAlso Carburante_Richiesto_Gasolio_Serra = 0 Then
                Throw New GiasException("L'azienda non ha acquistato carburante l'anno precedente o non è stato impostato alcun valore nella richiesta di anticipo ")
            End If

            Avanzamento_Richiesta = -1
        End If

        Dim rinunciaAnnoSuccessivo As Integer = IIf(cessata_Attivita, 1, 0)

        'creazione record
        Dim richiesta As New AgronicaCoreEntityFramework_POCO.UMA_Richieste_Testata With {
            .Piva_SuperUser = objParametri_server.PivaSuperUser,
            .Piva = piva,
            .Richiesta_Cod = richiesta_cod,
            .Pratica_Cod = pratica_cod,
            .Username_Creazione = objParametri_server.UtenteUsername,
            .Username_Modifica = objParametri_server.UtenteUsername,
            .Macchine_Impiegate = "",
            .Carburante_Approvato = 0,
            .Carburante_Richiesto = Carburante_Richiesto,
            .Carburante_Calcolato = 0,
            .Carburante_Richiesto_Benzina = Carburante_Richiesto_Benzina,
            .Carburante_Richiesto_Gasolio = Carburante_Richiesto_Gasolio,
            .Carburante_Richiesto_Gasolio_Serra = Carburante_Richiesto_Gasolio_Serra,
            .Validita_Inizio = Data_Inizio,
            .Validita_Fine = Data_Fine,
            .Inviato = 0,
            .Data_Creazione = DateTime.Now,
            .Data_Modifica = DateTime.Now,
            .Tipo_Richiesta = isTerzista,
            .Avanzamento_Richiesta = Avanzamento_Richiesta,
            .Rimanenza_Gasolio = Rimanenza_Gasolio,
            .Rimanenza_Benzina = Rimanenza_Benzina,
            .Rimanenza_Gasolio_Serra = Rimanenza_Gasolio_Serra,
            .Richiesta_Integrativa = Integrativa,
            .Tipo_Azienda = Tipo_Azienda,
            .Rinuncia_Nuova_Richiesta_Anno_Successivo = rinunciaAnnoSuccessivo,
            .Percentuale_Anticipo_Richiesta_da_Azienda = Percentuale_Anticipo_Carb,
            .Permesso_acqua = permesso_Acqua,
            .Note_permesso_acqua = note_permesso_acqua
        }

        Try

            Dim transactionOptions = New TransactionOptions With {
                .IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                res = testata.Nuova_Richiesta(richiesta, objParametri_server)

                If (String.IsNullOrEmpty(MessaggioErrore) And res = 0) Then

                    If anticipo Then
                        AggingiRigheFittizie(piva, richiesta_cod, carburante, objParametri_server, res, MessaggioErrore, scope)
                    End If
                    ' COMMIT Effettivo
                    scope.Complete()
                Else
                    ' Rollback
                    scope.Dispose()
                End If

            End Using

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)


        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        res = richiesta_cod
        Return res

    End Function
    'aggiungere ciclo for in base al tipo carburante
    Private Sub AggingiRigheFittizie(piva As String,
                                     richiesta_cod As Integer,
                                     carburanteRichiesto As UMACarburanteAnno,
                                     ByRef objParametri_server As AgronicaCoreParametri,
                                     ByRef res As Integer,
                                     MessaggioErrore As String,
                                     scope As TransactionScope)

        Dim NuovaRichiestaFittizia = CreaRichiestaFittizia(objParametri_server.PivaSuperUser, piva, richiesta_cod, objParametri_server.UtenteUsername, carburanteRichiesto.CarburanteRichiesto)
        Dim richiestaRiga As New AgronicaCoreUmaDal.UMA_Richieste_W
        res = richiestaRiga.Nuova_Richiesta(NuovaRichiestaFittizia, objParametri_server)

        If Not String.IsNullOrEmpty(MessaggioErrore) Or res <> 0 Then
            ' Rollback
            scope.Dispose()
        End If

        Dim TopCode As Integer
        Dim BaseCode As Integer
        AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, 1)

        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

        For Each item As KeyValuePair(Of Integer, Double) In carburanteRichiesto.CarburantePerTipo
            If item.Value > 0 Then
                Dim richiesta_dettaglio_cod = objSequenze.NuovoId_Tabella("UMA_Richieste_Lavorazioni", BaseCode, TopCode, objParametri_server)
                Dim NuovaRichiestaLavorazioneFittizia = CreaRichiestaLavorazioneFittizia(objParametri_server.PivaSuperUser,
                                                                                         piva,
                                                                                         richiesta_cod,
                                                                                         richiesta_dettaglio_cod,
                                                                                         objParametri_server.UtenteUsername,
                                                                                         item.Value,
                                                                                         item.Key)
                Dim richiestaLAvorazioneRiga As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_W
                res = richiestaLAvorazioneRiga.Nuova_Richiesta(NuovaRichiestaLavorazioneFittizia, objParametri_server)

                If Not String.IsNullOrEmpty(MessaggioErrore) Or res <> 0 Then
                    ' Rollback
                    scope.Dispose()
                End If
            End If
        Next

    End Sub

    Private Function CreaRichiestaFittizia(ByVal piva_SuperUser As String, ByVal piva As String,
                                              ByVal richiestaCod As Integer, ByVal UtenteUsername As String,
                                              ByVal carburanteRichiesto As Double) As AgronicaCoreEntityFramework_POCO.UMA_Richieste
        Return New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                .Piva_SuperUser = piva_SuperUser,
                .Piva = piva,
                .Gruppo_Colturale_UMA = "0000",
                .Richiesta_Cod = richiestaCod,
                .Totale_Superficie_UMA = 0,
                .Zona_Pendenza_A_UMA = 0,
                .Zona_Pendenza_B_UMA = 0,
                .Zona_Tessitura_Normale_UMA = 0,
                .Zona_Tessitura_Media_UMA = 0,
                .Zona_Tessitura_Tenace_UMA = 0,
                .Carburante_Calcolato = carburanteRichiesto,
                .Carburante_Richiesto = carburanteRichiesto,
                .Carburante_Approvato = carburanteRichiesto,
                .Validita_Inizio = "1900-01-01T00:00:00",
                .Validita_Fine = "2100-12-31T00:00:00",
                .Inviato = 0,
                .DataInvio = "2021-07-02T16:38:30.937",
                .Data_Creazione = DateTime.Now.ToLongDateString,
                .Data_Modifica = DateTime.Now.ToLongDateString,
                .Username_Creazione = UtenteUsername,
                .Username_Modifica = UtenteUsername,
                .Totale_Superficie_UMA_Edit = 0,
                .Zona_Pendenza_A_UMA_Edit = 0,
                .Zona_Pendenza_B_UMA_Edit = 0,
                .Zona_Tessitura_Normale_UMA_Edit = 0,
                .Zona_Tessitura_Media_UMA_Edit = 0,
                .Zona_Tessitura_Tenace_UMA_Edit = 0,
                .Programmazione_Cod = 0
            }
    End Function

    Private Function CreaRichiestaLavorazioneFittizia(ByVal piva_SuperUser As String, ByVal piva As String,
                                                      ByVal richiestaCod As Integer,
                                                      ByVal richiesta_dettaglio_cod As Integer,
                                                      ByVal UtenteUsername As String,
                                                      ByVal carburanteRichiesto As Double,
                                                      ByVal tipoCarburante As Integer) As AgronicaCoreEntityFramework_POCO.UMA_Richieste_Lavorazioni
        Return New AgronicaCoreEntityFramework_POCO.UMA_Richieste_Lavorazioni With {
                .Piva_SuperUser = piva_SuperUser,
                .Piva = piva,
                .Gruppo_Colturale_UMA = "0000",
                .Lavorazione_UMA = "00000",
                .Lavorazione_GIAS = "162",
                .Richiesta_Cod = richiestaCod,
                .Richiesta_Dettaglio_Cod = richiesta_dettaglio_cod,
                .Tipo_Carburante = tipoCarburante.ToString(),
                .Superficie_Maggiorazione_Trasferimenti = 0,
                .Nr_Lavorazioni_Previste = 1,
                .Nr_Lavorazioni_Richieste = 1,
                .Piu_Lavorazioni_Previste = 0,
                .Piu_Raccolti_Previsti = 0,
                .Fabbisogno_Calcolato = carburanteRichiesto,
                .Fabbisogno_Richiesto = carburanteRichiesto,
                .Fabbisogno_Assegnato = carburanteRichiesto,
                .Validita_Inizio = "1900-01-01T00:00:00",
                .Validita_Fine = "2100-12-31T00:00:00",
                .Inviato = 0,
                .DataInvio = "2021-07-02T16:38:30.953",
                .Data_Creazione = DateTime.Now.ToLongDateString,
                .Data_Modifica = DateTime.Now.ToLongDateString,
                .Username_Creazione = UtenteUsername,
                .Username_Modifica = UtenteUsername,
                .Totale_Superficie_UMA = 0,
                .Zona_Pendenza_A_UMA = 0,
                .Zona_Pendenza_B_UMA = 0,
                .Zona_Tessitura_Normale_UMA = 0,
                .Zona_Tessitura_Media_UMA = 0,
                .Zona_Tessitura_Tenace_UMA = 0,
                .Programmazione_Cod = 0,
                .Note_Compilatore = "",
                .Note_Approvatore = "",
                .Id_Attivita = 0,
                .Qta_Manuale = 0,
                .Mesi = 0
    }
    End Function

    Private _efContext As Gias_DeveloperServer_Entities
    Private _carburantiDAL As Vendita_Carburanti_DAL

    Private Function CaricaCarburantiScorsoAnno(ByVal piva As String,
                                                ByVal conto As Integer,
                                                ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef objParametri_user As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                ByRef Percentuale_Anticipo_Carb As Double,
                                                ByVal AnnoPrecedente As Integer) As UMACarburanteAnno
        Dim retObject As New UMACarburanteAnno
        Dim retObject2 As New UMACarburanteAnno

        Dim objVendite As New AgronicaCoreUmaDal.UMA_Vendite_R
        Dim dt As DataTable

        retObject.CarburantePerTipo.Add(enum_TipoCarburante_UMA.Gasolio, 0)
        retObject.CarburantePerTipo.Add(enum_TipoCarburante_UMA.Benzina, 0)
        retObject.CarburantePerTipo.Add(enum_TipoCarburante_UMA.Gasolio_Serra, 0)

        dt = objVendite.LeggiCarburanteVenduto(piva, AnnoPrecedente, conto, enum_TipoCarburante_UMA.Gasolio, "", objParametri_server)
        If dt.Rows.Count > 0 Then
            retObject.CarburantePerTipo(enum_TipoCarburante_UMA.Gasolio) = dt(0).Item("Totale_Carb") * Percentuale_Anticipo_Carb
        End If

        dt = objVendite.LeggiCarburanteVenduto(piva, AnnoPrecedente, conto, enum_TipoCarburante_UMA.Benzina, "", objParametri_server)
        If dt.Rows.Count > 0 Then
            retObject.CarburantePerTipo(enum_TipoCarburante_UMA.Benzina) = dt(0).Item("Totale_Carb") * Percentuale_Anticipo_Carb
        End If

        dt = objVendite.LeggiCarburanteVenduto(piva, AnnoPrecedente, conto, enum_TipoCarburante_UMA.Gasolio_Serra, "", objParametri_server)
        If dt.Rows.Count > 0 Then
            retObject.CarburantePerTipo(enum_TipoCarburante_UMA.Gasolio_Serra) = dt(0).Item("Totale_Carb") * Percentuale_Anticipo_Carb
        End If

        Return retObject

    End Function

    'Inserimento/modifica delle richieste in UMA_Richieste
    Public Function Aggiorna_Richieste(ByVal dtInsert As DataTable,
                                      ByVal dtUpdate As DataTable,
                                      ByVal piva As String,
                                      ByVal richiesta_cod As Integer,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim res As Integer
        Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_W
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim MessaggioErrore As String = String.Empty
        Dim richiesteInsert As New ArrayList
        Dim richiesteUpdate As New ArrayList
        Dim r As AgronicaCoreEntityFramework_POCO.UMA_Richieste

        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.Aggiorna_Richieste()"

        'Compongo record per inserimenti e modifiche
        For Each row As DataRow In dtInsert.Rows

            If (row.Item("Sup_B") = 0 AndAlso row.Item("Sup_A") = 0) Then
                row.Item("Sup_A") = row.Item("Sup_UMA")
                row.Item("sup_UMA_A_Edit") = row.Item("Sup_UMA")
            End If

            If (If(dtInsert.Columns.Contains("sup_media"), CDbl(row.Item("sup_media")), CDbl(row.Item("TerrenoMedio"))) = 0 AndAlso
                If(dtInsert.Columns.Contains("sup_normale"), CDbl(row.Item("sup_normale")), CDbl(row.Item("TerrenoNormale"))) = 0 AndAlso
                If(dtInsert.Columns.Contains("sup_tenace"), CDbl(row.Item("sup_tenace")), CDbl(row.Item("TerrenoTenace"))) = 0) Then
                If (dtInsert.Columns.Contains("sup_media")) Then
                    row.Item("sup_media") = row.Item("sup_UMA")
                Else
                    row.Item("TerrenoMedio") = row.Item("sup_UMA")
                End If
                row.Item("TerrenoNormale_Edit") = row.Item("sup_UMA")
            End If

            r = New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                    .Piva_SuperUser = objParametri.PivaSuperUser,
                    .Piva = piva,
                    .Gruppo_Colturale_UMA = row.Item("Macrouso_UMA_Cod"),
                    .Richiesta_Cod = richiesta_cod,
                    .Programmazione_Cod = row.Item("Programmazione_Cod"),
                    .Totale_Superficie_UMA = row.Item("Sup_UMA"),
                    .Zona_Pendenza_A_UMA = row.Item("Sup_A"),
                    .Zona_Pendenza_B_UMA = row.Item("Sup_B"),
                    .Carburante_Approvato = row.Item("assegnato"),
                    .Carburante_Calcolato = row.Item("calcolato"),
                    .Carburante_Richiesto = row.Item("richiesto"),
                    .Zona_Tessitura_Media_UMA = If(dtInsert.Columns.Contains("sup_media"), CDbl(row.Item("sup_media")), CDbl(row.Item("TerrenoMedio"))),
                    .Zona_Tessitura_Normale_UMA = If(dtInsert.Columns.Contains("sup_normale"), CDbl(row.Item("sup_normale")), CDbl(row.Item("TerrenoNormale"))),
                    .Zona_Tessitura_Tenace_UMA = If(dtInsert.Columns.Contains("sup_tenace"), CDbl(row.Item("sup_tenace")), CDbl(row.Item("TerrenoTenace"))),
                    .Username_Creazione = objParametri.UtenteUsername,
                    .Username_Modifica = objParametri.UtenteUsername,
                    .Validita_Inizio = AGRODATAINIZIO,
                    .Validita_Fine = AGRODATAFINE,
                    .Inviato = 0,
                    .Data_Creazione = DateTime.Now,
                    .Data_Modifica = DateTime.Now,
                    .Totale_Superficie_UMA_Edit = CDbl(row.Item("sup_UMA_Edit")),
                    .Zona_Pendenza_A_UMA_Edit = CDbl(row.Item("sup_UMA_A_Edit")),
                    .Zona_Pendenza_B_UMA_Edit = CDbl(row.Item("sup_UMA_B_Edit")),
                    .Zona_Tessitura_Normale_UMA_Edit = CDbl(row.Item("TerrenoNormale_Edit")),
                    .Zona_Tessitura_Media_UMA_Edit = CDbl(row.Item("TerrenoMedio_Edit")),
                    .Zona_Tessitura_Tenace_UMA_Edit = CDbl(row.Item("TerrenoTenace_Edit")),
                    .Regolamento_Cod = CInt(row.Item("Regolamento_Cod"))
            }

            richiesteInsert.Add(r)

        Next

        For Each row As DataRow In dtUpdate.Rows

            r = New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                    .Piva_SuperUser = objParametri.PivaSuperUser,
                    .Piva = piva,
                    .Gruppo_Colturale_UMA = row.Item("Macrouso_UMA_Cod"),
                    .Richiesta_Cod = richiesta_cod,
                    .Programmazione_Cod = row.Item("Programmazione_Cod"),
                    .Totale_Superficie_UMA = row.Item("Sup_UMA"),
                    .Zona_Pendenza_A_UMA = row.Item("Sup_A"),
                    .Zona_Pendenza_B_UMA = row.Item("Sup_B"),
                    .Carburante_Approvato = row.Item("assegnato"),
                    .Carburante_Calcolato = row.Item("calcolato"),
                    .Carburante_Richiesto = row.Item("richiesto"),
                    .Zona_Tessitura_Media_UMA = If(dtUpdate.Columns.Contains("sup_media"), CDbl(row.Item("sup_media")), CDbl(row.Item("TerrenoMedio"))),
                    .Zona_Tessitura_Normale_UMA = If(dtUpdate.Columns.Contains("sup_normale"), CDbl(row.Item("sup_normale")), CDbl(row.Item("TerrenoNormale"))),
                    .Zona_Tessitura_Tenace_UMA = If(dtUpdate.Columns.Contains("sup_tenace"), CDbl(row.Item("sup_tenace")), CDbl(row.Item("TerrenoTenace"))),
                    .Username_Creazione = objParametri.UtenteUsername,
                    .Username_Modifica = objParametri.UtenteUsername,
                    .Validita_Inizio = AGRODATAINIZIO,
                    .Validita_Fine = AGRODATAFINE,
                    .Inviato = 0,
                    .Data_Creazione = DateTime.Now,
                    .Data_Modifica = DateTime.Now,
                    .Totale_Superficie_UMA_Edit = CDbl(row.Item("sup_UMA_Edit")),
                    .Zona_Pendenza_A_UMA_Edit = CDbl(row.Item("sup_UMA_A_Edit")),
                    .Zona_Pendenza_B_UMA_Edit = CDbl(row.Item("sup_UMA_B_Edit")),
                    .Zona_Tessitura_Normale_UMA_Edit = CDbl(row.Item("TerrenoNormale_Edit")),
                    .Zona_Tessitura_Media_UMA_Edit = CDbl(row.Item("TerrenoMedio_Edit")),
                    .Zona_Tessitura_Tenace_UMA_Edit = CDbl(row.Item("TerrenoTenace_Edit"))
            }

            richiesteUpdate.Add(r)

        Next

        Try

            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                res = richieste.Aggiorna_Richieste(richiesteInsert, richiesteUpdate, New ArrayList, objParametri)

                If (String.IsNullOrEmpty(MessaggioErrore) And res = 0) Then

                    ' COMMIT Effettivo
                    scope.Complete()
                Else
                    ' Rollback
                    scope.Dispose()
                End If

            End Using

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)


        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return res

    End Function

    Public Function Check_Biologico_Integrative(ByVal richiesta_cod As Integer,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Dictionary(Of Integer, Integer)

        Dim res As Integer
        Dim resDict As New Dictionary(Of Integer, Integer)
        Dim listoIn As New ArrayList
        Dim listoOut As New ArrayList
        Dim richiesteTestataR As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim praticaR As New AgronicaCoreProfilazioneDAL.Pratiche_R
        Dim richiesteR As New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim richiesteW As New AgronicaCoreUmaDal.UMA_Richieste_W
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim MessaggioErrore As String = String.Empty
        Dim richiesteInsert As New ArrayList
        Dim richiesteUpdate As New ArrayList

        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.Check_Biologico_Integrative()"
        Dim richiestaCodOrig As New Integer

        Try

            Dim dtTesta = richiesteTestataR.Leggi("", richiesta_cod, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri)

            If dtTesta.Rows.Count > 0 Then
                Dim dtTestaRow = dtTesta.Rows.Item(0)
                Dim anno = praticaR.Leggi(dtTestaRow.Item("Pratica_Cod"), "", dtTestaRow.Item("Piva"), "", 0, 0, 0, 0, AGRODATAINIZIO,
                                          AGRODATAFINE, "", "", objParametri, 0, 0)
                Dim dtTesta2 = richiesteTestataR.Leggi(dtTestaRow.Item("Piva"), 0, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri,
                                                      dtTestaRow.Item("tipo_Richiesta"), anno.Rows.Item(0).Item("Anno"), dtTestaRow.Item("Avanzamento_Richiesta"),
                                                   " richiesta_cod ASC ")

                If dtTesta2.Rows.Count > 1 Then
                    richiestaCodOrig = dtTesta2.Rows.Item(0).Item("Richiesta_Cod")
                End If

            End If

            Dim dt = richiesteR.Leggi(dtTesta.Rows.Item(0).Item("Piva"), richiesta_cod, "", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri)
            If dt.Rows.Count > 0 Then
                Dim row = dt.Rows.Item(0)
                Dim dt2 = richiesteR.Leggi(dtTesta.Rows.Item(0).Item("Piva"), richiestaCodOrig, "", 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri)

                For Each dtRow In dt.Rows

                    Dim found = dt2.Select("Gruppo_Colturale_UMA = " + dtRow.Item("Gruppo_Colturale_UMA").ToString + " AND Programmazione_Cod = " + dtRow.Item("Programmazione_Cod").ToString + " ")

                    If found.Count > 0 Then

                        If CInt(found.First.Item("Regolamento_Cod").ToString) <> CInt(dtRow.Item("Regolamento_Cod").ToString) Then

                            resDict.Add(CInt(dtRow.Item("Gruppo_Colturale_UMA").ToString), CInt(found.First.Item("Regolamento_Cod").ToString))

                            Dim obj As New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                                .Piva_SuperUser = CStr(dtRow.Item("Piva_Superuser").ToString),
                                .Piva = CStr(dtRow.Item("Piva").ToString),
                                .Gruppo_Colturale_UMA = CInt(dtRow.Item("Gruppo_Colturale_UMA").ToString),
                                .Richiesta_Cod = CInt(dtRow.Item("Richiesta_Cod").ToString),
                                .Programmazione_Cod = CInt(dtRow.Item("Programmazione_Cod").ToString),
                                .Totale_Superficie_UMA = CDbl(dtRow.Item("Totale_Superficie_UMA").ToString),
                                .Zona_Pendenza_A_UMA = CDbl(dtRow.Item("Zona_Pendenza_A_UMA").ToString),
                                .Zona_Pendenza_B_UMA = CDbl(dtRow.Item("Zona_Pendenza_B_UMA").ToString),
                                .Zona_Tessitura_Normale_UMA = CDbl(dtRow.Item("Zona_Tessitura_Normale_UMA").ToString),
                                .Zona_Tessitura_Media_UMA = CDbl(dtRow.Item("Zona_Tessitura_Media_UMA").ToString),
                                .Zona_Tessitura_Tenace_UMA = CDbl(dtRow.Item("Zona_Tessitura_Tenace_UMA").ToString),
                                .Carburante_Calcolato = CDbl(dtRow.Item("Carburante_Calcolato").ToString),
                                .Carburante_Richiesto = CDbl(dtRow.Item("Carburante_Richiesto").ToString),
                                .Carburante_Approvato = CDbl(dtRow.Item("Carburante_Approvato").ToString),
                                .Validita_Inizio = dtRow.Item("Validita_Inizio"),
                                .Validita_Fine = dtRow.Item("Validita_Fine"),
                                .Inviato = CInt(dtRow.Item("Inviato").ToString),
                                .DataInvio = If(IsDBNull(dtRow.Item("DataInvio")), Nothing, dtRow.Item("DataInvio")),
                                .Data_Creazione = dtRow.Item("Data_Creazione"),
                                .Data_Modifica = dtRow.Item("Data_Modifica"),
                                .Username_Creazione = CStr(dtRow.Item("Username_Creazione").ToString),
                                .Username_Modifica = CStr(dtRow.Item("Username_Modifica").ToString),
                                .Totale_Superficie_UMA_Edit = CDbl(dtRow.Item("Totale_Superficie_UMA_Edit").ToString),
                                .Zona_Pendenza_A_UMA_Edit = CDbl(dtRow.Item("Zona_Pendenza_A_UMA_Edit").ToString),
                                .Zona_Pendenza_B_UMA_Edit = CDbl(dtRow.Item("Zona_Pendenza_B_UMA_Edit").ToString),
                                .Zona_Tessitura_Normale_UMA_Edit = CDbl(dtRow.Item("Zona_Tessitura_Normale_UMA_Edit").ToString),
                                .Zona_Tessitura_Media_UMA_Edit = CDbl(dtRow.Item("Zona_Tessitura_Media_UMA_Edit").ToString),
                                .Zona_Tessitura_Tenace_UMA_Edit = CDbl(dtRow.Item("Zona_Tessitura_Tenace_UMA_Edit").ToString),
                                .Regolamento_Cod = CInt(dtRow.Item("Regolamento_Cod").ToString)
                            }

                            listoOut.Add(obj)
                            Dim obj2 = obj.DeepCloneObject()
                            obj2.Regolamento_Cod = CInt(found.First.Item("Regolamento_Cod").ToString)
                            listoIn.Add(obj2)

                        End If
                    End If
                Next

                Dim transactionOptions = New TransactionOptions()
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                    res = richiesteW.Aggiorna_Richieste(New ArrayList, New ArrayList, listoOut, objParametri)

                    If (String.IsNullOrEmpty(MessaggioErrore) And res = 0) Then

                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If

                End Using

                Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                    res = richiesteW.Aggiorna_Richieste(listoIn, New ArrayList, New ArrayList, objParametri)

                    If (String.IsNullOrEmpty(MessaggioErrore) And res = 0) Then

                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If

                End Using

            End If


        Catch ex As Exception

        End Try

        Return resDict

    End Function

    Public Function Aggiorna_Richiesta_Carburanti(ByVal piva As String,
                                                  ByVal r_cod As Integer,
                                                  ByVal benzina As Integer,
                                                  ByVal gasolio As Integer,
                                                  ByVal gasolioSerra As Integer,
                                                  ByVal benzinaAppro As Integer,
                                                  ByVal gasolioAppro As Integer,
                                                  ByVal gasolioSerraAppro As Integer,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.Aggiorna_Richiesta_Carburanti()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim richiesteTestate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_W
        Dim risultato As Integer
        Dim totaleCarb As Integer

        Try
            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)


                    Dim richiestaTesta = (From testaUMA In
                                         GiasContext.UMA_Richieste_Testata Where
                                                                       testaUMA.Piva = piva And
                                                                       testaUMA.Richiesta_Cod = r_cod).FirstOrDefault

                    richiestaTesta.Richiesta_Iniziale_Benzina = benzina
                    richiestaTesta.Richiesta_Iniziale_Gasolio = gasolio
                    richiestaTesta.Richiesta_Iniziale_Gasolio_Serra = gasolioSerra
                    totaleCarb = benzina + gasolio + gasolioSerra
                    richiestaTesta.Approvazione_Iniziale_Benzina = benzinaAppro
                    richiestaTesta.Approvazione_Iniziale_Gasolio = gasolioAppro
                    richiestaTesta.Approvazione_Iniziale_Gasolio_Serra = gasolioSerraAppro
                    richiestaTesta.Carburante_Richiesto = IIf(richiestaTesta.Carburante_Richiesto = 0, totaleCarb, richiestaTesta.Carburante_Richiesto)

                    risultato = richiesteTestate.Aggiorna_Richiesta_Carburanti(richiestaTesta, objParametri)

                    GiasContext.SaveChanges()

                    If (String.IsNullOrEmpty(MessaggioErrore)) Then
                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If


                End Using
            End Using

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)


        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return risultato


    End Function

    Public Function Aggiorna_Richieste_Terzisti(Richiesta_Cod As Integer,
                                                ByVal righeInserite As String,
                                                ByVal righeModificate As String,
                                                ByVal righeCancellate As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                Optional ByVal pivaRichiesta As String = "",
                                                Optional ByVal anno As Integer = 0) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.Aggiorna_Richieste_Terzisti()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim Jinsert As New JArray
        Dim Jupdate As JArray
        Dim Jdelete As JArray
        Dim richiesteR = New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim richiesteW = New AgronicaCoreUmaDal.UMA_Richieste_W
        Dim lavorazioniW = New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_W
        Dim testataR = New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        'Dim testata As DataRow = testataR.LeggiDaRichiestaCod(Richiesta_Cod, objParametri).Rows.Item(0)
        Dim r As AgronicaCoreEntityFramework_POCO.UMA_Richieste
        Dim EFArrayToInsert As New ArrayList
        Dim EFArrayToUpdate As New ArrayList
        Dim EFArrayToDelete As New ArrayList
        Dim richiestexRegImpianti As New AgronicaCoreUmaDal.UMA_RichiesteXReg_Impianti_W
        Dim Risultato As Integer = 0
        Dim inOK As Boolean = False 'per controllare se ci sono stati dei nuovi record inseriti

        Dim legamiRApp As New DataTable ' per contenere i nuovi record di UMARichiestexregImpianti
        Dim legamiDaRecidere As List(Of Tuple(Of String, String)) = New List(Of Tuple(Of String, String))

        If righeInserite <> "" And righeInserite <> "[]" Then

            Jinsert = JArray.Parse(righeInserite)
            inOK = True

        End If

        Try
            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    '###########################################################################################################################################
                    '################################### NUOVI INSERIMENTI/MODIFICHE ###########################################################################
                    '###########################################################################################################################################

                    If inOK Then

                        'creo i record da inserire
                        For Each objRiga As JObject In Jinsert

                            If (Not IsNothing(objRiga.Item("supB_Edit")) And Not IsNothing(objRiga.Item("supA_Edit"))) Then 'aggiunto gloria per permettere salvataggio
                                If (CDbl(objRiga.Item("supB_Edit")) = 0 AndAlso CDbl(objRiga.Item("supA_Edit")) = 0) Then
                                    objRiga.Item("supA_Edit") = objRiga.Item("sup_tot")
                                    objRiga.Item("sup_UMA_A_Edit") = objRiga.Item("sup_tot")
                                End If
                            End If

                            If (Not IsNothing(objRiga.Item("tessitura_Norm_Edit")) And Not IsNothing(objRiga.Item("tessitura_Media_Edit")) And Not IsNothing(objRiga.Item("tessitura_Media_Edit"))) Then   'aggiunto gloria per permettere salvataggio
                                If (CDbl(objRiga.Item("tessitura_Norm_Edit")) = 0 AndAlso CDbl(objRiga.Item("tessitura_Media_Edit")) = 0 AndAlso
                                CDbl(objRiga.Item("tessitura_Tenace_Edit")) = 0) Then
                                    objRiga.Item("tessitura_Norm_Edit") = objRiga.Item("sup_tot")
                                End If
                            End If

                            r = New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                            .Piva = CStr(objRiga.Item("piva")),
                            .Piva_SuperUser = objParametri.PivaSuperUser,
                            .Richiesta_Cod = Richiesta_Cod,
                            .Programmazione_Cod = CStr(IIf(IsNothing(objRiga.Item("Programmazione_Cod")), -1, objRiga.Item("Programmazione_Cod"))),
                            .Gruppo_Colturale_UMA = CStr(IIf(IsNothing(objRiga.Item("Macrouso_UMA_Cod")), "0000", objRiga.Item("Macrouso_UMA_Cod"))),
                            .Totale_Superficie_UMA = CDbl(IIf(IsNothing(objRiga.Item("supA")), 0, objRiga.Item("supA"))) + CDbl(IIf(IsNothing(objRiga.Item("supB")), 0, objRiga.Item("supB"))),
                            .Zona_Pendenza_A_UMA = CDbl(IIf(IsNothing(objRiga.Item("supA")), 0, objRiga.Item("supA"))),
                            .Zona_Pendenza_B_UMA = CDbl(IIf(IsNothing(objRiga.Item("supB")), 0, objRiga.Item("supB"))),
                            .Zona_Tessitura_Normale_UMA = CDbl(IIf(IsNothing(objRiga.Item("tessitura_Norm")), 0, objRiga.Item("tessitura_Norm"))), 'CDbl(objRiga.Item("tessitura_Norm")),
                            .Zona_Tessitura_Media_UMA = CDbl(IIf(IsNothing(objRiga.Item("tessitura_Media")), 0, objRiga.Item("tessitura_Media"))), 'CDbl(objRiga.Item("tessitura_Media")),
                            .Zona_Tessitura_Tenace_UMA = CDbl(IIf(IsNothing(objRiga.Item("tessitura_Tenace")), 0, objRiga.Item("tessitura_Tenace"))),
                            .Carburante_Richiesto = CInt(IIf(IsNothing(objRiga.Item("ltrichiesto")), 0, objRiga.Item("ltrichiesto"))),
                            .Carburante_Calcolato = CInt(IIf(IsNothing(objRiga.Item("fabbisognoCalc")), 0, objRiga.Item("fabbisognoCalc"))),
                            .Carburante_Approvato = CInt(IIf(IsNothing(objRiga.Item("ltAssegnato")), 0, objRiga.Item("ltAssegnato"))),
                            .Validita_Inizio = AGRODATAINIZIO,
                            .Validita_Fine = AGRODATAFINE,
                            .Data_Creazione = Date.Now,
                            .Data_Modifica = Date.Now,
                            .DataInvio = Nothing,
                            .Inviato = 0,
                            .Username_Creazione = objParametri.UtenteUsername,
                            .Username_Modifica = objParametri.UtenteUsername,
                            .Totale_Superficie_UMA_Edit = CDbl(IIf(IsNothing(objRiga.Item("sup_tot")), 0, objRiga.Item("sup_tot"))),
                            .Zona_Pendenza_A_UMA_Edit = CDbl(IIf(IsNothing(objRiga.Item("supA_Edit")), 0, objRiga.Item("supA_Edit"))),
                            .Zona_Pendenza_B_UMA_Edit = CDbl(IIf(IsNothing(objRiga.Item("supB_Edit")), 0, objRiga.Item("supB_Edit"))),
                            .Zona_Tessitura_Media_UMA_Edit = CDbl(IIf(IsNothing(objRiga.Item("tessitura_Media_Edit")), 0, objRiga.Item("tessitura_Media_Edit"))),
                            .Zona_Tessitura_Normale_UMA_Edit = CDbl(IIf(IsNothing(objRiga.Item("tessitura_Norm_Edit")), 0, objRiga.Item("tessitura_Norm_Edit"))),
                            .Zona_Tessitura_Tenace_UMA_Edit = CDbl(IIf(IsNothing(objRiga.Item("tessitura_Tenace_Edit")), 0, objRiga.Item("tessitura_Tenace_Edit"))),
                            .Regolamento_Cod = IIf(CInt(objRiga.Item("Regolamento_Cod")) = 0, 1, CInt(objRiga.Item("Regolamento_Cod")))
                            }
                            '.Piva = CStr(objRiga.Item("piva")),
                            '.Piva_SuperUser = objParametri.PivaSuperUser,
                            '.Richiesta_Cod = Richiesta_Cod,
                            '.Programmazione_Cod = CStr(IIf(IsNothing(objRiga.Item("Programmazione_Cod")), -1, objRiga.Item("Programmazione_Cod"))),
                            '.Gruppo_Colturale_UMA = CStr(objRiga.Item("Macrouso_UMA_Cod")),
                            '.Totale_Superficie_UMA = CDbl(CDbl(objRiga.Item("supA")) + CDbl(objRiga.Item("supB"))),
                            '.Zona_Pendenza_A_UMA = CDbl(objRiga.Item("supA")),
                            '.Zona_Pendenza_B_UMA = CDbl(objRiga.Item("supB")),
                            '.Zona_Tessitura_Normale_UMA = CDbl(objRiga.Item("tessitura_Norm")),
                            '.Zona_Tessitura_Media_UMA = CDbl(objRiga.Item("tessitura_Media")),
                            '.Zona_Tessitura_Tenace_UMA = CDbl(objRiga.Item("tessitura_Tenace")),
                            '.Carburante_Richiesto = CInt(objRiga.Item("ltrichiesto")),
                            '.Carburante_Calcolato = CInt(objRiga.Item("fabbisognoCalc")),
                            '.Carburante_Approvato = CInt(objRiga.Item("ltAssegnato")),
                            '.Validita_Inizio = AGRODATAINIZIO,
                            '.Validita_Fine = AGRODATAFINE,
                            '.Data_Creazione = Date.Now,
                            '.Data_Modifica = Date.Now,
                            '.DataInvio = Nothing,
                            '.Inviato = 0,
                            '.Username_Creazione = objParametri.UtenteUsername,
                            '.Username_Modifica = objParametri.UtenteUsername,
                            '.Totale_Superficie_UMA_Edit = CDbl(IIf(IsNothing(objRiga.Item("sup_tot")), 0, objRiga.Item("sup_tot"))),
                            '.Zona_Pendenza_A_UMA_Edit = CDbl(objRiga.Item("supA_Edit")),
                            '.Zona_Pendenza_B_UMA_Edit = CDbl(objRiga.Item("supB_Edit")),
                            '.Zona_Tessitura_Media_UMA_Edit = CDbl(objRiga.Item("tessitura_Media_Edit")),
                            '.Zona_Tessitura_Normale_UMA_Edit = CDbl(objRiga.Item("tessitura_Norm_Edit")),
                            '.Zona_Tessitura_Tenace_UMA_Edit = CDbl(objRiga.Item("tessitura_Tenace_Edit"))

                            If (r.Gruppo_Colturale_UMA <> "") Then
                                EFArrayToInsert.Add(r)
                            End If

                            If r.Programmazione_Cod = -4 Then

                                Dim tempLegami = richiesteR.EstraiLegamiRichiestaxAppezzamento(r.Piva, Richiesta_Cod,
                                                                                  " (a.macrouso_UMA_Cod  = " & r.Gruppo_Colturale_UMA & " OR (a.macrouso_UMA_Cod IS NULL AND b.macrouso_UMA_Cod  = " & r.Gruppo_Colturale_UMA & ") OR (a.macrouso_UMA_Cod IS NULL AND b.macrouso_UMA_Cod IS NULL AND c.macrouso_UMA_Cod  = " & r.Gruppo_Colturale_UMA & ")" &
 "OR (a.macrouso_UMA_Cod IS NULL AND b.macrouso_UMA_Cod IS NULL AND c.macrouso_UMA_Cod IS NULL AND d.macrouso_UMA_Cod  = " & r.Gruppo_Colturale_UMA & ")) ", objParametri)

                                If legamiRApp.Rows.Count = 0 Then

                                    legamiRApp = tempLegami

                                Else

                                    legamiRApp.Merge(tempLegami)

                                End If

                            End If

                            'testata.Item("Carburante_Calcolato") += r.Carburante_Calcolato
                            'testata.Item("Carburante_Richiesto") += r.Carburante_Richiesto
                            'testata.Item("Carburante_Approvato") += r.Carburante_Approvato
                        Next

                    End If

                    If righeModificate <> "" AndAlso righeModificate <> "[]" Then

                        Jupdate = JArray.Parse(righeModificate)

                        'creo i record da inserire
                        For Each objRiga As JObject In Jupdate


                            r = New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                                .Piva = CStr(objRiga.Item("piva")),
                                .Piva_SuperUser = objParametri.PivaSuperUser,
                                .Richiesta_Cod = Richiesta_Cod,
                                .Programmazione_Cod = CStr(objRiga.Item("Programmazione_Cod")),
                                .Gruppo_Colturale_UMA = CStr(objRiga.Item("Macrouso_UMA_Cod")),
                                .Carburante_Richiesto = CInt(objRiga.Item("ltrichiesto")),
                                .Carburante_Calcolato = CInt(objRiga.Item("fabbisognoCalc")),
                                .Carburante_Approvato = CInt(objRiga.Item("ltAssegnato")),
                                .Totale_Superficie_UMA = CDbl(objRiga.Item("supA")) + CDbl(objRiga.Item("supB")),
                                .Zona_Pendenza_A_UMA = CDbl(objRiga.Item("supA")),
                                .Zona_Pendenza_B_UMA = CDbl(objRiga.Item("supB")),
                                .Zona_Tessitura_Normale_UMA = CDbl(objRiga.Item("tessitura_Norm")),
                                .Zona_Tessitura_Media_UMA = CDbl(objRiga.Item("tessitura_Media")),
                                .Zona_Tessitura_Tenace_UMA = CDbl(objRiga.Item("tessitura_Tenace")),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Data_Creazione = Date.Now,
                                .Data_Modifica = Date.Now,
                                .DataInvio = Nothing,
                                .Inviato = 0,
                                .Username_Creazione = objParametri.UtenteUsername,
                                .Username_Modifica = objParametri.UtenteUsername,
                                .Totale_Superficie_UMA_Edit = CDbl(objRiga.Item("sup_tot")),
                                .Zona_Pendenza_A_UMA_Edit = CDbl(objRiga.Item("supA_Edit")),
                                .Zona_Pendenza_B_UMA_Edit = CDbl(objRiga.Item("supB_Edit")),
                                .Zona_Tessitura_Media_UMA_Edit = CDbl(objRiga.Item("tessitura_Media_Edit")),
                                .Zona_Tessitura_Normale_UMA_Edit = CDbl(objRiga.Item("tessitura_Norm_Edit")),
                                .Zona_Tessitura_Tenace_UMA_Edit = CDbl(objRiga.Item("tessitura_Tenace_Edit")),
                                .Regolamento_Cod = IIf(CInt(objRiga.Item("Regolamento_Cod")) = 0, 1, CInt(objRiga.Item("Regolamento_Cod")))
                                }

                            Dim RichiestaDB = (From rd In GiasContext.UMA_Richieste Where rd.Richiesta_Cod = r.Richiesta_Cod And rd.Piva = r.Piva And rd.Gruppo_Colturale_UMA = r.Gruppo_Colturale_UMA And rd.Programmazione_Cod = r.Programmazione_Cod)

                            'If (rigaRichieste.Item("Macrouso_UMA_Cod") <> CStr(objRiga.Item("Macrouso_UMA_Cod")) Or rigaRichieste.Item("Programmazione_Cod") <> CStr(objRiga.Item("Programmazione_Cod"))) Then
                            '    EFArrayToInsert.Add(r)
                            '    Dim rr = Gias_EF_Utility.CopyEntity(GiasContext, r, Nothing, "username", AGRODATAINIZIO)
                            '    rr.Gruppo_Colturale_UMA = CStr(rigaRichieste.Item("Macrouso_UMA_Cod"))
                            '    rr.Programmazione_Cod = CStr(rigaRichieste.Item("Programmazione_Cod"))
                            '    EFArrayToDelete.Add(rr)

                            If RichiestaDB.Count > 0 Then
                                If (RichiestaDB.First.Regolamento_Cod <> r.Regolamento_Cod) Then

                                    EFArrayToDelete.Add(RichiestaDB.First)
                                    EFArrayToInsert.Add(r)

                                Else
                                    If (CStr(objRiga.Item("cambio")) <> Nothing) Then
                                        EFArrayToInsert.Add(r)
                                        Dim rr = Gias_EF_Utility.CopyEntity(GiasContext, r, Nothing, "username", AGRODATAINIZIO)
                                        If (CStr(objRiga.Item("cambio")) = "-1") Then
                                            rr.Gruppo_Colturale_UMA = "0000"
                                            rr.Programmazione_Cod = CStr(objRiga.Item("cambio"))
                                        Else
                                            rr.Gruppo_Colturale_UMA = CStr(objRiga.Item("cambio"))
                                        End If

                                        EFArrayToDelete.Add(rr)

                                    Else

                                        EFArrayToUpdate.Add(r)

                                    End If
                                End If

                            Else
                                Dim tempString = CStr(objRiga.Item("cambio"))
                                Dim RichiestaMacrousoCambiata = (From rd In GiasContext.UMA_Richieste Where rd.Richiesta_Cod = r.Richiesta_Cod And rd.Piva = r.Piva And rd.Gruppo_Colturale_UMA = tempString And rd.Programmazione_Cod = r.Programmazione_Cod)

                                If RichiestaMacrousoCambiata.Count > 0 Then
                                    EFArrayToDelete.Add(RichiestaMacrousoCambiata.First)
                                End If

                                EFArrayToInsert.Add(r)

                            End If
                        Next


                    End If

                    If righeCancellate <> "" AndAlso righeCancellate <> "[]" Then

                        Jdelete = JArray.Parse(righeCancellate)


                        'creo i record da cancellare
                        For Each objRiga As JObject In Jdelete

                            r = New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                                .Piva = CStr(objRiga.Item("piva")),
                                .Piva_SuperUser = objParametri.PivaSuperUser,
                                .Richiesta_Cod = Richiesta_Cod,
                                .Programmazione_Cod = CStr(objRiga.Item("Programmazione_Cod")),
                                .Gruppo_Colturale_UMA = CStr(objRiga.Item("Macrouso_UMA_Cod")),
                                .Totale_Superficie_UMA = CDbl(objRiga.Item("sup_tot")),
                                .Zona_Pendenza_A_UMA = CDbl(objRiga.Item("supA")),
                                .Zona_Pendenza_B_UMA = CDbl(objRiga.Item("supB")),
                                .Zona_Tessitura_Normale_UMA = CDbl(objRiga.Item("tessitura_Norm")),
                                .Zona_Tessitura_Media_UMA = CDbl(objRiga.Item("tessitura_Media")),
                                .Zona_Tessitura_Tenace_UMA = CDbl(objRiga.Item("tessitura_Tenace")),
                                .Validita_Inizio = AGRODATAINIZIO,
                                .Validita_Fine = AGRODATAFINE,
                                .Data_Creazione = Date.Now,
                                .Data_Modifica = Date.Now,
                                .DataInvio = Nothing,
                                .Inviato = 0,
                                .Username_Creazione = objParametri.UtenteUsername,
                                .Username_Modifica = objParametri.UtenteUsername,
                                .Totale_Superficie_UMA_Edit = CDbl(objRiga.Item("sup_tot")),
                                .Zona_Pendenza_A_UMA_Edit = CDbl(objRiga.Item("supA_Edit")),
                                .Zona_Pendenza_B_UMA_Edit = CDbl(objRiga.Item("supB_Edit")),
                                .Zona_Tessitura_Media_UMA_Edit = CDbl(objRiga.Item("tessitura_Media_Edit")),
                                .Zona_Tessitura_Normale_UMA_Edit = CDbl(objRiga.Item("tessitura_Norm_Edit")),
                                .Zona_Tessitura_Tenace_UMA_Edit = CDbl(objRiga.Item("tessitura_Tenace_Edit")),
                                .Regolamento_Cod = IIf(CInt(objRiga.Item("Regolamento_Cod")) = 0, 1, CInt(objRiga.Item("Regolamento_Cod")))
                                }

                            EFArrayToDelete.Add(r)

                            If r.Programmazione_Cod = -4 Then

                                legamiDaRecidere.Add(Tuple.Create(r.Piva, r.Gruppo_Colturale_UMA))

                            End If

                            'testata.Item("Carburante_Calcolato") -= r.Carburante_Calcolato
                            'testata.Item("Carburante_Richiesto") -= r.Carburante_Richiesto
                            'testata.Item("Carburante_Approvato") -= r.Carburante_Approvato
                        Next


                    End If



                    Risultato = richiesteW.Aggiorna_Richieste_Terzisti(EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)

                    If Risultato = 0 Then
                        If legamiRApp.Rows.Count > 0 Then

                            richiestexRegImpianti.InserisciAssociazioniPerNuovaPraticaContoTerzi(pivaRichiesta, legamiRApp, Richiesta_Cod, objParametri)

                        End If

                        If legamiDaRecidere.Count > 0 Then

                            richiestexRegImpianti.EliminaAssociazioni(pivaRichiesta, legamiDaRecidere, Richiesta_Cod, objParametri)

                        End If
                    End If

                    Dim EFArrayLavorazioniToDelete As New ArrayList

                    For Each richiesta_del As AgronicaCoreEntityFramework_POCO.UMA_Richieste In EFArrayToDelete

                        Dim Piva As String = richiesta_del.Piva
                        Dim macrouso_uma As String = richiesta_del.Gruppo_Colturale_UMA

                        Dim lav = (From el In GiasContext.UMA_Richieste_Lavorazioni Where el.Piva_SuperUser = Piva_SuperUser And
                                                                                        el.Piva = Piva And
                                                                                        el.Richiesta_Cod = Richiesta_Cod And
                                                                                        el.Gruppo_Colturale_UMA = macrouso_uma).ToList

                        EFArrayLavorazioniToDelete.AddRange(lav)

                    Next

                    lavorazioniW.Aggiorna_Lavorazioni(New ArrayList, New ArrayList, New ArrayList, EFArrayLavorazioniToDelete, objParametri)

                    Dim tot_Testata = (From umaLav In
                                         GiasContext.UMA_Richieste Where
                                                                       umaLav.Richiesta_Cod = Richiesta_Cod Group umaLav By gruppo = New With {
                                                                                                                          Key .Piva = umaLav.Piva,
                                                                                                                          Key .Richiesta_Cod = umaLav.Richiesta_Cod} Into g = Group,
                                                                                                                          Fabbisogno_Richiesto = Sum(umaLav.Carburante_Richiesto),
                                                                                                                          Fabbisogno_Calcolato = Sum(umaLav.Carburante_Calcolato),
                                                                                                                          Fabbisogno_Assegnato = Sum(umaLav.Carburante_Approvato)).FirstOrDefault

                    If tot_Testata IsNot Nothing Then

                        Dim testata = (From uma In GiasContext.UMA_Richieste_Testata Where uma.Richiesta_Cod = Richiesta_Cod).First

                        testata.Carburante_Approvato = tot_Testata.Fabbisogno_Assegnato
                        testata.Carburante_Calcolato = tot_Testata.Fabbisogno_Calcolato
                        testata.Carburante_Richiesto = tot_Testata.Fabbisogno_Richiesto

                        GiasContext.SaveChanges()

                    End If

                    GiasContext.SaveChanges()

                    If (String.IsNullOrEmpty(MessaggioErrore)) Then
                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If


                End Using
            End Using

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)


        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Risultato

    End Function

    Private Function ControlloSovrapposizioni(ByVal record As UMA_Richieste_Lavorazioni,
                                              ByVal isTerzista As Boolean,
                                              ByVal isIntegrativa As Boolean,
                                              ByVal avanzamento As Integer,
                                              ByRef Errs As String,
                                              ByRef ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim noSovrapposizioni As Boolean = True
        Dim dt As DataTable
        Dim leggi As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R

        If record.Programmazione_Cod < 0 AndAlso record.Programmazione_Cod <> -4 Then
            Return noSovrapposizioni
        End If

        dt = leggi.Leggi_Per_Controllo_Incrociato(record.Piva, record.Programmazione_Cod, "", record.Gruppo_Colturale_UMA, 0, "", "", ObjParametri, isTerzista, record.Richiesta_Cod, record.Lavorazione_UMA, avanzamento, isIntegrativa)

        If dt.Rows.Count > 0 Then

            '------------------------------------------------------------------------------------------------------------------------
            ' Eseguo il controllo solo se non si tratta di una lavorazione multipla
            '------------------------------------------------------------------------------------------------------------------------

            'Dim elencoLavMultiple As New List(Of Integer)({10003, 10013, 10017, 10028, 10076, 10145, 10149, 10159, 10218, 10228})

            Dim elencoLavMultiple = leggi.Leggi_Lista_Lavorazioni_Multiple_Richiesta(record.Richiesta_Cod, ObjParametri)

            If elencoLavMultiple.Count > 0 AndAlso elencoLavMultiple.Any(
                Function(m) m("Lav_UMA_Cod") = record.Lavorazione_UMA AndAlso
                            m("Macrouso_UMA_Cod") = record.Gruppo_Colturale_UMA AndAlso
                            (m("Regolamento_Cod") = record.Regolamento_Cod OrElse m("Regolamento_Cod") = 0)
                ) Then
                Return noSovrapposizioni
            End If

            '------------------------------------------------------------------------------------------------------------------------

            Dim leggiTot As New AgronicaCoreUmaDal.UMA_Richieste_R
            Dim dtTot = leggiTot.Leggi("", record.Richiesta_Cod, record.Gruppo_Colturale_UMA, record.Programmazione_Cod, enumSelezioneVariabile.Selezione_TabellaCompleta, ObjParametri).Rows(0)
            Dim totPendenzaA = record.Zona_Pendenza_A_UMA
            Dim totPendenzaB = record.Zona_Pendenza_B_UMA
            Dim totTessituraMedia = record.Zona_Tessitura_Media_UMA
            Dim totTessituraNormale = record.Zona_Tessitura_Normale_UMA
            Dim totTessituraTenace = record.Zona_Tessitura_Tenace_UMA
            Dim listaZiende As String = ""
            Dim assegnato_Sovrapp As Double = 0.0
            Dim terreno_Assegnato_Sovrapp As Double = 0.0
            Dim terreno_Assegnabile_Sovrapp As Double = 0.0

            For Each row As DataRow In dt.Rows

                If CInt(row.Item("stato_Cod")) = 2005 Then
                    assegnato_Sovrapp += row.Item("Fabbisogno_Assegnato")
                    terreno_Assegnato_Sovrapp += row.Item("Totale_Superficie_UMA")
                    terreno_Assegnabile_Sovrapp += row.Item("Totale_Richiedibile")
                End If

                totPendenzaA += row.Item("Zona_Pendenza_A_UMA")
                totPendenzaB += row.Item("Zona_Pendenza_B_UMA")
                totTessituraMedia += row.Item("Zona_Tessitura_Media_UMA")
                totTessituraNormale += row.Item("Zona_Tessitura_Normale_UMA")
                totTessituraTenace += row.Item("Zona_Tessitura_Tenace_UMA")
                listaZiende += " rendicontazione numero " + row.Item("numero") + " di " + row.Item("rag_soc") + " lavorazione " + row.Item("Lav_UMA_Des") + " "

            Next

            If assegnato_Sovrapp < record.Fabbisogno_Calcolato AndAlso
                terreno_Assegnato_Sovrapp = terreno_Assegnabile_Sovrapp AndAlso
                record.Totale_Superficie_UMA = dtTot.Item("Totale_Superficie_UMA") Then

                'Rimuovo la parte decimale del calcolato
                If record.Fabbisogno_Richiesto > 0 AndAlso assegnato_Sovrapp > (record.Fabbisogno_Calcolato - record.Fabbisogno_Richiesto + (record.Fabbisogno_Calcolato - Int(record.Fabbisogno_Calcolato))) Then

                    noSovrapposizioni = False
                    Errs += "Per la piva: " + record.Piva + " [SOVRAPPOSIZIONE LITRI] su " + listaZiende

                End If
            Else
                If (dtTot.Item("Zona_Pendenza_A_UMA") < totPendenzaA AndAlso dtTot.Item("Zona_Pendenza_A_UMA_Edit") < totPendenzaA) OrElse
                (dtTot.Item("Zona_Pendenza_B_UMA") < totPendenzaB AndAlso dtTot.Item("Zona_Pendenza_B_UMA_Edit") < totPendenzaB) OrElse
                (dtTot.Item("Zona_Tessitura_Media_UMA") < totTessituraMedia AndAlso dtTot.Item("Zona_Tessitura_Media_UMA_Edit") < totTessituraMedia) OrElse
                (dtTot.Item("Zona_Tessitura_Normale_UMA") < totTessituraNormale AndAlso dtTot.Item("Zona_Tessitura_Normale_UMA_Edit") < totTessituraNormale) OrElse
                (dtTot.Item("Zona_Tessitura_Tenace_UMA") < totTessituraTenace AndAlso dtTot.Item("Zona_Tessitura_Tenace_UMA_Edit") < totTessituraTenace) Then

                    noSovrapposizioni = False
                    Errs += "Per la piva: " + record.Piva + " su " + listaZiende

                End If
            End If

        End If

        Return noSovrapposizioni

    End Function

    Public Function ControlloPermessoAcqua(ByVal piva As String,
                                            ByVal richiesta_cod As Integer,
                                              ByVal isTerzista As Boolean,
                                              ByRef Errs As String,
                                              ByRef ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                           Optional ByVal tempPermesso As Double = -1) As Boolean

        Dim noErroriAcqua As Boolean = True
        Dim dtTesta As DataTable
        Dim dtLav As New DataTable
        Dim testaRow As DataRow
        Dim leggiLav As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R
        Dim leggiTesta As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim minCoefficiente As Double = 100.0
        Dim totCarbLavorazioni As Double = 0.0
        Dim permessoAcqua As Double = 0.0

        If Not isTerzista Then

            dtTesta = leggiTesta.Leggi(piva, richiesta_cod, 0, AGRODATAINIZIO, AGRODATAFINE, 0, "", -10, ObjParametri)
            If (dtTesta.Rows.Count > 0) Then
                testaRow = dtTesta.Rows.Item(0)


                dtLav = leggiLav.Leggi_Lavorazioni(piva,
                                               testaRow.Item("Anno"),
                                               testaRow.Item("Avanzamento_Richiesta"),
                                               testaRow.Item("Tipo_Richiesta"),
                                               "",
                                               "",
                                               0,
                                               0,
                                               ObjParametri,
                                               lavorazione_Gias:=LAVCOD_IRRIGAZIONE,
                                               xFiltroAggiuntivo:="ucm.Coeff_acq_distr > 0")

                If IsDBNull(testaRow.Item("permesso_Acqua")) AndAlso testaRow.Item("Richiesta_Integrativa") = 1 Then
                    Dim dtAcqua = leggiTesta.Leggi_rimanenza_iniziale(piva, testaRow.Item("Avanzamento_Richiesta"), 0, testaRow.Item("Anno"), ObjParametri, True)
                    If dtAcqua.Rows.Count > 0 Then
                        Dim temp = dtAcqua.Rows.Item(0).Item("permesso_Acqua")
                        permessoAcqua = IIf(IsDBNull(temp), 0, CDbl(temp))
                    End If
                ElseIf tempPermesso > -1 Then
                    permessoAcqua = tempPermesso
                Else
                    permessoAcqua = CDbl(testaRow.Item("permesso_Acqua"))
                End If

                If testaRow.Item("Richiesta_Integrativa") = 1 Then
                    Dim strFiltroRichiestePrec = "UMA_Richieste_Testata.Richiesta_Cod < " & richiesta_cod &
                " AND Pratiche_Stati_Attuali.Stato_Cod = " & enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo
                    Dim dtTestaPrec = leggiTesta.Leggi(piva,
                                                    0,
                                                    0,
                                                    AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    testaRow.Item("Anno"),
                                                    "",
                                                    enum_UMA_Avanzamento.Richiesta,
                                                    ObjParametri,
                                                    isTerzista:=False,
                                                    xFiltroAggiuntivo:=strFiltroRichiestePrec)
                    For Each testataPrecRow In dtTestaPrec.Rows
                        permessoAcqua += CDbl(testataPrecRow.Item("Permesso_acqua"))
                    Next
                End If

                For Each lav As DataRow In dtLav.Rows
                    If CDbl(lav.Item("Coeff_acq_distr")) < minCoefficiente Then
                        minCoefficiente = CDbl(lav.Item("Coeff_acq_distr"))
                    End If
                    totCarbLavorazioni += CDbl(lav.Item("Fabbisogno_Richiesto"))
                Next

                totCarbLavorazioni = Math.Round(totCarbLavorazioni, 0)

                Dim totCarbPermessoAcqua = Math.Round(permessoAcqua * minCoefficiente, 0)

                If totCarbPermessoAcqua < totCarbLavorazioni Then
                    noErroriAcqua = False
                    Errs = String.Format("Le lavorazioni di tipo irrigazione richiedono complessivamente più carburante ({0}L) rispetto quanto consentito dal permesso di attingimento acqua specificato ({1}L)",
                                        totCarbLavorazioni,
                                        totCarbPermessoAcqua)
                End If

            End If

        End If

        Return noErroriAcqua

    End Function

    'CREATO DA GLORIA
    Private Function ControlloSovrapposizioniPerLavParz(ByVal record As UMA_Lavorazioni_Parziali,
                                                        ByVal isTerzista As Boolean,
                                                        ByRef Errs As String,
                                                        ByRef ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim noSovrapposizioni As Boolean = True
        Dim dt As DataTable

        Dim leggi As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R
        dt = leggi.Leggi_Per_Controllo_Incrociato(record.Piva, 0, "", 0, 0, "", "", ObjParametri, isTerzista, record.Richiesta_Cod, record.Lavorazione_UMA)
        'dt = leggi.Leggi_Per_Controllo_Incrociato(record.Piva, record.Programmazione_Cod, "", record.Gruppo_Colturale_UMA, 0, "", "", ObjParametri, isTerzista, record.Richiesta_Cod, record.Lavorazione_UMA)

        If dt.Rows.Count > 0 Then

            '------------------------------------------------------------------------------------------------------------------------
            ' Eseguo il controllo solo se non si tratta di una lavorazione multipla
            '------------------------------------------------------------------------------------------------------------------------

            'Dim elencoLavMultiple As New List(Of Integer)({10003, 10013, 10017, 10028, 10076, 10145, 10149, 10159, 10218, 10228})

            Dim elencoLavMultiple = leggi.Leggi_Lista_Lavorazioni_Multiple_Richiesta(record.Richiesta_Cod, ObjParametri)

            If elencoLavMultiple.Count > 0 AndAlso elencoLavMultiple.Any(
                Function(m) m("Lav_UMA_Cod") = record.Lavorazione_UMA AndAlso
                            (m("Regolamento_Cod") = record.Regolamento_Cod OrElse m("Regolamento_Cod") = 0)
                ) Then
                Return noSovrapposizioni
            End If

            '------------------------------------------------------------------------------------------------------------------------

            Dim leggiTot As New AgronicaCoreUmaDal.UMA_Richieste_R
            Dim dtTot = leggiTot.Leggi("%", record.Richiesta_Cod, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, ObjParametri).Rows(0)
            Dim totPendenzaA = record.Zona_Pendenza_A_UMA
            Dim totPendenzaB = record.Zona_Pendenza_B_UMA
            Dim totTessituraMedia = record.Zona_Tessitura_Media_UMA
            Dim totTessituraNormale = record.Zona_Tessitura_Normale_UMA
            Dim totTessituraTenace = record.Zona_Tessitura_Tenace_UMA
            Dim listaZiende As String = ""

            For Each row As DataRow In dt.Rows

                totPendenzaA += row.Item("Zona_Pendenza_A_UMA")
                totPendenzaB += row.Item("Zona_Pendenza_B_UMA")
                totTessituraMedia += row.Item("Zona_Tessitura_Media_UMA")
                totTessituraNormale += row.Item("Zona_Tessitura_Normale_UMA")
                totTessituraTenace += row.Item("Zona_Tessitura_Tenace_UMA")
                listaZiende += " richiesta/rendicontazione numero " + row.Item("numero") + " di " + row.Item("rag_soc") + " lavorazione " + row.Item("Lav_UMA_Des") + " "

            Next

            If ((dtTot.Item("Zona_Pendenza_A_UMA") < totPendenzaA OrElse dtTot.Item("Zona_Pendenza_B_UMA") < totPendenzaB) OrElse
                (dtTot.Item("Zona_Tessitura_Media_UMA") < totTessituraMedia OrElse dtTot.Item("Zona_Tessitura_Normale_UMA") < totTessituraNormale OrElse dtTot.Item("Zona_Tessitura_Tenace_UMA") < totTessituraTenace)) Then

                noSovrapposizioni = False
                Errs += "Per la piva: " + record.Piva + " su " + listaZiende

            End If

        End If

        Return noSovrapposizioni

    End Function

    'FINE CREATO DA GLORIA

    'Inserimento/modifica delle lavorazioni in UMA_Richieste_Lavorazioni
    Public Function Aggiorna_Lavorazioni(ByVal Piva As String,
                                         ByVal Richiesta_Cod As Integer,
                                         ByVal IsTerzista As Boolean,
                                         ByVal Programmazione_Cod As Integer,
                                         ByVal righeInserite As String, ByVal righeModificate As String, ByVal righeCancellate As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByVal avanzamento As Integer,
                                         ByVal regolamento_Cod As Integer) As (Integer, String, String)

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.Aggiorna_lavorazioni()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim bOk As Boolean = False
        Dim Jinsert As New JArray
        Dim Jupdate As JArray
        Dim Jdelete As JArray
        Dim lavorazioniR = New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R
        Dim lavorazioniW = New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_W
        Dim r As AgronicaCoreEntityFramework_POCO.UMA_Richieste_Lavorazioni
        Dim macro = New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        ''Recupero tutti i macrousi UMA (che non ottengo dal record inserito)
        'Dim DtMacro As DataTable = macro.Macrousi(objParametri)
        'Dim dicMacro As New Dictionary(Of Integer, String)
        'Dim dicRichieste As New Dictionary(Of String, Tuple(Of Integer, Integer, Integer))
        'Dim appTupla As Tuple(Of Integer, Integer, Integer)
        Dim a = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim EFArrayToInsert As New ArrayList
        Dim EFArrayToUpdate As New ArrayList
        Dim EFArrayToDelete As New ArrayList
        Dim EFArrayUpdateRichieste As New ArrayList
        Dim EFArrayUpdateTestate As New ArrayList
        Dim rigaLavorazione As DataRow
        Dim Risultato As Integer = 0
        Dim ErrSovrapposizioni As String = ""
        Dim ErrPermessiAcqua As String = ""
        Dim TopCode As Integer
        Dim BaseCode As Integer
        AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, 1)
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim dettaglioCodArray As New ArrayList
        Dim inOK As Boolean = False 'per controllare se ci sono stati dei nuovi record inseriti

        ''inserisco i macrousi in un dizionario per poter avere un riferimento immediato codice-macrouso
        'For Each row In DtMacro.Rows
        '    dicMacro.Add(row.Item("Macrouso_UMA_Cod"), row.Item("Macrouso_UMA_Des"))
        'Next

        'preparo un array di cod_dettaglio freschi per i record nuovi
        'non è possibile crearli sul momento per via della transazione perchè crea errori

        Dim isIntegrativa As Boolean = macro.RecuperaSeIntegrativaDaRichiestaCod(Richiesta_Cod, objParametri)

        If righeInserite <> "" And righeInserite <> "[]" Then

            Jinsert = JArray.Parse(righeInserite)
            inOK = True
            For Each record In Jinsert

                dettaglioCodArray.Add(objSequenze.NuovoId_Tabella("UMA_Richieste_Lavorazioni", BaseCode, TopCode, objParametri))

            Next

        End If

        Try

            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            If inOK Then

                Dim index As Integer = 0

                'creo i record da inserire
                For Each objRiga As JObject In Jinsert

                    Dim Validita_Inizio = AGRODATAINIZIO
                    If Not IsDBNull(objRiga.Item("Validita_Inizio")) AndAlso Not objRiga.Item("Validita_Inizio") Is Nothing Then
                        Validita_Inizio = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objRiga.Item("Validita_Inizio")), a)
                    End If
                    Dim Validita_Fine = AGRODATAFINE
                    If Not IsDBNull(objRiga.Item("Validita_Fine")) AndAlso Not objRiga.Item("Validita_Fine") Is Nothing Then
                        Validita_Fine = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objRiga.Item("Validita_Fine")), a)
                    End If

                    r = New UMA_Richieste_Lavorazioni With {
                        .Piva = Piva,
                        .Piva_SuperUser = objParametri.PivaSuperUser,
                        .Lavorazione_GIAS = CStr(objRiga.Item("LAV_COD")),
                        .Lavorazione_UMA = CStr(objRiga.Item("Lav_UMA_Cod")),
                        .Gruppo_Colturale_UMA = CStr(objRiga.Item("Macrouso_UMA_Cod")),
                        .Programmazione_Cod = Programmazione_Cod,
                        .Richiesta_Cod = CInt(objRiga.Item("Richiesta_Cod")),
                        .Fabbisogno_Assegnato = CDbl(objRiga.Item("ltAssegnato")),
                        .Fabbisogno_Calcolato = CDbl(objRiga.Item("fabbisognoCalc")),
                        .Fabbisogno_Richiesto = CDbl(objRiga.Item("ltrichiesto")),
                        .Username_Creazione = objParametri.UtenteUsername,
                        .Username_Modifica = objParametri.UtenteUsername,
                        .Nr_Lavorazioni_Previste = CInt(objRiga.Item("nLavPreviste")),
                        .Nr_Lavorazioni_Richieste = CInt(objRiga.Item("nLavRichieste")),
                        .Piu_Lavorazioni_Previste = CInt(objRiga.Item("nLavPreviste")),
                        .Piu_Raccolti_Previsti = CInt(objRiga.Item("piuLavPreviste")),
                        .Tipo_Carburante = CStr(objRiga.Item("Car_Cod")),
                        .Superficie_Maggiorazione_Trasferimenti = CInt(objRiga("SupMaggiorazioneTrasferimenti")),
                        .Validita_Inizio = Validita_Inizio,
                        .Validita_Fine = Validita_Fine,
                        .Data_Creazione = Date.Now,
                        .Data_Modifica = Date.Now,
                        .Richiesta_Dettaglio_Cod = dettaglioCodArray(index),
                        .Totale_Superficie_UMA = CDbl(objRiga.Item("Superficie_Trattata")),
                        .Zona_Pendenza_A_UMA = CDbl(objRiga.Item("Sup_A")),
                        .Zona_Pendenza_B_UMA = CDbl(objRiga.Item("Sup_B")),
                        .Zona_Tessitura_Normale_UMA = CDbl(objRiga.Item("TerrenoNormale")),
                        .Zona_Tessitura_Media_UMA = CDbl(objRiga.Item("TerrenoMedio")),
                        .Zona_Tessitura_Tenace_UMA = CDbl(objRiga.Item("TerrenoTenace")),
                        .Id_Attivita = CInt(objRiga.Item("Attivita_Cod")),
                        .Note_Compilatore = objRiga.Item("Note_Compilatore"),
                        .Note_Approvatore = objRiga.Item("Note_Approvatore"),
                        .Qta_Manuale = CDbl(objRiga.Item("Qta_Manuale")),
                        .Mesi = CInt(objRiga.Item("Mesi")),
                        .CUAA_Terzista = IIf(objRiga.Item("CUAA") = "", Nothing, objRiga.Item("CUAA")),
                        .Regolamento_Cod = regolamento_Cod
                        }

                    Dim esisteRichiesta As Boolean = CheckEsistenza(r, objParametri)

                    If esisteRichiesta And r.Fabbisogno_Richiesto > 0 Then
                        Dim objRigaCUAA As String = objRiga.Item("CUAA")
                        If objRigaCUAA = "" OrElse objRigaCUAA = Nothing Then
                            If ControlloSovrapposizioni(r, IsTerzista, isIntegrativa, avanzamento, ErrSovrapposizioni, objParametri) Then
                                EFArrayToInsert.Add(r)
                                index += 1
                            End If
                        Else
                            EFArrayToInsert.Add(r)
                            index += 1
                        End If
                    End If
                Next

            End If

            If righeModificate <> "" And righeModificate <> "[]" Then
                Jupdate = JArray.Parse(righeModificate)

                For Each objRiga As JObject In Jupdate

                    'per le righe modificate prendo il record originale per reperire le informazioni non codifcate nella tabella (quali gli username creazione, data creazione ecc.)
                    rigaLavorazione = lavorazioniR.Leggi(Piva, objRiga.Item("Macrouso_UMA_Cod"), Programmazione_Cod, CInt(objRiga.Item("Richiesta_Cod")), objRiga.Item("richiestaDettaglioCod"), enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri).Rows.Item(0)

                    Dim Validita_Inizio = AGRODATAINIZIO
                    If Not IsDBNull(objRiga.Item("Validita_Inizio")) AndAlso Not objRiga.Item("Validita_Inizio") Is Nothing Then
                        Validita_Inizio = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objRiga.Item("Validita_Inizio")), a)
                    End If
                    Dim Validita_Fine = AGRODATAFINE
                    If Not IsDBNull(objRiga.Item("Validita_Fine")) AndAlso Not objRiga.Item("Validita_Fine") Is Nothing Then
                        Validita_Fine = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objRiga.Item("Validita_Fine")), a)
                    End If

                    r = New UMA_Richieste_Lavorazioni With {
                        .Piva = Piva,
                        .Piva_SuperUser = objParametri.PivaSuperUser,
                        .Lavorazione_GIAS = CStr(objRiga.Item("LAV_COD")),
                        .Lavorazione_UMA = CStr(objRiga.Item("Lav_UMA_Cod")),
                        .Gruppo_Colturale_UMA = CStr(objRiga.Item("Macrouso_UMA_Cod")),
                        .Richiesta_Cod = CInt(objRiga.Item("Richiesta_Cod")),
                        .Programmazione_Cod = Programmazione_Cod,
                        .Fabbisogno_Assegnato = CDbl(objRiga.Item("ltAssegnato")),
                        .Fabbisogno_Calcolato = CDbl(objRiga.Item("fabbisognoCalc")),
                        .Fabbisogno_Richiesto = CDbl(objRiga.Item("ltrichiesto")),
                        .Username_Creazione = objParametri.UtenteUsername,
                        .Username_Modifica = objParametri.UtenteUsername,
                        .Nr_Lavorazioni_Previste = CInt(objRiga.Item("nLavPreviste")),
                        .Nr_Lavorazioni_Richieste = CInt(objRiga.Item("nLavRichieste")),
                        .Piu_Lavorazioni_Previste = CInt(objRiga.Item("nLavPreviste")),
                        .Piu_Raccolti_Previsti = CInt(objRiga.Item("piuLavPreviste")),
                        .Tipo_Carburante = CStr(objRiga.Item("Car_Cod")),
                        .Superficie_Maggiorazione_Trasferimenti = CDbl(objRiga("SupMaggiorazioneTrasferimenti")),
                        .Validita_Inizio = Validita_Inizio,
                        .Validita_Fine = Validita_Fine,
                        .Data_Creazione = Date.Now,
                        .Data_Modifica = Date.Now,
                        .Richiesta_Dettaglio_Cod = CInt(rigaLavorazione.Item("Richiesta_Dettaglio_Cod")),
                        .Totale_Superficie_UMA = CDbl(objRiga.Item("Superficie_Trattata")),
                        .Zona_Pendenza_A_UMA = CDbl(objRiga.Item("Sup_A")),
                        .Zona_Pendenza_B_UMA = CDbl(objRiga.Item("Sup_B")),
                        .Zona_Tessitura_Normale_UMA = CDbl(objRiga.Item("TerrenoNormale")),
                        .Zona_Tessitura_Media_UMA = CDbl(objRiga.Item("TerrenoMedio")),
                        .Zona_Tessitura_Tenace_UMA = CDbl(objRiga.Item("TerrenoTenace")),
                        .Id_Attivita = CInt(objRiga.Item("Attivita_Cod")),
                        .Note_Compilatore = objRiga.Item("Note_Compilatore"),
                        .Note_Approvatore = objRiga.Item("Note_Approvatore"),
                        .Qta_Manuale = CDbl(objRiga.Item("Qta_Manuale")),
                        .Mesi = CInt(objRiga.Item("Mesi")),
                        .CUAA_Terzista = IIf(objRiga.Item("CUAA") = "", Nothing, CStr(objRiga.Item("CUAA"))),
                        .Regolamento_Cod = regolamento_Cod
                        }

                    ''qui vengono considerati solo i litri aggiunti/rimossi con le varie modifiche
                    ''alla fine nel dictionary saranno presenti i litri da AGGIUNGERE alle varie richieste
                    'If (dicRichieste.ContainsKey(r.Gruppo_Colturale_UMA)) Then
                    '    appTupla = dicRichieste(r.Gruppo_Colturale_UMA)
                    '    dicRichieste.Remove(r.Gruppo_Colturale_UMA)
                    '    dicRichieste.Add(r.Gruppo_Colturale_UMA, Tuple.Create(r.Fabbisogno_Calcolato - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Calcolato")) + appTupla.Item1,
                    '                                                          r.Fabbisogno_Richiesto - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Richiesto")) + appTupla.Item2,
                    '                                                          r.Fabbisogno_Assegnato - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Assegnato")) + appTupla.Item3))
                    'Else
                    '    dicRichieste.Add(r.Gruppo_Colturale_UMA, Tuple.Create(r.Fabbisogno_Calcolato - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Calcolato")),
                    '                                                          r.Fabbisogno_Richiesto - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Richiesto")),
                    '                                                          r.Fabbisogno_Assegnato - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Assegnato"))))
                    'End If

                    Dim objRigaCUAA As String = objRiga.Item("CUAA")
                    If objRigaCUAA = "" OrElse objRigaCUAA = Nothing Then
                        If ControlloSovrapposizioni(r, IsTerzista, isIntegrativa, avanzamento, ErrSovrapposizioni, objParametri) Then
                            EFArrayToUpdate.Add(r)
                        End If
                    Else
                        EFArrayToUpdate.Add(r)
                    End If

                Next

            End If

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    '###########################################################################################################################################
                    '################################### NUOVI INSERIMENTI/MODIFICHE ###########################################################################
                    '###########################################################################################################################################

                    If righeCancellate <> "" And righeCancellate <> "[]" Then
                        Jdelete = JArray.Parse(righeCancellate)

                        For Each objRiga As JObject In Jdelete

                            Dim Piva_del = CStr(objRiga.Item("Piva"))
                            Dim Gruppo_Colturale_UMA_del = CStr(objRiga.Item("Macrouso_UMA_Cod"))
                            Richiesta_Cod = CInt(objRiga.Item("Richiesta_Cod"))
                            Dim Richiesta_Dettaglio_Cod = CInt(objRiga.Item("richiestaDettaglioCod"))
                            Dim Programmazione_Cod_Del = CInt(objRiga.Item("Programmazione_Cod"))

                            If Richiesta_Dettaglio_Cod <> 0 Then
                                r = (From l In GiasContext.UMA_Richieste_Lavorazioni Where l.Piva = Piva_del And
                                                                                           l.Gruppo_Colturale_UMA = Gruppo_Colturale_UMA_del And
                                                                                           l.Richiesta_Cod = Richiesta_Cod And
                                                                                           l.Programmazione_Cod = Programmazione_Cod_Del And
                                                                                           l.Richiesta_Dettaglio_Cod = Richiesta_Dettaglio_Cod).FirstOrDefault


                                EFArrayToDelete.Add(r)
                            End If

                        Next

                    End If

                    'Da scommentare e ricontrollare quando si potrà aggiornare entity framework
                    'EFArrayUpdateTestate = CreaRecordUpdateTestate(Piva, Richiesta_Cod, dicRichieste, objParametri)

                    'Creo tutti i record di UMA_Richieste aggiornati
                    'EFArrayUpdateRichieste = CreaRecordUpdateRichieste(Piva, Richiesta_Cod, dicRichieste, objParametri)

                    Risultato = lavorazioniW.Aggiorna_Lavorazioni(EFArrayToInsert, EFArrayToUpdate, EFArrayUpdateRichieste, EFArrayToDelete, objParametri)

                    If ControlloPermessoAcqua(Piva, Richiesta_Cod, IsTerzista, ErrPermessiAcqua, objParametri) Then


                        'Aggiorno la tabella delle testate con i nuovi totali di carburante
                        AggiornaTotaliCarburante(GiasContext, objParametri.PivaSuperUser, Piva, Richiesta_Cod, objParametri.UtenteUsername)

                        'If avanzamento = 0 Then
                        '    ControlloAnticipo(Richiesta_Cod, objParametri, IsTerzista, MessaggioErrore)
                        'End If

                    End If

                    If (String.IsNullOrEmpty(MessaggioErrore) AndAlso String.IsNullOrEmpty(ErrPermessiAcqua)) Then
                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If

                End Using

            End Using

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)

        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return (Risultato, ErrSovrapposizioni, ErrPermessiAcqua)

    End Function

    Private Function CheckEsistenza(ByVal lavorazione As UMA_Richieste_Lavorazioni,
                                    ByVal objParametri As AgronicaCoreParametri) As Boolean

        Dim esiste As Boolean
        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.checkEsistenza()"
        Dim log As New AgronicaCoreDataProvider.LogProvider

        Try

            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    Dim res = (From rich In GiasContext.UMA_Richieste Where rich.Piva = lavorazione.Piva And
                                                                            rich.Gruppo_Colturale_UMA = lavorazione.Gruppo_Colturale_UMA And
                                                                            rich.Richiesta_Cod = lavorazione.Richiesta_Cod And
                                                                            rich.Programmazione_Cod = lavorazione.Programmazione_Cod).FirstOrDefault

                    esiste = Not IsNothing(res)

                End Using
            End Using

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)


        End Try

        Return esiste

    End Function

    Public Function EliminaRichiesta(ByVal Piva As String,
                                     ByVal richiestaCod As String,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.EliminaRichiesta()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim testataW = New AgronicaCoreUmaDal.UMA_Richieste_Testata_W
        Dim richiesteR = New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim lavorazioniR = New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R
        Dim EFArrayToDeleteTestate As New ArrayList
        Dim EFArrayToDeleteRichieste As New ArrayList
        Dim EFArrayToDeleteLavorazioni As New ArrayList
        Dim EFArrayToDeleteLavorazioniParz As New ArrayList
        Dim SuperPiva = objParametri.PivaSuperUser
        Dim Piva_del = CStr(Piva)
        Dim Richiesta_Cod = CInt(richiestaCod)
        Dim richiestexRegImpianti As New AgronicaCoreUmaDal.UMA_RichiesteXReg_Impianti_W
        Dim Risultato As Integer = 0

        Try

            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    '############################################################################################################################
                    '################################### ELIMINAZIONE ###########################################################################
                    '############################################################################################################################
                    Dim pratica_Cod As Integer

                    Dim objProfilazione_W As New AgronicaCoreProfilazioneDAL.Pratiche_W
                    Dim objProfilazione_Pratiche_StatiW As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_W
                    Dim objProfilazione_Pratiche_Stati_AttualiW As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_W
                    Dim objProfilazione_R As New AgronicaCoreProfilazioneDAL.Pratiche_R

                    Dim rT = (From l In GiasContext.UMA_Richieste_Testata Where l.Piva = Piva_del And
                                                                                        l.Richiesta_Cod = Richiesta_Cod And
                                                                                        l.Piva_SuperUser = SuperPiva).First

                    pratica_Cod = rT.Pratica_Cod

                    If pratica_Cod = 0 Then
                        Throw New Exception("Pratica non impostata")
                    End If

                    Dim dtPratica = objProfilazione_R.Leggi_conStatoAttuale(pratica_Cod, "", "", "", 0, 0, 0, enum_Servizi.Gestione_UMA, enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri, 0, 0)

                    If dtPratica.Rows.Count = 0 Then
                        Throw New Exception("Pratica non più nello stato iniziale")
                    End If

                    objProfilazione_W.Cancella(pratica_Cod, "", objParametri)
                    objProfilazione_Pratiche_StatiW.Cancella(pratica_Cod, 0, "", objParametri)
                    objProfilazione_Pratiche_Stati_AttualiW.Cancella(pratica_Cod, "", objParametri)

                    EFArrayToDeleteTestate.Add(rT)

                    Dim rR = (From l In GiasContext.UMA_Richieste Where l.Richiesta_Cod = Richiesta_Cod And
                                                                      l.Piva_SuperUser = SuperPiva).ToList

                    EFArrayToDeleteRichieste.AddRange(rR)

                    Dim rL = (From l In GiasContext.UMA_Richieste_Lavorazioni Where l.Richiesta_Cod = Richiesta_Cod And
                                                                      l.Piva_SuperUser = SuperPiva).ToList

                    EFArrayToDeleteLavorazioni.AddRange(rL)

                    'AGGIUNTO DA GLORIA PER ELIMINARE LE LAVORAZIONI PARZIALI 

                    Dim rLP = (From l In GiasContext.UMA_Lavorazioni_Parziali Where l.Richiesta_Cod = Richiesta_Cod And
                                                                      l.Piva_SuperUser = SuperPiva).ToList

                    EFArrayToDeleteLavorazioniParz.AddRange(rLP)

                    'FINE 

                    Risultato = testataW.Elimina_Richiesta(GiasContext, EFArrayToDeleteTestate, EFArrayToDeleteRichieste, EFArrayToDeleteLavorazioni, EFArrayToDeleteLavorazioniParz, objParametri)

                    If Risultato = 0 Then
                        richiestexRegImpianti.EliminaAssociazioni(Piva, New List(Of Tuple(Of String, String)), Richiesta_Cod, objParametri)
                    End If

                    GiasContext.SaveChanges()

                    If (String.IsNullOrEmpty(MessaggioErrore)) Then
                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If


                End Using
            End Using



        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)


        End Try


        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return Risultato


    End Function

    Private Function CreaRecordUpdateTestate(Piva As String,
                                             r_cod As Integer,
                                             dicRichieste As Dictionary(Of String, Tuple(Of Integer, Integer, Integer)),
                                             objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As ArrayList

        Dim testate As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim updateRichieste As New ArrayList
        Dim testata As DataRow
        testata = testate.Leggi(Piva, r_cod, 0, AGRODATAINIZIO, AGRODATAFINE, objParametri).Rows.Item(0)
        Dim r As AgronicaCoreEntityFramework_POCO.UMA_Richieste_Testata
        Dim calcolato As Integer
        Dim richiesto As Integer
        Dim Assegnato As Integer
        Dim richiesta_cod As Integer = testate.CheckTestate(Piva, r_cod, -2, objParametri).Rows(0)("Richiesta_Cod")

        For Each tup In dicRichieste.Values

            calcolato += tup.Item1
            richiesto += tup.Item2
            Assegnato += tup.Item3

        Next

        r = New AgronicaCoreEntityFramework_POCO.UMA_Richieste_Testata With {
                            .Piva_SuperUser = testata.Item("Piva_SuperUser"),
                            .Piva = Piva,
                            .Richiesta_Cod = richiesta_cod,
                            .Pratica_Cod = testata.Item("Pratica_Cod"),
                            .Username_Creazione = testata.Item("Username_Creazione"),
                            .Username_Modifica = testata.Item("Username_Modifica"),
                            .Validita_Inizio = testata.Item("Validita_Inizio"),
                            .Validita_Fine = testata.Item("Validita_Fine"),
                            .Inviato = testata.Item("Inviato"),
                            .DataInvio = testata.Item("Data_Invio"),
                            .Data_Creazione = testata.Item("Data_Creazione"),
                            .Data_Modifica = testata.Item("Data_Modifica")
                            }
        '.Carburante_Calcolato = calcolato,
        '.Carburante_Richiesto = richiesto,
        '.Carburante_Approvato = Assegnato,
        '.Tipo_Richiesta = testata.Item("Tipo_Richiesta")


        updateRichieste.Add(r)

        Return updateRichieste

    End Function

    Private Function CreaRecordUpdateRichieste(Piva As String,
                                               richiesta_cod As Integer,
                                               dicRichieste As Dictionary(Of String, Tuple(Of Integer, Integer, Integer)),
                                               objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As ArrayList

        Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim enumer = dicRichieste.GetEnumerator
        Dim updateRichieste As New ArrayList
        Dim richiesta As DataRow
        Dim r As AgronicaCoreEntityFramework_POCO.UMA_Richieste

        Do While enumer.MoveNext

            richiesta = richieste.Leggi(Piva, richiesta_cod, enumer.Current.Key, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri).Rows.Item(0)

            r = New AgronicaCoreEntityFramework_POCO.UMA_Richieste With {
                            .Piva_SuperUser = richiesta.Item("Piva_SuperUser"),
                            .Piva = Piva,
                            .Gruppo_Colturale_UMA = richiesta.Item("Gruppo_Colturale_UMA"),
                            .Richiesta_Cod = richiesta_cod,
                            .Programmazione_Cod = richiesta.Item("Programmazione_Cod"),
                            .Totale_Superficie_UMA = richiesta.Item("Totale_Superficie_UMA"),
                            .Zona_Pendenza_A_UMA = richiesta.Item("Zona_Pendenza_A_UMA"),
                            .Zona_Pendenza_B_UMA = richiesta.Item("Zona_Pendenza_B_UMA"),
                            .Carburante_Approvato = richiesta.Item("Carburante_Approvato") + enumer.Current.Value.Item3,
                            .Carburante_Calcolato = richiesta.Item("Carburante_Calcolato") + enumer.Current.Value.Item1,
                            .Carburante_Richiesto = richiesta.Item("Carburante_Richiesto") + enumer.Current.Value.Item2,
                            .Zona_Tessitura_Media_UMA = richiesta.Item("Zona_Tessitura_Media_UMA"),
                            .Zona_Tessitura_Normale_UMA = richiesta.Item("Zona_Tessitura_Normale_UMA"),
                            .Zona_Tessitura_Tenace_UMA = richiesta.Item("Zona_Tessitura_Tenace_UMA"),
                            .Username_Creazione = richiesta.Item("Username_Creazione"),
                            .Username_Modifica = richiesta.Item("Username_Modifica"),
                            .Validita_Inizio = richiesta.Item("Validita_Inizio"),
                            .Validita_Fine = richiesta.Item("Validita_Fine"),
                            .Inviato = richiesta.Item("Inviato"),
                            .Data_Creazione = richiesta.Item("Data_Creazione"),
                            .Data_Modifica = richiesta.Item("Data_Modifica")
            }

            updateRichieste.Add(r)

        Loop

        Return updateRichieste

    End Function

    Public Sub AssegnaAutomaticamenteCarburante(Piva As String, richiesta_cod As Integer, Percentuale_Decurtamento As Double, lav_Parziali As Boolean, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.AssegnaAutomaticamenteCarburante()"
        Dim gefutils As New Gias_EF_Utility
        Dim percentualeNulla As Boolean = Percentuale_Decurtamento = 0
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)
        Dim transactionOptions = New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted

        Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                Try

                    Dim totBenzAppro As Integer
                    Dim totGasolAppro As Integer
                    Dim totGasolSerraAppro As Integer
                    If lav_Parziali Then
                        Dim lavrazioni_Parziali = (From rl In GiasContext.UMA_Lavorazioni_Parziali Where rl.Richiesta_Cod = richiesta_cod).ToList
                        For Each ric In lavrazioni_Parziali
                            ric.Fabbisogno_Assegnato = If(percentualeNulla, ric.Fabbisogno_Richiesto,
                                                           Math.Round(ric.Fabbisogno_Richiesto * ((100 - Percentuale_Decurtamento) / 100), 0, MidpointRounding.AwayFromZero))
                            If ric.Tipo_Carburante = enum_TipoCarburante_UMA.Benzina Then
                                totBenzAppro += ric.Fabbisogno_Assegnato
                            ElseIf ric.Tipo_Carburante = enum_TipoCarburante_UMA.Gasolio Then
                                totGasolAppro += ric.Fabbisogno_Assegnato
                            ElseIf ric.Tipo_Carburante = enum_TipoCarburante_UMA.Gasolio_Serra Then
                                totGasolSerraAppro += ric.Fabbisogno_Assegnato
                            End If
                            ric.Data_Modifica = DateTime.Now
                            ric.Username_Modifica = objParametri.UtenteUsername
                            GiasContext.Entry(ric).State = EntityState.Modified
                        Next
                    Else
                        Dim richieste_lavorazioni = (From rl In GiasContext.UMA_Richieste_Lavorazioni Where rl.Richiesta_Cod = richiesta_cod).ToList
                        For Each ric In richieste_lavorazioni
                            ric.Fabbisogno_Assegnato = If(percentualeNulla, ric.Fabbisogno_Richiesto,
                                                           Math.Round(ric.Fabbisogno_Richiesto * ((100 - Percentuale_Decurtamento) / 100), 0, MidpointRounding.AwayFromZero))
                            If ric.Tipo_Carburante = enum_TipoCarburante_UMA.Benzina Then
                                totBenzAppro += ric.Fabbisogno_Assegnato
                            ElseIf ric.Tipo_Carburante = enum_TipoCarburante_UMA.Gasolio Then
                                totGasolAppro += ric.Fabbisogno_Assegnato
                            ElseIf ric.Tipo_Carburante = enum_TipoCarburante_UMA.Gasolio_Serra Then
                                totGasolSerraAppro += ric.Fabbisogno_Assegnato
                            End If
                            ric.Data_Modifica = DateTime.Now
                            ric.Username_Modifica = objParametri.UtenteUsername
                            GiasContext.Entry(ric).State = EntityState.Modified
                        Next
                    End If
                    GiasContext.SaveChanges()

                    Dim queryUmaRichAllevamenti =
                        From umaRichAllev In GiasContext.UMA_Richieste_Allevamenti
                        Where umaRichAllev.Richiesta_Cod = richiesta_cod

                    For Each richAllev In queryUmaRichAllevamenti
                        richAllev.Carburante_Approvato = If(percentualeNulla, richAllev.Carburante_Richiesto,
                                                           Math.Round(richAllev.Carburante_Richiesto.Value * ((100 - Percentuale_Decurtamento) / 100), 0, MidpointRounding.AwayFromZero))
                        If richAllev.Tipo_Carburante = 3 Then
                            totBenzAppro += richAllev.Carburante_Approvato
                        ElseIf richAllev.Tipo_Carburante = 2 Then
                            totGasolAppro += richAllev.Carburante_Approvato
                        ElseIf richAllev.Tipo_Carburante = 8 Then
                            totGasolSerraAppro += richAllev.Carburante_Approvato
                        End If
                        richAllev.Data_Modifica = DateTime.Now
                        richAllev.Username_Modifica = objParametri.UtenteUsername
                        GiasContext.Entry(richAllev).State = EntityState.Modified
                    Next


                    Dim Richiesta_Testata = (From rt In GiasContext.UMA_Richieste_Testata Where rt.Piva = Piva And
                                                                                                      rt.Richiesta_Cod = richiesta_cod).FirstOrDefault

                    If Richiesta_Testata IsNot Nothing Then
                        If Richiesta_Testata.Richiesta_Iniziale_Gasolio IsNot Nothing AndAlso Richiesta_Testata.Richiesta_Iniziale_Gasolio > 0 Then
                            Richiesta_Testata.Approvazione_Iniziale_Gasolio = If(percentualeNulla, CDbl(Richiesta_Testata.Richiesta_Iniziale_Gasolio),
                                                           Math.Round(CDbl(Richiesta_Testata.Richiesta_Iniziale_Gasolio) * ((100 - Percentuale_Decurtamento) / 100), 0, MidpointRounding.AwayFromZero))
                            Richiesta_Testata.Data_Modifica = DateTime.Now
                            Richiesta_Testata.Username_Modifica = objParametri.UtenteUsername
                        ElseIf (Richiesta_Testata.Tipo_Richiesta = -1 AndAlso totGasolAppro > 0) Then
                            Richiesta_Testata.Approvazione_Iniziale_Gasolio = totGasolAppro
                            Richiesta_Testata.Richiesta_Iniziale_Gasolio = If(percentualeNulla, totGasolAppro,
                                                           Math.Round(CDbl(totGasolAppro * 100 / (100 - Percentuale_Decurtamento)), 0))
                            Richiesta_Testata.Data_Modifica = DateTime.Now
                            Richiesta_Testata.Username_Modifica = objParametri.UtenteUsername
                        End If

                        If Richiesta_Testata.Richiesta_Iniziale_Benzina IsNot Nothing AndAlso Richiesta_Testata.Richiesta_Iniziale_Benzina > 0 Then
                            Richiesta_Testata.Approvazione_Iniziale_Benzina = If(percentualeNulla, Richiesta_Testata.Richiesta_Iniziale_Benzina,
                                                           Math.Round(CDbl(Richiesta_Testata.Richiesta_Iniziale_Benzina) * ((100 - Percentuale_Decurtamento) / 100), 0, MidpointRounding.AwayFromZero))
                            Richiesta_Testata.Data_Modifica = DateTime.Now
                            Richiesta_Testata.Username_Modifica = objParametri.UtenteUsername
                        ElseIf (Richiesta_Testata.Tipo_Richiesta = -1 AndAlso totBenzAppro > 0) Then
                            Richiesta_Testata.Approvazione_Iniziale_Benzina = totBenzAppro
                            Richiesta_Testata.Richiesta_Iniziale_Benzina = If(percentualeNulla, totBenzAppro,
                                                           Math.Round(CDbl(totBenzAppro * 100 / (100 - Percentuale_Decurtamento)), 0, MidpointRounding.AwayFromZero))
                            Richiesta_Testata.Data_Modifica = DateTime.Now
                            Richiesta_Testata.Username_Modifica = objParametri.UtenteUsername
                        End If

                        If Richiesta_Testata.Richiesta_Iniziale_Gasolio_Serra IsNot Nothing AndAlso Richiesta_Testata.Richiesta_Iniziale_Gasolio_Serra > 0 Then
                            Richiesta_Testata.Approvazione_Iniziale_Gasolio_Serra = If(percentualeNulla, Richiesta_Testata.Richiesta_Iniziale_Gasolio_Serra,
                                                           Math.Round(CDbl(Richiesta_Testata.Richiesta_Iniziale_Gasolio_Serra) * ((100 - Percentuale_Decurtamento) / 100), 0, MidpointRounding.AwayFromZero))
                            Richiesta_Testata.Data_Modifica = DateTime.Now
                            Richiesta_Testata.Username_Modifica = objParametri.UtenteUsername
                        ElseIf (Richiesta_Testata.Tipo_Richiesta = -1 AndAlso totGasolSerraAppro > 0) Then
                            Richiesta_Testata.Approvazione_Iniziale_Gasolio_Serra = totGasolSerraAppro
                            Richiesta_Testata.Richiesta_Iniziale_Gasolio_Serra = If(percentualeNulla, totGasolSerraAppro,
                                                           Math.Round(CDbl(totGasolSerraAppro * 100 / (100 - Percentuale_Decurtamento)), 0, MidpointRounding.AwayFromZero))
                            Richiesta_Testata.Data_Modifica = DateTime.Now
                            Richiesta_Testata.Username_Modifica = objParametri.UtenteUsername
                        End If

                    End If

                    GiasContext.SaveChanges()

                    If lav_Parziali Then
                        AggiornaTotaliCarburanteDaLavorazioniParziali(GiasContext, objParametri.PivaSuperUser, richiesta_cod, objParametri.UtenteUsername)
                    Else
                        AggiornaTotaliCarburante(GiasContext, objParametri.PivaSuperUser, Piva, richiesta_cod, objParametri.UtenteUsername)
                    End If

                    ' COMMIT Effettivo
                    GiasContext.SaveChanges()
                    scope.Complete()
                Catch ex As Exception
                    scope.Dispose()
                    Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
                End Try

            End Using

        End Using

    End Sub

    Public Sub CopiaRendicontazioneInRichiesta(ByVal Richiesta_Cod_Old As Integer,
                                                    ByVal Richiesta_Cod_New As Integer,
                                                    ByVal Approva_Automaticamente As Boolean,
                                                    ByVal Note_Approvazione As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.CopiaRendicontazioneInRichiesta()"
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Try
                Dim handleSequenze = New AgronicaCoreDataProvider.Agro_Sequenze()

                Dim Uma_testata_New = (From r In GiasContext.UMA_Richieste_Testata Where r.Richiesta_Cod = Richiesta_Cod_New).FirstOrDefault
                Dim Uma_testata_Old = (From r In GiasContext.UMA_Richieste_Testata Where r.Richiesta_Cod = Richiesta_Cod_Old).FirstOrDefault

                Uma_testata_New.Carburante_Calcolato = Uma_testata_Old.Carburante_Calcolato
                Uma_testata_New.Carburante_Richiesto = Uma_testata_Old.Carburante_Richiesto
                Uma_testata_New.Carburante_Approvato = Uma_testata_Old.Carburante_Approvato
                Uma_testata_New.Richiesta_Origine_Cod = Uma_testata_Old.Richiesta_Cod

                Uma_testata_New.Carburante_Richiesto_Benzina = Uma_testata_Old.Carburante_Richiesto_Benzina
                Uma_testata_New.Carburante_Richiesto_Gasolio = Uma_testata_Old.Carburante_Richiesto_Gasolio
                Uma_testata_New.Carburante_Richiesto_Gasolio_Serra = Uma_testata_Old.Carburante_Richiesto_Gasolio_Serra

                Uma_testata_New.Macchine_Impiegate = Uma_testata_Old.Macchine_Impiegate

                Uma_testata_New.Rimanenza_Benzina = Uma_testata_Old.Rimanenza_Benzina
                Uma_testata_New.Rimanenza_Gasolio = Uma_testata_Old.Rimanenza_Gasolio
                Uma_testata_New.Rimanenza_Gasolio_Serra = Uma_testata_Old.Rimanenza_Gasolio_Serra

                GiasContext.SaveChanges()

                Dim uma_richieste = (From r In GiasContext.UMA_Richieste Where r.Richiesta_Cod = Richiesta_Cod_Old).ToList

                Dim uma_richieste_insert As New ArrayList()
                Dim uma_lavorazioni_insert As New ArrayList()
                Dim uma_allevamenti_insert As New List(Of UMA_Richieste_Allevamenti)

                'Copio Richiesta Gruppi Colturali
                For Each uma_richiesta In uma_richieste
                    Dim uma_richiesta_New = Gias_EF_Utility.CopyEntity(GiasContext, uma_richiesta, Nothing, objParametri.UsernameOperazione, DateTime.Now)
                    uma_richiesta_New.Richiesta_Cod = Richiesta_Cod_New

                    'Copio singole lavorazioni
                    Dim uma_lavorazioni = (From r In GiasContext.UMA_Richieste_Lavorazioni
                                           Where r.Richiesta_Cod = Richiesta_Cod_Old And
                                                   r.Piva = uma_richiesta.Piva And
                                                   r.Gruppo_Colturale_UMA = uma_richiesta.Gruppo_Colturale_UMA And
                                                   r.Programmazione_Cod = uma_richiesta.Programmazione_Cod).ToList

                    For Each uma_lavorazione In uma_lavorazioni

                        Dim uma_lavorazione_New = Gias_EF_Utility.CopyEntity(GiasContext, uma_lavorazione, Nothing, objParametri.UsernameOperazione, DateTime.Now)
                        uma_lavorazione_New.Richiesta_Cod = Richiesta_Cod_New
                        uma_lavorazione_New.Richiesta_Dettaglio_Cod = handleSequenze.NuovoId_Tabella_EF(GiasContext, "UMA_Richieste_Lavorazioni", 0, 2000000000, objParametri)

                        uma_lavorazioni_insert.Add(uma_lavorazione_New)
                    Next

                    uma_richieste_insert.Add(uma_richiesta_New)

                Next

                'Copio Allevamenti
                Dim richAllevamenti = (From allev In GiasContext.UMA_Richieste_Allevamenti
                                       Where allev.Richiesta_Cod = Richiesta_Cod_Old).ToList()

                For Each allev As UMA_Richieste_Allevamenti In richAllevamenti
                    Dim allev_new = Gias_EF_Utility.CopyEntity(GiasContext, allev, Nothing, objParametri.UsernameOperazione, DateTime.Now)
                    allev_new.Richiesta_Cod = Richiesta_Cod_New

                    uma_allevamenti_insert.Add(allev_new)
                Next

                Dim objUMA_Richieste_DAL As New UMA_Richieste_W
                Dim objUMA_Richieste_Lavorazioni_DAL As New UMA_Richieste_Lavorazioni_W
                Dim objUMA_Richieste_Allev_DAL As New UMA_Richieste_Allevamenti_W
                'Aggiorna_Richieste
                objUMA_Richieste_DAL.Aggiorna_Richieste(uma_richieste_insert, New ArrayList, New ArrayList, objParametri)
                objUMA_Richieste_Lavorazioni_DAL.Aggiorna_Lavorazioni(uma_lavorazioni_insert, New ArrayList, New ArrayList, New ArrayList, objParametri)
                objUMA_Richieste_Allev_DAL.Aggiorna_Allevamenti(uma_allevamenti_insert, New List(Of UMA_Richieste_Allevamenti), New List(Of UMA_Richieste_Allevamenti), objParametri)

                If Approva_Automaticamente Then
                    Dim esitoApprovazione = False
                    Dim Note = "L'azienda richiedente certifica che non sono intervenute modifiche sostanziali rispetto alla rendicontazione dell'anno precedente"
                    esitoApprovazione = Approva_Automaticamente_Richiesta(GiasContext, Richiesta_Cod_New, Note, objParametri)

                    If Not esitoApprovazione Then
                        Throw New Exception("Approvazione automatica richiesta fallita")
                    End If

                End If

                ' COMMIT Effettivo
                GiasContext.SaveChanges()

            Catch ex As Exception
                Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
            End Try

        End Using

    End Sub


    Public Function AggiornaRichiesteAllevamenti(
            ByVal piva As String,
            ByVal richiestaCod As Integer,
            ByVal listaInsert As List(Of UMA_Richieste_Allevamenti),
            ByVal listaUpdate As List(Of UMA_Richieste_Allevamenti),
            ByVal listaDelete As List(Of UMA_Richieste_Allevamenti),
            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard()
        r.RispostaOK = True

        Dim gefutils As New Gias_EF_Utility()
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing

        Using scope As New TransactionScope
            Try
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                GiasContext.Database.Connection.Open()

                'objParametri_Server.objConnessione = GiasContext.Database.Connection

                'Leggo da database gli effettivi record della tabella per averne tutti i dati, poi trovo quelli corrispondenti alle mie liste
                'e ne modifico solo i campi necessari 

                For Each del In listaDelete
                    Dim efDel =
                        (From richiesta In GiasContext.UMA_Richieste_Allevamenti
                         Where richiesta.Piva_SuperUser = del.Piva_SuperUser And
                            richiesta.Piva = del.Piva And
                            richiesta.Richiesta_Cod = del.Richiesta_Cod And
                            richiesta.UMA_All_Cod = del.UMA_All_Cod).FirstOrDefault()

                    If efDel IsNot Nothing Then
                        'GiasContext.UMA_Richieste_Allevamenti.Attach(del)
                        GiasContext.UMA_Richieste_Allevamenti.Remove(efDel)
                    End If

                Next
                GiasContext.SaveChanges()

                For Each up In listaUpdate
                    Dim efUp =
                        (From richiesta In GiasContext.UMA_Richieste_Allevamenti
                         Where richiesta.Piva_SuperUser = up.Piva_SuperUser And
                            richiesta.Piva = up.Piva And
                            richiesta.Richiesta_Cod = up.Richiesta_Cod And
                            richiesta.UMA_All_Cod = up.UMA_All_Cod).FirstOrDefault()

                    If efUp IsNot Nothing Then
                        efUp.UMA_All_Cod = up.UMA_All_Cod
                        efUp.Totale_Capi = up.Totale_Capi
                        efUp.Carburante_Calcolato = up.Carburante_Calcolato
                        efUp.Carburante_Richiesto = up.Carburante_Richiesto
                        efUp.Carburante_Approvato = up.Carburante_Approvato
                        efUp.Tipo_Carburante = up.Tipo_Carburante
                        efUp.Note_Compilatore = up.Note_Compilatore
                        efUp.Note_Approvatore = up.Note_Approvatore
                        efUp.Data_Modifica = DateTime.Now
                        efUp.Username_Modifica = objParametri_Server.UtenteUsername

                        'GiasContext.UMA_Richieste_Allevamenti.Attach(up)
                        GiasContext.Entry(efUp).State = Entity.EntityState.Modified
                    End If


                Next
                GiasContext.SaveChanges()

                For Each ins In listaInsert
                    ins.Piva_SuperUser = objParametri_Server.PivaSuperUser
                    ins.Piva = piva
                    ins.Richiesta_Cod = richiestaCod
                    ins.inviato = 0
                    ins.Data_Creazione = DateTime.Now
                    ins.Username_Creazione = objParametri_Server.UtenteUsername
                    ins.Data_Modifica = DateTime.Now
                    ins.Username_Modifica = objParametri_Server.UtenteUsername
                    ins.Validita_Inizio = AGRODATAINIZIO
                    ins.Validita_Fine = AGRODATAFINE
                    GiasContext.UMA_Richieste_Allevamenti.Add(ins)
                Next
                GiasContext.SaveChanges()


                '-------------------------------------------------

                'Aggiorno la tabella delle testate con i nuovi totali di carburante
                AggiornaTotaliCarburante(GiasContext, objParametri_Server.PivaSuperUser, piva, richiestaCod, objParametri_Server.UtenteUsername)

                scope.Complete()

            Catch ex As Exception
                scope.Dispose()

                r.RispostaOK = False
                r.Errore = ex.Message

            Finally
                'Close the opened connection
                If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                    GiasContext.Database.Connection.Close()
                End If

            End Try
        End Using

        Return r
    End Function


    Public Sub AggiornaTotaliCarburante(
            ByRef GiasContext As Gias_DeveloperServer_Entities,
            ByVal Piva_SuperUser As String,
            ByVal Piva As String,
            ByVal Richiesta_Cod As Integer,
            ByVal Username_Modifica As String)

        'Ottengo i valori dei carburanti dalle lavorazioni della testata divisi in gruppi per tipo, per gruppo_colturale_uma e per Programmazione_Cod
        Dim queryUmaRichLavorazioni =
            From umaLav In GiasContext.UMA_Richieste_Lavorazioni
            Where umaLav.Piva_SuperUser = Piva_SuperUser And
                umaLav.Richiesta_Cod = Richiesta_Cod
            Group umaLav By
                _pivaSuperUser = umaLav.Piva_SuperUser,
                _piva = umaLav.Piva,
                _richiestaCod = umaLav.Richiesta_Cod,
                _gruppoCol = umaLav.Gruppo_Colturale_UMA,
                _tipoCarb = umaLav.Tipo_Carburante,
                _progCod = umaLav.Programmazione_Cod
                Into g = Group
            Select New With {
                .Piva_SuperUser = _pivaSuperUser,
                .Piva = _piva,
                .Richiesta_Cod = _richiestaCod,
                .Gruppo_Colturale_UMA = _gruppoCol,
                .Tipo_Carburante = _tipoCarb,
                .Programmazione_Cod = _progCod,
                .Carburante_Calcolato = g.Sum(Function(lav) lav.Fabbisogno_Calcolato),
                .Carburante_Richiesto = g.Sum(Function(lav) lav.Fabbisogno_Richiesto),
                .Carburante_Assegnato = g.Sum(Function(lav) lav.Fabbisogno_Assegnato)
            }

        'Ottengo le richieste relative alla testata
        Dim queryUmaRichieste =
            From umaRich In GiasContext.UMA_Richieste
            Where umaRich.Piva_SuperUser = Piva_SuperUser And
                umaRich.Richiesta_Cod = Richiesta_Cod

        For Each umaRichieste In queryUmaRichieste
            'Per ogni richiesta, calcolo il rispettivo totale carburante in base alle sue lavorazioni
            Dim queryUmaLavGruppoColt = queryUmaRichLavorazioni.Where(Function(lav) lav.Gruppo_Colturale_UMA = umaRichieste.Gruppo_Colturale_UMA AndAlso lav.Piva = umaRichieste.Piva AndAlso lav.Programmazione_Cod = umaRichieste.Programmazione_Cod)

            If queryUmaLavGruppoColt.Count > 0 Then
                Dim flagAggiornoRichiesta As Boolean = False

                Dim umaRichNewCarbCalc = queryUmaLavGruppoColt.Sum(Function(lav) lav.Carburante_Calcolato)
                If umaRichieste.Carburante_Calcolato <> umaRichNewCarbCalc Then
                    umaRichieste.Carburante_Calcolato = umaRichNewCarbCalc

                    flagAggiornoRichiesta = True
                End If

                Dim umaRichNewCarbRich = queryUmaLavGruppoColt.Sum(Function(lav) lav.Carburante_Richiesto)
                If umaRichieste.Carburante_Richiesto <> umaRichNewCarbRich Then
                    umaRichieste.Carburante_Richiesto = umaRichNewCarbRich

                    flagAggiornoRichiesta = True
                End If

                Dim umaRichNewCarbAppr = queryUmaLavGruppoColt.Sum(Function(lav) lav.Carburante_Assegnato)
                If umaRichieste.Carburante_Approvato <> umaRichNewCarbAppr Then
                    umaRichieste.Carburante_Approvato = umaRichNewCarbAppr

                    flagAggiornoRichiesta = True
                End If

                If flagAggiornoRichiesta = True Then
                    umaRichieste.Data_Modifica = DateTime.Now
                    umaRichieste.Username_Modifica = Username_Modifica

                    GiasContext.Entry(umaRichieste).State = Entity.EntityState.Modified
                End If

            ElseIf (umaRichieste.Carburante_Richiesto <> 0 OrElse umaRichieste.Carburante_Calcolato <> 0 OrElse umaRichieste.Carburante_Approvato <> 0) Then
                umaRichieste.Carburante_Richiesto = 0
                umaRichieste.Carburante_Calcolato = 0
                umaRichieste.Carburante_Approvato = 0
                umaRichieste.Data_Modifica = DateTime.Now
                umaRichieste.Username_Modifica = Username_Modifica
                GiasContext.Entry(umaRichieste).State = Entity.EntityState.Modified
            End If

        Next

        GiasContext.SaveChanges()

        'Divido i totali dei carburanti per le richieste di allevamenti, raggruppando per tipo
        Dim queryUmaRichAllevamenti =
            From umaRichAllev In GiasContext.UMA_Richieste_Allevamenti
            Where umaRichAllev.Piva_SuperUser = Piva_SuperUser And
                umaRichAllev.Richiesta_Cod = Richiesta_Cod
            Group umaRichAllev By
                _pivaSuperUser = umaRichAllev.Piva_SuperUser,
                _piva = umaRichAllev.Piva,
                _richiestaCod = umaRichAllev.Richiesta_Cod,
                _tipoCarb = umaRichAllev.Tipo_Carburante
                Into g = Group
            Select New With {
                .Piva_SuperUser = _pivaSuperUser,
                .Piva = _piva,
                .Richiesta_Cod = _richiestaCod,
                .Tipo_Carburante = _tipoCarb,
                .Carburante_Calcolato = g.Sum(Function(richAllev) richAllev.Carburante_Calcolato),
                .Carburante_Richiesto = g.Sum(Function(richAllev) richAllev.Carburante_Richiesto),
                .Carburante_Assegnato = g.Sum(Function(richAllev) richAllev.Carburante_Approvato)
            }

        '------------------------------------------------------------
        'Prelevo il record di testata e lo aggiorno
        Dim umaTestata =
            (From umaTest In GiasContext.UMA_Richieste_Testata
             Where umaTest.Piva_SuperUser = Piva_SuperUser And 'umaTest.Piva = Piva And
                  umaTest.Richiesta_Cod = Richiesta_Cod).First()

        Dim flagAggiornoTestata As Boolean = False

        Dim totaleCarburanteCalcolatoDaLavor As Double = 0
        Dim totaleCarburanteCalcolatoDaAllev As Double = 0
        If queryUmaRichLavorazioni.Count > 0 Then
            totaleCarburanteCalcolatoDaLavor = queryUmaRichLavorazioni.Sum(Function(lavor) lavor.Carburante_Calcolato)
        End If

        If queryUmaRichAllevamenti.Count > 0 Then
            totaleCarburanteCalcolatoDaAllev = queryUmaRichAllevamenti.Sum(Function(allev) allev.Carburante_Calcolato)
        End If

        Dim newCarbCalc = IIf(IsNothing(totaleCarburanteCalcolatoDaLavor), 0, totaleCarburanteCalcolatoDaLavor) + IIf(IsNothing(totaleCarburanteCalcolatoDaAllev), 0, totaleCarburanteCalcolatoDaAllev)
        If umaTestata.Carburante_Calcolato <> newCarbCalc Then
            umaTestata.Carburante_Calcolato = newCarbCalc

            flagAggiornoTestata = True
        End If

        Dim totaleCarburanteRichiestoDaLavor As Double = 0
        Dim totaleCarburanteRichiestoDaAllev As Double = 0
        If queryUmaRichLavorazioni.Count > 0 Then
            totaleCarburanteRichiestoDaLavor = queryUmaRichLavorazioni.Sum(Function(lavor) lavor.Carburante_Richiesto)
        End If

        If queryUmaRichAllevamenti.Count > 0 Then
            totaleCarburanteRichiestoDaAllev = queryUmaRichAllevamenti.Sum(Function(allev) allev.Carburante_Richiesto)
        End If

        Dim newCarbRich = IIf(IsNothing(totaleCarburanteRichiestoDaLavor), 0, totaleCarburanteRichiestoDaLavor) + IIf(IsNothing(totaleCarburanteRichiestoDaAllev), 0, totaleCarburanteRichiestoDaAllev)
        If umaTestata.Carburante_Richiesto <> newCarbRich Then
            umaTestata.Carburante_Richiesto = newCarbRich

            flagAggiornoTestata = True
        End If

        Dim totaleCarburanteAssegnatoDaLavor As Double = 0
        Dim totaleCarburanteAssegnatoDaAllev As Double = 0
        If queryUmaRichLavorazioni.Count > 0 Then
            totaleCarburanteAssegnatoDaLavor = queryUmaRichLavorazioni.Sum(Function(lavor) lavor.Carburante_Assegnato)
        End If

        If queryUmaRichAllevamenti.Count > 0 Then
            totaleCarburanteAssegnatoDaAllev = queryUmaRichAllevamenti.Sum(Function(allev) allev.Carburante_Assegnato)
        End If

        Dim newCarbAppr = IIf(IsNothing(totaleCarburanteAssegnatoDaLavor), 0, totaleCarburanteAssegnatoDaLavor) + IIf(IsNothing(totaleCarburanteAssegnatoDaAllev), 0, totaleCarburanteAssegnatoDaAllev)
        If umaTestata.Carburante_Approvato <> newCarbAppr Then
            umaTestata.Carburante_Approvato = newCarbAppr

            flagAggiornoTestata = True
        End If


        'Totale per la colonna Carburante_Richiesto_Benzina
        Dim totCarburanteRichiestoDaLavorBenz As Double
        Dim querylavBenz = queryUmaRichLavorazioni.Where(Function(lavor) lavor.Tipo_Carburante = 3)
        If querylavBenz.Count > 0 Then
            totCarburanteRichiestoDaLavorBenz = querylavBenz.Sum(Function(lavor) lavor.Carburante_Richiesto)
        End If

        Dim totCarburanteRichiestoDaAllevBenz As Double
        Dim queryAllevBenz = queryUmaRichAllevamenti.Where(Function(allev) allev.Tipo_Carburante = 3)
        If queryAllevBenz.Count > 0 Then
            totCarburanteRichiestoDaAllevBenz = queryAllevBenz.Sum(Function(allev) allev.Carburante_Richiesto)
        End If

        Dim newCarbRichBenz = totCarburanteRichiestoDaLavorBenz + totCarburanteRichiestoDaAllevBenz
        If umaTestata.Carburante_Richiesto_Benzina <> newCarbRichBenz Then
            umaTestata.Carburante_Richiesto_Benzina = newCarbRichBenz

            flagAggiornoTestata = True
        End If


        'Totale per la colonna Carburante_Richiesto_Gasolio
        Dim totCarburanteRichiestoDaLavorGas As Double
        Dim queryLavGas = queryUmaRichLavorazioni.Where(Function(lavor) lavor.Tipo_Carburante = 2)
        If queryLavGas.Count > 0 Then
            totCarburanteRichiestoDaLavorGas = queryLavGas.Sum(Function(lavor) lavor.Carburante_Richiesto)
        End If

        Dim totCarburanteRichiestoDaAllevGas As Double
        Dim queryAllevGas = queryUmaRichAllevamenti.Where(Function(allev) allev.Tipo_Carburante = 2)
        If queryAllevGas.Count > 0 Then
            totCarburanteRichiestoDaAllevGas = queryAllevGas.Sum(Function(allev) allev.Carburante_Richiesto)
        End If

        Dim newCarbRichGas = totCarburanteRichiestoDaLavorGas + totCarburanteRichiestoDaAllevGas
        If umaTestata.Carburante_Richiesto_Gasolio <> newCarbRichGas Then
            umaTestata.Carburante_Richiesto_Gasolio = newCarbRichGas

            flagAggiornoTestata = True
        End If


        'Totale per la colonna Carburante_Richiesto_Gasolio_Serra
        Dim totCarburanteRichiestoDaLavorSer As Double
        Dim queryLavSer = queryUmaRichLavorazioni.Where(Function(lavor) lavor.Tipo_Carburante = 8)
        If queryLavSer.Count > 0 Then
            totCarburanteRichiestoDaLavorSer = queryLavSer.Sum(Function(lavor) lavor.Carburante_Richiesto)
        End If

        Dim totCarburanteRichiestoDaAllevSer As Double
        Dim queryAllevSer = queryUmaRichAllevamenti.Where(Function(allev) allev.Tipo_Carburante = 8)
        If queryAllevSer.Count > 0 Then
            totCarburanteRichiestoDaAllevSer = queryAllevSer.Sum(Function(allev) allev.Carburante_Richiesto)
        End If

        Dim newCarbRichSer = totCarburanteRichiestoDaLavorSer + totCarburanteRichiestoDaAllevSer
        If umaTestata.Carburante_Richiesto_Gasolio_Serra <> newCarbRichSer Then
            umaTestata.Carburante_Richiesto_Gasolio_Serra = newCarbRichSer

            flagAggiornoTestata = True
        End If

        If flagAggiornoTestata = True Then
            umaTestata.Data_Modifica = DateTime.Now
            umaTestata.Username_Modifica = Username_Modifica

            GiasContext.Entry(umaTestata).State = Entity.EntityState.Modified
            GiasContext.SaveChanges()
        End If

        '-----------------------------------------------------------

    End Sub

    Public Sub AggiornaTotaliCarburanteDaLavorazioniParziali(
            ByRef GiasContext As Gias_DeveloperServer_Entities,
            ByVal Piva_SuperUser As String,
            ByVal Richiesta_Cod As Integer,
            ByVal Username_Modifica As String)

        'Ottengo i valori dei carburanti dalle lavorazioni parziali della testata divisi in gruppi per tipo e richiesta_cod
        Dim queryUmaRichLavorazioni =
            From umaLav In GiasContext.UMA_Lavorazioni_Parziali
            Where umaLav.Piva_SuperUser = Piva_SuperUser And
                umaLav.Richiesta_Cod = Richiesta_Cod
            Group umaLav By
                _pivaSuperUser = umaLav.Piva_SuperUser,
                _piva = umaLav.Piva,
                _richiestaCod = umaLav.Richiesta_Cod,
                _tipoCarb = umaLav.Tipo_Carburante,
                _tipoLav = umaLav.Lavorazione_UMA
                Into g = Group
            Select New With {
                .Piva_SuperUser = _pivaSuperUser,
                .Piva = _piva,
                .Richiesta_Cod = _richiestaCod,
                .Tipo_Carburante = _tipoCarb,
                .Lavorazione_UMA = _tipoLav,
                .Carburante_Calcolato = g.Sum(Function(lav) lav.Fabbisogno_Calcolato),
                .Carburante_Richiesto = g.Sum(Function(lav) lav.Fabbisogno_Richiesto),
                .Carburante_Assegnato = g.Sum(Function(lav) lav.Fabbisogno_Assegnato)
            }


        'Prelevo il record di testata e lo aggiorno
        Dim umaTestata =
            (From umaTest In GiasContext.UMA_Richieste_Testata
             Where umaTest.Piva_SuperUser = Piva_SuperUser And 'umaTest.Piva = Piva And
                  umaTest.Richiesta_Cod = Richiesta_Cod).First()

        Dim flagAggiornoTestata As Boolean = False

        Dim totaleCarburanteCalcolatoDaLavor As Double = 0
        If queryUmaRichLavorazioni.Count > 0 Then
            totaleCarburanteCalcolatoDaLavor = queryUmaRichLavorazioni.Sum(Function(lavor) lavor.Carburante_Calcolato)
        End If

        Dim newCarbCalc = IIf(IsNothing(totaleCarburanteCalcolatoDaLavor), 0, totaleCarburanteCalcolatoDaLavor)
        If umaTestata.Carburante_Calcolato <> newCarbCalc Then
            umaTestata.Carburante_Calcolato = newCarbCalc

            flagAggiornoTestata = True
        End If

        Dim totaleCarburanteRichiestoDaLavor As Double = 0
        If queryUmaRichLavorazioni.Count > 0 Then
            totaleCarburanteRichiestoDaLavor = queryUmaRichLavorazioni.Sum(Function(lavor) lavor.Carburante_Richiesto)
        End If

        Dim newCarbRich = IIf(IsNothing(totaleCarburanteRichiestoDaLavor), 0, totaleCarburanteRichiestoDaLavor)
        If umaTestata.Carburante_Richiesto <> newCarbRich Then
            umaTestata.Carburante_Richiesto = newCarbRich

            flagAggiornoTestata = True
        End If

        Dim totaleCarburanteAssegnatoDaLavor As Double = 0

        If queryUmaRichLavorazioni.Count > 0 Then
            totaleCarburanteAssegnatoDaLavor = queryUmaRichLavorazioni.Sum(Function(lavor) lavor.Carburante_Assegnato)
        End If


        Dim newCarbAppr = IIf(IsNothing(totaleCarburanteAssegnatoDaLavor), 0, totaleCarburanteAssegnatoDaLavor)
        If umaTestata.Carburante_Approvato <> newCarbAppr Then
            umaTestata.Carburante_Approvato = newCarbAppr

            flagAggiornoTestata = True
        End If


        'Totale per la colonna Carburante_Richiesto_Benzina
        Dim totCarburanteRichiestoDaLavorBenz As Double
        Dim querylavBenz = queryUmaRichLavorazioni.Where(Function(lavor) lavor.Tipo_Carburante = 3)
        If querylavBenz.Count > 0 Then
            totCarburanteRichiestoDaLavorBenz = querylavBenz.Sum(Function(lavor) lavor.Carburante_Richiesto)
        End If

        Dim newCarbRichBenz = totCarburanteRichiestoDaLavorBenz
        If umaTestata.Carburante_Richiesto_Benzina <> newCarbRichBenz Then
            umaTestata.Carburante_Richiesto_Benzina = newCarbRichBenz

            flagAggiornoTestata = True
        End If


        'Totale per la colonna Carburante_Richiesto_Gasolio
        Dim totCarburanteRichiestoDaLavorGas As Double
        Dim queryLavGas = queryUmaRichLavorazioni.Where(Function(lavor) lavor.Tipo_Carburante = 2)
        If queryLavGas.Count > 0 Then
            totCarburanteRichiestoDaLavorGas = queryLavGas.Sum(Function(lavor) lavor.Carburante_Richiesto)
        End If

        Dim newCarbRichGas = totCarburanteRichiestoDaLavorGas
        If umaTestata.Carburante_Richiesto_Gasolio <> newCarbRichGas Then
            umaTestata.Carburante_Richiesto_Gasolio = newCarbRichGas

            flagAggiornoTestata = True
        End If


        'Totale per la colonna Carburante_Richiesto_Gasolio_Serra
        Dim totCarburanteRichiestoDaLavorSer As Double
        Dim queryLavSer = queryUmaRichLavorazioni.Where(Function(lavor) lavor.Tipo_Carburante = 8)
        If queryLavSer.Count > 0 Then
            totCarburanteRichiestoDaLavorSer = queryLavSer.Sum(Function(lavor) lavor.Carburante_Richiesto)
        End If

        Dim newCarbRichSer = totCarburanteRichiestoDaLavorSer
        If umaTestata.Carburante_Richiesto_Gasolio_Serra <> newCarbRichSer Then
            umaTestata.Carburante_Richiesto_Gasolio_Serra = newCarbRichSer

            flagAggiornoTestata = True
        End If

        If flagAggiornoTestata = True Then
            umaTestata.Data_Modifica = DateTime.Now
            umaTestata.Username_Modifica = Username_Modifica

            GiasContext.Entry(umaTestata).State = Entity.EntityState.Modified
            GiasContext.SaveChanges()
        End If

        '-----------------------------------------------------------

    End Sub


    Public Function AggiornaTotaliSuperfici(ByVal Piva As String,
                                            ByVal Richiesta_Cod As Integer,
                                            ByVal Gruppo_Colturale As String,
                                            ByVal Programmazione_Cod As Integer,
                                            ByVal Regione_Cod As String,
                                            ByVal Totale_Superficie As Decimal,
                                            ByVal Zona_Pendenza_A As Decimal,
                                            ByVal Zona_Pendenza_B As Decimal,
                                            ByVal Zona_Tessitura_Normale As Decimal,
                                            ByVal Zona_Tessitura_Media As Decimal,
                                            ByVal Zona_Tessitura_Tenace As Decimal,
                                            ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard()
        r.RispostaOK = True

        Dim transactionOptions = New TransactionOptions()
        transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)

        Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
            Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                Try
                    Dim PivaSuperUser = objParametri_Server.PivaSuperUser
                    Dim richiesta = (From t In GiasContext.UMA_Richieste
                                     Where t.Piva_SuperUser = PivaSuperUser AndAlso
                                         t.Piva = Piva AndAlso
                                         t.Gruppo_Colturale_UMA = Gruppo_Colturale AndAlso
                                         t.Programmazione_Cod = Programmazione_Cod AndAlso
                                         t.Richiesta_Cod = Richiesta_Cod).FirstOrDefault

                    If richiesta IsNot Nothing AndAlso
                       (Totale_Superficie <> richiesta.Totale_Superficie_UMA OrElse
                        Zona_Pendenza_A <> richiesta.Zona_Pendenza_A_UMA OrElse
                        Zona_Pendenza_B <> richiesta.Zona_Pendenza_B_UMA OrElse
                        Zona_Tessitura_Normale <> richiesta.Zona_Tessitura_Normale_UMA OrElse
                        Zona_Tessitura_Media <> richiesta.Zona_Tessitura_Media_UMA OrElse
                        Zona_Tessitura_Tenace <> richiesta.Zona_Tessitura_Tenace_UMA) Then

                        Dim Totale_Superficie_Corrente = richiesta.Totale_Superficie_UMA

                        richiesta.Totale_Superficie_UMA = Totale_Superficie
                        richiesta.Totale_Superficie_UMA_Edit = Totale_Superficie
                        richiesta.Zona_Pendenza_A_UMA = Zona_Pendenza_A
                        richiesta.Zona_Pendenza_A_UMA_Edit = Zona_Pendenza_A
                        richiesta.Zona_Pendenza_B_UMA = Zona_Pendenza_B
                        richiesta.Zona_Pendenza_B_UMA_Edit = Zona_Pendenza_B
                        richiesta.Zona_Tessitura_Normale_UMA = Zona_Tessitura_Normale
                        richiesta.Zona_Tessitura_Normale_UMA_Edit = Zona_Tessitura_Normale
                        richiesta.Zona_Tessitura_Media_UMA = Zona_Tessitura_Media
                        richiesta.Zona_Tessitura_Media_UMA_Edit = Zona_Tessitura_Media
                        richiesta.Zona_Tessitura_Tenace_UMA = Zona_Tessitura_Tenace
                        richiesta.Zona_Tessitura_Tenace_UMA_Edit = Zona_Tessitura_Tenace
                        richiesta.Data_Modifica = DateTime.Now
                        richiesta.Username_Modifica = objParametri_Server.UtenteUsername

                        Dim Lavorazione As UMA_Richieste_Lavorazioni
                        Dim Lavorazioni = (From t In GiasContext.UMA_Richieste_Lavorazioni
                                           Where t.Piva_SuperUser = PivaSuperUser AndAlso
                                               t.Piva = Piva AndAlso
                                               t.Gruppo_Colturale_UMA = Gruppo_Colturale AndAlso
                                               t.Programmazione_Cod = Programmazione_Cod AndAlso
                                               t.Richiesta_Cod = Richiesta_Cod)
                        Dim lavorazioneModificata As Boolean

                        Dim testata = (From t In GiasContext.UMA_Richieste_Testata Where t.Piva_SuperUser = PivaSuperUser AndAlso t.Richiesta_Cod = richiesta.Richiesta_Cod).FirstOrDefault
                        Dim pratica = (From p In GiasContext.Pratiche Where p.Piva_SuperUser = PivaSuperUser AndAlso p.Pratica_Cod = testata.Pratica_Cod).FirstOrDefault
                        Dim Anno = pratica.Anno

                        Dim objUmaSetupR As New AgronicaCoreUmaDal.UMASetup_R
                        Dim dtUmaSetup = objUmaSetupR.LeggiSetup(Anno, objParametri_Server)
                        Dim percentualeZonaPendenzaB = If(dtUmaSetup.Rows.Count > 0, Math.Round(dtUmaSetup(0).Item("Per_Mag_Terreno_B") / 100, 4), 0)
                        Dim percentualeTerrenoMedio = If(dtUmaSetup.Rows.Count > 0, Math.Round(dtUmaSetup(0).Item("Per_Mag_Terreno_Medio") / 100, 4), 0)
                        Dim percentualeTerrenoTenace = If(dtUmaSetup.Rows.Count > 0, Math.Round(dtUmaSetup(0).Item("Per_Mag_Terreno_Tenace") / 100, 4), 0)
                        Dim L_Maggiorazione_Trasferimenti = If(dtUmaSetup.Rows.Count > 0, dtUmaSetup(0).Item("Nr_Litri_Maggiorazione"), 0)

                        Dim objConfMacrousixLavorazioniR As New AgronicaCoreUmaDal.UMA_Configurazione_MacrousixLavorazioni_R
                        Dim dtConfMacrousixLavorazioni As DataTable
                        Dim costoCarburante As Double
                        Dim fabbisogno_Superficie_Trattata As Double
                        Dim fabbisogno_UMA_B_Edit As Double
                        Dim fabbisogno_TerrenoMedio As Double
                        Dim fabbisogno_TerrenoTenace As Double
                        Dim fabbisogno_Trasferimenti As Double
                        Dim fabbisognioCal As Double

                        Dim carbLavorazioniAggiornato As Boolean = False
                        Dim totCarbLavorazioniCalc As Double = 0
                        Dim totCarbLavorazioniRich As Double = 0

                        For Each Lavorazione In Lavorazioni
                            lavorazioneModificata = False

                            If Lavorazione.Totale_Superficie_UMA = Totale_Superficie_Corrente Or
                                Lavorazione.Totale_Superficie_UMA > Totale_Superficie Then

                                'Lavorazione su tutta la superficie della richiesta (e non oltre!), allineo tutti i valori
                                Lavorazione.Totale_Superficie_UMA = Totale_Superficie
                                Lavorazione.Zona_Pendenza_A_UMA = Zona_Pendenza_A
                                Lavorazione.Zona_Pendenza_B_UMA = Zona_Pendenza_B
                                Lavorazione.Zona_Tessitura_Normale_UMA = Zona_Tessitura_Normale
                                Lavorazione.Zona_Tessitura_Media_UMA = Zona_Tessitura_Media
                                Lavorazione.Zona_Tessitura_Tenace_UMA = Zona_Tessitura_Tenace

                                lavorazioneModificata = True
                            Else
                                'Lavorazione solo su parte della superficie, verifico pendenze e tessiture entro i valori della richiesta

                                If Lavorazione.Zona_Pendenza_A_UMA > Zona_Pendenza_A Or
                                    Lavorazione.Zona_Pendenza_B_UMA > Zona_Pendenza_B Then

                                    If Lavorazione.Zona_Pendenza_A_UMA > Zona_Pendenza_A Then
                                        Lavorazione.Zona_Pendenza_A_UMA = Zona_Pendenza_A
                                        Lavorazione.Zona_Pendenza_B_UMA = Lavorazione.Totale_Superficie_UMA - Zona_Pendenza_A
                                    Else
                                        Lavorazione.Zona_Pendenza_A_UMA = Lavorazione.Totale_Superficie_UMA - Zona_Pendenza_B
                                        Lavorazione.Zona_Pendenza_B_UMA = Zona_Pendenza_B
                                    End If

                                    lavorazioneModificata = True
                                End If

                                If Lavorazione.Zona_Tessitura_Normale_UMA > Zona_Tessitura_Normale Or
                                    Lavorazione.Zona_Tessitura_Media_UMA > Zona_Tessitura_Media Or
                                    Lavorazione.Zona_Tessitura_Tenace_UMA > Zona_Tessitura_Tenace Then

                                    'Allineo superfici tessiture e calcolo superficie da redistribuire
                                    Dim supResidua As Decimal = 0
                                    If Lavorazione.Zona_Tessitura_Normale_UMA > Zona_Tessitura_Normale Then
                                        supResidua += Lavorazione.Zona_Tessitura_Normale_UMA - Zona_Tessitura_Normale
                                        Lavorazione.Zona_Tessitura_Normale_UMA = Zona_Tessitura_Normale
                                    End If
                                    If Lavorazione.Zona_Tessitura_Media_UMA > Zona_Tessitura_Media Then
                                        supResidua += Lavorazione.Zona_Tessitura_Media_UMA - Zona_Tessitura_Media
                                        Lavorazione.Zona_Tessitura_Media_UMA = Zona_Tessitura_Media
                                    End If
                                    If Lavorazione.Zona_Tessitura_Tenace_UMA > Zona_Tessitura_Tenace Then
                                        supResidua += Lavorazione.Zona_Tessitura_Tenace_UMA - Zona_Tessitura_Tenace
                                        Lavorazione.Zona_Tessitura_Tenace_UMA = Zona_Tessitura_Tenace
                                    End If

                                    'Redistribuisco superficie su tessiture inferiore al limite (prima Normale, poi Media, poi Tenace)
                                    If Lavorazione.Zona_Tessitura_Normale_UMA < Zona_Tessitura_Normale And supResidua > 0 Then
                                        If (Zona_Tessitura_Normale - Lavorazione.Zona_Tessitura_Normale_UMA) < supResidua Then
                                            supResidua -= Zona_Tessitura_Normale - Lavorazione.Zona_Tessitura_Normale_UMA
                                            Lavorazione.Zona_Tessitura_Normale_UMA = Zona_Tessitura_Normale
                                        Else
                                            Lavorazione.Zona_Tessitura_Normale_UMA += supResidua
                                            supResidua = 0
                                        End If
                                    End If
                                    If Lavorazione.Zona_Tessitura_Media_UMA < Zona_Tessitura_Media And supResidua > 0 Then
                                        If (Zona_Tessitura_Media - Lavorazione.Zona_Tessitura_Media_UMA) < supResidua Then
                                            supResidua -= Zona_Tessitura_Media - Lavorazione.Zona_Tessitura_Media_UMA
                                            Lavorazione.Zona_Tessitura_Media_UMA = Zona_Tessitura_Media
                                        Else
                                            Lavorazione.Zona_Tessitura_Media_UMA += supResidua
                                            supResidua = 0
                                        End If
                                    End If
                                    If Lavorazione.Zona_Tessitura_Tenace_UMA < Zona_Tessitura_Tenace And supResidua > 0 Then
                                        If (Zona_Tessitura_Tenace - Lavorazione.Zona_Tessitura_Tenace_UMA) < supResidua Then
                                            supResidua -= Zona_Tessitura_Tenace - Lavorazione.Zona_Tessitura_Tenace_UMA
                                            Lavorazione.Zona_Tessitura_Tenace_UMA = Zona_Tessitura_Tenace
                                        Else
                                            Lavorazione.Zona_Tessitura_Tenace_UMA += supResidua
                                            supResidua = 0
                                        End If
                                    End If

                                    lavorazioneModificata = True
                                End If

                            End If

                            If lavorazioneModificata Then
                                'Verifico se sono da aggiornare anche i fabbisogni di carburante Calcolato/Richiesto
                                dtConfMacrousixLavorazioni = objConfMacrousixLavorazioniR.Leggi(Regione_Cod,
                                                                                                    Lavorazione.Gruppo_Colturale_UMA,
                                                                                                    Lavorazione.Lavorazione_UMA,
                                                                                                    Lavorazione.Lavorazione_GIAS,
                                                                                                    Lavorazione.Id_Attivita,
                                                                                                    objParametri_Server,
                                                                                                    regolamentoCod:=Lavorazione.Regolamento_Cod)
                                If dtConfMacrousixLavorazioni.Rows.Count > 0 Then

                                    Dim Udm_Alternativa = dtConfMacrousixLavorazioni(0).Item("Udm_Alternativa")
                                    If IsNothing(Udm_Alternativa) OrElse IsDBNull(Udm_Alternativa) OrElse Udm_Alternativa = "" Then

                                        costoCarburante = 0
                                        Select Case Lavorazione.Tipo_Carburante
                                            Case enum_TipoCarburante_UMA.Gasolio_Agricolo, enum_TipoCarburante_UMA.Gasolio, enum_TipoCarburante_UMA.Gasolio_Serra
                                                costoCarburante = dtConfMacrousixLavorazioni(0).Item("Gasolio_Lt")
                                            Case enum_TipoCarburante_UMA.Benzina, enum_TipoCarburante_UMA.Benzina_Agricola
                                                costoCarburante = dtConfMacrousixLavorazioni(0).Item("Benzina_Lt")
                                        End Select

                                        Dim costoCarburantexNrLavorazioni As Double = costoCarburante * Lavorazione.Nr_Lavorazioni_Richieste

                                        Dim UMA_Lavorazione = (From l In GiasContext.UMA_Lavorazioni Where l.Regione_Cod = Regione_Cod And l.Lav_UMA_Cod = Lavorazione.Lavorazione_UMA).FirstOrDefault
                                        Dim Magg_MedioTenace As Boolean = UMA_Lavorazione.Maggiorazione_Terreno_MedioTenace = 1

                                        fabbisogno_Superficie_Trattata = Lavorazione.Totale_Superficie_UMA * costoCarburantexNrLavorazioni
                                        fabbisogno_UMA_B_Edit = Lavorazione.Zona_Pendenza_B_UMA * costoCarburantexNrLavorazioni * percentualeZonaPendenzaB
                                        fabbisogno_TerrenoMedio = If(Magg_MedioTenace, Lavorazione.Zona_Tessitura_Media_UMA * costoCarburantexNrLavorazioni * percentualeTerrenoMedio, 0)
                                        fabbisogno_TerrenoTenace = If(Magg_MedioTenace, Lavorazione.Zona_Tessitura_Tenace_UMA * costoCarburantexNrLavorazioni * percentualeTerrenoTenace, 0)
                                        fabbisogno_Trasferimenti = If(Lavorazione.Superficie_Maggiorazione_Trasferimenti = 1, Lavorazione.Totale_Superficie_UMA * L_Maggiorazione_Trasferimenti, 0)

                                        fabbisognioCal = fabbisogno_Superficie_Trattata + fabbisogno_UMA_B_Edit + fabbisogno_TerrenoMedio + fabbisogno_TerrenoTenace + fabbisogno_Trasferimenti
                                        fabbisognioCal = Math.Round(fabbisognioCal, 4)

                                        If fabbisognioCal <> Lavorazione.Fabbisogno_Calcolato Then
                                            If Lavorazione.Fabbisogno_Richiesto = Lavorazione.Fabbisogno_Calcolato Then
                                                Lavorazione.Fabbisogno_Richiesto = fabbisognioCal
                                            End If
                                            Lavorazione.Fabbisogno_Calcolato = fabbisognioCal

                                            carbLavorazioniAggiornato = True
                                        End If
                                    End If
                                End If

                                'Aggiorno username e data modifica
                                Lavorazione.Data_Modifica = DateTime.Now
                                Lavorazione.Username_Modifica = objParametri_Server.UtenteUsername
                            End If

                            totCarbLavorazioniCalc = totCarbLavorazioniCalc + Lavorazione.Fabbisogno_Calcolato
                            totCarbLavorazioniRich = totCarbLavorazioniRich + Lavorazione.Fabbisogno_Richiesto

                        Next

                        If carbLavorazioniAggiornato Then
                            richiesta.Carburante_Calcolato = totCarbLavorazioniCalc
                            richiesta.Carburante_Richiesto = totCarbLavorazioniRich
                        End If

                        GiasContext.SaveChanges()

                    End If

                    scope.Complete()

                Catch ex As Exception
                    'If any exception is caught, roll back the entire transaction and ends the transaction scope
                    scope.Dispose()

                    r.RispostaOK = False
                    r.Errore = ex.Message

                Finally
                    'Close the opened connection
                    GiasContext.Database.Connection.Close()
                End Try

            End Using
        End Using

        Return r
    End Function

    ''' <summary>
    ''' Inserisce su database una pratica con il rispettivo record di testata, ed i record di richiesta e lavorazioni
    ''' </summary>
    ''' <param name="fascicolo">Dictionary con la seguente struttura dati:
    ''' { Pratica => Pratica, Testata => Uma_Richieste_Testata, Richieste => List(Of Uma_Richieste), Lavorazioni => List(Of Uma_Richieste_Lavorazioni) }
    ''' Gli oggetti devono avere le proprietà impostate eccetto la loro chiave primaria progressiva
    ''' </param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function InsertPraticaRichiesteLavorazioni(ByVal fascicolo As Dictionary(Of String, Object),
                                                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                      ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreVarieBIZ.RispostaStandard
        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard()
        r.RispostaOK = True

        Dim objPratica = CType(fascicolo("Pratica"), Pratiche)
        Dim umaTestata = CType(fascicolo("Testata"), UMA_Richieste_Testata)
        Dim listaRichieste = CType(fascicolo("Richieste"), List(Of AgronicaCoreEntityFramework_POCO.UMA_Richieste))
        Dim listaLavorazioni = CType(fascicolo("Lavorazioni"), List(Of UMA_Richieste_Lavorazioni))

        Dim handlePratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W()
        Dim handleSequenze = New AgronicaCoreDataProvider.Agro_Sequenze()

        Dim gefutils As New Gias_EF_Utility()
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing

        Using scope As New TransactionScope()
            Try
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                GiasContext.Database.Connection.Open()

                'Collego la connessione al db appena aperta all'objParametri per poterla riusare nella funzione ImpostaPratica
                objParametri_Server.objConnessione = GiasContext.Database.Connection

                'Inserisco l'entità Pratica
                Dim praticaCod As Integer = 0
                Dim rispInsertPratica = handlePratiche.impostaPratica(
                    True,
                    objPratica.Piva,
                    objPratica.Cuaa,
                    objPratica.Username_Creazione,
                    objPratica.Servizio_Cod,
                    enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione,
                    objParametri_Server,
                    objParametri_Utenti,
                    objPratica.Anno,
                    objPratica.Numero,
                    praticaCod,
                    True,
                    "",
                    objPratica.Validita_Inizio,
                    objPratica.Validita_Fine,
                    0,
                    0
                )
                If rispInsertPratica.RispostaOK = False Then
                    Throw New Exception("Inserimento pratica non riuscito, " & rispInsertPratica.Errore)
                End If

                objPratica.Pratica_Cod = praticaCod

                'Inserisco il record della testata, prelevando la pk della pratica ed un nuovo id progressivo
                Dim richiestaTestataCod As Integer
                Try
                    umaTestata.Pratica_Cod = praticaCod
                    richiestaTestataCod = handleSequenze.NuovoId_Tabella_EF(GiasContext,
                                        "UMA_Richieste_Testata", 0, 2000000000, objParametri_Server)
                    umaTestata.Richiesta_Cod = richiestaTestataCod
                    GiasContext.UMA_Richieste_Testata.Add(umaTestata)
                    GiasContext.SaveChanges()
                Catch ex As Exception
                    Throw New Exception("Inserimento testata non riuscito, " & ex.Message)
                End Try

                'Inserisco i record delle richieste
                Dim umaRichiestaLog As New AgronicaCoreEntityFramework_POCO.UMA_Richieste
                Try
                    For Each umaRichiesta In listaRichieste
                        umaRichiestaLog = umaRichiesta

                        umaRichiesta.Richiesta_Cod = richiestaTestataCod
                        GiasContext.UMA_Richieste.Add(umaRichiesta)
                        GiasContext.SaveChanges()
                    Next
                Catch ex As Exception
                    Throw New Exception(String.Format("Inserimento richiesta gruppo colturale {0} non riuscito, {1}", umaRichiestaLog.Gruppo_Colturale_UMA, ex.Message))
                End Try

                'Inserisco i record delle lavorazioni
                Dim umaLavorazioneLog As New AgronicaCoreEntityFramework_POCO.UMA_Richieste_Lavorazioni
                Try
                    For Each umaLavorazione In listaLavorazioni
                        umaLavorazioneLog = umaLavorazione

                        umaLavorazione.Richiesta_Cod = richiestaTestataCod
                        umaLavorazione.Richiesta_Dettaglio_Cod = handleSequenze.NuovoId_Tabella_EF(GiasContext,
                                                "UMA_Richieste_Lavorazioni", 0, 2000000000, objParametri_Server)
                        GiasContext.UMA_Richieste_Lavorazioni.Add(umaLavorazione)
                        GiasContext.SaveChanges()
                    Next
                Catch ex As Exception
                    Throw New Exception(String.Format("Inserimento lavorazione codice uma {0} non riuscito, {1}", umaLavorazioneLog.Lavorazione_UMA, ex.Message))
                End Try

                'If execution reaches here, it indicates the successfull completion of all the save operation. hence commit the transaction.
                scope.Complete()

            Catch ex As Exception
                'If any exception is caught, roll back the entire transaction and ends the transaction scope
                scope.Dispose()

                r.RispostaOK = False
                r.Errore = ex.Message

            Finally
                'Close the opened connection
                If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                    GiasContext.Database.Connection.Close()
                End If
            End Try

        End Using

        Return r
    End Function

    ''' <summary>
    ''' Restituisce la lista dei totali dei carburanti utilizzati per tutte le lavorazioni e gli allevamenti di una testata divisa per la tipologia di carburante.
    ''' </summary>
    ''' <param name="codiceRichiesta"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Public Function LeggiTotaliCarburantePerTipo(ByVal codiceRichiesta As Integer,
                                                 ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                     Optional isLavorazioneParzialeRichiesta As Boolean = False
                                                 ) As List(Of CarburanteHelper)
        Dim dtLavorazioni As New DataTable

        Dim handleLavorazioni As New UMA_Richieste_Lavorazioni_R()
        If isLavorazioneParzialeRichiesta Then
            dtLavorazioni = handleLavorazioni.Leggi_Lavorazioni_Parziali("", codiceRichiesta, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)
        Else
            dtLavorazioni = handleLavorazioni.Leggi("", "", 0, codiceRichiesta, 0,
                                                    enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)
        End If

        Dim queryTipoCarbLavorazioni =
            From richLav In dtLavorazioni.AsEnumerable()
            Group richLav By
                tipoCarb = richLav.Field(Of String)("Tipo_Carburante")
                Into g = Group
            Select New CarburanteHelper With {
                .Tipo = tipoCarb,
                .Calcolato = g.Sum(Function(richLav) If(richLav("Fabbisogno_Calcolato").Equals(DBNull.Value), 0, richLav.Field(Of Double)("Fabbisogno_Calcolato"))),
                .Richiesto = g.Sum(Function(richLav) If(richLav("Fabbisogno_Richiesto").Equals(DBNull.Value), 0, richLav.Field(Of Double)("Fabbisogno_Richiesto"))),
                .Assegnato = g.Sum(Function(richLav) If(richLav("Fabbisogno_Assegnato").Equals(DBNull.Value), 0, richLav.Field(Of Double)("Fabbisogno_Assegnato")))
            }

        Dim handleAllev As New UMA_Richieste_Allevamenti_R()
        Dim dtAllev = handleAllev.Leggi("", "", codiceRichiesta, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri_Server)
        Dim queryTipoCarbAllev =
            From richAllev In dtAllev.AsEnumerable()
            Group richAllev By
                tipoCarb = richAllev.Field(Of Integer)("Tipo_Carburante")
                Into g = Group
            Select New CarburanteHelper With {
                .Tipo = tipoCarb,
                .Calcolato = g.Sum(Function(richLav) If(richLav("Carburante_Calcolato").Equals(DBNull.Value), 0, richLav.Field(Of Double)("Carburante_Calcolato"))),
                .Richiesto = g.Sum(Function(richLav) If(richLav("Carburante_Richiesto").Equals(DBNull.Value), 0, richLav.Field(Of Double)("Carburante_Richiesto"))),
                .Assegnato = g.Sum(Function(richLav) If(richLav("Carburante_Approvato").Equals(DBNull.Value), 0, richLav.Field(Of Double)("Carburante_Approvato")))
            }

        Dim listaTipoCarbLavorazioni = queryTipoCarbLavorazioni.ToList()
        Dim listaTipoCarbAllev = queryTipoCarbAllev.ToList()

        listaTipoCarbLavorazioni.AddRange(listaTipoCarbAllev)

        Dim query =
            From el In listaTipoCarbLavorazioni
            Group el By
                tipoCarb = el.Tipo
                Into g = Group
            Select New CarburanteHelper With {
                .Tipo = tipoCarb,
                .Calcolato = g.Sum(Function(x) x.Calcolato),
                .Richiesto = g.Sum(Function(x) x.Richiesto),
                .Assegnato = g.Sum(Function(x) x.Assegnato)
            }


        Return query.ToList()
    End Function


    Public Function Approva_Automaticamente_Richiesta(ByRef EFContext As Gias_DeveloperServer_Entities,
                                                      ByVal Richiesta_Cod As Integer,
                                                      ByVal Note As String,
                                                      ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim Esito As Boolean = False

        Dim objUMA_Richieste As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W

        Dim DTUma = objUMA_Richieste.Leggi_Elenco("", " t.Richiesta_Cod = " & Richiesta_Cod & " ", "", objParametri_Server, rendicontazioni:=False)

        If DTUma.Rows.Count = 0 Then
            DTUma = objUMA_Richieste.Leggi_Elenco("", " t.Richiesta_Cod = " & Richiesta_Cod & " ", "", objParametri_Server, rendicontazioni:=True)
        End If

        If DTUma.Rows.Count > 0 Then
            If DTUma.Rows(0)("Stato_Cod") = enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione Then

                Esito = objPratiche.Esegui_PassaggioDiStato_SuperUser_SenzaVerifiche(EFContext, "",
                                                                             DTUma.Rows(0)("Pratica_Cod"),
                                                                             enum_Servizi.Gestione_UMA,
                                                                             2005,
                                                                             Note,
                                                                             objParametri_Server)

            End If
        End If

        Return Esito
    End Function

    Public Function InsertPraticheApprovate(ByVal listaPratiche As List(Of Pratiche), ByVal listaTestate As List(Of UMA_Richieste_Testata),
                                         ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard()
        r.RispostaOK = True

        Dim handlePratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_W()
        Dim handleSequenze = New AgronicaCoreDataProvider.Agro_Sequenze()

        Dim gefutils As New Gias_EF_Utility()
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim GiasContext As Gias_DeveloperServer_Entities = Nothing

        Using scope As New TransactionScope()
            Try
                GiasContext = New Gias_DeveloperServer_Entities(EFConnString)
                GiasContext.Database.Connection.Open()

                'Collego la connessione al db appena aperta all'objParametri per poterla riusare nella funzione ImpostaPratica
                objParametri_Server.objConnessione = GiasContext.Database.Connection

                For i = 0 To listaPratiche.Count - 1
                    Dim objPratica = listaPratiche(i)
                    Dim umaTestata = listaTestate(i)

                    'Inserisco l'entità Pratica
                    Dim praticaCod As Integer = 0
                    Dim rispInsertPratica = handlePratiche.impostaPratica(
                        True,
                        objPratica.Piva,
                        objPratica.Cuaa,
                        objPratica.Username_Creazione,
                        objPratica.Servizio_Cod,
                        enum_WWorflow_WAnagraficaStati.Quaderno_Campagna_In_Compilazione,
                        objParametri_Server,
                        objParametri_Utenti,
                        objPratica.Anno,
                        objPratica.Numero,
                        praticaCod,
                        True,
                        "",
                        objPratica.Validita_Inizio,
                        objPratica.Validita_Fine,
                        0,
                        0
                    )
                    If rispInsertPratica.RispostaOK = False Then
                        Throw New Exception("Inserimento pratica non riuscito, " & rispInsertPratica.Errore)
                    End If

                    objPratica.Pratica_Cod = praticaCod

                    'Inserisco il record della testata, prelevando la pk della pratica ed un nuovo id progressivo
                    Dim richiestaTestataCod As Integer
                    Try
                        umaTestata.Pratica_Cod = praticaCod
                        richiestaTestataCod = handleSequenze.NuovoId_Tabella_EF(GiasContext,
                                            "UMA_Richieste_Testata", 0, 2000000000, objParametri_Server)
                        umaTestata.Richiesta_Cod = richiestaTestataCod
                        GiasContext.UMA_Richieste_Testata.Add(umaTestata)
                        GiasContext.SaveChanges()
                    Catch ex As Exception
                        Throw New Exception("Inserimento testata non riuscito, " & ex.Message)
                    End Try

                    If Not Approva_Automaticamente_Richiesta(GiasContext, richiestaTestataCod, "", objParametri_Server) Then
                        Throw New Exception("Approvazione richiesta non riuscita")
                    End If

                Next

                'If execution reaches here, it indicates the successfull completion of all the save operation. hence commit the transaction.
                scope.Complete()

            Catch ex As Exception
                'If any exception is caught, roll back the entire transaction and ends the transaction scope
                scope.Dispose()

                r.RispostaOK = False
                r.Errore = ex.Message

            Finally
                'Close the opened connection
                If GiasContext IsNot Nothing AndAlso GiasContext.Database.Connection.State = ConnectionState.Open Then
                    GiasContext.Database.Connection.Close()
                End If
            End Try
        End Using

        Return r
    End Function


    ''' <summary>
    ''' inserimento modifica e cancellazione della tabella UMA_Lavorazioni_parziali.
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="objParametri_Server"></param>
    ''' <returns></returns>
    Public Function Aggiorna_Lavorazioni_Parziali(ByVal Piva As String,
                                                  ByVal Richiesta_Cod As Integer,
                                                  ByVal IsTerzista As Boolean,
                                                  ByVal righeInserite As String,
                                                  ByVal righeModificate As String,
                                                  ByVal righeCancellate As String,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As (Integer, String)

        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Dim MessaggioErrore As String = String.Empty
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.Aggiorna_Lavorazioni_Parziali()"
        Dim log As New AgronicaCoreDataProvider.LogProvider
        Dim bOk As Boolean = False
        Dim Jinsert As New JArray
        Dim Jupdate As JArray
        Dim Jdelete As JArray
        Dim lavorazioniR = New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_R
        Dim lavorazioniW = New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_W
        Dim r As AgronicaCoreEntityFramework_POCO.UMA_Lavorazioni_Parziali

        'Dim macro = New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
        ''Recupero tutti i macrousi UMA (che non ottengo dal record inserito)
        'Dim DtMacro As DataTable = macro.Macrousi(objParametri)
        'Dim dicMacro As New Dictionary(Of Integer, String)
        'Dim dicRichieste As New Dictionary(Of String, Tuple(Of Integer, Integer, Integer))
        'Dim appTupla As Tuple(Of Integer, Integer, Integer)

        Dim a = New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim EFArrayToInsert As New ArrayList
        Dim EFArrayToUpdate As New ArrayList
        Dim EFArrayToDelete As New ArrayList
        Dim EFArrayUpdateRichieste As New ArrayList
        Dim EFArrayUpdateTestate As New ArrayList
        Dim rigaLavorazione As DataRow
        Dim Risultato As Integer = 0
        Dim Errs As String = ""
        Dim TopCode As Integer
        Dim BaseCode As Integer
        AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, 1)
        Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
        Dim lavorazioniParzCodArray As New ArrayList
        Dim inOK As Boolean = False 'per controllare se ci sono stati dei nuovi record inseriti

        ''inserisco i macrousi in un dizionario per poter avere un riferimento immediato codice-macrouso
        'For Each row In DtMacro.Rows
        '    dicMacro.Add(row.Item("Macrouso_UMA_Cod"), row.Item("Macrouso_UMA_Des"))
        'Next

        'preparo un array di cod_dettaglio freschi per i record nuovi
        'non è possibile crearli sul momento per via della transazione perchè crea errori

        If righeInserite <> "" And righeInserite <> "[]" Then
            Jinsert = JArray.Parse(righeInserite)
            inOK = True
            For Each record In Jinsert

                lavorazioniParzCodArray.Add(objSequenze.NuovoId_Tabella("UMA_Lavorazioni_Parziali", BaseCode, TopCode, objParametri))

            Next
        End If

        Try

            Dim transactionOptions = New TransactionOptions()
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri.StringaConnessione)

            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    '###########################################################################################################################################
                    '################################### NUOVI INSERIMENTI/MODIFICHE ###########################################################################
                    '###########################################################################################################################################

                    If inOK Then

                        Dim index As Integer = 0

                        'creo i record da inserire
                        For Each objRiga As JObject In Jinsert

                            Dim Validita_Inizio = AGRODATAINIZIO
                            If Not IsDBNull(objRiga.Item("Validita_Inizio")) AndAlso Not objRiga.Item("Validita_Inizio") Is Nothing Then
                                Validita_Inizio = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objRiga.Item("Validita_Inizio")), a)
                            End If
                            Dim Validita_Fine = AGRODATAFINE
                            If Not IsDBNull(objRiga.Item("Validita_Fine")) AndAlso Not objRiga.Item("Validita_Fine") Is Nothing Then
                                Validita_Fine = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objRiga.Item("Validita_Fine")), a)
                            End If

                            ''imposto ltrichiesto a 0 se è inserita la CUAA del terzista
                            Dim ltrichiesto As Double
                            'Dim cuaa As String

                            'If objRiga.Item("CUAA") <> "" AndAlso Not IsDBNull(objRiga.Item("CUAA")) Then
                            '    cuaa = objRiga.Item("CUAA")
                            '    ltrichiesto = CDbl(0)
                            'Else
                            ltrichiesto = CDbl(objRiga.Item("ltrichiesto"))
                            '    cuaa = Nothing
                            'End If

                            r = New UMA_Lavorazioni_Parziali With {
                                    .Piva_SuperUser = objParametri.PivaSuperUser,
                                    .Piva = Piva,
                                    .Lavorazione_UMA = CStr(objRiga.Item("Lav_UMA_Cod")),
                                    .Lavorazione_GIAS = CStr(objRiga.Item("LAV_COD")),
                                    .Richiesta_Cod = CInt(objRiga.Item("Richiesta_Cod")),
                                    .Lavorazione_Parziale_Cod = lavorazioniParzCodArray(index),
                                    .Tipo_Carburante = CStr(objRiga.Item("Car_Cod")),
                                    .Superficie_Maggiorazione_Trasferimenti = CInt(objRiga("SupMaggiorazioneTrasferimenti")),
                                    .Nr_Lavorazioni_Previste = CInt(objRiga.Item("nLavPreviste")),
                                    .Nr_Lavorazioni_Richieste = CInt(objRiga.Item("nLavRichieste")),
                                    .Piu_Lavorazioni_Previste = CInt(objRiga.Item("nLavPreviste")),
                                    .Piu_Raccolti_Previsti = CInt(objRiga.Item("piuLavPreviste")),
                                    .Fabbisogno_Assegnato = CDbl(objRiga.Item("ltAssegnato")),
                                    .Fabbisogno_Calcolato = CDbl(objRiga.Item("fabbisognoCalc")),
                                    .Fabbisogno_Richiesto = ltrichiesto,
                                    .Validita_Inizio = Validita_Inizio,
                                    .Validita_Fine = Validita_Fine,
                                    .Data_Creazione = Date.Now,
                                    .Data_Modifica = Date.Now,
                                    .Username_Creazione = objParametri.UtenteUsername,
                                    .Username_Modifica = objParametri.UtenteUsername,
                                    .Totale_Superficie_UMA = CDbl(objRiga.Item("Superficie_Trattata")),
                                    .Zona_Pendenza_A_UMA = CDbl(objRiga.Item("Sup_A")),
                                    .Zona_Pendenza_B_UMA = CDbl(objRiga.Item("Sup_B")),
                                    .Zona_Tessitura_Normale_UMA = CDbl(objRiga.Item("TerrenoNormale")),
                                    .Zona_Tessitura_Media_UMA = CDbl(objRiga.Item("TerrenoMedio")),
                                    .Zona_Tessitura_Tenace_UMA = CDbl(objRiga.Item("TerrenoTenace")),
                                    .Regolamento_Cod = CInt(objRiga.Item("Regolamento_Cod"))
                                }

                            ''il dictionary delle richieste tiene traccia per ogni macrouso la somma dei litri aggiunti con le nuove lavorazioni
                            ''così da poter aggiornare le richieste con i valori corretti
                            'If (dicRichieste.ContainsKey(r.Gruppo_Colturale_UMA)) Then
                            '    appTupla = dicRichieste(r.Gruppo_Colturale_UMA)
                            '    dicRichieste.Remove(r.Gruppo_Colturale_UMA)
                            '    dicRichieste.Add(r.Gruppo_Colturale_UMA, Tuple.Create(r.Fabbisogno_Calcolato + appTupla.Item1,
                            '                                                          r.Fabbisogno_Richiesto + appTupla.Item2,
                            '                                                          r.Fabbisogno_Assegnato + appTupla.Item3))
                            'Else
                            '    dicRichieste.Add(r.Gruppo_Colturale_UMA, Tuple.Create(r.Fabbisogno_Calcolato, r.Fabbisogno_Richiesto, r.Fabbisogno_Assegnato))
                            'End If

                            If ControlloSovrapposizioniPerLavParz(r, IsTerzista, Errs, objParametri) Then
                                EFArrayToInsert.Add(r)
                                index += 1
                            End If

                        Next

                    End If

                    If righeModificate <> "" And righeModificate <> "[]" Then
                        Jupdate = JArray.Parse(righeModificate)

                        For Each objRiga As JObject In Jupdate

                            'per le righe modificate prendo il record originale per reperire le informazioni non codifcate nella tabella (quali gli username creazione, data creazione ecc.)
                            rigaLavorazione = lavorazioniR.Leggi_Lavorazioni_Parziali(Piva, CInt(objRiga.Item("Richiesta_Cod")), objRiga.Item("Lavorazione_Parziale_Cod"), enumSelezioneVariabile.Selezione_TabellaCompleta, objParametri).Rows.Item(0)

                            Dim Validita_Inizio = AGRODATAINIZIO
                            If Not IsDBNull(objRiga.Item("Validita_Inizio")) AndAlso Not objRiga.Item("Validita_Inizio") Is Nothing Then
                                Validita_Inizio = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objRiga.Item("Validita_Inizio")), a)
                            End If
                            Dim Validita_Fine = AGRODATAFINE
                            If Not IsDBNull(objRiga.Item("Validita_Fine")) AndAlso Not objRiga.Item("Validita_Fine") Is Nothing Then
                                Validita_Fine = JsonConvert.DeserializeObject(Of Date)(JsonConvert.SerializeObject(objRiga.Item("Validita_Fine")), a)
                            End If

                            'imposto ltrichiesto a 0 se è inserita la CUAA del terzista
                            Dim ltrichiesto As Double

                            ltrichiesto = CDbl(objRiga.Item("ltrichiesto"))

                            'If (CStr(rigaLavorazione.Item("Lavorazione_GIAS")) <> CStr(objRiga.Item("LAV_COD"))) Then

                            '    Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                            '        r = (From l In GiasContext.UMA_Lavorazioni_Parziali Where l.Piva = CStr(objRiga.Item("Piva")) And
                            '                                                                       l.Richiesta_Cod = Richiesta_Cod And
                            '                                                                       l.Lavorazione_Parziale_Cod = LavorazioneParziale_Del).FirstOrDefault
                            '    End Using

                            'End If

                            r = New UMA_Lavorazioni_Parziali With {
                                .Piva_SuperUser = objParametri.PivaSuperUser,
                                .Piva = Piva,
                                .Lavorazione_UMA = CStr(objRiga.Item("Lav_UMA_Cod")),
                                .Lavorazione_GIAS = CStr(objRiga.Item("LAV_COD")),
                                .Richiesta_Cod = CInt(objRiga.Item("Richiesta_Cod")),
                                .Lavorazione_Parziale_Cod = CInt(objRiga.Item("Lavorazione_Parziale_Cod")),
                                .Tipo_Carburante = CStr(objRiga.Item("Car_Cod")),
                                .Superficie_Maggiorazione_Trasferimenti = CInt(objRiga("SupMaggiorazioneTrasferimenti")),
                                .Nr_Lavorazioni_Previste = CInt(objRiga.Item("nLavPreviste")),
                                .Nr_Lavorazioni_Richieste = CInt(objRiga.Item("nLavRichieste")),
                                .Piu_Lavorazioni_Previste = CInt(objRiga.Item("nLavPreviste")),
                                .Piu_Raccolti_Previsti = CInt(objRiga.Item("piuLavPreviste")),
                                .Fabbisogno_Assegnato = CDbl(objRiga.Item("ltAssegnato")),
                                .Fabbisogno_Calcolato = CDbl(objRiga.Item("fabbisognoCalc")),
                                .Fabbisogno_Richiesto = ltrichiesto,
                                .Validita_Inizio = Validita_Inizio,
                                .Validita_Fine = Validita_Fine,
                                .Data_Creazione = Date.Now,
                                .Data_Modifica = Date.Now,
                                .Username_Creazione = objParametri.UtenteUsername,
                                .Username_Modifica = objParametri.UtenteUsername,
                                .Totale_Superficie_UMA = CDbl(objRiga.Item("Superficie_Trattata")),
                                .Zona_Pendenza_A_UMA = CDbl(objRiga.Item("Sup_A")),
                                .Zona_Pendenza_B_UMA = CDbl(objRiga.Item("Sup_B")),
                                .Zona_Tessitura_Normale_UMA = CDbl(objRiga.Item("TerrenoNormale")),
                                .Zona_Tessitura_Media_UMA = CDbl(objRiga.Item("TerrenoMedio")),
                                .Zona_Tessitura_Tenace_UMA = CDbl(objRiga.Item("TerrenoTenace")),
                                .Regolamento_Cod = CInt(objRiga.Item("Regolamento_Cod"))
                                }

                            ''qui vengono considerati solo i litri aggiunti/rimossi con le varie modifiche
                            ''alla fine nel dictionary saranno presenti i litri da AGGIUNGERE alle varie richieste
                            'If (dicRichieste.ContainsKey(r.Gruppo_Colturale_UMA)) Then
                            '    appTupla = dicRichieste(r.Gruppo_Colturale_UMA)
                            '    dicRichieste.Remove(r.Gruppo_Colturale_UMA)
                            '    dicRichieste.Add(r.Gruppo_Colturale_UMA, Tuple.Create(r.Fabbisogno_Calcolato - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Calcolato")) + appTupla.Item1,
                            '                                                          r.Fabbisogno_Richiesto - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Richiesto")) + appTupla.Item2,
                            '                                                          r.Fabbisogno_Assegnato - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Assegnato")) + appTupla.Item3))
                            'Else
                            '    dicRichieste.Add(r.Gruppo_Colturale_UMA, Tuple.Create(r.Fabbisogno_Calcolato - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Calcolato")),
                            '                                                          r.Fabbisogno_Richiesto - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Richiesto")),
                            '                                                          r.Fabbisogno_Assegnato - Integer.Parse(rigaLavorazione.Item("Fabbisogno_Assegnato"))))
                            'End If

                            If ControlloSovrapposizioniPerLavParz(r, IsTerzista, Errs, objParametri) Then

                                If (CStr(rigaLavorazione.Item("Lavorazione_GIAS")) <> CStr(objRiga.Item("LAV_COD"))) Then
                                    EFArrayToInsert.Add(r)
                                    Dim rr = Gias_EF_Utility.CopyEntity(GiasContext, r, Nothing, "username", AGRODATAINIZIO)

                                    rr.Lavorazione_GIAS = CStr(rigaLavorazione.Item("Lavorazione_GIAS"))

                                    EFArrayToDelete.Add(rr)
                                Else

                                    EFArrayToUpdate.Add(r)
                                End If

                            End If

                        Next

                    End If

                    'Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                    '    Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
                    '###########################################################################################################################################
                    '################################### NUOVI INSERIMENTI/MODIFICHE ###########################################################################
                    '###########################################################################################################################################

                    If righeCancellate <> "" And righeCancellate <> "[]" Then
                        Jdelete = JArray.Parse(righeCancellate)

                        For Each objRiga As JObject In Jdelete

                            Dim Piva_del = CStr(objRiga.Item("Piva"))

                            Richiesta_Cod = CInt(objRiga.Item("Richiesta_Cod"))

                            Dim LavorazioneParziale_Del = CInt(objRiga.Item("Lavorazione_Parziale_Cod"))

                            If LavorazioneParziale_Del <> 0 Then
                                r = (From l In GiasContext.UMA_Lavorazioni_Parziali Where l.Piva = Piva_del And
                                                                                           l.Richiesta_Cod = Richiesta_Cod And
                                                                                           l.Lavorazione_Parziale_Cod = LavorazioneParziale_Del).FirstOrDefault


                                EFArrayToDelete.Add(r)
                            End If

                        Next

                    End If

                    'Da scommentare e ricontrollare quando si potrà aggiornare entity framework
                    'EFArrayUpdateTestate = CreaRecordUpdateTestate(Piva, Richiesta_Cod, dicRichieste, objParametri)

                    'Creo tutti i record di UMA_Richieste aggiornati
                    'EFArrayUpdateRichieste = CreaRecordUpdateRichieste(Piva, Richiesta_Cod, dicRichieste, objParametri)

                    Risultato = lavorazioniW.Aggiorna_Lavorazioni_Parziali(EFArrayToInsert, EFArrayToUpdate, EFArrayToDelete, objParametri)

                    'Aggiorno la tabella delle testate con i nuovi totali di carburante
                    AggiornaTotaliCarburanteDaLavorazioniParziali(GiasContext, objParametri.PivaSuperUser, Richiesta_Cod, objParametri.UtenteUsername)

                    'ControlloAnticipo(Richiesta_Cod, objParametri, True, MessaggioErrore)

                    If (String.IsNullOrEmpty(MessaggioErrore)) Then
                        ' COMMIT Effettivo
                        scope.Complete()
                    Else
                        ' Rollback
                        scope.Dispose()
                    End If

                End Using

            End Using

        Catch ex As Exception

            MessaggioErrore = "[" & NomeRoutine & "] : " & ex.Message
            log.Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception(MessaggioErrore)

        End Try

        If Not String.IsNullOrEmpty(MessaggioErrore) Then
            Throw New Exception(MessaggioErrore)
        End If

        Return (Risultato, Errs)

    End Function

    Public Sub GenerazioneAutomaticaMovimentiRimanenze(ByVal Pratica_Cod As Integer, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim Trasferimenti As New AgronicaCoreUmaDal.UMA_Richieste_Trasferimenti_R
        Dim Restituzioni As New AgronicaCoreUmaDal.UMA_Richieste_Restituzioni_R
        Dim messaggioErrore As String
        Dim DtTrasferimenti As DataTable
        Dim DtRestituzioni As DataTable
        Dim Richiesta_Cod As Integer = 0
        Dim Piva As String = 0
        Dim listaVendite As New List(Of UMA_VenditeToRow)
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.GenerazioneAutomaticaMovimentiRimanenze()"

        Try
            Dim DtTestata As DataTable = richieste.Leggi_CodicePratica(Pratica_Cod, objParametri)

            If (DtTestata.Rows.Count > 0) Then

                Dim RigaT As DataRow = DtTestata.Rows(0)
                Richiesta_Cod = RigaT("Richiesta_Cod")
                Dim Numero As String = RigaT("Numero")
                Dim ValInizioData As Date = DtTestata.Rows(0)("Validita_Inizio")
                Dim Anno = Year(ValInizioData)
                Dim Avanzamento As Integer = DtTestata.Rows(0)("Avanzamento_Richiesta")
                Dim TipoRichiesta As Integer = DtTestata.Rows(0)("Tipo_Richiesta")
                Piva = DtTestata.Rows(0)("Piva")
                DtTrasferimenti = Trasferimenti.Leggi(Piva, Richiesta_Cod, objParametri)
                DtRestituzioni = Restituzioni.Leggi(Piva, Richiesta_Cod, objParametri)

                'Lettura setup
                Dim setup As New UMASetup_R
                Dim gestioneRimanenze = 0
                Dim dtRiduzione = setup.LeggiSetup(Anno, objParametri)
                If dtRiduzione.Rows.Count > 0 Then
                    gestioneRimanenze = dtRiduzione.Rows(0).Item("Gestione_Rimanenze")
                End If

                'Se gestione rimanenze abilitata
                If gestioneRimanenze = 1 OrElse (gestioneRimanenze = 2 AndAlso Avanzamento = 1) Then

                    '--------------------------------------------------------------------------------
                    'Scrittura trasferimenti
                    '--------------------------------------------------------------------------------
                    For Each ROT As DataRow In DtTrasferimenti.Rows

                        'enum_TipoCarburante_UMA.Gasolio
                        If ROT("Confermato_Gasolio") > 0 Then
                            listaVendite.Add(CaricaUmaVendite(ROT("Data_Trasferimento"),
                                                              ROT("Confermato_Tipo_Rich"),
                                                              Richiesta_Cod,
                                                              ROT("Piva"),
                                                              ROT("Piva_Ricevente"),
                                                              enum_TipoCarburante_UMA.Gasolio,
                                                              ROT("Confermato_Gasolio"),
                                                              DescrCaricoTrasferimento,
                                                              OrigineMovTrasferimento,
                                                              Numero,
                                                              Anno,
                                                              Avanzamento))
                            listaVendite.Add(CaricaUmaVendite(ROT("Data_Trasferimento"),
                                                              TipoRichiesta,
                                                              Richiesta_Cod,
                                                              ROT("Piva"),
                                                              ROT("Piva"),
                                                              enum_TipoCarburante_UMA.Gasolio,
                                                              -1 * ROT("Confermato_Gasolio"),
                                                              DescrScaricoTrasferimento,
                                                              OrigineMovTrasferimento,
                                                              Numero,
                                                              Anno,
                                                              Avanzamento))
                        End If

                        'enum_TipoCarburante_UMA.Benzina
                        If ROT("Confermato_Benzina") > 0 Then
                            listaVendite.Add(CaricaUmaVendite(ROT("Data_Trasferimento"),
                                                              ROT("Confermato_Tipo_Rich"),
                                                              Richiesta_Cod,
                                                              ROT("Piva"),
                                                              ROT("Piva_Ricevente"),
                                                              enum_TipoCarburante_UMA.Benzina,
                                                              ROT("Confermato_Benzina"),
                                                              DescrCaricoTrasferimento,
                                                              OrigineMovTrasferimento,
                                                              Numero,
                                                              Anno,
                                                              Avanzamento))
                            listaVendite.Add(CaricaUmaVendite(ROT("Data_Trasferimento"),
                                                              TipoRichiesta,
                                                              Richiesta_Cod,
                                                              ROT("Piva"),
                                                              ROT("Piva"),
                                                              enum_TipoCarburante_UMA.Benzina,
                                                              -1 * ROT("Confermato_Benzina"),
                                                              DescrScaricoTrasferimento,
                                                              OrigineMovTrasferimento,
                                                              Numero,
                                                              Anno,
                                                              Avanzamento))
                        End If

                        'enum_TipoCarburante_UMA.Gasolio_Serra
                        If ROT("Confermato_Gasolio_Serra") > 0 Then
                            listaVendite.Add(CaricaUmaVendite(ROT("Data_Trasferimento"),
                                                              ROT("Confermato_Tipo_Rich"),
                                                              Richiesta_Cod,
                                                              ROT("Piva"),
                                                              ROT("Piva_Ricevente"),
                                                              enum_TipoCarburante_UMA.Gasolio_Serra,
                                                              ROT("Confermato_Gasolio_Serra"),
                                                              DescrCaricoTrasferimento,
                                                              OrigineMovTrasferimento,
                                                              Numero,
                                                              Anno,
                                                              Avanzamento))
                            listaVendite.Add(CaricaUmaVendite(ROT("Data_Trasferimento"),
                                                              TipoRichiesta,
                                                              Richiesta_Cod,
                                                              ROT("Piva"),
                                                              ROT("Piva"),
                                                              enum_TipoCarburante_UMA.Gasolio_Serra,
                                                              -1 * ROT("Confermato_Gasolio_Serra"),
                                                              DescrScaricoTrasferimento,
                                                              OrigineMovTrasferimento,
                                                              Numero,
                                                              Anno,
                                                              Avanzamento))
                        End If

                    Next

                    '--------------------------------------------------------------------------------
                    'Scrittura restituzioni
                    '--------------------------------------------------------------------------------
                    For Each ROT As DataRow In DtRestituzioni.Rows

                        'enum_TipoCarburante_UMA.Gasolio
                        If ROT("Confermato_Gasolio") > 0 Then
                            listaVendite.Add(CaricaUmaVendite(ROT("Data_Trasferimento"),
                                                              TipoRichiesta,
                                                              Richiesta_Cod,
                                                              ROT("Piva"),
                                                              ROT("Piva"),
                                                              enum_TipoCarburante_UMA.Gasolio,
                                                              -1 * ROT("Confermato_Gasolio"),
                                                              DescrScaricoRestituzione,
                                                              OrigineMovRestituzione,
                                                              Numero,
                                                              Anno,
                                                              Avanzamento))
                        End If

                        'enum_TipoCarburante_UMA.Benzina
                        If ROT("Confermato_Benzina") > 0 Then
                            listaVendite.Add(CaricaUmaVendite(ROT("Data_Trasferimento"),
                                                              TipoRichiesta,
                                                              Richiesta_Cod,
                                                              ROT("Piva"),
                                                              ROT("Piva"),
                                                              enum_TipoCarburante_UMA.Benzina,
                                                              -1 * ROT("Confermato_Benzina"),
                                                              DescrScaricoRestituzione,
                                                              OrigineMovRestituzione,
                                                              Numero,
                                                              Anno,
                                                              Avanzamento))
                        End If

                        'enum_TipoCarburante_UMA.Gasolio_Serra
                        If ROT("Confermato_Gasolio_Serra") > 0 Then
                            listaVendite.Add(CaricaUmaVendite(ROT("Data_Trasferimento"),
                                                              TipoRichiesta,
                                                              Richiesta_Cod,
                                                              ROT("Piva"),
                                                              ROT("Piva"),
                                                              enum_TipoCarburante_UMA.Gasolio_Serra,
                                                              -1 * ROT("Confermato_Gasolio_Serra"),
                                                              DescrScaricoRestituzione,
                                                              OrigineMovRestituzione,
                                                              Numero,
                                                              Anno,
                                                              Avanzamento))
                        End If

                    Next

                    '--------------------------------------------------------------------------------
                    'Scrittura recupero accise
                    '--------------------------------------------------------------------------------

                    'enum_TipoCarburante_UMA.Gasolio
                    Dim recAccConfGasolio = IIf(IsDBNull(RigaT("Rec_Acc_Conf_Gasolio")), 0, RigaT("Rec_Acc_Conf_Gasolio"))
                    If recAccConfGasolio > 0 Then
                        listaVendite.Add(CaricaUmaVendite(DateTime.Now.Date,
                                                          TipoRichiesta,
                                                          Richiesta_Cod,
                                                          RigaT("Piva"),
                                                          RigaT("Piva"),
                                                          enum_TipoCarburante_UMA.Gasolio,
                                                          -1 * recAccConfGasolio,
                                                          DescrScaricoRecAccise,
                                                          OrigineMovRecAccise,
                                                          Numero,
                                                          Anno,
                                                          Avanzamento))
                    End If

                    'enum_TipoCarburante_UMA.Benzina
                    Dim recAccConfBenzina = IIf(IsDBNull(RigaT("Rec_Acc_Conf_Benzina")), 0, RigaT("Rec_Acc_Conf_Benzina"))
                    If recAccConfBenzina > 0 Then
                        listaVendite.Add(CaricaUmaVendite(DateTime.Now.Date,
                                                          TipoRichiesta,
                                                          Richiesta_Cod,
                                                          RigaT("Piva"),
                                                          RigaT("Piva"),
                                                          enum_TipoCarburante_UMA.Benzina,
                                                          -1 * recAccConfBenzina,
                                                          DescrScaricoRecAccise,
                                                          OrigineMovRecAccise,
                                                          Numero,
                                                          Anno,
                                                          Avanzamento))
                    End If

                    'enum_TipoCarburante_UMA.Gasolio_Serra
                    Dim recAccConfGasolioSerra = IIf(IsDBNull(RigaT("Rec_Acc_Conf_Gasolio_Serra")), 0, RigaT("Rec_Acc_Conf_Gasolio_Serra"))
                    If recAccConfGasolioSerra > 0 Then
                        listaVendite.Add(CaricaUmaVendite(DateTime.Now.Date,
                                                          TipoRichiesta,
                                                          Richiesta_Cod,
                                                          RigaT("Piva"),
                                                          RigaT("Piva"),
                                                          enum_TipoCarburante_UMA.Gasolio_Serra,
                                                          -1 * recAccConfGasolioSerra,
                                                          DescrScaricoRecAccise,
                                                          OrigineMovRecAccise,
                                                          Numero,
                                                          Anno,
                                                          Avanzamento))
                    End If

                    If listaVendite.Count() > 0 Then
                        Dim UMA_Vend As New UMA_Vendite_W
                        messaggioErrore = UMA_Vend.Scrivi_vendite(listaVendite, objParametri)
                        If messaggioErrore <> "" Then
                            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
                        End If
                    End If

                    '--------------------------------------------------------------------------------
                    ' Aggiornamento rimanenze
                    '--------------------------------------------------------------------------------

                    ' Rendicontazione
                    If Avanzamento = 1 Then
                        Dim objRichTestata As New UMA_Richieste_Testata_W
                        objRichTestata.Modifica_Rimanenze_Rendicontazione(Piva, Richiesta_Cod, True, objParametri)
                    End If

                End If

            Else

                Throw New Exception("Attenzione non è stata trovata nessuna richiesta per questa pratica")

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)

        End Try

    End Sub
    Private Function CaricaUmaVendite(Data_Trasferimento As DateTime,
                                      Tipo_Richiesta As Integer,
                                      Richiesta_Cod As Integer,
                                      Piva_venditore As String,
                                      Pivacliente As String,
                                      TipoCarburante As Integer,
                                      Lt As Double,
                                      Note As String,
                                      Origine_Mov As Integer,
                                      Numero As String,
                                      Anno As Integer,
                                      Avanzamento As Integer) As UMA_VenditeToRow

        Dim Ven As New UMA_VenditeToRow

        If Origine_Mov = OrigineMovTrasferimento AndAlso Lt > 0 Then
            Ven.Anno = Year(Data_Trasferimento)
        Else
            Ven.Anno = Anno
        End If

        Ven.Conto_Proprio_Terzi = Tipo_Richiesta

        Ven.PIVA_Cliente = Pivacliente
        Ven.PIVA_Venditore = Piva_venditore

        Ven.Tipo_Carburante = TipoCarburante
        Ven.Lt = Lt

        Ven.Tipo_Documento = 0
        Ven.Data_Documento = Data_Trasferimento

        Dim tipoPratica = ""
        If Avanzamento = 0 Then
            tipoPratica = "Richiesta"
        End If
        If Avanzamento = 1 Then
            tipoPratica = "Rendicontazione"
        End If
        Ven.Nr_Documento = tipoPratica + " " + Anno.ToString() + "/" + Numero.ToString()

        Ven.Note_Rivenditore = Note
        Ven.Origine_Movimento = Origine_Mov
        Ven.Richiesta_Cod = Richiesta_Cod

        Return Ven

    End Function
    Public Function UndoGenerazioneAutomaticaMovimentiRimanenze(ByVal Pratica_Cod As Integer, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim UMA_Vend As New UMA_Vendite_W
        Dim richieste As New AgronicaCoreUmaDal.UMA_Richieste_R
        Dim Trasferimenti As New AgronicaCoreUmaDal.UMA_Richieste_Trasferimenti_R
        Dim Restituzioni As New AgronicaCoreUmaDal.UMA_Richieste_Restituzioni_R
        Dim messaggioErrore As String = ""
        Dim Richiesta_Cod As Integer = 0
        Dim listaVendite As New List(Of UMA_VenditeToRow)
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.GenerazioneAutomaticaMovimentiRimanenze()"
        Try
            Dim DtTestata As DataTable = richieste.Leggi_CodicePratica(Pratica_Cod, objParametri)

            If (DtTestata.Rows.Count > 0) Then

                Dim RigaT As DataRow = DtTestata.Rows(0)
                Richiesta_Cod = IIf(IsDBNull(RigaT("Richiesta_Cod")), 0, RigaT("Richiesta_Cod"))

                If Richiesta_Cod > 0 Then

                    'Lettura setup
                    Dim setup As New UMASetup_R
                    Dim gestioneRimanenze = 0
                    Dim ValInizioData As Date = RigaT("Validita_Inizio")
                    Dim Anno = Year(ValInizioData)
                    Dim dtRiduzione = setup.LeggiSetup(Anno, objParametri)
                    If dtRiduzione.Rows.Count > 0 Then
                        gestioneRimanenze = dtRiduzione.Rows(0).Item("Gestione_Rimanenze")
                    End If

                    'Se gestione rimanenze abilitata
                    If gestioneRimanenze = 1 OrElse (gestioneRimanenze = 2 AndAlso RigaT("Avanzamento_Richiesta") = 1) Then

                        '--------------------------------------------------------------------------------
                        'Cancellazione vendite collegate
                        '--------------------------------------------------------------------------------

                        UMA_Vend.Delete_Vendite_UMA(Richiesta_Cod, objParametri)

                        '--------------------------------------------------------------------------------
                        'Aggiornamento rimanenze
                        '--------------------------------------------------------------------------------

                        ' Rendicontazione
                        If RigaT("Avanzamento_Richiesta") = 1 Then
                            Dim objRichTestata As New UMA_Richieste_Testata_W
                            objRichTestata.Modifica_Rimanenze_Rendicontazione(RigaT("Piva"), Richiesta_Cod, False, objParametri)
                        End If

                    End If

                End If

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return messaggioErrore

    End Function

    Public Function CalcolaRecuperoAccise(ByVal piva As String,
                                          ByVal richiesta_cod As Integer,
                                          ByVal stato As String,
                                          ByVal avanzamento As Integer,
                                          ByVal objParametri As AgronicaCoreParametri,
                                          Optional ByVal aggiornaRecuperoAccise As Boolean = True) As (Boolean, Double, Double, Double)

        Dim trasferiti As New DataTable
        Dim testata = New UMA_Richieste_Testata_R
        Dim testataW = New UMA_Richieste_Testata_W
        Dim trasferimenti = New UMA_Richieste_Trasferimenti_R
        Dim restituzioni = New UMA_Richieste_Restituzioni_R
        Dim NomeRoutine As String = "AgronicaCoreUmaBIZ.UMA_Richieste.CalcolaRecuperoAccise()"
        Dim messaggioErrore As String

        Dim rimastoGasolio As Double
        Dim rimastoBenzina As Double
        Dim rimastoGasolioSerra As Double
        Dim res As Boolean = True

        Try
            Dim testataRes = testata.LeggiDaRichiestaCod(richiesta_cod, objParametri)

            Dim trasferimentiRes = trasferimenti.Leggi(piva, richiesta_cod, objParametri)

            Dim restituzioniRes = restituzioni.Leggi(piva, richiesta_cod, objParametri)

            If testataRes.Rows.Count > 0 Then

                Dim testa = testataRes.Rows.Item(0)

                rimastoGasolio = getDataRowItemTestNull(testa, "Rim_Dich_Gasolio")
                rimastoBenzina = getDataRowItemTestNull(testa, "Rim_Dich_Benzina")
                rimastoGasolioSerra = getDataRowItemTestNull(testa, "Rim_Dich_Gasolio_Serra")

                If trasferimentiRes.Rows.Count > 0 Then
                    For Each row As DataRow In trasferimentiRes.Rows
                        If stato = StatoInCompilazione Then
                            rimastoGasolio -= row.Item("Gasolio")
                            rimastoBenzina -= row.Item("Benzina")
                            rimastoGasolioSerra -= row.Item("Gasolio_Serra")
                        Else
                            rimastoGasolio -= row.Item("Confermato_Gasolio")
                            rimastoBenzina -= row.Item("Confermato_Benzina")
                            rimastoGasolioSerra -= row.Item("Confermato_Gasolio_Serra")
                        End If
                    Next
                End If

                If restituzioniRes.Rows.Count > 0 Then
                    For Each row As DataRow In restituzioniRes.Rows
                        If stato = StatoInCompilazione Then
                            rimastoGasolio -= row.Item("Gasolio")
                            rimastoBenzina -= row.Item("Benzina")
                            rimastoGasolioSerra -= row.Item("Gasolio_Serra")
                        Else
                            rimastoGasolio -= row.Item("Confermato_Gasolio")
                            rimastoBenzina -= row.Item("Confermato_Benzina")
                            rimastoGasolioSerra -= row.Item("Confermato_Gasolio_Serra")
                        End If
                    Next
                End If

                If avanzamento = 1 Then

                    If stato = StatoInCompilazione Then
                        rimastoGasolio -= getDataRowItemTestNull(testa, "Rim_Riass_Gasolio")
                        rimastoBenzina -= getDataRowItemTestNull(testa, "Rim_Riass_Benzina")
                        rimastoGasolioSerra -= getDataRowItemTestNull(testa, "Rim_Riass_Gasolio_Serra")
                    Else
                        rimastoGasolio -= getDataRowItemTestNull(testa, "Rim_Riass_Conf_Gasolio")
                        rimastoBenzina -= getDataRowItemTestNull(testa, "Rim_Riass_Conf_Benzina")
                        rimastoGasolioSerra -= getDataRowItemTestNull(testa, "Rim_Riass_Conf_Gasolio_Serra")
                    End If

                End If

            End If

            If rimastoGasolio < 0 OrElse rimastoBenzina < 0 OrElse rimastoGasolioSerra < 0 Then

                res = False

            End If

            If testataRes.Rows.Count > 0 AndAlso aggiornaRecuperoAccise Then

                Dim testa = testataRes.Rows.Item(0)
                testataW.Modifica_Recupero_Accise(piva,
                                                  richiesta_cod,
                                                  IIf(rimastoGasolio < 0, 0, rimastoGasolio),
                                                  IIf(rimastoBenzina < 0, 0, rimastoBenzina),
                                                  IIf(rimastoGasolioSerra < 0, 0, rimastoGasolioSerra),
                                                  stato,
                                                  objParametri)

            End If

        Catch ex As Exception

            messaggioErrore = ex.Message
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)

        End Try

        Return (res, rimastoGasolio, rimastoBenzina, rimastoGasolioSerra)

    End Function

    Private Function getDataRowItemTestNull(ByRef dr As DataRow, ByVal campo As String) As Double

        Return IIf(IsDBNull(dr.Item(campo)), 0, dr.Item(campo))

    End Function

End Class

Public Class AcquistatoAnnoPrecedente
    Public Property ltGasolio As Double = 0
    Public Property ltBenzina As Double = 0
    Public Property ltGasolioSerra As Double = 0
End Class

Public Class CarburanteHelper
    Public Property Tipo As Integer
    Public Property Calcolato As Double
    Public Property Richiesto As Double
    Public Property Assegnato As Double
End Class

