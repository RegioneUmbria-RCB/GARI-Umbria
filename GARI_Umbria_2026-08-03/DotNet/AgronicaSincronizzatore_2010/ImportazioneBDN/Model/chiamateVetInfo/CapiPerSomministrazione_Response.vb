Public Class CapiPerSomministrazione_Response
	Inherits VetInfoResponseModel.VetInfoData_Response

	'prescapo
	Public prescapoDtnascita As String
	Public prescapoIdentificativo As String
	Public prescapoNumero As String
	Public prescapoNumeroAnimali As String
	Public prescapoSesso As String

	Public prescapoSpecieCodice As String
	Public prescapoSpecieDescrizione As String

	Public prescapoTempiSospensione As List(Of TempoSospensione)

End Class

Public Class TempoSospensione
	Inherits VetInfoResponseModel.VetInfoData_Response

	Public tempososTipoAlimentoCodice As String
	Public tempososTipoAlimentoDescrizione As String
	Public tempososUnitaMisuraCodice As String
	Public tempososUnitaMisuraDescrizione As String
	Public tempososValore As String

End Class


