Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.OutData.NewAgri.NewAgri
Imports AgronicaCoreDTOStd.InData.NewAgri
Imports AgronicaCoreDTOStd


Public Class NewAgri

    Public Function N_Distribuito(cuaa As String,
                                  NewAgri As AgronicaCoreDTOStd.InData.NewAgri.,
                                  ByRef errorMessage As String,
                                  objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                  user_Agent As String) As List(Of AgronicaCoreDTOStd.OutData.NewAgri.NewAgri)

        Try
            Dim xImpCodR As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim piva = xImpCodR.Piva_from_CUAA(cuaa, objParametri_Server)

            If piva = "" Then
                'errorMessage = String.Format(My.Resources.AgronicaCoreDemetraBIZ.CuaaNonEsistente, cuaa)
                Exit Function
            End If

            Dim regolamento_cod As String = NewAgri.Regolamento_Cod
            Dim appezzamenti As List(Of AgronicaCoreDTOStd.InData.NewAgri.Appezzamento) = NewAgri.ElencoAppezzamenti

        Catch ex As Exception
            errorMessage = ex.Message
        End Try

    End Function

End Class
