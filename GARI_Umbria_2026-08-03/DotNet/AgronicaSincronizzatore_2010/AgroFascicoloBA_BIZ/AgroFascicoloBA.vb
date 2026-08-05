Imports SincroAnagrafeBA

Public Interface AgroFascicoloBA


    Function GetSingoloFascicoloAgea( _
            ByVal EnteValidatore_COD As Integer, _
            ByVal cuaa As String, _
            ByVal Validazione_Numero As String, _
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    ) As Fascicolo

End Interface
