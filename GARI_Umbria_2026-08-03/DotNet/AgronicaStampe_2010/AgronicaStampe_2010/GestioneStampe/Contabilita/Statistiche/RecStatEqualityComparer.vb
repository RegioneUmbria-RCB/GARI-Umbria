Public Class RecStatEqualityComparer
    Inherits EqualityComparer(Of RecordStatistica)

    Public Overloads Overrides Function Equals(x As RecordStatistica, y As RecordStatistica) As Boolean
        Return x.Piva.Equals(y.Piva) AndAlso x.CodLiv1.Equals(y.CodLiv1) AndAlso _
            x.CodLiv2.Equals(y.CodLiv2) AndAlso x.CodLiv3.Equals(y.CodLiv3) AndAlso _
            x.DesLiv1.Equals(y.DesLiv1) AndAlso x.DesLiv2.Equals(y.DesLiv2) AndAlso _
            x.DesLiv3.Equals(y.DesLiv3) AndAlso _
            x.UnitaMisura.Equals(y.UnitaMisura)
    End Function

    Public Overloads Overrides Function GetHashCode(obj As RecordStatistica) As Integer
        Return obj.GetKeyWithUdm().GetHashCode()
    End Function
End Class


Public Class RecStatEqualityComparer2
    Inherits EqualityComparer(Of RecordStatistica)

    Public Overloads Overrides Function Equals(x As RecordStatistica, y As RecordStatistica) As Boolean
        Return x.Piva.Equals(y.Piva) AndAlso x.CodLiv1.Equals(y.CodLiv1) AndAlso _
            x.CodLiv2.Equals(y.CodLiv2) AndAlso x.CodLiv3.Equals(y.CodLiv3) AndAlso _
            x.DesLiv1.Equals(y.DesLiv1) AndAlso x.DesLiv2.Equals(y.DesLiv2) AndAlso _
             x.DesLiv3.Equals(y.DesLiv3)
    End Function

    Public Overloads Overrides Function GetHashCode(obj As RecordStatistica) As Integer
        Return obj.GetKeyWithoutUdm().GetHashCode()
    End Function
End Class
