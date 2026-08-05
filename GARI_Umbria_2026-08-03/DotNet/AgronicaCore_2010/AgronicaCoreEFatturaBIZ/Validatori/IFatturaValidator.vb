Public Interface IFatturaValidator

    Function Validate(ByVal oggettoDaValidare As Object, ByVal fattura As FatturaGias, ByRef listaErrori As List(Of String)) As Boolean
    ReadOnly Property AllowNullObject As Boolean


End Interface
