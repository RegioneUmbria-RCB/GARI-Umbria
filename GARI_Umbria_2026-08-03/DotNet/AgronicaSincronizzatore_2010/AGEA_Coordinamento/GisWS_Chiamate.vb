Public Class GisWS_Chiamate
    Dim link As String
    Dim user As String
    Dim pwd As String
    Dim auth As GisWS.SOAPAutenticazione
    Dim client As GisWS.GisWs  'OprCFascicoloFS6.OprCFascicoloFS6

    Public Sub New(link As String, user As String, pwd As String)

        Me.link = link
        Me.user = user
        Me.pwd = pwd

        auth = New GisWS.SOAPAutenticazione()
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
        client = New GisWS.GisWs()
        client.SOAPAutenticazioneValue = auth

    End Sub


    Public Function recuperaParticellaCatasto(codiceNazionale As String,
                                              numeroFoglio As Integer,
                                              numeroParticella As String,
                                              codiceSubalterno As String,
                                              dataRiferimento As Date) As GisWS.ResponseParticellaCatastoType

        Dim RequestParticellaCatasto As New GisWS.ParticellaCatastoType

        RequestParticellaCatasto.codiceNazionale = codiceNazionale
        RequestParticellaCatasto.codiceSubalterno = codiceSubalterno
        RequestParticellaCatasto.dataRiferimento = dataRiferimento
        RequestParticellaCatasto.numeroFoglio = numeroFoglio
        RequestParticellaCatasto.numeroParticella = numeroParticella

        Return client.recuperaParticellaCatasto(RequestParticellaCatasto)

    End Function

End Class
