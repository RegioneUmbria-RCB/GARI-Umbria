Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility
Imports AgronicaCoreAnagrafeDAL
Imports System.Drawing
Imports System.IO

Public Class Scheda_OP_Tipo_1
    Inherits System.Web.UI.Page

    '----- Gestione Querystring
    Dim Piva As String
    Dim Anno As String
    Dim Rag_Soc As String
    Dim Tipo_Stampa As String
    Dim Tipo_Selezione As String
    Dim validita_inizio, validita_fine As String
    Dim fronteRetro As Boolean

    Private rptStampa As Rpt_Scheda_OP_Tipo_1

    'oggetto objparametri x server e utenti
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Quadro_P_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        rptStampa = New Rpt_Scheda_OP_Tipo_1

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strNomeCooperativa As String = ""

        'inizializzazione oggetti objParametri_Utenti e objParametri_Server
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Recupero i dati dalla QueryString 
        '#################################################################################

        Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
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

        Dim strFiltroImprese As String = If(Session("strParametri") IsNot Nothing, Session("strParametri").ToString(), "")
        Dim objStampe As New AgronicaCoreStampeDAL.Stampe_OP

        If Not Me.IsPostBack Then

            rptStampa.Section3.SectionFormat.EnableSuppress = True
            rptStampa.Section4.SectionFormat.EnableSuppress = True
            rptStampa.Section19.SectionFormat.EnableSuppress = True
            rptStampa.Section18.SectionFormat.EnableSuppress = True
            rptStampa.Section23.SectionFormat.EnableSuppress = True
            rptStampa.Section22.SectionFormat.EnableSuppress = True
            rptStampa.Section21.SectionFormat.EnableSuppress = True
            rptStampa.Section26.SectionFormat.EnableSuppress = True
            rptStampa.Section26b.SectionFormat.EnableSuppress = True
            rptStampa.Section27.SectionFormat.EnableSuppress = True
            rptStampa.Section29.SectionFormat.EnableSuppress = True
            rptStampa.Section30.SectionFormat.EnableSuppress = True
            rptStampa.Section31.SectionFormat.EnableSuppress = True
            rptStampa.Section32.SectionFormat.EnableSuppress = True
            rptStampa.Section33.SectionFormat.EnableSuppress = True
            rptStampa.Section34.SectionFormat.EnableSuppress = True
            rptStampa.Section35.SectionFormat.EnableSuppress = True
            rptStampa.Section36.SectionFormat.EnableSuppress = True
            rptStampa.Section37.SectionFormat.EnableSuppress = True
            rptStampa.Section1b.SectionFormat.EnableSuppress = True
            rptStampa.Section1c.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab1.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab2.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab3.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab4.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab5.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab6.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab7.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab8.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresab9.SectionFormat.EnableSuppress = True
            rptStampa.SectionImpresaPadre.SectionFormat.EnableSuppress = True
            rptStampa.SectionCooperativa.SectionFormat.EnableSuppress = True
            rptStampa.SectionSitiProduttivi.SectionFormat.EnableSuppress = True
            rptStampa.SectionSubFornitore.SectionFormat.EnableSuppress = True
            rptStampa.SectionTraOrogel.SectionFormat.EnableSuppress = True
            rptStampa.SectionTraApora.SectionFormat.EnableSuppress = True
            rptStampa.SectionSottoscrittoProduttore.SectionFormat.EnableSuppress = True

            rptStampa.Section1.ReportObjects("TextTitolo1").ObjectFormat.EnableSuppress = True
            rptStampa.Section5.ReportObjects("TextAllegato").ObjectFormat.EnableSuppress = True

            Select Case LCase(objParametri_Server.SuperUserUsername)
                Case "terremerse"
                    strNomeCooperativa = "TERREMERSE"
            End Select

            Select Case Tipo_Stampa

                Case enum_CodificaStampe.Adesione_Etico_Ambientale

                    Nome_Documento = "AdesioneEticoAmbientale"
                    CatCod = enum_CategorieDocumenti.AdesioneEticoAmbientale

                    CType(rptStampa.Section1.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "SCHEDA DI ADESIONE AI PROGRAMMI ETICI ED AMBIENTALI"
                    rptStampa.Section1.ReportObjects("TextTitolo1").ObjectFormat.EnableSuppress = False

                    rptStampa.Section3.SectionFormat.EnableSuppress = False

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)
                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("ModRefData", "")
                    rptStampa.SetParameterValue("TextTitolo", "")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                    CType(rptStampa.Section5.ReportObjects("TextAllegato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "ALL 02 PG 7.3 Rev. 3 del 14/11/2014"
                    rptStampa.Section5.ReportObjects("TextAllegato").ObjectFormat.EnableSuppress = False

                Case enum_CodificaStampe.Codice_Condotta

                    Nome_Documento = "CodiceCondotta"
                    CatCod = enum_CategorieDocumenti.CodiceCondotta

                    CType(rptStampa.Section1.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "SA 8000 CODICE DI CONDOTTA"
                    rptStampa.Section1.ReportObjects("TextTitolo1").ObjectFormat.EnableSuppress = False

                    rptStampa.Section4.SectionFormat.EnableSuppress = False

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)
                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("ModRefData", "")
                    rptStampa.SetParameterValue("TextTitolo", "")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                    CType(rptStampa.Section5.ReportObjects("TextAllegato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "ALL 04 PG 7.3 Rev. 5 del 14/11/2014"
                    rptStampa.Section5.ReportObjects("TextAllegato").ObjectFormat.EnableSuppress = False

                Case enum_CodificaStampe.Tenuta_Scheda_Campagna

                    Nome_Documento = "TenutaSchedaCampagna"
                    CatCod = enum_CategorieDocumenti.TenutaSchedaCampagna

                    CType(rptStampa.Section1.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "MODALITA' DI TENUTA SCHEDA DI CAMPAGNA"
                    rptStampa.Section1.ReportObjects("TextTitolo1").ObjectFormat.EnableSuppress = False

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)
                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("ModRefData", "")
                    rptStampa.SetParameterValue("TextTitolo", "")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                    rptStampa.Section19.SectionFormat.EnableSuppress = False

                Case enum_CodificaStampe.Adesione_DPI

                    Nome_Documento = "AdesioneDPI"
                    CatCod = enum_CategorieDocumenti.AdesioneDPI

                    CType(rptStampa.Section1.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "ADESIONE AI DPI"
                    rptStampa.Section1.ReportObjects("TextTitolo1").ObjectFormat.EnableSuppress = False

                    CType(rptStampa.Section5.ReportObjects("TextAllegato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "ALL. 03 PG 7.3 Rev. 3 del 14/11/2014"
                    rptStampa.Section5.ReportObjects("TextAllegato").ObjectFormat.EnableSuppress = False

                    'Dim Allineamento As String = "<style>" & _
                    '     " .litesto{ text-decoration:underline; } " & _
                    '    "</style>"
                    'Allineamento = "" '"<link href='../App_Styles/AgronicaStyle.css' rel='stylesheet' type='text/css' />"
                    'ol {
                    '    counter-reset: foo;
                    '    display: table;
                    '}

                    'li {
                    '    list-style: none;
                    '    counter-increment: foo;
                    '    display: table-row;
                    '}

                    'li::before {
                    '    content: counter(foo) ".";
                    '    display: table-cell;
                    '    text-align: right;
                    '    padding-right: .3em;
                    '}"

                    'Dim TestoAdesioneDPI As String = "<ul><li>adottare i disciplinari di produzione INTEGRATA e/o BIOLOGICA in vigore, previsti dal SERVIZIO DI ASSISTENZA TECNICA alle COLTIVAZIONI della cooperativa " & strNomeCooperativa & ".</li>" & _
                    '                                "<li>Mettere a disposizione della cooperativa " & strNomeCooperativa & " l’intera superficie delle colture interessate dal SERVIZIO di ASSISTENZA alle COLTIVAZIONI. La superficie delle colture interessate è riportata nell’allegato Catasto e Valorizzazioni aggiornato annualmente.</li>" & _
                    '                                "<li>Seguire i consigli e le indicazioni tecnico agronomiche proposte dal Tecnico/Consulente e o dal Coordinamento Provinciale di produzione integrata.</li>" & _
                    '                                "<li>Collaborare, all’esecuzione delle osservazioni e dei rilievi previsti dai disciplinari di produzioni integrata vigenti.</li>" & _
                    '                                "<li>Partecipare alle iniziative di divulgazione e formazione promosse dalla cooperativa " & strNomeCooperativa & ".</li>" & _
                    '                                "<li>Essere a conoscenza che il mancato rispetto delle regole comporta la sospensione dell’azienda dai programmi di Produzione Integrata e la revoca dei benefici derivanti dall’iniziativa di Valorizzazione Commerciale e/o dai benefici previsti dall’OCM.</li></ul>"

                    Dim TestoAdesioneDPI As String = strNomeCooperativa

                    'Dim TestoAdesioneDPI As String = Allineamento & "<ul><li style='float-align: right;'>adottare i disciplinari di produzione INTEGRATA e/o BIOLOGICA in vigore, previsti dal SERVIZIO DI ASSISTENZA TECNICA alle COLTIVAZIONI della cooperativa " & strNomeCooperativa & ".</li>" & _
                    '    "<li style='text-align: left;'>Mettere a disposizione della cooperativa " & strNomeCooperativa & " l’intera superficie delle colture interessate dal SERVIZIO di ASSISTENZA alle COLTIVAZIONI. La superficie delle colture interessate è riportata nell’allegato Catasto e Valorizzazioni aggiornato annualmente.</li>" & _
                    '    "<li style='align: left;'>Seguire i consigli e le indicazioni tecnico agronomiche proposte dal Tecnico/Consulente e o dal Coordinamento Provinciale di produzione integrata.</li>" & _
                    '    "<li>Collaborare, all’esecuzione delle osservazioni e dei rilievi previsti dai disciplinari di produzioni integrata vigenti.</li>" & _
                    '    "<li>Partecipare alle iniziative di divulgazione e formazione promosse dalla cooperativa " & strNomeCooperativa & ".</li>" & _
                    '    "<li style='text-align: right;'>Essere a conoscenza che il mancato rispetto delle regole comporta la sospensione dell’azienda dai programmi di Produzione Integrata e la revoca dei benefici derivanti dall’iniziativa di Valorizzazione Commerciale e/o dai benefici previsti dall’OCM.</li></ul>"

                    'Dim TestoAdesioneDPI As String = "-	adottare i disciplinari di produzione INTEGRATA e/o BIOLOGICA in vigore, previsti dal SERVIZIO DI ASSISTENZA TECNICA alle COLTIVAZIONI della cooperativa " & strNomeCooperativa & "." & vbCrLf & vbCrLf & _
                    '                                  "-	Mettere a disposizione della cooperativa " & strNomeCooperativa & " l’intera superficie delle colture interessate dal SERVIZIO di ASSISTENZA alle COLTIVAZIONI. La superficie delle colture interessate è riportata nell’allegato Catasto e Valorizzazioni aggiornato annualmente." & vbCrLf & vbCrLf & _
                    '                                  "-	Seguire i consigli e le indicazioni tecnico agronomiche proposte dal Tecnico/Consulente e o dal Coordinamento Provinciale di produzione integrata." & vbCrLf & vbCrLf & _
                    '                                  "-	Collaborare, all’esecuzione delle osservazioni e dei rilievi previsti dai disciplinari di produzioni integrata vigenti." & vbCrLf & vbCrLf & _
                    '                                  "-	Partecipare alle iniziative di divulgazione e formazione promosse dalla cooperativa " & strNomeCooperativa & "." & vbCrLf & vbCrLf & _
                    '                                  "-	Essere a conoscenza che il mancato rispetto delle regole comporta la sospensione dell’azienda dai programmi di Produzione Integrata e la revoca dei benefici derivanti dall’iniziativa di Valorizzazione Commerciale e/o dai benefici previsti dall’OCM." & vbCrLf & vbCrLf

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)
                    rptStampa.SetParameterValue("AdesioneDPI", TestoAdesioneDPI)
                    rptStampa.SetParameterValue("ModRefData", "")
                    rptStampa.SetParameterValue("TextTitolo", "")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                    rptStampa.Section18.SectionFormat.EnableSuppress = False
                    rptStampa.Section26.SectionFormat.EnableSuppress = False

                Case enum_CodificaStampe.Impegnativa_Eurep

                    Nome_Documento = "ImpegnativaGLOBAL"
                    CatCod = enum_CategorieDocumenti.ImpegnativaGLOBAL

                    CType(rptStampa.Section1.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "CONTRATTO PER CERTIFICAZIONE GLOBAL G.A.P."
                    rptStampa.Section1.ReportObjects("TextTitolo1").ObjectFormat.EnableSuppress = False

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)
                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("ModRefData", "")
                    rptStampa.SetParameterValue("TextTitolo", "")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                    Dim TestoGlobal As String = "1)	A produrre e consegnare il raccolto nei modi e nei termini stabiliti dalla Cooperativa " & strNomeCooperativa &
                                                      ", il prodotto ricavabile dall’Allegato Catasto e Valorizzazioni aggiornato annualmente."

                    CType(rptStampa.Section27.ReportObjects("TextGlobal"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TestoGlobal

                    rptStampa.Section27.SectionFormat.EnableSuppress = False

                    CType(rptStampa.Section5.ReportObjects("TextAllegato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "All. 05 Rev. 8 del 14/11/2014"
                    rptStampa.Section5.ReportObjects("TextAllegato").ObjectFormat.EnableSuppress = False

                Case enum_CodificaStampe.Impegnativa_QC

                    Nome_Documento = "ImpegnativaQC"
                    CatCod = enum_CategorieDocumenti.ImpegnativaQC

                    CType(rptStampa.Section1.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "CONTRATTO DI COLTIVAZIONE ORTOFRUTTA 'Q.C.' PER FILIERA COOP ITALIA"
                    rptStampa.Section1.ReportObjects("TextTitolo1").ObjectFormat.EnableSuppress = False

                    rptStampa.SectionImpresa.ReportObjects("TextPremesso").ObjectFormat.EnableSuppress = False

                    CType(rptStampa.Section5.ReportObjects("TextAllegato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "All. 11 Rev. 0 del 14/11/2014"
                    rptStampa.Section5.ReportObjects("TextAllegato").ObjectFormat.EnableSuppress = False

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)
                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("ModRefData", "")
                    rptStampa.SetParameterValue("TextTitolo", "")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                    'Dim TestoQC As String = "1)	A produrre e consegnare tutto il prodotto presente nell’Allegato Catasto e Valorizzazioni aggiornato annualmente, avente le caratteristiche organolettiche e qualitative conformi come da accordi di filliera Coop Italia, su indicazione della coop " & strNomeCooperativa & ";"

                    'CType(rptStampa.Section23.ReportObjects("TextImpegnativaQC"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = TestoQC

                    rptStampa.Section23.SectionFormat.EnableSuppress = False

                Case enum_CodificaStampe.Impegnativa_Confusione_Sessuale

                    Nome_Documento = "ImpegnativaConfusioneSessuale"
                    CatCod = enum_CategorieDocumenti.ImpegnativaConfusioneSessuale

                    CType(rptStampa.Section1.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "CONTRATTO DI COLTIVAZIONE ORTOFRUTTA PER L'APPLICAZIONE DEL METODO DELLA CONFUSIONE SESSUALE E/O DEL DISORIENTAMENTO"
                    rptStampa.Section1.ReportObjects("TextTitolo1").ObjectFormat.EnableSuppress = False

                    CType(rptStampa.Section22.ReportObjects("TextImpegnativaConSesCooperativa1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strNomeCooperativa

                    CType(rptStampa.Section22.ReportObjects("TextImpegnativaConSesAnno1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Anno
                    CType(rptStampa.Section22.ReportObjects("TextImpegnativaConSesAnno2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "31/12/" & Anno

                    Dim DSLoghi As New Ds_Loghi
                    CaricaDs_Loghi(DSLoghi, True, False)
                    rptStampa.SetDataSource(DSLoghi)
                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("ModRefData", "")
                    rptStampa.SetParameterValue("TextTitolo", "")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                    rptStampa.Section22.SectionFormat.EnableSuppress = False
                    rptStampa.Section29.SectionFormat.EnableSuppress = False

                Case enum_CodificaStampe.Allegato_CatastoeValorizzazioni

                    Nome_Documento = "AllegatoCatastoValorizzazioni"
                    CatCod = enum_CategorieDocumenti.AllegatoCatastoValorizzazioni

                    CType(rptStampa.Section1.ReportObjects("TextTitolo1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "ALLEGATO CATASTO E VALORIZZAZIONI" & vbCrLf & "Anno " & Anno
                    rptStampa.Section1.ReportObjects("TextTitolo1").ObjectFormat.EnableSuppress = False

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)


                    Dim DSImpianti As New DS_Scheda_OP_Tipo_1
                    CaricaDs_Impianti(DSImpianti)

                    'AggiungiSubtotale(DSImpianti)

                    rptStampa.OpenSubreport("Scheda_OP_Impianti.rpt").SetDataSource(DSImpianti)

                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("ModRefData", "")
                    rptStampa.SetParameterValue("TextTitolo", "")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                    rptStampa.Section21.SectionFormat.EnableSuppress = False

                    CType(rptStampa.Section5.ReportObjects("TextAllegato"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "All. 10 PG 7.3 Rev. 0 del 14/11/2014"
                    rptStampa.Section5.ReportObjects("TextAllegato").ObjectFormat.EnableSuppress = False

                Case enum_CodificaStampe.Adesione_ModuloGrasp

                    Nome_Documento = "AdesioneModuloGrasp"
                    CatCod = enum_CategorieDocumenti.AdesioneModuloGrasp

                    rptStampa.SectionImpresa.SectionFormat.EnableSuppress = True
                    rptStampa.Section1.SectionFormat.EnableSuppress = True
                    rptStampa.Section1b.SectionFormat.EnableSuppress = False
                    rptStampa.SectionTraOrogel.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab1.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab2.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab3.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab4.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab9.SectionFormat.EnableSuppress = False

                    Dim underlinedFont = New System.Drawing.Font("Arial", 10.2, FontStyle.Bold Or FontStyle.Underline)
                    CType(rptStampa.SectionTraOrogel.ReportObjects("Text70"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)
                    CType(rptStampa.SectionTraOrogel.ReportObjects("Text71"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)
                    CType(rptStampa.SectionTraOrogel.ReportObjects("Text72"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)
                    CType(rptStampa.SectionTraOrogel.ReportObjects("Text73"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)
                    rptStampa.SectionSitiProduttivi.SectionFormat.EnableSuppress = False
                    rptStampa.Section34.SectionFormat.EnableSuppress = False
                    rptStampa.Section26b.SectionFormat.EnableSuppress = False


                    Dim DSSitiProduttivi As New DS_Scheda_OP_Tipo_1
                    CaricaDs_SitiProduttivi(DSSitiProduttivi)

                    Dim subrpt = rptStampa.OpenSubreport("Scheda_OP_SitiProduttivi")
                    subrpt.SetDataSource(DSSitiProduttivi)
                    CType(subrpt.ReportDefinition.ReportObjects("TextSuperficie"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)

                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("TextTitolo", "<p align = 'center'>IMPEGNATIVA DI ADESIONE MODULO GRASP")
                    rptStampa.SetParameterValue("ModRefData", "<p align = 'center'><b>Mod.</b> IOF/DQI/08-1 Rev. 0 DATA: 16/05/2016")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                Case enum_CodificaStampe.Adesione_ProtocolloGlobalGAP

                    Nome_Documento = "AdesioneProtocolloGlobalGAP"
                    CatCod = enum_CategorieDocumenti.AdesioneProtocolloGlobalGAP

                    Dim Cod_GGN = ""
                    Dim Cod_Produttore = ""
                    Dim Cod_Socio = ""
                    Dim CUAA = ""

                    Dim ImpreseCodici_Read As New Imprese_Codici_Read

                    ImpreseCodici_Read.Leggi_CUAA_GGN_Produttore_Socio(Piva, CUAA, Cod_GGN, Cod_Produttore, Cod_Socio, objParametri_Server)

                    rptStampa.SectionImpresa.SectionFormat.EnableSuppress = True
                    rptStampa.Section1.SectionFormat.EnableSuppress = True
                    rptStampa.SectionTraOrogel.SectionFormat.EnableSuppress = False
                    rptStampa.Section1b.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab1.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab2.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab3.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab4.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab6.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab9.SectionFormat.EnableSuppress = False


                    'CType(rptStampa.SectionImpresab.ReportObjects("TextRappresentanteLegaleb"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Vet_Intestazione(1).ToUpper"
                    'CType(rptStampa.SectionImpresab.ReportObjects("TextAziendab"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Vet_Intestazione(10)"
                    'CType(rptStampa.SectionImpresab2.ReportObjects("TextIndirizzob"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "com_impresa + prov_impresa + cap_impresa + ind_impresa"
                    'CType(rptStampa.SectionImpresab3.ReportObjects("TextGGN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Cod_GGN

                    rptStampa.SectionSitiProduttivi.SectionFormat.EnableSuppress = False
                    rptStampa.Section30.SectionFormat.EnableSuppress = False
                    rptStampa.Section26b.SectionFormat.EnableSuppress = False

                    Dim DSSitiProduttivi As New DS_Scheda_OP_Tipo_1

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)
                    CaricaDs_SitiProduttivi(DSSitiProduttivi)

                    rptStampa.OpenSubreport("Scheda_OP_SitiProduttivi").SetDataSource(DSSitiProduttivi)

                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("TextTitolo", "<p align = 'center'>IMPEGNATIVA DI ADESIONE PROTOCOLLO GLOBALG.A.P. - GRASP")
                    rptStampa.SetParameterValue("ModRefData", "<p align = 'center'><b>Mod.</b> IOF/DQI/04-1 Rev.<u>4</u> DATA: <u>18/03/2024</u>")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                Case enum_CodificaStampe.Adesione_NurtureModule

                    Nome_Documento = "AdesioneNurtureModule"
                    CatCod = enum_CategorieDocumenti.AdesioneNurtureModule

                    Dim Cod_GGN = ""
                    Dim Cod_Produttore = ""
                    Dim Cod_Socio = ""

                    rptStampa.SectionImpresa.SectionFormat.EnableSuppress = True
                    rptStampa.Section1.SectionFormat.EnableSuppress = True
                    rptStampa.SectionTraOrogel.SectionFormat.EnableSuppress = False
                    rptStampa.Section1b.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab1.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab2.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab6.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab9.SectionFormat.EnableSuppress = False

                    Dim underlinedFont = New System.Drawing.Font("Arial", 10.2, FontStyle.Bold Or FontStyle.Underline)
                    CType(rptStampa.SectionTraOrogel.ReportObjects("Text70"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)
                    CType(rptStampa.SectionTraOrogel.ReportObjects("Text71"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)
                    CType(rptStampa.SectionTraOrogel.ReportObjects("Text72"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)
                    CType(rptStampa.SectionTraOrogel.ReportObjects("Text73"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)

                    rptStampa.SectionSitiProduttivi.SectionFormat.EnableSuppress = False
                    rptStampa.Section33.SectionFormat.EnableSuppress = False
                    rptStampa.Section26b.SectionFormat.EnableSuppress = False

                    Dim DSSitiProduttivi As New DS_Scheda_OP_Tipo_1
                    CaricaDs_SitiProduttivi(DSSitiProduttivi)

                    Dim subrpt = rptStampa.OpenSubreport("Scheda_OP_SitiProduttivi")
                    subrpt.SetDataSource(DSSitiProduttivi)
                    CType(subrpt.ReportDefinition.ReportObjects("TextSuperficie"), CrystalDecisions.CrystalReports.Engine.TextObject).ApplyFont(underlinedFont)

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)

                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("TextTitolo", "<p align = 'center'>IMPEGNATIVA DI ADESIONE NURTURE <u>Module</u>")
                    rptStampa.SetParameterValue("ModRefData", "<p align = 'center'><b>Mod.</b> IOF/DQI/04-2 Rev. <u>3</u> DATA: <u>10/05/2017</u>")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                Case enum_CodificaStampe.Adesione_Despar

                    Nome_Documento = "AdesioneDespar"
                    CatCod = enum_CategorieDocumenti.AdesioneDespar

                    Dim Cod_GGN = ""
                    Dim Cod_Produttore = ""
                    Dim Cod_Socio = ""

                    rptStampa.SectionImpresa.SectionFormat.EnableSuppress = True
                    rptStampa.Section1.SectionFormat.EnableSuppress = True
                    rptStampa.Section1b.SectionFormat.EnableSuppress = False
                    rptStampa.Section1c.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab2.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab5.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab8.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab9.SectionFormat.EnableSuppress = False
                    rptStampa.SectionSottoscrittoProduttore.SectionFormat.EnableSuppress = False

                    rptStampa.Section32.SectionFormat.EnableSuppress = False
                    rptStampa.Section26.SectionFormat.EnableSuppress = False

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)

                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("TextTitolo", "<p align = 'center'>IMPEGNATIVA DI ADESIONE")
                    rptStampa.SetParameterValue("ModRefData", "<p align = 'center'><b>Mod.</b> DATA: 02/03/2017")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                Case enum_CodificaStampe.Adesione_Conad

                    Nome_Documento = "AdesioneConad"
                    CatCod = enum_CategorieDocumenti.AdesioneConad

                    rptStampa.SectionImpresa.SectionFormat.EnableSuppress = True
                    rptStampa.Section1.SectionFormat.EnableSuppress = True
                    rptStampa.Section1b.SectionFormat.EnableSuppress = False
                    rptStampa.Section1c.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab2.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab5.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab8.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab9.SectionFormat.EnableSuppress = False
                    rptStampa.SectionSottoscrittoProduttore.SectionFormat.EnableSuppress = False

                    rptStampa.Section31.SectionFormat.EnableSuppress = False
                    rptStampa.Section26.SectionFormat.EnableSuppress = False

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)

                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("TextTitolo", "<p align = 'center'>IMPEGNATIVA DI ADESIONE")
                    rptStampa.SetParameterValue("ModRefData", "<p align = 'center'><b>Mod. Rev. 1</b> DATA: 24/04/2023 ")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                Case enum_CodificaStampe.Adesione_StandardLeaf

                    Nome_Documento = "AdesioneStandardLeaf"
                    CatCod = enum_CategorieDocumenti.AdesioneStandardLeaf

                    rptStampa.SectionImpresa.SectionFormat.EnableSuppress = True
                    rptStampa.Section1.SectionFormat.EnableSuppress = True
                    rptStampa.SectionTraOrogel.SectionFormat.EnableSuppress = False
                    rptStampa.Section1b.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab7.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab9.SectionFormat.EnableSuppress = False

                    rptStampa.SectionImpresab.ReportObjects("TextAziendab").ObjectFormat.EnableSuppress = True
                    rptStampa.SectionImpresab.ReportObjects("TextTimbro").ObjectFormat.EnableSuppress = False

                    rptStampa.Section35.SectionFormat.EnableSuppress = False
                    rptStampa.Section26b.SectionFormat.EnableSuppress = False
                    rptStampa.SectionSitiProduttivi.SectionFormat.EnableSuppress = False

                    Dim DSSitiProduttivi As New DS_Scheda_OP_Tipo_1
                    CaricaDs_SitiProduttivi(DSSitiProduttivi)

                    rptStampa.OpenSubreport("Scheda_OP_SitiProduttivi").SetDataSource(DSSitiProduttivi)

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)

                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("TextTitolo", "<p align = 'center'>IMPEGNATIVA DI ADESIONE <i>Standard LEAF</i>")
                    rptStampa.SetParameterValue("ModRefData", "<p align = 'center'><b>Mod.</b> IOF/DQI/09-1 Rev. <u>0</u> DATA: 22/05/2023")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

                Case enum_CodificaStampe.Accordo_Responsabilita_di_Filiera

                    Nome_Documento = "AccordoResponsabilitaFiliera"
                    CatCod = enum_CategorieDocumenti.AccordoResponsabilitaFiliera

                    rptStampa.SectionImpresa.SectionFormat.EnableSuppress = True
                    rptStampa.SectionImpresab.ReportObjects("TextReferenteAziendale").ObjectFormat.EnableSuppress = False


                    rptStampa.Section1.SectionFormat.EnableSuppress = True
                    rptStampa.SectionTraApora.SectionFormat.EnableSuppress = False
                    rptStampa.Section1b.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresaPadre.SectionFormat.EnableSuppress = False
                    rptStampa.Section36.SectionFormat.EnableSuppress = False
                    rptStampa.SectionSubFornitore.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab1.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab2.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab6.SectionFormat.EnableSuppress = False
                    rptStampa.SectionImpresab9.SectionFormat.EnableSuppress = False
                    rptStampa.Section37.SectionFormat.EnableSuppress = False

                    Dim DSLoghi As New Ds_Loghi
                    Dim DSImprese As New Ds_Imprese_StampaMassiva
                    CaricaDs_Loghi(DSLoghi, True, True)
                    objStampe.Carica_ImpresePerStampaMassiva(DSImprese, strFiltroImprese, Tipo_Selezione, objParametri_Server)
                    rptStampa.Database.Tables("Loghi").SetDataSource(DSLoghi)
                    rptStampa.Database.Tables("Imprese_StampaMassiva").SetDataSource(DSImprese)

                    rptStampa.SetParameterValue("AdesioneDPI", "")
                    rptStampa.SetParameterValue("TextTitolo", "<p align = 'center'>ACCORDO RESPONSABILITÀ DI FILIERA")
                    rptStampa.SetParameterValue("ModRefData", "<p align = 'center'>Mod. PP/DG/03-8 Rev.2 DATA: 29/06/2016")
                    ' imposto il parametro del fronte-retro
                    rptStampa.SetParameterValue("fronteRetro", fronteRetro)

            End Select

            Session("strParametri") = Nothing

            'CType(rptStampa.SectionImpresa.ReportObjects("TextPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Piva
            'CType(rptStampa.SectionImpresab1.ReportObjects("TextPivab"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Piva

            'CType(rptStampa.SectionImpresa.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Vet_Intestazione(10)"
            'CType(rptStampa.SectionImpresa.ReportObjects("TextRappresentanteLegale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
            'CType(rptStampa.SectionImpresa.ReportObjects("TextIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Vet_Intestazione(11)"
            'CType(rptStampa.SectionImpresa.ReportObjects("TextComune"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Vet_Intestazione(12)"
            ''If Vet_Intestazione(13) <> "0" And Vet_Intestazione(13) <> "00" Then
            ''CType(rptStampa.SectionImpresa.ReportObjects("TextProvincia"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Vet_Intestazione(13)"
            ''End If
            'CType(rptStampa.SectionImpresab6.ReportObjects("TextRecapitoTel"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Vet_Intestazione(28)"
            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(CatCod, "", "", objParametri_Server)
            objCatDoc = Nothing

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile

            For Each key As ParameterField In rptStampa.ParameterFields
                If key.HasCurrentValue = False Then
                    Debug.Print(key.Name.ToString)
                End If


            Next


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

                Log_Errori = Nome_Documento + ", Piva = " + CStr(Piva) + Log_Errori

                Nome_File = "LogErrori_" + Nome_Documento + "_p" & Piva + "_d" + Anno + CStr(Session("ASG_Utente_Username")) + ".txt"

                Dim objLog As New AgronicaCoreDataProvider.LogProvider
                Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
                If objAgroWeb.PathDirectoryLOG <> "" Then
                    objParametri_Server.LogDirectory = ""
                    Path_Errore = objAgroWeb.PathDirectoryLOG & Nome_Documento
                Else
                    Path_Errore = "C: \Agronica_LOG\Stampe_OP"
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

    Sub CaricaDs_Loghi(ByVal DsLoghi As Ds_Loghi, ByVal caricaLogo1 As Boolean, ByVal caricaLogo2 As Boolean)

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

                ' metto l'immagine2
                If caricaLogo2 Then
                    Try
                        Dim path As String = objWebConfig.Path_Directory_Loghi_Cliente & "Logo_Orizzontale_Standard2.bmp"
                        Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap(path)
                        Dim logo() As Byte
                        Dim c As New System.Drawing.ImageConverter
                        logo = c.ConvertTo(bmpF, GetType(Byte()))
                        Dim i As Integer = 0
                        'For i = 0 To DsLoghi.Loghi.Rows.Count - 1
                        'DsLoghi.Loghi.Rows(i).Item("Blob_Logo2") = logo
                        r.Item("Blob_Logo2") = logo
                        inserito = True
                        'Next
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

    'Public Sub AggiungiSubtotale(ByRef DSImpianti As DS_Scheda_OP_Tipo_1)
    '    Dim i As Integer
    '    Dim appoggio_sa As String = ""
    '    Dim appoggio_veg As String = ""
    '    Dim st As Double = 0

    '    Dim dtapp As DataTable = DSImpianti.DT_Scheda_OP_Tipo_1.Clone


    '    For i = 0 To DSImpianti.DT_Scheda_OP_Tipo_1.Rows.Count - 1
    '        If i = 0 Then
    '            st = DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i).Item("Sup_Imp")
    '            appoggio_sa = DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i).Item("sa_nome")
    '            appoggio_veg = DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i).Item("veg_des")
    '        Else
    '            If DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i).Item("sa_nome") <> appoggio_sa And _
    '                DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i).Item("veg_des") <> appoggio_veg Then
    '                'aggiungo 
    '                Dim dr As DataRow = dtapp.NewRow
    '                dr.Item("veg_des") = appoggio_veg
    '                dr.Item("sa_nome") = appoggio_sa
    '                dr.Item("Sup_Imp") = st
    '                dr.Item("totale") = 1
    '                dtapp.ImportRow(dr)

    '                'copio
    '                st = DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i).Item("Sup_Imp")
    '                appoggio_sa = DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i).Item("sa_nome")
    '                appoggio_veg = DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i).Item("veg_des")
    '            Else
    '                st = st + DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i).Item("Sup_Imp")
    '            End If
    '        End If

    '        dtapp.ImportRow(DSImpianti.DT_Scheda_OP_Tipo_1.Rows(i))
    '    Next
    '    DSImpianti.DT_Scheda_OP_Tipo_1 = dtapp

    'End Sub



    Public Sub CaricaDs_Impianti(ByRef DSImpianti As DS_Scheda_OP_Tipo_1)

        Dim strErr As String
        Dim sSql As New System.Text.StringBuilder

        Dim strFiltroImpianti As String = ""

        sSql.Length = 0
        sSql.AppendLine(" SELECT 0 as totale, CASE WHEN ISNULL(Imprese.partitaIvaReale, '') = '' THEN Reg_Impianti.PIVA ELSE Imprese.partitaIvaReale END PIVA, ")
        sSql.AppendLine(" Reg_Impianti.SA_COD, Reg_Impianti.APPEZZA, Reg_Impianti.ID_REG, Centri_Aziendali.sa_nome, Appezzamento.APP_NOME, ")
        sSql.AppendLine(" SpecieVegetali.Veg_Cod, Cultivar.Cul_Cod, SpecieVegetali.Veg_Des, Cultivar.Cul_Des, Reg_Impianti.Sup_Imp ")
        sSql.AppendLine(" , Reg_Impianti.validita_inizio ")
        sSql.AppendLine(" FROM Imprese ")
        sSql.AppendLine(" INNER JOIN Centri_Aziendali ON Imprese.PIVA = Centri_Aziendali.PIVA ")
        sSql.AppendLine(" INNER JOIN Appezzamento ON Centri_Aziendali.PIVA = Appezzamento.PIVA AND Centri_Aziendali.sa_cod = Appezzamento.SA_COD ")
        sSql.AppendLine(" INNER JOIN  Reg_Impianti ON Appezzamento.PIVA = Reg_Impianti.PIVA AND Appezzamento.SA_COD = Reg_Impianti.SA_COD AND ")
        sSql.AppendLine(" Appezzamento.APPEZZA = Reg_Impianti.APPEZZA ")
        sSql.AppendLine(" INNER JOIN Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod ")
        sSql.AppendLine(" INNER JOIN SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod ")
        sSql.AppendLine(" INNER JOIN UtentiXImprese ON Imprese.PIVA = UtentiXImprese.PIVA ")
        sSql.AppendLine(" WHERE UtentiXImprese.[USER] = '" + Agro_SQL_SaveText(CStr(Session("ASG_SuperUser_CodFiscale"))) + "' ")

        sSql.AppendLine(" AND Reg_Impianti.Validita_Inizio <= " & Agro_SQL_SaveDate(validita_fine))
        sSql.AppendLine(" AND Reg_Impianti.Validita_Fine >= " & Agro_SQL_SaveDate(validita_inizio))

        If Not Session("strParametri") Is Nothing Then
            strFiltroImpianti = Session("strParametri").ToString()
        End If
        sSql.AppendLine(strFiltroImpianti)


        'sSql.AppendLine(" ORDER BY sa_nome, Veg_Des, Cul_Des, app_nome ")
        '
        sSql.AppendLine(" ORDER BY sa_nome, Veg_Des, Cul_Des, Reg_Impianti.Validita_Inizio ")

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            objSQL.EseguiQuery_Lettura(objParametri_Server, sSql.ToString, "Scheda_OP_Tipo_1.CaricaDs_Impianti", DSImpianti, DSImpianti.DT_Scheda_OP_Tipo_1.TableName)
        Catch ex As Exception
            strErr = ex.Message
        End Try

        'Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        'objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        ''carico il dataset coi dati..
        'objSQL.SqlSelect(objParametri_Server.StringaConnessione, _
        '                    Session("ASG_Connessione_Server"), _
        '                    sSql.ToString, _
        '                    DSImpianti, _
        '                    DSImpianti.DataSetName, _
        '                    strErr)


        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else


        End If


    End Sub

    Public Sub CaricaDs_SitiProduttivi(ByRef DSSitiProduttivi As DS_Scheda_OP_Tipo_1)

        Dim strErr As String
        Dim sSql As New System.Text.StringBuilder
        sSql.Length = 0
        Dim strFiltroImpianti As String = ""

        sSql.AppendLine("Select sa_nome, com_des As Com_Des,ind_des as Ind_Des,sum(Sup_Imp) As Sup_Imp,  ")
        sSql.AppendLine("CASE WHEN ISNULL(Impresa.partitaIvaReale, '') = '' THEN Reg_Impianti.Piva ELSE Impresa.partitaIvaReale END AS Piva, ")
        sSql.AppendLine("Reg_Impianti.Sa_Cod, Veg_Des  ")
        sSql.AppendLine("From Reg_Impianti WITH(NOLOCK)   ")
        sSql.AppendLine("INNER Join Imprese Impresa WITH(NOLOCK) On Reg_Impianti.Piva = Impresa.Piva  ")
        sSql.AppendLine("INNER Join Cultivar c WITH(NOLOCK) On c.Cul_Cod = Reg_Impianti.CUL_COD  ")
        sSql.AppendLine("INNER Join SpecieVegetali s WITH(NOLOCK) On c.Veg_Cod = s.Veg_Cod  ")
        sSql.AppendLine("inner Join imprese_progetti ip WITH(NOLOCK) on Reg_Impianti.piva=ip.piva And Reg_Impianti.SA_COD=ip.sa_cod And Reg_Impianti.APPEZZA=ip.Appezza And Reg_Impianti.id_reg=ip.id_reg  ")
        sSql.AppendLine("inner Join Centri_Aziendali ca WITH(NOLOCK) on Reg_Impianti.piva=ca.piva  And Reg_Impianti.sa_cod=ca.sa_cod  ")
        sSql.AppendLine("inner Join CentrixIndirizzi ci WITH(NOLOCK) on ci.piva=ca.piva And ci.sa_cod=ca.sa_cod And tipo_indirizzo=1  ")
        sSql.AppendLine("inner Join Indirizzi i WITH(NOLOCK) on ci.cod_indirizzo=i.cod_indirizzo   ")
        sSql.AppendLine("WHERE ip.Validita_inizio < " & Agro_SQL_SaveDate(New Date(CInt(Anno), 12, 31)) & " ")
        sSql.AppendLine("And ip.Validita_Fine > " & Agro_SQL_SaveDate(New Date(CInt(Anno), 1, 1)) & " ")
        If Not Session("strParametri") Is Nothing Then
            strFiltroImpianti = Session("strParametri").ToString()
            sSql.AppendLine(strFiltroImpianti)
        Else
            sSql.AppendLine("And Reg_Impianti.piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If
        sSql.AppendLine("Group BY sa_nome, com_des, ind_des, s.veg_cod, s.veg_des, Reg_Impianti.Piva, Reg_Impianti.Sa_Cod, Impresa.partitaIvaReale ")


        Dim objSQL As New AgronicaCoreDataProvider.DataProvider

        Try
            objSQL.EseguiQuery_Lettura(objParametri_Server, sSql.ToString, "Scheda_OP_Tipo_1.CaricaDs_SitiProduttivi", DSSitiProduttivi, DSSitiProduttivi.DT_Scheda_OP_Tipo_1.TableName)
        Catch ex As Exception
            strErr = ex.Message
        End Try

        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else


        End If

    End Sub

End Class