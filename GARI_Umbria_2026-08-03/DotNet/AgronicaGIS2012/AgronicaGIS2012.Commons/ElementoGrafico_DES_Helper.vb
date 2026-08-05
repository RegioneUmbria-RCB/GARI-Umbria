

Imports System.Text

Public Class ElementoGrafico_DES_Helper

    Public Shared Sub QRY_GetFrom_ElementoGraficoDES_Piped(ByVal ElementoGrafico_DaEstrarre As String, ByVal ElementoGraficoSuccessivoAQuelloDaEstrarre As String, ByRef stb As StringBuilder, Optional ByVal NomeColonna As String = "ElementoGrafico_DES", Optional NomeColonnaAlias As String = "")

        Dim AliasC As String = NomeColonna
        If NomeColonnaAlias <> "" Then
            AliasC = NomeColonnaAlias
        End If


        If Not String.IsNullOrEmpty(ElementoGraficoSuccessivoAQuelloDaEstrarre) Then
            stb.Append("    replace( " & vbCrLf)
            stb.Append("    substring( " & vbCrLf)
            stb.Append("        " & NomeColonna & ", " & vbCrLf)
            stb.Append("        charindex( " & vbCrLf)
            stb.Append("              '" & ElementoGrafico_DaEstrarre & "§ ' " & vbCrLf)
            stb.Append("            , " & NomeColonna & vbCrLf)
            stb.Append("            , 0 " & vbCrLf)
            stb.Append("        ) " & vbCrLf)
            stb.Append("        ,  " & vbCrLf)
            stb.Append("        charindex( " & vbCrLf)
            stb.Append("              '" & ElementoGraficoSuccessivoAQuelloDaEstrarre & "§ ' " & vbCrLf)
            stb.Append("            , " & NomeColonna & vbCrLf)
            stb.Append("            , 0 " & vbCrLf)
            stb.Append("        ) -  " & vbCrLf)
            stb.Append("            charindex( " & vbCrLf)
            stb.Append("              '" & ElementoGrafico_DaEstrarre & "§ ' " & vbCrLf)
            stb.Append("            , " & NomeColonna & vbCrLf)
            stb.Append("            , 0 " & vbCrLf)
            stb.Append("        ) - 1 " & vbCrLf)
            stb.Append("    ) " & vbCrLf)
            stb.Append(" , '" & ElementoGrafico_DaEstrarre & "§ ' " & vbCrLf)
            stb.Append(" , '' " & vbCrLf)
            stb.Append(" ) as " & NomeColonnaAlias & "")

        Else
            stb.Append(" SUBSTRING ( " & vbCrLf)
            stb.Append("    " & NomeColonna & " " & vbCrLf)
            stb.Append("    , charindex ( " & vbCrLf)
            stb.Append("         '" & ElementoGrafico_DaEstrarre & "§ ' " & vbCrLf)
            stb.Append("    , " & NomeColonna & " " & vbCrLf)
            stb.Append("    , 0 " & vbCrLf)
            stb.Append(" ) + len('" & ElementoGrafico_DaEstrarre & "§ ') + 1 " & vbCrLf)
            stb.Append(" ,  " & vbCrLf)
            stb.Append(" len(" & NomeColonna & ") - " & vbCrLf)
            stb.Append(" charindex ( " & vbCrLf)
            stb.Append("    '" & ElementoGrafico_DaEstrarre & "§ ' " & vbCrLf)
            stb.Append("    , " & NomeColonna & " " & vbCrLf)
            stb.Append("    , 0 " & vbCrLf)
            stb.Append(" ) - len('" & ElementoGrafico_DaEstrarre & "§ ') " & vbCrLf)
            stb.Append(" ) as " & AliasC & vbCrLf)

        End If








    End Sub

End Class
