Public Class ScarichiPerSomministrazione_Response
	Inherits VetInfoResponseModel.VetInfoData_Response

	'presCapo
	Public presCapoCardinalita As Integer
	Public presCapoTotale As Integer

	'regsco
	Public regscoDescrizione As String
	Public regscoFlTipoMedicinale As String
	Public regscoDtEvento As String

	Public regscoNumero As String
	Public regscoEventoCodice As String
	Public regscoEventoDescrizione As String

	Public regscoMangimeComposizione As String
	Public regscoMangimeDenominazione As String

	Public regscoProdottoAic As String
	Public regscoProdottoConfezione As String
	Public regscoProdottoDenominazione As String
	Public regscoProdottoFamigliaAic As String
	Public regscoQuantitativo As String
	Public regscoUnitaMisuraCodice As String

End Class
