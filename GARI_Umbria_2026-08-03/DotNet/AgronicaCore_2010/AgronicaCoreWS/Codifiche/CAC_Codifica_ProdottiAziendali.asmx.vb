Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class CAC_Codifica_ProdottiAziendali
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiCodificaProdotti(ByVal Piva As String, ByVal Tipo_Codifica As Integer, ByVal Elem_Cod As Integer, ByVal objP_server As String) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try

            Dim objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

            Dim objCACProdotti As New AgronicaCoreAnagrafeDAL.CAC_Codifica_ProdottiAziendali_R
            Dim dtCACProdotti As DataTable = objCACProdotti.Leggi(Piva, Tipo_Codifica, Elem_Cod, "", "", "", "", objParametriServer)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaStringa = JsonConvert.SerializeObject(dtCACProdotti, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "LeggiCodificaProdotti, Errore: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    ' non esiste tabella in cui leggere questa roba
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function ElencoTipiCodifica(ByVal objP_server As String) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

            Dim jArrayListaOp As New JArray()

            Select Case objParametriServer.PivaSuperUser

                Case "01788291209" 'APOCONERPO
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito),
                                                    New JProperty("codifica_des", "Non definito")))
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.Agrintesa),
                                            New JProperty("codifica_des", "Agrintesa")))

                Case "01352530396" 'AGRISOL
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito),
                                                    New JProperty("codifica_des", "Non definito")))
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.Agrisol_Seled),
                                                    New JProperty("codifica_des", "Agrisol_Seled")))

                Case "03129920363" 'FRUIT MODENA
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito),
                                                    New JProperty("codifica_des", "Non definito")))
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.FruitModena_Seled),
                                                    New JProperty("codifica_des", "FruitModena_Seled")))

                Case "00554760397" 'FRANCESCONI
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.Francesconi),
                                                    New JProperty("codifica_des", "Francesconi")))

                Case "00069880391" 'TERREMERSE
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.Terremerse),
                                                    New JProperty("codifica_des", "Terremerse")))

                Case "03297010401" 'CEREALI ROMAGNA
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito),
                                                    New JProperty("codifica_des", "Non definito")))
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.ConsAgrRavenna),
                                                  New JProperty("codifica_des", "Cons. Agr. Ravenna")))

                Case "05644051004" 'COLDIRETTI NAZIONALE
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito),
                                                  New JProperty("codifica_des", "Non definito")))

                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_ConsAgrPerugia),
                                                New JProperty("codifica_des", "Cons. Agr. Perugia")))
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.Coldiretti_RegioneUmbria),
                                                  New JProperty("codifica_des", "Regione Umbria")))

                Case "02440620405" 'OROGEL
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_Specie.Orogel_Fresco_WMS),
                                            New JProperty("codifica_des", "WMS-ONPLANT OROGEL FRESCO")))
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_Specie.Orogel_Surgelato_WMS),
                                            New JProperty("codifica_des", "WMS-ONPLANT OROGEL SURGELATO")))
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_Specie.Orogel_Fresco_DBWIN),
                                            New JProperty("codifica_des", "DBWIN OROGEL FRESCO")))
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_Specie.Orogel_Surgelato_DBWIN),
                                            New JProperty("codifica_des", "DBWIN OROGEL SURGELATO")))
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito),
                                            New JProperty("codifica_des", "OROGEL-PASA")))

                Case Else
                    jArrayListaOp.Add(New JObject(New JProperty("codifica_cod", enum_Tipo_CAC_Codifica_ProdottiAziendali.NonDefinito),
                                                  New JProperty("codifica_des", "Non definito")))

            End Select


            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "ElencoTipiCodifica, Errore: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


End Class