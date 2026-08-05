Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class Interscambio_Parco_Macchine_R
    Public Function GetInterscambioParcoMacchine(
                                                ByVal codice_esterno As String,
                                                ByVal sistemaCod As Integer,
                                                ByRef GiasContext As Gias_DeveloperServer_Entities,
                                                Optional ByVal macCod As Integer = 0
                                                ) As Interscambio_Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreInterscambioBIZ.Interscambio_Parco_Macchine_R.GetInterscambioParcoMacchineEF()"
        Dim interscambio As Interscambio_Parco_Macchine

        Try
            Dim interscambioPM_R As New AgronicaCoreInterscambioDAL.Interscambio_Parco_Macchine_R
            interscambio = interscambioPM_R.GetInterscambioParcoMacchineEF(codice_esterno, sistemaCod, GiasContext, macCod)
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return interscambio
    End Function

    Public Function GetChiaveEsterna(
                                    ByVal sistemaCod As Integer,
                                    ByVal macCod As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreInterscambioBIZ.Interscambio_Parco_Macchine_R.GetChiaveEsterna()"
        Dim interscambio As DataTable

        Try
            Dim interscambioPM_R As New AgronicaCoreInterscambioDAL.Interscambio_Parco_Macchine_R
            interscambio = interscambioPM_R.Leggi(sistemaCod,
                                                    macCod,
                                                    "",
                                                    AGRODATAINIZIO,
                                                    AGRODATAFINE,
                                                    "",
                                                    "",
                                                    objParametri
                                                    )
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return interscambio
    End Function

End Class

Public Class Interscambio_Parco_Macchine_W
    Public Function PrepareEditInterscambioEF(
                                             ByVal macchina As Parco_Macchine,
                                             ByVal codice_esterno As String,
                                             ByVal sistemaCod As Integer,
                                             ByRef GiasContext As Gias_DeveloperServer_Entities,
                                             ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                             Optional ByVal ScriviLog As Boolean = True
                                             ) As Interscambio_Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.PrepareEditInterscambioD2G()"
        Dim interscambioPM_R As New Interscambio_Parco_Macchine_R
        Dim interscambio As Interscambio_Parco_Macchine

        Try
            Dim interscambioDAL_W As New AgronicaCoreInterscambioDAL.Interscambio_Parco_Macchine_W
            interscambio = interscambioDAL_W.PrepareEditInterscambioEF(
                macchina,
                codice_esterno,
                sistemaCod,
                GiasContext,
                objParametri_Server,
                objParametri_Utenti,
                False
                )
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return interscambio

    End Function

    Public Function PrepareCreateInterscambioEF(
                                               ByVal macchina As Parco_Macchine,
                                               ByVal codice_esterno As String,
                                               ByVal sistemaCod As Integer,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               Optional ByVal ScriviLog As Boolean = True
                                               ) As Interscambio_Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.PrepareCreateInterscambioD2G()"
        Dim interscambio As Interscambio_Parco_Macchine

        Try
            Dim interscambioDAL_W As New AgronicaCoreInterscambioDAL.Interscambio_Parco_Macchine_W
            interscambio = interscambioDAL_W.PrepareCreateInterscambioEF(
                macchina,
                codice_esterno,
                sistemaCod,
                GiasContext,
                objParametri_Server,
                objParametri_Utenti,
                False
                )
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return interscambio

    End Function

    Public Function PrepareDeleteInterscambioEF(
                                               ByVal macchina As Parco_Macchine,
                                               ByVal codice_esterno As String,
                                               ByVal sistemaCod As Integer,
                                               ByRef GiasContext As Gias_DeveloperServer_Entities,
                                               ByVal objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               ByVal objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                               Optional ByVal ScriviLog As Boolean = True
                                               ) As Interscambio_Parco_Macchine

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.EFMacchine.PrepareDeleteInterscambioD2G()"
        Dim interscambio As Interscambio_Parco_Macchine

        Try
            Dim interscambioDAL_W As New AgronicaCoreInterscambioDAL.Interscambio_Parco_Macchine_W
            interscambio = interscambioDAL_W.PrepareDeleteInterscambioEF(
                macchina,
                codice_esterno,
                sistemaCod,
                GiasContext,
                objParametri_Server,
                objParametri_Utenti,
                False
                )
        Catch ex As Exception
            Dim MessaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore, ex)
        End Try

        Return interscambio

    End Function
End Class
