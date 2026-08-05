Public Class WSBiologicoChiamate

    Dim link As String
    Dim user As String
    Dim pwd As String
    Dim auth As BiologicoWS.SOAPAutenticazione
    Dim client As BiologicoWS.GestSIBWS

    Public Sub New(link As String, user As String, pwd As String)

        Me.link = link
        Me.user = user
        Me.pwd = pwd

        auth = New BiologicoWS.SOAPAutenticazione()
        auth.username = user
        auth.password = pwd

        'Dim basicHttpBinding As New BasicHttpBinding()
        'basicHttpBinding.Name = "iFascicoloBindingFS6"
        'basicHttpBinding.CloseTimeout = New TimeSpan(12, 0, 0)
        'basicHttpBinding.OpenTimeout = New TimeSpan(12, 0, 0)
        'basicHttpBinding.SendTimeout = New TimeSpan(12, 0, 0)
        'basicHttpBinding.ReceiveTimeout = New TimeSpan(12, 0, 0)
        'basicHttpBinding.MaxReceivedMessageSize = Integer.MaxValue
        'basicHttpBinding.Security.Mode = BasicHttpSecurityMode.Transport
        'Dim EndpointAddress As New EndpointAddress(link)
        client = New BiologicoWS.GestSIBWS()
        client.SOAPAutenticazioneValue = auth

    End Sub

    Public Function DatiConsistenzaTerritorio(cuaa As String)
        Dim richiesta_RichiestaRispostaSincrona_DatiConsistenzaTerritorio As New BiologicoWS.richiesta_RichiestaRispostaSincrona_DatiConsistenzaTerritorio
        Dim cuaaType As New BiologicoWS.codiceFiscaleType
        cuaaType.Item = cuaa
        richiesta_RichiestaRispostaSincrona_DatiConsistenzaTerritorio.codiceFiscaleType = cuaaType
        Dim resp = client.DatiConsistenzaTerritorio(richiesta_RichiestaRispostaSincrona_DatiConsistenzaTerritorio)
        Return resp
    End Function

    Public Function DatiNotificaPerSoggetto(cuaa As String)
        Dim richiesta_RichiestaRispostaSincrona_DatiNotificaPerSoggetto As New BiologicoWS.richiesta_RichiestaRispostaSincrona_DatiNotificaPerSoggetto
        Dim cuaaType As New BiologicoWS.codiceFiscaleType
        cuaaType.Item = cuaa
        'richiesta_RichiestaRispostaSincrona_DatiNotificaPerSoggetto.codiceFiscaleType = cuaaType
        richiesta_RichiestaRispostaSincrona_DatiNotificaPerSoggetto.InputDatiNotificaPerSoggetto.codiceFiscaleType = cuaaType
        Dim resp = client.DatiNotificaPerSoggetto(richiesta_RichiestaRispostaSincrona_DatiNotificaPerSoggetto)

        Return resp
    End Function

    'Public Function DatiConsistenzaTerritorio(cuaa As String)
    '    Dim richiesta_RichiestaRispostaSincrona_DatiConsistenzaTerritorio As New BiologicoWS.richiesta_RichiestaRispostaSincrona_DatiConsistenzaTerritorio
    '    Dim cuaaType As New BiologicoWS.codiceFiscaleType
    '    cuaaType.Item = cuaa
    '    richiesta_RichiestaRispostaSincrona_DatiConsistenzaTerritorio.codiceFiscaleType = cuaaType
    '    Dim resp = client.DatiConsistenzaTerritorio(richiesta_RichiestaRispostaSincrona_DatiConsistenzaTerritorio)


    '    Dim richiesta_RichiestaRispostaSincrona_ComunicazioneNuovaNotifica As BiologicoWS.richiesta_RichiestaRispostaSincrona_ComunicazioneNuovaNotifica
    '    'richiesta_RichiestaRispostaSincrona_ComunicazioneNuovaNotifica.SIBWSComunicazioneNotifica.
    '    Dim respComunicazioneNuovaNotifica = client.ComunicazioneNuovaNotifica(richiesta_RichiestaRispostaSincrona_ComunicazioneNuovaNotifica)

    '    'Dim respComunicazionePAPImportatori = client.ComunicazionePAPImportatori
    '    'Dim respDatiSezioneTerritorio = client.DatiSezioneTerritorio
    '    'Dim respDatiSezioneUnitaProduttive = client.DatiSezioneUnitaProduttive
    '    'Dim respScaricaIterNotificaCuaa = client.ScaricaIterNotificaCuaa
    '    'Dim respTestWS = client.TestWS()

    '    Return resp
    'End Function

End Class
