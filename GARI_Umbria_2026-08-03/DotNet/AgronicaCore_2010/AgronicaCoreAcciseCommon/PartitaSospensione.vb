Public Class PartitaSospensione : Implements IListableDataTable

    Public ProgressivoRiga As Integer
    Public CodiceProdotto As String
    Public TipoMessaggio As Integer
    Public DocumentoAccompagnamentoDocumento As DocumentoAccompagnamentoMovimento
    Public Movimentazione As Movimentazione
    Public Stoccaggio As Stoccaggio
    Public Quantita As Quantita
    Public Contabilita As Contabilita

End Class

Public Class DocumentoAccompagnamentoMovimento

    Public TipoMovimento As String
    Public DataEmissione As DateTime
    Public NumeroIdentificativo As String
    Public DataRientro3C As DateTime
    Public DataGruppoStampa As DateTime

End Class

Public Class Movimentazione

    Public CS As String
    Public MittenteDestinatario As String
    Public Provenzienza As String

End Class

Public Class Stoccaggio

    Public TipoStoccaggio As String
    Public VNC As Decimal
    Public NumeroConfezioni As Decimal

End Class

Public Class Quantita

    Public LitriIdrati As Decimal
    Public GradoAlconalico As Decimal

End Class

Public Class Contabilita

    Public CauMov As String
    Public PosizioneFiscale As String
    Public AccisaSospesa As Decimal
    Public AccisaAssolta As Decimal

End Class


Public Class PSMapper

    Public TipoMessaggio As String
    Public TipoMovimento As String
    Public CausaleMov As String
    Public CS As String

End Class