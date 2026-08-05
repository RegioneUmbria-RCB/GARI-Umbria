Imports System.Web.Services
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDTOStd.InData.Profilazione
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModelsSTD.baseClass
Imports System.Net.Http


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<System.Web.Script.Services.ScriptService()>
Public Class ImpostazioniUtente
    Inherits WebService


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Carica_ImpostazioniUtente_Sezioni(InData As CoreWS_Generic(Of Leggi_Impostazioni)) As RispostaStandard
        Dim r As New RispostaStandard

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
            Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim utenti = InData.InData.Utenti
            Dim LEGGI_SOLO_IMPOSTAZIONI_SUPERUSER As Boolean = InData.InData.flagLetturaSuperUser

            Dim settings = objImpostazioni.CaricaSezioni_Impostazioni_Utente(
                InData.InData.flagLetturaSuperUser, objParametri_Utenti
            ).ToList()

            r.RispostaStringa = JsonConvert.SerializeObject(settings, Formatting.None)
            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Carica_ImpostazioniTemplate(InData As CoreWS_Generic(Of Leggi_Impostazioni)) As RispostaStandard
        Dim r As New RispostaStandard

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
            Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

            Dim users = InData.InData.Utenti
            Dim hash = New Dictionary(Of Integer, IEnumerable(Of AgronicaCoreModelsSTD.profilazione.ImpostazioneBase))

            Dim impostazioni = InData.InData.Impostazioni.GetEnumerator
            While (impostazioni.MoveNext)
                Dim dataItem = objImpostazioni.Carica_Impostazioni2(impostazioni.Current, objParametri_Utenti)
                hash.Add(impostazioni.Current, dataItem)
            End While
            If users.Length = 1 Then
                hash = objImpostazioni.LeggiDatiImpostazioniUtenteScalare(users.First, hash, objParametri_Utenti)
            End If
            Dim first = hash.First()

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(
                hash.AsEnumerable.Select(Function(x) New With {.Impostazione_Cod = x.Key, .data = x.Value}),
                Formatting.None
            )
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Carica_AreeGIAS(data As Object, objParametri_Utenti As String, objParametri_Server As String) As RispostaStandard
        Dim res As New RispostaStandard

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Utenti)

        Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

        Try
            Dim listItems = objImpostazioni.Carica_AreeGIAS(obj_Server)

            res.RispostaStringa = JsonConvert.SerializeObject(listItems, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    'Public Function Carica_AreeGIAS(data As Object, objParametri_Utenti As String, objParametri_Server As String) As RispostaStandard
    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Carica_AreeGIAS_NG(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim res As New RispostaStandard

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

        Try
            Dim listItems = objImpostazioni.Carica_AreeGIAS(obj_Server)

            res.RispostaStringa = JsonConvert.SerializeObject(listItems, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Salva_ImpostazioniUtenti(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.utente.Utente()))
        Dim res As New RispostaStandard

        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Try
            Dim objSettings As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_W
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim utenti As List(Of AgronicaCoreModelsSTD.utente.Utente) = InData.InData.ToList()

            For Each utente In utenti
                For Each i In utente.Impostazioni
                    Dim valueWasEmpty = String.IsNullOrEmpty(i.Valore)
                    objSettings.SalvaInfoAggiuntive(utente.Username, i.Impostazione_Cod, i.Valore, objParametri_Utenti)

                    objSettings.ScriviModificaImpostazioneUtente(
                        utente.Username, i.Impostazione_Cod,
                        objParametri_Utenti,
                        i.Valore)

                    If valueWasEmpty AndAlso objSettings.haFiltroMono(i.Impostazione_Cod) Then
                        Dim toDelete = New List(Of AgronicaCoreModelsSTD.utente.Utente)({
                            New AgronicaCoreModelsSTD.utente.Utente With {
                                .Username = utente.Username,
                                .Impostazioni = {i}.ToList
                            }
                        })
                        objSettings.CancellaImpostazioni(toDelete, objParametri_Server, objParametri_Utenti)
                    End If
                Next
            Next

            Dim gestoreCache As New AgronicaCoreVarieBIZ.GestoreCache
            Dim client As New HttpClient()

            gestoreCache.PulisciCacheImpostazioni(client, objParametri_Server)

            res.RispostaStringa = ""
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Copia_ImpostazioniUtente(InData As CoreWS_Generic(Of CopiaImpostazioniObj))
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If
        If InData.objP.objP_super_server = "" Then
            res.Errore = "objP_super_server non valorizzato"
            Return res
        End If

        Try
            Dim objSettings As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_W
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            objSettings.CopiaImpostazioni(InData.InData.Base, InData.InData.Template, objParametri_Server, objParametri_Utenti)



            res.RispostaStringa = ""
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function ResetDefault(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.utente.Utente()))
        Dim res As New RispostaStandard
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Try
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objSettings As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_W

            objSettings.CancellaImpostazioni(InData.InData, objParametri_Server, objParametri_Utenti)

            res.RispostaStringa = ""
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Carica_DatiDDL(data As Object, objParametri_Utenti As String, objParametri_Server As String) As RispostaStandard
        Dim res As New RispostaStandard

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objParametri_Utenti)

        Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

        Try

            Dim indata As Dictionary(Of String, Object) = data

            Dim user As String = indata.Last.Value(0)
            Dim Impostazione_Cod As Integer = indata.First.Value

            Dim listItems = objImpostazioni.Carica_DDL(Impostazione_Cod, user, obj_Server, obj_Utenti)

            res.RispostaStringa = JsonConvert.SerializeObject(listItems, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    'Public Function Carica_DatiDDL(data As Object, objParametri_Utenti As String, objParametri_Server As String) As RispostaStandard

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function Carica_DatiDDL_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.CaricaDatiDDL)) As RispostaStandard


        Dim res As New RispostaStandard

        Dim obj_Server As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_server)
        Dim obj_Utenti As AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

        Dim objImpostazioni As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

        Try


            Dim user As String = InData.InData.User
            Dim Impostazione_Cod As Integer = InData.InData.Impostazione_Cod

            'Dim indata As Dictionary(Of String, Object) = data

            'Dim user As String = indata.Last.Value(0)
            'Dim Impostazione_Cod As Integer = indata.First.Value

            Dim listItems = objImpostazioni.Carica_DDL(Impostazione_Cod, user, obj_Server, obj_Utenti)

            res.RispostaStringa = JsonConvert.SerializeObject(listItems, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function CaricaComboStampePreferite(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim res As New RispostaStandard
        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim ddl As New DropDownList
            Dim listItems As New List(Of BaseCodeDescrStr)
            AgronicaCoreUtility.CaricaListControl.StampeReport(
                ddl, False,
                "", "",
                "", "", objParametri_Server
            )
            Dim items As List(Of BaseCodeDescrStr) = (From item In ddl.Items
                                                      Select New BaseCodeDescrStr With {
                    .codice = item.Value, .descrizione = item.Text
                }).ToList

            res.RispostaStringa = JsonConvert.SerializeObject(items)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Function LeggiStampePreferite(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If
        If InData.objP.objP_super_server = "" Then
            res.Errore = "objP_super_server non valorizzato"
            Return res
        End If

        Dim objSettings As New AgronicaCoreUtentiBIZ.Utenti_Impostazioni_R

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim stampe = objSettings.LeggiStampePreferite(objParametri_Utenti, objParametri_Server)
            res.RispostaStringa = JsonConvert.SerializeObject(stampe)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

#Region "Guida Impostazioni"
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function AggiungiInTabellaGuida(InData As CoreWS_Generic(Of AgronicaCoreModelsSTD.profilazione.Impostazione()))
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If
        If InData.objP.objP_super_server = "" Then
            res.Errore = "objP_super_server non valorizzato"
            Return res
        End If

        Try
            Dim objImpostazioni As New AgronicaCoreMetaSchemaBIZ.GuidaImpostazioni
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim data = InData.InData

            objImpostazioni.AggiungiImpostazioneInTabellaGuida(data, objParametri_Server)

            res.RispostaStringa = JsonConvert.SerializeObject(data, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiSezioniGuida(InData As CoreWS_Generic(Of Integer))
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If
        If InData.objP.objP_super_server = "" Then
            res.Errore = "objP_super_server non valorizzato"
            Return res
        End If

        Try
            Dim objImpostazioni As New AgronicaCoreMetaSchemaBIZ.GuidaImpostazioni
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim data = objImpostazioni.LeggiSezioniGuida(objParametri_Server)

            res.RispostaStringa = JsonConvert.SerializeObject(data, Formatting.None)
            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try
        Return res
    End Function

#End Region

End Class

