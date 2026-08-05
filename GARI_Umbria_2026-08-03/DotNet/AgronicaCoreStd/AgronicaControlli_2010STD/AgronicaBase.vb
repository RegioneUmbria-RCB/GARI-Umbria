Imports AgronicaCoreVarieBizSTD
Imports AgronicaCoreUtilityStd
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class AgronicaBase
    Implements iAjaxAgronica

    Public Function AjaxAgronica(Of TipoRisposta)(Url As String, PostData As String, AccessToken As String, versioneClientChiamate As String) As RispostaStandard(Of TipoRisposta) Implements iAjaxAgronica.AjaxAgronica
        Dim rval1 As RispostaStandard(Of TipoRisposta)

        Try
            Dim rval As String
            Dim hlpHttp As New Http


            rval = AjaxAgronicaCommon(Url, AccessToken, versioneClientChiamate, hlpHttp, PostData)

            rval1 = JsonConvert.DeserializeObject(Of RispostaStandard(Of TipoRisposta))(rval)



        Catch ex As Exception

            Dim MessaggioErrore As String
            MessaggioErrore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            rval1 = Nothing

            Throw New Exception("Errore in AjaxAgronica!", ex)


        End Try


        Return rval1

    End Function

    Public Function AjaxAgronica(Of TipoPost, TipoRisposta)(Url As String, PostData As TipoPost, AccessToken As String, versioneClientChiamate As String) As RispostaStandard(Of TipoRisposta) Implements iAjaxAgronica.AjaxAgronica

        Dim rval1 As RispostaStandard(Of TipoRisposta)

        Try
            Dim rval As String
            Dim hlpHttp As New Http

            Dim json As String = JsonConvert.SerializeObject(PostData)

            rval = AjaxAgronicaCommon(Url, AccessToken, versioneClientChiamate, hlpHttp, json)

            rval1 = JsonConvert.DeserializeObject(Of RispostaStandard(Of TipoRisposta))(rval)



        Catch ex As Exception

            Dim MessaggioErrore As String
            MessaggioErrore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            rval1 = Nothing

            Throw New Exception("Errore in AjaxAgronica!", ex)


        End Try


        Return rval1

    End Function

    Private Shared Function AjaxAgronicaCommon(Url As String, AccessToken As String, versioneClientChiamate As String, hlpHttp As Http, json As String) As String
        Dim rval As String
        Dim hdr As System.Net.WebHeaderCollection = Nothing

        If Not String.IsNullOrEmpty(versioneClientChiamate) OrElse Not String.IsNullOrEmpty(AccessToken) Then
            hdr = New Net.WebHeaderCollection
        End If

        If Not String.IsNullOrEmpty(versioneClientChiamate) Then
            hdr.Add("versioneGias_APP", versioneClientChiamate)
        End If


        If Not String.IsNullOrEmpty(AccessToken) Then
            hdr.Add("Authorization", AccessToken)
        End If


        'Non passare l'URL come certificato: Il certificato SSL viene gestito automaticamente da .NET
        Dim xCertificato As String = ""

        ' SBAGLIATO: passa l'URL dell'endpoint che sta per chiamare come path del certificato client che poi verrà passato alla chiamata stessa!
        'If Url.ToLower.Contains("https") Then
        '    xCertificato = Url
        'End If

        rval = hlpHttp.chiamaWS(json, xCertificato, Url, "application/json", "POST", "application/json", "", CustomHeaders:=hdr)

        If rval.StartsWith("{""d"":{") Then

            Dim jsonObject = JObject.Parse(rval)
            rval = JsonConvert.SerializeObject(CType(jsonObject("d"), JObject))
        End If


        Return rval
    End Function

    Public Function AjaxAgronica(url As String, PostData As String, AccessToken As String, versioneClientChiamate As String) As RispostaStandard Implements iAjaxAgronica.AjaxAgronica

        Dim rval1 As RispostaStandard

        Try
            Dim rval As String
            Dim hlpHttp As New Http



            rval = AjaxAgronicaCommon(url, AccessToken, versioneClientChiamate, hlpHttp, PostData)

            rval1 = JsonConvert.DeserializeObject(Of RispostaStandard)(rval)



        Catch ex As Exception

            Dim MessaggioErrore As String
            MessaggioErrore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            rval1 = Nothing

            Throw New Exception("Errore in AjaxAgronica!", ex)


        End Try


        Return rval1

    End Function

    Public Function AjaxAgronica(Of TipoPost)(url As String, PostData As TipoPost, AccessToken As String, versioneGias_APP As String) As RispostaStandard Implements iAjaxAgronica.AjaxAgronica
        Dim rval1 As RispostaStandard

        Try
            Dim rval As String
            Dim hlpHttp As New Http



            Dim json As String = JsonConvert.SerializeObject(PostData)

            rval = AjaxAgronicaCommon(url, AccessToken, versioneGias_APP, hlpHttp, json)

            rval1 = JsonConvert.DeserializeObject(Of RispostaStandard)(rval)




        Catch ex As Exception

            Dim MessaggioErrore As String
            MessaggioErrore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True, source:=True)

            rval1 = Nothing

            Throw New Exception("Errore in AjaxAgronica!", ex)


        End Try


        Return rval1
    End Function


End Class

