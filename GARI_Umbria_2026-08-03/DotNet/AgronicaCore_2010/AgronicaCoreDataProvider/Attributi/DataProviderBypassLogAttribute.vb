
<System.AttributeUsage(System.AttributeTargets.Class Or
                       System.AttributeTargets.Method,
                       AllowMultiple:=False)>
Public Class DataProviderBypassLogAttribute
    Inherits System.Attribute

    Sub New()
        MyBase.New
    End Sub

End Class
