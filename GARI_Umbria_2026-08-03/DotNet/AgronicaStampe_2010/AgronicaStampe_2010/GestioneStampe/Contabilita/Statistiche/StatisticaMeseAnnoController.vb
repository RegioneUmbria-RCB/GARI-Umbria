Imports AgronicaCoreDataProvider
Imports AgronicaCoreContabDAL

Public Class StatisticaMeseAnnoController : Inherits StatisticaControllerBase

    Private _nomeFileReport As String = "Stat12Mesi_2Valori.rpt"
    Private _nomeFileReportOrdinatoValore As String = "Stat12Mesi_2ValoriOrdVal.rpt"

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

        Setta_Periodo_Temporale(_dataDal, _dataAl)

        ' Ottengo periodo di esercizio
        Dim tuttiMesi = statR.MonthsBetween(Convert.ToDateTime(_dataDal), Convert.ToDateTime(_dataAl)).ToList()
        Dim piuEsercizi As Boolean = tuttiMesi.Count() > 12

        Dim MostraSoloRiepilogoProdotti As Boolean = False


        Dim listaDatiZero = New List(Of RecordStatistica)
        'Dim datiPrecheck As DataTable
        '' Se selezionato tipo valore 4 (kg/pz) o 5 (nr) verifica se esiste anche solo una riga con totale vuoto
        '' in questo caso il report mostrera solo una sezione di riepilogo dei prodotti a 0
        'If _tipoValore = "4" OrElse _tipoValore = "5" Then
        '    datiPrecheck = statR.Precheck_Righe_Report_Vendite(
        '    _piva,
        '    _report,
        '    False,
        '     _numSin, _num, _numDes,
        '     _nrRiga,
        '     _dataDal, _dataAl,
        '     _clienti,
        '    _agenti,
        '    _causali,
        '    _specie,
        '    _varieta,
        '    _prodotti,
        '    _categorie,
        '    _categorieCommerciali,
        '    _cauTrasp,
        '    "",
        '    _tipoValore,
        '    _livelli,
        '    _rapportiContabili,
        '    _nazioniFatturazione,
        '    _tipoValore2,
        '    _objParametriServer)

        '    listaDatiZero = DataTableToList(datiPrecheck, True)

        '    If datiPrecheck.Rows.Count > 0 Then
        '        MostraSoloRiepilogoProdotti = True
        '    End If

        'End If

        ' Ottengo dataSource finale del report
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
        Dim listaDati = DataTableToList(dati, True)

        FillDataTableFromList(listaDati, dsStat)

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

        desUdm = OttieniDescrizioneUDM(_tipoValore2)
        drParametri.UnitaMisuraAbbreviata_2 = desUdm.Item1
        drParametri.UnitaMisuraEstesa_2 = desUdm.Item2

        drParametri.DesGruppo1 = OttieniDescrizioneLivello(raggruppamenti.FirstOrDefault())
        drParametri.DesGruppo2 = If(raggruppamenti.Count > 1, raggruppamenti.Skip(1).FirstOrDefault(), "")
        drParametri.DesGruppo3 = If(raggruppamenti.Count > 2, raggruppamenti.Skip(2).FirstOrDefault(), "")
        drParametri.LivelloRaggruppamento = raggruppamenti.Count()
        drParametri.AnnoConfronto = True
        drParametri.FormattaConDecimali = True
        drParametri.FormattaConDecimali_2 = True

        Dim numeroDecimaliPerQta As Integer = 0

        If _tipoValore = "0" Then
            Integer.TryParse(_decimali_qta, numeroDecimaliPerQta)
            drParametri.FormattaConDecimali = numeroDecimaliPerQta > 0
        End If
        If _tipoValore2 = "0" Then
            drParametri.FormattaConDecimali_2 = False
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

        'OttieniDescrizioniPeriodiPerUnEsercizio(tuttiMesi, descrizionePeriodo, descrizioneAnno1)
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

End Class
