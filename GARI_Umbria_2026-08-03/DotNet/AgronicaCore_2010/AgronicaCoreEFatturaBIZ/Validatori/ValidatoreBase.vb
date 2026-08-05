Public Class ValidatoreBase

    Protected ReadOnly _objectType As Type
    Protected ReadOnly _riferimento As String
    Protected ReadOnly _soggetto As EnuValidatoreSoggetto
    Protected ReadOnly _mapper As IDecodificheMapper
    Protected ReadOnly _templateMessaggio = "[{0}]: {1}"
    Public Sub New()

    End Sub
    Public Sub New(
                  ByVal objectType As Type,
                  ByVal riferimento As String,
                  ByVal soggetto As EnuValidatoreSoggetto,
                  ByVal mapper As IDecodificheMapper)
        _objectType = objectType
        _riferimento = riferimento
        _soggetto = soggetto
        _mapper = mapper
    End Sub

    Protected Function ComponiMessaggio(ByVal messaggio As String) As String
        Return String.Format(_templateMessaggio, _riferimento, messaggio)
    End Function
End Class

Public Enum EnuValidatoreSoggetto
    Cedente = 0
    Cessionario = 1
    CessionarioDiverso = 2
    LegaleRappresentante = 3
End Enum
