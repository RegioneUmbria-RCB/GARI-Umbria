Imports System.IO
Imports System.Text
Imports System.Web.Services
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreContabBIZ
Imports AgronicaCoreContabObject
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreUtility
Imports AgronicaCoreUtility.CaricaListControl
Imports AgronicaCoreVarieBIZ
Imports InData.Anagrafica
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Parco_Macchine_Costo = AgronicaCoreContabObject.Parco_Macchine_Costo
Imports Parco_Macchine_Manutenzione = AgronicaCoreContabObject.Parco_Macchine_Manutenzione


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Macchine
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getJSON_ParcoMacchine_js() As String
        Dim dd = File.ReadAllText("C:\TFS_AreaLavoro\Gias\RamoPrincipale\Src\GiasDotNet\AgronicaCore_2010\AgronicaCoreWS\Anagrafica\JSON_ParcoMacchine.json")
        Return dd
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getMacchina(ByVal Piva As String, ByVal Mac_Cod As Integer,
                                ByVal objParametri As String) As Parco_Macchine
        Dim risposta As New Parco_Macchine(Piva, Mac_Cod)

        risposta.Leggi(Utility.convertStringtoOBJparametri(objParametri))
        Return risposta
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getMacchina_NG(ByVal InData As CoreWS_Generic(Of getMacchina)) As Parco_Macchine
        Dim risposta As New Parco_Macchine(InData.InData.Piva, InData.InData.Mac_Cod)
        risposta.Leggi(Utility.convertStringtoOBJparametri(InData.objP.objP_server))
        Return risposta
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Macchine_APP(ByVal piva As String,
                                ByVal objP_super_server As String,
                                ByVal objP_server As String,
                                ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)


            ' Filtro per controllo gestione costi
            Dim filtriList As New List(Of String)
            Dim leggi_CDG_R As New CDG_BIZ_R
            Dim tipoCdG = leggi_CDG_R.GetTipoCdG(piva, objParametri_Server, objParametri_Utenti)
            If tipoCdG = enum_TipoCdG.NuovoTipo Then
                ' filtriList.Add("(Visibile_ctrl_gestione = 1)")
            End If

            Dim pivaEffettiva As String
            Dim caricaPubbliche As Boolean
            If Not String.IsNullOrEmpty(piva) Then
                ' Solo macchine di un'azienda specifica
                pivaEffettiva = piva
                caricaPubbliche = False
                filtriList.Add("(Parco_Macchine.Sa_Cod != -1)")
            Else
                ' Tutte le macchine comuni tranne quelle di un'azienda specifica
                pivaEffettiva = "99999999999"
                caricaPubbliche = True
            End If

            Dim xFiltroAggiuntivo As String = String.Join(" AND ", filtriList)

            Dim ParcoMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim DTMacchine As DataTable = ParcoMacchine.Leggi(
                pivaEffettiva,
                0,
                caricaPubbliche,
                "",
                "",
                "",
                "",
                "",
                0,
                "",
                False,
                0,
                "",
                False,
                CostantiPersonalizzate.AGRODATAINIZIO,
                CostantiPersonalizzate.AGRODATAFINE,
                xFiltroAggiuntivo,
                "",
                objParametri_Server
            )

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(DTMacchine, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getNewMacchina(ByVal Piva As String, ByVal objParametri As String) As Parco_Macchine
        Return New Parco_Macchine(Piva, 0)
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getNewMacchina_NG(InData As Object) As Parco_Macchine

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))
        Dim Piva As String = InData.InData
        InData.objP.objP_server = ""

        Return New Parco_Macchine(Piva, 0)
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function scriviMacchina(ByVal Parco_Macchina As Parco_Macchine,
                                   ByVal ASG_ProgressivoGIAS As Integer,
                                   ByVal IdServizio As Integer,
                                   ByVal objParametri As String
                                   ) As RispostaStandard
        Dim ris As New RispostaStandard
        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, Utility.convertStringtoOBJparametri(objParametri))
            ris.RispostaOK = Parco_Macchina.Salva(ASG_ProgressivoGIAS, IdServizio, Utility.convertStringtoOBJparametri(objParametri))

        Catch ex As Exception
            ris.RispostaOK = False
            ris.Errore = ex.Message
            AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione(Utility.convertStringtoOBJparametri(objParametri))
        End Try
        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(Utility.convertStringtoOBJparametri(objParametri))
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function scriviMacchina_NG(ByVal InData As CoreWS_Generic(Of scriviMacchina)) As RispostaStandard

        Dim ris As New RispostaStandard

        Dim ParcoMacchina As New Parco_Macchine
        ParcoMacchina.Piva = InData.InData.Parco_Macchina._Piva
        ParcoMacchina.Mac_Cod = InData.InData.Parco_Macchina._Mac_Cod
        ParcoMacchina.Sa_Cod = InData.InData.Parco_Macchina.Sa_Cod
        ParcoMacchina.Descrizione_per_Agenda = InData.InData.Parco_Macchina._Descrizione_per_Agenda
        ParcoMacchina.Mac_Cod_Origine = InData.InData.Parco_Macchina._Mac_Cod_Origine
        ParcoMacchina.Piva_superUser_Origine = InData.InData.Parco_Macchina._Piva_superUser_Origine
        ParcoMacchina.GiacenzaIniziale = InData.InData.Parco_Macchina.GiacenzaIniziale
        ParcoMacchina.Finalita = InData.InData.Parco_Macchina._Finalita
        ParcoMacchina.Tipo.text = InData.InData.Parco_Macchina._Tipo.text
        ParcoMacchina.Tipo.val = InData.InData.Parco_Macchina._Tipo.val
        ParcoMacchina.Marca = InData.InData.Parco_Macchina._Marca
        ParcoMacchina.Dettaglio_1.text = InData.InData.Parco_Macchina._Dettaglio_1.text
        ParcoMacchina.Dettaglio_1.val = InData.InData.Parco_Macchina._Dettaglio_1.val
        ParcoMacchina.Dettaglio_2.text = InData.InData.Parco_Macchina._Dettaglio_2.text
        ParcoMacchina.Dettaglio_2.val = InData.InData.Parco_Macchina._Dettaglio_2.val
        ParcoMacchina.Descrizione = InData.InData.Parco_Macchina._Descrizione
        ParcoMacchina.Targa = InData.InData.Parco_Macchina._Targa
        ParcoMacchina.Tipo_Targa = InData.InData.Parco_Macchina._Tipo_Targa
        ParcoMacchina.Telaio = InData.InData.Parco_Macchina._Telaio
        ParcoMacchina.Modello = InData.InData.Parco_Macchina._Modello
        ParcoMacchina.Proprietario = InData.InData.Parco_Macchina._Proprietario
        ParcoMacchina.Alimentazione = InData.InData.Parco_Macchina._Alimentazione
        ParcoMacchina.Potenza = InData.InData.Parco_Macchina._Potenza
        ParcoMacchina.UDM_Potenza = InData.InData.Parco_Macchina._UDM_Potenza
        ParcoMacchina.Taratura_Ugello = InData.InData.Parco_Macchina._Taratura_Ugello
        ParcoMacchina.Titolo_Possesso = InData.InData.Parco_Macchina._Titolo_Possesso
        ParcoMacchina.CUAA_Proprietario = InData.InData.Parco_Macchina._CUAA_Proprietario
        ParcoMacchina.Data_carico = InData.InData.Parco_Macchina._Data_carico
        ParcoMacchina.Data_scarico = InData.InData.Parco_Macchina._Data_scarico
        ParcoMacchina.Numero_Immatricolazione = InData.InData.Parco_Macchina._Numero_Immatricolazione
        ParcoMacchina.Data_Immatricolazione = InData.InData.Parco_Macchina._Data_Immatricolazione
        ParcoMacchina.Numero_Immatricolazione_Rimorchio = InData.InData.Parco_Macchina.Numero_Immatricolazione_Rimorchio
        ParcoMacchina.Numero_Autorizzazione_Trasporto = InData.InData.Parco_Macchina._Numero_Autorizzazione_Trasporto
        ParcoMacchina.Data_Rilascio_Autorizzazione = InData.InData.Parco_Macchina.Data_Rilascio_Autorizzazione
        ParcoMacchina.Data_Inizio_Utilizzo = InData.InData.Parco_Macchina._Data_Inizio_Utilizzo
        ParcoMacchina.Peso_Tara = InData.InData.Parco_Macchina._Peso_Tara
        ParcoMacchina.Macchina_Attiva = InData.InData.Parco_Macchina.Macchina_Attiva
        ParcoMacchina.Stato_Utilizzo = InData.InData.Parco_Macchina._Stato_Utilizzo
        ParcoMacchina.Data_Dismissione = InData.InData.Parco_Macchina._Data_Dismissione
        ParcoMacchina.Visibilita = InData.InData.Parco_Macchina._Visibilita
        ParcoMacchina.Note = InData.InData.Parco_Macchina._Note
        ParcoMacchina.Data_Ultima_Manutenzione = InData.InData.Parco_Macchina._Data_Ultima_Revisione
        ParcoMacchina.Costo_Acquisto = InData.InData.Parco_Macchina._Costo_Acquisto
        ParcoMacchina.Costo_Manutenzione_Revisione = InData.InData.Parco_Macchina._Costo_Manutenzione_Revisione
        ParcoMacchina.Ammortamento_Annuo_Percentuale = InData.InData.Parco_Macchina._Ammortamento_Annuo_Percentuale

        Dim Parco_Macchine As New Parco_Macchine_Manutenzione()
        For Each item In InData.InData.Parco_Macchina._Manutenzioni
            Parco_Macchine = New Parco_Macchine_Manutenzione()
            Parco_Macchine.ID_Agenda = item._ID_Agenda
            Parco_Macchine.Data = item._Data
            Parco_Macchine.Movimento_Des = item._Movimento_Des
            Parco_Macchine.Costo = item._Costo
            Parco_Macchine.N_Certificato = item._N_Certificato
            ParcoMacchina.Manutenzioni.Add(Parco_Macchine)
        Next

        Dim Parco_Macchine_Costo As New Parco_Macchine_Costo()
        For Each item In InData.InData.Parco_Macchina._Costi
            Parco_Macchine_Costo = New Parco_Macchine_Costo()
            Parco_Macchine_Costo.ID = item._ID
            Parco_Macchine_Costo.Unita_Misura = item._Unita_Misura
            Parco_Macchine_Costo.Unita_Misura_Des = item._Unita_Misura_Des
            Parco_Macchine_Costo.Prezzo = item._Prezzo
            Parco_Macchine_Costo.Validita_Inizio = item._Validita_Inizio
            Parco_Macchine_Costo.Validita_Fine = item._Validita_Fine
            ParcoMacchina.Costi.Add(Parco_Macchine_Costo)
        Next

        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, Utility.convertStringtoOBJparametri(InData.objP.objP_server))
            ris.RispostaOK = ParcoMacchina.Salva(InData.InData.ASG_ProgressivoGIAS, InData.InData.IdServizio, Utility.convertStringtoOBJparametri(InData.objP.objP_server))

        Catch ex As Exception
            ris.RispostaOK = False
            ris.Errore = ex.Message
            AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione(Utility.convertStringtoOBJparametri(InData.objP.objP_server))
        End Try
        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(Utility.convertStringtoOBJparametri(InData.objP.objP_server))
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Test_Macchine_Archivio_Lettura(InData As CoreWS_Generic(Of scriviMacchina)) As RispostaStandard
        Dim ris As New RispostaStandard
        Try
            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, Utility.convertStringtoOBJparametri(InData.objP.objP_server))
            ' ris.RispostaOK = InData.InData.Parco_Macchina.Salva(InData.InData.ASG_ProgressivoGIAS, InData.InData.IdServizio, Utility.convertStringtoOBJparametri(InData.objP.objP_server))

        Catch ex As Exception
            ris.RispostaOK = False
            ris.Errore = ex.Message
            AgronicaCoreDataProvider.ConnessioniTransazioni.RollBackTransazione(Utility.convertStringtoOBJparametri(InData.objP.objP_server))
        End Try
        AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(Utility.convertStringtoOBJparametri(InData.objP.objP_server))
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function cancellaMacchina(ByVal Parco_Macchina As Parco_Macchine,
                                     ByVal ASG_ProgressivoGIAS As Integer,
                                     ByVal IdServizio As Integer,
                                     ByVal objParametri As String) As Parco_Macchine

        Dim ris As Boolean = Parco_Macchina.Salva(ASG_ProgressivoGIAS, IdServizio, Utility.convertStringtoOBJparametri(objParametri))

        Return Parco_Macchina
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function cancellaMacchina_NG(InData As CoreWS_Generic(Of CancellaMacchina)) As Parco_Macchine

        Dim ParcoMacchina As New Parco_Macchine
        ParcoMacchina.Piva = InData.InData.Parco_Macchina._Piva
        ParcoMacchina.Mac_Cod = InData.InData.Parco_Macchina._Mac_Cod
        ParcoMacchina.Sa_Cod = InData.InData.Parco_Macchina.Sa_Cod
        ParcoMacchina.Descrizione_per_Agenda = InData.InData.Parco_Macchina._Descrizione_per_Agenda
        ParcoMacchina.Mac_Cod_Origine = InData.InData.Parco_Macchina._Mac_Cod_Origine
        ParcoMacchina.Piva_superUser_Origine = InData.InData.Parco_Macchina._Piva_superUser_Origine
        ParcoMacchina.GiacenzaIniziale = InData.InData.Parco_Macchina.GiacenzaIniziale
        ParcoMacchina.Finalita = InData.InData.Parco_Macchina._Finalita
        ParcoMacchina.Tipo.text = InData.InData.Parco_Macchina._Tipo.text
        ParcoMacchina.Tipo.val = InData.InData.Parco_Macchina._Tipo.val
        ParcoMacchina.Marca = InData.InData.Parco_Macchina._Marca
        ParcoMacchina.Dettaglio_1.text = InData.InData.Parco_Macchina._Dettaglio_1.text
        ParcoMacchina.Dettaglio_1.val = InData.InData.Parco_Macchina._Dettaglio_1.val
        ParcoMacchina.Dettaglio_2.text = InData.InData.Parco_Macchina._Dettaglio_2.text
        ParcoMacchina.Dettaglio_2.val = InData.InData.Parco_Macchina._Dettaglio_2.val
        ParcoMacchina.Descrizione = InData.InData.Parco_Macchina._Descrizione
        ParcoMacchina.Targa = InData.InData.Parco_Macchina._Targa
        ParcoMacchina.Tipo_Targa = InData.InData.Parco_Macchina._Tipo_Targa
        ParcoMacchina.Telaio = InData.InData.Parco_Macchina._Telaio
        ParcoMacchina.Modello = InData.InData.Parco_Macchina._Modello
        ParcoMacchina.Proprietario = InData.InData.Parco_Macchina._Proprietario
        ParcoMacchina.Alimentazione = InData.InData.Parco_Macchina._Alimentazione
        ParcoMacchina.Potenza = InData.InData.Parco_Macchina._Potenza
        ParcoMacchina.UDM_Potenza = InData.InData.Parco_Macchina._UDM_Potenza
        ParcoMacchina.Taratura_Ugello = InData.InData.Parco_Macchina._Taratura_Ugello
        ParcoMacchina.Titolo_Possesso = InData.InData.Parco_Macchina._Titolo_Possesso
        ParcoMacchina.CUAA_Proprietario = InData.InData.Parco_Macchina._CUAA_Proprietario
        ParcoMacchina.Data_carico = InData.InData.Parco_Macchina._Data_carico
        ParcoMacchina.Data_scarico = InData.InData.Parco_Macchina._Data_scarico
        ParcoMacchina.Numero_Immatricolazione = InData.InData.Parco_Macchina._Numero_Immatricolazione
        ParcoMacchina.Data_Immatricolazione = InData.InData.Parco_Macchina._Data_Immatricolazione
        ParcoMacchina.Numero_Immatricolazione_Rimorchio = InData.InData.Parco_Macchina.Numero_Immatricolazione_Rimorchio
        ParcoMacchina.Numero_Autorizzazione_Trasporto = InData.InData.Parco_Macchina._Numero_Autorizzazione_Trasporto
        ParcoMacchina.Data_Rilascio_Autorizzazione = InData.InData.Parco_Macchina.Data_Rilascio_Autorizzazione
        ParcoMacchina.Data_Inizio_Utilizzo = InData.InData.Parco_Macchina._Data_Inizio_Utilizzo
        ParcoMacchina.Peso_Tara = InData.InData.Parco_Macchina._Peso_Tara
        ParcoMacchina.Macchina_Attiva = InData.InData.Parco_Macchina.Macchina_Attiva
        ParcoMacchina.Stato_Utilizzo = InData.InData.Parco_Macchina._Stato_Utilizzo
        ParcoMacchina.Data_Dismissione = InData.InData.Parco_Macchina._Data_Dismissione
        ParcoMacchina.Visibilita = InData.InData.Parco_Macchina._Visibilita
        ParcoMacchina.Note = InData.InData.Parco_Macchina._Note
        ParcoMacchina.Data_Ultima_Manutenzione = InData.InData.Parco_Macchina._Data_Ultima_Revisione
        ParcoMacchina.Costo_Acquisto = InData.InData.Parco_Macchina._Costo_Acquisto
        ParcoMacchina.Costo_Manutenzione_Revisione = InData.InData.Parco_Macchina._Costo_Manutenzione_Revisione
        ParcoMacchina.Ammortamento_Annuo_Percentuale = InData.InData.Parco_Macchina._Ammortamento_Annuo_Percentuale

        Dim Parco_Macchine As New Parco_Macchine_Manutenzione()
        For Each item In InData.InData.Parco_Macchina._Manutenzioni
            Parco_Macchine = New Parco_Macchine_Manutenzione()
            Parco_Macchine.ID_Agenda = item._ID_Agenda
            Parco_Macchine.Data = item._Data
            Parco_Macchine.Movimento_Des = item._Movimento_Des
            Parco_Macchine.Costo = item._Costo
            Parco_Macchine.N_Certificato = item._N_Certificato
            ParcoMacchina.Manutenzioni.Add(Parco_Macchine)
        Next

        Dim Parco_Macchine_Costo As New Parco_Macchine_Costo()
        For Each item In InData.InData.Parco_Macchina._Costi
            Parco_Macchine_Costo = New Parco_Macchine_Costo()
            Parco_Macchine_Costo.ID = item._ID
            Parco_Macchine_Costo.Unita_Misura = item._Unita_Misura
            Parco_Macchine_Costo.Unita_Misura_Des = item._Unita_Misura_Des
            Parco_Macchine_Costo.Prezzo = item._Prezzo
            Parco_Macchine_Costo.Validita_Inizio = item._Validita_Inizio
            Parco_Macchine_Costo.Validita_Fine = item._Validita_Fine
            ParcoMacchina.Costi.Add(Parco_Macchine_Costo)
        Next
        Dim ris As Boolean = ParcoMacchina.Salva(InData.InData.ASG_ProgressivoGIAS, InData.InData.IdServizio, Utility.convertStringtoOBJparametri(InData.objP.objP_server))

        Return ParcoMacchina
    End Function

    'RR: da finire
    '    <WebMethod()>
    '    <Script.Services.ScriptMethod()>
    '    Public Function getStringoneXml(ByVal InData As CoreWS_Generic(Of getStringoneXml)) As String

    '        If InData.InData.Parco_Macchina._Mac_Cod = 0 Then
    '            Return InData.InData.Parco_Macchina.XML_GeneraStringoneFinale(enum_TipoOperazioneDB.Scrittura, InData.InData.ASG_ProgressivoGIAS, Utility.convertStringtoOBJparametri(InData.objP.objP_server))
    '        Else
    '            Return InData.InData.Parco_Macchina.XML_GeneraStringoneFinale(enum_TipoOperazioneDB.Modifica, InData.InData.ASG_ProgressivoGIAS, Utility.convertStringtoOBJparametri(InData.objP.objP_server))
    '        End If

    '    End Function

#Region "funzioni per il caricamento dati"

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getCentriVisibilita(ByVal Piva As String, ByVal objParametri As String) As List(Of rispostaValore_IntTesto)
        Dim ris As New List(Of rispostaValore_IntTesto)
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(cmb, True, "Macchina Aziendale", "0", Piva, False, 2, "", "", Utility.convertStringtoOBJparametri(objParametri))
        cmb.Items.Add(New ListItem("Macchina/Attrezzatura movimentabile da tutte le imprese ", "-1"))
        For Each e In cmb.Items
            ris.Add(New rispostaValore_IntTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getCentriVisibilita_NG(InData As Object) As List(Of rispostaValore_IntTesto)
        Dim ris As New List(Of rispostaValore_IntTesto)

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))
        Dim Piva As String = InData.InData
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(cmb, True, "Macchina Aziendale", "0", Piva, False, 2, "", "", Utility.convertStringtoOBJparametri(InData.objP.objP_server))
        cmb.Items.Add(New ListItem("Macchina/Attrezzatura movimentabile da tutte le imprese ", "-1"))
        For Each e In cmb.Items
            ris.Add(New rispostaValore_IntTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getFinalita() As List(Of rispostaValore_IntTesto)
        Dim ris As New List(Of rispostaValore_IntTesto)
        ris.Add(New rispostaValore_IntTesto With {.valore = 0, .testo = "Agricola/Zootecnica"})
        ris.Add(New rispostaValore_IntTesto With {.valore = 1, .testo = "Industriale"})
        ris.Add(New rispostaValore_IntTesto With {.valore = 2, .testo = "Commerciale"})

        Return ris
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoTipo(ByVal objParametri As String) As List(Of rispostaValoreTesto)

        Dim ris As New List(Of rispostaValoreTesto)
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.TipoMacchine(cmb, 1, "", "", Utility.convertStringtoOBJparametri(objParametri))
        'primo item a 0
        For Each e In cmb.Items
            ris.Add(New rispostaValoreTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoTipo_NG(ByVal objP_server As String) As List(Of rispostaValoreTesto)

        Dim ris As New List(Of rispostaValoreTesto)
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.TipoMacchine(cmb, 1, "", "", Utility.convertStringtoOBJparametri(objP_server))
        'primo item a 0
        For Each e In cmb.Items
            ris.Add(New rispostaValoreTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoDettaglio1(ByVal Tipo As String, ByVal objParametri As String) As List(Of rispostaValoreTesto)

        Dim ris As New List(Of rispostaValoreTesto)
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.TipoMacchine(cmb, 2, Tipo, "", Utility.convertStringtoOBJparametri(objParametri))
        For Each e In cmb.Items
            ris.Add(New rispostaValoreTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoDettaglio1_NG(InData As Object) As List(Of rispostaValoreTesto)
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))
        Dim Tipo As String = InData.InData
        Dim ris As New List(Of rispostaValoreTesto)
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.TipoMacchine(cmb, 2, Tipo, "", Utility.convertStringtoOBJparametri(InData.objP.objP_server))
        For Each e In cmb.Items
            ris.Add(New rispostaValoreTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoDettaglio2(ByVal Tipo As String, ByVal Dettaglio_1 As String, ByVal objParametri As String) As List(Of rispostaValoreTesto)
        Dim ris As New List(Of rispostaValoreTesto)
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.TipoMacchine(cmb, 3, Tipo, Dettaglio_1, Utility.convertStringtoOBJparametri(objParametri))
        For Each e In cmb.Items
            ris.Add(New rispostaValoreTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoDettaglio2_NG(InData As CoreWS_Generic(Of getElencoDettaglio2)) As List(Of rispostaValoreTesto)
        Dim ris As New List(Of rispostaValoreTesto)
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.TipoMacchine(cmb, 3, InData.InData.Tipo, InData.InData.Dettaglio_1, Utility.convertStringtoOBJparametri(InData.objP.objP_server))
        For Each e In cmb.Items
            ris.Add(New rispostaValoreTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoMarche(ByVal objParametri As String) As List(Of rispostaValore_IntTesto)
        Dim ris As New List(Of rispostaValore_IntTesto)
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.MarcheMacchine(cmb, Utility.convertStringtoOBJparametri(objParametri))
        For Each e In cmb.Items
            ris.Add(New rispostaValore_IntTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoMarche_NG(ByVal objP_server As String) As List(Of rispostaValore_IntTesto)
        Dim ris As New List(Of rispostaValore_IntTesto)
        Dim cmb As New DropDownList
        AgronicaCoreUtility.CaricaListControl.MarcheMacchine(cmb, Utility.convertStringtoOBJparametri(objP_server))
        For Each e In cmb.Items
            ris.Add(New rispostaValore_IntTesto With {.valore = e.value, .testo = e.text})
        Next
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoMarcheWS(ByVal objParametri As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            r.RispostaOK = True

            Dim risp = getElencoMarche(objParametri)
            Dim str = JsonConvert.SerializeObject(risp)
            r.RispostaStringa = str

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante getElencoMarcheWS:" & ex.Message

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getElencoMarcheWS_NG(ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            r.RispostaOK = True

            Dim risp = getElencoMarche(objP_server)
            Dim str = JsonConvert.SerializeObject(risp)
            r.RispostaStringa = str

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante getElencoMarcheWS:" & ex.Message

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getTipoTarga() As List(Of rispostaValore_IntTesto)
        Dim ris As New List(Of rispostaValore_IntTesto)
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoTarga.NonDefinito, .testo = "Non Definito"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoTarga.SenzaTarga, .testo = "Senza Targa"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoTarga.Stradale, .testo = "Stradale"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoTarga.Rimorchio, .testo = "Rimorchio"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoTarga.Triangolare, .testo = "Triangolare"})
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Macchine_Per_Documentale(ByVal objP_server As String, ByVal piva As String, ByVal mac_cod As Integer, id_tipologia As Integer) As RispostaStandard
        Dim r As New RispostaStandard()
        Dim xFiltro_Aggiuntivo As String = ""

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            If mac_cod <> 0 Then
                xFiltro_Aggiuntivo = "Parco_Macchine.Mac_Cod = " & mac_cod
            End If

            Select Case id_tipologia
                Case enum_ID_Area_Tipologia.Stazione_Meteo_Infragri
                    If xFiltro_Aggiuntivo <> "" Then
                        xFiltro_Aggiuntivo &= " AND "
                    End If

                    xFiltro_Aggiuntivo &= " ("
                    xFiltro_Aggiuntivo &= " (Parco_Macchine.Class_Code = '19' OR Parco_Macchine.Class_Code LIKE '19.%') "
                    xFiltro_Aggiuntivo &= " OR (Parco_Macchine.Class_Code = '20' OR Parco_Macchine.Class_Code LIKE '20.%') "
                    xFiltro_Aggiuntivo &= " OR (Parco_Macchine.Class_Code = '21' OR Parco_Macchine.Class_Code LIKE '21.%') "
                    xFiltro_Aggiuntivo &= ") "
            End Select

            Dim ddlMacchine As New DropDownList
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.Macchinari_2(ddlMacchine, False, "", "",
                                                               piva, "", xFiltro_Aggiuntivo, "", objParametriServer, descrizioneCompleta:=True, caricaPubbliche:=True)

            Dim jArrayListaOp As New JArray()

            For Each li As ListItem In ddlMacchine.Items
                jArrayListaOp.Add(New JObject(New JProperty("chiave", li.Value), New JProperty("nome", li.Text)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Macchine_Per_Piva(ByVal objP_server As String, ByVal piva As String, ByVal mac_cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard()
        Dim xFiltro_Aggiuntivo As String = ""

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            If mac_cod <> 0 Then
                xFiltro_Aggiuntivo = "Parco_Macchine.Mac_Cod = " & mac_cod
            End If


            Dim ddlMacchine As New DropDownList
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.Macchinari_2(ddlMacchine, False, "", "",
                                                               piva, "", xFiltro_Aggiuntivo, "", objParametriServer)

            Dim jArrayListaOp As New JArray()

            For Each li As ListItem In ddlMacchine.Items
                jArrayListaOp.Add(New JObject(New JProperty("chiave", li.Value), New JProperty("nome", li.Text)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Macchine_Per_Piva_NG(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard()

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim ddlMacchine As New DropDownList
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.Macchinari_2(ddlMacchine, False, "", "",
                                                               piva, "", "", "", objParametriServer)

            Dim jArrayListaOp As New JArray()

            For Each li As ListItem In ddlMacchine.Items
                jArrayListaOp.Add(New JObject(New JProperty("chiave", li.Value), New JProperty("nome", li.Text)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Macchine_Per_Contatto(ByVal objP_server As String,
                                                ByVal piva As String,
                                                ByVal codContatto As String,
                                                ByVal xOrderBy As String
                                                ) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim obj As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim dt As DataTable = obj.LeggiMacchineContatto(piva, codContatto, "", xOrderBy, objParametriServer)

            Dim jArrayListaOp As New JArray()
            For Each dr In dt.Rows()
                jArrayListaOp.Add(New JObject(New JProperty("Mac_Cod", dr.Item("Mac_Cod")),
                                              New JProperty("Mac_Des", dr.Item("Mac_Des")),
                                              New JProperty("Targa", dr.Item("Targa")),
                                              New JProperty("N_Immatricolazione", dr.Item("N_Immatricolazione")),
                                              New JProperty("N_Immatricolazione_Rimorchio", dr.Item("N_Immatricolazione_Rimorchio")),
                                              New JProperty("N_Autorizzazione_Trasporto", dr.Item("N_Autorizzazione_Trasporto")),
                                              New JProperty("Data_Rilascio_Autorizzazione", dr.Item("Data_Rilascio_Autorizzazione")),
                                              New JProperty("Peso", dr.Item("Peso"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
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
    Public Function Leggi_Macchine_Per_Contatto_NG(ByVal InData As CoreWS_Generic(Of Leggi_Macchine_Per_Contatto)) As RispostaStandard
        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim obj As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim dt As DataTable = obj.LeggiMacchineContatto(InData.InData.piva, InData.InData.codContatto, "", InData.InData.xOrderBy, objParametriServer)

            Dim jArrayListaOp As New JArray()
            For Each dr In dt.Rows()
                jArrayListaOp.Add(New JObject(New JProperty("Mac_Cod", dr.Item("Mac_Cod")),
                                              New JProperty("Mac_Des", dr.Item("Mac_Des")),
                                              New JProperty("Targa", dr.Item("Targa")),
                                              New JProperty("N_Immatricolazione", dr.Item("N_Immatricolazione")),
                                              New JProperty("N_Immatricolazione_Rimorchio", dr.Item("N_Immatricolazione_Rimorchio")),
                                              New JProperty("N_Autorizzazione_Trasporto", dr.Item("N_Autorizzazione_Trasporto")),
                                              New JProperty("Data_Rilascio_Autorizzazione", dr.Item("Data_Rilascio_Autorizzazione")),
                                              New JProperty("Peso", dr.Item("Peso"))))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
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
    Public Function Leggi_Macchine_Per_Tipo_NG(ByVal InData As CoreWS_Generic(Of Leggi_Macchine_Per_Tipo)) As RispostaStandard
        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim params As Leggi_Macchine_Per_Tipo = InData.InData

            Dim filtroAggiuntivo As String = ""
            If Not String.IsNullOrWhiteSpace(params.classCode) Then
                filtroAggiuntivo = ($"Parco_Macchine.Class_Code LIKE '{params.classCode.Trim}%'")
            End If

            Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim dtMacchine = objMacchine.leggi_x_anagrafica(params.piva, filtroAggiuntivo, "", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaStringa = JsonConvert.SerializeObject(dtMacchine, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMacchinaIrrigazioneDefault(ByVal InData As CoreWS_Generic(Of Leggi_Macchine_Irrigazione_Impianto)) As RispostaStandard
        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim params As Leggi_Macchine_Irrigazione_Impianto = InData.InData
            Dim jArrayMacchineIrrigazioneImpianto As New JArray()

            Dim objImpiantiMacchine As New AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_R
            Dim dtImpiantiMacchine = objImpiantiMacchine.ReadJoinDescriptions(objParametriServer, params.piva, params.sa_cod, params.appezza, params.id_reg)

            If dtImpiantiMacchine.Rows.Count > 0 Then
                Dim dr = dtImpiantiMacchine.Rows(0)
                PopolaJArrayMacchine(dr, jArrayMacchineIrrigazioneImpianto)
            Else
                Dim objImpiantiIrrigazione As New AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R
                Dim dtImpiantiIrrigazione = objImpiantiIrrigazione.LeggiImpiantoIrrigazioneDefaultPerImpiantoColturale(params.piva, params.sa_cod, params.appezza, params.id_reg, objParametriServer)
                If dtImpiantiIrrigazione.Rows.Count > 0 Then
                    Dim dr = dtImpiantiIrrigazione.Rows(0) 'dovrebbe avere sempre un solo record
                    PopolaJArrayImpianto(dr, jArrayMacchineIrrigazioneImpianto)
                End If

            End If

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayMacchineIrrigazioneImpianto.FirstOrDefault(), Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Macchine_Irrigazione_Impianto_NG(ByVal InData As CoreWS_Generic(Of Leggi_Macchine_Irrigazione_Impianto)) As RispostaStandard
        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim params As Leggi_Macchine_Irrigazione_Impianto = InData.InData
            Dim jArrayMacchineIrrigazioneImpianto As New JArray()

            Dim objImpiantiMacchine As New AgronicaCoreAnagrafeDAL.Reg_ImpiantiXParcoMacchine_R
            Dim dtImpiantiMacchine = objImpiantiMacchine.ReadJoinDescriptions(objParametriServer, params.piva, params.sa_cod, params.appezza, params.id_reg)

            For Each dr In dtImpiantiMacchine.Rows()
                PopolaJArrayMacchine(dr, jArrayMacchineIrrigazioneImpianto)
            Next

            Dim objImpiantiIrrigazione As New AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R
            Dim dtImpiantiIrrigazione = objImpiantiIrrigazione.Leggi(-1, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)

            For Each dr In dtImpiantiIrrigazione.Rows()
                PopolaJArrayImpianto(dr, jArrayMacchineIrrigazioneImpianto)
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayMacchineIrrigazioneImpianto, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    Private Sub PopolaJArrayMacchine(ByVal dr As DataRow, ByRef jArrayMacchineIrrigazioneImpianto As JArray)
        Dim efficienza = 0.01
        If dr.Item("efficienza") IsNot Nothing AndAlso dr.Item("efficienza") > 0 Then
            efficienza = dr.Item("efficienza")
        End If

        jArrayMacchineIrrigazioneImpianto.Add(New JObject(New JProperty("codice", "0_" & CStr(dr.Item("Mac_Cod"))),
                                      New JProperty("descrizione", dr.Item("Mac_Des")),
                                      New JProperty("flagIsMacchina", True),
                                      New JProperty("efficienza", efficienza),
                                      New JProperty("imp_cod", dr.Item("IMP_COD")),
                                      New JProperty("portata", dr.Item("portata"))))
    End Sub

    Private Sub PopolaJArrayImpianto(ByVal dr As DataRow, ByRef jArrayMacchineIrrigazioneImpianto As JArray)
        jArrayMacchineIrrigazioneImpianto.Add(New JObject(New JProperty("codice", CStr(dr.Item("Imp_Cod")) & "_0"),
                              New JProperty("descrizione", dr.Item("Imp_Des")),
                              New JProperty("flagIsMacchina", False),
                              New JProperty("efficienza", 100),
                              New JProperty("imp_cod", dr.Item("Imp_Cod")),
                              New JProperty("portata", 0)))
    End Sub

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiImpiantoIrrigazione(ByVal InData As CoreWS_Generic(Of Integer)) As RispostaStandard
        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim impCod = InData.InData

            Dim jObject As New JObject()
            Dim objImpiantiIrrigazione As New AgronicaCoreMetaSchemaDAL.ImpiantiIrrigazioni_R
            Dim dtImpiantiIrrigazione = objImpiantiIrrigazione.Leggi(impCod, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer)

            If (dtImpiantiIrrigazione.Rows.Count > 0) Then
                Dim dr = dtImpiantiIrrigazione.Rows(0)
                jObject = New JObject(New JProperty("codice", CStr(dr.Item("Imp_Cod")) & "_0"),
                                              New JProperty("descrizione", dr.Item("Imp_Des")),
                                              New JProperty("flagIsMacchina", False),
                                              New JProperty("efficienza", 100),
                                              New JProperty("portata", 0))
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(jObject, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getStringoneXml(ByVal Parco_Macchina As Parco_Macchine,
                                    ByVal ASG_ProgressivoGIAS As Integer,
                                    ByVal objParametri As String) As String

        If Parco_Macchina.Mac_Cod = 0 Then
            Return Parco_Macchina.XML_GeneraStringoneFinale(enum_TipoOperazioneDB.Scrittura, ASG_ProgressivoGIAS, Utility.convertStringtoOBJparametri(objParametri))
        Else
            Return Parco_Macchina.XML_GeneraStringoneFinale(enum_TipoOperazioneDB.Modifica, ASG_ProgressivoGIAS, Utility.convertStringtoOBJparametri(objParametri))
        End If

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getPotenzaUdm() As List(Of rispostaValore_IntTesto)
        Dim ris As New List(Of rispostaValore_IntTesto)
        ris.Add(New rispostaValore_IntTesto With {.valore = 0, .testo = ""})
        ris.Add(New rispostaValore_IntTesto With {.valore = 5001028, .testo = "CV"})
        ris.Add(New rispostaValore_IntTesto With {.valore = 5001027, .testo = "KW"})
        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getAlimentazioneMacchine() As List(Of rispostaValore_IntTesto)
        Dim ris As New List(Of rispostaValore_IntTesto)
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoAlimentazione.NonDefinita, .testo = "Non Definita"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoAlimentazione.Benzina, .testo = "Benzina"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoAlimentazione.Gasolio, .testo = "Gasolio"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoAlimentazione.Metano, .testo = "Metano"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoAlimentazione.Gpl, .testo = "Gpl"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoAlimentazione.Elettricita, .testo = "Elettricita"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoAlimentazione.Olio_Combustibile, .testo = "Olio Combustibile"})
        ris.Add(New rispostaValore_IntTesto With {.valore = enum_Macchine_TipoAlimentazione.Petrolio, .testo = "Petrolio"})

        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getTitoloPossesso() As List(Of rispostaValore_IntTesto)
        Dim ris As New List(Of rispostaValore_IntTesto)
        ris.Add(New rispostaValore_IntTesto With {.valore = 0, .testo = "Altro"})
        ris.Add(New rispostaValore_IntTesto With {.valore = 1, .testo = "Proprietà"})
        ris.Add(New rispostaValore_IntTesto With {.valore = 2, .testo = "Comodato d'uso"})
        ris.Add(New rispostaValore_IntTesto With {.valore = 3, .testo = "Affitto con contratto"})
        ris.Add(New rispostaValore_IntTesto With {.valore = 4, .testo = "Affitto senza contratto"})
        ris.Add(New rispostaValore_IntTesto With {.valore = 5, .testo = "In conto terzi"})
        ris.Add(New rispostaValore_IntTesto With {.valore = 6, .testo = "In convenzione"})
        Return ris
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetTipoMacchine(objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard


        Try
            Dim objParametriServer As AgronicaCoreParametri
            If objP_server = "" Then
                objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametriServer = Utility.convertStringtoOBJparametri(objP_server)
            End If

            Dim dll As New DropDownList

            Dim rval = AgronicaCoreUtility.CaricaListControl.TipoMacchineCompresso(objParametriServer)

            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception
            r.RispostaOK = False
            Dim messaggioErrore As String = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.Errore = messaggioErrore

        End Try

        Return r
    End Function
#End Region

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Macchine_Anagrafica(ByVal objP_super_server As String,
                                                  ByVal objP_server As String,
                                                  ByVal objP_utenti As String,
                                                  ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objParametriAgenda = InData

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data.Date, objParametriAgenda.Data.Date)
            End If

            Dim dt As DataTable = objMacchine.leggi_x_anagrafica(objParametriAgenda.Piva, "", " Parco_Macchine.Data_Modifica DESC ", objParametri_Server)

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.ResettaFinestra()
            End If

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Piva", "Piva", "string"))
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number"))
            l.Add(New ColonneNome("Mac_Cod", "Mac_Cod", "number"))
            l.Add(New ColonneNome("Cod_Contatto", "Cod_Contatto", "string") With {._hidden = True})
            l.Add(New ColonneNome("Contatto_Des", "Contatto", "string"))
            l.Add(New ColonneNome("tipologia", "tipologia", "string"))
            l.Add(New ColonneNome("CLASS_CODE", "Tipo", "string"))
            l.Add(New ColonneNome("Ditta_Des", "Ditta_Des", "string"))
            l.Add(New ColonneNome("Ditta_Cod", "Marca", "number"))
            l.Add(New ColonneNome("Modello", "Modello", "string"))
            l.Add(New ColonneNome("Macchina", "Macchina", "string"))
            l.Add(New ColonneNome("Telaio", "Telaio", "string"))
            l.Add(New ColonneNome("Targa", "Targa", "string"))
            l.Add(New ColonneNome("Codice", "Codice", "string"))

            l.Add(New ColonneNome("Validita_Inizio", "Validita Inizio", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Validita Fine", "date"))

            l.Add(New ColonneNome("Attivo", "Attivo", "number"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))
            l.Add(New ColonneNome("Visibilita", "Visibilità", "string"))


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Macchine_Anagrafica_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabBIZ.Parco_Macchine_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametriAgenda = InData.InData

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(objParametriAgenda.Data.Date, objParametriAgenda.Data.Date)
            End If

            Dim dt As DataTable = objMacchine.ReadMachinesRegistryNg(objParametriAgenda, objParametri_Server, orderBy:=" Parco_Macchine.Data_Modifica DESC ")

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.ResettaFinestra()
            End If

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Piva", "Piva", "string"))
            l.Add(New ColonneNome("rag_soc", "rag_soc", "string"))
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number"))
            l.Add(New ColonneNome("Mac_Cod", "Mac_Cod", "number"))
            l.Add(New ColonneNome("Cod_Contatto", "Cod_Contatto", "string") With {._hidden = True})
            l.Add(New ColonneNome("Contatto_Des", "Contatto", "string"))
            l.Add(New ColonneNome("tipologia", "tipologia", "string"))
            l.Add(New ColonneNome("CLASS_CODE", "Tipo", "string"))
            l.Add(New ColonneNome("AGEA_Des", "AGEA_Des", "string"))
            l.Add(New ColonneNome("Agea_Cod", "Agea_Cod", "string"))
            l.Add(New ColonneNome("Ditta_Des", "Ditta_Des", "string"))
            l.Add(New ColonneNome("Ditta_Cod", "Marca", "number"))
            l.Add(New ColonneNome("Modello", "Modello", "string"))
            l.Add(New ColonneNome("Macchina", "Macchina", "string"))
            l.Add(New ColonneNome("Telaio", "Telaio", "string"))
            l.Add(New ColonneNome("Targa", "Targa", "string"))
            l.Add(New ColonneNome("Codice", "Codice", "string"))

            l.Add(New ColonneNome("Validita_Inizio", "Validita Inizio", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Validita Fine", "date"))

            l.Add(New ColonneNome("Ultima_Manutenzione", "Ultima_Manutenzione", "date"))
            l.Add(New ColonneNome("Ultima_Revisione", "Ultima_Revisione", "date"))

            l.Add(New ColonneNome("Attivo", "Attivo", "number"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))
            l.Add(New ColonneNome("Visibilita", "Visibilità", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function ReadMachinesByClassCode(ByVal InData As CoreWS_Generic(Of InData.Anagrafica.MachinesXTypeReadParams)) As RispostaStandard
        Dim resp As New RispostaStandard

        If InData.objP.objP_server = "" Then
            resp.Errore = "objP_server non valorizzato"
            Return resp
        End If

        If InData.objP.objP_utenti = "" Then
            resp.Errore = "objP_utenti non valorizzato"
            Return resp
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabBIZ.Parco_Macchine_R
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim params = InData.InData

            Dim dt = objMacchine.ReadMachinesByClassCode(params, objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            resp.RispostaOK = True
            resp.RispostaStringa = JsonConvert.SerializeObject(dt, serializerSettings)
        Catch ex As GiasException
            resp.RispostaOK = True
            resp.RispostaStringa = Nothing
            resp.Errore = ex.Message
        Catch ex As Exception
            resp.RispostaOK = False
            resp.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return resp
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Caratteristiche_Macchina_Anagrafica(ByVal InData As CoreWS_Generic(Of String)) As rispostaStandard(Of List(Of BaseCodeDescr))

        Dim r As New rispostaStandard(Of List(Of BaseCodeDescr))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabBIZ.Parco_Macchine_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim m As DataTable
            Dim risp As New List(Of BaseCodeDescr)

            m = objMacchine.Leggi_Caratteristiche_Macchina(InData.InData, objParametri_Server)

            For Each row As DataRow In m.Rows
                Dim newBCD = New BaseCodeDescr(row.Item("Mac_Car_Cod"), row.Item("Mac_Car_Des") + " (" + row.Item("UDM_SIM") + ")")
                risp.Add(newBCD)
            Next row

            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Macchina_Anagrafica(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabBIZ.Parco_Macchine_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim m As ParcoMacchine

            m = objMacchine.Leggi_Macchina(InData.InData.Piva, InData.InData.Mac_Cod, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = m

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Scrivi_Macchina_Anagrafica(ByVal macchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine,
                                               ByVal tipoOperazione As AgronicaCoreDataProvider.TipiEnumerativi.enum_TipoOperazioneDB,
                                               ByVal objP_super_server As String,
                                               ByVal objP_server As String,
                                               ByVal objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchinaBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine = macchina
            Dim risp = objMacchinaBIZ.Scrivi_Macchina_Anagrafica(objMacchina,
                                                                 tipoOperazione,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti
                                                                 )


            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(macchina, Newtonsoft.Json.Formatting.None)


        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(macchina, Newtonsoft.Json.Formatting.None)
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}

        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Scrivi_Macchina_Anagrafica(ByVal InData As CoreWS_Generic(Of Scrivi_Macchina_Anagrafica)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r

        End If

        Try
            Dim objMacchinaBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objMacchina As AgronicaCoreModelsSTD.anagrafiche.ParcoMacchine = InData.InData.macchina
            Dim risp = objMacchinaBIZ.Scrivi_Macchina_Anagrafica(
                objMacchina,
                InData.InData.tipoOperazione,
                objParametri_Server,
                objParametri_Utenti
                )


            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData.macchina, Newtonsoft.Json.Formatting.None)

        Catch ex As GiasException
            r.RispostaStringa = ""
            r.Errore = ex.Message
            r.RispostaOK = False
            Return r
        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData.macchina, Newtonsoft.Json.Formatting.None)
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function

#Region "Movimenti Macchina"

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function IsMacchinaMovimentata(ByVal InData As CoreWS_Generic(Of ParcoMacchine)) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            If ParcoMacchineUtility.VerificaMovimentiMacchina(InData.InData, objParametri_Server, True) Then
                r.RispostaOK = True
                r.RispostaStringa = True
                r.Errore = "La macchina è stata movimentata"
            Else
                r.RispostaOK = True
                r.RispostaStringa = False
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function IsEditAllowed(ByVal InData As CoreWS_Generic(Of ParcoMacchine)) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            If ParcoMacchineUtility.IsEditAllowed(InData.InData, objParametri_Server) Then
                r.RispostaOK = True
                r.RispostaStringa = True
            Else
                r.RispostaOK = True
                r.RispostaStringa = False
                r.Errore = "La macchina è stata movimentata"
            End If
        Catch ex As GiasException
            r.RispostaOK = True
            r.RispostaStringa = False
            r.Errore = ex.Message
        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CentresOnWhichIsUsed(ByVal InData As CoreWS_Generic(Of ParcoMacchine)) As rispostaStandard(Of List(Of Integer))
        Dim r As New rispostaStandard(Of List(Of Integer))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            r.RispostaStringa = ParcoMacchineUtility.CentresOnWichIsUsed(InData.InData, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = New List(Of Integer)
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CompaniesByWichIsUsed(ByVal InData As CoreWS_Generic(Of ParcoMacchine)) As rispostaStandard(Of List(Of String))
        Dim r As New rispostaStandard(Of List(Of String))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            r.RispostaStringa = ParcoMacchineUtility.CompaniesByWhichIsUsed(InData.InData, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = New List(Of String)
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

#End Region

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Test_Macchine_Archivio_Lettura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine As New AgronicaCoreContabBIZ.Parco_Macchine_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim m As ParcoMacchine

            Dim objMacchine_R As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim dt As DataTable = objMacchine_R.leggi_x_anagrafica(InData.InData.Piva, "", " Parco_Macchine.Data_Modifica DESC ", objParametri_Server)

            r.ErroriGias = New List(Of ErroreGias)

            For Each row In dt.Rows
                Try
                    m = objMacchine.Leggi_Macchina(row("Piva"), row("Mac_Cod"), objParametri_Server)
                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .messaggio = "Errore Macchina " & row("Piva") & "_" & row("Mac_Cod")}
                                     )

                End Try
            Next

            r.RispostaOK = True

            If r.ErroriGias.Count > 0 Then
                r.RispostaOK = False
            End If

            r.RispostaStringa = True

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Test_Macchine_Archivio_Scrittura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objMacchine_BIZ_R As New AgronicaCoreContabBIZ.Parco_Macchine_R
            Dim objMacchine_BIZ_W As New AgronicaCoreContabBIZ.Parco_Macchine_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim m As New ParcoMacchine

            Dim objMacchine_R As New AgronicaCoreContabDAL.Parco_Macchine_R

            Dim dt As DataTable = objMacchine_R.leggi_x_anagrafica(InData.InData.Piva, "", " Parco_Macchine.Data_Modifica DESC ", objParametri_Server)

            r.ErroriGias = New List(Of ErroreGias)

            For Each row In dt.Rows
                Try
                    m = objMacchine_BIZ_R.Leggi_Macchina(row("Piva"), row("Mac_Cod"), objParametri_Server)
                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Macchina " & row("Piva") & "_" & row("Mac_Cod")}
                    )

                End Try

                Try
                    objMacchine_BIZ_W.Scrivi_Macchina_Anagrafica(m, enum_TipoOperazioneDB.Modifica, objParametri_Server, objParametri_Utenti)
                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Macchina " & row("Piva") & "_" & row("Mac_Cod")}
                    )

                End Try

            Next

            r.RispostaOK = True

            If r.ErroriGias.Count > 0 Then
                r.RispostaOK = False
            End If

            r.RispostaStringa = True

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaAssiciazioneBTM(ByVal codice As Integer, ByVal VIN As String, ByVal BTM_Serial As String, ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objMacchinaBIZ As New AgronicaCoreContabBIZ.Parco_Macchine_W
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            r.RispostaStringa = objMacchinaBIZ.Verifica_Associazione_BTM(codice, VIN, BTM_Serial, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CaricaListaMacchine(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.attivita.ModificaMultipla_Attivita)) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim data As AgronicaCoreModelsSTD.attivita.ModificaMultipla_Attivita = (InData.InData)

            If objParametri_Server Is Nothing Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If
            If objParametri_Utenti Is Nothing Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If
            If objParametri_Super_Server Is Nothing Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            Dim macchineR As New AgronicaCoreContabDAL.Parco_Macchine_R
            Dim DT_Macchine = macchineR.CaricaListaMacchine(data.Attivita_list, data.Solo_Aziendali, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT_Macchine)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function
End Class