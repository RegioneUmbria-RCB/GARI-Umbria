Imports System.Globalization
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq

Public Class Lista_DataOra_Intervallo_Date

    Public Chiave As String
    Public InfoAggiuntive As String
    Public Intervallo_Date As List(Of DataOra_Intervallo_Date)

    Public Sub New()
        Intervallo_Date = New List(Of DataOra_Intervallo_Date)
    End Sub
End Class

Public Class DataOra_Intervallo_Date
    Public Start_Date As Date
    Public End_Date As Date

End Class

Public Class DataOra

    Public Property DataBk As DataOra_Intervallo_Date

    Public Sub FinestraTemporaleImposta(objParametri As AgronicaCoreParametri, FinestraTemporaleInizio As Date, FinestraTemporaleFine As Date)
        DataBk = New DataOra_Intervallo_Date
        DataBk.Start_Date = objParametri.FinestraTemporaleInizio
        DataBk.End_Date = objParametri.FinestraTemporaleFine

        objParametri.FinestraTemporaleInizio = FinestraTemporaleInizio
        objParametri.FinestraTemporaleFine = FinestraTemporaleFine

    End Sub

    Public Sub FinestraTemporaleImposta(objParametri As AgronicaCoreParametri)

        objParametri.FinestraTemporaleInizio = DataBk.Start_Date
        objParametri.FinestraTemporaleFine = DataBk.End_Date
    End Sub

    Public Shared Function IntervalliDateUniti(ByVal ListaIntervalliDaUnire As List(Of DataOra_Intervallo_Date)) As List(Of DataOra_Intervallo_Date)

        'Dall'elenco di periodi in input si vuole ottenere un nuovo elenco in cui i periodi che si sovrappongono sono unificati fra loro in un macro-elemento.
        'Per fare ciò la lista iniziale viene ordinata per data inizio,
        'poi al primo ciclo le variabili new_range_item_start e new_range_item_end vengono inizializzate al primo periodo [if] (in quanto partono da Nothing),
        'che diventa quello di riferimento.
        'Dal successivo ciclo,
        '- se il periodo di riferimento finisce successivamente all'inizio di quello in esame [elseif] (quindi lo ingloba del tutto od in parte)
        '  il periodo di riferimento si estende alla data_fine maggiore fra la sua data e quella del periodo in esame.
        '- se il periodo di riferimento finisce prima dell'inizio di quello in esame [else] allora si aggiunge alla nuova lista il periodo di riferimento
        '  e questo viene aggiornato ai valori del periodo in esame.
        'all'ultima iterazione del ciclo il periodo di riferimento viene aggiunto come ultimo periodo della nuova lista.

        ListaIntervalliDaUnire.Sort(Function(x As DataOra_Intervallo_Date, y As DataOra_Intervallo_Date)
                                        If x.Start_Date < y.Start_Date Then
                                            Return -1
                                        ElseIf x.Start_Date > y.Start_Date Then
                                            Return 1
                                        End If
                                        Return 0
                                    End Function)


        Dim new_list_of_ranges As New List(Of DataOra_Intervallo_Date)

        Dim new_range_item_start As Date?
        Dim new_range_item_end As Date?

        'parte da uno per ottenere coicidenza con ultimo elemento
        Dim conteggioIterazioni As Integer = 0
        Dim length As Integer = ListaIntervalliDaUnire.Count

        For Each range_item In ListaIntervalliDaUnire

            If new_range_item_start Is Nothing Then
                new_range_item_start = range_item.Start_Date
                new_range_item_end = range_item.End_Date
            ElseIf new_range_item_end >= range_item.Start_Date Then
                new_range_item_end = IntervalliDateUniti_DataMaggiore(range_item.End_Date, new_range_item_end)
            Else
                new_list_of_ranges.Add(New DataOra_Intervallo_Date With {.Start_Date = new_range_item_start, .End_Date = new_range_item_end})
                new_range_item_start = range_item.Start_Date
                new_range_item_end = range_item.End_Date
            End If

            conteggioIterazioni += 1

            If conteggioIterazioni = length Then
                new_list_of_ranges.Add(New DataOra_Intervallo_Date With {.Start_Date = new_range_item_start, .End_Date = new_range_item_end})
            End If

        Next

        Return new_list_of_ranges

    End Function

    Private Shared Function IntervalliDateUniti_DataMaggiore(Data1 As Date, Data2 As Date) As Date

        If Data1 > Data2 Then
            Return Data1
        Else
            Return Data2
        End If

    End Function

    Public Shared Sub ConvertiIntervalloGiulianoInDate(AnnoRiferimento As Integer, gg_da As Integer, gg_a As Integer, ByRef DataDa As Date, ByRef dataa As Date)

        Dim AnnoRiferimentoPerCalcolo As Integer = Now.Date.Year()
        If AnnoRiferimento <> -1 Then
            AnnoRiferimentoPerCalcolo = AnnoRiferimento
        End If

        If gg_da < gg_a Then
            'intervallo compreso in un anno solare
            Dim D0101 As Date = "01/01/" & AnnoRiferimentoPerCalcolo

            DataDa = D0101.AddDays(gg_da)
            dataa = D0101.AddDays(gg_a)

        Else
            'intervallo a metà fra 2 anni solari
            Dim D0101 As Date
            Dim D0101Piu1 As Date

            If Now.Date.DayOfYear > gg_da Then
                D0101 = "01/01/" & AnnoRiferimentoPerCalcolo
                D0101Piu1 = "01/01/" & AnnoRiferimentoPerCalcolo + 1
            Else
                D0101 = "01/01/" & AnnoRiferimentoPerCalcolo - 1
                D0101Piu1 = "01/01/" & AnnoRiferimentoPerCalcolo
            End If

            DataDa = D0101.AddDays(gg_da)
            dataa = D0101Piu1.AddDays(gg_a)

        End If

    End Sub

    Public Shared Function RecuperaIntervalloMancanteSuDateInDataTable(EscludiRigheSeDatoPresente As Boolean, DataInizio As Date, DataFine As Date, dtDatiPerRigheMancanti As DataTable, ByVal NomeColonnaDataOra As String) As List(Of DateTime)

        Dim listaRighePresenti As List(Of DateTime) = (
                        From dd In dtDatiPerRigheMancanti.AsEnumerable
                        Select CType(dd(NomeColonnaDataOra), DateTime)
                    ).Distinct.ToList

        Dim listaFinale As New List(Of DateTime)
        If EscludiRigheSeDatoPresente Then

            Dim span As TimeSpan = DataFine.Subtract(DataInizio)

            Dim ddOraCorrente As DateTime = DataInizio


            Dim oreComplessiveDifferenza As Double = span.TotalHours

            For oraDaAggiungere = 1 To oreComplessiveDifferenza

                If listaRighePresenti.Count > 0 Then

                    Dim lFirst As DateTime = listaRighePresenti.First
                    If lFirst = ddOraCorrente Then
                        listaRighePresenti.RemoveAt(0)
                    Else
                        listaFinale.Add(ddOraCorrente)
                    End If

                Else
                    listaFinale.Add(ddOraCorrente)

                End If

                ddOraCorrente = DataInizio.AddHours(oraDaAggiungere)

            Next
        Else
            For Each d1 In listaRighePresenti
                listaFinale.Add(d1)
            Next
        End If

        Return listaFinale

    End Function

    Public Shared Function SeparatoreDataCorrente() As String

        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.DateSeparator

    End Function

    Public Shared Function SeparatoreOraCorrente() As String
        Return System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.TimeSeparator

    End Function


    Public Shared Sub JarrayAggiustaDate(ByRef j As JArray)

        Dim culture As Globalization.CultureInfo = Globalization.CultureInfo.InvariantCulture
        Dim style As Globalization.DateTimeStyles = Globalization.DateTimeStyles.None

        Dim dSep As String = SeparatoreDataCorrente()
        Dim tSep As String = SeparatoreOraCorrente()

        Dim Format1 As String = "dd" & dSep & "MM" & dSep & "yyyy HH" & tSep & "mm" & tSep & "ss" '"yyyyMMddHHmmss" '31/12/2016 22:00:00
        '
        For Each curProp In j.Descendants().OfType(Of JProperty)().Where(Function(p) (p.Value.Type = JTokenType.Date))
            Dim value = curProp.Value.ToString
            Dim myDate As DateTime
            If DateTime.TryParseExact(value, Format1, culture, style, myDate) Then
                curProp.Value = DataOraToDate_JSON_ISO8601(myDate)
            End If
        Next

    End Sub

    ''' <summary>
    ''' Converte un timestamp unix nella data specificata. se si passa il datetimekind UTC viene restituita una data UTC, altrimenti viene convertita in data locale
    ''' </summary>
    ''' <param name="uX"></param>
    ''' <param name="DateTimeKindDellaData"></param>
    ''' <returns></returns>
    Public Shared Function UnixTimeStampToDateTime(ByVal uX As Integer, Optional DateTimeKindDellaData As DateTimeKind = DateTimeKind.Utc) As DateTime

        Dim rval As DateTime
        rval = DateAdd(DateInterval.Second, uX, #1/1/1970#)

        If DateTimeKindDellaData = DateTimeKind.Local Then
            Dim convertedDate As DateTime = DateTime.SpecifyKind(rval, DateTimeKind.Utc)
            rval = convertedDate.ToLocalTime()
        End If

        Return rval

    End Function

    Public Shared Function DataStringaJSON_to_ISO8601(ByVal d As String, Optional DateTimeKindDellaData As DateTimeKind = DateTimeKind.Unspecified) As DateTime
        Select Case DateTimeKindDellaData
            Case DateTimeKind.Unspecified
                Return DateTime.Parse(d)
            Case DateTimeKind.Utc
                Return DateTime.Parse(d).ToUniversalTime
            Case Else
                Return DateTime.Parse(d)
        End Select

    End Function


    ''' <summary>
    ''' converte un datetime fra una Timezone ed un'altra e restituisce un datetime eventualmente convertito nella timezone richiesta nel secondo parametro.
    ''' </summary>
    ''' <param name="dataDaConvertire"></param>
    ''' <param name="system_time_zone_origine">Timezone della data origine</param>
    ''' <param name="system_time_zone_destinazione">Timezone della data di destinazione</param>
    ''' <returns></returns>
    Public Shared Function ConvertiTimeZone(
        ByVal dataDaConvertire As DateTime,
        system_time_zone_origine As String,
        system_time_zone_destinazione As String
    ) As DateTime


        If system_time_zone_origine.ToUpper = "UTC" Then
            dataDaConvertire = DateTime.SpecifyKind(dataDaConvertire, DateTimeKind.Utc)
        Else
            dataDaConvertire = DateTime.SpecifyKind(dataDaConvertire, DateTimeKind.Local)
        End If

        Return TimeZoneInfo.ConvertTime(dataDaConvertire, TimeZoneInfo.FindSystemTimeZoneById(system_time_zone_origine), TimeZoneInfo.FindSystemTimeZoneById(system_time_zone_destinazione))

    End Function

    ''' <summary>
    ''' Legge la data in formato ISO 8601 e restituisce un datetime eventualmente convertito nel datetimekind richiesto nel secondo parametro.
    ''' </summary>
    ''' <param name="StringaIso8601DaConvertire">Stringa che rappresenta la data in formato ISO 8601</param>
    ''' <param name="DateTimeKindDellaDataLetta">Date time Kind della stringa in formato ISO 8601</param>
    ''' <param name="DateTimeKindRichiestoPerLaDataConvertita">Date time kind richiesto per il valore restituito dalla funzione</param>
    ''' <param name="system_time_zone">Timezone della data (Se si richiede una conversione da Local ad UTC indica il TimeZone della data passata, altrimenti se si richiede una conversione da UTC a local indica il TimeZone in cui convertire la data restituita)</param>
    ''' <returns></returns>
    Public Shared Function LeggiDataDaStringa_ISO8601(
        ByVal StringaIso8601DaConvertire As String,
        DateTimeKindDellaDataLetta As DateTimeKind,
        DateTimeKindRichiestoPerLaDataConvertita As DateTimeKind,
        system_time_zone As String
    ) As DateTime

        'Rimuovo un eventuale "Z" finale, così da non influenzare le conversioni successive.
        StringaIso8601DaConvertire = StringaIso8601DaConvertire.TrimEnd("Z").TrimEnd("z")

        Select Case DateTimeKindRichiestoPerLaDataConvertita

            Case DateTimeKind.Unspecified
                Throw New Exception("La funzione LeggiDataDaStringa_ISO8601 non può essere chiamata con valore DateTimeKind Unspecified")

            Case DateTimeKind.Utc
                If DateTimeKindDellaDataLetta = DateTimeKind.Local Then
                    Dim d As DateTime = DateTime.Parse(StringaIso8601DaConvertire)
                    d = DateTime.SpecifyKind(d, DateTimeKind.Local)
                    Return TimeZoneInfo.ConvertTime(d, TimeZoneInfo.FindSystemTimeZoneById(system_time_zone), TimeZoneInfo.Utc)
                End If

                Return DateTime.Parse(StringaIso8601DaConvertire)

            Case DateTimeKind.Local

                If DateTimeKindDellaDataLetta = DateTimeKind.Utc Then
                    Dim d As DateTime = DateTime.Parse(StringaIso8601DaConvertire)
                    d = DateTime.SpecifyKind(d, DateTimeKind.Utc)
                    Return TimeZoneInfo.ConvertTime(d, TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById(system_time_zone))
                End If

                Return DateTime.Parse(StringaIso8601DaConvertire)

        End Select

    End Function


    ''' <summary>
    ''' Converte un data/ora in formato ISO8601
    ''' </summary>
    ''' <param name="d">la data da convertire</param>
    ''' <param name="DateTimeKindDellaData">indica come considerare il valore passato come parametro, se locale il valore risultato sarà attualizzato su UTC (es. per italia: le 21:00 del 13 ottobre sono le 19 in UTC</param>
    ''' <returns></returns>
    Public Shared Function DataOraToDate_JSON_ISO8601(ByVal d As DateTime, Optional DateTimeKindDellaData As DateTimeKind = DateTimeKind.Unspecified) As String

        Dim rval As String = ""

        Select Case DateTimeKindDellaData
            Case DateTimeKind.Unspecified, DateTimeKind.Utc

                rval = DataOraToDate(d)
                rval &= "Z"

            Case DateTimeKind.Local

                Dim d1 As DateTime = New DateTime(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second, d.Millisecond, DateTimeKindDellaData)
                Dim utcNow As DateTime = d1.ToUniversalTime()

                rval = utcNow.ToString("O")

        End Select


        Return rval

    End Function


    Public Shared Function DataToYYYYMMDD(ByVal d As DateTime) As String

        Dim rval As String =
            d.Year.ToString &
            d.Month.ToString.PadLeft(2, "00") &
            d.Day.ToString.PadLeft(2, "00")

        Return rval

    End Function


    Public Shared Function LeggiDataDaStringaYYYYMMDD(ByVal d As String) As Date

        Dim rval As String =
            d.Substring(0, 4) & "-" &
            d.Substring(4, 2) & "-" &
            d.Substring(6, 2) & "T00:00:00"


        Return DataStringaJSON_to_ISO8601(rval)

    End Function

    Public Shared Function DataOraToDate_SQL_ISO(ByVal d As DateTime) As String

        Dim rval As String = DataOraToDate(d)
        rval &= "." & d.Millisecond

        Return rval

    End Function

    Private Shared Function DataOraToDate(ByVal d As DateTime) As String

        Dim rval As String = ""
        rval =
            d.Year.ToString & "-" &
            d.Month.ToString.PadLeft(2, "00") & "-" &
            d.Day.ToString.PadLeft(2, "00") & "T" &
            d.Hour.ToString.PadLeft(2, "00") & ":" &
            d.Minute.ToString.PadLeft(2, "00") & ":" &
            d.Second.ToString.PadLeft(2, "00")

        Return rval

    End Function


    Public Shared Sub IncrementaGiorno(ByRef xg As Integer, ByRef yr As Integer)

        yr = yr + 1
        If yr = 24 Then
            yr = 0
            xg = xg + 1
        End If

    End Sub



    Public Shared Function GiulianoToDate(ByVal MioGiuliano As Integer, ByVal Anno As Long) As Date
        Dim InizioAnno As Date

        InizioAnno = DateSerial(Anno, 1, 1)

        Return DateAdd("d", MioGiuliano, InizioAnno)
    End Function




    Public Shared Function DateToGiuliano(MyDate As Date) As Integer
        Dim InizioAnno As Date

        InizioAnno = DateSerial(Year(MyDate), 1, 1)
        Return DateDiff("d", InizioAnno, MyDate)

    End Function

    Public Shared Function GetFirstDayOfWeek(Year As Integer, WeekNumber As Integer, rule As CalendarWeekRule) As DateTime

        Dim jan1 As DateTime = New DateTime(Year, 1, 1)
        Dim daysOffset As Integer = DayOfWeek.Monday - jan1.DayOfWeek
        Dim firstMonday As DateTime = jan1.AddDays(daysOffset)
        Dim firstWeek As Integer = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(firstMonday, rule, DayOfWeek.Monday)

        If firstWeek <= 1 Then
            WeekNumber -= 1
        End If

        Return firstMonday.AddDays(WeekNumber * 7)

    End Function

    Public Shared Function GetWeekNumberFromDate(ByVal data As DateTime, rule As CalendarWeekRule) As Integer
        Return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(data, rule, DayOfWeek.Monday)
    End Function
End Class
