Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieDAL

Public Class FattElettronica_DAL

    Public Shared Function LeggiDataAttivazione(ByVal Piva As String, ByRef objParametri_Server As AgronicaCoreParametri) As Date

        ' leggo impostazione azienda per data attivazione fatturazione
        Dim DataAttivazione As Date = AGRODATAFINE
        Dim objImpreseCodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        Dim data_attivazione As String = objImpreseCodici.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.DataAttivazioneEFattura, objParametri_Server)
        If data_attivazione <> "" Then
            DataAttivazione = Date.ParseExact(data_attivazione, "yyyyMMdd", Globalization.CultureInfo.InvariantCulture)
        End If
        Return DataAttivazione

    End Function

    ' legge connessione super server per servizio AgroGSB
    Public Shared Function LeggiConnessioneAgroGSB(ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Super_Server As AgronicaCoreParametri) As AgronicaCoreParametri

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim objParametri_Super_Server_AgroGSB = New AgronicaCoreDataProvider.AgronicaCoreParametri(objParametri_Super_Server)
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "Connessione_AgroGSB", "", "", objParametri_Server)

        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
            Dim connessione = DTConfigSiti.Rows(0).Item("Valore")
            If Not String.IsNullOrEmpty(connessione) Then
                objParametri_Super_Server_AgroGSB.StringaConnessione = connessione
            End If
        End If

        Return objParametri_Super_Server_AgroGSB

    End Function

    ' legge configurazione servizi per invio xml fatturazione elettronica
    Public Shared Function LeggiConfigurazioneServizio(ByVal Piva As String, ByVal Servizio As Integer, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Super_Server As AgronicaCoreParametri, Optional ByVal LeggiConnessione As Boolean = True) As Configurazione_Servizio

        If LeggiConnessione Then
            objParametri_Super_Server = LeggiConnessioneAgroGSB(objParametri_Server, objParametri_Super_Server)
        End If

        Dim Configurazione_Servizi_R As New AgronicaCoreVarieDAL.Configurazione_Servizi_R()
        Dim ConfigurazioneServizio =
            Configurazione_Servizi_R.LeggiSingolo(
                objParametri_Server.PivaSuperUser,
                enum_Id_Servizio.GiasOnline, Servizio, 0,
                "Parametri_Extra LIKE '%""PIVA"": """ & Piva & """%'",
                objParametri_Super_Server)

        Return ConfigurazioneServizio

    End Function

    Public Shared Function LeggiConfigurazioniServizio(ByVal Piva As String, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Super_Server As AgronicaCoreParametri) As DataTable

        ' legge connessione super server per servizio AgroGSB
        Dim objParametri_Super_Server_AgroGSB = LeggiConnessioneAgroGSB(objParametri_Server, objParametri_Super_Server)

        Dim filtroAggiuntivo As String = "Tipo_Sincro IN (" &
            enum_Tipi_Servizi_Background.EFattura_Generazione_XML & "," &
            enum_Tipi_Servizi_Background.EFattura_Invio_XML_Attivi & "," &
            enum_Tipi_Servizi_Background.EFattura_Verifica_Esiti & "," &
            enum_Tipi_Servizi_Background.EFattura_Ricevi_XML_Passivi & ")" &
            " AND Parametri_Extra LIKE '%""PIVA"": """ & Piva & """%'"

        ' legge configurazione servizi per fatturazione elettronica
        Dim Configurazione_Servizi_R As New AgronicaCoreVarieDAL.Configurazione_Servizi_R()
        Dim ConfigurazioniServizio =
            Configurazione_Servizi_R.Leggi(
                objParametri_Server.PivaSuperUser,
                enum_Id_Servizio.GiasOnline, 0, 0,
                filtroAggiuntivo, "",
                objParametri_Super_Server_AgroGSB)

        Return ConfigurazioniServizio

    End Function

    Public Shared Function LeggiImprese(ByRef objParametri_Server As AgronicaCoreParametri) As List(Of ImpresaFattElettronica)

        Dim Filtrone As New AgronicaCoreUtility.Filtrone
        Dim ClassJoin As New JoinFiltrone
        ClassJoin.bGerarchiaImprese = True

        Dim FiltroSQL = " Imprese_Codici.id_cod = " & enum_CodiciAnagrafe.DataAttivazioneEFattura & " "
        Filtrone.ImpostaVariabiliJOIN_xFiltroUtente(FiltroSQL, ClassJoin)

        Dim dtImprese =
                Filtrone.CreaDTFiltrone(objParametri_Server,
                    FiltroSQL,
                    enum_TipoSelect_FiltroneSuperNova.Imprese,
                    "ORDER BY Imprese.rag_soc",
                    ClassJoin)

        Dim listaImprese As List(Of ImpresaFattElettronica) =
                (From dd In dtImprese.AsEnumerable
                 Select New ImpresaFattElettronica With {
                         .Piva = dd("Piva"),
                         .Rag_Soc = dd("Rag_Soc")
                }).ToList

        Return listaImprese

    End Function

    Public Shared Function LeggiFattureLog(ByVal Piva As String, ByVal IdAgenda As Integer, ByRef objParametri_Server As AgronicaCoreParametri) As List(Of FatturaSDILog)

        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

        Dim fattureLog =
            (From sdi_log_dettaglio In dal.SDI_Log_Dettaglio
             Join sdi_log In dal.SDI_Log
                On sdi_log_dettaglio.Id_Log Equals sdi_log.Id_Log
             Where sdi_log.Piva.Equals(Piva) AndAlso sdi_log.Id_Agenda = IdAgenda
             Order By sdi_log_dettaglio.Id_Log_Dettaglio Descending
             Select New FatturaSDILog With
                {
                    .Piva = sdi_log.Piva,
                    .IdAgenda = sdi_log.Id_Agenda,
                    .DataLog = If(sdi_log_dettaglio.Data_Creazione.HasValue, sdi_log_dettaglio.Data_Creazione, sdi_log.Data_Creazione),
                    .Messaggio = sdi_log_dettaglio.Messaggio,
                    .TipoErrore = sdi_log_dettaglio.TipoErrore,
                    .CodiceErrore = sdi_log_dettaglio.CodiceErrore
            }).ToList()

        ' Return String.Join("<br/>", (From l In fattureLog Select l.sdi_log_dettaglio.Messaggio))

        Return fattureLog

    End Function

    Public Shared Function LeggiFatture(ByVal Piva As String, ByVal DataDal As Date?, ByVal DataAl As Date?, ByVal TipoDoc As String, ByVal FiltroLav As String, ByVal InvioXML As Boolean, ByRef objParametri_Server As AgronicaCoreParametri, ByRef objParametri_Utenti As AgronicaCoreParametri) As List(Of FatturaSDI)

        Dim DataAttivazione = LeggiDataAttivazione(Piva, objParametri_Server)
        Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim dal = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(efConnString)

        dal.Database.CommandTimeout = 3600

        Dim causali As Integer()
        If TipoDoc = "F" Then
            causali = {LAVCOD_FATTURA_EMESSA}
        ElseIf TipoDoc = "N" Then
            causali = {LAVCOD_NOTA_ACCREDITO_EMESSA}
        Else
            causali = {LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_FATTURA_EMESSA}
        End If

        Dim fattureSDILog =
            (From sdi_log In dal.SDI_Log
             Join agenda In dal.Agenda
                On sdi_log.Id_Agenda Equals agenda.Id_Agenda
             Join movimenti In dal.Movimenti
                On agenda.Id_Agenda Equals movimenti.Id_Agenda
             Join risorse_umane In dal.Risorse_Umane
                On movimenti.Cod_RisUm Equals risorse_umane.Cod_RisUm
             Join contatti In dal.Contatti
                On risorse_umane.Piva Equals contatti.Piva And risorse_umane.Cod_Contatto Equals contatti.Cod_Contatto
             Where causali.Contains(agenda.Lav_Cod) AndAlso agenda.PIVA.Equals(Piva) _
                AndAlso (movimenti.Data_Movimento >= DataAttivazione) _
                AndAlso movimenti.Cau_Mov.Equals("4000")
             Select New FatturaSDI With
                {
                    .Piva = contatti.Cod_Contatto,
                    .Rag_Soc = contatti.Rag_Soc,
                    .Cognome = contatti.Cognome,
                    .Nome = contatti.Nome,
                    .IdAgenda = agenda.Id_Agenda,
                    .Descrizione = agenda.des_lib,
                    .BloccoFlag = agenda.Blocco_Flag,
                    .BloccoFlagDes = If(CInt(agenda.Blocco_Flag) = 0, "No", If(CInt(agenda.Blocco_Flag) = 1, "Sì", If(CInt(agenda.Blocco_Flag) = 1000, "Standby", ""))),
                    .BloccoData = agenda.Blocco_Data,
                    .BloccoUsername = agenda.Blocco_Username,
                    .LavCod = agenda.Lav_Cod,
                    .ExtraInt = movimenti.Extra_Int,
                    .DataDocumento = movimenti.Data_Movimento,
                    .NrDocumento = movimenti.Doc_Numero,
                    .NrDocumentoDes = movimenti.Doc_Numero_Des,
                    .NrDocumentoSin = movimenti.Doc_Numero_Sin,
                    .IdLog = sdi_log.Id_Log,
                    .NomeFileXML = If(sdi_log.NomeFileXML, ""),
                    .NomeFileZIP = If(sdi_log.NomeFileZIP, ""),
                    .DataLogSDI = If(sdi_log.Data_Creazione, AGRODATAINIZIO),
                    .DataGenXML = If(sdi_log.Data_Gen_XML, AGRODATAINIZIO),
                    .DataOraInvio = If(sdi_log.Data_SDI, AGRODATAINIZIO),
                    .IdSDI = CStr(sdi_log.IDSDI),
                    .TipoFattura = sdi_log.TipoFattura,
                    .NrTentativi = CStr(sdi_log.TentativiInvio),
                    .StatoInvio = CStr(sdi_log.Gias_Status),
                    .StatoEsito = CStr(sdi_log.SDI_Status),
                    .StatoInvioDes = If(sdi_log.Gias_Status = 1, "In elaborazione", If(sdi_log.Gias_Status = 300, "XML non generato", If(sdi_log.Gias_Status = 301, "XML non generabile", If(sdi_log.Gias_Status = 400, "XML generato", If(sdi_log.Gias_Status = 401, "XML esportato", If(sdi_log.Gias_Status = 402, "XML da rigenerare", If(sdi_log.Gias_Status = 403, "Non Inviato", If(sdi_log.Gias_Status = 500, "Inviato", "")))))))),
                    .StatoEsitoDes = If(sdi_log.SDI_Status = 100, "Non ancora disponibile", If(sdi_log.SDI_Status = 200, "Esiti multipli", If(sdi_log.SDI_Status = 300, "Negativo", If(sdi_log.SDI_Status = 400, "Positivo", If(sdi_log.SDI_Status = 500, "Mancata consegna", ""))))),
                    .Note = If(sdi_log.Note, ""),
                    .InvioXML = InvioXML
            }).ToList()

        ' raggruppo log fatture per data creazione più recente ed le escludo da quelle dell'agenda
        Dim fattureSDI = fattureSDILog.GroupBy(Function(a) a.IdAgenda).ToList().SelectMany(Function(b) b.Where(Function(c) c.DataLogSDI = b.Max(Function(x) x.DataLogSDI))).ToList()
        Dim agendeFattureSDI As List(Of Integer) = (From r In fattureSDI Select r.IdAgenda).ToList()

        Dim fatture =
            (From agenda In dal.Agenda
             Join movimenti In dal.Movimenti
                On agenda.Id_Agenda Equals movimenti.Id_Agenda
             Join risorse_umane In dal.Risorse_Umane
                On movimenti.Cod_RisUm Equals risorse_umane.Cod_RisUm
             Join contatti In dal.Contatti
                On risorse_umane.Piva Equals contatti.Piva And risorse_umane.Cod_Contatto Equals contatti.Cod_Contatto
             Where causali.Contains(agenda.Lav_Cod) AndAlso agenda.PIVA.Equals(Piva) _
                AndAlso (movimenti.Data_Movimento >= DataAttivazione) _
                AndAlso (movimenti.Data_Movimento >= DataDal.Value AndAlso movimenti.Data_Movimento <= DataAl.Value) _
                AndAlso movimenti.Cau_Mov.Equals("4000")
             Select New FatturaSDI With
                {
                    .Piva = contatti.Cod_Contatto,
                    .Rag_Soc = contatti.Rag_Soc,
                    .Cognome = contatti.Cognome,
                    .Nome = contatti.Nome,
                    .IdAgenda = agenda.Id_Agenda,
                    .Descrizione = agenda.des_lib,
                    .BloccoFlag = agenda.Blocco_Flag,
                    .BloccoFlagDes = If(CInt(agenda.Blocco_Flag) = 0, "No", If(CInt(agenda.Blocco_Flag) = 1, "Sì", If(CInt(agenda.Blocco_Flag) = 1000, "Standby", ""))),
                    .BloccoData = agenda.Blocco_Data,
                    .BloccoUsername = agenda.Blocco_Username,
                    .LavCod = agenda.Lav_Cod,
                    .ExtraInt = movimenti.Extra_Int,
                    .DataDocumento = movimenti.Data_Movimento,
                    .NrDocumento = movimenti.Doc_Numero,
                    .NrDocumentoDes = movimenti.Doc_Numero_Des,
                    .NrDocumentoSin = movimenti.Doc_Numero_Sin,
                    .IdLog = 0,
                    .NomeFileXML = "",
                    .NomeFileZIP = "",
                    .DataLogSDI = AGRODATAINIZIO,
                    .DataGenXML = AGRODATAINIZIO,
                    .DataOraInvio = AGRODATAINIZIO,
                    .IdSDI = "",
                    .TipoFattura = "",
                    .NrTentativi = "",
                    .StatoInvio = "",
                    .StatoEsito = "",
                    .StatoInvioDes = "",
                    .StatoEsitoDes = "",
                    .Note = "",
                    .InvioXML = InvioXML
                }).ToList()

        fatture = fatture.Where(Function(e) Not agendeFattureSDI.Contains(e.IdAgenda)).ToList()
        Dim listaFatture = fatture.Concat(fattureSDI)

        Select Case FiltroLav
            Case "T"
                listaFatture = listaFatture.Where(Function(e) (
                    e.Esito = "N" OrElse e.BloccoFlag <> 0 OrElse
                    (e.DataDocumento >= DataDal.Value AndAlso e.DataDocumento <= DataAl.Value)
                ))
            Case "N"
                listaFatture = listaFatture.Where(Function(e) (
                    e.Esito = "N" AndAlso (e.DataDocumento >= DataDal.Value AndAlso e.DataDocumento <= DataAl.Value)
                ))
            Case "P"
                listaFatture = listaFatture.Where(Function(e) (
                    e.Esito = "P" AndAlso (e.DataDocumento >= DataDal.Value AndAlso e.DataDocumento <= DataAl.Value)
                ))
        End Select

        Return listaFatture.OrderBy(Function(e) e.DataDocumento).ThenBy(Function(e) e.NumeroDocumento).ToList()

    End Function

    Public Shared Sub AggiornaFattureLog(ByVal Piva As String, ByVal IdsLog As List(Of Integer), ByVal Stato As Integer, ByRef objParametri_Server As AgronicaCoreParametri)

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
        Dim giasContext As New Gias_DeveloperServer_Entities(efConnString)

        Dim logR = New SDI_Log_R(giasContext)
        Dim logW = New SDI_Log_W(giasContext)

        Dim qry = From logs In giasContext.SDI_Log
                  Where logs.Piva.Equals(Piva) _
                  AndAlso IdsLog.Contains(logs.Id_Log)
                  Select logs

        Dim listaLog = qry.ToList()
        For Each log In listaLog
            log.Gias_Status = Stato
            logW.Aggiorna(log)
        Next

    End Sub

    Public Shared Function LeggiUltimoScarico(ByVal PIVA As String, ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim dalR = New Imprese_Codici_R(objParametri_Server)
        Dim valore = dalR.Leggi(PIVA, enum_CodiciAnagrafe.DataUltimaRicezioneEFattura)
        If Not String.IsNullOrEmpty(valore) Then
            Dim info = valore.Split("|")
            Dim data = Date.ParseExact(info(0), "yyyyMMdd HHmmss", Globalization.CultureInfo.InvariantCulture)
            Return "<br><br>Data ultimo scarico: " & Format(data, "dd/MM/yyyy  HH:mm:ss") & "<br>Num. documenti scaricati: " & info(1)
        End If
        Return ""
    End Function

End Class
