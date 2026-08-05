Public Class OModuli_Referenze_Config_Testata_obj


    Private _Configurazioni As New List(Of AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_obj)

    Private _Piva As String

    Private _Modulo_Generazione As Integer

    Private _Descrizione As String

    Private _oFiltro_Veg_Cod As String

    Private _oFiltro_Cul_cod As String


    Public Property Configurazioni As List(Of AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_obj)
        Get
            Return _Configurazioni
        End Get
        Set(ByVal value As List(Of AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_obj))
            _Configurazioni = value
        End Set
    End Property


    Public Property OFiltro_Cul_cod As String
        Get
            Return _oFiltro_Cul_cod
        End Get
        Set(ByVal value As String)
            _oFiltro_Cul_cod = value
        End Set
    End Property
    Public Property OFiltro_Veg_Cod As String
        Get
            Return _oFiltro_Veg_Cod
        End Get
        Set(ByVal value As String)
            _oFiltro_Veg_Cod = value
        End Set
    End Property
    Public Property Descrizione As String
        Get
            Return _Descrizione
        End Get
        Set(ByVal value As String)
            _Descrizione = value
        End Set
    End Property
    Public Property Modulo_Generazione As Integer
        Get
            Return _Modulo_Generazione
        End Get
        Set(ByVal value As Integer)
            _Modulo_Generazione = value
        End Set
    End Property
    Public Property Piva As String
        Get
            Return _Piva
        End Get
        Set(ByVal value As String)
            _Piva = value
        End Set
    End Property




End Class
