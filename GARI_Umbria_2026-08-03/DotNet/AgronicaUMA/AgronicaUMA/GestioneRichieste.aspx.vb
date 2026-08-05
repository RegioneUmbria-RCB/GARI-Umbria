Imports AgronicaCoreDataProvider
Imports System.Xml
Imports AgronicaCoreDataProvider.Sicurezza
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

        If Not Request.QueryString("unid") Is Nothing Then

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

            Session.Clear()

            GiasBaseHelper.WarmUp_GestioneRichieste()

            'devo inserire anche nella querystring 
            'la stringa connessione al superserver, 
            'altrimenti il sito chiamato (che non ha nel webconfig le chiavi per il superserver)
            'non è in grado di leggere l'xml di passaggio parametri
            'che è salvato sul db cliente.
            'quindi tramite la tringa superserver e l'id cn_server può
            'recuperare la stringa connessione per il db cliente e leggere xml
            'Senza superserver andava a leggere sempre da connessione.ini che ora non deve 
            'esser più utilizzato
            If Not Request.QueryString("StrConSup") Is Nothing AndAlso Request.QueryString("StrConSup") <> "" Then
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
            If Not Request.QueryString("ln") Is Nothing Then
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

            If Not IsNothing(Session("ParametriAgenda_2010")) Then

                Dim objParametriAgenda_2010 As New ParametriAgenda_2010
                objParametriAgenda_2010.Leggi()
                'scommentare dopo test
                'Session("ParametriAgenda_2010") = Nothing
                System.Web.HttpContext.Current.Session("ParametriAgenda") = Nothing
                Dim objParametriAgenda As New ParametriAgenda

                'testare
                objParametriAgenda.SitoOrigine = HttpContext.Current.Session("Sito_Origine")
                SetObjParametriAgenda(objParametriAgenda, objParametriAgenda_2010)

                Dim TargetRedirect As String = String.Empty

                Select Case objParametriAgenda_2010.PaginaRichiesta
                    Case enum_PagineAgronicaUMA.UMA_Ricerca_Macrousi_Lavorazioni
                        Session("IDSezione") = 336
                        TargetRedirect = "./CarburantiUMA/ElencoRichieste.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&avanzamento=2"
                    Case enum_PagineAgronicaUMA.UMA_Approvazione_Rendicontazioni
                        Session("IDSezione") = 232
                        TargetRedirect = "./CarburantiUMA/ElencoRichieste.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&avanzamento=1"
                    Case enum_PagineAgronicaUMA.UMA_Approvazione_Richieste
                        Session("IDSezione") = 231
                        TargetRedirect = "./CarburantiUMA/ElencoRichieste.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder)
                    Case enum_PagineAgronicaUMA.UMA_Configurazione
                        Session("IDSezione") = 228
                        TargetRedirect = "./CarburantiUMA/ConfigurazioneUMA.aspx"
                    Case enum_PagineAgronicaUMA.UMA_Elenco_Rendicontazioni
                        Session("IDSezione") = 227
                        TargetRedirect = "./CarburantiUMA/ElencoRichieste.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&avanzamento=1"
                    Case enum_PagineAgronicaUMA.UMA_Elenco_Richieste
                        Session("IDSezione") = 218
                        TargetRedirect = "./CarburantiUMA/ElencoRichieste.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder)
                    Case enum_PagineAgronicaUMA.UMA_Nuova_Rendicontazione
                        Session("IDSezione") = 225
                        TargetRedirect = "./CarburantiUMA/RichiestaCarburanti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&avanzamento=1"
                    Case enum_PagineAgronicaUMA.UMA_Nuova_Rendicontazione_Terzista
                        Session("IDSezione") = 226
                        TargetRedirect = "./CarburantiUMA/RichiestaCarburanti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&type=-1" & "&avanzamento=1" & "&tipo_azienda=" & enum_TipoAzienda_UMA.Azienda_Terzista
                    Case enum_PagineAgronicaUMA.UMA_Nuova_Richiesta
                        Session("IDSezione") = 217
                        TargetRedirect = "./CarburantiUMA/RichiestaCarburanti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder)
                    Case enum_PagineAgronicaUMA.UMA_Nuova_Richiesta_Terzista
                        Session("IDSezione") = 224
                        TargetRedirect = "./CarburantiUMA/RichiestaCarburanti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&type=-1" & "&tipo_azienda=" & enum_TipoAzienda_UMA.Azienda_Terzista
                    Case enum_PagineAgronicaUMA.UMA_Sintesi
                        Session("IDSezione") = 235
                        TargetRedirect = "./CarburantiUMA/RiepilogoRichieste.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder)
                    Case enum_PagineAgronicaUMA.UMA_Vendite_Carburante
                        Session("IDSezione") = 233
                        TargetRedirect = "./CarburantiUMA/VenditeCarburantiUMA.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder)
                    Case enum_PagineAgronicaUMA.UMA_Richiesta_Anticipo
                        Session("IDSezione") = 294
                        TargetRedirect = "./CarburantiUMA/RichiestaCarburanti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&anticipo=1"
                    Case enum_PagineAgronicaUMA.UMA_Richiesta_Anticipo_Terzista
                        Session("IDSezione") = 298
                        TargetRedirect = "./CarburantiUMA/RichiestaCarburanti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&anticipo=1" & "&type=-1" & "&tipo_azienda=" & enum_TipoAzienda_UMA.Azienda_Terzista
                    Case enum_PagineAgronicaUMA.UMA_Richiesta_Anticipo_Cooperativa
                        Session("IDSezione") = 299
                        TargetRedirect = "./CarburantiUMA/RichiestaCarburanti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&anticipo=1" & "&type=-1" & "&tipo_azienda=" & enum_TipoAzienda_UMA.Cooperativa_Agricola
                    Case enum_PagineAgronicaUMA.UMA_Blocco_Particelle
                        Session("IDSezione") = 297
                        TargetRedirect = "./CarburantiUMA/BloccoParticelle.aspx"
                    Case enum_PagineAgronicaUMA.UMA_Nuova_Richiesta_Cooperativa
                        Session("IDSezione") = 292
                        TargetRedirect = "./CarburantiUMA/RichiestaCarburanti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&type=-1" & "&avanzamento=0" & "&tipo_azienda=" & enum_TipoAzienda_UMA.Cooperativa_Agricola
                    Case enum_PagineAgronicaUMA.UMA_Nuova_Rendicontazione_Cooperativa
                        Session("IDSezione") = 293
                        TargetRedirect = "./CarburantiUMA/RichiestaCarburanti.aspx?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) & "&type=-1" & "&avanzamento=1" & "&tipo_azienda=" & enum_TipoAzienda_UMA.Cooperativa_Agricola
                    Case enum_PagineAgronicaUMA.UMA_Report_Controllo
                        Session("IDSezione") = 330
                        TargetRedirect = "./CarburantiUMA/ReportControllo.aspx"
                    Case enum_PagineAgronicaUMA.UMA_Visibilita_Aziende
                        Session("IDSezione") = 245
                        TargetRedirect = "./CarburantiUMA/Utenti_Visibilita_Area.aspx?area=0"
                End Select

                Redirect(TargetRedirect)

            End If

        Else

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

    Private Sub SetObjParametriAgenda(ByRef objParametriAgenda As ParametriAgenda, ByRef objParametriAgenda_2010 As ParametriAgenda_2010)
        objParametriAgenda.Piva = objParametriAgenda_2010.Piva
        objParametriAgenda.Data = objParametriAgenda_2010.DataSelezionata
        objParametriAgenda.Id_Agenda = objParametriAgenda_2010.Id_Agenda
        objParametriAgenda.Sa_Cod = objParametriAgenda_2010.Sa_Cod
        objParametriAgenda.Lav_Cod = objParametriAgenda_2010.Lav_Cod
        objParametriAgenda.TipoOperazioneAgenda = objParametriAgenda_2010.TipoOperazioneAgenda
        objParametriAgenda.TargetOperazione = objParametriAgenda_2010.TargetOperazione
        objParametriAgenda.Programmazione_Cod = objParametriAgenda_2010.Programmazione_Cod
        objParametriAgenda.Tipo_Operazione = objParametriAgenda_2010.Tipo_Operazione

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

    Friend Sub ImpostaCultura(ByRef lingua As Lingua)
        If Not lingua Is Nothing Then
            System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(lingua.CodiceISO)
            '' questa istruzione da errore
            'System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en")
            '_LinguaCorrente = lingua
            Session("LinguaCorrente") = lingua
        End If
    End Sub

End Class