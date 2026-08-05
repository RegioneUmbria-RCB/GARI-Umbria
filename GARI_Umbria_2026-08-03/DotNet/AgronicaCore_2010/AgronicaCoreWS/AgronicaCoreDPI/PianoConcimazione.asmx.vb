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
Imports AgronicaControlli_2010
Imports AgronicaCorePianoConcimazioneBIZ
Imports AgronicaCoreModelsSTD.metaschema
Imports AgronicaCoreDTOStd.InData.Metaschema

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class PianoConcimazione
    Inherits System.Web.Services.WebService

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CalcoloNPK(ByVal objP_super_server As String, objP_server As String, Regolamento As Integer, Veg_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            If Regolamento > 0 AndAlso Veg_Cod > 0 Then

                Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

                If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                    GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                End If

                Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
                Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
                objParametriIngresso.Regolamento_Cod = Regolamento
                objParametriIngresso.Veg_Cod = Veg_Cod
                objParametriIngresso.Grfi_Cod = 0
                objParametriIngresso.SoloValorizzati = True
                objParametriIngresso.SoloVisibili = False
                objParametriIngresso.url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione


                Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                objParametriUscita = objPC_WS.FattoriCorrettivi_Leggi(objParametriIngresso)
                If objParametriUscita.ListaFattoriCorrettivi.Count > 0 Then

                    '                    Cicli la lista ricevuta e ricavi 
                    'enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard
                    '                    enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard
                    '                    enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard


                    'Dim N = objParametriUscita.ListaFattoriCorrettivi.Item(enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard).Valore
                    'Dim P = objParametriUscita.ListaFattoriCorrettivi.Item(enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard).Valore
                    'Dim K = objParametriUscita.ListaFattoriCorrettivi.Item(enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard).Valore
                    Dim FattoreN As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    FattoreN = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard And x.Visibile = 1)(0)
                    If Not FattoreN Is Nothing Then
                        'ResaBassaDes.Value = FattoreResaBassa.Descrizione & " " & FattoreResaBassa.Valore
                    End If

                    Dim FattoreP As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    FattoreP = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard And x.Visibile = 1)(0)
                    If Not FattoreP Is Nothing Then
                        'ResaBassaDes.Value = FattoreResaBassa.Descrizione & " " & FattoreResaBassa.Valore
                    End If

                    Dim FattoreK As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    FattoreK = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard And x.Visibile = 1)(0)
                    If Not FattoreK Is Nothing Then
                        'ResaBassaDes.Value = FattoreResaBassa.Descrizione & " " & FattoreResaBassa.Valore
                    End If

                    Dim result = JsonConvert.SerializeObject(New JObject(New JProperty("N", FattoreN.Valore), New JProperty("P", FattoreP.Valore), New JProperty("K", FattoreK.Valore)), Formatting.None)
                    r.RispostaStringa = result
                End If

            End If

            r.RispostaOK = True

        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_FasiCicloColturale_WS(ByVal objP_super_server As String,
                                             ByVal objP_server As String,
                                             ByVal PrimaRiga_Flag As Boolean,
                                             ByVal PrimaRiga_Text As String,
                                             ByVal PrimaRiga_Value As String,
                                             ByVal Regolamento_Cod As Integer,
                                             ByVal Veg_Cod As Integer,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String
                                             ) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.PC_FasiCicloColturale_WS(ddl,
                                                                      PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                                      Regolamento_Cod, Veg_Cod,
                                                                      "", "",
                                                                      GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PUA_FasiCicloColturale_WS(ByVal objP_super_server As String,
                                              ByVal objP_server As String,
                                              ByVal PrimaRiga_Flag As Boolean,
                                              ByVal PrimaRiga_Text As String,
                                              ByVal PrimaRiga_Value As String,
                                              ByVal Regolamento_Cod As Integer,
                                              ByVal Veg_Cod As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String
                                              ) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.PUA_FasiCicloColturale_WS(ddl,
                                                                       PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                                       Regolamento_Cod, Veg_Cod,
                                                                       "", "",
                                                                       GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_PrecessioneColturale_WS(ByVal objP_super_server As String,
                                               ByVal objP_server As String,
                                               ByVal PrimaRiga_Flag As Boolean,
                                               ByVal PrimaRiga_Text As String,
                                               ByVal PrimaRiga_Value As String,
                                               ByVal Regolamento_Cod As Integer,
                                               ByVal Pua_Tipo As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String
                                               ) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.PC_PrecessioneColturale_WS(ddl,
                                                                        PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                                        Regolamento_Cod, Pua_Tipo,
                                                                        "", "",
                                                                        GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_MatriciOrganiche_WS(ByVal objP_super_server As String,
                                           ByVal objP_server As String,
                                           ByVal PrimaRiga_Flag As Boolean,
                                           ByVal PrimaRiga_Text As String,
                                           ByVal PrimaRiga_Value As String,
                                           ByVal Regolamento_Cod As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String
                                           ) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.PC_MatriciOrganiche_WS(ddl,
                                                                    PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                                    Regolamento_Cod,
                                                                    "", "",
                                                                    GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PUA_EffluentiXFrequenza_GetEffluente_WS(ByVal objP_super_server As String,
                                                            ByVal objP_server As String,
                                                            ByVal PrimaRiga_Flag As Boolean,
                                                            ByVal PrimaRiga_Text As String,
                                                            ByVal PrimaRiga_Value As String,
                                                            ByVal Regolamento_Cod As Integer,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String
                                                            ) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.PUA_EffluentiXFrequenza_GetEffluente_WS(ddl,
                                                                                     PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                                                     Regolamento_Cod,
                                                                                     "", "",
                                                                                     GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_Frequenza_WS(ByVal objP_super_server As String,
                                    ByVal objP_server As String,
                                    ByVal PrimaRiga_Flag As Boolean,
                                    ByVal PrimaRiga_Text As String,
                                    ByVal PrimaRiga_Value As String,
                                    ByVal Regolamento_Cod As Integer,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String
                                    ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.PC_Frequenza_WS(ddl,
                                                             PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                             Regolamento_Cod,
                                                             "", "",
                                                             GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_Ubicazione_WS(ByVal objP_super_server As String,
                                     ByVal objP_server As String,
                                     ByVal PrimaRiga_Flag As Boolean,
                                     ByVal PrimaRiga_Text As String,
                                     ByVal PrimaRiga_Value As String,
                                     ByVal Regolamento_Cod As Integer,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String
                                     ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.PC_Ubicazione_WS(ddl,
                                                             PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                             Regolamento_Cod,
                                                             "", "",
                                                             GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_TipoAcqua_WS(ByVal objP_super_server As String,
                                    ByVal objP_server As String,
                                    ByVal PrimaRiga_Flag As Boolean,
                                    ByVal PrimaRiga_Text As String,
                                    ByVal PrimaRiga_Value As String,
                                    ByVal Regolamento_Cod As Integer,
                                    ByVal TipoZona As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String
                                    ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.PC_TipoAcqua_WS(ddl,
                                                             PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                             Regolamento_Cod, TipoZona,
                                                             "", "",
                                                             GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
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

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CalcoloNPK_grfi_cod(ByVal objP_super_server As String, objP_server As String, Regolamento As Integer, Veg_Cod As Integer, Grfi_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            If Regolamento > 0 AndAlso Veg_Cod > 0 Then

                Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

                If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                    GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                End If

                Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
                Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
                objParametriIngresso.Regolamento_Cod = Regolamento
                objParametriIngresso.Veg_Cod = Veg_Cod
                objParametriIngresso.Grfi_Cod = Grfi_Cod
                objParametriIngresso.SoloValorizzati = True
                objParametriIngresso.SoloVisibili = False
                objParametriIngresso.url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione


                Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                objParametriUscita = objPC_WS.FattoriCorrettivi_Leggi(objParametriIngresso)
                If objParametriUscita.ListaFattoriCorrettivi.Count > 0 Then

                    '                    Cicli la lista ricevuta e ricavi 
                    'enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard
                    '                    enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard
                    '                    enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard


                    'Dim N = objParametriUscita.ListaFattoriCorrettivi.Item(enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard).Valore
                    'Dim P = objParametriUscita.ListaFattoriCorrettivi.Item(enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard).Valore
                    'Dim K = objParametriUscita.ListaFattoriCorrettivi.Item(enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard).Valore
                    Dim FattoreN As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    FattoreN = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard And x.Visibile = 1)(0)
                    If Not FattoreN Is Nothing Then
                        'ResaBassaDes.Value = FattoreResaBassa.Descrizione & " " & FattoreResaBassa.Valore
                    End If

                    Dim FattoreP As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    FattoreP = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard And x.Visibile = 1)(0)
                    If Not FattoreP Is Nothing Then
                        'ResaBassaDes.Value = FattoreResaBassa.Descrizione & " " & FattoreResaBassa.Valore
                    End If

                    Dim FattoreK As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    FattoreK = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard And x.Visibile = 1)(0)
                    If Not FattoreK Is Nothing Then
                        'ResaBassaDes.Value = FattoreResaBassa.Descrizione & " " & FattoreResaBassa.Valore
                    End If

                    Dim result = JsonConvert.SerializeObject(New JObject(New JProperty("N", FattoreN.Valore), New JProperty("P", FattoreP.Valore), New JProperty("K", FattoreK.Valore)), Formatting.None)
                    r.RispostaStringa = result
                End If

            End If

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CalcoloNPK_GrfiCod_StatoCod(ByVal objP_super_server As String, objP_server As String, Regolamento As Integer, Veg_Cod As Integer, Grfi_Cod As Integer, Stato_Cod As Integer) As RispostaStandard
        Dim r As New RispostaStandard

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            If Regolamento > 0 AndAlso Veg_Cod > 0 Then

                Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

                If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                    GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                End If

                Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
                Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
                objParametriIngresso.Regolamento_Cod = Regolamento
                objParametriIngresso.Veg_Cod = Veg_Cod
                objParametriIngresso.Grfi_Cod = Grfi_Cod
                objParametriIngresso.SoloValorizzati = True
                objParametriIngresso.SoloVisibili = False
                objParametriIngresso.url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione


                Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                objParametriUscita = objPC_WS.FattoriCorrettivi_Leggi(objParametriIngresso)

                If objParametriUscita.ListaFattoriCorrettivi.Count > 0 Then

                    Dim FattoreN As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Dim FattoreP As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Dim FattoreK As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo

                    Select Case Stato_Cod

                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
                    'rimangono = 0

                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento

                            FattoreN = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_I_anno_allevamento)(0)
                            FattoreP = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_I_anno_allevamento)(0)
                            FattoreK = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_I_anno_allevamento)(0)

                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento

                            FattoreN = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_II_anno_allevamento)(0)
                            FattoreP = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_II_anno_allevamento)(0)
                            FattoreK = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_II_anno_allevamento)(0)

                        Case Else

                            FattoreN = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard And x.Visibile = 1)(0)
                            FattoreP = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard And x.Visibile = 1)(0)
                            FattoreK = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard And x.Visibile = 1)(0)

                    End Select

                    Dim valoreN = 0
                    Dim valoreP = 0
                    Dim valoreK = 0

                    If FattoreN IsNot Nothing Then
                        valoreN = FattoreN.Valore
                    End If
                    If FattoreP IsNot Nothing Then
                        valoreP = FattoreP.Valore
                    End If
                    If FattoreK IsNot Nothing Then
                        valoreK = FattoreK.Valore
                    End If

                    Dim result = JsonConvert.SerializeObject(New JObject(New JProperty("N", valoreN), New JProperty("P", valoreP), New JProperty("K", valoreK)), Formatting.None)
                    r.RispostaStringa = result

                End If

            End If

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PUAFabbisognoSingolo(ByVal objP_super_server As String,
                                         ByVal objP_server As String,
                                         ByVal Pua_Tipo As Integer,
                                         ByVal Regolamento_Cod As Integer,
                                         ByVal PuaAppezzamento As String
                                         ) As RispostaStandard

        Dim r As New RispostaStandard
        Dim fabbisognoN As Decimal = 0

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If

            'Deserializzazione dati
            Dim objPD As PUA_Appezzamento = JsonConvert.DeserializeObject(PuaAppezzamento, GetType(PUA_Appezzamento))

            Dim listPuaAppezzamenti As New List(Of PUA_Appezzamento) From {objPD}

            Dim objFabb As New AgronicaCorePianoConcimazioneBIZ.PUA_Fabbisogni
            Dim Fabb_Input As New PUA_Fabbisogni_input With {
                .Regolamento_Cod = Regolamento_Cod,
                .PUA_Tipo = Pua_Tipo,
                .PUA_ListaAppezzamenti = listPuaAppezzamenti,
                .Url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione
            }

            Dim Fabb_out As New PUA_Fabbisogni_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            Fabb_out = objPC_WS.PUAFabbisogni(Fabb_Input)

            fabbisognoN = Fabb_out.N_Fabbisogno


            Dim Jobj As New JObject(New JProperty("Fabbisogno_N", fabbisognoN),
                                    New JProperty("Fabbisogno_N_Complessivo", CDec(fabbisognoN * objPD.Superficie)))

            r.RispostaStringa = JsonConvert.SerializeObject(Jobj, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function



    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_Finalita_Rer_WS(ByVal objP_super_server As String,
                                               ByVal objP_server As String,
                                               ByVal PrimaRiga_Flag As Boolean,
                                               ByVal PrimaRiga_Text As String,
                                               ByVal PrimaRiga_Value As String,
                                               ByVal Regolamento_Cod As Integer,
                                               ByVal Veg_Cod As Integer,
                                               ByVal Grfi_Cod As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String
                                               ) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.Finalita_Rer_WS(ddl,
                                                            PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                            Regolamento_Cod, Veg_Cod, Grfi_Cod, "",
                                                            "", "",
                                                            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
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


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_Finalita_Rer_BPerc_WS(ByVal objP_super_server As String,
                                               ByVal objP_server As String,
                                               ByVal PrimaRiga_Flag As Boolean,
                                               ByVal PrimaRiga_Text As String,
                                               ByVal PrimaRiga_Value As String,
                                               ByVal Regolamento_Cod As Integer,
                                               ByVal Veg_Cod As Integer,
                                               ByVal Grfi_Cod As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String
                                               ) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.Finalita_Rer_BPerc_WS(ddl,
                                                            PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                            Regolamento_Cod, Veg_Cod, Grfi_Cod, "",
                                                            "", "",
                                                            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
                                              New JProperty("Codice", CInt((i.Value.Split("|")(0)))),
                                              New JProperty("BPerc", CDec((i.Value.Split("|")(1))))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_Finalita_Rer_BPercNMasResa_WS(ByVal objP_super_server As String,
                                                       ByVal objP_server As String,
                                                       ByVal PrimaRiga_Flag As Boolean,
                                                       ByVal PrimaRiga_Text As String,
                                                       ByVal PrimaRiga_Value As String,
                                                       ByVal Regolamento_Cod As Integer,
                                                       ByVal Veg_Cod As Integer,
                                                       ByVal Grfi_Cod As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByVal xOrderBy As String
                                                       ) As RispostaStandard
        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.Finalita_Rer_BPercNMasResa_WS(ddl,
                                                            PrimaRiga_Flag, PrimaRiga_Text, PrimaRiga_Value,
                                                            Regolamento_Cod, Veg_Cod, Grfi_Cod,
                                                            "", "",
                                                            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New JArray()
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New JObject(New JProperty("Descrizione", i.Text),
                                              New JProperty("Codice", CInt((i.Value.Split("|")(0)))),
                                              New JProperty("BPerc", CDec((i.Value.Split("|")(1)))),
                                              New JProperty("N", CDec((i.Value.Split("|")(2)))),
                                              New JProperty("Resa", CDec((i.Value.Split("|")(3)))),
                                              New JProperty("FattoreCorrettivoN", CDec((i.Value.Split("|")(4))))))


            Next

            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaOp, Formatting.None)
            r.RispostaOK = True


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Pua_Regolamenti(ByVal objP_super_server As String,
                                    ByVal objP_server As String,
                                        ByVal Tipo As enum_PUARegolamenti_Tipo,
                                        ByVal TipoMetodo As enum_PUA_Tipo,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByVal xOrderBy As String
                                        ) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            If objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_input
            objParametriIngresso.strFiltro = xFiltroAggiuntivo
            objParametriIngresso.Tipo = Tipo
            objParametriIngresso.TipoMetodo = TipoMetodo
            objParametriIngresso.strOrdinamento = xOrderBy
            objParametriIngresso.url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione

            Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Regolamenti_output
            Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
            objParametriUscita = objPC_WS.Regolamenti(objParametriIngresso)

            Dim jArrayListaR As New JArray()


            For Each i As Regolamento In objParametriUscita.ListaRegolamenti

                Dim jArrayListaP As New JArray()

                For Each j As Regolamento_Parametro In i.ListaParametri

                    Dim JobjP As New JObject(New JProperty("Descrizione", j.Descrizione),
                                             New JProperty("Codice", j.Codice),
                                             New JProperty("Valore", j.Valore))

                    jArrayListaP.Add(JobjP)

                Next


                Dim JobjR As New JObject(New JProperty("Descrizione", i.Descrizione),
                                        New JProperty("Codice", i.Codice),
                                        New JProperty("ListaParametri", jArrayListaP))


                jArrayListaR.Add(JobjR)

            Next


            r.RispostaStringa = JsonConvert.SerializeObject(jArrayListaR, Formatting.None)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function


    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function PC_Finalita_Rer_WS_Modello(InData As CoreWS_Generic(Of FiltroPC_Finalita_Rer)
                                               ) As rispostaStandard(Of List(Of FinalitaPianoConcimazione))
        Dim r As New rispostaStandard(Of List(Of FinalitaPianoConcimazione))

        Try

            If InData.objP.objP_super_server = "" Then
                r.Errore = "objP_super_server non valorizzato"
                Return r
            End If

            If InData.objP.objP_server = "" Then
                r.Errore = "objP_server non valorizzato"
                Return r
            End If

            Dim objParametriSuperServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)
            Dim objParametriServer As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriServer)

            If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = objConfSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametriSuperServer)
            End If


            Dim ddl As New DropDownList
            ListControl_PianoConcimazione_WS.Finalita_Rer_WS(ddl,
                                                            False, "", 0,
                                                            InData.InData.regolamento.codice,
                                                            InData.InData.specie.codice,
                                                            InData.InData.finalita.codice, "",
                                                            "", "",
                                                            GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione)

            Dim jArrayListaOp As New List(Of FinalitaPianoConcimazione)
            For Each i As ListItem In ddl.Items
                jArrayListaOp.Add(New FinalitaPianoConcimazione(i.Value) With {.descrizione = i.Text})
            Next

            r.RispostaStringa = jArrayListaOp
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function CalcoloNPK_Modello(InData As CoreWS_Generic(Of FiltroCalcoloNPK)) As rispostaStandard(Of ApportoMacroelementi)
        Dim r As New rispostaStandard(Of ApportoMacroelementi)

        If InData.objP.objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If


        Try

            Dim objParametri_SuperServer As AgronicaCoreDataProvider.AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_super_server)

            Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(InData.objP.objP_server)

            If InData.InData.regolamento.codice > 0 AndAlso InData.InData.specie.codice > 0 Then

                Dim xLeggiConfigurazioneSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

                Dim GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione As String = ""

                GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_SuperServer)

                If GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = "" Then
                    GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione = xLeggiConfigurazioneSiti.Leggi_Valore(0, "GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione", "", "", objParametri_Server)
                End If

                Dim objParametriUscita As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
                Dim objParametriIngresso As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
                objParametriIngresso.Regolamento_Cod = InData.InData.regolamento.codice
                objParametriIngresso.Veg_Cod = InData.InData.specie.codice
                objParametriIngresso.Grfi_Cod = InData.InData.finalita.codice
                objParametriIngresso.SoloValorizzati = True
                objParametriIngresso.SoloVisibili = False
                objParametriIngresso.url = GiasOnline_WS_PianoConcimazione_AgroAPI_PianoConcimazione


                Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                objParametriUscita = objPC_WS.FattoriCorrettivi_Leggi(objParametriIngresso)

                If objParametriUscita.ListaFattoriCorrettivi.Count > 0 Then

                    Dim FattoreN As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Dim FattoreP As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo
                    Dim FattoreK As New AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo

                    Select Case InData.InData.stato.codice

                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
                    'rimangono = 0

                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento

                            FattoreN = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_I_anno_allevamento)(0)
                            FattoreP = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_I_anno_allevamento)(0)
                            FattoreK = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_I_anno_allevamento)(0)

                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento

                            FattoreN = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_II_anno_allevamento)(0)
                            FattoreP = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_II_anno_allevamento)(0)
                            FattoreK = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_II_anno_allevamento)(0)

                        Case Else

                            FattoreN = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard And x.Visibile = 1)(0)
                            FattoreP = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard And x.Visibile = 1)(0)
                            FattoreK = objParametriUscita.ListaFattoriCorrettivi.Where(Function(x) x.Codice = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard And x.Visibile = 1)(0)

                    End Select


                    'Dim result = JsonConvert.SerializeObject(New JObject(New JProperty("N", FattoreN.Valore), New JProperty("P", FattoreP.Valore), New JProperty("K", FattoreK.Valore)), Formatting.None)
                    Dim result = New ApportoMacroelementi()

                    result.n = 0
                    result.p2o5 = 0
                    result.k2o = 0

                    If (FattoreN IsNot Nothing) Then
                        result.n = FattoreN.Valore
                    End If

                    If FattoreP IsNot Nothing Then
                        result.p2o5 = FattoreP.Valore
                    End If

                    If FattoreK IsNot Nothing Then
                        result.k2o = FattoreK.Valore
                    End If

                    r.RispostaStringa = result

                End If

            End If

            r.RispostaOK = True

        Catch ex As Exception
            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

End Class