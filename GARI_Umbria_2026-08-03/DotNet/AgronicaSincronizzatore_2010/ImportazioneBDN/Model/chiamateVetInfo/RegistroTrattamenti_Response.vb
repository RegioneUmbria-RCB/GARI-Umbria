Public Class RegistroTrattamenti_Response
	Inherits VetInfoResponseModel.VetInfoData_Response

	'forn
	Public fornProdottoAic As String
	Public fornProdottoFamigliaAic As String
	Public fornProdottoFamigliaDenominazione As String

	'pres
	Public presAziendaCodice As String
	Public presDetentoreDenominazione As String
	Public presDetentoreIdFiscale As String
	Public presDtEmissione As String
	Public presNote As String
	Public presNumero As String

	Public presProprietarioDenominazione As String
	Public presProprietarioIdFiscale As String

	Public presStatoCodice As String
	Public presStatoDescrizione As String

	Public presTipoCodice As String
	Public presTipoDescrizione As String

	Public presVeterinarioDenominazione As String
	Public presVeterinarioIdFiscale As String

	'presriga
	Public presrigaDescrizione As String
	Public presrigaNumero As String
	Public presrigaPosologia As String

	Public presrigaDtFineTrattamento As String
	Public presrigaDtInizioTrattamento As String
	Public presrigaDurataTrattamento As String

	Public presrigaFlOrmonale As Boolean
	Public presrigaFlTipoMedicinale As String

	Public presrigaMangimeCodice As String
	Public presrigaMangimeComposizione As String
	Public presrigaMangimeDenominazione As String

	Public presrigaProdottoAic As String
	Public presrigaProdottoConfezione As String
	Public presrigaProdottoDenominazione As String
	Public presrigaProdottoFamigliaAic As String
	Public presrigaProdottoFamigliaDenominazione As String
	Public presrigaProdottoUnitaMisuraCodice As String
	Public presrigaQuantitativo As String

	'som
	Public somCardinalita As String

	'trat
	Public tratDescrizione As String
	Public tratDtFine As String
	Public tratDtInizio As String
	Public tratEtichetta As String
	Public tratNote As String
	Public tratNumero As String

	Public tratStatoCodice As String
	Public tratStatoDescrizione As String

End Class
