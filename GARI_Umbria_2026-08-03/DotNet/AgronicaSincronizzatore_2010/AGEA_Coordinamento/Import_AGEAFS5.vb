Imports System.ServiceModel

Public Class Import_AGEAFS5

    Dim link As String
    Dim user As String
    Dim pwd As String
    Dim auth As SOAPAutenticazione
    Dim client As InterServiceClient

    Public Sub New(link As String, user As String, pwd As String)

        Me.link = link
        Me.user = user
        Me.pwd = pwd

        auth = New SOAPAutenticazione()
        auth.username = user
        auth.password = pwd

        Dim basicHttpBinding As New BasicHttpBinding()
        basicHttpBinding.Name = "iFascicoloBinding"
        basicHttpBinding.CloseTimeout = New TimeSpan(12, 0, 0)
        basicHttpBinding.OpenTimeout = New TimeSpan(12, 0, 0)
        basicHttpBinding.SendTimeout = New TimeSpan(12, 0, 0)
        basicHttpBinding.ReceiveTimeout = New TimeSpan(12, 0, 0)
        basicHttpBinding.MaxReceivedMessageSize = Integer.MaxValue
        basicHttpBinding.Security.Mode = BasicHttpSecurityMode.None
        Dim EndpointAddress As New EndpointAddress(link)
        client = New InterServiceClient(basicHttpBinding, EndpointAddress)

    End Sub

    Public Sub TrovaFascicoloFS50(CUAA As String, ByRef fascicolo As ISWSToOprResponse)

        Try
            auth.nomeServizio = "TrovaFascicoloFS5.0"
            Dim resp = client.TrovaFascicoloFS50(auth, CUAA)

            fascicolo = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub TrovaFascicoloFS20(CUAA As String, ByRef fascicolo As ISWSToOprResponse)

        Try
            auth.nomeServizio = "TrovaFascicoloFS2.0"
            Dim resp = client.TrovaFascicoloFS50(auth, CUAA)

            fascicolo = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiConsistenzaFS50(CUAA As String, ByRef territorio As ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiConsistenzaFS5.0"
            Dim resp = client.LeggiConsistenzaFS50(auth, CUAA)

            territorio = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiMacchine(CUAA As String, ByRef macchine As ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiMacchine"
            Dim resp = client.LeggiMacchine(auth, CUAA)

            macchine = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiSoggetti(CUAA As String, data As Date, ByRef soggetti As ISWSToOprResponse)

        Try
            auth.nomeServizio = "DettaglioSoggettoFS1.0"
            Dim dettaglioSoggetto As New ISWSDettaglioSoggetto
            dettaglioSoggetto.Cuaa = CUAA
            dettaglioSoggetto.Data = data.Year.ToString("D4") & data.Month.ToString("D2") & data.Day.ToString("D2")
            Dim resp = client.DettaglioSoggettoFS10(auth, dettaglioSoggetto)

            soggetti = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiConsistenzaFS30(CUAA As String, ByRef territorio As ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiConsistenzaFS2.0"
            Dim resp = client.LeggiConsistenzaFS20(auth, CUAA)

            territorio = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiVincoliAgronomici(CUAA As String, ByRef vincoli As ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiCatalogoVincoli"
            Dim dettaglioVincoli As New ISWSIdParticella

            dettaglioVincoli.provincia = "015"
            dettaglioVincoli.comune = "108"
            dettaglioVincoli.sezione = ""
            dettaglioVincoli.foglio = "12"
            dettaglioVincoli.particella = "1008"
            dettaglioVincoli.subalterno = ""

            Dim resp = client.LeggiCatalogoVincoli(auth, "AG")

            vincoli = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiAllevamenti(CUAA As String, ByRef allevamenti As ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiAllevamentiFS5.0"

            Dim resp = client.LeggiAllevamentiFS50(auth, CUAA)

            allevamenti = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiPianoColturale(CUAA As String, Anno As Integer, ByRef pianoColturale As ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiPianoColturale"
            Dim piano As New ISWSPiano
            piano.Cuaa = CUAA
            piano.Anno = Anno

            Dim resp = client.LeggiPianoColturale(auth, piano)

            pianoColturale = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiTerritorio(CUAA As String, ByRef territorio As ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiTerritorio"

            Dim resp = client.LeggiTerritorio(auth, CUAA)

            territorio = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

    Public Sub LeggiCUAA_Modificati(Giorni As Integer, ByRef territorio As ISWSToOprResponse)

        Try
            auth.nomeServizio = "LeggiCuaaModificati"

            Dim resp = client.LeggiCuaaModificati(auth, Giorni)

            territorio = resp

        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try

    End Sub

End Class
