Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreScadenziario
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiDAL
Imports Newtonsoft.Json
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreUtility

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Alert_Elenco
    Inherits System.Web.Services.WebService


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Elenco_ToKendoGrid_New(ByVal objP_server As String,
                                                 ByVal objP_utenti As String,
                                                 ByVal piva As String,
                                                 ByVal filtro_area As String,
                                                 ByVal filtro_tipologia As String,
                                                 ByVal chkdocumento As Integer,
                                                 ByVal chksoloattive As Integer,
                                                 ByVal chkstorico As Integer,
                                                 ByVal richiesta_cod As Integer,
                                                 ByVal analisi_testata_cod As Integer,
                                                 ByVal mac_cod As Integer,
                                                 ByVal Validita_Inizio As String,
                                                 ByVal Validita_Fine As String,
                                                 ByVal id_agenda As Integer,
                                                 ByVal Stato_Validazione As Integer,
                                                 ByVal Utente_Upload As Integer,
                                                 ByVal Inizio_Upload As String,
                                                 ByVal Fine_Upload As String,
                                                 ByVal Ricetta_Operazione_Cod As Integer,
                                                 ByVal xFiltroDocumenti As String,
                                                 ByVal Cod_Contatto As String,
                                                 ByVal Indici As String
                                                 ) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim dr_search_Indici() As DataRow
        Dim dr_search_Elenco() As DataRow
        Dim strIndici As String = ""
        Dim id_tipologia As Integer = 0
        Dim bDT_Inizializzato(10) As Integer
        Dim Arrayp() As String
        Dim ArrayIndici(0 To 2, 0 To 0)
        Dim ArrayEst() As String
        Dim bDuplicato As Boolean = False
        Dim i As Integer = 0
        Dim Indice As Integer = 0
        Dim Indice_Des As String = ""
        Dim strFiltroEntita As String = ""
        Dim Filtro_Aggiuntivo As String = ""
        Dim Autorizzazione As Integer = 0
        Dim workflowAbilitato As Boolean = False

        Dim bEntita_Contatti_Presenti As Boolean = False
        Dim bEntita_Macchine_Presenti As Boolean = False
        Dim bUma_Carburanti_Presenti As Boolean = False
        Dim bAnalisi_Terreno_Presenti As Boolean = False
        'Anna 06/06/22: aggiunta colonna Riferimenti e Ricette alla griglia
        Dim bEntita_Agenda_Presenti As Boolean = False
        Dim bEntita_AgendaContabile_Presenti As Boolean = False
        Dim bEntita_Ricette_Presenti As Boolean = False

        Dim dt_imprese As DataTable
        Dim dt_contatti As DataTable
        Dim dt_specie As DataTable
        Dim dt_macchine As DataTable
        Dim dt_centri As DataTable
        Dim dt_campi As DataTable
        Dim dt_appezza As DataTable

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim ObjImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim ObjContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim ObjSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim ObjMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim ObjCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim ObjCampi As New AgronicaCoreAnagrafeDAL.Campi_R
            Dim ObjAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            workflowAbilitato = GetWorkflowAbilitato(objParametri_Server)

            'DT Sempre Utili
            dt_contatti = ObjContatti.LeggiDatiMinimi("", "", "", objParametri_Server, True)
            dt_macchine = ObjMacchine.Leggi2("", 0, 0, 0, "", True, "", "Mac_Des", objParametri_Server)

            'Lettura Elenco

            'Introdotto perchè se viene lanciata la ricerca dalla pagina dell'UMA,
            'controlla solo se l'utente ha visibilità sulla tabella Utenti_Visibilita
            Dim Controlla_Solo_Utenti_Visibilita As Boolean = False

            'Introdotto perchè se viene lanciata la ricerca da una pagina diversa dall'UMA,
            'controlla solo se l'utente ha visibilità sulla tabella Utenti_Visibilita_Appoggio
            Dim Controlla_Solo_Utenti_Visibilita_Appoggio As Boolean = False

            'Costruzione Filtro Aggiuntvo
            If richiesta_cod > 0 Then
                Filtro_Aggiuntivo = "Alert_Area.ID_Area = " & enum_ID_Area_Alert.UMA_Carburanti & " And Alert_Entita.Richiesta_Cod = " & richiesta_cod
                Controlla_Solo_Utenti_Visibilita = True
            End If

            If analisi_testata_cod > 0 Then
                Filtro_Aggiuntivo = "Alert_Area.ID_Area = " & enum_ID_Area_Alert.Analisi & " And  Alert_Tipologia.ID_Tipologia = " & enum_ID_Area_Tipologia.Analisi_terreno & " And Alert_Entita.Analisi_Testata_Cod = " & analisi_testata_cod
                Controlla_Solo_Utenti_Visibilita_Appoggio = True
            End If

            'DCA20260105 aggiunto il filtro per Mac_Cod
            If mac_cod > 0 Then
                Filtro_Aggiuntivo = "Alert_Entita.Mac_Cod = " & mac_cod
                Controlla_Solo_Utenti_Visibilita_Appoggio = True
            End If

            If id_agenda > 0 Then
                Filtro_Aggiuntivo = "Alert_Entita.TipoEntita_Cod = " & enum_TipoEntita.OperazioneDiAgenda & "  And Alert_Entita.ID_Agenda = " & id_agenda.ToString
                Controlla_Solo_Utenti_Visibilita_Appoggio = True
            End If

            If Not String.IsNullOrEmpty(filtro_area) AndAlso Not filtro_area.Contains(",") AndAlso CInt(filtro_area) = enum_ID_Area_Alert.UMA_Carburanti Then
                Controlla_Solo_Utenti_Visibilita = True
            End If

            If Ricetta_Operazione_Cod > 0 Then
                Filtro_Aggiuntivo = "Alert_Entita.TipoEntita_Cod = " & enum_TipoEntita.Ricetta & "  And Alert_Entita.Ricetta_Operazione_cod = " & Ricetta_Operazione_Cod.ToString
                Controlla_Solo_Utenti_Visibilita_Appoggio = True
            End If

            If xFiltroDocumenti <> "" Then
                'Agginto per Audit Checklist
                'Ricavo tutti gli Allegati_Documenti_Cod collegati alla checlist, così da mostrare solo i documenti effettivi 
                Filtro_Aggiuntivo = "Allegati_Documenti.Allegati_Documenti_Cod IN (" & xFiltroDocumenti & ")"
                Controlla_Solo_Utenti_Visibilita_Appoggio = True
            End If

            If Cod_Contatto <> "" Then
                Filtro_Aggiuntivo = "Alert_Entita.TipoEntita_Cod = " & enum_TipoEntita.Contatto & "  And Alert_Entita.Cod_Contatto = '" & Cod_Contatto & "'"
                Controlla_Solo_Utenti_Visibilita_Appoggio = True
            End If

            Dim objElenco As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim Ordinamento As String = ""
            Dim Last_Id_Elenco As Integer = -1

            Ordinamento = "Id_Elenco, " &
                          "Autorizzato_Tipologia Desc, " &
                          "Autorizzato Asc, " &
                          "Autorizzato_Padre_Tipologia Desc, " &
                          "Autorizzato_Padre Asc, " &
                          "Autorizzato_Nonno_Tipologia Desc, " &
                          "Autorizzato_Nonno Asc, " &
                          "Autorizzato_Bis_Nonno_Tipologia Desc, " &
                          "Autorizzato_Bis_Nonno Asc, " &
                          "Autorizzato_Tris_Nonno_Tipologia Desc, " &
                          "Autorizzato_Tris_Nonno, " &
                          "Autorizzato_Quad_Nonno_Tipologia Desc, " &
                          "Autorizzato_Quad_Nonno "

            Dim dt As DataTable = objElenco.Leggi_Elenco(0,
                                                         filtro_area,
                                                         filtro_tipologia,
                                                         piva,
                                                         True,
                                                         False,
                                                         1,
                                                         Filtro_Aggiuntivo,
                                                         Ordinamento,
                                                         objParametri_Server,
                                                         objParametri_Utenti,
                                                         chkdocumento,
                                                         chksoloattive,
                                                         chkstorico,
                                                         Validita_Inizio,
                                                         Validita_Fine,
                                                         Controlla_Solo_Utenti_Visibilita:=Controlla_Solo_Utenti_Visibilita,
                                                         Controlla_Solo_Utenti_Visibilita_Appoggio:=Controlla_Solo_Utenti_Visibilita_Appoggio,
                                                         Stato_Validazione:=Stato_Validazione,
                                                         Utente_Upload:=Utente_Upload,
                                                         Inizio_Upload:=Inizio_Upload,
                                                         Fine_Upload:=Fine_Upload,
                                                         workflow_Documentale:=workflowAbilitato,
                                                         estraiFileAllegatoDB:=False,
                                                         Indici:=Indici)

            If dt.Rows.Count > 0 Then

                'Inserimento Campo Autorizzazione
                dt.Columns.Add(New DataColumn("Autorizzazione", GetType(Integer)))
                dt.Columns.Add(New DataColumn("Duplicato", GetType(Integer)))

                'Costruzione Filtro Entita
                For Each dr_entita As DataRow In dt.Rows

                    strFiltroEntita = strFiltroEntita & IIf(strFiltroEntita = "", "", ", ") & dr_entita.Item("Id_Alert_Entita")

                    'Controllo Presenza Entità Contatti
                    If CStr(dr_entita.Item("Cod_Contatto")) <> "" Then

                        If Not bEntita_Contatti_Presenti Then
                            dt.Columns.Add(New DataColumn("Contatto", GetType(String)))
                        End If

                        dr_search_Elenco = dt_contatti.Select("Cod_Contatto = '" & dr_entita.Item("Cod_Contatto") & "' ")

                        If dr_search_Elenco.Length > 0 Then
                            dr_entita.Item("Contatto") = dr_search_Elenco(0)("Rag_Soc")
                        End If

                        bEntita_Contatti_Presenti = True

                    End If

                    'Controllo Presenza Entità Macchine
                    If CInt(dr_entita.Item("Mac_Cod")) <> 0 Then

                        If Not bEntita_Macchine_Presenti Then
                            dt.Columns.Add(New DataColumn("Macchina", GetType(String)))
                        End If

                        dr_search_Elenco = dt_macchine.Select("Mac_Cod = " & dr_entita.Item("Mac_Cod") & " ")

                        If dr_search_Elenco.Length > 0 Then
                            dr_entita.Item("Macchina") = dr_search_Elenco(0)("Mac_Des")
                        End If

                        bEntita_Macchine_Presenti = True

                    End If

                    'Controllo Presenza Richiesta_Cod (Uma_Carburanti)
                    If CLng(dr_entita.Item("Richiesta_Cod")) <> 0 Then

                        bUma_Carburanti_Presenti = True

                    End If

                    'Controllo Presenza Analisi_Testata_Cod (Analisi del Terreno)
                    If CLng(dr_entita.Item("Analisi_Testata_Cod")) <> 0 Then

                        bAnalisi_Terreno_Presenti = True

                    End If

                    'Anna 06/06/22: aggiunta colonna Riferimenti e Ricette alla griglia
                    'Controllo Presenza Entità Agenda
                    If CInt(dr_entita.Item("ID_Agenda")) <> 0 Then

                        bEntita_Agenda_Presenti = True

                        Select Case CInt(dr_entita.Item("Lav_Cod_DocContabili"))
                            Case 1031, 'Fatture Attive da Sistema Esterno
                                 1025, 1054, 1076, 1078, 'Fatture Passive da Sistema Esterno
                                 2004, 'Ordini Acquisto
                                 1025, 'DDT Ricevuti
                                 1054, 1076, 1078, 'Conferimenti
                                 1031, 'DDT Emessi
                                 2002, 'Ordine Vendita
                                 1000, 'Fatture Passive
                                 1001 'Fatture Attive
                                bEntita_AgendaContabile_Presenti = True
                        End Select
                    End If

                    'Controllo Presenza Entità Ricette
                    If CInt(dr_entita.Item("Ricetta_Cod")) <> 0 Then

                        bEntita_Ricette_Presenti = True

                    End If

                Next

                strFiltroEntita = "Id_Alert_Entita In (" & strFiltroEntita & ")"

                Dim objIndici As New AgronicaCoreScadenziario.Alert_Indice_R
                Dim dt_indici As DataTable = objIndici.LeggiEntitaxIndici("", 0, strFiltroEntita, objParametri_Server)

                'Inserimento Indici Presenti in Struttura di Appoggio

                'Inserimento elenco indici documentali
                If dt_indici.Rows.Count > 0 Then

                    'Inserimento Colonna Riepilogo
                    dt.Columns.Add(New DataColumn("Indici", GetType(String)))

                    For Each dr As DataRow In dt.Rows

                        If Last_Id_Elenco = -1 Or Last_Id_Elenco <> dr("Id_Elenco") Then

                            Last_Id_Elenco = dr("Id_Elenco")

                            id_tipologia = dr.Item("Id_Tipologia")

                            'Correzione Data Scadenza
                            If IsDate(dr.Item("Data_Scadenza")) Then
                                If CDate(dr.Item("Data_Scadenza")) = AGRODATAFINE Then
                                    dr.Item("Data_Scadenza") = ""
                                End If
                            End If

                            strIndici = ""

                            If dt_indici.Rows.Count > 0 Then

                                dr_search_Indici = dt_indici.Select("Id_Tipologia = " & id_tipologia & " And Id_Alert_Entita = " & dr.Item("Id_Alert_Entita"))

                                For Each dr_indici As DataRow In dr_search_Indici

                                    'Controllo Presenza Indice Tra le Colonne
                                    bDuplicato = False
                                    Indice_Des = ""

                                    For i = 0 To UBound(ArrayIndici, 2) - 1

                                        If ArrayIndici(2, i) = dr_indici("Id_Indice") Then

                                            Indice = i
                                            bDuplicato = True
                                            Exit For

                                        End If
                                    Next

                                    If Not bDuplicato Then

                                        'Inserimento in struttura contententi gli indici utilizzati
                                        ReDim Preserve ArrayIndici(0 To 2, 0 To UBound(ArrayIndici, 2) + 1)

                                        ArrayIndici(0, UBound(ArrayIndici, 2) - 1) = "i" & Replace(dr_indici("Id_Indice"), "-", "_")
                                        ArrayIndici(1, UBound(ArrayIndici, 2) - 1) = dr_indici("TitoloIndice")
                                        ArrayIndici(2, UBound(ArrayIndici, 2) - 1) = dr_indici("Id_Indice")
                                        Indice = UBound(ArrayIndici, 2) - 1

                                        dt.Columns.Add(New DataColumn(ArrayIndici(0, Indice), GetType(String)))

                                    End If

                                    strIndici = strIndici & "<strong>" & dr_indici("TitoloIndice") & "</strong>:  "

                                    'Costruzione Descrizione
                                    Select Case dr_indici("TipoCampo")

                                        Case 0 'Testo Libero

                                            Indice_Des = dr_indici("Valore_Des")

                                            Select Case dr_indici("TipoDato")

                                                Case "string", "numeric", "date"

                                                    strIndici = strIndici & Indice_Des & "<br>"

                                                Case "boolean"

                                                    'Mostro nella griglia di Ricerca "Sì/No" al posto di "True/False"
                                                    If Indice_Des = "True" OrElse Indice_Des = True Then
                                                        Indice_Des = "Sì"
                                                    Else
                                                        Indice_Des = "No"
                                                    End If

                                                    strIndici = strIndici & Indice_Des & "<br>"

                                            End Select

                                        Case 1 'Valori

                                            Indice_Des = dr_indici("Valore")
                                            strIndici = strIndici & Indice_Des & "<br>"

                                        Case 2  'Elenco

                                            Select Case dr_indici("Elenco_Tipo")

                                                Case 1 'Imprese Gias

                                                    'Nota: inizializzazione per ottimizzazione letture
                                                    If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                        dt_imprese = ObjImprese.Leggi("", 0, "", "", objParametri_Server)
                                                        bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                                    End If

                                                    dr_search_Elenco = dt_imprese.Select("Piva = '" & dr_indici.Item("Elenco_Val") & "' ")

                                                    If dr_search_Elenco.Length > 0 Then

                                                        Indice_Des = dr_search_Elenco(0)("Rag_Soc")
                                                        strIndici = strIndici & Indice_Des & "<br>"

                                                    End If

                                                Case 2, 3 'Contatti Gias, Rapporti Contabili

                                                    dr_search_Elenco = dt_contatti.Select("Cod_Contatto = '" & dr_indici.Item("Elenco_Val") & "' ")

                                                    If dr_search_Elenco.Length > 0 Then

                                                        Indice_Des = dr_search_Elenco(0)("Rag_Soc")
                                                        strIndici = strIndici & Indice_Des & "<br>"

                                                    End If

                                                Case 4 'Specie Vegetali

                                                    If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                        dt_specie = ObjSpecie.Leggi(0, 0, "", "", 0, "", "", objParametri_Server)
                                                        bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                                    End If

                                                    dr_search_Elenco = dt_specie.Select("Veg_Cod = " & dr_indici.Item("Elenco_Val") & " ")

                                                    If dr_search_Elenco.Length > 0 Then

                                                        Indice_Des = dr_search_Elenco(0)("Veg_Des")
                                                        strIndici = strIndici & Indice_Des & "<br>"

                                                    End If

                                                Case 5 'Macchine

                                                    dr_search_Elenco = dt_macchine.Select("Mac_Cod = " & dr_indici.Item("Elenco_Val") & " ")

                                                    If dr_search_Elenco.Length > 0 Then

                                                        Indice_Des = dr_search_Elenco(0)("Mac_Des")
                                                        strIndici = strIndici & Indice_Des & "<br>"

                                                    End If

                                                Case 6 'Centri Aziendali

                                                    Arrayp = Split(dr_indici.Item("Elenco_Val"), "*")

                                                    If UBound(Arrayp) = 1 Then

                                                        If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                            dt_centri = ObjCentri.Anagrafica_Centri_Leggi("", 0, "", 0, "", "", objParametri_Server)
                                                            bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                                        End If

                                                        dr_search_Elenco = dt_centri.Select("Piva = '" & Arrayp(0) & "' And Sa_Cod = " & Arrayp(1) & " ")

                                                        If dr_search_Elenco.Length > 0 Then

                                                            Indice_Des = dr_search_Elenco(0)("Sa_Nome")
                                                            strIndici = strIndici & Indice_Des & "<br>"

                                                        End If

                                                    End If

                                                Case 7 'Campi

                                                    Arrayp = Split(dr_indici.Item("Elenco_Val"), "*")

                                                    If UBound(Arrayp) = 2 Then

                                                        If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                            dt_campi = ObjCampi.Leggi("", 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
                                                            bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                                        End If

                                                        dr_search_Elenco = dt_campi.Select("Piva = '" & Arrayp(0) & "' And Sa_Cod = " & Arrayp(1) & " And Campo_Cod = " & Arrayp(2) & " ")

                                                        If dr_search_Elenco.Length > 0 Then

                                                            Indice_Des = Trim(Left(dr_search_Elenco(0)("Sa_Nome"), 100)) & " - " & dr_search_Elenco(0)("Campo_Des")
                                                            strIndici = strIndici & Indice_Des & "<br>"

                                                        End If

                                                    End If

                                                Case 8 'Appezzamenti

                                                    Arrayp = Split(dr_indici.Item("Elenco_Val"), "*")

                                                    If UBound(Arrayp) = 3 Then

                                                        If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                            dt_appezza = ObjAppezza.Leggi("", 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
                                                            bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                                        End If

                                                        dr_search_Elenco = dt_appezza.Select("Piva = '" & Arrayp(0) & "' And Sa_Cod = " & Arrayp(1) & " And Appezza = " & Arrayp(3) & " ")

                                                        If dr_search_Elenco.Length > 0 Then

                                                            Indice_Des = Trim(Left(dr_search_Elenco(0)("Sa_Nome"), 100)) & " - " & dr_search_Elenco(0)("App_Nome")
                                                            strIndici = strIndici & Indice_Des & "<br>"

                                                        End If

                                                    End If

                                                Case Else

                                                    '.....

                                            End Select

                                    End Select

                                    'Correzione Indice
                                    dr.Item(ArrayIndici(0, Indice)) = Indice_Des

                                Next

                            End If

                            'Correzione Indici Riepilogo
                            dr.Item("Indici") = strIndici

                            '==============================================================================================================================
                            'Gestione Autorizzazioni
                            'Prendo il più basso con autorizzazione <> 0
                            '------------------------------------------------------------------------------------------------------------------------------

                            Autorizzazione = dr.Item("Autorizzato_Quad_Nonno")

                            If dr.Item("Autorizzato_Tris_Nonno") <> 0 Then
                                Autorizzazione = dr.Item("Autorizzato_Tris_Nonno")
                            End If

                            If dr.Item("Autorizzato_Bis_Nonno") <> 0 Then
                                Autorizzazione = dr.Item("Autorizzato_Bis_Nonno")
                            End If

                            If dr.Item("Autorizzato_Nonno") <> 0 Then
                                Autorizzazione = dr.Item("Autorizzato_Nonno")
                            End If

                            If dr.Item("Autorizzato_Padre") <> 0 Then
                                Autorizzazione = dr.Item("Autorizzato_Padre")
                            End If

                            If dr.Item("Autorizzato") <> 0 Then
                                Autorizzazione = dr.Item("Autorizzato")
                            End If

                            dr.Item("Autorizzazione") = Autorizzazione
                            dr.Item("Duplicato") = 0

                            '==============================================================================================================================

                        Else

                            'Duplicato --> da elimnare in js

                        End If

                    Next

                End If

                Last_Id_Elenco = -1

                'Nuovo Ciclo per sistemazione Allegati e Validazioni
                For Each dr As DataRow In dt.Rows

                    If Last_Id_Elenco = -1 Or Last_Id_Elenco <> dr("Id_Elenco") Then

                        Last_Id_Elenco = dr("Id_Elenco")

                        '==============================================================================================================================
                        'Gestione Autorizzazioni
                        'Prendo il più basso con autorizzazione <> 0
                        '------------------------------------------------------------------------------------------------------------------------------

                        Autorizzazione = dr.Item("Autorizzato_Quad_Nonno")

                        If dr.Item("Autorizzato_Tris_Nonno") <> 0 Then
                            Autorizzazione = dr.Item("Autorizzato_Tris_Nonno")
                        End If

                        If dr.Item("Autorizzato_Bis_Nonno") <> 0 Then
                            Autorizzazione = dr.Item("Autorizzato_Bis_Nonno")
                        End If

                        If dr.Item("Autorizzato_Nonno") <> 0 Then
                            Autorizzazione = dr.Item("Autorizzato_Nonno")
                        End If

                        If dr.Item("Autorizzato_Padre") <> 0 Then
                            Autorizzazione = dr.Item("Autorizzato_Padre")
                        End If

                        If dr.Item("Autorizzato") <> 0 Then
                            Autorizzazione = dr.Item("Autorizzato")
                        End If

                        dr.Item("Autorizzazione") = Autorizzazione
                        dr.Item("Duplicato") = 0

                        '==============================================================================================================================

                        If dr.Item("Allegati_Documenti_NomeFile") <> "" Then
                            'Determino Estensione in Caso di Campo non Valorizzato
                            If dr.Item("allegati_documenti_estensione") = "" Then
                                ArrayEst = Split(dr.Item("Allegati_Documenti_NomeFile"), ".")
                                If UBound(ArrayEst) > 0 Then
                                    dr.Item("allegati_documenti_estensione") = ArrayEst(1)
                                End If
                            End If
                        Else
                            'Annullo Possibilità di Validazione
                            dr.Item("Validazione_Flag") = -100 'Dummy
                            dr.Item("Validazione_Des") = ""
                        End If

                    Else

                        'Duplicato --> da elimnare in js
                        dr.Item("Duplicato") = 1

                    End If
                Next
            End If

            '===============================================================================================================================
            'Lettura dei Permessi
            '-------------------------------------------------------------------------------------------------------------------------------
            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
            Dim UtenteAbilitatoValidazione As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                        objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline,
                                                        enum_Security_Attivita.Documentale_Valid, enum_Security_Operazione.Modifica,
                                                        Date.Now, "", objParametri_Utenti)

            Dim UtenteAbilitatoValidazioneLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                                        objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasOnline,
                                                        enum_Security_Attivita.Documentale_Valid, enum_Security_Operazione.Lettura,
                                                        Date.Now, "", objParametri_Utenti)
            '===============================================================================================================================
            Dim l As New List(Of ColonneNome)

            If workflowAbilitato Then
                l.Add(New ColonneNome("Stato_Attuale", "Pratica Stato Attuale", "number") With {._hidden = True})
                l.Add(New ColonneNome("Descrizione_Stato", "Stato Attuale", "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            End If

            colonne_toKendoGrid(l, chkdocumento,
                                UtenteAbilitatoValidazioneLettura AndAlso Not workflowAbilitato,
                                UtenteAbilitatoValidazione,
                                bEntita_Contatti_Presenti,
                                bEntita_Macchine_Presenti,
                                bUma_Carburanti_Presenti,
                                bAnalisi_Terreno_Presenti,
                                bEntita_Agenda_Presenti,
                                bEntita_AgendaContabile_Presenti,
                                bEntita_Ricette_Presenti)

            For i = 0 To UBound(ArrayIndici, 2) - 1
                If ArrayIndici(1, i).ToString.Contains("Data") Then
                    l.Add(New ColonneNome(CStr(ArrayIndici(0, i)), ArrayIndici(1, i), "date"))
                Else
                    l.Add(New ColonneNome(CStr(ArrayIndici(0, i)), ArrayIndici(1, i), "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
                End If
            Next

            'Riepilogo
            If UBound(ArrayIndici, 2) > 0 Then
                l.Add(New ColonneNome("Indici", Gias.IndiciDocumentali, "string") With {._RemoveHtmlEncode = True})
            End If

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    Private Function GetWorkflowAbilitato(ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim MessaggioErrrore As String = ""
        Dim ObjAudit_Impostazione As New AgronicaCoreAuditDAL.Audit_Impostazioni_R
        Dim DtImpostazioni As DataTable = ObjAudit_Impostazione.LeggiImpostazione(Enum_Audit_impostazione.Documentale_GestioneWorkFlow, "", "", MessaggioErrrore, objParametri_Server)

        Return DtImpostazioni.Rows.Count > 0

    End Function

    Private Function colonne_toKendoGrid(ByRef l As List(Of ColonneNome), chkdocumento As Integer,
                                         UtenteAbilitatoValidazioneLettura As Boolean,
                                         UtenteAbilitatoValidazione As Boolean,
                                         bEntita_Contatti_Presenti As Boolean,
                                         bEntita_Macchine_Presenti As Boolean,
                                         bUma_Carburanti_Presenti As Boolean,
                                         bAnalisi_Terreno_Presenti As Boolean,
                                         bEntita_Agenda_Presenti As Boolean,
                                         bEntita_AgendaContabile_Presenti As Boolean,
                                         bEntita_Ricette_Presenti As Boolean
                                         ) As List(Of ColonneNome)

        'CAMPI NASCOSTI
        l.Add(New ColonneNome("ID_Tipologia", "ID Tipologia", "number") With {._hidden = True})
        l.Add(New ColonneNome("ID_Alert_Entita", "ID Alert Entita", "number") With {._hidden = True})
        l.Add(New ColonneNome("Allegati_Documenti_Cod", "Codice Allegato", "number") With {._hidden = IIf(UtenteAbilitatoValidazioneLettura, True, False)})

        l.Add(New ColonneNome("Gia_Passato", "Gia Passato", "number") With {._hidden = True})
        l.Add(New ColonneNome("ID_Elenco", "ID_Elenco", "number") With {._hidden = True})
        l.Add(New ColonneNome("ChkStorico", "ChkStorico", "number") With {._hidden = True})

        l.Add(New ColonneNome("Allegati_documenti_nomefile", "Allegati_documenti_nomefile", "string") With {._hidden = True})
        l.Add(New ColonneNome("Allegati_documenti_estensione", "Estensione", "string") With {._hidden = True})
        l.Add(New ColonneNome("CompressoDaGIAS", "CompressoDaGIAS", "string") With {._hidden = True})
        l.Add(New ColonneNome("sottocartella", "sottocartella", "string") With {._hidden = True})
        l.Add(New ColonneNome("ChkDocumento", "ChkDocumento", "number") With {._hidden = True})
        l.Add(New ColonneNome("Autorizzazione", "Autorizzazione", "number") With {._hidden = True})
        l.Add(New ColonneNome("Avanzamento_Richiesta", "Avanzamento Richiesta", "integer") With {._hidden = True})

        l.Add(New ColonneNome("Duplicato", "Duplicato", "integer") With {._hidden = True})
        l.Add(New ColonneNome("Pratica_Stato_Cod", "Stato Pratica", "number") With {._hidden = True})


        'CAMPI VISIBILI
        l.Add(New ColonneNome("Piva", Gias.PartitaIvaAbbr, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True, ._Display = False})
        l.Add(New ColonneNome("PivaReale", Gias.PartitaIvaAbbr, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Azienda", Gias.Azienda, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Categoria_1", Gias.CategoriaDocumento, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Categoria_2", Gias.TipologiaDocumento, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

        Dim Gias_Descrizione As String
        If chkdocumento = 1 Then '1 = doc, 0 = scad
            Gias_Descrizione = Gias.DescrizioneDocumento.ToString
        Else
            Gias_Descrizione = Gias.DescrizioneScadenza.ToString
        End If

        l.Add(New ColonneNome("Descrizione_Scadenza", Gias_Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Data_Scadenza", Gias.DataScadenza, "date"))

        'Anna 02/05/22: modificato campo Storico in DDL
        l.Add(New ColonneNome("Storico", Gias.Storicizzato, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True, ._Display = False})

        l.Add(New ColonneNome("Codice_Socio", Gias.CodiceSocio, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Indirizzo", Gias.IndirizzoCapComune, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Provincia", Gias.Provincia, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Regione", Gias.Regione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Stato", Gias.Stato, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

        l.Add(New ColonneNome("Note", Gias.Note, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True, ._Display = False})
        'l.Add(New ColonneNome("Username", "Username", "string") With {._hidden = True})
        'l.Add(New ColonneNome("Preavviso", "Preavviso", "number") With {._hidden = True})

        l.Add(New ColonneNome("Allegati_Documenti_Numero", Gias.nrAllegato, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Username_Upload", Gias.UsernameUpload, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        l.Add(New ColonneNome("Data_Upload", Gias.DataUpload, "date") With {._FormatoParticolare = "#=kendo.toString(Data_Upload, 'dd/MM/yyyy HH:mm')#"})

        'Inserimento colonna in caso di visibilità validazione
        If UtenteAbilitatoValidazioneLettura Then
            l.Add(New ColonneNome("Validazione_Flag", "Validazione_Flag", "number") With {._Editabile = UtenteAbilitatoValidazione, ._hidden = True})
            l.Add(New ColonneNome("Validazione_Des", Gias.Validazione, "string") With {._Editabile = UtenteAbilitatoValidazione, ._hidden = True, ._Filtrabile = True, ._FiltrabileConCheck = True})
        End If

        'l.Add(New ColonneNome("Colore", "Colore", "string") With {._hidden = True})

        'Inserimento Indici
        If bEntita_Contatti_Presenti Then
            l.Add(New ColonneNome("Contatto", Gias.Contatto, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Cod_Contatto", "Cod_Contatto", "string") With {._hidden = True})
        End If

        If bEntita_Macchine_Presenti Then
            l.Add(New ColonneNome("Macchina", Gias.Macchina, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Mac_Cod", "Mac_Cod", "number") With {._hidden = True})
        End If

        If bUma_Carburanti_Presenti Then
            l.Add(New ColonneNome("Pratica", Gias.Pratica, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        End If

        If bAnalisi_Terreno_Presenti Then
            l.Add(New ColonneNome("Analisi", Gias.Analisi, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Analisi_Testata_Cod", "Analisi_Testata_Cod", "number") With {._hidden = True})
        End If

        'Anna 06/06/22: aggiunta colonna Riferimenti e Ricette alla griglia
        If bEntita_Agenda_Presenti Then
            l.Add(New ColonneNome("Riferimento_Agenda", Gias.Riferimento, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("ID_Agenda", "ID_Agenda", "number") With {._hidden = True})

            l.Add(New ColonneNome("Descrizione_Operazione", Gias.Operazione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Data_Agenda", Gias.DataRiferimento, "date")) 'TO DO

            If bEntita_AgendaContabile_Presenti = True Then
                l.Add(New ColonneNome("Lav_Cod_DocContabili", "Lav_Cod_DocContabili", "number") With {._hidden = True})
                l.Add(New ColonneNome("Cod_Contatto_DocContabili", "Cod_Contatto_DocContabili", "string") With {._hidden = True})

                l.Add(New ColonneNome("Data_Movimento_DocContabili", Gias.DataRiferimento, "date"))
                l.Add(New ColonneNome("Rag_Soc_Contatto_DocContabili", Gias.ContattoRiferimento, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
                l.Add(New ColonneNome("Doc_Numero_Visualizzato_DocContabili", Gias.nrDocumento + " " + Gias.Riferimento, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            End If
        End If

        If bEntita_Ricette_Presenti Then
            l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("W_Anagrafica_Stati_Cod", "W_Anagrafica_Stati_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Ricetta_Operazione_Cod", "Ricetta_Operazione_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("Ricetta", Gias.RicettaBrogliaccio, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Codice_Ricetta", Gias.CodiceRicetta, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
        End If

        Return l

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_File_Allegato(ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal allegati_documenti_cod As Integer) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim bFS As Boolean = True

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'Lettura Elenco
            Dim handleAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim dt As DataTable = handleAllegati.Leggi_File_Da_AllegatoCod(piva, allegati_documenti_cod, objParametri_Server)

            If dt.Rows.Count > 0 Then

                Dim serializerSettings As New JsonSerializerSettings()
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None, serializerSettings)

                r.RispostaOK = True
            Else
                r.Errore = Gias.FileNonTrovato
            End If

        Catch ex As Exception
            r.Errore = Gias.FileNonTrovato
            Return r
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Lista_File_Allegati(ByVal objP_server As String,
                                              ByVal objP_utenti As String,
                                              ByVal listaAllegatiCod As String,
                                              ByVal fromAudit As Boolean) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim bFS As Boolean = True

        Dim returnObj As New returnObj
        Dim listaErrori As New List(Of String)
        Dim allegati_toRemove As New List(Of String)

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'Lettura Elenco

            Dim workflowAbilitato = GetWorkflowAbilitato(objParametri_Server)

            Dim objElenco As New AgronicaCoreScadenziario.Alert_Elenco_R

            returnObj.Dt = objElenco.Leggi_Allegato("",
                                                    0,
                                                    objParametri_Server,
                                                    " ad.Allegati_Documenti_Cod IN (" & listaAllegatiCod & ")",
                                                    True,
                                                    workflowAbilitato)

            For Each allegato In returnObj.Dt.Rows
                Dim prosegui = True
                'Controllo Tipo Salvataggio
                If Not IsDBNull(allegato.Item("File_Allegato_DB")) Then
                    If allegato.Item("File_Allegato_DB").ToString = "System.Byte[]" Then
                        'Salvataggio su DB
                        bFS = False
                    End If

                    'QUESTO CONTROLLO NON SERVE!! SE bFS = False, VUOL DIRE CHE SALVIAMO SU DB...IMPOSSIBILE CHE NON ESISTA IL FILE!
                    'Else
                    '    'Se il file non esiste, lo aggiungo alla lista degli errori da mostra alla fine del download
                    '    prosegui = False
                    '    allegati_toRemove.Add(allegato.Item("allegati_documenti_cod"))
                    '    returnObj.errori.Add(allegato.item("infoDocumento" + If(fromAudit, "Audit", "")))
                End If

                If bFS Then

                    Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
                    Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")
                    Percorso = FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso)

                    If allegato.Item("Sottocartella") <> "" Then
                        Percorso = FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso) & FileSystemHelper.AggiungiSlashSeNonEsiste(allegato.Item("Sottocartella"))
                    End If

                    Dim File_Name As String
                    File_Name = FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso) & allegato.Item("Allegati_Documenti_NomeFile")

                    Try
                        Dim fileByteArray As Byte()
                        'Salavataggio su FS
                        fileByteArray = My.Computer.FileSystem.ReadAllBytes(File_Name)

                        'Aggiornamento DT
                        allegato.Item("File_Allegato_DB") = fileByteArray
                    Catch ex As Exception
                        prosegui = False
                        allegati_toRemove.Add(allegato.Item("allegati_documenti_cod"))
                        returnObj.errori.Add(allegato.item("infoDocumento" + If(fromAudit, "Audit", "")))
                    End Try
                End If

                If prosegui Then
                    'Se il file è invalido o storicizzato aggiungo il corrispettivo tag davanti al nome del file
                    Dim InvalidStoricizzato As String = ""
                    If workflowAbilitato Then
                        If allegato.item("Stato_Attuale") = 404 OrElse allegato.item("Stato_Attuale") = 406 Then
                            InvalidStoricizzato += "[INVALID FILE]"
                        End If
                    Else
                        If allegato.item("Validazione_Flag") = -1 OrElse allegato.item("Validazione_Flag") = -2 Then
                            InvalidStoricizzato += "[INVALID FILE]"
                        End If
                    End If
                    If allegato.item("ChkStorico") = 1 Then
                        InvalidStoricizzato += "[ARCHIVED FILE]"
                    End If

                    If InvalidStoricizzato <> "" Then
                        allegato.Item("Allegati_Documenti_NomeFile") = InvalidStoricizzato + " " + allegato.Item("Allegati_Documenti_NomeFile")
                    End If
                End If
            Next

            If allegati_toRemove.Count > 0 Then
                Dim DrResult As Array = returnObj.Dt.Select("Allegati_Documenti_Cod IN ( " & String.Join(",", allegati_toRemove) & ")")
                For Each Dr In DrResult
                    returnObj.Dt.Rows.Remove(Dr)
                Next
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaStringa = JsonConvert.SerializeObject(returnObj, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = Gias.FileNonTrovato
            Return r
        End Try

        Return r
    End Function

    Private Class returnObj
        Public Property Dt As DataTable
        Public Property errori As New List(Of String)
    End Class

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Elenco_ToKendoGrid(ByVal objP_server As String, ByVal objP_utenti As String, ByVal filtro_area As String, ByVal filtro_tipologia As String) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim dr_search_Indici() As DataRow
        Dim dr_search_Elenco() As DataRow
        Dim strIndici As String = ""
        Dim id_tipologia As Integer = 0
        Dim bDT_Inizializzato(10) As Integer
        Dim Arrayp() As String

        Dim dt_imprese As DataTable
        Dim dt_contatti As DataTable
        Dim dt_specie As DataTable
        Dim dt_macchine As DataTable
        Dim dt_centri As DataTable
        Dim dt_campi As DataTable
        Dim dt_appezza As DataTable

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim ObjImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim ObjContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim ObjSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
            Dim ObjMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim ObjCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim ObjCampi As New AgronicaCoreAnagrafeDAL.Campi_R
            Dim ObjAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read

            Dim objElenco As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim dt As DataTable = objElenco.Leggi_Elenco(0, filtro_area, filtro_tipologia, "", True, False, 1, "", " Data_Scadenza_Data, Azienda", objParametri_Server, objParametri_Utenti)


            dt.Columns.Add(New DataColumn("Indici", GetType(String)))

            If dt.Rows.Count > 0 Then


                'Dim ObjUdm As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                'Dim dt_udm As DataTable = ObjUdm.Leggi(0, 0, "", "", 0, "", "Udm_Des", objParametri_Server)


                For Each dr As DataRow In dt.Rows

                    id_tipologia = dr.Item("Id_Tipologia")

                    'Inserimento elenco indici documentali
                    Dim objIndici As New AgronicaCoreScadenziario.Alert_Indice_R
                    Dim dt_indici As DataTable = objIndici.LeggiEntitaxIndici("", 0, "Id_Tipologia = " & id_tipologia, objParametri_Server)


                    strIndici = ""

                    If dt_indici.Rows.Count > 0 Then

                        'dr_search_Indici = dt_indici.Select("PivaSuperUser = '" & dr.Item("PivaSuperUser") & "' And " &
                        '                                    "Id_Alert_Entita = " & dr.Item("Id_Alert_Entita"))

                        dr_search_Indici = dt_indici.Select("Id_Alert_Entita = " & dr.Item("Id_Alert_Entita"))

                        For Each dr_indici As DataRow In dr_search_Indici

                            strIndici = strIndici & "<strong>" & dr_indici("TitoloIndice") & "</strong>: "

                            Select Case dr_indici("TipoCampo")

                                Case 0 'Testo Libero

                                    Select Case dr_indici("TipoDato")

                                        Case "string", "numeric", "date"

                                            strIndici = strIndici & dr_indici("Valore_Des") & "<br>"

                                        Case "boolean"

                                            strIndici = strIndici & dr_indici("Valore_Des") & "<br>"

                                    End Select


                                Case 1 'Valori

                                    strIndici = strIndici & dr_indici("Valore") & "<br>"

                                Case 2  'Elenco

                                    Select Case dr_indici("Elenco_Tipo")

                                        Case 1 'Imprese Gias

                                            If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                dt_imprese = ObjImprese.Leggi("", 0, "", "", objParametri_Server)
                                                bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                            End If

                                            dr_search_Elenco = dt_imprese.Select("Piva = '" & dr_indici.Item("Elenco_Val") & "' ")

                                            If dr_search_Elenco.Length > 0 Then

                                                strIndici = strIndici & dr_search_Elenco(0)("Rag_Soc") & "<br>"

                                            End If

                                        Case 2, 3 'Contatti Gias, Rapporti Contabili

                                            If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                dt_contatti = ObjContatti.LeggiDatiMinimi("", "", "", objParametri_Server, True)
                                                bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                            End If

                                            dr_search_Elenco = dt_contatti.Select("Cod_Contatto = '" & dr_indici.Item("Elenco_Val") & "' ")

                                            If dr_search_Elenco.Length > 0 Then

                                                strIndici = strIndici & dr_search_Elenco(0)("Rag_Soc") & "<br>"

                                            End If



                                        Case 4 'Specie Vegetali

                                            If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                dt_specie = ObjSpecie.Leggi(0, 0, "", "", 0, "", "", objParametri_Server)
                                                bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                            End If

                                            dr_search_Elenco = dt_specie.Select("Veg_Cod = " & dr_indici.Item("Elenco_Val") & " ")

                                            If dr_search_Elenco.Length > 0 Then

                                                strIndici = strIndici & dr_search_Elenco(0)("Veg_Des") & "<br>"

                                            End If

                                        Case 5 'Macchine

                                            If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                dt_macchine = ObjMacchine.Leggi2("", 0, 0, 0, "", True, "", "Mac_Des", objParametri_Server)
                                                bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                            End If

                                            dr_search_Elenco = dt_macchine.Select("Mac_Cod = " & dr_indici.Item("Elenco_Val") & " ")

                                            If dr_search_Elenco.Length > 0 Then

                                                strIndici = strIndici & dr_search_Elenco(0)("Mac_Des") & "<br>"

                                            End If

                                        Case 6 'Centri Aziendali

                                            Arrayp = Split(dr_indici.Item("Elenco_Val"), "*")

                                            If UBound(Arrayp) = 1 Then

                                                If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                    dt_centri = ObjCentri.Anagrafica_Centri_Leggi("", 0, "", 0, "", "", objParametri_Server)
                                                    bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                                End If

                                                dr_search_Elenco = dt_centri.Select("Piva = '" & Arrayp(0) & "' And Sa_Cod = " & Arrayp(1) & " ")

                                                If dr_search_Elenco.Length > 0 Then

                                                    strIndici = strIndici & dr_search_Elenco(0)("Sa_Nome") & "<br>"

                                                End If

                                            End If


                                        Case 7 'Campi

                                            Arrayp = Split(dr_indici.Item("Elenco_Val"), "*")

                                            If UBound(Arrayp) = 2 Then

                                                If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                    dt_campi = ObjCampi.Leggi("", 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
                                                    bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                                End If

                                                dr_search_Elenco = dt_campi.Select("Piva = '" & Arrayp(0) & "' And Sa_Cod = " & Arrayp(1) & " And Campo_Cod = " & Arrayp(2) & " ")

                                                If dr_search_Elenco.Length > 0 Then

                                                    strIndici = strIndici & dr_search_Elenco(0)("Sa_Nome") & " - " & dr_search_Elenco(0)("Campo_Des") & "<br>"

                                                End If

                                            End If


                                        Case 8 'Appezzamenti

                                            Arrayp = Split(dr_indici.Item("Elenco_Val"), "*")

                                            If UBound(Arrayp) = 3 Then

                                                If bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 0 Then

                                                    dt_appezza = ObjAppezza.Leggi("", 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
                                                    bDT_Inizializzato(dr_indici("Elenco_Tipo")) = 1

                                                End If

                                                dr_search_Elenco = dt_appezza.Select("Piva = '" & Arrayp(0) & "' And Sa_Cod = " & Arrayp(1) & " And Appezza = " & Arrayp(3) & " ")

                                                If dr_search_Elenco.Length > 0 Then

                                                    strIndici = strIndici & dr_search_Elenco(0)("Sa_Nome") & " - " & dr_search_Elenco(0)("App_Nome") & "<br>"

                                                End If

                                            End If


                                        Case Else

                                            '.....

                                    End Select


                            End Select


                        Next

                    End If

                    'Correzione Indici
                    dr.Item("Indici") = strIndici

                Next


            End If



            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Azienda", "Azienda", "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Categoria_1", "Categoria", "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Categoria_2", "Tipologia", "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("ID_Tipologia", "ID Tipologia", "number") With {._hidden = True})
            l.Add(New ColonneNome("ID_Alert_Entita", "ID Alert Entita", "number") With {._hidden = True})
            l.Add(New ColonneNome("Data_Scadenza", "Data Scadenza", "date"))
            l.Add(New ColonneNome("Gia_Passato", "Gia Passato", "number") With {._hidden = True})
            l.Add(New ColonneNome("ID_Elenco", "ID Elenco", "number") With {._hidden = True})
            l.Add(New ColonneNome("Descrizione_Scadenza", "Descrizione Scadenza", "string"))
            l.Add(New ColonneNome("Note", "Note", "string") With {._Display = False})
            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
            'l.Add(New ColonneNome("Username", "Username", "string") With {._hidden = True})
            'l.Add(New ColonneNome("Preavviso", "Preavviso", "number") With {._hidden = True})
            l.Add(New ColonneNome("sottocartella", "sottocartella", "string") With {._hidden = True})
            l.Add(New ColonneNome("Allegati_documenti_nomefile", "Allegati documenti nomefile", "string") With {._hidden = True})
            l.Add(New ColonneNome("Indici", "Indici Documentali", "string") With {._RemoveHtmlEncode = True})
            'l.Add(New ColonneNome("Colore", "Colore", "string") With {._hidden = True})


            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Importa_Scadenze(ByVal objP_server As String, ByVal ID_Tipologia_daImportare As String) As RispostaStandard

        Dim r As New RispostaStandard()
        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If ID_Tipologia_daImportare = "" Then
            r.Errore = "ID_Tipologia_daImportare non valorizzato"
            Return r
        End If

        Dim ArrayID_Tipologia As String() = ID_Tipologia_daImportare.Split("|")

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim msgErr As String = ""

        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W
        Dim esito As String = alert_W.Importa(objParametri_Server, ArrayID_Tipologia, msgErr)

        If msgErr = "" Then
            r.RispostaStringa = esito
            r.RispostaOK = True
        Else
            r.Errore = msgErr
        End If

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Cancella(ByVal objP_server As String, ByVal objP_utenti As String, ByVal id_elenco As String) As RispostaStandard

        Dim r As New RispostaStandard()
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
        Dim esito As String = ""
        Dim msgErr As String = ""

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

        Dim LeggiConfSiti As New Configurazione_Siti_R
        Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
        Dim path As String = dt_Conf.Rows(0).Item("Valore")
        'Dim path As String = ""


        Dim objW As New AgronicaCoreScadenziario_BIZ.Alert_W
        Dim res As Boolean = objW.Cancella(id_elenco, path, objParametri_Server)

        If res = False Then
            r.Errore = Gias.ErroreImpossibileCancellareScadenza
            Return r
        End If

        Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W
        res = mp_W.CancellaProgrammazioniFromChiave(objParametri_Server, enum_MailTipo.Scadenze_InScadenza, id_elenco)

        r.RispostaStringa = Gias.ScadenzaCancellataCorrettamente
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaMassivo(ByVal objP_server As String, ByVal objP_utenti As String, ByVal elenco As String) As RispostaStandard

        Dim r As New RispostaStandard()
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
        Dim esito As String = ""
        Dim msgErr As String = ""
        Dim ArrayP() As String
        Dim i As Integer = 0
        Dim res As Boolean = True

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                      AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)



        Dim objW As New AgronicaCoreScadenziario_BIZ.Alert_W
        Dim mp_W As New AgronicaCoreMailBIZ.Mail_Programmazione_W

        Dim LeggiConfSiti As New Configurazione_Siti_R
        Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
        Dim path As String = dt_Conf.Rows(0).Item("Valore")
        'Dim path As String = ""


        ArrayP = Split(elenco, ",")
        For i = 0 To UBound(ArrayP) - 1

            If IsNumeric(ArrayP(i)) Then

                If res Then
                    res = objW.Cancella(ArrayP(i), path, objParametri_Server)
                End If
                If res Then
                    res = mp_W.CancellaProgrammazioniFromChiave(objParametri_Server, enum_MailTipo.Scadenze_InScadenza, ArrayP(i))
                End If

            End If

        Next


        If res = False Then
            r.Errore = Gias.ErroreImpossibileCancellareScadenza
            Return r
        End If




        r.RispostaStringa = Gias.ScadenzaCancellataCorrettamente
        r.RispostaOK = True

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Aggiorna_Storicizzazione(ByVal objP_server As String, ByVal elenco As String, ByVal valore As Integer) As RispostaStandard

        Dim r As New RispostaStandard()
        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If elenco = "" Then
            r.Errore = "Documenti non selezionati"
            Return r

        Else
            'Formattazione
            If Right(Trim(elenco), 1) = "," Then
                elenco = Left(Trim(elenco), Len(Trim(elenco)) - 1)
            End If

        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim msgErr As String = ""

        Dim alert_W As New AgronicaCoreScadenziario.Alert_Entita_W
        Dim esito As String = alert_W.Storicizza(elenco, valore, objParametri_Server)

        If msgErr = "" Then
            r.RispostaStringa = esito
            r.RispostaOK = True
        Else
            r.Errore = msgErr
        End If

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function HasAttachment(inData As CoreWS_Generic(Of AgronicaCoreModelsSTD.documenti.AttachmentCheckParams)) As RispostaStandard
        Dim r As New RispostaStandard()
        If inData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If inData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        Try
            Dim params As New ObjParams With {
                .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(inData.objP.objP_utenti),
                .ObjParametri_Server = Utility.convertStringtoOBJparametri(inData.objP.objP_server),
                .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(inData.objP.objP_super_server)
            }

            Dim alertReader As New AgronicaCoreScadenziario.Alert_Elenco_R
            alertReader.HasAttachment(
                params.ObjParametri_Server,
                piva:=inData.InData.Piva,
                idagenda:=inData.InData.IdAgenda,
                maccod:=inData.InData.MacCod,
                ricettaCod:=inData.InData.RicettaCod,
                analisiTestataCod:=inData.InData.Analisi_Testata_Cod
            )

            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return r
    End Function

End Class