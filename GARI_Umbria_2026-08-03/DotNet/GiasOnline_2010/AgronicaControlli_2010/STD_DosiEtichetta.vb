Imports System.Data
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.costanti

Public Class STD_DosiEtichetta

    Public Function LeggiDosiEtichetta(specie As metaschema.utilizzi.Specie,
                                       dettaglioTrattamento As attivita.dettagli.DettaglioTrattamento,
                                       avversitaGruppo As metaschema.avversita.AvversitaGruppo,
                                       impianti As anagrafiche.Impianto(),
                                       prodottiDaTrattare As attivita.MovimentoDiMagazzino(),
                                       data As Date,
                                       objParametri_Super_Server As AgronicaCoreParametri,
                                       objParametri_Server As AgronicaCoreParametri,
                                       objParametri_Utenti As AgronicaCoreParametri) As List(Of metaschema.DoseEtichetta)

        Dim dosiEtichettaList As New List(Of metaschema.DoseEtichetta)

        Dim strErr As String = ""
        Dim DtRisultati As DataTable

        Dim av_cod As Integer = 0
        Dim av_gru As Integer = 0
        Dim FrCod As Integer = 0
        Dim Tipo_Richiesto As Integer = 0
        Dim formulatiXAllegatiNormative_IDRiga As Integer = 0
        If dettaglioTrattamento IsNot Nothing Then
            Tipo_Richiesto = dettaglioTrattamento.tipoFormulato
            formulatiXAllegatiNormative_IDRiga = dettaglioTrattamento.formulatiXAllegatiNormative_IDRiga
            If dettaglioTrattamento.prodotto IsNot Nothing Then
                FrCod = dettaglioTrattamento.prodotto.codice
            End If
        End If

        STD_Utility.GetCodiciAvversita(avversitaGruppo, Tipo_Richiesto, av_cod, av_gru)

        Dim Veg_Cod As String = "0" 'Destinazione d'uso
        If specie IsNot Nothing AndAlso specie.codice > 0 Then
            Veg_Cod = specie.codice
        End If

        Dim Grfi_cod As Integer = 0
        If impianti IsNot Nothing AndAlso impianti.Count > 0 Then
            Grfi_cod = STD_Utility.IdentificaGrfi_Cod(impianti, objParametri_Server)
        ElseIf prodottiDaTrattare IsNot Nothing AndAlso prodottiDaTrattare.Count > 0 Then
            Dim piva As String = STD_Utility.getPivaDaProdottiDaTrattare(prodottiDaTrattare)
            Grfi_cod = STD_Utility.IdentificaGrfi_Cod_ProdottiDaTrattare(piva, Veg_Cod, prodottiDaTrattare(0).Prodotto.elemCod, prodottiDaTrattare, objParametri_Server)
        End If

        Dim Copertura As String = STD_Utility.IdentificaCopertura(impianti, objParametri_Server)

        'Richiamo caricamento dosi etichetta da banche dati
        DtRisultati = AgronicaCoreWebService.DosiEtichetta_WS.DosiEtichetta_Elenco(Fr_Cod:=FrCod,
                                                                                   Veg_Cod:=Veg_Cod,
                                                                                   Tipo_Richiesto:=Tipo_Richiesto,
                                                                                   Av_Cod:=av_cod,
                                                                                   Av_Gru:=av_gru,
                                                                                   Data:=CStr(data),
                                                                                   Grfi_cod:=Grfi_cod,
                                                                                   FormulatiXAllegatiNormative_IDRiga:=formulatiXAllegatiNormative_IDRiga,
                                                                                   Copertura:=Copertura,
                                                                                   objParametri_Super_Server,
                                                                                   objParametri_Server,
                                                                                   objParametri_Utenti,
                                                                                   strErr)
        If strErr = "" Then
            If Not DtRisultati Is Nothing AndAlso DtRisultati.Rows.Count > 0 Then

                Dim obj_STD_UnitaMisura As New AgronicaControlli_2010.STD_UnitaDiMisura

                Dim List_Udm = obj_STD_UnitaMisura.LeggiUnitaDiMisuraConTipoControllo(objParametri_Server)

                Dim HashDosi As New Hashtable

                Dim Udm_Cod As Integer
                Dim DoseMin, DoseMax As Decimal
                Dim Intervallo_min, Intervallo_Max As Integer
                Dim Udm_Sim As String
                Dim AcquaMin, AcquaMax As Decimal
                Dim AcquaUdm_Cod As Integer
                Dim AcquaUdm_Sim As String
                Dim A_Epoca As Integer
                Dim Da_Epoca As Integer
                Dim Limite As Integer
                Dim Limite_Udm_Cod As Integer
                Dim LimiteUdm_Sim As String
                Dim Flag_Fioritura As Integer
                Dim Mdi_Cod As Integer
                Dim Flag_Protetto As Integer
                Dim DataSmaltimentoScorte As String
                Dim Gruppo_Dosaggi As Integer
                Dim Num_Max_Interventi_Globali As Integer
                Dim IDRiga_FormulatiXAllegatiNormative As Integer

                For i = 0 To DtRisultati.Rows.Count - 1

                    DoseMin = Math.Round(CDbl(DtRisultati.Rows(i).Item("Dose_Min")), 4)
                    DoseMax = Math.Round(CDbl(DtRisultati.Rows(i).Item("Dose_Max")), 4)
                    Udm_Cod = CInt(DtRisultati.Rows(i).Item("Udm_Cod"))
                    Udm_Sim = DtRisultati.Rows(i).Item("Udm_Sim")

                    AcquaMin = CDbl(DtRisultati.Rows(i).Item("Acqua_Min"))
                    AcquaMax = CDbl(DtRisultati.Rows(i).Item("Acqua_Max"))
                    AcquaUdm_Cod = CDbl(DtRisultati.Rows(i).Item("Acqua_Udm_Cod"))
                    AcquaUdm_Sim = ""

                    Intervallo_min = CInt(DtRisultati.Rows(i).Item("IntervalloTrattamenti_Min"))
                    Intervallo_Max = CInt(DtRisultati.Rows(i).Item("IntervalloTrattamenti_Max"))

                    Da_Epoca = DtRisultati.Rows(i).Item("Da_Epoca_1")
                    A_Epoca = DtRisultati.Rows(i).Item("a_Epoca_1")

                    Limite = DtRisultati.Rows(i).Item("Limiteinterventi")
                    Limite_Udm_Cod = DtRisultati.Rows(i).Item("udm_cod_limite")
                    LimiteUdm_Sim = ""

                    Flag_Fioritura = DtRisultati.Rows(i).Item("Epoca_Cod")

                    Mdi_Cod = DtRisultati.Rows(i).Item("Mdi_Cod")
                    Flag_Protetto = DtRisultati.Rows(i).Item("Flag_Protetto")

                    DataSmaltimentoScorte = ""
                    If Not IsDBNull(DtRisultati.Rows(i).Item("DataSmaltimentoScorte")) AndAlso IsDate(DtRisultati.Rows(i).Item("DataSmaltimentoScorte")) Then
                        DataSmaltimentoScorte = CDate(DtRisultati.Rows(i).Item("DataSmaltimentoScorte")).ToShortDateString
                    End If

                    Gruppo_Dosaggi = 0
                    If Not IsDBNull(DtRisultati.Rows(i).Item("Gruppo_Dosaggi")) AndAlso IsNumeric(DtRisultati.Rows(i).Item("Gruppo_Dosaggi")) Then
                        Gruppo_Dosaggi = CInt(DtRisultati.Rows(i).Item("Gruppo_Dosaggi"))
                    End If
                    Num_Max_Interventi_Globali = 0
                    If Not IsDBNull(DtRisultati.Rows(i).Item("Num_Max_Interventi_Globali")) AndAlso IsNumeric(DtRisultati.Rows(i).Item("Num_Max_Interventi_Globali")) Then
                        Num_Max_Interventi_Globali = CInt(DtRisultati.Rows(i).Item("Num_Max_Interventi_Globali"))
                    End If

                    IDRiga_FormulatiXAllegatiNormative = 0
                    If Not IsDBNull(DtRisultati.Rows(i).Item("FormulatiXAllegatiNormative_IDRiga")) AndAlso IsNumeric(DtRisultati.Rows(i).Item("FormulatiXAllegatiNormative_IDRiga")) Then
                        IDRiga_FormulatiXAllegatiNormative = CInt(DtRisultati.Rows(i).Item("FormulatiXAllegatiNormative_IDRiga"))
                    End If


                    If Not HashDosi.ContainsKey(DtRisultati.Rows(i).Item("For_Veg_Av_Dos_Cod") & "|" & DataSmaltimentoScorte) Then

                        Dim doseEtichetta = New AgronicaCoreModelsSTD.metaschema.DoseEtichetta()
                        doseEtichetta.codice = DtRisultati.Rows(i).Item("For_Veg_Av_Dos_Cod")
                        doseEtichetta.DoseMin = DoseMin
                        doseEtichetta.DoseMax = DoseMax

                        If Udm_Cod = 0 AndAlso Udm_Sim = "" Then
                            doseEtichetta.Udm = New metaschema.UnitaDiMisura(Udm_Cod)
                            doseEtichetta.Udm.simbolo = Udm_Sim
                        Else
                            doseEtichetta.Udm = List_Udm.Where(Function(Udm) Udm.codice = Udm_Cod)(0)
                        End If

                        doseEtichetta.AcquaMin = AcquaMin
                        doseEtichetta.AcquaMax = AcquaMax

                        If AcquaUdm_Cod = 0 AndAlso AcquaUdm_Sim = "" Then
                            doseEtichetta.UdmAcqua = New metaschema.UnitaDiMisura(AcquaUdm_Cod)
                            doseEtichetta.UdmAcqua.simbolo = AcquaUdm_Sim
                        Else
                            doseEtichetta.UdmAcqua = List_Udm.Where(Function(Udm) Udm.codice = AcquaUdm_Cod)(0)
                        End If


                        doseEtichetta.Da_Epoca = Da_Epoca
                        doseEtichetta.A_Epoca = A_Epoca
                        doseEtichetta.Limite = Limite

                        If Limite_Udm_Cod = 0 AndAlso LimiteUdm_Sim = "" Then
                            doseEtichetta.UdmLimite = New metaschema.UnitaDiMisura(Limite_Udm_Cod)
                            doseEtichetta.UdmLimite.simbolo = LimiteUdm_Sim
                        Else
                            doseEtichetta.UdmLimite = List_Udm.Where(Function(Udm) Udm.codice = Limite_Udm_Cod)(0)
                        End If

                        doseEtichetta.Epoca_Des = STD_Utility.getEpocaDes(doseEtichetta, objParametri_Server)

                        doseEtichetta.strCLTOSS_Grado = ""
                        doseEtichetta.Flag_Fioritura = New metaschema.FlagFioritura(Flag_Fioritura)
                        doseEtichetta.Flag_Fioritura.descrizione = STD_Utility.getFlagFiorituraDes(doseEtichetta, objParametri_Server)

                        doseEtichetta.IntervalloTrattamenti_Min = Intervallo_min
                        doseEtichetta.IntervalloTrattamenti_Max = Intervallo_Max

                        doseEtichetta.Mdi = New metaschema.Mdi(Mdi_Cod)
                        doseEtichetta.Mdi.descrizione = STD_Utility.getMidDes(doseEtichetta, objParametri_Server)

                        doseEtichetta.Flag_Protetto = New metaschema.FlagProtetto(Flag_Protetto)
                        doseEtichetta.Flag_Protetto.descrizione = STD_Utility.getFlagProtettoDes(doseEtichetta, objParametri_Server)

                        doseEtichetta.FormulatiXAllegatiNormative_IDRiga = IDRiga_FormulatiXAllegatiNormative
                        doseEtichetta.DataSmaltimentoScorte = DataSmaltimentoScorte
                        doseEtichetta.Gruppo_Dosaggi = Gruppo_Dosaggi
                        doseEtichetta.Num_Max_Interventi_Globali = Num_Max_Interventi_Globali

                        Dim doseText As String = ""
                        Dim doseValue As String = ""
                        STD_Utility.Stringhe_from_DosiEtichetta(New List(Of metaschema.DoseEtichetta)({doseEtichetta}), doseText, doseValue, objParametri_Server)

                        doseEtichetta.CodiceConcatenato = doseValue
                        doseEtichetta.DescrizioneConcatenata = doseText
                        dosiEtichettaList.Add(doseEtichetta)

                        HashDosi.Add(DtRisultati.Rows(i).Item("For_Veg_Av_Dos_Cod") & "|" & DataSmaltimentoScorte, "")

                    End If

                Next

            Else
                'DT: tolto "dose non disponibile", uniformata a dose non selezionata, per compatibilità con il pregresso su db
                'Dim doseEtichetta = New AgronicaCoreModelsSTD.metaschema.DoseEtichetta(0)
                'doseEtichetta.descrizione = My.Resources.AgronicaControlli_2010.DoseNonDisponibile
                'dosiEtichettaList.Add(doseEtichetta)
            End If
        End If

        Return dosiEtichettaList

    End Function

End Class
