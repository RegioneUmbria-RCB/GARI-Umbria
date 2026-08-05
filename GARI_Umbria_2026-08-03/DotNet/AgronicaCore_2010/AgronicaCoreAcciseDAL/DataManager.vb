Imports System.Data.Entity
Imports AgronicaCoreAcciseCommon
Imports AgronicaCoreAcciseCommon.DAA.Messaggi.Ie815
Imports AgronicaCoreAcciseDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieDAL

Public Class DataManager : Implements IDataManager

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _objParametriUtente As AgronicaCoreParametri
    Private ReadOnly _objParametriSuperServer As AgronicaCoreParametri
    Private ReadOnly _debug As Boolean
    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities

    Private ReadOnly _accDaa_R As ACCDAA_R
    Private ReadOnly _accDaa_W As ACCDAA_W
    Private ReadOnly _confServizi_R As Conf_Servizi_R
    Private ReadOnly _accDaa_Conf_RW As ACCDAA_ACC_Configurazione_RW

    Public Sub New(
                  ByVal objParametriServer As AgronicaCoreParametri,
                  ByVal objParametriUtente As AgronicaCoreParametri,
                  ByVal objParametriSuperServer As AgronicaCoreParametri,
                  ByVal debug As Boolean)

        _objParametriServer = objParametriServer
        _objParametriUtente = objParametriUtente
        _objParametriSuperServer = objParametriSuperServer
        _debug = debug

        Dim gefutils As New Gias_EF_Utility
        Dim efConnString As String = gefutils.GetEntityConnectionString(_objParametriServer.StringaConnessione)
        _giasContext = New Gias_DeveloperServer_Entities(efConnString)

        _accDaa_R = New ACCDAA_R(_giasContext)
        _accDaa_W = New ACCDAA_W(_giasContext)
        _confServizi_R = New Conf_Servizi_R(_objParametriSuperServer)
        _accDaa_Conf_RW = New ACCDAA_ACC_Configurazione_RW(_giasContext)

    End Sub

    Public Function LeggiConnessioneAgroGSB(
                                           ByRef objParametri_Server As AgronicaCoreParametri,
                                           ByRef objParametri_Super_Server As AgronicaCoreParametri) As AgronicaCoreParametri Implements IDataManager.LeggiConnessioneAgroGSB

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

    Public Function LeggiConfigurazioneServizio(ByVal Piva As String,
                                                ByVal Servizio As Integer,
                                                ByRef objParametri_Server As AgronicaCoreParametri,
                                                ByRef objParametri_Super_Server As AgronicaCoreParametri,
                                                Optional ByVal LeggiConnessione As Boolean = True) As Configurazione_Servizio Implements IDataManager.LeggiConfigurazioneServizio

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

    Public Sub AggiornaConfigurazioneServizio(ByVal Piva As String,
                                            ByVal idServizio As enum_Id_Servizio,
                                            ByVal tipoSincro As enum_Tipi_Servizi_Background,
                                            ByVal idRiga As Integer,
                                            ByVal userName As String,
                                            ByVal password As String,
                                            ByRef objParametri_Server As AgronicaCoreParametri,
                                            ByRef objParametri_Super_Server As AgronicaCoreParametri)

        Dim Configurazione_Servizi_W As New AgronicaCoreVarieDAL.Configurazione_Servizi_R()
        Configurazione_Servizi_W.SetUserNameEPassword(objParametri_Server.PivaSuperUser, idServizio, tipoSincro, idRiga, userName, password, objParametri_Super_Server)

    End Sub

    Public Function ServizioAttivo(ByVal servizio As Configurazione_Servizio) As Boolean Implements IDataManager.ServizioAttivo
        Return _confServizi_R.ServizioAttivo(servizio)
    End Function

    Public Sub DisabilitaServizio(servizio As Configurazione_Servizio) Implements IDataManager.DisabilitaServizio
        _confServizi_R.DisabilitaServizio(servizio)
    End Sub
    Public Function ACCDAA_LeggiDaaDaSpedire() As List(Of ACCDAA_W_StoricoMessaggiDogane) Implements IDataManager.ACCDAA_LeggiDaaDaSpedire

        Dim stati = New List(Of enum_WWorflow_WAnagraficaStati) From {enum_WWorflow_WAnagraficaStati.DAA_Telematico_Richiesta_Creata}
        Return _accDaa_R.ACCDAA_StoricoMessaggiDogane_Leggi(stati)

    End Function

    Public Function ACCDAA_LeggiMessaggiRisposta() As List(Of ACCDAA_W_StoricoMessaggiDogane) Implements IDataManager.ACCDAA_LeggiMessaggiRisposta
        Dim stati = New List(Of enum_WWorflow_WAnagraficaStati) From {enum_WWorflow_WAnagraficaStati.DAA_Telematico_File_Firmato_Inviato}
        Return _accDaa_R.ACCDAA_StoricoMessaggiDogane_Leggi(stati)
    End Function

    Public Function ACCDAA_LeggiEsiti() As List(Of ACCDAA_W_StoricoMessaggiDogane) Implements IDataManager.ACCDAA_LeggiEsiti
        Dim stati = New List(Of enum_WWorflow_WAnagraficaStati) From {enum_WWorflow_WAnagraficaStati.DAA_Telematico_In_fase_di_verifica}
        Return _accDaa_R.ACCDAA_StoricoMessaggiDogane_Leggi(stati)
    End Function

    Public Sub ACCDAA_AggiornaStatoManager(Of T)(ByVal testata As ACCDAA_DAA_Testata, ByVal messaggio As ACCDAA_W_StoricoMessaggiDogane) _
        Implements IDataManager.ACCDAA_AggiornaStatoManager

        _accDaa_W.ACCDAA_AggiornaStatoManager(Of T)(testata, messaggio)

    End Sub
    Public Function Leggi_ACCDAA_Testata(DAA_ID As Integer) As ACCDAA_DAA_Testata Implements IDataManager.Leggi_ACCDAA_Testata
        Return _accDaa_R.Leggi_ACCDAA_Testata(DAA_ID)
    End Function

    Public Sub ACCDAA_StoricoMessaggiDogane_Errori_Logga(ByVal idLog As Integer,
                                                         ByVal descrizione As String,
                                                         ByVal tipo As String,
                                                         ByVal numero As String,
                                                         ByVal campo As String) Implements IDataManager.ACCDAA_StoricoMessaggiDogane_Errori_Logga

        Dim errore = _accDaa_R.ACCDAA_StoricoMessaggiDogane_Errori_Leggi(idLog, tipo, numero)
        If errore Is Nothing Then
            errore = New ACCDAA_W_StoricoMessaggiDogane_Errori With
                    {
                        .Messaggio_id = idLog,
                        .Descrizione = If(String.IsNullOrEmpty(descrizione), "", descrizione),
                        .TipoErrore = tipo,
                        .Numero = numero,
                        .CampoMessaggio = campo
                    }
            _accDaa_W.ACCDAA_StoricoMessaggiDogane_Errori_Logga(errore, True)
        Else
            errore.Descrizione = descrizione
            _accDaa_W.ACCDAA_StoricoMessaggiDogane_Errori_Logga(errore, False)
        End If

    End Sub

    Public Sub ACCDAA_StoricoMessaggiDogane_Aggiorna(entita As ACCDAA_W_StoricoMessaggiDogane) Implements IDataManager.ACCDAA_StoricoMessaggiDogane_Aggiorna
        _accDaa_W.ACCDAA_StoricoMessaggiDogane_Aggiorna(entita)
    End Sub
    Public Sub ACCDAA_DAA_Testata_Aggiorna(entita As ACCDAA_DAA_Testata) Implements IDataManager.ACCDAA_DAA_Testata_Aggiorna
        _accDaa_W.ACCDAA_DAA_Testata_Aggiorna(entita)
    End Sub

    Public Sub ACCDAA_StoricoMessaggiDogane_Errori_Logga(idLog As Integer, errori As List(Of D_Parsed)) Implements IDataManager.ACCDAA_StoricoMessaggiDogane_Errori_Logga

        errori.ForEach(Sub(e)
                           Me.ACCDAA_StoricoMessaggiDogane_Errori_Logga(idLog, "", e.TipoErrore, e.CodiceErrore, e.Campo)
                       End Sub)

    End Sub

    Public Sub ACCDAA_StoricoMessaggiDogane_Errori_Logga(idLog As Integer, errori As List(Of J_Parsed)) Implements IDataManager.ACCDAA_StoricoMessaggiDogane_Errori_Logga
        errori.ForEach(Sub(e)
                           Me.ACCDAA_StoricoMessaggiDogane_Errori_Logga(idLog, e.DescrizioneErrore, e.TipoErrore, e.NumeroControllo, e.DescrizioneCampo)
                       End Sub)
    End Sub

    Public Sub Mov_Dettaglio_Tecnico_Extra_Aggiorna(entita As Mov_Dettaglio_Tecnico_Extra) Implements IDataManager.Mov_Dettaglio_Tecnico_Extra_Aggiorna
        _accDaa_W.Mov_Dettaglio_Tecnico_Extra_Aggiorna(entita)
    End Sub

    Public Function Mov_Dettaglio_Tecnico_Leggi(DAA_Id As Integer) As Mov_Dettaglio_Tecnico_Extra Implements IDataManager.Mov_Dettaglio_Tecnico_Leggi
        Return _accDaa_R.Mov_Dettaglio_Tecnico_Leggi(DAA_Id)
    End Function

    Public Sub ACCDAA_StoricoMessaggiDogane_Errori_Elimina(idLog As Integer) Implements IDataManager.ACCDAA_StoricoMessaggiDogane_Errori_Elimina
        Dim entita = _accDaa_R.ACCDAA_StoricoMessaggiDogane_Errori_Leggi(idLog, "", "")
        If entita IsNot Nothing Then
            _accDaa_W.ACCDAA_StoricoMessaggiDogane_Errori_Elimina(entita)
        End If
    End Sub

    Public Function Leggi_ACCDAA_ACC_RiepilogoInvioDati(rieplilogoInvioDati_ID As Integer) As ACCDAA_ACC_RiepilogoInvioDati Implements IDataManager.Leggi_ACCDAA_ACC_RiepilogoInvioDati
        Return _accDaa_R.Leggi_ACCDAA_ACC_RiepilogoInvioDati(rieplilogoInvioDati_ID)
    End Function

    Public Sub ACCDAA_ACC_RiepilogoInvioDati_Aggiorna(entita As ACCDAA_ACC_RiepilogoInvioDati) Implements IDataManager.ACCDAA_ACC_RiepilogoInvioDati_Aggiorna
        _accDaa_W.ACCDAA_ACC_RiepilogoInvioDati_Aggiorna(entita)
    End Sub

    Public Function Stampe_LeggiGaranzieCircolazione(Piva As String, dataDa As Date, dataA As Date) As List(Of GaranziaCircolazione) Implements IDataManager.Stampe_LeggiGaranzieCircolazione

        Dim reader As New ACCDAA_R(_objParametriServer)
        Dim dtSaldo = reader.Stampe_LeggiSaldoGaranziaAllaData(Piva, dataDa)
        Dim dt = reader.Stampe_LeggiGaranzieCircolazione(Piva, dataDa, dataA)

        Dim garanzie = dt.ToList(enum_TipoStampa_DAA.GaranziaCircolazione, dataDa, dataA)

        If garanzie.Any() Then

            Dim riportoFinePeriodo = OttieniRiportoFinePeriodo(dtSaldo)

            For Each g As GaranziaCircolazione In garanzie
                g.RiportoPeriodoPrecedente = riportoFinePeriodo
                g.SaldoProgressivo = (riportoFinePeriodo - g.Importo_Impegnato) + g.Importo_Svincolato
                riportoFinePeriodo = g.SaldoProgressivo
            Next
        End If

        Return garanzie.Select(Function(g) DirectCast(g, GaranziaCircolazione)).ToList()

    End Function

    Private Function OttieniRiportoFinePeriodo(ByVal dt As DataTable) As Decimal

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return 0
        End If

        Dim row As DataRow = dt.AsEnumerable().FirstOrDefault()
        Return CDec(row.Item(0)) - CDec(row.Item(1)) + CDec(row.Item(2))

    End Function

    Public Function Stampe_Intestazione(Piva As String) As IntestazioneReport Implements IDataManager.Stampe_Intestazione
        Dim intestazione = _accDaa_R.Stampe_Intestazione(Piva)

        If intestazione IsNot Nothing AndAlso intestazione.Any Then

            Dim flatData = intestazione.FirstOrDefault()

            Dim result = New IntestazioneReport With
            {
                .PIVA = flatData.PIVA,
                .CAP = flatData.CAP,
                .Comune = flatData.com_des,
                .Frazione = flatData.frz_des,
                .Indirizzo = flatData.ind_des,
                .Provincia = flatData.pro_cod,
                .RagioneSociale = flatData.rag_soc,
                .Stato = flatData.stato
            }

            result.CodiceAccisa = intestazione.Where(Function(i) i.Id_cod = "4003").FirstOrDefault().Val_cod
            result.CodiceUfficioDogane = intestazione.Where(Function(i) i.Id_cod = "4005").FirstOrDefault().Val_cod

            Return result

        End If

        Return New IntestazioneReport()

    End Function

    Public Function Stampe_LeggiPartiteSospensione(Piva As String, dataDa As Date, dataA As Date) As List(Of PartitaSospensione) Implements IDataManager.Stampe_LeggiPartiteSospensione

        Dim reader As New ACCDAA_R(_objParametriServer)
        Dim dtDaa = reader.Stampe_LeggiPartiteSospensione(Piva, dataDa, dataA)
        Dim partite = dtDaa.ToList(enum_TipoStampa_DAA.PartitaSospensione, dataDa, dataA)

        Return partite.Select(Function(g) DirectCast(g, PartitaSospensione)).ToList()

    End Function

    Public Function Leggi_Chiave_ACCDAA_ACC_Configurazione(ByVal chiave As String)

        Dim conf = _accDaa_Conf_RW.Leggi(chiave)
        If conf IsNot Nothing Then
            Return conf.ValoreConfigurazione
        Else
            Return ""
        End If

    End Function

    Public Function ServizioAttivo() As Boolean Implements IDataManager.ServizioAttivo

        Dim conf = _accDaa_Conf_RW.Leggi("ServizioAttivo")
        If conf IsNot Nothing Then
            Return CBool(conf.ValoreConfigurazione)
        Else
            Return True
        End If

    End Function

    Public Sub DisabilitaServizio() Implements IDataManager.DisabilitaServizio

        Dim conf = _accDaa_Conf_RW.Leggi("ServizioAttivo")
        If conf IsNot Nothing Then
            conf.ValoreConfigurazione = "False"
            _accDaa_Conf_RW.Aggiorna(conf)
        Else
            Dim entita As New ACCDAA_ACC_Configurazione With
            {
                .ID = New AgronicaCoreDataProvider.Agro_Sequenze().NuovoId_Tabella_EF(_giasContext, "ACCDAA_ACC_Configurazione", 0, 2000000000, _objParametriServer),
                .CodiceConfigurazione = "ServizioAttivo",
                .ValoreConfigurazione = "False"
            }
            _accDaa_Conf_RW.Inserisci(entita)
        End If
    End Sub

End Class

Public Class ACCDAA_ACC_Configurazione_RW : Inherits AcciseBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub
    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub
    Public Function Leggi(ByVal codiceConfigurazione As String) As ACCDAA_ACC_Configurazione

        Try
            Return (From conf In _giasContext.ACCDAA_ACC_Configurazione Where conf.CodiceConfigurazione.ToLower().Equals(codiceConfigurazione.ToLower())).FirstOrDefault()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_ACC_Configurazione_RW.Leggi")
        End Try

    End Function

    Public Sub Aggiorna(ByVal entita As ACCDAA_ACC_Configurazione)
        _giasContext.Entry(entita).State = EntityState.Modified
        _giasContext.SaveChanges()
    End Sub

    Public Sub Inserisci(ByVal entita As ACCDAA_ACC_Configurazione)
        _giasContext.ACCDAA_ACC_Configurazione.Add(entita)
        _giasContext.SaveChanges()
    End Sub

End Class

Public Class Conf_Servizi_R : Inherits AcciseBaseDAL

    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub

    Public Function ServizioAttivo(ByVal servizio As Configurazione_Servizio) As Boolean

        Try

            Dim retVal = New List(Of Configurazione_Servizio)
            Dim csr = New Configurazione_Servizi_R()
            Dim servizi = csr.Leggi(servizio.PivaSuperuser, servizio.Id_Servizio, servizio.Tipo_Sincro, servizio.Id_Riga, False, _objParametriServer.StringaConnessione)

            If servizi.Rows.Count = 0 Then
                Return False
            End If

            Dim servizioAttuale = servizi.AsEnumerable().FirstOrDefault()

            Return CBool(servizioAttuale.Item("Attivo"))

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Conf_Servizi_R.Leggi")
        End Try

    End Function

    Public Sub DisabilitaServizio(ByVal servizio As Configurazione_Servizio)

        Try
            Dim csr = New Configurazione_Servizi_R()
            csr.AbilitaDisabilita(servizio.PivaSuperuser, servizio.Id_Servizio, servizio.Tipo_Sincro, servizio.Id_Riga, False, _objParametriServer)
        Catch ex As Exception
            Throw RaiseDAlException(ex, "Conf_Servizi_R.AbilitaDisabilita")
        End Try

    End Sub

    Public Function Leggi(ByVal pivaSuperUser As String, ByVal tipoServizio As enum_Tipi_Servizi_Background) As List(Of Configurazione_Servizio)

        Try

            Dim retVal = New List(Of Configurazione_Servizio)

            Dim csr = New Configurazione_Servizi_R()
            Dim servizi = csr.Leggi(pivaSuperUser, enum_Id_Servizio.GiasOnline, tipoServizio, 0, False, _objParametriServer.StringaConnessione)

            If servizi.Rows.Count = 0 Then
                Return Nothing
            End If

            For Each row As DataRow In servizi.Rows
                retVal.Add(New Configurazione_Servizio With
                           {
                                .DirectoryFileEsportazioni = row.Item("DirectoryFileEsportazioni").ToString(),
                                .DirectoryLOG = row.Item("DirectoryLOG").ToString(),
                                .Parametri_Extra = row.Item("Parametri_Extra").ToString(),
                                .Id_Cod_Cliente = CInt(row.Item("Id_Cod_Cliente"))
                            })

            Next row

            Return retVal

        Catch ex As Exception
            Throw RaiseDAlException(ex, "Conf_Servizi_R.Leggi")
        End Try

    End Function

End Class

Public Class ACCDAA_R : Inherits AcciseBaseDAL

    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Private ReadOnly _objParametriServer As AgronicaCoreParametri
    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Sub New(ByVal objParametriServer As AgronicaCoreParametri)
        _objParametriServer = objParametriServer
    End Sub
    Public Function ACCDAA_StoricoMessaggiDogane_Leggi(ByVal stati As List(Of enum_WWorflow_WAnagraficaStati)) As List(Of ACCDAA_W_StoricoMessaggiDogane)

        Try

            Dim result = From w_smd In _giasContext.ACCDAA_W_StoricoMessaggiDogane
                         Where stati.Contains(w_smd.Stato_Manager.Value)
            Return result.ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_R.Leggi")
        End Try

    End Function

    Public Function ACCDAA_StoricoMessaggiDogane_Errori_Leggi(ByVal id As Integer,
                                                              ByVal tipo As String, ByVal numero As String) As ACCDAA_W_StoricoMessaggiDogane_Errori

        Try
            Dim result = From w_smd In _giasContext.ACCDAA_W_StoricoMessaggiDogane_Errori
                         Where w_smd.Messaggio_id.Equals(id) AndAlso w_smd.TipoErrore.Equals(tipo) AndAlso
                         w_smd.Numero.Equals(numero)
            Return result.FirstOrDefault()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_R.ACCDAA_StoricoMessaggiDogane_Errori_Leggi")
        End Try

    End Function

    Public Function Leggi_ACCDAA_Testata(ByVal DAA_ID As Integer) As ACCDAA_DAA_Testata

        Try
            Dim result = From t In _giasContext.ACCDAA_DAA_Testata
                         Where t.DAA_id = DAA_ID
            Return result.FirstOrDefault()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_R.Leggi_ACCDAA_Testata")
        End Try

    End Function

    Public Function Leggi_ACCDAA_ACC_RiepilogoInvioDati(ByVal rieplilogoInvioDati_ID As Integer) As ACCDAA_ACC_RiepilogoInvioDati

        Try
            Dim result = From t In _giasContext.ACCDAA_ACC_RiepilogoInvioDati
                         Where t.id.Equals(rieplilogoInvioDati_ID)
            Return result.FirstOrDefault()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_R.Leggi_ACCDAA_ACC_RiepilogoInvioDati")
        End Try

    End Function

    Public Function Mov_Dettaglio_Tecnico_Leggi(ByVal DAA_Id As Integer) As Mov_Dettaglio_Tecnico_Extra

        Try
            Return (From mvt In _giasContext.Mov_Dettaglio_Tecnico_Extra
                    Where mvt.Id_Agenda.Equals(DAA_Id) AndAlso mvt.Sa_Cod = 0).FirstOrDefault()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_R.Mov_Dettaglio_Tecnico_Leggi")
        End Try

    End Function

    Public Function Stampe_LeggiPartiteSospensione(Piva As String, dataDa As Date, dataA As Date) As DataTable

        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim nomeProcedura As String = "ACCDAA_R.Stampe_LeggiPartiteSospensione"

        Try

            Stb.Append(" Select ")
            Stb.Append(" 'EAD' as tipo, daa.id_mov_det, ")
            Stb.Append(" DAA.Speditore_CodiceAccisa as Codice_Accisa , isnull(DAA.TARIC, '') as TARIC , isnull(daa.CADD , '') as CADD ")
            Stb.Append(", isnull(daa.CPA , '') as CPA , isnull(CP.NC, '') as NC , daa.Peso as Qta  , daa.Doc_Numero_Sin , daa.Doc_Numero ")
            Stb.Append(", daa.Doc_Numero_Des , ")
            Stb.Append("  tes.Data_Riferimento as data_emis_documento_AAAAMMGG , ")
            Stb.Append(" tes.Data_Riferimento as data_origine, ")
            Stb.Append(" case when daa.tipo_Destinazione = 6 ")
            Stb.Append(" then COALESCE(Contatti_Codici_UfficioSpedizioneImportazione.val_Cod,'') else COALESCE(daa.Codice_Accisa_Destinatario,'') end as Cod_identificativo_destinatario ")
            Stb.Append(", daa.Titolo_Alcol as Grado_Alcool , daa.QTA_EXTRA as volume_nominale_confezioni , tes.stato_manager , ex.Codice_Alternativo as arc ")
            Stb.Append(", daa.num_colli , tes.arc_progressivo , daa.Data_Spedizione, COALESCE(r3c.DataRientroTerzaCopia, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & ") AS DataRientroTerzaCopia ")
            Stb.Append(", tes.DAA_id as DAA_ID, COALESCE( smd.Txt_Messaggio_C, '') as Messaggio, tes.Tipo_Messaggio ")
            Stb.Append("From GIASDAA daa ")
            Stb.Append("inner Join ACCDAA_DAA_Testata tes ")
            Stb.Append("On daa.Id_Agenda = tes.DAA_id ")
            Stb.Append(" inner Join Mov_Dettaglio_Tecnico_Extra ex ")
            Stb.Append("On daa.Id_Agenda = ex.Id_Agenda ")
            Stb.Append("And ex.Codice_Alternativo Is Not null ")
            Stb.Append("And ex.codice_alternativo <>'' ")
            Stb.Append("INNER Join Risorse_Umane as Risorse_Umane_Destinatario ")
            Stb.Append("On ex.ACCDAA_Cod_Risum_Destinatario = Risorse_Umane_Destinatario.Cod_RisUm ")
            Stb.Append("INNER Join Contatti as Contatti_Destinatario ")
            Stb.Append("On Risorse_Umane_Destinatario.Piva = Contatti_Destinatario.Piva ")
            Stb.Append("And Risorse_Umane_Destinatario.Cod_Contatto = Contatti_Destinatario.Cod_Contatto ")
            Stb.Append("Left Join Contatti_Codici AS Contatti_Codici_UfficioSpedizioneImportazione ")
            Stb.Append("On Contatti_Codici_UfficioSpedizioneImportazione.Piva = Contatti_Destinatario.Piva ")
            Stb.Append("And Contatti_Codici_UfficioSpedizioneImportazione.Cod_Contatto = Contatti_Destinatario.Cod_Contatto ")
            Stb.Append("And Contatti_Codici_UfficioSpedizioneImportazione.Id_cod = 4005 ")
            Stb.Append("Left Join [ACCDAA_ANAG_TA20_CodificaProdotti] AS [CP] ON [Daa].[id_Accisa_Cod] = [CP].[ID] ")
            Stb.Append("left join ACCDAA_IE818_NotaRicevimento r3c on tes.DAA_id = r3c.DAA_ID ")
            Stb.Append(" left join ACCDAA_W_StoricoMessaggiDogane smd on tes.DAA_id = smd.DAA_ID ")
            Stb.Append(" where tes.daa_id IN ( ")
            Stb.Append(" select distinct tes.DAA_id as DAA_ID ")
            Stb.Append(" From GIASDAA daa inner Join ACCDAA_DAA_Testata tes On daa.Id_Agenda = tes.DAA_id  ")
            Stb.Append(" Left join ACCDAA_IE818_NotaRicevimento r3c on tes.DAA_id = r3c.DAA_ID ")
            Stb.Append(" where  cast(tes.Data_Riferimento As Date) >= " & Agro_SQL_SaveDate(dataDa) & " ")
            Stb.Append(" And cast(tes.Data_Riferimento As Date) <= " & Agro_SQL_SaveDate(dataA) & ") ")
            Stb.Append(" And daa.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            Stb.Append("and smd.Stato_Manager = '" & enum_WWorflow_WAnagraficaStati.DAA_Telematico_Risposta_Positiva_Ricevuta & "' ")
            Stb.Append("and smd.Txt_Messaggio_C IS NOT NULL and smd.Txt_Messaggio_C <> ''")

            Stb.Append(" UNION ")

            Stb.Append(" Select ")
            Stb.Append(" 'ROR' as tipo, daa.id_mov_det, ")
            Stb.Append(" DAA.Speditore_CodiceAccisa as Codice_Accisa , isnull(DAA.TARIC, '') as TARIC , isnull(daa.CADD , '') as CADD ")
            Stb.Append(", isnull(daa.CPA , '') as CPA , isnull(CP.NC, '') as NC , daa.Peso as Qta  , daa.Doc_Numero_Sin , daa.Doc_Numero ")
            Stb.Append(", daa.Doc_Numero_Des , ")
            Stb.Append("  tes.Data_Riferimento as data_emis_documento_AAAAMMGG , ")
            Stb.Append(" COALESCE(r3c.DataRientroTerzaCopia,  CONVERT(DateTime,'1900/01/01',120) ) as data_origine, ")
            Stb.Append(" case when daa.tipo_Destinazione = 6 ")
            Stb.Append(" then COALESCE(Contatti_Codici_UfficioSpedizioneImportazione.val_Cod,'') else COALESCE(daa.Codice_Accisa_Destinatario,'') end as Cod_identificativo_destinatario ")
            Stb.Append(", daa.Titolo_Alcol as Grado_Alcool , daa.QTA_EXTRA as volume_nominale_confezioni , tes.stato_manager , ex.Codice_Alternativo as arc ")
            Stb.Append(", daa.num_colli , tes.arc_progressivo , daa.Data_Spedizione, COALESCE(r3c.DataRientroTerzaCopia, " & Agro_SQL_SaveDate(AGRODATAINIZIO) & ") AS DataRientroTerzaCopia ")
            Stb.Append(", tes.DAA_id as DAA_ID, COALESCE( smd.Txt_Messaggio_C, '') as Messaggio, tes.Tipo_Messaggio ")
            Stb.Append("From GIASDAA daa ")
            Stb.Append("inner Join ACCDAA_DAA_Testata tes ")
            Stb.Append("On daa.Id_Agenda = tes.DAA_id ")
            Stb.Append(" inner Join Mov_Dettaglio_Tecnico_Extra ex ")
            Stb.Append("On daa.Id_Agenda = ex.Id_Agenda ")
            Stb.Append("And ex.Codice_Alternativo Is Not null ")
            Stb.Append("And ex.codice_alternativo <>'' ")
            Stb.Append("INNER Join Risorse_Umane as Risorse_Umane_Destinatario ")
            Stb.Append("On ex.ACCDAA_Cod_Risum_Destinatario = Risorse_Umane_Destinatario.Cod_RisUm ")
            Stb.Append("INNER Join Contatti as Contatti_Destinatario ")
            Stb.Append("On Risorse_Umane_Destinatario.Piva = Contatti_Destinatario.Piva ")
            Stb.Append("And Risorse_Umane_Destinatario.Cod_Contatto = Contatti_Destinatario.Cod_Contatto ")
            Stb.Append("Left Join Contatti_Codici AS Contatti_Codici_UfficioSpedizioneImportazione ")
            Stb.Append("On Contatti_Codici_UfficioSpedizioneImportazione.Piva = Contatti_Destinatario.Piva ")
            Stb.Append("And Contatti_Codici_UfficioSpedizioneImportazione.Cod_Contatto = Contatti_Destinatario.Cod_Contatto ")
            Stb.Append("And Contatti_Codici_UfficioSpedizioneImportazione.Id_cod = 4005 ")
            Stb.Append("Left Join [ACCDAA_ANAG_TA20_CodificaProdotti] AS [CP] ON [Daa].[id_Accisa_Cod] = [CP].[ID] ")
            Stb.Append("left join ACCDAA_IE818_NotaRicevimento r3c on tes.DAA_id = r3c.DAA_ID ")
            Stb.Append(" left join ACCDAA_W_StoricoMessaggiDogane smd on tes.DAA_id = smd.DAA_ID ")
            Stb.Append(" where tes.daa_id IN ( ")
            Stb.Append(" select distinct tes.DAA_id as DAA_ID ")
            Stb.Append(" From GIASDAA daa inner Join ACCDAA_DAA_Testata tes On daa.Id_Agenda = tes.DAA_id  ")
            Stb.Append(" Left join ACCDAA_IE818_NotaRicevimento r3c on tes.DAA_id = r3c.DAA_ID ")
            Stb.Append(" where  cast(r3c.DataRientroTerzaCopia As Date) >= " & Agro_SQL_SaveDate(dataDa) & " ")
            Stb.Append(" And cast(r3c.DataRientroTerzaCopia As Date) <= " & Agro_SQL_SaveDate(dataA) & ") ")
            Stb.Append(" And daa.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            Stb.Append("and smd.Stato_Manager = '" & enum_WWorflow_WAnagraficaStati.DAA_Telematico_Risposta_Positiva_Ricevuta & "' ")
            Stb.Append("and smd.Txt_Messaggio_C IS NOT NULL and smd.Txt_Messaggio_C <> ''")
            Stb.Append(" order by data_origine, tipo, id_mov_det ")

            dt = EseguiQuery_Lettura(_objParametriServer, Stb.ToString, nomeProcedura)
            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, nomeProcedura)
        End Try

    End Function





    Public Function Stampe_LeggiSaldoGaranziaAllaData(ByVal Piva As String, ByVal dataA As DateTime) As DataTable

        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim nomeProcedura As String = "ACCDAA_R.Stampe_LeggiSaldoGaranziaAllaData"

        Try

            Stb.AppendLine(";with t1 as ( ")
            Stb.AppendLine("Select top 1 RiportoPeriodoPrecedente As GaranziaIniziale ")
            Stb.AppendLine("From ACCDAA_ACC_RiepilogoInvioDati  Where Piva = '" & Agro_SQL_SaveText(Piva) & "' order by DataA asc ), ")
            Stb.AppendLine("t2 As  ")
            Stb.AppendLine("( ")
            Stb.AppendLine("Select SUM(importo_Impegnato) As importo_Impegnato, SUM(importo_Svincolato) As importo_Svincolato ")
            Stb.AppendLine("from( ")
            Stb.AppendLine("Select ")
            Stb.AppendLine("0 as Importo_Impegnato  ")
            Stb.AppendLine(", g.Importo as Importo_Svincolato ")
            Stb.AppendLine(", r3c.DataRientroTerzaCopia as data_emis_documento_AAAAMMGG ")
            Stb.AppendLine(", 1 as tipo  ")
            Stb.AppendLine("From ACCDAA_DAA_GaranziaDiCircolazione g ")
            Stb.AppendLine("inner Join Agenda daa ")
            Stb.AppendLine("On g.DAA_ID = daa.Id_Agenda  and daa.lav_cod = 1063")
            Stb.AppendLine("inner Join ACCDAA_IE818_NotaRicevimento r3c  ")
            Stb.AppendLine("On r3c.DAA_ID = daa.Id_Agenda   ")
            Stb.AppendLine("inner Join ACCDAA_DAA_Testata tes  ")
            Stb.AppendLine("On daa.Id_Agenda = tes.DAA_id ")
            Stb.AppendLine("where r3c.DataRientroTerzaCopia < " & Agro_SQL_SaveDate(dataA) & " ")
            Stb.AppendLine("union all ")
            Stb.AppendLine("Select ")
            Stb.AppendLine("gaCir.Importo as Importo_Impegnato ")
            Stb.AppendLine(", 0 as importo_Svincolato ")
            Stb.AppendLine(", tes.Data_riferimento as data_emis_documento_AAAAMMGG ")
            Stb.AppendLine(", 2 as Tipo ")
            Stb.AppendLine("From ACCDAA_DAA_GaranziaDiCircolazione gaCir ")
            Stb.AppendLine("inner Join Agenda daa ")
            Stb.AppendLine("On gaCir.DAA_ID = daa.Id_Agenda and daa.lav_cod = 1063")
            Stb.AppendLine("inner Join ACCDAA_DAA_Testata tes ")
            Stb.AppendLine("On daa.Id_Agenda = tes.DAA_id ")
            Stb.AppendLine("where tes.Data_riferimento <" & Agro_SQL_SaveDate(dataA) & "   ) a ) ")
            Stb.AppendLine("Select ")
            Stb.AppendLine("coalesce(t1.GaranziaIniziale, 0) As GaranziaIniziale, ")
            Stb.AppendLine("coalesce(t2.Importo_Impegnato, 0) As Importo_Impegnato , ")
            Stb.AppendLine("coalesce(t2.Importo_Svincolato, 0) As  Importo_Svincolato ")
            Stb.AppendLine("From t1 cross Join t2 ")


            dt = EseguiQuery_Lettura(_objParametriServer, Stb.ToString, nomeProcedura)
            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_R.Stampe_LeggiGaranzieCircolazione")
        End Try

    End Function

    Public Function Stampe_LeggiGaranzieCircolazione(ByVal Piva As String, ByVal dataDa As DateTime, ByVal dataA As DateTime) As DataTable

        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim nomeProcedura As String = "ACCDAA_R.Stampe_LeggiGaranzieCircolazione"

        Try

            Stb.AppendLine(";with t1 as (")
            Stb.AppendLine("Select top 1 RiportoPeriodoPrecedente, RiportoFinePeriodo, 1 As rn")
            Stb.AppendLine("From ACCDAA_ACC_RiepilogoInvioDati Where Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            Stb.AppendLine("order by DataA desc, id desc ),")

            Stb.AppendLine("t2 as ")
            Stb.AppendLine("(")
            Stb.AppendLine("Select 'C' as Tipo,importo_Impegnato, importo_Svincolato, data_emis_documento_AAAAMMGG, 1 as  rn")
            Stb.AppendLine("from(")
            Stb.AppendLine("Select")
            Stb.AppendLine("0 as Importo_Impegnato ")
            Stb.AppendLine(", g.Importo as Importo_Svincolato ")
            Stb.AppendLine(", r3c.DataRientroTerzaCopia as data_emis_documento_AAAAMMGG ")
            Stb.AppendLine(", 1 as tipo ")
            Stb.AppendLine("From ACCDAA_DAA_GaranziaDiCircolazione g ")
            Stb.AppendLine("inner Join Agenda  daa ")
            Stb.AppendLine("On g.DAA_ID = daa.Id_Agenda and daa.lav_cod = 1063")
            Stb.AppendLine("inner Join ACCDAA_IE818_NotaRicevimento r3c ")
            Stb.AppendLine("On r3c.DAA_ID = daa.Id_Agenda  ")
            Stb.AppendLine("inner Join ACCDAA_DAA_Testata tes ")
            Stb.AppendLine("On daa.Id_Agenda = tes.DAA_id ")
            Stb.AppendLine("where r3c.DataRientroTerzaCopia >= " & Agro_SQL_SaveDate(dataDa) & " ")
            Stb.AppendLine("And r3c.DataRientroTerzaCopia <= " & Agro_SQL_SaveDate(dataA) & " ")
            Stb.AppendLine("union all ")

            Stb.AppendLine("Select ")
            Stb.AppendLine("gaCir.Importo as Importo_Impegnato ")
            Stb.AppendLine(", 0 as importo_Svincolato ")
            Stb.AppendLine(", tes.Data_riferimento as data_emis_documento_AAAAMMGG ")
            Stb.AppendLine(", 2 as Tipo ")
            Stb.AppendLine("From ACCDAA_DAA_GaranziaDiCircolazione gaCir ")
            Stb.AppendLine("inner Join Agenda daa ")
            Stb.AppendLine("On gaCir.DAA_ID = daa.Id_Agenda and daa.lav_cod = 1063")
            Stb.AppendLine("inner Join ACCDAA_DAA_Testata tes ")
            Stb.AppendLine("On daa.Id_Agenda = tes.DAA_id ")
            Stb.AppendLine("where tes.Data_riferimento >= " & Agro_SQL_SaveDate(dataDa) & " ")
            Stb.AppendLine("And tes.Data_riferimento <= " & Agro_SQL_SaveDate(dataA) & " ")
            Stb.AppendLine(") a ")
            Stb.AppendLine(")")
            Stb.AppendLine("Select MAX(coalesce(t1.RiportoPeriodoPrecedente, 0)) As RiportoPeriodoPrecedente ,  MAX(coalesce(t1.RiportoFinePeriodo, 0)) As RiportoFinePeriodo, ")
            Stb.AppendLine("coalesce(t2.Tipo, 'E') as Tipo, SUM(coalesce(t2.Importo_Impegnato, 0)) As Importo_Impegnato , SUM(coalesce(t2.Importo_Svincolato, 0)) As  Importo_Svincolato, ")
            Stb.AppendLine("coalesce(t2.data_emis_documento_AAAAMMGG, getDate()) as data_emis_documento_AAAAMMGG, SUM(0) as ImportoIntegrato, SUM(0) as ImportoScaduto ")
            Stb.AppendLine("From t1 full outer Join t2 On t1.rn = t2.rn ")
            Stb.AppendLine("group by tipo, data_emis_documento_AAAAMMGG ")
            Stb.AppendLine("Order By t2.data_emis_documento_AAAAMMGG ASC")

            dt = EseguiQuery_Lettura(_objParametriServer, Stb.ToString, nomeProcedura)

            Return dt

        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_R.Stampe_LeggiGaranzieCircolazione")
        End Try

    End Function

    Public Function Stampe_Intestazione(ByVal Piva As String) As IEnumerable(Of Object)

        Try

            Dim qry = From imp In _giasContext.Imprese
                      Join impi In _giasContext.ImpresexIndirizzi
                      On imp.PIVA Equals impi.PIVA
                      Join ind In _giasContext.Indirizzi
                      On impi.cod_indirizzo Equals ind.cod_indirizzo
                      Group Join c In _giasContext.Contatti
                      On imp.PIVA Equals c.Cod_Contatto
                      Into c_group = Group
                      From _c_group In c_group.DefaultIfEmpty()
                      Group Join cc In _giasContext.Contatti_Codici
                      On _c_group.Cod_Contatto Equals cc.Cod_Contatto
                      Into cc_group = Group
                      From _cc_group In cc_group.DefaultIfEmpty
                      Where imp.PIVA.Equals(Piva) AndAlso
                      impi.Tipo_Indirizzo.Equals(enum_IndirizzoTipo.SedeOperativa) AndAlso
                      _cc_group.Id_cod.Equals(4003) OrElse _cc_group.Id_cod.Equals(4005)
                      Select New With
                        {
                            imp.PIVA,
                            imp.rag_soc,
                            ind.ind_des,
                            ind.frz_des,
                            ind.CAP,
                            ind.com_des,
                            ind.pro_cod,
                            ind.stato,
                            ind.com_cod_istat,
                            _c_group.Nome,
                            _c_group.Cognome,
                            _c_group.Cod_Contatto,
                            _cc_group.Val_cod,
                            _cc_group.Id_cod
                        }

            Return qry.ToList()

        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_R.Stampe_Intestazione")
        End Try

    End Function

End Class

Public Class ACCDAA_W : Inherits AcciseBaseDAL

    Private ReadOnly _giasContext As Gias_DeveloperServer_Entities
    Public Sub New(ByVal giasContext As Gias_DeveloperServer_Entities)
        _giasContext = giasContext
    End Sub

    Public Sub ACCDAA_AggiornaStatoManager(Of T)(ByVal testata As ACCDAA_DAA_Testata, ByVal messaggio As ACCDAA_W_StoricoMessaggiDogane)

        If GetType(T).Name = GetType(CD815AType).Name Then
            If testata IsNot Nothing Then
                CD815AType_AggiornaStatoManager(testata,
                                            messaggio,
                                            enum_WWorflow_WAnagraficaStati.DAA_Telematico_DAA_proposto_IE815_inviato_a_sistema,
                                            enum_WWorflow_WAnagraficaStati.DAA_Telematico_File_Firmato_Inviato)
            Else
                CD815AType_AggiornaStatoManager(Nothing,
                                           messaggio,
                                           Nothing,
                                           enum_WWorflow_WAnagraficaStati.DAA_Telematico_In_fase_di_verifica)
            End If
        End If

    End Sub

    Public Sub ACCDAA_StoricoMessaggiDogane_Aggiorna(ByVal entita As ACCDAA_W_StoricoMessaggiDogane)

        Try
            entita.DataOraUltimaModifica_string = DateTime.Now
            _giasContext.Entry(entita).State = EntityState.Modified

            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_W.ACCDAA_StoricoMessaggiDogane_Aggiorna")
        End Try

    End Sub

    Public Sub ACCDAA_StoricoMessaggiDogane_Errori_Elimina(ByVal entita As ACCDAA_W_StoricoMessaggiDogane_Errori)

        Try
            _giasContext.ACCDAA_W_StoricoMessaggiDogane_Errori.Remove(entita)
            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_W.ACCDAA_StoricoMessaggiDogane_Errori_Elimina")
        End Try

    End Sub
    Public Sub ACCDAA_DAA_Testata_Aggiorna(ByVal entita As ACCDAA_DAA_Testata)

        Try
            entita.Data_Modifica = DateTime.Now
            _giasContext.Entry(entita).State = EntityState.Modified

            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_W.ACCDAA_DAA_Testata_Aggiorna")
        End Try

    End Sub

    Public Sub ACCDAA_ACC_RiepilogoInvioDati_Aggiorna(ByVal entita As ACCDAA_ACC_RiepilogoInvioDati)

        Try
            _giasContext.Entry(entita).State = EntityState.Modified
            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_W.ACCDAA_ACC_RiepilogoInvioDati")
        End Try

    End Sub

    Private Sub CD815AType_AggiornaStatoManager(ByVal testata As ACCDAA_DAA_Testata, ByVal messaggio As ACCDAA_W_StoricoMessaggiDogane,
                                                ByVal statoTestata As enum_WWorflow_WAnagraficaStati, ByVal statoMessaggio As enum_WWorflow_WAnagraficaStati)

        Dim dataModifica = DateTime.Now

        Try
            messaggio.Stato_Manager = statoMessaggio
            messaggio.DataOraUltimaModifica_string = dataModifica
            _giasContext.Entry(messaggio).State = EntityState.Modified

            If testata IsNot Nothing Then
                testata.Stato_Manager = statoTestata
                testata.Data_Modifica = dataModifica
                _giasContext.Entry(testata).State = EntityState.Modified
            End If

            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_W.CD815AType_AggiornaStatoManager")
        End Try

    End Sub

    Public Sub ACCDAA_StoricoMessaggiDogane_Errori_Logga(ByVal entita As ACCDAA_W_StoricoMessaggiDogane_Errori, ByVal isNew As Boolean)

        Try
            If isNew Then
                _giasContext.ACCDAA_W_StoricoMessaggiDogane_Errori.Add(entita)
            End If

            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_W.ACCDAA_StoricoMessaggiDogane_Errori_Logga")
        End Try

    End Sub


    Public Sub Mov_Dettaglio_Tecnico_Extra_Aggiorna(ByVal entita As Mov_Dettaglio_Tecnico_Extra)
        Try
            entita.data_modifica = DateTime.Now
            _giasContext.Entry(entita).State = EntityState.Modified
            _giasContext.SaveChanges()
        Catch ex As Exception
            Throw RaiseDAlException(ex, "ACCDAA_W.Mov_Dettaglio_Tecnico_Extra_Aggiorna")
        End Try
    End Sub

End Class
