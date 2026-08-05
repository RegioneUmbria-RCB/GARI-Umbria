Public Class recode_ProfilazioneDati

    Private _From_PivaSuperUser As String
    Public Property From_PivaSuperUser() As String
        Get
            Return _From_PivaSuperUser
        End Get
        Set(value As String)
            _From_PivaSuperUser = Value
        End Set
    End Property

    Private _To_PivaSuperUSer As String
    Public Property To_PivaSuperUSer() As String
        Get
            Return _To_PivaSuperUSer
        End Get
        Set(value As String)
            _To_PivaSuperUSer = value
        End Set
    End Property


    Private _From_Piva As String
    Public Property From_Piva() As String
        Get
            Return _From_Piva
        End Get
        Set(value As String)
            _From_Piva = value
        End Set
    End Property

    Private _To_Piva As String
    Public Property To_Piva() As String
        Get
            Return _To_Piva
        End Get
        Set(value As String)
            _To_Piva = value
        End Set
    End Property

    Private _From_Id_Profilo_Dati As Integer
    Public Property From_Id_Profilo_Dati() As Integer
        Get
            Return _From_Id_Profilo_Dati
        End Get
        Set(value As Integer)
            _From_Id_Profilo_Dati = value
        End Set
    End Property
    Private _To_Id_Profilo_Dati As Integer
    Public Property To_Id_Profilo_Dati() As Integer
        Get
            Return _To_Id_Profilo_Dati
        End Get
        Set(value As Integer)
            _To_Id_Profilo_Dati = value
        End Set
    End Property


    Private _From_Codice_Chiave As String
    Public Property From_Codice_Chiave() As String
        Get
            Return _From_Codice_Chiave
        End Get
        Set(value As String)
            _From_Codice_Chiave = value
        End Set
    End Property
    Private _To_Codice_Chiave As String
    Public Property To_Codice_Chiave() As String
        Get
            Return _To_Codice_Chiave
        End Get
        Set(value As String)
            _To_Codice_Chiave = value
        End Set
    End Property


    Private _From_Id_Gruppo As String
    Public Property From_Id_Gruppo() As String
        Get
            Return _From_Id_Gruppo
        End Get
        Set(value As String)
            _From_Id_Gruppo = value
        End Set
    End Property
    Private _To_Id_Gruppo As Integer
    Public Property To_Id_Gruppo() As String
        Get
            Return _To_Id_Gruppo
        End Get
        Set(value As String)
            _To_Id_Gruppo = value
        End Set
    End Property




End Class
