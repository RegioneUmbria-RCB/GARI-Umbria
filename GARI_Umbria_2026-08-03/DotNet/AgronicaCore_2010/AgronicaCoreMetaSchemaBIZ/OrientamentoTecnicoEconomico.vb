Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class OrientamentoTecnicoEconomico
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Leggi(ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.OrientamentoTecnicoEconomico)
        Dim orientamentoList As New List(Of AgronicaCoreModelsSTD.metaschema.OrientamentoTecnicoEconomico)

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R

        Dim DTRs = objCOM.Leggi("", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        orientamentoList = From row As DataRow In DTRs.Rows
                           Select New AgronicaCoreModelsSTD.metaschema.OrientamentoTecnicoEconomico(row("OTE_cod")) With {
                                .descrizione = row("OTE_des")
                           }

        Return orientamentoList
    End Function

    Public Function Leggi(codice As String, ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.OrientamentoTecnicoEconomico

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.OrientamentoTecnicoEconomico_R

        Dim DTRs = objCOM.Leggi("", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)


        If DTRs.Rows.Count = 0 Then
            Return Nothing
        End If

        Dim orientamento = New AgronicaCoreModelsSTD.metaschema.OrientamentoTecnicoEconomico(codice) With {.descrizione = DTRs.Rows(0)("OTE_des")}

        Return orientamento
    End Function

End Class
