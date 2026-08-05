Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class FontiNC

    Public Function assegnaID_NCaFonte(ID_NC As Integer, TipoNC_Codice As Integer?, TipoNC_Chiave As String, objParametri As AgronicaCoreParametri) As Boolean
        Dim esito As Boolean = True

        If IsNothing(TipoNC_Codice) Then
            Return True
        End If

        Try
            Select Case TipoNC_Codice
                Case enum_NC_Tipi.Nativo
                    'Non faccio nulla
                Case enum_NC_Tipi.DaArticoliFRUTTAGEL
                    'Non faccio nulla
                Case enum_NC_Tipi.DaChecklistSchedaControlliTTI
                    esito = assegnaID_NCaChecklistSchedaControlliTTI(ID_NC, TipoNC_Codice, TipoNC_Chiave, objParametri)
                Case enum_NC_Tipi.DaAnalisiResiduiPDC
                    esito = assegnaID_NCaAnalisiResiduiPDC(ID_NC, TipoNC_Chiave, objParametri)
            End Select

        Catch ex As Exception
            esito = False
        End Try

        Return esito
    End Function

    Private Function assegnaID_NCaChecklistSchedaControlliTTI(ID_NC As Integer, TipoNC_Codice As Integer?, TipoNC_Chiave As String, objParametri As AgronicaCoreParametri) As Boolean
        Dim esito As Boolean = True

        Dim arrayTipoNC_Chiave As String() = TipoNC_Chiave.Split("|")
        Dim piva As String = arrayTipoNC_Chiave(0)
        Dim sa_Cod As Integer = CInt(arrayTipoNC_Chiave(1))
        Dim oggettoNC_Cod As Integer = CInt(arrayTipoNC_Chiave(2))
        Dim Audit_Cod As Integer = CInt(arrayTipoNC_Chiave(3))
        Dim tipoNC As Integer = CInt(arrayTipoNC_Chiave(4))
        Dim Parametro1_Cod As Integer = CInt(arrayTipoNC_Chiave(5))
        Dim Parametro2_Cod As Integer? = arrayTipoNC_Chiave(6)
        'nct.TipoNC_Chiave = piva impresa | sa_cod centro aziendale | codice Audit_cod | tipo: forni/macchine/zone | codice problema liv1 (es: perdite) | codice problema liv2 (es: olio)
        'nct.TipoNC_Chiave = piva & "|" & sa_Cod & "|" & oggettoNC_Cod & "|" & Audit_Cod & "|" & tipoNC & "|" & Parametro1_Cod & "|" & Parametro2_Cod

        Dim adW As New AgronicaCoreAuditDAL.Audit_Dettagli_W

        Select Case tipoNC
            Case enum_Tipo_Dettaglio_Audit.Elementi
                esito = adW.ModificaID_NC_AuditElementi(Audit_Cod, enum_AuditTipi.AuditTipi_SchedaControlliTTI, 1, oggettoNC_Cod, Parametro1_Cod, Parametro2_Cod, ID_NC, objParametri)
            Case enum_Tipo_Dettaglio_Audit.Fabbricati
                esito = adW.ModificaID_NC_AuditFabbricati(Audit_Cod, enum_AuditTipi.AuditTipi_SchedaControlliTTI, 1, piva, sa_Cod, oggettoNC_Cod, Parametro1_Cod, Parametro2_Cod, ID_NC, objParametri)
            Case enum_Tipo_Dettaglio_Audit.Macchine
                esito = adW.ModificaID_NC_AuditMacchine(Audit_Cod, enum_AuditTipi.AuditTipi_SchedaControlliTTI, 1, oggettoNC_Cod, Parametro1_Cod, Parametro2_Cod, ID_NC, objParametri)
        End Select

        Return esito
    End Function

    Private Function assegnaID_NCaAnalisiResiduiPDC(ID_NC As Integer, TipoNC_Chiave As String, objParametri As AgronicaCoreParametri) As Boolean

        Dim arrayTipoNC_Chiave As String() = TipoNC_Chiave.Split("|")
        Dim PivaSuperUser As String = arrayTipoNC_Chiave(0)
        Dim ID_PDC_Testata As Integer = CInt(arrayTipoNC_Chiave(1))
        Dim ID_PDC_Dettagli As Integer = CInt(arrayTipoNC_Chiave(2))
        Dim ID_PDC_Campione As Integer = CInt(arrayTipoNC_Chiave(3))
        Dim Analisi_Testata_Cod As Integer = CInt(arrayTipoNC_Chiave(4))
        'nct.TipoNC_Chiave = ID_PDC_Testata & "|" & ID_PDC_Dettagli & "|" & ID_PDC_Campione & "|" & Analisi_Testata_Cod & "|" & piva

        Dim pdcW As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_W
        Dim esito As Boolean = pdcW.ModificaID_NC_PDCAnalisi(PivaSuperUser, ID_PDC_Testata, ID_PDC_Dettagli, ID_PDC_Campione, Analisi_Testata_Cod, ID_NC, objParametri)

        Return esito
    End Function

End Class
