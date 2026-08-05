Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Text

Public Class Stampe_QDC
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Shared Function Pulisci_Piva_Fittizia(ByVal piva As String) As String
        Dim piva_return As String
        If piva.ToUpper.StartsWith("F") = True Then
            piva_return = ""
        Else
            piva_return = piva
        End If
        Return piva_return
    End Function


    '###############################################################################
    Public Shared Function ReplaceTipoReport(ByVal TipoReport As Integer)

        Select Case TipoReport

            Case enum_CodificaStampe.SchedaCampagna_2078_Semplificata
                Return enum_CodificaStampe.SchedaCampagna_Multicentro

            Case enum_CodificaStampe.Eurep_Gap_Semplificata
                'la enum_CodificaStampe.Eurep_Gap non è usata
                Return enum_CodificaStampe.Eurep_Gap_Multicentro

            Case Else
                Return TipoReport

        End Select


    End Function


    '###############################################################################
    Public Sub Prepara_Parametri_Intestazione_ReportQDC(ByVal Piva As String, _
                                                          ByVal Data_inizio As Date, _
                                                          ByVal Data_fine As Date, _
                                                           ByRef Param_Rag_Soc As String, _
                                                            ByRef Param_Piva_CUAA As String, _
                                                            ByRef Param_Indirizzo As String, _
                                                            ByRef Param_Intervallo_Date As String, _
                                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                           )


        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Stampe_QDC.Prepara_Parametri_Intestazione_ReportQDC()"

        Dim MessaggioErrore As String = ""
        Dim DT As DataTable
        Dim objStampe As New AgronicaCoreStampeDAL.DocContab

        Param_Rag_Soc = ""
        Param_Piva_CUAA = ""
        Param_Indirizzo = ""
        Param_Intervallo_Date = ""

        Try

            DT = objStampe.DatiIntestazioneImpresa_ReportContab(Piva, _
                                                              "", "", _
                                                              objParametri)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Param_Rag_Soc = DT.Rows(0).Item("Rag_Soc")
                Param_Piva_CUAA = "Piva: " & DT.Rows(0).Item("PivaReale") & " - CUAA: " & DT.Rows(0).Item("codice_cuaa")
                Param_Indirizzo = DT.Rows(0).Item("ind_des") & " - " & DT.Rows(0).Item("cap") & " " & DT.Rows(0).Item("frz_des") & " " & DT.Rows(0).Item("LOCALITA") & " (" & DT.Rows(0).Item("COMUNI_PROV") & ") "

                'Param_Rag_Soc = AgronicaCoreDataProvider.UtilityProvider.QS_SaveText(Param_Rag_Soc)
                'Param_Piva_CUAA = AgronicaCoreDataProvider.UtilityProvider.QS_SaveText(Param_Piva_CUAA)
                'Param_Indirizzo = AgronicaCoreDataProvider.UtilityProvider.QS_SaveText(Param_Indirizzo)
            End If

            Param_Intervallo_Date = "Dal " & AgronicaCoreDataProvider.UtilityProvider.Sistema_ValiditaInizio(Data_inizio) & _
                                    " al " & AgronicaCoreDataProvider.UtilityProvider.Sistema_ValiditaFine(Data_fine)


        Catch ex As Exception
            Param_Rag_Soc = ""
            Param_Piva_CUAA = ""
            Param_Indirizzo = ""
            Param_Intervallo_Date = ""
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


    End Sub



End Class
