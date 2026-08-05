Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class CAC_Codifica_Dati_SistemiEsterni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ID As Integer,
                          Sistema_Cod As Integer,
                          Codice_Esterno As String,
                          Descrizione_Esterno As String,
                          Tabella_Gias As String,
                          Codice_Gias As String,
                          Validita_Inizio As Date,
                          Validita_Fine As Date,
                              xFiltroAggiuntivo As String,
                              xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.CAC_Codifica_Dati_SistemiEsterni_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  CAC_Codifica_Dati_SistemiEsterni ")
            StrSQL.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

            If ID <> 0 Then
                StrSQL.AppendLine(" AND ID  = " & Agro_SQL_SaveNum(ID) & "   " + vbCrLf)
            End If

            If Sistema_Cod <> 0 Then
                StrSQL.AppendLine(" AND Sistema_Cod  = " & Agro_SQL_SaveNum(Sistema_Cod) & "   " + vbCrLf)
            End If

            If Codice_Esterno <> "" Then
                StrSQL.AppendLine(" AND Codice_Esterno  = '" & Agro_SQL_SaveText(Codice_Esterno) & "'   " + vbCrLf)
            End If

            If Descrizione_Esterno <> "" Then
                StrSQL.AppendLine(" AND Descrizione_Esterno  = '" & Agro_SQL_SaveText(Descrizione_Esterno) & "'   " + vbCrLf)
            End If

            If Tabella_Gias <> "" Then
                StrSQL.AppendLine(" AND Tabella_Gias  = '" & Agro_SQL_SaveText(Tabella_Gias) & "'   " + vbCrLf)
            End If

            If Codice_Gias <> "" Then
                StrSQL.AppendLine(" AND Codice_Gias  = '" & Agro_SQL_SaveText(Codice_Gias) & "'   " + vbCrLf)
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Descrizione_Esterno ASC")
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

    Public Function GetCodiceEsterno_from_CodiceGias(ByVal codiceGias As String,
                                                     ByVal sistemaCod As enum_SistemiEsterni,
                                                     ByVal tabellaGias As String,
                                                     ByRef objParametri As AgronicaCoreParametri,
                                                     Optional ByVal validitaInizio As Date = AGRODATAINIZIO,
                                                     Optional ByVal validitaFine As Date = AGRODATAFINE,
                                                     Optional ByVal xFiltroAggiuntivo As String = "",
                                                     Optional ByVal xOrderBy As String = "") As String
        Const NomeRoutine As String = "AgronicaCoreMetaschemaDAL.CAC_Codifica_Dati_SistemiEsterni_R.GetCodiceEsterno_from_CodiceGias()"

        Dim MessaggioErrore As String
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim codiceEsterno As String = ""

        Try

            dt = Leggi(0, sistemaCod, "", "", tabellaGias, codiceGias,
                       validitaInizio, validitaFine, xFiltroAggiuntivo, xOrderBy, objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                codiceEsterno = dt(0)("Codice_Esterno")
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return codiceEsterno

    End Function

End Class
