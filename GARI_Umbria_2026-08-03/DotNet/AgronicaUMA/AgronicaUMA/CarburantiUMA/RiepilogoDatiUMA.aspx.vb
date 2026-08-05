Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports System.Web.Services
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports System.Web

Public Class RiepilogoDatiUMA
    Inherits System.Web.UI.Page

    Public QS_Piva As String = ""
    Public QS_Anno As String = ""
    Public QS_Type As Integer = 0
    Public QS_PagArrivo As String = ""

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiAnniRichiesteAzienda(ByVal piva As String,
                                                     ByVal tipo_richiesta As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objTestataR As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
            Dim dtAnno = objTestataR.Leggi_Elenco2(piva, "t.Tipo_Richiesta = " & tipo_richiesta, "Anno", objParametri_Server).DefaultView.ToTable(True, {"Anno"})

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaStringa = JsonConvert.SerializeObject(dtAnno, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiRiepilogoDatiAzienda(ByVal piva As String) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Super_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim Dt As New DataTable

            Dt.Columns.Add(New DataColumn("azienda", GetType(String)))
            Dt.Columns.Add(New DataColumn("cuaa", GetType(String)))
            Dt.Columns.Add(New DataColumn("address", GetType(String)))
            Dt.Columns.Add(New DataColumn("citta", GetType(String)))
            Dt.Columns.Add(New DataColumn("cap", GetType(String)))
            Dt.Columns.Add(New DataColumn("prov", GetType(String)))
            Dt.Columns.Add(New DataColumn("telefono", GetType(String)))
            Dt.Columns.Add(New DataColumn("email", GetType(String)))

            Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim strErrMsg = ""
            Dim dtImprese = objImpreseR.RecuperaDatiImpresa_From_Piva(piva, "", strErrMsg, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            Dim objImprese2R As New AgronicaCoreAnagrafeBIZ.Impresa_R


            If dtImprese.Rows.Count > 0 Then
                Dim rowImpresa = dtImprese.Rows(0)

                Dim d0 As DataRow
                d0 = Dt.NewRow

                d0("azienda") = rowImpresa("rag_soc")
                d0("cuaa") = rowImpresa("CUAA")
                d0("address") = rowImpresa("ind_des")
                d0("citta") = rowImpresa("LOCALITA")
                d0("cap") = rowImpresa("CAP")
                d0("prov") = rowImpresa("COMUNI_PROV")
                d0("telefono") = rowImpresa("Telefono")
                d0("email") = rowImpresa("EMail")

                Dt.Rows.Add(d0)

                Dim serializerSettings As New JsonSerializerSettings()
                serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

                r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
                r.RispostaOK = True

            End If

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function LeggiRiepilogoDatiCarburanti(ByVal piva As String,
                                              ByVal anno As String,
                                              ByVal tipo_richiesta As Integer) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Utenti) Then
                r.Sessione = False
                Return r
            End If

            Dim objParametri_Super_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Super_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Super_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim Dt As New DataTable

            Dt.Columns.Add(New DataColumn("litriAssRichiestaAnnoPrec_Gasolio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAssRichiestaAnnoPrec_Benzina", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAssRichiestaAnnoPrec_GasolioSerra", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAssRichiestaAnnoPrec_Richiedente", GetType(String)))
            Dt.Columns.Add(New DataColumn("litriAssRichiestaAnnoPrec_Approvatore", GetType(String)))

            Dt.Columns.Add(New DataColumn("litriAssRendicontazioneAnnoPrec_Gasolio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAssRendicontazioneAnnoPrec_Benzina", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAssRendicontazioneAnnoPrec_GasolioSerra", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAssRendicontazioneAnnoPrec_Richiedente", GetType(String)))
            Dt.Columns.Add(New DataColumn("litriAssRendicontazioneAnnoPrec_Approvatore", GetType(String)))

            Dt.Columns.Add(New DataColumn("litriRimAnnoPrec_Gasolio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriRimAnnoPrec_Benzina", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriRimAnnoPrec_GasolioSerra", GetType(Decimal)))

            Dt.Columns.Add(New DataColumn("litriRecuperoAccDichiarati_Gasolio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriRecuperoAccDichiarati_Benzina", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriRecuperoAccDichiarati_GasolioSerra", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriRecuperoAccConfermati_Gasolio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriRecuperoAccConfermati_Benzina", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriRecuperoAccConfermati_GasolioSerra", GetType(Decimal)))

            Dt.Columns.Add(New DataColumn("litriAnticipoAnnoCorr_Gasolio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAnticipoAnnoCorr_Benzina", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAnticipoAnnoCorr_GasolioSerra", GetType(Decimal)))

            Dt.Columns.Add(New DataColumn("litriAssPrimaRichiesta_Gasolio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAssPrimaRichiesta_Benzina", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAssPrimaRichiesta_GasolioSerra", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAssPrimaRichiesta_Richiedente", GetType(String)))
            Dt.Columns.Add(New DataColumn("litriAssPrimaRichiesta_Approvatore", GetType(String)))

            Dt.Columns.Add(New DataColumn("Numero_Integrative", GetType(Integer)))

            Dim litriAssRichiestaAnnoPrec_Gasolio As Decimal = 0
            Dim litriAssRichiestaAnnoPrec_Benzina As Decimal = 0
            Dim litriAssRichiestaAnnoPrec_GasolioSerra As Decimal = 0
            Dim ls_litriAssRichiestaAnnoPrec_Richiedenti As New List(Of String)
            Dim ls_litriAssRichiestaAnnoPrec_Approvatori As New List(Of String)

            Dim litriAssRendicontazioneAnnoPrec_Gasolio As Decimal = 0
            Dim litriAssRendicontazioneAnnoPrec_Benzina As Decimal = 0
            Dim litriAssRendicontazioneAnnoPrec_GasolioSerra As Decimal = 0
            Dim litriAssRendicontazioneAnnoPrec_Richiedente As String = ""
            Dim litriAssRendicontazioneAnnoPrec_Approvatore As String = ""

            Dim litriRimAnnoPrec_Gasolio As Decimal = 0
            Dim litriRimAnnoPrec_Benzina As Decimal = 0
            Dim litriRimAnnoPrec_GasolioSerra As Decimal = 0

            Dim litriRecuperoAccDichiarati_Gasolio As Decimal = 0
            Dim litriRecuperoAccDichiarati_Benzina As Decimal = 0
            Dim litriRecuperoAccDichiarati_GasolioSerra As Decimal = 0
            Dim litriRecuperoAccConfermati_Gasolio As Decimal = 0
            Dim litriRecuperoAccConfermati_Benzina As Decimal = 0
            Dim litriRecuperoAccConfermati_GasolioSerra As Decimal = 0

            Dim litriAnticipoAnnoCorr_Gasolio As Decimal = 0
            Dim litriAnticipoAnnoCorr_Benzina As Decimal = 0
            Dim litriAnticipoAnnoCorr_GasolioSerra As Decimal = 0

            Dim litriAssPrimaRichiesta_Gasolio As Decimal = 0
            Dim litriAssPrimaRichiesta_Benzina As Decimal = 0
            Dim litriAssPrimaRichiesta_GasolioSerra As Decimal = 0
            Dim litriAssPrimaRichiesta_Richiedente As String = ""
            Dim litriAssPrimaRichiesta_Approvatore As String = ""

            Dim ls_litriAssIntegrazione_Gasolio As New List(Of Decimal)
            Dim ls_litriAssIntegrazione_Benzina As New List(Of Decimal)
            Dim ls_litriAssIntegrazione_GasolioSerra As New List(Of Decimal)
            Dim ls_litriAssIntegrazione_Richiedente As New List(Of String)
            Dim ls_litriAssIntegrazione_Approvatore As New List(Of String)

            Dim litriAcquistatiAllaData_Gasolio As Decimal = 0
            Dim litriAcquistatiAllaData_Benzina As Decimal = 0
            Dim litriAcquistatiAllaData_GasolioSerra As Decimal = 0
            Dim litriAcquistabiliAllaData_Gasolio As Decimal = 0
            Dim litriAcquistabiliAllaData_Benzina As Decimal = 0
            Dim litriAcquistabiliAllaData_GasolioSerra As Decimal = 0

            Dim litriAssTotAnnoCorr_Gasolio = 0
            Dim litriAssTotAnnoCorr_Benzina = 0
            Dim litriAssTotAnnoCorr_GasolioSerra = 0

            Dim annoInt As Integer = Integer.Parse(anno)

            Dim litriAssGasolio As Decimal
            Dim litriAssBenzina As Decimal
            Dim litriAssGasolioSerra As Decimal
            Dim litriAssRichiedente As String
            Dim litriAssApprovatore As String

            Dim Avanzamento_Richiesta As Integer

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R

            Dim strRichiesteFiltro = "Stato_Cod = " & enum_WAnagraficaStati.Verifica_Intermedia_Completata_Con_Successo
            Dim AvanzamentoRichiesta = AgronicaCoreUmaDal.UMA_Richieste_Testata_R.AVANZAMENTO_RICHIESTA_FITTIZIO

            Dim objTestataR As New AgronicaCoreUmaDal.UMA_Richieste_Testata_R
            Dim dtRichiesteAnnoPrec = objTestataR.Leggi_ConSommaCarburanti(piva, 0, tipo_richiesta, annoInt - 1, "", "Richiesta_Cod", objParametri_Server, objParametri_Utenti)

            For Each row In dtRichiesteAnnoPrec.Rows
                Avanzamento_Richiesta = row("Avanzamento_Richiesta")

                litriAssGasolio = row("assegnato_gasolio")
                litriAssBenzina = row("assegnato_benzina")
                litriAssGasolioSerra = row("assegnato_gasolio_serra")
                litriAssRichiedente = If(row("Nome_Richiedente") = "" And row("Cognome_Richiedente") = "", row("Rag_Soc_Richiedente"), row("Nome_Richiedente") & " " & row("Cognome_Richiedente"))
                litriAssApprovatore = If(row("Nome_Approvatore") = "" And row("Cognome_Approvatore") = "", row("Rag_Soc_Approvatore"), row("Nome_Approvatore") & " " & row("Cognome_Approvatore"))

                If Avanzamento_Richiesta = 0 Then

                    litriAssRichiestaAnnoPrec_Gasolio += litriAssGasolio
                    litriAssRichiestaAnnoPrec_Benzina += litriAssBenzina
                    litriAssRichiestaAnnoPrec_GasolioSerra += litriAssGasolioSerra

                    If Not ls_litriAssRichiestaAnnoPrec_Richiedenti.Contains(litriAssRichiedente) Then
                        ls_litriAssRichiestaAnnoPrec_Richiedenti.Add(litriAssRichiedente)
                    End If
                    If Not ls_litriAssRichiestaAnnoPrec_Approvatori.Contains(litriAssApprovatore) Then
                        ls_litriAssRichiestaAnnoPrec_Approvatori.Add(litriAssApprovatore)
                    End If

                ElseIf Avanzamento_Richiesta = 1 Then

                    litriAssRendicontazioneAnnoPrec_Gasolio = litriAssGasolio
                    litriAssRendicontazioneAnnoPrec_Benzina = litriAssBenzina
                    litriAssRendicontazioneAnnoPrec_GasolioSerra = litriAssGasolioSerra
                    litriAssRendicontazioneAnnoPrec_Richiedente = litriAssRichiedente
                    litriAssRendicontazioneAnnoPrec_Approvatore = litriAssApprovatore

                    litriRimAnnoPrec_Gasolio = If(IsDBNull(row("Rimanenza_Gasolio")), 0, row("Rimanenza_Gasolio"))
                    litriRimAnnoPrec_Benzina = If(IsDBNull(row("Rimanenza_Benzina")), 0, row("Rimanenza_Benzina"))
                    litriRimAnnoPrec_GasolioSerra = If(IsDBNull(row("Rimanenza_Gasolio_Serra")), 0, row("Rimanenza_Gasolio_Serra"))

                End If
            Next

            Dim Numero_Integrative As Integer = 0

            Dim dtRichiesteAnnoCorr = objTestataR.Leggi_ConSommaCarburanti(piva, 0, tipo_richiesta, annoInt, "", "Richiesta_Cod", objParametri_Server, objParametri_Utenti)

            For Each row In dtRichiesteAnnoCorr.Rows
                Avanzamento_Richiesta = row("Avanzamento_Richiesta")

                litriAssGasolio = row("assegnato_gasolio")
                litriAssBenzina = row("assegnato_benzina")
                litriAssGasolioSerra = row("assegnato_gasolio_serra")
                litriAssRichiedente = If(row("Nome_Richiedente") = "" And row("Cognome_Richiedente") = "", row("Rag_Soc_Richiedente"), row("Nome_Richiedente") & " " & row("Cognome_Richiedente"))
                litriAssApprovatore = If(row("Nome_Approvatore") = "" And row("Cognome_Approvatore") = "", row("Rag_Soc_Approvatore"), row("Nome_Approvatore") & " " & row("Cognome_Approvatore"))

                If Avanzamento_Richiesta = -1 Then

                    litriAnticipoAnnoCorr_Gasolio = litriAssGasolio
                    litriAnticipoAnnoCorr_Benzina = litriAssBenzina
                    litriAnticipoAnnoCorr_GasolioSerra = litriAssGasolioSerra

                ElseIf Avanzamento_Richiesta = 0 Then

                    If row("Richiesta_Integrativa") Then

                        Numero_Integrative += 1

                        Dt.Columns.Add(New DataColumn("litriAssIntegrazione" & Numero_Integrative & "_Gasolio", GetType(Decimal)))
                        Dt.Columns.Add(New DataColumn("litriAssIntegrazione" & Numero_Integrative & "_Benzina", GetType(Decimal)))
                        Dt.Columns.Add(New DataColumn("litriAssIntegrazione" & Numero_Integrative & "_GasolioSerra", GetType(Decimal)))
                        Dt.Columns.Add(New DataColumn("litriAssIntegrazione" & Numero_Integrative & "_Richiedente", GetType(String)))
                        Dt.Columns.Add(New DataColumn("litriAssIntegrazione" & Numero_Integrative & "_Approvatore", GetType(String)))

                        ls_litriAssIntegrazione_Gasolio.Add(litriAssGasolio)
                        ls_litriAssIntegrazione_Benzina.Add(litriAssBenzina)
                        ls_litriAssIntegrazione_GasolioSerra.Add(litriAssGasolioSerra)
                        ls_litriAssIntegrazione_Richiedente.Add(litriAssRichiedente)
                        ls_litriAssIntegrazione_Approvatore.Add(litriAssApprovatore)

                    Else

                        litriAssPrimaRichiesta_Gasolio = litriAssGasolio
                        litriAssPrimaRichiesta_Benzina = litriAssBenzina
                        litriAssPrimaRichiesta_GasolioSerra = litriAssGasolioSerra
                        litriAssPrimaRichiesta_Richiedente = litriAssRichiedente
                        litriAssPrimaRichiesta_Approvatore = litriAssApprovatore

                    End If

                    litriAssTotAnnoCorr_Gasolio += litriAssGasolio
                    litriAssTotAnnoCorr_Benzina += litriAssBenzina
                    litriAssTotAnnoCorr_GasolioSerra += litriAssGasolioSerra

                    litriRecuperoAccDichiarati_Gasolio += If(IsDBNull(row("Rec_Acc_Dich_Gasolio")), 0, row("Rec_Acc_Dich_Gasolio"))
                    litriRecuperoAccDichiarati_Benzina += If(IsDBNull(row("Rec_Acc_Dich_Benzina")), 0, row("Rec_Acc_Dich_Benzina"))
                    litriRecuperoAccDichiarati_GasolioSerra += If(IsDBNull(row("Rec_Acc_Dich_Gasolio_Serra")), 0, row("Rec_Acc_Dich_Gasolio_Serra"))
                    litriRecuperoAccConfermati_Gasolio += If(IsDBNull(row("Rec_Acc_Conf_Gasolio")), 0, row("Rec_Acc_Conf_Gasolio"))
                    litriRecuperoAccConfermati_Benzina += If(IsDBNull(row("Rec_Acc_Conf_Benzina")), 0, row("Rec_Acc_Conf_Benzina"))
                    litriRecuperoAccConfermati_GasolioSerra += If(IsDBNull(row("Rec_Acc_Conf_Gasolio_Serra")), 0, row("Rec_Acc_Conf_Gasolio_Serra"))

                ElseIf Avanzamento_Richiesta = 1 Then

                    litriRecuperoAccDichiarati_Gasolio += If(IsDBNull(row("Rec_Acc_Dich_Gasolio")), 0, row("Rec_Acc_Dich_Gasolio"))
                    litriRecuperoAccDichiarati_Benzina += If(IsDBNull(row("Rec_Acc_Dich_Benzina")), 0, row("Rec_Acc_Dich_Benzina"))
                    litriRecuperoAccDichiarati_GasolioSerra += If(IsDBNull(row("Rec_Acc_Dich_Gasolio_Serra")), 0, row("Rec_Acc_Dich_Gasolio_Serra"))
                    litriRecuperoAccConfermati_Gasolio += If(IsDBNull(row("Rec_Acc_Conf_Gasolio")), 0, row("Rec_Acc_Conf_Gasolio"))
                    litriRecuperoAccConfermati_Benzina += If(IsDBNull(row("Rec_Acc_Conf_Benzina")), 0, row("Rec_Acc_Conf_Benzina"))
                    litriRecuperoAccConfermati_GasolioSerra += If(IsDBNull(row("Rec_Acc_Conf_Gasolio_Serra")), 0, row("Rec_Acc_Conf_Gasolio_Serra"))

                End If
            Next

            Dt.Columns.Add(New DataColumn("litriAcquistatiAllaData_Gasolio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAcquistatiAllaData_Benzina", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAcquistatiAllaData_GasolioSerra", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAcquistabiliAllaData_Gasolio", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAcquistabiliAllaData_Benzina", GetType(Decimal)))
            Dt.Columns.Add(New DataColumn("litriAcquistabiliAllaData_GasolioSerra", GetType(Decimal)))

            Dim objVendite As New AgronicaCoreUmaDal.UMA_Vendite_R
            Dim dtVendite = objVendite.LeggiCarburanteVenduto(piva, annoInt, tipo_richiesta, 0, "", objParametri_Server)

            Dim Tipo_Carburante As Integer

            For Each row In dtVendite.Rows
                Tipo_Carburante = row("Tipo_Carburante")

                Select Case Tipo_Carburante
                    Case enum_TipoCarburante_UMA.Gasolio
                        litriAcquistatiAllaData_Gasolio += row("Totale_Carb")

                    Case enum_TipoCarburante_UMA.Benzina
                        litriAcquistatiAllaData_Benzina += row("Totale_Carb")

                    Case enum_TipoCarburante_UMA.Gasolio_Serra
                        litriAcquistatiAllaData_GasolioSerra += row("Totale_Carb")

                End Select
            Next

            litriAcquistabiliAllaData_Gasolio = If(litriAssTotAnnoCorr_Gasolio > 0, litriAssTotAnnoCorr_Gasolio, litriAnticipoAnnoCorr_Gasolio) - litriAcquistatiAllaData_Gasolio - litriRimAnnoPrec_Gasolio
            litriAcquistabiliAllaData_Benzina = If(litriAssTotAnnoCorr_Benzina > 0, litriAssTotAnnoCorr_Benzina, litriAnticipoAnnoCorr_Benzina) - litriAcquistatiAllaData_Benzina - litriRimAnnoPrec_Benzina
            litriAcquistabiliAllaData_GasolioSerra = If(litriAssTotAnnoCorr_GasolioSerra > 0, litriAssTotAnnoCorr_GasolioSerra, litriAnticipoAnnoCorr_GasolioSerra) - litriAcquistatiAllaData_GasolioSerra - litriRimAnnoPrec_GasolioSerra

            Dim d0 As DataRow
            d0 = Dt.NewRow

            d0("litriAssRichiestaAnnoPrec_Gasolio") = litriAssRichiestaAnnoPrec_Gasolio
            d0("litriAssRichiestaAnnoPrec_Benzina") = litriAssRichiestaAnnoPrec_Benzina
            d0("litriAssRichiestaAnnoPrec_GasolioSerra") = litriAssRichiestaAnnoPrec_GasolioSerra
            d0("litriAssRichiestaAnnoPrec_Richiedente") = String.Join(", ", ls_litriAssRichiestaAnnoPrec_Richiedenti)
            d0("litriAssRichiestaAnnoPrec_Approvatore") = String.Join(", ", ls_litriAssRichiestaAnnoPrec_Approvatori)

            d0("litriAssRendicontazioneAnnoPrec_Gasolio") = litriAssRendicontazioneAnnoPrec_Gasolio
            d0("litriAssRendicontazioneAnnoPrec_Benzina") = litriAssRendicontazioneAnnoPrec_Benzina
            d0("litriAssRendicontazioneAnnoPrec_GasolioSerra") = litriAssRendicontazioneAnnoPrec_GasolioSerra
            d0("litriAssRendicontazioneAnnoPrec_Richiedente") = litriAssRendicontazioneAnnoPrec_Richiedente
            d0("litriAssRendicontazioneAnnoPrec_Approvatore") = litriAssRendicontazioneAnnoPrec_Approvatore

            d0("litriRimAnnoPrec_Gasolio") = litriRimAnnoPrec_Gasolio
            d0("litriRimAnnoPrec_Benzina") = litriRimAnnoPrec_Benzina
            d0("litriRimAnnoPrec_GasolioSerra") = litriRimAnnoPrec_GasolioSerra

            d0("litriRecuperoAccDichiarati_Gasolio") = litriRecuperoAccDichiarati_Gasolio
            d0("litriRecuperoAccDichiarati_Benzina") = litriRecuperoAccDichiarati_Benzina
            d0("litriRecuperoAccDichiarati_GasolioSerra") = litriRecuperoAccDichiarati_GasolioSerra
            d0("litriRecuperoAccConfermati_Gasolio") = litriRecuperoAccConfermati_Gasolio
            d0("litriRecuperoAccConfermati_Benzina") = litriRecuperoAccConfermati_Benzina
            d0("litriRecuperoAccConfermati_GasolioSerra") = litriRecuperoAccConfermati_GasolioSerra

            d0("litriAnticipoAnnoCorr_Gasolio") = litriAnticipoAnnoCorr_Gasolio
            d0("litriAnticipoAnnoCorr_Benzina") = litriAnticipoAnnoCorr_Benzina
            d0("litriAnticipoAnnoCorr_GasolioSerra") = litriAnticipoAnnoCorr_GasolioSerra

            d0("litriAssPrimaRichiesta_Gasolio") = litriAssPrimaRichiesta_Gasolio
            d0("litriAssPrimaRichiesta_Benzina") = litriAssPrimaRichiesta_Benzina
            d0("litriAssPrimaRichiesta_GasolioSerra") = litriAssPrimaRichiesta_GasolioSerra
            d0("litriAssPrimaRichiesta_Richiedente") = litriAssPrimaRichiesta_Richiedente
            d0("litriAssPrimaRichiesta_Approvatore") = litriAssPrimaRichiesta_Approvatore

            d0("Numero_Integrative") = Numero_Integrative

            For i As Integer = 1 To Numero_Integrative
                d0("litriAssIntegrazione" & i & "_Gasolio") = ls_litriAssIntegrazione_Gasolio(i - 1)
                d0("litriAssIntegrazione" & i & "_Benzina") = ls_litriAssIntegrazione_Benzina(i - 1)
                d0("litriAssIntegrazione" & i & "_GasolioSerra") = ls_litriAssIntegrazione_GasolioSerra(i - 1)
                d0("litriAssIntegrazione" & i & "_Richiedente") = ls_litriAssIntegrazione_Richiedente(i - 1)
                d0("litriAssIntegrazione" & i & "_Approvatore") = ls_litriAssIntegrazione_Approvatore(i - 1)
            Next

            d0("litriAcquistatiAllaData_Gasolio") = litriAcquistatiAllaData_Gasolio
            d0("litriAcquistatiAllaData_Benzina") = litriAcquistatiAllaData_Benzina
            d0("litriAcquistatiAllaData_GasolioSerra") = litriAcquistatiAllaData_GasolioSerra
            d0("litriAcquistabiliAllaData_Gasolio") = litriAcquistabiliAllaData_Gasolio
            d0("litriAcquistabiliAllaData_Benzina") = litriAcquistabiliAllaData_Benzina
            d0("litriAcquistabiliAllaData_GasolioSerra") = litriAcquistabiliAllaData_GasolioSerra

            Dt.Rows.Add(d0)

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            r.RispostaStringa = JsonConvert.SerializeObject(Dt, Formatting.None, serializerSettings)
            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r
    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function CercaPivaReale(ByVal piva As String) As RispostaStandard


        Dim r As New RispostaStandard
        Dim PartitaIvaReale As String = ""
        Dim leggiPivaReale As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Try

            Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Lingua.Gias_InizializzaCultura_DaSession()

            If (Not piva.Equals(String.Empty)) Then
                PartitaIvaReale = leggiPivaReale.Leggi_PivaReale(piva, objParametri_Server)
            End If

            Dim serializerSettings As New JsonSerializerSettings()
            serializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            r.RispostaStringa = JsonConvert.SerializeObject(PartitaIvaReale, Formatting.None, serializerSettings)

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False

            'uso questa funzione per ottenere il Messaggio..:
            r.Errore = "ErroreDuranteOperazione_" & vbCrLf &
        AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function


    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()
    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri
    Public objparametri_server_string, objparametri_utenti_string As String
    Dim objParametriAgenda As ParametriAgenda

    Private Sub inizializzoObjParametri()
        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objparametri_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        '---
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Master.flag_pag_GestioneCosti = True

        inizializzoObjParametri()
        inizializzoParametriPagina()

        Dim Entrata_Diretta As Integer = 0
        Dim Split As Integer = 0
        Dim PaginaRedirect As String

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Riepilogo_UMA,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        If Not UtenteAbilitatoLettura Then
            UtenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Riepilogo_UMA,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Riepilogo_UMA,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        If Not UtenteAbilitatoScrittura Then
            UtenteAbilitatoScrittura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.Riepilogo_UMA,
                                            enum_Security_Operazione.Modifica,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)
        End If

        If Not IsNothing(Request.QueryString("p")) Then
            QS_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, AgroKey_EncoderDecoder, Server)
            QS_Anno = Request.QueryString("anno").ToString
        End If

        QS_Type = 0
        If Not IsNothing(Request.QueryString("type")) Then
            QS_Type = CInt(Request.QueryString("type"))
        End If

        If Not IsNothing(Request.QueryString("pa")) Then
            Dim s_Arrivo = Request.QueryString("pa").ToString
            If (s_Arrivo = "1") Then
                QS_PagArrivo = "1"
                Master.flag_MostraHeader = False
                Master.flag_MostraFooter = False
            Else
                Master.flag_MostraHeader = True
                Master.flag_MostraFooter = True
            End If
        End If

        'Pagina di origine
        hdPaginaRedirect.Value = ""
        hdPaginaRedirect_Codificata.Value = ""

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoLettura.Value = UtenteAbilitatoLettura
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

        If Not UtenteAbilitatoLettura Then
            If String.IsNullOrWhiteSpace(CStr(hdPaginaRedirect_Codificata.Value)) Then
                Response.Redirect("~/Menu/MenuBS_2017.aspx")
            Else
                Response.Redirect(CStr(hdPaginaRedirect.Value) & "?p=" & Request.QueryString("p"))
            End If
        End If

        hdId_Agenda.Value = 0
        hdId_Mov.Value = 0
        hdId_Mov_Det.Value = 0
        hdId_Agenda_CDG.Value = 0
        hdModalita.Value = Entrata_Diretta
        hdId_CDG.Value = 0
        hdLav_Cod.Value = 0
        hdVeg_Cod.Value = 0
        hdDes_Lib.Value = ""
        hdMov_Desc.Value = ""
        hdSplit.Value = Split
        hdData.Value = JToken.Parse(JsonConvert.SerializeObject(Now))

        'Automatico (Impostazione da QDC)
        hdAutomatico.Value = 1 'Default

        If Not Page.IsPostBack Then


            If Not IsNothing(Session("ParametriAgenda_2010")) Then

                Dim objParametriAgenda_2010 As New ParametriAgenda_2010
                objParametriAgenda_2010.Leggi()
                hdPiva.Value = objParametriAgenda_2010.Piva
                hdId_Agenda.Value = Val(objParametriAgenda_2010.Id_Agenda)

            End If

            'If Entrata_Diretta = 0 Then

            ''Introdotta per non rieseguirlo se appena creata con provenienza da APP
            'Dim bombardinoDaEseguire = True

            'If Val(hdId_Agenda.Value) = 0 Then

            'End If

        End If

    End Sub

    Private Sub inizializzoParametriPagina()


    End Sub

    Public Shadows ReadOnly Property Master() As AgronicaUMA.UmaBootstrap
        Get
            Return CType(MyBase.Master, AgronicaUMA.UmaBootstrap)
        End Get
    End Property

End Class