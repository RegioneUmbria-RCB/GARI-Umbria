Public Class Input_Intersezione_GEE
    Implements InputGEE

    Protected input As DataRow
    Protected Parametri_addizionali As Dictionary(Of String, Object)

    Public Overloads Sub SetupParametriOpzionali(ByVal input As DataRow,
                                                  ByVal Parametri_addizionali As Dictionary(Of String, Object))
        Me.input = input
        Me.Parametri_addizionali = Parametri_addizionali
    End Sub

    Public Overloads Sub SetupParametriOpzionali() Implements InputGEE.SetupParametriOpzionali
        Throw New NotImplementedException()
    End Sub

    Public Function CreaParametriElementoGrafico() As Dictionary(Of Int32, String)

        If Parametri_addizionali Is Nothing Then
            Throw New Exception("E' necessario specificare la Descrizione Appezzamento come parametro addizionale.")
        End If

        Dim output As New Dictionary(Of Int32, String)

        Dim Elementi_Des = input("ElementoGrafico_Des").ToString.Split("|")
        Dim nomeToken = Elementi_Des.Where(Function(s) s.StartsWith(input("LayerElementiGrafici_Etichetta").ToString)).ToList

        If nomeToken.Count <> 1 Then
            Throw New Exception("Impossibile trovare il nome del elemento grafico.")
        End If

        output.Add(1, nomeToken.FirstOrDefault.Replace(String.Format("{0}§ ", input("LayerElementiGrafici_Etichetta").ToString), "").Trim)
        output.Add(2, Parametri_addizionali("Descrizione").ToString)
        output.Add(3, input("PercentualeIntersezione").ToString)

        Return output

    End Function

End Class
