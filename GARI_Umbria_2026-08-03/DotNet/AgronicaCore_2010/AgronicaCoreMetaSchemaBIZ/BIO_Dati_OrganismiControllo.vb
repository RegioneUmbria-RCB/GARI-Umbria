Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class BIO_Dati_OrganismiControllo
    Inherits AgronicaCoreDataProvider.LogProvider

    Public Function Leggi(ByRef objParametri_Server As AgronicaCoreParametri) As List(Of AgronicaCoreModelsSTD.metaschema.BioOrganismoDiControllo)
        Dim organismoList As New List(Of AgronicaCoreModelsSTD.metaschema.BioOrganismoDiControllo)

        Dim objCOM As New AgronicaCoreMetaSchemaDAL.BIO_Dati_OrganismiControllo_R

        Dim DTRs = objCOM.Leggi(0, "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        organismoList = From row As DataRow In DTRs.Rows
                        Select New AgronicaCoreModelsSTD.metaschema.BioOrganismoDiControllo(row("Orientamento_Cod")) With {
                            .descrizione = row("Orientamento_Des")
                        }

        Return organismoList
    End Function

End Class
