
Imports <xmlns="http://www.agronica.it/track/">


Imports System.Web.Services

Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreXML.XML_Stampe
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ
Imports System.Drawing
Imports AgronicaCoreContabDAL
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.My.Resources
Imports AgroAgenda_2010.Resources

Public Class MenuBS_Risposta
    Inherits RispostaStandard

    Public IsLink As Boolean

    Public Intestazione As String

    Public ImmagineProdotto As String

End Class

Public Class MenuBS_WS
    Inherits System.Web.UI.Page


    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub


#Region "Gestione Rintracciabilità"




    <WebMethod(EnableSession:=True)>
    Public Shared Function Test(ByVal filtro As String) As RispostaStandard
        Dim r As New RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If


        Try

            'Inserire il codice QUI..
            Dim dt1 As DataTable = HttpContext.Current.Session("dt")
            Dim dt2 As DataTable = AgronicaControlli_2010.jquery_watable_modificato.FiltraDTconFiltriWatable(dt1, filtro)


            r.RispostaOK = True
            r.RispostaStringa = " cosa vuoi comunicare"

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function Gestione_Operazione(
            ByVal Data As String,
            ByVal Piva As String,
            ByVal id_Agenda As String,
            ByVal Blocco_Flag As String,
            ByVal Lav_Cod As String,
            ByVal veg_cod As String,
            ByVal sa_cod As String,
            ByVal MenuAgenda_SelectedValue As String,
            ByVal PaginaRitorno As String
                ) As MenuBS_Risposta




        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Utenti As AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim ASG_Utente_Username As String
        ASG_Utente_Username = HttpContext.Current.Session("ASG_Utente_Username")

        Dim ASG_IdServizio As String
        ASG_IdServizio = HttpContext.Current.Session("ASG_IdServizio")

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If



        Dim UtenteAbilitato_Modifica As String = "True"

        Dim str_RISPOSTA As String

        Try

            'Inserire il codice QUI..
            str_RISPOSTA = MenuBS_Operazioni.Gestione_Operazione(
                id_Agenda,
                UtenteAbilitato_Modifica,
                MenuAgenda_SelectedValue,
                ASG_Utente_Username,
                ASG_IdServizio,
                Data,
                Lav_Cod,
                Blocco_Flag,
                Piva,
                veg_cod,
                sa_cod,
                objParametri_Server,
                objParametri_Utenti,
                PaginaRitorno
            )

            If Not str_RISPOSTA.StartsWith("ERR") Then
                r.IsLink = True
            End If
            r.RispostaOK = True
            r.RispostaStringa = str_RISPOSTA


        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaGrigliaRigheDocumenti(ByVal lotto As String,
            ByVal docNumeroSin As String, ByVal docNumero As Integer,
            ByVal docNumeroDes As String, ByVal nrRiga As String, ByVal fromOutToIn As Boolean, ByVal certificazione As Integer
       ) As RispostaStandard

        Dim objParametriAgenda As New ParametriAgenda

        Dim r As New RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim xSelectAggiuntiva As String = ""
            Dim xFiltroAggiuntivo As String = ""
            Dim xOrderBy As String = ""
            Dim FlagJollyInt As Short = 0
            Dim LavCodArray As Integer() = Nothing
            Dim Cau_MovArray As String() = Nothing
            Dim Cau_Agg As Integer = 0
            If fromOutToIn Then
                ReDim LavCodArray(2)
                LavCodArray(0) = (LAVCOD_BOLLA_EMESSA)
                LavCodArray(1) = (LAVCOD_FATTURA_EMESSA)
                LavCodArray(2) = (LAVCOD_DDT_CONTABILIZZATO_EMESSO)
                xSelectAggiuntiva += ", MovimentiCausaleAgg.Doc_Numero_Sin As MCauAgg_Doc_Numero_Sin , MovimentiCausaleAgg.Doc_Numero As MCauAgg_Doc_Numero, MovimentiCausaleAgg.Doc_Numero_Des As  MCauAgg_Doc_Numero_Des "
                Cau_Agg = CAU_REGISTRAZIONI
                If docNumeroSin <> "" Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero_Sin = '" & docNumeroSin & "'   "
                End If
                If docNumero <> 0 Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero = " & docNumero & "   "
                End If
                If docNumeroDes <> "" Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero_Des = '" & docNumeroDes & "'   "
                End If
                If nrRiga <> "" Then
                    xFiltroAggiuntivo += " And Movimenti_Dettagli.Extra_Str = '" & nrRiga & "' "
                End If
                If certificazione <> 0 Then
                    xFiltroAggiuntivo += " And OTabelle_Parametri_certificazioni.Tabella_Par_Cod = " & certificazione.ToString & " "
                End If
                xOrderBy = " ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC, MovimentiCausaleAgg.Doc_Numero_Sin, MovimentiCausaleAgg.Doc_Numero, MovimentiCausaleAgg.Doc_Numero_Des, Movimenti_Dettagli.Extra_Str"
            Else
                ReDim Cau_MovArray(0)
                Cau_MovArray(0) = (CAU_ACCETTAZIONE_BENI_DA_DIVERSI)
                Cau_Agg = CAU_REGISTRAZIONI_TERZIARIA
                xSelectAggiuntiva += ", MovimentiCausaleAgg.Doc_Numero_Sin As MCauAgg_Doc_Numero_Sin , MovimentiCausaleAgg.Doc_Numero As MCauAgg_Doc_Numero, MovimentiCausaleAgg.Doc_Numero_Des As  MCauAgg_Doc_Numero_Des "
                If docNumeroSin <> "" Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero_Sin = '" & docNumeroSin & "'   "
                End If
                If docNumero <> 0 Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero = " & docNumero & "   "
                End If
                If docNumeroDes <> "" Then
                    xFiltroAggiuntivo += " AND MovimentiCausaleAgg.Doc_Numero_Des = '" & docNumeroDes & "'   "
                End If
                If nrRiga <> "" Then
                    xFiltroAggiuntivo += " And Movimenti_Dettagli.Extra_Str = '" & nrRiga & "' "
                End If
                If certificazione <> 0 Then
                    xFiltroAggiuntivo += " And OTabelle_Parametri_certificazioni.Tabella_Par_Cod = " & certificazione.ToString & " "
                End If
                xOrderBy = " ORDER BY Agenda.PIVA, Movimenti.Data_Movimento DESC, MovimentiCausaleAgg.Doc_Numero_Sin, MovimentiCausaleAgg.Doc_Numero, MovimentiCausaleAgg.Doc_Numero_Des, Movimenti_Dettagli.Extra_Str"
            End If

            Dim leggi As New AgronicaCoreContabBIZ.Movimenti_Dettagli_R
            r.RispostaStringa =
                leggi.MovimentiDettagli_Leggi_FF(objParametriAgenda.Piva, 0, "", 0, 0, 0, 210, 0, 0, 0, 0, lotto,
                                                 0, 0, LavCodArray, Cau_MovArray, Cau_Agg, FlagJollyInt,
                     xSelectAggiuntiva, "", xFiltroAggiuntivo, xOrderBy, fromOutToIn,
                     objParametri_Server)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
            AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function exportPOC(ByVal TrackCode As String,
                                     ByVal TipoLotto As String,
                                     ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean,
                                     ByVal FromOutToIn As Boolean,
                                     ByVal cCertificazione As Integer,
                                     ByVal CalCodSelected As Integer,
                                     ByVal IdMovDetSelected As Integer,
                                     ByVal Modalita As Integer,
                                     ByVal DestinazionePath As String,
                                     ByVal LinkWSEsterno As String,
                                     ByVal WSEsternoParametri As String) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Utenti As AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If


        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            r = TrackMeExportPOC(ForzaNuovaEsecuzioneAlgoritmoRintraccia,
                                 FromOutToIn,
                                 cCertificazione,
                                 CalCodSelected,
                                 IdMovDetSelected,
                                 TrackCode,
                                 objParametri_Server,
                                 objParametri_Utenti,
                                 TipoLotto, Modalita,
                                 DestinazionePath,
                                 LinkWSEsterno,
                                 WSEsternoParametri)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function track(ByVal TrackCode As String, ByVal TipoLotto As String, ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean, ByVal FromOutToIn As Boolean, ByVal cCertificazione As Integer, ByVal CalCodSelected As Integer, ByVal IdMovDetSelected As Integer) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Utenti As AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If


        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            'Lettura di dati dettaglio rintracciabilità
            Dim dtAgenda As DataTable =
                TrackMeToDataTable(ForzaNuovaEsecuzioneAlgoritmoRintraccia, FromOutToIn, cCertificazione, CalCodSelected, IdMovDetSelected, TrackCode, objParametri_Server, objParametri_Utenti, TipoLotto)

            HttpContext.Current.Session("dt") = dtAgenda

            r.RispostaOK = True

            r.Intestazione = OttieniDescrizioneProdotto_DaLotto(TrackCode, IdMovDetSelected, objParametri_Server)

            r.ImmagineProdotto = OttieniImmagineProdotto_DaLotto(TrackCode, objParametri_Server)



            If dtAgenda.Rows.Count > 0 Then
                r.RispostaStringa = DT_to_Json_Azienda(dtAgenda, "1", objParametri_Server, False, False)
            Else
                r.RispostaStringa = "Zero"
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function trackDiagram(ByVal TrackCode As String, ByVal TipoLotto As String, ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean, ByVal FromOutToIn As Boolean, ByVal cCertificazione As Integer, ByVal CalCodSelected As Integer, ByVal IdMovDetSelected As Integer) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim objParametri_Utenti As AgronicaCoreParametri
        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If


        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            'Lettura di dati dettaglio rintracciabilità
            Dim Diagram = New AgronicaCoreContabBIZ.trackDiagram

            Diagram = TrackMeToDiagram(ForzaNuovaEsecuzioneAlgoritmoRintraccia, FromOutToIn, cCertificazione, CalCodSelected, IdMovDetSelected, TrackCode, objParametri_Server, objParametri_Utenti, TipoLotto)

            'Diagram.EntityList = Diagram.EntityList.OrderBy(Function(x) x.Ordine).ToList()

            'HttpContext.Current.Session("dt") = dtAgenda

            '

            r.Intestazione = OttieniDescrizioneProdotto_DaLotto(TrackCode, IdMovDetSelected, objParametri_Server)

            r.ImmagineProdotto = OttieniImmagineProdotto_DaLotto(TrackCode, objParametri_Server)


            r.RispostaOK = True
            If Diagram.EntityList.Count > 0 Or Diagram.EntityLinkList.Count > 0 Then
                r.RispostaStringa = JsonConvert.SerializeObject(Diagram)
            Else
                r.RispostaStringa = "Zero"
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    Private Shared Function OttieniImmagineProdotto_DaLotto(ByVal TrackCode As String, ByVal objParametri_Server As AgronicaCoreParametri) As String

        Dim trk As New AgronicaCoreContabBIZ.FF_Track
        Dim rval As String = ""

        rval = trk.OttieniImmagineProdottoDatoLotto(TrackCode, objParametri_Server)

        If rval <> "" Then

            Dim xLeggiAllegatiRepository As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim dt As DataTable = xLeggiAllegatiRepository.Leggi(0, "GestioneAllegati_Repository", "", "", objParametri_Server)
            Dim dtStampe As DataTable = xLeggiAllegatiRepository.Leggi(0, "LinkAgronicaStampe", "", "", objParametri_Server)

            If dt.Rows.Count > 0 Then

                Dim xPath As String = dt.Rows(0)("valore")
                If xPath <> "" Then

                    Dim xDesFullFilePath As String =
                        AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(xPath) & "img_interfacciamenti\"

                    Dim vDesFileName As String() = rval.Split("\")
                    Dim xDesFileName As String = vDesFileName(vDesFileName.Length - 1)

                    Dim CopyErr As Boolean = False

                    Try

                        Dim folderExists As Boolean
                        folderExists = My.Computer.FileSystem.DirectoryExists(xDesFullFilePath)

                        If Not folderExists Then
                            My.Computer.FileSystem.CreateDirectory(xDesFullFilePath)
                        End If

                        My.Computer.FileSystem.CopyFile(rval, xDesFullFilePath & xDesFileName, True)

                    Catch ex As Exception
                        'CopyErr = True
                    End Try


                    If Not CopyErr Then

                        Dim LinkAgronicaStampe As String = ""
                        If dtStampe.Rows.Count > 0 Then
                            LinkAgronicaStampe = dtStampe.Rows(0)("Valore")
                            LinkAgronicaStampe = LinkAgronicaStampe.ToLower.Replace("gestionerichieste.aspx", "")
                        End If

                        rval = LinkAgronicaStampe & "File_Allegati/img_interfacciamenti/" & xDesFileName
                    Else
                        rval = ""
                    End If

                End If


            End If

            Return rval

        End If

    End Function



    Private Shared Function OttieniDescrizioneProdotto_DaLotto(ByVal TrackCode As String, ByVal IdMovDetSelected As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As String

        Dim trk As New AgronicaCoreContabBIZ.FF_Track
        Return trk.OttieniDescrizioneProdottoDatoLotto(TrackCode, IdMovDetSelected, objParametri_Server)

    End Function

    Private Shared Function TrackMeToDiagram(ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean, ByVal FromOutToIn As Boolean, ByVal cCertificazione As Integer, ByVal CalCodSelected As Integer, ByVal IdMovDetSelected As Integer, ByVal TrackCode As String, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_utenti As AgronicaCoreParametri, ByVal lTipoLotto As String) As AgronicaCoreContabBIZ.trackDiagram

        Dim xDtrack As XDocument
        Dim sD As String

        Dim objParametriAgenda As New ParametriAgenda
        Dim piva As String = objParametriAgenda.Piva

        Dim xForza As String = "0"
        If ForzaNuovaEsecuzioneAlgoritmoRintraccia Then
            xForza = "1"
        End If

        Dim xFromOutToIn As String = "0"
        If FromOutToIn Then
            xFromOutToIn = "1"
        End If

        sD = "<track><lotsToTrack><lot><inputData><lotto>" & TrackCode & "</lotto><tipolotto>" & lTipoLotto & "</tipolotto><ricarica>" & xForza & "</ricarica><xFromOutToIn>" & xFromOutToIn & "</xFromOutToIn><xCertificazione>" & cCertificazione & "</xCertificazione><xCalCodSelected>" & CalCodSelected & "</xCalCodSelected><xIdMovDetSelected>" & IdMovDetSelected & "</xIdMovDetSelected><xPiva>" & piva & "</xPiva></inputData></lot></lotsToTrack></track>"

        Dim dtTrKOperazioniConf As String

        Dim trk As New AgronicaCoreContabBIZ.FF_Track

        'Qui viene lanciato il ciclo di letture o il reperimento da db se già lanciata in precedenza
        dtTrKOperazioniConf = trk.TrackMe(sD, False, True, objParametri_Server)
        xDtrack = XDocument.Parse(dtTrKOperazioniConf)

        Dim lXDtrack As IEnumerable(Of XElement) = xDtrack.<track>.<lotsToTrack>.<lot>.<outputData>.<trasformazioni>.<trasformazione_dati>

        Dim FF_TrackedData_Cod As String =
            (From iAg In xDtrack.<track>.<lotsToTrack>.<lot>.<inputData>.<FF_TrackedData_Cod> Select iAg.Value).FirstOrDefault

        Dim xLetturaCache As New AgronicaCoreContabDAL.FF_TrackedData_Agenda_R
        Dim dtCacheCompleta As DataTable =
            xLetturaCache.LeggiXCache(FF_TrackedData_Cod, "", IIf(FromOutToIn = True, "T.Ordine Asc,", "T.Ordine Desc,") + "Data_Movimento Desc ,t.Cal_Cod Desc", objParametri_Server)

        'Dim StartRows As DataTable = xLetturaCache.LeggiXCacheSoloEntità(FF_TrackedData_Cod, FromOutToIn, "", IIf(FromOutToIn = True, "T.Ordine desc,", "T.Ordine asc,") + "Data_Movimento Desc,t.Cal_Cod Desc ", objParametri_Server)
        'Dim DetailRows As DataTable = xLetturaCache.LeggiXCacheSoloConnessioni(FF_TrackedData_Cod, FromOutToIn, "", IIf(FromOutToIn = True, "T.Ordine asc,", "T.Ordine desc,") + "Data_Movimento Desc,t.Cal_Cod Desc ", objParametri_Server)


        Dim diagram As New AgronicaCoreContabBIZ.trackDiagram
        Dim SeqObj As Integer = 1
        Dim OrdineLayer As Integer = 0
        Dim X As Integer = 15
        Dim Y As Integer = 15
        Dim lav_cod As Integer = 0
        Dim preparazione_cod As Integer = 0


        For Each row In dtCacheCompleta.Rows
            Dim dataDoc = IIf(row("Data_Documento") = AGRODATAINIZIO, FormatDateTime(row("Data_Documento_Fornitore"), Microsoft.VisualBasic.DateFormat.ShortDate), FormatDateTime(row("Data_Documento"), Microsoft.VisualBasic.DateFormat.ShortDate))
            If dataDoc = AGRODATAINIZIO Then
                dataDoc = ""
            End If

            If lav_cod <> row("lav_cod") Or preparazione_cod <> row("lav_cod") Then
                lav_cod = row("lav_cod")
                preparazione_cod = row("lav_cod")
                OrdineLayer += 1
                Y += 60
                X = 15
            End If

            If row("Visibile") = 1 Then

                ' aggiungo l'entità del grafo
                diagram.EntityList.Add(New AgronicaCoreContabBIZ.TrackDiagramEntity() With {
                    .ID = SeqObj,
                    .id_mov_det = row("id_mov_det"),
                    .Name = row("lav_des"),
                    .CalCod = IIf(FromOutToIn = True, row("Cal_Cod"), row("Cal_Cod_Padre")),
                    .CalCodPadre = IIf(FromOutToIn = True, row("Cal_Cod_Padre"), row("Cal_Cod")),
                    .Lotto = row("Lotto"),
                    .DataDocumento = dataDoc,
                    .NumeroDocumento = IIf(row("Numero_Documento") = "", row("Numero_Documento_Fornitore"), row("Numero_Documento")),
                    .IntestatarioDocumento = row("Intestatario_Documento_Ragione_Sociale"),
                    .StartChainEntity = True,
                    .QtyDoc = row("Qta_Extra_Totale"),
                    .OrdineLayer = OrdineLayer,
                    .X = X,
                    .Y = Y
                })
                SeqObj += 1

                ' per ogni elemento verifico se il cal_cod punta ad altri Cal_Cod_Padre
                If FromOutToIn Then
                    If row("Cal_Cod_Padre") <> 0 Then
                        Dim objToLink = diagram.EntityList.Where(Function(z) z.CalCod = row("Cal_Cod_Padre")).ToList()
                        For Each obj In objToLink
                            If (diagram.EntityLinkList.Any(Function(z) z.StartLink = row("id_mov_det") And z.EndLink = obj.id_mov_det) = False) And
                               (diagram.EntityLinkList.Any(Function(z) z.EndLink = row("id_mov_det") And z.StartLink = obj.id_mov_det) = False) Then
                                diagram.EntityLinkList.Add(New AgronicaCoreContabBIZ.TrackDiagramLinkEntity() With {
                                            .StartLink = row("id_mov_det"),
                                            .EndLink = obj.id_mov_det,
                                            .Qty = row("Qta_Extra_Totale"),
                                            .UM = row("UDM_SIM")
                                            })

                            End If
                        Next
                    End If
                Else
                    If row("Cal_Cod") <> 0 Then
                        Dim objToLink = diagram.EntityList.Where(Function(z) z.CalCodPadre = row("Cal_Cod")).ToList()
                        For Each obj In objToLink
                            If (diagram.EntityLinkList.Any(Function(z) z.StartLink = row("id_mov_det") And z.EndLink = obj.id_mov_det) = False) And
                           (diagram.EntityLinkList.Any(Function(z) z.EndLink = row("id_mov_det") And z.StartLink = obj.id_mov_det) = False) Then
                                diagram.EntityLinkList.Add(New AgronicaCoreContabBIZ.TrackDiagramLinkEntity() With {
                                        .StartLink = row("id_mov_det"),
                                        .EndLink = obj.id_mov_det,
                                        .Qty = row("Qta_Extra_Totale"),
                                        .UM = row("UDM_SIM")
                                        })

                            End If
                        Next
                    End If
                End If
                X += 210
            End If
        Next






        ''1- creo le entità del grafo
        'For Each row In StartRows.Rows

        '    Dim dataDoc = IIf(row("Data_Documento") = AGRODATAINIZIO, FormatDateTime(row("Data_Documento_Fornitore"), Microsoft.VisualBasic.DateFormat.ShortDate), FormatDateTime(row("Data_Documento"), Microsoft.VisualBasic.DateFormat.ShortDate))
        '    If dataDoc = AGRODATAINIZIO Then
        '        dataDoc = ""
        '    End If

        '    If row("Visibile") = 1 Then
        '        Dim cal_cod_ori As Integer = -1
        '        If FromOutToIn Then
        '            cal_cod_ori = row("Cal_Cod")
        '        Else
        '            cal_cod_ori = row("Cal_Cod_Padre")
        '        End If
        '        diagram.EntityList.Add(New AgronicaCoreContabBIZ.TrackDiagramEntity() With {
        '            .ID = SeqObj,
        '            .id_mov_det = row("id_mov_det"),
        '            .Name = row("lav_des"),
        '            .CalCod = IIf(FromOutToIn = True, row("Cal_Cod"), row("Cal_Cod_Padre")),
        '            .Lotto = row("Lotto"),
        '            .DataDocumento = dataDoc,
        '            .NumeroDocumento = IIf(row("Numero_Documento") = "", row("Numero_Documento_Fornitore"), row("Numero_Documento")),
        '            .IntestatarioDocumento = row("Intestatario_Documento_Ragione_Sociale"),
        '            .StartChainEntity = True,
        '            .QtyDoc = row("Qta_Extra_Totale"),
        '            .Ordine = row("Ordine")
        '            })
        '        SeqObj += 1
        '        If DetailRows.Rows.Count > 0 Then
        '            Descend(diagram.EntityList, diagram.EntityLinkList, DetailRows, IIf(FromOutToIn = True, row("Cal_Cod"), row("Cal_Cod_Padre")), row("id_mov_det"), FromOutToIn, SeqObj)
        '        End If
        '    End If
        'Next

        'fase 2 - rimappo gli id_mov_det con gli idobj
        Dim id_obj As Integer
        For Each itm In diagram.EntityLinkList
            'If FromOutToIn = True Then
            '    itm.Qty = diagram.EntityList.Find(Function(x) x.id_mov_det = itm.StartLink).QtyDoc
            'Else
            '    itm.Qty = diagram.EntityList.Find(Function(x) x.id_mov_det = itm.EndLink).QtyDoc
            'End If
            id_obj = diagram.EntityList.Find(Function(z) z.id_mov_det = itm.StartLink).ID
            itm.StartLink = id_obj
            If itm.EndLink <> 0 Then
                id_obj = diagram.EntityList.Find(Function(z) z.id_mov_det = itm.EndLink).ID
                itm.EndLink = id_obj
            End If

        Next


        Return diagram
    End Function

    Private Shared Sub Descend(ByRef EntityList As List(Of AgronicaCoreContabBIZ.TrackDiagramEntity), ByRef EntityLinkList As List(Of AgronicaCoreContabBIZ.TrackDiagramLinkEntity), ByVal DetailRows As DataTable, StartCalCod As Integer, ByVal id_mov_det As Integer, ByVal FromOutToIn As Boolean, ByRef SeqObj As Integer)
        Dim dtChilds = New DataTable

        If FromOutToIn Then
            If DetailRows.Select("Cal_Cod_Padre=" + StartCalCod.ToString()).Count > 0 Then
                dtChilds = DetailRows.Select("Cal_Cod_Padre=" + StartCalCod.ToString()).CopyToDataTable()
            End If
        Else
            If DetailRows.Select("Cal_Cod=" + StartCalCod.ToString()).Count > 0 Then
                dtChilds = DetailRows.Select("Cal_Cod=" + StartCalCod.ToString()).CopyToDataTable()
            End If
        End If
        If dtChilds.Rows.Count <= 0 Then
            'non ha legami
        Else
            For Each row In dtChilds.Rows
                If row("Visibile") = 1 Then
                    Dim cnt = 0
                    If FromOutToIn = True Then
                        cnt = EntityList.Where(Function(x) x.id_mov_det = row("id_mov_det") And x.CalCod = row("Cal_Cod")).Count
                    Else
                        cnt = EntityList.Where(Function(x) x.id_mov_det = row("id_mov_det") And x.CalCod = row("Cal_Cod_Padre")).Count
                    End If
                    If cnt <= 0 Then
                        Dim dataDoc = IIf(row("Data_Documento") = AGRODATAINIZIO, FormatDateTime(row("Data_Documento_Fornitore"), Microsoft.VisualBasic.DateFormat.ShortDate), FormatDateTime(row("Data_Documento"), Microsoft.VisualBasic.DateFormat.ShortDate))
                        If dataDoc = AGRODATAINIZIO Then
                            dataDoc = ""
                        End If
                        EntityList.Add(New AgronicaCoreContabBIZ.TrackDiagramEntity() With {
                                    .ID = SeqObj,
                                    .id_mov_det = row("id_mov_det"),
                                    .Name = row("lav_des"),
                                    .CalCod = IIf(FromOutToIn = True, row("Cal_Cod"), row("Cal_Cod_Padre")),
                                    .Lotto = row("Lotto"),
                                    .DataDocumento = dataDoc,
                                    .NumeroDocumento = IIf(row("Numero_Documento") = "", row("Numero_Documento_Fornitore"), row("Numero_Documento")),
                                    .IntestatarioDocumento = row("Intestatario_Documento_Ragione_Sociale"),
                                    .StartChainEntity = False,
                                    .QtyDoc = row("Qta_Extra_Totale"),
                                    .OrdineLayer = row("Ordine")
                                    })
                        SeqObj += 1
                    End If

                    Descend(EntityList, EntityLinkList, DetailRows, IIf(FromOutToIn = True, row("Cal_Cod"), row("Cal_Cod_Padre")), row("id_mov_det"), FromOutToIn, SeqObj)
                    If EntityLinkList.Where(Function(x) x.StartLink = id_mov_det And x.EndLink = row("id_mov_det")).Count <= 0 Then
                        EntityLinkList.Add(New AgronicaCoreContabBIZ.TrackDiagramLinkEntity() With {
                                        .StartLink = id_mov_det,
                                        .EndLink = row("id_mov_det"),
                                        .Qty = 0,
                                        .UM = row("UDM_SIM")
                                        })
                    End If
                Else
                    Descend(EntityList, EntityLinkList, DetailRows, IIf(FromOutToIn = True, row("Cal_Cod"), row("Cal_Cod_Padre")), id_mov_det, FromOutToIn, SeqObj)
                End If

            Next
        End If
    End Sub

    Private Shared Function TrackMeExportPOC(ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean,
                                             ByVal FromOutToIn As Boolean,
                                             ByVal cCertificazione As Integer,
                                             ByVal CalCodSelected As Integer,
                                             ByVal IdMovDetSelected As Integer,
                                             ByVal TrackCode As String,
                                             ByVal objParametri_Server As AgronicaCoreParametri,
                                             ByVal objParametri_utenti As AgronicaCoreParametri,
                                             ByVal lTipoLotto As String,
                                             ByVal Modalita As Integer,
                                             ByVal DestinazionePath As String,
                                             ByVal LinkWSEsterno As String,
                                             ByVal WSEsternoParametri As String) As MenuBS_Risposta
        Dim xDtrack As XDocument
        Dim sD As String
        Dim ret As MenuBS_Risposta = New MenuBS_Risposta()

        Dim objParametriAgenda As New ParametriAgenda
        Dim piva As String = objParametriAgenda.Piva

        Dim xForza As String = "0"
        If ForzaNuovaEsecuzioneAlgoritmoRintraccia Then
            xForza = "1"
        End If

        Dim xFromOutToIn As String = "0"
        If FromOutToIn Then
            xFromOutToIn = "1"
        End If

        sD = "<track><lotsToTrack><lot><inputData><lotto>" & TrackCode & "</lotto><tipolotto>" & lTipoLotto & "</tipolotto><ricarica>" & xForza & "</ricarica><xFromOutToIn>" & xFromOutToIn & "</xFromOutToIn><xCertificazione>" & cCertificazione & "</xCertificazione><xCalCodSelected>" & CalCodSelected & "</xCalCodSelected><xIdMovDetSelected>" & IdMovDetSelected & "</xIdMovDetSelected><xPiva>" & piva & "</xPiva></inputData></lot></lotsToTrack></track>"

        Dim dtTrKOperazioniConf As String

        Dim trk As New AgronicaCoreContabBIZ.FF_Track

        'Qui viene lanciato il ciclo di letture o il reperimento da db se già lanciata in precedenza
        dtTrKOperazioniConf = trk.TrackMe(sD, True, False, objParametri_Server, True, True, True)

        xDtrack = XDocument.Parse(dtTrKOperazioniConf)

        Dim lXDtrack As IEnumerable(Of XElement) = xDtrack.<track>.<lotsToTrack>.<lot>.<outputData>.<trasformazioni>.<trasformazione_dati>

        Dim pathToExpFile As String = "C:\GIASLAN\File_Esportazioni\pocAntares"

        Select Case Modalita
            Case 0
                'export su file
                If DestinazionePath <> "" Then
                    pathToExpFile = DestinazionePath
                End If

                If IO.Directory.Exists(pathToExpFile) = False Then
                    IO.Directory.CreateDirectory(pathToExpFile)
                End If

                Try
                    Dim filename As String = (TrackCode + "_" + DateTime.Now().ToString("yyyyMMddHHmmss") + ".xml").Replace("/", "")

                    Dim xmlSettings = New System.Xml.XmlWriterSettings()
                    xmlSettings.Encoding = New UTF8Encoding(False)

                    Using w As System.Xml.XmlWriter = System.Xml.XmlWriter.Create(System.IO.Path.Combine(pathToExpFile, filename), xmlSettings)
                        xDtrack.Save(w)
                    End Using

                    Dim rCfgSiti = New AgronicaCoreVarieDAL.Configurazione_Siti_R
                    Dim EnableUploadFTP As Boolean = Boolean.Parse(rCfgSiti.Leggi_Valore(0, "Esporta_Tracciabilita_FTPMode", "", "", objParametri_Server))
                    If EnableUploadFTP = True Then
                        Dim ftp = New AgronicaCoreUtility.FTPHelper
                        Dim ftpUrl As String = rCfgSiti.Leggi_Valore(0, "Esporta_Tracciabilita_FTPUrl", "", "", objParametri_Server)
                        Dim ftpUser As String = rCfgSiti.Leggi_Valore(0, "Esporta_Tracciabilita_FTPUsername", "", "", objParametri_Server)
                        Dim ftpPass As String = rCfgSiti.Leggi_Valore(0, "Esporta_Tracciabilita_FTPPassword", "", "", objParametri_Server)

                        Dim ReturnCodeDescr As String = ""

                        If ftp.UploadFile(System.IO.Path.Combine(pathToExpFile, filename), ftpUrl, ftpUser, ftpPass, ReturnCodeDescr) = True Then
                            ret.RispostaOK = True
                            ret.RispostaStringa = "Esportato file " + filename + " e caricato su FTP"
                        Else
                            ret.RispostaOK = False
                            ret.RispostaStringa = "Errore durante l'upload del file su FTP: " & ReturnCodeDescr
                        End If

                    Else
                        ret.RispostaOK = True
                        ret.RispostaStringa = "Esportato file " + filename
                    End If
                Catch ex As Exception
                    ret.RispostaOK = False
                    ret.RispostaStringa = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
                End Try


            Case 1
                'invio a web service
                ret.RispostaOK = False
                ret.RispostaStringa = "Not implemented..."
        End Select

        Return ret

    End Function

    Private Shared Function TrackMeToDataTable(ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean, ByVal FromOutToIn As Boolean, ByVal cCertificazione As Integer, ByVal CalCodSelected As Integer, ByVal IdMovDetSelected As Integer, ByVal TrackCode As String, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_utenti As AgronicaCoreParametri, ByVal lTipoLotto As String) As DataTable
        Dim xDtrack As XDocument
        Dim sD As String

        Dim objParametriAgenda As New ParametriAgenda
        Dim piva As String = objParametriAgenda.Piva

        Dim xForza As String = "0"
        If ForzaNuovaEsecuzioneAlgoritmoRintraccia Then
            xForza = "1"
        End If

        Dim xFromOutToIn As String = "0"
        If FromOutToIn Then
            xFromOutToIn = "1"
        End If

        sD = "<track><lotsToTrack><lot><inputData><lotto>" & TrackCode & "</lotto><tipolotto>" & lTipoLotto & "</tipolotto><ricarica>" & xForza & "</ricarica><xFromOutToIn>" & xFromOutToIn & "</xFromOutToIn><xCertificazione>" & cCertificazione & "</xCertificazione><xCalCodSelected>" & CalCodSelected & "</xCalCodSelected><xIdMovDetSelected>" & IdMovDetSelected & "</xIdMovDetSelected><xPiva>" & piva & "</xPiva></inputData></lot></lotsToTrack></track>"

        Dim dtTrKOperazioniConf As String

        Dim trk As New AgronicaCoreContabBIZ.FF_Track

        'Qui viene lanciato il ciclo di letture o il reperimento da db se già lanciata in precedenza
        dtTrKOperazioniConf = trk.TrackMe(sD, False, False, objParametri_Server)

        xDtrack = XDocument.Parse(dtTrKOperazioniConf)

        '2b. per ciascun operazione leggo i costi scaricati


        Dim lXDtrack As IEnumerable(Of XElement) = xDtrack.<track>.<lotsToTrack>.<lot>.<outputData>.<trasformazioni>.<trasformazione_dati>



        Dim FF_TrackedData_Cod As String =
            (From iAg In xDtrack.<track>.<lotsToTrack>.<lot>.<inputData>.<FF_TrackedData_Cod> Select iAg.Value).FirstOrDefault




        Dim sa_cod As Integer = 0
        Dim validita_inizio As DateTime = CostantiPersonalizzate.AGRODATAINIZIO
        Dim validita_fine As DateTime = CostantiPersonalizzate.AGRODATAFINE
        Dim veg_cod As Integer = 0
        Dim cul_cod As Integer = 0
        Dim tipo As String = ""
        Dim gru_cod As Integer = 0
        Dim lav_cod As Integer = 0

        Dim flag_TerrenoNudo As Boolean = False

        Dim FiltroLavorazioni As String
        Dim Filtro_Tipo_GruppoOperazioni As String

        Dim sData_Selezionata As String
        Dim Data_Selezionata As DateTime

        Dim dtRval As DataTable = MenuBS_Lavorazioni.Carica_Lavorazioni(
            piva,
            Sa_Cod:=sa_cod,
            DataDa:=AGRODATAINIZIO,
            DataA:=AGRODATAFINE,
            Veg_Cod:=veg_cod,
            Cul_Cod:=cul_cod,
            Tipo:=tipo,
            Gru_Cod:=gru_cod,
            Lav_Cod:=lav_cod,
            Flag_TerrenoNudo:=flag_TerrenoNudo,
            xFiltroAggiuntivo_colturali:="",
            xFiltroAggiuntivo_postRaccolta:="",
            xFiltroAggiuntivo_contabili:="",
            xFiltroAggiuntivo_contabili_Macchine:="",
            xFiltroAggiuntivo_contabili_Audit:="",
            xOrderBy:="",
            objparametri_Server:=objParametri_Server,
            objparametri_Utenti:=objParametri_utenti,
            FF_TrackedData_Cod:=FF_TrackedData_Cod, FromOutToIn:=FromOutToIn, cCertificazione:=cCertificazione, righeAggiunte:=""
        )

        'Solo se è stata scelta una certificazione e solo se è stata scelta una riga di vendita ...
        If FF_TrackedData_Cod > 0 And FromOutToIn And IdMovDetSelected <> 0 Then
            If cCertificazione <> 0 Then
                Dim DvCertificazione As New DataView
                Dim DtCertificate As New DataTable
                DvCertificazione.Table = dtRval
                DvCertificazione.RowFilter = "FF_certificazioni_Codice = " & cCertificazione & ""
                DtCertificate = DvCertificazione.ToTable

                Dim dtRigheAggiuntive = ControllaCertificazione(ForzaNuovaEsecuzioneAlgoritmoRintraccia, piva, sa_cod, IdMovDetSelected,
                     dtRval, DtCertificate, cCertificazione, objParametri_Server, objParametri_utenti)
                If Not dtRigheAggiuntive Is Nothing Then
                    For Each riga In dtRigheAggiuntive.Rows()
                        dtRval.ImportRow(riga)
                    Next
                End If
            End If
        End If

        Return dtRval

    End Function


    Private Function getTrackWS_URL() As String

        Dim agrowebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim rval As String = agrowebConfig.LinkAgronicaAgenda2010

        rval = "http://localhost/" & rval.ToLower.Replace("gestionerichieste.aspx", "") & "/Track/Track.asmx"

        Return rval

    End Function

    Private Function Get_Str_Credenziali_WS(
                ByVal objparametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                ByVal objparametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As String


        Dim xCredenziali As New AgronicaCoreXML.XML_WS_Importa_Gias

        Return _
            xCredenziali.Genera_Stringa_Credenziali(
                True,
                Nothing,
                objparametri_server.SuperUserUsername,
                "",
                objparametri_server.PivaSuperUser,
                False, "", "", "", "", "", "",
                objparametri_server.StringaConnessione,
                objparametri_utenti.StringaConnessione)

    End Function



#End Region


#Region "recupero Operazioni agenda da filtro"




    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaOperazioniDettagliDestinazioni(ByVal filtro As String, RaggruppaPerCampo As Boolean) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda = New ParametriAgenda

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If


        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)

            Dim sa_cod As Integer = 0
            Dim veg_cod As Integer = 0
            Dim id_cod As Integer = 0

            Dim sData_Selezionata1 As String
            Dim Data_Selezionata1 As DateTime

            Dim sData_Selezionata2 As String
            Dim Data_Selezionata2 As DateTime

            Dim FiltroImpianti As String = ""



            If IsNumeric(jSonDatiTESTATA("sa_cod")) Then
                sa_cod = jSonDatiTESTATA("sa_cod")
            End If

            If IsNumeric(jSonDatiTESTATA("veg_cod")) Then
                veg_cod = jSonDatiTESTATA("veg_cod")
            ElseIf (jSonDatiTESTATA("veg_cod").ToString.Contains("/")) Then
                If IsNumeric(jSonDatiTESTATA("veg_cod").ToString.Split("/")(1)) Then
                    id_cod = jSonDatiTESTATA("veg_cod").ToString.Split("/")(1)
                End If
            End If

            sData_Selezionata1 = jSonDatiTESTATA("txt_Data1").ToString
            Data_Selezionata1 = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

            sData_Selezionata2 = jSonDatiTESTATA("txt_Data2").ToString
            Data_Selezionata2 = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)


            Dim impianti As New List(Of String)
            If jSonDatiTESTATA("impianti") IsNot Nothing AndAlso jSonDatiTESTATA("impianti").ToString <> "" Then
                Dim arr = JArray.Parse(jSonDatiTESTATA("impianti").ToString)
                For Each elem In arr
                    Dim v1 As String() = elem.ToString.Split("_")
                    Dim toAdd As String = " ( d.piva = '" & Agro_SQL_SaveText(v1(0)) & "' and d.sa_cod = " & v1(1) & " and d.appezza = " & v1(2) & " and d.id_Destinazione = " & v1(3) & " )"
                    impianti.Add(toAdd)
                Next

                If impianti.Count > 0 Then
                    FiltroImpianti = "AND (" & String.Join(" or ", impianti) & ") "
                End If


            End If

            Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)

            Dim stbAgea2015 As New StringBuilder
            stbAgea2015.AppendLine("--")


            Dim CodificaAgeaDaGias_livelloDettaglio As New List(Of enum_CodificaAgeaDaGias_livelloDettaglio)
            CodificaAgeaDaGias_livelloDettaglio.Add(enum_CodificaAgeaDaGias_livelloDettaglio.SpecieVegetale)
            CodificaAgeaDaGias_livelloDettaglio.Add(enum_CodificaAgeaDaGias_livelloDettaglio.CodiciAnagrafe)

            Dim SostiuisciSpecieSpecificaConSpecieGenerica As Boolean = True

            Dim leggiCodificheAgea As New AgronicaCoreMetaSchemaDAL.Agea_Codifiche_R
            leggiCodificheAgea.LeggiCodificheSuImpiantiOppurePlanning(
                stb:=stbAgea2015,
                piva:="",
                FiltroImpiantiConAND:="",
                Veg_Cod:=0,
                Cul_Cod:=0,
                Grfi_Cod:=0,
                Grva_Cod:=0,
                Metodo_Produzione_Cod:=0,
                Reg_Cod:=0,
                Id_Cod:=0,
                Grsp_Cod:=0,
                Cul_Cod_Agea:="",
                Uso_Cod_Agea:="",
                Occupazione_Cod_Agea:="",
                Destinazione_Cod_Agea:="",
                CodificaAgeaDaGias_livelloDettaglio:=CodificaAgeaDaGias_livelloDettaglio,
                SostiuisciSpecieSpecificaConSpecieGenerica:=SostiuisciSpecieSpecificaConSpecieGenerica,
                Qualita_Cod_Agea:="",
                xFiltroAggiuntivo:=" Veg_Cod_Agea <> '000' ",
                xOrderBy:="",
                objParametri:=objParametri_Server
            )

            Dim LetturaDettagli As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim dtGriglia As DataTable =
                LetturaDettagli.Leggi_DestinazioneConInformazioniRiepilogative(
                RaggruppaPerCampo:=RaggruppaPerCampo,
                piva:=objParametriAgenda.Piva,
                sa_Cod:=sa_cod,
                veg_cod:=veg_cod,
                id_Cod:=id_cod,
                validita_inizio:=Data_Selezionata1,
                validita_fine:=Data_Selezionata2,
                FiltroImpianti:=FiltroImpianti,
                NomeDB_Utenti:=NomeDB_Utenti,
                stbAgea2015,
                xFiltroAggiuntivo:="",
                xOrderBy:="",
                objParametri:=objParametri_Server
            )

            Leggi_DestinazioneConInformazioniRiepilogativeElaboraDT(dtGriglia, objParametri_Server, objParametri_Utenti)

            r.RispostaOK = True

            If dtGriglia.Rows.Count > 0 Then
                r.RispostaStringa = DT_to_Json_OperazioniDestinazioni(dtGriglia, objParametri_Server)
            Else
                r.RispostaStringa = "Zero"
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function


    Private Shared Sub Leggi_DestinazioneConInformazioniRiepilogativeElaboraDT(dt As DataTable, objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri)

        Dim ListaIDAgenda As New List(Of String)
        Dim piva As String = dt(0)("piva")
        For Each r In dt.Rows
            If Not ListaIDAgenda.Contains(r("ID_Agenda")) Then
                ListaIDAgenda.Add(r("ID_Agenda"))
            End If
        Next

        Dim objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
        Dim objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
        FasiFenoListaDescrizioni(dt, objParametri_Server, objParametri_Utenti, objParametriUscitaFasiNew, objParametriUscitaFasiOld)

        Dim ListaDescrizioni As List(Of KeyValuePair(Of Integer, String)) =
            MenuBS_Lavorazioni.getStrCostiMacchineOperatoriViaIDAgenda(piva, ListaIDAgenda, objParametri_Server)

        For Each r In dt.Rows

            Dim m1 As KeyValuePair(Of Integer, String)
            Dim m As String()
            m1 = (
                From l In ListaDescrizioni
                Where l.Key = r("ID_Agenda")
                Select l
            ).FirstOrDefault

            If Not m1.Value Is Nothing Then
                m = m1.Value.Split("|")
                r("Macchina_Trattore_codice") = m(1)
                r("Operatore") = m(0)
            End If

            FaseFenoDescrizioneSuDataTable(objParametriUscitaFasiNew, objParametriUscitaFasiOld, r)

        Next


    End Sub

    Private Shared Sub FasiFenoListaDescrizioni(
            dt As DataTable,
            objParametri_Server As AgronicaCoreParametri,
            objParametri_Utenti As AgronicaCoreParametri,
            ByRef objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output,
            ByRef objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
    )

        Dim listaFasiFenoOld As New List(Of Integer)
        Dim listaFasiFenoNew As New List(Of Integer)

        For Each r In dt.Rows
            Dim ff1 As Integer = r("Fase_Cod_Corrente")

            If ff1 > 1000 Then
                If Not listaFasiFenoNew.Contains(ff1) Then
                    listaFasiFenoNew.Add(ff1)
                End If

            Else
                If ff1 > 0 Then
                    If Not listaFasiFenoOld.Contains(ff1) Then
                        listaFasiFenoOld.Add(ff1)
                    End If
                End If
            End If
        Next

        Dim filtroFasiFenoOld As String = String.Join(",", listaFasiFenoOld)
        Dim filtroFasiFenoNew As String = String.Join(",", listaFasiFenoNew)

        Dim objParametriIngressoNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input = FFParametriIngresso(objParametri_Server, objParametri_Utenti)
        Dim objParametriIngressoOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input = FFParametriIngresso(objParametri_Server, objParametri_Utenti)

        Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS

        If filtroFasiFenoOld <> "" Then
            objParametriIngressoOld.strFiltro = "fs.Cod_SS in (" & filtroFasiFenoOld & ")"
            objParametriUscitaFasiOld = objFasi_WS.FasiFenologiche_OLD(objParametriIngressoOld)
        End If


        If filtroFasiFenoNew <> "" Then
            objParametriIngressoNew.strFiltro = "ss.Cod_SS in (" & filtroFasiFenoNew & ")"
            objParametriUscitaFasiNew = objFasi_WS.FasiFenologiche(objParametriIngressoNew)
        End If

    End Sub

    Private Shared Sub FaseFenoDescrizioneSuDataTable(objParametriUscitaFasiNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output, objParametriUscitaFasiOld As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output, r As DataRow)
        Dim Fase_Des_Corrente As String = ""
        Dim Fase_Cod_Corrente As Integer = r("Fase_Cod_Corrente")
        Fase_Des_Corrente = ""
        Select Case Fase_Cod_Corrente

            Case = 0

            Case < 1000 'caso vecchio av_cod = ff_cod
                Fase_Des_Corrente = (From aa In objParametriUscitaFasiOld.ListaFasiFenologiche
                                     Where aa.FF_Cod = Fase_Cod_Corrente
                                     Select aa.Descrizione
                                    ).FirstOrDefault

            Case Else ' caso nuovo av_cod= cod_css
                Fase_Des_Corrente = (From aa In objParametriUscitaFasiNew.ListaFasiFenologiche
                                     Where aa.Cod_SS = Fase_Cod_Corrente
                                     Select aa.Descrizione & " - BBCH " & aa.Stadio
                                    ).FirstOrDefault
        End Select
        r("FaseFenologica") = Fase_Des_Corrente
    End Sub

    Private Shared Function FFParametriIngresso(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
        Dim objParametriIngressoNew As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
        objParametriIngressoNew.Veg_Cod = 0
        objParametriIngressoNew.Lingua_Cod = objParametri_Server.Lingua_Cod

        Dim Leggi_impostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim imp As String = Leggi_impostazioni.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.SUPERUSER_COD_PERSON_RILIEVO_FASI_FENOLOGICHE, objParametri_Utenti, 2)
        If imp = "1" Then
            objParametriIngressoNew.Personalizzate = True
        End If

        Return objParametriIngressoNew
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaOperazioni(ByVal filtro As String) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda = New ParametriAgenda

        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If

        Lingua.Gias_InizializzaCultura_DaSession()

        Try

            Dim Mov_Biz As AgronicaCoreContabBIZ.Movimenti_R = New AgronicaCoreContabBIZ.Movimenti_R()
            r = Mov_Biz.CaricaOperazioni(filtro, objParametriAgenda.Piva, objParametri_Server, objParametri_Utenti)

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    Private Shared Sub caricaInSessioneImpostazioniutente()

        HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") = Leggi_Filtro_Tipo_GruppoOperazioni()

        HttpContext.Current.Session("Filtro_Utente_Lavorazioni") = Leggi_Filtro_Utente_Lavorazioni()

    End Sub

    Private Shared Function Leggi_Filtro_Tipo_GruppoOperazioni() As String
        'FILTRO GRUPPO OPERAZIONI
        Dim Filtro_Tipo_GruppoOperazioni As String = ""
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable = objUtentiImpostazioni.Leggi_Utente_Poi_SuperUser(
                                            enum_Impostazioni_Utenti.UTENTE_OPERAZIONI_TIPI_GRUPPI_OPERAZIONI_VISIBILI_MENU_AGENDA, 1,
                                            AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))

        If DT.Rows.Count > 0 Then

            If Not IsDBNull(DT.Rows(0).Item("Impostazione_Valore_1")) AndAlso DT.Rows(0).Item("Impostazione_Valore_1") <> "" Then
                Dim tipi As String = DT.Rows(0).Item("Impostazione_Valore_1")
                Dim tipis() As String = tipi.Split("|")

                For i = 0 To tipis.Count - 1
                    If i > 0 Then
                        Filtro_Tipo_GruppoOperazioni &= " OR "
                    End If

                    Select Case (tipis(i))
                        Case "C", "E", "Z", "P", "V"
                            Filtro_Tipo_GruppoOperazioni &= " GruppoOperazioni.TIPO = '" & tipis(i) & "' "
                        Case "E6"
                            Filtro_Tipo_GruppoOperazioni &= " ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 6 ) "
                        Case "E10"
                            Filtro_Tipo_GruppoOperazioni &= " ( GruppoOperazioni.TIPO = 'E' AND GruppoOperazioni.GRU_COD = 10 ) "

                    End Select

                Next

                HttpContext.Current.Session("Filtro_Tipo_GruppoOperazioni") = Filtro_Tipo_GruppoOperazioni
            End If

        End If

        Return Filtro_Tipo_GruppoOperazioni

    End Function

    Private Shared Function Leggi_Filtro_Utente_Lavorazioni() As String

        'FILTRO LAVORAZIONI
        Dim Filtro_Utente_Lavorazioni As String = ""
        Dim objUtentiImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DT As DataTable = objUtentiImpostazioni.Leggi(
                                        enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_LAVORAZIONI, 1,
                                        AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                        "", "", HttpContext.Current.Session("ASG_objParametri_Utenti"))

        If DT.Rows.Count > 0 Then
            Filtro_Utente_Lavorazioni = " agenda.Lav_Cod in ("
            For i = 0 To DT.Rows.Count - 1
                If i <> 0 Then
                    Filtro_Utente_Lavorazioni &= " ,"
                End If
                Filtro_Utente_Lavorazioni &= DT.Rows(i).Item("ID_0")
            Next
            Filtro_Utente_Lavorazioni &= " )  "

        End If

        Return Filtro_Utente_Lavorazioni

    End Function

    Private Shared Sub EstendiDatatable(ByRef dt As DataTable)

        If Not dt.Columns.Contains("chiave_composita") Then
            dt.Columns.Add("Centro_Aziendale", GetType(String))
            dt.Columns.Add("Specie", GetType(String))
            dt.Columns.Add("Appezzamenti_Coinvolti", GetType(String))
            dt.Columns.Add("Prodotti_Utilizzati", GetType(String))
            dt.Columns.Add("Avversita", GetType(String))

            dt.Columns.Add("chiave_composita", GetType(String))
            Dim delim As String() = New String(0) {"<br>"}

            For i = 0 To dt.Rows.Count - 1

                If Not IsDBNull(dt.Rows(i).Item("Dettagli")) Then

                    Dim dettagli = dt.Rows(i).Item("Dettagli")
                    Dim dett() As String = dettagli.Split(delim, StringSplitOptions.None)

                    For count = 0 To dett.Length - 1

                        If InStr(dett(count), ":") Then

                            Dim dett2 As String() = dett(count).Split(":")

                            If dett(count) <> "" Then

                                Dim final_s As String = dett2(1).Replace("</b> ", "")

                                Select Case dett2(0)
                                    Case "<b>Centro Az."
                                        dt.Rows(i).Item("Centro_Aziendale") = final_s
                                    Case "<b>Specie"
                                        dt.Rows(i).Item("Specie") = final_s
                                    Case "<b>Appezzamenti Coinvolti"
                                        dt.Rows(i).Item("Appezzamenti_Coinvolti") = final_s
                                    Case "<b>Prodotti Utilizzati"
                                        dt.Rows(i).Item("Prodotti_Utilizzati") = final_s
                                    Case " <b> Avversità"
                                        final_s = final_s.Replace("-", "")
                                        dt.Rows(i).Item("Avversita") = final_s
                                End Select

                            End If

                        End If
                    Next
                End If

                dt.Rows(i).Item("chiave_composita") = dt.Rows(i).Item("Data2") & "_" &
                                                                dt.Rows(i).Item("Id_Agenda") & "_" &
                                                                dt.Rows(i).Item("Lav_Cod") & "_" &
                                                                dt.Rows(i).Item("Piva") & "_" &
                                                                dt.Rows(i).Item("Sa_Cod") & "_" &
                                                                dt.Rows(i).Item("Blocco_Flag") & "_" &
                                                                dt.Rows(i).Item("Veg_Cod")


            Next
        End If

    End Sub

    Private Shared Sub EstendiDatatableZoo(ByRef dt As DataTable)

        dt.Columns.Add("Centro_Aziendale", GetType(String))
        dt.Columns.Add("Specie", GetType(String))
        dt.Columns.Add("Appezzamenti_Coinvolti", GetType(String))
        dt.Columns.Add("Prodotti_Utilizzati", GetType(String))
        dt.Columns.Add("Avversita", GetType(String))

        'If Not dt.Columns.Contains("chiave_composita") Then
        '    dt.Columns.Add("Centro_Aziendale", GetType(String))
        '    dt.Columns.Add("Specie", GetType(String))
        '    dt.Columns.Add("Appezzamenti_Coinvolti", GetType(String))
        '    dt.Columns.Add("Prodotti_Utilizzati", GetType(String))
        '    dt.Columns.Add("Avversita", GetType(String))

        '    dt.Columns.Add("chiave_composita", GetType(String))
        '    Dim delim As String() = New String(0) {"<br>"}

        '    For i = 0 To dt.Rows.Count - 1

        '        If Not IsDBNull(dt.Rows(i).Item("Dettagli")) Then

        '            Dim dettagli = dt.Rows(i).Item("Dettagli")
        '            Dim dett() As String = dettagli.Split(delim, StringSplitOptions.None)

        '            For count = 0 To dett.Length - 1

        '                If InStr(dett(count), ":") Then

        '                    Dim dett2 As String() = dett(count).Split(":")

        '                    If dett(count) <> "" Then

        '                        Dim final_s As String = dett2(1).Replace("</b> ", "")

        '                        Select Case dett2(0)
        '                            Case "<b>Centro Az."
        '                                dt.Rows(i).Item("Centro_Aziendale") = final_s
        '                            Case "<b>Specie"
        '                                dt.Rows(i).Item("Specie") = final_s
        '                            Case "<b>Appezzamenti Coinvolti"
        '                                dt.Rows(i).Item("Appezzamenti_Coinvolti") = final_s
        '                            Case "<b>Prodotti Utilizzati"
        '                                dt.Rows(i).Item("Prodotti_Utilizzati") = final_s
        '                            Case " <b> Avversità"
        '                                final_s = final_s.Replace("-", "")
        '                                dt.Rows(i).Item("Avversita") = final_s
        '                        End Select

        '                    End If

        '                End If
        '            Next
        '        End If

        '        dt.Rows(i).Item("chiave_composita") = dt.Rows(i).Item("Data2") & "_" &
        '                                                        dt.Rows(i).Item("Id_Agenda") & "_" &
        '                                                        dt.Rows(i).Item("Lav_Cod") & "_" &
        '                                                        dt.Rows(i).Item("Piva") & "_" &
        '                                                        dt.Rows(i).Item("Sa_Cod") & "_" &
        '                                                        dt.Rows(i).Item("Blocco_Flag") & "_" &
        '                                                        dt.Rows(i).Item("Veg_Cod")


        '    Next
        'End If

    End Sub

    Public Shared Function DT_to_Json_OperazioniDestinazioni(ByVal dt As DataTable, objParametri_Server As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        c = New ColonneNome("Id_Agenda", "Id_Agenda", "string")
        c._hidden = True
        l.Add(c)


        'l.Add(New ColonneNome("Impresa", "Impresa", "string"))
        l.Add(New ColonneNome("CentroAziendale", "Centro Aziendale", "string"))
        l.Add(New ColonneNome("data_movimento", "Data", "date"))
        l.Add(New ColonneNome("Operazione", "Operazione", "string"))
        'l.Add(New ColonneNome("DescrizioneCommessa", "Commessa", "string"))
        'l.Add(New ColonneNome("CodiceCommessa", "Commessa Codice", "string"))
        l.Add(New ColonneNome("Campo_Des", "Campo", "string"))
        l.Add(New ColonneNome("specie", "specie", "string"))
        l.Add(New ColonneNome("Varieta", "Varieta", "string"))
        l.Add(New ColonneNome("DestinazioneUso", "Uso", "string"))
        'l.Add(New ColonneNome("Macrouso_Des", "Macrouso", "string"))
        c = New ColonneNome("Macrouso_Des", "Macrouso", "string")
        c._Display = False
        l.Add(c)

        'l.Add(New ColonneNome("Ore", "Ore", "number"))
        c = New ColonneNome("Ore", "Ore", "string")
        c._Display = False
        l.Add(c)

        l.Add(New ColonneNome("SupTrattata", "HA", "number"))
        l.Add(New ColonneNome("Prodotto", "Prodotto", "string"))
        l.Add(New ColonneNome("LottoProdotto", "Lotto Prodotto", "string"))
        'l.Add(New ColonneNome("FattoreProporzione", "FattoreProporzione", "number"))
        l.Add(New ColonneNome("DoseHA", "Dose ad HA", "number"))
        l.Add(New ColonneNome("UNITA_DI_MISURA", "Unità di misura", "string"))
        l.Add(New ColonneNome("h2o", "Acqua", "number"))
        l.Add(New ColonneNome("avversita", "Avversità", "string"))
        l.Add(New ColonneNome("Operatore", "Operatore", "string"))
        l.Add(New ColonneNome("Macchina_Trattore_codice", "Macchina_Trattore_codice", "string"))
        'l.Add(New ColonneNome("Macchina_Botte_codice", "Macchina_Botte_codice", "string"))
        'l.Add(New ColonneNome("numeroPatentino", "numeroPatentino", "string"))
        l.Add(New ColonneNome("FaseFenologica", "Fase Fenologica", "string"))
        l.Add(New ColonneNome("Utente_Modifica", "autore ultima Modifica", "string"))

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp


    End Function

    ''' <summary>
    ''' Restituisce il formato griglia json
    ''' </summary>
    ''' <param name="dt"></param>
    ''' <param name="TipoGriglia">1 = no tracciabilità, 2 = tracciabilità</param>
    ''' <returns></returns>
    Public Shared Function DT_to_Json_Azienda(ByVal dt As DataTable,
                                              ByVal TipoGriglia As String,
                                              ByVal objParametri_Server As AgronicaCoreParametri,
                                              ByVal visualizza_codiciImp As Boolean,
                                              ByVal visualizza_KPIN_BlockName As Boolean) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        If TipoGriglia = "2" Then
            c = New ColonneNome("id_mov_det", "id_mov_det", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("id_agenda", "Sel.", "string")
            c._Filtrabile = False
            c._ColonnaDiSelezione = True
            c._hidden = True
            l.Add(c)
        End If

        'If TipoGriglia <> "2" Then
        '    'aggiungo i pulsanti per modifica ed eliminazione
        '    c = New ColonneNome("id_agenda", "tool", "string")
        '    c._FormatoParticolare = "<span id='ModificaRiga' class='fa fa-info-circle edit_elem fa-2x' chiave='{0}'></span> "
        '    c._Filtrabile = False
        '    l.Add(c)
        'End If

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Track_Cal_Cod_Padre", "FF_Track_Cal_Cod_Padre", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_Track_Cal_Cod", "FF_Track_Cal_Cod_Padre", "string")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("FF_Track_Lotto_Padre", "Lotto Padre", "string")
            'c._hidden = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("FF_Track_Lotto", "Lotto", "string")
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)
        End If

        If TipoGriglia = "2" Then

            l.Add(New ColonneNome("Data2", "Data", "date") With {._Filtrabile = False})
            l.Add(New ColonneNome("Lav_Des", "Operazione", "string"))
            l.Add(New ColonneNome("Specie", Resources.AgronicaAgenda_2010.Specie, "string") With {._Display = False})
            l.Add(New ColonneNome("cul_Des", Resources.AgronicaAgenda_2010.Varietà, "string") With {._Display = False})
            l.Add(New ColonneNome("Specie_Varieta", Resources.AgronicaAgenda_2010.Specie & " " & Resources.AgronicaAgenda_2010.Varietà, "string")) '"Specie Varietà"
            l.Add(New ColonneNome("Prodotti_Utilizzati", Resources.AgronicaAgenda_2010.ProdottiUtilizzati, "string") With {._Display = False})
            l.Add(New ColonneNome("Avversita", "Avversita", "string") With {._Display = False})
            l.Add(New ColonneNome("Dettaglio_Tecnico", "Dettaglio Tecnico", "string"))
            l.Add(New ColonneNome("Centro_Aziendale", Resources.AgronicaAgenda_2010.CentroAziendale, "string") With {._Display = False})
            l.Add(New ColonneNome("Centro_Campo", Resources.AgronicaAgenda_2010.Campo, "string")) '"Centri Campi"
            l.Add(New ColonneNome("Appezzamenti_Coinvolti", Resources.AgronicaAgenda_2010.AppezzamentiCoinvolti, "string"))
            l.Add(New ColonneNome("LottiImpianto", "Lotti Impianto", "string") With {._Display = False})
            l.Add(New ColonneNome("Sup_Trattata", "Superficie Movimentata", "string") With {._Display = False, ._FormatoParticolare = "#= kendo.format('{0}',Sup_Trattata) #"}) 'kendo.toString(Sup_Trattata,'n')
            'l.Add(New ColonneNome("Sup_Trattata", "Superficie Movimentata", "number") With {._Display = False, ._FormatoParticolare = "#=(Sup_Trattata === 0) ? '' : Sup_Trattata.toString().replace('.', ',')#"}) 'kendo.toString(Sup_Trattata,'n')
            'l.Add(New ColonneNome("Sup_Trattata", "Superficie Movimentata", "number") With {._Display = False}) 'kendo.toString(Sup_Trattata,'n')
            l.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})
            l.Add(New ColonneNome("tipo", "tipo", "string") With {._hidden = True}) 'identifica il tipo di operazione (per colorare le righe)
            l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})
            l.Add(New ColonneNome("Ricetta_Des", "Ricetta_Des", "string") With {._hidden = True})
            l.Add(New ColonneNome("Data", "Data_Stringa", "string") With {._hidden = True})
            l.Add(New ColonneNome("contabilizzato", "contabilizzato", "number") With {._hidden = True})
            l.Add(New ColonneNome("LottiProduzione", "Lotti di Produzione", "string") With {._Display = False})
            l.Add(New ColonneNome("Note", "Note", "string") With {._Display = False})
            l.Add(New ColonneNome("Costi_Operatori", "Operatori", "string") With {._Display = False})
            l.Add(New ColonneNome("Costi_Macchine", "Macchine", "string") With {._Display = False})
            l.Add(New ColonneNome("Creatore_Intervento", "Creatore Intervento", "string") With {._Display = False})
            l.Add(New ColonneNome("Data_Ultima_Modifica_Intervento", "Data Ultima Modifica Intervento", "date") With {._Display = False})
            l.Add(New ColonneNome("ID", "ID", "string") With {._Display = False})

            If visualizza_codiciImp Then
                l.Add(New ColonneNome("Codici_Impianto", "Codici Impianto", "string") With {._Display = False})
                l.Add(New ColonneNome("Codici_Appezzamenti", "Codici Appezzamento", "string") With {._Display = False})
            End If

            If visualizza_KPIN_BlockName Then
                l.Add(New ColonneNome("KPIN", "KPIN", "string") With {._Display = False})
                l.Add(New ColonneNome("BlockName", "Block Name", "string") With {._Display = False})
            End If

        Else
            l.Add(New ColonneNome("Data", "Data", "date"))
        End If

        If TipoGriglia = "1" Then
            c = New ColonneNome("Lav_des", Resources.AgronicaAgenda_2010.Descrizione, "string") '"Descrizione"
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("Dettagli", "Dettagli", "string") '"Dettagli"
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            l.Add(c)

            c = New ColonneNome("NomeComune", Resources.AgronicaAgenda_2010.CategoriaProdotto, "string") '"Categoria Prodotto"
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("FF_Referenza", Resources.AgronicaAgenda_2010.Prodotto, "string") ' "Prodotto"
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            Dim objConfigDettagli As AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R = New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
            Dim DTParamQual As DataTable = objConfigDettagli.Leggi(CStr(dt.Rows(0).Item("piva")), 0, False, "Tipo = 1", "", objParametri_Server)

            For Each paramQual In DTParamQual.Rows
                If paramQual("Tipo") = 1 Then
                    If Not ({"cliente"}).Contains(paramQual("Tabella_Key").ToString.ToLower) Then
                        c = New ColonneNome("FF_" & paramQual("Tabella_Key"), paramQual("Tabella_Key"), "string")
                        c._Filtrabile = True
                        c._FiltrabileConCheck = True
                        l.Add(c)
                    End If
                End If
            Next

        End If

        l.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
        l.Add(New ColonneNome("Veg_cod", "veg_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Operazione_DES", "Operazione_DES", "string") With {._hidden = True})
        l.Add(New ColonneNome("gru_des", "gru_des", "string") With {._hidden = True})

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Track_Qta_Extra_Totale", "Quantità", "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)

            c = New ColonneNome("FF_Track_Qta_Contenitori", "Contenitori", "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)

            c = New ColonneNome("FF_Track_Qta_Imballi", "Imballi", "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)
        End If

        c = New ColonneNome("Rag_Soc", "Azienda", "string")
        If TipoGriglia = "2" Then
            c._hidden = True
        Else
            c._Filtrabile = True
            c._FiltrabileConCheck = True
        End If
        l.Add(c)

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Righe_Aggiunte", "Righe aggiunte", "string")
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("FF_Operazioni_Campagna", "Operazioni Campagna", "string")
            c._Filtrabile = True
            c._RemoveHtmlEncode = True
            c._FiltrabileConCheck = True
            l.Add(c)
        End If

        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", "Descrizione Unica", "string") With {._Display = False, ._RemoveHtmlEncode = True})

        If dt.Columns.Contains("AggiungiAlPua") = True Then
            l.Add(New ColonneNome("AggiungiAlPua", "AggiungiAlPua", "string") With {._hidden = True})
        End If

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_KendoOpt(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function


    Public Shared Function DT_to_Json_Colturali(ByRef dt As DataTable, ByRef objParametri_Server As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id_agenda", "Sel.", "string") With {._hidden = True, ._Filtrabile = False, ._ColonnaDiSelezione = True})

        l.Add(New ColonneNome("Data2", "Data", "date") With {._Filtrabile = False})
        l.Add(New ColonneNome("Lav_Des", "Operazione", "string"))
        l.Add(New ColonneNome("Specie_Varieta", "Specie Varietà", "string"))
        l.Add(New ColonneNome("Dettaglio_Tecnico", "Dettaglio Tecnico", "string"))
        l.Add(New ColonneNome("Centro_Campo", "Centri Campi", "string"))
        l.Add(New ColonneNome("Appezzamenti_Coinvolti", "Appezzamenti", "string"))
        l.Add(New ColonneNome("LottiImpianto", "Lotti Impianto", "string") With {._Display = False})
        l.Add(New ColonneNome("Sup_Trattata", "Superficie Movimentata", "number") With {._Display = False, ._FormatoParticolare = "#=(Sup_Trattata === 0) ? '' : Sup_Trattata.toString().replace('.', ',')#"}) 'kendo.toString(Sup_Trattata,'n')
        l.Add(New ColonneNome("LottiProduzione", "Lotti di Produzione", "string") With {._Display = False})
        l.Add(New ColonneNome("Note", "Note", "string") With {._Display = False})
        l.Add(New ColonneNome("Costi_Operatori", "Operatori", "string") With {._Display = False})
        l.Add(New ColonneNome("Costi_Macchine", "Macchine", "string") With {._Display = False})
        l.Add(New ColonneNome("Creatore_Intervento", "Creatore Intervento", "string") With {._Display = False})
        l.Add(New ColonneNome("Data_Ultima_Modifica_Intervento", "Data Ultima Modifica Intervento", "date") With {._Display = False})
        l.Add(New ColonneNome("ID", "ID", "string") With {._Display = False})

        l.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})
        l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Data", "Data_Stringa", "string") With {._hidden = True})
        l.Add(New ColonneNome("contabilizzato", "contabilizzato", "number") With {._hidden = True})
        l.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
        l.Add(New ColonneNome("Veg_cod", "veg_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Rag_Soc", "Azienda", "string") With {._hidden = True})

        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", "Descrizione Unica", "string") With {._Display = False, ._RemoveHtmlEncode = True})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function

    Public Shared Function DT_to_Json_MagCont(ByRef dt As DataTable, ByRef objParametri_Server As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id_agenda", "Sel.", "string") With {._hidden = True, ._Filtrabile = False, ._ColonnaDiSelezione = True})

        l.Add(New ColonneNome("Data2", "Data", "date") With {._Filtrabile = False})
        l.Add(New ColonneNome("Lav_Des", "Operazione", "string"))
        l.Add(New ColonneNome("Prodotti", "Prodotti", "string"))
        l.Add(New ColonneNome("RifDdtFatture", "Rif DDT e Fatture", "string"))
        l.Add(New ColonneNome("Centro_Magazzino", "Centri Magazzini", "string"))
        'l.Add(New ColonneNome("Note", "Note", "string") With {._Display = False})
        l.Add(New ColonneNome("Creatore_Intervento", "Creatore Intervento", "string") With {._Display = False})
        l.Add(New ColonneNome("Data_Ultima_Modifica_Intervento", "Data Ultima Modifica Intervento", "date") With {._Display = False})
        l.Add(New ColonneNome("ID", "ID", "string") With {._Display = False})

        l.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})
        l.Add(New ColonneNome("Data", "Data_Stringa", "string") With {._hidden = True})
        l.Add(New ColonneNome("contabilizzato", "contabilizzato", "number") With {._hidden = True})
        l.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
        l.Add(New ColonneNome("Veg_cod", "veg_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Rag_Soc", "Azienda", "string") With {._hidden = True})
        l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})

        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", "Descrizione Unica", "string") With {._Display = False, ._RemoveHtmlEncode = True})

        '._FormatoParticolare = "#=Data# <b>#=Lav_Des#</b><i>#=(RifDdtFatture === '') ? '' : '<br>' + RifDdtFatture #</i><br>#=Centro_Magazzino#"}
        '$.grep([#=Data# <b>#=Lav_Des#</b>, #=(RifDdtFatture === '') ? '' : '<i>' + RifDdtFatture + '</i>'#, #=Centro_Magazzino#], Boolean).join('<br>');

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function

    Public Shared Function DT_to_Json_Audit(ByRef dt As DataTable, ByRef objParametri_Server As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id_agenda", "Sel.", "string") With {._hidden = True, ._Filtrabile = False, ._ColonnaDiSelezione = True})

        l.Add(New ColonneNome("Data2", "Data", "date") With {._Filtrabile = False})
        l.Add(New ColonneNome("Lav_Des", "Operazione", "string"))
        l.Add(New ColonneNome("Dettaglio_Tecnico", "Dettaglio Tecnico", "string") With {._Display = False})
        l.Add(New ColonneNome("Note", "Note", "string") With {._Display = False})
        l.Add(New ColonneNome("Creatore_Intervento", "Creatore Intervento", "string") With {._Display = False})
        l.Add(New ColonneNome("Data_Ultima_Modifica_Intervento", "Data Ultima Modifica Intervento", "date") With {._Display = False})
        l.Add(New ColonneNome("ID", "ID", "string") With {._Display = False})

        l.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})
        l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Data", "Data_Stringa", "string") With {._hidden = True})
        l.Add(New ColonneNome("contabilizzato", "contabilizzato", "number") With {._hidden = True})
        l.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
        l.Add(New ColonneNome("Veg_cod", "veg_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Rag_Soc", "Azienda", "string") With {._hidden = True})

        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", "Descrizione Unica", "string") With {._Display = False, ._RemoveHtmlEncode = True})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function

    Public Shared Function DT_to_Json_Macchine(ByRef dt As DataTable, ByRef objParametri_Server As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        l.Add(New ColonneNome("id_agenda", "Sel.", "string") With {._hidden = True, ._Filtrabile = False, ._ColonnaDiSelezione = True})

        l.Add(New ColonneNome("Data2", "Data", "date") With {._Filtrabile = False})
        l.Add(New ColonneNome("Lav_Des", "Operazione", "string"))
        l.Add(New ColonneNome("Dettaglio_Tecnico", "Dettaglio Tecnico", "string") With {._Display = False})
        l.Add(New ColonneNome("Note", "Note", "string") With {._Display = False})
        l.Add(New ColonneNome("Creatore_Intervento", "Creatore Intervento", "string") With {._Display = False})
        l.Add(New ColonneNome("Data_Ultima_Modifica_Intervento", "Data Ultima Modifica Intervento", "date") With {._Display = False})
        l.Add(New ColonneNome("ID", "ID", "string") With {._Display = False})

        l.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})
        l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Data", "Data_Stringa", "string") With {._hidden = True})
        l.Add(New ColonneNome("contabilizzato", "contabilizzato", "number") With {._hidden = True})
        l.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
        l.Add(New ColonneNome("Veg_cod", "veg_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Rag_Soc", "Azienda", "string") With {._hidden = True})

        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", "Descrizione Unica", "string") With {._Display = False, ._RemoveHtmlEncode = True})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function


    Public Shared Function DT_to_Json_AziendaZoo(ByVal dt As DataTable, ByVal TipoGriglia As String, ByVal objParametri_Server As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)
        Dim c As ColonneNome

        If TipoGriglia = "2" Then
            c = New ColonneNome("id_mov_det", "id_mov_det", "number")
            c._hidden = True
            l.Add(c)

            c = New ColonneNome("id_agenda", "Sel.", "string")
            c._Filtrabile = False
            c._ColonnaDiSelezione = True
            c._hidden = True
            l.Add(c)
        End If

        'If TipoGriglia <> "2" Then
        '    'aggiungo i pulsanti per modifica ed eliminazione
        '    c = New ColonneNome("id_agenda", "tool", "string")
        '    c._FormatoParticolare = "<span id='ModificaRiga' class='fa fa-info-circle edit_elem fa-2x' chiave='{0}'></span> "
        '    c._Filtrabile = False
        '    l.Add(c)
        'End If

        If TipoGriglia = "2" Then

            l.Add(New ColonneNome("Data2", AgronicaAgenda_2010.Data, "date") With {._Filtrabile = False})
            l.Add(New ColonneNome("Lav_Des", AgronicaAgenda_2010.Operazione, "string"))
            l.Add(New ColonneNome("Prodotti_Utilizzati", AgronicaAgenda_2010.ProdottiUtilizzati, "string") With {._Display = False})
            l.Add(New ColonneNome("Dettaglio_Tecnico", Gias.DettaglioTecnico, "string"))
            l.Add(New ColonneNome("Segnalazioni", HttpContext.GetLocalResourceObject("~/Menu/MenuBS_Agenda_Nuovo.aspx", "Segnalazioni"), "string"))
            l.Add(New ColonneNome("sa_nome", AgronicaAgenda_2010.CentroAziendale, "string") With {._Display = False})


            l.Add(New ColonneNome("chiave_composita", "chiave_composita", "string") With {._hidden = True})

            l.Add(New ColonneNome("tipo", "tipo", "string") With {._hidden = True}) 'identifica il tipo di operazione (per colorare le righe)


            l.Add(New ColonneNome("Data", "Data_Stringa", "string") With {._hidden = True})
            l.Add(New ColonneNome("contabilizzato", "contabilizzato", "number") With {._hidden = True})
            l.Add(New ColonneNome("LottiProduzione", Gias.LottiProduzione, "string") With {._Display = False})
            l.Add(New ColonneNome("Note", AgronicaAgenda_2010.Note, "string") With {._Display = False})
            l.Add(New ColonneNome("Costi_Operatori", Gias.Operatori, "string") With {._Display = False})
            l.Add(New ColonneNome("Costi_Macchine", AgronicaAgenda_2010.Macchine, "string") With {._Display = False})
            l.Add(New ColonneNome("Creatore_Intervento", AgronicaAgenda_2010.CreatoreIntervento, "string") With {._Display = False})
            l.Add(New ColonneNome("ID", "ID", "string") With {._Display = False})

        Else
            l.Add(New ColonneNome("Data", AgronicaAgenda_2010.Data, "string"))
        End If

        If TipoGriglia = "1" Then
            c = New ColonneNome("Lav_des", AgronicaAgenda_2010.Descrizione, "string")
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

            c = New ColonneNome("Dettagli", Gias.Dettagli, "string")
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            l.Add(c)

            c = New ColonneNome("FF_Referenza", AgronicaAgenda_2010.Prodotto, "string")
            c._RemoveHtmlEncode = True
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)

        End If

        l.Add(New ColonneNome("Lav_cod", "Lav_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Tipo_Accettazione", "Tipo_Accettazione", "number") With {._hidden = True})
        l.Add(New ColonneNome("Piva", "Piva", "string") With {._hidden = True})
        l.Add(New ColonneNome("sa_cod", "sa_cod", "string") With {._hidden = True})
        l.Add(New ColonneNome("Operazione_DES", "Operazione_DES", "string") With {._hidden = True})
        l.Add(New ColonneNome("Prodotti", AgronicaAgenda_2010.Prodotti, "string"))

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Track_Qta_Extra_Totale", "Kg", "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)

            c = New ColonneNome("FF_Track_Qta_Contenitori", Gias.Contenitori, "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)

            c = New ColonneNome("FF_Track_Qta_Imballi", AgronicaAgenda_2010.Imballi, "number")
            c._Filtrabile = True
            c._formatNr = "n0"
            l.Add(c)
        End If

        c = New ColonneNome("Rag_Soc", AgronicaAgenda_2010.Azienda, "string")
        If TipoGriglia = "2" Then
            c._hidden = True
        Else
            c._Filtrabile = True
            c._FiltrabileConCheck = True
        End If
        l.Add(c)

        If TipoGriglia = "1" Then
            c = New ColonneNome("FF_Righe_Aggiunte", Gias.RigheAggiunte, "string")
            c._Filtrabile = True
            c._FiltrabileConCheck = True
            l.Add(c)
        End If

        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", Gias.DescrizioneUnica, "string") With {._Display = False, ._RemoveHtmlEncode = True})


        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaRicette(ByVal filtro As String) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda = New ParametriAgenda

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If


        Lingua.Gias_InizializzaCultura_DaSession()

        Try
            If Utils.LEGACY_SWITCH_USECOREWS Then
                Dim bus As New AgronicaCoreContabBIZ.MovimentiNG(objParametri_Server, objParametri_Utenti) ' bussiness layer
                r.RispostaStringa = bus.CaricaRicette(filtro, objParametriAgenda.Piva)
                r.RispostaOK = True
                Return r
            Else

                Dim jSonDatiFiltro As JObject = JObject.Parse(filtro)

                Dim piva As String = objParametriAgenda.Piva
                Dim veg_cod As Integer = If(IsNumeric(jSonDatiFiltro("veg_cod")) AndAlso CInt(jSonDatiFiltro("veg_cod")) > 0, jSonDatiFiltro("veg_cod"), 0)
                Dim sa_cod As Integer = If(IsNumeric(jSonDatiFiltro("sa_cod")), jSonDatiFiltro("sa_cod"), 0)

                Dim sData_Selezionata1 As String = jSonDatiFiltro("txt_Data1").ToString
                Dim Data_Selezionata1 As DateTime = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

                Dim sData_Selezionata2 As String = jSonDatiFiltro("txt_Data2").ToString
                Dim Data_Selezionata2 As DateTime = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

                Dim stato As String = jSonDatiFiltro("stato").ToString
                Dim filtroQuery As String = ""

                If IsNumeric(stato) Then
                    filtroQuery = "ro.W_Anagrafica_Stati_Cod = " & stato
                    'Select Case stato
                    '    Case enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Da_Eseguire ' = 300
                    '        filtroQuery = "ro.W_Anagrafica_Stati_Cod = " & stato
                    '    Case enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita ' = 301
                    '        filtroQuery = "(ro.W_Anagrafica_Stati_Cod IS NULL OR ro.W_Anagrafica_Stati_Cod = " & stato & ")"
                    'End Select
                End If

                Dim TipoOperazioniStr As String = ""
                If jSonDatiFiltro("tipoOperazione") IsNot Nothing AndAlso jSonDatiFiltro("tipoOperazione").ToString <> "" Then
                    Dim arr = JArray.Parse(jSonDatiFiltro("tipoOperazione").ToString)

                    If arr.Count > 0 Then
                        For Each elem In arr
                            TipoOperazioniStr &= elem.ToString & ","
                        Next
                        TipoOperazioniStr = TipoOperazioniStr.Substring(0, TipoOperazioniStr.Length - 1)
                    End If

                End If

                Dim strFiltroRicetteOperazioni As String = ""
                Dim impianti As New List(Of String)
                If jSonDatiFiltro("impianti") IsNot Nothing AndAlso jSonDatiFiltro("impianti").ToString <> "" Then
                    Dim arr = JArray.Parse(jSonDatiFiltro("impianti").ToString)
                    If arr.Count > 0 Then
                        Dim DT_ID_Agenda As DataTable

                        Dim objMov_Destinazioni As New AgronicaCoreContabDAL.Ricette_Destinazioni_R
                        For Each elem In arr
                            Dim elemArr = elem.ToString.Split("_")
                            Dim dtAgImp = objMov_Destinazioni.Leggi_DistinctRicetta_Operazione_Cod_Impianti(objParametri_Server,
                                                                                 elemArr(0),
                                                                                 elemArr(1),
                                                                                 elemArr(2),
                                                                                 elemArr(3),
                                                                                 Data_Selezionata1,
                                                                                 Data_Selezionata2,
                                                                                 "",
                                                                                 "")

                            If DT_ID_Agenda Is Nothing Then
                                DT_ID_Agenda = dtAgImp.Copy
                            Else
                                DT_ID_Agenda.Merge(dtAgImp)
                            End If
                        Next

                        If DT_ID_Agenda.Rows.Count > 0 Then
                            DT_ID_Agenda = DT_ID_Agenda.DefaultView.ToTable(True, "Ricetta_Operazione_Cod")
                            strFiltroRicetteOperazioni = ""
                            For Each rowAgImp In DT_ID_Agenda.Rows
                                strFiltroRicetteOperazioni &= rowAgImp(0) & ","
                            Next
                            strFiltroRicetteOperazioni = strFiltroRicetteOperazioni.Substring(0, strFiltroRicetteOperazioni.Length - 1)
                        Else
                            strFiltroRicetteOperazioni = "0"
                        End If

                    End If

                End If


                Dim objRicette As New AgronicaCoreContabDAL.Ricette_R
                Dim dtRicette As DataTable = objRicette.Leggi_xMenuAgenda(piva, sa_cod, 0, veg_cod, 0, Data_Selezionata1, Data_Selezionata2, True, filtroQuery, "", objParametri_Server, TipoOperazioniStr, strFiltroRicetteOperazioni)

                dtRicette.Columns.Add(New DataColumn("veg_des_unificato", Type.GetType("System.String")))
                dtRicette.Columns.Add(New DataColumn("PermessoModifica", Type.GetType("System.String")))
                dtRicette.Columns.Add(New DataColumn("Descrizione_Unica", Type.GetType("System.String")))
                dtRicette.Columns.Add(New DataColumn("Dettaglio_Tecnico", Type.GetType("System.String")))
                dtRicette.Columns.Add(New DataColumn("Costi_Operatori", Type.GetType("System.String")))
                dtRicette.Columns.Add(New DataColumn("Costi_Macchine", Type.GetType("System.String")))

                Dim dtRisultato As DataTable = dtRicette.Clone()

                If Not IsNothing(dtRicette) AndAlso dtRicette.Rows.Count > 0 Then

                    Dim ObjUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim dtImpostazioni As DataTable = ObjUtenti.Leggi(enum_Impostazioni_Utenti.SUPERUSER_COD_PERMETTI_MODIFICA_RICETTE_CON_OPERAZIONI_REGISTRATE,
                                                                      2, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

                    Dim consentiModificaSeInUso As Boolean = If(Not IsNothing(dtImpostazioni) AndAlso dtImpostazioni.Rows.Count > 0 AndAlso dtImpostazioni.Rows(0).Item("Impostazione_Valore_1") = "1", True, False)

                    Dim strRicetta_Operazione_Cod As String() = (From r2 As DataRow In dtRicette.AsEnumerable Select New With {Key .roc = r2.Item("Ricetta_Operazione_Cod"), Key .rc = r2.Item("Ricetta_Cod"), Key .data = r2.Item("Ricetta_Operazione_Data")}).Distinct().OrderByDescending(Function(x) CDate(x.data)).ThenByDescending(Function(x) x.rc).Select(Function(y) CStr(y.roc)).ToArray()

                    If Not IsNothing(strRicetta_Operazione_Cod) Then

                        'COSTI ACCESSORI
                        Dim objCostiAccessori As New AgronicaCoreContabDAL.CostiAccessori_R
                        Dim DtCosti As DataTable = objCostiAccessori.CostiAccessori_from_Ricetta_Operazione_Cod(String.Join(",", strRicetta_Operazione_Cod), "", "", objParametri_Server)

                        'PRINCIPI ATTIVI
                        Dim HtProdPA As New Hashtable()
                        Dim HtPrincAtt As Hashtable = estraiPrincipiAttivi(dtRicette, HtProdPA, objParametri_Server)

                        'AVVERSITA
                        Dim objMovTec As New AgronicaCoreContabDAL.Ricette_Dett_Tecnico_R
                        Dim dtMovDetTec As DataTable = objMovTec.Leggi_x_avversita(piva, "", "", objParametri_Server)

                        For Each roc As Integer In strRicetta_Operazione_Cod

                            Dim dtRicettaSingola As DataTable = dtRicette.Select("Ricetta_Operazione_Cod=" & roc).CopyToDataTable()

                            Dim dr As DataRow = dtRicettaSingola.Rows(0)

                            If CDate(dr.Item("Validita_Inizio")) = AGRODATAINIZIO Then
                                dr.Item("Validita_Inizio") = DBNull.Value
                            End If

                            If CDate(dr.Item("Validita_Fine")) = AGRODATAFINE Then
                                dr.Item("Validita_Fine") = DBNull.Value
                            End If

                            If CDate(dr.Item("Ricetta_Operazione_Data")) = AGRODATAINIZIO Then
                                dr.Item("Ricetta_Operazione_Data") = dr.Item("Validita_Inizio")
                            End If

                            dr.Item("PermessoModifica") = If(dr.Item("Blocco_Flag") = 0 AndAlso (consentiModificaSeInUso OrElse (dr.Item("in_uso") = 0)), True, False)
                            dr.Item("Descrizione_Unica") = dr.Item("Ricetta_Numero") & " <b>" & dr.Item("lav_des") & "</b><br>" &
                                                           dr.Item("Ricetta_Operazione_Data") & " <i>" & dr.Item("Veg_Des_r") & "</i>"

                            Dim strApp_Nome As List(Of String) = (From rr As DataRow In dtRicettaSingola.AsEnumerable() Where CStr(rr.Item("App_Nome")) <> "" Select CStr(rr.Item("App_Nome"))).Distinct().ToList()
                            dr.Item("App_Nome") = String.Join(", ", strApp_Nome).Replace("'", "")

                            If IsDBNull(dr.Item("veg_cod_op")) Then
                                Dim x = (From rr As DataRow In dtRicettaSingola.AsEnumerable() Where Not IsDBNull(rr.Item("veg_cod_op")) Select rr.Item("veg_cod_op")).ToList()
                                If x.Count > 0 Then
                                    dr.Item("veg_cod_op") = x.First()
                                Else
                                    dr.Item("veg_cod_op") = "0"
                                End If
                            End If

                            If IsDBNull(dr.Item("veg_des_op")) Then
                                Dim x = (From rr As DataRow In dtRicettaSingola.AsEnumerable() Where Not IsDBNull(rr.Item("veg_des_op")) Select rr.Item("veg_des_op")).ToList()
                                If x.Count > 0 Then
                                    dr.Item("veg_des_op") = x.First()
                                End If
                            End If



                            dr.Item("veg_des_unificato") = If(IsNumeric(dr.Item("Tipo_Ricetta")) AndAlso {"5", "6", "9"}.Contains(dr.Item("Tipo_Ricetta")), If(IsDBNull(dr.Item("veg_des_op")), dr.Item("DestinazioneTerreniNudi_Des"), dr.Item("veg_des_op")), dr.Item("veg_des_r"))


                            '-------------------------------------------------------------------------------------------------------------------
                            'COSTI ACCESSORI
                            Dim listaOperatori As New List(Of String)
                            Dim listaMacchine As New List(Of String)
                            If Not IsNothing(DtCosti) AndAlso DtCosti.Rows.Count > 0 Then
                                Dim DrCosti() As DataRow = DtCosti.Select("Ricetta_Operazione_Cod=" & roc)
                                If Not IsNothing(DrCosti) Then
                                    For Each dr_costo As DataRow In DrCosti

                                        'è un record manodopera
                                        If dr_costo.Item("Cod_RisUm") <> 0 Then

                                            'Recupero il nome del contatto
                                            Dim nomeContatto As String = If(dr_costo.Item("Rag_Soc") <> "", dr_costo.Item("Rag_Soc"), String.Format("{0} {1}", dr_costo.Item("Cognome"), dr_costo.Item("Nome")))

                                            If Not listaOperatori.Contains(nomeContatto) Then
                                                listaOperatori.Add(nomeContatto)
                                            End If

                                        Else
                                            'è un record macchinario

                                            Dim detMacchina As String = dr_costo.Item("CLASS_DESC")
                                            detMacchina &= If(dr_costo.Item("Modello") <> "", " - Modello " & dr_costo.Item("Modello"), "")
                                            detMacchina &= If(dr_costo.Item("Ditta_Des") <> "", " - Marca " & dr_costo.Item("Ditta_Des"), "")

                                            If Not listaMacchine.Contains(detMacchina) Then
                                                listaMacchine.Add(detMacchina)
                                            End If

                                        End If

                                    Next
                                End If
                            End If

                            dr.Item("Costi_Operatori") = String.Join(", ", listaOperatori)
                            dr.Item("Costi_Macchine") = String.Join(", ", listaMacchine)
                            '-------------------------------------------------------------------------------------------------------------------

                            '-------------------------------------------------------------------------------------------------------------------
                            'DETTAGLIO TECNICO
                            Dim listaDetTec As New List(Of String)
                            For Each drDetTec As DataRow In dtRicettaSingola.Rows
                                Dim prod As String = ""
                                Dim princAtt As String = ""
                                Dim avv As String = ""

                                'PRODOTTI
                                Select Case drDetTec.Item("Elem_Cod")
                                    Case FERTILIZZANTI

                                        If drDetTec.Item("Pro_Cod") <> 0 Then
                                            prod = drDetTec.Item("Fer_Des") & ", "
                                        End If

                                        If drDetTec.Item("Mat_Cod") <> 0 Then
                                            prod = drDetTec.Item("Mat_Des")
                                        Else
                                            If prod.Length > 0 Then
                                                prod = Left(prod, prod.Length - 2)
                                            End If
                                        End If

                                    Case FORMULATI

                                        If drDetTec.Item("Fr_Des") <> "" Then
                                            prod = drDetTec.Item("Fr_Des")
                                        End If

                                    Case TRAPPOLE

                                        If drDetTec.Item("Trap_Des") <> "" Then
                                            prod = drDetTec.Item("Trap_Des")
                                        End If

                                    Case SEMENTI

                                        If drDetTec.Item("Mat_Des") <> "" Then
                                            prod = drDetTec.Item("Mat_Des") & " (Lotto:" & drDetTec.Item("Cod_Articolo") & ")"
                                        End If

                                    Case SEMILAVORATI_VEGETALI

                                        If drDetTec.Item("Mat_Des") <> "" Then

                                            Select Case drDetTec.Item("Lav_Cod")
                                                Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
                                                    prod = "Semilavorato trattato:" & drDetTec.Item("Mat_Des")
                                                Case Else
                                                    prod = Resources.AgronicaAgenda_2010.SemilavoratoRaccolto & drDetTec.Item("Mat_Des")
                                            End Select
                                        End If

                                End Select


                                Dim drMovDetTec2() As DataRow = dtMovDetTec.Select("Ricetta_Operazione_Cod=" & roc)
                                Dim listaAvv2 As New List(Of String)

                                For Each drAvv2 As DataRow In drMovDetTec2
                                    If drAvv2.Item("Av_des_vol") <> "" Then
                                        listaAvv2.Add(drAvv2.Item("Av_des_vol"))
                                    End If
                                    If drAvv2.Item("Av_Gru_des") <> "" Then
                                        listaAvv2.Add(drAvv2.Item("Av_Gru_des"))
                                    End If
                                Next

                                avv = String.Join(", ", listaAvv2)

                                'PRINCIPI ATTIVI / SOSTANZE ATTIVE
                                Dim codiciPrincAtt As String = "" 'cod1§titolo1|cod2§titolo2

                                If drDetTec.Item("PrincipiAttivi") <> "" Then
                                    codiciPrincAtt = drDetTec.Item("PrincipiAttivi")
                                Else
                                    If Not IsDBNull(drDetTec.Item("Pro_Cod")) AndAlso drDetTec.Item("Pro_Cod") <> 0 _
                                                AndAlso Not IsNothing(HtProdPA(drDetTec.Item("Pro_Cod"))) Then
                                        codiciPrincAtt = HtProdPA(drDetTec.Item("Pro_Cod"))
                                    End If
                                End If

                                Dim listaPrincAtt() As String = codiciPrincAtt.Split("|")
                                Dim listaPrincAttNomi As New List(Of String)
                                For Each pa As String In listaPrincAtt
                                    listaPrincAttNomi.Add(HtPrincAtt(pa.Split("§")(0))) 'estraggo il codice numerico e ricerco la stringa
                                Next
                                princAtt = String.Join(", ", listaPrincAttNomi)

                                'AGGIUNGO ALLA LISTA SE NON ESISTE GIA'
                                Dim testoDetTec As String = String.Join(" - ", {prod, princAtt, avv}.Where(Function(s) Not String.IsNullOrEmpty(s)))
                                If Not listaDetTec.Contains(testoDetTec) Then
                                    listaDetTec.Add(testoDetTec)
                                End If

                            Next

                            dr.Item("Dettaglio_Tecnico") = String.Join(", ", listaDetTec)

                            '-------------------------------------------------------------------------------------------------------------------

                            'Superficie Trattata
                            Dim Sup_TrattataTot As Decimal = 0
                            Dim dbUtil As New AgronicaCoreDataProvider.DatatableUtility
                            Dim strID_Reg_Prima_Appezza(,) As String = dbUtil.SelectDistinct(dtRicettaSingola, "APPEZZA", "ID_REG", False)

                            For w = 0 To strID_Reg_Prima_Appezza.Length / 2 - 1
                                Dim drAppezza() As DataRow = dtRicettaSingola.Select("APPEZZA=" & strID_Reg_Prima_Appezza(w, 0) & " AND ID_REG=" & strID_Reg_Prima_Appezza(w, 1) & " ")

                                If drAppezza.Length > 0 Then
                                    Sup_TrattataTot += If(drAppezza(0).Item("sup_trattata") <> 0, CDec(drAppezza(0).Item("Sup_Trattata")), CDec(drAppezza(0).Item("sup_app")))
                                End If
                            Next

                            dr.Item("Sup_Trattata") = Sup_TrattataTot

                            '-------------------------------------------------------------------------------------------------------------------

                            dtRisultato.ImportRow(dr)

                        Next

                    End If
                End If
                r.RispostaOK = True
                r.RispostaStringa = DT_to_Json_Ricette(dtRisultato, stato, objParametri_Server)
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    Public Shared Function DT_to_Json_Ricette(dt As DataTable, stato As enum_WWorflow_WAnagraficaStati, objParametri_Server As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'l.Add(New ColonneNome("WAnagraficaStati_Des", "Stato", "string"))
        l.Add(New ColonneNome("WAnagraficaStati_Cod", "WAnagraficaStati_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("WAnagraficaStati_Colore", "WAnagraficaStati_Colore", "string") With {._hidden = True})

        l.Add(New ColonneNome("piva", Gias.Piva, "string") With {._Display = False})
        l.Add(New ColonneNome("Sa_Cod", "sa_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("sa_nome", Gias.CentroAziendale, "string") With {._Display = False}) '"Centro Aziendale"
        l.Add(New ColonneNome("veg_cod_op", "veg_cod_op", "number") With {._hidden = True})
        l.Add(New ColonneNome("veg_cod_r", "veg_cod", "number") With {._hidden = True})
        'l.Add(New ColonneNome("veg_des_unificato", "Specie", "string") With {._Display = False})
        l.Add(New ColonneNome("veg_des_unificato", Gias.Specie, "string")) '"Specie"
        l.Add(New ColonneNome("Ricetta_Numero", Gias.CodiceRicetta, "string"))
        l.Add(New ColonneNome("Ricetta_Des", Gias.Descrizione, "string") With {._Display = False})
        l.Add(New ColonneNome("Tipo_Ricetta", "Tipo_Ricetta", "number") With {._hidden = True})
        l.Add(New ColonneNome("Tipo_Ricetta_des", Gias.TipoRicetta, "string") With {._Display = False})
        l.Add(New ColonneNome("Validita_Inizio", Gias.ValiditaInizio, "date") With {._Display = False})
        l.Add(New ColonneNome("Validita_Fine", Gias.ValiditaFine, "date") With {._Display = False})
        l.Add(New ColonneNome("Ricetta_Operazione_Cod", "ID", "number") With {._Display = False})

        l.Add(New ColonneNome("Ricetta_Operazione_Data", Gias.DataOperazione, "date")) ' "Data Operazione"
        l.Add(New ColonneNome("lav_cod", "lav_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("lav_des", Gias.Operazione, "string")) ' "Operazione"
        l.Add(New ColonneNome("Ricetta_Cod", "Ricetta_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("Ricetta_Operazione_Cod", "Ricetta_Operazione_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("App_Nome", Gias.Appezzamenti, "string")) ' "Appezzamenti"
        l.Add(New ColonneNome("Dettaglio_Tecnico", Gias.DettaglioTecnico, "string")) '
        l.Add(New ColonneNome("Sup_Trattata", Gias.SuperficieMovimentata, "number") With {._Display = False, ._FormatoParticolare = "#=(Sup_Trattata === 0) ? '' : Sup_Trattata.toString().replace('.', ',')#"})
        l.Add(New ColonneNome("Costi_Operatori", Gias.Operatori, "string")) 'With {._Display = False})
        l.Add(New ColonneNome("Costi_Macchine", Gias.Macchine, "string")) 'With {._Display = False})
        l.Add(New ColonneNome("APP_Ricetta_Operazione_ID", "APP_Ricetta_Operazione_ID", "string") With {._hidden = True})

        If stato = enum_WWorflow_WAnagraficaStati.Esecuzione_ed_avanzamento_delle_ricette_Eseguita Then
            l.Add(New ColonneNome("Origine", Gias.Origine, "string"))
        End If

        l.Add(New ColonneNome("in_uso", "in_uso", "string") With {._hidden = True})
        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("blocco_flag", "blocco_flag", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", Gias.DescrizioneUnica, "string") With {._Display = False, ._RemoveHtmlEncode = True})

        l.Add(New ColonneNome("Data_Creazione", Gias.DataCreazione, "date") With {._Display = False})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaColturali(ByVal filtro As String) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda = New ParametriAgenda

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If

        Try

            Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)

            Dim cul_cod As Integer = 0
            Dim tipo As String = ""
            Dim gru_cod As Integer = 0
            Dim lav_cod As Integer = 0

            Dim flag_TerrenoNudo As Boolean = False

            Dim sa_cod As Integer = If(IsNumeric(jSonDatiTESTATA("sa_cod")), jSonDatiTESTATA("sa_cod"), 0)
            Dim veg_cod As Integer = If(IsNumeric(jSonDatiTESTATA("veg_cod")), jSonDatiTESTATA("veg_cod"), 0)

            Dim sData_Selezionata1 As String = jSonDatiTESTATA("txt_Data1").ToString
            Dim Data_Selezionata1 As DateTime = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

            Dim sData_Selezionata2 As String = jSonDatiTESTATA("txt_Data2").ToString
            Dim Data_Selezionata2 As DateTime = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

            caricaInSessioneImpostazioniutente()

            Dim dtAgenda As DataTable = MenuBS_Lavorazioni.Carica_Operazioni_Colturali(
                                                                                        Piva:=objParametriAgenda.Piva,
                                                                                        Sa_Cod:=sa_cod,
                                                                                        DataDa:=Data_Selezionata1,
                                                                                        DataA:=Data_Selezionata2,
                                                                                        Veg_Cod:=veg_cod,
                                                                                        Cul_Cod:=cul_cod,
                                                                                        Tipo:=tipo,
                                                                                        Gru_Cod:=gru_cod,
                                                                                        Lav_Cod:=lav_cod,
                                                                                        Flag_TerrenoNudo:=flag_TerrenoNudo,
                                                                                        xOrderBy:="",
                                                                                        objparametri_Server:=objParametri_Server,
                                                                                        objparametri_Utenti:=objParametri_Utenti
                                                                                        )

            r.RispostaStringa = DT_to_Json_Colturali(dtAgenda, objParametri_Server)
            r.RispostaOK = True


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaMagCont(ByVal filtro As String) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda = New ParametriAgenda

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If

        Try

            Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)

            Dim gru_cod As Integer = 0
            Dim lav_cod As Integer = 0

            Dim sa_cod As Integer = If(IsNumeric(jSonDatiTESTATA("sa_cod")), jSonDatiTESTATA("sa_cod"), 0)

            Dim sData_Selezionata1 As String = jSonDatiTESTATA("txt_Data1").ToString
            Dim Data_Selezionata1 As DateTime = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

            Dim sData_Selezionata2 As String = jSonDatiTESTATA("txt_Data2").ToString
            Dim Data_Selezionata2 As DateTime = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

            caricaInSessioneImpostazioniutente()

            Dim dtAgenda As DataTable = MenuBS_Lavorazioni.Carica_Operazioni_MagCont(
                                                                                    Piva:=objParametriAgenda.Piva,
                                                                                    Sa_Cod:=sa_cod,
                                                                                    DataDa:=Data_Selezionata1,
                                                                                    DataA:=Data_Selezionata2,
                                                                                    Gru_Cod:=gru_cod,
                                                                                    Lav_Cod:=lav_cod,
                                                                                    xOrderBy:="",
                                                                                    objparametri_Server:=objParametri_Server,
                                                                                    objparametri_Utenti:=objParametri_Utenti
                                                                                    )

            r.RispostaStringa = DT_to_Json_MagCont(dtAgenda, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaAudit(ByVal filtro As String) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda = New ParametriAgenda

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If

        Try

            Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)

            Dim sa_cod As Integer = 0
            Dim gru_cod As Integer = 0
            Dim lav_cod As Integer = 0

            If IsNumeric(jSonDatiTESTATA("sa_cod")) Then
                sa_cod = jSonDatiTESTATA("sa_cod")
            End If

            Dim sData_Selezionata1 As String = jSonDatiTESTATA("txt_Data1").ToString
            Dim Data_Selezionata1 As DateTime = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

            Dim sData_Selezionata2 As String = jSonDatiTESTATA("txt_Data2").ToString
            Dim Data_Selezionata2 As DateTime = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

            caricaInSessioneImpostazioniutente()

            Dim dtAgenda As DataTable = MenuBS_Lavorazioni.Carica_Operazioni_Audit(
                                                                                    Piva:=objParametriAgenda.Piva,
                                                                                    Sa_Cod:=sa_cod,
                                                                                    DataDa:=Data_Selezionata1,
                                                                                    DataA:=Data_Selezionata2,
                                                                                    Gru_Cod:=gru_cod,
                                                                                    Lav_Cod:=lav_cod,
                                                                                    xOrderBy:="",
                                                                                    objparametri_Server:=objParametri_Server,
                                                                                    objparametri_Utenti:=objParametri_Utenti
                                                                                    )

            r.RispostaStringa = DT_to_Json_Audit(dtAgenda, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaMacchine(ByVal filtro As String) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda = New ParametriAgenda

        Dim r As New MenuBS_Risposta
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If

        Try

            Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)

            Dim sa_cod As Integer = 0
            Dim gru_cod As Integer = 0
            Dim lav_cod As Integer = 0

            If IsNumeric(jSonDatiTESTATA("sa_cod")) Then
                sa_cod = jSonDatiTESTATA("sa_cod")
            End If

            Dim sData_Selezionata1 As String = jSonDatiTESTATA("txt_Data1").ToString
            Dim Data_Selezionata1 As DateTime = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

            Dim sData_Selezionata2 As String = jSonDatiTESTATA("txt_Data2").ToString
            Dim Data_Selezionata2 As DateTime = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

            caricaInSessioneImpostazioniutente()

            Dim dtAgenda As DataTable = MenuBS_Lavorazioni.Carica_Operazioni_Macchine(
                                                                                    Piva:=objParametriAgenda.Piva,
                                                                                    Sa_Cod:=sa_cod,
                                                                                    DataDa:=Data_Selezionata1,
                                                                                    DataA:=Data_Selezionata2,
                                                                                    Gru_Cod:=gru_cod,
                                                                                    Lav_Cod:=lav_cod,
                                                                                    xOrderBy:="",
                                                                                    objparametri_Server:=objParametri_Server,
                                                                                    objparametri_Utenti:=objParametri_Utenti
                                                                                    )

            r.RispostaStringa = DT_to_Json_Macchine(dtAgenda, objParametri_Server)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaZoo(ByVal filtro As String) As MenuBS_Risposta

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda = New ParametriAgenda

        Lingua.Gias_InizializzaCultura_DaSession()

        If Utils.LEGACY_SWITCH_USECOREWS Then
            Dim r As New MenuBS_Risposta
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim bus As New AgronicaCoreContabBIZ.MovimentiNG(objParametri_Server, objParametri_Utenti) ' bussiness layer
            r = bus.CaricaZoo(filtro, objParametriAgenda.Piva)
            Return r
        Else


            Dim r As New MenuBS_Risposta
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
            End If

            Try

                'Inserire il codice QUI..

                Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)


                Dim dtAgenda As DataTable

                Dim piva As String = objParametriAgenda.Piva
                Dim sa_cod As Integer = 0
                Dim validita_inizio As DateTime = CostantiPersonalizzate.AGRODATAINIZIO
                Dim validita_fine As DateTime = CostantiPersonalizzate.AGRODATAFINE

                Dim sData_Selezionata1 As String
                Dim Data_Selezionata1 As DateTime

                Dim sData_Selezionata2 As String
                Dim Data_Selezionata2 As DateTime

                Dim TipoGriglia As String

                Try
                    TipoGriglia = jSonDatiTESTATA("TipoGriglia").ToString
                Catch ex As Exception
                End Try

                If IsNumeric(jSonDatiTESTATA("sa_cod")) Then
                    sa_cod = jSonDatiTESTATA("sa_cod")
                End If

                sData_Selezionata1 = jSonDatiTESTATA("txt_Data1").ToString
                Data_Selezionata1 = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

                sData_Selezionata2 = jSonDatiTESTATA("txt_Data2").ToString
                Data_Selezionata2 = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

                Dim objOperazioni As New AgronicaCoreAnagrafeDAL.Operazioni_R

                caricaInSessioneImpostazioniutente()

                dtAgenda = MenuBS_Lavorazioni.Carica_Operazioni_Zootecniche(
                    objParametriAgenda.Piva,
                    sa_cod,
                    Data_Selezionata1,
                    Data_Selezionata2,
                    "",
                    objParametri_Server,
                    objParametri_Utenti
                    )

                r.RispostaOK = True
                If dtAgenda.Rows.Count > 0 OrElse TipoGriglia = "2" Then

                    If TipoGriglia = "2" Then
                        EstendiDatatable(dtAgenda)
                    End If

                    HttpContext.Current.Session("dt") = dtAgenda
                    r.RispostaStringa = DT_to_Json_AziendaZoo(dtAgenda, TipoGriglia, objParametri_Server)

                Else
                    r.RispostaStringa = "Zero"
                End If



            Catch ex As Exception
                r.RispostaOK = False
                r.Errore = "Errore durante l'operazione: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, False)

            End Try

            Return r
        End If
    End Function

    Public Shared Function DT_to_Json_Zoo(ByVal dt As DataTable, ByVal objParametri_Server As AgronicaCoreParametri) As String

        'creo la lista delle colonne da visualizzare
        Dim l As New List(Of ColonneNome)

        'l.Add(New ColonneNome("WAnagraficaStati_Des", "Stato", "string"))
        l.Add(New ColonneNome("WAnagraficaStati_Cod", "WAnagraficaStati_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("WAnagraficaStati_Colore", "WAnagraficaStati_Colore", "string") With {._hidden = True})

        l.Add(New ColonneNome("piva", "piva", "string") With {._Display = False})
        l.Add(New ColonneNome("Sa_Cod", "sa_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("sa_nome", "Centro Aziendale", "string") With {._Display = False})
        l.Add(New ColonneNome("veg_cod_op", "veg_cod_op", "number") With {._hidden = True})
        l.Add(New ColonneNome("veg_cod_r", "veg_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("veg_des_unificato", "Specie", "string") With {._Display = False})
        l.Add(New ColonneNome("Ricetta_Numero", "Codice Ricetta", "string"))
        l.Add(New ColonneNome("Ricetta_Des", "Descrizione", "string") With {._Display = False})
        l.Add(New ColonneNome("Tipo_Ricetta", "Tipo_Ricetta", "number") With {._hidden = True})
        l.Add(New ColonneNome("Tipo_Ricetta_des", "Tipo Ricetta", "string") With {._Display = False})
        l.Add(New ColonneNome("Validita_Inizio", "Validità Inizio", "date") With {._Display = False})
        l.Add(New ColonneNome("Validita_Fine", "Validità Fine", "date") With {._Display = False})
        l.Add(New ColonneNome("Ricetta_Cod", "ID", "number") With {._Display = False})

        l.Add(New ColonneNome("Ricetta_Operazione_Data", "Data Operazione", "date"))
        l.Add(New ColonneNome("lav_cod", "lav_cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("lav_des", "Operazione", "string"))
        l.Add(New ColonneNome("Ricetta_Operazione_Cod", "Ricetta_Operazione_Cod", "number") With {._hidden = True})
        l.Add(New ColonneNome("App_Nome", "Appezzamenti", "string"))
        l.Add(New ColonneNome("Dettaglio_Tecnico", "Dettaglio Tecnico", "string"))
        l.Add(New ColonneNome("Costi_Operatori", "Operatori", "string")) 'With {._Display = False})
        l.Add(New ColonneNome("Costi_Macchine", "Macchine", "string")) 'With {._Display = False})

        l.Add(New ColonneNome("in_uso", "in_uso", "string") With {._hidden = True})
        l.Add(New ColonneNome("PermessoModifica", "PermessoModifica", "string") With {._hidden = True})
        l.Add(New ColonneNome("Descrizione_Unica", "Descrizione Unica", "string") With {._Display = False, ._RemoveHtmlEncode = True})

        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim risp As String = js.JSON_DataTable_Kendo(dt, l, tipoFiltroKendo:=TipoFiltroKendo_colonne.CasellaTesto_e_CasellaDiscesa)

        Return risp

    End Function

    Private Shared Function estraiPrincipiAttivi(ByRef DtAgenda As DataTable, ByRef HtProdPA As Hashtable, objParametri_Server As AgronicaCoreParametri) As Hashtable

        Dim res As New Hashtable()

        Dim objSqlDis As New AgronicaCoreUtility.DatatableUtility

        'seleziono le righe nella tabella di appoggio relative ai principi attivi
        Dim dtPA As DataTable = DtAgenda.Clone()
        For Each dr As DataRow In DtAgenda.Rows
            If dr.Item("elem_cod") = FORMULATI Then
                dtPA.ImportRow(dr)
            End If
        Next

        Dim PrincipiSalvati As Boolean = False

        Dim strPACod() As String = objSqlDis.SelectDistinct(dtPA, "PrincipiAttivi")

        If Not IsNothing(strPACod) AndAlso strPACod.Length > 0 Then

            'se i principi sono salvati tutti (cod1§titolo1|cod2§titolo2)
            'leggo in locale le descrizioni dei principi attivi
            If strPACod(0) <> "" Then
                PrincipiSalvati = True
                Dim Principi() As String
                Dim strElencoPACOD As String = ""
                For Each strPa_Cod As String In strPACod
                    Principi = Split(strPa_Cod, "|")
                    If Not IsNothing(Principi) Then
                        For Each p As String In Principi
                            strElencoPACOD &= Split(p, "§")(0) & ","
                        Next
                    End If
                Next
                If strElencoPACOD <> "" Then
                    Dim objPA As New AgronicaCoreMetaSchemaDAL.PrincipiAttivi_R
                    Dim DtPrincipiDes As DataTable = objPA.Leggi_Da_StrPa_Cod(Left(strElencoPACOD, strElencoPACOD.Length - 1), AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "pa_cod", objParametri_Server)

                    For Each dr As DataRow In DtPrincipiDes.Rows
                        If Not res.ContainsKey(dr.Item("PA_Cod")) Then
                            res.Add(dr.Item("PA_Cod").ToString, dr.Item("PA_Des"))
                        End If
                    Next
                End If
            End If
        End If

        'se almeno un campo è vuoto
        'leggo i principi da web service (con una sola chiamata per tutti i formulati)
        If PrincipiSalvati = False Then
            'ottengo i formulati distinti
            Dim ElencoFormulati As String = ""
            Dim strFrCod() As String = objSqlDis.SelectDistinct(dtPA, "pro_cod")
            If Not strFrCod Is Nothing Then
                ElencoFormulati = String.Join(",", strFrCod)
            End If
            If ElencoFormulati <> "" Then
                Dim objAgroWs As New AgronicaCoreWebService.AgroWs
                Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
                Dim DtPrincipi As DataTable = objAgroWs.ComposizioneFormulatiRecupera(ElencoFormulati, objParametri_Server, objParametri_Utenti)

                'Estraggo i PrincipiAttivi (cod - des)
                For Each dr As DataRow In DtPrincipi.Rows
                    Dim testo As String = dr.Item("Elenco_PrincipiAttivi")
                    If testo <> "" Then
                        Dim elenco As String() = Split(testo, "|")
                        For Each elem As String In elenco
                            Dim datiElem As String() = Split(elem, "§")
                            If datiElem.Count > 2 AndAlso Not res.ContainsKey(datiElem(0)) Then
                                res.Add(datiElem(0).ToString, datiElem(1))
                            End If
                        Next
                    End If
                Next

                'Estraggo i PrincipiAttivi (Fr_Cod - principiAttivi)
                HtProdPA = New Hashtable()
                For Each dr As DataRow In DtPrincipi.Rows
                    If Not HtProdPA.ContainsKey(dr.Item("Fr_Cod")) Then
                        HtProdPA.Add(dr.Item("Fr_Cod"), dr.Item("Elenco_PrincipiAttivi"))
                    End If
                Next

                objAgroWs = Nothing
            End If
        End If

        Return res

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiNumeroOperazioniInTab(ByVal filtro As String) As RispostaStandard

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim objParametriAgenda = New ParametriAgenda

        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
        End If

        Dim jSonDatiTESTATA As JObject = JObject.Parse(filtro)

        Dim sa_cod As Integer = If(IsNumeric(jSonDatiTESTATA("sa_cod")), jSonDatiTESTATA("sa_cod"), 0)
        Dim veg_cod As Integer = If(IsNumeric(jSonDatiTESTATA("veg_cod")), jSonDatiTESTATA("veg_cod"), 0)
        Dim gru_cod As Integer = 0

        Dim sData_Selezionata1 As String = jSonDatiTESTATA("txt_Data1").ToString
        Dim Data_Selezionata1 As DateTime = If(IsDate(sData_Selezionata1), CDate(sData_Selezionata1), AGRODATAINIZIO)

        Dim sData_Selezionata2 As String = jSonDatiTESTATA("txt_Data2").ToString
        Dim Data_Selezionata2 As DateTime = If(IsDate(sData_Selezionata2), CDate(sData_Selezionata2), AGRODATAFINE)

        Dim Filtro_Tipo_GruppoOperazioni As String = Leggi_Filtro_Tipo_GruppoOperazioni()
        Dim Filtro_Utente_Lavorazioni As String = Leggi_Filtro_Utente_Lavorazioni()

        Dim read As New AgronicaCoreAnagrafeDAL.Operazioni_R
        Dim dt As DataTable = read.Leggi_Numero_Operazioni_PerMenuAgenda(objParametriAgenda.Piva, sa_cod, Data_Selezionata1, Data_Selezionata2, veg_cod, gru_cod,
                                                                         Filtro_Utente_Lavorazioni, Filtro_Tipo_GruppoOperazioni, objParametri_Utenti, objParametri_Server)

        Dim JArrayListaOp As New JArray()
        For Each dc As DataColumn In dt.Columns
            JArrayListaOp.Add(New JObject(New JProperty("nome_tab", dc.ColumnName), New JProperty("conteggio", dt.Rows(0).Item(dc))))
        Next

        r.RispostaStringa = JsonConvert.SerializeObject(JArrayListaOp, Formatting.None)
        r.RispostaOK = True

        Return r

    End Function
#End Region

#Region "Certificazione"

    Private Shared Function ControllaCertificazione(ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean, ByVal Piva As String,
            ByVal Sa_Cod As Integer, ByVal id_mov_det As Integer,
            ByVal DtTutte As DataTable, ByVal DtCertificate As DataTable, ByVal cCertificazione As Integer, ByVal objparametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, ByVal objParametri_utenti As AgronicaCoreParametri) As DataTable

        ' dtRigheAggiuntive conterrà tutte le righe trovate in aggiunta
        Dim dtRigheAggiuntive As DataTable = DtTutte.Clone()
        Dim tutteCertificate As Boolean = True
        Dim mdR As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim calCodElottiTestati As New List(Of String)
        ' Legge la riga scelta
        Dim dt As DataTable = mdR.MovimentiDettagli_Leggi_FF(
                                 Piva, Sa_Cod, "", 0, 0, id_mov_det, 0,
                                 0, 0, 0, 0, "-999", 0, 0, Nothing,
                                     Nothing, 0, 0, "", "", "", "", objparametri_Server)

        If dt.Rows.Count = 1 Then

            ' Memorizza la quantità della riga scelta
            Dim totKgVenduti As Decimal = dt.Rows().Item(0)("Qta_Extra_Totale")

            ' Legge altre righe con stesso prodotto / calibro / qualità / certificazione presenti
            ' nello stesso DDT della riga scelta
            Dim xFiltroAggiuntivo As String = " And OTabelle_Parametri_Certificazioni.Tabella_Par_Cod = " & CInt(dt.Rows().Item(0)("Certificazioni_Cod")) & " "
            xFiltroAggiuntivo += " And OTabelle_Parametri_Calibro.Tabella_Par_Cod = " & CInt(dt.Rows().Item(0)("Calibro_Cod")) & " "
            xFiltroAggiuntivo += " And OTabelle_Parametri_Qualità.Tabella_Par_Cod = " & CInt(dt.Rows().Item(0)("Qualità_Cod")) & " "
            xFiltroAggiuntivo += " And Movimenti_Dettagli.Id_Mov_Det <> " & id_mov_det & " "
            Dim dtRigheStessoDDT As DataTable = mdR.MovimentiDettagli_Leggi_FF(
                                 Piva, Sa_Cod, "", CInt(dt.Rows().Item(0)("Id_Agenda")),
                                 CInt(dt.Rows().Item(0)("Id_Mov")), 0,
                                 CInt(dt.Rows().Item(0)("Elem_Cod")), 0, CInt(dt.Rows().Item(0)("Mat_Cod")),
                                 0, 0, "-999", 0, 0, Nothing,
                                     Nothing, 0, 0, "", "", xFiltroAggiuntivo, "", objparametri_Server)
            If dtRigheStessoDDT.Rows.Count > 0 Then
                For Each riga In dtRigheStessoDDT.Rows()
                    ' Aggiunge alla quantità della riga scelta le quantità delle altre righe dello stesso DDT
                    totKgVenduti += riga("Qta_Extra_Totale")
                Next
            End If

            'totKgTrovati conterrà i Kg di altre righe aggiuntive
            Dim totaliPerFase As New Hashtable
            totaliPerFase.Add("Entrata", 0)
            ' Se ci sono lavorazioni aggiungo all'HashTable in modo da andare a cercare di coprire anche quelle
            For Each riga In DtTutte.Rows()
                If Not String.IsNullOrEmpty(riga("FF_codice_generazione")) AndAlso
                    Not totaliPerFase.ContainsKey(riga("FF_codice_generazione")) Then
                    totaliPerFase.Add(riga("FF_codice_generazione"), 0)
                End If
            Next

            'Cerca il totale delle righe già tracciate in precedenza dividendo i totali per entrata e per fasi di lavorazione
            Dim lavCodConf As System.Nullable(Of Integer)() = {LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}
            For Each riga In DtCertificate.Rows()
                If lavCodConf.Contains(CInt(riga("Lav_Cod"))) Then
                    totaliPerFase("Entrata") = totaliPerFase("Entrata") + riga("FF_Track_Qta_Extra_Totale")
                Else
                    If totaliPerFase.ContainsKey(riga("FF_codice_generazione")) Then
                        totaliPerFase(riga("FF_codice_generazione")) = totaliPerFase("Entrata") + riga("FF_Track_Qta_Extra_Totale")
                    Else
                        totaliPerFase.Add(riga("FF_codice_generazione"), riga("FF_Track_Qta_Extra_Totale"))
                    End If
                End If
            Next

            ' Traccia ogni riga dello stesso DDT della riga scelta a parità di prodotto / calibro / qualità / certificazione 
            If dtRigheStessoDDT.Rows.Count > 0 Then
                For Each riga In dtRigheStessoDDT.Rows()
                    'Lettura di dati dettaglio rintracciabilità
                    Dim dtRigheTracciateStessoDDT As DataTable = TrackMeToDataTableCertificazione(ForzaNuovaEsecuzioneAlgoritmoRintraccia, True, Piva, cCertificazione, riga("Cal_Cod"), riga("Id_Mov_Det"), riga("Lotto"), "", objparametri_Server, objParametri_utenti)

                    Dim DvRigheTracciateStessoDDT As New DataView
                    Dim DtRigheCertificateTracciateStessoDDT As New DataTable
                    DvRigheTracciateStessoDDT.Table = dtRigheTracciateStessoDDT
                    DvRigheTracciateStessoDDT.RowFilter = "FF_certificazioni_Codice = " & cCertificazione & ""
                    DtRigheCertificateTracciateStessoDDT = DvRigheTracciateStessoDDT.ToTable

                    For Each rigaCertTracciateStessoDDT In DtRigheCertificateTracciateStessoDDT.Rows()
                        'Verifico di non aver già trovato ai giri precedenti ogni riga presente nell'insieme di righe di tracciabilità
                        Dim trovataRigaUguale As Boolean = False
                        For Each rigaAgg In DtTutte.Rows()
                            If rigaCertTracciateStessoDDT("Id_Mov_Det") = rigaAgg("Id_Mov_Det") Then
                                trovataRigaUguale = True
                            End If
                        Next
                        For Each rigaAgg In dtRigheAggiuntive.Rows()
                            If rigaCertTracciateStessoDDT("Id_Mov_Det") = rigaAgg("Id_Mov_Det") Then
                                trovataRigaUguale = True
                            End If
                        Next
                        ' Se non l'ho trovata aggiungo la riga a quelle trovate
                        If Not trovataRigaUguale Then
                            If lavCodConf.Contains(CInt(rigaCertTracciateStessoDDT("Lav_Cod"))) Then
                                totaliPerFase("Entrata") = totaliPerFase("Entrata") + rigaCertTracciateStessoDDT("FF_Track_Qta_Extra_Totale")
                            Else
                                If rigaCertTracciateStessoDDT("FF_certificazioni_Codice") = cCertificazione Then
                                    If totaliPerFase.ContainsKey(rigaCertTracciateStessoDDT("FF_codice_generazione").ToString()) Then
                                        totaliPerFase(rigaCertTracciateStessoDDT("FF_codice_generazione").ToString()) = totaliPerFase(rigaCertTracciateStessoDDT("FF_codice_generazione")) + rigaCertTracciateStessoDDT("FF_Track_Qta_Extra_Totale")
                                    Else
                                        totaliPerFase.Add(rigaCertTracciateStessoDDT("FF_codice_generazione").ToString(), rigaCertTracciateStessoDDT("FF_Track_Qta_Extra_Totale"))
                                    End If
                                End If
                            End If
                            dtRigheAggiuntive.ImportRow(rigaCertTracciateStessoDDT)
                        End If
                    Next
                Next
            End If

            ' Se ancora non copro con l'entrata ...
            If totaliPerFase("Entrata") < totKgVenduti Then

                ' kgMancanti viene passata a tutti i cicli sotto fino a quando non avrò coperto 
                Dim kgMancanti = totKgVenduti - totaliPerFase("Entrata")

                'Primo giro: tutte righe certificate
                '  Per ora sospeso: il problema è che farò molto prima a trovare righe di entrata che righe di lavorazione certificate
                '  e rischierei di girare in eterno
                '''tutteCertificate = True
                '''TrovaRigheCertificate(tutteCertificate, kgMancanti, dtRigheAggiuntive, Piva, Sa_Cod,
                '''    DtTutte, DtCertificate, cCertificazione, calCodElottiTestati, objparametri_Server)

                If kgMancanti > 0 Then
                    'Secondo giro: solo entrate certificate
                    tutteCertificate = False
                    calCodElottiTestati.Clear()
                    TrovaRigheCertificate(ForzaNuovaEsecuzioneAlgoritmoRintraccia, tutteCertificate, kgMancanti, dtRigheAggiuntive, Piva, Sa_Cod,
                    DtTutte, DtCertificate, cCertificazione, calCodElottiTestati, objparametri_Server, objParametri_utenti)
                End If

            End If

            ' Cerco di compensare movimenti di Confezionamento (-137) e Calibratura (-133)
            If totaliPerFase.ContainsKey("-137") Or totaliPerFase.ContainsKey("-133") Then

                'Data limite è sempre quella del movimento di vendita
                Dim UltimaData As Date = CDate(dt.Rows().Item(0)("Data_Movimento"))

                Dim xSelectAggiuntiva As String = ""
                Dim xOrderBy As String = ""
                Dim lavCodConf_LAVCOD_TRASFORMAZIONI As Integer() = {LAVCOD_TRASFORMAZIONI}
                Dim Cau_MovArray As String() = {CAU_CARICO}
                xOrderBy = " ORDER BY Agenda.PIVA, Qta_Extra_Totale DESC, Movimenti.Data_Movimento DESC"

                ' Cerco a parità di certificazione, linea prodotto, data movimento < data movimento di vendita
                xFiltroAggiuntivo = ""
                xFiltroAggiuntivo += " And OTabelle_Parametri_Certificazioni.Tabella_Par_Cod = " & cCertificazione
                xFiltroAggiuntivo += " And Movimenti.Data_Movimento <= '" & UltimaData & "' "

                'Cerco i movimenti di confezionamento per arrivare alla copertura del venduto
                'Parto da quelli di confezionamento perchè tracciando questi potrei poi trovare movimenti di calibratura
                ' collegati che vanno bene
                If totaliPerFase.ContainsKey("-137") AndAlso totaliPerFase("-137") < totKgVenduti Then
                    Dim LineaCodList As New List(Of Integer)
                    For Each ra In dtRigheAggiuntive.Rows
                        If ra("FF_codice_generazione") = -137 Then
                            If Not LineaCodList.Contains(CInt(ra("FF_Linea_Cod"))) Then
                                LineaCodList.Add(ra("FF_Linea_Cod"))
                            End If
                        End If
                    Next

                    If LineaCodList.Count > 0 Then

                        Dim xFiltroAggiuntivo137 As String = xFiltroAggiuntivo + " And lp.codice_generazione = -137 "

                        Dim primoGiro As Boolean = True
                        xFiltroAggiuntivo137 += " And Materie_Prime.Linea_Cod IN ( "
                        For Each linea In LineaCodList
                            If Not primoGiro Then
                                xFiltroAggiuntivo137 = xFiltroAggiuntivo137 & " , "
                            End If
                            primoGiro = False
                            xFiltroAggiuntivo137 = xFiltroAggiuntivo137 & linea
                        Next
                        xFiltroAggiuntivo137 += " ) "

                        Dim kgMancanti = totKgVenduti - totaliPerFase("-137")
                        calCodElottiTestati.Clear()

                        Dim DtTrace As DataTable = CercaRigheAlternative(True, ForzaNuovaEsecuzioneAlgoritmoRintraccia, Piva, cCertificazione,
                                                                             "", Cau_MovArray, 0, kgMancanti,
                                                          xSelectAggiuntiva, "", xFiltroAggiuntivo137, xOrderBy,
                                                                             calCodElottiTestati, "C", objparametri_Server, objParametri_utenti, False, totaliPerFase)
                        If Not DtTrace Is Nothing Then
                            For Each rigaTrace In DtTrace.Rows()

                                ' In questo giro accetto sia i movimenti di calibratura che di confezionamento
                                If rigaTrace("FF_codice_generazione") = -133 Or rigaTrace("FF_codice_generazione") = -137 Then
                                    Dim trovataRigaUguale As Boolean = False
                                    For Each rigaAgg In dtRigheAggiuntive.Rows()
                                        If rigaAgg("Id_Mov_Det") = rigaTrace("Id_Mov_Det") Then
                                            trovataRigaUguale = True
                                        End If
                                    Next
                                    If Not trovataRigaUguale Then

                                        ' Memorizzo la data meno recente di confezionamento perchè poi quando cercherò la calibratura
                                        ' dovrò trovare le righe precedenti
                                        If rigaTrace("FF_codice_generazione") = -137 AndAlso CDate(rigaTrace("Data2")) < UltimaData Then
                                            UltimaData = CDate(rigaTrace("Data_Movimento"))
                                        End If


                                        If totaliPerFase.ContainsKey(rigaTrace("FF_codice_generazione").ToString()) Then
                                            totaliPerFase(rigaTrace("FF_codice_generazione").ToString()) = totaliPerFase(rigaTrace("FF_codice_generazione").ToString()) + rigaTrace("FF_Track_Qta_Extra_Totale")
                                        Else
                                            totaliPerFase.Add(rigaTrace("FF_codice_generazione").ToString(), rigaTrace("FF_Track_Qta_Extra_Totale"))
                                        End If

                                        dtRigheAggiuntive.ImportRow(rigaTrace)

                                    End If
                                End If
                            Next
                        End If
                    End If
                End If

                ' TODO STEFANO Momentaneamente sospeso, perchè comunque bisognerebbe tracciare da un livello sopra 
                ' altrimenti la calibratura non compare
                ' Inoltre è troppo dispendioso in termini di tempo
                'If totaliPerFase.ContainsKey("-133") AndAlso totaliPerFase("-133") < totKgVenduti Then

                '    Dim LineaCodList As New List(Of Integer)
                '    For Each ra In dtRigheAggiuntive.Rows
                '        If ra("FF_codice_generazione") = -133 Then
                '            If Not LineaCodList.Contains(CInt(ra("FF_Linea_Cod"))) Then
                '                LineaCodList.Add(ra("FF_Linea_Cod"))
                '            End If
                '        End If
                '    Next

                '    If LineaCodList.Count > 0 Then

                '        Dim xFiltroAggiuntivo133 As String = xFiltroAggiuntivo + " And lp.codice_generazione = -133 "

                '        Dim primoGiro As Boolean = True
                '        xFiltroAggiuntivo133 += " And Materie_Prime.Linea_Cod IN ( "
                '        For Each linea In LineaCodList
                '            If Not primoGiro Then
                '                xFiltroAggiuntivo133 = xFiltroAggiuntivo133 & " , "
                '            End If
                '            primoGiro = False
                '            xFiltroAggiuntivo133 = xFiltroAggiuntivo133 & linea
                '        Next
                '        xFiltroAggiuntivo133 += " ) "

                '        Dim kgMancanti = totKgVenduti - totaliPerFase("-133")
                '        calCodElottiTestati.Clear()

                '        Dim DtTrace As DataTable = CercaRigheAlternative(True, ForzaNuovaEsecuzioneAlgoritmoRintraccia, Piva, cCertificazione,
                '                                                             "", Cau_MovArray, 0, kgMancanti,
                '                                          xSelectAggiuntiva, "", xFiltroAggiuntivo133, xOrderBy,
                '                                                             calCodElottiTestati, "C", objparametri_Server, False, totaliPerFase)
                '        If Not DtTrace Is Nothing Then
                '            For Each rigaTrace In DtTrace.Rows()

                '                ' In questo giro accetto solo i movimenti di calibratura
                '                If rigaTrace("FF_codice_generazione") = -133 Then
                '                    Dim trovataRigaUguale As Boolean = False
                '                    For Each rigaAgg In dtRigheAggiuntive.Rows()
                '                        If rigaAgg("Id_Mov_Det") = rigaTrace("Id_Mov_Det") Then
                '                            trovataRigaUguale = True
                '                        End If
                '                    Next
                '                    If Not trovataRigaUguale Then

                '                        If totaliPerFase.ContainsKey(rigaTrace("FF_codice_generazione").ToString()) Then
                '                            totaliPerFase(rigaTrace("FF_codice_generazione").ToString()) = totaliPerFase(rigaTrace("FF_codice_generazione").ToString()) + rigaTrace("FF_Track_Qta_Extra_Totale")
                '                        Else
                '                            totaliPerFase.Add(rigaTrace("FF_codice_generazione").ToString(), rigaTrace("FF_Track_Qta_Extra_Totale"))
                '                        End If

                '                        dtRigheAggiuntive.ImportRow(rigaTrace)

                '                    End If
                '                End If
                '            Next
                '        End If
                '    End If
                'End If

            End If

        End If
        Return dtRigheAggiuntive

    End Function

    Private Shared Function TrovaRigheCertificate(ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean, ByVal tutteCertificate As Boolean, ByRef kgMancanti As Decimal,
                                                    ByRef dtRigheAggiuntive As DataTable,
                                                    ByVal Piva As String, ByVal Sa_Cod As Integer,
                                                    ByVal DtTutte As DataTable, ByVal DtFiltrate As DataTable, ByVal cCertificazione As Integer,
                                                    ByRef calCodElottiTestati As List(Of String),
                                                    ByVal objparametri_Server As AgronicaCoreParametri, ByVal objparametri_Utenti As AgronicaCoreParametri) As Boolean

        For Each riga In DtTutte.Rows()

            Dim leggi As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

            Dim xSelectAggiuntiva As String = ""
            Dim xFiltroAggiuntivo As String = ""
            Dim xOrderBy As String = ""
            Dim Cau_MovArray As String() = {CAU_ACCETTAZIONE_BENI_DA_DIVERSI}
            Dim Cau_Agg As Integer = 0
            xSelectAggiuntiva += ""
            xOrderBy = " ORDER BY Agenda.PIVA, Qta_Extra_Totale DESC, Movimenti.Data_Movimento DESC"
            Dim lavCodConf As System.Nullable(Of Integer)() = {LAVCOD_DISTINTA_CARICO, LAVCOD_DISTINTA_CARICO_ACCETTAZIONE, LAVCOD_BOLLA_RICEVUTA, LAVCOD_ACCETTAZIONE_DIVERSI, LAVCOD_AUTO_DDT_EMESSO, LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE}

            If lavCodConf.Contains(CInt(riga("Lav_Cod"))) Then

                'Cerco i movimenti di entrata trovati al primo giro relativi al Lotto_Padre (che in questo caso è quello di entrata
                Dim dtEntrata As DataTable =
                            leggi.MovimentiDettagli_Leggi_FF(Piva, 0, "", 0, 0, 0, 210, 0, 0, 0, 0, riga("FF_Track_Lotto_Padre").ToString(),
                                                             0, 0, Nothing, Cau_MovArray, Cau_Agg, 0,
                                 xSelectAggiuntiva, "", "", xOrderBy,
                                 objparametri_Server)

                If Not dtEntrata Is Nothing Then

                    'Per ognuno cerco righe simili fino al raggiungimento del peso
                    For Each rigaEntrata In dtEntrata.Rows()
                        If kgMancanti > 0 Then

                            xFiltroAggiuntivo += " And Movimenti.Data_Movimento <= '" & CDate(rigaEntrata("Data_Movimento")) & "' "
                            xFiltroAggiuntivo += " And Movimenti_Dettagli.Elem_Cod = " & rigaEntrata("Elem_Cod").ToString & " "
                            xFiltroAggiuntivo += " And Movimenti_Dettagli.Mat_Cod = " & rigaEntrata("Mat_Cod").ToString & " "
                            xFiltroAggiuntivo += " And OTabelle_Parametri_Certificazioni.Tabella_Par_Cod = " & cCertificazione.ToString & " "

                            ' Prima lettura per prodotto + codice soggetto
                            Dim DtTrace As DataTable = CercaRigheAlternative(False, ForzaNuovaEsecuzioneAlgoritmoRintraccia, Piva, cCertificazione,
                                                                             rigaEntrata("Cod_Contatto").ToString(), Cau_MovArray, Cau_Agg, kgMancanti,
                                                          xSelectAggiuntiva, "", xFiltroAggiuntivo, xOrderBy,
                                                                             calCodElottiTestati, "W", objparametri_Server, objparametri_Utenti, tutteCertificate, Nothing)
                            If Not DtTrace Is Nothing Then
                                For Each rigaTrace In DtTrace.Rows()
                                    Dim trovataRigaUguale As Boolean = False
                                    For Each rigaAgg In dtRigheAggiuntive.Rows()
                                        If rigaAgg("Id_Mov_Det") = rigaTrace("Id_Mov_Det") Then
                                            trovataRigaUguale = True
                                        End If
                                    Next
                                    If Not trovataRigaUguale Then
                                        Dim lottoScambio As String = rigaTrace("FF_Track_Lotto")
                                        rigaTrace("FF_Track_Lotto") = rigaTrace("FF_Track_Lotto_Padre")
                                        rigaTrace("FF_Track_Lotto_Padre") = lottoScambio
                                        dtRigheAggiuntive.ImportRow(rigaTrace)
                                    End If
                                Next
                            End If



                            ' Seconda lettura senza codice soggetto
                            DtTrace = CercaRigheAlternative(False, ForzaNuovaEsecuzioneAlgoritmoRintraccia, Piva, cCertificazione, "", Cau_MovArray, Cau_Agg, kgMancanti,
                                                          xSelectAggiuntiva, "", xFiltroAggiuntivo, xOrderBy, calCodElottiTestati, "W", objparametri_Server, objparametri_Utenti, tutteCertificate, Nothing)
                            If Not DtTrace Is Nothing Then
                                For Each rigaTrace In DtTrace.Rows()
                                    Dim trovataRigaUguale As Boolean = False
                                    For Each rigaAgg In dtRigheAggiuntive.Rows()
                                        If rigaAgg("Id_Mov_Det") = rigaTrace("Id_Mov_Det") Then
                                            trovataRigaUguale = True
                                        End If
                                    Next
                                    If Not trovataRigaUguale Then
                                        Dim lottoScambio As String = rigaTrace("FF_Track_Lotto")
                                        rigaTrace("FF_Track_Lotto") = rigaTrace("FF_Track_Lotto_Padre")
                                        rigaTrace("FF_Track_Lotto_Padre") = lottoScambio
                                        dtRigheAggiuntive.ImportRow(rigaTrace)
                                    End If
                                Next
                            End If

                        End If
                    Next
                End If
            End If
        Next

        Return True
    End Function

    Private Shared Function CercaRigheAlternative(ByVal FromOutToIn As Boolean,
                                                  ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean, ByVal Piva As String, ByVal cCertificazione As Integer,
                                                  ByVal cod_contatto As String, ByVal Cau_MovArray As String(), ByVal Cau_Agg As Integer,
                                                  ByRef kgMancanti As Decimal, ByVal xSelectAggiuntiva As String, ByVal xJoinAggiuntiva As String,
                                                  ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String, ByRef calCodElottiTestati As List(Of String),
                                                  ByVal TipoRigheAggiunte As String,
                                                  ByVal objparametri_Server As AgronicaCoreParametri, ByVal objparametri_Utenti As AgronicaCoreParametri,
                                                  ByVal tutteCertificate As Boolean, ByVal totaliPerFase As Hashtable) As DataTable

        Dim leggi As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        Dim dtTraceAggiunte As DataTable
        Dim dtTrace As DataTable

        ' Trovo righe di entrata alternative certificate
        Dim dtAlternative As DataTable = leggi.MovimentiDettagli_Leggi_FF(Piva, 0, cod_contatto, 0, 0, 0, 210, 0, 0, 0, 0, "-999",
                                     0, 0, Nothing, Cau_MovArray, Cau_Agg, 0,
            xSelectAggiuntiva, xJoinAggiuntiva, xFiltroAggiuntivo, xOrderBy,
            objparametri_Server)

        If Not dtAlternative Is Nothing Then

            For Each rigaAltern In dtAlternative.Rows()

                If kgMancanti > 0 Then

                    Dim GiaProvato As Boolean = False
                    Dim codice As String = CStr(rigaAltern("Cal_Cod")) + "§§§" + CStr(rigaAltern("Id_Mov_Det")) + "§§§" + CStr(rigaAltern("Lotto"))
                    For Each objStr In calCodElottiTestati
                        If objStr = codice Then
                            GiaProvato = True
                        End If
                    Next
                    If Not GiaProvato Then

                        calCodElottiTestati.Add(CStr(rigaAltern("Cal_Cod")) + "§§§" + CStr(rigaAltern("Id_Mov_Det")) + "§§§" + CStr(rigaAltern("Lotto")))

                        Dim kgTrovati As Decimal = Decimal.Parse(rigaAltern("Qta_Extra_Totale"))

                        'Innesca nuovo giro di ricerca 
                        dtTrace = TrackMeToDataTableCertificazione(ForzaNuovaEsecuzioneAlgoritmoRintraccia, FromOutToIn, Piva, cCertificazione, CInt(rigaAltern("Cal_Cod")), CInt(rigaAltern("Id_Mov_Det")), CStr(rigaAltern("Lotto")), TipoRigheAggiunte, objparametri_Server, objparametri_Utenti)

                        If Not dtTrace Is Nothing Then

                            Dim righeLetteTutteOk As Boolean = True

                            If tutteCertificate Then
                                For Each rigaTrace In dtTrace.Rows()
                                    If String.IsNullOrEmpty(rigaTrace("FF_Certificazioni_Codice").ToString) OrElse CInt(rigaTrace("FF_Certificazioni_Codice")) <> cCertificazione Then
                                        righeLetteTutteOk = False
                                    End If
                                Next
                            End If

                            If righeLetteTutteOk Then
                                If kgMancanti > 0 Then
                                    kgMancanti -= kgTrovati
                                End If
                                If dtTraceAggiunte Is Nothing Then
                                    dtTraceAggiunte = dtTrace.Clone
                                End If
                                For Each dr In dtTrace.Rows
                                    dtTraceAggiunte.ImportRow(dr)
                                Next
                            End If
                        End If
                    End If
                End If
            Next
        End If

        Return dtTraceAggiunte

    End Function
    Private Shared Function TrackMeToDataTableCertificazione(ByVal ForzaNuovaEsecuzioneAlgoritmoRintraccia As Boolean, ByVal FromOutToIn As Boolean, ByVal Piva As String, ByVal cCertificazione As Integer, ByVal CalCodSelected As Integer, ByVal IdMovDetSelected As Integer, ByVal TrackCode As String, ByVal TipoRigheAggiunte As String, ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As DataTable
        Dim xDtrack As XDocument
        Dim sD As String

        Dim xForza As String = "0"
        If ForzaNuovaEsecuzioneAlgoritmoRintraccia Then
            xForza = "1"
        End If

        Dim xFromOutToIn As String = "0"
        If FromOutToIn Then
            xFromOutToIn = "1"
        End If

        sD = "<track><lotsToTrack><lot><inputData><lotto>" & TrackCode & "</lotto><tipolotto>" & "1" & "</tipolotto><ricarica>" & xForza & "</ricarica><xFromOutToIn>" & xFromOutToIn & "</xFromOutToIn><xCertificazione>" & cCertificazione & "</xCertificazione><xCalCodSelected>" & CalCodSelected & "</xCalCodSelected><xIdMovDetSelected>" & IdMovDetSelected & "</xIdMovDetSelected></inputData></lot></lotsToTrack></track>"

        Dim dtTrKOperazioniConf As String

        Dim trk As New AgronicaCoreContabBIZ.FF_Track

        'Qui viene lanciato il ciclo di letture o il reperimento da db se già lanciata in precedenza
        dtTrKOperazioniConf = trk.TrackMe(sD, False, False, objParametri_Server)

        xDtrack = XDocument.Parse(dtTrKOperazioniConf)

        Dim lXDtrack As IEnumerable(Of XElement) = xDtrack.<track>.<lotsToTrack>.<lot>.<outputData>.<trasformazioni>.<trasformazione_dati>

        Dim FF_TrackedData_Cod As String =
            (From iAg In xDtrack.<track>.<lotsToTrack>.<lot>.<inputData>.<FF_TrackedData_Cod> Select iAg.Value).FirstOrDefault

        Dim sa_cod As Integer = 0
        Dim validita_inizio As DateTime = AGRODATAINIZIO
        Dim validita_fine As DateTime = AGRODATAFINE
        Dim veg_cod As Integer = 0
        Dim cul_cod As Integer = 0
        Dim tipo As String = ""
        Dim gru_cod As Integer = 0
        Dim lav_cod As Integer = 0

        Dim flag_TerrenoNudo As Boolean = False

        Dim dtRval As DataTable = MenuBS_Lavorazioni.Carica_Lavorazioni(
            Piva:=Piva,
            Sa_Cod:=sa_cod,
            DataDa:=AGRODATAINIZIO,
            DataA:=AGRODATAFINE,
            Veg_Cod:=veg_cod,
            Cul_Cod:=cul_cod,
            Tipo:=tipo,
            Gru_Cod:=gru_cod,
            Lav_Cod:=lav_cod,
            Flag_TerrenoNudo:=flag_TerrenoNudo,
            xFiltroAggiuntivo_colturali:="",
            xFiltroAggiuntivo_postRaccolta:="",
            xFiltroAggiuntivo_contabili:="",
            xFiltroAggiuntivo_contabili_Macchine:="",
            xFiltroAggiuntivo_contabili_Audit:="",
            xOrderBy:="",
            objparametri_Server:=objParametri_Server,
            objparametri_Utenti:=objParametri_Utenti,
            FF_TrackedData_Cod:=FF_TrackedData_Cod, FromOutToIn:=FromOutToIn, cCertificazione:=cCertificazione, righeAggiunte:=TipoRigheAggiunte
        )

        Return dtRval

    End Function

#End Region

End Class