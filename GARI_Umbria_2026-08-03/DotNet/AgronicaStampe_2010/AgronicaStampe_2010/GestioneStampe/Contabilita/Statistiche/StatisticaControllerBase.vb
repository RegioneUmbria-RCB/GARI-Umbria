Imports AgronicaCoreDataProvider
Imports System.Globalization
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class StatisticaControllerBase

    Public _piva As String
    Public _numSin As String
    Public _num As String
    Public _numDes As String
    Public _nrRiga As String
    Public _dataDal As String
    Public _dataAl As String

    Public _clienti As String
    Public _agenti As String
    Public _causali As String
    Public _specie As String
    Public _varieta As String
    Public _prodotti As String
    Public _categorie As String
    Public _categorieCommerciali
    Public _cauTrasp As String
    Public _tipoReport As String
    Public _titolo As String
    Public _livelli As String
    Public _daMese As String
    Public _daAnno As String
    Public _aMese As String
    Public _aAnno As String
    Public _confrAnno As String
    Public _tipoValore As String
    Public _scostamento As String
    Public _saltoPagina1Liv As String
    Public _meseScost As String
    Public _report As String
    Public _ordinaXValore
    Public _rapportiContabili
    Public _nazioniFatturazione
    Public _tipoValore2 As String
    Public _objParametriServer As AgronicaCoreParametri
    Public _objParametriUtenti As AgronicaCoreParametri
    Public _decimali_qta As String

    Private _tuttiMesi As List(Of Tuple(Of Integer, Integer)) = Nothing
    Private _piuEsercizi As Boolean = False
    Private _mesiAnnoPrincipale As List(Of Tuple(Of Integer, Integer)) = Nothing
    Public _includiCorrispettivi As Boolean


    Public Shared Function ControllerFactory(ByVal piva As String,
        ByVal numSin As String,
        ByVal num As String,
        ByVal numDes As String,
        ByVal nrRiga As String,
        ByVal dataDal As String,
        ByVal dataAl As String,
        ByVal clienti As String,
        ByVal agenti As String,
        ByVal causali As String,
        ByVal specie As String,
        ByVal varieta As String,
        ByVal prodotti As String,
        ByVal categorie As String,
        ByVal categorieCommerciali As String,
        ByVal cauTrasp As String,
        ByVal tipoReport As String,
        ByVal titolo As String,
        ByVal livelli As String,
        ByVal daMese As String,
        ByVal daAnno As String,
        ByVal aMese As String,
        ByVal aAnno As String,
        ByVal confrAnno As String,
        ByVal tipoValore As String,
        ByVal scostamento As String,
        ByVal saltoPagina1Liv As String,
        ByVal meseScost As String,
        ByVal report As String,
        ByVal ordinaXValore As String,
        ByVal rapportiContabili As String,
        ByVal nazioniFatturazione As String,
        ByVal tipoValore2 As String,
        ByVal decimali_qta As String,
        ByVal includiCorrispettivi As Boolean,
        ByRef objParametriServer As AgronicaCoreParametri,
        ByRef objParametriUtenti As AgronicaCoreParametri) As StatisticaControllerBase

        Dim controller As StatisticaControllerBase = Nothing

        Select Case CInt(tipoReport)
            Case enum_Tipo_Stampa_Statistica.Statistica_mese_anno_confronto_fra_anni
                controller = New Statistica_Confronto_MeseAnniController(piva, numSin, num, numDes,
                                                                nrRiga, dataDal, dataAl, clienti,
                                                                agenti, causali, specie, varieta, prodotti, categorie,
                                                                categorieCommerciali, cauTrasp, tipoReport, titolo, livelli, daMese,
                                                                daAnno, aMese, aAnno, confrAnno, tipoValore, scostamento,
                                                                saltoPagina1Liv, meseScost, report, ordinaXValore,
                                                                rapportiContabili, nazioniFatturazione, tipoValore2, decimali_qta, includiCorrispettivi, objParametriServer, objParametriUtenti)

            Case enum_Tipo_Stampa_Statistica.Statistica_mese_singolo_anno_su_quantità_altro_Valore
                controller = New StatisticaMeseAnnoController(piva, numSin, num, numDes,
                                                                nrRiga, dataDal, dataAl, clienti,
                                                                agenti, causali, specie, varieta, prodotti, categorie,
                                                                categorieCommerciali, cauTrasp, tipoReport, titolo, livelli, daMese,
                                                                daAnno, aMese, aAnno, confrAnno, tipoValore, scostamento,
                                                                saltoPagina1Liv, meseScost, report, ordinaXValore,
                                                                rapportiContabili, nazioniFatturazione, tipoValore2, decimali_qta, includiCorrispettivi, objParametriServer, objParametriUtenti)

            Case enum_Tipo_Stampa_Statistica.Statistica_anno_con_scostamento_e_previsioni
                controller = New StatAnnoConPrevisioneController(piva, numSin, num, numDes,
                                                                nrRiga, dataDal, dataAl, clienti,
                                                                agenti, causali, specie, varieta, prodotti, categorie,
                                                                categorieCommerciali, cauTrasp, tipoReport, titolo, livelli, daMese,
                                                                daAnno, aMese, aAnno, confrAnno, tipoValore, scostamento,
                                                                saltoPagina1Liv, meseScost, report, ordinaXValore,
                                                                rapportiContabili, nazioniFatturazione, tipoValore2, decimali_qta, includiCorrispettivi, objParametriServer, objParametriUtenti)

            Case Else
                Throw New NotImplementedException("Tipo report non ancora supportato")

        End Select

        Return controller

    End Function

    Public Sub New(ByVal piva As String,
        ByVal numSin As String,
        ByVal num As String,
        ByVal numDes As String,
        ByVal nrRiga As String,
        ByVal dataDal As String,
        ByVal dataAl As String,
        ByVal clienti As String,
        ByVal agenti As String,
        ByVal causali As String,
        ByVal specie As String,
        ByVal varieta As String,
        ByVal prodotti As String,
        ByVal categorie As String,
        ByVal categorieCommerciali As String,
        ByVal cauTrasp As String,
        ByVal tipoReport As String,
        ByVal titolo As String,
        ByVal livelli As String,
        ByVal daMese As String,
        ByVal daAnno As String,
        ByVal aMese As String,
        ByVal aAnno As String,
        ByVal confrAnno As String,
        ByVal tipoValore As String,
        ByVal scostamento As String,
        ByVal saltoPagina1Liv As String,
        ByVal meseScost As String,
        ByVal report As String,
        ByVal ordinaXValore As String,
        ByVal rapportiContabili As String,
        ByVal nazioniFatturazione As String,
        ByVal tipoValore2 As String,
        ByVal decimali_qta As String,
        ByVal includiCorrispettivi As Boolean,
        ByRef objParametriServer As AgronicaCoreParametri,
        ByRef objParametriUtenti As AgronicaCoreParametri)

        _piva = piva
        _numSin = numSin
        _num = num
        _numDes = numDes
        _nrRiga = nrRiga
        _dataDal = dataDal
        _dataAl = dataAl
        _clienti = clienti
        _agenti = agenti
        _causali = causali
        _specie = specie
        _varieta = varieta
        _prodotti = prodotti
        _categorie = categorie
        _categorieCommerciali = categorieCommerciali
        _cauTrasp = cauTrasp
        _tipoReport = tipoReport
        _titolo = titolo
        _livelli = livelli
        _daMese = daMese
        _aMese = aMese
        _daAnno = daAnno
        _aAnno = aAnno
        _confrAnno = confrAnno
        _tipoValore = tipoValore
        _scostamento = scostamento
        _saltoPagina1Liv = saltoPagina1Liv
        _meseScost = meseScost
        _report = report
        _ordinaXValore = ordinaXValore
        _rapportiContabili = rapportiContabili
        _nazioniFatturazione = nazioniFatturazione
        _tipoValore2 = tipoValore2
        _decimali_qta = decimali_qta
        _includiCorrispettivi = includiCorrispettivi
        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti

    End Sub

    Public Overridable Function RiempiDataSourceStampa() As DataSet
        Return Nothing
    End Function

    Public Overridable Function Dammi_Nome_File_Report() As String
        Return String.Empty
    End Function

    Public Overridable Sub FillDataTableFromList(ByVal lista As List(Of RecordStatistica), ByRef dataSet As DS_Stat_12_Mesi)

        For Each rs As RecordStatistica In lista
            Dim row = dataSet.DT_Dati.NewDT_DatiRow()
            row.piva = rs.Piva
            row.Anno_Movimento = rs.Anno_Movimento
            row.Esercizio = rs.Esercizio
            row.UnitaMisura = rs.UnitaMisura
            row.CodLiv1 = rs.CodLiv1
            row.CodLiv2 = rs.CodLiv2
            row.CodLiv3 = rs.CodLiv3
            row.DesLiv1 = rs.DesLiv1
            row.DesLiv2 = rs.DesLiv2
            row.DesLiv3 = rs.DesLiv3
            row.Val_01 = rs.Val_01
            row.Val_02 = rs.Val_02
            row.Val_03 = rs.Val_03
            row.Val_04 = rs.Val_04
            row.Val_05 = rs.Val_05
            row.Val_06 = rs.Val_06
            row.Val_07 = rs.Val_07
            row.Val_08 = rs.Val_08
            row.Val_09 = rs.Val_09
            row.Val_10 = rs.Val_10
            row.Val_11 = rs.Val_11
            row.Val_12 = rs.Val_12
            row.ValC_01 = rs.ValC_01
            row.ValC_02 = rs.ValC_02
            row.ValC_03 = rs.ValC_03
            row.ValC_04 = rs.ValC_04
            row.ValC_05 = rs.ValC_05
            row.ValC_06 = rs.ValC_06
            row.ValC_07 = rs.ValC_07
            row.ValC_08 = rs.ValC_08
            row.ValC_09 = rs.ValC_09
            row.ValC_10 = rs.ValC_10
            row.ValC_11 = rs.ValC_11
            row.ValC_12 = rs.ValC_12
            row.CodDet = rs.CodDet
            row.DesDet = rs.DesDet
            row.Totale = rs.Totale
            dataSet.DT_Dati.Rows.Add(row)

        Next

    End Sub

    Public Overridable Function DataTableToList(ByVal table As DataTable,
                                       ByVal riempi_secondo_valore As Boolean,
                                       Optional ByVal soloRefernza As Boolean = False) As List(Of RecordStatistica)

        Dim records As New List(Of RecordStatistica)

        For Each row As DataRow In table.AsEnumerable()

            Dim rs As New RecordStatistica()

            If soloRefernza Then

                rs.CodDet = row.Item("Referenza_Codice").ToString()
                rs.DesDet = row.Item("Referenza_Descr").ToString()

            Else

                rs.Piva = row.Item("piva").ToString()
                rs.Anno_Movimento = row.Item("Anno_Movimento").ToString()
                rs.Esercizio = row.Item("Esercizio").ToString()
                rs.UnitaMisura = row.Item("Unita_Misura_Sigla").ToString()
                rs.TipoRecord = 0
                rs.CodLiv1 = row.Item("CodLiv1").ToString()
                rs.CodLiv2 = row.Item("CodLiv2").ToString()
                rs.CodLiv3 = row.Item("CodLiv3").ToString()
                rs.DesLiv1 = row.Item("DesLiv1").ToString()
                rs.DesLiv2 = row.Item("DesLiv2").ToString()
                rs.DesLiv3 = row.Item("DesLiv3").ToString()
                rs.CodDet = row.Item("CodDet").ToString()
                rs.DesDet = row.Item("DesDet").ToString()
                rs.Val_01 = CDec(row.Item("Val_01"))
                rs.Val_02 = CDec(row.Item("Val_02"))
                rs.Val_03 = CDec(row.Item("Val_03"))
                rs.Val_04 = CDec(row.Item("Val_04"))
                rs.Val_05 = CDec(row.Item("Val_05"))
                rs.Val_06 = CDec(row.Item("Val_06"))
                rs.Val_07 = CDec(row.Item("Val_07"))
                rs.Val_08 = CDec(row.Item("Val_08"))
                rs.Val_09 = CDec(row.Item("Val_09"))
                rs.Val_10 = CDec(row.Item("Val_10"))
                rs.Val_11 = CDec(row.Item("Val_11"))
                rs.Val_12 = CDec(row.Item("Val_12"))
                If riempi_secondo_valore Then
                    rs.ValC_01 = CDec(row.Item("Val_01_2"))
                    rs.ValC_02 = CDec(row.Item("Val_02_2"))
                    rs.ValC_03 = CDec(row.Item("Val_03_2"))
                    rs.ValC_04 = CDec(row.Item("Val_04_2"))
                    rs.ValC_05 = CDec(row.Item("Val_05_2"))
                    rs.ValC_06 = CDec(row.Item("Val_06_2"))
                    rs.ValC_07 = CDec(row.Item("Val_07_2"))
                    rs.ValC_08 = CDec(row.Item("Val_08_2"))
                    rs.ValC_09 = CDec(row.Item("Val_09_2"))
                    rs.ValC_10 = CDec(row.Item("Val_10_2"))
                    rs.ValC_11 = CDec(row.Item("Val_11_2"))
                    rs.ValC_12 = CDec(row.Item("Val_12_2"))
                End If
            End If

            records.Add(rs)

        Next

        If Not soloRefernza Then
            For Each r In records
                r.Key = String.Format("{0}{1}{2}{3}{4}{5}{6}", r.Piva, r.CodLiv1, r.CodLiv2, r.CodLiv3, r.CodDet, r.DesDet, r.UnitaMisura)
                r.Totale = r.Val_01 + r.Val_02 + r.Val_03 + r.Val_04 + r.Val_05 + r.Val_06 + r.Val_07 + r.Val_08 + r.Val_09 + r.Val_10 + r.Val_11 + r.Val_12
            Next
        End If

        Return records

    End Function

    Protected Function OttieniDescrizioneUDM(ByVal tipoValore As String) As Tuple(Of String, String)

        Select Case tipoValore

            Case "0"
                Return New Tuple(Of String, String)("Q.tà", "Quantità")
            Case "1"
                Return New Tuple(Of String, String)("Imp. netto", "Imponibile Netto")
            Case "2"
                Return New Tuple(Of String, String)("Importo", "Importo")
            Case "3"
                Return New Tuple(Of String, String)("Prov.", "Provvigioni")
            Case "4"
                Return New Tuple(Of String, String)("Kg / Lt ", "Kg / Lt")
            Case "5"
                Return New Tuple(Of String, String)("Pezzi / Nr", "Pezzi / Nr")
            Case Else
                Return New Tuple(Of String, String)("N.d.", "N.d.")

        End Select

    End Function

    Protected Function OttieniDescrizioneLivello(ByVal livello As String) As String

        Select Case livello.ToLower()
            Case "cliente"
                Return "Cliente"
            Case "fornitore"
                Return "Fornitore"
            Case "agente"
                Return "Agente"
            Case "destinazione"
                Return "Destinazione"
            Case "provenienza"
                Return "Provenienza"
            Case "prodotto"
                Return "Prodotto"
            Case "capo_area"
                Return "Capo Area"
            Case "categoria_commerciale"
                Return "Cat. Comm."
            Case "categoria_prodotto"
                Return "Cat. Prod."
            Case "stato"
                Return "Nazione"
            Case "stato_dest"
                Return "Nazione Destinazione"
            Case "stato_proven"
                Return "Nazione Provenienza"
            Case "rapporto"
                Return "Tipo Rap. Contab."
            Case "impresa"
                Return "Ragione Sociale Impresa"
            Case Else
                Return String.Empty
        End Select

    End Function

    Protected Function OttieniDescrizioniCampiDettaglio(ByVal livello) As Tuple(Of String, String)

        Select Case livello.ToLower()
            Case "cliente"
                Return New Tuple(Of String, String)("", "Cliente / Rag. Soc.")
            Case "fornitore"
                Return New Tuple(Of String, String)("", "Fornitore / Rag. Soc.")
            Case "agente"
                Return New Tuple(Of String, String)("", "Agente")
            Case "destinazione"
                Return New Tuple(Of String, String)("", "Destinazione")
            Case "provenienza"
                Return New Tuple(Of String, String)("", "Provenienza")
            Case "prodotto"
                Return New Tuple(Of String, String)("Cod.Art.", "Articolo")
            Case "capo_area"
                Return New Tuple(Of String, String)("", "Capo Area")
            Case "categoria_commerciale"
                Return New Tuple(Of String, String)("", "Catecoria Commerciale")
            Case "categoria_prodotto"
                Return New Tuple(Of String, String)("", "Categoria Prodotto")
            Case "stato"
                Return New Tuple(Of String, String)("", "Nazione")
            Case "stato_dest"
                Return New Tuple(Of String, String)("", "Nazione Destinazione")
            Case "stato_proven"
                Return New Tuple(Of String, String)("", "Nazione Provenienza")
            Case "impresa"
                Return New Tuple(Of String, String)("", "Ragione Sociale Impresa")
            Case Else
                Return New Tuple(Of String, String)("", "")
        End Select


    End Function

    Protected Function MostraCampoCodiceDettaglio(ByVal livello As String) As Boolean

        Select Case livello.ToLower()
            Case "cliente"
                Return False
            Case "fornitore"
                Return False
            Case "agente"
                Return False
            Case "destinazione"
                Return False
            Case "provenienza"
                Return False
            Case "prodotto"
                Return False
            Case "capo_area"
                Return False
            Case "categoria_commerciale"
                Return False
            Case "categoria_prodotto"
                Return False
            Case "stato"
                Return False
            Case "stato_dest"
                Return False
            Case "stato_proven"
                Return False
            Case "rapporto"
                Return False
            Case "impresa"
                Return False
            Case Else
                Return False
        End Select

    End Function

    Protected Function DescrizionePeriodoGenerale() As String

        If String.IsNullOrEmpty(_confrAnno) OrElse _confrAnno = "0" Then
            Return "Periodo: " + DescrizionePeriodoPrincipale()
        Else
            Return String.Format("Periodi: {0}   -   {1}", DescrizionePeriodoPrincipale(), DescrizionePeriodoConfronto())
        End If

    End Function

    Protected Function DescrizionePeriodoPrincipale() As String

        Return String.Format("{0}/{1} - {2}/{1}", _daMese.PadLeft(2, "0"), _daAnno, _aMese.PadLeft(2, "0"))

    End Function

    Protected Function DescrizionePeriodoConfronto() As String

        If String.IsNullOrEmpty(_confrAnno) OrElse _confrAnno = "0" Then
            Return String.Empty
        End If

        Return String.Format("{0}/{1} - {2}/{1}", _daMese.PadLeft(2, "0"), _confrAnno, _aMese.PadLeft(2, "0"))

    End Function

    Protected Sub OttieniDescrizioniPeriodiPerUnEsercizio(ByVal mesiAnni As List(Of Tuple(Of Integer, Integer)),
                         ByRef descrizionePeriodo As String,
                         ByRef descrizioneAnno1 As String)

        Dim mesiAnniEsercizio = (From m In mesiAnni.Take(12)
                                 Order By m.Item2 Ascending
                                 Group By anno = m.Item2
                                      Into mesi = Group, Count()
                                 Order By anno).ToList()

        If mesiAnniEsercizio.Count() = 1 Then
            descrizionePeriodo = "Anno: " + mesiAnniEsercizio.FirstOrDefault().anno.ToString()
            descrizioneAnno1 = mesiAnniEsercizio.FirstOrDefault().anno.ToString()
        Else
            descrizionePeriodo = String.Format("Periodi: {0}/{1} - {2}/{3}",
                                               mesiAnniEsercizio.FirstOrDefault().mesi.FirstOrDefault().Item1.ToString().PadLeft(2, "0"),
                                               mesiAnniEsercizio.FirstOrDefault().anno.ToString(),
                                               mesiAnniEsercizio.Skip(1).FirstOrDefault().mesi.LastOrDefault().Item1.ToString().PadLeft(2, "0"),
                                               mesiAnniEsercizio.Skip(1).FirstOrDefault().anno.ToString())
            descrizioneAnno1 = String.Format("{0}/{1} - {2}/{3}",
                                               mesiAnniEsercizio.FirstOrDefault().mesi.FirstOrDefault().Item1.ToString().PadLeft(2, "0"),
                                               mesiAnniEsercizio.FirstOrDefault().anno.ToString(),
                                               mesiAnniEsercizio.Skip(1).FirstOrDefault().mesi.LastOrDefault().Item1.ToString().PadLeft(2, "0"),
                                               mesiAnniEsercizio.Skip(1).FirstOrDefault().anno.ToString())
        End If

    End Sub

    Protected Sub Setta_Periodo_Temporale(ByVal dataDal As DateTime, ByVal dataAl As DateTime)

        _tuttiMesi = MonthsBetween(Convert.ToDateTime(dataDal), Convert.ToDateTime(dataAl)).ToList()
        _piuEsercizi = _tuttiMesi.Count() > 12

        If _piuEsercizi Then
            _mesiAnnoPrincipale = _tuttiMesi.Skip(12).Take(12).ToList()
        Else
            _mesiAnnoPrincipale = _tuttiMesi
        End If

    End Sub

    Protected Function Mese_Visibile(ByVal indiceMese As Integer) As Boolean


        Dim mese = _mesiAnnoPrincipale(indiceMese - 1).Item1
        If mese >= CInt(_daMese) AndAlso mese <= CInt(_aMese) Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Function MonthsBetween(ByVal startDate As DateTime, ByVal endDate As DateTime) As List(Of Tuple(Of Integer, Integer))

        Dim retVal = New List(Of Tuple(Of Integer, Integer))


        Dim iterator As DateTime
        Dim limit As DateTime

        If endDate > startDate Then
            iterator = New DateTime(startDate.Year, startDate.Month, 1)
            limit = endDate
        Else
            iterator = New DateTime(endDate.Year, endDate.Month, 1)
            limit = startDate
        End If

        Dim dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat

        While iterator <= limit
            retVal.Add(Tuple.Create(iterator.Month, iterator.Year))
            iterator = iterator.AddMonths(1)
        End While

        Return retVal

    End Function

End Class
