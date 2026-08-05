Imports System.ComponentModel
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Xml
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreAnagrafeBIZ.AnagrafeNG
Imports AgronicaCoreDTOStd.InData.Budget

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class CentroAziendale1
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Centri_Anagrafica(ByVal objP_super_server As String,
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
            Dim objCentri As New AgronicaCoreBudgetDAL.Centri_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If InData.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = InData.Data
                objParametri_Server.FinestraTemporaleFine = InData.Data
            End If

            Dim dtAnagrafica As DataTable = objCentri.Leggi_x_anagraficaNG(InData.Piva, 0, "", "", objParametri_Server) 'AgronicaCoreBudgetDAL.Leggi_x_anagraficaNG

            If InData.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = InData.Data
                objParametri_Server.FinestraTemporaleFine = InData.Data
            End If

            dtAnagrafica.Columns.AddRange({New DataColumn("Superficie_Totale", Type.GetType("System.Decimal")),
                                       New DataColumn("Superficie_Tare", Type.GetType("System.Decimal")),
                                       New DataColumn("SAU_Totale", Type.GetType("System.Decimal")),
                                       New DataColumn("tipoAttivitaDes", Type.GetType("System.String"))
                                      })


            For i As Integer = 0 To dtAnagrafica.Rows.Count - 1

                'Recupero le Superfici
                Dim Sup_Totale, Sup_Bosco, Sup_Prati, Sup_Tare, SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale As Double
                objCentri.Recupera_Superfici_CentroAziendale(dtAnagrafica.Rows(i).Item("Piva"), dtAnagrafica.Rows(i).Item("sa_cod"),
                                                    Sup_Totale, Sup_Bosco, Sup_Prati, Sup_Tare,
                                                    SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale,
                                                    Date.Today, objParametri_Server)

                dtAnagrafica.Rows(i).Item("Superficie_Totale") = Format(Sup_Totale, "0.0000")
                dtAnagrafica.Rows(i).Item("Superficie_Tare") = Format(Sup_Tare, "0.0000")
                dtAnagrafica.Rows(i).Item("SAU_Totale") = Format(SAU_Totale, "0.0000")

            Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Id_Budget", "chiave", "number") With {._hidden = True})
            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("piva", "Partita IVA", "string") With {._Display = False})
            l.Add(New ColonneNome("rag_soc", "Ragione Sociale", "string") With {._Display = False})
            l.Add(New ColonneNome("sa_nome", "Nome", "string"))
            l.Add(New ColonneNome("ind_des", "Indirizzo", "string"))
            l.Add(New ColonneNome("com_des", "Comune", "string"))
            l.Add(New ColonneNome("pro_cod", "Provincia", "string"))
            l.Add(New ColonneNome("CAP", "CAP", "string"))
            l.Add(New ColonneNome("Stato", "Stato", "string"))
            Dim c = New ColonneNome("Superficie_Totale", "Superficie Totale [Ha]", "number")
            l.Add(c)

            c = New ColonneNome("Superficie_Tare", "Superficie Tare [Ha]", "number")
            l.Add(c)

            c = New ColonneNome("SAU_Totale", "Sau Totale [Ha]", "number")
            l.Add(c)

            c = New ColonneNome("Validita_Inizio", "Inizio Validità", "date")
            c = New ColonneNome("Validita_Fine", "Fine Validità", "date")
            l.Add(c)

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))
            l.Add(New ColonneNome("Data_Modifica", "Data Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            'Dim risp As String = js.JSON_DataTable_Kendo(dtAnagrafica, l, False, False, TipoFiltroKendo_colonne.CasellaTesto,)
            Dim risp As String = JsonConvert.SerializeObject(dtAnagrafica)
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
    Public Function Leggi_Centro_Anagrafica(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Piva = InData.InData.Piva
            Dim Sa_Cod = InData.InData.Sa_Cod
            Dim centro As New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale()
            centro = objCentri.Centro_Leggi_Anagrafica(Piva:=Piva,
                                                       Sa_Cod:=Sa_Cod,
                                                       Leggi_Impresa:=True,
                                                       Leggi_Indirizzo:=True,
                                                       Leggi_Codici:=True,
                                                       Leggi_Rubrica:=True,
                                                       Leggi_Catasto:=False,
                                                       objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = centro

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

End Class