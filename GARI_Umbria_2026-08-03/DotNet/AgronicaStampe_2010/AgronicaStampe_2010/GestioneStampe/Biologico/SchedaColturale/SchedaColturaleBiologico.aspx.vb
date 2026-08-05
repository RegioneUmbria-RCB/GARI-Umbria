Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.My.Resources
Imports System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports CrystalDecisions.Shared

Public Class SchedaColturaleBiologico

    Inherits System.Web.UI.Page
    Private rptStampa As Rpt_SchedaColturaleBiologico
    Private rptFooterLogo As FooterLogo
    Private DSSchedaColturaleBiologico As DS_SchedaColturaleBiologico

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim QS_Anno As String
    Dim QS_DataDa As String
    Dim QS_DataA As String
    Dim Qs_Arrotondamento As Integer
    Dim Qs_StampaDefinitiva As String

    Dim Qs_DataStampa As String

    Dim BaseCode As Integer

    Dim strSa_Cod As String
    Dim strSa_Nome As String
    Dim strAppezza As String
    Dim strId_Reg As String
    Dim StrLav_Cod As String
    Dim ArraySezioni() As String

    Dim strFiltroAppezza As String
    Dim strFiltroImpianti As String
    Dim strFiltroImpianto As String

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim customLoghi As PersonalizzazioniGraficheCliente = Nothing

    Dim sql2017 As Boolean = True

#Region " Codice generato da Progettazione Web Form "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
        'istanzio l'oggetto report
        rptStampa = New Rpt_SchedaColturaleBiologico
        rptFooterLogo = New FooterLogo
    End Sub

#End Region

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Dim flagNascondiResponsabile As Boolean = False
        Dim flagNascondiOperatori As Boolean = False
        Dim flagNascondiMacchine As Boolean = False
        Dim flagNascondiDataFirma As Boolean = False
        Dim flagNascondiDataUltimaManutenzione As Boolean = False
        Dim TipoCodiceAppezzamento As Integer = 0
        '  Galassi, 01/03/2017 09.14.22: 
        'mostra il campo
        Dim flagVisualizzaCampo As Boolean = False
        'mostra quello specifico del bio
        Dim flagNascondiAppezzamentoBIO As Boolean = False
        'nasconde il codice solito
        Dim flagNascondinumeroAppezzamneto As Boolean = False
        Dim flagVisualizzaLottoSemine As Boolean = False
        Dim flagVisualizzaLottoRaccolte As Boolean = False

        Dim rag_soc As String = ""
        Dim sa_nome As String = ""
        Dim ind_des As String = ""
        Dim frz_des As String = ""
        Dim CAP As String = ""
        Dim com_des As String = ""
        Dim pro_cod As String = ""
        Dim pro_cod_istat As String = ""
        Dim com_cod_istat As String = ""
        Dim OrganismoControllo As String = ""

        Dim Log_Errori As String
        Dim Nome_Documento As String = "SchedaColturaleBiologico"
        Dim NomeFilePDF As String = ""


        '##############################################################
        '#####  Recupero le date dalla querystring           ##########
        '##############################################################

        Qs_Arrotondamento = CInt(Stringa_Decodifica(Request.QueryString("arr").ToString,
                                  AgroKey_EncoderDecoder,
                                  Server))

        ' se è una stampa definitiva salvo il pdf e visualizzo l'anteprima, altrimenti visualizzo solo l'anteprima
        If Not IsNothing(Request.QueryString("stDef")) AndAlso Stringa_Decodifica(Request.QueryString("stDef").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server) <> "" Then
            Qs_StampaDefinitiva = Stringa_Decodifica(Request.QueryString("stDef").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)
        Else
            Qs_StampaDefinitiva = "0"       ' stampa di prova
        End If



        QS_DataDa = Stringa_Decodifica(Request.QueryString("dI").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)

        QS_DataA = Stringa_Decodifica(Request.QueryString("dF").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)

        Qs_DataStampa = Stringa_Decodifica(Request.QueryString("dG").ToString,
                                        AgroKey_EncoderDecoder,
                                        Server)

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)




        Try
            Dim objDataProvider As New AgronicaCoreDataProvider.DataProvider
            Dim major As Integer = objDataProvider.VersioneSqlServer_Major(objParametri_Server)
            If major >= 14 Then
                sql2017 = True 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            Else
                sql2017 = False
            End If

            ' ----------------------------------------------
            ' Aggiungo Le impostazioni utente
            ' 
            ' ----------------------------------------------

            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DtImpostazioni As DataTable

            DtImpostazioni = objUtenti.Leggi(0, 1,
                                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                             "", "", objParametri_Utenti)
            Dim DrFiltro() As DataRow

            ' questo filtro lo leggo sempre
            DrFiltro = DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_SCHEDA_CAMPAGNA_BIO_NUM_APPEZZAMENTO)
            If Not DrFiltro Is Nothing AndAlso DrFiltro.Length > 0 Then
                If Not IsDBNull(DrFiltro(0).Item("Impostazione_Valore_1")) AndAlso DrFiltro(0).Item("Impostazione_Valore_1") <> "" Then
                    TipoCodiceAppezzamento = DrFiltro(0).Item("Impostazione_Valore_1")
                End If
            End If

            '  Galassi, 01/03/2017 09.09.26: Inserite le impostazioni BIO
            'USO LE STESSE DELLA SCHEDA DI CAMPAGNA

            '  Galassi, 01/03/2017 09.09.41: USO LE IMPOSTAZIONI BIO
            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_TECNICO_AUTORIZZANTE).Length > 0 Then
                flagNascondiResponsabile = True
            End If

            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_OPERATORI).Length > 0 Then
                flagNascondiOperatori = True
            End If

            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_MACCHINE).Length > 0 Then
                flagNascondiMacchine = True
            End If

            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_DATA_FIRMA).Length > 0 Then
                flagNascondiDataFirma = True
            End If

            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_DATA_ULTIMA_MANUTENZIONE).Length > 0 Then
                flagNascondiDataUltimaManutenzione = True
            End If

            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_CAMPO).Length > 0 Then
                flagVisualizzaCampo = True
            End If

            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_NUM_APPEZZAMENTO).Length > 0 Then
                flagNascondiAppezzamentoBIO = True
            End If

            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_SCHEDA_BIO_NUM_APPEZZAMENTO_BIO).Length > 0 Then
                flagNascondinumeroAppezzamneto = True
            End If

            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.Utente_SchedaColtBio_VisualizzaLottoSemine).Length > 0 Then
                flagVisualizzaLottoSemine = True
            End If

            If DtImpostazioni.Select("Impostazione_Cod = " & AgronicaCoreDataProvider.TipiEnumerativi.enum_Impostazioni_Utenti.Utente_SchedaColtBio_VisualizzaLottoRaccolte).Length > 0 Then
                flagVisualizzaLottoRaccolte = True
            End If



            '##############################################################
            '#####  Recupero piva, sa_cod e anno dalla stringa xml
            '##############################################################

            Dim strXmlVariabilistampe As String
            Dim htVariabiliStampe As System.Collections.Hashtable
            Dim strErr As String

            Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
            Dim XML_VariabiliStampe As System.Xml.XmlElement

            Dim XmlDoc As New System.Xml.XmlDocument
            Dim XML_FiltroStampa As System.Xml.XmlElement

            Dim Imprese_Codici_Read As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

            Dim MessaggioErrore As String

            Dim Matrice_Variabili(0, 0) As String

            Dim j As Integer

            strXmlVariabilistampe = Session("strXmlVariabilistampe")

            'Carico la stringa xml in un nuovo documento
            XmlDoc = New System.Xml.XmlDocument
            XmlDoc.LoadXml(strXmlVariabilistampe)

            If XmlDoc.HasChildNodes Then

                'XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

                XML_FiltroStampa = XmlDoc.SelectSingleNode("ParametriAgronicaStampe_2010")

                'Ricavo i parametri che servono

                Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")

                XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

                ReDim Matrice_Variabili(XMLs_VariabiliStampe.Count - 1, 4)

                strFiltroAppezza = String.Join(" OR ",
                    XMLs_VariabiliStampe.OfType(Of System.Xml.XmlElement) _
                    .Select(Function(n) $"( mov_dest_ric.PIVA ='{n.GetAttribute("piva")}' " +
                        $" AND mov_dest_ric.SA_COD={n.GetAttribute("sa_cod")}" +
                        $" AND mov_dest_ric.APPEZZA={n.GetAttribute("appezza")}" +
                        $" AND mov_dest_ric.Id_Destinazione={n.GetAttribute("id_reg")}" +
                        $" ) {vbCrLf}"))

                For j = 0 To XMLs_VariabiliStampe.Count - 1

                    XML_VariabiliStampe = XMLs_VariabiliStampe.Item(j)

                    Matrice_Variabili(j, 0) = XML_VariabiliStampe.GetAttribute("piva")
                    Matrice_Variabili(j, 1) = XML_VariabiliStampe.GetAttribute("sa_cod")
                    Matrice_Variabili(j, 2) = XML_VariabiliStampe.GetAttribute("appezza")
                    Matrice_Variabili(j, 3) = XML_VariabiliStampe.GetAttribute("id_reg")
                    Matrice_Variabili(j, 4) = XML_VariabiliStampe.GetAttribute("veg_cod")

                    'se appezza=0 e id_reg=0 la stampa riguarda tutti gli impianti del centro
                    If Matrice_Variabili(j, 2) <> "0" And Matrice_Variabili(j, 3) <> "0" Then

                        If InStr(strSa_Cod, Matrice_Variabili(j, 1)) = 0 Then

                            strSa_Cod = strSa_Cod & Matrice_Variabili(j, 1) & ","
                            strSa_Nome = strSa_Nome & objCentriAz.SaNome_from_SaCod(CStr(Matrice_Variabili(j, 0)), CInt(Matrice_Variabili(j, 1)), objParametri_Server) & ", "

                        End If

                        strFiltroImpianto = " (Reg_Impianti.PIVA='" + XML_VariabiliStampe.GetAttribute("piva") + "' " +
                        " AND Reg_Impianti.SA_COD=" + XML_VariabiliStampe.GetAttribute("sa_cod") +
                        " AND Reg_Impianti.APPEZZA=" + XML_VariabiliStampe.GetAttribute("appezza") +
                        " AND Reg_Impianti.ID_REG=" + XML_VariabiliStampe.GetAttribute("id_reg") +
                        " ) OR"

                        strFiltroImpianti = strFiltroImpianti + strFiltroImpianto

                        strAppezza = strAppezza & Matrice_Variabili(j, 2) & ","
                        strId_Reg = strId_Reg & Matrice_Variabili(j, 3) & ","

                    End If


                Next

                'elimino l'ultima virgola
                If strSa_Cod <> "" Then
                    strSa_Cod = Left(strSa_Cod, strSa_Cod.Length - 1)
                    strSa_Nome = Left(strSa_Nome, strSa_Nome.Length - 2)
                End If
                If strAppezza <> "" Then
                    strAppezza = Left(strAppezza, strAppezza.Length - 1)
                End If
                If strId_Reg <> "" Then
                    strId_Reg = Left(strId_Reg, strId_Reg.Length - 1)
                End If

                'tolgo l'ultimo OR
                strFiltroImpianti = Left(strFiltroImpianti, strFiltroImpianti.Length - 2)

                strFiltroImpianti = " AND (" + strFiltroImpianti + ")"

                'Per ora prendo piva, sa_cod del primo impianto 
                'poichè la scheda è stampabile su 1 Impresa (1 centro) 
                Qs_Piva = Matrice_Variabili(0, 0)


            End If


            '---------------------------------------------------------------------------------------------------
            '--- MODIFICA 13/05/2013
            '--- La versione precedente di Session("sezioni") era del tipo "1,2,3,4,5,6"
            '--- Adesso invece arriva una stringa del tipo "a,b,c,d,e,f,g"
            '--- Inoltre le sezioni "p", "g" ed "n" non erano contemplate
            '---------------------------------------------------------------------------------------------------



            'Public Const STR_OP_NON_GESTITE As String = " (  GruppoOperazioni.Tipo IN ('C','Z','P','E','V') 
            '                                  AND GruppoOperazioni.Gru_Cod not in ( 5,7,8,9,11 ) 
            '                                  AND Operazioni.Lav_Cod NOT IN (1021,1004,1005,1006,1007,1026,1027,1028,1029,1030,1032,1050,1051,1052,1053,1054,1055,1056,1057,1058,1060,1061,1062,1063,1064,1065,1066,1067,1068,1069,1070,1071,1072,1073,1074,1075,1076,1077,1078,30,3005,3006,3007,3008,3009,3010,3011,3012,3013,3014,3015,3016,3017,3018,3019,3024,3025,3026,3027,3028,3029,3031,3032,3022,3021,30233) ) "

            'Ricavo a seconda delle sezioni scelte nel filtro le operazioni che voglio stampare
            If Not Session("sezioni") Is Nothing Then

                ArraySezioni = Split(Session("sezioni"), ",")

                'Const FERTILIZZAZIONI As String = "b"
                'Const TRATTAMENTI As String = "c"
                'Const FASI_FENOLOGICHE As String = "d"
                'Const TRAPPOLE_INSTALLATE As String = "e"
                'Const RIL_AVVERS_NELLE_TRAPP As String = "f"
                'Const RIL_AVVERS_IN_CAMPO As String = "g"
                'Const IRRIGAZIONE As String = "h"
                'Const ALTRE_OPER_COLTURALI As String = "i"
                'Const INDICE_MAT_E_RACCOLTA As String = "l"
                'Const PIOGGE As String = "m"
                'Const RIL_PROD_E_DATA_RACC As String = "n"
                'Const SEMINE As String = "p"

                For j = 0 To UBound(ArraySezioni)

                    Select Case ArraySezioni(j)

                        Case "b"  'Fertilizzazioni          
                            StrLav_Cod &= CStr(LAVCOD_DISTRIBUZIONE_CONCIME) & ", " & CStr(LAVCOD_FERTIRRIGAZIONE) & ", " & CStr(LAVCOD_TRATTAMENTO_ANTIBUTTERATURA) &
                                ", " & CStr(LAVCOD_CONCIMAZIONE_FOGLIARE) & ", " & CStr(LAVCOD_DISTRIBUZIONE_AMMENDANTI) & ", " & CStr(LAVCOD_SARCHIATURA_CONCIMAZIONE) & ","

                        Case "c"      'Trattamenti
                            '28/01/2019: StrLav_Cod &= "74, 18, 103, 155, 13, 158,"
                            'spostati 118,121                    
                            StrLav_Cod &= CStr(LAVCOD_TRATTAMENTO_ANTIPARASSITARIO) & ", " & CStr(LAVCOD_DISERBO) & ", " & CStr(LAVCOD_TRATTAMENTO_FITOREGOLATORE) &
                                ", " & CStr(LAVCOD_GEODISINFESTAZIONE) & ", " & CStr(LAVCOD_CONCIA_SEME) & ", " & CStr(LAVCOD_DISSECCAMENTO) &
                                ", " & CStr(LAVCOD_DISTRIBUZIONE_INSETTI) & ", " & CStr(LAVCOD_CONFUSIONE_SESSUALE) & ", " & CStr(LAVCOD_DISORIENTAMENTO_SESSUALE) & "," & CStr(LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE) & ","

                        Case "d"     'Fasi Fenologiche
                            StrLav_Cod &= CStr(LAVCOD_FASI_FENOLOGICHE) & ","

                        Case "e"     'Trappole Installate, Confusione Sessuale, Disorientamento Sessuale, Cattura Massa
                            '28/01/2019: StrLav_Cod &= "107, 118, 121, 122," 
                            'spostati 118,121 
                            StrLav_Cod &= CStr(LAVCOD_INSTALLAZIONE_TRAPPOLE) & ", " & CStr(LAVCOD_CATTURE_MASSA) & "," & CStr(LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA) & ","

                        Case "f"     'Rilievi Avversità nelle Trappole
                            StrLav_Cod &= CStr(LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE) & ","

                        Case "g"    'Rilievi Avversita' in Campo
                            '28/01/2019: gestione erbe infestanti (119)
                            StrLav_Cod &= CStr(LAVCOD_RILIEVO_AVVERSITA_CAMPO) & ", " & CStr(LAVCOD_RILIEVO_ERBE_INFESTANTI) & ","

                        Case "h"      'irrigazione
                            StrLav_Cod &= "1,"

                        Case "i"     'Altre Operazioni

                            '14/01/2018: aggiunte 157, 161,
                            'StrLav_Cod &= "8, 9, 120, 10, 78, 12, 17, 19, 21, 22, 81, 23, 25, 27, 31, 32, 33, 34, 82, 83, 84, 40, 41, 85, 46, 47, 48, 87, 88, 49, 90, 91, 92, 53, 55, 115, 56, 57, 58, 59, 62, 63, 68, 75, 76, 77, 162,167, "
                            StrLav_Cod &= CStr(LAVCOD_ARATURA) & "," & CStr(LAVCOD_ANDANAMENTO) & "," & CStr(LAVCOD_ASPORTAZIONE_ORGANI_INFETTI) &
                                "," & CStr(LAVCOD_ASSOLCATURA) & "," & CStr(LAVCOD_CARICO_MANUALE_FRUTTA) & "," & CStr(LAVCOD_CIMATURA) &
                                "," & CStr(LAVCOD_DIRADAMENTO_MANUALE) & "," & CStr(LAVCOD_DISSODAMENTO) & "," & CStr(LAVCOD_ERPICATURA) &
                                "," & CStr(LAVCOD_ESTIRPATURA) & "," & CStr(LAVCOD_ESPIANTO) & "," & CStr(LAVCOD_FALCIACONDIZIONATURA) &
                                "," & CStr(LAVCOD_FALCIATURA_ERBAI) & "," & CStr(LAVCOD_FORMAZIONE_ARGINELLI) & "," & CStr(LAVCOD_FRANGIZOLLATURA) &
                                "," & CStr(LAVCOD_FRESATURA) & "," & CStr(LAVCOD_IMBALLO_FIENO_ROTOLI) & "," & CStr(LAVCOD_INTERRAMENTO_PAGLIE) &
                                "," & CStr(LAVCOD_LAVORAZIONE_TRA_FILA) & "," & CStr(LAVCOD_LAVORAZIONE_SU_FILA) & "," & CStr(LAVCOD_LEGATURA) &
                                "," & CStr(LAVCOD_LIVELLAMENTO) & "," & CStr(LAVCOD_MANUTENZIONE_ARGINI) & "," & CStr(LAVCOD_MESSA_DIMORA_PIANTE) &
                                "," & CStr(LAVCOD_MIETITREBBIATURA) & "," & CStr(LAVCOD_MINIMUM_TILLAGE) & "," & CStr(LAVCOD_PACCIAMATURA) &
                                "," & CStr(LAVCOD_POTATURA_SECCA) & "," & CStr(LAVCOD_POTATURA_VERDE) & "," & CStr(LAVCOD_PRESSATURA) &
                                "," & CStr(LAVCOD_RACCOLTA_LEGNA_POTATURA) & "," & CStr(LAVCOD_RACCOLTA_MANUALE) & "," & CStr(LAVCOD_RACCOLTA_MECCANICA) &
                                "," & CStr(LAVCOD_RANGHINATURA) & "," & CStr(LAVCOD_RINCALZATURA) & "," & CStr(LAVCOD_RIPPATURA) &
                                "," & CStr(LAVCOD_RIPUNTATURA) & "," & CStr(LAVCOD_RIVOLTAMENTO_FORAGGIO) & "," & CStr(LAVCOD_RULLATURA) &
                                "," & CStr(LAVCOD_SARCHIATURA) & "," & CStr(LAVCOD_SCARIFICATURA) & "," & CStr(LAVCOD_SCASSO) &
                                "," & CStr(LAVCOD_SOD_SEDDING) & "," & CStr(LAVCOD_TRINCIATURA) & "," & CStr(LAVCOD_VANGATURA) &
                                "," & CStr(LAVCOD_ZAPPATURA) & "," & CStr(LAVCOD_ERPICATURA_ROTANTE) & "," & CStr(LAVCOD_INTERVENTO_ANTIBRINA) &
                                "," & CStr(LAVCOD_ALTRE_OPERAZIONI) & "," & CStr(LAVCOD_STRIGLIATURA) & ","

                            'GRUPPO 5		        Altre Operazioni Colturali(non gestite) -> ma in realtà non si registrano
                            '13/02/2020: op aggiunte:
                            '11:                         Caricamento(letame)
                            '16:                         Condizionamento(erba)
                            '105:                        Diserbo(parziale)
                            '28:                         Formazione(caricoballe)
                            '29:                         Formazione(prose)
                            '42	Messa in asciutta
                            '44	Messa in opera  dreni
                            '43	Messa in opera armatura
                            '45:                         Mietitura()
                            '50:                         Pulizia(scoline)
                            '89:                         Pulizia(terreno)
                            '52:                         Raccolta(colletti)
                            '114:                        Rilievi(Avversita) ' Atmosferiche
                            '54:                         Rimpiazzo(fallanze)
                            '60:                         Scacchiatura()
                            '61	Scarico spandimento letame
                            '64:                         Scavo(scoline)
                            '65:                         Scerbatura()
                            '66:                         Scollettamento()
                            '67:                         Smontaggio(armature)
                            '69:                         Sommersione()
                            '70:                         Spargimento(terra)
                            '96:                         Spollonatura()
                            '97:                         Squadro(terreno)
                            '72:                         Trasporto(granella)
                            '73:                         Trasporto(letame)
                            '24	Trinciatura mais ceroso

                            'GRUPPO 2		        Rilievi alla Raccolta
                            '13/02/2020: op aggiunte:
                            ' 108	Danni alla Raccolta
                            ' 169	Rilievo Indici Rese/Raccolta

                            'GRUPPO 1		        Rilievi in Campo
                            '13/02/2020: op aggiunte:
                            '164	Monitoraggio acque di irrigazione
                            '150:    Reinnesco(TRAPPOLE)
                            '30:     Rilievo(Falda)

                            'gruppo 4		Lavorazioni
                            '13/02/2020: op aggiunte:
                            '152:   Gebiatura()
                            '154:    Lavorazione(Combinata)
                            '159:   Manutenzione(Impianti)
                            '168:   Pirodiserbo()
                            '153:   Rompicrosta()

                            StrLav_Cod &= "11,16,105,28,29,42,44,43,45,50,89,52,114,54,60,61,64,65,66,67,69,70,96,97,72,73,24,"
                            StrLav_Cod &= CStr(LAVCOD_DANNI_RACCOLTA) & "," & CStr(LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA) & ","
                            StrLav_Cod &= CStr(LAVCOD_MONITORAGGIO_ACQUE) & "," & CStr(LAVCOD_REINNESCO_TRAPPOLE) & "," & CStr(LAVCOD_RILIEVO_FALDA) & ","
                            StrLav_Cod &= CStr(LAVCOD_GEBIATURA) & "," & CStr(LAVCOD_LAVORAZIONE_CONBINATA) & "," & CStr(LAVCOD_MANUTENZIONE_IMPIANTI) & "," &
                                          CStr(LAVCOD_PIRODISERBO) & "," & CStr(LAVCOD_ROMPICROSTA) & "," & CStr(LAVCOD_ABBATTIMENTOIMPIANTI) & "," & CStr(LAVCOD_DEFOGLIAZIONE) & ","
                            StrLav_Cod &= CStr(LAVCOD_PASCOLAMENTO_PROPRIO) & "," & CStr(LAVCOD_PASCOLAMENTO_TERZI) & ","

                        Case "l"      'Indice di Maturità
                            StrLav_Cod &= CStr(LAVCOD_RILIEVO_INDICI_MATURITA) & ","

                        Case "m"     'Piogge
                            StrLav_Cod &= CStr(LAVCOD_RILIEVO_PIOGGE) & ","

                        Case "n"    'Rilievo Produzione e Data Raccolta
                            StrLav_Cod &= CStr(LAVCOD_RACCOLTA) & ","

                        Case "p"    'Semina - Trapianto - semina per sovescio - Semina Su Sodo - Trapianto in serra
                            StrLav_Cod &= CStr(LAVCOD_SEMINA) & ", " & CStr(LAVCOD_TRAPIANTO) & ", " & CStr(LAVCOD_SOVESCIO) &
                                ", " & CStr(LAVCOD_SOD_SEDDING) & ", " & CStr(LAVCOD_TRAPIANTO_IN_SERRA) & ","

                            '13/02/2020: gestita selezione sezione
                        Case "q"    'INFORMAZ_E_DICHIAR
                            '166	Dichiarazione di non utilizzo (Fertilizzazioni)
                            '165	Dichiarazione di non utilizzo (Trattamenti)
                            StrLav_Cod &= CStr(LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO) &
                                "," & CStr(LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO) & ","

                            '13/02/2020: gestita selezione sezione
                        Case "r"    'VISITE_ISPETTIVE
                            'LAVCOD_VISITA_GENERICA
                            'LAVCOD_MONITORAGGIO_TEMPI_RIENTRO
                            StrLav_Cod &= CStr(LAVCOD_VISITA_GENERICA) & "," & CStr(LAVCOD_MONITORAGGIO_TEMPI_RIENTRO)

                            '13/02/2020: gestita selezione sezione
                        Case "x"    'GESTIONE_RIFIUTI
                            StrLav_Cod &= CStr(LAVCOD_GESTIONE_RIFIUTI) & ","

                            '13/02/2020: gestita selezione sezione
                        Case "1"    'TRATTAMENTI_POSTRACCOLTA
                            StrLav_Cod &= CStr(LAVCOD_TRATTAMENTO_POST_RACCOLTA) & ","
                    End Select

                Next

                'Const INFORMAZ_E_DICHIAR As String = "q"
                'Const GESTIONE_RIFIUTI As String = "x"
                'Const TRATTAMENTI_POSTRACCOLTA As String = "1"
                'Const VISITE_ISPETTIVE As String = "r"

                'Const FITOFARMACI As String = "t"
                'Const PRATICHE_ECOLOGICHE As String = "u"
                'Const FORMAZIONE As String = "w"
                'Const MANUTEN_MACCHINARI As String = "y"
                'Const MACCH_SOLO_DIFESA As String = "z"

            End If

            '---------------------------------------------------------------------------------------------------
            '--- fine MODIFICA 13/05/2013
            '---------------------------------------------------------------------------------------------------

            If StrLav_Cod <> "" Then
                StrLav_Cod = Left(StrLav_Cod, StrLav_Cod.Length - 1)
            End If

        Catch ex As Exception
            Log_Errori += "- pageload: " + vbCrLf + ex.Message + vbCrLf
        End Try


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        If Not Me.IsPostBack Then

            Try

                CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

                Dim i As Integer

                CrystalReportViewer1.Style.Add("LEFT", "-275px")
                CrystalReportViewer1.Style.Add("TOP", "0px")
                CrystalReportViewer1.Style.Add("POSITION", "Absolute")
                Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

                Dim objSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                'calcolo il basecode x l'utente
                objSequenze.Calcola_BaseCode(Session("ASG_ProgressivoGIAS"), Nothing, BaseCode, objParametri_Server)

                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim pivaReale As String = objImp.Leggi_PivaReale(Qs_Piva, objParametri_Server)

                CType(rptStampa.Section1.ReportObjects("TextPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = AgronicaCoreStampeDAL.Stampe_QDC.Pulisci_Piva_Fittizia(pivaReale)

                'If Qs_Sa_Cod <> "0" Then
                '    Indirizzo_from_PivaSaCod(Qs_Piva, Qs_Sa_Cod, rag_soc, sa_nome, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat, Server)
                '    CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = sa_nome
                'Else
                'Indirizzo_from_Piva(Server, Session, Page, Qs_Piva, rag_soc, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat)
                Dim objInd As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
                objInd.Indirizzo_from_Piva(Qs_Piva, rag_soc, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat, objParametri_Server)
                'End If

                If strSa_Nome <> "" Then
                    CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strSa_Nome
                End If

                'Estraggo il legale rappresentante..
                'Recupera_RappresentanteLegale(Server, Session, Page, Qs_Piva, CStr(Session("ASG_SuperUser_CodFiscale")), RsRapprLegale, MessaggioErrore)
                'If RsRapprLegale.State <> 0 Then
                '    If Not RsRapprLegale.EOF Then
                '        CType(rptStampa.Section1.ReportObjects("TextResponsabile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = RsRapprLegale.Fields("CognomeNome_RapLeg").Value
                '    End If
                'End If

                Dim objLegale As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim Cognome, Nome As String

                objLegale.LegaleRappresentanteDati_from_PivaImpresa("", Nome, Cognome, "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", CStr(Qs_Piva), "", objParametri_Server)

                CType(rptStampa.Section1.ReportObjects("TextResponsabile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Cognome & " " & Nome

                ' Qs_Sa_Cod non è mai valorizzato
                Dim CentrixRubrica_Read As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
                'CType(rptStampa.Section1.ReportObjects("TextTel"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CentrixRubrica_Read.Recupera_Telefono_Centro(Qs_Piva, Qs_Sa_Cod, objParametri_Server)
                CType(rptStampa.Section1.ReportObjects("TextTel"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CentrixRubrica_Read.Recupera_Telefono_Centro(Qs_Piva, 0, objParametri_Server)

                CType(rptStampa.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = rag_soc
                CType(rptStampa.Section1.ReportObjects("TextIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ind_des
                CType(rptStampa.Section1.ReportObjects("TextFrazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = frz_des
                CType(rptStampa.Section1.ReportObjects("TextComune"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = com_des
                CType(rptStampa.Section1.ReportObjects("TextProvincia"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = pro_cod
                CType(rptStampa.Section1.ReportObjects("TextCap"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CAP
                CType(rptStampa.Section1.ReportObjects("TxtRegolamentoBIO"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descrizione_Regolamento_Bio_STAMPE

                'Dim Province As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
                'CType(rptStampa.Section6.ReportObjects("TextRegione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Province.Regione_from_Provincia(pro_cod, "", Nothing, Nothing, objParametri_Server)
                Dim objRegioni As New AgronicaCoreMetaSchemaDAL.Lista_Regioni_R
                Dim StrRegione As String
                If Not IsNothing(Session("Regione_Selezionata")) AndAlso Session("Regione_Selezionata") <> "" Then
                    StrRegione = objRegioni.RegioneDes_from_REG(Session("Regione_Selezionata"), objParametri_Server)
                Else
                    StrRegione = ""
                End If
                CType(rptStampa.Section6.ReportObjects("TextRegione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = StrRegione
                CType(rptStampa.Section6.ReportObjects("TextAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dal " & QS_DataDa & " Al " & QS_DataA 'QS_Anno

                'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
                'If customLoghi IsNot Nothing Then
                '    rptStampa.Section5.ReportObjects("Picture5").ObjectFormat.EnableSuppress = True
                '    rptStampa.Section5.ReportObjects("Text3").ObjectFormat.EnableSuppress = True
                'End If

                '-----------------------------------------------
                'CODICE OPERATORE
                Dim Codice_Operatore As String
                Dim objCentricodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                Codice_Operatore = objCentricodici.CodOperatoreBIO_from_PivaSaCod(Qs_Piva,
                                                                    Qs_Sa_Cod,
                                                                    objParametri_Server)
                'CType(rptStampa.Section1.ReportObjects("TextCodice"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Imprese_Codici_Read.Leggi_Codice_from_Imprese_Codici(Qs_Piva, 1033, objParametri_Server)
                CType(rptStampa.Section1.ReportObjects("TextCodice"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Codice_Operatore
                '-----------------------------------------------
                'ORGANISMO DI CONTROLLO
                Dim ODC_sigla As String = ""
                Dim ODC_des As String = ""
                Dim ODC_codice As String = ""
                objCentricodici.OrganismodiControllo_from_PivaSaCod(Qs_Piva,
                                                                    Qs_Sa_Cod,
                                                                    ODC_sigla,
                                                                     ODC_des,
                                                                     ODC_codice,
                                                                     objParametri_Server
                                                                    )

                If ODC_codice <> "" Then
                    OrganismoControllo = ODC_codice
                ElseIf ODC_des <> "" Then
                    OrganismoControllo = ODC_des
                End If
                '------------------------------------------

            Catch ex As Exception
                Log_Errori += "- intestazione: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim DSSchedaColturaleBiologico As New DS_SchedaColturaleBiologico
            Dim DSLogoFooter As New DS_LogoFooter

            Try
                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Carica_DSSchedaColturaleBiologico(DSSchedaColturaleBiologico, DSSchedaColturaleBiologico, flagNascondiResponsabile,
                                                  flagNascondiOperatori, flagNascondiMacchine, flagNascondiDataFirma, flagNascondiDataUltimaManutenzione,
                                                  flagVisualizzaCampo, flagNascondiAppezzamentoBIO, flagNascondinumeroAppezzamneto, TipoCodiceAppezzamento, flagVisualizzaLottoSemine, flagVisualizzaLottoRaccolte)

                Dim drLogo = DSLogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
                Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, Log_Errori, objParametri_Server)
                If logo.LogoStampe IsNot Nothing Then
                    drLogo.Logo = logo.LogoStampe
                    drLogo.TestoPostLogo = logo.TestoPostLogo
                    drLogo.TestoPreLogo = logo.TestoPreLogo
                End If
                DSLogoFooter.DT_LogoFooter.Rows.Add(drLogo)

            Catch ex As Exception
                Log_Errori += "- Carica_DSSchedaColturaleBiologico: " + vbCrLf + ex.Message + vbCrLf
            End Try

            '--------------------------------------------
            ' AGGANCIO DATI
            '--------------------------------------------
            Try
                rptStampa.SetDataSource(DSSchedaColturaleBiologico)
                rptFooterLogo.SetDataSource(DSLogoFooter)
                rptStampa.OpenSubreport("FooterLogo.rpt").SetDataSource(DSLogoFooter)
            Catch ex As Exception
                Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try
                'PARAMETRI
                Dim CUAA As String
                Dim objcodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                CUAA = objcodici.Leggi_CUAA(Qs_Piva, objParametri_Server)
                rptStampa.SetParameterValue("CUAA", CUAA)
                rptStampa.SetParameterValue("OrganismoControllo", OrganismoControllo)

                Dim dataOdierna As String = String.Empty
                If Not IsNothing(Session("MostraDataOdiernaStampa")) AndAlso
                    Session("MostraDataOdiernaStampa") = True Then
                    dataOdierna = DateTime.Today.ToString("dd/MM/yyyy")
                    'Pulisco la sessione, perche' altrimenti genera confiliti con le stampe bio materie prime e  vendite 
                    Session.Remove("MostraDataOdiernaStampa")
                End If
                rptStampa.SetParameterValue("dataOdierna", dataOdierna)

                Dim nome_completo = String.Empty
                If Not IsNothing(Session("MostraFirmaODC")) AndAlso
                    Session("MostraFirmaODC") = True Then
                    Dim utenti_dettaglio_dal = New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
                    nome_completo = utenti_dettaglio_dal.NomeCognome_From_Username(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
                    rptStampa.ReportDefinition.Sections("Section4").SectionFormat.EnableSuppress = False
                End If
                rptStampa.SetParameterValue("Nome_Completo", nome_completo)

            Catch ex As Exception
                Log_Errori += "- set parametri: " + vbCrLf + ex.Message + vbCrLf
            End Try
            'objParametri_Utenti.UtenteUsername

            Session("Report") = rptStampa


            If Qs_StampaDefinitiva = "1" Then

                Try

                    ' leggo la sottocartella da CategorieDocumenti
                    Dim Sottocartella As String
                    Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                    Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Biologico_Colturale, "", "", objParametri_Server)
                    objCatDoc = Nothing

                    Dim DataOraStampa As String

                    DataOraStampa = Right("00" & Now.Day.ToString, 2) &
                                    Right("00" & Now.Month.ToString, 2) &
                                    Right("00" & Now.Year.ToString, 4) & "_" &
                                    Right("00" & Now.Hour.ToString, 2) &
                                    Right("00" & Now.Minute.ToString, 2)

                    ' salvo il report in formato PDF
                    Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                    NomeFilePDF = Nome_Documento & "_p" & Qs_Piva & "_d" & DataOraStampa + ".pdf"
                    objGestFile.SalvaReportPdf(Session("Report"),
                                               enum_CategorieDocumenti.RegistriCampagna,
                                               Sottocartella,
                                               NomeFilePDF,
                                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                    Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                    Dim AllegatiDocumentiCod As Integer

                    Dim Data_Inizio, Data_Fine As Date

                    If IsDate(QS_DataDa) Then
                        Data_Inizio = CDate(QS_DataDa)
                    End If

                    If IsDate(QS_DataA) Then
                        Data_Fine = CDate(QS_DataA)
                    End If

                    If IsDate(Qs_DataStampa) Then
                        Data_Inizio = CDate(Qs_DataStampa)
                        Data_Fine = CDate(Qs_DataStampa)
                    End If


                    AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva,
                                                                     enum_CategorieDocumenti.RegistriCampagna,
                                                                     Nome_Documento,
                                                                     Nome_Documento & "_p" & Qs_Piva & "_d" & DataOraStampa + ".pdf",
                                                                     Sottocartella,
                                                                      "", "", "", "",
                                                                     CDate(Data_Inizio),
                                                                     CDate(Data_Fine),
                                                                     objParametri_Server)

                Catch ex As Exception
                    Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
                End Try

            End If 'allegati

            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                If NomeFilePDF = "" Then
                    Dim DataOraStampa As String

                    DataOraStampa = Right("00" & Now.Day.ToString, 2) &
                                    Right("00" & Now.Month.ToString, 2) &
                                    Right("00" & Now.Year.ToString, 4) & "_" &
                                    Right("00" & Now.Hour.ToString, 2) &
                                    Right("00" & Now.Minute.ToString, 2)

                    Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                    NomeFilePDF = Nome_Documento & "_p" & Qs_Piva & "_d" & DataOraStampa + ".pdf"
                    objGestFile.SalvaReportPdf(rptStampa, enum_CategorieDocumenti.RegistriCampagna,
                                           "", NomeFilePDF,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                End If
            Catch ex As Exception
                Log_Errori += "- Salvataggio PDF temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try
                rptStampa.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            If Log_Errori <> "" Then
                Dim Path_Errore, Str_Errore_Path, Nome_File As String
                Log_Errori = Nome_Documento + ", Data inizio = " + CStr(QS_DataDa) + ", Data fine = " + CStr(QS_DataA) + ", Piva = " + CStr(Qs_Piva) + ", Sa_Cod = " + CStr(Qs_Sa_Cod) +
                                             vbCrLf + vbCrLf + Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + "_" + CStr(Qs_Piva) + "_" + CStr(Qs_Sa_Cod) & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Biologico",
                                                 Nome_File,
                                                 Session("ASG_Utente_Username"),
                                                 "SchedaColturaleBiologico.aspx",
                                                 Log_Errori)

            End If

            DSSchedaColturaleBiologico.Dispose()
            DSSchedaColturaleBiologico = Nothing
            rptStampa.Close()
            rptStampa.Dispose()
            rptStampa = Nothing

            GC.Collect()

            'MS Eliminazione dal report in session per garantire deallocazione.
            Session.Remove("Report")

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                                "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                                "&NomePdf=" & Stringa_Codifica(NomeFilePDF, AgroKey_EncoderDecoder, Server))

            ''faccio il databind col visualizzatore dei reports...
            'CrystalReportViewer1.ReportSource = rptStampa
            'CrystalReportViewer1.DataBind()

            ''array di dataset e data table
            'Dim dsRpt() As DataSet = {DSSchedaColturaleBiologico}

            ''salvo il report nella sessione
            'Session("DS") = dsRpt


            'Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            '    If Not IsNothing(Session("DS")) Then

            '        'imposto la sorgente dati x il report...
            '        rptStampa.SetDataSource(CType(Session("DS")(0), DataSet))

            '        'faccio il databind col visualizzatore dei reports...
            '        CrystalReportViewer1.ReportSource = rptStampa
            '        CrystalReportViewer1.DataBind()

            '    End If

        End If 'postback

    End Sub



    '###############################################################################
    Public Function Esiste_CodiceAppezzamentoPAP(ByVal Piva As String,
                                                 ByVal Sa_Cod As Integer,
                                                 ByVal Appezza As Integer,
                                                 ByRef Valore As String,
                                                 ByRef objServer As Object) As Boolean


        Dim Rs As DataTable
        Dim Appezzamento_Codici_R As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R 'Agro_Anagrafe_AD.Appezzamento_Codici_R

        Rs = Appezzamento_Codici_R.Leggi(Piva,
                          Sa_Cod,
                          Appezza,
                          enum_CodiciAnagrafe.Codice_Appezza_Biologico,
                          "",
                          AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

        If Not IsNothing(Rs) Then

            If Rs.Rows.Count <> 0 Then

                Valore = Rs.Rows(0).Item("val_cod")
                Return True

            Else

                Valore = "-1"
                Return False

            End If

        Else

            Valore = "-1"
            Return False

        End If

        Rs = Nothing

    End Function



    '###########################################################################
    'riempie il ds x la scheda...
    'Indica nel N.App. il Numero Appezzamento inserito nel PAP
    Private Sub Carica_DSSchedaColturaleBiologico(ByRef DSSchedaColturaleBiologico As DS_SchedaColturaleBiologico, ByRef DS_SchedaColturaleBiologico_Sort As DS_SchedaColturaleBiologico,
                            ByVal Flag_Nasc_Resp As Boolean,
                            ByVal Flag_Nasc_Oper As Boolean,
                            ByVal Flag_Nasc_Macc As Boolean,
                            ByVal Flag_Nasc_DataFirma As Boolean,
                            ByVal Flag_Nasc_DataUltMan As Boolean,
                            ByVal flag_Visual_Campo As Boolean,
                            ByVal flag_Nasc_App_Bio As Boolean,
                            ByVal flag_Nasc_Num_App As Boolean,
                            ByVal TipoCodiceAppezzamento As Integer,
                            ByVal flagVisualizzaLottoSemine As Boolean,
                            ByVal flagVisualizzaLottoRaccolte As Boolean)

        Dim objSQL As New AgronicaCoreDataProvider.DataProvider
        Dim strErr As String
        Dim sSql As New System.Text.StringBuilder

        Dim i, j, n, c As Integer
        Dim RsProd As DataTable
        Dim RsIrrigazione As DataTable
        Dim DTOssFasi As DataTable
        Dim DrOssFasi() As DataRow

        Dim DtRilievoIndiciMaturita As New DataTable
        Dim DtRilievoAvversitaCampo As New DataTable
        Dim DtRilievoAvversitaTrappole As New DataTable

        Dim App_Nome As String
        Dim Num_Appezzamento As Double
        Dim Num_Appezzamento_PAP As String
        Dim Nome_Campo As String

        Dim strQuery As String

        Dim Produzione As Decimal = 0
        Dim Produzione_Tot As Decimal = 0
        Dim Qta_Tot As Decimal = 0
        Dim Qta_Media As Decimal = 0

        Dim SupColtura_Tot As Decimal = 0
        Dim SupTrattata_Tot As Decimal = 0
        Dim k As Integer

        Dim DataInizio, DataFine As Date
        If IsDate(QS_DataA) And IsDate(QS_DataDa) Then
            DataInizio = QS_DataDa
            DataFine = QS_DataA
        ElseIf IsDate(Qs_DataStampa) Then
            DataInizio = Qs_DataStampa
            DataFine = Qs_DataStampa
        End If

        sSql.Length = 0
        sSql.AppendLine(" SELECT DISTINCT Agenda.id_agenda, Agenda.Lav_Cod, Reg_Impianti.PIVA, Reg_Impianti.SA_COD, ISNULL(Agenda.Raccoglitore_Cod, 0) AS Raccoglitore_Cod, ")
        sSql.AppendLine("")
        sSql.AppendLine(" CASE WHEN Attivita.[Desc] IS NOT NULL ")
        sSql.AppendLine("   THEN Operazioni.LAV_DES + ': ' + Attivita.[Desc] ")
        sSql.AppendLine("   ELSE Operazioni.LAV_DES ")
        sSql.AppendLine(" END AS LAV_DES, ")
        sSql.AppendLine("")
        sSql.AppendLine(" Appezzamento.Campo_Cod, Reg_Impianti.APPEZZA, Appezzamento.App_Nome, Reg_Impianti.ID_REG, ")
        sSql.AppendLine("")
        sSql.AppendLine(" ISNULL((SELECT TOP 1 val_cod FROM Appezzamento_Codici ")
        sSql.AppendLine("         WHERE Appezzamento_Codici.Piva = Appezzamento.PIVA ")
        sSql.AppendLine("         AND Appezzamento_Codici.sa_cod = Appezzamento.sa_cod ")
        sSql.AppendLine("         AND Appezzamento_Codici.appezza = Appezzamento.APPEZZA ")
        sSql.AppendLine("         AND Appezzamento_Codici.id_cod = 1085 ")
        sSql.AppendLine("         ORDER BY Appezzamento_Codici.Data_Creazione DESC), '') AS NumApp_PAP, ")
        sSql.AppendLine("")
        sSql.AppendLine(" ISNULL(Reg_Impianti.CUL_COD, 0) AS CUL_COD, ISNULL(SpecieVegetali.Veg_Cod, 0) AS Veg_Cod, ISNULL(SpecieVegetali.Veg_Des, '') AS Veg_Des, ")
        sSql.AppendLine("")

        '20/08/2019 introdotta lettura destinazione d'uso 
        sSql.AppendLine(" CASE WHEN reg_impianti.cul_cod = 0 THEN ")
        sSql.AppendLine("       (ISNULL((SELECT TOP 1 descrizione ")
        sSql.AppendLine("                FROM Reg_Impianti_Codici RIC ")
        sSql.AppendLine("                INNER JOIN Codici_Anagrafe Cod ON RIC.id_cod = Cod.codice ")
        sSql.AppendLine("                WHERE progetto_cod = 0 ")
        sSql.AppendLine("                AND id_cod >= 3000 AND id_cod < 4000 ")
        sSql.AppendLine("                AND piva = Reg_Impianti.piva AND sa_cod = Reg_Impianti.sa_cod ")
        sSql.AppendLine("                AND appezza = Reg_Impianti.appezza AND id_reg = Reg_Impianti.id_reg ")
        sSql.AppendLine("                ), ''))")
        sSql.AppendLine(" ELSE  ( ")
        sSql.AppendLine("       Cultivar.Cul_Des  ")
        sSql.AppendLine("       )  ")
        sSql.AppendLine("  end AS Cul_Des, ")
        sSql.AppendLine("")
        sSql.AppendLine(" ISNULL(Reg_Impianti.GRVA_Cod_VEG,0) AS Grva_Cod, ISNULL(GruppoVarietale.Grva_Des,'') AS Grva_Des, ")
        sSql.AppendLine("")
        sSql.AppendLine(" Reg_Impianti.sup_imp AS SUP_APP, Mov_Destinazioni.qta2 as sup_trattata, ")
        sSql.AppendLine("")
        sSql.AppendLine(" Movimenti.Data_Movimento, ")
        sSql.AppendLine(" Movimenti.Cau_Mov, Movimenti_dettagli.Elem_Cod, ")
        sSql.AppendLine(" Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, ")
        sSql.AppendLine(" Movimenti_dettagli.Udm_Cod, ISNULL(UnitaMisura.UDM_SIM,'') AS Udm_Sim, ")
        sSql.AppendLine(" ISNULL(Formulati.Fr_Des, '') AS Fr_Des, ")
        sSql.AppendLine(" ISNULL(Fertilizzanti.Fer_Des, '') AS Fer_Des, ")
        sSql.AppendLine(" ISNULL(Trappole.TRAP_DES, '') AS Trap_Des, ")
        sSql.AppendLine(" ISNULL(InsettiUtili.Ins_Des, '') AS Ins_Des, ")
        sSql.AppendLine(" ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des, ")
        sSql.AppendLine(" Mov_Destinazioni.Qta, ")
        sSql.AppendLine("")

        '24/07/2019: aggiunti questi campi
        sSql.AppendLine(" Movimenti_dettagli.extra_int AS udm_cod_reale, ")
        sSql.AppendLine(" ISNULL((SELECT udm_sim ")
        sSql.AppendLine("         FROM UnitaMisura UnitaMisuraReale  ")
        sSql.AppendLine("         WHERE Movimenti_dettagli.extra_int = UnitaMisuraReale.UDM_COD  ")
        sSql.AppendLine("         ),'' ) AS Udm_Sim_Reale, ")
        sSql.AppendLine(" Movimenti_dettagli.udm_cod AS udm_cod_trasformato, ")
        sSql.AppendLine(" Movimenti_dettagli.Qta AS dose_ha_reale, ")
        sSql.AppendLine(" Movimenti_dettagli.Qta_extra AS dose_hl_reale, ")
        sSql.AppendLine(" Movimenti_dettagli.Qta_extra_totale AS dose_tot_reale,  ")
        sSql.AppendLine(" Movimenti_dettagli.mezzo_det AS ha_hl, ")
        sSql.AppendLine(" Movimenti_dettagli.udm_cod_extra AS dose_qta_tot, ")
        sSql.AppendLine("  ")
        sSql.AppendLine("")

        '--------------------------------------------------------------------------------
        '---------------   modifica per avversita
        '--------------------------------------------------------------------------------
        sSql.AppendLine(" Movimenti_dettagli.id_mov_det, ")
        sSql.AppendLine("")

        ' concatena in una stringa le avversità per ogni movimento_dettaglio leggendo dal Mov_Dettaglio_Tecnico
        'sSql.appendline("  ISNULL((SELECT CONVERT (nvarchar, avversita.av_Des_Vol)  + ',' AS [text()] ")
        sSql.AppendLine(" ISNULL((SELECT CONVERT(nvarchar, avversita.av_Des_Vol) + '|' + CONVERT (nvarchar, Mov_Dettaglio_Tecnico.id_mov_det)  + ',' AS [text()] ")
        sSql.AppendLine("         FROM Mov_Dettaglio_Tecnico, Avversita ")
        sSql.AppendLine("         WHERE (Mov_Dettaglio_Tecnico.PIVA = Movimenti.PIVA And Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti.Sa_Cod) ")
        sSql.AppendLine("         AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti.Id_Agenda AND Mov_Dettaglio_Tecnico.id_mov=Movimenti.id_mov  ")
        sSql.AppendLine("         AND (Mov_Dettaglio_Tecnico.av_cod <> 0 or Mov_Dettaglio_Tecnico.av_gru <> 0)    ")
        sSql.AppendLine("         AND Avversita.av_cod = Mov_Dettaglio_Tecnico.av_cod AND Mov_Dettaglio_Tecnico.av_cod <> 0 ")
        sSql.AppendLine("         For XML PATH ('')),'') AS [av_Des_Vol], ")
        sSql.AppendLine("")

        sSql.AppendLine(" ISNULL((SELECT DISTINCT Avversita.Av_Cod  ")
        sSql.AppendLine("         FROM Mov_Dettaglio_Tecnico mvd ")
        sSql.AppendLine("         LEFT JOIN Avversita on mvd.Av_Cod = Avversita.Av_Cod ")
        sSql.AppendLine("         WHERE (mvd.PIVA = Movimenti.PIVA And mvd.Sa_Cod = Movimenti.Sa_Cod) ")
        sSql.AppendLine("         AND mvd.Id_Agenda = Movimenti.Id_Agenda AND mvd.id_mov = Movimenti.id_mov AND mvd.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")
        sSql.AppendLine("         AND (mvd.Av_Cod <> 0 OR mvd.av_gru <> 0)    ")
        sSql.AppendLine("         AND mvd.av_cod <> 0 ")
        sSql.AppendLine("         ),'0') AS Av_Cod,  ")
        sSql.AppendLine("")

        sSql.AppendLine(" ISNULL((SELECT CONVERT (nvarchar, GruppoAvversita.Av_Gru_Des) + '|' + CONVERT (nvarchar, Mov_Dettaglio_Tecnico.id_mov_det)  + ',' AS [text()] ")
        sSql.AppendLine("         FROM Mov_Dettaglio_Tecnico, GruppoAvversita ")
        sSql.AppendLine("         WHERE(Mov_Dettaglio_Tecnico.PIVA = Movimenti.PIVA And Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti.Sa_Cod) ")
        sSql.AppendLine("         AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti.Id_Agenda AND Mov_Dettaglio_Tecnico.id_mov=Movimenti.id_mov  ")
        sSql.AppendLine("         AND (Mov_Dettaglio_Tecnico.av_cod <> 0 or Mov_Dettaglio_Tecnico.av_gru <> 0) ")
        sSql.AppendLine("         AND GruppoAvversita.av_gru=Mov_Dettaglio_Tecnico.av_gru ")
        sSql.AppendLine("         AND Mov_Dettaglio_Tecnico.av_gru <> 0  ")
        sSql.AppendLine("         For XML PATH ('')),'') AS [Av_Gru_Des], ")
        sSql.AppendLine("")

        sSql.AppendLine(" ISNULL((SELECT DISTINCT GruppoAvversita.Av_Gru ")
        sSql.AppendLine("         FROM Mov_Dettaglio_Tecnico mvd ")
        sSql.AppendLine("         LEFT JOIN GruppoAvversita on mvd.Av_Gru = GruppoAvversita.Av_Gru ")
        sSql.AppendLine("         WHERE(mvd.PIVA = Movimenti.PIVA And mvd.Sa_Cod = Movimenti.Sa_Cod) ")
        sSql.AppendLine("         AND mvd.Id_Agenda = Movimenti.Id_Agenda AND mvd.id_mov = Movimenti.id_mov AND mvd.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")
        sSql.AppendLine("         AND (mvd.av_cod <> 0 OR mvd.av_gru <> 0) ")
        sSql.AppendLine("         AND mvd.av_gru <> 0  ")
        sSql.AppendLine("         ),'0') AS Av_Gru, ")
        sSql.AppendLine("")

        '--------------------------------------------------------------------------------
        '---------------  fine modifica per avversita
        '--------------------------------------------------------------------------------

        'Note      
        'stbQ.appendline(" ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des, " )
        sSql.AppendLine(" ISNULL(Movimenti.Mov_Desc,'') AS Note, ISNULL(Movimenti_dettagli.Lotto, '') AS Lotto, ")

        sSql.AppendLine(" ISNULL((SELECT TOP 1 Imprese_Progetti.Progetto_Nome FROM Imprese_Progetti ")
        sSql.AppendLine("       WHERE Reg_Impianti.Piva = Imprese_Progetti.PIVA ")
        sSql.AppendLine(" 	     AND Reg_Impianti.sa_cod = Imprese_Progetti.sa_cod ")
        sSql.AppendLine(" 	     AND Reg_Impianti.appezza = Imprese_Progetti.APPEZZA ")
        sSql.AppendLine(" 	     AND Reg_Impianti.id_reg = Imprese_Progetti.ID_REG  ")

        'se ho scelto una data unica per la stampa..
        If Qs_DataStampa <> "" Then
            sSql.AppendLine($" 	     AND Imprese_Progetti.Validita_Inizio <= {Agro_SQL_SaveDate(CDate(Qs_DataStampa))}")
            sSql.AppendLine($" 	     AND Imprese_Progetti.Validita_Fine >= {Agro_SQL_SaveDate(CDate(Qs_DataStampa))}")
        Else
            sSql.AppendLine($" 	     AND Imprese_Progetti.Validita_Inizio <= {Agro_SQL_SaveDate(CDate(QS_DataA))}")
            sSql.AppendLine($" 	     AND Imprese_Progetti.Validita_Fine >= {Agro_SQL_SaveDate(CDate(QS_DataDa))}")
        End If

        sSql.AppendLine(" 	     ORDER BY Imprese_Progetti.Validita_Inizio DESC ), '') AS LottoImpianto,")

        sSql.AppendLine(" CASE WHEN Totali.TotaleImpianti > Totali.ImpiantiStampa THEN 'Parziale' ELSE 'Totale' END Estrazione, ")

        'Modaliltà applicazione e Nr domanda ACA
        sSql.AppendLine(" ISNULL(Movimenti.Modalita_Applicazione, 0) AS Modalita_Applicazione,")
        sSql.AppendLine(" ISNULL(ModAppl.Modalita_Applicazione_Descrizione, '') AS Modalita_Applicazione_Descrizione,")
        sSql.AppendLine(" ISNULL(CodiciACA.Nr_domanda_ACA, '') AS Nr_domanda_ACA")

        'AF 08/28: Utilizzato in caso di estrazione monocentro (serve solo per stampa trappole):
        ' se l'impianto è influenzato da altro centro o da altro impianto, lo indico con 1 (sotto), altrimenti 0
        sSql.AppendLine(", 0 AS isInfluenzatoDaAltroCentroOImpianto")

        sSql.AppendLine(" FROM Agenda  ")
        sSql.AppendLine(" INNER JOIN Movimenti ")
        sSql.AppendLine(" ON Agenda.PIVA = Movimenti.PIVA")
        sSql.AppendLine(" AND Agenda.Sa_Cod = Movimenti.Sa_Cod")
        sSql.AppendLine(" AND Agenda.Id_Agenda = Movimenti.Id_Agenda  ")
        sSql.AppendLine(" INNER JOIN Movimenti_dettagli")
        sSql.AppendLine(" ON Movimenti.PIVA = Movimenti_dettagli.PIVA")
        sSql.AppendLine(" AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod")
        sSql.AppendLine(" AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda")
        sSql.AppendLine(" AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov  ")

        sSql.AppendLine(" LEFT JOIN Mov_Dettaglio_Tecnico ")
        sSql.AppendLine(" ON Mov_Dettaglio_Tecnico.PIVA = Movimenti_dettagli.PIVA")
        sSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Sa_Cod = Movimenti_dettagli.Sa_Cod")
        sSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Id_Agenda = Movimenti_dettagli.Id_Agenda ")
        sSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Id_Mov = Movimenti_dettagli.Id_Mov  ")
        sSql.AppendLine(" AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ")

        'Modalità applicazione
        sSql.AppendLine("LEFT OUTER JOIN (")
        sSql.AppendLine("SELECT Codice, AGEA_Des As Modalita_Applicazione_Descrizione")
        sSql.AppendLine("FROM Modalita_Applicazione_Globali")
        sSql.AppendLine("UNION")
        sSql.AppendLine("SELECT Codice, Descrizione_Personalizzata")
        sSql.AppendLine("FROM Modalita_Applicazione")
        sSql.AppendLine(") As ModAppl")
        sSql.AppendLine("ON Movimenti.Modalita_Applicazione = ModAppl.Codice")

        sSql.AppendLine(" INNER JOIN Mov_Destinazioni")
        sSql.AppendLine(" ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva")
        sSql.AppendLine(" AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod")
        sSql.AppendLine(" AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda")
        sSql.AppendLine(" AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov")
        sSql.AppendLine(" AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det  ")
        sSql.AppendLine(" INNER JOIN Reg_Impianti")
        sSql.AppendLine(" ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA")
        sSql.AppendLine(" AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD")
        sSql.AppendLine(" AND Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA")
        sSql.AppendLine(" AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG  ")
        sSql.AppendLine(" INNER JOIN Operazioni")
        sSql.AppendLine(" ON Agenda.Lav_Cod = Operazioni.LAV_COD  ")
        sSql.AppendLine(" LEFT OUTER JOIN Cultivar")
        sSql.AppendLine(" ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod  ")
        sSql.AppendLine(" LEFT OUTER JOIN SpecieVegetali")
        sSql.AppendLine(" ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod  ")
        sSql.AppendLine(" INNER JOIN Appezzamento")
        sSql.AppendLine(" ON Reg_Impianti.APPEZZA = Appezzamento.APPEZZA")
        sSql.AppendLine(" AND dbo.Reg_Impianti.SA_COD = Appezzamento.SA_COD")
        sSql.AppendLine(" AND Reg_Impianti.PIVA = Appezzamento.PIVA ")
        sSql.AppendLine(" INNER JOIN Imprese_Progetti")
        sSql.AppendLine(" ON Reg_Impianti.PIVA = Imprese_Progetti.Piva")
        sSql.AppendLine(" AND Reg_Impianti.SA_COD = Imprese_Progetti.Sa_Cod")
        sSql.AppendLine(" AND Reg_Impianti.APPEZZA = Imprese_Progetti.Appezza")
        sSql.AppendLine(" AND Reg_Impianti.ID_REG = Imprese_Progetti.Id_Reg")
        sSql.AppendLine(" LEFT OUTER JOIN GruppoVarietale")
        sSql.AppendLine(" ON Reg_Impianti.GRVA_Cod_VEG = GruppoVarietale.Grva_Cod  ")
        sSql.AppendLine(" LEFT OUTER JOIN Formulati")
        sSql.AppendLine(" ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod ")
        sSql.AppendLine(" LEFT OUTER JOIN Trappole")
        sSql.AppendLine(" ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD  ")
        sSql.AppendLine(" LEFT OUTER JOIN Fertilizzanti")
        sSql.AppendLine(" ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod  ")
        sSql.AppendLine(" LEFT OUTER JOIN Materie_Prime")
        sSql.AppendLine(" ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod")
        sSql.AppendLine(" AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")
        sSql.AppendLine(" LEFT OUTER JOIN InsettiUtili")
        sSql.AppendLine(" ON Movimenti_dettagli.Pro_Cod = InsettiUtili.Ins_Cod ")

        sSql.AppendLine(" LEFT OUTER JOIN UnitaMisura")
        sSql.AppendLine(" ON Movimenti_dettagli.Udm_Cod = UnitaMisura.UDM_COD ")

        sSql.AppendLine(" LEFT OUTER JOIN Attivita")
        sSql.AppendLine(" ON Agenda.Id_Attivita = Attivita.ID_Attivita ")

        'Nr domanda ACA
        sSql.AppendLine(" LEFT JOIN (")
        If sql2017 Then
            sSql.AppendLine(" SELECT ipc.Piva, ipc.ProgettoCod, ipc.ContributoTipo,")
            sSql.AppendLine("        STRING_AGG(aca.ContributoDes, ', ') Nr_domanda_ACA")
            sSql.AppendLine(" FROM Imprese_ProgettiXContributi ipc")
            sSql.AppendLine(" JOIN  Contributi aca")
            sSql.AppendLine($" ON ipc.ContributoTipo = {CInt(ContributeType.ACA)}")
            sSql.AppendLine(" AND ipc.ContributoTipo = aca.Tipo")
            sSql.AppendLine(" AND ipc.ContributoCod = aca.ContributoCod")
            sSql.AppendLine(" GROUP BY ipc.Piva, ipc.ProgettoCod, ipc.ContributoTipo")
        Else
            sSql.AppendLine(" SELECT ipc_outer.Piva, ipc_outer.ProgettoCod, ipc_outer.ContributoTipo, ")
            sSql.AppendLine("        STUFF((SELECT ', ' + aca.ContributoDes FROM Imprese_ProgettiXContributi ipc")
            sSql.AppendLine("        INNER JOIN Contributi aca ON ipc.ContributoCod = aca.ContributoCod AND ipc.ContributoTipo = aca.Tipo ")
            sSql.AppendLine("        WHERE ipc.Piva = ipc_outer.Piva AND ipc.ProgettoCod = ipc_outer.ProgettoCod AND ipc.ContributoTipo = ipc_outer.ContributoTipo ")
            sSql.AppendLine("        FOR XML PATH('')), 1, 2, '') AS Nr_domanda_ACA")
            sSql.AppendLine(" FROM ")
            sSql.AppendLine($" (SELECT DISTINCT Piva, ProgettoCod, ContributoTipo FROM Imprese_ProgettiXContributi WHERE ContributoTipo = {CInt(ContributeType.ACA)}) ipc_outer")
        End If
        sSql.AppendLine(" ) CodiciACA")
        sSql.AppendLine(" ON Imprese_Progetti.Piva = CodiciACA.Piva")
        sSql.AppendLine(" AND Imprese_Progetti.Progetto_Cod = CodiciACA.ProgettoCod")

        'sSql.AppendLine("LEFT OUTER JOIN Reg_Impianti_Codici")
        'sSql.AppendLine("ON Reg_Impianti_Codici.PIVA = Imprese_Progetti.Piva")
        'sSql.AppendLine("AND Reg_Impianti_Codici.sa_cod = Imprese_Progetti.sa_cod")
        'sSql.AppendLine("AND Reg_Impianti_Codici.appezza = Imprese_Progetti.APPEZZA ")
        'sSql.AppendLine("AND Reg_Impianti_Codici.id_reg = Imprese_Progetti.ID_REG")
        'sSql.AppendLine("AND Reg_Impianti_Codici.Progetto_Cod = Imprese_Progetti.Progetto_Cod")
        'sSql.AppendLine($"AND Reg_Impianti_Codici.id_cod = {CInt(enum_CodiciAnagrafe.Impianto_Nr_domanda_ACA)}")
        'sSql.AppendLine("AND Imprese_Progetti.validita_inizio <= Agenda.Validita_Inizio")
        'sSql.AppendLine("AND Imprese_Progetti.validita_fine >= Agenda.Validita_Inizio")



        sSql.AppendLine(" LEFT JOIN (")
        sSql.AppendLine("SELECT a.piva, a.Id_Agenda,")
        sSql.AppendLine("COUNT(*) As TotaleImpianti,")
        sSql.AppendLine("(")
        sSql.AppendLine("SELECT COUNT(*) As count_impianti_stampa")
        sSql.AppendLine("FROM Mov_Destinazioni mov_dest_ric")
        sSql.AppendLine("WHERE mov_dest_ric.Piva = a.piva")
        sSql.AppendLine("And mov_dest_ric.Id_Agenda = a.Id_Agenda")
        sSql.AppendLine("And mov_dest_ric.Tipo_Destinazione = 0")
        sSql.AppendLine($" And ({strFiltroAppezza})")
        sSql.AppendLine(") As ImpiantiStampa")
        sSql.AppendLine("FROM agenda a")
        sSql.AppendLine("INNER JOIN Mov_Destinazioni mdest_agenda")
        sSql.AppendLine("On mdest_agenda.Piva = a.piva ")
        sSql.AppendLine("And mdest_agenda.Id_Agenda = a.Id_Agenda ")
        sSql.AppendLine("And mdest_agenda.Tipo_Destinazione = 0")
        sSql.AppendLine("GROUP BY a.piva, a.Id_Agenda")
        sSql.AppendLine(" ) Totali ")
        sSql.AppendLine("On Agenda.PIVA = Totali.PIVA")
        sSql.AppendLine("And Agenda.Id_Agenda = Totali.Id_Agenda")

        sSql.AppendLine("")
        sSql.AppendLine(" WHERE Imprese_Progetti.Regolamento_Cod = " & enum_Cod_Regolamento.Regolamento_bio & " ")

        '------------------------------------------------
        '14/01/2019: aggiunto per escludere le visite che hanno sezione dedicata
        sSql.AppendLine(" And Not EXISTS ( ")
        sSql.AppendLine("               Select 1 ")
        sSql.AppendLine("               FROM Mov_Dettagli_Riferimenti ")
        sSql.AppendLine("               WHERE Mov_Dettagli_Riferimenti.lav_cod_rif = Agenda.lav_cod ")
        sSql.AppendLine("               And Mov_Dettagli_Riferimenti.piva_rif = Agenda.piva ")
        sSql.AppendLine("               And Mov_Dettagli_Riferimenti.id_agenda_rif = Agenda.id_agenda ")
        sSql.AppendLine("               And Mov_Dettagli_Riferimenti.lav_cod = " & LAVCOD_VISITA.ToString)
        sSql.AppendLine("               ) ")
        '------------------------------------------------

        sSql.AppendLine(" And Reg_Impianti.PIVA = '" & Agro_SQL_SaveText(Qs_Piva) & "'")

        If strSa_Cod <> "" Then
            sSql.AppendLine(" AND Reg_Impianti.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(strSa_Cod, False) & ") ")
        End If

        sSql.AppendLine(strFiltroImpianti)

        If StrLav_Cod <> "" Then
            sSql.AppendLine(" AND Agenda.Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(StrLav_Cod, False) & ") ")
        End If

        '24/07/2018: patch
        'in data_movimento c'è salvata anche l'ora (probabilmente recente introduzione)
        'e il confronto su datetime nella ricerca di un preciso giorno scartava i movimenti di quel giorno con orario salvato
        'sSql.appendline(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataInizio) & " " )
        'sSql.appendline(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(DataFine) & " " )
        sSql.AppendLine(" AND CONVERT(Date, Movimenti.Data_Movimento) >= " & Agro_SQL_SaveDate(DataInizio) & " ")
        sSql.AppendLine(" AND CONVERT(Date,Movimenti.Data_Movimento) <= " & Agro_SQL_SaveDate(DataFine) & " ")
        sSql.AppendLine($"AND NOT (Movimenti_dettagli.Elem_Cod = {ALTRE_MATERIE} AND Movimenti_dettagli.Mat_Cod = {MAT_COD_ACQUA_IRRIGAZIONE})")


        If Session("RaggruppaXCampo") = True Then
            'sSql.appendline(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda, Appezzamento.Campo_Cod, Movimenti_dettagli.Udm_Cod, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, NumApp_PAP, Veg_Des   ")
            sSql.AppendLine(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda, Veg_Des, Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, Lotto, Movimenti_dettagli.Udm_Cod, NumApp_PAP   ")
        Else 'come prima
            sSql.AppendLine(" ORDER BY Movimenti.Data_Movimento, Agenda.Id_Agenda, Lotto, NumApp_PAP  ")
        End If

        ' ho specificato la stringa del nome della tabella perchè facendo il porting nel 2010 ci sono stati dei problemi con il dataset, 
        ' ho dovuto rinomiarlo, ma si tiene in memoria il nome della tabella vecchia
        objSQL.EseguiQuery_Lettura(objParametri_Server, sSql.ToString, "SchedaColturaleBiologico.Carica_DSSchedaColturaleBiologico", DSSchedaColturaleBiologico,
                                   "DS_SchedaColturaleBiologico2")

        If DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Count > 0 Then
            'AF 08/25: trovo tutte le operazioni che hanno il raccoglitore_cod valorizzato (quindi multicentro)
            'Per trovare tutti gli impianti influenzati da un altro centro (che non compaiono dalla select)

            'Centro A. Appezzamento 1 --> 100 trappole
            'Centro B. Appezzamento 2 --> 0 trappole
            'In un estrazione per il solo  Centro B non verrebbe mostrato che l'App 2. è influenzato al 100% da impianti di un altro centro

            Dim risultatoRaggruppamentoRaccoglitori = (From row In DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AsEnumerable()
                                                       Where Not IsDBNull(row("Raccoglitore_Cod")) AndAlso
                                                       row("Raccoglitore_Cod") <> 0 AndAlso
                                                       Not IsDBNull(row("Qta")) AndAlso
                                                       Not IsDBNull(row("Lav_Cod")) AndAlso
                                                       (row("Lav_Cod") = LAVCOD_INSTALLAZIONE_TRAPPOLE OrElse row("Lav_Cod") = LAVCOD_CATTURE_MASSA OrElse row("Lav_Cod") = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA)
                                                       Group row By Raccoglitore = row.Field(Of Integer)("Raccoglitore_Cod") Into Group
                                                       Select New With {
                                                                    .Raccoglitore_Cod = Raccoglitore,
                                                                    .TotaleQta = Group.Sum(Function(x) Convert.ToDecimal(x("Qta"))),
                                                                    .NumeroRighe = Group.Count()
                                                                }).ToList()
            Dim raccoglitoriConQtaZero = (From item In risultatoRaggruppamentoRaccoglitori
                                          Where item.TotaleQta = 0
                                          Select item).ToList()
            Dim raccoglitoriConQtaMaggioreZero = (From item In risultatoRaggruppamentoRaccoglitori
                                                  Where item.TotaleQta <> 0
                                                  Select item).ToList()

            'Lascio una sola riga per ogni raccoglitore_cod
            For Each item In raccoglitoriConQtaZero
                Dim counter As Integer = 1
                Do While counter < item.NumeroRighe
                    Dim rowDaRimuovere = (From row In DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AsEnumerable()
                                          Where Not IsDBNull(row("Raccoglitore_Cod")) AndAlso
                                              row("Raccoglitore_Cod") = item.Raccoglitore_Cod
                                          Select row).FirstOrDefault()
                    DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Remove(rowDaRimuovere)
                    counter += 1
                Loop

                'A questo punto mi segno l'ultimo impianto rimasto con qta 0, per poi andare a segnarlo come influenzato da altro centro
                Dim rowDaAggiornare = (From row In DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AsEnumerable()
                                       Where Not IsDBNull(row("Raccoglitore_Cod")) AndAlso
                                           row("Raccoglitore_Cod") = item.Raccoglitore_Cod
                                       Select row).FirstOrDefault()
                rowDaAggiornare("isInfluenzatoDaAltroCentroOImpianto") = 1
            Next

            'Per i raccoglitori con qta maggiore di 0 (casi misti, dove le trappole su sono più impianti), elimino tutte le righe con qta = 0
            For Each item In raccoglitoriConQtaMaggioreZero
                Dim rowsDaRimuovere = (From row In DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AsEnumerable()
                                       Where Not IsDBNull(row("Raccoglitore_Cod")) AndAlso
                                          row("Raccoglitore_Cod") = item.Raccoglitore_Cod AndAlso
                                              Not IsDBNull(row("Qta")) AndAlso
                                           row("Qta") = 0
                                       Select row).ToList()

                If rowsDaRimuovere IsNot Nothing Then
                    For Each row In rowsDaRimuovere
                        DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Remove(row)
                    Next
                End If
            Next



            'Per trovare tutti gli impianti influenzati da un altro impianto (che non compaiono dalla select)
            'Per la stampa BIO è possible selezionare singoli impianti
            Dim risultatoRaggruppamentoAgende = (From row In DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AsEnumerable()
                                                 Where Not IsDBNull(row("Raccoglitore_Cod")) AndAlso
                                                       row("Raccoglitore_Cod") = 0 AndAlso
                                                       Not IsDBNull(row("Qta")) AndAlso
                                                       Not IsDBNull(row("Lav_Cod")) AndAlso
                                                       (row("Lav_Cod") = LAVCOD_INSTALLAZIONE_TRAPPOLE OrElse row("Lav_Cod") = LAVCOD_CATTURE_MASSA OrElse row("Lav_Cod") = LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA)
                                                 Group row By Id_Agenda = row.Field(Of Integer)("Id_Agenda") Into Group
                                                 Select New With {
                                                                    .Id_Agenda = Id_Agenda,
                                                                    .TotaleQta = Group.Sum(Function(x) Convert.ToDecimal(x("Qta"))),
                                                                    .NumeroRighe = Group.Count()
                                                                }).ToList()

            Dim AgendeConQtaZero = (From item In risultatoRaggruppamentoAgende
                                    Where item.TotaleQta = 0
                                    Select item).ToList()
            Dim AgendeConQtaMaggioreZero = (From item In risultatoRaggruppamentoAgende
                                            Where item.TotaleQta <> 0
                                            Select item).ToList()

            'Lascio una sola riga per ogni raccoglitore_cod
            For Each item In AgendeConQtaZero
                Dim counter As Integer = 1
                Do While counter < item.NumeroRighe
                    Dim rowDaRimuovere = (From row In DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AsEnumerable()
                                          Where row("Id_Agenda") = item.Id_Agenda
                                          Select row).FirstOrDefault()
                    DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Remove(rowDaRimuovere)
                    counter += 1
                Loop

                'A questo punto mi segno l'ultimo impianto rimasto con qta 0, per poi andare a segnarlo come influenzato da altro impianto
                Dim rowDaAggiornare = (From row In DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AsEnumerable()
                                       Where row("Id_Agenda") = item.Id_Agenda
                                       Select row).FirstOrDefault()
                rowDaAggiornare("isInfluenzatoDaAltroCentroOImpianto") = 1
            Next

            'Per i raccoglitori con qta maggiore di 0 (casi misti, dove le trappole su sono più impianti), elimino tutte le righe con qta = 0
            For Each item In AgendeConQtaMaggioreZero
                Dim rowsDaRimuovere = (From row In DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AsEnumerable()
                                       Where row("Id_Agenda") = item.Id_Agenda AndAlso
                                              Not IsDBNull(row("Qta")) AndAlso
                                           row("Qta") = 0
                                       Select row).ToList()

                If rowsDaRimuovere IsNot Nothing Then
                    For Each row In rowsDaRimuovere
                        DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Remove(row)
                    Next
                End If
            Next
            ' Accertati di accettare le modifiche per aggiornare il DataTable
            DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AcceptChanges()


        End If

        For i = 0 To DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Count - 1
            Dim drR As DS_SchedaColturaleBiologico.DS_SchedaColturaleBiologico2Row = DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i)
            Select Case drR.Lav_Cod
                Case LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA, LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                    'Forzo l'elem_cod a trappole per farlo entrare nella gestione sotto relativa
                    drR.Elem_Cod = TRAPPOLE
                Case Else
                    Continue For
            End Select
        Next



        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else
            Dim id_agenda As Integer
            'Dim sup_app, memo_sup_app As Decimal
            Dim HTIdAgenda As New Hashtable

            '10/09/2019: il totale sup_app
            For i = 0 To DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Count - 1
                id_agenda = DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i).Item("id_agenda")

                '24/07/2018: visto che già esisteva l'hashtable degli id_agenda,
                'ne approfitto per salvare nel value la sup totale degli impianti
                'poichè nel caso di inst trappole e catture di massa mi serve per ripartire il numero totale delle trappole
                If Not HTIdAgenda.ContainsKey(id_agenda) Then
                    HTIdAgenda.Add(id_agenda, 0)
                End If
            Next
            '--- fine commento

            Dim str_id_agenda As String = ""
            If Not IsNothing(HTIdAgenda) AndAlso HTIdAgenda.Count > 0 Then
                For Each key In HTIdAgenda.Keys
                    str_id_agenda &= key.ToString & ","
                Next
                str_id_agenda = Mid(str_id_agenda, 1, str_id_agenda.Length - 1)
            End If

            Dim objMovDest As New AgronicaCoreContabDAL.Mov_Destinazioni_R
            Dim DtCosti As DataTable

            If str_id_agenda <> "" Then
                Dim objCostiAccessori As New AgronicaCoreContabDAL.CostiAccessori_R
                '16/09/2019: bisogna filtrare dopo con la data operazione
                DtCosti = objCostiAccessori.CostiAccessori_from_IdAgenda2(Qs_Piva, str_id_agenda, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                DTOssFasi = objMovDest.Leggi_Dati_OsservazioneFasi_ByStrIdAgenda(Qs_Piva, str_id_agenda, "", "", objParametri_Server)

            End If

            Dim objnote As New AgronicaCoreContabDAL.AgendaxNote_R
            Dim lotto As String
            Dim strModalitaApplicazione As String = String.Empty

            For i = 0 To DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Count - 1

                Dim drR As DS_SchedaColturaleBiologico.DS_SchedaColturaleBiologico2Row = DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i)

                id_agenda = drR.Id_Agenda
                lotto = drR.Lotto

                Dim modApplDescr As Object = drR("Modalita_Applicazione_Descrizione")
                If Not IsDBNull(modApplDescr) AndAlso Not String.IsNullOrEmpty(modApplDescr) Then
                    strModalitaApplicazione = $"Modalità applicazione: {modApplDescr}"
                End If

                drR.Data = CDate(drR.Data_Movimento).ToShortDateString

                drR.Qta = Arrotonda(drR.Qta, Qs_Arrotondamento)

                '-- tipologia varietale --
                If Not Session("VisualizzaTipologieVarietali") Is Nothing AndAlso Session("VisualizzaTipologieVarietali") = True Then
                    If drR.Grva_Des <> "" Then
                        drR.Cul_Des = drR.Cul_Des + " - " + drR.Grva_Des
                    End If
                End If

                '10/09/2019: inizializzo affinchè non dia errore
                drR.Ha = ""
                drR.Are = ""

                Dim CampoDes = ""

                'PARTO DALLE IMPOSTAZIONI PER DECIDERE SE VEDERE campo, n°aPP BIO, N°APP e quale app.
                If flag_Visual_Campo Then
                    Dim objCampo As New AgronicaCoreAnagrafeDAL.Campi_R
                    CampoDes = objCampo.CampoDes_from_CampoCod(drR.Piva, drR.Sa_Cod, drR.Campo_Cod, objParametri_Server)
                End If

                If Not flag_Nasc_Num_App Then
                    Dim objNum As New AgronicaCoreDataProvider.UtilityProvider
                    Num_Appezzamento = objNum.Numero_from_Stringa(drR.App_Nome)
                    drR.App_Nome = CodiceAppezzamento(CampoDes, drR.Appezza, TipoCodiceAppezzamento, drR.App_Nome, "",
                                                      Num_Appezzamento, drR.Item("Nr_domanda_ACA"), drR.LottoImpianto)
                Else
                    Num_Appezzamento = 0
                    drR.App_Nome = CampoDes
                End If


                If Not flag_Nasc_App_Bio AndAlso drR.NumApp_PAP <> "" Then
                    If Trim(drR.App_Nome) <> Trim(drR.NumApp_PAP) Then
                        drR.App_Nome = drR.App_Nome & " - " & drR.NumApp_PAP
                    End If
                End If

                '---------------------------------------------------------------------------

                Select Case drR.Elem_Cod

                    Case SEMENTI
                        '15/01/2019: aggiunto OR con Session("RaggruppaXCampo") perchè nel caso di raggruppamento era successo che aveva raggruppato aumentando la superficie degli impianti
                        'e quindi se cambia lotto crea nuova riga
                        If Session("RaggruppaXCampo") = True Or flagVisualizzaLottoSemine = True Then
                            If lotto.ToLower = "indefinito" Then
                                lotto = ""
                            End If
                            If lotto <> "" Then
                                lotto = " - lotto: " & lotto
                            End If
                            drR.Materia_Prima = drR.Mat_Des & lotto
                        Else
                            drR.Materia_Prima = drR.Mat_Des
                        End If

                    Case SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI
                        '29/04/2019: aggiunte SEMILAVORATI_VEGETALI, TRASFORMATI_VEGETALI per stampare 'fiori di zucca' nella raccolta dello zucchino

                        '15/01/2019: aggiunto OR con Session("RaggruppaXCampo") perchè nel caso di raggruppamento era successo che aveva raggruppato aumentando la superficie degli impianti
                        'e quindi se cambia lotto crea nuova riga

                        If Session("RaggruppaXCampo") = True Or flagVisualizzaLottoRaccolte = True Then
                            If lotto.ToLower = "indefinito" Then
                                lotto = ""
                            End If
                            If lotto <> "" Then
                                lotto = " - lotto: " & lotto
                            End If
                            drR.Materia_Prima = drR.Mat_Des & lotto
                        Else
                            drR.Materia_Prima = drR.Mat_Des
                        End If

                    Case FERTILIZZANTI
                        drR.Materia_Prima = drR.Fer_Des

                        If drR.Mat_Cod <> 0 Then
                            drR.Materia_Prima = drR.Mat_Des
                        End If

                    Case FORMULATI
                        Dim strAvv As String = ""

                        Dim Id_Mov_Det As Integer = drR.Id_Mov_Det
                        Dim ArrayAvGruTot() As String
                        Dim ArrayAvGru() As String
                        If Not IsDBNull(drR.Av_Gru_Des) AndAlso drR.Av_Gru_Des <> "" Then
                            ArrayAvGruTot = Split(IIf(drR.Av_Gru_Des.Trim.EndsWith(","), drR.Av_Gru_Des.Trim.Remove(drR.Av_Gru_Des.Trim.Length - 1, 1), drR.Av_Gru_Des), ",")
                            If Not ArrayAvGruTot Is Nothing AndAlso ArrayAvGruTot.Length > 0 Then
                                For a = 0 To ArrayAvGruTot.Length - 1
                                    ArrayAvGru = Split(ArrayAvGruTot(a), "|")
                                    If Not ArrayAvGru Is Nothing AndAlso ArrayAvGru.Length > 1 Then
                                        If Id_Mov_Det = ArrayAvGru(1) Or ArrayAvGru(1) = "0" Then
                                            strAvv &= ArrayAvGru(0) & ","
                                        End If
                                    End If
                                Next
                            End If
                        End If
                        Dim ArrayAvTot() As String
                        Dim ArrayAv() As String
                        If Not IsDBNull(drR.Av_Des_Vol) AndAlso drR.Av_Des_Vol <> "" Then
                            ArrayAvTot = Split(IIf(drR.Av_Des_Vol.Trim.EndsWith(","), drR.Av_Des_Vol.Trim.Remove(drR.Av_Des_Vol.Trim.Length - 1, 1), drR.Av_Des_Vol), ",")
                            If Not ArrayAvTot Is Nothing AndAlso ArrayAvTot.Length > 0 Then
                                For a = 0 To ArrayAvTot.Length - 1
                                    ArrayAv = Split(ArrayAvTot(a), "|")
                                    If Not ArrayAv Is Nothing AndAlso ArrayAv.Length > 1 Then
                                        If Id_Mov_Det = ArrayAv(1) Or ArrayAv(1) = "0" Then
                                            strAvv &= ArrayAv(0) & ","
                                        End If
                                    End If
                                Next
                            End If
                        End If

                        If strAvv <> "" Then
                            drR.Materia_Prima = drR.Fr_Des & IIf(strAvv <> "", " (" & Left(strAvv, strAvv.Length - 1) & ")", "")
                        Else
                            drR.Materia_Prima = drR.Fr_Des
                        End If

                    Case TRAPPOLE   'trappole
                        Dim dtAppCoinvolti As New DataTable

                        Select Case drR.Lav_Cod
                            Case LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA
                                drR.Materia_Prima = drR.Fr_Des + " (" & CStr(drR.Pro_Cod) & ")"
                            Case Else
                                drR.Materia_Prima = drR.Trap_Des
                        End Select


                        '----------------------------------------------
                        '10/09/2019: mi sono accorta che anche per LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                        'nel campo qta c'è esattamente il numero di trappole usate sull'impianto,
                        'quindi non serve fare la ripartizione sul sup_app (che tra l'altro non va più bene visto che ora c'è la sup_trattata)
                        drR.Qta = Arrotonda(drR.Qta, Qs_Arrotondamento)

                        Dim Appezzamento_Read As New AgronicaCoreAnagrafeDAL.Appezzamento_Read
                        Dim avversitaread As New AgronicaCoreMetaSchemaDAL.Avversita_R
                        Dim Dt_Trappole_X_Impianti As New DataTable
                        Dim Mov_Dettaglio_Tecnico_R As New AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R

                        Select Case drR.Lav_Cod
                            Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA
                                'la sigla avversità l'ha manipolata, aggiungendoci dei caratteri,
                                'quindi occorre ricavarla dal codice
                                Dim sigl As String = avversitaread.Abbreviazione_from_AvCod(CInt(drR.Item("av_cod")), objParametri_Server)
                                Dt_Trappole_X_Impianti = Mov_Dettaglio_Tecnico_R.Trappole_107122_X_ImpiantiInfluenza(
                                                     CStr(drR.Piva),
                                                     CInt(drR.Sa_Cod),
                                                     sigl,
                                                     0,
                                                      strFiltroImpianti.Substring(4),
                                                     "",
                                                     objParametri_Server,
                                                     joinImpiantixBIO:=True,
                                                     CaricaNrDomandaACA:=True,
                                                     sql2017:=sql2017,
                                                     Id_Agenda:=CInt(drR.Id_Agenda)
                                                     )

                            Case LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA
                                'AF 08/25: per poter gestire i casi multicentro:
                                'Tolto filtro Sa_Cod
                                'Aggiunto filtro Raccoglitore_Cod, da alternare al filtro secco Id_Agenda
                                'Multicentro --> Filtro solo Raccoglitore_Cod
                                'Monocentro --> Filtro solo Id_Agenda
                                Dt_Trappole_X_Impianti = Mov_Dettaglio_Tecnico_R.Trappole_Formulati_X_ImpiantiInfluenza(
                                                     CStr(drR.Piva),
                                                     CInt(drR.Sa_Cod),
                                                     If(drR.Item("Raccoglitore_Cod") = 0, CInt(drR.Id_Agenda), 0),
                                                     If(drR.Item("Raccoglitore_Cod") = 0, 0, drR.Item("Raccoglitore_Cod")),
                                                     CInt(drR.Pro_Cod),
                                                      strFiltroImpianti.Substring(4),
                                                     "",
                                                     objParametri_Server,
                                                     joinImpiantixBIO:=True,
                                                     CaricaNrDomandaACA:=True,
                                                     sql2017:=sql2017)
                        End Select

                        Dim isInfluenzatoDaAltroCentroOImpianto As Integer = drR.Item("isInfluenzatoDaAltroCentroOImpianto")
                        Dim stessoCentro As Boolean = drR.Item("Raccoglitore_Cod") = 0

                        Dim ListaImpiantiInfluenzati As New List(Of String)
                        If Dt_Trappole_X_Impianti.Rows.Count > 1 Then
                            Dim AppxTrappola_Piva As String = drR.Piva
                            Dim AppxTrappola_SaCod As Integer = drR.Sa_Cod
                            Dim AppxTrappola_Appezza As Integer = drR.Appezza

                            For Each impianto In Dt_Trappole_X_Impianti.Rows
                                'Leggo solo le descrizioni degli impianti INFLUENZATI, escludendo quello dove è installata la trappola
                                If impianto.Item("Azienda") <> AppxTrappola_Piva OrElse
                                    impianto.Item("Centro_Aziendale") <> AppxTrappola_SaCod OrElse
                                    impianto.Item("Imp_Influ_Appezza") <> AppxTrappola_Appezza OrElse
                                    isInfluenzatoDaAltroCentroOImpianto = 1 Then
                                    ListaImpiantiInfluenzati.Add(CodiceAppezzamento(impianto.Item("campo_des"),
                                                                                impianto.item("Imp_Influ_Appezza"),
                                                                                TipoCodiceAppezzamento,
                                                                                impianto.Item("app_nome"),
                                                                                impianto.Item("App_Nome_Breve"),
                                                                                -1,
                                                                                impianto.item("Nr_domanda_ACA")
                                                                                ))
                                End If
                            Next
                        End If

                        'aggiungo l'info sugli impianti influenzati se sono piu di uno, quindi se ci sono piu 
                        'impianti oltre quello di installazione
                        If Dt_Trappole_X_Impianti.Rows.Count > 1 Then
                            'Concateno le descrizioni degli impianti influenzati
                            Dim listaImpiantiString As String = String.Join(", ", ListaImpiantiInfluenzati.Distinct().ToList())

                            Dim nomeattuale As String = drR.App_Nome
                            If isInfluenzatoDaAltroCentroOImpianto Then
                                If stessoCentro Then
                                    drR.App_Nome = listaImpiantiString & " (Giustificati da impianti dello stesso centro)"
                                Else
                                    drR.App_Nome = listaImpiantiString & " (Giustificati da altro centro)"
                                End If
                            Else
                                drR.App_Nome = nomeattuale & " (Giustificati: " & listaImpiantiString & ")"
                            End If
                        ElseIf isInfluenzatoDaAltroCentroOImpianto = 1 Then
                            If stessoCentro Then
                                drR.App_Nome = drR.App_Nome & " (Giustificato da impianti dello stesso centro)"
                            Else
                                drR.App_Nome = drR.App_Nome & " (Giustificato da altro centro)"
                            End If
                        End If

                    Case INSETTI
                        drR.Materia_Prima = drR.Item("Ins_Des")


                        Dim strAvv As String = ""

                        Dim Id_Mov_Det As Integer = drR.Id_Mov_Det
                        Dim ArrayAvTot() As String
                        Dim ArrayAv() As String
                        If Not IsDBNull(drR.Item("Av_Cod")) AndAlso drR.Item("Av_Cod") <> "" AndAlso drR.Item("Av_Cod") <> "0" Then
                            If Not IsDBNull(drR.Av_Des_Vol) AndAlso drR.Av_Des_Vol <> "" Then
                                ArrayAvTot = Split(IIf(drR.Av_Des_Vol.Trim.EndsWith(","), drR.Av_Des_Vol.Trim.Remove(drR.Av_Des_Vol.Trim.Length - 1, 1), drR.Av_Des_Vol), ",")
                                If Not ArrayAvTot Is Nothing AndAlso ArrayAvTot.Length > 0 Then
                                    For a = 0 To ArrayAvTot.Length - 1
                                        ArrayAv = Split(ArrayAvTot(a), "|")
                                        If Not ArrayAv Is Nothing AndAlso ArrayAv.Length > 1 Then
                                            If Id_Mov_Det = ArrayAv(1) Or ArrayAv(1) = "0" Then
                                                strAvv &= ArrayAv(0) & ","
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        Else
                            If Not IsDBNull(drR.Item("Av_Gru")) AndAlso drR.Item("Av_Gru") = "0" Then
                                'Non c'è indicato un gruppo, è un insetto impollinatore
                                strAvv &= Gias.DistribuzionePerImpollinazioneNessunaAvversita.ToUpper() & ","
                            End If
                        End If

                        Dim ArrayAvGruTot() As String
                        Dim ArrayAvGru() As String
                        If Not IsDBNull(drR.Item("Av_Gru")) AndAlso drR.Item("Av_Gru") <> "" AndAlso drR.Item("Av_Gru") <> "0" Then
                            If Not IsDBNull(drR.Av_Gru_Des) AndAlso drR.Av_Gru_Des <> "" Then
                                ArrayAvGruTot = Split(IIf(drR.Av_Gru_Des.Trim.EndsWith(","), drR.Av_Gru_Des.Trim.Remove(drR.Av_Gru_Des.Trim.Length - 1, 1), drR.Av_Gru_Des), ",")
                                If Not ArrayAvGruTot Is Nothing AndAlso ArrayAvGruTot.Length > 0 Then
                                    For a = 0 To ArrayAvGruTot.Length - 1
                                        ArrayAvGru = Split(ArrayAvGruTot(a), "|")
                                        If Not ArrayAvGru Is Nothing AndAlso ArrayAvGru.Length > 1 Then
                                            If Id_Mov_Det = ArrayAvGru(1) Or ArrayAvGru(1) = "0" Then
                                                strAvv &= ArrayAvGru(0) & ","
                                            End If
                                        End If
                                    Next
                                End If
                            End If
                        End If

                        If strAvv <> "" Then
                            drR.Materia_Prima = drR.Item("Ins_Des") & IIf(strAvv <> "", " (" & Left(strAvv, strAvv.Length - 1) & ")", "")
                        Else
                            drR.Materia_Prima = drR.Item("Ins_Des")
                        End If

                End Select

                drR.Udm_Des = drR.Udm_Sim

                '---------------------
                '------ LAV_COD ------
                '---------------------
                Dim NoteChecked = ""
                'MODIFICARE QUI PER LE NOTE
                NoteChecked = objnote.ElencoDescrizioniInStringa(drR.Id_Agenda, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

                drR.Note = NoteChecked & drR.Note & " " & strModalitaApplicazione
                strModalitaApplicazione = String.Empty

                drR.Note &= CostiAccessori_Ottimizzata(drR.Piva,
                                                       DtCosti,
                                                       drR.Id_Agenda,
                                                          drR.Data,
                                                        Flag_Nasc_Resp,
                                                        Flag_Nasc_Oper,
                                                        Flag_Nasc_Macc,
                                                        Flag_Nasc_DataFirma,
                                                        Flag_Nasc_DataUltMan
                                                        )


                Select Case drR.Lav_Cod

                    Case LAVCOD_RACCOLTA    'se ho una raccolta indico la produzione

                        RsProd = objMovDest.Leggi_Prodotti_Raccolta(drR.Piva, drR.Sa_Cod, drR.Appezza, drR.Id_Reg, drR.Id_Agenda, "", "", objParametri_Server)

                        If Not IsNothing(RsProd) AndAlso RsProd.Rows.Count > 0 Then

                            Produzione = 0

                            '15/04/2019
                            Dim flag_kg As Boolean = False
                            For j = 0 To RsProd.Rows.Count - 1
                                Select Case RsProd.Rows(j).Item("Udm_Cod")
                                    Case enum_UnitaMisura.KG
                                        Produzione += RsProd.Rows(j).Item("Qta") / 100
                                        flag_kg = True
                                    Case enum_UnitaMisura.Numero
                                        '29/04/2019: gestito round all'intero nel caso di udm = numero (ad esempio fiori di zucca)
                                        Produzione += AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(RsProd.Rows(j).Item("Qta"))
                                    Case Else
                                        Produzione += RsProd.Rows(j).Item("Qta")
                                End Select
                            Next

                            'drR.Produzione = Produzione.ToString
                            drR.Produzione = ""

                            '15/04/2019
                            'drR.Udm_Des = "Q.li"
                            If flag_kg = True Then
                                drR.Udm_Des = "Q.li"
                            Else
                                drR.Udm_Des = RsProd.Rows(0).Item("UDM_SIM")
                            End If

                            drR.Qta = Produzione

                        End If

                    Case LAVCOD_IRRIGAZIONE       'se ho un irrigazione ricavo i dati

                        RsIrrigazione = objMovDest.Leggi_Dati_Irrigazione(drR.Piva, drR.Sa_Cod, drR.Appezza, drR.Id_Reg, drR.Id_Agenda, "", "", objParametri_Server)

                        If Not IsNothing(RsIrrigazione) AndAlso RsIrrigazione.Rows.Count > 0 Then

                            drR.Materia_Prima = "Acqua"

                            Dim strOre As String = ""
                            If Session("RaggruppaXCampo") = False Then
                                'se non è stato selezionato il raggruppa, aggiungo anche le ore
                                'altrimenti non viene calcolato il totale qta
                                If Not IsDBNull(RsIrrigazione.Rows(0).Item("Dose")) Then
                                    strOre = " (" & RsIrrigazione.Rows(0).Item("Dose") & " ore)"
                                End If
                            End If

                            drR.Qta = CStr(Arrotonda(RsIrrigazione.Rows(0).Item("Qta"), Qs_Arrotondamento)) & strOre

                            drR.Udm_Des = RsIrrigazione.Rows(0).Item("UDM_SIM")

                        End If

                    Case LAVCOD_FASI_FENOLOGICHE       'se ho un osservazione fasi fenologiche ricavo i dati

                        '15/06/2018: ottimizzazione per problema coldiretti
                        If Not IsNothing(DTOssFasi) AndAlso DTOssFasi.Rows.Count > 0 Then

                            DrOssFasi = DTOssFasi.Select("Sa_cod=" & drR.Sa_Cod.ToString & " AND appezza=" & drR.Appezza.ToString & " AND Id_Destinazione=" & drR.Id_Reg.ToString)

                            If Not IsNothing(DrOssFasi) Then

                                Dim Fasi As String = ""

                                For j = 0 To DrOssFasi.Count - 1
                                    Fasi &= DrOssFasi(j).Item("ff_des") & " (" & CDate(DrOssFasi(j).Item("data_ril")).ToShortDateString & "), "
                                Next

                                If Fasi <> "" Then
                                    Fasi = Left(Fasi, Fasi.Length - 2)
                                End If

                                drR.Materia_Prima = Fasi

                            End If

                        End If


                    Case LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE    'se ho un rilievo avversita nelle trappole ricavo i dati

                        DtRilievoAvversitaTrappole = objMovDest.Leggi_Dati_RilievoAvversitaTrappole(drR.Piva, drR.Sa_Cod, drR.Appezza, drR.Id_Reg, drR.Id_Agenda, "", "", objParametri_Server)

                        Dim Avv As String = ""

                        If Not IsNothing(DtRilievoAvversitaTrappole) AndAlso DtRilievoAvversitaTrappole.Rows.Count <> 0 Then

                            If DtRilievoAvversitaTrappole.Rows(0).Item("Av_Cod") <> 0 Then
                                drR.Materia_Prima = DtRilievoAvversitaTrappole.Rows(0).Item("av_des_vol") + " (" + DtRilievoAvversitaTrappole.Rows(0).Item("trap_des") + ")"
                            ElseIf DtRilievoAvversitaTrappole.Rows(0).Item("av_gru") <> 0 Then
                                drR.Materia_Prima = DtRilievoAvversitaTrappole.Rows(0).Item("av_gru_des") + " (" + DtRilievoAvversitaTrappole.Rows(0).Item("trap_des") + ")"
                            End If

                            drR.Udm_Des = "n. avv."

                            'drR.Qta = DtRilievoAvversitaTrappole.Rows(0).Item("qta_ril")
                            drR.Qta = Arrotonda(DtRilievoAvversitaTrappole.Rows(0).Item("qta_ril"), Qs_Arrotondamento)

                            For n = 1 To DtRilievoAvversitaTrappole.Rows.Count - 1

                                If DtRilievoAvversitaTrappole.Rows(n).Item("Av_Cod") <> 0 Then
                                    Avv = DtRilievoAvversitaTrappole.Rows(n).Item("av_des_vol")
                                ElseIf DtRilievoAvversitaTrappole.Rows(n).Item("av_gru") <> 0 Then
                                    Avv = DtRilievoAvversitaTrappole.Rows(n).Item("av_gru_des")
                                End If

                                DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AddDS_SchedaColturaleBiologico2Row(drR.Data_Movimento, drR.Veg_Cod, drR.Veg_Des, drR.Cul_Cod,
                                                                                                                           drR.Cul_Des, drR.Appezza, drR.Id_Reg, drR.App_Nome, drR.Sup_App,
                                                                                                                           drR.Ha, drR.Are, drR.Lav_Cod, drR.Lav_Des, "",
                                                                                                                           Avv + " (" + DtRilievoAvversitaTrappole.Rows(n).Item("trap_des") + ")",
                                                                                                                           0, "n. avv.", DtRilievoAvversitaTrappole.Rows(n).Item("qta_ril"),
                                                                                                                           drR.Id_Agenda, drR.Piva, drR.Sa_Cod, drR.Cau_Mov, drR.Data,
                                                                                                                           drR.NumApp_PAP, 0, 0, 0, 0, "", "", "", "", "", drR.Campo_Cod,
                                                                                                                           drR.Grva_Cod, drR.Grva_Des, drR.Av_Des_Vol, drR.Av_Gru_Des,
                                                                                                                           drR.Id_Mov_Det, drR.Note, drR.dose_tot_reale, drR.Udm_Sim_Reale,
                                                                                                                           drR.udm_cod_reale, drR.sup_trattata, drR.Lotto, drR.LottoImpianto,
                                                                                                                           drR.Estrazione)
                            Next


                        End If

                    Case LAVCOD_RILIEVO_AVVERSITA_CAMPO     'se ho un rilievo avversita in campo ricavo i dati

                        DtRilievoAvversitaCampo = objMovDest.Leggi_Dati_RilievoAvversitaCampo(drR.Piva, drR.Sa_Cod, drR.Appezza, drR.Id_Reg, drR.Id_Agenda, "", "", objParametri_Server)

                        If Not IsNothing(DtRilievoAvversitaCampo) AndAlso DtRilievoAvversitaCampo.Rows.Count <> 0 Then

                            For Each rilievo In DtRilievoAvversitaCampo.Rows
                                If drR.Udm_Cod = rilievo.Item("udm_Cod") AndAlso
                                    drR.Item("Av_Cod") = rilievo.Item("Av_Cod") AndAlso
                                    drR.Item("Av_Gru") = rilievo.Item("Av_Gru") Then

                                    If rilievo.Item("Av_Cod") <> 0 Then
                                        drR.Materia_Prima = rilievo.Item("Av_Des_Vol")
                                    Else
                                        drR.Materia_Prima = rilievo.Item("Av_Gru_Des")
                                    End If

                                    drR.Udm_Cod = rilievo.Item("Udm_Cod")
                                    drR.Udm_Des = rilievo.Item("UDM_SIM")

                                    drR.Qta = Arrotonda(rilievo.Item("qta"), Qs_Arrotondamento)

                                End If
                            Next
                        End If

                    Case LAVCOD_RILIEVO_INDICI_MATURITA

                        DtRilievoIndiciMaturita = objMovDest.Leggi_Dati_RilievoIndiciMaturita(drR.Piva, drR.Sa_Cod, drR.Appezza, drR.Id_Reg, drR.Id_Agenda, "", "", objParametri_Server)

                        If Not IsNothing(DtRilievoIndiciMaturita) AndAlso DtRilievoIndiciMaturita.Rows.Count <> 0 Then

                            drR.Materia_Prima = DtRilievoIndiciMaturita.Rows(0).Item("IND_MAT_DES")

                            drR.Udm_Des = DtRilievoIndiciMaturita.Rows(0).Item("UDM_SIM")

                            'drR.Qta = DtRilievoIndiciMaturita.Rows(0).Item("qta")
                            drR.Qta = drR.Qta = Arrotonda(DtRilievoIndiciMaturita.Rows(0).Item("qta"), Qs_Arrotondamento)

                            For n = 1 To DtRilievoIndiciMaturita.Rows.Count - 1

                                DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AddDS_SchedaColturaleBiologico2Row(drR.Data_Movimento, drR.Veg_Cod, drR.Veg_Des, drR.Cul_Cod,
                                                                                                                           drR.Cul_Des, drR.Appezza, drR.Id_Reg, drR.App_Nome, drR.Sup_App,
                                                                                                                           drR.Ha, drR.Are, drR.Lav_Cod, drR.Lav_Des, "",
                                                                                                                           DtRilievoIndiciMaturita.Rows(n).Item("IND_MAT_DES"),
                                                                                                                           DtRilievoIndiciMaturita.Rows(n).Item("UDM_COD"),
                                                                                                                           DtRilievoIndiciMaturita.Rows(n).Item("UDM_SIM"),
                                                                                                                           DtRilievoIndiciMaturita.Rows(n).Item("qta"),
                                                                                                                           drR.Id_Agenda, drR.Piva, drR.Sa_Cod, drR.Cau_Mov, drR.Data,
                                                                                                                           drR.NumApp_PAP, 0, 0, 0, "", "", "", "", "", "", drR.Campo_Cod,
                                                                                                                           drR.Grva_Cod, drR.Grva_Des, drR.Av_Des_Vol, drR.Av_Gru_Des,
                                                                                                                           drR.Id_Mov_Det, drR.Note, drR.dose_tot_reale, drR.Udm_Sim_Reale,
                                                                                                                           drR.udm_cod_reale, drR.sup_trattata, drR.Lotto, drR.LottoImpianto,
                                                                                                                           drR.Estrazione)
                            Next

                        End If

                    Case LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE

                        drR.Udm_Des = "n. feromoni"

                End Select

                If CStr(drR.Udm_Des).ToLower = "non definito" Then
                    drR.Udm_Des = ""
                End If


                'raggruppa x intervento (non so perchè si chiama "x campo""
                If Session("RaggruppaXCampo") = True Then '-----------

                    Dim Matr_Prec() As String
                    Dim f As Integer = 0
                    Dim trovato As Boolean = False
                    Dim HT_Imp As New Hashtable
                    Dim key_imp As String


                    trovato = False
                    Matr_Prec = Nothing

                    If i > 0 Then

                        Dim drR_Prec As DS_SchedaColturaleBiologico.DS_SchedaColturaleBiologico2Row = DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i - 1)

                        ''If (drR.Data = drR_Prec.Data) AndAlso _
                        ''(drR.Piva = drR_Prec.Piva) AndAlso _
                        ''(drR.Sa_Cod = drR_Prec.Sa_Cod) AndAlso _
                        ''(drR.Campo_Cod = drR_Prec.Campo_Cod) AndAlso _
                        ''(drR.Campo_Cod <> 0) AndAlso _
                        ''(drR.Veg_Cod = drR_Prec.Veg_Cod) AndAlso _
                        ''(drR.Udm_Cod = drR_Prec.Udm_Cod) AndAlso _
                        ''(drR.Elem_Cod = drR_Prec.Elem_Cod) AndAlso _
                        ''(drR.Pro_Cod = drR_Prec.Pro_Cod) AndAlso _
                        ''(drR.Mat_Cod = drR_Prec.Mat_Cod) AndAlso _
                        ''(drR.Lav_Cod = drR_Prec.Lav_Cod) Then


                        'If (drR.Data = drR_Prec.Data) AndAlso _
                        '   (drR.Piva = drR_Prec.Piva) AndAlso _
                        '   (drR.Sa_Cod = drR_Prec.Sa_Cod) AndAlso _
                        '   (drR.Campo_Cod = drR_Prec.Campo_Cod) AndAlso _
                        '   (drR.Veg_Cod = drR_Prec.Veg_Cod) AndAlso _
                        '   (drR.Udm_Cod = drR_Prec.Udm_Cod) AndAlso _
                        '   (drR.Elem_Cod = drR_Prec.Elem_Cod) AndAlso _
                        '   (drR.Pro_Cod = drR_Prec.Pro_Cod) AndAlso _
                        '   (drR.Mat_Cod = drR_Prec.Mat_Cod) AndAlso _
                        '   (drR.Lav_Cod = drR_Prec.Lav_Cod) Then

                        '05/09/2019: corretto raggruppamento perchè se c'era lo stesso tipo di operazione nella stessa data, raggruppava insieme e moltiplicava la superficie
                        If (drR.Id_Agenda = drR_Prec.Id_Agenda) AndAlso
                        (drR.Piva = drR_Prec.Piva) AndAlso
                        (drR.Veg_Cod = drR_Prec.Veg_Cod) AndAlso
                        (drR.Elem_Cod = drR_Prec.Elem_Cod) AndAlso
                        (drR.Pro_Cod = drR_Prec.Pro_Cod) AndAlso
                        (drR.Mat_Cod = drR_Prec.Mat_Cod) AndAlso
                        (drR.Lotto = drR_Prec.Lotto) AndAlso
                        (drR.Udm_Cod = drR_Prec.Udm_Cod) AndAlso
                        (drR.Item("Av_Cod") = drR_Prec.Item("Av_Cod")) AndAlso
                        (drR.Item("Av_Gru") = drR_Prec.Item("Av_Gru")) Then
                            '------------------------------------
                            '--- concatenazione materia prima ---
                            If drR.IsMateria_PrimaNull = True Then
                                drR.Materia_Prima = ""
                            End If

                            If drR_Prec.IsMateria_PrimaNull = True Then
                                drR_Prec.Materia_Prima = ""
                                trovato = False
                            Else
                                If drR.Lav_Cod = LAVCOD_DISTRIBUZIONE_INSETTI AndAlso drR_Prec.Materia_Prima.ToLowerInvariant().Contains(Gias.DistribuzionePerImpollinazioneNessunaAvversita.ToLowerInvariant()) Then
                                    'Per gli insetti, dove l'avversità può essere 0 (per impollinazione), salto lo split
                                    Matr_Prec = {drR_Prec.Materia_Prima}
                                Else
                                    Matr_Prec = Split(drR_Prec.Materia_Prima, ", ")
                                End If

                                For f = 0 To Matr_Prec.Length - 1
                                    If Matr_Prec(f) = drR.Materia_Prima Then
                                        trovato = True
                                        Exit For
                                    End If
                                Next

                            End If

                            If trovato = False Then
                                'qui non ci dovrebbe mai entrare, dato che se cambia mat_cod fa una nuova riga
                                drR.Materia_Prima = drR_Prec.Materia_Prima + ", " + drR.Materia_Prima
                            Else
                                drR.Materia_Prima = drR_Prec.Materia_Prima
                            End If

                            trovato = False
                            Matr_Prec = Nothing

                            '-------------------------------------
                            '--- concatenazione varietà ---
                            Matr_Prec = Split(drR_Prec.Cul_Des, ", ")

                            For f = 0 To Matr_Prec.Length - 1
                                If Matr_Prec(f) = drR.Cul_Des Then
                                    trovato = True
                                    Exit For
                                End If
                            Next

                            If trovato = False Then
                                drR.Cul_Des = drR_Prec.Cul_Des + ", " + drR.Cul_Des
                            Else
                                drR.Cul_Des = drR_Prec.Cul_Des
                            End If

                            trovato = False
                            Matr_Prec = Nothing

                            '--- concatenazione app_des ---
                            Matr_Prec = Split(drR_Prec.App_Nome, ", ")
                            For f = 0 To Matr_Prec.Length - 1
                                If Matr_Prec(f) = drR.App_Nome Then
                                    trovato = True
                                    Exit For
                                End If
                            Next
                            If trovato = False Then
                                drR.App_Nome = drR_Prec.App_Nome + ", " + vbCrLf + drR.App_Nome
                            Else
                                drR.App_Nome = drR_Prec.App_Nome
                            End If

                            trovato = False
                            Matr_Prec = Nothing

                            '05/09/2019: correzione bug segnalato da Bandinelli
                            ''--- somma sup_imp  ---
                            'drR.Sup_App = CStr(CDbl(drR.Sup_App) + CDbl(drR_Prec.Sup_App))
                            key_imp = drR.Piva & "_" & drR.Sa_Cod.ToString & "_" & drR.Appezza.ToString & "_" & drR.Id_Reg.ToString
                            If Not HT_Imp.Contains(key_imp) Then
                                'all'interno della stessa op di agenda sommo le sup degli impianti
                                '(non si deve sommare più volte la sup di uno stesso impianto)
                                HT_Imp.Add(key_imp, "")
                                SupColtura_Tot += CDec(drR.Sup_App)
                                drR.Sup_App = SupColtura_Tot

                                SupTrattata_Tot += CDec(drR.sup_trattata)
                                drR.sup_trattata = SupTrattata_Tot
                            End If


                            'EttariAreCentiare_from_Ettari(CDbl(drR.Sup_App), Ettari, Are, centiare)

                            'If InStr(drR.Sup_App, ",") <> 0 Then
                            '    drR.Ha = Ettari.ToString
                            '    drR.Are = Are.ToString
                            'Else
                            '    drR.Ha = drR.Sup_App
                            '    drR.Are = ""
                            'End If


                            '--- somma quantità impegata ---
                            'Controlli in caso di dbnull
                            If drR_Prec.IsQtaNull Then
                                drR_Prec.Qta = "0"
                            End If
                            If drR.IsQtaNull Then
                                drR.Qta = "0"
                            End If

                            If Not IsNumeric(drR_Prec.Qta) Then
                                drR_Prec.Qta = "0"
                            End If
                            If Not IsNumeric(drR.Qta) Then
                                drR.Qta = "0"
                            End If

                            '24/07/2019: cambiata gestione sulla qta per evitare di stampare valori arrotondati male
                            '(la somma delle qta non dava esattamente la qta totale -> ora stampata la qta totale salvata sull'operazione)
                            'nel CASE ELSE fa quello che faceva prima
                            Select Case drR.Lav_Cod
                                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISSECCAMENTO, LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                                     LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                                     LAVCOD_FERTIRRIGAZIONE

                                    If drR.Estrazione = "Totale" AndAlso CDec(drR.dose_tot_reale) <> 0 Then
                                        drR.Qta = ArrotondaVal(QtaTrasformata_from_QtaReale(CDec(drR.dose_tot_reale), drR.udm_cod_reale), Qs_Arrotondamento)

                                        '17/10/2019: introdotto controllo perchè le operazioni create fino ad una certa del 2018 non hanno i nuovi campi valorizzati,
                                        'per cui in stampa non verrebbero stampate le qta
                                        'If CDec(drR.dose_tot_reale) <> 0 Then

                                        'drR.Qta = ArrotondaVal(QtaTrasformata_from_QtaReale(CDec(drR.dose_tot_reale), drR.udm_cod_reale), Qs_Arrotondamento)

                                        drR.Udm_Des = UdmSimTrasformato_from_UdmCodReale(drR.udm_cod_reale, drR.Udm_Sim_Reale)
                                    Else
                                        'faccio quello che faceva prima del 24/07/2019 -> ci saranno le qta non arrotondate bene
                                        drR.Qta = ArrotondaVal(CDec(drR.Qta) + CDec(drR_Prec.Qta), Qs_Arrotondamento)
                                    End If

                                Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING

                                    If drR.Estrazione = "Totale" AndAlso CDec(drR.dose_tot_reale) <> 0 Then
                                        drR.Qta = ArrotondaVal(QtaTrasformata_from_QtaReale(CDec(drR.dose_tot_reale), drR.udm_cod_reale), Qs_Arrotondamento)
                                    Else
                                        drR.Qta = ArrotondaVal(CDec(drR.Qta) + CDec(drR_Prec.Qta), Qs_Arrotondamento)
                                    End If
                                    drR.Udm_Des = UdmSimTrasformato_from_UdmCodReale(drR.udm_cod_reale, drR.Udm_Sim_Reale)

                                Case Else
                                    '14/09/2018: introdotti i decimal e utilizzata la nuova funzione dia rrotondamento (così siamo sicuri dell'arrotondamento che viene fatto)
                                    'drR.Qta = Arrotonda(CDbl(drR.Qta) + CDbl(drR_Prec.Qta), Qs_Arr otondamento)

                                    'Per la Raccolta arrotondo alla fine le Qta in base al numero di decimali scelti
                                    If drR.Lav_Cod = LAVCOD_RACCOLTA Then
                                        drR.Qta = CDec(drR.Qta) + CDec(drR_Prec.Qta)
                                    Else
                                        drR.Qta = ArrotondaVal(CDec(drR.Qta) + CDec(drR_Prec.Qta), Qs_Arrotondamento)
                                    End If

                            End Select

                            '--- Sommo la produzione (in caso di raccolta) ---
                            If drR.Lav_Cod = 125 Then
                                If Not IsNumeric(drR.Produzione) Then
                                    drR.Produzione = "0"
                                End If
                                If Not IsNumeric(drR_Prec.Produzione) Then
                                    drR_Prec.Produzione = "0"
                                End If

                                drR.Produzione = CStr(CDec(drR.Produzione) + CDec(drR_Prec.Produzione))

                            End If

                            '--- Flaggo le righe da cancellare con 0 in id_agenda
                            drR_Prec.Id_Agenda = 0

                        Else
                            'nuova operazione o nuova materia prima o nuova udm o nuovo lotto o nuova specie

                            'riazzero l'ht
                            HT_Imp = New Hashtable
                            SupColtura_Tot = 0
                            SupTrattata_Tot = 0
                            key_imp = drR.Piva & "_" & drR.Sa_Cod.ToString & "_" & drR.Appezza.ToString & "_" & drR.Id_Reg.ToString
                            HT_Imp.Add(key_imp, "") '15/01/2020: patch, mancava l'add
                            SupColtura_Tot += CDec(drR.Sup_App)
                            drR.Sup_App = SupColtura_Tot

                            SupTrattata_Tot += CDec(drR.sup_trattata)
                            drR.sup_trattata = SupTrattata_Tot

                        End If
                    Else
                        '1° riga
                        key_imp = drR.Piva & "_" & drR.Sa_Cod.ToString & "_" & drR.Appezza.ToString & "_" & drR.Id_Reg.ToString
                        HT_Imp.Add(key_imp, "")
                        SupColtura_Tot = drR.Sup_App
                        SupTrattata_Tot = drR.sup_trattata
                    End If

                End If ' ------ Fine if --- Session("RaggruppaXCampo") = True

                '29-08-22 Casanova: Commentato perchè nella Trappole quando la quantità era 0 non mostrava nulla nella colonna quantità (chiamat nr 19643) 
                'If drR.Qta = "0" Then
                '    drR.Qta = ""
                'End If

            Next

            'Ciclo cancellando gli id_agenda = 0 
            For i = 0 To DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Count - 1

                'Arrotondo alla fine i decimali della Qta Raccolta
                If DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i).Item("Lav_Cod") = LAVCOD_RACCOLTA Then
                    DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i).Item("Qta") = ArrotondaVal(DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i).Item("Qta"), Qs_Arrotondamento)
                End If

                '10/09/2019:aggiunto arrotondamento a 4 della sup_trattata (lo faccio qui anzichè sopra, così vengono sommati valori non arrotondati)
                DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i).Item("sup_trattata") = ArrotondaVal_4(DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i).Item("sup_trattata"))

                If DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i).Item("id_agenda") = 0 Then
                    DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i).Delete()
                End If

            Next

            DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.AcceptChanges()

            If Session("RaggruppaXCampo") = True Then
                'Ciclo cancellando gli id_agenda = 0 
                For i = 0 To DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows.Count - 1

                    If i > 0 Then
                        Dim drR As DS_SchedaColturaleBiologico.DS_SchedaColturaleBiologico2Row = DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i)
                        Dim drR_Prec As DS_SchedaColturaleBiologico.DS_SchedaColturaleBiologico2Row = DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico2.Rows(i - 1)

                        '05/09/2019
                        'If (drR.Id_Agenda = drR_Prec.Id_Agenda AndAlso drR_Prec.Campo_Cod = drR.Campo_Cod) Then
                        If (drR.Id_Agenda = drR_Prec.Id_Agenda) AndAlso
                             (drR.Piva = drR_Prec.Piva) AndAlso
                             (drR.Veg_Cod = drR_Prec.Veg_Cod) Then

                            drR.Data = ""
                            drR.Veg_Des = ""
                            drR.Cul_Des = ""
                            drR.App_Nome = ""
                            drR.Ha = ""
                            drR.Are = ""
                            drR.sup_trattata = ""
                            drR.Sup_App = ""
                            drR.Lav_Des = ""

                            Select Case drR.Lav_Cod
                                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, LAVCOD_TRATTAMENTO_FITOREGOLATORE, LAVCOD_GEODISINFESTAZIONE, LAVCOD_DISSECCAMENTO, LAVCOD_TRATTAMENTO_POST_RACCOLTA,
                                     LAVCOD_DISTRIBUZIONE_CONCIME, LAVCOD_CONCIMAZIONE_FOGLIARE, LAVCOD_DISTRIBUZIONE_AMMENDANTI, LAVCOD_SARCHIATURA_CONCIMAZIONE, LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
                                     LAVCOD_FERTIRRIGAZIONE
                                    drR.Note = "* Attenzione, la presente riga è da intendersi in miscela con la precedente (stessa data, stessa botte)."
                                Case Else
                                    drR.Note = ""
                            End Select
                        End If
                    End If
                Next
            End If

            ''Riordino il dataset in seguito all'aggiunta delle nuove colonne..
            ''per fare ciò utilizzo un'altro dataset..
            'Dim dr() As DS_SchedaColturaleBiologico.DS_SchedaColturaleBiologicoRow = DSSchedaColturaleBiologico.DS_SchedaColturaleBiologico.Select("", "Data_Movimento,Id_Agenda,App_Nome")

            'For j = 0 To UBound(dr)
            '    DS_SchedaColturaleBiologico_Sort.DS_SchedaColturaleBiologico.ImportRow(dr(j))
            'Next

        End If


    End Sub

    '########################################################
    Function UdmSimTrasformato_from_UdmCodReale(ByVal udm_cod_reale As Integer,
                                                ByVal udm_sim_reale As String) As String

        Dim UdmSimTrasformato As String = ""

        Select Case udm_cod_reale
            Case enum_UnitaMisura.Grammi
                'UdmCodTrasformato = 2
                UdmSimTrasformato = "kg"
            Case enum_UnitaMisura.Quintali
                'UdmCodTrasformato = 2
                UdmSimTrasformato = "kg"
            Case enum_UnitaMisura.Tonnellate
                'UdmCodTrasformato = 2
                UdmSimTrasformato = "kg"
            Case enum_UnitaMisura.Milligrammi
                'UdmCodTrasformato = 2
                UdmSimTrasformato = "kg"

            Case enum_UnitaMisura.Millilitri
                'UdmCodTrasformato = 29
                UdmSimTrasformato = "l"
            Case enum_UnitaMisura.CentimetriCubi  'cc
                ' UdmCodTrasformato = 29
                UdmSimTrasformato = "l"
            Case enum_UnitaMisura.Metri_Cubi  'metri cubi
                'UdmCodTrasformato = 29
                UdmSimTrasformato = "l"

            Case enum_UnitaMisura.KG, enum_UnitaMisura.Litri
                ' UdmCodTrasformato = UdmCod
                UdmSimTrasformato = udm_sim_reale

            Case enum_UnitaMisura.Unita_Seme, enum_UnitaMisura.Num_Piante, enum_UnitaMisura.Confezioni
                '  UdmCodTrasformato = UdmCod
                UdmSimTrasformato = udm_sim_reale

            Case Else
                UdmSimTrasformato = udm_sim_reale
        End Select


        Return UdmSimTrasformato

    End Function

    '###############################
    Function QtaTrasformata_from_QtaReale(ByVal qta_reale As Decimal,
                                          ByVal udm_cod_reale As Integer) As String

        Dim qta_Trasformata As Decimal = 0

        Select Case udm_cod_reale 'verificare queste conversioni
            Case enum_UnitaMisura.Grammi
                qta_Trasformata = qta_reale / 1000

            Case enum_UnitaMisura.Milligrammi
                qta_Trasformata = qta_reale / 1000000

            Case enum_UnitaMisura.Quintali
                qta_Trasformata = qta_reale * 100

            Case enum_UnitaMisura.Tonnellate
                qta_Trasformata = qta_reale * 1000

            Case enum_UnitaMisura.Millilitri
                qta_Trasformata = qta_reale / 1000

            Case enum_UnitaMisura.CentimetriCubi
                qta_Trasformata = qta_reale / 1000

            Case enum_UnitaMisura.KG, enum_UnitaMisura.Litri
                qta_Trasformata = qta_reale

            Case enum_UnitaMisura.Unita_Seme, enum_UnitaMisura.Num_Piante, enum_UnitaMisura.Confezioni
                qta_Trasformata = qta_reale

            Case Else
                qta_Trasformata = qta_reale

        End Select

        Return qta_Trasformata

    End Function

    '########################################################
    Function CostiAccessori_Ottimizzata(ByVal Piva As String,
                                        ByVal DtCosti As DataTable,
                                           ByVal id_agenda As Integer,
                                           ByVal data As Date,
                                           ByVal Flag_Nasc_Resp As Boolean,
                                           ByVal Flag_Nasc_Oper As Boolean,
                                           ByVal Flag_Nasc_Macc As Boolean,
                                           ByVal Flag_Nasc_DataFirma As Boolean,
                                           ByVal Flag_Nasc_DataUltMan As Boolean
                                           ) As String

        'var aggiunte
        Dim nomeContatto As String = ""
        Dim strOperatori As String = ""
        Dim Patentino As String = ""
        Dim Titolare As String = ""
        Dim Data_Scadenza_Patentino As String = ""
        Dim Num_Operatori As Integer = 0
        Dim strResponsabili As String = ""
        Dim Num_Responsabili As Integer = 0
        Dim strMacchine As String = ""
        Dim Hash_Patentino As Hashtable
        Dim Num_Macchine As Integer = 0
        Dim strAppezzamenti As String = ""
        Dim strCosti As String = ""

        Dim strTot As String = ""

        If Not DtCosti Is Nothing AndAlso DtCosti.Rows.Count > 0 Then

            '29/01/2020: separate macchine e operatori per evitare il filtro delle date di patentino

            '----------------------------------
            '--------- OPERATORI -------------
            '----------------------------------

            '17/09/2019:
            'Dim DrCosti() As DataRow = DtCosti.Select("Id_Agenda=" & id_agenda.ToString)
            Dim DrCosti() As DataRow = DtCosti.Select(" Elem_cod = 0 " &
                                                      " AND Id_Agenda = " & id_agenda.ToString &
                                                      " AND Data_Rilascio_Patentino <= #" & CDate(data).ToString("MM/dd/yyyy") & "#" &
                                                      " AND Data_Scadenza_Patentino >= #" & CDate(data).ToString("MM/dd/yyyy") & "#")

            If Not DrCosti Is Nothing AndAlso DrCosti.Length > 0 Then

                For j = 0 To DrCosti.Length - 1

                    If DrCosti(j).Item("Rag_Soc") <> "" Then
                        nomeContatto = DrCosti(j).Item("Rag_Soc")
                    Else
                        nomeContatto = String.Format("{0} {1}", DrCosti(j).Item("Cognome"), DrCosti(j).Item("Nome"))
                    End If


                    If DrCosti(j).Item("Cau_Mov") = CAU_IMPUTAZIONE_TERZISTI Then

                        nomeContatto &= " (Terzista)"

                        If Not Flag_Nasc_DataFirma Then
                            nomeContatto &= " Firma ___________"
                        End If

                        nomeContatto &= vbCrLf

                    End If


                    'Controllo se è un responsabile o un operatore
                    If DrCosti(j).Item("Cau_Mov") <> CAU_IMPUTAZIONE_TECNICO_RESPONSABILE Then

                        strOperatori &= nomeContatto '& ","

                        If DrCosti(j).Item("Cau_Mov") <> CAU_IMPUTAZIONE_TERZISTI Then

                            If Not Flag_Nasc_DataFirma Then
                                strOperatori &= " Firma ___________"
                            End If

                        End If

                        strOperatori &= vbCrLf

                        If Not IsDBNull(DrCosti(j).Item("patentino")) AndAlso DrCosti(j).Item("patentino") <> "" Then

                            If Patentino = "" Then
                                Patentino = DrCosti(j).Item("patentino")
                            Else
                                If InStr(Patentino, DrCosti(j).Item("patentino")) = 0 Then
                                    Patentino &= " - " & DrCosti(j).Item("patentino")
                                End If
                            End If

                            If Titolare = "" Then
                                Titolare = nomeContatto
                            Else
                                If InStr(Titolare, nomeContatto) = 0 Then
                                    Titolare &= " - " & nomeContatto
                                End If
                            End If

                            If Data_Scadenza_Patentino = "" Then
                                If IsDate(DrCosti(j).Item("data_scadenza_patentino")) AndAlso
                                   CDate(DrCosti(j).Item("data_scadenza_patentino")) <> CDate(AGRODATAINIZIO) AndAlso
                                   CDate(DrCosti(j).Item("data_scadenza_patentino")) <> CDate(AGRODATAFINE) Then
                                    Data_Scadenza_Patentino = DrCosti(j).Item("data_scadenza_patentino")
                                End If
                            Else
                                If InStr(Data_Scadenza_Patentino, DrCosti(j).Item("data_scadenza_patentino")) = 0 Then
                                    If IsDate(DrCosti(j).Item("data_scadenza_patentino")) AndAlso
                                      CDate(DrCosti(j).Item("data_scadenza_patentino")) <> CDate(AGRODATAINIZIO) AndAlso
                                      CDate(DrCosti(j).Item("data_scadenza_patentino")) <> CDate(AGRODATAFINE) Then
                                        Data_Scadenza_Patentino &= " - " & DrCosti(j).Item("data_scadenza_patentino")
                                    End If
                                End If
                            End If

                        End If

                        Num_Operatori += 1

                    Else

                        strResponsabili &= nomeContatto '& ","

                        If Not Flag_Nasc_DataFirma Then
                            strResponsabili &= " Firma ___________"
                        End If

                        strResponsabili &= vbCrLf

                        Num_Responsabili += 1

                    End If

                    'If Not Hash_Patentino.ContainsKey(DrCosti(j).Item("Cod_RisUm")) AndAlso DrCosti(j).Item("Patentino") <> "" Then
                    '    Hash_Patentino.Add(DrCosti(j).Item("Cod_RisUm"), DrCosti(j).Item("Cod_RisUm"))
                    '    'CaricaDsPatentinoRow(DrCosti(j))
                    'End If

                Next

            End If ' filtro dr

            '----------------------------------
            '--------- MACCHINE  -------------
            '----------------------------------
            DrCosti = DtCosti.Select(" ELEM_COD = 1 " &
                                      " AND Id_Agenda = " & id_agenda.ToString)

            If Not DrCosti Is Nothing AndAlso DrCosti.Length > 0 Then

                strMacchine = ""

                For j = 0 To DrCosti.Length - 1

                    'è un record macchinario
                    strMacchine += DrCosti(j).Item("CLASS_DESC")

                    If DrCosti(j).Item("Modello") <> "" Then
                        strMacchine += " - Modello " & DrCosti(j).Item("Modello")
                    End If

                    If DrCosti(j).Item("Ditta_Des") <> "" Then
                        strMacchine += " - Marca " & DrCosti(j).Item("Ditta_Des")
                    End If

                    If Flag_Nasc_DataUltMan = False AndAlso DrCosti(j).Item("Ultima_Manutenzione") <> "01/01/1900" Then
                        strMacchine += " - Ultima Manutenzione " & DrCosti(j).Item("Ultima_Manutenzione")
                    End If

                    strMacchine += ","

                    Num_Macchine += 1

                Next

            End If ' filtro dr


        End If 'dt costi 


        If Num_Responsabili > 0 AndAlso Not Flag_Nasc_Resp Then
            strCosti = "Autorizzato da : " & vbCrLf & Left(strResponsabili, strResponsabili.Length - 2)
        End If

        If (Num_Operatori > 0 AndAlso Not Flag_Nasc_Oper) And (Num_Macchine > 0 AndAlso Not Flag_Nasc_Macc) Then

            If Trim(strCosti) <> "" Then
                strCosti = String.Format("{0}{1}Eseguito da :" & vbCrLf & "{2} con : {3}", strCosti, vbCrLf, Left(strOperatori, strOperatori.Length - 2), Left(strMacchine, strMacchine.Length - 1))
            Else
                strCosti = String.Format("Eseguito da :" & vbCrLf & "{0} con : {1}", Left(strOperatori, strOperatori.Length - 2), Left(strMacchine, strMacchine.Length - 1))
            End If

        ElseIf (Num_Operatori > 0 AndAlso Not Flag_Nasc_Oper) And (Num_Macchine = 0 Or Flag_Nasc_Macc) Then

            If Trim(strCosti) <> "" Then
                strCosti = String.Format("{0}{1}Eseguito da :" & vbCrLf & "{2}", strCosti, vbCrLf, Left(strOperatori, strOperatori.Length - 2))
            Else
                strCosti = String.Format("Eseguito da :" & vbCrLf & "{0}", Left(strOperatori, strOperatori.Length - 2))
            End If

        ElseIf (Num_Operatori = 0 Or Flag_Nasc_Oper) And (Num_Macchine > 0 AndAlso Not Flag_Nasc_Macc) Then

            If Trim(strCosti) <> "" Then
                strCosti = String.Format("{0}{1}Eseguito con :" & vbCrLf & "{2}", strCosti, vbCrLf, Left(strMacchine, strMacchine.Length - 1))
            Else
                strCosti = String.Format("Eseguito con :" & vbCrLf & "{0}", Left(strMacchine, strMacchine.Length - 1))
            End If

        End If

        ''appezzamenti
        'If Not (Not Flag_Multispecie AndAlso Not Flag_Multicentro) Then
        '    strTot = "App. " & strAppezzamenti & vbCrLf
        'End If

        ''operazione
        'strTot &= Lav_Des & vbCrLf

        ''fase/epoca etichetta
        'If FasiEpoche <> "" Then
        '    strTot &= FasiEpoche & vbCrLf
        'End If

        ''epoca diserbo
        'Select Case Lav_Cod
        '    Case 18
        '        If Dt_Operazione.Rows(0).Item("Epoca_Des") <> "" Then
        '            strTot &= Dt_Operazione.Rows(0).Item("Epoca_Des") & vbCrLf
        '        End If
        'End Select

        ''nota
        'If Mov_Desc <> "" Then
        '    strTot &= Mov_Desc & vbCrLf
        'End If

        ''note
        'If strNote <> "" Then
        '    strTot &= strNote '& vbCrLf
        'End If

        ''ricetta
        'If strRicetta <> "" Then
        '    strTot &= strRicetta & vbCrLf
        'End If

        'costi accessori
        If strCosti <> "" Then
            strTot &= strCosti & vbCrLf
        End If

        If strTot <> "" Then
            strTot = Left(strTot, strTot.Length - vbCrLf.Length)
        End If

        Return strTot

    End Function

    '(28/10/2014 modifica Fede) 
    'PRIORITA:
    'Se l'utente ha settato l'impostazione viene impostato in base all'impostazione scelta
    'Altrimenti la priorità è: 
    '1.RiferimentoAlfanumerico nell'appezzamento
    '2.Numeri nell'App_Nome
    '3.Appezza - BaseCode
    '(24/03/2015 modifica Fede) 
    'PRIORITA:
    'Se l'utente ha settato l'impostazione viene impostato in base all'impostazione scelta
    'Altrimenti la priorità è: 
    '1.RiferimentoAlfanumerico nell'appezzamento
    '2.App_Nome
    '3.Numeri nell'App_Nome
    '4.Appezza - BaseCode
    '
    ' Ultima modifica Estate 2016 - Inserito anche il riferimento al campo.
    Private Function CodiceAppezzamento(ByVal Campo_Des As String,
                                        ByVal Appezza As Integer,
                                        ByVal TipoCodiceAppezzamento As enum_TipoCodiceAppezzamento_SchedaCampagna,
                                        ByVal App_Nome As String,
                                        ByVal App_Nome_Breve As String,
                                        ByVal N_Appezza As Double,
                                        ByVal nrDomandaACA As Object,
                                        Optional Lotto As String = "") As String

        Dim strCodiceAppezzamento As String = ""
        Dim Num_Appezzamento As Long

        If TipoCodiceAppezzamento = enum_TipoCodiceAppezzamento_SchedaCampagna.RiferimentoAlfanumerico Then
            strCodiceAppezzamento = App_Nome_Breve
        End If

        If TipoCodiceAppezzamento = enum_TipoCodiceAppezzamento_SchedaCampagna.NumeriInAppNome Then
            Dim objNum As New AgronicaCoreDataProvider.UtilityProvider
            Num_Appezzamento = objNum.Numero_from_Stringa(App_Nome)
            If Num_Appezzamento = 0 Then
                strCodiceAppezzamento = ""
            Else
                strCodiceAppezzamento = CStr(Num_Appezzamento)
            End If
        End If

        If TipoCodiceAppezzamento = enum_TipoCodiceAppezzamento_SchedaCampagna.AppezzaMenoBasecode Then
            ' in molti ds non viene calcolato N_Appezza quindi lo calcolo qui
            If N_Appezza < 0 Then
                N_Appezza = Appezza - BaseCode
            End If
            'se è un valore alto lo tronco alle ultime 3 cifre..
            If CStr(N_Appezza).Length > 3 Then
                strCodiceAppezzamento = CInt(Right(CStr(N_Appezza), 3))
            Else
                strCodiceAppezzamento = N_Appezza
            End If
        End If

        If TipoCodiceAppezzamento = enum_TipoCodiceAppezzamento_SchedaCampagna.NomeAppezzamento Then
            strCodiceAppezzamento = App_Nome
        End If


        If strCodiceAppezzamento = "" Then

            If App_Nome_Breve <> "" Then
                strCodiceAppezzamento = App_Nome_Breve
            Else
                strCodiceAppezzamento = App_Nome
                'Dim objNum As New AgronicaCoreDataProvider.UtilityProvider
                'Num_Appezzamento = objNum.Numero_from_Stringa(App_Nome)
                'If Num_Appezzamento <> 0 Then
                '    strCodiceAppezzamento = CStr(Num_Appezzamento)
                'End If
            End If

            If strCodiceAppezzamento = "" Then
                ' in molti ds non viene calcolato N_Appezza quindi lo calcolo qui
                If N_Appezza < 0 Then
                    N_Appezza = Appezza - BaseCode
                End If
                'se è un valore alto lo tronco alle ultime 3 cifre..
                If CStr(N_Appezza).Length > 3 Then
                    strCodiceAppezzamento = CInt(Right(CStr(N_Appezza), 3))
                Else
                    strCodiceAppezzamento = N_Appezza
                End If
            End If

        End If

        If Not Campo_Des Is Nothing AndAlso Campo_Des.Trim <> "" Then
            strCodiceAppezzamento = String.Format("{0}/{1}/{2}", Campo_Des, strCodiceAppezzamento, Lotto)
        Else
            strCodiceAppezzamento = String.Format("{0}/{1}", strCodiceAppezzamento, Lotto)
        End If


        If (Not IsDBNull(nrDomandaACA)) AndAlso (Not String.IsNullOrEmpty(nrDomandaACA)) Then
            strCodiceAppezzamento = $"{strCodiceAppezzamento} (ACA: {nrDomandaACA})"
        End If

        Return strCodiceAppezzamento

    End Function

End Class
