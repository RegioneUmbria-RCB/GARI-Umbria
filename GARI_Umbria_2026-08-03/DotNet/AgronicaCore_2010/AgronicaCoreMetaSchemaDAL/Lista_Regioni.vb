Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Lista_Regioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'creata la tabella nella matrice in data 17/08/2012
    Public Function Leggi(ByVal REG As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Stato_Country As String = "IT"
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Regioni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As New DataTable

        Try

            'la tabella non esiste nel metaschema
            'ci portiamo avanti con il DAL
            'quando esisterà la query, Ordinamento_Alfabetico non servirà più
            'Dim Dr As DataRow
            'DT.Columns.Add(New DataColumn("Codice", GetType(String)))
            'DT.Columns.Add(New DataColumn("Descrizione", GetType(String)))

            'Select Case Ordinamento_Alfabetico

            '    Case False

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "001"
            '        Dr.Item("Descrizione") = "Piemonte"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "002"
            '        Dr.Item("Descrizione") = "Valle d'Aosta"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "003"
            '        Dr.Item("Descrizione") = "Lombardia"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "004"
            '        Dr.Item("Descrizione") = "Trentino Alto Adige"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "005"
            '        Dr.Item("Descrizione") = "Veneto"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "006"
            '        Dr.Item("Descrizione") = "Friuli Venezia Giulia"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "007"
            '        Dr.Item("Descrizione") = "Liguria"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "008"
            '        Dr.Item("Descrizione") = "Emilia Romagna"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "009"
            '        Dr.Item("Descrizione") = "Toscana"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "010"
            '        Dr.Item("Descrizione") = "Umbria"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "011"
            '        Dr.Item("Descrizione") = "Marche"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "012"
            '        Dr.Item("Descrizione") = "Lazio"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "013"
            '        Dr.Item("Descrizione") = "Abruzzo"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "014"
            '        Dr.Item("Descrizione") = "Molise"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "015"
            '        Dr.Item("Descrizione") = "Campania"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "016"
            '        Dr.Item("Descrizione") = "Puglia"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "017"
            '        Dr.Item("Descrizione") = "Basilicata"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "018"
            '        Dr.Item("Descrizione") = "Calabria"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "019"
            '        Dr.Item("Descrizione") = "Sicilia"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "020"
            '        Dr.Item("Descrizione") = "Sardegna"
            '        DT.Rows.Add(Dr)

            '    Case True

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "013"
            '        Dr.Item("Descrizione") = "Abruzzo"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "017"
            '        Dr.Item("Descrizione") = "Basilicata"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "018"
            '        Dr.Item("Descrizione") = "Calabria"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "015"
            '        Dr.Item("Descrizione") = "Campania"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "008"
            '        Dr.Item("Descrizione") = "Emilia Romagna"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "006"
            '        Dr.Item("Descrizione") = "Friuli Venezia Giulia"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "012"
            '        Dr.Item("Descrizione") = "Lazio"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "007"
            '        Dr.Item("Descrizione") = "Liguria"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "003"
            '        Dr.Item("Descrizione") = "Lombardia"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "011"
            '        Dr.Item("Descrizione") = "Marche"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "014"
            '        Dr.Item("Descrizione") = "Molise"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "001"
            '        Dr.Item("Descrizione") = "Piemonte"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "016"
            '        Dr.Item("Descrizione") = "Puglia"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "020"
            '        Dr.Item("Descrizione") = "Sardegna"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "019"
            '        Dr.Item("Descrizione") = "Sicilia"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "009"
            '        Dr.Item("Descrizione") = "Toscana"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "004"
            '        Dr.Item("Descrizione") = "Trentino Alto Adige"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "010"
            '        Dr.Item("Descrizione") = "Umbria"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "002"
            '        Dr.Item("Descrizione") = "Valle d'Aosta"
            '        DT.Rows.Add(Dr)

            '        Dr = DT.NewRow
            '        Dr.Item("Codice") = "005"
            '        Dr.Item("Descrizione") = "Veneto"
            '        DT.Rows.Add(Dr)

            'End Select

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Lista_Regioni ")
            StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If REG <> "" Then
                StrSQL.Append(" AND REG = '" & Agro_SQL_SaveText(Trim(REG)) & "' ")
            End If

            If Stato_Country <> "" Then

                If UCase(Left(Stato_Country & " ", 3)) = "ITA" Then
                    Stato_Country = "IT"
                End If

                StrSQL.Append(" AND Stato_Country = '" & Agro_SQL_SaveText(Trim(Stato_Country)) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Regione_Des ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '##############################################################################################
    Public Function RegioneDes_from_REG(ByVal REG As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Regioni_R.RegioneDes_from_REG()"
        Dim MessaggioErrore As String = ""
        Dim Regione_Des As String = ""

        Try

            Dim dt As DataTable

            dt = Leggi(REG, "", "", objParametri)

            If Not IsNothing(dt) Then
                If dt.Rows.Count > 0 Then
                    Regione_Des = dt.Rows(0).Item("Regione_Des")
                End If

            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Regione_Des

    End Function

    Public Function ReadByProvince(
                                  ByVal prov As String,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Lista_Regioni_R.ReadByProvince"
        Dim MessaggioErrore As String = ""
        Dim dt As DataTable

        Try
            Dim StrSQL As New System.Text.StringBuilder

            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM Lista_Regioni lr")
            StrSQL.AppendLine("    INNER JOIN Lista_Province lp ON lp.REG = lr.REG")
            StrSQL.AppendLine("        AND lp.PROV = '" & Agro_SQL_SaveText(prov) & "'")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt
    End Function


End Class
