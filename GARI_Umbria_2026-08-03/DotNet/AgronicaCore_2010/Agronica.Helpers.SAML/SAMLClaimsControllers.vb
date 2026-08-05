Public Class SAMLClaimsControllers

    Public Shared Function LeggiValoreDataChiave(chiave As String, cfg As List(Of SAMLClaimsRiconoscimento)) As String

        Dim rval As String = (
            From k In cfg
            Where k.GiasKey = chiave
            Select k.Valore
        ).FirstOrDefault

        Return rval

    End Function

End Class
