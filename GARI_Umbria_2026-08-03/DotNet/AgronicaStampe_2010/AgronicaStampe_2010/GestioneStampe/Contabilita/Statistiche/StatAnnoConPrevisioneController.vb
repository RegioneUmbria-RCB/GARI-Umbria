Imports AgronicaCoreDataProvider
Imports AgronicaCoreContabDAL

Public Class StatAnnoConPrevisioneController : Inherits StatisticaControllerBase

    Private _nomeFileReport As String = "StatAnnoConPrevisione.rpt"
    Private _nomeFileReportOrdinatoValore As String = "StatAnnoConPrevisioneOrdVal.rpt"

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

        MyBase.New(piva, numSin, num, numDes, nrRiga, dataDal, dataAl, clienti,
                agenti, causali, specie, varieta, prodotti, categorie,
                categorieCommerciali, cauTrasp, tipoReport, titolo, livelli, daMese,
                daAnno, aMese, aAnno, confrAnno, tipoValore, scostamento,
                saltoPagina1Liv, meseScost, report, ordinaXValore,
                rapportiContabili, nazioniFatturazione, tipoValore2, decimali_qta, includiCorrispettivi, objParametriServer, objParametriUtenti)
    End Sub

    Public Overrides Function Dammi_Nome_File_Report() As String

        Dim boolTester As Boolean
        Dim nomeFile As String = _nomeFileReport

        If Boolean.TryParse(_ordinaXValore, boolTester) Then
            If boolTester Then
                nomeFile = _nomeFileReportOrdinatoValore
            End If
        End If

        Return nomeFile

    End Function

    Public Overrides Function RiempiDataSourceStampa() As System.Data.DataSet

        Dim boolTester As Boolean

        Dim statR As New Statistiche_R
        Dim newDataDal As DateTime
        Dim newDataAl As DateTime

        If String.IsNullOrEmpty(_daMese) Then
            _daMese = "1"
        End If
        If String.IsNullOrEmpty(_aMese) Then
            _aMese = "12"
        End If
        If String.IsNullOrEmpty(_daAnno) Then
            _daAnno = Now.Year.ToString()
        End If

        If Not String.IsNullOrEmpty(_daMese) And Not String.IsNullOrEmpty(_daAnno) Then
            newDataDal = New DateTime(CInt(_daAnno), CInt(_daMese), 1)
            Dim dirtyDataAl = newDataDal.AddMonths(11)

            Dim lastDayOfMonth = DateTime.DaysInMonth(dirtyDataAl.Year, dirtyDataAl.Month)
            newDataAl = New DateTime(dirtyDataAl.Year, dirtyDataAl.Month, lastDayOfMonth)
            _dataDal = newDataDal.ToString("yyyy/MM/dd")
            _dataAl = newDataAl.ToString("yyyy/MM/dd")

        End If

        If Not String.IsNullOrEmpty(_confrAnno) AndAlso Not _confrAnno = "0" Then
            newDataDal = New DateTime(CInt(_confrAnno), CInt(_daMese), 1)
            _dataDal = newDataDal.ToString("yyyy/MM/dd")
        End If
        Setta_Periodo_Temporale(_dataDal, _dataAl)

        ' Ottengo periodo di esercizio
        Dim tuttiMesi = statR.MonthsBetween(Convert.ToDateTime(_dataDal), Convert.ToDateTime(_dataAl)).ToList()
        Dim piuEsercizi As Boolean = tuttiMesi.Count() > 12

        Dim MostraSoloRiepilogoProdotti As Boolean
        Dim listaDatiZero = New List(Of RecordStatistica)
        Dim datiPrecheck As DataTable
        ' Se selezionato tipo valore 4 (kg/pz) o 5 (nr) verifica se esiste anche solo una riga con totale vuoto
        ' in questo caso il report mostrera solo una sezione di riepilogo dei prodotti a 0
        If _tipoValore = "4" OrElse _tipoValore = "5" Then
            datiPrecheck = statR.Precheck_Righe_Report_Vendite(
            _piva,
            _report,
            False,
             _numSin, _num, _numDes,
             _nrRiga,
             _dataDal, _dataAl,
             _clienti,
            _agenti,
            _causali,
            _specie,
            _varieta,
            _prodotti,
            _categorie,
            _categorieCommerciali,
            _cauTrasp,
            "",
            _tipoValore,
            _livelli,
            _rapportiContabili,
            _nazioniFatturazione,
            _tipoValore2, _includiCorrispettivi,
            _objParametriServer, _objParametriUtenti)

            listaDatiZero = DataTableToList(datiPrecheck, False, True)

            If datiPrecheck.Rows.Count > 0 Then
                MostraSoloRiepilogoProdotti = True
            End If

        End If

        ' Ottengo dataSource finale del report
        Dim listaSourceFinale = New List(Of RecordStatistica)
        Dim dsStat As New DS_Stat_12_Mesi()

        ' report con procedimento normale (tipo valore <> 4 e 5)
        Dim dati = statR.Leggi_Righe_Report_Vendite(
               _piva,
               _report,
               False,
               _numSin, _num, _numDes,
               _nrRiga,
               _dataDal, _dataAl,
               _clienti,
               _agenti,
               _causali,
               _specie,
               _varieta,
               _prodotti,
               _categorie,
               _categorieCommerciali,
               _cauTrasp,
               "",
               _tipoValore,
               _livelli,
               _rapportiContabili,
               _nazioniFatturazione,
               _tipoValore2,
               _daMese, _aMese, _tipoReport, _includiCorrispettivi,
                _objParametriServer, _objParametriUtenti)

        ' Trasformo la dataTable in Lista
        Dim listaDati = DataTableToList(dati, False, False)

        If piuEsercizi Then
            Dim primario = listaDati.Where(Function(p) p.Esercizio.ToLower().Equals("primario")).ToList()

            ' Records Esercizio Secondario
            Dim secondario = listaDati.Where(Function(p) p.Esercizio.ToLower().Equals("secondario")).ToList()

            Dim comparer As New RecStatEqualityComparer()
            listaSourceFinale = FullJoinEsercizi(primario, secondario).Distinct(comparer).ToList()
        Else
            listaSourceFinale = listaDati
        End If

        FillDataTableFromList(listaSourceFinale, dsStat)

        ' parametri report
        Dim drParametri As DS_Stat_12_Mesi.DT_ParametriRow = dsStat.DT_Parametri.NewDT_ParametriRow

        ' mostrare solo riepilogo prodotti a 0
        drParametri.MostraSoloRiepilogoProdotti = MostraSoloRiepilogoProdotti

        Dim raggruppamenti = _livelli.Split("|").ToList()
        dsStat.DT_Parametri.Rows.Add(drParametri)

        drParametri.TitoloReport = _titolo

        Dim desUdm = OttieniDescrizioneUDM(_tipoValore)
        drParametri.UnitaMisuraAbbreviata = desUdm.Item1
        drParametri.UnitaMisuraEstesa = desUdm.Item2

        drParametri.DesGruppo1 = OttieniDescrizioneLivello(raggruppamenti.FirstOrDefault())
        drParametri.DesGruppo2 = If(raggruppamenti.Count > 1, raggruppamenti.Skip(1).FirstOrDefault(), "")
        drParametri.DesGruppo3 = If(raggruppamenti.Count > 2, raggruppamenti.Skip(2).FirstOrDefault(), "")
        drParametri.LivelloRaggruppamento = raggruppamenti.Count()
        drParametri.AnnoConfronto = piuEsercizi
        drParametri.FormattaConDecimali = True
        drParametri.NumeroDecimali = 2
        drParametri.TipoValore = _tipoValore

        Dim numeroDecimaliPerQta As Integer = 0
        If _tipoValore = "0" Then
            Integer.TryParse(_decimali_qta, numeroDecimaliPerQta)
            drParametri.FormattaConDecimali = numeroDecimaliPerQta > 0
            drParametri.NumeroDecimali = numeroDecimaliPerQta
        End If
        If _tipoValore = "5" Then
            drParametri.NumeroDecimali = 1
        End If

        If raggruppamenti.Count() > 1 Then
            If Boolean.TryParse(_saltoPagina1Liv, boolTester) Then
                drParametri.SaltoPaginaPrimoLivello = boolTester
            Else
                drParametri.SaltoPaginaPrimoLivello = False
            End If
        Else
            drParametri.SaltoPaginaPrimoLivello = False
        End If

        If Boolean.TryParse(_scostamento, boolTester) Then
            drParametri.ScosamentoTotale = boolTester
        Else
            drParametri.ScosamentoTotale = False
        End If

        Dim descrizionePeriodo As String = ""
        Dim descrizioneAnno1 As String = ""
        Dim descrizioneAnno2 As String = ""

        'If piuEsercizi = False Then
        '    OttieniDescrizioniPeriodiPerUnEsercizio(tuttiMesi, descrizionePeriodo, descrizioneAnno1)
        'Else
        '    OttieniDescrizioniPeriodiPerPiuEsercizi(tuttiMesi, descrizionePeriodo, descrizioneAnno1, descrizioneAnno2)
        'End If
        drParametri.DescrizionePeriodo = DescrizionePeriodoGenerale()
        drParametri.Anno1 = DescrizionePeriodoPrincipale()
        drParametri.Anno2 = DescrizionePeriodoConfronto()


        Dim descrizioneLivelloDettaglio = OttieniDescrizioniCampiDettaglio(_livelli.Split("|").LastOrDefault().ToLower())
        drParametri.DescrizioneCampoCodiceDettaglio = descrizioneLivelloDettaglio.Item1
        drParametri.DescrizioneCampoDescriDettaglio = descrizioneLivelloDettaglio.Item2

        drParametri.MostraCampoCodiceDettaglio = MostraCampoCodiceDettaglio(_livelli.Split("|").LastOrDefault().ToLower())

        ' Ultimo livello è il prodotto
        drParametri.LivelloProdotto = (_livelli.Split("|").LastOrDefault().ToLower() = "prodotto")


        ' intestazione nomi mesi
        Dim drMesi As DS_Stat_12_Mesi.DT_MESIRow = dsStat.DT_MESI.NewDT_MESIRow
        Dim mesiReali = tuttiMesi.Take(12).Select(Function(am) am.Item1).ToList()

        For i As Integer = 1 To mesiReali.Count()
            drMesi("DesMese" + i.ToString()) = MonthName(mesiReali(i - 1), True).ToUpper()
        Next
        drMesi("MeseInizioPeriodo") = MonthName(CInt(_daMese)).ToUpper()
        drMesi("MeseFinePeriodo") = MonthName(CInt(_aMese)).ToUpper()

        dsStat.DT_MESI.Rows.Add(drMesi)

        ' Dati prodotti con udm zero
        If MostraSoloRiepilogoProdotti AndAlso listaDatiZero.Any Then

            Dim taken = 0
            Dim indiceCampo = 1
            Dim drDatiZero As DS_Stat_12_Mesi.DT_DATI_ZERORow = dsStat.DT_DATI_ZERO.NewDT_DATI_ZERORow()

            While taken <= listaDatiZero.Count()

                Dim sottoset = listaDatiZero.Skip(taken).Take(200)
                Dim sb = New StringBuilder
                For Each rs As RecordStatistica In sottoset
                    sb.AppendLine(String.Format("{0}   {1}", rs.CodDet.PadLeft(20, " "), rs.DesDet))
                    sb.Append(Environment.NewLine)
                Next
                drDatiZero("Lista" + indiceCampo.ToString()) = sb.ToString()
                taken = taken + 200
                indiceCampo = indiceCampo + 1

            End While

            dsStat.DT_DATI_ZERO.Rows.Add(drDatiZero)
        End If
        drParametri.NumRecordZero = listaDatiZero.Count()

        Return dsStat

    End Function

    Private Function FullJoinEsercizi(ByVal primario As List(Of RecordStatistica), ByVal secondario As List(Of RecordStatistica)) As List(Of RecordStatistica)


        Dim leftJoin = From pri In primario
                                        Group Join sec In secondario On pri.Key Equals sec.Key
                                        Into temp = Group
                                        From sec In temp.DefaultIfEmpty()
                                        Select New With {
                                                    .Primario = pri,
                                                    .Secondario = sec
                                                }

        Dim rightJoin = From sec In secondario
                        Group Join pri In primario On sec.Key Equals pri.Key
                        Into temp = Group
                        From pri In temp.DefaultIfEmpty()
                        Select New With {
                                .Primario = pri,
                                .secondario = sec
                            }

        Dim fullJoin = leftJoin.Union(rightJoin)

        Dim result = New List(Of RecordStatistica)
        For Each fj As Object In fullJoin

            Dim recPrimario = DirectCast(fj.Primario, RecordStatistica)
            Dim recSecondario = DirectCast(fj.Secondario, RecordStatistica)

            Dim rs = New RecordStatistica

            If Not recPrimario Is Nothing Then
                rs.Piva = recPrimario.Piva
                rs.Anno_Movimento = recPrimario.Anno_Movimento
                rs.Esercizio = recPrimario.Esercizio
                rs.UnitaMisura = recPrimario.UnitaMisura
                rs.CodLiv1 = recPrimario.CodLiv1
                rs.CodLiv2 = recPrimario.CodLiv2
                rs.CodLiv3 = recPrimario.CodLiv3
                rs.DesLiv1 = recPrimario.DesLiv1
                rs.DesLiv2 = recPrimario.DesLiv2
                rs.DesLiv3 = recPrimario.DesLiv3
                rs.CodDet = recPrimario.CodDet
                rs.DesDet = recPrimario.DesDet
                rs.TotalePerPeriodoAnnoRif = recPrimario.TotalePerPeriodoAnnoRif
                rs.Totale12MesiAnnoRif = recPrimario.Totale12MesiAnnoRif
            End If

            If Not recSecondario Is Nothing Then
                rs.Piva = recSecondario.Piva
                rs.Anno_Movimento = recSecondario.Anno_Movimento
                rs.Esercizio = recSecondario.Esercizio
                rs.UnitaMisura = recSecondario.UnitaMisura
                rs.CodLiv1 = recSecondario.CodLiv1
                rs.CodLiv2 = recSecondario.CodLiv2
                rs.CodLiv3 = recSecondario.CodLiv3
                rs.DesLiv1 = recSecondario.DesLiv1
                rs.DesLiv2 = recSecondario.DesLiv2
                rs.DesLiv3 = recSecondario.DesLiv3
                rs.CodDet = recSecondario.CodDet
                rs.DesDet = recSecondario.DesDet
                rs.TotalePerPeriodoAnnoPrec = recSecondario.TotalePerPeriodoAnnoRif
                rs.Totale12MesiAnnoPrec = recSecondario.Totale12MesiAnnoRif

            End If

            result.Add(rs)


        Next

        Return result

    End Function

    Public Overrides Function DataTableToList(table As System.Data.DataTable,
                                              riempi_secondo_valore As Boolean,
                                              Optional soloRefernza As Boolean = False) As System.Collections.Generic.List(Of RecordStatistica)

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
                rs.TotalePerPeriodoAnnoRif = CDec(row.Item("TotalePerPeriodo"))
                rs.Totale12MesiAnnoRif = CDec(row.Item("Totale12Mesi"))
            End If

            records.Add(rs)

        Next

        If Not soloRefernza Then
            For Each r In records
                r.Key = String.Format("{0}{1}{2}{3}{4}{5}", r.Piva, r.CodLiv1, r.CodLiv2, r.CodLiv3, r.CodDet, r.UnitaMisura)
                r.Totale = r.Val_01 + r.Val_02 + r.Val_03 + r.Val_04 + r.Val_05 + r.Val_06 + r.Val_07 + r.Val_08 + r.Val_09 + r.Val_10 + r.Val_11 + r.Val_12
            Next
        End If

        Return records

    End Function

    Public Overrides Sub FillDataTableFromList(lista As System.Collections.Generic.List(Of RecordStatistica), ByRef dataSet As DS_Stat_12_Mesi)

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
            row.CodDet = rs.CodDet
            row.DesDet = rs.DesDet
            row.Totale = rs.Totale
            row.TotalePerPeriodoAnnoRif = rs.TotalePerPeriodoAnnoRif
            row.Totale12MesiAnnoRif = rs.Totale12MesiAnnoRif
            row.TotalePerPeriodoAnnoPrec = rs.TotalePerPeriodoAnnoPrec
            row.Totale12MesiAnnoPrec = rs.Totale12MesiAnnoPrec
            dataSet.DT_Dati.Rows.Add(row)

        Next

    End Sub

    Private Sub OttieniDescrizioniPeriodiPerPiuEsercizi(ByVal mesiAnni As List(Of Tuple(Of Integer, Integer)),
                           ByRef descrizionePeriodo As String,
                           ByRef descrizioneAnno1 As String,
                           ByRef descrizioneAnno2 As String)

        Dim mesiAnniEsrcizio1 = (From m In mesiAnni.Take(12)
                                 Order By m.Item2 Ascending
                                 Group By anno = m.Item2
                                      Into mesi = Group, Count()
                                 Order By anno).ToList()

        Dim mesiAnniEsrcizio2 = (From m In mesiAnni.Skip(12).Take(12)
                                 Order By m.Item2 Ascending
                                 Group By anno = m.Item2
                                      Into mesi = Group, Count()
                                 Order By anno).ToList()

        If mesiAnniEsrcizio1.Count() = 1 Then
            descrizioneAnno2 = mesiAnniEsrcizio1.FirstOrDefault().anno.ToString()
        Else
            descrizioneAnno2 = String.Format("{0}/{1} - {2}/{3}",
                                                   mesiAnniEsrcizio1.FirstOrDefault().mesi.FirstOrDefault().Item1.ToString().PadLeft(2, "0"),
                                                   mesiAnniEsrcizio1.FirstOrDefault().anno.ToString(),
                                                   mesiAnniEsrcizio1.Skip(1).FirstOrDefault().mesi.LastOrDefault().Item1.ToString().PadLeft(2, "0"),
                                                   mesiAnniEsrcizio1.Skip(1).FirstOrDefault().anno.ToString())
        End If

        If mesiAnniEsrcizio2.Count() = 1 Then
            descrizioneAnno1 = mesiAnniEsrcizio2.FirstOrDefault().anno.ToString()
        Else

            descrizioneAnno1 = String.Format("{0}/{1} - {2}/{3}",
                                                   mesiAnniEsrcizio2.FirstOrDefault().mesi.FirstOrDefault().Item1.ToString().PadLeft(2, "0"),
                                                   mesiAnniEsrcizio2.FirstOrDefault().anno.ToString(),
                                                   mesiAnniEsrcizio2.Skip(1).FirstOrDefault().mesi.LastOrDefault().Item1.ToString().PadLeft(2, "0"),
                                                   mesiAnniEsrcizio2.Skip(1).FirstOrDefault().anno.ToString())
        End If

        descrizionePeriodo = String.Format("Periodi: {0} - {1}", descrizioneAnno2, descrizioneAnno1)

    End Sub

End Class
