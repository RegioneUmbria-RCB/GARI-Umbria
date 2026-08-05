


Imports Microsoft.Web.Services3.Security
Imports Microsoft.Web.Services3.Security.Tokens
Imports Microsoft.Web.Services3.Design

Imports System.Xml





Public Class RecuperaDati




    '##########################################################################################
    Public Sub RecuperaDati_Azienda(
                            ByVal CUAA As String,
                            ByVal AnnoRiferimento As Integer,
                            ByVal FlagScaricaAzienda As Boolean,
                            ByVal FlagScaricaUtilizzi As Boolean,
                            ByVal FlagScaricaZone As Boolean,
                            ByRef objResponse As MyWsAgrea.ISWSResponse,
                            ByVal WsLink As String,
                            ByVal WsUsername As String,
                            ByVal WsPassword As String,
                            ByRef ErrCOD As Integer,
                            ByRef ErrMSG As String)

        Dim myISWSResponse As New MyWsAgrea.ISWSResponse
        Dim myProxy As New MyWsAgrea.GetPianoColturaleService


        Try '--------------------------------------------------------------------------

            Agronica.AgronicaPolicyAssertion.AgroUsername = WsUsername
            Agronica.AgronicaPolicyAssertion.AgroPassword = WsPassword
            Agronica.AgronicaPolicyAssertion.AgroFlag_Agrea1_AgriRer2 = 1

            myProxy.SetPolicy("AgronicaCustomPolicy_AGREA")
            myProxy.Url = WsLink
            myProxy.Timeout = 30000


            'myISWSResponse = myProxy.getPianoColturale("BBNFRZ67H23D458N", 2009, True, True, True)
            'myISWSResponse = myProxy.getPianoColturale("LNDGPP49A67C222A", 2009, True, True, True)
            'myISWSResponse = myProxy.getPianoColturale("SFFLNZ34H47D599Z", 2009, True, True, True)
            'myISWSResponse = myProxy.getPianoColturale("CLNGNN45T62B861K", 2009, True, True, True)
            'myISWSResponse = myProxy.getPianoColturale("GNDDNN46H57H678R", 2009, True, True, True)

            myISWSResponse = myProxy.getPianoColturale(
                                                CUAA, AnnoRiferimento,
                                                FlagScaricaAzienda,
                                                FlagScaricaUtilizzi,
                                                FlagScaricaZone)

            objResponse = myISWSResponse

            ErrCOD = 0
            ErrMSG = ""

        Catch ex As Exception '--------------------------------------------------------
            objResponse = Nothing
            ErrCOD = -101
            ErrMSG = "Errore durante il recupero delle informazioni : " & ex.Message
        Finally
            myProxy.Abort()
            myProxy.Dispose()
        End Try '----------------------------------------------------------------------

    End Sub




    '##########################################################################################
    Public Sub RecuperaDati_Fascicolo( _
                            ByVal DescrMandato As String, _
                            ByVal CUAA As String, _
                            ByRef objResponse As MyWsAgriRER.FascicoloSiar2Response, _
                            ByVal WsLink As String, _
                            ByVal WsUsername As String, _
                            ByVal WsPassword As String, _
                            ByRef ErrCOD As Integer, _
                            ByRef ErrMSG As String)

        Dim myProxy As New MyWsAgriRER.Fascicolo
        Dim myFascicoloSiarResponse As New MyWsAgriRER.FascicoloSiar2Response



        Try '--------------------------------------------------------------------------
            Agronica.AgronicaPolicyAssertion.AgroUsername = WsUsername
            Agronica.AgronicaPolicyAssertion.AgroPassword = WsPassword
            Agronica.AgronicaPolicyAssertion.AgroFlag_Agrea1_AgriRer2 = 2

            myProxy.SetPolicy("AgronicaCustomPolicy_AGRIRER")

            myProxy.Url = WsLink
            myProxy.Timeout = 30000

            myFascicoloSiarResponse = myProxy.getFascicolo2(DescrMandato, CUAA)

            objResponse = myFascicoloSiarResponse

            ErrCOD = 0
            ErrMSG = ""

        Catch ex As Exception '--------------------------------------------------------
            objResponse = Nothing
            ErrCOD = -201
            ErrMSG += "RecuperaDati_Fascicolo, errore: " & ex.Message & vbCrLf
            Try
                ErrMSG += " (" & DirectCast(DirectCast(DirectCast(ex, Microsoft.Web.Services3.ResponseProcessingException).Response, Microsoft.Web.Services3.SoapEnvelope).Fault, System.Exception).Message & ")" & vbCrLf
            Catch ex2 As Exception
                Dim debug As Boolean = True
            End Try
        Finally
            myProxy.Abort()
            myProxy.Dispose()
            Dim debug As Boolean = True
        End Try '----------------------------------------------------------------------

    End Sub



    '##########################################################################################
    Public Sub RecuperaDati_Codici( _
                            ByVal DescrMandato As String, _
                            ByRef objResponse As MyWsAgriRER.CodiciResponse, _
                            ByVal WsLink As String, _
                            ByVal WsUsername As String, _
                            ByVal WsPassword As String, _
                            ByRef ErrCOD As Integer, _
                            ByRef ErrMSG As String)

        Dim myProxy As New MyWsAgriRER.Fascicolo
        Dim myCodiciResponse As New MyWsAgriRER.CodiciResponse

        Try '--------------------------------------------------------------------------

            Agronica.AgronicaPolicyAssertion.AgroUsername = WsUsername
            Agronica.AgronicaPolicyAssertion.AgroPassword = WsPassword
            Agronica.AgronicaPolicyAssertion.AgroFlag_Agrea1_AgriRer2 = 2

            myProxy.SetPolicy("AgronicaCustomPolicy_AGRIRER")
            myProxy.Url = WsLink
            myProxy.Timeout = 30000

            myCodiciResponse = myProxy.getCodici(DescrMandato)

            objResponse = myCodiciResponse

            ErrCOD = 0
            ErrMSG = ""

        Catch ex As Exception '--------------------------------------------------------
            objResponse = Nothing
            ErrCOD = -301
            ErrMSG += "RecuperaDati_Codici, errore: " & ex.Message & vbCrLf

            Try
                ErrMSG += " (" & DirectCast(DirectCast(DirectCast(ex, Microsoft.Web.Services3.ResponseProcessingException).Response, Microsoft.Web.Services3.SoapEnvelope).Fault, System.Exception).Message & ")" & vbCrLf
            Catch ex2 As Exception
                Dim debug As Boolean = True
            End Try
        Finally
            myProxy.Abort()
            myProxy.Dispose()
            Dim debug As Boolean = True
        End Try '----------------------------------------------------------------------

    End Sub





    '##########################################################################################
    Public Sub RecuperaDati_AziendeModificate( _
                            ByVal DescrMandato As String, _
                            ByVal DataRif As String, _
                            ByRef objResponse As MyWsAgriRER.AziendeModificateResponse, _
                            ByVal WsLink As String, _
                            ByVal WsUsername As String, _
                            ByVal WsPassword As String, _
                            ByRef ErrCOD As Integer, _
                            ByRef ErrMSG As String)

        Dim myProxy As New MyWsAgriRER.Fascicolo
        Dim myAziendeModificateResponse As New MyWsAgriRER.AziendeModificateResponse

        Try '--------------------------------------------------------------------------

            Agronica.AgronicaPolicyAssertion.AgroUsername = WsUsername
            Agronica.AgronicaPolicyAssertion.AgroPassword = WsPassword
            Agronica.AgronicaPolicyAssertion.AgroFlag_Agrea1_AgriRer2 = 2

            myProxy.SetPolicy("AgronicaCustomPolicy_AGRIRER")
            myProxy.Url = WsLink
            myProxy.Timeout = 30000

            myAziendeModificateResponse = myProxy.getAziendeModificate(DescrMandato, DataRif)

            objResponse = myAziendeModificateResponse

            ErrCOD = 0
            ErrMSG = ""

        Catch ex As Exception '--------------------------------------------------------
            objResponse = Nothing
            ErrCOD = -401
            ErrMSG = "Errore durante il recupero delle informazioni : " & ex.Message
        Finally
            myProxy.Abort()
            myProxy.Dispose()
        End Try '----------------------------------------------------------------------

    End Sub


    '##########################################################################################
    Public Sub RecuperaDati_Effluenti( _
                            ByVal DescrMandato As String, _
                            ByVal CUAA As String, _
                            ByRef objResponse As MyWsEffluentiRER.EffluentiResponse, _
                            ByVal WsLink As String, _
                            ByVal WsUsername As String, _
                            ByVal WsPassword As String, _
                            ByRef ErrCOD As Integer, _
                            ByRef ErrMSG As String)

        Dim myProxy As New MyWsEffluentiRER.Effluenti
        Dim myEffluentiResponse As New MyWsEffluentiRER.EffluentiResponse

        Try '--------------------------------------------------------------------------

            Agronica.AgronicaPolicyAssertion.AgroUsername = WsUsername
            Agronica.AgronicaPolicyAssertion.AgroPassword = WsPassword
            Agronica.AgronicaPolicyAssertion.AgroFlag_Agrea1_AgriRer2 = 2

            'myProxy.SetPolicy("AgronicaCustomPolicy_AGRIRER")
            myProxy.Url = WsLink
            myProxy.Timeout = 30000

            'myEffluentiResponse = myProxy.getComunicazione(DescrMandato, idComunicazione, True)

            myEffluentiResponse = myProxy.getUltimaComunicazione(DescrMandato, CUAA)

            objResponse = myEffluentiResponse

            ErrCOD = 0
            ErrMSG = ""

        Catch ex As Exception '--------------------------------------------------------
            objResponse = Nothing
            ErrCOD = -301
            ErrMSG = "Errore durante il recupero delle informazioni : " & ex.Message
        Finally
            myProxy.Abort()
            myProxy.Dispose()
        End Try '----------------------------------------------------------------------

    End Sub

End Class
