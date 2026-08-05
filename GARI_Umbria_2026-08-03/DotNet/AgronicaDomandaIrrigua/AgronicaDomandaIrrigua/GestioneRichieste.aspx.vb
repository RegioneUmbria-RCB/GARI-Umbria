Imports AgronicaCoreDataProvider
Imports System.Xml
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports System.Web
Imports AgronicaCoreModello
Imports AgronicaControlli_2010
Imports Agronica.Helpers.GiasBase

Public Class GestioneRichieste
    Inherits System.Web.UI.Page

    Private AgroKey_EncoderDecoder As String = "cobaltoleccioplutone"

    Dim objGestioneRichieste As GestioneRichiesteClasse

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request.QueryString("unid") IsNot Nothing Then

            '------------------------------------------------------------------------------
            '-------------------------SUPERSERVER-----------------------------------------
            '------------------------------------------------------------------------------


            Dim Unid, Cn_Server As String
            Dim objGestioneRichieste As GestioneRichiesteClasse




            Unid = Stringa_Decodifica(
                                    Request.QueryString("unid").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

            Cn_Server = Stringa_Decodifica(
                            Request.QueryString("cn").ToString,
                            AgroKey_EncoderDecoder,
                            Server)


            'devo inserire anche nella querystring 
            'la stringa connessione al superserver, 
            'altrimenti il sito chiamato (che non ha nel webconfig le chiavi per il superserver)
            'non è in grado di leggere l'xml di passaggio parametri
            'che è salvato sul db cliente.
            'quindi tramite la tringa superserver e l'id cn_server può
            'recuperare la stringa connessione per il db cliente e leggere xml
            'Senza superserver andava a leggere sempre da connessione.ini che ora non deve 
            'esser più utilizzato
            If Request.QueryString("StrConSup") IsNot Nothing AndAlso Request.QueryString("StrConSup") <> "" Then
                Dim StringaConnessioneSuperserver As String = ""
                StringaConnessioneSuperserver = Stringa_Decodifica(
                    Request.QueryString("StrConSup").ToString,
                    AgroKey_EncoderDecoder,
                    Server)

                objGestioneRichieste = New GestioneRichiesteClasse(Unid,
                                    Cn_Server,
                                    StringaConnessioneSuperserver,
                                    Server.MapPath("AB_Immagini/IconeVegetali").ToString)

            Else


                'modalità senza superserver

                objGestioneRichieste = New GestioneRichiesteClasse(Unid,
                                                    Cn_Server, "",
                                                    Server.MapPath("AB_Immagini/IconeVegetali").ToString)

            End If

            '==========================================
            '=========== LINGUE ==============
            '==========================================
            'controllo se è impostata una lingua 
            Dim ling As New Lingua
            If Request.QueryString("ln") IsNot Nothing Then
                Dim linguaCodiceISO As String() = Request.QueryString("ln").Split("|")
                ling.CodiceISO = linguaCodiceISO(0)
                ling.Lingua_cod = linguaCodiceISO(1)
                CType(Session("ASG_objParametri_Server"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod
                CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod

                ImpostaCultura(ling)
            Else
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Read
                ling = objUtenti.Leggi_Lingua(CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri).UtenteUsername, "", "", CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri))
                CType(Session("ASG_objParametri_Server"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod
                CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri).Lingua_Cod = ling.Lingua_cod
                ImpostaCultura(ling)
            End If

            '------------------------------------------------------------------------------
            '-------------------------SUPERSERVER END--------------------------------------
            '------------------------------------------------------------------------------
            Dim strParametri As String = ""

            Dim hasParametriAgendaSignificativi As Boolean = False
            If Not IsNothing(Session("ParametriAgenda_2010")) Then
                Dim objPA2010Check As New ParametriAgenda_2010
                objPA2010Check.Leggi()
                hasParametriAgendaSignificativi = (objPA2010Check.PaginaRichiesta <> enum_PagineAgenda_2010.Menu)
            End If

            If hasParametriAgendaSignificativi Then

                GestioneRedirect_ParametriAgenda_2010()

            ElseIf Not IsNothing(Session("ParametriDomandaIrrigua")) Then

                Dim objParametriDomandaIrrigua As New ParametriDomandaIrrigua
                objParametriDomandaIrrigua.Leggi()
                System.Web.HttpContext.Current.Session("ParametriAgenda") = Nothing
                Dim objParametriAgenda As New ParametriAgenda

                objParametriAgenda.SitoOrigine = HttpContext.Current.Session("Sito_Origine")
                objParametriAgenda.PaginaSitoOrigine = objParametriDomandaIrrigua.PaginaProvenienza

                SetObjParametriAgenda(objParametriAgenda, objParametriDomandaIrrigua)

                Dim objAgroWebConfig As New AgroWebConfig()
                objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari = objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
                objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci

                objParametriAgenda.TornaASitoOrigine = True

                Dim TargetRedirect As String = String.Empty

                Select Case objParametriDomandaIrrigua.PaginaRichiesta
                    Case enum_PagineAgronicaDomandaIrrigua.NuovaDomanda
                        Session("IDSezione") = 308
                        TargetRedirect = "./DomandeIrrigue/DomandaIrrigua.aspx"
                    Case enum_PagineAgronicaDomandaIrrigua.ElencoDomande
                        Session("IDSezione") = 309
                        TargetRedirect = "./DomandeIrrigue/RicercaDomandeIrrigue.aspx"
                    Case enum_PagineAgronicaDomandaIrrigua.LettureContatoreAziendale
                        Session("IDSezione") = 311
                        TargetRedirect = "./Tariffazione/LettureContatoriAziendali.aspx"
                    Case enum_PagineAgronicaDomandaIrrigua.CalcoloTariffazione
                        Session("IDSezione") = 312
                        TargetRedirect = "./Tariffazione/CalcoloTariffazione.aspx"

                    ' Pagine operazione portate da AgroAgenda_2010. Replicano la mappatura/handling
                    ' che era in GestioneRedirect_ParametriAgenda_2010, ma con i campi presi da
                    ' ParametriDomandaIrrigua (schema "single object" del porting). I valori dell'enum
                    ' nuovo coincidono con quelli di enum_PagineAgenda_2010 (es. Pagina_Trattamenti_B = 31)
                    ' per compatibilità col payload Angular.
                    Case enum_PagineAgronicaDomandaIrrigua.Pagina_Trattamenti_B
                        TargetRedirect = "./Operazioni/Trattamenti_2.aspx"
                        objParametriAgenda.Appezza = objParametriDomandaIrrigua.Appezza
                        objParametriAgenda.Id_Imp = objParametriDomandaIrrigua.Id_Reg
                        TargetRedirect &= If(objParametriDomandaIrrigua.QueryStringFiltrino, "")
                        objParametriAgenda.salva()

                    Case enum_PagineAgronicaDomandaIrrigua.Pagina_Installazione_Trapppole
                        TargetRedirect = "./Operazioni/Installazione_Trappole.aspx"
                        objParametriAgenda.salva()

                    Case enum_PagineAgronicaDomandaIrrigua.Pagina_Reinnesco_Trappole
                        TargetRedirect = "./Operazioni/Reinnesco_Rilievi_Trappole.aspx"
                        objParametriAgenda.salva()

                    Case enum_PagineAgronicaDomandaIrrigua.Pagina_RilieviBS
                        TargetRedirect = "./Operazioni/RilieviBS.aspx"
                        objParametriAgenda.salva()

                    Case enum_PagineAgronicaDomandaIrrigua.Pagina_Raccolta
                        TargetRedirect = "./Operazioni/Raccolta.aspx"
                        objParametriAgenda.salva()

                    Case enum_PagineAgronicaDomandaIrrigua.Pagina_TrattamentiPostRaccolta
                        TargetRedirect = "./Operazioni/Trattamenti_PostRaccolta.aspx"
                        objParametriAgenda.salva()

                    Case enum_PagineAgronicaDomandaIrrigua.Pagina_Distribuzione_Insetti
                        TargetRedirect = "./Operazioni/Distribuzione_Insetti.aspx"
                        objParametriAgenda.salva()
                End Select

                If Not String.IsNullOrEmpty(TargetRedirect) Then
                    Redirect(TargetRedirect)
                End If

            End If

        Else

        End If
    End Sub

    Private Sub SetObjParametriAgenda(ByRef objParametriAgenda As ParametriAgenda, ByRef objParametriDomandaIrrigua As ParametriDomandaIrrigua)
        objParametriAgenda.Piva = objParametriDomandaIrrigua.Piva
        ' Stessa convenzione dell'overload ParametriAgenda_2010: se la data è il default AGRODATAINIZIO,
        ' uso oggi per avere una data valida per specie/impianti. L'utente può cambiarla a runtime.
        If objParametriDomandaIrrigua.DataSelezionata = AGRODATAINIZIO Then
            objParametriAgenda.Data = Date.Today
        Else
            objParametriAgenda.Data = objParametriDomandaIrrigua.DataSelezionata
        End If
        objParametriAgenda.Id_Agenda = objParametriDomandaIrrigua.Id_Agenda
        objParametriAgenda.Sa_Cod = objParametriDomandaIrrigua.Sa_Cod
        objParametriAgenda.Veg_Cod = objParametriDomandaIrrigua.Veg_Cod
        objParametriAgenda.Lav_Cod = objParametriDomandaIrrigua.Lav_Cod
        objParametriAgenda.TipoOperazioneAgenda = objParametriDomandaIrrigua.TipoOperazioneAgenda
        objParametriAgenda.TargetOperazione = objParametriDomandaIrrigua.TargetOperazione
        objParametriAgenda.Programmazione_Cod = objParametriDomandaIrrigua.Programmazione_Cod
        objParametriAgenda.Tipo_Operazione = objParametriDomandaIrrigua.Tipo_Operazione
        objParametriAgenda.TipoRicetta = objParametriDomandaIrrigua.TipoRicetta
        objParametriAgenda.RedirectUrl = objParametriDomandaIrrigua.RedirectUrl
        objParametriAgenda.QueryStringFiltrino = objParametriDomandaIrrigua.QueryStringFiltrino
    End Sub

    Private Sub SetObjParametriAgenda(ByRef objParametriAgenda As ParametriAgenda, ByRef objParametriAgenda_2010 As ParametriAgenda_2010)
        objParametriAgenda.Piva = objParametriAgenda_2010.Piva
        ' Conformare il comportamento con Angular: se la data arriva come AGRODATAINIZIO (default dal menu di AgroAgenda),
        ' uso oggi così specie/impianti vengono filtrati su una data valida. L'utente può comunque cambiarla nella pagina.
        If objParametriAgenda_2010.DataSelezionata = AGRODATAINIZIO Then
            objParametriAgenda.Data = Date.Today
        Else
            objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
        End If
        objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
        objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
        objParametriAgenda.Veg_Cod = objParametriAgenda_2010.Veg_Cod
        objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lav_Cod
        objParametriAgenda.TipoOperazioneAgenda = objParametriAgenda_2010.TipoOperazioneAgenda
        objParametriAgenda.TargetOperazione = objParametriAgenda_2010.TargetOperazione
        objParametriAgenda.Programmazione_Cod = objParametriAgenda_2010.Programmazione_Cod
        objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Tipo_Operazione
        objParametriAgenda.TipoRicetta = objParametriAgenda_2010.TipoRicetta
        objParametriAgenda.RedirectUrl = objParametriAgenda_2010.RedirectUrl
        objParametriAgenda.QueryStringFiltrino = objParametriAgenda_2010.QueryStringFiltrino

        If objParametriAgenda_2010.Impianti IsNot Nothing AndAlso objParametriAgenda_2010.Impianti.Count > 0 Then
            objParametriAgenda.Impianti = New List(Of ParametriAgenda_Temp.Impianto)
            For Each imp In objParametriAgenda_2010.Impianti
                Dim impianto_temp As New ParametriAgenda_Temp.Impianto
                impianto_temp.Piva = imp.Piva
                impianto_temp.Sa_Cod = imp.Sa_Cod
                impianto_temp.Appezza = imp.Appezza
                impianto_temp.ID_Reg = imp.Id_Reg
                impianto_temp.Progetto_Cod = imp.Progetto_Cod
                impianto_temp.Veg_Cod = imp.veg_cod
                objParametriAgenda.Impianti.Add(impianto_temp)
            Next
        End If
    End Sub

    Private Sub GestioneRedirect_ParametriAgenda_2010()
        Dim objParametriAgenda_2010 As New ParametriAgenda_2010
        objParametriAgenda_2010.Leggi()

        System.Web.HttpContext.Current.Session("ParametriAgenda") = Nothing
        Dim objParametriAgenda As New ParametriAgenda

        objParametriAgenda.SitoOrigine = HttpContext.Current.Session("Sito_Origine")
        objParametriAgenda.PaginaSitoOrigine = objParametriAgenda_2010.PaginaProvenienza

        SetObjParametriAgenda(objParametriAgenda, objParametriAgenda_2010)

        Dim objAgroWebConfig As New AgroWebConfig()
        objParametriAgenda.WS_Disciplinari_AgroWS_Disciplinari = objAgroWebConfig.GiasOnline_WS_Disciplinari_AgroWS_Disciplinari
        objParametriAgenda.WS_Fitofarmaci_AgroWS_Fitofarmaci = objAgroWebConfig.GiasOnline_WS_Fitofarmaci_AgroWS_Fitofarmaci

        objParametriAgenda.TornaASitoOrigine = True

        Dim TargetRedirect As String = String.Empty

        Select Case objParametriAgenda_2010.PaginaRichiesta

            Case enum_PagineAgenda_2010.Pagina_Trattamenti, enum_PagineAgenda_2010.Pagina_Trattamenti_B
                TargetRedirect = "./Operazioni/Trattamenti_2.aspx"
                objParametriAgenda.Appezza = objParametriAgenda_2010.Appezza
                objParametriAgenda.Id_Imp = objParametriAgenda_2010.Id_Reg
                TargetRedirect += objParametriAgenda_2010.QueryStringFiltrino
                objParametriAgenda.salva()

            Case enum_PagineAgenda_2010.Pagina_Installazione_Trapppole
                TargetRedirect = "./Operazioni/Installazione_Trappole.aspx"
                objParametriAgenda.salva()

            Case enum_PagineAgenda_2010.Pagina_Reinnesco_Trapppole
                TargetRedirect = "./Operazioni/Reinnesco_Rilievi_Trappole.aspx"
                objParametriAgenda.salva()

            Case enum_PagineAgenda_2010.Pagina_RilieviBS
                TargetRedirect = "./Operazioni/RilieviBS.aspx"
                objParametriAgenda.salva()

            Case enum_PagineAgenda_2010.Pagina_Raccolta
                TargetRedirect = "./Operazioni/Raccolta.aspx"
                objParametriAgenda.salva()

            Case enum_PagineAgenda_2010.Pagina_TrattamentiPostRaccolta
                TargetRedirect = "./Operazioni/Trattamenti_PostRaccolta.aspx"
                objParametriAgenda.salva()

            Case enum_PagineAgenda_2010.Pagina_Distribuzione_Insetti
                TargetRedirect = "./Operazioni/Distribuzione_Insetti.aspx"
                objParametriAgenda.salva()

        End Select

        If Not String.IsNullOrEmpty(TargetRedirect) Then
            Redirect(TargetRedirect)
        End If
    End Sub

    Private Sub Redirect(ByVal targetUrl As String)

        Dim objParametri_Super_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Utenti As AgronicaCoreParametri = Nothing

        If Not IsNothing(Session("ASG_objParametri_Super_Server")) Then
            objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        End If
        If Not IsNothing(Session("ASG_objParametri_Server")) Then
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        End If
        If Not IsNothing(Session("ASG_objParametri_Utenti")) Then
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        End If
        Dim idSezione = 0

        GiasBaseHelper.WarmUp_GestioneRichieste()

        Dim apiController As CoreApiControllerFactory = New CoreApiControllerFactory
        If Not IsNothing(objParametri_Super_Server) AndAlso Not IsNothing(objParametri_Server) Then
            apiController.Inizializza(objParametri_Super_Server, objParametri_Server)
            idSezione = apiController.DammiIdSezioneDaQueryString()
        End If

        If apiController.CanUseAPI AndAlso apiController.VersioneHeader = "2022" Then

            If idSezione <> 0 Then
                targetUrl = AgronicaCoreUtility.Varie.aggiungiAQueryString(targetUrl, "idBC", Stringa_Codifica(idSezione, AgroKey_EncoderDecoder))
            End If

            If Not IsNothing(Request.QueryString("sidebar")) Then
                targetUrl = AgronicaCoreUtility.Varie.aggiungiAQueryString(targetUrl, "sidebar", "off")
            End If

        End If

        Response.Redirect(targetUrl)

    End Sub

    Friend Sub ImpostaCultura(ByRef lingua As Lingua)
        If lingua IsNot Nothing Then
            System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(lingua.CodiceISO)
            '' questa istruzione da errore
            'System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en")
            '_LinguaCorrente = lingua
            Session("LinguaCorrente") = lingua
        End If
    End Sub

End Class