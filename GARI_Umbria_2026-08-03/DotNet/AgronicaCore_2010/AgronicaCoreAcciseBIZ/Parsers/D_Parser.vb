Imports AgronicaCoreAcciseCommon

Public NotInheritable Class D_Parser

    Public Shared Iterator Function Parse(ByVal messaggio As String) As IEnumerable(Of D_Parsed)

        Dim tutteLeRighe = messaggio.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        Dim righe = tutteLeRighe.Skip(1).Take(tutteLeRighe.Count() - 2).ToList()

        For Each r As String In righe

            Dim arrayData = r.ToCharArray()
            Dim nRiga = String.Join("", arrayData.Take(6)).Trim()
            Dim eData = String.Join("", arrayData.Skip(6)).Split("-")

            For Each e As String In eData

                Yield New D_Parsed With
                {
                    .Riga = nRiga,
                    .Campo = String.Join("", e.Take(8)).Trim(),
                    .CodiceErrore = String.Join("", e.Skip(10).Take(2)).Trim()
                }
            Next

        Next

    End Function

End Class
