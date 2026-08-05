Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreGisDAL

Public Class GIS_ProcessingAlgorithms_Cleaning_R
    Inherits LogProvider

    Public Function LeggiDatiDaAggiornare(ByVal algCode As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_ProcessingAlgorithms_Cleaning_R.LeggiDatiDaAggiornare()"
        Dim MessaggioErrore As String = ""
        Dim ret As DataTable = Nothing
        Try
            Dim procAlgDAL As New AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_R

            ret = procAlgDAL.LeggiDatiDaAggiornare(algCode, xFiltroAggiuntivo, xOrderBy, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            ret = Nothing
        End Try

        Return ret
    End Function

    Public Function LeggiAlgoritmoDaApplicare(ByVal algCode As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_ProcessingAlgorithms_Cleaning_R.LeggiAlgoritmoDaApplicare()"
        Dim MessaggioErrore As String = ""
        Dim ret As DataTable = Nothing
        Try
            Dim procAlgDAL As New AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_R

            ret = procAlgDAL.LeggiAlgoritmoDaApplicare(algCode, xFiltroAggiuntivo, xOrderBy, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            ret = Nothing
        End Try

        Return ret
    End Function
End Class

Public Class GIS_ProcessingAlgorithms_Cleaning_W
    Inherits LogProvider

    Public Function AggiornaStatoElaborazioneAlgoritmo(ByVal algCode As String,
                                              ByVal stato As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_ProcessingAlgorithms_Cleaning_W.AggiornaStatoElaborazioneAlgoritmo()"
        Dim MessaggioErrore As String = ""
        Dim ret As Boolean = False
        Try
            Dim procAlgDAL As New AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_W

            ret = procAlgDAL.AggiornaStato(algCode, stato, xFiltroAggiuntivo, xOrderBy, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            ret = Nothing
        End Try

        Return ret
    End Function

    Public Function AggiornaStatoElaborazioneFaseAlgoritmo(ByVal algCode As String,
                                              ByVal stato As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_ProcessingAlgorithms_Cleaning_W.AggiornaStatoElaborazioneFaseAlgoritmo()"
        Dim MessaggioErrore As String = ""
        Dim ret As Boolean = False
        Try
            Dim procAlgDAL As New AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_W

            ret = procAlgDAL.AggiornaStatoFase(algCode, stato, xFiltroAggiuntivo, xOrderBy, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            ret = Nothing
        End Try

        Return ret
    End Function

    Public Function AggiornaStatoElaborazioneChiaveGraficaAlgoritmo(ByVal chiavegrafica As String,
                                                                    ByVal algCode As String,
                                                                    ByVal stato As Integer,
                                                                    ByVal xFiltroAggiuntivo As String,
                                                                    ByVal xOrderBy As String,
                                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreGisBIZ.GIS_ProcessingAlgorithms_Cleaning_W.AggiornaStatoElaborazioneChiaveGraficaAlgoritmo()"
        Dim MessaggioErrore As String = ""
        Dim ret As Boolean = False
        Try
            Dim procAlgDAL As New AgronicaCoreGisDAL.GIS_ProcessingAlgorithms_Cleaning_W

            ret = procAlgDAL.AggiornaStatoDettaglio(chiavegrafica, algCode, stato, xFiltroAggiuntivo, xOrderBy, objParametri)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            ret = Nothing
        End Try

        Return ret
    End Function
End Class
