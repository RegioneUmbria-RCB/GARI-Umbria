Imports AgronicaCoreAcciseCommon

Public NotInheritable Class J_Parser

    Public Shared Iterator Function Parse(ByVal messaggio As String) As IEnumerable(Of J_Parsed)

        Dim tutteLeRighe = messaggio.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        Dim righe = tutteLeRighe.Skip(2).Take(tutteLeRighe.Count() - 2).Where(Function(r) Not r.StartsWith("INFO:")).ToList()

        For Each r As String In righe

            Dim arrayData = r.ToCharArray()
            Dim esito = String.Join("", arrayData.Skip(50).Take(1))
            Dim parsed = New J_Parsed With {.Esito = esito}

            If esito = "P" Then
                parsed.ARC = String.Join("", arrayData.Skip(51).Take(21))
                parsed.ProgressivoARC = String.Join("", arrayData.Skip(85).Take(5))
                parsed.Data = String.Join("", arrayData.Skip(72).Take(8))
            Else
                parsed.TipoErrore = String.Join("", arrayData.Skip(113).Take(1))
                parsed.NumeroControllo = String.Join("", arrayData.Skip(114).Take(3))
                parsed.DescrizioneErrore = String.Join("", arrayData.Skip(117).Take(50)).Trim()
            End If

            Yield parsed

        Next


    End Function

End Class
