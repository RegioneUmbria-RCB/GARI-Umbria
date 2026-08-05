Imports System.Web.Services
Imports AgronicaCoreContabObject
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.attivita.Attivita
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreContabDAL
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreUtentiDAL
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDTOStd.InData.Agenda
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreMapper.CheckAttivita
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModello
Imports AgronicaCoreModello.Utility_Agenda
Imports InData.Agenda
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.costanti
Imports InData.Metaschema
Imports AgronicaCoreModelsSTD.meteo
Imports AgronicaCoreModelsSTD.exceptions
Imports System.Globalization
Imports System.Transactions
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreModelsSTD.attivita.risorse


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Agenda
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviListaAttivitaToRaccoglitore(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try

            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim InDataInizializza As CoreWS_Generic(Of ScriviListaAttivita) =
                JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ScriviListaAttivita))(JsonConvert.SerializeObject(InData, settings), settings)

            Dim lista_Errori As New List(Of ErroreGias)
            Dim mapper As New AgronicaCoreMapper.AttivitaToAgenda

            Dim parametriAggiuntiviList = InDataInizializza.InData.parametri_aggiuntivi_list
            Dim lista_Attivita = InDataInizializza.InData.attivita_list

            objParametri_Server = Utility.convertStringtoOBJparametri(InDataInizializza.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(InDataInizializza.objP.objP_utenti)
            objParametri_Super_Server = Utility.convertStringtoOBJparametri(InDataInizializza.objP.objP_super_server)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            '=====================================
            '   RECUPERO FLAG PER MOSTRARE WARNING 
            '-------------------------------------
            Dim mostraWarning_CheckMagazzino, mostraWarning_ScriviAttivitaToAgenda, mostraWarning_CheckListaAttivita, mostraWarning_ControlloDPI As Boolean
            Dim idTestataVerificaConformita As Integer

            Dim creaCaricoMagazzinoXOriginePUA As Boolean = False

            For Each parametroAggiuntivo In parametriAggiuntiviList
                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.lista_MostraWarning Then
                    Dim dummy As List(Of AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita) = JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita))(parametroAggiuntivo.value)
                    For Each x In dummy
                        Select Case x.key
                            Case Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita
                                mostraWarning_CheckListaAttivita = CBool(x.value)
                            Case Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckMagazzino
                                mostraWarning_CheckMagazzino = CBool(x.value)
                            Case Key_Parametri_Aggiuntivi_Attivita.mostraWarning_ScriviAttivitaToAgenda
                                mostraWarning_ScriviAttivitaToAgenda = CBool(x.value)
                            Case Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI
                                mostraWarning_ControlloDPI = CBool(x.value)
                        End Select
                    Next
                End If

                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.CaricoMagazzinoAutomatico Then
                    creaCaricoMagazzinoXOriginePUA = CBool(parametroAggiuntivo.value)
                End If

                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.IdTestataVerificaConformita Then
                    Dim idTestata As Integer = 0
                    Integer.TryParse(parametroAggiuntivo.value, idTestata)
                    idTestataVerificaConformita = idTestata
                End If

            Next

            Dim isRicettaBrogliaccio As Boolean = Not (lista_Attivita(0).tipo.Equals(Tipo_Attivita.QuadernoDiCampagna))
            '===================================
            '   CHECK LISTA ATTIVITA'  to do...
            '-----------------------------------
            'Controlli basici (acqua, selezionare un impianto/prodotto/operazione...), replicati anche lato client 

            mapper.CheckAttivita_ListaAttivita(lista_Attivita,
                                               parametriAggiuntiviList,
                                               mostraWarning_CheckListaAttivita,
                                               objParametri_Super_Server, objParametri_Server, objParametri_Utenti,
                                               lista_Errori)
            If lista_Errori IsNot Nothing AndAlso lista_Errori.Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
                r.ErroriGias = lista_Errori
                Return r
            End If
            '===================================
            '   CHECK MAGAZZINO to do parte terzisti...
            '-----------------------------------

            mapper.CheckMagazzino_ListaAttivita(lista_Attivita, lista_Errori,
            mostraWarning_CheckMagazzino,
            creaCaricoMagazzinoXOriginePUA,
            objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            If lista_Errori IsNot Nothing AndAlso lista_Errori.Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
                r.ErroriGias = lista_Errori
                Return r
            End If

            If lista_Attivita(0).inviaRicetta Then
                lista_Errori = mapper.checkMagazziniRicettexApp_daAttivita(lista_Attivita, objParametri_Utenti, objParametri_Server, objParametri_Super_Server)

                If lista_Errori IsNot Nothing AndAlso lista_Errori.Count > 0 Then
                    r.RispostaOK = False
                    r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
                    r.ErroriGias = lista_Errori
                    Return r
                End If
            End If

            '============================================================
            '   RECUPERO GESTIONE MULTICENTRO
            '------------------------------------------------------------
            Dim codiciAttivita_x_CentriAziendali_List As New List(Of CodiciAttivita_x_CentriAziendali)
            For Each parametroAggiuntivo In parametriAggiuntiviList
                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.lista_Codici_Attivita_x_CentriAziendali Then
                    Dim codiciAttivita_x_CentriAziendali_array = JsonConvert.DeserializeObject(Of List(Of CodiciAttivita_x_CentriAziendali))(parametroAggiuntivo.value)
                    For Each item In codiciAttivita_x_CentriAziendali_array
                        codiciAttivita_x_CentriAziendali_List.Add(item)
                    Next

                End If
            Next

            Dim agendeToDelete As New List(Of Integer)
            Dim splittedByImpianto As Boolean = mapper.SplitByImpianto(lista_Attivita, objParametri_Server, objParametri_Utenti)
            If Not splittedByImpianto Then
                agendeToDelete = mapper.GestisciSplitAttivita(lista_Attivita, codiciAttivita_x_CentriAziendali_List, objParametri_Server, objParametri_Utenti)
            End If

            Dim listaAttivitaAgende = mapper.MappaListaAttivitaToListaAgenda(lista_Attivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            If Not (isRicettaBrogliaccio) Then
                '===================================
                '   CHECK DPI
                '-----------------------------------

                Dim bearerToken = GetBearerToken(objParametri_Super_Server)

                Dim complianceResultStatus As ComplianceResultStatus
                mapper.CheckDPI_listaAttivita(lista_Attivita, listaAttivitaAgende,
                                              lista_Errori, mostraWarning_ControlloDPI,
                                              objParametri_Super_Server, objParametri_Server, objParametri_Utenti, bearerToken, complianceResultStatus, idTestataVerificaConformita)


                If lista_Errori IsNot Nothing AndAlso lista_Errori.Count > 0 Then
                    r.RispostaOK = False
                    r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
                    If (complianceResultStatus IsNot Nothing) Then
                        r.ParametroDue_stringa = JsonConvert.SerializeObject(complianceResultStatus)
                    End If
                    r.ErroriGias = lista_Errori
                    Return r
                End If

            End If

            '===================================
            '   SCRITTURA 
            '-----------------------------------

            'scrittura di tutte le attività in modo transazionale con gestione del raccoglitore
            Dim listaAttivita_toReturn = mapper.ScriviListaAttivitaToRaccoglitore(listaAttivitaAgende,
                                                                                  InDataInizializza.InData.parametri_aggiuntivi_list,
                                                                                  InDataInizializza.InData.impianti_selezionati_list,
                                                                                  lista_Errori, agendeToDelete, mostraWarning_ScriviAttivitaToAgenda,
                                                                                  creaCaricoMagazzinoXOriginePUA,
                                                                                  objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            If lista_Errori IsNot Nothing AndAlso lista_Errori.Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
                r.ErroriGias = lista_Errori
                'r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(listaErrori(0).ex, True, source:=True)
                Return r
            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(listaAttivita_toReturn)

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function VerificaConformitaAttivita(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim r As New RispostaStandard

        Try
            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim inDataInit As CoreWS_Generic(Of ScriviListaAttivita) =
                JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ScriviListaAttivita))(JsonConvert.SerializeObject(InData, settings), settings)


            Dim lista_Errori As New List(Of ErroreGias)
            Dim mapper As New AgronicaCoreMapper.AttivitaToAgenda

            Dim parametriAggiuntiviList = inDataInit.InData.parametri_aggiuntivi_list
            Dim lista_Attivita = inDataInit.InData.attivita_list

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(inDataInit.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(inDataInit.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(inDataInit.objP.objP_super_server)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            '=====================================
            '   RECUPERO FLAG PER MOSTRARE WARNING 
            '-------------------------------------
            Dim mostraWarning_CheckListaAttivita, mostraWarning_ControlloDPI As Boolean
            Dim idTestataVerificaConformita As Integer

            Dim creaCaricoMagazzinoXOriginePUA As Boolean = False

            For Each parametroAggiuntivo In parametriAggiuntiviList
                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.lista_MostraWarning Then
                    Dim dummy As List(Of AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita) = JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita))(parametroAggiuntivo.value)
                    For Each x In dummy
                        Select Case x.key
                            Case Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckListaAttivita
                                mostraWarning_CheckListaAttivita = CBool(x.value)
                            Case Key_Parametri_Aggiuntivi_Attivita.mostraWarning_CheckDPI
                                mostraWarning_ControlloDPI = CBool(x.value)
                        End Select
                    Next
                End If

                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.CaricoMagazzinoAutomatico Then
                    creaCaricoMagazzinoXOriginePUA = CBool(parametroAggiuntivo.value)
                End If

                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.IdTestataVerificaConformita Then
                    Dim idTestata As Integer = 0
                    Integer.TryParse(parametroAggiuntivo.value, idTestata)
                    idTestataVerificaConformita = idTestata
                End If

            Next

            Dim isRicettaBrogliaccio As Boolean = Not (lista_Attivita(0).tipo.Equals(Tipo_Attivita.QuadernoDiCampagna))
            '===================================
            '   CHECK LISTA ATTIVITA'  to do...
            '-----------------------------------
            'Controlli basici (acqua, selezionare un impianto/prodotto/operazione...), replicati anche lato client 

            mapper.CheckAttivita_ListaAttivita(lista_Attivita,
                                               parametriAggiuntiviList,
                                               mostraWarning_CheckListaAttivita,
                                               objParametri_Super_Server, objParametri_Server, objParametri_Utenti,
                                               lista_Errori)
            'If lista_Errori IsNot Nothing AndAlso lista_Errori.Count > 0 Then
            '    r.RispostaOK = False
            '    r.RispostaStringa = JsonConvert.SerializeObject(inDataInit.InData)
            '    r.ErroriGias = lista_Errori
            '    Return r
            'End If

            '============================================================
            '   RECUPERO GESTIONE MULTICENTRO
            '------------------------------------------------------------
            Dim codiciAttivita_x_CentriAziendali_List As New List(Of CodiciAttivita_x_CentriAziendali)
            For Each parametroAggiuntivo In parametriAggiuntiviList
                If parametroAggiuntivo.key = Key_Parametri_Aggiuntivi_Attivita.lista_Codici_Attivita_x_CentriAziendali Then
                    Dim codiciAttivita_x_CentriAziendali_array = JsonConvert.DeserializeObject(Of List(Of CodiciAttivita_x_CentriAziendali))(parametroAggiuntivo.value)
                    For Each item In codiciAttivita_x_CentriAziendali_array
                        codiciAttivita_x_CentriAziendali_List.Add(item)
                    Next

                End If
            Next

            Dim agendeToDelete As New List(Of Integer)
            Dim splittedByImpianto As Boolean = mapper.SplitByImpianto(lista_Attivita, objParametri_Server, objParametri_Utenti)
            If Not splittedByImpianto Then
                agendeToDelete = mapper.GestisciSplitAttivita(lista_Attivita, codiciAttivita_x_CentriAziendali_List, objParametri_Server, objParametri_Utenti)
            End If

            Dim listaAttivitaAgende = mapper.MappaListaAttivitaToListaAgenda(lista_Attivita, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            If Not (isRicettaBrogliaccio) Then
                '===================================
                '   CHECK DPI
                '-----------------------------------

                Dim bearerToken = GetBearerToken(objParametri_Super_Server)

                Dim complianceResultStatus As ComplianceResultStatus
                mapper.CheckDPI_listaAttivita(lista_Attivita, listaAttivitaAgende,
                                              lista_Errori, mostraWarning_ControlloDPI,
                                              objParametri_Super_Server, objParametri_Server, objParametri_Utenti, bearerToken, complianceResultStatus, idTestataVerificaConformita)

                If lista_Errori IsNot Nothing AndAlso lista_Errori.Count > 0 Then
                    r.RispostaOK = True
                    r.RispostaStringa = JsonConvert.SerializeObject(inDataInit.InData)
                    If (complianceResultStatus IsNot Nothing) Then
                        r.ParametroDue_stringa = JsonConvert.SerializeObject(complianceResultStatus)
                    End If
                    r.ErroriGias = lista_Errori
                    Return r
                End If
            End If

            r.RispostaOK = True
            r.RispostaStringa = "No errors found."

        Catch ex As Exception
            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function

    Private Function GetBearerToken(objParametri_Super_Server As AgronicaCoreParametri) As String

        Dim bearerToken As String = ""
        Dim authCookieArray As String() = HttpContext.Current.Request.Headers.GetValues("auth_cookie")
        Dim authCookieString As String = ""
        If Not IsNothing(authCookieArray) AndAlso authCookieArray.Any() Then
            authCookieString = authCookieArray(0)
        End If

        If String.IsNullOrWhiteSpace(authCookieString) Then
            Dim authorization As String() = HttpContext.Current.Request.Headers.GetValues("Authorization")

            If Not IsNothing(authorization) AndAlso authorization.Any() > 0 Then
                bearerToken = authorization.Last().Substring("Bearer ".Length).Trim()
            End If
        Else
            Dim utentiTokenRead As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_R
            Dim DT As DataTable = utentiTokenRead.LeggiConAuthCookie(authCookieString, objParametri_Super_Server)
            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then
                bearerToken = DT.Rows(0)("BearerToken")
            End If
        End If

        Return bearerToken

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiListaAttivitaDaRicettaOperazionePerAgenda(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Object)
        Return LeggiListaAttivitaDaRicettaOperazione(InData, Tipo_Attivita.QuadernoDiCampagna, 0)
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiListaAttivitaDaRicettaOperazionePerRicetta(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Object)
        Return LeggiListaAttivitaDaRicettaOperazione(InData, Tipo_Attivita.Ricetta, Stati.Da_Eseguire)
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiListaAttivitaDaRicettaOperazionePerBrogliaccio(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Object)
        Return LeggiListaAttivitaDaRicettaOperazione(InData, Tipo_Attivita.Ricetta, Stati.Eseguita)
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiListaAttivitaDaAgenda(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Object)
        Dim r As New rispostaStandard(Of Object)

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim currentAttivitaDes = ""
        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'Istanzio gli oggetti InData.
            Dim strAgenda As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataAgenda As Operazione_Agenda = JsonConvert.DeserializeObject(Of Operazione_Agenda)(strAgenda,
                                                                                          New JsonSerializerSettings With {
                                                                                              .DateTimeZoneHandling = DateTimeZoneHandling.Local
                                                                                              }
                                                                                          )

            'Carico tutte le agende con lo stesso raccoglitore_cod dell'agenda passata in input
            Dim objAgenda As New Agenda_Operazione_Helper
            Dim agenda As Operazione_Agenda = objAgenda.Leggi(inDataAgenda.Piva, 0, inDataAgenda.Id_Agenda, 0, objParametri_Server)

            Dim listaAttivita = New List(Of AgronicaCoreModelsSTD.attivita.AttivitaConParametriAggiuntivi)
            Dim map As New AgronicaCoreMapper.AgendaToAttivita

            If agenda.Raccoglitore_Cod = 0 Then
                Dim listParametriAggiuntivi As New List(Of Parametri_Aggiuntivi_Attivita)

                currentAttivitaDes = agenda.Des_Lib
                Dim attivita = map.AgendaSuAttivita(agenda, verbose:=True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, listParametriAggiuntivi)

                Dim attivitaConParametriAggiuntivi = AgronicaCoreModelsSTD.attivita.AttivitaConParametriAggiuntivi.Create(attivita, listParametriAggiuntivi)

                listaAttivita.Add(attivitaConParametriAggiuntivi)
            Else
                Dim listaAgenda = objAgenda.LeggiLista_DaRaccoglitore(agenda.Piva, agenda.Raccoglitore_Cod, objParametri_Server)

                If listaAgenda IsNot Nothing Then
                    For Each agendaInLista In listaAgenda

                        Dim agendaRaccoglitore As Operazione_Agenda = objAgenda.Leggi(agendaInLista.Piva, 0, agendaInLista.Id_Agenda, 0, objParametri_Server)

                        If agendaRaccoglitore IsNot Nothing Then
                            Dim listParametriAggiuntivi As New List(Of Parametri_Aggiuntivi_Attivita)

                            currentAttivitaDes = agendaRaccoglitore.Des_Lib
                            Dim attivita = map.AgendaSuAttivita(agendaRaccoglitore, verbose:=True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, listParametriAggiuntivi)

                            Dim attivitaConParametriAggiuntivi = AgronicaCoreModelsSTD.attivita.AttivitaConParametriAggiuntivi.Create(attivita, listParametriAggiuntivi)

                            listaAttivita.Add(attivitaConParametriAggiuntivi)
                        End If

                    Next
                    AgronicaCoreMapper.Utility.SetDisciplinareMulti(listaAttivita)
                End If
            End If

            r.RispostaStringa = listaAttivita

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = currentAttivitaDes & ": " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAttivitaDaAgenda(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Object)
        Return LeggiAttivitaDaAgenda(InData, True)
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAttivitaDaAgendaNonVerbose(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Object)
        Return LeggiAttivitaDaAgenda(InData, False)
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getNewAgenda(ByVal Piva As String,
                                 ByVal Sa_Cod As Integer,
                                 ByVal Veg_Cod As Integer,
                                 ByVal ID_Cod As Integer,
                                 ByVal Lav_Cod As Integer,
                                 ByVal Data As String,
                                 ByVal objParametri As String) As AgronicaCoreContabObject.Operazione
        Dim risp As New AgronicaCoreContabObject.Operazione(Piva, Lav_Cod, Data)
        risp.Veg_Cod = Veg_Cod
        risp.ID_Cod = ID_Cod

        Dim objimp As New Impianti
        'devo inizializzare la lista di impianti
        risp.ListaImpianti = objimp.getListaImpianti(risp.Piva, risp.Sa_Cod, risp.Disciplinare_Cod,
                                             risp.Veg_Cod, risp.ID_Cod, Data, objParametri)



        Return risp
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getNewAgenda_NG(InData As CoreWS_Generic(Of getNewAgenda)) As AgronicaCoreContabObject.Operazione

        Dim risp As New AgronicaCoreContabObject.Operazione(InData.InData.Piva, InData.InData.Lav_Cod, InData.InData.Data)
        risp.Veg_Cod = InData.InData.Veg_Cod
        risp.ID_Cod = InData.InData.ID_Cod

        Dim objimp As New Impianti
        'devo inizializzare la lista di impianti
        risp.ListaImpianti = objimp.getListaImpianti(risp.Piva, risp.Sa_Cod, risp.Disciplinare_Cod,
                                             risp.Veg_Cod, risp.ID_Cod, InData.InData.Data, InData.objP.objP_server)




        Return risp
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Shared Function scriviAgenda(ByVal agenda As Operazione_Agenda,
                                   ByVal ASG_ProgressivoGIAS As Integer,
                                   ByVal IdServizio As Integer,
                                ByVal objParametri As String) As RispostaStandard
        Dim ris As New RispostaStandard




        ''inizializzare base e top code
        ''AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, Utility.convertStringtoOBJparametri(objParametri))
        'Dim objParametri_OGGETTO_SERVER As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objParametri)
        'Dim objAgendaScrivi As New Agenda_Operazione_Helper

        'If agenda.Id_Agenda <> 0 Then
        '    Dim CancellataOperazione As Boolean = False
        '    CancellataOperazione = objAgendaScrivi.Cancella(agenda.Piva, _
        '                                                     agenda.Sa_Cod, _
        '                                                     agenda.Id_Agenda, False, _
        '                                                    objParametri_OGGETTO_SERVER)
        'End If

        'Dim Id_Agenda = objAgendaScrivi.Scrivi(agenda, objParametri_OGGETTO_SERVER)





        Return ris
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getGiustificazione_NG(InData As Object) As List(Of Nota_Operazione)
        Dim risp As New List(Of Nota_Operazione)

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))
        Dim Data As String = InData.InData

        'carico solamente le giustificazioni e non se sono ceccate 

        Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

        'modifico la finestra temporale in modo da caricare solamente quelle che sono attive a oggi
        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data,
                                                                      Data)

        Dim CBL_Consigli As New CheckBoxList

        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Consigli,
                                                        False,
                                                         "", "",
                                                         0,
                                                         -2,
                                                         "", "",
                                                         objParametri_Server)

        objParametri_Server.ResettaFinestra()


        'converto la checkbox su oggetto
        For Each e In CBL_Consigli.Items
            Dim n As New Nota_Operazione
            n.Nota_Cod = e.Value
            n.Descrizione = e.Text
            n.selected = e.Selected

            risp.Add(n)
        Next

        Return risp
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getGiustificazione(ByVal InData As CoreWS_Generic(Of String),
                                       ByVal objParametri As String) As List(Of Nota_Operazione)
        Dim risp As New List(Of Nota_Operazione)

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))
        Dim Data As String = InData.InData

        'carico solamente le giustificazioni e non se sono ceccate 

        Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

        'modifico la finestra temporale in modo da caricare solamente quelle che sono attive a oggi
        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data,
                                                                      Data)

        Dim CBL_Consigli As New CheckBoxList

        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Consigli,
                                                        False,
                                                         "", "",
                                                         0,
                                                         -2,
                                                         "", "",
                                                         objParametri_Server)

        objParametri_Server.ResettaFinestra()


        'converto la checkbox su oggetto
        For Each e In CBL_Consigli.Items
            Dim n As New Nota_Operazione
            n.Nota_Cod = e.Value
            n.Descrizione = e.Text
            n.selected = e.Selected

            risp.Add(n)
        Next

        Return risp
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function getGiustificazione(ByVal Data As String,
                                       ByVal objParametri As String) As List(Of Nota_Operazione)
        Dim risp As New List(Of Nota_Operazione)

        'carico solamente le giustificazioni e non se sono ceccate 
        Dim objParametri_Server = Utility.convertStringtoOBJparametri(objParametri)

        'modifico la finestra temporale in modo da caricare solamente quelle che sono attive a oggi
        objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(Data,
                                                                      Data)

        Dim CBL_Consigli As New CheckBoxList

        AgronicaCoreUtility.CaricaListControl.Note_Intervento(CBL_Consigli,
                                                        False,
                                                         "", "",
                                                         0,
                                                         -2,
                                                         "", "",
                                                         objParametri_Server)

        objParametri_Server.ResettaFinestra()


        'converto la checkbox su oggetto
        For Each e In CBL_Consigli.Items
            Dim n As New Nota_Operazione
            n.Nota_Cod = e.Value
            n.Descrizione = e.Text
            n.selected = e.Selected

            risp.Add(n)
        Next

        Return risp
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaDDT(ByVal Piva As String, ByVal idAgenda As Integer, ByVal idTipologia As Integer, ByVal objP_server As String) As rispostaStandard(Of Object)
        Dim r As New rispostaStandard(Of Object)
        Dim lavCod As String = ""
        Dim xFiltroAggiuntivo As String = ""


        If idTipologia <> Nothing Then

            Select Case idTipologia
                Case -16 'Fatture Attive da Sistema Esterno
                    lavCod = "1031"
                Case -17 'Fatture Passive da Sistema Esterno
                    lavCod = "1025,1054,1076,1078"
                Case -18 'Ordini Acquisto
                    lavCod = "2004"
                Case -19 'DDT Ricevuti
                    lavCod = "1025"
                Case -20 'Conferimenti
                    lavCod = "1054,1076,1078"
                Case -21 'DDT Emessi
                    lavCod = "1031"
                Case -22 'Ordine Vendita
                    lavCod = "2002"
                Case -23 'Fatture Passive
                    lavCod = "1000"
                Case -24 'Fatture Attive
                    lavCod = "1001"
            End Select

            If lavCod <> "" Then
                xFiltroAggiuntivo = "Lav_Cod in (" + lavCod + ")"
            End If
        End If

        Dim currentAttivitaDes = ""
        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)

            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            Dim dt = objMov.MovimentiContabili_Contatto(0, Piva, 0, idAgenda, 0, 0, CAU_REGISTRAZIONI, 0, AGRODATAINIZIO, AGRODATAFINE, "XYZ", 0, "XYZ", 0, 0, AGRODATAINIZIO, xFiltroAggiuntivo, "", objParametri_Server)
            Dim strAgendaDDT As String = JsonConvert.SerializeObject(dt)

            r.RispostaStringa = strAgendaDDT

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = currentAttivitaDes & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaDDT_NG(InData As CoreWS_Generic(Of LeggiAgendaDDT)) As rispostaStandard(Of Object)
        Dim r As New rispostaStandard(Of Object)
        Dim lavCod As String = ""
        Dim xFiltroAggiuntivo As String = ""


        If InData.InData.idTipologia <> Nothing Then

            Select Case InData.InData.idTipologia
                Case -16 'Fatture Attive da Sistema Esterno
                    lavCod = "1031"
                Case -17 'Fatture Passive da Sistema Esterno
                    lavCod = "1025,1054,1076,1078"
                Case -18 'Ordini Acquisto
                    lavCod = "2004"
                Case -19 'DDT Ricevuti
                    lavCod = "1025"
                Case -20 'Conferimenti
                    lavCod = "1054,1076,1078"
                Case -21 'DDT Emessi
                    lavCod = "1031"
                Case -22 'Ordine Vendita
                    lavCod = "2002"
                Case -23 'Fatture Passive
                    lavCod = "1000"
                Case -24 'Fatture Attive
                    lavCod = "1001"
            End Select

            If lavCod <> "" Then
                xFiltroAggiuntivo = "Lav_Cod in (" + lavCod + ")"
            End If
        End If

        Dim currentAttivitaDes = ""
        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            Dim dt = objMov.MovimentiContabili_Contatto(0, InData.InData.Piva, 0, InData.InData.idAgenda, 0, 0, CAU_REGISTRAZIONI, 0, AGRODATAINIZIO, AGRODATAFINE, "XYZ", 0, "XYZ", 0, 0, AGRODATAINIZIO, xFiltroAggiuntivo, "", objParametri_Server)
            Dim strAgendaDDT As String = JsonConvert.SerializeObject(dt)

            r.RispostaStringa = strAgendaDDT
        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = currentAttivitaDes & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CheckConformita(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim currentAttivitaDes = ""

        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            'Istanzio gli oggetti InData.
            Dim strAttivitaMulti As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataAttivitaMulti As List(Of AgronicaCoreModelsSTD.attivita.Attivita) =
                JsonConvert.DeserializeObject(Of List(Of AgronicaCoreModelsSTD.attivita.Attivita))(strAttivitaMulti,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                                                                                          )

            Dim map As New AgronicaCoreMapper.AttivitaToAgenda

            'Check di ogni attivita, se esiste anche solo un errore bloccante, l'esito sarà risposta_ok=false
            r.RispostaOK = True
            r.RispostaStringa = "Operazione eseguita correttamente"
            r.ErroriGias = New List(Of ErroreGias)


            Dim listaErroriGlobale = New List(Of ErroreGias)
            For Each attivita In inDataAttivitaMulti

                currentAttivitaDes = attivita.job.descrizione

                Dim listaErroriAttivita = map.CheckConformita(attivita)

                'Si esce alla prima attività con errore bloccante 
                If listaErroriAttivita IsNot Nothing AndAlso listaErroriAttivita.Count > 0 Then
                    Dim erroreBloccante = listaErroriAttivita.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante)).FirstOrDefault

                    If erroreBloccante IsNot Nothing Then
                        r.RispostaOK = False
                        r.Errore = currentAttivitaDes + ": " + erroreBloccante.messaggio
                        r.ErroriGias.Add(erroreBloccante)
                        Return r
                    Else
                        r.RispostaOK = False
                        r.Errore = "Errori non bloccanti"
                        r.ErroriGias.AddRange(listaErroriAttivita)
                    End If
                End If
            Next

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = InData.InData
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = currentAttivitaDes & ": " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Controllo_Inserimento_Dose_Prodotto(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)


            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            'Istanzio gli oggetti InData.
            Dim strControlloInserimentoDoseProdotto As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataControlloInserimentoDoseProdotto As Controllo_Inserimento_Dose_Prodotto = JsonConvert.DeserializeObject(Of Controllo_Inserimento_Dose_Prodotto)(strControlloInserimentoDoseProdotto,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})


            Dim map As New AgronicaCoreMapper.CheckAttivita
            Dim listaErrori = map.ControlloInserimentoDoseProdotto(inDataControlloInserimentoDoseProdotto, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = "Operazione eseguita correttamente"
            r.ErroriGias = New List(Of ErroreGias)

            'Si esce alla prima attività con errore bloccante 
            If listaErrori IsNot Nothing AndAlso listaErrori.Count > 0 Then
                Dim erroreBloccante = listaErrori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante)).FirstOrDefault

                If erroreBloccante IsNot Nothing Then
                    r.RispostaOK = False
                    r.ErroriGias.Add(erroreBloccante)
                    Return r
                Else
                    r.RispostaOK = False
                    r.ErroriGias.AddRange(listaErrori)
                End If
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = InData.InData
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function


#Region "Funzioni private"

    Private Function LeggiListaAttivitaDaRicettaOperazione(InData As CoreWS_Generic(Of Object), tipoAttivita As Tipo_Attivita, stato As Stati) As rispostaStandard(Of Object)
        Dim r As New rispostaStandard(Of Object)

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim currentAttivitaDes = ""
        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'Istanzio gli oggetti InData.
            Dim strricettaoperazione As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataRicetta As Ricette_Operazioni = JsonConvert.DeserializeObject(Of Ricette_Operazioni)(strricettaoperazione,
                                                                                          New JsonSerializerSettings With {
                                                                                              .DateTimeZoneHandling = DateTimeZoneHandling.Local
                                                    })
            inDataRicetta.Ricetta_SuperUser = objParametri_Server.PivaSuperUser

            'Carico tutte le ricette con lo stesso raccoglitore_cod della ricetta passata in input
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametri_Server.StringaConnessione)
            Dim db As New Gias_DeveloperServer_Entities(EFConnString)

            Dim ricetta_operazione As Ricette_Operazioni = EFRicette.ReadRicettaOperazione(db, inDataRicetta.Ricetta_SuperUser, inDataRicetta.Ricetta_Operazione_Cod)

            Dim listaAttivita = New List(Of AgronicaCoreModelsSTD.attivita.AttivitaConParametriAggiuntivi)
            Dim map As New AgronicaCoreMapper.RicettaToAttivita

            Dim isRibaltamentoToAgenda = False
            If tipoAttivita = Tipo_Attivita.QuadernoDiCampagna Then
                isRibaltamentoToAgenda = True
            End If

            If ricetta_operazione.Raccoglitore_Cod = 0 Then
                Dim listParametriAggiuntivi As New List(Of Parametri_Aggiuntivi_Attivita)
                currentAttivitaDes = ricetta_operazione.Ricetta_Operazione_Des
                Dim attivita = map.RicettaOperazioneSuAttivita(ricetta_operazione, listParametriAggiuntivi, verbose:=True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, isRibaltamentoToAgenda)

                Dim attivitaConParametriAggiuntivi = AgronicaCoreModelsSTD.attivita.AttivitaConParametriAggiuntivi.Create(attivita, listParametriAggiuntivi)

                listaAttivita.Add(attivitaConParametriAggiuntivi)
            Else
                Dim listaRicette = EFRicette.ReadRicetteOperazioneByRaccoglitore(db, ricetta_operazione.Ricetta_SuperUser, ricetta_operazione.Raccoglitore_Cod)

                If listaRicette IsNot Nothing Then
                    For Each ricettaOperazioneInLista In listaRicette
                        Dim listParametriAggiuntivi As New List(Of Parametri_Aggiuntivi_Attivita)
                        currentAttivitaDes = ricettaOperazioneInLista.Ricetta_Operazione_Des
                        Dim attivita = map.RicettaOperazioneSuAttivita(ricettaOperazioneInLista, listParametriAggiuntivi, verbose:=True, objParametri_Super_Server, objParametri_Server, objParametri_Utenti, isRibaltamentoToAgenda)

                        Dim attivitaConParametriAggiuntivi = AgronicaCoreModelsSTD.attivita.AttivitaConParametriAggiuntivi.Create(attivita, listParametriAggiuntivi)

                        listaAttivita.Add(attivitaConParametriAggiuntivi)
                    Next

                    AgronicaCoreMapper.Utility.SetDisciplinareMulti(listaAttivita)

                End If
            End If

            For Each attivita In listaAttivita

                'ribaltamento ricetta/brogliaccio --> agenda
                If tipoAttivita = Tipo_Attivita.QuadernoDiCampagna Then

                    attivita.associazionePK = New AssociazionePK
                    attivita.associazionePK.id_agenda = 0
                    attivita.associazionePK.Ricetta_Cod = attivita.testataRicetta.Ricetta_Cod
                    attivita.associazionePK.Ricetta_Operazione_Cod = CInt(attivita.codice)

                    attivita.tipo = tipoAttivita
                    attivita.stato = Nothing
                    attivita.testataRicetta = Nothing
                    attivita.codice = "0"
                    attivita.raccoglitore = 0
                End If

                'ribaltamento ricetta --> brogliaccio
                If tipoAttivita = Tipo_Attivita.Ricetta AndAlso stato = Stati.Eseguita Then

                    attivita.associazionePK = Nothing

                    attivita.tipo = tipoAttivita
                    attivita.stato = stato
                    'DT: la testata rimane le stessa (il collegamento fra ricetta e brogliaccio non passa da RicettexAgenda ma dall'avere la stessa testata
                    attivita.codice = "0"
                    attivita.raccoglitore = 0
                End If

            Next

            r.RispostaOK = True
            r.RispostaStringa = listaAttivita

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = currentAttivitaDes & ": " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
    Private Function LeggiAttivitaDaAgenda(InData As CoreWS_Generic(Of Object), verbose As Boolean) As rispostaStandard(Of Object)
        Dim r As New rispostaStandard(Of Object)

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try
            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'Istanzio gli oggetti InData.
            Dim strAgenda As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataAgenda As Operazione_Agenda = JsonConvert.DeserializeObject(Of Operazione_Agenda)(strAgenda)

            Dim objAgenda As New Agenda_Operazione_Helper
            Dim agenda As Operazione_Agenda = objAgenda.Leggi(inDataAgenda.Piva, 0, inDataAgenda.Id_Agenda, 0, objParametri_Server)

            Dim map As New AgronicaCoreMapper.AgendaToAttivita

            Dim attivita = map.AgendaSuAttivita(agenda, verbose, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = attivita

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

#End Region


#Region "lettura agenda per Documentale"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaDDT_toKendoGrid(ByVal Piva As String,
                                               ByVal idAgenda As Integer,
                                               ByVal idArea As Integer,
                                               ByVal idTipologia As Integer,
                                               ByVal objP_server As String,
                                               ByVal dataDa As String,
                                               ByVal dataA As String,
                                               ByVal escludiIdAgenda As Boolean
                                               ) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiAgendaDDT_toKendoGrid()"

        Dim r As New rispostaStandard(Of Object)
        Dim lavCod As String = ""
        Dim xFiltroAggiuntivo As String = ""
        Dim joinContatti As Boolean = True
        Dim CAU_MOV As Integer = CAU_REGISTRAZIONI

        If idTipologia <> Nothing Then

            Select Case idTipologia
                Case enum_ID_Area_Tipologia.Fatture_Attive_Da_Sistema_Esterno
                    lavCod = "1031"
                Case enum_ID_Area_Tipologia.Fatture_Passive_Da_Sistema_Esterno
                    lavCod = "1025,1054,1076,1078"
                Case enum_ID_Area_Tipologia.Ordini_Acquisto
                    lavCod = "2004"
                Case enum_ID_Area_Tipologia.DDT_Ricevuti
                    lavCod = "1025"
                Case enum_ID_Area_Tipologia.Conferimenti
                    lavCod = "1054,1076,1078"
                Case enum_ID_Area_Tipologia.DDT_Emessi
                    lavCod = "1031"
                Case enum_ID_Area_Tipologia.Ordini_Vendita
                    lavCod = "2002"
                Case enum_ID_Area_Tipologia.Fatture_Passive
                    lavCod = "1000"
                Case enum_ID_Area_Tipologia.Fatture_Attive
                    lavCod = "1001"
                Case enum_ID_Area_Tipologia.Contratti_Affitto
                    lavCod = "2006"
                Case enum_ID_Area_Tipologia.Carichi_Magazzino
                    lavCod = "1022"
                    joinContatti = False
                    CAU_MOV = CAU_CARICO
                Case enum_ID_Area_Tipologia.Scarichi_Magazzino
                    lavCod = "1023"
                    joinContatti = False
                    CAU_MOV = CAU_SCARICO
            End Select

            If lavCod <> "" Then
                xFiltroAggiuntivo = " Agenda.Lav_Cod in (" + lavCod + ")"
            ElseIf idArea <> Nothing Then

                Select Case idArea
                    Case enum_ID_Area_Alert.Documenti_Contabili
                        lavCod = "1031,1025,1054,1076,1078,2004,1025,1054,1076,1078,1031,2002,1000,1001,2006"
                        xFiltroAggiuntivo = " Agenda.Lav_Cod in (" + lavCod + ")"
                    Case enum_ID_Area_Alert.Operazioni_Campagna_QDC
                        xFiltroAggiuntivo = " Agenda.Lav_Cod > 0 and Agenda.Lav_Cod < 1000 )"
                    Case enum_ID_Area_Alert.Carichi_Scarichi_Magazzino
                        lavCod = "1022,1023"
                        xFiltroAggiuntivo = " Agenda.Lav_Cod in (" + lavCod + ")"
                End Select

            End If


        End If

        If dataDa <> "" Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo += " AND "
            End If
            xFiltroAggiuntivo += " Movimenti.Data_Movimento >= '" & dataDa & "'"
        End If

        If dataA <> "" Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo += " AND "
            End If
            xFiltroAggiuntivo += " Movimenti.Data_Movimento <= '" & dataA & "'"
        End If

        If escludiIdAgenda = True Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo += " AND "
            End If
            xFiltroAggiuntivo += " Agenda.ID_Agenda <> " & idAgenda
            idAgenda = 0
        End If

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            Dim dt = objMov.MovimentiContabili_Contatto(0,
                                                        Piva,
                                                        0,
                                                        idAgenda,
                                                        0,
                                                        0,
                                                        CAU_MOV,
                                                        0,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        "XYZ",
                                                        0,
                                                        "XYZ",
                                                        0,
                                                        0,
                                                        AGRODATAINIZIO,
                                                        xFiltroAggiuntivo,
                                                        "",
                                                        objParametri_Server,
                                                        DescrizioneOperazione_xDocumentale:=True,
                                                        idTipologia,
                                                        joinContatti:=joinContatti)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Id_Agenda", "Id_Agenda", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("Operazione", Gias.Operazione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("des_lib", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})


            If joinContatti Then
                l.Add(New ColonneNome("Rag_Soc", Gias.ContattoDocumento, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
                l.Add(New ColonneNome("Cod_Contatto", "Cod_Contatto", "string") With {._hidden = True})
            End If

            l.Add(New ColonneNome("Doc_Numero_Visualizzato", Gias.nrDocumento, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            'se si tratta di un contratto d'affitto, aggiunge una colonna legata alla scadenza di testata
            Select Case idTipologia
                Case -26 'Contratti d'affitto
                    l.Add(New ColonneNome("Data_Movimento", Gias.DataRegistrazione, "date"))
                    l.Add(New ColonneNome("Scadenza_Contratto", Gias.DataScadenza, "date"))

                Case Else
                    l.Add(New ColonneNome("Data_Movimento", Gias.Data, "date"))
            End Select

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaDDT_toKendoGrid_NG(InData As CoreWS_Generic(Of LeggiAgendaDDT_toKendoGrid)) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiAgendaDDT_toKendoGrid()"

        Dim r As New rispostaStandard(Of Object)
        Dim lavCod As String = ""
        Dim xFiltroAggiuntivo As String = ""
        Dim joinContatti As Boolean = True
        Dim CAU_MOV As Integer = CAU_REGISTRAZIONI

        If InData.InData.idTipologia <> Nothing Then

            Select Case InData.InData.idTipologia
                Case enum_ID_Area_Tipologia.Fatture_Attive_Da_Sistema_Esterno
                    lavCod = "1031"
                Case enum_ID_Area_Tipologia.Fatture_Passive_Da_Sistema_Esterno
                    lavCod = "1025,1054,1076,1078"
                Case enum_ID_Area_Tipologia.Ordini_Acquisto
                    lavCod = "2004"
                Case enum_ID_Area_Tipologia.DDT_Ricevuti
                    lavCod = "1025"
                Case enum_ID_Area_Tipologia.Conferimenti
                    lavCod = "1054,1076,1078"
                Case enum_ID_Area_Tipologia.DDT_Emessi
                    lavCod = "1031"
                Case enum_ID_Area_Tipologia.Ordini_Vendita
                    lavCod = "2002"
                Case enum_ID_Area_Tipologia.Fatture_Passive
                    lavCod = "1000"
                Case enum_ID_Area_Tipologia.Fatture_Attive
                    lavCod = "1001"
                Case enum_ID_Area_Tipologia.Contratti_Affitto
                    lavCod = "2006"
                Case enum_ID_Area_Tipologia.Carichi_Magazzino
                    lavCod = "1022"
                    joinContatti = False
                    CAU_MOV = CAU_CARICO
                Case enum_ID_Area_Tipologia.Scarichi_Magazzino
                    lavCod = "1023"
                    joinContatti = False
                    CAU_MOV = CAU_SCARICO
            End Select

            If lavCod <> "" Then
                xFiltroAggiuntivo = " Agenda.Lav_Cod in (" + lavCod + ")"
            End If
        End If

        If InData.InData.dataDa <> "" Then
            xFiltroAggiuntivo += " AND Movimenti.Data_Movimento >= '" & InData.InData.dataDa & "'"
        End If
        If InData.InData.dataA <> "" Then
            xFiltroAggiuntivo += " AND Movimenti.Data_Movimento <= '" & InData.InData.dataA & "'"
        End If

        If InData.InData.escludiIdAgenda = True Then
            xFiltroAggiuntivo += " AND Agenda.ID_Agenda <> " & InData.InData.idAgenda
            InData.InData.idAgenda = 0
        End If

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod,
                                                                  AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "", "", objParametri_Server)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim objMov As New AgronicaCoreContabDAL.Movimenti_R
            Dim dt = objMov.MovimentiContabili_Contatto(0,
                                                        InData.InData.Piva,
                                                        0,
                                                        InData.InData.idAgenda,
                                                        0,
                                                        0,
                                                        CAU_MOV,
                                                        0,
                                                        AGRODATAINIZIO,
                                                        AGRODATAFINE,
                                                        "XYZ",
                                                        0,
                                                        "XYZ",
                                                        0,
                                                        0,
                                                        AGRODATAINIZIO,
                                                        xFiltroAggiuntivo,
                                                        "",
                                                        objParametri_Server,
                                                        DescrizioneOperazione_xDocumentale:=True,
                                                        InData.InData.idTipologia,
                                                        joinContatti:=joinContatti)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Id_Agenda", "Id_Agenda", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("Operazione", Gias.Operazione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("des_lib", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})


            If joinContatti Then
                l.Add(New ColonneNome("Rag_Soc", Gias.ContattoDocumento, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
                l.Add(New ColonneNome("Cod_Contatto", "Cod_Contatto", "string") With {._hidden = True})
            End If

            l.Add(New ColonneNome("Doc_Numero_Visualizzato", Gias.nrDocumento, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})

            'se si tratta di un contratto d'affitto, aggiunge una colonna legata alla scadenza di testata
            Select Case InData.InData.idTipologia
                Case -26 'Contratti d'affitto
                    l.Add(New ColonneNome("Data_Movimento", Gias.DataRegistrazione, "date"))
                    l.Add(New ColonneNome("Scadenza_Contratto", Gias.DataScadenza, "date"))

                Case Else
                    l.Add(New ColonneNome("Data_Movimento", Gias.Data, "date"))
            End Select

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaAttivita_toKendoGrid(ByVal Piva As String,
                                                    ByVal idAgenda As Integer,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal objP_server As String,
                                                    ByVal dataDa As String,
                                                    ByVal dataA As String) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiAgendaAttivita_toKendoGrid()"

        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        If dataDa <> "" Then
            xFiltroAggiuntivo += " Agenda.Validita_Inizio >= '" & dataDa & "'"
        End If
        If dataA <> "" Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo += " AND "
            End If
            xFiltroAggiuntivo += " Agenda.Validita_Inizio <= '" & dataA & "'"
        End If

        If xFiltroAggiuntivo <> "" Then
            xFiltroAggiuntivo += " AND "
        End If
        xFiltroAggiuntivo += " (Agenda.Lav_Cod > 0 AND Agenda.Lav_Cod < 1000)"

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)

            Dim objMov As New AgronicaCoreContabDAL.Agenda_R
            Dim dt = objMov.Leggi(Piva, Sa_Cod, idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Server, DescrizioneOperazione_xDocumentale:=True)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Id_Agenda", "Id_Agenda", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("Operazione", Gias.Operazione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("des_lib", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.Data, "date"))

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaAttivita_toKendoGrid_NG(InData As CoreWS_Generic(Of LeggiAgendaAttivita_toKendoGrid)) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiAgendaAttivita_toKendoGrid()"

        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        If InData.InData.dataDa <> "" Then
            xFiltroAggiuntivo += " Agenda.Validita_Inizio >= '" & InData.InData.dataDa & "'"
        End If
        If InData.InData.dataA <> "" Then
            xFiltroAggiuntivo += " AND Agenda.Validita_Inizio <= '" & InData.InData.dataA & "'"
        End If

        If InData.InData.dataDa <> "" Then
            xFiltroAggiuntivo += " AND "
        End If

        xFiltroAggiuntivo += " (Agenda.Lav_Cod > 0 AND Agenda.Lav_Cod < 1000)"

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objMov As New AgronicaCoreContabDAL.Agenda_R
            Dim dt = objMov.Leggi(InData.InData.Piva, InData.InData.Sa_Cod, InData.InData.idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Server, DescrizioneOperazione_xDocumentale:=True)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Id_Agenda", "Id_Agenda", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("Operazione", Gias.Operazione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("des_lib", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.Data, "date"))

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaGenerica_toKendoGrid(ByVal Piva As String,
                                                    ByVal idAgenda As Integer,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal objP_server As String,
                                                    ByVal dataDa As String,
                                                    ByVal dataA As String) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiAgendaGenerica_toKendoGrid()"


        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        If dataDa <> "" Then
            xFiltroAggiuntivo += " Agenda.Validita_Inizio >= '" & dataDa & "'"
        End If
        If dataA <> "" Then
            If xFiltroAggiuntivo <> "" Then
                xFiltroAggiuntivo += " AND "
            End If
            xFiltroAggiuntivo += " Agenda.Validita_Inizio <= '" & dataA & "'"
        End If

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)

            Dim objMov As New AgronicaCoreContabDAL.Agenda_R
            Dim dt = objMov.Leggi(Piva, Sa_Cod, idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Server)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Id_Agenda", "Id_Agenda", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("des_lib", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.Data, "date"))

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaGenerica_toKendoGrid_NG(InData As CoreWS_Generic(Of LeggiAgendaGenerica_toKendoGrid)) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiAgendaGenerica_toKendoGrid()"


        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        If InData.InData.dataDa <> "" Then
            xFiltroAggiuntivo += " Agenda.Validita_Inizio >= '" & InData.InData.dataDa & "'"
        End If
        If InData.InData.dataA <> "" Then
            xFiltroAggiuntivo += " AND Agenda.Validita_Inizio <= '" & InData.InData.dataA & "'"
        End If

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objMov As New AgronicaCoreContabDAL.Agenda_R
            Dim dt = objMov.Leggi(InData.InData.Piva, InData.InData.Sa_Cod, InData.InData.idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Server)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Id_Agenda", "Id_Agenda", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("des_lib", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.Data, "date"))

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaVisite_toKendoGrid_NG(InData As CoreWS_Generic(Of LeggiAgendaVisite_toKendoGrid)) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiAgendaVisite_toKendoGrid()"

        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        If InData.InData.dataDa <> "" Then
            xFiltroAggiuntivo += " Agenda.Validita_Inizio >= '" & InData.InData.dataDa & "'"
        End If
        If InData.InData.dataA <> "" Then
            xFiltroAggiuntivo += " AND Agenda.Validita_Inizio <= '" & InData.InData.dataA & "'"
        End If

        If InData.InData.dataDa <> "" Then
            xFiltroAggiuntivo += " AND "
        End If

        xFiltroAggiuntivo += " Agenda.Lav_Cod IN (5001,5002,5007)"

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objMov As New AgronicaCoreContabDAL.Agenda_R
            Dim dt = objMov.Leggi(InData.InData.Piva, InData.InData.Sa_Cod, InData.InData.idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Server, DescrizioneOperazione_xDocumentale:=True)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Id_Agenda", "Id_Agenda", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("Operazione", Gias.Operazione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("des_lib", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.DataVisita, "date"))

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendaVisite_toKendoGrid(ByVal Piva As String,
                                                 ByVal idAgenda As Integer,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal objP_server As String,
                                                  ByVal dataDa As String,
                                                  ByVal dataA As String) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiAgendaVisite_toKendoGrid()"

        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        If dataDa <> "" Then
            xFiltroAggiuntivo += " Agenda.Validita_Inizio >= '" & dataDa & "'"
        End If
        If dataA <> "" Then
            xFiltroAggiuntivo += " AND Agenda.Validita_Inizio <= '" & dataA & "'"
        End If

        If dataDa <> "" Then
            xFiltroAggiuntivo += " AND "
        End If

        xFiltroAggiuntivo += " Agenda.Lav_Cod IN (5001,5002,5007)"

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)

            Dim objMov As New AgronicaCoreContabDAL.Agenda_R
            Dim dt = objMov.Leggi(Piva, Sa_Cod, idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Server, DescrizioneOperazione_xDocumentale:=True)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Id_Agenda", "Id_Agenda", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("Operazione", Gias.Operazione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("des_lib", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.DataVisita, "date"))

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function
#End Region

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRicetteBrogliaccio_toKendoGrid_NG(InData As CoreWS_Generic(Of LeggiRicetteBrogliaccio_toKendoGrid)) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiRicetteBrogliaccio_toKendoGrid_NG()"

        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        If InData.InData.dataDa <> "" Then
            xFiltroAggiuntivo += " Ricette_Operazioni.Validita_Inizio >= '" & InData.InData.dataDa & "'"
        End If
        If InData.InData.dataA <> "" Then
            xFiltroAggiuntivo += " AND Ricette_Operazioni.Validita_Inizio <= '" & InData.InData.dataA & "'"
        End If

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objRicetteBrogliaccio As New AgronicaCoreContabDAL.Ricette_Operazioni_R
            Dim dt = objRicetteBrogliaccio.Leggi(0, InData.InData.Ricetta_Operazione_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Server, joinRicette:=True)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("W_Anagrafica_Stati_Cod", "W_Anagrafica_Stati_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("Ricetta_Operazione_Cod", "Ricetta_Operazione_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("RicettaBrogliaccio", Gias.RicettaODLBrogliaccio, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Ricetta_Operazione_Des", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Codice_Ricetta", Gias.CodiceRicetta, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.Data, "date"))

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRicetteBrogliaccio_toKendoGrid(InData As CoreWS_Generic(Of LeggiRicetteBrogliaccio_toKendoGrid)) As rispostaStandard(Of Object)
        Dim NomeRoutine As String = "Agenda.asmx/LeggiRicetteBrogliaccio_toKendoGrid()"

        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        If InData.InData.dataDa <> "" Then
            xFiltroAggiuntivo += " Ricette_Operazioni.Validita_Inizio >= '" & InData.InData.dataDa & "'"
        End If
        If InData.InData.dataA <> "" Then
            xFiltroAggiuntivo += " AND Ricette_Operazioni.Validita_Inizio <= '" & InData.InData.dataA & "'"
        End If

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objRicetteBrogliaccio As New AgronicaCoreContabDAL.Ricette_Operazioni_R
            Dim dt = objRicetteBrogliaccio.Leggi(0, InData.InData.Ricetta_Operazione_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Server, joinRicette:=True)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("W_Anagrafica_Stati_Cod", "W_Anagrafica_Stati_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("Ricetta_Operazione_Cod", "Ricetta_Operazione_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("RicettaBrogliaccio", Gias.RicettaODLBrogliaccio, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Ricetta_Operazione_Des", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Codice_Ricetta", Gias.CodiceRicetta, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.Data, "date"))

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiRicetteBrogliaccio_toKendoGrid(ByVal Ricetta_Operazione_Cod As Integer,
                                                        ByVal objP_server As String,
                                                        ByVal dataDa As String,
                                                        ByVal dataA As String) As rispostaStandard(Of Object)

        Dim NomeRoutine As String = "Agenda.asmx/LeggiRicetteBrogliaccio_toKendoGrid()"
        Dim r As New rispostaStandard(Of Object)
        Dim xFiltroAggiuntivo As String = ""

        If dataDa <> "" Then
            xFiltroAggiuntivo += " Ricette_Operazioni.Validita_Inizio >= '" & dataDa & "'"
        End If
        If dataA <> "" Then
            xFiltroAggiuntivo += " AND Ricette_Operazioni.Validita_Inizio <= '" & dataA & "'"
        End If

        Try
            Dim objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)

            Dim objRicetteBrogliaccio As New AgronicaCoreContabDAL.Ricette_Operazioni_R
            Dim dt = objRicetteBrogliaccio.Leggi(0, Ricetta_Operazione_Cod, 0, 0, AGRODATAINIZIO, AGRODATAFINE, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_Server, joinRicette:=True)

            Dim l As New List(Of ColonneNome)
            l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("Lav_Cod", "Lav_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("W_Anagrafica_Stati_Cod", "W_Anagrafica_Stati_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("Ricetta_Operazione_Cod", "Ricetta_Operazione_Cod", "number") With {._hidden = True})

            l.Add(New ColonneNome("RicettaBrogliaccio", Gias.RicettaODLBrogliaccio, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Ricetta_Operazione_Des", Gias.Descrizione, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Codice_Ricetta", Gias.CodiceRicetta, "string") With {._Filtrabile = True, ._FiltrabileConCheck = True})
            l.Add(New ColonneNome("Validita_Inizio", Gias.Data, "date"))

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}

            r.RispostaOK = False
            r.Errore = NomeRoutine & ": " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Controllo_Sportello(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Object)

        Dim r As New rispostaStandard(Of Object)

        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try

            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim InDataControllo_Sportello As Controllo_Sportello = JsonConvert.DeserializeObject(Of Controllo_Sportello)(JsonConvert.SerializeObject(InData.InData, settings), settings)
            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            Dim objPratiche As New AgronicaCoreProfilazioneBIZ.Pratiche_R

            objPratiche.Data_Sportello_Da_Servizio(InDataControllo_Sportello.Piva,
                                                   InDataControllo_Sportello.Servizio_Cod,
                                                   InDataControllo_Sportello.Data_Riferimento,
                                                   InDataControllo_Sportello.SportelloAperto,
                                                   InDataControllo_Sportello.Data_Min,
                                                   InDataControllo_Sportello.Data_Max,
                                                   objParametri_Server,
                                                   objParametri_Utenti)

            r.RispostaOK = True

            r.RispostaStringa = InDataControllo_Sportello

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Inizializza_QdC_NG(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Inizializza_QdC)

        Dim r As New rispostaStandard(Of Inizializza_QdC)

        Try

            Dim obj_Inizializza_QdC As New Inizializza_QdC

            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim InDataInizializza As CoreWS_Generic(Of LeggiInizializza_QdC) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiInizializza_QdC))(JsonConvert.SerializeObject(InData, settings), settings)

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InDataInizializza.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InDataInizializza.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InDataInizializza.objP.objP_utenti)

            Dim Inizializza As New AgronicaCoreMapper.Inizializza_QdC

            r = Inizializza.Inizializza_QdC_NG(InDataInizializza.InData,
                                               objParametri_Super_Server,
                                                objParametri_Server,
                                                objParametri_Utenti)

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Default_DPI_QdC_NG(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.Disciplinare)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.Disciplinare)

        Try

            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim InDataDefault_DPI As CoreWS_Generic(Of LeggiDefault_DPI_QdC) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiDefault_DPI_QdC))(JsonConvert.SerializeObject(InData, settings), settings)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InDataDefault_DPI.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InDataDefault_DPI.objP.objP_utenti)

            Dim objUtility As New AgronicaCoreMapper.Utility

            r.RispostaOK = True

            r.RispostaStringa = objUtility.Default_DPI_QdC(InDataDefault_DPI.InData,
                                                            objParametri_Server,
                                                            objParametri_Utenti)

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Imposta_DoseConsentitaDiserbo(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of DoseConsentitaDiserbo)

        Dim r As New rispostaStandard(Of DoseConsentitaDiserbo)

        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri
        Dim objParametri_Super_Server As AgronicaCoreParametri

        Try

            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim InData_DoseConsentita As LeggiDoseConsentitaDiserbo = JsonConvert.DeserializeObject(Of LeggiDoseConsentitaDiserbo)(JsonConvert.SerializeObject(InData.InData, settings), settings)
            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)
            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)

            Dim objMapper As New AgronicaCoreMapper.Utility

            Dim DoseConsentita = objMapper.Imposta_DoseConsentitaDiserbo(objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti,
                                                                        InData_DoseConsentita.attivita,
                                                                        InData_DoseConsentita.dettaglioTrattamento,
                                                                        InData_DoseConsentita.fabbricato,
                                                                        InData_DoseConsentita.lotto,
                                                                        InData_DoseConsentita.avversitaGruppo)

            r.RispostaOK = True

            r.RispostaStringa = DoseConsentita

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaMenuRicette(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of MenuRicette))

        Dim r As New rispostaStandard(Of List(Of MenuRicette))

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try

            Dim List_Ricetta_Operazione_Voci_Menu As New List(Of MenuRicette)

            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            Dim leggiLingua As New Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            'Istanzio gli oggetti InData.
            Dim inDataRicette As LeggiMenuRicette = JsonConvert.DeserializeObject(Of LeggiMenuRicette)(JsonConvert.SerializeObject(InData.InData, settings), settings)

            Dim objRicette_Destinazioni As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
            Dim resArr As New JArray()

            If Not IsNothing(inDataRicette) AndAlso Not IsNothing(inDataRicette.codiciAttivita) Then

                Dim List_Ricetta_Cod As List(Of Integer) = inDataRicette.codiciAttivita.ConvertAll(Function(c) CInt(c.CodiceRicetta)).ToList().Distinct().ToList()

                If Not IsNothing(List_Ricetta_Cod) AndAlso List_Ricetta_Cod.Count = 1 Then

                    'Il ricetta cod deve essere lo stesso
                    Dim Ricetta_Cod As Integer = List_Ricetta_Cod(0)

                    Dim AggiungiNuovoDettaglio = New MenuRicette With {
                            .tipo = 3,
                            .des = Gias.AggiungiNuovaOpAllaRicetta,
                            .ribaltata = False,
                            .raccoglitore_cod = 0,
                            .codiciAttivita = New List(Of CodiciXOperazione) From {
                                New CodiciXOperazione With {
                                    .CodiceAttivita = "0",
                                    .CodiceOperazioneRicetta = "0",
                                    .CodiceRicetta = Ricetta_Cod,
                                    .Operazione = Nothing,
                                    .APP_RicettaOperazione_ID = "",
                                    .Associazione_PK = Nothing,
                                    .Centro_Aziendale = Nothing,
                                    .CodiceAttivitaVisita = "",
                                    .CodicePUA = 0
                                }
                            }
                        }

                    If Ricetta_Cod <> 0 Then

                        Dim list_ricetta_operazione As List(Of Integer) = inDataRicette.codiciAttivita.ConvertAll(Function(c) CInt(c.CodiceOperazioneRicetta)).Where(Function(c) c > 0).ToList()

                        Dim xFiltroAggiuntivo As String = ""

                        If Not IsNothing(list_ricetta_operazione) AndAlso list_ricetta_operazione.Count > 0 Then
                            xFiltroAggiuntivo = " Ricette_Operazioni.Ricetta_Operazione_Cod NOT IN (" & String.Join(",", list_ricetta_operazione) & ") "
                        End If

                        Dim dt = objRicette_Destinazioni.Leggi_Distinct_Impianti(Ricetta_Cod,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                           xFiltroAggiuntivo,
                                                            "Ricette_Operazioni.Validita_Inizio ",
                                                            objParametri_Server)

                        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                            Dim ricettaxagenda As New AgronicaCoreContabDAL.RicettexAgenda_R

                            For Each row In dt.Rows
                                Dim data = CDate(row("Validita_Inizio")).ToShortDateString
                                Dim dataLong = CStr(CDate(row("Validita_Inizio")))
                                Dim lav_cod = row("Lav_Cod")
                                Dim lav_des = row("Lav_Des")
                                Dim ricetta_operazione_cod = CInt(row("Ricetta_Operazione_Cod"))
                                Dim raccoglitore_cod = CInt(row("Raccoglitore_Cod"))
                                Dim invia_app = CInt(row("Invia_App")) = 1
                                Dim sa_cod = CInt(row("Sa_Cod"))
                                Dim piva = CStr(row("Piva"))
                                Dim index_Ricetta_Operazione_Multi = -1


                                If raccoglitore_cod <> 0 Then
                                    index_Ricetta_Operazione_Multi = List_Ricetta_Operazione_Voci_Menu.FindIndex(Function(l) l.raccoglitore_cod = raccoglitore_cod AndAlso
                                                                                                                         l.codiciAttivita.FindIndex(Function(c) c.Operazione.primaryKey.codice <> lav_cod AndAlso
                                                                                                                         c.Centro_Aziendale.primaryKey.codice <> sa_cod AndAlso c.Centro_Aziendale.primaryKey.partitaIva <> piva))
                                End If

                                If index_Ricetta_Operazione_Multi = -1 Then
                                    Dim dtRibaltata = ricettaxagenda.Leggi(Ricetta_Cod,
                                                                                 ricetta_operazione_cod,
                                                                                 0,
                                                                                 AGRODATAINIZIO,
                                                                                 AGRODATAFINE,
                                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                                 "",
                                                                                 "",
                                                                                 objParametri_Server)

                                    Dim ribaltata As Boolean = False

                                    If dtRibaltata.Rows.Count > 0 Then
                                        ribaltata = True
                                    End If

                                    Dim New_Voce_Menu = New MenuRicette With {
                                                                    .tipo = 3,
                                                                    .des = "",
                                                                    .data = dataLong,
                                                                    .ribaltata = CBool(ribaltata),
                                                                    .inviata_ad_app = invia_app,
                                                                    .raccoglitore_cod = raccoglitore_cod,
                                                                    .codiciAttivita = New List(Of CodiciXOperazione) From {
                                                                        New CodiciXOperazione With {
                                                                        .CodiceAttivita = "0",
                                                                        .CodiceOperazioneRicetta = ricetta_operazione_cod,
                                                                        .CodiceRicetta = Ricetta_Cod,
                                                                        .Operazione = New AgronicaCoreModelsSTD.attivita.Lavorazione(lav_cod, lav_des),
                                                                        .Centro_Aziendale = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(sa_cod, piva)),
                                                                        .APP_RicettaOperazione_ID = "",
                                                                        .Associazione_PK = Nothing,
                                                                        .CodiceAttivitaVisita = "",
                                                                        .CodicePUA = 0
                                                                        }
                                                                    }
                                                                }

                                    New_Voce_Menu.des = ComponiDescrizioneVoceMenuRicette(New_Voce_Menu)

                                    List_Ricetta_Operazione_Voci_Menu.Add(New_Voce_Menu)
                                Else

                                    'Concateno tutte le Ricette multi (con lo stesso raccoglitore) in un unica voce del menu

                                    List_Ricetta_Operazione_Voci_Menu(index_Ricetta_Operazione_Multi).codiciAttivita.Add(New CodiciXOperazione With {
                                                                        .CodiceAttivita = "0",
                                                                        .CodiceOperazioneRicetta = ricetta_operazione_cod,
                                                                        .CodiceRicetta = Ricetta_Cod,
                                                                        .Operazione = New AgronicaCoreModelsSTD.attivita.Lavorazione(lav_cod, lav_des),
                                                                        .Centro_Aziendale = New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(sa_cod, piva))
                                                                        })



                                    List_Ricetta_Operazione_Voci_Menu(index_Ricetta_Operazione_Multi).des = ComponiDescrizioneVoceMenuRicette(List_Ricetta_Operazione_Voci_Menu(index_Ricetta_Operazione_Multi))

                                End If


                            Next

                        End If

                    End If


                    'Prima mostro l'aggiunta del nuovo dettaglio poi le ricette che si possono modificare perchè non sono state ribaltate in agenda e poi quelle inviate all'app
                    If Not IsNothing(List_Ricetta_Operazione_Voci_Menu) AndAlso List_Ricetta_Operazione_Voci_Menu.Count > 0 Then

                        Dim List_Ricetta_Operazione_Voci_Menu_Non_Ribaltate_Non_Inviate = List_Ricetta_Operazione_Voci_Menu.Where(Function(v) v.ribaltata = False AndAlso v.inviata_ad_app = False).ToList()

                        Dim List_Ricetta_Operazione_Voci_Menu_Ribaltate_Inviate = List_Ricetta_Operazione_Voci_Menu.Where(Function(v) v.ribaltata = True AndAlso v.inviata_ad_app = True).ToList()

                        Dim List_Ricetta_Operazione_Voci_Menu_Ribaltate = List_Ricetta_Operazione_Voci_Menu.Where(Function(v) v.ribaltata = True AndAlso v.inviata_ad_app = False).ToList()

                        Dim List_Ricetta_Operazione_Voci_Menu_Inviate = List_Ricetta_Operazione_Voci_Menu.Where(Function(v) v.inviata_ad_app = True AndAlso v.ribaltata = False).ToList()

                        List_Ricetta_Operazione_Voci_Menu = New List(Of MenuRicette)

                        If Not IsNothing(List_Ricetta_Operazione_Voci_Menu_Non_Ribaltate_Non_Inviate) AndAlso List_Ricetta_Operazione_Voci_Menu_Non_Ribaltate_Non_Inviate.Count > 0 Then
                            For Each v As MenuRicette In List_Ricetta_Operazione_Voci_Menu_Non_Ribaltate_Non_Inviate
                                List_Ricetta_Operazione_Voci_Menu.Add(v)
                            Next
                        End If

                        If Not IsNothing(List_Ricetta_Operazione_Voci_Menu_Ribaltate_Inviate) AndAlso List_Ricetta_Operazione_Voci_Menu_Ribaltate_Inviate.Count > 0 Then
                            For Each v As MenuRicette In List_Ricetta_Operazione_Voci_Menu_Ribaltate_Inviate
                                List_Ricetta_Operazione_Voci_Menu.Add(v)
                            Next
                        End If

                        If Not IsNothing(List_Ricetta_Operazione_Voci_Menu_Ribaltate) AndAlso List_Ricetta_Operazione_Voci_Menu_Ribaltate.Count > 0 Then
                            For Each v As MenuRicette In List_Ricetta_Operazione_Voci_Menu_Ribaltate
                                List_Ricetta_Operazione_Voci_Menu.Add(v)
                            Next
                        End If

                        If Not IsNothing(List_Ricetta_Operazione_Voci_Menu_Inviate) AndAlso List_Ricetta_Operazione_Voci_Menu_Inviate.Count > 0 Then
                            For Each v As MenuRicette In List_Ricetta_Operazione_Voci_Menu_Inviate
                                List_Ricetta_Operazione_Voci_Menu.Add(v)
                            Next
                        End If

                    End If

                    List_Ricetta_Operazione_Voci_Menu.Insert(0, AggiungiNuovoDettaglio)

                End If

            End If



            r.RispostaStringa = List_Ricetta_Operazione_Voci_Menu
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    Private Shared Function ComponiDescrizioneVoceMenuRicette(Voce_Menu As MenuRicette) As String

        Dim des = ""

        Dim List_Lav_Des As List(Of String) = Voce_Menu.codiciAttivita.ConvertAll(Function(c) c.Operazione.descrizione).ToList().Distinct().ToList()

        Dim del_str = Gias.del

        If Voce_Menu.ribaltata = False AndAlso Voce_Menu.inviata_ad_app = False Then

            Dim modifica_str = AgronicaControlliGIS.My.Resources.AgronicaControlliGIS.Modifica

            des = CStr(modifica_str & " " & String.Join(",", List_Lav_Des) & " " & del_str & " " & Voce_Menu.data)

        End If

        If Voce_Menu.ribaltata = True AndAlso Voce_Menu.inviata_ad_app = True Then

            Dim salvata_in_agenda_e_inviata_ad_app_str = Gias.SalvataInAgendaEInviataAllAPP

            des = CStr(String.Join(",", List_Lav_Des) & " " & del_str & " " & Voce_Menu.data & " (" & salvata_in_agenda_e_inviata_ad_app_str & ")")

        End If

        If Voce_Menu.ribaltata = True AndAlso Voce_Menu.inviata_ad_app = False Then

            Dim salvata_in_agenda_str = Gias.SalvataInAgenda

            des = CStr(String.Join(",", List_Lav_Des) & " " & del_str & " " & Voce_Menu.data & " (" & salvata_in_agenda_str & ")")
        End If

        If Voce_Menu.ribaltata = False AndAlso Voce_Menu.inviata_ad_app = True Then

            Dim inviata_ad_app_str = Gias.InviataAllAPP

            des = CStr(String.Join(",", List_Lav_Des) & " " & del_str & " " & Voce_Menu.data & " (" & inviata_ad_app_str & ")")
        End If

        Return des
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OperazioneAgenda(Piva As String, listaIdAgenda As String, ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim filtoAggiuntivo = ""
        Dim strDTMovDettagliRif As String = ""

        Try

            If listaIdAgenda <> "" Then
                filtoAggiuntivo = "Agenda.Id_Agenda in (" + listaIdAgenda + ")"

                Dim objMovDettagliRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                Dim dtMovDettagliRif = objMovDettagliRif.LeggiPerAgenda(Piva,
                                                               0,
                                                               0,
                                                               0,
                                                               "",
                                                               filtoAggiuntivo,
                                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                               objParametri_Server)

                dtMovDettagliRif.Columns.Add("EsistonoCostiCollegatiCDG", GetType(Boolean))

                For Each row As DataRow In dtMovDettagliRif.Rows
                    If row("Lav_Cod_Rif") = LAVCOD_COSTI_CDG Then
                        'Messaggi.AgroMsgBuonFine("NB: Esistono costi collegati a questa operazione.", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                        row("EsistonoCostiCollegatiCDG") = False
                    Else
                        row("EsistonoCostiCollegatiCDG") = True
                    End If
                Next

                strDTMovDettagliRif = JsonConvert.SerializeObject(dtMovDettagliRif)
            Else
                strDTMovDettagliRif = "[]"
            End If

            r.RispostaOK = True
            r.RispostaStringa = strDTMovDettagliRif
        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function OperazioneAgenda_NG(InData As CoreWS_Generic(Of OperazioneAgenda)) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim filtoAggiuntivo = ""
        Dim strDTMovDettagliRif As String = ""

        Try

            If InData.InData.listaIdAgenda <> "" Then
                filtoAggiuntivo = "Agenda.Id_Agenda in (" + InData.InData.listaIdAgenda + ")"

                Dim objMovDettagliRif = New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                Dim dtMovDettagliRif = objMovDettagliRif.LeggiPerAgenda(InData.InData.Piva,
                                                               0,
                                                               0,
                                                               0,
                                                               "",
                                                               filtoAggiuntivo,
                                                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                               objParametri_Server)

                dtMovDettagliRif.Columns.Add("EsistonoCostiCollegatiCDG", GetType(Boolean))

                For Each row As DataRow In dtMovDettagliRif.Rows
                    If row("Lav_Cod_Rif") = LAVCOD_COSTI_CDG Then
                        'Messaggi.AgroMsgBuonFine("NB: Esistono costi collegati a questa operazione.", Page, , CType(Ricerca.FindControlIterative(Page.Master, "UpdatePanelToolBar"), UpdatePanel))
                        row("EsistonoCostiCollegatiCDG") = False
                    Else
                        row("EsistonoCostiCollegatiCDG") = True
                    End If
                Next

                strDTMovDettagliRif = JsonConvert.SerializeObject(dtMovDettagliRif)
            Else
                strDTMovDettagliRif = "[]"
            End If

            r.RispostaOK = True
            r.RispostaStringa = strDTMovDettagliRif
        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaGrigliaOperazioni(filtroAggiuntivo As String, ByVal objP_server As String) As RispostaStandard
        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try
            Dim dtGrigliaOperazioni As DataTable
            Dim ObjOperazioni_R As New AgronicaCoreAnagrafeDAL.Operazioni_R

            dtGrigliaOperazioni = ObjOperazioni_R.Leggi(0,
                            0,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            filtroAggiuntivo,
                            "",
                            objParametri_Server)


            r.RispostaStringa = JsonConvert.SerializeObject(dtGrigliaOperazioni)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaGrigliaOperazioni_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))
        Dim filtroAggiuntivo As String = InData.InData
        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

        Try
            Dim dtGrigliaOperazioni As DataTable
            Dim ObjOperazioni_R As New AgronicaCoreAnagrafeDAL.Operazioni_R

            dtGrigliaOperazioni = ObjOperazioni_R.Leggi(0,
                            0,
                            enumSelezioneVariabile.Selezione_TabellaCompleta,
                            filtroAggiuntivo,
                            "",
                            objParametri_Server)


            r.RispostaStringa = JsonConvert.SerializeObject(dtGrigliaOperazioni)
            r.RispostaOK = True

        Catch ex As Exception
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ControllaMassimali(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of ControllaMassimali_QdC)
        Dim r As New rispostaStandard(Of ControllaMassimali_QdC)

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            'Istanzio gli oggetti InData.
            Dim strControlloInserimentoDoseProdotto As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataControlloInserimentoDoseProdotto As Controllo_Inserimento_Dose_Prodotto = JsonConvert.DeserializeObject(Of Controllo_Inserimento_Dose_Prodotto)(strControlloInserimentoDoseProdotto,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            Dim Qta_Ha_Distribuibile_Prodotto As Decimal = 1000000
            Dim Qta_Ha_Distribuibile_Prodotto_Ricette As Decimal = 1000000


            Dim map As New AgronicaCoreMapper.CheckAttivita
            Dim errori As New Dictionary(Of TipoErrore, List(Of String))
            Dim gridDosiManaged = map.getGridDosiManaged(inDataControlloInserimentoDoseProdotto)

            map.ClearErrori(errori)

            map.CheckMassimali(ConsideraProdottoInInserimento:=False, inDataControlloInserimentoDoseProdotto, gridDosiManaged, AgronicaCoreMapper.Utility.GetVegCodFromUtilizzoTerreno(inDataControlloInserimentoDoseProdotto.utilizzoTerreno), Qta_Ha_Distribuibile_Prodotto, Qta_Ha_Distribuibile_Prodotto_Ricette,
                                                      errori, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)


            r.RispostaOK = True
            r.RispostaStringa = New ControllaMassimali_QdC With {
                .Qta_Ha_Distribuibile_Prodotto = Qta_Ha_Distribuibile_Prodotto,
                .Qta_Ha_Distribuibile_Prodotto_Ricette = Qta_Ha_Distribuibile_Prodotto_Ricette
            }

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = InData.InData
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function GestioneFlagRicettaInviaApp(InData As CoreWS_Generic(Of ModificaListaRicetteQdC)) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try
            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            Dim listaRicette As List(Of AgronicaCoreDTOStd.InData.Agenda.APP_Ricette_Operazioni) = InData.InData.listaRicette

            Dim mapper As New AgronicaCoreMapper.RicettaToAttivita
            r.ErroriGias = mapper.controlloMagazziniRicettexApp_Menu(listaRicette, objParametri_Utenti, objParametri_Server, objParametri_Super_Server)

            Dim objRicette As New AgronicaCoreContabBIZ.Ricette_W
            r.RispostaOK = objRicette.InviaRicettaApp(listaRicette, 1, objParametri_Server)


        Catch ex As Exception

            r.RispostaOK = False
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function


#Region "Modifica Multipla Attivita"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ModificaMultiplaListaAttivita(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

        Try
            Dim iData As CoreWS_Generic(Of ModificaMultipla_Attivita) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ModificaMultipla_Attivita))(JsonConvert.SerializeObject(InData))
            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim lista_Errori As New List(Of ErroreGias)
            Dim objModifica As New AgronicaCoreMapper.ModificaMultipla_Attivita

            Dim Tipo_Modifica = iData.InData.Tipo_Modifica
            Dim lista_Attivita = iData.InData.Attivita_list
            Dim lista_Risorse = iData.InData.Risorsa_list
            Dim Magazzino = iData.InData.Magazzino
            Dim Elimina_Precedenti = iData.InData.Elimina_Precedenti

            objParametri_Server = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            '===================================
            '   MODIFICA MULTIPLA 
            '-----------------------------------
            'scrittura di tutte le attività in modo transazionale con gestione del raccoglitore
            lista_Errori = objModifica.ModificaMultipla_ListaAttivita(Tipo_Modifica, lista_Attivita, lista_Risorse, Magazzino, Elimina_Precedenti,
                                                                      objParametri_Server, objParametri_Utenti)

            If lista_Errori IsNot Nothing AndAlso 'Ritorno un errore, solo per i messaggi di tipo Bloccante
                lista_Errori.FindAll(Function(c) (c.severity = ErroreGias_Severity.Bloccante)).Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = JsonConvert.SerializeObject(iData.InData)
                r.ErroriGias = lista_Errori
                Return r
            End If

            'Nella lista di errori inserisco anche il messaggio di conferma salvataggio, con le eventuali operazioni non salvate
            r.ErroriGias = lista_Errori
            r.RispostaStringa = JsonConvert.SerializeObject(lista_Errori)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(InData.InData)
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function
#End Region

#Region "Misure per Avverista"

    <WebMethod>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMisurePerAvversitaAnagraficheExtended(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of ListaMisurePerAvversitaAnagraficaExtended)

        Dim r As New rispostaStandard(Of ListaMisurePerAvversitaAnagraficaExtended)
        Dim objParametri_Server As AgronicaCoreParametri

        Try
            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)

            Dim misure As New ListaMisurePerAvversitaAnagraficaExtended
            Dim objRead As New AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_Anagrafiche_R
            Dim listaValori = objRead.LeggiMisure(objParametri_Server)

            If listaValori Is Nothing Then
                Throw New Exception("Errore nella lettura delle misure per avversità.")
            End If

            misure.ListaMisure = listaValori

            r.RispostaOK = True
            r.RispostaStringa = misure

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMisurePerAvversitaAnagrafiche(InData As Object) As rispostaStandard(Of ListaMisurePerAvversitaAnagrafica)

        Dim resp As New rispostaStandard(Of ListaMisurePerAvversitaAnagrafica)

        Try
            Dim objParametri = DeserializzaInData(Of LeggiMisuraPerAvversitaAnagrafica_In)(InData)

            Dim DPI_FlagPrivatoPubblico As Integer = 0

            Dim DPI_Cod As Integer = 0

            If Not IsNothing(objParametri.InData.disciplinare) Then
                DPI_FlagPrivatoPubblico = objParametri.InData.disciplinare.disciplinarePubblicoPrivato

                DPI_Cod = objParametri.InData.disciplinare.codice
            End If

            Dim misure As New ListaMisurePerAvversitaAnagrafica

            Dim objRead As New AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_Anagrafiche_R

            Dim listaValori = objRead.LeggiMisureDaCodice(objParametri.InData.codice, DPI_FlagPrivatoPubblico, DPI_Cod, objParametri.Server)

            If listaValori Is Nothing Then
                Throw New Exception("Errore nella lettura delle misure per avversità.")
            End If

            misure.ListaMisure = listaValori

            resp.RispostaOK = True
            resp.RispostaStringa = misure

        Catch ex As Exception

            resp.RispostaOK = False
            resp.Errore = ex.Message

        End Try

        Return resp
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function MisurePerAvversitaAnagrafiche(ByVal InData As Object) As RispostaStandard

        Dim resp As New RispostaStandard

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim objParametri = DeserializzaInData(Of MisuraPerAvversitaAnagrafica_In)(InData)

        Try
            Dim xWrite As New AgronicaCoreMetaSchemaBIZ.MisuraXAvversita_Anagrafiche_W

            resp.RispostaOK = True

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri.Server)

            If objParametri.InData.MisureInsert IsNot Nothing AndAlso objParametri.InData.MisureInsert.Count > 0 Then
                resp.RispostaOK = xWrite.InserisciMisure(objParametri.InData.MisureInsert, objParametri.Server)
            End If

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nell'inserimento della lista misure.")
            End If

            If objParametri.InData.MisureUpdate IsNot Nothing AndAlso objParametri.InData.MisureUpdate.Count > 0 Then
                resp.RispostaOK = xWrite.AggiornaMisure(objParametri.InData.MisureUpdate, objParametri.Server)
            End If

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nella modifica della lista misure.")
            End If

            If objParametri.InData.MisureDelete IsNot Nothing AndAlso objParametri.InData.MisureDelete.Count > 0 Then
                resp.RispostaOK = xWrite.EliminaMisure(objParametri.InData.MisureDelete, objParametri.Server)
            End If

            If Not resp.RispostaOK Then
                Throw New Exception("Errore nell'eliminazione della lista misure.")
            End If

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri.Server)

            resp.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            If Not objParametri.Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri.Server)
            End If

            resp.RispostaOK = False
            resp.Errore = ex.Message
        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri.Server)
        End Try

        Return resp
    End Function
#End Region

#Region "Misure Indici Maturita"

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMisureIndiciMaturitaAnagrafiche(InData As CoreWS_Generic(Of LeggiMisureAvversita))
        Dim r As New RispostaStandard
        Dim d = InData.InData
        Try
            Dim params As New ObjParams With {
                .ObjParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti),
                .ObjParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server),
                .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            }
            Dim reader As New AgronicaCoreMetaSchemaBIZ.MisurexIndiciMaturita_Anagrafiche
            Dim list = reader.GetValuesListFor(
                d.avvCod, d.udmCod,
                d.vegCod, params
            )
            r.RispostaStringa = JsonConvert.SerializeObject(list)
            r.RispostaOK = True
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMisureIndiciMaturitaAnagrafiche2(InData As CoreWS_Generic(Of LeggiMisuraPerIndiciMaturitaAnagrafiche_IN)) As rispostaStandard(Of ListaMisuraPerIndiciMaturitaAnagrafica)
        Dim resp As New rispostaStandard(Of ListaMisuraPerIndiciMaturitaAnagrafica)

        Dim objParametri_Server As AgronicaCoreParametri

        Try
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim leggiMisuraPerIndiciMaturitaAnagraficheIN = InData.InData
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)

            Dim anagrafiche As New ListaMisuraPerIndiciMaturitaAnagrafica

            Dim objRead As New AgronicaCoreMetaSchemaBIZ.MisurexIndiciMaturita_Anagrafiche

            Dim listaValori = objRead.LeggiAnagrafiche(leggiMisuraPerIndiciMaturitaAnagraficheIN.Ind_Mat_Cod,
                                                       leggiMisuraPerIndiciMaturitaAnagraficheIN.Udm_Cod, leggiMisuraPerIndiciMaturitaAnagraficheIN.Anag_Cod,
                                                       objParametri_Server)

            If listaValori Is Nothing Then
                Throw New Exception("Errore nella lettura delle misure per indici maturita")
            End If

            anagrafiche.ListaMisure = listaValori
            resp.RispostaOK = True
            resp.RispostaStringa = anagrafiche

        Catch ex As Exception

            resp.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            resp.RispostaOK = False

        End Try
        Return resp
    End Function

    <WebMethod>
    <Script.Services.ScriptMethod()>
    Public Function MisurePerIndiciMaturita(ByVal InData As CoreWS_Generic(Of MisuraPerIndiciMaturita_In)) As RispostaStandard

        Dim response As New RispostaStandard

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
        Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)
        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP.objP_server)
        Dim misuraPerIndiciMaturitaIn = InData.InData

        Try
            Dim writeDal As New AgronicaCoreMetaSchemaBIZ.MisurexIndiciMaturita_Anagrafiche

            response.RispostaOK = True

            ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                        FlagTransazioneLocale,
                                                                        objParametri_Server)

            If misuraPerIndiciMaturitaIn.AnagraficaInsert IsNot Nothing AndAlso misuraPerIndiciMaturitaIn.AnagraficaInsert.Count > 0 Then
                response.RispostaOK = writeDal.InserisciAnagrafiche(misuraPerIndiciMaturitaIn.AnagraficaInsert, objParametri_Server)
            End If

            If Not response.RispostaOK Then
                Throw New Exception("Errore nell'inserimento della lista anagrafiche.")
            End If

            If misuraPerIndiciMaturitaIn.AnagraficaUpdate IsNot Nothing AndAlso misuraPerIndiciMaturitaIn.AnagraficaUpdate.Count > 0 Then
                response.RispostaOK = writeDal.AggiornaAnagrafiche(misuraPerIndiciMaturitaIn.AnagraficaUpdate, objParametri_Server)
            End If

            If Not response.RispostaOK Then
                Throw New Exception("Errore nella modifica della lista anagrafiche.")
            End If

            If misuraPerIndiciMaturitaIn.AnagraficaDelete IsNot Nothing AndAlso misuraPerIndiciMaturitaIn.AnagraficaDelete.Count > 0 Then
                response.RispostaOK = writeDal.EliminaAnagrafiche(misuraPerIndiciMaturitaIn.AnagraficaDelete, objParametri_Server)
            End If

            If Not response.RispostaOK Then
                Throw New Exception("Errore nell'eliminazione della lista anagrafiche.")
            End If

            ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            response.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception
            If Not objParametri_Server.objTransazione Is Nothing Then
                ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            response.RispostaOK = False
            response.Errore = ex.Message
        Finally
            ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return response

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiMisureIndiciMaturita(InData As CoreWS_Generic(Of Integer))
        Dim resp As New rispostaStandard(Of ListaMisuraPerIndiciMaturita)
        Dim objParametri_Server As AgronicaCoreParametri

        Try
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim indMatCod = InData.InData
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)

            Dim misure As New ListaMisuraPerIndiciMaturita

            Dim objRead As New AgronicaCoreMetaSchemaBIZ.MisureXIndiciMaturita
            Dim listaValori = objRead.LeggiMisureIndiciMaturita(indMatCod, objParametri_Server)

            If listaValori Is Nothing Then
                Throw New Exception("Errore nella lettura delle misure per indici maturita")
            End If

            misure.ListaMisure = listaValori
            resp.RispostaStringa = misure
            resp.RispostaOK = True

        Catch ex As Exception

            resp.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            resp.RispostaOK = False

        End Try
        Return resp

    End Function

#End Region
    Private Class ObjParametri(Of T)

        Public Super_Server As AgronicaCoreParametri
        Public Server As AgronicaCoreParametri
        Public Utenti As AgronicaCoreParametri
        Public InData As T

    End Class

    Private Function DeserializzaInData(Of T)(ByVal InData As Object) As ObjParametri(Of T)

        Dim JsonSettings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objInData As CoreWS_Generic(Of T) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of T))(JsonConvert.SerializeObject(InData), JsonSettings)

        Dim objParametri As New ObjParametri(Of T)

        If objInData.objP.objP_super_server IsNot Nothing AndAlso Not objInData.objP.objP_super_server.Equals("") Then
            objParametri.Super_Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_super_server)
        End If
        If objInData.objP.objP_server IsNot Nothing AndAlso Not objInData.objP.objP_server.Equals("") Then
            objParametri.Server = Utility.convertStringtoOBJparametri(objInData.objP.objP_server)
        End If
        If objInData.objP.objP_utenti IsNot Nothing AndAlso Not objInData.objP.objP_utenti.Equals("") Then
            objParametri.Utenti = Utility.convertStringtoOBJparametri(objInData.objP.objP_utenti)
        End If

        objParametri.InData = objInData.InData

        Return objParametri

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Recupera_Pua(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.Pua)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.Pua)

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim pua As Pua = Nothing

        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            'Istanzio gli oggetti InData.
            Dim strxPua As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataxPua As LeggiPUA = JsonConvert.DeserializeObject(Of LeggiPUA)(strxPua,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            Dim objUtility As New AgronicaCoreMapper.Utility

            Dim Piva As String = ""

            If Not IsNothing(inDataxPua.impresa) Then
                Piva = inDataxPua.impresa.partitaIva
            End If

            Dim Lav_Cod As Integer = 0

            If Not IsNothing(inDataxPua.operazioni) AndAlso inDataxPua.operazioni.FindIndex(Function(o) CInt(o.primaryKey.codice) = LAVCOD_DISTRIBUZIONE_AMMENDANTI) > -1 Then
                Lav_Cod = LAVCOD_DISTRIBUZIONE_AMMENDANTI
            End If

            pua = objUtility.Recupera_Pua_Valido_Alla_Data(Piva,
                                                             inDataxPua.data,
                                                             Lav_Cod,
                                                            inDataxPua.tipo_Attivita,
                                                            inDataxPua.tipo_Ricetta,
                                                            inDataxPua.stato,
                                                            inDataxPua.disciplinare,
                                                            objParametri_Server)

            r.RispostaOK = True

            r.RispostaStringa = pua

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaDisponibilitaAttualeFertilizzante(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Decimal)

        Dim r As New rispostaStandard(Of Decimal)

        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim FertilizzanteUsato As Decimal = 0

        Try

            'Ottengo AgronicaCoreParametri
            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)

            'Istanzio gli oggetti InData.
            Dim strxLeggiDisponibilita As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataxLeggiDisponibilita As LeggiDisponibilitaAttualeFertilizzante = JsonConvert.DeserializeObject(Of LeggiDisponibilitaAttualeFertilizzante)(strxLeggiDisponibilita,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            Dim objApporti As New AgronicaCorePUA_BIZ.PUA_Apporti_BIZ

            Dim Fer_Cod As Integer = 0

            Dim Eff_Cod As Integer = 0

            If Not IsNothing(inDataxLeggiDisponibilita.dettaglioFertilizzazione) Then

                If Not IsNothing(inDataxLeggiDisponibilita.dettaglioFertilizzazione.prodotto) Then
                    Fer_Cod = inDataxLeggiDisponibilita.dettaglioFertilizzazione.prodotto.codice
                End If

                If Not IsNothing(inDataxLeggiDisponibilita.dettaglioFertilizzazione.effluente) Then
                    Eff_Cod = inDataxLeggiDisponibilita.dettaglioFertilizzazione.effluente.codice
                End If

            End If

            Dim Pua_Cod As Integer = 0

            Dim Regolamento_Cod_Pua As Integer = 0

            If Not IsNothing(inDataxLeggiDisponibilita.pua) Then

                Pua_Cod = inDataxLeggiDisponibilita.pua.codice

                If Not IsNothing(inDataxLeggiDisponibilita.pua.disciplinare) Then
                    If Not IsNothing(inDataxLeggiDisponibilita.pua.disciplinare.regolamentoConcimazione) Then
                        Regolamento_Cod_Pua = inDataxLeggiDisponibilita.pua.disciplinare.regolamentoConcimazione.codice
                    Else
                        Regolamento_Cod_Pua = inDataxLeggiDisponibilita.pua.disciplinare.codice
                    End If
                End If

            End If


            Dim Data As DateTime = inDataxLeggiDisponibilita.data

            FertilizzanteUsato = objApporti.CaricaDisponibilitaAttuale(Fer_Cod, Eff_Cod, Pua_Cod, Regolamento_Cod_Pua, Data, objParametri_Server)

            r.RispostaOK = True

            r.RispostaStringa = FertilizzanteUsato

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiSpecieVegetaliQdC(InData As CoreWS_Generic(Of InData.Agenda.LeggiSpecieQdC)) As rispostaStandard(Of List(Of utilizzi.UtilizzoTerreno))

        Dim r As New rispostaStandard(Of List(Of utilizzi.UtilizzoTerreno))

        If InData.objP.objP_super_server Is Nothing Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim LeggiSpecieQdC As InData.Agenda.LeggiSpecieQdC = InData.InData

            Dim Specie_GiacenzeMagazzino As New List(Of utilizzi.UtilizzoTerreno)

            Dim Specie_ImpiantiAnagrafici As New List(Of utilizzi.UtilizzoTerreno)

            Dim Piva As String = ""

            Dim Sa_Cod As Integer = 0

            If Not IsNothing(InData.InData.centroAziendale) AndAlso
                Not IsNothing(InData.InData.centroAziendale.primaryKey) Then

                Piva = LeggiSpecieQdC.centroAziendale.primaryKey.partitaIva
                Sa_Cod = LeggiSpecieQdC.centroAziendale.primaryKey.codice

            ElseIf Not IsNothing(LeggiSpecieQdC.impresa) Then

                Piva = LeggiSpecieQdC.impresa.partitaIva

            End If

            'Se l'operazione è TRATTAMENTO_POST_RACCOLTA o CONCIA_SEME, leggo le specie dai magazzini
            'altrimenti leggo le specie disponibili dagli impianti anagrafici
            Dim LeggiSpecie_Da_GiacenzeMagazzino As Boolean = False

            Dim elem_cod_List As New List(Of Integer)

            Dim LeggiSpecie_Da_ImpiantiAnagrafici As Boolean = False

            If Not IsNothing(LeggiSpecieQdC.lavorazioni) AndAlso
                LeggiSpecieQdC.lavorazioni.Count > 0 AndAlso
                LeggiSpecieQdC.lavorazioni.FindIndex(Function(l) CInt(l.primaryKey.codice) = LAVCOD_TRATTAMENTO_POST_RACCOLTA OrElse
                                                                CInt(l.primaryKey.codice) = LAVCOD_CONCIA_SEME) > -1 Then

                If LeggiSpecieQdC.lavorazioni.FindIndex(Function(l) CInt(l.primaryKey.codice) = LAVCOD_TRATTAMENTO_POST_RACCOLTA) > -1 Then
                    elem_cod_List.Add(TRASFORMATI_VEGETALI)
                End If

                If LeggiSpecieQdC.lavorazioni.FindIndex(Function(l) CInt(l.primaryKey.codice) = LAVCOD_CONCIA_SEME) > -1 Then
                    elem_cod_List.Add(SEMENTI)
                End If

                LeggiSpecie_Da_GiacenzeMagazzino = True

                If LeggiSpecieQdC.lavorazioni.Count > 1 Then
                    LeggiSpecie_Da_ImpiantiAnagrafici = True
                End If

            Else
                LeggiSpecie_Da_ImpiantiAnagrafici = True
            End If

            If LeggiSpecie_Da_GiacenzeMagazzino Then

                Dim objContabDAL As New AgronicaCoreContabDAL.Giacenze_R
                Specie_GiacenzeMagazzino = objContabDAL.LeggiSpecieMagazzino_QdC(Piva,
                                                                                     Sa_Cod,
                                                                                     LeggiSpecieQdC.data,
                                                                                     elem_cod_List,
                                                                                     objParametri_Server,
                                                                                     objParametri_Utenti)

            End If

            If LeggiSpecie_Da_ImpiantiAnagrafici Then

                Dim objReg_Impianto As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R

                If LeggiSpecieQdC.soloAttiviAllaData Then
                    'Chiamata da dentro l'operazione, mostro solo le specie degli impianti attivi alla data
                    Specie_ImpiantiAnagrafici = objReg_Impianto.Leggi_SpecieVegetali_Attive_Impianti(Piva,
                                                                                           Sa_Cod,
                                                                                           0,
                                                                                           LeggiSpecieQdC.data,
                                                                                           True,
                                                                                           LeggiSpecieQdC.consideraTerrenoNudo,
                                                                                           True,
                                                                                           objParametri_Server,
                                                                                           objParametri_Utenti,
                                                                                           LeggiSpecieQdC.soloAttiviAllaData)

                Else
                    Specie_ImpiantiAnagrafici = objReg_Impianto.Leggi_SpecieVegetali_Impianti_NoFiltroData(Piva,
                                                                                                 Sa_Cod,
                                                                                                 True,
                                                                                                 LeggiSpecieQdC.consideraTerrenoNudo,
                                                                                                 True,
                                                                                                 objParametri_Server,
                                                                                                 objParametri_Utenti)
                End If
            End If


            Dim unione As List(Of utilizzi.UtilizzoTerreno) = Specie_GiacenzeMagazzino.Union(Specie_ImpiantiAnagrafici).Distinct().ToList()
            Dim utilizziTerreno As List(Of utilizzi.UtilizzoTerreno) = unione.Where(Function(s) s.codice <> "0").ToList()
            Dim specie As List(Of utilizzi.UtilizzoTerreno) = unione.Where(Function(s) s.codice = "0" AndAlso s.classType = ClassType.Varieta).GroupBy(Function(x) CType(x, utilizzi.Varieta).specie.codice).Select(Function(g) g.First()).ToList()

            Dim unioneDistinct As List(Of utilizzi.UtilizzoTerreno) = specie.Union(utilizziTerreno).ToList()

            r.RispostaOK = True
            r.RispostaStringa = unioneDistinct

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Modalita_Applicazione_QdC(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of BaseCodeDescr))

        Dim r As New rispostaStandard(Of List(Of BaseCodeDescr))

        Dim objParametri_Server As AgronicaCoreParametri
        Dim List_Modalita As New List(Of BaseCodeDescr)

        Try

            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)

            'Istanzio gli oggetti InData.
            Dim strxLeggiModalita As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataxLeggiModalita As Leggi_Modalita_Applicazione = JsonConvert.DeserializeObject(Of Leggi_Modalita_Applicazione)(strxLeggiModalita,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            If Not IsNothing(inDataxLeggiModalita.Operazione) AndAlso inDataxLeggiModalita.Operazione.getCodice() > 0 Then

                Dim objVarieDAL As New AgronicaCoreVarieDAL.Modalita_Applicazione

                Dim DT = objVarieDAL.Lettura_Scalare_Modalita_Applicazione(inDataxLeggiModalita.Operazione.getCodice(), objParametri_Server)

                If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                    For Each Dr In DT.Rows
                        List_Modalita.Add(New BaseCodeDescr(Dr("Codice"), Dr("Descrizione")))
                    Next
                End If

            End If


            r.RispostaOK = True

            r.RispostaStringa = List_Modalita

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Controlla_Giacenza_Fertilizzanti(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri
        Dim objParametri_Super_Server As AgronicaCoreParametri


        Try

            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)
            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)

            'Istanzio gli oggetti InData.
            Dim strxControlla As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataxControlla As ScriviListaAttivita = JsonConvert.DeserializeObject(Of ScriviListaAttivita)(strxControlla,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})


            Dim listErroriGias = AgronicaCoreMapper.Utility.Controlla_Giacenza_Fertilizzanti(inDataxControlla.attivita_list, objParametri_Super_Server, objParametri_Server, objParametri_Utenti)

            If Not IsNothing(listErroriGias) AndAlso listErroriGias.Count > 0 Then
                r.RispostaOK = False

                r.RispostaStringa = ""

                r.ErroriGias = listErroriGias
            Else
                r.RispostaOK = True

                r.RispostaStringa = ""
            End If


        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r
    End Function



    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiConsigliIrrigazione(InData As CoreWS_Generic(Of LeggiConsigliIrrigazioni)) As rispostaStandard(Of List(Of Consiglio_Irrigazione))

        Dim r As New rispostaStandard(Of List(Of Consiglio_Irrigazione))

        If InData.objP.objP_super_server Is Nothing Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim params As LeggiConsigliIrrigazioni = InData.InData

            Dim piva As String = params.piva
            Dim sa_cod As Integer = params.sa_cod
            Dim appezza As Integer = params.appezza
            Dim id_reg As Integer = params.id_reg
            Dim progetto_cod As Integer = params.progetto_cod

            Dim DSS_Irrigazione As New AgronicaCoreMeteoDAL.DSS_Irrigazione_R
            Dim Data_Esecuzione_Inizio As Date = CostantiPersonalizzate.AGRODATAINIZIO
            Dim Data_Esecuzione_Fine As Date = params.data
            Dim Numero_Giorni_Precedenti As Integer = 15

            If Data_Esecuzione_Fine <> CostantiPersonalizzate.AGRODATAINIZIO AndAlso Data_Esecuzione_Fine <> CostantiPersonalizzate.AGRODATAFINE Then
                Data_Esecuzione_Inizio = Data_Esecuzione_Fine.AddDays(-Numero_Giorni_Precedenti)
            End If

            Dim dtConsigli = DSS_Irrigazione.Leggi_Precedenti_Con_Turni(
                piva, sa_cod, appezza, id_reg, progetto_cod,
                Data_Esecuzione_Inizio, Data_Esecuzione_Fine,
                "", "", objParametri_Server)

            Dim consigliIrrigazione As New List(Of Consiglio_Irrigazione)

            If dtConsigli IsNot Nothing AndAlso dtConsigli.Rows.Count > 0 Then
                For Each row In dtConsigli.Rows
                    Dim consiglioIrrigazione As New Consiglio_Irrigazione(CInt(row("ID_DSS_Irrigazione")), CStr(row("Descrizione_DSS_Irrigazione"))) With {
                        .descrizione = row("Descrizione_DSS_Irrigazione"),
                        .dataEsecuzione = row("Data_Esecuzione"),
                        .dataConsiglio = row("Data_Consiglio"),
                        .qtaAcqua = row("Qta_Acqua_DSS_Irrigazione"),
                        .unitaDiMisura = New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(row("Udm_Cod_DSS_Irrigazione")) With {
                                            .descrizione = row("Udm_Des_DSS_Irrigazione"),
                                            .simbolo = row("Udm_Sim_DSS_Irrigazione")
                                        },
                        .providerConsiglio = row("Provider_Consiglio"),
                        .modelloConsiglio = row("Modello_Consiglio"),
                        .minDataTurno = IIf(IsDBNull(row("Min_Data_Turno")), Nothing, row("Min_Data_Turno")),
                        .maxDataTurno = IIf(IsDBNull(row("Max_Data_Turno")), Nothing, row("Max_Data_Turno"))
                    }
                    consigliIrrigazione.Add(consiglioIrrigazione)
                Next
            End If

            r.RispostaStringa = consigliIrrigazione
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    Private Class IdentificativiAttivitaAppDati
        Public Property Id As String
        Public Property Versione As String
        Public Property Tipo As String
        Public Property GiaProcessato As Boolean

    End Class

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function LeggiAgendeEBrogliacciPerApp(InData As CoreWS_Generic(Of Object)) As ArchivioAttivitaCampagna

        Dim result As New ArchivioAttivitaCampagna
        Dim logger = New LogProvider
        Dim NomeRoutine As String = "Agenda.asmx/LeggiAgendeEBrogliacciPerApp()"

        Try
            If InData.objP.objP_super_server Is Nothing Then
                Return Nothing
            End If

            If InData.objP.objP_server = "" Then
                Return Nothing
            End If

            If InData.objP.objP_utenti = "" Then
                Return Nothing
            End If

            Dim settings As New JsonSerializerSettings()
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim inDataInit As CoreWS_Generic(Of LeggiAgenda) =
                JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAgenda))(JsonConvert.SerializeObject(InData, settings), settings)

            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(inDataInit.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(inDataInit.objP.objP_utenti)
            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(inDataInit.objP.objP_super_server)
            Dim params As LeggiAgenda = inDataInit.InData

            Dim objAppHelper As New AppHelper
            Dim dataRif = New Date(If(Today.Month < 11, Today.Year - 1, Today.Year), 11, 1)

            Dim logAgendaDal As New AgronicaLogAgenda_R
            Dim logBrogliacciDal As New AgronicaLogRicette_R

            If Not params.SoloImpiantiAttivi Then
                Dim esistonoModificheBrogliacci = logBrogliacciDal.EsistonoModificheDopoLaDataXBrogliacciApp(params.Piva, dataRif, params.DataUltimaSincro, objParametriServer)

                If (params.LeggiSoloBrogliacci) Then
                    If Not esistonoModificheBrogliacci Then
                        result.LetturaNonEseguita = True
                        Return result
                    End If
                Else
                    Dim esistonoModificheAgenda = logAgendaDal.EsistonoModificheDopoLaDataXAttivitaApp(params.Piva, dataRif, params.DataUltimaSincro, objParametriServer)

                    If Not esistonoModificheAgenda AndAlso Not esistonoModificheBrogliacci Then
                        result.LetturaNonEseguita = True
                        Return result
                    End If
                End If
            End If

            Dim ricette_dettagli_r As New Ricette_Dettagli_R

            If Not params.LeggiSoloBrogliacci Then

                Dim agendaReadDal = New Agenda_R
                Dim dtAgende = agendaReadDal.LeggiAgendePerApp(params.Piva, dataRif, params.DataUltimaSincro, params.SoloImpiantiAttivi, objParametriServer)

                Dim agronicaLogAgenda As New AgronicaLogAgenda_R
                Dim ricetteXAgenda_r As New RicettexAgenda_R

                Dim agendaHelper = New Agenda_Operazione_Helper()
                Dim agendaToActivity = New AgronicaCoreMapper.AgendaToAttivita()

                Dim listaFinaleAttivita As New List(Of List(Of AgronicaCoreModelsSTD.attivita.Attivita))

                If dtAgende IsNot Nothing AndAlso dtAgende.Rows.Count > 0 Then

                    Dim dictRaccoglitoreAgende As New Dictionary(Of Integer, IdentificativiAttivitaAppDati)
                    For Each row As DataRow In dtAgende.Rows
                        Dim racc = CInt(row("Raccoglitore_Cod"))
                        If racc > 0 AndAlso Not dictRaccoglitoreAgende.ContainsKey(racc) Then

                            dictRaccoglitoreAgende.Add(racc, New IdentificativiAttivitaAppDati())

                            Dim dtDatiAgendaPerExport = agronicaLogAgenda.LeggiDatiPerExportAttivitaConRaccoglitoreCod(racc, objParametriServer)
                            If dtDatiAgendaPerExport IsNot Nothing AndAlso dtDatiAgendaPerExport.Rows.Count > 0 Then

                                Dim id As String = dtDatiAgendaPerExport.Rows(0)("ID")
                                Dim versione As String = dtDatiAgendaPerExport.Rows(0)("Versione")
                                Dim tipo As String = dtDatiAgendaPerExport.Rows(0)("Tipo")

                                dictRaccoglitoreAgende(racc).Id = id
                                dictRaccoglitoreAgende(racc).Versione = versione
                                dictRaccoglitoreAgende(racc).Tipo = tipo
                            End If

                        End If
                    Next

                    For Each row As DataRow In dtAgende.Rows
                        Try
                            Dim racc = CInt(row("Raccoglitore_Cod"))
                            If racc = 0 Then
                                Dim idAgenda = CInt(row("Id_Agenda"))
                                Dim agenda = agendaHelper.Leggi(params.Piva, 0, idAgenda, 0, objParametriServer)
                                Dim attivita = agendaToActivity.AgendaSuAttivita(agenda, False, objParametriSuperServer, objParametriServer, objParametriUtenti, leggiDisciplinari:=False)

                                Dim guid = CStr(row("GuidRicetta"))
                                attivita.guid = guid
                                Dim versione = CStr(row("Versione"))
                                attivita.versione = versione
                                Dim tipo = CStr(row("Tipo"))
                                attivita.origineAttivita = tipo
                                attivita.inizio = New Date(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day,
                                                           attivita.oraInizio.Hour, attivita.oraInizio.Minute, 0, DateTimeKind.Local)

                                If tipo = "10" Then 'attività proveniente da app
                                    Dim ricettaOperazioneCod = CInt(row("CodiceRicetta"))
                                    AssegnaOreAMacchineEOperatori(attivita, ricettaOperazioneCod, objParametriServer, ricette_dettagli_r)
                                ElseIf tipo = "90" Then 'attività proveniente da demetra
                                    Dim dtRicetteXAgenda = ricetteXAgenda_r.LeggiRicettaOperazioneCodDaIdAgenda(idAgenda, objParametriServer)
                                    If dtRicetteXAgenda IsNot Nothing AndAlso dtRicetteXAgenda.Rows.Count > 0 Then
                                        Dim ricettaOperazioneCod = CInt(dtRicetteXAgenda.Rows(0)("Ricetta_Operazione_Cod"))
                                        AssegnaOreAMacchineEOperatori(attivita, ricettaOperazioneCod, objParametriServer, ricette_dettagli_r)
                                    End If
                                End If

                                attivita.stato = Stati.Eseguita
                                Dim codiceGiasPianificata As String = CStr(row("CodiceGiasPianificata"))

                                If Not String.IsNullOrEmpty(codiceGiasPianificata) Then
                                    attivita.riferimentoPianificata = codiceGiasPianificata
                                End If

                                Dim listaAttivita As New List(Of AgronicaCoreModelsSTD.attivita.Attivita) From {
                                    attivita
                                }

                                listaFinaleAttivita.Add(listaAttivita)
                            Else
                                If Not dictRaccoglitoreAgende(racc).GiaProcessato Then
                                    dictRaccoglitoreAgende(racc).GiaProcessato = True

                                    Dim listaAgendeDt = agendaReadDal.LeggiIdAgendaELavCodPerAttivitaConRaccoglitoreCod(racc, objParametriServer)
                                    If listaAgendeDt IsNot Nothing AndAlso listaAgendeDt.Rows.Count > 0 Then

                                        Dim attivitaNonSupportata = False
                                        For Each rowAgenda As DataRow In listaAgendeDt.Rows
                                            Dim lav_cod = CInt(rowAgenda("Lav_Cod"))
                                            If Not CostantiPersonalizzate.OPERAZIONI_GESTITE_APP_DEMETRA_LIST.Contains(lav_cod) Then
                                                attivitaNonSupportata = True
                                                Exit For
                                            End If
                                        Next

                                        If attivitaNonSupportata Then
                                            Continue For
                                        End If

                                        Dim listaAttivita As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)

                                        For Each rowAgenda As DataRow In listaAgendeDt.Rows
                                            Dim id_agenda = CInt(rowAgenda("Id_Agenda"))
                                            Dim agenda = agendaHelper.Leggi(params.Piva, 0, id_agenda, 0, objParametriServer)
                                            Dim attivita = agendaToActivity.AgendaSuAttivita(agenda, False, objParametriSuperServer, objParametriServer, objParametriUtenti, leggiDisciplinari:=False)

                                            attivita.guid = dictRaccoglitoreAgende(racc).Id
                                            attivita.versione = dictRaccoglitoreAgende(racc).Versione
                                            attivita.origineAttivita = dictRaccoglitoreAgende(racc).Tipo
                                            attivita.inizio = New Date(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day,
                                                                       attivita.oraInizio.Hour, attivita.oraInizio.Minute, 0, DateTimeKind.Local)

                                            If attivita.origineAttivita = "10" OrElse attivita.origineAttivita = "90" Then 'attività proveniente da app o da demetra
                                                Dim dtRicetteXAgenda = ricetteXAgenda_r.LeggiRicettaOperazioneCodDaIdAgenda(id_agenda, objParametriServer)
                                                If dtRicetteXAgenda IsNot Nothing AndAlso dtRicetteXAgenda.Rows.Count > 0 Then
                                                    Dim ricettaOperazioneCod = CInt(dtRicetteXAgenda.Rows(0)("Ricetta_Operazione_Cod"))
                                                    AssegnaOreAMacchineEOperatori(attivita, ricettaOperazioneCod, objParametriServer, ricette_dettagli_r)
                                                End If
                                            End If

                                            listaAttivita.Add(attivita)
                                        Next

                                        listaFinaleAttivita.Add(listaAttivita)

                                    End If
                                End If
                            End If

                        Catch ex As Exception
                            logger.Scrivi_LOG(objParametriServer, NomeRoutine, ex.Message)
                        End Try
                    Next

                    result.Attivita = listaFinaleAttivita
                End If
            End If

            Dim ricetteOperazioniDal As New Ricette_Operazioni_R

            Dim dtBrogliacci = ricetteOperazioniDal.LeggiBrogliacciEAttivitaPianificatePerApp(params.Piva, dataRif, params.DataUltimaSincro, params.SoloImpiantiAttivi, objParametriServer)

            Dim gefutils As New Gias_EF_Utility
            Dim efConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)

            Dim map As New AgronicaCoreMapper.RicettaToAttivita
            Dim listaFinaleBrogliacci As New List(Of List(Of AgronicaCoreModelsSTD.attivita.Attivita))
            Dim listaAttivitaPianificate As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)

            If dtBrogliacci IsNot Nothing AndAlso dtBrogliacci.Rows.Count > 0 Then

                Dim dictRaccoglitoreBrogliacci As New Dictionary(Of Integer, IdentificativiAttivitaAppDati)
                For Each row As DataRow In dtBrogliacci.Rows
                    Dim racc = CInt(row("Raccoglitore_Cod"))
                    If racc > 0 AndAlso Not dictRaccoglitoreBrogliacci.ContainsKey(racc) Then

                        dictRaccoglitoreBrogliacci.Add(racc, New IdentificativiAttivitaAppDati())

                        Dim id As String = CStr(row("GuidRicetta"))
                        Dim versione As String = CStr(row("Versione"))
                        Dim tipo As String = CStr(row("Tipo"))

                        dictRaccoglitoreBrogliacci(racc).Id = id
                        dictRaccoglitoreBrogliacci(racc).Versione = versione
                        dictRaccoglitoreBrogliacci(racc).Tipo = tipo
                    End If
                Next

                For Each row As DataRow In dtBrogliacci.Rows

                    Dim transOptions As New TransactionOptions With {.IsolationLevel = IsolationLevel.ReadCommitted}

                    Using ts As New TransactionScope(TransactionScopeOption.Required, transOptions)
                        Using giasContext As New Gias_DeveloperServer_Entities(efConnString)

                            Try
                                Dim racc = CInt(row("Raccoglitore_Cod"))

                                If racc = 0 Then
                                    Dim ricettaOperazioneCod = CInt(row("Ricetta_Operazione_Cod"))
                                    Dim superUser = CStr(row("Ricetta_SuperUser"))
                                    Dim guid = CStr(row("GuidRicetta"))
                                    Dim versione = CStr(row("Versione"))
                                    Dim tipo = CStr(row("Tipo"))

                                    Dim ricettaOperazione As Ricette_Operazioni = EFRicette.ReadRicettaOperazione(giasContext, superUser, ricettaOperazioneCod)
                                    Dim listParametriAggiuntivi As New List(Of Parametri_Aggiuntivi_Attivita)

                                    Dim attivita = map.RicettaOperazioneSuAttivita(ricettaOperazione, listParametriAggiuntivi, verbose:=False, objParametriSuperServer, objParametriServer, objParametriUtenti, True, leggiDisciplinari:=False)
                                    attivita.guid = guid
                                    attivita.versione = versione
                                    attivita.origineAttivita = tipo
                                    attivita.codice = "-1"
                                    attivita.inizio = New Date(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day,
                                                                   attivita.oraInizio.Hour, attivita.oraInizio.Minute, 0, DateTimeKind.Local)

                                    If tipo = "10" OrElse tipo = "90" Then 'attività proveniente da app o da demetra
                                        AssegnaOreAMacchineEOperatori(attivita, ricettaOperazioneCod, objParametriServer, ricette_dettagli_r)
                                    End If

                                    Dim statoRicetta = CInt(row("StatoRicetta"))
                                    If statoRicetta = 300 Then 'ricetta

                                        attivita.stato = Stati.Da_Eseguire
                                        listaAttivitaPianificate.Add(attivita)

                                    ElseIf statoRicetta = 301 Then 'brogliaccio

                                        attivita.stato = Stati.Eseguita

                                        Dim codiceGiasPianificata As String = CStr(row("CodiceGiasPianificata"))

                                        If Not String.IsNullOrEmpty(codiceGiasPianificata) Then
                                            attivita.riferimentoPianificata = codiceGiasPianificata
                                        End If

                                        Dim listaBrogliacci As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)
                                        listaBrogliacci.Add(attivita)
                                        listaFinaleBrogliacci.Add(listaBrogliacci)

                                    End If
                                Else

                                    Continue For ' fix temporanea fino alla gestione delle multiattività da parte dell'app

                                    If Not dictRaccoglitoreBrogliacci(racc).GiaProcessato Then
                                        dictRaccoglitoreBrogliacci(racc).GiaProcessato = True

                                        Dim listaBrogliacciDt = ricetteOperazioniDal.LeggiRicettaOperazioneConRaccoglitoreCod(racc, objParametriServer)
                                        If listaBrogliacciDt IsNot Nothing AndAlso listaBrogliacciDt.Rows.Count > 0 Then

                                            Dim attivitaNonSupportata = False
                                            For Each rowBrogliaccio As DataRow In listaBrogliacciDt.Rows
                                                Dim lav_cod = CInt(rowBrogliaccio("Lav_Cod"))
                                                If Not CostantiPersonalizzate.OPERAZIONI_GESTITE_APP_DEMETRA_LIST.Contains(lav_cod) Then
                                                    attivitaNonSupportata = True
                                                    Exit For
                                                End If
                                            Next

                                            If attivitaNonSupportata Then
                                                Continue For
                                            End If

                                            Dim listaBrogliacci As New List(Of AgronicaCoreModelsSTD.attivita.Attivita)
                                            For Each rowBrogliaccio As DataRow In listaBrogliacciDt.Rows
                                                Dim ricettaOperazioneCod = CInt(rowBrogliaccio("Ricetta_Operazione_Cod"))
                                                Dim superUser = CStr(rowBrogliaccio("Ricetta_SuperUser"))

                                                Dim ricettaOperazione As Ricette_Operazioni = EFRicette.ReadRicettaOperazione(giasContext, superUser, ricettaOperazioneCod)
                                                Dim listParametriAggiuntivi As New List(Of Parametri_Aggiuntivi_Attivita)

                                                Dim attivita = map.RicettaOperazioneSuAttivita(ricettaOperazione, listParametriAggiuntivi, verbose:=False, objParametriSuperServer, objParametriServer, objParametriUtenti, True, leggiDisciplinari:=False)
                                                attivita.guid = dictRaccoglitoreBrogliacci(racc).Id
                                                attivita.versione = dictRaccoglitoreBrogliacci(racc).Versione
                                                attivita.origineAttivita = dictRaccoglitoreBrogliacci(racc).Tipo
                                                attivita.codice = "-1"
                                                attivita.inizio = New Date(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day,
                                                                   attivita.oraInizio.Hour, attivita.oraInizio.Minute, 0, DateTimeKind.Local)
                                                If attivita.origineAttivita = "10" OrElse attivita.origineAttivita = "90" Then 'attività proveniente da app o da demetra
                                                    AssegnaOreAMacchineEOperatori(attivita, ricettaOperazioneCod, objParametriServer, ricette_dettagli_r)
                                                End If
                                                listaBrogliacci.Add(attivita)
                                            Next

                                            listaFinaleBrogliacci.Add(listaBrogliacci)
                                        End If

                                    End If
                                End If

                            Catch ex As Exception
                                logger.Scrivi_LOG(objParametriServer, NomeRoutine, ex.Message)
                            End Try

                        End Using
                    End Using

                Next

            End If

            If Not params.LeggiSoloBrogliacci Then
                Dim dtAttivitaCancellate = logAgendaDal.LeggiAttivitaCancellatexApp(params.Piva, dataRif, params.DataUltimaSincro, objParametriServer)

                Dim listaAttivitaCancellate As New List(Of Integer)

                If (dtAttivitaCancellate IsNot Nothing AndAlso dtAttivitaCancellate.Rows.Count > 0) Then
                    For Each row As DataRow In dtAttivitaCancellate.Rows
                        Dim idAgenda = CInt(row("Id_Agenda"))
                        listaAttivitaCancellate.Add(idAgenda)
                    Next
                End If

                result.AttivitaCancellate = listaAttivitaCancellate

            End If

            Dim tipiDemetra As New List(Of String) From
            {
                AppHelper.enum_Dati_App.AttivitaDemetra,
                AppHelper.enum_Dati_App.RicetteDemetra
            }
            Dim tipiApp As New List(Of String) From
            {
                AppHelper.enum_Dati_App.Attivita,
                AppHelper.enum_Dati_App.Ricette
            }

            Dim dtBrogliacciCancellati = logBrogliacciDal.LeggiBrogliacciCancellatixApp(params.Piva, dataRif, tipiDemetra, tipiApp, params.DataUltimaSincro, objParametriServer)

            Dim listaBrogliacciCancellati As New List(Of String)
            Dim listaAttivitaPianificateCancellate As New List(Of String)

            If (dtBrogliacciCancellati IsNot Nothing AndAlso dtBrogliacciCancellati.Rows.Count > 0) Then
                For Each row As DataRow In dtBrogliacciCancellati.Rows
                    Dim guidRicetta = CStr(row("GuidRicetta"))

                    If Not String.IsNullOrWhiteSpace(guidRicetta) Then
                        Dim tipo = CStr(row("Tipo"))

                        If listaFinaleBrogliacci.SelectMany(Function(x) x).ToList().Any(Function(attivita) attivita.guid = guidRicetta) Then
                            Continue For
                        End If

                        If tipo = AppHelper.enum_Dati_App.RicetteDemetra OrElse tipo = AppHelper.enum_Dati_App.Ricette Then
                            If guidRicetta.Contains("|") Then
                                listaAttivitaPianificateCancellate.Add(guidRicetta.Split("|").First())
                            Else
                                listaAttivitaPianificateCancellate.Add(guidRicetta)
                            End If
                        Else
                            If guidRicetta.Contains("|") Then
                                listaBrogliacciCancellati.Add(guidRicetta.Split("|").First())
                            Else
                                listaBrogliacciCancellati.Add(guidRicetta)
                            End If
                        End If

                    End If
                Next
            End If

            result.Brogliacci = listaFinaleBrogliacci
            result.AttivitaPianificate = listaAttivitaPianificate
            result.BrogliacciCancellati = listaBrogliacciCancellati
            result.AttivitaPianificateCancellate = listaAttivitaPianificateCancellate

        Catch ex As Exception
            result = Nothing
        End Try

        Return result

    End Function

    Private Sub AssegnaOreAMacchineEOperatori(ByRef attivita As AgronicaCoreModelsSTD.attivita.Attivita, ByVal ricettaOperazioneCod As Integer, ByRef objParametriServer As AgronicaCoreParametri, ByRef ricette_dettagli_r As Ricette_Dettagli_R)

        Dim macchine = attivita.risorse.OfType(Of RisorsaMacchina).ToList()
        If macchine.Any() Then
            For Each macchina As RisorsaMacchina In macchine
                Dim dt = ricette_dettagli_r.LeggiOreMacchina(ricettaOperazioneCod, macchina.macchina.codice, objParametriServer)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    Dim qta = CDec(dt.Rows(0)("Qta"))
                    If qta > 0 Then
                        macchina.inizio = New Date(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day, 0, 0, 0, DateTimeKind.Local)
                        AssegnaOreLavorateARisorsa(macchina, qta)
                    End If
                End If
            Next
        End If

        Dim operatori = attivita.risorse.OfType(Of RisorsaPersona).ToList()
        If operatori.Any() Then
            For Each operatore As RisorsaPersona In operatori
                Dim dt = ricette_dettagli_r.LeggiOreOperatore(ricettaOperazioneCod, operatore.risorsaUmana.codice, objParametriServer)
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    Dim qta = dt.Rows(0)("Qta")
                    If qta > 0 Then
                        operatore.inizio = New Date(attivita.inizio.Year, attivita.inizio.Month, attivita.inizio.Day, 0, 0, 0, DateTimeKind.Local)
                        AssegnaOreLavorateARisorsa(operatore, qta)
                    End If
                End If
            Next
        End If

    End Sub

    Private Sub AssegnaOreLavorateARisorsa(ByRef risorsa As RisorsaTimeSheet, ByVal qta As Decimal)
        Dim hours As Integer = CInt(Math.Truncate(qta))
        Dim minutes As Integer = CInt(Math.Round((qta - hours) * 60))

        If minutes = 60 Then
            hours += 1
            minutes = 0
        End If

        risorsa.fine = New Date(risorsa.inizio.Value.Year, risorsa.inizio.Value.Month, risorsa.inizio.Value.Day, 0, 0, 0, DateTimeKind.Local).AddHours(hours).AddMinutes(minutes)
        risorsa.totaleOre = qta

    End Sub

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Ottieni_Numero_Trappole_Registrate(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AvversitaTrappole))

        Dim r As New rispostaStandard(Of List(Of AvversitaTrappole))

        Try

            Dim strObjP As String = JsonConvert.SerializeObject(InData.objP)
            Dim objP As CoreWS_GenericObjP = JsonConvert.DeserializeObject(Of CoreWS_GenericObjP)(strObjP)

            Dim objParametri_Server = Utility.convertStringtoOBJparametri(objP.objP_server)
            Dim objParametri_Utenti = Utility.convertStringtoOBJparametri(objP.objP_utenti)
            Dim objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP.objP_super_server)

            'Istanzio gli oggetti InData.
            Dim strxControlla As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataxControlla As Leggi_Numero_Trappole = JsonConvert.DeserializeObject(Of Leggi_Numero_Trappole)(strxControlla,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            Dim leggiLingua As New AgronicaCoreUtentiDAL.Lingue_Read
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(objParametri_Server.Lingua_Cod, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)
            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaCodiceISO)

            Dim ObjRilievi As New AgronicaControlli_2010.Rilievi

            Dim listAvversitaTrappole = ObjRilievi.LetturaAvversitaTrappole(inDataxControlla.eserciziCDC, inDataxControlla.avversitaGruppo, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = listAvversitaTrappole

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

End Class