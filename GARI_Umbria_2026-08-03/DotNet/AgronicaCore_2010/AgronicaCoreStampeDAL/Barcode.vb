Imports System.Text

Public Class BarCodeCreator

    Private _dbCollections As DataBlockCollection
    Private _charStart As Int32
    Private _charEnd As Int32
    Private _charPre As Int32

    Private _AiList As AI


    Private _Code128Type As String = "C"

    Public ReadOnly Property DataBlockCollection() As DataBlockCollection
        Get
            Return _dbCollections
        End Get
    End Property

    Private Function get_HumanReadable(ByVal barcodeDovePosizionare As Integer) As String
        If _dbCollections.Count > 0 Then
            Dim sb As New StringBuilder
            For Each db As DataBlock In _dbCollections.Values
                If db.BarcodeDovePosizionareIlBlocco = barcodeDovePosizionare Then
                    sb.Append(String.Format("({0}){1}", db.AI, db.Data))
                End If
            Next
            Return sb.ToString()
        End If
        Return String.Empty
    End Function

    Public ReadOnly Property HumanReadable_Top() As String
        Get
            Return get_HumanReadable(1)
        End Get
    End Property

    Public ReadOnly Property HumanReadable_Bottom() As String
        Get
            Return get_HumanReadable(3)
        End Get
    End Property

    Public ReadOnly Property HumanReadable_MID() As String
        Get
            Return get_HumanReadable(2)
        End Get
    End Property

    Public ReadOnly Property Code_Mid() As String
        Get
            Return getBlocks(2)
        End Get
    End Property


    Private Function getBarCode(ByVal sb As StringBuilder) As String

        If _Code128Type.ToLower = "none" Then
            Return sb.ToString()
        End If

        If _Code128Type = "A" Then
            If _charStart = -1 Then
                Return AzaleaCode.Azalea_Code_128_A(sb.ToString())
            Else
                Return AzaleaCode.Azalea_GS1_128_A(sb.ToString())
            End If

        End If

        If _Code128Type = "B" Then
            If _charStart = -1 Then
                Return AzaleaCode.Azalea_Code_128_B(sb.ToString())
            Else
                Return AzaleaCode.Azalea_GS1_128_B(sb.ToString())
            End If

        End If

        If _Code128Type = "C" Then
            If _charStart = -1 Then
                Return AzaleaCode.Azalea_Code_128_C(sb.ToString())
            Else
                Return AzaleaCode.Azalea_GS1_128_C(sb.ToString())
            End If

        End If

        If _Code128Type.ToLower = "auto" Then
            Return AzaleaCode.Azalea_GS1_128_A(sb.ToString())
        End If
        Return sb.ToString()
    End Function


    Private Sub InizializzaListaAI()
        If _AiList Is Nothing Then
            _AiList = New AI(True)
        End If
    End Sub

    Private Sub getCodeWithAI(ByRef sb As StringBuilder, ByVal db As DataBlock)
        If _charPre <> -1 Then
            Dim myAI As AI = _AiList.getAI(db.AI)
            If myAI IsNot Nothing AndAlso myAI.Fnc1 <> "" Then
                sb.Append(String.Format("{0}{1}{2}", db.AI, db.Data, Chr(_charPre)))
            Else
                sb.Append(String.Format("{0}{1}", db.AI, db.Data))
            End If

        Else
            sb.Append(String.Format("{0}{1}", db.AI, db.Data))
        End If
    End Sub

    Private Function getBlocks(ByVal barcodeDovePosizionare As Integer) As String
        InizializzaListaAI()

        If _dbCollections.Count > 0 Then
            Dim sb As New StringBuilder
            If _charStart <> -1 Then
                sb.Append(Chr(_charStart))
            End If
            For Each db As DataBlock In _dbCollections.Values
                If db.BarcodeDovePosizionareIlBlocco = barcodeDovePosizionare _
                Then
                    getCodeWithAI(sb, db)
                End If
            Next
            If _charEnd <> -1 Then
                sb.Append(Chr(_charEnd))
            End If


            Return getBarCode(sb)
        Else
            Return String.Empty
        End If
    End Function

    Public ReadOnly Property Code_Top() As String
        Get
            Return getBlocks(1)
        End Get
    End Property

    Public ReadOnly Property Code_Bottom() As String
        Get
            Return getBlocks(3)
        End Get
    End Property

    Public Sub New(ByVal CharStart As String, ByVal CharEnd As String, ByVal CharPre As String, ByVal code128type As String)
        _dbCollections = New DataBlockCollection(Me)
        If CharStart <> String.Empty AndAlso IsNumeric(CharStart) Then
            _charStart = CType(CharStart, Int32)
        Else
            _charStart = -1
        End If
        If CharEnd <> String.Empty AndAlso IsNumeric(CharEnd) Then
            _charEnd = CType(CharEnd, Int32)
        Else
            _charEnd = -1
        End If
        If CharPre <> String.Empty AndAlso IsNumeric(CharPre) Then
            _charPre = CType(CharPre, Int32)
        Else
            _charPre = -1
        End If

        _Code128Type = code128type
    End Sub

    Public Function CreateCode(ByVal Lunghezza As Int32, ByVal PadChar As String, ByVal Dati() As String) As String
        Dim sb As New StringBuilder

        'Aggiungo tutti i parametri tranne l'ultimo
        For i As Int32 = 0 To Dati.GetLength(0) - 2
            sb.Append(Dati(i))
        Next
        If Dati.GetLength(0) > 0 Then
            'Leggo l'ultimo parametro
            Dim s As String = Dati(Dati.GetLength(0) - 1)
            'Se la lunghezza è stata specificata
            'Aggiungo l'ultimo parametro con degli "0" a sinistra per raggiungere la lunghezza specificata
            If Not String.IsNullOrEmpty(PadChar) Then
                If Lunghezza >= sb.Length + s.Length Then
                    sb.Append(s.PadLeft(Lunghezza - sb.Length(), PadChar))
                Else
                    Throw New Exception("Dati non validi!")
                End If
            Else
                sb.Append(s)
            End If

        End If

        Return sb.ToString()
    End Function

    Public Function CreateCode(ByVal Dati() As String) As String
        Dim sb As New StringBuilder
        For i As Int32 = 0 To Dati.GetLength(0) - 1
            sb.Append(Dati(i))
        Next
        Return sb.ToString()
    End Function

    Public Shared Function AddSsccCheckDigit(ByVal Dati As String) As String
        If Dati.Length > 17 Then
            Throw New Exception("Dati non validi")
        Else
            Dati = Dati.PadLeft(17, "0"c)
            Dim chArray As Char() = Dati.ToCharArray
            Dim oddSum, evenSum, totalSum As Int32
            For i As Int32 = 16 To 0 Step -1
                Dim num As Int32 = CType(chArray(i).ToString(), Int32)
                If i Mod 2 = 0 Then
                    oddSum += num
                Else
                    evenSum += num
                End If
            Next
            oddSum *= 3
            totalSum = oddSum + evenSum
            Return Dati & (10 - (totalSum Mod 10)).ToString()
        End If
    End Function

End Class

Public Class DataBlockCollection
    Inherits SortedList

    Private _indiceDivisioneCodice As Int32

    Public Property IndiceDivisioneCodice() As Int32
        Get
            Return _indiceDivisioneCodice
        End Get
        Set(ByVal Value As Int32)
            _indiceDivisioneCodice = Value
        End Set
    End Property

    Friend _bcc As BarCodeCreator

    Default Public Overloads ReadOnly Property Item(ByVal Posizione As Int32) As DataBlock
        Get
            If MyBase.ContainsKey(Posizione) Then
                Return CType(MyBase.Item(Posizione), DataBlock)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal AI As String) As DataBlock
        Get
            For Each db As DataBlock In MyBase.Values
                If db.AI = AI Then
                    Return db
                End If
            Next
            Return Nothing
        End Get
    End Property

    Public Sub New(ByRef BCC As BarCodeCreator)
        MyBase.New()

        _bcc = BCC
        _indiceDivisioneCodice = -1
    End Sub

    Public Overloads Sub Add(ByRef db As DataBlock)
        If db IsNot Nothing Then
            If Not MyBase.ContainsKey(db.Posizione) Then
                MyBase.Add(db.Posizione, db)
            Else
                MyBase.Item(db.Posizione) = db
            End If
        End If
    End Sub

    Public Overloads Sub Remove(ByRef db As DataBlock)
        If db IsNot Nothing AndAlso MyBase.ContainsKey(db.Posizione) Then
            MyBase.Remove(db.Posizione)
        End If
    End Sub

    Public Overloads Sub Remove(ByRef Posizione As Int32)
        If MyBase.ContainsKey(Posizione) Then
            MyBase.Remove(Posizione)
        End If
    End Sub

End Class

Public Class DataBlock

    Private _dbc As DataBlockCollection
    Private _posizione As Int32
    Private _BarcodeDovePosizionareIlBlocco As Integer
    Private _ai As String
    Private _data As String
    Private _fixedLenght As Boolean
    Public Property BarcodeDovePosizionareIlBlocco As Integer
        Get
            Return _BarcodeDovePosizionareIlBlocco
        End Get
        Set(ByVal value As Integer)
            _BarcodeDovePosizionareIlBlocco = value
        End Set
    End Property

    Public ReadOnly Property Posizione() As Int32
        Get
            Return _posizione
        End Get
    End Property

    Public ReadOnly Property AI() As String
        Get
            Return _ai
        End Get
    End Property

    Public ReadOnly Property Data() As String
        Get
            Return _data
        End Get
    End Property

    Public Sub New(ByVal DBC As DataBlockCollection, ByVal Posizione As Int32, ByVal AI As String, ByVal Data As String, ByVal BarcodeDovePosizionareIlBlocco As Integer)
        _dbc = DBC
        _posizione = Posizione
        _ai = AI
        _data = Data
        _BarcodeDovePosizionareIlBlocco = BarcodeDovePosizionareIlBlocco

    End Sub

End Class

