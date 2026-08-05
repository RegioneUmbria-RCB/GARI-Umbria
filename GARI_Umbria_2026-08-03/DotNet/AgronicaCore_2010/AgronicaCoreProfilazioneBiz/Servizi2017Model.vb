

Imports System.Runtime.Serialization

Public Class GenericResponse
    Public Property statusCode As Integer
    Public Property message As String
    Public Property token As String
End Class


Public Class DemetraGenericRequest
    Public Property username As String
    Public Property CUAA As String
End Class

Public Class GenericRequest

    Public Property username As String
    Public Property password As String

End Class

Public Class AccountManagerResponse
    Inherits GenericResponse
End Class

Public Class widgetManagerRequest
    Public Property tipoWidget As String
    Public Property mobile As String
End Class

Public Class widgetManagerResponse
    Inherits GenericResponse

    Public Property dettaglioEsito As widgetManagerResponse_DettaglioEsito
    Public Property errori As String

End Class

Public Class widgetManagerResponse_DettaglioEsito
    Public Property tipoRisposta As String
    Public Property titoloWidget As String
    Public Property dettaglioRisposta As List(Of widgetManagerResponse_DettaglioRisposta)
End Class

Public Class widgetManagerResponse_DettaglioRisposta
    Public Property idWidget As String
    Public Property urlServizio As String
    Public Property titolo As String
    Public Property descrizione As String
    Public Property descrizioneAggiuntiva As String
    Public Property urlImmagine As String
    Public Property tipoRender As String
    Public Property nascosto As Boolean

End Class

Public Class AccountManagerRequest
    Inherits GenericRequest
    Public Property cognome As String
    Public Property nome As String
    Public Property via As String
    Public Property numeroCivico As String
    Public Property citta As String
    Public Property provincia As String
    Public Property cap As String
    Public Property tel As String
    Public Property fax As String
    Public Property email As String
    Public Property codiceFiscale As String
End Class

Public Class ProvisioningDSSRequest
    Inherits GenericRequest

    Public Property piva_SuperUser As String
    Public Property piva As String
    Public Property pacchettoCommercialeCod As Integer

    Public Property scadenza As String

    <IgnoreDataMember>
    Public ReadOnly Property dScadenza As DateTime
        Get
            Return AgronicaCoreUtility.DataOra.DataStringaJSON_to_ISO8601(scadenza)
        End Get
    End Property

End Class

Public Class ProvisioningDSSResponse
    Inherits GenericResponse

End Class


Public Class AccountManagerEstesaRequest
    Inherits AccountManagerRequest

    Public Property ragsoc As String
    Public Property piva As String
    Public Property telefono As String
    Public Property cellulare As String
    Public Property opzioni_pagamento As String
    Public Property tipologia As String

    Public Property pec As String
    Public Property sdi As String
    Public ReadOnly Property tipoContattoFatturaInt As Integer
        Get
            If tipoContattoFattura.ToLower = "privata" Then
                Return 1
            Else
                Return 2
            End If
        End Get
    End Property
    Public Property tipoContattoFattura As String
    Public Property numeroCoupon As String
    Public Property newsletter As String


End Class

Public Class ServiziRequest
    Public Property servizio_id As Integer
    Public Property stato_id As Integer
    Public Property annotazioniStato As String

End Class

Public Class ImpreseProfilateRequest
    Public Property impresa As String
    Public Property impresa_piva As String
    Public Property serviziRequest As ServiziRequest()
End Class

Public Class ProfileManagerRequest
    Public Property username As String
    Public Property impreseProfilateRequest As ImpreseProfilateRequest()
End Class

Public Class ServiziResponse
    Public Property servizio_id As Integer
    Public Property stato_id As Integer
    Public Property esitoBool As Boolean
    Public Property esito As String
End Class

Public Class accountingServizioStato

    Public Property servizio_id As Integer
    Public Property stato_id As Integer

    <IgnoreDataMember>
    Public Property dataScadenza1 As DateTime

    Public Property dataScadenza As String

    <OnSerializing>
    Private Sub OnSerializing(context As StreamingContext)

        Me.dataScadenza = AgronicaCoreUtility.DataOra.DataOraToDate_JSON_ISO8601(Me.dataScadenza1)

    End Sub

End Class

Public Class ImpreseProfilateResponse
    Public Property impresa As String
    Public Property serviziResponse As ServiziResponse()
End Class

Public Class AccoutingUtenti

    Public Property username As String

    Public Property impreseProfilate As AccountingImpreseProfilate()

End Class

Public Class AccoutingUtentiCompleto

    Public Property username As String

    Public usernameEsisteBool As Boolean

    Public usernameEsisteMsg As String

    Public usernameProfilatoBool As Boolean

    Public usernameProfilatoMsg As String

    Public impresaProfilataBool As Boolean

    Public impresaProfilataMsg As String

    Public Property impresa As AccountingImpreseProfilate()

End Class


Public Class AccountingImpreseProfilate
    Public Property impresa As String
    Public Property servizi As accountingServizioStato()
End Class

Public Class DettaglioEsito
    Public Property username As String
    Public Property impreseProfilateResponse As ImpreseProfilateResponse()
End Class

Public Class ProfileManagerResponse
    Inherits GenericResponse
    Public Property dettaglioEsito As DettaglioEsito

End Class

Public Class AccountingResponse
    Inherits GenericResponse

    Public Property accounting As AccoutingUtenti()

End Class

Public Class AuthenticationRequestDemetra
    Inherits DemetraGenericRequest
End Class

Public Class AuthenticationRequest
    Inherits GenericRequest
End Class

Public Class AuthenticationResponse
    Inherits GenericResponse
End Class

Public Class AccountingRequest
    Public Property piva As String
End Class


Public Class AccountingUserRequest
    Public Property username As String
    Public Property cuaa As String
End Class



Public Class AccountingUserResponse
    Inherits GenericResponse

    Public Property accounting As AccoutingUtentiCompleto()

End Class

