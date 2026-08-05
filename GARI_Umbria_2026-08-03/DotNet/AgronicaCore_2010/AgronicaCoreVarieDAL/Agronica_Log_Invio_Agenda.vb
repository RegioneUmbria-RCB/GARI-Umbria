Imports AgronicaCoreDataProvider
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Agronica_Log_Invio_Agenda_W

    Public Function Create_Agronica_Log_Invio_Agenda(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                                     Id_Agenda As Integer,
                                                     Id_Operazione_Esterna As Integer,
                                                     Id_log_invio As Integer,
                                                     ObjParametri_Server As AgronicaCoreParametri,
                                                     GiasContext As Gias_DeveloperServer_Entities,
                                                     Optional ByVal idMov As Integer = 0,
                                                     Optional ByVal idMovDett As Integer = 0,
                                                     Optional ByVal idMovDest As Integer = 0,
                                                     Optional ByVal causaleCod As Integer = 0,
                                                     Optional ByVal Piva As String = "",
                                                     Optional ByVal Chiave_Esterna As String = "",
                                                     Optional ByVal Chiave_Gias As String = "") As Agronica_Log_Invio_Agenda

        Dim idGen As New Agro_Sequenze
        Dim logInvioAgenda As New Agronica_Log_Invio_Agenda

        logInvioAgenda.Tipo_Esportazione = Tipo_Esportazione

        logInvioAgenda.Data_Creazione = DateTime.Now
        logInvioAgenda.Data_Modifica = DateTime.Now

        logInvioAgenda.Piva = Piva
        logInvioAgenda.ID_Agenda = Id_Agenda
        logInvioAgenda.ID_Log_Invio = Id_log_invio
        logInvioAgenda.ID_Operazione_Esterna = Id_Operazione_Esterna
        logInvioAgenda.Chiave_Esterna = Chiave_Esterna
        logInvioAgenda.Chiave = Chiave_Gias

        logInvioAgenda.Id_Mov = idMov
        logInvioAgenda.Id_Mov_Det = idMovDett
        logInvioAgenda.Id_Mov_Dest = idMovDest
        logInvioAgenda.Causale_Cod = causaleCod

        logInvioAgenda.inviato = 0
        logInvioAgenda.Username_Creazione = ObjParametri_Server.UtenteCodFiscale
        logInvioAgenda.Username_Modifica = ObjParametri_Server.UtenteCodFiscale

        logInvioAgenda.Validita_Inizio = AGRODATAINIZIO
        logInvioAgenda.Validita_Fine = AGRODATAFINE

        GiasContext.Agronica_Log_Invio_Agenda.Add(logInvioAgenda)
        GiasContext.SaveChanges()

        Return logInvioAgenda

    End Function

End Class

Public Class Agronica_Log_Invio_Agenda_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Leggi_UltimoLogInvio_exportArtea(ByVal listaAgende As List(Of Integer),
                                                     ByVal Dettaglio1 As String,
                                                     ByVal Dettaglio2 As String,
                                                     ByVal Dettaglio3 As String,
                                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                     ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Agenda_R.Leggi_UltimoLogInvio_exportArtea()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" WITH UltimoLog AS ( ")
            StrSQL.AppendLine("     SELECT MAX(ID_Log_Invio) UltimoLogxAgenda, ID_Agenda")
            StrSQL.AppendLine("     FROM Agronica_Log_Invio_Agenda")
            StrSQL.AppendLine("     GROUP BY ID_Agenda")
            StrSQL.AppendLine("     )")

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Agronica_Log_Invio_Agenda LogAgenda ")

            StrSQL.AppendLine(" LEFT JOIN Agronica_Log_Invio_Chiamate LogChiamate")
            StrSQL.AppendLine(" ON LogAgenda.ID_Log_Invio = LogChiamate.ID")

            StrSQL.AppendLine(" INNER JOIN UltimoLog ")
            StrSQL.AppendLine(" ON UltimoLog.ID_Agenda = LogAgenda.ID_Agenda ")

            StrSQL.AppendLine(" WHERE UltimoLog.UltimoLogxAgenda = LogAgenda.ID_Log_Invio ")
            StrSQL.AppendLine(" AND LogChiamate.Esito = 1 ")

            If listaAgende IsNot Nothing AndAlso listaAgende.Count > 0 Then
                StrSQL.AppendLine(" AND LogAgenda.ID_Agenda IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", listaAgende.ToArray()), False) & ")   ")
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


            StrSQL.AppendLine(" ORDER BY LogAgenda.ID_Agenda desc")

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

    Public Function Leggi_IDTecnicoIN(Dettaglio1 As String,
                                      Dettaglio2 As String,
                                      Dettaglio3 As String,
                                      xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                      ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Agenda_R.Leggi_IDTecnicoIN()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT TOP(1) * ")
            StrSQL.AppendLine(" FROM Agronica_Log_Invio_Agenda LogAgenda ")

            StrSQL.AppendLine(" LEFT JOIN Agronica_Log_Invio_Chiamate LogChiamate")
            StrSQL.AppendLine(" ON LogAgenda.ID_Log_Invio = LogChiamate.ID")

            StrSQL.AppendLine(" WHERE LogChiamate.Esito = 1 ")

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
                StrSQL.AppendLine(" AND LogChiamate.Dati_Ricevuti LIKE '%" & Agro_SQL_SaveText(xFiltroAggiuntivo) & "%'   ")
            End If


            StrSQL.AppendLine(" ORDER BY Data_Invio DESC")

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
    Public Function LeggiLog_JoinChiamate(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                          ByVal ID_Log_Invio As Integer,
                                          ByVal listaAgende As List(Of Integer),
                                          ByVal Validita_Inizio As String,
                                          ByVal Validita_Fine As String,
                                          ByVal Dettaglio1 As String,
                                          ByVal Dettaglio2 As String,
                                          ByVal Dettaglio3 As String,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable



        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Agenda_R.Leggi_JoinChiamate()"

        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Agronica_Log_Invio_Agenda LogAgenda ")

            StrSQL.AppendLine(" LEFT JOIN Agronica_Log_Invio_Chiamate LogChiamate")
            StrSQL.AppendLine(" ON LogAgenda.ID_Log_Invio = LogChiamate.ID")


            StrSQL.AppendLine(" WHERE 1=1 ")

            If Tipo_Esportazione <> 0 Then
                StrSQL.AppendLine(" AND LogAgenda.Tipo_Esportazione = " & Agro_SQL_SaveNum(Tipo_Esportazione) & "")
            End If

            If Validita_Inizio <> "" Then
                StrSQL.AppendLine(" AND LogAgenda.Data_Creazione >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Validita_Fine <> "" Then
                StrSQL.AppendLine(" AND LogAgenda.Data_Creazione >= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If ID_Log_Invio <> 0 Then
                StrSQL.AppendLine(" AND LogAgenda.ID_Log_Invio = " & Agro_SQL_SaveNum(ID_Log_Invio) & "   ")
            End If

            If listaAgende IsNot Nothing Then
                StrSQL.AppendLine(" AND LogAgenda.ID_Agenda IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", listaAgende.ToArray()), False) & ")   ")
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

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StrSQL.AppendLine(" ORDER BY LogChiamate.[ID] ASC")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim MessaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT
    End Function

End Class