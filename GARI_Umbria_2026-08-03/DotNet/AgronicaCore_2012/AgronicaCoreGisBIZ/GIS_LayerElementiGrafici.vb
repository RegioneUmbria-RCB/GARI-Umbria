Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Gis
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class GIS_LayerElementiGrafici
    Inherits DataProvider

    Public Function LeggiLayer(ByVal Layer_Cod As Integer,
                               ByVal Layer_Des As String,
                               ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                               ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCoreModelsSTD.Gis.PermessiLayer.LeggiPermessiLayer_Out

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.LeggiLayer()"
        Dim MessaggioErrore As String = ""
        Dim ret As New AgronicaCoreModelsSTD.Gis.PermessiLayer.LeggiPermessiLayer_Out

        Dim objGISLayer As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R
        Dim objPermessiUtenti As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXUtente_R
        Dim objPermessiGruppi As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXGruppiUtente_R

        Dim dtLayer As DataTable = Nothing

        If Layer_Cod > 0 Then
            dtLayer = objGISLayer.Leggi(objParametri_server.PivaSuperUser, "", Layer_Cod, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
        ElseIf Layer_Des <> "" Then
            dtLayer = objGISLayer.Leggi(objParametri_server.PivaSuperUser, "", 0, 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, " LayerElementiGrafici_Des ='" + Layer_Des + "' ", "", objParametri_server)
        Else
            Throw New Exception("Specificare codice layer o descrizione layer per poter procedere.")
        End If

        If dtLayer.Rows.Count > 0 Then
            ret.LayerCod = dtLayer.Rows(0)("LayerElementiGrafici_Cod")
            ret.LayerDescr = dtLayer.Rows(0)("LayerElementiGrafici_Des")

            Try
                Dim dtUserRights = objPermessiUtenti.Leggi(ret.LayerCod, "", "", "", objParametri_server, objParametri_utenti)
                For Each user In dtUserRights.Rows
                    ret.utenti_permessi.Add(New AgronicaCoreModelsSTD.Gis.PermessiLayer.PermessiXUtente() With {
                                        .Username = user("Utente"),
                                        .UsernameDescr = user("Nome") & " " & user("Cognome"),
                                        .Flag_Inserimento = user("Flag_Inserimento"),
                                        .Flag_Modifica = user("Flag_Modifica"),
                                        .Flag_Cancellazione = user("Flag_Cancellazione"),
                                        .Flag_Informazioni = user("Flag_Informazioni"),
                                        .Flag_Amministrazione = user("Flag_Amministrazione")
                                    })
                Next

                Dim dGroupRights = objPermessiGruppi.Leggi(ret.LayerCod, 0, "", "", objParametri_server, objParametri_utenti)
                For Each group In dGroupRights.Rows
                    ret.gruppiutente_permessi.Add(New AgronicaCoreModelsSTD.Gis.PermessiLayer.PermessiXGruppiUtente() With {
                                        .Gruppo = group("Gruppi_Utente_cod"),
                                        .GruppoDescr = group("Gruppi_Utente_Des"),
                                        .Flag_Inserimento = group("Flag_Inserimento"),
                                        .Flag_Modifica = group("Flag_Modifica"),
                                        .Flag_Cancellazione = group("Flag_Cancellazione"),
                                        .Flag_Informazioni = group("Flag_Informazioni"),
                                        .Flag_Amministrazione = group("Flag_Amministrazione")
                                    })
                Next

            Catch ex As Exception
                MessaggioErrore = ex.Message
                Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
                ret = Nothing
                Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            End Try

        End If



        Return ret

    End Function

    Public Function LeggiTipiOggettoPerElementoGrafico(ByVal PivaSuperUser As String,
                                                     ByVal LayerElementiGrafici_Cod As Int32,
                                                     ByRef objParametri_Server As AgronicaCoreParametri) As LeggiTipiOggettoPerElementoGrafico

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.LeggiTipiOggettoPerElementoGrafico()"

        Dim messaggioErrore As String = ""

        Dim ret As New LeggiTipiOggettoPerElementoGrafico
        ret.PivaSuperUser = ""
        ret.LayerElementiGrafici_Cod = 0
        ret.ListaTipiOggetto = New List(Of Int32)

        Dim DT As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXTipoOggetto_R

        DT = xRead.Leggi(PivaSuperUser,
                         LayerElementiGrafici_Cod,
                         0,
                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                         "",
                         "",
                         objParametri_Server)

        If Not (DT Is Nothing Or DT.Rows.Count <= 0) Then

            Dim firstRow = DT.Rows(0)

            ret.PivaSuperUser = firstRow("PivaSuperUser").ToString
            ret.LayerElementiGrafici_Cod = Int32.Parse(firstRow("LayerElementiGrafici_Cod"))

            For Each row In DT.Rows
                ret.ListaTipiOggetto.Add(Int32.Parse(row("GIS_TipoOggetto_Cod")))
            Next
        End If

        Return ret

    End Function

    Public Function AggiornaElementoGraficoPerTipoOggetto(ByVal PivaSuperUser As String,
                                                     ByVal LayerElementiGrafici_Cod As Int32,
                                                     ByVal GIS_TipoOggetto_Cod As Int32,
                                                     ByVal GIS_TipoOggetto_Cod_Prev As Int32,
                                                     ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.AggiornaElementoGraficoPerTipoOggetto()"

        Dim messaggioErrore As String = ""

        Dim success As Boolean

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXTipoOggetto_R

        Dim DT = xRead.Leggi(PivaSuperUser,
                             LayerElementiGrafici_Cod,
                             GIS_TipoOggetto_Cod_Prev,
                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "",
                             "",
                             objParametri_Server)

        If (DT.Rows.Count <= 0) Then
            Throw New Exception(String.Format("Impossibile trovare l'associazione tra l'elemento grafico {0} ed il tipo oggetto GIS {1}",
                                              LayerElementiGrafici_Cod,
                                              GIS_TipoOggetto_Cod_Prev)
                                              )
        End If

        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXTipoOggetto_W

        success = xWrite.Aggiorna(PivaSuperUser,
                                  LayerElementiGrafici_Cod,
                                  GIS_TipoOggetto_Cod,
                                  GIS_TipoOggetto_Cod_Prev,
                                  objParametri_Server)

        Return success

    End Function

    Public Function GetElencoLayerVisibiliPerTipologia_e_Utente(ByVal PivaSuperUser As String,
                                                               ByVal TipologiaLayer_Cod As Integer,
                                                               ByRef objParametri_Server As AgronicaCoreParametri) As LayerVisibiliUtenteTipologia
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.GetElencoLayerVisbiliPerTipologia_e_Utente()"

        Dim messaggioErrore As String = ""
        Dim ret As New LayerVisibiliUtenteTipologia
        Try
            Dim xLayerR As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R

            Dim dt = xLayerR.Leggi(PivaSuperUser, objParametri_Server.UtenteUsername, 0, TipologiaLayer_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            If dt.Rows.Count > 0 Then
                ret.PivaSuperUser = PivaSuperUser
                ret.Username = objParametri_Server.UtenteUsername
                ret.TipologiaLayer = TipologiaLayer_Cod
                ret.elencoLayers = New List(Of ElencoLayerVisibiliUtenteTipologia)
            End If
            For Each row In dt.Rows
                If row("Flag_Attivo") = 1 AndAlso row("Flag_Visibile") = 1 Then
                    ret.elencoLayers.Add(New ElencoLayerVisibiliUtenteTipologia() With {
                            .LayerElementiGrafici_Cod = row("LayerElementiGrafici_Cod"),
                            .LayerElementiGrafici_Des = row("LayerElementiGrafici_Des"),
                            .Flag_Attivo = row("Flag_Attivo"),
                            .Flag_Visbile = row("Flag_Visibile")
                     })
                End If
            Next


        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret

    End Function

    Public Function DisattivaAttributoLayer(ByVal PivaSuperUser As String,
                                                     ByVal UserName As String,
                                                     ByVal IdLayer As String,
                                                     ByVal TipoLayer As String,
                                                     ByVal ProgressivoDataStruct As String,
                                                     ByRef objParametri_Server As AgronicaCoreParametri,
                                                     ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.DisattivaAttributoLayer()"

        Dim messaggioErrore As String = ""

        Dim success As Boolean

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerTiles_R

        Dim IdLayerInt As Int32
        Dim TipoLayerInt As Int32
        Dim ProgressivoDataStructInt As Int32

        If Not Int32.TryParse(IdLayer, IdLayerInt) Then
            Throw New Exception("L'Identificativo del Layer deve essere un valore numerico")
        End If

        If Not Int32.TryParse(TipoLayer, TipoLayerInt) Then
            Throw New Exception("Il tipo del Layer deve essere un valore numerico")
        End If


        If Not Int32.TryParse(ProgressivoDataStruct, ProgressivoDataStructInt) Then
            Throw New Exception("Il progressivo dell'attributo deve essere un valore numerico")
        End If

        Dim DT = LeggiAttributoLayer(PivaSuperUser, ProgressivoDataStruct, objParametri_Server, objParametri_Utenti)

        DT = xRead.Leggi(PivaSuperUser,
                             IdLayerInt,
                             ProgressivoDataStructInt,
                             TipoLayerInt,
                             UserName,
                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "",
                             "",
                             objParametri_Server)

        If DT.Rows.Count = 0 Then
            Return True
        End If

        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerTiles_W


        success = xWrite.Elimina(PivaSuperUser,
                                 ProgressivoDataStructInt,
                                 UserName,
                                 TipoLayerInt,
                                 "",
                                 objParametri_Server)

        Return success

    End Function

    Public Function SalvaGUIDLayer(ByVal layerElementiGrafici_Cod As Integer,
                                   ByVal tipologiaLayer_cod As Integer,
                                   ByVal newGUIDLayer As String,
                                   ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_W

        resp = xWrite.SalvaGUIDLayer(layerElementiGrafici_Cod, tipologiaLayer_cod, newGUIDLayer, objParametri_Server)

        Return resp

    End Function

    Public Function SalvaGUIDStruct(ByVal tipologiaLayer_struct_cod As Integer,
                                    ByVal newGuidParam As String,
                                    ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W

        resp = xWrite.SalvaGUIDStruct(tipologiaLayer_struct_cod, newGuidParam, objParametri_Server)

        Return resp
    End Function

    Public Function AttivaAttributoLayer(ByVal PivaSuperUser As String,
                                         ByVal UserName As String,
                                         ByVal IdLayer As String,
                                         ByVal TipoLayer As String,
                                         ByVal ProgressivoDataStruct As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri,
                                         ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.AttivaAttributoLayer()"

        Dim messaggioErrore As String = ""

        Dim success As Boolean

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerTiles_R

        Dim IdLayerInt As Int32
        Dim TipoLayerInt As Int32
        Dim ProgressivoDataStructInt As Int32

        If Not Int32.TryParse(IdLayer, IdLayerInt) Then
            Throw New Exception("L'Identificativo del Layer deve essere un valore numerico")
        End If

        If Not Int32.TryParse(TipoLayer, TipoLayerInt) Then
            Throw New Exception("Il tipo del Layer deve essere un valore numerico")
        End If

        If Not Int32.TryParse(ProgressivoDataStruct, ProgressivoDataStructInt) Then
            Throw New Exception("Il progressivo dell'attributo deve essere un valore numerico")
        End If

        Dim DT = LeggiAttributoLayer(PivaSuperUser, ProgressivoDataStruct, objParametri_Server, objParametri_Utenti)

        Dim NomeAttributo = DT.Rows(0)("LayerElementiGrafici_Etichetta").ToString
        DT = xRead.Leggi(PivaSuperUser,
                             IdLayerInt,
                             ProgressivoDataStructInt,
                             TipoLayerInt,
                             UserName,
                             AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "",
                             "",
                             objParametri_Server)

        If DT.Rows.Count > 0 Then
            Return True
        End If

        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerTiles_W

        Dim DEFAULT_COLOR_RED As String = "ff0000"
        Dim DEFAULT_COLOR_BLACK As String = "000000"

        success = xWrite.Scrivi(PivaSuperUser,
                                IdLayerInt,
                                UserName,
                                ProgressivoDataStructInt,
                                TipoLayerInt,
                                NomeAttributo,
                                DEFAULT_COLOR_RED,
                                DEFAULT_COLOR_RED,
                                DEFAULT_COLOR_BLACK,
                                5,
                                AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
                                AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE,
                                objParametri_Server)


        Return success

    End Function

    Public Function ImpostaVisualizzazioneEtichettaLayer(ByVal PivaSuperUser As String,
                                          ByVal ProgressivoDataStruct As String,
                                          ByVal Impostazione As Boolean,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.ImpostaVisualizzazioneEtichettaLayer()"

        Dim messaggioErrore As String = ""

        Dim success As Boolean

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_R

        Dim DT = LeggiAttributoLayer(PivaSuperUser,
                                     ProgressivoDataStruct,
                                     objParametri_Server,
                                     objParametri_Utenti)

        Dim Etichetta = DT.Rows(0)("LayerElementiGrafici_Etichetta").ToString

        success = AggiornaAttributoLayer(PivaSuperUser,
                                         DT.Rows(0)("LayerElementiGrafici_Cod").ToString,
                                         Etichetta,
                                         DT.Rows(0)("LayerElementiGrafici_TipoDato").ToString,
                                         DT.Rows(0)("TipologiaLayer_cod"),
                                         -1,
                                         Impostazione,
                                         ProgressivoDataStruct,
                                         objParametri_Server,
                                         objParametri_Utenti)

        Return success

    End Function

    Public Function ImpostaCampoChiaveLayer(ByVal PivaSuperUser As String,
                                            ByVal ProgressivoDataStruct As String,
                                            ByVal CampoChiave As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim success As Boolean

        Dim DT = LeggiAttributoLayer(PivaSuperUser,
                                     ProgressivoDataStruct,
                                     objParametri_Server,
                                     objParametri_Utenti)

        success = AggiornaAttributoLayer(PivaSuperUser,
                                         DT.Rows(0)("LayerElementiGrafici_Cod").ToString,
                                         "",
                                         "",
                                         DT.Rows(0)("TipologiaLayer_cod"),
                                         Convert.ToInt32(CampoChiave),
                                         DT.Rows(0)("EtichettaVisibile"),
                                         ProgressivoDataStruct,
                                         objParametri_Server,
                                         objParametri_Utenti)

        Return success

    End Function

    Public Function LeggiAttributoLayer(ByVal PivaSuperUser As String,
                                          ByVal ProgressivoDataStruct As String,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri) As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_R

        Dim DT As DataTable = xRead.Leggi(PivaSuperUser,
                         0,
                         0,
                         Int32.Parse(ProgressivoDataStruct),
                         "",
                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                         "",
                         "",
                         objParametri_Server,
                         objParametri_Utenti)

        If DT.Rows.Count <> 1 Then
            Throw New Exception(String.Format("Impossibile recuperare le informazioni dell'attributo progressivo {0}", ProgressivoDataStruct))
        End If

        Return DT

    End Function

    Public Function EliminaAttributoLayer(ByVal PivaSuperUser As String,
                                          ByVal ProgressivoDataStruct As String,
                                          ByVal TipoLayer As Int32,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.EliminaAttributoLayer()"

        Dim messaggioErrore As String = ""

        Dim success As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W

        success = xWrite.EliminaAttributo(PivaSuperUser,
                                          ProgressivoDataStruct,
                                          TipoLayer,
                                          objParametri_Server,
                                          objParametri_Utenti)

        Return success
    End Function

    Public Function DeleteWholeLayerDataStruct(pivaSuperUser As String,
                                               layerCod As Int32,
                                               objParametriServer As AgronicaCoreParametri) As Boolean

        Dim objLayerDataStruct As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W
        Return objLayerDataStruct.DeleteWholeLayerDataStruct(pivaSuperUser, layerCod, objParametriServer)
    End Function

    Public Function AggiornaAttributoLayer(ByVal PivaSuperUser As String,
                                            ByVal IdLayer As String,
                                            ByVal Nome As String,
                                            ByVal TipoDato As String,
                                            ByVal TipoLayer As Int32,
                                            ByVal CampoChiave As Int32,
                                            ByVal etichettaVisibile As Nullable(Of Boolean),
                                            ByVal ProgressivoDataStruct As String,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.AggiornaAttributoLayer()"

        Dim messaggioErrore As String = ""

        Dim success As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W

        success = xWrite.AggiornaAttributo(PivaSuperUser,
                                            IdLayer,
                                            Nome,
                                            TipoDato,
                                            TipoLayer,
                                            CampoChiave,
                                           etichettaVisibile,
                                            ProgressivoDataStruct,
                                            objParametri_Server,
                                            objParametri_Utenti)

        Return success
    End Function

    Public Function SalvaAttributoLayer(ByVal PivaSuperUser As String,
                                            ByVal IdLayer As String,
                                            ByVal Nome As String,
                                            ByVal TipoDato As String,
                                            ByVal TipoLayer As Int32,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.SalvaAttributoLayer()"

        Dim messaggioErrore As String = ""

        Dim success As Boolean


        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_DataStruct_W

        success = xWrite.InserisciNuovoAttributo(PivaSuperUser,
                                        IdLayer,
                                        Nome,
                                        TipoDato,
                                        TipoLayer,
                                        objParametri_Server,
                                        objParametri_Utenti)

        Return success
    End Function

    Public Function GetElencoAttributiLayer(ByVal PivaSuperUser As String,
                                            ByVal UserName As String,
                                            ByVal idLayer As Int32,
                                            ByVal permessiWMS As Boolean,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Utenti As AgronicaCoreParametri) As LeggiImpostazioniAvanzateLayer
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.GetElencoAttributiLayer()"

        Dim messaggioErrore As String = ""

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R

        Dim ret As New LeggiImpostazioniAvanzateLayer
        ret.ListaAttributiLayer = New List(Of AttributoLayer)
        Try
            Dim dtRead = xRead.GetElencoAttributiLayer(PivaSuperUser,
                                                       UserName,
                                                       idLayer,
                                                       1,
                                                       permessiWMS,
                                                       objParametri_Server,
                                                       objParametri_Utenti)
            If Not (dtRead Is Nothing Or dtRead.Rows.Count <= 0) Then
                Dim keyLayer As String = ""
                Dim keyTile As String = ""
                Dim item As New AttributoLayer()

                Dim firstRow = dtRead.Rows(0)

                ret.Id = idLayer
                ret.NomeLayer = firstRow("LayerElementiGrafici_Des")
                ret.TipoGIS = firstRow("GIS_TipoOggetto_Cod")
                ret.Bloccato = firstRow("Bloccato")
                Dim tipologiaLayer_struct_cods = new HashSet(Of String)

                For Each row As DataRow In dtRead.Rows

                    If Not row.IsNull("TipologiaLayer_struct_cod") Then
                        tipologiaLayer_struct_cods.Add(row("TipologiaLayer_struct_cod"))
                        item = New AttributoLayer() With {
                                .NomeAttributo = row("LayerElementiGrafici_Etichetta"),
                                .ProgressivoDataStruct = row("TipologiaLayer_struct_cod"),
                                .TemaAttivo = row("Attivo"),
                                .TipoDato = row("LayerElementiGrafici_TipoDato"),
                                .CampoChiave = row("CampoChiave"),
                                .EtichettaVisibile = row("EtichettaVisibile"),
                                .Traduzioni = new List(of Gis_Traduzione)
                          }
                        ret.ListaAttributiLayer.Add(item)
                    End If
                Next

                If tipologiaLayer_struct_cods.Count > 0 Then
                    Dim dtTraduzioni = xRead.LeggiTraduzioniAttributoByStructCode(1, idLayer,
                                                                      Nothing,
                                                                      tipologiaLayer_struct_cods.ToArray(),
                                                                      objParametri_Server)
                    For Each row In dtTraduzioni.Rows
                        Dim attributo = ret.ListaAttributiLayer.FirstOrDefault(
                            Function(o) o.ProgressivoDataStruct = row("TipologiaLayer_struct_cod"))
                        If Not attributo Is Nothing
                            Dim traduzione = new Gis_Traduzione() With {
                                    .Lingua_Cod = row("Lingua_Cod"),
                                    .Lingua_Des = row("Descrizione"),
                                    .Traduzione = If (IsDbNull(row("LayerElementiGrafici_Etichetta")),
                                                      String.Empty,
                                                      row("LayerElementiGrafici_Etichetta"))
                                    }
                            attributo.Traduzioni.Add(traduzione)
                        End If
                    Next
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return ret
    End Function

    Public Function GetElencoTipoligieLayers(ByVal PivaSuperUser As String,
                                             ByVal TipologiaLayerCod As Integer,
                                             ByVal permessiWMS As Boolean,
                                             ByVal permessiDatiSatellitari As Boolean,
                                             ByVal permessiTecniciInCampo As Boolean,
                                             ByVal analisiProduttivita As Boolean,
                                             ByVal LeggiLayerNonVisibili As Boolean,
                                             ByVal LeggiLayerNonAttivi As Boolean,
                                             ByVal SementiSportelloCod As Integer,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri) As ElencoTipologieLayer
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.GetElencoTipoligieLayers()"

        Dim messaggioErrore As String = ""

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R
        Dim xReadDettagliTema As New AgronicaCoreGisBIZ.GIS_LayerTilesDescrizione_R

        'lavez - 16/01/2024 - collego i permessi di anagrafica agli appezzamenti\impianti
        Dim objUtentiPerm As New AgronicaCoreUtentiBIZ.Utenti_Permessi_R

        Dim utente_permessi_gis As AgronicaCoreDTOStd.InData.Gis.PermessiUtenteEntitaGIS = Nothing

        If SementiSportelloCod <= 0 Then
            utente_permessi_gis = objUtentiPerm.LeggiPemerssiUtenteEntitaGis(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
        End If

        Dim ret As New ElencoTipologieLayer
        ret.ListaTipologieLayer = New List(Of TipologiaLayer)
        Try
            Dim dtRead = xRead.GetElencoTipologiaLayers(PivaSuperUser, objParametri_Server.UtenteUsername, TipologiaLayerCod, permessiWMS, permessiDatiSatellitari, permessiTecniciInCampo, analisiProduttivita, False, False, "", objParametri_Server, objParametri_Utenti, LeggiLayerNonVisibili, LeggiLayerNonAttivi)
            If dtRead Is Nothing Then
                ret = Nothing
            Else
                Dim keyLayer As String = ""
                Dim keyTile As String = ""
                Dim LayerElementiGrafici_Cods = new HashSet(Of String)
                Dim item As New TipologiaLayer()

                For Each row In dtRead.Rows

                    If (row("TipologiaLayer_cod") & "|" & row("LayerElementiGrafici_Cod")) <> keyLayer Then
                        item = New TipologiaLayer() With {
                                .id = row("LayerElementiGrafici_Cod"),
                                .nome = IIf(row("LayerElementiGrafici_des") Is DBNull.Value, "", row("LayerElementiGrafici_des")),
                                .colore_1 = IIf(row("Colore_Primario_Layer") Is DBNull.Value, "", row("Colore_Primario_Layer")),
                                .colore_2 = IIf(row("Colore_Secondario_Layer") Is DBNull.Value, "", row("Colore_Secondario_Layer")),
                                .varianza = row("Varianza_Layer"),
                                .trasparenza = row("trasparenza"),
                                .MostraDescrizioneAssociata = IIf(row("MostraDescrizioneAssociata") Is DBNull.Value, "0", row("MostraDescrizioneAssociata")),
                                .zindex = row("zindex"),
                                .icona16 = IIf(row("icona16") Is DBNull.Value, "", row("icona16")),
                                .icona32 = IIf(row("icona32") Is DBNull.Value, "", row("icona32")),
                                .TipoNodoAlberoAnagrafe = row("TipoNodoAlberoAnagrafe"),
                                .flagvisibile = row("Flag_Visibile"),
                                .flagattivo = row("Flag_Attivo"),
                                .RaggruppaDescrizioneAssociata = row("RaggruppaDescrizioneAssociata"),
                                .FlagInserimento = GetPermesso(row("LayerElementiGrafici_Cod"), TipologiaLayerCod, SementiSportelloCod, IIf(row("Flag_Inserimento") Is DBNull.Value, "", row("Flag_Inserimento")), utente_permessi_gis),
                                .FlagModifica = GetPermesso(row("LayerElementiGrafici_Cod"), TipologiaLayerCod, SementiSportelloCod, IIf(row("Flag_Modifica") Is DBNull.Value, "", row("Flag_Modifica")), utente_permessi_gis),
                                .FlagCancellazione = GetPermesso(row("LayerElementiGrafici_Cod"), TipologiaLayerCod, SementiSportelloCod, IIf(row("Flag_Cancellazione") Is DBNull.Value, "", row("Flag_Cancellazione")), utente_permessi_gis),
                                .FlagInformazioni = IIf(row("Flag_Informazioni") Is DBNull.Value, "", row("Flag_Informazioni")),
                                .FlagAmministrazione = IIf(row("Flag_Amministrazione") Is DBNull.Value, "", row("Flag_Amministrazione")),
                                .FeatureTypeId = IIf(row("FeatureTypeId") Is DBNull.Value, "", row("FeatureTypeId")),
                                .tiles = New List(Of TipologiaTile),
                                .Traduzioni = New List(Of Gis_Traduzione)
                            }
                        ret.ListaTipologieLayer.Add(item)
                        keyLayer = row("TipologiaLayer_cod") & "|" & row("LayerElementiGrafici_Cod")
                        LayerElementiGrafici_Cods.Add(row("LayerElementiGrafici_Cod"))
                    End If
                    If row("LayerTiles_Cod") IsNot DBNull.Value Then
                        If (keyLayer & "|" & row("LayerTiles_Cod") <> keyTile) Then
                            Dim resp As rispostaStandard(Of List(Of TipologiaLabel))

                            resp = xReadDettagliTema.LeggiLayerTilesDescrizione(Convert.ToInt32(row("LayerTiles_Cod")),
                                                                                Convert.ToInt32(row("TipologiaLayer_cod")),
                                                                                Convert.ToInt32(row("LayerElementiGrafici_Cod")),
                                                                                0,
                                                                                objParametri_Server
                                                                                )
                            If Not resp.RispostaOK Then
                                Throw New Exception()
                            End If

                            item.tiles.Add(New TipologiaTile() With {
                                                .id = row("LayerTiles_Cod"),
                                                .nome = row("LayerTiles_Des"),
                                                .colore_primario = IIf(row("Colore_Primario_Tile") Is DBNull.Value, "", row("Colore_Primario_Tile")),
                                                .colore_secondario = IIf(row("Colore_Secondario_Tile") Is DBNull.Value, "", row("Colore_Secondario_Tile")),
                                                .varianza = row("Varianza_Tile"),
                                                .v_min = row("v_min"),
                                                .v_max = row("v_max"),
                                                .tilelayerpadre = row("tilelayerpadre"),
                                                .tilelabels = resp.RispostaStringa
                                           })
                            keyTile = keyLayer & "|" & row("LayerTiles_Cod")
                        End If
                    End If
                Next
                
                Dim dtTraduzioni = xRead.LeggiTraduzioni(TipologiaLayerCod,
                                                         LayerElementiGrafici_Cods.ToArray(), objParametri_Server)
                For Each row In dtTraduzioni.Rows
                    Dim layer =
                            ret.ListaTipologieLayer.FirstOrDefault(Function(o) o.id = row("LayerElementiGrafici_Cod"))
                    If Not layer Is Nothing
                        Dim traduzione = new Gis_Traduzione() With {
                                .Lingua_Cod = row("Lingua_Cod"),
                                .Lingua_Des = row("Descrizione"),
                                .Traduzione = If (IsDbNull(row("LayerElementiGrafici_Des")),
                                                  String.Empty,
                                                  row("LayerElementiGrafici_Des"))
                                }
                        layer.Traduzioni.Add(traduzione)
                    End If
                Next
            End If
        Catch ex As Exception
            ret = Nothing
        End Try
        Return ret
    End Function

    'lavez - 16/01/2024 - collego i permessi di anagrafica agli appezzamenti\impianti
    Private Function GetPermesso(ByVal LayerElementiGrafici_Cod As Integer,
                                 ByVal TipologiaLayer_Cod As Integer,
                                 ByVal SementiSportelloCod As Integer,
                                 ByVal Flag As String,
                                 ByVal Permessi As AgronicaCoreDTOStd.InData.Gis.PermessiUtenteEntitaGIS) As String
        If SementiSportelloCod > 0 Then
            ' Sotto sportello se la tipologia è "Imprese sementiere" il layer è sempre modificabile
            ' i permessi vengono poi gestiti direttamente sui poligoni
            Return IIf(TipologiaLayer_Cod = enum_TipologiaLayer.Organizzazione, "1", Flag)
        Else
            Select Case LayerElementiGrafici_Cod
                Case enum_Gis_LayerElementiGrafici_std.Centri_Aziendali
                    Return IIf(Permessi.centriAziendali.Scrittura, "1", "0")

                Case enum_Gis_LayerElementiGrafici_std.Fabbricati
                    Return IIf(Permessi.fabbricati.Scrittura, "1", "0")

                Case enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI
                    Return IIf(Permessi.appezzamenti.Scrittura, "1", "0")

                Case enum_Gis_LayerElementiGrafici_std.IMPIANTI
                    Return IIf(Permessi.impianti.Scrittura, "1", "0")

                Case enum_Gis_LayerElementiGrafici_std.CAMPI
                    Return IIf(Permessi.campi.Scrittura, "1", "0")

                Case Else
                    Return Flag
            End Select
        End If
    End Function

    Public Function ScriviNuovoLayerPersonalizzato(ScriviNuovoLayerPersonalizzatoInData As ScriviNuovoLayerPersonalizzatoInData,
                                                   objParametri_Server As AgronicaCoreParametri) As rispostaStandard(Of ScriviNuovoLayerPersonalizzatoOutData)

        Dim NomeRoutine As String = "ScriviNuovoLayerPersonalizzato"

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim rval As New rispostaStandard(Of ScriviNuovoLayerPersonalizzatoOutData)

        Try

            'Apro connessione DB
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(
                FlagConnessioneLocale,
                FlagTransazioneLocale,
                objParametri_Server
                )

            'Nuovo ID
            Dim ags As New Agro_Sequenze
            Dim LayerElementoGrafico_Cod As Integer = ags.NuovoId_Tabella(
                "GIS_LayerElementiGrafici_Personalizzati",
                1000000,
                2000000000,
                objParametri_Server
                )

            Dim trasparenza As Double = CostantiPersonalizzate.Layer_Trasparenza_Default
            Dim colore_primario As String = CostantiPersonalizzate.Layer_Colore_Primario_Default

            Select Case ScriviNuovoLayerPersonalizzatoInData.FeatureTypeId
                Case CInt(TipiEnumerativi.enum_GIS2012_TipoEntita.RASTER)
                    trasparenza = CostantiPersonalizzate.Layer_Trasparenza_Raster
                    colore_primario = CostantiPersonalizzate.Layer_Colore_Primario_Raster
            End Select

            'Scrittura
            Const tipologiaLayerStandard = 1
            Dim xScrivi As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W
            xScrivi.Scrivi(
                objParametri_Server.PivaSuperUser,
                objParametri_Server.UtenteUsername,
                LayerElementoGrafico_Cod,
                ScriviNuovoLayerPersonalizzatoInData.NomeLayer,
                1,
                1,
                "",
                "",
                colore_primario,
                "",
                1,
                trasparenza,
                ScriviNuovoLayerPersonalizzatoInData.MostraDescrizioneAssociata,
                tipologiaLayerStandard,
                LayerElementoGrafico_Cod,
                "x05_Appezzamento_16.png",
                "x05_Appezzamento_32.png",
                AGRODATAINIZIO,
                AGRODATAFINE,
                objParametri_Server)

            'Scrittura anagrafica
            Dim xScriviAnagrafica As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Anagrafica_W
            xScriviAnagrafica.Scrivi(
                objParametri_Server.PivaSuperUser,
                LayerElementoGrafico_Cod,
                tipologiaLayerStandard,
                ScriviNuovoLayerPersonalizzatoInData.NomeLayer,
                1,
                1,
                1,
                1,
                1,
                AGRODATAINIZIO,
                AGRODATAFINE,
                objParametri_Server)

            'Scrittura associazione tipo oggetto
            Dim tipoOggettoCod = ConvertiFeatureTypeIdInTipoOggetto(ScriviNuovoLayerPersonalizzatoInData.FeatureTypeId)
            Dim xScriviAssociazioneTipoOggetto As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXTipoOggetto_W
            xScriviAssociazioneTipoOggetto.Scrivi(
                objParametri_Server.PivaSuperUser,
                LayerElementoGrafico_Cod,
                tipoOggettoCod,
                AGRODATAINIZIO,
                AGRODATAFINE,
                objParametri_Server)

            'Scrittura dati utente
            Dim xScriviDatiUtente As New AgronicaCoreGisDAL.GIS_LayerElementiGraficiXUtente_W
            xScriviDatiUtente.Scrivi(
                LayerElementoGrafico_Cod,
                objParametri_Server.UtenteUsername,
                1,
                1,
                1,
                1,
                1,
                If(objParametri_Server.UtenteUsername = objParametri_Server.SuperUserUsername, 1, 0),
                AGRODATAINIZIO,
                AGRODATAFINE,
                objParametri_Server
                )

            ' super user always have permissions over every layer
            If objParametri_Server.UtenteUsername <> objParametri_Server.SuperUserUsername Then
                xScrivi.Scrivi(
                    objParametri_Server.PivaSuperUser,
                    objParametri_Server.SuperUserUsername,
                    LayerElementoGrafico_Cod,
                    ScriviNuovoLayerPersonalizzatoInData.NomeLayer,
                    1,
                    1,
                    "",
                    "",
                    colore_primario,
                    "",
                    1,
                    trasparenza,
                    ScriviNuovoLayerPersonalizzatoInData.MostraDescrizioneAssociata,
                    tipologiaLayerStandard,
                    LayerElementoGrafico_Cod,
                    "x05_Appezzamento_16.png",
                    "x05_Appezzamento_32.png",
                    AGRODATAINIZIO,
                    AGRODATAFINE,
                    objParametri_Server
                    )

                xScriviDatiUtente.Scrivi(
                    LayerElementoGrafico_Cod,
                    objParametri_Server.SuperUserUsername,
                    1,
                    1,
                    1,
                    1,
                    1,
                    1,
                    AGRODATAINIZIO,
                    AGRODATAFINE,
                    objParametri_Server
                    )
            End If


            'Chiudo transazione DB
            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            rval.RispostaOK = True
            rval.RispostaStringa = New ScriviNuovoLayerPersonalizzatoOutData With {.LayerElementiGrafici_Cod = LayerElementoGrafico_Cod}

        Catch ex As Exception

            'Rollback transazione DB
            If Not objParametri_Server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            Dim messaggioErrore = AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True)

            Dim Messaggio As String = ""
            If messaggioErrore <> "" Then
                Messaggio += "ERR: Sono stati rilevati i seguenti errori : " & vbCrLf
                Messaggio += "" & vbCrLf
                Messaggio += messaggioErrore
                Messaggio += "" & vbCrLf
                Messaggio += "Ritentare il salvataggio dopo la correzione..."
            End If

            rval.RispostaOK = False
            rval.Errore = Messaggio

        Finally
            'Chiudo connessione DB
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return rval
    End Function

    Public Function DeleteLayer(ByVal PivaSuperUser As String,
                                ByVal Utente As String,
                                ByVal LayerElementiGrafici_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreGisBIZ.GIS_LayerElementiGrafici.DeleteLayer()"
        Dim objElementiGrafici As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W

        Return objElementiGrafici.Cancella(PivaSuperUser, Utente, LayerElementiGrafici_Cod, xFiltroAggiuntivo, objParametri)
    End Function

    Private Function ConvertiFeatureTypeIdInTipoOggetto(featureTypeId As Integer) As Integer

        Dim tipoOggettoCod As Integer

        Select Case featureTypeId

            Case FeatureType.Point
                tipoOggettoCod = TipiEnumerativi.enum_Gis_TipoOggetto.Punto

            Case FeatureType.LineString
                tipoOggettoCod = TipiEnumerativi.enum_Gis_TipoOggetto.LineString

            Case FeatureType.Polygon
                tipoOggettoCod = TipiEnumerativi.enum_Gis_TipoOggetto.Poligono

            Case FeatureType.Raster
                tipoOggettoCod = TipiEnumerativi.enum_Gis_TipoOggetto.Raster

            Case Else
                Throw New Exception(String.Format("FeatureTypeId {0} non gestito", featureTypeId))

        End Select

        Return tipoOggettoCod

    End Function

    Public Class ScriviNuovoLayerPersonalizzatoInData

        Public Property NomeLayer() As String
        Public Property MostraDescrizioneAssociata As String
        Public Property FeatureTypeId As Integer

    End Class

    Public Class ScriviNuovoLayerPersonalizzatoOutData

        Public Property LayerElementiGrafici_Cod As Integer

    End Class

    Public Function LeggiElencoAllegatiLayer(ByVal layerElementiGrafici_Cod As Int32,
                                             ByVal tipologiaLayer_Cod As Int32,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_Utenti As AgronicaCoreParametri) As ElencoAllegatiLayer_Out

        Dim xRead As New AgronicaCoreGisDAL.LayerElementiGrafici_Allegati_R

        Dim resp As New ElencoAllegatiLayer_Out With {
            .elencoAllegatiLayer = New List(Of AllegatoLayer)
        }

        Dim DT As DataTable = xRead.LeggiElencoAllegatiLayer(layerElementiGrafici_Cod, tipologiaLayer_Cod, objParametri_Server, objParametri_Utenti)

        If DT Is Nothing Then
            Throw New Exception("Errore nella lettura degli allegati del layer.")
        End If

        For Each row In DT.Rows
            Dim allegato As New AllegatoLayer With {
                .allegati_Documenti_Cod = CInt(row("Allegati_Documenti_Cod")),
                .descrizione = row("Allegati_Documenti_Des").ToString,
                .data_estrazione = CDate(row("Data_Creazione")),
                .formato = "Shapefile Esri",
                .inizio_validita = CDate(row("Validita_Inizio")),
                .fine_validita = CDate(row("Validita_Fine")),
                .Utente_Esportazione = String.Format("{0} {1}", row("NomeUtenteEstrazione").ToString, row("CognomeUtenteEstrazione").ToString).Trim
            }

            resp.elencoAllegatiLayer.Add(allegato)
        Next

        Return resp

    End Function

    Public Function ModificaEstrazioneShape(ByVal allegato As AllegatoLayerModifica,
                                            ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim xWrite As New AgronicaCoreGisDAL.LayerElementiGrafici_Allegati_W

        Dim resp As Boolean

        resp = xWrite.ModificaEstrazioneShape(allegato.allegati_Documenti_Cod,
                                              allegato.descrizione,
                                              allegato.inizio_validita,
                                              allegato.fine_validita,
                                              objParametri_Server)

        If Not resp Then
            Throw New Exception("Errore nella modifica dei dati dell'estrazione shape.")
        End If

        Return resp
    End Function

    Public Function LeggiAllegatoFile(ByVal allegato_cod As Int32,
                                      ByRef objParametri_Server As AgronicaCoreParametri,
                                      ByRef objParametri_Utenti As AgronicaCoreParametri) As AllegatoFile
        Dim xRead As New AgronicaCoreGisDAL.LayerElementiGrafici_Allegati_R

        Dim DT As DataTable = xRead.LeggiAllegatoFile(allegato_cod, objParametri_Server, objParametri_Utenti)

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Throw New Exception("Allegato inesistente")
        End If

        Dim row = DT.Rows(0)

        Dim resp As New AllegatoFile With {
            .allegati_Documenti_Cod = CInt(row("Allegati_Documenti_Cod")),
            .fileName = row("Allegati_Documenti_NomeFile").ToString
        }

        If row("File_Allegato_DB").Length = 0 Then
            Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
            Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")

            Dim basePath = System.IO.Path.Combine(Percorso, System.IO.Path.GetFileNameWithoutExtension(resp.fileName))
            Dim fileNameFullPath = System.IO.Path.Combine(basePath, resp.fileName)

            Dim fileByteArray As Byte()
            fileByteArray = My.Computer.FileSystem.ReadAllBytes(fileNameFullPath)

            If fileByteArray Is Nothing OrElse fileByteArray.Length = 0 Then
                Throw New Exception("Impossibile leggere il contenuto del file.")
            End If

            resp.file_Allegato_DB = fileByteArray
        Else
            resp.file_Allegato_DB = CType(row("File_Allegato_DB"), Byte())
        End If

        Return resp
    End Function

    Public Function leggiDatiBaseLayer(ByVal layerElementiGrafici_cod As Int32,
                                       ByVal tipologiaLayer_cod As Int32,
                                       ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim DT As DataTable

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R

        DT = xRead.Leggi(objParametri.PivaSuperUser,
                         objParametri.UtenteUsername,
                         layerElementiGrafici_cod,
                         tipologiaLayer_cod,
                         AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                         "",
                         "",
                         objParametri)

        Return DT

    End Function

    Public Function EliminazioneTotaleDatiLayer(ByVal layerElementiGrafici_cod As Int32,
                                                objParametri As AgronicaCoreParametri) As RispostaStandard
        Dim DT As DataTable
        Dim xRead As New AgronicaCoreGisDAL.GIS_ElementiGrafici_R
        Dim resp As Boolean
        Dim rval As New RispostaStandard

        DT = xRead.LeggiDatiLayer(layerElementiGrafici_cod, objParametri)

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            rval.RispostaOK = True
            Return rval
        End If

        If DT.Select("Entita_GUID IS NOT NULL").Length > 0 Then
            rval.RispostaOK = False
            rval.Errore = "Non è possibile procedere, ci sono elementi già utilizzati."
            Return rval
        End If

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try
            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                           FlagTransazioneLocale,
                                                           objParametri)

            Dim xWrite As New AgronicaCoreGisDAL.GIS_ElementiGrafici_W

            resp = xWrite.EliminaElementiGraficiLayer(layerElementiGrafici_cod, objParametri)

            If Not resp Then
                Throw New Exception("Errore nell'eliminazione degli elementi grafici del layer.")
            End If

            For Each row In DT.Rows
                resp = xWrite.EliminaEntitaLayer(CInt(row("Entita_Cod")), objParametri)
            Next

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri)

            rval.RispostaOK = True

        Catch ex As Exception

            If Not objParametri.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri)
            End If

            rval.RispostaOK = False
            rval.Errore = ex.Message
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri)
        End Try

        Return rval
    End Function

    Public Function SalvaPaletteSLDDaXML(ByVal layerElementiGrafici_Cod As Int32,
                                         ByVal tipologiaLayer_Cod As Int32,
                                         ByVal filePalette() As Byte,
                                         ByRef objParametri_Server As AgronicaCoreParametri) As Boolean
        Dim resp As Boolean = False

        Dim xWrite As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_W

        Dim paletteObj As New JObject

        Dim fileContent As String = System.Text.Encoding.UTF8.GetString(filePalette)

        Dim paletteXML = XDocument.Parse(fileContent)

        If paletteXML.Descendants("{http://www.opengis.net/sld}ColorMap").Any Then

            Dim paletteString = paletteXML.Descendants("{http://www.opengis.net/sld}ColorMap").First.ToString.Replace("sld:", "")

            paletteString = String.Format("<RasterSymbolizer>{0}</RasterSymbolizer>", paletteString)

            paletteObj.Add("vizParams", Nothing)
            paletteObj.Add("SLDPalette", paletteString)

        Else
            Throw New Exception("Nel file non è presente una colorMap.")
        End If

        resp = xWrite.SalvaPaletteSLD(layerElementiGrafici_Cod, tipologiaLayer_Cod, paletteObj, objParametri_Server)

        Return resp
    End Function

    Public Function VerificaEsistenzaPalette(ByVal layerElementiGrafici_Cod As Int32,
                                             ByVal tipologiaLayer_Cod As Int32,
                                             ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim resp As Boolean

        Dim xRead As New AgronicaCoreGisDAL.GIS_LayerElementiGrafici_R

        resp = xRead.VerificaEsistenzaPalette(layerElementiGrafici_Cod, tipologiaLayer_Cod, objParametri_Server)

        Return resp
    End Function
End Class
