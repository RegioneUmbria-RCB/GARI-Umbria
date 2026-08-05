Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreMetaSchemaDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class FasiCicloColturale
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function CaricaComboStatoImpianto_NG(InData As CoreWS_Generic(Of CaricaComboStatoImpianto)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.InData.Veg_Cod = 0 Or InData.InData.Grfi_Cod = 0 Or InData.InData.Reg_Cod = 0 Then
            r.RispostaStringa = ""
            Return r
        End If

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            'DRUDI CHIAMATA NUOVA
            'Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            'Dim GiasOnline_WS_Disciplinari_AgroWS_Disciplinari As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)

            'Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_StatoImpianto_output
            'Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_StatoImpianto_input
            'objParametriIngresso.Grfi_Cod = Grfi_Cod
            'objParametriIngresso.Regolamento_Cod = Reg_Cod
            'objParametriIngresso.Veg_Cod = Veg_Cod
            ''objParametriIngresso.url = GiasOnline_WS_Disciplinari_AgroWS_Disciplinari

            'Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            'objParametriUscita = objPC_WS.StatoImpianto(objParametriIngresso, GiasOnline_WS_Disciplinari_AgroWS_Disciplinari)

            'Dim containsFirstValue As Boolean = False

            'If objParametriUscita.ListaStati.Count > 0 Then

            '    If PrimaRiga_Flag Then
            '        For Each stato In objParametriUscita.ListaStati
            '            If stato.Codice = PrimaRiga_Value Then
            '                containsFirstValue = True
            '            End If
            '        Next
            '    End If

            'End If

            'Dim JArrayLista As New JArray()
            'If PrimaRiga_Flag And Not containsFirstValue Then
            '    JArrayLista.Add(New JObject(New JProperty("id_fase", PrimaRiga_Value, New JProperty("fase_des", PrimaRiga_Text))))
            'End If

            'For Each stato In objParametriUscita.ListaStati
            '    JArrayLista.Add(New JObject(New JProperty("id_fase", stato.Codice), New JProperty("fase_des", stato.Descrizione)))
            'Next

            'DRUDI CHIAMATA VECCHIA
            Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

            Dim dt = objGruppoFinalita.FasiCicloColturale_Leggi(InData.InData.Veg_Cod, InData.InData.Grfi_Cod, InData.InData.Reg_Cod, "", "", objParametri_Server)

            Dim containsFirstValue As Boolean = False
            If InData.InData.PrimaRiga_Flag Then
                For Each row In dt.Rows
                    If row.item("grfi_Cod") = CInt(InData.InData.PrimaRiga_Value) Then
                        containsFirstValue = True
                    End If
                Next
            End If


            Dim JArrayLista As New JArray()
            If InData.InData.PrimaRiga_Flag And Not containsFirstValue Then
                JArrayLista.Add(
                    New JObject(New JProperty("id_fase", InData.InData.PrimaRiga_Value), New JProperty("fase_des", InData.InData.PrimaRiga_Text)))
            End If

            For Each stato In dt.Rows
                JArrayLista.Add(New JObject(New JProperty("id_fase", stato.item("Grfi_Cod")), New JProperty("fase_des", stato.item("Grfi_Des"))))
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
    Public Function CaricaComboStatoImpianto(objP_server As String,
                                        Veg_Cod As Integer,
                                        Grfi_Cod As String,
                                        Reg_Cod As Integer,
                                        PrimaRiga_Flag As Boolean,
                                        PrimaRiga_Text As String,
                                        PrimaRiga_Value As String) As RispostaStandard

        Dim r As New RispostaStandard

        If Veg_Cod = 0 Or Grfi_Cod = 0 Or Reg_Cod = 0 Then
            r.RispostaStringa = ""
            Return r
        End If

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            'DRUDI CHIAMATA NUOVA
            'Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            'Dim GiasOnline_WS_Disciplinari_AgroWS_Disciplinari As String = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)

            'Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_StatoImpianto_output
            'Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_StatoImpianto_input
            'objParametriIngresso.Grfi_Cod = Grfi_Cod
            'objParametriIngresso.Regolamento_Cod = Reg_Cod
            'objParametriIngresso.Veg_Cod = Veg_Cod
            ''objParametriIngresso.url = GiasOnline_WS_Disciplinari_AgroWS_Disciplinari

            'Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            'objParametriUscita = objPC_WS.StatoImpianto(objParametriIngresso, GiasOnline_WS_Disciplinari_AgroWS_Disciplinari)

            'Dim containsFirstValue As Boolean = False

            'If objParametriUscita.ListaStati.Count > 0 Then

            '    If PrimaRiga_Flag Then
            '        For Each stato In objParametriUscita.ListaStati
            '            If stato.Codice = PrimaRiga_Value Then
            '                containsFirstValue = True
            '            End If
            '        Next
            '    End If

            'End If

            'Dim JArrayLista As New JArray()
            'If PrimaRiga_Flag And Not containsFirstValue Then
            '    JArrayLista.Add(New JObject(New JProperty("id_fase", PrimaRiga_Value, New JProperty("fase_des", PrimaRiga_Text))))
            'End If

            'For Each stato In objParametriUscita.ListaStati
            '    JArrayLista.Add(New JObject(New JProperty("id_fase", stato.Codice), New JProperty("fase_des", stato.Descrizione)))
            'Next

            'DRUDI CHIAMATA VECCHIA
            Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

            Dim dt = objGruppoFinalita.FasiCicloColturale_Leggi(Veg_Cod, Grfi_Cod, Reg_Cod, "", "", objParametri_Server)

            Dim containsFirstValue As Boolean = False
            If PrimaRiga_Flag Then
                For Each row In dt.Rows
                    If row.item("grfi_Cod") = CInt(PrimaRiga_Value) Then
                        containsFirstValue = True
                    End If
                Next
            End If


            Dim JArrayLista As New JArray()
            If PrimaRiga_Flag And Not containsFirstValue Then
                JArrayLista.Add(
                    New JObject(New JProperty("id_fase", PrimaRiga_Value), New JProperty("fase_des", PrimaRiga_Text)))
            End If

            For Each stato In dt.Rows
                JArrayLista.Add(New JObject(New JProperty("id_fase", stato.item("Grfi_Cod")), New JProperty("fase_des", stato.item("Grfi_Des"))))
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
    Public Function Leggi_FasiCicloColturalexSpecie(InData As CoreWS_Generic(Of Leggi_FasiCicloColturalexSpecie)) As RispostaStandard

        Dim r As New RispostaStandard

        If InData.InData.Veg_Cod = 0 Then
            r.RispostaStringa = ""
            Return r
        End If

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objGruppoFinalita As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

            Dim dt = objGruppoFinalita.FasiCicloColturale_Anagrafiche(InData.InData.Veg_Cod, "", "", objParametri_Server)

            Dim JArrayLista As New JArray()

            For Each stato In dt.Rows
                JArrayLista.Add(New JObject(New JProperty("fase_cod", stato.item("Fase_Cod")), New JProperty("fase_des", stato.item("Fase_Des"))))
            Next

            r.RispostaStringa = JsonConvert.SerializeObject(JArrayLista, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
End Class