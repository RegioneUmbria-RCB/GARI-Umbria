Public Class DuplicaOperazione_parametriInput

    Private _id_agenda As Integer = 0
    Private _data As Date
    Private _note As String = ""

    Private _raccoglitore_cod As Integer = 0
    Private _lav_cod As Integer = 0

    Private _isLastxMultiAttivita As Boolean

    Public Property Id_agenda As Integer
        Get
            Return _id_agenda
        End Get
        Set(value As Integer)
            _id_agenda = value
        End Set
    End Property

    Public Property Data As Date
        Get
            Return _data
        End Get
        Set(value As Date)
            _data = value
        End Set
    End Property

    Public Property Note As String
        Get
            Return _note
        End Get
        Set(value As String)
            _note = value
        End Set
    End Property
    Public Property Raccoglitore_Cod As String
        Get
            Return _raccoglitore_cod
        End Get
        Set(value As String)
            _raccoglitore_cod = value
        End Set
    End Property
    Public Property Lav_Cod As String
        Get
            Return _lav_cod
        End Get
        Set(value As String)
            _lav_cod = value
        End Set
    End Property
    Public Property isLastxMultiAttivita As String
        Get
            Return _isLastxMultiAttivita
        End Get
        Set(value As String)
            _isLastxMultiAttivita = value
        End Set
    End Property
End Class

Public Class DuplicaOperazione_Impianto


    Private _piva As String
    Private _sa_cod As Integer
    Private _appezza As Integer
    Private _id_reg As Integer 

    Private _descrizione As String
    Private _sup_imp As Decimal
    Private _rag_soc As String
    Private _sa_nome As String

    Private _validita_inizio_distinta As String
    Private _validita_fine_distinta As String

    Private _app_nome As String

    Public Property piva As String
        Get
            Return _piva
        End Get
        Set(value As String)
            _piva = value
        End Set
    End Property

    Public Property sa_cod As Integer
        Get
            Return _sa_cod
        End Get
        Set(value As Integer)
            _sa_cod = value
        End Set
    End Property

    Public Property appezza As Integer
        Get
            Return _appezza
        End Get
        Set(value As Integer)
            _appezza = value
        End Set
    End Property

    Public Property id_reg As Integer
        Get
            Return _id_reg
        End Get
        Set(value As Integer)
            _id_reg = value
        End Set
    End Property

    Public Property descrizione As String
        Get
            Return _descrizione
        End Get
        Set(value As String)
            _descrizione = value
        End Set
    End Property

    Public Property sup_imp As Decimal
        Get
            Return _sup_imp
        End Get
        Set(value As Decimal)
            _sup_imp = value
        End Set
    End Property

    Public Property rag_soc As String
        Get
            Return _rag_soc
        End Get
        Set(value As String)
            _rag_soc = value
        End Set
    End Property

    Public Property sa_nome As String
        Get
            Return _sa_nome
        End Get
        Set(value As String)
            _sa_nome = value
        End Set
    End Property

    Public Property validita_inizio_distinta As String
        Get
            Return _validita_inizio_distinta
        End Get
        Set(value As String)
            _validita_inizio_distinta = value
        End Set
    End Property

    Public Property validita_fine_distinta As String
        Get
            Return _validita_fine_distinta
        End Get
        Set(value As String)
            _validita_fine_distinta = value
        End Set
    End Property

    Public Property app_nome As String
        Get
            Return _app_nome
        End Get
        Set(value As String)
            _app_nome = value
        End Set
    End Property

End Class