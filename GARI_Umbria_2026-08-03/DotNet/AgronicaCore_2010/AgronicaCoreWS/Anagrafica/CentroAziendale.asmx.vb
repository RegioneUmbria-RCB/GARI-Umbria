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
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Xml
Imports AgronicaCoreAnagrafeBIZ
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreAnagrafeBIZ.AnagrafeNG
Imports AgronicaCoreModelsSTD.costanti
Imports AgronicaCoreModelsSTD.metaschema.utilizzi
Imports AgronicaCoreDTOStd.InData.Anagrafica
Imports InData.Anagrafica
Imports AgronicaCoreModelsSTD.exceptions

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
Public Class CentroAziendale
    Inherits System.Web.Services.WebService

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CaricaCentroAziendale_GIS(ByVal objP_server As String, ByVal Piva As String) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

        Try

            Dim ddl_centro As New DropDownList
            Dim xRisp As String = AgronicaControlliGIS.V_M.CaricaCentroAziendale(Piva, False, objParametri_Server)

            xRisp = "<a>" & xRisp.Replace("&", "&amp;") & "</a>"
            Dim dRisp As XDocument = XDocument.Parse(xRisp)

            For Each vv In dRisp.Element("a").Elements("option")
                ddl_centro.Items.Add(New ListItem(vv.Value, vv.Attribute("value")))
            Next

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl_centro.Items, "Piva_Sa_Cod", "Sa_Nome")
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function CaricaCentroAziendale_GIS_NG(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

        Try

            Dim ddl_centro As New DropDownList
            Dim xRisp As String = AgronicaControlliGIS.V_M.CaricaCentroAziendale(piva, False, objParametri_Server)

            xRisp = "<a>" & xRisp.Replace("&", "&amp;") & "</a>"
            Dim dRisp As XDocument = XDocument.Parse(xRisp)

            For Each vv In dRisp.Element("a").Elements("option")
                ddl_centro.Items.Add(New ListItem(vv.Value, vv.Attribute("value")))
            Next

            r.RispostaStringa = AgronicaCoreUtility.CaricaListControl.itemsCollection_AsJsonArray(ddl_centro.Items, "Piva_Sa_Cod", "Sa_Nome")
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiCentriConFiltroUtente(ByVal objP_server As String,
                                                ByVal PrimaRiga_Flag As Boolean,
                                                ByVal PrimaRiga_Text As String,
                                                ByVal PrimaRiga_Value As String,
                                                ByVal Piva As String,
                                                ByVal Flag_SoloCentriAttivi As Boolean,
                                                ByVal Tipo_Value As Integer) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim ddl_Centri As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(ddl_Centri,
                                               PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                               Piva, Flag_SoloCentriAttivi, Tipo_Value,
                                               "", "", objParametri_Server)

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl_Centri.Items
                JArrayListaOp.Add(New JObject(New JProperty("sa_nome", i.Text), New JProperty("sa_cod", i.Value)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function LeggiCentriConFiltroUtente_NG(ByVal InData As CoreWS_Generic(Of LeggiCentriConFiltroUtente)) As RispostaStandard

        Dim r As New RispostaStandard()

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim ddl_Centri As New DropDownList
            AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(ddl_Centri,
                                               InData.InData.PrimaRiga_Flag, InData.InData.PrimaRiga_Text, InData.InData.PrimaRiga_Value,
                                                 InData.InData.Piva, InData.InData.Flag_SoloCentriAttivi, InData.InData.Tipo_Value,
                                               "", "", objParametri_Server)

            Dim JArrayListaOp As New JArray()
            For Each i As ListItem In ddl_Centri.Items
                JArrayListaOp.Add(New JObject(New JProperty("sa_nome", i.Text), New JProperty("sa_cod", i.Value)))
            Next

            'ritorno la tabella trasformata in json (aggiustando anche le colonne)
            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Newtonsoft.Json.Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = ex.Message
            Return r
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetCentri(ByVal piva As String, ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            'Dim dt As DataTable = objR.Leggi(piva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
            Dim dt As DataTable = objR.Leggi_x_anagrafica(piva, 0, "", "", objParametri_server)


            Dim lista As New List(Of String)
            Dim listaJ As New JArray

            For Each dr As DataRow In dt.Rows
                Dim objCentro As New JObject

                objCentro.Add(New JProperty("piva", dr.Item("piva")))
                objCentro.Add(New JProperty("sa_cod", dr.Item("sa_cod")))
                objCentro.Add(New JProperty("sa_nome", dr.Item("sa_nome")))
                objCentro.Add(New JProperty("cod_indirizzo", dr.Item("cod_indirizzo")))
                objCentro.Add(New JProperty("ind_des", dr.Item("ind_des")))
                objCentro.Add(New JProperty("frz_des", dr.Item("frz_des")))
                objCentro.Add(New JProperty("CAP", dr.Item("CAP")))
                objCentro.Add(New JProperty("Istat_Com", dr.Item("com_cod_istat")))
                objCentro.Add(New JProperty("Istat_Prov", dr.Item("pro_cod_istat")))
                objCentro.Add(New JProperty("stato", dr.Item("stato")))
                objCentro.Add(New JProperty("note", dr.Item("note")))
                objCentro.Add(New JProperty("Comune_Des", dr.Item("com_des")))
                objCentro.Add(New JProperty("Provincia_Des", dr.Item("pro_cod")))

                listaJ.Add(objCentro)
                lista.Add("{""sa_nome"":""" & jSon.Escape(dr.Item("sa_nome")) & """, ""sa_cod"":""" & dr.Item("sa_cod") & """}")
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = listaJ.ToString


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
    Public Function GetCentriZoo(ByVal piva As String, ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            'Dim dt As DataTable = objR.Leggi(piva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
            Dim dt As DataTable = objR.Leggi_x_anagraficaZoo(piva, 0, "", "", objParametri_server)


            Dim lista As New List(Of String)
            Dim listaJ As New JArray

            For Each dr As DataRow In dt.Rows
                Dim objCentro As New JObject

                objCentro.Add(New JProperty("piva", dr.Item("piva")))
                objCentro.Add(New JProperty("sa_cod", dr.Item("sa_cod")))
                objCentro.Add(New JProperty("sa_nome", dr.Item("sa_nome")))
                objCentro.Add(New JProperty("cod_indirizzo", dr.Item("cod_indirizzo")))
                objCentro.Add(New JProperty("ind_des", dr.Item("ind_des")))
                objCentro.Add(New JProperty("frz_des", dr.Item("frz_des")))
                objCentro.Add(New JProperty("CAP", dr.Item("CAP")))
                objCentro.Add(New JProperty("Istat_Com", dr.Item("com_cod_istat")))
                objCentro.Add(New JProperty("Istat_Prov", dr.Item("pro_cod_istat")))
                objCentro.Add(New JProperty("stato", dr.Item("stato")))
                objCentro.Add(New JProperty("note", dr.Item("note")))
                objCentro.Add(New JProperty("Comune_Des", dr.Item("com_des")))
                objCentro.Add(New JProperty("Provincia_Des", dr.Item("pro_cod")))

                listaJ.Add(objCentro)
                lista.Add("{""sa_nome"":""" & jSon.Escape(dr.Item("sa_nome")) & """, ""sa_cod"":""" & dr.Item("sa_cod") & """}")
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = listaJ.ToString


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
    Public Function GetCentri_NG(InData As Object) As RispostaStandard

        Dim r As New RispostaStandard
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        Dim piva As String = InData.InData

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            End If


            Dim objR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            'Dim dt As DataTable = objR.Leggi(piva, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_server)
            Dim dt As DataTable = objR.Leggi_x_anagrafica(piva, 0, "", "", objParametri_server)


            Dim lista As New List(Of String)
            Dim listaJ As New JArray

            For Each dr As DataRow In dt.Rows
                Dim objCentro As New JObject

                objCentro.Add(New JProperty("piva", dr.Item("piva")))
                objCentro.Add(New JProperty("sa_cod", dr.Item("sa_cod")))
                objCentro.Add(New JProperty("sa_nome", dr.Item("sa_nome")))
                objCentro.Add(New JProperty("cod_indirizzo", dr.Item("cod_indirizzo")))
                objCentro.Add(New JProperty("ind_des", dr.Item("ind_des")))
                objCentro.Add(New JProperty("frz_des", dr.Item("frz_des")))
                objCentro.Add(New JProperty("CAP", dr.Item("CAP")))
                objCentro.Add(New JProperty("Istat_Com", dr.Item("com_cod_istat")))
                objCentro.Add(New JProperty("Istat_Prov", dr.Item("pro_cod_istat")))
                objCentro.Add(New JProperty("stato", dr.Item("stato")))
                objCentro.Add(New JProperty("note", dr.Item("note")))
                objCentro.Add(New JProperty("Comune_Des", dr.Item("com_des")))
                objCentro.Add(New JProperty("Provincia_Des", dr.Item("pro_cod")))

                listaJ.Add(objCentro)
                lista.Add("{""sa_nome"":""" & jSon.Escape(dr.Item("sa_nome")) & """, ""sa_cod"":""" & dr.Item("sa_cod") & """}")
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista.Distinct.ToList) & "]"
            Dim strRisp As String = listaJ.ToString


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
    Public Function ScriviCentro(ByVal centro As String, ByVal objP_server As String, ByVal objP_utenti As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_server As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
                objParametri_server = HttpContext.Current.Session("ASG_objParametri_Server")
            Else
                objParametri_server = Utility.convertStringtoOBJparametri(objP_server)
            End If

            Dim objParametri_utenti As AgronicaCoreParametri
            If HttpContext.Current.Session("ASG_objParametri_Utenti") IsNot Nothing Then
                objParametri_utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
            Else
                objParametri_utenti = Utility.convertStringtoOBJparametri(objP_utenti)
            End If

            Dim objR As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim objW As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W

            Dim objCentro = JObject.Parse(centro)

            Dim sa_cod As Integer = objCentro.GetValue("sa_cod").ToString
            Dim piva As String = objCentro.GetValue("piva").ToString

            Dim TipoOperazione_Centro As enum_TipoOperazioneDB

            If sa_cod = 0 Then
                TipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
            ElseIf objR.Esiste_Centro2(piva, sa_cod, "", 0, "", objParametri_server) Then
                TipoOperazione_Centro = enum_TipoOperazioneDB.Modifica
            Else
                TipoOperazione_Centro = enum_TipoOperazioneDB.Scrittura
            End If

            Dim xmlDoc As XmlDocument

            Dim BaseCode, TopCode, Progressivo As Integer
            Dim obj As New AgronicaCoreUtentiDAL.Utenti_CodiciGiasPRO_R
            Dim dt As DataTable
            dt = obj.Leggi("", "", objParametri_utenti)
            Progressivo = dt.Rows(0).Item("progressivogias")

            'ricavo base e top 
            Dim objAgroSeq As New AgronicaCoreDataProvider.Agro_Sequenze
            objAgroSeq.Calcola_BaseCode(Progressivo,
                                    TopCode,
                                    BaseCode,
                                        objParametri_utenti
                                        )

            Dim streerore As String = ""

            Dim xCentro As XmlElement
            Dim objxml As New AgronicaCoreXML.XML_Anagrafe

            xCentro = objxml.XML_2_CentriAziendali(streerore,
                              xmlDoc,
                              BaseCode,
                              TopCode,
                              TipoOperazione_Centro,
                              piva,
                              sa_cod,
                              objCentro.GetValue("sa_nome").ToString,
                              1,
                               objCentro.GetValue("Istat_Prov").ToString,
                               objCentro.GetValue("Istat_Com").ToString,
                               objCentro.GetValue("cod_indirizzo").ToString,
                               objCentro.GetValue("ind_des").ToString,
                               objCentro.GetValue("frz_des").ToString,
                               objCentro.GetValue("CAP").ToString,
                               "ITALIA",
                               "",
                                Nothing,
                                Nothing,
)

            Dim objCentroBiz As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W
            Dim risp_boolean As Boolean

            Try
                risp_boolean = objCentroBiz.CentroAziendale_Scrivi(xCentro.OuterXml, piva, sa_cod, objParametri_server, objParametri_utenti)
                streerore = "sa_cod=" & sa_cod
            Catch ex As Exception
                streerore = ex.Message
            End Try
            Dim strFinale As String
            If risp_boolean Then
                If TipoOperazione_Centro = enum_TipoOperazioneDB.Modifica Then
                    strFinale = "Centro " + objCentro.GetValue("sa_nome").ToString + " modificato correttamente."
                Else
                    strFinale = "E' stato creato il centro: " + objCentro.GetValue("sa_nome").ToString + "."
                End If
            Else
                strFinale = streerore
            End If
            r.RispostaOK = True
            r.RispostaStringa = strFinale

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Centri_Anagrafica(InData As Object) As RispostaStandard
        Dim r As New RispostaStandard

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim iData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)

            Dim FinestraTemporaleInizio = objParametri_Server.FinestraTemporaleInizio
            Dim FinestraTemporaleFine = objParametri_Server.FinestraTemporaleFine

            If iData.InData.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = iData.InData.Data
                objParametri_Server.FinestraTemporaleFine = iData.InData.Data
            End If

            Dim dtAnagrafica As DataTable = objCentri.Leggi_x_anagraficaNG(iData.InData.Piva, 0, "", "", objParametri_Server, True)

            If iData.InData.Data <> AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO Then
                objParametri_Server.FinestraTemporaleInizio = iData.InData.Data
                objParametri_Server.FinestraTemporaleFine = iData.InData.Data
            End If

            'dtAnagrafica.Columns.AddRange({New DataColumn("Superficie_Totale", Type.GetType("System.Decimal")),
            '                           New DataColumn("Superficie_Tare", Type.GetType("System.Decimal")),
            '                           New DataColumn("SAU_Totale", Type.GetType("System.Decimal")),
            '                           New DataColumn("tipoAttivitaDes", Type.GetType("System.String"))
            '                          })


            'For i As Integer = 0 To dtAnagrafica.Rows.Count - 1

            '    'Recupero le Superfici
            '    Dim Sup_Totale, Sup_Bosco, Sup_Prati, Sup_Tare, SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale As Double
            '    objCentri.Recupera_Superfici_CentroAziendale(dtAnagrafica.Rows(i).Item("Piva"), dtAnagrafica.Rows(i).Item("sa_cod"),
            '                                        Sup_Totale, Sup_Bosco, Sup_Prati, Sup_Tare,
            '                                        SAU_Totale, SAU_Biologico, SAU_Conversione, SAU_Convenzionale,
            '                                        Date.Today, objParametri_Server)

            '    dtAnagrafica.Rows(i).Item("Superficie_Totale") = Format(Sup_Totale, "0.0000")
            '    dtAnagrafica.Rows(i).Item("Superficie_Tare") = Format(Sup_Tare, "0.0000")
            '    dtAnagrafica.Rows(i).Item("SAU_Totale") = Format(SAU_Totale, "0.0000")

            'Next

            Dim js As New AgronicaCoreDataProvider.JSON_DataTable
            'Dim risp As String = js.JSON_DataTable_Kendo(dtAnagrafica, l, False, False, TipoFiltroKendo_colonne.CasellaTesto,)
            Dim risp As String = JsonConvert.SerializeObject(dtAnagrafica)
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
    Public Function Carica_Comuni(ByVal provincia As String, objP_server As String) As rispostaStandard(Of ListItemCollection)
        Dim r As New rispostaStandard(Of ListItemCollection)
        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
            Dim centroServizio As New CentroAziendale_R
            r = centroServizio.CaricaComuni(provincia, objParametri_Server)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Centro_Anagrafica(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Piva = InData.InData.Piva
            Dim Sa_Cod = InData.InData.Sa_Cod
            Dim centro As New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale()
            centro = objCentri.Centro_Leggi_Anagrafica(Piva:=Piva,
                                                       Sa_Cod:=Sa_Cod,
                                                       Leggi_Impresa:=True,
                                                       Leggi_Indirizzo:=True,
                                                       Leggi_Codici:=True,
                                                       Leggi_Rubrica:=True,
                                                       Leggi_Catasto:=False,
                                                       objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = centro

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Leggi_Centro_Dropdowns(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of AnagrafeNG.CentroDropdownLists)
        Dim r As New rispostaStandard(Of AnagrafeNG.CentroDropdownLists)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim Piva = InData.InData.Piva
            Dim Sa_Cod = InData.InData.Sa_Cod

            Dim centroBiz As New CentroAziendaleNG_R
            Dim dropdownLists = centroBiz.OttieneDropdownCentroEdit(Piva, Sa_Cod, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = dropdownLists

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Scrivi_Centro_Anagrafica(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

        Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}

        Dim settings As New JsonSerializerSettings()
        settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

        Dim iData As CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale) =
            JsonConvert.DeserializeObject(
            Of CoreWS_Generic(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale))(JsonConvert.SerializeObject(InData, settings),
                                                                                      settings)

        If iData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If iData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(iData.objP.objP_utenti)
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local

            Dim objI As New AgronicaCoreMetaSchemaDAL.Istat_R

            Dim centro = objCentri.Scrivi_Centro_Anagrafica(iData.InData, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = iData.InData

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCentriAziendaliModello(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            'Istanzio gli oggetti InData.
            Dim strCentriAziendali As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataCentriAziendali As AgronicaCoreDTOStd.InData.Metaschema.LeggiCentriAziendali = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiCentriAziendali)(strCentriAziendali,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})


            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dim Dt = objCentri.Leggi_Filtro_Data(
                inDataCentriAziendali.impresa.partitaIva,
                0,
                inDataCentriAziendali.data,
                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                "",
                "",
                objParametri_Server
                )

            Dim listCentriAziendali As List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

            listCentriAziendali = (From row As DataRow In Dt.Rows
                                   Select New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(row("SA_COD"), row("PIVA"))) With {
                                       .nome = row("Sa_Nome"),
                                       .indirizzi = New List(Of IndirizzoAssociato) From {
                                           New IndirizzoAssociato() With {
                                               .tipo_Indirizzo = 1,
                                               .indirizzo = New Indirizzo() With {
                                                   .istatComune = New AgronicaCoreModelsSTD.metaschema.Istat() With {
                                                   .com = row("com_cod_istat"),
                                                   .prov = row("pro_cod_istat"),
                                                   .localita = row("com_des"),
                                                   .comuni_prov = row("pro_cod")
                                                   },
                                                   .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166() With {
                                                   .codice = row("stato"),
                                                   .descrizione = ""
                                                   }
                                               }
                                           }
                                       }
                                   }).ToList

            r.RispostaStringa = listCentriAziendali
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi_Centri_Aziendali_perSpecieImpianti(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            'Istanzio gli oggetti InData
            Dim strCentriAziendali As String = JsonConvert.SerializeObject(InData.InData)
            Dim inDataCentriAziendali As AgronicaCoreDTOStd.InData.Metaschema.LeggiCentriAziendali = JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiCentriAziendali)(strCentriAziendali,
                                                                                          New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local, .NullValueHandling = NullValueHandling.Ignore, .MissingMemberHandling = MissingMemberHandling.Ignore})

            Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dim veg_cod As Integer = 0
            Dim cul_cod As Integer = 0

            If inDataCentriAziendali.utilizzoTerreno IsNot Nothing Then
                If inDataCentriAziendali.utilizzoTerreno.classType = ClassType.Varieta Then
                    veg_cod = CType(inDataCentriAziendali.utilizzoTerreno, Varieta).specie.codice
                    cul_cod = CType(inDataCentriAziendali.utilizzoTerreno, Varieta).codice
                End If
            End If

            Dim filtra_validita_esercizi As Boolean = False

            If Not IsNothing(inDataCentriAziendali.filtra_validita_esercizi) Then
                filtra_validita_esercizi = inDataCentriAziendali.filtra_validita_esercizi
                objParametri_Server.FinestraTemporaleInizio = inDataCentriAziendali.data
                objParametri_Server.FinestraTemporaleFine = inDataCentriAziendali.data
            End If


            Dim Dt = objCentri.LeggiPerImpianti_Coltivazioni_Data(
                inDataCentriAziendali.impresa.partitaIva,
                0,
                0,
                Veg_Cod:=veg_cod,
                Cul_Cod:=cul_cod,
                Gru_Cod:=0,
                inDataCentriAziendali.data,
                enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                "",
                "",
                objParametri_Server,
                Filtra_Validita_Esercizi:=filtra_validita_esercizi)

            Dim listCentriAziendali As List(Of AgronicaCoreModelsSTD.anagrafiche.CentroAziendale)

            listCentriAziendali = (From row As DataRow In Dt.Rows
                                   Select New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale(New AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK(row("SA_COD"), row("PIVA"))) With {
                                        .nome = row("Sa_Nome"),
                                        .lat = row("lat"),
                                        .lng = row("long"),
                                        .indirizzi = New List(Of IndirizzoAssociato) From {New IndirizzoAssociato() With {
                                            .tipo_Indirizzo = 1,
                                            .indirizzo = New Indirizzo() With {
                                                .istatComune = New AgronicaCoreModelsSTD.metaschema.Istat() With {
                                                    .com = row("com_cod_istat"),
                                                    .prov = row("pro_cod_istat"),
                                                    .localita = row("com_des"),
                                                    .comuni_prov = row("pro_cod")
                                                },
                                                .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166() With {
                                                    .codice = row("stato"),
                                                    .descrizione = ""
                                                }
                                            }
                                        }}
                                   }).ToList

            r.RispostaStringa = listCentriAziendali
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.RispostaOK = False
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Test_Centri_Archivio_Lettura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

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

            Dim objCentriDAL_R As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim objCentriBIZ_R As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

            Dim dt = objCentriDAL_R.Leggi(InData.InData.Piva, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            r.ErroriGias = New List(Of ErroreGias)
            Dim i = 0
            For Each row In dt.Rows
                i += 1

                Try
                    Dim m = objCentriBIZ_R.Centro_Leggi_Anagrafica(row("Piva"), row("Sa_Cod"), True, True, True, True, False, objParametri_Server)

                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Centro " & row("Piva") & "_" & row("Sa_Cod")}
                                     )

                End Try
            Next

            r.RispostaOK = True
            r.RispostaStringa = True

            If r.ErroriGias.Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = False
            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Test_Centri_Archivio_Scrittura(ByVal InData As CoreWS_Generic(Of AgronicaCoreGestioneRichieste.Parametri_ObjParametriAgenda_NG)) As rispostaStandard(Of Boolean)

        Dim r As New rispostaStandard(Of Boolean)

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

            Dim objCentriDAL_R As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
            Dim objCentriBIZ_R As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R
            Dim objCentriBIZ_W As New AgronicaCoreAnagrafeBIZ.CentroAziendale_W

            Dim dt = objCentriDAL_R.Leggi(InData.InData.Piva, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            r.ErroriGias = New List(Of ErroreGias)
            Dim i = 0
            For Each row In dt.Rows
                i += 1
                Dim m As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale

                Try
                    m = objCentriBIZ_R.Centro_Leggi_Anagrafica(row("Piva"), row("Sa_Cod"), True, True, True, True, False, objParametri_Server)
                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                     .ex = ex.Message,
                                     .messaggio = "Errore Lettura Centro " & row("Piva") & "_" & row("Sa_Cod")}
                                     )

                End Try

                Try
                    If m IsNot Nothing Then
                        objCentriBIZ_W.Scrivi_Centro_Anagrafica(m, objParametri_Server, objParametri_Utenti)
                    End If

                Catch ex As Exception

                    r.ErroriGias.Add(New ErroreGias() With {
                                         .ex = ex.Message,
                                         .messaggio = "Errore Scrittura Centro " & row("Piva") & "_" & row("Sa_Cod")}
                                         )

                End Try


            Next

            r.RispostaOK = True
            r.RispostaStringa = True

            If r.ErroriGias.Count > 0 Then
                r.RispostaOK = False
                r.RispostaStringa = False
            End If



        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = False
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try
        Return r
    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Carica_Centri_Impresa_NG(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.CaricaCentriImpresa)) As rispostaStandard(Of Object)
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
            Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim piveList = JsonConvert.DeserializeObject(Of List(Of String))(InData.InData.Lista)

            Dim listItems = objCentri.Carica_centriXImprese(piveList, objParametri_Server, objParametri_Utenti)


            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(listItems)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function

    <WebMethod()> <Script.Services.ScriptMethod()>
    Public Function Carica_Centri_Imprese(InData As Object) As rispostaStandard(Of Object)
        Dim r As New rispostaStandard(Of Object)

        If InData.item("objP").item("objP_server") = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.item("objP").item("objP_utenti") = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.item("objP").item("objP_server"))
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.item("objP").item("objP_utenti"))

            Dim jArr = JArray.Parse(InData.item("InData"))
            Dim piveList = jArr.ToObject(Of List(Of String))

            Dim listItems = objCentri.Carica_centriXImprese(piveList, objParametri_Server, objParametri_Utenti)


            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(listItems)
        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
            r.ErroriGias = New List(Of ErroreGias) From {AgronicaCoreDataProvider.Gestione_Eccezioni_2015.ErrorHandler(ex)}
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function readCentreAddresses(ByVal InData As CoreWS_Generic(Of LeggiIndirizziCentro)) As rispostaStandard(Of List(Of IndirizzoAssociato))
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.anagrafiche.IndirizzoAssociato))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.objP.objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Try
            Dim objCentri As New AgronicaCoreAnagrafeBIZ.CentroAziendale_R

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_utenti)

            Dim piva = InData.InData.piva
            Dim sa_cod = InData.InData.sa_cod
            Dim indirizzi As New List(Of IndirizzoAssociato)
            indirizzi = objCentri.readCentreAddresses(piva, sa_cod, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = indirizzi

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

End Class