Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreAnagrafeDAL
Imports System.IO

Public Class Scheda_OP_Tipo_2
    Inherits System.Web.UI.Page

    '----- Gestione Querystring
    Dim Piva As String
    Dim Sa_Cod As String
    Dim Anno As String
    Dim Rag_Soc As String
    Dim Tipo_Stampa As String
    Dim Tipo_Selezione As String
    Dim Descr As String
    Dim validita_inizio, validita_fine As String
    Dim fronteRetro As Boolean

    Private rptStampa As Rpt_Scheda_OP_Tipo_2

    'oggetto objparametri x server e utenti
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Quadro_P_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        rptStampa = New Rpt_Scheda_OP_Tipo_2

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim Log As String

        Dim Sup_Condotta As Double

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '#################################################################################
        '#####  Recupero i dati dalla QueryString 
        '#################################################################################

        Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                   AgroKey_EncoderDecoder,
                                   Server)

        Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString,
                           AgroKey_EncoderDecoder,
                           Server)

        Rag_Soc = Stringa_Decodifica(Request.QueryString("r").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Anno = Stringa_Decodifica(Request.QueryString("a").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Tipo_Stampa = Stringa_Decodifica(Request.QueryString("t").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Tipo_Selezione = Stringa_Decodifica(Request.QueryString("tipo").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        fronteRetro = Stringa_Decodifica(Request.QueryString("fronteretro").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        '04/03/2021: sostituito passaggio in querystring con session per evitare errore:
        'HTTP Error 404.15 - Not Found
        'Il modulo filtro delle richieste è configurato per negare una richiesta quando la stringa di query è troppo lunga.
        'Descr = Stringa_Decodifica(Request.QueryString("des").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)
        Descr = Session("DescrImpianti")
        Session("DescrImpianti") = Nothing

        If Not IsNothing(Request.QueryString("vi")) Then
            validita_inizio = Stringa_Decodifica(Request.QueryString("vi").ToString,
                      AgroKey_EncoderDecoder, Server)
        Else
            validita_inizio = "01/01/" + CStr(Anno)
        End If

        If Not IsNothing(Request.QueryString("vf")) Then
            validita_fine = Stringa_Decodifica(Request.QueryString("vf").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)
        Else
            validita_fine = "31/12/" + CStr(Anno)
        End If


        Dim Log_Errori As String = ""

        Dim CatCod As enum_CategorieDocumenti
        Dim Nome_Documento As String

        If Not Me.IsPostBack Then

            rptStampa.Section2b.SectionFormat.EnableSuppress = True
            rptStampa.Section3.SectionFormat.EnableSuppress = True
            rptStampa.Section25.SectionFormat.EnableSuppress = True
            rptStampa.Section7.SectionFormat.EnableSuppress = True

            rptStampa.Section17.SectionFormat.EnableSuppress = True
            rptStampa.Section18.SectionFormat.EnableSuppress = True

            rptStampa.Section12.SectionFormat.EnableSuppress = True
            rptStampa.Section15.SectionFormat.EnableSuppress = True
            rptStampa.Section6.SectionFormat.EnableSuppress = True
            rptStampa.Section23.SectionFormat.EnableSuppress = True
            rptStampa.Section26.SectionFormat.EnableSuppress = True
            rptStampa.Section9.SectionFormat.EnableSuppress = True
            rptStampa.Section27.SectionFormat.EnableSuppress = True
            rptStampa.Section4.SectionFormat.EnableSuppress = True

            rptStampa.Section10.SectionFormat.EnableSuppress = True
            rptStampa.Section28.SectionFormat.EnableSuppress = True
            rptStampa.Section11.SectionFormat.EnableSuppress = True
            rptStampa.Section13.SectionFormat.EnableSuppress = True
            rptStampa.Section14.SectionFormat.EnableSuppress = True

            rptStampa.Section19.SectionFormat.EnableSuppress = True
            rptStampa.Section20.SectionFormat.EnableSuppress = True
            rptStampa.Section21.SectionFormat.EnableSuppress = True
            rptStampa.Section22.SectionFormat.EnableSuppress = True

            rptStampa.Section30.SectionFormat.EnableSuppress = True
            rptStampa.Section31.SectionFormat.EnableSuppress = True
            rptStampa.Section32.SectionFormat.EnableSuppress = True
            rptStampa.Section33.SectionFormat.EnableSuppress = True
            rptStampa.SectionFirmaFede.SectionFormat.EnableSuppress = True
            rptStampa.SectionDataFirma.SectionFormat.EnableSuppress = True

            '----------------------------------------------------------------------------------------


            'LOGO
            rptStampa.Section2.ReportObjects("PictureTerremerse").ObjectFormat.EnableSuppress = True
            rptStampa.Section2.ReportObjects("PicturePempacorer").ObjectFormat.EnableSuppress = True

            CType(rptStampa.Section24.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "IMPEGNO IRREVOCABILE DI CONFERIMENTO RELATIVO ALLA CAMPAGNA " & Today.Year.ToString
            CType(rptStampa.Section24.ReportObjects("TextColtura"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr

            'INTESTAZIONE
            Dim DSLoghi As New Ds_Loghi
            Dim DSImprese As New Ds_Imprese_StampaMassiva
            CaricaDs_Loghi(DSLoghi, True)

            Dim strFiltroImprese As String = If(Session("strParametri") IsNot Nothing, Session("strParametri").ToString(), "")
            Dim objStampe As New AgronicaCoreStampeDAL.Stampe_OP
            objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)

            Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim pivaReale As String = objImp.Leggi_PivaReale(Piva, objParametri_Server)

            rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
            rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)

            Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Select Case Tipo_Stampa

                Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Breve

                    Nome_Documento = "ImpegnativaOrticoleGestioneBreve"
                    CatCod = enum_CategorieDocumenti.ImpegnativaOrticoleGestioneBreve

                    '-------------------
                    'dati centro
                    rptStampa.Section3.SectionFormat.EnableSuppress = False
                    rptStampa.Section25.SectionFormat.EnableSuppress = False

                    Dim DtCentro As New DataTable
                    Dim Sa_Nome As String

                    DtCentro = objCentriAz.Leggi(Piva, Sa_Cod,
                                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                 "", "", objParametri_Server)

                    If Not DtCentro Is Nothing AndAlso DtCentro.Rows.Count > 0 Then

                        Select Case CInt(DtCentro.Rows(0).Item("Titolo_Possesso"))
                            Case 1
                                CType(rptStampa.Section3.ReportObjects("TextProprietario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "PROPRIETARIO"
                            Case Else
                                CType(rptStampa.Section3.ReportObjects("TextProprietario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "AFFITTUARIO"
                        End Select

                        CType(rptStampa.Section3.ReportObjects("TextComuneCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtCentro.Rows(0).Item("com_des") & " (" & DtCentro.Rows(0).Item("pro_cod") & ")"
                        CType(rptStampa.Section3.ReportObjects("TextIndirizzoCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtCentro.Rows(0).Item("ind_des")

                        Sa_Nome = DtCentro.Rows(0).Item("Sa_Nome")

                    End If

                    objCentriAz.Recupera_Superfici_CentroAziendale(Piva, Sa_Cod, Sup_Condotta, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Date.Today, objParametri_Server)

                    CType(rptStampa.Section3.ReportObjects("TextSupCondotta"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sup_Condotta.ToString

                    CType(rptStampa.Section3.ReportObjects("TextSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr


                    '-------------------
                    'particelle
                    rptStampa.Section7.SectionFormat.EnableSuppress = False

                    Dim DSParticelle As New DS_Scheda_OP_Tipo_2
                    Dim Sup_Specie As Double = 0

                    CaricaDs_Particelle(DSParticelle, Sup_Specie)

                    CType(rptStampa.Section25.ReportObjects("TextSupSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Sup_Specie, "0.0000")

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Catasto.rpt").SetDataSource(DSParticelle)

                    '-------------------
                    'testo
                    rptStampa.Section12.SectionFormat.EnableSuppress = False
                    rptStampa.Section15.SectionFormat.EnableSuppress = False
                    rptStampa.Section4.SectionFormat.EnableSuppress = False

                    CType(rptStampa.Section12.ReportObjects("TextSpecie2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr
                    CType(rptStampa.Section15.ReportObjects("TextSupSpecie2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Sup_Specie, "0.0000")

                    CType(rptStampa.Section15.ReportObjects("TextConferimento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "	CONFERIMENTO IN GESTIONE COMM.LE BREVE  : 	 HA_________________Q.LI  ________________________"

                    rptStampa.Section10.SectionFormat.EnableSuppress = False
                    rptStampa.Section10.SectionFormat.EnableNewPageBefore = True

                    rptStampa.Section28.SectionFormat.EnableSuppress = False
                    rptStampa.Section2b.SectionFormat.EnableSuppress = True

                    CType(rptStampa.Section10.ReportObjects("TextPianificazioneSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr
                    CType(rptStampa.Section28.ReportObjects("TextAziendaPianificazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Rag_Soc
                    CType(rptStampa.Section28.ReportObjects("TextCentroPianificazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sa_Nome

                    '-------------------
                    'impianti

                    rptStampa.Section11.SectionFormat.EnableSuppress = False

                    Dim DSImpianti As New DS_Scheda_OP_Tipo_2_Varieta

                    CaricaDs_Impianti(DSImpianti)

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Varieta.rpt").SetDataSource(DSImpianti)

                    rptStampa.Section14.SectionFormat.EnableSuppress = False


                Case enum_CodificaStampe.Impegnativa_Orticole_Gest_Annuale

                    Nome_Documento = "ImpegnativaOrticoleGestioneAnnuale"
                    CatCod = enum_CategorieDocumenti.ImpegnativaOrticoleGestioneAnnuale
                    '-------------------
                    'dati centro
                    rptStampa.Section3.SectionFormat.EnableSuppress = False
                    rptStampa.Section25.SectionFormat.EnableSuppress = False

                    Dim DtCentro As New DataTable
                    Dim Sa_Nome As String

                    DtCentro = objCentriAz.Leggi(Piva, Sa_Cod,
                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                            "", "", objParametri_Server)

                    If Not DtCentro Is Nothing AndAlso DtCentro.Rows.Count > 0 Then

                        Select Case CInt(DtCentro.Rows(0).Item("Titolo_Possesso"))
                            Case 1
                                CType(rptStampa.Section3.ReportObjects("TextProprietario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "PROPRIETARIO"
                            Case Else
                                CType(rptStampa.Section3.ReportObjects("TextProprietario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "AFFITTUARIO"
                        End Select

                        CType(rptStampa.Section3.ReportObjects("TextComuneCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtCentro.Rows(0).Item("com_des") & " (" & DtCentro.Rows(0).Item("pro_cod") & ")"
                        CType(rptStampa.Section3.ReportObjects("TextIndirizzoCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtCentro.Rows(0).Item("ind_des")

                        Sa_Nome = DtCentro.Rows(0).Item("Sa_Nome")

                    End If

                    objCentriAz.Recupera_Superfici_CentroAziendale(Piva, Sa_Cod, Sup_Condotta, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Date.Today, objParametri_Server)

                    CType(rptStampa.Section3.ReportObjects("TextSupCondotta"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sup_Condotta.ToString

                    CType(rptStampa.Section3.ReportObjects("TextSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr

                    '-------------------
                    'particelle
                    rptStampa.Section7.SectionFormat.EnableSuppress = False

                    Dim DSParticelle As New DS_Scheda_OP_Tipo_2
                    Dim Sup_Specie As Double = 0

                    CaricaDs_Particelle(DSParticelle, Sup_Specie)

                    CType(rptStampa.Section25.ReportObjects("TextSupSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Sup_Specie, "0.0000")

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Catasto.rpt").SetDataSource(DSParticelle)


                    '-------------------
                    'testo
                    rptStampa.Section12.SectionFormat.EnableSuppress = False
                    rptStampa.Section15.SectionFormat.EnableSuppress = False
                    rptStampa.Section4.SectionFormat.EnableSuppress = False


                    CType(rptStampa.Section12.ReportObjects("TextSpecie2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr
                    CType(rptStampa.Section15.ReportObjects("TextSupSpecie2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Sup_Specie, "0.0000")

                    CType(rptStampa.Section15.ReportObjects("TextConferimento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "	CONFERIMENTO IN GESTIONE COMM.LE ANNUALE  : 	 HA_________________Q.LI  ________________________"

                    rptStampa.Section10.SectionFormat.EnableSuppress = False
                    rptStampa.Section10.SectionFormat.EnableNewPageBefore = True

                    rptStampa.Section28.SectionFormat.EnableSuppress = False

                    CType(rptStampa.Section10.ReportObjects("TextPianificazioneSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr
                    CType(rptStampa.Section28.ReportObjects("TextAziendaPianificazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Rag_Soc
                    CType(rptStampa.Section28.ReportObjects("TextCentroPianificazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sa_Nome

                    '-------------------
                    'impianti

                    rptStampa.Section13.SectionFormat.EnableSuppress = False

                    Dim DSImpianti As New DS_Scheda_OP_Tipo_2_Varieta

                    CaricaDs_Impianti(DSImpianti)

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Varieta_2.rpt").SetDataSource(DSImpianti)

                    rptStampa.Section14.SectionFormat.EnableSuppress = False



                Case enum_CodificaStampe.Impegnativa_Orticole_Industria

                    Nome_Documento = "ImpegnativaOrticoleIndustria"
                    CatCod = enum_CategorieDocumenti.ImpegnativaOrticoleIndustria

                    '-------------------
                    'dati centro
                    rptStampa.Section3.SectionFormat.EnableSuppress = False
                    rptStampa.Section25.SectionFormat.EnableSuppress = False

                    Dim DtCentro As New DataTable
                    Dim Sa_Nome As String

                    DtCentro = objCentriAz.Leggi(Piva, Sa_Cod,
                                           AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                           "", "", objParametri_Server)

                    If Not DtCentro Is Nothing AndAlso DtCentro.Rows.Count > 0 Then

                        Select Case CInt(DtCentro.Rows(0).Item("Titolo_Possesso"))
                            Case 1
                                CType(rptStampa.Section3.ReportObjects("TextProprietario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "PROPRIETARIO"
                            Case Else
                                CType(rptStampa.Section3.ReportObjects("TextProprietario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "AFFITTUARIO"
                        End Select

                        CType(rptStampa.Section3.ReportObjects("TextComuneCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtCentro.Rows(0).Item("com_des") & " (" & DtCentro.Rows(0).Item("pro_cod") & ")"
                        CType(rptStampa.Section3.ReportObjects("TextIndirizzoCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtCentro.Rows(0).Item("ind_des")

                        Sa_Nome = DtCentro.Rows(0).Item("Sa_Nome")

                    End If

                    objCentriAz.Recupera_Superfici_CentroAziendale(Piva, Sa_Cod, Sup_Condotta, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Date.Today, objParametri_Server)

                    CType(rptStampa.Section3.ReportObjects("TextSupCondotta"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sup_Condotta.ToString

                    CType(rptStampa.Section3.ReportObjects("TextSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr

                    '-------------------
                    'particelle
                    rptStampa.Section7.SectionFormat.EnableSuppress = False

                    Dim DSParticelle As New DS_Scheda_OP_Tipo_2
                    Dim Sup_Specie As Double = 0

                    CaricaDs_Particelle(DSParticelle, Sup_Specie)

                    CType(rptStampa.Section25.ReportObjects("TextSupSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Sup_Specie, "0.0000")

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Catasto.rpt").SetDataSource(DSParticelle)

                    '-------------------
                    'testo
                    rptStampa.Section23.SectionFormat.EnableSuppress = False
                    rptStampa.Section26.SectionFormat.EnableSuppress = False
                    rptStampa.Section9.SectionFormat.EnableSuppress = False
                    rptStampa.Section27.SectionFormat.EnableSuppress = False
                    rptStampa.Section4.SectionFormat.EnableSuppress = False

                    rptStampa.Section4.ReportObjects("TextPunto5").ObjectFormat.EnableSuppress = True

                    rptStampa.Section10.SectionFormat.EnableSuppress = False
                    rptStampa.Section10.SectionFormat.EnableNewPageBefore = True

                    rptStampa.Section28.SectionFormat.EnableSuppress = False

                    CType(rptStampa.Section10.ReportObjects("TextPianificazioneSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr
                    CType(rptStampa.Section28.ReportObjects("TextAziendaPianificazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Rag_Soc
                    CType(rptStampa.Section28.ReportObjects("TextCentroPianificazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sa_Nome

                    '-------------------
                    'impianti

                    rptStampa.Section11.SectionFormat.EnableSuppress = False

                    Dim DSImpianti As New DS_Scheda_OP_Tipo_2_Varieta

                    CaricaDs_Impianti(DSImpianti)

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Varieta.rpt").SetDataSource(DSImpianti)

                    rptStampa.Section14.SectionFormat.EnableSuppress = False



                Case enum_CodificaStampe.Impegnativa_Fagiolino_Mercato_Fresco

                    Nome_Documento = "ImpegnativaFagiolinoMercatoFresco"
                    CatCod = enum_CategorieDocumenti.ImpegnativaFagiolinoMercatoFresco


                    '-------------------
                    'dati centro
                    rptStampa.Section3.SectionFormat.EnableSuppress = False
                    rptStampa.Section25.SectionFormat.EnableSuppress = False

                    Dim DtCentro As New DataTable
                    Dim Sa_Nome As String

                    DtCentro = objCentriAz.Leggi(Piva, Sa_Cod,
                                              AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                              "", "", objParametri_Server)

                    If Not DtCentro Is Nothing AndAlso DtCentro.Rows.Count > 0 Then

                        Select Case CInt(DtCentro.Rows(0).Item("Titolo_Possesso"))
                            Case 1
                                CType(rptStampa.Section3.ReportObjects("TextProprietario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "PROPRIETARIO"
                            Case Else
                                CType(rptStampa.Section3.ReportObjects("TextProprietario"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "AFFITTUARIO"
                        End Select

                        CType(rptStampa.Section3.ReportObjects("TextComuneCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtCentro.Rows(0).Item("com_des") & " (" & DtCentro.Rows(0).Item("pro_cod") & ")"
                        CType(rptStampa.Section3.ReportObjects("TextIndirizzoCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtCentro.Rows(0).Item("ind_des")

                        Sa_Nome = DtCentro.Rows(0).Item("Sa_Nome")

                    End If

                    objCentriAz.Recupera_Superfici_CentroAziendale(Piva, Sa_Cod, Sup_Condotta, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Nothing, Date.Today, objParametri_Server)

                    CType(rptStampa.Section3.ReportObjects("TextSupCondotta"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sup_Condotta.ToString

                    CType(rptStampa.Section3.ReportObjects("TextSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr

                    CType(rptStampa.Section12.ReportObjects("TextSpecie2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr

                    '-------------------
                    'particelle
                    rptStampa.Section7.SectionFormat.EnableSuppress = False

                    Dim DSParticelle As New DS_Scheda_OP_Tipo_2
                    Dim Sup_Specie As Double = 0

                    CaricaDs_Particelle(DSParticelle, Sup_Specie)

                    CType(rptStampa.Section25.ReportObjects("TextSupSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Sup_Specie, "0.0000")

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Catasto.rpt").SetDataSource(DSParticelle)

                    '-------------------
                    'testo
                    rptStampa.Section6.SectionFormat.EnableSuppress = False
                    rptStampa.Section4.SectionFormat.EnableSuppress = False

                    rptStampa.Section4.ReportObjects("TextPunto5").ObjectFormat.EnableSuppress = True

                    rptStampa.Section10.SectionFormat.EnableSuppress = False
                    rptStampa.Section10.SectionFormat.EnableNewPageBefore = True

                    rptStampa.Section28.SectionFormat.EnableSuppress = False

                    CType(rptStampa.Section10.ReportObjects("TextPianificazioneSpecie"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr
                    CType(rptStampa.Section28.ReportObjects("TextAziendaPianificazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Rag_Soc
                    CType(rptStampa.Section28.ReportObjects("TextCentroPianificazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Sa_Nome

                    '-------------------
                    'impianti

                    rptStampa.Section11.SectionFormat.EnableSuppress = False

                    Dim DSImpianti As New DS_Scheda_OP_Tipo_2_Varieta

                    CaricaDs_Impianti(DSImpianti)

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Varieta.rpt").SetDataSource(DSImpianti)

                    rptStampa.Section14.SectionFormat.EnableSuppress = False


                Case enum_CodificaStampe.Impegnativa_Pomodoro_Industria

                    Nome_Documento = "ImpegnativaPomodoroIndustria"
                    CatCod = enum_CategorieDocumenti.ImpegnativaPomodoroIndustria


                    If objParametri_Server.PivaSuperUser <> "00069880391" Then
                        rptStampa.Section2.ReportObjects("PicturePempacorer").ObjectFormat.EnableSuppress = True
                    Else
                        rptStampa.Section2.ReportObjects("PicturePempacorer").ObjectFormat.EnableSuppress = False
                    End If

                    rptStampa.Section2.ReportObjects("TextProtocollo1").ObjectFormat.EnableSuppress = True
                    rptStampa.Section2.ReportObjects("TextProtocollo2").ObjectFormat.EnableSuppress = True

                    CType(rptStampa.Section24.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "IMPEGNATIVA DI CONFERIMENTO RELATIVA ALLA CAMPAGNA POMODORO DA INDUSTRIA " & Today.Year.ToString

                    rptStampa.Section24.ReportObjects("TextColtura").ObjectFormat.EnableSuppress = True

                    rptStampa.Section7.SectionFormat.EnableSuppress = True

                    rptStampa.Section16.ReportObjects("TextPremesso").ObjectFormat.EnableSuppress = True
                    rptStampa.Section16.ReportObjects("Texte").ObjectFormat.EnableSuppress = True

                    rptStampa.Section16.ReportObjects("TextIscritto1").ObjectFormat.EnableSuppress = False
                    rptStampa.Section16.ReportObjects("TextIscritto2").ObjectFormat.EnableSuppress = False
                    rptStampa.Section16.ReportObjects("TextNLibroSoci").ObjectFormat.EnableSuppress = False

                    rptStampa.Section17.SectionFormat.EnableSuppress = False

                    '-------------------
                    'particelle
                    rptStampa.Section18.SectionFormat.EnableSuppress = False

                    Dim DSParticelle As New DS_Scheda_OP_Tipo_2
                    Dim Sup_Specie As Double = 0

                    CaricaDs_Particelle(DSParticelle, Sup_Specie)

                    CType(rptStampa.Section17.ReportObjects("TextSupPomodoro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Sup_Specie, "0.0000")

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Catasto_3.rpt").SetDataSource(DSParticelle)

                    '-------------------
                    'testo
                    rptStampa.Section19.SectionFormat.EnableSuppress = False

                    'INTESTAZIONE
                    rptStampa.Section20.SectionFormat.EnableSuppress = False
                    rptStampa.Section20.SectionFormat.EnableNewPageBefore = True

                    CType(rptStampa.Section20.ReportObjects("TextPiva2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = pivaReale
                    CType(rptStampa.Section20.ReportObjects("TextAzienda2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Rag_Soc

                    CType(rptStampa.Section20.ReportObjects("TextSupPomodoro2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Sup_Specie, "0.0000")

                    '-------------------
                    'impianti

                    rptStampa.Section21.SectionFormat.EnableSuppress = False

                    Dim DSImpianti As New DS_Scheda_OP_Tipo_2_Varieta

                    CaricaDs_Impianti(DSImpianti)

                    rptStampa.OpenSubreport("Rpt_Scheda_OP_Tipo_2_Varieta_3.rpt").SetDataSource(DSImpianti)


                    rptStampa.Section22.SectionFormat.EnableSuppress = False

                Case enum_CodificaStampe.Dichiarazione_di_Responsabilita

                    Nome_Documento = "DichiarazioneDiResponsabilita"
                    CatCod = enum_CategorieDocumenti.DichiarazioneResponsabilita

                    Dim ImpreseCodici_Read As New Imprese_Codici_Read

                    rptStampa.Section2.ReportObjects("PicturePempacorer").ObjectFormat.EnableSuppress = True


                    rptStampa.Section2.ReportObjects("TextProtocollo1").ObjectFormat.EnableSuppress = True
                    rptStampa.Section2.ReportObjects("TextProtocollo2").ObjectFormat.EnableSuppress = True
                    rptStampa.Section2.SectionFormat.EnableSuppress = True

                    CType(rptStampa.Section2b.ReportObjects("TextTitolo1b"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DICHIARAZIONE DI RESPONSABILITÀ Ai sensi del D.P.R. n 445 del 28/12/2000"

                    rptStampa.Section24.SectionFormat.EnableSuppress = True

                    rptStampa.Section7.SectionFormat.EnableSuppress = True

                    rptStampa.Section33.SectionFormat.EnableSuppress = False

                    rptStampa.Section16.ReportObjects("TextPremesso").ObjectFormat.EnableSuppress = True
                    rptStampa.Section16.ReportObjects("Texte").ObjectFormat.EnableSuppress = True

                    rptStampa.Section16.SectionFormat.EnableSuppress = False
                    rptStampa.Section16.ReportObjects("LabelCUAA").ObjectFormat.EnableSuppress = True
                    rptStampa.Section16.ReportObjects("TextCUAA").ObjectFormat.EnableSuppress = True
                    rptStampa.Section30.SectionFormat.EnableSuppress = False
                    rptStampa.SectionDataFirma.SectionFormat.EnableSuppress = False
                    rptStampa.Section2b.SectionFormat.EnableSuppress = False

                    CType(rptStampa.Section2b.ReportObjects("TextAllegato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Mod. PP/DG/03-6 Rev. 3"
                    rptStampa.Section2b.ReportObjects("TextAllegato").ObjectFormat.EnableSuppress = False

                Case enum_CodificaStampe.Fitoregolatori_Kiwi

                    Nome_Documento = "FitoregolatoriKiwi"
                    CatCod = enum_CategorieDocumenti.FitoregolatoriKiwi

                    rptStampa.Section2.ReportObjects("PicturePempacorer").ObjectFormat.EnableSuppress = True

                    rptStampa.Section2.ReportObjects("TextProtocollo1").ObjectFormat.EnableSuppress = True
                    rptStampa.Section2.ReportObjects("TextProtocollo2").ObjectFormat.EnableSuppress = True

                    CType(rptStampa.Section2b.ReportObjects("TextTitolo1b"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DICHIARAZIONE DI RESPONSABILITÀ UTILIZZO FITOREGOLATORI SU KIWI"

                    rptStampa.Section24.SectionFormat.EnableSuppress = True

                    rptStampa.Section7.SectionFormat.EnableSuppress = True

                    rptStampa.Section16.SectionFormat.EnableSuppress = True
                    rptStampa.Section16.ReportObjects("TextPremesso").ObjectFormat.EnableSuppress = True
                    rptStampa.Section16.ReportObjects("Texte").ObjectFormat.EnableSuppress = True

                    rptStampa.Section32.SectionFormat.EnableSuppress = False
                    rptStampa.Section31.SectionFormat.EnableSuppress = False
                    rptStampa.SectionFirmaFede.SectionFormat.EnableSuppress = False
                    rptStampa.Section2b.SectionFormat.EnableSuppress = False

                    CType(rptStampa.Section2b.ReportObjects("TextAllegato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Mod. 0.00 Rev.0"
                    rptStampa.Section2b.ReportObjects("TextAllegato").ObjectFormat.EnableSuppress = False

            End Select


            ' imposto il parametro del fronte-retro
            rptStampa.SetParameterValue("fronteRetro", fronteRetro)

            '----------------------------------------------------------------------------------------

            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(CatCod, "", "", objParametri_Server)
            objCatDoc = Nothing

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            objGestFile.SalvaReportPdf(rptStampa,
                                       CatCod,
                                       Sottocartella,
                                       Nome_Documento + "_p" + Piva + "_d" + Anno + ".pdf",
                                       objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer

            AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva,
                                                             CatCod,
                                                             Nome_Documento,
                                                             Nome_Documento + "_p" + Piva + "_d" + Anno + ".pdf",
                                                             Sottocartella,
                                                             "", "", "", "",
                                                             CDate("01/01/" & Anno),
                                                             CDate("31/12/" & Anno),
                                                             objParametri_Server)


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Nome_File As String
            ' Dim Str_Errore_Path As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Piva = " + CStr(Piva) + vbCrLf + vbCrLf + Log_Errori

                Nome_File = "LogErrori_" + Nome_Documento + "_p" & Piva + "_d" + Anno + CStr(Session("ASG_Utente_Username")) + ".txt"

                Dim objLog As New AgronicaCoreDataProvider.LogProvider
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                If objAgroWeb.PathDirectoryLOG <> "" Then
                    objParametri_Server.LogDirectory = ""
                    Path_Errore = objAgroWeb.PathDirectoryLOG & Nome_Documento
                Else
                    Path_Errore = "C:\Agronica_LOG\Stampe_OP"
                End If

                Dim CustomLOGParams As New AgronicaCoreDataProvider.CustomLOGParams With {
                    .LogDescrizioneUtente = Session("ASG_Utente_Username"),
                    .LogDirectory = Path_Errore,
                    .LogFileName = Nome_File
                }

                'objLog.Scrivi_LOG(Path_Errore, Nome_File, Session("ASG_Utente_Username"), "Qs_Piva.Page_Load", Log_Errori)
                objLog.Scrivi_LOG(objParametri_Server, "Qs_Piva.Page_Load", Log_Errori, CustomLOGParams:=CustomLOGParams)


            End If
            '-----------------------------------------

            Session("Report") = rptStampa
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))



        End If


    End Sub

    '###########################################################################
    Public Sub CaricaDs_Particelle(ByRef DSParticelle As DS_Scheda_OP_Tipo_2,
                                   ByRef Sup_Investita_Tot As Double)


        Dim strErr As String
        Dim stbQuery As New System.Text.StringBuilder

        Dim strFiltroImpianti As String = ""

        Dim i, j As Integer
        Dim Dt As DataTable
        Dim RigaDs As DS_Scheda_OP_Tipo_2.DT_Scheda_OP_Tipo_2Row

        Dim TrovataParticella As Boolean

        stbQuery.Length = 0

        stbQuery.AppendLine("SELECT CASE ")
        stbQuery.AppendLine("  WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Reg_Impianti.PIVA ")
        stbQuery.AppendLine("  ELSE Imprese.partitaIvaReale ")
        stbQuery.AppendLine("END PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, Reg_Impianti.Sup_Imp,  ")
        stbQuery.Append(" AppezzamentiXParticelle.PROV, AppezzamentiXParticelle.COM, AppezzamentiXParticelle.SEZIONE, AppezzamentiXParticelle.FOGLIO, AppezzamentiXParticelle.NUMERO, AppezzamentiXParticelle.SUBALTERNO, AppezzamentiXParticelle.AREA, ")
        stbQuery.Append(" ISTAT.LOCALITA, ISTAT.COMUNI_PROV, ")
        stbQuery.Append(" ImpreseXParticelle.Sup_Condotta, ImpreseXParticelle.TitoloPossesso, ")

        stbQuery.Append(" CASE ImpreseXParticelle.TitoloPossesso WHEN 1 THEN 'Proprietà' WHEN 2 THEN 'Comodato' ")
        stbQuery.Append("                                        WHEN 3 THEN 'Affitto con contratto' WHEN 4 THEN 'Affitto senza contratto' ")
        stbQuery.Append("                                        WHEN 5 THEN 'In conto terzi' ELSE 'Altro' END AS TitoloPossesso_Des ")
        stbQuery.Append("         ")

        stbQuery.Append(" FROM Reg_Impianti ")
        stbQuery.Append(" INNER JOIN Imprese ON Reg_Impianti.PIVA = Imprese.PIVA ")
        stbQuery.Append(" INNER JOIN AppezzamentiXParticelle ON Reg_Impianti.PIVA = AppezzamentiXParticelle.PIVA AND Reg_Impianti.SA_COD = AppezzamentiXParticelle.SA_COD AND  ")
        stbQuery.Append(" Reg_Impianti.APPEZZA = AppezzamentiXParticelle.APPEZZA ")
        stbQuery.Append(" INNER JOIN  ISTAT ON AppezzamentiXParticelle.PROV = ISTAT.PROV AND AppezzamentiXParticelle.COM = ISTAT.COM ")
        stbQuery.Append(" INNER JOIN ImpreseXParticelle ON AppezzamentiXParticelle.PIVA = ImpreseXParticelle.PIVA AND  ")
        stbQuery.Append(" AppezzamentiXParticelle.SA_COD = ImpreseXParticelle.sa_cod AND AppezzamentiXParticelle.PROV = ImpreseXParticelle.PROV AND  ")
        stbQuery.Append(" AppezzamentiXParticelle.COM = ImpreseXParticelle.COM AND AppezzamentiXParticelle.SEZIONE = ImpreseXParticelle.SEZIONE AND  ")
        stbQuery.Append(" AppezzamentiXParticelle.FOGLIO = ImpreseXParticelle.FOGLIO AND AppezzamentiXParticelle.NUMERO = ImpreseXParticelle.NUMERO AND  ")
        stbQuery.Append(" AppezzamentiXParticelle.SUBALTERNO = ImpreseXParticelle.SUBALTERNO ")

        If Not Session("strParametri") Is Nothing Then
            strFiltroImpianti = Session("strParametri").ToString()
            'Session("strParametri") = Nothing
        End If

        stbQuery.Append(" WHERE 1 = 1 " & strFiltroImpianti)

        stbQuery.Append(" AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQuery.Append(" AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQuery.Append(" ORDER BY LOCALITA, AppezzamentiXParticelle.FOGLIO, AppezzamentiXParticelle.NUMERO, AppezzamentiXParticelle.SEZIONE ")

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            Dt = objSQL.EseguiQuery_Lettura(objParametri_Server, stbQuery.ToString, "Scheda_OP_Tipo_2.CaricaDsParticelle")
        Catch ex As Exception
            strErr = ex.Message
        End Try

        'Dt = objSQL.SqlSelect(objParametri_Server.StringaConnessione, _
        '              Session("ASG_Connessione_Server"), _
        '              stbQuery.ToString, _
        '              1, _
        '              strErr)

        If Not Dt Is Nothing AndAlso Dt.Rows.Count > 0 Then

            For i = 0 To Dt.Rows.Count - 1

                TrovataParticella = False

                For j = 0 To DSParticelle.DT_Scheda_OP_Tipo_2.Count - 1

                    If Dt.Rows(i).Item("Prov") = DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Prov") And
                        Dt.Rows(i).Item("Com") = DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Com") And
                        Dt.Rows(i).Item("Sezione") = DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Sezione") And
                        Dt.Rows(i).Item("Foglio") = DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Foglio") And
                        Dt.Rows(i).Item("Numero") = DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Numero") And
                        Dt.Rows(i).Item("Subalterno") = DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Subalterno") Then

                        DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Sup_Impiegata") += CDbl(Dt.Rows(i).Item("area"))

                        TrovataParticella = True

                        Exit For

                    End If

                Next

                If TrovataParticella = False Then

                    RigaDs = DSParticelle.DT_Scheda_OP_Tipo_2.NewDT_Scheda_OP_Tipo_2Row

                    RigaDs.Comune = Dt.Rows(i).Item("LOCALITA")

                    RigaDs.Prov = Dt.Rows(i).Item("Prov")
                    RigaDs.Com = Dt.Rows(i).Item("Com")
                    RigaDs.Sezione = Dt.Rows(i).Item("Sezione")
                    RigaDs.Foglio = Dt.Rows(i).Item("Foglio")
                    RigaDs.Numero = Dt.Rows(i).Item("Numero")
                    RigaDs.Subalterno = Dt.Rows(i).Item("Subalterno")

                    RigaDs.Titolo_Possesso = Dt.Rows(i).Item("TitoloPossesso")
                    RigaDs.Titolo_Possesso_Des = Dt.Rows(i).Item("TitoloPossesso_Des")

                    RigaDs.Sup_Condotta = Format(CDbl(Dt.Rows(i).Item("Sup_Condotta")), "0.0000")

                    RigaDs.Sup_Impiegata = CDbl(Dt.Rows(i).Item("area"))

                    DSParticelle.DT_Scheda_OP_Tipo_2.Rows.Add(RigaDs)

                End If

            Next


        End If

        For j = 0 To DSParticelle.DT_Scheda_OP_Tipo_2.Count - 1

            Sup_Investita_Tot += DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Sup_Impiegata")

            DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Sup_Impiegata") = Format(CDbl(DSParticelle.DT_Scheda_OP_Tipo_2.Rows(j).Item("Sup_Impiegata")), "0.0000")

        Next

        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else


        End If


    End Sub

    '###########################################################################
    Public Sub CaricaDs_Impianti(ByRef DSImpianti As DS_Scheda_OP_Tipo_2_Varieta)

        Dim strErr As String
        Dim stbQuery As New System.Text.StringBuilder

        Dim strFiltroImpianti As String = ""

        stbQuery.Length = 0

        stbQuery.AppendLine("SELECT CASE ")
        stbQuery.AppendLine("  WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Reg_Impianti.PIVA ")
        stbQuery.AppendLine("  ELSE Imprese.partitaIvaReale ")
        stbQuery.AppendLine("END PIVA, Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG,  ")
        stbQuery.Append(" SpecieVegetali.Veg_Cod, SpecieVegetali.Veg_Des,  ")
        stbQuery.Append(" Cultivar.Cul_Cod, Cultivar.Cul_Des, Reg_Impianti.GRVA_Cod_VEG, ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, Reg_Impianti.Sup_Imp ")

        stbQuery.Append(" FROM SpecieVegetali ")
        stbQuery.Append(" INNER JOIN Cultivar ON SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod ")
        stbQuery.Append(" INNER JOIN Reg_Impianti ON Cultivar.Cul_Cod = Reg_Impianti.CUL_COD ")
        stbQuery.Append(" INNER JOIN Imprese ON Reg_Impianti.PIVA = Imprese.PIVA ")
        stbQuery.Append(" LEFT OUTER JOIN GruppoVarietale ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod ")

        If Not Session("strParametri") Is Nothing Then
            strFiltroImpianti = Session("strParametri").ToString()
            Session("strParametri") = Nothing
        End If

        stbQuery.Append(" WHERE 1 = 1 " & strFiltroImpianti)

        stbQuery.Append(" AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        stbQuery.Append(" AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        stbQuery.Append(" ORDER BY Cul_Des, Grva_Des ")


        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            objSQL.EseguiQuery_Lettura(objParametri_Server, stbQuery.ToString, "Scheda_OP_Tipo_2.CaricaDs_Impianti", DSImpianti, DSImpianti.DT_Scheda_OP_Tipo_2_Varieta.TableName)
        Catch ex As Exception
            strErr = ex.Message
        End Try


        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else


        End If


    End Sub


    Sub CaricaDs_Loghi(ByVal DsLoghi As Ds_Loghi, ByVal caricaLogo1 As Boolean)

        'DsLoghi.LoghiDataTable.NewLoghiRow()
        'Dim r As Ds_Loghi.LoghiRow = DsLoghi.Loghi.NewLoghiRow()
        Dim r As DataRow = DsLoghi.Loghi.NewLoghiRow()
        Dim x(0) As Byte
        x(0) = 0
        Dim bmpFx As System.Drawing.Bitmap = New System.Drawing.Bitmap(1, 1)
        bmpFx.SetPixel(0, 0, System.Drawing.Color.White)
        Dim logox() As Byte
        Dim cx As New System.Drawing.ImageConverter
        logox = cx.ConvertTo(bmpFx, GetType(Byte()))
        r.Item("Blob_Logo") = logox
        r.Item("Blob_Logo2") = logox


        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)
            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            'Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap("C:\AgroSorgenti - Copia\AgronicaAudit\AgronicaGlobalGap\AB_Immagini\Logo\Logo_Agronica.bmp")
            If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then

                If Not objWebConfig.Path_Directory_Loghi_Cliente.EndsWith("\") Then
                    objWebConfig.Path_Directory_Loghi_Cliente &= "\"
                End If
                Dim inserito As Boolean = False
                ' metto l'immagine1
                If caricaLogo1 Then
                    Try
                        Dim path As String = objWebConfig.Path_Directory_Loghi_Cliente & "Logo_Orizzontale_Standard.bmp"
                        Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap(path)
                        Dim logo() As Byte
                        Dim c As New System.Drawing.ImageConverter
                        logo = c.ConvertTo(bmpF, GetType(Byte()))
                        Dim i As Integer = 0
                        'For i = 0 To DsLoghi.Loghi.Rows.Count - 1
                        'DsLoghi.Loghi.Rows(i).Item("Blob_Logo") = logo
                        r.Item("Blob_Logo") = logo
                        inserito = True
                        ' Next
                    Catch ex As Exception

                    End Try
                End If

                'If inserito Then
                DsLoghi.Loghi.Rows.Add(r)
                DsLoghi.Loghi.AcceptChanges()
                'End If

            End If

        Catch ex As Exception

        End Try

    End Sub

End Class