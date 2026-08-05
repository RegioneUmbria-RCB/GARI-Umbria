Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Codice As String, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166 " + vbCrLf)
            StrSQL.Append(" WHERE 1 = 1 " & vbCrLf)

            If Codice <> "" Then

                If UCase(Left(Codice & " ", 3)) = "ITA" Then
                    Codice = "IT"
                End If

                StrSQL.Append(" AND Codice = '" & Agro_SQL_SaveText(Codice) + "'" + vbCrLf)

            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) + vbCrLf)
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Inviato >=0 " + vbCrLf)
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Inviato =-1 " + vbCrLf)
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '=====================================================================


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


    '###################################################################################
    Public Function Stato_In_PaesiterziISO3166(ByVal Stato As String, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R.Stato_In_PaesiterziISO3166()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Esiste As Boolean = False

        Try

            Stato = Left(Stato, 2)

            DT = Leggi(Stato, _
                         "", "", _
                        objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Esiste = True
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return Esiste

    End Function

    '###################################################################################
    Public Function Codice_Entrate_Unico_da_Codice(ByVal Stato As String, _
                                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                   ) As Integer

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R.Codice_Entrate_Unico_da_Codice()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Codice_Entrate As Integer = 0

        Try

            Stato = Left(Stato, 2)

            DT = Leggi(Stato, _
                       "", "", _
                       objParametri)

            If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then
                Codice_Entrate = DT.Rows(0).Item("Entrate_Unico_Elenco_paesi_territori_esteri_COD")
            End If

            DT = Nothing

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
            Return 0
        End Try

        Return Codice_Entrate

    End Function


End Class
