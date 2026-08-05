Imports AgronicaCoreVarieBizSTD

Public Interface iAjaxAgronica

    Function AjaxAgronica(url As String, PostData As String, AccessToken As String, versioneClientChiamate As String) As RispostaStandard

    Function AjaxAgronica(Of TipoPost)(url As String, PostData As TipoPost, AccessToken As String, versioneClientChiamate As String) As RispostaStandard

    Function AjaxAgronica(Of TipoRisposta)(Url As String, PostData As String, AccessToken As String, versioneClientChiamate As String) As RispostaStandard(Of TipoRisposta)

    Function AjaxAgronica(Of TipoPost, TipoRisposta)(Url As String, PostData As TipoPost, AccessToken As String, versioneClientChiamate As String) As RispostaStandard(Of TipoRisposta)

End Interface
