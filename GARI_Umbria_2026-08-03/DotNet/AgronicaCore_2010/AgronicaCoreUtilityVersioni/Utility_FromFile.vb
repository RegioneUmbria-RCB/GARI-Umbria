Imports System.IO

Public Class Utility_FromFile

#Region "Leggo Da File"

    Public Const TERMINAZIONE_FUNZIONE As String = """)"

    Public Shared Function LeggiFileVersione(ByVal pathFileInput As String) As String()
        'Per tutti è già integrata nel Changelog_Agronica_Obj,
        'qui mi servirebbe solo per quei siti che non hanno il versione aspx
        Dim contenutoFileVersione As String() = Nothing
            
        If File.Exists(pathFileInput) Then
            contenutoFileVersione = File.ReadAllLines(pathFileInput)
        End If

        Return contenutoFileVersione
    End Function

#End Region

#Region "Manipolazione stringhe per creazione Changelog"

    Public Shared Function IsTerminazioneFunzione(ByVal input As String) As Boolean
        Return input.EndsWith(TERMINAZIONE_FUNZIONE)
    End Function

    Public Shared Function IsParametroSplittato(ByVal input As String) As Boolean
        Return input.EndsWith("&")
    End Function

    Public Shared Function IsParametroSuccessivo(ByVal input As String) As Boolean
        Return input.EndsWith(",")
    End Function


    Public Shared Function RemoveDelimiter(ByVal item As String,
                                           ByVal delimiter As String,
                                           Optional ByVal endToRemove As String = ""
                                           ) As String

        Dim withoutStart As String = Trim(item.Remove(0, delimiter.Length))

        If endToRemove <> "" AndAlso item.EndsWith(endToRemove) Then
            withoutStart = withoutStart.Remove(withoutStart.Length-endToRemove.Length, endToRemove.Length)
        End If

        Return withoutStart
    End Function

    Public Shared Function TogliVirgoletteIniziali(ByVal input As String) As String
        'Dim totlaVirgoletta As String = input.Remove(0, ("""").Length)
        Dim totlaVirgoletta As String = input.TrimStart("""")
        Return totlaVirgoletta
    End Function

    Public Shared Function TogliAndFinale(ByVal input As String) As String
        'Dim toltoAnd As String = Trim(input.Remove(input.Length-("&").Length, ("&").Length))
        Dim toltoAnd As String = Trim(input.TrimEnd("&"))
        Dim totlaVirgoletta As String = toltoAnd.Remove(toltoAnd.Length-("""").Length, ("""").Length)
        Return totlaVirgoletta
    End Function

    Public Shared Function TogliTerminazioneFinale(ByVal input As String) As String
        Dim toltaVirgolettaParentesi As String = Trim(input.Remove(input.Length-TERMINAZIONE_FUNZIONE.Length, TERMINAZIONE_FUNZIONE.Length))
        Return toltaVirgolettaParentesi
    End Function

#End Region

End Class
