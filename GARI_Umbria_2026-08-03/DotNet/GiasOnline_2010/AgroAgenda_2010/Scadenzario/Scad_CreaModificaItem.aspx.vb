Imports AgroAgenda_2010.Resources
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreContabDAL

Public Class Scad_CreaModificaItem
    Inherits System.Web.UI.Page

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return Me.Master.PATH_GIASBASE
        End Get
    End Property

    Public Shared Function LeggiEntita(ByVal id_alert_entita As Integer) As DataTable
        Dim DT As New DataTable

        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim leggi As New AgronicaCoreScadenziario.Alert_Entita_R
            DT = leggi.Leggi(id_alert_entita, objParametri_Server)

        Catch ex As Exception

        End Try

        Return DT

    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiMacchina(ByVal piva As String, ByVal mac_cod As Integer) As RispostaStandard
        Dim DT As New DataTable
        Dim r As New RispostaStandard
        Dim Mac_Des_Completa As String = ""
        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            'Lettura Macchina
            Dim ObjMacchina = New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim dtMacchina As DataTable = ObjMacchina.Leggi(piva, mac_cod, False, "", "", "", "", "", 0, "", False, 0, "", True, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            If dtMacchina.Rows.Count > 0 Then
                Mac_Des_Completa = dtMacchina(0).Item("CLASS_DESC") & " - " & dtMacchina(0).Item("Mac_Des")
                If Trim(dtMacchina(0).Item("Targa")) <> "" Then
                    Mac_Des_Completa = Mac_Des_Completa & " - Targa: " & Trim(dtMacchina(0).Item("Targa"))
                End If
            End If

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = Mac_Des_Completa

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiRichiesteUMA(ByVal piva As String) As RispostaStandard
        Dim DT As New DataTable
        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim leggi As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
            DT = leggi.Leggi_Elenco2(piva, "", "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiRagione_Sociale(ByVal piva As String) As RispostaStandard
        Dim DT As New DataTable
        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim leggi As New AgronicaCoreAnagrafeDAL.Imprese_Read
            DT = leggi.EsisteRecordInTabellaImprese(piva, objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings With {
                .ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }

            Dim JArrayLista As New JArray()
            For Each dr In DT.Rows
                JArrayLista.Add(New JObject(New JProperty("piva", dr.Item("PIVA")), New JProperty("rag_soc", dr.Item("rag_soc"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Dim objParametri_Super_Server, objParametri_Server, objParametri_Utenti As AgronicaCoreParametri


    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.flag_pag_Scadenzario = True

        Dim bPermessiOk As Boolean = True
        Dim MessaggioErrrore As String = ""
        Dim Attivita_Cod As Integer
        Dim Permesso_Cod As Integer

        Response.Expires = 0
        hdId_Tipologia.Value = 0

        'Controllo se la sessione è ancora su
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If


        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))


        hdUsername.Value = Session("ASG_Utente_Username")
        hdPivaSuperUser.Value = objParametri_Super_Server.PivaSuperUser


        Master().Lbl_Titolo.Text = DirectCast(GetLocalResourceObject("Scadenzario"), String)

        If Not IsPostBack Then
            'i18n I messaggi all'interno di questo if sono tecnici, quindi non li traduco

            'Anna 23/05/22: Aggiunto componente kendoUpload, per caricamento di allegati multipli per singolo upload
            Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "UploadMultiploAllegati_Documentale", "", "", objParametri_Server)

            Dim Autorizzato As String = "true"
            If dt_Conf.Rows.Count > 0 Then
                Autorizzato = dt_Conf.Rows(0).Item("Valore")
            End If
            hf_UploadMultiploAllegatiAbilitato.Value = CBool(Autorizzato)

            Dim jsonParametri

            'Controllo se i parametri mi sono stati passati in querystring
            If Not IsNothing(Request.QueryString.Item("scadstr")) Then
                jsonParametri = Request.QueryString.Item("scadstr")
            Else
                Throw New Exception("Non è stato passato in QS nessun parametro")
                Exit Sub
            End If

            'Deserializzo il JSON che mi è stato passato
            Dim listaParametri = JsonConvert.DeserializeObject(jsonParametri)

            'Controllo che mi sia stato passato l'ID_Alert_Entita
            If Not IsNumeric(listaParametri("ID_Alert_Entita")) Then
                Throw New Exception("Non è stato passato nessun ID_Alert_Entita o questo non è un numero")
                Exit Sub
            End If

            'Controllo che mi sia stato passato l'ID_Elenco
            If Not IsNumeric(listaParametri("ID_Elenco")) Then
                Throw New Exception("Non è stato passato nessun ID_Elenco o questo non è un numero")
                Exit Sub
            End If


            hfId_Alert_Entita.Value = listaParametri("ID_Alert_Entita")
            hfId_Elenco.Value = listaParametri("ID_Elenco")


            'Controllo Tipo Operazione
            If Not IsNumeric(listaParametri("TipoOperazione")) Then
                hdTipoOperazione.Value = enum_Security_Operazione.Modifica
            Else
                hdTipoOperazione.Value = listaParametri("TipoOperazione")
            End If

            'Controllo se i parametri mi sono stati passati in querystring
            'Se area provenienza <> 0 ==> abbiamo aperto il documentale dal'esterno (UMA, AUDIT ...)
            If Not IsNumeric(listaParametri("area_provenienza")) OrElse listaParametri("area_provenienza") = 0 Then
                hdArea_Provenienza.Value = 0
            Else
                hdArea_Provenienza.Value = listaParametri("area_provenienza")
                Master.flag_MostraHeader = False
                Master.flag_MostraFooter = False
            End If

            'Controllo Richiesta_Cod
            If Not IsNumeric(listaParametri("Richiesta_Cod")) Then
                hdRichiesta_Cod.Value = 0
                hdId_Schema_Template.Value = 0
            Else
                hdRichiesta_Cod.Value = listaParametri("Richiesta_Cod")
                Master.flag_MostraHeader = False

                'Controllo Id_Schema_Template
                If Not IsNumeric(listaParametri("Id_Schema_Template")) Then
                    hdId_Schema_Template.Value = 0
                Else
                    hdId_Schema_Template.Value = listaParametri("Id_Schema_Template")
                End If

                'Controllo Id_Schema_Template
                If Not IsNumeric(listaParametri("Tipologia")) Then
                    hdId_Tipologia.Value = 0
                Else
                    hdId_Tipologia.Value = listaParametri("Tipologia")
                End If
            End If


            'Controllo Analisi_Testata_Cod
            If Not IsNumeric(listaParametri("Analisi_Testata_Cod")) Then
                hdAnalisi_Testata_Cod.Value = 0
            Else
                hdAnalisi_Testata_Cod.Value = listaParametri("Analisi_Testata_Cod")
                Master.flag_MostraHeader = False

                'Controllo Tipologia
                If Not IsNumeric(listaParametri("Tipologia")) Then
                    hdId_Tipologia.Value = 0
                Else
                    hdId_Tipologia.Value = listaParametri("Tipologia")
                End If

                'Controllo Data Scadenza Analisi
                If IsDBNull(listaParametri("Data_Scadenza_Analisi")) OrElse
                    listaParametri("Data_Scadenza_Analisi") = "31/12/2100" Then
                    hdData_Scadenza_Analisi.Value = ""
                Else
                    hdData_Scadenza_Analisi.Value = listaParametri("Data_Scadenza_Analisi")
                End If
            End If

            'Controllo Piva
            If Not IsNothing(listaParametri("Piva")) Then
                hdPiva.Value = listaParametri("Piva")
            Else
                hdPiva.Value = ""
            End If

            'Controllo Id_Agenda
            If Not IsNumeric(listaParametri("Id_Agenda")) Then
                hdIdAgenda.Value = 0
            Else
                hdIdAgenda.Value = listaParametri("Id_Agenda")

                'Controllo Tipologia
                If Not IsNumeric(listaParametri("Tipologia")) Then
                    hdId_Tipologia.Value = 0
                Else
                    hdId_Tipologia.Value = listaParametri("Tipologia")
                End If

                Master.flag_MostraHeader = False
            End If

            'Controllo Mat_Cod
            If Not IsNumeric(listaParametri("Mac_Cod")) Then
                hdMac_Cod.Value = 0
            Else
                hdMac_Cod.Value = listaParametri("Mac_Cod")

            End If


            'Controllo Ricetta_Operazione_Cod
            If Not IsNumeric(listaParametri("Ricetta_Operazione_Cod")) Then
                hdRicetta_Operazione_Cod.Value = 0
            Else
                hdRicetta_Operazione_Cod.Value = listaParametri("Ricetta_Operazione_Cod")
                Master.flag_MostraHeader = False
            End If



            'Controllo Cod_Contatto
            If IsNothing(listaParametri("Cod_Contatto")) Then
                hdCod_Contatto.Value = ""
            Else
                hdCod_Contatto.Value = listaParametri("Cod_Contatto")
                Master.flag_MostraHeader = False

                'Controllo Tipologia
                If Not IsNumeric(listaParametri("Tipologia")) Then
                    hdId_Tipologia.Value = 0
                Else
                    hdId_Tipologia.Value = listaParametri("Tipologia")
                End If
            End If

            'Controllo se i parametri mi sono stati passati in querystring
            If Not IsNothing(listaParametri("sito_provenienza")) Then
                hdSito_Provenienza.Value = listaParametri("sito_provenienza")
            Else
                hdSito_Provenienza.Value = ""
            End If

            'Controllo se i parametri mi sono stati passati in querystring
            'Agginto per Audit Checklist
            ' Ricavo tutti gli Allegati_Documenti_Cod collegati alla checlist, così da mostrare solo i documenti effettivi 
            If Not IsNothing(listaParametri("cod_Documenti")) Then
                hdxFiltroDocumenti.Value = listaParametri("cod_Documenti")
            Else
                hdxFiltroDocumenti.Value = ""
            End If



            Select Case hfId_Alert_Entita.Value

                Case -1 'Nuovo

                    'Controllo se i parametri mi sono stati passati in querystring
                    If Not IsNothing(Request.QueryString.Item("type")) Then
                        hdModalita.Value = Request.QueryString.Item("type")
                    Else
                        hdModalita.Value = ""
                    End If

                    ' Ricavo i Default sugli indici in caso di nuovo documento
                    If Not IsNothing(listaParametri("indici")) Then
                        hdIndici.Value = listaParametri("indici")
                    Else
                        hdIndici.Value = ""
                    End If

                Case Else

                    'Lettura del Tipo Di documento
                    Dim DT As New DataTable
                    DT = LeggiEntita(CInt(hfId_Alert_Entita.Value))
                    If DT.Rows.Count > 0 Then

                        Select Case CInt(DT(0)("ChkDocumento"))

                            Case 0

                                hdModalita.Value = "" 'Scadenza senza documento

                            Case 1

                                hdModalita.Value = "doc" 'Documento senza scadenza

                            Case 2


                                'Controllo se i parametri mi sono stati passati in querystring
                                If Not IsNothing(listaParametri("type")) Then
                                    hdModalita.Value = listaParametri("type")
                                Else
                                    hdModalita.Value = hdModalita.Value = "hybrid" 'Documento con Scadenza
                                End If



                        End Select


                    Else

                        'Eccezione
                        bPermessiOk = False
                        GoTo FinePermessi

                    End If

                    hdIndici.Value = ""

            End Select

            'Controllo se i workflow del documentale sono attivi, in tal caso non verrà visualizzata la DDL dello stato dell'allegato
            Dim ObjAudit_Impostazione As New AgronicaCoreAuditDAL.Audit_Impostazioni_R
            Dim DtImpostazioni As DataTable = ObjAudit_Impostazione.LeggiImpostazione(Enum_Audit_impostazione.Documentale_GestioneWorkFlow, "", "", MessaggioErrrore, objParametri_Server)


            hdWorkFlow_Abilitato.Value = DtImpostazioni.Rows.Count > 0


            'Pagina di origine
            hdPaginaRedirect.Value = ""
            hdPaginaRedirect_Codificata.Value = ""

            If hdModalita.Value = "doc" Then
                hdPaginaRedirect.Value = "./Scad_lista.aspx?type=doc"
            Else
                hdPaginaRedirect.Value = "./Scad_lista.aspx"
            End If

            If hdRichiesta_Cod.Value <> 0 Then

                'If hfId_Alert_Entita.Value = -1 Then

                '    'Nuova Richiesta Uma Carburanti --> ritorno alla pagina richiesta
                '    hdPaginaRedirect.Value = Request.QueryString.Item("origine_nc") & "?p=" & Stringa_Codifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder, Server) & "&rc=" & Stringa_Codifica(Request.QueryString("rc").ToString, AgroKey_EncoderDecoder, Server)

                'Else

                'Modifica Richiesta Uma Carburanti --> ritorno al filtrone
                hdPaginaRedirect.Value = hdPaginaRedirect.Value & "&p=" & hdPiva.Value & "&richiesta_cod=" & hdRichiesta_Cod.Value & "&id_schema_template=" & hdId_Schema_Template.Value & "&area_provenienza=7"


                'End If


            End If


            '============================================================================================================================
            'Controllo Permessi
            '----------------------------------------------------------------------------------------------------------------------------            
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

            Select Case UCase(hdModalita.Value)

                Case "" 'Scadenza                    

                    hdTitle.Value = AgronicaAgenda_2010.Scadenza
                    hdTitle_Scadenza.Value = AgronicaAgenda_2010.DataScadenza & ":" 'Obbligatorio
                    hdTitle_Allegato.Value = AgronicaAgenda_2010.Allegato 'Facoltativo

                    If hfId_Alert_Entita.Value = -1 AndAlso hfId_Elenco.Value = -1 Then
                        'Inserimento
                        Attivita_Cod = enum_Security_Attivita.Scadenzario_Inser
                        Permesso_Cod = hdTipoOperazione.Value

                    Else
                        'Modifica
                        Attivita_Cod = enum_Security_Attivita.Scadenzario_Lista
                        Permesso_Cod = hdTipoOperazione.Value
                    End If


                    'Controllo se l'utente ha i permessi per accedere
                    bPermessiOk = objPermessi.Controlla_Permessi_Utente(
                                  Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                  Attivita_Cod,
                                  Permesso_Cod,
                                  Date.Now, "", objParametri_Utenti)


                    'Controllo Permessi Allegato
                    hdAllegato_Permesso.Value = objPermessi.Controlla_Permessi_Utente(
                                  Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                  enum_Security_Attivita.Documentale_Lista,
                                  hdTipoOperazione.Value,
                                  Date.Now, "", objParametri_Utenti)


                Case "DOC" 'Documento

                    hdTitle.Value = AgronicaAgenda_2010.Documento
                    hdTitle_Scadenza.Value = AgronicaAgenda_2010.DataScadenza & ":" 'Facoltativo
                    hdTitle_Allegato.Value = AgronicaAgenda_2010.Allegato & "" 'Obbligatorio
                    hdAllegato_Permesso.Value = "True"

                    If hfId_Alert_Entita.Value = -1 AndAlso hfId_Elenco.Value = -1 Then
                        'Inserimento
                        Attivita_Cod = enum_Security_Attivita.Documentale_Inser
                        Permesso_Cod = hdTipoOperazione.Value
                    Else
                        'Modifica
                        Attivita_Cod = enum_Security_Attivita.Documentale_Lista
                        Permesso_Cod = hdTipoOperazione.Value
                    End If


                    'Controllo se l'utente ha i permessi per accedere
                    bPermessiOk = objPermessi.Controlla_Permessi_Utente(
                                  Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                  Attivita_Cod,
                                  Permesso_Cod,
                                  Date.Now, "", objParametri_Utenti)



                Case "HYBRID" 'Documento con Scadenza                    

                    hdTitle.Value = DirectCast(GetLocalResourceObject("DocumentoConScadenza"), String)
                    hdTitle_Scadenza.Value = AgronicaAgenda_2010.DataScadenza & ":" 'Facoltativo
                    hdTitle_Allegato.Value = AgronicaAgenda_2010.Allegato 'Facoltativo

                    If hfId_Alert_Entita.Value = -1 AndAlso hfId_Elenco.Value = -1 Then
                        'Inserimento
                        Attivita_Cod = enum_Security_Attivita.Scadenzario_Inser
                        Permesso_Cod = hdTipoOperazione.Value

                    Else
                        'Modifica
                        Attivita_Cod = enum_Security_Attivita.Scadenzario_Lista
                        Permesso_Cod = hdTipoOperazione.Value
                    End If


                    'Controllo se l'utente ha i permessi per accedere
                    bPermessiOk = objPermessi.Controlla_Permessi_Utente(
                                  Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                  Attivita_Cod,
                                  Permesso_Cod,
                                  Date.Now, "", objParametri_Utenti)


                    If bPermessiOk Then

                        'Controllo permessi Allegato
                        If hfId_Alert_Entita.Value = -1 AndAlso hfId_Elenco.Value = -1 Then
                            'Inserimento
                            Attivita_Cod = enum_Security_Attivita.Documentale_Inser
                            Permesso_Cod = hdTipoOperazione.Value
                        Else
                            'Modifica
                            Attivita_Cod = enum_Security_Attivita.Documentale_Lista
                            Permesso_Cod = hdTipoOperazione.Value
                        End If


                        'Controllo se l'utente ha i permessi per accedere
                        bPermessiOk = objPermessi.Controlla_Permessi_Utente(
                              Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                              Attivita_Cod,
                              Permesso_Cod,
                              Date.Now, "", objParametri_Utenti)



                    End If


                    'Controllo Permessi Allegato
                    hdAllegato_Permesso.Value = objPermessi.Controlla_Permessi_Utente(
                                  Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                  enum_Security_Attivita.Documentale_Lista,
                                  hdTipoOperazione.Value,
                                  Date.Now, "", objParametri_Utenti)

                Case Else 'Non riconosciuto

                    bPermessiOk = False

            End Select


            '===============================================================================================================
            'Permessi Validazione
            '---------------------------------------------------------------------------------------------------------------

            'Controllo Visibilità Validazione
            hdAllegato_Validazione_Visibilita.Value = objPermessi.Controlla_Permessi_Utente(
                                                        objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline,
                                                        enum_Security_Attivita.Documentale_Valid, enum_Security_Operazione.Lettura,
                                                        Date.Now, "", objParametri_Utenti)

            'Controllo Modifica Validazione
            hdAllegato_Validazione.Value = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Documentale_Valid, enum_Security_Operazione.Modifica,
                                            Date.Now, "", objParametri_Utenti)
            '===============================================================================================================


            'Controllo Permessi Storicizzazione
            hdAllegato_Permesso_Storicizzazione.Value = objPermessi.Controlla_Permessi_Utente(
                          Session("ASG_Utente_Username"), Session("ASG_IdServizio"),
                          enum_Security_Attivita.Documentale_Storicizzazione,
                          enum_Security_Operazione.Lettura,
                          Date.Now, "", objParametri_Utenti)



FinePermessi:

            If hdRichiesta_Cod.Value <> 0 OrElse hdAnalisi_Testata_Cod.Value <> 0 Then
                bPermessiOk = True
            End If



            If Not bPermessiOk Then
                Dim accessoNonConsentito As Boolean = False
                accessoNonConsentito = ApplicaControlloPermessi(Permesso_Cod, hfId_Elenco.Value)
                If accessoNonConsentito Then
                    Exit Sub
                End If
            End If
        Else

            '    'Popolo pannelli in base alla NC(passando l'oggetto, disegna quello e non legge da DB)
            '    nc = CType(Session("NC_CreaModificaItem_ObjNc"), NC_Testata)
            '    disegnaSchemaNC(nc)

        End If

        '============================================================================================================================

        'Imposto le variabili di ponte con il client
        If hdTipoOperazione.Value = 0 Then
            hf_UtenteAbilitatoScrittura.Value = False
        Else
            hf_UtenteAbilitatoScrittura.Value = bPermessiOk
        End If
    End Sub




    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Agenda(piva As String, id_agenda As Integer, raccoglitore_cod As Integer, id_budget As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            Dim leggi As New CDG_DAL_R
            r.RispostaStringa =
                leggi.Leggi_Agenda(piva, id_agenda, objParametri_Server, id_budget, raccoglitore_cod)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaAgenda_2010.ErroreDuranteOperazione_ & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Sub Scad_CreaModificaItem_Init(sender As Object, e As EventArgs) Handles Me.Init

    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    Private Function ApplicaControlloPermessi(ByVal tipoPermessoDaControllare As Boolean,
                                              ByVal ID_Elenco As Integer
                                              ) As Boolean

        Dim accessoNonConsentito As Boolean = False

        Select Case tipoPermessoDaControllare
            Case enum_Security_Operazione.Modifica '--- Utente senza permesso di modifica
                If ID_Elenco <> -1 Then
                    Dim messaggioErrore = "Non si hanno i permessi per modificare il documento. Impostata modalità di sola lettura."
                    CambioPaginaSolaLettura(messaggioErrore)
                Else
                    accessoNonConsentito = PaginaAccessoNonConsentito()
                    Return accessoNonConsentito
                End If
            Case enum_Security_Operazione.Lettura '--- Utente senza permesso di visualizzazione
                PaginaAccessoNonConsentito()
                Return accessoNonConsentito
        End Select


        ''--- Utente senza permesso di visualizzazione

        'If utenteAbilitatoLettura = False Then

        '    accessoNonConsentito = PaginaAccessoNonConsentito()
        '    Return accessoNonConsentito

        'End If

        ''--- Utente senza permesso di modifica

        'If utenteAbilitatoScrittura = False Then

        '    Select Case hdTipoOperazione.Value

        '        Case enum_TipoOperazioneDB.Modifica

        '            Dim messaggioErrore = "Non si hanno i permessi per modificare il documento. Impostata modalità di sola lettura."
        '            CambioPaginaSolaLettura(messaggioErrore)

        '        Case enum_TipoOperazioneDB.Scrittura

        '            accessoNonConsentito = PaginaAccessoNonConsentito()
        '            Return accessoNonConsentito

        '    End Select

        'End If

        Return accessoNonConsentito

    End Function

    Private Sub CambioPaginaSolaLettura(messaggioErrore As String)

        Dim modelloScript As String = "MessaggioErrore_Bootstrap('{0}', 'DIV_Messaggi');"

        Dim script = String.Format(modelloScript, messaggioErrore)


        Page.ClientScript.RegisterStartupScript(Me.GetType(),
                                        "ChangeInReadOnly",
                                        script,
                                        True)

        hdTipoOperazione.Value = enum_TipoOperazioneDB.Lettura

    End Sub


    Private Function PaginaAccessoNonConsentito()

        Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")

        Return True

    End Function

End Class