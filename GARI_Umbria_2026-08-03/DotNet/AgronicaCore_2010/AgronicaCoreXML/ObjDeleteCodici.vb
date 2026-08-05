Public Class ObjDeleteCodici

    Private _Impresa As Boolean
    Private _Centro As Boolean
    Private _Fabbricato As Boolean
    Private _Campo As Boolean
    Private _Appezzamento As Boolean
    Private _Impianto As Boolean
    Private _Distinta As Boolean
    Private _Contatto As Boolean
    Private _ImpreseXParticelle As Boolean
    Private _Parco_Macchine As Boolean
    Private _ParticelleCatastali As Boolean
    Private _HT_Keys_Distinta As Hashtable
    Private _HT_Keys_Impianto As Hashtable
    Private _HT_Keys_Centro As Hashtable

    Public Sub New()
        _Impresa = False
        _Centro = False
        _Fabbricato = False
        _Campo = False
        _Appezzamento = False
        _Impianto = False
        _Distinta = False
        _Contatto = False
        _ImpreseXParticelle = False
        _Parco_Macchine = False
        _ParticelleCatastali = False
        _HT_Keys_Distinta = New Hashtable
        _HT_Keys_Impianto = New Hashtable
        _HT_Keys_Centro = New Hashtable
    End Sub

    Public Property Impresa() As String
        Get
            Return _Impresa
        End Get
        Set(ByVal value As String)
            _Impresa = value
        End Set
    End Property

    Public Property Centro() As String
        Get
            Return _Centro
        End Get
        Set(ByVal value As String)
            _Centro = value
        End Set
    End Property

    Public Property Fabbricato() As String
        Get
            Return _Fabbricato
        End Get
        Set(ByVal value As String)
            _Fabbricato = value
        End Set
    End Property

    Public Property Campo() As String
        Get
            Return _Campo
        End Get
        Set(ByVal value As String)
            _Campo = value
        End Set
    End Property

    Public Property Appezzamento() As String
        Get
            Return _Appezzamento
        End Get
        Set(ByVal value As String)
            _Appezzamento = value
        End Set
    End Property

    Public Property Impianto() As String
        Get
            Return _Impianto
        End Get
        Set(ByVal value As String)
            _Impianto = value
        End Set
    End Property

    Public Property Distinta() As String
        Get
            Return _Distinta
        End Get
        Set(ByVal value As String)
            _Distinta = value
        End Set
    End Property

    Public Property Contatto() As String
        Get
            Return _Contatto
        End Get
        Set(ByVal value As String)
            _Contatto = value
        End Set
    End Property

    Public Property ImpreseXParticelle() As String
        Get
            Return _ImpreseXParticelle
        End Get
        Set(ByVal value As String)
            _ImpreseXParticelle = value
        End Set
    End Property

    Public Property Parco_Macchine() As String
        Get
            Return _Parco_Macchine
        End Get
        Set(ByVal value As String)
            _Parco_Macchine = value
        End Set
    End Property


    Public Property ParticelleCatastali() As String
        Get
            Return _ParticelleCatastali
        End Get
        Set(ByVal value As String)
            _ParticelleCatastali = value
        End Set
    End Property

    Public Property HT_Keys_Distinta() As Hashtable
        Get
            Return _HT_Keys_Distinta
        End Get
        Set(ByVal value As Hashtable)
            _HT_Keys_Distinta = value
        End Set
    End Property

    Public Property HT_Keys_Impianto() As Hashtable
        Get
            Return _HT_Keys_Impianto
        End Get
        Set(ByVal value As Hashtable)
            _HT_Keys_Impianto = value
        End Set
    End Property

    Public Property HT_Keys_Centro() As Hashtable
        Get
            Return _HT_Keys_Centro
        End Get
        Set(ByVal value As Hashtable)
            _HT_Keys_Centro = value
        End Set
    End Property


End Class