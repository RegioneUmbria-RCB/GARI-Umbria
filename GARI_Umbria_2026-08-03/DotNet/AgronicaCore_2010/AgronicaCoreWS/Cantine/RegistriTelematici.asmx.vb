Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreRegVinoDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreRegVinoBIZ

'<System.Web.Script.Services.ScriptService()> _

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class RegistriTelematici
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function WS_LeggiAnagrafiche(
                            ByVal Piva As String,
                            ByVal SaCod As Integer,
                            ByVal TipoAnagrafica As Integer,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByVal ChiamataDaGiasLan As Boolean,
                            ByVal objP_server As String,
                            ByVal CodOper As String,
                            ByVal codIcqrf As String) As String


        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Try

            'Inserire il codice QUI..
            Dim strRisposta As String = ""
            'Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_R
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.Cantina_RegistriTelematici
            Dim dt As DataTable



            'dt = objCoreStampeDAL.LeggiAnagrafiche( _
            '                        Piva, _
            '                        SaCod, _
            '                        TipoAnagrafica, _
            '                        xFiltroAggiuntivo, xOrderBy, _
            '                        ChiamataDaGiasLan, objParametri_Server, "", strRisposta)

            Dim filtroAggiuntivo As String = ""
            If xFiltroAggiuntivo <> "" Then
                filtroAggiuntivo = "( " + xFiltroAggiuntivo + " )"
            End If
            dt = objCoreStampeDAL.LeggiAnagrafiche(Piva, 0, TipoAnagrafica, filtroAggiuntivo, xOrderBy, ChiamataDaGiasLan, objParametri_Server, CodOper, codIcqrf, "", strRisposta, DateTime.Now)

            r.RispostaOK = True
            r.RispostaStringa = strRisposta

            Select Case TipoAnagrafica
                Case 0
                    r.RispostaStringa = DammiWaTableSoggetti(dt)
                Case 1
                    r.RispostaStringa = DammiWaTableVigne(dt)
                Case 2
                    r.RispostaStringa = DammiWaTableVasi(dt)
                Case 3
                    r.RispostaStringa = DammiWaTableProdotti(dt)
            End Select



        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function WS_LeggiOperazioni(
                            ByVal Piva As String,
                            ByVal icqrf As String,
                            ByVal CodOperazione As String,
                            ByVal dataInizio As Date,
                            ByVal dataFine As Date,
                            ByVal xFiltroAggiuntivo As String,
                            ByVal xOrderBy As String,
                            ByVal ChiamataDaGiasLan As Boolean,
                            ByVal objP_server As String
                            ) As String


        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Try

            'Inserire il codice QUI..
            Dim strRisposta As String = ""
            'Dim objCoreDAL As New AgronicaCoreContabDAL.Cespiti_R
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.Cantina_RegistriTelematici
            Dim dt As DataTable


            dataInizio = dataInizio.AddDays(1)
            dataFine = dataFine.AddDays(1)
            dataInizio = New Date(dataInizio.Year, dataInizio.Month, dataInizio.Day, 0, 0, 0)
            dataFine = New Date(dataFine.Year, dataFine.Month, dataFine.Day, 23, 59, 59)
            Dim dataI = New DateTime(dataInizio.Year, dataInizio.Month, dataInizio.Day, 0, 0, 0)
            Dim dataF = New DateTime(dataFine.Year, dataFine.Month, dataFine.Day, 23, 59, 59)
            dt = objCoreStampeDAL.LeggiOperazioni(
                                    Piva,
                                    icqrf,
                                    CodOperazione,
                                    dataI,
                                    dataF,
                                    xFiltroAggiuntivo, xOrderBy,
                                    objParametri_Server, strRisposta)


            r.RispostaOK = True

            r.RispostaStringa = DammiWaTableOperazioni(dt)


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function WS_LeggiCodiciIcqrf(
                            ByVal codOper As String,
                            ByVal objP_server As String
                            ) As String
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim strRisposta = ""

            Dim ddlApp As New DropDownList
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.Cantina_RegistriTelematici
            Dim piva = objCoreStampeDAL.getCodOperFromPivaSu(codOper, objParametri_Server)
            AgronicaCoreUtility.CaricaListControl.ListaIcqrf(ddlApp, True, "Tutti", "", piva, "", "", objParametri_Server)

            Dim rval1 As New List(Of String)
            Dim rval As String = "{""op"":["
            For Each elem As ListItem In ddlApp.Items
                rval1.Add("{""value"": """ & elem.Value & """, ""text"":""" & elem.Text & """} ")
            Next
            rval &= String.Join("," & vbCrLf, rval1.ToArray)
            rval &= "]}"

            r.RispostaOK = True
            r.RispostaStringa = rval

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function WS_InserisciAggiorna(
                            ByVal ID As String,
                            ByVal pivaSu As String,
                            ByVal saCod As Integer,
                            ByVal codOper1 As String,
                            ByVal codIcqrf1 As String,
                            ByVal tipo As Integer,
                            ByVal objP_server As String
                            ) As String
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim strRisposta = ""
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.Cantina_RegistriTelematici
            r.RispostaOK = True
            Select Case tipo
                Case 1 'Vasi
                    strRisposta += IAVaso(ID, pivaSu, codIcqrf1, codOper1, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case 2 'Vigne
                    strRisposta += IAVigna(ID, codOper1, codIcqrf1, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case 3 'Soggetti
                    strRisposta += IASoggetto(ID, pivaSu, codOper1, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case 4 'Operazioni
                    strRisposta += IAOperazione(ID, codOper1, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case 5 'Prodotti
                    strRisposta += IAProdotto(ID, codOper1, codIcqrf1, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case Else
                    strRisposta = "Non Gestito"
            End Select

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function WS_CambiaRichiesta(
                            ByVal ID As String,
                            ByVal pivaSu As String,
                            ByVal saCod As Integer,
                            ByVal codOper1 As String,
                            ByVal codIcqrf1 As String,
                            ByVal tipo As Integer,
                            ByVal objP_server As String,
                            ByVal richiesta As String,
                            ByVal NewId As String
                            ) As String
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim strRisposta = ""
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.Cantina_RegistriTelematici
            r.RispostaOK = True
            Select Case tipo
                Case 1 'Vasi
                    strRisposta += CambiaRichiestaVaso(ID, pivaSu, codIcqrf1, codOper1, objCoreStampeDAL, objParametri_Server, richiesta) & vbCrLf
                Case 2 'Vigne
                    'strRisposta += CambiaRichiestaVigna(ID, codOper1, codIcqrf1, objCoreStampeDAL, objParametri_Server, richiesta) & vbCrLf
                Case 3 'Soggetti
                    strRisposta += CambiaRichiestaSoggetto(ID, pivaSu, codOper1, objCoreStampeDAL, objParametri_Server, richiesta) & vbCrLf
                Case 4 'Operazioni
                    strRisposta += CambiaRichiestaOperazione(ID, codOper1, objCoreStampeDAL, objParametri_Server, richiesta, NewId) & vbCrLf
                Case 5 'Prodotti
                    'strRisposta += CambiaRichiestaProdotto(ID, codOper1, codIcqrf1, objCoreStampeDAL, objParametri_Server, richiesta) & vbCrLf
                Case Else
                    strRisposta = "Non Gestito"
            End Select

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function WS_VisualizzaErrore(ByVal codOper As String, ByVal CodIcqrf As String, ByVal id As String, ByVal tipo As Integer, ByVal objP_server As String) As String
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim strRisposta = ""
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.Cantina_RegistriTelematici
            Dim DTErroreSingolo = objCoreStampeDAL.getErroreSingolo(codOper, CodIcqrf, id, tipo, objParametri_Server)
            If DTErroreSingolo IsNot Nothing AndAlso DTErroreSingolo.Rows.Count > 0 Then
                strRisposta += DTErroreSingolo.Rows(0).Item("CodiceErrore") + " - " + DTErroreSingolo.Rows(0).Item("DescrizioneErrore")
            Else
                strRisposta += objCoreStampeDAL.getErrore(codOper, CodIcqrf, id, tipo, objParametri_Server)
            End If
            r.RispostaOK = True
            r.RispostaStringa = strRisposta
        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = ex.Message
            r.Errore = ex.Message
        End Try
        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function WS_Elimina(
                            ByVal ID As String,
                            ByVal pivaSu As String,
                            ByVal saCod As Integer,
                            ByVal codOper1 As String,
                            ByVal codIcqrf1 As String,
                            ByVal tipo As Integer,
                            ByVal objP_server As String
                            ) As String
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim strRisposta = ""
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.Cantina_RegistriTelematici
            r.RispostaOK = True
            Select Case tipo
                Case 1 'Vasi
                    'Dim codOper = objCoreStampeDAL.getCodOperFromPivaSu(pivaSu, objParametri_Server)
                    'Dim codIcqrf = objCoreStampeDAL.getCodIcqrfFromSaCod(pivaSu, saCod, objParametri_Server)
                    strRisposta += EVaso(ID, codOper1, codIcqrf1, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case 2 'Vigne
                    'strRisposta += EVigna(ID, pivaSu, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case 3 'Soggetti
                    Dim codOper = objCoreStampeDAL.getCodOperFromPivaSu(pivaSu, objParametri_Server)
                    strRisposta += ESoggetto(ID, codOper, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case 4 'Operazioni
                    strRisposta += EOperazione(ID, pivaSu, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case 5 'Prodotti
                    'Dim codOper = objCoreStampeDAL.getCodOperFromPivaSu(pivaSu, objParametri_Server)
                    'Dim codIcqrf = objCoreStampeDAL.getCodIcqrfFromSaCod(pivaSu, saCod, objParametri_Server)
                    strRisposta += EProdotto(ID, codOper1, codIcqrf1, objCoreStampeDAL, objParametri_Server) & vbCrLf
                Case Else
                    strRisposta = "Non Gestito"
            End Select

            r.RispostaStringa = strRisposta
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaStringa
    End Function

    <WebMethod()>
    Public Function WS_AggiornaCatalogoProdotto(
                            ByVal objP_server As String,
                            ByVal codOper As String
                            ) As String

        Dim r As New RispostaStandard
        Try
            Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            Dim writer As New AgronicaCoreRegVinoDAL.xDBProdottiSiRPV_W

            writer.ResettaCodiciSIAN(objParametri_Server)
            writer.aggiornaStato(objParametri_Server, "", "", codOper, AgronicaCoreRegVinoBIZ.Utility.StatoGIAS.Aggiornamento_Prodotto)

            r.RispostaStringa = "Aggiornamento Avviato"

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaStringa
    End Function

    Public Function WS_GetSaCodFromIcqrf(ByVal piva As String, ByVal codIcqrf As String, ByVal objP_server As String) As String
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
        Try
            Dim strRisposta = ""
            Dim objCoreStampeDAL As New AgronicaCoreStampeDAL.Cantina_RegistriTelematici
            r.RispostaOK = True
            Dim sa_cod As String = objCoreStampeDAL.getSaCodFromCodIcqrf(piva, codIcqrf, objParametri_Server)
            r.RispostaStringa = sa_cod
        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
            r.RispostaStringa = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r.RispostaStringa
    End Function

    Private Function getListID(ids As String) As List(Of String)
        Dim idList As New List(Of String)
        Dim arr = ids.Split("|")
        For Each el As String In arr
            el = el.Trim
            If el <> "" Then
                idList.Add(el)
            End If
        Next
        Return idList
    End Function

    Function DammiWaTableSoggetti(ByVal dt As DataTable) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("CodOper", "CodOper", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("CodiceSoggetto", "Codice Soggetto", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("CodiceCUAA", "Cuaa", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1500"
        l.Add(c)

        c = New ColonneNome("TipoSoggetto", "Tipo", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "300"
        l.Add(c)

        c = New ColonneNome("Nome", "Nome", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("Cognome", "Cognome", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("Rag_Soc", "Ragione Sociale", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "4400"
        l.Add(c)

        c = New ColonneNome("IndirizzoSede_CAP", "CAP", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "700"
        l.Add(c)

        c = New ColonneNome("IndirizzoSede_Indirizzo", "Indirizzo", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "2500"
        l.Add(c)

        c = New ColonneNome("IndirizzoSede_Provincia", "Prov", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "500"
        l.Add(c)

        c = New ColonneNome("IndirizzoSede_Comune", "Comune", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1500"
        l.Add(c)

        c = New ColonneNome("IndirizzoSede_Stato", "Nazione", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "300"
        l.Add(c)

        c = New ColonneNome("TipoRichiesta", "R", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "300"
        l.Add(c)

        c = New ColonneNome("Stato", "Stato", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "3500"
        l.Add(c)

        c = New ColonneNome("S", "Sel", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "350"
        l.Add(c)

        c = New ColonneNome("ID", "ID", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("CodOpers", "CodOpers", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("IDStato", "IDStato", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    Function DammiWaTableVigne(ByVal dt As DataTable) As String
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("CodVigna", "CodVigna", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("Descrizione", "Descrizione", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("TipoRichiesta", "R", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "300"
        l.Add(c)

        c = New ColonneNome("stato", "stato", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "4000"
        l.Add(c)

        c = New ColonneNome("S", "Sel", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "350"
        l.Add(c)

        c = New ColonneNome("IDStato", "IDStato", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    Function DammiWaTableVasi(ByVal dt As DataTable) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("CodOper", "CodOper", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("codIcqrf", "codIcqrf", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("CodVaso", "Identificativo", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("TipoVaso", "TipoVaso", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)


        c = New ColonneNome("Descrizione", "Descrizione", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "4000"
        l.Add(c)

        c = New ColonneNome("Volume", "Volume", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("TipoRichiesta", "R", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "300"
        l.Add(c)

        c = New ColonneNome("StatoSIAN", "Stato SIAN", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "4000"
        l.Add(c)

        c = New ColonneNome("S", "Sel", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "350"
        l.Add(c)

        c = New ColonneNome("ID", "ID", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("Icqrf", "Icqrf", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("IDStato", "IDStato", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("Sa_Cod", "Sa_Cod", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("vas_cod", "vas_cod", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    Function DammiWaTableOperazioni(ByVal dt As DataTable)
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("CodOper", "Cod Oper", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("CodiceIcqrf", "Cod Icqrf", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("CodiceOperazione", "Op", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "500"
        l.Add(c)

        c = New ColonneNome("TipoRichiesta", "R", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "300"
        l.Add(c)

        c = New ColonneNome("NumOperazione", "N Op", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "700"
        l.Add(c)


        c = New ColonneNome("DataOperazione", "Data", "date")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("des_lib", "Descrizione", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "9500"
        l.Add(c)

        c = New ColonneNome("WAnagraficaStati_Des", "Stato SIAN", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "4500"
        l.Add(c)

        c = New ColonneNome("S", "Sel", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "350"
        l.Add(c)

        c = New ColonneNome("ID", "ID", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("IDStato", "IDStato", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("ID_Agenda", "ID_Agenda", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    Function DammiWaTableProdotti(ByVal dt As DataTable)
        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("CodOper", "Cod Oper", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        'c = New ColonneNome("CodOper_Fisiche_Giuridiche", "FG", "string")
        'c._placeHolder = "..."
        'c._FormatoParticolare = "300"
        'l.Add(c)

        c = New ColonneNome("TipoRichiesta", "R", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "300"
        l.Add(c)

        c = New ColonneNome("CodIcqrf", "Codice Icqrf", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "1000"
        l.Add(c)

        c = New ColonneNome("Mat_Des", "Mat_Des", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "8000"
        l.Add(c)

        c = New ColonneNome("Lotto", "Lotto", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "2000"
        l.Add(c)

        c = New ColonneNome("CodPrimario", "CodPrimario", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "2000"
        l.Add(c)

        c = New ColonneNome("CodSecondario", "CodSecondario", "int")
        c._placeHolder = "..."
        c._FormatoParticolare = "2000"
        l.Add(c)

        c = New ColonneNome("StatoSIAN", "StatoSIAN", "date")
        c._placeHolder = "..."
        c._FormatoParticolare = "2000"
        l.Add(c)

        'c = New ColonneNome("S", "Sel", "string")
        'c._placeHolder = "..."
        'c._FormatoParticolare = "350"
        'l.Add(c)

        c = New ColonneNome("IDStato", "IDStato", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("Mat_Cod", "Mat_Cod", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        c = New ColonneNome("modificato", "modificato", "string")
        c._placeHolder = "..."
        c._FormatoParticolare = "0"
        l.Add(c)

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable(dt, l)
        Return risp
    End Function

    Function IAVigna(id As String, pivasu As String, codOper As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim msg As String = id + ": ricevuto"

        Return msg
    End Function

    Function EVigna(id As String, pivasu As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim msg As String = id + ": ricevuto"

        Return msg
    End Function

    Function IAVaso(id As String, pivasu As String, codIcqrf1 As String, codOper1 As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim reader As New xDBVasiSiRPV_R
        Dim writer As New xDBVasiSiRPV_W
        Dim msg As String = ""
        msg += id + ": "
        ' 1 Guardo se è presente una riga nelle tabelle di transcodifica
        Dim dti = reader.LeggiWs_RegVino_Vasi(objParametri_Server, codOper1, "", codIcqrf1, id)
        Dim canInsertRefresh = False
        Dim nuovaRichiesta As String = ""
        possoInserireAggiornare(dti, canInsertRefresh, nuovaRichiesta, msg)

        If canInsertRefresh Then
            If checkCuaa(codOper1) = "" Then
                If checkICQRF(codIcqrf1) = "" Then
                    Dim dtVaso = objCoreStampeDAL.LeggiAnagrafiche(pivasu, 0, 2, "", "", False, objParametri_Server, codOper1, codIcqrf1, id)
                    If dtVaso.Rows.Count > 0 Then
                        Dim vaso = dtVaso.Rows(0)

                        Dim codOper As String = codOper1
                        Dim codOper_FisicheGiuridiche As String = ""
                        If codOper.Length = 11 Then
                            codOper_FisicheGiuridiche = "G"
                        Else
                            codOper_FisicheGiuridiche = "F"
                        End If
                        Dim codIcqrf As String = vaso.Item("CodIcqrf").ToString.Replace("-", "").Replace("/", "").Replace("\", "")
                        Dim codVaso As String = vaso.Item("CodVaso")
                        Dim TipoVaso As String = vaso.Item("TipoVaso")
                        Dim Descrizione As String = vaso.Item("Descrizione")
                        Dim Volume As Double = vaso.Item("Volume")
                        Dim TipoRichiesta As String = nuovaRichiesta
                        Dim Gias_Stato As Integer = 18000001

                        writer.InserisciAggiornaWs_RegVino_Vasi(objParametri_Server, codOper, codOper_FisicheGiuridiche, codIcqrf, codVaso, TipoVaso, Descrizione, Volume, TipoRichiesta, Gias_Stato)
                    Else

                    End If
                Else
                    msg = id + ": " + checkICQRF(codIcqrf1)
                End If
            Else
                msg = id + ": " + checkCuaa(codOper1)
            End If
        End If

        Return msg
    End Function

    Function CambiaRichiestaVaso(id As String, pivasu As String, codIcqrf1 As String, codOper1 As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri, richiesta As String) As String
        Dim reader As New xDBVasiSiRPV_R
        Dim writer As New xDBVasiSiRPV_W
        Dim msg As String = ""
        msg += id + ": "
        Dim dtVaso = objCoreStampeDAL.LeggiAnagrafiche(pivasu, 0, 2, "", "", False, objParametri_Server, codOper1, codIcqrf1, id)
        If dtVaso.Rows.Count > 0 Then
            Dim vaso = dtVaso.Rows(0)
            Dim codOper As String = codOper1
            Dim codOper_FisicheGiuridiche As String = "G"
            Dim codIcqrf As String = vaso.Item("CodIcqrf").ToString.Replace("-", "").Replace("/", "").Replace("\", "")
            Dim codVaso As String = vaso.Item("CodVaso")
            Dim TipoVaso As String = vaso.Item("TipoVaso")
            Dim Descrizione As String = vaso.Item("Descrizione")
            Dim Volume As Double = vaso.Item("Volume")
            Dim TipoRichiesta As String = richiesta
            msg += richiesta
            Dim Gias_Stato As Integer = 18000000
            writer.InserisciAggiornaWs_RegVino_Vasi(objParametri_Server, codOper, codOper_FisicheGiuridiche, codIcqrf, codVaso, TipoVaso, Descrizione, Volume, TipoRichiesta, Gias_Stato)
        End If

        Return msg
    End Function

    Function EVaso(id As String, pivasu As String, codIcqrf As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim reader As New xDBVasiSiRPV_R
        Dim writer As New xDBVasiSiRPV_W
        Dim msg As String = ""
        msg += id + ": "
        ' 1 Guardo se è presente una riga nelle tabelle di transcodifica
        Dim dti = reader.LeggiWs_RegVino_Vasi(objParametri_Server, pivasu, , codIcqrf, id, , , , , )
        Dim canDelete = False
        Dim nuovaRichiesta As String = ""
        possoEliminare(dti, canDelete, nuovaRichiesta, msg)
        If canDelete Then
            writer.EliminaWs_RegVino_Vasi(objParametri_Server, pivasu, codIcqrf, id)
        End If
        Return msg
    End Function

    Function IASoggetto(id As String, pivasu As String, codOper1 As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim reader As New xDBSoggSiRPV_R
        Dim writer As New xDBSoggSiRPV_W
        Dim msg As String = ""
        Dim erroreStr As String = ""
        msg += id + ": "
        ' 1 Guardo se è presente una riga nelle tabelle di transcodifica
        Dim dti = reader.Leggiws_RegVino_Soggetti(objParametri_Server, id, codOper1)
        Dim canInsertRefresh = False
        Dim nuovaRichiesta As String = ""
        possoInserireAggiornare(dti, canInsertRefresh, nuovaRichiesta, msg)
        If canInsertRefresh Then
            If checkCuaa(codOper1) = "" Then
                Dim dtSoggetto = objCoreStampeDAL.LeggiAnagrafiche(pivasu,
                                    0,
                                    0,
                                    "", "",
                                    False, objParametri_Server, codOper1, "", id)
                If dtSoggetto.Rows.Count > 0 Then
                    Dim soggetto = dtSoggetto.Rows(0)
                    Dim errore = False
                    Dim codiceSoggetto As String = soggetto.Item("CodiceSoggetto")
                    If codiceSoggetto = "" Or codiceSoggetto.Contains("/") Or codiceSoggetto.Contains("\") Or codiceSoggetto.Contains("-") Or codiceSoggetto.Contains(".") Then
                        errore = True
                        erroreStr = "Codice Soggetto non valido"
                    End If
                    Dim codOper As String = codOper1
                    Dim codOper_FisicheGiuridiche As String = "G"
                    Dim cuaa As String
                    Dim cuaa_fisiche As Boolean
                    Dim nome As String = ""
                    Dim cognome As String = ""
                    Dim rag_soc As String = ""
                    If (Not IsDBNull(soggetto.Item("CUAA_PersonaFisica"))) AndAlso soggetto.Item("CUAA_PersonaFisica") <> "" Then
                        cuaa = soggetto.Item("CUAA_PersonaFisica")
                        If cuaa = "" Or cuaa.Contains("/") Or cuaa.Contains("\") Or cuaa.Contains("-") Or cuaa.Contains(".") Then
                            errore = True
                            erroreStr = "Codice Soggetto non valido"
                        End If
                        cuaa_fisiche = True
                    ElseIf (Not IsDBNull(soggetto.Item("CUAA_PersonaGiuridica"))) AndAlso soggetto.Item("CUAA_PersonaGiuridica") <> "" Then
                        cuaa = soggetto.Item("CUAA_PersonaGiuridica")
                        If cuaa = "" Or cuaa.Contains("/") Or cuaa.Contains("\") Or cuaa.Contains("-") Or cuaa.Contains(".") Then
                            errore = True
                            erroreStr = "Codice Soggetto non valido"
                        End If
                        cuaa_fisiche = False
                    Else
                        cuaa = ""
                    End If
                    If cuaa_fisiche Then
                        nome = soggetto.Item("Nome")
                        cognome = soggetto.Item("Cognome")
                        If nome = "" And cognome = "" Then
                            rag_soc = soggetto.Item("Rag_Soc")
                            Dim rr = rag_soc.Split(" ")
                            If rr.Length >= 2 Then
                                nome = rr(rr.Length - 1)
                                Dim i = 0
                                While (i <= (rr.Length - 2))
                                    cognome = rr(i) + " "
                                    i += 1
                                End While
                                nome = nome.Trim
                                cognome = cognome.Trim
                                rag_soc = ""
                            End If
                        End If
                    Else
                        rag_soc = soggetto.Item("Rag_Soc")
                        If rag_soc = "" Then
                            rag_soc = soggetto.Item("Cognome") & " " & soggetto.Item("Nome")
                        End If
                    End If
                    If nome = "" And cognome = "" And rag_soc = "" Then
                        errore = True
                        erroreStr = "Nome Cognome e Ragione Sociale assenti"
                    End If
                    If nome.Length > 50 Then
                        errore = True
                        erroreStr = "Nome troppo lungo, lunghezza massima 50"
                    End If
                    If cognome.Length > 50 Then
                        errore = True
                        erroreStr = "Cognome troppo lungo, lunghezza massima 50"
                    End If
                    If rag_soc.Length > 150 Then
                        errore = True
                        erroreStr = "Ragione Sociale troppo lunga, lunghezza massima 150"
                    End If

                    Dim tiposoggetto As String = soggetto.Item("TipoSoggetto")

                    Dim cap As String = ""
                    Dim indirizzo As String = ""
                    If Not IsDBNull(soggetto.Item("IndirizzoSede_Indirizzo")) Then
                        indirizzo = soggetto.Item("IndirizzoSede_Indirizzo")
                    End If
                    If indirizzo = "" Then
                        errore = True
                        erroreStr = "Indirizzo incompleto"
                    End If
                    Dim comune As String = ""
                    Dim provincia As String = ""
                    Dim stato As String = soggetto.Item("TipoSoggetto")
                    If stato = "IT" Then
                        If Not (IsDBNull(soggetto.Item("IndirizzoSede_CAP"))) Then
                            cap = soggetto.Item("IndirizzoSede_CAP")
                        End If
                        If Not (IsDBNull(soggetto.Item("IstatCom"))) Then
                            comune = soggetto.Item("IstatCom")
                        End If
                        If Not (IsDBNull(soggetto.Item("IstatProv"))) Then
                            provincia = soggetto.Item("IstatProv")
                        End If
                        stato = "380"
                        If cap = "" Or comune = "" Or provincia = "" Then
                            errore = True
                            erroreStr = "Indirizzo incompleto"
                        End If
                        If cap.Length <> 5 Then
                            errore = True
                            erroreStr = "CAP Errato"
                        End If
                    Else
                        cuaa = ""
                        cap = ""
                        comune = ""
                        provincia = ""
                        If Not IsDBNull(soggetto.Item("Codice_numerico_Stato")) Then
                            stato = soggetto.Item("Codice_numerico_Stato")
                            If stato = "" Then
                                errore = True
                                erroreStr = "Stato non codificato correttamente"
                            End If
                        End If
                    End If
                    If tiposoggetto = "" Then
                        errore = True
                        erroreStr = "Tipo Soggetto Errato"
                    End If
                    Dim TipoRichiesta As String = nuovaRichiesta
                    Dim Gias_Stato As Integer = 18000001
                    If errore Then
                        writer.InserisciAggiornaWs_RegVino_Soggetti(objParametri_Server,
                                                                          codiceSoggetto,
                                                                          codOper,
                                                                          codOper_FisicheGiuridiche,
                                                                          TipoRichiesta,
                                                                          cuaa,
                                                                          cuaa_fisiche,
                                                                          tiposoggetto,
                                                                          nome,
                                                                          cognome,
                                                                          rag_soc,
                                                                          cap,
                                                                          indirizzo,
                                                                          comune,
                                                                          provincia,
                                                                          stato,
                                                                          18000003)
                        msg = id + ": Errore - " + erroreStr
                    Else
                        writer.InserisciAggiornaWs_RegVino_Soggetti(objParametri_Server,
                                                                          codiceSoggetto,
                                                                          codOper,
                                                                          codOper_FisicheGiuridiche,
                                                                          TipoRichiesta,
                                                                          cuaa,
                                                                          cuaa_fisiche,
                                                                          tiposoggetto,
                                                                          nome,
                                                                          cognome,
                                                                          rag_soc,
                                                                          cap,
                                                                          indirizzo,
                                                                          comune,
                                                                          provincia,
                                                                          stato,
                                                                          Gias_Stato)
                    End If
                End If
            Else
                msg = id + ": " + checkCuaa(codOper1)
            End If
        End If

        Return msg
    End Function

    Function CambiaRichiestaSoggetto(id As String, pivasu As String, codOper1 As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri, richiesta As String) As String
        Dim reader As New xDBSoggSiRPV_R
        Dim writer As New xDBSoggSiRPV_W
        Dim msg As String = ""
        Dim erroreStr As String = ""
        msg += id + ": "
        ' 1 Guardo se è presente una riga nelle tabelle di transcodifica
        Dim canInsertRefresh = False
        Dim nuovaRichiesta As String = ""
        msg += richiesta
        writer.InserisciAggiornaRichiestaWs_RegVino_Soggetti(objParametri_Server, id, codOper1, richiesta, 18000000)
        Return msg
    End Function

    Function ESoggetto(id As String, pivasu As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim reader As New xDBSoggSiRPV_R
        Dim writer As New xDBSoggSiRPV_W
        Dim msg As String = ""
        msg += id + ": "
        ' 1 Guardo se è presente una riga nelle tabelle di transcodifica
        Dim dti = reader.Leggiws_RegVino_Soggetti(objParametri_Server, id, pivasu)
        Dim canDelete = False
        Dim nuovaRichiesta As String = ""
        possoEliminare(dti, canDelete, nuovaRichiesta, msg)
        If canDelete Then
            writer.EliminaWs_RegVino_Soggetti(objParametri_Server, pivasu, id)
        End If
        Return msg
    End Function

    Function IAOperazione(id As String, codOper As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim reader As New xDBOperazioniSiRPV_R
        Dim writer As New xDBOperazioniSiRPV_W
        Dim dt As DataTable
        Dim des_lib As String = ""
        Dim codiceOperazione As String = ""
        Dim dataOperazione As String = ""
        Dim numeroOperazione As String = ""
        dt = objCoreStampeDAL.LeggiOperazioni(
                                    "",
                                    "",
                                    "",
                                    AGRODATAINIZIO,
                                    AGRODATAFINE,
                                    "", "",
                                    objParametri_Server, "", id)
        des_lib = CStr(dt.Rows(0).Item("des_lib"))
        If des_lib.Length > 36 Then
            des_lib = des_lib.Substring(0, 36) + "..."
        End If
        codiceOperazione = CStr(dt.Rows(0).Item("CodiceOperazione"))
        dataOperazione = CDate(dt.Rows(0).Item("DataOperazione")).ToString("dd/MM/yyyy")
        numeroOperazione = CStr(dt.Rows(0).Item("NumOperazione"))
        Dim msg As String = dataOperazione + "-" + numeroOperazione + "-" + codiceOperazione + "-" + des_lib + ": "
        Dim dti As DataTable = reader.LeggiWs_RegVino_Operazioni(objParametri_Server, id)
        Dim soggetto As String = ""
        Dim piva As String = ""
        Dim canInsertUpdate = False
        Dim nuovaRichiesta As String = ""
        possoInserireAggiornare(dti, canInsertUpdate, nuovaRichiesta, msg)
        If objParametri_Server.Tipologia = agronicacoreparametri_tipologia.Vinificazione Then
            If canInsertUpdate Then
                writer.InserisciAggiornaWs_RegVino_Operazioni(objParametri_Server, id, nuovaRichiesta, 18000001)
            End If
        Else
            If canInsertUpdate Then
                Dim insertCom As Boolean = True
                Dim insertDest As Boolean = True
                Dim insertForn As Boolean = True
                Dim committente = dti.Rows(0).Item("CodCommittente")
                If committente <> "" Then
                    insertCom = controllaSoggetto(committente, objParametri_Server, soggetto, piva)
                End If
                Dim fornitore = dti.Rows(0).Item("CodFornitore")
                If fornitore <> "" Then
                    insertForn = controllaSoggetto(fornitore, objParametri_Server, soggetto, piva)
                End If
                Dim destinatario = dti.Rows(0).Item("CodDestinatario")
                If destinatario <> "" Then
                    insertDest = controllaSoggetto(destinatario, objParametri_Server, soggetto, piva)
                End If
                If insertCom And insertDest And insertForn Then
                    writer.InserisciAggiornaWs_RegVino_Operazioni(objParametri_Server, id, nuovaRichiesta, 18000001)
                Else
                    msg = id + ": Soggetto " + soggetto + " (P.Iva " + piva + " )" + " coinvolto nell'operazione non ancora telematizzato"
                End If
            End If
        End If

        Return msg

    End Function

    Function CambiaRichiestaOperazione(id As String, codOper As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri, richiesta As String, idNew As String) As String
        Dim reader As New xDBOperazioniSiRPV_R
        Dim writer As New xDBOperazioniSiRPV_W
        Dim msg As String = id + ": "
        If idNew = "" Then
            writer.InserisciAggiornaWs_RegVino_Operazioni(objParametri_Server, id, richiesta, 18000000)
        Else
            Try
                Dim idNewInt = CInt(idNew)
                writer.InserisciAggiornaWs_RegVino_Operazioni(objParametri_Server, id, richiesta, 18000000)
                writer.impostaProgressivo(id, idNewInt, objParametri_Server)
            Catch ex As Exception
                msg += "Id da impostare non valido"
            End Try
        End If
        Return msg
    End Function

    Function EOperazione(id As String, pivasu As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim reader As New xDBOperazioniSiRPV_R
        Dim writer As New xDBOperazioniSiRPV_W
        Dim msg As String = id + ": "
        Dim dti = reader.LeggiWs_RegVino_Operazioni(objParametri_Server, id)
        Dim canDelete = False
        Dim nuovaRichiesta As String = ""
        possoEliminare(dti, canDelete, nuovaRichiesta, msg)
        If canDelete Then
            writer.EliminaWs_RegVino_Operazioni(objParametri_Server, id)
        End If
        Return msg
    End Function

    Function IAProdotto(id As String, codOper As String, codIcqrf As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim msg As String = id + ": "
        Dim dti = objCoreStampeDAL.LeggiWs_RegVino_Prodotti(objParametri_Server, id, codOper, codIcqrf)
        Dim canInsertUpdate = False
        Dim nuovaRichiesta As String = ""
        possoInserireAggiornare(dti, canInsertUpdate, nuovaRichiesta, msg)
        If canInsertUpdate Then
            objCoreStampeDAL.InserisciAggiornaWs_RegVino_Prodotti(objParametri_Server, id, codOper, codIcqrf, nuovaRichiesta, 18000001)
        End If
        Return msg
    End Function

    Function EProdotto(id As String, codOper As String, codIcqrf As String, objCoreStampeDAL As AgronicaCoreStampeDAL.Cantina_RegistriTelematici, objParametri_Server As AgronicaCoreParametri) As String
        Dim msg As String = id + ": "
        Dim dti = objCoreStampeDAL.LeggiWs_RegVino_Prodotti(objParametri_Server, id, codOper, codIcqrf)
        Dim canDelete = False
        Dim nuovaRichiesta As String = ""
        possoEliminare(dti, canDelete, nuovaRichiesta, msg)
        If canDelete Then
            objCoreStampeDAL.EliminaWs_RegVino_Prodotti(objParametri_Server, codOper, codIcqrf, id)
        End If
        Return msg
    End Function

    Private Sub possoInserireAggiornare(ByRef dti As DataTable, ByRef canInsertRefresh As Boolean, ByRef nuovaRichiesta As String, ByRef msg As String)
        If dti.Rows.Count > 0 Then
            Dim richiesta As String = CStr(dti.Rows(0).Item("TipoRichiesta"))
            Dim stato As Integer = CInt(dti.Rows(0).Item("Gias_Stato"))
            Select Case richiesta
                Case "I"
                    'In questi stati non si può modificare
                    If stato = 18000001 Or _
                       stato = 18000002 Or _
                       stato = 18000004 Or _
                       stato = 18000005 Or _
                       stato = 18000006 Or _
                       stato = 18000010 Then
                        msg += "Non modificabile Attualmente"
                        canInsertRefresh = False
                    End If
                    'Stati di Errore, Inserisco
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Inserito"
                        canInsertRefresh = True
                        nuovaRichiesta = "I"
                    End If

                    'Valida, Aggiorno
                    If stato = 18000009 Then
                        msg += "Aggiornato"
                        canInsertRefresh = True
                        nuovaRichiesta = "A"
                    End If

                    If Not (stato = 18000001 Or
                        stato = 18000002 Or
                        stato = 18000003 Or
                        stato = 18000004 Or
                        stato = 18000005 Or
                        stato = 18000006 Or
                        stato = 18000007 Or
                        stato = 18000008 Or
                        stato = 18000009 Or
                        stato = 180000010) Then
                        msg += "Inserito"
                        canInsertRefresh = True
                        nuovaRichiesta = "I"
                    End If

                Case "A"
                    'In questi stati non si può modificare
                    If stato = 18000001 Or _
                       stato = 18000002 Or _
                       stato = 18000004 Or _
                       stato = 18000005 Or _
                       stato = 18000006 Or _
                       stato = 18000010 Then
                        msg += "Non modificabile Attualmente"
                        canInsertRefresh = False
                    End If
                    'Stati di Errore, Aggiorno
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Aggiorno"
                        canInsertRefresh = True
                        nuovaRichiesta = "A"
                    End If

                    'Valida, Aggiorno
                    If stato = 18000009 Then
                        msg += "Aggiorno"
                        canInsertRefresh = True
                        nuovaRichiesta = "A"
                    End If

                    If stato = 18000000 Then
                        msg += "Aggiorno"
                        canInsertRefresh = True
                        nuovaRichiesta = "A"
                    End If

                Case "E"
                    'In questi stati non si può modificare
                    If stato = 18000001 Or _
                       stato = 18000002 Or _
                       stato = 18000004 Or _
                       stato = 18000005 Or _
                       stato = 18000006 Or _
                       stato = 18000010 Then
                        msg += "Non modificabile Attualmente"
                        canInsertRefresh = False
                    End If
                    'Stati di Errore, Aggiorno
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Aggiorno"
                        canInsertRefresh = True
                        nuovaRichiesta = "I"
                    End If

                    'Valida, Inserisco
                    If stato = 18000011 Or stato = 18000000 Then
                        msg += "Inserito"
                        canInsertRefresh = True
                        nuovaRichiesta = "I"
                    End If
            End Select
        Else
            msg += "Inserito"
            canInsertRefresh = True
            nuovaRichiesta = "I"
        End If
    End Sub

    Private Sub possoEliminare(ByRef dti As DataTable, ByRef canDelete As Boolean, ByRef nuovaRichiesta As String, ByRef msg As String)
        If dti.Rows.Count > 0 Then
            Dim richiesta As String = CStr(dti.Rows(0).Item("TipoRichiesta"))
            Dim stato As Integer = CInt(dti.Rows(0).Item("Gias_Stato"))
            Select Case richiesta
                Case "I"
                    'In questi stati non si può modificare
                    If stato = 18000001 Or _
                       stato = 18000002 Or _
                       stato = 18000004 Or _
                       stato = 18000005 Or _
                       stato = 18000006 Or _
                       stato = 18000010 Then
                        msg += "Non modificabile Attualmente"
                        canDelete = False
                    End If
                    'Stati di Errore, Inserisco
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Elimino"
                        canDelete = True
                    End If

                    'Valida, Aggiorno
                    If stato = 18000009 Then
                        msg += "Elimino"
                        canDelete = True
                        nuovaRichiesta = "E"
                    End If
                Case "A"
                    'In questi stati non si può modificare
                    If stato = 18000001 Or _
                       stato = 18000002 Or _
                       stato = 18000004 Or _
                       stato = 18000005 Or _
                       stato = 18000006 Or _
                       stato = 18000010 Then
                        msg += "Non modificabile Attualmente"
                        canDelete = False
                    End If
                    'Stati di Errore, Aggiorno
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Elimino"
                        canDelete = True
                        nuovaRichiesta = "E"
                    End If

                    'Valida, Aggiorno
                    If stato = 18000009 Then
                        msg += "Elimino"
                        canDelete = True
                        nuovaRichiesta = "E"
                    End If
                Case "E"
                    'In questi stati non si può modificare
                    If stato = 18000001 Or _
                       stato = 18000002 Or _
                       stato = 18000004 Or _
                       stato = 18000005 Or _
                       stato = 18000006 Or _
                       stato = 18000010 Then
                        msg += "Non modificabile Attualmente"
                        canDelete = False
                    End If
                    'Stati di Errore, Aggiorno
                    If stato = 18000003 Or _
                       stato = 18000007 Or _
                       stato = 18000008 Then
                        msg += "Elimino"
                        canDelete = True
                        nuovaRichiesta = "E"
                    End If

                    'Valida, Inserisco
                    If stato = 18000011 Or stato = 18000009 Then
                        msg += "Già eliminata precedentemente"
                        canDelete = False
                        nuovaRichiesta = "E"
                    End If

                    If stato = 18000000 Then
                        msg += "Elimino"
                        canDelete = True
                        nuovaRichiesta = "E"
                    End If

            End Select
        Else
            msg += "Non presente nel SIAN"
            canDelete = False
        End If
    End Sub

    Private Function controllaSoggetto(committente As Object, objParametri As AgronicaCoreParametri, ByRef soggetto As String, ByRef piva As String) As Object
        Dim reader As New xDBSoggSiRPV_R
        Dim dti As DataTable = reader.Leggiws_RegVino_Soggetti(objParametri, committente)
        If dti.Rows.Count > 0 Then
            Dim stato As Integer = dti.Rows(0).Item("GIAS_Stato")
            If stato = 18000009 Then
                Return True
            Else
                Dim soggR As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim dtSogg = soggR.LeggiContattoSpecifico("", committente, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
                If dtSogg IsNot Nothing AndAlso dtSogg.Rows.Count > 0 Then
                    Dim nome As String = dtSogg.Rows(0).Item("Nome")
                    Dim cognome As String = dtSogg.Rows(0).Item("Cognome")
                    Dim rag_Soc As String = dtSogg.Rows(0).Item("rag_soc")
                    piva = committente
                    If nome IsNot Nothing AndAlso cognome IsNot Nothing Then
                        If nome <> "" Or cognome <> "" Then
                            soggetto = nome + " " + cognome
                        Else
                            soggetto = rag_Soc
                        End If
                    Else
                        soggetto = rag_Soc
                    End If
                End If
                Return False
            End If
        Else
            Dim soggR As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtSogg = soggR.LeggiContattoSpecifico("", committente, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)
            If dtSogg IsNot Nothing AndAlso dtSogg.Rows.Count > 0 Then
                Dim nome As String = dtSogg.Rows(0).Item("Nome")
                Dim cognome As String = dtSogg.Rows(0).Item("Cognome")
                Dim rag_Soc As String = dtSogg.Rows(0).Item("rag_soc")
                piva = committente
                If nome IsNot Nothing AndAlso cognome IsNot Nothing Then
                    If nome <> "" Or cognome <> "" Then
                        soggetto = nome + " " + cognome
                    Else
                        soggetto = rag_Soc
                    End If
                Else
                    soggetto = rag_Soc
                End If
            End If
            Return False
        End If
        Return False
    End Function

    Private Function checkCuaa(cuaa As String) As String
        If cuaa.Contains(" ") Or cuaa.Contains("\") Or cuaa.Contains("/") Or cuaa.Contains("-") Then
            Return "formato cuaa non corretto, eliminare spazi, \, /, -"
        End If
        If cuaa.Length = 11 Or cuaa.Length = 16 Then
            Return ""
        Else
            Return "lunghezza cuaa non corretta"
        End If
    End Function

    Private Function checkICQRF(icqrf As String) As String
        If icqrf.Contains(" ") Or icqrf.Contains("\") Or icqrf.Contains("/") Or icqrf.Contains("-") Then
            Return "formato icqrf non corretto, eliminare spazi, \, /, -"
        End If
        Try
            Dim int = CInt(icqrf.Substring(2))
        Catch ex As Exception
            Return "il codice icqrf non rispetta lo standard [a-zA-Z]{2}[0-9]{1,6}"
        End Try
        Return ""
    End Function



End Class
