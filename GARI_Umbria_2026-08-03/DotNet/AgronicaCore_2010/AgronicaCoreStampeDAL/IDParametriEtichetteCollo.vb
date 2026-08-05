

Public Class IDParametriEtichetteCollo

#Region " Public "

    Public Const IDMODELLODOCUMENTOCONFIGURAZIONE As Long = 140000
    Public Const IDSEZIONEDOCUMENTOCONFIGURAZIONE As Long = 30040

    Public Const IDMODELLODOCUMENTOSTORICO As Long = 140001
    Public Const IDSEZIONEDOCUMENTOSTORICO As Long = 30073

    Public sezioneTestata As Long

    Public Disabilitata As Long
    Public AnnoOrdine As Long
    Public BarcodeOrdine As Long
    Public CodiceCalibro As Long
    Public CodiceDescrizioneCalibro As Long
    Public CodiceCliente As Long
    Public CodiceConfezione As Long
    Public CodiceLinea As Long
    Public CodiceEAN As Long
    Public CodiceImballo As Long
    Public CodicePedanaBin As Long
    Public DataConfezionamento As Long
    Public DescrizioneCalibro As Long
    Public DescrizioneConfezione As Long
    Public DescrizioneImballo As Long
    Public DescrizionePedanaBin As Long
    Public Disciplinare As Long
    Public Lingua As Long
    Public LottoEsternoConfezione As Long
    Public LottoEsternoImballo As Long
    Public LottoInterno As Long
    Public NumeroConfezioniImballo As Long
    Public NumeroCopieConfezione As Long
    Public NumeroCopieImballo As Long
    Public NumeroImballi As Long
    Public NumeroOrdine As Long
    Public Operatore As Long
    Public PesoLordoPedanaBin As Long
    Public PesoNettoConfezione As Long
    Public PesoNettoImballo As Long
    Public PesoNettoPedanaBin As Long
    Public PesoNettoStimatoConfezione As Long
    Public PesoNettoStimatoImballo As Long
    Public PesoNettoStimatoPedanaBin As Long
    Public Provenienza As Long
    Public Qualita As Long
    Public RagioneSocialeCliente As Long
    Public RigaOrdine As Long
    Public LayoutConfezione As Long
    Public LayoutImballo As Long
    Public SerieOrdine As Long
    Public Specie As Long
    Public TestoLibero1 As Long
    Public TestoLibero2 As Long
    Public TestoLibero3 As Long
    Public Uscita As Long
    Public Varieta As Long

    'Public linea As Long
    'Public specie As Long
    'Public varieta As Long
    'Public codiceCalibro As Long
    'Public descrizioneCalibro As Long
    'Public codicePedana As Long
    'Public descrizionePedana As Long
    'Public codiceImballo As Long
    'Public descrizioneImballo As Long
    'Public numeroImballi As Long
    'Public codiceConfezione As Long
    'Public descrizioneConfezione As Long
    'Public numeroConfezioni As Long
    'Public pesoNettoStimato As Long
    'Public pesoLordo As Long
    'Public pesoNetto As Long
    'Public lingua As Long
    'Public layout As Long
    'Public provenienza As Long
    'Public dataConfezionamento As Long
    'Public codiceCliente As Long
    'Public ragioneSocialeCliente As Long
    'Public lottoEsterno As Long
    'Public lottoInterno As Long
    'Public testoLibero As Long
    'Public tipocodiceean As Long
    'Public codiceean As Long
    'Public uscita As Long
    'Public qualita As Long
    'Public pesoNettoStimatoImballo As Long
    'Public pesoNettoImballo As Long
    'Public numeroCopie As Long
    'Public barcodeOrdine As Long
    'Public annoOrdine As Long
    'Public serieOrdine As Long
    'Public numeroOrdine As Long
    'Public rigaOrdine As Long

    Public obbligatori As ArrayList

#End Region

#Region " New "

    Private _idmodellodocumento As Long

    Public Sub New()
        _idmodellodocumento = 0
        ImpostaValori()
        ImpostaObbligatori()
    End Sub

#End Region

#Region " Impostazioni "

    Private Sub ImpostaValori()

        Select Case _idmodellodocumento
            Case IDMODELLODOCUMENTOCONFIGURAZIONE
                sezioneTestata = 30039

                Disabilitata = 150835
                CodiceLinea = 140000
                Specie = 140001
                Varieta = 140002
                CodiceCalibro = 140003
                DescrizioneCalibro = 140004
                CodicePedanaBin = 140005
                DescrizionePedanaBin = 140006
                CodiceImballo = 140007
                DescrizioneImballo = 140008
                NumeroImballi = 140009
                CodiceConfezione = 140010
                DescrizioneConfezione = 140011
                NumeroConfezioniImballo = 140012
                PesoNettoStimatoPedanaBin = 140013
                PesoLordoPedanaBin = 140014
                PesoNettoPedanaBin = 140015
                Lingua = 140016
                LayoutConfezione = 140017
                Provenienza = 140018
                DataConfezionamento = 140019
                CodiceCliente = 140020
                RagioneSocialeCliente = 140021
                LottoEsternoConfezione = 140022
                LottoInterno = 140023
                TestoLibero1 = 140028
                CodiceEAN = 140030
                Uscita = 140031
                Qualita = 140032
                PesoNettoStimatoImballo = 140068
                PesoNettoImballo = 140069
                PesoNettoConfezione = 140072
                PesoNettoStimatoConfezione = 140073
                NumeroCopieConfezione = 150011
                NumeroCopieImballo = 150857
                BarcodeOrdine = 150014
                SerieOrdine = 150015
                NumeroOrdine = 150016
                RigaOrdine = 150017
                AnnoOrdine = 150018
                Operatore = 150805
                TestoLibero2 = 150808
                TestoLibero3 = 150809
                LayoutImballo = 150813
                LottoEsternoImballo = 150815
                CodiceDescrizioneCalibro = 150816
                Disciplinare = 150818

                'linea = 140000
                'Specie = 140001
                'Varieta = 140002
                'CodiceCalibro = 140003
                'DescrizioneCalibro = 140004
                'codicePedana = 140005
                'descrizionePedana = 140006
                'CodiceImballo = 140007
                'DescrizioneImballo = 140008
                'NumeroImballi = 140009
                'CodiceConfezione = 140010
                'DescrizioneConfezione = 140011
                'numeroConfezioni = 140012
                'pesoNettoStimato = 140013
                'pesoLordo = 140014
                'pesoNetto = 140015
                'Lingua = 140016
                'layout = 140017
                'Provenienza = 140018
                'DataConfezionamento = 140019
                'CodiceCliente = 140020
                'RagioneSocialeCliente = 140021
                'lottoEsterno = 140022
                'LottoInterno = 140023
                'testoLibero = 140028
                'tipocodiceean = 140029
                'CodiceEAN = 140030
                'Uscita = 140031
                'qualita = 140032
                'PesoNettoStimatoImballo = 140068
                'PesoNettoImballo = 140069
                'numeroCopie = 150011
                'BarcodeOrdine = 150014
                'annoOrdine = 150018
                'SerieOrdine = 150015
                'NumeroOrdine = 150016
                'RigaOrdine = 150017

            Case IDMODELLODOCUMENTOSTORICO
                sezioneTestata = 30072

                Disabilitata = -1
                CodiceLinea = 140034
                Specie = 140035
                Varieta = 140036
                CodiceCalibro = 140037
                DescrizioneCalibro = 140038
                CodicePedanaBin = 140039
                DescrizionePedanaBin = 140040
                CodiceImballo = 140041
                DescrizioneImballo = 140042
                NumeroImballi = 140043
                CodiceConfezione = 140044
                DescrizioneConfezione = 140045
                NumeroConfezioniImballo = 140046
                PesoNettoStimatoPedanaBin = 140047
                PesoLordoPedanaBin = 140048
                PesoNettoPedanaBin = 140049
                Lingua = 140050
                LayoutConfezione = 140051
                Provenienza = 140052
                DataConfezionamento = 140053
                CodiceCliente = 140054
                RagioneSocialeCliente = 140055
                LottoEsternoConfezione = 140056
                LottoInterno = 140057
                TestoLibero1 = 140062
                CodiceEAN = 140064
                Uscita = 140065
                Qualita = 140066
                PesoNettoStimatoImballo = 140070
                PesoNettoImballo = 140071
                PesoNettoConfezione = 140074
                PesoNettoStimatoConfezione = 140075
                NumeroCopieConfezione = 150012
                NumeroCopieImballo = 150859
                BarcodeOrdine = 150020
                SerieOrdine = 150021
                NumeroOrdine = 150022
                RigaOrdine = 150023
                AnnoOrdine = 150024
                Operatore = 150807
                TestoLibero2 = 150810
                TestoLibero3 = 150811
                LayoutImballo = 150812
                LottoEsternoImballo = 150814
                CodiceDescrizioneCalibro = 150817
                Disciplinare = 150819

                'linea = 140034
                'Specie = 140035
                'Varieta = 140036
                'CodiceCalibro = 140037
                'DescrizioneCalibro = 140038
                'codicePedana = 140039
                'descrizionePedana = 140040
                'CodiceImballo = 140041
                'DescrizioneImballo = 140042
                'NumeroImballi = 140043
                'CodiceConfezione = 140044
                'DescrizioneConfezione = 140045
                'numeroConfezioni = 140046
                'pesoNettoStimato = 140047
                'pesoLordo = 140048
                'pesoNetto = 140049
                'Lingua = 140050
                'layout = 140051
                'Provenienza = 140052
                'DataConfezionamento = 140053
                'CodiceCliente = 140054
                'RagioneSocialeCliente = 140055
                'lottoEsterno = 140056
                'LottoInterno = 140057
                'testoLibero = 140062
                'tipocodiceean = 140063
                'CodiceEAN = 140064
                'Uscita = 140065
                'qualita = 140066
                'PesoNettoStimatoImballo = 140070
                'PesoNettoImballo = 140071
                'numeroCopie = 150012
                'BarcodeOrdine = 150020
                'annoOrdine = 150024
                'SerieOrdine = 150021
                'NumeroOrdine = 150022
                'RigaOrdine = 150023

        End Select
    End Sub

    Private Sub ImpostaObbligatori()
        obbligatori = New ArrayList

        Dim dt As DataTable
        dt = FF_Etichette_General.ParametriObbligatori()

        If Not dt Is Nothing Then
            For Each r As DataRow In dt.Rows
                If Not r.IsNull("ID") Then
                    obbligatori.Add(CType(r.Item("ID"), Long))
                End If
            Next
        End If
    End Sub

#End Region

End Class
