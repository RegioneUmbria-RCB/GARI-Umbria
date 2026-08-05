
Public Class Dati_GPS
    Public TipologiaMovimento As String
    Public IMEI As String
    Public DataOra As DateTime
    Public ID_Coordinata As Integer
    Public Latitudine As String
    Public Longitudine As String
    Public Altezza As String
    Public Speed As String
    Public DOP As String
    Public FIX As String

    Public Sub New(ByVal Stringa As String)
        If Stringa.Split("|").Length <> 0 Then
            TipologiaMovimento = ""
            IMEI = ""
            TipologiaMovimento = Stringa.Split("|")(0)
            If Stringa.Split("|").Length > 1 Then
                'posizione 1 
                IMEI = Stringa.Split("|")(1)
            End If
            Dim app_1 As String = ""
            If Stringa.Split("|").Length > 2 Then
                'posizione 2 (data)
                app_1 = Stringa.Split("|")(2)
            End If
            Dim app_2 As String = ""
            If Stringa.Split("|").Length > 3 Then
                'posizione 3 (ora)
                app_2 = Stringa.Split("|")(3)
                'DataOra
            End If
            If app_1.Split(".").Length = 3 And app_2.Split(":").Length = 3 Then
                Dim str_app As String = app_1.Split(".")(1) + "/" + app_1.Split(".")(1) + "/" + app_1.Split(".")(2)
                If IsDate(str_app) Then
                    DataOra = New DateTime(app_1.Split(".")(2), app_1.Split(".")(1), app_1.Split(".")(0), _
                                       app_2.Split(":")(0), app_2.Split(":")(1), app_2.Split(":")(2))
                Else
                    DataOra = Now
                End If
            Else
                DataOra = Now
            End If

            Latitudine = ""
            If Stringa.Split("|").Length > 4 Then
                'posizione 4 
                Latitudine = Stringa.Split("|")(4).Replace(".", ",")
            End If

            If Stringa.Split("|").Length > 5 Then
                'posizione 5
                Longitudine = Stringa.Split("|")(5).Replace(".", ",")
            End If
            If Stringa.Split("|").Length > 6 Then
                'posizione 6
                Altezza = Stringa.Split("|")(6)
            End If
            If Stringa.Split("|").Length > 7 Then
                'posizione 7
                Speed = Stringa.Split("|")(7)
            End If
            If Stringa.Split("|").Length > 8 Then
                'posizione 8
                DOP = Stringa.Split("|")(8)
            End If
            If Stringa.Split("|").Length > 9 Then
                'posizione 9
                FIX = Stringa.Split("|")(9)
            End If
        End If
    End Sub

End Class


