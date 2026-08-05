Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class PianoColturale_XLS
    Inherits System.Web.UI.Page

#Region " PIANO COLTURALE EXCEL "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub
    Protected WithEvents TablePianoColturale As System.Web.UI.HtmlControls.HtmlTable

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
        Response.AddHeader("Content-Disposition", "inline; filename = PianoColturale.xls")

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

            DtParticelle = objCoreStampa.PianoColturale_Excel_Particelle(Part_Query1_TempTableCreazione, _
                                                                           Part_Query2_TempTableIndice, _
                                                                            Part_Query3_TempTableFill, _
                                                                            objParametri_Server)


            '##############################################################
            '#####  Recupero Campi #################
            '##############################################################

            DtCampi = objCoreStampa.PianoColturale_Excel_Campi(Campi_Query1_TempTableCreazione, _
                                                                    Campi_Query2_TempTableIndice, _
                                                                    Campi_Query3_TempTableFill, _
                                                                    1, _
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

            DtOperazioni = objCoreStampa.PianoColturale_Excel_ImpiantiConOperazioni(Impianti_Query1_TempTableCreazione, _
                                                                                    Impianti_Query2_TempTableIndice, _
                                                                                    Impianti_Query3_TempTableFill, _
                                                                                    objParametri_Server)


            'DtSemine = DtOperazioni.Clone
            'DtRaccolte = DtOperazioni.Clone

            ''##############################################################
            ''#####  Recupero Impianti senza operazioni #################
            ''##############################################################

            Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim pivaReale As String
            'Per ogni impianto recupero le semine/trapianti e le raccolte
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

                '----------------------------------------------------
                'ricavo le particelle associate all'appezzamento

                strParticelle = ""
                strSup_Intersezione = ""

                If Not IsNothing(DtParticelle) Then

                    DrParticelle = DtParticelle.Select("Piva='" + Piva + "'" + _
                                                     " AND Sa_Cod=" + Sa_Cod.ToString + _
                                                     " AND Appezza=" + Appezza.ToString)

                    If Not DrParticelle Is Nothing AndAlso DrParticelle.Length > 0 Then

                        For p = 0 To DrParticelle.Length - 1
                            strParticelle &= DrParticelle(p).Item("prov") & " (" & DrParticelle(p).Item("COMUNI_PROV") & ") - " & _
                                            DrParticelle(p).Item("com") & " (" & DrParticelle(p).Item("LOCALITA") & ") - " & _
                                            DrParticelle(p).Item("sezione") & " - " & _
                                            DrParticelle(p).Item("foglio") & " - " & _
                                            DrParticelle(p).Item("numero") & " - " & _
                                            DrParticelle(p).Item("subalterno") & "<br>"

                            strSup_Intersezione &= DrParticelle(p).Item("area") & "<br>"
                        Next

                    End If

                End If

                ' recupero data semina/raccolta prevista da esercizio attivo
                Dim Data_Semina_Prevista As String = ""
                Dim Data_Raccolta_Prevista As String = ""

                Dim objImpreseProgetti As New AgronicaCoreAnagrafeDAL.Impresa_Progetti_R
                Dim dtDistinta As DataTable = objImpreseProgetti.LeggiDistinta_Attiva_inData(
                    Piva, Sa_Cod, Appezza, Id_Reg, Date.Now, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)

                If dtDistinta IsNot Nothing AndAlso dtDistinta.Rows.Count > 0 Then
                    If Not IsDBNull(dtDistinta(0).Item("Data_Inizio_Prevista")) AndAlso CDate(dtDistinta(0).Item("Data_Inizio_Prevista")) <> AGRODATAINIZIO Then
                        Data_Semina_Prevista = dtDistinta(0).Item("Data_Inizio_Prevista")
                    End If
                    If Not IsDBNull(dtDistinta(0).Item("Data_Fine_Prevista")) AndAlso CDate(dtDistinta(0).Item("Data_Fine_Prevista")) <> AGRODATAFINE Then
                        Data_Raccolta_Prevista = dtDistinta(0).Item("Data_Fine_Prevista")
                    End If
                End If

                'DrImpianto = DtOperazioni.Select("Piva='" + Piva + "'" + _
                '                                 " AND Sa_Cod=" + Sa_Cod.ToString + _
                '                                 " AND Appezza=" + Appezza.ToString + _
                '                                 " AND Id_Reg=" + Id_Reg.ToString)



                ''-------------------------------------------------------------------------
                ''SE ESISTONO SEMINE/TRAPIANTI O RACCOLTE......
                ''ESTRAGGO I DATI PER INSERIRE EVENTUALMENTE PIU' RIGHE 
                ''(A SECONDA DEI LOTTI SEMINATI O DEI LAVORATI RACCOLTI)
                ''-------------------------------------------------------------------------
                'If Not IsNothing(DrImpianto) AndAlso DrImpianto.Length > 0 Then

                'DtImpianto = DtOperazioni.Clone

                'For j = 0 To DrImpianto.Length - 1
                '    DtImpianto.ImportRow(DrImpianto(j))
                'Next

                Num_Semine = 0
                Num_Raccolte = 0

                '---------------------------------
                'SEMINE
                '---------------------------------
                'drSemine = DtImpianto.Select("Lav_Cod=2 OR Lav_Cod =71")

                drSemine = DtOperazioni.Select("Piva='" + Piva + "'" + _
                                             " AND Sa_Cod=" + Sa_Cod.ToString + _
                                             " AND Appezza=" + Appezza.ToString + _
                                             " AND Id_Reg=" + Id_Reg.ToString + _
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
                'drRaccolte = DtImpianto.Select("Lav_Cod=125")

                drRaccolte = DtOperazioni.Select("Piva='" + Piva + "'" + _
                                            " AND Sa_Cod=" + Sa_Cod.ToString + _
                                            " AND Appezza=" + Appezza.ToString + _
                                            " AND Id_Reg=" + Id_Reg.ToString + _
                                            " AND Lav_Cod = 125 ")

                If Not IsNothing(drRaccolte) Then
                    Num_Raccolte = drRaccolte.Length
                Else
                    Num_Raccolte = 0
                End If

                'If Not IsNothing(drRaccolte) AndAlso drRaccolte.Length > 0 Then
                '    For j = 0 To drRaccolte.Length - 1
                '        DtRaccolte.ImportRow(drRaccolte(j))
                '    Next
                '    Num_Raccolte = DtRaccolte.Rows.Count
                'End If

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

                    drNoOp = DtOperazioni.Select("Piva='" + Piva + "'" + _
                                           " AND Sa_Cod=" + Sa_Cod.ToString + _
                                           " AND Appezza=" + Appezza.ToString + _
                                           " AND Id_Reg=" + Id_Reg.ToString)

                    If Not IsNothing(drNoOp) AndAlso drNoOp.Length > 0 Then

                        Campo_Des = Recupera_CampoDes(DtCampi, _
                                                    DtImpianti.Rows(i).Item("piva"), _
                                                    DtImpianti.Rows(i).Item("sa_cod"), _
                                                    drNoOp(0).Item("campo_cod"))

                        Grva_Des = Recupera_GrvaDes(DtTipVarietali, _
                                                    drNoOp(0).Item("grva_cod_veg"))


                        '--------------------------------Campi------------------------------------------

                        If strParticelle = "" AndAlso drNoOp(0).Item("campo_cod") <> 0 Then
                            Dim strParticelleCampo As String = ""
                            Dim strSup_IntersezioneCampi As String = ""
                            Recupera_Campo_Particelle(strParticelleCampo, strSup_IntersezioneCampi, _
                                                 DtImpianti.Rows(i).Item("piva"), _
                                                 DtImpianti.Rows(i).Item("sa_cod"), _
                                                 drNoOp(0).Item("campo_cod"))
                            strParticelle = "CAMPO: " & strParticelleCampo
                            strSup_Intersezione = strSup_IntersezioneCampi
                        End If


                        '--------------------------------------------------------------------------

                        'dest_uso = Recupera_GrvaDes(DtDestUso, _
                        '                  drNoOp(0).Item(""))


                        Inserisci_Riga(DtRisultati,
                                        num_impianto,
                                        pivaReale,
                                        DtImpianti.Rows(i).Item("sa_cod"),
                                        DtImpianti.Rows(i).Item("appezza"),
                                        DtImpianti.Rows(i).Item("id_reg"),
                                        drNoOp(0).Item("rag_soc"),
                                        drNoOp(0).Item("sa_nome"),
                                        Campo_Des,
                                        drNoOp(0).Item("app_nome"),
                                        drNoOp(0).Item("veg_cod"),
                                        drNoOp(0).Item("veg_des"),
                                        drNoOp(0).Item("cul_cod"),
                                        drNoOp(0).Item("cul_des"),
                                        drNoOp(0).Item("grva_cod_veg"),
                                        Grva_Des,
                                        drNoOp(0).Item("Reg_Impianti_Validita_Inizio"),
                                        drNoOp(0).Item("Reg_Impianti_Validita_Fine"),
                                        drNoOp(0).Item("sup_imp"),
                                        drNoOp(0).Item("P_HA"),
                                        drNoOp(0).Item("resa_prevista_HA"),
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
                                        strSup_Intersezione)

                    Else
                        debug = True
                    End If

                    'Inserisci_Riga(DtRisultati, _
                    '                DtImpianti.Rows(i).Item("piva"), _
                    '               DtImpianti.Rows(i).Item("sa_cod"), _
                    '               DtImpianti.Rows(i).Item("appezza"), _
                    '               DtImpianti.Rows(i).Item("id_reg"), _
                    '               DtImpianto.Rows(0).Item("rag_soc"), _
                    '               DtImpianto.Rows(0).Item("sa_nome"), _
                    '               DtImpianto.Rows(0).Item("campo_des"), _
                    '               DtImpianto.Rows(0).Item("app_nome"), _
                    '               DtImpianto.Rows(0).Item("veg_cod"), _
                    '               DtImpianto.Rows(0).Item("veg_des"), _
                    '               DtImpianto.Rows(0).Item("cul_cod"), _
                    '               DtImpianto.Rows(0).Item("cul_des"), _
                    '               DtImpianto.Rows(0).Item("grva_cod_veg"), _
                    '               DtImpianto.Rows(0).Item("grva_des"), _
                    '               DtImpianto.Rows(0).Item("Reg_Impianti_Validita_Inizio"), _
                    '               DtImpianto.Rows(0).Item("Reg_Impianti_Validita_Fine"), _
                    '               DtImpianto.Rows(0).Item("sup_imp"), _
                    '               DtImpianto.Rows(0).Item("P_HA"), _
                    '               DtImpianto.Rows(0).Item("resa_prevista_HA"), _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               "", _
                    '               strParticelle, _
                    '               strSup_Intersezione)

                Else

                    '===========================================
                    '=== IMPIANTO CON OPERAZIONI ===========
                    '==========================================

                    Select Case Num_Semine

                        'SE CI SONO PIU SEMINE...
                        'INSERISCO 'Num_Semine' RIGHE
                        Case Is >= Num_Raccolte

                            For j = 0 To Num_Semine - 1

                                'DtSemine.Rows(j) sostituito con

                                Id_Semina = drSemine(j).Item("id_agenda")
                                Data_Semina = drSemine(j).Item("data_movimento")
                                Lotto_Seme = "Cod." & drSemine(j).Item("Cod_Articolo")
                                If CStr(drSemine(j).Item("Lotto")).ToLower <> "indefinito" And drSemine(j).Item("Lotto") <> "" Then
                                    Lotto_Seme += " - Lotto: " + drSemine(j).Item("Lotto")
                                End If

                                Udm_Seme = drSemine(j).Item("udm_sim")
                                Qta_Seme = drSemine(j).Item("Qta_Impianto")

                                Qta = CDbl(Qta_Seme)

                                If Sup_Imp <> 0 And Qta <> 0 Then
                                    Qta_Seme_HA = CStr(Qta / Sup_Imp)
                                Else
                                    Qta_Seme_HA = ""
                                End If

                                '-------------------------------
                                'Ricavo il Fornitore delle sementi/piantine
                                'Fornitore = Recupera_Fornitore(Piva, drSemine(j).Item("mat_cod"))
                                'Fornitore = drSemine(j).Item("Fornitore")

                                Fornitore = Recupera_Fornitore(DT_Fornitori, _
                                                            QueryLetturaEseguita, _
                                                            drSemine(j).Item("piva"), _
                                                             drSemine(j).Item("mat_cod"), _
                                                            Fornitori_Query1_TempTableCreazione, _
                                                            Fornitori_Query2_TempTableIndice, _
                                                            Fornitori_Query3_TempTableFill)


                                If x < Num_Raccolte Then

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

                                    ' Calibro = ""
                                    Calibro = drRaccolte(j).Item("cal_des")

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

                                '-------------------------------

                                Campo_Des = Recupera_CampoDes(DtCampi, _
                                                            drSemine(j).Item("piva"), _
                                                            drSemine(j).Item("sa_cod"), _
                                                            drSemine(j).Item("campo_cod"))

                                Grva_Des = Recupera_GrvaDes(DtTipVarietali, _
                                                            drSemine(j).Item("grva_cod_veg"))



                                '--------------------------------Campi------------------------------------------

                                If strParticelle = "" AndAlso drSemine(j).Item("campo_cod") <> 0 Then
                                    Dim strParticelleCampo As String = ""
                                    Dim strSup_IntersezioneCampi As String = ""
                                    Recupera_Campo_Particelle(strParticelleCampo, strSup_IntersezioneCampi, _
                                                         drSemine(j).Item("piva"), _
                                                         drSemine(j).Item("sa_cod"), _
                                                         drSemine(j).Item("campo_cod"))
                                    strParticelle = "CAMPO: " & strParticelleCampo
                                    strSup_Intersezione = strSup_IntersezioneCampi
                                End If


                                '--------------------------------------------------------------------------

                                Inserisci_Riga(DtRisultati,
                                                num_impianto,
                                                pivaReale,
                                               DtImpianti.Rows(i).Item("sa_cod"),
                                               DtImpianti.Rows(i).Item("appezza"),
                                               DtImpianti.Rows(i).Item("id_reg"),
                                               drSemine(j).Item("rag_soc"),
                                               drSemine(j).Item("sa_nome"),
                                               Campo_Des,
                                               drSemine(j).Item("app_nome"),
                                               drSemine(j).Item("veg_cod"),
                                               drSemine(j).Item("veg_des"),
                                               drSemine(j).Item("cul_cod"),
                                               drSemine(j).Item("cul_des"),
                                               drSemine(j).Item("grva_cod_veg"),
                                               Grva_Des,
                                               drSemine(j).Item("Reg_Impianti_Validita_Inizio"),
                                               drSemine(j).Item("Reg_Impianti_Validita_Fine"),
                                               drSemine(j).Item("sup_imp"),
                                               drSemine(j).Item("P_HA"),
                                               drSemine(j).Item("resa_prevista_HA"),
                                               Id_Semina,
                                               Data_Semina,
                                               Lotto_Seme,
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
                                               Fornitore,
                                               strParticelle,
                                               strSup_Intersezione)

                                x += 1

                            Next

                            '#######################################################################

                            'SE CI SONO PIU RACCOLTE...
                            'INSERISCO 'Num_Raccolte' RIGHE
                        Case Is < Num_Raccolte

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

                                If x < Num_Semine Then

                                    Id_Semina = drSemine(j).Item("id_agenda")
                                    Data_Semina = drSemine(j).Item("data_movimento")
                                    Lotto_Seme = "Cod." & drSemine(j).Item("Cod_Articolo")
                                    If CStr(drSemine(j).Item("Lotto")).ToLower <> "indefinito" And drSemine(j).Item("Lotto") <> "" Then
                                        Lotto_Seme += " - Lotto: " + drSemine(j).Item("Lotto")
                                    End If
                                    Udm_Seme = drSemine(j).Item("udm_sim")
                                    Qta_Seme = drSemine(j).Item("Qta_Impianto")

                                    Qta = CDbl(Qta_Seme)
                                    If Sup_Imp <> 0 And Qta <> 0 Then
                                        Qta_Seme_HA = CStr(Qta / Sup_Imp)
                                    Else
                                        Qta_Seme_HA = ""
                                    End If

                                    '-------------------------------
                                    'Ricavo il Fornitore delle sementi/piantine
                                    'Fornitore = Recupera_Fornitore(Piva, drSemine(j).Item("mat_cod"))
                                    'Fornitore = drSemine(j).Item("Fornitore")

                                    Fornitore = Recupera_Fornitore(DT_Fornitori, _
                                                                    QueryLetturaEseguita, _
                                                                    drSemine(j).Item("piva"), _
                                                                    drSemine(j).Item("mat_cod"), _
                                                                    Fornitori_Query1_TempTableCreazione, _
                                                                    Fornitori_Query2_TempTableIndice, _
                                                                    Fornitori_Query3_TempTableFill)


                                    '-------------------------------


                                    ''-------------------------------
                                    ''Ricavo il Fornitore delle sementi/piantine

                                    'Fornitore = ""

                                    'RsBolle = BolleRicevute_from_MateriaPrima(Server, Session, Page, Piva, drSemine(j).Item("mat_cod"), ArrayIdAgenda)

                                    'If Not ArrayIdAgenda Is Nothing Then

                                    '    For a = 0 To UBound(ArrayIdAgenda)

                                    '        ReDim Preserve ArrayRsBolla(a)
                                    '        ArrayRsBolla(a) = RsBolle.Clone
                                    '        ArrayRsBolla(a).Filter = "Id_Agenda =" & ArrayIdAgenda(a)

                                    '        If ArrayRsBolla(a).State <> 0 Then

                                    '            Do While Not ArrayRsBolla(a).EOF

                                    '                'dati bolla
                                    '                If ArrayRsBolla(a).Fields("cau_mov").Value = "4000" Then

                                    '                    Fornitore = ArrayRsBolla(a).Fields("Fornitore").Value

                                    '                End If

                                    '                ArrayRsBolla(a).MoveNext()

                                    '            Loop

                                    '        End If

                                    '    Next

                                    'End If

                                Else
                                    Id_Semina = ""
                                    Data_Semina = Data_Semina_Prevista
                                    Lotto_Seme = ""
                                    Udm_Seme = ""
                                    Qta_Seme = ""
                                    Qta_Seme_HA = ""
                                    Fornitore = ""
                                End If

                                Campo_Des = Recupera_CampoDes(DtCampi, _
                                                                drRaccolte(j).Item("piva"), _
                                                                drRaccolte(j).Item("sa_cod"), _
                                                                drRaccolte(j).Item("campo_cod"))

                                Grva_Des = Recupera_GrvaDes(DtTipVarietali, _
                                                            drRaccolte(j).Item("grva_cod_veg"))

                                Inserisci_Riga(DtRisultati,
                                                num_impianto,
                                                pivaReale,
                                               DtImpianti.Rows(i).Item("sa_cod"),
                                               DtImpianti.Rows(i).Item("appezza"),
                                               DtImpianti.Rows(i).Item("id_reg"),
                                               drRaccolte(j).Item("rag_soc"),
                                               drRaccolte(j).Item("sa_nome"),
                                               Campo_Des,
                                               drRaccolte(j).Item("app_nome"),
                                               drRaccolte(j).Item("veg_cod"),
                                               drRaccolte(j).Item("veg_des"),
                                               drRaccolte(j).Item("cul_cod"),
                                               drRaccolte(j).Item("cul_des"),
                                               drRaccolte(j).Item("grva_cod_veg"),
                                               Grva_Des,
                                               drRaccolte(j).Item("Reg_Impianti_Validita_Inizio"),
                                               drRaccolte(j).Item("Reg_Impianti_Validita_Fine"),
                                               drRaccolte(j).Item("sup_imp"),
                                               drRaccolte(j).Item("P_HA"),
                                               drRaccolte(j).Item("resa_prevista_HA"),
                                               Id_Semina,
                                               Data_Semina,
                                               Lotto_Seme,
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
                                               Fornitore,
                                               strParticelle,
                                               strSup_Intersezione)

                                x += 1

                            Next


                    End Select

                End If 'NUM SEMINE E NUM RACCOLTE


                '#######################################################################

                '-------------------------------------------------------------------------
                'SE NON ESISTONO SEMINE/TRAPIANTI O RACCOLTE......
                'INSERISCO UNA RIGA CONTENENTE SOLO I DATI DELL'IMPIANTO......
                '-------------------------------------------------------------------------
                ' Else

                'LEGGIMI!!!!!!!!!!!!!!!!!!
                'QUESTA PARTE NON SERVE PIU', PERCHE' GLI IMPIANTI SENZA OPERAZIONI
                'VENGONO LETTI NELLA GIGA UNION DELLA QUERY PRINCIPALE

                '    debug = True

                'DtImpianto = Recupera_Dati_Impianto(DtImpianti.Rows(i).Item("piva"), _
                '                                    DtImpianti.Rows(i).Item("sa_cod"), _
                '                                    DtImpianti.Rows(i).Item("appezza"), _
                '                                    DtImpianti.Rows(i).Item("id_reg"))

                'If Not IsNothing(DtImpianto) Then

                '    Inserisci_Riga(DtImpianti.Rows(i).Item("piva"), _
                '                   DtImpianti.Rows(i).Item("sa_cod"), _
                '                   DtImpianti.Rows(i).Item("appezza"), _
                '                   DtImpianti.Rows(i).Item("id_reg"), _
                '                   DtImpianto.Rows(0).Item("rag_soc"), _
                '                   DtImpianto.Rows(0).Item("sa_nome"), _
                '                   DtImpianto.Rows(0).Item("campo_des"), _
                '                   DtImpianto.Rows(0).Item("app_nome"), _
                '                   DtImpianto.Rows(0).Item("veg_cod"), _
                '                   DtImpianto.Rows(0).Item("veg_des"), _
                '                   DtImpianto.Rows(0).Item("cul_cod"), _
                '                   DtImpianto.Rows(0).Item("cul_des"), _
                '                   DtImpianto.Rows(0).Item("grva_cod_veg"), _
                '                   DtImpianto.Rows(0).Item("grva_des"), _
                '                   DtImpianto.Rows(0).Item("Reg_Impianti_Validita_Inizio"), _
                '                   DtImpianto.Rows(0).Item("Reg_Impianti_Validita_Fine"), _
                '                   DtImpianto.Rows(0).Item("sup_imp"), _
                '                   DtImpianto.Rows(0).Item("P_HA"), _
                '                   DtImpianto.Rows(0).Item("resa_prevista_HA"), _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   "", _
                '                   strParticelle, _
                '                   strSup_Intersezione)

                'End If


                '  End If

                'If Not IsNothing(Dt_ImpNoOp) Then

                '    For j = 0 To Dt_ImpNoOp.Rows.Count - 1

                '        Inserisci_Riga(Dt_ImpNoOp.Rows(i).Item("piva"), _
                '                       Dt_ImpNoOp.Rows(i).Item("sa_cod"), _
                '                       Dt_ImpNoOp.Rows(i).Item("appezza"), _
                '                       Dt_ImpNoOp.Rows(i).Item("id_reg"), _
                '                       Dt_ImpNoOp.Rows(0).Item("rag_soc"), _
                '                       Dt_ImpNoOp.Rows(0).Item("sa_nome"), _
                '                       Dt_ImpNoOp.Rows(0).Item("campo_des"), _
                '                       Dt_ImpNoOp.Rows(0).Item("app_nome"), _
                '                       Dt_ImpNoOp.Rows(0).Item("veg_cod"), _
                '                       Dt_ImpNoOp.Rows(0).Item("veg_des"), _
                '                       Dt_ImpNoOp.Rows(0).Item("cul_cod"), _
                '                       Dt_ImpNoOp.Rows(0).Item("cul_des"), _
                '                       Dt_ImpNoOp.Rows(0).Item("grva_cod_veg"), _
                '                       Dt_ImpNoOp.Rows(0).Item("grva_des"), _
                '                       Dt_ImpNoOp.Rows(0).Item("Reg_Impianti_Validita_Inizio"), _
                '                       Dt_ImpNoOp.Rows(0).Item("Reg_Impianti_Validita_Fine"), _
                '                       Dt_ImpNoOp.Rows(0).Item("sup_imp"), _
                '                       Dt_ImpNoOp.Rows(0).Item("P_HA"), _
                '                       Dt_ImpNoOp.Rows(0).Item("resa_prevista_HA"), _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       "", _
                '                       strParticelle, _
                '                       strSup_Intersezione)


                '    Next 'ciclo sugli impianti senza operazioni

                'End If


            Next 'per ogni impianto (ciclo i)

            ''-------------------------------------------------------------------------
            ''ORDINO IL DATATABLE......
            ''-------------------------------------------------------------------------
            'Dv = New DataView

            'DtRisultati.TableName = "Movimenti"

            'Dv.Table = DtRisultati
            ''Dv.Sort = "rag_soc, sa_nome, campo_des, app_nome"
            'Dv.Sort = "rag_soc, sa_nome, veg_des, cul_des "

            ''-------------------------------------------------------------------------
            ''Aggiungo la colonna per colorare poi le righe....
            ''-------------------------------------------------------------------------

            'Dv.Table.Columns.Add("num_impianto")

            'Piva = ""
            'Sa_Cod = 0
            'Appezza = 0
            'Id_Reg = 0

            'For i = 0 To Dv.Count - 1

            '    If Piva = CStr(Dv.Item(i).Item("piva")) And _
            '        Sa_Cod = CInt(Dv.Item(i).Item("sa_cod")) And _
            '        Appezza = CInt(Dv.Item(i).Item("appezza")) And _
            '        Id_Reg = CInt(Dv.Item(i).Item("id_reg")) Then

            '        Dv.Item(i).Item("num_impianto") = N_Impianto

            '    Else
            '        Dv.Item(i).Item("num_impianto") = N_Impianto + 1
            '        N_Impianto += 1
            '    End If

            '    Piva = CStr(Dv.Item(i).Item("piva"))
            '    Sa_Cod = CInt(Dv.Item(i).Item("sa_cod"))
            '    Appezza = CInt(Dv.Item(i).Item("appezza"))
            '    Id_Reg = CInt(Dv.Item(i).Item("id_reg"))

            'Next

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
            Me.TablePianoColturale.Rows(0).Cells(0).ColSpan = 30

            'Creo la prima riga con l'intestazione
            Riga = New HtmlTableRow

            For j = 0 To 29
                Riga.Cells.Add(New HtmlTableCell)
                ElaboraCellaHTML(Riga.Cells(j), 2, "", "", "Gainsboro", "center", "top")
            Next

            Riga.Cells(0).InnerHtml = "Partita<br>Iva"
            Riga.Cells(1).InnerHtml = "Ragione<br>Sociale"
            Riga.Cells(2).InnerHtml = "Centro<br>Aziendale"
            Riga.Cells(3).InnerHtml = "Campo/Serra"
            Riga.Cells(4).InnerHtml = "Appezzamento"
            Riga.Cells(5).InnerHtml = "Sup.<br>[Ha]"

            Riga.Cells(6).InnerHtml = "Particelle<br>Catastali"
            Riga.Cells(7).InnerHtml = "Sup. Inters.<br>[Ha]"

            Riga.Cells(8).InnerHtml = "Coltura"
            Riga.Cells(9).InnerHtml = "Varieta'"
            Riga.Cells(10).InnerHtml = "Tipologia<br>Varietale"
            Riga.Cells(11).InnerHtml = "Data Inizio<br>Impianto"
            Riga.Cells(12).InnerHtml = "Data Fine<br>Impianto"
            Riga.Cells(13).InnerHtml = "Piante<br>/Ha"
            Riga.Cells(14).InnerHtml = "Piante<br>/Impianto"
            Riga.Cells(15).InnerHtml = "Resa Prevista<br>[Kg]/Ha"
            Riga.Cells(16).InnerHtml = "Resa Prevista<br>[Kg]<br>/Impianto"
            Riga.Cells(17).InnerHtml = "Data<br>Semina/Trapianto"
            Riga.Cells(18).InnerHtml = "Lotto<br>Seminato/Trapiantato"
            Riga.Cells(19).InnerHtml = "Unita' di<br>Misura"
            Riga.Cells(20).InnerHtml = "Quantita'<br>Seminata/Trapiantata"
            Riga.Cells(21).InnerHtml = "Quantita' Seminata/Trapiantata<br>/Ha"
            Riga.Cells(22).InnerHtml = "Fornitore"
            Riga.Cells(23).InnerHtml = "Data<br>Raccolta"
            Riga.Cells(24).InnerHtml = "Lotto"
            Riga.Cells(25).InnerHtml = "Articolo/Lavorato"
            Riga.Cells(26).InnerHtml = "Unita' di<br>Misura"
            Riga.Cells(27).InnerHtml = "Quantita'<br>Raccolta"
            Riga.Cells(28).InnerHtml = "Quantita' Raccolta<br>/Ha"
            Riga.Cells(29).InnerHtml = "Calibro"

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


                        For j = 0 To 29

                            'aggiungo la cella
                            Riga.Cells.Add(New HtmlTableCell)

                            Select Case j

                                Case 0
                                    'aggiungo alla piva lo spazio x salvare gli zeri..
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("piva")), "&nbsp;" & .Item("piva"), "&nbsp;")

                                Case 1
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("rag_soc")), .Item("rag_soc"), "&nbsp;")

                                Case 2
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("sa_nome")), .Item("sa_nome"), "&nbsp;")

                                Case 3
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("campo_des")), .Item("campo_des"), "&nbsp;")

                                Case 4
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("app_nome")), .Item("app_nome"), "&nbsp;")

                                Case 5
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("sup_imp")), .Item("sup_imp"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Sup_Imp = CDbl(Riga.Cells(j).InnerHtml)
                                        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.0000")
                                    Else
                                        Sup_Imp = 0
                                    End If

                                Case 6
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("particelle")), .Item("particelle"), "&nbsp;")

                                Case 7
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("sup_intersezione")), .Item("sup_intersezione"), "&nbsp;")



                                Case 8
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("veg_des")), .Item("veg_des"), "&nbsp;")

                                Case 9
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("cul_des")), .Item("cul_des"), "&nbsp;")

                                Case 10
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("grva_des")), .Item("grva_des"), "&nbsp;")

                                Case 11
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("Validita_Inizio")), .Item("Validita_Inizio"), "...")
                                    If Riga.Cells(j).InnerHtml <> "..." Then
                                        Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                                        If Riga.Cells(j).InnerHtml = "01/01/1900" Then
                                            Riga.Cells(j).InnerHtml = "..."
                                        End If
                                    End If

                                Case 12
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("Validita_Fine")), .Item("Validita_Fine"), "...")
                                    If Riga.Cells(j).InnerHtml <> "..." Then
                                        Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                                        If Riga.Cells(j).InnerHtml = "31/12/2100" Then
                                            Riga.Cells(j).InnerHtml = "..."
                                        End If
                                    End If

                                Case 13
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("piante_ha")), .Item("piante_ha"), "&nbsp;")

                                Case 14
                                    If Not IsDBNull(.Item("piante_ha")) AndAlso CDbl(.Item("piante_ha")) <> 0 AndAlso Sup_Imp <> 0 Then
                                        Riga.Cells(j).InnerHtml = CDbl(.Item("piante_ha")) * Sup_Imp
                                    Else
                                        Riga.Cells(j).InnerHtml = "0"
                                    End If

                                Case 15
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("resa_prevista_HA")), .Item("resa_prevista_HA"), "&nbsp;")

                                Case 16
                                    If Not IsDBNull(.Item("resa_prevista_HA")) AndAlso CDbl(.Item("resa_prevista_HA")) <> 0 AndAlso Sup_Imp <> 0 Then
                                        Riga.Cells(j).InnerHtml = CDbl(.Item("resa_prevista_HA")) * Sup_Imp
                                    Else
                                        Riga.Cells(j).InnerHtml = "0"
                                    End If

                                Case 17
                                    'stampo la data nel formato short
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("data_semina")), .Item("data_semina"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                                    End If

                                Case 18
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("lotto_seme")), .Item("lotto_seme") & "&nbsp;", "&nbsp;")

                                Case 19
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("udm_seme")), .Item("udm_seme"), "&nbsp;")

                                Case 20
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("qta_seme")), .Item("qta_seme"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.000")
                                    End If

                                Case 21
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("qta_seme_ha")), .Item("qta_seme_ha"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.000")
                                    End If

                                Case 22
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("fornitore")), .Item("fornitore"), "&nbsp;")


                                Case 23
                                    'stampo la data nel formato short
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("data_raccolta")), .Item("data_raccolta"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = CDate(Riga.Cells(j).InnerHtml).ToShortDateString
                                    End If

                                Case 24
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("lotto")), .Item("lotto") & "&nbsp;", "&nbsp;")

                                Case 25
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("lavorato")), .Item("lavorato") & "&nbsp;", "&nbsp;")

                                Case 26
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("udm_raccolta")), .Item("udm_raccolta"), "&nbsp;")

                                Case 27
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("qta_raccolta")), .Item("qta_raccolta"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.000")
                                    End If

                                Case 28
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("qta_raccolta_ha")), .Item("qta_raccolta_ha"), "&nbsp;")
                                    If Riga.Cells(j).InnerHtml <> "&nbsp;" And Riga.Cells(j).InnerHtml <> "" Then
                                        Riga.Cells(j).InnerHtml = Format(CDbl(Riga.Cells(j).InnerHtml), "0.000")
                                    End If

                                Case 29
                                    Riga.Cells(j).InnerHtml = IIf(Not IsDBNull(.Item("calibro")), .Item("calibro"), "&nbsp;")

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


    Private Function Recupera_CampoDes(ByVal DT As DataTable, _
                                        ByVal Piva As String, _
                                        ByVal Sa_Cod As Integer, _
                                        ByVal Campo_Cod As Integer) As String

        Dim Des As String = ""
        Dim msg As String

        Try
            If Campo_Cod <> 0 Then

                If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                    Dim Dr() As DataRow

                    Dr = DT.Select("Piva='" + Piva + "'" + _
                                    " AND Sa_Cod=" + Sa_Cod.ToString + _
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
    Private Function Recupera_GrvaDes(ByVal DT As DataTable, _
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
    Private Function Recupera_Fornitore(ByRef DT_Fornitori As DataTable, _
                                        ByRef QueryLetturaEseguita As Boolean, _
                                        ByVal Piva As String, _
                                        ByVal Mat_Cod As Integer, _
                                        ByVal Fornitori_Query1_TempTableCreazione As String, _
                                        ByVal Fornitori_Query2_TempTableIndice As String, _
                                        ByVal Fornitori_Query3_TempTableFill As String) As String

        Dim Des As String = ""
        Dim msg As String
        Dim i As Integer

        Try

            If Mat_Cod <> 0 Then

                If QueryLetturaEseguita = False Then

                    Dim objforn As New AgronicaCoreStampeDAL.AnagraficaAziendale

                    DT_Fornitori = objforn.PianoColturale_Excel_FornitoriSementi(Fornitori_Query1_TempTableCreazione, _
                                                                                Fornitori_Query2_TempTableIndice, _
                                                                                Fornitori_Query3_TempTableFill, _
                                                                                objParametri_Server)


                    QueryLetturaEseguita = True

                End If

                If Not IsNothing(DT_Fornitori) AndAlso DT_Fornitori.Rows.Count > 0 Then

                    Dim Dr() As DataRow

                    Dr = DT_Fornitori.Select("Piva='" + Piva + "'" + _
                                            " AND mat_cod = " + Mat_Cod.ToString + " ")

                    If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                        For i = 0 To Dr.Length - 1
                            Des += Dr(i).Item("Fornitore") & "<br>"
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
    Private Function Recupera_DestinazioneUso(ByVal DT As DataTable, _
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
        DtImpianti.Columns.Add(New DataColumn("PivaReale", GetType(String)))
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

        Return DtRisultati

    End Function


    '##############################################################################################################
    Private Sub Inserisci_Riga(ByRef DtRisultati As DataTable, _
                                ByVal Num_Impianto As Integer, _
                                ByVal Piva As String, _
                               ByVal Sa_Cod As String, _
                               ByVal Appezza As String, _
                               ByVal Id_Reg As String, _
                               ByVal Rag_Soc As String, _
                               ByVal Sa_nome As String, _
                               ByVal Campo_Des As String, _
                               ByVal App_Nome As String, _
                               ByVal Veg_Cod As String, _
                               ByVal Veg_Des As String, _
                               ByVal Cul_Cod As String, _
                               ByVal Cul_Des As String, _
                               ByVal Grva_Cod_Veg As String, _
                               ByVal Grva_Des As String, _
                               ByVal Validita_Inizio As String, _
                               ByVal Validita_Fine As String, _
                               ByVal Sup_Imp As String, _
                               ByVal Piante_HA As String, _
                               ByVal Resa_Prevista_HA As String, _
                               ByVal Id_Semina As String, _
                               ByVal Data_Semina As String, _
                               ByVal Lotto_Seme As String, _
                               ByVal Udm_Seme As String, _
                               ByVal Qta_Seme As String, _
                               ByVal Qta_Seme_HA As String, _
                               ByVal Id_Raccolta As String, _
                               ByVal Data_Raccolta As String, _
                               ByVal Lotto As String, _
                               ByVal Lavorato As String, _
                               ByVal Udm_Raccolta As String, _
                               ByVal Qta_Raccolta As String, _
                               ByVal Qta_Raccolta_HA As String, _
                               ByVal Calibro As String, _
                               ByVal Fornitore As String, _
                               ByVal Particelle As String, _
                               ByVal Sup_Intersezione As String _
                            )


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


        'Associo alla tabella la nuova riga creata
        DtRisultati.Rows.Add(Dr)


    End Sub


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

                '24/04/2019: commentato - delibera di Fabrizio, non visualizzare alcuna intersezone in quanto l'intersezione è sul campo, mentre nella riga c'è il dettaglio appezzamento
                'e non sarebbe quindi superficie corretta (la sup di intersezione dell'appezzamento è inferiore)
                'dt = o.Leggi(Piva, Sa_Cod, Campo_Cod, "", "", "", 0, 0, "", AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                'If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                '    'bug! visualizza sempre e solo l'intersezione della prima particella
                '    strSup_IntersezioneCampi &= dt.Rows(0).Item("area") & "<br>"
                'End If

                Return True
            End If

            Return False

        Catch ex As Exception
            msg = ex.Message
        End Try

        Return False

    End Function


End Class
