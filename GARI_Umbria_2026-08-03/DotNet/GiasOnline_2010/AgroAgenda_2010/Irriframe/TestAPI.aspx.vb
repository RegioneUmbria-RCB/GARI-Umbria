Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json.Linq

Public Class TestAPI
    Inherits System.Web.UI.Page

    Const DebugToken As String = "*****" ' Inserire qui il token per il debug

    Public objParametriAgenda As ParametriAgenda
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Private Sub inizializzoObjParametri()
        objParametriAgenda = New ParametriAgenda
        '---
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Public Shadows ReadOnly Property Master() As AgroAgenda_2010.AgendaBootstrap
        Get
            Return CType(MyBase.Master, AgroAgenda_2010.AgendaBootstrap)
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        inizializzoObjParametri()

        Master.SetTitoloPaginaCustom("Test API Integrazione IF Next")

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistraUtente(ByVal Username As String, ByVal Password As String, ByVal Id_User As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim api As New IFNextApi

            Dim user As New JObject(
                New JProperty("id", Id_User),
                New JProperty("username", Username),
                New JProperty("password", Password))

            r.RispostaStringa = api.RegUser(DebugToken, user)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistraImpresa(ByVal Username As String, ByVal Piva As String, ByVal RagSoc As String, ByVal Cuaa As String, ByVal Id_Farm As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim api As New IFNextApi

            Dim farm As New JObject(
                New JProperty("id", Id_Farm),
                New JProperty("username", Username),
                New JProperty("ragSoc", RagSoc),
                New JProperty("piva", Piva),
                New JProperty("cuaa", Cuaa)
            )

            r.RispostaStringa = api.RegFarm(DebugToken, farm)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistraAppezzamento(ByVal Username As String, ByVal Descrizione As String,
                                                ByVal Latitudine As String, ByVal Longitudine As String,
                                                ByVal Superficie As String, ByVal Pendenza As String,
                                                ByVal Id_Farm As Integer, ByVal Id_Plot As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim api As New IFNextApi

            Dim plot As New JObject(
                New JProperty("id", Id_Plot),
                New JProperty("idFarm", Id_Farm),
                New JProperty("username", Username),
                New JProperty("description", Descrizione),
                New JProperty("lat", Latitudine),
                New JProperty("lng", Longitudine),
                New JProperty("sup", Superficie),
                New JProperty("slope", Pendenza)
            )

            r.RispostaStringa = api.RegPlot(DebugToken, plot)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistraImpianto(ByVal Username As String, ByVal Descrizione As String,
                                                ByVal Coltura As String, ByVal Ciclo As String,
                                                ByVal Data_Inizio As String, ByVal Data_Raccolta As String,
                                                ByVal Su_Fila As String, ByVal Tra_Fila As String,
                                                ByVal Conduzione As String, ByVal Vigore As String,
                                                ByVal Id_Plot As Integer, ByVal Id_Crop As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim api As New IFNextApi

            Dim crop As New JObject(
                New JProperty("id", Id_Crop),
                New JProperty("idPlot", Id_Plot),
                New JProperty("username", Username),
                New JProperty("description", Descrizione),
                New JProperty("coltura", Coltura),
                New JProperty("ciclo", Ciclo),
                New JProperty("dataInizio", Data_Inizio),
                New JProperty("dataRaccolta", Data_Raccolta),
                New JProperty("suFila", Su_Fila),
                New JProperty("traFila", Tra_Fila),
                New JProperty("conduzione", Conduzione),
                New JProperty("vigore", Vigore),
                New JProperty("anno", CDate(Data_Inizio).Year)
            )

            r.RispostaStringa = api.RegCrop(DebugToken, crop)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function RegistraIrrigazione(ByVal Piva As String, ByVal Id_Agenda As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim Irrigazioni As New List(Of Object)
            Dim NoteIntervento As String = ""

            Dim objAgenda As New Agenda_Operazione_Helper
            Dim Agenda = objAgenda.Leggi(Piva, 0, Id_Agenda, 0, objParametri_Server)

            If Not IsNothing(Agenda) AndAlso Not IsNothing(Agenda.Movimenti) Then
                For i = 0 To Agenda.Movimenti.Count - 1

                    'controllo corrispondenza piva con agenda
                    If Agenda.Piva <> Agenda.Movimenti(i).Piva Then
                        Throw New ApplicationException
                    End If

                    Select Case Agenda.Movimenti(i).Cau_Mov

                        Case enum_Agenda_Causali.LAVORAZIONE

                            Select Case CInt(Agenda.Lav_Cod)
                                Case LAVCOD_IRRIGAZIONE
                                    LeggiIrrigazioni(Agenda, i, Irrigazioni, NoteIntervento, objParametri_Server)
                                Case Else
                                    Throw New NotImplementedException
                            End Select

                        Case Else
                            Throw New NotImplementedException

                    End Select
                Next
            End If

            Dim api As New IFNextApi

            For Each irrigazione In Irrigazioni
                Dim irrigation As New JObject(New JProperty("Volumemm", CDec(irrigazione("Dose"))))
                r.RispostaStringa = api.RegIrrigation(irrigation)
            Next

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    Public Shared Sub LeggiIrrigazioni(ByRef agenda As Operazione_Agenda, ByRef i As Integer,
                                             ByRef irrigazioni As List(Of Object),
                                             ByRef NoteIntervento As String,
                                             ByVal objParametri_Server As AgronicaCoreParametri)

        NoteIntervento = agenda.Movimenti(i).Mov_Desc


        'MOVIMENTO_DETTAGLIO TECNICO (dovrebbe essere nothing)
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli_Tecnici) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli_Tecnici.Count - 1

                'controllo corrispondenza piva con agenda
                If agenda.Piva <> agenda.Movimenti(i).Piva Then
                    Throw New ApplicationException
                End If

                'non dovrebbe essercene nessuno
                Throw New NotImplementedException
            Next

        End If

        '---------------------
        '----MOVIMENTI_DETTAGLI
        '---------------------
        'uno per ciascun impianto influenzato dalle trappole installate
        If Not IsNothing(agenda.Movimenti(i).Movimenti_Dettagli) Then

            For j = 0 To agenda.Movimenti(i).Movimenti_Dettagli.Count - 1

                'controllo corrispondenza piva con agenda
                'sa cod è uguale ma potrebbe cambiare se...
                If agenda.Piva <> agenda.Movimenti(i).Movimenti_Dettagli(j).Piva Then
                    Throw New ApplicationException
                End If


                'ci deve essere un solo mov dettaglio tecnico per un mov dettaglio 
                If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici.Count <> 1 Then
                    Throw New ApplicationException
                End If


                'ci deve essere un solo mov destinaz tecnico per un mov dettaglio 
                If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni.Count <> 1 Then
                    Throw New ApplicationException
                End If

                '------------------------------
                '----- MOVIMENTO DET TECNICO --
                '------------------------------
                Dim dose As Decimal = 0
                Dim ore As Decimal = 0
                Dim portata As Integer = 0
                Dim QuaTot As Decimal = 0
                Dim Data_i As Date = AGRODATAINIZIO
                Dim Data_f As Date = AGRODATAFINE
                Dim frequenza As Integer = 0
                Dim TipoIrrigaz As Integer = 0
                Dim UDM As Integer = 0

                Select Case CInt(agenda.Lav_Cod)
                    Case LAVCOD_IRRIGAZIONE
                        dose = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Qta_Ril
                        ore = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Dose
                        portata = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Parziale
                        Data_i = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Inn1_data
                        Data_f = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Inn2_data
                        frequenza = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Nitrati
                        TipoIrrigaz = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).Freatimetro
                        UDM = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Dettagli_Tecnici(0).dett_cod
                    Case Else
                        Throw New NotImplementedException
                End Select

                '---------------------
                'MOVIMENTO_DESTINAZIONI
                '---------------------
                Dim objAppezzamento As New AgronicaCoreModello.ParametriAgenda_Temp.Impianto

                objAppezzamento.Piva = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Piva
                objAppezzamento.Sa_Cod = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Sa_Cod
                objAppezzamento.Appezza = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Appezza
                objAppezzamento.ID_Reg = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione

                Select Case CInt(agenda.Lav_Cod)
                    Case LAVCOD_IRRIGAZIONE
                        QuaTot = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta
                        If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2 > 0 Then
                            objAppezzamento.Qta2 = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2
                        Else
                            objAppezzamento.Qta2 = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).LeggiSuperficie(objAppezzamento.Piva, objAppezzamento.Sa_Cod, objAppezzamento.Appezza, objAppezzamento.ID_Reg, objParametri_Server)
                        End If

                    Case Else
                        Throw New NotImplementedException
                End Select

                Dim objImp As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                Dim Dt_Imp As New DataTable
                Dt_Imp = objImp.Leggi(agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Piva,
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Sa_Cod,
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Appezza,
                             agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Id_Destinazione,
                             enumSelezioneVariabile.Selezione_JoinDescrizioni,
                             "", "", objParametri_Server)

                '----------------------------------------------------------------------
                '----------------Creo un oggetto per irrigazione impianto------------------
                '----------------------------------------------------------------------
                Dim qta_2 As Decimal = 0

                If agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2 <> 0 Then
                    qta_2 = agenda.Movimenti(i).Movimenti_Dettagli(j).Movimenti_Destinazioni(0).Qta2
                Else
                    qta_2 = (New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read()).LeggiSuperficie(objAppezzamento.Piva, objAppezzamento.Sa_Cod, objAppezzamento.Appezza, objAppezzamento.ID_Reg, objParametri_Server)
                End If


                irrigazioni.Add(New With
                                    {
                                        .ImpiantoIrrigato = objAppezzamento.Piva.ToString & "-" & objAppezzamento.Sa_Cod.ToString & "-" & objAppezzamento.Appezza.ToString & "-" & objAppezzamento.ID_Reg.ToString,
                                        .Dose = dose,
                                        .Ore = ore,
                                        .Portata = portata,
                                        .Data_Inizio = Data_i,
                                        .Data_Fine = Data_f,
                                        .Frequenza = frequenza,
                                        .Qta_Totale = QuaTot,
                                        .UDM_Dose = UDM,
                                        .TipoIrrigazioneUtilizzata = TipoIrrigaz,
                                        .Qta2 = qta_2
                                    })



            Next

        Else
            'ci deve essere almeno un movimento dettaglio per l'installazione di una trappola in un appezzamento
            Throw New ApplicationException
        End If

    End Sub

End Class