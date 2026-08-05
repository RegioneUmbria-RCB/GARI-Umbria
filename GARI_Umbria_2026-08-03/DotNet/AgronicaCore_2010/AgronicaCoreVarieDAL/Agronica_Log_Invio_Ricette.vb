Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Agronica_Log_Invio_Ricette_W

    Public Function Create_Agronica_Log_Invio_Ricette(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                                      ID_Ricetta As Integer,
                                                      ID_Ricetta_Operazione As Integer,
                                                      ID_Ricetta_Operazione_Esterna As Integer,
                                                      ID_Log_Invio As Integer,
                                                      ObjParametri_Server As AgronicaCoreParametri,
                                                      GiasContext As Gias_DeveloperServer_Entities) As Agronica_Log_Invio_Ricette

        Dim idGen As New Agro_Sequenze
        Dim logInvioRicette As New Agronica_Log_Invio_Ricette

        logInvioRicette.Tipo_Esportazione = Tipo_Esportazione

        logInvioRicette.datainvio = DateTime.Now
        logInvioRicette.Data_Creazione = DateTime.Now
        logInvioRicette.Data_Modifica = DateTime.Now

        logInvioRicette.ID_Ricetta = ID_Ricetta
        logInvioRicette.ID_Ricetta_Operazione = ID_Ricetta_Operazione
        logInvioRicette.ID_Ricetta_Operazione_Esterna = ID_Ricetta_Operazione_Esterna

        logInvioRicette.ID_Log_Invio = ID_Log_Invio

        logInvioRicette.inviato = 0
        logInvioRicette.Username_Creazione = ObjParametri_Server.UtenteCodFiscale
        logInvioRicette.Username_Modifica = ObjParametri_Server.UtenteCodFiscale

        logInvioRicette.Validita_Inizio = AGRODATAINIZIO
        logInvioRicette.Validita_Fine = AGRODATAFINE

        GiasContext.Agronica_Log_Invio_Ricette.Add(logInvioRicette)
        GiasContext.SaveChanges()

        Return logInvioRicette

    End Function

End Class

Public Class Agronica_Log_Invio_Ricette_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiLog_JoinChiamate(ByVal ID_Log_Invio As Integer,
                                          ByVal listaRicette As List(Of Integer),
                                          ByVal listaRicetteOperazioni As List(Of Integer),
                                          ByVal Validita_Inizio As String,
                                          ByVal Validita_Fine As String,
                                          ByVal Dettaglio1 As String,
                                          ByVal Dettaglio2 As String,
                                          ByVal Dettaglio3 As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Ricette_R.Leggi_JoinChiamate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Agronica_Log_Invio_Ricette LogRicette ")

            StrSQL.AppendLine(" LEFT JOIN Agronica_Log_Invio_Chiamate LogChiamate")
            StrSQL.AppendLine(" ON LogRicette.ID_Log_Invio = LogChiamate.ID")


            StrSQL.AppendLine(" WHERE 1=1 ")

            If Validita_Inizio <> "" Then
                StrSQL.AppendLine(" AND LogRicette.Data_Creazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Validita_Fine <> "" Then
                StrSQL.AppendLine(" AND LogRicette.Data_Creazione >= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If ID_Log_Invio <> 0 Then
                StrSQL.AppendLine(" AND LogRicette.ID_Log_Invio = " & Agro_SQL_SaveNum(ID_Log_Invio) & "   ")
            End If

            If listaRicette IsNot Nothing Then
                StrSQL.AppendLine(" AND LogRicette.ID_Ricetta IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", listaRicette.ToArray()), False) & ")   ")
            End If

            If listaRicetteOperazioni IsNot Nothing Then
                StrSQL.AppendLine(" AND LogRicette.ID_Ricetta_Operazione IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", listaRicetteOperazioni.ToArray()), False) & ")   ")
            End If

            If Dettaglio1 <> "" Then
                StrSQL.AppendLine(" AND LogChiamate.Dettaglio1 = '" & Agro_SQL_SaveText(Dettaglio1) & "'   ")
            End If

            If Dettaglio2 <> "" Then
                StrSQL.AppendLine(" AND LogChiamate.Dettaglio2 = '" & Agro_SQL_SaveText(Dettaglio2) & "'   ")
            End If

            If Dettaglio3 <> "" Then
                StrSQL.AppendLine(" AND LogChiamate.Dettaglio3 = '" & Agro_SQL_SaveText(Dettaglio3) & "'   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & "   ")
            End If


            StrSQL.AppendLine(" ORDER BY LogChiamate.ID ASC")

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

    Public Function Leggi(ByVal ID_Log_Invio As Integer,
                          ByVal ID_Ricetta As Integer,
                          ByVal ID_Ricetta_Operazione As Integer,
                          ByVal Validita_Inizio As String,
                          ByVal Validita_Fine As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Ricette_R.Leggi_JoinChiamate()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Agronica_Log_Invio_Ricette LogRicette ")

            StrSQL.AppendLine(" WHERE 1=1 ")

            If Validita_Inizio <> "" Then
                StrSQL.AppendLine(" AND LogRicette.Data_Creazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Validita_Fine <> "" Then
                StrSQL.AppendLine(" AND LogRicette.Data_Creazione >= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If ID_Log_Invio <> 0 Then
                StrSQL.AppendLine(" AND LogRicette.ID_Log_Invio = " & Agro_SQL_SaveNum(ID_Log_Invio) & "   ")
            End If

            If ID_Ricetta <> 0 Then
                StrSQL.AppendLine(" AND LogRicette.ID_Ricetta = " & Agro_SQL_SaveNum_NULL(ID_Ricetta) & "   ")
            End If

            If ID_Ricetta_Operazione <> 0 Then
                StrSQL.AppendLine(" AND LogRicette.ID_Ricetta_Operazione  = " & Agro_SQL_SaveNum_NULL(ID_Ricetta_Operazione) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & "   ")
            End If


            StrSQL.AppendLine(" ORDER BY LogRicette.ID_Log_Invio ASC")

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

End Class