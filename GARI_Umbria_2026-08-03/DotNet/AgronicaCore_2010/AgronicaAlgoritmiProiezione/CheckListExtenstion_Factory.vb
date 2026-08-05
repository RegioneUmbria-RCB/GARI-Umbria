Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CheckListExtenstion_Factory

    Public Function GetExtensionDaCodice(ByVal CheckListType_Cod As Int32,
                                         ByVal Parametri_Addizionali As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri) As ICheckListExtension

        Select Case CheckListType_Cod
            Case enum_GIS_CheckList_Type.ISCC
                Return New ComplianceISCC_Ext()
            Case Else
                Throw New Exception("CheckList non riconosciuta.")
        End Select
    End Function

End Class
