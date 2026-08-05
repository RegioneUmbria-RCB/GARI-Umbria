Imports AgronicaCoreDataProvider

Public Class CategoriexUnitaMisura

    Public Function Leggi_CategoriaxUnitaMisura(ByVal Elem_Cod As Integer,
                                                ByVal objParametri_Server As AgronicaCoreParametri,
                                                ByVal objParametri_Utenti As AgronicaCoreParametri)
        Dim objCatUDM As New AgronicaCoreMetaSchemaDAL.CategorieXUnitaMisura_R

        Dim xFiltroAggiuntivo = " Elem_Cod > 0 "

        Dim DT_Categorie As DataTable = objCatUDM.Leggi_con_UnitaMisura(Elem_Cod, xFiltroAggiuntivo, objParametri_Server)

        Dim listItems = (From row In DT_Categorie.Rows
                         Select (New With {
                             .Elem_Cod = row.item("Elem_Cod"),
                             .UDM_Cod = row.item("UDM_COD"),
                             .UDM_Des = row.item("UDM_DES")
                         })).ToList()

        Return listItems
    End Function

End Class
