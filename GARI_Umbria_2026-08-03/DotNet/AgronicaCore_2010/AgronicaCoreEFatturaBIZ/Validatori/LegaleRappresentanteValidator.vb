Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEFatturaBIZ

Public Class LegaleRappresentanteValidator : Inherits ValidatoreBase : Implements IFatturaValidator

    Public Sub New(ByVal objectType As Type,
                   ByVal riferimento As String,
                   ByVal soggetto As EnuValidatoreSoggetto,
                   ByVal mapper As IDecodificheMapper)
        MyBase.New(objectType, riferimento, soggetto, mapper)
    End Sub

    Public ReadOnly Property AllowNullObject As Boolean Implements IFatturaValidator.AllowNullObject
        Get
            Return True
        End Get
    End Property

    Public Function Validate(oggettoDaValidare As Object, fattura As FatturaGias, ByRef listaErrori As List(Of String)) As Boolean Implements IFatturaValidator.Validate

        If Not fattura.Cedente.TipoImpresa = enum_TipoImpresaGerarchia.DittaIndividuale Then
            Return True
        End If

        If oggettoDaValidare Is Nothing Then
            listaErrori.Add(ComponiMessaggio("Legale rappresentante non impostato!"))
        End If

        Dim lp = DirectCast(oggettoDaValidare, LegaleRappresentanteMap)

        ' Se è riempita la ragione sociale provo a spezzarzla per spazio
        If Not String.IsNullOrEmpty(lp.Anagrafica.rag_soc) Then
            Dim nomeCognome = lp.Anagrafica.rag_soc.Split(" ").ToList
            If nomeCognome.Count < 2 Then
                listaErrori.Add(ComponiMessaggio("Cognome / Nome non impostati per il legale rappresentante"))
            End If
        Else
            If String.IsNullOrEmpty(lp.Anagrafica.Cognome) OrElse String.IsNullOrEmpty(lp.Anagrafica.Nome) Then
                listaErrori.Add(ComponiMessaggio("Cognome / Nome non impostati per il legale rappresentante"))
            End If
        End If

        Return Not listaErrori.Any()

    End Function
End Class
