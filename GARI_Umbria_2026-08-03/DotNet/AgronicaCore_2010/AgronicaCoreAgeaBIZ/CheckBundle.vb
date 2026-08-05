Imports System.Reflection
Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData.Agea
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.attivita.risorse
Imports AgronicaCoreModelsSTD.exceptions

Public Class CheckBundle

    Private Enum ErrorType
        PlotsNotMapped
        NoOperations
        YearNotValid
        FiscalCodeNotValidWorker
        LicenseDateNotValidWorker
        NoCalibratioIDEquipment
        DateEndBeforeDateStartGrowthStage
        DateEndBeforeDateStartFarmingEvent
        DateEndBeforeDateStartPhytochemicalTreatment
        DateEndBeforeDateStartChemicalFertilization
        DateEndBeforeDateStartOrganicFertilization
        OperationOutOfScope
    End Enum

    Private Const MaxProCod As Integer = 1000000

    Private Const MinYear As Integer = 2000

    Private Const MaxYear As Integer = 2100

    Private MaxBBCHState As Integer = 9

    Private _erroriDaBypassare As New List(Of String)

    Public Property erroriDaBypassare() As List(Of String)
        Get
            Return _erroriDaBypassare
        End Get
        Set(ByVal value As List(Of String))
            _erroriDaBypassare = value
        End Set
    End Property

    Private _List_ErroriGias As New List(Of ErroreGias)

    Public Property List_ErroriGias() As List(Of ErroreGias)
        Get
            Return _List_ErroriGias
        End Get
        Set(ByVal value As List(Of ErroreGias))
            _List_ErroriGias = value
        End Set
    End Property

    Private PlotsError As New List(Of String)

    Public Sub CheckYear(ByVal Anno As Integer)

        Dim keyErroreGias As String = CreateKeyErroreGias(ErrorType.YearNotValid)

        If Not BypassCheck(keyErroreGias) Then
            If Anno < MinYear OrElse Anno > MaxYear Then

                Dim index = List_ErroriGias.FindIndex(Function(ErroreGias) CInt(ErroreGias.ex) = ErrorType.YearNotValid)

                If index = -1 Then
                    Dim msg = String.Format(My.Resources.AgronicaCoreAgeaBIZ.ScegliereUnAnnoCompresoTra, MinYear, MaxYear)
                    InsertErroreGias(msg, ErroreGias_Severity.Bloccante, keyErroreGias)
                End If

            End If
        End If

    End Sub

    Public Sub CheckPlot(ByVal plot As Plot, ByVal esercizioCdC As EsercizioCDC)

        Dim keyErroreGiasPlotsNotMapped As String = CreateKeyErroreGias(ErrorType.PlotsNotMapped)

        If Not BypassCheck(ErrorType.PlotsNotMapped) Then

            If String.IsNullOrWhiteSpace(plot.islandId) OrElse 
                String.IsNullOrWhiteSpace(plot.plotId) OrElse
                String.IsNullOrWhiteSpace(plot.pcgId) OrElse 
                String.IsNullOrWhiteSpace(plot.plantationId) OrElse
                String.IsNullOrWhiteSpace(plot.codiBarrScheVali) Then
                
                Dim pivaEsercizioCdC As String = esercizioCdC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva
                Dim saCodEsercizioCdC As Integer = esercizioCdC.esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice
                Dim appezzaEsercizioCdC As Integer = esercizioCdC.esercizio.impiantoPK.appezzamentoPK.codice
                Dim idRegEsercizioCdC As Integer = esercizioCdC.esercizio.impiantoPK.codice

                Dim key = "Piva " & PivaEsercizioCdC &
                          " SaCod " & saCodEsercizioCdC &
                          " Appezza " & appezzaEsercizioCdC & 
                          " IdReg " & idRegEsercizioCdC
                If Not PlotsError.Contains(key) Then
                    PlotsError.Add(key)
                End If

            End If
        End If

    End Sub

    Public Sub CheckOperation(ByVal Dict_Attivita As Dictionary(Of String, Attivita), ByVal Rag_Soc As String,
                              ByVal Anno As Integer)

        Dim keyErroreGias As String = CreateKeyErroreGias(ErrorType.NoOperations)

        If Not BypassCheck(keyErroreGias) Then
            If IsNothing(Dict_Attivita) OrElse Dict_Attivita.Count = 0 Then

                Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.NonSonoRegistrateOPperAzienda, Rag_Soc, Anno)

                InsertErroreGias(msg, ErroreGias_Severity.Bloccante, keyErroreGias)
            End If
        End If

    End Sub

    Public Function CheckIfProductIsExportable(ByVal risorsaDettaglioTrattamento As DettaglioTrattamento) As Boolean
        Dim result As Boolean = False

        If Not IsNothing(risorsaDettaglioTrattamento.prodotto) AndAlso risorsaDettaglioTrattamento.prodotto.codice < MaxProCod Then
            result = True
        End If

        Return result

    End Function

    Public Sub CheckWorkers(ByVal tp_Operatori As Tuple(Of List(Of RisorsaPersona), List(Of WorkerElement)))

        Dim keyErroreGiasFiscalCodeNotValidWorker As String = CreateKeyErroreGias(ErrorType.FiscalCodeNotValidWorker)

        Dim keyErroreGiasLicenseDateNotValidWorker As String = CreateKeyErroreGias(ErrorType.LicenseDateNotValidWorker)

        If Not BypassCheck(keyErroreGiasFiscalCodeNotValidWorker) AndAlso Not BypassCheck(keyErroreGiasLicenseDateNotValidWorker) Then

            If Not IsNothing(tp_Operatori) AndAlso tp_Operatori.Item1.Count = tp_Operatori.Item2.Count Then

                Dim _WorkerRegEx As String = "^[A-Za-z]{6}[0-9]{2}[A-Za-z]{1}[0-9]{2}[A-Za-z]{1}[0-9]{3}[A-Za-z]{1}$"

                Dim r As New Regex(_WorkerRegEx, RegexOptions.None, TimeSpan.FromSeconds(3))

                Dim IncorrectWorkersFiscalCode As New List(Of String)

                Dim IncorrectWorkersLicense As New List(Of String)

                For i As Integer = 0 To tp_Operatori.Item2.Count - 1

                    Dim fiscalCode As String = tp_Operatori.Item2(i).fiscalCode

                    Dim workerType As String = tp_Operatori.Item2(i).workerType

                    Dim descr As String = String.Empty

                    Dim index_risorsaPersona = tp_Operatori.Item1.FindIndex(Function(ris) Not IsNothing(ris.risorsaUmana) AndAlso
                                                                                                Not IsNothing(ris.risorsaUmana.contatto) AndAlso
                                                                                                ris.risorsaUmana.contatto.primaryKey.codice = fiscalCode)

                    If index_risorsaPersona > -1 Then
                        descr = String.Format("{0} {1} - {2}",
                                                  tp_Operatori.Item1(index_risorsaPersona).risorsaUmana.contatto.nome,
                                                    tp_Operatori.Item1(index_risorsaPersona).risorsaUmana.contatto.cognome,
                                                    fiscalCode)
                    End If

                    If Not CheckWorkerFiscalCode(workerType, fiscalCode, keyErroreGiasFiscalCodeNotValidWorker) Then
                        If Not String.IsNullOrEmpty(descr) AndAlso Not IncorrectWorkersFiscalCode.Contains(descr) Then
                            IncorrectWorkersFiscalCode.Add(descr)
                        End If
                    End If

                    If Not CheckWorkerLicense(tp_Operatori.Item2(i), keyErroreGiasLicenseDateNotValidWorker) Then
                        If Not IncorrectWorkersLicense.Contains(descr) Then
                            IncorrectWorkersLicense.Add(descr)
                        End If
                    End If

                Next

                If Not IsNothing(IncorrectWorkersFiscalCode) AndAlso IncorrectWorkersFiscalCode.Count > 0 Then
                    Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.CodiceFiscaleNonValido, String.Join(",", IncorrectWorkersFiscalCode))
                    InsertErroreGias(msg, ErroreGias_Severity.Warning, keyErroreGiasFiscalCodeNotValidWorker)
                End If


                If Not IsNothing(IncorrectWorkersLicense) AndAlso IncorrectWorkersLicense.Count > 0 Then
                    Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.DataRilascioSuccessivaDataScadenza, String.Join(",", IncorrectWorkersLicense))
                    InsertErroreGias(msg, ErroreGias_Severity.Warning, keyErroreGiasLicenseDateNotValidWorker)
                End If
            End If
        End If

    End Sub

    Private Function CheckWorkerLicense(ByVal worker As WorkerElement, ByVal keyErroreGiasLicenseDateNotValidWorker As String) As Boolean

        Dim valid As Boolean = True

        If Not BypassCheck(keyErroreGiasLicenseDateNotValidWorker) Then
            If worker.licenseReleaseDateToCheck > worker.licenseExpirationDateToCheck Then
                valid = False
            End If
        End If

        Return valid
    End Function

    Private Function CheckWorkerFiscalCode(ByVal worker_Type As String, ByVal fiscalCode As String, ByVal keyErroreGiasFiscalCodeNotValidWorker As String) As Boolean

        Dim valid As Boolean = True

        If Not BypassCheck(keyErroreGiasFiscalCodeNotValidWorker) Then
            Dim _WorkerRegEx As String = "^[A-Za-z]{6}[0-9]{2}[A-Za-z]{1}[0-9]{2}[A-Za-z]{1}[0-9]{3}[A-Za-z]{1}$"

            Dim r As New Regex(_WorkerRegEx, RegexOptions.None, TimeSpan.FromSeconds(3))

            Dim IncorrectWorkers As New List(Of String)

            If worker_Type = WorkerType.Internal AndAlso Not r.IsMatch(fiscalCode) Then
                valid = False
            End If
        End If

        Return valid
    End Function


    Public Sub CheckEquipments(ByVal tp_Macchine As Tuple(Of List(Of RisorsaMacchina), List(Of EquipmentElement)))

        Dim keyErroreNoCalibratioIDEquipment As String = CreateKeyErroreGias(ErrorType.NoCalibratioIDEquipment)

        If Not BypassCheck(keyErroreNoCalibratioIDEquipment) Then

            If Not IsNothing(tp_Macchine) Then

                Dim NoEquipmentsWithCalibrationID As New List(Of String)

                For i As Integer = 0 To tp_Macchine.Item2.Count - 1

                    Dim descr As String = tp_Macchine.Item2(i).description

                    If Not String.IsNullOrEmpty(descr) AndAlso Not CheckCalibrationIDEquipment(tp_Macchine.Item2(i).calibrationId, keyErroreNoCalibratioIDEquipment) Then
                        If Not NoEquipmentsWithCalibrationID.Contains(descr) Then
                            NoEquipmentsWithCalibrationID.Add(descr)
                        End If
                    End If
                Next

                If Not IsNothing(NoEquipmentsWithCalibrationID) AndAlso NoEquipmentsWithCalibrationID.Count > 0 Then
                    Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.NumeroCertificatoNonValorizzato, String.Join(",", NoEquipmentsWithCalibrationID))
                    InsertErroreGias(msg, ErroreGias_Severity.Warning, keyErroreNoCalibratioIDEquipment)
                End If
            End If
        End If
    End Sub


    Public Function CheckCalibrationIDEquipment(ByVal calibrationID As String, ByVal keyErroreGiasCalibrationID As String) As Boolean
        Dim valid As Boolean = True

        If Not BypassCheck(keyErroreGiasCalibrationID) Then
            If String.IsNullOrEmpty(calibrationID) OrElse String.IsNullOrWhiteSpace(calibrationID) Then
                valid = False
            End If
        End If

        Return valid
    End Function

    Public Sub InsertPlotsError()

        If Not IsNothing(PlotsError) AndAlso PlotsError.Count > 0 Then

            Dim keyErroreGias As String = CreateKeyErroreGias(ErrorType.PlotsNotMapped)

            Dim StrPlotsDescription As String = String.Join(" , ", PlotsError)

            Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.AppezzamentoNonMappato, StrPlotsDescription)

            If PlotsError.Count > 1 Then
                msg = String.Format(My.Resources.AgronicaCoreAgeaBIZ.AppezzamentiNonMappati, StrPlotsDescription)
            End If


            InsertErroreGias(msg, ErroreGias_Severity.Bloccante, keyErroreGias)
        End If
    End Sub


    Public Sub CheckGrowthStages(ByVal growthStages As List(Of GrowthStage))

        Dim keyErroreGias As String = CreateKeyErroreGias(ErrorType.DateEndBeforeDateStartGrowthStage)

        If Not BypassCheck(keyErroreGias) Then

            Dim BBCHNotCorrect As New List(Of String)

            Dim propsGrowthStage As List(Of PropertyInfo) = Nothing

            For Each growthStage In growthStages

                If IsNothing(propsGrowthStage) Then
                    propsGrowthStage = growthStage.GetType().GetProperties().ToList()
                End If

                If Not IsNothing(propsGrowthStage) AndAlso propsGrowthStage.Count > 0 Then
                    For i As Integer = 0 To MaxBBCHState

                        Dim BBCHState As String = i.ToString()

                        Dim index_StartDate_ToCheck = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & BBCHState & "StartDateToCheck")

                        Dim index_EndDate_ToCheck = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & BBCHState & "EndDateToCheck")

                        If Not CheckDateEndBeforeDateStart(propsGrowthStage(index_StartDate_ToCheck).GetValue(growthStage),
                                                            propsGrowthStage(index_EndDate_ToCheck).GetValue(growthStage)) Then

                            Dim index_Fase_Description = propsGrowthStage.FindIndex(Function(p) p.Name = "bbch" & BBCHState & "DescriptionToCheck")

                            Dim description As String = propsGrowthStage(index_Fase_Description).GetValue(growthStage)

                            If Not BBCHNotCorrect.Contains(description) Then
                                BBCHNotCorrect.Add(description)
                            End If

                        End If
                    Next
                End If
            Next

            If Not IsNothing(BBCHNotCorrect) AndAlso BBCHNotCorrect.Count > 0 Then

                Dim msg As String = ""

                If BBCHNotCorrect.Count = 1 Then
                    msg = String.Format(My.Resources.AgronicaCoreAgeaBIZ.DataInizioSuccessivaDataFineFaseBBCH, String.Join(",", BBCHNotCorrect))
                Else
                    msg = String.Format(My.Resources.AgronicaCoreAgeaBIZ.DataInizioSuccessivaDataFineFasiBBCH, String.Join(",", BBCHNotCorrect))
                End If

                InsertErroreGias(msg, ErroreGias_Severity.Warning, keyErroreGias)
            End If

        End If


    End Sub

    Public Sub CheckFarmingEvent(ByVal operationDesLib As String, ByVal farmingEvent As FarmingEvent)

        Dim keyErroreGias As String = CreateKeyErroreGias(ErrorType.DateEndBeforeDateStartFarmingEvent)

        If Not BypassCheck(keyErroreGias) Then
            If Not CheckDateEndBeforeDateStart(farmingEvent.startDate, farmingEvent.endDate) Then
                Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.DataInizioSuccessivaDataFineOperazione, operationDesLib)
                InsertErroreGias(msg, ErroreGias_Severity.Warning, keyErroreGias)
            End If
        End If

    End Sub

    Public Sub CheckPhytochemicalTreatment(ByVal operationDesLib As String, ByVal phytochemicalTreatment As PhytochemicalTreatment)

        Dim keyErroreGias As String = CreateKeyErroreGias(ErrorType.DateEndBeforeDateStartPhytochemicalTreatment)

        If Not BypassCheck(keyErroreGias) Then
            If Not CheckDateEndBeforeDateStart(phytochemicalTreatment.startDate, phytochemicalTreatment.endDate) Then
                Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.DataInizioSuccessivaDataFineOperazione, operationDesLib)
                InsertErroreGias(msg, ErroreGias_Severity.Warning, keyErroreGias)
            End If
        End If

    End Sub

    Public Sub CheckChemicalFertilization(ByVal operationDesLib As String, ByVal chemicalFertilization As ChemicalFertilization)

        Dim keyErroreGias As String = CreateKeyErroreGias(ErrorType.DateEndBeforeDateStartChemicalFertilization)

        If Not BypassCheck(keyErroreGias) Then
            If Not CheckDateEndBeforeDateStart(chemicalFertilization.startDate, chemicalFertilization.endDate) Then
                Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.DataInizioSuccessivaDataFineOperazione, operationDesLib)
                InsertErroreGias(msg, ErroreGias_Severity.Warning, keyErroreGias)
            End If
        End If

    End Sub

    Public Sub CheckOrganicFertilization(ByVal operationDesLib As String, ByVal organicFertilization As OrganicFertilization)

        Dim keyErroreGias As String = CreateKeyErroreGias(ErrorType.DateEndBeforeDateStartOrganicFertilization)

        If Not BypassCheck(keyErroreGias) Then
            If Not CheckDateEndBeforeDateStart(organicFertilization.startDate, organicFertilization.endDate) Then
                Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.DataInizioSuccessivaDataFineOperazione, operationDesLib)
                InsertErroreGias(msg, ErroreGias_Severity.Warning, keyErroreGias)
            End If
        End If

    End Sub

    Private Function CheckDateEndBeforeDateStart(ByVal StartDate As DateTime, ByVal EndDate As DateTime) As Boolean
        Dim valid As Integer = True

        If StartDate > EndDate Then
            valid = False
        End If

        Return valid
    End Function

    ''' <summary>
    ''' Non esporto le Operazioni che hanno una Data fuori dalla validità degli impianti
    ''' (può capitare su Coldiretti per dei dati salvati male).
    ''' Ma non faccio andare in errore l'esportazione di tutto il quaderno
    ''' </summary>
    ''' <param name="operationDate"></param>
    ''' <param name="operationDesLib"></param>
    ''' <param name="Dr"></param>
    Public Function SkipOperationOutOfScope(ByVal operationDate As Date, ByVal operationDesLib As String, ByVal Dr As DataRow) As Boolean

        Dim skip As Boolean = False

        If Not CheckIfOperationInScope(operationDate, operationDesLib, Dr, False) Then
            skip = True
        End If

        Return skip

    End Function

    Private Function CheckIfOperationInScope(ByVal operationDate As Date, ByVal operationDesLib As String, ByVal Dr As DataRow, ByVal GetError As Boolean) As Boolean

        Dim InScope As Boolean = True

        If Not IsNothing(Dr) Then

            Dim validita_inizio_impianto As Date = Dr("Validita_Inizio")

            Dim validita_fine_impianto As Date = Dr("Validita_Fine")

            If Not (operationDate <= validita_fine_impianto AndAlso operationDate >= validita_inizio_impianto) Then

                InScope = False

                If GetError Then

                    Dim keyErroreGias As String = CreateKeyErroreGias(ErrorType.OperationOutOfScope)

                    Dim App_Nome As String = Dr("App_Nome")

                    Dim msg As String = String.Format(My.Resources.AgronicaCoreAgeaBIZ.OperazioneOutOfScope, operationDesLib, App_Nome)

                    InsertErroreGias(msg, ErroreGias_Severity.Warning, keyErroreGias)
                End If

            End If
        End If

        Return InScope

    End Function

    Private Sub InsertErroreGias(ByVal msg As String, ByVal severity As ErroreGias_Severity, ByVal keyErroreGias As String)

        Dim index = List_ErroriGias.FindIndex(Function(ErroreGias) ErroreGias.severity = severity AndAlso ErroreGias.ex = keyErroreGias)

        If index = -1 Then

            List_ErroriGias.Add(New ErroreGias With {
                                .ex = keyErroreGias,
                                .messaggio = ConcatStringMsg("", msg),
                                .severity = severity
                                })
        Else
            List_ErroriGias(index).ex += String.Format("|{0}", keyErroreGias)
            List_ErroriGias(index).messaggio = ConcatStringMsg(List_ErroriGias(index).messaggio, msg)
        End If
    End Sub

    Private Function ConcatStringMsg(ByVal previous_msg As String, ByVal new_msg As String) As String

        If String.IsNullOrEmpty(previous_msg) Then
            Return new_msg
        Else
            Return String.Format("{0} <br> - {1}", previous_msg, new_msg)
        End If

    End Function

    Public Function ContinueExport() As Boolean
        Dim export As Boolean = True

        If List_ErroriGias.FindIndex(Function(ErroreGias) ErroreGias.severity = ErroreGias_Severity.Bloccante) > -1 OrElse
            (Not IsNothing(PlotsError) AndAlso PlotsError.Count > 0) Then
            export = False
        End If

        Return export

    End Function

    ''' <summary>
    ''' Crea la chiave per l'errore Gias ErrorType
    ''' </summary>
    ''' <param name="er"></param>
    ''' <returns></returns>
    Private Function CreateKeyErroreGias(ByVal er As Integer) As String
        Return String.Format("{0}", er)
    End Function


    Private Function BypassCheck(ByVal KeyErroreGias As String) As Boolean

        Dim bypass As Boolean = True

        If IsNothing(erroriDaBypassare) OrElse erroriDaBypassare.Count = 0 OrElse Not erroriDaBypassare.Contains(KeyErroreGias) Then
            bypass = False
        End If

        Return bypass

    End Function

End Class
