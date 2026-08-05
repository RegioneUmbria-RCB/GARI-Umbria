Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.Gis

Public Class MaschereLayerRaster_R
    Public Function LeggiElencoMaschere(ByVal layer_cod As Int32,
                                        ByVal tipologia_layer_cod As Int32,
                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                        ByRef objParametri_Server As AgronicaCoreParametri) As ElencoMaschereLayerRaster

        Dim xRead As New AgronicaCoreGisDAL.MaschereLayerRaster_R
        Dim xReadPermessi As New AgronicaCoreGisBIZ.Permessi_Maschera_R

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim attivaMascheraFiltro As Boolean = False

        attivaMascheraFiltro = ObjUtenti.Controlla_Permessi_Utente(objParametri_Utenti.UtenteUsername,
                                                                   TipiEnumerativi.enum_Id_Servizio.GiasOnline,
                                                                   TipiEnumerativi.enum_Security_Attivita.GIS_Gestione_Parametri_Maschere_Raster,
                                                                   TipiEnumerativi.enum_Security_Operazione.Modifica,
                                                                   Date.Now,
                                                                   "",
                                                                   objParametri_Utenti)

        Dim resp As New ElencoMaschereLayerRaster With {
            .elencoMaschere = New List(Of MascheraLayerRaster)
        }

        If layer_cod = TipiEnumerativi.enum_Gis_LayerElementiGrafici_std.ANALISI_MAPPE_SATELLITARI Then
            Dim maschera As New MascheraLayerRaster With {
                .maschera_cod = CostantiPersonalizzate.CodMascheraFiltroMappeSatellitari,
                .maschera_des = "Filtro su impianti",
                .LayerElementiGrafici_cod = 19,
                .TipologiaLayer_cod = 1,
                .LayerElementiGrafici_Raster_cod = layer_cod,
                .TipologiaLayer_Raster_cod = tipologia_layer_cod,
                .isAttivaPerUtenteCorrente = xRead.LeggiAttivazioneMascheraFiltro(objParametri_Server),
                .inizio_validita = CostantiPersonalizzate.AGRODATAINIZIO,
                .fine_validita = CostantiPersonalizzate.AGRODATAFINE,
                .permessiUtenteMaschera = New PermessoMaschera
            }

            Dim permessoMaschera As New PermessoMaschera With {
                .Flag_Amministrazione = 0,
                .Flag_Attivazione = Convert.ToInt32(attivaMascheraFiltro),
                .Flag_Cancellazione = 0,
                .Flag_Informazioni = 0,
                .Flag_Inserimento = 0,
                .Flag_Modifica = 0,
                .UserName = objParametri_Utenti.UtenteUsername
            }

            maschera.permessiUtenteMaschera = permessoMaschera
            resp.elencoMaschere.Add(maschera)
        End If

        Dim xReadGruppi As New AgronicaCoreUtentiDAL.Gruppi_Utente_R

        Dim gruppi_appartenenza As New List(Of Int32)
        Dim DT As DataTable

        DT = xReadGruppi.LeggiGruppiDaUtente(objParametri_Utenti)

        For Each row In DT.Rows
            gruppi_appartenenza.Add(CInt(row("Gruppi_Utente_cod")))
        Next

        Dim maschereDT As DataTable

        maschereDT = xRead.LeggiElencoMaschere(layer_cod, tipologia_layer_cod, gruppi_appartenenza, objParametri_Server)

        If maschereDT Is Nothing Then
            Throw New Exception("Errore nella lettura dell'elenco delle maschere per i layer Raster.")
        End If

        For Each row In maschereDT.Rows
            Dim maschera As New MascheraLayerRaster With {
                .maschera_cod = CInt(row("Maschera_Cod")),
                .maschera_des = row("Maschera_Des").ToString,
                .LayerElementiGrafici_cod = CInt(row("LayerElementiGrafici_cod")),
                .TipologiaLayer_cod = CInt(row("TipologiaLayer_cod")),
                .LayerElementiGrafici_Raster_cod = CInt(row("LayerElementiGrafici_Raster_cod")),
                .TipologiaLayer_Raster_cod = CInt(row("TipologiaLayer_Raster_cod")),
                .isAttivaPerUtenteCorrente = CBool(row("AttivaPerUtente")),
                .inizio_validita = CDate(row("Validita_Inizio")),
                .fine_validita = CDate(row("Validita_Fine")),
                .permessiUtenteMaschera = New PermessoMaschera With {
                    .Flag_Amministrazione = 0,
                    .Flag_Attivazione = 0,
                    .Flag_Cancellazione = 0,
                    .Flag_Informazioni = 0,
                    .Flag_Inserimento = 0,
                    .Flag_Modifica = 0
                }
            }

            Dim permessi = xReadPermessi.LeggiPermessiDaUtente(objParametri_Utenti, objParametri_Server, maschera.maschera_cod)

            If permessi IsNot Nothing _
               AndAlso permessi.elencoPermessiUtente IsNot Nothing _
               AndAlso permessi.elencoPermessiUtente.Count > 0 _
               AndAlso permessi.elencoPermessiUtente.FirstOrDefault.elencoPermessiMaschera IsNot Nothing _
               AndAlso permessi.elencoPermessiUtente.FirstOrDefault.elencoPermessiMaschera.Count > 0 Then

                maschera.permessiUtenteMaschera = permessi.elencoPermessiUtente.FirstOrDefault.elencoPermessiMaschera.FirstOrDefault

            End If

            resp.elencoMaschere.Add(maschera)

        Next

        Return resp
    End Function

End Class
Public Class MaschereLayerRaster_W
    Public Function InserisciMascheraLayerRaster(ByVal Maschera_Des As String,
                                                 ByVal LayerElementiGrafici_Raster_Cod As Int32,
                                                 ByVal TipologiaLayer_Raster_Cod As Int32,
                                                 ByVal LayerElementiGrafici_Cod As Int32,
                                                 ByVal TipologiaLayer_Cod As Int32,
                                                 ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                 ByRef objParametri_Server As AgronicaCoreParametri,
                                                 ByVal Optional Validita_Inizio As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                                                 ByVal Optional Validita_Fine As Date = CostantiPersonalizzate.AGRODATAFINE
                                                 ) As Boolean


        Dim resp As Boolean

        Dim xRead As New AgronicaCoreGisDAL.MaschereLayerRaster_R
        Dim xWrite As New AgronicaCoreGisDAL.MaschereLayerRaster_W
        Dim sequenza As New Agro_Sequenze

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim cod_maschera = xRead.VerificaEsistenzaMaschera(LayerElementiGrafici_Raster_Cod,
                                                               TipologiaLayer_Raster_Cod,
                                                               LayerElementiGrafici_Cod,
                                                               TipologiaLayer_Cod,
                                                               objParametri_Server)

            If cod_maschera <> 0 Then
                Throw New Exception("Maschera già presente a sistema.")
            End If

            Dim nuovoIDMaschera = sequenza.NuovoId_Tabella("GIS_Maschera", 0, Int32.MaxValue, objParametri_Server, True)

            resp = xWrite.InserisciMascheraLayerRaster(nuovoIDMaschera,
                                                       Maschera_Des,
                                                       LayerElementiGrafici_Raster_Cod,
                                                       TipologiaLayer_Raster_Cod,
                                                       LayerElementiGrafici_Cod,
                                                       TipologiaLayer_Cod,
                                                       Validita_Inizio,
                                                       Validita_Fine,
                                                       objParametri_Server)

            If Not resp Then
                Throw New Exception("Errore nell'inserimento della maschera.")
            End If

            Dim xWritePermessi As New AgronicaCoreGisDAL.Permessi_Maschera_W

            resp = xWritePermessi.InsertPermesso(1, 1, 1, 1, 1, 1,
                                                 nuovoIDMaschera,
                                                 objParametri_Utenti.UtenteUsername,
                                                 0,
                                                 objParametri_Server)

            If Not resp Then
                Throw New Exception("Errore nell'inserimento dei permessi sulla nuova maschera.")
            End If

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If
        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return resp

    End Function

    Public Function AggiornaMascheraLayerRaster(ByVal Maschera_Cod As Int32,
                                                ByVal Maschera_Des As String,
                                                ByVal LayerElementiGrafici_Raster_Cod As Int32,
                                                ByVal TipologiaLayer_Raster_Cod As Int32,
                                                ByVal LayerElementiGrafici_Cod As Int32,
                                                ByVal TipologiaLayer_Cod As Int32,
                                                ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByVal Optional Validita_Inizio As Date = CostantiPersonalizzate.AGRODATAINIZIO,
                                                ByVal Optional Validita_Fine As Date = CostantiPersonalizzate.AGRODATAFINE
                                                ) As Boolean

        Dim resp As Boolean

        Dim xRead As New AgronicaCoreGisDAL.MaschereLayerRaster_R
        Dim xWrite As New AgronicaCoreGisDAL.MaschereLayerRaster_W
        Dim sequenza As New Agro_Sequenze

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            If Maschera_Cod = 0 Then
                Throw New Exception("E' necessario fornire un codice per la maschera che si vuole modificare")
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            Dim cod_maschera = xRead.VerificaEsistenzaMaschera(LayerElementiGrafici_Raster_Cod,
                                                               TipologiaLayer_Raster_Cod,
                                                               LayerElementiGrafici_Cod,
                                                               TipologiaLayer_Cod,
                                                               objParametri_Server)

            If cod_maschera <> 0 AndAlso cod_maschera <> Maschera_Cod Then
                Throw New Exception("Maschera già presente a sistema.")
            End If

            resp = xWrite.AggiornaMascheraLayerRaster(Maschera_Cod,
                                                      Maschera_Des,
                                                      LayerElementiGrafici_Raster_Cod,
                                                      TipologiaLayer_Raster_Cod,
                                                      LayerElementiGrafici_Cod,
                                                      TipologiaLayer_Cod,
                                                      Validita_Inizio,
                                                      Validita_Fine,
                                                      objParametri_Server)

            If Not resp Then
                Throw New Exception("Errore nell'inserimento della maschera.")
            End If

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If
        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return resp

    End Function

    Public Function EliminaMascheraLayerRaster(ByVal Maschera_Cod As Int32,
                                               ByRef objParametri_Utenti As AgronicaCoreParametri,
                                               ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.MaschereLayerRaster_W

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            resp = xWrite.EliminaMascheraLayerRaster(Maschera_Cod, objParametri_Server)

            If Not resp Then
                Throw New Exception("Errore nella cancellazione della maschera.")
            End If

            Dim xWritePermessi As New AgronicaCoreGisDAL.Permessi_Maschera_W

            xWritePermessi.DeletePermessiInteraMaschera(Maschera_Cod, objParametri_Server)

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Throw ex
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return resp

    End Function

    Public Function AttivaDisattivaMaschera(ByVal Maschera_Cod As Integer,
                                            ByVal isAttivo As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.MaschereLayerRaster_W

        resp = xWrite.EliminaAttivazione(Maschera_Cod, objParametri_Server)

        If Not resp Then
            Throw New Exception("Errore Nell'eliminazione della vecchia impostazione")
        End If

        resp = xWrite.InserisciAttivazione(Maschera_Cod, isAttivo, objParametri_Server)

        If Not resp Then
            Throw New Exception("Errore Nell'eliminazione della vecchia impostazione")
        End If

        Return resp

    End Function
End Class
