Public Class RegistroTrattamentiDaValidare_Response
	Inherits VetInfoResponseModel.VetInfoData_Response

	'forn
	Public fornProdottoFamigliaAic As String
	Public fornProdottoFamigliaDenominazione As String

	'pres
	Public presAziendaCodice As String
	Public presAziendaDenominazione As String
	Public presDetentoreDenominazione As String
	Public presDetentoreIdFiscale As String
	Public presDtEmissione As String
	Public presrigaDtFineTrattamento As Date
	Public presrigaDtInizioTrattamento As Date
	Public presNumero As String
	Public presNote As String

	Public presProprietarioDenominazione As String
	Public presProprietarioIdFiscale As String


	Public presTipoCodice As String
	Public presTipoDescrizione As String

	Public presrigaDurataTrattamento As String
	Public presrigaDescrizione As String
	Public presrigaMangimeComposizione As String
	Public presrigaMangimeDenominazione As String
	Public presrigaNumero As String
	Public presrigaPosologia As String
	Public presrigaProdottoAic As String
	Public presrigaProdottoConfezione As String
	Public presrigaProdottoDenominazione As String
	Public presrigaProdottoFamigliaAic As String
	Public presrigaProdottoFamigliaDenominazione As String
	Public presrigaProdottoUnitaMisuraCodice As String
	Public presrigaQuantitativo As Double
	Public somCardinalita As Integer
	Public tratDescrizione As String
	Public tratDtFine As Date
	Public tratNote As String
	Public tratNumero As String
	Public tratStatoCodice As String
	Public tratStatoDescrizione As String

End Class
