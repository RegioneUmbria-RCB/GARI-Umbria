Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreDpiBIZ

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class Utenti_Impostazioni_W
    Inherits System.Web.Services.WebService

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Set_AnnataAgraria(objP_utenti As String, inizio_GG As String, inizio_MM As String, fine_GG As String, fine_MM As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            'Controllo i dati che mi sono stati passati
            If Not IsNumeric(inizio_GG) OrElse Not IsNumeric(inizio_MM) OrElse Not IsNumeric(fine_GG) OrElse Not IsNumeric(fine_MM) Then
                r.Errore = "Errore durante l'operazione: i dati inviati non sono corretti"
                Return r
            End If

            'Controllo se esite già il dato per decidere se fare un insert o un update
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt As DataTable = ui_R.Leggi(enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim ui_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W
            If dt.Rows.Count > 0 Then
                ui_W.Modifica(enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria,
                                      inizio_GG.PadLeft(2, "0") & inizio_MM.PadLeft(2, "0") & fine_GG.PadLeft(2, "0") & fine_MM.PadLeft(2, "0"), "", "", "",
                                      CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE, objParametri_Utenti)
            Else
                ui_W.Scrivi(enum_Impostazioni_Utenti.UTENTE_InizioFineAnnataAgraria,
                                      inizio_GG.PadLeft(2, "0") & inizio_MM.PadLeft(2, "0") & fine_GG.PadLeft(2, "0") & fine_MM.PadLeft(2, "0"), "", "", "",
                                      CostantiPersonalizzate.AGRODATAINIZIO, CostantiPersonalizzate.AGRODATAFINE, objParametri_Utenti)
            End If


            r.RispostaOK = True
            r.RispostaStringa = "ok"

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Set_ImpostazioniDisciplinari(objP_server As String, objP_utenti As String, piva As String, disciplinare As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If piva = "" Then
            r.Errore = "piva non valorizzato"
            Return r
        End If

        If disciplinare = "" Then
            r.Errore = "disciplinare non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Try

            'Controllo se è un utente CAA (l'impostazione non esiste o è vuota)
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim res As String = ui_R.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_Attiva_Configurazione_Pratica, objParametri_Utenti)

            '---------------------------SALVO LE STAMPE PREFERITE--------------------------------------
            Dim ui_W As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W

            If res = "1" Then 'Se è un utente del portale soci (non CAA) salvo le preferenze delle stampe

                Dim strStampe As String = ""

                'Vado in sostituzione di tutti i preferiti, ovviamente se ci torno, è un problema...
                If disciplinare = "delete" Then
                    ui_W.Cancella(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, "", objParametri_Utenti)
                Else

                    Dim arrayStampe As New List(Of String)

                    If disciplinare = "-2" Then 'BIO

                        arrayStampe.AddRange({enum_CodificaStampe.SchedaColturale_Biologico, enum_CodificaStampe.SchedaMateriePrime_Biologico,
                                     enum_CodificaStampe.Bolle, enum_CodificaStampe.PAP_Vegetale, enum_CodificaStampe.SchedaVendite_Biologico,
                                     enum_CodificaStampe.SchedaMagazzinoGiacenze, enum_CodificaStampe.SchedaMagazzinoMovimenti,
                                     enum_CodificaStampe.Eurep_Gap_Multicentro, enum_CodificaStampe.SchedaTracciabilita, enum_CodificaStampe.Quadro_P})

                    ElseIf disciplinare.Contains("/") Then ' INTEGRATA

                        Dim regione As String = OttieniRegioneDaDisciplinare(objP_server, objP_utenti, disciplinare)

                        Select Case regione
                            Case enum_Regioni.Veneto
                                arrayStampe.Add(enum_CodificaStampe.SchedaCampagna_Multi_Lombardia)
                                arrayStampe.Add(enum_CodificaStampe.RegistroTrattamenti_Veneto)
                            Case enum_Regioni.Lombardia
                                arrayStampe.Add(enum_CodificaStampe.SchedaCampagna_Multi_Lombardia)
                                arrayStampe.Add(enum_CodificaStampe.RegistroTrattamenti_Veneto)
                            Case enum_Regioni.Trentino_Alto_Adige
                                arrayStampe.Add(enum_CodificaStampe.SchedaCampagna_ProvAut_Trento)
                            Case Else
                                arrayStampe.Add(enum_CodificaStampe.SchedaCampagna_Multicentro)
                        End Select

                        arrayStampe.AddRange({enum_CodificaStampe.Eurep_Gap_Multicentro, enum_CodificaStampe.SchedaMagazzinoFertilizzanti, enum_CodificaStampe.SchedaMagazzinoGiacenze,
                                      enum_CodificaStampe.SchedaMagazzinoMovimenti, enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari,
                                     enum_CodificaStampe.Quadro_P, enum_CodificaStampe.SchedaTracciabilita})

                    ElseIf disciplinare = "0" Then ' CONVENZIONALE

                        Dim ixi_R As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
                        Dim dtInd As DataTable = ixi_R.Leggi(piva, 0, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta, "", "", objParametri_Server)

                        If Not IsNothing(dtInd) AndAlso dtInd.Rows.Count > 0 Then

                            Select Case CInt(dtInd.Rows(0).Item("reg_cod"))
                                Case enum_Regioni.Veneto
                                    arrayStampe.Add(enum_CodificaStampe.RegistroTrattamenti_Veneto)
                                Case enum_Regioni.Lombardia
                                    arrayStampe.Add(enum_CodificaStampe.RegistroTrattamenti_Veneto)
                                Case enum_Regioni.Trentino_Alto_Adige
                                    arrayStampe.Add(enum_CodificaStampe.SchedaCampagna_ProvAut_Trento)
                                Case enum_Regioni.Emilia_Romagna
                                    arrayStampe.Add(enum_CodificaStampe.SchedaRegistrazione_Semplificata) 'Registro dei trattamenti ER
                                Case Else
                                    arrayStampe.Add(enum_CodificaStampe.RegistroTrattamenti_Semplificata)
                            End Select

                        Else
                            arrayStampe.Add(enum_CodificaStampe.RegistroTrattamenti_Semplificata)
                        End If

                        arrayStampe.AddRange({enum_CodificaStampe.SchedaMagazzinoMovimenti, enum_CodificaStampe.SchedaMagazzinoGiacenze, enum_CodificaStampe.Quadro_P})

                    End If

                    'Estraggo i pacchetti acquistati
                    Dim leggiServizioStatoQDC As New AgronicaCoreProfilazioneBIZ.Pratiche_R
                    Dim xRisp As RispostaStandard = leggiServizioStatoQDC.leggiServiziStati(enum_WWorflow.Servizi_Agronica_2017, piva, 0, "", objParametri_Server, objParametri_Utenti)

                    If xRisp.RispostaOK Then
                        Dim descrizioneServizioStatoQDCArray As JArray = JArray.Parse(xRisp.RispostaStringa)

                        For Each itemDescrizioneServizioStatoQDC As JObject In descrizioneServizioStatoQDCArray
                            Select Case CInt(itemDescrizioneServizioStatoQDC("ServizioCod"))
                            'Case enum_Servizi.QStandard
                            '    bool_QStandard = True
                            'Case enum_Servizi.QPlus
                            '    bool_QPlus = True
                            'Case enum_Servizi.QBio
                            '    bool_QBio = True
                            'Case enum_Servizi.QMaps
                            '    bool_QMaps = True
                                Case enum_Servizi.QFert
                                    arrayStampe.AddRange({enum_CodificaStampe.Bilancio_Fertilizzazioni, enum_CodificaStampe.Registro_Fertilizzazioni})

                            End Select
                        Next

                    Else
                        r.Errore = "Impossibile leggere l'elenco dei servizi acquistati"
                        Return r
                    End If


                    strStampe = String.Join("|", arrayStampe)

                    'Controllo se esisteva già il record e di conseguenza scrivo o modifico
                    Dim dt As DataTable = ui_R.Leggi2(1, objParametri_Server.UtenteUsername, enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, "", "", objParametri_Utenti)

                    If IsNothing(dt) OrElse dt.Rows.Count = 0 Then
                        ui_W.Scrivi(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, strStampe, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
                    Else
                        ui_W.Modifica(enum_Impostazioni_Utenti.UTENTE_STAMPE_PREFERITE, strStampe, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
                    End If

                End If

            End If


            '---------------------------SALVO IL DISCIPLINARE------------------------------------------
            Dim ic_W As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Write

            If disciplinare = "delete" Then
                ic_W.Cancella(piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, "", objParametri_Server)

            Else
                'Controllo se esisteva già il record e di conseguenza scrivo o modifico
                Dim ic_R As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Dim oldVal As String = ic_R.Leggi_Codice_from_Imprese_Codici(piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, objParametri_Server)

                If oldVal = "" Then
                    ic_W.Scrivi(piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, disciplinare, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                Else
                    ic_W.Modifica(piva, enum_CodiciAnagrafe.Disciplinare_Aziendale_Default, disciplinare, AGRODATAINIZIO, AGRODATAFINE, "", objParametri_Server)
                End If
            End If

            '--------------------------------------------------------------------------------------------


            r.RispostaOK = True
            r.RispostaStringa = "ok"

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    Private Function OttieniRegioneDaDisciplinare(objP_server As String, objP_utenti As String, disciplinare As String) As String

        'Estraggo la regione di appartenenza del disciplinare
        Dim dpi_r As New DPI
        Dim datiDisciplinare As String = dpi_r.CaricaDettagliDisciplinare(objP_server, objP_utenti, disciplinare).RispostaStringa

        Dim XmlDocumento As New System.Xml.XmlDocument()
        XmlDocumento.LoadXml(datiDisciplinare)

        'Dim regione As String = XmlDocumento.GetAttribute("id_ente")
        Dim regione As String = XmlDocumento.GetElementsByTagName("Record")(0).Attributes("reg_cod").Value

        Return regione

    End Function

    <WebMethod()>
    <Script.Services.ScriptMethod()>
    Public Function Set_Impostazioni(objP_Utenti As String, impostazione_cod As Integer, valoreDaSalvare As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_Utenti = "" Then
            r.Errore = "objP_Utenti non valorizzato"
            Return r
        End If

        If Not IsNumeric(impostazione_cod) OrElse impostazione_cod = 0 Then
            r.Errore = "impostazione_cod non valorizzato"
            Return r
        End If

        If valoreDaSalvare = "" Then
            r.Errore = "valoreDaSalvare non valorizzato"
            Return r
        End If

        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_Utenti)

        Try

            Dim ui_w As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W()

            If valoreDaSalvare = "notSet" Then
                ui_w.Cancella(impostazione_cod, "", objParametri_Utenti)
            Else

                'Controllo se esisteva già il record e di conseguenza scrivo o modifico
                Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim dt As DataTable = ui_R.Leggi(impostazione_cod, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

                If IsNothing(dt) OrElse dt.Rows.Count = 0 Then
                    ui_w.Scrivi(impostazione_cod, valoreDaSalvare, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
                Else
                    ui_w.Modifica(impostazione_cod, valoreDaSalvare, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
                End If

            End If

            r.RispostaOK = True
            r.RispostaStringa = "ok"

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function Set_ImpostazioniMagazzino(objP_server As String, objP_utenti As String, piva As String, magazzino As String) As RispostaStandard

        Dim r As New RispostaStandard()

        If objP_server = "" Then
            r.Errore = "objP_server non valorizzato"
            Return r
        End If

        If objP_utenti = "" Then
            r.Errore = "objP_utenti non valorizzato"
            Return r
        End If

        If piva = "" Then
            r.Errore = "piva non valorizzato"
            Return r
        End If

        If magazzino = "" Then
            r.Errore = "magazzino non valorizzato"
            Return r
        End If

        Dim objParametri_Server As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_server)
        Dim objParametri_Utenti As AgronicaCoreParametri = Utility.convertStringtoOBJparametri(objP_utenti)

        Dim ui_w As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_W()

        If magazzino = "notSet" Then
            ui_w.Cancella(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO, "", objParametri_Utenti)
        Else

            'Controllo se esisteva già il record e di conseguenza scrivo o modifico
            Dim ui_R As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim dt As DataTable = ui_R.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            If IsNothing(dt) OrElse dt.Rows.Count = 0 Then
                ui_w.Scrivi(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO, magazzino, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
            Else
                ui_w.Modifica(enum_Impostazioni_Utenti.UTENTE_COD_DEFAULT_GESTIONE_MAGAZZINO, magazzino, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
            End If

            If magazzino = "0" Then
                'Se non volgio il magazzino, tolgo anche l'obbligatorietà
                'Controllo se esisteva già il record e di conseguenza scrivo o modifico
                Dim dt2 As DataTable = ui_R.Leggi(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO, 1, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

                If IsNothing(dt2) OrElse dt2.Rows.Count = 0 Then
                    ui_w.Scrivi(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO, 0, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
                Else
                    ui_w.Modifica(enum_Impostazioni_Utenti.UTENTE_COD_BLOCCA_SE_SENZA_MAGAZZINO, 0, "", "", "", AGRODATAINIZIO, AGRODATAFINE, objParametri_Utenti)
                End If
            Else
                'TO DO: imposto l'impostazione in base al mio profilo...

            End If

        End If

        r.RispostaOK = True
        r.RispostaStringa = "ok"

        Return r

    End Function
End Class