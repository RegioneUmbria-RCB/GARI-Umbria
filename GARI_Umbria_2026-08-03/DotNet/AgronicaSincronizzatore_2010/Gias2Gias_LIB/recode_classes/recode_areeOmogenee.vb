Public Class recode_areeOmogenee

    Private _From_PivaSuperUser As String

    Public Property From_PivaSuperUser() As String
        Get
            Return _From_PivaSuperUser
        End Get
        Set(value As String)
            _From_PivaSuperUser = Value
        End Set
    End Property


    Private _To_PivaSuperUser As String
    Public Property To_PivaSuperUser() As String
        Get

            Return _To_PivaSuperUser
        End Get
        Set(value As String)
            _To_PivaSuperUser = value
        End Set
    End Property


    Private _From_Area_Omogenea_cod As Integer

    Public Property From_Area_Omogenea_cod() As Integer
        Get
            Return _From_Area_Omogenea_cod
        End Get
        Set(value As Integer)
            _From_Area_Omogenea_cod = Value
        End Set
    End Property



    Private _To_Area_Omogenea_cod As Integer
    Public Property To_Area_Omogenea_cod() As Integer
        Get
            Return _To_Area_Omogenea_cod
        End Get
        Set(value As Integer)
            _To_Area_Omogenea_cod = Value
        End Set
    End Property



End Class
