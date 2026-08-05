Public Class PCG_Validity_List
    Public Property id_appezzamento_padre As String
    Public Property id_appezzamento As String
    Public Property id_dichiarazione As String
    Public Property validita_inizio As DateTime
    Public Property validita_fine As DateTime

    Public Function GetKey() As String
        Return id_appezzamento_padre + "|" + id_appezzamento + "|" + id_dichiarazione
    End Function


End Class
