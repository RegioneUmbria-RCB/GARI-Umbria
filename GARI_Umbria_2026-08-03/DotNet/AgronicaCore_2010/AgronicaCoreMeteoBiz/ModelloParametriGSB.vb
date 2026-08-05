Public Class ModelloParametriGSB

    Public Property ParametriChiamataWsMeteo As ParametriChiamataWsMeteo

    Public Property GiasOnline_WS_Meteo_Meteo As String

    Public Property ListaSpecieFiltro As New List(Of Integer)

    Public Property Validita_Inizio_impianti As Date

End Class

Public Class ParametriChiamataWsMeteo

    Public Property ASG_Utente_Username As String
    Public Property ASG_Utente_Username_Crypt As String

    Public Property ASG_Utente_Password_Crypt As String


End Class
