Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider

Public Class Statistica_Confronto_MeseAnniController : Inherits StatisticaControllerBase

    Private _nomeFileReport As String = "Stat12Mesi.rpt"
    Private _nomeFileReportOrdinatoValore As String = "Stat12MesiOrdVal.rpt"

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
        Dim listaDati = DataTableToList(dati, False)

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
        drParametri.TipoValore = _tipoValore
        drParametri.NumeroDecimali = 2

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
        For i As Integer = 1 To 12
            drMesi("Mese" + i.ToString() + "Visible") = Mese_Visibile(i)
        Next
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

                rs.Val_01 = recPrimario.Val_01
                rs.Val_02 = recPrimario.Val_02
                rs.Val_03 = recPrimario.Val_03
                rs.Val_04 = recPrimario.Val_04
                rs.Val_05 = recPrimario.Val_05
                rs.Val_06 = recPrimario.Val_06
                rs.Val_07 = recPrimario.Val_07
                rs.Val_08 = recPrimario.Val_08
                rs.Val_09 = recPrimario.Val_09
                rs.Val_10 = recPrimario.Val_10
                rs.Val_11 = recPrimario.Val_11
                rs.Val_12 = recPrimario.Val_12

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

                rs.ValC_01 = recSecondario.Val_01
                rs.ValC_02 = recSecondario.Val_02
                rs.ValC_03 = recSecondario.Val_03
                rs.ValC_04 = recSecondario.Val_04
                rs.ValC_05 = recSecondario.Val_05
                rs.ValC_06 = recSecondario.Val_06
                rs.ValC_07 = recSecondario.Val_07
                rs.ValC_08 = recSecondario.Val_08
                rs.ValC_09 = recSecondario.Val_09
                rs.ValC_10 = recSecondario.Val_10
                rs.ValC_11 = recSecondario.Val_11
                rs.ValC_12 = recSecondario.Val_12

            End If

            result.Add(rs)


        Next

        Return result

    End Function

End Class
