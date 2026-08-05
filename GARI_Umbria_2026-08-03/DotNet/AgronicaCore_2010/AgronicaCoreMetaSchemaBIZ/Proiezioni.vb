Imports AgronicaCoreModelsSTD.Gis

Public Class Proiezioni_R
    Public Function LeggiElencoAlgoritmi(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As ElencoAlgoritmi

        Dim elencoAlgoritmi As New ElencoAlgoritmi With {
            .Algoritmi = New List(Of AlgoritmoProiezione)
        }

        Dim xRead As New AgronicaCoreMetaSchemaDAL.Proiezioni_R

        Dim DT As DataTable

        DT = xRead.LeggiElencoAlgoritmi(objParametri)

        If DT Is Nothing Then
            Throw New Exception("Errore nella lettura degli algoritmi")
        End If

        Dim AlgoritmoCorrente As Int32 = 0
        Dim algoritmo As AlgoritmoProiezione = Nothing

        For Each row In DT.Rows
            If CInt(row("LayerAnalysisConfig_Algorithm_Cod")) <> AlgoritmoCorrente Then

                If AlgoritmoCorrente <> 0 Then
                    elencoAlgoritmi.Algoritmi.Add(algoritmo)
                End If

                AlgoritmoCorrente = CInt(row("LayerAnalysisConfig_Algorithm_Cod"))

                algoritmo = New AlgoritmoProiezione With {
                    .Algoritmo_Cod = AlgoritmoCorrente,
                    .Algoritmo_Des = row("LayerAnalysisConfig_Algorithm_Des").ToString,
                    .Parametri = New List(Of ParametriAlgoritmoProiezione)
                }
            End If

            Dim param As New ParametriAlgoritmoProiezione With {
                .Parametro_Cod = CInt(row("LayerAnalysisConfig_Algorithm_Param_Cod")),
                .Parametro_Des = row("LayerAnalysisConfig_Algorithm_Param_Des").ToString,
                .tipo_Parametro = CInt(row("LayerXConfig_Cod"))
            }

            algoritmo.Parametri.Add(param)
        Next

        If algoritmo IsNot Nothing Then
            elencoAlgoritmi.Algoritmi.Add(algoritmo)
        End If

        Return elencoAlgoritmi
    End Function
End Class
Public Class Proiezioni_W

End Class
