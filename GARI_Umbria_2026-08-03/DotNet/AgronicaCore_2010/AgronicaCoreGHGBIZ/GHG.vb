Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.metaschema

Public Class GHG_EEC
    Public EMFertilizer As Double
    Public EMN2O As Double
    Public EMInputs As Double
    Public EMDiesel As Double
    Public EMEletricity As Double

    Public HA As Double
    Public tonnRaccolte As Double

    Public Sub New()
        EMFertilizer = 0.0
        EMN2O = 0.0
        EMInputs = 0.0
        EMDiesel = 0.0
        EMEletricity = 0.0

        HA = 0.0
        tonnRaccolte = 0.0
    End Sub

    Public Function getEEC() As Decimal
        If (tonnRaccolte = 0 OrElse HA = 0) Then
            Throw New Exception("L'EEC del GHG non può essere calcolato senza specificare le tonnelate e gli ettari delle raccolte.")
        End If
        Return (EMFertilizer + EMN2O + EMInputs + EMDiesel + EMEletricity) / (tonnRaccolte / HA)
    End Function

    Public Function getEECIntero() As Decimal
        Return (EMFertilizer + EMN2O + EMInputs + EMDiesel + EMEletricity)
    End Function

    Public Sub sumEEC(eec As GHG_EEC)
        EMFertilizer += eec.EMFertilizer
        EMN2O += eec.EMN2O
        EMInputs += eec.EMInputs
        EMDiesel += eec.EMDiesel
        EMEletricity += eec.EMEletricity

        HA += eec.HA
        tonnRaccolte += eec.tonnRaccolte
    End Sub
End Class

Public Class GHG_Result
    ' Total emissions from the use of the fuel.
    Public eec As GHG_EEC
    ' Annaualised emissions from carbon stock changes caused by land-use change.
    Public el As Double
    ' Annualised emissions from carbon stock changes caused by land-use change.
    Public ep As Double
    ' Emissions from transport and distribution.
    Public etd As Double
    ' Emissions from the fuel in use.
    Public eu As Double
    ' Emission savings from soil carbon accumulation via improved agrocultural management.
    Public esca As Double
    ' Emission savings from CO2 capture and geological storage.
    Public eccs As Double
    ' Emission savings from CO2 capture and replacement
    Public eccr As Double

    Public direttiva As Integer

    Public Sub New()
        eec = New GHG_EEC()
        el = 0.0
        ep = 0.0
        etd = 0.0
        eu = 0.0
        esca = 0.0
        eccs = 0.0

        direttiva = 0
    End Sub

    Public Sub sumGHGResults(ghgResult As GHG_Result)
        eec.sumEEC(ghgResult.eec)
        el += ghgResult.el
        ep += ghgResult.ep
        etd += ghgResult.etd
        eu += ghgResult.eu
        esca += ghgResult.esca
        eccs += ghgResult.eccs
        eccr += ghgResult.eccr
    End Sub

    Public Function getTotalEmissions() As Double
        Return eec.getEEC() + el + ep + etd + eu - esca - eccs - eccr
    End Function

End Class

Public Class ParamQual_GHG
    Public FF_lowiluc_Bool As Boolean
    Public FF_prevunusedland_Bool As Boolean
    Public FF_elbonus_Bool As Boolean
    Public FF_intermediatecrop_Bool As Boolean
End Class

Public Class GHG

    Public Function Ottieni_ParamQual_GHG(agendeDiRaccolta As List(Of RifAttivita), ObjParametri_Super_Server As AgronicaCoreParametri, ObjParametri_Server As AgronicaCoreParametri, ObjParametri_Utenti As AgronicaCoreParametri) As ParamQual_GHG
        Dim agendaHelper = New Agenda_Operazione_Helper()
        Dim agendaToActivity = New AgronicaCoreMapper.AgendaToAttivita()
        Dim plantRead = New Reg_Impianto_R()
        Dim plantCodesRead = New Reg_Impianti_Codici_R()
        Dim paramQual = New ParamQual_GHG()

        Dim plantSet = New HashSet(Of String)()
        Dim ELBonus = False
        Dim cover = False
        Dim lowIluc = False
        Dim unusedLand = False

        If agendeDiRaccolta.Count = 0 Then
            ELBonus = True
            unusedLand = True
            lowIluc = True
        End If
        For Each agendaRif In agendeDiRaccolta
            Dim raccAgenda = agendaHelper.Leggi(agendaRif.partitaIva, agendaRif.saCod, agendaRif.agenda, 0, ObjParametri_Server)
            Dim raccActivity = agendaToActivity.AgendaSuAttivita(raccAgenda, True, ObjParametri_Super_Server, ObjParametri_Server, ObjParametri_Utenti)
            For Each cdc In raccActivity.centriDiCosto
                If cdc.classType <> ClassType.EsercizioCDC Then
                    Continue For
                End If
                Dim exerciseCDC = CType(cdc, centri_di_costo.EsercizioCDC)
                Dim exerciseRif = exerciseCDC.esercizio.getRiferimento()
                Dim plantId = exerciseRif.getImpiantoFullId()
                If plantSet.Contains(plantId) Then
                    Continue For
                End If
                Dim plant = plantRead.Leggi_Impianto_Anagrafica(exerciseRif.partitaIva, exerciseRif.saCod, exerciseRif.appezza, exerciseRif.idReg, False, False, Nothing, False, False, ObjParametri_Super_Server, ObjParametri_Server, ObjParametri_Utenti)
                If plant Is Nothing Then
                    Continue For
                End If
                If plant.cover_Crops Then
                    cover = True
                End If
                Dim terrenoInutilizzato = plantCodesRead.LeggiValCod_2(exerciseRif.partitaIva, exerciseRif.saCod, exerciseRif.appezza, exerciseRif.idReg, 0, enum_CodiciAnagrafe.Terreno_Inutilizzato, False, "", "", ObjParametri_Server)
                Dim terrenoDegradato = plantCodesRead.LeggiValCod_2(exerciseRif.partitaIva, exerciseRif.saCod, exerciseRif.appezza, exerciseRif.idReg, 0, enum_CodiciAnagrafe.Terreno_Degradato, False, "", "", ObjParametri_Server)
                If terrenoInutilizzato = "1" Then
                    unusedLand = True
                    If terrenoDegradato = "1" Then
                        ELBonus = True
                    End If
                End If
                Dim iluc = plantCodesRead.LeggiValCod_2(exerciseRif.partitaIva, exerciseRif.saCod, exerciseRif.saCod, exerciseRif.idReg, 0, enum_CodiciAnagrafe.Low_ILUC, False, "", "", ObjParametri_Server)
                If iluc = "1" Then
                    lowIluc = True
                End If
                plantSet.Add(plantId)
            Next
        Next
        Return New ParamQual_GHG With {
            .FF_elbonus_Bool = ELBonus,
            .FF_intermediatecrop_Bool = cover,
            .FF_lowiluc_Bool = lowIluc,
            .FF_prevunusedland_Bool = unusedLand
        }
    End Function

    Public Function Ottieni_EEC_Presunto(piva As String, elemCod As Integer, matCod As Integer, data As DateTime, ObjParametri_Server As AgronicaCoreParametri) As Double
        Dim companyGHGParameters = New AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_R()
        Dim materialRead As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Dim materialDT = materialRead.Leggi2(piva, elemCod, matCod, "", 0, "", True, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Server)
        If materialDT.Rows.Count < 1 Then
            Throw New Exception("Prodotto non trovato")
        End If
        Dim materialDR = materialDT.Rows(0)
        Dim vegCod = materialDR.Field(Of Integer)("Veg_Cod")
        Dim culCod = materialDR.Field(Of Integer)("Cul_Cod")
        Dim regolamentoCod = materialDR.Field(Of Integer)("Regolamento")
        Dim GHGDT = companyGHGParameters.Leggi(piva, vegCod, culCod, regolamentoCod, data, "", "", ObjParametri_Server)
        If GHGDT.Rows.Count = 0 Then
            Return 0
        End If
        Dim GHGDR = GHGDT.Rows(0)
        Dim eec = GHGDR.Field(Of Double)("EEC")
        Return eec
    End Function

    Public Function Calcola_GHG_DaRaccolte(
                               agendeDiRaccolta As List(Of RifAttivita),
                               ObjParametri_Super_Server As AgronicaCoreParametri,
                               ObjParametri_Server As AgronicaCoreParametri,
                               ObjParametri_Utenti As AgronicaCoreParametri
    ) As GHG_Result
        Dim GHGResult = New GHG_Result()

        Dim agendaHelper = New Agenda_Operazione_Helper()
        Dim agendaToActivity = New AgronicaCoreMapper.AgendaToAttivita()
        Dim destinationsRead = New AgronicaCoreContabDAL.Mov_Destinazioni_R()
        Dim projectsRead = New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R()

        For Each agendaRif In agendeDiRaccolta
            Dim raccAgenda = agendaHelper.Leggi(agendaRif.partitaIva, agendaRif.saCod, agendaRif.agenda, 0, ObjParametri_Server)
            Dim raccActivity = agendaToActivity.AgendaSuAttivita(raccAgenda, True, ObjParametri_Super_Server, ObjParametri_Server, ObjParametri_Utenti)
            For Each cdc In raccActivity.centriDiCosto
                If cdc.classType <> ClassType.EsercizioCDC Then
                    Continue For
                End If
                Dim exerciseCDC = CType(cdc, centri_di_costo.EsercizioCDC)
                Dim exerciseInterval = exerciseCDC.esercizio.validita
                Dim exerciseRif = exerciseCDC.esercizio.getRiferimento()
                Dim projectDT = projectsRead.LeggiMinimal(exerciseRif.partitaIva, exerciseRif.saCod, exerciseRif.appezza, exerciseRif.idReg, exerciseRif.progettoCod, raccActivity.inizio, raccActivity.inizio, "", "", ObjParametri_Server)
                If projectDT.Rows.Count = 0 Then
                    Throw New Exception("Impossibile trovare l'esercizio annesso all'attività di raccolta.")
                End If
                Dim projectDR = projectDT.Rows(0)
                Dim projectStart = projectDR.Field(Of Date)("Validita_Inizio")
                Dim projectEnd = projectDR.Field(Of Date)("Validita_Fine")

                Dim agendasDT = destinationsRead.Leggi_DistinctID_Agenda_Impianti(ObjParametri_Server, exerciseRif.partitaIva, exerciseRif.saCod, exerciseRif.appezza, exerciseRif.idReg, projectStart, projectEnd, "", "")
                For Each agendaDR As DataRow In agendasDT.Rows
                    GHGResult.sumGHGResults(
                        Calcola_GHG(
                            New RifAttivita(exerciseRif.partitaIva, exerciseRif.saCod, agendaDR.Field(Of Integer)("Id_Agenda")),
                            ObjParametri_Super_Server,
                            ObjParametri_Server,
                            ObjParametri_Utenti
                        )
                    )
                Next
            Next
        Next

        Return GHGResult
    End Function

    Public Function Calcola_GHG(
                               rifAgenda As RifAttivita,
                               ObjParametri_Super_Server As AgronicaCoreParametri,
                               ObjParametri_Server As AgronicaCoreParametri,
                               ObjParametri_Utenti As AgronicaCoreParametri
    ) As GHG_Result
        Dim agendaHelper = New Agenda_Operazione_Helper()
        Dim agendaToActivity = New AgronicaCoreMapper.AgendaToAttivita()
        Dim fertilizerRead = New FertilizzantixTipologie_R()
        Dim fertilizerGHGRead = New Fertilizzanti_GHG_R()
        Dim cultivarGHGRead = New Cultivar_GHG_R()
        Dim formulatiGHGRead = New Formulati_GHG_R()
        Dim movDetailsRifHelper = New Agenda_Movimenti_Dettagli_Riferimenti_Helper()
        Dim cdgTestataRead = New AgronicaCoreContabDAL.CDG_DAL_R()
        Dim machineRead = New Parco_Macchine_R()
        Dim addressRead = New Indirizzi_R()
        Dim directiveRead = New Direttive_R()

        Dim countryCode = ""
        Dim directive = 0

        Dim GHGResult = New GHG_Result()


        Dim addresses = addressRead.Leggi_Indirizzi_Associati_Impresa(rifAgenda.partitaIva, ObjParametri_Server)
        If addresses.Count > 0 Then
            countryCode = addresses(0).indirizzo.stato.codice
        End If
        ' TODO: Input parameter directiveDes? 
        Dim directiveDT = directiveRead.Leggi(0, "", "", "", ObjParametri_Server)
        If directiveDT.Rows.Count > 0 Then
            directive = directiveDT.Rows(0).Field(Of Integer)("Direttiva_Cod")
            GHGResult.direttiva = directive
        End If

        Dim agenda = agendaHelper.Leggi(rifAgenda.partitaIva, rifAgenda.saCod, rifAgenda.agenda, 0, ObjParametri_Server)
        If agenda Is Nothing Then
            Throw New Exception($"Agenda non trovata: piva = {rifAgenda.partitaIva}, saCod = {rifAgenda.saCod}, agendaId = {rifAgenda.agenda}")
        End If
        Dim activity = agendaToActivity.AgendaSuAttivita(agenda, False, ObjParametri_Super_Server, ObjParametri_Server, ObjParametri_Utenti)

        Dim rifs = movDetailsRifHelper.LeggiRiferimentiAgenda(rifAgenda.partitaIva, rifAgenda.saCod, rifAgenda.agenda, 4500, 0, ObjParametri_Server)
        For Each rif In rifs
            Dim cdgTestataDT = cdgTestataRead.Leggi_CDG_Testata(rifAgenda.partitaIva, rif.Id_Agenda_Rif, 0, " Mac_Cod <> 0 ", ObjParametri_Server)
            For Each cdgTestataDR As DataRow In cdgTestataDT.Rows
                Dim machine = machineRead.Leggi_Macchina(rifAgenda.partitaIva, cdgTestataDR.Field(Of Integer)("Mac_Cod"), ObjParametri_Server)
                GHGResult.eec.EMDiesel += calcolaConsumoMacchinaCO2(machine, enum_UnitaMisura.Litro__HA, countryCode, directive, ObjParametri_Server)
            Next
        Next

        For Each resource In activity.risorse
            Select Case resource.classType
                Case ClassType.DettaglioRaccolta

                    Dim details = DirectCast(resource, dettagli.DettaglioRaccolta)
                    GHGResult.eec.tonnRaccolte += AgronicaCoreMapper.Utility.GetDoseTrasformata(details.quantitaTotaleReale, details.unitaDiMisura.codice) / 1000
                    For Each cdc In activity.centriDiCosto
                        If cdc.classType <> ClassType.EsercizioCDC Then
                            Continue For
                        End If
                        Dim exerciseCDC = CType(cdc, AgronicaCoreModelsSTD.attivita.centri_di_costo.EsercizioCDC)
                        GHGResult.eec.HA += exerciseCDC.superficieTrattata
                    Next
                    Exit Select

                Case ClassType.DettaglioFertilizzazione

                    Dim details = DirectCast(resource, dettagli.DettaglioFertilizzazione)
                    Dim product = details.prodotto
                    Dim productDosageHA = AgronicaCoreMapper.Utility.GetDoseTrasformata(details.doseHaReale, details.unitaDiMisuraIndicata.codice)
                    Dim fertilizerDT = fertilizerRead.Leggi(product.codice, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", ObjParametri_Server)
                    If fertilizerDT.Rows.Count = 0 Then
                        Throw New Exception($"Il fertilizzante con codice = {product.codice} non esiste.")
                    End If
                    Dim fertilizerDR = fertilizerDT.Rows(0)
                    Dim fertilizerType = fertilizerDR.Field(Of Integer)("Tp_Cod")
                    Dim fertilizerCod = fertilizerDR.Field(Of Integer)("Fer_Cod")

                    Dim fertilizerGHGDT = fertilizerGHGRead.Leggi(directive, countryCode, fertilizerType, fertilizerCod, activity.inizio, "", "", ObjParametri_Server)
                    If fertilizerGHGDT.Rows.Count = 0 Then
                        Throw New Exception($"Fertilizzante non trovato: Tp_Cod = {fertilizerType}, Fer_Cod = {fertilizerCod}")
                    End If
                    Dim fertilizerGHGDR = fertilizerGHGDT.Rows(0)
                    Dim Product_C02 = fertilizerGHGDR.Field(Of Double)("Standard_Factor_EMFertilizer_Prodotto")
                    Dim CO2 = Product_C02
                    If CO2 = 0 Then
                        Dim N_CO2 = fertilizerGHGDR.Field(Of Double)("Standard_Factor_EMFertilizer_N") * fertilizerDR.Field(Of Double)("N")
                        Dim P_CO2 = fertilizerGHGDR.Field(Of Double)("Standard_Factor_EMFertilizer_P") * fertilizerDR.Field(Of Double)("P2O5")
                        Dim K_CO2 = fertilizerGHGDR.Field(Of Double)("Standard_Factor_EMFertilizer_K") * fertilizerDR.Field(Of Double)("K2O")
                        CO2 = (N_CO2 + P_CO2 + K_CO2)
                    End If
                    Dim EMFertilizer = productDosageHA * CO2

                    If isChemicalFertilizer(fertilizerType) AndAlso details.N > 0 Then
                        GHGResult.eec.EMN2O += productDosageHA * (details.N * fertilizerGHGDR.Field(Of Double)("Standard_Factor_EMN2O"))
                    End If
                    GHGResult.eec.EMFertilizer += EMFertilizer
                    Exit Select

                Case ClassType.DettaglioSemina

                    Dim details = DirectCast(resource, dettagli.DettaglioSemina)
                    Dim productDosageHA = AgronicaCoreMapper.Utility.GetDoseTrasformata(details.doseHaReale, details.unitaDiMisuraIndicata.codice)

                    Dim objSementi As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    Dim sementeDT = objSementi.Leggi(agenda.Piva, Sa_Cod:=0,
                                       CostantiPersonalizzate.SEMENTI,
                                       details.prodotto.codice, "", 0, 0,
                                       0, 0, 0, 0, 0,
                                       "", 0, "",
                                       False, Flag_MateriePrimeSoloPrivate:=False,
                                       "",
                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                       "",
                                       "",
                                       ObjParametri_Server)
                    If sementeDT.Rows.Count = 0 Then
                        Continue For
                    End If

                    Dim sementeDR = sementeDT.Rows(0)
                    details.varieta = New utilizzi.Varieta(sementeDR.Item("Cul_cod"))
                    details.varieta.specie = New utilizzi.Specie(sementeDR.Item("Veg_cod"))
                    Dim cultivarGHGDT = cultivarGHGRead.Leggi(directive, countryCode, details.varieta.specie.codice, details.varieta.codice, "", "", ObjParametri_Server)
                    If cultivarGHGDT.Rows.Count = 0 Then
                        Throw New Exception($"Semente non trovato nel metaschema: culCod = {details.varieta.codice}, vegCod = {details.varieta.specie.codice}")
                    End If
                    Dim EMInput = productDosageHA * cultivarGHGDT.Rows(0).Field(Of Double)("Standard_Factor")
                    GHGResult.eec.EMInputs += EMInput
                    Exit Select

                Case ClassType.DettaglioTrattamento

                    Dim details = DirectCast(resource, dettagli.DettaglioTrattamento)
                    Dim productDosageHA = AgronicaCoreMapper.Utility.GetDoseTrasformata(details.doseHaReale, details.unitaDiMisuraIndicata.codice)
                    Dim formulatiDT = formulatiGHGRead.Leggi(directive, countryCode, details.prodotto.codice, "", "", ObjParametri_Server)
                    If formulatiDT.Rows.Count = 0 Then
                        Throw New Exception($"Formulato non trovato: Fr_Cod = {details.prodotto.codice}")
                    End If
                    Dim EMInput = productDosageHA * formulatiDT.Rows(0).Field(Of Double)("Standard_Factor")
                    GHGResult.eec.EMInputs += EMInput
                    Exit Select

                Case ClassType.RisorsaMacchina

                    Dim details = DirectCast(resource, risorse.RisorsaMacchina)
                    Dim macchina = machineRead.Leggi_Macchina("", details.macchina.codice, ObjParametri_Server)
                    If (macchina Is Nothing) Then
                        Throw New Exception($"Invalido riferimento alla macchina (MacCod = {details.macchina.codice}) nell'attività {activity.codice}")
                    End If
                    GHGResult.eec.EMDiesel += calcolaConsumoMacchinaCO2(macchina, enum_UnitaMisura.Litro__HA, countryCode, directive, ObjParametri_Server)
            End Select
        Next
        Return GHGResult
    End Function


    Private Function calcolaConsumoMacchinaCO2(macchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine, udmCod As enum_UnitaMisura, countryCode As String, directive As Integer, ObjParametri_Server As AgronicaCoreParametri) As Double
        Dim power = 0

        If macchina.potenza = "" OrElse Not Decimal.TryParse(macchina.potenza.Replace(",", "."), power) Then
            Throw New Exception($"La macchina {macchina.descrizione} non ha una potenza valida: Potenza => {macchina.potenza}")
        End If
        Dim machinesConsumption = New Macchine_Consumi_R()
        Dim fuelGHGRead = New AgronicaCoreMetaSchemaDAL.Carburanti_GHG_R()
        Dim consumptionDT = machinesConsumption.Leggi(macchina.tipo.codice, macchina.marca.codice, macchina.potenza, macchina.unita_Misura.codice, macchina.alimentazione.codice, udmCod, "", "", ObjParametri_Server)
        If consumptionDT.Rows.Count = 0 Then
            Throw New Exception($"Impossibile trovare consumo macchina {macchina.descrizione}")
        End If
        Dim consumption = consumptionDT.Rows(0).Field(Of Double)("Consumo")
        ' TODO aggiungere direttiva e stato
        Dim fuelDT = fuelGHGRead.Leggi(macchina.alimentazione.codice, 0, directive, countryCode, "", "", ObjParametri_Server)
        If fuelDT.Rows.Count = 0 Then
            Throw New Exception($"Impossibile trovare fattore di conversione a C02 per il carburante con codice {macchina.alimentazione.codice}")
        End If
        Dim CO2Factor = fuelDT.Rows(0).Field(Of Double)("Standard_Factor")
        Return consumption * CO2Factor
    End Function

    Private Function isChemicalFertilizer(fertilizerType As Integer) As Boolean
        'TODO aggiungere tipo fertilizzanti a tipiEnumerativi
        If fertilizerType = 6 OrElse fertilizerType = 7 OrElse fertilizerType = 8 Then
            Return False
        End If
        Return True
    End Function

    Public Function leggi_ImpreseParametri(piva As String, specie As List(Of Integer), culCod As Integer, regolamentoCod As Integer, dataValiditaInizio As DateTime, dataValiditaFine As DateTime, ObjParametri_Utente As AgronicaCoreParametri, ObjParametri_Server As AgronicaCoreParametri)
        Dim ImpreseParametriR As New AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_R
        Dim GHGDT = ImpreseParametriR.LeggiParametri(piva, specie, culCod, regolamentoCod, dataValiditaInizio, dataValiditaFine, "", "", True, ObjParametri_Utente, ObjParametri_Server)
        Return GHGDT
    End Function

    Public Function scrivi_ImpreseParametri(piva As String, Veg_Cod As Integer, Cul_Cod As Integer, Regolamento_Cod As Integer, EEC As Decimal, Validita_Inizio As Date, Validita_Fine As Date, ObjParametri_Server As AgronicaCoreParametri)
        Dim ImpreseParametriW As New AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_W
        Dim result = ImpreseParametriW.Scrivi(piva, Veg_Cod, Cul_Cod, Regolamento_Cod, EEC, Validita_Inizio, Validita_Fine, ObjParametri_Server)
        Return result
    End Function

    Public Function modifica_ImpreseParametri(ID As Integer, piva As String, Veg_Cod As Integer, Cul_Cod As Integer, Regolamento_Cod As Integer, EEC As Decimal, Validita_Inizio As Date, Validita_Fine As Date, ObjParametri_Server As AgronicaCoreParametri)
        Dim ImpreseParametriW As New AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_W
        Dim result = ImpreseParametriW.Modifica(ID, piva, Veg_Cod, Cul_Cod, Regolamento_Cod, EEC, Validita_Inizio, Validita_Fine, ObjParametri_Server)
        Return result
    End Function

    Public Function cancella_ImpreseParametri(ID As Integer, ObjParametri_Server As AgronicaCoreParametri)
        Dim ImpreseParametriW As New AgronicaCoreAnagrafeDAL.Imprese_Parametri_GHG_W
        Dim result = ImpreseParametriW.Cancella(ID, ObjParametri_Server)
        Return result
    End Function

End Class
