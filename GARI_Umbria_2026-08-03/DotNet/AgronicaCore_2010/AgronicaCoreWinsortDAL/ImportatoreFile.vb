Imports System.IO
Imports System.IO.File
Imports System.Data.OleDb
Imports System.Globalization
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public MustInherit Class AbsImportatoreFile

    Public Class Calibro

        Private _qualita As String
        Private _nome As String
        Private _peso As Decimal
        Private _perc As Nullable(Of Decimal)
        Private _num As Nullable(Of Integer)

        Public ReadOnly Property qualita As String
            Get
                Return _qualita
            End Get
        End Property
        Public ReadOnly Property nome As String
            Get
                Return _nome
            End Get
        End Property
        Public Property peso As Decimal
            Get
                Return _peso
            End Get
            Set(value As Decimal)
                _peso = value
            End Set
        End Property
        Public Property perc As Nullable(Of Decimal)
            Get
                Return _perc
            End Get
            Set(value As Nullable(Of Decimal))
                _perc = value
            End Set
        End Property
        Public ReadOnly Property num As Nullable(Of Integer)
            Get
                Return _num
            End Get
        End Property

        Public Sub New(ByVal qualita As String, ByVal nome As String, ByVal peso As Decimal, ByVal perc As Nullable(Of Decimal), ByVal num As Nullable(Of Integer))
            _qualita = qualita
            _nome = nome
            _peso = peso
            _perc = perc
            _num = num
        End Sub

    End Class

    Protected _file As StreamReader

    Protected _dataInizio As Nullable(Of DateTime)
    Protected _dataFine As Nullable(Of DateTime)
    Protected _programma As String
    Protected _nrBolla As String
    Protected _conferitoreCodice As String
    Protected _conferitoreNome As String
    Protected _lotto As String
    Protected _varieta As String
    Protected _rifBolla As String
    Protected _note As String
    Protected _pesoTot As Nullable(Of Decimal)
    Protected _scarti As Nullable(Of Decimal)
    Protected _durata As Nullable(Of Long)
    Protected _numeroTot As Nullable(Of Integer)

    Protected _calibri As List(Of Calibro)

    Public ReadOnly Property conferitoreCodice As String
        Get
            Return _conferitoreCodice
        End Get
    End Property
    Public ReadOnly Property conferitoreNome As String
        Get
            Return _conferitoreNome
        End Get
    End Property
    Public ReadOnly Property dataInizio() As Nullable(Of DateTime)
        Get
            Return _dataInizio
        End Get
    End Property
    Public ReadOnly Property dataFine() As Nullable(Of DateTime)
        Get
            Return _dataFine
        End Get
    End Property
    Public ReadOnly Property lotto() As String
        Get
            Return _lotto
        End Get
    End Property
    Public ReadOnly Property varieta() As String
        Get
            Return _varieta
        End Get
    End Property
    Public ReadOnly Property programma() As String
        Get
            Return _programma
        End Get
    End Property
    Public ReadOnly Property nrBolla() As String
        Get
            Return _nrBolla
        End Get
    End Property
    Public ReadOnly Property rifBolla() As String
        Get
            Return _rifBolla
        End Get
    End Property
    Public ReadOnly Property pesoTot As Nullable(Of Decimal)
        Get
            Return _pesoTot
        End Get
    End Property
    Public ReadOnly Property numeroTot As Nullable(Of Integer)
        Get
            Return _numeroTot
        End Get
    End Property
    Public ReadOnly Property durata As Nullable(Of Long)
        Get
            Return _durata
        End Get
    End Property
    Public ReadOnly Property scarti As Nullable(Of Decimal)
        Get
            Return _scarti
        End Get
    End Property
    Public ReadOnly Property note() As String
        Get
            Return _note
        End Get
    End Property
    Public ReadOnly Property Calibri() As List(Of Calibro)
        Get
            Return _calibri
        End Get
    End Property

    Protected Function toDecimal(ByVal str As String, ByVal def As Decimal) As Decimal
        Dim ret As Decimal
        Dim sep As String = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator
        str = str.Replace(".", sep)
        str = str.Replace(",", sep)
        If Not Decimal.TryParse(str, ret) Then
            ret = def
        End If
        Return ret
    End Function

    Protected Sub aggiustaPercentuali(ByVal calcPerc As Boolean, ByVal maxDiffPerc As Decimal)

        If (_calibri.Count = 0) Then
            Return
        End If

        'Lavoro con le percentuali al secondo decimale e con i pesi al terzo decimale...

        Dim __pesoTot As Decimal = If(_pesoTot.HasValue, _pesoTot.Value, 0D)
        Dim __scarti As Decimal = If(_scarti.HasValue, _scarti.Value, 0D)

        Dim totalePeso As Decimal = __pesoTot + __scarti
        If (totalePeso = 0) Then
            Return
        End If
        Dim totalePerc As Integer = 0
        Dim tmp_perc As Decimal

        For Each cal As Calibro In _calibri
            If (calcPerc) Then
                tmp_perc = Math.Round(cal.peso * 100D / totalePeso, 2)
                cal.perc = tmp_perc
            Else
                tmp_perc = cal.perc
            End If
            totalePerc += Math.Truncate(tmp_perc * 100D)
        Next

        totalePerc += Math.Truncate(Math.Round((__scarti * 100D) / totalePeso, 2) * 100D)

        Dim diffPerc As Integer
        diffPerc = totalePerc - 10000
        If (Math.Abs(diffPerc) = 0) Then 'Differenza nulla, niente da correggere
            Return
        End If
        If (Math.Abs(diffPerc) > (maxDiffPerc * 100D)) Then 'Differenza troppo grande... caso da gestire in altra sede
            Return
        End If

        'Cerco il cal con il peso massimo...
        Dim idxMax As Integer = 0
        Dim maxPeso As Decimal = _calibri.ElementAt(0).peso

        For idx As Integer = 0 To _calibri.Count - 1
            If (_calibri.ElementAt(idx).peso > maxPeso) Then
                idxMax = idx
                maxPeso = _calibri.ElementAt(idx).peso
            End If
        Next

        tmp_perc = _calibri.ElementAt(idxMax).perc

        Dim newCalPerc100 As Integer = Math.Truncate((tmp_perc * 100D) - diffPerc)
        Dim newCalPeso1000 As Integer = Math.Truncate(totalePeso * 1000D) * newCalPerc100 / 10000
        Dim diffPeso1000 As Integer = Math.Truncate(_calibri.ElementAt(idxMax).peso * 1000D) - newCalPeso1000

        _pesoTot = Math.Round((((__pesoTot * 1000D) - diffPeso1000) / 1000D), 3)

        Dim newCalPerc As Decimal = newCalPerc100
        newCalPerc /= 100D
        _calibri.ElementAt(idxMax).perc = Math.Round(newCalPerc, 2)

        Dim newCalPeso As Decimal = newCalPeso1000
        newCalPeso /= 1000D
        _calibri.ElementAt(idxMax).peso = Math.Round(newCalPeso, 3)

    End Sub

    Public Sub New(ByVal filename As String)

        _file = New StreamReader(filename)

        '_dataInizio = Date.MinValue
        '_dataFine = Date.MinValue
        '_programma = String.Empty
        '_nrBolla = String.Empty
        '_conferitoreCodice = String.Empty
        '_conferitoreNome = String.Empty
        '_lotto = String.Empty
        '_varieta = String.Empty
        '_rifBolla = String.Empty
        '_note = String.Empty
        '_pesoTot = 0D
        '_scarti = 0D
        '_durata = 0
        '_numeroTot = 0

        _calibri = New List(Of Calibro)

    End Sub

    Public MustOverride Function importa() As Boolean

End Class



Public Class ImportatoreCalibratrice : Inherits AbsImportatoreFile

    Private Function LineToDT(line As String) As DateTime

        Dim sDTs As String() = line.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)

        Dim Ds = Trim(sDTs(0))
        Dim Ts = Trim(sDTs(1))

        Dim sDs = Ds.Split("/")
        Dim sTs = Ts.Split(".")

        Dim day As Integer = sDs(0)
        Dim month As Integer = sDs(1)
        Dim year As Integer = sDs(2) + 2000
        Dim hours As Integer = sTs(0)
        Dim minutes As Integer = sTs(1)
        Dim seconds As Integer = sTs(2)

        Return New DateTime(year, month, day, hours, minutes, seconds)

    End Function

    Private Sub raggruppaCalibri()

        Dim newList As New List(Of Calibro)
        Dim nome As String
        Dim num As Integer
        Dim peso As Decimal

        Dim idx As Integer
        While _calibri.Count > 0

            Dim cal0 As Calibro = _calibri.ElementAt(0)
            nome = cal0.nome
            num = cal0.num
            peso = cal0.peso

            _calibri.RemoveAt(0)

            idx = 0

            While idx < _calibri.Count

                Dim cal1 As Calibro = _calibri.ElementAt(idx)
                If cal1.nome.Equals(nome) Then

                    num += cal1.num
                    peso += cal1.peso

                    _calibri.RemoveAt(idx)
                Else
                    idx += 1
                End If

            End While

            newList.Add(New Calibro("", nome, peso, Nothing, num))

        End While

        _calibri.AddRange(newList)

    End Sub

    Public Sub New(ByVal filename As String)
        MyBase.New(filename)
    End Sub

    Public Overrides Function importa() As Boolean

        Dim cntRow = 0
        Dim strNome As String = "StVuota"
        Dim strNumero As String = ""
        Dim strPeso As String = ""

        While Not _file.EndOfStream And cntRow < 300

            'leggo 100 gruppi di 3 -> ogni gruppo è un Calibro
            Select Case cntRow Mod 3
                Case 0
                    If (Not strNome = "StVuota") Then
                        'aggiungo un calibro
                        _calibri.Add(New Calibro("", strNome, toDecimal(strPeso, 0D), Nothing, CInt(strNumero)))
                    End If
                    strNome = Trim(_file.ReadLine())
                Case 1
                    strNumero = Trim(_file.ReadLine())
                Case 2
                    strPeso = Trim(_file.ReadLine())
            End Select

            cntRow += 1

        End While

        'Dopo 300 righe ho le informazioni generali (7 righe)
        cntRow = 0
        Dim line As String

        While Not _file.EndOfStream And cntRow < 7

            line = Trim(_file.ReadLine)

            Select Case cntRow
                Case 0
                    _dataInizio = LineToDT(line)
                Case 1
                    _conferitoreCodice = line
                Case 2
                    _conferitoreNome = line
                Case 3
                    _lotto = line
                Case 4
                    _varieta = line
                Case 5
                    'NULLA -> Qualità ingresso???
                Case 6
                    _dataFine = LineToDT(line)
            End Select

            cntRow += 1

        End While

        If (String.IsNullOrEmpty(_lotto)) Then

            _file.Close()
            Return False

        End If

        cntRow = 0
        Dim scarti_in_gr As Integer = 0

        While Not _file.EndOfStream And cntRow < 6

            line = Trim(_file.ReadLine)

            'Scarto manuale???
            'Scarto manuale???
            'Peso accumulato scarti???
            'Peso accumulato scarti???
            'Peso accumulato scarti???
            'Peso accumulato scarti???

            scarti_in_gr += Double.Parse(line, CultureInfo.InvariantCulture)

            cntRow += 1

        End While

        _scarti = CDec(scarti_in_gr) / 1000D

        '100 righe x 23 colonne ciascuna
        cntRow = 0
        Dim cntCol As Integer
        While Not _file.EndOfStream And cntRow < 100

            cntCol = 0

            While Not _file.EndOfStream And cntCol < 23

                line = Trim(_file.ReadLine)

                cntCol += 1

            End While

            cntRow += 1
        End While

        line = ""
        While Not _file.EndOfStream

            line = Trim(_file.ReadLine)

        End While

        _durata = CLng(line)

        'Dim totalLine = 0
        'Dim readingCal = True
        'Dim readingGeneralInformation = False
        'Dim readingScarti = False
        'Dim calIterator = 0
        'Dim generalInfoIndex = 0
        'Dim resto As Integer


        '    If line = "StVuota" Then
        '        readingCal = False
        '    End If
        '    If readingCal Then
        '        resto = calIterator Mod 3
        '        Select Case resto
        '            Case 0
        '                calibriNomi.Add(line)
        '                calIterator += 1
        '            Case 1
        '                calibriNumeri.Add(line)
        '                calIterator += 1
        '            Case 2
        '                calibriPesi.Add(line)
        '                calIterator = 0
        '        End Select
        '    End If
        '    If totalLine = 300 Then
        '        readingGeneralInformation = True
        '        readingCal = False
        '    End If
        '    If readingGeneralInformation Then
        '        Select Case generalInfoIndex
        '            Case 0
        '                dataInizio = line
        '                generalInfoIndex += 1
        '            Case 1
        '                conferitore_codice = line
        '                generalInfoIndex += 1
        '            Case 2
        '                conferitore_nome = line
        '                generalInfoIndex += 1
        '            Case 3
        '                lotto = line
        '                generalInfoIndex += 1
        '            Case 4
        '                varieta = line
        '                generalInfoIndex += 1
        '            Case 5

        '                generalInfoIndex += 1
        '            Case 6
        '                dataFine = line
        '                readingGeneralInformation = False
        '        End Select
        '    End If
        '    If totalLine = 307 Then
        '        readingGeneralInformation = False
        '        readingScarti = True
        '    End If
        '    If readingScarti Then
        '        If line <> "1" Then
        '            scarti.Add(line)
        '        Else
        '            readingScarti = False
        '        End If
        '    End If
        '    totalLine += 1
        'End While
        'Dim durata = line

        _file.Close()

        raggruppaCalibri()

        _pesoTot = 0
        _numeroTot = 0

        For Each c As Calibro In _calibri
            _pesoTot += c.peso
            _numeroTot += c.num
        Next

        aggiustaPercentuali(True, 100)

        Return True

    End Function

End Class



Public Class ImportatoreCampionatrice : Inherits AbsImportatoreFile

    Private Function LineToDT(ByVal line As String) As DateTime

        Dim sDTs As String() = line.Split(New Char() {" "c}, StringSplitOptions.RemoveEmptyEntries)

        Dim Ds = Trim(sDTs(0))
        Dim Ts = Trim(sDTs(1))

        Dim sDs = Ds.Split("-")
        Dim sTs = Ts.Split(":")

        Dim day As Integer = sDs(0)
        Dim month As Integer = sDs(1)
        Dim year As Integer = sDs(2)
        Dim hours As Integer = sTs(0)
        Dim minutes As Integer = sTs(1)

        Return New DateTime(year, month, day, hours, minutes, 0)

    End Function

    Public Sub New(ByVal filename As String)
        MyBase.New(filename)
    End Sub

    Public Overrides Function importa() As Boolean

        Dim lines = New List(Of String)

        While Not _file.EndOfStream
            lines.Add(Trim(_file.ReadLine()))
        End While
        _file.Close() 'Per sicurezza è meglio chiudere StreamReader altrimenti non riuscirò a spostare il file nella cartella degli importati

        _dataInizio = LineToDT(lines.Item(0))
        _dataFine = LineToDT(lines.Item(1))
        _programma = lines.Item(2)
        _nrBolla = lines.Item(3)
        _conferitoreCodice = lines.Item(4)
        _conferitoreNome = String.Empty
        _lotto = lines.Item(5)
        _varieta = lines.Item(6)
        _rifBolla = lines.Item(7)
        _note = lines.Item(8)  'Data conferimento (sempre?)
        _pesoTot = toDecimal(lines.Item(28), 0)

        '22 righe ogni QUALITA (3)
        '
        '1 QUALITA ...
        '2 calibro
        '3 calibro
        '
        '
        '
        '20 calibro
        '21 calibro
        '22 TOTALE

        Dim offsetRiga As Integer = 40
        Dim iRiga As Integer
        Dim q As Integer = 0
        Dim str_q As String
        Dim line As String

        While q < 3

            str_q = lines.Item(offsetRiga + (q * 22))

            iRiga = 1
            While iRiga < 21

                line = lines.Item(offsetRiga + (q * 22) + iRiga)

                Dim values As String() = line.Split(";")

                If Not String.IsNullOrEmpty(values(0)) Then

                    ' 5 valori ogni linea...
                    Dim descr As String = Trim(values(0))
                    Dim peso_gr As Integer
                    If Not Integer.TryParse(Trim(values(1)), peso_gr) Then
                        peso_gr = 0
                    End If
                    Dim tot_kg = toDecimal(Trim(values(2)), 0)
                    Dim perc = toDecimal(Trim(values(3)), 0)
                    Dim conferito = toDecimal(Trim(values(4)), 0)

                    _calibri.Add(New Calibro(str_q, descr, tot_kg, perc, Nothing))

                End If

                iRiga += 1
            End While

            q += 1
        End While

        'Esistono dei files che vengono prdotti dalla macchina in fase di taratura, questi files non devono essere importati...
        'Il controllo per verificare se il file è uno di questi avviene tramite 
        If (String.IsNullOrEmpty(_conferitoreCodice) AndAlso
            String.IsNullOrEmpty(_varieta) AndAlso
            String.IsNullOrEmpty(_rifBolla)) OrElse
            (_pesoTot < 1) Then

            Return False

        End If

        aggiustaPercentuali(False, 0.5)

        Return True

    End Function

End Class

Public Class ImportatoreCalibCompac : Inherits AbsImportatoreFile

    Private Function clearString(ByVal str As String) As String
        Return str.Replace(Chr(34), "")
    End Function

    Private Function toDateTime(ByVal strD As String, ByVal strT As String) As DateTime
        Return DateTime.ParseExact(strD & " " & strT, "dd/MM/yyyy HH.mm", Nothing)
    End Function

    Public Sub New(ByVal filename As String)
        MyBase.New(filename)
    End Sub

    Public Overrides Function importa() As Boolean

        '_programma As String
        '_nrBolla As String
        '_rifBolla As String
        '_note As String
        '_scarti As Nullable(Of Decimal)
        '_durata As Nullable(Of Long)

        Dim linesplit As String()
        Dim tag As Integer

        Dim Sizes As New List(Of String)
        Dim FruitCnt As New List(Of Integer)
        Dim Weights_gr As New List(Of Decimal)

        While Not _file.EndOfStream

            linesplit = Trim(_file.ReadLine()).Split(New Char() {","c}, StringSplitOptions.RemoveEmptyEntries)

            If (Not Integer.TryParse(linesplit(0), tag)) Then
                tag = -1
            End If

            If (tag = 101) Then

                _lotto = clearString(linesplit(2))
                _dataInizio = toDateTime(linesplit(3), linesplit(4))
                _dataFine = toDateTime(linesplit(5), linesplit(6))

            ElseIf (tag = 102) Then

                _conferitoreCodice = clearString(linesplit(1))
                _conferitoreNome = clearString(linesplit(2))

            ElseIf (tag = 103) Then

                _varieta = clearString(linesplit(2))

            ElseIf (tag = 400) Then 'Elenco di Sizes

                For i As Integer = 2 To linesplit.Length - 1
                    Sizes.Add(clearString(linesplit(i)))
                Next

            ElseIf (tag = 450) Then 'Fruit count for sizes

                For i As Integer = 1 To linesplit.Length - 1
                    FruitCnt.Add(CInt(linesplit(i)))
                Next

            ElseIf (tag = 451) Then 'Weights for sizes

                For i As Integer = 1 To linesplit.Length - 1
                    Weights_gr.Add(toDecimal(linesplit(i), 0D))
                Next

            End If

        End While

        _file.Close() 'Per sicurezza è meglio chiudere StreamReader altrimenti non riuscirò a spostare il file nella cartella degli importati

        If (Not Sizes.Count = FruitCnt.Count) Or (Not Sizes.Count = Weights_gr.Count) Then
            Return False
        End If

        _pesoTot = 0
        _numeroTot = 0

        Dim cnt = Sizes.Count - 1
        Dim weight_kg As Decimal
        Dim fruit_cnt As Integer
        For i As Integer = 0 To cnt
            'aggiungo un calibro
            weight_kg = Math.Round(Weights_gr.Item(i) / 1000D, 3)
            fruit_cnt = FruitCnt.Item(i)

            _calibri.Add(New Calibro("", Sizes.Item(i), weight_kg, Nothing, fruit_cnt))

            _pesoTot += weight_kg
            _numeroTot += fruit_cnt

        Next

        aggiustaPercentuali(True, 100)

        Return True

    End Function

End Class
