Imports AgronicaCoreModelsSTD.Gis

Public Class GIS_Utility

    Public Shared Function GetLimiteByTipoOperatoreData(
            ByVal defaultLimite As String,
            ByVal tipoOperatoreData As FiltroTemporale.enum_OperatoreFiltroTemporale
        ) As String

        Dim limite = defaultLimite

        Select Case tipoOperatoreData

            Case FiltroTemporale.enum_OperatoreFiltroTemporale.Precedente
                limite = "<"

            Case FiltroTemporale.enum_OperatoreFiltroTemporale.PrecedenteUguale
                limite = "<="

            Case FiltroTemporale.enum_OperatoreFiltroTemporale.Successivo
                limite = ">"

            Case FiltroTemporale.enum_OperatoreFiltroTemporale.SuccessivoUguale
                limite = ">="

        End Select

        Return limite

    End Function

End Class
