Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Widgets
Imports AgronicaCoreModelsSTD.Widgets
Imports System.Data.SqlClient
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class Utenti_Widgets
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ElencoWidgets(ByVal InData As Object) As rispostaStandard(Of List(Of Widget))

        Dim r As New rispostaStandard(Of List(Of Widget))

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widgets_In) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widgets_In))(JsonConvert.SerializeObject(InData), a)

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_super_server = "" Then
            r.Errore = "objP_super_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_utenti)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_super_server)

            Dim objUtenti As New AgronicaCoreUtentiBIZ.Widgets
            Dim objMeta As New AgronicaCoreMetaSchemaDAL.Widgets_R
            Dim widgets = objUtenti.LeggiWidgets(params.InData.UserName,
                                          objParametri_Utenti)

            'se viene chiamata con UserName -1, mi basta la risposta appena sopra, non devo procedere con le seguenti considerazioni
            If Not IsNothing(widgets) AndAlso (params.InData.UserName <> CostantiPersonalizzate.UserDefaultWidgets) Then
                If widgets.Count = 0 Then

                    ' Se all'utente non è ancora associato nessun widget precarico quelli con presetIniziale
                    ' controllando che prima magari sia presente la configurazione per UserName -1
                    Dim widgetsForUsers = objUtenti.LeggiWidgets(CostantiPersonalizzate.UserDefaultWidgets,
                                                                objParametri_Utenti)

                    Dim dt As DataTable = objMeta.Leggi("", "", "",
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "", objParametri_Server, ,, True)

                    ' se esiste una configurazione per utente -1, allora aggiorna il dataTable da riportare con i widget di default
                    If widgetsForUsers.Any() Then
                        Dim listIDWidget = widgetsForUsers.Select(Function(x) x.IdWidget).ToList()

                        dt = dt.AsEnumerable().Where(Function(row) listIDWidget.Contains(row.Field(Of Integer)("IdWidget"))).
                                                CopyToDataTable()

                        For Each row As DataRow In dt.Rows
                            Dim widget = widgetsForUsers.FirstOrDefault(Function(w) w.IdWidget = row.Field(Of Integer)("IdWidget"))
                            If widget IsNot Nothing Then
                                row("Aspetto") = widget.Aspetto
                                row("Parametri") = widget.Parametri
                                row("Abilitato") = widget.Abilitato
                                row("Visibile") = widget.Visibile
                            End If
                        Next

                    End If

                    If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                        Dim isSuperUser As Boolean = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername)
                        Dim userName As String = If(isSuperUser, objParametri_Server.SuperUserUsername, objParametri_Server.UtenteUsername)
                        Dim widgetsToInsert As New List(Of Widget)

                        For Each row As DataRow In dt.Rows
                            Dim w As New Widget With
                                   {
                                        .IdWidget = Convert.ToInt32(row.Item("IdWidget")),
                                       .UserName = userName,
                                       .Abilitato = Convert.ToBoolean(row.Item("Abilitato")),
                                       .Visibile = Convert.ToBoolean(row.Item("Visibile")),
                                       .RichiedeAziendaSelezionata = Convert.ToBoolean(row.Item("RichiedeAziendaSelezionata")),
                                       .Aspetto = IIf(row.Item("Aspetto") Is DBNull.Value, "", row.Item("Aspetto")),
                                       .Parametri = IIf(row.Item("Parametri") Is DBNull.Value, "", row.Item("Parametri")),
                                       .Codice = IIf(row.Item("Codice") Is DBNull.Value, "", row.Item("Codice")),
                                       .Titolo = IIf(row.Item("Titolo") Is DBNull.Value, "", row.Item("Titolo")),
                                       .Descrizione = IIf(row.Item("Descrizione") Is DBNull.Value, "", row.Item("Descrizione")),
                                       .UserNameCreazione = objParametri_Server.SuperUserUsername,
                                       .UserNameModifica = objParametri_Server.SuperUserUsername
                                   }
                            widgetsToInsert.Add(w)
                        Next
                        ApplicaPermessi(widgetsToInsert, objParametri_Server, objParametri_Utenti)
                        Dim widgetPermessi = widgetsToInsert.Where(Function(w) w.Abilitato).ToList()
                        objUtenti.PresetInizialeWidgetUtente(widgetPermessi, objParametri_Utenti)

                        widgets = objUtenti.LeggiWidgets(params.InData.UserName,
                                              objParametri_Utenti,
                                              params.InData.Visibile,
                                              params.InData.Abilitato)

                    End If

                Else
                    ApplicaPermessi(widgets, objParametri_Server, objParametri_Utenti)
                    widgets = widgets.Where(Function(w) w.Abilitato = params.InData.Abilitato).ToList
                    If Not IsNothing(params.InData.Visibile) AndAlso params.InData.Visibile.HasValue Then
                        widgets = widgets.Where(Function(w) w.Visibile = params.InData.Visibile).ToList
                    End If
                End If

            End If

            r.RispostaStringa = widgets
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    Private Sub ApplicaPermessi(ByRef widgets As List(Of Widget),
                                ByRef objParametriServer As AgronicaCoreParametri,
                                ByVal objParametriUtenti As AgronicaCoreParametri)

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim isSuperUser As Boolean = (objParametriServer.SuperUserUsername = objParametriServer.UtenteUsername)

        Try
            ' Controllo dei permessi
            For Each w As Widget In widgets

                If Not isSuperUser Then
                    If Not String.IsNullOrEmpty(w.Parametri) Then
                        Dim pb = JsonConvert.DeserializeObject(Of WidgetParametriBase)(w.Parametri)

                        ' prima controllo se permessi multipli
                        If Not IsNothing(pb) AndAlso Not String.IsNullOrEmpty(pb.ListaCodPermessi) AndAlso Not String.IsNullOrWhiteSpace(pb.ListaCodPermessi) Then
                            Dim codiciPermessi = pb.ListaCodPermessi.Split(",")
                            If codiciPermessi.Count > 0 Then
                                Dim almenoUnoAbilitato As Boolean = False
                                For Each cp In codiciPermessi
                                    If IsNumeric(cp.Trim) Then
                                        almenoUnoAbilitato = ObjUtenti.Controlla_Permessi_Utente(
                                                            objParametriUtenti.UtenteUsername, 5,
                                                            CInt(cp), enum_Security_Operazione.Lettura, Date.Now, "", objParametriUtenti)
                                        If almenoUnoAbilitato Then
                                            Exit For
                                        End If
                                    End If
                                Next
                                w.Abilitato = If(almenoUnoAbilitato, True, False)
                            End If
                        End If

                        ' permesso singolo
                        If Not IsNothing(pb) AndAlso pb.CodPermesso.HasValue AndAlso pb.CodPermesso <> -1 Then

                            Dim abilitatoLettura = ObjUtenti.Controlla_Permessi_Utente(
                            objParametriUtenti.UtenteUsername, 5,
                            pb.CodPermesso, enum_Security_Operazione.Lettura, Date.Now, "", objParametriUtenti)
                            w.Abilitato = abilitatoLettura

                        End If
                    End If
                Else
                    w.Abilitato = True
                End If
            Next
        Catch ex As Exception
            For Each w As Widget In widgets
                w.Abilitato = True
            Next
        End Try


    End Sub

       <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ScriviAggiornaWidgetsUtente(ByVal InData As Object) As rispostaStandard(Of List(Of String))
        Dim r As New rispostaStandard(Of List(Of String))
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Aggiorna_Widgets_In) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Aggiorna_Widgets_In))(JsonConvert.SerializeObject(InData), a)

        Dim risposte As New List(Of String)

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParams As New ObjParams With {
                .objParametri_Utenti = Utility.convertStringtoOBJparametri(params.objP.objP_utenti),
                .objParametri_server = Utility.convertStringtoOBJparametri(params.objP.objP_server),
                .ObjParametri_SuperServer = Utility.convertStringtoOBJparametri(params.objP.objP_super_server)
            }
            Dim objUtenti As New AgronicaCoreUtentiBIZ.Widgets
            risposte = objUtenti.scriviAggiornaWidgetsUtente(params.InData.widgets, objparams)

            r.RispostaStringa = risposte
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AggiornaWidgetsUtente(ByVal InData As Object) As rispostaStandard(Of List(Of String))
        Dim r As New rispostaStandard(Of List(Of String))
        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Aggiorna_Widgets_In) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Aggiorna_Widgets_In))(JsonConvert.SerializeObject(InData), a)

        Dim risposte As New List(Of String)

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_utenti)
            Dim objUtenti As New AgronicaCoreUtentiBIZ.Widgets

            For Each w As Widget In params.InData.widgets
                risposte.Add(objUtenti.AggiornaWidgetUtente(w, objParametri_Utenti))
            Next

            r.RispostaStringa = risposte
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function UserWidgetsReset(ByVal InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of String) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData), a)

        Dim risposte As New List(Of String)

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If
        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_utenti)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objUtenti As New AgronicaCoreUtentiBIZ.Widgets
            Dim objMetaschema As New AgronicaCoreMetaSchemaBIZ.Widgets

            Dim systemWidgets = objMetaschema.LeggiWidgets("", "", "",
                                                           objParametri_Server, objParametri_Utenti,
                                                           Nothing, True, True)

            ' verifichiamo che non esista alcun record con username -1
            Dim widgetsForUsers = objUtenti.LeggiWidgets(CostantiPersonalizzate.UserDefaultWidgets,
                                                         objParametri_Utenti)

            If widgetsForUsers.Any() Then
                Dim listIDWidgetsUsers = widgetsForUsers.Select(Function(x) x.IdWidget).ToList()
                systemWidgets = systemWidgets.Where(Function(obj) listIDWidgetsUsers.Contains(obj.IdWidget)).ToList()

                For Each objWidget In systemWidgets
                    Dim elementoDaModificare = widgetsForUsers.Find(Function(f) f.IdWidget = objWidget.IdWidget)
                    If elementoDaModificare IsNot Nothing Then
                        objWidget.Aspetto = elementoDaModificare.Aspetto
                        objWidget.Parametri = elementoDaModificare.Parametri
                        objWidget.Abilitato = elementoDaModificare.Abilitato
                        objWidget.Visibile = elementoDaModificare.Visibile
                    End If
                Next

            End If

            If Not IsNothing(systemWidgets) AndAlso systemWidgets.Any Then
                objUtenti.CancellaWidgets(params.InData, objParametri_Utenti)

                ApplicaPermessi(systemWidgets, objParametri_Server, objParametri_Utenti)
                Dim widgetPermessi = systemWidgets.Where(Function(w) w.Abilitato).ToList
                For Each w As Widget In widgetPermessi
                    w.UserName = params.InData
                    risposte.Add(objUtenti.ScriviWidgetUtente(w, objParametri_Utenti))
                Next
            End If

            r.RispostaStringa = "OK"
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function AggiornaConfigurazioneWidget(ByVal InData As Object) As rispostaStandard(Of String)

        Dim r As New rispostaStandard(Of String)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim params As CoreWS_Generic(Of Widget_Configuration) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Widget_Configuration))(JsonConvert.SerializeObject(InData), a)

        Dim risposte As New List(Of String)

        If params.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If params.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_utenti)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(params.objP.objP_server)
            Dim objUtenti As New AgronicaCoreUtentiBIZ.Widgets
            Dim objMetashcema As New AgronicaCoreMetaSchemaBIZ.Widgets

            Dim widget As New Widget With
            {
                .Abilitato = params.InData.Abilitato,
                .UserName = params.InData.UserName,
                .Visibile = params.InData.Visibile,
                .IdWidget = params.InData.IdWidget
            }
            If String.IsNullOrEmpty(widget.UserName) OrElse widget.UserName = CostantiPersonalizzate.UserDefaultWidgets Then
                widget.UserName = objParametri_Server.UtenteUsername
            End If

            ' Verifico se il record è già presente nella tabella utentiu_widgets per l'utente
            Dim dt As DataTable = objUtenti.Leggi(widget.IdWidget, widget.UserName, objParametri_Utenti)
            If Not IsNothing(dt) AndAlso dt.Rows.Count = 0 Then
                ' Vado a recuperare i setting di default da tabella di metaschema
                Dim metaWidgets = objMetashcema.Leggi(widget.IdWidget, objParametri_Server)
                If Not IsNothing(metaWidgets) Then
                    widget.Parametri = metaWidgets.Parametri
                    widget.Aspetto = metaWidgets.Aspetto
                End If

                objUtenti.ScriviWidgetUtente(widget, objParametri_Utenti)
            Else
                ' Aggiorno
                objUtenti.AggiornaWidgetUtente(widget, objParametri_Utenti)
            End If

            r.RispostaStringa = "OK"
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r


    End Function


End Class

