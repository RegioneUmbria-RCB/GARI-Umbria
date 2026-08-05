Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDTOStd.InData.Agea
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Reflection
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.exceptions

Public Class AttivitaToAgea

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Const ORGANIC_FERTILIZER_TAXONOMY_ID As Integer = 1

    Public Const CHEMICAL_FERTILIZER_TAXONOMY_ID As Integer = 43

    Private ObjParametri_Server As AgronicaCoreParametri = Nothing

    Private ObjParametri_Utenti As AgronicaCoreParametri = Nothing

    Private ObjParametri_Super_Server As AgronicaCoreParametri = Nothing


    Private Class Obj_Qta_Prodotto

        Private _waterQuantity As Decimal
        Private _productQuantity As Decimal
        Private _actionArea As Decimal

        Public Property waterQuantity() As Decimal
            Get
                Return _waterQuantity
            End Get
            Set(ByVal value As Decimal)
                _waterQuantity = value
            End Set
        End Property

        Public Property productQuantity() As Decimal
            Get
                Return _productQuantity
            End Get
            Set(ByVal value As Decimal)
                _productQuantity = value
            End Set
        End Property

        ''' <summary>
        ''' Superficie trattata (nel caso in cui ho gli impianti) oppure
        ''' Quantita di proddotto trattata (nel caso in cui non ho gli impianti ma tratto un prodotto a mgazzino come per esempio nei Trattamenti Post Raccolta e nella Concia del Seme)
        ''' </summary>
        ''' <returns></returns>

        Public Property actionArea() As Decimal
            Get
                Return _actionArea
            End Get
            Set(ByVal value As Decimal)
                _actionArea = value
            End Set
        End Property

    End Class


    Private tzh As New JsonSerializerSettings With {
        .DateFormatString = "yyyy-MM-ddT00:00:00Z",
        .Converters = New List(Of JsonConverter) From {New DecimalRoundingConverter()}
    }

    Private DT_Lavorazioni As DataTable = Nothing

    Private DT_Appezzamenti_Impianti As DataTable = Nothing

    Private DT_UdM As DataTable = Nothing

    Private DT_Avversita As DataTable = Nothing

    Private DT_Macchine As DataTable = Nothing

    Private DT_Modalita_Applicazione_Globali As DataTable = Nothing

    Private List_Qta As List(Of Qta_Prodotto_X_Impianto) = Nothing

    Private Sistema_Cod_Agea As Integer = 5

    Private Validita_Inizio As Date = AGRODATAINIZIO

    Private Validita_Fine As Date = AGRODATAFINE

    ''' <summary>
    ''' Se impostato a True i campi productQuantity e waterQuantity vengono popolati con Dose/Ha e Quantità Ha visti sul Gias
    ''' </summary>
    Private ProductAndWaterQuantity_HA As Boolean = True

    Private Dict_Superficie_Trattata_X_Operazione As New Dictionary(Of String, Decimal)

    Private Dict_Qta_Prodotto_Trattata_X_Operazione As New Dictionary(Of String, Decimal)

    Private tp_Operatori As Tuple(Of List(Of RisorsaPersona), List(Of WorkerElement)) = Nothing

    Private tp_Macchine As Tuple(Of List(Of RisorsaMacchina), List(Of EquipmentElement)) = Nothing

    Private checkBundle As New CheckBundle

    Private ObjUtility As New Utility


    Public Function SendAttivitaToAgea(ByVal Piva As String, ByVal CUAA As String, ByVal Anno As Integer,
                                       ByVal Rag_Soc As String, ByVal objParametri_Str As CoreWS_GenericObjP,
                                       ByRef List_ErroriGias As List(Of ErroreGias)) As String

        Dim bundle_Id As String = String.Empty
        Dim bundle As New Bundle

        Try
            ObjParametri_Super_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Str.objP_super_server)
            ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Str.objP_server)
            ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Str.objP_utenti)

            bundle.cuaa = CUAA
            bundle.campaignYear = Anno
            bundle.creationUser = ObjParametri_Utenti.UtenteUsername
            bundle.farmDescription = Rag_Soc
            bundle.data = Nothing

            Dim str_bundle As String = JsonConvert.SerializeObject(bundle, tzh)
            Dim endpoint As String = "/api/bundle"
            bundle_Id = ObjUtility.CallHubAgeaPOST(str_bundle, endpoint, ObjParametri_Super_Server)

        Catch ex As Exception

            Dim severity As String = AgronicaCoreDemetraBIZ.Util_Costanti.ESITO_KO
            Dim messageError As String = ""

            If ex.GetType() Is GetType(InfoExceptionGias2AgeaHub) Then
                severity = AgronicaCoreDemetraBIZ.Util_Costanti.ESITO_OK
            End If
            ObjUtility.WriteLogToElasticSearch(CUAA, bundle, ex.Message, severity, ObjParametri_Server, ObjParametri_Super_Server)

            Throw New Exception(ex.Message)
        Finally
            List_ErroriGias = checkBundle.List_ErroriGias
        End Try

        Return bundle_Id
    End Function


    Public Function ExtractQdCToAgea(ByVal Piva As String, ByVal CUAA As String, ByVal Anno As Integer, ByVal Rag_Soc As String,
                                     ByVal objParametri_Str As CoreWS_GenericObjP, ByRef erroriDaBypassare As List(Of String),
                                     ByRef List_ErroriGias As List(Of ErroreGias)) As Bundle

        Dim bundle_Id As String = String.Empty

        Dim bundle As New Bundle

        Try
            checkBundle.List_ErroriGias = List_ErroriGias

            checkBundle.erroriDaBypassare = erroriDaBypassare

            ObjParametri_Super_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Str.objP_super_server)

            ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Str.objP_server)

            ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Str.objP_utenti)

            Validita_Inizio = New Date(Anno, 1, 1)

            Validita_Fine = New Date(Anno, 12, 31)


            If String.IsNullOrEmpty(CUAA) Then
                Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                CUAA = xImpCodR.Leggi_CUAA(Piva, ObjParametri_Server)
            End If

            If (String.IsNullOrEmpty(Piva) OrElse String.IsNullOrEmpty(Rag_Soc)) Then
                Dim objanag As New AgronicaCoreAnagrafeDAL.Imprese_Read
                If String.IsNullOrEmpty(Piva) Then
                    Piva = objanag.Piva_From_CUAA(CUAA, ObjParametri_Server)
                End If

                If String.IsNullOrEmpty(Rag_Soc) Then
                    Rag_Soc = objanag.RagSoc_from_Piva(Piva, ObjParametri_Server)
                End If
            End If

            Dim objAnagrafeDAL As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            DT_Appezzamenti_Impianti = objAnagrafeDAL.Leggi_Join_Appezzamento(Piva, Validita_Inizio, Validita_Fine, "", "", ObjParametri_Server)

            bundle = MapBundle(GetDictAttivita(Piva), Piva, CUAA, Anno, Rag_Soc)

        Catch ex As Exception

            Dim severity As String = AgronicaCoreDemetraBIZ.Util_Costanti.ESITO_KO

            Dim messageError As String = ""

            If ex.GetType() Is GetType(InfoExceptionGias2AgeaHub) Then
                severity = AgronicaCoreDemetraBIZ.Util_Costanti.ESITO_OK
            End If

            ObjUtility.WriteLogToElasticSearch(CUAA, bundle, ex.Message, severity, ObjParametri_Server, ObjParametri_Super_Server)

            Throw New Exception(ex.Message)
        Finally
            List_ErroriGias = checkBundle.List_ErroriGias
            erroriDaBypassare = checkBundle.erroriDaBypassare
        End Try

        Return bundle
    End Function



    Public Function MapAttivitatoAgea(ByVal Piva As String, ByVal CUAA As String, ByVal Anno As Integer, ByVal Rag_Soc As String,
                                      ByVal objParametri_Str As CoreWS_GenericObjP, ByVal erroriDaBypassare As List(Of String), ByRef List_ErroriGias As List(Of ErroreGias)) As String

        Dim bundle_Id As String = String.Empty

        Dim bundle As New Bundle

        Try
            checkBundle.List_ErroriGias = List_ErroriGias

            checkBundle.erroriDaBypassare = erroriDaBypassare

            ObjParametri_Super_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Str.objP_super_server)

            ObjParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Str.objP_server)

            ObjParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Str.objP_utenti)

            Validita_Inizio = New Date(Anno, 1, 1)

            Validita_Fine = New Date(Anno, 12, 31)

            If String.IsNullOrEmpty(CUAA) Then
                Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                CUAA = xImpCodR.Leggi_CUAA(Piva, ObjParametri_Server)
            End If

            Dim objAnagrafeDAL As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            DT_Appezzamenti_Impianti = objAnagrafeDAL.Leggi_Join_Appezzamento(Piva, Validita_Inizio, Validita_Fine, "", "", ObjParametri_Server)

            bundle = MapBundle(GetDictAttivita(Piva), Piva, CUAA, Anno, Rag_Soc)

            bundle_Id = ExportBundle(bundle)

        Catch ex As Exception

            Dim severity As String = AgronicaCoreDemetraBIZ.Util_Costanti.ESITO_KO

            Dim messageError As String = ""

            If ex.GetType() Is GetType(InfoExceptionGias2AgeaHub) Then
                severity = AgronicaCoreDemetraBIZ.Util_Costanti.ESITO_OK
            End If

            Dim utility As New Utility

            utility.WriteLogToElasticSearch(CUAA, bundle, ex.Message, severity, ObjParametri_Server, ObjParametri_Super_Server)

            Throw New Exception(ex.Message)
        Finally
            List_ErroriGias = checkBundle.List_ErroriGias
        End Try

        Return bundle_Id
    End Function

    Private Function MapBundle(ByVal Dict_Attivita As Dictionary(Of String, Attivita), ByVal Piva As String, ByVal CUAA As String, ByVal Anno As Integer, ByVal Rag_Soc As String) As Bundle

        Dim bundle As New Bundle()

        bundle.cuaa = CUAA

        bundle.campaignYear = Anno

        bundle.creationUser = ObjParametri_Utenti.UtenteUsername

        bundle.farmDescription = Rag_Soc

        Dim supply = MapSupply(Dict_Attivita, Piva, Anno, Rag_Soc)

        If Not IsNothing(supply) Then
            bundle.data = JsonConvert.SerializeObject(supply, tzh)
        Else
            bundle.data = ""
        End If

        Return bundle

    End Function

    Private Function MapSupply(ByVal Dict_Attivita As Dictionary(Of String, Attivita), ByVal Piva As String, ByVal Anno As Integer, ByVal Rag_Soc As String) As Supply

        Dim supply As New Supply()

        supply.farmDescription = Rag_Soc

        supply.countryNotebookName = String.Format(My.Resources.AgronicaCoreAgeaBIZ.QDCAAnnoData, Anno, Date.Now.ToShortDateString())

        supply.countryNotebookDesc = String.Format(My.Resources.AgronicaCoreAgeaBIZ.QDCAAnnoData, Anno, Date.Now.ToShortDateString())

        supply.campaingYear = Anno

        checkBundle.CheckYear(Anno)

        If checkBundle.ContinueExport() Then
            checkBundle.CheckOperation(Dict_Attivita, Rag_Soc, Anno)
        End If


        If checkBundle.ContinueExport() Then

            Dim DT_Validita_Rilievi_Fasi_Fenologiche As DataTable = Nothing

            Dim IDTestataTemp_Rilievi_FF As Integer = 0

            Dim tp_plots_Rilievi_FF As New List(Of Tuple(Of DataRow, Plot, Impianto.PK, Decimal))

            Dim List_Distinct_Impianti As New List(Of Impianto.PK)

            Dim List_Lav_Cod_With_Adversity = New List(Of Integer) From {LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                                        LAVCOD_DISERBO,
                                        LAVCOD_DISSECCAMENTO,
                                        LAVCOD_GEODISINFESTAZIONE,
                                        LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                                        LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                                        LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                                        LAVCOD_CONCIA_SEME}

            Dim List_Lav_Cod_With_Equipment = New List(Of Integer) From {LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                                        LAVCOD_DISERBO,
                                        LAVCOD_DISSECCAMENTO,
                                        LAVCOD_GEODISINFESTAZIONE,
                                        LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                                        LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                                        LAVCOD_TRATTAMENTO_POST_RACCOLTA}


            Dim tp_Warehouses As New List(Of Tuple(Of Warehouse, FabbricatoLight))

            supply.warehouses.AddRange(MapWarehouses(Piva, tp_Warehouses, ObjParametri_Server, ObjParametri_Utenti))

            For index As Integer = 0 To Dict_Attivita.Values.Count - 1

                Dim attivita As Attivita = Dict_Attivita.Values(index)

                Dim operationId As String = Dict_Attivita.Keys(index)

                Dim operationDesLib As String = attivita.descrizione + String.Format(" - {0}", attivita.inizio.ToShortDateString())

                Dim Id_Agenda As Integer = CInt(attivita.codice)

                Dim Lav_Cod = attivita.job.primaryKey.codice

                If IsNothing(DT_Avversita) AndAlso List_Lav_Cod_With_Adversity.Contains(Lav_Cod) Then
                    Dim objMetaschemaDAL As New AgronicaCoreMetaSchemaDAL.AvversitaxGruppoAvversita_R

                    DT_Avversita = objMetaschemaDAL.LeggiDaAangrafiche(0, 0, AGRODATAINIZIO, AGRODATAFINE, "", "", ObjParametri_Server)
                End If

                If IsNothing(DT_Macchine) AndAlso List_Lav_Cod_With_Equipment.Contains(Lav_Cod) Then
                    Dim objContabDAL As New AgronicaCoreContabDAL.Parco_Macchine_R

                    DT_Macchine = objContabDAL.Leggi("", 0, True, "", "", "", "", "", 0,
                                                                     "", False, 0, "", True, AGRODATAINIZIO,
                                                                     AGRODATAFINE, "", "", ObjParametri_Server)
                End If

                Dim tp_plots As New List(Of Tuple(Of DataRow, Plot, Impianto.PK, Decimal))

                If Not IsNothing(attivita.centriDiCosto) AndAlso attivita.centriDiCosto.Count > 0 Then

                    Dim Superficie_Trattata_Totale As Decimal = 0

                    Dim Qta_Prodotto_Trattata_Totale As Decimal = 0

                    For Each centroCdC In attivita.centriDiCosto
                        If centroCdC.classType = ClassType.EsercizioCDC Then

                            Dim esercizioCdC As EsercizioCDC = CType(centroCdC, EsercizioCDC)

                            Dim ImpiantoPK As Impianto.PK = esercizioCdC.esercizio.impiantoPK

                            Dim Dr As DataRow = ObjUtility.FindImpianto(ImpiantoPK, DT_Appezzamenti_Impianti)

                            If Not checkBundle.SkipOperationOutOfScope(attivita.inizio, operationDesLib, Dr) Then

                                Dim superficieTrattataImpianto As Decimal = esercizioCdC.superficieTrattata

                                Dim plot = MapPlot(ImpiantoPK, Dr)

                                Dim tp_plot = Tuple.Create(Dr, plot, ImpiantoPK, superficieTrattataImpianto)

                                tp_plots.Add(tp_plot)

                                If Lav_Cod = LAVCOD_FASI_FENOLOGICHE Then
                                    tp_plots_Rilievi_FF.Add(tp_plot)
                                End If

                                If List_Distinct_Impianti.FindIndex(Function(imp) imp.codice = ImpiantoPK.codice AndAlso
                                                                                imp.appezzamentoPK.codice = ImpiantoPK.appezzamentoPK.codice AndAlso
                                                                                 imp.appezzamentoPK.centroAziendalePK.codice = ImpiantoPK.appezzamentoPK.centroAziendalePK.codice AndAlso
                                                                                 imp.appezzamentoPK.centroAziendalePK.partitaIva = ImpiantoPK.appezzamentoPK.centroAziendalePK.partitaIva) = -1 Then

                                    List_Distinct_Impianti.Add(ImpiantoPK)

                                    checkBundle.CheckPlot(plot, esercizioCdC)

                                End If

                                Superficie_Trattata_Totale += esercizioCdC.superficieTrattata

                            End If

                        ElseIf centroCdC.classType = ClassType.ProdottoDaTrattareCDC Then

                            Select Case Lav_Cod
                                Case LAVCOD_TRATTAMENTO_POST_RACCOLTA, LAVCOD_CONCIA_SEME
                                    Dim prodottoDaTrattareCdC As ProdottoDaTrattareCDC = CType(centroCdC, ProdottoDaTrattareCDC)

                                    If Not IsNothing(prodottoDaTrattareCdC) AndAlso Not IsNothing(prodottoDaTrattareCdC.giacenzaMagazzino) AndAlso tp_Warehouses.Count > 0 Then

                                        If Qta_Prodotto_Trattata_Totale = 0 Then
                                            Qta_Prodotto_Trattata_Totale = (From CdC In attivita.centriDiCosto
                                                                            Where CdC.classType = ClassType.ProdottoDaTrattareCDC
                                                                            Select CType(CdC, ProdottoDaTrattareCDC).qtaTrattata).Distinct().Sum()
                                        End If

                                        Dim tp_Warehouse = tp_Warehouses.Find(Function(t) t.Item2.primaryKey.centroAziendalePK.partitaIva = prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.partitaIva AndAlso
                                                                                 t.Item2.primaryKey.centroAziendalePK.codice = prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.centroAziendalePK.codice AndAlso
                                                                                 t.Item2.primaryKey.codice = prodottoDaTrattareCdC.giacenzaMagazzino.Magazzino.primaryKey.codice)

                                        If Not IsNothing(tp_Warehouse) Then

                                            If Lav_Cod = LAVCOD_TRATTAMENTO_POST_RACCOLTA Then
                                                supply.productTreatments.AddRange(MapProductTreatments(operationId, tp_Warehouse.Item1, prodottoDaTrattareCdC, Anno, attivita.inizio,
                                                                                                       attivita.oraInizio, Qta_Prodotto_Trattata_Totale, attivita.risorse))
                                            ElseIf Lav_Cod = LAVCOD_CONCIA_SEME Then
                                                supply.seedTreatments.AddRange(MapSeedTreatments(operationId, tp_Warehouse.Item1, prodottoDaTrattareCdC, Anno, attivita.inizio, attivita.oraInizio,
                                                                                                 Qta_Prodotto_Trattata_Totale, attivita.risorse))
                                            End If
                                        End If
                                    End If
                            End Select
                        End If
                    Next

                    If Not String.IsNullOrEmpty(operationId) Then
                        If Superficie_Trattata_Totale > 0 Then
                            Dict_Superficie_Trattata_X_Operazione.Add(operationId, Superficie_Trattata_Totale)
                        ElseIf Qta_Prodotto_Trattata_Totale > 0 Then
                            Dict_Qta_Prodotto_Trattata_X_Operazione.Add(operationId, Qta_Prodotto_Trattata_Totale)
                        End If
                    End If

                End If



                If checkBundle.ContinueExport() Then

                    For Each tp_plot In tp_plots

                        If Not IsNothing(DT_Lavorazioni) AndAlso DT_Lavorazioni.Select("Lav_Cod = " & Lav_Cod).Length > 0 Then
                            supply.farmingEvents.Add(MapFarmingEvent(operationId, operationDesLib, Lav_Cod, tp_plot.Item2, tp_plot.Item1, attivita.inizio))
                        Else

                            If Not IsNothing(attivita.risorse) AndAlso attivita.risorse.Count > 0 Then

                                If IsNothing(List_Qta) AndAlso Not ProductAndWaterQuantity_HA Then
                                    Dim objAgronicaCoreContabBIZ As New Movimenti_R

                                    List_Qta = objAgronicaCoreContabBIZ.List_Qta_Prodotto_X_Impianti(Piva, 0, Validita_Inizio, Validita_Fine, ObjParametri_Server)
                                End If

                                Select Case Lav_Cod
                                    Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                                        LAVCOD_DISERBO,
                                        LAVCOD_DISSECCAMENTO,
                                        LAVCOD_GEODISINFESTAZIONE,
                                        LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                                        LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE

                                        supply.phytochemicalTreatments.AddRange(MapPhytochemicalTreatments(operationId, operationDesLib, tp_plot.Item2, tp_plot.Item1, tp_plot.Item3, tp_plot.Item4,
                                                                                                           Anno, attivita.inizio, attivita.oraInizio, attivita.risorse, attivita.modalitaApplicazione))


                                    Case LAVCOD_IRRIGAZIONE
                                        supply.irrigations.AddRange(MapIrrigations(operationId, Piva, tp_plot.Item2, tp_plot.Item1, attivita.inizio, attivita.oraInizio, attivita.risorse))

                                    Case LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_FERTIRRIGAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA
                                        supply.chemicalFertilizations.AddRange(MapChemicalFertilizations(operationId, operationDesLib, tp_plot.Item2, tp_plot.Item1, tp_plot.Item3, tp_plot.Item4,
                                                                                                         attivita.inizio, attivita.oraInizio, attivita.risorse, attivita.modalitaApplicazione))

                                    Case LAVCOD_DISTRIBUZIONE_AMMENDANTI
                                        supply.organicFertilizations.AddRange(MapOrganicFertilizations(operationId, operationDesLib, tp_plot.Item2, tp_plot.Item1, tp_plot.Item3, tp_plot.Item4,
                                                                                                       attivita.inizio, attivita.oraInizio, attivita.risorse, attivita.modalitaApplicazione))
                                End Select
                            End If


                        End If

                    Next


                    If Lav_Cod = LAVCOD_FASI_FENOLOGICHE Then
                        Dim obj__Tmp_Agenda As New AgronicaCoreVarieDAL.__tmp_Agenda_W


                        obj__Tmp_Agenda.Scrivi(IDTestataTemp_Rilievi_FF, Piva, Id_Agenda, Lav_Cod, ObjParametri_Server)
                    End If

                End If

            Next

            checkBundle.InsertPlotsError()

            If IDTestataTemp_Rilievi_FF > 0 AndAlso checkBundle.ContinueExport() Then
                supply.growthStages.AddRange(MapGrowthStages(Piva, tp_plots_Rilievi_FF, IDTestataTemp_Rilievi_FF, ObjParametri_Server))
                checkBundle.CheckGrowthStages(supply.growthStages)
            End If

            If checkBundle.ContinueExport() Then
                supply.plotDescriptions = MapPlots(List_Distinct_Impianti)

                If Not IsNothing(tp_Macchine) Then
                    checkBundle.CheckEquipments(tp_Macchine)
                    supply.equipmentList.AddRange(tp_Macchine.Item2 _
                                              .GroupBy(Function(k) New Tuple(Of String, Date, String)(k.calibrationId, k.lastCheckDate, k.description),
                                                       Function(key, gr) gr.FirstOrDefault()))

                End If

                If Not IsNothing(tp_Operatori) Then
                    checkBundle.CheckWorkers(tp_Operatori)
                    supply.workers.AddRange(tp_Operatori.Item2 _
                        .GroupBy(Function(k) New Tuple(Of String, String)(k.fiscalCode, k.vatNumber),
                                 Function(key, gr) gr.FirstOrDefault()))
                End If

            End If

        End If

        Return supply
    End Function

    Private Function MapFarmingEvent(ByVal operationId As String, ByVal operationDesLib As String, ByVal Lav_Cod As Integer, ByVal plot As Plot,
                                     ByVal Dr_Appezzamento_Impianto As DataRow, ByVal data_operazione As Date) As FarmingEvent

        Dim farmingEvent As New FarmingEvent(operationId, plot.islandId, plot.plotId, plot.pcgId, plot.plantationId, plot.codiBarrScheVali)

        farmingEvent.farmingPhase = DT_Lavorazioni.Select("Lav_Cod = " & Lav_Cod)(0)("Lav_Cod_Esterno")

        farmingEvent.startDate = GetDateTimeStr(data_operazione)

        farmingEvent.endDate = GetDateTimeStr(data_operazione)

        checkBundle.CheckFarmingEvent(operationDesLib, farmingEvent)

        Return farmingEvent
    End Function


    Private Function MapPhytochemicalTreatment(ByVal operationId As String, ByVal operationDesLib As String, ByVal plot As Plot, ByVal Dr_Appezzamento_Impianto As DataRow,
                                               ByVal ImpiantoPK As Impianto.PK, ByVal superficieTrattataImpianto As Decimal, ByVal anno As Integer, ByVal data_operazione As Date,
                                               ByVal ora_operazione As Date, ByVal risorsaDettaglioTrattamento As dettagli.DettaglioTrattamento,
                                               ByVal risorseMacchine As List(Of risorse.RisorsaMacchina),
                                               ByVal risorsePersone As List(Of risorse.RisorsaPersona), ByVal risorsaAcqua As RisorsaAcqua, ByVal modalitaApplicazione As BaseCodeDescr) As PhytochemicalTreatment

        Dim phytochemicalTreatment As New PhytochemicalTreatment(operationId, plot.islandId, plot.plotId, plot.pcgId, plot.plantationId, plot.codiBarrScheVali)

        phytochemicalTreatment.startDate = GetDateTimeStr(data_operazione, ora_operazione)

        phytochemicalTreatment.endDate = GetDateTimeStr(data_operazione, ora_operazione)

        phytochemicalTreatment.productRegistrationNumber = risorsaDettaglioTrattamento.prodotto.codice.ToString()

        phytochemicalTreatment.productMeasureUnit = GetProductMeasureUnit(risorsaDettaglioTrattamento.unitaDiMisura)

        Dim obj_Qta_Prodotto_X_Impianto = GetProductAndWaterQuantityFromPlot(operationId, risorsaDettaglioTrattamento, Dr_Appezzamento_Impianto, ImpiantoPK,
                                                                             superficieTrattataImpianto, risorsaAcqua)

        If Not IsNothing(obj_Qta_Prodotto_X_Impianto) Then
            phytochemicalTreatment.productQuantity = obj_Qta_Prodotto_X_Impianto.productQuantity

            phytochemicalTreatment.waterQuantity = obj_Qta_Prodotto_X_Impianto.waterQuantity

            phytochemicalTreatment.actionArea = obj_Qta_Prodotto_X_Impianto.actionArea
        Else
            phytochemicalTreatment.productQuantity = 0

            phytochemicalTreatment.waterQuantity = 0

            phytochemicalTreatment.actionArea = 0
        End If

        phytochemicalTreatment.applicationTypeCode = GetApplicationTypeCode(modalitaApplicazione)

        phytochemicalTreatment.adversityName = GetAdversityName(risorsaDettaglioTrattamento.avversitaGruppo)

        phytochemicalTreatment.workers = MapWorkers(risorsePersone, anno)

        phytochemicalTreatment.equipment = MapEquipments(risorseMacchine)

        phytochemicalTreatment.treatmentApplicationMode = GetTreatmentApplicationMode(phytochemicalTreatment.equipment)

        checkBundle.CheckPhytochemicalTreatment(operationDesLib, phytochemicalTreatment)

        Return phytochemicalTreatment

    End Function

    Private Function MapPhytochemicalTreatments(ByVal operationId As String, ByVal operationDesLib As String, ByVal plot As Plot, ByVal Dr_Appezzamento_Impianto As DataRow,
                                                ByVal ImpiantoPK As Impianto.PK, ByVal superficieTrattataImpianto As Decimal, ByVal anno As Integer, ByVal data_operazione As Date, ByVal ora_operazione As Date,
                                                ByVal risorse As List(Of risorse.Risorsa), ByVal modalitaApplicazione As BaseCodeDescr) As List(Of PhytochemicalTreatment)


        Dim phytochemicalTreatments As New List(Of PhytochemicalTreatment)

        Dim risorseDettaglioTrattamento = GetDettagliTrattamentoFromAttivita(risorse)

        If Not IsNothing(risorseDettaglioTrattamento) AndAlso risorseDettaglioTrattamento.Count > 0 Then

            Dim risorseMacchine As List(Of RisorsaMacchina) = GetRisorseMacchineFromAttivita(risorse)

            Dim risorsePersone As List(Of RisorsaPersona) = GetRisorsePersoneFromAttivita(risorse)

            Dim risorsaAcqua As RisorsaAcqua = GetRisorsaAcquaFromAttivita(risorse)

            For Each risorsaDettaglioTrattamento In risorseDettaglioTrattamento

                If checkBundle.CheckIfProductIsExportable(risorsaDettaglioTrattamento) Then
                    phytochemicalTreatments.Add(MapPhytochemicalTreatment(operationId, operationDesLib, plot, Dr_Appezzamento_Impianto, ImpiantoPK, superficieTrattataImpianto, anno,
                                                                          data_operazione, ora_operazione, risorsaDettaglioTrattamento, risorseMacchine,
                                                                          risorsePersone, risorsaAcqua, modalitaApplicazione))
                End If
            Next
        End If

        Return phytochemicalTreatments

    End Function

    Private Function MapGrowthStages(ByVal Piva As String, ByVal tp_plots_Rilievi_FF As List(Of Tuple(Of DataRow, Plot, Impianto.PK, Decimal)), ByVal IDTestataTemp As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As List(Of GrowthStage)

        Dim growthStages As New List(Of GrowthStage)

        Dim objMovimentiBIZ As New AgronicaCoreContabBIZ.Movimenti_R

        Dim List_Validita As List(Of Validita_Rilievi_FF_X_Impianto) = objMovimentiBIZ.List_Validita_Rilievi_Fasi_Fenologiche_X_Impianti(Piva, IDTestataTemp, Validita_Inizio, Validita_Fine, objParametri_Server)

        If Not IsNothing(List_Validita) AndAlso List_Validita.Count > 0 Then
            For Each validita_rilievi_FF In List_Validita
                Dim item = tp_plots_Rilievi_FF _
                    .Where(Function(t_p) Not IsNothing(t_p.Item1) AndAlso
                        t_p.Item1("Piva") = validita_rilievi_FF.Piva AndAlso
                        t_p.Item1("Sa_Cod") = validita_rilievi_FF.Sa_Cod AndAlso
                        t_p.Item1("Appezza") = validita_rilievi_FF.Appezza AndAlso
                        t_p.Item1("ID_REG") = validita_rilievi_FF.Id_Destinazione) _
                    .FirstOrDefault()

                If Not IsNothing(item) Then
                    growthStages.Add(MapGrowthStage(item.Item2, validita_rilievi_FF))
                End If
            Next
        End If

        If IDTestataTemp > 0 Then
            Dim obj__Tmp_Agenda As New AgronicaCoreVarieDAL.__tmp_Agenda_W


            obj__Tmp_Agenda.CancellaRecordDaIDTestataTemp(IDTestataTemp, objParametri_Server)
        End If

        Return growthStages
    End Function

    Private Function MapGrowthStage(ByVal plot As Plot, ByVal validita_rilievi_FF As Validita_Rilievi_FF_X_Impianto) As GrowthStage

        Dim growthStage As New GrowthStage(plot.islandId, plot.plotId, plot.pcgId, plot.plantationId, plot.codiBarrScheVali)

        If Not IsNothing(validita_rilievi_FF) AndAlso Not IsNothing(validita_rilievi_FF.Stadi_FF) AndAlso validita_rilievi_FF.Stadi_FF.Count > 0 Then

            Dim propsGrowthStage As List(Of PropertyInfo) = growthStage.GetType().GetProperties().ToList()

            For Each stadio_FF As Stadio_FF In validita_rilievi_FF.Stadi_FF

                Dim index_Fase_Description = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & stadio_FF.Stadio_Principale & "DescriptionToCheck")

                Dim index_StartDate = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & stadio_FF.Stadio_Principale & "StartDate")

                Dim index_StartDate_ToCheck = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & stadio_FF.Stadio_Principale & "StartDateToCheck")

                Dim index_StartDate_operationId = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & stadio_FF.Stadio_Principale & "StartDate_operationId")

                Dim index_StartDate_operationDescription = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & stadio_FF.Stadio_Principale & "StartDate_operationDescriptionToCheck")

                Dim index_EndDate = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & stadio_FF.Stadio_Principale & "EndDate")

                Dim index_EndDate_ToCheck = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & stadio_FF.Stadio_Principale & "EndDateToCheck")

                Dim index_EndDate_operationId = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & stadio_FF.Stadio_Principale & "EndDate_operationId")

                Dim index_EndDate_operationDescription = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & stadio_FF.Stadio_Principale & "EndDate_operationDescriptionToCheck")

                If index_Fase_Description > -1 AndAlso index_StartDate > -1 AndAlso index_StartDate_ToCheck > -1 AndAlso index_StartDate_operationId > -1 AndAlso
                    index_EndDate > -1 AndAlso index_EndDate_ToCheck > -1 AndAlso index_EndDate_operationId > -1 AndAlso index_EndDate_operationDescription > -1 Then

                    propsGrowthStage(index_Fase_Description).SetValue(growthStage, stadio_FF.Descrizione_Stadio_Principale)
                    propsGrowthStage(index_StartDate).SetValue(growthStage, GetDateTimeStr(stadio_FF.Validita_Inizio))
                    propsGrowthStage(index_StartDate_ToCheck).SetValue(growthStage, stadio_FF.Validita_Inizio)
                    propsGrowthStage(index_EndDate).SetValue(growthStage, GetDateTimeStr(stadio_FF.Validita_Fine))
                    propsGrowthStage(index_EndDate_ToCheck).SetValue(growthStage, stadio_FF.Validita_Fine)
                    propsGrowthStage(index_StartDate_operationId).SetValue(growthStage, Create_operationId(validita_rilievi_FF.Piva, stadio_FF.Id_Agenda_Validita_Inizio))
                    propsGrowthStage(index_StartDate_operationId).SetValue(growthStage, Create_operationId(validita_rilievi_FF.Piva, stadio_FF.Id_Agenda_Validita_Inizio))
                    propsGrowthStage(index_StartDate_operationDescription).SetValue(growthStage, stadio_FF.Des_Lib_Validita_Inizio)
                    propsGrowthStage(index_EndDate_operationDescription).SetValue(growthStage, stadio_FF.Des_Lib_Validita_Fine)
                End If
            Next
        End If

        Return growthStage

    End Function


    Private Function MapPlots(ByVal List_ImpiantiPK As List(Of Impianto.PK)) As List(Of Plot)

        Dim plots As New List(Of Plot)

        For Each ImpiantoPK In List_ImpiantiPK
            plots.Add(MapPlot(ImpiantoPK, Nothing))
        Next

        Return plots

    End Function

    Private Function MapPlot(ByVal impiantoKey As Impianto.PK, ByRef Dr_Appezzamento_Impianto As DataRow) As Plot

        Dim plot As Plot = Nothing

        Dim plot_key As Plot_Key = MapPlot_Key(impiantoKey, Dr_Appezzamento_Impianto)

        plot = New Plot(plot_key.islandId, plot_key.plotId, plot_key.pcgId, plot_key.plantationId, plot_key.codiBarrScheVali)

        If Not IsNothing(Dr_Appezzamento_Impianto) Then
            plot.plotDescription = Dr_Appezzamento_Impianto("App_Nome")
        End If

        Return plot

    End Function


    Private Function MapPlot_Key(ByVal impiantoKey As Impianto.PK, ByRef Dr_Appezzamento_Impianto As DataRow) As Plot_Key

        Dim plot_key As Plot_Key = Nothing

        Dim plotId As String = ""

        Dim islandId As String = ""

        Dim pcgId As String = ""

        Dim plantationId As String = ""

        Dim codiBarrScheVali As String = ""

        If IsNothing(Dr_Appezzamento_Impianto) Then
            Dim objUtility As New Utility

            Dr_Appezzamento_Impianto = objUtility.FindImpianto(impiantoKey, DT_Appezzamenti_Impianti)
        End If

        If Not IsNothing(Dr_Appezzamento_Impianto) Then
            plotId = IIf(IsDBNull(Dr_Appezzamento_Impianto("Agea_identificativoAppezzamento")), "", Dr_Appezzamento_Impianto("Agea_identificativoAppezzamento"))
            islandId = IIf(IsDBNull(Dr_Appezzamento_Impianto("Agea_identificativoIsola")), "", Dr_Appezzamento_Impianto("Agea_identificativoIsola"))
            pcgId = IIf(IsDBNull(Dr_Appezzamento_Impianto("Agea_identificativoPianoColtivazione")), "", Dr_Appezzamento_Impianto("Agea_identificativoPianoColtivazione"))
            plantationId = IIf(IsDBNull(Dr_Appezzamento_Impianto("Agea_idColt")), "", Dr_Appezzamento_Impianto("Agea_idColt"))
            codiBarrScheVali = IIf(IsDBNull(Dr_Appezzamento_Impianto("Agea_codiBarrScheVali")), "", Dr_Appezzamento_Impianto("Agea_codiBarrScheVali"))
        End If

        plot_key = New Plot_Key(islandId, plotId, pcgId, plantationId, codiBarrScheVali)

        Return plot_key

    End Function

    Private Function MapWorkers(ByVal risorsePersone As List(Of risorse.RisorsaPersona), ByVal anno As Integer) As List(Of Worker)

        Dim workers As New List(Of Worker)

        If Not IsNothing(risorsePersone) AndAlso risorsePersone.Count > 0 Then
            For Each risorsaPersona In risorsePersone
                workers.Add(MapWorker(risorsaPersona, anno))
            Next
        End If

        Return workers
    End Function

    Private Function MapWorker(ByVal risorsaPersona As risorse.RisorsaPersona, ByVal anno As Integer) As Worker
        Dim worker As New Worker

        If Not IsNothing(risorsaPersona) AndAlso Not IsNothing(risorsaPersona.risorsaUmana) AndAlso Not IsNothing(risorsaPersona.risorsaUmana.contatto) AndAlso
            Not String.IsNullOrEmpty(risorsaPersona.risorsaUmana.contatto.primaryKey.codice) Then

            Dim personType = GetPersonType(risorsaPersona.risorsaUmana)
            Dim workerElem = New WorkerElement(risorsaPersona.risorsaUmana, personType, anno)
            worker = workerElem.ToWorker()

            If IsNothing(tp_Operatori) Then
                tp_Operatori = Tuple.Create(New List(Of RisorsaPersona), New List(Of WorkerElement))
            End If

            If tp_Operatori.Item1.FindIndex(Function(t_p) t_p.risorsaUmana.codice = risorsaPersona.risorsaUmana.codice AndAlso
                                                    t_p.risorsaUmana.contatto.primaryKey.codice = risorsaPersona.risorsaUmana.contatto.primaryKey.codice AndAlso
                                                    t_p.risorsaUmana.contatto.primaryKey.partitaIva = risorsaPersona.risorsaUmana.contatto.primaryKey.partitaIva) = -1 Then

                tp_Operatori.Item1.Add(risorsaPersona)

                tp_Operatori.Item2.Add(workerElem)
            End If


        End If

        Return worker
    End Function

    Private Function MapEquipments(ByVal risorseMacchine As List(Of risorse.RisorsaMacchina)) As List(Of Equipment)
        Dim equipments As New List(Of Equipment)

        If Not IsNothing(risorseMacchine) AndAlso risorseMacchine.Count > 0 Then
            For Each risorsaMacchina In risorseMacchine
                equipments.Add(MapEquipment(risorsaMacchina))
            Next
        End If

        Return equipments
    End Function

    Private Function MapEquipment(ByVal risorsaMacchina As risorse.RisorsaMacchina) As Equipment

        Dim equipment = Nothing

        Dim equipmentElem = Nothing

        If Not IsNothing(DT_Macchine) Then

            Dim Dr As DataRow = DT_Macchine _
                .Select("Mac_Cod = " & risorsaMacchina.macchina.codice).FirstOrDefault()

            If Not IsNothing(Dr) Then

                equipmentElem = New EquipmentElement(Dr)

                equipment = equipmentElem.ToEquipment()
            End If
        End If

        If Not IsNothing(equipmentElem) Then

            If IsNothing(tp_Macchine) Then
                tp_Macchine = Tuple.Create(New List(Of RisorsaMacchina), New List(Of EquipmentElement))
            End If

            If tp_Macchine.Item1.FindIndex(Function(m) m.macchina.codice = risorsaMacchina.macchina.codice) = -1 Then
                tp_Macchine.Item1.Add(risorsaMacchina)

                tp_Macchine.Item2.Add(equipmentElem)
            End If
        End If

        Return equipment
    End Function

    Private Function MapIrrigations(ByVal operationId As String, ByVal Piva As String, ByVal plot As Plot, ByVal Dr_Appezzamento_Impianto As DataRow, ByVal data_operazione As Date,
                                    ByVal ora_irrigazione As Date, ByVal risorse As List(Of risorse.Risorsa)) As List(Of Irrigation)

        Dim irrigations As New List(Of Irrigation)

        If risorse.FindIndex(Function(r) r.classType = ClassType.DettaglioIrrigazione) > -1 Then

            Dim risorseDettaglioIrrigazione = risorse.FindAll(Function(r) r.classType = ClassType.DettaglioIrrigazione).ConvertAll(Function(r) CType(r, dettagli.DettaglioIrrigazione))

            If Not IsNothing(risorseDettaglioIrrigazione) AndAlso risorseDettaglioIrrigazione.Count > 0 Then
                Dim item = risorseDettaglioIrrigazione _
                    .Where(Function(r) (Not IsNothing(r.esercizioCDC)) AndAlso
                        (Not IsNothing(Dr_Appezzamento_Impianto)) AndAlso
                        r.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva = Dr_Appezzamento_Impianto("Piva") AndAlso
                        r.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice = Dr_Appezzamento_Impianto("Sa_Cod") AndAlso
                        r.esercizioCDC.esercizio.impiantoPK.appezzamentoPK.codice = Dr_Appezzamento_Impianto("Appezza") AndAlso
                        r.esercizioCDC.esercizio.impiantoPK.codice = Dr_Appezzamento_Impianto("Id_Reg")) _
                    .FirstOrDefault()

                If Not IsNothing(item) Then

                    Dim irrigation As Irrigation = Nothing

                    Dim risorsaDettaglioIrrigazione As dettagli.DettaglioIrrigazione = item

                    Dim superficie_trattata As Decimal = risorsaDettaglioIrrigazione.esercizioCDC.superficieTrattata

                    Dim qta_totale As Decimal = risorsaDettaglioIrrigazione.QtaTotale

                    If risorsaDettaglioIrrigazione.Frequenza > 0 AndAlso
                        risorsaDettaglioIrrigazione.DataInizio <> AGRODATAINIZIO AndAlso risorsaDettaglioIrrigazione.DataFine <> AGRODATAFINE Then

                        Dim Differenza_Giorni = DateDiff(DateInterval.Day, risorsaDettaglioIrrigazione.DataInizio, risorsaDettaglioIrrigazione.DataFine) + 1

                        If Differenza_Giorni > 0 Then

                            Dim Frequenza = risorsaDettaglioIrrigazione.Frequenza

                            Dim Giorni_Irrigati = Math.Floor(Differenza_Giorni / Frequenza)

                            If Giorni_Irrigati > 0 Then

                                Dim dataIrrigazione = risorsaDettaglioIrrigazione.DataInizio

                                Dim giornoIrrigazione As Integer = 1

                                While dataIrrigazione <= risorsaDettaglioIrrigazione.DataFine AndAlso giornoIrrigazione <= Giorni_Irrigati

                                    irrigations.Add(MapIrrigation(operationId, Piva, plot, Dr_Appezzamento_Impianto, dataIrrigazione, ora_irrigazione, qta_totale, superficie_trattata))

                                    dataIrrigazione = dataIrrigazione.AddDays(Frequenza)
                                    giornoIrrigazione += 1

                                End While
                            End If
                        End If



                    Else
                        irrigations.Add(MapIrrigation(operationId, Piva, plot, Dr_Appezzamento_Impianto, data_operazione, ora_irrigazione, qta_totale, superficie_trattata))
                    End If

                End If
            End If
        End If

        Return irrigations

    End Function

    Private Function MapIrrigation(ByVal operationId As String, ByVal Piva As String, ByVal plot As Plot, ByVal Dr_Appezzamento_Impianto As DataRow, ByVal data_irrigazione As Date,
                                   ByVal ora_irrigazione As Date, ByVal qta As Decimal, ByVal superficie_trattata As Decimal) As Irrigation

        Dim irrigation As New Irrigation(operationId, plot.islandId, plot.plotId, plot.pcgId, plot.plantationId, plot.codiBarrScheVali)

        irrigation.eventDate = GetDateTimeStr(data_irrigazione, New DateTime(data_irrigazione.Year, data_irrigazione.Month, data_irrigazione.Day,
                                                                                    0, 0, 0))

        irrigation.actionArea = superficie_trattata

        irrigation.quantity = qta

        'TODO Campo non obbligatorio da capire come popolarlo
        irrigation.applicationTypeCode = Nothing

        'Per ora messo fisso a codice
        irrigation.fertigation = False

        Return irrigation

    End Function

    Private Function MapChemicalFertilizations(ByVal operationId As String, ByVal operationDesLib As String, ByVal plot As Plot, ByVal Dr_Appezzamento_Impianto As DataRow, ByVal ImpiantoPK As Impianto.PK,
                                               ByVal superficieTrattataImpianto As Decimal, ByVal data_operazione As Date,
                                               ByVal ora_operazione As Date, ByVal risorse As List(Of risorse.Risorsa), ByVal modalitaApplicazione As BaseCodeDescr) As List(Of ChemicalFertilization)

        Dim chemicalFertilizations As New List(Of ChemicalFertilization)

        If risorse.FindIndex(Function(r) r.classType = ClassType.DettaglioFertilizzazione) > -1 Then

            Dim risorseDettaglioFertilizzazione = risorse.FindAll(Function(r) r.classType = ClassType.DettaglioFertilizzazione).ConvertAll(Function(r) CType(r, dettagli.DettaglioFertilizzazione))

            Dim risorsaAcqua As risorse.RisorsaAcqua = Nothing

            If risorse.FindIndex(Function(r) r.classType = ClassType.RisorsaAcqua) > -1 Then
                risorsaAcqua = CType(risorse.Find(Function(r) r.classType = ClassType.RisorsaAcqua), risorse.RisorsaAcqua)
            End If

            If Not IsNothing(risorseDettaglioFertilizzazione) AndAlso risorseDettaglioFertilizzazione.Count > 0 Then
                For Each risorsaDettaglioFertilizzazione In risorseDettaglioFertilizzazione

                    chemicalFertilizations.Add(MapChemicalFertilization(operationId, operationDesLib, plot, Dr_Appezzamento_Impianto, ImpiantoPK, superficieTrattataImpianto,
                                                                        data_operazione, ora_operazione, risorsaDettaglioFertilizzazione, risorsaAcqua, modalitaApplicazione))
                Next
            End If

        End If

        Return chemicalFertilizations
    End Function

    Private Function MapChemicalFertilization(ByVal operationId As String, ByVal operationDesLib As String, ByVal plot As Plot, ByVal Dr_Appezzamento_Impianto As DataRow, ByVal ImpiantoPK As Impianto.PK,
                                              ByVal superficieTrattataImpianto As Decimal, ByVal data_operazione As Date, ByVal ora_operazione As Date,
                                               ByVal risorsaDettaglioFertilizzazione As dettagli.DettaglioFertilizzazione, ByVal risorsaAcqua As RisorsaAcqua, ByVal modalitaApplicazione As BaseCodeDescr) As ChemicalFertilization

        Dim chemicalFertilization As New ChemicalFertilization(operationId, plot.islandId, plot.plotId, plot.pcgId, plot.plantationId, plot.codiBarrScheVali)

        chemicalFertilization.startDate = GetDateTimeStr(data_operazione, ora_operazione)

        chemicalFertilization.endDate = GetDateTimeStr(data_operazione, ora_operazione)

        chemicalFertilization.productRegistrationNumber = risorsaDettaglioFertilizzazione.prodotto.codice.ToString()

        chemicalFertilization.productMeasureUnit = GetProductMeasureUnit(risorsaDettaglioFertilizzazione.unitaDiMisura)

        Dim obj_Qta_Prodotto_X_Impianto = GetProductAndWaterQuantityFromPlot(operationId, risorsaDettaglioFertilizzazione, Dr_Appezzamento_Impianto,
                                                                             ImpiantoPK, superficieTrattataImpianto, risorsaAcqua)

        If Not IsNothing(obj_Qta_Prodotto_X_Impianto) Then
            chemicalFertilization.productQuantity = obj_Qta_Prodotto_X_Impianto.productQuantity

            chemicalFertilization.actionArea = obj_Qta_Prodotto_X_Impianto.actionArea
        Else
            chemicalFertilization.productQuantity = 0

            chemicalFertilization.actionArea = 0
        End If

        chemicalFertilization.areaQuantity = Nothing

        'If chemicalFertilization.actionArea > 0 Then
        '    If chemicalFertilization.actionArea = Dr_Appezzamento_Impianto("Sup_Imp") Then
        '        chemicalFertilization.areaQuantity = Quantity.Tutta
        '    Else
        '        chemicalFertilization.areaQuantity = Quantity.Inbanda
        '    End If
        'End If

        chemicalFertilization.applicationTypeCode = GetApplicationTypeCode(modalitaApplicazione)

        chemicalFertilization.planting = Nothing

        'Per ora fisso a codice
        chemicalFertilization.productTaxonomyId = CHEMICAL_FERTILIZER_TAXONOMY_ID

        checkBundle.CheckChemicalFertilization(operationDesLib, chemicalFertilization)

        Return chemicalFertilization
    End Function

    Private Function MapOrganicFertilizations(ByVal operationId As String, ByVal operationDesLib As String, ByVal plot As Plot, ByVal Dr_Appezzamento_Impianto As DataRow, ByVal ImpiantoPK As Impianto.PK,
                                              ByVal superficieTrattataImpianto As Decimal, ByVal data_operazione As Date,
                                               ByVal ora_operazione As Date, ByVal risorse As List(Of risorse.Risorsa), ByVal modalitaApplicazione As BaseCodeDescr) As List(Of OrganicFertilization)

        Dim organicFertilizations As New List(Of OrganicFertilization)

        If risorse.FindIndex(Function(r) r.classType = ClassType.DettaglioFertilizzazione) > -1 Then

            Dim risorseDettaglioFertilizzazione = risorse.FindAll(Function(r) r.classType = ClassType.DettaglioFertilizzazione).ConvertAll(Function(r) CType(r, dettagli.DettaglioFertilizzazione))

            If Not IsNothing(risorseDettaglioFertilizzazione) AndAlso risorseDettaglioFertilizzazione.Count > 0 Then
                For Each risorsaDettaglioFertilizzazione In risorseDettaglioFertilizzazione

                    organicFertilizations.Add(MapOrganicFertilization(operationId, operationDesLib, plot, Dr_Appezzamento_Impianto, ImpiantoPK, superficieTrattataImpianto,
                                                                      data_operazione, ora_operazione, risorsaDettaglioFertilizzazione, modalitaApplicazione))
                Next
            End If

        End If

        Return organicFertilizations
    End Function

    Private Function MapOrganicFertilization(ByVal operationId As String, ByVal operationDesLib As String, ByVal plot As Plot, ByVal Dr_Appezzamento_Impianto As DataRow, ByVal ImpiantoPK As Impianto.PK,
                                             ByVal superficieTrattataImpianto As Decimal, ByVal data_operazione As Date,
                                             ByVal ora_operazione As Date, ByVal risorsaDettaglioFertilizzazione As dettagli.DettaglioFertilizzazione, ByVal modalitaApplicazione As BaseCodeDescr) As OrganicFertilization

        Dim organicFertilization As New OrganicFertilization(operationId, plot.islandId, plot.plotId, plot.pcgId, plot.plantationId, plot.codiBarrScheVali)

        organicFertilization.startDate = GetDateTimeStr(data_operazione, ora_operazione)

        organicFertilization.endDate = GetDateTimeStr(data_operazione, ora_operazione)

        Dim obj_Qta_Prodotto_X_Impianto = GetProductAndWaterQuantityFromPlot(operationId, risorsaDettaglioFertilizzazione, Dr_Appezzamento_Impianto,
                                                                             ImpiantoPK, superficieTrattataImpianto, Nothing)

        If Not IsNothing(obj_Qta_Prodotto_X_Impianto) Then
            organicFertilization.productQuantity = obj_Qta_Prodotto_X_Impianto.productQuantity

            organicFertilization.actionArea = obj_Qta_Prodotto_X_Impianto.actionArea
        Else
            organicFertilization.productQuantity = 0

            organicFertilization.actionArea = 0
        End If

        organicFertilization.productMeasureUnit = GetProductMeasureUnit(risorsaDettaglioFertilizzazione.unitaDiMisura)

        organicFertilization.applicationTypeCode = GetApplicationTypeCode(modalitaApplicazione)

        organicFertilization.planting = Nothing

        'Per ora fisso a codice
        organicFertilization.productTaxonomyId = ORGANIC_FERTILIZER_TAXONOMY_ID

        organicFertilization.nitrogenPerMil = 0

        If risorsaDettaglioFertilizzazione.N > 0 Then
            organicFertilization.nitrogenPerMil = risorsaDettaglioFertilizzazione.N * 10
        End If

        organicFertilization.phosphorusPerMil = 0

        If risorsaDettaglioFertilizzazione.P > 0 Then
            organicFertilization.phosphorusPerMil = risorsaDettaglioFertilizzazione.P * 10
        End If

        organicFertilization.potassiumPerMil = 0

        If risorsaDettaglioFertilizzazione.K > 0 Then
            organicFertilization.potassiumPerMil = risorsaDettaglioFertilizzazione.K * 10
        End If

        organicFertilization.npkCoefficient = risorsaDettaglioFertilizzazione.efficienza

        organicFertilization.dryMatter = Nothing

        checkBundle.CheckOrganicFertilization(operationDesLib, organicFertilization)

        Return organicFertilization
    End Function

    Private Function MapWarehouses(ByVal Piva As String, ByRef tp_Warehouses As List(Of Tuple(Of Warehouse, FabbricatoLight)), ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of Warehouse)

        Dim Data_Inizio As Date = Validita_Inizio
        'Dim Data_Fine As Date = If(Validita_Fine.CompareTo(Date.Now) > 0, Date.Now, Validita_Fine)
        Dim Data_Fine As Date = Validita_Fine

        Dim warehouses As New List(Of Warehouse)

        Dim objFabbricato As New AgronicaCoreAnagrafeDAL.Fabbricati_R
        Dim dtFabbricati = objFabbricato.Leggi_3(Piva, 0, 0, "", "", objParametri_Server, True)

        Dim warehouse As Warehouse
        For Each row In dtFabbricati.Rows
            warehouse = MapWarehouse(Piva, row, Data_Inizio, Data_Fine, objParametri_Server, objParametri_Utenti)
            warehouses.Add(warehouse)
            tp_Warehouses.Add(Tuple.Create(warehouse, New FabbricatoLight(Piva, row("Sa_Cod"), row("Fabbricato_Cod"), row("Fabbricato_des"))))
        Next

        warehouses = warehouses.Where(Function(s) s.pesticides.Count > 0 Or s.chemicalFertilizers.Count > 0 Or s.organicFertilizers.Count > 0).ToList()

        Return warehouses
    End Function

    Private Function MapWarehouse(ByVal Piva As String, ByRef row As DataRow, ByVal Data_Inizio As Date, ByVal Data_Fine As Date, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri) As Warehouse

        Dim address As String = IIf(IsDBNull(row("ind_des")), "", row("ind_des"))
        Dim municipality As String = row("LOCALITA")
        Dim foglio As String = row("FOGLIO")
        Dim particella As String = row("NUMERO")
        Dim subalterno As String = row("SUBALTERNO")
        Dim georeferencing As String = ""   'TODO Campo non obbligatorio, non presente su GIAS, per ora non lo passiamo 
        Dim capacity As Decimal = row("MC_Convenzionale")
        Dim description As String = GetWarehouse_description(row("Sa_Cod"), row("Fabbricato_Cod"), row("Fabbricato_des"))

        If capacity = 0 And row("MC_Biologico") <> 0 Then
            capacity = row("MC_Biologico")
        End If

        Dim SaCod As Integer = row("Sa_Cod")
        Dim FabbricatoCod As Integer = row("Fabbricato_Cod")

        Dim warehouse As New Warehouse(address, municipality, foglio, particella, subalterno, georeferencing, capacity, description)

        MapWarehouseStocks(Piva, SaCod, FabbricatoCod, Data_Inizio, Data_Fine, warehouse, objParametri_Server, objParametri_Utenti)

        'warehouse.pesticides.OrderBy(Function(x) x.productRegistrationNumber).ThenBy(Function(x) x.storageDate)
        'warehouse.chemicalFertilizers.OrderBy(Function(x) x.productRegistrationNumber).ThenBy(Function(x) x.storageDate)
        'warehouse.organicFertilizers.OrderBy(Function(x) x.productRegistrationNumber).ThenBy(Function(x) x.storageDate)

        Return warehouse
    End Function

    Private Sub MapWarehouseStocks(ByVal Piva As String, ByVal SaCod As Integer, ByVal FabbricatoCod As Integer, ByVal Data_Inizio As Date, ByVal Data_Fine As Date, ByRef warehouse As Warehouse, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim dictLastDateFormulati As New Dictionary(Of Integer, Dictionary(Of Integer, Date))
        Dim dictLastDateFertilizzanti As New Dictionary(Of Integer, Dictionary(Of Integer, Date))

        Dim dictQtaFormulati As New Dictionary(Of Integer, Dictionary(Of Integer, Decimal))
        Dim dictQtaFertilizzanti As New Dictionary(Of Integer, Dictionary(Of Integer, Decimal))

        Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R
        'Dim data_giacenza As Date = Data_Inizio.AddDays(-1)
        'Dim dtGiacenzeFormulati = objGiacenze.SchedaGiacenzeMagazzino(data_giacenza, Piva, SaCod, FabbricatoCod, FORMULATI, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, False, "", "", "", "", "", "", "", "", "", "", "", "", objParametri_Server, objParametri_Utenti)
        'Dim dtGiacenzeFertilizzanti = objGiacenze.SchedaGiacenzeMagazzino(data_giacenza, Piva, SaCod, FabbricatoCod, FERTILIZZANTI, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, False, "", "", "", "", "", "", "", "", "", "", "", "", objParametri_Server, objParametri_Utenti)

        'Dim movimentiObj As New AgronicaCoreContabDAL.Movimenti_R
        'Dim dtMovimentiFormulati = movimentiObj.SchedaMovimentiMagazzino(Data_Inizio, Data_Fine, Piva, SaCod, FabbricatoCod, FORMULATI, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, "", "", "", "", "", "", "", "", "", "", "", "Data_Movimento", objParametri_Server, objParametri_Utenti)
        'Dim dtMovimentiFertilizzanti = movimentiObj.SchedaMovimentiMagazzino(Data_Inizio, Data_Fine, Piva, SaCod, FabbricatoCod, FERTILIZZANTI, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, "", "", "", "", "", "", "", "", "", "", "", "Data_Movimento", objParametri_Server, objParametri_Utenti)

        'For Each row In dtMovimentiFormulati.Rows
        '    MapWarehouseStock(row, FORMULATI, Data_Inizio, dtGiacenzeFormulati, dictLastDateFormulati, dictQtaFormulati, warehouse, objParametri_Server)
        'Next

        'For Each row In dtMovimentiFertilizzanti.Rows
        '    MapWarehouseStock(row, FERTILIZZANTI, Data_Inizio, dtGiacenzeFertilizzanti, dictLastDateFertilizzanti, dictQtaFertilizzanti, warehouse, objParametri_Server)
        'Next

        'Come data inizio si considerano tutte le giacenze registrate precedenti al 01/01 dell'anno di esportazione
        Dim data_giacenza_inizio As Date = Data_Inizio.AddDays(-1)

        Dim data_giacenza_fine As Date = Data_Fine

        Dim dtGiacenzaInizialeFormulati = objGiacenze.SchedaGiacenzeMagazzino(data_giacenza_inizio, Piva, SaCod, FabbricatoCod, FORMULATI, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, False, "", "", "", "", "", "", "", "", "", "", "", "", objParametri_Server, objParametri_Utenti)
        Dim dtGiacenzaFinaleFormulati = objGiacenze.SchedaGiacenzeMagazzino(data_giacenza_fine, Piva, SaCod, FabbricatoCod, FORMULATI, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, False, "", "", "", "", "", "", "", "", "", "", "", "", objParametri_Server, objParametri_Utenti)
        Dim distinctFormulati As DataTable = dtGiacenzaFinaleFormulati.DefaultView.ToTable(True, {"Pro_Cod", "Udm_Cod"})

        Dim dtGiacenzaInizialeFertilizzanti = objGiacenze.SchedaGiacenzeMagazzino(data_giacenza_inizio, Piva, SaCod, FabbricatoCod, FERTILIZZANTI, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, False, "", "", "", "", "", "", "", "", "", "", "", "", objParametri_Server, objParametri_Utenti)
        Dim dtGiacenzaFinaleFertilizzanti = objGiacenze.SchedaGiacenzeMagazzino(data_giacenza_fine, Piva, SaCod, FabbricatoCod, FERTILIZZANTI, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, False, "", "", "", "", "", "", "", "", "", "", "", "", objParametri_Server, objParametri_Utenti)
        Dim distinctFertilizzanti As DataTable = dtGiacenzaFinaleFertilizzanti.DefaultView.ToTable(True, {"Pro_Cod", "Udm_Cod"})

        'Unita di Misura iniziale è uguale a quella finale
        Dim UdmCod As Integer = 0
        Dim ProCod As Integer = 0
        Dim Giacenza_Inizio As Decimal = 0
        Dim Giacenza_Fine As Decimal = 0

        For Each row In distinctFormulati.Rows
            ProCod = row("Pro_Cod")
            UdmCod = row("Udm_Cod")
            Giacenza_Fine = 0
            Giacenza_Inizio = 0

            Dim drGiacenzaFinaleFormulati = dtGiacenzaFinaleFormulati.Select("Pro_Cod = " & ProCod & " And Udm_Cod = " & UdmCod)

            If Not IsNothing(drGiacenzaFinaleFormulati) AndAlso drGiacenzaFinaleFormulati.Length > 0 Then
                Giacenza_Fine = Agro_Math.ArrotondaVal_3(drGiacenzaFinaleFormulati.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("Giacenza")))
            End If

            Dim drGiacenzaInizialeFormulati = dtGiacenzaInizialeFormulati.Select("Pro_Cod = " & ProCod & " And Udm_Cod = " & UdmCod)

            If Not IsNothing(drGiacenzaInizialeFormulati) AndAlso drGiacenzaInizialeFormulati.Length > 0 Then
                Giacenza_Inizio = Agro_Math.ArrotondaVal_3(drGiacenzaInizialeFormulati.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("Giacenza")))
            End If

            MapWarehouseStock(FORMULATI, ProCod, Data_Inizio, Data_Fine, UdmCod, UdmCod, Giacenza_Inizio, Giacenza_Fine, warehouse, objParametri_Server)
        Next

        For Each row In distinctFertilizzanti.Rows
            ProCod = row("Pro_Cod")
            UdmCod = row("Udm_Cod")
            Giacenza_Fine = 0
            Giacenza_Inizio = 0

            Dim drGiacenzaFinaleFertilizzanti = dtGiacenzaFinaleFertilizzanti.Select("Pro_Cod = " & ProCod & " And Udm_Cod = " & UdmCod)

            If Not IsNothing(drGiacenzaFinaleFertilizzanti) AndAlso drGiacenzaFinaleFertilizzanti.Length > 0 Then
                Giacenza_Fine = Agro_Math.ArrotondaVal_3(drGiacenzaFinaleFertilizzanti.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("Giacenza")))
            End If

            Dim drGiacenzaInizialeFertilizzanti = dtGiacenzaInizialeFertilizzanti.Select("Pro_Cod = " & ProCod & " And Udm_Cod = " & UdmCod)

            If Not IsNothing(drGiacenzaInizialeFertilizzanti) AndAlso drGiacenzaInizialeFertilizzanti.Length > 0 Then
                Giacenza_Inizio = Agro_Math.ArrotondaVal_3(drGiacenzaInizialeFertilizzanti.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("Giacenza")))
            End If

            MapWarehouseStock(FERTILIZZANTI, ProCod, Data_Inizio, Data_Fine, UdmCod, UdmCod, Giacenza_Inizio, Giacenza_Fine, warehouse, objParametri_Server)
        Next

    End Sub

    Private Sub MapWarehouseStock(ByVal ElemCod As Integer, ByVal ProCod As Integer,
                                  ByVal Data_Inizio As Date, ByVal Data_Fine As Date,
                                  ByVal UdmCod_Inizio As Integer, ByVal UdmCod_Fine As Integer,
                                  ByVal Giacenza_Inizio As String, ByVal Giacenza_Fine As String,
                                  ByRef warehouse As Warehouse, ByRef objParametri_Server As AgronicaCoreParametri)

        Dim storageDate As Date
        Dim quantityLastModifiedDate As Date
        Dim initialQuantity As Decimal
        Dim initialMeasureUnit As String
        Dim quantity As Decimal
        Dim measureUnit As String

        Dim productTaxonomyId As Integer
        Dim productRegistrationNumber As String
        Dim nitrogenPerMil As Decimal
        Dim phosphorusPerMil As Decimal
        Dim potassiumPerMil As Decimal
        Dim fertilizerDescription As String

        storageDate = Data_Inizio
        quantityLastModifiedDate = Data_Fine
        initialQuantity = Giacenza_Inizio
        quantity = Giacenza_Fine
        initialMeasureUnit = GetProductMeasureUnit(New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(UdmCod_Inizio))
        measureUnit = GetProductMeasureUnit(New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(UdmCod_Fine))

        Select Case ElemCod
            Case FORMULATI

                productRegistrationNumber = ProCod

                warehouse.pesticides.Add(New WarehouseStockPesticide(storageDate, quantityLastModifiedDate, initialQuantity, initialMeasureUnit, quantity, measureUnit, productRegistrationNumber))

            Case FERTILIZZANTI

                Dim objFertilizzanti As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
                Dim dtFertilizzanti = objFertilizzanti.Leggi_Completa_Classificazione(ProCod, "", 0, False, "", 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                If dtFertilizzanti.Rows.Count > 0 Then

                    fertilizerDescription = dtFertilizzanti(0)("Fer_Des")

                    ' Tipologie
                    ' 1: Antibutteratura
                    ' 2: Fogliare
                    ' 3: Idrosolubile
                    ' 4: Concimazione pieno campo
                    ' 5: Fertirrigazione
                    ' 6: Concimazione organica
                    ' 7: Letame
                    ' 8: Liquame
                    If dtFertilizzanti.Select("TP_COD in (6, 7, 8)").Length > 0 Then

                        ' Per ora settata da codice
                        productTaxonomyId = ORGANIC_FERTILIZER_TAXONOMY_ID
                        nitrogenPerMil = dtFertilizzanti(0)("N") * 10
                        phosphorusPerMil = dtFertilizzanti(0)("P2O5") * 10
                        potassiumPerMil = dtFertilizzanti(0)("K2O") * 10

                        warehouse.organicFertilizers.Add(New WarehouseStockOrganicFertilizer(storageDate, quantityLastModifiedDate, initialQuantity, initialMeasureUnit, quantity, measureUnit, productTaxonomyId, nitrogenPerMil, phosphorusPerMil, potassiumPerMil, fertilizerDescription))

                    Else

                        ' Per ora settata da codice
                        productTaxonomyId = CHEMICAL_FERTILIZER_TAXONOMY_ID
                        productRegistrationNumber = ProCod

                        warehouse.chemicalFertilizers.Add(New WarehouseStockChemicalFertilizer(storageDate, quantityLastModifiedDate, initialQuantity, initialMeasureUnit, quantity, measureUnit, productTaxonomyId, productRegistrationNumber, fertilizerDescription))

                    End If

                End If
        End Select

    End Sub

    'Private Sub MapWarehouseStock(ByRef row As DataRow, ByVal ElemCod As Integer, ByVal Data_Inizio As Date, ByRef dtGiacenze As DataTable,
    '                              ByRef dictProdLastDate As Dictionary(Of Integer, Dictionary(Of Integer, Date)),
    '                              ByRef dictProdLastQta As Dictionary(Of Integer, Dictionary(Of Integer, Decimal)),
    '                              ByRef warehouse As Warehouse, ByRef objParametri_Server As AgronicaCoreParametri)

    '    Dim storageDate As Date
    '    Dim quantityLastModifiedDate As Date
    '    Dim initialQuantity As Decimal
    '    Dim initialMeasureUnit As String
    '    Dim quantity As Decimal
    '    Dim measureUnit As String

    '    Dim productTaxonomyId As Integer
    '    Dim productRegistrationNumber As String
    '    Dim nitrogenPerMil As Decimal
    '    Dim phosphorusPerMil As Decimal
    '    Dim potassiumPerMil As Decimal
    '    Dim fertilizerDescription As String

    '    Dim ProCod As Integer = row("Pro_Cod")
    '    Dim UdmCod As Integer = row("Udm_Cod")
    '    Dim CauMov As Integer = row("Cau_Mov")

    '    Dim carico As Boolean

    '    Dim dictProdDatePerUdm As Dictionary(Of Integer, Date)
    '    If Not dictProdLastDate.ContainsKey(ProCod) Then
    '        dictProdDatePerUdm = New Dictionary(Of Integer, Date)
    '        dictProdLastDate.Add(ProCod, dictProdDatePerUdm)
    '    Else
    '        dictProdDatePerUdm = dictProdLastDate(ProCod)
    '    End If

    '    Dim lastProdDate As Date
    '    If Not dictProdDatePerUdm.ContainsKey(UdmCod) Then
    '        lastProdDate = Data_Inizio
    '        dictProdDatePerUdm.Add(UdmCod, Data_Inizio)
    '    Else
    '        lastProdDate = dictProdDatePerUdm(UdmCod)
    '    End If

    '    Dim dictProdQtaPerUdm As Dictionary(Of Integer, Decimal)
    '    If Not dictProdLastQta.ContainsKey(ProCod) Then
    '        dictProdQtaPerUdm = New Dictionary(Of Integer, Decimal)
    '        dictProdLastQta.Add(ProCod, dictProdQtaPerUdm)
    '    Else
    '        dictProdQtaPerUdm = dictProdLastQta(ProCod)
    '    End If

    '    Dim lastProdQta As Decimal
    '    If Not dictProdQtaPerUdm.ContainsKey(UdmCod) Then
    '        Dim drGiacenze = dtGiacenze.Select("Pro_Cod = " & ProCod)
    '        lastProdQta = If(drGiacenze.Length, drGiacenze(0)("Giacenza"), 0)
    '        dictProdQtaPerUdm.Add(UdmCod, lastProdQta)
    '    Else
    '        lastProdQta = dictProdQtaPerUdm(UdmCod)
    '    End If

    '    measureUnit = GetProductMeasureUnit(New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(UdmCod))

    '    quantityLastModifiedDate = lastProdDate
    '    initialQuantity = lastProdQta
    '    initialMeasureUnit = measureUnit

    '    storageDate = row("Data_Movimento")
    '    If CauMov = CAU_CONFERIMENTO Or CauMov = CAU_CARICO Or CauMov = CAU_ACCETTAZIONE_BENI_DA_DIVERSI Then
    '        quantity = lastProdQta + row("Qta")
    '        carico = True
    '    Else
    '        quantity = lastProdQta - row("Qta")
    '        carico = False
    '    End If

    '    dictProdDatePerUdm(UdmCod) = storageDate
    '    dictProdQtaPerUdm(UdmCod) = quantity

    '    If Not carico Then
    '        Return
    '    End If

    '    Select Case ElemCod
    '        Case FORMULATI

    '            productRegistrationNumber = ProCod
    '            warehouse.pesticides.Add(New WarehouseStockPesticide(storageDate, quantityLastModifiedDate, initialQuantity, initialMeasureUnit, quantity, measureUnit, productRegistrationNumber))

    '        Case FERTILIZZANTI

    '            Dim objFertilizzanti As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
    '            Dim dtFertilizzanti = objFertilizzanti.Leggi_Completa_Classificazione(ProCod, "", 0, False, "", 0, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

    '            If dtFertilizzanti.Rows.Count > 0 Then

    '                fertilizerDescription = dtFertilizzanti(0)("Fer_Des")

    '                ' Tipologie
    '                ' 1: Antibutteratura
    '                ' 2: Fogliare
    '                ' 3: Idrosolubile
    '                ' 4: Concimazione pieno campo
    '                ' 5: Fertirrigazione
    '                ' 6: Concimazione organica
    '                ' 7: Letame
    '                ' 8: Liquame
    '                If dtFertilizzanti.Select("TP_COD in (6, 7, 8)").Length > 0 Then

    '                    ' Per ora settata da codice
    '                    productTaxonomyId = ORGANIC_FERTILIZER_TAXONOMY_ID
    '                    nitrogenPerMil = dtFertilizzanti(0)("N") * 10
    '                    phosphorusPerMil = dtFertilizzanti(0)("P2O5") * 10
    '                    potassiumPerMil = dtFertilizzanti(0)("K2O") * 10
    '                    warehouse.organicFertilizers.Add(New WarehouseStockOrganicFertilizer(storageDate, quantityLastModifiedDate, initialQuantity, initialMeasureUnit, quantity, measureUnit, productTaxonomyId, nitrogenPerMil, phosphorusPerMil, potassiumPerMil, fertilizerDescription))

    '                Else

    '                    ' Per ora settata da codice
    '                    productTaxonomyId = CHEMICAL_FERTILIZER_TAXONOMY_ID
    '                    productRegistrationNumber = ProCod
    '                    warehouse.chemicalFertilizers.Add(New WarehouseStockChemicalFertilizer(storageDate, quantityLastModifiedDate, initialQuantity, initialMeasureUnit, quantity, measureUnit, productTaxonomyId, productRegistrationNumber, fertilizerDescription))

    '                End If

    '            End If
    '    End Select

    'End Sub

    Private Function GetProductMeasureUnit(ByVal unitaDiMisura As AgronicaCoreModelsSTD.metaschema.UnitaDiMisura) As String

        Dim productMeasureUnit As String = String.Empty

        If IsNothing(DT_UdM) Then
            Dim objMetaschemaDAL As New AgronicaCoreMetaSchemaDAL.Codifica_UnitaMisura_SistemiEsterni_R

            DT_UdM = objMetaschemaDAL.Leggi(0, Sistema_Cod_Agea, "", 0, Now, Now, "", "", ObjParametri_Server)
        End If

        Dim Udm_Cod_Base As Integer = 0

        If Not IsNothing(unitaDiMisura) AndAlso unitaDiMisura.codice > 0 Then
            Udm_Cod_Base = unitaDiMisura.codice
        End If

        If Udm_Cod_Base > 0 Then
            Dim Dr As DataRow() = DT_UdM.Select("UDM_Cod = " & Udm_Cod_Base)

            If Not IsNothing(Dr) AndAlso Dr.Count = 1 Then
                productMeasureUnit = Dr(0)("UDM_Cod_Esterno")
            Else
                'Se l'unità di misura base è diversa da kg o litri (esempio caso CONFUSIONE\DISORIENTAMENTO SESSUALE) allora imposto come unità di misura U
                Dr = DT_UdM.Select("UDM_Cod = " & enum_UnitaMisura.UNITA)

                If Not IsNothing(Dr) AndAlso Dr.Count = 1 Then
                    productMeasureUnit = Dr(0)("UDM_Cod_Esterno")
                End If
            End If
        End If

        If String.IsNullOrEmpty(productMeasureUnit) Then
            productMeasureUnit = Nothing
        End If

        Return productMeasureUnit
    End Function

    Private Function GetProductAndWaterQuantityFromPlot(ByVal operationId As String, ByVal dettaglio As Risorsa, ByVal Dr_Appezzamento_Impianto As DataRow,
                                                        ByVal ImpiantoPK As Impianto.PK, ByVal superficieTrattataImpianto As Decimal, ByVal risorsaAcqua As RisorsaAcqua) As Obj_Qta_Prodotto

        Dim obj_Qta_Prodotto_X_Impianto As Obj_Qta_Prodotto = Nothing

        If Not IsNothing(dettaglio) Then

            Dim objMetaschemaDAL As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

            Dim Pro_Cod As Integer = 0

            Dim Dose_Ha As Decimal = 0

            Dim waterQuantity As Decimal = 0

            Dim productQuantity As Decimal = 0

            Dim actionArea As Decimal = 0

            Dim Udm_Cod As Integer = 0

            Dim Udm_Cod_Base As Integer = 0

            Select Case dettaglio.classType
                Case ClassType.DettaglioTrattamento
                    Dim dettaglioTrattamento As dettagli.DettaglioTrattamento = CType(dettaglio, dettagli.DettaglioTrattamento)
                    Pro_Cod = dettaglioTrattamento.prodotto.codice
                    Dose_Ha = dettaglioTrattamento.doseHaReale

                    If Not IsNothing(dettaglioTrattamento.unitaDiMisura) Then
                        Udm_Cod_Base = dettaglioTrattamento.unitaDiMisura.codice
                    End If

                    If Not IsNothing(dettaglioTrattamento.unitaDiMisuraIndicata) Then
                        Udm_Cod = dettaglioTrattamento.unitaDiMisuraIndicata.codice
                    End If

                Case ClassType.DettaglioFertilizzazione
                    Dim dettaglioFertilizzazione As dettagli.DettaglioFertilizzazione = CType(dettaglio, dettagli.DettaglioFertilizzazione)
                    Pro_Cod = dettaglioFertilizzazione.prodotto.codice
                    Dose_Ha = dettaglioFertilizzazione.doseHaReale


                    If Not IsNothing(dettaglioFertilizzazione.unitaDiMisura) Then
                        Udm_Cod_Base = dettaglioFertilizzazione.unitaDiMisura.codice
                    End If

                    If Not IsNothing(dettaglioFertilizzazione.unitaDiMisuraIndicata) Then
                        Udm_Cod = dettaglioFertilizzazione.unitaDiMisuraIndicata.codice
                    End If

            End Select

            If Not ProductAndWaterQuantity_HA Then

                If Not IsNothing(List_Qta) AndAlso List_Qta.Count > 0 Then

                    Dim Qta_Prodotto_X_Impianto = List_Qta.Find(Function(prodotto_x_impianto) prodotto_x_impianto.Piva = Dr_Appezzamento_Impianto("Piva") AndAlso
                                                                                            prodotto_x_impianto.Sa_Cod = Dr_Appezzamento_Impianto("Sa_Cod") AndAlso
                                                                                             prodotto_x_impianto.Appezza = Dr_Appezzamento_Impianto("Appezza") AndAlso
                                                                                             prodotto_x_impianto.Id_Destinazione = Dr_Appezzamento_Impianto("ID_REG") AndAlso
                                                                                             prodotto_x_impianto.Pro_Cod.ToString() = Pro_Cod)

                    If Not IsNothing(Qta_Prodotto_X_Impianto) Then
                        waterQuantity = Qta_Prodotto_X_Impianto.Qta_Acqua
                        productQuantity = Qta_Prodotto_X_Impianto.Qta_Prodotto
                        actionArea = Qta_Prodotto_X_Impianto.Sup_Impianto
                    End If
                End If

            Else

                actionArea = superficieTrattataImpianto

                If Not IsNothing(risorsaAcqua) Then

                    If risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE Then

                        If Not IsNothing(Dict_Superficie_Trattata_X_Operazione) AndAlso Dict_Superficie_Trattata_X_Operazione.Count > 0 Then

                            Dim superficie_trattata_totale As Decimal = Dict_Superficie_Trattata_X_Operazione.Item(operationId)

                            If superficie_trattata_totale > 0 Then
                                waterQuantity = risorsaAcqua.acqua / superficie_trattata_totale
                            End If
                        End If

                    ElseIf risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.HA Then
                        waterQuantity = risorsaAcqua.acqua
                    End If

                End If

                If Udm_Cod > 0 AndAlso Udm_Cod_Base > 0 Then
                    If Udm_Cod = Udm_Cod_Base Then
                        productQuantity = Dose_Ha
                    Else
                        productQuantity = objMetaschemaDAL.Converti(Udm_Cod, Dose_Ha, Udm_Cod_Base)
                    End If
                End If

            End If

            'Converto l'acqua da inviare da Ettolitri (come è salvata su db) in Litri
            'La Qta_Prodotto è sempre espressa in Kg o Litri

            obj_Qta_Prodotto_X_Impianto = New Obj_Qta_Prodotto With {
                .waterQuantity = IIf(waterQuantity = 0, 0, objMetaschemaDAL.Converti(enum_UnitaMisura.Ettolitro, waterQuantity, enum_UnitaMisura.Litri)),
                .productQuantity = productQuantity,
                .actionArea = actionArea
            }

        End If


        Return obj_Qta_Prodotto_X_Impianto

    End Function

    Private Function ExportBundle(ByVal bundle As Bundle) As Integer

        Dim bundle_Id As Integer = 0

        If IsNothing(checkBundle.List_ErroriGias) OrElse checkBundle.List_ErroriGias.Count = 0 Then
            Dim utility As New Utility

            Dim str_bundle As String = JsonConvert.SerializeObject(bundle, tzh)

            Dim endpoint As String = "/api/bundle"

            bundle_Id = utility.CallHubAgeaPOST(str_bundle, endpoint, ObjParametri_Super_Server)

        Else

            'Scrivo su ElasticSearch solo gli ErroriGias bloccanti non i Warning
            Dim List_ErroriGias_Bloccanti = checkBundle.List_ErroriGias.Where(Function(errore) errore.severity = ErroreGias_Severity.Bloccante).ToList()

            If Not IsNothing(List_ErroriGias_Bloccanti) AndAlso List_ErroriGias_Bloccanti.Count > 0 Then

                Dim TuttiMessaggi As String = String.Join(" , ", (From errore In List_ErroriGias_Bloccanti Select errore.messaggio).ToList())

                Throw New InfoExceptionGias2AgeaHub(TuttiMessaggi)

            End If
        End If

        Return bundle_Id
    End Function

    Private Function Create_operationId(ByVal Piva As String, ByVal Id_Agenda As Integer) As String
        Return String.Join("_", Piva, Id_Agenda)
    End Function

    Private Function GetPersonType(ByVal risorsaUmana As RisorseUmane) As PersonType
        Dim result = AgronicaCoreDTOStd.InData.Agea.PersonType.PersonaFisica
        If risorsaUmana.contatto.fisico_Giuridico = PERSONA_GIURIDICA Then
            result = AgronicaCoreDTOStd.InData.Agea.PersonType.PersonaGiuridica
        End If
        Return result
    End Function

    Private Function MapProductTreatments(ByVal operationId As String, ByVal warehouse As Warehouse,
                                          ByVal prodottoDaTrattareCdC As ProdottoDaTrattareCDC, ByVal Anno As Integer, ByVal data_operazione As Date,
                                          ByVal ora_operazione As Date, ByVal Qta_Prodotto_Trattata_Totale As Decimal, ByVal risorse As List(Of risorse.Risorsa)) As List(Of ProductTreatment)

        Dim productTreatments As New List(Of ProductTreatment)

        Dim risorseDettaglioTrattamento = GetDettagliTrattamentoFromAttivita(risorse)

        If Not IsNothing(risorseDettaglioTrattamento) AndAlso risorseDettaglioTrattamento.Count > 0 Then

            Dim risorseMacchine As List(Of RisorsaMacchina) = GetRisorseMacchineFromAttivita(risorse)

            Dim risorsePersone As List(Of RisorsaPersona) = GetRisorsePersoneFromAttivita(risorse)

            Dim risorsaAcqua As RisorsaAcqua = GetRisorsaAcquaFromAttivita(risorse)

            Dim DT_Materie_Prime As DataTable = Nothing

            For Each risorsaDettaglioTrattamento In risorseDettaglioTrattamento

                If checkBundle.CheckIfProductIsExportable(risorsaDettaglioTrattamento) Then
                    productTreatments.Add(MapProductTreatment(operationId, warehouse, prodottoDaTrattareCdC, Anno, data_operazione, ora_operazione,
                                                              Qta_Prodotto_Trattata_Totale, risorsaDettaglioTrattamento,
                                                              risorseMacchine, risorsePersone, risorsaAcqua, DT_Materie_Prime))
                End If
            Next
        End If

        Return productTreatments
    End Function

    Private Function MapProductTreatment(ByVal operationId As String, ByVal warehouse As Warehouse, ByVal prodottoDaTrattareCdC As ProdottoDaTrattareCDC,
                                         ByVal Anno As Integer, ByVal data_operazione As Date, ByVal ora_operazione As Date,
                                         ByVal Qta_Prodotto_Trattata_Totale As Decimal, ByVal risorsaDettaglioTrattamento As dettagli.DettaglioTrattamento,
                                         ByVal risorseMacchine As List(Of risorse.RisorsaMacchina),
                                         ByVal risorsePersone As List(Of risorse.RisorsaPersona), ByVal risorsaAcqua As RisorsaAcqua,
                                         ByRef DT_Materie_Prime As DataTable) As ProductTreatment

        Dim productTreatment As New ProductTreatment(operationId, warehouse.address, warehouse.municipality, warehouse.foglio,
                                                     warehouse.particella, warehouse.subalterno, warehouse.georeferencing, warehouse.capacity)

        productTreatment.agriculturalCommodity = GetAgricolturalCommodity(prodottoDaTrattareCdC, DT_Materie_Prime)

        Dim obj_qta_prodotto_x_fabbricato = GetProductAndWaterQuantityFromWarehouse(operationId, risorsaDettaglioTrattamento, Qta_Prodotto_Trattata_Totale, prodottoDaTrattareCdC, risorsaAcqua)

        If Not IsNothing(obj_qta_prodotto_x_fabbricato) Then
            productTreatment.quantity = obj_qta_prodotto_x_fabbricato.productQuantity

            productTreatment.targetProductQuantity = obj_qta_prodotto_x_fabbricato.actionArea
        Else
            productTreatment.quantity = 0

            productTreatment.targetProductQuantity = 0
        End If

        productTreatment.eventDate = GetDateTimeStr(data_operazione, ora_operazione)

        productTreatment.productRegistrationNumber = risorsaDettaglioTrattamento.prodotto.codice.ToString()

        productTreatment.measureUnit = GetProductMeasureUnit(risorsaDettaglioTrattamento.unitaDiMisura)

        productTreatment.targetProductMeasureUnit = GetProductMeasureUnit(prodottoDaTrattareCdC.giacenzaMagazzino.UdM)

        productTreatment.adversityName = GetAdversityName(risorsaDettaglioTrattamento.avversitaGruppo)

        productTreatment.Workers = MapWorkers(risorsePersone, Anno)

        productTreatment.equipment = MapEquipments(risorseMacchine)

        productTreatment.treatmentApplicationMode = GetTreatmentApplicationMode(productTreatment.equipment)

        Return productTreatment
    End Function

    Private Function GetAdversityName(ByVal avversitaGruppo As AgronicaCoreModelsSTD.metaschema.avversita.AvversitaGruppo) As String
        Dim adversityName As String = "NO_ADVERSITY"

        If Not IsNothing(DT_Avversita) Then
            If Not IsNothing(avversitaGruppo) Then

                Select Case avversitaGruppo.classType
                    Case ClassType.Avversita

                        Dim Dr = DT_Avversita.Select("Av_Cod = " & avversitaGruppo.codice)

                        If Not IsNothing(Dr) AndAlso Dr.Count > 0 Then
                            adversityName = Dr(0)("Av_Des_Lat")
                        End If
                    Case ClassType.GruppoAvversita

                        Dim Dr = DT_Avversita.Select("Av_Gru = " & avversitaGruppo.codice)

                        If Not IsNothing(Dr) AndAlso Dr.Count > 0 Then
                            adversityName = Dr(0)("Av_Gru_Des_Lat")
                        End If
                End Select

            End If
        End If

        Return adversityName
    End Function

    Private Function GetProductAndWaterQuantityFromWarehouse(ByVal operationId As String, ByVal dettaglio As Risorsa, ByVal Qta_Prodotto_Trattata_Totale As Decimal,
                                                             ByVal prodottoDaTrattareCdC As ProdottoDaTrattareCDC, ByVal risorsaAcqua As RisorsaAcqua) As Obj_Qta_Prodotto

        Dim obj_Qta_Prodotto_X_Fabbricato As Obj_Qta_Prodotto = Nothing

        If Not IsNothing(dettaglio) Then

            Dim objMetaschemaDAL As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R

            Dim Pro_Cod As Integer = 0

            Dim Dose_Ha As Decimal = 0

            Dim Qta_Totale As Decimal = 0

            Dim waterQuantity As Decimal = 0

            Dim productQuantity As Decimal = 0

            Dim actionArea As Decimal = 0

            Dim Udm_Cod As Integer = 0

            Dim Udm_Cod_Base As Integer = 0


            Select Case dettaglio.classType
                Case ClassType.DettaglioTrattamento
                    Dim dettaglioTrattamento As dettagli.DettaglioTrattamento = CType(dettaglio, dettagli.DettaglioTrattamento)
                    Pro_Cod = dettaglioTrattamento.prodotto.codice
                    Dose_Ha = dettaglioTrattamento.doseHaReale
                    Qta_Totale = dettaglioTrattamento.quantitaTotaleReale

                    If Not IsNothing(dettaglioTrattamento.unitaDiMisura) Then
                        Udm_Cod_Base = dettaglioTrattamento.unitaDiMisura.codice
                    End If

                    If Not IsNothing(dettaglioTrattamento.unitaDiMisuraIndicata) Then
                        Udm_Cod = dettaglioTrattamento.unitaDiMisuraIndicata.codice
                    End If

            End Select

            If Not IsNothing(prodottoDaTrattareCdC) Then

                If Not IsNothing(prodottoDaTrattareCdC.giacenzaMagazzino) AndAlso Not IsNothing(prodottoDaTrattareCdC.giacenzaMagazzino.UdM) Then
                    If prodottoDaTrattareCdC.giacenzaMagazzino.UdM.codice = enum_UnitaMisura.KG Then
                        'QtaTrattata è convertita in quintali io la devo esportare in kg
                        actionArea = prodottoDaTrattareCdC.qtaTrattata * 100
                    Else
                        actionArea = prodottoDaTrattareCdC.qtaTrattata
                    End If
                End If

            End If

            If Not ProductAndWaterQuantity_HA Then

                productQuantity = Qta_Totale

                If Not IsNothing(risorsaAcqua) Then
                    Select Case risorsaAcqua.doseAcqua
                        Case RisorsaAcqua.TipoDoseAcqua.TOTALE
                            waterQuantity = risorsaAcqua.acqua
                        Case RisorsaAcqua.TipoDoseAcqua.HA
                            waterQuantity = risorsaAcqua.acqua * prodottoDaTrattareCdC.qtaTrattata
                    End Select
                End If

            Else

                productQuantity = Dose_Ha

                If Not IsNothing(risorsaAcqua) Then

                    If risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.TOTALE Then

                        waterQuantity = risorsaAcqua.acqua / Qta_Prodotto_Trattata_Totale

                    ElseIf risorsaAcqua.doseAcqua = RisorsaAcqua.TipoDoseAcqua.HA Then
                        waterQuantity = risorsaAcqua.acqua
                    End If

                End If

                If Udm_Cod > 0 AndAlso Udm_Cod_Base > 0 Then
                    If Udm_Cod = Udm_Cod_Base Then
                        productQuantity = Dose_Ha
                    Else
                        productQuantity = objMetaschemaDAL.Converti(Udm_Cod, Dose_Ha, Udm_Cod_Base)
                    End If
                End If

            End If

            'Converto l'acqua da inviare da Ettolitri (come è salvata su db) in Litri
            'La Qta_Prodotto è sempre espressa in Kg o Litri

            obj_Qta_Prodotto_X_Fabbricato = New Obj_Qta_Prodotto With {
                .waterQuantity = IIf(waterQuantity = 0, 0, objMetaschemaDAL.Converti(enum_UnitaMisura.Ettolitro, waterQuantity, enum_UnitaMisura.Litri)),
                .productQuantity = productQuantity,
                .actionArea = actionArea
            }

        End If


        Return obj_Qta_Prodotto_X_Fabbricato

    End Function

    Private Function GetTreatmentApplicationMode(ByVal equipments As List(Of Equipment)) As String

        Dim treatmentApplicationMode As String = Treatment_Modality.Manual

        If Not IsNothing(equipments) AndAlso equipments.Count > 0 Then
            treatmentApplicationMode = Treatment_Modality.Machinery
        End If

        Return treatmentApplicationMode

    End Function

    Private Function GetDettagliTrattamentoFromAttivita(ByVal risorse As List(Of Risorsa)) As List(Of DettaglioTrattamento)

        Dim risorseDettaglioTrattamento As New List(Of DettaglioTrattamento)

        If Not IsNothing(risorse) AndAlso risorse.FindIndex(Function(r) r.classType = ClassType.DettaglioTrattamento) > -1 Then
            risorseDettaglioTrattamento = risorse.FindAll(Function(r) r.classType = ClassType.DettaglioTrattamento).ConvertAll(Function(r) CType(r, dettagli.DettaglioTrattamento))
        End If

        Return risorseDettaglioTrattamento
    End Function

    Private Function GetRisorseMacchineFromAttivita(ByVal risorse As List(Of Risorsa)) As List(Of RisorsaMacchina)
        Dim risorseMacchine As New List(Of RisorsaMacchina)

        If Not IsNothing(risorse) AndAlso risorse.FindIndex(Function(r) r.classType = ClassType.RisorsaMacchina) > -1 Then
            risorseMacchine = risorse.FindAll(Function(r) r.classType = ClassType.RisorsaMacchina).ConvertAll(Function(r) CType(r, RisorsaMacchina))
        End If

        Return risorseMacchine
    End Function

    Private Function GetRisorsePersoneFromAttivita(ByVal risorse As List(Of Risorsa)) As List(Of RisorsaPersona)
        Dim risorsePersone As New List(Of RisorsaPersona)

        If Not IsNothing(risorse) AndAlso risorse.FindIndex(Function(r) r.classType = ClassType.RisorsaPersona) > -1 Then
            risorsePersone = risorse.FindAll(Function(r) r.classType = ClassType.RisorsaPersona).ConvertAll(Function(r) CType(r, RisorsaPersona))
        End If

        Return risorsePersone
    End Function

    Private Function GetRisorsaAcquaFromAttivita(ByVal risorse As List(Of Risorsa)) As RisorsaAcqua
        Dim risorsaAcqua As risorse.RisorsaAcqua = Nothing

        If Not IsNothing(risorse) AndAlso risorse.FindIndex(Function(r) r.classType = ClassType.RisorsaAcqua) > -1 Then
            risorsaAcqua = CType(risorse.Find(Function(r) r.classType = ClassType.RisorsaAcqua), risorse.RisorsaAcqua)
        End If

        Return risorsaAcqua
    End Function

    Private Function MapSeedTreatments(ByVal operationId As String, ByVal warehouse As Warehouse,
                                      ByVal prodottoDaTrattareCdC As ProdottoDaTrattareCDC, ByVal Anno As Integer, ByVal data_operazione As Date,
                                      ByVal ora_operazione As Date, ByVal Qta_Prodotto_Trattata_Totale As Decimal, ByVal risorse As List(Of risorse.Risorsa)) As List(Of SeedTreatment)

        Dim seedTreatments As New List(Of SeedTreatment)

        Dim risorseDettaglioTrattamento = GetDettagliTrattamentoFromAttivita(risorse)

        If Not IsNothing(risorseDettaglioTrattamento) AndAlso risorseDettaglioTrattamento.Count > 0 Then

            Dim risorsePersone As List(Of RisorsaPersona) = GetRisorsePersoneFromAttivita(risorse)

            Dim risorsaAcqua As RisorsaAcqua = GetRisorsaAcquaFromAttivita(risorse)

            Dim DT_Materie_Prime As DataTable = Nothing

            Dim DT_Codifica_SPV As DataTable = Nothing

            For Each risorsaDettaglioTrattamento In risorseDettaglioTrattamento

                If checkBundle.CheckIfProductIsExportable(risorsaDettaglioTrattamento) Then


                    seedTreatments.Add(MapSeedTreatment(operationId, warehouse, prodottoDaTrattareCdC, Anno, data_operazione, ora_operazione,
                                                        Qta_Prodotto_Trattata_Totale, risorsaDettaglioTrattamento,
                                                              risorsePersone, risorsaAcqua, DT_Materie_Prime, DT_Codifica_SPV))
                End If
            Next
        End If

        Return seedTreatments
    End Function

    Private Function MapSeedTreatment(ByVal operationId As String, ByVal warehouse As Warehouse, ByVal prodottoDaTrattareCdC As ProdottoDaTrattareCDC,
                                         ByVal Anno As Integer, ByVal data_operazione As Date, ByVal ora_operazione As Date,
                                         ByVal Qta_Prodotto_Trattata_Totale As Decimal, ByVal risorsaDettaglioTrattamento As dettagli.DettaglioTrattamento,
                                         ByVal risorsePersone As List(Of risorse.RisorsaPersona), ByVal risorsaAcqua As RisorsaAcqua,
                                         ByRef DT_Materie_Prime As DataTable, ByRef DT_Codifica_SPV As DataTable) As SeedTreatment

        Dim seedTreatment As New SeedTreatment(operationId, warehouse.address, warehouse.municipality, warehouse.foglio,
                                                     warehouse.particella, warehouse.subalterno, warehouse.georeferencing, warehouse.capacity)

        Dim obj_qta_prodotto_x_fabbricato = GetProductAndWaterQuantityFromWarehouse(operationId, risorsaDettaglioTrattamento, Qta_Prodotto_Trattata_Totale, prodottoDaTrattareCdC, risorsaAcqua)

        If Not IsNothing(obj_qta_prodotto_x_fabbricato) Then
            seedTreatment.quantity = obj_qta_prodotto_x_fabbricato.productQuantity

            seedTreatment.seedQuantity = obj_qta_prodotto_x_fabbricato.actionArea
        Else
            seedTreatment.quantity = 0

            seedTreatment.seedQuantity = 0
        End If

        seedTreatment.seedType = GetSeedType(prodottoDaTrattareCdC, DT_Materie_Prime, DT_Codifica_SPV)

        seedTreatment.eventDate = GetDateTimeStr(data_operazione, ora_operazione)

        seedTreatment.productRegistrationNumber = risorsaDettaglioTrattamento.prodotto.codice.ToString()

        seedTreatment.measureUnit = GetProductMeasureUnit(risorsaDettaglioTrattamento.unitaDiMisura)

        seedTreatment.seedMeasureUnit = GetProductMeasureUnit(prodottoDaTrattareCdC.giacenzaMagazzino.UdM)

        seedTreatment.adversityName = GetAdversityName(risorsaDettaglioTrattamento.avversitaGruppo)

        seedTreatment.Workers = MapWorkers(risorsePersone, Anno)

        Return seedTreatment

    End Function

    Private Function GetSeedType(ByVal prodottoDaTrattareCdC As ProdottoDaTrattareCDC,
                                 ByRef DT_Materie_Prime As DataTable, ByRef DT_Codifica_SPV As DataTable) As String

        Dim seedType As String = String.Empty

        If Not IsNothing(prodottoDaTrattareCdC) AndAlso Not IsNothing(prodottoDaTrattareCdC.giacenzaMagazzino) AndAlso Not IsNothing(prodottoDaTrattareCdC.giacenzaMagazzino.Prodotto) Then
            Select Case prodottoDaTrattareCdC.giacenzaMagazzino.Prodotto.elemCod
                Case SEMENTI

                    Dim Veg_Cod As Integer = 0

                    If Not IsNothing(prodottoDaTrattareCdC.giacenzaMagazzino.Prodotto.specie) Then
                        Veg_Cod = prodottoDaTrattareCdC.giacenzaMagazzino.Prodotto.specie.codice
                    End If

                    If Veg_Cod = 0 Then

                        Dim Mat_Cod As Integer = prodottoDaTrattareCdC.giacenzaMagazzino.Prodotto.codice

                        If IsNothing(DT_Materie_Prime) Then

                            Dim objMaterie As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

                            DT_Materie_Prime = objMaterie.Leggi2("", SEMENTI, 0, "", 0, "",
                                                                    True, xSelezioneVariabile:=enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                    "", "", ObjParametri_Server)
                        End If

                        If Not IsNothing(DT_Materie_Prime) AndAlso DT_Materie_Prime.Rows.Count > 0 Then

                            Dim Dr As DataRow() = DT_Materie_Prime.Select("Mat_Cod = " & Mat_Cod)

                            If Not IsNothing(Dr) AndAlso Dr.Length = 1 Then
                                Veg_Cod = Dr(0)("Veg_Cod")
                            End If


                        End If
                    End If

                    If Veg_Cod > 0 Then

                        If IsNothing(DT_Codifica_SPV) Then

                            Dim objCodifica_SpecieVegetali As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Agea_2015_2020_R

                            DT_Codifica_SPV = objCodifica_SpecieVegetali.leggi(ObjParametri_Server, "", "",
                                                                                "", "", "", "", "", 0, 0,
                                                                                0, 0, 0, 0, 0, 0)
                        End If

                        If Not IsNothing(DT_Codifica_SPV) AndAlso DT_Codifica_SPV.Rows.Count > 0 Then

                            Dim Dr As DataRow() = DT_Codifica_SPV.Select("Veg_Cod = " & Veg_Cod)

                            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then

                                Dim Dt = Dr.CopyToDataTable()

                                Dt.DefaultView.Sort = "Occupazione_Cod"

                                seedType = Dt(0)("Occupazione_Cod")
                            End If
                        End If
                    End If

            End Select
        End If

        If String.IsNullOrEmpty(seedType) Then
            seedType = Nothing
        End If

        Return seedType

    End Function

    Private Function GetAgricolturalCommodity(ByVal prodottoDaTrattareCDC As ProdottoDaTrattareCDC, ByRef DT_Materie_Prime As DataTable) As String

        Dim agriculturalCommodity As String = String.Empty

        Dim Mat_Cod As Integer = 0

        Dim Mat_Des As String = String.Empty

        If Not IsNothing(prodottoDaTrattareCDC) AndAlso Not IsNothing(prodottoDaTrattareCDC.giacenzaMagazzino) AndAlso
                Not IsNothing(prodottoDaTrattareCDC.giacenzaMagazzino.Prodotto) Then

            Mat_Cod = prodottoDaTrattareCDC.giacenzaMagazzino.Prodotto.codice

            Mat_Des = prodottoDaTrattareCDC.giacenzaMagazzino.Prodotto.descrizione

            If String.IsNullOrEmpty(Mat_Des) Then

                If IsNothing(DT_Materie_Prime) Then

                    Dim objMaterie As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

                    Dim Elem_Cod As Integer = prodottoDaTrattareCDC.giacenzaMagazzino.Prodotto.elemCod

                    DT_Materie_Prime = objMaterie.Leggi2("", Elem_Cod, 0, "", 0, "", True,
                                                        xSelezioneVariabile:=enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Server)
                End If

                If Not IsNothing(DT_Materie_Prime) AndAlso DT_Materie_Prime.Rows.Count > 0 AndAlso Mat_Cod > 0 Then

                    Dim Dr As DataRow() = DT_Materie_Prime.Select("Mat_Cod = " & Mat_Cod)

                    If Not IsNothing(Dr) AndAlso Dr.Length = 1 Then
                        Mat_Des = Dr(0)("Mat_Des")
                    End If
                End If
            End If
        End If

        If Mat_Cod > 0 AndAlso Not String.IsNullOrEmpty(Mat_Des) Then
            agriculturalCommodity = Mat_Des & " [" & Mat_Cod & "]"
        End If

        Return agriculturalCommodity
    End Function

    Private Function GetDateTimeStr(ByVal data As Date,
                                    Optional ByVal ora As DateTime = AGRODATAINIZIO) As String

        Dim DateTimeStr As String = String.Empty

        If ora <> AGRODATAINIZIO Then
            DateTimeStr = New DateTime(data.Year, data.Month, data.Day,
                                        ora.Hour, ora.Minute, ora.Second).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        Else
            DateTimeStr = data.ToString("yyyy-MM-dd")
        End If

        Return DateTimeStr
    End Function

    Private Function GetApplicationTypeCode(ByVal modalitaApplicazione As BaseCodeDescr) As String

        Dim applicationTypeCode As String = String.Empty

        If Not IsNothing(modalitaApplicazione) Then

            If modalitaApplicazione.descrizione = String.Empty Then

                'Le modalita di Applicazione Globali sono quelle che hanno il codice negativo
                If modalitaApplicazione.codice < 0 Then

                    If IsNothing(DT_Modalita_Applicazione_Globali) Then
                        Dim obj As New AgronicaCoreMetaSchemaDAL.Modalita_Applicazione_Globali

                        DT_Modalita_Applicazione_Globali = obj.Leggi(0, "", "", "", ObjParametri_Server)
                    End If

                    If Not IsNothing(DT_Modalita_Applicazione_Globali) AndAlso DT_Modalita_Applicazione_Globali.Rows.Count > 0 Then

                        Dim Dr As DataRow() = DT_Modalita_Applicazione_Globali.Select("Codice = " & modalitaApplicazione.codice)

                        If Not IsNothing(Dr) AndAlso Dr.Length = 1 Then
                            applicationTypeCode = Dr(0)("AGEA_COD")
                        End If
                    End If
                End If
            Else
                applicationTypeCode = modalitaApplicazione.descrizione
            End If
        End If

        If String.IsNullOrEmpty(applicationTypeCode) Then
            applicationTypeCode = Nothing
        End If

        Return applicationTypeCode

    End Function

    Private Function GetWarehouse_description(ByVal Sa_Cod As Integer, ByVal Fabbricato_Cod As Integer, ByVal Fabbricato_Des As String) As String
        Return String.Join("_", Sa_Cod, Fabbricato_Cod) & "-" & Fabbricato_Des
    End Function

    Private Function GetListLavCod() As List(Of Integer)

        Dim List_Lav_Cod = New List(Of Integer)({LAVCOD_FASI_FENOLOGICHE,
                                        LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                                        LAVCOD_DISERBO,
                                        LAVCOD_DISSECCAMENTO,
                                        LAVCOD_GEODISINFESTAZIONE,
                                        LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                                        LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                                        LAVCOD_IRRIGAZIONE,
                                        LAVCOD_DISTRIBUZIONE_CONCIME,
                                        LAVCOD_SARCHIATURA_CONCIMAZIONE,
                                        LAVCOD_DISTRIBUZIONE_AMMENDANTI,
                                        LAVCOD_CONCIMAZIONE_FOGLIARE,
                                        LAVCOD_FERTIRRIGAZIONE,
                                        LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                                        LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                                        LAVCOD_CONCIA_SEME})

        'Le Lavorazioni da inviare (operazioni senza prodotto) le leggo dalla tabella Codifica_Operazioni_SistemiEsterni con Sistema_Cod = 5 (AGEA), rientra anche la 
        'semina che è da inviare senza prodotti
        Dim objMetaschemaDAL As New AgronicaCoreMetaSchemaDAL.Codifica_Operazioni_SistemiEsterni_R

        DT_Lavorazioni = objMetaschemaDAL.Leggi(0, Sistema_Cod_Agea, "", 0, Now, Now, "", "", ObjParametri_Server)

        If Not IsNothing(DT_Lavorazioni) AndAlso DT_Lavorazioni.Rows.Count > 0 Then
            For Each Dr In DT_Lavorazioni.Rows
                List_Lav_Cod.Add(Dr("Lav_Cod"))
            Next
        End If

        Return List_Lav_Cod
    End Function


    Private Function GetDictAttivita(ByVal Piva As String) As Dictionary(Of String, Attivita)

        Dim Dict_Attivita As New Dictionary(Of String, Attivita)

        Dim xFiltroAggiuntivo As String = "Agenda.Lav_Cod IN (" & String.Join(",", GetListLavCod()) & ") AND Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine, False) & " AND Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio, False)

        Dim objAgenda As New AgronicaCoreContabDAL.Agenda_R

        Dim DT_Agende As DataTable = objAgenda.Leggi(Piva, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, "", ObjParametri_Server)

        If Not IsNothing(DT_Agende) AndAlso DT_Agende.Rows.Count > 0 Then
            For Each Dr In DT_Agende.Rows
                Dim chiave As String = Create_operationId(Dr("Piva"), Dr("Id_Agenda"))
                If Not Dict_Attivita.ContainsKey(chiave) Then
                    Dim objAgenda_Operazione As New Agenda_Operazione_Helper()
                    Dim agenda As Operazione_Agenda = objAgenda_Operazione.Leggi(Dr("Piva"), 0, Dr("Id_Agenda"), 0, ObjParametri_Server)

                    If Not IsNothing(agenda) AndAlso agenda.Id_Agenda > 0 Then

                        Dim agendaToActivity As New AgronicaCoreMapper.AgendaToAttivita()

                        Dim attivita = agendaToActivity.AgendaSuAttivita(agenda, False, ObjParametri_Super_Server, ObjParametri_Server, ObjParametri_Utenti)

                        If Not IsNothing(attivita) Then
                            Dict_Attivita.Add(chiave, attivita)
                        End If

                    End If
                End If

            Next
        End If

        Return Dict_Attivita
    End Function

End Class
