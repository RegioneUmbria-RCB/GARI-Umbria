Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreDataProvider.UtilityProvider_2010
Imports AgronicaCoreUtility


''' <summary>
'''     Struttura utilizzata come enumerico di stringhe (Elenco di costanti di stringhe raggruppati)
''' </summary>
''' <remarks></remarks>
Public Structure ElencoReportChiave
    Const FRONTESPIZIO As String = "a"
    Const FERTILIZZAZIONI As String = "b"
    Const TRATTAMENTI As String = "c"
    Const FASI_FENOLOGICHE As String = "d"
    Const TRAPPOLE_INSTALLATE As String = "e"
    Const RIL_AVVERS_NELLE_TRAPP As String = "f"
    Const RIL_AVVERS_IN_CAMPO As String = "g"
    Const IRRIGAZIONE As String = "h"
    Const ALTRE_OPER_COLTURALI As String = "i"
    Const INDICE_MAT_E_RACCOLTA As String = "l"
    Const PIOGGE As String = "m"
    Const RIL_PROD_E_DATA_RACC As String = "n"
    Const DATI_CATASTALI As String = "o"
    Const SEMINE As String = "p"
    Const INFORMAZ_E_DICHIAR As String = "q"
    Const VISITE_ISPETTIVE As String = "r"
    Const PERSONALE_CON_PATENTINO As String = "s"
    Const FITOFARMACI As String = "t"
    Const PRATICHE_ECOLOGICHE As String = "u"
    Const VERIFICHE_CONFORMITA As String = "v"
    Const FORMAZIONE As String = "w"
    Const GESTIONE_RIFIUTI As String = "x"
    Const MANUTEN_MACCHINARI As String = "y"
    Const MACCH_SOLO_DIFESA As String = "z"
    Const TRATTAMENTI_POSTRACCOLTA As String = "1"
End Structure


Public Class Selezione_SchedaCampagna
    Inherits System.Web.UI.Page

    Dim Log_Errori As String = ""
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri


#Region "SEZIONI"


    '  Galassi, 12/12/2016 09:49:29: Causa oggetti ASP "molto simpatici", bisogna per forza abilitare tutti i check che si vorrà poi
    '   abilitare anche in un secondo momento lato JS in stampa perchè altrimenti il .NET prende in considerazione la parte asp e non quello
    '   che è stato modificato lato JS e non valuta le modifiche avvenute in pagina. Quindi pre-abilitiamo.

    'Dim ElencoReport_SchedaCampagna As String() = {} 'DISATTIVATA

    Dim ElencoReport_SchedaCampagna_Semplificata As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                               ElencoReportChiave.DATI_CATASTALI,
                                                               ElencoReportChiave.SEMINE,
                                                               ElencoReportChiave.FERTILIZZAZIONI,
                                                               ElencoReportChiave.TRATTAMENTI,
                                                               ElencoReportChiave.FITOFARMACI,
                                                               ElencoReportChiave.FASI_FENOLOGICHE,
                                                               ElencoReportChiave.TRAPPOLE_INSTALLATE,
                                                               ElencoReportChiave.RIL_AVVERS_NELLE_TRAPP,
                                                               ElencoReportChiave.RIL_AVVERS_IN_CAMPO,
                                                               ElencoReportChiave.IRRIGAZIONE,
                                                               ElencoReportChiave.ALTRE_OPER_COLTURALI,
                                                               ElencoReportChiave.INDICE_MAT_E_RACCOLTA,
                                                               ElencoReportChiave.RIL_PROD_E_DATA_RACC,
                                                               ElencoReportChiave.PIOGGE,
                                                               ElencoReportChiave.MANUTEN_MACCHINARI,
                                                               ElencoReportChiave.VERIFICHE_CONFORMITA}
    'ElencoReportChiave.PERSONALE_CON_PATENTINO,
    'ElencoReportChiave.PRATICHE_ECOLOGICHE,

    'Dim ElencoReport_Eurep_Gap As String() = {} 'DISATTIVATA

    'Dim ElencoReport_SchedaCampagna_ConserveItalia As String() = {} ' USA LA CAMPAGNA SEMPLIFICATA

    'ANCHE PIZZOLI
    Dim ElencoReport_Eurep_Gap_Semplificata As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                          ElencoReportChiave.DATI_CATASTALI,
                                                          ElencoReportChiave.SEMINE,
                                                          ElencoReportChiave.FERTILIZZAZIONI,
                                                          ElencoReportChiave.TRATTAMENTI,
                                                          ElencoReportChiave.FITOFARMACI,
                                                          ElencoReportChiave.FASI_FENOLOGICHE,
                                                          ElencoReportChiave.TRAPPOLE_INSTALLATE,
                                                          ElencoReportChiave.RIL_AVVERS_NELLE_TRAPP,
                                                          ElencoReportChiave.RIL_AVVERS_IN_CAMPO,
                                                          ElencoReportChiave.IRRIGAZIONE,
                                                          ElencoReportChiave.ALTRE_OPER_COLTURALI,
                                                          ElencoReportChiave.INDICE_MAT_E_RACCOLTA,
                                                          ElencoReportChiave.RIL_PROD_E_DATA_RACC,
                                                          ElencoReportChiave.PIOGGE,
                                                          ElencoReportChiave.PERSONALE_CON_PATENTINO,
                                                          ElencoReportChiave.MANUTEN_MACCHINARI,
                                                          ElencoReportChiave.VISITE_ISPETTIVE,
                                                          ElencoReportChiave.VERIFICHE_CONFORMITA}
    'ElencoReportChiave.PRATICHE_ECOLOGICHE,

    Dim ElencoReport_SchedaCampagnaMulti_Lombardia As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                                  ElencoReportChiave.DATI_CATASTALI,
                                                                  ElencoReportChiave.TRATTAMENTI,
                                                                  ElencoReportChiave.FITOFARMACI,
                                                                  ElencoReportChiave.FERTILIZZAZIONI,
                                                                  ElencoReportChiave.INFORMAZ_E_DICHIAR,
                                                                  ElencoReportChiave.IRRIGAZIONE,
                                                                  ElencoReportChiave.ALTRE_OPER_COLTURALI,
                                                                  ElencoReportChiave.PIOGGE,
                                                                  ElencoReportChiave.MANUTEN_MACCHINARI}

    'se si attivano nuovi report, bisogna ricordarsi l'aggancio del dataset sul sottoreport
    'sulla funzione TipoReportSchedaCampagnaMulti_QuadernoCampagnaLombardiaVeneto
    'e adeguare TipoReportSchedaCampagnaMulti_RegistroTrattamenti e TipoReportSchedaCampagnaMulti_RegistroFertilizzazioni
    'x sopprimere la sezione
    '8altrimenti viene generato errore "accesso ai parametri non corretto!!!)
    Dim ElencoReport_RegistroUnicoAziendale As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                           ElencoReportChiave.DATI_CATASTALI,
                                                           ElencoReportChiave.SEMINE,
                                                           ElencoReportChiave.TRATTAMENTI,
                                                           ElencoReportChiave.FITOFARMACI,
                                                           ElencoReportChiave.FERTILIZZAZIONI,
                                                           ElencoReportChiave.INFORMAZ_E_DICHIAR,
                                                           ElencoReportChiave.IRRIGAZIONE,
                                                           ElencoReportChiave.ALTRE_OPER_COLTURALI,
                                                           ElencoReportChiave.PIOGGE,
                                                           ElencoReportChiave.RIL_PROD_E_DATA_RACC,
                                                           ElencoReportChiave.MANUTEN_MACCHINARI,
                                                           ElencoReportChiave.PERSONALE_CON_PATENTINO,
                                                           ElencoReportChiave.FASI_FENOLOGICHE,
                                                           ElencoReportChiave.TRAPPOLE_INSTALLATE,
                                                           ElencoReportChiave.RIL_AVVERS_NELLE_TRAPP,
                                                           ElencoReportChiave.RIL_AVVERS_IN_CAMPO,
                                                           ElencoReportChiave.INDICE_MAT_E_RACCOLTA,
                                                           ElencoReportChiave.VISITE_ISPETTIVE,
                                                           ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA}

    Dim ElencoReport_RegistroTrattamenti As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                        ElencoReportChiave.DATI_CATASTALI,
                                                        ElencoReportChiave.PERSONALE_CON_PATENTINO,
                                                        ElencoReportChiave.TRATTAMENTI,
                                                        ElencoReportChiave.FITOFARMACI,
                                                        ElencoReportChiave.INFORMAZ_E_DICHIAR}

    Dim ElencoReport_RegistroTrattamenti_VenetoUnico As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                     ElencoReportChiave.PERSONALE_CON_PATENTINO,
                                                     ElencoReportChiave.MANUTEN_MACCHINARI,
                                                    ElencoReportChiave.TRATTAMENTI,
                                                    ElencoReportChiave.FITOFARMACI,
                                                     ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA}

    Dim ElencoReport_RegistroFertilizzazioni As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                            ElencoReportChiave.DATI_CATASTALI,
                                                            ElencoReportChiave.FERTILIZZAZIONI,
                                                            ElencoReportChiave.INFORMAZ_E_DICHIAR}

    'PER IL VENETO VIENE UTILIZZATO UN FILTRO DIVERSO
    'Dim ElencoReport_RegistroTrattamenti_Veneto As String() = {}

    Dim ElencoReport_SchedaCampagna_Multicentro As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                               ElencoReportChiave.DATI_CATASTALI,
                                                               ElencoReportChiave.PERSONALE_CON_PATENTINO,
                                                               ElencoReportChiave.SEMINE,
                                                               ElencoReportChiave.FERTILIZZAZIONI,
                                                               ElencoReportChiave.TRATTAMENTI,
                                                               ElencoReportChiave.FITOFARMACI,
                                                               ElencoReportChiave.FASI_FENOLOGICHE,
                                                               ElencoReportChiave.TRAPPOLE_INSTALLATE,
                                                               ElencoReportChiave.RIL_AVVERS_NELLE_TRAPP,
                                                               ElencoReportChiave.RIL_AVVERS_IN_CAMPO,
                                                               ElencoReportChiave.IRRIGAZIONE,
                                                               ElencoReportChiave.ALTRE_OPER_COLTURALI,
                                                               ElencoReportChiave.INDICE_MAT_E_RACCOLTA,
                                                               ElencoReportChiave.RIL_PROD_E_DATA_RACC,
                                                               ElencoReportChiave.PIOGGE,
                                                               ElencoReportChiave.MANUTEN_MACCHINARI,
                                                               ElencoReportChiave.INFORMAZ_E_DICHIAR,
                                                               ElencoReportChiave.VERIFICHE_CONFORMITA,
                                                               ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA,
                                                               ElencoReportChiave.VISITE_ISPETTIVE}

    Dim ElencoReport_SchedaInterventiAgronomici As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                           ElencoReportChiave.FERTILIZZAZIONI,
                                                           ElencoReportChiave.TRATTAMENTI,
                                                           ElencoReportChiave.RIL_AVVERS_NELLE_TRAPP,
                                                           ElencoReportChiave.RIL_AVVERS_IN_CAMPO,
                                                           ElencoReportChiave.IRRIGAZIONE,
                                                           ElencoReportChiave.ALTRE_OPER_COLTURALI,
                                                           ElencoReportChiave.RIL_PROD_E_DATA_RACC,
                                                           ElencoReportChiave.PIOGGE,
                                                           ElencoReportChiave.VISITE_ISPETTIVE}

    'ElencoReportChiave.SEMINE,
    'ElencoReportChiave.TRAPPOLE_INSTALLATE,
    '    ElencoReportChiave.INDICE_MAT_E_RACCOLTA,


    Dim ElencoReport_Eurep_Gap_Multicentro As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                          ElencoReportChiave.DATI_CATASTALI,
                                                          ElencoReportChiave.SEMINE,
                                                          ElencoReportChiave.FERTILIZZAZIONI,
                                                          ElencoReportChiave.TRATTAMENTI,
                                                          ElencoReportChiave.FITOFARMACI,
                                                          ElencoReportChiave.FASI_FENOLOGICHE,
                                                          ElencoReportChiave.TRAPPOLE_INSTALLATE,
                                                          ElencoReportChiave.RIL_AVVERS_NELLE_TRAPP,
                                                          ElencoReportChiave.RIL_AVVERS_IN_CAMPO,
                                                          ElencoReportChiave.IRRIGAZIONE,
                                                          ElencoReportChiave.ALTRE_OPER_COLTURALI,
                                                          ElencoReportChiave.INDICE_MAT_E_RACCOLTA,
                                                          ElencoReportChiave.RIL_PROD_E_DATA_RACC,
                                                          ElencoReportChiave.PIOGGE,
                                                          ElencoReportChiave.PERSONALE_CON_PATENTINO,
                                                          ElencoReportChiave.MANUTEN_MACCHINARI,
                                                          ElencoReportChiave.VISITE_ISPETTIVE,
                                                          ElencoReportChiave.INFORMAZ_E_DICHIAR,
                                                          ElencoReportChiave.VERIFICHE_CONFORMITA}
    'ElencoReportChiave.PRATICHE_ECOLOGICHE,

    Dim ElencoReport_Prov_Aut_Trento As String() = {ElencoReportChiave.FRONTESPIZIO,
                                                    ElencoReportChiave.DATI_CATASTALI,
                                                    ElencoReportChiave.SEMINE,
                                                    ElencoReportChiave.FERTILIZZAZIONI,
                                                    ElencoReportChiave.TRATTAMENTI,
                                                    ElencoReportChiave.FITOFARMACI,
                                                    ElencoReportChiave.FASI_FENOLOGICHE,
                                                    ElencoReportChiave.TRAPPOLE_INSTALLATE,
                                                    ElencoReportChiave.RIL_AVVERS_NELLE_TRAPP,
                                                    ElencoReportChiave.RIL_AVVERS_IN_CAMPO,
                                                    ElencoReportChiave.IRRIGAZIONE,
                                                    ElencoReportChiave.ALTRE_OPER_COLTURALI,
                                                    ElencoReportChiave.INDICE_MAT_E_RACCOLTA,
                                                    ElencoReportChiave.RIL_PROD_E_DATA_RACC,
                                                    ElencoReportChiave.PIOGGE,
                                                    ElencoReportChiave.PERSONALE_CON_PATENTINO,
                                                    ElencoReportChiave.PRATICHE_ECOLOGICHE,
                                                    ElencoReportChiave.MANUTEN_MACCHINARI,
                                                    ElencoReportChiave.VISITE_ISPETTIVE,
                                                    ElencoReportChiave.INFORMAZ_E_DICHIAR,
                                                    ElencoReportChiave.VERIFICHE_CONFORMITA,
                                                    ElencoReportChiave.FORMAZIONE,
                                                    ElencoReportChiave.GESTIONE_RIFIUTI}
#End Region

    '######################################################################################################
    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        AddHandler CType(Page.Master.FindControl("ImgBtn_AnnullaTutto"), ImageButton).Click, AddressOf Me.AnnullaTutto

    End Sub

    '######################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Tolgo la pagina dalla cache
        Response.Expires = 0

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean

        'Attivazione forzata provvisoria
        UtenteAbilitato = True


        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.
        If UtenteAbilitato = False Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If



        ' ----------------------------------------------
        ' Blocco dove vengono aggiunti gli elementi CSS non disponibili in asp al codice
        ' NB: vanno aggiunti solo quelli che poi non devono essere modificati in pageload o successivamente
        ' ----------------------------------------------
        If Not IsNothing(Me.CBL_Sezioni.Items.FindByValue("t")) Then
            'Trattamenti --> Fitofarmaci
            CBL_Sezioni.Items.FindByValue("t").Attributes.Add("id", "checkFito")
            CBL_Sezioni.Items.FindByValue("t").Attributes.Add("style", "margin-left:10px;")
        End If
        If Not IsNothing(Me.CBL_Sezioni.Items.FindByValue("c")) Then
            CBL_Sezioni.Items.FindByValue("c").Attributes.Add("id", "checkTratt")
            CBL_Sezioni.Items.FindByValue("c").Attributes.Add("onclick", "SelezionaDeselezionaFito();")
        End If

        'Macchine --> Difesa
        'FUTURO PER STAMPARE TUTTE LE MACCHINE
        'CBL_Sezioni.Items.FindByValue("z").Attributes.Add("id", "checkSoloMacchDif")
        'CBL_Sezioni.Items.FindByValue("z").Attributes.Add("style", "margin-left:10px;")
        'CBL_Sezioni.Items.FindByValue("y").Attributes.Add("id", "checkMacch")
        'CBL_Sezioni.Items.FindByValue("y").Attributes.Add("onclick", "SelezionaDeselezionaSoloDifesa();")

        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not Page.IsPostBack Then

        Else
            'output.Write("Postback has occured")
            Exit Sub
        End If


        '##############################################################
        '#####  Recupero Utente, piva e veg_cod  
        '##############################################################

        Dim Qs_Piva As String = ""
        Dim Qs_Veg_Cod As String = "0"
        Dim Qs_Sa_Cod As String = "0"
        Dim Qs_Sezione As String = ""

        Dim Matrice_Variabili(0, 0) As String

        'Dim ListaImpianti() As 

        Dim strXmlVariabilistampe As String
        Dim i As Integer
        Dim SaCod_Riferimento As Integer = 0
        Dim SaCod_Attuale As Integer = 0
        Dim RigheMatrice As Integer = -1 '-1 così si fa l'incremento anche per la prima riga
        Dim Log As String = ""

        Dim Report As Integer
        Dim UtilFiltr As Integer

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_ParametriStampa As System.Xml.XmlElement
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim CodiceTerreno As Integer
        Dim strCodiciTerreno As String
        Dim strCodiceTerreno As String

        Dim Flag_ErbOrt As Boolean = False
        Dim clc = New AgronicaCoreUtility.CaricaListControl
        Dim objVerificaDisciplinare2010 As New AgronicaCoreGestioneRichieste.ParametriVerificaDisciplinare2010
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

        Try

            strXmlVariabilistampe = Session("strXmlVariabilistampe")

            'Carico la stringa xml in un nuovo documento
            XmlDoc = New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXmlVariabilistampe)

            Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim GruppoVeg As New AgronicaCoreMetaSchemaDAL.GruppoVegetale_R

            If XmlDoc.HasChildNodes Then

                XML_ParametriStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

                'Ricavo i parametri che servono

                Session("ASG_Utente_Username") = XML_ParametriStampa.GetAttribute("username")

                Report = XML_ParametriStampa.GetAttribute("report")

                '07/01/2019: delibera di dismettere le stampe semplificate facendole puntare sempre alle multicentro
                Report = AgronicaCoreStampeDAL.Stampe_QDC.ReplaceTipoReport(Report)

                txtReport.Value = Report        ' hidden usato per ImpostaChk()

                ViewState("Report") = Report

                Dim objreport As New AgronicaCoreMetaSchemaDAL.StampeReport
                Master.LblTitolo.Text = objreport.LeggiDescrizione(Report, "", objParametri_Server)

                '  Galassi, 16/09/2016 10:26:04: Prelevo UtilizzaImpiantiFiltrati dal tag FiltroStampa per capire se arrivo dal filtrone
                ' e non devo selezionare tutti gli impianti di quella specie :D
                XML_FiltroStampa = XML_ParametriStampa.GetElementsByTagName("FiltraImpianti")(0)
                If Not IsNothing(XML_FiltroStampa) Then
                    If (Not XML_FiltroStampa.GetAttribute("UtilizzaImpiantiFiltrati") Is Nothing) Then
                        UtilFiltr = XML_FiltroStampa.GetAttribute("UtilizzaImpiantiFiltrati")
                    Else
                        UtilFiltr = 0
                    End If
                Else
                    UtilFiltr = 0
                End If


                ViewState("UtilizzaImpiantiFiltrati") = UtilFiltr
                XMLs_VariabiliStampe = XML_ParametriStampa.GetElementsByTagName("VariabiliStampe")

                'Modifica del 16/12/2009: poichè nel filtrone di lan e online 
                'non c'è il controllo che sia selezionato un solo centro,
                'per non mandare in errore l'assegna colture precedenti,
                'viene salvata la matrice Matrice_Variabili solo con gli impianti
                'del primo centro trovato

                'ReDim Matrice_Variabili(XMLs_VariabiliStampe.Count - 1, 5)

                'Poichè il ReDim Preserve modifica solo la dimensione a destra
                'devo sapere prima qual è il numero di impianti per dimensionare la matrice
                'faccio quindi 2 volte il ciclo su XMLs_VariabiliStampe

                For i = 0 To XMLs_VariabiliStampe.Count - 1
                    RigheMatrice += 1
                Next


                '-------------------------------------------------------------------
                '----- Logo Regionale

                ' Per default viene stampato il logo della regione
                Chk_LogoRegione.Checked = True
                ' Combo delle regioni per la gestione del logo
                AgronicaCoreUtility.CaricaListControl.Regioni(Me.Cmb_Regioni,
                                                                True, " ", "",
                                                                "",
                                                                "", "",
                                                                objParametri_Server)

                'per questo report di default imposto tutte le sezioni da stampare anche vuote
                If Report = enum_CodificaStampe.SchedaInterventiAgronomici Then
                    ViewState("SezioniVuote") = "a|b|c|f|g|h|i|m|n|r"
                    Me.Chk_IntestazioneOrgref.Visible = True
                Else
                    Me.Chk_IntestazioneOrgref.Visible = False
                End If

                Select Case Report

                    Case enum_CodificaStampe.Registro_Trattamenti_Massivo,
                        enum_CodificaStampe.Registro_Fertilizzazioni_Massivo

                        Riepilogo_Aziende.Visible = True
                        Riepilogo_Azienda.Visible = False

                        ReDim Matrice_Variabili(RigheMatrice, 0)

                        're-inizializzo
                        RigheMatrice = -1

                        Dim Dt As New DataTable
                        Dim Dr As DataRow

                        Dt.Columns.Add(New DataColumn("piva", GetType(String)))
                        Dt.Columns.Add(New DataColumn("rag_soc", GetType(String)))

                        Dim NomiChiavi(0) As String
                        NomiChiavi(0) = "piva"

                        'lettura aziende da stampare
                        Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                        Dim dtc As DataTable = cf.Leggi(0, "Stampe_Massive_Filtro", " valore = 'false' ", "", HttpContext.Current.Session("ASG_objParametri_Server"))


                        If dtc.Rows.Count = 1 Then
                            'DA TABELLA
                            Dim DtSm As New DataTable
                            Dim objSM As New AgronicaCoreStampeDAL.Stampe_Massive_R
                            DtSm = objSM.Leggi("", Report, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
                            For i = 0 To DtSm.Rows.Count - 1
                                Dr = Dt.NewRow
                                Dr.Item("rag_soc") = DtSm.Rows(i).Item("rag_soc")
                                Dr.Item("piva") = DtSm.Rows(i).Item("piva")
                                Dt.Rows.Add(Dr)
                            Next

                        Else
                            'PASSATI DA FILTRONE

                            For i = 0 To XMLs_VariabiliStampe.Count - 1

                                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                                RigheMatrice += 1

                                Matrice_Variabili(RigheMatrice, 0) = XML_VariabiliStampe.GetAttribute("piva")

                                Dr = Dt.NewRow
                                Dr.Item("rag_soc") = ""
                                Dr.Item("piva") = Matrice_Variabili(RigheMatrice, 0)
                                Dt.Rows.Add(Dr)

                            Next

                        End If

                        Me.DataGrid_Imprese.DataSource = Dt
                        Me.DataGrid_Imprese.DataKeyNames = NomiChiavi
                        Me.DataGrid_Imprese.DataBind()

                        'Qs_Piva = Matrice_Variabili(0, 0)
                        'Session("Piva") = Qs_Piva

                        'fine stampe massive
                        '-----------------------------------

                    Case Else

                        Riepilogo_Aziende.Visible = False
                        Riepilogo_Azienda.Visible = True

                        ReDim Matrice_Variabili(RigheMatrice, 5)

                        're-inizializzo
                        RigheMatrice = -1

                        Dim HashSaCod As New Hashtable
                        Dim HashVegCod As New Hashtable
                        Dim HashDestUso As New Hashtable

                        For i = 0 To XMLs_VariabiliStampe.Count - 1

                            XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                            'Modifica del 16/12/2009: poichè nel filtrone di lan e online 
                            'non c'è il controllo che sia selezionato un solo centro,
                            'per non mandare in errore l'assegna colture precedenti,
                            'viene salvata la matrice Matrice_Variabili solo con gli impianti
                            'del primo centro trovato

                            SaCod_Attuale = XML_VariabiliStampe.GetAttribute("sa_cod")

                            If SaCod_Riferimento = 0 Then
                                'è il primo giro,
                                'imposto il sa_cod di riferimento con il primo che trovo
                                SaCod_Riferimento = SaCod_Attuale
                            End If

                            RigheMatrice += 1


                            Matrice_Variabili(RigheMatrice, 0) = XML_VariabiliStampe.GetAttribute("piva")
                            Matrice_Variabili(RigheMatrice, 1) = XML_VariabiliStampe.GetAttribute("sa_cod")
                            Matrice_Variabili(RigheMatrice, 2) = XML_VariabiliStampe.GetAttribute("appezza")
                            Matrice_Variabili(RigheMatrice, 3) = XML_VariabiliStampe.GetAttribute("id_reg")
                            Matrice_Variabili(RigheMatrice, 4) = XML_VariabiliStampe.GetAttribute("veg_cod")

                            ' @Paolo
                            ' Inserimento variabile Id_Agenda per la stampa dei movimenti
                            Matrice_Variabili(RigheMatrice, 5) = XML_VariabiliStampe.GetAttribute("id_agenda")


                            '(03/09/2016) fede aggiunte combo
                            If CInt(Matrice_Variabili(RigheMatrice, 1)) <> 0 Then
                                If Not HashSaCod.ContainsKey(CInt(Matrice_Variabili(RigheMatrice, 1))) Then
                                    HashSaCod.Add(CInt(Matrice_Variabili(RigheMatrice, 1)), "")
                                End If
                            End If
                            If CInt(Matrice_Variabili(RigheMatrice, 4)) <> 0 Then
                                If Not HashVegCod.ContainsKey(CInt(Matrice_Variabili(RigheMatrice, 4))) Then
                                    HashVegCod.Add(CInt(Matrice_Variabili(RigheMatrice, 4)), "")
                                End If
                            End If



                            'Nel caso multispecie il flag=True significa che almeno uno degli impianti selezionati
                            'è di erbacee o orticole
                            If Not Flag_ErbOrt AndAlso (CInt(GruppoVeg.GruCod_from_VegCod(Matrice_Variabili(RigheMatrice, 4), objParametri_Server))) <> 1 Then
                                Flag_ErbOrt = True
                            End If

                            Dim objRegImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

                            CodiceTerreno = objRegImpianti.Leggi_DestinazioneUso_Impianto(
                                                                    CStr(Matrice_Variabili(RigheMatrice, 0)),
                                                                    CInt(Matrice_Variabili(RigheMatrice, 1)),
                                                                    CInt(Matrice_Variabili(RigheMatrice, 2)),
                                                                    CInt(Matrice_Variabili(RigheMatrice, 3)),
                                                                    "", "", objParametri_Server)

                            objRegImpianti = Nothing

                            If CodiceTerreno <> 0 Then
                                Dim Codice_Anagrafe_R As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R
                                strCodiceTerreno = Codice_Anagrafe_R.CodiceAnagrafeDes_from_CodiceAnagrafeCod(CodiceTerreno, objParametri_Server)
                                If InStr(strCodiciTerreno, strCodiceTerreno) = 0 Then
                                    strCodiciTerreno &= strCodiceTerreno & ", "
                                End If
                                If Not HashDestUso.ContainsKey(CodiceTerreno) Then
                                    HashDestUso.Add(CodiceTerreno, strCodiceTerreno)
                                End If
                            Else
                                If Matrice_Variabili(RigheMatrice, 4) = "0" Then
                                    If InStr(strCodiciTerreno, "Terreno Nudo") = 0 Then
                                        strCodiciTerreno &= "Terreno Nudo, "
                                    End If
                                    If Not HashDestUso.ContainsKey(CodiceTerreno) Then
                                        HashDestUso.Add(CodiceTerreno, "Terreno Nudo")
                                    End If
                                End If
                            End If


                            If Not CStr(XML_VariabiliStampe.GetAttribute("sezione")) Is Nothing Then
                                Qs_Sezione = CStr(XML_VariabiliStampe.GetAttribute("sezione"))
                                ''Matrice_Variabili(i, 5) = XML_VariabiliStampe.GetAttribute("sezione")
                                Matrice_Variabili(RigheMatrice, 5) = XML_VariabiliStampe.GetAttribute("sezione")
                            Else
                                ''Matrice_Variabili(i, 5) = ""
                                Matrice_Variabili(RigheMatrice, 5) = ""
                            End If

                            Dim objImpianto As New AgronicaCoreGestioneRichieste.Impianto

                            objImpianto.Piva = XML_VariabiliStampe.GetAttribute("piva")
                            objImpianto.Sa_Cod = XML_VariabiliStampe.GetAttribute("sa_cod")
                            objImpianto.Appezza = XML_VariabiliStampe.GetAttribute("appezza")
                            objImpianto.Id_Reg = XML_VariabiliStampe.GetAttribute("id_reg")
                            objImpianto.Veg_Cod = XML_VariabiliStampe.GetAttribute("veg_cod")

                            objVerificaDisciplinare2010.AddList(objImpianto)

                        Next

                        '------------

                        Session("Matrice_Variabili") = Matrice_Variabili

                        Qs_Piva = Matrice_Variabili(0, 0)

                        '(03/09/2016) fede aggiunte combo
                        Dim strSaCod As String = ""
                        Dim strVegCod As String = ""
                        For Each Hsacod As Int32 In HashSaCod.Keys
                            strSaCod &= Hsacod & ","
                        Next
                        For Each Hvegcod As Int32 In HashVegCod.Keys
                            strVegCod &= Hvegcod & ","
                        Next

                        If strSaCod <> "" Then
                            clc.Centri_Aziendali(Cmb_CentroAziendale,
                                                                        True, "Tutti i Centri", "0",
                                                                        Qs_Piva,
                                                                         False,
                                                                         2,
                                                                         " sa_cod IN (" & Left(strSaCod, strSaCod.Length - 1) & ") ", "",
                                                                            objParametri_Server)
                        End If

                        If strVegCod <> "" Then
                            AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize(Cmb_Specie,
                                                                        False, "", "0",
                                                                        0,
                                                                        "", True, 0, 0, 0,
                                                                        " SpecieVegetali.veg_cod IN (" & Left(strVegCod, strVegCod.Length - 1) & ") ", "",
                                                                        objParametri_Server, objParametri_Utenti)

                        End If

                        If strCodiciTerreno <> "" Then
                            strCodiciTerreno = Left(strCodiciTerreno, strCodiciTerreno.Length - 2)
                            For Each Key As Int32 In HashDestUso.Keys
                                Select Case Key
                                    Case 0 'terreno nudo
                                        Cmb_Specie.Items.Add(New ListItem(HashDestUso.Item(Key), 0))
                                    Case Else 'dest uso
                                        Cmb_Specie.Items.Add(New ListItem(HashDestUso.Item(Key), -Key))
                                End Select
                            Next
                        End If

                        Qs_Sa_Cod = Matrice_Variabili(0, 1) 'deve essere = a quello di riferimento
                        Qs_Veg_Cod = Matrice_Variabili(0, 4)


                        Session("Piva") = Qs_Piva

                        objVerificaDisciplinare2010.Piva = Qs_Piva


                        Dim Gru_Cod As Integer

                        Gru_Cod = CInt(GruppoVeg.GruCod_from_VegCod(Qs_Veg_Cod, objParametri_Server))

                        If Gru_Cod = 1 AndAlso Not Flag_ErbOrt Then

                            DivRotazione.Visible = False
                            Me.Chk_Priorita_ColturePrecedenti.Visible = False

                        End If

                        If Qs_Veg_Cod = "0" Then

                            'If strCodiciTerreno <> "" Then

                            '    If Report <> enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita Then
                            '        Me.Lbl_Specie_Vegetale.Text = strCodiciTerreno
                            '    Else
                            '        Me.Lbl_Specie_Vegetale.Text = "......"
                            '    End If
                            'End If

                            Me.Lbl_Specie_Vegetale.Text = "......"

                            'se ho scelto un rilievo piogge,nn avendo la specie,nn faccio scegliere le altre sezioni
                        ElseIf Qs_Veg_Cod = "-1" Then
                            Me.Lbl_Specie_Vegetale.Text = " "
                            Me.CBL_Sezioni.Enabled = False
                            Me.ImgBtnSelezionaTutte.Enabled = False
                            Me.ImgBtnEliminaSelezione.Enabled = False
                        Else

                            'If Report <> enum_CodificaStampe.RegistroTrattamenti_Semplificata AndAlso
                            '    Report <> enum_CodificaStampe.Registro_Fertilizzazioni AndAlso
                            '    Report <> enum_CodificaStampe.RegistroTrattamenti_Veneto AndAlso
                            '    Report <> enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita AndAlso
                            '    Report <> enum_CodificaStampe.SchedaCampagna_Multi_Lombardia AndAlso
                            '    Report <> enum_CodificaStampe.RegistroAziendaleUnico Then

                            '    Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                            '    Me.Lbl_Specie_Vegetale.Text = objSpecie.VegDes_from_VegCod(CInt(Qs_Veg_Cod), objParametri_Server)

                            '    objSpecie = Nothing
                            'Else
                            '    Me.Lbl_Specie_Vegetale.Text = "......"
                            'End If

                            If HashVegCod.Count = 1 OrElse HashDestUso.Count = 1 Then
                                Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
                                Me.Lbl_Specie_Vegetale.Text = objSpecie.VegDes_from_VegCod(CInt(Qs_Veg_Cod), objParametri_Server)
                            Else
                                Me.Lbl_Specie_Vegetale.Text = "......"
                            End If

                        End If

                        '---------------------------
                        If Report = enum_CodificaStampe.SchedaInterventiAgronomici Then
                            '20/04/2018: gestione del codice “Organismo referente intestatario QDC”  (sviluppo per orogel)
                            Dim objcodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                            Dim flag_organismoIntest As Integer = 0
                            Dim Val_Cod As String = ""
                            Val_Cod = objcodici.ValCod_from_SaCodIdCod(Qs_Piva, Qs_Sa_Cod, enum_CodiciAnagrafe.OrganismoRefIntestatarioQDC, objParametri_Server)
                            If IsNumeric(Val_Cod) Then
                                Val_Cod = CInt(flag_organismoIntest)
                                If flag_organismoIntest = 1 Then
                                    Me.Chk_IntestazioneOrgref.Checked = True
                                End If
                            End If
                        End If


                        '-------------------------------------------------------------------
                        '29/08/2017 MarcoG: DDL DEI MAGAZZINI PER LE STAMPE DIRETTE
                        clc.Fabbricati(ddlMagazzini, True, "Selezionare un Magazzino...", "-1", Qs_Piva, 0, 0,
                                                                         FABBRICATI_NO_STALLE, True, "", " Fabbricati.Fabbricato_Des ", AGRODATAFINE, objParametri_Server)

                        'Se c'è un solo elemento, lo seleziono già
                        If ddlMagazzini.Items.Count = 2 Then
                            ddlMagazzini.SelectedIndex = 1
                        End If

                        '-------------------------------------------------------------------
                        '----- Tabella Imprese

                        Me.Lbl_Piva.Text = Qs_Piva

                        If Report <> enum_CodificaStampe.RegistroTrattamenti_Semplificata AndAlso
                            Report <> enum_CodificaStampe.Registro_Fertilizzazioni AndAlso
                            Report <> enum_CodificaStampe.RegistroTrattamenti_Veneto AndAlso
                            Report <> enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita AndAlso
                            Report <> enum_CodificaStampe.SchedaCampagna_Multicentro AndAlso
                            Report <> enum_CodificaStampe.SchedaCampagna_Multi_Lombardia AndAlso
                           Report <> enum_CodificaStampe.Eurep_Gap_Multicentro AndAlso
                                Report <> enum_CodificaStampe.RegistroAziendaleUnico Then

                            Dim DtCentro As New DataTable
                            Dim objCentro As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

                            DtCentro = objCentro.Leggi(Qs_Piva, Qs_Sa_Cod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                                                      "", "", objParametri_Server)

                            If Not DtCentro Is Nothing AndAlso DtCentro.Rows.Count > 0 Then

                                Me.Lbl_Impresa.Text = DtCentro.Rows(0).Item("rag_soc")
                                Me.Lbl_Centro.Text = DtCentro.Rows(0).Item("Sa_Nome")

                                '----------------------------------------------------

                                If DtCentro.Rows(0).Item("Validita_Inizio") = "01/01/1900" Then
                                    Lbl_Validita_Inizio.Text = "  ......  "
                                Else
                                    Lbl_Validita_Inizio.Text = DtCentro.Rows(0).Item("Validita_Inizio")
                                End If

                                If DtCentro.Rows(0).Item("Validita_Fine") = "31/12/2100" Then
                                    Lbl_Validita_Fine.Text = "  ......  "
                                Else
                                    Lbl_Validita_Fine.Text = DtCentro.Rows(0).Item("Validita_Fine")
                                End If

                            End If

                        Else
                            'Se la scheda è multicentro imposto solo la label della ragione sociale
                            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

                            Me.Lbl_Impresa.Text = objImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
                            Me.Lbl_Centro.Text = "  ......  "
                            Lbl_Validita_Inizio.Text = "  ......  "
                            Lbl_Validita_Fine.Text = "  ......  "

                        End If

                        Master.LblRag_Soc.Text = Me.Lbl_Impresa.Text

                        Dim ProCod As String = String.Empty
                        Dim ProCodIstat As String = String.Empty
                        Dim RegioneCod As String = String.Empty

                        Dim objInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                        objInd.Indirizzo_from_PivaSaCod(Qs_Piva, Qs_Sa_Cod,
                                                        Nothing, Nothing,
                                                        Nothing, Nothing,
                                                        Nothing, ProCod, Nothing,
                                                        ProCodIstat, Nothing,
                                                        RegioneCod, objParametri_Server)

                        'Seleziono la regione di appartenenza dell'impresa
                        Cmb_Regioni.SelectedIndex =
                            Cmb_Regioni.Items.IndexOf(
                                Cmb_Regioni.Items.FindByValue(
                                    RegioneCod))


                End Select


                '---------------------
                '(03/09/2016) Fede: temporaneo da sistemare  '  Galassi, 16/09/2016 14:54:48: Tericamente e practicamente non più temporaneo
                Select Case Report

                    Case enum_CodificaStampe.Registro_Trattamenti_Massivo,
                        enum_CodificaStampe.Registro_Fertilizzazioni_Massivo,
                        enum_CodificaStampe.Registro_Fertilizzazioni,
                        enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                        enum_CodificaStampe.RegistroTrattamenti_Veneto,
                        enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita,
                        enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
                        enum_CodificaStampe.RegistroAziendaleUnico

                        Cmb_CentroAziendale.Visible = False
                        Cmb_Specie.Visible = False
                        Lbl_Specie_Vegetale.Style.Remove("display")
                        Lbl_Centro.Style.Remove("display")

                    Case Else
                        If ViewState("UtilizzaImpiantiFiltrati") = 1 Then

                            Cmb_CentroAziendale.Visible = False
                            Cmb_Specie.Visible = False
                            Lbl_Specie_Vegetale.Style.Remove("display")
                            Lbl_Centro.Style.Remove("display")
                        End If

                End Select

                '------------------------------
                Select Case Report

                    Case enum_CodificaStampe.Eurep_Gap,
                     enum_CodificaStampe.Eurep_Gap_Semplificata,
                         enum_CodificaStampe.SchedaCampagna_Pizzoli,
                        enum_CodificaStampe.Eurep_Gap_Multicentro

                        DivGlobalGap.Visible = True
                        DivTempoRientro.Visible = True

                        If Report = enum_CodificaStampe.Eurep_Gap_Multicentro Then
                            CheckTitoloGlobal.Visible = True
                        End If

                    Case enum_CodificaStampe.SchedaCampagna_Multi_Lombardia
                        DivRegolamenti.Visible = True

                    Case enum_CodificaStampe.RegistroAziendaleUnico
                        DivRegolamenti.Visible = True
                        DivOrdinamento.Visible = True

                    Case enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita
                        DivOrdinamento.Visible = True

                End Select
                '-------------------------------------

            End If 'XmlDoc.HasChildNodes

            If Log <> "" Then
                Messaggi.AgroMsgBox(Log, Me.Master.Page)
            End If

            'la prima volta che carico la pagina la metto in primo piano
            '(in caso contrario rimane in primo piano la pagina del GiasOnline)
            'Dim strFocus As String = "<script language='javascript'> window.focus() </script>"
            'Me.FindControl("Form1").Controls.Add(New LiteralControl(strFocus))
            Dim listaReportConMostraDataOdiernaGestita = New List(Of enum_CodificaStampe) From {enum_CodificaStampe.SchedaColturale_Biologico,
                enum_CodificaStampe.SchedaCampagna_Multicentro_ACA}

            If Not listaReportConMostraDataOdiernaGestita.Contains(Report) Then
                Me.Chk_MostraDataOdiernaStampa.Visible = False
            End If

            Select Case Report
                Case enum_CodificaStampe.SchedaCampagna_Multicentro_ACA
                    Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim valore_Impostazione_Cod_PreflagMostraData = Integer.Parse(objImpostazioni.LeggiConDefault(
                            enum_Impostazioni_Utenti.Utente_Cod_DefaultMostraDataACA, 2,
                            "0", objParametri_Utenti)) = 1

                    Me.Chk_MostraDataOdiernaStampa.Checked = valore_Impostazione_Cod_PreflagMostraData

                Case enum_CodificaStampe.Eurep_Gap_Semplificata,
                     enum_CodificaStampe.SchedaCampagna_Pizzoli,
                     enum_CodificaStampe.Eurep_Gap_Multicentro,
                     enum_CodificaStampe.SchedaCampagna_ProvAut_Trento

                    Me.CBL_Sezioni.Items.FindByValue("y").Enabled = True
                    Me.CBL_Sezioni.Items.FindByValue("r").Enabled = True
                    'CBL_Sezioni.Items.Add(New ListItem("Manutenzione Macchinari", "y"))
                    'CBL_Sezioni.Items.Add(New ListItem("Visite Ispettive", "r"))
                    Me.Chk_VisualizzaCapitolatoPrivato.Visible = True

                Case enum_CodificaStampe.SchedaCampagna_Multicentro,
                    enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                        enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                        enum_CodificaStampe.SchedaCampagna_ConserveItalia

                    Me.Chk_VisualizzaCapitolatoPrivato.Visible = True

                Case enum_CodificaStampe.SchedaColturale_Biologico

                    Me.Chk_Data_UltimaRaccolta.Visible = False
                    Me.Chk_Qta_Raccolte.Visible = False
                    Me.Chk_Tutte_Raccolte.Visible = False
                    Me.Chk_RaggruppaXCampo.Visible = True
                    Me.Chk_RaggruppaXCampo.Checked = True

                    Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim valore_Impostazione_Cod_MostraFirmaODC = Integer.Parse(objImpostazioni.LeggiConDefault(
                            enum_Impostazioni_Utenti.UTENTE_Cod_MostraFirmaODCBio, 2,
                            "0", objParametri_Utenti)) = 1
                    Dim valore_Impostazione_Cod_PreflagMostraData = Integer.Parse(objImpostazioni.LeggiConDefault(
                            enum_Impostazioni_Utenti.Utente_Cod_DefaultMostraDataBio, 2,
                            "0", objParametri_Utenti)) = 1

                    Me.Chk_MostraFirmaODC.Checked = valore_Impostazione_Cod_MostraFirmaODC
                    Me.Chk_MostraFirmaODC.Visible = valore_Impostazione_Cod_MostraFirmaODC

                    Me.Chk_MostraDataOdiernaStampa.Checked = valore_Impostazione_Cod_PreflagMostraData

                    'Case enum_CodificaStampe.RegistroTrattamenti_Semplificata
                    '    CBL_Sezioni.Items.Insert(2, New ListItem("Personale con Patentino", "s"))

                Case enum_CodificaStampe.RegistroTrattamenti_Veneto
                    '20/06/2018
                    Me.CBL_Sezioni.Visible = True
                    SEZIONI_VisualizzaSezionixRegTrattVeneto()

                    'Me.CBL_SezioniVeneto.Visible = True
                    Me.RBL_SezioniVeneto.Visible = True
                    'Me.ImgBtnSelezionaTutte.Visible = False
                    'Me.Label15.Visible = False
                    'Me.ImgBtnEliminaSelezione.Visible = False
                    'Me.Label14.Visible = False
                    'Cmb_Regioni.Visible = False
                    Chk_Data_UltimaRaccolta.Visible = False
                    Chk_Qta_Raccolte.Visible = False
                    Chk_Tutte_Raccolte.Visible = False
                    Lbl_ImpostaSezioni.Visible = False

                    ' TO DO NICO
                    BtnStampeVuote.Visible = False
            End Select

            If {enum_CodificaStampe.SchedaCampagna_Multicentro, enum_CodificaStampe.RegistroAziendaleUnico, enum_CodificaStampe.Eurep_Gap_Multicentro}.Contains(Report) Then
                Me.Chk_Avversita_Qta.Visible = True
                Dim impAvversita = objUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.StampaCampagna_Default_Vedi_AvversitaQta, objParametri_Utenti)
                If String.IsNullOrEmpty(impAvversita) OrElse impAvversita = "1" Then
                    Me.Chk_Avversita_Qta.Checked = True
                End If
            End If


            'CBL_Sezioni.Items.Add(New ListItem("Pratiche Ecologiche", "u"))
            ''questo filtro è per le impostazioni utente riguardanti le pratiche ecologiche
            'DrFiltro = DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Security_Attivita.CheckList_Pratiche_Ecologiche_APOT)
            'If Not DrFiltro Is Nothing AndAlso DrFiltro.Length > 0 Then
            '    If Not IsDBNull(DrFiltro(0).Item("Impostazione_Valore_1")) AndAlso DrFiltro(0).Item("Impostazione_Valore_1") <> "" Then
            '        RigaFascicoli.Visible = True
            '        Dim FiltroFascicoli As String = "Allegati_Documenti_Piva='" & Qs_Piva & "' AND Allegati_Documenti_CatCod=9 "
            '        AgronicaCoreUtility.CaricaListControl.Fascicoli(Cmb_Fascicoli, True, "Tutte", "0", FiltroFascicoli, " Allegati_Documenti_Numero DESC ", objParametri_Server)
            '    End If
            'End If

            'SPOSTATO QUI PER ABILITARE O DISABILITARE SUCCESSIVAMENTE
            SEZIONI_AggiungiSezioniConPermessi(Report)

            SEZIONI_AbilitaDisabilita(Report)

            If Qs_Sezione = "" Then
                If Report <> enum_CodificaStampe.RegistroTrattamenti_Veneto Then
                    SEZIONI_SelezionaTutte()
                End If
            Else

                For i = 0 To Me.CBL_Sezioni.Items.Count - 1

                    If InStr(Qs_Sezione, Me.CBL_Sezioni.Items(i).Value) <> 0 Then

                        'End If
                        'If Me.CBL_Sezioni.Items(i).Value = Qs_Sezione Then
                        Me.CBL_Sezioni.Items(i).Selected = True
                    Else
                        Me.CBL_Sezioni.Items(i).Selected = False
                    End If

                Next

            End If

            'se ho selezionato un rilievo piogge..
            If Qs_Sezione = "m" Then

                For i = 0 To Me.CBL_Sezioni.Items.Count - 1

                    If InStr(Qs_Sezione, Me.CBL_Sezioni.Items(i).Value) <> 0 Then
                        'If Me.CBL_Sezioni.Items(i).Value = Qs_Sezione Then
                        Me.CBL_Sezioni.Items(i).Selected = True
                    Else
                        Me.CBL_Sezioni.Items(i).Selected = False
                    End If

                Next

            End If



            ViewState("Piva") = Qs_Piva
            ViewState("Sa_Cod") = Qs_Sa_Cod



            '##############################################################
            '#####  CARICO I DATI  ########################################
            '##############################################################


            Imposta_Pannelli(enum_Pannello.Pannello_Filtro)

            Session("Revisione") = Nothing


            '-------------------------------------------------------------------
            '----- Imposto la data di stampa

            Me.rblStampa.SelectedItem.Value = 1
            rblStampa_SelectedIndexChanged(Me, Nothing)


            Dim PathAllegati As String
            'Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
            If objAgroWeb.GestioneAllegati_Repository = String.Empty Then
                PathAllegati = "C:\GIASLAN\AgronicaStampe_Allegati\"
            Else
                PathAllegati = objAgroWeb.GestioneAllegati_Repository
            End If

            RblStampaProva.ToolTip = "La stampa definitiva salverà una copia del file nella cartella " & PathAllegati

            objAgroWeb = Nothing

            'In base all'impostazione utente visualizzo o meno la combo fascicoli (x umbria)


            Dim DtImpostazioni As DataTable

            DtImpostazioni = objUtenti.Leggi(0, 1,
                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "", "", objParametri_Utenti)
            Dim DrFiltro() As DataRow

            ' questo filtro lo leggo sempre
            DrFiltro = DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_FILTRA_FASCICOLO)
            If Not DrFiltro Is Nothing AndAlso DrFiltro.Length > 0 Then
                If Not IsDBNull(DrFiltro(0).Item("Impostazione_Valore_1")) AndAlso DrFiltro(0).Item("Impostazione_Valore_1") <> "" Then
                    RigaFascicoli.Visible = True
                    Dim FiltroFascicoli As String = "Allegati_Documenti_Piva='" & Qs_Piva & "' AND Allegati_Documenti_CatCod=9 "
                    AgronicaCoreUtility.CaricaListControl.Fascicoli(Cmb_Fascicoli, True, "Tutte", "0", FiltroFascicoli, " Allegati_Documenti_Numero DESC ", objParametri_Server)
                End If
            End If


        Catch ex As Exception
            Log_Errori &= "pageload: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '-----------------------------
        If Log_Errori <> "" Then

            Dim nomeFile As String = "LogErrori_filtroQDC_" & objParametri_Server.UtenteUsername & ".txt"

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                             "Stampe_QDC",
                                             nomeFile,
                                             objParametri_Server.UtenteUsername,
                                             "Selezione_SchedaCampagna.aspx",
                                             Log_Errori)

            Messaggi.AgroMsgBox(Log_Errori, Me.Master.Page)
        End If


    End Sub


    '########################################################################################
    Private Enum enum_Pannello

        Pannello_Filtro = 1
        Pannello_ColturePrecedenti = 2
        Pannello_GlobalGAP = 3

    End Enum

    '###########################################################################
    Private Sub Imposta_Pannelli(ByVal PannelloAttivo As enum_Pannello)

        Tabella_Filtro.Visible = False
        Tabella_Rotazione.Visible = False
        Tabella_GlobalGap.Visible = False


        Select Case PannelloAttivo

            Case enum_Pannello.Pannello_Filtro
                Tabella_Filtro.Visible = True

            Case enum_Pannello.Pannello_ColturePrecedenti
                Tabella_Rotazione.Visible = True

            Case enum_Pannello.Pannello_GlobalGAP
                Tabella_GlobalGap.Visible = True

        End Select

    End Sub

    '#############################################
    '20/06/2018: centralizzato codice in funzione dedicata
    Private Sub SEZIONI_SalvaSezioniSeleziontexStampa(ByRef strSezioni As String,
                                              ByRef Num_Sezioni As Integer,
                                              ByVal SchedaVeneto As String)

        'Salvo in una var di sessione i dati selezionati
        'se ho selezionato il sottoreport metto il num a cui equivale
        'altrimenti metto 0
        For i = 0 To Me.CBL_Sezioni.Items.Count - 1
            If Me.CBL_Sezioni.Items(i).Selected Then
                strSezioni = strSezioni & CBL_Sezioni.Items(i).Value & ","
                Num_Sezioni += 1
            Else
                'strSezioni = strSezioni & "0,"
            End If
        Next

        If strSezioni <> "" And SchedaVeneto = "" Then
            'caso stampe <> veneto
            'tolgo l'ultima virgola
            strSezioni = Left(strSezioni, strSezioni.Length - 1)
        ElseIf strSezioni <> "" And SchedaVeneto <> "" Then
            'c'è un filtro sezioni ed è la stampa del veneto
            'non tolgo la virgola e aggiungo la scheda veneto (i miei predecessori hanno gestito la scheda sfruttando le sezioni)
            If SchedaVeneto.ToLower = "a" Then
                'la manutenzione dei macchinari al momento è attivata solo su scheda A
                strSezioni &= SchedaVeneto
            Else
                'per le altre impsoto solo la scheda
                strSezioni = SchedaVeneto
            End If

        ElseIf strSezioni = "" And SchedaVeneto <> "" Then
            'non c'è un filtro sezioni ed è la stampa del veneto
            'non tolgo la virgola e aggiungo la scheda veneto (i miei predecessori hanno gestito la scheda sfruttando le sezioni)
            strSezioni &= SchedaVeneto
        Else
            'scheda <> dal veneto in cui non sono state selezionate sezioni
            '-> viene dato alert di blocco
            Dim debug As Boolean = True
        End If

        Session("sezioni") = strSezioni

    End Sub

    '#############################################
    Private Sub SEZIONI_VisualizzaSezionixRegTrattVeneto()

        'rimuovo tutte le sezioni e aggiungo quella delle macchine (che è stata introdotta opzionale alla scheda A)
        Me.CBL_Sezioni.Items.Clear()
        Me.CBL_Sezioni.Items.Add(New System.Web.UI.WebControls.ListItem("Manutenzione Macchinari (Scheda A)", "y"))

    End Sub

    '###########################################################################
    Private Sub rblStampa_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rblStampa.SelectedIndexChanged

        If Me.rblStampa.SelectedItem.Value = 0 Then

            Cella_Frecce.Visible = False
            Cella_Giorno.Visible = True
            Riga_Intervallo.Visible = False

            TxtStampa.Text = Date.Today.ToShortDateString

        Else

            Cella_Frecce.Visible = True
            Cella_Giorno.Visible = False
            Riga_Intervallo.Visible = True

            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DataInizio, DataFine As Date
            objImpost.AnnataAgraria(Date.Today, DataInizio, DataFine, objParametri_Utenti)

            TxtValiditaInizio.Text = DataInizio
            TxtValiditaFine.Text = DataFine

            ' di defualt imposto l'anno solare, e non più l'annata agraria (23/04/2014 Nico chiesto da Fabrizio)
            'TxtValiditaInizio.Text = "01/01/" & Now.Year
            'TxtValiditaFine.Text = "31/12/" & Now.Year

            ''di default imposto le date dell'intervallo coincidenti con l'annata agraria 
            ''dal 1/11 al 31/10
            'Mese = Now.Month

            'Select Case Mese
            '    Case 11, 12
            '        'se sono nei mesi di novembre o dicembre..........
            '        'l'annata agraria va dal 1/11 di quest'anno al 31/10 del prossimo
            '        TxtValiditaInizio.Text = "01/11/" & Now.Year
            '        TxtValiditaFine.Text = "31/10/" & Now.Year + 1

            '    Case Else
            '        'l'annata agraria va dal 1/11 dell'anno scorso al 31/10 di quest'anno
            '        TxtValiditaInizio.Text = "01/11/" & Now.Year - 1
            '        TxtValiditaFine.Text = "31/10/" & Now.Year

            'End Select

        End If

        RicaricaCalendari()

    End Sub



    '########################################################################################
    Private Sub ImgBtnSelezionaTutte_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnSelezionaTutte.Click
        SEZIONI_SelezionaTutte()
    End Sub

    Private Sub SEZIONI_SelezionaTutte()

        Dim i As Integer
        Dim Report As Integer

        Report = ViewState("Report")

        Select Case Report

            Case enum_CodificaStampe.SchedaInterventiAgronomici

                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FRONTESPIZIO).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FERTILIZZAZIONI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.RIL_AVVERS_NELLE_TRAPP).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.RIL_AVVERS_IN_CAMPO).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.IRRIGAZIONE).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.ALTRE_OPER_COLTURALI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.RIL_PROD_E_DATA_RACC).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.PIOGGE).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.VISITE_ISPETTIVE).Selected = True


            Case enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                   enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                   enum_CodificaStampe.SchedaCampagna_Multicentro

                For i = 0 To Me.CBL_Sezioni.Items.Count - 1
                    If CBL_Sezioni.Items(i).Enabled = True Then
                        Me.CBL_Sezioni.Items(i).Selected = True
                    End If
                Next

                'deseleziono la sezione fasi fenologiche e patentino
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FASI_FENOLOGICHE).Selected = False
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.PERSONALE_CON_PATENTINO).Selected = False

                'deseleziono tratt post raccolta
                If Not IsNothing(Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA)) Then
                    Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA).Selected = False
                End If

            Case enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                enum_CodificaStampe.Registro_Trattamenti_Massivo

                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FRONTESPIZIO).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.PERSONALE_CON_PATENTINO).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.DATI_CATASTALI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.INFORMAZ_E_DICHIAR).Selected = True

            Case enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita

                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FRONTESPIZIO).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.MANUTEN_MACCHINARI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.PERSONALE_CON_PATENTINO).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA).Selected = True


            Case enum_CodificaStampe.SchedaCampagna_Multi_Lombardia

                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FRONTESPIZIO).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.DATI_CATASTALI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FERTILIZZAZIONI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.IRRIGAZIONE).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.ALTRE_OPER_COLTURALI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.PIOGGE).Selected = True
                Me.Chk_LogoRegione.Checked = False

            Case enum_CodificaStampe.RegistroAziendaleUnico
                For i = 0 To Me.CBL_Sezioni.Items.Count - 1
                    If CBL_Sezioni.Items(i).Enabled = True Then
                        Me.CBL_Sezioni.Items(i).Selected = True
                    End If
                Next
                'Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FRONTESPIZIO).Selected = True
                'Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.DATI_CATASTALI).Selected = True
                'Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FERTILIZZAZIONI).Selected = True
                'Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI).Selected = True
                'Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.IRRIGAZIONE).Selected = True
                'Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.ALTRE_OPER_COLTURALI).Selected = True
                'Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.PIOGGE).Selected = True
                'Me.Chk_LogoRegione.Checked = False

            Case enum_CodificaStampe.Registro_Fertilizzazioni,
                enum_CodificaStampe.Registro_Fertilizzazioni_Massivo

                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FRONTESPIZIO).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.DATI_CATASTALI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FERTILIZZAZIONI).Selected = True
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.INFORMAZ_E_DICHIAR).Selected = True

            Case enum_CodificaStampe.RegistroTrattamenti_Veneto
                RBL_SezioniVeneto.Items.FindByValue(ElencoReportChiave.FRONTESPIZIO).Selected = True

            Case enum_CodificaStampe.SchedaCampagna_ConserveItalia,
                enum_CodificaStampe.SchedaColturale_Biologico
                For i = 0 To Me.CBL_Sezioni.Items.Count - 1
                    Me.CBL_Sezioni.Items(i).Selected = True
                Next
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.PERSONALE_CON_PATENTINO).Selected = False

                'deseleziono tratt post raccolta
                If Not IsNothing(Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA)) Then
                    Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA).Selected = False
                End If

                If Report = enum_CodificaStampe.SchedaCampagna_ConserveItalia AndAlso Not IsNothing(Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.INFORMAZ_E_DICHIAR)) Then
                    Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.INFORMAZ_E_DICHIAR).Selected = False
                End If
                If Report = enum_CodificaStampe.SchedaCampagna_ConserveItalia AndAlso Not IsNothing(Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.VISITE_ISPETTIVE)) Then
                    Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.VISITE_ISPETTIVE).Selected = False
                End If

            Case enum_CodificaStampe.Eurep_Gap_Semplificata,
                 enum_CodificaStampe.SchedaCampagna_Pizzoli,
                 enum_CodificaStampe.Eurep_Gap_Multicentro
                For i = 0 To Me.CBL_Sezioni.Items.Count - 1
                    Me.CBL_Sezioni.Items(i).Selected = True
                Next

                'deseleziono la sezione macchinari e note
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.INFORMAZ_E_DICHIAR).Selected = False
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.MANUTEN_MACCHINARI).Selected = False

                'deseleziono tratt post raccolta
                If Not IsNothing(Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA)) Then
                    Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA).Selected = False
                End If

            Case Else
                For i = 0 To Me.CBL_Sezioni.Items.Count - 1
                    If CBL_Sezioni.Items(i).Enabled = True Then
                        Me.CBL_Sezioni.Items(i).Selected = True
                    End If
                Next


        End Select


        If Not IsNothing(Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.VERIFICHE_CONFORMITA)) Then
            Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.VERIFICHE_CONFORMITA).Selected = False
        End If

        If Report <> enum_CodificaStampe.RegistroTrattamenti_Veneto Then
            If Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.TRATTAMENTI).Selected = True Then
                Me.CBL_Sezioni.Items.FindByValue(ElencoReportChiave.FITOFARMACI).Selected = True
            End If
        End If

    End Sub

    '########################################################################################
    Private Sub ImgBtnEliminaSelezione_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnEliminaSelezione.Click
        SEZIONI_DeselezionaTutte()
    End Sub

    Private Sub SEZIONI_DeselezionaTutte()
        Dim i As Integer
        Dim Report As Integer

        Report = ViewState("Report")

        Select Case Report
            Case enum_CodificaStampe.RegistroTrattamenti_Veneto
                Me.RBL_SezioniVeneto.Items(0).Selected = False
                Me.RBL_SezioniVeneto.Items(1).Selected = False
                Me.RBL_SezioniVeneto.Items(2).Selected = False
                Me.RBL_SezioniVeneto.Items(3).Selected = False
                Me.RBL_SezioniVeneto.Items(4).Selected = False
            Case Else
                For i = 0 To Me.CBL_Sezioni.Items.Count - 1
                    Me.CBL_Sezioni.Items(i).Selected = False
                Next
        End Select

    End Sub


    '##############################################################################################################
    Private Sub Carica_Dgr_ColturePrecedenti()

        Dim GruppoVeg As New AgronicaCoreMetaSchemaDAL.GruppoVegetale_R
        Dim Reg_Impianti_Read As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
        Dim ObjImpiantiCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

        Dim Dt As New DataTable
        Dim Dr As DataRow
        Dim i As Integer

        Dim DtCodici As New DataTable
        Dim DrCodici() As DataRow

        Dim Qs_Piva As String
        Dim Qs_Sa_Cod As String
        Dim Validita_inizio As Date
        Dim Validita_fine As Date
        Dim Data_inizio, Data_fine As Date

        Dim Matrice_Variabili(0, 0) As String
        Dim Indici_Matrice() As Integer

        Dim Counter As Integer = 0
        Dim Vet_Impianti() As String


        '----- Definisco la struttura del DataTable

        Dt.Columns.Add(New DataColumn("Piva", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Cod", GetType(String)))
        Dt.Columns.Add(New DataColumn("Appezza", GetType(String)))
        Dt.Columns.Add(New DataColumn("Id_Reg", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sa_Nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("Campo_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("App_Nome", GetType(String)))
        Dt.Columns.Add(New DataColumn("Veg_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Cul_Des", GetType(String)))
        Dt.Columns.Add(New DataColumn("Dest_Uso", GetType(String)))
        Dt.Columns.Add(New DataColumn("Sup_Imp", GetType(String)))
        Dt.Columns.Add(New DataColumn("Validita", GetType(String)))

        Dt.Columns.Add(New DataColumn("Coltura1", GetType(String)))
        Dt.Columns.Add(New DataColumn("Coltura2", GetType(String)))
        Dt.Columns.Add(New DataColumn("Coltura3", GetType(String)))
        Dt.Columns.Add(New DataColumn("Coltura4", GetType(String)))


        Matrice_Variabili = Session("Matrice_Variabili")

        Qs_Piva = ViewState("Piva")
        Qs_Sa_Cod = ViewState("Sa_Cod")

        If Me.TxtValiditaInizio.Text <> "" Then
            Data_inizio = CDate(Me.TxtValiditaInizio.Text)
        Else
            Data_inizio = AGRODATAINIZIO
        End If

        If Me.TxtValiditaFine.Text <> "" Then
            Data_fine = CDate(Me.TxtValiditaFine.Text)
        Else
            Data_fine = AGRODATAFINE
        End If


        For i = 0 To UBound(Matrice_Variabili)

            If CInt(GruppoVeg.GruCod_from_VegCod(Matrice_Variabili(i, 4), objParametri_Server)) <> 1 AndAlso
               Not Matrice_Variabili(i, 0) Is Nothing Then

                If Indici_Matrice Is Nothing Then
                    ReDim Indici_Matrice(0)
                Else
                    ReDim Preserve Indici_Matrice(Indici_Matrice.Length)
                End If

                Indici_Matrice(Counter) = i
                Counter += 1

                'passo l'annata agraria selezionata nel filtro,
                'così evito di visualizzare impianti non attivi in quell'annata
                Vet_Impianti = Reg_Impianti_Read.Descrizioni2_From_Piva_Sa_Cod_Appezza_Id_Reg(
                                                CStr(Matrice_Variabili(i, 0)),
                                                CInt(Matrice_Variabili(i, 1)),
                                                Matrice_Variabili(i, 2),
                                                Matrice_Variabili(i, 3),
                                                Data_inizio,
                                                Data_fine,
                                                objParametri_Server)

                If Not IsNothing(Vet_Impianti) AndAlso Not IsNothing(Vet_Impianti(0)) Then

                    'Creo una nuova riga
                    Dr = Dt.NewRow

                    ' la riempio
                    Dr.Item("piva") = Matrice_Variabili(i, 0)
                    Dr.Item("sa_cod") = Matrice_Variabili(i, 1)
                    Dr.Item("Appezza") = Matrice_Variabili(i, 2)
                    Dr.Item("Id_Reg") = Matrice_Variabili(i, 3)
                    Dr.Item("Sa_Nome") = Vet_Impianti(1)
                    Dr.Item("Campo_Des") = Vet_Impianti(15)
                    Dr.Item("App_Nome") = Vet_Impianti(2)
                    Dr.Item("Veg_Des") = Vet_Impianti(10)
                    Dr.Item("Cul_des") = Vet_Impianti(8)
                    Dr.Item("Dest_uso") = Vet_Impianti(12)
                    Dr.Item("sup_imp") = Vet_Impianti(6)

                    Validita_inizio = Vet_Impianti(13)
                    Validita_fine = Vet_Impianti(14)

                    Dr.Item("validita") = "Dal "
                    If Validita_inizio = AGRODATAINIZIO Then
                        Dr.Item("validita") += "..."
                    Else
                        Dr.Item("validita") += CStr(Validita_inizio)
                    End If
                    Dr.Item("validita") += " al "
                    If Validita_fine = AGRODATAFINE Then
                        Dr.Item("validita") += "..."
                    Else
                        Dr.Item("validita") += CStr(Validita_fine)
                    End If


                    'se sono state salvate le colture precedenti le carico
                    DtCodici = ObjImpiantiCodici.Leggi(CStr(Dr.Item("piva")),
                                                     CInt(Dr.Item("sa_cod")),
                                                     CInt(Dr.Item("Appezza")),
                                                     CInt(Dr.Item("Id_Reg")),
                                                     "", 0, "",
                                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "", objParametri_Server)


                    If Not DtCodici Is Nothing AndAlso DtCodici.Rows.Count > 0 Then

                        DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_1)
                        If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
                            Dr.Item("Coltura1") = DrCodici(0).Item("val_cod")
                        Else
                            Dr.Item("Coltura1") = ""
                        End If

                        DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_2)
                        If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
                            Dr.Item("Coltura2") = DrCodici(0).Item("val_cod")
                        Else
                            Dr.Item("Coltura2") = ""
                        End If

                        DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_3)
                        If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
                            Dr.Item("Coltura3") = DrCodici(0).Item("val_cod")
                        Else
                            Dr.Item("Coltura3") = ""
                        End If

                        DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_4)
                        If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
                            Dr.Item("Coltura4") = DrCodici(0).Item("val_cod")
                        Else
                            Dr.Item("Coltura4") = ""
                        End If

                    End If

                    'aggiungo la riga
                    Dt.Rows.Add(Dr)

                End If

            End If 'gruppo veg

        Next

        Session("Indici_Matrice") = Indici_Matrice


        Dim NomiChiavi(7) As String

        'Chiave della griglia
        NomiChiavi(0) = "Piva"
        NomiChiavi(1) = "Sa_Cod"
        NomiChiavi(2) = "Appezza"
        NomiChiavi(3) = "Id_Reg"
        NomiChiavi(4) = "Coltura1"
        NomiChiavi(5) = "Coltura2"
        NomiChiavi(6) = "Coltura3"
        NomiChiavi(7) = "Coltura4"

        GridView_Rotazione.DataSource = Dt
        GridView_Rotazione.DataKeyNames = NomiChiavi
        GridView_Rotazione.DataBind()

        GridView_Rotazione.Visible = True

        ''visualizzo eventuali colture precedenti salvate sul DB
        'For i = 0 To GridView_Rotazione.Rows.Count - 1

        '    If Not IsDBNull(Dt.Rows(i).Item("Coltura1")) Then
        '        CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura1"), TextBox).Text = GridView_Rotazione.DataKeys(i).Item(2)
        '    End If
        '    If Not IsDBNull(Dt.Rows(i).Item("Coltura2")) Then
        '        CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura2"), TextBox).Text = GridView_Rotazione.DataKeys(i).Item(3)
        '    End If
        '    If Not IsDBNull(Dt.Rows(i).Item("Coltura3")) Then
        '        CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura3"), TextBox).Text = GridView_Rotazione.DataKeys(i).Item(4)
        '    End If
        '    If Not IsDBNull(Dt.Rows(i).Item("Coltura4")) Then
        '        CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura4"), TextBox).Text = GridView_Rotazione.DataKeys(i).Item(5)
        '    End If

        'Next

        'For i = 0 To GridView_Rotazione.Items.Count - 1

        '    If Not IsDBNull(Dt.Rows(i).Item("Coltura1")) Then
        '        CType(DataGridRotazione.Items(i).FindControl("Txt_Coltura1"), TextBox).Text = Dt.Rows(i).Item("Coltura1")
        '    End If

        '    If Not IsDBNull(Dt.Rows(i).Item("Coltura2")) Then
        '        CType(DataGridRotazione.Items(i).FindControl("Txt_Coltura2"), TextBox).Text = Dt.Rows(i).Item("Coltura2")
        '    End If

        '    If Not IsDBNull(Dt.Rows(i).Item("Coltura3")) Then
        '        CType(DataGridRotazione.Items(i).FindControl("Txt_Coltura3"), TextBox).Text = Dt.Rows(i).Item("Coltura3")
        '    End If

        '    If Not IsDBNull(Dt.Rows(i).Item("Coltura4")) Then
        '        CType(DataGridRotazione.Items(i).FindControl("Txt_Coltura4"), TextBox).Text = Dt.Rows(i).Item("Coltura4")
        '    End If

        'Next



        For i = 0 To GridView_Rotazione.Rows.Count - 1

            If Not IsDBNull(Dt.Rows(i).Item("Coltura1")) Then
                CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura1"), TextBox).Text = Dt.Rows(i).Item("Coltura1")
            End If

            If Not IsDBNull(Dt.Rows(i).Item("Coltura2")) Then
                CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura2"), TextBox).Text = Dt.Rows(i).Item("Coltura2")
            End If

            If Not IsDBNull(Dt.Rows(i).Item("Coltura3")) Then
                CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura3"), TextBox).Text = Dt.Rows(i).Item("Coltura3")
            End If

            If Not IsDBNull(Dt.Rows(i).Item("Coltura4")) Then
                CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura4"), TextBox).Text = Dt.Rows(i).Item("Coltura4")
            End If

        Next

        ObjImpiantiCodici = Nothing




    End Sub


    '##############################################################################################################
    Private Sub Salva_Colture_Precedenti()

        Dim Dt As New DataTable
        ' Dim Dr As DataRow
        Dim Reg_Impianti_Codici_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R 'New Agro_Anagrafe_AD.Reg_Impianti_Codici_W
        Dim Reg_Impianti_Codici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W 'New Agro_Anagrafe_AD.Reg_Impianti_Codici_R

        Dim Rs As DataTable
        'Dim DtCodici As New DataTable
        Dim DrCodici() As DataRow

        Dim i As Integer
        Dim intDummy As Integer
        Dim strErr As String

        Dim Matrice_Variabili(0, 0) As String
        Dim Indici_Matrice() As Integer

        Dim Qs_Piva As String
        Dim Qs_Sa_Cod As String


        Matrice_Variabili = Session("Matrice_Variabili")
        Indici_Matrice = Session("Indici_Matrice")

        Qs_Piva = ViewState("Piva")
        Qs_Sa_Cod = ViewState("Sa_Cod")


        For i = 0 To GridView_Rotazione.Rows.Count - 1

            'se i codici esistono già li modifico altrimenti li inserisco
            Rs = Reg_Impianti_Codici_R.Leggi(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                             "", 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

            If Not IsNothing(Rs) AndAlso Rs.Rows.Count <> 0 Then

                DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_1)

                If DrCodici Is Nothing Or DrCodici.Length = 0 Then

                    intDummy = Reg_Impianti_Codici_W.Scrivi(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                             CInt(enum_CodiciAnagrafe.Coltura_Precedente_1),
                                                            CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura1"), TextBox).Text),
                                                            AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                Else

                    Reg_Impianti_Codici_W.Modifica(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                              CInt(enum_CodiciAnagrafe.Coltura_Precedente_1),
                                                   CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura1"), TextBox).Text),
                                                   CDate(Estremo_Validita_Inizio),
                                                   CDate(Estremo_Validita_Fine),
                                                   "", objParametri_Server)

                End If

                'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_2)
                DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_2)

                'se il codice esiste già lo modifico altrimenti lo scrivo
                If DrCodici Is Nothing Or DrCodici.Length = 0 Then

                    intDummy = Reg_Impianti_Codici_W.Scrivi(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                              CInt(enum_CodiciAnagrafe.Coltura_Precedente_2),
                                    CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura2"), TextBox).Text),
                                    AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)
                Else

                    Reg_Impianti_Codici_W.Modifica(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                               CInt(enum_CodiciAnagrafe.Coltura_Precedente_2),
                                                    CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura2"), TextBox).Text),
                                                    CDate(Estremo_Validita_Inizio),
                                                    CDate(Estremo_Validita_Fine),
                                                    "", objParametri_Server)

                End If

                'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_3)
                DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_3)

                'se il codice esiste già lo modifico altrimenti lo scrivo
                If DrCodici Is Nothing Or DrCodici.Length = 0 Then

                    intDummy = Reg_Impianti_Codici_W.Scrivi(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                               CInt(enum_CodiciAnagrafe.Coltura_Precedente_3),
                                    CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura3"), TextBox).Text),
                                    AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                Else

                    Reg_Impianti_Codici_W.Modifica(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                               CInt(enum_CodiciAnagrafe.Coltura_Precedente_3),
                                        CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura3"), TextBox).Text),
                                        CDate(Estremo_Validita_Inizio),
                                        CDate(Estremo_Validita_Fine),
                                        "", objParametri_Server)

                End If

                DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_4)

                'se il codice esiste già lo modifico altrimenti lo scrivo
                If DrCodici Is Nothing Or DrCodici.Length = 0 Then

                    intDummy = Reg_Impianti_Codici_W.Scrivi(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                              CInt(enum_CodiciAnagrafe.Coltura_Precedente_4),
                                                            CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura4"), TextBox).Text),
                                                            AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                Else

                    Reg_Impianti_Codici_W.Modifica(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                             CInt(enum_CodiciAnagrafe.Coltura_Precedente_4),
                                                    CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura4"), TextBox).Text),
                                                    CDate(Estremo_Validita_Inizio),
                                                    CDate(Estremo_Validita_Fine),
                                                    "", objParametri_Server)

                End If


            Else

                'se nn esiste nessun codice li creo
                intDummy = Reg_Impianti_Codici_W.Scrivi(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                             CInt(enum_CodiciAnagrafe.Coltura_Precedente_1),
                                                        CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura1"), TextBox).Text),
                                                        AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                intDummy = Reg_Impianti_Codici_W.Scrivi(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                             CInt(enum_CodiciAnagrafe.Coltura_Precedente_2),
                                                        CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura2"), TextBox).Text),
                                                        AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                intDummy = Reg_Impianti_Codici_W.Scrivi(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                             CInt(enum_CodiciAnagrafe.Coltura_Precedente_3),
                                                        CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura3"), TextBox).Text),
                                                        AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                intDummy = Reg_Impianti_Codici_W.Scrivi(CStr(GridView_Rotazione.DataKeys(i).Item(0)),
                                             CInt(GridView_Rotazione.DataKeys(i).Item(1)),
                                             GridView_Rotazione.DataKeys(i).Item(2),
                                             GridView_Rotazione.DataKeys(i).Item(3),
                                             CInt(enum_CodiciAnagrafe.Coltura_Precedente_4),
                                                        CStr(CType(GridView_Rotazione.Rows(i).FindControl("Txt_Coltura4"), TextBox).Text),
                                                        AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

            End If

        Next

    End Sub

    '##############################################################################################################
    Private Sub ImgBtn_SalvaDatiGLOBALGAP_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_SalvaDatiGLOBALGAP.Click
        GlobalGap_SalvaDati()
    End Sub
    Private Sub GlobalGap_SalvaDati()

        Imposta_Pannelli(enum_Pannello.Pannello_Filtro)

        Try
            '23/04/2018: salvataggio su db della revisione
            'Session("Revisione") = Me.TxtRevisione.Text

            Dim objCAC As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_W
            Dim tipo_codifica As Integer = 0

            objCAC.Cancella(enum_CodificaStampe.Eurep_Gap_Semplificata,
                        enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Revisione_Reportistica,
                        tipo_codifica,
                        "",
                         objParametri_Server)

            objCAC.Cancella(enum_CodificaStampe.Eurep_Gap_Multicentro,
                     enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Revisione_Reportistica,
                     tipo_codifica,
                     "",
                      objParametri_Server)

            If Me.TxtRevisione.Text <> "" Then

                objCAC.Scrivi(enum_CodificaStampe.Eurep_Gap_Semplificata,
                              Me.TxtRevisione.Text,
                              enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Revisione_Reportistica,
                              "Revisione Report Global-Gap Semplificata",
                              tipo_codifica,
                               0, 0, 0, "", "", "",
                               AGRODATAINIZIO,
                               AGRODATAFINE,
                               objParametri_Server)

                objCAC.Scrivi(enum_CodificaStampe.Eurep_Gap_Multicentro,
                          Me.TxtRevisione.Text,
                          enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Revisione_Reportistica,
                          "Revisione Report Global-Gap Multicentro",
                          tipo_codifica,
                           0, 0, 0, "", "", "",
                           AGRODATAINIZIO,
                           AGRODATAFINE,
                           objParametri_Server)

            End If

        Catch ex As Exception
            Log_Errori = ex.Message
        End Try

        RicaricaCalendari()

        '-----------------------------
        If Log_Errori <> "" Then

            Dim nomeFile As String = "LogErrori_filtroQDC_" & objParametri_Server.UtenteUsername & ".txt"

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                             "Stampe_QDC",
                                             nomeFile,
                                             objParametri_Server.UtenteUsername,
                                             "Selezione_SchedaCampagna.aspx",
                                             Log_Errori)
        End If

    End Sub

    '##############################################################################################################
    Private Sub ImgBtn_DatiGLOBALGAP_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_DatiGLOBALGAP.Click
        GlobalGap_VisualizzaDati()
    End Sub
    Private Sub GlobalGap_VisualizzaDati()

        Imposta_Pannelli(enum_Pannello.Pannello_GlobalGAP)

        Dim objCACInfoAgg As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
        Dim Tipo_Codifica As Integer = 0
        '23/04/2018: salvataggio su db della revisione
        'If Not Session("Revisione") Is Nothing Then
        '    Revisione = CStr(Session("Revisione"))
        'End If
        Me.TxtRevisione.Text = objCACInfoAgg.InfoAgg_Des_from_InfoAgg_Cod(enum_CodificaStampe.Eurep_Gap_Multicentro,
                                                                    enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Revisione_Reportistica,
                                                                    Tipo_Codifica,
                                                                     objParametri_Server)

        RicaricaCalendari()

    End Sub

    '###########################################################################
    Private Sub ImgBtnAnnulla2_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnAnnulla2.Click

        Imposta_Pannelli(enum_Pannello.Pannello_Filtro)
        RicaricaCalendari()

    End Sub

    '###########################################################################
    Private Sub ImgBtnAnnulla3_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtnAnnulla3.Click

        Imposta_Pannelli(enum_Pannello.Pannello_Filtro)

        RicaricaCalendari()

    End Sub

    '###########################################################################
    Private Sub ImgBtn_AnnataPrecedente_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_AnnataPrecedente.Click
        If Me.TxtValiditaInizio.Text <> "" Then
            Me.TxtValiditaInizio.Text = DateAdd(DateInterval.Year, -1, CDate(Me.TxtValiditaInizio.Text))
        End If
        If Me.TxtValiditaFine.Text <> "" Then
            Me.TxtValiditaFine.Text = DateAdd(DateInterval.Year, -1, CDate(Me.TxtValiditaFine.Text))
        End If

        RicaricaCalendari()

    End Sub

    '###########################################################################
    Private Sub ImgBtn_AnnataSuccessiva_Click(ByVal sender As System.Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_AnnataSuccessiva.Click
        If Me.TxtValiditaInizio.Text <> "" Then
            Me.TxtValiditaInizio.Text = DateAdd(DateInterval.Year, 1, CDate(Me.TxtValiditaInizio.Text))
        End If
        If Me.TxtValiditaFine.Text <> "" Then
            Me.TxtValiditaFine.Text = DateAdd(DateInterval.Year, 1, CDate(Me.TxtValiditaFine.Text))
        End If

        RicaricaCalendari()

    End Sub

    '###########################################################################
    Private Sub RicaricaCalendari()

        ' ricarico i calendari
        Dim Str As New StringBuilder
        Str.AppendLine("$(document).ready(function () {")
        Str.AppendLine("    $('.datepicker').datepicker({ ")
        Str.AppendLine("    dateFormat:  'dd/mm/yy',")
        Str.AppendLine("    disabled: false,")
        Str.AppendLine("    changeMonth: true,")
        Str.AppendLine("    changeYear: true")
        Str.AppendLine("});")
        Str.AppendLine("$.datepicker.regional['it'];")

        Str.AppendLine("     $('#dialogSezioniVuote').dialog({ ")
        Str.AppendLine("             autoOpen: false,")
        Str.AppendLine("             modal: true,")
        Str.AppendLine("             buttons: {")
        Str.AppendLine("                 'Aggiungi': function () {")
        Str.AppendLine("                 $(this).dialog('close');")
        Str.AppendLine("            $('#" & SalvaSezioniVuote.ClientID & "').click();")
        Str.AppendLine("             },")
        Str.AppendLine("             'Annulla': function () {")
        Str.AppendLine("                 $(this).dialog('close');")
        Str.AppendLine("                 $('#" & AnnullaSezioniVuote.ClientID & "').click();")
        Str.AppendLine("             }")
        Str.AppendLine("         }")
        Str.AppendLine("     });")

        Str.AppendLine("     $('#chkSelezionaTutteImprese').click(function () { SelezionaDeselezionaTutti(); });")

        If ViewState("Report") = enum_CodificaStampe.RegistroTrattamenti_Veneto Then
            Str.AppendLine("     $('#" & BtnStampeVuote.ClientID & "').hide(); ")
        End If

        Str.AppendLine("     $('#" & BtnStampeVuote.ClientID & "').click(function () {")
        Str.AppendLine("             $('#dialogSezioniVuote').dialog('open');")
        Str.AppendLine("             $('#dialogSezioniVuote').parent().appendTo($('form:first')); ")
        Str.AppendLine("     });")


        Str.AppendLine("});")


        ScriptManager.RegisterStartupScript(Pannello_Date, Pannello_Date.GetType(),
                                         String.Format("jQuery_{0}", Pannello_Date.ClientID), Str.ToString, True)
    End Sub



    '###########################################################################
    Private Sub Imgbtn_RotazioneColturale_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles Imgbtn_RotazioneColturale.Click

        Imposta_Pannelli(enum_Pannello.Pannello_ColturePrecedenti)

        Carica_Dgr_ColturePrecedenti()

        RicaricaCalendari()

    End Sub

    '###########################################################################
    Private Sub Imgbtn_AssegnaColture_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles Imgbtn_AssegnaColture.Click

        Salva_Colture_Precedenti()

        Imposta_Pannelli(enum_Pannello.Pannello_Filtro)

        RicaricaCalendari()

    End Sub

    '###########################################################################
    Private Sub ImgBtn_Stampa_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Stampa.Click

        Dim Report As Integer
        Dim Piva As String = ""

        Report = ViewState("Report")

        Session("ReportSelezionato") = Report

        Select Case Report

            Case enum_CodificaStampe.RegistroTrattamenti_Veneto
                Stampa_Veneto()

            Case enum_CodificaStampe.Registro_Trattamenti_Massivo,
                enum_CodificaStampe.Registro_Fertilizzazioni_Massivo

                Dim strXmlVariabilistampe As String = ""
                Dim StrNodiVariabili As String = ""
                Dim StrNodo As String = ""
                Dim objVS As New AgronicaCoreXML.XML_Stampe

                For i = 0 To DataGrid_Imprese.Rows.Count - 1
                    If CType(DataGrid_Imprese.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked Then
                        Piva = Me.DataGrid_Imprese.Rows(i).Cells(1).Text

                        Dim vVarStampe(0) As ElementoStampe
                        vVarStampe(0).Nome = "piva"
                        vVarStampe(0).Valore = Piva

                        StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
                        StrNodiVariabili = StrNodiVariabili & StrNodo

                    End If
                Next

                If StrNodiVariabili <> "" Then
                    strXmlVariabilistampe = Session("strXmlVariabilistampe")

                    'Carico la stringa xml in un nuovo documento
                    Dim XmlDoc As New System.Xml.XmlDocument
                    XmlDoc.LoadXml(strXmlVariabilistampe)
                    Dim XML_FiltroStampa As System.Xml.XmlElement = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

                    XML_FiltroStampa.InnerXml = StrNodiVariabili

                    Session("strXmlVariabilistampe") = XmlDoc.OuterXml

                End If

                Stampa()


            Case enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                enum_CodificaStampe.Registro_Fertilizzazioni,
                enum_CodificaStampe.SchedaCampagna_Multi_Lombardia,
                enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita,
                enum_CodificaStampe.RegistroAziendaleUnico

                Stampa()

            Case Else

                If Not (Not ViewState("UtilizzaImpiantiFiltrati") Is Nothing AndAlso ViewState("UtilizzaImpiantiFiltrati") = 1) Then
                    Dim SaCod As Integer = 0
                    Dim VegCod As Integer = 0
                    Dim CulCod As Integer = 0
                    Dim id_cod As Integer = 0

                    '(03/09/2016) fede introdotte combo centro, specie
                    Select Case Cmb_CentroAziendale.SelectedValue
                        Case "0" 'tutti
                            SaCod = 0
                        Case Else 'singolo
                            SaCod = CInt(Cmb_CentroAziendale.SelectedValue)
                    End Select

                    Select Case CInt(Cmb_Specie.SelectedValue)
                        Case 0 'terreno nudo
                        Case Is < 0 'dest uso
                            id_cod = Math.Abs(CInt(Cmb_Specie.SelectedValue))
                        Case Else 'specie
                            VegCod = CInt(Cmb_Specie.SelectedValue)
                    End Select

                    Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                    Dim Dt As DataTable
                    Dim HashImp As New Hashtable
                    Dim leggiAncheBloccati As Boolean = True
                    Dim strDataFiltro As String = String.Empty

                    Dim strXmlVariabilistampe As String = Session("strXmlVariabilistampe")
                    Dim StrNodiVariabili As String = ""
                    Dim StrNodo As String = ""
                    Dim objVS As New AgronicaCoreXML.XML_Stampe

                    If Me.rblStampa.SelectedItem.Value = 1 Then

                        If Not IsDate(Me.TxtValiditaInizio.Text) Then
                            Messaggi.AgroMsgBox("Impostare una data iniziale di riferimento per la stampa ...", Me.Master.Page)
                            Exit Sub
                        End If

                        If Not IsDate(Me.TxtValiditaFine.Text) Then
                            Messaggi.AgroMsgBox("Impostare una data finale di riferimento per la stampa ...", Me.Master.Page)
                            Exit Sub
                        End If

                        strDataFiltro = String.Format("  Imprese_Progetti.Validita_Inizio <= {0} ", Agro_SQL_SaveDate(CDate(TxtValiditaFine.Text)))
                        strDataFiltro = String.Format("{0}AND Imprese_Progetti.Validita_Fine >= {1} ", strDataFiltro, Agro_SQL_SaveDate(CDate(TxtValiditaInizio.Text)))

                    Else
                        'altrimenti passo alla scheda di campagna solo la data scelta
                        If Not IsDate(Me.TxtStampa.Text) Then
                            Messaggi.AgroMsgBox("Impostare una data iniziale di riferimento per la stampa ...", Me.Master.Page)
                            Exit Sub
                        Else
                            strDataFiltro = String.Format("  Imprese_Progetti.Validita_Inizio <= {0} ", Agro_SQL_SaveDate(CDate(TxtStampa.Text)))
                            strDataFiltro = String.Format("{0}AND Imprese_Progetti.Validita_Fine >= {1} ", strDataFiltro, Agro_SQL_SaveDate(CDate(TxtStampa.Text)))
                        End If

                    End If

                    'filtro sulla destinazione d'uso
                    If id_cod <> 0 Then
                        strDataFiltro &= "    AND EXISTS ( " &
                                        "   Select 1 " &
                                        "   from  Reg_Impianti_Codici  " &
                                        "   WHERE   PIVA = Reg_Impianti.PIVA " &
                                        "   AND sa_cod = Reg_Impianti.sa_cod  " &
                                        "   AND appezza = Reg_Impianti.appezza  " &
                                        "  AND Id_Reg = Reg_Impianti.id_reg  " &
                                  "         and id_cod = " & Agro_SQL_SaveNum(id_cod) & " ) "
                    End If

                    Dt = objImpianti.Leggi_Impianti_xAgenda2(False,
                                                    Session("Piva"),
                                                    SaCod,
                                                    VegCod,
                                                    CulCod,
                                                    "",
                                                    0,
                                                    -1,
                                                    strDataFiltro,
                                                    " Cul_Des, App_Nome, Progetto ",
                                                    objParametri_Server, leggiAncheBloccati)

                    If Dt.Rows.Count = 0 Then
                        Messaggi.AgroMsgBox("Nessun Impianto Selezionato Per La Stampa", Me.Master.Page)
                        Exit Sub
                    End If
                    For i = 0 To Dt.Rows.Count - 1
                        If Not HashImp.ContainsKey(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg")) Then
                            HashImp.Add(Dt.Rows(i).Item("sa_cod") & "|" & Dt.Rows(i).Item("appezza") & "|" & Dt.Rows(i).Item("id_reg"), "")
                            Dim vVarStampe(5) As ElementoStampe
                            vVarStampe(0).Nome = "piva"
                            vVarStampe(0).Valore = Dt.Rows(i).Item("piva")
                            vVarStampe(1).Nome = "sa_cod"
                            vVarStampe(1).Valore = Dt.Rows(i).Item("sa_cod")
                            vVarStampe(2).Nome = "appezza"
                            vVarStampe(2).Valore = Dt.Rows(i).Item("appezza")
                            vVarStampe(3).Nome = "id_reg"
                            vVarStampe(3).Valore = Dt.Rows(i).Item("id_reg")
                            vVarStampe(4).Nome = "veg_cod"
                            vVarStampe(4).Valore = Dt.Rows(i).Item("veg_cod")
                            vVarStampe(5).Nome = "id_cod"
                            vVarStampe(5).Valore = If(IsNothing(id_cod), "", id_cod)
                            StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
                            StrNodiVariabili = StrNodiVariabili & StrNodo
                        End If
                    Next

                    'Carico la stringa xml in un nuovo documento
                    Dim XmlDoc As New System.Xml.XmlDocument
                    XmlDoc.LoadXml(strXmlVariabilistampe)
                    Dim XML_FiltroStampa As System.Xml.XmlElement = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

                    XML_FiltroStampa.InnerXml = StrNodiVariabili

                    Dim attrPiva As System.Xml.XmlAttribute = XmlDoc.CreateAttribute("piva")
                    attrPiva.Value = Session("Piva")
                    XML_FiltroStampa.Attributes.Append(attrPiva)

                    Dim attrsaCod As System.Xml.XmlAttribute = XmlDoc.CreateAttribute("sa_cod")
                    attrsaCod.Value = SaCod
                    XML_FiltroStampa.Attributes.Append(attrsaCod)

                    Session("strXmlVariabilistampe") = XmlDoc.OuterXml
                End If


                Stampa()

        End Select

        'If Report <> enum_CodificaStampe.RegistroTrattamenti_Veneto Then
        '    Stampa()
        'Else
        '    Stampa_Veneto()
        'End If

    End Sub


    '###########################################################################
    Private Sub Stampa()

        Dim TargetURL As String
        Dim DataInizio As Date
        Dim DataFine As Date
        Dim DataStampa As Date
        Dim str_DataInizio As String
        Dim str_DataFine As String
        Dim str_DataStampa As String
        Dim strSezioni As String = ""
        Dim Num_Sezioni As Integer
        Dim Report As Integer

        Report = ViewState("Report")

        Dim flag_organismoIntest As Boolean = Me.Chk_IntestazioneOrgref.Checked

        '----- Verifico i dati

        'Se seleziono un intervallo passo alla scheda di campagna le due date scelte
        If Me.rblStampa.SelectedItem.Value = 1 Then
            If Me.TxtValiditaInizio.Text = "" Then
                Messaggi.AgroMsgBox("Impostare una data iniziale di riferimento per la stampa ...", Me.Master.Page)
                Exit Sub
            Else
                DataInizio = CDate(Me.TxtValiditaInizio.Text)
            End If

            If Me.TxtValiditaFine.Text = "" Then
                Messaggi.AgroMsgBox("Impostare una data finale di riferimento per la stampa ...", Me.Master.Page)
                Exit Sub
            Else
                DataFine = CDate(Me.TxtValiditaFine.Text)
            End If

            str_DataInizio = Format(DataInizio, "dd/MM/yyyy")
            str_DataFine = Format(DataFine, "dd/MM/yyyy")
            str_DataStampa = ""

        Else
            'altrimenti passo alla scheda di campagna solo la data scelta
            If Me.TxtStampa.Text = "" Then
                Messaggi.AgroMsgBox("Impostare una data iniziale di riferimento per la stampa ...", Me.Master.Page)
                Exit Sub
            Else
                DataStampa = CDate(Me.TxtStampa.Text)
            End If

            str_DataInizio = ""
            str_DataFine = ""
            str_DataStampa = Format(DataStampa, "dd/MM/yyyy")

        End If

        Dim flag_reg_condizionalita As Boolean = False
        Dim flag_reg_psr As Boolean = False
        Dim flag_reg_misura10 As Boolean = False

        If Me.DivRegolamenti.Visible = True Then
            flag_reg_condizionalita = Me.Chk_RegCondizionalita.Checked
            flag_reg_psr = Me.Chk_RegPSR.Checked
            flag_reg_misura10 = Me.Chk_RegMisura10.Checked
        End If

        Dim tipo_ordinamento As Integer = 0
        If Me.DivOrdinamento.Visible = True Then
            tipo_ordinamento = Me.Rbl_Ordinamento.SelectedValue
        End If

        Select Case Report
            Case enum_CodificaStampe.SchedaColturale_Biologico

                TargetURL = "../Biologico/SchedaColturale/SchedaColturaleBiologico.aspx" &
                    "?dI=" &
                    Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
                    "&dF=" &
                    Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
                    "&dG=" &
                    Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
                    "&arr=" &
                    Stringa_Codifica(Rbl_Arrotondamento.SelectedValue, AgroKey_EncoderDecoder, Server) &
                    "&stDef=" &
                    Stringa_Codifica(RblStampaProva.SelectedValue, AgroKey_EncoderDecoder, Server)

            Case Else

                TargetURL = "SchedaCampagna.aspx" &
                                "?dI=" &
                                Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
                                "&dF=" &
                                Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
                                "&dG=" &
                                Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
                                "&arr=" &
                                Stringa_Codifica(Rbl_Arrotondamento.SelectedValue, AgroKey_EncoderDecoder, Server) &
                                "&stDef=" &
                                Stringa_Codifica(RblStampaProva.SelectedValue, AgroKey_EncoderDecoder, Server) &
                                "&for=" &
                                Stringa_Codifica(flag_organismoIntest, AgroKey_EncoderDecoder, Server) &
                                "&frc=" &
                                Stringa_Codifica(flag_reg_condizionalita, AgroKey_EncoderDecoder, Server) &
                                "&frpsr=" &
                                Stringa_Codifica(flag_reg_psr, AgroKey_EncoderDecoder, Server) &
                                "&frmis=" &
                                Stringa_Codifica(flag_reg_misura10, AgroKey_EncoderDecoder, Server) &
                                "&ord=" &
                                Stringa_Codifica(tipo_ordinamento, AgroKey_EncoderDecoder, Server) &
                                "&chkannoimp=" &
                                Stringa_Codifica(Me.Chk_StampaAnnoImpiantoPluriennali.Checked, AgroKey_EncoderDecoder, Server)

        End Select


        'If Report = enum_CodificaStampe.SchedaColturale_Biologico Then

        '    TargetURL = "../Biologico/SchedaColturale/SchedaColturaleBiologico.aspx" &
        '                "?dI=" &
        '                Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
        '                "&dF=" &
        '                Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
        '                "&dG=" &
        '                Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
        '                "&arr=" &
        '                Stringa_Codifica(Rbl_Arrotondamento.SelectedValue, AgroKey_EncoderDecoder, Server) &
        '                "&stDef=" &
        '                Stringa_Codifica(RblStampaProva.SelectedValue, AgroKey_EncoderDecoder, Server)

        'Else

        '    TargetURL = "SchedaCampagna.aspx" &
        '                    "?dI=" &
        '                    Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
        '                    "&dF=" &
        '                    Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
        '                    "&dG=" &
        '                    Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
        '                    "&arr=" &
        '                    Stringa_Codifica(Rbl_Arrotondamento.SelectedValue, AgroKey_EncoderDecoder, Server) &
        '                    "&stDef=" &
        '                    Stringa_Codifica(RblStampaProva.SelectedValue, AgroKey_EncoderDecoder, Server) &
        '                    "&for=" &
        '                    Stringa_Codifica(flag_organismoIntest, AgroKey_EncoderDecoder, Server) &
        '                    "&frc=" &
        '                    Stringa_Codifica(flag_reg_condizionalita, AgroKey_EncoderDecoder, Server) &
        '                    "&frpsr=" &
        '                    Stringa_Codifica(flag_reg_psr, AgroKey_EncoderDecoder, Server) &
        '                    "&frmis=" &
        '                    Stringa_Codifica(flag_reg_misura10, AgroKey_EncoderDecoder, Server) &
        '                    "&ord=" &
        '                    Stringa_Codifica(tipo_ordinamento, AgroKey_EncoderDecoder, Server) &
        '                    "&chkannoimp=" &
        '                    Stringa_Codifica(Me.Chk_StampaAnnoImpiantoPluriennali.Checked, AgroKey_EncoderDecoder, Server)

        'End If


        SEZIONI_SalvaSezioniSeleziontexStampa(strSezioni, Num_Sezioni, "")

        'controllo che sia selezionata almeno una sezione
        If Num_Sezioni = 0 Then
            Messaggi.AgroMsgBox("Selezionare almeno una Sezione!", Me.Master.Page)
            Exit Sub
        End If

        Session("AvversitaQta") = Me.Chk_Avversita_Qta.Checked

        If Me.Chk_Data_UltimaRaccolta.Checked = True Then
            Session("DataUltimaRaccolta") = True
        Else
            Session("DataUltimaRaccolta") = False
        End If

        If Me.Chk_Tutte_Raccolte.Checked = True Then
            Session("TutteRaccolte") = True
        Else
            Session("TutteRaccolte") = False
        End If

        If Me.Chk_Qta_Raccolte.Checked = True Then
            Session("QtaRaccolte") = True
        Else
            Session("QtaRaccolte") = False
        End If

        If Me.Chk_RaggruppaXCampo.Checked = True Then
            Session("RaggruppaXCampo") = True
        Else
            Session("RaggruppaXCampo") = False
        End If

        If Me.Chk_VisualizzaTipologieVarietali.Checked = True Then
            Session("VisualizzaTipologieVarietali") = True
        Else
            Session("VisualizzaTipologieVarietali") = False
        End If


        If Me.Chk_VisualizzaCapitolatoPrivato.Checked = True Then
            Session("CapitolatoPrivato") = True
        Else
            Session("CapitolatoPrivato") = False
        End If

        If Me.Chk_VisualizzaFinalita.Checked = True Then
            Session("VisualizzaFinalita") = True
        Else
            Session("VisualizzaFinalita") = False
        End If

        If Me.Chk_VisualizzaAcquaHa.Checked = True Then
            Session("VisualizzaAcquaHa") = True
        Else
            Session("VisualizzaAcquaHa") = False
        End If

        If Me.Chk_MostraValoriSignificativiNeiRilievi.Checked = True Then
            Session("MostraValoriSignificativiNeiRilievi") = True
        Else
            Session("MostraValoriSignificativiNeiRilievi") = False
        End If

        If Me.Chk_MostraDataOdiernaStampa.Checked = True Then
            Session("MostraDataOdiernaStampa") = True
        Else
            Session("MostraDataOdiernaStampa") = False
        End If

        If Me.Chk_MostraFirmaODC.Checked = True Then
            Session("MostraFirmaODC") = True
        Else
            Session("MostraFirmaODC") = False
        End If

        If Me.Chk_Priorita_ColturePrecedenti.Checked = True Then
            Session("Priorita_ColturePrecedenti") = True
        Else
            Session("Priorita_ColturePrecedenti") = False
        End If

        Session("Regione_Selezionata") = ""
        Session("Regione_Copertina") = ""
        If Me.Chk_LogoRegione.Checked = True Then
            If Me.Cmb_Regioni.SelectedIndex >= 0 Then
                Session("Regione_Selezionata") = Me.Cmb_Regioni.SelectedValue.ToString
                Session("Regione_Copertina") = Me.Cmb_Regioni.SelectedValue.ToString
            End If
        End If

        Session("sezioniVuote") = ViewState("SezioniVuote") 'Txt_StampeVuote.Text

        Session("TempoRientro") = Me.Txt_TempoRientro.Text

        Session("TipoSuperficie") = Rbl_Superfici.SelectedValue

        If RigaFascicoli.Visible = True AndAlso Cmb_Fascicoli.SelectedValue <> "0" Then
            Session("Fascicolo") = Cmb_Fascicoli.SelectedValue
        End If

        If Me.CheckTitoloGlobal.Checked = True Then
            Session("TitoloGlobal") = True
        Else
            Session("TitoloGlobal") = False
        End If

        If Me.Chk_MostraDataOdiernaStampa.Checked = True Then
            Session("MostraDataOdiernaStampa") = True
        Else
            Session("MostraDataOdiernaStampa") = False
        End If


        Response.Redirect(TargetURL)


    End Sub


    '###########################################################################
    Private Sub Stampa_Veneto()


        Dim DataInizio As Date
        Dim DataFine As Date
        Dim DataStampa As Date
        Dim str_DataInizio As String
        Dim str_DataFine As String
        Dim str_DataStampa As String
        Dim i As Integer
        Dim strSezioni As String = ""
        Dim Scheda As String
        Dim Num_Sezioni As Integer

        '----- Verifico i dati

        'Se seleziono un intervallo passo alla scheda di campagna le due date scelte
        If Me.rblStampa.SelectedItem.Value = 1 Then

            If Me.TxtValiditaInizio.Text = "" Then
                Messaggi.AgroMsgBox("Impostare una data iniziale di riferimento per la stampa ...", Me.Master.Page)
                Exit Sub
            Else
                DataInizio = CDate(Me.TxtValiditaInizio.Text)
            End If

            If Me.TxtValiditaFine.Text = "" Then
                Messaggi.AgroMsgBox("Impostare una data finale di riferimento per la stampa ...", Me.Master.Page)
                Exit Sub
            Else
                DataFine = CDate(Me.TxtValiditaFine.Text)
            End If

            str_DataInizio = Format(DataInizio, "dd/MM/yyyy")
            str_DataFine = Format(DataFine, "dd/MM/yyyy")
            str_DataStampa = ""

            'altrimenti passo alla scheda di campagna solo la data scelta
        Else

            If Me.TxtStampa.Text = "" Then
                Messaggi.AgroMsgBox("Impostare una data iniziale di riferimento per la stampa ...", Me.Master.Page)
                Exit Sub
            Else
                DataStampa = CDate(Me.TxtStampa.Text)
            End If

            str_DataInizio = ""
            str_DataFine = ""
            str_DataStampa = Format(DataStampa, "dd/MM/yyyy")

        End If


        '----- Preparo la stringa di apertura di una nuova pagina

        Dim TargetURL As String
        Dim QueryString As String

        Session("Regione_Selezionata") = ""
        If Me.Chk_LogoRegione.Checked = True Then
            If Me.Cmb_Regioni.SelectedIndex >= 0 Then
                Session("Regione_Selezionata") = Me.Cmb_Regioni.SelectedValue.ToString
            End If
        End If

        If Me.Chk_VisualizzaFinalita.Checked = True Then
            Session("VisualizzaFinalita") = True
        Else
            Session("VisualizzaFinalita") = False
        End If

        For i = 0 To Me.RBL_SezioniVeneto.Items.Count - 1

            If Me.RBL_SezioniVeneto.Items(i).Selected = True Then

                Scheda = RBL_SezioniVeneto.Items(i).Value

                SEZIONI_SalvaSezioniSeleziontexStampa(strSezioni, Num_Sezioni, Scheda)

                Select Case Scheda

                    Case "a",
                         "b",
                         "c",
                         "d"

                        TargetURL = "SchedaCampagna.aspx"
                        QueryString = "?dI=" &
                                        Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
                                        "&dF=" &
                                        Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
                                        "&dG=" &
                                        Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
                                        "&arr=" &
                                        Stringa_Codifica(Me.Rbl_Arrotondamento.SelectedValue, AgroKey_EncoderDecoder, Server) &
                                        "&stDef=" &
                                        Stringa_Codifica(RblStampaProva.SelectedValue, AgroKey_EncoderDecoder, Server)

                        '20/06/2018 vedi SalvaSezioniSeleziontexStampa
                        '"&sez=" & _
                        ' Stringa_Codifica(Scheda, AgroKey_EncoderDecoder, Server) & _

                        Session("sezioniVuote") = ViewState("SezioniVuote")     'Txt_StampeVuote.Text

                        Session("TempoRientro") = Me.Txt_TempoRientro.Text

                        Page_NewWindow_2010(Page, TargetURL, QueryString, , , , , , , , , , True)

                        RicaricaCalendari()

                    Case "e"

                        TargetURL = "SchedaTrattamentiTerzista.aspx"
                        QueryString = "?p=" &
                                        Stringa_Codifica(CStr(Session("Piva")), AgroKey_EncoderDecoder, Server) +
                                        "&dI=" &
                                        Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
                                        "&dF=" &
                                        Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
                                        "&dG=" &
                                        Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
                                        "&stDef=" &
                                        Stringa_Codifica(RblStampaProva.SelectedValue, AgroKey_EncoderDecoder, Server)

                        Page_NewWindow_2010(Page, TargetURL, QueryString, , , , , , , , , , True)

                        RicaricaCalendari()

                End Select

                Num_Sezioni += 1

            End If

        Next

        'controllo che sia selezionata almeno una sezione
        If Num_Sezioni = 0 Then
            Messaggi.AgroMsgBox("Selezionare almeno una Sezione!", Me.Master.Page)
            Exit Sub
        End If

    End Sub

    ' ##########################################################################################################################
    Private Sub ImgBtn_Elenco_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_Elenco.Click

        Dim TargetURL As String = "../ElencoReport.aspx"
        Dim QueryString As String

        Page_NewWindow_2010(Page, TargetURL, QueryString, , 500, 800, , , , , , , True)

        RicaricaCalendari()

    End Sub


    ' ##################################################################################################
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Session("Matrice_Variabili") = Nothing

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      window.close() ")
        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType(),
                                  String.Format("jQuery_{0}", Me.Page.ClientID), strJS1.ToString, True)

    End Sub


#Region "Sezione Vuote"

    ' ##################################################################################################
    Protected Sub SalvaSezioniVuote_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SalvaSezioniVuote.Click

        SezioniVuote_Salva()

    End Sub

    Private Sub SezioniVuote_Salva()

        Dim str As String = ""
        Dim i As Integer = 0
        For i = 0 To ListaSezioniVuote.Items.Count - 1
            If ListaSezioniVuote.Items(i).Selected = True Then
                str = str & IIf(str <> "", "|", "") & ListaSezioniVuote.Items(i).Value
            End If
        Next

        ViewState("SezioniVuote") = str

    End Sub

    Protected Sub AnnullaSezioniVuote_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AnnullaSezioniVuote.Click

        SezioniVuote_AggiornaVisibilita()

    End Sub

    Private Sub SezioniVuote_Impostazioni()

        Dim Report As Integer
        Report = ViewState("Report")

        ' rimuovo i check che non devo visualizzare in base al report
        Select Case Report

            Case enum_CodificaStampe.SchedaCampagna_2078

                ListaSezioniVuote.Items.RemoveAt(11)    ' n
                ListaSezioniVuote.Items.RemoveAt(6)     ' g
                ListaSezioniVuote.Items.RemoveAt(3)     ' d
                ListaSezioniVuote.Items.RemoveAt(0)     ' a

            Case enum_CodificaStampe.RegistroTrattamenti

                ListaSezioniVuote.Items.RemoveAt(11)    ' n
                ListaSezioniVuote.Items.RemoveAt(10)    ' m
                ListaSezioniVuote.Items.RemoveAt(9)     ' l
                ListaSezioniVuote.Items.RemoveAt(8)     ' i
                ListaSezioniVuote.Items.RemoveAt(7)     ' h
                ListaSezioniVuote.Items.RemoveAt(6)     ' g
                ListaSezioniVuote.Items.RemoveAt(5)     ' f
                ListaSezioniVuote.Items.RemoveAt(4)     ' e
                ListaSezioniVuote.Items.RemoveAt(3)     ' d
                ListaSezioniVuote.Items.RemoveAt(1)     ' b
                ListaSezioniVuote.Items.RemoveAt(0)     ' a

            Case enum_CodificaStampe.SchedaRegistrazione,
                 enum_CodificaStampe.SchedaColturale_Biologico

                ListaSezioniVuote.Items.RemoveAt(11)    ' n
                ListaSezioniVuote.Items.RemoveAt(6)     ' g
                ListaSezioniVuote.Items.RemoveAt(1)     ' b

            Case enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                enum_CodificaStampe.SchedaRegistrazione_Semplificata

                ListaSezioniVuote.Items.RemoveAt(3)     ' d

            Case enum_CodificaStampe.SchedaCampagna_ConserveItalia

                ListaSezioniVuote.Items.RemoveAt(11)    ' n
                ListaSezioniVuote.Items.RemoveAt(6)     ' g
                ListaSezioniVuote.Items.RemoveAt(3)     ' d

            Case enum_CodificaStampe.Eurep_Gap_Semplificata

                ListaSezioniVuote.Items.RemoveAt(6)     ' g
                ListaSezioniVuote.Items.RemoveAt(3)     ' d

            Case enum_CodificaStampe.RegistroTrattamenti_Semplificata

                ListaSezioniVuote.Items.RemoveAt(11)    ' n
                ListaSezioniVuote.Items.RemoveAt(10)    ' m
                ListaSezioniVuote.Items.RemoveAt(9)     ' l
                ListaSezioniVuote.Items.RemoveAt(8)     ' i
                ListaSezioniVuote.Items.RemoveAt(7)     ' h
                ListaSezioniVuote.Items.RemoveAt(6)     ' g
                ListaSezioniVuote.Items.RemoveAt(5)     ' f
                ListaSezioniVuote.Items.RemoveAt(4)     ' e
                ListaSezioniVuote.Items.RemoveAt(3)     ' d
                ListaSezioniVuote.Items.RemoveAt(1)     ' b

            Case enum_CodificaStampe.Eurep_Gap

                ListaSezioniVuote.Items.RemoveAt(11)    ' n
                ListaSezioniVuote.Items.RemoveAt(10)    ' m
                ListaSezioniVuote.Items.RemoveAt(9)     ' l
                ListaSezioniVuote.Items.RemoveAt(8)     ' i
                ListaSezioniVuote.Items.RemoveAt(7)     ' h
                ListaSezioniVuote.Items.RemoveAt(6)     ' g
                ListaSezioniVuote.Items.RemoveAt(5)     ' f
                ListaSezioniVuote.Items.RemoveAt(4)     ' e
                ListaSezioniVuote.Items.RemoveAt(3)     ' d
                ListaSezioniVuote.Items.RemoveAt(0)     ' a

            Case 4, 16

                'nn ancora gestito

            Case enum_CodificaStampe.SchedaCampagna_Pizzoli

                ListaSezioniVuote.Items.RemoveAt(6)     ' g
                ListaSezioniVuote.Items.RemoveAt(3)     ' d

        End Select

        SezioniVuote_AggiornaVisibilita()

    End Sub

    Private Sub SezioniVuote_AggiornaVisibilita()

        Dim str As String = ""
        str = ViewState("SezioniVuote")

        Dim Lista() As String

        If Not IsNothing(str) AndAlso str <> "" Then
            Lista = str.Split("|")
        End If

        Dim i As Integer
        For i = 0 To ListaSezioniVuote.Items.Count - 1
            ListaSezioniVuote.Items(i).Selected = False
        Next

        If Not IsNothing(Lista) Then
            For i = 0 To Lista.Length - 1

                ListaSezioniVuote.Items.FindByValue(Lista(i)).Selected = True

            Next
        End If


    End Sub

#End Region






    Protected Sub ImgBtn_VerificaConformita_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_VerificaConformita.Click

        Dim DataInizio As Date
        Dim DataFine As Date

        If Me.TxtValiditaInizio.Text = "" Then
            Messaggi.AgroMsgBox("Impostare una data iniziale di riferimento per il controllo ...", Me.Master.Page)
            Exit Sub
        Else
            DataInizio = CDate(Me.TxtValiditaInizio.Text)
        End If

        If Me.TxtValiditaFine.Text = "" Then
            Messaggi.AgroMsgBox("Impostare una data finale di riferimento per il controllo ...", Me.Master.Page)
            Exit Sub
        Else
            DataFine = CDate(Me.TxtValiditaFine.Text)
        End If

        Dim objVerificaDisciplinare2010 As New AgronicaCoreGestioneRichieste.ParametriVerificaDisciplinare2010
        objVerificaDisciplinare2010.Leggi()
        objVerificaDisciplinare2010.Validita_Inizio = DataInizio
        objVerificaDisciplinare2010.Validita_Fine = DataFine

        Dim strOpen = AgronicaCoreGestioneRichieste.RedirectGestione.ApriPopUp_SitoAgenda_PassandoDirettamente_ParametriVerificaDisciplinare2010_senza_blocco_script(
                    Enum_SiteRedirector.Sito_AgronicaStampe_2010, objVerificaDisciplinare2010)

        Page_NewWindow_2010(Page, strOpen, "", , 500, 800, , , , , , , True)

        RicaricaCalendari()

    End Sub

    Private Sub SEZIONI_AggiungiSezioniConPermessi(ByVal report As enum_CodificaStampe)

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim permesso_Pratiche_Ecologiche As Boolean = False
        permesso_Pratiche_Ecologiche = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.CheckList_Pratiche_Ecologiche_APOT,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)
        If permesso_Pratiche_Ecologiche AndAlso report = enum_CodificaStampe.SchedaCampagna_ProvAut_Trento Then
            Dim checkPraticheEco As ListItem = New ListItem("Pratiche Ecologiche", "u")
            checkPraticheEco.Selected = True
            CBL_Sezioni.Items.Add(checkPraticheEco)
        End If

        Dim permesso_Formazione As Boolean = False
        permesso_Formazione = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.CheckList_Formazione,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)
        If permesso_Formazione AndAlso report = enum_CodificaStampe.SchedaCampagna_ProvAut_Trento Then
            Dim CheckList_Formazione As ListItem = New ListItem("Formazione", "w")
            CheckList_Formazione.Selected = True
            CBL_Sezioni.Items.Add(CheckList_Formazione)
        End If

        Dim permesso_Gestione_Rifiuti As Boolean = False
        permesso_Gestione_Rifiuti = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.Gestione_Rifiuti,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)
        If permesso_Gestione_Rifiuti AndAlso report = enum_CodificaStampe.SchedaCampagna_ProvAut_Trento Then
            Dim Gestione_Rifiuti As ListItem = New ListItem("Gestione Rifiuti", "x")
            Gestione_Rifiuti.Selected = True
            CBL_Sezioni.Items.Add(Gestione_Rifiuti)
        End If

        ' SEZIONE CONFORMITA
        Dim permesso_conformita As Boolean = False
        permesso_conformita = objPermessi.Controlla_Permessi_Utente(
                                    Session("ASG_Utente_Username"),
                                    Session("ASG_IdServizio"),
                                    enum_Security_Attivita.VerificaConformita_Gestione,
                                    enum_Security_Operazione.Lettura,
                                    Date.Now,
                                    "",
                                    objParametri_Utenti)
        If permesso_conformita And report <> enum_CodificaStampe.RegistroTrattamenti_Veneto Then
            Dim checkConformita As ListItem = New ListItem("Verifiche Conformita", "v")
            checkConformita.Selected = False
            CBL_Sezioni.Items.Add(checkConformita)
        End If

    End Sub

    Private Sub SEZIONI_AbilitaDisabilita(Report As enum_CodificaStampe)

        Dim elencoReportAbilitati() As String

        Select Case Report

            Case enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                   enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                    enum_CodificaStampe.SchedaCampagna_ConserveItalia,
                    enum_CodificaStampe.SchedaColturale_Biologico

                'CAMPAGNA CONSERVE E BIO
                elencoReportAbilitati = ElencoReport_SchedaCampagna_Semplificata
                'SCHEDA CAMPAGNA SEMPLIFICATA

            Case enum_CodificaStampe.SchedaCampagna_Multicentro,
                          enum_CodificaStampe.SchedaInterventiAgronomici

                'SCHEDA CAMPAGNA MULTI
                elencoReportAbilitati = ElencoReport_SchedaCampagna_Multicentro

                'Case enum_CodificaStampe.SchedaInterventiAgronomici

                '    elencoReportAbilitati = ElencoReport_SchedaInterventiAgronomici

            Case enum_CodificaStampe.RegistroTrattamenti_Semplificata,
                enum_CodificaStampe.Registro_Trattamenti_Massivo ', _
                'enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita

                'RESGISTRO TRATTAMENTI
                elencoReportAbilitati = ElencoReport_RegistroTrattamenti

            Case enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita
                elencoReportAbilitati = ElencoReport_RegistroTrattamenti_VenetoUnico

            Case enum_CodificaStampe.SchedaCampagna_Multi_Lombardia

                'MULTI_LOMBARDIA - VENETO 
                elencoReportAbilitati = ElencoReport_SchedaCampagnaMulti_Lombardia

            Case enum_CodificaStampe.RegistroAziendaleUnico

                elencoReportAbilitati = ElencoReport_RegistroUnicoAziendale

            Case enum_CodificaStampe.Registro_Fertilizzazioni,
                enum_CodificaStampe.Registro_Fertilizzazioni_Massivo

                'RESGISTRO FERTILIZZAZIONI
                elencoReportAbilitati = ElencoReport_RegistroFertilizzazioni

            Case enum_CodificaStampe.RegistroTrattamenti_Veneto

                'VENETO

                elencoReportAbilitati = Nothing

            Case enum_CodificaStampe.Eurep_Gap_Semplificata,
                 enum_CodificaStampe.SchedaCampagna_Pizzoli

                'GLOBAL SINGOLO
                elencoReportAbilitati = ElencoReport_Eurep_Gap_Semplificata

            Case enum_CodificaStampe.Eurep_Gap_Multicentro

                'GLOBAL MULI
                elencoReportAbilitati = ElencoReport_Eurep_Gap_Multicentro

            Case enum_CodificaStampe.SchedaCampagna_ProvAut_Trento

                'TRENTO
                elencoReportAbilitati = ElencoReport_Prov_Aut_Trento

            Case Else

                elencoReportAbilitati = Nothing

        End Select

        If Not IsNothing(elencoReportAbilitati) Then
            For i = 0 To CBL_Sezioni.Items.Count - 1
                CBL_Sezioni.Items(i).Enabled = False
            Next

            For i = 0 To elencoReportAbilitati.Length - 1

                Try
                    CBL_Sezioni.Items.FindByValue(elencoReportAbilitati(i)).Enabled = True
                Catch ex As Exception
                    Console.Write(ex.Message)
                End Try
            Next
        End If

    End Sub

    Protected Sub ImgBtn_StampaMagazzino_Click(sender As Object, e As System.Web.UI.ImageClickEventArgs) Handles ImgBtn_StampaMagazzino.Click

        Const PaginaLinkStampaSchedaGiacenzeMagazzino As String = "../Magazzino/Giacenze/SchedaGiacenzeMagazzino_2.aspx"
        Const PaginaLinkStampaSchedaMovimentiMagazzino As String = "../Magazzino/Movimenti/SchedaMovimentiMagazzino.aspx"
        Const PaginaLinkStampaSchedaFertilizzantiMagazzino As String = "../Magazzino/Fertilizzanti/SchedaFertilizzantiMagazzino.aspx"
        Const PaginaLinkStampaSchedaProdottiFitosanitariMagazzino As String = "../Magazzino/ProdottiFitosanitari/SchedaProdottiFitosanitariMagazzino.aspx"

        Dim TargetURL As String = ""
        Dim Data_Stampa, Data_Inizio, Data_Fine As Date
        Dim Elem_Cod As Integer = 0
        Dim Pro_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0
        Dim Tipo_Arrotondamento As Integer = Rbl_Arrotondamento.SelectedValue
        Dim Str_elem_cod As String = ""
        Dim FlagVisualizzaComposizione As Integer = 0 '0=NON Stampa la composizione dei prodotti fitosanitari - 1=Stampa la composizione dei prodotti fitosanitari
        Dim CarichiScarichi As Integer = 0 '0=carichi e scarichi - 1=solo carichi - 2=solo scarichi
        Dim Ordinamento As Integer = 0 '0=data - 1=prodotto
        Dim StampaLotto As Integer = 1 '0=Stampa sempre il lotto - 1=Stampa in base alla configurazione del prodotto
        Dim Lotto = ""
        Dim Raggruppamento As Integer = 1 '0=Stampa raggruppata - 1=Stampa standard

        If ddlMagazzini.SelectedValue = "-1" Then
            Messaggi.AgroMsgBox("Selezionare il Magazzino!", Master.Page)
            Exit Sub
        End If

        Dim Piva As String = Lbl_Piva.Text
        Dim Sa_Cod As Integer = ddlMagazzini.SelectedValue.Split("|")(1)
        Dim Fabbricato_Cod As Integer = ddlMagazzini.SelectedValue.Split("|")(0)

        Select Case RblStampaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze

                TargetURL = PaginaLinkStampaSchedaGiacenzeMagazzino

                If Me.TxtStampa.Text = "" Then
                    Messaggi.AgroMsgBox("Selezionare la data in cui verificare le Giacenze di Magazzino. Non un intervallo.", Master.Page)
                    Exit Sub
                Else
                    Data_Stampa = TxtStampa.Text
                End If

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                TargetURL = PaginaLinkStampaSchedaMovimentiMagazzino
                CarichiScarichi = 0

                Data_Inizio = TxtValiditaInizio.Text
                Data_Fine = TxtValiditaFine.Text

            Case enum_CodificaStampe.SchedaMagazzinoFertilizzanti, enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari

                If RblStampaMagazzino.SelectedValue = enum_CodificaStampe.SchedaMagazzinoFertilizzanti Then
                    TargetURL = PaginaLinkStampaSchedaFertilizzantiMagazzino
                Else
                    TargetURL = PaginaLinkStampaSchedaProdottiFitosanitariMagazzino
                End If

                Data_Inizio = TxtValiditaInizio.Text
                Data_Fine = TxtValiditaFine.Text

        End Select

        Dim ChkList_Categorie As New CheckBoxList
        AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(ChkList_Categorie, False, "", "", 0, CAU_SCARICO, 0, False, False,
                                                                 " (Tabella IN ('Materie_Prime','TipologieSementi') ) AND  Elem_Cod NOT IN (" + CStr(FARMACI) + ", " + CStr(MANGIMI) + ")",
                                                                 "", objParametri_Server)

        For Each item As ListItem In ChkList_Categorie.Items
            Str_elem_cod += item.Value + ","
        Next
        Str_elem_cod = "(" + Left(Str_elem_cod, Str_elem_cod.Length - 1) + ")"

        ''================================================================

        Session("Regione_Selezionata") = ""
        If Chk_LogoRegione.Checked = True Then
            If Cmb_Regioni.SelectedIndex >= 0 Then
                Session("Regione_Selezionata") = Me.Cmb_Regioni.SelectedValue.ToString
            End If
        End If

        Dim Querystring As String = "?p=" + Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) +
                                    "&s=" + Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder, Server) +
                                    "&f=" + Stringa_Codifica(Fabbricato_Cod, AgroKey_EncoderDecoder, Server) +
                                    "&e=" + Stringa_Codifica(Elem_Cod, AgroKey_EncoderDecoder, Server) +
                                    "&pro=" + Stringa_Codifica(Pro_Cod, AgroKey_EncoderDecoder, Server) +
                                    "&mat=" + Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Server) +
                                    "&arr=" + Stringa_Codifica(Tipo_Arrotondamento, AgroKey_EncoderDecoder, Server)

        Select Case RblStampaMagazzino.SelectedValue

            Case enum_CodificaStampe.SchedaMagazzinoGiacenze

                Querystring += "&ds=" + Stringa_Codifica(Data_Stampa.ToShortDateString, AgroKey_EncoderDecoder, Server) +
                               "&fec=" + Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder, Server) +
                               "&fvc=" + Stringa_Codifica(FlagVisualizzaComposizione, AgroKey_EncoderDecoder, Server) &
                               "&lot=" + Stringa_Codifica(Lotto, AgroKey_EncoderDecoder, Server) &
                               "&sl=" + Stringa_Codifica(StampaLotto, AgroKey_EncoderDecoder, Server) &
                               "&rag=" + Stringa_Codifica(Raggruppamento, AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.SchedaMagazzinoMovimenti

                Querystring += "&di=" + Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder, Server) +
                               "&df=" + Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder, Server) +
                               "&ord=" + Stringa_Codifica(Ordinamento, AgroKey_EncoderDecoder, Server) +
                               "&fec=" + Stringa_Codifica(Str_elem_cod, AgroKey_EncoderDecoder, Server) &
                               "&lot=" + Stringa_Codifica(Lotto, AgroKey_EncoderDecoder, Server) &
                               "&sl=" + Stringa_Codifica(StampaLotto, AgroKey_EncoderDecoder, Server) &
                               "&cs=" + Stringa_Codifica(CarichiScarichi, AgroKey_EncoderDecoder, Server)

            Case enum_CodificaStampe.SchedaMagazzinoFertilizzanti, enum_CodificaStampe.SchedaMagazzinoProdottiFitosanitari

                Querystring += "&di=" + Stringa_Codifica(Data_Inizio.ToShortDateString, AgroKey_EncoderDecoder, Server) +
                               "&df=" + Stringa_Codifica(Data_Fine.ToShortDateString, AgroKey_EncoderDecoder, Server) +
                               "&ord=" + Stringa_Codifica(Ordinamento, AgroKey_EncoderDecoder, Server) +
                               "&fvc=" + Stringa_Codifica(FlagVisualizzaComposizione, AgroKey_EncoderDecoder, Server)

        End Select

        'Page_NewWindow_2010(Page, TargetURL, Querystring, , , , , , , , , "MainContent", True)
        Page_NewWindow_2010(Page, TargetURL, Querystring, , , , , , , , , , True)

    End Sub

End Class


