
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.Gis.PermessiLayer
Imports Microsoft.VisualBasic.ApplicationServices

Public Class GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiPermessiLayer(ByVal LayerCod As Integer,
                                        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreModelsSTD.Gis.PermessiLayer.LeggiPermessiLayer_Out

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_R.LeggiPermessiUtente()"
        Dim MessaggioErrore As String = ""
        Dim ret As New AgronicaCoreModelsSTD.Gis.PermessiLayer.LeggiPermessiLayer_Out

        Dim objGISLayer As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R
        Dim objPermessiUtenti As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXUtente_R
        Dim objPermessiGruppi As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXGruppiUtente_R

        If LayerCod = 0 Then
            Throw New Exception("Codice Layer non valorizzato")
        End If

        Dim dtLayer = objGISLayer.Leggi(objParametri_server.PivaSuperUser, "", LayerCod, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)

        ret.LayerCod = LayerCod
        If dtLayer.Rows.Count > 0 Then
            ret.LayerDescr = dtLayer.Rows(0)("LayerElementiGrafici_Des")
        End If

        Try
            Dim dtUserRights = objPermessiUtenti.Leggi(LayerCod, "", "", "", objParametri_server, objParametri_utenti)
            For Each user In dtUserRights.Rows
                ret.utenti_permessi.Add(New AgronicaCoreModelsSTD.Gis.PermessiLayer.PermessiXUtente() With {
                                            .Username = user("Utente"),
                                            .UsernameDescr = user("Nome") & " " & user("Cognome"),
                                            .Flag_Inserimento = user("Flag_Inserimento"),
                                            .Flag_Modifica = user("Flag_Modifica"),
                                            .Flag_Cancellazione = user("Flag_Cancellazione"),
                                            .Flag_Informazioni = user("Flag_Informazioni"),
                                            .Flag_Amministrazione = user("Flag_Amministrazione"),
                                            .Flag_Rimozione = user("Flag_Rimozione")
                                        })
            Next

            Dim dGroupRights = objPermessiGruppi.Leggi(LayerCod, 0, "", "", objParametri_server, objParametri_utenti)
            For Each group In dGroupRights.Rows
                ret.gruppiutente_permessi.Add(New AgronicaCoreModelsSTD.Gis.PermessiLayer.PermessiXGruppiUtente() With {
                                            .Gruppo = group("Gruppi_Utente_cod"),
                                            .GruppoDescr = group("Gruppi_Utente_Des"),
                                            .Flag_Inserimento = group("Flag_Inserimento"),
                                            .Flag_Modifica = group("Flag_Modifica"),
                                            .Flag_Cancellazione = group("Flag_Cancellazione"),
                                            .Flag_Informazioni = group("Flag_Informazioni"),
                                            .Flag_Amministrazione = group("Flag_Amministrazione"),
                                            .Flag_Rimozione = group("Flag_Rimozione")
                                        })
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            ret = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ret

    End Function

    Public Function LeggiPermessiLayerUtente(ByVal LayerCod As Integer,
                                        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreModelsSTD.Gis.PermessiLayer.LeggiPermessiLayerUtenti_Out

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_R.LeggiPermessiLayerUtente()"
        Dim MessaggioErrore As String = ""
        Dim ret As New AgronicaCoreModelsSTD.Gis.PermessiLayer.LeggiPermessiLayerUtenti_Out

        Dim objGISLayer As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R
        Dim objPermessiUtenti As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXUtente_R

        If LayerCod = 0 Then
            Throw New Exception("Codice Layer non valorizzato")
        End If

        Dim dtLayer = objGISLayer.Leggi(objParametri_server.PivaSuperUser, "", LayerCod, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)

        ret.LayerCod = LayerCod
        If dtLayer.Rows.Count > 0 Then
            ret.LayerDescr = dtLayer.Rows(0)("LayerElementiGrafici_Des")
        End If

        Try
            Dim dtUserRights = objPermessiUtenti.Leggi(LayerCod, "", "", "", objParametri_server, objParametri_utenti)
            For Each user In dtUserRights.Rows
                ret.utenti_permessi.Add(New AgronicaCoreModelsSTD.Gis.PermessiLayer.PermessiXUtente() With {
                                            .Username = user("Utente"),
                                            .UsernameDescr = user("Nome") & " " & user("Cognome"),
                                            .Flag_Inserimento = user("Flag_Inserimento"),
                                            .Flag_Modifica = user("Flag_Modifica"),
                                            .Flag_Cancellazione = user("Flag_Cancellazione"),
                                            .Flag_Informazioni = user("Flag_Informazioni"),
                                            .Flag_Amministrazione = user("Flag_Amministrazione"),
                                            .Flag_Rimozione = user("Flag_Rimozione")
                                        })
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            ret = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ret

    End Function

    Public Function LeggiPermessiLayerGruppiUtente(ByVal LayerCod As Integer,
                                        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreModelsSTD.Gis.PermessiLayer.LeggiPermessiLayerGruppiUtente_Out

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_R.LeggiPermessiLayerGruppiUtente()"
        Dim MessaggioErrore As String = ""
        Dim ret As New AgronicaCoreModelsSTD.Gis.PermessiLayer.LeggiPermessiLayerGruppiUtente_Out

        Dim objGISLayer As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R
        Dim objPermessiGruppi As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXGruppiUtente_R

        If LayerCod = 0 Then
            Throw New Exception("Codice Layer non valorizzato")
        End If

        Dim dtLayer = objGISLayer.Leggi(objParametri_server.PivaSuperUser, "", LayerCod, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)

        ret.LayerCod = LayerCod
        If dtLayer.Rows.Count > 0 Then
            ret.LayerDescr = dtLayer.Rows(0)("LayerElementiGrafici_Des")
        End If


        Try

            Dim dGroupRights = objPermessiGruppi.Leggi(LayerCod, 0, "", "", objParametri_server, objParametri_utenti)
            For Each group In dGroupRights.Rows
                ret.gruppiutente_permessi.Add(New AgronicaCoreModelsSTD.Gis.PermessiLayer.PermessiXGruppiUtente() With {
                                            .Gruppo = group("Gruppi_Utente_cod"),
                                            .GruppoDescr = IIf(group("Gruppi_Utente_Des") Is DBNull.Value, "", group("Gruppi_Utente_Des")),
                                            .Flag_Inserimento = group("Flag_Inserimento"),
                                            .Flag_Modifica = group("Flag_Modifica"),
                                            .Flag_Cancellazione = group("Flag_Cancellazione"),
                                            .Flag_Informazioni = group("Flag_Informazioni"),
                                            .Flag_Amministrazione = group("Flag_Amministrazione"),
                                            .Flag_Rimozione = group("Flag_Rimozione")
                                        })
            Next

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            ret = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return ret

    End Function

End Class
Public Class GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function AggiornaPermessiXUtente_Gruppo(ByVal LayerCod As Integer,
                                                   ByVal ElencoPermessiUtente As List(Of PermessiXUtente),
                                                   ByVal ElencoPermessiGruppi As List(Of PermessiXGruppiUtente),
                                                   ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W.AggiornaPermessiXUtente_Gruppo()"
        Dim MessaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xRisp As Boolean = False
        If LayerCod = 0 Then
            Throw New Exception("Codice Layer non valorizzato")
        End If
        If LayerCod < 1000000 Then
            Throw New Exception("Non è possibile impostare permessi per i Layers Standard, solo per quelli personalizzati")
        End If

        Try
            'Apro connessione DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_server)

            Dim objPermessiUtenti As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXUtente_W
            Dim objPermessiGruppi As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXGruppiUtente_W

            If ElencoPermessiUtente IsNot Nothing Then
                objPermessiUtenti.Cancella(LayerCod,
                                           "",
                                           "",
                                           objParametri_server)
                For Each user In ElencoPermessiUtente

                    objPermessiUtenti.Scrivi(LayerCod,
                                         user.Username,
                                         user.Flag_Inserimento,
                                         user.Flag_Modifica,
                                         user.Flag_Cancellazione,
                                         user.Flag_Informazioni,
                                         user.Flag_Amministrazione,
                                         user.Flag_Rimozione,
                                         AGRODATAINIZIO,
                                         AGRODATAFINE,
                                         objParametri_server)
                Next
            End If

            If ElencoPermessiGruppi IsNot Nothing Then
                objPermessiGruppi.Cancella(LayerCod,
                                           0,
                                           "",
                                           objParametri_server)
                For Each group In ElencoPermessiGruppi

                    objPermessiGruppi.Scrivi(LayerCod,
                                         group.Gruppo,
                                         group.Flag_Inserimento,
                                         group.Flag_Modifica,
                                         group.Flag_Cancellazione,
                                         group.Flag_Informazioni,
                                         group.Flag_Amministrazione,
                                         group.Flag_Rimozione,
                                         AGRODATAINIZIO,
                                         AGRODATAFINE,
                                         objParametri_server)
                Next
            End If

            'Chiudo transazione DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)

            xRisp = True

        Catch ex As Exception

            'Rollback transazione DB
            If Not objParametri_server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            'Chiudo connessione DB
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return xRisp
    End Function

    Public Function AggiornaPermessiXUtente(ByVal LayerCod As Integer,
                                            ByVal ElencoPermessiUtente As List(Of PermessiXUtente),
                                            ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W.AggiornaPermessiXUtente()"
        Dim MessaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xRisp As Boolean = False
        If LayerCod = 0 Then
            Throw New Exception("Codice Layer non valorizzato")
        End If
        If LayerCod < 1000000 Then
            Throw New Exception("Non è possibile impostare permessi per i Layers Standard, solo per quelli personalizzati")
        End If

        Try
            'Apro connessione DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_server)

            Dim objPermessiUtenti As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXUtente_W
            Dim objPermessiUtentiR As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXUtente_R
            Dim objLayerElemGraficiR As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R
            Dim objLayerElemGraficiW As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W
            Dim objLayerElemGrafici As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici



            If ElencoPermessiUtente IsNot Nothing Then
                'controllo aggiuntivo per presenza record su GIS_layerelementigrafici
                'dt di partenza per eventuali record mancanti
                Dim dtLayerSource = objLayerElemGraficiR.Leggi(objParametri_server.PivaSuperUser, objParametri_server.UtenteUsername, LayerCod, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)
                If dtLayerSource.Rows.Count <= 0 Then
                    Throw New Exception("Record su GIS_layerElementiGrafici per l'utente " + objParametri_server.UtenteUsername + "non presente impossibile proseguire")
                End If


                objPermessiUtenti.Cancella(LayerCod,
                                           "",
                                           "",
                                           objParametri_server)

                For Each user In ElencoPermessiUtente
                    objPermessiUtenti.Scrivi(LayerCod,
                                         user.Username,
                                         user.Flag_Inserimento,
                                         user.Flag_Modifica,
                                         user.Flag_Cancellazione,
                                         user.Flag_Informazioni,
                                         user.Flag_Amministrazione,
                                         user.Flag_Rimozione,
                                         AGRODATAINIZIO,
                                         AGRODATAFINE,
                                         objParametri_server)

                    Dim check = objLayerElemGraficiR.Leggi(objParametri_server.PivaSuperUser, user.Username, LayerCod, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)
                    If check.Rows.Count <= 0 Then
                        objLayerElemGraficiW.Scrivi(objParametri_server.PivaSuperUser,
                                                    user.Username,
                                                    LayerCod,
                                                    dtLayerSource.Rows(0)("LayerElementiGrafici_Des"),
                                                    dtLayerSource.Rows(0)("Flag_Attivo"),
                                                    dtLayerSource.Rows(0)("Flag_Visibile"),
                                                    IIf(dtLayerSource.Rows(0)("Colore_Base") Is DBNull.Value, "", dtLayerSource.Rows(0)("Colore_Base")),
                                                    IIf(dtLayerSource.Rows(0)("Colore_Selezionato") Is DBNull.Value, "", dtLayerSource.Rows(0)("Colore_Selezionato")),
                                                    IIf(dtLayerSource.Rows(0)("Colore_Primario") Is DBNull.Value, "", dtLayerSource.Rows(0)("Colore_Primario")),
                                                    IIf(dtLayerSource.Rows(0)("Colore_Secondario") Is DBNull.Value, "", dtLayerSource.Rows(0)("Colore_Secondario")),
                                                    dtLayerSource.Rows(0)("Varianza"),
                                                    dtLayerSource.Rows(0)("Trasparenza"),
                                                    dtLayerSource.Rows(0)("MostraDescrizioneAssociata"),
                                                    dtLayerSource.Rows(0)("TipologiaLayer_Cod"),
                                                    dtLayerSource.Rows(0)("ZIndex"),
                                                    IIf(dtLayerSource.Rows(0)("Icona16") Is DBNull.Value, "", dtLayerSource.Rows(0)("Icona16")),
                                                    IIf(dtLayerSource.Rows(0)("Icona32") Is DBNull.Value, "", dtLayerSource.Rows(0)("Icona32")),
                                                    AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    objParametri_server
                                )
                    End If
                Next

                Dim dtClear = objLayerElemGraficiR.LeggiElencoRecordLayerElementiGraficiSenzaPermessi(objParametri_server.PivaSuperUser, LayerCod, objParametri_server, objParametri_utenti)
                For Each row In dtClear.Rows
                    objLayerElemGrafici.DeleteLayer(objParametri_server.PivaSuperUser, row("Utente"), LayerCod, "", objParametri_server)
                Next
            End If

            'Chiudo transazione DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)

            xRisp = True

        Catch ex As Exception

            'Rollback transazione DB
            If Not objParametri_server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            'Chiudo connessione DB
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
        End Try

        Return xRisp
    End Function

    Public Function AggiornaPermessiXGruppiUtente(ByVal LayerCod As Integer,
                                                  ByVal ElencoPermessiGruppi As List(Of PermessiXGruppiUtente),
                                                  ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean


        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici_Permessi_Utente_GruppiUtente_W.AggiornaPermessiXGruppiUtente()"
        Dim MessaggioErrore As String = ""
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim xRisp As Boolean = False
        If LayerCod = 0 Then
            Throw New Exception("Codice Layer non valorizzato")
        End If
        If LayerCod < 1000000 Then
            Throw New Exception("Non è possibile impostare permessi per i Layers Standard, solo per quelli personalizzati")
        End If

        Try
            'Apro connessione DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri_server)

            Dim objPermessiGruppi As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXGruppiUtente_W
            Dim objLayerElemGraficiR As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R
            Dim objLayerElemGraficiW As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W
            Dim objLayerElemGrafici As New AgronicaCoreGisBIZ.GIS_LayerElementiGrafici
            Dim objUtentiGruppiUtente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R

            If ElencoPermessiGruppi IsNot Nothing Then
                'controllo aggiuntivo per presenza record su GIS_layerelementigrafici
                'dt di partenza per eventuali record mancanti
                Dim dtLayerSource = objLayerElemGraficiR.Leggi(objParametri_server.PivaSuperUser, objParametri_server.UtenteUsername, LayerCod, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)
                If dtLayerSource.Rows.Count <= 0 Then
                    Throw New Exception("Record su GIS_layerElementiGrafici per l'utente " + objParametri_server.UtenteUsername + "non presente impossibile proseguire")
                End If

                objPermessiGruppi.Cancella(LayerCod,
                                           0,
                                           "",
                                           objParametri_server)
                For Each group In ElencoPermessiGruppi
                    objPermessiGruppi.Scrivi(LayerCod,
                                         group.Gruppo,
                                         group.Flag_Inserimento,
                                         group.Flag_Modifica,
                                         group.Flag_Cancellazione,
                                         group.Flag_Informazioni,
                                         group.Flag_Amministrazione,
                                         group.Flag_Rimozione,
                                         AGRODATAINIZIO,
                                         AGRODATAFINE,
                                         objParametri_server)

                    Dim dtUtenti = objUtentiGruppiUtente.Leggi("", group.Gruppo, "", "", objParametri_utenti)
                    For Each userRow In dtUtenti.Rows
                        Dim check = objLayerElemGraficiR.Leggi(objParametri_server.PivaSuperUser, userRow("userName"), LayerCod, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)
                        If check.Rows.Count <= 0 Then
                            objLayerElemGraficiW.Scrivi(objParametri_server.PivaSuperUser,
                                                            userRow("userName"),
                                                            LayerCod,
                                                            dtLayerSource.Rows(0)("LayerElementiGrafici_Des"),
                                                            dtLayerSource.Rows(0)("Flag_Attivo"),
                                                            dtLayerSource.Rows(0)("Flag_Visibile"),
                                                            IIf(dtLayerSource.Rows(0)("Colore_Base") Is DBNull.Value, "", dtLayerSource.Rows(0)("Colore_Base")),
                                                            IIf(dtLayerSource.Rows(0)("Colore_Selezionato") Is DBNull.Value, "", dtLayerSource.Rows(0)("Colore_Selezionato")),
                                                            IIf(dtLayerSource.Rows(0)("Colore_Primario") Is DBNull.Value, "", dtLayerSource.Rows(0)("Colore_Primario")),
                                                            IIf(dtLayerSource.Rows(0)("Colore_Secondario") Is DBNull.Value, "", dtLayerSource.Rows(0)("Colore_Secondario")),
                                                            dtLayerSource.Rows(0)("Varianza"),
                                                            dtLayerSource.Rows(0)("Trasparenza"),
                                                            dtLayerSource.Rows(0)("MostraDescrizioneAssociata"),
                                                            dtLayerSource.Rows(0)("TipologiaLayer_Cod"),
                                                            dtLayerSource.Rows(0)("ZIndex"),
                                                            IIf(dtLayerSource.Rows(0)("Icona16") Is DBNull.Value, "", dtLayerSource.Rows(0)("Icona16")),
                                                            IIf(dtLayerSource.Rows(0)("Icona32") Is DBNull.Value, "", dtLayerSource.Rows(0)("Icona32")),
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                            objParametri_server
                                        )
                        End If
                    Next

                Next

                Dim dtClear = objLayerElemGraficiR.LeggiElencoRecordLayerElementiGraficiSenzaPermessi(objParametri_server.PivaSuperUser, LayerCod, objParametri_server, objParametri_utenti)
                For Each row In dtClear.Rows
                    objLayerElemGrafici.DeleteLayer(objParametri_server.PivaSuperUser, row("Utente"), LayerCod, "", objParametri_server)
                Next
            End If

            'Chiudo transazione DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)

            xRisp = True

        Catch ex As Exception

            'Rollback transazione DB
            If Not objParametri_server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)
            End If

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        Finally
            'Chiudo connessione DB
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
        End Try

        Return xRisp
    End Function

    Public Function ImpostaLayerGisSePermessiCartografia(ByVal utenteDestinazione As String,
                                                         ByRef objParametri_server As AgronicaCoreParametri) As String
        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False
        Dim messaggioErrore As String = ""

        Try
            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Apro la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                            FlagTransazioneLocale,
                                                                            objParametri_server)

            Dim xGisDalPermessi As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W

            Dim bRisUpdateSuperUserOneShot As Boolean =
                xGisDalPermessi.UpdateSuperUserOneShot(objParametri_server.PivaSuperUser, objParametri_server)

            If Not bRisUpdateSuperUserOneShot Then
                Throw New Exception("Cfg Super user non riuscita")
            End If

            Dim bRisRicopiaLayerDaAltroUtenteTipologia As Boolean =
                xGisDalPermessi.RicopiaLayerDaAltroUtenteTipologia(objParametri_server.SuperUserUsername, utenteDestinazione, 1, objParametri_server)

            If Not bRisRicopiaLayerDaAltroUtenteTipologia Then
                Throw New Exception("Copia da utente non riuscita")
            End If

            Dim bRisRicopiaLayerTilesDaAltroUtenteTipologia As Boolean =
                xGisDalPermessi.RicopiaLayerTilesDaAltroUtenteTipologia(objParametri_server.SuperUserUsername, utenteDestinazione, 1, objParametri_server)

            If Not bRisRicopiaLayerTilesDaAltroUtenteTipologia Then
                Throw New Exception("Copia da utente non riuscita")
            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Chiudo la transazione e la connessione al DB
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_server)

        Catch ex As Exception
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            'Faccio il rollback della transazione
            If Not objParametri_server.objTransazione Is Nothing Then
                'objParametri.objTransazione.Rollback()
                'objParametri.objTransazione = Nothing
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_server)

            End If

            messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)


            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then
                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione ..."
            End If

            messaggioErrore = Messaggio
        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_server)
        End Try

        Return messaggioErrore
    End Function


End Class
