Public Class GisPurpose
    Public Enum enum_GisPurpose
        Completo = 0
        SementiSportello = 1
        SementiMappaturaLibera = 2
    End Enum

    Public Property Sementi As String
    Public Property SementiMappaturaLibera As String

    Public Sub New(Sementi As String, SementiMappaturaLibera As String)
        Me.Sementi = Sementi
        Me.SementiMappaturaLibera = SementiMappaturaLibera
    End Sub


    Public Function Mode() As enum_GisPurpose

        If Not String.IsNullOrEmpty(Sementi) Then

            If Sementi <> "-1" Then

                Return enum_GisPurpose.SementiSportello

            End If

        End If


        If Not String.IsNullOrEmpty(SementiMappaturaLibera) Then
            If SementiMappaturaLibera = "1" Then

                Return enum_GisPurpose.SementiMappaturaLibera

            End If
        End If


        Return enum_GisPurpose.Completo

    End Function

    Public Function SementiSportelloCod(Optional ByVal DefValue As Integer = -1) As Integer

        If Mode() = enum_GisPurpose.SementiSportello Then
            Return Sementi.Split("|")(4)
        End If

        Return DefValue

    End Function

    Public Function SementiSportelloDataInizio(Optional ByVal DefValue As DateTime = #01/01/1900#) As DateTime

        If Mode() = enum_GisPurpose.SementiSportello Then
            Return Sementi.Split("|")(6)
        End If

        Return DefValue

    End Function

    Public Function SementiSportelloDataFine(Optional ByVal DefValue As DateTime = #12/31/2100#) As DateTime

        If Mode() = enum_GisPurpose.SementiSportello Then
            Return Sementi.Split("|")(7)
        End If

        Return DefValue

    End Function

    Public Function SementiSportelloSpecie(Optional ByVal DefValue As Integer = -1) As Integer

        If Mode() = enum_GisPurpose.SementiSportello Then
            Return Sementi.Split("|")(0)
        End If

        Return DefValue

    End Function

    Public Function SementiSportelloSottospecie(Optional ByVal DefValue As Integer = -1) As Integer

        If Mode() = enum_GisPurpose.SementiSportello Then
            Return Sementi.Split("|")(1)
        End If

        Return DefValue

    End Function

    Public Function SementiSportelloGruppo(Optional ByVal DefValue As Integer = -1) As Integer

        If Mode() = enum_GisPurpose.SementiSportello Then
            Return Sementi.Split("|")(2)
        End If

        Return DefValue

    End Function

    Public Function SementiSportelloGenotipo(Optional ByVal DefValue As Integer = -1) As Integer

        If Mode() = enum_GisPurpose.SementiSportello Then
            Return Sementi.Split("|")(3)
        End If

        Return DefValue

    End Function

    Public Function SementiSportelloProgrammazione() As String

        If Mode() = enum_GisPurpose.SementiSportello Then
            Return Sementi.Split("|")(5)
        End If

        Return ""

    End Function

End Class
