Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text

Public Class PDC_Mappature_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' Legge il contenuto della tabella PDC_Mappature
    ''' </summary>
    ''' <param name="codRisUm">Cod_Risum del laboratorio [le mappature possono essere diverse a seconda del laboratorio] (opzionale = 0)</param>
    ''' <param name="idParametro">(opzionale = 0)</param>
    ''' <param name="tipo">Tipo parametro: utilizzare le costanti PDC_MAPPATURA_* (opzionale = "")</param>
    ''' <param name="codice_GIAS">Codice del parametro in GIAS (opzionale = "")</param>
    ''' <param name="codice_ALTRO">Codice del parametro in un altro programma (opzionale = "")</param>
    ''' <param name="xFiltroAggiuntivo">Filtro aggiuntivo per la query di select (opzionale = "")</param>
    ''' <param name="xOrderBy">Criteri di ordinamento (opzionale = "")</param>
    ''' <param name="objParametri">AgronicaCoreParametri per la connessione</param>
    ''' <param name="dataValidita">restituisce solo i valori validi a quella data</param>
    ''' <returns>DataTable contenente i dati della tabella</returns>
    ''' <remarks>Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per PDC_MAPPATURA_*</remarks>
    Public Function Leggi(ByVal codRisUm As Integer,
                          ByVal idParametro As Integer,
                          ByVal tipo As String,
                          ByVal codice_GIAS As String,
                          ByVal codice_ALTRO As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal dataValidita As Date = #2/1/1900#
                          ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   codRisUm = 0
        '   idParametro = 0
        '   tipo = ""
        '   codice_GIAS = ""
        '   codice_ALTRO = ""
        '====================================================================================
        '
        '   Per il tipo è possibile utilizzare le costanti personalizzate che cominciano per PDC_MAPPATURA_*
        '
        '====================================================================================

        Const nomeRoutine = "PDC_Mappature_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            '---------------------------------------------
            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM PDC_Mappature ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If codRisUm <> 0 Then
                stb.AppendLine(" AND Cod_Risum = " & Agro_SQL_SaveNum(codRisUm) & " ")
            End If

            If idParametro <> 0 Then
                stb.AppendLine(" AND ID_PDC_Mappatura = " & Agro_SQL_SaveNum(idParametro) & " ")
            End If

            If tipo <> "" Then
                stb.AppendLine(" AND Tipo = '" & Agro_SQL_SaveText(tipo) & "' ")
            End If

            If codice_GIAS <> "" Then
                stb.AppendLine(" AND Codice_GIAS = '" & Agro_SQL_SaveText(codice_GIAS) & "' ")
            End If

            If codice_ALTRO <> "" Then
                stb.AppendLine(" AND Codice_ALTRO = '" & Agro_SQL_SaveText(codice_ALTRO) & "' ")
            End If

            If dataValidita <> #2/1/1900# Then
                stb.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(dataValidita) & " ")
                stb.AppendLine(" AND Validita_Fine >= " & Agro_SQL_SaveDate(dataValidita) & " ")
            End If


            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    Public Function Leggi_Mappatura_ALTRO_From_GIAS(ByVal codRisUm As Integer,
                                                    ByRef idParam As Integer,
                                                    ByVal tipo As String,
                                                    ByVal codice_GIAS As String,
                                                    ByRef codice_ALTRO As String,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    Optional ByVal dataValidita As Date = #2/1/1900#
                                                    ) As Integer

        Const nomeRoutine = "PDC_Mappature_R.Leggi_Mappatura_ALTRO_From_GIAS()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = Leggi(codRisUm, 0, tipo, codice_GIAS, "", "", "", objParametri, dataValidita)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codice_ALTRO = CStr(If(IsDBNull(dt.Rows(0).Item("Codice_ALTRO")), "", dt.Rows(0).Item("Codice_ALTRO")))
                idParam = CInt(dt.Rows(0).Item("ID_PDC_Mappatura"))
            Else
                numElementi = 0
                codice_ALTRO = ""
                idParam = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codice_ALTRO = ""
            idParam = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

    Public Function Leggi_Mappatura_GIAS_From_ALTRO(ByVal codRisUm As Integer,
                                                    ByRef idParam As Integer,
                                                    ByVal tipo As String,
                                                    ByVal codice_ALTRO As String,
                                                    ByRef codice_GIAS As String,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    Optional ByVal dataValidita As Date = #2/1/1900#
                                                    ) As Integer

        Const nomeRoutine = "PDC_Mappature_R.Leggi_Mappatura_GIAS_From_ALTRO()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim numElementi As Integer = 0

        Try
            dt = Leggi(codRisUm, 0, tipo, "", codice_ALTRO, "", "", objParametri, dataValidita)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                numElementi = dt.Rows.Count
                codice_GIAS = CStr(If(IsDBNull(dt.Rows(0).Item("Codice_GIAS")), "", dt.Rows(0).Item("Codice_GIAS")))
                idParam = CInt(dt.Rows(0).Item("ID_PDC_Mappatura"))
            Else
                numElementi = 0
                codice_GIAS = ""
                idParam = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            codice_GIAS = ""
            idParam = 0
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return numElementi

    End Function

End Class
