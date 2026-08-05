Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
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
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class Reg_Impianto
    Inherits System.Web.Services.WebService

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CaricaImpiantiEsistenti_GIS(ByVal objP_server As String, ByVal Piva As String, ByVal Sa_Cod As String, ByVal Veg_Cod As String, ByVal isSementi As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            Dim xRisp As String = AgronicaControlliGIS.V_M.CaricaImpiantiEsistenti(Piva, Sa_Cod, Veg_Cod, CBool(isSementi), False, objParametri_Server)
            Dim ddlImpianto As New DropDownList

            xRisp = "<a>" & xRisp.Replace("&", "&amp;") & "</a>"
            Dim dRisp As XDocument = XDocument.Parse(xRisp)

            For Each vv In dRisp.Element("a").Elements("option")
                ddlImpianto.Items.Add(New ListItem(vv.Value, vv.Attribute("value")))
            Next

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddlImpianto.Items, "Id_Reg", "Reg_Descr")
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)> <Script.Services.ScriptMethod()>
    Public Function CaricaImpiantiEsistenti_GIS_NG(InData As CoreWS_Generic(Of CaricaImpiantiEsistenti_GIS)) As RispostaStandard
        Dim r As New RispostaStandard


        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

        Try

            Dim xRisp As String = AgronicaControlliGIS.V_M.CaricaImpiantiEsistenti(InData.InData.Piva,
                                                                                   InData.InData.Sa_Cod,
                                                                                   InData.InData.Veg_Cod,
                                                                                   InData.InData.isSementi,
                                                                                   False,
                                                                                   objParametri_Server)



            Dim ddlImpianto As New DropDownList

            xRisp = "<a>" & xRisp.Replace("&", "&amp;") & "</a>"
            Dim dRisp As XDocument = XDocument.Parse(xRisp)

            For Each vv In dRisp.Element("a").Elements("option")
                ddlImpianto.Items.Add(New ListItem(vv.Value, vv.Attribute("value")))
            Next

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddlImpianto.Items, "Id_Reg", "Reg_Descr")
            r.RispostaOK = True

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CaricaCombo_SpecieColtivate(ByVal objP_server As String,
                                                ByVal Piva As String, ByVal Sa_Cod As String,
                                                ByVal DataInizio As String, ByVal DataFine As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try
            If Not IsDate(DataInizio) Then
                DataInizio = CostantiPersonalizzate.AGRODATAINIZIO
            End If
            If Not IsDate(DataFine) Then
                DataFine = CostantiPersonalizzate.AGRODATAFINE
            End If

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(DataInizio), CDate(DataFine))

            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim Dt As DataTable = objImpianti.Leggi_Specie(Piva,
                                            Sa_Cod,
                                            0, 0, 0, 0,
                                            "",
                                            "",
                                            objParametri_Server)

            objParametri_Server.ResettaFinestra()

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("veg_cod", dr.Item("veg_cod")), New JProperty("veg_des", dr.Item("veg_des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True


        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CaricaCombo_SpecieColtivate_NG(InData As CoreWS_Generic(Of CaricaCombo_SpecieColtivate)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

        Try
            If Not IsDate(InData.InData.DataInizio) Then
                InData.InData.DataInizio = CostantiPersonalizzate.AGRODATAINIZIO
            End If
            If Not IsDate(InData.InData.DataFine) Then
                InData.InData.DataFine = CostantiPersonalizzate.AGRODATAFINE
            End If

            objParametri_Server.ImpostaFinestre_con_SalvataggioTemporale(CDate(InData.InData.DataInizio), CDate(InData.InData.DataFine))

            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim Dt As DataTable = objImpianti.Leggi_Specie(InData.InData.Piva,
                                            InData.InData.Sa_Cod,
                                            0, 0, 0, 0,
                                            "",
                                            "",
                                            objParametri_Server)

            objParametri_Server.ResettaFinestra()

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("veg_cod", dr.Item("veg_cod")), New JProperty("veg_des", dr.Item("veg_des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True


        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetImpresexAppezza_Movimentati(piva As String,
                                          sa_cod As Integer,
                                          appezza As Integer,
                                          ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If

            Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dim dt As DataTable = objAppezza.Leggi(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)
            dt.Columns.Add(New DataColumn("utilizzo"))
            For Each rowAppezza In dt.Rows

                Dim dt_Impianto = objReg_Impianti.Leggi_x_anagrafica(rowAppezza("piva"), rowAppezza("sa_cod"), rowAppezza("appezza"), 0, 0, "", "", objParametri_server)
                If dt_Impianto.Rows.Count > 0 Then
                    Dim rowImpianto = dt_Impianto.Rows(0)
                    If rowImpianto("cul_cod") = 0 Then

                        Dim val = objReg_Impianti.Codice_Anagrafe_from_PivaSaCodAppezzaIdImp(rowImpianto("PIVA"), rowImpianto("SA_COD"), rowImpianto("APPEZZA"), rowImpianto("ID_REG"), objParametri_server)
                        If val = "" Then
                            val = Str_TerrenoNudo
                        End If
                        rowAppezza("utilizzo") = val
                    Else

                        rowAppezza("utilizzo") = rowImpianto("veg_des") & " - " & rowImpianto("cul_des")

                    End If
                End If
            Next


            Dim lista As New List(Of String)
            Dim supCatastale As Double = 0
            Dim jarr = New JArray()
            For Each dr As DataRow In dt.Rows
                Dim jobj As JObject = New JObject
                Dim chiave = dr.Item("PIVA") & "_" & dr.Item("SA_COD") & "_" & dr.Item("APPEZZA")
                jobj.Add(New JProperty("chiave", chiave))
                jobj.Add(New JProperty("Piva", dr.Item("PIVA")))
                jobj.Add(New JProperty("Sa_cod", dr.Item("SA_COD")))
                jobj.Add(New JProperty("Appezza", dr.Item("APPEZZA")))
                jobj.Add(New JProperty("App_Nome", dr.Item("APP_NOME")))
                jobj.Add(New JProperty("Sup_App", dr.Item("SUP_APP")))
                jobj.Add(New JProperty("Utilizzo", dr.Item("utilizzo")))
                jobj.Add(New JProperty("movimentato", AppezzaMovimentato(dr.Item("PIVA"), dr.Item("SA_COD"), dr.Item("APPEZZA"), objParametri_server)))

                If CInt(dr.Item("blk_Flag")) = "-1" Then
                    jobj.Add(New JProperty("bloccato", True))
                Else
                    jobj.Add(New JProperty("bloccato", False))
                End If

                jarr.Add(jobj)
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = jarr.ToString

            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetImpresexAppezza_Movimentati_NG(InData As CoreWS_Generic(Of GetImpresexAppezza_Movimentati)) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            End If

            Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
            Dim objReg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dim dt As DataTable = objAppezza.Leggi(InData.InData.Piva, InData.InData.Sa_Cod, InData.InData.appezza, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)
            dt.Columns.Add(New DataColumn("utilizzo"))
            For Each rowAppezza In dt.Rows

                Dim dt_Impianto = objReg_Impianti.Leggi_x_anagrafica(rowAppezza("piva"), rowAppezza("sa_cod"), rowAppezza("appezza"), 0, 0, "", "", objParametri_server)
                If dt_Impianto.Rows.Count > 0 Then
                    Dim rowImpianto = dt_Impianto.Rows(0)
                    If rowImpianto("cul_cod") = 0 Then

                        Dim val = objReg_Impianti.Codice_Anagrafe_from_PivaSaCodAppezzaIdImp(rowImpianto("PIVA"), rowImpianto("SA_COD"), rowImpianto("APPEZZA"), rowImpianto("ID_REG"), objParametri_server)
                        If val = "" Then
                            val = Str_TerrenoNudo
                        End If
                        rowAppezza("utilizzo") = val
                    Else

                        rowAppezza("utilizzo") = rowImpianto("veg_des") & " - " & rowImpianto("cul_des")

                    End If
                End If
            Next


            Dim lista As New List(Of String)
            Dim supCatastale As Double = 0
            Dim jarr = New JArray()
            For Each dr As DataRow In dt.Rows
                Dim jobj As JObject = New JObject
                Dim chiave = dr.Item("PIVA") & "_" & dr.Item("SA_COD") & "_" & dr.Item("APPEZZA")
                jobj.Add(New JProperty("chiave", chiave))
                jobj.Add(New JProperty("Piva", dr.Item("PIVA")))
                jobj.Add(New JProperty("Sa_cod", dr.Item("SA_COD")))
                jobj.Add(New JProperty("Appezza", dr.Item("APPEZZA")))
                jobj.Add(New JProperty("App_Nome", dr.Item("APP_NOME")))
                jobj.Add(New JProperty("Sup_App", dr.Item("SUP_APP")))
                jobj.Add(New JProperty("Utilizzo", dr.Item("utilizzo")))
                jobj.Add(New JProperty("movimentato", AppezzaMovimentato(dr.Item("PIVA"), dr.Item("SA_COD"), dr.Item("APPEZZA"), objParametri_server)))

                If CInt(dr.Item("blk_Flag")) = "-1" Then
                    jobj.Add(New JProperty("bloccato", True))
                Else
                    jobj.Add(New JProperty("bloccato", False))
                End If

                jarr.Add(jobj)
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = jarr.ToString

            r.RispostaOK = True
            r.RispostaStringa = strRisp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function Leggi_Appezzamento_Global_Anagrafica_G(ByVal objP_super_server As String,
                                                            ByVal objP_server As String,
                                                            ByVal objP_utenti As String,
                                                            ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim Piva = InData.Piva
            Dim Sa_Cod = InData.Sa_Cod
            Dim Appezza = InData.Appezza

            Dim appezzamento = objAppezzamento.Leggi_AppezzamentoGlobal_Anagrafica(Piva:=Piva,
                                                Sa_Cod:=Sa_Cod,
                                                Appezza:=Appezza,
                                                Leggi_Impresa:=True,
                                                Leggi_Centro:=True,
                                                Leggi_Campo:=True,
                                                Leggi_Impianti:=True,
                                                Leggi_Distinte:=True,
                                                objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(appezzamento, Formatting.None)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function Leggi_Appezzamento_Global_Anagrafica_G_NG(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
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
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Piva = InData.InData.Piva
            Dim Sa_Cod = InData.InData.Sa_Cod
            Dim Appezza = InData.InData.Appezza

            Dim appezzamento = objAppezzamento.Leggi_AppezzamentoGlobal_Anagrafica(Piva:=Piva,
                                                Sa_Cod:=Sa_Cod,
                                                Appezza:=Appezza,
                                                Leggi_Impresa:=True,
                                                Leggi_Centro:=True,
                                                Leggi_Campo:=True,
                                                Leggi_Impianti:=True,
                                                Leggi_Distinte:=True,
                                                objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(appezzamento, Formatting.None)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Appezzamenti_Anagrafica(ByVal objP_super_server As String,
                                                  ByVal objP_server As String,
                                                  ByVal objP_utenti As String,
                                                  ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim Piva = InData.Piva
            Dim Sa_Cod = InData.Sa_Cod
            Dim Appezza = InData.Appezza
            Dim Campo_Cod = InData.Campo_Cod
            Dim Data_Filtro = DateTime.Now


            Dim DT_appezzamenti = objAppezzamento.Leggi_Appezzamenti_Anagrafica(Piva:=Piva,
                                                Sa_Cod:=Sa_Cod,
                                                Appezza:=Appezza,
                                                Campo_Cod:=Campo_Cod,
                                                Data_Filtro:=Data_Filtro,
                                                objParametri_Server, objParametri_Utenti)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT_appezzamenti, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Appezzamenti_Anagrafica(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
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
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Piva = InData.InData.Piva
            Dim Sa_Cod = InData.InData.Sa_Cod
            Dim Appezza = InData.InData.Appezza
            Dim Campo_Cod = InData.InData.Campo_Cod
            Dim Data_Filtro = InData.InData.Data


            Dim DT_appezzamenti = objAppezzamento.Leggi_Appezzamenti_Anagrafica(Piva:=Piva,
                                                Sa_Cod:=Sa_Cod,
                                                Appezza:=Appezza,
                                                Campo_Cod:=Campo_Cod,
                                                Data_Filtro:=Data_Filtro,
                                                objParametri_Server, objParametri_Utenti)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT_appezzamenti, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Impianti_Anagrafica(ByVal objP_super_server As String,
                                                  ByVal objP_server As String,
                                                  ByVal objP_utenti As String,
                                                  ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim objParametriAgenda = InData
            If objParametriAgenda.Appezza = "" Then
                objParametriAgenda.Appezza = 0
            End If

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.Data.Date
            End If

            Dim dt As DataTable = objImpianti.Leggi_x_anagrafica2(objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, objParametriAgenda.Appezza, 0, objParametriAgenda.Campo_Cod, "", "", objParametri_Server, Date.Now)

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
                objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
            End If

            'dt.Columns.Add(New DataColumn("codice_anagrafe"))
            'dt.Columns.Add(New DataColumn("utilizzo"))
            'dt.Columns.Add(New DataColumn("varieta"))
            dt.Columns.Add(New DataColumn("lotto"))
            dt.Columns.Add(New DataColumn("Descrizione"))
            dt.Columns.Add(New DataColumn("regolamento"))
            dt.Columns.Add(New DataColumn("disciplinare"))
            dt.Columns.Add(New DataColumn("stato_impianto"))
            dt.Columns.Add(New DataColumn("stato_impianto_des"))

            dt.Columns.Add(New DataColumn("limite_N"))
            dt.Columns.Add(New DataColumn("limite_P"))
            dt.Columns.Add(New DataColumn("limite_K"))
            dt.Columns.Add(New DataColumn("cod_kpin"))
            dt.Columns.Add(New DataColumn("cod_block"))

            dt.Columns.Add(New DataColumn("piante_ha"))
            dt.Columns.Add(New DataColumn("piante_impianto"))

            'dt.Columns.Add(New DataColumn("CoverB", GetType(Boolean)))
            'dt.Columns.Add(New DataColumn("MonitoratoB", GetType(Boolean)))

            Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
            Dim objReg As New AgronicaCoreMetaSchemaDAL.Regolamenti_R

            'Dim codiciImpianti As New Dictionary(Of String, String)
            'LeggiCodiciImpianti(objParametriAgenda.Piva, codiciImpianti, objParametri_Server)

            'Dim codiciEsercizi As New Dictionary(Of String, String)
            'LeggiCodiciEsercizi(objParametriAgenda.Piva, codiciEsercizi, objParametri_Server)

            If objParametriAgenda.Sa_Cod = "" Then
                objParametriAgenda.Sa_Cod = "0"
            End If

            If objParametriAgenda.Appezza = "" Then
                objParametriAgenda.Appezza = "0"
            End If

            If objParametriAgenda.Id_Reg = "" Then
                objParametriAgenda.Id_Reg = "0"
            End If

            If objParametriAgenda.Campo_Cod = "" Then
                objParametriAgenda.Campo_Cod = "0"
            End If

            Dim dtDistinta = objImpreseProgetti.LeggiDistinta_Attiva_inDataxAnagrafica(objParametriAgenda.Piva,
                                                                  objParametriAgenda.Sa_Cod,
                                                                  objParametriAgenda.Appezza,
                                                                  objParametriAgenda.Id_Reg,
                                                                  objParametriAgenda.Data,
                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  " Imprese_Progetti.Validita_Inizio DESC ",
                                                                  objParametri_Server)

            Dim dataRiferimento = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO

            If objParametriAgenda.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                dataRiferimento = objParametriAgenda.Data
            End If

            For Each data In dt.Rows

                If data("su_fila_m") <> "" AndAlso data("tra_fila_m") <> "" AndAlso data("germinabilita") <> "" Then
                    CalcolaPiante(CDbl(data("su_fila_m")), CDbl(data("tra_fila_m")), If(data("interbina") <> "", CDbl(data("interbina")), 0), CDbl(data("germinabilita")), data("sup_imp"), data)
                End If

                ' Leggi dati distina attiva alla data corrente
                Dim rowDist As DataRow()

                rowDist = dtDistinta.Select(" sa_Cod = " & data("sa_cod") &
                                                " AND Appezza = " & data("Appezza") &
                                                " AND ID_reg = " & data("ID_reg") & " ")


                If rowDist IsNot Nothing AndAlso rowDist.Length > 0 Then
                    Dim i = 0

                    data("lotto") = rowDist(0)("Progetto_Nome")
                    data("descrizione") = rowDist(0)("Progetto_Des")
                    data("stato_impianto_des") = rowDist(0)("stato_impianto_des")
                    data("cod_kpin") = rowDist(0)("cod_kpin")
                    data("cod_block") = rowDist(0)("cod_block")
                    data("regolamento") = rowDist(0)("regolamento")

                End If

            Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Campo_Cod", "Campo_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Appezza", "Appezza", "number") With {._hidden = True})
            l.Add(New ColonneNome("Id_Reg", "Id_Reg", "number") With {._hidden = True})
            l.Add(New ColonneNome("veg_cod", "veg_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("cul_cod", "cul_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("grfi_cod", "grfi_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("grva_cod_veg", "grva_cod_veg", "number") With {._hidden = True})

            l.Add(New ColonneNome("Sa_Nome", "Centro", "string"))
            l.Add(New ColonneNome("Campo_Des", "Campo", "string"))
            l.Add(New ColonneNome("app_nome", "Nome Appezzamento", "string"))
            l.Add(New ColonneNome("Codice_Impianto", "Codice Impianto", "string"))
            l.Add(New ColonneNome("utilizzo", "Utilizzo", "string"))
            l.Add(New ColonneNome("varieta", "Varietà", "string"))
            l.Add(New ColonneNome("gru_des", "Gruppo Vegetale", "string"))
            l.Add(New ColonneNome("grfi_des", "Finalità", "string"))
            l.Add(New ColonneNome("grva_des", "Tipologia Varietale", "string"))

            'Superficie formattata
            Dim c = New ColonneNome("sup_imp", "Sup. [Ha]", "number")
            c._formatNr = "n4"
            l.Add(c)


            l.Add(New ColonneNome("Validita_Inizio", "Valida dal", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Valida al", "date"))

            l.Add(New ColonneNome("tra_fila_m", "Tra Fila", "number"))
            l.Add(New ColonneNome("su_fila_m", "Su Fila", "number"))
            l.Add(New ColonneNome("piante_ha", "Piante/Ha", "number"))
            l.Add(New ColonneNome("piante_impianto", "Piante/Impianto", "number"))

            l.Add(New ColonneNome("port_cod", "port_cod", "number"))
            l.Add(New ColonneNome("port_des", "Portinnesto", "string"))

            l.Add(New ColonneNome("foral_cod", "foral_cod", "number"))
            l.Add(New ColonneNome("foral_des", "Forma Allevamento", "string"))

            l.Add(New ColonneNome("cop_cod", "cop_cod", "number"))
            l.Add(New ColonneNome("cop_des", "Copertura", "string"))

            l.Add(New ColonneNome("setup_cod", "Semina/Trapianto", "string"))
            l.Add(New ColonneNome("CoverB", "Cover Crops", "boolean"))
            l.Add(New ColonneNome("MonitoratoB", "Monitorato", "boolean"))

            l.Add(New ColonneNome("lotto", "lotto", "string"))
            l.Add(New ColonneNome("descrizione", "descrizione", "string"))
            l.Add(New ColonneNome("stato_impianto_des", "Stato Impianto", "string"))
            l.Add(New ColonneNome("regolamento", "Regolamento", "string"))
            l.Add(New ColonneNome("cod_kpin", "KPIN", "string"))
            l.Add(New ColonneNome("cod_block", "BLOCK", "string"))

            l.Add(New ColonneNome("Data_Modifica", "Ultima Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))

            l.Add(New ColonneNome("blk_flag", "blk_flag", "number"))

            l.Add(New ColonneNome("Data_Inizio_Portinnesto", "Data_Inizio_Portinnesto", "date"))


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Impianti_Anagrafica_NG(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
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
            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim objParametriAgenda = InData
            If objParametriAgenda.InData.Appezza = "" Then
                objParametriAgenda.InData.Appezza = 0
            End If

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If objParametriAgenda.InData.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = objParametriAgenda.InData.Data.Date
                objParametri_Server.FinestraTemporaleFine = objParametriAgenda.InData.Data.Date
            End If

            Dim dt As DataTable = objImpianti.Leggi_x_anagrafica2(objParametriAgenda.InData.Piva, objParametriAgenda.InData.Sa_Cod, objParametriAgenda.InData.Appezza, 0, objParametriAgenda.InData.Campo_Cod, "", "", objParametri_Server, Date.Now)

            If objParametriAgenda.InData.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = FinestraTemporaleInizio
                objParametri_Server.FinestraTemporaleFine = FinestraTemporaleFine
            End If

            'dt.Columns.Add(New DataColumn("codice_anagrafe"))
            'dt.Columns.Add(New DataColumn("utilizzo"))
            'dt.Columns.Add(New DataColumn("varieta"))
            dt.Columns.Add(New DataColumn("lotto"))
            dt.Columns.Add(New DataColumn("Descrizione"))
            dt.Columns.Add(New DataColumn("regolamento"))
            dt.Columns.Add(New DataColumn("disciplinare"))
            dt.Columns.Add(New DataColumn("stato_impianto"))
            dt.Columns.Add(New DataColumn("stato_impianto_des"))

            dt.Columns.Add(New DataColumn("limite_N"))
            dt.Columns.Add(New DataColumn("limite_P"))
            dt.Columns.Add(New DataColumn("limite_K"))
            dt.Columns.Add(New DataColumn("cod_kpin"))
            dt.Columns.Add(New DataColumn("cod_block"))

            dt.Columns.Add(New DataColumn("piante_ha"))
            dt.Columns.Add(New DataColumn("piante_impianto"))

            'dt.Columns.Add(New DataColumn("CoverB", GetType(Boolean)))
            'dt.Columns.Add(New DataColumn("MonitoratoB", GetType(Boolean)))

            Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
            Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
            Dim objReg As New AgronicaCoreMetaSchemaDAL.Regolamenti_R

            'Dim codiciImpianti As New Dictionary(Of String, String)
            'LeggiCodiciImpianti(objParametriAgenda.Piva, codiciImpianti, objParametri_Server)

            'Dim codiciEsercizi As New Dictionary(Of String, String)
            'LeggiCodiciEsercizi(objParametriAgenda.Piva, codiciEsercizi, objParametri_Server)

            If objParametriAgenda.InData.Sa_Cod = "" Then
                objParametriAgenda.InData.Sa_Cod = "0"
            End If

            If objParametriAgenda.InData.Appezza = "" Then
                objParametriAgenda.InData.Appezza = "0"
            End If

            If objParametriAgenda.InData.Id_Reg = "" Then
                objParametriAgenda.InData.Id_Reg = "0"
            End If

            If objParametriAgenda.InData.Campo_Cod = "" Then
                objParametriAgenda.InData.Campo_Cod = "0"
            End If

            Dim dtDistinta = objImpreseProgetti.LeggiDistinta_Attiva_inDataxAnagrafica(objParametriAgenda.InData.Piva,
                                                                  objParametriAgenda.InData.Sa_Cod,
                                                                  objParametriAgenda.InData.Appezza,
                                                                  objParametriAgenda.InData.Id_Reg,
                                                                  objParametriAgenda.InData.Data,
                                                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                  "",
                                                                  " Imprese_Progetti.Validita_Inizio DESC ",
                                                                  objParametri_Server)

            Dim dataRiferimento = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO

            If objParametriAgenda.InData.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                dataRiferimento = objParametriAgenda.InData.Data
            End If

            For Each data In dt.Rows

                If data("su_fila_m") <> "" AndAlso data("tra_fila_m") <> "" AndAlso data("germinabilita") <> "" Then
                    CalcolaPiante(CDbl(data("su_fila_m")), CDbl(data("tra_fila_m")), If(data("interbina") <> "", CDbl(data("interbina")), 0), CDbl(data("germinabilita")), data("sup_imp"), data)
                End If

                ' Leggi dati distina attiva alla data corrente
                Dim rowDist As DataRow()

                rowDist = dtDistinta.Select(" sa_Cod = " & data("sa_cod") &
                                                " AND Appezza = " & data("Appezza") &
                                                " AND ID_reg = " & data("ID_reg") & " ")


                If rowDist IsNot Nothing AndAlso rowDist.Length > 0 Then
                    Dim i = 0

                    data("lotto") = rowDist(0)("Progetto_Nome")
                    data("descrizione") = rowDist(0)("Progetto_Des")
                    data("stato_impianto_des") = rowDist(0)("stato_impianto_des")
                    data("cod_kpin") = rowDist(0)("cod_kpin")
                    data("cod_block") = rowDist(0)("cod_block")
                    data("regolamento") = rowDist(0)("regolamento")

                End If

            Next

            'creo la lista delle colonne da visualizzare
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("chiave", "chiave", "string") With {._hidden = True})
            l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
            l.Add(New ColonneNome("Sa_Cod", "Sa_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Campo_Cod", "Campo_Cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("Appezza", "Appezza", "number") With {._hidden = True})
            l.Add(New ColonneNome("Id_Reg", "Id_Reg", "number") With {._hidden = True})
            l.Add(New ColonneNome("veg_cod", "veg_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("cul_cod", "cul_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("grfi_cod", "grfi_cod", "number") With {._hidden = True})
            l.Add(New ColonneNome("grva_cod_veg", "grva_cod_veg", "number") With {._hidden = True})

            l.Add(New ColonneNome("Sa_Nome", "Centro", "string"))
            l.Add(New ColonneNome("Campo_Des", "Campo", "string"))
            l.Add(New ColonneNome("app_nome", "Nome Appezzamento", "string"))
            l.Add(New ColonneNome("Codice_Impianto", "Codice Impianto", "string"))
            l.Add(New ColonneNome("utilizzo", "Utilizzo", "string"))
            l.Add(New ColonneNome("varieta", "Varietà", "string"))
            l.Add(New ColonneNome("gru_des", "Gruppo Vegetale", "string"))
            l.Add(New ColonneNome("grfi_des", "Finalità", "string"))
            l.Add(New ColonneNome("grva_des", "Tipologia Varietale", "string"))

            'Superficie formattata
            Dim c = New ColonneNome("sup_imp", "Sup. [Ha]", "number")
            c._formatNr = "n4"
            l.Add(c)


            l.Add(New ColonneNome("Validita_Inizio", "Valida dal", "date"))
            l.Add(New ColonneNome("Validita_Fine", "Valida al", "date"))

            l.Add(New ColonneNome("tra_fila_m", "Tra Fila", "number"))
            l.Add(New ColonneNome("su_fila_m", "Su Fila", "number"))
            l.Add(New ColonneNome("piante_ha", "Piante/Ha", "number"))
            l.Add(New ColonneNome("piante_impianto", "Piante/Impianto", "number"))

            l.Add(New ColonneNome("port_cod", "port_cod", "number"))
            l.Add(New ColonneNome("port_des", "Portinnesto", "string"))

            l.Add(New ColonneNome("foral_cod", "foral_cod", "number"))
            l.Add(New ColonneNome("foral_des", "Forma Allevamento", "string"))

            l.Add(New ColonneNome("cop_cod", "cop_cod", "number"))
            l.Add(New ColonneNome("cop_des", "Copertura", "string"))

            l.Add(New ColonneNome("setup_cod", "Semina/Trapianto", "string"))
            l.Add(New ColonneNome("CoverB", "Cover Crops", "boolean"))
            l.Add(New ColonneNome("MonitoratoB", "Monitorato", "boolean"))

            l.Add(New ColonneNome("lotto", "lotto", "string"))
            l.Add(New ColonneNome("descrizione", "descrizione", "string"))
            l.Add(New ColonneNome("stato_impianto_des", "Stato Impianto", "string"))
            l.Add(New ColonneNome("regolamento", "Regolamento", "string"))
            l.Add(New ColonneNome("cod_kpin", "KPIN", "string"))
            l.Add(New ColonneNome("cod_block", "BLOCK", "string"))

            l.Add(New ColonneNome("Data_Modifica", "Ultima Modifica", "date"))
            l.Add(New ColonneNome("Utente_Modifica", "Utente Modifica", "string"))

            l.Add(New ColonneNome("Data_Creazione", "Data Creazione", "date"))
            l.Add(New ColonneNome("Utente_Creazione", "Utente Creazione", "string"))

            l.Add(New ColonneNome("blk_flag", "blk_flag", "number"))

            l.Add(New ColonneNome("Data_Inizio_Portinnesto", "Data_Inizio_Portinnesto", "date"))


            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            Dim risp As String = js.JSON_DataTable_Kendo(dt, l, AssegnaAutomaticamenteTipoFiltro_daTipoDato:=True, tipoFiltroKendo:=TipoFiltroKendo_colonne.Menu)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <
        Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)
        >
    Public Function Leggi_Appezzamento_Global_Anagrafica_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)
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
                                                data:=AGRODATAINIZIO,
                                                filtroData:=False,
                                                Leggi_Distinte:=True,
                                                Leggi_Cartografia:=False,
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

    <WebMethod()> <
        Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)
        >
    Public Function Leggi_Appezzamento_Global_Anagrafica(ByVal objP_super_server As String,
                                                            ByVal objP_server As String,
                                                            ByVal objP_utenti As String,
                                                            ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.Appezzamento)

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

        Try
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim appezzamento = objAppezzamento.Leggi_Appezzamento_Anagrafica(Piva:=InData.Piva,
                                                Sa_Cod:=InData.Sa_Cod,
                                                Appezza:=InData.Appezza,
                                                IdReg:=0,
                                                Leggi_Impianti:=True,
                                                Leggi_Indirizzi:=True,
                                                Leggi_Catasto:=True,
                                                data:=AGRODATAINIZIO,
                                                filtroData:=False,
                                                Leggi_Distinte:=True,
                                                Leggi_Cartografia:=False,
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

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Esercizi_Anagrafica(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
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
            Dim objImpianto As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Piva = InData.InData.Piva
            Dim Sa_Cod = InData.InData.Sa_Cod
            Dim Appezza = InData.InData.Appezza
            Dim Id_Reg = InData.InData.Id_Reg
            Dim Campo_Cod = InData.InData.Campo_Cod
            Dim Data_Filtro = InData.InData.Data


            Dim DT_appezzamenti = objImpianto.Leggi_Esercizi_Anagrafica(
                Piva:=Piva,
                Sa_Cod:=Sa_Cod,
                Appezza:=Appezza,
                Id_Reg:=Id_Reg,
                Campo_Cod:=Campo_Cod,
                Data_Filtro:=Data_Filtro,
                objParametri_Server,
                objParametri_Utenti
                )

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT_appezzamenti, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function CaricaDatiCatastali(ByVal objP_super_server As String,
                                        ByVal objP_server As String,
                                        ByVal objP_utenti As String,
                                        ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG,
                                        ByVal Campo_Cod As Integer,
                                        ByVal flag_Macrousi As Boolean,
                                        ByVal flag_Utilizzi As Boolean,
                                        ByVal flag_Varieta As Boolean,
                                        ByVal ValiditaInizio As Date,
                                        ByVal ValiditaFine As Date
                                        ) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

            Dim Piva = InData.Piva
            Dim Sa_Cod = InData.Sa_Cod
            Dim Appezza = InData.Appezza
            Dim Tipo_Operazione = InData.TipoOperazioneDB

            Dim SuperficieIntersezioneTotale As Double = 0

            Dim DT = objAppezzamento.LeggiDatiCatastali(Piva:=Piva,
                                                        Sa_Cod:=Sa_Cod,
                                                        Appezza:=Appezza,
                                                        Campo_Cod:=Campo_Cod,
                                                        flag_Macrousi:=flag_Macrousi,
                                                        flag_Utilizzi:=flag_Utilizzi,
                                                        flag_Varieta:=flag_Varieta,
                                                        ValiditaInizio:=ValiditaInizio,
                                                        ValiditaFine:=ValiditaFine,
                                                        Tipo_Operazione:=Tipo_Operazione,
                                                        SuperficieIntersezioneTotale:=SuperficieIntersezioneTotale,
                                                        objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)
            r.ParametroDue_stringa = SuperficieIntersezioneTotale

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function CaricaDatiCatastali_NG(InData As CoreWS_Generic(Of CaricaDatiCatastali)) As RispostaStandard

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
            Dim objAppezzamento As New AgronicaCoreAnagrafeBIZ.Appezzamento_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Piva = InData.InData.parametri_ObjParametriAgenda_NG.Piva
            Dim Sa_Cod = InData.InData.parametri_ObjParametriAgenda_NG.Sa_Cod
            Dim Appezza = InData.InData.parametri_ObjParametriAgenda_NG.Appezza
            Dim Tipo_Operazione = InData.InData.parametri_ObjParametriAgenda_NG.TipoOperazioneDB

            Dim SuperficieIntersezioneTotale As Double = 0

            Dim DT = objAppezzamento.LeggiDatiCatastali(
                Piva:=Piva,
                Sa_Cod:=Sa_Cod,
                Appezza:=Appezza,
                Campo_Cod:=InData.InData.Campo_Cod,
                flag_Macrousi:=InData.InData.flag_Macrousi,
                flag_Utilizzi:=InData.InData.flag_Utilizzi,
                flag_Varieta:=InData.InData.flag_Varieta,
                ValiditaInizio:=InData.InData.ValiditaInizio,
                ValiditaFine:=InData.InData.ValiditaFine,
                Tipo_Operazione:=Tipo_Operazione,
                SuperficieIntersezioneTotale:=SuperficieIntersezioneTotale,
                objParametri_Server
                )

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)
            r.ParametroDue_stringa = SuperficieIntersezioneTotale

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function LeggiParticelle_Per_Appezzamento(ByVal objP_super_server As String,
                                                    ByVal objP_server As String,
                                                    ByVal objP_utenti As String,
                                                    ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
                                                    ) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim Piva = InData.Piva
            Dim Sa_Cod = InData.Sa_Cod
            Dim Appezza = InData.Appezza

            Dim SuperficieIntersezioneTotale As Double = 0

            Dim DT = objAppezzaxParticelle.LeggiParticelle_Da_Appezzamento(Piva:=Piva,
                                                        Sa_Cod:=Sa_Cod,
                                                        Appezza:=Appezza,
                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                        "", "",
                                                        objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)
            r.ParametroDue_stringa = SuperficieIntersezioneTotale

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function LeggiParticelle_Per_Appezzamento(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objAppezzaxParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim Piva = InData.InData.Piva
            Dim Sa_Cod = InData.InData.Sa_Cod
            Dim Appezza = InData.InData.Appezza

            Dim SuperficieIntersezioneTotale As Double = 0

            Dim DT = objAppezzaxParticelle.LeggiParticelle_Da_Appezzamento(Piva:=Piva,
                                                        Sa_Cod:=Sa_Cod,
                                                        Appezza:=Appezza,
                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                        "", "",
                                                        objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)
            r.ParametroDue_stringa = SuperficieIntersezioneTotale

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function Controlla_Validita_Appezzamenti(ByVal objP_super_server As String,
                                                    ByVal objP_server As String,
                                                    ByVal objP_utenti As String,
                                                    ByVal InData As AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG
                                                    ) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objMov_Destinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim Piva = InData.Piva
            Dim Sa_Cod = InData.Sa_Cod
            Dim Appezza = InData.Appezza

            Dim DT = objMov_Destinazioni.Controlla_Validita_Appezzamenti(Piva:=Piva,
                                                                        Sa_Cod:=Sa_Cod,
                                                                        Appezza:=Appezza,
                                                                        "", "",
                                                                        objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function Controlla_Validita_Appezzamenti(InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objMov_Destinazioni As New AgronicaCoreContabDAL.Mov_Destinazioni_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim Piva = InData.InData.Piva
            Dim Sa_Cod = InData.InData.Sa_Cod
            Dim Appezza = InData.InData.Appezza

            Dim DT = objMov_Destinazioni.Controlla_Validita_Appezzamenti(Piva:=Piva,
                                                                        Sa_Cod:=Sa_Cod,
                                                                        Appezza:=Appezza,
                                                                        "", "",
                                                                        objParametri_Server)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function CaricaGridImpianti(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiImpianto)) As RispostaStandard
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
            Dim objReg_Impianto As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)
            Dim objParametri_Super_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim DT As New DataTable


            Dim Piva As String = InData.InData.centroAziendale.primaryKey.partitaIva

            Dim Sa_Cod As Integer = InData.InData.centroAziendale.primaryKey.codice

            Dim Data As Date = InData.InData.data

            Dim Lav_Cod_List As New List(Of Integer)

            If Not IsNothing(InData.InData.lavorazioni) AndAlso InData.InData.lavorazioni.Count > 0 Then
                Lav_Cod_List = Array.ConvertAll(InData.InData.lavorazioni, Function(s) CInt(s.primaryKey.codice)).ToList()
            End If

            Dim consideraTerrenoNudo As Boolean = InData.InData.consideraTerrenoNudo

            Dim dettagliTerrenoNudo As Boolean = InData.InData.dettagliTerrenoNudo

            Dim TipoOperazioneDB As enum_TipoOperazioneDB = InData.InData.tipo_operazione_db

            Dim Veg_Cod As Integer = InData.InData.veg_cod

            Dim Dest_Cod As Integer = InData.InData.dest_cod

            Dim Dpi_Cod As Integer = 0

            Dim Grfi_Cod As Integer = 0

            Dim flagProtetto As Integer = 0

            Dim disciplinarePubblicoPrivato As Integer = 0


            'Il Dpi_Cod (in questo caso sarebbe il codice della direttiva nitrati) mi serve per leggere le classi tessitura
            If Lav_Cod_List.Exists(Function(lav_cod) {LAVCOD_DISTRIBUZIONE_AMMENDANTI}.Contains(lav_cod)) AndAlso
                Not IsNothing(InData.InData.direttiva_nitrati) AndAlso
                InData.InData.direttiva_nitrati.regolamentoConcimazione.tipo = enum_PUARegolamenti_Tipo.PUA Then

                Dpi_Cod = InData.InData.direttiva_nitrati.regolamentoConcimazione.codice

            End If

            Dim Campo_Cod As Integer = InData.InData.campo.primaryKey.codice

            Dim TipoRicetta As Integer = InData.InData.tipo_ricetta

            objReg_Impianto.LeggiImpianti_QdC(DT,
                                                Sa_Cod,
                                                Veg_Cod,
                                                Piva,
                                                Data,
                                                consideraTerrenoNudo,
                                                dettagliTerrenoNudo,
                                                Dest_Cod,
                                                TipoOperazioneDB,
                                                Dpi_Cod,
                                                Grfi_Cod,
                                                flagProtetto,
                                                disciplinarePubblicoPrivato,
                                                Lav_Cod_List,
                                                TipoRicetta,
                                                InData.InData.id_agenda_list.ToList(),
                                                InData.InData.ricetta_operazione_cod_list.ToList(),
                                                Campo_Cod,
                                                InData.InData.tipo_attivita,
                                                InData.InData.stato,
                                                objParametri_Server,
                                                objParametri_Utenti,
                                                objParametri_Super_Server)

            If Lav_Cod_List.Exists(Function(lav_cod) {LAVCOD_RACCOLTA}.Contains(lav_cod)) AndAlso DT.Rows.Count > 0 Then
                verificaCarenza(DT, Data, Piva, Sa_Cod, Veg_Cod, objParametri_Server, objParametri_Utenti)
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(DT, serializerSettings)

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_SpecieVegetali_Attive_Impianti(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiImpianto)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno))


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

            Dim SpecieVegetaliAttive As List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.UtilizzoTerreno)

            Dim Piva As String = ""

            Dim Sa_Cod As Integer = 0

            If Not IsNothing(InData.InData.centroAziendale) Then
                Piva = InData.InData.centroAziendale.primaryKey.partitaIva

                Sa_Cod = InData.InData.centroAziendale.primaryKey.codice
            Else
                Piva = InData.InData.impresa.partitaIva
            End If

            Dim Data As Date = InData.InData.data

            Dim consideraTerrenoNudo As Boolean = InData.InData.consideraTerrenoNudo

            Dim dettagliTerrenoNudo As Boolean = InData.InData.dettagliTerrenoNudo

            Dim filtra_validita_esercizi As Boolean = False

            If Not IsNothing(InData.InData.filtra_validita_esercizi) Then
                filtra_validita_esercizi = InData.InData.filtra_validita_esercizi
            End If

            Dim objReg_Impianto As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_R
            SpecieVegetaliAttive = objReg_Impianto.Leggi_SpecieVegetali_Attive_Impianti(Piva,
                                                                                        Sa_Cod,
                                                                                        0,
                                                                                        Data,
                                                                                        True,
                                                                                        consideraTerrenoNudo,
                                                                                        dettagliTerrenoNudo,
                                                                                        objParametri_Server,
                                                                                        objParametri_Utenti,
                                                                                        filtra_validita_esercizi)


            r.RispostaOK = True
            r.RispostaStringa = SpecieVegetaliAttive

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_DropDown_Esercizi_Codici(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.CodiceAnagrafe))

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
            StrCodiciImpianto = objcodAn.Filtro_Codici_Anagrafe(4, 3, 2, objParametri_Server)
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
            StrCodiciImpianto = Replace(StrCodiciImpianto, " Or Codice = " + CStr(enum_CodiciAnagrafe.Impianto_Nr_domanda_ACA), "")
            StrCodiciImpianto = Replace(StrCodiciImpianto, "Codice = " + CStr(enum_CodiciAnagrafe.Impianto_Nr_domanda_ACA) + " Or ", "")


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
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function GetCodiceImpianto(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of Object)

        Dim r As New rispostaStandard(Of Object)

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

            Dim piva = InData.InData("piva")
            Dim year = InData.InData("year")
            Static Dim algoritmoCodifica As String = Replica_GIAS.LeggiAlgoritmoCodifica(piva, objParametri_Server)
            Static Dim codiceImpianto As String = Replica_GIAS.LeggiCodiceProgressivo(piva, algoritmoCodifica, enum_SequenzaProgressiviTipi.CodiciProgettoAgricoli, year, objParametri_Server)

            Dim response As New With {
                .algoritmoCodifica = algoritmoCodifica,
                .codiceImpianto = codiceImpianto
            }

            r.RispostaOK = True
            r.RispostaStringa = response
        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function GenerateDescriptions(InData As CoreWS_Generic(Of String)) As rispostaStandard(Of List(Of Object))

        Dim r As New rispostaStandard(Of List(Of Object))

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

            Dim dt As DataTable
            Dim objCampi As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            dt = objCampi.MateriePrime_DescrizioniOP(InData.InData, 0, "201,204,210", "", "", "", objParametri_Server)

            Dim response As New List(Of Object)
            Dim i As Integer

            For i = 0 To dt.Rows.Count - 1
                Dim item As New With {
                    .codice = dt.Rows(i).Item("cod_articolo"),
                    .descrizione = dt.Rows(i).Item("cod_articolo") & " - " & dt.Rows(i).Item("mat_des")
                }
                response.Add(item)
            Next

            r.RispostaOK = True
            r.RispostaStringa = response

        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function ClosePlant(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of String)
        Dim r As New rispostaStandard(Of String)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}
        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.Esercizio))(JsonConvert.SerializeObject(InData), a)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim biz As New AgronicaCoreAnagrafeBIZ.Reg_Impianto_W
            Dim esercizio As AgronicaCoreModelsSTD.anagrafiche.Esercizio = iData.InData
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametriUtente As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            biz.ClosePlant(
                esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.partitaIva,
                esercizio.impiantoPK.appezzamentoPK.centroAziendalePK.codice,
                esercizio.impiantoPK.appezzamentoPK.codice,
                esercizio.impiantoPK.codice,
                esercizio.codice,
                esercizio.esercizio_Chiuso,
                esercizio.esercizioReplica,
                objParametriServer,
                objParametriUtente
                )

            r.RispostaOK = True
            r.RispostaStringa = ""
        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod(ResponseFormat:=Script.Services.ResponseFormat.Json)>
    Public Function WriteProjectsXContributesRecords(InData As CoreWS_Generic(Of List(Of Esercizio))) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore}
        Dim iData As CoreWS_Generic(Of List(Of Esercizio)) =
            JsonConvert.DeserializeObject(Of CoreWS_Generic(Of List(Of Esercizio)))(JsonConvert.SerializeObject(InData), a)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim biz As New ProgettiXContributi_W
            Dim esercizi As List(Of Esercizio) = iData.InData
            Dim objServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objUtente As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            r.RispostaStringa = biz.WriteRecords(
                objServer,
                objUtente,
                esercizi
                )

            r.RispostaOK = True
        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function



    Public Function AppezzaMovimentato(piva As String, sa_cod As Integer, appezza As Integer, objParametri_server As AgronicaCoreParametri) As Boolean
        Dim movimentato As Boolean = False
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objAppezzaParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

        Dim listaParticelleCoinvolte As New List(Of String)
        Dim listaAppCoinvolti As New List(Of String)
        listaAppCoinvolti.Add(piva & "_" & sa_cod & "_" & appezza)
        Dim dtParticelle = objAppezzaParticelle.LeggiParticelle_Da_Appezzamento(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
        For Each partRow In dtParticelle.Rows
            'Dim strPart = ""
            'strPart = partRow("Prov") & "_" & partRow("Com") & "_" & partRow("Sezione") & "_" & partRow("Foglio") & "_" & partRow("Numero") & "_" & partRow("Subalterno")
            'If Not listaParticelleCoinvolte.Contains(strPart) Then
            '    listaParticelleCoinvolte.Add(strPart)
            'End If
            Dim dtAppezza = objAppezzaParticelle.LeggiAppezzamenti_Da_Particella(partRow("Prov"),
                                                                 partRow("Com"),
                                                                 partRow("Sezione"),
                                                                 partRow("Foglio"),
                                                                 partRow("Numero"),
                                                                 partRow("Subalterno"),
                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                 "", "",
                                                                 objParametri_server)
            For Each rowAppezza In dtAppezza.Rows
                Dim strAppezza = rowAppezza("Piva") & "_" & rowAppezza("sa_cod") & "_" & rowAppezza("Appezza")
                If Not listaAppCoinvolti.Contains(strAppezza) Then
                    listaAppCoinvolti.Add(strAppezza)
                    AppCoinvolti(listaAppCoinvolti, rowAppezza("Piva"), rowAppezza("sa_cod"), rowAppezza("Appezza"), objParametri_server)
                End If
            Next

        Next

        For Each strApp In listaAppCoinvolti
            Dim appArr = strApp.Split("_")
            Dim xPiva = appArr(0)
            Dim xSa_cod = appArr(1)
            Dim xAppezza = appArr(2)
            Dim dt = objImpianti.Leggi_Operazioni_Impianti(xPiva, xSa_cod, xAppezza, 0, "", "", objParametri_server)
            If dt IsNot Nothing And dt.Rows.Count > 0 Then
                movimentato = True
                Exit For
            End If
        Next

        Return movimentato

    End Function

    Public Sub AppCoinvolti(ByRef listaApp As List(Of String), piva As String, sa_cod As Integer, appezza As Integer, objParametri_server As AgronicaCoreParametri)
        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim objAppezza As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
        Dim objAppezzaParticelle As New AgronicaCoreAnagrafeDAL.AppezzaxParticelle_R

        Dim dtParticelle = objAppezzaParticelle.LeggiParticelle_Da_Appezzamento(piva, sa_cod, appezza, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
        For Each partRow In dtParticelle.Rows
            'Dim strPart = ""
            'strPart = partRow("Prov") & "_" & partRow("Com") & "_" & partRow("Sezione") & "_" & partRow("Foglio") & "_" & partRow("Numero") & "_" & partRow("Subalterno")
            'If Not listaParticelleCoinvolte.Contains(strPart) Then
            '    listaParticelleCoinvolte.Add(strPart)
            'End If
            Dim dtAppezza = objAppezzaParticelle.LeggiAppezzamenti_Da_Particella(partRow("Prov"),
                                                                 partRow("Com"),
                                                                 partRow("Sezione"),
                                                                 partRow("Foglio"),
                                                                 partRow("Numero"),
                                                                 partRow("Subalterno"),
                                                                 enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                 "", "",
                                                                 objParametri_server)

            For Each rowAppezza In dtAppezza.Rows
                Dim strAppezza = rowAppezza("Piva") & "_" & rowAppezza("sa_cod") & "_" & rowAppezza("Appezza")
                If Not listaApp.Contains(strAppezza) Then
                    listaApp.Add(strAppezza)
                    AppCoinvolti(listaApp, rowAppezza("Piva"), rowAppezza("sa_cod"), rowAppezza("Appezza"), objParametri_server)
                End If
            Next

        Next

    End Sub

    Private Shared Sub verificaCarenza(ByRef DT As DataTable,
                                       DataOperazione As Date,
                                       Piva As String,
                                       Sa_Cod As Integer,
                                       Veg_Cod As Integer,
                                       objParametri_Server As AgronicaCoreParametri,
                                       objParametri_Utenti As AgronicaCoreParametri)


        Try
            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

                ' ----------------------------------------------------
                ' INIZIALIZZAZIONE DATATABLES E VARIABILI X VERIFICA 
                ' ----------------------------------------------------
                Dim Dt_Raccolte As New DataTable
                Dim Dt_Raccolte_Impianti As DataTable = DT.Copy()
                Dim Dt_Trattamenti_x_Raccolte As New DataTable
                Dim Dt_Trattamenti_Impianti_x_Raccolte As New DataTable
                Dim DTCarenze As New DataTable

                Dim strfrcod As String = ""

                ' ----------------------------------------------------
                ' ESTRAZIONE FORMULATI UTILIZZATI FINO ALLA DATA DELL'OPERAZIONE
                ' ----------------------------------------------------
                Dim obj_Op As New AgronicaCoreContabDAL.Mov_Destinazioni_R

                Dim dtfrcod As DataTable = obj_Op.Leggi_Prodotti_Utilizzati_Su_VegCod_IntervalloTemporale(Piva, Sa_Cod, Veg_Cod, AGRODATAINIZIO, AGRODATAFINE, CostantiPersonalizzate.FORMULATI, "", "", objParametri_Server)
                If Not dtfrcod Is Nothing AndAlso dtfrcod.Rows.Count > 0 Then
                    For Each fr_cod In dtfrcod.Rows
                        strfrcod &= fr_cod.Item("pro_cod") & ","
                    Next

                    ' ----------------------------------------------------
                    ' ESTRAZIONE CARENZA FORMULATI USATI
                    ' ----------------------------------------------------
                    If strfrcod <> "" Then
                        Dim objws As New AgronicaCoreWebService.AgroWs
                        DTCarenze = objws.TempoCarenza_from_Multiple_FrCod_VegCod_Intervallo(Left(strfrcod, strfrcod.Length - 1), Veg_Cod, CDate(AGRODATAINIZIO), CDate(DataOperazione), objParametri_Server, objParametri_Utenti)
                    End If
                End If


                Dt_Trattamenti_x_Raccolte = obj_Op.Leggi_Operazioni_VegCod_IntervalloTemporale_x_Analisi_Conformita(Piva, Sa_Cod, Veg_Cod, AGRODATAINIZIO, CDate(DataOperazione), enum_TipoOperazione_xAnalisiConformita.Difesa_Diserbo, "", " movimenti.Data_Movimento desc", objParametri_Server)
                If Not Dt_Trattamenti_x_Raccolte Is Nothing AndAlso Dt_Trattamenti_x_Raccolte.Rows.Count > 0 Then

                    Dim strIdAgendaTrattamenti_x_Raccolte As String = ""
                    For Each row In Dt_Trattamenti_x_Raccolte.Rows
                        strIdAgendaTrattamenti_x_Raccolte &= row.Item("id_agenda") & ","
                    Next

                    If strIdAgendaTrattamenti_x_Raccolte <> "" Then
                        Dt_Trattamenti_Impianti_x_Raccolte = obj_Op.Leggi_Dettagli_Impianti_x_Analisi_Conformita(Piva, Left(strIdAgendaTrattamenti_x_Raccolte, strIdAgendaTrattamenti_x_Raccolte.Length - 1), enum_TipoOperazione_xAnalisiConformita.Difesa_Diserbo, "", "", objParametri_Server)
                    End If
                End If

                ' ----------------------------------------------------
                '  CREAZIONE DATATABLE RACCOLTA -- COLONNE USATE PER LA VERIFICA
                ' ----------------------------------------------------
                Dt_Raccolte.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
                Dt_Raccolte.Columns.Add(New DataColumn("piva", GetType(String)))
                Dt_Raccolte.Columns.Add(New DataColumn("sa_cod", GetType(Integer)))
                Dt_Raccolte.Columns.Add(New DataColumn("Data_Movimento", GetType(String)))
                Dt_Raccolte.Columns.Add(New DataColumn("Lav_Cod", GetType(Integer)))
                Dt_Raccolte.Columns.Add(New DataColumn("des_lib", GetType(String)))
                Dt_Raccolte.Columns.Add(New DataColumn("Veg_Cod", GetType(Integer)))

                Dim drRaccolta = Dt_Raccolte.NewRow
                drRaccolta.Item("id_agenda") = -1
                drRaccolta.Item("piva") = Piva
                drRaccolta.Item("sa_cod") = Sa_Cod
                drRaccolta.Item("Data_Movimento") = DataOperazione
                drRaccolta.Item("Lav_Cod") = LAVCOD_RACCOLTA
                drRaccolta.Item("des_lib") = ""
                drRaccolta.Item("Veg_Cod") = Veg_Cod
                Dt_Raccolte.Rows.Add(drRaccolta)

                ' ----------------------------------------------------
                '  AGGIUNTA COLONNE USATE PER LA VERIFICA -- IMPIANTI
                ' ----------------------------------------------------
                Dt_Raccolte_Impianti.Columns.Add(New DataColumn("id_agenda", GetType(Integer)))
                Dt_Raccolte_Impianti.Columns.Add(New DataColumn("id_destinazione", GetType(Integer)))
                Dt_Raccolte_Impianti.Columns.Add(New DataColumn("progetto_nome", GetType(String)))
                Dt_Raccolte_Impianti.Columns.Add(New DataColumn("validita_inizio_esercizio", GetType(String)))
                Dt_Raccolte_Impianti.Columns.Add(New DataColumn("validita_fine_esercizio", GetType(String)))
                Dt_Raccolte_Impianti.Columns.Add(New DataColumn("qta2", GetType(Decimal)))
                For Each row In Dt_Raccolte_Impianti.Rows
                    row.Item("id_agenda") = -1
                    row.Item("id_destinazione") = row.Item("id_reg")
                    row.Item("progetto_nome") = row.Item("progetto")
                    row.Item("validita_inizio_esercizio") = row.Item("validita_inizio_distinta")
                    row.Item("validita_fine_esercizio") = row.Item("validita_fine_distinta")
                    row.Item("qta2") = row.Item("sup_imp")
                Next

                ' ----------------------------------------------------
                ' VERIFICA CARENZA
                ' ----------------------------------------------------
                Dim objDpiVerifica As New AgronicaCoreDpiBIZ.DPI_Verifica
                objDpiVerifica.Verifica_Raccolte(Dt_Raccolte, Dt_Raccolte_Impianti,
                                                 Dt_Trattamenti_Impianti_x_Raccolte, DTCarenze,
                                                 True,
                                                 objParametri_Server, objParametri_Utenti, isFromVerificaCarenzaRaccoltaQdCNG:=True, DT)
            End If

        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try
    End Sub

    Private Shared Function CalcolaPiante(ByVal dist_su As Double, ByVal dist_tra As Double, ByVal interb As Double, ByVal germin As Double, ByVal superficie As Double, ByRef data As Object) As Boolean
        If dist_su > 0 AndAlso dist_tra > 0 AndAlso germin > 0 Then
            ' Dim denominatore = If(interb > 0, (interb / 2) * dist_su, dist_su * dist_tra)
            Dim denominatore = If(interb > 0, Math.Abs(dist_tra - interb) * dist_su, dist_su * dist_tra)
            Dim PianteHa = 10000 / denominatore * (germin / 100)
            Dim PianteImpianto = PianteHa * superficie
            data("piante_ha") = Int(PianteHa)
            data("piante_impianto") = Int(PianteImpianto)
            Return True
        End If
        Return False
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function ModificaMultipla(objP_server As String, parametri As String, dati As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            Dim objEreditatore As New AgronicaCoreAnagrafeBIZ.Ereditatore
            r = objEreditatore.ModificaMultipla_PianoColturale_NEW(parametri, dati, objParametri_Server)


        Catch ex As GiasException
            'Errore gestito
            r.RispostaStringa = "Impossibile proseguire con il salvataggio: <br>" & ex.Message
            r.RispostaOK = False
        Catch ex As Exception

            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

End Class