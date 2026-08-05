
Imports System.Drawing
Imports System.Data
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports System.Configuration.ConfigurationManager
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreSementieriBIZ
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreSementieriDAL
Imports AgronicaCoreVarieBIZ
Imports AgronicaControlli_2010
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreModello.ParametriAgenda_Temp

Public Class InterferenzeNotifiche
    Public Interferenze As String
    Public Notifiche As String
End Class

Partial Class GST_Menu
    Inherits System.Web.UI.Page

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GestioneSportello() As String

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If objParametri_Utenti.UtenteUsername <> objParametri_Server.SuperUserUsername Then
            Return ""
        End If

        Return "../GST_Sportello/GST_Sportello.aspx"
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function NuovoImpianto(ByVal Sportello As String, ByVal PrevCons As String, ByVal SportelloDdlValue As String) As String

        Dim specie_sementi As String = Sportello.Split("|")(0)
        Dim targetRedirect As String = "..\..\GIS\GIS.aspx"

        If specie_sementi < 0 Then

            HttpContext.Current.Session.Remove("Sementi")
            HttpContext.Current.Session.Remove("DatiPassaggio")
            HttpContext.Current.Session("SementiMappaturaLibera") = "1"

        Else

            HttpContext.Current.Session("Sementi") = Sportello
            HttpContext.Current.Session("DatiPassaggio") = PrevCons
            HttpContext.Current.Session.Remove("SementiMappaturaLibera")

        End If

        HttpContext.Current.Session.Remove("AggiornaLayer_FiltroAggiuntivo")

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")

        Dim apicontroller As New CoreApiControllerFactory
        apicontroller.Inizializza(objParametri_Super_Server, objParametri_Server)

        If apicontroller.CanUseAPI AndAlso apicontroller.MenuGisNG Then

            Dim params As New Dictionary(Of String, String)
            params.Add("Sementi", SportelloDdlValue)
            params.Add("DatiPassaggio", PrevCons)
            params.Add("SementiMappaturaLibera", (specie_sementi < 0))

            Dim parametri_Aggiuntivi As New JObject
            parametri_Aggiuntivi("QSF") = Newtonsoft.Json.JsonConvert.SerializeObject(params)

            MenuBS_2017_RedirectGestione.RedirectGenerico(objParametri_Utenti.PivaSuperUser,
                                                              Enum_SiteRedirector.GiasNG,
                                                              enum_PagineGiasNG.Pagina_GIS,
                                                              targetRedirect,
                                                              objParametri_Server,
                                                              parametri_Aggiuntivi)
        End If

        Return targetRedirect

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GisDaInterferenza(ByVal EntitaCod As String, ByVal Sportello As String, ByVal PrevCons As String) As String

        Dim arrSportello = Sportello.Split("|")
        arrSportello(6) = String.Format(New Date(1900, 1, 1))
        arrSportello(7) = String.Format(New Date(2100, 12, 31))
        Sportello = String.Join("|", arrSportello)

        HttpContext.Current.Session("Sementi") = Sportello
        HttpContext.Current.Session("DatiPassaggio") = PrevCons
        HttpContext.Current.Session.Remove("SementiMappaturaLibera")

        Dim filtroEntita = "Entita.Entita_Cod IN (" & String.Join(",", EntitaCod.Split("|")) & ")"
        HttpContext.Current.Session("AggiornaLayer_FiltroAggiuntivo") = filtroEntita

        Return "../../GIS/GIS.aspx?Entita=" + EntitaCod + "&ReadOnly=1"

    End Function



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function GisDaCoordinate(ByVal Lat As String, ByVal Lng As String, ByVal Sportello As String, ByVal PrevCons As String) As String
        Dim arrSportello = Sportello.Split("|")
        arrSportello(6) = String.Format(New Date(1900, 1, 1))
        arrSportello(7) = String.Format(New Date(2100, 12, 31))
        Sportello = String.Join("|", arrSportello)

        HttpContext.Current.Session("Sementi") = Sportello
        HttpContext.Current.Session("DatiPassaggio") = PrevCons
        HttpContext.Current.Session.Remove("SementiMappaturaLibera")

        Dim filtroEntita = "Entita.Entita_Cod IN (-1)"
        HttpContext.Current.Session("AggiornaLayer_FiltroAggiuntivo") = filtroEntita

        Return "../../GIS/GIS.aspx?Lat=" + Lat + "&Lng=" + Lng + "&ReadOnly=1"

    End Function



    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaModificheInterferenza(ByVal Lista_InterferenzaCod As String, ByVal Valore As String) As String

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim stato As Integer = If(Valore.Trim.ToLower = "si", 2, 3)
        Dim objInter As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_W
        Dim arrInterfCod As Integer() = Array.ConvertAll(Lista_InterferenzaCod.Split("@").ToArray(), Function(str) Integer.Parse(str))
        For Each cod In arrInterfCod
            objInter.Modifica(cod, stato, objParametri_Server)
        Next

        Return "true"
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function AccettaRifiutaInterferenza(Interferenza_Cod As Integer, FlagAccetta As Boolean) As String

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim stato As Integer = If(FlagAccetta, 2, 3)

        Dim objInter As New AgronicaCoreSementieriDAL.Sementieri_Sportello_InterferenzePerConferma_W

        objInter.Modifica(Interferenza_Cod, stato, objParametri_Server)

        Return "true"
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaElenco(ByVal Sportello As String) As String

        If String.IsNullOrEmpty(Sportello) Then
            Return ""
        End If

        Dim id_Sportello As Integer = Sportello.Split("|")(4)

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim objR As New AgronicaCoreSementieriDAL.Sementieri_Sportello_LOG_R
        Dim dt As DataTable = objR.Leggi(id_Sportello, 0, objParametri_Server, objParametri_Utenti)

        Dim variazioni As New DataTable("Variazioni")
        variazioni.Columns.Add("Data")
        variazioni.Columns.Add("Referente")
        variazioni.Columns.Add("Operazione")
        variazioni.Columns.Add("Indirizzo")
        variazioni.Columns.Add("CAP")
        variazioni.Columns.Add("Comune")
        variazioni.Columns.Add("Prov")
        variazioni.Columns.Add("Lat")
        variazioni.Columns.Add("Lng")
        variazioni.Columns.Add("Specie")
        variazioni.Columns.Add("Tipologia")
        variazioni.Columns.Add("Superficie")

        Dim riga As DataRow

        Dim utente As String
        Dim operazione As String
        Dim campo As String
        Dim lat As Double
        Dim lng As Double
        Dim strLat As String
        Dim strLng As String
        Dim sup As Double
        Dim strSup As String

        For Each row In dt.Rows

            If row("Flag_Attivo") = 0 Then
                riga = objR.LeggiDaCache(row("Descrizione_Casella"))
            Else
                riga = row
            End If

            utente = If(riga.Table.Columns.Contains("LayerDesc"), riga("LayerDesc"), riga("Rag_SocSementiero"))
            operazione = "Operazione sconosciuta"
            Select Case CInt(riga("TipoOperazione_DB"))
                Case 1
                    operazione = "Inserito nuovo appezzamento"
                Case 2
                    operazione = "Modificato un appezzamento esistente"
                Case 3
                    operazione = "Eliminato un appezzamento esistente"
            End Select
            campo = ""
            If riga.Table.Columns.Contains("Via_Stringa") AndAlso Not IsDBNull(riga("Via_Stringa")) Then
                campo = riga("Via_Stringa")
            End If
            If String.IsNullOrEmpty(campo) Then
                campo = riga("Ind_Des") & " " & riga("frz_des") & " (" & riga("com_des") & " - " & riga("pro_des") & ")"
            End If
            lat = If(riga.Table.Columns.Contains("True_Lat"), riga("True_Lat"), 0)
            lng = If(riga.Table.Columns.Contains("True_Lng"), riga("True_Lng"), 0)
            If lat = 0 AndAlso Not IsDBNull(riga("lat")) AndAlso IsNumeric(riga("lat")) Then
                lat = riga("lat")
            End If
            If lng = 0 AndAlso Not IsDBNull(riga("long")) AndAlso IsNumeric(riga("long")) Then
                lng = riga("long")
            End If
            strLat = If(lat <> 0, Agro_Math.ArrotondaVal_6(lat).ToString("N6"), "")
            strLng = If(lng <> 0, Agro_Math.ArrotondaVal_6(lng).ToString("N6"), "")

            sup = If(riga.Table.Columns.Contains("Superficie"), riga("Superficie"), 0)
            strSup = If(sup > 0, Agro_Math.ArrotondaVal_4(sup).ToString("0.0000"), "")

            Dim dr_var As DataRow = variazioni.NewRow

            dr_var.Item("Data") = CDate(riga("Data_log")).ToString("g")
            dr_var.Item("Referente") = utente
            dr_var.Item("Operazione") = operazione
            dr_var.Item("Indirizzo") = campo
            dr_var.Item("CAP") = riga("CAP")
            dr_var.Item("Comune") = riga("Com_DES")
            dr_var.Item("Prov") = riga("Pro_DES")
            dr_var.Item("Lat") = strLat
            dr_var.Item("Lng") = strLng
            dr_var.Item("Specie") = riga("Veg_Des")
            dr_var.Item("Tipologia") = riga("grva_des")
            dr_var.Item("Superficie") = strSup

            variazioni.Rows.Add(dr_var)

        Next

        If variazioni.Rows.Count = 0 Then
            Return ""
        End If

        HttpContext.Current.Session("LogDT_Variazioni") = variazioni

        Return "LOG_Variazioni.aspx"

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaPermessiEstrazione() As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        Try

            Dim permessiWMS As Boolean = ObjUtenti.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Interferenze_ScaricoDati_Consolida,
            enum_Security_Operazione.Scrittura,
            Date.Now,
            "",
            objParametri_Utenti)

            r.RispostaOK = True
            r.RispostaStringa = permessiWMS.ToString

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaVariazioni(ByVal Sportello As String, ByVal ElencoVariazioni As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permessiSalvataggio As Boolean

        Try

            permessiSalvataggio = ObjUtenti.Controlla_Permessi_Utente(
            objParametri_Utenti.UtenteUsername,
            enum_Id_Servizio.GiasOnline,
            enum_Security_Attivita.Interferenze_ScaricoDati_Consolida,
            enum_Security_Operazione.Lettura,
            Date.Now,
            "",
            objParametri_Utenti)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
            Return r
        End Try

        If Not permessiSalvataggio Then
            r.RispostaOK = False
            r.Errore = "L'utente non ha i permessi per effettuare l'operazione richiesta"
            Return r
        End If

        If String.IsNullOrWhiteSpace(Sportello) Then
            r.RispostaOK = False
            r.Errore = "Selezionare uno sportello"
            Return r
        End If

        Dim Cod_sportello As String = Sportello.Split("|")(4)
        Dim Sportello_int As Int32

        If Not Int32.TryParse(Cod_sportello, Sportello_int) Then
            r.RispostaOK = False
            r.Errore = "Codice sportello non valido"
            Return r
        End If

        Dim FlagConnessioneLocale As Boolean
        Dim FlagTransazioneLocale As Boolean

        Try

            AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessioneXCoreBiz(FlagConnessioneLocale,
                                                                                    FlagTransazioneLocale,
                                                                                    objParametri_Server)

            If String.IsNullOrWhiteSpace(ElencoVariazioni) Then
                Dim messaggi = MostraMessaggi(Sportello)
                ElencoVariazioni = messaggi.Notifiche
            End If

            Dim xWrite As New AgronicaCoreSementieriDAL.Sementieri_EstrazioneVariazioni_W

            xWrite.Elimina(Sportello_int, 0, objParametri_Server)

            For Each obj In Newtonsoft.Json.JsonConvert.DeserializeObject(ElencoVariazioni)
                Dim motivo_variazione As String = ""

                Select Case obj("tipo_operazione").ToString
                    Case "1"
                        motivo_variazione = "Nuovo appezzamento"
                    Case "2"
                        motivo_variazione = "Modificato appezzamento"
                    Case "3"
                        motivo_variazione = "Eliminato appezzamento"
                End Select

                Dim data_variazione = Convert.ToDateTime(obj("data"))

                xWrite.Scrivi(Sportello_int,
                          obj("regione").ToString,
                          obj("dittaSementiera").ToString,
                          obj("entita_cod").ToString,
                          obj("specie").ToString,
                          obj("nomeScientifico").ToString,
                          obj("tipologia").ToString,
                          obj("indirizzo_centro")("prov").ToString,
                          obj("indirizzo_centro")("com").ToString,
                          obj("aziendaAgricola").ToString,
                          obj("indirizzo_centro")("via").ToString,
                          Decimal.Parse(obj("coord")("lat")),
                          Decimal.Parse(obj("coord")("lng")),
                          Decimal.Parse(obj("superficie")),
                          motivo_variazione,
                          data_variazione,
                          0,
                          CostantiPersonalizzate.AGRODATAINIZIO,
                          objParametri_Server)
            Next

            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazioneXCoreBiz(FlagTransazioneLocale, objParametri_Server)

            r.RispostaOK = True
            r.RispostaStringa = "Operazione completata con successo."

        Catch ex As Exception

            If Not objParametri_Server.objTransazione Is Nothing Then
                AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiTransazione(2, objParametri_Server)
            End If

            r.RispostaOK = False
            r.Errore = ex.Message

        Finally
            AgronicaCoreDataProvider.ConnessioniTransazioni.ChiudiConnessioneXCoreBiz(FlagConnessioneLocale, objParametri_Server)
        End Try

        Return r
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function VerificaInterferenze(ByVal Sportello As String, ByVal PrevCons As String) As String

        If String.IsNullOrEmpty(Sportello) Then
            Return ""
        End If

        Dim specie_sementi As String = Sportello.Split("|")(0)
        If specie_sementi < 0 Then
            Return ""
        End If

        HttpContext.Current.Session("Sementi") = Sportello
        HttpContext.Current.Session("DatiPassaggio") = PrevCons

        Return "../GST_Filtro/GST_Filtro_Impianti.aspx"
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ElencoSportelli(ByVal SportelliPrecedenti As String) As String

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim dataRiferimento As DateTime = Now.Date
        If SportelliPrecedenti = "1" Then
            dataRiferimento = CostantiPersonalizzate.AGRODATAFINE
        End If

        Dim leggiSementi As New AgronicaCoreSementieriDAL.Sportello_R
        Dim dt As DataTable = leggiSementi.Leggi(0, 0, dataRiferimento, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", objParametri_Server)

        Dim elenco As New JArray

        dt.DefaultView.Sort = "Sementieri_Sportello_Configurazione_cod DESC"
        dt = dt.DefaultView.ToTable

        Dim descr As String
        Dim val As String

        For Each drow As DataRow In dt.Rows

            If Not dataRiferimento = CostantiPersonalizzate.AGRODATAFINE Then
                descr = drow("Sementieri_Sportello_Configurazione_des")
            Else
                descr = drow("Sementieri_Sportello_Configurazione_des") & " - " & drow("Sementieri_Sportello_Passaggi_des")
            End If

            val = drow("id_specie") & "|0|0|0|" & drow("Sementieri_Sportello_Configurazione_cod").ToString & "|" & drow("Sementieri_Sportello_Configurazione_des").ToString & "|" & drow("Validita_Inizio").ToString & "|" & drow("Validita_Fine").ToString

            elenco.Add(New JObject(New JProperty("descr", descr), New JProperty("val", val)))
        Next

        Return elenco.ToString
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function InfoSportello(ByVal Sportello As String) As String

        If String.IsNullOrEmpty(Sportello) Then
            Return ""
        End If

        Dim sportello_cod As Integer = Sportello.Split("|")(4)

        If sportello_cod < 0 Then
            Return ""
        End If

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim obj_R As New AgronicaCoreSementieriDAL.Sportello_R
        Dim Dt As DataTable = obj_R.Leggi_SportelloxPassaggi(sportello_cod, 0, 0, "ORDER BY Data_Inizio", objParametri_Server)

        Dim adesso As DateTime = DateTime.Now()
        Dim DataInizio, DataFine As DateTime

        Dim elencoFasi As New JArray
        For Each row In Dt.Rows

            Dim fase As New JObject

            DataInizio = CDate(row("Data_Inizio"))
            DataInizio = New Date(DataInizio.Year, DataInizio.Month, DataInizio.Day, 0, 0, 0)
            DataFine = CDate(row("Data_Fine"))
            DataFine = New Date(DataFine.Year, DataFine.Month, DataFine.Day, 23, 59, 59)

            fase("attiva") = DataInizio <= adesso AndAlso adesso <= DataFine
            fase("fase") = row("Sementieri_Sportello_Passaggi_des").ToString
            fase("dataInizio") = DataInizio
            fase("dataFine") = DataFine
            fase("visImpianti") = If(CInt(row("Visibilita_Impianti")) = 1, "Tutti", "Solo interni")
            fase("operPermesse") = If(CInt(row("DestinazioneSalvataggio")) < 0, "No", "Si")
            fase("logOperazioni") = If(CInt(row("LoggaOperazioni")) = 1, "Si", "No")
            fase("notificaInterferenze") = If(CInt(row("RichiediConfermaSuInterferenze")) = 1, "Si", "No")

            elencoFasi.Add(fase)
        Next

        Return elencoFasi.ToString()
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function ImpostaPreventivoConsuntivo(ByVal Sportello As String) As String

        If String.IsNullOrEmpty(Sportello) Then
            Return ""
        End If

        Dim sportello_cod As Integer = Sportello.Split("|")(4)

        If sportello_cod < 0 Then
            Return ""
        End If

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim dataRiferimento As DateTime = Now.Date
        Dim leggiPassaggio As New AgronicaCoreSementieriDAL.Sportello_R
        Dim dt As DataTable = leggiPassaggio.Leggi(sportello_cod, 0, dataRiferimento, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", objParametri_Server)

        Dim drows As DataRow() = dt.Select(" Sementieri_Sportello_Configurazione_cod = " & sportello_cod)

        Dim objResult As New JObject
        objResult("text") = ""
        objResult("data") = ""
        objResult("info") = ""

        Dim data, info As String
        If drows.Count > 0 Then

            Dim sp = drows(0)

            objResult("text") = sp("Sementieri_Sportello_Passaggi_des").ToString

            data = sp("Sementieri_Sportello_Passaggi_cod") & "|"
            data &= sp("data_inizio") & "|"
            data &= sp("data_fine") & "|"
            data &= sp("visibilita_impianti") & "|"
            data &= sp("DestinazioneSalvataggio") & "|"
            data &= sp("RichiediConfermaSuInterferenze") & "|"
            data &= sp("LoggaOperazioni") & "|"
            data &= sp("VisualizzaOperazioniLoggate")

            objResult("data") = data

            info = "<i>Visibilità impianti</i> <b>"
            If CInt(sp("visibilita_impianti")) = 1 Then
                info &= "TUTTI"
            Else
                info &= "SOLO INTERNI"
            End If
            info &= "</b> - <i>Comunica operazioni</i> <b>"
            If CInt(sp("LoggaOperazioni")) = 1 Then
                info &= "SI"
            Else
                info &= "NO"
            End If
            info &= "</b> - <i>Notifica interferenze</i> <b>"
            If CInt(sp("RichiediConfermaSuInterferenze")) = 1 Then
                info &= "SI"
            Else
                info &= "NO"
            End If
            info &= "</b>"

            objResult("info") = info

        End If

        Return objResult.ToString
    End Function


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function MostraMessaggi(ByVal Sportello As String) As InterferenzeNotifiche

        Dim i_n As New InterferenzeNotifiche With {.Interferenze = "", .Notifiche = ""}

        If String.IsNullOrEmpty(Sportello) Then
            Return i_n
        End If

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim CodiceFiscaleTecnico As String = HttpContext.Current.Session("Codice_Fiscale_Tecnico")
        CodiceFiscaleTecnico = HttpContext.Current.Session("Codice_Fiscale_Tecnico_OK")
        If objParametri_Server.PivaSuperUser = objParametri_Server.UtenteCodFiscale Then
            'è Superuser...
            CodiceFiscaleTecnico = ""
        End If

        Dim arrSportello = Sportello.Split("|")
        'controllo le specie vegetali
        Dim id_Specie As Integer = arrSportello(0)
        Dim DataInizioSportello As DateTime = arrSportello(6)
        Dim DataFineSportello As DateTime = arrSportello(7)

        Dim objInterferenze As New AgronicaCoreSementieriBIZ.Interferenze
        'Dim strInterferenze As String = objInterferenze.MostraInterferenzeGlobali(objParametri_Utenti.UtenteUsername, "", id_Specie, CodiceFiscaleTecnico, "", objParametri_Server, objParametri_Utenti, DataInizioSportello, DataFineSportello)
        Dim strInterferenze As String = objInterferenze.MostraInterferenzeGlobali_New(id_Specie, CodiceFiscaleTecnico, False, DataInizioSportello, DataFineSportello, objParametri_Server, objParametri_Utenti)

        i_n.Interferenze = strInterferenze



        Dim sportello_cod As Integer = arrSportello(4)

        Dim objR As New AgronicaCoreSementieriDAL.Sementieri_Sportello_LOG_R
        Dim dt As DataTable = objR.Leggi(sportello_cod, 0, objParametri_Server, objParametri_Utenti)

        Dim arrNotizie As New JArray

        For Each dr As DataRow In dt.Rows

            Dim riga As DataRow

            If dr("Flag_Attivo") = 0 Then
                riga = objR.LeggiDaCache(dr("Descrizione_Casella"))
            Else
                riga = dr
            End If

            Dim notizia As New _notizia(riga)

            Dim objNotizia = notizia.ToObject

            If objNotizia IsNot Nothing Then

                arrNotizie.Add(objNotizia)
            End If
        Next

        i_n.Notifiche = arrNotizie.ToString()

#If False Then
        
        Dim riga As DataRow
        Dim flagSep As Boolean = False
        Dim utente As String
        Dim operazione As String
        Dim campo As String
        Dim lat As Double
        Dim lng As Double
        Dim strLat As String
        Dim strLng As String
        Dim color As String

        Dim strCtrl As New StringBuilder

        strCtrl.Append("<div id='notizieDitta' style='text-align: left;font-family: Lucida Grande, Lucida Sans, Arial, sans-serif;font-size: 1.1em'>")

            For Each dtr As DataRow In dt.Rows

            If dtr("Flag_Attivo") = 0 Then
                riga = objR.LeggiDaCache(dtr("Descrizione_Casella"))
            Else
                riga = dtr
            End If

            If flagSep Then
                strCtrl.Append("<div style='margin-top: 10px; margin-bottom: 10px; border-top: 1px solid #ccc;'></div>")
            End If
            flagSep = True

            utente = If(riga.Table.Columns.Contains("LayerDesc"), riga("LayerDesc"), riga("Rag_SocSementiero"))
            operazione = "Operazione sconosciuta"
            Select Case CInt(riga("TipoOperazione_DB"))
                Case 1
                    operazione = "Inserito nuovo appezzamento"
                Case 2
                    operazione = "Modificato un appezzamento esistente"
                Case 3
                    operazione = "Eliminato un appezzamento esistente"
            End Select
            campo = ""
            If riga.Table.Columns.Contains("Via_Stringa") AndAlso Not IsDBNull(riga("Via_Stringa")) Then
                campo = "<b>" & riga("Via_Stringa") & "</b>"
            End If
            If String.IsNullOrEmpty(campo) Then
                campo = "<b>" & riga("Ind_Des") & "</b> " & riga("frz_des") & " (" & riga("com_des") & " - " & riga("pro_des") & ")"
            End If
            lat = If(riga.Table.Columns.Contains("True_Lat"), riga("True_Lat"), 0)
            lng = If(riga.Table.Columns.Contains("True_Lng"), riga("True_Lng"), 0)
            If lat = 0 AndAlso Not IsDBNull(riga("lat")) AndAlso IsNumeric(riga("lat")) Then
                lat = riga("lat")
            End If
            If lng = 0 AndAlso Not IsDBNull(riga("long")) AndAlso IsNumeric(riga("long")) Then
                lng = riga("long")
            End If
            strLat = If(lat <> 0, Agro_Math.ArrotondaVal_6(lat).ToString("N6"), "")
            strLng = If(lng <> 0, Agro_Math.ArrotondaVal_6(lng).ToString("N6"), "")

            color = If(riga("OperazioneDaEvidenziare") = 1, " color: red;", "")

            strCtrl.Append("<div style='padding-top:10px; padding-bottom: 10px;" & color & "'>")

            strCtrl.Append("<div style='display:flex; align-items:stretch; justify-content:space-between;'>")

            strCtrl.Append("<div><b>" & CDate(riga("Data_log")).ToString("g") & "</b> - " & utente & "</div>")
            strCtrl.Append("<div style='border:1px solid #ccc; border-radius:4px; margin-left:7px; background-color:#eee; cursor:pointer;' onclick='")
            If CInt(riga("TipoOperazione_DB")) <> 3 Then
                strCtrl.Append("vai_a_gis_da_log(""" & riga("Entita_Cod").ToString() & """)")
            Else
                strCtrl.Append("vai_a_gis_da_coord(""" & lat.ToString() & """, """ & lng.ToString() & """)")
            End If
            strCtrl.Append("")
            strCtrl.Append("'>")

            strCtrl.Append("<div style='font-weight:bolder; padding-left:10px;padding-right:10px; color:black;'>")
            'strCtrl.Append("<span>&#x21AA;</span>")
            strCtrl.Append("<span class='fa fa-globe'></span>")
            strCtrl.Append("</div>")
            strCtrl.Append("</div>")

            strCtrl.Append("</div>")

            strCtrl.Append("<div style='margin-top: 4px;'><i>" & operazione & "</i>: " & campo & "</div>")
            strCtrl.Append("<div style='margin-top: 4px;'><i>Lat</i>: <b>" & strLat & "</b> - <i>Long</i>: <b>" & strLng & "</b></div>")
            strCtrl.Append("<div style='margin-top: 4px;'><i>Specie</i>: " & riga("Veg_Des") & " - <i>Tipologia</i>: " & riga("grva_des") & "</div>")

            strCtrl.Append("</div>")
        Next

        strCtrl.Append("</div>")

        i_n.Notifiche = strCtrl.ToString

#End If

        Return i_n

    End Function

    Private Class _notizia
        Private ReadOnly _entita_cod As Integer
        Private ReadOnly _data As Date
        Private ReadOnly _utente As String
        Private ReadOnly _tipo_operazione As Integer
        Private ReadOnly _indirizzo As String
        Private ReadOnly _via As String
        Private ReadOnly _fraz As String
        Private ReadOnly _com As String
        Private ReadOnly _prov As String
        Private ReadOnly _lat As Decimal
        Private ReadOnly _lng As Decimal
        Private ReadOnly _specie As String
        Private ReadOnly _tipologia As String
        Private ReadOnly _nomeScientifico As String
        Private ReadOnly _regione As String
        Private ReadOnly _superficie As String
        Private ReadOnly _dittaSementiera As String
        Private ReadOnly _aziendaAgricola As String

        Public Sub New(dr As DataRow)
            _entita_cod = 0
            If dr.Table.Columns.Contains("Entita_Cod") AndAlso Not IsDBNull(dr("Entita_Cod")) AndAlso Not String.IsNullOrEmpty(dr("Entita_Cod")) Then

                _entita_cod = CInt(dr("Entita_Cod"))
            End If
            _data = CDate(dr("Data_log"))
            _utente = If(dr.Table.Columns.Contains("LayerDesc"), dr("LayerDesc"), dr("Rag_SocSementiero"))
            _dittaSementiera = If(dr.Table.Columns.Contains("LayerDesc") AndAlso Not String.IsNullOrEmpty(dr("LayerDesc")), dr("LayerDesc").ToString, "")
            _aziendaAgricola = If(dr.Table.Columns.Contains("Rag_SocSementiero") AndAlso Not String.IsNullOrEmpty(dr("Rag_SocSementiero")), dr("Rag_SocSementiero").ToString, "")
            _tipo_operazione = CInt(dr("TipoOperazione_DB"))
            _indirizzo = ""
            If dr.Table.Columns.Contains("Via_Stringa") AndAlso Not IsDBNull(dr("Via_Stringa")) Then
                _indirizzo = dr("Via_Stringa")
            End If
            _via = dr("Ind_Des").ToString
            _fraz = dr("frz_des").ToString
            _com = dr("com_des").ToString
            _prov = dr("pro_des").ToString
            _lat = If(dr.Table.Columns.Contains("True_Lat"), dr("True_Lat"), 0)
            _lng = If(dr.Table.Columns.Contains("True_Lng"), dr("True_Lng"), 0)
            If _lat = 0 AndAlso Not IsDBNull(dr("lat")) AndAlso IsNumeric(dr("lat")) Then
                _lat = dr("lat")
            End If
            If _lng = 0 AndAlso Not IsDBNull(dr("long")) AndAlso IsNumeric(dr("long")) Then
                _lng = dr("long")
            End If
            _specie = dr("Veg_Des").ToString
            _tipologia = dr("grva_des").ToString
            If dr.Table.Columns.Contains("NomeScientifico") Then
                _nomeScientifico = dr("NomeScientifico").ToString
            Else
                _nomeScientifico = _specie
            End If
            If dr.Table.Columns.Contains("Superficie") Then
                _superficie = dr("Superficie")
            Else
                _superficie = 0
            End If

        End Sub

        Public Function ToObject() As JObject

            If String.IsNullOrEmpty(_indirizzo) AndAlso String.IsNullOrEmpty(_via) AndAlso (_lat < 0.001 OrElse _lng < 0.001) Then

                Return Nothing
            End If

            Dim obj As New JObject(New JProperty("entita_cod", _entita_cod),
                                   New JProperty("data", _data.ToString("s")),
                                   New JProperty("utente", _utente),
                                   New JProperty("tipo_operazione", _tipo_operazione),
                                   New JProperty("indirizzo", _indirizzo))

            Dim indirizzo_centro As New JObject(New JProperty("via", _via),
                                                New JProperty("fraz", _fraz),
                                                New JProperty("com", _com),
                                                New JProperty("prov", _prov))

            obj.Add("indirizzo_centro", indirizzo_centro)
            obj.Add("coord", New JObject(New JProperty("lat", _lat), New JProperty("lng", _lng)))
            obj.Add("specie", _specie)
            obj.Add("tipologia", _tipologia)
            obj.Add("nomeScientifico", _nomeScientifico)
            obj.Add("regione", _regione)
            obj.Add("superficie", _superficie)
            obj.Add("aziendaAgricola", _aziendaAgricola)
            obj.Add("dittaSementiera", _dittaSementiera)

            Return obj
        End Function
    End Class


    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function TabellaDistanze() As String

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim jsonTable = VerificaInterferenza.TabelleDistanze(objParametri_Server)

        'Dim cols As String() = {
        '"Distanze [mt]",
        '"Fra due varietà di sottospecie diverse (entrambe OP)",
        '"Fra due varietà di sottospecie diverse (entrambe HY)",
        '"Fra due varietà di gruppi diversi della stessa sottospecie (entrambe OP)",
        '"Fra due varietà di gruppi diversi della stessa sottospecie (entrambe HY)",
        '"Fra due varietà diverse dello stesso gruppo (entrambe OP)",
        '"Fra due varietà diverse dello stesso gruppo (entrambe HY)",
        '"Fra una varietà OP ed una varietà HY"
        '}

        Dim resObj As New JObject

        resObj.Add("header", New JArray From {
                   New JObject(New JProperty("title", "Distanze [mt]"), New JProperty("field", "specie")),
                   New JObject(New JProperty("title", "Sottospecie diverse"), New JProperty("field", "sottospecie_diverse"),
                               New JProperty("columns",
                                             New JArray From {
                                             New JObject(New JProperty("title", "OP"), New JProperty("field", "op")),
                                             New JObject(New JProperty("title", "HY"), New JProperty("field", "hy"))
                                             })),
                   New JObject(New JProperty("title", "Stessa sottospecie gruppi diversi"), New JProperty("field", "gruppi_diversi"),
                               New JProperty("columns",
                                             New JArray From {
                                             New JObject(New JProperty("title", "OP"), New JProperty("field", "op")),
                                             New JObject(New JProperty("title", "HY"), New JProperty("field", "hy"))
                                             })),
                   New JObject(New JProperty("title", "Stesso gruppo"), New JProperty("field", "stesso_gruppo"),
                               New JProperty("columns",
                                             New JArray From {
                                             New JObject(New JProperty("title", "OP"), New JProperty("field", "op")),
                                             New JObject(New JProperty("title", "HY"), New JProperty("field", "hy"))
                                             })),
                   New JObject(New JProperty("title", "Varietà OP e varieta HY"), New JProperty("field", "diversi_genotipi"))
        })

        resObj.Add("rows", jsonTable)

        Return resObj.ToString
    End Function

    <Script.Services.ScriptMethod()>
    <WebMethod(EnableSession:=True)>
    Public Shared Function TabellaCodifica() As String

        Dim tabella(,) As String = {
        {"Bietola   ", "da zucchero ", "monogerme                       ", "Barbabietola da Zucchero", "Monogerme"},
        {"          ", "            ", "plurigerme                      ", "                        ", "Plurigerme"},
        {"          ", "da foraggio ", "monogerme a radice bianca       ", "Barbabietola da Foraggio", "Monogerme a Radice Bianca"},
        {"          ", "            ", "monogerme a radice gialla       ", "                        ", "Monogerme a Radice Gialla"},
        {"          ", "            ", "monogerme a radice rossa        ", "                        ", "Monogerme a Radice Rossa"},
        {"          ", "            ", "plurigerme a radice bianca      ", "                        ", "Plurigerme a Radice Bianca"},
        {"          ", "            ", "plurigerme a radice gialla      ", "                        ", "Plurigerme a Radice Gialla"},
        {"          ", "            ", "plurigerme a radice rossa       ", "                        ", "Plurigerme a Radice Rossa"},
        {"          ", "da orto     ", "a radice piatta rossa           ", "Bietola Rossa(da Orto)  ", "Radice Piatta Rossa"},
        {"          ", "            ", "a radice piatta rosata          ", "                        ", "Radice Piatta Rosata"},
        {"          ", "            ", "a radice tonda rossa            ", "                        ", "Radice Tonda Rossa"},
        {"          ", "            ", "a radice tonda rosata           ", "                        ", "Radice Tonda Rosata"},
        {"          ", "            ", "a radice lunga rossa            ", "                        ", "Radice Lunga Rossa"},
        {"          ", "da costa    ", "a costa bianca e foglia verde   ", "Bietola da Coste        ", "Costa bianca e foglia verde"},
        {"          ", "            ", "a costa bianca e foglia bionda  ", "                        ", "Costa bianca e foglia bionda"},
        {"          ", "            ", "a costa verde e foglia verde    ", "                        ", "Costa verde e foglia verde"},
        {"          ", "            ", "a costa rossa e foglia rossa    ", "                        ", "Costa rossa e foglia rossa"},
        {"Brassiche ", "            ", "Rapa                            ", "Rapa Primaverile e Autunnale", ""},
        {"          ", "            ", "Juncea                          ", "Senape                      ", ""},
        {"Carota    ", "            ", "Lunga                           ", "Carota                      ", "Lunga"},
        {"          ", "            ", "Mezza Lunga                     ", "                            ", "Mezza Lunga"},
        {"          ", "            ", "Rotonda                         ", "                            ", "Rotonda"},
        {"          ", "            ", "da Foraggio                     ", "                            ", "da Foraggio"},
        {"Cicoria   ", "            ", "a Foglie Colorate               ", "Cicoria                     ", "Foglie Colorate"},
        {"          ", "            ", "a Foglia Bionda                 ", "                            ", "Foglia Bionda"},
        {"          ", "            ", "a Foglia Verde                  ", "                            ", "Foglia Verde"},
        {"          ", "            ", "Tipo Catalogna                  ", "                            ", "Tipo Catalogna"},
        {"          ", "            ", "Tipo Selvatica                  ", "                            ", "Tipo Selvatica"},
        {"          ", "            ", "da Radice                       ", "                            ", "da Radice"},
        {"Ravanello ", "Maritimus   ", "                                ", "Ravanello                   ", "Maritimus"},
        {"          ", "Oleifera    ", "                                ", "                            ", "Oleifera"},
        {"          ", "Major       ", "Tondo Bianco                    ", "                            ", "Major Rotondo Bianco"},
        {"          ", "            ", "Tondo Nero                      ", "                            ", "Major Tondo Nero"},
        {"          ", "            ", "Mezzo Lungo Nero                ", "                            ", "Major Mezzo Lungo Nero"},
        {"          ", "            ", "Lungo Bianco                    ", "                            ", "Major Lungo Bianco"},
        {"          ", "            ", "Lungo Nero                      ", "                            ", "Major Lungo Nero"},
        {"          ", "            ", "Lungo Violetto                  ", "                            ", "Major Lungo Violetto"},
        {"          ", "Macrocarpus ", "da Radice Lungo Bianco          ", "                            ", "Macrocarpus da Radice Lungo Bianco"},
        {"          ", "            ", "da Radice Mezzo Lungo Bianco    ", "                            ", "Macrocarpus da Radice Mezzo Lungo Bianco"},
        {"          ", "            ", "da Radice Rotondo Bianco        ", "                            ", "Macrocarpus da Radice Rotondo Bianco"},
        {"          ", "            ", "da Germogli                     ", "                            ", "Macrocarpus da Germogli"},
        {"          ", "Radicula    ", "Rotondo Rosso                   ", "                            ", "Radicula Rotondo Rosso"},
        {"          ", "            ", "Rotondo Rosso G.P.B.            ", "                            ", "Radicula Rotondo Rosso G.P.B."},
        {"          ", "            ", "Rotondo Rosso P.P.B.            ", "                            ", "Radicula Rotondo Rosso P.P.B."},
        {"          ", "            ", "Ovale Bianco                    ", "                            ", "Radicula Ovale Bianco"},
        {"          ", "            ", "Ovale Rosso                     ", "                            ", "Radicula Ovale Rosso"},
        {"          ", "            ", "Mezzo Lungo Rosso G.P.B.        ", "                            ", "Radicula Mezzo Lungo Rosso G.P.B."},
        {"          ", "            ", "Mezzo Lungo Rosso P.P.B.        ", "                            ", "Radicula Mezzo Lungo Rosso P.P.B."},
        {"          ", "            ", "Lungo Rosso Punta Bianca        ", "                            ", "Radicula Lungo Rosso Punta Bianca"},
        {"          ", "            ", "Lungo Rosso                     ", "                            ", "Radicula Lungo Rosso"},
        {"          ", "            ", "Lungo Bianco                    ", "                            ", "Radicula Lungo Bianco"},
        {"Cavolo    ", "Acephala    ", "da Foraggio                     ", "Cavolo da Foraggio          ", ""},
        {"          ", "            ", "Laciniato                       ", "Cavolo Laciniato            ", ""},
        {"          ", "Botrytis    ", "Cavolfiore                      ", "Cavolfiore                  ", ""},
        {"          ", "            ", "Broccolo a Palla                ", "Cavolo Broccolo             ", "Broccolo a Palla"},
        {"          ", "            ", "Broccolo a Germoglio            ", "                            ", "Broccolo a Germoglio"},
        {"          ", "Bullata     ", "di Bruxelles                    ", "Cavolo di Bruxelles         ", ""},
        {"          ", "Capitata    ", "Cappuccio Bianco                ", "Cavolo Cappuccio Bianco     ", "Cappuccio a Palla"},
        {"          ", "            ", "Cappuccio Conico                ", "                            ", "Cappuccio Conico"},
        {"          ", "            ", "Cappuccio Rosso                 ", "Cavolo Cappuccio Rosso      ", "Cappuccio a Palla"},
        {"          ", "Gongyloides ", "Rapa Bianco                     ", "Cavolo Rapa                 ", "Gongyloides Rapa Bianco"},
        {"          ", "            ", "Rapa Blu                        ", "                            ", "Gongyloides Rapa Blu"},
        {"          ", "Sabauda     ", "Verza                           ", "Cavolo Verza                ", ""},
        {"Cipolla   ", "            ", "a Bulbo Giallo Piatto           ", "Cipolla                     ", "Bulbo Giallo"},
        {"          ", "            ", "a Bulbo Giallo Tondo            ", "                            ", "Bulbo Giallo Tondo"},
        {"          ", "            ", "a Bulbo Rosso Piatto            ", "                            ", "Bulbo Rosso"},
        {"          ", "            ", "a Bulbo Rosso Tondo             ", "                            ", "Bulbo Rosso Tondo"},
        {"          ", "            ", "a Bulbo Bianco Piatto           ", "                            ", "Bulbo Bianco"},
        {"          ", "            ", "a Bulbo Bianco Tondo            ", "                            ", "Bulbo Bianco Tondo"},
        {"          ", "            ", "tipo Dorata di Parma            ", "                            ", "tipo Dorata di Parma"},
        {"          ", "            ", "tipo Ramata di Milano           ", "                            ", "tipo Ramata di Milano"},
        {"          ", "            ", "tipo Bianca di Lisbona          ", "                            ", "tipo Bianca di Lisbona"},
        {"Zucchino  ", "            ", "Lungo Verde Scuro               ", "Zucchino                    ", "Lungo Scuro (Tipologia Americano)"},
        {"          ", "            ", "Lungo Verde Chiaro              ", "                            ", "Lungo Chiaro"},
        {"          ", "            ", "Lungo Striato                   ", "                            ", "Lungo Striato"},
        {"          ", "            ", "Lungo Bianco                    ", "                            ", "Lungo Bianco"},
        {"          ", "            ", "Rotondo Verde Scuro             ", "                            ", "Rotondo Verde Scuro"},
        {"          ", "            ", "Rotondo Verde Chiaro            ", "                            ", "Rotondo Verde Chiaro"},
        {"Cetriolo  ", "            ", "Corto                           ", "Cetriolo                    ", "Corto"},
        {"          ", "            ", "Medio                           ", "                            ", "Medio"},
        {"          ", "            ", "Lungo                           ", "                            ", "Lungo"}
        }

        Dim content As New StringBuilder

        content.AppendLine("<div style=""padding:20px;"">")

        content.AppendLine("<div style=""display:grid; grid-gap:1px; padding:1px 0px 1px 1px; background-color:#ccc; grid-template-columns:auto auto auto 1px auto auto;"">")

        content.AppendLine("<div class=""grid-item grid-header"" style=""grid-column: 1 / span 3; justify-content:center; font-size:large;"">Codifica SEMENTIERI</div>")
        content.AppendLine("<div class=""grid-item grid-header"" style=""grid-column: 5 / span 2; justify-content:center; font-size:large;"">Codifica GIAS</div>")
        content.AppendLine("<div class=""grid-item2 grid-header"" style=""font-size:larger;"">Specie</div>")
        content.AppendLine("<div class=""grid-item2 grid-header"" style=""font-size:larger;"">Sottospecie</div>")
        content.AppendLine("<div class=""grid-item2 grid-header"" style=""font-size:larger;"">Gruppo</div>")
        content.AppendLine("<div></div>")
        content.AppendLine("<div class=""grid-item2 grid-header"" style=""font-size:larger;"">Specie vegetale</div>")
        content.AppendLine("<div class=""grid-item2 grid-header"" style=""font-size:larger;"">Tipologia varietale</div>")

        Dim t_r_0 As String
        Dim t_r_1 As String
        Dim t_r_2 As String
        Dim t_r_3 As String
        Dim t_r_4 As String
        Dim alt_class = " grid-alt-clr"
        For r = 0 To tabella.GetUpperBound(0)
            t_r_0 = tabella(r, 0).Trim()
            t_r_1 = tabella(r, 1).Trim()
            t_r_2 = tabella(r, 2).Trim()
            t_r_3 = tabella(r, 3).Trim()
            t_r_4 = tabella(r, 4).Trim()

            If Not String.IsNullOrEmpty(t_r_0) Then
                If String.IsNullOrEmpty(alt_class) Then
                    alt_class = " grid-alt-clr"
                Else
                    alt_class = ""
                End If
            End If

            content.AppendLine("<div class=""grid-item2" & alt_class & """>" & t_r_0 & "</div>")
            content.AppendLine("<div class=""grid-item2" & alt_class & """>" & t_r_1 & "</div>")
            content.AppendLine("<div class=""grid-item2" & alt_class & """>" & t_r_2 & "</div>")
            content.AppendLine("<div></div>")
            content.AppendLine("<div class=""grid-item2 " & alt_class & """>" & t_r_3 & "</div>")
            content.AppendLine("<div class=""grid-item2 " & alt_class & """>" & t_r_4 & "</div>")
        Next

        content.AppendLine("</div>")
        content.AppendLine("</div>")

        Return content.ToString()
    End Function



    Private Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit

        If Session.IsNewSession Then
            Response.Redirect("../GST_Autenticazione/Autenticazione.aspx")
        End If

        '----- Dimensiono le variabili
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

    End Sub


    '####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If IsNothing(objParametri_Server) Then
            Response.Redirect("../GST_Autenticazione/Autenticazione.aspx")
        End If

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean = False

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                                    objParametri_Utenti.UtenteUsername,
                                    5,
                                    TipiEnumerativi.enum_Security_Attivita.Interferenze_MenuPrincipale_Accesso,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)



        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio alla autenticazione.

        If (UtenteAbilitato = False) Then
            Response.Redirect("../GST_Autenticazione/Autenticazione.aspx")
        End If

        If objParametri_Utenti.UtenteUsername <> objParametri_Server.SuperUserUsername Then
            btnGestioneSportello.Visible = False
            btnStampaElenco.Visible = False
        End If

        If Session("Codice_Fiscale_Tecnico") Is Nothing Then

            Dim Codice_Fiscale_Tecnico As String = ""
            Dim xUtenteCorrente As String = CType(objParametri_Server, AgronicaCoreDataProvider.AgronicaCoreParametri).UtenteUsername
            Dim objUtentiDAL As New AgronicaCoreUtentiDAL.Utenti_xGruppi_Utente_R
            Codice_Fiscale_Tecnico = objUtentiDAL.Leggi_IdentificativoGruppoUtenti_Singolo(xUtenteCorrente, objParametri_Utenti)
            Session("Codice_Fiscale_Tecnico") = Codice_Fiscale_Tecnico
            Session("Codice_Fiscale_Tecnico_OK") = Codice_Fiscale_Tecnico

        End If

        '##############################################################
        '#####  Preparazione della pagina  ############################
        '##############################################################

        '----- Attivo i pulsanti a seconda dei permessi

        Dim DT As New DataTable
        Dim i As Integer

        DT = objPermessi.Leggi(Session("ASG_Utente_Username"),
                               11,
                               0, 0,
                               0, "", "", objParametri_Utenti)


        If (Not IsNothing(DT)) AndAlso (DT.Rows.Count >= 1) Then

            Dim Attivita As String

            For i = 0 To DT.Rows.Count - 1

                Attivita = Right("0000000" & DT.Rows(i).Item("ID_Attivita"), 3)

                Select Case Attivita

                    Case "002"      'UTENTI
                        'imgBtnGestioneSportello.ImageUrl = MemoIcona_GestioneSportello
                        'imgBtnGestioneSportello.Enabled = True
                        'lblGestioneSportello.ForeColor = MemoColore_GestioneSportello

                    Case "003"      'DISTANZE
                        'ImgBtnGestioneConfigurazione.ImageUrl = MemoIcona_GestioneConfigurazione
                        'ImgBtnGestioneConfigurazione.Enabled = True
                        'LblGestioneConfigurazione.ForeColor = MemoColore_GestioneConfigurazione

                    Case "004"     'COLORI
                        'ImgBtnGestioneColori.ImageUrl = MemoIcona_GestioneColori
                        'ImgBtnGestioneColori.Enabled = True
                        'LblGestioneColori.ForeColor = MemoColore_GestioneColori

                    Case "005"

                        'Utente di tipo COOPERATIVA
                        Session("TipoUtente") = 1

                        'ImgBtnVerificaInterferenze.ImageUrl = MemoIcona_VerificaInterferenze
                        'ImgBtnVerificaInterferenze.Enabled = True
                        'LblVerificaInterferenze.ForeColor = MemoColore_VerificaInterferenze

                    Case "006"

                        'Utente di tipo SUPERUSER
                        Session("TipoUtente") = 2

                        'ImgBtnVerificaInterferenze.ImageUrl = MemoIcona_VerificaInterferenze
                        'ImgBtnVerificaInterferenze.Enabled = True
                        'LblVerificaInterferenze.ForeColor = MemoColore_VerificaInterferenze

                End Select

            Next

        End If

        'Pulizia Risorse
        DT.Dispose()

    End Sub


    ''####################################################################################
    'Private Sub ImgBtnGestioneColori_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnGestioneColori.Click

    '    Dim TargetURL As String
    '    TargetURL = "../GST_Colori/Gestione_Colori.aspx"
    '    Response.Redirect(TargetURL)

    'End Sub

End Class
