
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class SMLClaimsCfg

    Public Property TipoDiSAMLClaimPerRiconoscereUtente As enum_SAML_RiconoscimentoUtente

    Public Property ListaClaims As List(Of SAMLClaimsKeys)
    Public Property ListaClaimsPerRiconoscimento As List(Of SAMLClaimsRiconoscimento)


End Class

Public Class SAMLClaimsKeys

    Public Property SAMLKey As String
    Public Property GiasKey As String

End Class

Public Class SAMLClaimsRiconoscimento
    Inherits SAMLClaimsKeys

    Public Property Valore As String

End Class
