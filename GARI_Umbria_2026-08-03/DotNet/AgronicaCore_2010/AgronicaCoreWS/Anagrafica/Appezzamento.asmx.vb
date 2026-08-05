Imports System.Web.Services
Imports AgronicaControlliGIS
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.baseClass
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreVarieBIZ
Imports InData
Imports InData.Anagrafica
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Appezzamento
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    Public Function Scrivi_Appezzamento_Anagrafica(InData As Object) As rispostaStandard(Of anagrafiche.Appezzamento)
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}
        Dim iData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of anagrafiche.Appezzamento))(JsonConvert.SerializeObject(InData), a)

        Dim data = New CoreWS_Generic(Of AppezzamentoFiltroTemporale) With {
                .objP = iData.objP,
                .InData = New AppezzamentoFiltroTemporale With {.Appezzamento = iData.InData}
        }
        Return HandleScriviAppezzamento(data)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function Scrivi_Appezzamento_Anagrafica_Filtro_Temporale(InData As Object) As rispostaStandard(Of anagrafiche.Appezzamento)
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}
        Dim iData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AppezzamentoFiltroTemporale))(JsonConvert.SerializeObject(InData), a)
        Return HandleScriviAppezzamento(iData)
    End Function

    Private Function HandleScriviAppezzamento(iData As CoreWS_Generic(Of AppezzamentoFiltroTemporale)) As rispostaStandard(Of anagrafiche.Appezzamento)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

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

        Dim objParametri_Super_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Utenti As AgronicaCoreParametri = Nothing

        Dim FlagTransazioneLocale As Boolean = False
        Dim FlagConnessioneLocale As Boolean = False

        Try

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim AppezzamentoBIZ As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

            Dim esitoPositivoScritturaAppezzamento = AppezzamentoBIZ.Appezzamento_ScriviModifica(
                iData.InData.Appezzamento,
                objParametri_Server,
                objParametri_Utenti,
                OpenNewTransaction:=objParametri_Server.objTransazione Is Nothing
                )

            If esitoPositivoScritturaAppezzamento Then

                Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

                If iData.InData.Appezzamento.flag_cancellazione = False Then

                    Dim obj_Appezzamento = objAppezzamento.Leggi_Appezzamento_Anagrafica(
                        iData.InData.Appezzamento.primaryKey.centroAziendalePK.partitaIva,
                        iData.InData.Appezzamento.primaryKey.centroAziendalePK.codice,
                        iData.InData.Appezzamento.primaryKey.codice,
                        0,
                        True,
                        True,
                        True,
                        AGRODATAINIZIO,
                        False,
                        True,
                        True,
                        objParametri_Super_Server,
                        objParametri_Server,
                        objParametri_Utenti
                        )

                    Valorizzazione_Geojson_NodeInfo_Appezzamento(
                        obj_Appezzamento,
                        iData,
                        objParametri_Super_Server,
                        objParametri_Server,
                        objParametri_Utenti
                        )

                    r.RispostaOK = True
                    r.RispostaStringa = obj_Appezzamento

                Else

                    r.RispostaOK = True
                    r.RispostaStringa = iData.InData.Appezzamento

                End If

            Else

                Throw New Exception("Errore in scrittura Appezzamento with data " + iData.InData.ToString())

            End If

            If objParametri_Server.objTransazione IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)
            End If

        Catch ex As GiasException

            r.RispostaOK = False
            r.RispostaStringa = iData.InData.Appezzamento
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = ""

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = iData.InData.Appezzamento
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

        Finally

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function Leggi_Appezzamento_Anagrafica(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of LeggiAppezzamento) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAppezzamento))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim Piva = iData.InData.appezzamento.primaryKey.centroAziendalePK.partitaIva
            Dim Sa_Cod = iData.InData.appezzamento.primaryKey.centroAziendalePK.codice
            Dim Appezza = iData.InData.appezzamento.primaryKey.codice
            Dim IdReg = 0

            If (iData.InData.appezzamento.impianti IsNot Nothing) Then
                IdReg = iData.InData.appezzamento.impianti(0).primaryKey.codice
            End If

            Dim Data_Filtro = DateTime.Now

            Dim obj_Appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento = objAppezzamento.Leggi_Appezzamento_Anagrafica(
                Piva,
                Sa_Cod,
                Appezza,
                IdReg,
                iData.InData.leggiImpianti,
                iData.InData.leggiIndirizzi,
                iData.InData.leggiCatasto,
                iData.InData.data,
                iData.InData.filtroData,
                iData.InData.leggiDistinte,
                iData.InData.leggiCartografia,
                objParametri_Super_Server,
                objParametri_Server,
                objParametri_Utenti
                )

            If obj_Appezzamento Is Nothing Then
                Dim keys() As String = {Piva, Sa_Cod, Appezza}
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
                                x.Disciplinari_Elenco_TuttigliElemInChiave(
                                    ddl,
                                    False,
                                    "",
                                    "",
                                    Session,
                                    objParametri_Server,
                                    objParametri_Utenti,
                                    0,
                                    var.specie.codice.ToString,
                                    0,
                                    0,
                                    True,
                                    True,
                                    False,
                                    New AgronicaCoreGestioneRichieste.AgroWebConfig() With {.Flag_DisciplinarePrivato = disc.disciplinarePubblicoPrivato, .Flag_DisciplinareAttivo = True},
                                    True
                                    )

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

    <WebMethod(EnableSession:=True)>
    Public Function Leggi_Appezzamento_AnagraficaXGUID(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento))(JsonConvert.SerializeObject(InData), a)

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

        Dim objParametri_Super_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Utenti As AgronicaCoreParametri = Nothing

        Try

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            If iData.InData.flag_cancellazione = False Then

                Dim obj_Appezzamento = objAppezzamento.Leggi_Appezzamento_GUID(InData.InData.guid,
                                                                               objParametri_Super_Server,
                                                                               objParametri_Server,
                                                                               objParametri_Utenti)

                r.RispostaOK = True
                r.RispostaStringa = obj_Appezzamento

            Else

                r.RispostaOK = True
                r.RispostaStringa = iData.InData

            End If

        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ReadAgriculturalPlotLight(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.AppezzamentoJoinDescrizioni)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.AppezzamentoJoinDescrizioni)

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of Parametri_ObjParametriAgenda_NG) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Parametri_ObjParametriAgenda_NG))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim Piva = iData.InData.Piva
            Dim Sa_Cod = iData.InData.Sa_Cod
            Dim Appezza = iData.InData.Appezza

            Dim obj_Appezzamento As AgronicaCoreModelsSTD.anagrafiche.Appezzamento = objAppezzamento.ReadAgriculturalPlotLight(
                Piva,
                Sa_Cod,
                Appezza,
                objParametriServer:=objParametriServer,
                objParametriUtenti:=objParametriUtenti,
                objParametriSuperServer:=objParametriSuperServer
                )

            If obj_Appezzamento Is Nothing Then
                Dim keys() As String = {Piva, Sa_Cod, Appezza}
                Throw New Exception("Errore in lettura Appezzamento with key " + String.Join("|", keys))
            End If

            r.RispostaOK = True
            r.RispostaStringa = obj_Appezzamento

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function Leggi_Contributi(InData As Object
                                                  ) As rispostaStandard(Of List(Of BaseCodeDescrStr))
        Dim r As New rispostaStandard(Of List(Of BaseCodeDescrStr))

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impianto) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impianto))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreMetaSchemaDAL.Contributi

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim ValiditaInizio = If(iData.InData.validita Is Nothing, AGRODATAINIZIO, iData.InData.validita.inizio)
            Dim ValiditaFine = If(iData.InData.validita Is Nothing, AGRODATAFINE, iData.InData.validita.fine)

            Dim obj_Contributi As DataTable = objAppezzamento.LeggiContributi(
                                                                                ValiditaInizio,
                                                                                ValiditaFine,
                                                                                "",
                                                                                "",
                                                                                objParametri_Server
                                                                                )
            Dim rispostaList As List(Of BaseCodeDescrStr) = New List(Of BaseCodeDescrStr)
            For Each contributo In obj_Contributi.Rows
                rispostaList.Add(New BaseCodeDescrStr(contributo("Contributo_Cod"), contributo("Contributo_Des")))
            Next

            r.RispostaOK = True
            r.RispostaStringa = rispostaList

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function Leggi_Appezzamento_UtilizzoTerreno(InData As Object
                                                  ) As rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno)

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impianto.PK) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impianto.PK))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim Piva = iData.InData.appezzamentoPK.centroAziendalePK.partitaIva
            Dim Sa_Cod = iData.InData.appezzamentoPK.centroAziendalePK.codice
            Dim Appezza = iData.InData.appezzamentoPK.codice
            Dim IdReg = iData.InData.codice

            Dim Data_Filtro = DateTime.Now

            Dim obj_UtilizzoTerreno As AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno = objAppezzamento.Leggi_Appezzamento_UtilizzoTerreno(
                                                                                Piva,
                                                                                Sa_Cod,
                                                                                Appezza,
                                                                                IdReg,
                                                                                objParametri_Server)

            If obj_UtilizzoTerreno Is Nothing Then
                Dim keys() As String = {Piva, Sa_Cod, Appezza}
                Throw New Exception("Errore in lettura Appezzamento with key " + String.Join("|", keys))
            End If

            r.RispostaOK = True
            r.RispostaStringa = obj_UtilizzoTerreno

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_DropDown_Appezzamenti_Codici(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe))


        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim StrCodiciImpianto As String
            'carico la combo dei codici
            Dim objcodAn As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            StrCodiciImpianto = objcodAn.Filtro_Codici_Anagrafe(3, 3, 2, objParametri_Server)
            'Elimino il codice Titolo Possesso, Metodo Produzione, Magazzino Conferimento,
            'Organismo Referente, Capitolato Privato e Dettaglio Specie Personalizzato 
            'perchè già presenti
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.TitoloPossesso), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.TitoloPossesso) + " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.MetodoDiProduzione), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.MetodoDiProduzione) + " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Riferimento_Trasferimento_Dati) + " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Organismo_Referente), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Organismo_Referente) + " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Capitolato_Privato), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Capitolato_Privato) + " Or ", "")
            ' - - - - - - - - - - - - - - - - A N N A      I N I Z I O - - - - - - - - - - - - - - - -
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Zespri_Fasi_Fase) + " Or ", "")
            ' - - - - - - - - - - - - - - - - A N N A      F I N E - - - - - - - - - - - - - - - - - -
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Dettaglio_Specie_Personalizzato) + " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Magazzino_Conferimento), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Magazzino_Conferimento) + " Or ", "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Data_Inizio_Portinnesto) + " Or ", "")


            Dim obj_Codici As New List(Of AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe)
            Dim objCodiceAnagrafe As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
            Dim DTCod = objCodiceAnagrafe.Leggi(0,
                                     "",
                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                     StrCodiciImpianto,
                                     "",
                                     objParametri_Server)
            For Each rowCod In DTCod.Rows
                Dim jCod As New AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe
                jCod.codice = rowCod("Codice")
                jCod.descrizione = rowCod("Descrizione")
                obj_Codici.Add(jCod)
            Next

            r.RispostaOK = True
            r.RispostaStringa = obj_Codici

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Max_DataModifica(InData As Object) As rispostaStandard(Of AgronicaCoreDTOStd.InData.Data)

        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.Data)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim objStr = JsonConvert.SerializeObject(InData, a)

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

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

            Dim objAppezza_R As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
            Dim dataMax = objAppezza_R.LeggiMaxData(iData.InData, objParametri_Server, objParametri_Utenti)

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
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Max_DataModifica_NG(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.LeggiFiltro)) As rispostaStandard(Of AgronicaCoreDTOStd.InData.Data)

        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.Data)

        'Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        'Dim objStr = JsonConvert.SerializeObject(InData, a)

        'Dim iData As CoreWS_Generic(Of String) =
        '    JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objAppezza_R As New AgronicaCoreAnagrafeBIZ.Appezzamento_R
            Dim dataMax = objAppezza_R.LeggiMaxData(InData.InData.Ricerca, objParametri_Server, objParametri_Utenti)

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
    <Script.Services.ScriptMethod()>
    Public Function BloccaSblocca_Appezzamenti(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.BloccaSbloccaAppezzamenti) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.BloccaSbloccaAppezzamenti))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objAppezzamentoBIZ As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

            objAppezzamentoBIZ.BloccaSbloccaAppezzamenti(iData.InData.appezzamenti, iData.InData.blocca, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Verifica_Superficie(InData As CoreWS_Generic(Of Object)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim MessaggioErrore As String = ""

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impianto) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impianto))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim Validita_Fine = iData.InData.validita.fine
            Dim Validita_Inizio = iData.InData.validita.inizio

            Dim sa_cod = iData.InData.primaryKey.appezzamentoPK.centroAziendalePK.codice
            Dim piva = iData.InData.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
            Dim appezza = iData.InData.primaryKey.appezzamentoPK.codice
            Dim id_Reg = iData.InData.primaryKey.codice
            Dim sup_imp = iData.InData.superficie

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objImpiantoR As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim imp = objImpiantoR.Leggi(piva, sa_cod, appezza, id_Reg, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim ObjAgenda As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim DTAgenda = ObjAgenda.LeggiCronologiaMovimenti(piva,
                                                        sa_cod,
                                                        appezza,
                                                        id_Reg,
                                                        AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                        "",
                                                        "",
                                                        objParametri_Server)

            If imp.Rows.Count > 0 Then
                If sup_imp <> imp.Rows(0).Item("sup_imp") And DTAgenda.Rows.Count > 0 Then
                    MessaggioErrore += ("Attenzione! E' variata la Superficie dell'impianto! Poiché a quest'ultimo risultano essere associati movimenti d'agenda sarà NECESSARIO modificare tali movimenti al fine di consentire un coerente ricalcolo delle dosi! Si desidera MODIFICARE comunque la superficie?")
                    r.RispostaStringa = JsonConvert.SerializeObject(MessaggioErrore, Formatting.None)
                    r.RispostaOK = True
                Else
                    r.RispostaStringa = ""
                    r.RispostaOK = True
                End If
            End If

        Catch ex As Exception
            MessaggioErrore = ex.Message.ToString()
            'r.RispostaOK = False
            'r.Errore = MessaggioErrore
            Throw New Exception(MessaggioErrore)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Verifica_OperazioniAgenda(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of AgronicaCoreDTOStd.InData.Agenda.PresenzaMovimentazioni)

        Dim r As New rispostaStandard(Of AgronicaCoreDTOStd.InData.Agenda.PresenzaMovimentazioni)
        Dim MessaggioErrore As String = ""
        Dim nomeRoutine As String = "Appezzamento.Verifica_OperazioniAgenda()"

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impianto) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Impianto))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim retObj As New AgronicaCoreDTOStd.InData.Agenda.PresenzaMovimentazioni
            With retObj
                .trattamentiConcimazioni = False
                .movimentazioni = False
            End With

            Dim sa_cod = iData.InData.primaryKey.appezzamentoPK.centroAziendalePK.codice
            Dim piva = iData.InData.primaryKey.appezzamentoPK.centroAziendalePK.partitaIva
            Dim appezza = iData.InData.primaryKey.appezzamentoPK.codice
            Dim id_Reg = iData.InData.primaryKey.codice
            Dim sup_imp = iData.InData.superficie

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objMovDes As New AgronicaCoreContabDAL.Mov_Destinazioni_R

            Dim lavCod_trattamentiConcimazioni As New List(Of Integer)

            retObj.trattamentiConcimazioni = False
            retObj.movimentazioni = False
            Dim controllo As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
            Try
                iData.InData.utilizzoTerreno = New DestinazioneUso() With {
                    .classType = ClassType.DestinazioneUso,
                    .codice = -1,
                    .descrizione = ""
                }
                controllo.Verifica_Utilizzo(iData.InData, objParametri_Server)
            Catch ex As Exception
                retObj.movimentazioni = True
            End Try
            r.RispostaStringa = retObj
            r.RispostaOK = True
            'controlla la presenza di movimenti di agenda
            'TODO: aggiungere brogliaccio,CDG, Tabelle temporanee APP
            'If controllo.controllo_MovimentiRicettexEliminazione(iData.InData, piva, sa_cod, appezza, id_Reg, objParametri_Server) Then
            '    retObj.movimentazioni = True
            '    r.RispostaStringa = retObj 'JsonConvert.SerializeObject(retObj, Formatting.None)
            '    r.RispostaOK = True
            'Else
            '    retObj.movimentazioni = False
            '    r.RispostaStringa = retObj 'JsonConvert.SerializeObject(retObj, Formatting.None)
            '    r.RispostaOK = True
            'End If


        Catch ex As Exception
            MessaggioErrore = ex.Message.ToString()
            'r.RispostaOK = False
            'r.Errore = MessaggioErrore
            Throw New Exception(MessaggioErrore)
        End Try

        Return r
    End Function

    '<WebMethod()> <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    'Public Function Leggi_Appezzamento_Global_Anagrafica(ByVal objP_super_server As String,
    '                                                        ByVal objP_server As String,
    '                                                        ByVal objP_utenti As String,
    '                                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

    <WebMethod()> <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function Leggi_Appezzamento_Global_Anagrafica(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If InData.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim appezzamento = objAppezzamento.Leggi_Appezzamento_Anagrafica(Piva:=InData.InData.Piva,
                                                Sa_Cod:=InData.InData.Sa_Cod,
                                                Appezza:=InData.InData.Appezza,
                                                IdReg:=0,
                                                Leggi_Impianti:=True,
                                                Leggi_Indirizzi:=True,
                                                Leggi_Catasto:=True,
                                                Leggi_Cartografia:=True,
                                                data:=AGRODATAINIZIO,
                                                filtroData:=False,
                                                Leggi_Distinte:=True,
                                                objParametri_Super_Server:=objParametri_Super_Server,
                                                objParametri_Server:=objParametri_Server,
                                                objParametri_Utenti:=objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = appezzamento
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function leggiGenerazionePoligoniDefaultValue(InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

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

            Dim configurazioneSitiLettura As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim GIS_cmbElementoGrafico_GenerazionePoligoni_DefaultValue =
            configurazioneSitiLettura.Leggi_Valore(
                0, "GIS_cmbElementoGrafico_GenerazionePoligoni_DefaultValue", "", "", objParametri_Server
            )

            r.RispostaStringa = GIS_cmbElementoGrafico_GenerazionePoligoni_DefaultValue
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            r.RispostaOK = False
            Return r
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CopiaSposta_Appezzamenti(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.CopiaSpostaAppezzamenti) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Anagrafica.CopiaSpostaAppezzamenti))(JsonConvert.SerializeObject(InData))

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

            Dim objAppezzamentoBIZ As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

            Dim msg = objAppezzamentoBIZ.CopiaSpostaAppezzamenti(iData.InData.Appezzamenti,
                                                                 iData.InData.NuovoCentro,
                                                                 iData.InData.SpostaEliminaOrigine,
                                                                 iData.InData.CopiaCatasto,
                                                                 objParametri_Super_Server,
                                                                 objParametri_Server,
                                                                 objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = ""

            If msg <> "" Then
                Dim ErroreGias As New ErroreGias
                ErroreGias.messaggio = msg
                ErroreGias.severity = ErroreGias_Severity.WarningBloccante

                r.ErroriGias.Add(ErroreGias)
            End If


        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

#Region "COMBO MODIFICA MULTIPLA"
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboCmb_Contributi(ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objAppezzamento As New AgronicaCoreMetaSchemaDAL.Contributi
            Dim DT As DataTable = objAppezzamento.LeggiContributi(AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            Dim jArray As New JArray()
            For Each dr As DataRow In DT.Rows
                jArray.Add(New JObject(New JProperty("text", dr.Item("Contributo_Des")),
                                                   New JProperty("value", dr.Item("Contributo_Cod"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArray, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
#End Region

#Region "AppezzamentiXParcoMacchine"
    <WebMethod(EnableSession:=True)>
    Public Function ReadAppezzamentiXParcoMacchine(InData As Object) As rispostaStandard(Of List(Of LinkedMachine(Of anagrafiche.Appezzamento.PK)))

        Dim r As New rispostaStandard(Of List(Of LinkedMachine(Of anagrafiche.Appezzamento.PK)))
        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of Anagrafica.AppezzamentoXParcoMacchine) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Anagrafica.AppezzamentoXParcoMacchine))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objBIZ As New AgronicaCoreAnagrafeBIZ.AppezzamentiXParcoMacchine_R

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim piva = iData.InData.piva
            Dim saCod = iData.InData.saCod
            Dim appezza = iData.InData.appezza
            Dim macCod = iData.InData.macCod
            Dim classCode = iData.InData.classCode
            Dim startValidity = iData.InData.validity.inizio
            Dim endValidity = iData.InData.validity.fine

            Dim linkedMachines As List(Of LinkedMachine(Of anagrafiche.Appezzamento.PK)) = objBIZ.Read(
                objServer:=objParametriServer,
                objUtenti:=objParametriUtenti,
                piva,
                saCod,
                appezza,
                macCod,
                classCode,
                startValidity,
                endValidity
                )

            r.RispostaOK = True
            r.RispostaStringa = linkedMachines

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function WriteAppezzamentiXParcoMacchine(InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)
        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objBIZ As New AgronicaCoreAnagrafeBIZ.AppezzamentiXParcoMacchine_W

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            r.RispostaOK = True
            r.RispostaStringa = objBIZ.Write(
                iData.InData,
                objParametriServer,
                objParametriUtenti
                )

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function EditAppezzamentiXParcoMacchine(InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)
        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of anagrafiche.LinkedMachine(Of anagrafiche.Appezzamento.PK)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of anagrafiche.LinkedMachine(Of anagrafiche.Appezzamento.PK)))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objBIZ As New AgronicaCoreAnagrafeBIZ.AppezzamentiXParcoMacchine_W

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            r.RispostaOK = True
            r.RispostaStringa = objBIZ.Edit(
                iData.InData,
                objParametriServer,
                objParametriUtenti
                )

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function DeleteAppezzamentiXParcoMacchine(InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)
        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of AppezzamentoXParcoMacchine) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AppezzamentoXParcoMacchine))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objBIZ As New AgronicaCoreAnagrafeBIZ.AppezzamentiXParcoMacchine_W

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            r.RispostaOK = True
            r.RispostaStringa = objBIZ.Delete(
                objParametriServer,
                objParametriUtenti,
                iData.InData.piva,
                iData.InData.saCod,
                iData.InData.appezza,
                iData.InData.macCod,
                iData.InData.classCode,
                iData.InData.validity.inizio,
                iData.InData.validity.fine
                )

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function DeleteAppezzamentiXParcoMacchineRecords(InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)
        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objBIZ As New AgronicaCoreAnagrafeBIZ.AppezzamentiXParcoMacchine_W

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            r.RispostaOK = True
            r.RispostaStringa = objBIZ.DeleteRecords(
                iData.InData,
                objParametriServer,
                objParametriUtenti
                )

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function
#End Region

#Region "PlotWeaving"
    <WebMethod(EnableSession:=True)>
    Public Function UpdatePlotsWeaving(InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)
        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objBIZ As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            r.RispostaOK = True
            r.RispostaStringa = objBIZ.UpdateWeaving(
                iData.InData,
                objParametriServer,
                objParametriUtenti
                )

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function
#End Region

#Region "PlotSlope"
    <WebMethod(EnableSession:=True)>
    Public Function UpdatePlotsSlope(InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)
        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objBIZ As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            r.RispostaOK = True
            r.RispostaStringa = objBIZ.UpdateSlope(
                iData.InData,
                objParametriServer,
                objParametriUtenti
                )

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function
#End Region

#Region "PlotConstrain"
    <WebMethod(EnableSession:=True)>
    Public Function UpdatePlotsConstrain(InData As Object) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)
        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of anagrafiche.Appezzamento)))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objBIZ As New AgronicaCoreAnagrafeBIZ.Appezzamento_W

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametriUtenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            r.RispostaOK = True
            r.RispostaStringa = objBIZ.UpdateConstrain(
                iData.InData,
                objParametriServer,
                objParametriUtenti
                )

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function
#End Region

    Private Sub Valorizzazione_Geojson_NodeInfo_Appezzamento(
        ByRef obj_Appezzamento As anagrafiche.Appezzamento,
        ByVal iData As CoreWS_Generic(Of AppezzamentoFiltroTemporale),
        ByRef objParametri_Super_Server As AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri)

        '29/11/2022 - lavez - valorizzazione geojson + nodeinfo di ritorno per app\imp appena inserito

        Dim ns As XNamespace = "http://www.opengis.net/gml"

        Dim objEntitaBiz As New AgronicaCoreGisBIZ.DatiEntita_R

        Dim cfgalbero As New ConfigurazioneAlbero
        cfgalbero.TipologiaLayer_Cod = 1

        Dim Codice_Fiscale_Tecnico As String = "CF TEC"

        V_M.CodiceFiscaleTecnicoImposta(objParametri_Server, objParametri_Utenti, Codice_Fiscale_Tecnico)

        Dim elencoLayers = New List(Of ElencoLayerVisibiliUtenteTipologia) From {
            New ElencoLayerVisibiliUtenteTipologia() With {.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI},
            New ElencoLayerVisibiliUtenteTipologia() With {.LayerElementiGrafici_Cod = enum_Gis_LayerElementiGrafici_std.IMPIANTI}
        }
        Dim layersVisibili = New LayerVisibiliUtenteTipologia With {
                .Username = objParametri_Server.UtenteUsername,
                .PivaSuperUser = objParametri_Server.PivaSuperUser,
                .TipologiaLayer = 1, ' TODO controllare il valore
                .elencoLayers = elencoLayers
        }

        Dim dt = objEntitaBiz.LeggiPerGerarchiaImprese(
            objParametri_Server.PivaSuperUser,
            "",
            0,
            1,
            iData.InData.Appezzamento.primaryKey.centroAziendalePK.partitaIva,
            iData.InData.Appezzamento.primaryKey.centroAziendalePK.codice,
            iData.InData.Appezzamento.primaryKey.codice,
            iData.InData.Appezzamento.campoPK.codice,
            0,
            0,
            0,
            "",
            "",
            "-1",
            -1,
            -1,
            "-1",
            0,
            0,
            0,
            0,
            False,
            True,
            False,
            True,
            -1,
            Codice_Fiscale_Tecnico,
            "",
            cfgalbero,
            "",
            objParametri_Server,
            objParametri_Utenti,
            filtroTemporaleAvanzato:=iData.InData.FiltroTemporale,
            LayersVisibili:=layersVisibili)

        obj_Appezzamento.obj_app = New GisDataReadRval_New(Of GeoJSONAgroGisProp)

        Const layerAppezzamento = CInt(enum_Gis_LayerElementiGrafici_std.APPEZZAMENTI)

        Dim layerlistAppezzamento As New List(Of Integer)

        layerlistAppezzamento.Add(layerAppezzamento)

        obj_Appezzamento.obj_app.myGeoJson = V_M.GetPlaceGeoJSONFromDataTable(
            objParametri_Server.PivaSuperUser,
            dt,
            ns,
            enum_TipologiaLayer.Std,
            layerlistAppezzamento,
            Nothing,
            "",
            -1,
            Codice_Fiscale_Tecnico,
            objParametri_Super_Server,
            objParametri_Server,
            objParametri_Utenti)

        obj_Appezzamento.nodeInfo_app = V_M.GetNodeTreeInfos(
            layerAppezzamento,
            iData.InData.Appezzamento.primaryKey.centroAziendalePK.partitaIva,
            iData.InData.Appezzamento.primaryKey.centroAziendalePK.codice,
            iData.InData.Appezzamento.campoPK.codice,
            iData.InData.Appezzamento.primaryKey.codice,
            0,
            cfgalbero,
            objParametri_Server)

        For Each impianto In obj_Appezzamento.impianti

            Valorizzazione_Geojson_NodeInfo_Impianto(
                impianto,
                objEntitaBiz,
                ns,
                iData,
                cfgalbero,
                Codice_Fiscale_Tecnico,
                objParametri_Super_Server,
                objParametri_Server,
                objParametri_Utenti)

        Next

    End Sub

    Private Sub Valorizzazione_Geojson_NodeInfo_Impianto(
        ByRef impianto As Impianto,
        ByRef objEntitaBiz As AgronicaCoreGisBIZ.DatiEntita_R,
        ByVal ns As XNamespace,
        ByVal iData As CoreWS_Generic(Of AppezzamentoFiltroTemporale),
        ByVal cfgalbero As ConfigurazioneAlbero,
        ByVal Codice_Fiscale_Tecnico As String,
        ByRef objParametri_Super_Server As AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef objParametri_Utenti As AgronicaCoreParametri)

        Dim codice_specie = 0
        Dim codice_destinazione_uso = 0

        If Not IsNothing(impianto.utilizzoTerreno) Then

            Select Case impianto.utilizzoTerreno.GetType()

                Case GetType(AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta)

                    codice_specie = CType(impianto.utilizzoTerreno, AgronicaCoreModelsSTD.metaschema.utilizzi.Varieta).specie.codice
                    codice_destinazione_uso = 0

                Case Else

                    codice_specie = 0
                    codice_destinazione_uso = impianto.utilizzoTerreno.codice

            End Select

        End If

        Dim dt = objEntitaBiz.LeggiPerGerarchiaImprese(
            objParametri_Server.PivaSuperUser,
            "",
            0,
            0,
            iData.InData.Appezzamento.primaryKey.centroAziendalePK.partitaIva,
            iData.InData.Appezzamento.primaryKey.centroAziendalePK.codice,
            iData.InData.Appezzamento.primaryKey.codice,
            iData.InData.Appezzamento.campoPK.codice,
            codice_specie,
            codice_destinazione_uso,
            impianto.primaryKey.codice,
            "",
            "",
            "-1",
            -1,
            -1,
            "-1",
            0,
            0,
            0,
            0,
            False,
            True,
            False,
            True,
            -1,
            Codice_Fiscale_Tecnico,
            "",
            cfgalbero,
            "",
            objParametri_Server,
            objParametri_Utenti)

        impianto.obj_imp = New GisDataReadRval_New(Of GeoJSONAgroGisProp)

        Const layerImpianto = CInt(enum_Gis_LayerElementiGrafici_std.IMPIANTI)

        Dim layerlistImpianto = New List(Of Integer)

        layerlistImpianto.Add(layerImpianto)

        impianto.obj_imp.myGeoJson = V_M.GetPlaceGeoJSONFromDataTable(
            objParametri_Server.PivaSuperUser,
            dt,
            ns,
            enum_TipologiaLayer.Std,
            layerlistImpianto,
            Nothing,
            "",
            -1,
            Codice_Fiscale_Tecnico,
            objParametri_Super_Server,
            objParametri_Server,
            objParametri_Utenti)

        impianto.nodeInfo_imp = V_M.GetNodeTreeInfos(
            layerImpianto,
            iData.InData.Appezzamento.primaryKey.centroAziendalePK.partitaIva,
            iData.InData.Appezzamento.primaryKey.centroAziendalePK.codice,
            iData.InData.Appezzamento.campoPK.codice,
            iData.InData.Appezzamento.primaryKey.codice,
            impianto.primaryKey.codice,
            cfgalbero,
            objParametri_Server)

    End Sub

End Class