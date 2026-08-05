Imports AgronicaCoreVarieBizSTD
Imports AgronicaCoreWebServiceSTD

Public Class EseguiChiamateApi
    Implements iEseguiChiamateApi

    Private url As String
    Private AccessToken As String
    Private versioneClientChiamate As String
    Private xChiamate As New AgronicaControlli_2010STD.AgronicaBase

    Public Sub New(ByVal Url As String, ByVal AccessToken As String, versioneClientChiamate As String)

        Me.url = Url
        Me.AccessToken = AccessToken
        Me.versioneClientChiamate = versioneClientChiamate
        Me.xChiamate = New AgronicaControlli_2010STD.AgronicaBase()
    End Sub

    Public Function ChiamaWS(PostData As String) As RispostaStandard Implements iEseguiChiamateApi.ChiamaWS
        Return xChiamate.AjaxAgronica(Me.url, PostData, Me.AccessToken, Me.versioneClientChiamate)
    End Function

    Public Function ChiamaWS(Of TipoPost)(PostData As TipoPost) As RispostaStandard Implements iEseguiChiamateApi.ChiamaWS
        Return xChiamate.AjaxAgronica(Of TipoPost)(Me.url, PostData, Me.AccessToken, Me.versioneClientChiamate)
    End Function

    Public Function ChiamaWS(Of TipoRisposta)(PostData As String) As RispostaStandard(Of TipoRisposta) Implements iEseguiChiamateApi.ChiamaWS
        Return xChiamate.AjaxAgronica(Of TipoRisposta)(Me.url, PostData, Me.AccessToken, Me.versioneClientChiamate)
    End Function

    Public Function ChiamaWS(Of TipoPost, TipoRisposta)(PostData As TipoPost) As RispostaStandard(Of TipoRisposta) Implements iEseguiChiamateApi.ChiamaWS
        Return xChiamate.AjaxAgronica(Of TipoPost, TipoRisposta)(Me.url, PostData, Me.AccessToken, Me.versioneClientChiamate)
    End Function
End Class
