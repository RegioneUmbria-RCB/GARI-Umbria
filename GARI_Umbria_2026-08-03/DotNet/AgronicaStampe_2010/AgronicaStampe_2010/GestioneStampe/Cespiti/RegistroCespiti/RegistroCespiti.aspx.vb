Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class RegistroCespiti
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_RegistroCespiti As Rpt_RegistroCespiti


#Region " REGISTRO CESPITI "

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

        '----------------------------
        '   INIZIALIZZAZIONE REPORT
        '----------------------------
        Rpt_RegistroCespiti = New Rpt_RegistroCespiti
    End Sub

#End Region


    'Dim Data_Da, Data_A As String
    Dim Piva As String
    Dim Sa_Cod As Integer
    Dim RagSoc_Impresa As String
    Dim int_Configurazione_Moduli As enum_Omni_Modulo_Generazione
    Dim Anno As Integer
    Dim TipoStampa As String



    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################


        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)


        Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))


        RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                                           AgroKey_EncoderDecoder, _
                                           Server)


        If RagSoc_Impresa = "" Then
            RagSoc_Impresa = objImpreseR.RagSoc_from_Piva(Piva, objParametri_Server)
        End If


        int_Configurazione_Moduli = CInt(Stringa_Decodifica(CStr(Request.QueryString("cm")), _
                      AgroKey_EncoderDecoder, _
                      Server))


        Anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("an")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))


        TipoStampa = Stringa_Decodifica(CStr(Request.QueryString("ts")), _
                                 AgroKey_EncoderDecoder, _
                                 Server)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################
        Dim Nome_Documento As String = "RegistroCespiti"
        Dim Log_Errori As String

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS As New DS_RegistroCespiti

            Try

                Log_Errori = ""

                'FUNZIONE CHE FA TUTTO
                Stampa_RegistroCespiti(DS, Log_Errori)

            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Piva = " + CStr(Piva) + ", " + vbCrLf + _
                                "Sa_Cod = " + CStr(Sa_Cod) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "RegistroCespiti", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "RegistroCespiti.aspx", _
                                                 Log_Errori)

            End If


            Try
                Session("Report") = Rpt_RegistroCespiti
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If

        '==================================================================

    End Sub



    '#####################################################################################################
    Private Sub Stampa_RegistroCespiti(ByRef DS As DS_RegistroCespiti, _
                                    ByRef Log_Errori As String)


        Dim DT As DataTable
        Dim DT2 As DataTable

        Dim i As Integer
        Dim j As Integer
        Dim NumRigheCespite As Integer
        'PARAMETRI PASSATI AL REPORT
        Dim Parametro_RagSoc_Impresa As String = ""
        'Variabili dati per il report


        'Variabili per dati cespite
        Dim IdCodCespite As Long = 0
        Dim DesCespite As String = ""
        Dim IdCodCategoria As Long = 0
        Dim DesCategoria As String = ""
        Dim IndTipoBene As String = ""
        Dim IndNaturaBene As Integer



        'Dati Movimenti
        Dim ST_RIGA As String = ""

        Dim DATAMOV As Date
        Dim ANNOMOV As Integer
        Dim CAUMOV As Integer
        Dim DES_CAUMOV As String
        Dim l_str_DesCausaleMovimento As String
        Dim QTAMOV As Integer

        Dim COSTO As Double
        Dim FIS_VALOREAMMORTIZZABILE As Double
        Dim ANNOACQ As Integer
        Dim QTAACQ As Integer

        Dim VALALIENAZ As Double
        Dim QTAALIENAZ As Integer
        Dim DATALIENAZ As Date


        Dim PRC As Double
        Dim VALMOV As Double
        Dim FONDO As Double
        Dim RESIDUO As Double
        Dim MINUS As Double
        Dim PLUS As Double

        Dim FIS_PRC As Double
        Dim FIS_VALMOV As Double
        Dim FIS_FONDO As Double
        Dim FIS_RESIDUO As Double
        Dim FIS_MINUS As Double
        Dim FIS_PLUS As Double


        Parametro_RagSoc_Impresa = RagSoc_Impresa


        Try


            Dim objStCespiti As New AgronicaCoreStampeDAL.Cespiti
            Dim xFiltroAggiuntivo As String = ""

            xFiltroAggiuntivo = "1=1"


            'FUNZIONE CHE ESEGUE LA QUERY
            DT = objStCespiti.LeggiMovimentiCespiti(Piva, _
                                    Sa_Cod, _
                                    0, 0, Anno, _
                                    xFiltroAggiuntivo, "", _
                                    int_Configurazione_Moduli, _
                                    objParametri_Server)

        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

                Dim DR_C As DS_RegistroCespiti.DT_RegistroCespitiRow
                Dim DR_F As DS_RegistroCespiti.DT_RegistroCespitiRow

                For i = 0 To DT.Rows.Count - 1

                    'Causali Movimento
                    'RilevazioneIniziale = 10
                    'Acquisto = 20
                    'Ammortamento = 30
                    'AmmortamentoFis = 32
                    'Alienazione = 40 'Vendita/Rottamazione

                    If DT.Rows(i).Item("id_cod_cespite") <> IdCodCespite Then

                        Dim objStCespiti2 As New AgronicaCoreStampeDAL.Cespiti
                        DT2 = objStCespiti2.LeggiNumRigheMovCespite(Anno, _
                                   DT.Rows(i).Item("id_cod_cespite"), _
                                    "", "", objParametri_Server)

                        If DT2.Rows.Count <> 0 Then
                            NumRigheCespite = DT2.Rows(0).Item("Conteggio")
                        Else
                            NumRigheCespite = 0
                        End If
                        j = 0


                        'Inizializzazioni
                        ST_RIGA = ""
                        'DATAMOV As Date
                        'ANNOMOV As Integer
                        'CAUMOV As Integer
                        'DES_CAUMOV As String
                        'l_str_DesCausaleMovimento As String
                        QTAMOV = 0

                        COSTO = 0
                        FIS_VALOREAMMORTIZZABILE = 0
                        ANNOACQ = 0
                        QTAACQ = 0

                        VALALIENAZ = 0
                        QTAALIENAZ = 0
                        'DATALIENAZ = 0


                        PRC = 0
                        VALMOV = 0
                        FONDO = 0
                        RESIDUO = 0
                        MINUS = 0
                        PLUS = 0

                        FIS_PRC = 0
                        FIS_VALMOV = 0
                        FIS_FONDO = 0
                        FIS_RESIDUO = 0
                        FIS_MINUS = 0
                        FIS_PLUS = 0


                        'Si aggiungono nuove righe: 1 per i dati civilistici , l'altra per i dati fiscali 
                        DR_C = DS.DT_RegistroCespiti.NewDT_RegistroCespitiRow
                        DR_F = DS.DT_RegistroCespiti.NewDT_RegistroCespitiRow
                    End If
                        'DATI ANAGRAFICA CESPITE

                        '''ST_RIGA = "0"

                        ''''Verifico se il cespite ha solo la riga dell'acq o rilevazione iniziale
                        '''If DT.Rows(i).Item("id_cod_cespite") <> IdCodCespite Then

                        '''    Dim objStCespiti2 As New AgronicaCoreStampeDAL.Cespiti
                        '''    DT2 = objStCespiti2.LeggiNumRigheMovCespite(Anno, _
                        '''               DT.Rows(i).Item("id_cod_cespite"), _
                        '''                "", "", objParametri_Server)

                        '''    For j = 0 To DT2.Rows.Count - 1
                        '''        If DT2.Rows(j).Item("Conteggio") = 1 Then
                        '''            ST_RIGA = "1"
                        '''        Else
                        '''            ST_RIGA = "0"
                        '''        End If
                        '''    Next

                        '''End If


                        IdCodCespite = DT.Rows(i).Item("id_cod_cespite")
                        DesCespite = DT.Rows(i).Item("des_cespite")
                        IdCodCategoria = DT.Rows(i).Item("id_cod_categoria")
                        DesCategoria = DT.Rows(i).Item("des_categoria")
                        IndTipoBene = DT.Rows(i).Item("ind_tipo_bene")
                        IndNaturaBene = DT.Rows(i).Item("ind_natura_bene")

                        DATAMOV = DT.Rows(i).Item("dat_mov")
                        ANNOMOV = DT.Rows(i).Item("esercizio")
                        CAUMOV = DT.Rows(i).Item("cau_mov")
                        DES_CAUMOV = l_str_DesCausaleMovimento
                        QTAMOV = DT.Rows(i).Item("qta_N_mov")

                    If CAUMOV = 30 Then
                        If ANNOMOV = Anno Then
                            PRC = DT.Rows(i).Item("prc_amm")
                            VALMOV = DT.Rows(i).Item("val_qtaN_mov")
                        Else
                            PRC = 0
                            VALMOV = 0
                        End If
                    End If
                    If CAUMOV = 30 Or NumRigheCespite = 1 Then
                        FONDO = DT.Rows(i).Item("val_qtaN_fondo_amm")
                        RESIDUO = DT.Rows(i).Item("val_qtaN_residuo_amm")
                    End If

                    If CAUMOV = 32 Then
                        If ANNOMOV = Anno Then
                            FIS_PRC = DT.Rows(i).Item("fis_prc_amm")
                            FIS_VALMOV = DT.Rows(i).Item("fis_val_qtaN_mov")
                        Else
                            FIS_PRC = 0
                            FIS_VALMOV = 0
                        End If
                    End If
                        If CAUMOV = 32 Or NumRigheCespite = 1 Then
                            FIS_FONDO = DT.Rows(i).Item("fis_val_qtaN_fondo_amm")
                            FIS_RESIDUO = DT.Rows(i).Item("fis_val_qtaN_residuo_amm")
                        End If


                        If CAUMOV = 10 Or CAUMOV = 20 Then
                            COSTO = DT.Rows(i).Item("val_qtaN_costo_acq")
                            FIS_VALOREAMMORTIZZABILE = DT.Rows(i).Item("val_qtaN_mov")
                            ANNOACQ = DT.Rows(i).Item("esercizio")
                            'Non utilizzato
                            QTAACQ = DT.Rows(i).Item("qta_N_mov")
                            'If ST_RIGA = "1" Then
                            '    PRC = 0
                            '    VALMOV = 0
                            'End If
                        End If


                        'Se nell'anno di stampa si sono verificate alienazioni, recupero i valori
                        If CAUMOV = 40 Then
                            If Anno = DT.Rows(i).Item("esercizio") Then
                                VALALIENAZ = DT.Rows(i).Item("val_qtaN_mov")
                                QTAALIENAZ = DT.Rows(i).Item("qta_N_mov")
                                DATALIENAZ = DT.Rows(i).Item("dat_mov")
                                'Minus/Plus Civ
                                MINUS = DT.Rows(i).Item("val_qtaN_minus")
                                PLUS = DT.Rows(i).Item("val_qtaN_plus")
                                'Minus/Plus Fis
                                FIS_MINUS = DT.Rows(i).Item("fis_val_qtaN_minus")
                                FIS_PLUS = DT.Rows(i).Item("fis_val_qtaN_plus")
                            End If
                        End If

                        'If IdCodCespite = 150 And ST_RIGA = "1" Then
                        '    IdCodCespite = IdCodCespite
                        'End If

                        'IMPOSTO I DATI NEL DATASET DI STAMPA
                        DR_C.IdCodCespite = IdCodCespite
                        DR_F.IdCodCespite = IdCodCespite

                        DR_C.DesCespite = DesCespite
                        DR_F.DesCespite = DesCespite

                        DR_C.IdCodCategoria = IdCodCategoria
                        DR_F.IdCodCategoria = IdCodCategoria

                        DR_C.DesCategoria = DesCategoria
                        DR_F.DesCategoria = DesCategoria
                        If IndTipoBene = "0" Then
                            DR_C.IndTipoBene = "Materiale"
                            DR_F.IndTipoBene = "Materiale"
                        Else
                            DR_C.IndTipoBene = "Immateriale"
                            DR_F.IndTipoBene = "Immateriale"
                        End If
                        If IndNaturaBene = 0 Then
                            DR_C.IndNaturaBene = "Nuovo"
                            DR_F.IndNaturaBene = "Nuovo"
                        Else
                            DR_C.IndNaturaBene = "Usato"
                            DR_F.IndNaturaBene = "Usato"
                        End If


                        DR_C.DATAMOV = DATAMOV
                        DR_F.DATAMOV = DATAMOV

                        DR_C.ANNO = ANNOMOV
                        DR_F.ANNO = ANNOMOV

                        'N.B.:FISSI e vengono testati sul report
                        DR_C.CAUMOV = 30
                        DR_F.CAUMOV = 32

                        DR_C.QTAMOV = QTAMOV
                        DR_F.QTAMOV = QTAMOV

                        'DR_C.ST_RIGA = ST_RIGA

                        DR_C.COSTO = COSTO
                        DR_F.FIS_COSTO = COSTO

                        DR_F.FIS_VALOREAMMORTIZZABILE = FIS_VALOREAMMORTIZZABILE

                        DR_C.ANNOACQ = ANNOACQ
                        DR_F.ANNOACQ = ANNOACQ


                        DR_C.PRC = PRC
                        DR_C.VALMOV = VALMOV
                        DR_C.FONDO = FONDO
                        DR_C.RESIDUO = RESIDUO


                        DR_F.FIS_PRC = FIS_PRC
                        DR_F.FIS_VALMOV = FIS_VALMOV
                        DR_F.FIS_FONDO = FIS_FONDO
                        DR_F.FIS_RESIDUO = FIS_RESIDUO


                        DR_C.VALALIENAZ = VALALIENAZ
                        DR_F.FIS_VALALIENAZ = VALALIENAZ

                        DR_C.QTAALIENAZ = QTAALIENAZ
                        DR_F.QTAALIENAZ = QTAALIENAZ

                        DR_C.DATALIENAZ = DATALIENAZ
                        DR_F.DATALIENAZ = DATALIENAZ
                        'Minus/Plus Civ
                        DR_C.MINUS = MINUS
                        DR_C.PLUS = PLUS
                        'Minus/Plus Fis
                        DR_F.FIS_MINUS = FIS_MINUS
                        DR_F.FIS_PLUS = FIS_PLUS



                        j = j + 1

                        If j = NumRigheCespite Then
                            DS.DT_RegistroCespiti.Rows.Add(DR_C)
                            DS.DT_RegistroCespiti.Rows.Add(DR_F)
                        End If

                        'If ST_RIGA = "1" Then
                        '    DR_C.CAUMOV = 30
                        '    DR_C.VALMOV = 0
                        '    DS.DT_RegistroCespiti.Rows.Add(DR_C)

                        '    'DR = DS.DT_RegistroCespiti.NewDT_RegistroCespitiRow

                        '    'DR.CAUMOV = 32
                        '    'DR.FIS_VALMOV = 0
                        '    'DS.DT_RegistroCespiti.Rows.Add(DR)
                        'Else
                        '    DS.DT_RegistroCespiti.Rows.Add(DR_C)
                        'End If

                        '---------------------------

                Next

            End If 'controllo sul dt

        Catch ex As Exception
            Log_Errori += "- Valorizzazione dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        '####################################################

        Try

            'imposto il datset sul report
            Rpt_RegistroCespiti.SetDataSource(DS)


        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Try
            Rpt_RegistroCespiti.SetParameterValue("par_RagSoc_Impresa", Parametro_RagSoc_Impresa)
            Rpt_RegistroCespiti.SetParameterValue("par_Configurazione_Moduli", CStr(int_Configurazione_Moduli))
            'Anno,TipoStampa
            Rpt_RegistroCespiti.SetParameterValue("par_Anno", CStr(Anno))
            Rpt_RegistroCespiti.SetParameterValue("par_TipoStampa", CStr(TipoStampa))

        Catch ex As Exception
            Log_Errori += "- impostazione parametri: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub





End Class
