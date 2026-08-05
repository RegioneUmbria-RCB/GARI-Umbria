Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreDataProvider.UtilityProvider_2010
Imports AgronicaCoreUtility
Imports System.Web.Services
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports System.IO

''' <summary>
'''     Struttura utilizzata come enumerico di stringhe (Elenco di costanti di stringhe raggruppati)
''' </summary>
''' <remarks></remarks>




Public Class Selezione_SchedaCampagna_BS
    Inherits System.Web.UI.Page



    ''' <summary>
    '''     Struttura utilizzata come enumerico di stringhe (Elenco di costanti di stringhe raggruppati)
    ''' </summary>
    ''' <remarks></remarks>



    Public nomeStampa As String = ""
    Public mostraFirmaODC As Boolean = False
    Public mostraDataDiStampa As Boolean = False
    Public DefaultCheckImpostaOrganismoReferente As Boolean = False
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

   Dim ElencoReport_SchedaCampagna_Multicentro_ACA As String() = {
       ElencoReportChiave.FRONTESPIZIO,
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
       ElencoReportChiave.INFORMAZ_E_DICHIAR,
       ElencoReportChiave.VERIFICHE_CONFORMITA,
       ElencoReportChiave.TRATTAMENTI_POSTRACCOLTA
   }

    Public Sub New()

    End Sub
#End Region

    '######################################################################################################
    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        Exit Sub
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

        '##############################################################
        '#####  Verifico se sono in Post-Back  ########################
        '##############################################################

        If Not Page.IsPostBack Then
            Dim DataInizio, DataFine As Date
            Dim objImpost As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            objImpost.AnnataAgraria(Date.Today, DataInizio, DataFine, objParametri_Utenti)

            hdQS_DataInizioAnnata.Value = DataInizio
            hdQS_DataFineAnnata.Value = DataFine
            hdQS_DataOggi.Value = Date.Today.ToShortDateString
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
        Dim strCodiciTerreno As String = ""
        Dim strCodiceTerreno As String = ""

        Dim Flag_ErbOrt As Boolean = False
        hd_Flag_ErbOrt.Value = Flag_ErbOrt

        Dim objVerificaDisciplinare2010 As New AgronicaCoreGestioneRichieste.ParametriVerificaDisciplinare2010

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

                'forza report 
                'Report = 29

                hdReportSelezionato.Value = Report        ' hidden usato per ImpostaChk()


                ViewState("Report") = Report
                Session("Report") = Report

                Dim objreport As New AgronicaCoreMetaSchemaDAL.StampeReport
                nomeStampa = objreport.LeggiDescrizione(Report, "", objParametri_Server)

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
                hd_visualizzaImpinatiFiltrati.Value() = UtilFiltr.ToString()

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



                'per questo report di default imposto tutte le sezioni da stampare anche vuote
                'If Report = enum_CodificaStampe.SchedaInterventiAgronomici Then
                '    checkedSezioneVuote.Value = "a|b|c|f|g|h|i|m|n|r"
                '    Me.Chk_IntestazioneOrgref.Visible = True
                'Else
                '    Me.Chk_IntestazioneOrgref.Visible = False
                'End If

                Select Case Report

                    Case enum_CodificaStampe.Registro_Trattamenti_Massivo,
                        enum_CodificaStampe.Registro_Fertilizzazioni_Massivo

                        'Riepilogo_Aziende.Visible = True
                        'Riepilogo_Azienda.Visible = False

                        'ReDim Matrice_Variabili(RigheMatrice, 0)

                        ''re-inizializzo
                        'RigheMatrice = -1

                        'Dim Dt As New DataTable
                        'Dim Dr As DataRow

                        'Dt.Columns.Add(New DataColumn("piva", GetType(String)))
                        'Dt.Columns.Add(New DataColumn("rag_soc", GetType(String)))

                        'Dim NomiChiavi(0) As String
                        'NomiChiavi(0) = "piva"

                        ''lettura aziende da stampare
                        'Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                        'Dim dtc As DataTable = cf.Leggi(0, "Stampe_Massive_Filtro", " valore = 'false' ", "", HttpContext.Current.Session("ASG_objParametri_Server"))


                        'If dtc.Rows.Count = 1 Then
                        '    'DA TABELLA
                        '    Dim DtSm As New DataTable
                        '    Dim objSM As New AgronicaCoreStampeDAL.Stampe_Massive_R
                        '    DtSm = objSM.Leggi("", Report, AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni, "", "", objParametri_Server)
                        '    For i = 0 To DtSm.Rows.Count - 1
                        '        Dr = Dt.NewRow
                        '        Dr.Item("rag_soc") = DtSm.Rows(i).Item("rag_soc")
                        '        Dr.Item("piva") = DtSm.Rows(i).Item("piva")
                        '        Dt.Rows.Add(Dr)
                        '    Next

                        'Else
                        '    'PASSATI DA FILTRONE

                        '    For i = 0 To XMLs_VariabiliStampe.Count - 1

                        '        XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                        '        RigheMatrice += 1

                        '        Matrice_Variabili(RigheMatrice, 0) = XML_VariabiliStampe.GetAttribute("piva")

                        '        Dr = Dt.NewRow
                        '        Dr.Item("rag_soc") = ""
                        '        Dr.Item("piva") = Matrice_Variabili(RigheMatrice, 0)
                        '        Dt.Rows.Add(Dr)

                        '    Next

                        'End If

                        'Me.DataGrid_Imprese.DataSource = Dt
                        'Me.DataGrid_Imprese.DataKeyNames = NomiChiavi
                        'Me.DataGrid_Imprese.DataBind()

                        'Qs_Piva = Matrice_Variabili(0, 0)
                        'Session("Piva") = Qs_Piva

                        'fine stampe massive
                        '-----------------------------------

                    Case Else

                        'Riepilogo_Aziende.Visible = False
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
                                hd_Flag_ErbOrt.Value = Flag_ErbOrt
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

                        hd_Lista_Sa_Cod_Da_Variabili_Stampe.Value = ""
                        hd_Lista_Veg_Cod_Da_Variabili_Stampe.Value = ""
                        hd_codiciTerreno.Value = strCodiciTerreno
                        hd_hashDestUso.Value = JsonConvert.SerializeObject(HashDestUso)


                        '(03/09/2016) fede aggiunte combo
                        Dim strSaCod As String = ""
                        Dim strVegCod As String = ""
                        For Each Hsacod As Int32 In HashSaCod.Keys
                            strSaCod &= Hsacod & ","
                        Next
                        For Each Hvegcod As Int32 In HashVegCod.Keys
                            strVegCod &= Hvegcod & ","
                        Next

                        hd_Lista_Sa_Cod_Da_Variabili_Stampe.Value = strSaCod
                        hd_Lista_Veg_Cod_Da_Variabili_Stampe.Value = strVegCod

                        Qs_Sa_Cod = Matrice_Variabili(0, 1) 'deve essere = a quello di riferimento
                        Qs_Veg_Cod = Matrice_Variabili(0, 4)

                        Session("Piva") = Qs_Piva

                        objVerificaDisciplinare2010.Piva = Qs_Piva


                        Dim Gru_Cod As Integer

                        Gru_Cod = CInt(GruppoVeg.GruCod_from_VegCod(Qs_Veg_Cod, objParametri_Server))
                        hd_GruCod.Value = Gru_Cod

                        'If Gru_Cod = 1 AndAlso Not Flag_ErbOrt Then

                        '    DivRotazione.Visible = False
                        'End If

                        If Qs_Veg_Cod = "0" Then

                            'If strCodiciTerreno <> "" Then

                            '    If Report <> enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita Then
                            '        Me.Lbl_Specie_Vegetale.Text = strCodiciTerreno
                            '    Else
                            Me.Lbl_Specie_Vegetale.Text = "......"
                            '    End If
                            'End If

                            'se ho scelto un rilievo piogge,nn avendo la specie,nn faccio scegliere le altre sezioni


                        ElseIf Qs_Veg_Cod = "-1" Then
                            Me.Lbl_Specie_Vegetale.Text = " "
                            Me.btnSelezionaTutteSezioni.Visible = False 'ok
                            Me.btnDeselezionaTutteSezioni.Visible = False 'ok
                        Else

                            'If Report <> enum_CodificaStampe.RegistroTrattamenti_Semplificata AndAlso
                            '    Report <> enum_CodificaStampe.Registro_Fertilizzazioni AndAlso
                            '    Report <> enum_CodificaStampe.RegistroTrattamenti_Veneto AndAlso
                            '    Report <> enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita AndAlso
                            '    Report <> enum_CodificaStampe.SchedaCampagna_Multi_Lombardia AndAlso
                            '    Report <> enum_CodificaStampe.RegistroAziendaleUnico Then
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
                                    DefaultCheckImpostaOrganismoReferente = True
                                End If
                            End If
                        End If




                        '-------------------------------------------------------------------
                        '----- Tabella Imprese

                        Me.Lbl_Piva.Text = Qs_Piva
                        hds_piva.Value = Lbl_Piva.Text

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

                End Select
                Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                Dim impAvversita = objUtenti.Impostazione_Valore_From_Impostazione_Cod_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.StampaCampagna_Default_Vedi_AvversitaQta, objParametri_Utenti)
                Dim value as String = If(String.IsNullOrEmpty(impAvversita) OrElse impAvversita = "1", "1", "0") ' Preso dalla pagina Selezione_SchedaCampagna.aspx.vb, non so se' e' voluto che di default sia attivo
                hd_StampaCampagna_Default_Vedi_AvversitaQta.Value = value
                        
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
                        Lbl_Specie_Vegetale.Style.Remove("display")
                        Lbl_Centro.Style.Remove("display")


                    Case Else
                        If ViewState("UtilizzaImpiantiFiltrati") = 1 Then
                            'TODO DECOMMENTARE ALLA FINE
                            'Cmb_CentroAziendale.Visible = False
                            'Cmb_Specie.Visible = False
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

                        'DivGlobalGap.Visible = True
                        hd_globalGapDiv.Value() = "visible"
                        hd_TempoDiRientro.Value() = "visible"

                        If Report = enum_CodificaStampe.Eurep_Gap_Multicentro Then
                            'CheckTitoloGlobal.Visible = True
                        End If

                    Case enum_CodificaStampe.SchedaCampagna_Multi_Lombardia
                        hd_regolamentiDiv.Value() = "visible"

                    Case enum_CodificaStampe.RegistroAziendaleUnico
                        hd_regolamentiDiv.Value() = "visible"
                        hd_ordinamentoDiv.Value() = "visible"

                    Case enum_CodificaStampe.RegistroTrattamentiVeneto_StdCondizionalita
                        hd_ordinamentoDiv.Value() = "visible"

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


            ' Memorizzo variabili server negli hidden fields per il JS
            hd_Sa_Cod.Value = Qs_Sa_Cod
            hd_Sezione.Value = Qs_Sezione
            hd_VegCod.Value = Qs_Veg_Cod
            ' cIdReportSelezionato -> Report

            'forza case
            'hd_EnumCodificaStampe.Value = "2"

            'Report = 29
            Select Case Report

                Case enum_CodificaStampe.Eurep_Gap_Semplificata,
                     enum_CodificaStampe.SchedaCampagna_Pizzoli,
                     enum_CodificaStampe.Eurep_Gap_Multicentro,
                     enum_CodificaStampe.SchedaCampagna_ProvAut_Trento
                    'Me.Chk_VisualizzaCapitolatoPrivato.Visible = True

                Case enum_CodificaStampe.SchedaCampagna_Multicentro,
                    enum_CodificaStampe.SchedaCampagna_2078_Semplificata,
                    enum_CodificaStampe.SchedaRegistrazione_Semplificata,
                    enum_CodificaStampe.SchedaCampagna_ConserveItalia
                    'Me.Chk_VisualizzaCapitolatoPrivato.Visible = True

                Case enum_CodificaStampe.SchedaColturale_Biologico

                    'Me.Chk_RaggruppaXCampo.Visible = True 'ok
                    'Me.Chk_RaggruppaXCampo.Checked = True 'ok
                    Dim objImpostazioni As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
                    Dim valore_Impostazione_Cod_MostraFirmaODC = Integer.Parse(objImpostazioni.LeggiConDefault(
                            enum_Impostazioni_Utenti.UTENTE_Cod_MostraFirmaODCBio, 2,
                            "0", objParametri_Utenti)) = 1
                    Dim valore_Impostazione_Cod_PreflagMostraData = Integer.Parse(objImpostazioni.LeggiConDefault(
                            enum_Impostazioni_Utenti.Utente_Cod_DefaultMostraDataBio, 2,
                            "0", objParametri_Utenti)) = 1

                    mostraFirmaODC = valore_Impostazione_Cod_MostraFirmaODC
                    mostraDataDiStampa = valore_Impostazione_Cod_PreflagMostraData



                Case enum_CodificaStampe.RegistroTrattamenti_Veneto
                    '20/06/2018
                    'SEZIONI_VisualizzaSezionixRegTrattVeneto()
                    'Lbl_ImpostaSezioni.Visible = False 'ok
                    '' TO DO NICO
                    'BtnStampeVuote.Visible = False


            End Select

            ''questo filtro è per le impostazioni utente riguardanti le pratiche ecologiche
            'DrFiltro = DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Security_Attivita.CheckList_Pratiche_Ecologiche_APOT)
            'If Not DrFiltro Is Nothing AndAlso DrFiltro.Length > 0 Then
            '    If Not IsDBNull(DrFiltro(0).Item("Impostazione_Valore_1")) AndAlso DrFiltro(0).Item("Impostazione_Valore_1") <> "" Then
            '        Fascicoli.Visible = True
            '        Dim FiltroFascicoli As String = "Allegati_Documenti_Piva='" & Qs_Piva & "' AND Allegati_Documenti_CatCod=9 "
            '        AgronicaCoreUtility.CaricaListControl.Fascicoli(Cmb_Fascicoli, True, "Tutte", "0", FiltroFascicoli, " Allegati_Documenti_Numero DESC ", objParametri_Server)
            '    End If
            'End If

            'SPOSTATO QUI PER ABILITARE O DISABILITARE SUCCESSIVAMENTE
            SEZIONI_AggiungiSezioniConPermessi(Report)

            SEZIONI_AbilitaDisabilita(Report)

            ViewState("Piva") = Qs_Piva
            ViewState("Sa_Cod") = Qs_Sa_Cod



            '##############################################################
            '#####  CARICO I DATI  ########################################
            '##############################################################




            Session("Revisione") = Nothing


            '-------------------------------------------------------------------
            '----- Imposto la data di stampa



            Dim PathAllegati As String
            'Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
            If objAgroWeb.GestioneAllegati_Repository = String.Empty Then
                PathAllegati = "C:\GIASLAN\AgronicaStampe_Allegati\"
                hd_PathAllegati.Value = PathAllegati
            Else
                PathAllegati = objAgroWeb.GestioneAllegati_Repository
                hd_PathAllegati.Value = PathAllegati
            End If

            objAgroWeb = Nothing

            'In base all'impostazione utente visualizzo o meno la combo fascicoli (x umbria)

            'Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            'Dim DtImpostazioni As DataTable

            'DtImpostazioni = objUtenti.Leggi(0, 1,
            '                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
            '                                 "", "", objParametri_Utenti)
            ' Dim DrFiltro() As DataRow

            ' questo filtro lo leggo sempre
            'DrFiltro = DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_CAMPAGNA_FILTRA_FASCICOLO)
            'If Not DrFiltro Is Nothing AndAlso DrFiltro.Length > 0 Then
            '    If Not IsDBNull(DrFiltro(0).Item("Impostazione_Valore_1")) AndAlso DrFiltro(0).Item("Impostazione_Valore_1") <> "" Then
            '        Fascicoli.Visible = True
            '        Dim FiltroFascicoli As String = "Allegati_Documenti_Piva='" & Qs_Piva & "' AND Allegati_Documenti_CatCod=9 "
            '        AgronicaCoreUtility.CaricaListControl.Fascicoli(Cmb_Fascicoli, True, "Tutte", "0", FiltroFascicoli, " Allegati_Documenti_Numero DESC ", objParametri_Server)
            '    End If
            'End If


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



    '#############################################
    '20/06/2018: centralizzato codice in funzione dedicata


    Private Shared Sub SEZIONI_SalvaSezioniSeleziontexStampa_New(  'ByVal parametriStampa As Parametri_Controlli_Pre_StampaNew,
                                              ByRef strSezioni As String,
                                              ByRef Num_Sezioni As Integer,
                                              ByVal SchedaVeneto As String)

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

        HttpContext.Current.Session("sezioni") = strSezioni


    End Sub




    '##############################################################################################################

    <WebMethod(EnableSession:=True)>
    Public Shared Function Carica_Kendo_ColturePrecedenti() As RispostaStandard
        Dim r As New RispostaStandard
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Try
            Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

            If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
                r.Sessione = False
                Return r
            End If

            Dim GruppoVeg As New AgronicaCoreMetaSchemaDAL.GruppoVegetale_R
            Dim Reg_Impianti_Read As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
            Dim ObjImpiantiCodici As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R

            Dim Dt As New DataTable
            Dim Dr As DataRow
            Dim i As Integer

            Dim DtCodici As New DataTable
            Dim DrCodici() As DataRow

            'Dim Qs_Piva As String
            'Dim Qs_Sa_Cod As String
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
            Dt.Columns.Add(New DataColumn("CodColtura1", GetType(String)))
            Dt.Columns.Add(New DataColumn("CodColtura2", GetType(String)))
            Dt.Columns.Add(New DataColumn("CodColtura3", GetType(String)))
            Dt.Columns.Add(New DataColumn("CodColtura4", GetType(String)))
            Dt.Columns.Add(New DataColumn("campo_cod", GetType(String)))
            Dt.Columns.Add(New DataColumn("Validita_inizio_Impianto", GetType(String)))
            Dt.Columns.Add(New DataColumn("KeyReg", GetType(String)))


            Matrice_Variabili = HttpContext.Current.Session("Matrice_Variabili")

            'Qs_Piva = ViewState("Piva")
            'Qs_Sa_Cod = ViewState("Sa_Cod")

            Data_inizio = AGRODATAINIZIO

            Data_fine = AGRODATAFINE


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
                        Dr.Item("campo_cod") = Vet_Impianti(16)
                        Dr.Item("Validita_inizio_Impianto") = Vet_Impianti(13)
                        Dr.Item("KeyReg") = $"{Matrice_Variabili(i, 1)}|{Matrice_Variabili(i, 2)}|{Matrice_Variabili(i, 3)}"

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
                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 AndAlso Not DBNull.Value.Equals(DrCodici(0).Item("val_cod")) Then
                                Dr.Item("Coltura1") = DrCodici(0).Item("val_cod")
                                Dr.Item("CodColtura1") = DrCodici(0).Item("val_cod")
                            Else
                                Dr.Item("Coltura1") = ""
                                Dr.Item("CodColtura1") = ""
                            End If

                            DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_2)
                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 AndAlso Not DBNull.Value.Equals(DrCodici(0).Item("val_cod")) Then
                                Dr.Item("Coltura2") = DrCodici(0).Item("val_cod")
                                Dr.Item("CodColtura2") = DrCodici(0).Item("val_cod")
                            Else
                                Dr.Item("Coltura2") = ""
                                Dr.Item("CodColtura2") = ""
                            End If

                            DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_3)
                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 AndAlso Not DBNull.Value.Equals(DrCodici(0).Item("val_cod")) Then
                                Dr.Item("Coltura3") = DrCodici(0).Item("val_cod")
                                Dr.Item("CodColtura3") = DrCodici(0).Item("val_cod")
                            Else
                                Dr.Item("Coltura3") = ""
                                Dr.Item("CodColtura3") = ""
                            End If

                            DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_4)
                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 AndAlso Not DBNull.Value.Equals(DrCodici(0).Item("val_cod")) Then
                                Dr.Item("Coltura4") = DrCodici(0).Item("val_cod")
                                Dr.Item("CodColtura4") = DrCodici(0).Item("val_cod")
                            Else
                                Dr.Item("Coltura4") = ""
                                Dr.Item("CodColtura4") = ""
                            End If

                        End If

                        'aggiungo la riga
                        Dt.Rows.Add(Dr)

                    End If

                End If 'gruppo veg

            Next

            'Session("Indici_Matrice") = Indici_Matrice
            'ALEX inserire controllo culture per rigo


            'InserisciPrecessioniColturali(Dt, objParametri_Server)






            'Dim NomiChiavi(12) As String

            'Chiave della griglia
            'NomiChiavi(0) = "Piva"
            'NomiChiavi(1) = "Sa_Cod"
            'NomiChiavi(2) = "Appezza"
            'NomiChiavi(3) = "Id_Reg"
            'NomiChiavi(4) = "Coltura1"
            'NomiChiavi(5) = "Coltura2"
            'NomiChiavi(6) = "Coltura3"
            'NomiChiavi(7) = "Coltura4"
            'NomiChiavi(8) = "CodColtura1"
            'NomiChiavi(9) = "CodColtura2"
            'NomiChiavi(10) = "CodColtura3"
            'NomiChiavi(11) = "CodColtura4"
            'NomiChiavi(12) = "KeyReg"
            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("PIva", "Piva Aziendale", "string"))
            l.Add(New ColonneNome("sa_cod", "sa_cod", "string"))
            l.Add(New ColonneNome("Sa_Nome", "Centro Aziendale", "string"))
            l.Add(New ColonneNome("Id_Reg", "Id_Reg", "string"))
            l.Add(New ColonneNome("Appezza", "Appezza", "string"))
            l.Add(New ColonneNome("Campo_Des", "Campo", "string"))
            l.Add(New ColonneNome("App_Nome", "App.", "string"))
            l.Add(New ColonneNome("Veg_Des", "Specie", "string"))
            l.Add(New ColonneNome("Cul_Des", "Varietà", "string"))
            l.Add(New ColonneNome("Dest_Uso", "Dest. Uso", "string"))
            l.Add(New ColonneNome("Sup_Imp", "Sup.Imp. [ha]", "string"))
            l.Add(New ColonneNome("Validita", "Validità Impianto", "string"))
            l.Add(New ColonneNome("Coltura1", "Coltura Precedente", "string") With {._Editabile = True})
            l.Add(New ColonneNome("Coltura2", "Coltura Precedente 2 anno", "string") With {._Editabile = True})
            l.Add(New ColonneNome("Coltura3", "Coltura Precedente 3 anno", "string") With {._Editabile = True})
            l.Add(New ColonneNome("Coltura4", "Coltura Precedente 4 anno", "string") With {._Editabile = True})
            l.Add(New ColonneNome("CodColtura1", "Codice Coltura Precedente 1", "string") With {._Editabile = False})
            l.Add(New ColonneNome("CodColtura2", "Codice Coltura Precedente 2 anno", "string") With {._Editabile = True})
            l.Add(New ColonneNome("CodColtura3", "Codice Coltura Precedente 3 anno", "string") With {._Editabile = True})
            l.Add(New ColonneNome("CodColtura4", "Codice Coltura Precedente 4 anno", "string") With {._Editabile = True})
            l.Add(New ColonneNome("KeyReg", "KeyReg Chiave", "string") With {._Editabile = True})


            Dim risp As String = js.JSON_DataTable_Kendo(Dt, l)
            r.RispostaOK = True
            r.RispostaStringa = risp
            ObjImpiantiCodici = Nothing
        Catch ex As Exception
            r.RispostaOK = True
            r.Errore = "Errore Durante la lettura 'Carica_Kendo_ColturePrecedenti'" &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try
        Return r



    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function ImpostaDatiGlobalGap() As RispostaStandard

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim objCACInfoAgg As New AgronicaCoreAnagrafeDAL.CAC_Codifica_InfoAggiuntive_R
            Dim Tipo_Codifica As Integer = 0
            r.RispostaStringa = objCACInfoAgg.InfoAgg_Des_from_InfoAgg_Cod(enum_CodificaStampe.Eurep_Gap_Multicentro,
                                                                        enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Revisione_Reportistica,
                                                                        Tipo_Codifica,
                                                                         objParametriServer)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    Private Shared Function LeggiPercorsoAllegati(ByRef objParametriServer As AgronicaCoreParametri) As String
        Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim percorso As String = objConfSiti.Leggi_Valore(6,
                                                          "GestioneAllegati_Repository",
                                                          "", "", objParametriServer)

        If String.IsNullOrEmpty(percorso) Then
            percorso = "C:\GIASLAN\AgronicaStampe_Allegati\" 'default
        End If

        Return FileSystemHelper.AggiungiSlashSeNonEsiste(percorso)

    End Function
    Private Shared Function OttieniFile(ByVal nomeFile As String,
                                 ByVal sottoCartella As String,
                                 ByRef objParametri_Server As AgronicaCoreParametri
                                 ) As String

        Dim xRisp As String = ""
        Try

            Dim SitoAllegati As String = LeggiPercorsoAllegati(objParametri_Server)

            Dim link As String = ""

            'TODO: Nel nomeFile potrei avere già il percorso completo

            If nomeFile.Contains("/") Then
                link = SitoAllegati & nomeFile
            Else
                'controllo se sono in modifica o in inserimento
                If IsDBNull(nomeFile) Then
                    link = SitoAllegati & "TEMP\" & nomeFile
                Else
                    Dim sottoCartellaLoc As String = ""
                    If Not String.IsNullOrEmpty(sottoCartella) Then
                        sottoCartellaLoc = FileSystemHelper.AggiungiSlashSeNonEsiste(sottoCartella)
                    End If

                    link = SitoAllegati & sottoCartellaLoc & nomeFile
                End If
            End If

            xRisp = link


        Catch ex As Exception
            Throw New Exception(ex.Message)
        End Try

        Return xRisp

    End Function


    Private Shared Function FileToByteArray(filePath As String) As Byte()
        Dim fileStream As FileStream = File.OpenRead(filePath)
        Dim byteArray As Byte() = New Byte(fileStream.Length - 1) {}

        ' Leggi i byte dal file e li memorizza nell'array di byte
        fileStream.Read(byteArray, 0, byteArray.Length)

        ' Chiudi lo stream del file
        fileStream.Close()

        Return byteArray
    End Function
    '##############################################################################################################
    <WebMethod(EnableSession:=True)>
    Public Shared Function SalvaColturePrecedenti(ByVal paramString As String) As RispostaStandard


        Dim r = New RispostaStandard
        Dim parametri As ParametriSalvaModel = Nothing

        Dim Reg_Impianti_Codici_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R 'New Agro_Anagrafe_AD.Reg_Impianti_Codici_W
        Dim Reg_Impianti_Codici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W 'New Agro_Anagrafe_AD.Reg_Impianti_Codici_R


        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}


        Dim Validita_Inizio As DateTime = AGRODATAINIZIO
        Dim Validita_Fine As DateTime = AGRODATAFINE

        Try

            parametri = JsonConvert.DeserializeObject(Of ParametriSalvaModel)(paramString)

            ''inseriti
            'If Not String.IsNullOrEmpty(parametri.RigheInserite) Then
            '    Dim inseriti As List(Of SalvaColturePrecedenti) = New List(Of Numeratore_Tipo_Model)()
            '    inseriti.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_Tipo_Model))(parametri.RigheInserite, settingLoc))
            '    For Each m As Numeratore_Tipo_Model In inseriti

            '        If Not m.Validita_Inizio Is Nothing Then Validita_Inizio = m.Validita_Inizio
            '        If Not m.Validita_Fine Is Nothing Then Validita_Fine = m.Validita_Fine

            '        objNumTipiW.Scrivi(parametri.Piva, m.Sigla, m.Descrizione,
            '                           Validita_Inizio, Validita_Fine, "", "", objParametri_Server)
            '    Next


            'End If

            'modificati
            If Not String.IsNullOrEmpty(parametri.RigheModificate) Then
                Dim modificati As List(Of CulturePrecedentiRow) = New List(Of CulturePrecedentiRow)()
                modificati.AddRange(JsonConvert.DeserializeObject(Of List(Of CulturePrecedentiRow))(parametri.RigheModificate, settingLoc))
                For Each m As CulturePrecedentiRow In modificati
                    Dim intDummy As Integer
                    Dim DrCodici() As DataRow
                    Dim Rs As DataTable = Reg_Impianti_Codici_R.Leggi(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                           "", 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                    If Not IsNothing(Rs) AndAlso Rs.Rows.Count <> 0 Then

                        DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_1)

                        If DrCodici Is Nothing Or DrCodici.Length = 0 Then

                            intDummy = Reg_Impianti_Codici_W.Scrivi(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                     CInt(enum_CodiciAnagrafe.Coltura_Precedente_1),
                                                                    m.Coltura1,
                                                                    AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                        Else

                            Reg_Impianti_Codici_W.Modifica(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                      CInt(enum_CodiciAnagrafe.Coltura_Precedente_1),
                                                            m.Coltura1,
                                                           CDate(Estremo_Validita_Inizio),
                                                           CDate(Estremo_Validita_Fine),
                                                           "", objParametri_Server)

                        End If

                        'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_2)
                        DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_2)

                        'se il codice esiste già lo modifico altrimenti lo scrivo
'                        DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_1)

                        If DrCodici Is Nothing Or DrCodici.Length = 0 Then

                            intDummy = Reg_Impianti_Codici_W.Scrivi(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                     CInt(enum_CodiciAnagrafe.Coltura_Precedente_2),
                                                                    m.Coltura2,
                                                                    AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                        Else

                            Reg_Impianti_Codici_W.Modifica(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                      CInt(enum_CodiciAnagrafe.Coltura_Precedente_2),
                                                            m.Coltura2,
                                                           CDate(Estremo_Validita_Inizio),
                                                           CDate(Estremo_Validita_Fine),
                                                           "", objParametri_Server)

                        End If


                        'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_3)
                        DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_3)

                        If DrCodici Is Nothing Or DrCodici.Length = 0 Then

                            intDummy = Reg_Impianti_Codici_W.Scrivi(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                     CInt(enum_CodiciAnagrafe.Coltura_Precedente_3),
                                                                    m.Coltura3,
                                                                    AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                        Else

                            Reg_Impianti_Codici_W.Modifica(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                      CInt(enum_CodiciAnagrafe.Coltura_Precedente_3),
                                                            m.Coltura3,
                                                           CDate(Estremo_Validita_Inizio),
                                                           CDate(Estremo_Validita_Fine),
                                                           "", objParametri_Server)

                        End If


                        DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_4)

                        'se il codice esiste già lo modifico altrimenti lo scrivo
                        If DrCodici Is Nothing Or DrCodici.Length = 0 Then

                            intDummy = Reg_Impianti_Codici_W.Scrivi(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                     CInt(enum_CodiciAnagrafe.Coltura_Precedente_4),
                                                                    m.Coltura4,
                                                                    AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                        Else

                            Reg_Impianti_Codici_W.Modifica(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                      CInt(enum_CodiciAnagrafe.Coltura_Precedente_4),
                                                            m.Coltura4,
                                                           CDate(Estremo_Validita_Inizio),
                                                           CDate(Estremo_Validita_Fine),
                                                           "", objParametri_Server)

                        End If



                    Else

                        'se nn esiste nessun codice li creo
                        intDummy = Reg_Impianti_Codici_W.Scrivi(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                     CInt(enum_CodiciAnagrafe.Coltura_Precedente_1),
                                                                m.Coltura1,
                                                                AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                        intDummy = Reg_Impianti_Codici_W.Scrivi(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                     CInt(enum_CodiciAnagrafe.Coltura_Precedente_2),
                                                                m.Coltura2,
                                                                AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                        intDummy = Reg_Impianti_Codici_W.Scrivi(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                     CInt(enum_CodiciAnagrafe.Coltura_Precedente_3),
                                                                m.Coltura3,
                                                                AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                        intDummy = Reg_Impianti_Codici_W.Scrivi(m.Piva, CInt(m.sa_cod), CInt(m.Appezza), CInt(m.Id_Reg),
                                                     CInt(enum_CodiciAnagrafe.Coltura_Precedente_4),
                                                                m.Coltura4,
                                                                AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)

                    End If




                Next
            End If

            ''eliminati
            'If Not String.IsNullOrEmpty(parametri.RigheCancellate) Then
            '    Dim cancellati As List(Of Numeratore_Tipo_Model) = New List(Of Numeratore_Tipo_Model)()
            '    cancellati.AddRange(JsonConvert.DeserializeObject(Of List(Of Numeratore_Tipo_Model))(parametri.RigheCancellate, settingLoc))
            '    For Each m As Numeratore_Tipo_Model In cancellati

            '        'Check se utilizzato prima di eliminarlo
            '        Dim dtPS = objNumPs.Leggi(m.Piva, m.Tipo, "", "", objParametri_Server)
            '        If Not dtPS Is Nothing And dtPS.Rows.Count > 0 Then
            '            r.RispostaStringa = "Non è possibile eliminare questo Tipo Numeratore perchè utilizzato nei Prefissi/Suffissi."
            '            Return r
            '        Else
            '            objNumTipiW.Cancella(m.Piva, m.Tipo, "", objParametri_Server)
            '        End If

            '    Next
            'End If

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
    Public Shared Function DownloadFileElencoReport(ByVal NomeFile As String, ByVal NomeSottoCartella As String) As rispostaStandard(Of Byte())

        Dim r = New rispostaStandard(Of Byte())
        Dim parametri As ParametriDownloadElencoReport = Nothing

        Dim Reg_Impianti_Codici_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R 'New Agro_Anagrafe_AD.Reg_Impianti_Codici_W
        Dim Reg_Impianti_Codici_W As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_W 'New Agro_Anagrafe_AD.Reg_Impianti_Codici_R


        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim inError As Boolean = False
        Dim percorsoFile As String = ""

        Try

            'Dim clickedRow As GridViewRow = TryCast(sender.NamingContainer, GridViewRow)

            'Dim nomeFile As String = clickedRow.Cells(COL_NOME_FILE).Text
            'Dim sottoCartella As String = clickedRow.Cells(COL_SOTTOCARTELLA).Text.Replace("&nbsp;", "")

            percorsoFile = OttieniFile(NomeFile, NomeSottoCartella.Replace("&nbsp;", ""), objParametri_Server:=HttpContext.Current.Session("ASG_objParametri_Server"))

            If String.IsNullOrEmpty(percorsoFile) OrElse Not File.Exists(percorsoFile) Then
                r.RispostaOK = False
                r.Errore = "File non trovato"
                Return r
            End If
            Dim byteArray As Byte() = FileToByteArray(percorsoFile)

            r.RispostaOK = True
            r.RispostaStringa = byteArray.ToArray()
        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore: " & ex.Message

        End Try
        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function CaricaElenco() As RispostaStandard
        Dim r As New RispostaStandard
        Dim js As New AgronicaCoreDataProvider.JSON_DataTable
        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If
        Dim strFiltroImpianti As String = ""
        Dim Cat_Cod As enum_CategorieDocumenti = enum_CategorieDocumenti.RegistriCampagna 'default

        Dim strXmlVariabilistampe As String = HttpContext.Current.Session("strXmlVariabilistampe")

        Dim XmlDoc As New System.Xml.XmlDocument
        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        'Carico la stringa xml in un nuovo documento
        XmlDoc = New System.Xml.XmlDocument
        XmlDoc.LoadXml(strXmlVariabilistampe)

        If XmlDoc.HasChildNodes Then

            'Ricavo i parametri che servono

            XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

            HttpContext.Current.Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")

            XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

            Dim strFiltroImpianto As String

            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                strFiltroImpianto = " (Alert_Entita.PIVA='" & XML_VariabiliStampe.GetAttribute("piva") & "' " &
                                    " AND Alert_Entita.SA_COD=" & XML_VariabiliStampe.GetAttribute("sa_cod") &
                                    " AND Alert_Entita.APPEZZA=" & XML_VariabiliStampe.GetAttribute("appezza") &
                                    " AND Alert_Entita.Id_Imp=" & XML_VariabiliStampe.GetAttribute("id_reg") &
                                    " ) OR"

                strFiltroImpianti &= strFiltroImpianto

            Next

            'tolgo l'ultimo OR
            strFiltroImpianti = "(" & Left(strFiltroImpianti, strFiltroImpianti.Length - 2) & ")"

        End If



        Try

            Dim objAlert As New AgronicaCoreScadenziario.Alert_Elenco_R
            Dim dt As DataTable = objAlert.Leggi_ElencoAllegati(Cat_Cod,
                                                                strFiltroImpianti,
                                                                "",
                                                                "",
                                                                objParametri_Server)

            If IsNothing(dt) OrElse dt.Rows.Count = 0 Then

                Select Case Cat_Cod

                    Case enum_CategorieDocumenti.RegistriCampagna
                        r.RispostaStringa = "Non sono stati archiviati report per gli impianti selezionati."

                    Case Else
                        'Case enum_CategorieDocumenti.LibroGiornale
                        r.RispostaStringa = "Non sono stati archiviati report per la stampa in oggetto."

                End Select
                r.Sessione = False
                Return r
            End If

            Dim dtElenco As New DataTable
            dtElenco.Columns.Add(New DataColumn("Piva", GetType(String)))
            dtElenco.Columns.Add(New DataColumn("Documento_Cod", GetType(Integer)))
            dtElenco.Columns.Add(New DataColumn("SottoCartella", GetType(String)))
            dtElenco.Columns.Add(New DataColumn("NomeFile", GetType(String)))
            dtElenco.Columns.Add(New DataColumn("Inizio", GetType(String)))
            dtElenco.Columns.Add(New DataColumn("Fine", GetType(String)))

            For i = 0 To dt.Rows.Count - 1

                Dim nomeFile As String = If(IsDBNull(dt.Rows(i).Item("Allegati_Documenti_NomeFile")), "", dt.Rows(i).Item("Allegati_Documenti_NomeFile"))

                If Not String.IsNullOrEmpty(nomeFile) Then

                    Dim dr As DataRow = dtElenco.NewRow

                    dr.Item("Piva") = dt.Rows(i).Item("Allegati_Documenti_Piva")
                    dr.Item("Documento_Cod") = CInt(dt.Rows(i).Item("Allegati_Documenti_Cod"))
                    dr.Item("SottoCartella") = If(IsDBNull(dt.Rows(i).Item("Sottocartella")), "", dt.Rows(i).Item("Sottocartella"))
                    dr.Item("NomeFile") = nomeFile

                    If Not IsDBNull(dt.Rows(i).Item("Validita_Inizio")) AndAlso (IsDate(dt.Rows(i).Item("Validita_Inizio"))) Then
                        dr.Item("Inizio") = CDate(dt.Rows(i).Item("Validita_Inizio")).ToShortDateString
                    Else
                        dr.Item("Inizio") = ""
                    End If

                    If Not IsDBNull(dt.Rows(i).Item("Validita_Fine")) AndAlso (IsDate(dt.Rows(i).Item("Validita_Fine"))) Then
                        dr.Item("Fine") = CDate(dt.Rows(i).Item("Validita_Fine")).ToShortDateString
                    Else
                        dr.Item("Fine") = ""
                    End If

                    dtElenco.Rows.Add(dr)
                End If

            Next

            Dim l As New List(Of ColonneNome)

            l.Add(New ColonneNome("Piva", "Piva Aziendale", "string"))
            l.Add(New ColonneNome("Documento_Cod", "Documento_Cod", "Integer"))
            l.Add(New ColonneNome("SottoCartella", "SottoCartella", "string"))
            l.Add(New ColonneNome("NomeFile", "NomeFile", "string"))
            l.Add(New ColonneNome("Inizio", "Inizio", "string"))
            l.Add(New ColonneNome("Fine", "Fine", "string"))

            Dim risp As String = js.JSON_DataTable_Kendo(dtElenco, l)
            r.RispostaOK = True
            r.RispostaStringa = risp

        Catch ex As Exception
            r.RispostaOK = True
            r.Errore = "Errore Durante la lettura 'Carica_Kendo_ColturePrecedenti'" &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)


        End Try
        Return r

    End Function






    <WebMethod(EnableSession:=True)>
    Public Shared Function GlobalGap_SalvaDatiNew(ByVal strDati As String) As RispostaStandard

        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If

        Dim Log_Errori As String = ""
        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

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

            If strDati <> "" Then

                objCAC.Scrivi(enum_CodificaStampe.Eurep_Gap_Semplificata,
                              strDati,
                              enum_CAC_Codifica_InfoAggiuntive_ArgomentoCod.Revisione_Reportistica,
                              "Revisione Report Global-Gap Semplificata",
                              tipo_codifica,
                               0, 0, 0, "", "", "",
                               AGRODATAINIZIO,
                               AGRODATAFINE,
                               objParametri_Server)

                objCAC.Scrivi(enum_CodificaStampe.Eurep_Gap_Multicentro,
                          strDati,
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

    End Function




    <WebMethod(EnableSession:=True)>
    Public Shared Function Controlli_Pre_StampaNew(ByVal objParams As String) As RispostaStandard

        Dim r As New RispostaStandard

        Dim strSezioni As String = ""
        Dim numSezioni As Integer = 0

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametriUtenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim settingLoc As New JsonSerializerSettings With
                {
                    .DateTimeZoneHandling = DateTimeZoneHandling.Local
                }

            'se non restituisce errore durante la deserializzazione del Json
            Dim parametri As Parametri_Controlli_Pre_StampaNew = JsonConvert.DeserializeObject(Of Parametri_Controlli_Pre_StampaNew)(objParams, settingLoc)
            Dim sezioni As Sezioni_Pre_StampaNew = JsonConvert.DeserializeObject(Of Sezioni_Pre_StampaNew)(objParams, settingLoc)

            
            Dim Report As Integer = parametri.reportSelezionato

            ' codice che prima era in imgbtnStampa_Click
            Select Case Report
                Case enum_CodificaStampe.Registro_Trattamenti_Massivo, enum_CodificaStampe.Registro_Fertilizzazioni_Massivo

                    Dim strXmlVariabilistampe As String = ""
                    Dim StrNodiVariabili As String = ""
                    Dim StrNodo As String = ""
                    Dim objVS As New AgronicaCoreXML.XML_Stampe

                    ' TODO Richiedere a Federica sta roba sulla datagrid IMprese

                    'For i = 0 To DataGrid_Imprese.Rows.Count - 1
                    '    If CType(DataGrid_Imprese.Rows(i).FindControl("ChkSeleziona"), CheckBox).Checked Then
                    '        Piva = Me.DataGrid_Imprese.Rows(i).Cells(1).Text

                    '        Dim vVarStampe(0) As ElementoStampe
                    '        vVarStampe(0).Nome = "piva"
                    '        vVarStampe(0).Valore = Piva

                    '        StrNodo = objVS.XML_VariabiliStampe(vVarStampe)
                    '        StrNodiVariabili = StrNodiVariabili & StrNodo

                    '    End If
                    'Next

                    If StrNodiVariabili <> "" Then
                        strXmlVariabilistampe = HttpContext.Current.Session("strXmlVariabilistampe")

                        'Carico la stringa xml in un nuovo documento
                        Dim XmlDoc As New System.Xml.XmlDocument
                        XmlDoc.LoadXml(strXmlVariabilistampe)
                        Dim XML_FiltroStampa As System.Xml.XmlElement = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

                        XML_FiltroStampa.InnerXml = StrNodiVariabili

                        HttpContext.Current.Session("strXmlVariabilistampe") = XmlDoc.OuterXml

                    End If

                Case Else

                    If Not (Not parametri.UtilizzaImpiantiFiltrati Is Nothing AndAlso parametri.UtilizzaImpiantiFiltrati = "1") Then
                        Dim SaCod As Integer = 0
                        Dim VegCod As Integer = 0
                        Dim CulCod As Integer = 0
                        Dim id_cod As Integer = 0


                        Select Case parametri.ddlCentroAziendale
                            Case "0" 'tutti
                                SaCod = 0
                            Case Else 'singolo
                                SaCod = CInt(parametri.ddlCentroAziendale)
                        End Select

                        Select Case CInt(parametri.ddlSpecieVegetale)
                            Case 0 'terreno nudo
                            Case Is < 0 'dest uso
                                id_cod = Math.Abs(CInt(parametri.ddlSpecieVegetale))
                            Case Else 'specie
                                VegCod = CInt(parametri.ddlSpecieVegetale)
                        End Select

                        Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
                        Dim Dt As DataTable
                        Dim HashImp As New Hashtable
                        Dim strDataFiltro As String = String.Empty

                        Dim strXmlVariabilistampe As String = HttpContext.Current.Session("strXmlVariabilistampe")
                        Dim StrNodiVariabili As String = ""
                        Dim StrNodo As String = ""
                        Dim objVS As New AgronicaCoreXML.XML_Stampe

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
                                                    HttpContext.Current.Session("Piva"),
                                                    SaCod,
                                                    VegCod,
                                                    CulCod,
                                                    "",
                                                    0,
                                                    -1,
                                                    strDataFiltro,
                                                    " Cul_Des, App_Nome, Progetto ",
                                                    objParametriServer, True)

                        If Dt.Rows.Count = 0 Then
                            r.RispostaOK = False
                            r.RispostaStringa = "Nessun Impianto Selezionato Per La Stampa"
                            Return r
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
                        attrPiva.Value = HttpContext.Current.Session("Piva")
                        XML_FiltroStampa.Attributes.Append(attrPiva)

                        Dim attrsaCod As System.Xml.XmlAttribute = XmlDoc.CreateAttribute("sa_cod")
                        attrsaCod.Value = SaCod
                        XML_FiltroStampa.Attributes.Append(attrsaCod)

                        HttpContext.Current.Session("strXmlVariabilistampe") = XmlDoc.OuterXml
                    End If


            End Select

            ' controllo su numero sezioni da stampare selezionate (options) --> SEZIONI_SalvaSezioniSeleziontexStampa
            ' salva sezioni
            If (parametri.check_frontespizio) Then
                numSezioni += 1
                strSezioni += sezioni.check_frontespizio_value & ","
            End If
            If (parametri.check_personale) Then
                numSezioni += 1
                strSezioni += sezioni.check_personale_value & ","
            End If
            If (parametri.check_dati_catastali) Then
                numSezioni += 1
                strSezioni += sezioni.check_dati_catastali_value & ","
            End If
            If (parametri.check_semine) Then
                numSezioni += 1
                strSezioni += sezioni.check_semine_value & ","
            End If
            If (parametri.check_Fertilizzazioni) Then
                numSezioni += 1
                strSezioni += sezioni.check_Fertilizzazioni_value & ","
            End If
            If (parametri.check_trattamenti) Then
                numSezioni += 1
                strSezioni += sezioni.check_trattamenti_value & ","
            End If
            If (parametri.check_fitoregolatori) Then
                numSezioni += 1
                strSezioni += sezioni.check_fitoregolatori_value & ","
            End If
            If (parametri.check_fasi_fenologiche) Then
                numSezioni += 1
                strSezioni += sezioni.check_fasi_fenologiche_value & ","
            End If
            If (parametri.check_trappole) Then
                numSezioni += 1
                strSezioni += sezioni.check_trappole_value & ","
            End If
            If (parametri.check_ril_avver_trappole) Then
                numSezioni += 1
                strSezioni += sezioni.check_ril_avver_trappole_value & ","
            End If
            If (parametri.check_ril_avver_campo) Then
                numSezioni += 1
                strSezioni += sezioni.check_ril_avver_campo_value & ","
            End If
            If (parametri.check_irrigazione) Then
                numSezioni += 1
                strSezioni += sezioni.check_irrigazione_value & ","
            End If
            If (parametri.check_operazioni_colturali) Then
                numSezioni += 1
                strSezioni += sezioni.check_operazioni_colturali_value & ","
            End If
            If (parametri.check_ind_maturita) Then
                numSezioni += 1
                strSezioni += sezioni.check_ind_maturita_value & ","
            End If
            If (parametri.check_rilievo_prod) Then
                numSezioni += 1
                strSezioni += sezioni.check_rilievo_prod_value & ","
            End If
            If (parametri.check_piogge) Then
                numSezioni += 1
                strSezioni += sezioni.check_piogge_value & ","
            End If
            If (parametri.check_informazioni) Then
                numSezioni += 1
                strSezioni += sezioni.check_informazioni_value & ","
            End If
            If (parametri.check_manutenzione) Then
                numSezioni += 1
                strSezioni += sezioni.check_manutenzione_value & ","
            End If
            If (parametri.check_visite_ispettive) Then
                numSezioni += 1
                strSezioni += sezioni.check_visite_ispettive_value & ","
            End If
            If (parametri.check_trattamenti_post_raccolta) Then
                numSezioni += 1
                strSezioni += sezioni.check_trattamenti_post_raccolta_value
            End If
            If (parametri.check_formazione) Then
                numSezioni += 1
                strSezioni += sezioni.check_formazione_value & ","
            End If
            If (parametri.check_gestione_rifiuti) Then
                numSezioni += 1
                strSezioni += sezioni.check_gestione_rifiuti_value & ","
            End If
            If (parametri.check_pratiche_ecologiche) Then
                numSezioni += 1
                strSezioni += sezioni.check_pratiche_ecologiche_value & ","
            End If
            If (parametri.check_verifiche_conf) Then
                numSezioni += 1
                strSezioni += sezioni.check_verifiche_conf_value & ","
            End If

            If numSezioni = 0 AndAlso String.IsNullOrEmpty(parametri.schedaVeneto) Then
                r.RispostaOK = False
                r.RispostaStringa = "Selezionare almeno una Sezione!"
                Return r
            End If

            SEZIONI_SalvaSezioniSeleziontexStampa_New(strSezioni, numSezioni, "")

            r.RispostaOK = True
            r.RispostaStringa = "Parametri Corretti:"


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " '& vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function StampaNew(ByVal objParams As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim TargetURL As String
        Dim str_DataInizio As String
        Dim str_DataFine As String
        Dim str_DataStampa As String
        Dim strSezioni As String = ""
        Dim Report As Integer
'        Report = HttpContext.Current.Session("ReportSelezionato")



        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim Session = HttpContext.Current.Session
        Dim Server = HttpContext.Current.Server


        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim settingLoc As New JsonSerializerSettings With
            {
                .DateTimeZoneHandling = DateTimeZoneHandling.Local
            }

            'se non restituisce errore durante la deserializzazione del Json
            Dim parametri As Parametri_Controlli_Pre_StampaNew = JsonConvert.DeserializeObject(Of Parametri_Controlli_Pre_StampaNew)(objParams, settingLoc)
            Dim sezioni As Sezioni_Pre_StampaNew = JsonConvert.DeserializeObject(Of Sezioni_Pre_StampaNew)(objParams, settingLoc)
            Report = parametri.reportSelezionato

            Dim flag_organismoIntest As Boolean = parametri.checkImpostaOrganismoReferente
            Dim flag_reg_condizionalita As Boolean = parametri.checkRegCondizionalita
            Dim flag_reg_psr As Boolean = parametri.checkRegPSR
            Dim flag_reg_misura10 As Boolean = parametri.checkRegMisura10

            'controllo date
            If (parametri.txtValiditaInizio = Nothing) And (parametri.txtValiditaFine = Nothing) Then
                str_DataStampa = Format(parametri.txtStampaGiorno, "dd/MM/yyyy")
                str_DataInizio = ""
                str_DataFine = ""
            End If
            If (parametri.txtStampaGiorno = Nothing) Then
                str_DataInizio = Format(parametri.txtValiditaInizio, "dd/MM/yyyy")
                str_DataFine = Format(parametri.txtValiditaFine, "dd/MM/yyyy")
                str_DataStampa = ""
            End If

            'parametri stampa
            Dim stampa As Integer

            If (parametri.checkStampaProva) Then
                stampa = 0
            Else
                stampa = 1
            End If

            'If Me.DivRegolamenti.Visible = True Then
            '    flag_reg_condizionalita = Me.Chk_RegCondizionalita.Checked
            '    flag_reg_psr = Me.Chk_RegPSR.Checked
            '    flag_reg_misura10 = Me.Chk_RegMisura10.Checked
            'End If

            Dim tipo_ordinamento As Integer = parametri.ddlOrdinamento
            Dim anno_impianto_colt_pluriennali As Integer
            If (parametri.chkStampaAnnoImpiantoPluriennali) Then
                anno_impianto_colt_pluriennali = 1
            Else
                anno_impianto_colt_pluriennali = 0
            End If

            'If Me.DivOrdinamento.Visible = True Then
            '    tipo_ordinamento = Me.Rbl_Ordinamento.SelectedValue
            'End If

            If Report = enum_CodificaStampe.SchedaColturale_Biologico Then

                TargetURL = "../../Biologico/SchedaColturale/SchedaColturaleBiologico.aspx" &
                    "?dI=" &
                    Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
                    "&dF=" &
                    Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
                    "&dG=" &
                    Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
                    "&arr=" &
                    Stringa_Codifica(parametri.ddlArrotondamento.ToString, AgroKey_EncoderDecoder, Server) &
                    "&stDef=" &
                    Stringa_Codifica(stampa.ToString, AgroKey_EncoderDecoder, Server)

            Else

                TargetURL = "../SchedaCampagna.aspx" &
                        "?dI=" &
                        Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
                        "&dF=" &
                        Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
                        "&dG=" &
                        Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
                        "&arr=" &
                        Stringa_Codifica(parametri.ddlArrotondamento.ToString, AgroKey_EncoderDecoder, Server) &
                        "&stDef=" &
                        Stringa_Codifica(stampa.ToString, AgroKey_EncoderDecoder, Server) &
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
                        Stringa_Codifica(anno_impianto_colt_pluriennali.ToString, AgroKey_EncoderDecoder, Server)

            End If

            If (parametri.chkDataUltimaRaccolta) Then
                Session("DataUltimaRaccolta") = True
            Else
                Session("DataUltimaRaccolta") = False
            End If
            
            If parametri.chkAvversitaQta Then
                Session("AvversitaQta") = True
            Else
                Session("AvversitaQta") = False
            End If

            If (parametri.chkTutteRaccolte) = True Then
                Session("TutteRaccolte") = True
            Else
                Session("TutteRaccolte") = False
            End If

            If (parametri.chkQtaQtaRaccolte) = True Then
                Session("QtaRaccolte") = True
            Else
                Session("QtaRaccolte") = False
            End If

            If (parametri.chkRaggruppaXCampo) = True Then
                Session("RaggruppaXCampo") = True
            Else
                Session("RaggruppaXCampo") = False
            End If

            If (parametri.chkVisualizzaTipologieVarietali) = True Then
                Session("VisualizzaTipologieVarietali") = True
            Else
                Session("VisualizzaTipologieVarietali") = False
            End If

            If (parametri.chkVisualizzaCapitolatoPrivato) = True Then
                Session("CapitolatoPrivato") = True
            Else
                Session("CapitolatoPrivato") = False
            End If

            If (parametri.chkVisualizzaFinalita) = True Then
                Session("VisualizzaFinalita") = True
            Else
                Session("VisualizzaFinalita") = False
            End If

            If (parametri.chkVisualizzaAcquaHa) = True Then
                Session("VisualizzaAcquaHa") = True
            Else
                Session("VisualizzaAcquaHa") = False
            End If

            If (parametri.chkMostraValoriSignificativiNeiRilievi) = True Then
                Session("MostraValoriSignificativiNeiRilievi") = True
            Else
                Session("MostraValoriSignificativiNeiRilievi") = False
            End If

            If (parametri.chkPrioritaColturePrecedenti) = True Then
                Session("Priorita_ColturePrecedenti") = True
            Else
                Session("Priorita_ColturePrecedenti") = False
            End If

            If (parametri.checkMostraDataDiStampa) = True Then
                Session("MostraDataOdiernaStampa") = True
            Else
                Session("MostraDataOdiernaStampa") = False
            End If

            If (parametri.checkMostraFirmaODC) = True Then
                Session("MostraFirmaODC") = True
            Else
                Session("MostraFirmaODC") = False
            End If
            '            If (parametri.checkImpostaOrganismoReferente) = True Then
            '                Session("MostraFirmaODC") = True
            '            Else
            '                Session("MostraFirmaODC") = False
            '            End If

            If (parametri.checkSuperfici) = True Then
                Session("TipoSuperficie") = "i"
            Else
                Session("TipoSuperficie") = "a"
            End If


            Session("Regione_Selezionata") = parametri.ddlRegioni


            Session("Regione_Copertina") = parametri.ddlRegioni



            Session("sezioniVuote") = parametri.ElencoSezioniVuote

            Session("TempoRientro") = parametri.tempoRientro
            Session("ReportSelezionato") = Report


            'If Fascicoli.Visible = True AndAlso Cmb_Fascicoli.SelectedValue <> "0" Then
            '    Session("Fascicolo") = Cmb_Fascicoli.SelectedValue
            'End If
            If (parametri.checkGlobalGap) Then
                Session("TitoloGlobal") = True
            Else
                Session("TitoloGlobal") = False
            End If

            If (parametri.checkVisualizzaLotto) = True Then
                Session("VisualizzaLottoEsercizio") = True
            Else
                Session("VisualizzaLottoEsercizio") = False
            End If

            r.RispostaStringa = TargetURL
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Stampa_MagazziniNew(ByVal objParams As String) As RispostaStandard

        Const PaginaLinkStampaSchedaFertilizzantiMagazzino As String = "../Magazzino/Fertilizzanti/SchedaFertilizzantiMagazzino.aspx"
        Const PaginaLinkStampaSchedaProdottiFitosanitariMagazzino As String = "../Magazzino/ProdottiFitosanitari/SchedaProdottiFitosanitariMagazzino.aspx"

        Dim Str_elem_cod As String = ""
        Dim FlagVisualizzaComposizione As Integer = 0 '0=NON Stampa la composizione dei prodotti fitosanitari - 1=Stampa la composizione dei prodotti fitosanitari
        Dim CarichiScarichi As Integer = 0 '0=carichi e scarichi - 1=solo carichi - 2=solo scarichi
        Dim Ordinamento As Integer = 0 '0=data - 1=prodotto
        Dim StampaLotto As Integer = 1 '0=Stampa sempre il lotto - 1=Stampa in base alla configurazione del prodotto
        Dim Lotto = ""
        Dim Raggruppamento As Integer = 1 '0=Stampa raggruppata - 1=Stampa standard

        Dim r As New RispostaStandard
        Dim TargetURL As String = ""
        Dim str_DataInizio As String
        Dim str_DataFine As String
        Dim str_DataStampa As String
        Dim strSezioni As String = ""
        Dim Report As Integer
        Dim SchedaSelezionata As String = ""
'        Report = HttpContext.Current.Session("ReportSelezionato")
        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim Session = HttpContext.Current.Session
        Dim Server = HttpContext.Current.Server
        Dim numSezioni As Integer = 0
        Dim Elem_Cod As Integer = 0
        Dim Pro_Cod As Integer = 0
        Dim Mat_Cod As Integer = 0

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try
            Dim settingLoc As New JsonSerializerSettings With
            {
                .DateTimeZoneHandling = DateTimeZoneHandling.Local
            }

            'se non restituisce errore durante la deserializzazione del Json
            Dim parametri As Parametri_Controlli_Pre_StampaNew = JsonConvert.DeserializeObject(Of Parametri_Controlli_Pre_StampaNew)(objParams, settingLoc)
            Dim sezioni As Sezioni_Pre_StampaNew = JsonConvert.DeserializeObject(Of Sezioni_Pre_StampaNew)(objParams, settingLoc)
            Dim Tipo_Arrotondamento As Integer = parametri.ddlArrotondamento
            Report = parametri.reportSelezionato
            
            'controllo date
            If (parametri.txtValiditaInizio = Nothing) And (parametri.txtValiditaFine = Nothing) Then
                str_DataStampa = Format(parametri.txtStampaGiorno, "dd/MM/yyyy")
                str_DataInizio = ""
                str_DataFine = ""
            End If
            If (parametri.txtStampaGiorno = Nothing) Then
                str_DataInizio = Format(parametri.txtValiditaInizio, "dd/MM/yyyy")
                str_DataFine = Format(parametri.txtValiditaFine, "dd/MM/yyyy")
                str_DataStampa = ""
            End If


            If parametri.ddlMagazzino = "-1" Then
                'Messaggi.AgroMsgBox("Selezionare il Magazzino!",)
            End If
            Dim Piva As String = parametri.piva  'check
            Dim Sa_Cod As Integer = parametri.ddlMagazzino.Split("|")(1)
            Dim Fabbricato_Cod As Integer = parametri.ddlMagazzino.Split("|")(0)

            If (parametri.checkStampeMagazzino) Then
                TargetURL = "../" + PaginaLinkStampaSchedaFertilizzantiMagazzino
            Else
                TargetURL = "../" + PaginaLinkStampaSchedaProdottiFitosanitariMagazzino
            End If

            Dim ChkList_Categorie As New CheckBoxList
            AgronicaCoreUtility.CaricaListControl.CategorieMagazzino(ChkList_Categorie, False, "", "", 0, CAU_SCARICO, 0, False, False,
                                                             " (Tabella IN ('Materie_Prime','TipologieSementi') ) AND  Elem_Cod NOT IN (" + CStr(FARMACI) + ", " + CStr(MANGIMI) + ")",
                                                             "", HttpContext.Current.Session("ASG_objParametri_Server")) 'objParametri_Server)

            For Each item As ListItem In ChkList_Categorie.Items
                Str_elem_cod += item.Value + ","
            Next
            Str_elem_cod = "(" + Left(Str_elem_cod, Str_elem_cod.Length - 1) + ")"

            ''================================================================

            Dim QueryString As String = "?p=" + Stringa_Codifica(Piva, AgroKey_EncoderDecoder, Server) +
                                "&s=" + Stringa_Codifica(Sa_Cod, AgroKey_EncoderDecoder, Server) +
                                "&f=" + Stringa_Codifica(Fabbricato_Cod, AgroKey_EncoderDecoder, Server) +
                                "&e=" + Stringa_Codifica(Elem_Cod, AgroKey_EncoderDecoder, Server) +
                                "&pro=" + Stringa_Codifica(Pro_Cod, AgroKey_EncoderDecoder, Server) +
                                "&mat=" + Stringa_Codifica(Mat_Cod, AgroKey_EncoderDecoder, Server) +
                                "&arr=" + Stringa_Codifica(Tipo_Arrotondamento, AgroKey_EncoderDecoder, Server)

            If String.IsNullOrEmpty(str_DataInizio) AndAlso String.IsNullOrEmpty(str_DataFine) AndAlso Not String.IsNullOrEmpty(str_DataStampa) Then
                str_DataInizio = str_DataStampa
                str_DataFine = str_DataStampa
            End If

            QueryString += "&di=" + Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) +
                           "&df=" + Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) +
                           "&ord=" + Stringa_Codifica(Ordinamento, AgroKey_EncoderDecoder, Server) +
                           "&fvc=" + Stringa_Codifica(FlagVisualizzaComposizione, AgroKey_EncoderDecoder, Server)



            Session("Regione_Selezionata") = parametri.ddlRegioni
            Session("Regione_Copertina") = parametri.ddlRegioni
            
            r.RispostaStringa = String.Format("{0}{1}", TargetURL, QueryString)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Stampa_VenetoNew(ByVal objParams As String) As RispostaStandard

        Dim r As New RispostaStandard
        Dim TargetURL As String
        Dim str_DataInizio As String
        Dim str_DataFine As String
        Dim str_DataStampa As String
        Dim strSezioni As String = ""
        Dim Report As Integer
        Dim SchedaSelezionata As String = ""
'        Report = HttpContext.Current.Session("ReportSelezionato")

        Dim objParametriServer As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim Session = HttpContext.Current.Session
        Dim Server = HttpContext.Current.Server
        Dim numSezioni As Integer = 0

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametriServer) Then
            r.Sessione = False
            Return r
        End If

        Try

            Dim settingLoc As New JsonSerializerSettings With
            {
                .DateTimeZoneHandling = DateTimeZoneHandling.Local
            }

            'se non restituisce errore durante la deserializzazione del Json
            Dim parametri As Parametri_Controlli_Pre_StampaNew = JsonConvert.DeserializeObject(Of Parametri_Controlli_Pre_StampaNew)(objParams, settingLoc)
            Dim sezioni As Sezioni_Pre_StampaNew = JsonConvert.DeserializeObject(Of Sezioni_Pre_StampaNew)(objParams, settingLoc)

            Report = parametri.reportSelezionato
            'controllo date
            If (parametri.txtValiditaInizio = Nothing) And (parametri.txtValiditaFine = Nothing) Then
                str_DataStampa = Format(parametri.txtStampaGiorno, "dd/MM/yyyy")
                str_DataInizio = ""
                str_DataFine = ""
            End If
            If (parametri.txtStampaGiorno = Nothing) Then
                str_DataInizio = Format(parametri.txtValiditaInizio, "dd/MM/yyyy")
                str_DataFine = Format(parametri.txtValiditaFine, "dd/MM/yyyy")
                str_DataStampa = ""
            End If

            'parametri stampa
            Dim stampa As Integer

            If (parametri.checkStampaProva) Then
                stampa = 0
            Else
                stampa = 1
            End If

            Dim QueryString As String

            Session("Regione_Selezionata") = parametri.ddlRegioni
            Session("Regione_Copertina") = parametri.ddlRegioni

            If parametri.chkVisualizzaFinalita = True Then
                Session("VisualizzaFinalita") = True
            Else
                Session("VisualizzaFinalita") = False
            End If

            SchedaSelezionata = parametri.schedaVeneto

            SEZIONI_SalvaSezioniSeleziontexStampa_New(strSezioni, numSezioni, SchedaSelezionata)

            Select Case SchedaSelezionata
                Case "a", "b", "c", "d"
                    TargetURL = "../SchedaCampagna.aspx"
                    QueryString = "?dI=" &
                                Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
                                "&dF=" &
                                Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
                                "&dG=" &
                                Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
                                "&arr=" &
                                Stringa_Codifica(parametri.ddlArrotondamento.ToString, AgroKey_EncoderDecoder, Server) &
                                "&stDef=" &
                                Stringa_Codifica(stampa, AgroKey_EncoderDecoder, Server)

                    Session("sezioniVuote") = parametri.ElencoSezioniVuote
                    Session("TempoRientro") = parametri.tempoRientro

                Case "e"

                    TargetURL = "../SchedaTrattamentiTerzista.aspx"
                    QueryString = "?p=" &
                                Stringa_Codifica(CStr(Session("Piva")), AgroKey_EncoderDecoder, Server) +
                                "&dI=" &
                                Stringa_Codifica(str_DataInizio, AgroKey_EncoderDecoder, Server) &
                                "&dF=" &
                                Stringa_Codifica(str_DataFine, AgroKey_EncoderDecoder, Server) &
                                "&dG=" &
                                Stringa_Codifica(str_DataStampa, AgroKey_EncoderDecoder, Server) &
                                "&stDef=" &
                                Stringa_Codifica(stampa, AgroKey_EncoderDecoder, Server)

            End Select

            r.RispostaStringa = String.Format("{0}{1}", TargetURL, QueryString)
            r.RispostaOK = True

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function
    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Magazzini(ByVal piva As String,
                                                  ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Server As AgronicaCoreParametri = Nothing
        Dim clc = New AgronicaCoreUtility.CaricaListControl
        Try

            objParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

            Dim ddlMagazzini As New DropDownList()
            Dim listaMagazzini As New List(Of Object)


            '-------------------------------------------------------------------
            '29/08/2017 MarcoG: DDL DEI MAGAZZINI PER LE STAMPE DIRETTE
            clc.Fabbricati(ddlMagazzini, True, "Selezionare un Magazzino...", "-1", piva, 0, 0,
                                                             FABBRICATI_NO_STALLE, True, "", " Fabbricati.Fabbricato_Des ", AGRODATAFINE, objParametri_Server)



            For Each item As ListItem In ddlMagazzini.Items
                listaMagazzini.Add(
                        New With
                        {
                            .Sa_Cod = item.Value,
                            .Sa_Nome = item.Text
                        }
                    )
            Next





            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(listaMagazzini)


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function



    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Centri_Aziendali(ByVal piva As String,
                                                  ByVal listaSaCod As String,
                                                  ByVal strCodiciTerreno As String,
                                                  ByVal hashDestinazioniUso As String,
                                                  ByVal objP_server As String) As RispostaStandard

        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Server As AgronicaCoreParametri = Nothing

        Try

            objParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

            Dim ddl_CentroAziendale As New DropDownList()
            Dim listaCentriAziendali As New List(Of Object)
            Dim hashDestUso As Hashtable = JsonConvert.DeserializeObject(Of Hashtable)(hashDestinazioniUso)

            If listaSaCod <> "" Then
                AgronicaCoreUtility.CaricaListControl.Centri_Aziendali(ddl_CentroAziendale,
                                                                        True, "Tutti i Centri", "0",
                                                                        piva,
                                                                         False,
                                                                         2,
                                                                         " sa_cod IN (" & Left(listaSaCod, listaSaCod.Length - 1) & ") ", "",
                                                                            objParametri_Server)

                If strCodiciTerreno <> "" Then
                    strCodiciTerreno = Left(strCodiciTerreno, strCodiciTerreno.Length - 2)
                    For Each Key As Int32 In hashDestUso.Keys
                        Select Case Key
                            Case 0 'terreno nudo
                                ddl_CentroAziendale.Items.Add(New ListItem(hashDestUso.Item(Key), 0))
                            Case Else 'dest uso
                                ddl_CentroAziendale.Items.Add(New ListItem(hashDestUso.Item(Key), -Key))
                        End Select
                    Next
                End If

                For Each item As ListItem In ddl_CentroAziendale.Items
                    listaCentriAziendali.Add(
                        New With
                        {
                            .Sa_Cod = item.Value,
                            .Sa_Nome = item.Text
                        }
                    )
                Next

            End If



            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(listaCentriAziendali)


        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Specie_Vegetali(ByVal listaVegCod As String,
                                                 ByVal strCodiciTerreno As String,
                                                 ByVal hashDestinazioniUso As String,
                                                 ByVal objP_server As String,
                                                 ByVal objP_utenti As String) As RispostaStandard
        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Dim objParametri_Server As AgronicaCoreParametri = Nothing
        Dim objParametri_Utenti As AgronicaCoreParametri = Nothing

        Try

            objParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)
            objParametri_Utenti = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_utenti)

            Dim listaSpecieVegetali As New List(Of Object)
            Dim ddl_SpecieVegetale As New DropDownList()
            Dim hashDestUso As Hashtable = JsonConvert.DeserializeObject(Of Hashtable)(hashDestinazioniUso)

            If listaVegCod <> "" Then
                AgronicaCoreUtility.CaricaListControl.SpecieVegetale_Optimize(ddl_SpecieVegetale,
                                                        False, "", "0",
                                                        0,
                                                        "", True, 0, 0, 0,
                                                        " SpecieVegetali.veg_cod IN (" & Left(listaVegCod, listaVegCod.Length - 1) & ") ", "",
                                                        objParametri_Server, objParametri_Utenti)

                If strCodiciTerreno <> "" Then
                    strCodiciTerreno = Left(strCodiciTerreno, strCodiciTerreno.Length - 2)
                    For Each Key As Int32 In hashDestUso.Keys
                        Select Case Key
                            Case 0 'terreno nudo
                                ddl_SpecieVegetale.Items.Add(New ListItem(hashDestUso.Item(Key), 0))
                            Case Else 'dest uso
                                ddl_SpecieVegetale.Items.Add(New ListItem(hashDestUso.Item(Key), -Key))
                        End Select
                    Next
                End If

                For Each item As ListItem In ddl_SpecieVegetale.Items
                    listaSpecieVegetali.Add(
                        New With
                        {
                            .Sv_Cod = item.Value,
                            .Sv_Nome = item.Text
                        }
                    )
                Next


            End If

            r.RispostaOK = True
            r.RispostaStringa = JsonConvert.SerializeObject(listaSpecieVegetali)

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    Public Shared Function Leggi_Regione_Appartenenza(
                                                 ByVal piva As String,
                                                 ByVal sa_cod As Integer,
                                                 ByVal objP_server As String
                                                 ) As RispostaStandard

        Dim r As New RispostaStandard
        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            r.Sessione = False
            Return r
        End If
        Try
            Dim objParametri_Server As AgronicaCoreParametri = Nothing
            objParametri_Server = AgronicaCoreDataProvider.Utility.convertStringtoOBJparametri(objP_server)

            Dim ProCod As String = String.Empty
            Dim ProCodIstat As String = String.Empty
            Dim RegioneCod As String = String.Empty

            Dim objInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
            objInd.Indirizzo_from_PivaSaCod(piva, sa_cod,
                                        Nothing, Nothing,
                                        Nothing, Nothing,
                                        Nothing, ProCod, Nothing,
                                        ProCodIstat, Nothing,
                                        RegioneCod, objParametri_Server)
            r.RispostaOK = True
            r.RispostaStringa = RegioneCod

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)
        End Try
        Return r
    End Function


    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Session("Matrice_Variabili") = Nothing

        Dim strJS1 As New StringBuilder
        strJS1.AppendLine("$(document).ready(function () { ")
        strJS1.AppendLine("      window.close() ")
        strJS1.AppendLine(" });")

        ScriptManager.RegisterStartupScript(Me.Page, Me.Page.GetType(),
                                  String.Format("jQuery_{0}", Me.Page.ClientID), strJS1.ToString, True)

    End Sub


    Private Sub SEZIONI_AggiungiSezioniConPermessi(ByVal report As enum_CodificaStampe)

        Dim sezioniSuPermessi As New List(Of String)

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
            sezioniSuPermessi.Add("u")
            Dim checkPraticheEco As ListItem = New ListItem("Pratiche Ecologiche", "u")
            checkPraticheEco.Selected = True
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
            sezioniSuPermessi.Add("w")
            Dim CheckList_Formazione As ListItem = New ListItem("Formazione", "w")
            CheckList_Formazione.Selected = True
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
            sezioniSuPermessi.Add("x")
            Dim Gestione_Rifiuti As ListItem = New ListItem("Gestione Rifiuti", "x")
            Gestione_Rifiuti.Selected = True
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
            sezioniSuPermessi.Add("v")
            Dim checkConformita As ListItem = New ListItem("Verifiche Conformita", "v")
            checkConformita.Selected = False
        End If

        hd_ElencoSezioneSuPermessi.Value = String.Join(",", sezioniSuPermessi)

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

            Case enum_CodificaStampe.SchedaCampagna_Multicentro_ACA

                elencoReportAbilitati = ElencoReport_SchedaCampagna_Multicentro_ACA
 
            Case Else
 
                elencoReportAbilitati = Nothing

        End Select

        If Not IsNothing(elencoReportAbilitati) AndAlso elencoReportAbilitati.Count > 0 Then
            hd_ElencoReportAbilitati.Value = String.Join(",", elencoReportAbilitati)
        Else
            hd_ElencoReportAbilitati.Value = ""
        End If


    End Sub
    'Private Shared Sub InserisciPrecessioniColturali(ByRef Dt As DataTable, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

    '    'Dim DtCodici As New DataTable
    '    Dim DrCodici() As DataRow

    '    Dim Inizio As String
    '    Dim Fine As String
    '    Dim Num_giorni As Long
    '    Dim Num_ColturePrecedenti As Integer
    '    Dim stbQueryColture As New System.Text.StringBuilder
    '    Dim Reg_Impianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read
    '    Dim Reg_Impianti_Codici_R As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
    '    Dim Appezza_Codici_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
    '    Dim Campo_Codici_R As New AgronicaCoreAnagrafeDAL.Campi_Codici_Read
    '    Dim objSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
    '    Dim objSql As New AgronicaCoreDataProvider.DataProvider
    '    For Each Ro As DataRow In Dt.Rows
    '        If String.IsNullOrEmpty(Ro("Coltura1")) Or String.IsNullOrEmpty(Ro("Coltura2")) Or String.IsNullOrEmpty(Ro("Coltura3")) Or String.IsNullOrEmpty(Ro("Coltura4")) Then

    '            Dim Rs As DataTable = Reg_Impianti.LeggiBaseCulture(Ro("piva"), Ro("sa_cod"), Ro("APPEZZA"), Ro("Validita_inizio_Impianto"), objParametri)


    '            If Not IsNothing(Rs) Then


    '                'se ho impianti precedenti carico questi...
    '                '1.	valorizzo le colture precedenti in base agli impianti precedenti inseriti nello stesso appezzamento 
    '                '(dando per scontato che non ci siano dei buchi tra gli anni e non considerando i terreni nudi con durata minore ai 6 mesi).
    '                If Rs.Rows.Count > 0 Then

    '                    Num_ColturePrecedenti = Rs.Rows.Count


    '                    For n = 0 To Num_ColturePrecedenti - 1

    '                        Select Case n

    '                            Case 0
    '                                If Not IsDBNull(Rs.Rows(n).Item("Veg_Cod")) And Ro("Coltura1") = "" Then
    '                                    Ro("Coltura1") = Rs.Rows(n).Item("Veg_Cod")
    '                                ElseIf Ro("Coltura1") = "" Then
    '                                    'se c'è un terreno nudo lo faccio vedere solo se 
    '                                    'la sua durata supera i sei mesi!!!
    '                                    Fine = Rs.Rows(n).Item("Validita_Fine")
    '                                    Inizio = Rs.Rows(n).Item("Validita_Inizio")
    '                                    Num_giorni = DateDiff(DateInterval.Day, CDate(Inizio), CDate(Fine))
    '                                    If Num_giorni > 180 Then
    '                                        Ro("Coltura1") = "Terreno Nudo"
    '                                    End If
    '                                End If

    '                            Case 1
    '                                If Not IsDBNull(Rs.Rows(n).Item("Veg_Cod")) And Ro("Coltura2") = "" Then
    '                                    Ro("Coltura2") = Rs.Rows(n).Item("Veg_Cod")
    '                                ElseIf Ro("Coltura2") = "" Then
    '                                    'se c'è un terreno nudo lo faccio vedere solo se 
    '                                    'la sua durata supera i sei mesi!!!
    '                                    Fine = Rs.Rows(n).Item("Validita_Fine")
    '                                    Inizio = Rs.Rows(n).Item("Validita_Inizio")
    '                                    Num_giorni = DateDiff(DateInterval.Day, CDate(Inizio), CDate(Fine))
    '                                    If Num_giorni > 180 Then
    '                                        Ro("Coltura2") = "Terreno Nudo"
    '                                    End If
    '                                End If

    '                            Case 2
    '                                If Not IsDBNull(Rs.Rows(n).Item("Veg_Cod")) And Ro("Coltura3") = "" Then
    '                                    Ro("Coltura3") = Rs.Rows(n).Item("Veg_Cod")
    '                                ElseIf Ro("Coltura3") = "" Then
    '                                    'se c'è un terreno nudo lo faccio vedere solo se 
    '                                    'la sua durata supera i sei mesi!!!
    '                                    Fine = Rs.Rows(n).Item("Validita_Fine")
    '                                    Inizio = Rs.Rows(n).Item("Validita_Inizio")
    '                                    Num_giorni = DateDiff(DateInterval.Day, CDate(Inizio), CDate(Fine))
    '                                    If Num_giorni > 180 Then
    '                                        Ro("Coltura3") = "Terreno Nudo"
    '                                    End If
    '                                End If

    '                            Case 3
    '                                If Not IsDBNull(Rs.Rows(n).Item("Veg_Cod")) And Ro("Coltura4") = "" Then
    '                                    Ro("Coltura4") = Rs.Rows(n).Item("Veg_Cod")
    '                                ElseIf Ro("Coltura4") = "" Then
    '                                    'se c'è un terreno nudo lo faccio vedere solo se 
    '                                    'la sua durata supera i sei mesi!!!
    '                                    Fine = Rs.Rows(n).Item("Validita_Fine")
    '                                    Inizio = Rs.Rows(n).Item("Validita_Inizio")
    '                                    Num_giorni = DateDiff(DateInterval.Day, CDate(Inizio), CDate(Fine))
    '                                    If Num_giorni > 180 Then
    '                                        Ro("Coltura4") = "Terreno Nudo"
    '                                    End If
    '                                End If
    '                        End Select



    '                    Next
    '                End If

    '                '2.	Vado a prelevare con il seguente ordine: APPEZZAMENTO, CAMPO le precessioni salvate dall'anagrafica

    '                If Ro("Coltura1") = "" Or Ro("Coltura2") = "" Or Ro("Coltura3") = "" Or Ro("Coltura4") = "" Then

    '                    'Dim ObjCOM_R As Object 'New Agro_Anagrafe_AD.Reg_Impianti_Codici_R
    '                    'ObjCOM_R = Server.CreateCANCELLATOObject("Agro_Anagrafe_AD.Reg_Impianti_Codici_R")

    '                    'leggo se sono state salvate le colture precedenti
    '                    Rs = Appezza_Codici_R.Leggi(CStr(Ro("piva")),
    '                                        CInt(Ro("sa_cod")),
    '                                        CInt(Ro("APPEZZA")),
    '                                        0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

    '                    If Not IsNothing(Rs) AndAlso Rs.Rows.Count <> 0 Then

    '                        'DtCodici = objSql.CreateDT_from_RS(Rs, strErr)

    '                        If Ro("Coltura1") = "" Then
    '                            'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_1)
    '                            DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_1)
    '                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
    '                                Ro("Coltura1") = objSpecie.VegDes_from_VegCod(CInt(DrCodici(0).Item("val_cod").ToString.Split("|")(0)), objParametri)
    '                            End If
    '                        End If


    '                        If Ro("Coltura2") = "" Then
    '                            'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_2)
    '                            DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_2)
    '                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
    '                                Ro("Coltura2") = objSpecie.VegDes_from_VegCod(CInt(DrCodici(0).Item("val_cod").ToString.Split("|")(0)), objParametri)
    '                            End If
    '                        End If


    '                        If Ro("Coltura3") = "" Then
    '                            'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_3)
    '                            DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_3)
    '                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
    '                                Ro("Coltura3") = objSpecie.VegDes_from_VegCod(CInt(DrCodici(0).Item("val_cod").ToString.Split("|")(0)), objParametri)
    '                            End If
    '                        End If


    '                        If Ro("Coltura4") = "" Then
    '                            'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_4)
    '                            DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Appezzamento_Precedente_4)
    '                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
    '                                Ro("Coltura4") = objSpecie.VegDes_from_VegCod(CInt(DrCodici(0).Item("val_cod").ToString.Split("|")(0)), objParametri)
    '                            End If
    '                        End If

    '                    End If

    '                End If

    '                If (Ro("Coltura1") = "" Or Ro("Coltura2") = "" Or Ro("Coltura3") = "" Or Ro("Coltura4") = "") AndAlso Ro("Campo_Cod") <> 0 Then

    '                    'Dim ObjCOM_R As Object 'New Agro_Anagrafe_AD.Reg_Impianti_Codici_R
    '                    'ObjCOM_R = Server.CreateCANCELLATOObject("Agro_Anagrafe_AD.Reg_Impianti_Codici_R")

    '                    'leggo se sono state salvate le colture precedenti
    '                    Rs = Campo_Codici_R.Leggi(CStr(Ro("piva")),
    '                                        CInt(Ro("sa_cod")),
    '                                        Ro("Campo_Cod"),
    '                                        0, "", AGRODATAINIZIO, AGRODATAFINE, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

    '                    If Not IsNothing(Rs) AndAlso Rs.Rows.Count <> 0 Then

    '                        'DtCodici = objSql.CreateDT_from_RS(Rs, strErr)

    '                        If Ro("Coltura1") = "" Then
    '                            'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_1)
    '                            DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Campo_Precedente_1)
    '                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
    '                                Ro("Coltura1") = objSpecie.VegDes_from_VegCod(CInt(DrCodici(0).Item("val_cod").ToString.Split("|")(0)), objParametri)
    '                            End If
    '                        End If


    '                        If Ro("Coltura2") = "" Then
    '                            'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_2)
    '                            DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Campo_Precedente_2)
    '                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
    '                                Ro("Coltura2") = objSpecie.VegDes_from_VegCod(CInt(DrCodici(0).Item("val_cod").ToString.Split("|")(0)), objParametri)
    '                            End If
    '                        End If


    '                        If Ro("Coltura3") = "" Then
    '                            'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_3)
    '                            DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Campo_Precedente_3)
    '                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
    '                                Ro("Coltura3") = objSpecie.VegDes_from_VegCod(CInt(DrCodici(0).Item("val_cod").ToString.Split("|")(0)), objParametri)
    '                            End If
    '                        End If


    '                        If Ro("Coltura4") = "" Then
    '                            'DrCodici = DtCodici.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Precedente_4)
    '                            DrCodici = Rs.Select("id_cod=" & enum_CodiciAnagrafe.Coltura_Campo_Precedente_4)
    '                            If Not DrCodici Is Nothing AndAlso DrCodici.Length > 0 Then
    '                                Ro("Coltura4") = objSpecie.VegDes_from_VegCod(CInt(DrCodici(0).Item("val_cod").ToString.Split("|")(0)), objParametri)
    '                            End If
    '                        End If

    '                    End If

    '                End If



    '            End If
    '        End If
    '    Next


    'End Sub


    Public Class Parametri_Controlli_Pre_StampaNew

        'MAPPATURA CONTROLLI
        Public check_frontespizio As Boolean
        Public check_personale As Boolean
        Public check_dati_catastali As Boolean
        Public check_semine As Boolean
        Public check_Fertilizzazioni As Boolean
        Public check_trattamenti As Boolean
        Public check_fitoregolatori As Boolean
        Public check_fasi_fenologiche As Boolean
        Public check_trappole As Boolean
        Public check_ril_avver_trappole As Boolean
        Public check_ril_avver_campo As Boolean
        Public check_irrigazione As Boolean
        Public check_operazioni_colturali As Boolean
        Public check_ind_maturita As Boolean
        Public check_rilievo_prod As Boolean
        Public check_piogge As Boolean
        Public check_informazioni As Boolean
        Public check_manutenzione As Boolean
        Public check_visite_ispettive As Boolean
        Public check_trattamenti_post_raccolta As Boolean
Public check_formazione As Boolean
        Public check_gestione_rifiuti As Boolean
Public check_pratiche_ecologiche As Boolean
        Public check_verifiche_conf As Boolean
        Public chkAvversitaQta As Boolean
        Public chkTutteRaccolte As Boolean
        Public chkQtaQtaRaccolte As Boolean
        Public chkDataUltimaRaccolta As Boolean
        Public checkGlobalGap As Boolean
        Public chkPrioritaColturePrecedenti As Boolean
        Public chkVisualizzaTipologieVarietali As Boolean
        Public chkVisualizzaCapitolatoPrivato As Boolean
        Public chkVisualizzaFinalita As Boolean
        Public chkRaggruppaXCampo As Boolean
        Public chkVisualizzaAcquaHa As Boolean
        Public chkStampaAnnoImpiantoPluriennali As Boolean
        Public chkMostraValoriSignificativiNeiRilievi As Boolean
        Public checkSuperfici As Boolean
        Public checkData As Boolean
        Public checkStampaProva As Boolean
        Public checkStampeMagazzino As Boolean
        Public reportSelezionato As Integer
        Public ddlRegioni As String
        Public ddlArrotondamento As Integer
        Public txtValiditaInizio As DateTime
        Public txtValiditaFine As DateTime
        Public txtStampaGiorno As DateTime
        Public schedaVeneto As String
        Public tempoRientro As Integer
        Public ddlCentroAziendale As String
        Public ddlSpecieVegetale As String
        Public checkVisualizzaLotto As Boolean
        Public checkMostraFirmaODC As Boolean
        Public checkImpostaOrganismoReferente As Boolean
        Public checkMostraDataDiStampa As Boolean
        Public checkRegCondizionalita As Boolean
        Public checkRegPSR As Boolean
        Public checkRegMisura10 As Boolean
        'Magazzini
        Public ddlMagazzino As String
        Public ddlOrdinamento As String
        Public piva As String
        Public UtilizzaImpiantiFiltrati As String

        Public ElencoSezioniVuote As String

        Public Sub New()

        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class
    Public Class Sezioni_Pre_StampaNew
        'MAPPATURA SEZIONI
        Public check_frontespizio_value As String
        Public check_personale_value As String
        Public check_dati_catastali_value As String
        Public check_semine_value As String
        Public check_Fertilizzazioni_value As String
        Public check_trattamenti_value As String
        Public check_fitoregolatori_value As String
        Public check_fasi_fenologiche_value As String
        Public check_trappole_value As String
        Public check_ril_avver_trappole_value As String
        Public check_ril_avver_campo_value As String
        Public check_irrigazione_value As String
        Public check_operazioni_colturali_value As String
        Public check_ind_maturita_value As String
        Public check_rilievo_prod_value As String
        Public check_piogge_value As String
        Public check_informazioni_value As String
        Public check_manutenzione_value As String
Public check_formazione_value As String
        Public check_gestione_rifiuti_value As String
        Public check_visite_ispettive_value As String
Public check_pratiche_ecologiche_value As String
        Public check_verifiche_conf_value As String
        Public check_trattamenti_post_raccolta_value As String
    End Class

#Region "Modelli per Rendering GUI"
    Private Class CulturePrecedentiRow

        Public KeyReg As String
        Public Sa_Nome As String
        Public Piva As String
        Public sa_cod As String
        Public Appezza As String
        Public Id_Reg As String

        Public Campo_Des As String
        Public App_Nome As String
        Public Veg_Des As String
        Public Cul_Des As String
        Public Dest_Uso As String
        Public Sup_Imp As String
        Public Validita As String
        Public Coltura1 As String
        Public Coltura2 As String
        Public Coltura3 As String
        Public Coltura4 As String
        Public CodColtura1 As String
        Public CodColtura2 As String
        Public CodColtura3 As String
        Public CodColtura4 As String

    End Class

    Private Class ParametriSalvaModel


        Public RigheInserite As String
        Public RigheModificate As String
        Public RigheCancellate As String

    End Class
    Private Class ParametriDownloadElencoReport


        Public NomeFile As String
        Public NomeSottoCartella As String


    End Class

#End Region

End Class


