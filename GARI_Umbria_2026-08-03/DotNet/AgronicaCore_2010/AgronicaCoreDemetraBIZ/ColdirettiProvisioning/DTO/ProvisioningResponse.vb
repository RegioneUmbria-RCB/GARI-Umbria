Public Class ProvisioningResponseError
    Public errors As List(Of ErrorObject)
End Class

Public Class ErrorObject
    Public status As Integer
    Public detail As String
End Class

Public Class ProvisioningResponse
    Public data As List(Of DataObject)
End Class

Public Class DataObject
    Public type As String
    Public id As Integer
    Public service_id As Integer
    Public company_id As Integer
    Public service_subscription_status_id As Integer
    Public file_guid As String
    Public created_at As DateTime
    Public updated_at As DateTime
    Public deleted_at As DateTime
End Class
