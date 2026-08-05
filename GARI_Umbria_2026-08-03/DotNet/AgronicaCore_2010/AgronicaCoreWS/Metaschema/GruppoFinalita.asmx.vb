Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports AgronicaCoreDTOStd.InData.Metaschema
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaControlliGIS

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class GruppoFinalita
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboFinalita_NG(InData As CoreWS_Generic(Of CaricaComboFinalita)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objCom As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
            Dim Dt As DataTable = objCom.Leggi(0,
                                                InData.InData.Veg_Cod,
                                                InData.InData.StringaCerca,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                InData.InData.FiltroAggiuntivo,
                                                    InData.InData.Ordinamento,
                                                    objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("grfi_cod", dr.Item("GRFI_COD")), New JProperty("grfi_des", dr.Item("GRFI_DES"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboFinalita(objP_server As String,
                                        Veg_Cod As Integer,
                                        StringaCerca As String,
                                        FiltroAggiuntivo As String, Ordinamento As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objCom As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
            Dim Dt As DataTable = objCom.Leggi(0,
                                                Veg_Cod,
                                                StringaCerca,
                                                AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                FiltroAggiuntivo,
                                                    Ordinamento,
                                                    objParametri_Server)

            Dim JArrayLista As New JArray()
            For Each dr In Dt.Rows
                JArrayLista.Add(New JObject(New JProperty("grfi_cod", dr.Item("GRFI_COD")), New JProperty("grfi_des", dr.Item("GRFI_DES"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboFinalita2_NG(InData As CoreWS_Generic(Of CaricaComboFinalita2)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
            End If

            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_input
            objParametriIngresso.Regolamento_Cod = InData.InData.Regolamento_Cod
            objParametriIngresso.Veg_Cod = InData.InData.Veg_Cod
            objParametriIngresso.Url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS

            'objParametriUscita = objPC_WS.FasiCicloColturaleStatoImpianto(objParametriIngresso)
            objParametriUscita = objPC_WS.FasiCicloColturale(objParametriIngresso)

            Dim JArrayLista As New JArray()
            For i = 0 To objParametriUscita.ListaFasi.Count - 1
                JArrayLista.Add(New JObject(New JProperty("grfi_cod", objParametriUscita.ListaFasi(i).Codice), New JProperty("grfi_des", objParametriUscita.ListaFasi(i).Descrizione)))
            Next

            'Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

            'Dim Dt = objGruppoFinalita.FasiCicloColturale_Leggi(Veg_Cod,
            '                                            Grfi_Cod,
            '                                            Regolamento_Cod,
            '                                            FiltroAggiuntivo,
            '                                            Ordinamento,
            '                                            objParametri_Server)

            'Dim JArrayLista As New JArray()
            'For Each dr In Dt.Rows
            '    JArrayLista.Add(New JObject(New JProperty("grfi_cod", dr.Item("GRFI_COD")), New JProperty("grfi_des", dr.Item("GRFI_DES"))))
            'Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboFinalita2(ByVal objP_super_server As String,
                                         objP_server As String,
                                    Veg_Cod As Integer,
                                    Grfi_Cod As Integer,
                                    Regolamento_Cod As Integer,
                                    StringaCerca As String,
                                    FiltroAggiuntivo As String, Ordinamento As String) As RispostaStandard

        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
            End If

            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_input
            objParametriIngresso.Regolamento_Cod = Regolamento_Cod
            objParametriIngresso.Veg_Cod = Veg_Cod
            objParametriIngresso.Url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS

            'objParametriUscita = objPC_WS.FasiCicloColturaleStatoImpianto(objParametriIngresso)
            objParametriUscita = objPC_WS.FasiCicloColturale(objParametriIngresso)

            Dim JArrayLista As New JArray()
            For i = 0 To objParametriUscita.ListaFasi.Count - 1
                JArrayLista.Add(New JObject(New JProperty("grfi_cod", objParametriUscita.ListaFasi(i).Codice), New JProperty("grfi_des", objParametriUscita.ListaFasi(i).Descrizione)))
            Next

            'Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

            'Dim Dt = objGruppoFinalita.FasiCicloColturale_Leggi(Veg_Cod,
            '                                            Grfi_Cod,
            '                                            Regolamento_Cod,
            '                                            FiltroAggiuntivo,
            '                                            Ordinamento,
            '                                            objParametri_Server)

            'Dim JArrayLista As New JArray()
            'For Each dr In Dt.Rows
            '    JArrayLista.Add(New JObject(New JProperty("grfi_cod", dr.Item("GRFI_COD")), New JProperty("grfi_des", dr.Item("GRFI_DES"))))
            'Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Leggi(InData As CoreWS_Generic(Of AgronicaCoreDTOStd.InData.Metaschema.LeggiFinalita)) As rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita))

        Dim r As New rispostaStandard(Of List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim cache As Cache
            If HttpContext.Current.Cache IsNot Nothing Then
                cache = HttpContext.Current.Cache
            End If

            Dim key = "GruppoFinalitaModello" &
                InData.InData.specie.codice &
                If(InData.InData.ParametriSementieri IsNot Nothing, InData.InData.ParametriSementieri.Sementi, "") &
                If(InData.InData.ParametriSementieri IsNot Nothing, InData.InData.ParametriSementieri.SementiMappaturaLibera, "")

            Dim listFinalita As List(Of AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita)

            If InData.InData.cache AndAlso cache IsNot Nothing AndAlso cache.Item(key) IsNot Nothing Then
                r.RispostaStringa = cache.Item(key)
                r.RispostaOK = True
                Return r
            End If
            '---------------------------------------------------------------------------------------------------------------------
            ' Salvatore Zammataro 04-05-2023: modifiche aggiunte per la gestione Sementieri
            Dim veg_cod_DT As New DataTable
            Dim xFiltroAggiuntivo As String = ""
            Dim mappaturaSpecieDaSportello As New AgronicaCoreSementieriDAL.Mappatura_Specie_R
            Dim codiceSportelloInt As Int32 = 0

            If InData.InData.ParametriSementieri IsNot Nothing AndAlso InData.InData.ParametriSementieri.Sementi <> "" Then

                Dim gp As New GisPurpose(InData.InData.ParametriSementieri.Sementi, If(InData.InData.ParametriSementieri.SementiMappaturaLibera = "True", "1", ""))
                Dim mode = gp.Mode()

                If mode = GisPurpose.enum_GisPurpose.SementiSportello Then

                    xFiltroAggiuntivo = " = " 'solo finalità mappata

                ElseIf mode = GisPurpose.enum_GisPurpose.SementiMappaturaLibera Then

                    xFiltroAggiuntivo = " <> " 'escludo finalità mappata

                End If

                If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then

                    Dim leggiObj As New AgronicaCoreSementieriDAL.Varie_R
                    Dim finalita As Integer = leggiObj.SpecieMappata_Finalita(InData.InData.specie.codice, objParametri_Server)
                    If finalita <> -1 Then

                        xFiltroAggiuntivo = " AND GruppoFinalita.Grfi_Cod" & xFiltroAggiuntivo & CStr(finalita) & " "
                    Else

                        xFiltroAggiuntivo = ""
                    End If
                End If
            End If
            '---------------------------------------------------------------------------------------------------------------------

            ' filtro specie vegetali x app
            Dim listaSpecie = InData.InData.listaSpecie
            If listaSpecie IsNot Nothing AndAlso listaSpecie.Count > 0 Then
                xFiltroAggiuntivo &= " AND GruppoFinalitaxSpecieVegetali.Veg_Cod IN (" & String.Join(",", listaSpecie) & ") "
            End If

            Dim objCom As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R
            Dim Dt As DataTable = objCom.Leggi(0, InData.InData.specie.codice, "",
                                               AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                               xFiltroAggiuntivo, "", objParametri_Server)

            listFinalita = (From dr In Dt.Rows
                            Select New AgronicaCoreModelsSTD.metaschema.utilizzi.GruppoFinalita(dr("grfi_cod")) With {
                                .descrizione = dr("grfi_des"),
                                .specieCod = dr("Veg_Cod")
                           }).ToList

            cache.Insert(key, listFinalita)

            r.RispostaStringa = listFinalita
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiFaseCicloColturale(InData As CoreWS_Generic(Of FiltroFinalita2)) As rispostaStandard(Of List(Of FaseCicloColturale))

        Dim r As New rispostaStandard(Of List(Of FaseCicloColturale))

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
            End If

            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_input
            'objParametriIngresso.Regolamento_Cod = InData.InData.regolamentoConcimazione.codice
            objParametriIngresso.Regolamento_Cod = 0
            objParametriIngresso.Veg_Cod = InData.InData.specie.codice
            objParametriIngresso.Url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS

            objParametriUscita = objPC_WS.FasiCicloColturale(objParametriIngresso)

            Dim JArrayLista As New List(Of FaseCicloColturale)
            For i = 0 To objParametriUscita.ListaFasi.Count - 1
                JArrayLista.Add(New FaseCicloColturale(objParametriUscita.ListaFasi(i).Codice) With {.descrizione = objParametriUscita.ListaFasi(i).Descrizione})
            Next

            r.RispostaStringa = JArrayLista
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function LeggiComboGruppoFinalitaNuovoImpianto(InData As CoreWS_Generic(Of String)) As RispostaStandard
        Dim r As New RispostaStandard

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        Try
            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)
            Dim ddl As New DropDownList
            AgronicaCoreUtility.CaricaListControl.GruppoFinalita(
                ddl,
                False, "", "",
                "", "", objParametri_Server
            )
            Dim list = (From item In ddl.Items
                        Select New AgronicaCoreModelsSTD.baseClass.BaseCodeDescr(item.Value, item.Text)
                       ).ToList()

            r.RispostaStringa = JsonConvert.SerializeObject(list)
            r.RispostaOK = True
        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

End Class