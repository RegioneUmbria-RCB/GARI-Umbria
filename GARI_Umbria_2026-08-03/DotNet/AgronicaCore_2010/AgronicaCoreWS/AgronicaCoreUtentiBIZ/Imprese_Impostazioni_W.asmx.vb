Imports System.ComponentModel
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDTOStd.InData
Imports AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
Imports AgronicaCoreDTOStd.InData.Profilazione
Imports AgronicaCoreModelsSTD.profilazione
Imports AgronicaCoreModelsSTD.utente
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Imprese_Impostazioni_W
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Salva le stesse impostazioni per ogni azienda passata.
    ''' </summary>
    ''' <param name="InData">Oggetto di tipo {imprese[], impostazioni[]}.</param>
    ''' <returns></returns>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaImpostazioni_AziendeCentri_NG(InData As CoreWS_Generic(Of SalvaImpostazioni_AziendeCentri)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_W
        Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim saveSettings = Sub(piva As String, Sa_Cod As Integer)
                                   For Each impostazione In InData.InData.impostazioni
                                       objSettings.ScriviModifica_Impostazione(piva, Sa_Cod,
                                                        impostazione.Impostazione_Cod, impostazione.Valore,
                                                        objParametri_Server)
                                   Next
                               End Sub

            For Each azienda In InData.InData.imprese

                'If azienda.Sa_Cod = 0 Then
                '    'Salvo le impostazioni per ogni centro
                '    Dim piva() As String = {azienda.piva}
                '    Dim centri = objCentri.Carica_centriXImprese(piva.ToList(), objParametri_Server, objParametri_Utenti).ToList()
                '    For Each centro In centri
                '        saveSettings.Invoke(azienda.piva, centro.sa_cod)
                '    Next
                'End If

                saveSettings.Invoke(azienda.piva, azienda.Sa_Cod)

            Next

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

    <Obsolete("This method is deprecated, use SalvaImpostazioni_AziendeCentri_NG instead.")>
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function SalvaImpostazioni_AziendeCentri(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_W
        Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim impostazioni As Object() = InData.InData.item("impostazioni")
            Dim imprese As Object() = InData.InData.item("imprese")

            Dim settingsList = (From i In impostazioni
                                Select (New AgronicaCoreModelsSTD.profilazione.Impostazione With {
                           .Data_Creazione = i.item("Data_Creazione"),
                           .Data_Modifica = i.item("Data_Modifica"),
                           .Data_Invio = i.item("Data_Invio"),
                           .Impostazione_AziendaCentro = i.item("Impostazione_AziendaCentro"),
                           .Impostazione_AziendaCentroSpecie = i.item("Impostazione_AziendaCentroSpecie"),
                           .Impostazione_Cod = i.item("Impostazione_Cod"),
                           .Impostazione_Des = i.item("Impostazione_Des"),
                           .Impostazione_SuperUser = i.item("Impostazione_SuperUser"),
                           .Inviato = i.item("Inviato"),
                           .Note = i.item("Note"),
                           .Ordine = i.item("Ordine"),
                           .Tipo_Campo = i.item("Tipo_Campo"),
                           .Sezione_Cod = i.item("Sezione_Cod"),
                           .Sezione_Des = i.item("Sezione_Des"),
                           .SottoSezione_Cod = i.item("SottoSezione_Cod"),
                           .SottoSezione_Des = i.item("SottoSezione_Des"),
                           .Livello_Cod = i.item("Livello_Cod"),
                           .Livello_Des = i.item("Livello_Des"),
                           .Username_Creazione = i.item("Username_Creazione"),
                           .Username_Modifica = i.item("Username_Modifica"),
                           .Validita_Inizio = i.item("Validita_Inizio"),
                           .Validita_Fine = i.item("Validita_Fine"),
                           .codice = i.item("codice"),
                           .descrizione = i.item("descrizione"),
                           .value = i.item("value")
                        })).ToList()


            For Each azienda In imprese

                If azienda.item("Sa_Cod") = 0 Then
                    Dim piva() As String = {azienda.item("piva")}
                    Dim centri = objCentri.Carica_centriXImprese(piva.ToList(), objParametri_Server, objParametri_Utenti)
                    Dim centro = azienda
                    For Each c In centri
                        centro.item("Sa_Cod") = c.sa_cod
                        centro.item("Sa_Nome") = c.sa_nome

                        objSettings.SalvaImpostazioni_Aziende_Centri(centro, settingsList, objParametri_Server)
                    Next
                    Continue For
                End If

                objSettings.SalvaImpostazioni_Aziende_Centri(azienda, settingsList, objParametri_Server)
            Next

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaImpostazioni_AziendeCentri(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_W
        Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim imprese As Object() = InData.InData
            objSettings.CancellaImpostazioni_Azienda_Centri(imprese, objParametri_Server)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaImpostazioni_AziendeCentri_NG(InData As CoreWS_Generic(Of Imprese_Impostazioni())) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_W

            objSettings.CancellaImpostazioni_AziendeCentri(InData.InData, objParametri_Server)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaImpostazione_AziendeCentri(InData As CoreWS_Generic(Of Object)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_W
        Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim imprese As IDictionary(Of String, Object) = InData.InData.item("impresa")
            Dim Impostazione_Cod = InData.InData.item("impostazione")

            objSettings.CancellaImpostazione_Azienda_Centri(imprese, Impostazione_Cod, objParametri_Server)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CancellaImpostazione_AziendeCentri_NG(InData As CoreWS_Generic(Of CancellaImpostazione_AziendeCentri)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_W
        Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim imprese As Object = InData.InData.impresa
            Dim Impostazione_Cod = InData.InData.Impostazione_Cod

            objSettings.CancellaImpostazione_Azienda_Centri_NG(imprese, Impostazione_Cod, objParametri_Server)

            res.RispostaOK = True
        Catch ex As Exception
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CopiaImpostazioniAziende(InData As CoreWS_Generic(Of CopiaImpostazioniObj)) As RispostaStandard
        Dim res As New RispostaStandard

        If InData.objP.objP_server = "" Then
            res.Errore = "objP_server non valorizzato"
            Return res
        End If
        If InData.objP.objP_utenti = "" Then
            res.Errore = "objP_utenti non valorizzato"
            Return res
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri
        Dim objParametri_Server As AgronicaCoreParametri
        Dim objSettings As New AgronicaCoreAnagrafeBIZ.Imprese_Impostazioni_W

        Try
            objParametri_Utenti = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            objParametri_Server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim isFormattedOk = Function(str As String) str <> "" AndAlso str.Contains("_")
            Dim basi = InData.InData.Base.GetEnumerator
            Dim template = New ImpresaDto With {
                .piva = InData.InData.Template.Split("_")(0),
                .Sa_Cod = Convert.ToInt32(InData.InData.Template.Split("_")(1))
            }

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

            While basi.MoveNext
                If Not isFormattedOk(basi.Current) OrElse Not isFormattedOk(InData.InData.Template) Then
                    Throw New Exception("Keys not correctly formatted")
                End If
                Dim base = New ImpresaDto With {
                    .piva = basi.Current.Split("_")(0),
                    .Sa_Cod = Convert.ToInt32(basi.Current.Split("_")(1))
                }
                objSettings.CopiaImpostazioni(
                    base, template,
                    objParametri_Utenti,
                    objParametri_Server,
                    False
                )
            End While

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(1, objParametri_Server)
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)

            res.RispostaOK = True
        Catch ex As Exception
            If objParametri_Server IsNot Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessione(objParametri_Server)
            End If
            res.RispostaOK = False
            res.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return res

    End Function

End Class