Imports AgronicaCoreDataProvider

Public Class ProvisioningDSSbiz

    Public Function GestioneProvisioning(o As ProvisioningDSSRequest, ByVal objParametri_Server As AgronicaCoreParametri) As ProvisioningDSSResponse

        Dim res As New ProvisioningDSSResponse

        Try
            Dim scriviModelliAutorizzati As New AgronicaCoreModelliPrevisionaliDAL.ModelliPrevisionaliXpiva_OperazioniAutorizzate_W
            scriviModelliAutorizzati.RiportaModelliDatoPacchettoCommerciale(o.pacchettoCommercialeCod, o.piva_SuperUser, o.piva, o.scadenza, objParametri_Server)

            res.message = "Autorizzazioni generate correttamente. "

        Catch ex As Exception
            res.message = "Errori in fase di autorizzazione pacchetto comm.le: " & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return res

    End Function

End Class
