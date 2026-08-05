Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDTOStd.InData.AgronicaCoreUtility

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class CaricaListControl
    Inherits System.Web.Services.WebService

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiSpecieColtivate_NG(InData As CoreWS_Generic(Of LeggiSpecieColtivate)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim ddlSpecie As New DropDownList
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.TutteSpecieColtivate_3(ddlSpecie,
                                                        InData.InData.PrimaRiga_Flag, InData.InData.PrimaRiga_Text, InData.InData.PrimaRiga_Value,
                                                        InData.InData.Piva, InData.InData.Sa_Cod, InData.InData.Data_Da, InData.InData.Data_A, InData.InData.ConsideraTerrenoNudo,
                                                        "", "", objParametriServer, InData.InData.leggiAncheBloccati)

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddlSpecie.Items
                JArrayListaOp.Add(New JObject(New JProperty("veg_des", i.Text), New JProperty("veg_cod", i.Value)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiSpecieColtivate(ByVal objP_server As String,
                                         ByVal PrimaRiga_Flag As Boolean,
                                         ByVal PrimaRiga_Text As String,
                                         ByVal PrimaRiga_Value As String,
                                         ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Data_Da As Date,
                                         ByVal Data_A As Date,
                                         ByVal ConsideraTerrenoNudo As Boolean,
                                         ByVal leggiAncheBloccati As Boolean
                                         ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim ddlSpecie As New DropDownList
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.TutteSpecieColtivate_3(ddlSpecie,
                                                        PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                        Piva, Sa_Cod, Data_Da, Data_A, ConsideraTerrenoNudo,
                                                        "", "", objParametriServer, leggiAncheBloccati)

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddlSpecie.Items
                JArrayListaOp.Add(New JObject(New JProperty("veg_des", i.Text), New JProperty("veg_cod", i.Value)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function
    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiSezionali(ByVal objP_server As String,
                                   ByVal PrimaRiga_Flag As Boolean,
                                   ByVal PrimaRiga_Text As String,
                                   ByVal PrimaRiga_Value As String,
                                   ByVal Tipo_Value As Integer,
                                   ByVal Piva As String,
                                   ByVal Sezionale_Cod As Integer
                                   ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim ddlSezionali As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Imprese_Sezionali(ddlSezionali,
                                                        PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                        Tipo_Value, Piva, Sezionale_Cod,
                                                        "", "", objParametriServer)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddlSezionali.Items
                jArrayListaOp.Add(New JObject(New JProperty("Sezionale_Des", i.Text),
                                              New JProperty("Sezionale_Cod", i.Value)))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiAspettoBeni(ByVal objP_server As String,
                                     ByVal PrimaRiga_Flag As Boolean,
                                     ByVal PrimaRiga_Text As String,
                                     ByVal PrimaRiga_Value As String,
                                     ByVal moduloGias As Integer
                                     ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim ddlAspettoBeni As New DropDownList
            AgronicaCoreUtility.CaricaListControl.AspettoBeni(ddlAspettoBeni,
                                                              PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                              moduloGias,
                                                              objParametriServer)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddlAspettoBeni.Items
                jArrayListaOp.Add(New JObject(New JProperty("AspettoBene_Des", i.Text),
                                              New JProperty("AspettoBene_Cod", i.Value)))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiCausaliTrasporto(ByVal objP_server As String,
                                          ByVal PrimaRiga_Flag As Boolean,
                                          ByVal PrimaRiga_Text As String,
                                          ByVal PrimaRiga_Value As String,
                                          ByVal moduloGias As Integer,
                                          ByVal lavCod As Integer
                                          ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim ddlCausaliTrasporto As New DropDownList
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.CausaliTrasporto(ddlCausaliTrasporto,
                                                                   PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                                   moduloGias, lavCod,
                                                                   objParametriServer)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddlCausaliTrasporto.Items
                jArrayListaOp.Add(New JObject(New JProperty("Causale_Trasporto_Des", i.Text),
                                              New JProperty("Causale_Trasporto_Cod", i.Value)))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function



    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiSigleAE(ByVal objP_server As String,
                                ByVal PrimaRiga_Flag As Boolean,
                                ByVal PrimaRiga_Text As String,
                                ByVal PrimaRiga_Value As String,
                                ByVal lavCod As Integer
                                ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim ddlSigleAE As New DropDownList
            Dim clc = New AgronicaCoreUtility.CaricaListControl
            clc.SigleAE(ddlSigleAE,
                        PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                        lavCod,
                        objParametriServer)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddlSigleAE.Items
                jArrayListaOp.Add(New JObject(New JProperty("xDescrizione", i.Text),
                                              New JProperty("Codice", i.Value)))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiGestioneVettore(ByVal objP_server As String,
                                         ByVal PrimaRiga_Flag As Boolean,
                                         ByVal PrimaRiga_Text As String,
                                         ByVal PrimaRiga_Value As String,
                                         ByVal flagEstero As Boolean
                                         ) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim ddlGestioneVettore As New DropDownList
            AgronicaCoreUtility.CaricaListControl.GestioneVettore(ddlGestioneVettore,
                                                                  PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                                  flagEstero,
                                                                  objParametriServer)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddlGestioneVettore.Items
                jArrayListaOp.Add(New JObject(New JProperty("GestioneVettore_Des", i.Text),
                                              New JProperty("GestioneVettore_Cod", i.Value)))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

End Class