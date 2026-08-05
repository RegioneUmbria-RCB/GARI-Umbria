Public Class ProdottoPerPrescrizione_Response
	Inherits VetInfoResponseModel.VetInfoData_Response

	'prescapo
	Public prescapoCardinalita As Integer
	Public prescapoTotale As Integer

	'presriga
	Public presrigaDescrizione As String
	Public presrigaDtFineTrattamento As Object
	Public presrigaDtInizioTrattamento As Object
	Public presrigaDurataTrattamento As Integer?
	Public presrigaNumero As String
	Public presrigaPosologia As String

	Public presrigaFlAntimicrobico As Boolean
	Public presrigaFlOrmonale As Boolean
	Public presrigaFlTipoMedicinale As String
	Public presrigaFlVaccino As Boolean

	Public presrigaMangimeComposizione As String
	Public presrigaMangimeDenominazione As String

	Public presrigaNote As String
	Public presrigaProdottoAic As String
	Public presrigaProdottoConfezione As String
	Public presrigaProdottoDenominazione As String
	Public presrigaProdottoFamigliaAic As String
	Public presrigaProdottoFamigliaDenominazione As String
	Public presrigaProdottoUnitaMisuraCodice As String
	Public presrigaQuantitativo As Double

End Class
