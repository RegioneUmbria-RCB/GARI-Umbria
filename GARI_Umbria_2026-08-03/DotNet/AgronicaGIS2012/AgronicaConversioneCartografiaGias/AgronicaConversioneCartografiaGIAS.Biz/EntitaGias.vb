Imports AgronicaGIS2012.Commons

Public Class EntitaGias

    Private _DatoConvertito As Int16

    Public Property DatoConvertito() As Int16
        Get
            Return _DatoConvertito
        End Get
        Set(value As Int16)
            _DatoConvertito = Value
        End Set
    End Property
    Private _Layer As String
    Public Property Layer() As String
        Get
            Return _Layer
        End Get
        Set(value As String)
            _Layer = value
        End Set
    End Property

    Private _codiceGias As String
    Public Property CodiceGias() As String
        Get
            Return _codiceGias
        End Get
        Set(value As String)
            _codiceGias = value
        End Set
    End Property
    Private _DatiCartograficiED50 As New List(Of xyz)
    Public Property DatiCartograficiED50() As List(Of xyz)
        Get
            Return _DatiCartograficiED50
        End Get
        Set(value As List(Of xyz))
            _DatiCartograficiED50 = value
        End Set
    End Property

    Private _DatiCartograficiOriginali_WGS84 As New List(Of xyz)
    Public Property DatiCartograficiOriginali_WGS84() As List(Of xyz)
        Get
            Return _DatiCartograficiOriginali_WGS84
        End Get
        Set(value As List(Of xyz))
            _DatiCartograficiOriginali_WGS84 = value
        End Set
    End Property

    Private _DatiCartograficiWGS84_WKT As String
    Public Property DatiCartograficiWGS84_WKT() As String
        Get
            Return _DatiCartograficiWGS84_WKT
        End Get
        Set(value As String)
            _DatiCartograficiWGS84_WKT = value
        End Set
    End Property

    Private _Colore As String
    Public Property Colore() As String
        Get
            Return _Colore
        End Get
        Set(value As String)
            _Colore = Value
        End Set
    End Property

    Private _Descr As String
    Public Property Descr() As String
        Get
            Return _Descr
        End Get
        Set(value As String)
            _Descr = Value
        End Set
    End Property

    Private _Text As String
    Public Property Text() As String
        Get
            Return _Text
        End Get
        Set(value As String)
            _Text = Value
        End Set
    End Property

    Private _Data_Modifica As DateTime
    Public Property Data_Modifica() As DateTime
        Get
            Return _Data_Modifica
        End Get
        Set(value As DateTime)
            _Data_Modifica = Value
        End Set
    End Property

    Private _Data_Creazione As DateTime
    Public Property Data_Creazione() As DateTime
        Get
            Return _Data_Creazione
        End Get
        Set(value As DateTime)
            _Data_Creazione = Value
        End Set
    End Property


    Private _appezza As Long = 0
    Public Property Appezza() As Long
        Get
            Return _appezza
        End Get
        Set(value As Long)
            _appezza = Value
        End Set
    End Property
    Private _regImpianto As Long = 0
    Public Property RegImpianto() As Long
        Get
            Return _regImpianto
        End Get
        Set(value As Long)
            _regImpianto = Value
        End Set
    End Property
    Private _campo_cod As Integer = 0
    Public Property Campo_cod() As Integer
        Get
            Return _campo_cod
        End Get
        Set(value As Integer)
            _campo_cod = Value
        End Set
    End Property

    Private _PROV As String = ""
    Public Property PROV() As String
        Get
            Return _PROV
        End Get
        Set(value As String)
            _PROV = Value
        End Set
    End Property
    Private _COM As String = ""
    Public Property COM() As String
        Get
            Return _COM
        End Get
        Set(value As String)
            _COM = Value
        End Set
    End Property
    Private _SEZIONE As String = ""
    Public Property SEZIONE() As String
        Get
            Return _SEZIONE
        End Get
        Set(value As String)
            _SEZIONE = Value
        End Set
    End Property
    Private _FOGLIO As String = ""
    Public Property FOGLIO() As String
        Get
            Return _FOGLIO
        End Get
        Set(value As String)
            _FOGLIO = Value
        End Set
    End Property
    Private _NUMERO As String = ""
    Public Property NUMERO() As String
        Get
            Return _NUMERO
        End Get
        Set(value As String)
            _NUMERO = Value
        End Set
    End Property
    Private _SUBALTERNO As String = ""
    Public Property SUBALTERNO() As String
        Get
            Return _SUBALTERNO
        End Get
        Set(value As String)
            _SUBALTERNO = Value
        End Set
    End Property

    Private _nuovoTipoEntita As Integer
    Public Property NuovoTipoEntita() As Integer
        Get
            Return _nuovoTipoEntita
        End Get
        Set(value As Integer)
            _nuovoTipoEntita = Value
        End Set
    End Property


    Private _ElementoGrafico_cod As Integer
    Public Property ElementoGrafico_cod() As Integer
        Get
            Return _ElementoGrafico_cod
        End Get
        Set(value As Integer)
            _ElementoGrafico_cod = Value
        End Set
    End Property


    Private _Entita_Cod As Integer
    Public Property Entita_Cod() As Integer
        Get
            Return _Entita_Cod
        End Get
        Set(value As Integer)
            _Entita_Cod = Value
        End Set
    End Property
    Private _TipoOperazioneDB As Integer
    Public Property TipoOperazioneDB() As Integer
        Get
            Return _TipoOperazioneDB
        End Get
        Set(value As Integer)
            _TipoOperazioneDB = Value
        End Set
    End Property


    Private _analisi_campione_cod As Integer
    Public Property Analisi_campione_cod() As Integer
        Get
            Return _analisi_campione_cod
        End Get
        Set(value As Integer)
            _analisi_campione_cod = Value
        End Set
    End Property

    Private _id_agenda As Integer
    Public Property Id_agenda As Integer
        Get
            Return _id_agenda
        End Get
        Set(ByVal value As Integer)
            _id_agenda = value
        End Set
    End Property

    Private _id_mov_det As Integer
    Public Property Id_mov_det As Integer
        Get
            Return _id_mov_det
        End Get
        Set(value As Integer)
            _id_mov_det = value
        End Set
    End Property

    Private _Ricetta_Operazione_Cod As Integer
    Public Property Ricetta_Operazione_Cod As Integer
        Get
            Return _Ricetta_Operazione_Cod
        End Get
        Set(ByVal value As Integer)
            _Ricetta_Operazione_Cod = value
        End Set
    End Property

    Public Property Programmazione_Entita_Cod As Integer
        Get
            Return _Programmazione_Entita_Cod
        End Get
        Set(value As Integer)
            _Programmazione_Entita_Cod = value
        End Set
    End Property

    Public Property Programmazione_cod As Integer
        Get
            Return _Programmazione_cod
        End Get
        Set(value As Integer)
            _Programmazione_cod = value
        End Set
    End Property


    Private _Programmazione_cod As Integer


    Private _Programmazione_Entita_Cod As Integer

End Class
