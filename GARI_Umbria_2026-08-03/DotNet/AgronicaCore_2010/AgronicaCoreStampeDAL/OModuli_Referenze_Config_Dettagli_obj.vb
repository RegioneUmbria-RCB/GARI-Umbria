Public Class OModuli_Referenze_Config_Dettagli_obj

    Private _Piva As String

    Private _ID_Testata As Integer

    Private _Tipo As Integer

    Private _Tabella_ID As Integer

    Private _Tabella_Key As String

    Private _Configurazione_Des As String

    Private _ChkReferenza As Integer

    Private _ChkOrdine As Integer

    Private _ChkOmni_Invisibili As Integer

    Private _ChkEtichetta As Integer

    Private _ChkEdit As Integer

    Private _CFG_Etichette As New List(Of OModuli_Referenze_Config_Dettagli_Label_obj)


    Public Property CFG_Etichette As List(Of OModuli_Referenze_Config_Dettagli_Label_obj)
        Get
            Return _CFG_Etichette
        End Get
        Set(ByVal value As List(Of OModuli_Referenze_Config_Dettagli_Label_obj))
            _CFG_Etichette = value
        End Set
    End Property

    Public Property ChkEdit As Integer
        Get
            Return _ChkEdit
        End Get
        Set(ByVal value As Integer)
            _ChkEdit = value
        End Set
    End Property



    Public Property ChkEtichetta As Integer
        Get
            Return _ChkEtichetta
        End Get
        Set(ByVal value As Integer)
            _ChkEtichetta = value
        End Set
    End Property
    Public Property ChkOmni_Invisibili As Integer
        Get
            Return _ChkOmni_Invisibili
        End Get
        Set(ByVal value As Integer)
            _ChkOmni_Invisibili = value
        End Set
    End Property
    Public Property ChkOrdine As Integer
        Get
            Return _ChkOrdine
        End Get
        Set(ByVal value As Integer)
            _ChkOrdine = value
        End Set
    End Property
    Public Property ChkReferenza As Integer
        Get
            Return _ChkReferenza
        End Get
        Set(ByVal value As Integer)
            _ChkReferenza = value
        End Set
    End Property
    Public Property Configurazione_Des As String
        Get
            Return _Configurazione_Des
        End Get
        Set(ByVal value As String)
            _Configurazione_Des = value
        End Set
    End Property
    Public Property Tabella_Key As String
        Get
            Return _Tabella_Key
        End Get
        Set(ByVal value As String)
            _Tabella_Key = value
        End Set
    End Property
    Public Property Tabella_ID As Integer
        Get
            Return _Tabella_ID
        End Get
        Set(ByVal value As Integer)
            _Tabella_ID = value
        End Set
    End Property
    Public Property Tipo As Integer
        Get
            Return _Tipo
        End Get
        Set(ByVal value As Integer)
            _Tipo = value
        End Set
    End Property
    Public Property ID_Testata As Integer
        Get
            Return _ID_Testata
        End Get
        Set(ByVal value As Integer)
            _ID_Testata = value
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
