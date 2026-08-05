Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEFatturaDAL

Public Class FatturaSDI

    Public Piva As String
    Public Rag_Soc As String
    Public Cognome As String
    Public Nome As String
    Public IdAgenda As Integer
    Public LavCod As Integer
    Public CodRisum As Integer
    Public ExtraInt As Integer
    Public Descrizione As String
    Public BloccoFlag As Integer
    Public BloccoFlagDes As String
    Public BloccoData As Date
    Public BloccoUsername As String
    Public DataDocumento As Date
    Public NrDocumento As String
    Public NrDocumentoDes As String
    Public NrDocumentoSin As String
    Public IdLog As Integer
    Public NomeFileXML As String
    Public NomeFileZIP As String
    Public DataLogSDI As Date
    Public DataGenXML As Date
    Public IdSDI As String
    Public DataOraInvio As Date
    Public TipoFattura As String
    Public NrTentativi As String
    Public StatoInvio As String
    Public StatoEsito As String
    Public StatoInvioDes As String
    Public StatoEsitoDes As String
    Public InvioXML As Boolean
    Public Note As String

    Public ReadOnly Property Cliente() As String
        Get
            Return Rag_Soc & " " & Cognome & " " & Nome
        End Get
    End Property

    Public ReadOnly Property NumeroDocumento() As String
        Get
            Return NrDocumentoSin & Right("00000" & NrDocumento, 5) & NrDocumentoDes
        End Get
    End Property

    Public ReadOnly Property TipoDocumento() As String
        Get
            If LavCod = LAVCOD_NOTA_ACCREDITO_EMESSA Then
                Return "Nota di Accredito al Cliente Emessa"
            ElseIf LavCod = LAVCOD_FATTURA_EMESSA Then
                If ExtraInt = 1 Then
                    Return "Fattura Accompagnatoria Emessa"
                End If
                Return "Fattura Emessa"
            End If
            Return ""
        End Get
    End Property

    Public ReadOnly Property Esito() As String
        Get
            If InvioXML Then
                Select Case StatoEsito
                    Case StatoFattura_SDI.RicevutaConsegna, StatoFattura_SDI.RicevutaMancataConsegna
                        Return "P"
                    Case StatoFattura_SDI.RicevutaScato
                        Return "N"
                End Select
            Else
                Select Case StatoInvio
                    Case StatoFattura_Gias.XMLGenerato
                        Return "P"
                    Case StatoFattura_Gias.XMLNonGenerabile, StatoFattura_Gias.XMLNonGeneratoPerErrori
                        Return "N"
                End Select
            End If
            Return ""
        End Get
    End Property

End Class

Public Class FatturaSDILog

    Public Piva As String
    Public IdAgenda As Integer
    Public DataLog As Date
    Public Messaggio As String
    Public CodiceErrore As String
    Public TipoErrore As Integer

End Class

Public Class ImpresaFattElettronica

    Public Piva As String
    Public Rag_Soc As String

End Class