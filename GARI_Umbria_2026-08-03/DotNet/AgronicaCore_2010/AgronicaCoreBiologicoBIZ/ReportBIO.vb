Imports AgronicaCoreBiologicoDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.My.Resources
Imports Newtonsoft.Json.Linq
Imports System.Web
Imports System.Globalization
Imports System.Web.UI

Public Class ReportBIO

    Public Function ReportBio(ByVal filtro As FiltroReportBIO,
                              ByVal userName As String,
                              ByVal prgGias As String,
                              ByRef objParametriServer As AgronicaCoreParametri,
                              ByRef objParametriUtenti As AgronicaCoreParametri) As rispostaStandard(Of RispostaReportBio)

        Dim r As New rispostaStandard(Of RispostaReportBio)
        r.RispostaOK = False

        Try
            Dim dal As New BIO_ReportBio
            Dim xOrderBy As String = String.Empty
            Dim dati As DataTable
            Dim serializerSettings As New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore}

            If filtro.dpDataInizio Is Nothing OrElse Not filtro.dpDataInizio.HasValue Then
                filtro.dpDataInizio = AGRODATAINIZIO
            End If
            If filtro.dpDataFine Is Nothing OrElse Not filtro.dpDataFine.HasValue Then
                filtro.dpDataFine = AGRODATAFINE
            End If

            'tipo output, tabellare o report è in questo momento gestito in maniera implicita, deducibile da come assegnato il flag "IsLink"
            'rispettivamente a false e true
            Select Case filtro.ddlEstrazioni
                Case enum_Tipo_Report_BIO.Carichi_Scarichi
                    dati = dal.Report_Carichi_Scarichi(
                        filtro.hdPiva,
                        filtro.ddlEstrazioni,
                        filtro.dpDataInizio,
                        filtro.dpDataFine,
                        filtro.msCategorieMagazzino.ToList(),
                        filtro.ddlTipiMovimenti.ToList(),
                        filtro.msTipoAppezzamento.ToList(),
                        filtro.ddlOrigineDatiCauScarichi,
                        filtro.ddlCentriAziendali,
                        xOrderBy, objParametriServer)
                    Dim carichiScarichi = AggiustaDatiPrincipiAttiviEGiacenze(filtro, dati, objParametriServer, objParametriUtenti)
                    AggiustaQtaInBaseAStroni(carichiScarichi)

                    Dim carichiScarichiFinale = carichiScarichi.Where(Function(f) Not f.StornatoTotalmente).ToList()

                    If dati IsNot Nothing Then
                        r.RispostaStringa = New RispostaReportBio With
                            {
                                .IsLink = False,
                                .Risposta = JsonConvert.SerializeObject(carichiScarichiFinale, Formatting.None, serializerSettings),
                                .TipoReport = filtro.ddlEstrazioni
                            }
                        r.RispostaOK = True
                    Else
                        r.Errore = Gias.ErroreOttenimentoReport
                    End If

                Case enum_Tipo_Report_BIO.Scheda_Materie_Prime,
                     enum_Tipo_Report_BIO.Scheda_Vendite,
                     enum_Tipo_Report_BIO.Registro_Preparazioni,
                     enum_Tipo_Report_BIO.Report_Terzisti
                    Dim targetUrl As String = String.Empty
                    Dim errore As String = String.Empty
                    Dim flagErrore As Boolean = ComponiUrlPerStampa(filtro, targetUrl, errore)
                    If Not flagErrore Then
                        r.RispostaStringa = New RispostaReportBio With
                           {
                               .IsLink = True,
                               .Risposta = targetUrl,
                               .TipoReport = filtro.ddlEstrazioni
                           }
                        r.RispostaOK = True
                    Else
                        r.RispostaStringa = New RispostaReportBio With
                           {
                               .IsLink = True,
                               .Risposta = "",
                               .TipoReport = filtro.ddlEstrazioni
                           }
                        r.Errore = errore
                    End If

                Case Else
                    r.Errore = Gias.ReportNonRiconosciuto
            End Select

        Catch ex As Exception
            r.Errore = Gias.ErroreDuranteOperazione_ & vbCrLf & AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    Private Function AggiustaDatiPrincipiAttiviEGiacenze(ByVal filtro As FiltroReportBIO,
                                                         ByVal data As DataTable,
                                                         ByRef objParametriServer As AgronicaCoreParametri,
                                                         ByRef objParametriUtenti As AgronicaCoreParametri) As IEnumerable(Of CaricoScarico)

        If data Is Nothing OrElse data.Rows.Count = 0 Then
            Return New List(Of CaricoScarico)
        End If

        Dim carichiScarichi = New List(Of CaricoScarico)
        For Each r As DataRow In data.AsEnumerable
            Dim objCs = New CaricoScarico With
                {
                    .ChiaveProdotto = "",
                    .Id_Agenda = CInt(r.Item("id_agenda")),
                    .Id_Mov_Det = CInt(r.Item("id_mov_det")),
                    .Sa_Cod = CInt(r.Item("Sa_Cod")),
                    .Elem_Cod = CInt(r.Item("Elem_Cod")),
                    .Pro_Cod = CInt(r.Item("pro_cod")),
                    .Mat_Cod = CInt(r.Item("mat_cod")),
                    .Piva = r.Item("Piva").ToString,
                    .Cau_Mov = r.Item("Cau_Mov").ToString,
                    .Cod_Risum = CInt(r.Item("cod_risum")),
                    .Tipo_Movimento = r.Item("Tipo_Movimento").ToString(),
                    .Tipo_Prodotto = r.Item("Tipo_Prodotto").ToString(),
                    .Articolo = r.Item("Articolo").ToString(),
                    .Descrizione = r.Item("Descrizione").ToString(),
                    .Classificazione = r.Item("Classificazione").ToString(),
                    .OP = r.Item("OP").ToString(),
                    .Fornitore = r.Item("Fornitore").ToString(),
                    .Fornitore_Ragione_Sociale = r.Item("Fornitore_Ragione_Sociale").ToString(),
                    .Fabbricato_Des = r.Item("Fabbricato_Des").ToString(),
                    .Id_Destinazione = CInt(r.Item("Id_Destinazione")),
                    .Bio_Convers = r.Item("Bio_Convers").ToString(),
                    .App_BIO = r.Item("App_BIO").ToString(),
                    .Lotto = r.Item("Lotto").ToString(),
                    .Data = Convert.ToDateTime(r.Item("Data_Movimento")),
                    .Ora = Convert.ToDateTime(r.Item("Ora")),
                    .Qta = CDec(r.Item("Qta")),
                    .Qta_Originale = CDec(r.Item("Qta_Originale")),
                    .Progetto = r.Item("Progetto").ToString(),
                    .Campo = r.Item("Campo").ToString(),
                    .Campo_Descrizione = r.Item("Campo_Descrizione").ToString(),
                    .Sup = CDec(r.Item("Sup")),
                    .Tratta_Principi_attivi = r.Item("tratta_Principi_attivi").ToString(),
                    .Tratta_CU = CDec(r.Item("Tratta_CU")),
                    .Tratta_Azoto = CDec(r.Item("Tratta_Azoto")),
                    .Tratta_Fosforo = CDec(r.Item("Tratta_Fosforo")),
                    .Tratta_Efficienza = CDec(r.Item("Tratta_Efficienza")),
                    .Giacenza = 0,
                    .DDTNumero = r.Field(Of String)("ddt_numero"),
                    .DDTData = r.Field(Of DateTime?)("ddt_data"),
                    .DDTOra = r.Field(Of DateTime?)("ddt_ora"),
                    .Matricola = r.Field(Of String)("Matricola"),
                    .Gen_Cod = r.Field(Of Integer)("Gen_Cod"),
                    .Gen_Des = r.Field(Of String)("Gen_Des"),
                    .Spe_Cod = r.Field(Of Integer)("Spe_Cod"),
                    .Spe_Des = r.Field(Of String)("Spe_Des"),
                    .Raz_Cod = r.Field(Of Integer)("Raz_Cod"),
                    .Raz_Des = r.Field(Of String)("Raz_Des"),
                    .Nome_Animale = r.Field(Of String)("Nome_Animale"),
                    .Rif_Esterno = r.Field(Of String)("Rif_Esterno"),
                    .Rif_Esterno_2 = r.Field(Of String)("Rif_Esterno_2")
                }

            carichiScarichi.Add(objCs)

        Next

        Threading.Tasks.Parallel.ForEach(carichiScarichi,
                                    Sub(cs)

                                        Dim titoloAzoto = cs.Tratta_Azoto
                                        Dim titoloRame = 0
                                        Dim titoloFosforo = cs.Tratta_Fosforo

                                        cs.Rame = 0
                                        cs.Azoto = 0
                                        cs.Fosforo = 0

                                        cs.ChiaveProdotto = String.Format("{0}|{1}|{2}|{3}", cs.Elem_Cod, cs.Pro_Cod, cs.Mat_Cod, cs.Id_Destinazione)
                                        Select Case cs.Elem_Cod
                                            Case FERTILIZZANTI
                                                titoloRame = cs.Tratta_CU
                                            Case FORMULATI
                                                If Not String.IsNullOrEmpty(cs.Tratta_Principi_attivi) Then
                                                    Dim vpc = cs.Tratta_Principi_attivi.Split("|")
                                                    Dim rameMetallico = vpc.FirstOrDefault(Function(v) v.StartsWith("883"))
                                                    If rameMetallico IsNot Nothing Then
                                                        Dim valoreRame As Decimal = 0
                                                        If Decimal.TryParse(rameMetallico.Split("§")(1), NumberStyles.Any, New CultureInfo("en-US"), valoreRame) Then
                                                            titoloRame = valoreRame
                                                        End If
                                                    End If
                                                End If
                                        End Select

                                        If cs.Tratta_Efficienza = 0 Then
                                            cs.Tratta_Efficienza = 1
                                        End If

                                        If titoloRame <> 0 Then
                                            cs.Rame = Math.Abs((cs.Qta * titoloRame) / 100)
                                        End If
                                        If titoloAzoto <> 0 Then
                                            cs.Azoto = Math.Abs((cs.Qta * titoloAzoto * cs.Tratta_Efficienza) / 100)
                                        End If
                                        If titoloFosforo <> 0 Then
                                            cs.Fosforo = Math.Abs((cs.Qta * titoloFosforo) / 100)
                                        End If

                                    End Sub)

        If filtro.kSwitchGiacenzeMagazzino Then

            Dim categorie = carichiScarichi.Select(Function(cs)
                                                       Return cs.Elem_Cod
                                                   End Function).Distinct().ToList()
            Dim objGiacenze As New AgronicaCoreContabDAL.Giacenze_R
            For Each cat As Integer In categorie

                Dim dtGiacenze As DataTable
                dtGiacenze = objGiacenze.SchedaGiacenzeMagazzino(DateTime.Now,
                                                                     filtro.hdPiva,
                                                                     0,
                                                                     0,
                                                                     cat, 0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                                     True,
                                                                     "", "", "", "", "", "", "", "", "",
                                                                     "", "",
                                                                     "",
                                                                     objParametriServer, objParametriUtenti,
                                                                     flagRecuperaCodArticolo:=True,
                                                                     codArticolo:="",
                                                                     cercaCodArticoloPerLike:=True)

                If dtGiacenze IsNot Nothing AndAlso dtGiacenze.Rows.Count > 0 Then

                    For Each g As DataRow In dtGiacenze.Rows
                        Dim Id_Destinazione As Integer = CInt(g.Item("Id_Destinazione"))
                        Dim Pro_Cod As Integer = CInt(g.Item("Pro_Cod"))
                        Dim Mat_Cod As Integer = CInt(g.Item("Mat_Cod"))
                        Dim Elem_Cod As Integer = cat
                        Dim Giacenza = Convert.ToDecimal(g.Item("Giacenza"))
                        Dim ChiaveRicerca = String.Format("{0}|{1}|{2}|{3}", Elem_Cod, Pro_Cod, Mat_Cod, Id_Destinazione)
                        Dim csFiltrati = carichiScarichi.Where(Function(c) c.ChiaveProdotto = ChiaveRicerca)
                        If csFiltrati IsNot Nothing AndAlso csFiltrati.Any Then
                            For Each cs In csFiltrati
                                cs.Giacenza = Giacenza
                            Next
                        End If

                    Next
                End If

            Next
        End If

        Return carichiScarichi

    End Function

    Private Sub AggiustaQtaInBaseAStroni(ByRef carichiScarichi As List(Of CaricoScarico))

        Dim rif_esterni As List(Of String) = ((From cs In carichiScarichi Select cs.Rif_Esterno).Distinct).ToList
        For Each rif In rif_esterni

            If Not String.IsNullOrEmpty(rif) Then

                Dim storni = carichiScarichi.Where(Function(cs) cs.Rif_Esterno_2.Equals(rif)).OrderBy(Function(o) o.Id_Mov_Det)
                If storni.Any Then
                    Dim sommaStorni = storni.Sum(Function(s) s.Qta_Originale)

                    ' cerco movimento originario
                    Dim movimentoPadre = carichiScarichi.FirstOrDefault(Function(p) p.Rif_Esterno.Equals(rif))
                    If Not IsNothing(movimentoPadre) Then
                        If movimentoPadre.Qta - sommaStorni <= 0 Then
                            'Stornato totalmente
                            movimentoPadre.StornatoTotalmente = True
                            For Each storno In storni
                                storno.StornatoTotalmente = True
                            Next
                        Else
                            ' Stornato parzialmente
                            For Each storno In storni
                                If (movimentoPadre.Qta - storno.Qta_Originale) >= 0 Then
                                    movimentoPadre.Qta -= storno.Qta_Originale
                                    storno.StornatoTotalmente = True
                                End If
                            Next
                        End If
                    End If
                End If

            End If

        Next

    End Sub



    Private Function ComponiUrlPerStampa(ByVal filtro As FiltroReportBIO,
                                         ByRef targetUrl As String,
                                         ByRef errore As String) As Boolean
        Dim flagErrore As Boolean = False
        Dim arrErrori As New JArray()

        If filtro.ddlImprese = "" Then
            arrErrori.Add(Gias.SelezionareUnaImpresa)
        End If

        If IsNothing(filtro.dpDataInizio) Then
            filtro.dpDataInizio = New Date
        End If
        If IsNothing(filtro.dpDataFine) Then
            filtro.dpDataFine = New Date
        End If
        If filtro.dpDataInizio > filtro.dpDataFine Then
            arrErrori.Add(Gias.DataFineDeveEssereMaggioreOUgualeDellaDataInizio)
        End If

        Select Case filtro.ddlEstrazioni
            Case enum_Tipo_Report_BIO.Scheda_Materie_Prime
                'Obbligatorietà su msCategorieMagazzino
                If filtro.msCategorieMagazzino.Length = 0 Then
                    arrErrori.Add(Gias.SelezionareAlmenoUnaCategoriaProdotto)
                End If

            Case enum_Tipo_Report_BIO.Scheda_Vendite
                'Obbligatorietà su msCategorieMagazzino
                If filtro.msCategorieMagazzino.Length = 0 Then
                    arrErrori.Add(Gias.SelezionareAlmenoUnaCategoriaProdotto)
                End If

            Case enum_Tipo_Report_BIO.Registro_Preparazioni
                ' Obbligatorietà su ddlSemilavoratiTrasformati e ddlPreparazioni
                If filtro.ddlSemilavoratiTrasformati = "" Then
                    arrErrori.Add(Gias.SelezionareUnProdottoSemilavoratoOTrasformato)
                End If
                If filtro.ddlSemilavoratiTrasformati = "" Then
                    arrErrori.Add(Gias.SelezionareLaLineaDiProduzionePreparazione)
                End If
        End Select

        If arrErrori.Count = 0 Then

            Dim utilityControl As New Control

            Dim querystring = "p=" & Stringa_Codifica(filtro.ddlImprese, AgroKey_EncoderDecoder, Nothing) &
                            "&s=" & Stringa_Codifica(filtro.ddlCentriAziendali, AgroKey_EncoderDecoder, Nothing) &
                            "&f=" & Stringa_Codifica(filtro.ddlMagazzini, AgroKey_EncoderDecoder, Nothing) &
                            "&di=" & Stringa_Codifica(filtro.dpDataInizio.Value.ToShortDateString(), AgroKey_EncoderDecoder, Nothing) &
                            "&df=" & Stringa_Codifica(filtro.dpDataFine.Value.ToShortDateString(), AgroKey_EncoderDecoder, Nothing) &
                            "&arr=" & Stringa_Codifica(filtro.ddlArrotondamenti, AgroKey_EncoderDecoder, Nothing)

            Dim Str_elem_cod As String = "(" & String.Join(", ", filtro.msCategorieMagazzino) & ")"

            Select Case filtro.ddlEstrazioni
                Case enum_Tipo_Report_BIO.Scheda_Materie_Prime
                    querystring &= "&fec=" & Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder, Nothing) &
                                    "&lcp=" & Stringa_Codifica(filtro.ddlClassiProdotto, AgroKey_EncoderDecoder, Nothing) &
                                    "&vas=" & Stringa_Codifica(filtro.kSwitchConsistenzaVasca, AgroKey_EncoderDecoder, Nothing) &
                                    "&chkca=" & Stringa_Codifica(filtro.kSwitchCodiceArticolo, AgroKey_EncoderDecoder, Nothing) &
                                    "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Nothing) &
                                    "&rc=" & Stringa_Codifica(filtro.ddlRegioni, AgroKey_EncoderDecoder, Nothing) &
                                    "&mstdata=" & Stringa_Codifica(filtro.kSwitchMostraDataStampa, AgroKey_EncoderDecoder, Nothing) &
                                    "&odc=" & Stringa_Codifica(filtro.kSwitchMostraFirmaODC, AgroKey_EncoderDecoder, Nothing)
                    targetUrl = utilityControl.ResolveUrl("~/GestioneStampe/Biologico/SchedaMateriePrime/SchedaMateriePrimeBiologico.aspx?" & querystring)

                Case enum_Tipo_Report_BIO.Scheda_Vendite
                    querystring &= "&fec=" & Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder, Nothing) &
                                            "&lcp=" & Stringa_Codifica(filtro.ddlClassiProdotto, AgroKey_EncoderDecoder, Nothing) &
                                            "&m=" & Stringa_Codifica(filtro.ddlProdotti, AgroKey_EncoderDecoder, Nothing) &
                                            "&ru=" & Stringa_Codifica(filtro.ddlContatti, AgroKey_EncoderDecoder, Nothing) &
                                            "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Nothing) &
                                            "&sl=" & Stringa_Codifica(filtro.ddlStampeLotto, AgroKey_EncoderDecoder, Nothing) &
                                            "&chkca=" & Stringa_Codifica(filtro.kSwitchCodiceArticolo, AgroKey_EncoderDecoder, Nothing) &
                                            "&rc=" & Stringa_Codifica(filtro.ddlRegioni, AgroKey_EncoderDecoder, Nothing) &
                                            "&mstdata=" & Stringa_Codifica(filtro.kSwitchMostraDataStampa, AgroKey_EncoderDecoder, Nothing) &
                                            "&odc=" & Stringa_Codifica(filtro.kSwitchMostraFirmaODC, AgroKey_EncoderDecoder, Nothing)

                    targetUrl = utilityControl.ResolveUrl("~/GestioneStampe/Biologico/SchedaVendite/SchedaVenditeBiologico.aspx?" & querystring)

                Case enum_Tipo_Report_BIO.Registro_Preparazioni
                    Dim Mat_Des As String = filtro.semilavTrasfDes
                    Dim Mat_Cod As Integer = CInt(filtro.ddlSemilavoratiTrasformati.Split("|")(0))
                    Dim Elem_Cod As Integer = CInt(filtro.ddlSemilavoratiTrasformati.Split("|")(1))

                    Dim Linea_Cod As Integer = CInt(filtro.ddlPreparazioni.Split("|")(0))
                    Dim Preparazione_Cod As Integer = CInt(filtro.ddlPreparazioni.Split("|")(1))

                    'Filtri non strettamente obbligatori in quanto sono usati per eventuale log di errore
                    'QS_SaveText()
                    Dim ragSocImpresa As String = ""
                    Dim nomeCentro As String = ""

                    Dim intLogoRegione = If(filtro.kSwitchLogoRegione, 1, 0)

                    querystring &= "&lc=" & Stringa_Codifica(Linea_Cod, AgroKey_EncoderDecoder, Nothing) &
                                   "&pc=" & Stringa_Codifica(Preparazione_Cod, AgroKey_EncoderDecoder, Nothing) &
                                   "&rc=" & Stringa_Codifica(filtro.ddlRegioni, AgroKey_EncoderDecoder, Nothing) &
                                   "&fr=" & Stringa_Codifica(intLogoRegione, AgroKey_EncoderDecoder, Nothing) &
                                   "&mdes=" & Stringa_Codifica(Mat_Des, AgroKey_EncoderDecoder, Nothing) &
                                   "&m=" & Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Nothing) &
                                   "&e=" & Stringa_Codifica(Elem_Cod, AgroKey_EncoderDecoder, Nothing) &
                                   "&rs=" & Stringa_Codifica(ragSocImpresa, AgroKey_EncoderDecoder, Nothing) &
                                   "&sn=" & Stringa_Codifica(nomeCentro, AgroKey_EncoderDecoder, Nothing) &
                                   "&random=" & Stringa_Codifica(AgronicaCoreDataProvider.UtilityProvider.StringaUnivoca(), AgroKey_EncoderDecoder, Nothing) &
                                   "&seza=" & Stringa_Codifica(filtro.kSwitchSezioneA, AgroKey_EncoderDecoder, Nothing) &
                                   "&sezb=" & Stringa_Codifica(filtro.kSwitchSezioneB, AgroKey_EncoderDecoder, Nothing)
                    targetUrl = utilityControl.ResolveUrl("~/GestioneStampe/Biologico/RegistroPreparazioniBio/RegistroPreparazioniBio.aspx?" & querystring)

                Case enum_Tipo_Report_BIO.Report_Terzisti
                    Dim Mat_Cod = filtro.msProdotti
                    Dim Lvl_Det = filtro.ddlLivelliDettaglio
                    Dim Fornitori = filtro.msFornitore '"33331|23943" 


                    querystring &= "&m=" & Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Nothing) &
                                   "&ld=" & Stringa_Codifica(Lvl_Det, AgroKey_EncoderDecoder, Nothing) &
                                   "&for=" & Stringa_Codifica(Fornitori, AgroKey_EncoderDecoder, Nothing)

                    targetUrl = utilityControl.ResolveUrl("~/GestioneStampe/Biologico/ModelloTerzistiBio/ModelloTerzistiBio.aspx?" & querystring)

            End Select

        End If

        If arrErrori.Count > 0 Then
            errore = arrErrori.ToString()
            flagErrore = True
        End If


        Return flagErrore

    End Function

    Private Class CaricoScarico
        Public ChiaveProdotto As String
        Public Id_Agenda As Integer
        Public Id_Mov_Det As Integer
        Public Sa_Cod As Integer
        Public Elem_Cod As Integer
        Public Pro_Cod As Integer
        Public Mat_Cod As Integer
        Public Piva As String
        Public Cau_Mov As String
        Public Cod_Risum As Integer
        Public Tipo_Movimento As String
        Public Tipo_Prodotto As String
        Public Articolo As String
        Public Descrizione As String
        Public Classificazione As String
        Public OP As String
        Public Fornitore As String
        Public Fornitore_Ragione_Sociale As String
        Public Fabbricato_Des As String
        Public Id_Destinazione As Integer
        Public Bio_Convers As String
        Public App_BIO As String
        Public Lotto As String
        Public Data As DateTime
        Public Ora As DateTime
        Public Qta As Decimal
        Public Qta_Originale As Decimal
        Public Progetto As String
        Public Campo As String
        Public Campo_Descrizione As String
        Public Sup As Decimal
        Public Tratta_Principi_attivi As String
        Public Tratta_CU As Decimal
        Public Tratta_Azoto As Decimal
        Public Tratta_Fosforo As Decimal
        Public Tratta_Efficienza As Decimal
        Public Giacenza As Decimal
        Public Azoto As Decimal
        Public Rame As Decimal
        Public Fosforo As Decimal
        Public DDTNumero As String
        Public DDTData As DateTime?
        Public DDTOra As DateTime?
        Public Matricola As String
        Public Gen_Cod As Integer
        Public Gen_Des As String
        Public Spe_Cod As Integer
        Public Spe_Des As String
        Public Raz_Cod As Integer
        Public Raz_Des As String
        Public Nome_Animale As String
        Public Rif_Esterno As String
        Public Rif_Esterno_2 As String
        Public StornatoTotalmente As Boolean
    End Class

End Class

Public Class RispostaReportBio
    Public IsLink As Boolean
    Public Risposta As String
    Public TipoReport As Integer
End Class