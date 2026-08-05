Imports AgronicaCoreDataProvider

Public Interface ICheckListExtension
    Function Esegui(ByVal Esecuzione_cod As Integer,
                    ByVal Esecuzione_GUID As String,
                    ByVal ParametriEsecuzione As String,
                    ByVal Intersection As String,
                    ByRef ObjParametri As AgronicaCoreParametri) As Boolean


End Interface
