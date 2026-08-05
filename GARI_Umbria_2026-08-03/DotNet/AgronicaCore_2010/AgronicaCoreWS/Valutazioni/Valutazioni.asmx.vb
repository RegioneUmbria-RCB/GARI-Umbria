Imports System.ComponentModel
Imports System.IO
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDTOStd.InData.Valutazioni
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreModelsSTD.valutazioni
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
<ToolboxItem(False)>
Public Class Valutazioni
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Testata(ByVal InData As CoreWS_Generic(Of LeggiTestata)) As rispostaStandard(Of List(Of Valutazione_Testata))

        Dim r As New rispostaStandard(Of List(Of Valutazione_Testata))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R
            Dim listItems As List(Of Valutazione_Testata) = objR.Leggi_Testata(InData.InData.piva, InData.InData.idTestata,
                                                                               objParametri,
                                                                               InData.InData.includiAnno,
                                                                               "", "")

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Testata_x_Griglia(ByVal InData As CoreWS_Generic(Of LeggiTestata)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R
            Dim dt As DataTable = objR.Leggi_Testata_x_Griglia(InData.InData.piva, InData.InData.idTestata,
                                                                      objParametri, "", "")

            Dim risp As String = JsonConvert.SerializeObject(dt)
            r.RispostaStringa = risp
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Scrivi_Valutazione_Testata(InData As Object) As rispostaStandard(Of Valutazione_Testata)

        Dim r As New rispostaStandard(Of Valutazione_Testata)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of Valutazione_Testata) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of Valutazione_Testata))(JsonConvert.SerializeObject(InData, settings), settings)

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

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W
            Dim xRisp = objW.ScriviModifica_Testata(iData.InData, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = iData.InData

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Scrivi_Valutazione_Dettaglio(InData As Object) As rispostaStandard(Of Valutazione_Dettaglio)

        Dim r As New rispostaStandard(Of Valutazione_Dettaglio)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of Valutazione_Dettaglio) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of Valutazione_Dettaglio))(JsonConvert.SerializeObject(InData, settings), settings)

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

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W
            Dim xRisp = objW.ScriviModifica_Dettaglio(iData.InData, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = iData.InData

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function




    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Modifica_Valutazione_Dettaglio_x_Griglia(InData As CoreWS_Generic(Of LeggiGriglia)) As rispostaStandard(Of Valutazione_Scrivi_Griglia)

        Dim r As New rispostaStandard(Of Valutazione_Scrivi_Griglia)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of Valutazione_Scrivi_Griglia) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of Valutazione_Scrivi_Griglia))(JsonConvert.SerializeObject(InData, settings), settings)

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

            If InData.InData.jsonGriglia <> "" AndAlso InData.InData.jsonGriglia.ToString <> "[]" Then

                Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W
                Dim xRisp = objW.ModificaDettaglio_x_Griglia(InData.InData.jsonGriglia, objParametri_Server)

            End If

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Modifica_Valutazione_Dettaglio_Specifico_x_Griglia(InData As CoreWS_Generic(Of LeggiGriglia)) As rispostaStandard(Of Valutazione_Scrivi_Griglia)

        Dim r As New rispostaStandard(Of Valutazione_Scrivi_Griglia)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of Valutazione_Scrivi_Griglia) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of Valutazione_Scrivi_Griglia))(JsonConvert.SerializeObject(InData, settings), settings)

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

            If InData.InData.jsonGriglia <> "" AndAlso InData.InData.jsonGriglia.ToString <> "[]" Then

                Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W
                Dim xRisp = objW.ModificaDettaglio_Specifico_x_Griglia(InData.InData.jsonGriglia, objParametri_Server)

            End If

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Scrivi_Valutazione_Dettaglio_Specifico(InData As Object) As rispostaStandard(Of Valutazione_Dettaglio_Specifico)

        Dim r As New rispostaStandard(Of Valutazione_Dettaglio_Specifico)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of Valutazione_Dettaglio_Specifico) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of Valutazione_Dettaglio_Specifico))(JsonConvert.SerializeObject(InData, settings), settings)

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

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W
            Dim xRisp = objW.ScriviModifica_Dettaglio_Specifico(iData.InData, False, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = iData.InData

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function




    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Aggiorna_Valutazione_Dettaglio(InData As Object) As rispostaStandard(Of Valutazione_Testata)

        Dim r As New rispostaStandard(Of Valutazione_Testata)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of LeggiTestata) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of LeggiTestata))(JsonConvert.SerializeObject(InData, settings), settings)

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

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W
            Dim xRisp = objW.Aggiorna_Dettaglio(iData.InData.piva, iData.InData.idTestata, objParametri_Server)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Aggiorna_Valutazione_Dettaglio_Specifico(InData As Object) As rispostaStandard(Of Valutazione_Testata)

        Dim r As New rispostaStandard(Of Valutazione_Testata)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of LeggiTestata) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of LeggiTestata))(JsonConvert.SerializeObject(InData, settings), settings)

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

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W
            Dim xRisp = objW.Aggiorna_Dettaglio_Specifico(iData.InData.piva, iData.InData.idTestata, objParametri_Server)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Aggiorna_Valutazione_Dettaglio_Singolo_Conto(InData As Object) As rispostaStandard(Of Valutazione_Testata)

        Dim r As New rispostaStandard(Of Valutazione_Testata)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of LeggiDettaglio) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of LeggiDettaglio))(JsonConvert.SerializeObject(InData, settings), settings)

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

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W


            Dim xRisp = objW.Aggiorna_Dettaglio(iData.InData.piva, iData.InData.idTestata, objParametri_Server, iData.InData.ContoCod)
            xRisp = objW.Aggiorna_Dettaglio_Specifico(iData.InData.piva, iData.InData.idTestata, objParametri_Server, iData.InData.ContoCod)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Aggiorna_Valutazione_Dettaglio_Arete(InData As Object) As rispostaStandard(Of Valutazione_Dettaglio_Specifico_Arete)

        Dim r As New rispostaStandard(Of Valutazione_Dettaglio_Specifico_Arete)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of LeggiDettaglioSpecificoArete) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of LeggiDettaglioSpecificoArete))(JsonConvert.SerializeObject(InData, settings), settings)

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

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W

            Dim jSonArete = Newtonsoft.Json.JsonConvert.DeserializeObject(iData.InData.jSonArete)

            For Each obj As JObject In jSonArete

                Select Case obj("tipo")

                    Case "MTO" 'Mezzi Tecnici

                        Select Case obj("categoria")
                            Case "MAT" 'Manodopera Terzisti
                            Case "MAD" 'Manodopera Dipendenti
                            Case "CAR" 'Carburante
                        End Select

                    Case "CPR" 'Costo Prodotto

                    Case "ALC" ' Altri Costi

                    Case "PRA"  'Post raccolta

                        Select Case obj("categoria")
                            Case "TRA" 'Trasporto,
                            Case ”STO” 'Stoccaggio,
                            Case ”ESS” 'Essicazione
                        End Select

                    Case "RIE" 'Riepilogo

                        Select Case obj("categoria")
                            Case "CTO" 'Costi Totali
                            Case "RTO"  'Ricavi Totali
                            Case "MAR" 'Margine per Coltura
                            Case "MPR" 'Margine % per Coltura
                            Case "CTP" 'Costo per prodotto
                            Case "RCP" 'Ricavo per Prodotto
                        End Select

                End Select

            Next

            Dim xRisp = objW.Aggiorna_Dettaglio_Specifico(iData.InData.piva, iData.InData.idTestata, objParametri_Server)

            r.RispostaOK = True


        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Dettaglio_Eco_x_Griglia(ByVal InData As CoreWS_Generic(Of LeggiDettaglio)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xFiltroAggiuntivo As String = "Valutazione_Sezione.Pat_Eco = 2"

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R
            Dim dt As DataTable = objR.Leggi_Dettaglio_x_Griglia(InData.InData.piva, InData.InData.idTestata, InData.InData.ContoCod,
                                                                 objParametri, xFiltroAggiuntivo, "")

            Dim risp As String = JsonConvert.SerializeObject(dt)
            r.RispostaStringa = risp
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function




    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Dettaglio_Pat_x_Griglia(ByVal InData As CoreWS_Generic(Of LeggiDettaglio)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xFiltroAggiuntivo As String = "Valutazione_Sezione.Pat_Eco = 1"

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R
            Dim dt As DataTable = objR.Leggi_Dettaglio_x_Griglia(InData.InData.piva, InData.InData.idTestata, InData.InData.ContoCod,
                                                                 objParametri, xFiltroAggiuntivo, "")

            Dim risp As String = JsonConvert.SerializeObject(dt)
            r.RispostaStringa = risp
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function





    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Dettaglio_Specifico_x_Griglia(ByVal InData As CoreWS_Generic(Of LeggiDettaglioSpecifico)) As RispostaStandard

        Dim r As New RispostaStandard
        Dim xFiltroAggiuntivo As String = ""

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R
            Dim dt As DataTable = objR.Leggi_Dettaglio_Specifico_x_Griglia(InData.InData.piva, InData.InData.idTestata, InData.InData.ContoCod,
                                                                           objParametri, xFiltroAggiuntivo, "")

            Dim risp As String = JsonConvert.SerializeObject(dt)
            r.RispostaStringa = risp
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiElencoTipoAnno(InData As Object) As rispostaStandard(Of List(Of ValutazioneTipoAnno))

        Dim r As New rispostaStandard(Of List(Of ValutazioneTipoAnno))

        Dim settings As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim iData As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), settings)


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

            Dim lista As New List(Of ValutazioneTipoAnno) From {
                    New ValutazioneTipoAnno With {.codice = 1, .descrizione = "Chiuso"},
                    New ValutazioneTipoAnno With {.codice = 2, .descrizione = "Aperto"},
                    New ValutazioneTipoAnno With {.codice = 3, .descrizione = "Previsionale"}
            }

            r.RispostaOK = True
            r.RispostaStringa = lista


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Piano_Conti(ByVal InData As CoreWS_Generic(Of LeggiPianoConti)) As rispostaStandard(Of List(Of Valutazione_Piano_Conti))

        Dim r As New rispostaStandard(Of List(Of Valutazione_Piano_Conti))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R
            Dim listItems As List(Of Valutazione_Piano_Conti) = objR.Leggi_Piano_Conti(InData.InData.piva, InData.InData.pianoCod,
                                                                                       objParametri,
                                                                                       "", "")

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Piano_Conti_x_Griglia(ByVal InData As CoreWS_Generic(Of LeggiPianoConti)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R
            Dim dt As DataTable = objR.Leggi_Piano_Conti_x_Griglia(InData.InData.piva, InData.InData.pianoCod,
                                                                   objParametri, "", "")

            Dim risp As String = JsonConvert.SerializeObject(dt)
            r.RispostaStringa = risp
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Scrivi_Piano_Conti(InData As Object) As rispostaStandard(Of Valutazione_Piano_Conti)

        Dim r As New rispostaStandard(Of Valutazione_Piano_Conti)

        Dim settings As New JsonSerializerSettings With {
                .DateTimeZoneHandling = DateTimeZoneHandling.Local
                }

        Dim iData As CoreWS_Generic(Of Valutazione_Piano_Conti) = JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of Valutazione_Piano_Conti))(JsonConvert.SerializeObject(InData, settings), settings)

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

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W
            Dim xRisp = objW.ScriviModifica_Piano_Conti(iData.InData, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = iData.InData

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Conti(ByVal InData As CoreWS_Generic(Of LeggiConto)) As rispostaStandard(Of List(Of Valutazione_Conto))

        Dim r As New rispostaStandard(Of List(Of Valutazione_Conto))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R
            Dim listItems As List(Of Valutazione_Conto) = objR.Leggi_Conto(InData.InData.ContoCod, InData.InData.SezioneCod,
                                                                                       objParametri,
                                                                                       "", "")

            r.RispostaStringa = listItems
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Piano_ContixConti_x_Tree(ByVal InData As CoreWS_Generic(Of LeggiPianoContixConti)) As rispostaStandard(Of List(Of TreeValutazionePianoContixConti))

        Dim r As New rispostaStandard(Of List(Of TreeValutazionePianoContixConti))
        Dim objR As New AgronicaCoreContabBIZ.Valutazioni_R

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)


            Dim ObjTreeValutazionePianoContixConti As List(Of TreeValutazionePianoContixConti) = objR.Leggi_Piano_ContixConti_Tree(InData.InData.piva, InData.InData.pianoCod, objParametri)

            r.RispostaStringa = ObjTreeValutazionePianoContixConti
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Scrivi_Piano_ContixConti_x_Tree(InData As Object) As rispostaStandard(Of TreeValutazionePianoContixConti)

        Dim r As New rispostaStandard(Of TreeValutazionePianoContixConti)

        Dim settings As New JsonSerializerSettings With {
            .DateTimeZoneHandling = DateTimeZoneHandling.Local
        }

        Dim iData As CoreWS_Generic(Of TreeValutazionePianoContixConti) = JsonConvert.DeserializeObject(
                Of CoreWS_Generic(Of TreeValutazionePianoContixConti))(JsonConvert.SerializeObject(InData, settings), settings)

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

            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objW As New AgronicaCoreContabBIZ.Valutazioni_W
            Dim xRisp = objW.ScriviModifica_Piano_ContixConti_Tree(iData.InData, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = iData.InData

        Catch ex As Exception

            r.RispostaOK = False

            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ReportExcel(ByVal InData As CoreWS_Generic(Of LeggiTestata)) As rispostaStandard(Of ValutazioneExcel)

        Dim r As New rispostaStandard(Of ValutazioneExcel)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim objParametri As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim listaFile As New List(Of String)
            Dim aziendeVuote As New List(Of String)
            Dim erroreDatatable As String
            Dim objspv = New AgronicaCoreContabBIZ.Valutazioni_W

            listaFile = objspv.creaReportExcel(InData.InData.piva, InData.InData.idTestata,
                                               objParametri, aziendeVuote, erroreDatatable)

            Dim objExcel As New ValutazioneExcel
            If listaFile.Count > 0 Then

                'prendo il primo
                'TODO: eliminare tutta questa parte, perchè tanto ne avrò uno solo

                Dim primoFile As String = listaFile(0)
                Dim fileInfo = New FileInfo(primoFile)


                objExcel.FileName = fileInfo.Name
                objExcel.Extension = fileInfo.Extension
                objExcel.Data = My.Computer.FileSystem.ReadAllBytes(primoFile)

                File.Delete(primoFile)


            End If

            r.RispostaOK = True
            r.RispostaStringa = objExcel

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


End Class



