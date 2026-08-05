Public Class IDParametriEtichettePalletCard

#Region " Public "

    Public Const IDMODELLODOCUMENTOPALLETCARD As Long = 150000
    Public Const IDPUNTOPALLETCARD As Long = 30005

    Public origine As Long
    Public linea As Long
    Public uscita As Long
    Public dataOraUscita As Long
    Public lingua As Long
    Public layout As Long
    Public barcodeOrdine As Long
    Public annoOrdine As Long
    Public serieOrdine As Long
    Public numeroOrdine As Long
    Public rigaOrdine As Long
    Public barcodeBolla As Long
    Public annoBolla As Long
    Public serieBolla As Long
    Public numeroBolla As Long
    Public rigaBolla As Long
    Public dataOrdine As Long
    Public cliente As Long
    Public specie As Long
    Public varieta As Long
    Public calibro As Long
    Public pedana As Long
    Public numeroPedane As Long
    Public imballo As Long
    Public numeroImballi As Long
    Public confezione As Long
    Public numeroConfezioni As Long
    Public pesoStimatoComplessivo As Long
    Public pesoLordo As Long
    Public pesoNettoComplessivo As Long
    Public note As Long
    Public dataConfezionamento As Long
    Public qualita As Long
    Public provenienza As Long
    Public lottoInterno As Long
    Public lottoEsternoImballo As Long
    Public lottoEsternoConfezione As Long
    Public codiceean As Long
    Public pesoStimatoXPedana As Long
    Public pesoNettoXPedana As Long
    Public nPedana As Long
    Public SSCC As Long
    Public stampata As Long
    'MC 14-06-06
    Public disciplinare As Long
    '/MC 14-06-06

#End Region

#Region " New "

    Public Sub New()
        ImpostaValori()
    End Sub

#End Region

#Region " Impostazioni "

    Private Sub ImpostaValori()
        origine = 150025
        linea = 150055
        uscita = 150056
        dataOraUscita = 150057
        lingua = 150065
        layout = 150066
        barcodeOrdine = 150026
        annoOrdine = 150027
        serieOrdine = 150028
        numeroOrdine = 150029
        rigaOrdine = 150030
        barcodeBolla = 150068
        annoBolla = 150069
        serieBolla = 150070
        numeroBolla = 150071
        rigaBolla = 150072
        dataOrdine = 150033
        cliente = 150032
        specie = 150034
        varieta = 150035
        calibro = 150037
        pedana = 150039
        numeroPedane = 150040
        imballo = 150042
        numeroImballi = 150043
        confezione = 150045
        numeroConfezioni = 150046
        pesoStimatoComplessivo = 150047
        pesoLordo = 150048
        pesoNettoComplessivo = 150049
        note = 150050
        dataConfezionamento = 150059
        qualita = 150058
        provenienza = 150060
        lottoInterno = 150061
        lottoEsternoImballo = 150075
        lottoEsternoConfezione = 150062
        codiceean = 150064
        pesoStimatoXPedana = 150051
        pesoNettoXPedana = 150052
        nPedana = 150053
        SSCC = 150054
        stampata = 150074
        disciplinare = 150077
    End Sub

#End Region

End Class

