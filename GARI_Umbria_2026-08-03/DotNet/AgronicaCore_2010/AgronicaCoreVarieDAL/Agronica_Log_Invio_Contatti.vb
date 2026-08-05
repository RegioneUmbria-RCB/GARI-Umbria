Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Agronica_Log_Invio_Contatti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                          Chiave As String,
                          Chiave_Esterna As String,
                          Piva As String,
                          Sa_Cod As Integer,
                          Cod_Contatto As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          xFiltroAggiuntivo As String,
                          xOrderBy As String,
                          ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Contatti_R.Leggi"

        Dim dt As DataTable
        Dim StrSQL As New Text.StringBuilder

        Try
            StrSQL.Length = 0

            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                     enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM Agronica_Log_Invio_Contatti")

                Case enumSelezioneVariabile.Selezione_JoinCompleta,
                     enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.AppendLine(" SELECT Agronica_Log_Invio_Contatti.[ID] ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Tipo_Esportazione ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Id_Log_Invio ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Stato ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Chiave ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Chiave_Esterna ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Piva ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Sa_Cod ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Cod_Contatto ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Note ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Data_Creazione ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Data_Modifica ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Username_Creazione ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Contatti.Username_Modifica ")

                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.PivaSuperuser ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.Dati_Inviati ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.Data_Invio ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.Esito ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.Dati_Ricevuti ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.Controllata ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.Tipo_Operazione ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.Dettaglio1 ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.Dettaglio2 ")
                    StrSQL.AppendLine(" ,Agronica_Log_Invio_Chiamate.Dettaglio3 ")

                    StrSQL.AppendLine(" FROM Agronica_Log_Invio_Contatti")
                    StrSQL.AppendLine(" INNER JOIN Agronica_Log_Invio_Chiamate")
                    StrSQL.AppendLine(" ON Agronica_Log_Invio_Contatti.ID_Log_Invio = Agronica_Log_Invio_Chiamate.ID")

            End Select

            StrSQL.AppendLine(" WHERE 1=1 ")

            If Tipo_Esportazione <> 0 Then
                StrSQL.AppendLine(" AND Agronica_Log_Invio_Contatti.Tipo_Esportazione = " & Agro_SQL_SaveNum(Tipo_Esportazione) & "")
            End If

            If Chiave <> "" Then
                StrSQL.AppendLine(" AND Agronica_Log_Invio_Contatti.Chiave = '" & Agro_SQL_SaveText(Chiave) & "'")
            End If

            If Chiave_Esterna <> "" Then
                StrSQL.AppendLine(" AND Agronica_Log_Invio_Contatti.Chiave_Esterna = '" & Agro_SQL_SaveText(Chiave_Esterna) & "'")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Agronica_Log_Invio_Contatti.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Agronica_Log_Invio_Contatti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "")
            End If

            If Cod_Contatto <> "" Then
                StrSQL.AppendLine(" AND Agronica_Log_Invio_Contatti.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server) & "   ")
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy))
            Else
                StrSQL.AppendLine(" ORDER BY Agronica_Log_Invio_Contatti.[ID] ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim MessaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

End Class

Public Class Agronica_Log_Invio_Contatti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(Tipo_Esportazione As Integer,
                           ID_Log_Invio As Integer,
                           Stato As Integer,
                           Chiave As String,
                           Chiave_Esterna As String,
                           Piva As String,
                           Sa_Cod As Integer,
                           Cod_Contatto As String,
                           Note As String,
                           ObjParametri_Server As AgronicaCoreParametri)

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Contatti_W.Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp = True

        Try

            Dim now = DateTime.Now()
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Agronica_Log_Invio_Contatti ( ")
            StrSQL.Append(" Tipo_Esportazione, ID_Log_Invio, Stato, Chiave, Chiave_Esterna, ")
            StrSQL.Append(" Piva, Sa_Cod, Cod_Contatto, Note, inviato, datainvio, ")
            StrSQL.Append(" Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, ")
            StrSQL.Append(" Validita_Inizio, Validita_Fine) ")

            StrSQL.Append("   VALUES (")
            StrSQL.Append($" {Agro_SQL_SaveNum(Tipo_Esportazione)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(ID_Log_Invio)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Stato)}, ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Chiave)}', ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Chiave_Esterna)}', ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Piva)}', ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Sa_Cod)}, ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Cod_Contatto)}', ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Note)}', ")
            StrSQL.Append($" 0, NULL, ")
            StrSQL.Append($" {Agro_SQL_SaveDateTime(now)}, ")
            StrSQL.Append($" {Agro_SQL_SaveDateTime(now)}, ")
            StrSQL.Append($" '{Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)}', ")
            StrSQL.Append($" '{Agro_SQL_SaveText(ObjParametri_Server.UsernameOperazione)}', ")
            StrSQL.Append($" {Agro_SQL_SaveDateTime(AGRODATAINIZIO)}, ")
            StrSQL.Append($" {Agro_SQL_SaveDateTime(AGRODATAFINE)} ")
            StrSQL.Append("   )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return xRisp
    End Function

    Public Function Create_Agronica_Log_Invio_Contatti(Tipo_Esportazione As Integer,
                                                       ID_Log_Invio As Integer,
                                                       Stato As Integer,
                                                       Chiave As String,
                                                       Chiave_Esterna As String,
                                                       Piva As String,
                                                       Sa_Cod As Integer,
                                                       Cod_Contatto As String,
                                                       Note As String,
                                                       ObjParametri_Server As AgronicaCoreParametri,
                                                       GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities
                                                       ) As AgronicaCoreEntityFramework_POCO.Agronica_Log_Invio_Contatti

        Dim idGen As New Agro_Sequenze
        Dim log As New AgronicaCoreEntityFramework_POCO.Agronica_Log_Invio_Contatti

        log.Tipo_Esportazione = Tipo_Esportazione

        log.ID_Log_Invio = ID_Log_Invio

        log.Stato = Stato

        log.Chiave = Chiave
        log.Chiave_Esterna = Chiave_Esterna

        log.Piva = Piva
        log.Sa_Cod = Sa_Cod
        log.Cod_Contatto = Cod_Contatto

        log.Note = Note

        log.inviato = 0
        log.datainvio = Nothing

        log.Data_Creazione = DateTime.Now
        log.Data_Modifica = DateTime.Now

        log.Username_Creazione = ObjParametri_Server.UtenteCodFiscale
        log.Username_Modifica = ObjParametri_Server.UtenteCodFiscale

        log.Validita_Inizio = AGRODATAINIZIO
        log.Validita_Fine = AGRODATAFINE

        GiasContext.Agronica_Log_Invio_Contatti.Add(log)
        GiasContext.SaveChanges()

        Return log

    End Function
End Class
