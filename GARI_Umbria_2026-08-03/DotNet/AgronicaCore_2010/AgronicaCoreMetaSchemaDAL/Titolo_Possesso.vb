Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Titolo_Possesso_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function TitoloPossessoDes_from_TitoloPossessoCod( _
                            ByVal TitoloPossessoCod As Integer _
                            ) As String

        Dim Des As String

        Select Case TitoloPossessoCod

            Case enum_TitoloPossesso.Altro
                Des = "Altro"
            Case enum_TitoloPossesso.Proprieta
                Des = "Proprietà"
            Case enum_TitoloPossesso.Comodato
                Des = "Comodato d'uso"
            Case enum_TitoloPossesso.AffittoContratto
                Des = "Affitto con contratto"
            Case enum_TitoloPossesso.AffittoSenzaContratto
                Des = "Affitto senza contratto"
            Case enum_TitoloPossesso.InContoTerzi
                Des = "In conto terzi"
            Case enum_TitoloPossesso.InConvenzione
                Des = "In convenzione"
            Case enum_TitoloPossesso.InCompartecipazione
                Des = "In compartecipazione"
            Case Else
                Des = "Altro (non definito)"
        End Select

        'Restituisco il risultato
        Return Des
    End Function

End Class
