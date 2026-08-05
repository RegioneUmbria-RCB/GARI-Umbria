Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class TipologiaSede
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Leggi(ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.TipologiaSede)
        Dim TipologiaList As New List(Of AgronicaCoreModelsSTD.metaschema.TipologiaSede)

        Dim objCOM As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        Dim DTRs = objCOM.Leggi(0, "",
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "creatore = 'CSA' and gruppo = 'TIPO_CA'",
                            "",
                            objParametri_Server)

        TipologiaList = From row As DataRow In DTRs.Rows
                        Select New AgronicaCoreModelsSTD.metaschema.TipologiaSede(CInt(row("codice"))) With {
                            .descrizione = row("descrizione")
                        }

        Return TipologiaList
    End Function

    Public Function Leggi(codice As String, ByRef objParametri_Server As AgronicaCoreParametri) As AgronicaCoreModelsSTD.metaschema.TipologiaSede

        Dim objCOM As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

        Dim DTRs = objCOM.Leggi(codice, "",
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            "creatore = 'CSA' and gruppo = 'TIPO_CA'",
                            "",
                            objParametri_Server)

        If DTRs.Rows.Count = 0 Then
            Return Nothing
        End If

        Dim tipologiaSede = New AgronicaCoreModelsSTD.metaschema.TipologiaSede(codice) With {.descrizione = DTRs.Rows(0)("descrizione")}

        Return tipologiaSede
    End Function

End Class
