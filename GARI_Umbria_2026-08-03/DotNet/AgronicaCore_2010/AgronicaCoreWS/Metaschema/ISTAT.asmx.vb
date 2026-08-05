Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreUtility
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class ISTAT
    Inherits System.Web.Services.WebService


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function GetProvincie(ByVal valori_regioni As String, ByVal stato As String, objP_Server As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try



            Dim strP As String = "GetProvincie_" & valori_regioni
            If IsNothing(System.Web.HttpContext.Current.Cache(strP)) Or stato <> "" Then

                'Dim objParametri_server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
                Dim objParametri_server = Utility.convertStringtoOBJparametri(objP_Server)

                Dim objR As New AgronicaCoreMetaSchemaDAL.Lista_Province_R

                Dim regioniFiltro As String = ""
                If Not String.IsNullOrEmpty(valori_regioni) Then
                    regioniFiltro = " Lista_Province.REG in(" & valori_regioni & ")"

                End If



                Dim dt As DataTable = objR.Leggi("", "", enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                 regioniFiltro, "", objParametri_server, stato)

                Dim lista As New List(Of String)
                For Each dr As DataRow In dt.Rows
                    lista.Add("{""sigla"":""" & jSon.Escape(dr.Item("SIGLA")) & """,""regione_cod"":""" & jSon.Escape(dr.Item("REG")) & """,""regione_des"":""" & jSon.Escape(dr.Item("regione_des")) & """, ""Provincia_Des"":""" & jSon.Escape(dr.Item("provincia")) & """, ""Istat_Prov"":""" & dr.Item("prov") & """, ""Stato_Country"":""" & dr.Item("Stato_Country") & """}")
                Next


                Dim strRisp As String = "[" & String.Join(",", lista) & "]"

                System.Web.HttpContext.Current.Cache(strP) = strRisp

                r.RispostaOK = True
                r.RispostaStringa = strRisp


            Else
                r.RispostaStringa = System.Web.HttpContext.Current.Cache(strP)
                r.RispostaOK = True
            End If
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
    Public Function GetComuni(ByVal valori_provincia As String, objP_Server As String) As RispostaStandard

        Dim r As New RispostaStandard


        Try


            Dim objParametri_server = Utility.convertStringtoOBJparametri(objP_Server)

            Dim objR As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim xFiltroAggiuntivo As String
            If Not String.IsNullOrEmpty(valori_provincia) Then
                xFiltroAggiuntivo = " Lista_Province.prov in(" & valori_provincia & ")"
            End If

            Dim dt As DataTable = objR.Leggi(valori_provincia, "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)

            Dim lista As New List(Of String)
            'lista.Add("{""provincia_des"":""" & "nessuno" & """, ""descrizione"":""" & "nessuno" & """, ""cod_istat"":""" & "" & """}")
            For Each dr As DataRow In dt.Rows
                lista.Add("{""Provincia_Des"":""" & jSon.Escape(dr.Item("COMUNI_PROV")) & """, ""Comune_Des"":""" & jSon.Escape(dr.Item("LOCALITA")) & """, ""Istat_Com"":""" & dr.Item("COM") & """}")
            Next


            Dim strRisp As String = "[" & String.Join(",", lista) & "]"


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
    Public Function GetComuni_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.GetComuni)) As RispostaStandard

        Dim r As New RispostaStandard


        Try


            Dim objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreMetaSchemaDAL.Istat_R
            Dim xFiltroAggiuntivo As String
            If Not String.IsNullOrEmpty(InData.InData.Prov) Then
                xFiltroAggiuntivo = " Lista_Province.prov in(" & InData.InData.Prov & ")"
            End If

            Dim dt As DataTable = objR.Leggi(InData.InData.Prov, "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)

            Dim lista As New List(Of String)
            'lista.Add("{""provincia_des"":""" & "nessuno" & """, ""descrizione"":""" & "nessuno" & """, ""cod_istat"":""" & "" & """}")
            For Each dr As DataRow In dt.Rows
                lista.Add("{""Provincia_Des"":""" & jSon.Escape(dr.Item("COMUNI_PROV").ToString()) & """, ""Comune_Des"":""" & jSon.Escape(dr.Item("LOCALITA")) & """, ""Istat_Com"":""" & dr.Item("COM") & """}")
            Next


            Dim strRisp As String = "[" & String.Join(",", lista) & "]"


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
    Public Function GetStati(objP_Server As String) As RispostaStandard

        Dim r As New RispostaStandard


        Try


            Dim objParametri_server = Utility.convertStringtoOBJparametri(objP_Server)

            Dim objR As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R

            Dim dt As DataTable = objR.Leggi("", "", "", objParametri_server)


            Dim arr As New JArray
            Dim lista As New List(Of String)
            'lista.Add("{""provincia_des"":""" & "nessuno" & """, ""descrizione"":""" & "nessuno" & """, ""cod_istat"":""" & "" & """}")
            For Each dr As DataRow In dt.Rows
                'lista.Add("{""descrizione"":""" & jSon.Escape(dr.Item("descrizione")) & """, ""codice"":""" & jSon.Escape(dr.Item("codice")) & """ }")
                Dim jobj As New JObject
                jobj("descrizione") = CStr(dr.Item("descrizione"))
                jobj("codice") = CStr(dr.Item("codice"))
                jobj("gestione_gerarchia_geografica") = CStr(dr.Item("Gestione_Gerarchia_Geografica"))
                arr.Add(jobj)
            Next


            'Dim strRisp As String = "[" & String.Join(",", lista) & "]"
            Dim strRisp As String = arr.ToString

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
    Public Function GetCAP(Prov As String, Com As String, objP_Server As String) As RispostaStandard

        Dim r As New RispostaStandard


        Try


            Dim objParametri_server = Utility.convertStringtoOBJparametri(objP_Server)
            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

            Dim dt = objIstat.Leggi(Prov, Com, "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)

            Dim res = "00000"

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                res = dt.Rows(0)("CAP")
            End If

            r.RispostaOK = True
            r.RispostaStringa = res

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
    Public Function GetCAP_NG(ByVal InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.GetCAP)) As RispostaStandard
        Dim r As New RispostaStandard


        Try


            Dim objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim objIstat As New AgronicaCoreMetaSchemaDAL.Istat_R

            Dim dt = objIstat.Leggi(InData.InData.Prov, InData.InData.Com, "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_server)

            Dim res = "00000"

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                res = dt.Rows(0)("CAP")
            End If

            r.RispostaOK = True
            r.RispostaStringa = res

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


            r.Errore = MessaggioErrore


        End Try

        Return r

    End Function

    <WebMethod()>
    Public Function GetStati_Modello_NG(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.GetStatiModello)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166))
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.GetStatiModello))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        'se passo il valore di ricerca a null (omesso) forso a blank per non rompere il ws
        If InData.InData.ValoreRicerca Is Nothing Then
            InData.InData.ValoreRicerca = ""
        End If

        Try


            Dim objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R

            Dim dt As DataTable = objR.Leggi("", "", "", objParametri_server)


            Dim list = (From dr In dt.Rows Select New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166() With {
                                               .codice = dr("codice"),
                                               .descrizione = dr("descrizione"),
                                               .gestioneGerarchia = dr("Gestione_Gerarchia_Geografica")
            }).ToList()

            r.RispostaOK = True
            r.RispostaStringa = list
        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.Errore = MessaggioErrore
        End Try

        Return r
    End Function

    <WebMethod()>
    Public Function GetStati_Modello(InData As CoreWS_Generic(Of Object)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166))
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of Object))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        'se passo il valore di ricerca a null (omesso) forso a blank per non rompere il ws
        If InData.InData Is Nothing Then
            InData.InData = ""
        End If

        Try


            Dim objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreMetaSchemaDAL.ACCDAA_ANAG_T005_TabellaCodiciPaesiTerziISO3166_R

            Dim dt As DataTable = objR.Leggi("", "", "", objParametri_server)


            Dim list = (From dr In dt.Rows Select New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166() With {
                                               .codice = dr("codice"),
                                               .descrizione = dr("descrizione"),
                                               .gestioneGerarchia = dr("Gestione_Gerarchia_Geografica")
            }).ToList()

            r.RispostaOK = True
            r.RispostaStringa = list
        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.Errore = MessaggioErrore
        End Try

        Return r
    End Function


    <WebMethod()>
    Public Function GetProvince_Modello(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Provincia))
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Provincia))
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of GetProvincie))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        'se passo il valore di ricerca a null (omesso) forso a blank per non rompere il ws
        If InData.InData Is Nothing Then
            InData.InData = ""
        End If

        Try
            Dim objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
            Dim statoCountry As String = "IT"

            Dim xFiltroAggiuntivo As String = ""
            If Not String.IsNullOrEmpty(InData.InData.stato) Then
                Dim filtro = CStr(InData.InData.stato).Split("|")
                If filtro.Length > 1 Then
                    xFiltroAggiuntivo = " Lista_Province.Stato_Country IN ('" & Replace(filtro(1), ",", "','") & "') "
                    statoCountry = ""
                Else
                    statoCountry = InData.inData.stato
                    'xFiltroAggiuntivo = " Lista_Province.REG IN ('" & Replace(InData.inData, ",", "','") & "') "
                End If
            End If

            Dim dt As DataTable = objR.Leggi("", InData.InData.regione, enumSelezioneVariabile.Selezione_JoinDescrizioni, xFiltroAggiuntivo, "", objParametri_server, statoCountry)

            Dim lista As New List(Of AgronicaCoreModelsSTD.metaschema.Provincia)
            For Each dr As DataRow In dt.Rows
                lista.Add(
                    New AgronicaCoreModelsSTD.metaschema.Provincia(dr.Item("PROV")) With {
                        .descrizione = dr.Item("PROVINCIA"),
                        .sigla = dr.Item("SIGLA"),
                        .regione = New AgronicaCoreModelsSTD.metaschema.Regione(dr.Item("REG")) With {.descrizione = dr.Item("Regione_Des")},
                        .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166(dr.Item("Stato_Country")),
                        .comuneDefault = dr.Item("Com")
                    })
            Next

            r.RispostaOK = True
            r.RispostaStringa = lista
        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.Errore = MessaggioErrore
        End Try

        Return r
    End Function

    <WebMethod()>
    Public Function GetComuni_Modello(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Comune))
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Comune))
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        'se passo il valore di ricerca a null (omesso) forso a blank per non rompere il ws
        If InData.inData Is Nothing Then
            InData.inData = ""
        End If

        Try

            Dim objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objR As New AgronicaCoreMetaSchemaDAL.Istat_R

            Dim xFiltroAggiuntivo As String = ""
            If Not String.IsNullOrEmpty(InData.inData) Then
                Dim filtro = CStr(InData.inData).Split("|")
                If filtro.Length > 1 Then
                    xFiltroAggiuntivo = " Stato_Country IN ('" & Replace(filtro(1), ",", "','") & "') "
                Else
                    xFiltroAggiuntivo = " PROV IN ('" & Replace(InData.inData, ",", "','") & "') "
                End If
            End If

            Dim dt As DataTable = objR.Leggi("", "", "", "", "", enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri_server)

            Dim lista As New List(Of AgronicaCoreModelsSTD.metaschema.Comune)
            For Each dr As DataRow In dt.Rows
                lista.Add(
                    New AgronicaCoreModelsSTD.metaschema.Comune(dr.Item("COM")) With {
                        .descrizione = dr.Item("LOCALITA"),
                        .cap = If(IsDBNull(dr.Item("CAP")), "", dr.Item("CAP")),
                        .codiceCatastale = If(IsDBNull(dr.Item("CodiceCatastale")), "", dr.Item("CodiceCatastale")),
                        .provincia = New AgronicaCoreModelsSTD.metaschema.Provincia(dr.Item("PROV")) With {
                            .stato = New AgronicaCoreModelsSTD.metaschema.CodiciNazioniISO3166(dr.Item("Stato_Country"))
                        }
                    })
            Next

            r.RispostaOK = True
            r.RispostaStringa = lista

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
    Public Function readRegioni(InData As Object) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Regione))
        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.Regione))
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.InData Is Nothing Then
            InData.InData = ""
        End If

        Try
            Dim objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim dtRegioni As DataTable = Nothing

            Dim objRegioniRead As New AgronicaCoreMetaSchemaDAL.Lista_Regioni_R
            dtRegioni = objRegioniRead.Leggi("", "", "", objParametri_server, InData.InData)

            Dim lista = (From d In dtRegioni.AsEnumerable() Select New With
                                                                {
                                                                .codice = d.Item("REG").ToString(),
                                                                .descrizione = d.Item("Regione_Des").ToString()
                                                                }).ToList

            r.RispostaOK = True
            r.RispostaStringa = lista.Select(Of AgronicaCoreModelsSTD.metaschema.Regione)(Function(regione) New AgronicaCoreModelsSTD.metaschema.Regione(regione.codice) With {.descrizione = regione.descrizione}).ToList
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
    Public Function readRegionsByProvince(InData As Object) As rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.Regione)
        Dim r As New rispostaStandard(Of AgronicaCoreModelsSTD.metaschema.Regione)
        InData = JsonConvert.DeserializeObject(Of CoreWS_Generic(Of String))(JsonConvert.SerializeObject(InData))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If InData.InData Is Nothing Then
            InData.InData = ""
        End If

        Try
            Dim objParametri_server = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim dtRegioni As DataTable = Nothing

            Dim objRegioniRead As New AgronicaCoreMetaSchemaDAL.Lista_Regioni_R
            dtRegioni = objRegioniRead.ReadByProvince(InData.InData, objParametri_server)

            Dim regione As New AgronicaCoreModelsSTD.metaschema.Regione
            If dtRegioni.Rows.Count > 0 Then
                regione.codice = dtRegioni.Rows(0)("REG")
                regione.descrizione = dtRegioni.Rows(0)("Regione_Des")
            End If

            r.RispostaOK = True
            r.RispostaStringa = regione
        Catch ex As Exception
            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            r.Errore = MessaggioErrore
        End Try

        Return r
    End Function

End Class