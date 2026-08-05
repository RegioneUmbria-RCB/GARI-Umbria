Imports System.Web.Script.Serialization
Imports AgronicaCoreGestioneRichieste
Imports Newtonsoft.Json.Linq

Public Class PianoConcimazione_WS
    '
#Region "PIANO NUTRIZIONALE"
    Public Function CalcolaPianoNutrizionaleBilancio(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleBilancio_input) As AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleBilancio_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/CalcolaPianoNutrizionaleBilancio"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim BilancioOutput As AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleBilancio_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleBilancio_output)(rval)

        Return BilancioOutput
    End Function

    Public Function CalcolaPianoNutrizionaleSemplificato(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleSemplificato_input) As AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleSemplificato_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/CalcolaPianoNutrizionaleSemplificato"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim SchedeOutput As AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleSemplificato_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoNutrizionaleSemplificato_output)(rval)

        Return SchedeOutput
    End Function

#End Region

    Public Function CalcolaBilancio(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/CalcolaBilancio"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim BilancioOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output)(rval)

        Return BilancioOutput

    End Function

    Public Function CalcolaBilancio_IBF(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_IBF_INPUT) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/CalcolaBilancio_IBF"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim BilancioOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazioneBilancio_output)(rval)

        Return BilancioOutput

    End Function

    Public Function FattoriCorrettivi_Leggi(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.url IsNot Nothing AndAlso Input.url <> "" Then
            url = Input.url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If
        url &= "/FattoriCorrettivi"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim FattoriOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output)(rval)

        Return FattoriOutput

    End Function

    Public Function FattoriCorrettivi_ConFinalitaGias_Leggi(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input, Optional ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing, Optional ByRef objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing, Optional ByVal leggiUrlDaConfigurazioniSiti As Boolean = False) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.url IsNot Nothing AndAlso Input.url <> "" Then
            url = Input.url
        Else

            If Not leggiUrlDaConfigurazioniSiti Then
                Dim objAgroWebConfig As New AgroWebConfig
                url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
            End If

            If leggiUrlDaConfigurazioniSiti OrElse String.IsNullOrEmpty(url) Then
                Dim objConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                url = objConfig.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)

                If url = "" AndAlso objParametri_SuperServer IsNot Nothing Then
                    url = objConfig.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)
                End If
            End If

        End If
        url &= "/FattoriCorrettivi_ConFinalitaGias"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim FattoriOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output)(rval)

        Return FattoriOutput

    End Function

    Public Function LimiteMAS(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/LimiteMAS"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim LimiteMASOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_output)(rval)

        Return LimiteMASOutput

    End Function

    Public Function LimiteMASElenco(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_Elenco_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/LimiteMASElenco"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim LimiteMASOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_Elenco_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_Elenco_output)(rval)

        Return LimiteMASOutput

    End Function

    Public Function So_Dotazione(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input) As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/So_Dotazione"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Dotazione_Output As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione =
            jss.Deserialize(Of AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione)(rval)

        Return Dotazione_Output

    End Function

    Public Function CalcAtt_Dotazione(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input) As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/CalcareAttivo_Dotazione"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Dotazione_Output As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione =
            jss.Deserialize(Of AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione)(rval)

        Return Dotazione_Output

    End Function

    Public Function P2O5_Dotazione(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input) As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/P2O5_Dotazione"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Dotazione_Output As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione =
            jss.Deserialize(Of AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione)(rval)

        Return Dotazione_Output

    End Function

    Public Function K2O_Dotazione(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Suolo_input) As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/K2O_Dotazione"


        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Dotazione_Output As AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione =
            jss.Deserialize(Of AgronicaCoreDataProvider.TipiEnumerativi.enum_PianoConcimazione_Dotazione)(rval)

        Return Dotazione_Output

    End Function

    Public Function Regolamenti(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input, Optional ByRef objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing, Optional ByRef objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Nothing, Optional ByVal leggiUrlDaConfigurazioniSiti As Boolean = False) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.url <> "" Then
            url = Input.url
        Else
            If Not leggiUrlDaConfigurazioniSiti Then
                Dim objAgroWebConfig As New AgroWebConfig
                url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
            End If

            If leggiUrlDaConfigurazioniSiti OrElse String.IsNullOrEmpty(url) Then
                Dim objConfig As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                url = objConfig.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                If url = "" AndAlso objParametri_SuperServer IsNot Nothing Then
                    url = objConfig.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)
                End If
            End If
        End If

        url &= "/Regolamenti"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output)(rval)

        Return Output

    End Function

    Public Function FasiCicloColturale(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/FasiCicloColturale"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output)(rval)

        Return Output

    End Function

    Public Function FasiCicloColturaleStatoImpianto(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/FasiCicloColturaleStatoImpianto"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output)(rval)

        Return Output

    End Function

    Public Function SpecieConcimazione(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_SpecieConcimazione_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_SpecieConcimazione_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/SpecieConcimazione"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_SpecieConcimazione_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_SpecieConcimazione_output)(rval)

        Return Output

    End Function

    Public Function StatoImpianto(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_StatoImpianto_input, urlParameter As String) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_StatoImpianto_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""


        If urlParameter IsNot Nothing AndAlso urlParameter <> "" Then
            url = urlParameter
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/StatoImpianto"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_StatoImpianto_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_StatoImpianto_output)(rval)

        Return Output

    End Function

    Public Function FinalitaRER(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/FinalitaRER"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_output)(rval)

        Return Output

    End Function

    Public Function Precessione(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Precessione_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Precessione_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/Precessione"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Precessione_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Precessione_output)(rval)

        Return Output

    End Function

    Public Function PrecessionexSpecie(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_PrecessionexSpecie_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_PrecessionexSpecie_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/PrecessionexSpecie"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_PrecessionexSpecie_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_PrecessionexSpecie_output)(rval)

        Return Output

    End Function

    Public Function Ubicazione(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Ubicazione_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Ubicazione_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/Ubicazione"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Ubicazione_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Ubicazione_output)(rval)

        Return Output

    End Function

    Public Function DisponibilitaOssigeno(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_DisponibilitaOssigeno_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_DisponibilitaOssigeno_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/DisponibilitaOssigeno"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_DisponibilitaOssigeno_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_DisponibilitaOssigeno_output)(rval)

        Return Output

    End Function

    Public Function Frequenza(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Frequenza_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Frequenza_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/Frequenza"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Frequenza_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Frequenza_output)(rval)

        Return Output

    End Function

    Public Function MatriciOrganiche(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_MatriciOrganiche_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_MatriciOrganiche_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/MatriciOrganiche"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_MatriciOrganiche_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_MatriciOrganiche_output)(rval)

        Return Output

    End Function

    Public Function EpocheModalitaxSpecie(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/EpocheModalitaxSpecie"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_output)(rval)

        Return Output

    End Function

    Public Function EpocheModalitaxSpecie_APP(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_input, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim xlettura As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        url = xlettura.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
        url &= "/EpocheModalitaxSpecie"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_output)(rval)

        Return Output

    End Function

    Public Function Effluenti(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_input, ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig


        If Not IsNothing(objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione) AndAlso Not String.IsNullOrEmpty(objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione) Then
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        Else
            Dim xlettura As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            url = xlettura.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

            If url = "" Then
                url = xlettura.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
            End If
        End If

        url &= "/Effluenti"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output)(rval)

        Return Output

    End Function

    Public Function EffluentiXFrequenza(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_input) As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/EffluentiXFrequenza"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_EffluentiXFrequenza_output)(rval)

        Return Output

    End Function

    Public Function EpocheModalita(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_EpocheModalita_input) As AgronicaCorePianoConcimazioneBIZ.PUA_EpocheModalita_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/EpocheModalita"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_EpocheModalita_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_EpocheModalita_output)(rval)

        Return Output

    End Function

    Public Function EfficienzaxTipiAllevamentixEffluenti(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_EfficienzaxTipiAllevamentixEffluenti_input) As AgronicaCorePianoConcimazioneBIZ.PUA_EfficienzaxTipiAllevamentixEffluenti_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/EfficienzaxTipiAllevamentixEffluenti"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_EfficienzaxTipiAllevamentixEffluenti_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_EfficienzaxTipiAllevamentixEffluenti_output)(rval)

        Return Output

    End Function

    Public Function PossibilitaDistribuzione(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_PossibilitaDistribuzione_input) As AgronicaCorePianoConcimazioneBIZ.PUA_PossibilitaDistribuzione_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/PossibilitaDistribuzione"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_PossibilitaDistribuzione_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_PossibilitaDistribuzione_output)(rval)

        Return Output

    End Function

    Public Function ClassiTessitura(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_input) As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/ClassiTessitura"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_output)(rval)

        Return Output

    End Function

    Public Function ClassiTessituraDaAnalisi(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_input) As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/ClassiTessituraDaAnalisi"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output)(rval)

        Return Output

    End Function

    Public Function ClassiTessituraDaListaAnalisi(url As String, Input As List(Of AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_input)) As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output

        Dim hlpHttp As New Http
        Dim rval As String = ""

        If url = "" Then
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/ClassiTessituraDaListaAnalisi"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_ClassiTessitura_Analisi_output)(rval)

        Return Output

    End Function

    Public Function ListaTipiAllevamenti(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_ListaTipiAllevamenti_input) As AgronicaCorePianoConcimazioneBIZ.PUA_ListaTipiAllevamenti_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        Dim objAgroWebConfig As New AgroWebConfig
        url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        url &= "/ListaTipiAllevamenti"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_ListaTipiAllevamenti_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_ListaTipiAllevamenti_output)(rval)

        Return Output

    End Function


    Public Function PUAFabbisogni(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_Fabbisogni_input) As AgronicaCorePianoConcimazioneBIZ.PUA_Fabbisogni_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/PUAFabbisogni"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_Fabbisogni_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_Fabbisogni_output)(rval)

        Return Output

    End Function

    Public Function PUAFabbisogniCompresso(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_Fabbisogni_input) As AgronicaCorePianoConcimazioneBIZ.PUA_Fabbisogni_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/PUAFabbisogniCompresso"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        Dim s_comp As String = "{ ""input"": """ & AgronicaCoreUtility.AgroZip.CompressioneBase64(1, s) & """}"
        rval = hlpHttp.chiamaWS(s_comp, Nothing, url, "application/json", "POST", "application/json", "")

        Dim j As JObject = JObject.Parse(rval)

        Dim s1 As String = j("PUAFabbisogniCompressoResult")

        Dim s_decomp As String = AgronicaCoreUtility.AgroZip.DeCompressioneBase64(1, s1)

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_Fabbisogni_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_Fabbisogni_output)(s_decomp)

        Return Output

    End Function

    Public Function TipoAcqua(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_TipoAcqua_input) As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_TipoAcqua_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/TipoAcqua"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_TipoAcqua_output =
                jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_TipoAcqua_output)(rval)

        Return Output

    End Function


    Public Function CoefficienteTempo_Coltura(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteTempo_Coltura_input) As AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteTempo_Coltura_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""

        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/CoefficienteTempoColtura"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteTempo_Coltura_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteTempo_Coltura_output)(rval)

        Return Output

    End Function


    Public Function CoefficienteB_Coltura(ByVal Input As AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteB_Coltura_input) As AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteB_Coltura_output

        Dim hlpHttp As New Http
        Dim rval As String = ""
        Dim url As String = ""


        If Input.Url <> "" Then
            url = Input.Url
        Else
            Dim objAgroWebConfig As New AgroWebConfig
            url = objAgroWebConfig.GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
        End If

        url &= "/CoefficienteBColtura"

        Dim jss As New JavaScriptSerializer
        Dim s As String = jss.Serialize(Input)

        rval = hlpHttp.chiamaWS(s, Nothing, url, "application/json", "POST", "application/json", "")

        Dim Output As AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteB_Coltura_output =
            jss.Deserialize(Of AgronicaCorePianoConcimazioneBIZ.PUA_CoefficienteB_Coltura_output)(rval)

        Return Output

    End Function

End Class
