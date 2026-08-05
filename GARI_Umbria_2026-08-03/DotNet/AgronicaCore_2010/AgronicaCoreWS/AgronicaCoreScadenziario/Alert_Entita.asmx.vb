Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreScadenziario
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreUtility

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Alert_Entita
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_con_documenti(ByVal objP_server As String, ByVal objP_utenti As String, ByVal id_elenco As Integer) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If id_elenco = 0 Then
            r.Errore = "id_elenco non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objScadenza As New AgronicaCoreScadenziario.Alert_Entita_R
            Dim dt As DataTable = objScadenza.Leggi_con_documenti(id_elenco, objParametri_Server, objParametri_Utenti)

            'New JProperty("Data", Date.ParseExact(dr.Item("Data_Scadenza").ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")),

            'Controlla che sia presente l'allegato
            If dt(0).Item("Allegati_Documenti_Cod") <> 0 Then

                Dim bFS As Boolean = True
                If Not IsDBNull(dt(0).Item("File_Allegato_DB")) Then
                    If dt(0).Item("File_Allegato_DB").ToString = "System.Byte[]" Then
                        'Salvataggio su DB
                        bFS = False
                    End If
                End If

                If bFS AndAlso
                    Not IsDBNull(dt(0).Item("Allegati_Documenti_NomeFile")) AndAlso
                    dt(0).Item("Allegati_Documenti_NomeFile") <> "" Then

                    Dim LeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim dt_Conf As DataTable = LeggiConfSiti.Leggi(6, "GestioneAllegati_Repository", "", "", objParametri_Server)
                    Dim Percorso As String = FileSystemHelper.AggiungiSlashSeNonEsiste(dt_Conf.Rows(0).Item("Valore"))

                    If dt(0).Item("Sottocartella") <> "" Then
                        Percorso = Percorso & FileSystemHelper.AggiungiSlashSeNonEsiste(dt(0).Item("Sottocartella"))
                    End If

                    Dim File_Name As String

                    File_Name = Percorso & dt(0).Item("Allegati_Documenti_NomeFile")

                    Dim fileByteArray As Byte()
                    'Salavataggio su FS
                    fileByteArray = My.Computer.FileSystem.ReadAllBytes(File_Name)

                    'Aggiornamento DT
                    dt(0).Item("File_Allegato_DB") = fileByteArray
                End If
            End If

            Dim objPraticheStati As New AgronicaCoreProfilazioneDAL.Pratiche_Stati_Attuali_R
            Dim descrizione_Stato As String = ""
            Dim Allegati_Documenti_Data As String = ""

            Dim JArrayListaOp As New JArray()
            For Each dr In dt.Rows
                If Not IsDBNull(dr.Item("Pratica_Cod")) AndAlso dr.Item("Pratica_Cod") > 0 Then
                    Dim dtp As New DataTable

                    dtp = objPraticheStati.LeggiConDescrizioneStato(dr.Item("Pratica_Cod"), "", "", objParametri_Server)

                    If dtp.Rows.Count > 0 Then
                        descrizione_Stato = dtp.Rows(0).Item("WAnagraficaStati_Des")
                    Else
                        descrizione_Stato = ""
                    End If

                End If

                If IsDate(dr.Item("Allegati_Documenti_Data")) Then
                    Allegati_Documenti_Data = CDate(dr.Item("Allegati_Documenti_Data")).ToString("dd/MM/yyyy")
                Else
                    Allegati_Documenti_Data = ""
                End If


                JArrayListaOp.Add(New JObject(
                                        New JProperty("id_area", dr.Item("id_area")),
                                        New JProperty("Area", dr.Item("nome_area")),
                                        New JProperty("ID_Tipologia", dr.Item("ID_Tipologia")),
                                        New JProperty("Tipologia", dr.Item("nome_tipologia")),
                                        New JProperty("TipoEntita_Cod", dr.Item("TipoEntita_Cod")),
                                        New JProperty("Allegati_Documenti_Cod", dr.Item("Allegati_Documenti_Cod")),
                                        New JProperty("Allegati_Documenti_Estensione", dr.Item("Allegati_Documenti_Estensione")),
                                        New JProperty("Id_Alert_Entita", dr.Item("Id_Alert_Entita")),
                                        New JProperty("piva", dr.Item("piva")),
                                        New JProperty("ID_Agenda", dr.Item("ID_Agenda")),
                                        New JProperty("Sa_Cod", dr.Item("Sa_Cod")),
                                        New JProperty("Appezza", dr.Item("Appezza")),
                                        New JProperty("Cod_Contatto", dr.Item("Cod_Contatto")),
                                        New JProperty("Analisi_Testata_Cod", dr.Item("Analisi_Testata_Cod")),
                                        New JProperty("PC_Testata_Cod", dr.Item("PC_Testata_Cod")),
                                        New JProperty("PUA_Cod", dr.Item("PUA_Cod")),
                                        New JProperty("Mac_Cod", dr.Item("piva") & "|" & dr.Item("Sa_Cod") & "|" & dr.Item("Mac_Cod")),
                                        New JProperty("Data", CDate(dr.Item("Data_Scadenza")).ToString("dd/MM/yyyy")),
                                        New JProperty("Testo", dr.Item("Descrizione_Scadenza")),
                                        New JProperty("Allegati_Documenti_Numero", dr.Item("Allegati_Documenti_Numero")),
                                        New JProperty("Allegati_Documenti_Data", Allegati_Documenti_Data),
                                        New JProperty("File_Name", dr.Item("Allegati_Documenti_NomeFile")),
                                        New JProperty("Sottocartella", dr.Item("Sottocartella")),
                                        New JProperty("Validazione_Flag", dr.Item("Validazione_Flag")),
                                        New JProperty("Username_Upload", dr.Item("Username_Upload")),
                                        New JProperty("Data_Upload", dr.Item("Data_Upload")),
                                        New JProperty("ChkStorico", dr.Item("ChkStorico")),
                                        New JProperty("Note", dr.Item("Note")),
                                        New JProperty("Data_Creazione", dr.Item("Data_Creazione")),
                                        New JProperty("Richiesta_Cod", dr.Item("Richiesta_Cod")),
                                        New JProperty("Id_Schema_Template", dr.Item("Id_Schema_Template")),
                                        New JProperty("File_Allegato_DB", dr.Item("File_Allegato_DB")),
                                        New JProperty("CompressoDaGIAS", dr.Item("CompressoDaGIAS")),
                                        New JProperty("Ricetta_Operazione_cod", dr.Item("Ricetta_Operazione_cod")),
                                        New JProperty("Lav_Cod", dr.Item("Lav_Cod")),
                                        New JProperty("Id_ImpresexParticelle", dr.Item("Id_ImpresexParticelle")),
                                        New JProperty("Stato_Attuale", descrizione_Stato),
                                        New JProperty("Validazione_Data", dr.Item("Validazione_Data")),
                                        New JProperty("Allegati_Documenti_Ente_Des", dr.Item("Allegati_Documenti_Ente_Des"))
                                        ))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_documenti_con_agenda(ByVal piva As String, ByVal id_agenda As Integer, ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard()
        Dim xFiltroAggiuntivo As String = ""

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If id_agenda = 0 Then
            r.Errore = "id_agenda non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objScadenza As New AgronicaCoreScadenziario.Alert_Entita_R
            Dim dt As DataTable = objScadenza.LeggiAlertEntita_AlertElenco(0, piva, id_agenda, objParametri_Server, xFiltroAggiuntivo, "")

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

End Class