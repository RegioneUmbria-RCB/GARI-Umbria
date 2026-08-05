Imports AgronicaCoreDataProvider
Imports System.Transactions
Imports AgronicaCoreUmaDal
Imports Microsoft.VisualBasic.Logging
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Public Class UMA_Richieste_Anticipi_BIZ

    Inherits AgronicaCoreDataProvider.LogProvider
    Public Function ModificaAnticipo(ByVal richiestaCod As Integer,
                                    ByVal percAnticipo As Double,
                                    ByVal gasolioAnticipo As Double,
                                    ByVal benzinaAnticipo As Double,
                                    ByVal gasolioSerraAnticipo As Double,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim TestataWDAL As New UMA_Richieste_Testata_W
        Dim TestataRDAL As New UMA_Richieste_Testata_R
        Dim VenditeUMA As New UMA_Vendite_R
        Dim MessaggioErrore As String
        Dim result As String = ""
        Dim FlagConnessioneLocale As Boolean = False
        Dim FlagTransazioneLocale As Boolean = False
        Dim NomeRoutine As String = "UMA_Richieste_Anticipi_BIZ.ModificaAnticipo()"

        Dim dtTestata = TestataRDAL.LeggiDaRichiestaCod(richiestaCod, objParametri)

        Dim piva = CStr(dtTestata.Rows.Item(0).Item("Piva"))
        Dim tipoRichiesta = CInt(dtTestata.Rows.Item(0).Item("Tipo_Richiesta"))

        Dim anno = TestataRDAL.RecuperaAnnoRichiesta(richiestaCod, objParametri)

        Dim dtVendite = VenditeUMA.LeggiCarburanteVenduto(piva, anno, tipoRichiesta, 0, "", objParametri)

        Dim carb As Double = 0
        Dim tempCarb As Double = 0

        For Each row In dtVendite.Rows
            carb = row.Item("Totale_Carb")
            Select Case CInt(row.Item("Tipo_Carburante"))
                Case enum_TipoCarburante_UMA.Gasolio
                    tempCarb = gasolioAnticipo
                Case enum_TipoCarburante_UMA.Benzina
                    tempCarb = benzinaAnticipo
                Case enum_TipoCarburante_UMA.Gasolio_Serra
                    tempCarb = gasolioSerraAnticipo
                Case Else

            End Select
            If carb > 0 AndAlso carb > tempCarb Then
                result = "Anticipo inserito inferiore al carburante già acquistato"
            End If
        Next

        If result = "" Then
            Dim transactionOptions = New TransactionOptions With {
                .IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            }
            Using scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
                Try
                    SostituisciLavorazioneFittizia(piva, richiestaCod, enum_TipoCarburante_UMA.Gasolio, gasolioAnticipo, objParametri, result, scope)
                    SostituisciLavorazioneFittizia(piva, richiestaCod, enum_TipoCarburante_UMA.Benzina, benzinaAnticipo, objParametri, result, scope)
                    SostituisciLavorazioneFittizia(piva, richiestaCod, enum_TipoCarburante_UMA.Gasolio_Serra, gasolioSerraAnticipo, objParametri, result, scope)

                    TestataWDAL.Modifica_Campo_Richiesta("Carburante_Richiesto_Gasolio", gasolioAnticipo, piva, richiestaCod, objParametri)
                    TestataWDAL.Modifica_Campo_Richiesta("Carburante_Richiesto_Benzina", benzinaAnticipo, piva, richiestaCod, objParametri)
                    TestataWDAL.Modifica_Campo_Richiesta("Carburante_Richiesto_Gasolio_Serra", gasolioSerraAnticipo, piva, richiestaCod, objParametri)
                    TestataWDAL.Modifica_Campo_Richiesta("Percentuale_Anticipo_Richiesta_da_Azienda", percAnticipo, piva, richiestaCod, objParametri)

                    ' COMMIT Effettivo
                    scope.Complete()
                Catch ex As Exception
                    result = Nothing
                    MessaggioErrore = ex.Message
                    Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
                    ' Rollback
                    scope.Dispose()
                    Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
                End Try
            End Using
        End If

        Return result

    End Function

    Private Sub SostituisciLavorazioneFittizia(piva As String,
                                             richiesta_cod As Integer,
                                             tipoCarburante As Integer,
                                             carburanteRichiesto As Double,
                                             ByRef objParametri_server As AgronicaCoreParametri,
                                             MessaggioErrore As String,
                                             scope As TransactionScope)

        Dim TopCode As Integer
        Dim BaseCode As Integer
        Dim richiestaLAvorazioneRiga As New AgronicaCoreUmaDal.UMA_Richieste_Lavorazioni_W
        Dim deleteLavorazioni As New ArrayList

        'If carburanteRichiesto > 0 Then

        AgronicaCoreDataProvider.UtilityProvider.Calcola_BaseCode_TopCode(BaseCode, TopCode, 1)
            Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
            Dim gEfUtils As New Gias_EF_Utility
            Dim efConnString As String = gEfUtils.GetEntityConnectionString(objParametri_server.StringaConnessione)
            Using GiasContext As New Gias_DeveloperServer_Entities(efConnString)


                Dim lav = (From umaLav In
                                     GiasContext.UMA_Richieste_Lavorazioni Where
                                                                   umaLav.Piva = piva And
                                                                   umaLav.Richiesta_Cod = richiesta_cod And
                                                                   umaLav.Tipo_Carburante = tipoCarburante).SingleOrDefault

                If Not IsNothing(lav) Then

                    deleteLavorazioni.Add(lav)
                    richiestaLAvorazioneRiga.Aggiorna_Lavorazioni(New ArrayList, New ArrayList, New ArrayList, deleteLavorazioni, objParametri_server)

                End If

            End Using


            Dim richiesta_dettaglio_cod = objSequenze.NuovoId_Tabella("UMA_Richieste_Lavorazioni", BaseCode, TopCode, objParametri_server)
            Dim NuovaRichiestaLavorazioneFittizia = CreaRichiestaLavorazioneFittizia(objParametri_server.PivaSuperUser,
                                                                                        piva,
                                                                                        richiesta_cod,
                                                                                        richiesta_dettaglio_cod,
                                                                                        objParametri_server.UtenteUsername,
                                                                                        carburanteRichiesto,
                                                                                        tipoCarburante)


            Dim res = richiestaLAvorazioneRiga.Nuova_Richiesta(NuovaRichiestaLavorazioneFittizia, objParametri_server)

            If Not String.IsNullOrEmpty(MessaggioErrore) Or res <> 0 Then
                ' Rollback
                scope.Dispose()
            End If
        'End If

    End Sub

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
End Class
