

Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class RiferimentoTrasferimentoDati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(
        byval flagLeggiPubblici As Boolean,
        ByVal piva As string,
        byval Piva_Organismo As String,
        ByVal xFiltroAggiuntivo As String, _
        ByVal xOrderBy As String, _
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As DataTable

        '----- Descrizione
        
        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.RiferimentoTrasferimentoDati_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim DTRif As New DataTable

        DTRif.Columns.Add(New DataColumn("piva_padre", GetType(String)))
        DTRif.Columns.Add(New DataColumn("ragsoc_padre", GetType(String)))

        Dim objcontatti As New Contatti_R

        Try


            DT = objcontatti.Leggi_Contatti_ByCod_Rapporto(flagLeggiPubblici,
                                                            piva,
                                                            Piva_Organismo,
                                                            COD_RIFERIMENTO_TRASFERIMENTO_DATI,
                                                            xFiltroAggiuntivo, xOrderBy,
                                                            objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Dim i As Integer
                Dim Dr As DataRow

                For i = 0 To DT.Rows.Count - 1

                    Dr = DTRif.NewRow

                    Dr.Item("piva_padre") = DT.Rows(i).Item("Cod_Contatto")
                    Dr.Item("ragsoc_padre") = If(DT.Rows(i).Item("rag_soc") = "", DT.Rows(i).Item("COGNOME") & " " & DT.Rows(i).Item("NOME"), DT.Rows(i).Item("rag_soc"))

                    DTRif.Rows.Add(Dr)

                Next

            End If


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DTRif = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DTRif


    End Function


End Class

