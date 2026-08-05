Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ

Public Class CodiciCedenteValidator : Inherits ValidatoreBase : Implements IFatturaValidator

    Public Sub New(ByVal objectType As Type,
                  ByVal riferimento As String,
                  ByVal soggetto As EnuValidatoreSoggetto,
                  ByVal mapper As IDecodificheMapper)
        MyBase.New(objectType, riferimento, soggetto, mapper)
    End Sub

    Public ReadOnly Property AllowNullObject As Boolean Implements IFatturaValidator.AllowNullObject
        Get
            Return False
        End Get
    End Property

    Public Function Validate(oggettoDaValidare As Object, fattura As FatturaGias, ByRef listaErrori As List(Of String)) As Boolean Implements IFatturaValidator.Validate

        Dim datiCedente = DirectCast(oggettoDaValidare, DatiCedenteMap)
        If datiCedente Is Nothing Then
            Return False
        End If

        Dim tipoImpresa = datiCedente.OttieniDatoAggiuntivo(enum_CodiciAnagrafe.TipoSocieta)
        If (String.IsNullOrEmpty(tipoImpresa)) Then
            listaErrori.Add(ComponiMessaggio("Tipo Impresa non impostato"))
        End If

        Return Not listaErrori.Any()

    End Function
End Class
