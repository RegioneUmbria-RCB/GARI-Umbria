Imports AgronicaCoreAuditDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class AuditAgronica

    Public Function LeggiCampi(Audit_tipo As Integer, Regolamento_Cod As Integer, Campo_Cod As Integer, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditCampiModel)

        Dim auditLeggi As New Audit_Campi_R

        Dim dt As DataTable =
            auditLeggi.Leggi(Audit_tipo, Regolamento_Cod, Campo_Cod, "", "", objParametri)

        Dim rval As List(Of AuditCampiModel) =
            (From dd In dt.AsEnumerable
             Select New AuditCampiModel With {
                     .Audit_Tipo = dd("Audit_tipo"),
                     .Regolamento_Cod = dd("Regolamento_Cod"),
                     .Campo_Cod = dd("Campo_Cod"),
                     .Campo_Des = dd("Campo_Des"),
                     .Note = IIf(IsDBNull(dd("Note")), "", dd("Note"))
            }).ToList

        Return rval
    End Function

    Public Function LeggiCodici(ByVal Audit_tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Disp_Cod As Integer, ByVal Sezione_Cod As Integer, ByVal Default_Cod As Integer, ByVal Data_Inizio As Date, ByVal Data_Fine As Date, ByVal objParametri As AgronicaCoreParametri, Optional ByVal xfiltroaggiuntivo As String = "") As List(Of AuditCodiciModel)

        Dim auditLeggi As New Audit_Disposizioni_R
        Dim dt As DataTable = auditLeggi.LeggiCodici(Audit_tipo, Regolamento_Cod, Disp_Cod, Sezione_Cod, 0, Default_Cod, Data_Inizio, Data_Fine, xfiltroaggiuntivo, "", objParametri)

        Dim rval As List(Of AuditCodiciModel) =
            (From dd In dt.AsEnumerable
             Select New AuditCodiciModel With {
                .Audit_Tipo = dd("Audit_tipo"),
                .Regolamento_Cod = dd("Regolamento_Cod"),
                .Disp_Cod = dd("Disp_Cod"),
                .Disp_Nome = dd("Disp_Nome"),
                .Sezione_Cod = dd("Sezione_Cod"),
                .Sezione_Des = dd("Sezione_Des"),
                .Parte = dd("Parte"),
                .Punto_Numero = dd("Punto_Numero"),
                .Descrizione = dd("Descrizione"),
                .Allegato = IIf(IsDBNull(dd("Allegato")), "", dd("Allegato")),
                .Nota = dd("Nota"),
                .Tipo = dd("Tipo"),
                .Punteggio = IIf(IsDBNull(dd("Punteggio")), 0, dd("Punteggio")),
                .PropostaCorrettiva = IIf(IsDBNull(dd("PropostaCorrettiva")), 0, dd("PropostaCorrettiva")),
                .Criterio = IIf(IsDBNull(dd("Criterio")), "", dd("Criterio")),
                .Valore = IIf(IsDBNull(dd("Valore")), "", dd("Valore")),
                .Valore_2 = IIf(IsDBNull(dd("Valore_2")), "", dd("Valore_2")),
                .Punto_Numero_Default = IIf(IsDBNull(dd("Punto_Numero_Default")), "", dd("Punto_Numero_Default")),
                .Punto_Numero_Deroga = IIf(IsDBNull(dd("Punto_Numero_Deroga")), "", dd("Punto_Numero_Deroga"))
            }).ToList

        Return rval
    End Function

    Public Function LeggiCodiciDisposizioni(ByVal Audit_tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Disp_Cod As Integer, ByVal Parte As Integer, ByVal Data_Inizio As Date, ByVal Data_Fine As Date, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditCodiciModel)

        Dim auditLeggi As New Audit_Disposizioni_R
        Dim dt As DataTable = auditLeggi.LeggiCodiciDisposizioni(Audit_tipo, Regolamento_Cod, Disp_Cod, 0, Parte, Data_Inizio, Data_Fine, "", "", objParametri)

        Dim rval As List(Of AuditCodiciModel) =
            (From dd In dt.AsEnumerable
             Select New AuditCodiciModel With {
                .Audit_Tipo = dd("Audit_tipo"),
                .Regolamento_Cod = dd("Regolamento_Cod"),
                .Disp_Cod = dd("Disp_Cod"),
                .Disp_Nome = dd("Disp_Nome"),
                .Sezione_Cod = dd("Sezione_Cod"),
                .Sezione_Des = dd("Sezione_Des"),
                .Parte = dd("Parte"),
                .Punto_Numero = dd("Punto_Numero"),
                .Descrizione = dd("Descrizione"),
                .Allegato = IIf(IsDBNull(dd("Allegato")), "", dd("Allegato")),
                .Nota = dd("Nota"),
                .Tipo = dd("Tipo"),
                .Punteggio = IIf(IsDBNull(dd("Punteggio")), 0, dd("Punteggio")),
                .PropostaCorrettiva = IIf(IsDBNull(dd("PropostaCorrettiva")), 0, dd("PropostaCorrettiva")),
                .Criterio = IIf(IsDBNull(dd("Criterio")), "", dd("Criterio")),
                .Valore = IIf(IsDBNull(dd("Valore")), "", dd("Valore")),
                .Valore_2 = IIf(IsDBNull(dd("Valore_2")), "", dd("Valore_2")),
                .Punto_Numero_Default = IIf(IsDBNull(dd("Punto_Numero_Default")), "", dd("Punto_Numero_Default")),
                .Punto_Numero_Deroga = IIf(IsDBNull(dd("Punto_Numero_Deroga")), "", dd("Punto_Numero_Deroga")),
                .FunCalcoloLivello = IIf(IsDBNull(dd("FunCalcoloLivello")), "", dd("FunCalcoloLivello"))
            }).ToList

        Return rval
    End Function

    Public Function LeggiDisposizioni(Audit_tipo As Integer, Regolamento_Cod As Integer, Campo_Cod As Integer, Disp_Cod As Integer, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditDisposizioniModel)
        Dim auditLeggi As New Audit_Disposizioni_R
        Dim dt As DataTable = auditLeggi.Leggi(Audit_tipo, Regolamento_Cod, Campo_Cod, Disp_Cod, "", "", "", objParametri)
        Dim rval As List(Of AuditDisposizioniModel) =
            (From dd In dt.AsEnumerable
             Select New AuditDisposizioniModel With {
                    .Audit_Tipo = dd("Audit_tipo"),
                    .Regolamento_Cod = dd("Regolamento_Cod"),
                    .Disp_Cod = dd("Disp_Cod"),
                    .Disp_Nome = dd("Disp_Nome"),
                    .Descrizione = dd("Descrizione"),
                    .Attivazione = IIf(IsDBNull(dd("Attivazione")), "", dd("Attivazione")),
                    .Campo = dd("Campo"),
                    .Ordine = IIf(IsDBNull(dd("Ordine")), 0, dd("Ordine")),
                    .Note = IIf(IsDBNull(dd("Note")), "", dd("Note"))
            }).ToList
        Return rval
    End Function

    Public Function LeggiDomandeDisposizioni(Audit_tipo As Integer, Regolamento_Cod As Integer, Disp_Cod As Integer, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditDomandeDisposizioniModel)
        Dim auditLeggi As New Audit_Disposizioni_R
        Dim dt As DataTable = auditLeggi.LeggiDomande(Audit_tipo, Regolamento_Cod, Disp_Cod, 0, "", "", objParametri)
        Dim rval As List(Of AuditDomandeDisposizioniModel) =
            (From dd In dt.AsEnumerable
             Select New AuditDomandeDisposizioniModel With {
                    .Audit_Tipo = dd("Audit_tipo"),
                    .Regolamento_Cod = dd("Regolamento_Cod"),
                    .Disp_Cod = dd("Disp_Cod"),
                    .Domanda_Cod = dd("Domanda_Cod"),
                    .Disp_Cod_Dominante = IIf(IsDBNull(dd("Disp_Cod_Dominante")), 0, dd("Disp_Cod_Dominante"))
            }).ToList
        Return rval
    End Function

    Public Function LeggiDomandeInterviste(Audit_Tipo As Integer, Regolamento_Cod As Integer, Intervista_Cod As Integer, ByVal Piva As String, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditDomandeIntervisteModel)

        Dim auditLeggi As New Audit_Domande_Interviste_R
        Dim dt As DataTable = auditLeggi.LeggiDomande(Audit_Tipo, Regolamento_Cod, "", "", objParametri)

        Dim rval As List(Of AuditDomandeIntervisteModel) =
            (From dd In dt.AsEnumerable
             Select New AuditDomandeIntervisteModel With {
                     .Audit_Tipo = dd("Audit_tipo"),
                     .Regolamento_Cod = dd("Regolamento_Cod"),
                     .Domanda_Cod = dd("Domanda_Cod"),
                     .Domanda_Des = dd("Domanda_Des"),
                     .Attivazione = IIf(IsDBNull(dd("Attivazione")), "", dd("Attivazione")),
                     .Tipo = dd("Tipo"),
                     .Ordine = IIf(IsDBNull(dd("Ordine")), 0, dd("Ordine"))
            }).ToList

        Return rval

    End Function

    Public Function LeggiRegolamenti(Audit_tipo As Integer, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditRegolamentiModel)

        Dim auditLeggi As New Audit_Regolamenti_R

        Dim dt As DataTable =
            auditLeggi.Leggi_RegolamentoValido(Audit_tipo, AGRODATAINIZIO, AGRODATAFINE, "", "Ordine", objParametri)

        Dim rval As List(Of AuditRegolamentiModel) =
            (From dd In dt.AsEnumerable
             Select New AuditRegolamentiModel With {
                     .Audit_Tipo = dd("Audit_tipo"),
                     .Regolamento_Cod = dd("Regolamento_Cod"),
                     .Regolamento_Des = dd("Regolamento_Des")
            }).ToList

        Return rval
    End Function

    Public Function LeggiSezioni(ByVal Audit_tipo As Integer, ByVal Regolamento_Cod As Integer, ByVal Disp_Cod As Integer, ByVal Parte As Integer, ByVal Data As Date, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditSezioniModel)

        Dim auditLeggi As New Audit_Disposizioni_R
        Dim dt As DataTable = auditLeggi.LeggiSezioni(Audit_tipo, Regolamento_Cod, Disp_Cod, 0, Parte, Data, "", "", objParametri)

        Dim rval As List(Of AuditSezioniModel) =
            (From dd In dt.AsEnumerable
             Select New AuditSezioniModel With {
                     .Audit_Tipo = dd("Audit_tipo"),
                     .Regolamento_Cod = dd("Regolamento_Cod"),
                     .Sezione_Cod = dd("Sezione_Cod"),
                     .Sezione_Des = dd("Sezione_Des"),
                     .CalcoloPunteggio = dd("CalcoloPunteggio"),
                     .Parte = dd("Parte"),
                     .Ordine = IIf(IsDBNull(dd("Ordine")), 0, dd("Ordine")),
                     .FunCalcoloLivello = IIf(IsDBNull(dd("FunCalcoloLivello")), "", dd("FunCalcoloLivello"))
            }).ToList

        Return rval
    End Function

    Public Function LeggiStati(Audit_tipo As Integer, ByVal objParametri As AgronicaCoreParametri) As List(Of AuditStatiModel)

        Dim auditLeggi As New Audit_Stati_R

        Dim dt As DataTable =
            auditLeggi.Leggi(Audit_tipo, AGRODATAINIZIO, AGRODATAFINE, "", "Ordine", objParametri)

        Dim rval As List(Of AuditStatiModel) =
            (From dd In dt.AsEnumerable
             Select New AuditStatiModel With {
                     .Audit_Tipo = dd("Audit_tipo"),
                     .Stato_Cod = dd("Stato_Cod"),
                     .Stato_Des = dd("Stato_Des"),
                     .Ordine = IIf(IsDBNull(dd("Ordine")), 0, dd("Ordine"))
            }).ToList

        Return rval
    End Function

End Class
