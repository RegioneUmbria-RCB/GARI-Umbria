Imports AgronicaCoreDataProviderSTD.TipiEnumerativi

Public Class RilevamentoDiMagazzino

    Public Property Prodotto As Prodotto

    Public Property Magazzino As Fabbricato

    Public Property Qta As Decimal

    Public Property Lotto As String

    Public Property Cal_Cod As Integer

    Public Property Cod_Progetto As Integer

    Public Property udm As UnitaMisura

    Public Property TipoRilevamento As enum_RilevamentoMagazzinoTipo

    Public Property Descrizione As String

    Public Property N As Decimal?

    Public Property P2O5 As Decimal?

    Public Property K2O As Decimal?

    Public Property Cu As Decimal?

End Class
