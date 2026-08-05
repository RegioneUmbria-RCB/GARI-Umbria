Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class cls_SottoReport_IVA

    '###########################################
    'inserisci righe nel ds dell'IVA ACQUISTI
    Public Function Inserisci_Riga_sottoReportIVA( _
                        ByRef DS_SR_IVA As DS_SottoReport_IVA, _
                        ByVal Titolo As String, _
                        ByVal Cod_Iva As Integer, _
                        ByVal Aliquota_Sigla As String, _
                        ByVal Imponibile As Decimal, _
                        ByVal Imposta As Decimal, _
                        ByVal Totale As Decimal, _
                         ByVal Riepilogo_Imponibile As Decimal, _
                        ByVal Riepilogo_Imposta As Decimal, _
                        ByVal Riepilogo_Totale As Decimal, _
                         ByVal Detraibile_Imponibile As Decimal, _
                        ByVal Detraibile_Imposta As Decimal, _
                        ByVal Detraibile_Totale As Decimal, _
                        ByVal Indetraibile_Imponibile As Decimal, _
                        ByVal Indetraibile_Imposta As Decimal, _
                        ByVal Indetraibile_Totale As Decimal)

        Dim Dr As DS_SottoReport_IVA.DT_SottoReport_IVARow

        Dr = DS_SR_IVA.DT_SottoReport_IVA.NewRow

        Dr.Titolo = Titolo

        Dr.Cod_Iva = Cod_Iva
        Dr.Aliquota = Aliquota_Sigla

        Dr.Imponibile = Format(Imponibile, "##,###,##0.00")
        Dr.Imposta = Format(Imposta, "##,###,##0.00")
        Dr.Totale = Format(Totale, "##,###,##0.00")

        Dr.ImponibileFloat = Imponibile
        Dr.ImpostaFloat = Imposta
        Dr.TotaleFloat = Totale

        Dr.Riepilogo_Imponibile = Format(Riepilogo_Imponibile, "##,###,##0.00")
        Dr.Riepilogo_Imposta = Format(Riepilogo_Imposta, "##,###,##0.00")
        Dr.Riepilogo_Totale = Format(Riepilogo_Totale, "##,###,##0.00")

        Dr.Riepilogo_ImponibileFloat = Riepilogo_Imponibile
        Dr.Riepilogo_ImpostaFloat = Riepilogo_Imposta
        Dr.Riepilogo_TotaleFloat = Riepilogo_Totale

        Dr.Detraibile_Imponibile = Format(Detraibile_Imponibile, "##,###,##0.00")
        Dr.Detraibile_Imposta = Format(Detraibile_Imposta, "##,###,##0.00")
        Dr.Detraibile_Totale = Format(Detraibile_Totale, "##,###,##0.00")

        Dr.Indetraibile_Imponibile = Format(Indetraibile_Imponibile, "##,###,##0.00")
        Dr.Indetraibile_Imposta = Format(Indetraibile_Imposta, "##,###,##0.00")
        Dr.Indetraibile_Totale = Format(Indetraibile_Totale, "##,###,##0.00")

        DS_SR_IVA.DT_SottoReport_IVA.Rows.Add(Dr)

    End Function

    '###########################################
    'inserisci righe nel ds dell'IVA VENDITE
    Public Function Inserisci_Riga_sottoReportIVA_DUPLICATO( _
                       ByRef DS_SR_IVA As DS_SottoReportIVA_Duplicato, _
                       ByVal Titolo As String, _
                       ByVal Cod_Iva As Integer, _
                       ByVal Aliquota_Sigla As String, _
                       ByVal Imponibile As Decimal, _
                       ByVal Imposta As Decimal, _
                       ByVal Compensazione As Decimal, _
                       ByVal Compensazione_Export As Decimal, _
                       ByVal Totale As Decimal, _
                       ByVal Riepilogo_Imponibile As Decimal, _
                       ByVal Riepilogo_Imposta As Decimal, _
                       ByVal Riepilogo_Totale As Decimal, _
                       ByVal Compensazione_Imponibile As Decimal, _
                       ByVal Compensazione_Imposta As Decimal, _
                       ByVal Compensazione_Totale As Decimal, _
                       ByVal NonCompensazione_Imponibile As Decimal, _
                       ByVal NonCompensazione_Imposta As Decimal, _
                       ByVal NonCompensazione_Totale As Decimal, _
                       ByVal Compensazione_Export_Totale As Decimal)

        Dim Dr As DS_SottoReportIVA_Duplicato.DT_SottoReportIVA_DuplicatoRow

        Dr = DS_SR_IVA.DT_SottoReportIVA_Duplicato.NewRow

        Dr.Titolo = Titolo

        Dr.Cod_Iva = Cod_Iva
        Dr.Aliquota = Aliquota_Sigla

        Dr.Imponibile = Format(Imponibile, "##,###,##0.00")
        Dr.Imposta = Format(Imposta, "##,###,##0.00")
        Dr.Compensazione = Format(Compensazione, "##,###,##0.00")
        Dr.Compensazione_Export = Format(Compensazione_Export, "##,###,##0.00")
        Dr.Totale = Format(Totale, "##,###,##0.00")

        Dr.ImponibileFloat = Imponibile
        Dr.ImpostaFloat = Imposta
        Dr.TotaleFloat = Totale

        Dr.Riepilogo_Imponibile = Format(Riepilogo_Imponibile, "##,###,##0.00")
        Dr.Riepilogo_Imposta = Format(Riepilogo_Imposta, "##,###,##0.00")
        Dr.Riepilogo_Totale = Format(Riepilogo_Totale, "##,###,##0.00")

        Dr.Riepilogo_ImponibileFloat = Riepilogo_Imponibile
        Dr.Riepilogo_ImpostaFloat = Riepilogo_Imposta
        Dr.Riepilogo_TotaleFloat = Riepilogo_Totale

        Dr.Compensazione_Imponibile = Format(Compensazione_Imponibile, "##,###,##0.00")
        Dr.Compensazione_Imposta = Format(Compensazione_Imposta, "##,###,##0.00")
        Dr.Compensazione_Totale = Format(Compensazione_Totale, "##,###,##0.00")

        Dr.NonCompensazione_Imponibile = Format(NonCompensazione_Imponibile, "##,###,##0.00")
        Dr.NonCompensazione_Imposta = Format(NonCompensazione_Imposta, "##,###,##0.00")
        Dr.NonCompensazione_Totale = Format(NonCompensazione_Totale, "##,###,##0.00")

        Dr.Compensazione_Export_Totale = Format(Compensazione_Export_Totale, "##,###,##0.00")

        DS_SR_IVA.DT_SottoReportIVA_Duplicato.Rows.Add(Dr)

    End Function

    '########################################################################################
    Public Function CaricaGriglia_DtIvaGenerale() As DataTable

        Dim DtIvaGenerale As New DataTable

        DtIvaGenerale.Columns.Add(New DataColumn("imponibile_netto", GetType(Decimal)))
        DtIvaGenerale.Columns.Add(New DataColumn("cod_iva", GetType(Integer)))
        DtIvaGenerale.Columns.Add(New DataColumn("iva", GetType(Decimal)))
        DtIvaGenerale.Columns.Add(New DataColumn("aliquota_iva", GetType(Integer)))
        DtIvaGenerale.Columns.Add(New DataColumn("aliquota_des", GetType(String)))
        DtIvaGenerale.Columns.Add(New DataColumn("totale", GetType(Decimal)))
        'lato acquisto è l'iva indetraibile
        'lato vendita è l'iva in compensazione
        DtIvaGenerale.Columns.Add(New DataColumn("iva_indetraibile_perc", GetType(Decimal)))
        DtIvaGenerale.Columns.Add(New DataColumn("iva_indetraibile", GetType(Decimal)))
        DtIvaGenerale.Columns.Add(New DataColumn("modalita_doc", GetType(Integer)))

        Return DtIvaGenerale

    End Function

    '########################################################################################
    'iva_indetraibile_perc:
    'lato acquisto è l'iva indetraibile
    'lato vendita è l'iva in compensazione
    Public Sub InserisciRiga_DtIvaGenerale(ByRef DT_IVA_Generale As DataTable, _
                                            ByVal imponibile_netto As Decimal, _
                                            ByVal cod_iva As Integer, _
                                            ByVal iva As Decimal, _
                                            ByVal aliquota_iva As Decimal, _
                                            ByVal aliquota_des As String, _
                                            ByVal totale As Decimal, _
                                            ByVal iva_indetraibile_perc As Decimal, _
                                            ByVal iva_indetraibile As Decimal, _
                                            ByVal modalita_doc As Integer)

        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = DT_IVA_Generale.NewRow

        Dr.Item("imponibile_netto") = imponibile_netto 'Format(imponibile_netto, "##,###,##0.00") 
        Dr.Item("cod_iva") = cod_iva
        Dr.Item("iva") = iva ' Format(iva, "##,###,##0.00") 
        Dr.Item("aliquota_iva") = aliquota_iva
        Dr.Item("aliquota_des") = aliquota_des
        Dr.Item("totale") = totale ' Format(totale, "##,###,##0.00") 
        Dr.Item("iva_indetraibile_perc") = iva_indetraibile_perc
        Dr.Item("iva_indetraibile") = iva_indetraibile
        Dr.Item("modalita_doc") = modalita_doc

        'Associo alla tabella la nuova riga creata
        DT_IVA_Generale.Rows.Add(Dr)

    End Sub

    '########################################################################################
    Public Sub AggiornaRiga_DtIvaGenerale(ByRef DR_IVA_Generale As DataRow, _
                                          ByVal imponibile_netto As Decimal, _
                                          ByVal iva As Decimal, _
                                          ByVal totale As Decimal, _
                                          Optional ByVal iva_indetraibile As Decimal = 0)

        DR_IVA_Generale.Item("imponibile_netto") = imponibile_netto  'Format(imponibile_netto, "##,###,##0.00")  
        DR_IVA_Generale.Item("iva") = iva 'Format(iva, "##,###,##0.00")
        DR_IVA_Generale.Item("iva_indetraibile") = iva_indetraibile 'Format(iva_indetraibile, "##,###,##0.00") 
        DR_IVA_Generale.Item("totale") = totale 'Format(totale, "##,###,##0.00") 

    End Sub

    '########################################################################################
    'da usare nel caso delle vendite (ds acquisti deve essere valorizzato vuoto per non generare errore sul report)
    Public Sub Carica_DSSottoReportIVA_vuoto_VENDITE(ByRef DS_SR_IVA As DS_SottoReport_IVA)

        Inserisci_Riga_sottoReportIVA(DS_SR_IVA, _
                                        "", _
                                        0, _
                                        "", _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                        0, _
                                        0)

    End Sub

    '########################################################################################
    Public Sub Carica_DSSottoReportIVA_duplicato_vuoto(ByRef DS_SR_IVA As DS_SottoReportIVA_Duplicato)

        Inserisci_Riga_sottoReportIVA_DUPLICATO(DS_SR_IVA, _
                                                "", _
                                                0, _
                                                "", _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, _
                                                0, 0, 0, 0, 0, 0)

    End Sub

    '########################################################################################
    'carica dataset IVA ACQUISTI
    Public Sub Carica_DSSottoReportIVA_daDtIVAGenerale(ByRef DS_SR_IVA As DS_SottoReport_IVA, _
                                                        ByRef DT_IVA_Generale_NoOrder As DataTable, _
                                                        ByVal titolo As String)

        'ByRef Totale_Imponibile As Decimal, _
        'ByRef Totale_Imposta As Decimal, _
        'ByRef Totale_Importo As Decimal)


        Dim dv As New DataView(DT_IVA_Generale_NoOrder)
        dv.Sort = "cod_iva, iva_indetraibile_perc"

        'Dim ciccio, aaa As Integer
        'For aaa = 0 To dv.Count
        '    ciccio = dv(aaa).Item("cod_iva")
        '    Dim deb As Integer = 876
        'Next

        'Dim DT_IVA_Generale As DataTable
        'DT_IVA_Generale = dv.Table


        Dim Totale_Imponibile As Decimal = 0
        Dim Totale_Imposta As Decimal = 0
        Dim Totale_Importo As Decimal = 0
        Dim Detraibile_Imponibile As Decimal = 0
        Dim Detraibile_Imposta As Decimal = 0
        Dim Detraibile_Totale As Decimal = 0
        Dim Indetraibile_Imponibile As Decimal = 0
        Dim Indetraibile_Imposta As Decimal = 0
        Dim Indetraibile_Totale As Decimal = 0
        Dim Riga_Imponibile As Decimal = 0
        Dim Riga_Imposta As Decimal = 0
        Dim Riga_Importo As Decimal = 0
        Dim Riga_Indetraibile_Imponibile As Decimal = 0
        Dim Riga_Indetraibile_Imposta As Decimal = 0
        Dim Riga_Indetraibile_Totale As Decimal = 0

        'If Not IsNothing(DT_IVA_Generale) AndAlso DT_IVA_Generale.Rows.Count > 0 Then
        If Not IsNothing(dv) AndAlso dv.Count > 0 Then
            Dim i As Integer

            'For i = 0 To DT_IVA_Generale.Rows.Count - 1
            For i = 0 To dv.Count - 1

                'With DT_IVA_Generale.Rows(i)
                With dv(i)

                    Riga_Imponibile = .Item("imponibile_netto")
                    Riga_Imposta = .Item("iva")
                    Riga_Importo = .Item("Totale")

                    Totale_Imponibile += Riga_Imponibile
                    Totale_Imposta += Riga_Imposta
                    Totale_Importo += Riga_Importo

                    Select Case .Item("iva_indetraibile_perc")

                        Case 0
                            Detraibile_Imponibile += .Item("imponibile_netto")
                            Detraibile_Imposta += .Item("iva")
                            Detraibile_Totale += .Item("Totale")

                        Case Is <> 0

                            Riga_Indetraibile_Imponibile = ArrotondaVal_2(.Item("imponibile_netto") * .Item("iva_indetraibile_perc") / 100)
                            Riga_Indetraibile_Imposta = ArrotondaVal_2(.Item("iva") * .Item("iva_indetraibile_perc") / 100)
                            Riga_Indetraibile_Totale = ArrotondaVal_2(.Item("Totale") * .Item("iva_indetraibile_perc") / 100)

                            Indetraibile_Imponibile += Riga_Indetraibile_Imponibile
                            Indetraibile_Imposta += Riga_Indetraibile_Imposta
                            Indetraibile_Totale += Riga_Indetraibile_Totale

                            Detraibile_Imponibile += Riga_Imponibile - Riga_Indetraibile_Imponibile
                            Detraibile_Imposta += Riga_Imposta - Riga_Indetraibile_Imposta
                            Detraibile_Totale += Riga_Importo - Riga_Indetraibile_Totale

                    End Select

                    Inserisci_Riga_sottoReportIVA(DS_SR_IVA, _
                                                    titolo, _
                                                    .Item("cod_iva"), _
                                                    .Item("aliquota_des"), _
                                                    .Item("imponibile_netto"), _
                                                    .Item("iva"), _
                                                    .Item("Totale"), _
                                                    Totale_Imponibile, _
                                                    Totale_Imposta, _
                                                    Totale_Importo, _
                                                    Detraibile_Imponibile, _
                                                     Detraibile_Imposta, _
                                                     Detraibile_Totale, _
                                                     Indetraibile_Imponibile, _
                                                     Indetraibile_Imposta, _
                                                     Indetraibile_Totale)

                End With

            Next

        End If

    End Sub

    '########################################################################################
    'carica dataset IVA ACQUISTI con sviluppo "revisione arrotondamenti"
    Public Sub Carica_DSSottoReportIVA_daDtIVAGenerale_ACQUISTI_NEW(ByRef DS_SR_IVA As DS_SottoReport_IVA, _
                                                                    ByRef DT_IVA_Generale_NoOrder As DataTable, _
                                                                    ByVal titolo As String)


        Dim dv As New DataView(DT_IVA_Generale_NoOrder)
        dv.Sort = "cod_iva, iva_indetraibile_perc"
        Dim Totale_Imponibile As Decimal = 0
        Dim Totale_Imposta As Decimal = 0
        Dim Totale_Importo As Decimal = 0
        Dim Detraibile_Imponibile As Decimal = 0
        Dim Detraibile_Imposta As Decimal = 0
        Dim Detraibile_Totale As Decimal = 0
        Dim Indetraibile_Imponibile As Decimal = 0
        Dim Indetraibile_Imposta As Decimal = 0
        Dim Indetraibile_Totale As Decimal = 0
        Dim Riga_Imponibile As Decimal = 0
        Dim Riga_Imposta As Decimal = 0
        Dim Riga_Importo As Decimal = 0
        Dim Riga_Indetraibile As Decimal = 0
        Dim Riga_Indetraibile_Imponibile As Decimal = 0
        Dim Riga_Indetraibile_Imposta As Decimal = 0
        Dim Riga_Indetraibile_Totale As Decimal = 0

        If Not IsNothing(dv) AndAlso dv.Count > 0 Then
            Dim i As Integer

            For i = 0 To dv.Count - 1

                With dv(i)

                    Riga_Imponibile = .Item("imponibile_netto")
                    Riga_Imposta = .Item("iva")
                    Riga_Importo = .Item("Totale")
                    Riga_Indetraibile = .Item("iva_indetraibile")

                    Totale_Imponibile += Riga_Imponibile
                    Totale_Imposta += Riga_Imposta
                    Totale_Importo += Riga_Importo

                    Select Case .Item("iva_indetraibile_perc")

                        Case 0
                            Detraibile_Imponibile += .Item("imponibile_netto")
                            Detraibile_Imposta += .Item("iva")
                            Detraibile_Totale += .Item("Totale")

                        Case Is <> 0

                            'Riga_Indetraibile_Imponibile = ArrotondaVal_2(.Item("imponibile_netto") * .Item("iva_indetraibile_perc") / 100)
                            'Riga_Indetraibile_Imposta = ArrotondaVal_2(.Item("iva") * .Item("iva_indetraibile_perc") / 100)
                            'Riga_Indetraibile_Totale = ArrotondaVal_2(.Item("Totale") * .Item("iva_indetraibile_perc") / 100)

                            'maga 25/01/2018:
                            'non è salvato l'imponibile della parte dell'iva indetraibile, quindi andrebbe calcolato
                            'con giulia e scatto si è deciso di non calcolarlo per evitare problemi di arrotondamenti o di totali che non tornano
                            'nel riepilogo facciamo vedere solo il totale dell'iva indetraibile e dell'iva detraibile
                            Riga_Indetraibile_Imponibile = 0
                            Riga_Indetraibile_Imposta = Riga_Indetraibile
                            Riga_Indetraibile_Totale = 0

                            Indetraibile_Imponibile += Riga_Indetraibile_Imponibile
                            Indetraibile_Imposta += Riga_Indetraibile_Imposta
                            Indetraibile_Totale += Riga_Indetraibile_Totale

                            Detraibile_Imponibile += Riga_Imponibile - Riga_Indetraibile_Imponibile
                            Detraibile_Imposta += Riga_Imposta - Riga_Indetraibile_Imposta
                            Detraibile_Totale += Riga_Importo - Riga_Indetraibile_Totale

                    End Select

                    Inserisci_Riga_sottoReportIVA(DS_SR_IVA, _
                                                    titolo, _
                                                    .Item("cod_iva"), _
                                                    .Item("aliquota_des"), _
                                                    .Item("imponibile_netto"), _
                                                    .Item("iva"), _
                                                    .Item("Totale"), _
                                                    Totale_Imponibile, _
                                                    Totale_Imposta, _
                                                    Totale_Importo, _
                                                    Detraibile_Imponibile, _
                                                     Detraibile_Imposta, _
                                                     Detraibile_Totale, _
                                                     Indetraibile_Imponibile, _
                                                     Indetraibile_Imposta, _
                                                     Indetraibile_Totale)

                End With

            Next

        End If

    End Sub

    '########################################################################################
    'carica dataset IVA VENDITE
    Public Sub Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE(ByRef DS_SR_IVA As DS_SottoReportIVA_Duplicato, _
                                                                ByRef DT_IVA_Generale_NoOrder As DataTable, _
                                                                ByVal titolo As String)

        'ByRef Totale_Imponibile As Decimal, _
        'ByRef Totale_Imposta As Decimal, _
        'ByRef Totale_Importo As Decimal)

        Dim dv As New DataView(DT_IVA_Generale_NoOrder)
        'dv.Sort = "cod_iva"
        dv.Sort = "cod_iva, iva_indetraibile_perc"

        Dim Flag_EstereEsenti As Boolean = False

        'totali a piè della tabella
        Dim Totale_Imponibile As Decimal = 0
        Dim Totale_Imposta As Decimal = 0
        Dim Totale_Compensazione As Decimal = 0
        Dim Totale_Importo As Decimal = 0
        Dim Totale_Compensazione_Imponibile As Decimal = 0
        Dim Totale_Compensazione_Imposta As Decimal = 0
        Dim Totale_Compensazione_Totale As Decimal = 0
        Dim Totale_NonCompensazione_Imponibile As Decimal = 0
        Dim Totale_NonCompensazione_Imposta As Decimal = 0
        Dim Totale_NonCompensazione_Totale As Decimal = 0
        Dim Totale_Compensazione_Export_Totale As Decimal = 0

        ' la riga è quella del riepilogo iva vendite
        'dataset con un dettaglio su ogni aliquota * % compens
        Dim Riga_Imponibile As Decimal = 0
        Dim Riga_Imposta As Decimal = 0
        Dim Riga_Compensazione As Decimal = 0
        Dim Riga_Compensazione_Export As Decimal = 0
        Dim Riga_Importo As Decimal = 0
        Dim Riga_Compensazione_Imponibile As Decimal = 0
        Dim Riga_Compensazione_Imposta As Decimal = 0
        Dim Riga_Compensazione_Compensazione As Decimal = 0
        Dim Riga_Compensazione_Totale As Decimal = 0
        Dim Riga_NonCompensazione_Imponibile As Decimal = 0
        Dim Riga_NonCompensazione_Imposta As Decimal = 0
        Dim Riga_NonCompensazione_Compensazione As Decimal = 0
        Dim Riga_NonCompensazione_Totale As Decimal = 0

        Dim Iva_Compensazione_Perc As Decimal = 0
        Dim IvaCompensazione As Decimal = 0

        'If Not IsNothing(DT_IVA_Generale) AndAlso DT_IVA_Generale.Rows.Count > 0 Then
        If Not IsNothing(dv) AndAlso dv.Count > 0 Then
            Dim i As Integer

            'For i = 0 To DT_IVA_Generale.Rows.Count - 1
            For i = 0 To dv.Count - 1

                'With DT_IVA_Generale.Rows(i)
                With dv(i)

                    Select Case .Item("cod_iva")
                        'vendite estere non imponibili=>non c'è iva, ma c'è compensazione su iva teorica
                        '   ==> vanno riportate a parte e non dentro a compensazione
                        Case 78, 95, 67, 83, 84
                            Flag_EstereEsenti = True
                        Case Else
                            Flag_EstereEsenti = False
                    End Select

                    Riga_Imponibile = .Item("imponibile_netto")
                    Riga_Imposta = .Item("iva")
                    Riga_Compensazione = .Item("iva_indetraibile")
                    Riga_Compensazione_Export = 0
                    Riga_Importo = .Item("Totale")

                    Totale_Imponibile += Riga_Imponibile
                    Totale_Imposta += Riga_Imposta
                    Totale_Compensazione += Riga_Compensazione
                    Totale_Importo += Riga_Importo

                    Iva_Compensazione_Perc = .Item("iva_indetraibile_perc")

                    'di tutto questo servono solo i totali dell'iva in compensazione e non in compensazione
                    'il totale non c'è nel report
                    'l'imponibile è stampato solo nella riga di riepilogo totale
                    Select Case Iva_Compensazione_Perc

                        Case 0
                            Riga_NonCompensazione_Imponibile = Riga_Imponibile
                            Riga_NonCompensazione_Imposta = Riga_Imposta
                            Riga_NonCompensazione_Compensazione = Riga_Compensazione
                            Riga_NonCompensazione_Totale = Riga_Importo

                            Totale_NonCompensazione_Imponibile += Riga_NonCompensazione_Imponibile
                            Totale_NonCompensazione_Imposta += Riga_NonCompensazione_Imposta
                            Totale_NonCompensazione_Totale += Riga_NonCompensazione_Totale

                        Case Is <> 0

                            '  Giulia, 18/10/2016 16.51.04: La compensazione va calcolata anche quando c'è esclusione iva, sulla base imponibile dell'iva teorica

                            'MODIFICA DEL 04/08/2015:
                            'visto che ora l'iva in compensazione si calcola sull'imponibile
                            'dalla modifica precedente veniva calcolata anche sulle righe di esclusione iva
                            '(perchè l'imponibile era valorizzato)
                            'introdotto quindi controllo per evitare l'errore

                            If False Then
                                'If Riga_Imposta = 0 Then
                                'sono gli articoli esclusione iva

                                Riga_NonCompensazione_Imponibile = Riga_Imponibile
                                Riga_NonCompensazione_Imposta = Riga_Imposta
                                Riga_NonCompensazione_Compensazione = Riga_Compensazione
                                Riga_NonCompensazione_Totale = Riga_Importo

                                Totale_NonCompensazione_Imponibile += Riga_NonCompensazione_Imponibile
                                Totale_NonCompensazione_Imposta += Riga_NonCompensazione_Imposta
                                Totale_NonCompensazione_Totale += Riga_NonCompensazione_Totale

                            Else
                                'modifica del 23/07/2015: corretto il calcolo

                                'Riga_Compensazione_Imponibile = ArrotondaVal_2(Riga_Imponibile * Iva_Compensazione_Perc / 100)
                                'Riga_Compensazione_Imposta = ArrotondaVal_2(Riga_Imposta * Iva_Compensazione_Perc / 100)
                                'Riga_Compensazione_Totale = ArrotondaVal_2(Riga_Importo * Iva_Compensazione_Perc / 100)

                                IvaCompensazione = Riga_Imponibile * Iva_Compensazione_Perc / 100
                                'IvaDebito = Riga_Imposta - IvaCompensazione

                                Riga_Compensazione_Imponibile = 0

                                If Flag_EstereEsenti Then
                                    Riga_Compensazione = 0
                                    Riga_Compensazione_Imposta = 0
                                    Riga_Compensazione_Export = ArrotondaVal_2(IvaCompensazione)
                                Else
                                    '  Giulia, 14/11/2016 17.30.58: Ricalcolo forzatamente la compensazione raggruppata per evitare errori di arrotondamento
                                    Riga_Compensazione = ArrotondaVal_2(IvaCompensazione)
                                    Riga_Compensazione_Imposta = ArrotondaVal_2(IvaCompensazione)
                                    Riga_Compensazione_Export = 0
                                End If

                                'Riga_Compensazione_Compensazione =
                                Riga_Compensazione_Totale = 0

                                Totale_Compensazione_Imponibile += Riga_Compensazione_Imponibile
                                Totale_Compensazione_Imposta += Riga_Compensazione_Imposta
                                Totale_Compensazione_Totale += Riga_Compensazione_Totale
                                Totale_Compensazione_Export_Totale += Riga_Compensazione_Export

                                Riga_NonCompensazione_Imponibile = Riga_Imponibile - Riga_Compensazione_Imponibile
                                Riga_NonCompensazione_Imposta = Riga_Imposta - Riga_Compensazione_Imposta
                                Riga_NonCompensazione_Totale = Riga_Importo - Riga_Compensazione_Totale

                                Totale_NonCompensazione_Imponibile += Riga_NonCompensazione_Imponibile
                                Totale_NonCompensazione_Imposta += Riga_NonCompensazione_Imposta
                                Totale_NonCompensazione_Totale += Riga_NonCompensazione_Totale

                            End If



                    End Select

                    Inserisci_Riga_sottoReportIVA_DUPLICATO(DS_SR_IVA, _
                                                            titolo, _
                                                            .Item("cod_iva"), _
                                                            .Item("aliquota_des"), _
                                                            Riga_Imponibile,
                                                            Riga_Imposta, _
                                                            Riga_Compensazione, _
                                                            Riga_Compensazione_Export, _
                                                            Riga_Importo, _
                                                            Totale_Imponibile, _
                                                            Totale_Imposta, _
                                                            Totale_Importo, _
                                                            Totale_Compensazione_Imponibile, _
                                                            Totale_Compensazione_Imposta, _
                                                            Totale_Compensazione_Totale, _
                                                            Totale_NonCompensazione_Imponibile, _
                                                            Totale_NonCompensazione_Imposta, _
                                                            Totale_NonCompensazione_Totale, _
                                                            Totale_Compensazione_Export_Totale)

                End With

            Next

        End If

    End Sub

    '########################################################################################
    'carica dataset IVA VENDITE con sviluppo "revisione arrotondamenti"
    Public Sub Carica_DSSottoReportIVA_daDtIVAGenerale_VENDITE_NEW(ByRef DS_SR_IVA As DS_SottoReportIVA_Duplicato, _
                                                                    ByRef DT_IVA_Generale_NoOrder As DataTable, _
                                                                    ByVal titolo As String, _
                                                                    ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim dv As New DataView(DT_IVA_Generale_NoOrder)
        dv.Sort = "cod_iva, iva_indetraibile_perc"

        Dim Flag_EstereEsenti As Boolean = False

        'totali a piè della tabella
        Dim Totale_Imponibile As Decimal = 0
        Dim Totale_Imposta As Decimal = 0
        'Dim Totale_Compensazione As Decimal = 0 commentato perchè non è usato
        Dim Totale_Importo As Decimal = 0
        Dim Totale_Compensazione_Imponibile As Decimal = 0
        Dim Totale_Compensazione_Imposta As Decimal = 0
        Dim Totale_Compensazione_Totale As Decimal = 0
        Dim Totale_NonCompensazione_Imponibile As Decimal = 0
        Dim Totale_NonCompensazione_Imposta As Decimal = 0
        Dim Totale_NonCompensazione_Totale As Decimal = 0
        Dim Totale_Compensazione_Export_Totale As Decimal = 0

        ' la riga è quella del riepilogo iva vendite
        'dataset con und ettaglio su ogni aliquota * % compens
        Dim Riga_Imponibile As Decimal = 0
        Dim Riga_Imposta As Decimal = 0
        Dim Riga_Compensazione As Decimal = 0
        Dim Riga_Compensazione_Export As Decimal = 0
        Dim Riga_Importo As Decimal = 0
        Dim Riga_Compensazione_Imponibile As Decimal = 0
        Dim Riga_Compensazione_Imposta As Decimal = 0
        Dim Riga_Compensazione_Compensazione As Decimal = 0
        Dim Riga_Compensazione_Totale As Decimal = 0
        Dim Riga_NonCompensazione_Imponibile As Decimal = 0
        Dim Riga_NonCompensazione_Imposta As Decimal = 0
        Dim Riga_NonCompensazione_Compensazione As Decimal = 0
        Dim Riga_NonCompensazione_Totale As Decimal = 0

        Dim Iva_Compensazione_Perc As Decimal = 0
        'Dim IvaCompensazione As Decimal = 0
        Dim Flag_Credito_Imposta_Export As Integer = 0
        Dim objIVA As New AgronicaCoreMetaSchemaDAL.IVA_Aliquote_R
        Dim objHLP As New AgronicaCoreContabHLP.Contabilita
        Dim DT_IVAaliquote As DataTable

        If Not IsNothing(dv) AndAlso dv.Count > 0 Then
            Dim i As Integer

            DT_IVAaliquote = objIVA.Leggi(0, 0, -1, -1, NATURA_ESCLUSIONE_NOFILTRO, "",
                                         "", _
                                         objParametri_Server)

            For i = 0 To dv.Count - 1

                With dv(i)

                    '31/01/2018: 
                    'gestita l'informazione da tabella e non da select case

                    'Select Case .Item("cod_iva")
                    '    'vendite estere non imponibili=>non c'è iva, ma c'è compensazione su iva teorica
                    '    '   ==> vanno riportate a parte e non dentro a compensazione
                    '    Case 78, 95, 67, 83, 84
                    '        Flag_EstereEsenti = True
                    '    Case Else
                    '        Flag_EstereEsenti = False
                    'End Select

                    Flag_Credito_Imposta_Export = objHLP.Cod_from_Cod(DT_IVAaliquote, "Codice", "Flag_Credito_Imposta_Export", .Item("cod_iva"))

                    If Flag_Credito_Imposta_Export = 1 Then
                        Flag_EstereEsenti = True
                    Else
                        Flag_EstereEsenti = False
                    End If

                    Riga_Imponibile = .Item("imponibile_netto")
                    Riga_Imposta = .Item("iva")
                    Riga_Compensazione = .Item("iva_indetraibile")
                    Riga_Compensazione_Export = 0
                    Riga_Importo = .Item("Totale")

                    Totale_Imponibile += Riga_Imponibile
                    Totale_Imposta += Riga_Imposta
                    'Totale_Compensazione += Riga_Compensazione
                    Totale_Importo += Riga_Importo

                    Iva_Compensazione_Perc = .Item("iva_indetraibile_perc")

                    'di tutto questo servono solo i totali dell'iva in compensazione e non in compensazione
                    'il totale non c'è nel report
                    'l'imponibile è stampato solo nella riga di riepilogo totale
                    Select Case Iva_Compensazione_Perc

                        Case 0
                            Riga_NonCompensazione_Imponibile = Riga_Imponibile
                            Riga_NonCompensazione_Imposta = Riga_Imposta
                            Riga_NonCompensazione_Compensazione = Riga_Compensazione
                            Riga_NonCompensazione_Totale = Riga_Importo

                            Totale_NonCompensazione_Imponibile += Riga_NonCompensazione_Imponibile
                            Totale_NonCompensazione_Imposta += Riga_NonCompensazione_Imposta
                            Totale_NonCompensazione_Totale += Riga_NonCompensazione_Totale

                        Case Is <> 0

                            '  Giulia, 18/10/2016 16.51.04: La compensazione va calcolata anche quando c'è esclusione iva, sulla base imponibile dell'iva teorica

                            'MODIFICA DEL 04/08/2015:
                            'visto che ora l'iva in compensazione si calcola sull'imponibile
                            'dalla modifica precedente veniva calcolata anche sulle righe di esclusione iva
                            '(perchè l'imponibile era valorizzato)
                            'introdotto quindi controllo per evitare l'errore

                            If False Then
                                'If Riga_Imposta = 0 Then
                                'sono gli articoli esclusione iva

                                Riga_NonCompensazione_Imponibile = Riga_Imponibile
                                Riga_NonCompensazione_Imposta = Riga_Imposta
                                Riga_NonCompensazione_Compensazione = Riga_Compensazione
                                Riga_NonCompensazione_Totale = Riga_Importo

                                Totale_NonCompensazione_Imponibile += Riga_NonCompensazione_Imponibile
                                Totale_NonCompensazione_Imposta += Riga_NonCompensazione_Imposta
                                Totale_NonCompensazione_Totale += Riga_NonCompensazione_Totale

                            Else

                                'questi dati fin da subito non erano calcolati, sono totali che non vengono visualizzati nel report
                                Riga_Compensazione_Imponibile = 0
                                Riga_Compensazione_Totale = 0

                                '***************************************************************
                                'MAGA 24/01/2018: importo da non calcolare ma da leggere da db

                                'IvaCompensazione = Riga_Imponibile * Iva_Compensazione_Perc / 100

                                If Flag_EstereEsenti Then
                                    Riga_Compensazione_Imposta = 0
                                    'Riga_Compensazione_Export = ArrotondaVal_2(IvaCompensazione)
                                    Riga_Compensazione_Export = Riga_Compensazione
                                    Riga_Compensazione = 0
                                Else
                                    '  Giulia, 14/11/2016 17.30.58: Ricalcolo forzatamente la compensazione raggruppata per evitare errori di arrotondamento
                                    'Riga_Compensazione = ArrotondaVal_2(IvaCompensazione)
                                    'Riga_Compensazione_Imposta = ArrotondaVal_2(IvaCompensazione)
                                    Riga_Compensazione_Imposta = Riga_Compensazione
                                    Riga_Compensazione_Export = 0
                                End If
                                '***************************************************************

                                Totale_Compensazione_Imponibile += Riga_Compensazione_Imponibile
                                Totale_Compensazione_Imposta += Riga_Compensazione_Imposta
                                Totale_Compensazione_Totale += Riga_Compensazione_Totale
                                Totale_Compensazione_Export_Totale += Riga_Compensazione_Export

                                Riga_NonCompensazione_Imponibile = Riga_Imponibile - Riga_Compensazione_Imponibile
                                Riga_NonCompensazione_Imposta = Riga_Imposta - Riga_Compensazione_Imposta
                                Riga_NonCompensazione_Totale = Riga_Importo - Riga_Compensazione_Totale

                                Totale_NonCompensazione_Imponibile += Riga_NonCompensazione_Imponibile
                                Totale_NonCompensazione_Imposta += Riga_NonCompensazione_Imposta
                                Totale_NonCompensazione_Totale += Riga_NonCompensazione_Totale

                            End If



                    End Select

                    Inserisci_Riga_sottoReportIVA_DUPLICATO(DS_SR_IVA, _
                                                            titolo, _
                                                            .Item("cod_iva"), _
                                                            .Item("aliquota_des"), _
                                                            Riga_Imponibile,
                                                            Riga_Imposta, _
                                                            Riga_Compensazione, _
                                                            Riga_Compensazione_Export, _
                                                            Riga_Importo, _
                                                            Totale_Imponibile, _
                                                            Totale_Imposta, _
                                                            Totale_Importo, _
                                                            Totale_Compensazione_Imponibile, _
                                                            Totale_Compensazione_Imposta, _
                                                            Totale_Compensazione_Totale, _
                                                            Totale_NonCompensazione_Imponibile, _
                                                            Totale_NonCompensazione_Imposta, _
                                                            Totale_NonCompensazione_Totale, _
                                                            Totale_Compensazione_Export_Totale)

                End With

            Next

        End If

    End Sub

    '########################################################################
    'iva_indetraibile_perc:
    'lato acquisto è l'iva indetraibile
    'lato vendita è l'iva in compensazione
    Public Sub DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA(ByVal DT_IVA_Round As DataTable, _
                                                        ByRef DT_IVA_Generale As DataTable, _
                                                        ByVal Flag_Acquisto As Boolean)

        Dim z, w As Integer
        Dim ImponibPiuIVA As Decimal
        Dim Trovato As Boolean
        Dim aliquota_des As String

        Dim GeneraleIva, GeneraleImponibile, GeneraleIndetraibile As Decimal

        For w = 0 To DT_IVA_Round.Rows.Count - 1

            Trovato = False

            If Not IsNothing(DT_IVA_Generale) And DT_IVA_Generale.Rows.Count > 0 Then

                'il dt generale contiene già dei codici iva
                For z = 0 To DT_IVA_Generale.Rows.Count - 1

                    If DT_IVA_Round.Columns.Contains("iva_indetraibile_perc") = True Then

                        '--------------------------------------------------------------------------
                        '------------  CASO REGISTRI IVA / LIQUIDAZIONE IVA -----------------------
                        '--------------------------------------------------------------------------

                        'If DT_IVA_Round.Rows(w).Item("cod_iva") = DT_IVA_Generale.Rows(z).Item("cod_iva") Then
                        'If DT_IVA_Round.Rows(w).Item("cod_iva") = DT_IVA_Generale.Rows(z).Item("cod_iva") _
                        '    And DT_IVA_Round.Rows(w).Item("iva_indetraibile_perc") = DT_IVA_Generale.Rows(z).Item("iva_indetraibile_perc") Then
                        If DT_IVA_Round.Rows(w).Item("cod_iva") = DT_IVA_Generale.Rows(z).Item("cod_iva") _
                            And DT_IVA_Round.Rows(w).Item("iva_indetraibile_perc") = DT_IVA_Generale.Rows(z).Item("iva_indetraibile_perc") _
                            And DT_IVA_Round.Rows(w).Item("modalita_doc") = DT_IVA_Generale.Rows(z).Item("modalita_doc") Then

                            Trovato = True

                            GeneraleIva = DT_IVA_Generale.Rows(z).Item("iva")
                            GeneraleImponibile = DT_IVA_Generale.Rows(z).Item("imponibile_netto")
                            GeneraleIndetraibile = DT_IVA_Generale.Rows(z).Item("iva_indetraibile")

                            GeneraleIva = ArrotondaVal_2(GeneraleIva + DT_IVA_Round.Rows(w).Item("iva"))
                            GeneraleImponibile = ArrotondaVal_2(GeneraleImponibile + DT_IVA_Round.Rows(w).Item("imponibile_netto"))
                            GeneraleIndetraibile = ArrotondaVal_2(GeneraleIndetraibile + DT_IVA_Round.Rows(w).Item("iva_indetraibile"))

                            ImponibPiuIVA = ArrotondaVal_2(GeneraleIva + GeneraleImponibile)

                            'objSottoIva
                            AggiornaRiga_DtIvaGenerale(DT_IVA_Generale.Rows(z),
                                                       GeneraleImponibile,
                                                       GeneraleIva,
                                                       ImponibPiuIVA,
                                                       GeneraleIndetraibile)
                            Exit For

                        End If


                    Else
                        '--------------------------------------------------------------------------
                        '------------  CASO REGISTRO CORRISPETTIVI -----------------------
                        '--------------------------------------------------------------------------

                        If DT_IVA_Round.Rows(w).Item("cod_iva") = DT_IVA_Generale.Rows(z).Item("cod_iva") Then

                            Trovato = True

                            GeneraleIva = DT_IVA_Generale.Rows(z).Item("iva")
                            GeneraleImponibile = DT_IVA_Generale.Rows(z).Item("imponibile_netto")

                            GeneraleIva = ArrotondaVal_2(GeneraleIva + DT_IVA_Round.Rows(w).Item("iva"))
                            GeneraleImponibile = ArrotondaVal_2(GeneraleImponibile + DT_IVA_Round.Rows(w).Item("imponibile_netto"))

                            ImponibPiuIVA = ArrotondaVal_2(GeneraleIva + GeneraleImponibile)

                            'objSottoIva
                            AggiornaRiga_DtIvaGenerale(DT_IVA_Generale.Rows(z),
                                                        GeneraleImponibile,
                                                        GeneraleIva,
                                                        ImponibPiuIVA)
                            Exit For

                        End If


                    End If

                Next

                'codice iva non trovato
                If Trovato = False Then
                    DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA_Inner(DT_IVA_Round,
                                                                DT_IVA_Generale,
                                                                Flag_Acquisto,
                                                                w)

                End If

            Else
                'il dt generale è ancora vuoto -> inserisco direttamente i riepiloghi iva
                DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA_Inner(DT_IVA_Round,
                                                                 DT_IVA_Generale,
                                                                 Flag_Acquisto,
                                                                  w)

            End If

        Next 'DT_IVA_Round - w

    End Sub

    '########################################################################
    'iva_indetraibile_perc:
    'lato acquisto è l'iva indetraibile
    'lato vendita è l'iva in compensazione
    Private Sub DtIVAGenerale_ValorizzaDa_DtRiepilogoIVA_Inner(ByVal DT_IVA_Round As DataTable,
                                                                ByRef DT_IVA_Generale As DataTable,
                                                                ByVal Flag_Acquisto As Boolean,
                                                                ByVal w As Integer)

        Dim ImponibPiuIVA As Decimal
        Dim aliquota_des As String

        ImponibPiuIVA = DT_IVA_Round.Rows(w).Item("imponibile_netto") + DT_IVA_Round.Rows(w).Item("iva")

        If DT_IVA_Round.Columns.Contains("iva_indetraibile_perc") = True Then

            '--------------------------------------------------------------------------
            '------------  CASO REGISTRI IVA / LIQUIDAZIONE IVA -----------------------
            '--------------------------------------------------------------------------

            If DT_IVA_Round.Rows(w).Item("iva_indetraibile_perc") <> 0 Then
                If Flag_Acquisto = True Then
                    aliquota_des = CStr(DT_IVA_Round.Rows(w).Item("aliquota_des")) & " Indetraibile " & CStr(DT_IVA_Round.Rows(w).Item("iva_indetraibile_perc")) & "%"
                Else
                    aliquota_des = CStr(DT_IVA_Round.Rows(w).Item("aliquota_des")) & " Compensazione " & CStr(DT_IVA_Round.Rows(w).Item("iva_indetraibile_perc")) & "%"
                End If
            Else
                If DT_IVA_Round.Rows(w).Item("modalita_doc") = enum_ModalitaFattura.Fattura_AcquistiIntracom Then
                    aliquota_des = CStr(DT_IVA_Round.Rows(w).Item("aliquota_des")) & " Acq. IntraUE"
                Else
                    aliquota_des = DT_IVA_Round.Rows(w).Item("aliquota_des")
                End If
            End If

            'objSottoIva.
            InserisciRiga_DtIvaGenerale(DT_IVA_Generale,
                                        DT_IVA_Round.Rows(w).Item("imponibile_netto"),
                                        DT_IVA_Round.Rows(w).Item("cod_iva"),
                                        DT_IVA_Round.Rows(w).Item("iva"),
                                        DT_IVA_Round.Rows(w).Item("aliquota_iva"),
                                        aliquota_des,
                                        ImponibPiuIVA,
                                        DT_IVA_Round.Rows(w).Item("iva_indetraibile_perc"),
                                        DT_IVA_Round.Rows(w).Item("iva_indetraibile"),
                                        DT_IVA_Round.Rows(w).Item("modalita_doc"))




        Else

            '--------------------------------------------------------------------------
            '------------  CASO REGISTRO CORRISPETTIVI -----------------------
            '--------------------------------------------------------------------------

            aliquota_des = DT_IVA_Round.Rows(w).Item("aliquota_des")

            'objSottoIva.
            InserisciRiga_DtIvaGenerale(DT_IVA_Generale, _
                                        DT_IVA_Round.Rows(w).Item("imponibile_netto"), _
                                        DT_IVA_Round.Rows(w).Item("cod_iva"), _
                                        DT_IVA_Round.Rows(w).Item("iva"), _
                                        DT_IVA_Round.Rows(w).Item("aliquota_iva"), _
                                        aliquota_des, _
                                        ImponibPiuIVA, _
                                        0, _
                                        0, _
                                        0)

        End If


    End Sub


End Class
