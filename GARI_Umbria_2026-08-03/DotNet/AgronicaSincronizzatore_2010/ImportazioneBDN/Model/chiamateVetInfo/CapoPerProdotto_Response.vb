Public Class CapoPerProdotto_Response
	Inherits VetInfoResponseModel.VetInfoData_Response

	Public prescapoCodificaCodice As String
	Public prescapoCodificaDescrizione As String

	Public prescapoTipoDiagnosiCodice As String
	Public prescapoTipoDiagnosiDescrizione As String

	Public prescapoDtNascita As Object
	Public prescapoFlStatoAnomalia As String
	Public prescapoIdentificativo As String
	Public prescapoNumero As String
	Public prescapoNumeroAnimali As String
	Public prescapoSesso As String

	Public prescapoSommministrazioneCodice As String
	Public prescapoSommministrazioneDescrizione As String

	Public prescapoSottocategoriaCodice As String
	Public prescapoSottocategoriaDescrizione As String

	Public prescapoSpecieCodice As String
	Public prescapoSpecieDescrizione As String

	Public prescapoTempiSospensione As List(Of TempiSospensioneCapo)

End Class

Public Class TempiSospensioneCapo
	Public tempososTipoAlimentoCodice As String
	Public tempososTipoAlimentoDescrizione As String

	Public tempososUnitaMisuraCodice As String
	Public tempososUnitaMisuraDescrizione As String

	Public tempososValore As Integer?

End Class
