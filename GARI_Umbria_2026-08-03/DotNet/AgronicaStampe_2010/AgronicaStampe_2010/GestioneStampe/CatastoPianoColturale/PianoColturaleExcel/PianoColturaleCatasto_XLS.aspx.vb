Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Imports System.Xml
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreUtility

Public Class PianoColturaleCatasto_XLS
    Inherits System.Web.UI.Page


#Region " PIANO COLTURALE CATASTO EXCEL "

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
    End Sub

#End Region


    Const Numero_Colonne As Integer = 30

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    '#########################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '##############################################################
        'La Pagina deve essere visualizzata come un foglio Excel
        '##############################################################

        Response.ContentType = "application/vnd.ms-excel"
        Response.AddHeader("Content-Disposition", "inline; filename = pianocolturalecatasto.xls")

        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean
        'Dim strDummy As String      'controllo accesso negato.....

        'UtenteAbilitato = Controlla_Permessi_Utente_2( _
        '                            Server, Session, Page, _
        '                            Session("ASG_Utente_Username"), _
        '                            Session("ASG_IdServizio"), _
        '                            enum_Security_Attivita.Gest_Stampe, _
        '                            enum_Security_Operazione.Lettura, _
        '                            strDummy)

        '----- Se l'utente non ha il permesso per visualizzare la pagina ... lo invio al menu.


        '----- !!!!!!!!!!! -------------

        'Attivazione forzata provvisoria

        UtenteAbilitato = True

        '----- !!!!!!!!!!! -------------

        If UtenteAbilitato = False Then
            Response.Redirect("../../Messaggi/AccessoNegato.htm")
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))



        '##############################################################
        '#####  Recupero gli Impianti scelti  #########################
        '##############################################################

        Dim strXmlVariabilistampe As String
        Dim XmlDoc As New XmlDocument
        Dim Riga As HtmlTableRow

        strXmlVariabilistampe = Session("strXmlVariabilistampe")

        If strXmlVariabilistampe <> "" Then

            'Carico la stringa xml in un nuovo documento
            XmlDoc = New System.Xml.XmlDocument

            Try

                XmlDoc.LoadXml(strXmlVariabilistampe)

                Stampa_PianoColturale(XmlDoc)

            Catch ex As Exception
                Riga = New HtmlTableRow
                Riga.Cells.Add(New HtmlTableCell)
                Riga.Cells(0).ColSpan = Numero_Colonne
                Riga.Cells(0).InnerHtml = "LoadXML: " + ex.Message
                Me.TablePianoColturale.Rows.Add(Riga)
            End Try

        End If

    End Sub

    '###################################################
    Private Sub Stampa_PianoColturale(ByVal XmlDoc As XmlDocument)

        Dim debug As Boolean
        Dim Riga As HtmlTableRow

        Dim XML_FiltroStampa As System.Xml.XmlElement
        Dim XMLs_VariabiliStampe As System.Xml.XmlNodeList
        Dim XML_VariabiliStampe As System.Xml.XmlElement

        Dim i, j, x, y As Integer

        Dim DtImpianti As DataTable
        Dim DrImpianti As DataRow

        'Dim DtImpianto As New DataTable
        'Dim DrImpianto() As DataRow

        Dim DtParticelle As DataTable
        Dim DrParticelle() As DataRow

        Dim DtCampiParticelle As DataTable
        Dim DrCampiParticelle() As DataRow

        Dim DtCampi As DataTable
        Dim DtTipVarietali As DataTable
        '  Dim DtDestUso As DataTable

        'Dim strFiltroImpianti As String
        'Dim strFiltroImpianto As String

        'Dim strFiltroParticelle As String
        'Dim strFiltroParticella As String

        Dim DtOperazioni As DataTable
        Dim DrOperazioni As DataRow

        Dim DtRisultati As DataTable
        Dim DrRisultati As DataRow

        Dim drSemine() As DataRow
        Dim drRaccolte() As DataRow
        Dim drFioriture() As DataRow
        Dim drNoOp() As DataRow

        Dim Num_Righe_Impianto As Integer
        Dim Num_Semine As Integer
        Dim Num_Raccolte As Integer

        'Dim DtSemine As DataTable
        'Dim DtRaccolte As DataTable

        Dim Id_Semina As String
        Dim Lotto_Seme As String
        Dim Data_Semina As String
        Dim Qta_Seme As String
        Dim Udm_Seme As String
        Dim Qta_Seme_HA As String
        Dim Id_Raccolta As String
        Dim Lavorato As String
        Dim Lotto As String
        Dim Data_Raccolta As String
        Dim Data_Fioritura As String
        Dim Qta_Raccolta As String
        Dim Udm_Raccolta As String
        Dim Qta_Raccolta_HA As String
        Dim Calibro As String
        Dim Sup_Imp As Double
        Dim Qta As Double

        '    Dim Dv As DataView

        Dim Piva As String = ""
        Dim Sa_Cod As Integer = 0
        Dim Appezza As Integer = 0
        Dim Id_Reg As Integer = 0
        Dim Veg_cod As Integer = 0

        '  Dim N_Impianto As Integer = 0

        Dim p As Integer

        Dim Fornitore As String

        Dim strParticelle As String
        Dim strSup_Intersezione As String

        Dim strProvincia As String
        Dim strComune As String

        'Dim Dt_ImpNoOp As DataTable
        'Dim Dr_ImpNoOp() As DataRow

        Dim num_impianto As Integer
        Dim Campo_Des, Grva_Des, DestUso As String
        Dim DT_Fornitori As DataTable
        Dim QueryLetturaEseguita As Boolean = False

        Dim msg As String
        Dim Impianti_Query1_TempTableCreazione As String = ""
        Dim Impianti_Query2_TempTableIndice As String = ""
        Dim Impianti_Query3_TempTableFill As String = ""
        'Dim Impianti_Query4_TempTableJoin As String = ""
        Dim Part_Query1_TempTableCreazione As String = ""
        Dim Part_Query2_TempTableIndice As String = ""
        Dim Part_Query3_TempTableFill As String = ""
        'Dim Part_Query4_TempTableJoin As String = ""
        Dim Campi_Query1_TempTableCreazione As String = ""
        Dim Campi_Query2_TempTableIndice As String = ""
        Dim Campi_Query3_TempTableFill As String = ""
        ' Dim Campi_Query4_TempTableJoin As String = ""
        Dim Fornitori_Query1_TempTableCreazione As String = ""
        Dim Fornitori_Query2_TempTableIndice As String = ""
        Dim Fornitori_Query3_TempTableFill As String = ""
        ' Dim Fornitori_Query4_TempTableJoin As String = ""


        Dim strLotto_Seme As String = ""
        Dim strDataSemina As String = ""
        Dim strFornitore As String = ""

        Dim strRag_Soc As String = ""
        Dim strSa_Nome As String = ""
        Dim strApp_nome As String = ""
        Dim strVeg_cod As String = ""
        Dim strVeg_des As String = ""
        Dim strCul_cod As String = ""
        Dim strCul_des As String = ""
        Dim strGrva_cod_veg As String = ""
        Dim strReg_Impianti_Validita_Inizio As String = ""
        Dim strReg_Impianti_Validita_Fine As String = ""
        Dim strSup_Imp As String = ""
        Dim strP_Ha As String = ""
        Dim strResa_prevista_HA As String = ""
        Dim Cul_cod_oi_pomodoro As Integer = 0
        Dim strCul_des_oi_pomodoro As String = ""
        Dim grvaCod_oi_pomodoro As Integer = 0
        Dim tipologiaVariataleInLettere As String = ""
        Dim tecnico_riferimento As String = ""

        Try

            '##############################################################
            '----- Definisco la struttura dei DataTable dei Risultati
            '##############################################################


            DtRisultati = Crea_Dt_Risultati()

            '---- carica dt impianti
            DtImpianti = Crea_Dt_Impianti()

            '####################################################################

            Dim TabelleTemp_Mode As Integer
            Dim Str_TabelleTemp_RegolaConfronto As String

            TabelleTemp_Mode = Recupera_TabelleTemp_Mode()

            '1: CREAZIONE TABELLA TEMPORANEA TRAMITE SELECT INSERT INTO 
            '2: TRAMITE CREATE TABLE
            Select Case TabelleTemp_Mode

                Case 1
                    'Impianti_Query1_TempTableCreazione += " DROP TABLE #tempimpianti "
                    Impianti_Query1_TempTableCreazione += " SELECT Piva, Sa_Cod, Appezza, Id_Reg "
                    Impianti_Query1_TempTableCreazione += "   INTO #tempimpianti   "
                    Impianti_Query1_TempTableCreazione += "       FROM Reg_Impianti "
                    Impianti_Query1_TempTableCreazione += "           WHERE 1 = 0   "
                    Impianti_Query1_TempTableCreazione += vbCrLf

                    'Part_Query1_TempTableCreazione += " DROP TABLE #tempappezza "
                    Part_Query1_TempTableCreazione += " SELECT Piva, Sa_Cod, Appezza "
                    Part_Query1_TempTableCreazione += "   INTO #tempappezza   "
                    Part_Query1_TempTableCreazione += "       FROM Appezzamento "
                    Part_Query1_TempTableCreazione += "           WHERE 1 = 0   "
                    Part_Query1_TempTableCreazione += vbCrLf

                    Campi_Query1_TempTableCreazione += " SELECT Piva, Sa_Cod "
                    Campi_Query1_TempTableCreazione += "   INTO #tempcentri   "
                    Campi_Query1_TempTableCreazione += "       FROM Centri_Aziendali "
                    Campi_Query1_TempTableCreazione += "           WHERE 1 = 0   "
                    Campi_Query1_TempTableCreazione += vbCrLf

                    Fornitori_Query1_TempTableCreazione += " SELECT Piva "
                    Fornitori_Query1_TempTableCreazione += "   INTO #tempimprese   "
                    Fornitori_Query1_TempTableCreazione += "       FROM Imprese "
                    Fornitori_Query1_TempTableCreazione += "           WHERE 1 = 0   "
                    Fornitori_Query1_TempTableCreazione += vbCrLf

                Case 2

                    'Recupera regola di Confronto: collate sql_LATIN1_GENERAL_cp850_ci_as NOT NULL

                    Str_TabelleTemp_RegolaConfronto = Recupera_Str_TabelleTemp_RegolaConfronto()

                    Impianti_Query1_TempTableCreazione += "   CREATE TABLE #tempimpianti (	"
                    Impianti_Query1_TempTableCreazione += " [Piva]       [nvarchar] (25) " + Str_TabelleTemp_RegolaConfronto + " NOT NULL ,"
                    Impianti_Query1_TempTableCreazione += " [Sa_Cod]     [int] 		    NOT NULL ,"
                    Impianti_Query1_TempTableCreazione += " [Appezza]    [int] 	        NOT NULL ,"
                    Impianti_Query1_TempTableCreazione += " [Id_Reg]     [int] 		    NOT NULL "
                    Impianti_Query1_TempTableCreazione += " ) ON [PRIMARY]"
                    Impianti_Query1_TempTableCreazione += vbCrLf

                    Part_Query1_TempTableCreazione += "   CREATE TABLE #tempappezza (	"
                    Part_Query1_TempTableCreazione += " [Piva]       [nvarchar] (25) " + Str_TabelleTemp_RegolaConfronto + " NOT NULL ,"
                    Part_Query1_TempTableCreazione += " [Sa_Cod]     [int] 		    NOT NULL ,"
                    Part_Query1_TempTableCreazione += " [Appezza]    [int] 	        NOT NULL "
                    Part_Query1_TempTableCreazione += " ) ON [PRIMARY]"
                    Part_Query1_TempTableCreazione += vbCrLf

                    Campi_Query1_TempTableCreazione += "   CREATE TABLE #tempcentri (	"
                    Campi_Query1_TempTableCreazione += " [Piva]       [nvarchar] (25) " + Str_TabelleTemp_RegolaConfronto + " NOT NULL ,"
                    Campi_Query1_TempTableCreazione += " [Sa_Cod]     [int] 		    NOT NULL "
                    Campi_Query1_TempTableCreazione += " ) ON [PRIMARY]"
                    Campi_Query1_TempTableCreazione += vbCrLf

                    Fornitori_Query1_TempTableCreazione += "   CREATE TABLE #tempimprese (	"
                    Fornitori_Query1_TempTableCreazione += " [Piva]       [nvarchar] (25) " + Str_TabelleTemp_RegolaConfronto + " NOT NULL "
                    Fornitori_Query1_TempTableCreazione += " ) ON [PRIMARY]"
                    Fornitori_Query1_TempTableCreazione += vbCrLf


                    'Non creo la chiave, perchè poi mi da dei problemi
                    'creo l'indice dopo
                    'Query1_TempTableCreazione += " ALTER TABLE #tempimpianti ADD "
                    'Query1_TempTableCreazione += " CONSTRAINT [PK_Temp_Impianti] PRIMARY KEY  CLUSTERED "
                    'Query1_TempTableCreazione += " ("
                    'Query1_TempTableCreazione += " [Piva], "
                    'Query1_TempTableCreazione += " [Sa_Cod], "
                    'Query1_TempTableCreazione += " [Appezza], "
                    'Query1_TempTableCreazione += " [Id_Reg] "
                    'Query1_TempTableCreazione += " )  ON [PRIMARY] "
                    'Query1_TempTableCreazione += vbCrLf

            End Select

            Impianti_Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempimpianti] ON [dbo].[#tempimpianti]([Piva], [Sa_Cod], [Appezza], [Id_Reg]) "

            Part_Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempappezza] ON [dbo].[#tempappezza]([Piva], [Sa_Cod], [Appezza]) "

            Campi_Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempcentri] ON [dbo].[#tempcentri]([Piva], [Sa_Cod]) "

            Fornitori_Query2_TempTableIndice = " CREATE UNIQUE INDEX [#AgroIndextempimprese] ON [dbo].[#tempimprese]([Piva]) "

            '####################################################################


            'XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")
            'XMLs_VariabiliStampe = XML_FiltroStampa.GetElementsByTagName("VariabiliStampe")

            XMLs_VariabiliStampe = XmlDoc.SelectNodes("//VariabiliStampe")

            Dim HT_Appezza As New Hashtable
            Dim HT_Centri As New Hashtable
            Dim HT_Imprese As New Hashtable
            Dim chiave As String

            Dim strVegCod_xFasi As String = ""

            For i = 0 To XMLs_VariabiliStampe.Count - 1

                XML_VariabiliStampe = XMLs_VariabiliStampe.Item(i)

                If (IsNothing(XML_VariabiliStampe.GetAttribute("piva"))) Then
                    msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("piva")) = "") Then
                    msg = "LA PARTITA IVA E' NULLA!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("sa_cod"))) Then
                    msg = "IL SA_COD E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("sa_cod")) = "") Then
                    msg = "IL SA_COD E' NULLO!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("appezza"))) Then
                    msg = "APPEZZA E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("appezza")) = "") Then
                    msg = "APPEZZA E' NULLO!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("id_reg"))) Then
                    msg = "ID_REG E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("id_reg")) = "") Then
                    msg = "ID_REG E' NULLO!!!" & vbCrLf
                End If
                If (IsNothing(XML_VariabiliStampe.GetAttribute("veg_cod"))) Then
                    msg = "VEG_COD E' NULLO!!!" & vbCrLf
                ElseIf (CStr(XML_VariabiliStampe.GetAttribute("veg_cod")) = "") Then
                    msg = "VEG_COD E' NULLO!!!" & vbCrLf
                End If

                If msg <> "" Then
                    Riga = New HtmlTableRow
                    Riga.Cells.Add(New HtmlTableCell)
                    Riga.Cells(0).ColSpan = Numero_Colonne
                    Riga.Cells(0).InnerHtml = "Si è verificato un errore in fase di reperimento informazioni:" & vbCrLf & msg
                    Me.TablePianoColturale.Rows.Add(Riga)
                    Exit Sub
                End If

                'ricavo gli impianti
                Piva = XML_VariabiliStampe.GetAttribute("piva")
                Sa_Cod = XML_VariabiliStampe.GetAttribute("sa_cod")
                Appezza = XML_VariabiliStampe.GetAttribute("appezza")
                Id_Reg = XML_VariabiliStampe.GetAttribute("id_reg")
                Veg_cod = XML_VariabiliStampe.GetAttribute("veg_cod")

                '====================================
                'Creo una nuova riga
                DrImpianti = DtImpianti.NewRow

                DrImpianti.Item("piva") = Piva
                DrImpianti.Item("sa_cod") = Sa_Cod
                DrImpianti.Item("appezza") = Appezza
                DrImpianti.Item("id_reg") = Id_Reg
                'DrImpianti.Item("veg_cod") = Veg_cod


                If InStr(strVegCod_xFasi, Veg_cod & ",") = 0 AndAlso IsNumeric(Veg_cod) AndAlso CInt(Veg_cod) <> 0 Then
                    strVegCod_xFasi &= Veg_cod & ","
                End If



                'aggiungo la riga
                DtImpianti.Rows.Add(DrImpianti)
                '====================================

                'strFiltroImpianto = " (Reg_Impianti.PIVA='" + XML_VariabiliStampe.GetAttribute("piva") + "' " + _
                '                    " AND Reg_Impianti.SA_COD=" + XML_VariabiliStampe.GetAttribute("sa_cod") + _
                '                    " AND Reg_Impianti.APPEZZA=" + XML_VariabiliStampe.GetAttribute("appezza") + _
                '                    " AND Reg_Impianti.ID_REG=" + XML_VariabiliStampe.GetAttribute("id_reg") + _
                '                    " ) OR"

                'strFiltroImpianti = strFiltroImpianti + strFiltroImpianto

                Impianti_Query3_TempTableFill += " INSERT INTO #tempimpianti (Piva, Sa_Cod, Appezza, Id_Reg)  " & vbCrLf
                Impianti_Query3_TempTableFill += " VALUES     ('" + Agro_SQL_SaveText(Piva) + "'," + Agro_SQL_SaveNum(Sa_Cod) + "," + Agro_SQL_SaveNum(Appezza) + "," + Agro_SQL_SaveNum(Id_Reg) + ")  " & vbCrLf

                chiave = Piva & "|" & CStr(Sa_Cod) & "|" & CStr(Appezza)
                If Not HT_Appezza.Contains(chiave) Then
                    HT_Appezza.Add(chiave, "")
                    Part_Query3_TempTableFill += " INSERT INTO #tempappezza (Piva, Sa_Cod, Appezza)  " & vbCrLf
                    Part_Query3_TempTableFill += " VALUES     ('" + Agro_SQL_SaveText(Piva) + "'," + Agro_SQL_SaveNum(Sa_Cod) + "," + Agro_SQL_SaveNum(Appezza) + ")  " & vbCrLf
                End If

                chiave = Piva & "|" & CStr(Sa_Cod)
                If Not HT_Centri.Contains(chiave) Then
                    HT_Centri.Add(chiave, "")
                    Campi_Query3_TempTableFill += " INSERT INTO #tempcentri (Piva, Sa_Cod)  " & vbCrLf
                    Campi_Query3_TempTableFill += " VALUES     ('" + Agro_SQL_SaveText(Piva) + "'," + Agro_SQL_SaveNum(Sa_Cod) + ")  " & vbCrLf
                End If

                chiave = Piva
                If Not HT_Imprese.Contains(chiave) Then
                    HT_Imprese.Add(chiave, "")
                    Fornitori_Query3_TempTableFill += " INSERT INTO #tempimprese (Piva)  " & vbCrLf
                    Fornitori_Query3_TempTableFill += " VALUES     ('" + Agro_SQL_SaveText(Piva) + "' )  " & vbCrLf
                End If

                'strFiltroParticella = " (AppezzamentiXParticelle.PIVA='" + XML_VariabiliStampe.GetAttribute("piva") + "' " + _
                '                      " AND AppezzamentiXParticelle.SA_COD=" + XML_VariabiliStampe.GetAttribute("sa_cod") + _
                '                      " AND AppezzamentiXParticelle.APPEZZA=" + XML_VariabiliStampe.GetAttribute("appezza") + _
                '                      " ) OR"

                'strFiltroParticelle = strFiltroParticelle + strFiltroParticella

            Next


            '(01/02/2021) recupero le fasi fioritura
            Dim strFF_Cod_Fioritura As String = ""

            If strVegCod_xFasi <> "" Then
                strVegCod_xFasi = Left(strVegCod_xFasi, strVegCod_xFasi.Length - 1)
            End If

            'fasi new bbch
            If strVegCod_xFasi <> "" Then
                Dim objParametriIngresso As New AgronicaCoreMetaSchemaBIZ.FasiFenologiche_input
                Dim objFasi_WS As New AgronicaCoreWebService.FasiFenologiche_WS
                Dim objParametriUscitaNew As AgronicaCoreMetaSchemaBIZ.FasiFenologiche_output
                objParametriIngresso.strFiltro = " ss.VEG_COD IN (" & strVegCod_xFasi & ")"
                objParametriUscitaNew = objFasi_WS.FasiFenologiche(objParametriIngresso)

                For f = 0 To objParametriUscitaNew.ListaFasiFenologiche.Count - 1
                    If objParametriUscitaNew.ListaFasiFenologiche(f).Fioritura Then
                        strFF_Cod_Fioritura &= objParametriUscitaNew.ListaFasiFenologiche(f).Cod_SS & ","
                    End If
                Next
            End If


            'fasi old
            Dim objFioriture As New AgronicaCoreMetaSchemaDAL.FasiFenologichexFioriture_R
            Dim DtFio As DataTable
            If strVegCod_xFasi <> "" Then
                DtFio = objFioriture.Fioriture_from_VegCod(strVegCod_xFasi,
                                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                            "", "", objParametri_Server)
                If Not DtFio Is Nothing AndAlso DtFio.Rows.Count > 0 Then
                    For i = 0 To DtFio.Rows.Count - 1
                        strFF_Cod_Fioritura &= DtFio.Rows(i).Item("ff_cod") & ","
                    Next
                End If
            End If

            If strFF_Cod_Fioritura <> "" Then
                strFF_Cod_Fioritura = Left(strFF_Cod_Fioritura, strFF_Cod_Fioritura.Length - 1)
            End If

            HT_Appezza = Nothing
            HT_Centri = Nothing
            HT_Imprese = Nothing

            ''tolgo l'ultimo OR
            'strFiltroImpianti = Left(strFiltroImpianti, strFiltroImpianti.Length - 2)

            'strFiltroImpianti = " AND (" + strFiltroImpianti + ")"

            ''tolgo l'ultimo OR
            'strFiltroParticelle = Left(strFiltroParticelle, strFiltroParticelle.Length - 2)

            'strFiltroParticelle = " AND (" + strFiltroParticelle + ")"


            Dim objCoreStampa As New AgronicaCoreStampeDAL.AnagraficaAziendale

            '##############################################################
            '#####  Recupero Particelle #################
            '##############################################################

            ' DtParticelle = CreaDtParticelle_OLD(strFiltroParticelle)

            DtParticelle = objCoreStampa.PianoColturale_Excel_Particelle(Part_Query1_TempTableCreazione,
                                                                           Part_Query2_TempTableIndice,
                                                                            Part_Query3_TempTableFill,
                                                                            objParametri_Server)

            DtCampiParticelle = objCoreStampa.PianoColturale_Excel_CampiParticelle(Part_Query1_TempTableCreazione,
                                                                       Part_Query2_TempTableIndice,
                                                                        Part_Query3_TempTableFill,
                                                                        objParametri_Server)


            '##############################################################
            '#####  Recupero Campi #################
            '##############################################################

            DtCampi = objCoreStampa.PianoColturale_Excel_Campi(Campi_Query1_TempTableCreazione,
                                                                    Campi_Query2_TempTableIndice,
                                                                    Campi_Query3_TempTableFill,
                                                                    1,
                                                                    objParametri_Server)


            '##############################################################
            '#####  Recupero Tipologie Varietali #################
            '##############################################################

            Dim objMS As New AgronicaCoreMetaSchemaDAL.GruppoVarietale_R

            DtTipVarietali = objMS.LeggiTabella(0, "", "", objParametri_Server)

            ''##############################################################
            ''#####  Recupero DestinazioneUso #################
            ''##############################################################

            'Dim objCod As New AgronicaCoreAnagrafeDAL.Codice_Anagrafe_R

            'DtDestUso = objCod.Leggi(0, "TERRENO", _
            '                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
            '                            " Codici_Anagrafe.Codice >= 3000 AND Codici_Anagrafe.Codice <4000 ", "", _
            '                            objParametri_Server)



            '##############################################################
            '#####  Recupero Semine, Trapianti e Raccolte #################
            '##############################################################

            ' DtOperazioni = CreaDtOperazioni_OLD(strFiltroImpianti)

            'dim Flag_1_ConnGias_2_ConnAltro,  Connessione_Server, Stringa_Connessione_Altro as string

            DtOperazioni = objCoreStampa.PianoColturale_Excel_ImpiantiConOperazioni(Impianti_Query1_TempTableCreazione,
                                                                                    Impianti_Query2_TempTableIndice,
                                                                                    Impianti_Query3_TempTableFill,
                                                                                    objParametri_Server,
                                                                                    True)
            'Dim ArrayPiva(DtImpianti.Rows.Count) As String
            'Dim ArraySa_Cod(DtImpianti.Rows.Count) As Integer
            'Dim ArrayAppezza(DtImpianti.Rows.Count) As Integer
            'Dim ArrayId_Reg(DtImpianti.Rows.Count) As Integer

            'For i = 0 To DtImpianti.Rows.Count - 1
            '    ArrayPiva(i) = DtImpianti.Rows(i).Item("piva")
            '    ArraySa_Cod(i) = DtImpianti.Rows(i).Item("sa_cod")
            '    ArrayAppezza(i) = DtImpianti.Rows(i).Item("appezza")
            '    ArrayId_Reg(i) = DtImpianti.Rows(i).Item("id_reg")
            'Next

            Dim objImpianti As New Reg_Impianti_Read
            'Dim Dt_Impianti = objImpianti.Leggi_Dati_Impianti(
            '    ArrayPiva, ArraySa_Cod, ArrayAppezza, ArrayId_Reg,
            '    AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

            'DtSemine = DtOperazioni.Clone
            'DtRaccolte = DtOperazioni.Clone

            ''##############################################################
            ''#####  Recupero Impianti senza operazioni #################
            ''##############################################################

            'Per ogni impianto recupero le semine/trapianti e le raccolte
            Dim objImpreseCodici = New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
            Dim cacheDati = New Dictionary(Of String, String)

            Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim pivaReale As String
            For i = 0 To DtImpianti.Rows.Count - 1

                num_impianto = i + 1

                'DtSemine.Clear()
                'DtRaccolte.Clear()
                ' DtImpianto.Clear()

                Piva = CStr(DtImpianti.Rows(i).Item("piva"))
                pivaReale = objImp.Leggi_PivaReale(Piva, objParametri_Server)
                Sa_Cod = CInt(DtImpianti.Rows(i).Item("sa_cod"))
                Appezza = CInt(DtImpianti.Rows(i).Item("appezza"))
                Id_Reg = CInt(DtImpianti.Rows(i).Item("id_reg"))

                If cacheDati.ContainsKey("tr-" + Piva) Then
                    tecnico_riferimento = cacheDati.Item("tf-" + Piva)
                Else
                    tecnico_riferimento = objImpreseCodici.TecnicoRiferimento_from_PIVA(Piva, objParametri_Server)
                    cacheDati.Item("tf-" + Piva) = tecnico_riferimento
                End If

                ' ricavo data fioritura impianto
                Data_Fioritura = objImpianti.Leggi_Data_Fioritura(
                    Piva, Sa_Cod, Appezza, Id_Reg, strFF_Cod_Fioritura,
                    "", "", objParametri_Server)

                '----------------------------------------------------
                'ricavo le particelle associate all'appezzamento

                strParticelle = ""
                strSup_Intersezione = ""

                If Not IsNothing(DtParticelle) Then
                    DrParticelle = DtParticelle.Select("Piva='" + Piva + "'" +
                                                     " AND Sa_Cod=" + Sa_Cod.ToString +
                                                     " AND Appezza=" + Appezza.ToString)

                End If

                If Not IsNothing(DtCampiParticelle) Then
                    DrCampiParticelle = DtCampiParticelle.Select("Piva='" + Piva + "'" +
                                                     " AND Sa_Cod=" + Sa_Cod.ToString +
                                                     " AND Appezza=" + Appezza.ToString)

                End If


                '13/08/2019 CARLO
                'Ho aggiornato il report piano colturale per catasto per includere anche le seguenti informazioni:
                '•	Rif. Appezzamento (Appezzamento_Codici con id_cod = 1104)
                '•	Cod. Particella (ImpresexParticelle_Codici con id_cod = 1318)
                '•	Cod. Impianto (Reg_Impianti_Codici con id_cod = 1300)
                '•	Cod. Esercizio corrente (ImpreseProgetti  campo Progetto_Nome)


                ' recupero riferimento appezzamento
                Dim Rif_Appezzamento As String
                Dim objCodAppezzamento As New AgronicaCoreAnagrafeDAL.Appezzamento_Codici_R
                Dim dtCodAppezzamento = objCodAppezzamento.Leggi(Piva, Sa_Cod, Appezza, enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento, "", enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
                If dtCodAppezzamento IsNot Nothing AndAlso dtCodAppezzamento.Rows.Count > 0 Then
                    Rif_Appezzamento = dtCodAppezzamento.Rows(0)("Val_Cod")
                End If
                objCodAppezzamento = Nothing

                ' recupero codice impianto e referente
                Dim objReferente As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                Dim PivaReferente As String
                Dim RagSocReferente As String
                Dim Cod_Impianto As String
                PivaReferente = objReferente.Leggi_Codice_from_Reg_Impianti_Codici_Distinta(Piva, Sa_Cod, Appezza, Id_Reg, enum_CodiciAnagrafe.Organismo_Referente, objParametri_Server)
                Cod_Impianto = objReferente.Leggi_Codice_from_Reg_Impianti_Codici_Distinta(Piva, Sa_Cod, Appezza, Id_Reg, enum_CodiciAnagrafe.Codice_Impianto, objParametri_Server)
                objReferente = Nothing

                ' recupero codice esercizio
                Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                Dim Cod_Esercizio As String
                Dim Data_Semina_Prevista As String = ""
                Dim Data_Raccolta_Prevista As String = ""
                Dim Data_Fioritura_Prevista As String = ""
                Dim dtDistinta As DataTable = objImpreseProgetti.LeggiDistinta_Attiva_inData(
                    Piva, Sa_Cod, Appezza, Id_Reg, Date.Now,
                    enumSelezioneVariabile.Selezione_TabellaCompleta,
                    "", "", objParametri_Server)

                If dtDistinta IsNot Nothing AndAlso dtDistinta.Rows.Count > 0 Then
                    Cod_Esercizio = dtDistinta.Rows(0)("Progetto_Nome")
                    If Not IsDBNull(dtDistinta(0).Item("Data_Inizio_Prevista")) AndAlso CDate(dtDistinta(0).Item("Data_Inizio_Prevista")) <> AGRODATAINIZIO Then
                        Data_Semina_Prevista = dtDistinta(0).Item("Data_Inizio_Prevista")
                    End If
                    If Not IsDBNull(dtDistinta(0).Item("Data_Fine_Prevista")) AndAlso CDate(dtDistinta(0).Item("Data_Fine_Prevista")) <> AGRODATAFINE Then
                        Data_Raccolta_Prevista = dtDistinta(0).Item("Data_Fine_Prevista")
                    End If
                    If Not IsDBNull(dtDistinta(0).Item("Data_Fioritura_Prevista")) AndAlso CDate(dtDistinta(0).Item("Data_Fioritura_Prevista")) <> AGRODATAINIZIO Then
                        Data_Fioritura_Prevista = dtDistinta(0).Item("Data_Fioritura_Prevista")
                        If Data_Fioritura = "" Then
                            Data_Fioritura = Data_Fioritura_Prevista
                        End If
                    End If
                End If


                If PivaReferente <> "" Then
                    '09/09/2019, patch:
                    'dal 27 Ottobre 2017 l'organismo referente è un contatto
                    'Dim objRagSoc As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    'RagSocReferente = objRagSoc.RagSoc_from_Piva(PivaReferente, objParametri_Server)
                    'objRagSoc = Nothing
                    Dim objCont As New AgronicaCoreAnagrafeDAL.Contatti_R
                    RagSocReferente = objCont.RagSoc_from_CodContatto2(PivaReferente, "", objParametri_Server)
                    objCont = Nothing
                Else
                    RagSocReferente = ""
                End If

                Dim Cuaa As String
                Dim objCuaa As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                Cuaa = objCuaa.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.CodiceCUAA, objParametri_Server)
                objCuaa = Nothing

                Num_Semine = 0
                Num_Raccolte = 0


                '---------------------------------
                'SEMINE
                '---------------------------------
                'drSemine = DtImpianto.Select("Lav_Cod=2 OR Lav_Cod =71")

                drSemine = DtOperazioni.Select("Piva='" + Piva + "'" +
                                             " AND Sa_Cod=" + Sa_Cod.ToString +
                                             " AND Appezza=" + Appezza.ToString +
                                             " AND Id_Reg=" + Id_Reg.ToString +
                                             " AND Lav_Cod IN (2,71) ")

                If Not IsNothing(drSemine) Then
                    Num_Semine = drSemine.Length
                Else
                    Num_Semine = 0
                End If

                ' If Not IsNothing(drSemine) AndAlso drSemine.Length > 0 Then
                'For j = 0 To drSemine.Length - 1
                '    DtSemine.ImportRow(drSemine(j))
                'Next
                ' Num_Semine = DtSemine.Rows.Count
                ' End If

                '---------------------------------
                'RACCOLTE
                '---------------------------------

                drRaccolte = DtOperazioni.Select("Piva='" + Piva + "'" +
                                            " AND Sa_Cod=" + Sa_Cod.ToString +
                                            " AND Appezza=" + Appezza.ToString +
                                            " AND Id_Reg=" + Id_Reg.ToString +
                                            " AND Lav_Cod = 125 ")

                If Not IsNothing(drRaccolte) Then
                    Num_Raccolte = drRaccolte.Length
                Else
                    Num_Raccolte = 0
                End If


                '---------------------------------
                x = 0

                Sup_Imp = 0

                If Num_Semine > 0 Then
                    'Sup_Imp = CDbl(DtSemine.Rows(0).Item("sup_imp"))
                    Sup_Imp = CDbl(drSemine(0).Item("sup_imp"))
                Else
                    If Num_Raccolte > 0 Then
                        'Sup_Imp = CDbl(dtRaccolte.Rows(0).Item("sup_imp"))
                        Sup_Imp = CDbl(drRaccolte(0).Item("sup_imp"))
                    End If
                End If



                '#######################################################################

                If Num_Semine = 0 And Num_Raccolte = 0 Then

                    '===========================================
                    '=== IMPIANTO SENZA OPERAZIONI ===========
                    '==========================================

                    drNoOp = DtOperazioni.Select("Piva='" + Piva + "'" +
                                           " AND Sa_Cod=" + Sa_Cod.ToString +
                                           " AND Appezza=" + Appezza.ToString +
                                           " AND Id_Reg=" + Id_Reg.ToString)

                    If Not IsNothing(drNoOp) AndAlso drNoOp.Length > 0 Then

                        Campo_Des = Recupera_CampoDes(DtCampi,
                                                    DtImpianti.Rows(i).Item("piva"),
                                                    DtImpianti.Rows(i).Item("sa_cod"),
                                                    drNoOp(0).Item("campo_cod"))

                        Grva_Des = Recupera_GrvaDes(DtTipVarietali,
                                                    drNoOp(0).Item("grva_cod_veg"))

                        strRag_Soc = drNoOp(0).Item("rag_soc")
                        strSa_Nome = drNoOp(0).Item("sa_nome")

                        strApp_nome = drNoOp(0).Item("app_nome")
                        strVeg_cod = drNoOp(0).Item("veg_cod")
                        strVeg_des = drNoOp(0).Item("veg_des")
                        strCul_cod = drNoOp(0).Item("cul_cod")
                        Cul_cod_oi_pomodoro = drNoOp(0).Item("Cul_Cod_OI_Pomodoro")
                        strCul_des = drNoOp(0).Item("cul_des")
                        strCul_des_oi_pomodoro = drNoOp(0).Item("Cul_Des_OI_Pomodoro")
                        strGrva_cod_veg = drNoOp(0).Item("grva_cod_veg")

                        Dim regolamento_cod As Integer = If(Not IsDBNull(drNoOp(0).Item("regolamento_cod")), drNoOp(0).Item("regolamento_cod"), -1)
                        Dim grva_cod_cultivar As Integer = If(Not IsDBNull(drNoOp(0).Item("grva_cod_cultivar")), drNoOp(0).Item("grva_cod_cultivar"), 0)
                        Dim grva_des_cultivar = Recupera_GrvaDes(DtTipVarietali, grva_cod_cultivar)
                        tipologiaVariataleInLettere = getTipologiaVariatale(strGrva_cod_veg, Grva_Des, grva_cod_cultivar, grva_des_cultivar, regolamento_cod)

                        strReg_Impianti_Validita_Inizio = drNoOp(0).Item("Reg_Impianti_Validita_Inizio")
                        strReg_Impianti_Validita_Fine = drNoOp(0).Item("Reg_Impianti_Validita_Fine")

                        strP_Ha = drNoOp(0).Item("P_HA")
                        strResa_prevista_HA = drNoOp(0).Item("resa_prevista_HA")



                        ''--------------------------------Campi------------------------------------------

                        'If strParticelle = "" AndAlso drNoOp(0).Item("campo_cod") <> 0 Then
                        '    Dim strParticelleCampo As String = ""
                        '    Dim strSup_IntersezioneCampi As String = ""
                        '    Recupera_Campo_Particelle(strParticelleCampo, strSup_IntersezioneCampi, _
                        '                         DtImpianti.Rows(i).Item("piva"), _
                        '                         DtImpianti.Rows(i).Item("sa_cod"), _
                        '                         drNoOp(0).Item("campo_cod"))
                        '    strParticelle = "CAMPO: " & strParticelleCampo
                        '    strSup_Intersezione = strSup_IntersezioneCampi
                        'End If

                        '--------------------------------------------------------------------------

                        'dest_uso = Recupera_GrvaDes(DtDestUso, _
                        '                  drNoOp(0).Item(""))

                        If Not DrParticelle Is Nothing AndAlso DrParticelle.Length > 0 Then

                            For p = 0 To DrParticelle.Length - 1

                                If p = 0 Then
                                    strSup_Imp = drNoOp(0).Item("sup_imp")
                                Else
                                    strSup_Imp = "0"
                                End If

                                strParticelle = DrParticelle(p).Item("prov") & " (" & DrParticelle(p).Item("COMUNI_PROV") & ") - " &
                                                DrParticelle(p).Item("com") & " (" & DrParticelle(p).Item("LOCALITA") & ") - " &
                                                DrParticelle(p).Item("sezione") & " - " &
                                                DrParticelle(p).Item("foglio") & " - " &
                                                DrParticelle(p).Item("numero") & " - " &
                                                DrParticelle(p).Item("subalterno")

                                strSup_Intersezione = DrParticelle(p).Item("area")
                                strProvincia = DrParticelle(p).Item("prov")
                                strComune = DrParticelle(p).Item("com")

                                Dim Cod_Particella = Recupera_Codice_Particella(
                                    DtImpianti.Rows(i).Item("piva"), DtImpianti.Rows(i).Item("sa_cod"),
                                    DrParticelle(p).Item("prov"), DrParticelle(p).Item("com"),
                                    DrParticelle(p).Item("sezione"), DrParticelle(p).Item("foglio"),
                                    DrParticelle(p).Item("numero"), DrParticelle(p).Item("subalterno"))

                                Inserisci_Riga(DtRisultati,
                                        num_impianto,
                                        pivaReale,
                                        DtImpianti.Rows(i).Item("sa_cod"),
                                        DtImpianti.Rows(i).Item("appezza"),
                                        DtImpianti.Rows(i).Item("id_reg"),
                                        strRag_Soc,
                                        strSa_Nome,
                                        Campo_Des,
                                        strApp_nome,
                                        strVeg_cod,
                                        strVeg_des,
                                        strCul_cod,
                                        strCul_des,
                                        strGrva_cod_veg,
                                        Grva_Des,
                                        strReg_Impianti_Validita_Inizio,
                                        strReg_Impianti_Validita_Fine,
                                        strSup_Imp,
                                        strP_Ha,
                                        strResa_prevista_HA,
                                        "",
                                        Data_Semina_Prevista,
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        Data_Raccolta_Prevista,
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        "",
                                        strParticelle,
                                        strSup_Intersezione,
                                        strProvincia,
                                        strComune,
                                        DrParticelle(p).Item("COMUNI_PROV"),
                                        DrParticelle(p).Item("LOCALITA"),
                                        DrParticelle(p).Item("sezione"),
                                        DrParticelle(p).Item("foglio"),
                                        DrParticelle(p).Item("numero"),
                                        DrParticelle(p).Item("subalterno"),
                                        PivaReferente,
                                        RagSocReferente,
                                        Cuaa,
                                        Cod_Particella,
                                        Rif_Appezzamento,
                                        Cod_Impianto,
                                        Cod_Esercizio,
                                        Data_Fioritura,
                                        Cul_cod_oi_pomodoro,
                                        strCul_des_oi_pomodoro,
                                        tecnico_riferimento,
                                        tipologiaVariataleInLettere)

                            Next

                        Else

                            'NON C'è CATASTO SUGLI APPEZZAMENTI
                            'VERIFICO SE C'è CATASTO SUI CAMPI

                            If Not DrCampiParticelle Is Nothing AndAlso DrCampiParticelle.Length > 0 Then

                                For p = 0 To DrCampiParticelle.Length - 1

                                    If p = 0 Then
                                        strSup_Imp = drNoOp(0).Item("sup_imp")
                                    Else
                                        strSup_Imp = "0"
                                    End If

                                    strParticelle = DrCampiParticelle(p).Item("prov") & " (" & DrCampiParticelle(p).Item("COMUNI_PROV") & ") - " &
                                                    DrCampiParticelle(p).Item("com") & " (" & DrCampiParticelle(p).Item("LOCALITA") & ") - " &
                                                    DrCampiParticelle(p).Item("sezione") & " - " &
                                                    DrCampiParticelle(p).Item("foglio") & " - " &
                                                    DrCampiParticelle(p).Item("numero") & " - " &
                                                    DrCampiParticelle(p).Item("subalterno")

                                    '24/04/2019: commentato - delibera di Fabrizio, non visualizzare alcuna intersezone in quanto l'intersezione è sul campo, mentre nella riga c'è il dettaglio appezzamento
                                    'e non sarebbe quindi superficie corretta (la sup di intersezione dell'appezzamento è inferiore)
                                    'strSup_Intersezione = DrCampiParticelle(p).Item("area")
                                    strSup_Intersezione = ""

                                    '(08/02/2021) ri-modificato su indicazione di Fabrizio x AINPO (nella scheda di campagna mostriamo questo dato)
                                    strSup_Intersezione = DrCampiParticelle(p).Item("area")

                                    strProvincia = DrCampiParticelle(p).Item("prov")
                                    strComune = DrCampiParticelle(p).Item("com")

                                    Dim Cod_Particella = Recupera_Codice_Particella(
                                        DtImpianti.Rows(i).Item("piva"), DtImpianti.Rows(i).Item("sa_cod"),
                                        DrCampiParticelle(p).Item("prov"), DrCampiParticelle(p).Item("com"),
                                        DrCampiParticelle(p).Item("sezione"), DrCampiParticelle(p).Item("foglio"),
                                        DrCampiParticelle(p).Item("numero"), DrCampiParticelle(p).Item("subalterno"))

                                    Inserisci_Riga(DtRisultati,
                                            num_impianto,
                                             pivaReale,
                                            DtImpianti.Rows(i).Item("sa_cod"),
                                            DtImpianti.Rows(i).Item("appezza"),
                                            DtImpianti.Rows(i).Item("id_reg"),
                                            strRag_Soc,
                                            strSa_Nome,
                                            Campo_Des,
                                            strApp_nome,
                                            strVeg_cod,
                                            strVeg_des,
                                            strCul_cod,
                                            strCul_des,
                                            strGrva_cod_veg,
                                            Grva_Des,
                                            strReg_Impianti_Validita_Inizio,
                                            strReg_Impianti_Validita_Fine,
                                            strSup_Imp,
                                            strP_Ha,
                                            strResa_prevista_HA,
                                            "",
                                            Data_Semina_Prevista,
                                            "",
                                            "",
                                            "",
                                            "",
                                            "",
                                            Data_Raccolta_Prevista,
                                            "",
                                            "",
                                            "",
                                            "",
                                            "",
                                            "",
                                            "",
                                            strParticelle,
                                            strSup_Intersezione,
                                            strProvincia,
                                            strComune,
                                            DrCampiParticelle(p).Item("COMUNI_PROV"),
                                            DrCampiParticelle(p).Item("LOCALITA"),
                                            DrCampiParticelle(p).Item("sezione"),
                                            DrCampiParticelle(p).Item("foglio"),
                                            DrCampiParticelle(p).Item("numero"),
                                            DrCampiParticelle(p).Item("subalterno"),
                                            PivaReferente,
                                            RagSocReferente,
                                            Cuaa,
                                            Cod_Particella,
                                            Rif_Appezzamento,
                                            Cod_Impianto,
                                            Cod_Esercizio,
                                            Data_Fioritura,
                                            Cul_cod_oi_pomodoro,
                                            strCul_des_oi_pomodoro,
                                            tecnico_riferimento,
                                            tipologiaVariataleInLettere)
                                Next

                            Else
                                '-----------------------------------------
                                'NON C'è CATASTO
                                '-----------------------------------------

                                strSup_Imp = drNoOp(0).Item("sup_imp")

                                Inserisci_Riga(DtRisultati,
                                           num_impianto,
                                            pivaReale,
                                           DtImpianti.Rows(i).Item("sa_cod"),
                                           DtImpianti.Rows(i).Item("appezza"),
                                           DtImpianti.Rows(i).Item("id_reg"),
                                           strRag_Soc,
                                           strSa_Nome,
                                           Campo_Des,
                                           strApp_nome,
                                           strVeg_cod,
                                           strVeg_des,
                                           strCul_cod,
                                           strCul_des,
                                           strGrva_cod_veg,
                                           Grva_Des,
                                           strReg_Impianti_Validita_Inizio,
                                           strReg_Impianti_Validita_Fine,
                                           strSup_Imp,
                                           strP_Ha,
                                           strResa_prevista_HA,
                                           "",
                                           Data_Semina_Prevista,
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           Data_Raccolta_Prevista,
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           "",
                                           PivaReferente,
                                           RagSocReferente,
                                           Cuaa,
                                           "",
                                           Rif_Appezzamento,
                                           Cod_Impianto,
                                           Cod_Esercizio,
                                           Data_Fioritura,
                                            Cul_cod_oi_pomodoro,
                                            strCul_des_oi_pomodoro,
                                           tecnico_riferimento,
                                            tipologiaVariataleInLettere)

                            End If 'catasto sui campi 

                        End If 'catasto sugli appezza

                    Else
                        debug = True
                    End If

                Else

                    '===========================================
                    '=== IMPIANTO CON OPERAZIONI ===========
                    '==========================================

                    If Num_Semine > 0 Then

                        strLotto_Seme = ""
                        strDataSemina = ""
                        strFornitore = ""


                        strRag_Soc = drSemine(0).Item("rag_soc")
                        strSa_Nome = drSemine(0).Item("sa_nome")

                        strApp_nome = drSemine(0).Item("app_nome")
                        strVeg_cod = drSemine(0).Item("veg_cod")
                        strVeg_des = drSemine(0).Item("veg_des")
                        strCul_cod = drSemine(0).Item("cul_cod")
                        Cul_cod_oi_pomodoro = drSemine(0).Item("Cul_Cod_OI_Pomodoro")
                        strCul_des = drSemine(0).Item("cul_des")
                        strCul_des_oi_pomodoro = drSemine(0).Item("Cul_Des_OI_Pomodoro")
                        strGrva_cod_veg = drSemine(0).Item("grva_cod_veg")

                        strReg_Impianti_Validita_Inizio = drSemine(0).Item("Reg_Impianti_Validita_Inizio")
                        strReg_Impianti_Validita_Fine = drSemine(0).Item("Reg_Impianti_Validita_Fine")
                        strSup_Imp = drSemine(0).Item("sup_imp")
                        strP_Ha = drSemine(0).Item("P_HA")
                        strResa_prevista_HA = drSemine(0).Item("resa_prevista_HA")


                        Dim QtaTotSeme As Integer = 0

                        For j = 0 To Num_Semine - 1

                            Id_Semina = drSemine(j).Item("id_agenda")
                            Data_Semina = drSemine(j).Item("data_movimento")
                            Lotto_Seme = "Cod." & drSemine(j).Item("Cod_Articolo")
                            If CStr(drSemine(j).Item("Lotto")).ToLower <> "indefinito" And drSemine(j).Item("Lotto") <> "" Then
                                Lotto_Seme += " - Lotto: " + drSemine(j).Item("Lotto")
                            End If

                            Udm_Seme = drSemine(j).Item("udm_sim")

                            Qta_Seme = drSemine(j).Item("Qta_Impianto")
                            QtaTotSeme += Qta_Seme

                            '-------------------------------
                            'Ricavo il Fornitore delle sementi/piantine
                            'Fornitore = Recupera_Fornitore(Piva, drSemine(j).Item("mat_cod"))
                            'Fornitore = drSemine(j).Item("Fornitore")

                            Fornitore = Recupera_Fornitore(DT_Fornitori,
                                                        QueryLetturaEseguita,
                                                        drSemine(j).Item("piva"),
                                                         drSemine(j).Item("mat_cod"),
                                                        Fornitori_Query1_TempTableCreazione,
                                                        Fornitori_Query2_TempTableIndice,
                                                        Fornitori_Query3_TempTableFill)

                            If strDataSemina.LastIndexOf(Data_Semina) < 0 Then
                                strDataSemina &= IIf(strDataSemina = "", "", " - ") & Data_Semina
                            End If

                            If strLotto_Seme.LastIndexOf(Lotto_Seme) < 0 Then
                                strLotto_Seme &= IIf(strLotto_Seme = "", "", " - ") & Lotto_Seme
                            End If

                            If strFornitore.LastIndexOf(Fornitore) < 0 Then
                                strFornitore &= IIf(strFornitore = "", "", " - ") & Fornitore
                            End If


                            Campo_Des = Recupera_CampoDes(DtCampi,
                                                        drSemine(j).Item("piva"),
                                                        drSemine(j).Item("sa_cod"),
                                                        drSemine(j).Item("campo_cod"))

                            Grva_Des = Recupera_GrvaDes(DtTipVarietali,
                                                        drSemine(j).Item("grva_cod_veg"))

                            Dim regolamento_cod As Integer = If(Not IsDBNull(drSemine(j).Item("regolamento_cod")), drSemine(j).Item("regolamento_cod"), -1)
                            Dim grva_cod_cultivar As Integer = If(Not IsDBNull(drSemine(j).Item("grva_cod_cultivar")), drSemine(j).Item("grva_cod_cultivar"), 0)
                            Dim grva_des_cultivar = Recupera_GrvaDes(DtTipVarietali, grva_cod_cultivar)
                            tipologiaVariataleInLettere = getTipologiaVariatale(strGrva_cod_veg, Grva_Des, grva_cod_cultivar, grva_des_cultivar, regolamento_cod)

                            ''--------------------------------Campi------------------------------------------

                            'If strParticelle = "" AndAlso drSemine(j).Item("campo_cod") <> 0 Then
                            '    Dim strParticelleCampo As String = ""
                            '    Dim strSup_IntersezioneCampi As String = ""
                            '    Recupera_Campo_Particelle(strParticelleCampo, strSup_IntersezioneCampi, _
                            '                         drSemine(j).Item("piva"), _
                            '                         drSemine(j).Item("sa_cod"), _
                            '                         drSemine(j).Item("campo_cod"))
                            '    strParticelle = "CAMPO: " & strParticelleCampo
                            '    strSup_Intersezione = strSup_IntersezioneCampi
                            'End If

                            x += 1

                        Next

                        ' dopo che ho finito il ciclo delle semine dell'impianto
                        Qta_Seme = Format(QtaTotSeme, "0.00")

                        'totale del seme dell'impianto
                        Qta = CDbl(QtaTotSeme)

                        If Sup_Imp <> 0 And Qta <> 0 Then
                            Qta_Seme_HA = Format(Qta / Sup_Imp, "0.00")
                        Else
                            Qta_Seme_HA = ""
                        End If

                    Else
                        Id_Semina = ""
                        Data_Semina = ""
                        Lotto_Seme = ""
                        Udm_Seme = ""
                        Qta_Seme = ""
                        Qta_Seme_HA = ""
                        Fornitore = ""

                    End If

                    ' raccolte

                    If Num_Raccolte > 0 Then

                        strApp_nome = drRaccolte(0).Item("app_nome")
                        strVeg_cod = drRaccolte(0).Item("veg_cod")
                        strVeg_des = drRaccolte(0).Item("veg_des")
                        strCul_cod = drRaccolte(0).Item("cul_cod")
                        Cul_cod_oi_pomodoro = drRaccolte(0).Item("Cul_Cod_OI_Pomodoro")
                        strCul_des = drRaccolte(0).Item("cul_des")
                        strCul_des_oi_pomodoro = drRaccolte(0).Item("Cul_Des_OI_Pomodoro")
                        strGrva_cod_veg = drRaccolte(0).Item("grva_cod_veg")

                        strApp_nome = drRaccolte(0).Item("app_nome")
                        strVeg_cod = drRaccolte(0).Item("veg_cod")

                        strReg_Impianti_Validita_Inizio = drRaccolte(0).Item("Reg_Impianti_Validita_Inizio")
                        strReg_Impianti_Validita_Fine = drRaccolte(0).Item("Reg_Impianti_Validita_Fine")
                        strSup_Imp = drRaccolte(0).Item("sup_imp")
                        strP_Ha = drRaccolte(0).Item("P_HA")
                        strResa_prevista_HA = drRaccolte(0).Item("resa_prevista_HA")


                        For j = 0 To Num_Raccolte - 1

                            Id_Raccolta = drRaccolte(j).Item("id_agenda")
                            Data_Raccolta = drRaccolte(j).Item("data_movimento")
                            Lotto = drRaccolte(j).Item("Lotto")
                            Lavorato = drRaccolte(j).Item("Mat_Des")
                            Udm_Raccolta = drRaccolte(j).Item("udm_sim")
                            Qta_Raccolta = drRaccolte(j).Item("Qta_Impianto")

                            Qta = CDbl(Qta_Raccolta)
                            If Sup_Imp <> 0 And Qta <> 0 Then
                                Qta_Raccolta_HA = CStr(Qta / Sup_Imp)
                            Else
                                Qta_Raccolta_HA = ""
                            End If

                            Calibro = drRaccolte(j).Item("cal_des")
                            'Calibro = ""

                            Campo_Des = Recupera_CampoDes(DtCampi,
                                                            drRaccolte(j).Item("piva"),
                                                            drRaccolte(j).Item("sa_cod"),
                                                            drRaccolte(j).Item("campo_cod"))

                            Grva_Des = Recupera_GrvaDes(DtTipVarietali,
                                                        drRaccolte(j).Item("grva_cod_veg"))

                            Dim regolamento_cod As Integer = If(Not IsDBNull(drRaccolte(j).Item("regolamento_cod")), drRaccolte(j).Item("regolamento_cod"), -1)
                            Dim grva_cod_cultivar As Integer = If(Not IsDBNull(drRaccolte(j).Item("grva_cod_cultivar")), drRaccolte(j).Item("grva_cod_cultivar"), 0)
                            Dim grva_des_cultivar = Recupera_GrvaDes(DtTipVarietali, grva_cod_cultivar)
                            tipologiaVariataleInLettere = getTipologiaVariatale(strGrva_cod_veg, Grva_Des, grva_cod_cultivar, grva_des_cultivar, regolamento_cod)


                            x += 1

                        Next
                    Else
                        Id_Raccolta = ""
                        Data_Raccolta = Data_Raccolta_Prevista
                        Lavorato = ""
                        Lotto = ""
                        Udm_Raccolta = ""
                        Qta_Raccolta = ""
                        Qta_Raccolta_HA = ""
                        Calibro = ""
                    End If

                    If Not DrParticelle Is Nothing AndAlso DrParticelle.Length > 0 Then

                        For p = 0 To DrParticelle.Length - 1

                            strParticelle = DrParticelle(p).Item("prov") & " (" & DrParticelle(p).Item("COMUNI_PROV") & ") - " &
                                            DrParticelle(p).Item("com") & " (" & DrParticelle(p).Item("LOCALITA") & ") - " &
                                            DrParticelle(p).Item("sezione") & " - " &
                                            DrParticelle(p).Item("foglio") & " - " &
                                            DrParticelle(p).Item("numero") & " - " &
                                            DrParticelle(p).Item("subalterno")

                            strSup_Intersezione = DrParticelle(p).Item("area")

                            strProvincia = DrParticelle(p).Item("prov")
                            strComune = DrParticelle(p).Item("com")

                            If p = 0 Then
                                'strSup_Imp ok
                            Else
                                strSup_Imp = 0
                            End If

                            ' recupero codice particella
                            Dim Cod_Particella = Recupera_Codice_Particella(
                                    DtImpianti.Rows(i).Item("piva"), DtImpianti.Rows(i).Item("sa_cod"),
                                    DrParticelle(p).Item("prov"), DrParticelle(p).Item("com"),
                                    DrParticelle(p).Item("sezione"), DrParticelle(p).Item("foglio"),
                                    DrParticelle(p).Item("numero"), DrParticelle(p).Item("subalterno"))

                            Inserisci_Riga(DtRisultati,
                                            num_impianto,
                                            pivaReale,
                                            DtImpianti.Rows(i).Item("sa_cod"),
                                            DtImpianti.Rows(i).Item("appezza"),
                                            DtImpianti.Rows(i).Item("id_reg"),
                                            strRag_Soc,
                                            strSa_Nome,
                                            Campo_Des,
                                            strApp_nome,
                                            strVeg_cod,
                                            strVeg_des,
                                            strCul_cod,
                                            strCul_des,
                                            strGrva_cod_veg,
                                            Grva_Des,
                                            strReg_Impianti_Validita_Inizio,
                                            strReg_Impianti_Validita_Fine,
                                            strSup_Imp,
                                            strP_Ha,
                                            strResa_prevista_HA,
                                            Id_Semina,
                                            strDataSemina,
                                            strLotto_Seme,
                                            Udm_Seme,
                                            Qta_Seme,
                                            Qta_Seme_HA,
                                            Id_Raccolta,
                                            Data_Raccolta,
                                            Lotto,
                                            Lavorato,
                                            Udm_Raccolta,
                                            Qta_Raccolta,
                                            Qta_Raccolta_HA,
                                            Calibro,
                                            strFornitore,
                                            strParticelle,
                                            strSup_Intersezione,
                                            strProvincia,
                                            strComune,
                                            DrParticelle(p).Item("COMUNI_PROV"),
                                            DrParticelle(p).Item("LOCALITA"),
                                            DrParticelle(p).Item("sezione"),
                                            DrParticelle(p).Item("foglio"),
                                            DrParticelle(p).Item("numero"),
                                            DrParticelle(p).Item("subalterno"),
                                            PivaReferente,
                                            RagSocReferente,
                                            Cuaa,
                                            Cod_Particella,
                                            Rif_Appezzamento,
                                            Cod_Impianto,
                                            Cod_Esercizio,
                                            Data_Fioritura,
                                            Cul_cod_oi_pomodoro,
                                            strCul_des_oi_pomodoro,
                                            tecnico_riferimento,
                                            tipologiaVariataleInLettere)

                        Next


                    Else
                        'NON C'è CATASTO SUGLI APPEZZAMENTI
                        'VERIFICO SE C'è CATASTO SUI CAMPI

                        If Not DrCampiParticelle Is Nothing AndAlso DrCampiParticelle.Length > 0 Then

                            For p = 0 To DrCampiParticelle.Length - 1

                                strParticelle = DrCampiParticelle(p).Item("prov") & " (" & DrCampiParticelle(p).Item("COMUNI_PROV") & ") - " &
                                                DrCampiParticelle(p).Item("com") & " (" & DrCampiParticelle(p).Item("LOCALITA") & ") - " &
                                                DrCampiParticelle(p).Item("sezione") & " - " &
                                                DrCampiParticelle(p).Item("foglio") & " - " &
                                                DrCampiParticelle(p).Item("numero") & " - " &
                                                DrCampiParticelle(p).Item("subalterno")

                                '24/04/2019: commentato - delibera di Fabrizio, non visualizzare alcuna intersezone in quanto l'intersezione è sul campo, mentre nella riga c'è il dettaglio appezzamento
                                'e non sarebbe quindi superficie corretta (la sup di intersezione dell'appezzamento è inferiore)
                                'strSup_Intersezione = DrCampiParticelle(p).Item("area")
                                strSup_Intersezione = ""

                                '(08/02/2021) ri-modificato su indicazione di Fabrizio x AINPO (nella scheda di campagna mostriamo questo dato)
                                strSup_Intersezione = DrCampiParticelle(p).Item("area")

                                strProvincia = DrCampiParticelle(p).Item("prov")
                                strComune = DrCampiParticelle(p).Item("com")

                                If p = 0 Then
                                    'strSup_Imp ok
                                Else
                                    strSup_Imp = 0
                                End If

                                Dim Cod_Particella = Recupera_Codice_Particella(
                                        DtImpianti.Rows(i).Item("piva"), DtImpianti.Rows(i).Item("sa_cod"),
                                        DrCampiParticelle(p).Item("prov"), DrCampiParticelle(p).Item("com"),
                                        DrCampiParticelle(p).Item("sezione"), DrCampiParticelle(p).Item("foglio"),
                                        DrCampiParticelle(p).Item("numero"), DrCampiParticelle(p).Item("subalterno"))

                                Inserisci_Riga(DtRisultati,
                                                num_impianto,
                                                pivaReale,
                                                DtImpianti.Rows(i).Item("sa_cod"),
                                                DtImpianti.Rows(i).Item("appezza"),
                                                DtImpianti.Rows(i).Item("id_reg"),
                                                strRag_Soc,
                                                strSa_Nome,
                                                Campo_Des,
                                                strApp_nome,
                                                strVeg_cod,
                                                strVeg_des,
                                                strCul_cod,
                                                strCul_des,
                                                strGrva_cod_veg,
                                                Grva_Des,
                                                strReg_Impianti_Validita_Inizio,
                                                strReg_Impianti_Validita_Fine,
                                                strSup_Imp,
                                                strP_Ha,
                                                strResa_prevista_HA,
                                                Id_Semina,
                                                strDataSemina,
                                                strLotto_Seme,
                                                Udm_Seme,
                                                Qta_Seme,
                                                Qta_Seme_HA,
                                                Id_Raccolta,
                                                Data_Raccolta,
                                                Lotto,
                                                Lavorato,
                                                Udm_Raccolta,
                                                Qta_Raccolta,
                                                Qta_Raccolta_HA,
                                                Calibro,
                                                strFornitore,
                                                strParticelle,
                                                strSup_Intersezione,
                                                strProvincia,
                                                strComune,
                                                DrCampiParticelle(p).Item("COMUNI_PROV"),
                                                DrCampiParticelle(p).Item("LOCALITA"),
                                                DrCampiParticelle(p).Item("sezione"),
                                                DrCampiParticelle(p).Item("foglio"),
                                                DrCampiParticelle(p).Item("numero"),
                                                DrCampiParticelle(p).Item("subalterno"),
                                                PivaReferente,
                                                RagSocReferente,
                                                Cuaa,
                                                Cod_Particella,
                                                Rif_Appezzamento,
                                                Cod_Impianto,
                                                Cod_Esercizio,
                                                Data_Fioritura,
                                                Cul_cod_oi_pomodoro,
                                                strCul_des_oi_pomodoro,
                                            tecnico_riferimento,
                                                tipologiaVariataleInLettere)

                            Next

                        Else

                            '-----------------------------
                            'NON C'E' CATASTO
                            '------------------------------

                            '18/06/2020 patch
                            'strSup_Imp = drNoOp(0).Item("sup_imp")
                            strSup_Imp = Sup_Imp

                            Inserisci_Riga(DtRisultati,
                               num_impianto,
                               pivaReale,
                               DtImpianti.Rows(i).Item("sa_cod"),
                               DtImpianti.Rows(i).Item("appezza"),
                               DtImpianti.Rows(i).Item("id_reg"),
                               strRag_Soc,
                               strSa_Nome,
                               Campo_Des,
                               strApp_nome,
                               strVeg_cod,
                               strVeg_des,
                               strCul_cod,
                               strCul_des,
                               strGrva_cod_veg,
                               Grva_Des,
                               strReg_Impianti_Validita_Inizio,
                               strReg_Impianti_Validita_Fine,
                               strSup_Imp,
                               strP_Ha,
                               strResa_prevista_HA,
                               Id_Semina,
                               strDataSemina,
                               strLotto_Seme,
                               Udm_Seme,
                               Qta_Seme,
                               Qta_Seme_HA,
                               Id_Raccolta,
                               Data_Raccolta,
                               Lotto,
                               Lavorato,
                               Udm_Raccolta,
                               Qta_Raccolta,
                               Qta_Raccolta_HA,
                               Calibro,
                               strFornitore,
                               "",
                               "",
                               "",
                               "",
                               "",
                               "",
                               "",
                               "",
                               "",
                               "",
                               PivaReferente,
                               RagSocReferente,
                               Cuaa,
                               "",
                               Rif_Appezzamento,
                               Cod_Impianto,
                               Cod_Esercizio,
                                Data_Fioritura,
                               Cul_cod_oi_pomodoro,
                                strCul_des_oi_pomodoro,
                               tecnico_riferimento,
                                tipologiaVariataleInLettere)

                        End If 'catasto sul campo 

                    End If 'catasto sugli appezza

                End If


            Next 'per ogni impianto (ciclo i)



        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Stampa: " + ex.Message
            Me.TablePianoColturale.Rows.Add(Riga)
        End Try



        '##############################################################
        '#####  Costruisco la tabella   ###############################
        '##############################################################

        ' Crea_Excel(Dv)
        Crea_Excel(DtRisultati)

    End Sub

    '################################################
    Private Sub Crea_Excel(ByVal DT As DataTable)

        Dim Riga As HtmlTableRow
        Dim i, j As Integer
        Dim Resto As Integer
        Dim Sup_Imp As Double

        Try


            Me.TablePianoColturale.Rows(0).Cells(0).InnerHtml = " PIANO COLTURALE "
            Me.TablePianoColturale.Rows(0).Cells(0).ColSpan = 44

            'Creo la prima riga con l'intestazione
            Riga = New HtmlTableRow

            For j = 0 To 44
                Riga.Cells.Add(New HtmlTableCell)
                ElaboraCellaHTML(Riga.Cells(j), 2, "", "", "Gainsboro", "center", "top")
            Next

            Riga.Cells(0).InnerHtml = "Organismo<br>Referente"
            Riga.Cells(1).InnerHtml = "Partita<br>Iva"
            Riga.Cells(2).InnerHtml = "CUAA"

            Riga.Cells(3).InnerHtml = "Ragione<br>Sociale"
            Riga.Cells(4).InnerHtml = "Centro<br>Aziendale"
            Riga.Cells(5).InnerHtml = "Campo/Serra"

            Riga.Cells(6).InnerHtml = "Rif. Appezzamento"
            Riga.Cells(7).InnerHtml = "Appezzamento"
            Riga.Cells(8).InnerHtml = "Sup.<br>[Ha]"

            Riga.Cells(9).InnerHtml = "Cod. Particella"
            Riga.Cells(10).InnerHtml = "Prov."
            Riga.Cells(11).InnerHtml = "Com."

            Riga.Cells(12).InnerHtml = "ISTAT<br>Codice Comune"
            Riga.Cells(13).InnerHtml = "ISTAT<br>provincia"
            Riga.Cells(14).InnerHtml = "ISTAT<br>comune"
            Riga.Cells(15).InnerHtml = "Sezione"
            Riga.Cells(16).InnerHtml = "Foglio"
            Riga.Cells(17).InnerHtml = "Particella"
            Riga.Cells(18).InnerHtml = "Subalterno"

            Riga.Cells(19).InnerHtml = "Catasto"
            Riga.Cells(20).InnerHtml = "Sup. Inters.<br>[Ha]"

            Riga.Cells(21).InnerHtml = "Codice Impianto"
            Riga.Cells(22).InnerHtml = "Codice Esercizio"

            Riga.Cells(23).InnerHtml = "Coltura"
            Riga.Cells(24).InnerHtml = "Varieta'"
            Riga.Cells(25).InnerHtml = "Tipologia<br>Varietale"
            Riga.Cells(26).InnerHtml = "Data Inizio<br>Impianto"
            Riga.Cells(27).InnerHtml = "Data Fine<br>Impianto"
            Riga.Cells(28).InnerHtml = "Piante<br>/Ha"
            Riga.Cells(29).InnerHtml = "Piante<br>/Impianto"
            Riga.Cells(30).InnerHtml = "Resa Prevista<br>[Kg]/Ha"
            Riga.Cells(31).InnerHtml = "Resa Prevista<br>[Kg]<br>/Impianto"
            Riga.Cells(32).InnerHtml = "Data<br>Semina/Trapianto"
            Riga.Cells(33).InnerHtml = "Lotto<br>Seminato/Trapiantato"
            Riga.Cells(34).InnerHtml = "Unita' di<br>Misura"
            Riga.Cells(35).InnerHtml = "Quantita'<br>Seminata/Trapiantata"
            Riga.Cells(36).InnerHtml = "Quantita' Seminata/Trapiantata<br>/Ha"
            Riga.Cells(37).InnerHtml = "Fornitore"
            Riga.Cells(38).InnerHtml = "Data Fioritura"
            Riga.Cells(39).InnerHtml = "Data Raccolta"
            Riga.Cells(40).InnerHtml = "Cod. Industria OI"
            Riga.Cells(41).InnerHtml = "Des. Industria OI"
            Riga.Cells(42).InnerHtml = "Tecnico di Riferimento"
            Riga.Cells(43).InnerHtml = "Settimana Trapianto"
            Riga.Cells(44).InnerHtml = "Tipologia Variatale Iniziale"

            TablePianoColturale.Rows.Add(Riga)

            If Not IsNothing(DT) Then

                For i = 0 To DT.Rows.Count - 1

                    With DT.Rows(i)

                        Riga = New HtmlTableRow

                        'Coloro le righe degli impianti pari...
                        Resto = CDbl(.Item("num_impianto")) Mod 2

                        If Resto = 0 Then

                            Riga.Style.Item("background-color") = "#CCFFCC"

                        End If


                        For j = 0 To 44

                            'aggiungo la cella
                            Riga.Cells.Add(New HtmlTableCell)

                            Select Case j

                                Case 0
                                    'aggiungo alla piva lo spazio x salvare gli zeri..
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("ragsoc_referente")), "&nbsp;" & .Item("ragsoc_referente"), "&nbsp;")

                                Case 1
                                    'aggiungo alla piva lo spazio x salvare gli zeri..
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("piva")), "&nbsp;" & .Item("piva"), "&nbsp;")

                                Case 2
                                    'aggiungo alla piva lo spazio x salvare gli zeri..
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("cuaa")), "&nbsp;" & .Item("cuaa"), "&nbsp;")

                                Case 3
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("rag_soc")), .Item("rag_soc"), "&nbsp;")

                                Case 4
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("sa_nome")), .Item("sa_nome"), "&nbsp;")



                                Case 5
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("campo_des")), .Item("campo_des"), "&nbsp;")
                                Case 6
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("rif_appezzamento")), .Item("rif_appezzamento"), "&nbsp;")
                                Case 7
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("app_nome")), .Item("app_nome"), "&nbsp;")

                                Case 8
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("sup_imp")), .Item("sup_imp"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Sup_Imp = CDbl(Riga.Cells(j).InnerHtml)
                                        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.0000")
                                    Else
                                        Sup_Imp = 0
                                    End If
                                Case 9
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("cod_particella")), .Item("cod_particella"), "&nbsp;")

                                Case 10
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("prov_des")), "&nbsp;" & .Item("prov_des"), "&nbsp;")
                                Case 11
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("com_des")), "&nbsp;" & .Item("com_des"), "&nbsp;")
                                Case 12
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("prov_comIstat")), "&nbsp;" & .Item("prov_comIstat"), "&nbsp;")
                                Case 13
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("prov")), "&nbsp;" & .Item("prov"), "&nbsp;")
                                Case 14
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("com")), "&nbsp;" & .Item("com"), "&nbsp;")
                                Case 15
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("sezione")), "&nbsp;" & .Item("sezione"), "&nbsp;")
                                Case 16
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("foglio")), "&nbsp;" & .Item("foglio"), "&nbsp;")
                                Case 17
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("particella")), "&nbsp;" & .Item("particella"), "&nbsp;")
                                Case 18
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("subalterno")), "&nbsp;" & .Item("subalterno"), "&nbsp;")
                                Case 19
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("particelle")), .Item("particelle"), "&nbsp;")
                                Case 20
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("sup_intersezione")), .Item("sup_intersezione"), "&nbsp;")
                                Case 21
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("cod_impianto")), .Item("cod_impianto"), "&nbsp;")
                                Case 22
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("cod_esercizio")), .Item("cod_esercizio"), "&nbsp;")
                                Case 23
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("veg_des")), .Item("veg_des"), "&nbsp;")
                                Case 24
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("cul_des")), .Item("cul_des"), "&nbsp;")
                                Case 25
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("grva_des")), .Item("grva_des"), "&nbsp;")

                                Case 26
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("Validita_Inizio")), .Item("Validita_Inizio"), "...")
                                    If Riga.Cells(j).InnerHtml <> "..." Then
                                        Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                                        If Riga.Cells(j).InnerHtml = "01/01/1900" Then
                                            Riga.Cells(j).InnerHtml = "..."
                                        End If
                                    End If

                                Case 27
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("Validita_Fine")), .Item("Validita_Fine"), "...")
                                    If Riga.Cells(j).InnerHtml <> "..." Then
                                        Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                                        If Riga.Cells(j).InnerHtml = "31/12/2100" Then
                                            Riga.Cells(j).InnerHtml = "..."
                                        End If
                                    End If

                                Case 28
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("piante_ha")), .Item("piante_ha"), "&nbsp;")

                                Case 29
                                    If Not IsDBNull(.Item("piante_ha")) AndAlso CDbl(.Item("piante_ha")) <> 0 AndAlso Sup_Imp <> 0 Then
                                        Riga.Cells(j).InnerHtml = CDbl(.Item("piante_ha")) * Sup_Imp
                                    Else
                                        Riga.Cells(j).InnerHtml = "0"
                                    End If

                                Case 30
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("resa_prevista_HA")), .Item("resa_prevista_HA"), "&nbsp;")

                                Case 31
                                    If Not IsDBNull(.Item("resa_prevista_HA")) AndAlso CDbl(.Item("resa_prevista_HA")) <> 0 AndAlso Sup_Imp <> 0 Then
                                        Riga.Cells(j).InnerHtml = CDbl(.Item("resa_prevista_HA")) * Sup_Imp
                                    Else
                                        Riga.Cells(j).InnerHtml = "0"
                                    End If

                                Case 32
                                    'stampo la data nel formato short
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("data_semina")), .Item("data_semina"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        If IsDate(Riga.Cells(j).InnerHtml) Then
                                            Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                                        End If
                                    End If

                                Case 33
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("lotto_seme")), .Item("lotto_seme") & "&nbsp;", "&nbsp;")

                                Case 34
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("udm_seme")), .Item("udm_seme"), "&nbsp;")

                                Case 35
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("qta_seme")), .Item("qta_seme"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.000")
                                    End If

                                Case 36
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("qta_seme_ha")), .Item("qta_seme_ha"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.000")
                                    End If

                                Case 37
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("fornitore")), .Item("fornitore"), "&nbsp;")

                                Case 38
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("data_fioritura")), .Item("data_fioritura"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                                    End If

                                Case 39
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("data_raccolta")), .Item("data_raccolta"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                                    End If

                                    'Case 26
                                    '    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("lotto")), .Item("lotto") & "&nbsp;", "&nbsp;")

                                    'Case 27
                                    '    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("lavorato")), .Item("lavorato") & "&nbsp;", "&nbsp;")

                                    'Case 28
                                    '    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("udm_raccolta")), .Item("udm_raccolta"), "&nbsp;")

                                    'Case 29
                                    '    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("qta_raccolta")), .Item("qta_raccolta"), "&nbsp;")
                                    '    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                    '        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.000")
                                    '    End If

                                    'Case 30
                                    '    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("qta_raccolta_ha")), .Item("qta_raccolta_ha"), "&nbsp;")
                                    '    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                    '        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.000")
                                    '    End If

                                    'Case 31
                                    '    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("calibro")), .Item("calibro"), "&nbsp;")

                                Case 40

                                    Riga.Cells(j).InnerHtml = "0"

                                    If Not IsNothing(.Item("Cul_Cod_OI_Pomodoro")) AndAlso Not IsDBNull(.Item("Cul_Cod_OI_Pomodoro")) AndAlso .Item("Cul_Cod_OI_Pomodoro") > 0 Then
                                        Riga.Cells(j).InnerHtml = .Item("Cul_Cod_OI_Pomodoro").ToString()
                                    End If
                                Case 41
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("Cul_Des_OI_Pomodoro")) AndAlso Not String.IsNullOrEmpty(.Item("Cul_Des_OI_Pomodoro")), .Item("Cul_Des_OI_Pomodoro"), "&nbsp;")
                                Case 42
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("Tecnico_Riferimento_Nome")) AndAlso Not String.IsNullOrEmpty(.Item("Tecnico_Riferimento_Nome")), .Item("Tecnico_Riferimento_Nome"), "&nbsp;")
                                Case 43
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("Settimana_Trapianto")) AndAlso Not String.IsNullOrEmpty(.Item("Settimana_Trapianto")), .Item("Settimana_Trapianto"), "&nbsp;")
                                Case 44
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("grva_des_iniziale")) AndAlso Not String.IsNullOrEmpty(.Item("grva_des_iniziale")), .Item("grva_des_iniziale"), "&nbsp;")
                                Case Else
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item(j)), .Item(j), "&nbsp;")

                            End Select

                        Next


                    End With

                    'Aggiungo la Riga alla Tabella 
                    Me.TablePianoColturale.Rows.Add(Riga)

                Next


            End If

        Catch ex As Exception
            Riga = New HtmlTableRow
            Riga.Cells.Add(New HtmlTableCell)
            Riga.Cells(0).ColSpan = Numero_Colonne
            Riga.Cells(0).InnerHtml = "Crea Excel: " + ex.Message
            Me.TablePianoColturale.Rows.Add(Riga)
        End Try

    End Sub


    Private Function Recupera_CampoDes(ByVal DT As DataTable,
                                        ByVal Piva As String,
                                        ByVal Sa_Cod As Integer,
                                        ByVal Campo_Cod As Integer) As String

        Dim Des As String = ""
        Dim msg As String

        Try
            If Campo_Cod <> 0 Then

                If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                    Dim Dr() As DataRow

                    Dr = DT.Select("Piva='" + Piva + "'" +
                                    " AND Sa_Cod=" + Sa_Cod.ToString +
                                    " AND Campo_Cod=" + Campo_Cod.ToString)

                    If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                        Des = Dr(0).Item("Campo_Des")
                    End If

                End If

            End If


        Catch ex As Exception
            msg = ex.Message
        End Try

        Return Des

    End Function


    '##############################################################
    Private Function Recupera_GrvaDes(ByVal DT As DataTable,
                                        ByVal Grva_Cod As Integer) As String

        Dim Des As String = ""
        Dim msg As String
        Dim ABS_Grva_Cod As Integer

        Try

            If Grva_Cod <> 0 Then

                If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                    Dim Dr() As DataRow

                    ABS_Grva_Cod = Math.Abs(Grva_Cod)

                    Dr = DT.Select(" Grva_Cod = " + ABS_Grva_Cod.ToString + "")

                    If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                        Des = Dr(0).Item("Grva_Des")
                        If Grva_Cod < 0 Then
                            Des += " --- Ibrido"
                        End If
                    End If

                End If

            End If


        Catch ex As Exception
            msg = ex.Message
        End Try

        Return Des

    End Function

    '##############################################################
    Private Function Recupera_Fornitore(ByRef DT_Fornitori As DataTable,
                                        ByRef QueryLetturaEseguita As Boolean,
                                        ByVal Piva As String,
                                        ByVal Mat_Cod As Integer,
                                        ByVal Fornitori_Query1_TempTableCreazione As String,
                                        ByVal Fornitori_Query2_TempTableIndice As String,
                                        ByVal Fornitori_Query3_TempTableFill As String) As String

        Dim Des As String = ""
        Dim msg As String
        Dim i As Integer

        Try

            If Mat_Cod <> 0 Then

                If QueryLetturaEseguita = False Then

                    Dim objforn As New AgronicaCoreStampeDAL.AnagraficaAziendale

                    DT_Fornitori = objforn.PianoColturale_Excel_FornitoriSementi(Fornitori_Query1_TempTableCreazione,
                                                                                Fornitori_Query2_TempTableIndice,
                                                                                Fornitori_Query3_TempTableFill,
                                                                                objParametri_Server)


                    QueryLetturaEseguita = True

                End If

                If Not IsNothing(DT_Fornitori) AndAlso DT_Fornitori.Rows.Count > 0 Then

                    Dim Dr() As DataRow

                    Dr = DT_Fornitori.Select("Piva='" + Piva + "'" +
                                            " AND mat_cod = " + Mat_Cod.ToString + " ")

                    If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                        For i = 0 To Dr.Length - 1
                            Des += Dr(i).Item("Fornitore") & " - "
                        Next
                    End If

                End If

            End If


        Catch ex As Exception
            msg = ex.Message
        End Try

        Return Des

    End Function

    '##############################################################
    Private Function Recupera_DestinazioneUso(ByVal DT As DataTable,
                                                ByVal Id_Cod As Integer) As String

        Dim Des As String = ""

        Try
            If Id_Cod <> 0 Then

                If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                    Dim Dr() As DataRow

                    Dr = DT.Select(" Campo_Cod=" + Id_Cod.ToString)

                    If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                        Des = Dr(0).Item("Campo_Des")
                    End If

                End If

            End If


        Catch ex As Exception

        End Try

        Return Des

    End Function


    '########################################################################################
    Private Function Crea_Dt_Impianti() As DataTable

        Dim DtImpianti As New DataTable

        '----- Definisco la struttura dei DataTable degli Impianti

        DtImpianti.Columns.Add(New DataColumn("piva", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("sa_cod", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("appezza", GetType(String)))
        DtImpianti.Columns.Add(New DataColumn("id_reg", GetType(String)))
        'DtImpianti.Columns.Add(New DataColumn("veg_cod", GetType(String)))

        Return DtImpianti

    End Function



    '########################################################################################
    Private Function Crea_Dt_Risultati() As DataTable

        '----- Definizione delle variabili

        Dim DtRisultati As New DataTable
        Dim Dr As DataRow

        '----- Definisco la struttura del DataTable

        DtRisultati.Columns.Add(New DataColumn("piva", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("sa_cod", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("appezza", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("id_reg", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("rag_soc", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("sa_nome", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("campo_des", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("app_nome", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("veg_cod", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("veg_des", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("cul_cod", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("cul_des", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("grva_cod_veg", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("grva_des", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("validita_inizio", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("validita_fine", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("sup_imp", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("piante_ha", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("resa_prevista_ha", GetType(String)))

        DtRisultati.Columns.Add(New DataColumn("particelle", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("sup_intersezione", GetType(String)))


        DtRisultati.Columns.Add(New DataColumn("id_semina", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("data_semina", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("lotto_seme", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("udm_seme", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("qta_seme", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("qta_seme_ha", GetType(String)))

        DtRisultati.Columns.Add(New DataColumn("id_raccolta", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("data_raccolta", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("lotto", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("lavorato", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("udm_raccolta", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("qta_raccolta", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("qta_raccolta_ha", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("calibro", GetType(String)))

        DtRisultati.Columns.Add(New DataColumn("fornitore", GetType(String)))

        DtRisultati.Columns.Add(New DataColumn("num_impianto", GetType(Integer)))

        DtRisultati.Columns.Add(New DataColumn("prov_comIstat", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("prov", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("com", GetType(String)))


        DtRisultati.Columns.Add(New DataColumn("prov_des", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("com_des", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("sezione", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("foglio", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("particella", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("subalterno", GetType(String)))

        DtRisultati.Columns.Add(New DataColumn("piva_referente", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("ragsoc_referente", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("cuaa", GetType(String)))

        DtRisultati.Columns.Add(New DataColumn("rif_appezzamento", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("cod_particella", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("cod_impianto", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("cod_esercizio", GetType(String)))

        DtRisultati.Columns.Add(New DataColumn("data_fioritura", GetType(String)))

        DtRisultati.Columns.Add(New DataColumn("Cul_Cod_OI_Pomodoro", GetType(Integer)))

        DtRisultati.Columns.Add(New DataColumn("Cul_Des_OI_Pomodoro", GetType(String)))

        DtRisultati.Columns.Add(New DataColumn("Tecnico_Riferimento_Nome", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("Settimana_Trapianto", GetType(String)))
        DtRisultati.Columns.Add(New DataColumn("grva_des_iniziale", GetType(String)))

        Return DtRisultati

    End Function


    '##############################################################################################################
    Private Sub Inserisci_Riga(ByRef DtRisultati As DataTable,
                                ByVal Num_Impianto As Integer,
                                ByVal Piva As String,
                               ByVal Sa_Cod As String,
                               ByVal Appezza As String,
                               ByVal Id_Reg As String,
                               ByVal Rag_Soc As String,
                               ByVal Sa_nome As String,
                               ByVal Campo_Des As String,
                               ByVal App_Nome As String,
                               ByVal Veg_Cod As String,
                               ByVal Veg_Des As String,
                               ByVal Cul_Cod As String,
                               ByVal Cul_Des As String,
                               ByVal Grva_Cod_Veg As String,
                               ByVal Grva_Des As String,
                               ByVal Validita_Inizio As String,
                               ByVal Validita_Fine As String,
                               ByVal Sup_Imp As String,
                               ByVal Piante_HA As String,
                               ByVal Resa_Prevista_HA As String,
                               ByVal Id_Semina As String,
                               ByVal Data_Semina As String,
                               ByVal Lotto_Seme As String,
                               ByVal Udm_Seme As String,
                               ByVal Qta_Seme As String,
                               ByVal Qta_Seme_HA As String,
                               ByVal Id_Raccolta As String,
                               ByVal Data_Raccolta As String,
                               ByVal Lotto As String,
                               ByVal Lavorato As String,
                               ByVal Udm_Raccolta As String,
                               ByVal Qta_Raccolta As String,
                               ByVal Qta_Raccolta_HA As String,
                               ByVal Calibro As String,
                               ByVal Fornitore As String,
                               ByVal Particelle As String,
                               ByVal Sup_Intersezione As String,
                               ByVal Prov As String,
                               ByVal Com As String,
                               ByVal Prov_des As String,
                               ByVal Com_des As String,
                               ByVal Sezione As String,
                               ByVal Foglio As String,
                               ByVal Particella As String,
                               ByVal Subalterno As String,
                               ByVal Piva_Referente As String,
                               ByVal Ragsoc_Referente As String,
                               ByVal Cuaa As String,
                               ByVal Cod_Particella As String,
                               ByVal Rif_Appezzamento As String,
                               ByVal Cod_Impianto As String,
                               ByVal Cod_Esercizio As String,
                               ByVal Data_Fioritura As String,
                               ByVal Cul_cod_oi_pomodoro As Integer,
                               ByVal Cul_des_oi_pomodoro As String,
                               ByVal Tecnico_Riferimento_Nome As String,
                               ByVal TipologiaVariataleInLettere As String)


        Dim Dr As DataRow

        'Creo una nuova riga
        Dr = DtRisultati.NewRow

        'Definisco i valori
        Dr.Item("piva") = Piva
        Dr.Item("sa_cod") = Sa_Cod
        Dr.Item("appezza") = Appezza
        Dr.Item("id_reg") = Id_Reg
        Dr.Item("rag_soc") = Rag_Soc
        Dr.Item("sa_nome") = Sa_nome
        Dr.Item("campo_des") = Campo_Des
        Dr.Item("app_nome") = App_Nome
        Dr.Item("veg_cod") = Veg_Cod
        Dr.Item("veg_des") = Veg_Des
        Dr.Item("cul_cod") = Cul_Cod
        Dr.Item("cul_des") = Cul_Des
        Dr.Item("grva_cod_veg") = Grva_Cod_Veg
        Dr.Item("grva_des") = Grva_Des
        Dr.Item("validita_inizio") = Validita_Inizio
        Dr.Item("validita_fine") = Validita_Fine
        Dr.Item("sup_imp") = Sup_Imp
        Dr.Item("piante_ha") = Piante_HA
        Dr.Item("resa_prevista_ha") = Resa_Prevista_HA
        Dr.Item("id_semina") = Id_Semina
        Dr.Item("data_semina") = Data_Semina
        Dr.Item("lotto_seme") = Lotto_Seme
        Dr.Item("udm_seme") = Udm_Seme
        Dr.Item("qta_seme") = Qta_Seme
        Dr.Item("qta_seme_ha") = Qta_Seme_HA
        Dr.Item("id_raccolta") = Id_Raccolta
        Dr.Item("data_raccolta") = Data_Raccolta
        Dr.Item("lotto") = Lotto
        Dr.Item("lavorato") = Lavorato
        Dr.Item("udm_raccolta") = Udm_Raccolta
        Dr.Item("qta_raccolta") = Qta_Raccolta
        Dr.Item("qta_raccolta_ha") = Qta_Raccolta_HA
        Dr.Item("calibro") = Calibro

        Dr.Item("particelle") = Particelle
        Dr.Item("sup_intersezione") = Sup_Intersezione

        Dr.Item("fornitore") = Fornitore

        Dr.Item("num_impianto") = Num_Impianto

        Dr.Item("prov_comIstat") = Prov & Com
        Dr.Item("prov") = Prov
        Dr.Item("com") = Com

        Dr.Item("prov_des") = Prov_des
        Dr.Item("com_des") = Com_des
        Dr.Item("sezione") = Sezione
        Dr.Item("foglio") = Foglio
        Dr.Item("particella") = Particella
        Dr.Item("subalterno") = Subalterno

        Dr.Item("piva_referente") = Piva_Referente
        Dr.Item("ragsoc_referente") = Ragsoc_Referente
        Dr.Item("cuaa") = Cuaa

        Dr.Item("rif_appezzamento") = Rif_Appezzamento
        Dr.Item("cod_particella") = Cod_Particella
        Dr.Item("cod_impianto") = Cod_Impianto
        Dr.Item("cod_esercizio") = Cod_Esercizio

        Dr.Item("data_fioritura") = Data_Fioritura

        Dr.Item("Cul_Cod_OI_Pomodoro") = Cul_cod_oi_pomodoro

        Dr.Item("Cul_Des_OI_Pomodoro") = Cul_des_oi_pomodoro

        Dr.Item("Tecnico_Riferimento_Nome") = Tecnico_Riferimento_Nome


        Dim valueSettimanaTrapianto = ""
        Dim data As DateTime
        If Not String.IsNullOrEmpty(Data_Semina) AndAlso DateTime.TryParse(Data_Semina, data) Then
            valueSettimanaTrapianto = DataOra.GetWeekNumberFromDate(data, System.Globalization.CalendarWeekRule.FirstFullWeek)
        End If
        Dr.Item("Settimana_Trapianto") = valueSettimanaTrapianto

        Dr.Item("grva_des_iniziale") = TipologiaVariataleInLettere

        'Associo alla tabella la nuova riga creata
        DtRisultati.Rows.Add(Dr)


    End Sub


    Private Function getTipologiaVariatale(grva_cod_impianto As Integer, grva_des_impianto As String, grva_cod_cultivar As Integer, grva_des_cultivar As String, regolamento_cod As Integer) As String
        'Uhalid, 27/05/2024
        'Tipologia variatale in lettere, il gruppo variatale viene joinato seguendo due perconsi diversi:
        ' - reg_impianti.CUL_COD = Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Cul_Cod_Gias  -> Codifica_Varieta_OIPomodorodaIndustriaNordItalia.Grva_Cod_Gias = GruppoVarietale.Grva_Cod_Gias
        ' - reg_impianti.grva_cod_veg = GruppoVarietale.Grva_Cod_Gias
        ' Ogni tanto sono diversi e abbastanza spesso partendo dal cul_cod non riesce a trovare un gruppo variatale, li leggo in cascata e poi matcho per trovare tipologia variatale in lettere, ha priorita' quello partendo dal cul_cod
        'Tipologia L = lungo; T=tondo; P= Pizzutello;  S = Lungo bio; O= tondo bio; C = Datterino;  D= Ciliegino metro; R= Ciliegino baglior; V= Volare; U = Saladette; I = Tondo arancione; G =Tondo giallo; F = Tondo verde
        ' ^ da excel mutti \\rubino2\DOCUMENTAZIONE\GIAS --- Clienti --- AINPO\materiale inviato da AINPO\7 ALL .7 Format Tracciabilità Mutti 2023.xls
        ' Alcune tipologia variatale non le abbiamo sul db, tondo arancione etc, quindi non verrano usate.
        Dim grva_des_to_match = grva_des_cultivar
        If grva_cod_cultivar = 0 Then
            grva_des_to_match = grva_des_impianto
        End If


        Dim tipologiaVariatale = "-"
        Dim isBio = (regolamento_cod = 4)

        grva_des_to_match = grva_des_to_match.ToLowerInvariant
        If grva_des_to_match.Contains("lungo") Then
            If isBio Then
                tipologiaVariatale = "S" ' Lungo Bio
            Else
                tipologiaVariatale = "L" ' Lungo
            End If
        ElseIf grva_des_to_match.Contains("tondo") Then ' Dal db vedo che non abbiamo la tipologia verde, giallo e arancione, quindi sara' sempre O oppure T
            If isBio Then
                tipologiaVariatale = "O" ' Tondo Bio
            Else
                If grva_des_to_match.Contains("arancione") Then
                    tipologiaVariatale = "I" ' Tondo arancione
                ElseIf grva_des_to_match.Contains("giallo") Then
                    tipologiaVariatale = "G" ' Tondo Giallo
                ElseIf grva_des_to_match.Contains("verde") Then
                    tipologiaVariatale = "F" ' Tondo verde
                Else
                    tipologiaVariatale = "T" ' Tondo 
                End If
            End If
        ElseIf grva_des_to_match.Contains("datterino") Then
            tipologiaVariatale = "C" ' Datterino
        ElseIf grva_des_to_match.Contains("ciliegino") Then
            If grva_des_to_match.Contains("metro") Then
                tipologiaVariatale = "D" ' Ciliegino Metro
            ElseIf grva_des_to_match.Contains("baglior") Then
                tipologiaVariatale = "R" ' Ciliegino Baglior
            Else
                tipologiaVariatale = "D" ' Default, in pratica andra sempre su D, sul db non abbiamo nessun GruppoVariatale metro o baglior
            End If
        ElseIf grva_des_to_match.Contains("volare") Then
            tipologiaVariatale = "V" ' Volare
        ElseIf grva_des_to_match.Contains("saledette") Then
            tipologiaVariatale = "U" ' Saledette
        End If

        Return tipologiaVariatale




    End Function

    Private Function Recupera_Campo_Particelle(ByRef strParticelleCampo As String, ByRef strSup_IntersezioneCampi As String, _
                                    ByVal Piva As String, _
                                    ByVal Sa_Cod As Integer, _
                                    ByVal Campo_Cod As Integer) As Boolean

        Dim Des As String = ""
        Dim msg As String
        strParticelleCampo = ""
        strSup_IntersezioneCampi = ""
        Try

            If Campo_Cod <> 0 Then
                Dim o As New AgronicaCoreAnagrafeDAL.CampixParticelle_R
                Dim dt As DataTable = o.Recupera_Particelle_CAMPO_Squadro(Piva, Sa_Cod, Campo_Cod, AGRODATAINIZIO, AGRODATAFINE, objParametri_Server)


                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    Dim p As Integer = 0
                    For p = 0 To dt.Rows.Count - 1
                        strParticelleCampo &= dt.Rows(p).Item("prov") & " (" & dt.Rows(p).Item("COMUNI_PROV") & ") - " & _
                                        dt.Rows(p).Item("com") & " (" & dt.Rows(p).Item("LOCALITA") & ") - " & _
                                        dt.Rows(p).Item("sezione") & " - " & _
                                        dt.Rows(p).Item("foglio") & " - " & _
                                        dt.Rows(p).Item("numero") & " - " & _
                                        dt.Rows(p).Item("subalterno") & "<br>"

                    Next

                End If
                strSup_IntersezioneCampi = ""
                dt = o.Leggi(Piva, Sa_Cod, Campo_Cod, "", "", "", 0, 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                    strSup_IntersezioneCampi &= dt.Rows(0).Item("area") & "<br>"
                End If
                Return True
            End If
            Return False

        Catch ex As Exception
            msg = ex.Message
        End Try

        Return False

    End Function

    Private Function Recupera_Codice_Particella(
                                    ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Provincia As String,
                                    ByVal Comune As String,
                                    ByVal Sezione As String,
                                    ByVal Foglio As Integer,
                                    ByVal Numero As Integer,
                                    ByVal Subalterno As String) As String

        Dim Cod_Particella As String = ""

        Dim objCodParticella = New AgronicaCoreAnagrafeDAL.ImpresexParticelle_Codici_R

        Dim dtCodParticella = objCodParticella.Leggi(0,
            Piva, Sa_Cod, Provincia, Comune, Sezione, Foglio, Numero, Subalterno,
            enum_CodiciAnagrafe.CodiceParticella, "",
            enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

        If Not dtCodParticella Is Nothing AndAlso dtCodParticella.Rows.Count > 0 Then
            Cod_Particella = dtCodParticella.Rows(0).Item("Val_Cod")
        End If

        Return Cod_Particella

    End Function

End Class
