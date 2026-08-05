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
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDTOStd
Imports AgronicaCoreDTOStd.InData.Budget
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreModelsSTD.costanti
Imports System.Transactions
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Appezzamento1
    Inherits System.Web.Services.WebService

    <WebMethod()>
    Public Function HelloWorld() As String
        Return "Hello World"
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function Leggi_Appezzamento_Anagrafica(InData As Object
                                                  ) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreDTOStd.InData.Anagrafica.LeggiAppezzamento)) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreDTOStd.InData.Anagrafica.LeggiAppezzamento)))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_R

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim Id_Budget = iData.InData.Id_Budget
            Dim Piva = iData.InData.ElementoAnagrafico.appezzamento.primaryKey.centroAziendalePK.partitaIva
            Dim Sa_Cod = iData.InData.ElementoAnagrafico.appezzamento.primaryKey.centroAziendalePK.codice
            Dim Appezza = iData.InData.ElementoAnagrafico.appezzamento.primaryKey.codice
            Dim IdReg = 0

            If (iData.InData.ElementoAnagrafico.appezzamento.impianti IsNot Nothing) Then
                IdReg = iData.InData.ElementoAnagrafico.appezzamento.impianti(0).primaryKey.codice
            End If

            Dim Data_Filtro = DateTime.Now

            Dim obj_Appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento = objAppezzamento.Leggi_Appezzamento_Anagrafica(
                Id_Budget,
                Piva,
                Sa_Cod,
                Appezza,
                IdReg,
                iData.InData.ElementoAnagrafico.leggiImpianti,
                iData.InData.ElementoAnagrafico.leggiIndirizzi,
                iData.InData.ElementoAnagrafico.leggiCatasto,
                iData.InData.ElementoAnagrafico.data,
                iData.InData.ElementoAnagrafico.filtroData,
                iData.InData.ElementoAnagrafico.leggiDistinte,
                iData.InData.ElementoAnagrafico.leggiCartografia,
                objParametri_Super_Server,
                objParametri_Server,
                objParametri_Utenti
                )

            If obj_Appezzamento Is Nothing Then
                Dim keys() As String = {Id_Budget, Piva, Sa_Cod, Appezza}
                Throw New Exception("Errore in lettura Appezzamento with key " + String.Join("|", keys))
            End If



            'recupero descrizione disciplinari
            For Each imp In obj_Appezzamento.impianti
                If imp.utilizzoTerreno IsNot Nothing Then
                    If imp.utilizzoTerreno.classType = ClassType.Varieta Then
                        For Each ese In imp.esercizi
                            Dim var As AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta = imp.utilizzoTerreno
                            Dim Data = ""
                            Dim disc = ese.disciplinare
                            If disc IsNot Nothing AndAlso (disc.descrizione Is Nothing Or disc.descrizione = "") Then

                                '' VAnni: 6/9/2017: ovvio alla carenza di sessione..
                                Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                                Dim GiasOnline_WS_Disciplinari_AgroWS_Disciplinari As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Disciplinari_AgroWS_Disciplinari", "", "", objParametri_Server)
                                HttpContext.Current.Session("WS_DPI") = GiasOnline_WS_Disciplinari_AgroWS_Disciplinari

                                Dim GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci", "", "", objParametri_Server)
                                HttpContext.Current.Session("WS_FITO") = GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci


                                Session("ASG_SuperUser_CodFiscale") = objParametri_Server.PivaSuperUser
                                Session("ASG_Utente_Username_Crypt") = Sicurezza.Stringa_Codifica_LANCompatibile(objParametri_Server.SuperUserUsername, CostantiPersonalizzate.AgroKey_EncoderDecoder)

                                Dim xletturautente As New AgronicaCoreUtentiDAL.Utenti_Read
                                Dim pass As String = xletturautente.Password_From_UserName(objParametri_Server.SuperUserUsername, objParametri_Utenti)
                                Session("ASG_Utente_Password_Crypt") = Sicurezza.Stringa_Codifica_LANCompatibile(pass, CostantiPersonalizzate.AgroKey_EncoderDecoder)

                                '' VAnni: 6/9/2017: fine carenza sessione...
                                If IsDate(Data) Then
                                    'TODO: Giulia fix data stringa
                                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(Data), AGRODATAFINE)
                                Else
                                    objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(AGRODATAINIZIO, AGRODATAFINE)
                                End If

                                Dim x As New AgronicaCoreDpiBIZ.CaricaListControl
                                Dim ddl As New DropDownList
                                x.Disciplinari_Elenco_TuttigliElemInChiave(ddl, False, "", "", Session,
                                                      objParametri_Server, objParametri_Utenti,
                                                      0,
                                                      var.specie.codice.ToString, 0, 0,
                                                      True, True, False,
                                           New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = disc.disciplinarePubblicoPrivato, .Flag_DisciplinareAttivo = True}, True)

                                objParametri_Server.ResettaFinestra()

                                Dim listItems = (From item As ListItem In ddl.Items Where item.Value = disc.codice Select New AgronicaCoreModelsSTD.metaschema.Disciplinare(item.Value) With {
                                    .descrizione = item.Text
                                }).ToList

                                If listItems IsNot Nothing And listItems.Count > 0 Then
                                    ese.disciplinare.descrizione = listItems(0).descrizione
                                End If
                            End If
                        Next
                    End If
                End If

            Next

            r.RispostaOK = True
            r.RispostaStringa = obj_Appezzamento

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Max_DataModifica(InData As Object) As rispostaStandard(Of AgronicaCoreDTOStd.InData.Data)

        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.Data)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objStr = JsonConvert.SerializeObject(InData, a)

        Dim iData As CoreWS_Generic(Of BudgetAnagrafica(Of String)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of BudgetAnagrafica(Of String)))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim objAppezza_R As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_R
            Dim dataMax = objAppezza_R.LeggiMaxData(iData.InData.Id_Budget, iData.InData.ElementoAnagrafico, objParametri_Server, objParametri_Utenti)

            Dim resp = New AgronicaCoreDTOStd.InData.Data
            resp.data = dataMax

            r.RispostaStringa = resp

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function Scrivi_Appezzamento_Anagrafica(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of BudgetAnagrafica(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)))(JsonConvert.SerializeObject(InData), a)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If iData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim AppezzamentoBIZ As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_W
            Dim listRibaltamento_Appezzamento As New List(Of Ribaltamento_Appezzamento)

            Dim esitoPositivoScritturaAppezzamento As Boolean = AppezzamentoBIZ.Appezzamento_ScriviModifica(iData.InData.Id_Budget, iData.InData.ElementoAnagrafico, objParametri_Server, objParametri_Utenti,
                                                                                                            listRibaltamento_Appezzamento:=listRibaltamento_Appezzamento)

            If esitoPositivoScritturaAppezzamento Then
                If iData.InData.Delete_Reale_From_Ribaltamento AndAlso listRibaltamento_Appezzamento.Count > 0 Then
                    Dim msgCancellazioneReale = Appezzamento_W.Internal_Delete_Appezzamento_Reale_Da_Ribaltamento(listRibaltamento_Appezzamento(0), objParametri_Server, objParametri_Utenti)
                    If msgCancellazioneReale <> "" Then
                        r.ErroriGias.Add(New ErroreGias With {
                                         .messaggio = iData.InData.ElementoAnagrafico.descrizione & ": " & msgCancellazioneReale,
                                         .severity = ErroreGias_Severity.Info})
                    End If
                End If

                Dim objAppezzamento As New AgronicaCoreBudgetBIZ.Budget_Appezzamento_R

                If iData.InData.ElementoAnagrafico.flag_cancellazione = False Then
                    Dim obj_Appezzamento = objAppezzamento.Leggi_Appezzamento_Anagrafica(iData.InData.Id_Budget,
                                                                                         iData.InData.ElementoAnagrafico.primaryKey.centroAziendalePK.partitaIva,
                                                                                         iData.InData.ElementoAnagrafico.primaryKey.centroAziendalePK.codice,
                                                                                         iData.InData.ElementoAnagrafico.primaryKey.codice, 0,
                                                                                         True, True, True,
                                                                                         AGRODATAINIZIO,
                                                                                         False, True, True,
                                                                                         objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

                    'Valorizzazione_Geojson_NodeInfo_Appezzamento(
                    '    obj_Appezzamento,
                    '    iData,
                    '    objParametri_Server,
                    '    objParametri_Utenti)


                    r.RispostaOK = True
                    r.RispostaStringa = obj_Appezzamento
                Else
                    r.RispostaOK = True
                    r.RispostaStringa = iData.InData.ElementoAnagrafico
                End If
            Else
                Throw New Exception(String.Format(Gias.ErroreScritturaAppezzamentoConData, iData.InData.ToString()))
            End If


        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = iData.InData.ElementoAnagrafico
            'uso questa funzione per ottenere il Messaggio..:
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Ribalta_BudgetReale(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Try

            Dim str = JsonConvert.SerializeObject(InData)

            Dim iData As CoreWS_Generic(Of List(Of BudgetAnagrafica(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento))) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of BudgetAnagrafica(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento))))(JsonConvert.SerializeObject(InData))

            If InData.objP.objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            If InData.objP.objP_utenti = "" Then
                r.Errore = "objP_utenti non valorizzato"
                Return r
            End If

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim dicAnagrafica_Ribaltata As New Anagrafica_Ribaltata

            Dim primo_elem = iData.InData.First().ElementoAnagrafico
            Dim ultimo_elem = iData.InData.Last().ElementoAnagrafico

            Dim ribaltati_con_successo As Integer = 0

            Dim list_msg_gestiti As New List(Of String)
            Dim list_msg_non_gestiti As New List(Of String)

            For Each elem In iData.InData
                Dim Id_Budget = elem.Id_Budget
                Dim objApp = elem.ElementoAnagrafico

                Dim isFirst As Boolean = elem.ElementoAnagrafico.Equals(primo_elem)
                Dim isLast As Boolean = elem.ElementoAnagrafico.Equals(ultimo_elem)

                Dim msg_gestito As String = ""

                Using scope As New TransactionScope()
                    Using GiasContext As Gias_DeveloperServer_Entities = Gias_EF_Utility.CreateGiasContextConnection(objParametri_Server.StringaConnessione)
                        Try
                            RibaltamentoAnagraficheColturali_W.Ribalta_BudgetReale(Id_Budget, objApp, dicAnagrafica_Ribaltata, ribaltati_con_successo, msg_gestito, isFirst, isLast, objParametri_Server, objParametri_Utenti, GiasContext)

                            If msg_gestito <> "" Then
                                list_msg_gestiti.Add(msg_gestito)
                            End If

                            GiasContext.SaveChanges()
                            scope.Complete()

                        Catch ex As Exception
                            list_msg_non_gestiti.Add("- " & objApp.descrizione & ": " & ex.Message)
                        Finally
                            scope.Dispose()
                        End Try
                    End Using
                End Using
            Next

            Dim messaggio As String = ""
            If list_msg_gestiti.Count > 0 OrElse list_msg_non_gestiti.Count > 0 Then
                Dim ErroreGias As New ErroreGias
                messaggio += "<b>" & String.Format(Gias.RibaltatiXimpiantiSuY, ribaltati_con_successo, iData.InData.Count) & "</b>" & NEWLINE & NEWLINE

                If list_msg_gestiti.Count > 0 Then
                    messaggio += Gias.ImpossibileRibaltareImpiantiSuPianoColturaleEffettivoMovimentiAssociati & ": " &
                        NEWLINE & String.Join("<br>", list_msg_gestiti)
                End If

                If list_msg_non_gestiti.Count > 0 Then
                    If messaggio <> "" Then
                        messaggio += NEWLINE
                    End If

                    messaggio += Gias.ImpossibileRibaltareImpiantiSuPianoColturaleEffettivo & ": " &
                        NEWLINE & String.Join("<br>", list_msg_non_gestiti)
                End If

                ErroreGias.messaggio = messaggio
                ErroreGias.severity = ErroreGias_Severity.WarningBloccante
                r.ErroriGias.Add(ErroreGias)
            Else
                r.RispostaStringa = Gias.ImpiantiRibaltatiSuccessoPianoColturaleEffettivo
            End If

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function
End Class