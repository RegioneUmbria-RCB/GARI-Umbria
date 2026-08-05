Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class DatiIOT
    Private Function LeggiUrlCoreWS(ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim DTRoot As DataTable = objConfigSiti.Leggi(0, "LanToWebSiteBasePath", "", "", objParametri_Server)
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametri_Server)

        Dim url As String = ""

        If Not IsNothing(DTRoot) AndAlso DTRoot.Rows.Count > 0 AndAlso LCase(DTRoot.Rows(0).Item("Valore")) <> "" AndAlso Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) <> "" Then
            url = DTRoot.Rows(0).Item("Valore")
            url += DTConfigSiti.Rows(0).Item("Valore")
        End If

        Return url
    End Function

    Public Function DatiIOTElabora(ByVal objParams As AgronicaCoreDTOStd.InData.IoT.RichiediDatiIOT,
                                   ByVal objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                   ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlCoreWS(objParametri_Server)
        url &= "/IOT/DatiReteAcqua.asmx/DatiIOTElabora"

        Dim hlpHttp As New Http

        Dim req As New CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.RichiediDatiIOT)
        req.objP = New CoreWS_GenericObjP(Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Super_Server), AgroKey_EncoderDecoder),
                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Server), AgroKey_EncoderDecoder),
                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Utenti), AgroKey_EncoderDecoder))
        req.InData = objParams

        Dim risp As String = hlpHttp.chiamaWS(JsonConvert.SerializeObject(New CoreWSRequest(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.RichiediDatiIOT))(req)), Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Return risp.Substring(5, risp.Length - 6)

    End Function

    Public Function LeggiDispositiviXSorgente(ByVal objParams As AgronicaCoreDTOStd.InData.IoT.DispositiviXSorgente,
                                              ByVal objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                              ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlCoreWS(objParametri_Server)
        url &= "IOT/DatiReteAcqua.asmx/LeggiDispositiviXSorgente"

        Dim hlpHttp As New Http

        Dim req As New CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.DispositiviXSorgente)
        req.objP = New CoreWS_GenericObjP(Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Super_Server), AgroKey_EncoderDecoder),
                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Server), AgroKey_EncoderDecoder),
                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Utenti), AgroKey_EncoderDecoder))
        req.InData = objParams


        Dim risp As String = hlpHttp.chiamaWS(JsonConvert.SerializeObject(New CoreWSRequest(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.DispositiviXSorgente))(req)), Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Return risp.Substring(5, risp.Length - 6)

    End Function

    Public Function LocalizzaDispositivo(ByVal objParams As AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn,
                                  ByVal objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim url As String = LeggiUrlCoreWS(objParametri_Server)
        url &= "IOT/DatiReteAcqua.asmx/LeggiDispositivo"

        Dim hlpHttp As New Http

        Dim req As New CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn)
        req.objP = New CoreWS_GenericObjP(Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Super_Server), AgroKey_EncoderDecoder),
                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Server), AgroKey_EncoderDecoder),
                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Utenti), AgroKey_EncoderDecoder))
        req.InData = objParams


        Dim risp As String = hlpHttp.chiamaWS(JsonConvert.SerializeObject(New CoreWSRequest(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn))(req)), Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Return risp.Substring(5, risp.Length - 6)

        'Dim url As String = LeggiUrlMeteo(objParametri_Server)
        'url &= "/LeggiStazione"

        'Dim hlpHttp As New Http

        'Dim objParams As New JObject From {
        '    {"Id_Stazione", stazione_cod},
        '    {"NeedSensors", needSensors}
        '}

        'Dim inputParams As New JObject
        'inputParams("inputParams") = MeteoWSParametri.Encrypt(objParams)

        'Dim risp As String = hlpHttp.chiamaWS(inputParams.ToString, Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        'Dim objRisp = JObject.Parse(risp)

        'Return objRisp("LeggiStazioneResult").ToString
    End Function

    Public Function LeggiTipoSensoriPerDispositivo(ByVal objParams As AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn,
                                                  ByVal objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                  ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As String
        Dim url As String = LeggiUrlCoreWS(objParametri_Server)
        url &= "IOT/DatiReteAcqua.asmx/LeggiElencoTipiSensorePerDispositivo"

        Dim hlpHttp As New Http

        Dim req As New CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn)
        req.objP = New CoreWS_GenericObjP(Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Super_Server), AgroKey_EncoderDecoder),
                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Server), AgroKey_EncoderDecoder),
                                          Sicurezza.Stringa_Codifica_LANCompatibile(JsonConvert.SerializeObject(objParametri_Utenti), AgroKey_EncoderDecoder))
        req.InData = objParams


        Dim risp As String = hlpHttp.chiamaWS(JsonConvert.SerializeObject(New CoreWSRequest(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.IoT.LeggiDispositivoIn))(req)), Nothing, url, "application/json", "POST", "application/json", "", TimeOutRichiesta:=600)

        Return risp.Substring(5, risp.Length - 6)
    End Function
End Class
