Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreContabDAL
Imports AgronicaCoreModelsSTD.Widgets
Imports AgronicaCoreDTOStd.InData.Widgets
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class RicercaDocumenti
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiDocumenti(
            ByVal type As String,
            ByVal doc_type As String,
            ByVal tutteLeCausali As String,
            ByVal dettaglio As Boolean,
            ByVal piva As String,
            ByVal report As String,
            ByVal cubo As Boolean,
            ByVal _descrizione As String,
            ByVal _docNumeroSin As String,
            ByVal _docNumero As Integer,
            ByVal _docNumeroDes As String,
            ByVal _nrRiga As String,
            ByVal _dataRegDal As String,
            ByVal _dataRegAl As String,
            ByVal _centriAziendali As String,
            ByVal _clienti As String,
            ByVal _agenti As String,
            ByVal _causali As String,
            ByVal _specie As String,
            ByVal _varieta As String,
            ByVal _prodotti As String,
            ByVal _categorie As String,
            ByVal _categcommerciali As String,
            ByVal _soloDDTNonFatturati As Boolean,
            ByVal _soloOrdiniNonSpediti As Boolean,
            ByVal _modalitaFatturazione As Boolean,
            ByVal _utenteAbilitatoLettura As Boolean,
            ByVal _utenteAbilitatoScrittura As Boolean,
            ByVal objP_server As String,
            ByVal objP_utenti As String
            ) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim flagConnessione As Boolean = False

        Try
            Utility.VerificaApriConnessione(objParametri_Server, flagConnessione)
            Utility.VerificaApriConnessione(objParametri_Utenti, flagConnessione)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim _causali_trasp As String = ""
            'Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            'Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "Report_Vendite_Causali", "", "", objParametri_Server)
            'If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
            '    _causali_trasp = DTConfigSiti.Rows(0)("valore").ToString
            'End If

            Dim centriAziendaliAmmessi = ElencoCentriAziendaliAmmessi(piva, objParametri_Server)

            Dim hasGestioneWorkflow As Boolean

            Dim ricercaDoc As New RicercaDocumenti_R

            Dim dati = ricercaDoc.Cerca_Documenti(type,
                                                  doc_type,
                                                  tutteLeCausali,
                                                  centriAziendaliAmmessi,
                                                  dettaglio,
                                                  piva,
                                                  report,
                                                  cubo,
                                                  _descrizione,
                                                  _docNumeroSin,
                                                  _docNumero,
                                                  _docNumeroDes,
                                                  _nrRiga,
                                                  _dataRegDal,
                                                  _dataRegAl,
                                                  _centriAziendali,
                                                  _clienti,
                                                  _agenti,
                                                  _causali,
                                                  _specie,
                                                  _varieta,
                                                  _prodotti,
                                                  _categorie,
                                                  _categcommerciali,
                                                  _causali_trasp,
                                                  _soloDDTNonFatturati,
                                                  _soloOrdiniNonSpediti,
                                                  _modalitaFatturazione,
                                                  "",
                                                  objParametri_Server,
                                                  objParametri_Utenti,
                                                  hasGestioneWorkflow)

            If dati.Rows.Count > 0 Then

                Dim ObjRiscossioni As New AgronicaCoreContabDAL.Pagamenti_R
                Dim Flag_Riscosso As Boolean = False
                Dim Flag_NonRiscosso As Boolean = False
                Dim Flag_ParzialmenteRiscosso As Boolean = False
                Dim Tot_Importo_gia_Pagato As Decimal = 0
                Dim Tot_Importo_da_Pagare As Decimal = 0

                Dim handleGruppiUtenteStati As New AgronicaCoreUtentiBIZ.GruppiUtente_PermessiStato(objParametri_Server, objParametri_Utenti)
                Dim arrGruppiXUtente() = New Integer() {}

                If hasGestioneWorkflow AndAlso handleGruppiUtenteStati.IsUtenteSuperUser() = False Then

                    Dim handleGruppiUtente As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R()
                    Dim dtGruppiUtente = handleGruppiUtente.LeggiJoinGruppi(objParametri_Utenti.UtenteUsername, 0, "", "", objParametri_Utenti)

                    arrGruppiXUtente = dtGruppiUtente.AsEnumerable().Select(Function(permesso) permesso.Field(Of Integer)("Gruppi_Utente_Cod")).ToArray()

                    If arrGruppiXUtente.Length <> 1 Then
                        r.ErroriGias.Add(New ErroreGias() With {
                            .messaggio = "L'utente appartiene a più gruppi, di conseguenza non è possibile stabilirne i permessi sui singoli documenti. Modifica, cancellazione e avanzamenti di stato sono stati disabilitati",
                            .severity = ErroreGias_Severity.Warning,
                            .tipo = ErroreGias_Tipo.ConflittoPermessi
                        })
                    End If
                End If

                If (doc_type = "F" AndAlso Not dettaglio) OrElse (hasGestioneWorkflow AndAlso Not handleGruppiUtenteStati.IsUtenteSuperUser()) Then

                    If doc_type = "F" And Not dettaglio Then
                        dati.Columns.Add(New DataColumn("Stato_Pagamento", GetType(Integer)))
                        dati.Columns.Add(New DataColumn("Stato_Pagamento_Des", GetType(String)))
                    End If

                    If hasGestioneWorkflow AndAlso handleGruppiUtenteStati.IsUtenteSuperUser() = False Then
                        dati.Columns.Add(New DataColumn("Permesso_Modifica_Workflow", GetType(Boolean)))
                        dati.Columns.Add(New DataColumn("Permesso_Cancella_Workflow", GetType(Boolean)))
                    End If

                    For Each dr As DataRow In dati.Rows

                        If doc_type = "F" And Not dettaglio Then
                            Select Case dr("Lav_Cod")

                                Case 1001, 1000, 1053, 1002, 1003, 1055, 1058, 1070 'Fatture, Note

                                    '======================================================================================================
                                    'Lettura Riscossioni
                                    ObjRiscossioni.Riscosso_NonRiscosso_ParzialmenteRiscosso_byQuery(piva, 0, dr("Id_Agenda"), 0, dr("Lav_Cod"), dr("Importo"), Flag_Riscosso, Flag_NonRiscosso, Flag_ParzialmenteRiscosso, Tot_Importo_gia_Pagato, Tot_Importo_da_Pagare, "", "", objParametri_Server)

                                    If Flag_Riscosso Then

                                        dr("Stato_Pagamento") = 1 'Riscosso
                                        dr("Stato_Pagamento_Des") = "SI"

                                    ElseIf Flag_ParzialmenteRiscosso Then

                                        dr("Stato_Pagamento") = 2 'Parzialmente Riscosso
                                        dr("Stato_Pagamento_Des") = Tot_Importo_gia_Pagato
                                    Else

                                        dr("Stato_Pagamento") = 0 'Non Riscosso
                                        dr("Stato_Pagamento_Des") = "NO"

                                    End If
                                    '------------------------------------------------------------------------------------------------------
                                Case Else

                                    dr("Stato_Pagamento") = ""
                                    dr("Stato_Pagamento_Des") = ""

                            End Select
                        End If

                        If hasGestioneWorkflow AndAlso handleGruppiUtenteStati.IsUtenteSuperUser() = False Then

                            If arrGruppiXUtente.Length = 1 Then
                                Dim permessiUtenteStatoDoc = handleGruppiUtenteStati.LeggiPermessiStato(arrGruppiXUtente(0), dr.Field(Of Integer)("Pratica_Servizio_Cod"), dr.Field(Of Integer)("Pratica_Stato_Cod"))
                                permessiUtenteStatoDoc = handleGruppiUtenteStati.DeterminaPermessiStatoUtente(permessiUtenteStatoDoc, _utenteAbilitatoLettura, _utenteAbilitatoScrittura)

                                dr("Permesso_Modifica_Workflow") = permessiUtenteStatoDoc.Modifica
                                dr("Permesso_Cancella_Workflow") = permessiUtenteStatoDoc.Cancella
                            Else
                                dr("Permesso_Modifica_Workflow") = False
                                dr("Permesso_Cancella_Workflow") = False
                            End If

                            'Else
                            '    dr("Permesso_Modifica_Workflow") = True
                            '    dr("Permesso_Cancella_Workflow") = True
                        End If

                    Next
                End If

            End If

            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dati, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
            Utility.VerificaChiudiConnessione(objParametri_Utenti, flagConnessione)
        End Try

        Return r
    End Function



    '    Private Function ImpostaRiscossioni(ByVal Piva As String, ByVal Id_Agenda As Long, ByVal Lav_Cod As Long, ByVal importo As Double) As Integer
    '        Dim ImportoPagato As Decimal = 0
    '        Dim strPagamento As String = ""
    '        Dim ObjRiscossioni As New AgronicaCoreContabDAL.Pagamenti_R
    '        Dim DtRiscossioni As New DataTable

    '        Select Case Lav_Cod

    '            Case 1001, 1000, 1053, 1002, 1003, 1055, 1058, 1070 'Fatture, Note



    '                '======================================================================================================
    '                'Lettura Riscossioni
    '                ObjRiscossioni.Riscosso_NonRiscosso_ParzialmenteRiscosso_byQuery(Piva, 0, Id_Agenda, , Lav_Cod, importo, Flag_Riscosso, Flag_NonRiscosso, Flag_ParzialmenteRiscosso, Tot_Importo_gia_Pagato:=, Tot_Importo_da_Pagare:=, "", "", o)
    '                '------------------------------------------------------------------------------------------------------

    '                ImportoPagato = 0
    '                strPagamento = ""

    '                If RsRiscossioni.State <> 0 Then

    '                    'Filtro i Pagamenti Avvenuti
    '                    RsRiscossioni.Filter = "Previsto_Avvenuto = 1 "

    '                    Do While Not RsRiscossioni.EOF

    '                        If IsNumeric(RsRiscossioni("Importo")) Then

    '                            ImportoPagato = ImportoPagato + RsRiscossioni("Importo")
    '                            '
    '                            '                        On Error GoTo ByPass
    '                            '                        If Not IsNull(RsRiscossioni("Cau_Pagamento_Sigla")) Then
    '                            '
    '                            '                           strPagamento = strPagamento & "     " & RsRiscossioni("Cau_Pagamento_Sigla") & " in data " & RsRiscossioni("Data_Pagamento")
    '                            '
    '                            '                        End If
    '                            '
    '                        End If
    '                        'ByPass:
    '                        RsRiscossioni.MoveNext

    '                    Loop

    '                End If

    '                On Error GoTo GestioneErrore

    '                Select Case Riga

    '                    Case 0 'Ritorno un identificativo

    '                        If ImportoPagato = 0 Then
    '                            FormImpostaRiscossioni = 1
    '                        ElseIf CSng(ImportoPagato) = CSng(importo) Then
    '                            FormImpostaRiscossioni = 2
    '                        Else
    '                            FormImpostaRiscossioni = 3
    '                        End If

    '                    Case Else 'Imposto la griglia

    '                        If ImportoPagato = 0 And CDbl(MSDoc.TextMatrix(Riga, COL_IMPORTO)) <> 0 Then
    '                            MSDoc.TextMatrix(Riga, COL_SALDATA) = "NO"
    '                            MSDoc.TextMatrix(Riga, COL_SALDO_RESIDUO) = Format(MSDoc.TextMatrix(Riga, COL_IMPORTO), "##,###,###.00")
    '                            FormImpostaRiscossioni = 1
    '                        ElseIf CSng(importo) - CSng(ImportoPagato) <= (PAGAMENTO_TOLLERANZA / 100) Then
    '                            MSDoc.TextMatrix(Riga, COL_SALDATA) = "SI"
    '                            MSDoc.TextMatrix(Riga, COL_SALDO_RESIDUO) = Format(0, "##,###,###.00")
    '                            FormImpostaRiscossioni = 2
    '                        Else
    '                            MSDoc.TextMatrix(Riga, COL_SALDATA) = Format(ImportoPagato, "##,###,###.00")
    '                            MSDoc.TextMatrix(Riga, COL_SALDO_RESIDUO) = Format(CDbl(MSDoc.TextMatrix(Riga, COL_IMPORTO)) - ImportoPagato, "##,###,###.00")
    '                            FormImpostaRiscossioni = 3
    '                        End If

    '                        '===============================================================================================================
    '                        'Costruzione Note Pagamento
    '                        '---------------------------------------------------------------------------------------------------------------
    '                        If RsRiscossioni.State <> 0 Then

    '                            RsRiscossioni.Filter = "Note <> ''"

    '                            Do While Not RsRiscossioni.EOF

    '                                Note = IIf(Trim(Note) = "", "", "/ ") & Agro_SQL_SaveText(RsRiscossioni("Note"))

    '                                RsRiscossioni.MoveNext

    '                            Loop

    '                        End If

    '                        MSDoc.TextMatrix(Riga, COL_NOTE_PAGAMENTO) = Trim(Note)
    '                        '===============================================================================================================

    '                        '     MSDoc.TextMatrix(Riga, COL_TTT) = Trim(MSDoc.TextMatrix(Riga, COL_TTT) & "  " & Trim(strPagamento))

    '                End Select


    '                '####################################################################################################################
    '                '######################################### MODALITA DI PAGAMENTO ####################################################
    '                '####################################################################################################################

    '                If Riga > 0 Then

    '                    If RsRiscossioni.State <> 0 Then

    '                        Select Case FormImpostaRiscossioni

    '                            Case 0, 1 'Non Pagato --> Piano Pagamenti

    '                                RsRiscossioni.Filter = "Previsto_Avvenuto = 0"


    '                            Case Else 'Parziale,Saldato --> Pagamenti Eseguiti

    '                                RsRiscossioni.Filter = "Previsto_Avvenuto = 1"

    '                        End Select


    '                        ReDim Pagamenti(0 To 0)

    '                        Do While Not RsRiscossioni.EOF

    '                            'Controllo Duplicati
    '                            bDuplicato = False

    '                            For Indice = 0 To UBound(Pagamenti, 1) - 1

    '                                If Pagamenti(Indice) = RsRiscossioni("Cau_Pagamento") Then

    '                                    bDuplicato = True
    '                                    Exit For

    '                                End If

    '                            Next Indice

    '                            If Not bDuplicato Then

    '                                ReDim Preserve Pagamenti(0 To UBound(Pagamenti, 1) + 1)

    '                                Pagamenti(UBound(Pagamenti, 1) - 1) = RsRiscossioni("Cau_Pagamento")

    '                                'Costruzione Stringa Modalità Pagamento
    '                                Modalita_Pagamento = Modalita_Pagamento & FormCmbSeleziona_Descrizione(CmbPagamento, RsRiscossioni("Cau_Pagamento")) & ", "

    '                                'Costruzione Stringa Risorse Finanziarie
    '                                Select Case cPro_Costi.Attivo_Passivo(Lav_Cod)

    '                                    Case "Attivo"

    '                                        If InStr(1, Risorse_Finanziarie, FormCmbSeleziona_Descrizione(CmbRisorsa, RsRiscossioni("Cod_Liquidita_Dare"))) = 0 Then

    '                                            Risorse_Finanziarie = Risorse_Finanziarie & FormCmbSeleziona_Descrizione(CmbRisorsa, RsRiscossioni("Cod_Liquidita_Dare")) & ", "

    '                                        End If

    '                                    Case "Passivo"

    '                                        If InStr(1, Risorse_Finanziarie, FormCmbSeleziona_Descrizione(CmbRisorsa, RsRiscossioni("Cod_Liquidita_Avere"))) = 0 Then

    '                                            Risorse_Finanziarie = Risorse_Finanziarie & FormCmbSeleziona_Descrizione(CmbRisorsa, RsRiscossioni("Cod_Liquidita_Avere")) & ", "

    '                                        End If

    '                                End Select

    '                            End If

    '                            RsRiscossioni.MoveNext

    '                        Loop

    '                        If Trim(Modalita_Pagamento) <> "" Then

    '                            Modalita_Pagamento = Trim(Left(Modalita_Pagamento, Len(Modalita_Pagamento) - 2))

    '                            MSDoc.TextMatrix(Riga, COL_ALLEGATO_DES) = Modalita_Pagamento

    '                        End If


    '                        If Trim(Risorse_Finanziarie) <> "" Then

    '                            Risorse_Finanziarie = Trim(Left(Risorse_Finanziarie, Len(Risorse_Finanziarie) - 2))

    '                            MSDoc.TextMatrix(Riga, COL_RISORSA_DES) = Risorse_Finanziarie

    '                        End If


    '                    End If

    '                End If

    '                  '####################################################################################################################
    '                  '####################################################################################################################
    '                  '####################################################################################################################



    '            Case False

    '                FormImpostaRiscossioni = 0
    '                MSDoc.TextMatrix(Riga, COL_SALDATA) = "--"

    '        End Select


    '        'Distruggo gli oggetti
    '        Set cMagazzino = Nothing
    '        Set RsRiscossioni = Nothing

    '      End Select

    '        '----------------------------------------------------------------------------------------------------------
    '        Exit Function
    'GestioneErrore:
    '        Agro_GestioneErrore ClasseNome, MetodoNome, Err.Number, Err.Description, Agro_ErrSaveToLog + Agro_ErrShowMessage, LogFileName
    'End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function HasAttachment(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.documenti.AttachmentCheckParams)) As RispostaStandard
        Dim r As New RispostaStandard()
        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        Try
            Dim params As New ObjParams With {
                .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
                .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
                .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            }

            Dim alertReader As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim hasAttachments = alertReader.HasAttachment(
                params.ObjParametri_Server,
                piva:=InData.InData.Piva,
                idAgenda:=InData.InData.IdAgenda,
                macCod:=InData.InData.MacCod,
                ricettaCod:=InData.InData.RicettaCod,
                analisiTestataCod:=InData.InData.Analisi_Testata_Cod
            )

            r.RispostaOK = True
            r.RispostaStringa = hasAttachments
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

    Private Function ElencoCentriAziendaliAmmessi(ByVal Piva As String, ByRef objParametri As AgronicaCoreParametri) As IEnumerable(Of Object)


        Dim ddl_Centri As New DropDownList
        AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(ddl_Centri,
                                           False, "", "",
                                           Piva, False, 2,
                                           "", "", objParametri)


        Dim centriAziendali = New List(Of Object)

        For Each i As ListItem In ddl_Centri.Items
            centriAziendali.Add(New With
                                {
                                    .sa_nome = i.Text,
                                    .sa_cod = i.Value
                                })
        Next
        Return centriAziendali

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ListaContatti(
                                ByVal objP_server As String,
                                ByVal Piva As String,
                                ByVal Cod_Contatto As String,
                                ByVal Cod_RisUm As Integer,
                                ByVal FlagPubblico As Boolean,
                                ByVal ID_CF As Integer,
                                ByVal Cod_RisUm_Origine As Integer,
                                ByVal Piva_SuperUser_Origine As String,
                                ByVal Progressivo As String,
                                ByVal testoRicerca As String,
                                ByVal xOrderBy As String
                                ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim valoreFiltro As String

            Dim objFilters As JArray = Nothing
            If Not String.IsNullOrEmpty(testoRicerca) Then
                objFilters = JArray.Parse(testoRicerca)
                If Not objFilters Is Nothing Then
                    For Each parFiltro As JObject In objFilters
                        valoreFiltro = parFiltro("value").ToString()
                    Next
                End If
            End If

            Dim filtroAggiuntivo As String = If(String.IsNullOrEmpty(valoreFiltro), "",
                String.Format(" (Contatti.Rag_Soc Like '%{0}%' OR Contatti.Nome Like '%{0}%' or Contatti.Cognome Like '%{0}%' ) ", valoreFiltro))

            Dim obj As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtContatti As DataTable = obj.Leggi_Distinct_Contatti2(
                                                    Piva, Cod_Contatto, Cod_RisUm,
                                                    FlagPubblico, ID_CF, Cod_RisUm_Origine, Piva_SuperUser_Origine, Progressivo,
                                                    filtroAggiuntivo, xOrderBy, objParametriServer)

            Dim jArrayListaOp As New JArray()

            For Each dr As DataRow In dtContatti.Rows

                Dim partitaIva As String = ""
                Dim codContatto = Trim(dr.Item("Cod_Contatto"))
                Dim contatto_id_cf = dr.Item("Id_cf")

                If Not codContatto.StartsWith("-") Then
                    partitaIva = codContatto
                End If


                Dim ragSoc As String = Trim(CStr(dr.Item("Rag_Soc")) & CStr(dr.Item("Cognome")) & " " & CStr(dr.Item("Nome")))

                jArrayListaOp.Add(New JObject(New JProperty("nome", ragSoc),
                                              New JProperty("cod_contatto", codContatto),
                                              New JProperty("id_cf", contatto_id_cf)))

            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Top_N_Documenti(ByVal InData As Object) As rispostaStandard(Of List(Of WidgetAcquisto))

        Dim r As New rispostaStandard(Of List(Of WidgetAcquisto))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
        Dim params As CoreWS_Generic(Of Widget_Acquisti_IN) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_Acquisti_IN))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(params.objP.objP_utenti)
        Dim flagConnessione As Boolean = False
        Dim piva As String = params.InData.Piva
        Dim topNRows As Integer = 15
        Dim risultati As New List(Of WidgetAcquisto)
        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim dati As DataTable = Nothing

        Try
            Utility.VerificaApriConnessione(objParametri_Server, flagConnessione)
            Utility.VerificaApriConnessione(objParametri_Utenti, flagConnessione)

            Dim centriAziendaliAmmessi = ElencoCentriAziendaliAmmessi(piva, objParametri_Server)
            Dim ricercaDoc As New RicercaDocumenti_R

            If Not IsNothing(params.InData.NumeroMovimenti) Then
                topNRows = params.InData.NumeroMovimenti
            End If

            Dim elencoCausali = New List(Of CausaleDocumento) From
                {
                     New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "F", .LAV_COD = "1000", .LAV_DES = "FatturaRicevuta"},
                     New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "F", .LAV_COD = "1002", .LAV_DES = "NotaAccreditoRicevuta"},
                     New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = "1025", .LAV_DES = "DDTRicevuto"},
                     New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = "1075", .LAV_DES = "DistintaDiCarico"},
                     New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "C", .LAV_COD = "1077", .LAV_DES = "AutoDDTEmesso"},
                     New CausaleDocumento With {.TYPE = "A", .DOC_TYPE = "O", .LAV_COD = "2004", .LAV_DES = "OrdineDiAcquisto"}
                }
            Dim xOrderBy As String = " Data_Movimento DESC, Numero_Movimento DESC, Riga ASC "
            Dim causaliAmmesse As String = JsonConvert.SerializeObject(elencoCausali)

            Dim permessoLetturaDDT = ObjUtenti.Controlla_Permessi_Utente(
                        objParametri_Utenti.UtenteUsername, 5,
                        enum_Security_Attivita.Consegne_Acquisto, enum_Security_Operazione.Lettura, Date.Now, "", objParametri_Utenti)

            ' 1) Prima DDT Non fatturati
            If permessoLetturaDDT Then

#Region "Chiamate precedente commentata"
                'dati = ricercaDoc.Cerca_Documenti("A",
                '                                  "C",
                '                                  causaliAmmesse,
                '                                  centriAziendaliAmmessi,
                '                                  True,
                '                                  piva,
                '                                  "",
                '                                  False,
                '                                  "",
                '                                  "",
                '                                  0,
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  True,
                '                                  False,
                '                                  False,
                '                                  xOrderBy,
                '                                  objParametri_Server,
                '                                  objParametri_Utenti,
                '                                  False,
                '                                  topNRows
                '                                  )
#End Region

                dati = ricercaDoc.CercaDocumentiPerPopolamentoWidget("A", "C", causaliAmmesse, centriAziendaliAmmessi, piva, "",
                                                                     "", objParametri_Server, topNRows)


                risultati.AddRange(MappaDatiQuery(dati))
            End If

            Dim permessoLetturaFatture = ObjUtenti.Controlla_Permessi_Utente(
                        objParametri_Utenti.UtenteUsername, 5,
                        enum_Security_Attivita.Fatture_Acquisto, enum_Security_Operazione.Lettura, Date.Now, "", objParametri_Utenti)

            ') 2 FATTURE
            If permessoLetturaFatture Then

#Region "Chiamata precedente commentata"
                'dati = ricercaDoc.Cerca_Documenti("A",
                '                                  "F",
                '                                  causaliAmmesse,
                '                                  centriAziendaliAmmessi,
                '                                  True,
                '                                  piva,
                '                                  "",
                '                                  False,
                '                                  "",
                '                                  "",
                '                                  0,
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  "",
                '                                  False,
                '                                  False,
                '                                  False,
                '                                  xOrderBy,
                '                                  objParametri_Server,
                '                                  objParametri_Utenti,
                '                                  False,
                '                                  topNRows
                '                                  )
#End Region

                dati = ricercaDoc.CercaDocumentiPerPopolamentoWidget("A", "F", causaliAmmesse, centriAziendaliAmmessi, piva, "",
                                                                     "", objParametri_Server, topNRows)

                risultati.AddRange(MappaDatiQuery(dati))
            End If

            Dim risultatiOrdinati = risultati.OrderByDescending(Function(ris) ris.DataDoc).Take(topNRows).ToList

            r.RispostaStringa = risultatiOrdinati
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        Finally
            Utility.VerificaChiudiConnessione(objParametri_Server, flagConnessione)
            Utility.VerificaChiudiConnessione(objParametri_Utenti, flagConnessione)
        End Try

        Return r
    End Function

    Private Function MappaDatiQuery(ByVal dati As DataTable) As List(Of WidgetAcquisto)

        Dim result As New List(Of WidgetAcquisto)

        If Not IsNothing(dati) AndAlso dati.Rows.Count > 0 Then
            Dim expandoAcquisti = dati.ToExpandoObject
            For Each acq In expandoAcquisti

                Dim Des_lib As String = If(acq("Des_Lib") Is DBNull.Value, "", acq("Des_Lib"))
                Dim Lav_cod As Integer = If(acq("Lav_Cod") Is DBNull.Value, 0, CInt(acq("Lav_Cod")))
                Dim Sa_cod As Integer = If(acq("Sa_Cod") Is DBNull.Value, 0, CInt(acq("Sa_Cod")))
                Dim Fornitore As String = If(acq("Soggetto_RagioneSociale") Is DBNull.Value, "", acq("Soggetto_RagioneSociale"))
                Dim Data_doc As DateTime = If(acq("Data_Documento") Is DBNull.Value, DateTime.Now, Convert.ToDateTime(acq("Data_Documento")))
                Dim Doc_Numero_Sin As String = If(acq("Doc_Numero_Sin") Is DBNull.Value, "", acq("Doc_Numero_Sin"))
                Dim Doc_Numero As String = If(acq("Doc_Numero") Is DBNull.Value, "", acq("Doc_Numero"))
                Dim Doc_Numero_Des As String = If(acq("Doc_Numero_Des") Is DBNull.Value, "", acq("Doc_Numero_Des"))
                Dim Prodotto As String = If(acq("Mat_Des") Is DBNull.Value, "", acq("Mat_Des"))
                Dim Udm_Sim As String = If(acq("Unita_Misura_Sigla") Is DBNull.Value, "", acq("Unita_Misura_Sigla"))
                Dim Qta As Decimal = If(acq("Qta") Is DBNull.Value, "", CDec(acq("Qta")))
                Dim PrezzoNetto As Decimal = If(acq("Prezzo_Netto") Is DBNull.Value, "", CDec(acq("Prezzo_Netto")))

                result.Add(
                            New WidgetAcquisto With
                            {
                                .Id_Agenda = CInt(acq("Id_Agenda")),
                                .Piva = acq("PIVA"),
                                .Des_Lib = Des_lib,
                                .Lav_Cod = Lav_cod,
                                .Sa_cod = Sa_cod,
                                .Fornitore = Fornitore,
                                .DataDoc = Data_doc,
                                .NrDoc = String.Format("{0} {1} {2}", Doc_Numero_Sin, Doc_Numero, Doc_Numero_Des),
                                .Prodotto = Prodotto,
                                .Udm_Sim = Udm_Sim,
                                .Qta = Qta,
                                .PrezzoNetto = PrezzoNetto
                            }
                        )
            Next
        End If

        Return result

    End Function

End Class