Imports System.Net

Public Class OprFascicoloFS6_Chiamate

    Dim link As String
    Dim user As String
    Dim pwd As String
    Dim auth As OprFascicoloFS6.SOAPAutenticazione
    Dim client As OprFascicoloFS6.OprFascicoloFS6

    Public Sub New(link As String, user As String, pwd As String)

        Me.link = link
        Me.user = user
        Me.pwd = pwd

        auth = New OprFascicoloFS6.SOAPAutenticazione()
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
        client = New OprFascicoloFS6.OprFascicoloFS6()
        client.SOAPAutenticazioneValue = auth

    End Sub

    Public Sub TrovaFascicoloFS60(CUAA As String, ByRef fascicolo As OprFascicoloFS6.ISWSToOprResponse)

        Try
            auth.nomeServizio = "TrovaFascicoloFS6.0"

            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Dim resp = client.TrovaFascicoloFS6(CUAA)

            ServicePointManager.SecurityProtocol = seBK

            fascicolo = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiSchedaValidazione(CUAA As String, IdScheda As String, ByRef pianoColturale As OprFascicoloFS6.ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiSchedaValidazione"
            Dim objRichiesta As New OprFascicoloFS6.RichiestaScheda
            Dim iswsScheda As New OprFascicoloFS6.ISWReqScheda
            iswsScheda.IdScheda = IdScheda
            iswsScheda.CUAA = CUAA
            objRichiesta.ISWReqScheda = iswsScheda

            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Dim resp = client.LeggiSchedaValidazione(objRichiesta)

            ServicePointManager.SecurityProtocol = seBK

            pianoColturale = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiSchedeCuaa(CUAA As String, Anno As Integer, ByRef territorio As OprFascicoloFS6.ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiSchedeCUAA"
            Dim objRichiesta As New OprFascicoloFS6.RichiestaSchede
            Dim iswsSchede As New OprFascicoloFS6.ISWSCHEDE
            iswsSchede.AnnoCampagna = Anno
            iswsSchede.CUAA = CUAA
            objRichiesta.ISWSCHEDE = iswsSchede

            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Dim resp = client.LeggiSchedeCUAA(objRichiesta)

            ServicePointManager.SecurityProtocol = seBK

            territorio = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiCUAA_Modificati(Giorni As Integer, ByRef territorio As OprFascicoloFS6.ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiCuaaModificati"

            'Dim seBK = ServicePointManager.SecurityProtocol
            'ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Dim resp = client.LeggiCuaaModificatiFS6(Giorni)

            'ServicePointManager.SecurityProtocol = seBK


            territorio = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub AnagraficaAziendaFS6(CUAA As String, ByRef risposta As OprFascicoloFS6.ISWSToOprResponse)

        Try
            auth.nomeServizio = "AnagraficaAziendaFS6"

            Dim resp = client.AnagraficaAziendaFS6(CUAA)


            risposta = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiConsistenzaFS6(CUAA As String, ByRef fascicolo As OprFascicoloFS6.ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiConsistenzaFS6.0"

            Dim seBK = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12

            Dim resp = client.LeggiConsistenzaFS6(CUAA)

            ServicePointManager.SecurityProtocol = seBK

            fascicolo = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

End Class
