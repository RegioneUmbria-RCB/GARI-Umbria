Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.metaschema

Public Class MisureXIndiciMaturita

    Public Function LeggiMisureIndiciMaturita(ByVal ind_mat_cod As Integer, ByRef objParametri As AgronicaCoreParametri) As List(Of MisuraPerIndiciMaturita)

        Dim result As New List(Of MisuraPerIndiciMaturita)
        Dim xRead As New AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_R

        Dim dt = xRead.LeggiMisureIndiciMaturita(ind_mat_cod, objParametri)

        For Each row In dt.Rows
            Dim misura As New MisuraPerIndiciMaturita
            misura.Codice = $"{row("Ind_Mat_Cod")}_{row("Udm_Cod")}"
            misura.Descrizione = $"{row("IND_MAT_DES")} ({row("UDM_DES")})"

            result.Add(misura)
        Next

        Return result

    End Function
End Class
