Imports AgronicaCoreVarieBizSTD

Public Interface iEseguiChiamateApi

    Function ChiamaWS(PostData As String) As RispostaStandard

    Function ChiamaWS(Of TipoPost)(PostData As TipoPost) As RispostaStandard

    Function ChiamaWS(Of TipoRisposta)(PostData As String) As RispostaStandard(Of TipoRisposta)

    Function ChiamaWS(Of TipoPost, TipoRisposta)(PostData As TipoPost) As RispostaStandard(Of TipoRisposta)


End Interface
