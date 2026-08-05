Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreScadenziario
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtentiDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Alert_Area
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_AreePerAzienda(ByVal objP_server As String, ByVal piva As String, ByVal soloPrivate As Boolean, ByVal controllaSeUtenteAutorizzato As Boolean, ByVal tipoPermessoDaControllare As Integer, ByVal Filtro As String) As RispostaStandard
        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'Leggo le pive padri (in mainera ricorsiva) del figlio
            'Dim gi As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
            'Dim pivePadre As String = gi.Ricava_Stringa_PivePadre_Ricorsivo(piva, "", objParametri_Server)

            'If pivePadre.Trim <> "" Then
            '    pivePadre = "(" & pivePadre & ")"
            'End If

            'Leggo le Aree della Piva in oggetto e di tutti i suoi padri
            Dim objArea As New AgronicaCoreScadenziario.Alert_Area_R
            Dim dt As DataTable = objArea.LeggiPerPiva(piva, "", soloPrivate, objParametri_Server, controllaSeUtenteAutorizzato, tipoPermessoDaControllare, Filtro)

            'Creo una lista di oggetti con l'elenco delle aree
            Dim JArrayListaOp As New JArray()
            For Each dr In dt.Rows
                JArrayListaOp.Add(New JObject(New JProperty("id_area", dr.Item("id_area")),
                                              New JProperty("nome", dr.Item("nome")),
                                              New JProperty("tipoentita_cod", dr.Item("tipoentita_cod")),
                                              New JProperty("tipoentita_cod_secondario", dr.Item("tipoentita_cod_secondario"))
                                              ))
            Next

            'ritorno l'array trasformato in json 
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
    Public Function Leggi_Aree(ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'Leggo le Aree della Piva in oggetto e di tutti i suoi padri
            Dim objArea As New AgronicaCoreScadenziario.Alert_Area_R
            Dim dt As DataTable = objArea.Leggi("", 0, objParametri_Server)

            'Creo una lista di oggetti con l'elenco delle aree
            Dim JArrayListaOp As New JArray()
            For Each dr In dt.Rows
                JArrayListaOp.Add(New JObject(New JProperty("id_area", dr.Item("id_area")), New JProperty("nome", dr.Item("nome"))))
            Next

            'ritorno l'array trasformato in json 
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r


    End Function


    '<WebMethod()>
    '<Script.Services.ScriptMethod()>
    'Public Function Leggi_RapConScadenze(ByVal objP_server As String) As RispostaStandard

    '    Dim r As New RispostaStandard()

    '    If objP_server = "" Then
    '        r.Errore = "objP_server non valorizzato"
    '        Return r
    '    End If

    '    Try
    '        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

    '        'Leggo i RapCon validi per il documentale
    '        Dim objRapCon As New AgronicaCoreAnagrafeDAL.Rapporti_Contabili_R
    '        Dim dt As DataTable = objRapCon.RapportiContabili_Leggi("", "", objParametri_Server)


    '        'ritorno l'array trasformato in json 
    '        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
    '        r.RispostaStringa = JsonConvert.SerializeObject(dt, Formatting.None)
    '        r.RispostaOK = True

    '    Catch ex As Exception
    '        r.Errore = ex.Message
    '        Return r
    '    End Try

    '    Return r


    'End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Aggiungi(ByVal objP_server As String,
                             ByVal objP_utenti As String,
                             ByVal NomeArea As String,
                             ByVal Piva As String
                             ) As RispostaStandard

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

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, 
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = new Globalization.CultureInfo(linguaCodiceISO)

        Try

            'apro una transazione
            ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            'Verifico se esiste e nel caso estraggo l'id della tipologia
            Dim a_R As New AgronicaCoreScadenziario.Alert_Area_R()
            Dim idArea As Integer? = a_R.LeggiIdDaNomeArea(NomeArea, Piva, objParametri_Server)

            If Not IsNothing(idArea) Then
                r.Errore = Gias.ErroreCategoriaEsistente
                Return r
            End If

            'Salvo la nuova categoria
            Dim seq As New Agro_Sequenze()
            idArea = seq.NuovoId_Tabella("Alert_Area", 100, 2000000000, objParametri_Server)

            Dim a_W As New AgronicaCoreScadenziario.Alert_Area_W()
            Dim res As Boolean = a_W.Scrivi(idArea, NomeArea, 0, Piva, "", objParametri_Server)

            If res = False Then
                r.Errore = Gias.ErroreSalvataggioNuovaCategoria
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
                Return r
            End If

            r.RispostaOK = True
            r.RispostaStringa = Gias.CategoriaSalvataCorrettamente

            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server) 'Flag_Commit1_Rollback2

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server) 'Flag_Commit1_Rollback2
            r.Errore = Gias.ErroreDuePunti_ + ex.Message
            Return r
        End Try

        Return r


    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Modifica(ByVal objP_server As String,
                             ByVal objP_utenti As String,
                             ByVal ID_Area As Integer,
                             ByVal Nome_Area As String,
                             ByVal Piva As String
                             ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If
        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If ID_Area < 10 Then '--------------------------10
            r.Errore = Gias.ImpossibileModificareUnaCategoriaDiSistema
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, 
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = new Globalization.CultureInfo(linguaCodiceISO)

        Dim Colore As String = ""
        Dim Preavviso As Integer = 0
        Dim Cat_Cod As Integer = 0

        Try
            'Ora le categorie sono per installazione non più per singola impresa
            'Dim a_R As New AgronicaCoreScadenziario.Alert_Area_R()
            'Dim dt As DataTable = a_R.Leggi("", ID_Area, objParametri_Server)
            'If dt.Rows(0).Item("Piva") <> Piva Then
            '    r.Errore = Gias.ImpossibileModificareCategoriaCreataDaAltraAzienda
            '    Return r
            'End If


            Dim a_W As New AgronicaCoreScadenziario.Alert_Area_W()
            Dim res As Boolean = a_W.Modifica(ID_Area, Nome_Area, 0, objParametri_Server)

            If res = False Then
                r.Errore = Gias.ErroreDuranteModificaDellaTipologia
                Return r
            End If

            r.RispostaOK = True
            r.RispostaStringa = Gias.CategoriaModificataCorrettamente

        Catch ex As Exception
            r.Errore = Gias.ErroreDuePunti_ + ex.Message
            Return r
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Cancella(ByVal objP_server As String,
                             ByVal objP_utenti As String,
                             ByVal ID_Area As Integer,
                             ByVal Piva As String
                             ) As RispostaStandard

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

        Dim leggiLingua As New Lingue_Read
        Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, 
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Utenti)
        Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
        Threading.Thread.CurrentThread.CurrentUICulture = new Globalization.CultureInfo(linguaCodiceISO)

        If ID_Area < 10 Then '--------------------------10
            r.Errore = Gias.ImpossibileModificareUnaCategoriaDiSistema
            Return r
        End If

        Dim a_R As New AgronicaCoreScadenziario.Alert_Area_R()
        Dim inUso As Boolean = a_R.TestSeAreaInUso(ID_Area, objParametri_Server)

        If inUso = True Then
            r.Errore = Gias.CategoriaConElementiFiglioImpossibileCancellare
            Return r
        End If

        'apro una transazione per la cancellazione
        ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

        Try

            Dim res As Boolean

            Dim a_W As New AgronicaCoreScadenziario.Alert_Area_W()
            res = a_W.Cancella(ID_Area, objParametri_Server)

            If res = False Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                r.Errore = Gias.ErroreCancellazioneCategoria
                Return r
            Else

                'Cancello anche CategTipologiaDocumentiXUtenti (Autorizzazione  Utenti ,Tipologia e Categoria)
                Dim CategTipoxUtenti_W As New AgronicaCoreScadenziario.CategTipologiaDocumentiXUtenti_W
                res = CategTipoxUtenti_W.Cancella("", "", ID_Area, 0, objParametri_Server)

                If res = False Then
                    ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                    r.Errore = Gias.ErroreCancellazioneCategoria
                    Return r
                End If
            End If

            ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = Gias.CategoriaCancellataCorrettamente

        Catch ex As Exception
            ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            r.Errore = ex.Message
            Return r
        Finally
            'Se è andato tutto bene
            ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
        End Try

        Return r

    End Function

End Class