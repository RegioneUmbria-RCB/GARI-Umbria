Imports System.Globalization
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello
Imports AgronicaCoreScadenziario
Imports AgronicaCoreScadenziario_BIZ
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq



' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Alert
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Scrivi(ByVal objP_server As String,
                           ByVal objP_utenti As String,
                           ByVal strObjJSON As String
                           ) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.ALERT.Scrivi()"

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        'Aggiunto per Audit Checklist_
        'Per ogni nuovo documento creato (quindi anche multitipologia), mi salvo l'allegati_documenti_cod relativo,
        'da rispedire indietro alla ricerca documenti, per vedere anche i documenti appena aggiunti
        Dim newAllegati_Documenti_Cod As New List(Of String)
        Try

            'Scompatto il JSON
            Dim objJson As JObject = JObject.Parse(strObjJSON)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DTSalvaAllegato As New DataTable
            Dim SalvaAllegato As Integer = 0 'File System Default

            DTSalvaAllegato = objImpostazioni.Leggi2(2, objParametri_Server.SuperUserUsername, enum_Impostazioni_Utenti.SUPERUSER_DOCUMENTALE_SALVA_ALLEGATO_SU_DB, "", "", objParametri_Utenti)

            If Not IsNothing(DTSalvaAllegato) AndAlso DTSalvaAllegato.Rows.Count > 0 Then
                SalvaAllegato = CInt(DTSalvaAllegato.Rows(0).Item("Impostazione_Valore_1"))
            End If

            'SalvaAllegato = 1

            'Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            'Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
            '                                objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline,
            '                                enum_Security_Attivita.Scadenzario_Lista, enum_Security_Operazione.Modifica,
            '                                Date.Now, "", objParametri_Utenti)

            Dim UtenteAbilitatoScrittura = True

            If Not UtenteAbilitatoScrittura Then
                r.Errore = Gias.MancanzaPermessiPerCreareModificareScadenze
                Return r
            End If


            'ESTRAGGO I DATI DALL'OGGETTO JSON
            Dim ID_Area As Integer = 0
            Dim ID_Tipologia As Integer = objJson("id_tipologia")

            If IsNumeric(objJson("id_area")) Then
                ID_Area = objJson("id_area")
            Else
                Dim ObjTipologia = New Alert_Tipologia_R
                Dim dtTipologia As DataTable = ObjTipologia.Leggi(0, ID_Tipologia, "", False, objParametri_Server)
                ID_Area = dtTipologia.Rows(0).Item("Id_Area")
            End If

            Dim Piva As String = If(IsNothing(objJson("piva")) OrElse objJson("piva").ToString = "", "", objJson("piva"))
            Dim Sa_Cod As Integer = If(IsNothing(objJson("sa_cod")) OrElse objJson("sa_cod").ToString = "", 0, objJson("sa_cod"))
            Dim appezza As Integer = If(IsNothing(objJson("appezza")) OrElse objJson("appezza").ToString = "", 0, objJson("appezza"))
            Dim Cod_Contatto As String = If(IsNothing(objJson("cod_contatto")) OrElse objJson("cod_contatto").ToString = "", "", objJson("cod_contatto"))
            Dim Analisi_Testata_Cod As Integer = IIf(IsNothing(objJson("analisi_testata_cod")) OrElse objJson("analisi_testata_cod").ToString = "", 0, objJson("analisi_testata_cod"))
            Dim PC_Testata_Cod As Integer = If(IsNothing(objJson("pc_testata_cod")) OrElse objJson("pc_testata_cod").ToString = "", 0, objJson("pc_testata_cod"))
            Dim Pua_Cod As Integer = If(IsNothing(objJson("pua_cod")) OrElse objJson("pua_cod").ToString = "", 0, objJson("pua_cod"))
            Dim Richiesta_Cod As Integer = If(IsNothing(objJson("richiesta_cod")) OrElse objJson("richiesta_cod").ToString = "", 0, objJson("richiesta_cod"))
            Dim Id_Schema_Template As Integer = If(IsNothing(objJson("id_schema_template")) OrElse objJson("id_schema_template").ToString = "", 0, objJson("id_schema_template"))
            Dim ID_App As String = If(IsNothing(objJson("ID_App")) OrElse objJson("ID_App").ToString = "", "", objJson("ID_App"))
            Dim ChkStorico As Integer = If(IsNothing(objJson("chkstorico")) OrElse objJson("chkstorico").ToString = "", 0, objJson("chkstorico"))

            '==============================================================================================================================================================================
            'Controllo Permesso Upload
            '------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            Dim ObjPermessoUpload As New AgronicaCoreAuditBIZ.AuditCheckList

            Dim data_upload_permesso As DateTime = Now
            If Not IsNothing(objJson("data_upload")) Then
                If IsDate(objJson("data_upload").ToString) Then
                    data_upload_permesso = CDate(objJson("data_upload").ToString)
                End If
            End If

            Dim Upload_Errore As String = ObjPermessoUpload.VerificaPermessoUploadDocumento(Piva, -1, 0, ID_Tipologia, 0, data_upload_permesso, objParametri_Server, objParametri_Utenti)

            If Trim(Upload_Errore) <> "" Then
                r.Errore = Upload_Errore
                Return r
            End If
            '==============================================================================================================================================================================


            'Anna
            Dim MultiTipologia As String = If(IsNothing(objJson("multiTipologia")) OrElse objJson("multiTipologia").ToString = "", "", objJson("multiTipologia"))

            Dim Mac_Cod As Integer = 0 'If(IsNothing(objJson("mac_cod")) OrElse objJson("mac_cod").ToString = "", 0, objJson("mac_cod"))
            If (Not IsNothing(objJson("mac_cod")) AndAlso objJson("mac_cod").ToString <> "") Then
                Mac_Cod = objJson("mac_cod").ToString().Split("|")(2)
                Sa_Cod = objJson("mac_cod").ToString().Split("|")(1)
            End If

            'Recupero le chiavi
            Dim Id_Alert_Entita As Integer = If(IsNothing(objJson("id_alert_entita")) OrElse objJson("id_alert_entita").ToString = "", 0, objJson("id_alert_entita"))
            Dim Id_Elenco As Integer = If(IsNothing(objJson("id_elenco")) OrElse objJson("id_elenco").ToString = "", 0, objJson("id_elenco"))
            Dim Allegati_Documenti_Cod As Integer = If(IsNothing(objJson("allegati_documenti_cod")) OrElse objJson("allegati_documenti_cod").ToString = "", 0, objJson("allegati_documenti_cod"))

            'Determino se è l'allegato è stato modificato
            Dim bAllegato_Modificato As Boolean = True
            Dim bModificaOld As Boolean = False

            If Not IsNothing(objJson("ballegato_modificato")) Then
                bAllegato_Modificato = objJson("ballegato_modificato")
            Else
                bModificaOld = True 'Attivazione vecchia funzione di modifica
            End If

            Dim Allegati_Documenti_numero As String = objJson("num_documento")

            Dim Allegati_Documenti_Data As DateTime = AGRODATAINIZIO
            If Not IsNothing(objJson("data_documento")) Then
                If IsDate(objJson("data_documento").ToString) Then
                    Allegati_Documenti_Data = CDate(objJson("data_documento").ToString)
                End If
            End If


            Dim CompressoDaGIAS As Boolean = CBool(objJson("CompressoDaGIAS"))

            '========================================================================================================================
            'Campi non più usati
            '------------------------------------------------------------------------------------------------------------------------
            Dim data_rilascio As Date = AGRODATAINIZIO
            Dim ente_rilascio As String = ""
            '========================================================================================================================
            Dim Data_scadenza As Date = AGRODATAFINE

            'TODO: fare fix definitivo per data
            If Date.TryParseExact(objJson("data").ToString, "dd/MM/yyyy",
                               CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
                Data_scadenza = Date.ParseExact(objJson("data").ToString, "dd/MM/yyyy",
                                                CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
            End If

            Dim Descrizione_scadenza As String = objJson("testo")
            Dim Note As String = objJson("note")

            Dim file_name As String = objJson("nome_file")
            Dim file_allegato As String = objJson("file_allegato")

            Dim validazione_flag As String = objJson("validazione_flag")
            Dim username_upload As String = objJson("username_upload")
            Dim data_upload As DateTime = Now

            'Determino se è una scadenza o un documento
            Dim chkdocumento As Integer = 0
            Dim elenco_des As String = ""

            If Not IsNothing(file_name) AndAlso file_name <> "" Then
                chkdocumento = 1 'Documento
                elenco_des = Gias.Documento
                If Data_scadenza <> AGRODATAFINE Then
                    chkdocumento = 2 'Hybrid
                    elenco_des = Gias.DocumentoConScadenza
                End If
            Else
                chkdocumento = 0 'Scadenza
                elenco_des = Gias.Scadenza
            End If

            Dim EntitaxIndici As JArray
            EntitaxIndici = objJson("entitaxindici")

            Dim Analisi_Campione_Cod As Integer = 0
            Dim Campo_Cod As Integer = 0
            Dim COM As Integer = 0
            Dim FOGLIO As Integer = 0
            Dim ID_Agenda As Integer = If(IsNothing(objJson("id_agenda")) OrElse objJson("id_agenda").ToString = "", 0, objJson("id_agenda"))
            Dim Id_Imp As Integer = 0
            Dim NUMERO As Integer = 0
            Dim Programmazione_Entita_Cod As Integer = 0
            Dim PROV As Integer = 0
            Dim Ricetta_Operazione_cod As Integer = If(IsNothing(objJson("Ricetta_Operazione_Cod")) OrElse objJson("Ricetta_Operazione_Cod").ToString = "", 0, objJson("Ricetta_Operazione_Cod"))
            Dim Id_ImpresexParticelle As Integer = If(IsNothing(objJson("Id_ImpresexParticelle")) OrElse objJson("Id_ImpresexParticelle").ToString = "", 0, objJson("Id_ImpresexParticelle"))
            Dim SEZIONE As Integer = 0
            Dim SUBALTERNO As Integer = 0
            Dim TipoEntita_Cod As Integer = 0
            Dim MessaggioErrore As String = ""

            Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
            Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")

            'identifico il tipoentita_cod
            Dim objxTipoInput As New AgronicaCoreScadenziario.Tipo_Entita_Chiavi_R
            TipoEntita_Cod = objxTipoInput.GetTipoEntitaCod(ID_Tipologia, objParametri_Server,
                                                            Piva, Sa_Cod, appezza, Analisi_Testata_Cod,
                                                            PC_Testata_Cod, Pua_Cod,
                                                            Richiesta_Cod, ID_Agenda, Ricetta_Operazione_cod,
                                                            Id_ImpresexParticelle)

            'Preparazione Oggetto Entità
            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                .ChkDocumento = chkdocumento,
                .ChkStorico = ChkStorico,
                .analisi_campione_cod = Analisi_Campione_Cod,
                .Appezza = appezza,
                .Campo_Cod = Campo_Cod,
                .COM = COM,
                .FOGLIO = FOGLIO,
                .ID_Agenda = ID_Agenda,
                .Id_Imp = Id_Imp,
                .NUMERO = NUMERO,
                .Piva = Piva,
                .PivaSuperUser = objParametri_Server.PivaSuperUser,
                .Programmazione_Entita_Cod = Programmazione_Entita_Cod,
                .PROV = PROV,
                .Ricetta_Operazione_cod = Ricetta_Operazione_cod,
                .Id_ImpresexParticelle = Id_ImpresexParticelle,
                .Sa_Cod = Sa_Cod,
                .SEZIONE = SEZIONE,
                .SUBALTERNO = SUBALTERNO,
                .Cod_Contatto = Cod_Contatto,
                .TipoEntita_Cod = TipoEntita_Cod,
                .Analisi_Testata_Cod = Analisi_Testata_Cod,
                .PC_Testata_Cod = PC_Testata_Cod,
                .PUA_Cod = Pua_Cod,
                .Mac_Cod = Mac_Cod,
                .Richiesta_Cod = Richiesta_Cod,
                .Id_Schema_Template = Id_Schema_Template
            }



            'SE NE DEVO CREARE UNA NUOVA
            If Id_Alert_Entita <= 0 Then

                'Dim path As String = "" 'ViewState("PATH")
                Dim ret_Id_Elenco As Integer = -1

                'scrivo 
                Dim workFlow As String = ""


                'Rename dell'allegato
                Dim ObjAgenda_W As New AgronicaCoreContabDAL.Agenda_W
                file_name = ObjAgenda_W.RenameAllegato(Piva, ID_Agenda, file_name, objParametri_Server)


                Dim ObjAudit_Impostazione As New AgronicaCoreAuditDAL.Audit_Impostazioni_R
                Dim DtImpostazioni As DataTable = ObjAudit_Impostazione.LeggiImpostazione(Enum_Audit_impostazione.Documentale_GestioneWorkFlow, "", "", MessaggioErrore, objParametri_Server)

                If DtImpostazioni.Rows.Count > 0 Then
                    workFlow = CStr(DtImpostazioni.Rows(0).Item("Valore1"))
                End If

                Dim strerr As String = alert_W.Scrivi(ID_Tipologia,
                                                      objE,
                                                      Data_scadenza,
                                                      Descrizione_scadenza,
                                                      file_name,
                                                      Percorso,
                                                      AGRODATAINIZIO,
                                                      AGRODATAFINE,
                                                      objParametri_Server,
                                                      Note,
                                                      ret_Id_Elenco,
                                                      EntitaxIndici,
                                                      ente_rilascio,
                                                      validazione_flag,
                                                      username_upload,
                                                      data_upload,
                                                      file_allegato,
                                                      Allegati_Documenti_numero,
                                                      SalvaAllegato,
                                                      ID_App,
                                                      ID_Area:=ID_Area,
                                                      CompressoDaGIAS:=CompressoDaGIAS,
                                                      newAllegati_Documenti_Cod:=newAllegati_Documenti_Cod,
                                                      Piva:=Piva,
                                                      WorkFlow_Documentale:=workFlow,
                                                      Allegati_Documenti_Data:=Allegati_Documenti_Data)

                If strerr <> "" Then
                    r.Errore = strerr
                    Return r
                End If

                If MultiTipologia <> "" Then
                    Dim lista_MultiTipologia = Split(MultiTipologia, ",")
                    If lista_MultiTipologia.Count > 0 Then
                        For Each IDTipologia In lista_MultiTipologia
                            strerr &= alert_W.Scrivi(CInt(IDTipologia),
                                                     objE,
                                                     Data_scadenza,
                                                     Descrizione_scadenza,
                                                     file_name,
                                                     Percorso,
                                                     AGRODATAINIZIO,
                                                     AGRODATAFINE,
                                                     objParametri_Server,
                                                     Note,
                                                     ret_Id_Elenco,
                                                     EntitaxIndici,
                                                     ente_rilascio,
                                                     validazione_flag,
                                                     username_upload,
                                                     data_upload,
                                                     file_allegato,
                                                     Allegati_Documenti_numero,
                                                     SalvaAllegato,
                                                     ID_App,
                                                     ID_Area:=ID_Area,
                                                     CompressoDaGIAS:=CompressoDaGIAS,
                                                     newAllegati_Documenti_Cod:=newAllegati_Documenti_Cod,
                                                     Piva:=Piva,
                                                     WorkFlow_Documentale:=workFlow,
                                                     Allegati_Documenti_Data:=Allegati_Documenti_Data)
                        Next
                    End If

                End If

                Id_Elenco = ret_Id_Elenco

            Else

                'SE NE DEVO MODIFICARE UNA ESISTENTE
                Dim strerr As String = ""
                Select Case bModificaOld
                    Case True
                        'Gestione vecchia modifica (da conservare finché non viene soppresso la vecchia gestione)
                        strerr = alert_W.ModificaOld(ID_Tipologia, Id_Elenco, Data_scadenza, Descrizione_scadenza, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server, Note, Id_Alert_Entita, EntitaxIndici)
                    Case False
                        strerr = alert_W.Modifica(ID_Tipologia,
                                                  objE,
                                                  Id_Elenco, Id_Alert_Entita,
                                                  Allegati_Documenti_Cod,
                                                  Data_scadenza, Descrizione_scadenza,
                                                  file_name, Percorso,
                                                  AGRODATAINIZIO, AGRODATAFINE,
                                                  objParametri_Server,
                                                  Note,
                                                  EntitaxIndici,
                                                  validazione_flag,
                                                  username_upload, data_upload,
                                                  file_allegato, bAllegato_Modificato, Allegati_Documenti_numero,
                                                  SalvaAllegato,
                                                  ID_Area:=ID_Area,
                                                  CompressoDaGIAS:=CompressoDaGIAS,
                                                  newAllegati_Documenti_Cod:=newAllegati_Documenti_Cod,
                                                  Allegati_Documenti_Data:=Allegati_Documenti_Data)
                End Select

                If strerr <> "" Then
                    r.Errore = strerr
                    Return r
                End If

            End If

            'Agginto per Audit Checklist
            'Per ogni nuovo documento creato (quindi anche multitipologia), mi salvo l'allegati_documenti_cod relativo,
            'da rispedire indietro alla ricerca dcoumenti, per vedere anche i documenti appena aggiunti
            Dim newCod_Documenti_Str As String = ""
            If newAllegati_Documenti_Cod.Count > 0 Then
                newCod_Documenti_Str = String.Join(",", newAllegati_Documenti_Cod)
            End If

            r.RispostaStringa = String.Format(Gias.SalvataggioElemXCorretto, elenco_des) & "|" & newCod_Documenti_Str
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Modifica_Validazione(ByVal objP_server As String,
                                         ByVal objP_utenti As String,
                                         ByVal strObjJSON As String
                                         ) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.ALERT.Modifica_Validazione()"

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

        Try

            'Scompatto il JSON
            Dim objJson As JObject = JObject.Parse(strObjJSON)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'ESTRAGGO I DATI DALL'OGGETTO JSON
            Dim Allegati_Documenti_Doc As Integer = objJson("allegati_documenti_cod")
            Dim Validazione_Flag As Integer = objJson("validazione_flag")
            Dim Data_modifica As DateTime = Date.Now

            Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
            objDoc.Modifica_Validazione(Allegati_Documenti_Doc, Validazione_Flag, Data_modifica, objParametri_Server)


            r.RispostaStringa = Gias.ValidazioneModificataCorrettamente
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Carica_DatiApp(ByVal objP_server As String,
                                   ByVal objP_utenti As String
                                   ) As RispostaStandard

        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.ALERT.Carica_DatiApp()"

        Dim r As New RispostaStandard()
        Dim documentiDaImportare As Integer = 0
        Dim documentiImportati As Integer = 0
        Dim msgErr As New StringBuilder
        Dim strObjJSON As String
        Dim Dummy As Integer

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

        Try

            objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim App_Documento_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
            Dim App_Documento_W As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
            Dim App_Dati_R As New AgronicaCoreContabDAL.APP_Dati_R

            Dim dtApp As DataTable = App_Documento_R.LeggiApp_Documenti(True, "", "Data_Creazione Desc", objParametri_Server)

            If dtApp.Rows.Count > 0 Then

                documentiDaImportare = dtApp.Rows.Count

                For Each drApp As DataRow In dtApp.Rows

                    'Scrittura Documento
                    Dim myObj As New ObjAllegati_Documenti

                    myObj.ID_App = UtilityProvider.Agro_SQL_SaveText(drApp("ID"))
                    myObj.id_elenco = -1
                    myObj.id_alert_entita = -1
                    myObj.allegati_documenti_cod = 0
                    myObj.ballegato_modificato = True
                    myObj.piva = UtilityProvider.Agro_SQL_SaveText(drApp("Piva"))
                    myObj.sa_cod = 0
                    myObj.appezza = 0
                    'myObj.mac_cod = 0
                    myObj.cod_contatto = 0

                    myObj.analisi_testata_cod = 0
                    myObj.pc_testata_cod = 0
                    myObj.pua_cod = 0
                    myObj.num_documento = ""
                    myObj.testo = UtilityProvider.Agro_SQL_SaveText(drApp("Descrizione"))
                    myObj.data = Format(drApp("Data_Scadenza"), "dd/MM/yyyy")
                    myObj.nome_file = UtilityProvider.Agro_SQL_SaveText(drApp("NomeFile"))

                    Dim bytes As Byte() = DirectCast(drApp("FileAllegato"), Byte())
                    myObj.file_allegato = Convert.ToBase64String(bytes)

                    myObj.id_tipologia = Val(drApp("id_tipologia"))

                    myObj.note = UtilityProvider.Agro_SQL_SaveText(drApp("Note"))

                    myObj.validazione_flag = 0
                    myObj.username_upload = UtilityProvider.Agro_SQL_SaveText(drApp("username_creazione"))


                    '=======================================================================================================================
                    'Verifica se si tratta di visita (verrà effettuato un rename del file)
                    '-----------------------------------------------------------------------------------------------------------------------
                    Dim Arrayp() As String
                    Arrayp = Split(myObj.ID_App, "|")
                    If UBound(Arrayp) > 0 Then

                        Dim dtApp_Dati As DataTable = App_Dati_R.Leggi_DatiAPP(Arrayp(0), "", False, "", "", objParametri_Server)

                        If dtApp_Dati.Rows.Count > 0 Then
                            myObj.id_agenda = dtApp_Dati(0)("Riferimento")
                        End If

                    End If
                    '=======================================================================================================================

                    'Serializzazione
                    Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                    strObjJSON = JsonConvert.SerializeObject(myObj, Formatting.None, serializerSettings)

                    r = Scrivi(objP_server, objP_utenti, strObjJSON)

                    If r.Errore = "" Then
                        documentiImportati += 1
                    Else
                        msgErr.Append(r.Errore.ToOrigin(objParametri_Server) & "<br>")
                    End If

                    'Aggiornamento Campi App_Documenti (TODO: valutare se svuotare il campo FileAllegato se importazione OK)
                    Dummy = App_Documento_W.Modifica_ImportazioneApp(myObj.ID_App, r.Errore.ToOrigin(objParametri_Server), Date.Now, objParametri_Server)

                Next

                r.RispostaStringa = "Sono stati importati correttamente " & documentiImportati & " documenti su " & documentiDaImportare

                If msgErr.Length > 0 Then
                    r.RispostaStringa &= "<br><br>ERRORI:<br>" & msgErr.ToString
                End If

            Else

                r.RispostaStringa = "Nessun documento da importare"

            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function DocumentiScrivi_APP(
        ByVal objP_super_server As String,
        ByVal objP_server As String,
        ByVal objP_utenti As String,
        ByVal Documento As DocumentoPerScarico
        ) As RispostaStandard

        Dim result As New RispostaStandard

        Try

            'Prelevo l'obj Parametri
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)
            Dim unid As String = Documento.guid
            Dim xScritturaApp As New Allegati
            result = xScritturaApp.DocumentiScrivi_APP(Documento, unid, objParametri_Server, objParametri_Utenti)

            Dim importazione As Boolean = False

            ' importazione dati app
            If result.RispostaOK AndAlso importazione Then

                Dim App_Documento_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
                Dim App_Documento_W As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

                Dim dtApp As DataTable = App_Documento_R.LeggiApp_Documenti(False, " ID LIKE '%" & unid & "%' ", "", objParametri_Server)

                For Each drApp As DataRow In dtApp.Rows

                    'Scrittura Documento
                    Dim myObj As New ObjAllegati_Documenti With {
                    .ID_App = UtilityProvider.Agro_SQL_SaveText(drApp("ID")),
                    .id_elenco = -1,
                    .id_alert_entita = -1,
                    .allegati_documenti_cod = 0,
                    .ballegato_modificato = True,
                    .piva = UtilityProvider.Agro_SQL_SaveText(drApp("Piva")),
                    .sa_cod = 0,
                    .appezza = 0,
                    .cod_contatto = 0,
                    .analisi_testata_cod = 0,
                    .pc_testata_cod = 0,
                    .pua_cod = 0,
                    .num_documento = "",
                    .testo = UtilityProvider.Agro_SQL_SaveText(drApp("Descrizione")),
                    .data = Format(drApp("Data_Scadenza"), "dd/MM/yyyy"),
                    .nome_file = UtilityProvider.Agro_SQL_SaveText(drApp("NomeFile")),
                    .file_allegato = Convert.ToBase64String(DirectCast(drApp("FileAllegato"), Byte())),
                    .id_tipologia = Val(drApp("id_tipologia")),
                    .note = UtilityProvider.Agro_SQL_SaveText(drApp("Note")),
                    .validazione_flag = 0,
                    .username_upload = UtilityProvider.Agro_SQL_SaveText(drApp("username_creazione"))
                }

                    'Serializzazione
                    Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
                    Dim strObjJSON = JsonConvert.SerializeObject(myObj, Formatting.None, serializerSettings)
                    Dim r = Scrivi(objP_server, objP_utenti, strObjJSON)

                    'Aggiornamento Campi App_Documenti (TODO: valutare se svuotare il campo FileAllegato se importazione OK)
                    App_Documento_W.Modifica_ImportazioneApp(myObj.ID_App, r.Errore.ToOrigin(objParametri_Server), Date.Now, objParametri_Server)

                Next

            End If

        Catch ex As Exception

            result.RispostaOK = False
            result.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return result

    End Function

    ' ##################################################################################################################################################################
    ' Anna 23/07/21: Aggiunta button per spostare documenti su DB
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function spostaAllegatiSuDatabase(ByVal objP_server As String,
                                             ByVal objP_utenti As String,
                                             ByVal filtro_area As String,
                                             ByVal filtro_tipologia As String
                                             ) As RispostaStandard
        Dim NomeRoutine As String = "AgronicaCoreScadenziario_BIZ.ALERT.spostaAllegatiSuDatabase()"

        Dim r As New RispostaStandard()
        Dim FileNonTrovatiCancellazione As String = ""
        Dim FileNonTrovati As String = ""
        Dim bFS As Boolean = True

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objDP As New DataProvider

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            Dim Filtro_Aggiuntivo As String = ""
            Dim chkdocumento As Integer = 1
            Dim chksoloattive As Integer = 0
            Dim Validita_Inizio As Date = AGRODATAINIZIO
            Dim Validita_Fine As Date = AGRODATAFINE

            Dim objElenco As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim dt As DataTable = objElenco.Leggi_Elenco(0, filtro_area, filtro_tipologia, "", True, False, 1, Filtro_Aggiuntivo, "", objParametri_Server, objParametri_Utenti, chkdocumento, chksoloattive, , Validita_Inizio, Validita_Fine)

            Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
            Dim PercorsoBase As String = dt_Conf.Rows(0).Item("Valore")
            PercorsoBase = FileSystemHelper.AggiungiSlashSeNonEsiste(PercorsoBase)
            Dim Percorso As String = ""
            Dim ElencoFileDaEliminareDaFS As New List(Of String)

            If dt.Rows.Count > 0 Then

                For Each File As DataRow In dt.Rows
                    'Controllo Tipo Salvataggio
                    If Not IsDBNull(File.Item("File_Allegato_DB")) Then
                        If File.Item("File_Allegato_DB").ToString = "System.Byte[]" Then
                            'Salvataggio su DB
                            bFS = False
                        End If
                    End If

                    If bFS Then

                        If File.Item("Sottocartella") <> "" Then
                            Percorso = FileSystemHelper.AggiungiSlashSeNonEsiste(PercorsoBase) & FileSystemHelper.AggiungiSlashSeNonEsiste(File.Item("Sottocartella"))
                        End If

                        Dim File_Name As String

                        If Not My.Computer.FileSystem.FileExists(File.Item("Allegati_Documenti_NomeFile")) Then
                            File_Name = FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso) & File.Item("Allegati_Documenti_NomeFile")
                        Else
                            File_Name = File.Item("Allegati_Documenti_NomeFile")
                        End If

                        Dim fileByteArray As Byte() = Nothing
                        Try
                            'Salvataggio su FS
                            fileByteArray = My.Computer.FileSystem.ReadAllBytes(File_Name)
                        Catch ex As Exception
                            FileNonTrovati &= "[" & ex.Message & "]. <br>"
                        End Try

                        If fileByteArray IsNot Nothing AndAlso fileByteArray.Count > 0 Then

                            'Aggiornamento 
                            Dim ObjDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

                            Dim allegati_documenti_piva As String
                            Dim allegati_documenti_cod As String

                            allegati_documenti_piva = File.Item("Allegati_Documenti_Piva")
                            allegati_documenti_cod = File.Item("Allegati_Documenti_Cod")


                            Dim Risultato = False
                            Risultato = ObjDoc.Modifica_File_Allegato_DB(allegati_documenti_piva, allegati_documenti_cod, fileByteArray, objParametri_Server)

                            If Risultato AndAlso Not ElencoFileDaEliminareDaFS.Contains(File_Name) Then
                                'Aggiungo il File_Name alla lista di File che successivamente dovranno essere eliminati
                                'N.B. non li elimino direttamente il questo ciclo perché è capitato che un Allegati_Documenti_Cod sia associato a più Alert_Elenco,
                                'e dopo andrebbe in errore la lettura del file
                                ElencoFileDaEliminareDaFS.Add(File_Name)

                            End If

                        End If
                    End If
                    bFS = True
                Next

                'Elimino i File da FS che sono stati spostati su DB
                For Each File_Name As String In ElencoFileDaEliminareDaFS

                    Try
                        My.Computer.FileSystem.DeleteFile(File_Name)

                    Catch ex As Exception
                        FileNonTrovatiCancellazione &= "File non cancellato: " & (File_Name) & " [" & ex.Message & "]" & ","
                    End Try

                Next

                r.RispostaStringa = FileNonTrovatiCancellazione

                If FileNonTrovati = "" Then
                    r.RispostaOK = True

                    If String.IsNullOrEmpty(FileNonTrovatiCancellazione) Then
                        r.RispostaStringa = "Allegati spostati su Database."
                    End If

                Else
                    r.RispostaOK = False
                    r.Errore = FileNonTrovati
                    objDP.Scrivi_LOG(objParametri_Server, NomeRoutine, FileNonTrovati)
                End If

                If Not String.IsNullOrEmpty(FileNonTrovatiCancellazione) Then
                    objDP.Scrivi_LOG(objParametri_Server, NomeRoutine, FileNonTrovatiCancellazione)
                End If

            Else

                r.Errore = Gias.FileNonTrovato

            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            objDP.Scrivi_LOG(objParametri_Server, NomeRoutine, ex.Message)
        End Try

        Return r

    End Function

End Class