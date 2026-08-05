Imports System.Globalization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports Newtonsoft.Json.Linq
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports Newtonsoft.Json
Imports AgronicaCoreVarieDAL
Imports System.IO
Imports AgronicaCoreScadenziario
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreUtility
Imports Agronica.Helpers.CA

Public Class GestioneAllegati
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Public Operazione As Integer = enum_TipoOperazioneDB.Modifica
    Public objAllegati As New JObject()
    Public dati_Allegati As String = ""
    Public kendoServer As Boolean = False
    Public fromPatentiniContatto As Boolean = False

    ' Querystring
    Dim inModifica As Boolean = False
    Dim Qs_Piva As String
    Dim Qs_ID_Elenco As String
    Dim Qs_AnalisiTestataCod As String
    Dim Qs_IdArea As String
    Dim Qs_CodContatto As String
    Dim Qs_IdTipologia As String
    Dim Qs_DataScadenza As String = ""
    Dim Qs_DataRilascio As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Master.Lbl_Titolo.Text = "Gestione Allegati"
        Master.flag_MostraHeader = False
        Master.flag_MostraFooter = False

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        If Not IsNothing(Request.QueryString("piva")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("piva").ToString, AgroKey_EncoderDecoder)
        End If

        If Not IsNothing(Request.QueryString("a")) Then
            Qs_IdArea = Stringa_Decodifica(Request.QueryString("a").ToString, AgroKey_EncoderDecoder)
        End If

        If Not IsNothing(Request.QueryString("t")) Then
            Qs_IdTipologia = Stringa_Decodifica(Request.QueryString("t").ToString, AgroKey_EncoderDecoder)
        End If

        If Not IsNothing(Request.QueryString("o")) Then
            Operazione = Stringa_Decodifica(Request.QueryString("o").ToString, AgroKey_EncoderDecoder)
        End If

        If Not IsNothing(Request.QueryString("cod")) Then
            Qs_AnalisiTestataCod = Stringa_Decodifica(Request.QueryString("cod").ToString, AgroKey_EncoderDecoder)
        End If

        If Not IsNothing(Request.QueryString("c_contatto")) Then
            Qs_CodContatto = Stringa_Decodifica(Request.QueryString("c_contatto").ToString, AgroKey_EncoderDecoder)
        End If

        If Not IsNothing(Request.QueryString("dr")) Then
            Qs_DataScadenza = Stringa_Decodifica(Request.QueryString("dr").ToString, AgroKey_EncoderDecoder)
            If Qs_DataScadenza = "01/01/1900" Then
                Qs_DataScadenza = ""
            End If
        End If

        If Not IsNothing(Request.QueryString("ds")) Then
            Qs_DataRilascio = Stringa_Decodifica(Request.QueryString("ds").ToString, AgroKey_EncoderDecoder)
            If Qs_DataRilascio = "31/12/2100" Then
                Qs_DataRilascio = ""
            End If
        End If

        If Not IsNothing(Request.QueryString("fromPatentiniContatto")) AndAlso Request.QueryString("fromPatentiniContatto") = 1 Then
            fromPatentiniContatto = True
        End If

        If kendoServer Then
            dati_Allegati = carica_Kendo_Allegati(Qs_Piva, Qs_CodContatto, Qs_AnalisiTestataCod)
        End If

        Dim Num_Documento As String = ""
        Dim Ente_Rilascio As String = ""

        If Not String.IsNullOrEmpty(Qs_AnalisiTestataCod) AndAlso Qs_AnalisiTestataCod <> "0" Then
            Dim objTestataRead As New Analisi_Testata_R
            Dim dtTestata = objTestataRead.Leggi(CInt(Qs_AnalisiTestataCod), 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            Qs_DataRilascio = dtTestata.Rows(0).Item("Analisi_Testata_Data_Inizio")
            Qs_DataScadenza = dtTestata.Rows(0).Item("Analisi_Testata_Data_Fine")
            Dim objCertificatoRead As New Analisi_Certificato_Read
            Dim filtro_analisi As String = "Analisi_Testata_Cod = " & Qs_AnalisiTestataCod
            Dim dtCertificato = objCertificatoRead.LeggiConFiltroTestata(0, filtro_analisi, "", objParametri_Server)
            If dtCertificato.Rows.Count > 0 Then
                Num_Documento = dtCertificato.Rows(0).Item("Analisi_Certificato_Des")
                Dim Laboratorio As Integer = dtCertificato.Rows(0).Item("Analisi_Certificato_Laboratorio")
                If Laboratorio <> 0 Then
                    Dim objContattiRead As New Contatti_R
                    Dim dtLaboratorio = objContattiRead.Leggi(
                        objParametri_Server.PivaSuperUser, "", Laboratorio,
                        -8, True, False, 0, 0, False, 0, -99,
                        0, "", True, 0, 0, 0, 0, 0,
                        AGRODATAINIZIO, AGRODATAFINE,
                        False, "", "",
                        objParametri_Server)
                    If dtLaboratorio.Rows.Count > 0 Then
                        Ente_Rilascio = dtLaboratorio.Rows(0).Item("Rag_Soc")
                    End If
                End If
            End If
        End If

        objAllegati.Add(New JProperty("Piva", Qs_Piva))
        objAllegati.Add(New JProperty("Cod_Contatto", Qs_CodContatto))
        objAllegati.Add(New JProperty("Analisi_Testata_Cod", Qs_AnalisiTestataCod))
        objAllegati.Add(New JProperty("Area", Qs_IdArea))
        objAllegati.Add(New JProperty("Tipologia", Qs_IdTipologia))
        objAllegati.Add(New JProperty("Data_Scadenza", Qs_DataScadenza))
        objAllegati.Add(New JProperty("Data_Rilascio", Qs_DataRilascio))
        objAllegati.Add(New JProperty("Num_Documento", Num_Documento))
        objAllegati.Add(New JProperty("Ente_Rilascio", Ente_Rilascio))
        objAllegati.Add(New JProperty("dati_Allegati", dati_Allegati))

    End Sub

    Protected Function carica_Kendo_Allegati(piva As String, cod_contatto As String, analisi_testata_cod As String) As String

        Dim objDocumenti As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
        Dim filtro As String = "Alert_Elenco.Data_Scadenza DESC"
        Dim dati As DataTable = objDocumenti.Leggi_Allegati_Entita(piva, cod_contatto, CInt(analisi_testata_cod), 0, 0, 0, 0, 0, "", "", objParametri_Server)
        Return JSON_DataTableAllegati_Tabella(dati)

    End Function

    Private Shared Function JSON_DataTableAllegati_Tabella(ByRef DT_Allegati As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("ID_Elenco", "ID_Elenco", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("ID_Alert_Entita", "ID_Alert_Entita", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Allegati_Documenti_Cod", "Codice", "number")
        c._hidden = True
        l.Add(c)

        c = New ColonneNome("Tipologia", "Tipo Documento", "string")
        l.Add(c)

        c = New ColonneNome("Allegati_Documenti_Numero", "N. Documento", "string")
        l.Add(c)

        c = New ColonneNome("Data_Scadenza", "Data Scadenza", "string")
        l.Add(c)

        c = New ColonneNome("Allegati_Documenti_NomeFile", "Nome File", "string")
        l.Add(c)

        c = New ColonneNome("Allegati_Documenti_Ente_Des", "Ente Rilascio", "string")
        l.Add(c)

        c = New ColonneNome("Validazione_Data", "Data Rilascio", "string")
        l.Add(c)

        c = New ColonneNome("Descrizione_Scadenza", "Descrizione", "string")
        l.Add(c)

        'ritorno la tabella trasformata in json (aggiustando anche le colonne)
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        js.Editabile_Deafault = False

        Return js.JSON_DataTable_Kendo(DT_Allegati, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

    End Function

    Private Shared Function LeggiPercorsoAllegati(ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim LeggiConfSiti As New Configurazione_Siti_R
        Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
        Dim Percorso As String = dt_Conf.Rows(0).Item("Valore")
        Return FileSystemHelper.AggiungiSlashSeNonEsiste(Percorso)
    End Function


    '#########################################################################################
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaCombo_TipoDocumento(ByVal ID_Area As Integer, ByVal ID_Tipologia As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim objtipologia As New AgronicaCoreScadenziario.Alert_Tipologia_R
            Dim dt As DataTable = objtipologia.Leggi(ID_Area, ID_Tipologia, "", False, objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In dt.Rows
                JArrayLista.Add(New JObject(New JProperty("value", dr.Item("ID_Tipologia")),
                                            New JProperty("text", dr.Item("Nome"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiAllegati(ByVal Piva As String, ByVal Cod_Contatto As String, ByVal Analisi_Testata_Cod As String, ByVal ID_Elenco As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim objDocumenti As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
            Dim dati As DataTable = objDocumenti.Leggi_Allegati_Entita(Piva, Cod_Contatto, CInt(Analisi_Testata_Cod), ID_Elenco, 0, 0, 0, 0, "", "Alert_Elenco.Data_Scadenza DESC", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(dati, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaIndicixTipologia(ByVal piva As String,
                                                   ByVal id_area As Integer,
                                                   ByVal id_tipologia As Integer,
                                                   ByVal Data_Creazione As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            DT = leggi.LeggiIndicixTipologia(piva, id_area, id_tipologia, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, 0, objParametri_Server, Data_Creazione:=Data_Creazione)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiEntitaxIndici(ByVal piva As String, ByVal id_alert_entita As Integer) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            DT = leggi.LeggiEntitaxIndici(piva, id_alert_entita, "", objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RicercaDDLIndice(ByVal piva As String, ByVal id_indice As Integer, ByVal tipocampo As Integer, ByVal elenco_tipo As Integer, ByVal elenco_cod As Integer, elenco_cod_string As String, ByVal data_riferimento As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim DT As New DataTable

        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If Not IsDate(data_riferimento) Then
                data_riferimento = Now.Date
            Else
                data_riferimento = CDate(data_riferimento)
            End If


            Dim leggi As New AgronicaCoreScadenziario.Alert_Indice_R
            DT = leggi.LeggiDDLIndice(piva, id_indice, tipocampo, elenco_tipo, elenco_cod, data_riferimento, objParametri_Server, Elenco_Cod_String:=elenco_cod_string)

            Dim serializerSettings As New JsonSerializerSettings With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}
            r.RispostaStringa = JsonConvert.SerializeObject(DT, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function ControllaDatiNecessariSchema(ByVal piva As String, ByVal richiesta_cod As Integer, ByVal id_schema_template As Integer, ByVal nome_file As String, ByVal file_allegato As String) As RispostaStandard
        Dim DT As New DataTable
        Dim r As New RispostaStandard
        Dim Flag_Firmato_Digit As Integer = 0
        Dim Suffisso_File As String = ""
        Dim fileByteArray As Byte()
        Dim bFirma As Boolean = True
        Dim bEstensione As Boolean = True
        Dim Arrayp() As String


        Try

            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            Dim leggi As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
            DT = leggi.Leggi_Schema_Template(id_schema_template, "", "", objParametri_Server)


            If DT.Rows.Count > 0 Then

                Flag_Firmato_Digit = DT(0)("Flag_Firmato_Digit")
                Suffisso_File = DT(0)("Suffisso_File")

                'Flag_Firmato_Digit = 1
                'Suffisso_File = "exe"


                'Firma Digitale
                If Flag_Firmato_Digit = 1 And Not String.IsNullOrEmpty(file_allegato) Then

                    fileByteArray = Convert.FromBase64String(file_allegato)

                    'Creazione ByteArray
                    Dim plainData = Text.Encoding.UTF8.GetBytes(file_allegato)

                    Dim param As New Parametri_SignHelper With {
                        .Percorso_File_Temporanei = "",
                        .Percorso_File_Segnati = "",
                        .Percorso_Certificato = "",
                        .Chiave_Certificato = ""
                    }

                    Dim sh As New SignHelper(param, piva, nome_file, "GestioneAllegati.aspx", objParametri_Server)

                    bFirma = sh.Verifica_Firma_Valida_NEW(fileByteArray)

                End If


                'Estensione
                If Trim(Suffisso_File) <> "" And Not String.IsNullOrEmpty(nome_file) Then

                    Arrayp = Split(nome_file, ".")

                    If UBound(Arrayp) > 0 Then

                        If UCase(Suffisso_File) <> UCase(Arrayp(UBound(Arrayp))) Then

                            bEstensione = False

                        End If

                    Else

                        bEstensione = False

                    End If

                End If

            End If


            'Controlli
            If Not bFirma Then
                r.RispostaStringa = "Il documento '" & nome_file & "' non è valido. Il file deve essere firmato digitalmente."
            End If

            If Not bEstensione Then
                r.RispostaStringa = "L'estenione del documento '" & nome_file & "' non è corretta. Il file deve avere estensione '" & Suffisso_File & "'."
            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaAllegato(Piva As String, Cod_Contatto As String,
                                         ByVal Analisi_Testata_Cod As String,
                                         Tipo_Documento As Integer, Num_Documento As String,
                                         Ente_Rilascio As String, Data_Rilascio As String,
                                         Descrizione As String, Data_Scadenza As String,
                                         Nome_File As String, File_Allegato As String, ID_Elenco As Integer,
                                         Validazione_Flag As Integer,
                                         Username_Upload As String, Data_Upload As String,
                                         ByVal Note As String,
                                         ByVal EntitaxIndici As String) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W
        Dim objScriviEntitaxIndici As New AgronicaCoreScadenziario.Alert_Indice_W

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            'Se siamo in inserimento di un nuovo Patentino, se ne esiste uno ancora attivo e cambio la data scadenza a Data_Rilascio - 1
            If ID_Elenco = 0 Then
                cambiaDataScadenzaAllegato(Piva, Cod_Contatto, Data_Rilascio, Data_Scadenza, objParametri_Server)
            End If

            Dim Percorso As String = LeggiPercorsoAllegati(objParametri_Server)

            Dim objE As New AgronicaCoreScadenziario.Alert_Entita With {
                .analisi_campione_cod = 0,
                .Appezza = 0,
                .Campo_Cod = 0,
                .COM = "",
                .FOGLIO = 0,
                .ID_Agenda = 0,
                .Id_Imp = 0,
                .NUMERO = 0,
                .Piva = Piva,
                .PivaSuperUser = objParametri_Server.PivaSuperUser,
                .Programmazione_Entita_Cod = 0,
                .PROV = "",
                .Ricetta_Operazione_cod = 0,
                .Sa_Cod = 0,
                .SEZIONE = "",
                .SUBALTERNO = "",
                .Cod_Contatto = Cod_Contatto,
                .TipoEntita_Cod = enum_TipoEntita.Contatto,
                .Analisi_Testata_Cod = Analisi_Testata_Cod,
                .PC_Testata_Cod = 0,
                .PUA_Cod = 0,
                .Mac_Cod = 0
            }

            ' imposta campo descrizione se vuoto
            If String.IsNullOrEmpty(Descrizione) Then
                If Tipo_Documento = enum_ID_Area_Tipologia.Patentino_trattamenti Then
                    Descrizione = "Patentino " & If(Num_Documento = "", "di ", Num_Documento & " di ")
                    Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
                    Dim dt = objContatti.LeggiContattoSpecifico(Piva, Cod_Contatto, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                    If dt.Rows.Count > 0 Then
                        Descrizione &= dt.Rows(0).Item("cognome") & " " & dt.Rows(0).Item("nome")
                    End If
                    Descrizione &= If(Ente_Rilascio <> "" OrElse IsDate(Data_Rilascio), " rilasciato" & If(Ente_Rilascio <> "", " da " & Ente_Rilascio, "") & If(IsDate(Data_Rilascio), " il " & CDate(Data_Rilascio).ToShortDateString(), ""), "")
                ElseIf Tipo_Documento = enum_ID_Area_Tipologia.Analisi_terreno Then
                    Dim objAnalisiTestata As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
                    Dim dt = objAnalisiTestata.LeggiConCertificati(Piva, 0, 0, 0, " Analisi_Testata.Analisi_Testata_Cod=" & Analisi_Testata_Cod, "", objParametri_Server)
                    If dt.Rows.Count > 0 Then
                        Dim data = String.Empty
                        If Not IsDBNull(dt.Rows(0).Item("Analisi_Testata_Data_Inizio")) Then
                            data = "(" & CDate(dt.Rows(0).Item("Analisi_Testata_Data_Inizio")).ToShortDateString & ")"
                        End If
                        Descrizione = dt.Rows(0).Item("Analisi_Testata_Des") & " - " & data
                        Descrizione &= If(IsDate(Data_Scadenza), " - scadenza il:" & CDate(Data_Scadenza).ToShortDateString(), "")
                    End If
                End If
            End If

            Dim ret_Id_Elenco As Integer = -1

            'TODO: fare fix definitivo per data
            Dim dataUpload_Date As DateTime
            If Date.TryParseExact(Data_Upload, "dd/MM/yyyy",
                                  CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, Nothing) Then
                dataUpload_Date = Date.ParseExact(Data_Upload, "dd/MM/yyyy",
                                                 CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal)
            ElseIf Not IsDate(Data_Upload) Then
                dataUpload_Date = Now
            End If

            Dim strErr As String

            If ID_Elenco = 0 Then
                'NUOVO PATENTINO
                strErr = alert_W.Scrivi_Allegato(Tipo_Documento, objE, Num_Documento, Ente_Rilascio,
                                             If(String.IsNullOrEmpty(Data_Rilascio), AGRODATAINIZIO, CDate(Data_Rilascio)),
                                             If(String.IsNullOrEmpty(Data_Scadenza), AGRODATAFINE, CDate(Data_Scadenza)),
                                             Descrizione, Percorso, Nome_File, File_Allegato, AGRODATAINIZIO, AGRODATAFINE,
                                             objParametri_Server, "", ret_Id_Elenco, Validazione_Flag, Username_Upload, dataUpload_Date, fromGestioneAllegati:=True, If(ID_Elenco <> 0, True, False))
            Else
                'MODIFICA PATENTINO
                strErr = alert_W.Modifica(Tipo_Documento,
                                          objE,
                                          ID_Elenco,
                                          -1, -1,
                                          Data_Scadenza,
                                          Descrizione,
                                          Nome_File,
                                          Percorso,
                                          AGRODATAINIZIO,
                                          AGRODATAFINE,
                                          objParametri_Server,
                                          Note,
                                          Nothing,
                                          Validazione_Flag,
                                          objParametri_Server.UtenteUsername,
                                          Data_Upload,
                                          File_Allegato,
                                          True,
                                          Num_Documento,
                                          1,
                                          Nothing,
                                          enum_ID_Area_Alert.Contatti,
                                          False,
                                          Nothing)
            End If



            r.RispostaOK = True
            If Not String.IsNullOrEmpty(strErr) Then
                r.RispostaOK = False
                r.Errore = "Errore durante la scrittura, nessun salvataggio effettuato"
            ElseIf ID_Elenco <> 0 Then
                ' cancello il vecchio allegato se sono in modifica
                If Not alert_W.Cancella_Allegato(ID_Elenco, "", objParametri_Server, scriviLog:=False) Then
                    r.RispostaOK = False
                    r.Errore = "Errore durante la cancellazione dell'allegato"
                End If
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante la scrittura, nessun salvataggio effettuato"
        End Try

        Return r

    End Function

    Private Shared Function cambiaDataScadenzaAllegato(ByVal Piva As String,
                                                       ByVal Cod_Contatto As String,
                                                       ByVal Data_Rilascio As Date,
                                                       ByVal Data_Scadenza As Date,
                                                        ByVal objParametri_Server As AgronicaCoreParametri
                                                       )

        Dim xFiltroAggiuntivo = "Alert_Elenco.Data_Scadenza > '" + Data_Rilascio + "'"

        'Leggo allegati con data scadenza successiva
        Dim objDocumenti As New AgronicaCoreAnagrafeDAL.Allegati_EntitaxDocumenti_R
        Dim dati As DataTable = objDocumenti.Leggi_Allegati_Entita(Piva, Cod_Contatto, 0, 0, 0, 0, 0, 0, xFiltroAggiuntivo, "Alert_Elenco.Data_Scadenza DESC", objParametri_Server)

        If dati.Rows.Count > 0 Then
            Dim ID_Elenco = dati(0).Item("ID_Elenco")
            Dim Descrizione_Scadenza = dati(0).Item("Descrizione_Scadenza")
            Dim NewDataScadenza = Data_Rilascio.AddDays(-1)

            Dim objAlertElenco As New AgronicaCoreScadenziario.Alert_Elenco_W

            objAlertElenco.ModificaAutomatica(ID_Elenco, NewDataScadenza, Descrizione_Scadenza, objParametri_Server)
        End If

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CancellaAllegatiContatto(Piva As String, Cod_Contatto As String) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim Percorso As String = LeggiPercorsoAllegati(objParametri_Server)

            r.RispostaOK = alert_W.Cancella_Allegati_Contatto(Piva, Cod_Contatto, objParametri_Server, Percorso)
            If Not r.RispostaOK Then
                r.Errore = "Errore durante la cancellazione degli allegati contatto"
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante la cancellazione degli allegati contatto"
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function CancellaAllegato(ID_Elenco As Integer) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim Percorso As String = LeggiPercorsoAllegati(objParametri_Server)

            r.RispostaOK = True
            If ID_Elenco <> 0 AndAlso Not alert_W.Cancella_Allegato(ID_Elenco, Percorso, objParametri_Server, fromGestioneAllegati:=True) Then
                r.RispostaOK = False
                r.Errore = "Errore durante la cancellazione dell'allegato"
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante la cancellazione dell'allegato"
        End Try

        Return r

    End Function

    <Obsolete("Il download degli allegati utilizza gli stessi metodi del documentale, vedi AgronicaCoreScadenziario/Alert_Elenco.asmx/Leggi_File_Allegato")>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ScaricaAllegato(ByVal Cod_Documento As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        'Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        'Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim Percorso As String = LeggiPercorsoAllegati(objParametri_Server)

            Dim objAllegato_R As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_R
            Dim DT_Allegato As DataTable = objAllegato_R.Leggi(Cod_Documento, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            If Not IsDBNull(DT_Allegato.Rows(0).Item("Sottocartella")) And Not IsDBNull(DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")) Then

                Dim Sottocartella As String = DT_Allegato.Rows(0).Item("Sottocartella")
                Dim Nome_File As String = DT_Allegato.Rows(0).Item("Allegati_documenti_nomefile")

                If Not File.Exists(Percorso & Sottocartella & "/" & Nome_File) Then
                    r.Errore = "File non trovato"
                    r.RispostaOK = False
                Else
                    r.ParametroDue_stringa = Stringa_Codifica(Percorso & Sottocartella & "/" & Nome_File, AgroKey_EncoderDecoder)
                    r.ParametroDue = True
                    r.RispostaStringa = "OK"
                    r.RispostaOK = True
                End If
            End If



        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaPatentino(Piva As String,
                                          Cod_Contatto As String,
                                          Tipo_Documento As Integer,
                                          Num_Documento As String,
                                          Ente_Rilascio As String,
                                          Data_Rilascio As String,
                                          Descrizione As String,
                                          Data_Scadenza As String,
                                          Nome_File As String,
                                          File_Allegato As String,
                                          ID_Elenco As Integer,
                                          ID_Alert_Entita As Integer,
                                          Allegati_Documenti_Cod As Integer
                                          ) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim alert_W As New AgronicaCoreScadenziario_BIZ.Alert_W

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim strErr = alert_W.Salva_Patentino(Piva,
                                                 Cod_Contatto,
                                                 Tipo_Documento,
                                                 Num_Documento,
                                                 Ente_Rilascio,
                                                 Data_Rilascio,
                                                 Descrizione,
                                                 Data_Scadenza,
                                                 Nome_File,
                                                 File_Allegato,
                                                 ID_Elenco,
                                                 ID_Alert_Entita,
                                                 Allegati_Documenti_Cod,
                                                 objParametri_Server,
                                                 objParametri_Utenti)

            r.RispostaOK = True

            If Not String.IsNullOrEmpty(strErr) Then
                r.RispostaOK = False
                r.Errore = "Errore durante la scrittura, nessun salvataggio effettuato"
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante la scrittura, nessun salvataggio effettuato"
        End Try

        Return r

    End Function
End Class