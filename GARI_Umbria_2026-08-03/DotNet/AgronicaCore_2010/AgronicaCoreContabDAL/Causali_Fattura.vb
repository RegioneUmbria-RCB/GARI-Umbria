Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Causali_Fattura_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal piva As String,
                          ByVal codice As Integer,
                          ByVal categoria As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Causali_Fattura_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Causali_Fattura ")
            'stb.AppendLine(" WHERE (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' OR Piva_SuperUser = 'AAAAAAAAAAA') ")
            stb.AppendLine(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            
            If piva <> "" Then
                stb.AppendLine(" AND (Piva = '" & Agro_SQL_SaveText(piva) & "' OR Piva = 'AAAAAAAAAAA') ")
            End If

            If codice <> 0 Then
                stb.AppendLine(" AND Codice = " & Agro_SQL_SaveNum(codice) & " ")
            End If

            If categoria <> "" Then
                stb.AppendLine(" AND Categoria = '" & Agro_SQL_SaveText(categoria) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    stb.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    stb.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" ORDER BY Causali_Fattura.Descrizione ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class


'#################################################################
'#################################################################
'#################################################################
