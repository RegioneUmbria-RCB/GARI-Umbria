Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Analisi
Imports AgronicaCoreModelsSTD.analisi
Imports InData.Analisi
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreUtility
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreWebService
Imports DocumentFormat.OpenXml.Wordprocessing
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<Script.Services.ScriptService()>
<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Analisi_Modello
    Inherits WebService

    <WebMethod(EnableSession:=True)>
    Public Function LeggiAnalisiTerreno(InData As CoreWS_Generic(Of LeggiAnalisiTerreno)) As rispostaStandard(Of AnalisiTerreno)

        Dim r As New rispostaStandard(Of AnalisiTerreno)

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of LeggiAnalisiTerreno) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiAnalisiTerreno))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAnalisiModello As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_R

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim obj_Analisi As AnalisiTerreno = AgronicaCoreAnagrafeBIZ.Analisi_Modello_R.Leggi_AnalisiTerreno_Modello(
                                                                                                      objParametri_Server.PivaSuperUser,
                                                                                                      iData.InData.Analisi_Cod,
                                                                                                      objParametri_Super_Server,
                                                                                                      objParametri_Server,
                                                                                                      objParametri_Utenti)



            r.RispostaOK = True
            r.RispostaStringa = obj_Analisi

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ScriviAnalisiTerreno(InData As Object) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim iData As CoreWS_Generic(Of ScriviAnalisiTerreno) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ScriviAnalisiTerreno))(JsonConvert.SerializeObject(InData), a)

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

            Dim AnalisiModelloBIZ As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_W
            Dim esitoScrittura = AnalisiModelloBIZ.Scrivi_AnalisiTerreno_Modello(iData.InData.Piva,
                                                                                 iData.InData.AnalisiTerreno.codice,
                                                                                 iData.InData.AnalisiTerreno,
                                                                                 objParametri_Super_Server,
                                                                                 objParametri_Server,
                                                                                 objParametri_Utenti,
                                                                                 NoteLog:="Analisi_Modello.asmx",
                                                                                 saltaControlliAggancio:=iData.InData.saltaControlliAggancio)

            r.RispostaStringa = JsonConvert.SerializeObject(iData.InData.AnalisiTerreno)

        Catch ex As GiasException

            r.RispostaOK = False
            r.RispostaStringa = ex.Message
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = JsonConvert.SerializeObject(iData.InData.AnalisiTerreno)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ModificaAnalisiTerrenoInLine(InData As Object) As rispostaStandard(Of Boolean)
        Dim r As New rispostaStandard(Of Boolean)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim iData As CoreWS_Generic(Of ScriviAnalisiTerreno) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ScriviAnalisiTerreno))(JsonConvert.SerializeObject(InData), a)

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

            Dim AnalisiModelloBIZ As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_W
            Dim objAnalisiTerreno As AgronicaCoreModelsSTD.analisi.AnalisiTerreno

            Dim analisiTerreno_Old = AgronicaCoreAnagrafeBIZ.Analisi_Modello_R.Leggi_AnalisiTerreno_Modello(objParametri_Server.PivaSuperUser,
                                                                                                            iData.InData.AnalisiTerreno.codice,
                                                                                                            objParametri_Super_Server,
                                                                                                            objParametri_Server,
                                                                                                            objParametri_Utenti)
            analisiTerreno_Old.descrizione = iData.InData.AnalisiTerreno.descrizione
            If analisiTerreno_Old.certificatoAnalisi Is Nothing Then
                analisiTerreno_Old.certificatoAnalisi = New CertificatoAnalisi()
            End If

            analisiTerreno_Old.certificatoAnalisi.numero_certificato = iData.InData.AnalisiTerreno.certificatoAnalisi.numero_certificato
            analisiTerreno_Old.note = iData.InData.AnalisiTerreno.note
            analisiTerreno_Old.validita = iData.InData.AnalisiTerreno.validita

            objAnalisiTerreno = analisiTerreno_Old

            Dim esitoScrittura = AnalisiModelloBIZ.Scrivi_AnalisiTerreno_Modello(iData.InData.Piva,
                                                                                 iData.InData.AnalisiTerreno.codice,
                                                                                 objAnalisiTerreno,
                                                                                 objParametri_Super_Server,
                                                                                 objParametri_Server,
                                                                                 objParametri_Utenti,
                                                                                 NoteLog:="Analisi_Modello.asmx (inline)",
                                                                                 saltaControlliAggancio:=True) 'Sempre true perchè dall'inline non si possono modificare i parametri
            r.RispostaOK = True
            r.RispostaStringa = esitoScrittura

        Catch ex As GiasException

            r.RispostaOK = False
            r.RispostaStringa = False
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function CancellaAnalisiTerreno(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard
        Dim errMsg = ""

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        Dim iData As CoreWS_Generic(Of ScriviAnalisiTerreno) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of ScriviAnalisiTerreno))(JsonConvert.SerializeObject(InData), a)

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

            Dim AnalisiModelloBIZ As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_W

            If iData.InData.AnalisiTerreno.flag_cancellazione Then
                Dim esitoScrittura = AnalisiModelloBIZ.Scrivi_AnalisiTerreno_Modello(iData.InData.Piva,
                                                                                         iData.InData.AnalisiTerreno.codice,
                                                                                         iData.InData.AnalisiTerreno,
                                                                                         objParametri_Super_Server,
                                                                                         objParametri_Server,
                                                                                         objParametri_Utenti,
                                                                                         NoteLog:="Analisi_Modello.asmx")
                errMsg = If(esitoScrittura, "", "Cancellazione non riuscita")
            End If


            r.RispostaOK = If(errMsg <> "", False, True)
            r.RispostaStringa = errMsg

        Catch ex As GiasException

            r.RispostaOK = False
            r.RispostaStringa = "Errore durante la cancellazione: " & ex.Message
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = ""

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = "Errore durante la cancellazione: " & ex.Message
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function CancellaListaAnalisiTerreno(InData As CoreWS_Generic(Of ScriviListaAnalisiTerreno)) As RispostaStandard
        Dim r As New RispostaStandard
        Dim errMsg = ""

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

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

        Dim listaAnalisiTerreno = InData.InData.listaAnalisiTerreno

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Dim AnalisiModelloBIZ As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_W

        Dim errori = New List(Of String)

        Try
            objParametri_Super_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            For Each cancellazione In listaAnalisiTerreno
                Dim descrizione As String = cancellazione.AnalisiTerreno.descrizione
                Try
                    If cancellazione.AnalisiTerreno.flag_cancellazione Then
                        AnalisiModelloBIZ.Scrivi_AnalisiTerreno_Modello(cancellazione.Piva,
                                                                        cancellazione.AnalisiTerreno.codice,
                                                                        cancellazione.AnalisiTerreno,
                                                                        objParametri_Super_Server,
                                                                        objParametri_Server,
                                                                        objParametri_Utenti,
                                                                        NoteLog:="Analisi_Modello.asmx")
                    End If
                Catch ex As GiasException
                    errori.Add("- " & descrizione & ": " & ex.Message)
                Catch ex As Exception
                    errori.Add(ex.Message)
                End Try
            Next

            If errori.Count > 0 Then
                errMsg = Gias.ErroreCancellazioneAnalisi & vbCrLf & String.Join(vbCrLf, errori)

                r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(New GiasException(errMsg))}
            End If

            r.RispostaOK = If(errMsg <> "", False, True)
            r.RispostaStringa = errMsg

        Catch ex As Exception
            errMsg = "Errore durante la cancellazione: " & ex.Message
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function LeggiListaAnalisiTerreno(InData As CoreWS_Generic(Of LeggiListaAnalisiTerreno)) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim str = JsonConvert.SerializeObject(InData)

        Dim iData As CoreWS_Generic(Of LeggiListaAnalisiTerreno) = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of LeggiListaAnalisiTerreno))(JsonConvert.SerializeObject(InData))

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAnalisiModello As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_R

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim applicaVisibilitaUtente As Boolean = False
            If Not iData.InData.ApplicaVisibilitaUMA Then
                Dim xUsrVis As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                applicaVisibilitaUtente = xUsrVis.Leggi(0, "", "", objParametri_Server).Rows.Count > 0
            End If

            Dim objTestata As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
            Dim DT As DataTable = objTestata.Leggi_ListaAnalisiTerreno_NG(0,
                                                                          applicaVisibilitaUtente,
                                                                          iData.InData,
                                                                          "",
                                                                          "",
                                                                          objParametri_Server,
                                                                          objParametri_Utenti)

            r.RispostaOK = True
            Dim rispostaJson = objAnalisiModello.DT_to_Json_AnalisiTerreno(DT)
            r.RispostaStringa = rispostaJson

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try
        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function LeggiLaboratori(objP_super_server As String, objP_server As String, objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim errMsg = ""

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim filtroLaboratori As String = ""
            Dim laboratori As String = "" 'LeggiLaboratoriUtente(objParametri_Server)

            If laboratori <> "" Then
                filtroLaboratori = "Risorse_Umane.Cod_RisUm IN (" & laboratori & ")"
            End If

            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dt = objContatti.Leggi(
                objParametri_Server.PivaSuperUser,
                "", 0, -8, True, False, 0, 0, False, 0, -99, 0, "", True, 0, 0, 0, 0, 0,
                CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE, False, filtroLaboratori, "",
                objParametri_Server)

            Dim ArrayLaboratori As New List(Of Contatto)
            For Each row In dt.Rows

                Dim laboratorio = New Contatto With {
                        .primaryKey = New Contatto.PK(row.Item("piva"), row.Item("cod_Contatto")),
                        .ragione_Sociale = row.Item("Rag_Soc"),
                        .nome = row.Item("Nome"),
                        .cognome = row.Item("Cognome"),
                        .codiceFiscale = row.Item("Codice_Fiscale")
                    }

                laboratorio.risorseUmane = New List(Of RisorseUmane) From {
                    New RisorseUmane(row.Item("cod_risum"))
                }

                ArrayLaboratori.Add(laboratorio)
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(ArrayLaboratori, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function LeggiSchemiAnalisi(InData As CoreWS_Generic(Of LeggiSchemiAnalisi)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim errMsg = ""

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

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

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        Try

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objTipologia As New AgronicaCoreAnagrafeDAL.Analisi_Tipologia_R
            Dim objAnParaRead As New AgronicaCoreAnagrafeDAL.Analisi_Parametri_R

            Dim ListaSchemi As New List(Of AnalisiTipologia)

            Dim dtGriglia As New DataTable

            If (InData.InData.LeggiMetaschema) Then
                dtGriglia = objTipologia.Leggi(InData.InData.Analisi_Tipologia_Cod,
                                               InData.InData.Analisi_Tipo,
                                               "Analisi_Tipologia.Analisi_Tipologia_Cod < 0", 'Prendo solo quelli di metaschema
                                               "",
                                               objParametri_Server)
            Else
                If InData.InData.Laboratorio <> 0 Then
                    dtGriglia = objTipologia.Leggi2(InData.InData.Analisi_Tipologia_Cod,
                                                    InData.InData.Analisi_Tipo,
                                                    InData.InData.Laboratorio,
                                                    "Analisi_Tipologia.Analisi_Tipologia_Cod > 0", 'Prendo solo quelli creati dagli utenti
                                                    "",
                                                    objParametri_Server)
                End If
            End If

            'Estraggo id dai risultati (ID_Tipologia)
            Dim ListaTipologie = From row In dtGriglia
                                 Select row.Field(Of Integer)("Analisi_Tipologia_Cod")
                                 Distinct

            Dim xFiltroAggiuntivo As String = ""

            If ListaTipologie IsNot Nothing And ListaTipologie.Count > 0 Then
                xFiltroAggiuntivo += " Analisi_Tipologia_Dettagli.Analisi_Tipologia_Cod in ( "
                For Each _tipologia In ListaTipologie
                    xFiltroAggiuntivo += _tipologia & ","
                Next
                xFiltroAggiuntivo = xFiltroAggiuntivo.Remove(xFiltroAggiuntivo.Count - 1)
                xFiltroAggiuntivo += ")"
            End If

            Dim parametri As DataTable = objAnParaRead.LeggiConTipologia(0,
                                                                         InData.InData.Analisi_Tipologia_Cod,
                                                                         xFiltroAggiuntivo,
                                                                         "",
                                                                         objParametri_Server)

            If dtGriglia.Rows.Count > 0 Then

                Dim objAnalisiModello As New AgronicaCoreAnagrafeBIZ.Analisi_Modello_R

                For Each _Schema In dtGriglia.Rows
                    Dim ListaParametri = New List(Of DettaglioTipologia)

                    Dim analisiTipologia = New AnalisiTipologia(_Schema.Item("Analisi_Tipologia_Cod")) With {
                        .descrizione = _Schema.Item("Analisi_Tipologia_Des"),
                        .descrizioneLunga = _Schema.Item("Analisi_Tipologia_Des_Long"),
                        .numeroDeterminazioni = _Schema.Item("Numero_Determinazioni")
                    }

                    Dim parametrixSchema = parametri.Select("Analisi_Tipologia_Cod = " & analisiTipologia.codice & " ").CopyToDataTable()

                    If parametrixSchema IsNot Nothing AndAlso parametrixSchema.Rows.Count > 0 Then

                        For Each _parametro In parametrixSchema.Rows

                            Dim analisiDettaglio = New DettaglioTipologia() With {
                                .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(_parametro.Item("Analisi_Parametro_Cod"), _parametro.Item("Analisi_Parametro_Des")) With {
                                    .unitaMisura = New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(_parametro.Item("Analisi_Parametro_Udm")) With {
                                        .descrizione = _parametro.Item("Analisi_Parametro_Udm_Des"),
                                        .simbolo = _parametro.Item("Analisi_Parametro_Udm_Sim")
                                    },
                                    .descrizione = _parametro.item("Analisi_Parametro_Des"),
                                    .min = _parametro.Item("Analisi_Parametro_ValoreMin"),
                                    .max = _parametro.Item("Analisi_Parametro_ValoreMax"),
                                    .simbolo = _parametro.Item("Analisi_Parametro_Simbolo"),
                                    .obbligatorio = objAnalisiModello.Check_ParametroObbligatorio(InData.InData.Analisi_Tipo, _parametro.Item("Analisi_Parametro_TipoAnalisi"))
                                }
                            }

                            ListaParametri.Add(analisiDettaglio)
                        Next

                    End If

                    analisiTipologia.dettagli = ListaParametri
                    ListaSchemi.Add(analisiTipologia)
                Next
            End If

            r.RispostaStringa = JsonConvert.SerializeObject(ListaSchemi, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    '########################################################################################

    <WebMethod(EnableSession:=True)>
    Public Function LeggiParametriAnalisi(InData As CoreWS_Generic(Of LeggiParametriAnalisiDettagli)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim errMsg = ""

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}

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

        Dim objParametri_Super_Server As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objParametri_Utenti As AgronicaCoreParametri

        '----- Definizione delle variabili
        Dim DTRs As DataTable
        Dim JArrayPA As New JArray()

        Dim DettaglioValido As Boolean
        Dim DettaglioObbligatorio As Boolean

        Dim objAnParaRead As New AgronicaCoreAnagrafeDAL.Analisi_Parametri_R

        Dim SenzaSchema As Boolean = False
        'Dim Analisi_Tipologia_Cod = InData.InData.Analisi_Tipologia_Cod
        Dim TipologiaAnalisi = InData.InData.Tipo_Analisi
        Dim ListaParametri As New List(Of DettaglioTipologia)

        Try

            objParametri_Super_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            objParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            DTRs = objAnParaRead.Leggi_con_iesimo(TipologiaAnalisi, True, True, objParametri_Server)

            If Not IsNothing(DTRs) AndAlso DTRs.Rows.Count > 0 Then

                For Each _parametro In DTRs.Rows
                    DettaglioValido = True
                    DettaglioObbligatorio = False

                    If Mid(_parametro.Item("Analisi_Parametro_TipoAnalisi"), TipologiaAnalisi, 1) < 1 Then
                        DettaglioValido = False
                    End If
                    If Mid(_parametro.Item("Analisi_Parametro_TipoAnalisi"), TipologiaAnalisi, 1) = 2 Then
                        DettaglioObbligatorio = True
                    End If


                    If DettaglioValido = True Then

                        Dim analisiDettaglio = New DettaglioTipologia() With {
                                .parametro = New AgronicaCoreModelsSTD.metaschema.AnalisiParametro(_parametro.Item("Analisi_Parametro_Cod")) With {
                                    .unitaMisura = New AgronicaCoreModelsSTD.metaschema.UnitaDiMisura(_parametro.Item("Analisi_Parametro_Udm")) With {
                                        .descrizione = _parametro.Item("Analisi_Parametro_Udm_Des"),
                                        .simbolo = _parametro.Item("Analisi_Parametro_Udm_Sim")
                                    },
                                    .descrizione = _parametro.item("Analisi_Parametro_Des"),
                                    .obbligatorio = DettaglioObbligatorio
                                }
                        }

                        ListaParametri.Add(analisiDettaglio)

                    End If

                Next

            End If

            r.RispostaStringa = JsonConvert.SerializeObject(ListaParametri, Formatting.None)
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gias.ErroreDuranteOperazione_ & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

End Class