Public Class Codici_Anagrafe
    Public Sub New()

    End Sub

    Public Function CreaNuovoCodiceSincroCliente(ByVal SuperUsername As String,
                                                 ByVal RagioneSociale As String,
                                                 ByVal Creatore As String,
                                                 ByRef ObjParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim rCodice As New AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_R
        Dim wCodice As New AgronicaCoreMetaSchemaDAL.Codici_Anagrafe_W

        Dim ret As Boolean = False

        Dim newCode = rCodice.GetNextCodiceSincroCliente(ObjParametri)
        If newCode > 0 Then
            ret = wCodice.CreaNuovoCodiceSincroCliente(newCode, RagioneSociale, SuperUsername, Creatore, ObjParametri)
        End If

        Return ret

    End Function
End Class
