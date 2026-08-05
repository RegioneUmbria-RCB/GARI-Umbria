
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Agronica_Log_Invio_Anagrafe_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                         Tipo_Esportazione As Integer,
                         ID_Log_Invio As Integer,
                         Stato As Integer,
                         Tipo As String,
                         Chiave As String,
                         Piva As String,
                         Sa_Cod As Integer,
                         Appezza As Integer,
                         Id_Reg As Integer,
                         Progetto_Cod As Integer,
                         ObjParametri_Server As AgronicaCoreParametri,
                             Optional Chiave_Esterna As String = "",
                             Optional Campo_Cod As Integer = 0,
                             Optional Fabbricato_Cod As Integer = 0,
                             Optional Data_InvioInizio As DateTime = AGRODATAINIZIO,
                             Optional Data_InvioFine As DateTime = AGRODATAFINE,
                             Optional Causale_Cod As Integer = 0,
                             Optional Mac_Cod As Integer = 0
                         ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_R.Leggi"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM Agronica_Log_Invio_Anagrafe ")
            StrSQL.AppendLine(" WHERE 1 = 1 ")

            If Tipo_Esportazione <> 0 Then
                StrSQL.AppendLine($" AND Tipo_Esportazione = {Agro_SQL_SaveNum(Tipo_Esportazione)} ")
            End If
            If ID_Log_Invio <> 0 Then
                StrSQL.AppendLine($" AND ID_Log_Invio = {Agro_SQL_SaveNum(ID_Log_Invio)} ")
            End If
            If Stato <> 0 Then
                StrSQL.AppendLine($" AND Stato = {Agro_SQL_SaveNum(Stato)} ")
            End If
            If Tipo <> "" Then
                StrSQL.AppendLine($" AND Tipo = '{Agro_SQL_SaveText(Tipo)}' ")
            End If
            If Chiave <> "" Then
                StrSQL.AppendLine($" AND Chiave = '{Agro_SQL_SaveText(Chiave)}' ")
            End If
            If Piva <> "" Then
                StrSQL.AppendLine($" AND Piva = '{Agro_SQL_SaveText(Piva)}' ")
            End If
            If Sa_Cod <> 0 Then
                StrSQL.AppendLine($" AND Sa_Cod = {Agro_SQL_SaveNum(Sa_Cod)}")
            End If
            If Appezza <> 0 Then
                StrSQL.AppendLine($" AND Appezza = {Agro_SQL_SaveNum(Appezza)} ")
            End If
            If Id_Reg <> 0 Then
                StrSQL.AppendLine($" AND Id_Reg = {Agro_SQL_SaveNum(Id_Reg)} ")
            End If
            If Progetto_Cod <> 0 Then
                StrSQL.AppendLine($" AND Progetto_Cod = {Agro_SQL_SaveNum(Progetto_Cod)} ")
            End If

            If Chiave_Esterna <> "" Then
                StrSQL.AppendLine($" AND Chiave_Esterna = '{Agro_SQL_SaveText(Chiave_Esterna)}'")
            End If
            If Campo_Cod <> 0 Then
                StrSQL.AppendLine($" AND Campo_Cod = {Agro_SQL_SaveNum(Campo_Cod)}")
            End If
            If Fabbricato_Cod <> 0 Then
                StrSQL.AppendLine($" AND Fabbricato_Cod = {Agro_SQL_SaveNum(Fabbricato_Cod)}")
            End If
            If Data_InvioInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine($" AND Data_Invio >= {Agro_SQL_SaveDateTime(Data_InvioInizio)}")
            End If
            If Data_InvioFine <> AGRODATAFINE Then
                StrSQL.AppendLine($" AND Data_Invio <= {Agro_SQL_SaveDateTime(Data_InvioFine)}")
            End If
            If Causale_Cod <> 0 Then
                StrSQL.AppendLine($" AND Causale_Cod = {Agro_SQL_SaveNum(Causale_Cod)}")
            End If
            If Mac_Cod <> 0 Then
                StrSQL.AppendLine($" AND Mac_Cod = {Agro_SQL_SaveNum(Mac_Cod)}")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(ObjParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT
    End Function

    Public Function Leggi_LogOperazioniBDN(ByVal piva As String,
                                           ByVal saCod As Integer,
                                           ByVal codAnimale As Integer,
                                           ByVal idAgenda As Integer,
                                           ByVal idMov As Integer,
                                           ByVal idMovDet As Integer,
                                           ByRef objParametri_Server As AgronicaCoreParametri)
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_R.Leggi_LogOperazioniBDN"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT mov.Piva, ")
            StrSQL.AppendLine("    Agenda.Id_Agenda, ")
            StrSQL.AppendLine("    mov.Id_Mov, ")
            StrSQL.AppendLine("    mov_dett.Id_Mov_Det, ")
            StrSQL.AppendLine("    mov_dett.Cod_Progetto, ")
            StrSQL.AppendLine("    alich.ID AS IDlogInvioChiamata, ")
            StrSQL.AppendLine("    alich.Esito, ")
            StrSQL.AppendLine("    alich.Data_Invio, ")
            StrSQL.AppendLine("    alich.Controllata, ")
            StrSQL.AppendLine("    alich.Tipo_Operazione, ")
            StrSQL.AppendLine("    alich.Dati_Inviati, ")
            StrSQL.AppendLine("    alich.Dati_Ricevuti, ")
            StrSQL.AppendLine("    alich.Username_Creazione, ")
            StrSQL.AppendLine("    alian.ID AS IDlogInvioAnagrafe, ")
            StrSQL.AppendLine("    alian.Stato, ")
            StrSQL.AppendLine("    alian.Tipo, ")
            StrSQL.AppendLine("    alian.Chiave, ")
            StrSQL.AppendLine("    alian.Chiave_Esterna, ")
            StrSQL.AppendLine("    alian.Note, ")
            StrSQL.AppendLine("    alian.Causale_Cod, ")
            StrSQL.AppendLine("    BDN_Causali.Descrizione AS Causale_Des ")
            StrSQL.AppendLine("FROM Agenda ")
            StrSQL.AppendLine("INNER JOIN Movimenti AS mov ON Agenda.PIVA = mov.Piva AND Agenda.Id_Agenda = mov.Id_Agenda ")
            StrSQL.AppendLine("INNER JOIN Movimenti_dettagli AS mov_dett ON mov.Piva = mov_dett.Piva AND mov.Id_Mov = mov_dett.Id_Mov ")
            StrSQL.AppendLine("INNER JOIN Mov_Destinazioni AS mov_dest ON mov_dett.Piva = mov_dest.Piva AND mov_dett.Id_Mov = mov_dest.Id_Mov AND mov_dett.Id_Mov_Det = mov_dest.Id_Mov_Det ")
            StrSQL.AppendLine("INNER JOIN Agronica_Log_Invio_Anagrafe AS alian ON mov_dett.Piva = alian.Piva AND mov_dett.Cod_Progetto = alian.Progetto_Cod ")
            StrSQL.AppendLine("INNER JOIN Agronica_Log_Invio_Chiamate AS alich ON alian.ID_Log_Invio = alich.ID ")
            StrSQL.AppendLine("INNER JOIN Agronica_Log_Invio_Agenda AS aliag ON alich.ID = aliag.ID_Log_Invio AND mov_dett.Id_Agenda = aliag.Id_Agenda AND mov_dett.Id_Mov = aliag.Id_Mov AND mov_dett.Id_Mov_Det = aliag.Id_Mov_Det ")
            StrSQL.AppendLine("LEFT JOIN BDN_Causali ON alian.Causale_Cod = BDN_Causali.ID ")
            StrSQL.AppendLine("WHERE 1=1 ")
            StrSQL.AppendLine("    AND alian.Tipo_Esportazione = " & enum_Esportazioni_Sistema_Cod.BDN)
            If piva <> "" Then
                StrSQL.AppendLine("    AND Agenda.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
            End If
            If saCod <> 0 Then
                StrSQL.AppendLine("    AND alian.Sa_Cod = " & Agro_SQL_SaveNum(saCod))
            End If
            If codAnimale <> 0 Then
                StrSQL.AppendLine("    AND mov_dett.Cod_Progetto = " & Agro_SQL_SaveNum(codAnimale))
            End If
            If idAgenda <> 0 Then
                StrSQL.AppendLine("    AND aliag.ID_Agenda = " & Agro_SQL_SaveNum(idAgenda))
            End If
            If idMov <> 0 Then
                StrSQL.AppendLine("    AND aliag.Id_Mov = " & Agro_SQL_SaveNum(idMov))
            End If
            If idMovDet <> 0 Then
                StrSQL.AppendLine("    AND aliag.Id_Mov_Det = " & Agro_SQL_SaveNum(idMovDet))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_LogSincroStalla(ByVal piva As String,
                                          ByVal saCod As Integer,
                                          ByVal staNum As Integer,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal Filtro_Visibilita_Utente As Boolean,
                                          ByRef objParametri_Server As AgronicaCoreParametri)
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_R.Leggi_LogSincroStalla"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT alian.Chiave, ")
            StrSQL.AppendLine("     alian.ID AS Id_Log_Anagrafe, ")
            StrSQL.AppendLine("     Imprese.rag_soc AS Impresa, ")
            StrSQL.AppendLine("     Centri_Aziendali.sa_nome AS Centro, ")
            StrSQL.AppendLine("     Fabbricati.Fabbricato_Des AS Stalla, ")
            StrSQL.AppendLine("     IIF(alian.Tipo = 'SincroBDN', 'BDN', 'VetInfo') AS Tipo, ")
            StrSQL.AppendLine("     alic.Esito, ")
            StrSQL.AppendLine("     alic.Dati_Ricevuti, ")
            StrSQL.AppendLine("     alian.Note, ")
            StrSQL.AppendLine("     Utenti.[USER], ")
            StrSQL.AppendLine("     alic.Data_Creazione AS Data_Inizio, ")
            StrSQL.AppendLine("     alic.Data_Modifica AS Data_Fine ")
            StrSQL.AppendLine("FROM Agronica_Log_Invio_Chiamate AS alic ")
            StrSQL.AppendLine("INNER JOIN Agronica_Log_Invio_Anagrafe AS alian ON alic.ID = alian.ID_Log_Invio ")
            StrSQL.AppendLine("INNER JOIN Imprese ON alian.Piva = Imprese.Piva ")
            StrSQL.AppendLine("INNER JOIN Centri_Aziendali ON Imprese.Piva = Centri_aziendali.Piva AND alian.Sa_Cod = Centri_Aziendali.sa_cod ")
            StrSQL.AppendLine("INNER JOIN Fabbricati ON Centri_aziendali.Piva = Fabbricati.Piva AND Centri_aziendali.sa_cod = Fabbricati.sa_Cod AND alian.Fabbricato_Cod = Fabbricati.Fabbricato_Cod ")
            StrSQL.AppendLine("INNER JOIN Stalla ON Fabbricati.PIVA = Stalla.PIVA And Fabbricati.SA_COD = Stalla.sa_cod And Fabbricati.Fabbricato_Cod = Stalla.STA_NUM ")

            If Filtro_Visibilita_Utente Then
                StrSQL.AppendLine(" inner join utenti_Visibilita_Appoggio p (NOLOCK) ON Stalla.Piva = p.piva AND p.Sa_Cod = Stalla.Sa_Cod  AND p.entita_Cod = 2 AND p.Appezza = 0 AND p.Id_Reg = 0 AND p.Username = " & Agro_SQL_SaveText_NULL(objParametri_Server.UtenteUsername) & " ")
            End If

            StrSQL.AppendLine("LEFT JOIN Utenti ON alian.Username_Modifica = Utenti.CODICE_FISCALE ")
            StrSQL.AppendLine("WHERE Fabbricati.Validita_inizio < " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine) & " ")
            StrSQL.AppendLine("     AND Fabbricati.Validita_Fine > " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio) & " ")
            StrSQL.AppendLine("     AND (alian.Tipo = 'SincroBDN' OR alian.Tipo = 'SincroVetInfo') ")
            If piva <> "" Then
                StrSQL.AppendLine("     AND Imprese.Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If
            If saCod <> 0 Then
                StrSQL.AppendLine("     AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(saCod))
            Else
                'legge se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim FiltroCentri As String = ""
                Dim DtCentriVisibili As DataTable
                DtCentriVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(piva) & "'", "", objParametri_Server)
                If Not DtCentriVisibili Is Nothing AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For i = 0 To DtCentriVisibili.Rows.Count - 1
                        FiltroCentri &= DtCentriVisibili.Rows(i).Item("sa_cod") & ","
                    Next
                    If FiltroCentri <> "" Then
                        StrSQL.AppendLine("     AND (Fabbricati.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ")) ")
                    End If
                End If
            End If
            If staNum <> 0 Then
                StrSQL.AppendLine("     AND Fabbricati.Fabbricato_Cod = " & Agro_SQL_SaveNum(staNum))
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine("    AND alian.Data_Modifica >= " & Agro_SQL_SaveDateTime(Validita_Inizio) & " ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                Validita_Fine = Validita_Fine.AddHours(23)
                Validita_Fine = Validita_Fine.AddMinutes(59)
                Validita_Fine = Validita_Fine.AddSeconds(59)
                Validita_Fine = Validita_Fine.AddMilliseconds(999)
                StrSQL.AppendLine("    AND alian.Data_Modifica <= " & Agro_SQL_SaveDateTime(Validita_Fine) & " ")
            End If


            StrSQL.AppendLine("ORDER BY Data_Inizio DESC ")

            '------------------------------------------------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '------------------------------------------------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return dt

    End Function

End Class

Public Class Agronica_Log_Invio_Anagrafe_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Scrivi(Tipo_Esportazione As Integer,
                           ID_Log_Invio As Integer,
                           Stato As Integer,
                           Tipo As String,
                           Chiave As String,
                           Piva As String,
                           Sa_Cod As Integer,
                           Appezza As Integer,
                           Id_Reg As Integer,
                           Progetto_Cod As Integer,
                           Note As String,
                           Data_Invio As DateTime,
                           ObjParametri_Server As AgronicaCoreParametri,
                               Optional Chiave_Esterna As String = "",
                               Optional Campo_Cod As Integer = 0,
                               Optional Fabbricato_Cod As Integer = 0,
                               Optional Causale_Cod As Integer = 0,
                               Optional Mac_Cod As Integer = 0
                           )

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W.Scrivi()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp = True

        Try

            Dim now = DateTime.Now()
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO Agronica_Log_Invio_Anagrafe(Tipo_Esportazione, ID_Log_Invio, Stato, Tipo, Chiave, Chiave_Esterna, ")
            StrSQL.Append("                          Piva, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, Campo_Cod, Fabbricato_Cod, Causale_Cod, Mac_Cod, Note, inviato, datainvio, ")
            StrSQL.Append("                          Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine) ")

            StrSQL.Append("   VALUES (")
            StrSQL.Append($" {Agro_SQL_SaveNum(Tipo_Esportazione)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(ID_Log_Invio)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Stato)}, ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Tipo)}', ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Chiave)}', ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Chiave_Esterna)}', ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Piva)}', ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Sa_Cod)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Appezza)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Id_Reg)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Progetto_Cod)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Campo_Cod)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Fabbricato_Cod)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Causale_Cod)}, ")
            StrSQL.Append($" {Agro_SQL_SaveNum(Mac_Cod)}, ")
            StrSQL.Append($" '{Agro_SQL_SaveText(Note)}', ")
            StrSQL.Append($" 0, ")
            StrSQL.Append($" {Agro_SQL_SaveDateTime(Data_Invio)}, ")
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

    Public Function Modifica(ByVal Tipo_Esportazione As Integer,
                             ByVal ID_Log_Invio As Integer,
                             ByVal Stato As Integer,
                             ByVal Tipo As String,
                             ByVal Chiave As String,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal Id_Reg As Integer,
                             ByVal Progetto_Cod As Integer,
                             ByVal Note As String,
                             ByVal Data_Invio As DateTime,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri_Server As AgronicaCoreParametri,
                             ByVal Optional Chiave_Esterna As String = "",
                             ByVal Optional Campo_Cod As Integer = 0,
                             ByVal Optional Fabbricato_Cod As Integer = 0,
                             ByVal Optional Causale_Cod As Integer = 0,
                             ByVal Optional Mac_Cod As Integer = 0
                             )
        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W.Modifica()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp = True

        Try
            Dim now = DateTime.Now()
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" UPDATE Agronica_Log_Invio_Anagrafe SET ")
            StrSQL.AppendLine("     Tipo_Esportazione = " & Agro_SQL_SaveNum(Tipo_Esportazione))
            StrSQL.AppendLine("     , Stato = " & Agro_SQL_SaveNum(Stato))
            StrSQL.AppendLine("     , Tipo = '" & Agro_SQL_SaveText(Tipo) & "'")
            StrSQL.AppendLine("     , Chiave = '" & Agro_SQL_SaveText(Chiave) & "'")
            StrSQL.AppendLine("     , Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.AppendLine("     , Appezza = " & Agro_SQL_SaveNum(Appezza))
            StrSQL.AppendLine("     , Id_Reg = " & Agro_SQL_SaveNum(Id_Reg))
            StrSQL.AppendLine("     , Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod))
            StrSQL.AppendLine("     , Note = '" & Agro_SQL_SaveText(Note) & "'")
            StrSQL.AppendLine("     , dataInvio = " & Agro_SQL_SaveDate(Data_Invio))
            StrSQL.AppendLine("     , Campo_Cod = " & Agro_SQL_SaveNum(Campo_Cod))
            StrSQL.AppendLine("     , Fabbricato_Cod = " & Agro_SQL_SaveNum(Fabbricato_Cod))
            StrSQL.AppendLine("     , Causale_Cod = " & Agro_SQL_SaveNum(Causale_Cod))
            StrSQL.AppendLine("     , Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod))
            StrSQL.AppendLine("     , Data_Modifica = " & Agro_SQL_SaveDateTime(now))
            StrSQL.AppendLine("     , Username_Modifica = '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "' ")
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("      AND Chiave = '" & Agro_SQL_SaveText(Chiave) & "'")
            StrSQL.AppendLine("      AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.AppendLine("      AND ID_Log_Invio = " & Agro_SQL_SaveNum(ID_Log_Invio))
            StrSQL.AppendLine("      AND Appezza = " & Agro_SQL_SaveNum(Appezza))

            If Causale_Cod <> 0 Then
                StrSQL.AppendLine("      AND Causale_Cod = " & Agro_SQL_SaveNum(Causale_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Cancella(ByVal ID_Log_Invio As Integer,
                             ByVal Chiave As String,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Appezza As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri_Server As AgronicaCoreParametri,
                             ByVal Optional Causale_Cod As Integer = 0)

        Dim NomeRoutine As String = "AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W.Modifica()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp = True

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE Agronica_Log_Invio_Anagrafe ")
            StrSQL.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("      AND Chiave = '" & Agro_SQL_SaveText(Chiave) & "'")
            StrSQL.AppendLine("      AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            StrSQL.AppendLine("      AND ID_Log_Invio = " & Agro_SQL_SaveNum(ID_Log_Invio))
            StrSQL.AppendLine("      AND Appezza = " & Agro_SQL_SaveNum(Appezza))

            If Causale_Cod <> 0 Then
                StrSQL.AppendLine("      AND Causale_Cod = " & Agro_SQL_SaveNum(Causale_Cod))
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri_Server))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Create_Agronica_Log_Invio_Anagrafe(Tipo_Esportazione As Integer,
                                                       ID_Log_Invio As Integer,
                                                       Stato As Integer,
                                                       Tipo As String,
                                                       Chiave As String,
                                                       Piva As String,
                                                       Sa_Cod As Integer,
                                                       Appezza As Integer,
                                                       Id_Reg As Integer,
                                                       Progetto_Cod As Integer,
                                                       Note As String,
                                                       ObjParametri_Server As AgronicaCoreParametri,
                                                       GiasContext As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                                       Optional Chiave_Esterna As String = "",
                                                       Optional Campo_Cod As Integer = 0,
                                                       Optional Fabbricato_Cod As Integer = 0,
                                                       Optional Causale_Cod As Integer = 0,
                                                       Optional Mac_Cod As Integer = 0
                                                       ) As AgronicaCoreEntityFramework_POCO.Agronica_Log_Invio_Anagrafe

        Dim logws As New AgronicaCoreEntityFramework_POCO.Agronica_Log_Invio_Anagrafe

        logws.Tipo_Esportazione = Tipo_Esportazione
        logws.ID_Log_Invio = ID_Log_Invio

        logws.Stato = Stato
        logws.Tipo = Tipo

        logws.Chiave = Chiave
        logws.Chiave_Esterna = Chiave_Esterna

        logws.Piva = Piva
        logws.Sa_Cod = Sa_Cod
        logws.Appezza = Appezza
        logws.Campo_Cod = Campo_Cod
        logws.Id_Reg = Id_Reg
        logws.Progetto_Cod = Progetto_Cod
        logws.Fabbricato_Cod = Fabbricato_Cod

        logws.Note = Note

        logws.inviato = 0

        logws.Data_Creazione = DateTime.Now
        logws.Data_Modifica = DateTime.Now

        logws.Username_Creazione = ObjParametri_Server.UsernameOperazione
        logws.Username_Modifica = ObjParametri_Server.UsernameOperazione

        logws.Validita_Inizio = AGRODATAINIZIO
        logws.Validita_Fine = AGRODATAFINE

        logws.Causale_Cod = Causale_Cod
        logws.Mac_Cod = Mac_Cod

        GiasContext.Agronica_Log_Invio_Anagrafe.Add(logws)
        GiasContext.SaveChanges()

        Return logws

    End Function
End Class
