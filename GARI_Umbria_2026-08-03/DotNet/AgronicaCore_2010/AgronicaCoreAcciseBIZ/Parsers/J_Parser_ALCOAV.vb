Imports AgronicaCoreAcciseCommon

Public NotInheritable Class J_Parser_ALCOAV

    Public Shared Iterator Function Parse(ByVal messaggio As String) As IEnumerable(Of J_Parsed)

        Dim tutteLeRighe = messaggio.Split(ControlChars.CrLf.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
        Dim righe = tutteLeRighe.Skip(2).ToList()

        Dim rigaEsito = righe.FirstOrDefault()
        Dim righeErroriSegnalazioni = righe.Where(Function(r) r.StartsWith(" E") OrElse r.StartsWith(" S")).ToList()
        Dim esitoGenerale = String.Join("", righe.FirstOrDefault().ToCharArray().Skip(42).Take(1))

        Yield New J_Parsed With {.Esito = If(esitoGenerale = "0", "P", "N")}

        If Not righeErroriSegnalazioni Is Nothing AndAlso righeErroriSegnalazioni.Any() AndAlso esitoGenerale = "2" Then

            For Each r As String In righeErroriSegnalazioni

                Dim parsed = New J_Parsed With {.Esito = If(esitoGenerale = "0", "P", "N")}

                Dim dettagli = String.Join("", r.ToCharArray().Skip(2)).Split(",")
                If dettagli.Length = 1 Then
                    Continue For
                End If

                Dim tipoErrore = String.Join("", r.ToCharArray().Skip(1).Take(1))
                parsed.ProgressivoRiga = dettagli(0).Split(":")(1)
                parsed.TipoErrore = tipoErrore
                parsed.DescrizioneCampo = dettagli(2).Split(":")(1)
                parsed.NumeroControllo = dettagli(3).Split(":")(1)

                Yield parsed

            Next

        End If

    End Function

End Class
